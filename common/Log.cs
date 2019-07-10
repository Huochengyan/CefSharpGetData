using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browserform
{
    class Log : TraceListener
    {
        string logPath = System.Environment.CurrentDirectory + "/log/formlog.txt";
        public override void Write(string message)
        {
            File.AppendAllText(logPath, message);
        }

        public override void WriteLine(string message)
        {
            File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ") + message + Environment.NewLine);
        }
    }
}
