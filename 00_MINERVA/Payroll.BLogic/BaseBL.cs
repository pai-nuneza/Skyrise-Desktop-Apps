using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;


namespace Payroll.BLogic
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
        
        public static DataSet searchTable(string selectCommand)
        {
            DataSet ds = new DataSet();
            string sqlSelectCommand = selectCommand;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlSelectCommand);
                dal.CloseDB();
            }
            return ds;
        }
        
        public static DataSet searchTable(string selectCommand, string status)
        {
            DataSet ds = new DataSet();
            string sqlSelectCommand = selectCommand;
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@status", status, SqlDbType.VarChar);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlSelectCommand, CommandType.Text, param);
                ds = dal.ExecuteDataSet(selectCommand, CommandType.Text, param);
                
                dal.CloseDB();
            }
            return ds;
        }
        //end
        #endregion//public

        #region protected
        protected DateTime GetServerDate()
        {
            string sqlQuery = @"Select Convert(char(10),Getdate(),101) AS ServerDate";

            DateTime date;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                date = Convert.ToDateTime(dal.ExecuteScalar(sqlQuery, CommandType.Text));
                dal.CloseDB();
            }

            return date;
        }
        
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
                retval = Convert.ToInt32(obj.ToString().Replace(",","").Split('.')[0]);
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

        public static DataRow getCompanyLogo()
        {
            DataSet ds = new DataSet();

            string query = @"Select Mcm_CompanyLogo From M_Company";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
                dal.CloseDB();
            }
            return ds.Tables[0].Rows[0];

        }

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
            //DALHelper tmpdal = null;
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

    }
}
