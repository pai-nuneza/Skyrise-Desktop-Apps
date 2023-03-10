//using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Payroll.DAL;
using System;
using System.Data;
using System.IO;

namespace CommonLibrary
{
    public class ExportToExcel
    {
        public string ExportExcelBPI(DataSet dsEmployees, string fileName, string accountNo, string transDate)
        {
            double totalNetPay = 0;

            for (int i = 0; i < dsEmployees.Tables[0].Rows.Count; i++)
            {
                double damount = Convert.ToDouble(dsEmployees.Tables[0].Rows[i]["Net Pay Amount"]);
                totalNetPay += damount;
            }


            var strMsg = string.Empty;
            try
            {
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excel = new ExcelPackage();

                // name of the sheet
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                // setting the properties
                // of the work sheet 
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;

                // Setting the properties
                // of the first row
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                // Header of the Excel sheet
                workSheet.Cells[1, 1].Value = "H";
                workSheet.Cells[1, 2].Value = "Payroll Date";
                workSheet.Cells[1, 3].Value = Convert.ToDateTime(transDate).ToString("MMMM dd, yyyy");
                workSheet.Cells[1, 4].Value = "Payroll Time";
                workSheet.Cells[1, 6].Value = "Total Amount";
                workSheet.Cells[1, 7].Value = totalNetPay;
                workSheet.Cells[1, 8].Value = "Total Count";
                workSheet.Cells[1, 9].Value = dsEmployees.Tables[0].Rows.Count;
                workSheet.Cells[1, 10].Value = "FUNDING ACCOUNT";
                workSheet.Cells[1, 11].Value = accountNo;

                workSheet.Cells[1, 7].Style.Numberformat.Format = "#,##0.00";


                // 2nd Row Header
                workSheet.Cells[2, 1].Value = "DETAIL CONSTANT";
                workSheet.Cells[2, 2].Value = "EMPLOYEE NAME";
                workSheet.Cells[2, 3].Value = "EMPLOYEE ACCOUNT";
                workSheet.Cells[2, 4].Value = "AMOUNT";
                workSheet.Cells[2, 5].Value = "REMARKS";


                int recordIndex = 3;

                for (int i = 0; i < dsEmployees.Tables[0].Rows.Count; i++)
                {
                    workSheet.Cells[recordIndex, 1].Value = "D";
                    workSheet.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[recordIndex, 2].Value = dsEmployees.Tables[0].Rows[i]["Employee Name"].ToString().Trim();
                    workSheet.Cells[recordIndex, 3].Value = dsEmployees.Tables[0].Rows[i]["Bank Account No"].ToString().Trim();
                    workSheet.Cells[recordIndex, 4].Value = dsEmployees.Tables[0].Rows[i]["Net Pay Amount"];
                    workSheet.Cells[recordIndex, 5].Value = dsEmployees.Tables[0].Rows[i]["Remarks"].ToString().Trim();

                    workSheet.Cells[recordIndex, 4].Style.Numberformat.Format = "#,##0.00";
                    recordIndex++;
                }

                // By default, the column width is not 
                // set to auto fit for the content
                // of the range, so we are using
                // AutoFit() method here. 

                for (int i = 1; i < 13; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                // file name with .xlsx extension 
                string p_strPath = fileName;

                if (File.Exists(p_strPath))
                    File.Delete(p_strPath);

                // Create excel file on physical disk 
                FileStream objFileStrm = File.Create(p_strPath);
                objFileStrm.Close();

                // Write content to excel file 
                File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
                //Close Excel package
                excel.Dispose();
                CommonProcedures.showMessageInformation("BPI Payroll Bank Remittance Generation Successful!");
            }
            catch (Exception)
            {
                strMsg += "\nCan't generate the file";
                throw;
            }

            return strMsg;
        }

        public string ExportExcelChinaBank(DataSet dsEmployees, string fileName, string accountNo, string transDate, string companyCode, string centralProfile)
        {

            double totalAmount = 0;

            for (int i = 0; i < dsEmployees.Tables[0].Rows.Count; i++)
            {
                double damount = Convert.ToDouble(dsEmployees.Tables[0].Rows[i]["Net Pay Amount"]);
                totalAmount += damount;
            }

            var strMsg = string.Empty;
            try
            {
               // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excel = new ExcelPackage();

                // name of the sheet
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                // setting the properties
                // of the work sheet 
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;

                // Setting the properties
                // of the first row
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                DataSet bankDetails = GetBankDetails(companyCode, "CB", centralProfile);
                DataSet currencyWord = CurrencyToWords(centralProfile, totalAmount.ToString());
                // Header of the Excel sheet
                workSheet.Cells[1, 1].Value = "Date";
                workSheet.Cells[1, 2].Value = Convert.ToDateTime(transDate).ToString("MMMM dd, yyyy");

                workSheet.Cells[3, 1].Value = bankDetails.Tables[0].Rows[0]["Mbn_BankInChargeName"].ToString();
                workSheet.Cells[4, 1].Value = bankDetails.Tables[0].Rows[0]["Mbn_BankInChargeJobTitle"].ToString();
                workSheet.Cells[5, 1].Value = bankDetails.Tables[0].Rows[0]["Mbn_BankName"].ToString();
                workSheet.Cells[6, 1].Value = bankDetails.Tables[0].Rows[0]["Mbn_BankAddress"].ToString();

                workSheet.Cells[8, 1].Value = "Dear Mr. Osmena:";

                workSheet.Cells[9, 1].Value = string.Format("Kindly debit the amount of {0}", currencyWord.Tables[0].Rows[0]["CurrencyWords"].ToString());
                workSheet.Cells[10, 1].Value = string.Format("(PHP {0}) from the account number {1} for the payroll of the following employees", String.Format("{0:n}", totalAmount), accountNo);


                // 2nd Row Header
                workSheet.Cells[13, 1].Value = string.Empty;
                workSheet.Cells[13, 2].Value = "Account Name";
                workSheet.Cells[13, 3].Value = "Account Number";
                workSheet.Cells[13, 4].Value = "Amount";


                int recordIndex = 14;
                int counter = 1;
                for (int i = 0; i < dsEmployees.Tables[0].Rows.Count; i++)
                {
                    workSheet.Cells[recordIndex, 1].Value = counter.ToString();
                    workSheet.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    workSheet.Cells[recordIndex, 2].Value = dsEmployees.Tables[0].Rows[i]["Employee Name"].ToString().Trim();
                    workSheet.Cells[recordIndex, 3].Value = dsEmployees.Tables[0].Rows[i]["Bank Account No"].ToString().Trim();
                    workSheet.Cells[recordIndex, 4].Value = Convert.ToDouble(dsEmployees.Tables[0].Rows[i]["Net Pay Amount"]);

                    workSheet.Cells[recordIndex, 4].Style.Numberformat.Format = "#,##0.00";
                    recordIndex++;
                    counter++;
                }

                int preparedByRow = recordIndex + 2;
                workSheet.Cells[preparedByRow, 1].Value = "Prepared by: ";
                workSheet.Cells[preparedByRow + 3, 1].Value = bankDetails.Tables[0].Rows[0]["Mbn_PayrollSignatory"].ToString();

                int approvedByRow = preparedByRow + 6;
                workSheet.Cells[approvedByRow, 1].Value = "Approved by: ";
                workSheet.Cells[approvedByRow + 3, 1].Value = bankDetails.Tables[0].Rows[0]["Mbn_PayrollSignatory2"].ToString();


                // By default, the column width is not 
                // set to auto fit for the content
                // of the range, so we are using
                // AutoFit() method here. 

                workSheet.Column(1).Width = 15;
                for (int i = 2; i < 10; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                // file name with .xlsx extension 
                string p_strPath = fileName;

                if (File.Exists(p_strPath))
                    File.Delete(p_strPath);

                // Create excel file on physical disk 
                FileStream objFileStrm = File.Create(p_strPath);
                objFileStrm.Close();

                // Write content to excel file 
                File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
                //Close Excel package
                excel.Dispose();
                CommonProcedures.showMessageInformation("Chinabank Payroll Bank Remittance Generation Successful!");
            }
            catch (Exception)
            {
                strMsg += "\nCan't generate the file";
                throw;
            }

            return strMsg;
        }

        public DataSet GetBankDetails(string companyCode, string bankCode, string centralProfile)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(centralProfile, false))
            {
                try
                {
                    dal.OpenDB();

                    string query = string.Format(@"SELECT Mbn_BankInChargeName
                        , Mbn_BankInChargeJobTitle
                        , Mbn_BankName
                        , Mbn_BankAddress
                        , dbo.Udf_GetTitleCase(RTRIM(Sig1.Mem_Firstname) + ' ' + RTRIM(Sig1.Mem_LastName)) as Mbn_PayrollSignatory
                        , dbo.Udf_GetTitleCase(RTRIM(Sig2.Mem_Firstname) + ' ' + RTRIM(Sig2.Mem_LastName))  as Mbn_PayrollSignatory2
                        FROM M_Bank
                        LEFT JOIN M_Employee Sig1 ON Sig1.Mem_IDNo = Mbn_PayrollSignatory
                        LEFT JOIN M_Employee Sig2 ON Sig2.Mem_IDNo = Mbn_PayrollSignatory2
                        WHERE Mbn_CompanyCode = '{0}'
                        AND Mbn_BankCode = '{1}'", companyCode, bankCode);

                    ds = dal.ExecuteDataSet(query);

                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageInformation(er.Message);
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }

                return ds;
            }
        }

        public DataSet CurrencyToWords(string centralProfile, string amount)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(centralProfile, false))
            {
                try
                {
                    dal.OpenDB();

                    string query = string.Format(@"select dbo.Udf_CurrencyToWords({0}) AS CurrencyWords", amount);
                    ds = dal.ExecuteDataSet(query);

                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageInformation(er.Message);
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }

                return ds;
            }
        }
    }
}
