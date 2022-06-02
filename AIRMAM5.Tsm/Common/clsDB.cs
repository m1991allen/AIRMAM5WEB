using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AIRMAM5.Tsm.Common
{
    public class clsDB
    {
        ///// <summary>
        ///// 資料庫查詢
        ///// </summary>
        ///// <param name="_spName">預存程序名稱</param>
        ///// <param name="_dic">參數</param>
        ///// <returns>DataTable</returns>
        //public static DataTable Do_Query(string _spName, Dictionary<string, string> _dic = null)
        //{
        //    SqlConnection _conn = new SqlConnection(Properties.Settings.Default.fsSQL_CONN);
        //    SqlDataAdapter _da = new SqlDataAdapter(_spName, _conn);
        //    DataTable _dt = new DataTable();
        //    try
        //    {
        //        if (_conn.State == ConnectionState.Closed)
        //        {
        //            _conn.Open();
        //        }

        //        _da.SelectCommand.CommandType = CommandType.StoredProcedure;
        //        if (_dic != null && _dic.Count > 0)
        //        {
        //            foreach (string paraName in _dic.Keys)
        //            {
        //                _da.SelectCommand.Parameters.Add(new SqlParameter("@" + paraName, _dic[paraName]));
        //            }
        //        }

        //        _da.Fill(_dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + DateTime.Now.ToString("yyyyMMdd") + ".txt", "發生時間:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "，錯誤訊息:" + _spName + "=>" + ex.Message + "\r\n");
        //        _dt = null;
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //        _conn.Dispose();
        //        _da.Dispose();
        //    }

        //    return _dt;
        //}

        ///// <summary>
        ///// 資料庫交易
        ///// </summary>
        ///// <param name="_spName">預存程序名稱</param>
        ///// <param name="_dic">參數</param>
        ///// <returns>訊息</returns>
        //public static string Do_Tran(string _spName, Dictionary<string, string> _dic)
        //{
        //    string _result = "";
        //    SqlConnection _conn = new SqlConnection(Properties.Settings.Default.fsSQL_CONN);
        //    SqlDataAdapter _da = new SqlDataAdapter(_spName, _conn);
        //    DataTable _dt = new DataTable();
        //    try
        //    {

        //        if (_conn.State == ConnectionState.Closed)
        //        {
        //            _conn.Open();
        //        }

        //        _da.SelectCommand.CommandType = CommandType.StoredProcedure;
        //        if (_dic != null && _dic.Count > 0)
        //        {
        //            foreach (string paraName in _dic.Keys)
        //            {
        //                _da.SelectCommand.Parameters.Add(new SqlParameter("@" + paraName, _dic[paraName]));
        //            }
        //        }
        //        _da.Fill(_dt);

        //        _result = _dt.Rows[0][0].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + DateTime.Now.ToString("yyyyMMdd") + ".txt", "發生時間:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "，錯誤訊息:" + _spName + "=>" + ex.Message + "\r\n");
        //        _result = "ERROR-" + ex.Message;
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //        _conn.Dispose();
        //    }
        //    return _result;
        //}
    }
}