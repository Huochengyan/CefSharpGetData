using System.Diagnostics;
using System.IO;

namespace Browserform.common
{
    /// <summary>
    /// 文件流压缩解压
    /// </summary>
    public class ZipHelper
    {
        //public static byte[] Compress(Stream Source)
        //{
            //Debug.Assert(null != Source);
            //Source.Seek(0, SeekOrigin.Begin);
            //MemoryStream objMem = new MemoryStream();
            //GZipOutputStream objGzip = new GZipOutputStream(objMem);

            //const int BUFFER_SIZE = 1024 * 10;
            //byte[] arrBuffer = new byte[BUFFER_SIZE];
            //int nGetedCount = 0;
            //do
            //{
            //    nGetedCount = Source.Read(arrBuffer, 0, BUFFER_SIZE);
            //    objGzip.Write(arrBuffer, 0, nGetedCount);
            //} while (nGetedCount > 0);
            //objGzip.Finish();
            //byte[] arrResult = objMem.ToArray();
            //objGzip.Close();  //压缩完成后，输出流就会被关闭
            //objGzip = null;
            //objMem.Close();
            //objMem = null;
            //return arrResult;
        //}
    }
}
