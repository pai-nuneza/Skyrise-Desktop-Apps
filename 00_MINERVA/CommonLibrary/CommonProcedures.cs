using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using Excel;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Payroll.DAL;

namespace CommonLibrary
{
    public class CommonProcedures
    {
        private static readonly string CompanyName;

        private static GenericPromptBL promptBL = new GenericPromptBL();

        static CommonProcedures()
        {
            //CompanyName = LoginInfo.getUser().CompanyName;
        }

        public static void IsCharacter(KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !(e.KeyChar == '\b') && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        public static void IsNumeric(KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !(e.KeyChar == '\b') && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        public static void IsCorrectAddressFormat(KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && !(e.KeyChar == '\b') && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        public static string ConvertToTime(System.Windows.Forms.TextBox txtBoxName)
        {
            if (txtBoxName.Text.Contains(":"))
            {
                return txtBoxName.Text;
            }
            else
            {
                if (Convert.ToInt32(txtBoxName.Text) <= 2359 && Convert.ToInt32(txtBoxName.Text) >= 0 && txtBoxName.Text.Length >= 3)
                {
                    if (txtBoxName.Text.Length == 4)
                        txtBoxName.Text = txtBoxName.Text.Insert(2, ":");
                    else
                        txtBoxName.Text = "0" + txtBoxName.Text.Insert(1, ":");

                }
                else
                {
                    txtBoxName.Text = string.Empty;
                    txtBoxName.Focus();
                }
            }

            return txtBoxName.Text;
        }

        public static bool GetNumericWithoutDecimal(Control txtBox, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == '\b' || (e.KeyChar >= '0' && e.KeyChar <= '9')))
                return true;
            else
                return false;
        }

        public static bool GetNumericWithDecimal(Control txtBox, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString().Equals("."))
            {
                if (txtBox.Text.Contains("."))
                    return true;
                else
                    return false;
            }
            else if (!(e.KeyChar == '.' || e.KeyChar == '\b' || (e.KeyChar >= '0' && e.KeyChar <= '9')))
                return true;
            else
                return false;
        }

        public static string getQuery(
            string select,
            string from,
            string where,
            string otherCondition,
            string orderby
            )
        {
            return getQuery(
                        select,
                        from,
                        where,
                        otherCondition,
                        "",
                        "",
                        orderby);
        }

        public static string getQuery(
            string select,
            string from,
            string where,
            string otherCondition,
            string groupby,
            string having,
            string orderby
            )
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(select);
            sbQuery.Append("\n");
            sbQuery.Append(from);
            sbQuery.Append("\n");
            sbQuery.Append(where);
            sbQuery.Append("\n");
            sbQuery.Append(otherCondition);
            sbQuery.Append("\n");
            sbQuery.Append(groupby);
            sbQuery.Append("\n");
            sbQuery.Append(having);
            sbQuery.Append("\n");
            sbQuery.Append(orderby);
            sbQuery.Append("\n");
            return sbQuery.ToString();
        }

