using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CommonLibrary;


namespace Payroll.DAL
{
    public class DALHelper : BaseDAL, IDisposable
    {
        private const int BatchSize = 10;
        private SqlConnection dbConn;
        private SqlTransaction trans;
        private int timeOut;

        #region constructor

        public DALHelper()
        {
            this.dbConn = this.getConnection();
        }

        public DALHelper(bool flag)
        {
            if (flag)
                this.dbConn = this.getConnectionDTR();
            else
                this.dbConn = this.getConnection();
        }

        public DALHelper(string DB)
        {
            this.dbConn = this.getConnection();
        }

        public DALHelper(string datasource, string DBName)
        {
            this.dbConn = this.getLocalConnection(datasource, DBName);
        }

        public DALHelper(string ProfileName, bool CheckProfile)
        {
            if (CheckProfile)
                this.dbConn = this.getConnection();
            else
                this.dbConn = this.getConnectionSelectedDB(ProfileName);
        }

        public DALHelper(string datasource, string DBName, string User, string Password)
        {
            this.dbConn = this.getLocalConnection(datasource, DBName, User, Password);
        }

        #endregion//constructor

        #region methods

        #region public

        #region others

        public void OpenDB()
        {
            if (this.dbConn.State == ConnectionState.Closed)
            {
                try
                {
                    this.dbConn.Open();
                }
                catch (Exception e)
                {
                    throw new PayrollException(e);
                }
            }
        }

        public void CloseDB()
        {
            if (this.dbConn.State == ConnectionState.Open)
                this.dbConn.Close();
        }

        public void BeginTransaction()
        {
            if (this.trans == null)
                this.trans = this.dbConn.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.trans.Commit();
            this.trans = null;
        }

        public void RollBackTransaction()
        {
            this.trans.Rollback();
            this.trans = null;
        }

        // added for transaction level snapshot isolation
        public void BeginTransactionSnapshot()
        {
            //if (this.trans == null)
            //{
            //    this.trans = this.dbConn.BeginTransaction(IsolationLevel.Snapshot);

            //}
            BeginTransaction();
        }

        public void BeginTransactionSnapshot(string transactionName)
        {
            if (this.trans == null)
            {
                //this.trans = this.dbConn.BeginTransaction(IsolationLevel.Snapshot, transactionName);
                this.trans = this.dbConn.BeginTransaction(transactionName);
            }
        }

        public void CommitTransactionSnapshot()
        {
            //this.trans.Commit();
            CommitTransaction();
        }

        public void RollBackTransactionSnapshot()
        {
            RollBackTransaction();
        }

        public DataTable GetTableStructure(string tableName)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.FillSchema(dt, SchemaType.Source);

            return dt;
        }

        public DataTable GetTableStructure(string strqry, bool istablename)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = this.dbConn.CreateCommand();
            if (!istablename)
                cmd.CommandText = strqry;
            else
                cmd.CommandText = string.Format("SELECT * FROM {0}", strqry);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.FillSchema(dt, SchemaType.Source);

