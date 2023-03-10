using System;
using System.Collections.Generic;
using System.Text;

using System.Data.OleDb;
using System.Data;

namespace CommonLibrary
{
    public class ExcelHelper
    {
        public DataSet GetExcelData(string excelFile)
        {
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;
            DataSet ds = new DataSet();
            try
            {
                String connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;", excelFile);
                objConn = new OleDbConnection(connString);
                objConn.Open();
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                if (i > 0)
                {
                    string sql = string.Format(" select * from [{0}]", excelSheets[0].ToString());
                    OleDbDataAdapter da = new OleDbDataAdapter(sql, objConn);
                    da.Fill(ds);
                }

                return ds;
            }
            catch (Exception ex)
            {
                return new DataSet();
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }
    }
}
