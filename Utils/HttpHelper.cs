using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Moban.Utils
{
    public class HttpHelper
    {
        public static CookieCollection getCookieOnStr(string url, string str)
        {
            CookieCollection cookies = new CookieCollection();
            ArrayList list = smethod_7(url, str);
            for (int i = 0; i < list.Count; i++)
            {
                Cookie cookie = (Cookie)list[i];
                cookies.Add(cookie);
            }
            return cookies;
        }

        public static Image GetImage(string url, string refrere, ref CookieCollection cookie, int timeout)
        {
            return HttpGetImage(url, refrere, ref cookie, timeout);
        }

        public static string Http(string url, string postdata, string refrere, bool ispost, int encoding, bool sendmode, ref CookieCollection cookie, int timeout)
        {
            if (ispost)
            {
                return HttpPOST(url, postdata, refrere, encoding != 1, sendmode, ref cookie, timeout, "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.2 Safari/537.4", true, true);
            }
            return HttpGET(url, refrere, encoding, ref cookie, timeout, "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.2 Safari/537.4", true);
        }

        public static string HttpGET(string Url, string refrere, int encoding, ref CookieCollection cookie, int timeout, string useragent, bool isRedirect)
        {
            Stream responseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            string message;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 0x3e8;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.smethod_0);
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.Headers.Clear();
                request.CookieContainer = smethod_3(cookie, Url);
                request.Method = "GET";
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Referer = refrere;
                request.Timeout = timeout * 0x3e8;
                request.AllowAutoRedirect = false;
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers.Add("Accept-Language", "zh-cn");
                request.UserAgent = useragent;
                string str = request.Headers.ToString();
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                smethod_4(ref cookie, Url, response.Headers["Set-Cookie"], response.Cookies);
                string html = "";
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                Encoding encoding2 = Encoding.Default;


                if (encoding == 1)
                {
                    encoding2 = Encoding.Default;
                    html = new StreamReader(responseStream, encoding2).ReadToEnd();
                }
                else if (encoding == 2)
                {
                    encoding2 = Encoding.UTF8;
                    html = new StreamReader(responseStream, encoding2).ReadToEnd();
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {

                    #region 原始方法

                    //MemoryStream stream2 = new MemoryStream();
                    //int count = 0;
                    //byte[] buffer = new byte[0x200];
                    //while ((count = responseStream.Read(buffer, 0, 0x200)) > 0)
                    //{
                    //    stream2.Write(buffer, 0, count);
                    //}
                    //response.Close();
                    //if (stream2.Length > 0L)
                    //{
                    //    stream2.Seek(0L, SeekOrigin.Begin);
                    //    byte[] buffer2 = new byte[stream2.Length];
                    //    stream2.Read(buffer2, 0, buffer2.Length);
                    //    stream2.Seek(0L, SeekOrigin.Begin);
                    //    html = Encoding.UTF8.GetString(buffer2);
                    //    bool flag = false;
                    //    try
                    //    {
                    //        ArrayList list = getregex(html, "(?<=charset=).*?(?=\")");
                    //        if (list.Count > 0)
                    //        {
                    //            flag = true;
                    //        }
                    //        if ((list.Count > 0) && (encoding2 != Encoding.GetEncoding(list[0].ToString())))
                    //        {
                    //            encoding2 = Encoding.GetEncoding(list[0].ToString());
                    //        }
                    //    }
                    //    catch
                    //    {
                    //    }
                    //    if (!flag)
                    //    {
                    //        if (Regex.Matches(html, "[一-龥]").Count < 10)
                    //        {
                    //            html = Encoding.Default.GetString(buffer2);
                    //        }
                    //    }
                    //    else if (encoding2 != Encoding.UTF8)
                    //    {
                    //        html = encoding2.GetString(buffer2);
                    //    }
                    //}

                    #endregion

                    byte[] urlPageData = GetBytesFromStream(responseStream);
                    string Encodeing = string.Empty;
                    Encodeing = getEncoding(response.Headers.ToString()); //获取头部的
                    if (string.IsNullOrEmpty(Encodeing)) //头部有编码
                    {
                        string pageHtml = Encoding.Default.GetString(urlPageData);
                        Encodeing = getEncoding(pageHtml);
                        if (string.IsNullOrEmpty(Encodeing)) //内容里有编码
                        {
                            if (utf8_probability(urlPageData) > 80)
                            {
                                Encodeing = "UTF-8";
                            }
                            else
                            {
                                Encodeing = "GB2312";
                            }
                        }
                    }
                    html = Encoding.GetEncoding(Encodeing).GetString(urlPageData);
                }
                //html = string.Concat(new object[] { html, "\r\n\r\n=================================================\r\n\r\n本次请求：", Url, " 响应结果：", response.StatusCode, "\r\n\r\nCookie数量", request.CookieContainer.Count, "\r\n", request.CookieContainer.GetCookieHeader(new Uri(Url)), "\r\nrequest:\r\n", str, "\r\nresponse:\r\n", response.Headers.ToString(), "\r\n\r\n=================================================\r\n\r\n" });
                //html = string.Concat(new object[] { html, "" });

                if (isRedirect)
                {
                    if ((response.Headers["Location"] != null) && (response.Headers["Location"].Length > 2))
                    {
                        string url = "";
                        if (response.Headers["Location"].ToLower().Contains("http://"))
                        {
                            url = response.Headers["Location"];
                        }
                        else
                        {
                            string str4 = smethod_2(response.Headers["Location"], Url);
                            url = Url;
                            if (str4 != null)
                            {
                                url = str4;
                            }
                        }
                        if (url.ToLower() != Url.ToLower())
                        {
                            html = HttpGET(url, Url, encoding, ref cookie, timeout, useragent, isRedirect) + html;
                        }
                    }
                    else if ((response.Headers["Refresh"] != null) && (response.Headers["Refresh"].Length > 2))
                    {
                        try
                        {
                            string str5 = response.Headers["Refresh"].ToLower().Replace("url=", "`").Split(new char[] { '`' })[1];
                            if (!str5.Contains("http://"))
                            {
                                string str6 = smethod_2(str5, Url);
                                str5 = Url;
                                if (str6 != null)
                                {
                                    str5 = str6;
                                }
                            }
                            if (str5.ToLower() != Url.ToLower())
                            {
                                html = HttpGET(str5, Url, encoding, ref cookie, timeout, useragent, isRedirect) + html;
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("发现一个变态服务器。。" + exception.Message);
                        }
                    }
                }
                response.Close();
                request.Abort();
                message = html;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return message;
        }

        public static string HttpGET(string Url, string refrere, int encoding, ref HttpStatusCode statusCode, int timeout, string useragent, bool isRedirect)
        {
            Stream responseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            string message;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 0x3e8;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.smethod_0);
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.Headers.Clear();
                CookieCollection cookie = new CookieCollection();
                request.CookieContainer = smethod_3(cookie, Url);
                request.Method = "GET";
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Referer = refrere;
                request.Timeout = timeout * 1000;
                request.AllowAutoRedirect = false;
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers.Add("Accept-Language", "zh-cn");
                
                request.UserAgent = useragent;
                string str = request.Headers.ToString();
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                smethod_4(ref cookie, Url, response.Headers["Set-Cookie"], response.Cookies);
                statusCode = response.StatusCode;
                string html = "";
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                Encoding encoding2 = Encoding.Default;


                if (encoding == 1)
                {
                    encoding2 = Encoding.Default;
                    html = new StreamReader(responseStream, encoding2).ReadToEnd();
                }
                else if (encoding == 2)
                {
                    encoding2 = Encoding.UTF8;
                    html = new StreamReader(responseStream, encoding2).ReadToEnd();
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {

                    #region 原始方法

                    //MemoryStream stream2 = new MemoryStream();
                    //int count = 0;
                    //byte[] buffer = new byte[0x200];
                    //while ((count = responseStream.Read(buffer, 0, 0x200)) > 0)
                    //{
                    //    stream2.Write(buffer, 0, count);
                    //}
                    //response.Close();
                    //if (stream2.Length > 0L)
                    //{
                    //    stream2.Seek(0L, SeekOrigin.Begin);
                    //    byte[] buffer2 = new byte[stream2.Length];
                    //    stream2.Read(buffer2, 0, buffer2.Length);
                    //    stream2.Seek(0L, SeekOrigin.Begin);
                    //    html = Encoding.UTF8.GetString(buffer2);
                    //    bool flag = false;
                    //    try
                    //    {
                    //        ArrayList list = getregex(html, "(?<=charset=).*?(?=\")");
                    //        if (list.Count > 0)
                    //        {
                    //            flag = true;
                    //        }
                    //        if ((list.Count > 0) && (encoding2 != Encoding.GetEncoding(list[0].ToString())))
                    //        {
                    //            encoding2 = Encoding.GetEncoding(list[0].ToString());
                    //        }
                    //    }
                    //    catch
                    //    {
                    //    }
                    //    if (!flag)
                    //    {
                    //        if (Regex.Matches(html, "[一-龥]").Count < 10)
                    //        {
                    //            html = Encoding.Default.GetString(buffer2);
                    //        }
                    //    }
                    //    else if (encoding2 != Encoding.UTF8)
                    //    {
                    //        html = encoding2.GetString(buffer2);
                    //    }
                    //}

                    #endregion

                    byte[] urlPageData = GetBytesFromStream(responseStream);
                    string Encodeing = string.Empty;
                    Encodeing = getEncoding(response.Headers.ToString()); //获取头部的
                    if (string.IsNullOrEmpty(Encodeing)) //头部有编码
                    {
                        string pageHtml = Encoding.Default.GetString(urlPageData);
                        Encodeing = getEncoding(pageHtml);
                        if (string.IsNullOrEmpty(Encodeing)) //内容里有编码
                        {
                            if (utf8_probability(urlPageData) > 80)
                            {
                                Encodeing = "UTF-8";
                            }
                            else
                            {
                                Encodeing = "GB2312";
                            }
                        }
                    }
                    html = Encoding.GetEncoding(Encodeing).GetString(urlPageData);
                }
                //html = string.Concat(new object[] { html, "\r\n\r\n=================================================\r\n\r\n本次请求：", Url, " 响应结果：", response.StatusCode, "\r\n\r\nCookie数量", request.CookieContainer.Count, "\r\n", request.CookieContainer.GetCookieHeader(new Uri(Url)), "\r\nrequest:\r\n", str, "\r\nresponse:\r\n", response.Headers.ToString(), "\r\n\r\n=================================================\r\n\r\n" });
                //html = string.Concat(new object[] { html, "" });

                if (isRedirect)
                {
                    if ((response.Headers["Location"] != null) && (response.Headers["Location"].Length > 2))
                    {
                        string url = "";
                        if (response.Headers["Location"].ToLower().Contains("http://"))
                        {
                            url = response.Headers["Location"];
                        }
                        else
                        {
                            string str4 = smethod_2(response.Headers["Location"], Url);
                            url = Url;
                            if (str4 != null)
                            {
                                url = str4;
                            }
                        }
                        if (url.ToLower() != Url.ToLower())
                        {
                            html = HttpGET(url, Url, encoding, ref statusCode, timeout, useragent, isRedirect) + html;
                        }
                    }
                    else if ((response.Headers["Refresh"] != null) && (response.Headers["Refresh"].Length > 2))
                    {
                        try
                        {
                            string str5 = response.Headers["Refresh"].ToLower().Replace("url=", "`").Split(new char[] { '`' })[1];
                            if (!str5.Contains("http://"))
                            {
                                string str6 = smethod_2(str5, Url);
                                str5 = Url;
                                if (str6 != null)
                                {
                                    str5 = str6;
                                }
                            }
                            if (str5.ToLower() != Url.ToLower())
                            {
                                html = HttpGET(str5, Url, encoding, ref statusCode, timeout, useragent, isRedirect) + html;
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("发现一个变态服务器。。" + exception.Message);
                        }
                    }
                }
                response.Close();
                request.Abort();
                message = html;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return message;
        }


        public static string HttpGET302(string Url, string refrere, int encoding, ref HttpStatusCode statusCode, int timeout, string useragent, bool isRedirect)
        {
            Stream responseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            string message;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 0x3e8;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.smethod_0);
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.Headers.Clear();
                CookieCollection cookie = new CookieCollection();
                request.CookieContainer = smethod_3(cookie, Url);
                request.Method = "GET";
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Referer = refrere;
                request.Timeout = timeout * 1000;
                request.AllowAutoRedirect = false;
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers.Add("Accept-Language", "zh-cn");

                request.UserAgent = useragent;
                string str = request.Headers.ToString();
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                smethod_4(ref cookie, Url, response.Headers["Set-Cookie"], response.Cookies);
                statusCode = response.StatusCode;
                string html = "";
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                Encoding encoding2 = Encoding.Default;


                if (encoding == 1)
                {
                    encoding2 = Encoding.Default;
                    html = new StreamReader(responseStream, encoding2).ReadToEnd();
                }
                else if (encoding == 2)
                {
                    encoding2 = Encoding.UTF8;
                    html = new StreamReader(responseStream, encoding2).ReadToEnd();
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {

                    #region 原始方法

                    //MemoryStream stream2 = new MemoryStream();
                    //int count = 0;
                    //byte[] buffer = new byte[0x200];
                    //while ((count = responseStream.Read(buffer, 0, 0x200)) > 0)
                    //{
                    //    stream2.Write(buffer, 0, count);
                    //}
                    //response.Close();
                    //if (stream2.Length > 0L)
                    //{
                    //    stream2.Seek(0L, SeekOrigin.Begin);
                    //    byte[] buffer2 = new byte[stream2.Length];
                    //    stream2.Read(buffer2, 0, buffer2.Length);
                    //    stream2.Seek(0L, SeekOrigin.Begin);
                    //    html = Encoding.UTF8.GetString(buffer2);
                    //    bool flag = false;
                    //    try
                    //    {
                    //        ArrayList list = getregex(html, "(?<=charset=).*?(?=\")");
                    //        if (list.Count > 0)
                    //        {
                    //            flag = true;
                    //        }
                    //        if ((list.Count > 0) && (encoding2 != Encoding.GetEncoding(list[0].ToString())))
                    //        {
                    //            encoding2 = Encoding.GetEncoding(list[0].ToString());
                    //        }
                    //    }
                    //    catch
                    //    {
                    //    }
                    //    if (!flag)
                    //    {
                    //        if (Regex.Matches(html, "[一-龥]").Count < 10)
                    //        {
                    //            html = Encoding.Default.GetString(buffer2);
                    //        }
                    //    }
                    //    else if (encoding2 != Encoding.UTF8)
                    //    {
                    //        html = encoding2.GetString(buffer2);
                    //    }
                    //}

                    #endregion

                    byte[] urlPageData = GetBytesFromStream(responseStream);
                    string Encodeing = string.Empty;
                    Encodeing = getEncoding(response.Headers.ToString()); //获取头部的
                    if (string.IsNullOrEmpty(Encodeing)) //头部有编码
                    {
                        string pageHtml = Encoding.Default.GetString(urlPageData);
                        Encodeing = getEncoding(pageHtml);
                        if (string.IsNullOrEmpty(Encodeing)) //内容里有编码
                        {
                            if (utf8_probability(urlPageData) > 80)
                            {
                                Encodeing = "UTF-8";
                            }
                            else
                            {
                                Encodeing = "GB2312";
                            }
                        }
                    }
                    html = Encoding.GetEncoding(Encodeing).GetString(urlPageData);
                }
                //html = string.Concat(new object[] { html, "\r\n\r\n=================================================\r\n\r\n本次请求：", Url, " 响应结果：", response.StatusCode, "\r\n\r\nCookie数量", request.CookieContainer.Count, "\r\n", request.CookieContainer.GetCookieHeader(new Uri(Url)), "\r\nrequest:\r\n", str, "\r\nresponse:\r\n", response.Headers.ToString(), "\r\n\r\n=================================================\r\n\r\n" });
                //html = string.Concat(new object[] { html, "" });

                if (isRedirect)
                {
                    if ((response.Headers["Location"] != null) && (response.Headers["Location"].Length > 2))
                    {
                        string url = "";
                        if (response.Headers["Location"].ToLower().Contains("http://"))
                        {
                            url = response.Headers["Location"];
                        }
                        else
                        {
                            string str4 = smethod_2(response.Headers["Location"], Url);
                            url = Url;
                            if (str4 != null)
                            {
                                url = str4;
                            }
                        }

                        html = url;
                        
                    }
                    else if ((response.Headers["Refresh"] != null) && (response.Headers["Refresh"].Length > 2))
                    {
                        try
                        {
                            string str5 = response.Headers["Refresh"].ToLower().Replace("url=", "`").Split(new char[] { '`' })[1];
                            if (!str5.Contains("http://"))
                            {
                                string str6 = smethod_2(str5, Url);
                                str5 = Url;
                                if (str6 != null)
                                {
                                    str5 = str6;
                                }
                            }
                            if (str5.ToLower() != Url.ToLower())
                            {
                                html = HttpGET(str5, Url, encoding, ref statusCode, timeout, useragent, isRedirect) + html;
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("发现一个变态服务器。。" + exception.Message);
                        }
                    }
                }
                response.Close();
                request.Abort();
                message = html;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return message;
        }


        /// <summary>
        /// 从字符串中匹配查找编码信息
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string getEncoding(string html)
        {
            //Match match = Regex.Match(html, "charset=(?'ecncoding'.*[\\d|k|K])\"");
            Match match = Regex.Match(html, "(charset|encoding)=[',\"]?(?<RESULT>[0-9,a-zA-Z,-]{2,})", RegexOptions.IgnoreCase);
            GroupCollection groups = match.Groups;
            string ecncoding = groups["RESULT"].Value;
            return ecncoding.ToUpper();
        }

        private static byte[] GetBytesFromStream(Stream stream)
        {
            List<byte> arBuffer = new List<byte>();

            byte[] buffer = new byte[1024];
            int offset = 1024;
            int count = stream.Read(buffer, 0, offset);
            while (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    arBuffer.Add(buffer[i]);
                }
                count = stream.Read(buffer, 0, offset);
            }

            return (byte[])arBuffer.ToArray();
        }

        /// <summary>
        /// 判断是否是Utf-8编码
        /// </summary>
        /// <param name="rawtext"></param>
        /// <returns> >80  就是utf-8</returns>
        static int utf8_probability(byte[] rawtext)
        {
            try
            {


                int score = 0;
                int i, rawtextlen = 0;
                int goodbytes = 0, asciibytes = 0;
                // Maybe also use UTF8 Byte Order Mark:  EF BB BF
                // Check to see if characters fit into acceptable ranges
                rawtextlen = rawtext.Length;
                for (i = 0; i < rawtextlen; i++)
                {
                    if ((rawtext[i] & (byte)0x7F) == rawtext[i])
                    {  // One byte
                        asciibytes++;
                        // Ignore ASCII, can throw off count
                    }
                    else
                    {
                        int m_rawInt0 = Convert.ToInt16(rawtext[i]);
                        int m_rawInt1 = Convert.ToInt16(rawtext[i + 1]);
                        int m_rawInt2 = Convert.ToInt16(rawtext[i + 2]);

                        if (256 - 64 <= m_rawInt0 && m_rawInt0 <= 256 - 33 && // Two bytes
                         i + 1 < rawtextlen &&
                         256 - 128 <= m_rawInt1 && m_rawInt1 <= 256 - 65)
                        {
                            goodbytes += 2;
                            i++;
                        }
                        else if (256 - 32 <= m_rawInt0 && m_rawInt0 <= 256 - 17 && // Three bytes
                         i + 2 < rawtextlen &&
                         256 - 128 <= m_rawInt1 && m_rawInt1 <= 256 - 65 &&
                         256 - 128 <= m_rawInt2 && m_rawInt2 <= 256 - 65)
                        {
                            goodbytes += 3;
                            i += 2;
                        }
                    }
                }

                if (asciibytes == rawtextlen) { return 0; }
                score = (int)(100 * ((float)goodbytes / (float)(rawtextlen - asciibytes)));
                // If not above 98, reduce to zero to prevent coincidental matches
                // Allows for some (few) bad formed sequences
                if (score > 98)
                {
                    return score;
                }
                else if (score > 95 && goodbytes > 30)
                {
                    return score;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {

                return 0;
            }
        }

        public static Image HttpGetImage(string Url, string refrere, ref CookieCollection cookie, int timeout)
        {
            Image image = null;
            Stream responseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Image image2;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 0x3e8;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.smethod_0);
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.Proxy = WebRequest.GetSystemWebProxy();
                request.ServicePoint.Expect100Continue = false;
                request.Headers.Clear();
                request.CookieContainer = smethod_3(cookie, Url);
                request.Method = "GET";
                request.Referer = refrere;
                request.Timeout = timeout * 0x3e8;
                request.AllowAutoRedirect = false;
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers.Add("Accept-Language", "zh-cn");
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.2)";
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                smethod_4(ref cookie, Url, response.Headers["Set-Cookie"], response.Cookies);
                image = Image.FromStream(responseStream);
                request.Abort();
                image2 = image;
            }
            catch
            {
                image2 = null;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return image2;
        }

        public static string HttpPOST(string Url, string postDataStr, string refrere, bool isutf8, bool sendmode, ref CookieCollection cookie, int timeout, string useragent, bool isPostOnGzip, bool isRedirect)
        {
            Stream responseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            string message;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 0x3e8;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.smethod_0);
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(Url));
                request.ServicePoint.Expect100Continue = false;
                request.CookieContainer = smethod_3(cookie, Url);
                request.Method = "POST";
                if (isPostOnGzip)
                {
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                }
                if (sendmode)
                {
                    string newValue = "X" + DateTime.Now.Ticks;
                    request.ContentType = "multipart/form-data; boundary=---------------------------" + newValue;
                    postDataStr = postDataStr.Replace("7da38e26991192", newValue);
                }
                else
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                request.Referer = refrere;
                request.Timeout = timeout * 0x3e8;
                request.AllowAutoRedirect = false;
                request.Accept = "*/*";
                request.Headers.Add("Accept-Language", "zh-CN");
                request.Headers.Add("Cache-Control", "no-cache");
                request.UserAgent = useragent;
                byte[] buffer = isutf8 ? Encoding.UTF8.GetBytes(postDataStr) : Encoding.Default.GetBytes(postDataStr);
                request.ContentLength = buffer.Length;
                string str2 = request.Headers.ToString();
                using (Stream stream3 = request.GetRequestStream())
                {
                    stream3.Write(buffer, 0, buffer.Length);
                }
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                string str3 = isutf8 ? new StreamReader(responseStream, Encoding.UTF8).ReadToEnd() : new StreamReader(responseStream, Encoding.Default).ReadToEnd();
                str3 = string.Concat(new object[] { str3, "\r\n\r\n=================================================\r\n\r\n本次请求：", Url, " 响应结果：", response.StatusCode, "\r\n\r\nCookie数量", request.CookieContainer.Count, "\r\n", request.CookieContainer.GetCookieHeader(new Uri(Url)), "\r\nrequest:\r\n", str2, "\r\nresponse:\r\n", response.Headers.ToString(), "\r\n\r\n=================================================\r\n\r\n" });
                smethod_4(ref cookie, Url, response.Headers["Set-Cookie"], response.Cookies);
                if (isRedirect)
                {
                    if ((response.Headers["Location"] != null) && (response.Headers["Location"].Length > 2))
                    {
                        string url = "";
                        if (response.Headers["Location"].ToLower().Contains("http://"))
                        {
                            url = response.Headers["Location"];
                        }
                        else
                        {
                            string str5 = smethod_2(response.Headers["Location"], Url);
                            url = Url;
                            if (str5 != null)
                            {
                                url = str5;
                            }
                        }
                        str3 = HttpGET(url, Url, isutf8 ? 2 : 1, ref cookie, timeout, useragent, isRedirect) + str3;
                    }
                    else if ((response.Headers["Refresh"] != null) && (response.Headers["Refresh"].Length > 2))
                    {
                        string str6 = response.Headers["Refresh"].ToLower().Replace("url=", "`").Split(new char[] { '`' })[1];
                        if (!str6.Contains("http://"))
                        {
                            string str7 = smethod_2(str6, Url);
                            str6 = Url;
                            if (str7 != null)
                            {
                                str6 = str7;
                            }
                        }
                        str3 = HttpGET(str6, Url, isutf8 ? 2 : 1, ref cookie, timeout, useragent, isRedirect) + str3;
                    }
                }
                message = str3;
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return message;
        }

        public static Image HttpPostImage(string Url, string postDataStr, string refrere, bool isutf8, bool sendmode, ref CookieCollection cookie, int timeout)
        {
            Image image = null;
            Stream responseStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Image image2;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 0x3e8;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.smethod_0);
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(Url));
                request.Proxy = WebRequest.GetSystemWebProxy();
                request.ServicePoint.Expect100Continue = false;
                request.CookieContainer = smethod_3(cookie, Url);
                request.Method = "POST";
                request.AutomaticDecompression = DecompressionMethods.GZip;
                if (sendmode)
                {
                    string newValue = "X" + DateTime.Now.Ticks;
                    request.ContentType = "multipart/form-data; boundary=---------------------------" + newValue;
                    postDataStr = postDataStr.Replace("7da38e26991192", newValue);
                }
                else
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                request.Referer = refrere;
                request.Timeout = timeout * 0x3e8;
                request.AllowAutoRedirect = false;
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers.Add("Accept-Language", "zh-cn");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("UA-CPU", "x86");
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.2)";
                byte[] buffer = isutf8 ? Encoding.UTF8.GetBytes(postDataStr) : Encoding.Default.GetBytes(postDataStr);
                request.ContentLength = buffer.Length;
                request.Headers.ToString();
                using (Stream stream3 = request.GetRequestStream())
                {
                    stream3.Write(buffer, 0, buffer.Length);
                }
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                smethod_4(ref cookie, Url, response.Headers["Set-Cookie"], response.Cookies);
                image = Image.FromStream(responseStream);
                request.Abort();
                image2 = image;
            }
            catch
            {
                image2 = null;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return image2;
        }

        private static bool smethod_0(object object_0, X509Certificate x509Certificate_0, X509Chain x509Chain_0, SslPolicyErrors sslPolicyErrors_0)
        {
            return true;
        }

        private static string smethod_2(string string_0, string string_1)
        {
            try
            {
                string_0 = string_0.Trim();
                if (string_0.Contains("javascript") && string_0.Contains(":"))
                {
                    return null;
                }
                if ((string_0.Length > 7) && ((string_0.ToLower().Substring(0, 7) == "http://") || (string_0.ToLower().Substring(0, 8) == "https://")))
                {
                    return string_0;
                }
                if (string_0 == "")
                {
                    return string_1;
                }
                string uriString = "";
                Uri uri = new Uri(string_1);
                if (string_0.Substring(0, 1) == "/")
                {
                    uriString = "http://" + uri.Host + string_0;
                }
                else
                {
                    string pathAndQuery = uri.PathAndQuery;
                    if (pathAndQuery.Substring(pathAndQuery.Length - 1, 1) == "/")
                    {
                        uriString = "http://" + uri.Host + pathAndQuery + string_0;
                    }
                    else if ((string_0.Length > 1) && (string_0.Substring(0, 1) == "?"))
                    {
                        uriString = "http://" + uri.Host + uri.PathAndQuery + string_0;
                    }
                    else
                    {
                        if (uri.PathAndQuery.Contains("/"))
                        {
                            pathAndQuery = pathAndQuery.Replace(uri.PathAndQuery.Split(new char[] { '/' })[uri.PathAndQuery.Split(new char[] { '/' }).Length - 1], "");
                        }
                        else
                        {
                            pathAndQuery = "/";
                        }
                        uriString = "http://" + uri.Host + pathAndQuery + string_0;
                    }
                }
                new Uri(uriString);
                return uriString;
            }
            catch (Exception exception)
            {
                Console.WriteLine(DateTime.Now.ToString() + ":在处理一个URL时出错了，错误：" + exception.Message + ",但并不影响任务执行！", 1);
                return null;
            }
        }

        private static CookieContainer smethod_3(CookieCollection cookieCollection_0, string string_0)
        {
            CookieContainer container = new CookieContainer();
            Uri uri = new Uri(string_0);
            foreach (Cookie cookie in cookieCollection_0)
            {
                bool flag = ((cookie.Domain == uri.Host) || (cookie.Domain == ("." + uri.Host))) || (uri.Host.Contains(cookie.Domain) && (cookie.Domain.Substring(0, 1) == "."));
                bool flag2 = uri.PathAndQuery.Contains(cookie.Path);
                if (flag && flag2)
                {
                    if (cookie.Domain == ("." + uri.Host))
                    {
                        Cookie cookie2 = new Cookie(cookie.Name, cookie.Value, cookie.Path, uri.Host);
                        container.Add(cookie2);
                    }
                    else
                    {
                        container.Add(cookie);
                    }
                }
            }
            return container;
        }

        private static void smethod_4(ref CookieCollection cookieCollection_0, string string_0, string string_1, CookieCollection cookieCollection_1)
        {
            if (string_1 == null)
            {
                string_1 = "";
            }
            ArrayList list = smethod_6(string_0, string_1);
            for (int i = 0; i < list.Count; i++)
            {
                Cookie cookie = (Cookie)list[i];
                cookieCollection_0.Add(cookie);
            }
            if (cookieCollection_1.Count > 0)
            {
                foreach (Cookie cookie2 in cookieCollection_1)
                {
                    cookieCollection_0.Add(cookie2);
                }
            }
        }

        private static string smethod_5(string string_0)
        {
            char[] array = string_0.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        private static ArrayList smethod_6(string string_0, string string_1)
        {
            string str5;
            Cookie cookie2;
            int num4;
            ArrayList list = new ArrayList();
            Uri uri = new Uri(string_0);
            string_1 = string_1.Replace(", ", "  ");
            if (!string_1.Contains(","))
            {
                str5 = string_1;
                if (!str5.Contains(";"))
                {
                    return list;
                }
                cookie2 = new Cookie
                {
                    Domain = "AAAA"
                };
                num4 = 0;
                goto Label_04AD;
            }
            string[] strArray = string_1.Split(new char[] { ',' });
            int index = 0;
        Label_0048:
            if (index >= strArray.Length)
            {
                return list;
            }
            string str = strArray[index];
            if (!str.Contains(";"))
            {
                goto Label_02B1;
            }
            Cookie cookie = new Cookie
            {
                Domain = "AAAA"
            };
            int num2 = 0;
            goto Label_0246;
        Label_0240:
            num2++;
        Label_0246: ;
            if (num2 < str.Split(new char[] { ';' }).Length)
            {
                string str2 = str.Split(new char[] { ';' })[num2];
                if (!str2.Contains("="))
                {
                    goto Label_0240;
                }
                string str3 = smethod_5(smethod_5(str2.Split(new char[] { '=' })[0].Trim()).Trim());
                string str4 = "";
                int num3 = 1;
            Label_015C: ;
                if (num3 < str2.Split(new char[] { '=' }).Length)
                {
                    if (num3 == (str2.Split(new char[] { '=' }).Length - 1))
                    {
                        str4 = str4 + str2.Split(new char[] { '=' })[num3];
                    }
                    else
                    {
                        str4 = str4 + str2.Split(new char[] { '=' })[num3] + "=";
                    }
                    num3++;
                    goto Label_015C;
                }
                if (str3.ToLower() == "domain")
                {
                    if (str4.Substring(0, 1) != ".")
                    {
                        cookie.Domain = "." + str4;
                    }
                    else
                    {
                        cookie.Domain = str4;
                    }
                }
                else if (str3.ToLower() == "path")
                {
                    cookie.Path = str4;
                }
                else if (str3.ToLower() == "version")
                {
                    cookie.Version = Convert.ToInt32(str4);
                }
                else if ((str3.ToLower() != "expires") && (str3.ToLower() != "max-age"))
                {
                    cookie.Name = str3;
                    cookie.Value = str4;
                }
                goto Label_0240;
            }
            if (cookie.Domain == "AAAA")
            {
                cookie.Domain = "." + uri.Host;
            }
            if (cookie.Name != "")
            {
                try
                {
                    list.Add(cookie);
                }
                catch
                {
                }
            }
        Label_02B1:
            index++;
            goto Label_0048;
        Label_04A7:
            num4++;
        Label_04AD: ;
            if (num4 < str5.Split(new char[] { ';' }).Length)
            {
                string str6 = str5.Split(new char[] { ';' })[num4];
                if (!str6.Contains("="))
                {
                    goto Label_04A7;
                }
                string str7 = smethod_5(smethod_5(str6.Split(new char[] { '=' })[0].Trim()).Trim());
                string str8 = "";
                int num5 = 1;
            Label_03C3: ;
                if (num5 < str6.Split(new char[] { '=' }).Length)
                {
                    if (num5 == (str6.Split(new char[] { '=' }).Length - 1))
                    {
                        str8 = str8 + str6.Split(new char[] { '=' })[num5];
                    }
                    else
                    {
                        str8 = str8 + str6.Split(new char[] { '=' })[num5] + "=";
                    }
                    num5++;
                    goto Label_03C3;
                }
                if (str7.ToLower() == "domain")
                {
                    if (str8.Substring(0, 1) != ".")
                    {
                        cookie2.Domain = "." + str8;
                    }
                    else
                    {
                        cookie2.Domain = str8;
                    }
                }
                else if (str7.ToLower() == "path")
                {
                    cookie2.Path = str8;
                }
                else if (str7.ToLower() == "version")
                {
                    cookie2.Version = Convert.ToInt32(str8);
                }
                else if ((str7.ToLower() != "expires") && (str7.ToLower() != "max-age"))
                {
                    cookie2.Name = str7;
                    cookie2.Value = str8;
                }
                goto Label_04A7;
            }
            if (cookie2.Domain == "AAAA")
            {
                cookie2.Domain = "." + uri.Host;
            }
            if (cookie2.Name != "")
            {
                try
                {
                    list.Add(cookie2);
                }
                catch
                {
                }
            }
            return list;
        }

        private static ArrayList smethod_7(string string_0, string string_1)
        {
            Uri uri;
            string[] strArray;
            int num;
            ArrayList list2;
            ArrayList list = new ArrayList();
            try
            {
                uri = new Uri(string_0);
                if (string.IsNullOrEmpty(string_1))
                {
                    return list;
                }
                strArray = string_1.Split(new char[] { ';' });
                num = 0;
                goto Label_003B;
            }
            catch
            {
                list2 = list;
            }
            return list2;
        Label_003B:
            if (num >= strArray.Length)
            {
                return list;
            }
            string str = strArray[num];
            Cookie cookie = new Cookie
            {
                Domain = "AAAA"
            };
            int index = 0;
            goto Label_0228;
        Label_0222:
            index++;
        Label_0228: ;
            if (index < str.Split(new char[] { ';' }).Length)
            {
                string str2 = str.Split(new char[] { ';' })[index];
                if (!str2.Contains("="))
                {
                    goto Label_0222;
                }
                string str3 = smethod_5(smethod_5(str2.Split(new char[] { '=' })[0].Trim()).Trim());
                string str4 = "";
                int num3 = 1;
            Label_013E: ;
                if (num3 < str2.Split(new char[] { '=' }).Length)
                {
                    if (num3 == (str2.Split(new char[] { '=' }).Length - 1))
                    {
                        str4 = str4 + str2.Split(new char[] { '=' })[num3];
                    }
                    else
                    {
                        str4 = str4 + str2.Split(new char[] { '=' })[num3] + "=";
                    }
                    num3++;
                    goto Label_013E;
                }
                if (str3.ToLower() == "domain")
                {
                    if (str4.Substring(0, 1) != ".")
                    {
                        cookie.Domain = "." + str4;
                    }
                    else
                    {
                        cookie.Domain = str4;
                    }
                }
                else if (str3.ToLower() == "path")
                {
                    cookie.Path = str4;
                }
                else if (str3.ToLower() == "version")
                {
                    cookie.Version = Convert.ToInt32(str4);
                }
                else if ((str3.ToLower() != "expires") && (str3.ToLower() != "max-age"))
                {
                    cookie.Name = str3;
                    cookie.Value = str4;
                }
                goto Label_0222;
            }
            if (cookie.Domain == "AAAA")
            {
                cookie.Domain = "." + uri.Host;
            }
            if (cookie.Name != "")
            {
                try
                {
                    list.Add(cookie);
                }
                catch
                {
                }
            }
            num++;
            goto Label_003B;
        }



        public static ArrayList getregex(string html, string regex)
        {
            ArrayList list = new ArrayList();
            try
            {
                MatchCollection matchs = Regex.Matches(html, regex, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                for (int i = 0; i < matchs.Count; i++)
                {
                    list.Add(matchs[i].Groups[0].Value);
                }
                return list;
            }
            catch (Exception exception)
            {
                list = new ArrayList();
                list.Add("正则表达式有误：" + exception.Message);
                return list;
            }
        }

    }
}
