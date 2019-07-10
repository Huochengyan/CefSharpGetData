using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browserform
{
  public class Record
    {
        public string MyNick { get; set; }

        public string ChatNick { get; set; }
        public List<MessageInfo> MessageList { get; set; }
     }

    public class MessageInfo
    {
        // 每条消息有一个好友昵称
        public string FriendNick { get; set; }
        public string Content { get; set; }

        public int MessageType { get; set; }

        public string time { get; set; }

        //public bool IsGroup { get; set; }
    }
}
