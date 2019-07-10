using MyDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browserform.common
{
    /// <summary>
    /// 群组 操作类 
    /// </summary>
    public class WeChatGroup
    {
        public WeChatGroup()
        { }

        /// <summary>
        /// 添加 群组里的群组成员 好友 
        /// </summary>
        /// <param name="info">添加 的信息</param>
        /// <param name="myCookiesContainer">Cookie</param>
        /// <param name="wxorwx2">新老微信标识 老微信 1 新微信 2</param>
        /// <returns></returns>
        public JObject AddGroupUser(AddGroupUser info, System.Net.CookieContainer myCookiesContainer, int wxorwx2)
        {
            string url = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxverifyuser?r=" + info.R + "&lang=zh_CN&pass_ticket="
            + info.pass_ticket;
            if (wxorwx2 == 1)//老微信 
            {
                url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxverifyuser?r=" + info.R + "&lang=zh_CN&pass_ticket=" + info.pass_ticket;
            }
            JObject job = null;
            string postdate = String.Empty;
            string add1 = "{\"BaseRequest\":{\"Uin\":" + info.Uin + ",\"Sid\":\"" + info.Sid + "\",\"Skey\":\"" + info.Skey + "\",";
            string add2 = "\"DeviceID\":\"" + info.DeviceID + "\"},\"Opcode\":2,\"VerifyUserListSize\":1,\"VerifyUserList\":";
            string add3 = "[{\"Value\":\"" + info.Value + "\",\"VerifyUserTicket\":\"\"}],";
            string add4 = "\"VerifyContent\":\"" + info.VerifyContent + "\",\"SceneListCount\":1,\"SceneList\":[33],\"skey\":\"" + info.Skey + "\"}";
            postdate = add1 + add2 + add3 + add4;
            if (wxorwx2 == 1)
            {
                job = WXService.SendPostRequest_Old(url, postdate, myCookiesContainer);
            }
            else
            {
                job = WXService.SendPostRequest(url, postdate, myCookiesContainer);
            }
            return job;
        }


        /// <summary>
        ///  获得请求群组的信息
        /// </summary>
        /// <param name="myCookiesContainer"></param>
        /// <param name="wxorwx2"></param>
        /// <returns></returns>
        public common.Noumenon_GetGroupUser GetGroupInfo(string GroupUserName, System.Net.CookieContainer myCookiesContainer, int wxorwx2, LoginRedirectResult loginRedirectResult)
        {
            string post_Uin = loginRedirectResult.wxuin;
            string post_Sid = loginRedirectResult.wxsid;
            string post_Skey = loginRedirectResult.skey;
            string post_DeviceID =  CreateNewDeviceID();
            string post_GroupName = GroupUserName;
            string ur1l = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=";
            if (wxorwx2 == 1)
            {
                ur1l = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=";
            }
            else
            {
                ur1l = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=";
            }

            common.Noumenon_GetGroupUser info = new common.Noumenon_GetGroupUser();
            info.Url = String.Format(ur1l + "{0}&lang=zh_CN&pass_ticket={1}", getR().ToString(), loginRedirectResult.pass_ticket);
            info.ToUserName = GroupUserName;
            info.PostData = "{\"BaseRequest\":{\"Uin\":" + post_Uin + ",\"Sid\":\"" + post_Sid + "\",\"Skey\":\"" + post_Skey + "\",\"DeviceID\":\"" + post_DeviceID + "\"},\"Count\":1,\"List\":[{\"UserName\":\"" + GroupUserName + "\",\"EncryChatRoomId\":\"" + "" + "\"}]}";
            return info;
        }


        public List<common.MemberList> GetALLUser(common.Noumenon_GetGroupUser info, int WxorWx2, System.Net.CookieContainer myCookieContainer)
        {
            List<common.MemberList> Group_MemberList = new List<common.MemberList>();
            string froupname = info.ToUserName;
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
                    infoM.GroupUserName = froupname;
                    if (!Group_MemberList.Contains(infoM))
                        Group_MemberList.Add(infoM);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return Group_MemberList;
        }


        long getR()
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
    }
    public class AddGroupUser
    {
        /// <summary>
        /// 当前微信唯一标识 
        /// </summary>
        public string Uin = "";
        /// <summary>
        /// Sid
        /// </summary>
        public string Sid = "";
        /// <summary>
        /// Skey
        /// </summary>
        public string Skey = "";
        /// <summary>
        /// 要添加的人的UserName
        /// </summary>
        public string Value = "";

        /// <summary>
        /// 要添加的群好友的 昵称
        /// </summary>
        public string NickName = "";
        /// <summary>
        /// 登录设备 
        /// </summary>
        public string DeviceID = "";
        /// <summary>
        /// 验证消息 
        /// </summary>
        public string VerifyContent = "";

        /// <summary>
        /// pass_ticket
        /// </summary>
        public string pass_ticket = "";

        /// <summary>
        /// 时间戳
        /// </summary>
        public long R;



    }
}
