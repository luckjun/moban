using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Moban.Model;
using Moban.Utils;
using Moban.DAL;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.IO;

namespace MB
{
    public class Crawl
    {
        public volatile bool shouldStop;
        private MainForm form;
        private int iStart;
        private int iEnd;
        private bool synchronous;
        //SqlHelp sqlHelp = new SqlHelp();

        Moban.DAL.MobanDAL bll = new MobanDAL();

        public Crawl(MainForm frm, int iS, int iE)
        {
            this.form = frm;
            this.iEnd = iE;
            this.iStart = iS;
            this.synchronous = true;
        }

        public void Templatemonster()
        {
            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                DateTime b1 = DateTime.Now;
                string page = "http://www.templatemonster.com/category.php?from=";
                string pageBody = string.Empty;
                string pattern = "<td>(.*?)</td>";
                string linkPattern = "data-detail_page=\"(.*?)\"";
                MobanDAL bll = new MobanDAL();
                for (int j = iStart; j < iEnd; j++)
                {
                    try
                    {
                        CookieCollection cookieCollection = new CookieCollection();
                        form.AddMessage(DateTime.Now + "内容：" + page + j, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(page + j, "http://www.templatemonster.com/", 2, ref cookieCollection, 30, "", false);
                        form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);

                            //获取列表item
                            foreach (Match m in mc)
                            {
                                MobanInfo mobaninfo = new Moban.Model.MobanInfo();
                                mobaninfo.CrawlTime = DateTime.Now;
                                mobaninfo.CreateTime = DateTime.Now;
                                mobaninfo.Host = "templatemonster.com";

                                Regex rxlink = new Regex(linkPattern, RegexOptions.IgnoreCase);
                                MatchCollection mclink = rxlink.Matches(m.ToString());
                                foreach (Match ml in mclink)
                                {
                                    mobaninfo.Url = ml.Groups[1].ToString();
                                    form.AddMessage("网址:" + mobaninfo.Url, MainForm.richTextBoxC);
                                }
                                if (!string.IsNullOrEmpty(mobaninfo.Url) && mobaninfo.Url.EndsWith("html"))
                                {

                                    Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase);
                                    MatchCollection mcpic = rxpic.Matches(m.ToString());
                                    foreach (Match ml in mcpic)
                                    {
                                        mobaninfo.MPic = ml.Groups[1].ToString();
                                        form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                        break;
                                    }

                                    Regex rxtype = new Regex("Template Type(?<href>(.*?))>(?<type>(.*?))</a><br/>", RegexOptions.IgnoreCase);
                                    MatchCollection mcrxtype = rxtype.Matches(m.ToString());
                                    foreach (Match ml in mcrxtype)
                                    {
                                        mobaninfo.Type = ml.Groups["type"].Value.Trim();
                                        form.AddMessage("类型:" + mobaninfo.Type, MainForm.richTextBoxC);
                                        break;
                                    }

                                    Regex rxid = new Regex("Item: #(?<id>(\\d*?))<br/>", RegexOptions.IgnoreCase);
                                    MatchCollection mcrxid = rxid.Matches(m.ToString());
                                    foreach (Match ml in mcrxid)
                                    {
                                        mobaninfo.Demo = "http://www.templatemonster.com/demo/" + ml.Groups["id"].Value.Trim() + ".html";
                                        form.AddMessage("Demo:" + mobaninfo.Demo, MainForm.richTextBoxC);
                                        break;
                                    }
                                    
                                    mobaninfo.BPic = mobaninfo.MPic.Replace("-m.", "-" + mobaninfo.Type.ToLower() + "-b.");

                                    if (!bll.Exists(mobaninfo.Url))
                                    {
                                        if (bll.Add(mobaninfo) == 0)
                                        {
                                            form.AddMessage("保存失败", MainForm.richTextBoxC);
                                        }
                                        else
                                        {
                                            form.AddMessage("保存成功", MainForm.richTextBoxC);
                                        }
                                    }
                                    else
                                    {
                                        form.AddMessage("已经存在", MainForm.richTextBoxC);
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                    }

                    System.Threading.Thread.Sleep(3 * 1000);
                }
            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxLog);
                System.Threading.Thread.Sleep(3 * 1000);
            }
            form.AddMessage("完成", MainForm.richTextBoxC);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Themeforest()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://themeforest.net/category/all?page=";

                string pageBody = string.Empty;
                string pattern = "data-item-id=(.*?)<small class=\"price";
                string linkPattern = "<a href=\"http://themeforest.net/item/(.*?)\\?";
                MobanDAL bll = new MobanDAL();
                for (int j = iStart; j < iEnd; j++)
                {
                    try
                    {
                        string pageurl =  page + j;
                        if (j==1)
                        {
                            pageurl = "http://themeforest.net/category/all";
                        }
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + pageurl, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(pageurl, "http://themeforest.net/", 2, ref statusCode, 30, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(5 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);

                            //获取列表item
                            foreach (Match m in mc)
                            {
                                MobanInfo mobaninfo = new Moban.Model.MobanInfo();
                                mobaninfo.CrawlTime = DateTime.Now;
                                mobaninfo.CreateTime = DateTime.Now;
                                mobaninfo.Host = "themeforest.net";

                                Regex rxlink = new Regex(linkPattern, RegexOptions.IgnoreCase);
                                MatchCollection mclink = rxlink.Matches(m.ToString());
                                foreach (Match ml in mclink)
                                {
                                    mobaninfo.Url = "http://themeforest.net/item/" + ml.Groups[1].ToString();
                                    form.AddMessage("网址:" + mobaninfo.Url, MainForm.richTextBoxC);
                                }
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase);
                                MatchCollection mcpic = rxpic.Matches(m.ToString());
                                foreach (Match ml in mcpic)
                                {
                                    mobaninfo.MPic = ml.Groups[1].ToString();
                                    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                    break;
                                }
                                mobaninfo.BPic = "";
                                //Regex rxtype = new Regex("<span class=\"meta-categories\">in(?<type>(.*?))</span>", RegexOptions.IgnoreCase);
                                //MatchCollection mcrxtype = rxtype.Matches(m.ToString());
                                //foreach (Match ml in mcrxtype)
                                //{
                                //    mobaninfo.Type = ml.Groups["type"].Value.Trim();
                                //    form.AddMessage("类型:" + mobaninfo.Type, MainForm.richTextBoxC);
                                //    break;
                                //}
                                mobaninfo.Type = "";
                                //pageBody = HttpHelper.HttpGET(mobaninfo.Url, "http://themeforest.net/", 2, ref statusCode, 30, "", false);
                                //if (statusCode == HttpStatusCode.OK)
                                //{
                                //    mobaninfo.Body = pageBody;
                                //    Regex rxid = new Regex("<a href=\"(.*?)\" role=\"button\" class=\"btn btn-icon live-preview\"", RegexOptions.IgnoreCase);
                                //    MatchCollection mcrxid = rxid.Matches(pageBody);
                                //    foreach (Match ml in mcrxid)
                                //    {
                                //        mobaninfo.Demo = "http://themeforest.net" + ml.Groups[1].Value.Trim();
                                //        form.AddMessage("Demo:" + mobaninfo.Demo, MainForm.richTextBoxC);
                                //        break;
                                //    }
                                //    mobaninfo.IsDownLoad = 1;
                                //}

                                if (!bll.Exists(mobaninfo.Url))
                                {
                                    if (bll.Add(mobaninfo) == 0)
                                    {
                                        form.AddMessage("保存失败", MainForm.richTextBoxC);
                                    }
                                    else
                                    {
                                        form.AddMessage("保存成功", MainForm.richTextBoxC);
                                    }
                                }
                                else
                                {
                                    form.AddMessage("已经存在", MainForm.richTextBoxC);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxC);
                    }

                    System.Threading.Thread.Sleep(3 * 1000);

                }
               
            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }

        /// <summary>
        /// elemisfreebies.com
        /// </summary>
        public void Elemisfreebies()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://elemisfreebies.com/page/";

                string pageBody = string.Empty;
                string pattern = "featured(.*?)(.*?)End Post";
                string linkPattern = "<h2 class=\"title\"><a href=\"(.*?)\">(.*?)</a></h2>";
                MobanDAL bll = new MobanDAL();
                for (int j = iStart; j < iEnd; j++)
                {
                    try
                    {
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + page + j, MainForm.richTextBoxC);
                        string pageurl = page + j + "/";
                        if (j==1)
                        {
                            pageurl = "http://elemisfreebies.com/";
                        }
                        pageBody = HttpHelper.HttpGET(pageurl, "http://elemisfreebies.com/", 2, ref statusCode, 30, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(5 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);

                            //获取列表item
                            foreach (Match m in mc)
                            {
                                MobanInfo mobaninfo = new Moban.Model.MobanInfo();
                                mobaninfo.CrawlTime = DateTime.Now;
                                mobaninfo.CreateTime = DateTime.Now;
                                mobaninfo.Host = "elemisfreebies.com";
                                Regex rxlink = new Regex(linkPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mclink = rxlink.Matches(m.ToString());
                                foreach (Match ml in mclink)
                                {
                                    mobaninfo.Url = ml.Groups[1].ToString().Trim();
                                    
                                    mobaninfo.Name = ml.Groups[2].ToString().Trim();
                                    form.AddMessage("网址:" + mobaninfo.Url, MainForm.richTextBoxC);
                                }
                                Regex rxpic = new Regex("thumb.php\\?src=(.*?)&", RegexOptions.IgnoreCase);
                                MatchCollection mcpic = rxpic.Matches(m.ToString());
                                foreach (Match ml in mcpic)
                                {
                                    mobaninfo.MPic = ml.Groups[1].ToString();
                                    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                    break;
                                }
                                if (string.IsNullOrEmpty(mobaninfo.MPic))
                                {
                                    continue;
                                }
                                mobaninfo.BPic = mobaninfo.MPic;
                                //Regex rxtype = new Regex("<span class=\"meta-categories\">in(?<type>(.*?))</span>", RegexOptions.IgnoreCase);
                                //MatchCollection mcrxtype = rxtype.Matches(m.ToString());
                                //foreach (Match ml in mcrxtype)
                                //{
                                //    mobaninfo.Type = ml.Groups["type"].Value.Trim();
                                //    form.AddMessage("类型:" + mobaninfo.Type, MainForm.richTextBoxC);
                                //    break;
                                //}
                                mobaninfo.Type = "";
                                //pageBody = HttpHelper.HttpGET(mobaninfo.Url, "http://themeforest.net/", 2, ref statusCode, 30, "", false);
                                //if (statusCode == HttpStatusCode.OK)
                                //{
                                //    mobaninfo.Body = pageBody;
                                //    Regex rxid = new Regex("<a href=\"(.*?)\" role=\"button\" class=\"btn btn-icon live-preview\"", RegexOptions.IgnoreCase);
                                //    MatchCollection mcrxid = rxid.Matches(pageBody);
                                //    foreach (Match ml in mcrxid)
                                //    {
                                //        mobaninfo.Demo = "http://themeforest.net" + ml.Groups[1].Value.Trim();
                                //        form.AddMessage("Demo:" + mobaninfo.Demo, MainForm.richTextBoxC);
                                //        break;
                                //    }
                                //    mobaninfo.IsDownLoad = 1;
                                //}

                                if (!bll.Exists(mobaninfo.Url))
                                {
                                    if (bll.Add(mobaninfo) == 0)
                                    {
                                        form.AddMessage("保存失败", MainForm.richTextBoxC);
                                    }
                                    else
                                    {
                                        form.AddMessage("保存成功", MainForm.richTextBoxC);
                                    }
                                }
                                else
                                {
                                    form.AddMessage("已经存在", MainForm.richTextBoxC);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxC);
                    }

                    System.Threading.Thread.Sleep(3 * 1000);

                }

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }

        /// <summary>
        /// freepsdfiles.net
        /// </summary>
        public void Freepsdfiles()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://freepsdfiles.net/page/";

                string pageBody = string.Empty;
                string pattern = "<div class=\"post\">(.*?)<div class=\"box-gradient\"";
                string linkPattern = "<h2><a href=\"(.*?)\"(.*?)>(.*?)</a></h2>";
                MobanDAL bll = new MobanDAL();
                for (int j = iStart; j < iEnd; j++)
                {
                    try
                    {
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + page + j, MainForm.richTextBoxC);
                        string pageurl = page + j + "/";
                        if (j == 1)
                        {
                            pageurl = "http://freepsdfiles.net/";
                        }
                        pageBody = HttpHelper.HttpGET(pageurl, "http://freepsdfiles.net/", 2, ref statusCode, 30, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(5 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);

                            //获取列表item
                            foreach (Match m in mc)
                            {
                                MobanInfo mobaninfo = new Moban.Model.MobanInfo();
                                mobaninfo.CrawlTime = DateTime.Now;
                                mobaninfo.CreateTime = DateTime.Now;
                                mobaninfo.Host = "freepsdfiles.net";

                                Regex rxlink = new Regex(linkPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mclink = rxlink.Matches(m.ToString());
                                foreach (Match ml in mclink)
                                {
                                    mobaninfo.Url = ml.Groups[1].ToString().Trim();

                                    mobaninfo.Name = ml.Groups[3].ToString().Trim();
                                    form.AddMessage("网址:" + mobaninfo.Url, MainForm.richTextBoxC);
                                }
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase);
                                MatchCollection mcpic = rxpic.Matches(m.ToString());
                                foreach (Match ml in mcpic)
                                {
                                    mobaninfo.MPic = ml.Groups[1].ToString();
                                    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                    break;
                                }
                                
                                mobaninfo.BPic = mobaninfo.MPic;
                                //Regex rxtype = new Regex("<span class=\"meta-categories\">in(?<type>(.*?))</span>", RegexOptions.IgnoreCase);
                                //MatchCollection mcrxtype = rxtype.Matches(m.ToString());
                                //foreach (Match ml in mcrxtype)
                                //{
                                //    mobaninfo.Type = ml.Groups["type"].Value.Trim();
                                //    form.AddMessage("类型:" + mobaninfo.Type, MainForm.richTextBoxC);
                                //    break;
                                //}
                                mobaninfo.Type = "";
                                //pageBody = HttpHelper.HttpGET(mobaninfo.Url, "http://themeforest.net/", 2, ref statusCode, 30, "", false);
                                //if (statusCode == HttpStatusCode.OK)
                                //{
                                //    mobaninfo.Body = pageBody;
                                //    Regex rxid = new Regex("<a href=\"(.*?)\" role=\"button\" class=\"btn btn-icon live-preview\"", RegexOptions.IgnoreCase);
                                //    MatchCollection mcrxid = rxid.Matches(pageBody);
                                //    foreach (Match ml in mcrxid)
                                //    {
                                //        mobaninfo.Demo = "http://themeforest.net" + ml.Groups[1].Value.Trim();
                                //        form.AddMessage("Demo:" + mobaninfo.Demo, MainForm.richTextBoxC);
                                //        break;
                                //    }
                                //    mobaninfo.IsDownLoad = 1;
                                //}

                                if (!bll.Exists(mobaninfo.Url))
                                {
                                    if (bll.Add(mobaninfo) == 0)
                                    {
                                        form.AddMessage("保存失败", MainForm.richTextBoxC);
                                    }
                                    else
                                    {
                                        form.AddMessage("保存成功", MainForm.richTextBoxC);
                                    }
                                }
                                else
                                {
                                    form.AddMessage("已经存在", MainForm.richTextBoxC);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxC);
                    }

                    System.Threading.Thread.Sleep(3 * 1000);

                }

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }

        /// <summary>
        /// css-free-templates.com
        /// </summary>
        public void Freecssfiles()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://css-free-templates.com/page/";

                string pageBody = string.Empty;
                string pattern = "<div class=\"post\">(.*?)<div class=\"box-gradient\"";
                string linkPattern = "<h2><a href=\"(.*?)\"(.*?)>(.*?)</a></h2>";
                MobanDAL bll = new MobanDAL();
                for (int j = iStart; j < iEnd; j++)
                {
                    try
                    {
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + page + j, MainForm.richTextBoxC);
                        string pageurl = page + j + "/";
                        if (j == 1)
                        {
                            pageurl = "http://css-free-templates.com/";
                        }
                        pageBody = HttpHelper.HttpGET(pageurl, "http://css-free-templates.com/", 2, ref statusCode, 30, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(5 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);

                            //获取列表item
                            foreach (Match m in mc)
                            {
                                MobanInfo mobaninfo = new Moban.Model.MobanInfo();
                                mobaninfo.CrawlTime = DateTime.Now;
                                mobaninfo.CreateTime = DateTime.Now;
                                mobaninfo.Host = "css-free-templates.com";

                                Regex rxlink = new Regex(linkPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mclink = rxlink.Matches(m.ToString());
                                foreach (Match ml in mclink)
                                {
                                    mobaninfo.Url = ml.Groups[1].ToString().Trim();

                                    mobaninfo.Name = ml.Groups[3].ToString().Trim();
                                    form.AddMessage("网址:" + mobaninfo.Url, MainForm.richTextBoxC);
                                }
                                Regex rxpic = new Regex("src=\"(.*?)\"", RegexOptions.IgnoreCase);
                                MatchCollection mcpic = rxpic.Matches(m.ToString());
                                foreach (Match ml in mcpic)
                                {
                                    mobaninfo.MPic = ml.Groups[1].ToString();
                                    //form.AddMessage("缩略图:" + mobaninfo.MPic, MainForm.richTextBoxC);
                                    break;
                                }

                                mobaninfo.BPic = mobaninfo.MPic;
                                //Regex rxtype = new Regex("<span class=\"meta-categories\">in(?<type>(.*?))</span>", RegexOptions.IgnoreCase);
                                //MatchCollection mcrxtype = rxtype.Matches(m.ToString());
                                //foreach (Match ml in mcrxtype)
                                //{
                                //    mobaninfo.Type = ml.Groups["type"].Value.Trim();
                                //    form.AddMessage("类型:" + mobaninfo.Type, MainForm.richTextBoxC);
                                //    break;
                                //}
                                mobaninfo.Type = "";
                                //pageBody = HttpHelper.HttpGET(mobaninfo.Url, "http://themeforest.net/", 2, ref statusCode, 30, "", false);
                                //if (statusCode == HttpStatusCode.OK)
                                //{
                                //    mobaninfo.Body = pageBody;
                                //    Regex rxid = new Regex("<a href=\"(.*?)\" role=\"button\" class=\"btn btn-icon live-preview\"", RegexOptions.IgnoreCase);
                                //    MatchCollection mcrxid = rxid.Matches(pageBody);
                                //    foreach (Match ml in mcrxid)
                                //    {
                                //        mobaninfo.Demo = "http://themeforest.net" + ml.Groups[1].Value.Trim();
                                //        form.AddMessage("Demo:" + mobaninfo.Demo, MainForm.richTextBoxC);
                                //        break;
                                //    }
                                //    mobaninfo.IsDownLoad = 1;
                                //}

                                if (!bll.Exists(mobaninfo.Url))
                                {
                                    if (bll.Add(mobaninfo) == 0)
                                    {
                                        form.AddMessage("保存失败", MainForm.richTextBoxC);
                                    }
                                    else
                                    {
                                        form.AddMessage("保存成功", MainForm.richTextBoxC);
                                    }
                                }
                                else
                                {
                                    form.AddMessage("已经存在", MainForm.richTextBoxC);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxC);
                    }

                    System.Threading.Thread.Sleep(3 * 1000);

                }

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }


        public void YueMei()
        {

            form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
            DateTime b1 = DateTime.Now;
            string page = "http://www.yuemei.com/parts.html";
            string pageBody = string.Empty;
            string pattern = "<li><a href=\"http://www.yuemei.com/(\\w{1,30})/\" target=\"_blank\">(\\w{1,20})</a></li>";
            string linkPattern = "data-detail_page=\"(.*?)\"";
            MobanDAL bll = new MobanDAL();

            try
            {
                CookieCollection cookieCollection = new CookieCollection();
                form.AddMessage(DateTime.Now + "内容：" + page, MainForm.richTextBoxC);
                pageBody = HttpHelper.HttpGET(page, "http://www.yuemei.com/", 2, ref cookieCollection, 30, "", false);
                form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                if (!string.IsNullOrEmpty(pageBody))
                {
                    Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection mc = rx.Matches(pageBody);

                    //获取列表item
                    foreach (Match m in mc)
                    {
                        try
                        {


                            MobanInfo mobaninfo = new Moban.Model.MobanInfo();
                            mobaninfo.CrawlTime = DateTime.Now;
                            mobaninfo.CreateTime = DateTime.Now;
                            mobaninfo.Host = "yuemei.com";

                            mobaninfo.Url = "http://www.yuemei.com/" + m.Groups[1].ToString().Trim() + "/" + "baike.html";
                            mobaninfo.Name = m.Groups[2].ToString().Trim();

                            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                            string baikebody = HttpHelper.HttpGET(mobaninfo.Url, "http://www.yuemei.com/", 2, ref statusCode, 30, "", false);
                            if (statusCode != HttpStatusCode.OK)
                            {
                                System.Threading.Thread.Sleep(5 * 1000);
                                continue;
                            }
                            Regex rxbody = new Regex("<div class=\"review_left_new\">(.*?)<div class=\"review_right_new\">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcbody = rxbody.Matches(baikebody);
                            foreach (Match ml in mcbody)
                            {
                                mobaninfo.Body = "<div class=\"review_left_new\">" + ml.Groups[1].ToString();
                                break;
                            }

                            Hashtable imgHash = new Hashtable();
                            Regex rximglink = new Regex(" src=\"(?<mpic>(.*?))\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcimglink = rximglink.Matches(mobaninfo.Body);
                            foreach (Match ml in mcimglink)
                            {
                                string picUrl =  ml.Groups["mpic"].ToString();
                                
                                if (picUrl.StartsWith("/"))
                                {
                                    picUrl = "http://www.yuemei.com/" + picUrl;
                                }

                                Uri uri = new Uri(picUrl);
                                string ImgName = uri.Segments[uri.Segments.Length - 1].ToString();
                                string fileFolder = "E:\\Moban\\yuemei\\Images\\" + uri.Host + "\\" + uri.AbsolutePath.Replace(ImgName, "").Replace("/", "\\");

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
 

                            if (!bll.PartsExists(mobaninfo.Url))
                            {
                                if (bll.PartsAdd(mobaninfo) == 0)
                                {
                                    form.AddMessage("保存失败", MainForm.richTextBoxC);
                                }
                                else
                                {
                                    form.AddMessage("保存成功", MainForm.richTextBoxC);
                                }
                            }
                            else
                            {
                                form.AddMessage("已经存在", MainForm.richTextBoxC);
                            }
                        }
                        catch (Exception ex)
                        {
                            form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Message, MainForm.richTextBoxLog);
            }

            form.AddMessage("完成", MainForm.richTextBoxC);
        }
        

        public void DownLoadPage()
        {
            try
            {
                MobanDAL bll = new MobanDAL();
                DataSet ds = bll.GetNeedDownBodyList();
                DataTable dt = ds.Tables[0];
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                string url = string.Empty;
                string pageBody = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //System.Threading.Thread.Sleep(2*1000);
                    try
                    {
                        url = dt.Rows[i]["url"].ToString();
                        if (string.IsNullOrEmpty(url))
                        {
                            continue;
                        }
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage("内容：" + url, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(url, "http://www.templatemonster.com/", 2, ref statusCode, 30, "", false);
                        form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            form.AddMessage(url + statusCode, MainForm.richTextBoxLog);
                            System.Threading.Thread.Sleep(2 * 1000);
                            continue;
                        }

                        MobanInfo model = new MobanInfo();
                        model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                        model.Body = pageBody;
                        model.CrawlTime = DateTime.Now;
                        model.IsDownLoad = 1;
                        if (new MobanDAL().DownLoadPage(model))
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

            form.AddMessage("\n完成！", MainForm.richTextBoxC);

        }


        public void DownLoadScreenshotsPage()
        {
            bool IsStop = true;
            while (IsStop)
            {
                try
                {
                    Hashtable imgHash = new Hashtable();
                    MBPicDAL picdal = new MBPicDAL();
                    ScreenshotsDAL bll = new ScreenshotsDAL();
                    DataSet ds = bll.GetNeedDownScreenshotsList();
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        form.AddMessage(DateTime.Now + "...............全部完成.............", MainForm.richTextBoxC);
                        IsStop = false;
                        return;
                    }
                    form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                    string url = string.Empty;
                    string pageBody = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        System.Threading.Thread.Sleep(1 * 1000);
                        try
                        {
                            url = dt.Rows[i]["Screenshots"].ToString();
                            if (string.IsNullOrEmpty(url))
                            {
                                continue;
                            }
                            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                            form.AddMessage("内容：" + url, MainForm.richTextBoxC);
                            pageBody = HttpHelper.HttpGET(url.ToLower(), "http://themeforest.net/", 2, ref statusCode, 30, "Mozilla/5.0 (Windows NT 6.2; rv:23.0) Gecko/20100101 Firefox/23.0", false);
                            form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                            if (statusCode != HttpStatusCode.OK)
                            {
                                if (statusCode == HttpStatusCode.InternalServerError)
                                {
                                    MobanInfo mbinfo = new MobanInfo();
                                    mbinfo.Id = int.Parse(dt.Rows[i]["id"].ToString());
                                    mbinfo.Screenshots = "";
                                    new MobanDAL().UpdateScreenshots(mbinfo);
                                }
                                continue;
                            }

                            Regex rxlink = new Regex("class=\"thumbnails\">(?<thumbnails>(.*?))class=\"comments\">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mclink = rxlink.Matches(pageBody);
                            foreach (Match ml in mclink)
                            {
                                Regex rximglink = new Regex("src=\"(?<img>(.*?))\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcimglink = rximglink.Matches(ml.Groups["thumbnails"].ToString());
                                foreach (Match m in mcimglink)
                                {
                                    try
                                    {
                                        imgHash.Add(m.Groups["img"].ToString(), "");
                                        imgHash.Add(m.Groups["img"].ToString().Replace(".__thumbnail", ""), "");
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }
                                }
                            }

                            ScreenshotsInfo model = new ScreenshotsInfo();
                            model.MId = int.Parse(dt.Rows[i]["id"].ToString());
                            model.Body = pageBody;
                            model.IsA = 1;
                            if (bll.Add(model) > 0)
                            {
                                form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                            }
                            else
                            {
                                form.AddMessage("保存失败", MainForm.richTextBoxC);
                            }


                            foreach (DictionaryEntry item in imgHash)
                            {
                                string picurl = item.Key.ToString();
                                if (!picurl.StartsWith("http"))
                                {
                                    continue;
                                }

                                if (!picdal.Exists(picurl))
                                {
                                    picdal.Add(picurl);
                                }

                                if (!bll.ExistsPic(model.MId, picurl))
                                {
                                    int type = 1;
                                    if (picurl.IndexOf("__thumbnail") > -1)
                                    {
                                        type = 0;
                                    }
                                    bll.AddPic(model.MId, picurl, type);
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
                    System.Threading.Thread.Sleep(5 * 1000);
                }
                form.AddMessage(DateTime.Now + "全部完成！", MainForm.richTextBoxC);
            }
        }


        public void GetDemoUrl()
        {
            bool IsStop = true;
            while (IsStop)
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    DataSet ds = bll.GetNoDemoUrlList("templatemonster.com");
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        form.AddMessage(DateTime.Now + "...............全部完成.............", MainForm.richTextBoxC);
                        IsStop = false;
                        return;
                    }

                    form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                    string url = string.Empty;
                    string pageBody = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //System.Threading.Thread.Sleep(1*1000);
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            url = dt.Rows[i]["demo"].ToString();
                            if (string.IsNullOrEmpty(url))
                            {
                                continue;
                            }
                            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                            form.AddMessage("内容：" + url, MainForm.richTextBoxC);
                            pageBody = HttpHelper.HttpGET(url, "http://www.templatemonster.com/", 2, ref statusCode, 30, "", false);
                            form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                            if (statusCode != HttpStatusCode.OK)
                            {
                                model.DemoUrl = "404";
                                new MobanDAL().UpdateDemoUrl(model);
                                continue;
                            }


                            Regex rxlink = new Regex("href=\"(?<DemoUrl>(.*?))\">Preview without Frame</a>", RegexOptions.IgnoreCase);
                            MatchCollection mclink = rxlink.Matches(pageBody);
                            foreach (Match ml in mclink)
                            {
                                model.DemoUrl = new Uri(ml.Groups["DemoUrl"].ToString()).ToString();
                                form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }


                            //model.DemoUrl = DemoUrl;
                            //model.CrawlTime = DateTime.Now;
                            //model.IsDownLoad = 1;
                            if (new MobanDAL().UpdateDemoUrl(model))
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


        public void GetThemeforestDemoUrl()
        {
            
            bool IsStop = true;
            while (IsStop)
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    DataSet ds = bll.GetNoDemoUrlList("Themeforest.net");
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        form.AddMessage(DateTime.Now + "...............全部完成.............", MainForm.richTextBoxC);
                        IsStop = false;
                        return;
                    }
                    form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                    string url = string.Empty;
                    string pageBody = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        System.Threading.Thread.Sleep(1*1000);
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            url = dt.Rows[i]["demo"].ToString();
                            if (string.IsNullOrEmpty(url))
                            {
                                continue;
                            }
                            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                            //form.AddMessage("内容：" + url, MainForm.richTextBoxC);
                            pageBody = HttpHelper.HttpGET(url, url, 2, ref statusCode, 30, "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.57 Safari/537.36", false);
                            //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                            if (statusCode != HttpStatusCode.OK)
                            {
                                form.AddMessage("网址:404", MainForm.richTextBoxLog);
                                model.DemoUrl = "404";
                                new MobanDAL().UpdateDemoUrl(model);
                                continue;
                            }


                            Regex rxlink = new Regex("class=\"close\" href=\"(?<DemoUrl>(.*?))\">Remove Frame</a>", RegexOptions.IgnoreCase);
                            MatchCollection mclink = rxlink.Matches(pageBody);
                            foreach (Match ml in mclink)
                            {
                                model.DemoUrl = ml.Groups["DemoUrl"].ToString();
                                form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                            }

                            if (new MobanDAL().UpdateDemoUrl(model))
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
                            System.Threading.Thread.Sleep(1 * 1000);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                    System.Threading.Thread.Sleep(5 * 1000);
                }
            }
        }


        public void GetDemo()
        {
            try
            {
                MobanDAL bll = new MobanDAL();
                DataSet ds = bll.GetNoDemoUrlList();
                DataTable dt = ds.Tables[0];
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                string url = string.Empty;
                string pageBody = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //System.Threading.Thread.Sleep(1*1000);
                    try
                    {
                        MobanInfo model = new MobanInfo();
                        model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                        url = dt.Rows[i]["demo"].ToString();
                        if (string.IsNullOrEmpty(url))
                        {
                            continue;
                        }
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage("内容：" + url, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(url, "http://www.templatemonster.com/", 2, ref statusCode, 30, "", false);
                        form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            model.DemoUrl = "404";
                            new MobanDAL().UpdateDemoUrl(model);
                            continue;
                        }


                        Regex rxlink = new Regex("<a href=\"(?<DemoUrl>(.*?))\">Preview without Frame</a>", RegexOptions.IgnoreCase);
                        MatchCollection mclink = rxlink.Matches(pageBody);
                        foreach (Match ml in mclink)
                        {
                            model.DemoUrl = ml.Groups["DemoUrl"].ToString();
                            form.AddMessage("网址:" + model.DemoUrl, MainForm.richTextBoxC);
                        }


                        //model.DemoUrl = DemoUrl;
                        //model.CrawlTime = DateTime.Now;
                        //model.IsDownLoad = 1;
                        if (new MobanDAL().UpdateDemoUrl(model))
                        {
                            form.AddMessage("ID:" + model.Id + "保存成功", MainForm.richTextBoxC);
                        }
                        else
                        {
                            form.AddMessage("保存失败", MainForm.richTextBoxC);
                            System.Threading.Thread.Sleep(1 * 1000);
                        }

                    }
                    catch (Exception ex)
                    {

                        form.AddMessage(ex.Message, MainForm.richTextBoxC);
                        System.Threading.Thread.Sleep(1 * 1000);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(5 * 1000);
            }
        }

        public void GetThemeforestDemo()
        {
            bool IsStop = true;
            while (IsStop)
            {
                try
                {
                    MobanDAL bll = new MobanDAL();
                    DataSet ds = bll.GetNoDemoList("Themeforest.net");
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        form.AddMessage(DateTime.Now + "...............全部完成.............", MainForm.richTextBoxC);
                        IsStop = false;
                        return;
                    }
                    form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                    string pageBody = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //System.Threading.Thread.Sleep(1*1000);
                        try
                        {
                            MobanInfo model = new MobanInfo();
                            model.Id = int.Parse(dt.Rows[i]["id"].ToString());
                            pageBody = dt.Rows[i]["body"].ToString();

                            if (string.IsNullOrEmpty(pageBody))
                            {
                                form.AddMessage(model.Id + "内容为空.", MainForm.richTextBoxLog);
                                System.Threading.Thread.Sleep(1 * 1000);
                                continue;
                            }

                            //Regex rxid = new Regex("class=\"btn btn-icon live-preview\"", RegexOptions.IgnoreCase);
                            
                            Regex rxid = new Regex("<a href=\"/item/(.*?)\" role=\"button\" class=\"btn btn-icon live-preview\"", RegexOptions.IgnoreCase);
                            MatchCollection mcrxid = rxid.Matches(pageBody);
                            foreach (Match ml in mcrxid)
                            {
                                model.Demo = "http://themeforest.net" + ml.Groups[1].Value.Trim();
                                form.AddMessage("Demo:" + model.Demo, MainForm.richTextBoxC);
                                break;
                            }
                            if (string.IsNullOrEmpty(model.Demo))
                            {
                                model.Demo = "404";
                            }

                            if (new MobanDAL().UpdateDemo(model))
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
                            System.Threading.Thread.Sleep(1 * 1000);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddMessage(DateTime.Now + ex.Message, MainForm.richTextBoxC);
                    System.Threading.Thread.Sleep(5 * 1000);
                }
            }
        }


        public void FirstPPT()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://www.1ppt.com/article/";

                string pageBody = string.Empty;
                string pattern = "data-item-id=(.*?)<small class=\"price";
                string linkPattern = "<a href=\"http://themeforest.net/item/(.*?)\\?";
                PPTDAL bll = new PPTDAL();
                for (int j = iStart; j < iEnd; j++)
                {
                    try
                    {
                        string pageurl = page + j + ".html";//ppt页面地址                     
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + pageurl, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(pageurl, "http://www.1ppt.com/", 1, ref statusCode, 20, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(5 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {

                            PPTInfo ppt = new PPTInfo();

                            ppt.Host = "www.1ppt.com";
                            ppt.Url = pageurl;
                            ppt.Body = pageBody;

                            Regex rxpic = new Regex("<div class=\"content\">.*?<img.*?src=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mcpic = rxpic.Matches(pageBody);
                            foreach (Match ml in mcpic)
                            {
                                string pic = ml.Groups[1].ToString();
                                if (pic.StartsWith("/") && pic.EndsWith(".jpg"))
                                {
                                    ppt.BPic = "http://www.1ppt.com" + pic;
                                    ppt.MPic = ppt.BPic.Replace(".jpg", "-lp.jpg");
                                }
                                break;
                            }

                            ImageDownLoad.DownLoadImg(ppt.BPic, "E:\\Moban\\Images\\");
                            ImageDownLoad.DownLoadImg(ppt.MPic, "E:\\Moban\\Images\\");

                            Regex rxtit = new Regex("<h1>(.*?)</h1>", RegexOptions.IgnoreCase);
                            MatchCollection mcrxtit = rxtit.Matches(pageBody);
                            foreach (Match ml in mcrxtit)
                            {
                                ppt.Title = ml.Groups[1].Value.Trim();
                                break;
                            }

                            Regex rxtype = new Regex("<div class=\"content\">(.*?)<div class=\"bdshare", RegexOptions.IgnoreCase);
                            MatchCollection mcrxtype = rxtype.Matches(pageBody);
                            foreach (Match ml in mcrxtype)
                            {
                                ppt.Jieshao = StringHelper.NoHTML(ml.Groups[1].Value.Trim());
                                break;
                            }

                            Regex rxtags = new Regex("<li>标签：(.*?)</li>", RegexOptions.IgnoreCase);
                            MatchCollection mcrxtags = rxtags.Matches(pageBody);
                            foreach (Match ml in mcrxtags)
                            {
                                string tags = ml.Groups[1].Value.Trim();

                                Regex rxtag = new Regex("<a href=\".*?\" target=\"_blank\">(.*?)</a>", RegexOptions.IgnoreCase);
                                MatchCollection mcrxtag = rxtag.Matches(tags);
                                foreach (Match m in mcrxtag)
                                {
                                    ppt.Tag += m.Groups[1].ToString() + ",";
                                }
                                ppt.Tag = ppt.Tag.Substring(0, ppt.Tag.Length-1);
                                break;
                            }

                            Regex rxid = new Regex("<li><a href=\"(.*?)\" target=\"_blank\" class=\"green\"", RegexOptions.IgnoreCase);
                            MatchCollection mcrxid = rxid.Matches(pageBody);
                            foreach (Match ml in mcrxid)
                            {
                                ppt.DownLink = ml.Groups[1].Value.Trim();
                                form.AddMessage("DownLink:" + ppt.DownLink, MainForm.richTextBoxC);
                                break;
                            }
                            if (!bll.Exists(ppt.Url))
                            {
                                if (bll.Add(ppt) == 0)
                                {
                                    form.AddMessage("保存失败", MainForm.richTextBoxC);
                                }
                                else
                                {
                                    form.AddMessage("保存成功", MainForm.richTextBoxC);
                                }
                            }
                            else
                            {
                                form.AddMessage("已经存在", MainForm.richTextBoxC);
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxC);
                    }

                    System.Threading.Thread.Sleep(3 * 1000);

                }

            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }

        public void FivePPT()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://www.51ppt.com.cn/SiteMap/Soft";

                string pageBody = string.Empty;
                string pattern = "<li><a href='(.*?)'>(.*?)</a>";
                string linkPattern = "<a href=\"http://themeforest.net/item/(.*?)\\?";
                PPTDAL bll = new PPTDAL();
                for (int j = iStart; j < iEnd; j++)
                {

                    try
                    {
                        string pageurl = page + j + ".htm";//ppt页面地址                     
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + pageurl, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(pageurl, "http://www.51ppt.com.cn/", 1, ref statusCode, 10, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(2 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);
                            //获取列表item
                            foreach (Match mm in mc)
                            {
                                PPTInfo ppt = new PPTInfo();
                                ppt.Host = "www.51ppt.com.cn";
                                ppt.Url = mm.Groups[1].ToString();
                                ppt.Title = mm.Groups[2].ToString();

                                if (bll.Exists(ppt.Url))
                                {
                                    form.AddMessage(j + "--已经存在", MainForm.richTextBoxC);
                                    continue;
                                }


                                //开始获取页面
                                HttpStatusCode pstatusCode = HttpStatusCode.InternalServerError;
                                form.AddMessage(DateTime.Now + "内容：" + ppt.Url, MainForm.richTextBoxC);
                                string pptpageBody = HttpHelper.HttpGET(ppt.Url, "http://www.51ppt.com.cn/", 1, ref pstatusCode, 10, "", false);
                                if (pstatusCode != HttpStatusCode.OK)
                                {
                                    System.Threading.Thread.Sleep(2 * 1000);
                                    continue;
                                }

                                Regex rxpic = new Regex("<img class='pic2' src='(.*?)'  width='300'", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection mcpic = rxpic.Matches(pptpageBody);
                                foreach (Match ml in mcpic)
                                {
                                    string pic = ml.Groups[1].ToString();
                                    if (pic.StartsWith("/"))
                                    {
                                        ppt.BPic = "http://www.51ppt.com.cn" + pic;
                                        ppt.MPic = ppt.BPic;
                                    }
                                    break;
                                }
                                if (string.IsNullOrEmpty(ppt.BPic))
                                {
                                    ppt.BPic = "http://www.51ppt.com.cn/images/nopic.gif";
                                    ppt.MPic = ppt.BPic;
                                }
                                ImageDownLoad.DownLoadImg(ppt.BPic, "E:\\Moban\\Images\\");
                                ppt.Jieshao = "";
                                ppt.Tag = "";
                                ppt.Body = pptpageBody;

                                

                                Regex rxid = new Regex("<LI><a href='/Soft/ShowSoftDown\\.asp\\?UrlID=1&SoftID=(.*?)' target='_blank'>本地下载1", RegexOptions.IgnoreCase);
                                MatchCollection mcrxid = rxid.Matches(pptpageBody);
                                foreach (Match ml in mcrxid)
                                {
                                    ppt.DownLink = "http://www.51ppt.com.cn/Soft/ShowSoftDown.asp?UrlID=1&SoftID=" + ml.Groups[1].Value.Trim();
                                    break;
                                }
                                if (string.IsNullOrEmpty(ppt.DownLink))
                                {
                                    ppt.DownLink = "http://www.51ppt.com.cn/othertemplates/ShowSoftDown.asp?UrlID=1&SoftID=" + StringHelper.GetRegValue(pptpageBody, "<LI><a href='/othertemplates/ShowSoftDown\\.asp\\?UrlID=1&SoftID=(.*?)' target='_blank'>下载地址1");
                                }

                                string dpageBody = HttpHelper.HttpGET(ppt.DownLink, "http://www.51ppt.com.cn/", 1, ref pstatusCode, 10, "", false);

                                ppt.DownLink = StringHelper.GetRegValue(dpageBody, "b><A href=\"(.*?)\">点击下载");


                                try
                                {
                                    if (bll.Add(ppt) == 0)
                                    {
                                        form.AddMessage("保存失败", MainForm.richTextBoxC);
                                    }
                                    else
                                    {
                                        form.AddMessage("保存成功", MainForm.richTextBoxC);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    form.AddMessage(j + " -  " + ex.Message, MainForm.richTextBoxLog);
                                    continue;
                                }
                                


                            }
                        }
                    }
                    catch (Exception ex)
                    {                       
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                        

                    }
                }
                System.Threading.Thread.Sleep(3 * 1000);



            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }


        public void ChinazPPT()
        {

            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);

                DateTime b1 = DateTime.Now;
                string page = "http://sc.chinaz.com/ppt/index_";

                string pageBody = string.Empty;
                string pattern = "<div class=\"box picblock col3\"(.*?)</a></p>";
                PPTDAL bll = new PPTDAL();
                for (int j = iStart; j < iEnd; j++)
                {

                    try
                    {
                        string pageurl = page + j + ".html";//ppt页面地址  
                        if (j==1)
                        {
                            pageurl = "http://sc.chinaz.com/ppt/";
                        }
                        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                        form.AddMessage(DateTime.Now + "内容：" + pageurl, MainForm.richTextBoxC);
                        pageBody = HttpHelper.HttpGET(pageurl, "http://sc.chinaz.com/", 1, ref statusCode, 10, "", false);
                        if (statusCode != HttpStatusCode.OK)
                        {
                            System.Threading.Thread.Sleep(2 * 1000);
                            continue;
                        }

                        //form.AddMessage(DateTime.Now + "内容获取完成", MainForm.richTextBoxC);
                        if (!string.IsNullOrEmpty(pageBody))
                        {
                            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection mc = rx.Matches(pageBody);
                            //获取列表item
                            foreach (Match mm in mc)
                            {
                                PPTInfo ppt = new PPTInfo();
                                ppt.Host = "sc.chinaz.com";
                                ppt.Url = "http://sc.chinaz.com/" + StringHelper.GetRegValue(mm.Groups[1].ToString(), "href=\"(.*?)\"");
                                ppt.Title = StringHelper.GetRegValue(mm.Groups[1].ToString(), "alt=\"(.*?)\"");

                                if (bll.Exists(ppt.Url))
                                {
                                    form.AddMessage(j + "--已经存在", MainForm.richTextBoxC);
                                    continue;
                                }

                                ppt.MPic = StringHelper.GetRegValue(mm.Groups[1].ToString(), "src2=\"(.*?)\"");
                                ppt.BPic = ppt.MPic.Replace("_s", "");
                                ImageDownLoad.DownLoadImg(ppt.BPic, "E:\\Moban\\Images\\");
                                ImageDownLoad.DownLoadImg(ppt.MPic, "E:\\Moban\\Images\\");

                                //开始获取页面
                                HttpStatusCode pstatusCode = HttpStatusCode.InternalServerError;
                                form.AddMessage(ppt.Url, MainForm.richTextBoxC);
                                string pptpageBody = HttpHelper.HttpGET(ppt.Url, "http://sc.chinaz.com/", 1, ref pstatusCode, 10, "", false);
                                if (pstatusCode != HttpStatusCode.OK)
                                {
                                    System.Threading.Thread.Sleep(2 * 1000);
                                    continue;
                                }

                                ppt.Jieshao = StringHelper.GetRegValue(pptpageBody, "PT简介</span><div class=\"smr\">(.*?)<p><b>本作品");
                                string tags = StringHelper.GetRegValue(pptpageBody, "div class=\"smr\"><a hr(.*?)</a></div>");

                                ppt.Tag = StringHelper.GetRegValues(tags,"t=\"_blank\">(.*?)</a>");
                                ppt.Body = pptpageBody;
                                ppt.DownLink = "http://bjlt.sc.chinaz.com/" + StringHelper.GetRegValues(pptpageBody, "href='http://bjlt\\.sc\\.chinaz\\.com/(.*?)'");
                               
                                try
                                {
                                    if (bll.Add(ppt) == 0)
                                    {
                                        form.AddMessage("保存失败", MainForm.richTextBoxC);
                                    }
                                    else
                                    {
                                        form.AddMessage("保存成功", MainForm.richTextBoxC);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    form.AddMessage(j + " -  " + ex.Message, MainForm.richTextBoxLog);
                                    continue;
                                }



                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);


                    }
                }
                System.Threading.Thread.Sleep(3 * 1000);



            }
            catch (Exception ex)
            {
                form.AddMessage(ex.Source, MainForm.richTextBoxC);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            form.AddMessage("\n\n完成", MainForm.richTextBoxC);

        }


    }
}
