using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Moban.DBUtility;
using System.Data.SqlClient;
using Moban.Model;

namespace Moban.DAL
{
    public class PPTDAL
    {


        public PPTDAL() { }


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string Url)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from PPT");
            strSql.Append(" where url=@Url");
            SqlParameter[] parameters = {
					new SqlParameter("@Url", SqlDbType.NVarChar,400)
                           };
            parameters[0].Value = Url;
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        public bool ExistsByDLFileName(string filename)
        {
            string strSql = string.Format("select count(1) from PPT where downlink like '%{0}%'", filename);
            return DbHelperSQL.Exists(strSql);
        }
     

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.PPTInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into PPT(");
            strSql.Append("[Title],[url],[Host],[mpic],[bpic],[body],[Jieshao],[Tag],[DownLink])");
            strSql.Append(" values (");
            strSql.Append("@Title,@url,@Host,@mpic,@bpic,@body,@Jieshao,@Tag,@DownLink)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {	 
                    new SqlParameter("@Title", SqlDbType.NVarChar),
					new SqlParameter("@url", SqlDbType.NVarChar),
                    new SqlParameter("@Host", SqlDbType.NVarChar),
					new SqlParameter("@mpic", SqlDbType.NVarChar),
					new SqlParameter("@bpic", SqlDbType.NVarChar),
                    new SqlParameter("@body", SqlDbType.NVarChar),
                    new SqlParameter("@Jieshao", SqlDbType.NVarChar) ,
                    new SqlParameter("@Tag", SqlDbType.NVarChar) ,                 
                    new SqlParameter("@DownLink", SqlDbType.NVarChar)    
                    };
            parameters[0].Value = model.Title;
            parameters[1].Value = model.Url;
            parameters[2].Value = model.Host;
            parameters[3].Value = model.MPic;
            parameters[4].Value = model.BPic;
            parameters[5].Value = model.Body;
            parameters[6].Value = model.Jieshao;
            parameters[7].Value = model.Tag;
            parameters[8].Value = model.DownLink;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public int PartsAdd(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Parts(");
            strSql.Append("[Name],[url],[body],[CreateTime])");
            strSql.Append(" values (");
            strSql.Append("@Name,@url,@body,@CreateTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					 
                    new SqlParameter("@Name", SqlDbType.NVarChar),
					new SqlParameter("@url", SqlDbType.NVarChar),
                    new SqlParameter("@body", SqlDbType.NText),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime)                        
                    
                                       };
            parameters[0].Value = model.Name;
            parameters[1].Value = model.Url;
            parameters[2].Value = model.Body;
            parameters[3].Value = model.CreateTime;


            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(string title, string body)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into HTML(");
            strSql.Append("[title],[body])");
            strSql.Append(" values (");
            strSql.Append("@title,@body)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					 
                    new SqlParameter("@title", SqlDbType.NVarChar),
					new SqlParameter("@body", SqlDbType.NVarChar)   
                    
                                       };
            parameters[0].Value = title;
            parameters[1].Value = body;
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }


        public DataSet GetList()
        {
            string strSql = "select mPic from moban";
            return DbHelperSQL.Query(strSql.ToString());
        }
        public DataSet GetNeedDownBodyList()
        {
            string strSql = "select id,url from moban where isdownload = 0 or isdownload is null order by id asc";
            return DbHelperSQL.Query(strSql.ToString());
        }


        public DataSet GetNoDemoList(string host)
        {
            string strSql = string.Format("select top 20 id,body from moban where host = '{0}' and (demo is null or demo = '') order by id asc", host);
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNoDemoUrlList()
        {
            string strSql = "select id,demo from moban where demourl is null order by id asc";
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNoDemoUrlList(string host)
        {
            string strSql = string.Format("select top 20 id,demo from moban where demo!='404' and host = '{0}' and (demourl is null or demourl = '') order by id asc", host);
            return DbHelperSQL.Query(strSql.ToString());
        }


        public DataSet GetNeedDownList()
        {
            string strSql = "select top 100 id,host,downlink from ppt where IsDown is null or IsDown =0 order by newid()";
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNeedAnalyzeList(string host)
        {
            string strSql = "select top 100 id,body from moban where (isAnalyze is null or isAnalyze = '') and host = '" + host + "' order by id asc";
            return DbHelperSQL.Query(strSql.ToString());
        }

        public DataSet GetNeedAnalyzeTypeList()
        {
            string strSql = "select top 1 id,body from moban where name = '' or name is null order by id asc";
            return DbHelperSQL.Query(strSql.ToString());

        }


        public PPTInfo GetModelByFileName(string filename)
        {
            string strSql = string.Format("select  top 1 * from PPT  where downlink like '%/{0}'", filename);
            PPTInfo model = new PPTInfo();
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Id"] != null && ds.Tables[0].Rows[0]["Id"].ToString() != "")
                {
                    model.Id = int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
                }
                if (ds.Tables[0].Rows[0]["IsDown"] != null && ds.Tables[0].Rows[0]["IsDown"].ToString() != "")
                {
                    model.IsDown = int.Parse(ds.Tables[0].Rows[0]["IsDown"].ToString());
                }
                model.DownLink = ds.Tables[0].Rows[0]["DownLink"].ToString();
                return model;
            }
            else
            {
                return null;
            }
        }

        public MobanInfo GetModelByFileNameWithId(int id)
        {
            string strSql = string.Format("select  top 1 * from moban  where id={0}", id);
            MobanInfo model = new MobanInfo();
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Id"] != null && ds.Tables[0].Rows[0]["Id"].ToString() != "")
                {
                    model.Id = int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
                }
                model.DemoUrl = ds.Tables[0].Rows[0]["demourl"].ToString();
                return model;
            }
            else
            {
                return null;
            }
        }


        public bool UpdateIsDownLoad(Model.PPTInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update PPT set IsDown=@IsDown where Id=@Id;");
            SqlParameter[] parameters = {
                    new SqlParameter("@IsDown", SqlDbType.Int) ,
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };

            parameters[0].Value = model.IsDown;
            parameters[1].Value = model.Id;

            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;

        }

        public bool DownLoadPage(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set body=@body,IsDownLoad=@IsDownLoad,CrawlTime=@CrawlTime where Id=@Id;");
            SqlParameter[] parameters = {
                    new SqlParameter("@body", SqlDbType.NVarChar),
                    new SqlParameter("@IsDownLoad", SqlDbType.Int) ,
                    new SqlParameter("@CrawlTime", SqlDbType.DateTime)  ,  
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };

            parameters[0].Value = model.Body;
            parameters[1].Value = model.IsDownLoad;
            parameters[2].Value = model.CrawlTime;
            parameters[3].Value = model.Id;

            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;

        }

        /// <summary>
        /// 更新demo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateScreenshots(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set Screenshots=@Screenshots  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@Screenshots", SqlDbType.NVarChar),                    
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.Screenshots;
            parameters[1].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }

        /// <summary>
        /// 更新demo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateDemo(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set demo=@demo  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@demo", SqlDbType.NVarChar),                    
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.Demo;
            parameters[1].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }


        /// <summary>
        /// 更新demourl
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
            strSql.Append("Update moban set LeftCol=@LeftCol,RightCol=@RightCol,BPics=@BPics,IsAnalyze=@IsAnalyze,Screenshots=@Screenshots  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@LeftCol", SqlDbType.NVarChar),  
                    new SqlParameter("@RightCol", SqlDbType.NVarChar),  
                    new SqlParameter("@BPics", SqlDbType.NVarChar), 
                    new SqlParameter("@IsAnalyze", SqlDbType.Int), 
                    new SqlParameter("@Screenshots", SqlDbType.NVarChar), 
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.LeftCol;
            parameters[1].Value = model.RightCol;
            parameters[2].Value = model.BPics;
            parameters[3].Value = model.IsAnalyze;
            parameters[4].Value = model.Screenshots;
            parameters[5].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }


