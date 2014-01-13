using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moban.DAL;
using System.Net;
using Moban.Utils;
using System.Text.RegularExpressions;
using Moban.Model;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;

namespace APP
{
    class Program
    {
        static string[] EXTS = { "jpg", "jpeg", "gif", "png" };

        static long avhash(string im)
        {
            return avhash(Image.FromFile(im));
        }

        static long avhash(Image im)
        {
            // resize((8, 8), Image.ANTIALIAS)
            Bitmap bmp = new Bitmap(8, 8);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(im, 0, 0, 8, 8);
            }
            // convert('L')
            var data = new long[64];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    var l = c.R * 299 / 1000 + c.G * 587 / 1000 + c.B * 114 / 1000;
                    data[y * 8 + x] = l;
                }
            }
            var avg = data.Sum() / 64;
            return data.Select((i, y) => new { y, z = i < avg ? 0 : 1 }).Aggregate(0L, (x, a) => x | ((long)a.z << a.y));
        }

        static long hamming(long h1, long h2)
        {
            long h = 0, d = h1 ^ h2;
            while (d > 0)
            {
                h += 1;
                d &= d - 1;
            }
            return h;
        }

        static void Main(string[] args)
        {

            if (args.Length == 0 || args.Length > 2)
            {
                string arg0 = Process.GetCurrentProcess().ProcessName;
                Console.WriteLine("Usage: {0} image.jpg [dir]", arg0);
            }
            else
            {
                string im = args[0];
                string wd = args.Length < 2 ? "." : args[1];
                long h = avhash(im);

                var images = new List<string>();
                foreach (var ext in EXTS)
                    images.AddRange(Directory.GetFiles(wd, "*." + ext));

                var seq = new List<object[]>();
                int prog = images.Count > 50 ? 1 : 0;
                foreach (var f in images)
                {
                    seq.Add(new object[] { f, hamming(avhash(f), h) });
                    if (prog > 0)
                    {
                        var perc = 100.0 * prog / images.Count;
                        var x = (int)(2 * perc / 5);

                        Console.Write("\rCalculating... [" + new String('#', x) + new String(' ', 40 - x) + "]");
                        Console.Write("{0:#.00}% ({1}/{2})", perc, prog, images.Count);
                        Console.Out.Flush();
                        prog += 1;
                    }
                }

                if (prog > 0)
                    Console.WriteLine();

                foreach (var a in seq.OrderBy(i => i[1]))
                    Console.WriteLine("{1}\t{0}", a[0], a[1]);
            }
           // Console.Read();
            //Get365PsdUrl();
            //Console.WriteLine(a);


            //var data = new long[64];
            //for (int y = 0; y < bmp.Height; y++)
            //{
            //    for (int x = 0; x < bmp.Width; x++)
            //    {
            //        Color c = bmp.GetPixel(x, y);
            //        var l = c.R * 299 / 1000 + c.G * 587 / 1000 + c.B * 114 / 1000;
            //        data[y * 8 + x] = l;
            //    }
            //}
            //var avg = data.Sum() / 64;
            //return data.Select((i, y) => new { y, z = i < avg ? 0 : 1 }).Aggregate(0L, (x, a) => x | ((long)a.z << a.y));

            Console.Read();
        }


        #region 365psd

        public static void Get365PsdUrl()
        {

            string page = "http://365psd.com/page/";

            string pageBody = string.Empty;
            string pattern = "<div id=\"post-(.*?)</div><!--";
            string linkPattern = "href=\"(.*?)\"";
            PSDDAL dal = new PSDDAL();
            for (int j = 2; j < 3; j++)
            {
                try
                {
                    CookieCollection cookieCollection = new CookieCollection();
                    Console.WriteLine(DateTime.Now + "内容：" + page + j);

                    pageBody = HttpHelper.HttpGET(page + j + "/", "http://365psd.com/", 2, ref cookieCollection, 30, "", false);
                    Console.WriteLine(DateTime.Now + "内容获取完成");
                    if (!string.IsNullOrEmpty(pageBody))
                    {
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        MatchCollection mc = rx.Matches(pageBody);

                        //获取列表item
                        foreach (Match m in mc)
                        {


                            string b = m.ToString();
                            PSDInfo psdinfo = new PSDInfo();

                            psdinfo.CreateTime = DateTime.Now;

                            Regex rxlink = new Regex(linkPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(m.ToString());
                            foreach (Match ml in mclink)
                            {
                                psdinfo.Url = ml.Groups[1].ToString();
                                Console.WriteLine("网址:" + psdinfo.Url);
                            }
                            if (string.IsNullOrEmpty(psdinfo.Url))
                            {
                                continue;
                            }
                            Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcpic = rxpic.Matches(m.ToString());
                            foreach (Match ml in mcpic)
                            {
                                psdinfo.PicUrl = ml.Groups[1].ToString();
                                Console.WriteLine("缩略图:" + psdinfo.PicUrl);
                                DownLoadImg(psdinfo.PicUrl);
                                break;
                            }

                            Regex rxt = new Regex("<h2>(.*?)</h2>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mct = rxt.Matches(m.ToString());
                            foreach (Match ml in mct)
                            {
                                psdinfo.Title = ml.Groups[1].ToString();
                                Console.WriteLine("标题:" + psdinfo.Title);
                                
                                break;
                            }

                            if (!dal.Exists(psdinfo.Url))
                            {
                                if (dal.Add(psdinfo) == 0)
                                {
                                    Console.WriteLine("保存失败");
                                }
                                else
                                {
                                    Console.WriteLine("保存成功");
                                }
                            }
                            else
                            {
                                Console.WriteLine("已经存在");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                System.Threading.Thread.Sleep(10 * 1000);

            }
        }

        public static void GetPsdBody()
        {
            bool IsStop = true;
            while (IsStop)
            {
                PSDDAL dal = new PSDDAL();
                DataSet ds = dal.GetNeedDownBodyList();
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count == 0)
                {
                    IsStop = false;
                }

                Console.WriteLine(DateTime.Now + "...............抓取循环开始.............");
               
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PSDInfo model = new PSDInfo();
                    model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                    model.Url = dt.Rows[i]["url"].ToString();

                    if (string.IsNullOrEmpty(model.Url))
                    {
                        continue;
                    }
                    HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                    Console.WriteLine("内容：" + model.Url);
                    model.Body = HttpHelper.HttpGET(model.Url, "http://365psd.com/", 2, ref statusCode, 30, "", false);
                    Console.WriteLine(DateTime.Now + "内容获取完成");
                    if (statusCode != HttpStatusCode.OK)
                    {
                        continue;
                    }

                    Regex rxlink = new Regex("<div class=\"img\">(?<leftcol>(.*?))</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection mclink = rxlink.Matches(model.Body);
                    foreach (Match ml in mclink)
                    {
                        model.Info = ml.Groups["leftcol"].ToString();

                        Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        MatchCollection mcpic = rxpic.Matches(ml.ToString());
                        foreach (Match mm in mcpic)
                        {
                            DownLoadImg(mm.Groups[1].ToString());
                        }
                    }




                    Regex rxright = new Regex("<a href=\"http://365psd.com/wp-content/uploads/(?<rightcol>(.*?))\" class=\"download", RegexOptions.IgnoreCase);
                    MatchCollection mcrg = rxright.Matches(model.Body);
                    foreach (Match ml in mcrg)
                    {
                        model.DownUrl = "http://365psd.com/wp-content/uploads/" + ml.Groups["rightcol"].ToString();
                        // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                    }



                    try
                    {
                        Regex regexObj = new Regex("\" rel=\"tag\">(.*?)</a>");
                        Match matchResult = regexObj.Match(model.Body);
                        while (matchResult.Success)
                        {
                            model.Tag += (matchResult.Groups[1].Value) + ",";
                            matchResult = matchResult.NextMatch();
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        // Syntax error in the regular expression
                    }

                    model.Status = 1;
                     
                    if (dal.DownLoadPage(model))
                    {
                        Console.WriteLine("ID:" + model.Id + "保存成功");
                    }
                    else
                    {
                        Console.WriteLine("保存失败");
                    }
                    System.Threading.Thread.Sleep(3 * 1000);
                }
            }
        }

        #endregion





        public static void DownLoadImg(string picUrl)
        {
            string subFileFolder = "E:\\Moban\\Images\\";
            Uri uri = new Uri(picUrl);
            string ImgName = uri.Segments[uri.Segments.Length - 1].ToString();
            string fileFolder = subFileFolder + uri.Host + "\\" + uri.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");

            string imgFile = fileFolder + ImgName;
            WebClient Wc = new WebClient();
            try
            {
                if (!File.Exists(imgFile))
                {
                    if (!Directory.Exists(fileFolder))
                        Directory.CreateDirectory(fileFolder);
                    Wc.DownloadFile(picUrl, imgFile);
                    Console.WriteLine(DateTime.Now + imgFile);
                    //System.Threading.Thread.Sleep(1*1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + ex.Message);
            }
        }
    
    
    }
}
