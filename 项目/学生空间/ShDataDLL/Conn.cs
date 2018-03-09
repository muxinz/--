using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using ShErrorDeal;

namespace ShDataDLL
{
    /// <summary>
    /// ���ݿ������
    /// </summary>
    public class Conn
    {
        private SqlConnection _conn;
        private bool _connected = false;
        private IDbTransaction _transaction = null;
        private IDbCommand _transCommand = null;
        public string ConnStr = "";

        #region ��˼Զ�� ִ�д洢���̣�����һ�� DataSet ���� public DataSet GetDataTable(string procName, SqlParameter[] parameters)
        /// <summary>
        /// ִ�д洢���̣�����һ�� DataTable ����
        /// </summary>
        public DataSet GetDataSet(string procName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(procName, _conn as SqlConnection);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter parameter in parameters)
                {
                    adapter.SelectCommand.Parameters.Add(parameter);
                }
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, procName, "Conn_GetDataSet_2");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "Conn_GetDataSet_2", 0);

                ds = null;
            }
            return ds;
        }
        #endregion

        //#region С�� ִ�д洢���̣�����һ��DataSet���� public DataSet GetDataSet(string procName, params SqlParameter[] parameters)
        ///// <summary>
        ///// ִ�д洢���̣�����һ��DataSet ����
        ///// </summary>
        //public DataSet GetDataSet(string procName, params SqlParameter[] parameters)
        //{
        //    SqlDataAdapter adapter = new SqlDataAdapter(procName, _conn as SqlConnection);
        //    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
        //    foreach (SqlParameter parameter in parameters)
        //    {
        //        adapter.SelectCommand.Parameters.Add(parameter);
        //    }
        //    DataSet ds = new DataSet();
        //    adapter.Fill(ds);
        //    return ds;
        //}
        //#endregion 


        //��˼Զд�ķ���

        public DataSet GetDataSet(List<string> sqls, List<string> Dt_Name)
        {
            string SqlText = "";
            try
            {
                DataSet ds = new DataSet();
                for (int i = 0; i < sqls.Count; i++)
                {
                    SqlText = sqls[i];
                    SqlDataAdapter da = new SqlDataAdapter(SqlText, _conn);
                    da.Fill(ds, Dt_Name[i]);
                }
                return ds;
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, SqlText, "Conn_GetDataSet");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "Conn_GetDataSet", 0);
                return null;
            }
        }


        //СӢ������
        public DataTable Proc_GetDataTable(string sql)//����Ǵ���һ����������䣬���ʹ̶��洢���̣�����һ��datatable��Ŀ���ǲ�ѯ�Ĳ���Ҫ�������open key ,close key ��
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da_getall = new SqlDataAdapter("GetQueryData", _conn);
                da_getall.SelectCommand.CommandType = CommandType.StoredProcedure;
                da_getall.SelectCommand.Parameters.Add("@sqls", SqlDbType.VarChar, -1);
                da_getall.SelectCommand.Parameters["@sqls"].Value = sql;
                da_getall.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, sql, "Proc_GetDataTable");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "Proc_GetDataTable", 0);
                return null;
            }
        }

        //СӢ������
        public DataSet Proc_GetDataSet(List<string> sql, List<string> tablename)//����sql���List����Ӧ����List������Ǵ��ݶ����ѯ����Զ�Ӧ�ı��������ʹ̶��洢���̣�����һ��dataset��Ŀ���ǲ�ѯ�Ĳ���Ҫ�������open key ,close key ��
        {
            string sqls = "";
            try
            {
                DataSet ds = new DataSet();
                for (int i = 0; i < sql.Count; i++)
                {
                    sqls = sql[i];
                    SqlDataAdapter da_getall = new SqlDataAdapter("GetQueryData", _conn);
                    da_getall.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da_getall.SelectCommand.Parameters.Add("@sqls", SqlDbType.VarChar, -1);
                    da_getall.SelectCommand.Parameters["@sqls"].Value = sqls;
                    da_getall.Fill(ds, tablename[i]);
                }
                return ds;
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, sqls, "Proc_GetDataSet");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "Proc_GetDataSet", 0);
                return null;
            }
        }
        //СӢ������  
        #region �ô洢����ִ���������޸ġ��ϴ���ɾ��sql����
        public static string Exec_Sqls(string sql)//ִ�е�������sql��䣬���ش洢����ִ�еĴ���Number���������ֵ�ĳ�����0��ʾִ�гɹ�
        {
            SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString);
            myConn.Open();
            SqlCommand comm = new SqlCommand("sqlUp", myConn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 720;
            comm.Parameters.Add("@sqls", SqlDbType.NVarChar, -1);
            comm.Parameters["@sqls"].Value = sql;
            SqlParameter errorNum = new SqlParameter("@errorNum", SqlDbType.Int);
            errorNum.Direction = ParameterDirection.Output;   //��������ΪReturnValue        
            comm.Parameters.Add(errorNum);
            SqlParameter errorStr = new SqlParameter("@errorStr", SqlDbType.NVarChar, -1);
            errorStr.Direction = ParameterDirection.Output;   //��������ΪReturnValue        
            comm.Parameters.Add(errorStr);
            comm.ExecuteNonQuery();
            myConn.Close();
            string errorerrorNum = errorNum.Value.ToString().Trim();
            return errorerrorNum;
            //if (errorerrorNum.Length > 0)
            //{
            //    Response.Write("����ʧ�ܣ�");
            //}
            //else
            //{
            //    Response.Write("�����ɹ���");
            //}
        }
        #endregion

        #region Ŀǰû����ô��

        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        public Conn()
        {
            _conn = new SqlConnection(ConnectionStrings);
            openconn();
            ConnStr = ConnectionStrings;
        }

        /// <summary>
        /// ���������캯��
        /// </summary>
        /// <param name="connstr"></param>
        public Conn(string connstr)
        {
            _conn = new SqlConnection(connstr);
            openconn();
            ConnStr = connstr;
        }
        #endregion

        public bool Connected
        {
            get
            {
                return _connected;
            }

        }
            
        public  static readonly string CurrenConnectionStrings=ConnectionStrings;
        
        private void openconn()
        {
            if (!_connected)
            {
                try
                {
                    _conn.Open();
                    _connected = true;
                }
                catch(Exception eee)
                {
                    throw eee;
                }
            }
        }

        #region ���ݿ����Ӵ� private string ConnectionStrings
        /// <summary>
        /// ���ݿ����Ӵ�
        /// </summary>
        private static string ConnectionStrings
        {
            get
            {

                return (string)ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

            }
        }
        #endregion

        #region �����ݿ����� public void OpenConn()
        /// <summary>
        /// �����ݿ�����
        /// </summary>
        public void OpenConn()
        {
            if (!_connected)
            {
                try
                {
                    _conn = new SqlConnection(ConnStr);
                    _conn.Open();
                    _connected = true;
                }
                catch
                {
                    throw new Exception("���ݿ�����ʧ��..."); 
                }
            }
        }
        #endregion

        #region �ر����ݿ����� public void CloseConn()
        /// <summary>
        /// �ر����ݿ�����
        /// </summary>
        public void CloseConn()
        {
            if (_connected)
            {
                try
                {
                    _conn.Close();
                    _connected = false;
                }
                catch
                {
                    throw new Exception("���ݿ�ر�ʧ�ܣ�");
                }
            }
        }
        #endregion

        #region ִ��ͳ����������һ����ֵ public int ExecuteCount(string sql)
        /// <summary>
        /// ִ��ͳ������������һ����ֵ
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteCount(string sql)
        {
            SqlCommand _command = new SqlCommand(sql, _conn);
            _command.CommandTimeout = 90;
            return Convert.ToInt32(_command.ExecuteScalar());
        }
        #endregion

        #region ִ�в�ѯ������Ӱ������ public int Execute(string sql)
        /// <summary>
        /// ִ�в�ѯ������Ӱ������
        /// </summary>
        public int Execute(string sql)
        {
            SqlCommand _command = new SqlCommand(sql, _conn);
            _command.CommandTimeout = 90;
            try{
                return Convert.ToInt32(_command.ExecuteNonQuery());
            }
            catch(Exception eee)
            {
                throw eee;
            }
        }
        #endregion

        #region ִ�в�ѯ������һ�� DataTable���� public DataTable GetDataTable(string sql)
        /// <summary>
        /// ִ�в�ѯ������һ�� DataTable����
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
                SqlCommand _command = new SqlCommand(sql, _conn);
                _command.CommandTimeout = 90;
                DataTable dt = new DataTable();
                System.Data.Common.DbDataAdapter Adapter = new System.Data.SqlClient.SqlDataAdapter(_command);
                Adapter.Fill(dt);
                return dt; 
          
        }
        #endregion

        #region ִ�в�ѯ������һ�� DataSet���� public DataSet GetDataSet(string sql)
        /// <summary>
        ///  ִ�в�ѯ������һ�� DataSet����
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand _command = new SqlCommand(sql, _conn);
                _command.CommandTimeout = 90;
                SqlDataAdapter adapter = new SqlDataAdapter(_command);

                adapter.Fill(ds);
            }
            catch 
            {

            }
            return ds;
        }
        #endregion

        #region ִ�в�ѯ�����ص�һ�е�һ��ʵ��ֵ public object GetScalar(string sql)
        /// <summary>
        /// ִ�в�ѯ�����ص�һ�е�һ��ʵ��ֵ
        /// </summary>
        public object GetScalar(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, _conn);
            cmd.CommandTimeout = 90;
            return cmd.ExecuteScalar();
        }
        #endregion

        #region ִ�в�ѯ������һ�� IDataReader ���� public IDataReader GetReader(string sql)
        /// <summary>
        /// ִ�в�ѯ������һ�� SqlDataReader ����
        /// </summary>
        public IDataReader GetReader(string sql)
        {
            IDbCommand cmd = new SqlCommand(sql, _conn);
            cmd.CommandTimeout = 90;
            return cmd.ExecuteReader();
        }
        #endregion

        #region ִ�д洢���̣�����Ӱ������ public int Execute(string procName, SqlParameter[] parameters)
        /// <summary>
        /// ִ�д洢���̣�����Ӱ������
        /// </summary>
        public int Execute(string procName, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(procName, _conn as SqlConnection);
            cmd.CommandTimeout = 90;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            return Convert.ToInt32(cmd.ExecuteNonQuery());
        }
        #endregion

        #region ִ�д洢���̣����ص�һ�е�һ��ʵ����� public object GetScalar(string procName, SqlParameter[] parameters)
        /// <summary>
        /// ִ�д洢���̣����ص�һ�е�һ��ʵ�����
        /// </summary>
        public object GetScalar(string procName, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(procName, _conn as SqlConnection);
            cmd.CommandTimeout = 90;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            return cmd.ExecuteScalar();
        }
        #endregion

        #region ִ�д洢���̣�����һ�� IDataReader ���� public IDataReader GetReader(string procName, SqlParameter[] parameters)
        /// <summary>
        /// ִ�д洢���̣�����һ�� IDataReader ����
        /// </summary>
        public IDataReader GetReader(string procName, params SqlParameter[] parameters)
        {
            IDbCommand cmd = new SqlCommand(procName, _conn as SqlConnection);
            cmd.CommandTimeout = 90;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            return cmd.ExecuteReader();
        }
        #endregion

        #region ִ�д洢���̣�����һ�� DataTable ���� public DataTable GetDataTable(string procName, SqlParameter[] parameters)
        /// <summary>
        /// ִ�д洢���̣�����һ�� DataTable ����
        /// </summary>
        public DataTable GetDataTable(string procName, params SqlParameter[] parameters)
        {

            SqlDataAdapter adapter = new SqlDataAdapter(procName, _conn as SqlConnection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                adapter.SelectCommand.Parameters.Add(parameter);
            }
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }
        #endregion




        #region ��������ѯ��ִ��ͳ����������һ����ֵ public int ExecuteCount(string sql)
        /// <summary>
        /// ִ��ͳ������������һ����ֵ
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteCount(string sql, params IDbDataParameter[] parameters)
        {
            SqlCommand _command = new SqlCommand(sql, _conn);
            _command.CommandTimeout = 90;
            for (int i = 0; i < parameters.Length; i++)
            {
                _command.Parameters.Add(parameters[i]);
            }
            int af = Convert.ToInt32(_command.ExecuteScalar());
            _command.Parameters.Clear();
            return af;
        }
        #endregion        

        #region ��������ѯ������Ӱ������� public int Execute(string commandText, params IDbDataParameter[] parameters)
        /// <summary> 
        /// ����Ӱ�����������������ѯ
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Execute(string commandText, params IDbDataParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(commandText, _conn);
            cmd.CommandTimeout = 90;
            for (int i = 0; i < parameters.Length; i++)
            {
                cmd.Parameters.Add(parameters[i]);
            }
            return cmd.ExecuteNonQuery();
        }
        #endregion

        #region ��������ѯ������DataTable public DataTable GetDataTable(string commandText, params IDbDataParameter[] parameters)
        /// <summary>
        /// ��������ѯ������DataTable
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string commandText, params IDbDataParameter[] parameters)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, _conn);
            foreach (IDbDataParameter parameter in parameters)
            {
                adapter.SelectCommand.Parameters.Add(parameter);
            }
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }
        #endregion

        #region ��������ѯ�����ص�һ�е�һ��object���� public object GetScalar(string commandText, params IDbDataParameter[] parameters)
        /// <summary>
        /// ��������ѯ�����ص�һ�е�һ��object����
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetScalar(string commandText, params IDbDataParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(commandText, _conn);
            cmd.CommandTimeout = 90;
            foreach (IDbDataParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            return cmd.ExecuteScalar();
        }
        #endregion

        #region  ��������ѯ������IDataReader public IDataReader GetReader(string commandText, params IDbDataParameter[] parameters)
        /// <summary>
        /// ��������ѯ������IDataReader
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader GetReader(string commandText, params IDbDataParameter[] parameters)
        {
            IDbCommand cmd = new SqlCommand(commandText, _conn);
            cmd.CommandTimeout = 90;
            for (int i = 0; i < parameters.Length; i++)
            {
                cmd.Parameters.Add(parameters[i]);
            }
            return cmd.ExecuteReader();
        }
        #endregion

        #region ����IDbDataParameter�Ĳ���
        /// <summary>
        /// ����IDbDataParameter�Ĳ���
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string parameterName, DbType dbType, object value)
        {
            SqlParameter retvl = new SqlParameter();
            retvl.ParameterName = parameterName;
            retvl.DbType = dbType;
            retvl.Value = value;
            return retvl;
        }
        #endregion

        #region ����SqlParameter�Ĳ���
        /// <summary>
        /// ����SqlParameter�Ĳ���
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter CreateParameter(string parameterName, SqlDbType SqldbType, object value)
        {
            SqlParameter retvl = new SqlParameter();
            retvl.ParameterName = parameterName;
            retvl.SqlDbType  = SqldbType;
            retvl.SqlValue  = value;
            return retvl;
        }
        #endregion


        #region �������
        public IDbTransaction m_transaction {
            get {
                return _transaction;
            }
        }
        public IDbCommand m_transCommand
        {
            get
            {
                return _transCommand;
            }
        }
        #endregion

        #region ���������ز���
        /// <summary>
        /// ��ʼ����
        /// </summary>
        public void BeginTransaction()
        {
            _transaction = _conn.BeginTransaction();
        }
        /// <summary>
        /// �ύ����
        /// </summary>
        public void Commit()
        {
            _transaction.Commit();
            if (_transCommand != null)
            {
                _transCommand.Dispose();
            }
            _transaction.Dispose();
        }
        /// <summary>
        /// ����ع�
        /// </summary>
        public void Rollback()
        {
            _transaction.Rollback();
            if (_transCommand != null)
            {
                _transCommand.Dispose();
            }
            _transaction.Dispose();
        }

        /// <summary>
        /// ����ִ�в���
        /// </summary>
        /// <param name="commandText"></param>
        public void ExecuteTransaction(string commandText)
        {
            
            if (null == _transCommand)
            {
                _transCommand = new SqlCommand(commandText, _conn);
                _transCommand.Transaction = _transaction;
            }
            else
            {
                _transCommand.CommandText = commandText;
            }
            _transCommand.ExecuteNonQuery();
        }
        /// <summary>
        /// ִ�в���(��������
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        public void ExecuteTransaction(string commandText, params IDbDataParameter[] parameters)
        {
            if (null == _transCommand)
            {
                _transCommand = new SqlCommand(commandText, _conn);
                _transCommand.Transaction = _transaction;
            }
            else
            {
                _transCommand.CommandText = commandText;
            }

            _transCommand.Parameters.Clear();
            for (int i = 0; i < parameters.Length; i++)
            {
                _transCommand.Parameters.Add(parameters[i]);
            }
            _transCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// ִ�в��� ���ص�һ�е�һ��
        /// </summary>
        /// <param name="commandText"></param>
        public object ExecuteTransactionScalar(string commandText){
            if (null == _transCommand)
            {
                _transCommand = new SqlCommand(commandText, _conn);
                _transCommand.Transaction = _transaction;
            }
            else
            {
                _transCommand.CommandText = commandText;
            }
            return _transCommand.ExecuteScalar();
        }
        /// <summary>
        /// ִ�в��� ���ص�һ�е�һ��(��������
        /// </summary>
        /// <param name="commandText"></param>
        public object ExecuteTransactionScalar(string commandText, params IDbDataParameter[] parameters)
        {
            if (null == _transCommand)
            {
                _transCommand = new SqlCommand(commandText, _conn);
                _transCommand.Transaction = _transaction;
            }
            else
            {
                _transCommand.CommandText = commandText;
            }

            _transCommand.Parameters.Clear();
            for (int i = 0; i < parameters.Length; i++)
            {
                _transCommand.Parameters.Add(parameters[i]);
            }
            return _transCommand.ExecuteScalar();
        }

        public DataTable getDataTableCommand(string sql)
        {
            if (null == _transCommand)
            {
                _transCommand = new SqlCommand(sql, _conn);
                _transCommand.Transaction = _transaction;
            }
            else
            {
                _transCommand.CommandText = sql;
 
            }
            DataTable dt = new DataTable();
            System.Data.Common.DbDataAdapter Adapter = new System.Data.SqlClient.SqlDataAdapter((SqlCommand)_transCommand);
            Adapter.Fill(dt);
            return dt; 
        }
        #endregion
        #endregion

    }
}