            return dt;
        }

        public void setTimeOut(int timeOut)
        {
            this.timeOut = timeOut;
        }
        #endregion//others

        #region batchUpdate

        public void BatchUpdate(DataTable dt, string tableName)
        {
            this.BatchUpdate(dt, tableName, true, BatchSize);
        }

        public void BatchUpdate(DataTable dt, string tableName, bool continueOnError)
        {
            this.BatchUpdate(dt, tableName, continueOnError, BatchSize);
        }

        public void BatchUpdate(DataTable dt, string tableName, bool continueOnError, int batchSize)
        {
            ///*SqlCommand cmd = this.dbConn.CreateCommand();
            //cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
            //cmd.CommandType = CommandType.Text;

            //if (this.trans != null)
            //    cmd.Transaction = this.trans;

            //dt.TableName = tableName;*/

            //SqlDataAdapter adapter = new SqlDataAdapter(string.Format("SELECT * FROM {0}", tableName), this.dbConn);
            ////adapter.SelectCommand = cmd;
            ////adapter.UpdateBatchSize = batchSize;
            ////adapter.ContinueUpdateOnError = continueOnError;

            //if (this.trans != null)
            //    adapter.SelectCommand.Transaction = this.trans;

            //SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
            ////cmdBuilder.SetAllValues = false;
            //cmdBuilder.RefreshSchema();

            //adapter.InsertCommand = cmdBuilder.GetInsertCommand();
            //adapter.UpdateCommand = cmdBuilder.GetUpdateCommand();
            //adapter.DeleteCommand = cmdBuilder.GetDeleteCommand();

            //DataSet ds = new DataSet();

            //ds.Tables.Add(dt.Copy());



            //int x = adapter.Update(ds);

            //x = x + 1;

            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = timeOut;
            cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
            cmd.CommandType = CommandType.Text;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            dt.TableName = tableName;

            SqlDataAdapter adapter = new SqlDataAdapter(string.Format("SELECT * FROM {0}", tableName), this.dbConn);
            adapter.SelectCommand = cmd;
            adapter.UpdateBatchSize = batchSize;
            adapter.ContinueUpdateOnError = continueOnError;

            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
            cmdBuilder.SetAllValues = false;

            adapter.Update(dt);
        }

        #endregion//batchUpdate

        #region executeNonQuery

        public int ExecuteNonQuery(string sqlStatement)
        {
            return this.ExecuteNonQuery(sqlStatement, CommandType.Text, null);
        }

        public int ExecuteNonQuery(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteNonQuery(sqlStatement, cmdType, null);
        }

        public int ExecuteNonQuery(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol)
        {
            int affectedRows = 0;
            bool isOutputParameter = false;

            try
            {
                SqlCommand cmd = this.dbConn.CreateCommand();
                cmd.CommandTimeout = timeOut;
                cmd.CommandText = sqlStatement;
                cmd.CommandType = cmdType;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                if (paramCol != null)
                {
                    foreach (ParameterInfo prmInfo in paramCol)
                    {
                        SqlParameter prm = new SqlParameter();

                        prm.ParameterName = prmInfo.Name;
                        prm.SqlDbType = prmInfo.DataType;
                        prm.Size = prmInfo.Size;
                        prm.Value = prmInfo.Value;
                        prm.Direction = prmInfo.Direction;

                        cmd.Parameters.Add(prm);
                    }
                }

                affectedRows = cmd.ExecuteNonQuery();

                for (int i = 0; i < cmd.Parameters.Count; i++)
                {
                    isOutputParameter = (((SqlParameter)cmd.Parameters[i]).Direction == ParameterDirection.Output) || (((SqlParameter)cmd.Parameters[i]).Direction == ParameterDirection.InputOutput) || (((SqlParameter)cmd.Parameters[i]).Direction == ParameterDirection.ReturnValue);

                    if (isOutputParameter)
                        paramCol[i].Value = ((SqlParameter)cmd.Parameters[i]).Value;
                }
            }
            catch (SqlException exSql)
            {
                #region Save to Log File
                string strError = "Error code: " + exSql.Number.ToString() + ". ";
                for (int i = 0; i < exSql.Errors.Count; i++)
                {
                    strError += "Index [" + i + "] " +
                                "Message: " + exSql.Errors[i].Message + " " +
                                "LineNumber: " + exSql.Errors[i].LineNumber + ". ";
                }
                CommonProcedures.logErrorToFile(strError);
                #endregion
                if (exSql.Number == 1205)
                {
                    strError = string.Format(@"Sorry, the transaction failed to proceed because it is locked by another user. Please try again in a few minutes. (Error Code: 1205)");
                }
                else if (exSql.Number == 3960)
                {
                    strError = string.Format(@"Sorry, the transaction failed to proceed because another user has updated the data you are accessing. Please try again. (Error Code: 3960)");
                }
                else if ((exSql.Number >= 3951
               && exSql.Number <= 3957)
                    || exSql.Number == 3961)
                {
                    strError = string.Format(@"For error codes 3951,3952,3953,3954,3955,3956,3957 and 3961:
Snapshot Isolation Problem. Please contact your administrator. (Error Code: 39%%)");
                }
                else if (exSql.Number == -2)
                {
                    strError = string.Format("Request Timeout. Waited {0} seconds. Please try again.", timeOut);
                }
                //Send to exception below
                throw new Exception(string.Format("{0}\n\nLog file is saved in C:\\MinervaLogFile directory.", strError), exSql);
            }
            catch (Exception ex)
            {
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new PayrollException(ex);
            }

            return affectedRows;
        }

        #endregion//executeNonQuery

        #region executeReader

        public SqlDataReader ExecuteReader(string sqlStatement)
        {
            return this.ExecuteReader(sqlStatement, CommandType.Text, null, CommandBehavior.Default);
        }

        public SqlDataReader ExecuteReader(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteReader(sqlStatement, cmdType, null, CommandBehavior.Default);
        }

        public SqlDataReader ExecuteReader(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol, CommandBehavior behavior)
        {
            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = timeOut;
            cmd.CommandText = sqlStatement;
            cmd.CommandType = cmdType;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            if (paramCol != null)
            {
                foreach (ParameterInfo prmInfo in paramCol)
                {
                    SqlParameter prm = new SqlParameter();

                    prm.ParameterName = prmInfo.Name;
                    prm.SqlDbType = prmInfo.DataType;
                    prm.Size = prmInfo.Size;
                    prm.Value = prmInfo.Value;
                    prm.Direction = prmInfo.Direction;

                    cmd.Parameters.Add(prm);
                }
            }

            return cmd.ExecuteReader(behavior);

        }

        #endregion//executeReader

        #region executeScalar

        public object ExecuteScalar(string sqlStatement)
        {
            return this.ExecuteScalar(sqlStatement, CommandType.Text, null);
        }

        public object ExecuteScalar(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteScalar(sqlStatement, cmdType, null);
        }

        public object ExecuteScalar(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol)
        {
            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = timeOut;
            cmd.CommandText = sqlStatement;
            cmd.CommandType = cmdType;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            if (paramCol != null)
            {
                foreach (ParameterInfo prmInfo in paramCol)
                {
                    SqlParameter prm = new SqlParameter();

                    prm.ParameterName = prmInfo.Name;
                    prm.SqlDbType = prmInfo.DataType;
                    prm.Size = prmInfo.Size;
                    prm.Value = prmInfo.Value;
                    prm.Direction = prmInfo.Direction;

                    cmd.Parameters.Add(prm);
                }
            }

            return cmd.ExecuteScalar();
        }

        #endregion//executeScalar

        #region executeDataSet

        public DataSet ExecuteDataSet(string sqlStatement)
        {
            return this.ExecuteDataSet(sqlStatement, CommandType.Text, null);
        }

        public DataSet ExecuteDataSet(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteDataSet(sqlStatement, cmdType, null);
        }

        public DataSet ExecuteDataSet(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol)
        {
            DataSet dataSet = new DataSet();

            try
            {
                SqlCommand cmd = this.dbConn.CreateCommand();
                cmd.CommandTimeout = timeOut;
                cmd.CommandText = sqlStatement;
                cmd.CommandType = cmdType;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                if (paramCol != null)
                {
                    foreach (ParameterInfo prmInfo in paramCol)
                    {
                        SqlParameter prm = new SqlParameter();

                        prm.ParameterName = prmInfo.Name;
                        prm.SqlDbType = prmInfo.DataType;
                        prm.Size = prmInfo.Size;
                        prm.Value = prmInfo.Value;
                        prm.Direction = prmInfo.Direction;

                        cmd.Parameters.Add(prm);
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(dataSet);

                //code below will trim all text fields 
                foreach (DataTable dt in dataSet.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (dt.Columns[i].DataType == Type.GetType("System.String", true))
                                row[i] = row[i].ToString().TrimEnd();
                        }
                    }
                }
            }
            catch (SqlException exSql)
            {
                #region Save to Log File
                string strError = "Error code: " + exSql.Number.ToString() + ". ";
                for (int i = 0; i < exSql.Errors.Count; i++)
                {
                    strError += "Index [" + i + "] " +
                                "Message: " + exSql.Errors[i].Message + " " +
                                "LineNumber: " + exSql.Errors[i].LineNumber + ". ";
                }
                CommonProcedures.logErrorToFile(strError);
                #endregion
                if (exSql.Number == 1205)
                {
                    strError = string.Format(@"Sorry, the transaction failed to proceed because it is locked by another user. Please try again in a few minutes. (Error Code: 1205)");
                }
                else if (exSql.Number == 3960)
                {
                    strError = string.Format(@"Sorry, the transaction failed to proceed because another user has updated the data you are accessing. Please try again. (Error Code: 3960)");
                }
                else if ((exSql.Number >= 3951
               && exSql.Number <= 3957)
                    || exSql.Number == 3961)
                {
                    strError = string.Format(@"For error codes 3951,3952,3953,3954,3955,3956,3957 and 3961:
                        Snapshot Isolation Problem. Please contact your administrator. (Error Code: 39%%)");
                }
                else if (exSql.Number == -2)
                {
                    strError = string.Format("Request Timeout. Waited {0} seconds. Please try again.", timeOut);
                }
                throw new Exception(string.Format("{0}\n\nLog file is saved in C:\\MinervaLogFile directory.", strError), exSql);
            }
            catch (Exception ex)
            {
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new PayrollException(ex);
            }

            return dataSet;
        }

        #endregion//executeDataSet

        #endregion//public

        #region private

        private SqlConnection getConnection()
        {
            string[] param = new string[4];
            param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            param[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["CentralDBName"].ToString());
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());

            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["PayrollConnectionString"].ConnectionString, param);

            return new SqlConnection(connectionString);
        }

        private SqlConnection getConnectionSelectedDB(string ProfileName)
        {
            string[] param = new string[4];
            param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            param[1] = ProfileName;
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());

            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["PayrollConnectionString"].ConnectionString, param);

            return new SqlConnection(connectionString);
        }

        public SqlConnection getConnectionDTR()
        {
            string[] param = new string[4];
            param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            param[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());

            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["dtrConnectionString"].ConnectionString, param);

            return new SqlConnection(connectionString);
        }

        private SqlConnection getLocalConnection(string datasource, string DBName)
        {
            string[] param = new string[2];
            param[0] = datasource;
            param[1] = DBName;
            string connectionString = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;
	                                User ID=sa;Password=systemadmin", param);

            return new SqlConnection(connectionString);
        }

        private SqlConnection getLocalConnection(string datasource, string DBName, string User, string Password)
        {
            string[] param = new string[4];
            param[0] = datasource;
            param[1] = DBName;
            param[2] = User;
            param[3] = Password;
            string connectionString = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;
	                                User ID={2};Password={3}", param);

            return new SqlConnection(connectionString);
        }

        #endregion//private

        #endregion//methods

        #region IDisposable

        public void Dispose()
        {
            if (this.dbConn != null && this.dbConn.State != ConnectionState.Closed)
                this.dbConn.Close();

            GC.SuppressFinalize(this);
        }

        #endregion//IDisposable

    }
}
