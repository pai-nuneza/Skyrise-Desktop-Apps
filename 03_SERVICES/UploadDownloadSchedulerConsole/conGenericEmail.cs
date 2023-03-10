using System;
using Posting.DAL;
using System.Data;
using System.Net.Mail;
using System.IO;

namespace UploadDownloadSchedulerConsole
{
    public class conGenericEmail
    {
        public void ProcessEmail(DateTime dtNow)
        {
            DataSet dsServices = ExecuteDataSetQuery3(
            @"
                    select 
	                    Meh_ServiceCode
	                    ,Meh_ScheduleType
                        ,Meh_Interval
	                    ,Meh_TimeSetting
	                    ,Meh_InitialRun
	                    ,Meh_LatestRun
                        ,Meh_NextRun
                        ,Meh_Runtime
                        ,Meh_Monday
                        ,Meh_Tuesday
                        ,Meh_Wednesday
                        ,Meh_Thursday
                        ,Meh_Friday
                        ,Meh_Saturday
                        ,Meh_Sunday
                        ,Meh_MonthDay
                        ,Meh_NthDay
                        ,Meh_DayOfWeek
                        ,Meh_SpecialInterval
                        ,Meh_SpecialCondition
                    from M_EmailServiceHdr
                    where Meh_RecordStatus = 'A'
                "
            );

            //Exits when no registerd services
            if (dsServices.Tables[0].Rows.Count == 0)
                return;

            #region Get Time settings of Registered services

            dsServices.Tables[0].Columns.Add("StartTime");
            DataSet dsProcess = dsServices.Clone();

            for (int idx = 0; idx < dsServices.Tables[0].Rows.Count; idx++)
            {
                string serviceCode = dsServices.Tables[0].Rows[idx]["Meh_ServiceCode"].ToString();
                if (dsServices.Tables[0].Rows[idx]["Meh_ScheduleType"].ToString() == "D")
                {
                    switch (dsServices.Tables[0].Rows[idx]["Meh_TimeSetting"].ToString())
                    {
                        case "1":
                            //Multi
                            dsServices.Tables[0].Rows[idx]["StartTime"] =
                            Get_T_SchedulerServiceTimeSetting_ClosestTime(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                            break;
                        case "2":
                            //Specified Runtime
                            dsServices.Tables[0].Rows[idx]["StartTime"] =
                            Get_Runtime(dsServices.Tables[0].Rows[idx]["Meh_Runtime"].ToString(), dtNow.ToString("HHmm"));
                            break;
                        case "3":
                            //Special interval
                            dsServices.Tables[0].Rows[idx]["StartTime"] =
                            Get_TimeInterval(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                            break;
                        case "4":
                            //Other
                            dsServices.Tables[0].Rows[idx]["StartTime"] =
                            Get_OtherTime(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                            break;
                    }
                }
                else
                    if (dsServices.Tables[0].Rows[idx]["Meh_ScheduleType"].ToString() == "W")
                    {
                        if (WeeklyCheck(dsServices.Tables[0].Rows[idx], dtNow))
                        {
                            switch (dsServices.Tables[0].Rows[idx]["Meh_TimeSetting"].ToString())
                            {
                                case "1":
                                    //Multi
                                    dsServices.Tables[0].Rows[idx]["StartTime"] =
                                    Get_T_SchedulerServiceTimeSetting_ClosestTime(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                                    break;
                                case "2":
                                    //Specified Runtime
                                    dsServices.Tables[0].Rows[idx]["StartTime"] =
                                    Get_Runtime(dsServices.Tables[0].Rows[idx]["Meh_Runtime"].ToString(), dtNow.ToString("HHmm"));
                                    break;
                                case "3":
                                    //Special interval
                                    dsServices.Tables[0].Rows[idx]["StartTime"] =
                                    Get_TimeInterval(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                                    break;
                            }
                        }
                    }
                    else
                        if (dsServices.Tables[0].Rows[idx]["Meh_ScheduleType"].ToString() == "M")
                        {
                            if (MonthlyCheck(dsServices.Tables[0].Rows[idx], dtNow))
                            {
                                switch (dsServices.Tables[0].Rows[idx]["Meh_TimeSetting"].ToString())
                                {
                                    case "1":
                                        //Multi
                                        dsServices.Tables[0].Rows[idx]["StartTime"] =
                                        Get_T_SchedulerServiceTimeSetting_ClosestTime(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                                        break;
                                    case "2":
                                        //Specified Runtime
                                        dsServices.Tables[0].Rows[idx]["StartTime"] =
                                        Get_Runtime(dsServices.Tables[0].Rows[idx]["Meh_Runtime"].ToString(), dtNow.ToString("HHmm"));
                                        break;
                                    case "3":
                                        //Special interval
                                        dsServices.Tables[0].Rows[idx]["StartTime"] =
                                        Get_TimeInterval(dsServices.Tables[0].Rows[idx], dtNow.ToString("HHmm"));
                                        break;
                                }
                            }
                        }
                if (dsServices.Tables[0].Rows[idx]["StartTime"].ToString().Trim() != string.Empty)
                {
                    DataRow dr = dsProcess.Tables[0].NewRow();
                    dr["Meh_ServiceCode"] = dsServices.Tables[0].Rows[idx]["Meh_ServiceCode"];
                    dr["Meh_ScheduleType"] = dsServices.Tables[0].Rows[idx]["Meh_ScheduleType"];
                    dr["Meh_Interval"] = dsServices.Tables[0].Rows[idx]["Meh_Interval"];
                    dr["Meh_TimeSetting"] = dsServices.Tables[0].Rows[idx]["Meh_TimeSetting"];
                    dr["Meh_InitialRun"] = dsServices.Tables[0].Rows[idx]["Meh_InitialRun"];
                    dr["Meh_LatestRun"] = dsServices.Tables[0].Rows[idx]["Meh_LatestRun"];
                    dr["Meh_NextRun"] = dsServices.Tables[0].Rows[idx]["Meh_NextRun"];
                    dr["Meh_Runtime"] = dsServices.Tables[0].Rows[idx]["Meh_Runtime"];
                    dr["Meh_Monday"] = dsServices.Tables[0].Rows[idx]["Meh_Monday"];
                    dr["Meh_Tuesday"] = dsServices.Tables[0].Rows[idx]["Meh_Tuesday"];
                    dr["Meh_Wednesday"] = dsServices.Tables[0].Rows[idx]["Meh_Wednesday"];
                    dr["Meh_Thursday"] = dsServices.Tables[0].Rows[idx]["Meh_Thursday"];
                    dr["Meh_Friday"] = dsServices.Tables[0].Rows[idx]["Meh_Friday"];
                    dr["Meh_Saturday"] = dsServices.Tables[0].Rows[idx]["Meh_Saturday"];
                    dr["Meh_Sunday"] = dsServices.Tables[0].Rows[idx]["Meh_Sunday"];
                    dr["Meh_MonthDay"] = dsServices.Tables[0].Rows[idx]["Meh_MonthDay"];
                    dr["Meh_NthDay"] = dsServices.Tables[0].Rows[idx]["Meh_NthDay"];
                    dr["Meh_DayOfWeek"] = dsServices.Tables[0].Rows[idx]["Meh_DayOfWeek"];
                    dr["Meh_SpecialInterval"] = dsServices.Tables[0].Rows[idx]["Meh_SpecialInterval"];
                    dr["Meh_SpecialCondition"] = dsServices.Tables[0].Rows[idx]["Meh_SpecialCondition"];
                    dr["StartTime"] = dsServices.Tables[0].Rows[idx]["StartTime"];
                    dsProcess.Tables[0].Rows.Add(dr);
                }
            }

            bool flag = true;
            for (int idx = 0; dsProcess != null && idx < dsProcess.Tables[0].Rows.Count; idx++)
            {
                if (dsProcess.Tables[0].Rows[idx]["StartTime"].ToString().Trim() != string.Empty)
                {
                    flag = doEmail(dsProcess.Tables[0].Rows[idx]);
                    if (flag)
                        UpdateServiceHeader(dsProcess.Tables[0].Rows[idx], dtNow, "SERVICE");
                }
            }

            #endregion
        }

        #region Time assignment

        public string Get_T_SchedulerServiceTimeSetting_ClosestTime(DataRow dr, string dtNow)
        {
            string retTime = string.Empty;

            DataSet ds = ExecuteDataSetQuery3(
            string.Format(
            @"
                    select * from M_ServiceTiming
                    where Mst_ServiceCode = '{0}'
                ", dr["Meh_ServiceCode"].ToString()));

            if (ds.Tables[0].Rows.Count > 0)
            {
                bool flag = true;
                for (int idx = 2; idx < 98 && flag; idx++)
                {
                    string time = ds.Tables[0].Columns[idx].Caption;
                    time = time.Replace("Mst_", "");
                    if (Convert.ToBoolean(ds.Tables[0].Rows[0][idx])
                    &&
                    GetTimeToMinutes(time, string.Empty)
                    == GetTimeToMinutes(dtNow, string.Empty)
                    )
                    {
                        flag = false;
                        retTime = time;
                    }
                }
            }

            return retTime;
        }

        public string Get_Runtime(string runtime, string dtNow)
        {
            string retTime = string.Empty;
            if (GetTimeToMinutes(dtNow, string.Empty) == GetTimeToMinutes(runtime, string.Empty))
                retTime = runtime;
            return retTime;
        }

        public string Get_TimeInterval(DataRow dr, string dtNow)
        {

            string retTime = string.Empty;
            string sqlQuery = string.Empty;
            int interval = 0;
            #region Get Special Condition query

            sqlQuery = dr["Meh_SpecialCondition"].ToString();
            interval = Convert.ToInt32(dr["Meh_SpecialInterval"].ToString());

            #endregion
            DataSet ds;
            #region Locate the time using query

            if (sqlQuery.Trim() != string.Empty)
            {
                ds = ExecuteDataSetQuery2(sqlQuery, dr["Meh_ServiceCode"].ToString());
                for (int idx = 0; idx < ds.Tables[0].Rows.Count; idx++)
                {
                    // if interval set is negative then minutes b4 
                    // if interval set is positive then minutes after
                    if ((GetTimeToMinutes(dtNow, string.Empty)) == (GetTimeToMinutes(ds.Tables[0].Rows[idx][0].ToString(), string.Empty) + (interval)))
                        retTime = dtNow;
                }
            }

            #endregion

            return retTime;
        }

        public string Get_OtherTime(DataRow dr, string dtNow)
        {
            string retTime = string.Empty;
            string LatestRun = string.Empty;
            LatestRun = Convert.ToDateTime(dr["Meh_NextRun"]).ToString("HHmm");
            if (GetTimeToMinutes(LatestRun, string.Empty) == GetTimeToMinutes(dtNow, string.Empty))
                retTime = LatestRun;

            return retTime;
        }

        public int GetTimeToMinutes(string time, string plus)
        {
            int digi = (Convert.ToInt32(time.Substring(0, 2)) * 60) + Convert.ToInt32(time.Substring(2, 2));
            if (plus != string.Empty)
            {
                digi += (Convert.ToInt32(plus));
            }
            return digi;
        }

        public string GetTimeToHour(int time, int plus)
        {
            string ret = string.Empty;
            int part1 = 0;
            int part2 = 0;
            string str1 = string.Empty;
            string str2 = string.Empty;
            part1 = time / 60;
            part2 = time % 60;
            if (plus != 0)
            {
                if (plus >= 60)
                {
                    part1 += plus / 60;
                }
                part2 += plus % 60;
            }
            str1 = part1.ToString();
            str2 = part2.ToString();
            if (str1.Length < 2)
                str1 = "0" + str1;
            if (str2.Length < 2)
                str2 = "0" + str2;
            ret = str1.Substring(0, 2) + str2.Substring(0, 2);
            return ret;
        }

        #endregion

        #region Weekly checking

        private bool WeeklyCheck(DataRow dr, DateTime dtNow)
        {
            bool ret = false;
            DateTime dt;
            dt = Convert.ToDateTime(dr["Meh_NextRun"]);
            int weeks = Convert.ToInt32(dr["Meh_Interval"]) * 7;
            bool isDay = false;
            switch (dtNow.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (Convert.ToBoolean(dr["Meh_Monday"]))
                        isDay = true;
                    break;
                case DayOfWeek.Tuesday:
                    if (Convert.ToBoolean(dr["Meh_Tuesday"]))
                        isDay = true;
                    break;
                case DayOfWeek.Wednesday:
                    if (Convert.ToBoolean(dr["Meh_Wednesday"]))
                        isDay = true;
                    break;
                case DayOfWeek.Thursday:
                    if (Convert.ToBoolean(dr["Meh_Thursday"]))
                        isDay = true;
                    break;
                case DayOfWeek.Friday:
                    if (Convert.ToBoolean(dr["Meh_Friday"]))
                        isDay = true;
                    break;
                case DayOfWeek.Saturday:
                    if (Convert.ToBoolean(dr["Meh_Saturday"]))
                        isDay = true;
                    break;
                case DayOfWeek.Sunday:
                    if (Convert.ToBoolean(dr["Meh_Sunday"]))
                        isDay = true;
                    break;
            }
            if (isDay)
            {
                if (dt.ToString("MM/dd/yyyy") == dtNow.ToString("MM/dd/yyyy"))
                    ret = true;
            }

            return ret;
        }

        #endregion

        #region Monthly checking

        private bool MonthlyCheck(DataRow dr, DateTime dtNow)
        {
            bool ret = false;
            DateTime dt;

            dt = Convert.ToDateTime(dr["Meh_NextRun"]);

            if (dt.ToString("MM/dd/yyyy") == dtNow.ToString("MM/dd/yyyy"))
                ret = true;

            return ret;
        }

        #endregion

        #region Email

        public bool doEmail(DataRow dr)
        {
            bool ret = true;
            string _To = string.Empty;
            string _CC = string.Empty;
            string _BCC = string.Empty;
            string EmailSubject = string.Empty;
            string EmailBody = string.Empty;

            #region Get Email Settings

            DataSet DsEmailSettings = ExecuteDataSetQuery3(
            string.Format(@"
                    select 
	                    Meh_Subject
	                    ,Meh_SubjectFormula
	                    ,Meh_MessageBody
	                    ,Meh_MessageFormula
	                    ,Meh_EmailFrom
	                    ,Meh_ResultDisplayAs
	                    ,Meh_IsAllowEmailWithNoDetail
                    from M_EmailServiceHdr
                    where Meh_ServiceCode = '{0}'
                ", dr["Meh_ServiceCode"].ToString())
            );

            #endregion

            #region Get Recipients

            DataSet dsToRecipients = ExecuteDataSetQuery3(
            string.Format(@"
                    select 
                    *
                    ,
                    case when Med_RecipientType = 'T'
                    then '1'
                    when Med_RecipientType = 'C'
                    then '2'
                    else '3'
                    end [OrderType]

                     from M_EmailServiceDtl
                     where Med_ServiceCode = '{0}'
                    order by [OrderType] asc, Med_RecipientSeq asc
                       
                ", dr["Meh_ServiceCode"].ToString())
            );



            #endregion

            #region Setup Email Subject

            EmailSubject = DsEmailSettings.Tables[0].Rows[0]["Meh_Subject"].ToString() + " ";

            if (DsEmailSettings.Tables[0].Rows[0]["Meh_SubjectFormula"].ToString().Trim() != string.Empty)
            {
                try
                {
                    EmailSubject += ExecuteDataSetQuery2(DsEmailSettings.Tables[0].Rows[0]["Meh_SubjectFormula"].ToString().Trim().Replace("''", "'"), dr["Meh_ServiceCode"].ToString()).Tables[0].Rows[0][0].ToString();
                }
                catch
                {
                }
            }

            #endregion

            #region Setup Body

            EmailBody = DsEmailSettings.Tables[0].Rows[0]["Meh_MessageBody"].ToString();
            DataSet dsEmailBody = null;
            if (DsEmailSettings.Tables[0].Rows[0]["Meh_MessageFormula"].ToString() != string.Empty)
            {
                try
                {
                    dsEmailBody = ExecuteDataSetQuery2(DsEmailSettings.Tables[0].Rows[0]["Meh_MessageFormula"].ToString().Replace("''", "'"), dr["Meh_ServiceCode"].ToString());
                }
                catch
                {
                }
            }

            #endregion

            if ((dsEmailBody != null && dsEmailBody.Tables[0].Rows.Count > 0)
            || Convert.ToBoolean(DsEmailSettings.Tables[0].Rows[0]["Meh_IsAllowEmailWithNoDetail"]))
            {
                #region Main Process
                try
                {
                    string[] strTo = _To.Split(',');
                    string[] strCC = _CC.Split(',');
                    string[] strBCC = _BCC.Split(',');
                    string SMTPServer = string.Empty;
                    string SMTPLogin = string.Empty;
                    string SMTPPasswornd = string.Empty;
                    int SMTPPort = 0;

                    DataSet ds = null;

                    using (DALHelper dal = new DALHelper("", true))
                    {
                        try
                        {
                            dal.OpenDB();
                            ds = dal.ExecuteDataSet(@"
                            SELECT [Msc_LineNo]
                                  ,[Msc_SMTPServer]
                                  ,[Msc_Login]
                                  ,[Msc_Password]
                                  ,[Msc_Port]
                                  ,[Msc_Description]
                                  ,[Msc_Description]
                                  ,[Msc_IsDefaultSMTP]
                                  ,[Msc_RecordStatus]
                                  ,[Usr_login]
                                  ,[ludatetime]
                              FROM [M_SMTP]
                              where [Msc_IsDefaultSMTP] = 1
                                and Msc_RecordStatus = 'A'
                            ", CommandType.Text);

                            if (ds != null && ds.Tables[0].Rows.Count == 0)
                            {
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
                                if (ds != null && ds.Tables[0].Rows.Count == 0)
                                {
                                    SMTPServer = ds.Tables[0].Rows[0]["Msc_SMTPServer"].ToString().Trim();
                                    SMTPLogin = ds.Tables[0].Rows[0]["Msc_Login"].ToString().Trim();
                                    SMTPPasswornd = ds.Tables[0].Rows[0]["Msc_Password"].ToString().Trim();
                                    SMTPPort = Convert.ToInt32(ds.Tables[0].Rows[0]["Msc_Port"].ToString().Trim());
                                }
                                else
                                {
                                    SMTPServer = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"].ToString();
                                    SMTPLogin = System.Configuration.ConfigurationManager.AppSettings["SMTPLogin"].ToString();
                                    SMTPPasswornd = System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"].ToString();
                                    SMTPPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"].ToString());
                                }
                            }
                            else
                            {
                                SMTPServer = ds.Tables[0].Rows[0]["Msc_SMTPServer"].ToString().Trim();
                                SMTPLogin = ds.Tables[0].Rows[0]["Msc_Login"].ToString().Trim();
                                SMTPPasswornd = ds.Tables[0].Rows[0]["Msc_Password"].ToString().Trim();
                                SMTPPort = Convert.ToInt32(ds.Tables[0].Rows[0]["Msc_Port"].ToString().Trim());
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(DsEmailSettings.Tables[0].Rows[0]["Meh_EmailFrom"].ToString());

                    for (int idx = 0; idx < dsToRecipients.Tables[0].Rows.Count; idx++)
                    {
                        if (dsToRecipients.Tables[0].Rows[idx]["Med_RecipientType"].ToString() == "T")
                        {
                            mail.To.Add(dsToRecipients.Tables[0].Rows[idx]["Med_EmailAddress"].ToString());
                        }
                        else
                            if (dsToRecipients.Tables[0].Rows[idx]["Med_RecipientType"].ToString() == "C")
                            {
                                mail.CC.Add(dsToRecipients.Tables[0].Rows[idx]["Med_EmailAddress"].ToString());
                            }
                            else
                                if (dsToRecipients.Tables[0].Rows[idx]["Med_RecipientType"].ToString() == "B")
                                {
                                    mail.Bcc.Add(dsToRecipients.Tables[0].Rows[idx]["Med_EmailAddress"].ToString());
                                }
                    }

                    //mail.Bcc.Add("");

                    mail.Subject = EmailSubject;

                    if (DsEmailSettings.Tables[0].Rows[0]["Meh_ResultDisplayAs"].ToString() == "A")
                    {
                        HRCReportsBL reportsBL = new HRCReportsBL();
                        if (dsEmailBody != null && dsEmailBody.Tables[0].Rows.Count > 0)
                        {
                            DataSet dsComp = ExecuteDataSetQuery2(@"
                            select
	                            Mcm_CompanyName
	                            ,Mcm_CompanyAddress1
	                            + ' ' + Mcd_Name 
	                            ,Mcm_TelNo
                            from M_Company
                            left join M_CodeDtl
                            on Mcd_Code = Mcm_CompanyAddress3
                            and Mcd_CodeType = 'ZIPCODE'
                            ", dr["Meh_ServiceCode"].ToString());
                            UploadDownloadSchedulerConsole.DevExpressreports.DXrptHRCReports2 report = new UploadDownloadSchedulerConsole.DevExpressreports.DXrptHRCReports2(
                            EmailSubject
                            , dsComp
                            , dsEmailBody
                            , reportsBL.getMargins()
                            , reportsBL.getReportFont()
                            , null
                            , false);
                            MemoryStream mem = new MemoryStream();
                            report.ExportToXlsx(mem);
                            mem.Seek(0, System.IO.SeekOrigin.Begin);
                            Attachment att = new Attachment(mem, EmailSubject + "_Attachment.xlsx", "application/xlsx");
                            mail.Attachments.Add(att);
                            mem.Flush();
                            EmailBody = EmailBody.Replace("@FORMULA", "");
                            mail.Body = EmailBody;
                        }
                    }
                    else
                    {
                        int[] widths = getMaxWidths(dsEmailBody);
                        string Body = string.Empty;

                        for (int idx = 0; idx < dsEmailBody.Tables[0].Columns.Count; idx++)
                        {
                            string temp = dsEmailBody.Tables[0].Columns[idx].Caption.Trim();
                            if (temp.Length < widths[idx])
                            {
                                int len = temp.Length;
                                for (int i = 0; i < widths[idx] - len; i++)
                                {
                                    temp += " ";
                                }
                            }
                            Body += temp;
                        }

                        for (int idx = 0; idx < dsEmailBody.Tables[0].Rows.Count; idx++)
                        {
                            Body += "\n";
                            for (int idx2 = 0; idx2 < dsEmailBody.Tables[0].Columns.Count; idx2++)
                            {
                                string temp = dsEmailBody.Tables[0].Rows[idx][idx2].ToString().Trim();
                                if (temp.Length < widths[idx2])
                                {
                                    int len = temp.Length;
                                    for (int i = 0; i < widths[idx2] - len; i++)
                                    {
                                        temp += " ";
                                    }
                                }
                                Body += temp;
                            }
                        }
                        EmailBody = EmailBody.Replace("\n", "<br />").Replace("\t", "&#09;");

                        //EmailBody = EmailBody.Replace("@FORMULA", Body);
                        string body2 = string.Empty;
                        body2 = @"
                                <html>
                                <body style='border: 0px; margin: 0px; font-family: Tahoma; font-size: 12px;'>

                                @BODY

                                </body>
                                </html>
                        ";
                        string formula2 = string.Empty;
                        formula2 = @"
                            <table style='border: 0px; margin: 0px; padding-top: 2px; padding-left: 5px; padding-bottom: 2px; padding-right: 5px; font-family: Tahoma; font-size: 12px;'>
                               <tr>
                        ";
                        for (int idx = 0; idx < dsEmailBody.Tables[0].Columns.Count; idx++)
                        {
                            formula2 += @"<td>" + dsEmailBody.Tables[0].Columns[idx].Caption.ToString() + @"</td>";
                        }
                        for (int idx = 0; idx < dsEmailBody.Tables[0].Rows.Count; idx++)
                        {
                            formula2 += @"
                                <tr>";
                            for (int idx2 = 0; idx2 < dsEmailBody.Tables[0].Columns.Count; idx2++)
                            {
                                formula2 += "<td>" + dsEmailBody.Tables[0].Rows[idx][idx2].ToString() + @"</td>";
                            }
                            formula2 += @"
                                </tr>";
                        }
                        formula2 += "</table>";
                        EmailBody = EmailBody.Replace("@FORMULA", formula2);
                        body2 = body2.Replace("@BODY", EmailBody);
                        mail.IsBodyHtml = true;
                        mail.Body = body2;
                    }
                    //mail.Body = EmailBody;
                    SmtpClient smtp = new SmtpClient(SMTPServer, SMTPPort);
                    smtp.EnableSsl = false;

                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    object userState = mail;

                    if (SMTPLogin.Trim() != string.Empty && SMTPPasswornd.Trim() != string.Empty)
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(SMTPLogin, SMTPPasswornd);
                    }
                    smtp.Send(mail);
                    //smtp.Send(mail, userState);
                    ret = true;
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                }

                #endregion
            }

            return ret;
        }

        private int[] getMaxWidths(DataSet dsSource)
        {
            //labelsTotalWidth = 0;
            int[] colwidths = new int[dsSource.Tables[0].Columns.Count];
            string tempWidth = "";

            for (int i = 0; i < dsSource.Tables[0].Columns.Count; i++)
            {
                int maxWidth = 0;
                tempWidth = dsSource.Tables[0].Columns[i].Caption.Trim();
                maxWidth = maxWidth > tempWidth.Length ? maxWidth : tempWidth.Length;

                for (int j = 0; j < dsSource.Tables[0].Rows.Count; j++)
                {
                    tempWidth = dsSource.Tables[0].Rows[j][i].ToString().Trim();
                    maxWidth = maxWidth > tempWidth.Length ? maxWidth : tempWidth.Length;
                }
                colwidths[i] = maxWidth + 10;
                //labelsTotalWidth += maxWidth + 10;
            }

            return colwidths;
        }


        #endregion

        #region Update Last Run

        public void UpdateServiceFreeRun(DataRow dr, DateTime dtNow, string user)
        {
            #region update
            ExecuteNonQuery3(
            string.Format(
            @"
                    update M_EmailServiceHdr
                    set 
                            Meh_ForceRun = '{0}'
                            ,Usr_Login = '{1}'
                            ,Ludatetime = GETDATE()
                    where Meh_ServiceCode = '{2}'
                ", dtNow.ToString("MM/dd/yyyy HH:mm")
            , user
            , dr["Meh_ServiceCode"].ToString().Trim())
            );
            #endregion
        }

        public void UpdateServiceHeader(DataRow dr, DateTime dtNow, string user)
        {
            if (dr["Meh_ScheduleType"].ToString().Trim() == "D")
            {
                UpdateServiceHeaderDaily(dr, dtNow, user);
            }
            else
                if (dr["Meh_ScheduleType"].ToString().Trim() == "W")
                {
                    UpdateServiceHeaderWeekly(dr, dtNow, user);
                }
                else
                    if (dr["Meh_ScheduleType"].ToString().Trim() == "M")
                    {
                        UpdateServiceHeaderMonthly(dr, dtNow, user);
                    }
        }

        #region Update Next run Daily

        private void UpdateServiceHeaderDaily(DataRow dr, DateTime dtNow, string user)
        {
            DateTime dtNextRun = DateTime.Now;
            string time = dr["StartTime"].ToString();

            if (dr["Meh_TimeSetting"].ToString() == "1") //Multi
            {
                #region Multi
                #region Query
                DataSet dstemp = ExecuteDataSetQuery3(string.Format(
                @"SELECT [Mst_0000]
                          ,[Mst_0015]
                          ,[Mst_0030]
                          ,[Mst_0045]
                          ,[Mst_0100]
                          ,[Mst_0115]
                          ,[Mst_0130]
                          ,[Mst_0145]
                          ,[Mst_0200]
                          ,[Mst_0215]
                          ,[Mst_0230]
                          ,[Mst_0245]
                          ,[Mst_0300]
                          ,[Mst_0315]
                          ,[Mst_0330]
                          ,[Mst_0345]
                          ,[Mst_0400]
                          ,[Mst_0415]
                          ,[Mst_0430]
                          ,[Mst_0445]
                          ,[Mst_0500]
                          ,[Mst_0515]
                          ,[Mst_0530]
                          ,[Mst_0545]
                          ,[Mst_0600]
                          ,[Mst_0615]
                          ,[Mst_0630]
                          ,[Mst_0645]
                          ,[Mst_0700]
                          ,[Mst_0715]
                          ,[Mst_0730]
                          ,[Mst_0745]
                          ,[Mst_0800]
                          ,[Mst_0815]
                          ,[Mst_0830]
                          ,[Mst_0845]
                          ,[Mst_0900]
                          ,[Mst_0915]
                          ,[Mst_0930]
                          ,[Mst_0945]
                          ,[Mst_1000]
                          ,[Mst_1015]
                          ,[Mst_1030]
                          ,[Mst_1045]
                          ,[Mst_1100]
                          ,[Mst_1115]
                          ,[Mst_1130]
                          ,[Mst_1145]
                          ,[Mst_1200]
                          ,[Mst_1215]
                          ,[Mst_1230]
                          ,[Mst_1245]
                          ,[Mst_1300]
                          ,[Mst_1315]
                          ,[Mst_1330]
                          ,[Mst_1345]
                          ,[Mst_1400]
                          ,[Mst_1415]
                          ,[Mst_1430]
                          ,[Mst_1445]
                          ,[Mst_1500]
                          ,[Mst_1515]
                          ,[Mst_1530]
                          ,[Mst_1545]
                          ,[Mst_1600]
                          ,[Mst_1615]
                          ,[Mst_1630]
                          ,[Mst_1645]
                          ,[Mst_1700]
                          ,[Mst_1715]
                          ,[Mst_1730]
                          ,[Mst_1745]
                          ,[Mst_1800]
                          ,[Mst_1815]
                          ,[Mst_1830]
                          ,[Mst_1845]
                          ,[Mst_1900]
                          ,[Mst_1915]
                          ,[Mst_1930]
                          ,[Mst_1945]
                          ,[Mst_2000]
                          ,[Mst_2015]
                          ,[Mst_2030]
                          ,[Mst_2045]
                          ,[Mst_2100]
                          ,[Mst_2115]
                          ,[Mst_2130]
                          ,[Mst_2145]
                          ,[Mst_2200]
                          ,[Mst_2215]
                          ,[Mst_2230]
                          ,[Mst_2245]
                          ,[Mst_2300]
                          ,[Mst_2315]
                          ,[Mst_2330]
                          ,[Mst_2345]
                      FROM M_ServiceTiming
                            where Mst_ServiceCode = '{0}'", dr["Meh_ServiceCode"].ToString())
                );
                #endregion

                int trail = -1;
                int flag1 = -1;
                int day = 0;
                for (int idx = 0; idx != trail && flag1 == -1; idx++)
                {
                    if (idx == dstemp.Tables[0].Columns.Count)
                    {
                        idx = 0;
                        day = 1;
                    }
                    if (trail != -1)
                    {
                        if (Convert.ToBoolean(dstemp.Tables[0].Rows[0][idx]))
                        {
                            flag1 = idx;
                        }
                    }
                    string queryTime = dstemp.Tables[0].Columns[idx].Caption.ToString().Replace("Sst_", "");
                    if (queryTime == time)
                    {
                        trail = idx;
                    }
                }
                if (flag1 == -1)
                {
                    dtNextRun = ComputeForNextRunDaily(dr, dtNow, time);
                }
                else
                {
                    dtNow = dtNow.AddDays(day);
                    dtNextRun = ComputeForNextRunDailyDay(dr, dtNow, dstemp.Tables[0].Columns[flag1].Caption.ToString().Replace("Sst_", ""));
                }

                #endregion
            }
            else
                if (dr["Meh_TimeSetting"].ToString() == "2") //Specific
                {
                    #region Specific
                    dtNextRun = ComputeForNextRunDaily(dr, dtNow, time);

                    #endregion
                }
                else
                    if (dr["Meh_TimeSetting"].ToString() == "3") //Special
                    {
                        #region Special
                        dtNextRun = ComputeForNextRunDailySpecial(dr, dtNow, time);

                        #endregion
                    }
                    else
                        if (dr["Meh_TimeSetting"].ToString() == "4") //Other
                        {
                            #region Other
                            dtNextRun = ComputeForNextRunMinute(dr, dtNow, time);

                            #endregion
                        }

            #region update
            ExecuteNonQuery3(
            string.Format(
            @"
                    update M_EmailServiceHdr
                    set 
                            Meh_LatestRun = '{0}'
                            ,Meh_NextRun = '{1}'
                            ,Usr_Login = '{3}'
                            ,Ludatetime = GETDATE()
                    where Meh_ServiceCode = '{2}'
                ", dr["Meh_NextRun"].ToString()
            , dtNextRun.ToString("MM/dd/yyyy HH:mm")
            , dr["Meh_ServiceCode"].ToString()
            , user)
            );
            #endregion
        }

        public DateTime ComputeForNextRunMinute(DataRow dr, DateTime dtNow, string HHmm)
        {
            int interval = Convert.ToInt32(dr["Meh_Interval"]);
            dtNow = dtNow.AddMinutes(interval);
            dtNow = Convert.ToDateTime(dtNow.ToString("MM/dd/yyyy HH:mm"));

            return dtNow;
        }

        public DateTime ComputeForNextRunDaily(DataRow dr, DateTime dtNow, string HHmm)
        {
            dtNow = dtNow.AddDays(1);
            dtNow = Convert.ToDateTime(dtNow.ToString("MM/dd/yyyy") + " " + HHmm.Substring(0, 2) + ":" + HHmm.Substring(2, 2));

            return dtNow;
        }

        public DateTime ComputeForNextRunDailyDay(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtRet = DateTime.Now;
            dtRet = Convert.ToDateTime(dtNow.ToString("MM/dd/yyyy") + " " + HHmm.Substring(0, 2) + ":" + HHmm.Substring(2, 2));
            return dtRet;
        }

        public DateTime ComputeForNextRunDailySpecial(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtRet = DateTime.Now;
            DataSet ds = ExecuteDataSetQuery3(
            string.Format(
            @"
                    select 
                        Meh_SpecialInterval
                        ,Meh_SpecialCondition
                    from M_EmailServiceHdr
                    where Meh_ServiceCode = '{0}'
                ", dr["Meh_ServiceCode"].ToString())
            );
            DataSet ds2 = ExecuteDataSetQuery2(
            ds.Tables[0].Rows[0]["Meh_SpecialCondition"].ToString(), dr["Meh_ServiceCode"].ToString()
            );
            if (ds2.Tables[0].Rows.Count == 1)
            {
                dtRet = ComputeForNextRunDaily(dr, dtNow, HHmm);
            }
            else
            {
                int interval = Convert.ToInt32(ds.Tables[0].Rows[0]["Meh_SpecialInterval"]);
                int idx = 0;
                bool flag = true;
                for (; idx < ds2.Tables[0].Rows.Count && flag; )
                {
                    int time1 = GetTimeToMinutes(HHmm, string.Empty);
                    int time2 = GetTimeToMinutes(ds2.Tables[0].Rows[idx][0].ToString(), string.Empty);
                    if ((time2 + interval) == time1)
                    {
                        flag = false;
                    }
                    else
                        idx++;
                }
                if (idx + 1 >= ds2.Tables[0].Rows.Count)
                {
                    //Means that has gone to all scheduled time in the query and will go to the start with a different week
                    idx = 0;
                    dtRet = ComputeForNextRunDaily(dr, dtNow, GetTimeToHour(GetTimeToMinutes(ds2.Tables[0].Rows[0][0].ToString(), interval.ToString()), 0));
                }
                else
                {
                    //Means there is still remaining scheduled time
                    dtRet = ComputeForNextRunDailyDay(dr, dtNow, GetTimeToHour(GetTimeToMinutes(ds2.Tables[0].Rows[idx + 1][0].ToString(), interval.ToString()), 0));
                }
            }
            return dtRet;
        }

        #endregion

        #region Update Next run Weekly

        public void UpdateServiceHeaderWeekly(DataRow dr, DateTime dtNow, string user)
        {
            DateTime dtNextRun = DateTime.Now;
            string time = dr["StartTime"].ToString();

            if (dr["Meh_TimeSetting"].ToString() == "1") //Multi
            {
                #region Multi
                #region Query
                DataSet dstemp = ExecuteDataSetQuery3(string.Format(
                @"SELECT [Mst_0000]
                          ,[Mst_0015]
                          ,[Mst_0030]
                          ,[Mst_0045]
                          ,[Mst_0100]
                          ,[Mst_0115]
                          ,[Mst_0130]
                          ,[Mst_0145]
                          ,[Mst_0200]
                          ,[Mst_0215]
                          ,[Mst_0230]
                          ,[Mst_0245]
                          ,[Mst_0300]
                          ,[Mst_0315]
                          ,[Mst_0330]
                          ,[Mst_0345]
                          ,[Mst_0400]
                          ,[Mst_0415]
                          ,[Mst_0430]
                          ,[Mst_0445]
                          ,[Mst_0500]
                          ,[Mst_0515]
                          ,[Mst_0530]
                          ,[Mst_0545]
                          ,[Mst_0600]
                          ,[Mst_0615]
                          ,[Mst_0630]
                          ,[Mst_0645]
                          ,[Mst_0700]
                          ,[Mst_0715]
                          ,[Mst_0730]
                          ,[Mst_0745]
                          ,[Mst_0800]
                          ,[Mst_0815]
                          ,[Mst_0830]
                          ,[Mst_0845]
                          ,[Mst_0900]
                          ,[Mst_0915]
                          ,[Mst_0930]
                          ,[Mst_0945]
                          ,[Mst_1000]
                          ,[Mst_1015]
                          ,[Mst_1030]
                          ,[Mst_1045]
                          ,[Mst_1100]
                          ,[Mst_1115]
                          ,[Mst_1130]
                          ,[Mst_1145]
                          ,[Mst_1200]
                          ,[Mst_1215]
                          ,[Mst_1230]
                          ,[Mst_1245]
                          ,[Mst_1300]
                          ,[Mst_1315]
                          ,[Mst_1330]
                          ,[Mst_1345]
                          ,[Mst_1400]
                          ,[Mst_1415]
                          ,[Mst_1430]
                          ,[Mst_1445]
                          ,[Mst_1500]
                          ,[Mst_1515]
                          ,[Mst_1530]
                          ,[Mst_1545]
                          ,[Mst_1600]
                          ,[Mst_1615]
                          ,[Mst_1630]
                          ,[Mst_1645]
                          ,[Mst_1700]
                          ,[Mst_1715]
                          ,[Mst_1730]
                          ,[Mst_1745]
                          ,[Mst_1800]
                          ,[Mst_1815]
                          ,[Mst_1830]
                          ,[Mst_1845]
                          ,[Mst_1900]
                          ,[Mst_1915]
                          ,[Mst_1930]
                          ,[Mst_1945]
                          ,[Mst_2000]
                          ,[Mst_2015]
                          ,[Mst_2030]
                          ,[Mst_2045]
                          ,[Mst_2100]
                          ,[Mst_2115]
                          ,[Mst_2130]
                          ,[Mst_2145]
                          ,[Mst_2200]
                          ,[Mst_2215]
                          ,[Mst_2230]
                          ,[Mst_2245]
                          ,[Mst_2300]
                          ,[Mst_2315]
                          ,[Mst_2330]
                          ,[Mst_2345]
                      FROM M_ServiceTiming
                            where Mst_ServiceCode = '{0}'", dr["Meh_ServiceCode"].ToString())
                );
                #endregion

                int trail = -1;
                int flag = -1;
                int week = 0;
                for (int idx = 0; idx != trail && flag == -1; idx++)
                {
                    if (idx == dstemp.Tables[0].Columns.Count)
                    {
                        idx = 0;
                        week = 7 * Convert.ToInt32(dr["Meh_Interval"]);
                    }
                    if (trail != -1)
                    {
                        if (Convert.ToBoolean(dstemp.Tables[0].Rows[0][idx]))
                        {
                            flag = idx;
                        }
                    }
                    string queryTime = dstemp.Tables[0].Columns[idx].Caption.ToString().Replace("Sst_", "");
                    if (queryTime == time)
                    {
                        trail = idx;
                    }
                }
                if (flag == -1)
                {
                    dtNextRun = ComputeForNextRunWeekly(dr, dtNow, time);
                }
                else
                {
                    dtNow = dtNow.AddDays(week);
                    dtNextRun = ComputeForNextRunWeeklyDaily(dr, dtNow, dstemp.Tables[0].Columns[flag].Caption.ToString().Replace("Sst_", ""));
                }

                #endregion
            }
            else
                if (dr["Meh_TimeSetting"].ToString() == "2") //Specific
                {
                    #region Specific
                    dtNextRun = ComputeForNextRunWeekly(dr, dtNow, time);

                    #endregion
                }
                else
                    if (dr["Meh_TimeSetting"].ToString() == "3") //Special
                    {
                        #region Special
                        dtNextRun = ComputeForNextRunWeeklySpecial(dr, dtNow, time);

                        #endregion
                    }
            #region Update
            ExecuteNonQuery3(
            string.Format(
            @"
                    update M_EmailServiceHdr
                    set 
                            Meh_LatestRun = '{0}'
                            ,Meh_NextRun = '{1}'
                            ,Usr_Login = '{3}'
                            ,Ludatetime = GETDATE()
                    where Meh_ServiceCode = '{2}'
                ", dr["Meh_NextRun"].ToString()
            , dtNextRun.ToString("MM/dd/yyyy HH:mm")
            , dr["Meh_ServiceCode"].ToString().Trim()
            , user)
            );
            #endregion
        }

        public DateTime ComputeForNextRunWeekly(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtReturn = DateTime.Now;

            DataSet ds = ExecuteDataSetQuery3(
            string.Format(
            @"
                     select
                        Meh_Monday
                        ,Meh_Tuesday
                        ,Meh_Wednesday
                        ,Meh_Thursday
                        ,Meh_Friday
                        ,Meh_Saturday
                        ,Meh_Sunday
                    from M_EmailServiceHdr
                    where Meh_ServiceCode = '{0}'
                    ", dr["Meh_ServiceCode"].ToString()));
            DateTime dt = dtNow;
            int days = 0;
            bool isDay = false;
            for (int idx = 0; idx < 7 && !isDay; idx++)
            {
                #region loop
                dt = dt.AddDays(1);
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    days = 1;
                }
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Monday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Tuesday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Tuesday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Wednesday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Wednesday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Thursday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Thursday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Friday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Friday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Saturday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Saturday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Sunday:
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Meh_Sunday"]))
                            isDay = true;
                        break;
                }
                #endregion
            }
            if (dt.DayOfWeek == dtNow.DayOfWeek)
            {
                int interval = Convert.ToInt32(dr["Meh_Interval"]);
                dt = dtNow.AddDays(interval * 7);
            }
            else
                if (days == 1)
                {
                    int interval = Convert.ToInt32(dr["Meh_Interval"]);
                    dt = dt.AddDays(interval * 7);
                }
            dtReturn = Convert.ToDateTime(dt.ToString("MM/dd/yyyy") + " " + HHmm.Substring(0, 2) + ":" + HHmm.Substring(2, 2));

            return dtReturn;
        }

        public DateTime ComputeForNextRunWeeklyDaily(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtRet = DateTime.Now;
            dtRet = Convert.ToDateTime(dtNow.ToString("MM/dd/yyyy") + " " + HHmm.Substring(0, 2) + ":" + HHmm.Substring(2, 2));
            return dtRet;
        }

        public DateTime ComputeForNextRunWeeklySpecial(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtRet = DateTime.Now;
            DataSet ds = ExecuteDataSetQuery3(
            string.Format(
            @"
                    select 
                        Meh_SpecialInterval
                        ,Meh_SpecialCondition
                    from M_EmailServiceHdr
                    where Meh_ServiceCode = '{0}'
                ", dr["Meh_ServiceCode"].ToString())
            );
            DataSet ds2 = ExecuteDataSetQuery2(
            ds.Tables[0].Rows[0]["Meh_SpecialCondition"].ToString(), dr["Meh_ServiceCode"].ToString()
            );
            if (ds2.Tables[0].Rows.Count == 1)
            {
                dtRet = ComputeForNextRunWeekly(dr, dtNow, HHmm);
            }
            else
            {
                int interval = Convert.ToInt32(ds.Tables[0].Rows[0]["Meh_SpecialInterval"]);
                int idx = 0;
                bool flag = true;
                for (; idx < ds2.Tables[0].Rows.Count && flag; )
                {
                    int time1 = GetTimeToMinutes(HHmm, string.Empty);
                    int time2 = GetTimeToMinutes(ds2.Tables[0].Rows[idx][0].ToString(), string.Empty);
                    if ((time2 + interval) == time1)
                    {
                        flag = false;
                    }
                    else
                        idx++;
                }
                if (idx + 1 >= ds2.Tables[0].Rows.Count)
                {
                    //Means that has gone to all scheduled time in the query and will go to the start with a different week
                    idx = 0;
                    dtRet = ComputeForNextRunWeekly(dr, dtNow, GetTimeToHour(GetTimeToMinutes(ds2.Tables[0].Rows[0][0].ToString(), interval.ToString()), 0));
                }
                else
                {
                    //Means there is still remaining scheduled time
                    dtRet = ComputeForNextRunWeeklyDaily(dr, dtNow, GetTimeToHour(GetTimeToMinutes(ds2.Tables[0].Rows[idx + 1][0].ToString(), interval.ToString()), 0));
                }
            }
            return dtRet;
        }

        #endregion

        #region Update Next run Monthly

        public void UpdateServiceHeaderMonthly(DataRow dr, DateTime dtNow, string user)
        {
            DateTime dtNextRun = DateTime.Now;
            string time = dr["StartTime"].ToString();

            if (dr["Meh_TimeSetting"].ToString() == "1") //Multi
            {
                #region Multi
                #region Query
                DataSet dstemp = ExecuteDataSetQuery3(string.Format(
                @"SELECT [Mst_0000]
                          ,[Mst_0015]
                          ,[Mst_0030]
                          ,[Mst_0045]
                          ,[Mst_0100]
                          ,[Mst_0115]
                          ,[Mst_0130]
                          ,[Mst_0145]
                          ,[Mst_0200]
                          ,[Mst_0215]
                          ,[Mst_0230]
                          ,[Mst_0245]
                          ,[Mst_0300]
                          ,[Mst_0315]
                          ,[Mst_0330]
                          ,[Mst_0345]
                          ,[Mst_0400]
                          ,[Mst_0415]
                          ,[Mst_0430]
                          ,[Mst_0445]
                          ,[Mst_0500]
                          ,[Mst_0515]
                          ,[Mst_0530]
                          ,[Mst_0545]
                          ,[Mst_0600]
                          ,[Mst_0615]
                          ,[Mst_0630]
                          ,[Mst_0645]
                          ,[Mst_0700]
                          ,[Mst_0715]
                          ,[Mst_0730]
                          ,[Mst_0745]
                          ,[Mst_0800]
                          ,[Mst_0815]
                          ,[Mst_0830]
                          ,[Mst_0845]
                          ,[Mst_0900]
                          ,[Mst_0915]
                          ,[Mst_0930]
                          ,[Mst_0945]
                          ,[Mst_1000]
                          ,[Mst_1015]
                          ,[Mst_1030]
                          ,[Mst_1045]
                          ,[Mst_1100]
                          ,[Mst_1115]
                          ,[Mst_1130]
                          ,[Mst_1145]
                          ,[Mst_1200]
                          ,[Mst_1215]
                          ,[Mst_1230]
                          ,[Mst_1245]
                          ,[Mst_1300]
                          ,[Mst_1315]
                          ,[Mst_1330]
                          ,[Mst_1345]
                          ,[Mst_1400]
                          ,[Mst_1415]
                          ,[Mst_1430]
                          ,[Mst_1445]
                          ,[Mst_1500]
                          ,[Mst_1515]
                          ,[Mst_1530]
                          ,[Mst_1545]
                          ,[Mst_1600]
                          ,[Mst_1615]
                          ,[Mst_1630]
                          ,[Mst_1645]
                          ,[Mst_1700]
                          ,[Mst_1715]
                          ,[Mst_1730]
                          ,[Mst_1745]
                          ,[Mst_1800]
                          ,[Mst_1815]
                          ,[Mst_1830]
                          ,[Mst_1845]
                          ,[Mst_1900]
                          ,[Mst_1915]
                          ,[Mst_1930]
                          ,[Mst_1945]
                          ,[Mst_2000]
                          ,[Mst_2015]
                          ,[Mst_2030]
                          ,[Mst_2045]
                          ,[Mst_2100]
                          ,[Mst_2115]
                          ,[Mst_2130]
                          ,[Mst_2145]
                          ,[Mst_2200]
                          ,[Mst_2215]
                          ,[Mst_2230]
                          ,[Mst_2245]
                          ,[Mst_2300]
                          ,[Mst_2315]
                          ,[Mst_2330]
                          ,[Mst_2345]
                      FROM M_ServiceTiming
                            where Mst_ServiceCode = '{0}'", dr["Meh_ServiceCode"].ToString())
                );
                #endregion

                int trail = -1;
                int flag = -1;
                int months = 0;
                for (int idx = 0; idx != trail && flag == -1; idx++)
                {
                    if (idx == dstemp.Tables[0].Columns.Count)
                    {
                        idx = 0;
                        months = Convert.ToInt32(dr["Meh_Interval"]);
                    }
                    if (trail != -1)
                    {
                        if (Convert.ToBoolean(dstemp.Tables[0].Rows[0][idx]))
                        {
                            flag = idx;
                        }
                    }
                    string queryTime = dstemp.Tables[0].Columns[idx].Caption.ToString().Replace("Sst_", "");
                    if (queryTime == time)
                    {
                        trail = idx;
                    }
                }
                if (flag == -1)
                {
                    dtNextRun = ComputeForNextRunMonthly(dr, dtNow, time);
                }
                else
                {
                    dtNow = dtNow.AddMonths(months);
                    dtNextRun = ComputeForNextRunMonthlyDaily(dr, dtNow, dstemp.Tables[0].Columns[flag].Caption.ToString().Replace("Sst_", ""));
                }

                #endregion
            }
            else
                if (dr["Meh_TimeSetting"].ToString() == "2") //Specific
                {
                    #region Specific
                    dtNextRun = ComputeForNextRunMonthly(dr, dtNow, time);

                    #endregion
                }
                else
                    if (dr["Meh_TimeSetting"].ToString() == "3") //Special
                    {
                        #region Special
                        dtNextRun = ComputeForNextRunMonthlySpecial(dr, dtNow, time);

                        #endregion
                    }
            #region Update
            ExecuteNonQuery3(string.Format(
            @"
                    update M_EmailServiceHdr
                    set 
                            Meh_LatestRun = '{0}'
                            ,Meh_NextRun = '{1}'
                            ,Usr_Login = '{3}'
                            ,Ludatetime = GETDATE()
                    where Meh_ServiceCode = '{2}'
                ", dr["Meh_NextRun"].ToString()
            , dtNextRun.ToString("MM/dd/yyyy HH:mm")
            , dr["Meh_ServiceCode"].ToString().Trim()
            , user)
            );
            #endregion
        }

        public DateTime ComputeForNextRunMonthly(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtReturn = DateTime.Now;

            int interval = Convert.ToInt32(dr["Meh_Interval"]);

            dtNow = dtNow.AddMonths(interval);
            if (dr["Meh_MonthDay"].ToString() != "0")
            {
                int monthday = Convert.ToInt32(dr["Meh_MonthDay"]);
                if (monthday > DateTime.DaysInMonth(dtNow.Year, dtNow.Month))
                {
                    monthday = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                }
                dtReturn = new DateTime(dtNow.Year, dtNow.Month, monthday);
            }
            else
            {
                switch (dr["Meh_DayOfWeek"].ToString())
                {
                    case "DY":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 1);
                                break;
                            case "2":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 2);
                                break;
                            case "3":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 3);
                                break;
                            case "4":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 4);
                                break;
                            case "L":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, DateTime.DaysInMonth(dtNow.Year, dtNow.Month));
                                break;
                        }
                        #endregion
                        break;
                    case "WD":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonth(dtNow, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonth(dtNow, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonth(dtNow, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonth(dtNow, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthRevers(dtNow);
                                break;
                        }
                        #endregion
                        break;
                    case "WK":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekendRevers(dtNow);
                                break;
                        }
                        #endregion
                        break;
                    case "MO":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Monday);
                                break;
                        }
                        #endregion
                        break;
                    case "TU":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Tuesday);
                                break;
                        }
                        #endregion
                        break;
                    case "WE":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Wednesday);
                                break;
                        }
                        #endregion
                        break;
                    case "TH":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Thursday);
                                break;
                        }
                        #endregion
                        break;
                    case "FR":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Friday);
                                break;
                        }
                        #endregion
                        break;
                    case "SA":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Saturday);
                                break;
                        }
                        #endregion
                        break;
                    case "SU":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Sunday);
                                break;
                        }
                        #endregion
                        break;
                }
            }
            dtReturn = Convert.ToDateTime(dtReturn.ToString("MM/dd/yyyy") + " " + HHmm.Substring(0, 2) + ":" + HHmm.Substring(2, 2));
            return dtReturn;
        }

        public DateTime ComputeForNextRunMonthlyDaily(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtRet = DateTime.Now;
            dtRet = Convert.ToDateTime(dtNow.ToString("MM/dd/yyyy") + " " + HHmm.Substring(0, 2) + ":" + HHmm.Substring(2, 2));
            return dtRet;
        }

        public DateTime ComputeForNextRunMonthlySpecial(DataRow dr, DateTime dtNow, string HHmm)
        {
            DateTime dtRet = DateTime.Now;

            DataSet ds2 = ExecuteDataSetQuery2(
            dr["Meh_SpecialCondition"].ToString(), dr["Meh_ServiceCode"].ToString()
            );
            if (ds2.Tables[0].Rows.Count == 1)
            {
                dtRet = ComputeForNextRunMonthly(dr, dtNow, HHmm);
            }
            else
            {
                int interval = Convert.ToInt32(dr["Meh_SpecialInterval"]);
                int idx = 0;
                bool flag = true;
                for (; idx < ds2.Tables[0].Rows.Count && flag; )
                {
                    int time1 = GetTimeToMinutes(HHmm, string.Empty);
                    int time2 = GetTimeToMinutes(ds2.Tables[0].Rows[idx][0].ToString(), string.Empty);
                    if ((time2 + interval) == time1)
                    {
                        flag = false;
                    }
                    else
                        idx++;
                }
                if (idx + 1 >= ds2.Tables[0].Rows.Count)
                {
                    //Means that has gone to all scheduled time in the query and will go to the start with a different week
                    idx = 0;
                    dtRet = ComputeForNextRunMonthly(dr, dtNow, GetTimeToHour(GetTimeToMinutes(ds2.Tables[0].Rows[0][0].ToString(), interval.ToString()), 0));
                }
                else
                {
                    //Means there is still remaining scheduled time
                    dtRet = ComputeForNextRunMonthlyDaily(dr, dtNow, GetTimeToHour(GetTimeToMinutes(ds2.Tables[0].Rows[idx + 1][0].ToString(), interval.ToString()), 0));
                }
            }
            return dtRet;
        }

        private DateTime LoopThroughMonth(DateTime dt, int interval)
        {
            DateTime dtNow = dt;
            dt = new DateTime(dt.Year, dt.Month, 1);
            int ctr = 0;
            bool flag = true;
            for (int idx = 0; idx < DateTime.DaysInMonth(dt.Year, dt.Month) && flag; idx++)
            {
                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    ctr++;
                    if (ctr == interval)
                    {
                        dtNow = dt;
                        flag = false;
                    }
                }
                dt = dt.AddDays(1);
            }

            return dtNow;
        }

