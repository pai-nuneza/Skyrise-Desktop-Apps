using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using CommonLibrary;

using Payroll.DAL;

namespace Payroll.BLogic
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

        //Added by Charlie
        //Used to get tables from DTR DB
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

        public static DataRow GenerateFromCentralDB(string tableName, string CentralProfile)
        {
            DataTable dt;

            using (DALHelper dbUtil = new DALHelper(CentralProfile, false))
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(tableName);

                dbUtil.CloseDB();
            }

            return dt.NewRow();
        }

    }
}
