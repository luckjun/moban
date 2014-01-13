using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Moban.DBUtility;
using System.Data.SqlClient;

namespace Moban.DAL
{
    public class MBPicDAL
    {


        public MBPicDAL() { }


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string Picurl)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from MBPic");
            strSql.Append(" where Picurl=@Picurl");
            SqlParameter[] parameters = {
					new SqlParameter("@Picurl", SqlDbType.NVarChar,400)
                           };
            parameters[0].Value = Picurl;
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.MBPicInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into MBPic(");
            strSql.Append("[Picurl],[LocalFile],[IsDownLoad],[CreateTime],[CrawlTime])");
            strSql.Append(" values (");
            strSql.Append("@PicUrl,@LocalFile,@IsDownLoad,@CreateTime,@CrawlTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					 
                    new SqlParameter("@PicUrl", SqlDbType.NVarChar),
					new SqlParameter("@LocalFile", SqlDbType.NVarChar),
					new SqlParameter("@IsDownLoad", SqlDbType.NVarChar),
				    new SqlParameter("@CreateTime", SqlDbType.DateTime) ,                    
                    new SqlParameter("@CrawlTime", SqlDbType.DateTime)    
                    
                                       };
            parameters[0].Value = model.PicUrl;
            parameters[1].Value = model.LocalFile;
            parameters[2].Value = model.IsDownLoad;
            parameters[3].Value = model.CreateTime;
            parameters[4].Value = model.CrawlTime;
             

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public int Add(string PicUrl)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into MBPic(");
            strSql.Append("[Picurl],[IsDownLoad],[CreateTime],[CrawlTime])");
            strSql.Append(" values (");
            strSql.Append("@PicUrl,@IsDownLoad,@CreateTime,@CrawlTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					 
                    new SqlParameter("@PicUrl", SqlDbType.NVarChar),
					new SqlParameter("@IsDownLoad", SqlDbType.Int),
				    new SqlParameter("@CreateTime", SqlDbType.DateTime) ,                    
                    new SqlParameter("@CrawlTime", SqlDbType.DateTime)    
                    
                                       };
            parameters[0].Value = PicUrl;
            parameters[1].Value = 0;
            parameters[2].Value = DateTime.Now;
            parameters[3].Value = DateTime.Now;
 
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }


        public DataSet GetList()
        {
            string strSql = "select mPic from moban";
           return DbHelperSQL.Query(strSql.ToString());
        }
        public DataSet GetNeedDownImageList()
        {
            string strSql = "select top 20 picurl from mbpic where IsDownLoad=0 or IsDownLoad is null order by id asc";
            return DbHelperSQL.Query(strSql.ToString());
        }


        public bool DownLoadImage(Model.MBPicInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update MBPic set IsDownLoad=@IsDownLoad,CrawlTime=@CrawlTime where picurl=@picurl;");
            SqlParameter[] parameters = {
					 
                     
                    
                    new SqlParameter("@IsDownLoad", SqlDbType.Int) ,
                    new SqlParameter("@CrawlTime", SqlDbType.DateTime)  ,  
                    new SqlParameter("@picurl", SqlDbType.NVarChar)
                                        };

            
            parameters[0].Value = model.IsDownLoad;
            parameters[1].Value = model.CrawlTime;
            parameters[2].Value = model.PicUrl;

            try
            {
                int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
                return obj == 0 ? false : true;
            }
            catch (Exception ex)
            {

                return false;
            }

            
        }
        
    }
}
