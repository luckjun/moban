using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moban.Model
{
    public class MobanInfo
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
        public string Host { get; set; }
        public string MPic { get; set; }

        public string BPic { get; set; }

        public string Body { get; set; }

        public string Demo { get; set; }

        public string Info { get; set; }

        public int IsDownLoad { get; set; }

        public string DemoUrl { get; set; }

        public string BPics { get; set; }

        public int IsMDL { get; set; }

        public int IsBML { get; set; }

        public string LeftCol { get; set; }

        public string RightCol { get; set; }

        public int IsAnalyze { get; set; }
        public string Imgs { get; set; }


        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Type4 { get; set; }

        public string Relatedtype { get; set; }

        public string Title { get; set; }

        public string ProductDescription { get; set; }


        public DateTime CreateTime { get; set; }

        public DateTime CrawlTime { get; set; }

        public int Status { get; set; }

        public string Screenshots { get; set; }

        public string DownLoadUrl { get; set; }

    }
}
