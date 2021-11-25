using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;


//数据访问层DAL主要功能就是获取数据库数据，采用ADO.NET得方式对数据库进行访问


namespace SoftwareManage.DAL
{
    public class DataAccess
    {
        //通过配置文件中App.config中获取数据库连接地址
        string dbConfig = ConfigurationManager.ConnectionStrings["db_config"].ToString();
       
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adapter;
        SqlTransaction trans;

        //访问完数据库之后将对象销毁，重写Dispose()函数
        private void Dispose() 
        {
            if (adapter != null)
            {
                adapter.Dispose();adapter = null;
            }
            if (comm != null)
            {
                comm.Dispose();comm = null;
            }
            if (trans != null)
            {
                trans.Dispose();trans = null;
            }
            if (conn != null)
            {
                conn.Close();conn.Dispose();conn = null;
            }
        }


        private DataTable GetDatas(string sql) 
        {
            DataTable dt = new DataTable();
            try
            {
                conn = new SqlConnection(dbConfig);
                conn.Open();

                adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            {
                this.Dispose();
            }
            return dt;
        }

        //获取存储区得相关数据
        public DataTable GetStorageArea()
        {
            string str = "select * from storage_area";
            return this.GetDatas(str);
        }

        //获取设备相关数据
        public DataTable GetDevice() 
        {
            string str = "select * from device";
            return this.GetDatas(str);
        }

        //获取数据监控信息
        public DataTable GetMonitorValues() 
        {
            string str = "select * from Monior_values ORDER BY d_id,value_id";
            return this.GetDatas(str);
        }

    }
}
