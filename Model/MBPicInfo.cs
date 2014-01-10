using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moban.Model
{
   public class MBPicInfo
    {
       public int Id { get; set; }
       public string PicUrl { get; set; }
       public string LocalFile { get; set; }
       public int IsDownLoad { get; set; }
       public DateTime CreateTime { get; set; }
       public DateTime CrawlTime { get; set; } 
    }
}
