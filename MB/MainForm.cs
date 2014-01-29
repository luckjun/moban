using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Moban.Utils;
using System.Net;
using System.Text.RegularExpressions;
using Moban.Model;
using System.IO;
using HtmlAgilityPack;
using System.Collections;
using Moban.DAL;
using QConnectSDK.Context;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MB
{
    public partial class MainForm : Form
    {
        public static RichTextBox richTextBoxC;
        public static RichTextBox richTextBoxLog;
        private Thread[] threadC;

        public MainForm()
        {
            InitializeComponent();
            threadC = new Thread[0];

            richTextBoxC = rtb_C;
            richTextBoxLog = rtb_log;
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

        }

        private delegate void AddMessageDelegate(string message, RichTextBox richTextBox);

        public void AddMessage(string message, RichTextBox richTextBox)
        {
            if (richTextBox.InvokeRequired)
            {
                AddMessageDelegate d = AddMessage;
                richTextBox.Invoke(d, message, richTextBox);
            }
            else
            {
                richTextBox.AppendText(message + "\r");
            }
        }

        /// <summary>
        /// 抓取站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.Templatemonster);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }
        /// <summary>
        /// 获取全部文章内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            
            try
            {
                DownLoad download = new DownLoad(this, "E:\\Moban\\Images\\");
                Thread t = new Thread(download.DownLoadImg);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();


                //if (!Directory.Exists(fileFolder))
                //    Directory.CreateDirectory(fileFolder);
                //Uri uri = new Uri("http://www.ff.c/dd/a.jpg");
                //int a = uri.Segments.Length;
                //string file = uri.Segments[a - 1].ToString();

                //ImageDownLoad.DownLoadImg("http://mat1.gtimg.com/www/mb/images/wide/logo_20120326.png", "E:\\Moban\\Images\\");


                //CookieCollection cookieCollection = new CookieCollection();
                //string page = " http://www.elemisfreebies.com/wp-content/plugins/download-monitor/download.php?id=176";
                //string pageBody = HttpHelper.HttpGET(page, "http://www.templatemonster.com/", 2, ref cookieCollection, 30, "", false);
                //rtb_C.Text = pageBody;
            }
            catch (Exception ex)
            {

                rtb_C.AppendText(ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.DownLoadPage);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient wc = new WebClient();

                wc.DownloadFile("http://mat1.gtimg.com/www/mb/images/wide/logo_2012032611.png", Environment.CurrentDirectory+"/a.png");
                return;
                string url = "http://www.a.com";

                GetSnap thumb = new GetSnap(url);
                System.Drawing.Bitmap x = thumb.GetBitmap();//获取截图
                string FileName = DateTime.Now.ToString("yyyyMMddhhmmss");

                x.Save(Environment.CurrentDirectory + "/SnapPic/" + FileName + ".jpg");//保存截图到SnapPic目录下

            }
            catch (Exception ex)
            {
                rtb_C.Text = ex.Message;
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.GetDemoUrl);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeContent analyzeContent = new AnalyzeContent(this);

                Thread t = new Thread(analyzeContent.AnalyzeTemplatemonster);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CookieCollection cookie = new CookieCollection();
            string pageBody = HttpHelper.HttpGET("http://themeforest.net/theme_previews/5471574-responsive-opencart-theme-boss-electronues", "http://themeforest.net/", 2, ref cookie, 30, "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)", false);

            rtb_C.AppendText(pageBody);
        }

        private void button8_Click(object sender, EventArgs e)
        {

            try
            {
                //DownWebSite downWebSite = new DownWebSite(this, rtb_C.Text);

                //Thread t = new Thread(downWebSite.DownLoadIE);
                //t.IsBackground = true;
                //t.Start();
                //rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }

        }

        /// <summary>
        /// 根据文件夹内容自动抓取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                DownWebSite downWebSite = new DownWebSite(this,txtSite.Text, rtb_C.Text);

                Thread t = new Thread(downWebSite.AutoGetFiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        /// <summary>
        /// 根据首页html获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                DownWebSite downWebSite = new DownWebSite(this, rtb_C.Text);

                Thread t = new Thread(downWebSite.GetWebSiteByHtml);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.Themeforest);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.GetThemeforestDemoUrl);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }

        }


        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeContent analyzeContent = new AnalyzeContent(this);

                Thread t = new Thread(analyzeContent.AnalyzeThemeforest);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        /// <summary>
        /// 预览图集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.DownLoadScreenshotsPage);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }
        
        /// <summary>
        /// 补全demo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.GetThemeforestDemo);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }


        private void button16_Click(object sender, EventArgs e)
        {
            try
            {

                DownThemeforestWebSite downWebSite = new DownThemeforestWebSite(this, rtb_C.Text);
                Thread t = new Thread(downWebSite.GetWebSiteByHtml);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {

                DownThemeforestWebSite downWebSite = new DownThemeforestWebSite(this, rtb_C.Text);
                Thread t = new Thread(downWebSite.AutoGetFiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }


        /// <summary>
        /// 只用到下载图片
        /// </summary>
        public class DownLoad
        {
            public volatile bool shouldStop;
            private MainForm form;
            private string subFileFolder;

            //SqlHelp sqlHelp = new SqlHelp();

            public DownLoad(MainForm frm, string sFileFolder)
            {
                this.form = frm;
                this.subFileFolder = sFileFolder;
            }

            /// <summary>
            /// 下载mbpic表图片
            /// </summary>
            public void DownLoadImg()
            {
                bool IsStop = true;
                while (IsStop)
                {
                    try
                    {
                        MBPicDAL bll = new MBPicDAL();
                        DataSet ds = bll.GetNeedDownImageList();
                        System.Data.DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count == 0)
                        {
                            form.AddMessage(DateTime.Now + "...............全部完成.............", MainForm.richTextBoxC);
                            IsStop = false;
                            return;
                        }
                        form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                        string picUrl = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            picUrl = dt.Rows[i]["picurl"].ToString();
                            if (string.IsNullOrEmpty(picUrl))
                            {
                                continue;
                            }

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
                                    form.AddMessage(DateTime.Now + imgFile, MainForm.richTextBoxC);
                                    //System.Threading.Thread.Sleep(1*1000);
                                }
                                MBPicInfo model = new MBPicInfo();
                                model.PicUrl = picUrl;
                                model.LocalFile = imgFile;
                                model.IsDownLoad = 1;
                                model.CrawlTime = DateTime.Now;
                                bll.DownLoadImage(model);

                            }
                            catch (Exception ex)
                            {
                                MBPicInfo model = new MBPicInfo();
                                model.PicUrl = picUrl;

                                model.IsDownLoad = 2;
                                model.CrawlTime = DateTime.Now;
                                bll.DownLoadImage(model);

                                form.AddMessage(picUrl + ex.Message, MainForm.richTextBoxLog);
                                continue;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                    }
                }
            }
        }


        public class AnalyzeContent 
        {
            private MainForm form;

            public AnalyzeContent(MainForm frm)
            {
                this.form = frm;
            }

            public void AnalyzeTemplatemonster()
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    DataSet ds = bll.GetNeedAnalyzeList("templatemonster.com");
                    System.Data.DataTable dt = ds.Tables[0];
                    form.AddMessage(DateTime.Now + "....抓取循环开始...", MainForm.richTextBoxC);
                     
                   // string pageBody = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //System.Threading.Thread.Sleep(1*1000);
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            model.Body = dt.Rows[i]["body"].ToString();
                            model.IsAnalyze = 2;
                            if (string.IsNullOrEmpty(model.Body))
                            {
                                continue;
                            }

                            Regex rxlink = new Regex("<td class=\"leftcol\">(?<lef>(.*?))<div class=\"clear\">(?<leftcol>(.*?))</td>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(model.Body);
                            foreach (Match ml in mclink)
                            {
                                model.LeftCol = ml.Groups["leftcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }
                            Regex rxright = new Regex("<span class=\"text\">(?<rightcol>(.*?))<br/><br/>  </span>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcrg = rxright.Matches(model.Body);
                            foreach (Match ml in mcrg)
                            {
                                model.RightCol = ml.Groups["rightcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }

                             
                            if (!string.IsNullOrEmpty(model.LeftCol))
                            {
                                Regex rxpic = new Regex("src='(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcpic = rxpic.Matches(model.LeftCol);
                                foreach (Match ml in mcpic)
                                {
                                    model.BPics = ml.Groups[1].ToString();
                                    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                    break;
                                }
                                foreach (Match ml in mcpic)
                                {
                                    string picUrl = ml.Groups[1].ToString();
                                    Uri uri = new Uri(picUrl);
                                    string ImgName = uri.Segments[uri.Segments.Length - 1].ToString();
                                    string fileFolder = "E:\\Moban\\Images\\" + uri.Host + "\\" + uri.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");

                                    string imgFile = fileFolder + ImgName;
                                    WebClient Wc = new WebClient();
                                    try
                                    {
                                        if (!File.Exists(imgFile))
                                        {
                                            if (!Directory.Exists(fileFolder))
                                                Directory.CreateDirectory(fileFolder);
                                            Wc.DownloadFile(picUrl, imgFile);
                                            form.AddMessage(DateTime.Now + imgFile, MainForm.richTextBoxC);
                                            //System.Threading.Thread.Sleep(1*1000);
                                        }
                                        


                                    }
                                    catch (Exception ex)
                                    {
                                        form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                                        continue;
                                    }
                                }

                                model.IsAnalyze = 1;
                            }
                             

                            //model.DemoUrl = DemoUrl;
                            //model.CrawlTime = DateTime.Now;
                            //model.IsDownLoad = 1;
                            if (new MobanDAL().UpdateAnalyzeContent(model))
                            {
                                form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                            }
                            else
                            {
                                form.AddMessage("保存失败", MainForm.richTextBoxC);
                            }

                        }
                        catch (Exception ex)
                        {

                            form.AddMessage(ex.Message, MainForm.richTextBoxC);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                }
            }

            public void AnalyzeTemplatemonsterType()
            {
                while (true)
                {
                    try
                    {
                        MobanDAL bll = new MobanDAL();
                        DataSet ds = bll.GetNeedAnalyzeTypeList();
                        System.Data.DataTable dt = ds.Tables[0];
                        form.AddMessage(DateTime.Now + "....抓取循环开始...", MainForm.richTextBoxC);

                        // string pageBody = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //System.Threading.Thread.Sleep(1*1000);
                            try
                            {
                                MobanInfo model = new MobanInfo();
                                model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                                model.Body = dt.Rows[i]["body"].ToString();
                                model.IsAnalyze = 2;
                                if (string.IsNullOrEmpty(model.Body))
                                {
                                    continue;
                                }

                                Regex rxlink = new Regex("<ul class=\"breadcrumbs\" itemprop=\"breadcrumb\">(?<leftcol>(.*?))</ul>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mclink = rxlink.Matches(model.Body);
                                foreach (Match ml in mclink)
                                {
                                    model.LeftCol = ml.Groups["leftcol"].ToString();

                                    // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                                }
                                Regex rxright = new Regex("<span class=\"text\">(?<rightcol>(.*?))<br/><br/>  </span>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcrg = rxright.Matches(model.Body);
                                foreach (Match ml in mcrg)
                                {
                                    model.RightCol = ml.Groups["rightcol"].ToString();
                                    // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                                }


                                if (!string.IsNullOrEmpty(model.LeftCol))
                                {
                                    Regex rxpic = new Regex("\">(.*?)<", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                    MatchCollection mcpic = rxpic.Matches(model.LeftCol);

                                    if (mcpic.Count == 5)
                                    {
                                        model.Type1 = mcpic[1].Groups[1].ToString();
                                        model.Type2 = mcpic[2].Groups[1].ToString();
                                        model.Type3 = mcpic[3].Groups[1].ToString();
                                        model.Type4 = mcpic[4].Groups[1].ToString();
                                    }
                                }

                                Regex rxname = new Regex("<h1 id=\"previewProductName\">(.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcname = rxname.Matches(model.Body);
                                if (mcname.Count > 0)
                                {
                                    model.Title = mcname[0].Groups[1].ToString();
                                }

                                try
                                {
                                    model.Name = Regex.Match(model.RightCol, "<span class=\"fontNormal\" itemprop=\"name\">(.*?)</span>").Groups[1].Value;
                                }
                                catch (ArgumentException ex)
                                {
                                    // Syntax error in the regular expression
                                }

                                try
                                {
                                    Regex regexObj = new Regex("class='bluelink' target='_blank'>(.*?)</a>");
                                    Match matchResult = regexObj.Match(model.RightCol);
                                    while (matchResult.Success)
                                    {
                                        model.Relatedtype += (matchResult.Groups[1].Value) + ",";
                                        matchResult = matchResult.NextMatch();
                                    }
                                }
                                catch (ArgumentException ex)
                                {
                                    // Syntax error in the regular expression
                                }



                                model.IsAnalyze = model.IsAnalyze + 3;

                                if (new MobanDAL().UpdateAnalyzeType(model))
                                {
                                    form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                                }
                                else
                                {
                                    form.AddMessage("保存失败", MainForm.richTextBoxC);
                                }

                            }
                            catch (Exception ex)
                            {

                                form.AddMessage(ex.Message, MainForm.richTextBoxC);
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                    }
                }
            }

            public void AnalyzeThemeforest()
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    MBPicDAL picdal = new MBPicDAL();
                    DataSet ds = bll.GetNeedAnalyzeList("Themeforest.net");
                    System.Data.DataTable dt = ds.Tables[0];
                    form.AddMessage(DateTime.Now + "....抓取循环开始...", MainForm.richTextBoxC);
                    Hashtable imgHash = new Hashtable();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            model.Body = dt.Rows[i]["body"].ToString();
                            model.IsAnalyze = 2;
                            if (string.IsNullOrEmpty(model.Body))
                            {
                                continue;
                            }    

                            Regex rximglink = new Regex("itemprop=\"image\" src=\"(?<mpic>(.*?))\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcimglink = rximglink.Matches(model.Body);
                            foreach (Match ml in mcimglink)
                            {
                                model.BPic = ml.Groups["mpic"].ToString();
                                try
                                {
                                    imgHash.Add(model.BPic, "");
                                }
                                catch (Exception)
                                {
                                    continue;

                                }
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }


                            Regex rxprelink = new Regex("<a href=\"/theme_previews/(?<Screenshots>(.*?))\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcprelink = rxprelink.Matches(model.Body);
                            foreach (Match ml in mcprelink)
                            {
                                model.Screenshots = "http://Themeforest.net/theme_previews/" + ml.Groups["Screenshots"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }
                            

                            Regex rxlink = new Regex("itemprop=\"description\">(?<leftcol>(.*?))<div class=\"more-work\">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(model.Body);
                            foreach (Match ml in mclink)
                            {
                                model.LeftCol = ml.Groups["leftcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }
                            Regex rxright = new Regex("id=\"item_attributes\">(?<rightcol>(.*?))</table>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcrg = rxright.Matches(model.Body);
                            foreach (Match ml in mcrg)
                            {
                                model.RightCol = ml.Groups["rightcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }


                            if (!string.IsNullOrEmpty(model.LeftCol))
                            {
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcpic = rxpic.Matches(model.LeftCol);
                                //foreach (Match ml in mcpic)
                                //{
                                //    model.BPics = ml.Groups[1].ToString();
                                //    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                //    break;
                                //}
                                foreach (Match ml in mcpic)
                                {
                                    try
                                    {
                                        string picUrl = ml.Groups[1].ToString();
                                        imgHash.Add(model.MPic, "");
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }

                                model.IsAnalyze = 1;
                            }
                            if (new MobanDAL().UpdateAnalyzeContent(model))
                            {
                                form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                            }
                            else
                            {
                                form.AddMessage("保存失败", MainForm.richTextBoxC);
                            }

                            foreach (DictionaryEntry item in imgHash)
                            {
                                string url = item.Key.ToString();
                                if (!url.StartsWith("http"))
                                {
                                    continue;
                                }
                                if (!picdal.Exists(url))
                                {
                                    picdal.Add(url);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            form.AddMessage(ex.Message, MainForm.richTextBoxC);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                }
            }

            /// <summary>
            /// 分析elemisfreebies.com页面内容
            /// </summary>
            public void AnalyzeElemisfreebies()
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    MBPicDAL picdal = new MBPicDAL();
                    DataSet ds = bll.GetNeedAnalyzeList("elemisfreebies.com");
                    System.Data.DataTable dt = ds.Tables[0];
                    form.AddMessage(DateTime.Now + "....抓取循环开始...", MainForm.richTextBoxC);
                    Hashtable imgHash = new Hashtable();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            model.Body = dt.Rows[i]["body"].ToString();
                            model.IsAnalyze = 2;
                            if (string.IsNullOrEmpty(model.Body))
                            {
                                continue;
                            }

                            Regex rximglink = new Regex("<h1 class=\"title\">(.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcimglink = rximglink.Matches(model.Body);
                            foreach (Match ml in mcimglink)
                            {
                                model.Title = ml.Groups[1].ToString();
                            }

                            Regex rxprelink = new Regex("<a href=\"(.*?)\" class=\"button\" style=\"margin-top:10px;\">Download File<span></span></a>", RegexOptions.IgnoreCase );
                            MatchCollection mcprelink = rxprelink.Matches(model.Body);
                            foreach (Match ml in mcprelink)
                            {
                                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                                Console.WriteLine("内容：" + model.Url);
                                string durl = ml.Groups[1].ToString().Replace("http://elemisfreebies.com/downloads/#", "http://elemisfreebies.com/wp-content/plugins/download-monitor/download.php?id=");
                                model.DownLoadUrl = HttpHelper.HttpGET302(durl, "http://elemisfreebies.com/", 2, ref statusCode, 30, "", true);

                            }
                            Regex rxid = new Regex("<a class=\"button3 yellow\" href=\"(.*?)\" target=\"_blank\">View HTML Demo", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcrxid = rxid.Matches(model.Body);
                            foreach (Match ml in mcrxid)
                            {
                                model.Demo =  ml.Groups[1].Value.Trim();
                                form.AddMessage("Demo:" + model.Demo, MainForm.richTextBoxC);
                                break;
                            }

                            Regex rxlink = new Regex("<div class=\"secondary\">(?<leftcol>(.*?))<div class=\"related outer\">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(model.Body);
                            foreach (Match ml in mclink)
                            {
                                model.LeftCol = ml.Groups["leftcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }
                            Regex rxright = new Regex("<div class=\"text\">(?<rightcol>(.*?))</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcrg = rxright.Matches(model.Body);
                            foreach (Match ml in mcrg)
                            {
                                model.RightCol = ml.Groups["rightcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }


                            if (!string.IsNullOrEmpty(model.LeftCol))
                            {
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcpic = rxpic.Matches(model.LeftCol);
                                //foreach (Match ml in mcpic)
                                //{
                                //    model.BPics = ml.Groups[1].ToString();
                                //    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                //    break;
                                //}
                                foreach (Match ml in mcpic)
                                {
                                    try
                                    {
                                        string picUrl = ml.Groups[1].ToString();
                                        imgHash.Add(model.MPic, "");
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }

                                model.IsAnalyze = 1;
                            }
                            if (new MobanDAL().UpdateAnalyzeElemisfreebiesContent(model))
                            {
                                form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                            }
                            else
                            {
                                form.AddMessage("保存失败", MainForm.richTextBoxC);
                            }

                            foreach (DictionaryEntry item in imgHash)
                            {
                                string url = item.Key.ToString();
                                if (!url.StartsWith("http"))
                                {
                                    continue;
                                }
                                if (!picdal.Exists(url))
                                {
                                    picdal.Add(url);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            form.AddMessage(ex.Message, MainForm.richTextBoxC);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                }
            }


            public void AnalyzeFreepsdfiles()
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    MBPicDAL picdal = new MBPicDAL();
                    DataSet ds = bll.GetNeedAnalyzeList("freepsdfiles.net");
                    System.Data.DataTable dt = ds.Tables[0];
                    form.AddMessage(DateTime.Now + "....抓取循环开始...", MainForm.richTextBoxC);
                    Hashtable imgHash = new Hashtable();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            model.Body = dt.Rows[i]["body"].ToString();
                            model.IsAnalyze = 2;
                            if (string.IsNullOrEmpty(model.Body))
                            {
                                continue;
                            }

                            Regex rximglink = new Regex("<h2 class=\"pagetitle\">(.*?)</h2>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcimglink = rximglink.Matches(model.Body);
                            foreach (Match ml in mcimglink)
                            {
                                model.Title = ml.Groups[1].ToString();
                            }

                            Regex rxprelink = new Regex("href=\"http://freepsdfiles\\.net/download/\\?id=(.*?)\"", RegexOptions.IgnoreCase);
                            MatchCollection mcprelink = rxprelink.Matches(model.Body);
                            foreach (Match ml in mcprelink)
                            {
                                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                                Console.WriteLine("内容：" + model.Url);
                                string durl = "http://freepsdfiles.net/get-file/?id=" + ml.Groups[1].ToString();
                                model.DownLoadUrl = HttpHelper.HttpGET302(durl, "http://freepsdfiles.net/", 2, ref statusCode, 30, "", true);

                            }
                            //Regex rxid = new Regex("<a class=\"button3 yellow\" href=\"(.*?)\" target=\"_blank\">View HTML Demo", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            //MatchCollection mcrxid = rxid.Matches(model.Body);
                            //foreach (Match ml in mcrxid)
                            //{
                            //    model.Demo = ml.Groups[1].Value.Trim();
                            //    form.AddMessage("Demo:" + model.Demo, MainForm.richTextBoxC);
                            //    break;
                            //}

                            Regex rxlink = new Regex("<div class=\"entry\">(?<leftcol>(.*?))<div class=\"buttons \">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(model.Body);
                            foreach (Match ml in mclink)
                            {
                                model.LeftCol = ml.Groups["leftcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }
                            Regex rxright = new Regex("<div class=\"entry-meta\">(?<rightcol>(.*?))</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcrg = rxright.Matches(model.Body);
                            foreach (Match ml in mcrg)
                            {
                                model.RightCol = ml.Groups["rightcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }


                            if (!string.IsNullOrEmpty(model.LeftCol))
                            {
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcpic = rxpic.Matches(model.LeftCol);
                                //foreach (Match ml in mcpic)
                                //{
                                //    model.BPics = ml.Groups[1].ToString();
                                //    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                //    break;
                                //}
                                foreach (Match ml in mcpic)
                                {
                                    try
                                    {
                                        string picUrl = ml.Groups[1].ToString();
                                        imgHash.Add(model.MPic, "");
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }

                                model.IsAnalyze = 1;
                            }
                            if (new MobanDAL().UpdateAnalyzeElemisfreebiesContent(model))
                            {
                                form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                            }
                            else
                            {
                                form.AddMessage("保存失败", MainForm.richTextBoxC);
                            }

                            foreach (DictionaryEntry item in imgHash)
                            {
                                string url = item.Key.ToString();
                                if (!url.StartsWith("http"))
                                {
                                    continue;
                                }
                                if (!picdal.Exists(url))
                                {
                                    picdal.Add(url);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            form.AddMessage(ex.Message, MainForm.richTextBoxC);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                }
            }


            public void AnalyzeFreecssfiles()
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    MBPicDAL picdal = new MBPicDAL();
                    DataSet ds = bll.GetNeedAnalyzeList("css-free-templates.com");
                    System.Data.DataTable dt = ds.Tables[0];
                    form.AddMessage(DateTime.Now + "....抓取循环开始...", MainForm.richTextBoxC);
                    Hashtable imgHash = new Hashtable();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            model.Body = dt.Rows[i]["body"].ToString();
                            model.IsAnalyze = 2;
                            if (string.IsNullOrEmpty(model.Body))
                            {
                                continue;
                            }

                            Regex rximglink = new Regex("<h2 class=\"pagetitle\">(.*?)</h2>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcimglink = rximglink.Matches(model.Body);
                            foreach (Match ml in mcimglink)
                            {
                                model.Title = ml.Groups[1].ToString();
                            }

                            Regex rxprelink = new Regex("href=\"http://css-free-templates\\.com/download/\\?id=(.*?)\"", RegexOptions.IgnoreCase);
                            MatchCollection mcprelink = rxprelink.Matches(model.Body);
                            foreach (Match ml in mcprelink)
                            {
                                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                                Console.WriteLine("内容：" + model.Url);
                                string durl = "http://css-free-templates.com/get-file/?id=" + ml.Groups[1].ToString();
                                model.DownLoadUrl = HttpHelper.HttpGET302(durl, "http://css-free-templates.com/", 2, ref statusCode, 30, "", true);

                            }
                            //Regex rxid = new Regex("<a class=\"button3 yellow\" href=\"(.*?)\" target=\"_blank\">View HTML Demo", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            //MatchCollection mcrxid = rxid.Matches(model.Body);
                            //foreach (Match ml in mcrxid)
                            //{
                            //    model.Demo = ml.Groups[1].Value.Trim();
                            //    form.AddMessage("Demo:" + model.Demo, MainForm.richTextBoxC);
                            //    break;
                            //}

                            Regex rxlink = new Regex("<div class=\"entry\">(?<leftcol>(.*?))<div class=\"buttons \">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(model.Body);
                            foreach (Match ml in mclink)
                            {
                                model.LeftCol = ml.Groups["leftcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }
                            Regex rxright = new Regex("<div class=\"entry-meta\">(?<rightcol>(.*?))</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcrg = rxright.Matches(model.Body);
                            foreach (Match ml in mcrg)
                            {
                                model.RightCol = ml.Groups["rightcol"].ToString();
                                // form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }


                            if (!string.IsNullOrEmpty(model.LeftCol))
                            {
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcpic = rxpic.Matches(model.LeftCol);
                                //foreach (Match ml in mcpic)
                                //{
                                //    model.BPics = ml.Groups[1].ToString();
                                //    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                //    break;
                                //}
                                foreach (Match ml in mcpic)
                                {
                                    try
                                    {
                                        string picUrl = ml.Groups[1].ToString();
                                        imgHash.Add(model.MPic, "");
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                }

                                model.IsAnalyze = 1;
                            }
                            if (new MobanDAL().UpdateAnalyzeElemisfreebiesContent(model))
                            {
                                form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                            }
                            else
                            {
                                form.AddMessage("保存失败", MainForm.richTextBoxC);
                            }

                            foreach (DictionaryEntry item in imgHash)
                            {
                                string url = item.Key.ToString();
                                if (!url.StartsWith("http"))
                                {
                                    continue;
                                }
                                if (!picdal.Exists(url))
                                {
                                    picdal.Add(url);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            form.AddMessage(ex.Message, MainForm.richTextBoxC);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                }

                form.AddMessage(DateTime.Now + "   完成", MainForm.richTextBoxC);
            }


        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.Elemisfreebies);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeContent analyzeContent = new AnalyzeContent(this);

                Thread t = new Thread(analyzeContent.AnalyzeElemisfreebies);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.Freepsdfiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeContent analyzeContent = new AnalyzeContent(this);

                Thread t = new Thread(analyzeContent.AnalyzeFreepsdfiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.Freecssfiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeContent analyzeContent = new AnalyzeContent(this);

                Thread t = new Thread(analyzeContent.AnalyzeFreecssfiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                DownWebSite downWebSite = new DownWebSite(this, rtb_C.Text);

                Thread t = new Thread(downWebSite.GetWebSiteByID);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                DownWebSite downWebSite = new DownWebSite(this, txtSite.Text, rtb_C.Text);

                Thread t = new Thread(downWebSite.NewAutoGetFiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            GetRequestToken();
        }

       
      
       

        private void GetRequestToken() 
       { 
           var context = new QzoneContext(); 
           string state = Guid.NewGuid().ToString().Replace("-", ""); 
           string scope = "get_user_info,add_share,list_album,upload_pic,check_page_fans,add_t,add_pic_t,del_t,get_repost_list,get_info,get_other_info,get_fanslist,get_idolist,add_idol,del_idol,add_one_blog,add_topic,get_tenpay_addr"; 
           var authenticationUrl = context.GetAuthorizationUrl(state,scope); 
           //request token, request token secret 需要保存起来 
           //在demo演示中，直接保存在全局变量中.真实情况需要网站自己处理 
           //Session["requeststate"] = state;           
           //Response.Redirect(authenticationUrl);
           string s = authenticationUrl;
       }

        private void button27_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.YueMei);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.FirstPPT);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.FivePPT);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            try
            {
                Crawl crawl = new Crawl(this, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                crawl.shouldStop = false;
                Thread t = new Thread(crawl.ChinazPPT);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }





        public void RARsave(string patch, string rarPatch, string rarName)
        {
            String the_rar;
            RegistryKey the_Reg;
            Object the_Obj;
            String the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                //regkey = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRAR.exe\shell\open\command"); 
                //the_Reg = Registry.ClassesRoot.OpenSubKey(@"ApplicationsWinRAR.exeShellOpenCommand");
                the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                Directory.CreateDirectory(patch);
                //命令参数  
                //the_Info = " a    " + rarName + "  " + @"C:Test70821.txt"; //文件压缩  
                the_Info = " a    " + rarName + "  " + patch + "  -r"; ;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //打包文件存放目录  
                the_StartInfo.WorkingDirectory = rarPatch;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string unRAR(string unRarPatch, string rarPatch, string rarName)
        {
            String the_rar;
            RegistryKey the_Reg;
            Object the_Obj;
            String the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                the_Reg = Registry.ClassesRoot.OpenSubKey(@"ApplicationsWinRAR.exeShellOpenCommand");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath+unRarPatch);
                the_Info = "e   " + rarName + "  " + System.Windows.Forms.Application.StartupPath + unRarPatch + " -y";
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_StartInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath + rarPatch;//获取压缩包路径  
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return System.Windows.Forms.Application.StartupPath + unRarPatch;
        }

        private void button31_Click(object sender, EventArgs e)
        {
            string strtxtPath = "D:\\Moban\\un\\aa.txt";
            string strzipPath = "D:\\Moban\\un\\free.zip";
            System.Diagnostics.Process Process1 = new System.Diagnostics.Process();
            Process1.StartInfo.FileName = "Winrar.exe";
            Process1.StartInfo.CreateNoWindow = true;
            Process1.StartInfo.Arguments = " a -r " + strzipPath + " " + strtxtPath;
            Process1.Start();
            Process1.WaitForExit();
            if (Process1.HasExited)
            {
                int iExitCode = Process1.ExitCode;
                if (iExitCode == 0)
                {
                    //正常完成
                }
                else
                {
                    //有错
                }
            }
            Process1.Close();
            return;
            try
            {
                RARsave("D:\\Moban\\un\\", "D:\\Moban\\rar\\", "aa");
            }
            catch (Exception ex)
            {
                
                throw;
            }
           
        }

        private void button32_Click(object sender, EventArgs e)
        {
           
        }

        private void button33_Click(object sender, EventArgs e)
        {
            string str1 = @"C:\WebSite7\Presentation1.jpg";
            string str = @"C:\WebSite7\Presentation1.ppt";
            //ApplicationClass pptApplication = new ApplicationClass(); 
            //Presentation pptPresentation = pptApplication.Presentations.Open(str, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
            //pptPresentation.Slides.Item[1].Export(str1, "jpg", 320, 240);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            try
            {
                CheckFiles checkFiles = new CheckFiles(this, txtSite.Text);
                Thread t = new Thread(checkFiles.DeownloadFiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button33_Click_1(object sender, EventArgs e)
        {
            try
            {
                CheckFiles checkFiles = new CheckFiles(this, txtSite.Text);
                Thread t = new Thread(checkFiles.AutoCheckFiles);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
        }

        private void button32_Click_1(object sender, EventArgs e)
        {
            try
            {
                PPTManager pptManager = new PPTManager(this,txtSite.Text);

                Thread t = new Thread(pptManager.PPTArrangeWithFile);
                t.IsBackground = true;
                t.Start();
                rtb_C.Clear();
            }
            catch (Exception ex)
            {
                rtb_C.AppendText(ex.Message);
            }
            
        }

        //生成图片
        public void ExportImages(string filePath, Microsoft.Office.Interop.PowerPoint.Slide slide, int index,int width)
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            slide.Export(filePath + "\\" + index + ".jpg", "jpg", width, (int)(slide.Application.Height * width / slide.Application.Width));
        }

        public void CreateIndexImg(string fileFolders,int id)
        {
            

            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            var imgFiles = from file in
                               Directory.EnumerateFiles(fileFolders)
                           where file.ToLower().EndsWith(".jpg")
                           select file;
            List<string> imgList = new List<string>();
            int mod = imgFiles.Count<string>() % 3;
            imgList = imgFiles.ToList();
            switch (mod)
            {
                case 1:
                    imgList.Remove("1.jpg");
                    break;
                case 2:
                    imgList.Add("D:\\Personal\\1.jpg");
                    break;
                default:
                    break;
            }

            if (imgFiles.Count<string>() == 1)
            {
                Bitmap b1 = new Bitmap(fileFolders + "1.jpg");          
                Size size = new System.Drawing.Size();
                size.Width = 650;
                size.Height = (int)b1.Height * 660 / b1.Width;
                Bitmap aImage = GetImageThumb(b1, size);
                SaveAsJPEG(aImage, fileFolders.Replace("n\\", "") + id + ".jpg", 50);
            }
            else
            {
                int row = imgList.Count / 5 + 1;
                if (row > 3)
                {
                    row = 3;
                }
                Bitmap b1 = new Bitmap(fileFolders + "1.jpg");
                int height = (int)b1.Height * 660 / b1.Width ;
                int rh = (int)b1.Height * 126 / b1.Width ;

                Size size = new System.Drawing.Size();
                size.Width = 660;
                size.Height = height;
                Bitmap indexImage = GetImageThumb(b1, size);

                Bitmap bImage = new Bitmap(660, height + (rh + 5) * row + 10);
                Graphics g = Graphics.FromImage(bImage);
                g.Clear(System.Drawing.Color.White);
                g.DrawImage(indexImage, 0, 0, 660, height);

                int i = 0;
                foreach (string item in imgList)
                {
                    Bitmap b = new Bitmap(item);
                    int left = 10 + (i % 5) * 10 + (i % 5) * 120;
                    int top = 5 + height + (i / 5) * 5 + (i / 5) * rh;
                    g.DrawImage(b, left, top, 120, rh);
                    i++;
                    if (i==15)
                    {
                        break;
                    }
                    rtb_C.AppendText(i + " : " + left + "  " + top + "\r\n");
                }

                MessageBox.Show(i.ToString());
                SaveAsJPEG(bImage, fileFolders.Replace("n\\", "") + id + ".jpg", 50);

                //aImage.Save(fileFolders.Replace("n\\", "") + "index2.jpg", System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        //压缩图片
        public Bitmap GetImageThumb(Bitmap mg, Size newSize)
        {
            double ratio = 0d;
            double myThumbWidth = 0d;
            double myThumbHeight = 0d;
            int x = 0;
            int y = 0;

            Bitmap bp;

            if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height /
            Convert.ToDouble(newSize.Height)))
                ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
            else
                ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
            myThumbHeight = Math.Ceiling(mg.Height / ratio);
            myThumbWidth = Math.Ceiling(mg.Width / ratio);

            Size thumbSize = new Size((int)newSize.Width, (int)newSize.Height);
            bp = new Bitmap(newSize.Width, newSize.Height);
            x = (newSize.Width - thumbSize.Width) / 2;
            y = (newSize.Height - thumbSize.Height);
            System.Drawing.Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
            g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);

            return bp;
        }

        public static bool GetPicThumbnail(string sFile, string outPath, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;

            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    iSource.Save(outPath, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    iSource.Save(outPath, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                iSource.Dispose();
            }
        }

        /// <summary>
        /// 保存为JPEG格式，支持压缩质量选项
        /// </summary>
        /// <param name="bmp">原始位图</param>
        /// <param name="FileName">新文件地址</param>
        /// <param name="Qty">压缩质量，越大越好，文件也越大(0-100)</param>
        /// <returns>成功标志</returns>
        public static bool SaveAsJPEG(Bitmap bmp, string FileName, int Qty)
        {
            try
            {
                EncoderParameter p;
                EncoderParameters ps;

                ps = new EncoderParameters(1);

                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Qty);
                ps.Param[0] = p;

                bmp.Save(FileName, GetCodecInfo("image/jpeg"), ps);

                return true;
            }
            catch
            {
                return false;
            }
        }
       
        /// <summary>
        /// 保存JPG时用
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns>得到指定mimeType的ImageCodecInfo</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }


        private Bitmap CreateNodeImg(string txt, Color txtColor)
        {
            if (txtColor == Color.Transparent)
                txtColor = Color.Black;
            Bitmap newBitMap = new Bitmap(12, 14);
            Graphics g = Graphics.FromImage(newBitMap);
            if (txtColor != Color.Black)
            {
                g.Clear(txtColor);//背景色
            }
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            FontFamily fm = new FontFamily("Arial");

            System.Drawing.Font font = new System.Drawing.Font(fm, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush sb = new SolidBrush(Color.Black);
            g.DrawString(txt, font, sb, new PointF(0, 0));
            g.Dispose();
            return newBitMap;
        }

 
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        private Bitmap MergerImg(params Bitmap[] maps)
        {
            int i = maps.Length;
            if (i == 0)
                throw new Exception("图片数不能够为0");
            //创建要显示的图片对象,根据参数的个数设置宽度
            Bitmap backgroudImg = new Bitmap(i * 12, 16);
            Graphics g = Graphics.FromImage(backgroudImg);
            //清除画布,背景设置为白色
            g.Clear(System.Drawing.Color.White);
            for (int j = 0; j < i; j++)
            {
                g.DrawImage(maps[j], j * 11, 0, maps[j].Width, maps[j].Height);
            }
            g.Dispose();
            return backgroudImg;
        }
  
        
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="bitMapDic"></param>
        /// <returns></returns>
        private Bitmap MergerImg(Dictionary<string, Bitmap> bitMapDic)
        {
            if (bitMapDic == null || bitMapDic.Count == 0)
                throw new Exception("图片数不能够为0");
            //创建要显示的图片对象,根据参数的个数设置宽度
            Bitmap backgroudImg = new Bitmap(bitMapDic.Count * 12, 16);
            Graphics g = Graphics.FromImage(backgroudImg);
            //清除画布,背景设置为白色
            g.Clear(System.Drawing.Color.White);
            int j = 0;
            foreach (KeyValuePair<string, Bitmap> entry in bitMapDic)
            {
                Bitmap map = entry.Value;
                g.DrawImage(map, j * 11, 0, map.Width, map.Height);
                j++;
            }
            g.Dispose();
            return backgroudImg;

            //合并图片还可以：
            //int i = maps.Length;
            //if (i == 0)
            //    throw new Exception("图片数不能够为0");
            ////创建要显示的图片对象,根据参数的个数设置宽度
            //Bitmap backgroudImg = new Bitmap(i * 16, 16);
            //Graphics g = Graphics.FromImage(backgroudImg);
            ////清除画布,背景设置为白色
            //g.Clear(System.Drawing.Color.White);
            //g.DrawImageUnscaled(maps[0], 0, 0);
            //g.DrawImageUnscaled(maps[1], maps[0].Width, 0);
            //g.Dispose();
            //return backgroudImg;
        }
 



        private void button35_Click(object sender, EventArgs e)
        {
            string fileFolders = "D:\\Personal\\1";
            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            Hashtable ht = new Hashtable();
            //遍历文件夹
            //foreach (FileInfo file in TheFolder.GetFiles())
            //{

            //try
            //{
                var imgFiles = from file in
                                Directory.EnumerateFiles(fileFolders)
                            where file.ToLower().EndsWith(".jpg")
                            select file;

                foreach (var imgFile in imgFiles)
                {
                    Console.WriteLine("{0}", imgFile);
                }
                Console.WriteLine("{0} files found.", imgFiles.Count<string>().ToString());
            //}
            //catch (UnauthorizedAccessException UAEx)
            //{
            //    Console.WriteLine(UAEx.Message);
            //}
            //catch (PathTooLongException PathEx)
            //{
            //    Console.WriteLine(PathEx.Message);
            //}

            //}
                List<string> list = new List<string>();
                int mod = imgFiles.Count<string>() % 3;
             list = imgFiles.ToList();
             switch (mod)
             {
                 case 1:
                     list.Remove("0.jpg");
                     break;
                 case 2:
                     list.Add("D:\\Personal\\1.jpg");
                     break;
                 default:
                     break;
             }
                
             //double[] arr = new double[50];
             //List<double> list = arr.ToList();
             //list.RemoveAt(5 + 1);
             //double[] newarr = list.ToArray();

            string jpg1 = "D:\\Personal\\1.jpg";
            Bitmap bImage = new Bitmap(650,700);
            //加载4张图片
            Bitmap b1 = new Bitmap(jpg1);
            Bitmap b2 = new Bitmap(jpg1);
            Bitmap b3 = new Bitmap(jpg1);
            Bitmap b4 = new Bitmap(jpg1);
            //依次写入图片就行行了啊
            //Graphics g = bImage.GetGraphics();
            Graphics g = Graphics.FromImage(bImage);
            g.Clear(System.Drawing.Color.Azure);

            g.DrawImage(b1, 0, 0, b1.Width, b1.Height);
            g.DrawImage(b1, b1.Width, b1.Height, b2.Width, b2.Height);
            g.DrawImage(b1, 0, b1.Height, b3.Width, b3.Height);
            g.DrawImage(b1, b1.Width, b1.Height, b4.Width, b4.Height);
            string filename = @"D:\Personal\2.jpg";
            bImage.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            CreateIndexImg("D:\\MYPPT\\images\\41484\\n\\",41484);
        }    
    
    }
}
  
   