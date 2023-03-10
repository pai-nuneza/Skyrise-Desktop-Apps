using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Posting.DAL;

namespace Posting.BLogic
{
    public class DbRecord
    {
        public static DataRow Generate(string tableName)
        {
            DataTable dt;

            using (DALHelper dbUtil = new DALHelper())
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(tableName);

                dbUtil.CloseDB();
            }

            return dt.NewRow();
        }

        public static DataTable GenerateTable(string tableName)
        {
            DataTable dt;

            using (DALHelper dbUtil = new DALHelper())
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(tableName);

                dbUtil.CloseDB();
            }

            return dt.Clone();
        }

        public static DataTable GenerateTable(string tableName, DALHelper dbUtil)
        {
            DataTable dt = dbUtil.GetTableStructure(tableName);
            return dt.Clone();
        }

        public static DataTable GenerateCustomTable(string strqry)
        {
            DataTable dt;

            using (DALHelper dbUtil = new DALHelper())
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(strqry, false);

                dbUtil.CloseDB();
            }

            return dt;
        }

        public static DataRow GenerateDTR(string tableName)
        {
            DataTable dt;

            using (DALHelper dbUtil = new DALHelper(true))
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(tableName);

                dbUtil.CloseDB();
            }

            return dt.NewRow();
        }
    }
}
