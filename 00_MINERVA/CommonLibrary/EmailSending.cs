using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using Payroll.DAL;
using System.Data;


namespace CommonLibrary
{
    public class EmailSending
    {
        private string SMTPServer;
        private int SMTPPort;
        private string SMTPFrom;
        private string SMTPTo;
        private string SMTPSubject;
        private string SMTPBody;
        private string SMTPCc;
        private string SMTPBcc;
        private string SMTPLogin;
        private string SMTPPassword;
        private bool SMTPEnableSSL;

        public EmailSending()
        {
            SMTPServer = @"";
            SMTPLogin = @"";
            SMTPPassword = @"";
            SMTPPort = 25;
            SMTPSubject = string.Empty;
            SMTPBody = string.Empty;
            SMTPFrom = "";
            SMTPTo = string.Empty;
            SMTPCc = string.Empty;
            SMTPBcc = "";
            SMTPEnableSSL = false;
        }

        public EmailSending(System.Data.DataRow Config)
        {
            SMTPServer = Convert.ToString(Config["Msc_SMTPServer"]);
            SMTPLogin = Convert.ToString(Config["Msc_Login"]);
            SMTPPassword = Convert.ToString(Config["Msc_Password"]);
            SMTPPort = Convert.ToInt32(Config["Msc_Port"]);
            SMTPSubject = string.Empty;
            SMTPBody = string.Empty;
            SMTPFrom = Convert.ToString(Config["Msc_SenderEmail"]);
            SMTPTo = string.Empty;
            SMTPCc = string.Empty;
            SMTPBcc = string.Empty;
            SMTPEnableSSL = Convert.ToBoolean(Config["Msc_EnableSSL"]);
        }