        private DateTime LoopThroughMonthRevers(DateTime dt)
        {
            bool flag = true;
            DateTime dtNow = dt;
            dt = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            for (int idx = dt.Day; idx > 0 && flag; idx--)
            {
                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    dtNow = dt;
                    flag = false;
                }
                dt = dt.AddDays(-1);
            }
            return dtNow;
        }

        private DateTime LoopThroughMonthWeekend(DateTime dt, int interval)
        {
            DateTime dtNow = dt;
            dt = new DateTime(dt.Year, dt.Month, 1);
            int ctr = 0;
            bool flag = true;
            for (int idx = 1; idx <= DateTime.DaysInMonth(dt.Year, dt.Month) && flag; idx++)
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    ctr++;
                    if (ctr == interval)
                    {
                        dtNow = dt;
                        flag = false;
                    }
                }
                dt = dt.AddDays(1);
            }
            return dtNow;
        }

        private DateTime LoopThroughMonthWeekendRevers(DateTime dt)
        {
            bool flag = true;
            DateTime dtNow = dt;
            dt = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            for (int idx = dt.Day; idx > 0 && flag; idx--)
            {
                if (dt.ToString("MM/dd/yyyy") == dtNow.ToString("MM/dd/yyyy"))
                {
                    if (dtNow.DayOfWeek == DayOfWeek.Saturday || dtNow.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dtNow = dt;
                        flag = false;
                    }
                }
                dt = dt.AddDays(-1);
            }
            return dtNow;
        }

        private DateTime LoopThroughMonthWeekdays(DateTime dt, DayOfWeek day, int interval)
        {
            DateTime dtNow = dt;
            dt = new DateTime(dt.Year, dt.Month, 1);
            int ctr = 0;
            bool flag = true;
            for (int idx = 1; idx <= DateTime.DaysInMonth(dt.Year, dt.Month) && flag; idx++)
            {
                if (dt.DayOfWeek == day)
                {
                    ctr++;
                    if (ctr == interval)
                    {
                        dtNow = dt;
                        flag = false;
                    }
                }
                dt = dt.AddDays(1);
            }
            return dtNow;
        }

        private DateTime LoopThroughMonthWeekdaysRevers(DateTime dt, DayOfWeek day)
        {
            DateTime dtNow = dt;
            bool flag = true;
            dt = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            for (int idx = DateTime.DaysInMonth(dt.Year, dt.Month); idx > 0 && flag; idx--)
            {
                if (dt.DayOfWeek == day)
                {
                    dtNow = dt;
                    flag = false;
                }
                dt = dt.AddDays(-1);
            }
            return dtNow;
        }

        #endregion

        #endregion

        #region Forced Run

        public void UpdateServiceForceRun(DataRow dr, DateTime dtNow, string user)
        {
            ExecuteNonQuery3(
            string.Format(
            @"
                    update M_EmailServiceHdr
                    set 
                        Meh_LatestRun = '{0}'
                        ,Meh_ForceRun = '{0}'
                        ,usr_Login = '{1}'
                        ,Ludatetime = GETDATE()
                    where Meh_ServiceCode = '{2}'
                ", dtNow.ToString("MM/dd/yyyy HH:mm")
            , user
            , dr["Meh_ServiceCode"].ToString()
            ));
            dr["Meh_NextRun"] = dtNow.ToString("MM/dd/yyyy HH:mm");
            dr["StartTime"] = dtNow.ToString("HHmm");
            UpdateServiceWithClosestRunTime(dr, dtNow, user);
        }

        public void UpdateServiceWithClosestRunTime(DataRow dr, DateTime dtNow, string user)
        {
            if (dr["Meh_ScheduleType"].ToString().Trim() == "D")
            {
                string time = UpdateDailyWithClosestRuntime(dr, dtNow, user);
                dtNow = dtNow.AddDays(1);
                ExecuteDataSetQuery3(
                string.Format(
                @"
                        update M_EmailServiceHdr
                        set
                            Meh_NextRun = '{0}'
                            ,usr_login = '{1}'
                            ,Ludatetime = GETDATE()
                        where Meh_ServiceCode = '{2}'
                    ", dtNow.ToString("MM/dd/yyyy") + " " + time.Substring(0, 2) + ":" + time.Substring(2, 2)
                , user
                , dr["Meh_ServiceCode"].ToString())
                );
            }
            else
                if (dr["Meh_ScheduleType"].ToString().Trim() == "W")
                {
                    UpdateWeeklyWithClosestRuntime(dr, dtNow, user);
                }
                else
                    if (dr["Meh_ScheduleType"].ToString().Trim() == "M")
                    {
                        UpdateMonthlyWithClosestRuntime(dr, dtNow, user);
                    }
        }

        public string UpdateDailyWithClosestRuntime(DataRow dr, DateTime dtNow, string user)
        {
            string time = string.Empty;
            if (dr["Meh_TimeSetting"].ToString().Trim() == "1")
            {
                #region Multi
                #region Query
                DataSet ds = ExecuteDataSetQuery3(
                string.Format(
                @"
                    SELECT [Mst_0000]
                          ,[Mst_0015]
                          ,[Mst_0030]
                          ,[Mst_0045]
                          ,[Mst_0100]
                          ,[Mst_0115]
                          ,[Mst_0130]
                          ,[Mst_0145]
                          ,[Mst_0200]
                          ,[Mst_0215]
                          ,[Mst_0230]
                          ,[Mst_0245]
                          ,[Mst_0300]
                          ,[Mst_0315]
                          ,[Mst_0330]
                          ,[Mst_0345]
                          ,[Mst_0400]
                          ,[Mst_0415]
                          ,[Mst_0430]
                          ,[Mst_0445]
                          ,[Mst_0500]
                          ,[Mst_0515]
                          ,[Mst_0530]
                          ,[Mst_0545]
                          ,[Mst_0600]
                          ,[Mst_0615]
                          ,[Mst_0630]
                          ,[Mst_0645]
                          ,[Mst_0700]
                          ,[Mst_0715]
                          ,[Mst_0730]
                          ,[Mst_0745]
                          ,[Mst_0800]
                          ,[Mst_0815]
                          ,[Mst_0830]
                          ,[Mst_0845]
                          ,[Mst_0900]
                          ,[Mst_0915]
                          ,[Mst_0930]
                          ,[Mst_0945]
                          ,[Mst_1000]
                          ,[Mst_1015]
                          ,[Mst_1030]
                          ,[Mst_1045]
                          ,[Mst_1100]
                          ,[Mst_1115]
                          ,[Mst_1130]
                          ,[Mst_1145]
                          ,[Mst_1200]
                          ,[Mst_1215]
                          ,[Mst_1230]
                          ,[Mst_1245]
                          ,[Mst_1300]
                          ,[Mst_1315]
                          ,[Mst_1330]
                          ,[Mst_1345]
                          ,[Mst_1400]
                          ,[Mst_1415]
                          ,[Mst_1430]
                          ,[Mst_1445]
                          ,[Mst_1500]
                          ,[Mst_1515]
                          ,[Mst_1530]
                          ,[Mst_1545]
                          ,[Mst_1600]
                          ,[Mst_1615]
                          ,[Mst_1630]
                          ,[Mst_1645]
                          ,[Mst_1700]
                          ,[Mst_1715]
                          ,[Mst_1730]
                          ,[Mst_1745]
                          ,[Mst_1800]
                          ,[Mst_1815]
                          ,[Mst_1830]
                          ,[Mst_1845]
                          ,[Mst_1900]
                          ,[Mst_1915]
                          ,[Mst_1930]
                          ,[Mst_1945]
                          ,[Mst_2000]
                          ,[Mst_2015]
                          ,[Mst_2030]
                          ,[Mst_2045]
                          ,[Mst_2100]
                          ,[Mst_2115]
                          ,[Mst_2130]
                          ,[Mst_2145]
                          ,[Mst_2200]
                          ,[Mst_2215]
                          ,[Mst_2230]
                          ,[Mst_2245]
                          ,[Mst_2300]
                          ,[Mst_2315]
                          ,[Mst_2330]
                          ,[Mst_2345]
                      FROM M_ServiceTiming
                            where Mst_ServiceCode = '{0}'
                    "));
                #endregion

                if (ds.Tables[0].Rows.Count > 0)
                {
                    bool flag = true;
                    for (int idx = 0; idx < ds.Tables[0].Columns.Count && flag; idx++)
                    {
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0][idx]))
                        {
                            flag = false;
                            time = ds.Tables[0].Columns[idx].Caption.ToString().Trim().Replace("Mst_", "");
                        }
                    }
                }
                #endregion
            }
            else
                if (dr["Meh_TimeSetting"].ToString().Trim() == "2")
                {
                    #region Specific
                    time = dr["Meh_Runtime"].ToString();
                    #endregion
                }
                else
                    if (dr["Meh_TimeSetting"].ToString().Trim() == "3")
                    {
                        #region Special
                        DataSet ds2 = ExecuteDataSetQuery3(
                        dr["Meh_SpecialCondition"].ToString()
                        );
                        if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                            time = GetTimeToHour(
                            GetTimeToMinutes(ds2.Tables[0].Rows[0][0].ToString(), string.Empty)
                            + (Convert.ToInt32(dr["Meh_SpecialInterval"].ToString()))
                            , 0);

                        #endregion
                    }
                    else
                        if (dr["Meh_TimeSetting"].ToString().Trim() == "4")
                        {
                            #region Other
                            time = dtNow.AddMinutes(Convert.ToInt32(dr["Meh_Interval"])).ToString("HHmm");
                            #endregion
                        }
            return time;
        }

        public void UpdateWeeklyWithClosestRuntime(DataRow dr, DateTime dtNow, string user)
        {
            bool isDay = false;
            for (int idx = 0; idx < 7 && !isDay; idx++)
            {
                dtNow = dtNow.AddDays(1);
                switch (dtNow.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (Convert.ToBoolean(dr["Meh_Monday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Tuesday:
                        if (Convert.ToBoolean(dr["Meh_Tuesday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Wednesday:
                        if (Convert.ToBoolean(dr["Meh_Wednesday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Thursday:
                        if (Convert.ToBoolean(dr["Meh_Thursday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Friday:
                        if (Convert.ToBoolean(dr["Meh_Friday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Saturday:
                        if (Convert.ToBoolean(dr["Meh_Saturday"]))
                            isDay = true;
                        break;
                    case DayOfWeek.Sunday:
                        if (Convert.ToBoolean(dr["Meh_Sunday"]))
                            isDay = true;
                        break;
                }
            }
            int interval = Convert.ToInt32(dr["Meh_Interval"]);
            string time = UpdateDailyWithClosestRuntime(dr, dtNow, user);
            ExecuteDataSetQuery3(
            string.Format(
            @"
                        update M_EmailServiceHdr
                        set
                            Meh_NextRun = '{0}'
                            ,usr_login = '{1}'
                            ,Ludatetime = GETDATE()
                        where Meh_ServiceCode = '{2}'
                    ", dtNow.ToString("MM/dd/yyyy") + " " + time.Substring(0, 2) + ":" + time.Substring(2, 2)
            , user
            , dr["Meh_ServiceCode"].ToString())
            );
        }

        public void UpdateMonthlyWithClosestRuntime(DataRow dr, DateTime dtNow, string user)
        {
            int interval = Convert.ToInt32(dr["Meh_Interval"]);
            dtNow = dtNow.AddMonths(interval);
            DateTime dtReturn = dtNow;
            if (dr["Meh_MonthDay"] != null && dr["Meh_MonthDay"].ToString().Trim() != "0")
            {
                int monthday = Convert.ToInt32(dr["Meh_MonthDay"]);
                if (monthday > DateTime.DaysInMonth(dtReturn.Year, dtReturn.Month))
                    monthday = DateTime.DaysInMonth(dtReturn.Year, dtReturn.Month);
                dtReturn = new DateTime(dtReturn.Year, dtReturn.Month, monthday);
            }
            else
            {
                switch (dr["Meh_DayOfWeek"].ToString())
                {
                    case "DY":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 1);
                                break;
                            case "2":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 2);
                                break;
                            case "3":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 3);
                                break;
                            case "4":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, 4);
                                break;
                            case "L":
                                dtReturn = new DateTime(dtNow.Year, dtNow.Month, DateTime.DaysInMonth(dtNow.Year, dtNow.Month));
                                break;
                        }
                        #endregion
                        break;
                    case "WD":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonth(dtNow, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonth(dtNow, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonth(dtNow, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonth(dtNow, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthRevers(dtNow);
                                break;
                        }
                        #endregion
                        break;
                    case "WK":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekend(dtNow, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekendRevers(dtNow);
                                break;
                        }
                        #endregion
                        break;
                    case "MO":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Monday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Monday);
                                break;
                        }
                        #endregion
                        break;
                    case "TU":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Tuesday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Tuesday);
                                break;
                        }
                        #endregion
                        break;
                    case "WE":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Wednesday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Wednesday);
                                break;
                        }
                        #endregion
                        break;
                    case "TH":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Thursday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Thursday);
                                break;
                        }
                        #endregion
                        break;
                    case "FR":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Friday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Friday);
                                break;
                        }
                        #endregion
                        break;
                    case "SA":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Saturday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Saturday);
                                break;
                        }
                        #endregion
                        break;
                    case "SU":
                        #region
                        switch (dr["Meh_NthDay"].ToString())
                        {
                            case "1":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 1);
                                break;
                            case "2":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 2);
                                break;
                            case "3":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 3);
                                break;
                            case "4":
                                dtReturn = LoopThroughMonthWeekdays(dtNow, DayOfWeek.Sunday, 4);
                                break;
                            case "L":
                                dtReturn = LoopThroughMonthWeekdaysRevers(dtNow, DayOfWeek.Sunday);
                                break;
                        }
                        #endregion
                        break;
                }
            }
            string time = UpdateDailyWithClosestRuntime(dr, dtNow, user);
            ExecuteDataSetQuery3(
            string.Format(
            @"
                        update M_EmailServiceHdr
                        set
                            Meh_NextRun = '{0}'
                            ,usr_login = '{1}'
                            ,Ludatetime = GETDATE()
                        where Meh_ServiceCode = '{2}'
                    ", dtNow.ToString("MM/dd/yyyy") + " " + time.Substring(0, 2) + ":" + time.Substring(2, 2)
            , user
            , dr["Meh_ServiceCode"].ToString())
            );
        }

        #endregion

        #region Queries Processes

        public DataSet ExecuteDataSetQuery2(string query, string txtServiceCode)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    DataSet dsprof = dal.ExecuteDataSet(
                    string.Format(@"
                                select Mpf_DatabaseName from M_EmailServiceProfile
                                left join M_Profile
                                on Mpf_DatabaseNo = Mep_DatabaseNo
                                where Mep_ServiceCode = '{0}'
                            ", txtServiceCode));
                    if (dsprof != null && dsprof.Tables[0].Rows.Count > 0)
                    {
                        for (int idx = 0; idx < dsprof.Tables[0].Rows.Count; idx++)
                        {
                            using (DALHelper dal2 = new DALHelper(dsprof.Tables[0].Rows[idx]["Prf_Database"].ToString(), false))
                            {
                                try
                                {
                                    dal2.OpenDB();
                                    DataSet dstemp = dal2.ExecuteDataSet(query, CommandType.Text);
                                    if (ds == null)
                                        ds = dstemp;
                                    else
                                        ds.Merge(dstemp);
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    dal2.CloseDB();
                                }
                            }
                        }
                        string col = ds.Tables[0].Columns[0].Caption;
                        DataView view = new DataView(ds.Tables[0]);

                        string[] Cols = new string[ds.Tables[0].Columns.Count];
                        for (int idx = 0; idx < ds.Tables[0].Columns.Count; idx++)
                        {
                            Cols[idx] = ds.Tables[0].Columns[idx].Caption;
                        }

                        DataTable dt = view.ToTable(true, Cols);
                        DataSet ds2 = new DataSet();
                        ds2.Tables.Add(dt);
                        ds = ds2;
                    }
                }
                catch (Exception er)
                {
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public bool ExecuteNonQuery2(string query, string Database)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper(Database, false))
            {
                try
                {
                    dal.OpenDB();
                    //dal.BeginTransaction();

                    dal.ExecuteNonQuery(query, CommandType.Text);

                    //dal.CommitTransaction();
                    ret = true;
                }
                catch (Exception er)
                {
                    //dal.RollBackTransaction();
                    ret = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public DataSet ExecuteDataSetQuery3(string query)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                }
                catch (Exception er)
                {
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public bool ExecuteNonQuery3(string query)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    //dal.BeginTransaction();

                    dal.ExecuteNonQuery(query, CommandType.Text);

                    //dal.CommitTransaction();
                    ret = true;
                }
                catch (Exception er)
                {
                    //dal.RollBackTransaction();
                    ret = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        #endregion
    }
}
