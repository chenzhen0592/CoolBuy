using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CoolBuy.Data
{
    public interface IDatabaseConnect
    {
        void BeginTransaction();
        void Commit();
        void RollBack();
        bool ExecuteNoQuery(string sql, DynamicParameters parameters);
        void ExecuteNoQuery<T>(string sql, T model);
        List<T> ExecuteList<T>(string sql, DynamicParameters parameters);
        T ExecuteSingle<T>(string sql, DynamicParameters parameters);

        List<T> ExecProc<T>(string processName, DynamicParameters parameters);
        void ExecProc(string processName, DynamicParameters parameters);

        T ExecProcReturnSingle<T>(string processName, DynamicParameters parameter);
    }

    public class MySqlConnect : IDatabaseConnect
    {
        private MySqlConnection  sqlConnection = null;


        public static string ConnectString { get; set; } = ConfigurationManager.AppSettings["connectionstring"];

        public MySqlConnect()
        {
            //ConnectString = config.GetConnectionString("MSSQL");
        }

        public MySqlConnect(string connectString)
        {
            //ConnectString = config.GetConnectionString("MSSQL");
        }

        private bool Open()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new MySqlConnection(ConnectString);
                sqlConnection.Open();
            }
            else if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
            else if (sqlConnection.State == ConnectionState.Broken)
            {
                sqlConnection.Close();
                sqlConnection = new MySqlConnection(ConnectString);
                sqlConnection.Open();
            }
            return sqlConnection.State == ConnectionState.Open;
        }
        private void Close()
        {
            if (!isTransactionRun)
                Dispose();
        }


        private MySqlConnection Connection
        {
            get
            {
                if (sqlConnection == null)
                {
                    sqlConnection = new MySqlConnection(ConnectString);
                }
                return sqlConnection;
            }
            set { sqlConnection = null; }
        }

        private IDbTransaction transaction;
        protected IDbTransaction Transaction
        {
            get
            {
                if (null == transaction)
                    BeginTransaction();
                return transaction;
            }
        }

        int transactionCount = 0;
        private bool isTransactionRun;
        private bool isRollBack = false;

        public void BeginTransaction()
        {
            if (null == transaction)
            {
                try
                {
                    Open();
                    transaction = this.Connection.BeginTransaction();
                    isTransactionRun = true;
                }
                catch (Exception ex)
                {
                    Close();
                    throw ex;
                }
            }
            transactionCount++;
        }

        public void Commit()
        {
            transactionCount--;
            if (transactionCount <= 0)
            {
                try
                {
                    if (!isRollBack)
                        this.transaction.Commit();
                    else
                        this.transaction.Rollback();
                }
                finally
                {
                    DisposeTransaction();
                }
            }
        }

        public void Dispose()
        {
            MySqlConnection conn = this.Connection;
            if (conn != null && !isTransactionRun)
            {
                this.Connection = null;
                transactionCount = 0;
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                conn.Dispose();
            }
        }

        public void RollBack()
        {
            isRollBack = true;
            transactionCount--;
            if (transactionCount <= 0)
            {
                try
                {
                    this.transaction.Rollback();
                }
                finally
                {
                    DisposeTransaction();
                }
            }
        }

        private void DisposeTransaction()
        {
            try
            {
                if (null != transaction)
                    transaction.Dispose();
                transaction = null;
                isTransactionRun = false;
                if (transactionCount != 0)
                    transactionCount = 0;
                if (isRollBack)
                    isRollBack = false;
                Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// create,update,Delete operate
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ExecuteNoQuery(string sql, DynamicParameters parameters)
        {
            int result = Connection.Execute(sql, parameters, transaction);
            return result > 0;
        }

        /// <summary>
        ///  create,update,Delete operate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="model"></param>
        public void ExecuteNoQuery<T>(string sql, T model)
        {
            Connection.Execute(sql, model, transaction);
        }
        /// <summary>
        /// return a list 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(string sql, DynamicParameters parameters)
        {
            return Connection.Query<T>(sql, parameters, transaction).AsList();
        }
        /// <summary>
        /// return single Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteSingle<T>(string sql, DynamicParameters parameters)
        {
            //return (T) Connection.QuerySingleOrDefault(sql, parameters);
            return (T)Connection.QueryFirstOrDefault<T>(sql, parameters, transaction);
        }
        public List<T> ExecProc<T>(string processName, DynamicParameters parameters)
        {
            return Connection.Query<T>(processName, parameters, null, false, null, CommandType.StoredProcedure).AsList();
        }

        public void ExecProc(string processName, DynamicParameters parameters)
        {
            Connection.Query(processName, parameters, null, true, null, CommandType.StoredProcedure);
        }

        public T ExecProcReturnSingle<T>(string processName, DynamicParameters parameters)
        {
            return Connection.QueryFirst<T>(processName, parameters, null, 3000, CommandType.StoredProcedure);
        }

    }

    public class MSSqlConnect: IDatabaseConnect
    {
        private SqlConnection sqlConnection = null;


        public static string ConnectString { get; set; } = ConfigurationManager.AppSettings["connectionstring"];

        public MSSqlConnect()
        {
            //ConnectString = config.GetConnectionString("MSSQL");
        }

        public MSSqlConnect(string connectString)
        {
            //ConnectString = config.GetConnectionString("MSSQL");
        }

        private bool Open()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(ConnectString);
                sqlConnection.Open();
            }
            else if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
            else if (sqlConnection.State == ConnectionState.Broken)
            {
                sqlConnection.Close();
                sqlConnection = new SqlConnection(ConnectString);
                sqlConnection.Open();
            }
            return sqlConnection.State == ConnectionState.Open;
        }
        private void Close()
        {
            if (!isTransactionRun)
                Dispose();
        }
        

        private SqlConnection Connection
        {
            get
            {
                if (sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(ConnectString);
                }
                return sqlConnection;
            }
            set { sqlConnection = null; }
        }

        private IDbTransaction transaction;
        protected IDbTransaction Transaction
        {
            get
            {
                if (null == transaction)
                    BeginTransaction();
                return transaction;
            }
        }
        
        int transactionCount = 0;
        private bool isTransactionRun;
        private bool isRollBack = false;

        public void BeginTransaction()
        {
            if (null == transaction)
            {
                try
                {
                    Open();
                    transaction = this.Connection.BeginTransaction();
                    isTransactionRun = true;
                }
                catch (Exception ex)
                {
                    Close();
                    throw ex;
                }
            }
            transactionCount++;
        }

        public void Commit()
        {
            transactionCount--;
            if (transactionCount <= 0)
            {
                try
                {
                    if (!isRollBack)
                        this.transaction.Commit();
                    else
                        this.transaction.Rollback();
                }
                finally
                {
                    DisposeTransaction();
                }
            }
        }

        public void Dispose()
        {
            SqlConnection conn = this.Connection;
            if (conn != null && !isTransactionRun)		
            {
                this.Connection = null;
                transactionCount = 0;
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                conn.Dispose();
            }
        }

        public void RollBack()
        {
            isRollBack = true;
            transactionCount--;
            if (transactionCount <= 0)
            {
                try
                {
                    this.transaction.Rollback();
                }
                finally
                {
                    DisposeTransaction();
                }
            }
        }

        private void DisposeTransaction()
        {
            try
            {
                if (null != transaction)
                    transaction.Dispose();
                transaction = null;
                isTransactionRun = false;
                if (transactionCount != 0)
                    transactionCount = 0;
                if (isRollBack)
                    isRollBack = false;
                Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// create,update,Delete operate
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public  bool ExecuteNoQuery(string sql, DynamicParameters parameters)
        {
            int result= Connection.Execute(sql, parameters, transaction);
            return result > 0;
        }

        /// <summary>
        ///  create,update,Delete operate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="model"></param>
        public void ExecuteNoQuery<T>(string sql, T model)
        {
            Connection.Execute(sql, model, transaction);
        }
        /// <summary>
        /// return a list 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(string sql, DynamicParameters parameters)
        {
            return Connection.Query<T>(sql, parameters,transaction).AsList();
        }
        /// <summary>
        /// return single Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteSingle<T>(string sql, DynamicParameters parameters)
        {
            //return (T) Connection.QuerySingleOrDefault(sql, parameters);
            return (T)Connection.QueryFirstOrDefault<T>(sql, parameters,transaction);
        }
        public List<T> ExecProc<T>(string processName, DynamicParameters parameters)
        {
            return Connection.Query<T>(processName, parameters, null, false, null, CommandType.StoredProcedure).AsList();
        }

        public void ExecProc(string processName,DynamicParameters parameters)
        {
            Connection.Query(processName, parameters,  null, true, null, CommandType.StoredProcedure);
        }

        public T ExecProcReturnSingle<T>(string processName, DynamicParameters parameters)
        {
           return Connection.QueryFirst<T>(processName, parameters, null,3000, CommandType.StoredProcedure);
        }
  
    }


    /// <summary>
    /// Different thread get different DataBaseConnect
    /// </summary>
    public class DataBus
    {
        [ThreadStatic]
        private static IDatabaseConnect ThreadDatabaseConnect;

        private static string UseDb= ConfigurationManager.AppSettings["usedb"];
        public static IDatabaseConnect GetDataBase()
        {
            if (ThreadDatabaseConnect == null)
            {
                switch (UseDb.ToLower())
                {
                    case "mysql":
                        ThreadDatabaseConnect = new MySqlConnect();
                        break;
                    default:
                        ThreadDatabaseConnect = new MSSqlConnect();
                        break;
                }
                
            }
            return ThreadDatabaseConnect;
        }
    }
}
