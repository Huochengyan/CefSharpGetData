using Browserform.Frm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browserform
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Trace.Listeners.Clear();
                Trace.Listeners.Add(new Log());

                //Application.Run(new FrmChatRecord("3422760224", Environment.CurrentDirectory));//1914506234  3422760224   10574944
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
               MyDB.LogInfo.SaveCollapseLog(ex.ToString());
            }
        }
    }
}

