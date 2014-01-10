using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Moban.Utils;
using Moban.Model;
using Moban.DAL;
using System.Collections;
using System.Data;

namespace MB
{
    public class DownWebSite
    {
        private MainForm form;
        private string urls;
        private string fileFolders;
        public DownWebSite(MainForm frm)
        {
            this.form = frm;
        }

        public DownWebSite(MainForm frm, string u)
        {
            this.form = frm;
            this.urls = u;
        }

        public DownWebSite(MainForm frm,string file, string u)
        {
            this.form = frm;
            this.urls = u;
            fileFolders = file;
        }

        public void DownLoadWebSite()
        {
            WebClient wc = new WebClient();
            Uri uri = new Uri("http://static.livedemo00.template-help.com/wt_37359/");

            string page = wc.DownloadString(uri.ToString());
            string mainFolder = Environment.CurrentDirectory + "\\" + uri.Host + "\\" + uri.AbsolutePath.Replace("/", "\\");

            //根目录
            if (!Directory.Exists(mainFolder))
                Directory.CreateDirectory(mainFolder);


            Regex rxpic = new Regex("href=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mcpic = rxpic.Matches(page);
            foreach (Match ml in mcpic)
            {
                string picUrl = ml.Groups[1].ToString();
                if (!picUrl.StartsWith("http"))
                {
                    picUrl = uri.ToString() + picUrl;
                }
                Uri uris = new Uri(picUrl);
                string ImgName = uris.Segments[uris.Segments.Length - 1].ToString();
                string fileFolders = Environment.CurrentDirectory + "\\" + uris.Host + "\\" + uris.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");

                string imgFile = fileFolders + ImgName;
                WebClient Wc = new WebClient();
                try
                {
                    if (!File.Exists(imgFile))
                    {
                        if (!Directory.Exists(fileFolders))
                            Directory.CreateDirectory(fileFolders);
                        Wc.DownloadFile(picUrl, imgFile);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 下载保存文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileFolders"></param>
        /// <param name="fileName"></param>
        private void DownLoadFile(string url, string fileFolders, string fileName)
        {
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(fileFolders))
                    Directory.CreateDirectory(fileFolders);
                new WebClient().DownloadFile(url, fileName);
            }
        }


        /// <summary>
        /// 根据首页
        /// </summary>
        public void GetWebSiteByHtml()
        {
            try
            {
                if (string.IsNullOrEmpty(urls))
                {
                    MessageBox.Show("地址不能为空");
                    return;
                }
                string[] urlA = urls.Split('\n');
                foreach (string url in urlA)
                {
                    HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                    HttpHelper.HttpGET(url + "index.html", url, 2, ref statusCode, 30, "", false);
                    if (statusCode == HttpStatusCode.OK)
                    {
                        DownLoadFile(url + "index.html", "E:\\Moban\\Site\\");
                    }
                }
                 
                MobanInfo model = new MobanInfo();
                MobanDAL DAL = new MobanDAL();
                DirectoryInfo TheFolder = new DirectoryInfo("E:\\Moban\\Site\\");
                //遍历文件夹
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    model = DAL.GetModelByFileName("/" + NextFolder.Name + "/");
                    if (model != null)
                    {

                        GetWebSiteFiles(NextFolder.FullName, model.DemoUrl.Replace("index.html", ""), "E:\\Moban\\Site\\");
                        //model.IsDownLoad = 2;
                        //DAL.UpdateIsDownLoad(model);

                    }

                    //GetWebSiteFiles(urls, "");
                    //this.listBox1.Items.Add(NextFolder.Name);
                }

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Message, MainForm.richTextBoxC);
                //richTextBoxLog.AppendText(DateTime.Now + ex.Message + "\n");
            }


        }

        public void GetWebSiteByID()
        {
            try
            {
                if (string.IsNullOrEmpty(urls))
                {
                    MessageBox.Show("地址不能为空");
                    return;
                }
                string[] urlA = urls.Split('\n');
                MobanInfo model ;
                List<MobanInfo> list = new List<MobanInfo>(0);
                MobanDAL DAL = new MobanDAL();
                foreach (string url in urlA)
                {
                    try
                    {
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        model = DAL.GetModelByFileNameWithId(int.Parse(url));
                        if (model==null)
                        {
                            continue;
                        }
                        string demourl = model.DemoUrl;
                        if (!demourl.EndsWith("index.html"))
                        {
                            demourl += "index.html";
                        }
                        HttpHelper.HttpGET(demourl, demourl, 2, ref statusCode, 30, "", false);
                        if (statusCode == HttpStatusCode.OK)
                        {
                            DownLoadFile(demourl, "E:\\Moban\\NewSite\\" + model.Id + "\\");
                            list.Add(model);//
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);    
                    }           
                }



                //DirectoryInfo TheFolder = new DirectoryInfo("E:\\Moban\\NewSite\\");
                ////遍历文件夹
                //foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                //{
                //    model = DAL.GetModelByFileName("/" + NextFolder.Name + "/");
                //    if (model != null)
                //    {

                //        GetWebSiteFiles(NextFolder.FullName, model.DemoUrl.Replace("index.html", ""), "E:\\Moban\\Site\\");
                //        //model.IsDownLoad = 2;
                //        //DAL.UpdateIsDownLoad(model);

                //    }

                //    //GetWebSiteFiles(urls, "");
                //    //this.listBox1.Items.Add(NextFolder.Name);
                //}

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Message, MainForm.richTextBoxC);
                //richTextBoxLog.AppendText(DateTime.Now + ex.Message + "\n");
            }

            form.AddMessage("\n\n全部完成", MainForm.richTextBoxC);  
        }


