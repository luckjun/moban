using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Moban.Utils
{
    public static class StringHelper
    {
        
        #region 去除HTML标记
        ///<summary>   
        ///去除HTML标记   
        ///</summary>   
        ///<param name="NoHTML">包括HTML的源码</param>   
        ///<returns>已经去除后的文字</returns>   
        public static string NoHTML(string Htmlstring)
        {
            //Regex myReg = new Regex(@"(\<.[^\<]*\>)", RegexOptions.IgnoreCase);
            //Htmlstring = myReg.Replace(Htmlstring, "");
            //myReg = new Regex(@"(\<\/[^\<]*\>)", RegexOptions.IgnoreCase);
            //Htmlstring = myReg.Replace(Htmlstring, "");
            //return Htmlstring;

            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "“", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "”", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring = Htmlstring.Replace("<", "&lt;");
            Htmlstring = Htmlstring.Replace(">", "&gt;");
            return Htmlstring;
        }
        #endregion

        public static string GetRegValue(string str, string pattern)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(str);
            foreach (Match ml in mc)
            {
                return ml.Groups[1].ToString();
            }
            return "";
        }

        public static string GetRegValues(string str, string pattern)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(str);
            string rtn = "";
            foreach (Match ml in mc)
            {
                rtn += ml.Groups[1].ToString() + ",";
            }

            return rtn.Substring(0, rtn.Length - 1);
        }


    }
}
