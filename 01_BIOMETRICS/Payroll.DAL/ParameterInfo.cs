using System;
using System.Data;
using System.Data.SqlClient;  

namespace Payroll.DAL
{
    public class ParameterInfo
    {
        public string Name;
        public object Value;
        public SqlDbType DataType;
        public ParameterDirection Direction;
        public int Size;

        #region constructor

        public ParameterInfo(string paramName, object paramValue)
            : this(paramName, paramValue, SqlDbType.NVarChar, ParameterDirection.Input, 100)
        {
        }

        public ParameterInfo(string paramName, object paramValue, SqlDbType paramType)
            : this(paramName, paramValue, paramType, ParameterDirection.Input, 50)
        {
        }

        public ParameterInfo(string paramName, object paramValue, SqlDbType paramType, int paramSize)
            : this(paramName, paramValue, paramType, ParameterDirection.Input, paramSize)
        {
        }

        public ParameterInfo(string paramName, object paramValue, SqlDbType paramType, ParameterDirection paramDirection)
            : this(paramName, paramValue, paramType, paramDirection, 0)
        {
        }

        public ParameterInfo(string paramName, object paramValue, SqlDbType paramType, ParameterDirection paramDirection, int paramSize)
        {
            Name = paramName;
            Value = paramValue;
            DataType = paramType;
            Direction = paramDirection;
            Size = paramSize;
        }

        #endregion//constructor
    }
}
