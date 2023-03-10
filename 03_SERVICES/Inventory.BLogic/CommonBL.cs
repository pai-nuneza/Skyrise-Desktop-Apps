using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Threading;
using Posting.DAL;
using CommonPostingLibrary;
using Posting.BLogic;

namespace Posting.BLogic
{
    public class CommonBL : BaseBL
    {
        public CommonBL()
        { 
        
        }

        #region Overrides
        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        public bool UpdateVersion(string SystemID, string AppVersion, string UserLogin, string CompanyCode)
        {
            bool retval = false;
            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@SystemID", SystemID.Trim(), SqlDbType.VarChar, 15);
            param[1] = new ParameterInfo("@AppVersion", AppVersion.Trim(), SqlDbType.VarChar, 20);
            param[2] = new ParameterInfo("@UserLogin", UserLogin.Trim(), SqlDbType.VarChar, 15);
            param[3] = new ParameterInfo("@CompanyCode", CompanyCode.Trim(), SqlDbType.VarChar, 10);

            #region query
            string query = @"If Exists (SELECT TOP 1 * FROM .dbo.T_SystemVersion WHERE Tsv_SystemCode = @SystemID AND Tsv_CompanyCode = @CompanyCode)
	                            BEGIN
		                            UPDATE .dbo.T_SystemVersion 
                                        SET Tsv_VersionNumber = @AppVersion 
				                            ,Usr_Login = @UserLogin
				                            ,Ludatetime = getdate()
                                    WHERE Tsv_SystemCode = @SystemID
                                        AND Tsv_CompanyCode = @CompanyCode
	                            END
                            ELSE
	                            BEGIN
		                            INSERT INTO .dbo.T_SystemVersion
		                            (
                                        Tsv_CompanyCode
			                            , Tsv_SystemCode
			                            , Tsv_SystemName
			                            , Tsv_VersionNumber
			                            , Tsv_RecordStatus
			                            , Usr_Login
			                            , Ludatetime
		                            )
		                            SELECT
                                        @CompanyCode
			                            ,@SystemID
			                            ,@SystemID
			                            ,@AppVersion
			                            ,'A'
			                            ,@UserLogin
			                            ,GETDATE()
	                            END";
            #endregion
            using (DALHelper dal = new DALHelper("", true))
            {
                dal.OpenDB();
                try
                {
                    dal.ExecuteNonQuery(query, CommandType.Text, param);
                    retval = true;
                }
                catch(Exception err) 
                {
                    dal.CloseDB();
                    retval = false;
                    throw( new Exception(err.Message));  
                }
                finally 
                {
                    dal.CloseDB();
                }
            }

            return retval;
        }

