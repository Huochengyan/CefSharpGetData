using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browserform.common
{
    #region 微信消息体
    /// <summary>
    /// WeChat msg
    /// </summary>
    public  class WXMsg
    {
        //正常返回结果
        private string Re_0 = "window.synccheck={ retcode: \"0\",selector: \"0\"}";
        //发送消息返回结果
        private string Re_2 = "window.synccheck={ retcode: \"0\",selector: \"2\"}";
        //朋友圈有动态
        private string Re_4 = "window.synccheck={ retcode: \"0\",selector: \"4\"}";
        //有消息返回结果
        private string Re_6 = "window.synccheck={ retcode: \"0\",selector: \"6\"}";
    }
    /// <summary>
    /// 接收的消息类型
    /// </summary>
    public enum WxMsg_Type
    {
        /// <summary>
        /// 文字消息
        /// </summary>
        type_1=1,

        /// <summary>
        ///消息类型未知？
        /// </summary>
        type_2=2,

        /// <summary>
        /// 图片消息
        /// </summary>
        type_3=3,

   
    

        //10000为系统消息。
    }

    /// <summary>
    /// 指定要跳过的消息类型
    /// </summary>
    public enum WxMsg_Type_Neglect
    {
        /// <summary>
        /// 分享消息 
        /// </summary>
        type_49 = 49,


         /// <summary>
         /// 语音消息
        /// </summary>
        type_34 = 34,


        /// <summary>
        /// 自定义动画
        /// </summary>
        type_47 = 47,
    }

    #endregion

    #region 微信组列表

    #endregion

    #region  微信组消息
    /// <summary>
    /// 微信组消息
    /// </summary>
    [Serializable()]
    public class WxGroupMsg
    {
        private BaseRequest _BaseRequest;
        public BaseRequest BaseRequest { get => _BaseRequest; set => _BaseRequest = value; }
        
        private Msg _Msg;
        public Msg Msg { get => _Msg; set => _Msg = value; }
       
        private string _Scene = "";
        public string Scene { get => _Scene; set => _Scene = value; }
    }


    /// <summary>
    /// 微信组消息 - 消息体 登录状态参数
    /// </summary>
    public class BaseRequest {
        public string Uin = "";
        public string Sid = "";
        public string Skey = "";
        public string DeviceID = "";
    }



    /// <summary>
    /// 微信组消息 -  消息体 - 详细 
    /// </summary>
    public class Msg
    {
        public string Type = "";
        public string Content = "";
        public string FromUserName = "";
        /// <summary>
        /// 微信组Username
        /// </summary>
        public string ToUserName = "";
        public string LocalID = "";
        public string ClientMsgId = "";
    }
    #endregion

    #region 微信组成员
    /// <summary>
    /// 微信群组成员 
    /// </summary>
    [Serializable()]
    public class WxGroupContact
    {
        private BaseResponse _BaseResponse;
        /// <summary>
        /// 返回信息
        /// </summary>
        public BaseResponse BaseResponse { get => _BaseResponse; set => _BaseResponse = value; }


        private ContactList _ContactList;
        /// <summary>
        /// 返回的群组成员
        /// </summary>
        public ContactList ContactList { get => _ContactList; set => _ContactList = value; }


        private int _Count = 0;
        /// <summary>
        /// 返回的群组成员个数
        /// </summary>
        public int Count { get => _Count; set => _Count = value; }

    }
    /// <summary>
    /// 返回信息
    /// </summary>
    [Serializable()]
    public class BaseResponse
    {
        private string _Ret = "";
        /// <summary>
        /// 返回状态
        /// </summary>
        public string Ret { get => _Ret; set => _Ret = value; }

        private string _ErrMsg = "";
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrMsg { get => _ErrMsg; set => _ErrMsg = value; }

      
    }
    /// <summary>
    /// 返回的群组成员
    /// </summary>
    [Serializable()]
    public class ContactList {
        private int _Uin =0;
        private string _UserName = "";
        private string _NickName = "";
        private string _HeadImgUrl = "";
        private int _MemberCount = 0;
        private MemberList _memberList=new MemberList();
        private int _ContactFlag = 0;
        /// <summary>
        ///群组 Uin ID
        /// </summary>
        public int Uin { get => _Uin; set => _Uin = value; }
        /// <summary>
        ///群组 UserName
        /// </summary>
        public string UserName { get => _UserName; set => _UserName = value; }
        /// <summary>
        /// 群组昵称
        /// </summary>
        public string NickName { get => _NickName; set => _NickName = value; }
        /// <summary>
        /// 群组 头像路径
        /// </summary>
        public string HeadImgUrl { get => _HeadImgUrl; set => _HeadImgUrl = value; }
        
        /// <summary>
        /// 群组   成员个数
        /// </summary>
        public int MemberCount { get => _MemberCount; set => _MemberCount = value; }
        /// <summary>
        /// 标识
        /// </summary>
        public int ContactFlag { get => _ContactFlag; set => _ContactFlag = value; }
        public string RemarkName { get => _RemarkName; set => _RemarkName = value; }
        public int HideInputBarFlag { get => _HideInputBarFlag; set => _HideInputBarFlag = value; }
        public int Sex { get => _Sex; set => _Sex = value; }
        public string Signature { get => _Signature; set => _Signature = value; }
        public int VerifyFlag { get => _VerifyFlag; set => _VerifyFlag = value; }
        public int OwnerUin { get => _OwnerUin; set => _OwnerUin = value; }
        public string PYInitial { get => _PYInitial; set => _PYInitial = value; }
        public string PYQuanPin { get => _PYQuanPin; set => _PYQuanPin = value; }
        public string RemarkPYInitial { get => _RemarkPYInitial; set => _RemarkPYInitial = value; }
        public string RemarkPYQuanPin { get => _RemarkPYQuanPin; set => _RemarkPYQuanPin = value; }
        public int StarFriend { get => _StarFriend; set => _StarFriend = value; }
        public int AppAccountFlag { get => _AppAccountFlag; set => _AppAccountFlag = value; }
        public int Statues { get => _Statues; set => _Statues = value; }
        public int AttrStatus { get => _AttrStatus; set => _AttrStatus = value; }
        public string Province { get => _Province; set => _Province = value; }
        public string City { get => _City; set => _City = value; }
        public string Alias { get => _Alias; set => _Alias = value; }
        public int SnsFlag { get => _SnsFlag; set => _SnsFlag = value; }
        public int UniFriend { get => _UniFriend; set => _UniFriend = value; }
        public string DisplayName { get => _DisplayName; set => _DisplayName = value; }
        public int ChatRoomId { get => _ChatRoomId; set => _ChatRoomId = value; }
        public string KeyWord { get => _KeyWord; set => _KeyWord = value; }
        public string EncryChatRoomId { get => _EncryChatRoomId; set => _EncryChatRoomId = value; }
        public int IsOwner { get => _IsOwner; set => _IsOwner = value; }
        public MemberList MemberList { get => _memberList; set => _memberList = value; }

        private string _RemarkName = "";
        private int _HideInputBarFlag = 0;
        private int _Sex = 0;
        private string _Signature = "";
        private int _VerifyFlag = 0;
        private int _OwnerUin = 0;
        private string _PYInitial = "";
        private string _PYQuanPin = "";
        private string _RemarkPYInitial = "";
        private string _RemarkPYQuanPin = "";
        private int _StarFriend = 0;
        private int _AppAccountFlag = 0;
        private int _Statues = 0;
        private int _AttrStatus = 0;
        private string _Province = "";
        private string _City = "";
        private string _Alias = "";
        private int _SnsFlag = 0;
        private int _UniFriend = 0;
        private string _DisplayName = "";
        private int _ChatRoomId = 0;
        private string _KeyWord = "";
        private string _EncryChatRoomId = "";
        private int _IsOwner = 0;



    }
    [Serializable]
    public class MemberList
    {
        private int _Uin = 0;
        private string    _UserName = "";
        private string    _NickName = "";
        private string    _AttrStatus = "";
        private string    _PYInitial = "";
        private string    _PYQuanPin = "";
        private string    _RemarkPYInitial = "";
        private string    _RemarkPYQuanPin = "";
        private int _MemberStatus = 0;
        private string    _DisplayName = "";
        private string    _KeyWord = "";

      
        public string UserName { get => _UserName; set => _UserName = value; }
        public string NickName { get => _NickName; set => _NickName = value; }
        public string AttrStatus { get => _AttrStatus; set => _AttrStatus = value; }
        public string PYInitial { get => _PYInitial; set => _PYInitial = value; }
        public string PYQuanPin { get => _PYQuanPin; set => _PYQuanPin = value; }
        public string RemarkPYInitial { get => _RemarkPYInitial; set => _RemarkPYInitial = value; }
        public string RemarkPYQuanPin { get => _RemarkPYQuanPin; set => _RemarkPYQuanPin = value; }
        public string DisplayName { get => _DisplayName; set => _DisplayName = value; }
        public string KeyWord { get => _KeyWord; set => _KeyWord = value; }
        public int Uin { get => _Uin; set => _Uin = value; }
        public int MemberStatus { get => _MemberStatus; set => _MemberStatus = value; }

        /// <summary>
        /// 当前群组成员 所属群组的UserNmae
        /// </summary>
        public string GroupUserName = "";
        /// <summary>
        /// 当前群组成员 所属群组的NickName
        /// </summary>
        public string GroupNickName = "";

    }

    /// <summary>
    /// 群组UserName 和 群组NickName
    /// </summary>
    public class GroupUserAndNickName {

        private string _userName = "";

        private string _nickName = "";
        /// <summary>
        /// 群组  UserName
        /// </summary>
        public string UserName { get => _userName; set => _userName = value; }
        /// <summary>
        /// 群组  NickName
        /// </summary>
        public string NickName { get => _nickName; set => _nickName = value; }
    }
    #endregion
}
