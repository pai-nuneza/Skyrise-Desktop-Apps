using System;
using System.Collections.Generic;
using System.Text;
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

        // added modified functions for isolation level capability
        public DataSet GetDisplayData(DALHelper dhHelper)
        {
            DataSet dsResult = new DataSet();
            dsResult = dhHelper.ExecuteDataSet(sqlSelectCommand);
            return dsResult;
        }

    }
}
