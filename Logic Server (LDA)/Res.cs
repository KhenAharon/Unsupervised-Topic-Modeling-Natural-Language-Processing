using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Res
    {
        public string json1 { get; set; }
        //public string json2 { get; set; }

        public Res(string json1/*,string json2*/)
        {
            this.json1 = json1;
          //  this.json2 = json2;
        }
    }

    public class Res2
    {
        public string json1 { get; set; }
        public string json2 { get; set; }

        public Res2(string json1,string json2)
        {
            this.json1 = json1;
            this.json2 = json2;
        }
    }
}
