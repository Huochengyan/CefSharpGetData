using Browserform.common;
using CefSharp;
using CefSharp.WinForms;
using Microsoft.VisualBasic;
using MyDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browserform
{
    public partial class Form1 : Form
    {
        #region 初始参数 信息
        public static Form1 Instance;

        public static string RootPath = System.Environment.CurrentDirectory;

        //关于该进程的唯一标识
        public static int uuid = 0;
        //public static Mywebbrowser mywebbrowser = new Mywebbrowser();
        //public static WebBrowser browser;
        //从treeview里面打开tab页时，有设备号参数
        public static string DevNum;

        //内存中的好友列表，用于聊天记录查询
        private static Root friendsRoot = new Root();

        /// <summary>
        ///微信昵称
        /// </summary>
        private string NickName = "";

        //微信号唯一标识
        private static string wxUin = "";

        //自己的微信UserName
        private static string MyUserName;

        //机器模式
        private static bool IsRobot = false;

        private static CookieContainer myCookieContainer = new CookieContainer();

        //登录相关信息
        private LoginRedirectResult loginRedirectResult;
        /// <summary>
        /// 自己信息项,主要针对群消息存储
        /// </summary>
        private MyDB.Model.MyWxInfo mywxinfo = new MyDB.Model.MyWxInfo();
        private static string Skey = "";
        /// <summary>
        /// 当前微信新老标识
        /// </summary>
        private static int WxorWx2 = 1;

        #endregion

        #region  进程通讯模块
        ///// 100:生成UUID
        ///// 200:刷新页面
        ///// 300:退出登录
        ///// 400:自动回复
        ///// 500:聊天记录
        ///// 600:退出当前微信
        ///// 700:退出当前进程
        ///// <summary>
        ///// 定义结构体
        ///// </summary>
        class MsgFilter : IMessageFilter
        {

            #region  可快捷回复等功能
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == 0x80F2)
                {
                    if (uuid == m.LParam.ToInt32())
                    {
                        int id = m.WParam.ToInt32();
                        string words = MyDB.quickWordsOperation.findById(id).Words;
                        CheckAndSend(words);
                    }
                    return true;
                }
                if (m.Msg == 0x80F1 && uuid == 0)
                {
                    uuid = m.WParam.ToInt32();
                    DevNum = Xml.GetDevNum(uuid);
                    //   mywebbrowser.devNum = DevNum;
                }

                if (m.Msg == 0x80F0)
                {
                    /// 100:生成UUID 
                    if (m.WParam.ToInt32() == 100 && uuid == 0)
                    {
                        uuid = m.LParam.ToInt32();

                        // mywebbrowser.uuid = uuid;
                        // MessageBox.Show(uuid.ToString());
                        return true;
                    }
                    /// 200:刷新页面
                    else if (m.WParam.ToInt32() == 200)
                    {
                        if (uuid == m.LParam.ToInt32())
                        {
                            // browser.Refresh();
                            //初始化所有设置
                            // mywebbrowser.InitinalBrowser();
                            web.Refresh();
                            web.Reload(true);


                        }
                        return true;
                    }
                    /// 400:自动回复.该功能针对所有微信
                    else if (m.WParam.ToInt32() == 400 || m.WParam.ToInt32() == 900)
                    {
                        if (m.WParam.ToInt32() == 900)
                        {
                            // MessageBox.Show("启动自动回复！");
                            IsRobot = true;
                        }
                        else if (m.WParam.ToInt32() == 400)
                        {
                            // MessageBox.Show("关闭自动回复！");
                            IsRobot = false;
                        }
                        return true;
                    }
                    //500:聊天记录
                    else if (m.WParam.ToInt32() == 500)
                    {
                        if (m.LParam.ToInt32() == uuid)
                        {


                            if (wxUin != "" && wxUin != null) //这里根据数据的名字加载聊天记录
                            {
                                new ChatRecord(wxUin, RootPath).ShowDialog();
                            }
                            else
                            {
                                // MessageBox.Show("请先登录！");
                                MessageBox.Show("请登录微信", "未登录", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    //600:退出登陆
                    else if (m.WParam.ToInt32() == 600)
                    {
                        if (m.LParam.ToInt32() == uuid)
                        {
                            string loginout = "$('a[title ='退出']').click(); $('a[title ='Log Out']').click();";
                            web.ExecuteScriptAsync(loginout);
                            //MessageBox.Show("执行退出");
                           // web.Load("https://wx.qq.com/?lang=zh_CN");
                            // web.EvaluateScriptAsync("function confirm() {return true;} confirm();");

                            return true;
                            // web.Reload(false);

                        }
                    }
                    //700:关闭当前进程。
                    else if (m.WParam.ToInt32() == 700)
                    {
                        if (m.LParam.ToInt32() == uuid)
                        {
                            Application.Exit();
                            Cef.Shutdown();
                            web.Dispose();
                        }
                    }
                }
                if (m.Msg == 0x80F3)
                {
                    int uuid = m.LParam.ToInt32();
                    if (uuid == Form1.uuid) //12. 再判断修改用户备注信息
                    {
                        int userID = m.WParam.ToInt32();
                        Form1.UpdateUserTag(userID.ToString());
                        return true;
                    }
                }
                if (m.Msg == 0x80F4)
                {
                    int uuid = m.LParam.ToInt32();
                    if (m.WParam.ToInt32() == 331)//1. 先判断刷新用户
                    {
                        if (Convert.ToInt32(uuid) == Form1.uuid)//刷新下用户
                        {
                            Form1.RefUserInfo();
                        }
                        return true;
                    }
                }
                if (m.Msg == 0x80F5) //群组 
                {


                    if (m.WParam.ToInt32() == 100)
                    {
                        if (uuid == m.LParam.ToInt32())
                        {
                            if (wxUin != "" && wxUin != null) //这里根据数据的名字加载聊天记录
                            {
                                new Frm.FrmGroupUser().Show();
                            }
                            else
                            {

                                //  MessageBox.Show("请登录微信!");
                                MessageBox.Show("请登录微信", "未登录", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }


                        }
                        return true;
                    }
                }
                return false;
            }
            #endregion 
        }
        #endregion

        private static bool checkChildFrmExist(string childFrmName)//参数窗体名称
        {
            foreach (Form childFrm in Application.OpenForms)
            {
                if (childFrm.Name == childFrmName)
                {
                    if (childFrm.WindowState == FormWindowState.Minimized)
                        childFrm.WindowState = FormWindowState.Normal;
                    childFrm.Activate();
                    return true;
                }
            }
            return false;
        }

        public Form1()
        {

            InitializeComponent();
            Instance = this;
            SuppressWininetBehavior();

            //启动时，调用接口取获取唯一标识UUID号,取到了UUID号，才可以进行下次一点击增加微信
            MsgFilter myinfo = new MsgFilter();
            Application.AddMessageFilter(myinfo);
        }

        private unsafe void SuppressWininetBehavior()
        {
            /* SOURCE: http://msdn.microsoft.com/en-us/library/windows/desktop/aa385328%28v=vs.85%29.aspx
            * INTERNET_OPTION_SUPPRESS_BEHAVIOR (81):
            *      A general purpose option that is used to suppress behaviors on a process-wide basis. 
            *      The lpBuffer parameter of the function must be a pointer to a DWORD containing the specific behavior to suppress. 
            *      This option cannot be queried with InternetQueryOption. 
            *      
            * INTERNET_SUPPRESS_COOKIE_PERSIST (3):
            *      Suppresses the persistence of cookies, even if the server has specified them as persistent.
            *      Version:  Requires Internet Explorer 8.0 or later.
            */


            int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
            int* optionPtr = &option;

            bool success = InternetSetOption(0, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
            if (!success)
            {
                MessageBox.Show("Something went wrong !>?");
            }
        }

        [DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);


        static ChromiumWebBrowser web;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {              
                 //Connect();
                 InitSocket();
                 var re = new request();

                //接收和发送的http消息
                re.msg += Re_msg;
                re.msg2 += Re_msg2;
                web = new ChromiumWebBrowser("https://wx2.qq.com/?lang=zh_CN");   // 绑定 wx2.qq.com 定向不同

                web.Dock = DockStyle.Fill;
                web.RequestHandler = re;
                web.FrameLoadStart += Web_FrameLoadStart;
                web.FrameLoadEnd += Web_FrameLoadEnd;
                web.LoadingStateChanged += Web_LoadingStateChanged;
             
                this.Invoke(new Action(() =>
                {
                    this.Controls.Add(web);
                }));
             
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Web_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();

            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += Visitor_GetUin;
            cookieManager.VisitAllCookies(visitor);
        }

        private void Web_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {

            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();

            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += Visitor_GetUin;
            cookieManager.VisitAllCookies(visitor);
            //  modcssAsync();
           // modcssAsync();
        }

        private void Visitor_GetUin(CefSharp.Cookie obj)
        {
            System.Net.Cookie ck = new System.Net.Cookie(obj.Name, obj.Value, obj.Path, obj.Domain);
            myCookieContainer.Add(ck);

            if (ck.Name == "wxuin")  //获取微信号唯一标识Uid
            {
                wxUin = ck.Value;
                mywxinfo.Uin = wxUin;
            }


            if (ck.Name == "wxsid")
            {
                loginRedirectResult.wxsid = ck.Value;
            }
        }

        private void Web_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            ICookieManager cookieManager = CefSharp.Cef.GetGlobalCookieManager();
            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += Visitor_SendCookie;
            cookieManager.VisitAllCookies(visitor);
           // modcssAsync();
            //ItemClickAsync();
        }
        private void Visitor_SendCookie(CefSharp.Cookie obj)
        {
            System.Net.Cookie ck = new System.Net.Cookie(obj.Name, obj.Value, obj.Path, obj.Domain);
            myCookieContainer.Add(ck);
            if (ck.Name == "wxuin")  //获取微信号唯一标识Uid
            {
                wxUin = ck.Value;
                mywxinfo.Uin = wxUin;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj2"></param>
        //发送http
        private void Re_msg2(string obj, object obj2)
        {
            getReceiveMesAsync(obj);
        }
        string pass_ticket = String.Empty;
        private void Re_msg(string obj)
        {
           // modcssAsync();
            #region 1. wx2 新版微信
            if (obj.Contains("https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true") || obj.Contains("https://login.wx2.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true"))
            {          
                getIconAsync(obj);
            }

            getSendMes(obj);  //获取发送的消息

            getQrcode(obj);  //设备远程登录时，获取二维码

            //开启线程，抓取用户列表并保存到数据库
            if (obj.Contains("https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?"))
            {
                modcssAsync();
                ///新加获取Cookie 
                WxorWx2 = 2;
                var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
                CookieVisitor visitor = new CookieVisitor();
                visitor.SendCookie += Visitor_GetUin;
                cookieManager.VisitAllCookies(visitor);

                Thread getFriendsThread = new Thread(new ParameterizedThreadStart(getfriends));
                getFriendsThread.IsBackground = true;
                getFriendsThread.Start((Object)obj);
            }
            if (obj.Contains("https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxnewloginpage"))
            {
                GetLoginInfo(obj.ToString());
            }
            #endregion

            #region  2. wx 老版本微信

            //开启线程，抓取用户列表并保存到数据库 在Respone获取  Re_msg2方法中 
            if (obj.Contains("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket") || obj.Contains("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?"))
            {
                modcssAsync();
                WxorWx2 = 1;
                ///新加获取Cookie
                var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
                CookieVisitor visitor = new CookieVisitor();
                visitor.SendCookie += Visitor_GetUin;
                cookieManager.VisitAllCookies(visitor);


                Thread getFriendsThread = new Thread(new ParameterizedThreadStart(getfriends));
                getFriendsThread.IsBackground = true;
                getFriendsThread.Start((Object)obj);
            }

            if (obj.Contains("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxnewloginpage"))
            {
                WxorWx2 = 1;
                GetLoginInfo(obj.ToString());
            }
            #endregion

            #region 3.  群组信息

            if (obj.Contains("pass_ticket"))
            {
                NameValueCollection col = common.HtmlGetInfo.GetQueryString(obj.ToString());
                pass_ticket = col["pass_ticket"];
            }
            #endregion
            if (obj.Contains("https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxlogout?") || obj.Contains("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxlogout?"))
            {
                Send("WinForm", 600, uuid, 0x80F0);
            }
            Application.DoEvents();
        }
        /// <summary>
        /// 当前聊天组NickName
        /// </summary>
        string Group_NowNickName = String.Empty;
        /// <summary>
        /// 当前聊天组UserName
        /// </summary>
        string Group_NowUserName = String.Empty;
        /// <summary>
        /// 当前聊天群组 UserName和NickName
        /// </summary>
        List<common.GroupUserAndNickName> list_GroupName = new List<common.GroupUserAndNickName>();

        /// <summary>
        /// 获取组信息
        /// </summary>
        /// <param name="obj">群组消息</param>
        /// <param name="Wx2orWx">老微信1，新微信2 </param>
        /// <returns></returns>
        private async Task getGroupsAsync(Object obj, int WxorWx2)
        {
            common.WxGroupMsg WXGroupMsg = JsonConvert.DeserializeObject<common.WxGroupMsg>(obj.ToString());
            if (WXGroupMsg.Msg.Content == "")
                return;
            var html = await web.GetSourceAsync();
            if (Group_NowNickName == "" && Group_NowUserName == "")
            {
                string GroupName = common.HtmlGetInfo.GetGroupNickName(html);
                if (GroupName != "")
                {
                    Group_NowNickName = GroupName;
                }
                Group_NowUserName = WXGroupMsg.Msg.ToUserName;
                common.GroupUserAndNickName group = new common.GroupUserAndNickName();
                group.NickName = Group_NowNickName;
                group.UserName = Group_NowUserName;
                if (!group.NickName.Contains("@@"))
                {
                    list_GroupName.Add(group);
                }
            }
            else
            {
                if (WXGroupMsg.Msg.ToUserName != Group_NowUserName)
                {
                    string GroupName = common.HtmlGetInfo.GetGroupNickName(html);
                    if (GroupName != "")
                        Group_NowNickName = GroupName;
                    Group_NowUserName = WXGroupMsg.Msg.ToUserName;

                    common.GroupUserAndNickName group = new common.GroupUserAndNickName();
                    group.NickName = Group_NowNickName;
                    group.UserName = Group_NowUserName;
                    if (!group.NickName.Contains("@@"))
                    {
                        list_GroupName.Add(group);
                    }

                }
            }
            // Console.WriteLine("当前聊天群组：" + Group_NowNickName + " " + Group_NowUserName);
            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += Visitor_GetUin;
            cookieManager.VisitAllCookies(visitor);
            GetGroupUserList(WXGroupMsg, WxorWx2);
        }
        /// <summary>
        /// 当前微信聊天群组人员
        /// </summary>
        List<common.MemberList> Group_MemberList = new List<common.MemberList>();
        /// <summary>
        /// 获取群组
        /// </summary>
        /// <param name="WxGroupMsg">获取群组 通过UserName</param>
        /// <param name="WxorWx2">WxorWx2 新老微信标识， 老微信1 新微信2</param>
        private void GetGroupUserList(common.WxGroupMsg WxGroupMsg, int WxorWx2)
        {
            common.Noumenon_GetGroupUser info = GetGroupInfoByGroupName(WxGroupMsg);
            string froupname = WxGroupMsg.Msg.ToUserName;
            if (Group_NowUserName != froupname)
            {
                JObject job = new JObject();
                if (WxorWx2 == 1) //老微信
                {
                    job = common.WXService.SendPostRequest_Old(info.Url, info.PostData, myCookieContainer);
                }
                if (WxorWx2 == 2) //新微信
                {
                    job = common.WXService.SendPostRequest(info.Url, info.PostData, myCookieContainer);
                }

                try
                {
                    var BaseResponse = job["BaseResponse"];
                    var Count = job["Count"];
                    var ContactList = job["ContactList"];
                    var arr = ContactList.Last.ToString();
                    JObject json1 = (JObject)JsonConvert.DeserializeObject(arr);
                    JArray MemberList = (JArray)json1["MemberList"];
                    for (int i = 0; i < MemberList.Count; i++)
                    {
                        common.MemberList infoM = new common.MemberList();
                        infoM.UserName = MemberList[i]["UserName"].ToString();
                        infoM.NickName = MemberList[i]["NickName"].ToString();
                        infoM.GroupUserName = WxGroupMsg.Msg.ToUserName;
                        if (!Group_MemberList.Contains(infoM))
                            Group_MemberList.Add(infoM);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        public System.IO.StringReader DeCompress(byte[] str)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            stream.Write(str, 0, str.Length);
            stream.Position = 0;
            GZipStream zip = new GZipStream(stream, CompressionMode.Decompress);
            System.IO.StreamReader rd = new System.IO.StreamReader(zip);
            return new System.IO.StringReader(rd.ReadToEnd());
        }
        private common.Noumenon_GetGroupUser GetGroupInfoByGroupName(common.WxGroupMsg WxGroupMsg)
        {
            string post_Uin = loginRedirectResult.wxuin;
            string post_Sid = loginRedirectResult.wxsid;
            string post_Skey = loginRedirectResult.skey;
            string post_DeviceID = CreateNewDeviceID();
            string post_GroupName = WxGroupMsg.Msg.ToUserName;
            string ur1l = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=";
            if (WxorWx2 == 1)
            {
                ur1l = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=";
            }
            else
            {
                ur1l = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=";
            }

            common.Noumenon_GetGroupUser info = new common.Noumenon_GetGroupUser();
            info.Url = String.Format(ur1l + "{0}&lang=zh_CN&pass_ticket={1}", getR().ToString(), pass_ticket);
            info.ToUserName = WxGroupMsg.Msg.ToUserName;
            info.PostData = "{\"BaseRequest\":{\"Uin\":" + post_Uin + ",\"Sid\":\"" + post_Sid + "\",\"Skey\":\"" + post_Skey + "\",\"DeviceID\":\"" + post_DeviceID + "\"},\"Count\":1,\"List\":[{\"UserName\":\"" + WxGroupMsg.Msg.ToUserName + "\",\"EncryChatRoomId\":\"" + "" + "\"}]}";
            return info;
        }

        private async Task GetInfoNickNameAsync()
        {
            var html = await web.GetSourceAsync();
            NickName = common.HtmlGetInfo.GetNickName(html);
            SendServerMsgs(NickName);
            //SendMsg(NickName);
            
            CommonTools.LoginDir.AddNickname(uuid.ToString(), NickName);
            Send("WinForm", 111, uuid, 0x80F0); //更新昵称        
        }


        /// <summary>
        /// 获取接收到的消息
        /// </summary>
        /// <param name="obj"></param>
        private async Task getReceiveMesAsync(string obj)
        {
            //获取接收的消息
            if (obj.Contains("MsgId") && obj.Contains("AddMsgCount") && obj.Contains("Val"))
            {
                ReceiveMessageRoot root = JsonConvert.DeserializeObject<MyDB.ReceiveMessageRoot>(obj);

                ///要排除的特殊消息体
                if (Enum.IsDefined(typeof(common.WxMsg_Type_Neglect), root.AddMsgList[0].MsgType)) { return; }

                if (MyUserName == null)
                {
                    if (root.AddMsgList[0].MsgType == 51)
                    {
                        MyUserName = root.AddMsgList[0].FromUserName;
                    }
                    if (!root.AddMsgList[0].FromUserName.Contains("@@"))
                    {
                        MyUserName = root.AddMsgList[0].ToUserName;
                    }

                    if (root.AddMsgList[0].MsgType == 1 && !root.AddMsgList[0].FromUserName.Contains("@@")) //文本消息
                    {
                        SendMsg(root.AddMsgList[0].FromUserName, "XXXXX");

                        //发送回传到数据库  接收别人发给我的消息
                        int result = MyDB.WeChatUser.SaveWeCharChatLog(root, wxUin, friendsRoot, 1, NickName);
                        //保存到数据库
                        RecMessageOperation.writeMessage(root, wxUin, friendsRoot);
                        ///告诉主线程新消息
                       //Send("WinForm", 222, uuid, 0x80F0); //更新消息
                       SendServer_NewMsg();
                    }
                    //来自群组的消息
                    if (root.AddMsgList[0].FromUserName.Contains("@@"))
                    {
                        List<MyDB.MemberListItem> listn = common.ConvertInfo.CovnertMemberListItem(Group_MemberList);
                        RecMessageOperation.WriteGroupMessage(mywxinfo, root, wxUin, friendsRoot, listn, GetNickNameByUserName(root.AddMsgList[0].FromUserName)); //Group_NowNickName
                        return;
                    }

                }
                else
                {
                    if (root.AddMsgList[0].MsgType == 1 && !root.AddMsgList[0].Content.ToString().Substring(0, 1).Contains("@")) //文本消息
                    {
                        SendMsg(root.AddMsgList[0].FromUserName, "XXXXX");
                        //发送回传到数据库  接收别人发给我的消息
                        int result = MyDB.WeChatUser.SaveWeCharChatLog(root, wxUin, friendsRoot, 1, NickName);
                        //IsRobot = true;
                        if (IsRobot == true)
                        {
                            for (int i = 0; i < root.AddMsgList.Count; i++)
                            {
                                string ResultWord = GetResultWord(root.AddMsgList[i].Content);
                                //CheckAndSend(ResultWord);
                                // AutoSendMsg(root, ResultWord); 可以发送的
                                //loginRedirectResult.skey = Skey;
                                // common.WXService.AutoSendMsg(myCookieContainer, ResultWord, root.AddMsgList[0].FromUserName, root.AddMsgList[0].ToUserName, 1, loginRedirectResult);
                                string js_func = "window.chatFactory = angular.element(document).injector().get('chatFactory');";
                                js_func += "function wxSendTextMessage(tousername,msg,silent){";
                                js_func += "'use strict';";
                                js_func += "if ('current' == tousername)";
                                js_func += "{";
                                js_func += "tousername = angular.element(document).injector().get('chatFactory').getCurrentUserName();";
                                js_func += "}";
                                js_func += "try";
                                js_func += "{";
                                js_func += "    if (silent)";
                                js_func += "   {";
                                js_func += "let t = window.chatFactory.createMessage({";
                                js_func += "MsgType: angular.element(document).injector().get('confFactory').MSGTYPE_TEXT,";
                                js_func += "Type: angular.element(document).injector().get('confFactory').MSGTYPE_TEXT,";
                                js_func += "Content: msg,";
                                js_func += "ToUserName: tousername,";
                                js_func += "});";
                                js_func += "window.chatFactory.appendMessage(t);";
                                js_func += "window.chatFactory.sendMessage(t);";
                                js_func += "}";
                                js_func += "else";
                                js_func += "{";
                                js_func += "let oldusername = angular.element(document).injector().get('chatFactory').getCurrentUserName();";
                                js_func += "if (oldusername != tousername)";
                                js_func += "{";
                                js_func += "angular.element(document).injector().get('chatFactory').setCurrentUserName(tousername);";
                                js_func += "}";
                                js_func += "let oldmsg = angular.element('#editArea').scope().editAreaCtn;";
                                js_func += "angular.element('#editArea').scope().editAreaCtn = msg;";
                                js_func += "angular.element('#editArea').scope().sendTextMessage();";
                                js_func += "angular.element('#editArea').scope().editAreaCtn = oldmsg;";
                                js_func += "angular.element('#editArea').text(oldmsg);";
                                js_func += "angular.element(document).injector().get('chatFactory').setCurrentUserName(oldusername);";
                                js_func += "}";
                                js_func += "}";
                                js_func += "catch (err)";
                                js_func += "{";
                                js_func += "}";
                                js_func += "}";
                                js_func += "wxSendTextMessage('" + root.AddMsgList[i].FromUserName + "' ,'" + ResultWord + "' ,true)";
                                JavascriptResponse x = await web.EvaluateScriptAsync(js_func);
                                //Console.WriteLine(x);
                            }
                        }
                        //保存到数据库
                        RecMessageOperation.writeMessage(root, wxUin, friendsRoot);
                    }
                    if (root.AddMsgList[0].ToUserName.Contains("@@") && !root.AddMsgList[0].Content.Contains("<br/>"))
                    {
                        if (root.AddMsgList[0].Content == "")
                        {
                            // Console.WriteLine("==========打开其他设备消息：==========" + root.AddMsgList[0].Content);
                        }
                        else
                        {
                            //  Console.WriteLine("==========来自其他设备消息：==========" + root.AddMsgList[0].Content);
                        }

                        List<MyDB.MemberListItem> listn = common.ConvertInfo.CovnertMemberListItem(Group_MemberList);
                        RecMessageOperation.WriteGroupMessage(mywxinfo, root, wxUin, friendsRoot, listn, Group_NowNickName);
                        return;
                    }
                    ///群里来的图片消息
                    if (root.AddMsgList[0].FromUserName.Contains("@@") && root.AddMsgList[0].MsgType == 3)
                    {
                        // Console.WriteLine("群里来图片消息了");
                    }
                    ///告诉主线程新消息
                    //Send("WinForm", 222, uuid, 0x80F0);
                    SendServer_NewMsg();
                }

                ///群组消息
                if (root.AddMsgList[0].FromUserName.Contains("@@"))//|| root.AddMsgList[0].ToUserName.Contains("@@"))//群组消息
                {
                    GroupMsg(root);
                }

                // type 3 图片消息
                if (root.AddMsgList[0].MsgType == 3) //&& !root.AddMsgList[0].FromUserName.Contains("@@")
                {
                    loginRedirectResult.WxorWx2 = WxorWx2;
                    string result = common.DownFriendsInfo.SaveSendImgPath(myCookieContainer, root, loginRedirectResult);
                    if (result != "")
                    {
                        root.AddMsgList[0].Content = "file:" + result;
                        RecMessageOperation.writeMessage(root, wxUin, friendsRoot);
                    }
                }


               
            }
        }
        /// <summary>
        /// 1.接收到的普通消息
        /// </summary>
        /// <param name="root"></param>
        private void NormalMsg(ReceiveMessageRoot root)
        {

        }
        /// <summary>
        ///2.接收到的群组消息
        /// </summary>
        /// <param name="root"></param>
        private void GroupMsg(ReceiveMessageRoot root)
        {
            //Console.WriteLine("\r\n我通过客服接收到的群组消息：==========" + root.AddMsgList[0].Content);
            List<MyDB.MemberListItem> listn = common.ConvertInfo.CovnertMemberListItem(Group_MemberList);
            if (MyUserName == root.AddMsgList[0].FromUserName)
            {
                mywxinfo.UserName = MyUserName;
            }

            for (int i = 0; i < list_GroupName.Count; i++)
            {
                if (root.AddMsgList[0].FromUserName == list_GroupName[i].UserName)
                {
                    Group_NowNickName = list_GroupName[i].NickName;
                }
            }

            RecMessageOperation.WriteGroupMessage(mywxinfo, root, wxUin, friendsRoot, listn, Group_NowNickName);
        }
        /// <summary>
        /// 3. 发送的群组消息
        /// </summary>
        private void SendGroupMsg(SendMsgRequest root)
        {
            //Console.WriteLine("\r\n我通过客服发送群的：==============" + root.Msg.Content);
            List<MyDB.MemberListItem> listn = common.ConvertInfo.CovnertMemberListItem(Group_MemberList);
            string nickname = GetNickNameByUserName(root.Msg.ToUserName);
            if (nickname == "")
            {
                SaveWebToGroupMsg(root, wxUin, friendsRoot, listn);
            }
            else
            {
                RecMessageOperation.WriteGroupMessage(root, wxUin, friendsRoot, listn, nickname);
            }
        }

        //发送消息
        private void SendMsg(string toUserName, string content)
        {
            return;  //不执行这个刷新

            //构造参数
            MyDB.Msg msg = new MyDB.Msg();
            msg.FromUserName = MyUserName;
            msg.ToUserName = toUserName;
            msg.Content = content;
            msg.ClientMsgId = DateTime.Now.Millisecond;//14948501206950223;
            msg.LocalID = DateTime.Now.Millisecond; //14948501206950223;//
            msg.Type = 1;

            SendBaseRequest mBaseReq = new SendBaseRequest();
            mBaseReq.Sid = loginRedirectResult.wxsid;
            mBaseReq.Skey = Skey;
            mBaseReq.Uin = loginRedirectResult.wxuin;
            mBaseReq.DeviceID = CreateNewDeviceID();


            //发送消息
            string url = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid={0}&r={1}&lang=zh_CN&pass_ticket={2}";

            if (WxorWx2 == 1)
            {
                url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid={0}&r={1}&lang=zh_CN&pass_ticket={2}";
            }

            url = string.Format(url, mBaseReq.Sid, getR(), loginRedirectResult.pass_ticket);
            SendMsgRequest req = new SendMsgRequest();
            req.BaseRequest = mBaseReq;
            req.Msg = msg;
            req.Scene = DateTime.Now.Millisecond;
            string requestJson = JsonConvert.SerializeObject(req);
            string repJsonStr = PostString(url, requestJson);

            //if (repJsonStr == null) return null;
            //var rep = JsonConvert.DeserializeObject<SendMsgResponse>(repJsonStr);
            //return rep;
        }

        /// <summary>
        /// 自动发送消息
        /// </summary>
        /// <param name="resultMsg"></param>
        private void AutoSendMsg(ReceiveMessageRoot root, string resultMsg)
        {
            ///参数体
            common.WxMsgParsed pa = new common.WxMsgParsed();
            pa.Pass_Ticket = pass_ticket;
            pa.Sid = loginRedirectResult.wxsid;
            pa.SKey = Skey;
            pa.Uin = wxUin;

            ///消息体
            common.WXMsg_Message msg = new common.WXMsg_Message();
            msg.From = root.AddMsgList[0].ToUserName;
            msg.To = root.AddMsgList[0].FromUserName;
            msg.Msg = resultMsg;
            msg.Readed = false;
            msg.Time = DateTime.Now;
            msg.Type = 1;

            common.UserMessage msginfo = new common.UserMessage();
            string result = msginfo.SendMsg(pa, myCookieContainer, msg, false);
        }

        private string PostString(string url, string content)
        {
            //mHandler = new HttpClientHandler();
            //mHandler.UseCookies = true;
            //mHandler.AutomaticDecompression = DecompressionMethods.GZip;
            //mHandler.AllowAutoRedirect = true;
            //mHttpClient = new HttpClient(mHandler);
            //mHttpClient.DefaultRequestHeaders.ExpectContinue = false;
            //SetHttpHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
            //SetHttpHeader("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4,ja;q=0.2");
            //SetHttpHeader("Accept-Encoding", "gzip, deflate, sdch, br");

            HttpResponseMessage response = mHttpClient.PostAsync(new Uri(url), new StringContent(content)).Result;

            string ret = response.Content.ReadAsStringAsync().Result;
            response.Dispose();
            return ret;
        }
        /// <summary>
        /// 从群组List里 根据UserName 获得NickName
        /// </summary>
        /// <param name="UserName">@@UserName</param>
        /// <returns></returns>
        private string GetNickNameByUserName(string UserName)
        {
            foreach (common.GroupUserAndNickName item in list_GroupName)
            {
                if (item.UserName == UserName)
                {
                    return item.NickName;
                }
            }
            return "";
        }


        /// <summary>
        /// POST
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">方法</param>
        /// <param name="param">json参数</param>
        /// <returns></returns>
        public static string WebServiceApp(string url, string param)
        {
            //转换输入参数的编码类型，获取bytep[]数组 
            byte[] byteArray = Encoding.UTF8.GetBytes(param);
            //初始化新的webRequst
            //1． 创建httpWebRequest对象
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            //2． 初始化HttpWebRequest对象
            webRequest.Method = "POST";
            webRequest.CookieContainer = myCookieContainer;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            //3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
            Stream newStream = webRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            //4． 读取服务器的返回信息
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string phpend = php.ReadToEnd();
            return phpend;
        }





        HttpClientHandler mHandler;
        HttpClient mHttpClient = new HttpClient();
        private void SetHttpHeader(string name, string value)
        {
            if (mHttpClient.DefaultRequestHeaders.Contains(name))
            {
                mHttpClient.DefaultRequestHeaders.Remove(name);
            }

            mHttpClient.DefaultRequestHeaders.Add(name, value);
        }

        static long getR()
        {
            return GetTimeStamp();
        }

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        private static string CreateNewDeviceID()
        {
            Random ran = new Random();
            int rand1 = ran.Next(10000, 99999);
            int rand2 = ran.Next(10000, 99999);
            int rand3 = ran.Next(10000, 99999);
            return string.Format("e{0}{1}{2}", rand1, rand2, rand3);
        }



        static int flagIndex = 0;
        static bool isStart = false;
        private void getSendMes(string obj)
        {
            //如果我在群里发送群组消息了 ,就开始记录这个群的聊天信息
            if (obj.Contains("@@"))
            {
                try
                {
                    common.WxGroupMsg groupmsg = JsonConvert.DeserializeObject<common.WxGroupMsg>(obj.ToString());
                    GetGroupUserList(groupmsg, WxorWx2);
                }
                catch (Exception ex)
                { }

            }
            //识别发送的消息
            if (isStart == true)
                flagIndex++;

            if (obj.Contains("https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg"))
            {
                flagIndex = 0;
                isStart = true;
                WxorWx2 = 2;
            }
            if (obj.Contains("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg"))
            {
                flagIndex = 0;
                isStart = true;
                WxorWx2 = 1;
            }
            if (flagIndex == 2)
            {
                SendMsgRequest root = JsonConvert.DeserializeObject<SendMsgRequest>(obj);
                //保存到数据库 我发给群消息 ==》群别人的
                if (root.Msg.ToUserName.Contains("@@"))
                {
                    getGroupsAsync(obj, WxorWx2);
                    SendGroupMsg(root);
                    return;
                }
                //我发送给好友的消息（非群消息）
                if (root.Msg.ToUserName != "filehelper" && friendsRoot.MemberList != null)//好友列表里面没有文件助手
                {
                    SendMessageOperation.writeMessage(root, wxUin, friendsRoot);
                    //发送回传到数据库   我发给==》别人的 
                    int reuslt1 = MyDB.WeChatUser.SaveWebCharLog1(root, wxUin, friendsRoot, 0, NickName);
                    Now_UserName = root.Msg.ToUserName;
                    wxSid = loginRedirectResult.wxsid;
                    //if (reuslt1 != 0)
                    //    MessageBox.Show("发送存储失败！");       
                    ffAsync();
                }


            }

        }
        private void SaveWebToGroupMsg(SendMsgRequest root, string wxUin, Root friendsRoot, List<MyDB.MemberListItem> listn)
        {
            //记录的是第一条发送的群组消息类型, 因不发送前是没有UserName的
            var task = GetGroupNickNameByJS(root, wxUin, friendsRoot, listn);
            if (!task.IsCompleted)
            {
                //Console.WriteLine("异步方法未完成,开始等待");
            }
            else
            {
                // Console.WriteLine("异步方法完成,开始等待");
            }

        }

        string qrcode = "";
        private void getQrcode(string obj)
        {
            try
            {
                if (DevNum == null) //如果没有设备号，则不用传输二维码
                    return;

                if (!obj.Contains("https://login.weixin.qq.com/qrcode/"))
                    return;
                string qrUrl = "";
                if (!qrcode.Equals(obj))
                {
                    qrUrl = obj.ToString();
                }

                string filePath = "";  //图片保存路径
                HttpWebRequest request = HttpWebRequest.Create(qrUrl) as HttpWebRequest;
                request.Method = "GET";

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
                //像服务器传输二维码
                Cloud.Login(DevNum.TrimStart(), filePath);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="url">URL</param>
        private void getfriends(object url)
        {
            try
            {   //获得昵称
                GetInfoNickNameAsync();

                JObject obj = common.WXService.GetContactByUrl(url.ToString(), myCookieContainer, cookieLogin);
                friendsRoot = JsonConvert.DeserializeObject<MyDB.Root>(obj.ToString());

                //写入数据库

                friendsOperation.WriteFriends(friendsRoot, wxUin);

                //写入回传用户好友列表
                int result = MyDB.WeChatUser.SaveWeChatFriendsList(friendsRoot, wxUin, NickName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MethodBase method = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod();
                CommonTools.ExceptionLogInfo.SaveExceptionInfo(method.ReflectedType.FullName, method.Name, ex.ToString() + "url:为：" + url);
            }
        }



        string cookieLogin = "";
        //获取登录信息
        private void GetLoginInfo(string redirect_url)
        {
            HttpClient mHttpClient = new HttpClient();
            mHttpClient.DefaultRequestHeaders.Referrer = new Uri("https://wx.qq.com/");
            string url = redirect_url + "&fun=new&version=v2&lang=zh_CN";
            HttpResponseMessage response = new HttpClient().GetAsync(new Uri(url)).Result;
            string ret = response.Content.ReadAsStringAsync().Result;

            /////////////////////// cookie

            //string s1 = cooki.Substring(cooki.IndexOf("Set-Cookie:"), cooki.Length- cooki.IndexOf("Set-Cookie:"));
            //// s1 = s1.Replace("Set-Cookie:", "").Replace("Domain=wx2.qq.com;", "");
            //cookieLogin = s1;



            ///////////////////////

            response.Dispose();
            if (ret == null) return;
            if (ret == "<error><ret>1203</ret><message></message></error>")
            {
                MessageBox.Show("登录频繁，请稍后登录！");
                return;
            }
            try
            {
                loginRedirectResult = new LoginRedirectResult();
                loginRedirectResult.pass_ticket = ret.Split(new string[] { "pass_ticket" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
                loginRedirectResult.skey = ret.Split(new string[] { "skey" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
                loginRedirectResult.wxsid = ret.Split(new string[] { "wxsid" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
                loginRedirectResult.wxuin = ret.Split(new string[] { "wxuin" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
                loginRedirectResult.isgrayscale = ret.Split(new string[] { "isgrayscale" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');

                wxUin = loginRedirectResult.wxuin;

                Skey = loginRedirectResult.skey;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ret);
                string loginout = "$('a[title ='退出']').click(); $('a[title ='Log Out']').click();";
                web.ExecuteScriptAsync(loginout);
                Send("WinForm", 600, uuid, 0x80F0);
            }
        }




        private static void CheckAndSend(string words)
        {
            //执行JS，检查当前是否有输入框

            //有。则发送
            send(words);
        }
        //是否获得头像标志位

        private bool AlreadyGetIcon = false;
        /// <summary>
        /// 每个browser，第一次扫描到该url即为头像url。在刷新的时候，记住将标志位复位。
        /// </summary>
        private async Task getIconAsync(string url)
        {
            Application.DoEvents();
            Thread.Sleep(1000);

            var html = await web.GetSourceAsync();
            if (AlreadyGetIcon == false)
            {
                if (html.Contains("data:img/jpg;base64") == false)
                {
                    html = await web.GetSourceAsync();   //异步线程 UI无响应 ？再来一次
                    if (html.Contains("data:img/jpg;base64") == false)
                    {
                        return;
                    }
                }

                //当前browser唯一标识uuid作为文件名
                if (!Directory.Exists(Environment.CurrentDirectory + "/HeaderImages/"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "/HeaderImages/");
                }

                string filePath = Environment.CurrentDirectory + "/HeaderImages/" + uuid + ".jpg";
                if (html.ToString().Contains("mm-src="))
                {

                    if (common.Base64Helper.get_image(html.ToString(), filePath) != "")
                    {
                        Send("WinForm", 111, uuid, 0x80F0);
                        AlreadyGetIcon = true;
                        SendServer_UserImg(uuid.ToString(), filePath);
                        return;
                    }
                    else
                    {
                        //主界面加载去
                    }
                }
            }


        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="content"></param>
        private static void send(string content)
        {
            string scriptline01 = @"function SendMessage() {";
            string scriptline02 = @"    angular.element('#editArea').scope().editAreaCtn = '" + content + "'; ";
            string scriptline03 = @"    angular.element('#editArea').scope().sendTextMessage(); ";
            string scriptline04 = @"      ;}";
            string scriptline05 = @"SendMessage();";
            string js = scriptline01 + scriptline02 + scriptline03 + scriptline04 + scriptline05;
            web.ExecuteScriptAsync(js);
            //  Console.WriteLine(js);
            // js = "SendMessage(){angular.element('#editArea').scope().editAreaCtn='11111111';}";
            //   js = "alert(document.body.innerHTML);";
        }

        #region Win32 消息交互
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

        #region 本地聊天记录
        /// <summary>
        /// 异步记录第一条我发出的群组消息 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="wxUin"></param>
        /// <param name="friendsRoot"></param>
        /// <param name="listn"></param>
        /// <returns></returns>
        private async Task GetGroupNickNameByJS(SendMsgRequest root, string wxUin, Root friendsRoot, List<MyDB.MemberListItem> listn)
        {
            string result_NickName = String.Empty;
            string js = "function getNickName(){ var obj=document.getElementsByClassName('title_name ng-binding')[0].innerHTML;   return obj; }  getNickName();";
            JavascriptResponse x = await web.EvaluateScriptAsync(js);
            if (x.Success == true)
                result_NickName = x.Result.ToString();
            RecMessageOperation.WriteGroupMessage(root, wxUin, friendsRoot, listn, result_NickName);

        }
        private string SaveLocaleChatLog(ReceiveMessageRoot root, string uid, Root friendsRoot)
        {

            List<MessageInfo> list = new List<MessageInfo>();
            for (int i = 0; i < root.AddMsgList.Count; i++)
            {
                MessageInfo nowmes = new MessageInfo();
                nowmes.MessageType = root.AddMsgList[i].MsgType;
                nowmes.Content = root.AddMsgList[i].Content;
                nowmes.FriendNick = root.AddMsgList[i].ToUserName;
                nowmes.time = DateTime.Now.ToString();
                list.Add(nowmes);
            }
            Record d = new Record();
            d.MessageList = new List<MessageInfo>();
            d.MyNick = "我的昵称" + DateTime.Now.ToString("yyyymmdd");
            d.ChatNick = "222";
            OperationRecord.WriteRecord(d);
            return "";
        }
        #endregion

        #region 智能回复消息

        private string GetResultWord(string msg)
        {
            //获取数据库所有key
            List<word> allWord = wordOperation.FindAll();

            //回复内容         //优先级(int全局变量自动初始化为0，局部变量未定义)
            string reply = ""; int pri = 100;

            //遍历所有包含的key。
            foreach (var m in allWord)
            {   //存在该关键字
                if (Regex.Match(msg, m.KeyWord).ToString() != "")
                {
                    //判断优先级。选择优先级最高的。0~100 0最大
                    if (m.Priority < pri)
                    {
                        reply = m.Reply;
                        pri = m.Priority;
                    }
                }
            }
            return reply;
        }
        #endregion

        #region  外部设置宽高  js Style 
        private static async Task modcssAsync()
        {
            try
            {
                //1. 外部设置宽高  js Style 
                string scriptline1 = @"  function SetWidth() {  var obj = document.getElementsByClassName('main_inner')[0].style.maxWidth = '3000px';}";
                string scriptline2 = @"SetWidth();";
                string js = scriptline1 + scriptline2;
                web.ExecuteScriptAsync(js);
                string Js_height = "function SetHeight(){document.getElementsByClassName('main')[0].style.padding='0'; document.getElementsByClassName('main')[0].style.height='100%'; } SetHeight();";
                JavascriptResponse x = await web.EvaluateScriptAsync(Js_height);
                //2. 外部改变通讯录的样式

                if (MyDB.WeChatConfig.PingIP(MyDB.WeChatConfig.Bg_css1.ToString()))
                {
                    string cssfile = RootPath + "/bg1.css";
                    if (!File.Exists(cssfile))
                        return;
                    string Js_str2 = "function SetBag(){" +

                    " $(\"link[rel = 'stylesheet']\").attr('href','');" +
                    " $(\"link[rel = 'stylesheet']\").attr('href','" + MyDB.WeChatConfig.Bg_css1.ToString() + "');" +
                    "} SetBag();";
                    //JavascriptResponse yo = await web.EvaluateScriptAsync(Js_str2);
                }
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.ToString());
            }

             return;
            string Js_1 = "function SetBag(){" +

                // 1.a.底色
                "document.getElementsByClassName('panel')[0].style.backgroundColor='#b0b0b0'; " +
               //   b.指定消息背景
               "$('.chat_item.top').css('background','#b0b0b0'); " +

                // 2. a.列表的边
                " var arr=document.getElementsByClassName('chat_item');" +
                " for(var i=0; i< arr.length; i++){ arr[i].style.borderBottomColor = '#808080';} " +
                "$('.contact_list .contact_title').css('borderBottomColor','#FFF');" +

                 //  b  .contact_list.contact_item
                 "$('.contact_list .contact_item').css('borderBottomColor','#808080');" +


                //3.分组标题栏
                "$('.contact_list .contact_title').css('background','#808080');" +
                "$('.contact_list .contact_title').css('color','#FFF');" +

                //4.  搜索框
                "$('.search_bar .frm_search').css('background','#808080');" +

                //5. 选中底色 .chat_item.active
                //"$('.chat_item.active').css('background','#6AAEFF');" +
                //"$('.chat_item.inactive').css('background','#808080');" +

                //6.  .tab .tab_item:after    tab标题的右边框设置
                " var style = document.createElement('style');" +
                " var text = document.createTextNode('.tab:after{content:none;border-bottom:1px solid red;}');" +
                " style.appendChild(text);" +
                " document.body.appendChild(style);" +
                " $('.tab').addClass('.tab:after');" +

                //7. .tab_item  的边框颜色
                " var style1 = document.createElement('style');" +
                " var text1 = document.createTextNode('.tab_item:after{content:none;}');" +
                " style.appendChild(text1);" +
                " document.body.appendChild(style1);" +
                " $('.tab_item').addClass('.tab:after');" +

                //8.
                "document.getElementsByClassName('tab_item')[0].style.borderRightColor='#808080'; " +


                "} SetBag();  var Bobj=$('.tab').onclick =SetBag;";
            JavascriptResponse y = await web.EvaluateScriptAsync(Js_1);
            if (y.Success != true)
            {
               // MessageBox.Show(y.Message.ToString());
            }

        }
        /// <summary>
        /// 替换css
        /// </summary>
        private static void ChangeCss()
        {
            //string js_str = "";
            //string js_str = String.Format(@"$(\'link[rel = stylesheet]\').attr('href','" + MyDB.WeChatConfig.Bg_css + "')"')";
            //web.ExecuteScriptAsync(js_str);
        }

        #endregion

        #region 获得昵称或者UserName 查到 数据库ID
        private async Task ItemClickAsync()
        {
            string js = "function AdditemClick(){document.getElementsByClassName('title poi').addEventListener('onchange', ff()); }    function ff(){ alert(1) ;}   ";
            JavascriptResponse x = await web.EvaluateScriptAsync(js);
            if (x.Success == true)
            {
                // Console.WriteLine("");
            }
            else
            {
                MessageBox.Show("失败" + x.Message);
            }
        }

        /// <summary>
        /// 根据昵称找到UserID  向主程序发送 UserID
        /// </summary>
        private void SendUserID(string UserNickName, string UserRemarkName)
        {
            //string dbpath = RootPath + "/db/" + wxUin + ".db";
            //if (!File.Exists(dbpath))
            //    return;
            //DataTable dt = MyDB.WeChatUserInfo.GetUserInfoByUserReMarkName(dbpath, UserNickName);
            //if (dt.Rows.Count == 0)
            //    dt = MyDB.WeChatUserInfo.GetUserInfoByUserNickName(dbpath, UserNickName);
            //foreach (DataRow item in dt.Rows)
            //{
            //    string DB_UserID = wxUin + "_" + item["id"].ToString();
            //    string pathfile = RootPath + "/temp.ini";
            //    if (File.Exists(pathfile))
            //        File.Delete(pathfile);
            //    File.AppendAllText(pathfile, DB_UserID + Environment.NewLine);
            //    Send("WinForm", 333, Convert.ToInt32(item["id"].ToString()), 0x80F3);
            //}

            string dbpath = RootPath + "/db/" + wxUin + ".db";
            if (!File.Exists(dbpath))
                return;
            DataTable dt = MyDB.WeChatUserInfo.GetUserInfoByUserReMarkName(dbpath, UserNickName);
            if (dt.Rows.Count == 0)
                dt = MyDB.WeChatUserInfo.GetUserInfoByUserNickName(dbpath, UserNickName);
            MyDB.Model.UserInfoModel user = new MyDB.Model.UserInfoModel();
            foreach (DataRow item in dt.Rows)
            {             
                user.ID = Convert.ToInt32(item["id"].ToString());
            }
            MyDB.Model.BaseMessage basemsg = new MyDB.Model.BaseMessage();
            basemsg.TypeID = MyDB.Model.MessageType.更新用户信息面板.ToString();
            basemsg.Uuid = Form1.uuid.ToString();
            basemsg.Uin = Form1.wxUin;
            basemsg.Data =JsonConvert.SerializeObject(user);

            SendServer_MsgUserInfo(basemsg);
        }


        #endregion

        #region  备注 操作 

        /// <summary>
        /// 修改备注 
        /// </summary>
        /// <param name="UserID"></param>
        public static void UpdateUserTag(string UserID)
        {
            MyDB.Model.UserInfoModel info = MyDB.friendsOperation.GetUserInfoByID(RootPath + "/db/" + wxUin + ".db", UserID);
            string rem = info.RemarkName;
            UpdateUserNote(rem);
        }

        /// <summary>
        /// 当前聊天的UserName
        /// </summary>
        static string Now_UserName = "";
        static string wxSid = "";
        /// <summary>
        /// 修改好友备注
        /// </summary>
        private static void UpdateUserNote(string RemarkName)
        {
            string posturl = String.Empty;
            string UserName = Now_UserName;
            string DeviceID = CreateNewDeviceID();
            string loginsid = wxSid;
            string postData = "{\"UserName\":\"" + UserName + "\",\"CmdId\":2,\"RemarkName\":\"" + RemarkName + "\",\"BaseRequest\":{\"Uin\":" + wxUin + ",\"Sid\":\"" + loginsid + "\",\"Skey\":\"" + Skey + "\",\"DeviceID\":\"" + DeviceID + "\"}}";
            JObject job = null;
            if (WxorWx2 == 2) //新微信
            {
                posturl = common.WXService.Wx2_UpdateNote;
                job = common.WXService.SendPostRequest(posturl, postData, myCookieContainer);
            }
            if (WxorWx2 == 1) //老微信
            {
                posturl = common.WXService.Wx_UpdateNote;
                job = common.WXService.SendPostRequest_Old(posturl, postData, myCookieContainer);
            }
            string ret = job["BaseResponse"]["Ret"].ToString();
            if (ret == "0")
                MessageBox.Show("修改成功！");
            else
                MessageBox.Show("修改失败");
        }

        /// <summary>
        /// 刷新用户信息
        /// </summary>
        public static void RefUserInfo()
        {
            new Form1().ffAsync();
        }

        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            web.Dispose();
            //this.Dispose(true);
        }



        /// <summary>
        /// 开启 个人信息面板
        /// </summary>
        /// <returns></returns>
        private async Task ffAsync()
        {

            var html = await web.GetSourceAsync();
            string nickname = common.HtmlGetInfo.GetGroupNickName(html);
            Now_UserName = common.HtmlGetInfo.GetGroupuserName(html).Trim().ToString();
            foreach (var item in friendsRoot.MemberList)
            {
                if (item.NickName == nickname || item.RemarkName == nickname)
                    SendUserID(item.NickName, item.RemarkName);
            }
            if (nickname.Contains("<img")) //特殊有标签的情况
            {
                string username = common.HtmlGetInfo.GetGroupuserName(html).Trim().ToString();

                //string classstr = nickname.ToString().Substring(nickname.IndexOf("class="));
                //classstr=classstr.ToString().Substring(0,classstr.IndexOf("text=")).Trim();
                foreach (var item in friendsRoot.MemberList)
                {
                    // if (item.NickName.Contains(classstr) || item.NickName.Contains(classstr))
                    if (item.UserName.ToString() == username)
                        SendUserID(item.NickName, item.RemarkName);
                }
            }
        }

     

        private void button2_Click(object sender, EventArgs e)
        {
            ChatRecord f = new ChatRecord(wxUin, RootPath);
            f.Show();
        }
        /// <summary>
        /// 测试 方法 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //Form1.RefUserInfo();
            //        GroupUserAdd();
             new Frm.FrmGroupUser().Show();
            //  new ChatRecord(wxUin, RootPath).ShowDialog();
        }
        /// <summary>
        /// 添加 群组里的成员为好友  要加载群的 好友信息的
        /// </summary>

        public void GroupUserAdd1()
        {
            //  { "BaseRequest":{ "Uin":10574944,"Sid":"lV7x5GldKA0kIqZL","Skey":"@crypt_dfd6dfb_94b387356a6a610ab210bd342f51c484",
            //          "DeviceID":"e286141931138338"},"Opcode":2,"VerifyUserListSize":1,"VerifyUserList":
            //      [{"Value":"@8c5741b58fbbf4e3179abca2c9d9259e81d2b33983e3d9fac5de83007dbe6224","VerifyUserTicket":""}],
            //      "VerifyContent":"我是wrist","SceneListCount":1,"SceneList":[33],"skey":"@crypt_dfd6dfb_94b387356a6a610ab210bd342f51c484"}

            try
            {
                ICookieManager cookieManager = CefSharp.Cef.GetGlobalCookieManager();
                CookieVisitor visitor = new CookieVisitor();
                visitor.SendCookie += Visitor_SendCookie;
                cookieManager.VisitAllCookies(visitor);
                common.AddGroupUser info = new common.AddGroupUser();
                info.Uin = wxUin;
                //info.Sid = loginRedirectResult.wxsid;
                info.Sid = wxSid;
                info.Skey = loginRedirectResult.skey;
                info.Value = ""; //这里不指定要加谁为好友 ， 在加群好友管理里指定 。
                info.VerifyContent = "hello! hi ";
                info.DeviceID = CreateNewDeviceID();
                info.R = getR();
                info.pass_ticket = pass_ticket;
                Frm.FrmGroupUser.List_GroupName = ListGroupNameAddByRoot();   // list_GroupName;
                if (ListGroupNameAddByRoot().Count == 0)
                    Frm.FrmGroupUser.List_GroupName = list_GroupName;
                Frm.FrmGroupUser.Group_MemberList = Group_MemberList;
                Frm.FrmGroupUser.info = info;
                Frm.FrmGroupUser.myCookieContainer = myCookieContainer;
                Frm.FrmGroupUser.WxorWx2 = WxorWx2;
                Frm.FrmGroupUser.loginRedirectResult = loginRedirectResult;
                //  new Frm.FrmGroupUser(list_GroupName, Group_MemberList, info, myCookieContainer, WxorWx2).Show();

            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// 从数据库的群组列表里默认加载一个群到群列表里
        /// </summary>
        private void GetListGroupName()
        {
            string tempusername = String.Empty;

            for (int i = 0; i < friendsRoot.MemberCount; i++)
            {
                if (friendsRoot.MemberList[i].UserName.Contains("@@"))
                {
                    tempusername = friendsRoot.MemberList[i].UserName;
                    break;
                }
            }
            //构造不发送的 群消息 获得列表
            common.WxGroupMsg groupmsg1 = new common.WxGroupMsg();
            groupmsg1.Msg.ToUserName = tempusername;
            GetGroupUserList(groupmsg1, WxorWx2);
        }
        /// <summary>
        /// 从好友列表里加载 已经添加的群列表
        /// </summary>
        private List<common.GroupUserAndNickName> ListGroupNameAddByRoot()
        {
            List<common.GroupUserAndNickName> list1 = new List<common.GroupUserAndNickName>();
            for (int i = 0; i < friendsRoot.MemberCount; i++)
            {
                if (friendsRoot.MemberList[i].UserName.Contains("@@"))
                {
                    common.GroupUserAndNickName info = new common.GroupUserAndNickName();
                    info.NickName = friendsRoot.MemberList[i].NickName;
                    info.UserName = friendsRoot.MemberList[i].UserName;
                    list1.Add(info);
                }
            }

            foreach (var item in list_GroupName)
            {
                if (!list1.Contains(item))
                    list1.Add(item);
            }
            return list1;
        }

        #region 新的通讯

        private void button1_Click_1(object sender, EventArgs e)
        {
            _scm.SendMsg(textBox1.Text);
        }


        
        SocketClientManager _scm = null;
        string ip = "127.0.0.1";
        private int port = 0;

        private void InitSocket()
        {
            port=MyDB.LocalhostIP.CheckServerPort();
            if (port == 0)
            {
                MessageBox.Show("未能发现服务端口");
                return;
            }
            _scm = new SocketClientManager(ip, port);
            _scm.OnReceiveMsg += OnReceiveMsg;
            _scm.OnConnected += OnConnected;
            _scm.OnFaildConnect += OnFaildConnect;
            _scm.Start();
        }

        private void OnFaildConnect()
        {
            Console.WriteLine(GetDateNow() + "  " + "连接服务器" + ip + " : " + port + " 成功\r\n");
        }

        private void OnConnected()
        {
            Console.WriteLine(GetDateNow() + "  " + "连接服务器" + ip + " : " + port + "成功\r\n");
            string ipClient = _scm._socket.LocalEndPoint.ToString().Split(':')[0];
        }
        /// <summary>
        /// 接收到的消息
        /// </summary>
        private void OnReceiveMsg()
        {
            byte[] buffer = _scm.socketInfo.buffer;
            string msg = Encoding.UTF8.GetString(buffer).Replace("\0", "");
            if (msg != "")
            {
                try
                {
                    MyDB.Model.BaseMessage info = JsonConvert.DeserializeObject<MyDB.Model.BaseMessage>(msg);
                    switch (info.TypeID)
                    {
                        case "打开群组":
                            OpenGroup();
                            break;
                        case "关闭所有":
                            Application.Exit();
                            Cef.Shutdown();
                            web.Dispose();
                            break;
                        case "更改备注":
                            UpdateRemarkName(info);
                            break;
                        case "关闭当前":
                            Application.Exit();
                            Cef.Shutdown();
                            web.Dispose();
                            break;
                        case "刷新当前用户":
                            ffAsync();
                            break;
                        default :break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("接收消息解析异常,消息："+msg);
                }
            }
        }
        private void UpdateRemarkName(MyDB.Model.BaseMessage info)
        {
            //MessageBox.Show(info.Data);
            string useriD = JsonConvert.DeserializeObject<MyDB.Model.UserInfoModel>(info.Data).ID.ToString();
            UpdateUserTag(useriD);
        }
        private void  OpenGroup()
        {
            new Frm.FrmGroupUser().Show();
        }

      
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        private void SendServerMsgs(string msg)
        {
            MyDB.Model.BaseMessage baseinfo = new MyDB.Model.BaseMessage();
            baseinfo.ClientIP = _scm._socket.LocalEndPoint.ToString();
            baseinfo.TypeID = MyDB.Model.MessageType.更新昵称.ToString();
            baseinfo.Uin = wxUin;
            baseinfo.Uuid = Form1.uuid.ToString();
            baseinfo.WechatName = msg;
            string jsonstr = JsonConvert.SerializeObject(baseinfo);
            _scm.SendMsg(jsonstr);
        }
        public string GetDateNow()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        private void SendServer_NewMsg()
        {
            MyDB.Model.BaseMessage baseinfo = new MyDB.Model.BaseMessage();
            baseinfo.ClientIP = _scm._socket.LocalEndPoint.ToString();
            baseinfo.TypeID = MyDB.Model.MessageType.新消息.ToString();
            baseinfo.Uin = wxUin;
            baseinfo.Uuid = Form1.uuid.ToString();
            baseinfo.WechatName = NickName;
            string jsonstr = JsonConvert.SerializeObject(baseinfo);
            _scm.SendMsg(jsonstr);
        }
        /// <summary>
        /// 发送个人面板
        /// </summary>
        /// <param name="baseinfo"></param>
        private void SendServer_MsgUserInfo(MyDB.Model.BaseMessage baseinfo)
        {
            string jsonstr = JsonConvert.SerializeObject(baseinfo);
            _scm.SendMsg(jsonstr);
        }
        /// <summary>
        /// 更新头像
        /// </summary>
        private void SendServer_UserImg(string uuid,string filepath)
        {
            MyDB.Model.BaseMessage baseinfo = new MyDB.Model.BaseMessage();
            baseinfo.ClientIP = _scm._socket.LocalEndPoint.ToString();
            baseinfo.TypeID = MyDB.Model.MessageType.更新用户头像.ToString();
            baseinfo.Uin = wxUin;
            baseinfo.Uuid = Form1.uuid.ToString();
            baseinfo.WechatName = NickName;
            string jsonstr = JsonConvert.SerializeObject(baseinfo);
            _scm.SendMsg(jsonstr);

        }
        #endregion
    }
}


