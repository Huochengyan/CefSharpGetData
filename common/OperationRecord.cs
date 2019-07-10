using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Browserform
{
    //操作聊天记录实现
    class OperationRecord
    {

        /// <summary>
        /// 保存聊天记录的时候，去查找聊天记录文件下。微信昵称的文件夹。然后创建并保存跟好友的聊天
        /// </summary>
        public static string GetUserFile(string MyNickName, string FriendNickName)
        {
            string RecordFile = string.Format("{0}\\{1}", Xml.GetWorkDirectory(), "聊天记录");
            if (!Directory.Exists(RecordFile))
            {
                Directory.CreateDirectory(RecordFile);
            }

            string UserFile = RecordFile + @"\" + MyNickName;
            if (!Directory.Exists(UserFile))
            {
                Directory.CreateDirectory(UserFile);
            }
            string ChatRecordFile = string.Format("{0}\\{1}", UserFile, FriendNickName)+".xml";
            return ChatRecordFile;
        }

        //找到所有的聊天记录文件
        public static List<string> GetXmlFileName(string MyNickName)
        {
            List<string> fileList = new List<string>();
            string RecordFile = string.Format("{0}\\{1}", Xml.GetWorkDirectory(), "聊天记录");
            if (!Directory.Exists(RecordFile))
            {
                return new List<string>(); //不存在，返回
            }

            string UserFile = RecordFile + @"\" + MyNickName;
            if (!Directory.Exists(UserFile))
            {
                return new List<string>(); //不存在没，返回
            }
            var files = Directory.GetFiles(UserFile, "*.xml", SearchOption.AllDirectories);
            
            foreach(string i in files)
            {
                fileList.Add(System.IO.Path.GetFileNameWithoutExtension(i));
            }
            return fileList;
        }

        /// <summary>
        /// 保存二维码文件夹
        /// </summary>
        public static string QRFile()
        {
            string QRFile = string.Format("{0}\\{1}", Xml.GetWorkDirectory(), "二维码");
            if (!Directory.Exists(QRFile))
            {
                Directory.CreateDirectory(QRFile);
            }
            return QRFile;
        }


        /// <summary>
        /// 记录单挑消息消息
        /// </summary>
        /// <returns></returns>
        public static bool WriteRecord(Record record)
        {
            try
            {
                string RecordFile = GetUserFile(record.MyNick, record.ChatNick);
                if (!File.Exists(RecordFile))
                {
                    CreateXmlFile(RecordFile, "聊天记录");
                }
                MessageInfo lastMessage = GetLast(RecordFile);
                if (compareMessage(record.MessageList[0], lastMessage))
                {
                    return true;
                }
                if (record.MessageList[0].Content == "" || record.MessageList[0].Content == null)
                    return true;
                XmlDocument XmlDoc = GetXmlDocument(RecordFile);
                XmlNode rootXml = XmlDoc.SelectSingleNode("聊天记录");
                XmlElement xe = XmlDoc.CreateElement("message");

                xe.SetAttribute("content", record.MessageList[0].Content);
                xe.SetAttribute("time", record.MessageList[0].time);
                xe.SetAttribute("type", record.MessageList[0].MessageType.ToString());
                xe.SetAttribute("friendNick", record.MessageList[0].FriendNick);

                XmlNode xn = rootXml.LastChild;
                rootXml.InsertAfter(xe, xn);
                return SaveXmlDocument(XmlDoc, RecordFile);
            }
            catch(Exception ex)
            {
                //Trace.TraceError("记录单挑信息失败:"+ex.Message);
                return false;
            }
        }


        public static bool WriteMessage(MessageInfo message,string my,string fri)
        {
            try
            {
                string RecordFile = GetUserFile(my, fri);
                if (!File.Exists(RecordFile))
                {
                    CreateXmlFile(RecordFile, "聊天记录");
                }

                XmlDocument XmlDoc = GetXmlDocument(RecordFile);
                XmlNode rootXml = XmlDoc.SelectSingleNode("聊天记录");
                XmlElement xe = XmlDoc.CreateElement("message");

                if (message.Content == "" || message.Content == null)
                    return true;

                xe.SetAttribute("content", message.Content);
                xe.SetAttribute("time", message.time);
                xe.SetAttribute("type", message.MessageType.ToString());
                xe.SetAttribute("friendNick", message.FriendNick);

                XmlNode xn = rootXml.LastChild;
                rootXml.InsertAfter(xe, xn);
                return SaveXmlDocument(XmlDoc, RecordFile);
            }
            catch(Exception ex)
            {
                //Trace.TraceError("记录信息失败:"+ex.Message);
                return false;
            }
        }



        //写入面板上所有信息。取xml文件里面最新一条消息，和record对比。排除掉特定时间之后的

        public static void WriteMutiRecord(Record record)
        {
            try
            {
                if (record.MessageList.Count == 0)
                    return;
                //xml文件不存在，直接保存

                string RecordFile = GetUserFile(record.MyNick, record.ChatNick);
                if (!File.Exists(RecordFile))
                {
                    CreateXmlFile(RecordFile, "聊天记录");
                    //直接保存
                    foreach (MessageInfo i in record.MessageList)
                    {
                        WriteMessage(i, record.MyNick, record.ChatNick);
                    }
                    return;
                }
                else
                {
                    MessageInfo lastMessage = GetLast(RecordFile);
                    if (lastMessage.Content == null)
                    {
                        //保存全部
                        //直接保存
                        foreach (MessageInfo i in record.MessageList)
                        {
                            WriteMessage(i, record.MyNick, record.ChatNick);
                        }

                    }
                    int index = -1;//表示着上次最后消息的次序，如果为-1,保存全部
                    for (int i = 0; i < record.MessageList.Count; i++)
                    {
                        if (compareMessage(record.MessageList[i], lastMessage))
                        {
                            index = i + 1;
                        }
                    }

                    if (index == -1)//保存全部
                    {
                        //直接保存
                        foreach (MessageInfo i in record.MessageList)
                        {
                            WriteMessage(i, record.MyNick, record.ChatNick);
                        }
                    }
                    else
                    {
                        for (int i = index; i < record.MessageList.Count; i++)
                        {
                            WriteMessage(record.MessageList[i], record.MyNick, record.ChatNick);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //Trace.TraceError("批量记录信息失败:" + ex.Message);
            }
        }

        public static bool compareMessage(MessageInfo a,MessageInfo b) //比较时间，只比较时分，不比较秒
        {
            try
            {
                if (b.Content == null)
                    return false;
                if (a.Content == null)
                    return true;
                if (a.Content.Equals(b.Content) && a.MessageType == b.MessageType && a.time.Equals(b.time))
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                //Trace.TraceError("compareMessage异常" + ex.Message);
                return false;
            }
        }

        //找出特定xml文件的最后一行信息
        public static MessageInfo GetLast(string filePath)
        {
            try
            {
                MessageInfo mes = new MessageInfo();
                XmlDocument clsXmlDoc = GetXmlDocument(filePath);
                foreach (XmlNode nd in clsXmlDoc.ChildNodes)
                {
                    if (nd.Name == "聊天记录")
                    {
                        if (nd.ChildNodes.Count != 0)
                        {
                            mes.Content = nd.LastChild.Attributes["content"].Value;
                            mes.MessageType = Convert.ToInt16(nd.LastChild.Attributes["type"].Value);
                            mes.time = nd.LastChild.Attributes["time"].Value;
                            mes.FriendNick = nd.LastChild.Attributes["friendNick"].Value;
                        }
                    }
                }
                return mes;
            }
            catch(Exception ex)
            {
                //Trace.TraceError("GetLast异常:" + ex.Message);
                return new MessageInfo();
            }
        }

        //读取消息
        public static Record ReadRecord(string my,string fri)
        {
            Record myrecord = new Record();
            myrecord.MessageList = new List<MessageInfo>();
            myrecord.MyNick = my;
            myrecord.ChatNick = fri;
            string RecordFile = GetUserFile(my, fri);
            if (!File.Exists(RecordFile))
            {
                return new Record();
            }
            XmlDocument clsXmlDoc = GetXmlDocument(RecordFile);
            foreach (XmlNode nd in clsXmlDoc.ChildNodes)
            {
                if (nd.Name == "聊天记录")
                {
                    foreach (XmlNode x in nd.ChildNodes)
                    {
                        if (x.Name == "message")
                        {
                            MessageInfo mes = new MessageInfo();
                            mes.Content= x.Attributes["content"].Value;
                            mes.MessageType= Convert.ToInt16(x.Attributes["type"].Value);
                            mes.time = x.Attributes["time"].Value;
                            mes.FriendNick= x.Attributes["friendNick"].Value;
                            myrecord.MessageList.Add(mes);
                        }
                    }
                }
            }
            return myrecord;
       }


        //创建消息文件
        public static bool CreateXmlFile(string szFileName, string szRootName)
        {
            XmlDocument clsXmlDoc = new XmlDocument();
            clsXmlDoc.AppendChild(clsXmlDoc.CreateXmlDeclaration("1.0", "GBK", null));
            clsXmlDoc.AppendChild(clsXmlDoc.CreateNode(XmlNodeType.Element, szRootName, ""));

            try
            {
                clsXmlDoc.Save(szFileName);
                return true;
            }
            catch(Exception ex)
            {
                //Trace.TraceError("CreateXmlFile异常:"+ex.Message);
                return false;
            }
        }







    /// <summary>
    /// 从XML文件获取对应的XML文档对象
    /// </summary>
    /// <param name="szXmlFile">XML文件</param>
    /// <returns>XML文档对象</returns>
    public static XmlDocument GetXmlDocument(string szXmlFile)
    {
        if (Xml.IsEmptyString(szXmlFile))
            return null;
        if (!File.Exists(szXmlFile))
            return null;
        XmlDocument clsXmlDoc = new XmlDocument();
        try
        {
            clsXmlDoc.Load(szXmlFile);
        }
        catch
        {
            return null;
        }
        return clsXmlDoc;
    }

    /// <summary>
    /// 将XML文档对象保存为XML文件
    /// </summary>
    /// <param name="clsXmlDoc">XML文档对象</param>
    /// <param name="szXmlFile">XML文件</param>
    /// <returns>bool:保存结果</returns>
    public static bool SaveXmlDocument(XmlDocument clsXmlDoc, string szXmlFile)
    {
        if (clsXmlDoc == null)
            return false;
        if (Xml.IsEmptyString(szXmlFile))
            return false;
        try
        {
            if (File.Exists(szXmlFile))
                File.Delete(szXmlFile);
        }
        catch
        {
            return false;
        }
        try
        {
            clsXmlDoc.Save(szXmlFile);
        }
        catch
        {
            return false;
        }
        return true;
    }

}
}
