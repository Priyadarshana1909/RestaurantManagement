using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace RestaurantDAL
{

    public sealed class SqlHelper
    {
        #region Constants
        public const string CONN_STRING_KEY = "ConnectionString";
        #endregion

        public sealed class ConnectionInfo
        {
            private string m_strServer = "";
            private string m_strDatabase = "";
            private string m_strUserId = "";
            private string m_strPassword = "";

            internal ConnectionInfo(string connectionString)
            {
                SqlConnectionStringBuilder conBuilder = new SqlConnectionStringBuilder(connectionString);
                m_strServer = conBuilder.DataSource;
                m_strDatabase = conBuilder.InitialCatalog;
                m_strUserId = conBuilder.UserID;
                m_strPassword = conBuilder.Password;
            }

            public string Server
            {
                get { return m_strServer; }
                internal set { m_strServer = value; }
            }
            public string Database
            {
                get { return m_strDatabase; }
                internal set { m_strDatabase = value; }
            }
            public string UserId
            {
                get { return m_strUserId; }
                internal set { m_strUserId = value; }
            }
            public string Password
            {
                get { return m_strPassword; }
                internal set { m_strPassword = value; }
            }
        }

        #region Private Members
        private static string m_SUserID;
        private static string strConnectionString = "";
        public static SqlConnection objSqlConnection;
        private static SqlTransaction objSqlTransaction;

        #endregion // Private Members


        #region Properties
        /// <summary>
        /// User ID
        /// </summary>
        public static string UserID
        {
            get { return m_SUserID; }
            set { m_SUserID = value; }
        }
        // SqlConnectionStringBuilder

        /// <summary>
        /// ConnectionString
        /// </summary>
        /// <remarks>
        /// History	:
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static string ConnectionString
        {
            set
            {
                strConnectionString = value;
            }
        }
        #endregion // Properties


        # region Constructors
        /// <summary>
        /// Since this class provides only static methods, make the default constructor 
        /// private to prevent instances from being created with "new SqlHelper()".
        /// </summary>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private SqlHelper()
        {
        }
        #endregion // Constructors


        #region Public Functions


        /// <summary>
        /// Opens new connection and creates a transaction
        /// </summary>
        /// <param name="connectionString"></param>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///  
        /// </remarks>
        public static void InitializeTransaction(string connectionString)
        {
            objSqlConnection = new SqlConnection(connectionString);
            objSqlConnection.Open();
            objSqlTransaction = objSqlConnection.BeginTransaction();
            SqlHelperParameterCache.SqlConn = new SqlConnection(connectionString);
            SqlHelperParameterCache.SqlConn.Open();
        }


        /// <summary>
        /// Rollbacks any changes done since InitializeTransaction
        /// </summary>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///  
        /// </remarks>
        public static void RollBack()
        {
            objSqlTransaction.Rollback();
            objSqlConnection.Close();
            objSqlConnection = null;
        }


        /// <summary>
        /// Commits any changes done since InitializeTransaction
        /// </summary>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///  
        /// </remarks>
        public static void Commit()
        {
            objSqlTransaction.Commit();
            objSqlConnection.Close();
            objSqlConnection = null;
        }


        /// <summary>
        /// Used to execute a Procedure and return values from OUTPUT parameters of the proc
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="arrListIn"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static ArrayList ExecuteReturnArrayList(string connectionString, string spName, ArrayList arrListIn)
        {
            if (objSqlConnection != null)
                return ExecuteReturnArrayList(objSqlConnection, spName, arrListIn);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                return ExecuteReturnArrayList(cn, spName, arrListIn);
            }
        }


        /// <summary>
        /// Following function is added by Jayesh Tankariya on 11-05-2015 to work with ACID property of database (Either all operations are done or none.)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commands"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public static bool ExecuteTransaction(string strConnection, ArrayList commands, List<KeyValuePair<string, object>> param = null)
        {
            bool bStatus = false;
            SqlConnection connection = new SqlConnection(strConnection);
            //string[] lines = commands; // regex.Split(sql);
            bool useTransaction = true;
            SqlTransaction transaction = null;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            if (useTransaction)
                transaction = connection.BeginTransaction();
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.Connection = connection;
                if (useTransaction)
                    cmd.Transaction = transaction;

                foreach (string line in commands)
                {
                    if (line.Length > 0)
                    {
                        cmd.CommandText = line;
                        if (param != null)
                        {
                            for (var i = 0; i < param.Count; i++)
                            {
                                if (cmd.CommandText.Contains(param[i].Key))
                                    cmd.Parameters.Add(param[i].Key, SqlDbType.Binary).Value = (byte[])param[i].Value;
                            }
                        }
                        cmd.CommandType = CommandType.Text;

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            string msg = e.Message;
                            if (useTransaction)
                                transaction.Rollback();
                            throw;
                        }
                    }
                }
                bStatus = true;
            }
            if (bStatus && useTransaction)
                transaction.Commit();

            return bStatus;
        }


        /// <summary>
        /// Used to execute a Procedure and return values from OUTPUT parameters of the proc
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="spName"></param>
        /// <param name="arrListIn"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///
        /// </remarks>
        public static ArrayList ExecuteReturnArrayList(SqlConnection connection, string spName, ArrayList arrListIn)
        {
            ArrayList arrList = new ArrayList();
            if ((arrListIn != null) && (arrListIn.Count > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName, false);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, arrListIn);

                //Attach Parameters
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;

                //if the transaction exists, assign it.
                if (objSqlTransaction != null)
                {
                    cmd.Transaction = objSqlTransaction;
                }

                foreach (SqlParameter param in commandParameters)
                {
                    if (param.Direction == ParameterDirection.InputOutput || param.Direction == ParameterDirection.Input)
                    {
                        if (param.DbType == DbType.DateTime)
                        {
                            DateTime dtTemp = DateTime.MinValue;
                            try
                            {
                                dtTemp = DateTime.Parse(param.Value.ToString());
                            }
                            catch
                            {
                                dtTemp = DateTime.MinValue;
                            }
                            if (dtTemp == DateTime.MinValue)
                                param.Value = System.Data.SqlTypes.SqlDateTime.Null;
                        }
                        cmd.Parameters.Add(param);
                    }
                }
                try
                {
                    cmd.ExecuteNonQuery();
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        arrList.Add(param.Value);
                    }
                }
                catch (SqlException e)
                {
                    string strmsg = e.Message;
                }
            }
            return arrList;
        }


        /// <summary>
        /// Used to execute a Procedure and return values from OUTPUT parameters of the proc
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="spName"></param>
        /// <param name="arrListIn"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///
        /// </remarks>
        public static ArrayList ExecuteReturnArrayList(string connectionString, string spName, params object[] parameterValues)
        {
            if (objSqlConnection != null)
                return ExecuteReturnArrayList(objSqlConnection, spName, parameterValues);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                return ExecuteReturnArrayList(cn, spName, parameterValues);
            }
        }


        /// <summary>
        /// Used to execute a Procedure and return values from OUTPUT parameters of the proc
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="spName"></param>
        /// <param name="arrListIn"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static ArrayList ExecuteReturnArrayList(SqlConnection connection, string spName, params object[] parameterValues)
        {
            ArrayList arrListIn = new ArrayList();
            foreach (object x in parameterValues)
            {
                arrListIn.Add(x);
            }
            return ExecuteReturnArrayList(connection, spName, arrListIn);
        }


        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (objSqlConnection != null)
                return ExecuteNonQuery(objSqlConnection, commandType, commandText, commandParameters);

            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        ///  e.g.:  
        ///	int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameterValues"></param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        ///	<remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                //SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, false);

                ////assign the provided values to these parameters based on parameter order
                //AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, (SqlParameter[])parameterValues);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///
        /// </remarks>

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, objSqlTransaction, commandType, commandText, commandParameters);
            //AttachParameters(cmd, commandParameters);
            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }


        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///	
        /// </remarks>
        public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }


        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connection">SQL Connection object</param>
        /// <param name="transaction">SQL Transaction object</param>
        /// <param name="commandType">Command Type</param>
        /// <param name="TimeOut">Command timeout in minutes</param>
        /// <param name="commandText">SP Name</param>
        /// <param name="commandParameters">Params</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///	
        /// </remarks>
        public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, CommandType commandType, int TimeOut, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            //Set command object timeout. Multiply * 60 to convert passed in minutes in to seconds required by CommandTimeout property
            cmd.CommandTimeout = TimeOut * 60;
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }


        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters. First thing it does, it inserts a record with the
        /// user info into the session table.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="sUserID"></param>
        /// <param name="parameterValues"></param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        //public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, string sUserID, params object[] parameterValues)
        //{
        //    int retval = 0;
        //    SqlConnection conn;

        //    if (objSqlConnection != null)
        //        conn = objSqlConnection;
        //    else
        //    {
        //        conn = new SqlConnection(connectionString);
        //        conn.Open();
        //    }
        //    //First Insert the connection ID into session table
        //    //ExecuteNonQuery(conn, CommandType.Text,"p_insert_db_session_info '" + sUserID + "',0");

        //    if (commandType == CommandType.Text)
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = conn;
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandText = commandText;
        //        if (objSqlTransaction != null)
        //        {
        //            cmd.Transaction = objSqlTransaction;
        //        }

        //        retval = cmd.ExecuteNonQuery();
        //    }
        //    else
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, commandText, false);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of SqlParameters
        //        retval = ExecuteNonQuery(conn, CommandType.StoredProcedure, commandText, commandParameters);
        //    }

        //    if (objSqlConnection == null)
        //    {
        //        conn.Close();
        //        conn = null;
        //    }
        //    return retval;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="UpdatedDataSet"></param>
        /// <param name="SQL"></param>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static void UpdateDS(string ConnectionString, DataSet UpdatedDataSet, string SQL)
        {
            if (objSqlConnection != null)
                UpdateDS(objSqlConnection, UpdatedDataSet, SQL, true);

            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                cn.Open();

                UpdateDS(cn, UpdatedDataSet, SQL, true);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="UpdatedDataSet"></param>
        /// <param name="SQL"></param>
        /// <param name="setSessionInfo"></param>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static void UpdateDS(SqlConnection connection, DataSet UpdatedDataSet, string SQL, bool setSessionInfo)
        {
            SqlDataAdapter dAdapter;
            SqlCommandBuilder dBuilder;

            //First Insert the connection ID into session table
            if (setSessionInfo)
            {
                //ExecuteNonQuery(connection, CommandType.Text,"p_insert_db_session_info '" + m_SUserID + "',0");
            }

            dAdapter = new SqlDataAdapter();
            dAdapter.SelectCommand = new SqlCommand(SQL, connection);

            dBuilder = new SqlCommandBuilder(dAdapter);
            SqlCommand sC = dBuilder.GetInsertCommand();
            Console.WriteLine(sC.CommandText);

            dAdapter.Update(UpdatedDataSet, UpdatedDataSet.Tables[0].TableName);
        }


        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// e.g.:  
        /// DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (objSqlConnection != null)
                return ExecuteDataset(objSqlConnection, commandType, commandText, commandParameters);

            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        /// DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameterValues"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                //SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                ////assign the provided values to these parameters based on parameter order
                //AssignParameterValues(commandParameters, parameterValues);



                //call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, (SqlParameter[])parameterValues);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        /// DataSet ds = ExecuteDataset(connString,10,"GetOrders", 24, 36);
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="timeout"></param>
        /// <param name="spName"></param>
        /// <param name="parameterValues"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static DataSet ExecuteDataset(string connectionString, int timeout, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, timeout, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteDataset(connectionString, timeout, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 65535;
            PrepareCommand(cmd, connection, objSqlTransaction, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // detach the SqlParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// DataSet ds = ExecuteDataset(conn,10,CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        /// /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static DataSet ExecuteDataset(SqlConnection connection, int timeout, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = timeout * 60;
            PrepareCommand(cmd, connection, objSqlTransaction, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // detach the SqlParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// DataSet ds = ExecuteDataset(conn,CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        public static DataSet ExecuteDataset(string connectionString, int timeout, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (objSqlConnection != null)
                return ExecuteDataset(objSqlConnection, commandType, commandText, commandParameters);

            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, timeout, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// e.g.:  
        /// int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (objSqlConnection != null)
                return ExecuteScalar(objSqlConnection, commandType, commandText, commandParameters);

            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        /// int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameterValues"></param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, objSqlTransaction, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;

        }


        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// e.g.:  
        /// int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static object ExecuteScalar(SqlConnection connection, int timeout, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            //Set command object timeout. Multiply * 60 to convert passed in minutes in to seconds required by CommandTimeout property
            cmd.CommandTimeout = timeout * 60;
            PrepareCommand(cmd, connection, objSqlTransaction, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static SqlDataReader ExecuteDR(SqlConnection connection, CommandType commandType, string commandText) //, params  SqlParameter[] commandParameters)
        {
            SqlCommand objCommand;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            objCommand = new SqlCommand(commandText, connection);
            return objCommand.ExecuteReader();
        }

        /// <summary>
        /// Returns connection string for given TPA
        /// or ConnectionString property.
        /// </summary>
        /// <param name="strPrefix">Connection Prefix</param>
        /// <returns>Connection string</returns>
        /// <remarks>
        /// History	:
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>

        public static string GetConnectionString(string strPrefix)
        {
            string strKey = strPrefix.Trim() + CONN_STRING_KEY;
            string strConnect = "";// System.Configuration.ConfigurationSettings.AppSettings[strKey];

            // the connection string can be set two different ways
            // option 1: config file, option 2: ConnectionString property
            if (strConnect == null)
            {
                strConnect = strConnectionString;
            }
            else
            {
                strConnectionString = strConnect;
            }
            return strConnect;
        }

        public static ConnectionInfo GetConnectionInfo(string connectionString)
        {
            return new ConnectionInfo(connectionString);
        }
        /// <summary>
        /// Makes the SQL string literals safe from SQL injection attacks
        /// </summary>
        /// <param name="strInputSQL">SQL clause</param>
        /// <returns>SQL clause saving from SQL injection attacks</returns>
        /// <remarks>
        /// Reference: ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/dnnetsec/html/SecNetch12.htm#sqlinjectionattacks
        /// History:
        /// DATE			BY					DESCRIPTION
        /// ---------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static string SafeSqlLiteral(string strInputSQL)
        {
            return strInputSQL.Replace("'", "''");
        }


        /// <summary>
        /// Makes the SQL string literals in a 'LIKE' clause safe from SQL injection attacks
        /// </summary>
        /// <param name="strInputSQL">SQL clause using in the LIKE statement</param>
        /// <returns>SQL clause saving from SQL injection attacks</returns>
        /// <remarks>
        /// Reference: ms-help://MS.VSCC.2003/MS.MSDNQTR.2003APR.1033/dnnetsec/html/SecNetch12.htm#sqlinjectionattacks
        /// History:
        /// DATE			BY					DESCRIPTION
        /// ---------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static string SafeSqlLikeClauseLiteral(string strInputSQL)
        {
            // Make the following replacements:
            // '  becomes  ''
            // [  becomes  [[]
            // %  becomes  [%]
            // _  becomes  [_]

            string strOutputSQL = strInputSQL;
            strOutputSQL = strInputSQL.Replace("'", "''");
            strOutputSQL = strOutputSQL.Replace("[", "[[]");
            strOutputSQL = strOutputSQL.Replace("%", "[%]");
            //strOutputSQL = strOutputSQL.Replace("_", "[_]");  
            return strOutputSQL;
        }


        /// <summary>
        /// Returns SQL Server Date and Time
        /// </summary>
        /// <param name="strConnectionString">ConnectionString</param>
        /// <returns>DateTime</returns>
        /// <remarks>
        /// History:
        /// DATE			BY					DESCRIPTION
        /// ---------------------------------------------------------------------------------
        ///
        /// </remarks>
        public static DateTime GetServerDateTime(string strConnectionString)
        {
            DataSet dSet;
            dSet = SqlHelper.ExecuteDataset(strConnectionString,
                CommandType.Text,
                "SELECT getDate()");
            return DateTime.Parse(dSet.Tables[0].Rows[0][0].ToString());
        }

        /// <summary>
        /// Create SQL job
        /// </summary>
        /// <param name="conString"></param>
        /// <param name="jobInfo"></param>
        /// <returns></returns>
        //public static bool CreateJob(string conString, SqlJobHelper.JobInfo jobInfo)
        //{
        //    return SqlJobHelper.CreateJob(conString, jobInfo);
        //}

        ///// <summary>
        ///// Execute job
        ///// </summary>
        ///// <param name="conString"></param>
        ///// <param name="jobInfo"></param>
        ///// <returns></returns>
        //public static SqlJobHelper.JobInfo.CurrentStatus ExecuteJob(string conString, string jobName)
        //{
        //    return SqlJobHelper.ExecuteJob(conString, jobName);
        //}

        ///// <summary>
        ///// Stop Sql Job
        ///// </summary>
        ///// <param name="conString"></param>
        ///// <param name="jobName"></param>
        ///// <returns></returns>
        //public static SqlJobHelper.JobInfo.CurrentStatus StopJob(string conString, string jobName)
        //{
        //    return SqlJobHelper.StopJob(conString, jobName);
        //}
        ///// <summary>
        ///// Execute job
        ///// </summary>
        ///// <param name="conString"></param>
        ///// <param name="jobInfo"></param>
        ///// <returns></returns>
        //public static SqlJobHelper.JobInfo.CurrentStatus GetJobStatus(string conString, string jobName)
        //{
        //    return SqlJobHelper.GetJobStatus(conString, jobName);
        //}

        #endregion // Public Functions


        #region Output parameters - Softweb
        /// <summary>
        /// Used to execute a Procedure and return values from OUTPUT parameters of the proc. In this method we have not used Static variables of SqlTransaction
        /// and SqlConnection. We have passed connectionString and SqlTransaction as parameters.
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <param name="Transaction"></param>
        /// <param name="spName"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///
        /// </remarks>
        public static ArrayList ExecuteReturnArrayList(string connectionString, SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            ArrayList arrListIn = new ArrayList();
            foreach (object x in parameterValues)
            {
                arrListIn.Add(x);
            }
            return ExecuteReturnArrayList(connectionString, transaction, spName, arrListIn);
        }
        public static ArrayList ExecuteReturnArrayList(string connectionString, SqlTransaction transaction, string spName, ArrayList arrListIn)
        {
            ArrayList arrList = new ArrayList();
            if ((arrListIn != null) && (arrListIn.Count > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, arrListIn);

                //Attach Parameters
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = transaction.Connection;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;

                //if the transaction exists, assign it.
                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }

                foreach (SqlParameter param in commandParameters)
                {
                    if (param.Direction == ParameterDirection.InputOutput || param.Direction == ParameterDirection.Input)
                    {
                        if (param.DbType == DbType.DateTime)
                        {
                            DateTime dtTemp = DateTime.MinValue;
                            try
                            {
                                dtTemp = DateTime.Parse(param.Value.ToString());
                            }
                            catch
                            {
                                dtTemp = DateTime.MinValue;
                            }
                            if (dtTemp == DateTime.MinValue)
                                param.Value = System.Data.SqlTypes.SqlDateTime.Null;
                        }
                        cmd.Parameters.Add(param);
                    }
                }
                try
                {
                    cmd.ExecuteNonQuery();
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        arrList.Add(param.Value);
                    }
                }
                catch (SqlException e)
                {
                    string strmsg = e.Message;
                }
            }
            return arrList;
        }
        #endregion


        #region Private Functions
        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of SqlParameters tho be added to command</param>
        /// <remarks>
        /// Change History	:
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }


        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters.
        /// </summary>
        /// <param name="commandParameters">array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            //			if (commandParameters.Length != parameterValues.Length)
            //			{
            //				throw new ArgumentException("Parameter count does not match Parameter Value count.");
            //			}

            //iterate through the SqlParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = parameterValues.Length; i < j; i++)
            {
                if (commandParameters[i].DbType == DbType.DateTime)
                {
                    DateTime dtTemp = DateTime.MinValue;
                    try
                    {
                        dtTemp = DateTime.Parse(parameterValues[i].ToString());
                    }
                    catch { /* ignore format exception, assign null */ }

                    if (dtTemp == DateTime.MinValue)
                    {
                        commandParameters[i].Value = System.Data.SqlTypes.SqlDateTime.Null;
                    }
                    else
                    {
                        commandParameters[i].Value = parameterValues[i];
                    }
                }
                else
                {
                    commandParameters[i].Value = ((SqlParameter)(parameterValues[i])).Value;
                }
            }
        }


        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">the SqlCommand to be prepared</param>
        /// <param name="connection">a valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">a valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <remarks>
        /// Change History	:
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        ///
        /// </remarks>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }


        /// <summary>
        /// Assigns parameter values
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <param name="arrListIN"></param>
        /// <remarks>
        /// Change History	:
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private static void AssignParameterValues(SqlParameter[] commandParameters, ArrayList arrListIN)
        {
            if ((commandParameters == null) || (arrListIN == null))
            {
                //do nothing if we get no data
                return;
            }

            //if (commandParameters.Length != arrListIN.Count)
            //{
            //    throw new ArgumentException("Parameter count does not match Parameter Value count.");
            //}

            //iterate through the SqlParameters, assigning the values from the corresponding position in the 
            //value array
            //for (int i = 0, j = commandParameters.Length; i < j; i++)
            for (int i = 0, j = arrListIN.Count; i < j; i++)
            {
                commandParameters[i].Value = arrListIN[i];
            }
        }

        #endregion // Private Methods
    }

    /// <summary>
    /// SqlHelperParameterCache provides functions to leverage a static cache of 
    /// procedure parameters, and the ability to discover parameters for stored procedures 
    /// at run-time.
    /// </summary>
    /// <remarks>
    /// Created By		: 
    /// Created Date	: 
    /// Change History	:
    /// DATE		BY				DESCRIPTION
    /// ------------------------------------------------------------------------------
    /// 
    /// </remarks>	
    public sealed class SqlHelperParameterCache
    {
        #region Private Members
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
        private static SqlConnection objSqlConnection;
        #endregion


        #region Properties
        /// <summary>
        /// SqlConnection
        /// </summary>
        public static SqlConnection SqlConn
        {
            get { return objSqlConnection; }
            set { objSqlConnection = value; }
        }
        #endregion


        #region Constructors
        /// <summary>
        /// Since this class provides only static methods, make the default constructor 
        /// private to prevent instances from being created with 
        /// "new SqlHelperParameterCache()".
        /// </summary>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private SqlHelperParameterCache()
        {
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command </param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }


        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>Returns an array of SqlParamters</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string hashKey = connectionString + ":" + commandText;

            SqlParameter[] cachedParameters = (SqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }


        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// This method will query the database for this information, and then store 
        /// it in a cache for future requests.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }


        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// This method will query the database for this information, and then store 
        /// it in a cache for future requests.
        /// </summary>
        /// <param name="connection">A valid SqlConnection </param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            if (objSqlConnection != null)
                GetSpParameterSet(objSqlConnection, spName, false);

            return GetSpParameterSet(connection, spName, false);
        }


        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// This method will query the database for this information, and then store it 
        /// in a cache for future requests.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (objSqlConnection != null)
                return GetSpParameterSet(objSqlConnection, spName, includeReturnValueParameter);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                return GetSpParameterSet(cn, spName, includeReturnValueParameter);
            }
        }


        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// This method will query the database for this information, and then store it 
        /// in a cache for future requests.
        /// </summary>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            SqlConnection conn = connection;
            if (objSqlConnection != null)
                conn = objSqlConnection;

            string hashKey = conn.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            SqlParameter[] cachedParameters;

            cachedParameters = (SqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                cachedParameters = (SqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(conn, spName, includeReturnValueParameter));
            }

            return CloneParameters(cachedParameters);
        }


        #endregion Public Methods


        #region Private Methods
        /// <summary>
        /// Resolves at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure </param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (objSqlConnection != null)
                return DiscoverSpParameterSet(objSqlConnection, spName, includeReturnValueParameter);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                return DiscoverSpParameterSet(cn, spName, includeReturnValueParameter);
            }
        }


        /// <summary>
        /// Resolves at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connection">a valid SqlConnection </param>
        /// <param name="spName">the name of the stored procedure </param>
        /// <param name="includeReturnValueParameter">whether or not to include their return value parameter</param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            SqlConnection conn = connection;
            if (objSqlConnection != null)
                conn = objSqlConnection;

            using (SqlCommand cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                return discoveredParameters;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        /// <remarks>
        /// DATE		BY				DESCRIPTION
        /// ------------------------------------------------------------------------------
        /// 
        /// </remarks>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            //deep copy of cached SqlParameter array
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion

    }

    //public sealed class SqlJobHelper
    //{
    //    public class JobInfo
    //    {

    //        string m_strName = "";
    //        string m_strDescription = "";
    //        string m_strCategory = "";

    //        public enum CurrentStatus
    //        {
    //            Idle,
    //            Running,
    //            Suspended,
    //            Unknown
    //        }

    //        public JobInfo()
    //        {

    //        }

    //        public JobInfo(string Name, string Desc, string Category)
    //        {
    //            m_strName = Name;
    //            m_strDescription = Desc;
    //            m_strCategory = Category;
    //        }

    //        List<StepInfo> m_Steps = new List<StepInfo>();

    //        public string Name
    //        {
    //            get { return m_strName; }
    //            set { m_strName = value; }
    //        }

    //        public string Description
    //        {
    //            get { return m_strDescription; }
    //            set { m_strDescription = value; }
    //        }

    //        public string Category
    //        {
    //            get { return m_strCategory; }
    //            set { m_strCategory = value; }
    //        }

    //        internal List<StepInfo> Steps
    //        {
    //            get { return m_Steps; }
    //            set { m_Steps = value; }
    //        }

    //        public void AddStep(StepInfo step)
    //        {
    //            Steps.Add(step);
    //        }

    //        /// <summary>
    //        /// Convert DMO Job status 
    //        /// </summary>
    //        /// <param name="curStatus"></param>
    //        /// <returns></returns>
    //        //internal static CurrentStatus GetStatus(SQLDMO.SQLDMO_JOBEXECUTION_STATUS curStatus)
    //        //{
    //        //    switch (curStatus)
    //        //    {
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_BetweenRetries :
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Executing:
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_WaitingForStepToFinish:
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_WaitingForWorkerThread:
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_PerformingCompletionActions:
    //        //            return CurrentStatus.Running;
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Idle:
    //        //            return CurrentStatus.Idle;
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Suspended:
    //        //            return CurrentStatus.Suspended;
    //        //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Unknown:
    //        //            return CurrentStatus.Unknown;
    //        //        default:
    //        //            return CurrentStatus.Unknown;
    //        //    }
    //        //}

    //        //public static string Status
    //    } // class JobInfo

    //    public class StepInfo
    //    {

    //        public enum SubSystem
    //        {
    //            TSQL,
    //            CmdExec
    //        }

    //        int m_intStepID = 1;
    //        string m_strName = "";
    //        string m_strDatabaseName = "";
    //        SubSystem m_SubSystemType;
    //        string m_strCommand = "";

    //        public StepInfo()
    //        { }

    //        public StepInfo(string Name, SubSystem SubSystemType, string Command)
    //        {
    //            m_strName = Name;
    //            m_SubSystemType = SubSystemType;
    //            m_strCommand = Command;
    //        }

    //        public int StepID
    //        {
    //            get { return m_intStepID; }
    //            set { m_intStepID = value; }
    //        }

    //        public string Name
    //        {
    //            get { return m_strName; }
    //            set { m_strName = value; }
    //        }

    //        public string DatabaseName
    //        {
    //            get { return m_strDatabaseName; }
    //            set { m_strDatabaseName = value; }
    //        }

    //        public string Command
    //        {
    //            get { return m_strCommand; }
    //            set { m_strCommand = value; }
    //        }

    //        public SubSystem SubSystemType
    //        {
    //            get { return m_SubSystemType; }
    //            set { m_SubSystemType = value; }
    //        }
    //    } //class StepInfo

    //    #region Methods
    //    /// <summary>
    //    /// Return DMO Sql Server Object
    //    /// </summary>
    //    /// <param name="conString"></param>
    //    /// <returns></returns>
    //    //private static SQLDMO.JobServer GetJobServer(string conString)
    //    //{
    //    //    SqlConnectionStringBuilder conInfo = new SqlConnectionStringBuilder(conString);

    //    //    SQLDMO._SQLServer SQLServer = new SQLDMO.SQLServerClass();
    //    //    try
    //    //    {
    //    //        SQLServer.Connect(conInfo.DataSource, conInfo.UserID, conInfo.Password);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        throw new Exception(ex.ToString());
    //    //    }

    //    //    switch (SQLServer.JobServer.Status)
    //    //    {
    //    //        case SQLDMO.SQLDMO_SVCSTATUS_TYPE.SQLDMOSvc_Stopped:
    //    //            SQLServer.JobServer.Start();
    //    //            SQLServer.JobServer.AutoStart = true;
    //    //            break;
    //    //    }

    //    //    return SQLServer.JobServer;
    //    //}

    //    ///// <summary>
    //    ///// 
    //    ///// </summary>
    //    ///// <param name="jobName"></param>
    //    ///// <returns></returns>
    //    //internal static SQLDMO.Job GetJob(string conString, string jobName)
    //    //{
    //    //    SQLDMO.JobServer jobServer = GetJobServer(conString);
    //    //    foreach (SQLDMO.Job job in jobServer.Jobs)
    //    //    {
    //    //        if (job.Name.ToLower() == jobName.ToLower())
    //    //            return job;
    //    //    }

    //    //    return null;
    //    //}

    //    //internal static SqlJobHelper.JobInfo.CurrentStatus StopJob(string conString, string jobName)
    //    //{
    //    //    SQLDMO.Job job = GetJob(conString, jobName);
    //    //    if (job == null)
    //    //        throw new Exception("Job does not exist.");

    //    //    SQLDMO.SQLDMO_JOBEXECUTION_STATUS status = job.CurrentRunStatus;
    //    //    switch (status)
    //    //    {
    //    //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_BetweenRetries :
    //    //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Executing :
    //    //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_PerformingCompletionActions:
    //    //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_WaitingForStepToFinish:
    //    //        case SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_WaitingForWorkerThread:
    //    //            job.Stop();
    //    //            break;
    //    //    }
    //    //    return SqlJobHelper.JobInfo.GetStatus(status);
    //    //}

    //    //internal static SqlJobHelper.JobInfo.CurrentStatus ExecuteJob(string conString, string jobName)
    //    //{
    //    //    SQLDMO.Job job = GetJob(conString, jobName);
    //    //    if (job == null)
    //    //        throw new Exception("Job does not exist.");

    //    //    SQLDMO.SQLDMO_JOBEXECUTION_STATUS status = job.CurrentRunStatus;

    //    //    if (status == SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Idle)
    //    //        job.Start(null);

    //    //    return SqlJobHelper.JobInfo.GetStatus(status);
    //    //}

    //    //internal static SqlJobHelper.JobInfo.CurrentStatus GetJobStatus(string conString, string jobName)
    //    //{
    //    //    SQLDMO.Job job = GetJob(conString, jobName);
    //    //    if (job == null)
    //    //        throw new Exception("Job does not exist.");

    //    //    SQLDMO.SQLDMO_JOBEXECUTION_STATUS status = job.CurrentRunStatus;
    //    //    return SqlJobHelper.JobInfo.GetStatus(status);
    //    //}

    //    ///// <summary>
    //    ///// Remove Job Steps
    //    ///// </summary>
    //    ///// <param name="job"></param>
    //    //internal static void RemoveJobSteps(SQLDMO.Job job)
    //    //{
    //    //    foreach (SQLDMO.JobStep step in job.JobSteps)
    //    //    {
    //    //        step.Remove();
    //    //    }
    //    //}

    //    //internal static bool CreateJob(string conString, JobInfo job)
    //    //{

    //    //    SqlConnectionStringBuilder conInfo = new SqlConnectionStringBuilder(conString);
    //    //    SQLDMO.JobServer jobServer = GetJobServer(conString);

    //    //    if (jobServer == null)
    //    //        return false;

    //    //    SQLDMO.Job SQLJob = GetJob(conString, job.Name);

    //    //    try
    //    //    {
    //    //        if (SQLJob != null)
    //    //        {
    //    //            if (SQLJob.CurrentRunStatus == SQLDMO.SQLDMO_JOBEXECUTION_STATUS.SQLDMOJobExecution_Idle)
    //    //            {
    //    //                SQLJob.Remove();
    //    //            }
    //    //            else
    //    //            {
    //    //                jobServer = null;
    //    //                throw new Exception("Job is in use") ;
    //    //            }
    //    //        }
    //    //        SQLJob = new SQLDMO.Job();
    //    //        SQLJob.Name = job.Name;
    //    //        SQLJob.Description = job.Description;
    //    //        SQLJob.Category = "";//job.Category;
    //    //        jobServer.Jobs.Add(SQLJob);

    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        jobServer = null;
    //    //        throw new Exception(ex.ToString());
    //    //    }

    //    //    SQLDMO.JobStep aJobStep = null;

    //    //    foreach (StepInfo step in job.Steps)
    //    //    {
    //    //        aJobStep = new SQLDMO.JobStep();

    //    //        try
    //    //        {
    //    //            aJobStep.Name = step.Name;
    //    //            aJobStep.StepID = SQLJob.JobSteps.Count + 1;
    //    //            aJobStep.DatabaseName = step.DatabaseName.Length == 0 ? conInfo.InitialCatalog : step.DatabaseName;
    //    //            aJobStep.SubSystem = step.SubSystemType.ToString();
    //    //            aJobStep.Command = step.Command;

    //    //            aJobStep.OnSuccessAction
    //    //              = SQLDMO.SQLDMO_JOBSTEPACTION_TYPE.SQLDMOJobStepAction_QuitWithSuccess;
    //    //            aJobStep.OnFailAction
    //    //              = SQLDMO.SQLDMO_JOBSTEPACTION_TYPE.SQLDMOJobStepAction_QuitWithFailure;

    //    //            SQLJob.JobSteps.Add(aJobStep);
    //    //            SQLJob.ApplyToTargetServer(conInfo.DataSource);
    //    //            aJobStep.DoAlter();
    //    //            SQLJob.Refresh();
    //    //            aJobStep.Refresh();
    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            jobServer = null;
    //    //            throw new Exception(ex.ToString());
    //    //        }
    //    //    }
    //    //    jobServer = null;
    //    //    return true;
    //    //} //CreateJob

    //    #endregion Methods



    //} // SqlJobHelper
}
