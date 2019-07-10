using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Browserform
{
    class Cloud
    {
        static string ServerUrl = Xml.GetServerUrl();

        /// <summary>
        /// 封装HtppClient请求
        /// </summary>
        /// <param name="request">请求数据模型</param>
        /// <param name="Api">请求服务器接口</param>
        /// <param name="way">get or post</param>
        /// <returns></returns>
        public async static Task<HttpResponseMessage> HttpConnect(object request, string api, string mode)
        {

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                using (var client = new HttpClient())
                {
                    //超时时间：第一次1秒，第二次2秒，第三次3秒。值得商榷
                    // client.Timeout = TimeSpan.FromMilliseconds(3000 * (RequestCount + 1));
                    client.BaseAddress = new Uri(ServerUrl);

                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));

                    if (mode == "Post")
                    {
                        response = await client.PostAsync(api, new StringContent(JsonConvert.SerializeObject(request)));
                    }
                    else if (mode == "Get")
                    {
                        response = await client.GetAsync(api);
                    }

                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return response;
        }

        public async static void Login(string device, string PicPath)
        {
            try
            {
                string responseText = "";
                FileStream fs = new FileStream(PicPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] fileBytes = new byte[fs.Length];
                fs.Read(fileBytes, 0, fileBytes.Length);
                fs.Close(); fs.Dispose();

                HttpRequestClient httpRequestClient = new HttpRequestClient();
                httpRequestClient.SetFieldValue("device", device);
                httpRequestClient.SetFieldValue("pic", Path.GetFileName(PicPath), "application/octet-stream", fileBytes);
                string UploadApiUrl = ServerUrl + "/wgcs/custom/upload";
                httpRequestClient.Upload(UploadApiUrl, out responseText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器连接异常！");
            }
        }


        /// <summary>
        /// description：http post请求客户端
        /// last-modified-date：2012-02-28
        /// </summary>
        public class HttpRequestClient
        {
            #region //字段
            private ArrayList bytesArray;
            private Encoding encoding = Encoding.UTF8;
            private string boundary = String.Empty;
            #endregion

            #region //构造方法
            public HttpRequestClient()
            {
                bytesArray = new ArrayList();
                string flag = DateTime.Now.Ticks.ToString("x");
                boundary = "---------------------------" + flag;
            }
            #endregion

            #region //方法
            /// <summary>
            /// 合并请求数据
            /// </summary>
            /// <returns></returns>
            private byte[] MergeContent()
            {
                int length = 0;
                int readLength = 0;
                string endBoundary = "--" + boundary + "--\r\n";
                byte[] endBoundaryBytes = encoding.GetBytes(endBoundary);

                bytesArray.Add(endBoundaryBytes);

                foreach (byte[] b in bytesArray)
                {
                    length += b.Length;
                }

                byte[] bytes = new byte[length];

                foreach (byte[] b in bytesArray)
                {
                    b.CopyTo(bytes, readLength);
                    readLength += b.Length;
                }

                return bytes;
            }

            /// <summary>
            /// 上传
            /// </summary>
            /// <param name="requestUrl">请求url</param>
            /// <param name="responseText">响应</param>
            /// <returns></returns>
            public bool Upload(String requestUrl, out String responseText)
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

                byte[] responseBytes;
                byte[] bytes = MergeContent();

                try
                {
                    responseBytes = webClient.UploadData(requestUrl, bytes);
                    responseText = System.Text.Encoding.UTF8.GetString(responseBytes);
                    return true;
                }
                catch (WebException ex)
                {
                    Stream responseStream = ex.Response.GetResponseStream();
                    responseBytes = new byte[ex.Response.ContentLength];
                    responseStream.Read(responseBytes, 0, responseBytes.Length);
                }
                responseText = System.Text.Encoding.UTF8.GetString(responseBytes);
                return false;
            }

            /// <summary>
            /// 设置表单数据字段
            /// </summary>
            /// <param name="fieldName">字段名</param>
            /// <param name="fieldValue">字段值</param>
            /// <returns></returns>
            public void SetFieldValue(String fieldName, String fieldValue)
            {
                string httpRow = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                string httpRowData = String.Format(httpRow, fieldName, fieldValue);

                bytesArray.Add(encoding.GetBytes(httpRowData));
            }

            /// <summary>
            /// 设置表单文件数据
            /// </summary>
            /// <param name="fieldName">字段名</param>
            /// <param name="filename">字段值</param>
            /// <param name="contentType">内容内型</param>
            /// <param name="fileBytes">文件字节流</param>
            /// <returns></returns>
            public void SetFieldValue(String fieldName, String filename, String contentType, Byte[] fileBytes)
            {
                string end = "\r\n";
                string httpRow = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string httpRowData = String.Format(httpRow, fieldName, filename, contentType);

                byte[] headerBytes = encoding.GetBytes(httpRowData);
                byte[] endBytes = encoding.GetBytes(end);
                byte[] fileDataBytes = new byte[headerBytes.Length + fileBytes.Length + endBytes.Length];

                headerBytes.CopyTo(fileDataBytes, 0);
                fileBytes.CopyTo(fileDataBytes, headerBytes.Length);
                endBytes.CopyTo(fileDataBytes, headerBytes.Length + fileBytes.Length);

                bytesArray.Add(fileDataBytes);
            }
            #endregion
        }
    }
}