        #region 小蔡

        /// <summary>
        /// 自动补全
        /// </summary>
        public void Xiaocai()
        {

            MobanInfo model = new MobanInfo();
            MobanDAL bll = new MobanDAL();
            TxtHtml("E:\\TXT\\");
            //richTextBoxC.AppendText("完成" + "\n");
            form.AddMessage("wancheng", MainForm.richTextBoxC);
        }

        public void TxtHtml(string folderFullName)
        {
            MobanInfo model = new MobanInfo();
            MobanDAL bll = new MobanDAL();

            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                //richTextBoxC.AppendText(NextFolder.Name + "\n\n\n");
                form.AddMessage(NextFolder.FullName + "\n\n", MainForm.richTextBoxLog);
                TxtHtml(NextFolder.FullName);
            }
            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                try
                {
                    if (NextFile.Extension != ".txt")
                    {
                        continue;
                    }
                    if (!bll.Exists(NextFile.Name))
                    {
                        string strBody = "";
                        using (StreamReader reader = new StreamReader(NextFile.FullName, Encoding.Default))
                        {
                            string l = String.Empty;
                            while ((l = reader.ReadLine()) != null)
                            {
                                if (!string.IsNullOrEmpty(l.Trim()))
                                {
                                    strBody += "<p>" + l + "</p>" + Environment.NewLine;
                                }
                            }
                        }
                        // string strBody = File.ReadAllText(NextFile.FullName);
                        bll.Add(NextFile.Name, strBody);
                        //richTextBoxC.AppendText(NextFile.Name + "\n");
                        form.AddMessage(NextFile.Name + "\n", MainForm.richTextBoxC);
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(ex.Message + "\n\n", MainForm.richTextBoxLog);
                   // richTextBoxLog.AppendText(ex.Message + "\n");

                }

            }
        }
       
        #endregion

