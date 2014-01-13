using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moban.Model
{
    public class PSDInfo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Info { get; set; }
        public string DownUrl { get; set; }
        public string Tag { get; set; }
        public string PicUrl { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
