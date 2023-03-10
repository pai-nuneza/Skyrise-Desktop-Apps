using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CommonPostingLibrary;
using Posting.DAL;


namespace Posting.BLogic
{
    public abstract class BaseBL
    {
        public BaseBL()
        {
        }

        #region public
        public abstract int Add(DataRow row);
        public abstract int Update(DataRow row);
        public abstract int Delete(string code, string userLogin);
        public abstract DataSet FetchAll();
        public abstract DataRow Fetch(string code);
        #endregion

        #region protected
        protected string GetScalarValue(string column, string table, string condition, DALHelper dal)
        {
            string retval = string.Empty;
            #region query
            string sqlquery =
                    @"SELECT " + column +
                    ((table != null && table.Trim() != "") ?
                        " FROM " + table : "") +
                    ((condition != null && condition.Trim() != "") ?
                        " WHERE " + condition : "");
            #endregion
            try
            {
                retval = dal.ExecuteScalar(sqlquery).ToString();
            }
            catch (Exception)
            {
                retval = "0";
            }

            return retval;
        }
        #endregion

        protected string GetValue(object objVal)
        {
            if (objVal != null)
                return objVal.ToString().Trim();
            else
                return String.Empty;
        }

        protected Int32 getIntValue(object obj)
        {
            Int32 retval = 0;
            try
            {
                retval = Convert.ToInt32(obj.ToString().Replace(",", "").Split('.')[0]);
            }
            catch (Exception)
            {
                retval = 0;
            }
            return retval;
        }

        protected decimal getDecimalValue(object objVal)
        {
            Decimal retval = Convert.ToDecimal(0.0);
            try
            {
                retval = Convert.ToDecimal(objVal);
            }
            catch (Exception)
            {
                retval = Convert.ToDecimal(0.0);
            }
            return retval;
        }
    }
}

