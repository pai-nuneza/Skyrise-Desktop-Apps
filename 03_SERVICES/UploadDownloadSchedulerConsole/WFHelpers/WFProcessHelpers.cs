namespace UploadDownloadSchedulerConsole.WFHelpers
{
    using System;
    using System.Data;
    using CommonPostingLibrary;

    public static class WFHandlers
    {
        private static bool HTMLEmail = CommonProcedures.GetAppSettingConfigBool("EWSSEMAILINHTMLFORMAT", false);
        private static string fromParameter = CommonProcedures.GetAppSettingConfigString("FROM", "", false);
        private static string urlParameter = CommonProcedures.GetAppSettingConfigString("EWSSServer", "", false);
        private static string EmailSignature = CommonProcedures.GetAppSettingConfigString("SIGNATURE", "", false);
        private static bool MailLogging = CommonProcedures.GetAppSettingConfigBool("MailLogging", false);
        private static string subjectParameter = "EWSS Notification";
        //Initialized external static variables.
        private static conEWSSNotification WrkFNotification = new conEWSSNotification();
        
        public enum MailFormat
        { 
            HTML,
            Text
        }

        public static string StandardWorkFlowNotificationSending(DataSet dsDetails, string TransactionType, string Action)
        {
            //Revise looping procedure for notifaction sending.
            WFHelpers.WFHandlers.MailFormat EmailStandardFormat = (HTMLEmail) ? WFHelpers.WFHandlers.MailFormat.HTML : WFHelpers.WFHandlers.MailFormat.Text;
            string UpdateControlNumbers = string.Empty;

            string[] ParamNames;

            if (dsDetails != null && dsDetails.Tables[0].Rows.Count > 0)
            {
                //Assign Parameters
                ParamNames = WFHelpers.WFHandlers.NotifyHeaders;

                //Get NotifyHeaders
                DataTable NotifyHeader = dsDetails.Tables[0].DefaultView.ToTable(true, ParamNames);

                foreach (DataRow HeaderRow in NotifyHeader.Rows)
                {
                    DataRow[] drrNotifyDetails;
                    string SentControlNumbers = string.Empty;

                    string hRecipientEmail = HeaderRow["Recipient Email"].ToString();
                    string hRecipientName = HeaderRow["Recipient Name"].ToString();
                    string GroupQuery = WFHelpers.WFHandlers.NotifyGroupFilter(hRecipientEmail, hRecipientName);

                    //Retreiving notification group.
                    drrNotifyDetails = dsDetails.Tables[0].Select(GroupQuery, "[Name], [Date] ASC");

                    //Start Composition of Email Introduction.
                    
                    string EmailGreetings           = string.Empty;
                    string EmailBodyIntroduction    = string.Empty;
                    string EmailMessage             = string.Empty;
                    string EmailNotifactionContent  = string.Empty;
                    int ItemNo = 0;
                    bool IsNotMessageOnly           = true;
                    bool IsNotify                   = false;
                    bool bFileInBehalf              = false;

                    //Start Composistion of Email Content
                    foreach (DataRow dr in drrNotifyDetails)
                    {
                        string RecipientName    = dr["Recipient Name"].ToString();
                        string RecipientTitle   = dr["Recipient Title"].ToString();
                        if (!Action.Equals("A")) //Approver
                            bFileInBehalf = (dr["In Behalf"].ToString().Equals("1") ? true : false);
                        else
                            bFileInBehalf = false;

                        ++ItemNo;

                        if (IsNotMessageOnly)
                        {
                            IsNotMessageOnly = false;
                            EmailGreetings = WFHelpers.WFHandlers.EmailGreetingComposer(RecipientName, RecipientTitle, EmailStandardFormat);
                            EmailBodyIntroduction = WFHelpers.WFHandlers.EmailBodyIntroductionComposer(TransactionType
                                                                                                     , Action
                                                                                                     , RecipientName
                                                                                                     , EmailStandardFormat
                                                                                                     , drrNotifyDetails.Length);
                        }

                        EmailMessage += WFHelpers.WFHandlers.EmailContentComposer(dr, Action, ItemNo, bFileInBehalf);

                        //Acummulating control number.
                        SentControlNumbers += "'" + dr["Document Sequence"].ToString() + "',";
                        IsNotify = true;
                    }

                    //Final email content formation.
                    EmailNotifactionContent = EmailGreetings + EmailBodyIntroduction + EmailMessage + ((EmailStandardFormat == WFHelpers.WFHandlers.MailFormat.HTML) ? "</table><br>" : "") + WFHelpers.WFHandlers.getFooter(EmailStandardFormat, urlParameter);

                    //Send Email.
                    if (IsNotify && conEWSSNotification.SendEmail("EWSSNotification"
                                                                    , fromParameter
                                                                    , hRecipientEmail
                                                                    , subjectParameter + " - " + TransactionType + (Action == "A" ? " Request" : " Status")
                                                                    , EmailNotifactionContent
                                                                    , HTMLEmail
                                                                    , MailLogging))
                    {
                        UpdateControlNumbers = UpdateControlNumbers + SentControlNumbers;
                    }
                }
            }

            return UpdateControlNumbers + "''";
        }
        public static string StandardWorkFlowNotificationSendingSummary(DataSet dsDetails, string TransactionType)
        {
            WFHelpers.WFHandlers.MailFormat EmailStandardFormat = (HTMLEmail) ? WFHelpers.WFHandlers.MailFormat.HTML : WFHelpers.WFHandlers.MailFormat.Text;
            string UpdateControlNumbers = string.Empty;

            string[] ParamNames;
            
            string EmailBody = string.Empty;
            HRCReportsBL reportsBL = new HRCReportsBL();
            if (dsDetails != null && dsDetails.Tables[0].Rows.Count > 0)
            {
                //Assign Parameters
                ParamNames = WFHelpers.WFHandlers.NotifyHeaders;

                //Get NotifyHeaders
                DataTable NotifyHeader = dsDetails.Tables[0].DefaultView.ToTable(true, new string[]{"UserCode","Recipient Email","Recipient Name"});
              

                foreach (DataRow HeaderRow in NotifyHeader.Rows)
                {
                    DataRow[] drrNotifyDetails;
                    string SentControlNumbers = string.Empty;
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    //Intialize notify header fields.
                    string Action = "";//HeaderRow["Action"].ToString();
                    string RecipientEmail = HeaderRow["Recipient Email"].ToString();
                    string RecipientName = HeaderRow["Recipient Name"].ToString();
                    string GroupQuery = string.Format("[UserCode]='{0}'", HeaderRow["UserCode"].ToString());
                   
                    //Retreiving notification group.
                    DataSet dataEmail = new DataSet();
                    DataTable dtEmail = new DataTable();
                    dtEmail.Columns.Add("Transaction Type");
                    dtEmail.Columns.Add("Name");
                    dtEmail.Columns.Add("Status");
                    dtEmail.Columns.Add("Date");
                    dtEmail.Columns.Add("Applied Date/Time");
                    dtEmail.Columns.Add("Type");
                    dtEmail.Columns.Add("Start Time");
                    dtEmail.Columns.Add("End Time");
                    dtEmail.Columns.Add("Hours");
                    dtEmail.Columns.Add("From");
                    dtEmail.Columns.Add("To");
                    dtEmail.Columns.Add("Beneficairy Name");
                    dtEmail.Columns.Add("Relationship");
                    dtEmail.Columns.Add("Street/Barangay/City");
                    dtEmail.Columns.Add("Reason");
                    dtEmail.Columns.Add("Section");
                    dtEmail.Columns.Add("Endorsed Date/Time");
                    drrNotifyDetails = dsDetails.Tables[0].Select(GroupQuery, "Recipient Email ASC");
                    
                    //Start Composistion of Email Introduction.
                    foreach (DataRow drnote in drrNotifyDetails)
                    {
                        DataRow drFixed = dtEmail.NewRow();
                        drFixed["Transaction Type"] = drnote["Transaction Type"];
                        drFixed["Name"] = drnote["Name"];
                        drFixed["Status"] = drnote["Status"];
                        drFixed["Date"] = drnote["Date"];
                        drFixed["Applied Date/Time"] = drnote["Applied Date/Time"];
                        drFixed["Type"] = drnote["Type"];
                        drFixed["Start Time"] = drnote["Start Time"];
                        drFixed["End Time"] = drnote["End Time"];
                        drFixed["Hours"] = drnote["Hours"];
                        drFixed["From"] = drnote["From"];
                        drFixed["To"] = drnote["To"];
                        drFixed["Beneficairy Name"] = drnote["Beneficairy Name"];
                        drFixed["Relationship"] = drnote["Relationship"];
                        drFixed["Street/Barangay/City"] = drnote["Street/Barangay/City"];
                        drFixed["Reason"] = drnote["Reason"];
                        drFixed["Section"] = drnote["Section"];
                        drFixed["Endorsed Date/Time"] = drnote["Endorsed Date/Time"];
                        dtEmail.Rows.Add(drFixed);
                    }
                    dataEmail.Tables.Add(dtEmail);
                    string EmailGreetings = string.Empty;
                    string EmailBodyIntroduction = string.Empty;
                    string EmailMessage = string.Empty;
                    string EmailNotifactionContent = string.Empty;
                    int ItemNo = 0;
                    bool IsNotMessageOnly = true;
                    bool IsNotify = false;
                    bool IsDetail = true; //This will indicate if query old(with detail column) or new(detail is specified to different columns).

                    //Start Composistion of Email Content
                    //foreach (DataRow dr in drrNotifyDetails)
                    
                   ;// dr["Recipient Name"].ToString();
                    string Name = "";
                    string Detail = "";
                    ++ItemNo;
                        
                    if (IsNotMessageOnly)
                    {
                           
                        if (dsDetails != null && dsDetails.Tables[0].Rows.Count > 0)
                        {
                            DataSet dsComp = new DataSet();
                            using (Posting.DAL.DALHelper dal = new Posting.DAL.DALHelper())
                            {
                                try
                                {
                                    dal.OpenDB();
                                    dsComp = dal.ExecuteDataSet(@"
                                                        select
                            	                            Mcm_CompanyName
                            	                            ,Mcm_CompanyAddress1
                            	                            + ' ' + Mcd_Name
                            	                            ,Mcm_TelNo
                                                        from M_Company
                                                        left join M_CodeDtl
                                                        on Mcd_Code = Mcm_CompanyAddress3
                                                        and Mcd_CodeType = 'ZIPCODE'");
                                    dal.CloseDB();
                                }
                                catch
                                { }
                            }
                               
                            UploadDownloadSchedulerConsole.DevExpressreports.DXrptHRCReports2 report = new UploadDownloadSchedulerConsole.DevExpressreports.DXrptHRCReports2(
                            "Transaction Summary"
                            , dsComp
                            , dataEmail
                            , reportsBL.getMargins()
                            , reportsBL.getReportFont()
                            , null
                            , false);
                            System.IO.MemoryStream mem = new System.IO.MemoryStream();
                            report.ExportToXlsx(mem);
                            mem.Seek(0, System.IO.SeekOrigin.Begin);
                            System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(mem, "Transaction_Summary" + "_Attachment.xlsx", "application/xlsx");
                            mail.Attachments.Add(att);
                            mem.Flush();
                            //EmailBody = EmailBody.Replace("@FORMULA", "");
                            mail.Body = EmailBody;
                            mail.IsBodyHtml = true;
                        }
                    }
                       
                        IsNotify = true;
                       

                        //Final email content formation.
                                //mail.Bcc.Add("knenriquez@n-pax.com");
                                //mail.Bcc.Add("rparriesgado@n-pax.com");
                    mail.To.Add(RecipientEmail);
                    mail.Subject = "Transaction Summary";
                    EmailGreetings = WFHelpers.WFHandlers.EmailGreetingComposer(RecipientName, "", EmailStandardFormat);
                    mail.Body = EmailGreetings + "Good day.<br><br> Attached to this email are the transaction(s) that were endorsed to you.<br><br>Respectfully yours,<br><br> SYSTEM ADMINISTRATOR";

                    string SMTPServer = string.Empty;
                    string SMTPLogin = string.Empty;
                    string SMTPPasswornd = string.Empty;
                    int SMTPPort = 0;
                    using (Posting.DAL.DALHelper dal = new Posting.DAL.DALHelper("", true))
                    {
                        try
                        {
                            DataSet ds = null;
                            ds = dal.ExecuteDataSet(@"
                                    SELECT [Msc_LineNo]
                                          ,[Msc_SMTPServer]
                                          ,[Msc_Login]
                                          ,[Msc_Password]
                                          ,[Msc_Port]
                                          ,[Msc_SenderEmail]
                                          ,[Msc_Description]
                                          ,[Msc_IsDefaultSMTP]
                                          ,[Msc_RecordStatus]
                                          ,[Usr_login]
                                          ,[ludatetime]
                                      FROM [M_SMTP]
                                      where [Msc_IsDefaultSMTP] = 0
                                        and Msc_RecordStatus = 'A'
                                        order by Msc_LineNo asc
                                        ", CommandType.Text);
                            if (ds != null && ds.Tables[0].Rows.Count != 0)
                            {
                                SMTPServer = ds.Tables[0].Rows[0]["Msc_SMTPServer"].ToString().Trim();
                                SMTPLogin = ds.Tables[0].Rows[0]["Msc_Login"].ToString().Trim();
                                SMTPPasswornd = ds.Tables[0].Rows[0]["Msc_Password"].ToString().Trim();
                                SMTPPort = Convert.ToInt32(ds.Tables[0].Rows[0]["Msc_Port"].ToString().Trim());
                                mail.From = new System.Net.Mail.MailAddress(ds.Tables[0].Rows[0]["Msc_SenderEmail"].ToString().Trim());
                            }
                            else
                            {
                                SMTPServer = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"].ToString();
                                SMTPLogin = System.Configuration.ConfigurationManager.AppSettings["SMTPLogin"].ToString();
                                SMTPPasswornd = System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"].ToString();
                                SMTPPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"].ToString());
                                mail.From = new System.Net.Mail.MailAddress("NOREPLY-PAYROLLSKIPPERSUPPORT@SSAS.COM");
                            }
                        }
                        catch
                        {}
                    }
                        EmailNotifactionContent = EmailGreetings + EmailBodyIntroduction + EmailMessage + ((EmailStandardFormat == WFHelpers.WFHandlers.MailFormat.HTML) ? "</table><br>" : "") + WFHelpers.WFHandlers.getFooter(EmailStandardFormat, urlParameter);

                        //Send Email.
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(SMTPServer, SMTPPort);
                        smtp.EnableSsl = false;

                        mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;

                        object userState = mail;

                        if (SMTPLogin.Trim() != string.Empty && SMTPPasswornd.Trim() != string.Empty)
                        {
                            smtp.Credentials = new System.Net.NetworkCredential(SMTPLogin, SMTPPasswornd);
                        }
                        smtp.Send(mail);
                    }
                }
            

            return UpdateControlNumbers + "''";
        }
        public static string StandardUnpairedLogsNotificationSending(DataSet dsDetails)
        {
            WFHelpers.WFHandlers.MailFormat EmailStandardFormat = (HTMLEmail) ? WFHelpers.WFHandlers.MailFormat.HTML : WFHelpers.WFHandlers.MailFormat.Text;

            //Assign Parameters
            string[] ParamNames = WFHelpers.WFHandlers.NotifyHeaders;

            //Get NotifyHeaders
            DataTable NotifyHeader = dsDetails.Tables[0].DefaultView.ToTable(true, ParamNames);

            foreach (DataRow HeaderRow in NotifyHeader.Rows)
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                string RecipientEmail = HeaderRow["Recipient Email"].ToString();
                string RecipientName = HeaderRow["Recipient Name"].ToString();
                string GroupQuery = string.Format("[Recipient Name]='{0}'", RecipientName);
                DataRow[] drUnpairedLogs = dsDetails.Tables[0].Select(GroupQuery, "[Date] ASC");

                string EmailGreetings           = string.Empty;
                string EmailBodyIntroduction    = string.Empty;
                string EmailMessage             = string.Empty;
                string EmailNotifactionContent = string.Empty;
                int ItemNo                      = 0;
                bool IsNotMessageOnly           = true;
                bool IsNotify                   = false;

                foreach (DataRow dr in drUnpairedLogs)
                {
                    string RecipientTitle = dr["Recipient Title"].ToString();
                    ++ItemNo;
                    if (IsNotMessageOnly)
                    {
                        IsNotMessageOnly = false;
                        EmailGreetings = WFHelpers.WFHandlers.EmailGreetingComposer(RecipientName, RecipientTitle, EmailStandardFormat);
                        EmailBodyIntroduction = WFHelpers.WFHandlers.UnpairedLogsEmailBodyIntroductionComposer(EmailStandardFormat, drUnpairedLogs.Length);
                    }

                    EmailMessage += WFHelpers.WFHandlers.UnpairedLogsEmailContentComposer(dr, ItemNo);
                    IsNotify = true;
                }

                //Final email content formation.
                EmailNotifactionContent = EmailGreetings + EmailBodyIntroduction + EmailMessage + ((EmailStandardFormat == WFHelpers.WFHandlers.MailFormat.HTML) ? "</table><br>" : "") + WFHelpers.WFHandlers.getFooter(EmailStandardFormat, urlParameter);

                //Send Email.
                if (IsNotify && conEWSSNotification.SendEmail("UnpairedLogs"
                                                                , fromParameter
                                                                , RecipientEmail
                                                                , "Unpaired Logs Notification"
                                                                , EmailNotifactionContent
                                                                , HTMLEmail
                                                                , MailLogging))
                {
                    //UpdateControlNumbers = UpdateControlNumbers + SentControlNumbers;
                }
            }
            return "";
        }

        #region Private Methods

        private static string[] _NotifyHeaders = new string[2] { "Recipient Email", "Recipient Name"};
        private static string[] NotifyHeaders
        {
            get
            {
                return _NotifyHeaders;
            }
        }
        private static string NotifyGroupFilter(string RecipientEmail, string RecipientName)
        {
            string query = string.Format("[{0}] = '{1}' and [{2}] = '{3}'"
                                        , _NotifyHeaders[0]   
                                        , RecipientEmail
                                        , _NotifyHeaders[1]
                                        , RecipientName);

            return query;                             
        }

        private static string EmailGreetingComposer(string RecipientName, string RecipientTitle, MailFormat Format)
        {
            string Greeting = string.Empty;
            
            switch(Format)
            {
                case MailFormat.HTML:
                    Greeting = "Dear " + RecipientTitle + " " + RecipientName + @",<br><br>" + getGreeting() + ".<br><br>";
                    break;
                case MailFormat.Text:
                    Greeting = "Dear "+ RecipientTitle + " " + RecipientName + @",

" + getGreeting() + @".
";
                    break;
            }

            return Greeting;
        }

        private static string EmailBodyIntroductionComposer(string TransactionType, string Action, string RecipientName, MailFormat Format, int recordCount)
        {
            string BodyIntroductionHTML = string.Empty;
            string BodyIntroductionText = string.Empty;

            #region Message body introduction - NEW

            if (Action.ToUpper() == "A") //Approver
            {
                BodyIntroductionHTML += @"The following employee(s) would like to file for " + TransactionType + @".<br><br>";

                BodyIntroductionText += @"
The following employee(s) would like to file for " + TransactionType + @".

";
            }
            else
            {
                if (recordCount == 1)
                {
                    BodyIntroductionHTML += @"This is the status of your request:<br><br>";

                    BodyIntroductionText += @"
This is the status of your request:

";
                }
                else if (recordCount > 1)
                {
                    BodyIntroductionHTML += @"The following are the status of your requests:<br><br>";

                    BodyIntroductionText += @"
The following are the status of your requests:

";
                }
            }


            BodyIntroductionHTML += @"<table border=""1"">";

            #endregion

            switch (Format)
            {
                case MailFormat.HTML:
                    return BodyIntroductionHTML;
                case MailFormat.Text:
                    return BodyIntroductionText;
                default:
                    return BodyIntroductionHTML;
            }
        }

        private static string EmailContentComposer(DataRow drDetails, string Action, int itemNo, bool bFileInBehalf)
        {
            //New Version

            string retVal = string.Empty;
            if (itemNo == 1)
            {
                //create column headers
                retVal += "<tr>";
                retVal += "<th>No.</th>";
                for (int i = 0; i < drDetails.Table.Columns.Count; i++)
                {

                    if (drDetails.Table.Columns[i].ColumnName.Equals("Recipient Email")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Name")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Title")
                      || drDetails.Table.Columns[i].ColumnName.Equals("In Behalf")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Profile")
                      || drDetails.Table.Columns[i].ColumnName.Equals("ID Number")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Name")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Document Sequence"))
                    {
                        if (Action.Equals("A") || bFileInBehalf) //A - Approver|Superior || Can File in Behalf - Employee 
                        {
                            if (drDetails.Table.Columns[i].ColumnName.Equals("ID Number") || drDetails.Table.Columns[i].ColumnName.Equals("Name") || drDetails.Table.Columns[i].ColumnName.Equals("Profile"))
                                retVal += "<th>" + drDetails.Table.Columns[i].ColumnName + "</th>";
                        }
                    }
                    else
                    {
                        retVal += "<th>" + drDetails.Table.Columns[i].ColumnName + "</th>";
                    }
                }
                retVal += "</tr>";
            }

            retVal += "<tr>";
            retVal += "<td>" + itemNo.ToString() + "</td>";
            for (int i = 0; i < drDetails.Table.Columns.Count; i++)
            {
                if (drDetails.Table.Columns[i].ColumnName.Equals("Recipient Email")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Name")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Title")
                      || drDetails.Table.Columns[i].ColumnName.Equals("In Behalf")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Profile")
                      || drDetails.Table.Columns[i].ColumnName.Equals("ID Number")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Name")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Document Sequence"))
                {
                    if (Action.Equals("A") || bFileInBehalf) //A - Approver|Superior || Can File in Behalf - Employee 
                    {
                        if (drDetails.Table.Columns[i].ColumnName.Equals("ID Number") || drDetails.Table.Columns[i].ColumnName.Equals("Name") || drDetails.Table.Columns[i].ColumnName.Equals("Profile"))
                            retVal += "<td>" + drDetails[i].ToString() + "</td>";
                    }   
                }
                else
                {
                    retVal += "<td>" + drDetails[i].ToString() + "</td>";
                }
            }
            retVal += "</tr>";

            return retVal;
        }

        #region Email Add on
        private static string getGreeting()
        {
            string value = "Good Day!";
            if (DateTime.Now.Hour < 12)
            {
                value = "Good Morning";
            }
            else if (DateTime.Now.Hour == 12)
            {
                value = "Good Noon";
            }
            else if (DateTime.Now.Hour < 17)
            {
                value = "Good Afternoon";
            }
            else
            {
                value = "Good Evening";
            }
            return value;
        }

        private static string getFooter(MailFormat Format, string urlParameter)
        {
            string message = string.Empty;

            switch (Format)
            {
                case MailFormat.HTML:
                    message = @"<br>
*This is a system-generated message. Please do not reply to this email.*<br><br>For more details, logon to EWSS at http://{0}
";
                    break;

                case MailFormat.Text:
                    message = @"
*This is a system-generated message. Please do not reply to this email.*

For more details, logon to EWSS at http://{0}
";
                    break;
            }

            return string.Format(message, urlParameter, EmailSignature);
        }

        #endregion

        #region Unpaired Logs
        private static string UnpairedLogsEmailBodyIntroductionComposer(MailFormat Format, int recordCount)
        {
            string BodyIntroductionHTML = string.Empty;
            string BodyIntroductionText = string.Empty;

            #region Message body introduction - NEW

            if (recordCount == 1)
            {
                BodyIntroductionHTML += @"You have unpaired log in the system.<br><br>";

                BodyIntroductionText += @"
You have unpaired log in the system.

";
            }
            else if (recordCount > 1)
            {
                BodyIntroductionHTML += @"You have unpaired logs in the system.<br><br>";

                BodyIntroductionText += @"
You have unpaired logs in the system.

";
            }


            BodyIntroductionHTML += @"<table border=""1"">";

            #endregion

            switch (Format)
            {
                case MailFormat.HTML:
                    return BodyIntroductionHTML;
                case MailFormat.Text:
                    return BodyIntroductionText;
                default:
                    return BodyIntroductionHTML;
            }
        }

        private static string UnpairedLogsEmailContentComposer(DataRow drDetails, int itemNo)
        {
            string retVal = string.Empty;
            if (itemNo == 1)
            {
                //create column headers
                retVal += "<tr>";
                retVal += "<th>No.</th>";
                for (int i = 0; i < drDetails.Table.Columns.Count; i++)
                {
                    if (drDetails.Table.Columns[i].ColumnName.Equals("IDNumber")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Email")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Name")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Title")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Seq"))
                    {
                        //if ((drDetails.Table.Columns[i].ColumnName.Equals("IDNumber") || (drDetails.Table.Columns[i].ColumnName.Equals("Recipient Name"))))
                        //{
                        //    retVal += "<th>" + drDetails.Table.Columns[i].ColumnName + "</th>";
                        //}
                    }
                    else
                    {
                        retVal += "<th>" + drDetails.Table.Columns[i].ColumnName + "</th>";
                    }
                }
                retVal += "</tr>";
            }

            retVal += "<tr>";
            retVal += "<td>" + itemNo.ToString() + "</td>";
            for (int i = 0; i < drDetails.Table.Columns.Count; i++)
            {
                if (drDetails.Table.Columns[i].ColumnName.Equals("IDNumber")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Email")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Name")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Recipient Title")
                      || drDetails.Table.Columns[i].ColumnName.Equals("Seq"))
                {
                    //if ((drDetails.Table.Columns[i].ColumnName.Equals("IDNumber") || (drDetails.Table.Columns[i].ColumnName.Equals("Recipient Name"))))
                    //{
                    //    retVal += "<td>" + drDetails[i].ToString() + "</td>";
                    //}
                }
                else
                {
                    retVal += "<td>" + drDetails[i].ToString() + "</td>";
                }
            }
            retVal += "</tr>";

            return retVal;
        }

        #endregion

        #endregion
    }
}
