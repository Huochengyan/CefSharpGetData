using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace dpreview.Handler
{
    public class AppendResponseFilter : IResponseFilter
    {
        private static Encoding encoding = Encoding.UTF8;



        public event Action<string, string, string, long> VOIDFUN;
        private string _url;
        private string _type;
        public AppendResponseFilter(string url, string type)
        {
            _url = url;
            _type = type;

        }
        bool IResponseFilter.InitFilter()
        {
            return true;
        }
        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                return FilterStatus.Done;
            }
            dataInRead = dataIn.Length;
            dataOutWritten = Math.Min(dataInRead, dataOut.Length);

            byte[] buffer = new byte[dataOutWritten];
            int bytesRead = dataIn.Read(buffer, 0, (int)dataOutWritten);

   
            var s = System.Text.Encoding.UTF8.GetString(buffer);
            VOIDFUN?.BeginInvoke(s, _url, _type, dataInRead, null, null);
            dataOut.Write(buffer, 0, bytesRead);
            return FilterStatus.Done;
        }

        public void Dispose()
        {

        }

        #region
        public event Action<byte[]> NotifyData;
        private int contentLength = 0;
        public List<byte> dataAll = new List<byte>();

        public void SetContentLength(int contentLength)
        {
            this.contentLength = contentLength;
        }
        #endregion
    }



    public class FilterManager
    {
        private static Dictionary<string, IResponseFilter> dataList = new Dictionary<string, IResponseFilter>();

        public static IResponseFilter CreateFilter(string guid)
        {
            lock (dataList)
            {
                var filter = new AppendResponseFilter("","");
                dataList.Add(guid, filter);

                return filter;
            }
        }

        public static IResponseFilter GetFileter(string guid)
        {
            lock (dataList)
            {
                return dataList[guid];
            }
        }
    }
}