        /// <summary>
        /// 自动补全
        /// </summary>
        public void AutoGetFiles()
        {

            MobanInfo model = new MobanInfo();
            MobanDAL bll = new MobanDAL();
            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                model = bll.GetModelByFileName("/" + NextFolder.Name + "/");
                if (model != null)
                {

                    GetWebSiteFiles(NextFolder.FullName, model.DemoUrl.Replace("index.html", "") + "/", fileFolders);

                    //System.Threading.Thread.Sleep(5 * 1000);
                }
                //GetWebSiteFiles(urls, "");
                //this.listBox1.Items.Add(NextFolder.Name);
            }

        }

        /// <summary>
        /// 根据id 自动补全
        /// </summary>
        public void NewAutoGetFiles()
        {

            MobanInfo model = new MobanInfo();
            MobanDAL bll = new MobanDAL();
            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                try
                {
                    model = bll.GetModelByFileNameWithId(int.Parse(NextFolder.Name));
                    if (model != null)
                    {
                        Uri uri = new Uri(model.DemoUrl);

                        NewGetWebSiteFiles(NextFolder.FullName, uri, fileFolders + "\\" + model.Id);                        
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                    continue;
                }
            }
            form.AddMessage("\n\n全部完成", MainForm.richTextBoxC);    
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="picUrl"></param>
        /// <param name="subFileFolder"></param>
        public void DownLoadFile(string picUrl, string subFileFolder)
        {
            try
            {
                Uri uri = new Uri(picUrl);
                string ImgName = uri.Segments[uri.Segments.Length - 1].ToString();
                string fileFolder = subFileFolder + uri.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");
                string imgFile = fileFolder + ImgName;
                if (!File.Exists(imgFile))
                {
                    if (!Directory.Exists(fileFolder))
                        Directory.CreateDirectory(fileFolder);
                    WebClient Wc = new WebClient();
                    Wc.DownloadFile(picUrl, imgFile);
                    Console.WriteLine(DateTime.Now + imgFile);
                    form.AddMessage(picUrl, MainForm.richTextBoxC);
                    //richTextBoxC.AppendText(picUrl + "\n");
                    //System.Threading.Thread.Sleep(1*1000);
                }
            }
            catch (Exception ex)
            {
                form.AddMessage(picUrl + ex.Message, MainForm.richTextBoxLog);
                //richTextBoxLog.AppendText(picUrl + ex.Message + "\n");
            }
        }

        /// <summary>
        /// 或者站点下的内容
        /// </summary>
        /// <param name="folderFullName"></param>
        /// <param name="website"></param>
        public void GetWebSiteFiles(string folderFullName, string website, string subFileFolder)
        {
            Hashtable hashtable = new Hashtable();
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                GetWebSiteFiles(NextFolder.FullName, website, subFileFolder);
                //this.listBox1.Items.Add(NextFolder.Name);
            }
            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                string strBody = File.ReadAllText(NextFile.FullName);

                //获取图片
                Regex imglink = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcimglink = imglink.Matches(strBody);
                foreach (Match ml in mcimglink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                Regex cssimglink = new Regex("url\\((.*?)\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection mccssimglink = cssimglink.Matches(strBody);
                foreach (Match ml in mccssimglink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex csslink = new Regex("@import\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mccsslink = csslink.Matches(strBody);
                foreach (Match ml in mccsslink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex jslink = new Regex("include\\(\"(.*?)\"\\);", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcjslink = jslink.Matches(strBody);
                foreach (Match ml in mcjslink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex alink = new Regex("href=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcalink = alink.Matches(strBody);
                foreach (Match ml in mcalink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex swflink = new Regex("value=\"(.*?)\\.swf\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcswflink = swflink.Matches(strBody);
                foreach (Match ml in mcswflink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString() + ".swf", "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                foreach (DictionaryEntry item in hashtable)
                {
                    string url = item.Key.ToString();
                    if (url.IndexOf('+') > -1 || url.IndexOf("#") > -1)
                    {
                        continue;
                    }
                    url = url.Replace("\"", "").Trim();
                    url = url.Replace("'", "").Trim();
                    url = url.Replace("../", "").Trim();
                    url = url.Replace(website, "");
                    DownLoadFile(website + url, subFileFolder);
                }

            }
        }

        /// <summary>
        /// 根据id 自动补全
        /// </summary>
        /// <param name="folderFullName"></param>
        /// <param name="website"></param>
        /// <param name="subFileFolder"></param>
        public void NewGetWebSiteFiles(string folderFullName, Uri uri, string subFileFolder)
        {
            Hashtable hashtable = new Hashtable();
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                NewGetWebSiteFiles(NextFolder.FullName, uri, subFileFolder);
                //this.listBox1.Items.Add(NextFolder.Name);
            }
            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                string strBody = File.ReadAllText(NextFile.FullName);
                strBody = strBody.Replace(" = ", "=");

                string thisFileFolder = NextFile.Directory.FullName.Replace(subFileFolder,"");

                //获取图片
                Regex imglink = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcimglink = imglink.Matches(strBody);
                foreach (Match ml in mcimglink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                Regex cssimglink = new Regex("url\\((.*?)\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection mccssimglink = cssimglink.Matches(strBody);
                foreach (Match ml in mccssimglink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex csslink = new Regex("@import\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mccsslink = csslink.Matches(strBody);
                foreach (Match ml in mccsslink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex jslink = new Regex("include\\(\"(.*?)\"\\);", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcjslink = jslink.Matches(strBody);
                foreach (Match ml in mcjslink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex alink = new Regex("href=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcalink = alink.Matches(strBody);
                foreach (Match ml in mcalink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex swflink = new Regex("value=\"(.*?)\\.swf\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcswflink = swflink.Matches(strBody);
                foreach (Match ml in mcswflink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString() + ".swf", "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                foreach (DictionaryEntry item in hashtable)
                {
                    string url = item.Key.ToString();
                    if (url.IndexOf('+') > -1 || url.IndexOf("#") > -1)
                    {
                        continue;
                    }
                    url = url.Replace("\"", "").Trim();
                    url = url.Replace("\\", "").Trim();
                    url = url.Replace("'", "").Trim();

                    if (url.IndexOf(uri.Host)>-1) //如果是绝对网址
                    {
                         DownLoadFile(url, subFileFolder);
                    }
                    //url = url.Replace("../", "").Trim();
                    //url = url.Replace(website, "");
                    
                    if (url.StartsWith("http"))
                    {
                        continue;
                    }
                    if (url.Length==0)
                    {
                        continue;
                    }
                    if (url.StartsWith("javascript:"))
                    {
                        continue;
                    }
                    if (url.IndexOf("@") > -1)
                    {
                        continue;
                    }
                    if (url.StartsWith("../../"))
                    {
                        url = url.Replace("../../","/");
                        thisFileFolder = thisFileFolder.Replace("\\" + NextFile.Directory.Name, "");
                        thisFileFolder = thisFileFolder.Replace("\\" + NextFile.Directory.Parent.Name, "");
                        url = uri.Scheme + "://" + uri.Host + thisFileFolder + url;
                    }
                    if (url.StartsWith("../"))
                    {
                        url = url.Replace("../", "/");
                        thisFileFolder = thisFileFolder.Replace("\\" + NextFile.Directory.Name, "");
                        url = uri.Scheme + "://" + uri.Host + thisFileFolder + url;
                    }
                    if (url.StartsWith("/"))
                    {
                        url = uri.Scheme + "://" + uri.Host + url;
                    }
                    url = url.Replace("\\","/");
                    if (!url.StartsWith("http"))
                    {
                        url = uri.ToString().Replace("index.html", "") + "/" + url;
                    }


                    DownLoadFile(url, subFileFolder);
                }

            }
        }

    }


    public class DownThemeforestWebSite
    {
        private MainForm form;
        private string urls;
        private string fileFolders;
        public DownThemeforestWebSite(MainForm frm)
        {
            this.form = frm;
        }

        public DownThemeforestWebSite(MainForm frm, string u)
        {
            this.form = frm;
            this.urls = u;
        }

        public DownThemeforestWebSite(MainForm frm, string file, string u)
        {
            this.form = frm;
            this.urls = u;
            fileFolders = file;
        }

        /// <summary>
        /// 下载保存文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileFolders"></param>
        /// <param name="fileName"></param>
        private void DownLoadFile(string url, string fileFolders, string fileName)
        {
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(fileFolders))
                    Directory.CreateDirectory(fileFolders);
                new WebClient().DownloadFile(url, fileName);
            }
        }

        /// <summary>
        /// 根据首页
        /// </summary>
        public void GetWebSiteByHtml()
        {
            try
            {
                if (string.IsNullOrEmpty(urls))
                {
                    MessageBox.Show("地址不能为空");
                    return;
                }
                string[] urlA = urls.Split('\n');
                foreach (string item in urlA)
                {
                    string[] idurl = item.Split('|');
                    if (idurl.Length == 2)
                    {
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        HttpHelper.HttpGET(idurl[1] + "index.html", idurl[1], 2, ref statusCode, 30, "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.57 Safari/537.36", false);
                        if (statusCode == HttpStatusCode.OK)
                        {
                            DownLoadFile(idurl[1] + "index.html", "E:\\Moban\\Themeforest\\" + idurl[0] + "\\");
                        }
                    }
                }

                MobanInfo model = new MobanInfo();
                MobanDAL DAL = new MobanDAL();
                DirectoryInfo TheFolder = new DirectoryInfo("E:\\Moban\\Themeforest\\");
                //遍历文件夹
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    try
                    {
                        model = DAL.GetModelByFileNameWithId(int.Parse(NextFolder.Name));
                        if (model != null)
                        {

                            GetWebSiteFiles(NextFolder.FullName, model.DemoUrl.Replace("index.html", ""), "E:\\Moban\\Themeforest\\" + model.Id + "\\");
                            //model.IsDownLoad = 2;
                            //DAL.UpdateIsDownLoad(model);

                        }

                        //GetWebSiteFiles(urls, "");
                        //this.listBox1.Items.Add(NextFolder.Name);
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                        continue;
                    }
                    
                }

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                //richTextBoxLog.AppendText(DateTime.Now + ex.Message + "\n");
            }
        }


        /// <summary>
        /// 自动补全
        /// </summary>
        public void AutoGetFiles()
        {

            MobanInfo model = new MobanInfo();
            MobanDAL bll = new MobanDAL();
            DirectoryInfo TheFolder = new DirectoryInfo("E:\\Moban\\Themeforest\\");
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                model = bll.GetModelByFileNameWithId(int.Parse(NextFolder.Name));
                if (model != null)
                {

                    GetWebSiteFiles(NextFolder.FullName, model.DemoUrl.Replace("index.html", "") + "/", "E:\\Moban\\Themeforest\\" + model.Id + "\\");

                    //System.Threading.Thread.Sleep(5 * 1000);
                }
                //GetWebSiteFiles(urls, "");
                //this.listBox1.Items.Add(NextFolder.Name);
            }

        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="picUrl"></param>
        /// <param name="subFileFolder"></param>
        public void DownLoadFile(string picUrl, string subFileFolder)
        {

            Uri uri = new Uri(picUrl);
            string ImgName = uri.Segments[uri.Segments.Length - 1].ToString();
            string fileFolder = subFileFolder + uri.Host + uri.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");
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
                    form.AddMessage(picUrl, MainForm.richTextBoxC);
                    //richTextBoxC.AppendText(picUrl + "\n");
                    //System.Threading.Thread.Sleep(1*1000);
                }
            }
            catch (Exception ex)
            {
                form.AddMessage(picUrl + ex.Message, MainForm.richTextBoxLog);
                //richTextBoxLog.AppendText(picUrl + ex.Message + "\n");
            }
        }

        /// <summary>
        /// 或者站点下的内容
        /// </summary>
        /// <param name="folderFullName"></param>
        /// <param name="website"></param>
        public void GetWebSiteFiles(string folderFullName, string website, string subFileFolder)
        {
            Hashtable hashtable = new Hashtable();
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                GetWebSiteFiles(NextFolder.FullName, website, subFileFolder);
                //this.listBox1.Items.Add(NextFolder.Name);
            }
            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                string strBody = File.ReadAllText(NextFile.FullName);

                //获取图片
                Regex imglink = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcimglink = imglink.Matches(strBody);
                foreach (Match ml in mcimglink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                Regex cssimglink = new Regex("url\\((.*?)\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection mccssimglink = cssimglink.Matches(strBody);
                foreach (Match ml in mccssimglink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex csslink = new Regex("@import\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mccsslink = csslink.Matches(strBody);
                foreach (Match ml in mccsslink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex jslink = new Regex("include\\(\"(.*?)\"\\);", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcjslink = jslink.Matches(strBody);
                foreach (Match ml in mcjslink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex alink = new Regex("href=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcalink = alink.Matches(strBody);
                foreach (Match ml in mcalink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString(), "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regex swflink = new Regex("value=\"(.*?)\\.swf\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mcswflink = swflink.Matches(strBody);
                foreach (Match ml in mcswflink)
                {
                    try
                    {
                        hashtable.Add(ml.Groups[1].ToString() + ".swf", "");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                foreach (DictionaryEntry item in hashtable)
                {
                    string url = item.Key.ToString();
                    if (url.IndexOf('+') > -1 || url.IndexOf("#") > -1)
                    {
                        continue;
                    }
                    if (url.StartsWith("http"))
                    {
                        if (url.IndexOf("website") == -1)
                        {
                            continue;
                        }
                    }
                    url = url.Replace("\"", "").Trim();
                    url = url.Replace("'", "").Trim();
                    url = url.Replace("../", "").Trim();
                    url = url.Replace("./", "").Trim();
                    url = url.Replace(website, "");
                    url = website + url;

                    DownLoadFile(url, subFileFolder);
                }
            }
        }
    }

    public class CheckFiles
    {
        private MainForm form;
        private string urls;
        private string fileFolders;
        public CheckFiles(MainForm frm)
        {
            this.form = frm;
        }

        public CheckFiles(MainForm frm, string folders)
        {
            this.form = frm;
            this.fileFolders = folders;
        }


        public void AutoCheckFiles()
        {

            PPTInfo model = new PPTInfo();
            PPTDAL bll = new PPTDAL();
            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            Hashtable ht = new Hashtable();
            //遍历文件夹
            foreach (FileInfo file in TheFolder.GetFiles())
            {
                try
                {
                    ht.Add(file.Name,"");
                }
                catch (Exception ex)
                {

                    form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                    continue;
                }

            }
            form.AddMessage("开始判断", MainForm.richTextBoxC);
            int i=ht.Count;;
            foreach (DictionaryEntry item in ht)
            {
                i--;
                try
                {
                    form.AddMessage(i.ToString() + "     " + item.Key.ToString(), MainForm.richTextBoxC);
                    model = bll.GetModelByFileName(item.Key.ToString());
                    if (model != null)
                    {
                        if (model.IsDown == 0)
                        {
                            model.IsDown = 1;
                            bll.UpdateIsDownLoad(model);
                            form.AddMessage(item.Key.ToString(), MainForm.richTextBoxC);
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                    continue;
                }
            }
            

            form.AddMessage("------END---------", MainForm.richTextBoxC);
        }

        public void MoveDeownloadFiles()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            //遍历文件夹
            foreach (FileInfo file in TheFolder.GetFiles())
            {
                try
                {
                    string fileName = System.Web.HttpUtility.UrlDecode(file.Name, Encoding.UTF8);
                    //file.MoveTo("E:\\PPTs\\www.51ppt.com.cn\\" + fileName);
                    form.AddMessage(fileName, MainForm.richTextBoxC);
                     
                }
                catch (Exception ex)
                {

                    form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                    continue;
                }

            }
            form.AddMessage("------END---------", MainForm.richTextBoxC);
        }

        public void DeownloadFiles()
        {
            bool IsStop = true;
            while (IsStop)
            {
                PPTDAL bll = new PPTDAL();
                DataSet ds = bll.GetNeedDownList();
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count == 0)
                {
                    form.AddMessage(DateTime.Now + "...............全部完成.............", MainForm.richTextBoxC);
                    IsStop = false;
                    return;
                }
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        PPTInfo model = new PPTInfo();
                        model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                        model.DownLink = dt.Rows[i]["downlink"].ToString();
                        model.Host = dt.Rows[i]["host"].ToString();
                        if (string.IsNullOrEmpty(model.DownLink))
                        {
                            continue;
                        }

                        bool rt = DeownloadFile(model.Host, model.DownLink);
                        if (rt)
                        {
                            form.AddMessage(DateTime.Now + model.DownLink, MainForm.richTextBoxC);
                            model.IsDown = 1;
                            bll.UpdateIsDownLoad(model);
                        }
                        else 
                        {
                            form.AddMessage(model.DownLink, MainForm.richTextBoxLog); 
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                        continue;
                    }
                }
                form.AddMessage("------END---------", MainForm.richTextBoxC);
            }
        }


        public bool DeownloadFile(string host, string fileurl)
        {
            bool flag = false;
            //打开上次下载的文件
            long SPosition = 0;
            //实例化流对象
            FileStream FStream;

            string fileFolder = "E:\\PPTs\\" + host + "\\";
            if (!Directory.Exists(fileFolder))
                Directory.CreateDirectory(fileFolder);
            
            Uri uri = new Uri(fileurl);
            string fileName = uri.Segments[uri.Segments.Length - 1].ToString();
            fileName = System.Web.HttpUtility.UrlDecode(fileName, Encoding.UTF8);

            string strFileName = fileFolder + fileName;
            //判断要下载的文件夹是否存在
            if (File.Exists(strFileName))
            {
                //打开要下载的文件
                FStream = File.OpenWrite(strFileName);
                //获取已经下载的长度
                SPosition = FStream.Length;
                FStream.Seek(SPosition, SeekOrigin.Current);
            }
            else
            {
                //文件不保存创建一个文件
                FStream = new FileStream(strFileName, FileMode.Create);
                SPosition = 0;
            }
            try
            {
                //打开网络连接
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                if (SPosition > 0)
                    myRequest.AddRange((int)SPosition);             //设置Range值
                //向服务器请求,获得服务器的回应数据流
                Stream myStream = myRequest.GetResponse().GetResponseStream();
                //定义一个字节数据
                byte[] btContent = new byte[512];
                int intSize = 0;
                intSize = myStream.Read(btContent, 0, 512);
                while (intSize > 0)
                {
                    FStream.Write(btContent, 0, intSize);
                    intSize = myStream.Read(btContent, 0, 512);
                }
                //关闭流
                FStream.Close();
                myStream.Close();
                flag = true;        //返回true下载成功
            }
            catch (Exception)
            {
                FStream.Close();
                flag = false;       //返回false下载失败
            }
            return flag;
        }
         
    }

 
}