        public bool UpdateAnalyzeElemisfreebiesContent(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set LeftCol=@LeftCol,RightCol=@RightCol,Title=@Title,IsAnalyze=@IsAnalyze,DownLoadUrl=@DownLoadUrl,Demo=@Demo  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@LeftCol", SqlDbType.NVarChar),  
                    new SqlParameter("@RightCol", SqlDbType.NVarChar),  
                    new SqlParameter("@Title", SqlDbType.NVarChar), 
                    new SqlParameter("@IsAnalyze", SqlDbType.Int), 
                    new SqlParameter("@DownLoadUrl", SqlDbType.NVarChar), 
                    new SqlParameter("@Demo", SqlDbType.NVarChar), 
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.LeftCol;
            parameters[1].Value = model.RightCol;
            parameters[2].Value = model.Title;
            parameters[3].Value = model.IsAnalyze;
            parameters[4].Value = model.DownLoadUrl;
            parameters[5].Value = model.Demo;
            parameters[6].Value = model.Id;
            int obj = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return obj == 0 ? false : true;
        }


        public bool UpdateAnalyzeThemeforestContent(Model.MobanInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update moban set LeftCol=@LeftCol,RightCol=@RightCol,BPics=@BPics,IsAnalyze=@IsAnalyze,Screenshots=@Screenshots,mpic=@mpic  where Id=@Id;");
            SqlParameter[] parameters = {                   
                    new SqlParameter("@LeftCol", SqlDbType.NVarChar),  
                    new SqlParameter("@RightCol", SqlDbType.NVarChar),  
                    new SqlParameter("@BPics", SqlDbType.NVarChar), 
                    new SqlParameter("@IsAnalyze", SqlDbType.Int), 
                    new SqlParameter("@Screenshots", SqlDbType.NVarChar), 
                    new SqlParameter("@mpic", SqlDbType.NVarChar), 
                    new SqlParameter("@Id", SqlDbType.Int)
                                        };
            parameters[0].Value = model.LeftCol;
            parameters[1].Value = model.RightCol;
            parameters[2].Value = model.BPics;
            parameters[3].Value = model.IsAnalyze;
            parameters[4].Value = model.Screenshots;
            parameters[5].Value = model.MPic;
            parameters[6].Value = model.Id;
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
