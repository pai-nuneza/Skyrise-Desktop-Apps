using System;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.IO;


namespace CommonPostingLibrary
{
    public class CommonProcedures
    {
        private static readonly string CompanyName;

        static CommonProcedures()
        {
            
        }

        public static void ProcessExceptionMessage(Exception ex)
        {
            string[] errMessage = ex.Message.Split(' ', '\n', '\r', '\t', '\'', '\"');
            int wordctr;
            int error;
            wordctr = errMessage.Length - 1;
            while (wordctr > 0 &&
            (errMessage[wordctr] != "Snapshot" && errMessage[wordctr] != "snapshot") &&
            (errMessage[wordctr] != "Cancelled" && errMessage[wordctr] != "cancelled") &&
            (errMessage[wordctr] != "Invalid" && errMessage[wordctr] != "invalid" &&
            errMessage[wordctr] != "Incorrect" && errMessage[wordctr] != "incorrect"))
            {
                wordctr--;
            }
            if (!(errMessage[wordctr] != "Snapshot" && errMessage[wordctr] != "snapshot"))
                throw new PayrollException("Conflict During Saving\nPlease Repeat Transaction");
            else
                if (!(errMessage[wordctr] != "Cancelled" && errMessage[wordctr] != "cancelled"))
                    throw new PayrollException("Transaction has been Cancelled\nNothing was commited");
                else
                    if (!(errMessage[wordctr] != "Invalid" && errMessage[wordctr] != "invalid" &&
                    errMessage[wordctr] != "Incorrect" && errMessage[wordctr] != "incorrect"))
                        throw ex;
                    else
                        throw new PayrollException(ex.Message);
        }

        //public static string getQueryCompanySupplierName()
        //{
        //    return @"SELECT isnull(max(Smt_SupplierCode),'') as Smt_SupplierCode
        //             FROM [T_SupplierMaster]
        //             WHERE Smt_Status = 'A'
					   // AND Smt_SupplierShortName = 'FEP'";
        //}

        public static string checkForStockLedgerNegative(DataRow drow)
        {
            StringBuilder strBuil = new StringBuilder();
            for (int i = 0; i < drow.ItemArray.Length; ++i)
            {
                if (decimal.Parse(drow[i].ToString()) < 0)
                {
                    strBuil.Append(drow.Table.Columns[i].ColumnName);
                    strBuil.Append("\n");
                }
            }
            if (strBuil.ToString() != String.Empty)
            {
                return "The Operation Cannot Proceed because\n" +
                "It would causes negative values\n" +
                "To the following columns\n" +
                strBuil.ToString();
            }
            return String.Empty;
        }
        //public static string getQueryStockLedgerNegative(string stock, string costcenter, string type)
        //{
        //    string retval = @"  select Sil_OrderedQtySB
		      //                          ,Sil_IntransitQtySB
		      //                          ,Sil_ReceivedQtySB
		      //                          ,Sil_StockBegQty
		      //                          ,Sil_ReceivedQty
		      //                          ,Sil_IssuedQty
		      //                          ,Sil_TransInQuantity
		      //                          ,Sil_TransOutQuantity
		      //                          ,Sil_StockEndQty
		      //                          ,Sil_ReservedQty
		      //                          ,Sil_LocatorQty
		      //                          ,Sil_HoldQuantity
        //                        from dbo.T_StockLedger
        //                        where Sil_StockCode = '{0}'
	       //                         and Sil_CostCenterCode = '{1}'
	       //                         and Sil_StockTypeCode = '{2}'";
        //    retval = string.Format(retval, stock, costcenter, type);
        //    return retval;
        //}

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

