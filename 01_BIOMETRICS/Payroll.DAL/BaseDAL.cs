using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using CommonLibrary;

namespace Payroll.DAL
{
    public class BaseDAL
    {
        public BaseDAL()
        {
            //if (ConfigurationManager.AppSettings["isExempted"] == "FALSE")
            //   if (!CommonProcedures.isAllowTransaction())
            //       throw new PayrollException("Payroll is on going, you cannot transact.");
        }        
    }
}
