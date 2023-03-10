using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using System.IO;
using CommonLibrary;
using System.Configuration;

namespace Payroll.BLogic
{
    ///      Author : Jule Eric Bernasor
    ///        Date : October 10, 2008
    /// Description : Inheritable Email Sending BLogic
    
    public abstract class BaseServiceBL
    {
        public void SendEmail(string Subject, string MailBody, string MenuCode)
        {
            SendEmail(Subject, MailBody, MenuCode, string.Empty);
        }

        public void SendEmail(string Subject, string MailBody, string MenuCode, string From)
        {
            GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], "Processing " + MenuCode + " Email Notification.");
            
            EmailSending em;
            DataSet dsConfig;

            try
            {
                using (DALHelper dalHelper = new DALHelper())
                {
                    string sqlFetch = @"SELECT [Sct_SMTPServer]
		                                     ,[Sct_Login]
		                                     ,[Sct_Password]
		                                     ,[Sct_Port]
		                                     ,[Sct_DefaultSender]
	                                     FROM [T_SMTPConfiguration]
	                                    WHERE [Sct_Status] = 'A'
                                     ORDER BY [Sct_DefaultSMTP] DESC";

                    dsConfig = dalHelper.ExecuteDataSet(sqlFetch);
                }

                if (dsConfig.Tables[0].Rows.Count > 0)
                    em = new EmailSending(dsConfig.Tables[0].Rows[0]);
                else
                    em = new EmailSending();


                em.To = ReturnEmailAdd(MenuCode, "T");
                em.Cc = ReturnEmailAdd("DEFAULT", "C");

                if (em.To.Equals(string.Empty))
                    em.To = ReturnEmailAdd("DEFAULT", "T");

                if (em.Cc.Equals(string.Empty))
                    em.Cc = @"jsbernasor@n-pax.com";//ReturnEmailAdd("DEFAULT", "C");
              
                if (!From.Equals(string.Empty))
                    em.From = From;
                
                em.Subject = Subject;
                em.MailBody = MailBody;
              
                em.Send();
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], MenuCode + " Email Notification Succesful.");
            }
            catch (Exception Ex)
            {
                GenerateTextFile("MailLog" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], MenuCode + " Email Notification Failed!\nError: " + Ex.Message);
            }
        }

/* Add Start Email Sending with Attachment Toby/N-PAX 20101029 */
        public bool SendEmailWithAttachment(string Subject, string MailBody, string MenuCode, string To, string From, string attachmentPath)
        {
            EmailSending em;
            DataSet dsConfig;

            try
            {
                using (DALHelper dalHelper = new DALHelper())
                {
                    string sqlFetch = @"SELECT [Sct_SMTPServer]
		                                     ,[Sct_Login]
		                                     ,[Sct_Password]
		                                     ,[Sct_Port]
		                                     ,[Sct_DefaultSender]
	                                     FROM [T_SMTPConfiguration]
	                                    WHERE [Sct_Status] = 'A'
                                     ORDER BY [Sct_DefaultSMTP] DESC";

                    dsConfig = dalHelper.ExecuteDataSet(sqlFetch);
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

        public static string ReturnEmailAdd(string MenuCode, string RecipientType)
        {
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();

            ParameterInfo[] paramCol = new ParameterInfo[2];
            paramCol[0] = new ParameterInfo("@MenuCode", MenuCode, SqlDbType.NChar);
            paramCol[1] = new ParameterInfo("@RecipientType", RecipientType, SqlDbType.NChar);


            string sqlStatement = @"SELECT	Mur_OfficeEmailAddress
                                    FROM	M_User
                                    JOIN	T_ServiceEmailRecipient
                                    ON		Mur_UserCode = Sse_UserCode
                                    WHERE	Sse_MenuCode = @MenuCode
                                    AND		Sse_RecipientType = @RecipientType
                                    AND		Mur_OfficeEmailAddress IS NOT NULL
                                    AND		RTRIM(Mur_OfficeEmailAddress) <> ''";

            using (DALHelper dal = new DALHelper())
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

        public string GetUserName(string UserCode)
        {
            string value;
            using (DALHelper dal = new DALHelper())
            {
                string sqlQuery = @"SELECT	Umt_userfname + ' ' + Umt_userlname
                                    FROM	M_User
                                    WHERE	Mur_UserCode = '{0}'";
                try
                {
                    dal.OpenDB();
                    DataRow drUser = dal.ExecuteDataSet(string.Format(sqlQuery, UserCode)).Tables[0].Rows[0];
                    dal.CloseDB();
                    value = drUser[0].ToString().Trim();
                }
                catch
                {
                    value = string.Empty;
                }

                return value;
            }
        }

        public string GetUserEmail(string UserCode)
        {
            string value;
            using (DALHelper dal = new DALHelper())
            {
                string sqlQuery = @"SELECT	Mur_OfficeEmailAddress
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

            using (DALHelper dal = new DALHelper())
            {
                string sqlQuery = @"SELECT	Mur_OfficeEmailAddress
		                                    ,Umt_userfname 
		                                    + ' ' 
		                                    + Umt_userlname
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
