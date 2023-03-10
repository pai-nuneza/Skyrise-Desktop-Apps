using System;
using System.Data;
using System.Configuration;
using CommonPostingLibrary;
using System.Net.Mail;
using System.Net;
using Posting.DAL;
using Posting.BLogic;
using System.Windows.Forms;

namespace UploadDownloadSchedulerConsole
{
    class conUnpairedLogs
    {
        #region Email Parameters (Company Sepcific)
        CommonBL CommonBL = new CommonBL();
        private static string fromParameter = CommonProcedures.GetAppSettingConfigString("FROM", "", false); 
        private static string subjectParameter = "Unpaired Logs Notification";
        private static string urlParameter = ConfigurationManager.AppSettings["EWSSServer"].ToString();
        private static string smtpServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
        private static int smtpPort = 25;
        private static string smtpUsername = ConfigurationManager.AppSettings["SMTPUsername"].ToString();
        private static string smtpPassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString();
        private static string smtpCC = ConfigurationManager.AppSettings["CC"].ToString();
        private static string smtpBCC = ConfigurationManager.AppSettings["BC"].ToString();
        private static DataTable dtDBNames;
        private static string CentralProfile = string.Empty;
        private static int UnpairedLogsPastDays = int.Parse(ConfigurationManager.AppSettings["UnpairedLogsPastDays"].ToString());
        public static string POCKETSIZE = "";
        
        #endregion

        public conUnpairedLogs()
        {
            CentralProfile = CommonProcedures.GetAppSettingConfigString("CentralDBName", "", true);
            dtDBNames = GetProfileDetails();

            try
            {
                if (!Int32.TryParse(CommonProcedures.GetAppSettingConfigString("SMTPPort", "", false), out smtpPort))
                {
                    smtpPort = 25;
                }
            }
            catch
            {
                //no implementation SMTPort not in config
            }
        }

        public void runUnpairedLogsReport()
        {
            DataSet dsDetails = getUnpairedLogsMultiplePockets();
            if (dsDetails.Tables.Count > 0 && dsDetails.Tables[0].Rows.Count > 0)
                WFHelpers.WFHandlers.StandardUnpairedLogsNotificationSending(dsDetails);

            #region //Two Pockets
            //if (dsDetails.Tables.Count > 0 && dsDetails.Tables[0].Rows.Count > 0)
            //{
            //    string messageBody = string.Empty;

            //    string tempRecipientEmail = string.Empty;
            //    string tempRecipientName = string.Empty;
            //    string prevRecipientEmail = string.Empty;
            //    string prevRecipientName = string.Empty;
            //    string prevEmployeeName = string.Empty;

            //    for (int i = 0; i < dsDetails.Tables[0].Rows.Count; i++)
            //    {
            //        tempRecipientEmail = dsDetails.Tables[0].Rows[i]["Recipient Email"].ToString();
            //        tempRecipientName = dsDetails.Tables[0].Rows[i]["Recipient Name"].ToString();
            //        if (!prevRecipientEmail.Equals(tempRecipientEmail))
            //        {
            //            if (i > 0 && !prevRecipientEmail.Equals(string.Empty))
            //            {
            //                SendEmail(fromParameter
            //                         , prevRecipientEmail
            //                         , "Unpaired Logs Notification"
            //                         , messageBody + "</table><br>" + getFooter(true)
            //                         , true);
            //            }
            //            messageBody = "Hi " + tempRecipientName + @",<br><br>" + getGreeting() + @".<br><br>";


            //            messageBody += @"You have unpaired log(s) in the system.<br>Detail(s):<br>";
            //            messageBody += @"<table border=""1"">
            //                             <tr><th>Log Date</th>
            //                                 <th>Dow</th>
            //                                 <th>Day Code</th>
            //                                 <th>Time In 1</th>
            //                                 <th>Time Out 1</th>
            //                                 <th>Time In 2</th>
            //                                 <th>Time Out 2</th>
            //                                 <th>Remarks</th>
            //                            </tr>";
            //            prevRecipientEmail = tempRecipientEmail;
            //            prevRecipientName = tempRecipientName;
            //        }

            //        messageBody += "<tr>" + formatUnapiredLogsDetail(dsDetails.Tables[0].Rows[i]) + @"</tr>";
            //    }
            //    //Send last email
            //    if (!prevRecipientEmail.Equals(string.Empty))
            //    {
            //        SendEmail(fromParameter
            //                        , prevRecipientEmail
            //                        , "Unpaired Logs Notification"
            //                        , messageBody + "</table><br>" + getFooter(true)
            //                        , true);
            //    }
            //}
            #endregion
        }

