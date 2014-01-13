using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Moban.DBUtility;
using System.Data;

namespace Moban.DAL
{
  public  class ScreenshotsDAL
    {
      public ScreenshotsDAL()
      { }


      /// <summary>
      /// 是否存在该记录
      /// </summary>
      public bool Exists(int mid)
      {
          StringBuilder strSql = new StringBuilder();
          strSql.Append("select count(1) from Screenshots");
          strSql.Append(" where mid=@mid");
          SqlParameter[] parameters = {
					new SqlParameter("@mid", SqlDbType.Int)
                           };
          parameters[0].Value = mid;
          return DbHelperSQL.Exists(strSql.ToString(), parameters);
      }

      public bool ExistsPic(int mid, string pic)
      {
          string strSql = string.Format("select count(1) from ScreenshotsPic where mid={0} and pic = '{1}'", mid, pic);
          return DbHelperSQL.Exists(strSql.ToString());
      }


      /// <summary>
      /// 增加一条数据
      /// </summary>
      public int Add(Model.ScreenshotsInfo model)
      {
          StringBuilder strSql = new StringBuilder();
          strSql.Append("insert into Screenshots(");
          strSql.Append("[mid],[body],[IsA])");
          strSql.Append(" values (");
          strSql.Append("@mid,@body,@IsA)");
          strSql.Append(";select @@IDENTITY");
          SqlParameter[] parameters = {
					 
                    new SqlParameter("@mid", SqlDbType.Int),
					new SqlParameter("@body", SqlDbType.NVarChar),
					new SqlParameter("@IsA", SqlDbType.Int)   
                    
                                       };
          parameters[0].Value = model.MId;
          parameters[1].Value = model.Body;
          parameters[2].Value = model.IsA;
          object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
          return obj == null ? 0 : Convert.ToInt32(obj);
      }

      public int AddPic(int mid,string pic,int type)
      {
          StringBuilder strSql = new StringBuilder();
          strSql.Append("insert into ScreenshotsPic(");
          strSql.Append("[mid],[pic],[type])");
          strSql.Append(" values (");
          strSql.Append("@mid,@pic,@type)");
          strSql.Append(";select @@IDENTITY");
          SqlParameter[] parameters = {
					 
                    new SqlParameter("@mid", SqlDbType.Int),
					new SqlParameter("@pic", SqlDbType.NVarChar),
					new SqlParameter("@type", SqlDbType.Int)   
                    
                                       };
          parameters[0].Value = mid;
          parameters[1].Value = pic;
          parameters[2].Value = type;
          object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
          return obj == null ? 0 : Convert.ToInt32(obj);
      }


      public DataSet GetNeedDownScreenshotsList()
      {
          string strSql = "select top 20 id,Screenshots from moban where Screenshots<>'' and host = 'Themeforest.net' and id not in (select mid from Screenshots) order by id asc";
          return DbHelperSQL.Query(strSql.ToString());

      }
    }
}
