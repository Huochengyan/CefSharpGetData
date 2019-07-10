using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browserform.Frm
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }
        static ChromiumWebBrowser web;
        private void Test_Load(object sender, EventArgs e)
        {
            //var re = new request();

            //接收和发送的http消息
            //re.msg += Re_msg;
            //re.msg2 += Re_msg2;
            web = new ChromiumWebBrowser("https://www.baidu.com");

            web.Dock = DockStyle.Fill;
            //web.RequestHandler = re;
            web.FrameLoadEnd += Web_FrameLoadEnd;
            this.groupBox1.Controls.Add(web);
        }
        private void Web_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();



        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                Convert.ToInt32("asdf");
            }
            catch (Exception ex)
            {
                MethodBase method = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod();
                CommonTools.ExceptionLogInfo.SaveExceptionInfo(method.ReflectedType.FullName, method.Name, ex.ToString());
            }

            return;

            Task<string> htmlSource = web.GetSourceAsync();
            
            
            Clipboard.SetText(htmlSource.Result.ToString());
            




         }
        
    }
    class China {
        public string name = "";
        protected string Visitor = "";

        private void fff()
        { 
            //1.获得是否选择了继续执行 赋值给flag

            bool flag = false; //定义变量接收是否继续的
            if (flag)
            {
                //继续执行代码
            }
            else {
                //不执行跳出
            }
            
        }
      


    }
    
}
