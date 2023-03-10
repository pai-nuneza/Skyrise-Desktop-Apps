using System;
using System.Collections.Generic;
using System.Text;

using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace CommonLibrary
{
    public class ExcelToDataset
    {
        string ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;""";
        string ConnectionString2007 = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;HDR=YES;IMEX=1;"""; // Clark/MovementReason 08/04/2010
        string EXCELFILENAME = string.Empty;
        String[] tableNames;
        
        OleDbConnection conn = null;
        OleDbCommand comm = null;
        OleDbDataAdapter da = null;

        DataTable tables = null;

        public ExcelToDataset()
        {
        }

        /// <summary>
        /// Load ang Excel File. If ma load na pwede nka mka kuha sa 
        /// worksheetnames thru sa method nga ExcelWorksheetNames.
        /// </summary>
        /// <param name="excelFilename">Filename sa excel.</param>
        public void LoadExcel(string excelFilename)
        {
            this.EXCELFILENAME = excelFilename;
            ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;""";
            ConnectionString = string.Format(ConnectionString, EXCELFILENAME);

            try
            {
                // Clark/MovementReason 08/04/2010 START
                try
                {
                    conn = new OleDbConnection();
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                }
                catch
                {
                    // Connection to Excel 8.0 failed so close it.
                    if (conn != null)
                        conn.Close();

                    conn.Dispose();
                    conn = null;

                    // Then attempt to use Excel 12.0 (aka Office 2007)
                    string connStr2007 = string.Format(ConnectionString2007, EXCELFILENAME);
                    conn = new OleDbConnection();
                    conn.ConnectionString = connStr2007;
                    conn.Open();
                }
                // Clark/MovementReason 08/04/2010 END

                //retrieve worksheets
                tables = conn.GetSchema("Tables", null);

                //retrieve worksheet names
                tableNames = new String[tables.Rows.Count];
                int i = 0;

                foreach (DataRow row in tables.Rows)
                {
                    tableNames[i] = row["TABLE_NAME"].ToString();
                    tableNames[i] = tableNames[i].TrimStart(new char[] { '\'' }).TrimEnd(new char[] {'\''});
                    i++;
                }
               
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex);
            }
            finally
            {
                if(conn!=null)
                    conn.Close();

                conn.Dispose();
                conn = null;
            }
        }

        /// <summary>
        /// Worksheet names. Ma gamit rani if ga load ka ug excel file.
        /// </summary>
        /// <returns></returns>
        public String[] ExcelWorksheetNames()
        {
            return this.tableNames;
        }

        /// <summary>
        /// Retrieve data from a worksheet with a specific range. 
        /// E.g. A1:E4
        /// </summary>
        /// <param name="worksheetname">Worksheet Name.</param>
        /// <param name="startCell">Starting Cell.</param>
        /// <param name="lastCell">Last Cell.</param>
        /// <returns></returns>
        public DataSet GetDataSet(string worksheetname, string startCell, string lastCell)
        {
            string col = GetFirstColumn(worksheetname);
            lastCell = lastCell + getLastRow(worksheetname);
            string query = string.Format("select * from [{0}{1}:{2}] where {3} is not null", worksheetname, startCell, lastCell,col);
            return ConvertToDataSet(query);   
        }

        private string getLastRow(string worksheetname)
        {
            DataSet ds = new DataSet();
            string lastRow = string.Empty;
            string firstCol = GetFirstColumn(worksheetname);
            string query = string.Format("select count({0}) as [totalrows] from [{1}] where {0} is not null", firstCol, worksheetname);
            ds = ConvertToDataSet(query);
            lastRow = ds.Tables[0].Rows[0]["totalrows"].ToString().Trim();
            return lastRow;
        }

        /// <summary>
        /// Retrieve data from excel. Bisag unsa kuhaun niya sa excel.
        /// </summary>
        /// <param name="worksheetname">Worksheet name</param>
        /// <returns></returns>
        public DataSet GetDataSet(string worksheetname)
        {
            string col = GetFirstColumn(worksheetname);
            string query = string.Format("select * from [{0}]", worksheetname);// where {1} is not null", worksheetname,col);
            return ConvertToDataSet(query);    
           
        }

        /// <summary>
        /// Retrieve data sa excel, then e specify ang starting cell. E.g. A3.
        /// </summary>
        /// <param name="worksheetname">Worksheet Name.</param>
        /// <param name="startCell">Starting Cell.</param>
        /// <returns></returns>
        public DataSet GetDataSet(string worksheetname, string startCell)
        {
            string endCell = "GG50000"; //constant kay wla ko kabalo unsaun pag kuha sa last column
            string col = GetFirstColumn(worksheetname);
            string query = string.Format("select * from [{0}{1}:{2}] where {3} is not null", worksheetname, startCell, endCell,col);
            return ConvertToDataSet(query);
        }

        private string GetFirstColumn(string worksheetname)
        {
            string columnName = string.Empty;
            ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;""";
            ConnectionString = string.Format(ConnectionString, EXCELFILENAME);
            DataSet dsColumns = new DataSet();
            try
            {
                // Clark/MovementReason 08/04/2010 START
                try
                {
                    conn = new OleDbConnection();
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                }
                catch
                {
                    // Connection to Excel 8.0 failed so close it.
                    if (conn != null)
                        conn.Close();

                    conn.Dispose();
                    conn = null;

                    // Then attempt to use Excel 12.0 (aka Office 2007)
                    string connStr2007 = string.Format(ConnectionString2007, EXCELFILENAME);
                    conn = new OleDbConnection();
                    conn.ConnectionString = connStr2007;
                    conn.Open();
                }
                // Clark/MovementReason 08/04/2010 END

                comm = new OleDbCommand();
                comm.Connection = conn;

                //retrieve data from worksheets
                comm.CommandText = string.Format(@"select * from [{0}]",worksheetname);

                da = new OleDbDataAdapter(comm);

                da.Fill(dsColumns);

                columnName = dsColumns.Tables[0].Columns[0].ColumnName;

            }
            catch (Exception ex)
            {
                throw new PayrollException(ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();

                conn.Dispose();
                conn = null;
            }
            return columnName;
        }

        private DataSet ConvertToDataSet(string query)
        {
            DataSet tempData = new DataSet();
            ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;""";
            ConnectionString = string.Format(ConnectionString, EXCELFILENAME);
            try
            {
                // Clark/MovementReason 08/04/2010 START
                try
                {
                    conn = new OleDbConnection();
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                }
                catch
                {
                    // Connection to Excel 8.0 failed so close it.
                    if (conn != null)
                        conn.Close();

                    conn.Dispose();
                    conn = null;

                    // Then attempt to use Excel 12.0 (aka Office 2007)
                    string connStr2007 = string.Format(ConnectionString2007, EXCELFILENAME);
                    conn = new OleDbConnection();
                    conn.ConnectionString = connStr2007;
                    conn.Open();
                }
                // Clark/MovementReason 08/04/2010 END


                comm = new OleDbCommand();
                comm.Connection = conn;
                
                //retrieve data from worksheets
                comm.CommandText = query;

                da = new OleDbDataAdapter(comm);

                da.Fill(tempData);
            }
            catch (Exception ex)
            {
                throw new PayrollException("Unable to read data.");
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                if (da != null)
                    da.Dispose();
                if (comm != null)
                    comm.Dispose();

                conn.Dispose();
                conn = null;
                da = null;
                comm = null;
            }
            return tempData;
        }
    }
}