        private string formatUnapiredLogsDetail(DataRow dr)
        {
            string detail = "<td>" + dr["Log Date"].ToString() + "</td>"
                   + "<td>" + dr["DoW"].ToString() + "</td>"
                   + "<td>" + dr["Day Code"].ToString() + "</td>"
                   + "<td>" + dr["Time In 1"] + "</td>"
                   + "<td>" + dr["Time Out 1"] + "</td>"
                   + "<td>" + dr["Time In 2"] + "</td>"
                   + "<td>" + dr["Time Out 2"] + "</td>"
                   + "<td>" + dr["Remarks"] + "</td>";

            return detail;
        }

        private DataSet getUnpairedLogs()
        {
                DataSet dsDetails = new DataSet();
                #region Fetch Details

                string sql = @"SELECT  {0}..T_EmpTimeRegister.Ttr_IDNo as [IDNumber]
		    , Mem_OfficeEmailAddress as [Recipient Email]
		    , {1}.dbo.Udf_DisplayName(Ttr_IDNo,'{4}') as [Recipient Name]
		    , CONVERT(char(10), Ttr_Date,101) as [Log Date]
		    , LEFT(DATENAME(dw, Ttr_Date),3) as [DoW]
		    , Ttr_DayCode as [Day Code]
		    , {1}.dbo.Udf_DisplayShift('{3}',Ttr_ShiftCode) as [Shift Code]
		    , CASE WHEN Ttr_ActIn_1 = '0000' then '' ELSE LEFT(Ttr_ActIn_1,2) + ':' + RIGHT(Ttr_ActIn_1, 2) END as [Time In 1]
		    , CASE WHEN Ttr_ActOut_1 = '0000' then '' ELSE LEFT(Ttr_ActOut_1,2) +':' + RIGHT(Ttr_ActOut_1, 2) END as [Time Out 1]
		    , CASE WHEN Ttr_ActIn_2 = '0000' then '' ELSE LEFT(Ttr_ActIn_2, 2) + ':' + RIGHT(Ttr_ActIn_2, 2) END as [Time In 2]
		    , CASE WHEN Ttr_ActOut_2 = '0000' then '' ELSE LEFT(Ttr_ActOut_2, 2) + ':' + RIGHT(Ttr_ActOut_2, 2) END as [Time Out 2]
		    , CASE WHEN (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') THEN 'No In'
			WHEN (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') THEN 'No In 1'
			WHEN (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000')  THEN 'No In 1'
			WHEN (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000')  THEN 'No In 1 & In 2'
			WHEN (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  THEN 'No In 1 and Out 2'
			WHEN (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') THEN 'No In 2'
			WHEN (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000')  THEN 'No Out'
			WHEN (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') THEN 'No Out 1'
			WHEN (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  THEN 'No Out 1 and Out 2'
			WHEN (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  THEN 'No Out 2'
			WHEN (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000') THEN 'No Out 2'
			ELSE '' END as [Remarks]
            FROM {0}..T_EmpTimeRegister
            INNER JOIN {0}..M_Employee on Mem_IDNo = T_EmpTimeRegister.Ttr_IDNo
            INNER JOIN {1}..M_Shift on Msh_ShiftCode = Ttr_ShiftCode
                AND Msh_CompanyCode = '{3}'
            xxxxx
            WHERE  ((Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') or
	            (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') or
	            (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000') or 
	            (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') or
	            (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000') or 
	            (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') or
	            (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000') or 
	            (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') or
	            (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000') or 
	            (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000') or 
	            (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000'))
	            AND CONVERT(CHAR(10),Ttr_Date,101) <= CONVERT(CHAR(10),DATEADD(day, {2}, GETDATE()),101)
	            AND ISNULL(Mem_OfficeEmailAddress,'') <> ''
	            AND (ISNULL(Ttr_WFPayLVCode,'') = '' AND Ttr_WFPayLVHr = 0.00)
	            AND (ISNULL(Ttr_WFNoPayLVCode,'') = '' AND Ttr_WFNoPayLVHr = 0.00)
	            AND Ttm_IDNo IS NULL";
                #endregion

                string sqlFinal = string.Empty;
                for (int i = 0; i < dtDBNames.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        sqlFinal += string.Format(sql, dtDBNames.Rows[i]["DBName"].ToString(), CentralProfile, UnpairedLogsPastDays, dtDBNames.Rows[i]["CompanyCode"].ToString(), (new CommonBL()).GetParameterValueFromCentral("NAMEDSPLY", dtDBNames.Rows[i]["CompanyCode"].ToString()));
                }
                    else
                    {
                        sqlFinal += @" 
                                   UNION 
                                 " + string.Format(sql, dtDBNames.Rows[i]["DBName"].ToString(), CentralProfile, UnpairedLogsPastDays, dtDBNames.Rows[i]["CompanyCode"].ToString(), (new CommonBL()).GetParameterValueFromCentral("NAMEDSPLY", dtDBNames.Rows[i]["CompanyCode"].ToString()));
                }
                }

                sqlFinal += @" ORDER BY 1, 3";


                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception e)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "UnpairedLogsError", "conUnpairedLogs : getUnpairedLogsRecordSet", e.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
                return dsDetails;
        }

        private DataSet getUnpairedLogsMultiplePockets()
        {
            DataSet dsDetails = new DataSet();
            #region Fetch Details
            string sql = @"SELECT  Ttm_IDNo AS [IDNumber]
                                    , Mem_OfficeEmailAddress AS [Recipient Email]
	                                , {1}.dbo.Udf_DisplayName(Ttm_IDNo,'{3}')  AS [Recipient Name]
                                    , CASE WHEN Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]
 		                            , CONVERT(CHAR(10), Ttm_Date,101) AS [Date]
		                            , LEFT(DATENAME(dw, Ttm_Date),3) AS [Day Week]
		                            , Ttr_DayCode AS [Day Code]
		                            , '[' + Ttr_ShiftCode + '] ' + LEFT(Msh_ShiftIn1,2)+':'+RIGHT(Msh_ShiftIn1,2) + '-' + LEFT(Msh_ShiftOut1, 2) + ':' + RIGHT(Msh_ShiftOut1,2)
			                             + ' ' + LEFT(Msh_ShiftIn2, 2) + ':' + RIGHT(Msh_ShiftIn2, 2) + '-' + LEFT(Msh_ShiftOut2, 2) + ':' + RIGHT(Msh_ShiftOut2, 2) AS  [Shift]
		                            , CASE WHEN TIn = '0000' THEN '' ELSE LEFT(TIn, 2) + ':' + RIGHT(Tin, 2) END AS [Time In]
		                            , CASE WHEN TOut = '0000' THEN '' ELSE LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) END AS [Time Out]
		                            , CASE WHEN TIn <> '0000' AND TOut = '0000' THEN 'No Out ' + Seq 
		                                ELSE 'No In ' + Seq END AS [Remarks]
                                    , Seq
                            FROM {0}..T_EmpTimeRegisterMisc
                            INNER JOIN {0}..T_EmpTimeRegister ON Ttr_IDNo = Ttm_IDNo
	                            AND Ttr_Date = Ttm_Date
	                            AND Ttr_WFTimeMod = 'N'
                                AND Ttr_SkipService = 'N'
                                AND CONVERT(CHAR(10),Ttm_Date,101) <= CONVERT(CHAR(10),DATEADD(day, {5}, GETDATE()),101)
                            INNER JOIN {0}..M_Employee on Mem_IDNo = Ttm_IDNo
                                AND LEN(ISNULL(Mem_OfficeEmailAddress,'')) > 0
                            INNER JOIN {1}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
	                            AND Msh_CompanyCode = '{2}'
                                AND Msh_RequiredLogsOnBreak = 1
                            CROSS APPLY ( VALUES ('1', Ttm_ActIn_01,Ttm_Actout_01),
					                            ('2', Ttm_ActIn_02,Ttm_Actout_02),
					                            ('3', Ttm_ActIn_03,Ttm_Actout_03),
					                            ('4', Ttm_ActIn_04,Ttm_Actout_04),
					                            ('5', Ttm_ActIn_05,Ttm_Actout_05),
					                            ('6', Ttm_ActIn_06,Ttm_Actout_06),
					                            ('7', Ttm_ActIn_07,Ttm_Actout_07),
					                            ('8', Ttm_ActIn_08,Ttm_Actout_08),
					                            ('9', Ttm_ActIn_09,Ttm_Actout_09),
					                            ('10', Ttm_ActIn_10,Ttm_Actout_10),
					                            ('11', Ttm_ActIn_11,Ttm_Actout_11),
					                            ('12', Ttm_ActIn_12,Ttm_Actout_12)) temp (Seq, TIn, TOut)
                            WHERE ((TIn <> '0000' AND TOut = '0000') OR (TIn = '0000' AND TOut <> '0000'))
	                        UNION ALL    
	                        SELECT Ttr_IDNo AS [IDNumber]			
                                    , Mem_OfficeEmailAddress AS [Recipient Email]			
	                                , {1}.dbo.Udf_DisplayName(Ttr_IDNo,'{3}')  AS [Recipient Name]		
                                    , CASE WHEN Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]			
 		                            , CONVERT(CHAR(10), Ttr_Date,101) AS [Date]	
		                            , LEFT(DATENAME(dw, Ttr_Date),3) AS [Day Week]	
		                            , Ttr_DayCode AS [Day Code]	
		                            , '[' + Ttr_ShiftCode + '] ' + LEFT(Msh_ShiftIn1,2)+':'+RIGHT(Msh_ShiftIn1,2) + '-' + LEFT(Msh_ShiftOut1, 2) + ':' + RIGHT(Msh_ShiftOut1,2)	
			                                + ' ' + LEFT(Msh_ShiftIn2, 2) + ':' + RIGHT(Msh_ShiftIn2, 2) + '-' + LEFT(Msh_ShiftOut2, 2) + ':' + RIGHT(Msh_ShiftOut2, 2) AS  [Shift]
		                            , LEFT(Tel_LogTime, 2) + ':' + RIGHT(Tel_LogTime, 2) as [Time In]
		                            , '' AS [Time Out]	
		                            , 'Unposted Log' AS [Remarks]	
		                            , Seq = 99 
                            FROM {6}..T_EmpDTR			
                            INNER JOIN {0}..T_EmpTimeRegister ON Ttr_IDNo = Tel_IDNo			
	                            AND Ttr_Date = Tel_LogDate		
	                            AND Ttr_WFTimeMod = 'N'		
	                            AND Ttr_SkipService = 'N'
	                            AND CONVERT(CHAR(10),Ttr_Date,101) <= CONVERT(CHAR(10),DATEADD(day, {5}, GETDATE()),101)	
                            INNER JOIN {0}..M_Employee ON Mem_IDNo = Ttr_IDNo	
	                            AND LEN(ISNULL(Mem_OfficeEmailAddress,'')) > 0		
                            INNER JOIN {1}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode			
	                            AND Msh_CompanyCode = '{2}'		
                            WHERE Tel_IsPosted = 0			
	                            AND ISNULL(Tel_Remark,'') NOT IN ('P','B','A')			
                                
                            ";
            #endregion

            string sqlFinal = string.Empty;
            for (int i = 0; i < dtDBNames.Rows.Count; i++)
            {
                string NAMEDSPLY = (new CommonBL()).GetParameterValueFromCentral("NAMEDSPLY", dtDBNames.Rows[i]["CompanyCode"].ToString());
                string TKADJCYCLE = (new CommonBL()).GetParameterValueFromPayroll("TKADJCYCLE", dtDBNames.Rows[i]["CompanyCode"].ToString(), dtDBNames.Rows[i]["DBName"].ToString());
                
                if (i == 0) 
                {
                    sqlFinal += string.Format(sql, dtDBNames.Rows[i]["DBName"].ToString()
                                                , dtDBNames.Rows[i]["CentralProfile"].ToString()
                                                , dtDBNames.Rows[i]["CompanyCode"].ToString()
                                                , NAMEDSPLY
                                                , TKADJCYCLE
                                                , UnpairedLogsPastDays
                                                , Globals.DTRDBName);
                }
                else
                {
                    sqlFinal += @" 
                                   UNION 
                                 " + string.Format(sql, dtDBNames.Rows[i]["DBName"].ToString()
                                                     , dtDBNames.Rows[i]["CentralProfile"].ToString()
                                                     , dtDBNames.Rows[i]["CompanyCode"].ToString()
                                                     , NAMEDSPLY
                                                     , TKADJCYCLE
                                                     , UnpairedLogsPastDays
                                                     , Globals.DTRDBName);
                }
            }
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    sqlFinal += @" ORDER BY [IDNumber], [Date], [Seq]";
                    dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                }
                catch (Exception e)
                {
                    NLLogger.Logger _log = new NLLogger.Logger();
                    _log.WriteLog(Application.StartupPath, "UnpairedLogsError", "conUnpairedLogs : getUnpairedLogsMultiplePockets", e.Message.ToString(), true);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsDetails;
        }

        public DataTable GetProfileDetails()
        {
            DataTable DBNames = new DataTable();
            DataSet dsTemp = new DataSet();
            string sql = string.Format(@"
                            SELECT Mpf_DatabaseNo
                                , Mpf_ProfileName
                                , Mpf_ServerName
                                , Mpf_DatabaseName
                                , Mpf_CentralProfile
                                , Mpf_UserID
                                , Mpf_Password
                                , Mpf_CompanyCode
                             FROM M_Profile
                            WHERE Mpf_RecordStatus = 'A'
                            AND Mpf_ProfileType = 'P'
                            AND Mpf_ProfileCategory = '{0}'"
                           , ConfigurationManager.AppSettings["ProfileCategory"].ToString());
            using (DALHelper dal = new DALHelper(Encrypt.decryptText(ConfigurationManager.AppSettings["CentralDBName"].ToString()), false))
            {
                try
                {
                    dal.OpenDB();
                    dsTemp = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch (Exception e)
                {
                    NLLogger.Logger _log = new NLLogger.Logger();
                    _log.WriteLog(Application.StartupPath, "UnpairedLogsError", "conUnpairedLogs : GetProfileDetails", e.Message.ToString(), true);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            //Test connections
            if (dsTemp.Tables.Count > 0 && dsTemp.Tables[0].Rows.Count > 0)
            {
                DBNames.Columns.Add("DatabaseNo");
                DBNames.Columns.Add("DBName");
                DBNames.Columns.Add("CentralProfile");
                DBNames.Columns.Add("CompanyCode");

                bool connectionOK = true;
                for (int i = 0; i < dsTemp.Tables[0].Rows.Count; i++)
                {
                    connectionOK = true;
                    using (DALHelper dalTest = new DALHelper(dsTemp.Tables[0].Rows[i]["Mpf_DatabaseName"].ToString(), false))
                    {
                        try
                        {
                            dalTest.OpenDB();
                            dalTest.CloseDB();
                        }
                        catch (Exception)
                        {
                            connectionOK = false;
                        }
                    }

                    if (connectionOK)
                    {
                        DBNames.Rows.Add(DBNames.NewRow());
                        DBNames.Rows[DBNames.Rows.Count - 1]["DatabaseNo"] = dsTemp.Tables[0].Rows[i]["Mpf_DatabaseNo"].ToString();
                        DBNames.Rows[DBNames.Rows.Count - 1]["DBName"] = dsTemp.Tables[0].Rows[i]["Mpf_DatabaseName"].ToString();
                        DBNames.Rows[DBNames.Rows.Count - 1]["CentralProfile"] = dsTemp.Tables[0].Rows[i]["Mpf_CentralProfile"].ToString();
                        DBNames.Rows[DBNames.Rows.Count - 1]["CompanyCode"] = dsTemp.Tables[0].Rows[i]["Mpf_CompanyCode"].ToString();
                    }
                }
            }

            return DBNames;
        }

    }
}