        public string GetParameterValueFromCentral(string PolicyCode, string CompanyCode)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_DataType
                                    , Mph_NumValue
                                    , Mph_CharValue
                                    , Mph_Formula 
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper("", true))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup in Central.", CompanyCode, PolicyCode)));
        }

        public string GetParameterValueFromPayroll(string PolicyCode, string CompanyCode, string PayrollDBName)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = string.Format(@"SELECT Mph_DataType
                                                , Mph_NumValue
                                                , Mph_CharValue
                                                , Mph_Formula 
                                           FROM {0}..M_PolicyHdr
                                           WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                            AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                            AND RTRIM(Mph_RecordStatus) = 'A'", PayrollDBName);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetDTRDatabaseName()
        {
            return (ConfigurationManager.AppSettings["DBNameDTR"].ToString());
        }

        #region Email Sending Functions

        public void SendMail(string ServiceCode, string emailBody)
        {
            string TOList = this.getToList(ServiceCode);
            string CCList = this.getCCList(ServiceCode);

            if (TOList.Equals(string.Empty))
            {
                using (DALHelper dal = new DALHelper(true))
                {
                    try
                    {
                        dal.OpenDB();
                        SendMail(TOList
                        , CCList
                        , ConfigurationManager.AppSettings["BCC"].ToString()
                        , ConfigurationManager.AppSettings["FROM"].ToString()
                        , this.GetCommonSubject(ServiceCode)
                        , emailBody
                        , null
                        , null
                        , ConfigurationManager.AppSettings["SMTPServer"].ToString()
                        , ConfigurationManager.AppSettings["SMTPLogin"].ToString()
                        , ConfigurationManager.AppSettings["SMTPPassword"].ToString()
                        , dal
                        , Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString())
                        , 0);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception("Error on Run Attendance Report.\n" + ex.Message);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
        }

        public void SendMailHTML(string ServiceCode, string emailBody)
        {
            string TOList = this.getToList(ServiceCode);
            string CCList = this.getCCList(ServiceCode);

            if (TOList.Equals(string.Empty))
            {
                using (DALHelper dal = new DALHelper(true))
                {
                    try
                    {
                        dal.OpenDB();
                        SendMailHTML(TOList
                        , CCList
                        , ConfigurationManager.AppSettings["BCC"].ToString()
                        , ConfigurationManager.AppSettings["FROM"].ToString()
                        , this.GetCommonSubject(ServiceCode)
                        , emailBody
                        , null
                        , null
                        , ConfigurationManager.AppSettings["SMTPServer"].ToString()
                        , ConfigurationManager.AppSettings["SMTPLogin"].ToString()
                        , ConfigurationManager.AppSettings["SMTPPassword"].ToString()
                        , dal
                        , Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString())
                        , 0);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception("Error on Run Attendance Report.\n" + ex.Message);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
        }

        public void SendMail(string TO, string CC, string Subject, string emailBody)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    SendMail(TO
                    , CC
                    , ConfigurationManager.AppSettings["BCC"].ToString()
                    , ConfigurationManager.AppSettings["FROM"].ToString()
                    , Subject
                    , emailBody
                    , null
                    , null
                    , ConfigurationManager.AppSettings["SMTPServer"].ToString()
                    , ConfigurationManager.AppSettings["SMTPLogin"].ToString()
                    , ConfigurationManager.AppSettings["SMTPPassword"].ToString()
                    , dal
                    , Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString())
                    , 0);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Error on Run Attendance Report.\n" + ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public void SendMailHTML(string TO, string CC, string Subject, string emailBody)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    SendMailHTML(TO
                    , CC
                    , ConfigurationManager.AppSettings["BCC"].ToString()
                    , ConfigurationManager.AppSettings["FROM"].ToString()
                    , Subject
                    , emailBody
                    , null
                    , null
                    , ConfigurationManager.AppSettings["SMTPServer"].ToString()
                    , ConfigurationManager.AppSettings["SMTPLogin"].ToString()
                    , ConfigurationManager.AppSettings["SMTPPassword"].ToString()
                    , dal
                    , Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString())
                    , 0);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Error on Run Attendance Report.\n" + ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        //This function will do the the sending to the email address
        public void SendMail(string mailTo
        , string mailCC
        , string mailBCC
        , string mailFrom
        , string mailSbjct
        , string mailBody
        , ArrayList attachments
        , ArrayList nonAttachementFiles
        , string mailServer
        , string uname
        , string pwd
        , DALHelper dal
        , int nPortNo
        , int nUseSSL)
        {
            try
            {
                // create mail message object
                MailMessage mail = new MailMessage();

                string strMailFrom = mailFrom;
                string strMailTo = mailTo;
                string strMailCC = mailCC;
                string strMailBCC = mailBCC;
                string strSMTPServer = mailServer;
                string strUserName = uname;
                string strPassword = pwd;
                string[] strMailToArray = strMailTo.Split(',');
                string[] strMailCCArray = strMailCC.Split(',');
                string[] strMailBCCArray = strMailBCC.Split(',');

                mail.From = new MailAddress(strMailFrom); // put the FROM address here
                for (int nMailToCnt = 0; nMailToCnt < strMailToArray.Length; nMailToCnt++)
                {
                    if (strMailToArray[nMailToCnt] != string.Empty)
                        mail.To.Add(strMailToArray[nMailToCnt]); // ADD the TO address here
                }

                for (int nMailCCCnt = 0; nMailCCCnt < strMailCCArray.Length; nMailCCCnt++)
                {
                    if (strMailCCArray[nMailCCCnt] != string.Empty)
                        mail.CC.Add(strMailCCArray[nMailCCCnt]); // ADD the CC address here
                }

                for (int nMailBCCCnt = 0; nMailBCCCnt < strMailBCCArray.Length; nMailBCCCnt++)
                {
                    if (strMailBCCArray[nMailBCCCnt] != string.Empty)
                        mail.Bcc.Add(strMailBCCArray[nMailBCCCnt]);// ADD the BCC address here
                }

                mail.Subject = mailSbjct; // put subject here	               
                mail.Body = mailBody; // put body of email here
                Attachment attachmentobj;
                if (attachments != null)
                    for (int attachment = 0; attachment < attachments.Count; attachment++)
                    {
                        attachmentobj = (Attachment)attachments[attachment];
                        mail.Attachments.Add(attachmentobj);
                    }

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(strSMTPServer, nPortNo);//587 for Gmail

                if (nUseSSL == 1)
                    smtp.EnableSsl = true;
                else
                    smtp.EnableSsl = false;

                //Use this if you need an delivery notification of an email. DeliveryNotificationOption is an enumeration
                //and can be used to set the delivery notification on the following options:
                //1. Delay
                //2. Never
                //3. None
                //4. OnFailure
                //5. OnSuccess
                //You can use also use OnFailure enum with OnSuccess enum. If in case the e-mail fails to delivered you'll get notification for
                //both the cases
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                //Add "Disposition-Notification-To" for Read receipt
                //mail.Headers.Add("Disposition-Notification-To", strMailFrom);

                object userState = mail;
                smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);

                if (strUserName.Trim() != string.Empty && strPassword.Trim() != string.Empty)
                    smtp.Credentials = new System.Net.NetworkCredential(strUserName, strPassword);
                smtp.SendAsync(mail, userState); // and then send the mail      
            }
            catch (Exception err)
            {
                //Your code here
            }
            finally
            {
                //Your code here
            }
            Thread.Sleep(30000);
        }

        public void SendMailHTML(string mailTo
        , string mailCC
        , string mailBCC
        , string mailFrom
        , string mailSbjct
        , string mailBody
        , ArrayList attachments
        , ArrayList nonAttachementFiles
        , string mailServer
        , string uname
        , string pwd
        , DALHelper dal
        , int nPortNo
        , int nUseSSL)
        {
            try
            {
                // create mail message object
                MailMessage mail = new MailMessage();

                string strMailFrom = mailFrom;
                string strMailTo = mailTo;
                string strMailCC = mailCC;
                string strMailBCC = mailBCC;
                string strSMTPServer = mailServer;
                string strUserName = uname;
                string strPassword = pwd;
                string[] strMailToArray = strMailTo.Split(',');
                string[] strMailCCArray = strMailCC.Split(',');
                string[] strMailBCCArray = strMailBCC.Split(',');

                mail.From = new MailAddress(strMailFrom); // put the FROM address here
                for (int nMailToCnt = 0; nMailToCnt < strMailToArray.Length; nMailToCnt++)
                {
                    if (strMailToArray[nMailToCnt] != string.Empty)
                        mail.To.Add(strMailToArray[nMailToCnt]); // ADD the TO address here
                }

                for (int nMailCCCnt = 0; nMailCCCnt < strMailCCArray.Length; nMailCCCnt++)
                {
                    if (strMailCCArray[nMailCCCnt] != string.Empty)
                        mail.CC.Add(strMailCCArray[nMailCCCnt]); // ADD the CC address here
                }

                for (int nMailBCCCnt = 0; nMailBCCCnt < strMailBCCArray.Length; nMailBCCCnt++)
                {
                    if (strMailBCCArray[nMailBCCCnt] != string.Empty)
                        mail.Bcc.Add(strMailBCCArray[nMailBCCCnt]);// ADD the BCC address here
                }

                mail.Subject = mailSbjct; // put subject here
                mail.IsBodyHtml = true; // set to HTML Format ang body
                mail.Body = mailBody; // put body of email here

                System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                // Add the alternate body to the message.
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(mailBody, Encoding.ASCII, mimeType.ToString());
                mail.AlternateViews.Add(alternate);

                Attachment attachmentobj;
                if (attachments != null)
                    for (int attachment = 0; attachment < attachments.Count; attachment++)
                    {
                        attachmentobj = (Attachment)attachments[attachment];
                        mail.Attachments.Add(attachmentobj);
                    }

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(strSMTPServer, nPortNo);//587 for Gmail

                if (nUseSSL == 1)
                    smtp.EnableSsl = true;
                else
                    smtp.EnableSsl = false;

                //Use this if you need an delivery notification of an email. DeliveryNotificationOption is an enumeration
                //and can be used to set the delivery notification on the following options:
                //1. Delay
                //2. Never
                //3. None
                //4. OnFailure
                //5. OnSuccess
                //You can use also use OnFailure enum with OnSuccess enum. If in case the e-mail fails to delivered you'll get notification for
                //both the cases
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                //Add "Disposition-Notification-To" for Read receipt
                //mail.Headers.Add("Disposition-Notification-To", strMailFrom);

                object userState = mail;
                smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);

                if (strUserName.Trim() != string.Empty && strPassword.Trim() != string.Empty)
                    smtp.Credentials = new System.Net.NetworkCredential(strUserName, strPassword);
                smtp.SendAsync(mail, userState); // and then send the mail      
            }
            catch (Exception err)
            {
                //Your code here
            }
            finally
            {
                //Your code here
            }
            Thread.Sleep(30000);
        }

        protected void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
            }
            else
                if (e.Error != null)
                {
                }
                else
                {
                    //Success
                }
        }

        public string getToList(string MenuCode)
        {
            DataSet ds = new DataSet();
            string ToList = string.Empty;

            #region qurey
            string qString = @" select emt_emailaddress from T_ServiceEmailRecipient
                                left join M_Employee on Mem_IDNo = sse_usercode
                                where Sse_MenuCode = '" + MenuCode + "' and Sse_RecipientType = 'T' and Sse_Status = 'A'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ToList = ToList + ds.Tables[0].Rows[i][0].ToString().Trim() + ",";
                }
                ToList = ToList.Substring(0, ToList.Length - 1);
            }

            return ToList;
        }

        public string getCCList(string MenuCode)
        {
            DataSet ds = new DataSet();
            string ToList = string.Empty;

            #region qurey
            string qString = @" select emt_emailaddress from T_ServiceEmailRecipient
                                left join M_Employee on Mem_IDNo = sse_usercode
                                where Sse_MenuCode = '" + MenuCode + "' and Sse_RecipientType = 'C' and Sse_Status = 'A'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ToList = ToList + ds.Tables[0].Rows[i][0].ToString().Trim() + ",";
                }
                ToList = ToList.Substring(0, ToList.Length - 1);
            }

            return ToList;
        }

        private string GetCommonSubject(string ProcessName)
        {
            DataSet ds = new DataSet();
            string Subject = string.Empty;

            #region qurey
            string qString = string.Format(@" select Msm_ServiceName
                                from M_Service
                                where Msm_ServiceCode = '{0}'", ProcessName);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                Subject = "Automated Email Sent for Service " + ds.Tables[0].Rows[0][0].ToString() + ".";
            }

            return Subject;
        }

        public string CreateCommonBody(string ProcessName, bool isSuccesful)
        {
            DataSet ds = new DataSet();
            string emailBody = string.Empty;

            #region qurey
            string qString = string.Format(@" select Msm_ServiceName
                                from M_Service
                                where Msm_ServiceCode = '{0}'", ProcessName);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (isSuccesful)
                {
                    emailBody = @"
The service for " + ds.Tables[0].Rows[0][0].ToString() + " executed successfully.";
                }
                else
                {
                    emailBody = @"
The service for " + ds.Tables[0].Rows[0][0].ToString() + " failed to execute.";
                }
            }

            return emailBody;
        }

        #endregion

    }
}
