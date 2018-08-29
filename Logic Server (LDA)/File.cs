using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class File
    {
        public string name { get; set; }
        public List<int> topics { get; set; }
        public List<float> percentage { get; set; }
        public string preview { get; set; }
        public string path { get; set; }

        public File(string name , string path , string preview)
       {
            this.name = name;
            topics = new List<int>();
            this.path = path;
            this.preview = preview;
            percentage = new List<float>();

        }

        public void addTopic(int id)
       {
            topics.Add(id);
       }

        public void addPresentage(float num)
        {
            percentage.Add(num);
        }

    }
}
