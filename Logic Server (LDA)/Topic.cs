using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Topic
    {
        public int id { get; set; }
        public List<string> keys { get; set; }

        public Topic(int id)
        {
            this.id = id;
            this.keys = new List<string>();
        }

        public void AddKey(string key)
        {
            this.keys.Add(key);
        }

    }
}
