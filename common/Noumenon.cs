using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browserform.common
{
    public class Noumenon
    {
    }
    /// <summary>
    /// 请求群组列表消息实体
    /// </summary>
    public class Noumenon_GetGroupUser
    {
        private string _ToUserName = "";
        private string _Url = "";
        private string _PostData = "";
        private string _FromUserName = "";
        /// <summary>
        /// 要请求的群组的UserName  包含“@@”de的UserName
        /// </summary>
        public string ToUserName { get => _ToUserName; set => _ToUserName = value; }
       
        /// <summary>
        /// 请求的User地址
        /// </summary>
        public string Url { get => _Url; set => _Url = value; }

        /// <summary>
        /// 要发送的实体数据
        /// </summary>
        public string PostData { get => _PostData; set => _PostData = value; }

        /// <summary>
        /// 来自哪里的消息 
        /// </summary>
        public string FromUserName { get => _FromUserName; set => _FromUserName = value; }
    }
}