        public void Send()
        {
            try
            {
                MailMessage emailCust = new MailMessage(this.SMTPFrom, this.SMTPTo);
                emailCust.Subject = this.SMTPSubject;
                emailCust.Body = this.SMTPBody;
                if (!this.SMTPCc.Equals(string.Empty))
                    emailCust.CC.Add(this.SMTPCc);

                if (!this.SMTPBcc.Equals(string.Empty))
                    emailCust.Bcc.Add(this.SMTPBcc);

                SmtpClient mailClient = new SmtpClient(this.SMTPServer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailClient.Port = this.SMTPPort;
                mailClient.EnableSsl = this.SMTPEnableSSL;
                if (SMTPLogin.Trim() != string.Empty && SMTPPassword.Trim() != string.Empty)
                {
                    mailClient.Credentials = new NetworkCredential(this.SMTPLogin, this.SMTPPassword);
                }
                mailClient.Send(emailCust);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /*===========================================================================
         * Function    : void SendWithAttachment(string attachmentpath) 
         * Purpose     : This function sends email with attachment file
         * Parameters  : Input : string attachmentpath = path of the attachment file
         *               Output: None                                       
         * Return      : void                                                                                
         * Date Created: October 29, 2010                                   
        /*===========================================================================*/
        public void SendWithAttachment(string attachmentPath)
        {
            //byte[] bytes = null;
            //System.IO.File.WriteAllBytes(attachmentPath, bytes); 
            try
            {
                MailMessage emailCust = new MailMessage(this.SMTPFrom, this.SMTPTo);
                emailCust.Subject = this.SMTPSubject;
                emailCust.Body = this.SMTPBody;
                Attachment attachment = new Attachment(attachmentPath);
                emailCust.Attachments.Add(attachment);
                if (!this.SMTPCc.Equals(string.Empty))
                    emailCust.CC.Add(this.SMTPCc);

                //if (!this.SMTPBcc.Equals(string.Empty))
                //    emailCust.Bcc.Add(this.SMTPBcc);

                SmtpClient mailClient = new SmtpClient(this.SMTPServer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailClient.Port = this.SMTPPort;
                mailClient.EnableSsl = this.SMTPEnableSSL;
                if (SMTPLogin.Trim() != string.Empty && SMTPPassword.Trim() != string.Empty)
                {
                    mailClient.Credentials = new NetworkCredential(this.SMTPLogin, this.SMTPPassword);
                }
                mailClient.Send(emailCust);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendWithBinaryDataArchived(byte[] data, string employeeID, string EmployeePassword, string PayCycle, string CompanyCode, string CentralProfile)
        {
            try
            {
                MailMessage emailCust = new MailMessage(this.SMTPFrom, this.SMTPTo);
                emailCust.Subject = this.SMTPSubject;
                emailCust.Body = this.SMTPBody;

                MemoryStream ms = new MemoryStream();

                using (ZipFile zip = new ZipFile())
                {
                    if (EmployeePassword == "")
                    {
                        string qryDefPass = string.Format(@"
                                             SELECT RTRIM(Mph_CharValue)
                                             FROM M_PolicyHdr
                                             WHERE Mph_PolicyCode='PASSWRDDEF'
                                                AND Mph_CompanyCode = '{0}'", CompanyCode);
                        DataTable DefPass;

                        using (DALHelper dal = new DALHelper(CentralProfile, false))
                        {
                            dal.OpenDB();
                            DefPass = dal.ExecuteDataSet(qryDefPass).Tables[0];
                            dal.CloseDB();
                        }
                        zip.Password = DefPass.Rows[0][0].ToString();
                    }
                    else
                    {
                        zip.Password = EmployeePassword;
                    }

                    zip.AddEntry(string.Format("PAYSLIP_{0}.PDF", PayCycle), data);
                    //zip.Save(employeeID + "PAYSLIP.zip");
                    zip.Save(ms);

                    ms.Seek(0, SeekOrigin.Begin);

                    //Attachment attachment = new Attachment(employeeID + "PAYSLIP.zip", "");
                    Attachment attachment = new Attachment(ms, string.Format("PAYSLIP_{0}.zip", PayCycle));
                    emailCust.Attachments.Add(attachment);
                    if (!this.SMTPCc.Equals(string.Empty))
                        emailCust.CC.Add(this.SMTPCc);

                    SmtpClient mailClient = new SmtpClient(this.SMTPServer);
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailClient.Port = this.SMTPPort;
                    mailClient.EnableSsl = this.SMTPEnableSSL;
                    //mailClient.UseDefaultCredentials = false;
                    if (SMTPLogin.Trim() != string.Empty && SMTPPassword.Trim() != string.Empty)
                    {
                        mailClient.Credentials = new NetworkCredential(this.SMTPLogin, this.SMTPPassword);
                    }
                    mailClient.Send(emailCust);
                }

                //File.Delete(employeeID + "PAYSLIP.zip");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendWithBinaryData(byte[] data, string PayCycle)
        {
            try
            {
                MailMessage emailCust = new MailMessage(this.SMTPFrom, this.SMTPTo);
                emailCust.Subject = this.SMTPSubject;
                emailCust.Body = this.SMTPBody;

                MemoryStream ms = new MemoryStream(data);

                Attachment attachment = new Attachment(ms, string.Format("PAYSLIP_{0}.PDF", PayCycle));
                emailCust.Attachments.Add(attachment);
                if (!this.SMTPCc.Equals(string.Empty))
                    emailCust.CC.Add(this.SMTPCc);

                SmtpClient mailClient = new SmtpClient(this.SMTPServer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailClient.Port = this.SMTPPort;
                mailClient.EnableSsl = this.SMTPEnableSSL;
                //mailClient.UseDefaultCredentials = false;
                if (SMTPLogin.Trim() != string.Empty && SMTPPassword.Trim() != string.Empty)
                {
                    mailClient.Credentials = new NetworkCredential(this.SMTPLogin, this.SMTPPassword);
                }
                mailClient.Send(emailCust);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SMTP
        {
            get {
                return this.SMTPServer;
            }
            set {
                this.SMTPServer = value;
            }
           
        }

        public int Port
        {
            get
            {
                return this.SMTPPort;
            }
            set
            {
                this.SMTPPort = value;
            }

        }

        public string SMTPUser
        {
            get
            {
                return this.SMTPLogin;
            }
            set
            {
                this.SMTPLogin = value;
            }
        }

        public string SMTPPass
        {
            get
            {
                return this.SMTPPassword;
            }
            set
            {
                this.SMTPPassword = value;
            }
        }

        public string To
        {
            get {
                return this.SMTPTo;
            }
            set {
                this.SMTPTo = value;
            }
        }

        public string Cc
        {
            get
            {
                return this.SMTPCc;
            }
            set
            {
                this.SMTPCc = value;
            }
        }

        public string From
        {
            get
            {
                return this.SMTPFrom;
            }
            set
            {
                this.SMTPFrom = value;
            }
        }

        public string Subject
        {
            get
            {
                return this.SMTPSubject;
            }
            set
            {
                this.SMTPSubject = value;
            }
        }

        public string MailBody
        {
            get
            {
                return this.SMTPBody;
            }
            set
            {
                this.SMTPBody = value;
            }
        }

        public bool EnableSSL
        {
            get
            {
                return this.SMTPEnableSSL;
            }
            set
            {
                this.SMTPEnableSSL = value;
            }
        }

    }
}
