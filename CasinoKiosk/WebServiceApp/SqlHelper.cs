using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace WebServiceApp
{
    public class SqlHelper
    {
        public SqlConnection sqlCn;
        private string strErrorMessage;
        private int intErrorNumber;
        private string mErrorMsg;
        private int mErrorCode;
        private string sConnectionString;
        public SqlHelper()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["CasinoLoyaltyKiosk"].ConnectionString;
            sConnectionString = ConfigurationManager.ConnectionStrings["CasinoLoyaltyKiosk"].ConnectionString;
        }
        public string ConnectionString
        {
            get
            {
                return sConnectionString;
            }
            set
            {
                sConnectionString = value;
            }
        }
        // Ham mo ket noi
        public void OpenConnection(string module)
        {
            sConnectionString = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
        }
        /// <summary>
        /// String: Câu thông báo lỗi khi truy cập SQL
        /// </summary>
        /// 
        public string ErrorMessage
        {
            get
            {
                return strErrorMessage;
            }
        }
        /// <summary>
        /// int: Số lỗi khi truy cập SQL
        /// </summary>
        public int ErrorNumber
        {
            get
            {
                return intErrorNumber;
            }
        }

        /// <summary>
        /// Ham KetNoiCSDL
        /// </summary>
        /// <return>
        /// DataTable
        /// </return>>
        public SqlConnection OpenConnection()
        {
            SqlConnection sqlCn = new SqlConnection(ConnectionString);
            try
            {
                sqlCn.Open();
            }
            catch
            {
                return null;
            }
            return sqlCn;
        }

        public void CloseConnection(SqlConnection sqlCn)
        {
            if (sqlCn != null)
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();
            }
        }

        public DataTable SP_TO_DataTable(
            string TenStoreProcedure,
            SqlParameter[] sqlParam)
        {
            DataTable dtbTmp = new DataTable();
            try
            {
                sqlCn = OpenConnection();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandTimeout = 2000;
                sqlCmd.Connection = sqlCn;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = TenStoreProcedure;
                for (int i = 0; i < sqlParam.Length; i++)
                {
                    sqlCmd.Parameters.Add(sqlParam[i]);
                }
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = sqlCmd;
                sqlDA.Fill(dtbTmp);
                CloseConnection(sqlCn);
            }
            catch (SqlException sqlEx)
            {
                strErrorMessage = sqlEx.Message;
                intErrorNumber = sqlEx.Number;
            }
            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();

            }
            return dtbTmp;
        }
        //Thuc thi store procedue khong tham so
        protected DataTable SP_To_DataTable_WithOut_Paras(
               string TenStoreProcedure)
        {
            DataTable dtbTmp = new DataTable();
            try
            {
                sqlCn = OpenConnection();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandTimeout = 2000;
                sqlCmd.Connection = sqlCn;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = TenStoreProcedure;
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = sqlCmd;
                sqlDA.Fill(dtbTmp);
                CloseConnection(sqlCn);
            }
            catch (SqlException sqlEx)
            {
                strErrorMessage = sqlEx.Message;
                intErrorNumber = sqlEx.Number;
            }
            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();

            }
            return dtbTmp;
        }
        /// <summary>
        /// Thuc thi 1 store tra ve du lieu la dataset
        /// </summary>
        /// <param name="TenStoreProcedure"></param>
        /// <param name="sqlParam"></param>
        /// <returns></returns>
        protected DataSet SP_To_DataSet(
            string TenStoreProcedure,
            SqlParameter[] sqlParam)
        {
            DataSet dtbTmp = new DataSet();
            try
            {
                sqlCn = OpenConnection();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandTimeout = 2000;
                sqlCmd.Connection = sqlCn;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = TenStoreProcedure;
                for (int i = 0; i < sqlParam.Length; i++)
                {
                    sqlCmd.Parameters.Add(sqlParam[i]);
                }
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = sqlCmd;
                sqlDA.Fill(dtbTmp);
                CloseConnection(sqlCn);
            }
            catch (SqlException sqlEx)
            {
                strErrorMessage = sqlEx.Message;
                intErrorNumber = sqlEx.Number;
            }
            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();

            }
            return dtbTmp;
        }
        /// <summary>
        /// Thực thi 1 StoreProcedure không trả lại giá trị
        /// </summary>
        public void ExecStoreProcedure(string TenStoreProcedure, SqlParameter[] sqlParam)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandTimeout = 2000;
                sqlCn = OpenConnection();
                sqlCmd.Connection = sqlCn;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = TenStoreProcedure;
                for (int i = 0; i < sqlParam.Length; i++)
                {
                    sqlCmd.Parameters.Add(sqlParam[i]);
                }
                sqlCmd.ExecuteNonQuery();
                CloseConnection(sqlCn);
            }
            catch (SqlException sqlEx)
            {
                strErrorMessage = sqlEx.Message;
                intErrorNumber = sqlEx.Number;
            }

            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();
            }
        }
        /// <summary>
        /// Thực thi 1 câu lệnh SQL dạng select có trả về dữ liệu là 1 bảng
        /// </summary>
        /// <return>
        /// DataTable
        /// </return>>
        public DataTable SQL_query_to_DataTable(string strSQL)
        {
            sqlCn = OpenConnection();
            SqlDataAdapter Adapter = new SqlDataAdapter(strSQL, sqlCn);
            DataTable ds = new DataTable();
            try
            {
                Adapter.Fill(ds);
                CloseConnection(sqlCn);
            }
            catch (SqlException E)
            {
                string strDescriptionError = E.Message;
            }
            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();
            }
            return ds;

        }
        /// <summary>
        /// Thực thi 1 câu lệnh SQL dạng select có trả về dữ liệu là DataSet
        /// </summary>
        /// <return>
        /// DataSet
        /// </return>>
        public DataSet SQL_query_ToDataSet(string strSQL)
        {
            sqlCn = OpenConnection();
            SqlDataAdapter Adapter = new SqlDataAdapter(strSQL, sqlCn);
            DataSet ds = new DataSet();
            try
            {
                Adapter.Fill(ds);
                CloseConnection(sqlCn);
            }
            catch (SqlException E)
            {
                string strDescriptionError = E.Message;
            }
            return ds;
        }
        /// <summary>
        /// Thực thi 1 câu lệnh SQL dạng select không trả lại giá trị
        /// </summary>
        public string ExecSQL(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandTimeout = 2000;
            sqlCn = OpenConnection();
            sqlCommand.Connection = sqlCn;
            sqlCommand.Parameters.Clear();
            sqlCommand.CommandText = strSQL;
            sqlCommand.CommandType = CommandType.Text;
            try
            {
                intErrorNumber = sqlCommand.ExecuteNonQuery();
                mErrorCode = 0;
                mErrorMsg = "";
                CloseConnection(sqlCn);
            }
            catch (SqlException ex)
            {
                intErrorNumber = ex.Number;
                mErrorMsg = ex.Message;
                System.Console.Write(ex.StackTrace);
            }
            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();
            }
            return mErrorMsg;

        }
        public string ExecSQL_GetFirstValue(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandTimeout = 2000;
            sqlCn = OpenConnection();
            sqlCommand.Connection = sqlCn;
            sqlCommand.Parameters.Clear();
            sqlCommand.CommandText = strSQL;
            sqlCommand.CommandType = CommandType.Text;
            string Result = "";
            try
            {
                Result = sqlCommand.ExecuteScalar().ToString();
                mErrorCode = 0;
                mErrorMsg = "";
                CloseConnection(sqlCn);
            }
            catch (SqlException ex)
            {
                intErrorNumber = ex.Number;
                mErrorMsg = ex.Message;
                System.Console.Write(ex.StackTrace);
            }
            finally
            {
                if (sqlCn.State == ConnectionState.Open)
                    sqlCn.Close();
                sqlCn.Dispose();
            }
            return Result;

        }
    }
}