        public static string createAffectedInformation(DataTable tablerecords, string header, int recordperline)
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
            if (MessageBox.Show(message, CompanyName + " " + "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                return true;
            else
                return false;
        }

        public static DialogResult showMessageQuestionYesNo(string message)
        {
            return MessageBox.Show(message, CompanyName + " " + "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        }
        public static string generateLastSeqNo(int lastSeqNo, int totalWidth)
        {
            return lastSeqNo.ToString().PadLeft(totalWidth, '0');
        }

        public static void showMessageInformation(string message)
        {
            MessageBox.Show(message, CompanyName + " " + "Info", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static void showMessageError(string message)
        {
            MessageBox.Show(message, CompanyName + " " + "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult showMessageWarning(string message)
        {
            return MessageBox.Show(message, CompanyName + " " + "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult showMessageQuestion(string message)
        {
            return MessageBox.Show(message, CompanyName + " " + "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        }

        public static string getAppFolderPath(string applicationPath, string folderName)
        {
            return System.IO.Path.GetFullPath(applicationPath + @"\..\.." + folderName);
        }

        public static DataRow getSelectedRow(DataTable dt, string condition)
        {
            DataRow[] rows = dt.Select(condition);

            if (rows.Length > 0)
                return rows[0];
            else
                return null;
        }

        public static string getShippingDetailValue(string s)
        {
            string value = "";

            if (s.Equals("S"))
            {
                value = "Ship Via : Sea Freight";
            }
            if (s.Equals("A"))
            {
                value = "Ship Via : Air Freight";
            }
            if (s.Equals("L"))
            {
                value = "Ship Via : Land Freight";
            }
            return value;
        }

        public static void setStatusValues(DataGridView dgView, string statParam)
        {
            for (int i = 0; i < dgView.RowCount; i++)
            {
                if (dgView.Rows[i].Cells[statParam].Value.ToString() == "A")
                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.ACTIVE;
                else
                    if (dgView.Rows[i].Cells[statParam].Value.ToString() == "N")
                        dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.NEW;
                    else
                        if (dgView.Rows[i].Cells[statParam].Value.ToString() == "U")
                            dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.ONHOLD;
                        else
                            if (dgView.Rows[i].Cells[statParam].Value.ToString() == "C")
                                dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.CANCELLED;
                            else
                                if (dgView.Rows[i].Cells[statParam].Value.ToString() == "F")
                                    dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.FULFILLED;
                                else
                                    if (dgView.Rows[i].Cells[statParam].Value.ToString() == "G")
                                        dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.GENERATED;
                                    else
                                        if (dgView.Rows[i].Cells[statParam].Value.ToString() == "T")
                                            dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.GENERATED;
                                        else
                                            if (dgView.Rows[i].Cells[statParam].Value.ToString() == "I")
                                                dgView.Rows[i].Cells[statParam].Value = CommonEnum.Status.INTRANSIT;
            }
        }
      
        //public static bool checkRemoteConnectivity()
        //{
        //    SqlConnection testdbConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SCPConnectionString"].ConnectionString);
        //    bool canConnect = true;
        //    int tryCounter = 0;

        //    while (canConnect != false && tryCounter < 1)
        //    {
        //        try
        //        {
        //            testdbConn.Open();
        //            SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM T_SystemCalendar", testdbConn);
        //            DataSet ds = new DataSet();
        //            ad.Fill(ds, "SystemControl");
        //            testdbConn.Close();
        //            canConnect = true;
        //            tryCounter++;
        //        }
        //        catch
        //        {
        //            canConnect = false;
        //        }
        //    }
        //    return canConnect;
        //}
        //end
        public static bool isAllowTransaction()
        {
            //            SqlConnection testdbConn = new SqlConnection(ConfigurationManager.ConnectionStrings["erpConnectionString"].ConnectionString);
            //            bool canTransact = false;


            //            try
            //            {
            //                testdbConn.Open();
            //                SqlDataAdapter ad = new SqlDataAdapter(@"SELECT 
            //                                                            Scm_InventoryOpening AS Scm_InventoryOpening,
            //                                                            Scm_InventoryClosing AS Scm_InventoryClosing,
            //                                                            Scm_openingbackup AS Scm_openingbackup,
            //                                                            Scm_cycleopening AS Scm_cycleopening,
            //                                                            Scm_closingbackup AS Scm_closingbackup,
            //                                                            Scm_cycleclosing AS Scm_cycleclosing
            //                                                        FROM T_SystemControl", testdbConn);
            //                DataSet ds = new DataSet();
            //                ad.Fill(ds, "SystemControl");
            //                if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Scm_InventoryOpening"]) == false &&
            //                    Convert.ToBoolean(ds.Tables[0].Rows[0]["Scm_InventoryClosing"]) == false &&
            //                    Convert.ToBoolean(ds.Tables[0].Rows[0]["Scm_openingbackup"]) == false &&
            //                    Convert.ToBoolean(ds.Tables[0].Rows[0]["Scm_cycleopening"]) == false &&
            //                    Convert.ToBoolean(ds.Tables[0].Rows[0]["Scm_closingbackup"]) == true &&
            //                    Convert.ToBoolean(ds.Tables[0].Rows[0]["Scm_cycleclosing"]) == false)
            //                    canTransact = true;
            //                else
            //                    canTransact = false;
            //            }
            //            catch
            //            {
            //                canTransact = false;
            //            }
            //            finally
            //            {
            //                testdbConn.Close();
            //            }
            //            return canTransact;
            return true;
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

        public static void logErrorToFile(string strLogText)
        {
            // Create a writer and open the file:
            StreamWriter log;
            string dir = @"C:\HRCLogFile";  // folder location
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
    }
}
