using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Moban.DBUtility;
using System.Data.SqlClient;

namespace Moban.DAL
{
    public class PSDDAL
    {


        public PSDDAL() { }


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string Url)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from PSD");
            strSql.Append(" where url=@Url");
            SqlParameter[] parameters = {
					new SqlParameter("@Url", SqlDbType.NVarChar,400)
                           };
            parameters[0].Value = Url;
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.PSDInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into PSD(");
            strSql.Append("[Title],[url],[PicUrl],[CreateTime])");
            strSql.Append(" values (");
            strSql.Append("@Title,@url,@PicUrl,@CreateTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					 
                    new SqlParameter("@Title", SqlDbType.NVarChar),
					new SqlParameter("@url", SqlDbType.NVarChar),
					new SqlParameter("@PicUrl", SqlDbType.NVarChar),                  
                    new SqlParameter("@CreateTime", SqlDbType.DateTime)                                        
                                       };
            parameters[0].Value = model.Title;
            parameters[1].Value = model.Url;
            parameters[2].Value = model.PicUrl;
            parameters[3].Value = model.CreateTime;
 

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }


        public DataSet GetPicUrlList()
        {
            string strSql = "select PicUrl from psd";
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNeedDownBodyList()
        {
            string strSql = "select id,url from psd where Status  is null  order by id asc";
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNoDemoUrlList()
        {
            string strSql = "select id,demo from moban where demourl is null order by id asc";
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNeedAnalyzeList()
        {
            string strSql = "select top 1000 id,body from moban where IsAnalyze =1 order by id asc";
            return DbHelperSQL.Query(strSql.ToString());

        }

        public DataSet GetNeedAnalyzeTypeList()
        {
            string strSql = "select top 1 id,body from moban where name = '' or name is null order by id asc";
            return DbHelperSQL.Query(strSql.ToString());

        }

        public bool DownLoadPage(Model.PSDInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update psd set body=@body,info=@info,DownUrl=@DownUrl,Tag=@Tag,Status=@Status where Id=@Id;");
            SqlParameter[] parameters = {
					 
                     
                    new SqlParameter("@body", SqlDbType.NVarChar),
                    new SqlParameter("@info", SqlDbType.NVarChar),
                    new SqlParameter("@DownUrl", SqlDbType.NVarChar),
                      new SqlParameter("@Tag", SqlDbType.NVarChar)  ,  
                    new SqlParameter("@Status", SqlDbType.Int) ,            
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };

            parameters[0].Value = model.Body;
            parameters[1].Value = model.Info;
            parameters[2].Value = model.DownUrl;
            parameters[3].Value = model.Tag;
            parameters[4].Value = model.Status;
            parameters[5].Value = model.Id;

            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;

        }




        public bool UpdateDemoUrl(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set demourl=@demourl  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@demourl", SqlDbType.NVarChar),                    
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.DemoUrl;
            parameters[1].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }

        

        public bool UpdateAnalyzeContent(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set LeftCol=@LeftCol,RightCol=@RightCol,BPics=@BPics,IsAnalyze=@IsAnalyze  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@LeftCol", SqlDbType.NVarChar),  
                     new SqlParameter("@RightCol", SqlDbType.NVarChar),  
                      new SqlParameter("@BPics", SqlDbType.NVarChar), 
                       new SqlParameter("@IsAnalyze", SqlDbType.Int), 
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.LeftCol;
            parameters[1].Value = model.RightCol;
            parameters[2].Value = model.BPics;
            parameters[3].Value = model.IsAnalyze;
            parameters[4].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }

        public bool UpdateAnalyzeType(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set Type1=@Type1,Type2=@Type2,Type3=@Type3,Type4=@Type4,Name=@Name,Title=@Title,Relatedtype=@Relatedtype  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@Type1", SqlDbType.NVarChar),  
                     new SqlParameter("@Type2", SqlDbType.NVarChar),  
                     new SqlParameter("@Type3", SqlDbType.NVarChar),   
                     new SqlParameter("@Type4", SqlDbType.NVarChar),   
                     new SqlParameter("@Name", SqlDbType.NVarChar),  
                     new SqlParameter("@Title", SqlDbType.NVarChar),  
                      new SqlParameter("@Relatedtype", SqlDbType.NVarChar), 
                      
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.Type1;
            parameters[1].Value = model.Type2;
            parameters[2].Value = model.Type3;
            parameters[3].Value = model.Type4;
            parameters[4].Value = model.Name;
            parameters[5].Value = model.Title;
            parameters[6].Value = model.Relatedtype;
            parameters[7].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }
    }
}
