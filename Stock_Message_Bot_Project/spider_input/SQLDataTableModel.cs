using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;//SQLiteConnection ~ NuGet 引用 『System.Data.SQLite.Core』
using Dapper;//DynamicParameters  ~ NuGet 引用 『System.Data.SQLite.Core』
using System.Data;//IDbConnection
using System.Configuration;//ConfigurationManager   ~ NuGet 引用 『System.Configuration.ConfigurationMan』

namespace spider_input
{
    public class SQLDataTableModel
    {
        private static bool m_blnlogfile = true;

        public static string ConnectionStringLoad(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        //---
        //C# 通用操作語法 ~ for System.Data.SQLite.Core & Finisar.SQLite 元件 
        //https://einboch.pixnet.net/blog/post/248187728
        public static SQLiteConnection OpenConn(string Database)//資料庫連線程式
        {
            string cnstr = ConnectionStringLoad(Database);
            SQLiteConnection icn = new SQLiteConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            icn.Open();
            return icn;
        }

        public static SQLiteConnection OpenConn()//資料庫連線程式
        {
            string cnstr = ConnectionStringLoad("Default");
            SQLiteConnection icn = new SQLiteConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            icn.Open();
            return icn;
        }
        public static void CreateSQLiteDatabase(string Database)//建立資料庫程式
        {
            string cnstr = string.Format("Data Source=" + Database + ";Version=3;New=True;Compress=True;");
            SQLiteConnection icn = new SQLiteConnection();
            icn.ConnectionString = cnstr;
            icn.Open();
            icn.Close();
        }

        public static void CreateSQLiteTable( string CreateTableString)//建立資料表程式
        {
            SQLiteConnection icn = OpenConn();//OpenConn(Database);
            SQLiteCommand cmd = new SQLiteCommand(CreateTableString, icn);
            SQLiteTransaction mySqlTransaction = icn.BeginTransaction();
            try
            {
                cmd.Transaction = mySqlTransaction;
                cmd.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                //throw (ex);
            }
            if (icn.State == ConnectionState.Open) icn.Close();
        }

        public static void SQLiteInsertUpdateDelete(string Database, string SqlSelectString)//新增資料程式
        {
            SQLiteConnection icn = OpenConn(Database);
            SQLiteCommand cmd = new SQLiteCommand(SqlSelectString, icn);
            SQLiteTransaction mySqlTransaction = icn.BeginTransaction();
            try
            {
                cmd.Transaction = mySqlTransaction;
                cmd.ExecuteNonQuery();
                mySqlTransaction.Commit();
                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SqlSelectString, "success");
                    LogFile.Write( StrLog);
                }
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                //throw (ex);
                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SqlSelectString, ex.ToString());
                    LogFile.Write( StrLog);
                }
            }
            if (icn.State == ConnectionState.Open) icn.Close();
        }

        public static void SQLiteInsertUpdateDelete(string SqlSelectString)//新增資料程式
        {

            // The critical section.
            SQLiteConnection icn = OpenConn();//OpenConn(Database);
            SQLiteCommand cmd = new SQLiteCommand(SqlSelectString, icn);
            SQLiteTransaction mySqlTransaction = icn.BeginTransaction();
            try
            {
                cmd.Transaction = mySqlTransaction;
                cmd.ExecuteNonQuery();
                mySqlTransaction.Commit();
                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SqlSelectString, "success");
                    LogFile.Write( StrLog);
                }
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SqlSelectString, ex.ToString());
                    LogFile.Write( StrLog);
                }
            }
            if (icn.State == ConnectionState.Open) icn.Close();

        }

        public static DataTable GetDataTable(string Database, string SQLiteString)//讀取資料程式
        {
            DataTable myDataTable = new DataTable(Database);
            try
            {

                SQLiteConnection icn = OpenConn(Database);
                SQLiteDataAdapter da = new SQLiteDataAdapter(SQLiteString, icn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds);
                myDataTable = ds.Tables[0];
                if (icn.State == ConnectionState.Open) icn.Close();

                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SQLiteString, "success");
                    LogFile.Write( StrLog);
                }
            }
            catch (Exception ex)
            {
                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SQLiteString, ex.ToString());
                    LogFile.Write( StrLog);
                }
            }

            return myDataTable;
        }

        public static DataTable GetDataTable(string SQLiteString)//讀取資料程式
        {
            DataTable myDataTable = new DataTable();

            try
            {

                SQLiteConnection icn = OpenConn();//OpenConn(Database);
                SQLiteDataAdapter da = new SQLiteDataAdapter(SQLiteString, icn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds);
                myDataTable = ds.Tables[0];
                if (icn.State == ConnectionState.Open) icn.Close();

                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SQLiteString, "success");
                    LogFile.Write( StrLog);
                }
            }
            catch (Exception ex)
            {
                if (m_blnlogfile)
                {
                    String StrLog = String.Format("{0}: {1};{2}", "Json2SQLite", SQLiteString, ex.ToString());
                    LogFile.Write( StrLog);
                }
            }

            return myDataTable;
        }


        //---C# SQLite通用操作語法 ~ for System.Data.SQLite.Core & Finisar.SQLite 元件 

        ///應用
        ///產生SQLite的資料庫文件，副檔名為.db
        ///CreateSQLiteDatabase("data.db");
        ///建立資料表test
        ///string createtablestring = "create table test (speed double, dist double);";
        ///CreateSQLiteTable("data.db", createtablestring);
        ///插入資料到test表中
        ///string insertstring = "insert into test (speed,dist) values ('10','100');insert into test (speed,dist) values ('20','200');";
        ///SQLiteInsertUpdateDelete("data.db", insertstring);
        ///讀取資料
        ///DataTable dt = GetDataTable("data.db", "select * from test");
        ///dataGridView1.DataSource = dt;  
    }//SQLDataTableModel
}
