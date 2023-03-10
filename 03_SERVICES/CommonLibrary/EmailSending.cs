using System;
using System.Configuration;
using System.Net.Mail;
using System.Net;


namespace CommonPostingLibrary
{
    public class EmailSending
    {
        private string SMTPServer;
        private int SMPTPort;
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
            SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            SMTPLogin = ConfigurationManager.AppSettings["SMTPLogin"].ToString();
            SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString();
            SMPTPort = 587;
            SMTPSubject = string.Empty;
            SMTPBody = string.Empty;
            SMTPFrom = string.Empty;
            SMTPTo = string.Empty;
            SMTPCc = string.Empty;
            //SMTPBcc = "jccapetillo@n-pax.com";
        }

        public void Send()
        {
            try
            {
                MailMessage emailCust = new MailMessage(this.SMTPFrom, this.SMTPTo);
                emailCust.Subject = this.SMTPSubject;
                emailCust.Body = this.SMTPBody;

                //if (this.SMTPCc.Trim().Equals(string.Empty))
                //    this.SMTPCc = "jccapetillo@n-pax.com";

                //if (this.SMTPBcc.Trim().Equals(string.Empty))
                //    this.SMTPBcc = "jccapetillo@n-pax.com";

                emailCust.CC.Add(this.SMTPCc);
                emailCust.Bcc.Add(this.SMTPBcc);

                SmtpClient mailClient = new SmtpClient(this.SMTPServer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
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
            get
            {
                return this.SMTPServer;
            }
            set
            {
                this.SMTPServer = value;
            }
        }

        public int Port
        {
            get
            {
                return this.SMPTPort;
            }
            set
            {
                this.SMPTPort = value;
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
            get
            {
                return this.SMTPTo;
            }
            set
            {
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
        public string Bcc
        {
            get
            {
                return this.SMTPBcc;
            }
            set
            {
                this.SMTPBcc = value;
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
