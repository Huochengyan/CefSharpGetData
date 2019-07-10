using MyDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Browserform.Form1;

namespace Browserform.common
{

    public class WXService
    {

        #region   新老微信标识 

        /// <summary>
        /// 新微信获取通讯录
        /// </summary>
        public static string Wx2_Getcontact = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?";

        /// <summary>
        /// 老微信获取通讯录
        /// </summary>
        public static string Wx_Getcontact = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?";

        /// <summary>
        /// 发送消息 Url
        /// </summary>
        public static string Wx2_sendmsg_url_ = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid=";

        //  public static string 

        #endregion

        #region 新老微信统配
        /// <summary>
        /// 统配下载别人发给我的图片消息
        /// </summary>
        public static string Wx2_NewImgMsg = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID={0}&skey={1}";

        public static string Wx_OldimgMsg = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID={0}&skey={1}";

        /// <summary>
        /// 新微信改备注 网址 
        /// </summary>
        public static string Wx2_UpdateNote = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxoplog";
        /// <summary>
        /// 老微信改备注
        /// </summary>
        public static string Wx_UpdateNote = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxoplog";
       
        #endregion


        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public static CookieContainer CookiesContainer;
       
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] SendGetRequest(string url)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }

                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据 请求新微信
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static JObject SendPostRequest(string url, string body, CookieContainer myCookiesContainer)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.CookieContainer = myCookiesContainer;  //启用cookie
                request.Accept = "application/json, text/plain, */*";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Headers.Add("Origin", "https://wx2.qq.com");
                request.KeepAlive = true;
                request.UserAgent= "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";              
                request.ContentLength = request_body.Length;
                request.ContentType = "application/json;charset=UTF-8";
                request.Referer = "https://wx2.qq.com/?&lang=zh_CN";
                request.Host = "wx2.qq.com";
                request.ServicePoint.Expect100Continue = false;

                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_body, 0, request_body.Length);

               
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();
               
                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                request_stream.Close();
                response.Close();

                //Console.WriteLine("##########################################################################################");
                //Console.WriteLine("request.Url:\r\n" + url.ToString());
                //Console.WriteLine("request.postdata:\r\n" + body.ToString());
                //Console.WriteLine("request.Headers详细为:\r\n" + request.Headers.ToString());
                //Console.WriteLine("request.Cookie 详细为:\r\n" + GetAllCookies(request.CookieContainer).ToString());
                //Console.WriteLine("##########################################################################################");
                string strdata = String.Empty;

                if (response.ContentEncoding == "gzip") {
                    strdata = DeCompress(buf);
                }
                else
                {
                    strdata = Encoding.UTF8.GetString(buf);
                }
                              
                JObject job = (JObject)JsonConvert.DeserializeObject(strdata);
                return job;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据 请求老微信
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static JObject SendPostRequest_Old(string url, string body, CookieContainer myCookiesContainer)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.CookieContainer = myCookiesContainer;  //启用cookie
                request.Accept = "application/json, text/plain, */*";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Headers.Add("Origin", "https://wx.qq.com");
                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
                request.ContentLength = request_body.Length;
                request.ContentType = "application/json;charset=UTF-8";
                request.Referer = "https://wx.qq.com/?&lang=zh_CN";
                request.Host = "wx.qq.com";
                request.ServicePoint.Expect100Continue = false;

                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_body, 0, request_body.Length);


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                request_stream.Close();
                response.Close();

                //Console.WriteLine("##########################################################################################");
                //Console.WriteLine("request.Url:\r\n" + url.ToString());
                //Console.WriteLine("request.postdata:\r\n" + body.ToString());
                //Console.WriteLine("request.Headers详细为:\r\n" + request.Headers.ToString());
                //Console.WriteLine("request.Cookie 详细为:\r\n" + GetAllCookies(request.CookieContainer).ToString());
                //Console.WriteLine("##########################################################################################");
                string strdata = String.Empty;

                if (response.ContentEncoding == "gzip")
                {
                    strdata = DeCompress(buf);
                }
                else
                {
                    strdata = Encoding.UTF8.GetString(buf);
                }

                JObject job = (JObject)JsonConvert.DeserializeObject(strdata);
                return job;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)

        {

            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",

                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (property != null)

            {

                var collection = property.GetValue(header, null) as NameValueCollection;

                collection[name] = value;

            }

        }
        /// <summary>
        /// 1 获取好友列表 
        /// </summary>
        /// <param name="url">路径</param>
        /// <param name="myCookieContainer">Cookie</param>
        /// <returns></returns>
        public static JObject GetContactByUrl(string url, CookieContainer myCookieContainer1,string CookieStr)
        {
            StringBuilder content = new StringBuilder();
            HttpWebRequest request = HttpWebRequest.Create(url.ToString()) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = myCookieContainer1;
            CookiesContainer = myCookieContainer1;
            HttpWebResponse response = null;
            using (WebResponse wr = request.GetResponse())
            {
                response = wr as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    // 开始读取数据
                    Char[] sReaderBuffer = new Char[256];
                    int count = sReader.Read(sReaderBuffer, 0, 256);
                    while (count > 0)
                    {
                        String tempStr = new String(sReaderBuffer, 0, count);
                        content.Append(tempStr);
                        count = sReader.Read(sReaderBuffer, 0, 256);
                    }
                    // 读取结束
                    sReader.Close();
                }
            }

            JObject job = (JObject)JsonConvert.DeserializeObject(content.ToString());

            //Console.WriteLine("================获取通讯录是的CookieList================================");
            //Console.WriteLine(GetAllCookies(myCookieContainer1));
            //Console.WriteLine("================================================");
            return job;
        }
 
        /// <summary>
        /// 获取所有Cookie信息
        /// </summary>
        /// <param name="cc">Cookie信息 字符串</param>
        /// <returns></returns>
        public static string  GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
            System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            StringBuilder sbc = new StringBuilder();
            foreach (Cookie cookie in lstCookies)
            {
                sbc.AppendFormat("{0}={1};\r\n"
                , cookie.Name, cookie.Value);
            }

            return sbc.ToString();
        }



        #region   发送消息
        /// <summary>
        /// 自动回复发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        public static string AutoSendMsg(CookieContainer myCookieContainer, string msg, string from, string to, int type, LoginRedirectResult LoginRedirectResult)
        {
            string MsgResult = String.Empty;

            string msg_json = "{{" +
            "\"BaseRequest\":{{" +
                "\"DeviceID\" : \"789789\"," +
                "\"Sid\" : \"{0}\"," +
                "\"Skey\" : \"{6}\"," +
                "\"Uin\" : \"{1}\"" +
            "}}," +
            "\"Msg\" : {{" +
                "\"ClientMsgId\" : {8}," +
                "\"Content\" : \"{2}\"," +
                "\"FromUserName\" : \"{3}\"," +
                "\"LocalID\" : {9}," +
                "\"ToUserName\" : \"{4}\"," +
                "\"Type\" : {5}" +
            "}}," +
            "\"rr\" : {7}" +
            "}}";

            string sid = LoginRedirectResult.wxsid;
            string uin = LoginRedirectResult.wxuin;

            if (sid != null && uin != null)
            {
                msg_json = string.Format(msg_json, sid, uin, msg, from, to, type, LoginRedirectResult.skey, DateTime.Now.Millisecond, DateTime.Now.Millisecond, DateTime.Now.Millisecond);

                byte[] bytes = BaseService.SendPostRequest(myCookieContainer, Wx2_sendmsg_url_ + sid + "&lang=zh_CN&pass_ticket=" + LoginRedirectResult.pass_ticket, msg_json);

                MsgResult = Encoding.UTF8.GetString(bytes);
            }
            return MsgResult;
        }
        #endregion


        public static string DeCompress(byte[] str)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            stream.Write(str, 0, str.Length);
            stream.Position = 0;
            GZipStream zip = new GZipStream(stream, CompressionMode.Decompress);
            System.IO.StreamReader rd = new System.IO.StreamReader(zip);
            return new System.IO.StringReader(rd.ReadToEnd()).ReadToEnd();
        }

    }
    public class LoginCookItem
    {
        public string Name = "";
        public string Value = "";
        public string Domain = "";
    }
}
