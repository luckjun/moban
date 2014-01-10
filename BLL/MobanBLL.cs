using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Moban.Model;

namespace Moban.BLL
{
   public class MobanBLL
    {
       private readonly DAL.MobanDAL dal = new DAL.MobanDAL();

       public MobanBLL() { }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string url)
        {
            return dal.Exists(url);
        }



        public int Add(Moban.Model.MobanInfo model)
        {
            return dal.Add(model);
        }

        public int Add(string Title,string Body)
        {
            return dal.Add(Title, Body);
        }


        public DataSet GetList()
        {
            return dal.GetList();
        }

        public DataSet GetNeedDownBodyList()
        {
            return dal.GetNeedDownBodyList();
        }

        public DataSet GetNoDemoUrlList()
        {
           return dal.GetNoDemoUrlList(); 
        }

        public bool DownLoadPage(MobanInfo model)
        {
            return dal.DownLoadPage(model);
        }

        public DataSet GetNeedAnalyzeList()
        {
            return dal.GetNeedAnalyzeList(); 

        }

        public DataSet GetNeedAnalyzeTypeList()
        {
            return dal.GetNeedAnalyzeTypeList(); 

        }

        public MobanInfo GetModelByFileName(string filename)
        {
            return dal.GetModelByFileName(filename);
        }

        public bool UpdateDemoUrl(MobanInfo model)
        {
            return dal.UpdateDemoUrl(model);
        }



        public bool UpdateIsDownLoad(MobanInfo model)
        {
            return dal.UpdateIsDownLoad(model);
        }

        public bool UpdateAnalyzeContent(MobanInfo model)
        {
            return dal.UpdateAnalyzeContent(model);
        }
        public bool UpdateAnalyzeType(MobanInfo model)
        {
            return dal.UpdateAnalyzeType(model);
        }
    }
}
