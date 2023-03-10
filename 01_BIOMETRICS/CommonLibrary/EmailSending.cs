using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;


namespace CommonLibrary
{
    ///      Author : Jule Eric Bernasor
    ///        Date : October 10, 2008
    /// Description : Email Sending Class

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

        public EmailSending()
        {

            SMTPServer = @"203.115.191.71";
            SMTPLogin = @"jsbernasor@n-pax.com";
            SMTPPassword = @"PEN5406835";
            SMTPPort = 25;
            SMTPSubject = string.Empty;
            SMTPBody = string.Empty;
            SMTPFrom = "CustomerSupport@IMergeOnline.Com";
            SMTPTo = string.Empty;
            SMTPCc = string.Empty;
            SMTPBcc = "Jule@IMergeOnline.Com";

        }

        public EmailSending(System.Data.DataRow Config)
        {
            SMTPServer = Convert.ToString(Config["Sct_SMTPServer"]);
            SMTPLogin = Convert.ToString(Config["Sct_Login"]);
            SMTPPassword = Convert.ToString(Config["Sct_Password"]);
            SMTPPort = Convert.ToInt32(Config["Sct_Port"]);
            SMTPSubject = string.Empty;
            SMTPBody = string.Empty;
            SMTPFrom = Convert.ToString(Config["Sct_DefaultSender"]);
            SMTPTo = string.Empty;
            SMTPCc = string.Empty;
            SMTPBcc = "Jule@IMergeOnline.Com";

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
                mailClient.Credentials = new NetworkCredential(this.SMTPLogin, this.SMTPPassword);
                mailClient.Send(emailCust);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
/* Add Start Email Sending with Attachment Toby/N-PAX 20101029 */
        /*===========================================================================
         * Function    : void SendWithAttachment(string attachmentpath) 
         * Purpose     : This function sends email with attachment file
         * Parameters  : Input : string attachmentpath = path of the attachment file
         *               Output: None                                       
         * Return      : void                                           
         * Author      : Toby A. Trazo                                      
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
                mailClient.Credentials = new NetworkCredential(this.SMTPLogin, this.SMTPPassword);
                mailClient.Send(emailCust);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
/* Add End Email Sending with Attachment Toby/N-PAX 20101029 */
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

    }
}
