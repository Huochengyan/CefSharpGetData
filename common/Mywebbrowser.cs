using mshtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MyDB;
using System.Drawing;

namespace Browserform
{
    public class Mywebbrowser
    {
        private System.Timers.Timer t1 = new System.Timers.Timer(1000);  //用于去获取二维码
        private System.Timers.Timer t2 = new System.Timers.Timer(1000);   //用于绑定click事件
        private System.Timers.Timer RecordTimer = new System.Timers.Timer(1000);   //聊天记录
        private System.Timers.Timer RobotTimer = new System.Timers.Timer(1000);  //识别新消息（左侧红色图标）
        private System.Timers.Timer AddTimer = new System.Timers.Timer(100);
        private WebBrowser browser;
        private const string URL = "https://wx.qq.com/?lang=zh_CN";
        //设备号，用户传输二维码
        public string devNum;
        //用于设置头像
        public int uuid;
        //当前微信的昵称，用于保存消息记录
        public string MyNickName="";
        public string FriendNickName;//在左侧点击的时候，切换
                                     //二维码图片暂时保存在本地，部署时保存到临时文件夹，且定期清理
        System.Timers.Timer SaveTimer = new System.Timers.Timer(500);


        public WebBrowser CreateBrowser()
        {
            try
            {
                browser = new WebBrowser();
                browser.Navigate(URL);
                browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
                browser.ScriptErrorsSuppressed = true;
                //判断二维码加载完成
                t1.Elapsed += new ElapsedEventHandler(checkQRcode);
                t1.Start();

                //获取聊天面板上所有消息 1.保存多条 2.保存单挑
                SaveTimer.Elapsed += new ElapsedEventHandler(GetAllMessage);
                RecordTimer.Elapsed += new ElapsedEventHandler(ScanRecord);//页面完成登录后，立即启动消息扫描线程来记录消息。
                RecordTimer.Start();
                AddTimer.Elapsed += new ElapsedEventHandler(AddScript);
                AddTimer.Start();
                return browser;
            }
            catch (Exception ex)
            {
                return browser;
            }
        }

       

        #region 发送进程消息

        /// <summary>
        /// 发送命令消息
        /// </summary>
        /// <param name="ThreadName">执行的进程名</param>
        /// <param name="wpara">命令:
        /// 100:头像获取
        /// 200:新消息提示
        /// </param>
        /// <param name="lpara"></param>
        private bool Send(string ThreadName, int wpara, int lpara, uint flag)    // 0x80F0   0x80F1
        {
            Process[] arrPro = Process.GetProcessesByName(ThreadName);
            bool re = false;

            for (int i = 0; i < arrPro.Length; i++)
            {
                //向目标进程的主线程发送消息
                re = PostThreadMessage(arrPro[i].Threads[0].Id, flag, (IntPtr)wpara, (IntPtr)lpara);
            }
            return re;
        }
        //测试
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, IntPtr wParam, IntPtr lParam);


        #endregion



