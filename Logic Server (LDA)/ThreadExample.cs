using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ThreadExample
    {
        // The ThreadProc method is called when the thread starts.
        // It loops ten times, writing to the console and yielding 
        // the rest of its time slice each time, and then ends.
        //  public static Socket sock2;// = Program.sock.Accept();

        public static void ThreadProc()
        {
            // IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            //Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //sock.Bind(ipep);
            //sock.Listen(0);
            try
            {
                while (true)
                {
                    Program.sock2 = Program.sock.Accept();
                    byte[] msg1 = Encoding.ASCII.GetBytes("no");
                    try
                    {
                        int bytesSent = Program.sock2.Send(msg1);
                        Program.sock2.Close();
                        System.Threading.Thread.Sleep(5000);
                    }
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                // sock2.Shutdown(SocketShutdown.Both);
                Program.sock2.Close();
            }
        }
    }
}
