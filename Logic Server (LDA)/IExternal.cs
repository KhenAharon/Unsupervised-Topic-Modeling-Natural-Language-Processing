using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum Result
    {
        Fail = 1,
        Success = 2,
        Error = 3

    };

    public interface IExternal
    {
        /*
       SetPath
           Set path
           working on spesfic to folder
       */
        void SetPath(string path);

        /*
      GetPath
      Get path
      working on spesfic to folder
  */
        string GetPath();

        /*
        Run
        run command.
        */
        Result Run(string command);
    }
}