        #region  增加外层ui解决webbrowser bug
        public void AddScript(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (browser.InvokeRequired)
                {
                    browser.BeginInvoke(new Action(delegate
                    {
                        if (browser.Document == null || browser.Document.DomDocument == null || ((HTMLDocument)browser.Document.DomDocument).documentElement == null)
                            return;
                        if (browser.Document.GetElementById("ld") != null)
                            return;
                        foreach (HtmlElement i in browser.Document.GetElementsByTagName("a"))
                        {
                            if (i.GetAttribute("classname") == "button_primary ng-scope" || i.GetAttribute("classname") == "button_default button_primary")
                            {
                                AddTimer.Stop();
                                AddJavaScript();
                                HtmlElement j = browser.Document.GetElementById("ld");
                                j.Click += delegate
                                {
                                    ccc();
                                    AddTimer.Start();
                                };
                            }
                        }

                    }));
                }
                else
                {
                    //宿主异常结束时，在这里结束
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void ccc()
        {
            string scriptline01 = @"function cccScript() {";
            string scriptline02 = @"$('.button_primary').click() } ";
            string scriptline03 = @"cccScript();";

            string strScript = scriptline01 + scriptline02 + scriptline03;
            IHTMLWindow2 win = (IHTMLWindow2)browser.Document.Window.DomWindow;
            win.execScript(strScript, "Javascript");
        }

        public void AddJavaScript()
        {
            string scriptline01 = @"function AddScript() {";
            string scriptline02 = @"$('.dialog_ft').parent().css({position:'relative'}); ";
            string scriptline04 = @"$('.dialog_ft').append(""<div id='ld' style ='width:190px;height:42px;position: absolute;z-index:500;margin-top:-42px; margin-left: 146px; '></div >"")  ;}";
            string scriptline05 = @"AddScript();";

            string strScript = scriptline01 + scriptline02 + scriptline04 + scriptline05;
            IHTMLWindow2 win = (IHTMLWindow2)browser.Document.Window.DomWindow;
            win.execScript(strScript, "Javascript");
        }

        #endregion
        //执行发送脚本
        public void SendJSMessage(string msg)
        {
            string reply = IsContain(msg);
            if (!reply.Equals(""))
            {
                send(reply);
            }
        }


        //判断key包含
        private string IsContain(string msg)
        {
            //获取数据库所有key
            List<word> allWord= wordOperation.FindAll();

            //回复内容         //优先级(int全局变量自动初始化为0，局部变量未定义)
            string reply = ""; int pri = 100;

            //遍历所有包含的key。
            foreach (var m in allWord)
            {   //存在该关键字
                if (Regex.Match(msg,m.KeyWord).ToString()!="")
                {
                    //判断优先级。选择优先级最高的。0~100 0最大
                    if (m.Priority<pri)
                    {
                        reply = m.Reply;
                        pri = m.Priority;
                    }          
                }
            }
            return reply;
        }


        //回复指定的字符串
        public void send(string content)
        {
            string scriptline01 = @"function SendMessage() {";
            string scriptline02 = @"    angular.element('#editArea').scope().editAreaCtn = '"+content+"'; ";
            string scriptline03 = @"    angular.element('#editArea').scope().sendTextMessage(); ";
            string scriptline04 = @"      ;}";
            string scriptline05 = @"SendMessage();";

            string strScript = scriptline01 + scriptline02 + scriptline03 + scriptline04 + scriptline05;
            IHTMLWindow2 win = (IHTMLWindow2)browser.Document.Window.DomWindow;
            win.execScript(strScript, "Javascript");
        }


        public int CheckAndSend(string content)  //title_name ng-binding
        {
            try
            {

                foreach (HtmlElement i in browser.Document.GetElementsByTagName("a"))
                {
                    if (i.GetAttribute("classname") == "title_name ng-binding")
                    {
                        if (i.InnerText == null)
                        {
                            return -1;
                        }
                        else
                        {
                            send(content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            return 0;
        }
    
        //刷新时初始化
        public void InitinalBrowser()
        {
            try
            {
                t1.Start();
                t2.Start();
                browser.ScriptErrorsSuppressed = true;
                RecordTimer.Start();
                //AddTimer.Elapsed += new ElapsedEventHandler(AddScript);
                //AddTimer.Start();
            }
            catch(Exception ex)
            {
            }
        }


        //退出当前微信
        public void ExitWeChat()
        {
            try
            {
                foreach (HtmlElement i in browser.Document.GetElementsByTagName("i"))
                {
                    if (i.GetAttribute("classname") == "web_wechat_add")
                    { 
                        i.Parent.InvokeMember("Click");
                    }
                }
                foreach (HtmlElement i in browser.Document.GetElementsByTagName("a"))
                {
                    if (i.GetAttribute("title") == "退出")
                    {
                        i.InvokeMember("Click");
                    }
                }
            }
            catch(Exception ex)
            {
               //Trace.TraceError("退出微信失败:"+ex.Message);
            }
        }

        string text1 = "";
        public void checkQRcode(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string filePath;
                //Trace.TraceInformation("进入CheckQRcode方法");
                if (browser.InvokeRequired)
                {
                    browser.BeginInvoke(new Action(delegate
                    {
                        if (browser.Document == null || browser.Document.DomDocument == null || ((HTMLDocument)browser.Document.DomDocument).documentElement == null)
                            return;
                        text1 = ((HTMLDocument)browser.Document.DomDocument).documentElement.outerHTML;

                    }));
                }
                else
                {
                    //宿主异常结束时，在这里结束
                    Application.Exit();
                }
                if (text1.Contains("https://login.weixin.qq.com/qrcode"))
                {
                    t1.Stop();
                    int souceIndex = text1.IndexOf("https://login.weixin.qq.com/qrcode");
                    char[] dd = new char[47];
                    text1.CopyTo(souceIndex, dd, 0, 47);
                    string url1 = new string(dd);
                    HttpWebRequest request = HttpWebRequest.Create(url1) as HttpWebRequest;
                    request.Method = "GET";
                    // HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    HttpWebResponse response = null;
                    using (WebResponse wr = request.GetResponse())
                    {
                        response = wr as HttpWebResponse;
                        using (Stream stream = response.GetResponseStream())
                        {
                            //当前时间作为文件名
                            filePath = OperationRecord.QRFile() + @"/" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
                            using (Stream fsStream = new FileStream(filePath, FileMode.Create))
                            {
                                stream.CopyTo(fsStream);
                            }
                        }
                    }
                    if (devNum != null)
                    {
                        //像服务器传输二维码
                        Cloud.Login(devNum.TrimStart(), filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceError("获取二维码并上传图片失败："+ex.Message);
            }
        }


        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(browser.Url.ToString());
                if (browser.ReadyState == WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                t2.Elapsed += new System.Timers.ElapsedEventHandler(theout);
                t2.Start();

                //检测新消息。
                //1.人工模式下，发送消息提示到界面
                //2.机器模式下，需要识别并自动回复
                RobotTimer.Elapsed += new System.Timers.ElapsedEventHandler(recogiseMessage);
                RobotTimer.Start();
            }
            catch (Exception ex)
            {
                //Trace.TraceError("未知异常:"+ex.Message);
            }
        }

        bool flag = true;
        string text = "";
        HtmlElementCollection divHtml = null;
        HtmlElementCollection aHtml = null;
        //用于绑定左侧click事件
        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (browser.InvokeRequired)
                {
                    browser.BeginInvoke(new Action(delegate
                {
                    if (browser.Document == null || browser.Document.DomDocument == null || ((mshtml.HTMLDocument)browser.Document.DomDocument).documentElement == null)
                        return;
                    //这里判断是否加载到了好友列表
                    text = ((mshtml.HTMLDocument)browser.Document.DomDocument).documentElement.outerHTML;
                    divHtml = browser.Document.GetElementsByTagName("div");
                    aHtml = browser.Document.GetElementsByTagName("a");
                    if (text.Contains("attr ng-binding") && divHtml != null)
                    {
                        if (flag == false)
                        {
                            flag = true;
                            return;
                        }
                        flag = false;
                        t2.Stop();
                        foreach (HtmlElement i in divHtml)
                        {
                            if (i.GetAttribute("classname") == "scroll-wrapper chat_list scrollbar-dynamic")
                            {
                                i.Click += delegate
                                {
                                    LeftClickDeal();
                                };
                            }
                        }

                        //获取我的nickname
                        if (MyNickName == "")
                        {
                            foreach (HtmlElement i in browser.Document.GetElementsByTagName("span"))
                            {
                                if (i.GetAttribute("ng-bind-html") == "account.NickName")
                                {
                                    MyNickName = i.InnerText;
                                    //此处存储微信昵称
                                    //MessageBox.Show("uuid_421:" + uuid.ToString() + MyNickName);
                                    //CommonTools.LoginDir.AddNickname(uuid.ToString(),MyNickName);
                                }
                            }
                            //获取头像
                            //此处有bug 20170526
                            GetIcon(uuid);

                            
                        }
                    }
                }));
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceError("theout函数处理异常：" + ex.Message);
            }
        }

        //获取用户头像
        private void GetIcon(int uuid)
        {
                string url = browser.Document.GetElementsByTagName("img")[0].GetAttribute("src") as string + "&type=big";
                CookieContainer myCookieContainer = new CookieContainer();
                string cookieStr = browser.Document.Cookie;

                string[] cookstr = cookieStr.Split(';');
                foreach (string str in cookstr)
                {
                    string[] cookieNameValue = str.Split('=');
                    Cookie ck = new Cookie(cookieNameValue[0].Trim().ToString(), cookieNameValue[1].Trim().ToString(), "", "wx2.qq.com");
                    myCookieContainer.Add(ck);
                }

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = myCookieContainer;
                request.Method = "GET";
                HttpWebResponse response = null;
                using (WebResponse wr = request.GetResponse())
                {
                    response = wr as HttpWebResponse;
                    using (Stream stream = response.GetResponseStream())
                    {
                        //当前browser唯一标识uuid作为文件名
                        if (!Directory.Exists(Environment.CurrentDirectory + "/HeaderImages/"))
                        {
                            Directory.CreateDirectory(Environment.CurrentDirectory + "/HeaderImages/");
                        }
                    try
                    {
                        string filePath = Environment.CurrentDirectory + "/HeaderImages/" + uuid + ".jpg";
                        Bitmap map = new Bitmap((Image)new Bitmap(stream), 48, 48);

                        map.Save(filePath);
                        map=null;                   
                    }
                    catch (Exception ex)
                    {
                       // Console.WriteLine(ex.ToString());
                    }
                    }
                }
                //发送消息 1.VS代码：
                // Send("vshost32.exe", 111, uuid, 0x80F0);
                Send("WinForm", 111, uuid, 0x80F0);
        }


        //左侧点击事件
        //1.获取当前好友NickName
        //2.保存面板上的所有消息
        public void LeftClickDeal()
        {
            foreach (HtmlElement pp in browser.Document.GetElementsByTagName("a")) //获取好友昵称
            {
                if (pp.GetAttribute("classname") == "title_name ng-binding")
                {
                    FriendNickName = pp.InnerText;
                }
            }
            SaveTimer.Start();
            //Trace.TraceInformation("点击左侧事件触发，开始保存界面所有消息");
        }


        string LastTime;
        //获取聊天面板上所有消息,，并保存
        public void GetAllMessage(object source, ElapsedEventArgs e)
        {
            try
            {
                if (browser.InvokeRequired)
                {
                    browser.BeginInvoke(new Action(delegate
                    {
                        if (browser.Document == null || browser.Document.DomDocument == null || ((mshtml.HTMLDocument)browser.Document.DomDocument).documentElement == null)
                            return;
                        Record AllMessage = new Record();
                        AllMessage.MessageList = new List<MessageInfo>();

                        //聊天界面是否有消息？
                        foreach (HtmlElement i in browser.Document.GetElementsByTagName("div"))
                        {
                            if (i.GetAttribute("ng-repeat") == "message in chatContent")  //发现消息并处理
                            {
                                if (i.GetElementsByTagName("pre").Count == 0)
                                    continue;
                                AllMessage.MyNick = MyNickName;
                                AllMessage.ChatNick = FriendNickName;
                                MessageInfo amessage = new MessageInfo();
                                amessage.Content = i.GetElementsByTagName("pre")[0].InnerText;  //消息内容
                                foreach (HtmlElement mes in i.GetElementsByTagName("div"))
                                {
                                    if (mes.GetAttribute("classname") == "message ng-scope me")
                                    {
                                        amessage.MessageType = 0; //我发出的消息
                                    }
                                    else if (mes.GetAttribute("classname") == "message ng-scope you")
                                    {
                                        amessage.MessageType = 1;  //我接收的消息
                                        foreach (HtmlElement ee in mes.GetElementsByTagName("img"))
                                        {
                                            if (ee.GetAttribute("classname") == "avatar")
                                            {
                                                amessage.FriendNick = ee.GetAttribute("title").ToString();
                                            }
                                        }
                                    }
                                }
                                if (i.GetElementsByTagName("span").Count != 0)//时间元素为空，则取上次消息的时间
                                {
                                    if (i.GetElementsByTagName("span")[0].InnerText.Length == 5&& !isExists(i.GetElementsByTagName("span")[0].InnerText)) //防止附件大小被误识别为日期。
                                    {
                                        amessage.time = i.GetElementsByTagName("span")[0].InnerText;
                                        LastTime = amessage.time;
                                    }
                                }
                                else
                                {
                                    amessage.time = LastTime;
                                }
                                //把消息加入队列。如果此时切换了friend，应立即保存？
                                AllMessage.MessageList.Add(amessage);
                            }
                        }
                        //这里保存
                        SaveTimer.Stop();
                        OperationRecord.WriteMutiRecord(AllMessage);
                    }));
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceError("GetAllMessage处理异常:" + ex.Message);
            }
        }

        //判断字符串里是否存在英文
        public bool isExists(string str)
        {
            return Regex.Matches(str, "[a-zA-Z]").Count > 0;
        }

        //关闭自动回复模式
        public void CloseRobot()
        {
            try
            {
                RobotStyle = false;
                RobotTimer.Stop();
            }
            catch (Exception ex)
            {
                
            }
        }

        public bool RobotStyle = false;

        //开启自动回复模式
        public void StartRobot()
        {
            RobotStyle = true;
        }

        /// <summary>
        /// 识别左上角红色小图标（标志着新消息），并自动回复。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recogiseMessage(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (browser.InvokeRequired)
                {
                    browser.BeginInvoke(new Action(delegate
                {
                    if (browser.Document == null || browser.Document.DomDocument == null || ((mshtml.HTMLDocument)browser.Document.DomDocument).documentElement == null)
                        return;
                    foreach (HtmlElement i in browser.Document.GetElementsByTagName("i"))
                    {
                        if (i.GetAttribute("classname") == "icon web_wechat_reddot_middle ng-binding ng-scope")  //"chat_item slide-left ng-scope")
                        {
                            //新消息提醒
                            Send("WinForm", 222, uuid, 0x80F0);

                            if (RobotStyle==true)
                            i.Parent.InvokeMember("Click");                      
                        }
                    }         
                }));
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceError("recogiseMessage操作失败:"+ex.Message); 
            }
        }


        MessageInfo LastFriendMessage;  //当前聊天界面，最后一条好友发送的消息//在左侧点击事件里面初始化
        MessageInfo LastMyMessage;
        string time1;
        /// <summary>
        /// 最后一条好友消息和上次不一样，保存
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void ScanRecord(object source, ElapsedEventArgs e)
        {
            try
            {
                if (browser.InvokeRequired)
                {
                    //扫描左侧新消息
                    browser.BeginInvoke(new Action(delegate
                    {
                        if (browser.Document == null || browser.Document.DomDocument == null || ((HTMLDocument)browser.Document.DomDocument).documentElement == null)
                            return;
                        string text = ((HTMLDocument)browser.Document.DomDocument).documentElement.outerHTML;
                        int j = 0;
                        int type = 0;
                        MessageInfo CurrentLastMessage = new MessageInfo();
                        String currentFriendNick="";
                        string uniqueFriend = FriendNickName;//防止在保存最后一条消息的时候，切换导致的好友名称混乱
                        
                        //判断新消息到来的最后一条消息，1.是对方发的。2.跟上条不一样
                        foreach (HtmlElement i in browser.Document.GetElementsByTagName("div"))
                        {
                            if (i.GetAttribute("ng-repeat") == "message in chatContent")  //发现消息并处理
                            {
                                    j++;
                                //获取最后一条是谁发的
                                foreach (HtmlElement mes in i.GetElementsByTagName("div"))
                                {
                                    if (mes.GetAttribute("classname") == "message ng-scope me")
                                    {
                                        type = 0; //我发出的消息
     
                                    }
                                    else if (mes.GetAttribute("classname") == "message ng-scope you")
                                    {
                                        type = 1;  //我接收的消息
                                        foreach (HtmlElement ee in mes.GetElementsByTagName("img"))
                                        {
                                            if (ee.GetAttribute("classname") == "avatar")
                                            {
                                                currentFriendNick = ee.GetAttribute("title").ToString();
                                            }
                                        }              
                                    }
                                }
                                if (i.GetElementsByTagName("span").Count != 0)//时间元素为空，则取上次消息的时间
                                {
                                    if (i.GetElementsByTagName("span")[0].InnerText.Length == 5 && !isExists(i.GetElementsByTagName("span")[0].InnerText))
                                    {
                                        //去掉秒
                                        time1 = i.GetElementsByTagName("span")[0].InnerText;
                                    }
                                }
                            }
                        }
                            
                        if (browser.Document.GetElementsByTagName("pre").Count == 1)  //可能是好友添加信息
                            return;
                        if (j == 0)  //没有消息
                            return;
                        if (browser.Document.GetElementsByTagName("pre").Count - 1 != j)
                        {
                            if (RobotStyle == true && type == 1)
                            {
                             //   MessageBox.Show("特殊情况测试");
                              //  SendJSMessage("");
                                return;
                            }      
                        }
                        //if (browser.Document.GetElementsByTagName("pre")[j - 1].InnerText == null)
                        //    return;
                        int count = browser.Document.GetElementsByTagName("pre").Count;
                        MessageInfo nowmes = new MessageInfo
                        {
                            Content = browser.Document.GetElementsByTagName("pre")[count - 2].InnerText,
                            MessageType = type,
                            time = time1,
                            FriendNick=currentFriendNick
                        };
                        Trace.TraceInformation("最后一条信息："+nowmes.Content+","+nowmes.time);

                        if (type == 0)  //我发送的消息，则保存
                        {
                            if (!uniqueFriend.Equals(FriendNickName))
                                return;
                            if (CompareMessage(nowmes, LastMyMessage)) 
                            {
                                Record d = new Record();
                                d.MessageList = new List<MessageInfo>();
                                d.MyNick = MyNickName;
                             
                                d.ChatNick = uniqueFriend;
                                d.MessageList.Add(nowmes);
                                LastMyMessage = nowmes;
                                OperationRecord.WriteRecord(d);
                            }
                            return;
                        }
                        if (CompareMessage(nowmes, LastFriendMessage))
                        {
                            Record d = new Record();
                            d.MessageList = new List<MessageInfo>();
                            d.MyNick = MyNickName;
                            if (!uniqueFriend.Equals(FriendNickName))
                                return;
                            d.ChatNick = uniqueFriend;
                            d.MessageList.Add(nowmes);
                            LastFriendMessage = nowmes;
                            OperationRecord.WriteRecord(d);
                        }
                        if (RobotStyle == true)
                        {
                            SendJSMessage(nowmes.Content);
                        }
                    }));
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceError("ScanRecord操作异常:"+ex.Message);    
            }
        }

        //为true则保存
        public bool CompareMessage(MessageInfo mes1,MessageInfo mes2)
        {
            try
            {
                if (mes2 == null)
                    return true;
                if (mes2.Content == null)
                    return true;

                if (mes1.Content != mes2.Content || mes1.time != mes2.time)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
                return false;
            }
        }
    }
}
                    





