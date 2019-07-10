using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Browserform.common
{
    public  class HtmlGetInfo
    {
        /// <summary>
        /// 获得 NickName
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static string GetNickName(string Html)
        {
            string NickName = "";

            // 定义正则表达式用来匹配 img 标签 
            try
            {
                string spanreg = String.Format(@"<span ng-bind-html='.*'(>).*</span>").Replace("'", "\"");
                Regex regImg = new Regex(spanreg, RegexOptions.IgnoreCase);

                // 搜索匹配的字符串 
                MatchCollection matches = regImg.Matches(Html); 
                string htmlSpan = matches[0].Value;
                NickName = htmlSpan.Substring(htmlSpan.IndexOf(">")+1).ToString();
                NickName = NickName.Replace("</span>", "");



                return NickName;
            }
            catch (Exception ex)
            {
                return NickName;
            }
        }
        /// <summary>
        /// 获得非通讯录里的 群组的NickNickName
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static string GetGroupNickName(string Html)
        {
            string GroupNickName = "";
            // 定义正则表达式用来匹配 img 标签 
            try
            {

                Regex re = new Regex(@"<div[^>]*class=""title_wrap""[^>]*>[\s\S]*?</div>", RegexOptions.Multiline);
                MatchCollection matches1 = re.Matches(Html);
                string ftr=matches1[0].Value;
                string f1 = ftr.Substring(ftr.IndexOf("<a"), (ftr.IndexOf("</a>")- ftr.IndexOf("<a")));
                GroupNickName = f1.Substring(f1.IndexOf('>')+1);
                return GroupNickName;
            }
            catch (Exception ex)
            {
                return GroupNickName;
            }
        }
        public static string GetGroupuserName(string Html)
        {
            string GroupNickName = "";
            string UserName = "";
            // 定义正则表达式用来匹配 img 标签 
            try
            {

                Regex re = new Regex(@"<div[^>]*class=""title_wrap""[^>]*>[\s\S]*?</div>", RegexOptions.Multiline);
                MatchCollection matches1 = re.Matches(Html);
                string ftr = matches1[0].Value;
                string f1 = ftr.Substring(ftr.IndexOf("<a"), (ftr.IndexOf("</a>") - ftr.IndexOf("<a")));
                GroupNickName = f1.Substring(f1.IndexOf('>') + 1);
                string arr11 = UserName = f1.Substring(f1.IndexOf("data-username")).ToString();
                UserName = arr11.ToString().Substring(0, arr11.IndexOf(' ')).ToString();
                UserName= UserName.Substring(UserName.IndexOf('@'));
                UserName = UserName.Replace("\\", "").ToString().Replace("\""," ");                                
                return UserName;
            }
            catch (Exception ex)
            {
                return GroupNickName;
            }
        }

        

        public static string GetGroupInfoBySay(string html)
        {
            return "";
                   
        }


        /// 测试.
        /// </summary>
        public void Test()
        {
            string pageURL = "http://www.google.com.hk/search?hl=zh-CN&source=hp&q=%E5%8D%9A%E6%B1%87%E6%95%B0%E7%A0%81&aq=f&aqi=g2&aql=&oq=&gs_rfai=";
            Uri uri = new Uri(pageURL);
            string queryString = uri.Query;
            NameValueCollection col = GetQueryString(queryString);
            string searchKey = col["q"];
            //结果 searchKey = "博汇数码"
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString)
        {
            return GetQueryString(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncoded"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString, Encoding encoding, bool isEncoded)
        {
            queryString = queryString.Replace("?", "");
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(queryString))
            {
                int count = queryString.Length;
                for (int i = 0; i < count; i++)
                {
                    int startIndex = i;
                    int index = -1;
                    while (i < count)
                    {
                        char item = queryString[i];
                        if (item == '=')
                        {
                            if (index < 0)
                            {
                                index = i;
                            }
                        }
                        else if (item == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (index >= 0)
                    {
                        key = queryString.Substring(startIndex, index - startIndex);
                        value = queryString.Substring(index + 1, (i - index) - 1);
                    }
                    else
                    {
                        key = queryString.Substring(startIndex, i - startIndex);
                    }
                    if (isEncoded)
                    {
                        result[MyUrlDeCode(key, encoding)] = MyUrlDeCode(value, encoding);
                    }
                    else
                    {
                        result[key] = value;
                    }
                    if ((i == (count - 1)) && (queryString[i] == '&'))
                    {
                        result[key] = string.Empty;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 解码URL.
        /// </summary>
        /// <param name="encoding">null为自动选择编码</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                     
                string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return HttpUtility.UrlDecode(str, encoding);
        }



    }
}
