using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Browserform.common
{
    public class Base64Helper
    {
        /// <summary>
        /// 换了采用 登录钱扫码时候获取
        /// </summary>
        /// <param name="strHtml"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string get_image(string strHtml, string filePath)
        {
            string[] arr1 = null;
            try
            {
                arr1 = GetHtmlImageUrlList(strHtml);
                string baseimg = arr1[1].ToString();
                string bsse64 = baseimg.Remove(0, 20);
                byte[] arr = Convert.FromBase64String(bsse64.Replace(" %2B", " +"));
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                return filePath;
            }
            catch (Exception ex)
            {
                MethodBase method = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod();
                string ErrorImg = "";
                for (int i = 0; i < arr1.Length; i++)
                {
                    ErrorImg += arr1[i].ToString();
                }
                CommonTools.ExceptionLogInfo.SaveExceptionInfo(method.ReflectedType.FullName, method.Name, ex.ToString()+"图片数组"+ ErrorImg.ToString()+"错误网页："+ strHtml);
            }
            return "";

        }
        /// <summary> 
        /// 取得HTML中所有图片的 URL。 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
                // 定义正则表达式用来匹配 img 标签 
                Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

                // 搜索匹配的字符串 
                MatchCollection matches = regImg.Matches(sHtmlText);
                int i = 0;
                string[] sUrlList = new string[matches.Count];

                // 取得匹配项列表 
                foreach (Match match in matches)
                    sUrlList[i++] = match.Groups["imgUrl"].Value;
                return sUrlList;
        }



        public static async Task<string> GetUxinAsync(ChromiumWebBrowser web)
        {
            var html = await web.GetSourceAsync();
            Regex reg = new Regex("&amp;uin=(.*)deviceid");
            MatchCollection mc = reg.Matches(html);  //在内容中匹配与正则表达式匹配的字符  
            string str = "";
            foreach (Match m in mc)     //循环匹配到的字符  
            {
                str = m.Value;
                return str;
            }


            return "";
        }

    }
}
