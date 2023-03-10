using System;
using System.Text;
using System.Data;
using Payroll.DAL;
using System.IO;
using CommonLibrary;
using System.Configuration;

namespace Payroll.BLogic
{
   
    public abstract class BaseServiceBL
    {
        public void SendEmail(string Subject, string MailBody, string MenuCode, string CentralProfile, string CompanyCode)
        {
            SendEmail(Subject, MailBody, MenuCode, string.Empty, string.Empty, CentralProfile, CompanyCode);
        }

        public bool SendEmail(string Subject, string MailBody, string MenuCode, string From, string To, string CentralProfile, string CompanyCode)
        {
            GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], "Processing " + MenuCode + " Email Notification.");
            
            EmailSending em;
            DataSet dsConfig;

            try
            {
                using (DALHelper dalHelper = new DALHelper(CentralProfile, false))
                {
                    string sqlFetch = string.Format(@"
                                        SELECT [Msc_LineNo]
                                             ,[Msc_SMTPServer]
		                                     ,[Msc_Login]
		                                     ,[Msc_Password]
		                                     ,[Msc_Port]
		                                     ,[Msc_SenderEmail]
                                             ,[Msc_EnableSSL]
	                                     FROM M_SMTP
	                                    WHERE Msc_RecordStatus = 'A'
                                              AND Msc_CompanyCode = '{0}'  
                                        ORDER BY Msc_IsDefaultSMTP DESC", CompanyCode);

                    dsConfig = dalHelper.ExecuteDataSet(sqlFetch);
                }

                for (int i = 0; i < dsConfig.Tables[0].Rows.Count; i++)
                {
                    //dsConfig.Tables[0].Rows[i]["Msc_Password"] = Encrypt.decryptText(dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString(), dsConfig.Tables[0].Rows[i]["Msc_Login"].ToString() + "_" + dsConfig.Tables[0].Rows[i]["Msc_LineNo"].ToString());
                    dsConfig.Tables[0].Rows[i]["Msc_Password"] = dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString();
                }

                if (dsConfig.Tables[0].Rows.Count > 0)
                    em = new EmailSending(dsConfig.Tables[0].Rows[0]);
                else
                    em = new EmailSending();


                if (!To.Equals(string.Empty))
                {
                    em.To = To;
                }
                else
                {
                    em.To = ReturnEmailAdd(MenuCode, "T");
                    em.Cc = ReturnEmailAdd("DEFAULT", "C");

                    if (em.To.Equals(string.Empty))
                        em.To = ReturnEmailAdd("DEFAULT", "T");

                    //if (em.Cc.Equals(string.Empty))
                    //    em.Cc = ReturnEmailAdd("DEFAULT", "C");
                }
              
                if (!From.Equals(string.Empty))
                    em.From = From;
                
                em.Subject = Subject;
                em.MailBody = MailBody;
              
                em.Send();
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], MenuCode + " Email Notification Succesful.");
                return true;
            }
            catch (Exception Ex)
            {
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], MenuCode + " Email Notification Failed!\nError: " + Ex.Message);
                return false;
            }
        }

        public bool SendEmailWithAttachment(string Subject, string MailBody, string MenuCode, string To, string From, string attachmentPath, string CentralProfile, string CompanyCode)
        {
            EmailSending em;
            DataSet dsConfig;

            try
            {
                using (DALHelper dalHelper = new DALHelper(CentralProfile, false))
                {
                    string sqlFetch = string.Format(@"SELECT [Msc_LineNo]
                                                            ,[Msc_SMTPServer]
                                                            ,[Msc_Login]
                                                            ,[Msc_Password]
                                                            ,[Msc_Port]
                                                            ,[Msc_SenderEmail]
                                                            ,[Msc_EnableSSL]
                                                    FROM M_SMTP
                                                    WHERE Msc_RecordStatus = 'A'
                                                        AND Msc_CompanyCode = '{0}'
                                                    ORDER BY Msc_IsDefaultSMTP DESC", CompanyCode);

                    dsConfig = dalHelper.ExecuteDataSet(sqlFetch);
                }

                for (int i = 0; i < dsConfig.Tables[0].Rows.Count; i++)
                {
                    //dsConfig.Tables[0].Rows[i]["Msc_Password"] = Encrypt.decryptText(dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString(), dsConfig.Tables[0].Rows[i]["Msc_Login"].ToString() + "_" + dsConfig.Tables[0].Rows[i]["Msc_LineNo"].ToString());
                    dsConfig.Tables[0].Rows[i]["Msc_Password"] = dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString();
                }

                if (dsConfig.Tables[0].Rows.Count > 0)
                    em = new EmailSending(dsConfig.Tables[0].Rows[0]);
                else
                    em = new EmailSending();  

                if (attachmentPath == string.Empty) /* if no attachment, get CC recepient*/
                    em.To = ReturnEmailAdd(MenuCode, "C");
                else
                    em.To = To;
                if (em.To.Equals(string.Empty))
                    em.To = ReturnEmailAdd("DEFAULT", "T");

                if (dsConfig.Tables[0].Rows.Count > 0 && Convert.ToBoolean(dsConfig.Tables[0].Rows[0]["Msc_EnableSSL"]) == true)
                    From = dsConfig.Tables[0].Rows[0]["Msc_Login"].ToString(); //For Office365 Security Feature (Mail FROM must be equal to the SMTP Login)
                if (!From.Equals(string.Empty))
                    em.From = From;

                em.Subject = Subject;
                em.MailBody = MailBody;

                em.SendWithAttachment(attachmentPath);
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], MenuCode + " Email Notification Succesful.");
                return true;
            }
            catch (Exception Ex)
            {
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], MenuCode + " Email Notification Failed!\nError: " + Ex.Message);
                return false;
            }
        }

        public bool SendEmailWithBinaryData(string Subject, string MailBody, string MenuCode, string To, string From, byte[] data, string EmployeeID, bool IsAlternate, string EmailPassword, string CentralProfile, string CompanyCode, string PayCycle)
        {
            EmailSending em;
            DataSet dsConfig;

            try
            {
                using (DALHelper dalHelper = new DALHelper(CentralProfile, false))
                {
                    string sqlFetch = @"SELECT [Msc_LineNo]
                                                ,[Msc_SMTPServer]
                                                ,[Msc_Login]
                                                ,[Msc_Password]
                                                ,[Msc_Port]
                                                ,[Msc_SenderEmail]
                                                ,[Msc_EnableSSL]
                                        FROM M_SMTP
                                        WHERE Msc_RecordStatus = 'A'
                                            AND Msc_CompanyCode = '{0}'
                                        ORDER BY Msc_IsDefaultSMTP DESC";

                    dsConfig = dalHelper.ExecuteDataSet(sqlFetch);
                }

                for (int i = 0; i < dsConfig.Tables[0].Rows.Count; i++)
                {
                    //dsConfig.Tables[0].Rows[i]["Msc_Password"] = Encrypt.decryptText(dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString(), dsConfig.Tables[0].Rows[i]["Msc_Login"].ToString() + "_" + dsConfig.Tables[0].Rows[i]["Msc_LineNo"].ToString());
                    dsConfig.Tables[0].Rows[i]["Msc_Password"] = dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString();
                }

                if (dsConfig.Tables[0].Rows.Count > 0)
                    em = new EmailSending(dsConfig.Tables[0].Rows[0]);
                else
                    em = new EmailSending();

                em.To = To;
                if (em.To.Equals(string.Empty))
                    em.To = ReturnEmailAdd("DEFAULT", "T");

                if (dsConfig.Tables[0].Rows.Count > 0 && Convert.ToBoolean(dsConfig.Tables[0].Rows[0]["Msc_EnableSSL"]) == true)
                    From = dsConfig.Tables[0].Rows[0]["Msc_Login"].ToString(); //For Office365 Security Feature (Mail FROM must be equal to the SMTP Login)
                if (!From.Equals(string.Empty))
                    em.From = From;

                em.Subject = Subject;
                em.MailBody = MailBody;

                if (IsAlternate == true)
                {
                    em.SendWithBinaryDataArchived(data, EmployeeID, EmailPassword, PayCycle, CompanyCode, CentralProfile);
                }
                else
                    em.SendWithBinaryData(data, PayCycle);

                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"] + "\\MinervaLogFile", MenuCode + " Email Notification Succesfully sent to ." + EmployeeID + ": " + To);
                return true;
            }
            catch (Exception Ex)
            {
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"] + "\\MinervaLogFile", MenuCode + " Email Notification Failed!\nError: " + EmployeeID + ": " + Ex.Message);
                return false;
            }
        }

        public string SendMailTrailWithBinaryDataString(string Subject, string MailBody, string MenuCode, string To, string From, byte[] data, string EmployeeID, bool IsAlternate, string EmployeePassword, string CentralProfile, string CompanyCode, string PayCycle)
        {
            EmailSending em;
            DataSet dsConfig;

            try
            {
                using (DALHelper dalHelper = new DALHelper(CentralProfile, false))
                {
                    string sqlFetch = string.Format(@"SELECT [Msc_LineNo]
                                                ,[Msc_SMTPServer]
                                                ,[Msc_Login]
                                                ,[Msc_Password]
                                                ,[Msc_Port]
                                                ,[Msc_SenderEmail]
                                                ,[Msc_EnableSSL]
                                        FROM M_SMTP
                                        WHERE Msc_RecordStatus = 'A'
                                            AND Msc_CompanyCode = '{0}'
                                        ORDER BY Msc_IsDefaultSMTP DESC", CompanyCode);

                    dsConfig = dalHelper.ExecuteDataSet(sqlFetch);
                }

                

                for (int i = 0; i < dsConfig.Tables[0].Rows.Count; i++)
                {
                    //string PW = Encrypt.encryptText(dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString(), dsConfig.Tables[0].Rows[i]["Msc_Login"].ToString() + "_" + dsConfig.Tables[0].Rows[i]["Msc_LineNo"].ToString());
                    //dsConfig.Tables[0].Rows[i]["Msc_Password"] = Encrypt.decryptText(dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString(), dsConfig.Tables[0].Rows[i]["Msc_Login"].ToString() + "_" + dsConfig.Tables[0].Rows[i]["Msc_LineNo"].ToString());
                    dsConfig.Tables[0].Rows[i]["Msc_Password"] = dsConfig.Tables[0].Rows[i]["Msc_Password"].ToString();
                }

                if (dsConfig.Tables[0].Rows.Count > 0)
                    em = new EmailSending(dsConfig.Tables[0].Rows[0]);
                else
                    em = new EmailSending();

                em.To = To;
                if (em.To.Equals(string.Empty))
                    em.To = ReturnEmailAdd("DEFAULT", "T");

                if (dsConfig.Tables[0].Rows.Count > 0 && Convert.ToBoolean(dsConfig.Tables[0].Rows[0]["Msc_EnableSSL"]) == true)
                    From = dsConfig.Tables[0].Rows[0]["Msc_Login"].ToString(); //For Office365 Security Feature (Mail FROM must be equal to the SMTP Login)
                if (!From.Equals(string.Empty))
                    em.From = From;

                em.Subject = Subject;
                em.MailBody = MailBody;

                if (IsAlternate == true)
                {
                    em.SendWithBinaryDataArchived(data, EmployeeID, EmployeePassword, PayCycle, CompanyCode, CentralProfile);
                }
                else
                    em.SendWithBinaryData(data, PayCycle);

                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"] + "\\MinervaLogFile", MenuCode + " Email Notification Succesfully sent to ." + EmployeeID + ": " + To);
                return string.Empty;
            }
            catch (Exception Ex)
            {
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"] + "\\MinervaLogFile", MenuCode + " Email Notification Failed!\nError: " + EmployeeID + ": " + Ex.Message);
                return Ex.Message.ToString();
            }
        }

        public static string ReturnEmailAdd(string MenuCode, string RecipientType)
        {
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();
            CommonBL commonBL = new CommonBL();

            ParameterInfo[] paramCol = new ParameterInfo[2];
            paramCol[0] = new ParameterInfo("@MenuCode", MenuCode, SqlDbType.NChar);
            paramCol[1] = new ParameterInfo("@RecipientType", RecipientType, SqlDbType.NChar);
            paramCol[1] = new ParameterInfo("@ProfileCode", LoginInfo.getUser().DBNumber, SqlDbType.NVarChar);


            string sqlStatement = @"SELECT	Mur_OfficeEmailAddress
                                    FROM	M_User
                                    JOIN	M_ServiceRecipient
                                    ON		Mur_UserCode = Msr_UserCode
                                    WHERE	Msr_ModuleCode = @MenuCode
                                    AND		Msr_RecipientType = @RecipientType
                                    AND     Msr_ProfileCode = @ProfileCode
                                    AND		Mur_OfficeEmailAddress IS NOT NULL
                                    AND		RTRIM(Mur_OfficeEmailAddress) <> ''";

            using (DALHelper dal = new DALHelper("PROFILE", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    ds = dal.ExecuteDataSet(sqlStatement, CommandType.Text, paramCol);
                    dal.CommitTransaction();

                }
                catch (Exception ex)
                {
                    dal.RollBackTransaction();
                }
                finally
                {
                    dal.CloseDB();
                }


                if (ds != null)
                {
                    int count = 1;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        sb.Append(dr[0].ToString());
                        if (ds.Tables[0].Rows.Count != count)
                        {
                            sb.Append(",");
                        }
                        count++;
                    }
                }

                return sb.ToString();
            }


        }

        public string GenerateTextFile(string strFileName, string strPath, string strText)
        {
            string strFullFileName;
            System.IO.FileStream fs;
            System.Threading.Mutex file = new System.Threading.Mutex(false);

            if (!strPath.EndsWith(@"\"))
                strPath += @"\";

            strFullFileName = String.Format("{0}{1}", strPath, strFileName);

            if (!System.IO.Directory.Exists(strPath))
            {
                System.IO.Directory.CreateDirectory(strPath);
            }

            try
            {
                file.WaitOne();

                if (File.Exists(strFullFileName))
                    fs = new FileStream(strFullFileName, FileMode.Append, FileAccess.Write);
                else
                    fs = new FileStream(strFullFileName, FileMode.OpenOrCreate, FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs);

                sw.Write(DateTime.Now.ToString("s"));
                sw.Write(sw.NewLine);
                sw.Write(strText);
                sw.Write(sw.NewLine);
                sw.Write(sw.NewLine);


                sw.Close();
                fs.Close();
            }
            catch
            {
            }
            finally
            {
                file.Close();
            }

            return strFileName;
        }

        public string GetUserEmail(string UserCode)
        {
            string value;
            using (DALHelper dal = new DALHelper("PROFILE", true))
            {
                string sqlQuery = @"SELECT Mur_OfficeEmailAddress
                                    FROM	M_User
                                    WHERE	Mur_UserCode = '{0}'";
                try
                {
                    dal.OpenDB();
                    DataRow drUser = dal.ExecuteDataSet(string.Format(sqlQuery, UserCode)).Tables[0].Rows[0];
                    dal.CloseDB();

                    if (!drUser[0].ToString().Trim().Equals(string.Empty))
                        value = drUser[0].ToString().Trim();
                    else
                        value = string.Empty;
                }
                catch
                {
                    value = string.Empty;
                }

                return value;
            }
        }
        
        public string GenericMailBody(string Detail, DateTime Start, DateTime End, string Specific, string UserCode)
        {
            string UserName = "CustomerSupport@IMergeOnline.Com";
            string UserEmail = "CustomerSupport@IMergeOnline.Com";

            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, true))
            {
                string sqlQuery = @"SELECT	Mur_OfficeEmailAddress
		                                    ,Mur_UserFirstName 
		                                    + ' ' 
		                                    + Mur_UserFirstName
                                    FROM	M_User
                                    WHERE	Mur_UserCode = '{0}'";

                try
                {
                    dal.OpenDB();
                    DataRow drUser = dal.ExecuteDataSet(string.Format(sqlQuery, UserCode)).Tables[0].Rows[0];
                    dal.CloseDB();

                    if (!drUser[1].ToString().Trim().Equals(string.Empty))
                        UserName = drUser[1].ToString().Trim();

                    if (!drUser[0].ToString().Trim().Equals(string.Empty))
                        UserEmail = drUser[0].ToString().Trim();
                }
                catch
                {
                }
            }

            return string.Format("Good day!\n\nThis is a system generated message to inform you that {0}\n\nProcessing Statistics:\nStart Date and Time: {1}\nFinish Date and Time: {2}\n{3}\n\n\nRespectfully Yours,\n\n{4}",Detail,Start.ToUniversalTime(),End.ToUniversalTime(),Specific, UserName);

        }

    }
}