        public static bool createExcelFile(System.Data.DataTable dt, string filename)
        {
            bool retval = false;

            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;

            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int row = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    row++;
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                    }
                }
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retval;
        }

        public static bool createExcelFile(System.Data.DataTable dt, System.Data.DataTable dt2, string filename)
        {
            bool retval = false;

            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Worksheet objSheet2 = null;

            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);
                objSheet2 = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[2]).Select(Missing.Value);

                int row = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    row++;
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                    }
                }
                row = 0;
                foreach (DataRow dr in dt2.Rows)
                {
                    row++;
                    for (int col = 0; col < dt2.Columns.Count; col++)
                    {
                        objSheet2.Cells[row + 1, col + 1] = dr[col].ToString();
                    }
                }
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retval;
        }

        public static bool createExcelFile(DataSet ds, string filename)
        {
            bool retval = false;

            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;

            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int row = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    row++;
                    for (int col = 0; col < ds.Tables[0].Columns.Count; col++)
                    {
                        objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                    }
                }
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retval;
        }

        public static bool createExcelFile(DataView dView, string filename, int columnCounter, string[] columnNames)
        {
            bool retval = true;
            System.Data.DataTable dt = dView.ToTable();
            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;

            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int row = 0;
                for (int col = 0; col < columnCounter; col++)
                {
                    objSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                }

                row = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    row++;
                    for (int col = 0; col < columnCounter; col++)
                    {
                        objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                    }
                }
                objSheet.Columns.AutoFit();
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                retval = false;
                throw new PayrollException(e);
            }

            return retval;
        }

        public static void createExcelFile(string[] CompanyProfile, string ReportTitle, string SubreportTitle,
            DataView dView, string filename, int columnCounter, string[] columnNames)
        {
            System.Data.DataTable dt = dView.ToTable();
            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Range objRange;
            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int startRow = 0;

                objRange = objSheet.get_Range("A1", "A8");
                objRange.Font.Bold = true;

                for (int i = 1; i <= 8; i++)
                {
                    objRange = objSheet.get_Range("A" + i, "M" + i);
                    objRange.Merge(true);
                }

                for (int cp = 0; cp < CompanyProfile.Length; cp++)
                {
                    objSheet.Cells[startRow + 1, 1] = CompanyProfile[cp].ToString();
                    startRow++;
                }

                objSheet.Cells[startRow + 2, 1] = ReportTitle;
                startRow += 2;

                objSheet.Cells[startRow + 2, 1] = SubreportTitle;
                startRow += 3;

                int row = startRow;
                for (int col = 0; col < columnCounter; col++)
                {
                    objSheet.Cells[row + 1, col + 1] = columnNames[col].ToString();
                }

                row = startRow;

                foreach (DataRow dr in dt.Rows)
                {
                    row++;
                    for (int col = 0; col < columnCounter; col++)
                    {
                        objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                    }
                }
                objSheet.Columns.AutoFit();
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            finally
            {
                objApp = null;
                objBooks = null;
                objBook = null;
                objSheet = null;
                objRange = null;
            }
        }

        public static void createExcelFile(DataSet dsCompanyProfile, string ReportTitle, DataSet dsPayPeriod, 
            DataSet Records, DataSet dsSig, System.Drawing.Font rFont) ///00016
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "xlsx";
            sf.Filter = "Excel File (.xlsx)|*.xlsx|Excel File (.xls)|*.xls";
            sf.ShowDialog();

            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Range objRange;
            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int startRow = 0;

                objRange = objSheet.get_Range("A1", "A8");
                objRange.Font.Bold = true;

                //for (int i = 1; i <= 8; i++)
                //{
                //    objRange = objSheet.get_Range("A" + i, "M" + i);
                //    objRange.Merge(true);
                //}

                if (dsCompanyProfile != null && dsCompanyProfile.Tables.Count > 0 && dsCompanyProfile.Tables[0].Rows.Count > 0)
                {
                    objSheet.Cells[1, 2] = dsCompanyProfile.Tables[0].Rows[0]["Mcm_CompanyName"].ToString();
                    startRow++;

                    objSheet.Cells[2, 2] = dsCompanyProfile.Tables[0].Rows[0]["Address"].ToString();
                    startRow++;

                    objSheet.Cells[3, 2] = dsCompanyProfile.Tables[0].Rows[0]["Contacts"].ToString();
                    startRow++;

                    if (dsCompanyProfile.Tables[0].Rows[0]["Mcm_CompanyLogo"] != null 
                        && dsCompanyProfile.Tables[0].Rows[0]["Mcm_CompanyLogo"].ToString() != "")
                    {
                        MemoryStream ms = new MemoryStream((byte[])dsCompanyProfile.Tables[0].Rows[0]["Mcm_CompanyLogo"]);
                        Image img = Image.FromStream(ms);
                        objRange = objSheet.get_Range("A2", "A2");
                        objRange.Select();
                        Clipboard.SetDataObject(img);
                        objSheet.Paste(objRange, Missing.Value);
                        Clipboard.Clear();
                    }

                }

                objSheet.Cells[5, 2] = ReportTitle;
                startRow += 2;

                if (dsPayPeriod != null && dsPayPeriod.Tables[0].Rows.Count > 0)
                {
                    objSheet.Cells[6, 2] = string.Format("PAYROLL PERIOD: {0}", dsPayPeriod.Tables[0].Rows[0]["Payperiod"].ToString());
                    objSheet.Cells[7, 2] = string.Format("PAYROLL CUT-OFF: {0} - {1}"
                        , dsPayPeriod.Tables[0].Rows[0]["Start Cycle"].ToString()
                        , dsPayPeriod.Tables[0].Rows[0]["End Cycle"].ToString());
                    startRow += 3;
                }

                //mao na ni
                int row = startRow;
                for (int tableCount = 0; tableCount < Records.Tables.Count; tableCount++)
                {
                    row = startRow;

                    int columnCounter = Records.Tables[tableCount].Columns.Count;

                    //for (int col = 0; col < columnCounter; col++)
                    //{
                    //    objSheet.Cells[row + 1, col + 1] = ColumnHeader.Tables[tableCount].Rows[0][col].ToString();
                    //}
                    foreach (DataColumn dtColumn in Records.Tables[tableCount].Columns)
                    {
                        for (int col = 0; col < columnCounter; col++)
                        {
                            objSheet.Cells[row + 1, col + 1] = dtColumn.ColumnName.ToString();
                        }
                    }

                    row = startRow;

                    foreach (DataRow dr in Records.Tables[tableCount].Rows)
                    {
                        row++;
                        startRow++;
                        for (int col = 0; col < columnCounter; col++)
                        {
                            objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                        }
                    }

                    startRow = startRow + 2;
                    //objRange = objSheet.get_Range(startRow, columnCounter);
                    //objRange.Font.Name = rFont;
                }
                //hangtud diri


                objSheet.Columns.AutoFit();
                objBook.Close(true, sf.FileName, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            finally
            {
                objApp = null;
                objBooks = null;
                objBook = null;
                objSheet = null;
                objRange = null;
            }
        }

        public static void createExcelFile(string[] CompanyProfile, string ReportTitle, string SubreportTitle,
            DataSet Records, string filename, DataSet ColumnHeader)
        {
            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Range objRange;
            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int startRow = 0;

                objRange = objSheet.get_Range("A1", "A8");
                objRange.Font.Bold = true;

                for (int i = 1; i <= 8; i++)
                {
                    objRange = objSheet.get_Range("A" + i, "M" + i);
                    objRange.Merge(true);
                }

                for (int cp = 0; cp < CompanyProfile.Length; cp++)
                {
                    objSheet.Cells[startRow + 1, 1] = CompanyProfile[cp].ToString();
                    startRow++;
                }

                objSheet.Cells[startRow + 2, 1] = ReportTitle;
                startRow += 2;

                objSheet.Cells[startRow + 2, 1] = SubreportTitle;
                startRow += 3;

                ////mao na ni
                int row = startRow;
                for (int tableCount = 0; tableCount < ColumnHeader.Tables.Count; tableCount++)
                {
                    row = startRow;

                    int columnCounter = ColumnHeader.Tables[tableCount].Columns.Count;

                    for (int col = 0; col < columnCounter; col++)
                    {
                        objSheet.Cells[row + 1, col + 1] = ColumnHeader.Tables[tableCount].Rows[0][col].ToString();
                    }

                    row = startRow;

                    foreach (DataRow dr in Records.Tables[tableCount].Rows)
                    {
                        row++;
                        startRow++;
                        for (int col = 0; col < columnCounter; col++)
                        {
                            objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                        }
                    }

                    startRow = startRow + 2;
                }
                ////hangtud diri

                objSheet.Columns.AutoFit();
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            finally
            {
                objApp = null;
                objBooks = null;
                objBook = null;
                objSheet = null;
                objRange = null;
            }
        }

        public static void createExcelFileForAlphalist71(string[] CompanyProfile, string ReportTitle, string SubreportTitle,
            DataView dView, string filename, int columnCounter, string[] columnNames)
        {
            System.Data.DataTable dt = dView.ToTable();
            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Range objRange;
            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int startRow = 0;

                objRange = objSheet.get_Range("A1", "A8");
                objRange.Font.Bold = true;

                for (int i = 1; i <= 7; i++)
                {
                    objRange = objSheet.get_Range("A" + i, "M" + i);
                    objRange.Merge(true);
                }

                for (int cp = 0; cp < CompanyProfile.Length; cp++)
                {
                    objSheet.Cells[startRow + 1, 1] = CompanyProfile[cp].ToString();
                    startRow++;
                }

                objSheet.Cells[startRow + 2, 1] = ReportTitle;
                startRow += 2;

                //objSheet.Cells[startRow + 2, 1] = SubreportTitle;
                //startRow += 3;

                int start = startRow;

                #region [generate fixed header]
                #region [merge up to down]
                objRange = objSheet.get_Range("A" + startRow, "A" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("B" + startRow, "B" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("C" + (startRow + 1), "C" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("D" + (startRow + 1), "D" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("E" + (startRow + 1), "E" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("H" + (startRow + 1), "H" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("T" + startRow, "T" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("U" + startRow, "U" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("V" + startRow, "V" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("W" + startRow, "W" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("X" + (startRow + 1), "X" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("Y" + (startRow + 1), "Y" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("Z" + startRow, "Z" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("AA" + startRow, "AA" + (startRow + 2));
                objRange.Merge(false);
                #endregion

                #region [merge left to right]
                objRange = objSheet.get_Range("C" + startRow, "E" + startRow);
                objRange.Merge(true);

                objRange = objSheet.get_Range("H" + startRow, "Q" + startRow);
                objRange.Merge(true);

                objRange = objSheet.get_Range("I" + (startRow + 1), "M" + (startRow + 1));
                objRange.Merge(true);

                objRange = objSheet.get_Range("N" + (startRow + 1), "Q" + (startRow + 1));
                objRange.Merge(true);

                objRange = objSheet.get_Range("X" + startRow, "Y" + startRow);
                objRange.Merge(true);
                #endregion

                #region [merge both]
                objRange = objSheet.get_Range("R" + (startRow), "S" + (startRow + 1));
                objRange.Merge(true);
                objRange.Merge(false);
                #region [merge both]
                objRange = objSheet.get_Range("F" + (startRow), "G" + (startRow + 2));
                objRange.Merge(true);
                objRange.Merge(false);
                #endregion
                #endregion

                #region [naming of generated fixed header]
                startRow = startRow - 1;
                objSheet.Cells[startRow + 1, 1] = "SEQ NO.";
                objSheet.Cells[startRow + 1, 2] = "TIN";
                objSheet.Cells[startRow + 1, 3] = "NAME OF EMPLOYEES";
                objSheet.Cells[startRow + 2, 3] = "Last Name";
                objSheet.Cells[startRow + 2, 4] = "First Name";
                objSheet.Cells[startRow + 2, 5] = "Middle Name";
                objSheet.Cells[startRow + 1, 6] = "INCLUSIVE DATES";
                objSheet.Cells[startRow + 1, 8] = "(4) GROSS COMPENSATION INCOME";
                objSheet.Cells[startRow + 2, 8] = "Gross Compensation Income";
                objSheet.Cells[startRow + 2, 9] = "NON-TAXABLE";
                objSheet.Cells[startRow + 3, 9] = "13th Month Pay & Other Benefits";
                objSheet.Cells[startRow + 3, 10] = "De Minimis Benefits";
                objSheet.Cells[startRow + 3, 11] = "SSS, GSIS, PHIC, & Pag-ibig Contibutions, and Union Dues";
                objSheet.Cells[startRow + 3, 12] = "Salaries & Other Forms of Compensation";
                objSheet.Cells[startRow + 3, 13] = "Total Non-Taxable/Exempt Compensation Income";
                objSheet.Cells[startRow + 2, 14] = "TAXABLE";
                objSheet.Cells[startRow + 3, 14] = "Basic Salary";
                objSheet.Cells[startRow + 3, 15] = "13 Month Pay & Other Benefits";
                objSheet.Cells[startRow + 3, 16] = "Salaries & Other Forms of Compensation";
                objSheet.Cells[startRow + 3, 17] = "Total Taxable Compensation Income";
                objSheet.Cells[startRow + 1, 18] = "EXEMPTION";
                objSheet.Cells[startRow + 3, 18] = "Code";
                objSheet.Cells[startRow + 3, 19] = "Amount";
                objSheet.Cells[startRow + 1, 20] = "Premium Paid on Health and/or Hospital Insurance";
                objSheet.Cells[startRow + 1, 21] = "Net Taxable Compensation Income";
                objSheet.Cells[startRow + 1, 22] = "TAX - DUE   (JAN. - DEC.) ";
                objSheet.Cells[startRow + 1, 23] = "TAX - WITHHELD  (JAN. - NOV.) ";
                objSheet.Cells[startRow + 1, 24] = "YEAR-END ADJUSTMENT (10a) or (10b)";
                objSheet.Cells[startRow + 2, 24] = "AMOUNT WITHHELD AND PAID FOR IN DECEMBER";
                objSheet.Cells[startRow + 2, 25] = "OVER WITHHELD TAX REFUNDED TO EMPLOYEE";
                objSheet.Cells[startRow + 1, 26] = "AMOUNT OF TAX WITHHELD AS ADJUSTED (to be reflected in BIR Form No. 2316)";
                objSheet.Cells[startRow + 1, 27] = "Substitute Filing? Yes/No";
                #endregion
                #endregion

                startRow += 3;

                int row = startRow;
                for (int col = 0; col < columnCounter; col++)
                {
                    objSheet.Cells[row + 1, col + 1] = "'" + columnNames[col].ToString();
                }

                row = startRow;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        row++;
                        for (int col = 0; col < columnCounter; col++)
                        {
                            objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                        }
                    }
                    row += 2;
                }
                else
                {
                    row += 3;
                }

                objRange = objSheet.get_Range("A" + row, "G" + row);
                objRange.Merge(true);
                objSheet.Cells[row, 1] = "TOTAL";

                objRange = objSheet.get_Range("H" + row, "H" + row); objRange.Formula = "=SUM(H12:H" + (row - 1);
                objRange = objSheet.get_Range("I" + row, "I" + row); objRange.Formula = "=SUM(I12:I" + (row - 1);
                objRange = objSheet.get_Range("J" + row, "J" + row); objRange.Formula = "=SUM(J12:J" + (row - 1);
                objRange = objSheet.get_Range("K" + row, "K" + row); objRange.Formula = "=SUM(K12:K" + (row - 1);
                objRange = objSheet.get_Range("L" + row, "L" + row); objRange.Formula = "=SUM(L12:L" + (row - 1);
                objRange = objSheet.get_Range("M" + row, "M" + row); objRange.Formula = "=SUM(M12:M" + (row - 1);
                objRange = objSheet.get_Range("N" + row, "N" + row); objRange.Formula = "=SUM(N12:N" + (row - 1);
                objRange = objSheet.get_Range("O" + row, "O" + row); objRange.Formula = "=SUM(O12:O" + (row - 1);
                objRange = objSheet.get_Range("P" + row, "P" + row); objRange.Formula = "=SUM(P12:P" + (row - 1);
                objRange = objSheet.get_Range("Q" + row, "Q" + row); objRange.Formula = "=SUM(Q12:Q" + (row - 1);
                objRange = objSheet.get_Range("R" + row, "R" + row); objRange.Formula = "=SUM(R12:R" + (row - 1);
                objRange = objSheet.get_Range("S" + row, "S" + row); objRange.Formula = "=SUM(S12:S" + (row - 1);
                objRange = objSheet.get_Range("T" + row, "T" + row); objRange.Formula = "=SUM(T12:T" + (row - 1);
                objRange = objSheet.get_Range("U" + row, "U" + row); objRange.Formula = "=SUM(U12:U" + (row - 1);
                objRange = objSheet.get_Range("V" + row, "V" + row); objRange.Formula = "=SUM(V12:V" + (row - 1);
                objRange = objSheet.get_Range("W" + row, "W" + row); objRange.Formula = "=SUM(W12:W" + (row - 1);
                objRange = objSheet.get_Range("X" + row, "X" + row); objRange.Formula = "=SUM(X12:X" + (row - 1);
                objRange = objSheet.get_Range("Y" + row, "Y" + row); objRange.Formula = "=SUM(Y12:Y" + (row - 1);
                objRange = objSheet.get_Range("Z" + row, "Z" + row); objRange.Formula = "=SUM(Z12:Z" + (row - 1);
                //objRange = objSheet.get_Range("Y" + row, "Y" + row); objRange.Formula = "=SUM(Y12:Y" + (row - 1);

                int end = row;

                #region [Draw Lines]
                objRange = objSheet.get_Range("A" + start, "AA" + end);
                objRange.Borders.Color = 1;
                objRange = objSheet.get_Range("F12", "G" + (end - 1));
                objRange.NumberFormat = "mm/dd/yyy";
                objRange = objSheet.get_Range("H12", "Z" + end);
                objRange.NumberFormat = "#,##0.00";
                #endregion

                objSheet.Columns.AutoFit();
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            finally
            {
                objApp = null;
                objBooks = null;
                objBook = null;
                objSheet = null;
                objRange = null;
            }
        }

        public static void createExcelFileForAlphalist73(string[] CompanyProfile, string ReportTitle, string SubreportTitle,
            DataView dView, string filename, int columnCounter, string[] columnNames)
        {
            System.Data.DataTable dt = dView.ToTable();
            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Range objRange;
            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int startRow = 0;

                objRange = objSheet.get_Range("A1", "A8");
                objRange.Font.Bold = true;

                for (int i = 1; i <= 7; i++)
                {
                    objRange = objSheet.get_Range("A" + i, "M" + i);
                    objRange.Merge(true);
                }

                for (int cp = 0; cp < CompanyProfile.Length; cp++)
                {
                    objSheet.Cells[startRow + 1, 1] = CompanyProfile[cp].ToString();
                    startRow++;
                }

                objSheet.Cells[startRow + 2, 1] = ReportTitle;
                startRow += 2;

                //objSheet.Cells[startRow + 2, 1] = SubreportTitle;
                //startRow += 3;

                int start = startRow;

                #region [generate fixed header]
                #region [merge up to down]
                objRange = objSheet.get_Range("A" + startRow, "A" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("B" + startRow, "B" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("C" + (startRow + 1), "C" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("D" + (startRow + 1), "D" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("E" + (startRow + 1), "E" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("F" + (startRow + 1), "F" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("R" + startRow, "R" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("S" + startRow, "S" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("T" + startRow, "T" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("U" + startRow, "U" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("V" + (startRow + 1), "V" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("W" + (startRow + 1), "W" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("X" + startRow, "X" + (startRow + 2));
                objRange.Merge(false);

                objRange = objSheet.get_Range("Y" + startRow, "Y" + (startRow + 2));
                objRange.Merge(false);
                #endregion

                #region [merge left to right]
                objRange = objSheet.get_Range("C" + startRow, "E" + startRow);
                objRange.Merge(true);

                objRange = objSheet.get_Range("F" + startRow, "O" + startRow);
                objRange.Merge(true);

                objRange = objSheet.get_Range("G" + (startRow + 1), "K" + (startRow + 1));
                objRange.Merge(true);

                objRange = objSheet.get_Range("L" + (startRow + 1), "O" + (startRow + 1));
                objRange.Merge(true);

                objRange = objSheet.get_Range("V" + startRow, "W" + startRow);
                objRange.Merge(true);
                #endregion

                #region [merge both]
                objRange = objSheet.get_Range("P" + (startRow), "Q" + (startRow + 1));
                objRange.Merge(true);
                objRange.Merge(false);
                #endregion
                #endregion

                #region [naming of generated fixed header]
                startRow = startRow - 1;
                objSheet.Cells[startRow + 1, 1] = "SEQ NO.";
                objSheet.Cells[startRow + 1, 2] = "TIN";
                objSheet.Cells[startRow + 1, 3] = "NAME OF EMPLOYEES";
                objSheet.Cells[startRow + 2, 3] = "Last Name";
                objSheet.Cells[startRow + 2, 4] = "First Name";
                objSheet.Cells[startRow + 2, 5] = "Middle Name";
                objSheet.Cells[startRow + 1, 6] = "(4) GROSS COMPENSATION INCOME";
                objSheet.Cells[startRow + 2, 6] = "Gross Compensation Income";
                objSheet.Cells[startRow + 2, 7] = "NON-TAXABLE";
                objSheet.Cells[startRow + 3, 7] = "13th Month Pay & Other Benefits";
                objSheet.Cells[startRow + 3, 8] = "De Minimis Benefits";
                objSheet.Cells[startRow + 3, 9] = "SSS, GSIS, PHIC, & Pag-ibig Contibutions, and Union Dues";
                objSheet.Cells[startRow + 3, 10] = "Salaries & Other Forms of Compensation";
                objSheet.Cells[startRow + 3, 11] = "Total Non-Taxable/Exempt Compensation Income";
                objSheet.Cells[startRow + 2, 12] = "TAXABLE";
                objSheet.Cells[startRow + 3, 12] = "Basic Salary";
                objSheet.Cells[startRow + 3, 13] = "13 Month Pay & Other Benefits";
                objSheet.Cells[startRow + 3, 14] = "Salaries & Other Forms of Compensation";
                objSheet.Cells[startRow + 3, 15] = "Total Taxable Compensation Income";
                objSheet.Cells[startRow + 1, 16] = "EXEMPTION";
                objSheet.Cells[startRow + 3, 16] = "Code";
                objSheet.Cells[startRow + 3, 17] = "Amount";
                objSheet.Cells[startRow + 1, 18] = "Premium Paid on Health and/or Hospital Insurance";
                objSheet.Cells[startRow + 1, 19] = "Net Taxable Compensation Income";
                objSheet.Cells[startRow + 1, 20] = "TAX - DUE   (JAN. - DEC.) ";
                objSheet.Cells[startRow + 1, 21] = "TAX - WITHHELD  (JAN. - NOV.) ";
                objSheet.Cells[startRow + 1, 22] = "YEAR-END ADJUSTMENT (10a) or (10b)";
                objSheet.Cells[startRow + 2, 22] = "AMOUNT WITHHELD AND PAID FOR IN DECEMBER";
                objSheet.Cells[startRow + 2, 23] = "OVER WITHHELD TAX REFUNDED TO EMPLOYEE";
                objSheet.Cells[startRow + 1, 24] = "AMOUNT OF TAX WITHHELD AS ADJUSTED (to be reflected in BIR Form No. 2316)";
                objSheet.Cells[startRow + 1, 25] = "Substitute Filing? Yes/No";
                #endregion

                startRow += 3;

                int row = startRow;
                for (int col = 0; col < columnCounter; col++)
                {
                    objSheet.Cells[row + 1, col + 1] = "'" + columnNames[col].ToString();
                }

                row = startRow;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        row++;
                        for (int col = 0; col < columnCounter; col++)
                        {
                            objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                        }
                    }
                    row += 2;
                }
                else
                {
                    row += 3;
                }

                objRange = objSheet.get_Range("A" + row, "E" + row);
                objRange.Merge(true);
                objSheet.Cells[row, 1] = "TOTAL";

                objRange = objSheet.get_Range("F" + row, "F" + row); objRange.Formula = "=SUM(F12:F" + (row - 1);
                objRange = objSheet.get_Range("G" + row, "G" + row); objRange.Formula = "=SUM(G12:G" + (row - 1);
                objRange = objSheet.get_Range("H" + row, "H" + row); objRange.Formula = "=SUM(H12:H" + (row - 1);
                objRange = objSheet.get_Range("I" + row, "I" + row); objRange.Formula = "=SUM(I12:I" + (row - 1);
                objRange = objSheet.get_Range("J" + row, "J" + row); objRange.Formula = "=SUM(J12:J" + (row - 1);
                objRange = objSheet.get_Range("K" + row, "K" + row); objRange.Formula = "=SUM(K12:K" + (row - 1);
                objRange = objSheet.get_Range("L" + row, "L" + row); objRange.Formula = "=SUM(L12:L" + (row - 1);
                objRange = objSheet.get_Range("M" + row, "M" + row); objRange.Formula = "=SUM(M12:M" + (row - 1);
                objRange = objSheet.get_Range("N" + row, "N" + row); objRange.Formula = "=SUM(N12:N" + (row - 1);
                objRange = objSheet.get_Range("O" + row, "O" + row); objRange.Formula = "=SUM(O12:O" + (row - 1);
                objRange = objSheet.get_Range("P" + row, "P" + row); objRange.Formula = "=SUM(P12:P" + (row - 1);
                objRange = objSheet.get_Range("Q" + row, "Q" + row); objRange.Formula = "=SUM(Q12:Q" + (row - 1);
                objRange = objSheet.get_Range("R" + row, "R" + row); objRange.Formula = "=SUM(R12:R" + (row - 1);
                objRange = objSheet.get_Range("S" + row, "S" + row); objRange.Formula = "=SUM(S12:S" + (row - 1);
                objRange = objSheet.get_Range("T" + row, "T" + row); objRange.Formula = "=SUM(T12:T" + (row - 1);
                objRange = objSheet.get_Range("U" + row, "U" + row); objRange.Formula = "=SUM(U12:U" + (row - 1);
                objRange = objSheet.get_Range("V" + row, "V" + row); objRange.Formula = "=SUM(V12:V" + (row - 1);
                objRange = objSheet.get_Range("W" + row, "W" + row); objRange.Formula = "=SUM(W12:W" + (row - 1);
                objRange = objSheet.get_Range("X" + row, "X" + row); objRange.Formula = "=SUM(X12:X" + (row - 1);
                //objRange = objSheet.get_Range("Y" + row, "Y" + row); objRange.Formula = "=SUM(Y12:Y" + (row - 1);

                int end = row;

                #region [Draw Lines]
                objRange = objSheet.get_Range("A" + start, "Y" + end);
                objRange.Borders.Color = 1;
                objRange = objSheet.get_Range("F12", "X" + end);
                objRange.NumberFormat = "#,##0.00";
                #endregion

                objSheet.Columns.AutoFit();
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            finally
            {
                objApp = null;
                objBooks = null;
                objBook = null;
                objSheet = null;
                objRange = null;
            }
        }

        public static void createExcelFileForAlphalist74(string[] CompanyProfile, string ReportTitle, string SubreportTitle,
            DataView dView, string filename, int columnCounter, string[] columnNames)
        {
            System.Data.DataTable dt = dView.ToTable();
            Excel.Application objApp = new Excel.Application();
            Excel.Workbooks objBooks = objApp.Workbooks;
            Excel.Workbook objBook = objBooks.Add(Missing.Value);
            Excel.Worksheet objSheet = null;
            Excel.Range objRange;
            try
            {
                objSheet = (Excel.Worksheet)objBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                ((Excel.Worksheet)objBook.Sheets[1]).Select(Missing.Value);

                int startRow = 0;

                objRange = objSheet.get_Range("A1", "A8");
                objRange.Font.Bold = true;

                for (int i = 1; i <= 7; i++)
                {
                    objRange = objSheet.get_Range("A" + i, "M" + i);
                    objRange.Merge(true);
                }

                for (int cp = 0; cp < CompanyProfile.Length; cp++)
                {
                    objSheet.Cells[startRow + 1, 1] = CompanyProfile[cp].ToString();
                    startRow++;
                }

                objSheet.Cells[startRow + 2, 1] = ReportTitle;
                startRow += 2;

                //objSheet.Cells[startRow + 2, 1] = SubreportTitle;
                //startRow += 3;

                int start = startRow;

                #region [generate fixed header]
                #region [merge up to down]
                objRange = objSheet.get_Range("A" + startRow, "A" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("B" + startRow, "B" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("C" + (startRow + 2), "C" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("D" + (startRow + 2), "D" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("E" + (startRow + 2), "E" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("F" + (startRow + 1), "F" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("X" + (startRow + 1), "X" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("Y" + (startRow + 1), "Y" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("AB" + (startRow + 1), "AB" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("AC" + (startRow + 1), "AC" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("AD" + (startRow + 1), "AD" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("AG" + (startRow + 2), "AG" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("AH" + (startRow + 2), "AH" + (startRow + 3));
                objRange.Merge(false);
                objRange = objSheet.get_Range("AI" + (startRow + 1), "AI" + (startRow + 3));
                objRange.Merge(false);
                #endregion

                #region [merge left to right]
                objRange = objSheet.get_Range("F" + startRow, "AI" + startRow);
                objRange.Merge(true);
                objRange = objSheet.get_Range("G" + (startRow + 1), "O" + (startRow + 1));
                objRange.Merge(true);
                objRange = objSheet.get_Range("G" + (startRow + 2), "K" + (startRow + 2));
                objRange.Merge(true);
                objRange = objSheet.get_Range("L" + (startRow + 2), "O" + (startRow + 2));
                objRange.Merge(true);
                objRange = objSheet.get_Range("P" + (startRow + 1), "W" + (startRow + 1));
                objRange.Merge(true);
                objRange = objSheet.get_Range("P" + (startRow + 2), "T" + (startRow + 2));
                objRange.Merge(true);
                objRange = objSheet.get_Range("U" + (startRow + 2), "W" + (startRow + 2));
                objRange.Merge(true);
                objRange = objSheet.get_Range("AG" + (startRow + 1), "AH" + (startRow + 1));
                objRange.Merge(true);
                #endregion

                #region [merge both]
                objRange = objSheet.get_Range("C" + (startRow), "E" + (startRow + 1));
                objRange.Merge(true);
                objRange.Merge(false);
                objRange = objSheet.get_Range("Z" + (startRow + 1), "AA" + (startRow + 2));
                objRange.Merge(true);
                objRange.Merge(false);
                objRange = objSheet.get_Range("AE" + (startRow + 1), "AF" + (startRow + 2));
                objRange.Merge(true);
                objRange.Merge(false);
                #endregion
                #endregion

                #region [naming of generated fixed header]
                startRow = startRow - 1;
                objSheet.Cells[startRow + 1, 1] = "SEQ NO.";
                objSheet.Cells[startRow + 1, 2] = "TIN";
                objSheet.Cells[startRow + 1, 3] = "NAME OF EMPLOYEE";
                objSheet.Cells[startRow + 1, 6] = "GROSS COMPENSATION INCOME";

                objSheet.Cells[startRow + 2, 6] = "GROSS COMPENSATION INCOME";
                objSheet.Cells[startRow + 2, 7] = "PREVIOUS EMPLOYER";
                objSheet.Cells[startRow + 2, 16] = "PRESENT EMPLOYER";
                objSheet.Cells[startRow + 2, 24] = "TOTAL COMPENSATION (PRESENT)";
                objSheet.Cells[startRow + 2, 25] = "TOTAL TAXABLE (PREVIOUS AND PRESENT EMPLOYERS)";
                objSheet.Cells[startRow + 2, 26] = "EXEMPTION";
                objSheet.Cells[startRow + 2, 28] = "PREMIUM PAID ON HEALTH AND/OR HOSPITAL INSURANCE";
                objSheet.Cells[startRow + 2, 29] = "NET TAXABLE COMPENSATION INCOME";
                objSheet.Cells[startRow + 2, 30] = "TAX DUE (JAN. - DEC.)";
                objSheet.Cells[startRow + 2, 31] = "TAX WITHHELD (JAN. - NOV.)";
                objSheet.Cells[startRow + 2, 33] = "YEAR-END ADJUSTMENT (10a - 10b)";
                objSheet.Cells[startRow + 2, 35] = "AMOUNT OF TAX WITHHELD AS ADJUSTED (To be reflected in BIR Form No. 2316 issued by the present employer)";

                objSheet.Cells[startRow + 3, 3] = "LAST NAME";
                objSheet.Cells[startRow + 3, 4] = "FIRST NAME";
                objSheet.Cells[startRow + 3, 5] = "MIDDLE NAME";
                objSheet.Cells[startRow + 3, 7] = "NON-TAXABLE";
                objSheet.Cells[startRow + 3, 12] = "TAXABLE";
                objSheet.Cells[startRow + 3, 16] = "NON-TAXABLE";
                objSheet.Cells[startRow + 3, 21] = "TAXABLE";
                objSheet.Cells[startRow + 3, 33] = "AMOUNT WITHHELD AND PAID FOR IN DECEMBER";
                objSheet.Cells[startRow + 3, 34] = "OVER WITHHELD TAX REFUND TO EMPLOYEE";

                objSheet.Cells[startRow + 4, 7] = "13TH MONTH PAY AND OTHER BENEFITS";
                objSheet.Cells[startRow + 4, 8] = "DE MINIMIS BENEFITS";
                objSheet.Cells[startRow + 4, 9] = "SSS, GSIS, PHIC AND PAG-IBIG CONTRIBUTIONS, AND UNION DUES ";
                objSheet.Cells[startRow + 4, 10] = "SALARIES AND OTHER FORMS OF COMPENSATION";
                objSheet.Cells[startRow + 4, 11] = "TOTAL NON-TAXABLE COMPENSATION INCOME (PREVIOUS)";
                objSheet.Cells[startRow + 4, 12] = "BASIC SALARY";
                objSheet.Cells[startRow + 4, 13] = "13TH MONTH PAY AND OTHER BENEFITS";
                objSheet.Cells[startRow + 4, 14] = "SALARIES AND OTHER FORMS OF COMPENSATION";
                objSheet.Cells[startRow + 4, 15] = "TOTAL TAXABLE (PREVIOUS EMPLOYER)";
                objSheet.Cells[startRow + 4, 16] = "13TH MONTH PAY AND OTHER BENEFITS";
                objSheet.Cells[startRow + 4, 17] = "DE MINIMIS BENEFITS";
                objSheet.Cells[startRow + 4, 18] = "SSS, GSIS, PHIC AND PAG-IBIG CONTRIBUTIONS, AND UNION DUES ";
                objSheet.Cells[startRow + 4, 19] = "SALARIES AND OTHER FORMS OF COMPENSATION";
                objSheet.Cells[startRow + 4, 20] = "TOTAL NON-TAXABLE COMPENSATION INCOME (PRESENT)";
                objSheet.Cells[startRow + 4, 21] = "BASIC SALARY";
                objSheet.Cells[startRow + 4, 22] = "13TH MONTH PAY AND OTHER BENEFITS";
                objSheet.Cells[startRow + 4, 23] = "SALARIES AND OTHER FORMS OF COMPENSATION";
                objSheet.Cells[startRow + 4, 26] = "CODE";
                objSheet.Cells[startRow + 4, 27] = "AMOUNT";
                objSheet.Cells[startRow + 4, 31] = "PREVIOUS EMPLOYER";
                objSheet.Cells[startRow + 4, 32] = "PRESENT EMPLOYER";
                #endregion

                startRow += 4;

                int row = startRow;
                for (int col = 0; col < columnCounter; col++)
                {
                    objSheet.Cells[row + 1, col + 1] = "'" + columnNames[col].ToString();
                }

                row = startRow;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        row++;
                        for (int col = 0; col < columnCounter; col++)
                        {
                            objSheet.Cells[row + 1, col + 1] = dr[col].ToString();
                        }
                    }
                    row += 2;
                }
                else
                {
                    row += 3;
                }

                objRange = objSheet.get_Range("A" + row, "E" + row);
                objRange.Merge(true);
                objSheet.Cells[row, 1] = "TOTAL";

                objRange = objSheet.get_Range("F" + row, "F" + row); objRange.Formula = "=SUM(F13:F" + (row - 1);
                objRange = objSheet.get_Range("G" + row, "G" + row); objRange.Formula = "=SUM(G13:G" + (row - 1);
                objRange = objSheet.get_Range("H" + row, "H" + row); objRange.Formula = "=SUM(H13:H" + (row - 1);
                objRange = objSheet.get_Range("I" + row, "I" + row); objRange.Formula = "=SUM(I13:I" + (row - 1);
                objRange = objSheet.get_Range("J" + row, "J" + row); objRange.Formula = "=SUM(J13:J" + (row - 1);
                objRange = objSheet.get_Range("K" + row, "K" + row); objRange.Formula = "=SUM(K13:K" + (row - 1);
                objRange = objSheet.get_Range("L" + row, "L" + row); objRange.Formula = "=SUM(L13:L" + (row - 1);
                objRange = objSheet.get_Range("M" + row, "M" + row); objRange.Formula = "=SUM(M13:M" + (row - 1);
                objRange = objSheet.get_Range("N" + row, "N" + row); objRange.Formula = "=SUM(N13:N" + (row - 1);
                objRange = objSheet.get_Range("O" + row, "O" + row); objRange.Formula = "=SUM(O13:O" + (row - 1);
                objRange = objSheet.get_Range("P" + row, "P" + row); objRange.Formula = "=SUM(P13:P" + (row - 1);
                objRange = objSheet.get_Range("Q" + row, "Q" + row); objRange.Formula = "=SUM(Q13:Q" + (row - 1);
                objRange = objSheet.get_Range("R" + row, "R" + row); objRange.Formula = "=SUM(R13:R" + (row - 1);
                objRange = objSheet.get_Range("S" + row, "S" + row); objRange.Formula = "=SUM(S13:S" + (row - 1);
                objRange = objSheet.get_Range("T" + row, "T" + row); objRange.Formula = "=SUM(T13:T" + (row - 1);
                objRange = objSheet.get_Range("U" + row, "U" + row); objRange.Formula = "=SUM(U13:U" + (row - 1);
                objRange = objSheet.get_Range("V" + row, "V" + row); objRange.Formula = "=SUM(V13:V" + (row - 1);
                objRange = objSheet.get_Range("W" + row, "W" + row); objRange.Formula = "=SUM(W13:W" + (row - 1);
                objRange = objSheet.get_Range("X" + row, "X" + row); objRange.Formula = "=SUM(X13:X" + (row - 1);
                objRange = objSheet.get_Range("Y" + row, "Y" + row); objRange.Formula = "=SUM(Y13:Y" + (row - 1);
                objRange = objSheet.get_Range("AA" + row, "AA" + row); objRange.Formula = "=SUM(AA13:AA" + (row - 1);
                objRange = objSheet.get_Range("AB" + row, "AB" + row); objRange.Formula = "=SUM(AB13:AB" + (row - 1);
                objRange = objSheet.get_Range("AC" + row, "AC" + row); objRange.Formula = "=SUM(AC13:AC" + (row - 1);
                objRange = objSheet.get_Range("AD" + row, "AD" + row); objRange.Formula = "=SUM(AD13:AD" + (row - 1);
                objRange = objSheet.get_Range("AE" + row, "AE" + row); objRange.Formula = "=SUM(AE13:AE" + (row - 1);
                objRange = objSheet.get_Range("AF" + row, "AF" + row); objRange.Formula = "=SUM(AF13:AF" + (row - 1);
                objRange = objSheet.get_Range("AG" + row, "AG" + row); objRange.Formula = "=SUM(AG13:AG" + (row - 1);
                objRange = objSheet.get_Range("AH" + row, "AH" + row); objRange.Formula = "=SUM(AH13:AH" + (row - 1);
                objRange = objSheet.get_Range("AI" + row, "AI" + row); objRange.Formula = "=SUM(AI13:AI" + (row - 1);

                int end = row;

                #region [Draw Lines]
                objRange = objSheet.get_Range("A" + start, "AI" + end);
                objRange.Borders.Color = 1;
                objRange = objSheet.get_Range("F13", "AI" + end);
                objRange.NumberFormat = "#,##0.00";
                #endregion

                objSheet.Columns.AutoFit();
                objBook.Close(true, filename, Missing.Value);
                objBooks.Close();
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            finally
            {
                objApp = null;
                objBooks = null;
                objBook = null;
                objSheet = null;
                objRange = null;
            }
        }

        public static string createAffectedInformation(string[] records, string header, int recordperline)
        {
            StringBuilder affectedInformation = new StringBuilder();
            affectedInformation.AppendFormat(header);
            for (int i = 0, j = 0; i < records.Length; i = j)
            {
                for (j = i; j < records.Length && j - i < recordperline; ++j)
                {
                    affectedInformation.AppendFormat("{0},", records[j]);
                }
                affectedInformation.Remove(affectedInformation.ToString().Length - 1, 1);
                affectedInformation.Append("\n");
            }
            return affectedInformation.ToString();
        }

        public static string createAffectedInformation(System.Data.DataTable tablerecords, string header, int recordperline)
        {
            string[] records = new string[tablerecords.Rows.Count];
            for (int i = 0; i < tablerecords.Rows.Count; ++i)
            {
                records[i] = tablerecords.Rows[i][0].ToString();
                for (int j = 1; j < tablerecords.Columns.Count; ++j)
                    records[i] = records[i] + "." + tablerecords.Rows[i][j].ToString();
            }
            return createAffectedInformation(records, header, recordperline);
        }

        public static bool checkIfYes(string message)
        {
            if (MessageBox.Show(message, "Human Resource Companion", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                return true;
            else
                return false;
        }

        public static CommonEnum.GenieDialogResult showMessageQuestionYesNo(string message)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(CommonEnum.GenieMessageBoxIcon.CONFIRMATION, message);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static string generateLastSeqNo(int lastSeqNo, int totalWidth)
        {
            return lastSeqNo.ToString().PadLeft(totalWidth, '0');
        }

        public static bool GetProcessFlag(string SystemID, string ProcessID)
        {
            #region query
            string query = string.Format(
                @"SELECT
	                Tsc_SetFlag
                FROM T_SettingControl
                WHERE Tsc_SystemCode = '{0}'
	                AND Tsc_SettingCode = '{1}'"
                , SystemID
                , ProcessID
                , LoginInfo.getUser().CompanyCode);
            #endregion

            System.Data.DataTable dtResult = new System.Data.DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                return Convert.ToBoolean(dtResult.Rows[0][0]);
            else
                return false;
        }

        public static string GetSystemMessage(int MessageCode)
        {
            return promptBL.GetSystemMessage(MessageCode);
        }

        public static string GetSystemMessage(int MessageCode, string Field1)
        {
            return string.Format(promptBL.GetSystemMessage(MessageCode), Field1);
        }

        public static string GetSystemMessage(int MessageCode, string Field1, string Field2)
        {
            return string.Format(promptBL.GetSystemMessage(MessageCode), Field1, Field2);
        }

        public static string GetSystemMessage(int MessageCode, string Field1, string Field2, string Field3)
        {
            return string.Format(promptBL.GetSystemMessage(MessageCode), Field1, Field2, Field3);
        }

        public static bool ShowMessageCutOffAll(bool cutoff)
        {
            bool enable = false;

            #region Identify cutoff flags
            bool ProcessTimekeepCutoff = GetProcessFlag("TIME", "CUT-OFF");
            bool ProcessOvertimeCutoff = GetProcessFlag("OVERTIME", "CUT-OFF");
            bool ProcessLeaveCutoff    = GetProcessFlag("LEAVE", "CUT-OFF");
            bool ProcessPayrollCutoff  = GetProcessFlag("PAYROLL", "CUT-OFF");

            ProcessTimekeepCutoff = (cutoff) ? !ProcessTimekeepCutoff : ProcessTimekeepCutoff;
            ProcessOvertimeCutoff = (cutoff) ? !ProcessOvertimeCutoff : ProcessOvertimeCutoff;
            ProcessLeaveCutoff    = (cutoff) ? !ProcessLeaveCutoff : ProcessLeaveCutoff;
            ProcessPayrollCutoff  = (cutoff) ? !ProcessPayrollCutoff : ProcessPayrollCutoff;

            string flagCutoffs = "";
            List<string> flags = new List<string>();
            int flagNo = 1;

            if (ProcessTimekeepCutoff)
            {
                flagCutoffs += string.Format("\n {0}. Time", flagNo++);
                flags.Add("TIME");
            }
            if (ProcessOvertimeCutoff)
            {
                flagCutoffs += string.Format("\n {0}. Overtime", flagNo++);
                flags.Add("OVERTIME");
            }
            if (ProcessLeaveCutoff)
            {
                flagCutoffs += string.Format("\n {0}. Leave", flagNo++);
                flags.Add("LEAVE");
            }
            if (ProcessPayrollCutoff)
            {
                flagCutoffs += string.Format("\n {0}. Payroll", flagNo++);
                flags.Add("PAYROLL");
            }
            #endregion

            if (flagCutoffs != "")
            {
                #region Release/cut-off
                string ReleaseCutOff = string.Format("These flag(s) are already locked.{0}\nDo you want to unlock them?", flagCutoffs);
                string CutOffMessage = string.Format("These flag(s) are not locked yet.{0}\nDo you want to lock them?", flagCutoffs);
                string Message = (cutoff) ? CutOffMessage : ReleaseCutOff;

                List<string> allowedFlags = new List<string>();
                List<string> notAllowedFlags = new List<string>();
                foreach (string flag in flags)
                {
                    if (HasCutOffAccessRights(string.Format("{0}CUTOFF", flag)))
                    {
                        allowedFlags.Add(flag);
                    }
                    else
                    {
                        notAllowedFlags.Add(flag);
                    }
                }

                if (notAllowedFlags.Count == 0)
                {
                    if (showMessageQuestion(Message) == CommonEnum.GenieDialogResult.Yes)
                    {
                        foreach (string flag in allowedFlags)
                        {
                            CreateProcessControlTrail(cutoff.ToString(), flag, "CUT-OFF");
                            UpdateProcessControlFlag(cutoff.ToString(), flag, "CUT-OFF");
                        }
                        enable = true;
                    }
                }
                else
                {
                    flagNo = 1;
                    string menuCodesLack = string.Empty;
                    foreach (string flag in notAllowedFlags)
                    {
                        menuCodesLack += string.Format("\n{0}. {1}", flagNo++, flag);
                    }
                    string cutoffMessage = string.Empty;
                    if (!cutoff)
                        cutoffMessage = "The following is locked";
                    else
                        cutoffMessage = "The following is not locked yet";
                    showMessageInformation(string.Format("Cannot proceed. {0}:{1}", cutoffMessage, menuCodesLack));
                }
                #endregion
            }
            else
                enable = true;

            return enable;
        }

        public static bool showMessageCutOff(CommonLibrary.CommonEnum.Processes cutOffType, bool release)
        {
            bool enable = false;
            string message = "";
            string releaseCutOff = @"{0} is locked. Changes in {0}-related transaction is not available. Do you want to unlock it?";
            string cutOffMessage = @"{0} is not locked yet. Do you want to lock it?";
            string type = "";
            string menuCode = "";

            switch (cutOffType)
            {
                case CommonEnum.Processes.TIME:
                    type = "TIME";
                    menuCode = StringEnum.GetStringValue(CommonEnum.MenuCode.TIMECUTOFF);
                    break;
                case CommonEnum.Processes.PAYROLL:
                    type = "PAYROLL";
                    menuCode = StringEnum.GetStringValue(CommonEnum.MenuCode.PAYROLLCUTOFF);
                    break;
                case CommonEnum.Processes.LEAVE:
                    type = "LEAVE";
                    menuCode = StringEnum.GetStringValue(CommonEnum.MenuCode.LEAVECUTOFF);
                    break;
                case CommonEnum.Processes.OVERTIME:
                    type = "OVERTIME";
                    menuCode = StringEnum.GetStringValue(CommonEnum.MenuCode.OVERTIMECUTOFF);
                    break;
                case CommonEnum.Processes.CYCLE:
                    type = "CYCLE";
                    menuCode = StringEnum.GetStringValue(CommonEnum.MenuCode.CYCLECUTOFF);
                    break;
            }

            //check access right
            bool hasRights = HasCutOffAccessRights(menuCode);

            if (hasRights)
            {
                if (release)
                    message = string.Format(releaseCutOff, type);
                else
                    message = string.Format(cutOffMessage, type);

                CommonEnum.GenieDialogResult result = showMessageQuestion(message);

                if (result == CommonEnum.GenieDialogResult.Yes)
                {
                    CreateProcessControlTrail((!release).ToString(), type, "CUT-OFF");
                    UpdateProcessControlFlag((!release).ToString(), type, "CUT-OFF");

                    enable = true;
                }
            }
            else
            {
                if (release)
                    showMessageInformation(string.Format("Cannot proceed. {0} is locked.", type));
                else
                    showMessageInformation(string.Format("Cannot proceed. {0} is not locked yet.", type));
            }

            return enable;
        }

        private static bool HasCutOffAccessRights(string MenuCode)
        {
            bool enable = false;
            string UserCode = LoginInfo.getUser().UserCode;

            #region query
            string query = string.Format(@"
                            DECLARE @SYSTEMID VARCHAR(10)
                            DECLARE @GROUPCODE VARCHAR(15)

                            SET @SYSTEMID = (SELECT 
	                                            Msm_SystemCode --SystemID
                                            FROM M_SystemModule
                                            WHERE Msm_ModuleCode = '{0}'
                                            AND Msm_CompanyCode = '{2}'
                                            AND Msm_RecordStatus = 'A') --menuCode

                            SET @GROUPCODE = (SELECT
                                                Mud_UserRoleCode
                                            FROM M_UserDtl
                                            WHERE Mud_UserCode = '{1}' --UserCode
                                                AND Mud_SystemCode = @SYSTEMID
                                                AND Mud_CompanyCode = '{2}'
												AND Mud_ProfileCode = '{3}'
                                                AND Mud_RecordStatus = 'A')

                            SELECT
	                            Mra_CanView
                            FROM M_UserRoleAccess
                            WHERE Mra_ModuleCode = '{0}'
	                            AND Mra_UserRoleCode = @GROUPCODE --UserGroupCode
                                AND Mra_CompanyCode = '{2}'
                                AND Mra_RecordStatus = 'A'"
                            , MenuCode
                            , UserCode
                            , LoginInfo.getUser().CompanyCode
                            , LoginInfo.getUser().DBNumber);
            #endregion

            System.Data.DataTable dtResult = new System.Data.DataTable();
            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, false))
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dtResult.Rows[0][0]))
                    enable = true;
            }

            return enable;
        }

        public static string GetCurrentPayPeriod()
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = @"SELECT Tps_PayCycle 
                                    FROM T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                        AND Tps_RecordStatus = 'A'";

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        private static int UpdateProcessControlFlag(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region update query
            string Upstring = @"Update T_SettingControl
                                    Set Tsc_SetFlag = @Tsc_SetFlag
                                        ,Usr_Login = @Usr_Login
                                        ,Ludatetime = GetDate()
                                    Where Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode";
            #endregion

            Payroll.DAL.ParameterInfo[] UpparamInfo = new Payroll.DAL.ParameterInfo[4];
            UpparamInfo[0] = new Payroll.DAL.ParameterInfo("@Tsc_SetFlag", Tsc_SetFlag);
            UpparamInfo[1] = new Payroll.DAL.ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            UpparamInfo[2] = new Payroll.DAL.ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);
            UpparamInfo[3] = new Payroll.DAL.ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(Upstring, CommandType.Text, UpparamInfo);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        private static int CreateProcessControlTrail(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region update query
            string Upstring = @"Insert Into T_SettingControlTrl
                                SELECT Tsc_SystemCode 
                                     , Tsc_SettingCode 
                                     , Tsc_SettingName 
                                     , @ProcessFlag
                                     , Tsc_AccessType
                                     , @PayCycle
                                     , NULL
                                     , Tsc_RecordStatus
                                     , @Usr_Login
                                     , GETDATE()
                                FROM T_SettingControl
                                WHERE Tsc_SystemCode  = @SystemID
                                    AND Tsc_SettingCode = @ProcessID";
            #endregion

            Payroll.DAL.ParameterInfo[] UpparamInfo = new Payroll.DAL.ParameterInfo[5];
            UpparamInfo[0] = new Payroll.DAL.ParameterInfo("@ProcessFlag", Tsc_SetFlag);
            UpparamInfo[1] = new Payroll.DAL.ParameterInfo("@SystemID", Tsc_SystemCode);
            UpparamInfo[2] = new Payroll.DAL.ParameterInfo("@ProcessID", Tsc_SettingCode);
            UpparamInfo[3] = new Payroll.DAL.ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            UpparamInfo[4] = new Payroll.DAL.ParameterInfo("@PayCycle", GetCurrentPayPeriod());

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(Upstring, CommandType.Text, UpparamInfo);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        #region Message Calls

        public static CommonEnum.GenieDialogResult ShowMessage(int MessageCode, List<string> Fields)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(MessageCode, Fields);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        #region With menu codes
        public static CommonEnum.GenieDialogResult ShowMessage(int MessageCode, string MenuCode)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(MessageCode, MenuCode);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static CommonEnum.GenieDialogResult ShowMessage(int MessageCode, List<string> Fields, string MenuCode)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(MessageCode, Fields, MenuCode);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static CommonEnum.GenieDialogResult ShowMessage(int MessageCode, string Field, string MenuCode)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(MessageCode, Field, MenuCode);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static CommonEnum.GenieDialogResult ShowMessage(int MessageCode, string Field1, string Field2, string MenuCode)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(MessageCode, Field1, Field2, MenuCode);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static CommonEnum.GenieDialogResult ShowMessage(int MessageCode, string Field1, string Field2, string Field3, string MenuCode)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(MessageCode, Field1, Field2, Field3, MenuCode);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        #endregion
        #endregion

        public static void showMessageInformation(string message)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(CommonEnum.GenieMessageBoxIcon.INFORMATION, message);
            if (prompt.IsView)
                prompt.ShowDialog();
        }

        public static void showMessageError(string message)
        {

            frmGenericPrompt prompt = new frmGenericPrompt(CommonEnum.GenieMessageBoxIcon.ERROR, message);
            if (prompt.IsView)
                prompt.ShowDialog();
        }

        public static CommonEnum.GenieDialogResult showMessageWarning(string message)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(CommonEnum.GenieMessageBoxIcon.WARNING, message);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static CommonEnum.GenieDialogResult showMessageWarningOnly(string message)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(CommonEnum.GenieMessageBoxIcon.WARNING, message);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static CommonEnum.GenieDialogResult showMessageQuestion(string message)
        {
            frmGenericPrompt prompt = new frmGenericPrompt(CommonEnum.GenieMessageBoxIcon.CONFIRMATION, message);
            if (prompt.IsView)
                prompt.ShowDialog();

            return prompt.GenieDialogResult;
        }

        public static void logErrorToFile(string strLogText)
        {
            // Create a writer and open the file:
            StreamWriter log;
            string dir = @"C:\MinervaLogFile";  // folder location
            if (!Directory.Exists(dir))  // if it doesn't exist, create
                Directory.CreateDirectory(dir);

            if (!File.Exists(string.Format("C:\\MinervaLogFile\\{0:yyyyMMdd}.txt", DateTime.Now)))
                log = new StreamWriter(string.Format("C:\\MinervaLogFile\\{0:yyyyMMdd}.txt", DateTime.Now));
            else
                log = File.AppendText(string.Format("C:\\MinervaLogFile\\{0:yyyyMMdd}.txt", DateTime.Now));

            // Write to the file:
            log.WriteLine(DateTime.Now);
            log.WriteLine(strLogText);
            log.WriteLine();

            // Close the stream:
            log.Close();
        }

        public static string GetSQLError(int Severity)
        {
            string message = "";
            switch (Severity)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10: message = "System can't proceed with the transaction due to an unknown error.";
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16: message = "System can't proceed with the transaction due to an erroneous query.";
                    break;
                case 17: message = "System can't proceed with the transaction due to a locked resource.";
                    break;
                case 18: message = "System can't proceed with the transaction due to SQL internal software problem.";
                    break;
                case 19: message = "There is not enough resource to save the transaction.";
                    break;
                case 20:
                case 21: message = "There is a problem with the current SQL process.";
                    break;
                case 22: message = "System can't proceed with the transaction due to damaged table or index. Please contact your administrator.";
                    break;
                case 23: message = "Database is currently unavailable. Please contact your administrator.";
                    break;
                case 24: message = "There is a hardware problem in the system. Please contact your administrator.";
                    break;
                case 25: message = "Fatal error was encountered. Please contact your administrator.";
                    break;
            }
            return message;
        }

        public static string getAppFolderPath(string applicationPath, string folderName)
        {
            return System.IO.Path.GetFullPath(applicationPath + @"\..\.." + folderName);
        }

        public static DataRow getSelectedRow(System.Data.DataTable dt, string condition)
        {
            DataRow[] rows = dt.Select(condition);

            if (rows.Length > 0)
                return rows[0];
            else
                return null;
        }

        public static void setStatusValues(DataGridView dgView, string statParam)
        {
            for (int i = 0; i < dgView.RowCount; i++)
            {
                if (dgView.Rows[i].Cells[statParam].Value.ToString() == "A")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.ACTIVE;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "N")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.NEW;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "U")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.ONHOLD;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "C")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.CANCELLED;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "F")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.FULFILLED;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "G")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.GENERATED;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "T")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.GENERATED;
                else if (dgView.Rows[i].Cells[statParam].Value.ToString() == "I")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.INTRANSIT;
            }
        }
     
        public static string IntegerToWords(long inputNum)
        {
            int dig1;
            int dig2;
            int dig3;
            int level = 0;
            int lastTwo;
            int threeDigits;

            string retval = "";
            string x = "";
            string[] ones ={
                            "",
                            "one",
                            "two",
                            "three",
                            "four",
                            "five",
                            "six",
                            "seven",
                            "eight",
                            "nine",
                            "ten",
                            "eleven",
                            "twelve",
                            "thirteen",
                            "fourteen",
                            "fifteen",
                            "sixteen",
                            "seventeen",
                            "eighteen",
                            "nineteen"
                          };
            string[] tens ={
                            "",
                            "ten",
                            "twenty",
                            "thirty",
                            "forty",
                            "fifty",
                            "sixty",
                            "seventy",
                            "eighty",
                            "ninety"
                          };
            string[] thou ={
                            "",
                            "thousand",
                            "million",
                            "billion",
                            "trillion",
                            "quadrillion",
                            "quintillion"
                          };

            bool isNegative = false;
            if (inputNum < 0)
            {
                isNegative = true;
                inputNum *= -1;
            }

            if (inputNum == 0)
                return ("zero");

            string s = inputNum.ToString();

            while (s.Length > 0)
            {
                // Get the three rightmost characters
                x = (s.Length < 3) ? s : s.Substring(s.Length - 3, 3);

                // Separate the three digits
                threeDigits = int.Parse(x);
                lastTwo = threeDigits % 100;
                dig1 = threeDigits / 100;
                dig2 = lastTwo / 10;
                dig3 = (threeDigits % 10);

                // append a "thousand" where appropriate
                if (level > 0 && dig1 + dig2 + dig3 > 0)
                {
                    retval = thou[level] + " " + retval;
                    retval = retval.Trim();
                }

                // check that the last two digits is not a zero
                if (lastTwo > 0)
                {
                    if (lastTwo < 20) // if less than 20, use "ones" only
                        retval = ones[lastTwo] + " " + retval;
                    else // otherwise, use both "tens" and "ones" array
                        retval = tens[dig2] + " " + ones[dig3] + " " + retval;
                }

                // if a hundreds part is there, translate it
                if (dig1 > 0)
                    retval = ones[dig1] + " hundred " + retval;

                s = (s.Length - 3) > 0 ? s.Substring(0, s.Length - 3) : "";
                level++;
            }

            while (retval.IndexOf("  ") > 0)
                retval = retval.Replace("  ", " ");

            retval = retval.Trim();

            if (isNegative)
                retval = "negative " + retval;

            return (retval);
        }

        public static string GetAppSettingConfigString(string key, string defaultValue)
        {
            return GetAppSettingConfigString(key, defaultValue, GetAppSettingConfigBool("UseDecryption", false));
        }

        public static string GetAppSettingConfigString(string key, string defaultValue, bool useDecryption)
        {
            string configValue = (ConfigurationManager.AppSettings[key] != null ? ConfigurationManager.AppSettings[key].ToString() : defaultValue);

            return (useDecryption ? Encrypt.decryptText(configValue) : configValue);
        }

        public static int GetAppSettingConfigInt(string key, int defaultValue)
        {
            return GetAppSettingConfigInt(key, defaultValue, GetAppSettingConfigBool("UseDecryption", false));
        }

        public static int GetAppSettingConfigInt(string key, int defaultValue, bool useDecryption)
        {
            int configValue = 0;

            if (int.TryParse(ConfigurationManager.AppSettings[key], out configValue))
            {
                return Convert.ToInt32(Encrypt.decryptText(configValue.ToString()));
            }

            return Convert.ToInt32(Encrypt.decryptText(defaultValue.ToString()));
        }

        public static bool GetAppSettingConfigBool(string key, bool defaultValue)
        {
            return (ConfigurationManager.AppSettings[key] != null ? string.Compare(ConfigurationManager.AppSettings[key].ToString(), "true", true) == 0 : defaultValue);
        }

        /*===========================================================================
        * Function    : setFocusColor(object sender, EventArgs e, Color color)
        * Purpose     : This function sets the back color of the object
        *               Output: None                                       
        * Return      : None                                                               
       /*===========================================================================*/
        public static void setFocusColor(object sender, EventArgs e, Color color)
        {
            if (sender.GetType().ToString() == "System.Windows.Forms.TextBox")
                ((System.Windows.Forms.TextBox)sender).BackColor = color;
            else if (sender.GetType().ToString() == "System.Windows.Forms.ComboBox")
                ((System.Windows.Forms.ComboBox)sender).BackColor = color;
        }

        /// <summary>
        /// Regular expression, which is used to validate an E-Mail address.
        /// </summary>
        private const string MatchEmailPattern =
                  @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
           + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }
    }
}
