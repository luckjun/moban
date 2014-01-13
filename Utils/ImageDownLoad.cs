using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Moban.Utils
{
    public static class ImageDownLoad
    {
        public static void DownLoadImg(string url, string subFileFolder)
        {
            Uri uri = new Uri(url);
            string ImgName = uri.Segments[uri.Segments.Length - 1].ToString();
            string fileFolder = subFileFolder + uri.Host + "\\" + uri.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");
            if (!Directory.Exists(fileFolder))
                Directory.CreateDirectory(fileFolder);
            string imgFile = fileFolder + ImgName;
            WebClient Wc = new WebClient();
            try
            {
                if (!File.Exists(imgFile))
                {
                    if (!Directory.Exists(fileFolder))
                        Directory.CreateDirectory(fileFolder);
                    Wc.DownloadFile(url, imgFile);
                    Console.WriteLine(DateTime.Now + imgFile);
                    //System.Threading.Thread.Sleep(1*1000);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
