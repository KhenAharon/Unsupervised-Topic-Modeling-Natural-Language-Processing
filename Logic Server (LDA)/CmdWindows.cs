using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Server
{
    public class CmdWindows : IExternal
    {
        private string path;

        public CmdWindows(string path)
        {
            this.path = path;
        }

        //set path for running
        public void SetPath(string path)
        {
            this.path = path;
        }

        //get path for running
        public String GetPath()
        {
            return this.path;
        }

        //run cmd
        public Result Run(string command)
        {
          //create cmd process
            ProcessStartInfo ProcessInfo;
            Process Process;
            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " +  command);
            ProcessInfo.WorkingDirectory = @"c:\Mallet";
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;
            ProcessInfo.WindowStyle=ProcessWindowStyle.Hidden;
            Process = Process.Start(ProcessInfo);

            return Result.Success;
        }
    }
}
