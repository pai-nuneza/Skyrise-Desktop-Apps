using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using System.Collections;

namespace Posting.DAL
{
    public class AccessDALHelper : IDisposable
    {
        private string ConnectionString = string.Empty;
        private OleDbConnection Connection;
        private OleDbTransaction Transaction;

        public AccessDALHelper()
        {
            string _Connection = ConfigurationManager.ConnectionStrings["AccessConnectionString"].ToString();
            string _DataSource = ConfigurationManager.AppSettings["AccessDataSource"].ToString();
            string _UserID = ConfigurationManager.AppSettings["AccessUserID"].ToString();
            string _Password = ConfigurationManager.AppSettings["AccessPassword"].ToString();

            this.ConnectionString = string.Format(_Connection, _DataSource, _UserID, _Password);
            Connection = new OleDbConnection(ConnectionString);
        }

        public void OpenDB()
        {
            if (Connection.State != ConnectionState.Open)
                this.Connection.Open();
        }

        public void CloseDB()
        {
            if (Connection.State != ConnectionState.Closed)
                this.Connection.Close();
        }

        public void BeginTransaction()
        {
            if (this.Transaction == null)
                this.Transaction = this.Connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (this.Transaction != null)
                this.Transaction.Commit();

            this.Transaction = null;
        }

        public void RollBackTrasaction()
        {
            if (this.Transaction != null)
                this.Transaction.Rollback();

            this.Transaction = null;
        }

        public int ExecuteNonQuery(string Query)
        {
            return ExecuteNonQuery(Query, null);
        }

        public int ExecuteNonQuery(string Query, AccessParamter[] AccessParamters)
        {
            if (Connection.State != ConnectionState.Open)
                this.Connection.Open();

            OleDbCommand Command = this.Connection.CreateCommand();
            Command.CommandTimeout = 0;
            Command.CommandType = CommandType.Text;
            Command.CommandText = Query;

            if (AccessParamters != null)
                foreach (AccessParamter AccessParamter in AccessParamters)
                    Command.Parameters.Add(AccessParamter.OLEParameter);

            if (this.Transaction != null)
                Command.Transaction = this.Transaction;

            return Command.ExecuteNonQuery();
        }

        public DataSet ExecuteDataSet(string Query)
        {
            return ExecuteDataSet(Query, null);
        }

        public DataSet ExecuteDataSet(string Query, AccessParamter[] AccessParamters)
        {
            DataSet Value = new DataSet();

            if (Connection.State != ConnectionState.Open)
                this.Connection.Open();


            OleDbCommand Command = this.Connection.CreateCommand();
            Command.CommandTimeout = 0;
            Command.CommandType = CommandType.Text;
            Command.CommandText = Query;

            if (AccessParamters != null)
                foreach (AccessParamter AccessParamter in AccessParamters)
                    Command.Parameters.Add(AccessParamter.OLEParameter);

            if (this.Transaction != null)
                Command.Transaction = this.Transaction;

            OleDbDataAdapter Adapter = new OleDbDataAdapter();
            Adapter.SelectCommand = Command;
            Adapter.Fill(Value);

            foreach (DataTable Table in Value.Tables)
                foreach (DataRow Row in Table.Rows)
                    foreach (DataColumn Column in Table.Columns)
                        if (Column.DataType == Type.GetType("System.String", true))
                            Row[Column] = Row[Column].ToString().TrimEnd();

            return Value;
        }

        public static string DataSourcePath()
        {
            return ConfigurationManager.AppSettings["AccessDataSource"].ToString();
        }

        public void Dispose()
        {
            this.CloseDB();
            GC.SuppressFinalize(this);
        }
    }

    // public class AccessParameterCollection : InternalDataCollectionBase
    // {
    //     ArrayList _ParameterList = new ArrayList();

    //     protected virtual ArrayList List { get { return _ParameterList; } }

    //     public void Add(AccessParamter AccessParamter)
    //     {
    //         _ParameterList.Add(AccessParamter);
    //     }

    //     public void Add(string ParamterName, object ParamterValue)
    //     {
    //         Add(new AccessParamter(ParamterName, ParamterValue));
    //     }

    //     public AccessParamter this[int Index]
    //     {
    //         get
    //         {
    //             if (_ParameterList.Count > Index)
    //                 return (AccessParamter)_ParameterList[Index];
    //             else
    //                 throw new Exception("AccessParameterCollections does not contain AccessParameter indexed '" + Index.ToString() + "'");
    //         }
    //     }

    //     public AccessParamter this[string Name]
    //     {
    //         get
    //         {
    //             foreach (AccessParamter Parameter in _ParameterList)
    //                 if (Parameter.Name == Name)
    //                     return Parameter;

    //             throw new Exception("AccessParameterCollections does not contain AccessParameter named '" + Name + "'");
    //         }
    //     }

    //     public AccessParamter[] AccessParamters
    //     {
    //         get
    //         {
    //             AccessParamter[] Parameters = new AccessParamter[_ParameterList.Count];

    //             for (int i = 0; i < _ParameterList.Count; i++)
    //             {
    //                 Parameters[i] = (AccessParamter)_ParameterList[i];
    //             }

    //             return Parameters;
    //         }
    //     }
    //}

    public class AccessParamter
    {
        public string Name;
        public object Value;

        public AccessParamter(string ParamterName, object ParamterValue)
        {
            Name = ParamterName;
            Value = ParamterValue;
        }

        public OleDbParameter OLEParameter
        {
            get
            {
                OleDbParameter _OLEParameter = new OleDbParameter(Name, Value);
                return _OLEParameter;
            }
        }
    }
}
