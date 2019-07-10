using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Browserform.common
{
    /// <summary>
    /// 用户消息
    /// </summary>
    public class UserMessage
    {
        //发送消息url
        private static string _sendmsg_url = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid=";
        public string SendMsg(WxMsgParsed info, CookieContainer CookiesContainer, WXMsg_Message msg, bool showOnly)
        {
            //发送
            if (!showOnly)
            {
               return  SendMsg(info,CookiesContainer, msg.Msg, msg.From, msg.To, msg.Type);
            }
            return "";
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        public string  SendMsg(WxMsgParsed info, CookieContainer CookiesContainer, string msg, string from, string to, int type)
        {
            string Msg_Result = String.Empty;
            string SKey =info.SKey;
            string Pass_Ticket = info.Pass_Ticket;
            string sid = info.Sid;
            string uin = info.Uin;

           // Console.WriteLine("发送消息:" + DateTime.Now.ToString());
            string msg_json = "{{" +
            "\"BaseRequest\":{{" +
                "\"DeviceID\" : \"e441551176\"," +
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



            if (sid != null && uin != null)
            {
                msg_json = string.Format(msg_json, sid, uin, msg, from, to, type, SKey, DateTime.Now.Millisecond, DateTime.Now.Millisecond, DateTime.Now.Millisecond);

                byte[] bytes = BaseService.SendPostRequest(CookiesContainer,_sendmsg_url + sid + "&lang=zh_CN&pass_ticket=" +Pass_Ticket, msg_json);

                Msg_Result = Encoding.UTF8.GetString(bytes);
            }
            return Msg_Result;
        }

    }
    /// <summary>
    /// 微信消息
    /// </summary>
    public class WXMsg_Message
    {
        /// <summary>
        /// 消息发送方
        /// </summary>
        public string From
        {
            get;
            set;
        }
        /// <summary>
        /// 消息接收方
        /// </summary>
        public string To
        {
            set;
            get;
        }
        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool Readed
        {
            get;
            set;
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Msg
        {
            get;
            set;
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int Type
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 消息体参数 
    /// </summary>
    public class WxMsgParsed
    {
        private string sKey = "";
        private string pass_Ticket = "";
        private string sid = "";
        private string uin = "";

        public string SKey { get => sKey; set => sKey = value; }
        public string Pass_Ticket { get => pass_Ticket; set => pass_Ticket = value; }
        public string Sid { get => sid; set => sid = value; }
        public string Uin { get => uin; set => uin = value; }
    }
}
