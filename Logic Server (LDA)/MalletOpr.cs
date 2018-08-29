using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    using System.IO;
    public class MalletOpr
    {
        private IExternal ext;
        private string outputFile;


        public MalletOpr(IExternal cmd)
        {
            ext = cmd;
        }


        //create mallet file
        public Result CreateMalletFile(string pathOfCurpus,string resultFile,List<string> keys)
        {
            string flag = GetFlags(keys);
            string command =String.Format(@"""bin\mallet import-dir --input {0} --output {1} {2}""",pathOfCurpus,resultFile,flag);
            Result result = ext.Run(command);
            return result;
        }

        //get parameters for running
        private string GetFlags(List<string> keys)
        {
            string flag = "";
            int n = keys.Count;
            int i = 0;
            while(i<n)
            {
                flag += "--" + keys[i];
                flag += " ";
                i++;
            }
            return flag;
        }


        //get all topics from result file
        public List<Topic> getTopics()
        {
            string path = this.ext.GetPath() + @"\" + this.outputFile;
            string[] lines = System.IO.File.ReadAllLines(path);
            List<Topic> topics = new List<Topic>();
            int i = 1; 
            foreach (string line in lines)
            {
                //create each topic
                Topic topic = new Topic(i);
                string[] data = line.Split('\t');
                string[] keys = data[2].Split(' ');

                for (int j=0; j<keys.Length-1; j++)
                {
                    topic.AddKey(keys[j]);
                    
                }

                topics.Add(topic);
                i++;
            }
            return topics;
        }


        //get topics for each file - only dominnate topics 
        public List<File> getTopicsForFiles()
        {
            Console.Write("enter first");
            string path = this.ext.GetPath() + @"\" + "topics.txt";
            string[] lines = System.IO.File.ReadAllLines(path);
            Console.Write("read topics");
            List<File> topics = new List<File>();
            File file;

            //run over files
            foreach (string line in lines)
            {
               file =  getTopicForFile(line);
               topics.Add(file);
                Console.Write("topics1");
            }
            return topics;
        }

    
        //run topics
        public Result RunTopics(int numTopic,string inputFile, string resultPath, string resultPath2)
        {
            this.outputFile = resultPath;
            string command = String.Format( @"""bin\mallet train-topics  --input {0} --num-topics {1} --output-state topic-state.gz --output-topic-keys {2} --output-doc-topics {3}""", inputFile,numTopic.ToString(),resultPath,resultPath2); ;
            Result result = ext.Run(command);
            return result;
        }

        //get topics and all other parameters for  files
        private File getTopicForFile(string data)
        {
            Console.Write("get for file");
            string[] mData = data.Split('\t');
            List<int> numTopics = new List<int>();
            string filename = Path.GetFileName(mData[1]);
            Console.Write("1");
            string path = mData[1].Remove(0,6);
            Console.Write("2");
            string[] lines = System.IO.File.ReadAllLines(path);
            int j = 0;
            string prev = "";
            if ((lines!=null)&&(lines.Length>0))
            {
                prev = lines[j];
                Console.Write("3");
                //eactract first line
                while ((prev.Length < 25) && (j<lines.Length))
                {
                    j++;
                    prev = lines[j];
                }
                Console.Write("4");
            }
            //create file
            File file = new File(filename,path,prev);
            float num;
            for (int i = 2; i < mData.Length; i++)
            {
                num = float.Parse(mData[i]);
                //if bigger 0.25
                if (num > 0.25)
                {
                    file.addTopic(i-1);
                    file.addPresentage(num);
                }

            }
            Console.Write("5");

            return file;
        }
    }

    }
