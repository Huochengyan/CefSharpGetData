using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Browserform
{
    //XML和文件处理工具类
   public class Xml
    {
        //系统配置文件
        public static string CONFIG_FILE = "System.xml";

        /// <summary>
        /// 得到程序工作目录
        /// </summary>
        /// <returns></returns>
        public static string GetWorkDirectory()
        {
            try
            {
                return Path.GetDirectoryName(typeof(Xml).Assembly.Location);
            }
            catch
            {
                return Environment.CurrentDirectory;
            }
        }

        /// <summary>
        /// 判断字符串是否为空串
        /// </summary>
        /// <param name="szString">目标字符串</param>
        /// <returns>true:为空串;false:非空串</returns>
        public static bool IsEmptyString(string szString)
        {
            if (szString == null)
                return true;
            if (szString.Trim() == string.Empty)
                return true;
            return false;
        }


        /// <summary>
        /// 从配置文件获取服务器地址
        /// </summary>
        /// <returns></returns>
        public static string GetServerUrl()
        {
            string url = "";
            string szConfigFile = string.Format("{0}\\{1}", GetWorkDirectory(), CONFIG_FILE);
            if (!File.Exists(szConfigFile))
            {
                if (!CreateXmlFile(szConfigFile, "System"))
                    throw new Exception("配置文件不存在");
            }
            XmlDocument XmlDoc = GetXmlDocument(szConfigFile);
            XmlNode rootXml = XmlDoc.SelectSingleNode("System");

            foreach (XmlNode xn in rootXml.ChildNodes)
            {
                if (xn.Name == "Server")
                {
                    XmlElement ee = (XmlElement)xn.ChildNodes[0];
                    url = ee.GetAttribute("url");
                }
            }
            return url;
        }


        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <param name="szFileName">XML文件</param>
        /// <param name="szRootName">根节点名</param>
        /// <returns>bool</returns>
        public static bool CreateXmlFile(string szFileName, string szRootName)
        {
            if (szFileName == null || szFileName.Trim() == "")
                return false;
            if (szRootName == null || szRootName.Trim() == "")
                return false;

            XmlDocument clsXmlDoc = new XmlDocument();
            clsXmlDoc.AppendChild(clsXmlDoc.CreateXmlDeclaration("1.0", "GBK", null));
            clsXmlDoc.AppendChild(clsXmlDoc.CreateNode(XmlNodeType.Element, szRootName, ""));

            XmlNode userconfigNode = clsXmlDoc.SelectSingleNode(szRootName);
            userconfigNode.AppendChild(clsXmlDoc.CreateNode(XmlNodeType.Element, "Server", ""));

            XmlNode appSettingNode = userconfigNode.SelectSingleNode("Server");
            XmlElement xe = clsXmlDoc.CreateElement("key");
            xe.SetAttribute("url", "https://ds.mrray.cn/");
            appSettingNode.AppendChild(xe);

            try
            {
                clsXmlDoc.Save(szFileName);
                return true;
            }
            catch
            {
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
            if (IsEmptyString(szXmlFile))
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
            if (IsEmptyString(szXmlFile))
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






        public static string GetDevNum(int uuid)
        {
            try
            {
                string FilePath = string.Format("{0}\\{1}", Xml.GetWorkDirectory(), "设备号");
                if (!Directory.Exists(FilePath))
                {
                    return "";
                }
                //新建文本文件写入uuid和设备号
                string DevPath = string.Format("{0}\\{1}", FilePath, uuid.ToString()) + ".txt";
                if (!File.Exists(DevPath))
                {
                    MessageBox.Show("不存在");
                }
                return System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(DevPath));
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
