using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using CommonPostingLibrary;

namespace Posting.DAL
{
    public class BaseDAL
    {
        public BaseDAL()
        {
            if (ConfigurationManager.AppSettings["isExempted"] == "FALSE")
            {
                if (!CommonProcedures.isAllowTransaction())
                {
                    //   throw new InventoryException("Payroll is on going, you cannot transact.");
                }
            }
        }
    }
}

