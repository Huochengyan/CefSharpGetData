using MyDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Browserform.common
{
    /// <summary>
    /// 加载用户好友信息类
    /// </summary>
    public class DownFriendsInfo
    {
        static  string FriendsImg = System.Environment.CurrentDirectory + "\\RecvFiles\\";
      

        public static System.Action SaveFriendsImg(MyDB.Root root_friend, CookieContainer myCookieContainer, string wxUin)
        {
            try
            {
                string wxUinFile = FriendsImg + wxUin + "/";
                for (int i = 0; i < root_friend.MemberList.Count; i++)
                {
                    string imgurl = "https://wx2.qq.com" + root_friend.MemberList[i].HeadImgUrl;
                    string FileName = root_friend.MemberList[i].UserName.ToString().Replace("@", "") + ".jpg";
                    CommonTools.WebRequestAction.getimages(imgurl, myCookieContainer, wxUinFile, FileName);
                }
            }
            catch (Exception ex)
            { }
            return null;

        }

        /// <summary>
        /// 获得信息
        /// </summary>
        /// <returns></returns>
        public static Root GetFriends(string url, CookieContainer myCookieContainer)
        {
            Root root = new Root();
            try
            {
                StringBuilder content = new StringBuilder();
                HttpWebRequest request = HttpWebRequest.Create(url.ToString()) as HttpWebRequest;
                request.Method = "GET";
                request.CookieContainer = myCookieContainer;
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
                //读取好友列表并保存到数据库
                root = JsonConvert.DeserializeObject<MyDB.Root>(content.ToString());
                return root;
            }
            catch (Exception ex)
            {
                return root;
            }
        }


        /// <summary>
        /// 保存接收到的 发的图片信息
        /// </summary>
        /// <returns></returns>
        public static string SaveSendImgPath(CookieContainer myCookieContainer, ReceiveMessageRoot msg, LoginRedirectResult LoginRedirectResult)
        {
            try
            {
                string url = String.Empty;
                if (LoginRedirectResult.WxorWx2 == 0) return "";
                if (LoginRedirectResult.WxorWx2 == 1)
                    url = String.Format(common.WXService.Wx_OldimgMsg, msg.AddMsgList[0].MsgId, LoginRedirectResult.skey);
                if (LoginRedirectResult.WxorWx2 == 2)
                    url = String.Format(common.WXService.Wx2_NewImgMsg,msg.AddMsgList[0].MsgId, LoginRedirectResult.skey);
                string Imgdir = System.Environment.CurrentDirectory + "\\RecvFiles\\"+ LoginRedirectResult.wxuin + "\\";
                string FileName = msg.AddMsgList[0].MsgId + ".jpg";
                string resultPath = CommonTools.WebRequestAction.getimages(url, myCookieContainer, Imgdir, FileName);
                return resultPath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
