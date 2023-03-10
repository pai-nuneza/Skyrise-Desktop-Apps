using System;
using System.Data;
using System.Data.SqlClient;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class GenericLookupBL 
    {
        private string sqlSelectCommand;

        public GenericLookupBL()
        {
        }
        
        public GenericLookupBL(string sqlPassedSelectCommand )
        {
            this.sqlSelectCommand = sqlPassedSelectCommand;
        }

        public DataSet GetDisplayData()
        {
            DataSet dsResult = new DataSet();

            using (DALHelper dhHelper = new DALHelper())
            {
                dhHelper.OpenDB();
                dsResult = dhHelper.ExecuteDataSet(sqlSelectCommand);
                dhHelper.CloseDB();
            }

            return dsResult;
        }

        public DataSet GetDisplayData(string lookupCode)
        {
            DataSet dsResult = new DataSet();
            ParameterInfo[] paramCol = new ParameterInfo[1];
            paramCol[0] = new ParameterInfo("@lookupCode", "%" + lookupCode + "%", SqlDbType.NVarChar, 20);

            using (DALHelper dhHelper = new DALHelper())
            {
                dhHelper.OpenDB();
                dsResult = dhHelper.ExecuteDataSet(sqlSelectCommand, CommandType.Text, paramCol);
                dhHelper.CloseDB();
            }

            return dsResult;
        }

        // 03/17/2007
        // kris
        // added modified functions for isolation level capability
        public DataSet GetDisplayDataDH(DALHelper dhHelper)
        {
            DataSet dsResult = new DataSet();

            dsResult = dhHelper.ExecuteDataSet(sqlSelectCommand);

            return dsResult;
        }

        public DataSet GetDisplayDataDH(string lookupCode, DALHelper dhHelper)
        {
            DataSet dsResult = new DataSet();
            ParameterInfo[] paramCol = new ParameterInfo[1];
            paramCol[0] = new ParameterInfo("@lookupCode", "%" + lookupCode + "%", SqlDbType.NVarChar, 20);

            dsResult = dhHelper.ExecuteDataSet(sqlSelectCommand, CommandType.Text, paramCol);

            return dsResult;
        }
        // end of 03/17/2007 update

    }
}
