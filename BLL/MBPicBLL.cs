using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Moban.Model;

namespace Moban.BLL
{
   public class MBPicBLL
    {
       private readonly DAL.MBPicDAL dal = new DAL.MBPicDAL();

       public MBPicBLL() { }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string url)
        {
            return dal.Exists(url);
        }

        public int Add(Moban.Model.MBPicInfo model)
        {
            return dal.Add(model);
        }


        public DataSet GetList()
        {
            return dal.GetList();
        }

        public DataSet GetNeedDownImageList()
        {
            return dal.GetNeedDownImageList();
        }



        public bool DownLoadImage(MBPicInfo model)
        {
            return dal.DownLoadImage(model);
        }
    }
}
