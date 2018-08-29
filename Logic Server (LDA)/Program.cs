using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Server
{
    public enum Types
    {
        File,
        Topic
    }

    class Program
    {
        public static Socket sock = null;
        public static Socket sock2 = null;

        public static int WriteToClient(byte[] bytes, Socket sender)
        {
            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                int bytesSent = sender.Send(bytes);
                return bytesSent;
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return 0;

            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
                return 0;

            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                return 0;

            }

        }

        static void Main(string[] args)
        {

            // Establish the remote endpoint for the socket.  
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(ipep);
            sock.Listen(0);
            while (true)
            {
                sock2 = sock.Accept();
                //read requset from socket
                byte[] msg = new byte[1024];
                int recv = sock2.Receive(msg);
                string input = Encoding.ASCII.GetString(msg, 0, recv);
                Console.Write("get : {0}", input);
                Req req = JsonConvert.DeserializeObject<Req>(input);
                //initalized paramters - should be data from client
                string malletPath = @"c:\mallet";
                string dataPath = req.path;
                string resultMalletFile = "keys.mallet";
                string resultTxtFile = "keys.txt";
                string DataOfFile = "topics.txt";
                int numTopics = req.topics;
                List<string> flags = req.flags;
                //begin - extract zip files
                dataPath = ExtractFolder(dataPath);
                Thread t = null;
                // if ruj the parser
                if (req.tagger == 1)
                {
                    sock2.Close();
                    t = new Thread(new ThreadStart(ThreadExample.ThreadProc));
                    t.Start();
                    toknizer(dataPath);
                    System.Threading.Thread.Sleep(30000);
                    parser2(dataPath);
                }
                IExternal cmd = new CmdWindows(malletPath);
                MalletOpr mallet = new MalletOpr(cmd);

                //run - crate mallet file
                mallet.CreateMalletFile(dataPath, resultMalletFile, flags);
                System.Threading.Thread.Sleep(10000);
                //run LDA
                mallet.RunTopics(numTopics, resultMalletFile, resultTxtFile, DataOfFile);
                System.Threading.Thread.Sleep(88000);
                //convert results to Json
                List<Topic> getTopics = mallet.getTopics();
                string jsonString = JsonConvert.SerializeObject(getTopics);
                byte[] msg1 = Encoding.UTF8.GetBytes(jsonString);
                //write to socket         
                if (t != null)
                {
                    t.Abort();
                    System.Threading.Thread.Sleep(8000);
                    sock2 = sock.Accept();
                }
                int bytes = WriteToClient(msg1, sock2);
                // Release the socket.  
                sock2.Shutdown(SocketShutdown.Both);
                sock2.Close();
                sock2 = sock.Accept();
                List<File> files = mallet.getTopicsForFiles();
                string jsonString2 = JsonConvert.SerializeObject(files);
                byte[] msg2 = Encoding.UTF8.GetBytes(jsonString2);
                int bytes2 = WriteToClient(msg2, sock2);
                sock2.Close();
            }
        }
        public static string ExtractFolder(string path)
        {
            string filename = Path.GetFileName(path);
            string[] name = filename.Split('.');
            string tempPath = Path.Combine(@"C:\mallet\sample-data\web", name[0]);
            if (Directory.Exists(tempPath))
            {
                deleteAllFiles(tempPath);
                Directory.Delete(tempPath, true);
            }

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //Put the path of installed winrar.exe
            proc.StartInfo.FileName = @"C:\Program Files (x86)\WinRAR\winRAR.exe";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.EnableRaisingEvents = true;
            string src = path;
            string des = @"C:\mallet\sample-data\web\";
            proc.StartInfo.Arguments = String.Format("x -o+ \"{0}\" \"{1}\"", src, des);
            proc.Start();
            System.Threading.Thread.Sleep(10000);
            return Path.Combine(des,name[0]);

        }

        private static void deleteAllFiles(string path)
        {
            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile))
                {
                    System.IO.File.Delete(fileName);
                }
                System.IO.Directory.Delete(dirFile);
            }
        }

        public static void toknizer(string path)
        {
            ProcessStartInfo ProcessInfo;
            Process Process;
            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile))
                {
                    string name = Path.GetFileName(fileName);
                    //string file = Path.Combine(path, fileName);
                    string file2 = Path.Combine(path, dirFile, ("a" + name + ".txt"));
                    //   string command = string.Format("cmd.exe", " / K " + "hebtokenizer.py < {0} > {1}", file, file2);
                    if (name[0] != 'a')
                    {
                        string command = String.Format(@"""python hebtokenizer.py < {0} > {1}""", fileName, file2);

                        ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + command);
                        ProcessInfo.WorkingDirectory = @"c:\Python27";
                        ProcessInfo.CreateNoWindow = true;
                        ProcessInfo.UseShellExecute = true;
                        ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process = Process.Start(ProcessInfo);
                    }
                }
            }

            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile))
                {
                    string name = Path.GetFileName(fileName);
                    if (name[0] != 'a')
                    {
                        try
                        {
                            System.IO.File.Delete(fileName);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine("not del {0}", fileName);
                        }
                    }
                }


            }
        }

        //parser 
        public static void parser2(string path)
        {
            ProcessStartInfo ProcessInfo;
            Process Process;
            //run over all files
            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile))
                {

                    string name = Path.GetFileName(fileName);
                    string[] words = name.Split('.');
                    string file2 = Path.Combine(path, dirFile, ("c" + words[0] + ".txt"));
                    //if new file run parser
                    if (name[0] != 'c')
                    {
                        string command = String.Format(@"""python C:\Users\matan\Downloads\hebdepparser\parse.py < {0} > {1}""", fileName, file2);

                        ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + command);
                        ProcessInfo.WorkingDirectory = @"c:\Python27";
                        ProcessInfo.CreateNoWindow = true;
                        ProcessInfo.UseShellExecute = true;
                        ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process = Process.Start(ProcessInfo);
                        System.Threading.Thread.Sleep(100);

                    }
                }
            }

            System.Threading.Thread.Sleep(30000);

            //run over all directories
            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile))
                {
                    string name = Path.GetFileName(fileName);
                    if ((name[0] != 'c') && (name[0] != 'd'))
                    {
                        try
                        {
                            System.IO.File.Delete(fileName);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine("not del {0}", fileName);
                        }
                    }
                    else
                    {
                        StreamWriter writer = null;
                        string[] words = name.Split('.');
                        string file2 = Path.Combine(path, dirFile, ("d" + words[0] + ".txt"));
                        string[] lines = System.IO.File.ReadAllLines(fileName);
                        writer = new StreamWriter(file2, true);
                        foreach (string line in lines)
                        {
                            if (!(line.Equals("")))
                            {
                                string[] newLine = line.Split('\t');
                                string toWrite = newLine[1];
                                writer.WriteLine(toWrite);
                            }
                        }
                        writer.Close();
                    }
                }
            }

            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile))
                {
                    string name = Path.GetFileName(fileName);
                    if (name[0] != 'd')
                    {
                        try
                        {
                            System.IO.File.Delete(fileName);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine("not del {0}", fileName);
                        }
                    }
                }
            }
        }
        static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //evtdata.Data has the output data
            //use it, display it, or discard it
        }
    }
}