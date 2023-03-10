using System;
using System.Data;
using System.Configuration;
using System.IO;
using CommonPostingLibrary;
using Posting.BLogic;
using System.Net.Mail;
using System.Net;
using Posting.DAL;
using System.Windows.Forms;

namespace UploadDownloadSchedulerConsole
{
    class conEWSSNotification
    {
        #region Email Parameters (Company Sepcific)
        private static string fromParameter = string.Empty;
        private static string urlParameter = string.Empty;
        private static string smtpSevrer = string.Empty;
        private static string smtpUsername = string.Empty;
        private static string smtpPassword = string.Empty;
        private static string smtpCC = string.Empty;
        private static string smtpBCC = string.Empty;
        private static int smtpPort = 25;
        private NLLogger.Logger _log = new NLLogger.Logger();
        private static DataTable dtDBNames;
        private static bool asHTML = false;
        private static string CentralProfile = string.Empty;
        #endregion
        public conEWSSNotification()
        {
            try
            {
                //constructor
                fromParameter = CommonProcedures.GetAppSettingConfigString("FROM", "", false);             //ConfigurationManager.AppSettings["FROM"];
                urlParameter = CommonProcedures.GetAppSettingConfigString("EWSSServer", "", false);    //ConfigurationManager.AppSettings["WORKFLOWServer"].ToString();
                smtpSevrer = CommonProcedures.GetAppSettingConfigString("SMTPServer", "", false);          // ConfigurationManager.AppSettings["SMTPServer"].ToString();
                smtpUsername = CommonProcedures.GetAppSettingConfigString("SMTPUsername", "", false);      //ConfigurationManager.AppSettings["SMTPUsername"].ToString();
                smtpPassword = CommonProcedures.GetAppSettingConfigString("SMTPPassword", "", false);      //ConfigurationManager.AppSettings["SMTPPassword"].ToString();
                smtpCC = CommonProcedures.GetAppSettingConfigString("CC", "", false);                      //ConfigurationManager.AppSettings["CC"].ToString();
                smtpBCC = CommonProcedures.GetAppSettingConfigString("BC", "", false);                     //ConfigurationManager.AppSettings["BC"].ToString();
                asHTML = CommonProcedures.GetAppSettingConfigBool("EWSSEMAILINHTMLFORMAT", false);            //Convert.ToBoolean(ConfigurationManager.AppSettings["WFNOTIFICATIONHTML"].ToString());
                CentralProfile = CommonProcedures.GetAppSettingConfigString("CentralDBName", "", true);

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
            catch
            {
                asHTML = false;
            }
        }
        public static string GenerateTextFile(string strFileName, string strPath, string strText)
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
        public void runEWSSNotification()
        {
            dtDBNames = GetProfileDetails();

            //Ovetime
            try
            {
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getEmployeeOvertime(), "OVERTIME") : this.SendNotification(getEmployeeOvertime(), "OVERTIME"));
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getSuperiorOvertime(), "OVERTIME", "A") : this.SendNotification(getSuperiorOvertime(), "OVERTIME", "A"));
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : OVERTIME", e.StackTrace, true);
                //proceed to other notification
            }
            //Leave
            try
            {
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getEmployeeLeave(), "LEAVE") : this.SendNotification(getEmployeeLeave(), "LEAVE"));
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getSuperiorLeave(), "LEAVE", "A") : this.SendNotification(getSuperiorLeave(), "LEAVE", "A"));
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : LEAVE", e.StackTrace, true);
                //proceed to other notification
            }
            //Time Correction
            try
            {
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getEmployeeTimeCor(), "TIME CORRECTION") : this.SendNotification(getEmployeeTimeCor(), "TIME CORRECTION"));
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getSuperiorTimeCor(), "TIME CORRECTION", "A") : this.SendNotification(getSuperiorTimeCor(), "TIME CORRECTION", "A"));
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : TIME CORRECTION", e.StackTrace, true);
                //proceed to other notification
            }
            //Shift
            try
            {
                //this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getEmployeeShift(), "SHIFT") : this.SendNotification(getEmployeeShift(), "SHIFT"));
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getSuperiorShift(), "SHIFT", "A") : this.SendNotification(getSuperiorShift(), "SHIFT", "A"));
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : SHIFT", e.StackTrace, true);
                //proceed to other notification
            }
            //Employee Update
            try
            {
                this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(getSuperiorEmpUpdate(), "EMPLOYEEUPDATE", "A") : this.SendNotification(getSuperiorEmpUpdate(), "EMPLOYEEUPDATE", "A"));
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : EMPLOYEEUPDATE", e.StackTrace, true);
                //proceed to other notification
            }
            this.ClearUpDataEmailNotification();
        }

        #region EWSS Transactions
        private DataSet getEmployeeOvertime()
        {
            #region Query
            string query = @"SELECT SUBM.Mem_OfficeEmailAddress AS [Recipient Email]
		                        , {1}.dbo.Udf_DisplayName(Tot_SubmittedBy,@NAMEDSPLY) AS [Recipient Name]  
                                , CASE WHEN SUBM.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]
		                        , CASE WHEN Tot_IDNo <> Tot_SubmittedBy THEN 1 ELSE 0 END AS [In Behalf]
		                        , Mpf_ProfileName AS [Profile]
		                        , Tot_IDNo AS [ID Number]
		                        , {1}.dbo.Udf_DisplayName(Tot_IDNo,@NAMEDSPLY) AS [Name]  
		                        , REQTYPE.Mcd_Name AS [Request Type]
		                        , CONVERT(CHAR(10), Tot_OvertimeDate, 101) AS [Date]
		                        , Mpd_SubName AS [Overtime Type]
		                        , LEFT(Tot_StartTime, 2) + ':' + RIGHT(Tot_StartTime, 2) AS [Start Time]
		                        , LEFT(Tot_EndTime, 2) + ':' + RIGHT(Tot_EndTime, 2) AS [End Time]
		                        , Tot_OvertimeHours AS [Hour(s)]
		                        , Tot_ReasonForRequest AS [Reason for Request]
		                        , DOCUGSTAT.Mcd_Name  AS [Status]
		                        , CASE  WHEN Tot_OvertimeStatus IN ('05', '21') THEN	Tot_Authority1Comments
				                        WHEN Tot_OvertimeStatus IN ('07', '22') THEN	Tot_Authority2Comments
				                        WHEN Tot_OvertimeStatus IN ('09', '23') THEN	Tot_Authority3Comments
				                        WHEN Tot_OvertimeStatus IN ('11', '24') THEN	Tot_Authority4Comments
				                        WHEN Tot_OvertimeStatus IN ('13', '14', '15', '25') THEN	Tot_Authority5Comments
			                        END AS [Comments]
                                , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                        FROM {0}..T_EmailSending
                        INNER JOIN {0}..T_EmpOvertime ON Tot_DocumentNo = Ten_DocumentNo
	                         AND Tot_OvertimeStatus IN ('21', '22', '23', '24', '25', '05','07','09','11','13','14','15') 
                        INNER JOIN {1}..M_Employee SUBM ON SUBM.Mem_IDNo = Tot_SubmittedBy
                        LEFT JOIN {0}..M_PolicyDtl ON Mpd_CompanyCode = '{2}'
	                        AND  Mpd_SubCode = Tot_OvertimeType
	                        AND Mpd_PolicyCode = 'OTTYPE'
                        INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo = Tot_SubmittedBy
	                        AND Tar_DocumentCode = 'OVERTIME'
	                        AND  ISNULL(Tar_EndDate, Tot_OvertimeDate) >= Tot_OvertimeDate AND Tot_OvertimeDate >= Tar_StartDate
	                        AND (Tot_OvertimeType in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) OR
		                        'ALL' in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )
                            AND Tar_CompanyCode = '{2}'
	                        AND Tar_ProfileCode = '{3}'
                        INNER JOIN {1}..M_ApprovalRoute ON  Mar_CompanyCode = '{2}'
	                        AND Mar_RouteCode = Tar_RouteID  
                        LEFT JOIN {1}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_CompanyCode = '{2}'
	                        AND DOCUGSTAT.Mcd_Code  = Tot_OvertimeStatus  
                            AND DOCUGSTAT.Mcd_CodeType ='DOCUGSTAT'
                        LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                        AND REQTYPE.Mcd_Code = Tot_RequestType
	                        AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                        INNER JOIN {1}..M_Profile ON Mpf_DatabaseNo = '{3}'
                        WHERE Ten_RecordStatus = 'A'
	                        AND Ten_DocumentCode ='OVERTIME'
                            AND Ten_ActivityCode <> 'E'
	                        AND (CASE WHEN Ten_ActivityCode IN ('A' , 'C')  AND Tar_SendApproveEmail = 1 THEN 1
			                    WHEN Ten_ActivityCode = 'J'  AND Tar_SendRejectEmail = 1 THEN 1
			                    WHEN Ten_ActivityCode = 'R'  AND Tar_SendReturnEmail = 1 THEN 1
			                    WHEN Ten_ActivityCode = 'C'  AND Tar_SendApproveEmail = 1 THEN 1
			                    ELSE 0 END ) = 1 
	                       AND ISNULL(SUBM.Mem_OfficeEmailAddress,'') <> ''    	
            ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, false);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name], [Name], [Date], [Start Time]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getEmployeeOvertime", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        private DataSet getSuperiorOvertime()
        {
            #region Query
            string query = @"SELECT SUPERIOR.Mem_OfficeEmailAddress as [Recipient Email]
		                        , {1}.dbo.Udf_DisplayName(CASE WHEN Tot_OvertimeStatus = '04' THEN Mar_Authority1 
											                        WHEN Tot_OvertimeStatus = '06' THEN Mar_Authority2 
											                        WHEN Tot_OvertimeStatus = '08' THEN Mar_Authority3
											                        WHEN Tot_OvertimeStatus = '10' THEN Mar_Authority4 
											                        WHEN Tot_OvertimeStatus = '12' THEN Mar_Authority5 
											                        ELSE '' END,@NAMEDSPLY) as [Recipient Name]  
                                , CASE WHEN SUPERIOR.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]
		                        , Mpf_ProfileName as [Profile]
		                        , Tot_IDNo as [ID Number]
		                        , {1}.dbo.Udf_DisplayName(Tot_IDNo,@NAMEDSPLY) as [Name]  
		                        , REQTYPE.Mcd_Name as [Request Type]
		                        , CONVERT(CHAR(10), Tot_OvertimeDate, 101) as [Date]
		                        , Mpd_SubName as [Overtime Type]
		                        , LEFT(Tot_StartTime, 2) + ':' + RIGHT(Tot_StartTime, 2) as [Start Time]
		                        , LEFT(Tot_EndTime, 2) + ':' + RIGHT(Tot_EndTime, 2) as [End Time]
		                        , Tot_OvertimeHours as [Hour(s)]
		                        , Tot_ReasonForRequest as [Reason for Request]
                                , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                        FROM {0}..T_EmailSending
                        INNER JOIN {0}..T_EmpOvertime on Tot_DocumentNo = Ten_DocumentNo
	                         AND Tot_OvertimeStatus IN ('04', '06', '08', '10', '12')
                        INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Tot_IDNo
	                        AND Tar_DocumentCode = 'OVERTIME'
	                        AND  ISNULL(Tar_EndDate, Tot_OvertimeDate) >= Tot_OvertimeDate AND Tot_OvertimeDate >= Tar_StartDate
	                        AND (Tot_OvertimeType in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) OR
		                        'ALL' in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )
                            AND Tar_CompanyCode = '{2}'
	                        AND Tar_ProfileCode = '{3}'
                        INNER JOIN {1}..M_ApprovalRoute on Mar_RouteCode = Tar_RouteID  
                            AND Mar_CompanyCode = '{2}'
                        INNER JOIN {1}..M_Employee SUPERIOR on SUPERIOR.Mem_IDNo = CASE WHEN Tot_OvertimeStatus = '04' THEN Mar_Authority1 
																	                        WHEN Tot_OvertimeStatus = '06' THEN Mar_Authority2 
																	                        WHEN Tot_OvertimeStatus = '08' THEN Mar_Authority3
																	                        WHEN Tot_OvertimeStatus = '10' THEN Mar_Authority4 
																	                        WHEN Tot_OvertimeStatus = '12' THEN Mar_Authority5 
																	                        ELSE '' END	
                        LEFT JOIN {0}..M_PolicyDtl on Mpd_SubCode = Tot_OvertimeType
	                        AND Mpd_PolicyCode = 'OTTYPE'
                            AND Mpd_CompanyCode = '{2}'
                        LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_Code = Tot_RequestType
	                        AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                            AND REQTYPE.Mcd_CompanyCode = '{2}'
                        INNER JOIN {1}..M_Profile on Mpf_DatabaseNo = '{3}'
                        WHERE Ten_RecordStatus = 'A'
	                        AND Ten_DocumentCode ='OVERTIME'
	                        AND Ten_ActivityCode = 'E'
	                        ---AND (CASE WHEN Ten_ActivityCode = 'E' AND Tar_ReceiveEndorseEmail = 1 THEN 1 ELSE 0 END) = 1 
	                        AND ISNULL(SUPERIOR.Mem_OfficeEmailAddress,'') <> ''	
";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, false);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name], [Name], [Date], [Start Time]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getSuperiorOvertime", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        private DataSet getEmployeeLeave()
        {
            #region Query
            string query = @"SELECT SUBM.Mem_OfficeEmailAddress as [Recipient Email]
		                        , {1}.dbo.Udf_DisplayName(Tlv_SubmittedBy,@NAMEDSPLY) as [Recipient Name]  
                                , CASE WHEN SUBM.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]
		                        , CASE WHEN Tlv_IDNo <> Tlv_SubmittedBy THEN 1 ELSE 0 END as [In Behalf]
		                        , Mpf_ProfileName as [Profile]
		                        , Tlv_IDNo as [ID Number]
		                        , {1}.dbo.Udf_DisplayName(Tlv_IDNo,@NAMEDSPLY) as [Name]  
		                        , REQTYPE.Mcd_Name as [Request Type]
		                        , CONVERT(CHAR(10), Tlv_LeaveDate, 101) as [Date]
		                        , Mlv_LeaveDescriptiON as [Leave Type]
		                        , LEFT(Tlv_StartTime, 2) + ':' + RIGHT(Tlv_StartTime, 2) as [Start Time]
		                        , LEFT(Tlv_EndTime, 2) + ':' + RIGHT(Tlv_EndTime, 2) as [End Time]
		                        , CAST(CASE WHEN @LVHRENTRY='TRUE' THEN Tlv_LeaveHours ELSE Tlv_LeaveHours/@LVHRSINDAY END as decimal(7,4)) as [@LVHRLABEL]
		                        , Tlv_ReasONForRequest as [Reason for Request]
		                        , DOCUGSTAT.Mcd_Name  as [Status]
		                        , CASE  WHEN Tlv_LeaveStatus IN ('05', '21') THEN	Tlv_Authority1Comments
				                        WHEN Tlv_LeaveStatus IN ('07', '22') THEN	Tlv_Authority2Comments
				                        WHEN Tlv_LeaveStatus IN ('09', '23') THEN	Tlv_Authority3Comments
				                        WHEN Tlv_LeaveStatus IN ('11', '24') THEN	Tlv_Authority4Comments
				                        WHEN Tlv_LeaveStatus IN ('13', '14', '15', '25') THEN	Tlv_Authority5Comments
			                        END as [Comments]
                                , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                        FROM {0}..T_EmailSending
                        INNER JOIN {0}..T_EmpLeave ON Tlv_DocumentNo = Ten_DocumentNo
	                         AND Tlv_LeaveStatus IN ('21', '22', '23', '24', '25', '05','07','09','11','13','14','15') 
                        INNER JOIN {1}..M_Employee SUBM ON SUBM.Mem_IDNo = Tlv_SubmittedBy
                        INNER JOIN {1}..M_Leave ON Mlv_LeaveCode =  Tlv_LeaveCode
                            AND Mlv_CompanyCode = '{2}'
                        INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Tlv_SubmittedBy
	                        AND Tar_DocumentCode = 'LEAVE'
	                        AND  ISNULL(Tar_EndDate, Tlv_LeaveDate) >= Tlv_LeaveDate AND Tlv_LeaveDate >= Tar_StartDate
	                        AND (Tlv_LeaveCode in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) OR
		                        'ALL' in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )
                            AND Tar_CompanyCode = '{2}'
	                        AND Tar_ProfileCode = '{3}'
                        INNER JOIN {1}..M_ApprovalRoute ON  Mar_CompanyCode = '{2}'
	                        AND Mar_RouteCode = Tar_RouteID  
                        LEFT JOIN {1}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_CompanyCode = '{2}'
	                        AND DOCUGSTAT.Mcd_Code  = Tlv_LeaveStatus  
                            AND DOCUGSTAT.Mcd_CodeType ='DOCUGSTAT'
                        LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                        AND REQTYPE.Mcd_Code = Tlv_RequestType
	                        AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                        INNER JOIN {1}..M_Profile ON Mpf_DatabaseNo = '{3}'
                        WHERE Ten_RecordStatus = 'A'
	                        AND Ten_DocumentCode ='LEAVE'
                          AND Ten_ActivityCode <> 'E'
	                        AND (CASE WHEN Ten_ActivityCode IN ('A' , 'C')  AND Tar_SendApproveEmail = 1 THEN 1
			                        WHEN Ten_ActivityCode = 'J'  AND Tar_SendRejectEmail = 1 THEN 1
			                        WHEN Ten_ActivityCode = 'R'  AND Tar_SendReturnEmail = 1 THEN 1
			                        WHEN Ten_ActivityCode = 'C'  AND Tar_SendApproveEmail = 1 THEN 1
			                        ELSE 0 END ) = 1 
	                        AND ISNULL(SUBM.Mem_OfficeEmailAddress,'') <> ''
                    ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, true);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name], [Name], [Date], [Start Time]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getEmployeeLeave", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            } 
            return dsDetails;
        }
        private DataSet getSuperiorLeave()
        {
            #region Query
            string query = @"SELECT 	  SUPERIOR.Mem_OfficeEmailAddress as [Recipient Email]
		                            , {1}.dbo.Udf_DisplayName(CASE WHEN Tlv_LeaveStatus = '04' THEN Mar_Authority1 
											                            WHEN Tlv_LeaveStatus = '06' THEN Mar_Authority2 
											                            WHEN Tlv_LeaveStatus = '08' THEN Mar_Authority3
											                            WHEN Tlv_LeaveStatus = '10' THEN Mar_Authority4 
											                            WHEN Tlv_LeaveStatus = '12' THEN Mar_Authority5 
											                            ELSE '' END,@NAMEDSPLY) as [Recipient Name]
                                    , CASE WHEN SUPERIOR.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]  
		                            , Mpf_ProfileName as [Profile]
		                            , Tlv_IDNo as [ID Number]
		                            , {1}.dbo.Udf_DisplayName(Tlv_IDNo,@NAMEDSPLY) as [Name]  
		                            , REQTYPE.Mcd_Name as [Request Type]
		                            , CONVERT(CHAR(10), Tlv_LeaveDate, 101) as [Date]
		                            , Mlv_LeaveDescription as [Leave Type]
		                            , LEFT(Tlv_StartTime, 2) + ':' + RIGHT(Tlv_StartTime, 2) as [Start Time]
		                            , LEFT(Tlv_EndTime, 2) + ':' + RIGHT(Tlv_EndTime, 2) as [End Time]
		                            , CAST(CASE WHEN @LVHRENTRY='TRUE' THEN Tlv_LeaveHours ELSE Tlv_LeaveHours/@LVHRSINDAY END as decimal(7,4)) as [@LVHRLABEL]
		                            , Tlv_ReasonForRequest as [Reason for Request]
                                    , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                            FROM {0}..T_EmailSending
                            INNER JOIN {0}..T_EmpLeave on Tlv_DocumentNo = Ten_DocumentNo
	                             AND Tlv_LeaveStatus IN ('04', '06', '08', '10', '12')
                            INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Tlv_IDNo
	                            AND Tar_DocumentCode = 'LEAVE'
	                            AND  ISNULL(Tar_EndDate, Tlv_LeaveDate) >= Tlv_LeaveDate AND Tlv_LeaveDate >= Tar_StartDate
	                            AND (Tlv_LeaveCode in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) OR
		                            'ALL' in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )
                                AND Tar_CompanyCode = '{2}'
	                            AND Tar_ProfileCode = '{3}'
                            INNER JOIN {1}..M_ApprovalRoute on Mar_RouteCode = Tar_RouteID  
                                AND Mar_CompanyCode = '{2}'
                            INNER JOIN {1}..M_Employee SUPERIOR on SUPERIOR.Mem_IDNo = CASE WHEN Tlv_LeaveStatus = '04' THEN Mar_Authority1 
																	                            WHEN Tlv_LeaveStatus = '06' THEN Mar_Authority2 
																	                            WHEN Tlv_LeaveStatus = '08' THEN Mar_Authority3
																	                            WHEN Tlv_LeaveStatus = '10' THEN Mar_Authority4 
																	                            WHEN Tlv_LeaveStatus = '12' THEN Mar_Authority5 
																	                            ELSE '' END	
                            INNER JOIN {1}..M_Leave ON Mlv_LeaveCode =  Tlv_LeaveCode
                                AND Mlv_CompanyCode = '{2}'
                            LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                            AND REQTYPE.Mcd_Code = Tlv_RequestType
	                            AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                            INNER JOIN {1}..M_Profile on Mpf_DatabaseNo = '{3}'
                            WHERE Ten_RecordStatus = 'A'
	                            AND Ten_DocumentCode ='LEAVE'
	                            AND Ten_ActivityCode = 'E'
	                            ---AND (CASE WHEN Ten_ActivityCode = 'E' AND Tar_ReceiveEndorseEmail = 1 THEN 1 ELSE 0 END) = 1 
	                            AND ISNULL(SUPERIOR.Mem_OfficeEmailAddress,'') <> ''
                    ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, true);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name], [Name], [Date], [Start Time]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getSuperiorLeave", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        private DataSet getEmployeeTimeCor()
        {
            #region Query
            string query = @"SELECT SUBM.Mem_OfficeEmailAddress as [Recipient Email]
		                    , {1}.dbo.Udf_DisplayName(Ttm_SubmittedBy,@NAMEDSPLY) as [Recipient Name]  
                            , CASE WHEN SUBM.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title]
		                    , CASE WHEN Ttm_IDNo <> Ttm_SubmittedBy THEN 1 ELSE 0 END as [In Behalf]
		                    , Mpf_ProfileName as [Profile]
		                    , Ttm_IDNo as [ID Number]
		                    , {1}.dbo.Udf_DisplayName(Ttm_IDNo,@NAMEDSPLY) as [Name]  
		                    , REQTYPE.Mcd_Name as [Request Type]
		                    , CONVERT(CHAR(10), Ttm_TimeCorDate, 101) as [Date]
		                    , TMERECTYPE.Mcd_Name as [Correction Type]
		                    , STUFF((SELECT '   ' + CASE WHEN LogCtrlIn IN ('1','2') THEN  
			                    LEFT(TIn, 2) + ':' + RIGHT(TIn, 2) 
				                      ELSE
					                    '[' + LEFT(TIn, 2) + ':' + RIGHT(TIn, 2) + '] '
				                      END + '-' + 
				                      CASE WHEN LogCtrlOut IN ('1','2') THEN  
					                    LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) 
				                      ELSE
					                    '[' + LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) + '] '
				                      END
			                    FROM {0}..Udv_TimeCorrection TC
	                    CROSS APPLY ( VALUES ('1', Ttm_TimeIn01,Ttm_TimeOut01, LEFT(Ttm_LogControl,1),SUBSTRING(Ttm_LogControl,2,1)),
								                    ('2', Ttm_TimeIn02,Ttm_TimeOut02,SUBSTRING(Ttm_LogControl,3,1),SUBSTRING(Ttm_LogControl,4,1)),
								                    ('3', Ttm_TimeIn03,Ttm_TimeOut03,SUBSTRING(Ttm_LogControl,5,1),SUBSTRING(Ttm_LogControl,6,1)),
								                    ('4', Ttm_TimeIn04,Ttm_TimeOut04,SUBSTRING(Ttm_LogControl,7,1),SUBSTRING(Ttm_LogControl,8,1)),
								                    ('5', Ttm_TimeIn05,Ttm_TimeOut05,SUBSTRING(Ttm_LogControl,9,1),SUBSTRING(Ttm_LogControl,10,1)),
								                    ('6', Ttm_TimeIn06,Ttm_TimeOut06,SUBSTRING(Ttm_LogControl,11,1),SUBSTRING(Ttm_LogControl,12,1)),
								                    ('7', Ttm_TimeIn07,Ttm_TimeOut07,SUBSTRING(Ttm_LogControl,13,1),SUBSTRING(Ttm_LogControl,14,1)),
								                    ('8', Ttm_TimeIn08,Ttm_TimeOut08,SUBSTRING(Ttm_LogControl,15,1),SUBSTRING(Ttm_LogControl,16,1)),
								                    ('9', Ttm_TimeIn09,Ttm_TimeOut09,SUBSTRING(Ttm_LogControl,17,1),SUBSTRING(Ttm_LogControl,18,1)),
								                    ('10', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,19,1),SUBSTRING(Ttm_LogControl,20,1)),
								                    ('11', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,21,1),SUBSTRING(Ttm_LogControl,22,1)),
								                    ('12', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,23,1),SUBSTRING(Ttm_LogControl,24,1))
								                    ) temp (Seq, TIn, TOut,LogCtrlIn,LogCtrlOut)
				                    WHERE TC.Ttm_IDNo = T_EmpTimeCorrection.Ttm_IDNo
				                    AND TC.Ttm_TimeCorDate = T_EmpTimeCorrection.Ttm_TimeCorDate
				                    AND (LogCtrlIn <> 'X' OR LogCtrlOut <> 'X')
		                        ORDER BY Seq
			                    FOR XML PATH('')),1,1,'') as [Time]		
		                    , Ttm_ReasonForRequest as [Reason for Request]
		                    , DOCUGSTAT.Mcd_Name  as [Status]
		                    , CASE  WHEN Ttm_TimeCorStatus IN ('05', '21') THEN	Ttm_Authority1Comments
				                    WHEN Ttm_TimeCorStatus IN ('07', '22') THEN	Ttm_Authority2Comments
				                    WHEN Ttm_TimeCorStatus IN ('09', '23') THEN	Ttm_Authority3Comments
				                    WHEN Ttm_TimeCorStatus IN ('11', '24') THEN	Ttm_Authority4Comments
				                    WHEN Ttm_TimeCorStatus IN ('13', '14', '15', '25') THEN	Ttm_Authority5Comments
			                    END as [Comments]
                            , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                    FROM {0}..T_EmailSending
                    INNER JOIN {0}..T_EmpTimeCorrection ON Ttm_DocumentNo = Ten_DocumentNo
	                     AND Ttm_TimeCorStatus IN ('21', '22', '23', '24', '25', '05','07','09','11','13','14','15') 
                    INNER JOIN {1}..M_Employee SUBM ON SUBM.Mem_IDNo = Ttm_SubmittedBy
                    INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Ttm_SubmittedBy
	                    AND Tar_DocumentCode = 'TIMECOR'
	                    AND  ISNULL(Tar_EndDate, Ttm_TimeCorDate) >= Ttm_TimeCorDate AND Ttm_TimeCorDate >= Tar_StartDate
	                    AND (Ttm_TimeCorType in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) OR
		                    'ALL' in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )
                        AND Tar_CompanyCode = '{2}'
	                    AND Tar_ProfileCode = '{3}'
                    INNER JOIN {1}..M_ApprovalRoute ON  Mar_CompanyCode = '{2}'
	                    AND Mar_RouteCode = Tar_RouteID  
                    LEFT JOIN {1}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_CompanyCode = '{2}'
	                    AND DOCUGSTAT.Mcd_Code  = Ttm_TimeCorStatus  
                        AND DOCUGSTAT.Mcd_CodeType ='DOCUGSTAT'
                    LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                    AND REQTYPE.Mcd_Code = Ttm_RequestType
	                    AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                    LEFT JOIN {1}..M_CodeDtl TMERECTYPE ON TMERECTYPE.Mcd_CompanyCode = '{2}'
	                    AND TMERECTYPE.Mcd_Code = Ttm_TimeCorType 
	                    AND TMERECTYPE.Mcd_CodeType = 'TMERECTYPE'
                    INNER JOIN {1}..M_Profile ON Mpf_DatabaseNo = '{3}'
                    WHERE Ten_RecordStatus = 'A'
	                    AND Ten_DocumentCode ='TIMECOR'
                        AND Ten_ActivityCode <> 'E'
	                    AND (CASE WHEN Ten_ActivityCode IN ('A' , 'C')  AND Tar_SendApproveEmail = 1 THEN 1
			                    WHEN Ten_ActivityCode = 'J'  AND Tar_SendRejectEmail = 1 THEN 1
			                    WHEN Ten_ActivityCode = 'R'  AND Tar_SendReturnEmail = 1 THEN 1
			                    WHEN Ten_ActivityCode = 'C'  AND Tar_SendApproveEmail = 1 THEN 1
			                    ELSE 0 END ) = 1 
	                    AND ISNULL(SUBM.Mem_OfficeEmailAddress,'') <> ''
                    ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, false);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name], [Name], [Date]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getEmployeeTimeCor", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        private DataSet getSuperiorTimeCor()
        {
            #region Query
            string query = @"SELECT SUPERIOR.Mem_OfficeEmailAddress as [Recipient Email]
		                        , {1}.dbo.Udf_DisplayName(CASE WHEN Ttm_TimeCorStatus = '04' THEN Mar_Authority1 
											                        WHEN Ttm_TimeCorStatus = '06' THEN Mar_Authority2 
											                        WHEN Ttm_TimeCorStatus = '08' THEN Mar_Authority3
											                        WHEN Ttm_TimeCorStatus = '10' THEN Mar_Authority4 
											                        WHEN Ttm_TimeCorStatus = '12' THEN Mar_Authority5 
											                        ELSE '' END,@NAMEDSPLY) as [Recipient Name] 
                                , CASE WHEN SUPERIOR.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END AS [Recipient Title] 
		                        , Mpf_ProfileName as [Profile]
		                        , Ttm_IDNo as [ID Number]
		                        , {1}.dbo.Udf_DisplayName(Ttm_IDNo,@NAMEDSPLY) as [Name]  
		                        , REQTYPE.Mcd_Name as [Request Type]
		                        , CONVERT(CHAR(10), Ttm_TimeCorDate, 101) as [Date]
		                        , TMERECTYPE.Mcd_Name as [Correction Type]
		                        , STUFF((SELECT '   ' + CASE WHEN LogCtrlIn IN ('1','2') THEN  
			                        LEFT(TIn, 2) + ':' + RIGHT(TIn, 2) 
				                          ELSE
					                        '[' + LEFT(TIn, 2) + ':' + RIGHT(TIn, 2) + '] '
				                          END + '-' + 
				                          CASE WHEN LogCtrlOut IN ('1','2') THEN  
					                        LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) 
				                          ELSE
					                        '[' + LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) + '] '
				                          END
			                   FROM {0}..Udv_TimeCorrection TC
	                        CROSS APPLY ( VALUES ('1', Ttm_TimeIn01,Ttm_TimeOut01, LEFT(Ttm_LogControl,1),SUBSTRING(Ttm_LogControl,2,1)),
								                        ('2', Ttm_TimeIn02,Ttm_TimeOut02,SUBSTRING(Ttm_LogControl,3,1),SUBSTRING(Ttm_LogControl,4,1)),
								                        ('3', Ttm_TimeIn03,Ttm_TimeOut03,SUBSTRING(Ttm_LogControl,5,1),SUBSTRING(Ttm_LogControl,6,1)),
								                        ('4', Ttm_TimeIn04,Ttm_TimeOut04,SUBSTRING(Ttm_LogControl,7,1),SUBSTRING(Ttm_LogControl,8,1)),
								                        ('5', Ttm_TimeIn05,Ttm_TimeOut05,SUBSTRING(Ttm_LogControl,9,1),SUBSTRING(Ttm_LogControl,10,1)),
								                        ('6', Ttm_TimeIn06,Ttm_TimeOut06,SUBSTRING(Ttm_LogControl,11,1),SUBSTRING(Ttm_LogControl,12,1)),
								                        ('7', Ttm_TimeIn07,Ttm_TimeOut07,SUBSTRING(Ttm_LogControl,13,1),SUBSTRING(Ttm_LogControl,14,1)),
								                        ('8', Ttm_TimeIn08,Ttm_TimeOut08,SUBSTRING(Ttm_LogControl,15,1),SUBSTRING(Ttm_LogControl,16,1)),
								                        ('9', Ttm_TimeIn09,Ttm_TimeOut09,SUBSTRING(Ttm_LogControl,17,1),SUBSTRING(Ttm_LogControl,18,1)),
								                        ('10', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,19,1),SUBSTRING(Ttm_LogControl,20,1)),
								                        ('11', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,21,1),SUBSTRING(Ttm_LogControl,22,1)),
								                        ('12', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,23,1),SUBSTRING(Ttm_LogControl,24,1))
								                        ) temp (Seq, TIn, TOut,LogCtrlIn,LogCtrlOut)
				                        WHERE TC.Ttm_IDNo = T_EmpTimeCorrection.Ttm_IDNo
				                        AND TC.Ttm_TimeCorDate = T_EmpTimeCorrection.Ttm_TimeCorDate
				                        AND (LogCtrlIn <> 'X' OR LogCtrlOut <> 'X')
		                            ORDER BY Seq
			                        FOR XML PATH('')),1,1,'') as [Time]
		                        , Ttm_ReasonForRequest as [Reason for Request]
                                , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                        FROM {0}..T_EmailSending
                        INNER JOIN {0}..T_EmpTimeCorrection on Ttm_DocumentNo = Ten_DocumentNo
	                         AND Ttm_TimeCorStatus IN ('04', '06', '08', '10', '12')
                        INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Ttm_IDNo
	                        AND Tar_DocumentCode = 'TIMECOR'
	                        AND  ISNULL(Tar_EndDate, Ttm_TimeCorDate) >= Ttm_TimeCorDate AND Ttm_TimeCorDate >= Tar_StartDate
	                        AND (Ttm_TimeCorType in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) OR
		                        'ALL' in ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )
                            AND Tar_CompanyCode = '{2}'
	                        AND Tar_ProfileCode = '{3}'
                        INNER JOIN {1}..M_ApprovalRoute on Mar_CompanyCode = '{2}'
                            AND Mar_RouteCode = Tar_RouteID  
                        INNER JOIN {1}..M_Employee SUPERIOR on SUPERIOR.Mem_IDNo = CASE WHEN Ttm_TimeCorStatus = '04' THEN Mar_Authority1 
																	                        WHEN Ttm_TimeCorStatus = '06' THEN Mar_Authority2 
																	                        WHEN Ttm_TimeCorStatus = '08' THEN Mar_Authority3
																	                        WHEN Ttm_TimeCorStatus = '10' THEN Mar_Authority4 
																	                        WHEN Ttm_TimeCorStatus = '12' THEN Mar_Authority5 
																	                        ELSE '' END	
                        LEFT JOIN {1}..M_CodeDtl TMERECTYPE ON TMERECTYPE.Mcd_CompanyCode = '{2}'
	                        AND TMERECTYPE.Mcd_Code = Ttm_TimeCorType 
	                        AND TMERECTYPE.Mcd_CodeType = 'TMERECTYPE'
                        LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                        AND  REQTYPE.Mcd_Code = Ttm_RequestType
	                        AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                        INNER JOIN {1}..M_Profile on Mpf_DatabaseNo = '{3}'
                        WHERE Ten_RecordStatus = 'A'
	                        AND Ten_DocumentCode ='TIMECOR'
	                        AND Ten_ActivityCode = 'E'
	                        ---AND (CASE WHEN Ten_ActivityCode = 'E' AND Tar_ReceiveEndorseEmail = 1 THEN 1 ELSE 0 END) = 1 
	                        AND ISNULL(SUPERIOR.Mem_OfficeEmailAddress,'') <> ''
                    ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, false);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name] ,[Name] ,[Date]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getSuperiorTimeCor", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        private DataSet getSuperiorShift()
        {
            #region Query
            string query = @"SELECT SUPERIOR.Mem_OfficeEmailAddress as [Recipient Email]
		                        , {1}.dbo.Udf_DisplayName(CASE WHEN Tes_ShiftStatus = '04' THEN Mar_Authority1 								
	                                                        WHEN Tes_ShiftStatus = '06' THEN Mar_Authority2 							
	                                                        WHEN Tes_ShiftStatus = '08' THEN Mar_Authority3							
	                                                        WHEN Tes_ShiftStatus = '10' THEN Mar_Authority4 							
	                                                        WHEN Tes_ShiftStatus = '12' THEN Mar_Authority5 							
	                                                     ELSE '' END,@NAMEDSPLY) as [Recipient Name] 
                                , CASE WHEN SUPERIOR.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END as [Recipient Title] 
		                        , Mpf_ProfileName as [Profile]
		                        , Tes_IDNo as [ID Number]
		                        , {1}.dbo.Udf_DisplayName(Tes_IDNo,@NAMEDSPLY) as [Name]  
		                        , REQTYPE.Mcd_Name as [Request Type]
		                        , CONVERT(CHAR(10), Tes_ShiftDate, 101) as [Date]
		                        , '[' + Tes_ShiftCode + '] ' + LEFT(Msh_ShiftIn1,2)+':'+RIGHT(Msh_ShiftIn1,2) + '-' + LEFT(Msh_ShiftOut1, 2) + ':' + RIGHT(Msh_ShiftOut1,2)								
	                                + ' ' + LEFT(Msh_ShiftIn2, 2) + ':' + RIGHT(Msh_ShiftIn2, 2) + '-' + LEFT(Msh_ShiftOut2, 2) + ':' + RIGHT(Msh_ShiftOut2, 2) as [Shift]	
		                        , Tes_ReasonForRequest as [Reason for Request]
                                , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                            FROM {0}..T_EmailSending
                            INNER JOIN {0}..T_EmpShift ON Tes_DocumentNo = Ten_DocumentNo								
                                AND Tes_ShiftStatus IN ('04', '06', '08', '10', '12')
                            INNER JOIN {1}..M_Shift ON Msh_ShiftCode = Tes_ShiftCode								
	                            AND Msh_CompanyCode = '{2}'	
                            INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Tes_IDNo
	                            AND Tar_DocumentCode = 'SHIFT'
	                            AND ISNULL(Tar_EndDate, Tes_ShiftDate) >= Tes_ShiftDate AND Tes_ShiftDate >= Tar_StartDate
                                AND Tar_CompanyCode = '{2}'
	                            AND Tar_ProfileCode = '{3}'
                            INNER JOIN {1}..M_ApprovalRoute ON Mar_CompanyCode = '{2}'
                                AND Mar_RouteCode = Tar_RouteID  
                            INNER JOIN {1}..M_Employee SUPERIOR ON SUPERIOR.Mem_IDNo = CASE WHEN Tes_ShiftStatus = '04' THEN Mar_Authority1 
																	                        WHEN Tes_ShiftStatus = '06' THEN Mar_Authority2 
																	                        WHEN Tes_ShiftStatus = '08' THEN Mar_Authority3
																	                        WHEN Tes_ShiftStatus = '10' THEN Mar_Authority4 
																	                        WHEN Tes_ShiftStatus = '12' THEN Mar_Authority5 
																	                   ELSE '' END	
                            LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                            AND REQTYPE.Mcd_Code = Tes_RequestType
	                            AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                            INNER JOIN {1}..M_Profile ON Mpf_DatabaseNo = '{3}'
                            WHERE Ten_RecordStatus = 'A'
	                            AND Ten_DocumentCode ='SHIFT'
	                            AND Ten_ActivityCode = 'E'
	                            ---AND (CASE WHEN Ten_ActivityCode = 'E' AND Tar_ReceiveEndorseEmail = 1 THEN 1 ELSE 0 END) = 1 
	                            AND ISNULL(SUPERIOR.Mem_OfficeEmailAddress,'') <> ''
                    ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, false);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name] ,[Name] ,[Date]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getSuperiorShift", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        private DataSet getSuperiorEmpUpdate()
        {
            #region Query
            string query = @"SELECT SUPERIOR.Mem_OfficeEmailAddress as [Recipient Email]
		                        , {1}.dbo.Udf_DisplayName(CASE WHEN Tei_EmpUpdateStatus = '04' THEN Mar_Authority1 								
	                                                        WHEN Tei_EmpUpdateStatus = '06' THEN Mar_Authority2 							
	                                                        WHEN Tei_EmpUpdateStatus = '08' THEN Mar_Authority3							
	                                                        WHEN Tei_EmpUpdateStatus = '10' THEN Mar_Authority4 							
	                                                        WHEN Tei_EmpUpdateStatus = '12' THEN Mar_Authority5 							
	                                                     ELSE '' END,@NAMEDSPLY) as [Recipient Name] 
                                , CASE WHEN SUPERIOR.Mem_Gender = 'M' THEN 'Mr' ELSE 'Ms' END as [Recipient Title] 
		                        , Mpf_ProfileName as [Profile]
		                        , Tei_IDNo as [ID Number]
		                        , {1}.dbo.Udf_DisplayName(Tei_IDNo,@NAMEDSPLY) as [Name]  
		                        , REQTYPE.Mcd_Name as [Request Type]
		                        , CONVERT(CHAR(10),Tei_RequestDate, 101) as [Date]
		                       , UPPER(CASE Tei_UpdateType WHEN  'PD' THEN 'Personal Data'								
	                                    WHEN 'FE' THEN 'Family Data - Existing Member(s)'							
	                                    WHEN 'FA' THEN 'Family Data - Additional Member(s)'							
	                                   ELSE '' END) as [Type]		
		                        , Tei_ReasonForRequest as [Reason for Request]
                                , Ten_DocumentNo + Ten_LineNo AS [Document Sequence]
                            FROM {0}..T_EmailSending
                            INNER JOIN {0}..T_EmpInfo ON Tei_DocumentNo = Ten_DocumentNo								
                                AND Tei_EmpUpdateStatus IN ('04', '06', '08', '10', '12')
                            INNER JOIN {1}..T_EmpApprovalRoute ON Tar_IDNo =  Tei_IDNo
	                            AND Tar_DocumentCode = 'EMPUPDATE'
	                            AND ISNULL(Tar_EndDate, Tei_RequestDate) >= Tei_RequestDate AND Tei_RequestDate >= Tar_StartDate								
                                AND (Tei_UpdateType IN ( SELECT [Data] FROM {1}.[dbo].Udf_Split(Tar_SubDocumentCode, ',')) OR								
                                    'ALL' IN ( SELECT [Data] FROM {1}.[dbo].Udf_Split (Tar_SubDocumentCode, ',')) )				
                                AND Tar_CompanyCode = '{2}'
	                            AND Tar_ProfileCode = '{3}'
                            INNER JOIN {1}..M_ApprovalRoute ON Mar_CompanyCode = '{2}'
                                AND Mar_RouteCode = Tar_RouteID  
                            INNER JOIN {1}..M_Employee SUPERIOR ON SUPERIOR.Mem_IDNo = CASE WHEN Tei_EmpUpdateStatus = '04' THEN Mar_Authority1 
																	                        WHEN Tei_EmpUpdateStatus = '06' THEN Mar_Authority2 
																	                        WHEN Tei_EmpUpdateStatus = '08' THEN Mar_Authority3
																	                        WHEN Tei_EmpUpdateStatus = '10' THEN Mar_Authority4 
																	                        WHEN Tei_EmpUpdateStatus = '12' THEN Mar_Authority5 
																	                   ELSE '' END	
                            LEFT JOIN {1}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_CompanyCode = '{2}'
	                            AND REQTYPE.Mcd_Code = Tei_RequestType
	                            AND REQTYPE.Mcd_CodeType = 'REQTYPE'
                            INNER JOIN {1}..M_Profile ON Mpf_DatabaseNo = '{3}'
                            WHERE Ten_RecordStatus = 'A'
	                            AND Ten_DocumentCode ='EMPUPDATE'
	                            AND Ten_ActivityCode = 'E'
	                            ---AND (CASE WHEN Ten_ActivityCode = 'E' AND Tar_ReceiveEndorseEmail = 1 THEN 1 ELSE 0 END) = 1 
	                            AND ISNULL(SUPERIOR.Mem_OfficeEmailAddress,'') <> ''
                    ";
            #endregion
            DataSet dsDetails = new DataSet();
            string sqlFinal = SetupProfile(query, false);
            if (!sqlFinal.Equals(""))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        sqlFinal += @" ORDER BY [Recipient Name] ,[Name] ,[Date]";
                        dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        NLLogger.Logger _log = new NLLogger.Logger();
                        _log.WriteLog(Application.StartupPath, "EWSSError", "runEWSSNotification : getSuperiorEmpUpdate", ex.Message.ToString(), true);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return dsDetails;
        }
        #endregion

        #region EWSS Transaction Summary
        public void runEWSSSummaryNotification()
        {
            dtDBNames = GetProfileDetails();

            try
            {
                this.UpdateNotificationFlag(WFHelpers.WFHandlers.StandardWorkFlowNotificationSendingSummary(getSummary(), "TRANSACTION SUMMARY"));
                //this.UpdateNotificationFlag(asHTML ? this.SendNotificationHTMLFormat(, ) : this.SendNotification(getJobSplitMod(), "JOBSPLIT UPDATE"));
            }
            catch (Exception e)
            {
                //proceed to other notification
                _log.WriteLog(Application.StartupPath, "Log", "runEWSSSummaryNotification : TRANSACTION SUMMARY - Error", e.StackTrace, true);

            }
        }
        private DataSet getSummary()
        {
            DataSet dsDetails = new DataSet();
            #region Fetch Details from Summary Transaction
            string sql = @"SELECT CASE Tot_OvertimeStatus
                                                 WHEN '04' THEN Umt1.Muh_UserCode 
                                                 WHEN '06' THEN Umt2.Muh_UserCode
                                                 WHEN '08' THEN Umt3.Muh_UserCode
												 WHEN '10' THEN Umt4.Muh_UserCode
												 WHEN '12' THEN Umt5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE Tot_OvertimeStatus
												 WHEN '04' THEN Umt1.Muh_EmailAddress
												 WHEN '06' THEN Umt2.Muh_EmailAddress
												 WHEN '08' THEN Umt3.Muh_EmailAddress
												 WHEN '10' THEN Umt4.Muh_EmailAddress
												 WHEN '12' THEN Umt5.Muh_EmailAddress
                                                  END [Recipient Email]
							,CASE Tot_OvertimeStatus
                                                 WHEN '04' THEN UMT1.Muh_FirstName + ' ' + Umt1.Muh_LastName
                                                 WHEN '06' THEN UMT2.Muh_FirstName + ' ' + Umt2.Muh_LastName
                                                 WHEN '08' THEN UMT3.Muh_FirstName + ' ' + Umt3.Muh_LastName
                                                 WHEN '10' THEN UMT4.Muh_FirstName + ' ' + Umt4.Muh_LastName
                                                 WHEN '12' THEN UMT5.Muh_FirstName + ' ' + Umt5.Muh_LastName
                                                  END [Recipient Name]					  
							 ,'OVERTIME'[Transaction Type]
							 , Mem_LastName +', '+Mem_FirstName [Name]
							  , ADT1.Mcd_Name [Status]
                              , CONVERT(varchar(10),Tot_OvertimeDate,101) [Date]
                              , CONVERT(varchar(10),Tot_RequestDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Tot_RequestDate,114),5)[Applied Date/Time]
                              , ISNULL(Mpd_SubName, '-no overtime type setup-') [Type]
                              , LEFT(Tot_StartTime,2) + ':' + RIGHT(Tot_StartTime,2) [Start Time]
                              , LEFT(Tot_EndTime,2) + ':' + RIGHT(Tot_EndTime,2) [End Time]
                              ,Convert(varchar,Tot_OvertimeHours) [Hours]
							  ,'-NA-'[From]
							  ,'-NA-'[To]
							  ,'-NA-'[Beneficairy Name]
							  ,'-NA-'[Relationship]
							  ,'-NA-'[Street/Barangay/City]
                              , Tot_ReasonForRequest [Reason]
                              , dbo.getCostCenterFullNameV2(LEFT(Tot_CostcenterCode, 6)) [Section]
                              , CONVERT(varchar(10),Tot_SubmittedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Tot_SubmittedDate,114),5)[Endorsed Date/Time]
                             
                           FROM {0}..T_EmpOvertime
                          INNER JOIN {0}..M_Employee
                             ON Mem_IDNo = Tot_IDNo
                          
                           LEFT JOIN {0}..M_CodeDtl AD2
                             ON AD2.Mcd_Code = Tot_OvertimeClass
                            AND AD2.Mcd_CodeType = (SELECT Mmc_CodeTypeLookup FROM {0}..M_MiscellaneousColumn WHERE Mmc_ColumnName = 'Tot_Filler01')
						   LEFT JOIN {0}..M_CodeDtl ADT1
                             ON ADT1.Mcd_CodeType = 'WFSTATUS'
                            AND ADT1.Mcd_Code = Tot_OvertimeStatus
                           LEFT JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Tar_IDNo = Tot_IDNo
                            AND empApprovalRoute.Tar_IDNo = 'OVERTIME'
                           LEFT JOIN {0}..M_PolicyDtl
                             ON Mpd_ParamValue = Tot_OvertimeType
                            AND Mpd_PolicyCode = 'OTTYPE'
                          INNER JOIN {0}..M_ApprovalRoute AS routeMaster
                             ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
						   LEFT JOIN {0}..M_UserHdr UMT1
                             ON UMT1.Muh_UserCode = routeMaster.Mar_Authority1
                           LEFT JOIN {0}..M_UserHdr UMT2
                             ON UMT2.Muh_UserCode = routeMaster.Mar_Authority2
                           LEFT JOIN {0}..M_UserHdr UMT3
                             ON UMT3.Muh_UserCode = routeMaster.Mar_Authority3
						   LEFT JOIN {0}..M_UserHdr UMT4
                             ON UMT4.Muh_UserCode = routeMaster.Mar_Authority4
						   LEFT JOIN {0}..M_UserHdr UMT5
                             ON UMT5.Muh_UserCode = routeMaster.Mar_Authority5
                          WHERE Tot_OvertimeStatus in ('04','06','08','10','12')
UNION
SELECT                      CASE Tlv_LeaveStatus
                                                 WHEN '04' THEN Umt1.Muh_UserCode 
                                                 WHEN '06' THEN Umt2.Muh_UserCode
                                                 WHEN '08' THEN Umt3.Muh_UserCode
												 WHEN '10' THEN Umt4.Muh_UserCode
												 WHEN '12' THEN Umt5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE Tlv_LeaveStatus
                                                 WHEN '04' THEN Umt1.Muh_EmailAddress
												 WHEN '06' THEN Umt2.Muh_EmailAddress
												 WHEN '08' THEN Umt3.Muh_EmailAddress
												 WHEN '10' THEN Umt4.Muh_EmailAddress
												 WHEN '12' THEN Umt5.Muh_EmailAddress
                                                  END 
							,CASE Tlv_LeaveStatus
                                                 WHEN '04' THEN UMT1.Muh_FirstName + ' ' + Umt1.Muh_LastName
                                                 WHEN '06' THEN UMT2.Muh_FirstName + ' ' + Umt2.Muh_LastName
                                                 WHEN '08' THEN UMT3.Muh_FirstName + ' ' + Umt3.Muh_LastName
                                                 WHEN '10' THEN UMT4.Muh_FirstName + ' ' + Umt4.Muh_LastName
                                                 WHEN '12' THEN UMT5.Muh_FirstName + ' ' + Umt5.Muh_LastName
                                                  END [Recipient Name]						  
							 ,'LEAVE'[Transaction Type]
							
                              , Mem_LastName +', '+Mem_FirstName [Name]
							  , ADT1.Mcd_Name [Status]
							  , CONVERT(varchar(10),Tlv_LeaveDate,101) [Date]
							  , CONVERT(varchar(10),Tlv_RequestDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Tlv_RequestDate,114),5)[Applied Date/Time]
                              , Mlv_LeaveDescription [Leave Type]
                              , LEFT(Tlv_StartTime,2) + ':' + RIGHT(Tlv_StartTime,2) [Start Time]
                              , LEFT(Tlv_EndTime,2) + ':' + RIGHT(Tlv_EndTime,2) [End Time]
                              , Convert(varchar,Tlv_LeaveHours) [Hours]
							  ,'-NA-'[From]
							  ,'-NA-'[To]
							  ,'-NA-'[Beneficairy Name]
							  ,'-NA-'[Relationship]
							  ,'-NA-'[Street/Barangay/City]
                              , Tlv_ReasonForRequest [Reason]
                              , dbo.getCostCenterFullNameV2(LEFT(Tlv_CostcenterCode, 6)) [Section]
                              , CONVERT(varchar(10),Tlv_SubmittedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Tlv_SubmittedDate,114),5)[Endorsed Date/Time]
                              
                              
                           FROM {0}..T_EmpLeave
                          INNER JOIN {0}..M_Employee
                             ON Mem_IDNo = Tlv_IDNo
                           
                            LEFT JOIN {0}..M_CodeDtl ADT1
                             ON ADT1.Mcd_CodeType = 'WFSTATUS'
                            AND ADT1.Mcd_Code = Tlv_LeaveStatus
                           LEFT JOIN {0}..M_Leave
                             ON Mlv_LeaveCode = Tlv_LeaveCode
                           LEFT JOIN {0}..M_CodeDtl AD2
                             ON AD2.Mcd_Code = Tlv_InitiatedBy
                            AND AD2.Mcd_CodeType = (SELECT Mmc_CodeTypeLookup FROM {0}..M_MiscellaneousColumn WHERE Mmc_ColumnName = 'Tlv_Filler01')
                           LEFT JOIN {0}..M_CodeDtl AD3
                             ON AD3.Mcd_Code = Tlv_LeaveReasonCode
                            AND AD3.Mcd_CodeType = (SELECT Mmc_CodeTypeLookup FROM {0}..M_MiscellaneousColumn WHERE Mmc_ColumnName = 'Tlv_Filler02')
                           LEFT JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Tar_IDNo = Tlv_IDNo
                            AND empApprovalRoute.Tar_IDNo = 'LEAVE'
                          INNER JOIN {0}..M_ApprovalRoute AS routeMaster
                             ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
							 LEFT JOIN {0}..M_UserHdr UMT1
                             ON UMT1.Muh_UserCode = routeMaster.Mar_Authority1
                           LEFT JOIN {0}..M_UserHdr UMT2
                             ON UMT2.Muh_UserCode = routeMaster.Mar_Authority2
                           LEFT JOIN {0}..M_UserHdr UMT3
                             ON UMT3.Muh_UserCode = routeMaster.Mar_Authority3
						   LEFT JOIN {0}..M_UserHdr UMT4
                             ON UMT4.Muh_UserCode = routeMaster.Mar_Authority4
						   LEFT JOIN {0}..M_UserHdr UMT5
                             ON UMT5.Muh_UserCode = routeMaster.Mar_Authority5
                          WHERE Tlv_LeaveStatus in ('04','06','08','10','12')
								

UNION
SELECT                       CASE Ttm_TimeCorStatus
                                                 WHEN '04' THEN Umt1.Muh_UserCode 
                                                 WHEN '06' THEN Umt2.Muh_UserCode
                                                 WHEN '08' THEN Umt3.Muh_UserCode
												 WHEN '10' THEN Umt4.Muh_UserCode
												 WHEN '12' THEN Umt5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE Ttm_TimeCorStatus
                                                 WHEN '04' THEN Umt1.Muh_EmailAddress
												 WHEN '06' THEN Umt2.Muh_EmailAddress
												 WHEN '08' THEN Umt3.Muh_EmailAddress
												 WHEN '10' THEN Umt4.Muh_EmailAddress
												 WHEN '12' THEN Umt5.Muh_EmailAddress
                                                  END 
							,CASE Ttm_TimeCorStatus
                                                 WHEN '04' THEN UMT1.Muh_FirstName + ' ' + Umt1.Muh_LastName
                                                 WHEN '06' THEN UMT2.Muh_FirstName + ' ' + Umt2.Muh_LastName
                                                 WHEN '08' THEN UMT3.Muh_FirstName + ' ' + Umt3.Muh_LastName
                                                 WHEN '10' THEN UMT4.Muh_FirstName + ' ' + Umt4.Muh_LastName
                                                 WHEN '12' THEN UMT5.Muh_FirstName + ' ' + Umt5.Muh_LastName
                                                  END [Recipient Name]					  
							 ,'TIME MODIFICATION'[Transaction Type]
                               , Mem_LastName +', '+Mem_FirstName [Name]
							  , ADT1.Mcd_Name [Status]
                              , CONVERT(varchar(10),Ttm_TimeCorDate,101) [Date]
                              , CONVERT(varchar(10),Ttm_RequestDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Ttm_RequestDate,114),5)[Applied Date/Time]
                              , ADT2.Mcd_Name [Type]
                              ,  CASE WHEN (Ttm_TimeIn1 = '')
										THEN '' 
										ELSE LEFT(Ttm_TimeIn1,2) + ':' + RIGHT(Ttm_TimeIn1,2)
                                 END [Time IN1]

                              , CASE WHEN (Ttm_TimeOut2 = '')
										THEN ''
										ELSE +LEFT(Ttm_TimeOut2,2) + ':' + RIGHT(Ttm_TimeOut2,2)
									END
											[Time OUT]
                              , '-NA-'
							  ,'-NA-'[From]
							  ,'-NA-'[To]
							  ,'-NA-'[Beneficairy Name]
							  ,'-NA-'[Relationship]
							  ,'-NA-'[Street/Barangay/City]
                              , Ttm_ReasonForRequest [Reason]
                             
                              , dbo.getCostCenterFullNameV2(LEFT(Ttm_CostcenterCode, 6)) [Section]
                              
                              , CONVERT(varchar(10),Ttm_SubmittedBy,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Ttm_SubmittedBy,114),5)[Endorsed Date/Time]
                             
                           FROM {0}..T_EmpTimeCorrection
                           LEFT JOIN {0}..T_EmpTimeRegister E1
                             ON E1.Ttr_Date = Ttm_TimeCorDate
                            AND E1.Ttr_IDNo = Ttm_IDNo
                           LEFT JOIN {0}..T_EmpTimeRegisterHst E2
                             ON E2.Ttr_Date = Ttm_TimeCorDate
                            AND E2.Ttr_IDNo = Ttm_IDNo
                           LEFT JOIN {0}..M_Shift S1
                             ON S1.Msh_ShiftCode = E1.Ttr_ShiftCode
                           LEFT JOIN {0}..M_Shift S2
                             ON S2.Msh_ShiftCode = E2.Ttr_ShiftCode
                          INNER JOIN {0}..M_Employee
                             ON Mem_IDNo = Ttm_IDNo
                          
                           LEFT JOIN {0}..M_CodeDtl ADT1
                             ON ADT1.Mcd_CodeType = 'WFSTATUS'
                            AND ADT1.Mcd_Code = Ttm_TimeCorStatus
                           LEFT JOIN {0}..M_CodeDtl ADT2
                             ON ADT2.Mcd_CodeType = 'TMERECTYPE'
                            AND ADT2.Mcd_Code = Ttm_TimeCorType
                           LEFT JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Tar_IDNo = Ttm_IDNo
                            AND empApprovalRoute.Tar_IDNo = 'TIMEMOD'
                          INNER JOIN {0}..M_ApprovalRoute AS routeMaster
                             ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
							  LEFT JOIN {0}..M_UserHdr UMT1
                             ON UMT1.Muh_UserCode = routeMaster.Mar_Authority1
                           LEFT JOIN {0}..M_UserHdr UMT2
                             ON UMT2.Muh_UserCode = routeMaster.Mar_Authority2
                           LEFT JOIN {0}..M_UserHdr UMT3
                             ON UMT3.Muh_UserCode = routeMaster.Mar_Authority3
						   LEFT JOIN {0}..M_UserHdr UMT4
                             ON UMT4.Muh_UserCode = routeMaster.Mar_Authority4
						   LEFT JOIN {0}..M_UserHdr UMT5
                             ON UMT5.Muh_UserCode = routeMaster.Mar_Authority5
                          WHERE Ttm_TimeCorStatus in ('04','06','08','10','12')


UNION
SELECT                       CASE Mve_Status
                                                 WHEN '04' THEN Umt1.Muh_UserCode 
                                                 WHEN '06' THEN Umt2.Muh_UserCode
                                                 WHEN '08' THEN Umt3.Muh_UserCode
												 WHEN '10' THEN Umt4.Muh_UserCode
												 WHEN '12' THEN Umt5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE Mve_Status
                                                 WHEN '04' THEN Umt1.Muh_EmailAddress
												 WHEN '06' THEN Umt2.Muh_EmailAddress
												 WHEN '08' THEN Umt3.Muh_EmailAddress
												 WHEN '10' THEN Umt4.Muh_EmailAddress
												 WHEN '12' THEN Umt5.Muh_EmailAddress
                                                  END 
							,CASE Mve_Status
                                                 WHEN '04' THEN UMT1.Muh_FirstName + ' ' + Umt1.Muh_LastName
                                                 WHEN '06' THEN UMT2.Muh_FirstName + ' ' + Umt2.Muh_LastName
                                                 WHEN '08' THEN UMT3.Muh_FirstName + ' ' + Umt3.Muh_LastName
                                                 WHEN '10' THEN UMT4.Muh_FirstName + ' ' + Umt4.Muh_LastName
                                                 WHEN '12' THEN UMT5.Muh_FirstName + ' ' + Umt5.Muh_LastName
                                                  END [Recipient Name]					  
							 ,'MOVEMENT' [Transaction Type]
                               , Mem_LastName +', '+Mem_FirstName [Name]
							  , ADT1.Mcd_Name [Status]
                               , CONVERT(varchar(10),Mve_EffectivityDate,101) [Date]

                               , CONVERT(varchar(10),Mve_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Mve_AppliedDate,114),5)[Applied Date/Time]
                                , ADT2.Mcd_Name [Move Type]
                               
								 ,'-NA-'
								 ,'-NA-'
								 ,'-NA-'
                               , CASE WHEN (Mve_Type = 'S')
                                      THEN Mve_From --Andre commented show only code as instructed ->S1.Msh_ShiftName
                                      WHEN (Mve_Type = 'G')
                                      THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Mcd_Name + '/' + A3.Mcd_Name 
                                      WHEN (Mve_Type = 'C')
                                      THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
                                      ELSE Mve_From
                                  END [From]
                               , CASE WHEN (Mve_Type = 'S')
                                      THEN Mve_To --Andre commented show only code as instructed ->S2.Msh_ShiftName
                                      WHEN (Mve_Type = 'G')
                                      THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Msh_ShiftNameA2.Mcd_Name + ' / ' + A4.Mcd_Name 
                                      WHEN (Mve_Type = 'C')
                                      THEN Mve_To --Andre commented show only code as instructed ->S2.Msh_ShiftNamedbo.getCostCenterFullNameV2(Mve_To)
                                      ELSE Mve_To
                                  END [To]
								 ,'-NA-'[Beneficairy Name]
							  ,'-NA-'[Relationship]
							  ,'-NA-'[Street/Barangay/City]
                               , Mve_Reason [Reason]
                               , dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 6)) [Section]
                               , CONVERT(varchar(10),Mve_EndorsedDateToChecker,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Mve_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              
                            FROM {0}..T_Movement
                           INNER JOIN {0}..M_Employee
                              ON Mem_IDNo = Mve_EmployeeId
                          
                            LEFT JOIN {0}..M_CodeDtl ADT1
                              ON ADT1.Mcd_CodeType = 'WFSTATUS'
                             AND ADT1.Mcd_Code = Mve_Status
                            LEFT JOIN {0}..M_CodeDtl ADT2
                              ON ADT2.Mcd_CodeType = 'MOVETYPE'
                             AND ADT2.Mcd_Code = Mve_Type
                            ---- JOIN FOR FROM - TO Description
	                        LEFT JOIN {0}..M_Shift S1
	                          ON S1.Msh_ShiftCode = Mve_From
	                         AND Mve_Type = 'S'
	                        LEFT JOIN {0}..M_Shift S2
	                          ON S2.Msh_ShiftCode = Mve_To
	                         AND Mve_Type = 'S'
	                        LEFT JOIN {0}..M_CodeDtl A1
	                          ON A1.Mcd_Code = LEFT(Mve_From, 3)
	                         AND A1.Mcd_CodeType = 'WORKTYPE'
	                         AND Mve_Type = 'G'
	                        LEFT JOIN {0}..M_CodeDtl A2
	                          ON A2.Mcd_Code = LEFT(Mve_To, 3)
	                         AND A2.Mcd_CodeType = 'WORKTYPE'
	                         AND Mve_Type = 'G'
	                        LEFT JOIN {0}..M_CodeDtl A3
	                          ON A3.Mcd_Code = LTRIM(RIGHT(Mve_From, 3))
	                         AND A3.Mcd_CodeType = 'WORKGROUP'
	                         AND Mve_Type = 'G'
	                        LEFT JOIN {0}..M_CodeDtl A4
	                          ON A4.Mcd_Code = LTRIM(RIGHT(Mve_To, 3))
	                         AND A4.Mcd_CodeType = 'WORKGROUP'
	                         AND Mve_Type = 'G'
                           LEFT JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Tar_IDNo = Mve_EmployeeId
                            AND empApprovalRoute.Tar_IDNo = 'MOVEMENT'
                          INNER JOIN {0}..M_ApprovalRoute AS routeMaster
                             ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
							  LEFT JOIN {0}..M_UserHdr UMT1
                              ON UMT1.Muh_UserCode = routeMaster.Mar_Authority1
                            LEFT JOIN {0}..M_UserHdr UMT2
                              ON UMT2.Muh_UserCode = routeMaster.Mar_Authority2
                            LEFT JOIN {0}..M_UserHdr UMT3
                              ON UMT3.Muh_UserCode = routeMaster.Mar_Authority3
						    LEFT JOIN {0}..M_UserHdr UMT4
                             ON UMT4.Muh_UserCode = routeMaster.Mar_Authority4
						   LEFT JOIN {0}..M_UserHdr UMT5
                             ON UMT5.Muh_UserCode = routeMaster.Mar_Authority5
                          WHERE Mve_Status in ('04','06','08','10','12')

UNION
SELECT                         CASE Pit_Status
                                                 WHEN '04' THEN C1.Muh_UserCode 
                                                 WHEN '06' THEN C2.Muh_UserCode
                                                 WHEN '08' THEN AP.Muh_UserCode
												 WHEN '10' THEN C4.Muh_UserCode
												 WHEN '12' THEN C5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE Pit_Status
                                                 WHEN '04' THEN C1.Muh_EmailAddress
												 WHEN '06' THEN C2.Muh_EmailAddress
												 WHEN '08' THEN AP.Muh_EmailAddress
												 WHEN '10' THEN C4.Muh_EmailAddress
												 WHEN '12' THEN C5.Muh_EmailAddress
                                                  END 
								,CASE Pit_Status
                                                 WHEN '04' THEN C1.Muh_LastName +', '+C1.Muh_FirstName
                                                 WHEN '06' THEN C2.Muh_LastName +', '+C2.Muh_FirstName
                                                 WHEN '08' THEN AP.Muh_LastName +', '+AP.Muh_FirstName
												 WHEN '10' THEN C4.Muh_LastName +', '+C4.Muh_FirstName
												 WHEN '12' THEN C5.Muh_LastName +', '+C5.Muh_FirstName
                                                  END [Recipient Name]					  
							 ,'TAX/CIVIL MOVEMENT' [Transaction Type]
                               , Mem_LastName +', '+Mem_FirstName [Name]
							  , AD1.Mcd_Name [Status]
							  
                               , Convert(varchar(10), Pit_EffectivityDate, 101) [Date]
							   , CONVERT(varchar(10),Pit_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Pit_AppliedDate,114),5)[Applied Date/Time]
                               , Pit_MoveType [Type]
							   ,'-NA-'
							   ,'-NA-'
							   ,'-NA-'
							   ,'NA'[Beneficairy Name]
							  ,'NA'[Relationship]
                               , Pit_From+'/'+Pit_Filler1 [From]
                               --, ADFTAX.Mcd_Name [From Tax Desc]
                               , Pit_To+'/'+Pit_Filler2 [To]
                               --, ADTTAX.Mcd_Name [To Tax Desc]
                               --, Pit_Filler1 [From Civil Code]
                               --, ADFCIVIL.Mcd_Name [From Civil Desc]
                               --, Pit_Filler2 [To Civil Code]
                               --, ADTCIVIL.Mcd_Name [To Civil Desc]

							   ,'NA'[Street/Barangay/City]
                               , Pit_Reason [Reason]
                               --, dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 4)) [Department]
                               , dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 6)) [Section]
                               
                               , CONVERT(varchar(10),Pit_EndorsedDateToChecker,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Pit_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                               
                            FROM {0}..T_PersonnelInfoMovement
                            LEFT JOIN {0}..M_Employee
                              ON Mem_IDNo =  Pit_EmployeeId
                           
                            LEFT JOIN {0}..M_CodeDtl AD1 
                              ON AD1.Mcd_Code = Pit_Status 
                             AND AD1.Mcd_CodeType =  'WFSTATUS'
                            LEFT JOIN {0}..M_CodeDtl ADFTAX 
                              ON ADFTAX.Mcd_Code = Pit_From 
                             AND ADFTAX.Mcd_CodeType =  'TAXCODE'
                            LEFT JOIN {0}..M_CodeDtl ADTTAX 
                              ON ADTTAX.Mcd_Code = Pit_To 
                             AND ADTTAX.Mcd_CodeType =  'TAXCODE'
                            LEFT JOIN {0}..M_CodeDtl ADFCIVIL 
                              ON ADFCIVIL.Mcd_Code = Pit_Filler1 
                             AND ADFCIVIL.Mcd_CodeType =  'CIVILSTAT'
                            LEFT JOIN {0}..M_CodeDtl ADTCIVIL 
                              ON ADTCIVIL.Mcd_Code = Pit_Filler2 
                             AND ADTCIVIL.Mcd_CodeType =  'CIVILSTAT'
                           INNER JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute 
                              ON empApprovalRoute.Tar_IDNo = Pit_EmployeeId
                             AND empApprovalRoute.Tar_DocumentCode = 'TAXMVMNT'
                            LEFT JOIN {0}..M_ApprovalRoute AS routeMaster 
                              ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
							  LEFT JOIN {0}..M_UserHdr C1 
                              ON C1.Muh_UserCode = routeMaster.Mar_Authority1
                            LEFT JOIN {0}..M_UserHdr C2 
                              ON C2.Muh_UserCode = routeMaster.Mar_Authority2
                            LEFT JOIN {0}..M_UserHdr AP 
                              ON AP.Muh_UserCode = routeMaster.Mar_Authority3
							LEFT JOIN {0}..M_UserHdr C4
                             ON C4.Muh_UserCode = routeMaster.Mar_Authority4
						   LEFT JOIN {0}..M_UserHdr C5
                             ON C5.Muh_UserCode = routeMaster.Mar_Authority5
                           WHERE Pit_Status in ('04','06','08','10','12')
                            AND Pit_MoveType = 'P1'

UNION
SELECT                      CASE But_Status
                                                 WHEN '04' THEN C1.Muh_UserCode 
                                                 WHEN '06' THEN C2.Muh_UserCode
                                                 WHEN '08' THEN AP.Muh_UserCode
												 WHEN '10' THEN C4.Muh_UserCode
												 WHEN '12' THEN C5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE But_Status
                                                 WHEN '04' THEN C1.Muh_EmailAddress
												 WHEN '06' THEN C2.Muh_EmailAddress
												 WHEN '08' THEN AP.Muh_EmailAddress
												 WHEN '10' THEN C4.Muh_EmailAddress
												 WHEN '12' THEN C5.Muh_EmailAddress
                                                  END 
								,CASE But_Status
                                                 WHEN '04' THEN C1.Muh_LastName +', '+C1.Muh_FirstName
                                                 WHEN '06' THEN C2.Muh_LastName +', '+C2.Muh_FirstName
                                                 WHEN '08' THEN AP.Muh_LastName +', '+AP.Muh_FirstName
												 WHEN '10' THEN C4.Muh_LastName +', '+C4.Muh_FirstName
												 WHEN '12' THEN C5.Muh_LastName +', '+C5.Muh_FirstName
                                                  END [Recipient Name]					  
							 ,'BENEFICIARY UPDATE' [Transaction Type]
                               , Mem_LastName +', '+Mem_FirstName [Name]
							  , AD1.Mcd_Name [Status]
                               --, dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 4)) [Department]
							   , Convert(varchar(10), But_EffectivityDate, 101) [Effectivity Date]
                               , CONVERT(varchar(10),But_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),But_AppliedDate,114),5)[Applied Date/Time]
								 , CASE WHEN (But_Type = 'N') 
			                          THEN 'NEW ENTRY'
			                          ELSE 'UPDATE EXISTING'
		                          END [Type]
								  ,'-NA-'
								  ,'-NA-'
								  ,'-NA-'
                               ,'-NA-'
							   ,'-NA-'
                               , But_Lastname+', '+But_Firstname [Beneficiary Name]
	                           --, But_Firstname [xFirtsname]
	                           --, But_Middlename [Beneficiary Middlename]
	                           --, Convert(varchar(10), But_Birthdate, 101) [Birthdate]
	                           , '('+But_Relationship+') -'+AD2.Mcd_Name [Relationship]
	                           --, AD2.Mcd_Name [Relationship Desc]
	                           --, But_Hierarchy [Hierarchy Code]
	                           --, AD3.Mcd_Name [Hierarchy Desc]
	                           --, But_HMODependent [HMO Dependent]
	                           --, But_InsuranceDependent [Insurance Dependent]
	                           --, But_BIRDependent [BIR Dependent]
	                           --, But_AccidentDependent [Accident Dependent]
	                           --, Convert(varchar(10), But_DeceasedDate, 101) [Deceased Date]
	                           --, Convert(varchar(10), But_CancelDate, 101) [Cancelled Date]
							   ,'-NA-'[Street/Barangay/City]
                               , But_Reason [Reason]
							   , dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) [Section]
                               , CONVERT(varchar(10),But_EndorsedDateToChecker,101) 
                                 + ' '
                                 + LEFT(CONVERT(varchar(20),But_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                            FROM {0}..T_BeneficiaryUpdate
                            LEFT JOIN {0}..M_Employee
                              ON Mem_IDNo =  But_EmployeeId
                            
                            LEFT JOIN {0}..M_CodeDtl AD1 
                              ON AD1.Mcd_Code = But_Status 
                             AND AD1.Mcd_CodeType =  'WFSTATUS'
                            LEFT JOIN {0}..M_CodeDtl AD2
                              ON AD2.Mcd_Code = But_Relationship
                             AND AD2.Mcd_CodeType = 'RELATION'
                            LEFT JOIN {0}..M_CodeDtl AD3
                              ON AD3.Mcd_Code = But_Hierarchy
                             AND AD3.Mcd_CodeType = 'HIERARCHDP'
                           INNER JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute 
                              ON empApprovalRoute.Tar_IDNo = But_EmployeeId
                             AND empApprovalRoute.Tar_DocumentCode = 'BNEFICIARY'
                            LEFT JOIN {0}..M_ApprovalRoute AS routeMaster 
                              ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
							  LEFT JOIN {0}..M_UserHdr C1 
                              ON C1.Muh_UserCode = routeMaster.Mar_Authority1
                            LEFT JOIN {0}..M_UserHdr C2 
                              ON C2.Muh_UserCode = routeMaster.Mar_Authority2
                            LEFT JOIN {0}..M_UserHdr AP 
                              ON AP.Muh_UserCode = routeMaster.Mar_Authority3
							LEFT JOIN {0}..M_UserHdr C4
                             ON C4.Muh_UserCode = routeMaster.Mar_Authority4
						   LEFT JOIN {0}..M_UserHdr C5
                             ON C5.Muh_UserCode = routeMaster.Mar_Authority5
                           WHERE But_Status in ('04','06','08','10','12')

UNION
SELECT                        CASE Amt_Status
                                                 WHEN '04' THEN C1.Muh_UserCode 
                                                 WHEN '06' THEN C2.Muh_UserCode
                                                 WHEN '08' THEN AP.Muh_UserCode
												 WHEN '10' THEN C4.Muh_UserCode
												 WHEN '12' THEN C5.Muh_UserCode
                                                  END [UserCode]
							 ,CASE Amt_Status
                                                 WHEN '04' THEN C1.Muh_EmailAddress
												 WHEN '06' THEN C2.Muh_EmailAddress
												 WHEN '08' THEN AP.Muh_EmailAddress
												 WHEN '10' THEN C4.Muh_EmailAddress
												 WHEN '12' THEN C5.Muh_EmailAddress
                                                  END 
							,CASE Amt_Status
                                                 WHEN '04' THEN C1.Muh_LastName +', '+C1.Muh_FirstName
                                                 WHEN '06' THEN C2.Muh_LastName +', '+C2.Muh_FirstName
                                                 WHEN '08' THEN AP.Muh_LastName +', '+AP.Muh_FirstName
												 WHEN '10' THEN C4.Muh_LastName +', '+C4.Muh_FirstName
												 WHEN '12' THEN C5.Muh_LastName +', '+C5.Muh_FirstName
                                                  END [Recipient Name]					  
							 ,'ADDRESS UPDATE' [Transaction Type]
                               , Mem_LastName +', '+Mem_FirstName [Name]
							  , AD1.Mcd_Name [Status]
                               , Convert(varchar(10), Amt_EffectivityDate, 101) [Date]
							   , CONVERT(varchar(10),Amt_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Amt_AppliedDate,114),5)[Applied Date/Time]
                               , CASE Amt_Type 
                                      WHEN 'A1' THEN 'Present'
                                      WHEN 'A2' THEN 'Permanent'
                                      WHEN 'A3' THEN 'Emergency Contact'
                                      ELSE ''
                                  END [Type]
								  ,'-NA-'
								  ,'-NA-'
								  ,'-NA-'
								  ,'-NA-'
								  ,'-NA-'
								  ,'-NA-'
								  ,'-NA-'
                               , Amt_Address1+'/'+ADADDRESS2.Mcd_Name+'/'+ADADDRESS3.Mcd_Name [Street/Barangay/City]
        --                       , Amt_TelephoneNo [Telephone No]
        --                       , case Amt_Type
		      --                      when 'A1' then Amt_CellularNo 
		      --                      else '- not applicable -'
	       --                     end [Cellular No]
        --                        , case Amt_Type
		      --                      when 'A1' then Amt_EmailAddress 
		      --                      else '- not applicable -'
	       --                     end [Email Address]
        --                        , case Amt_Type 
					   --             when 'A1' then Mtr_TranspoCode
					   --             else '- not applicable -'
				    --            end [Route Code]
			     --               , case Amt_Type	
					   --             when 'A1' then Rte_RouteName
					   --             else '- not applicable -'
				    --            end [Route Name]
			     --               , case Amt_Type	
								--	when 'A1' then CAST(Rte_Amount AS VARCHAR)
								--	else '- not applicable -'
								--end [Amount]
        --                        , CASE Amt_Type 
        --                                WHEN 'A3' THEN Amt_ContactPerson
        --                                ELSE '- not applicable -'
        --                            END [Contact Person]
        --                          , CASE Amt_Type 
        --                                WHEN 'A3' THEN ADRelation.Mcd_Name
        --                                ELSE '- not applicable -'
        --                            END [Contact Relation]
                               , Amt_Reason [Reason]
                               --, dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 4)) [Department]
                               , dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 6)) [Section]
                               , CONVERT(varchar(10),Amt_EndorsedDateToChecker,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Amt_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                               
                               
                            FROM {0}..T_AddressMovement
                            LEFT JOIN {0}..M_Employee
                              ON Mem_IDNo =  Amt_EmployeeId
                           
                            LEFT JOIN {0}..M_CodeDtl AD1 
                              ON AD1.Mcd_Code = Amt_Status 
                             AND AD1.Mcd_CodeType =  'WFSTATUS'
                            LEFT JOIN {0}..M_CodeDtl ADADDRESS2 
                              ON ADADDRESS2.Mcd_Code = Amt_Address2
                             AND ADADDRESS2.Mcd_CodeType =  'BARANGAY'
                            LEFT JOIN {0}..M_CodeDtl ADADDRESS3
                              ON ADADDRESS3.Mcd_Code = Amt_Address3 
                             AND ADADDRESS3.Mcd_CodeType =  'ZIPCODE'
                           INNER JOIN {0}..T_EmpApprovalRoute AS empApprovalRoute 
                              ON empApprovalRoute.Tar_IDNo = Amt_EmployeeId
                             AND empApprovalRoute.Tar_DocumentCode = 'ADDRESS'
                            LEFT JOIN {0}..M_ApprovalRoute AS routeMaster 
                              ON routeMaster.Mar_RouteCode = empApprovalRoute.Tar_RouteID
                            LEFT JOIN {0}..M_CodeDtl ADRelation
                                ON ADRelation.Mcd_Code = Amt_ContactRelation
                                AND ADRelation.Mcd_CodeType = 'RELATION'
                            LEFT JOIN {0}..M_Transpo ON Mtr_TranspoCode = Amt_Filler1 
				                    AND Mtr_StartDate = (SELECT MAX(Mtr_StartDate) 
									    FROM {0}..M_Transpo
									        WHERE Mtr_StartDate <= Amt_EffectivityDate
									        AND Mtr_TranspoCode = Amt_Filler1)
							 LEFT JOIN {0}..M_UserHdr C1 
                              ON C1.Muh_UserCode = routeMaster.Mar_Authority1
                            LEFT JOIN {0}..M_UserHdr C2 
                              ON C2.Muh_UserCode = routeMaster.Mar_Authority2
                            LEFT JOIN {0}..M_UserHdr AP 
                              ON AP.Muh_UserCode = routeMaster.Mar_Authority3
						    LEFT JOIN {0}..M_UserHdr C4
                              ON C4.Muh_UserCode = routeMaster.Mar_Authority4
						    LEFT JOIN {0}..M_UserHdr C5
                              ON C5.Muh_UserCode = routeMaster.Mar_Authority5
                           WHERE Amt_Status in ('04','06','08','10','12')
--Order by 1,2,3,4,5
";
            #endregion

            sql = @"Select * from (
                          " + sql +
                   ")summary where ([Recipient Email] is not null AND [Recipient Email] != '') AND ([Recipient Name] is not null AND [Recipient Name] != '')";

            string sqlFinal = string.Empty;
            for (int i = 0; i < dtDBNames.Rows.Count; i++)
            {
                if (i == 0)
                {
                    sqlFinal += string.Format(sql, dtDBNames.Rows[i]["DBName"].ToString());
                }
                else
                {
                    sqlFinal += @" 
                                   UNION 
                                 " + string.Format(sql, dtDBNames.Rows[i]["DBName"].ToString());
                }
            }
            sqlFinal += @" ORDER by 1,2,3,4,5";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsDetails = dal.ExecuteDataSet(sqlFinal, CommandType.Text);
                }
                catch (Exception ex)
                {
                    NLLogger.Logger _log = new NLLogger.Logger();
                    _log.WriteLog(Application.StartupPath, "EWSSError", "StandardWorkFlowNotificationSendingSummary : getSummary", ex.Message.ToString(), true);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsDetails;
        }
        #endregion
        private string SetupProfile(string query, bool bLeave)
        {
            #region Set-up Profile
            string sqlFinal = "";
            for (int i = 0; i < dtDBNames.Rows.Count; i++)
            {
                if (i == 0)
                    sqlFinal += string.Format(query, dtDBNames.Rows[i]["DBName"].ToString(), dtDBNames.Rows[i]["CentralProfile"].ToString(), dtDBNames.Rows[i]["CompanyCode"].ToString(), dtDBNames.Rows[i]["DatabaseNo"].ToString());
                else
                {
                    sqlFinal += @" 
                                   UNION 
                                 " + string.Format(query, dtDBNames.Rows[i]["DBName"].ToString(), dtDBNames.Rows[i]["CentralProfile"].ToString(), dtDBNames.Rows[i]["CompanyCode"].ToString(), dtDBNames.Rows[i]["DatabaseNo"].ToString());
                }
                sqlFinal = sqlFinal.Replace("@NAMEDSPLY", string.Format("'{0}'", (new CommonBL()).GetParameterValueFromCentral("NAMEDSPLY", dtDBNames.Rows[i]["CompanyCode"].ToString())));

                if (bLeave)
                {
                    string LVHRENTRY = (new CommonBL()).GetParameterValueFromPayroll("LVHRENTRY", dtDBNames.Rows[i]["CompanyCode"].ToString(), dtDBNames.Rows[i]["DBName"].ToString());
                    sqlFinal = sqlFinal.Replace("@LVHRENTRY", string.Format("'{0}'", LVHRENTRY));
                    sqlFinal = sqlFinal.Replace("@LVHRSINDAY", string.Format("{0}", (new CommonBL()).GetParameterValueFromPayroll("LVHRSINDAY", dtDBNames.Rows[i]["CompanyCode"].ToString(), dtDBNames.Rows[i]["DBName"].ToString())));
                    sqlFinal = sqlFinal.Replace("@LVHRLABEL", (Convert.ToBoolean(LVHRENTRY) ? "Hour(s)" : "Day(s)"));
                }
            }
            return sqlFinal;
            #endregion
        }
        private string SendNotification(DataSet dsDetails, string TransactionType)
        {
            return WFHelpers.WFHandlers.StandardWorkFlowNotificationSending(dsDetails, TransactionType, "");
            //Revise procedure. Maintenable procedure
            #region Old Code

////            string controlNumbers = string.Empty;
////            int itemNo = 0;
////            if (dsDetails.Tables.Count > 0 && dsDetails.Tables[0].Rows.Count > 0)
////            {
////                string messageBody = string.Empty;
////                string tempRecipientEmail = string.Empty;
////                string tempRecipientName = string.Empty;
////                string tempEmployeeName = string.Empty;
////                string tempAction = string.Empty;
////                string prevRecipientEmail = string.Empty;
////                string prevRecipientName = string.Empty;
////                string prevEmployeeName = string.Empty;
////                string prevAction = string.Empty;


////                for (int iNotify = 0; iNotify < dsDetails.Tables[0].Rows.Count; iNotify++)
////                {
////                    if (Convert.ToBoolean(dsDetails.Tables[0].Rows[iNotify]["Notify Flag"].ToString()))
////                    {
////                        tempRecipientEmail = dsDetails.Tables[0].Rows[iNotify]["Recipient Email"].ToString();
////                        tempRecipientName = dsDetails.Tables[0].Rows[iNotify]["Recipient Name"].ToString();
////                        tempEmployeeName = dsDetails.Tables[0].Rows[iNotify]["Name"].ToString();
////                        tempAction = dsDetails.Tables[0].Rows[iNotify]["Action"].ToString();


////                        if (prevEmployeeName != string.Empty
////                            && ((prevEmployeeName != tempEmployeeName && prevAction != tempAction)
////                            || prevAction != tempAction))
////                        {
////                            SendEmail(fromParameter
////                                        , prevRecipientEmail
////                                        , subjectParameter + " - " + TransactionType
////                                        , messageBody + getFooter());
////                            itemNo = 0;
////                            messageBody = string.Empty;
////                            prevEmployeeName = "";
////                        }

////                        if (messageBody.Trim() == string.Empty)
////                        {
////                            messageBody = "Hi " + tempRecipientName + @",
////
////" + getGreeting() + @".
////";
////                            #region Message body introduction
////                            switch (dsDetails.Tables[0].Rows[iNotify]["Action"].ToString().ToUpper())
////                            {
////                                case "ENDORSE":
////                                    messageBody += @"
////The following employee(s) would like to file for " + TransactionType + @"
////Detail(s):
////
////";
////                                    break;
////                                case "APPROVE":
////                                    messageBody += @"
////Your transaction for " + TransactionType + @" has been APPROVED.
////Detail(s):
////
////";
////                                    break;
////                                case "RETURN":
////                                    messageBody += @"
////Your transaction for " + TransactionType + @" has been RETURNED.
////Detail(s):
////
////";
////                                    break; ;
////                                case "DISAPPROVE":
////                                    messageBody += @"
////Your transaction for " + TransactionType + @" has been DISAPPROVED.
////Detail(s):
////
////";
////                                    break;
////                                default:
////                                    break;
////                            }
////                            #endregion

////                        }
////                        itemNo++;

////                        if ((!prevEmployeeName.Equals(tempEmployeeName) || itemNo == 1) && tempAction == "ENDORSE")
////                        {
////                            messageBody += "    " + tempEmployeeName + @"
////";
////                            prevEmployeeName = tempEmployeeName;
////                        }


////                        prevEmployeeName = tempEmployeeName;
////                        prevAction = tempAction;
////                        prevRecipientEmail = tempRecipientEmail;
////                        prevRecipientName = tempRecipientName;

////                        messageBody += "        " + dsDetails.Tables[0].Rows[iNotify]["Detail"].ToString() + @"
////";
////                    }
////                    controlNumbers += "'" + dsDetails.Tables[0].Rows[iNotify]["Control Sequence"].ToString() + "',";
////                }

////                //Sends last email
////                if (!prevRecipientEmail.Equals(string.Empty))
////                {
////                    SendEmail(fromParameter
////                             , prevRecipientEmail
////                             , subjectParameter + " - " + TransactionType
////                             , messageBody + getFooter());
////                }
////            }
            ////            return controlNumbers + "''";

            #endregion 
        }
        private string SendNotification(DataSet dsDetails, string TransactionType, string Action)
        {
            return WFHelpers.WFHandlers.StandardWorkFlowNotificationSending(dsDetails, TransactionType, Action);
        }
        private string SendNotificationHTMLFormat(DataSet dsDetails, string TransactionType)
        {
            return WFHelpers.WFHandlers.StandardWorkFlowNotificationSending(dsDetails, TransactionType, "");

            //Revise procedure. Maintenable procedure.
            #region Old Code

////            string controlNumbers = string.Empty;
////            int itemNo = 0;

////            if (dsDetails.Tables.Count > 0 && dsDetails.Tables[0].Rows.Count > 0)
////            {
////                string messageBody = string.Empty;

////                string tempRecipientEmail = string.Empty;
////                string tempRecipientName = string.Empty;
////                string tempEmployeeName = string.Empty;
////                string tempAction = string.Empty;
////                string prevRecipientEmail = string.Empty;
////                string prevRecipientName = string.Empty;
////                string prevEmployeeName = string.Empty;
////                string prevAction = string.Empty;

////                for (int iNotify = 0; iNotify < dsDetails.Tables[0].Rows.Count; iNotify++)
////                {
////                    if (Convert.ToBoolean(dsDetails.Tables[0].Rows[iNotify]["Notify Flag"].ToString()))
////                    {
////                        tempRecipientEmail = dsDetails.Tables[0].Rows[iNotify]["Recipient Email"].ToString();
////                        tempRecipientName = dsDetails.Tables[0].Rows[iNotify]["Recipient Name"].ToString();
////                        tempEmployeeName = dsDetails.Tables[0].Rows[iNotify]["Name"].ToString();
////                        tempAction = dsDetails.Tables[0].Rows[iNotify]["Action"].ToString();


////                        if (prevEmployeeName != string.Empty
////                            && ((prevEmployeeName != tempEmployeeName && prevAction != tempAction)
////                            || prevAction != tempAction))
////                        {
////                            SendEmail(fromParameter
////                                         , prevRecipientEmail
////                                         , subjectParameter + " - " + TransactionType
////                                         , messageBody + "</table><br>" + getFooter(true)
////                                         , true);
////                            messageBody = string.Empty;
////                        }

////                        prevEmployeeName = tempEmployeeName;
////                        prevAction = tempAction;
////                        prevRecipientEmail = tempRecipientEmail;
////                        prevRecipientName = tempRecipientName;

////                        if (messageBody.Trim() == string.Empty)
////                        {
////                            messageBody = "Hi " + tempRecipientName + @",<br><br>" + getGreeting() + ".<br><br>";
////                            #region Message body introduction
////                            switch (dsDetails.Tables[0].Rows[iNotify]["Action"].ToString().ToUpper())
////                            {
////                                case "ENDORSE":
////                                    messageBody += @"The following employee(s) would like to file for " + TransactionType + @"<br>
////Detail(s):<br><br>";
////                                    break;
////                                case "APPROVE":
////                                    messageBody += @"Your transaction for " + TransactionType + @" has been APPROVED.<br><br>
////Detail(s):<br><br>";
////                                    break;
////                                case "RETURN":
////                                    messageBody += @"Your transaction for " + TransactionType + @" has been RETURNED.<br><br>
////Detail(s):<br><br>";
////                                    break; ;
////                                case "DISAPPROVE":
////                                    messageBody += @"Your transaction for " + TransactionType + @" has been DISAPPROVED.<br><br>
////Detail(s):<br><br>";
////                                    break;
////                                default:
////                                    break;
////                            }
////                            #endregion
////                            messageBody += @"<table border=""1"">";
////                        }
////                        itemNo++;

////                        if ((!prevEmployeeName.Equals(tempEmployeeName) || itemNo == 1) && tempAction == "ENDORSE")
////                        {
////                            prevEmployeeName = tempEmployeeName;
////                            messageBody += "<tr>";
////                            messageBody += "<td>" + dsDetails.Tables[0].Rows[iNotify]["Name"].ToString() + "</td>";
////                            messageBody += "<tr>";
////                        }

////                        messageBody += "<tr>";
////                        messageBody += FormatDetailForHTML(dsDetails.Tables[0].Rows[iNotify]["Detail"].ToString());
////                        messageBody += "</tr>";
////                    }
////                    controlNumbers += "'" + dsDetails.Tables[0].Rows[iNotify]["Control Sequence"].ToString() + "',";
////                }
////                //Sends last email
////                if (!prevRecipientEmail.Equals(string.Empty))
////                {
////                    SendEmail(fromParameter
////                             , prevRecipientEmail
////                             , subjectParameter + " - " + TransactionType
////                             , messageBody + "</table><br>" + getFooter(true)
////                             , true);
////                }
////            }
            ////            return controlNumbers + "''";

            #endregion
        }
        private string SendNotificationHTMLFormat(DataSet dsDetails, string TransactionType, string Action)
        {
            return WFHelpers.WFHandlers.StandardWorkFlowNotificationSending(dsDetails, TransactionType, Action);
        }
        private int UpdateNotificationFlag(string controlNumbers)
        {
            int affected = 0;
            string sql = string.Format(@"  UPDATE @DBName..T_EmailSending 
                                            SET Ten_RecordStatus = 'P'
                                            , Ten_UpdatedBy = 'SERVICE'
                                            , Ten_UpdatedDate = GETDATE()
                                            WHERE Ten_DocumentNo+Ten_LineNo IN ({0}) 
                                           ", controlNumbers);
            if (!controlNumbers.Equals(string.Empty))
            {
                string sqlFinal = string.Empty;
                for (int i = 0; i < dtDBNames.Rows.Count; i++)
                {
                    sqlFinal += sql.Replace("@DBName", dtDBNames.Rows[i]["DBName"].ToString());
                }

                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransactionSnapshot();
                        affected = dal.ExecuteNonQuery(sqlFinal, CommandType.Text);
                        dal.CommitTransactionSnapshot();
                    }
                    catch
                    {
                        dal.RollBackTransactionSnapshot();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            return affected;
        }
        public static void SendEmail(string mailFrom, string mailTo, string mailSubject, string mailMsg)
        {
            try
            {
                MailMessage mail = new MailMessage(mailFrom, mailTo);
                //MailMessage mail = new MailMessage(mailFrom, "apsungahid@n-pax.com");
                mail.Subject = mailSubject;
                mail.Body = mailMsg;
                //mail.CC.Add(smtpCC);
                //if (mailSubject.Equals("Early Bird Report"))
                //{
                //    mail.CC.Add("");
                //    mail.CC.Add("");
                //}
                // mail.Bcc.Add(smtpBCC);
                SmtpClient mailClient = new SmtpClient(smtpSevrer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (smtpUsername.Trim() != string.Empty && smtpPassword.Trim() != string.Empty)
                {
                    mailClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }
                mailClient.Port = smtpPort;
                //mailClient.EnableSsl = true;
                mailClient.Send(mail);
            }
            catch (Exception e)
            {
                NLLogger.Logger _log = new NLLogger.Logger();
                _log.WriteLog(Application.StartupPath, "Log", "runEWSSNotification : SendEmail", e.StackTrace, true);
            }
        }
        public static bool SendEmail(string SenderName, string mailFrom, string mailTo, string mailSubject, string mailMsg, bool isHTML, bool bMailLogging)
        {
            bool ret = false;

            try
            {
                MailMessage mail = new MailMessage(mailFrom, mailTo);
                mail.Subject = mailSubject;
                mail.Body = mailMsg;
                if (isHTML)
                {
                    mail.IsBodyHtml = true;
                }

                //if (!smtpCC.Equals(string.Empty) && (mail.Body.Contains("REJECTED") || mail.Body.Contains("CANCELLED")))
                //{
                //    mail.CC.Add(smtpCC);
                //}

                SmtpClient mailClient = new SmtpClient(smtpSevrer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (smtpUsername.Trim() != string.Empty && smtpPassword.Trim() != string.Empty)
                {
                    mailClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }
                mailClient.Port = smtpPort;
                if (smtpPort == 587 || smtpPort == 465)
                {
                    mailClient.EnableSsl = true;
                }

                mailClient.Send(mail);

                if (bMailLogging)
                {
                    NLLogger.Logger _log = new NLLogger.Logger();
                    _log.WriteLog(Application.StartupPath, "SendEmail", string.Format("con{0} : SendEmail", SenderName), string.Format("Mail to: {0} \r\nTransaction: {1}\r\nMsg: [{2}]", mailTo, mailSubject, mailMsg), true);
                }

                ret = true;
            }
            catch (Exception e)
            {
                NLLogger.Logger _log = new NLLogger.Logger();
                _log.WriteLog(Application.StartupPath, "SendEmailError", string.Format("con{0} : SendEmail", SenderName), string.Format("Mail to: {0} \r\nTransaction: {1}\r\nMsg: [{2}]", mailTo, mailSubject, mailMsg), true);
                _log.WriteLog(Application.StartupPath, "EWSSError", string.Format("con{0} : SendEmail", SenderName), e.StackTrace, true);
            }

            return ret;
        }
        public static bool SendEmailWithCC(string mailFrom, string mailTo, string mailSubject, string mailMsg, bool isHTML,string[] CC)
        {
            bool ret = false;

            try
            {
                MailMessage mail = new MailMessage(mailFrom, mailTo);
                //MailMessage mail = new MailMessage(mailFrom, "apsungahid@n-pax.com");
                mail.Subject = mailSubject;
                mail.Body = mailMsg;
                if (isHTML)
                {
                    mail.IsBodyHtml = true;
                }

                if (!smtpCC.Equals(string.Empty) && (mail.Body.Contains("REJECTED")))
                {
                    mail.CC.Add(smtpCC);
                }
               
                foreach (string addCC in CC)
                {
                    if(addCC!=null && addCC.Trim()!="")
                    mail.CC.Add(addCC);
                }
                
                SmtpClient mailClient = new SmtpClient(smtpSevrer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (smtpUsername.Trim() != string.Empty && smtpPassword.Trim() != string.Empty)
                {
                    mailClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }
                mailClient.Port = smtpPort;

                if (smtpPort == 587 || smtpPort == 465)
                {
                    mailClient.EnableSsl = true;
                }

                mailClient.Send(mail);

                ret = true;
            }
            catch (Exception e)
            {
                NLLogger.Logger _log = new NLLogger.Logger();
                _log.WriteLog(Application.StartupPath, "Log", "conWorkflowNotification : SendEmail", e.StackTrace, true);
                
            }

            return ret;
        }
        private void ClearUpDataEmailNotification()
        {
            string sqlCleanUp = @"  DELETE FROM @DBName..T_EmailSendingHst
                                     WHERE ( Ludatetime < DATEADD(MONTH, -5, GETDATE()) AND Ten_RecordStatus IN ('P', 'X') )
                                        OR ( Ludatetime < DATEADD(MONTH, -6, GETDATE()))
                                    ";

            string sqlFinal = string.Empty;
            for (int i = 0; i < dtDBNames.Rows.Count; i++)
            {
                sqlFinal += sqlCleanUp.Replace("@DBName", dtDBNames.Rows[i]["DBName"].ToString());
            }

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    dal.ExecuteNonQuery(sqlFinal, CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch
                {
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }
        public DataTable GetProfileDetails()
        {
            DataTable DBNames = new DataTable();
            DataSet dsTemp = new DataSet();
            string sql = string.Format(@"SELECT Mpf_DatabaseNo
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
                    _log.WriteLog(Application.StartupPath, "EWSSError", "conEWSSNotification : GetProfileDetails", e.Message.ToString(), true);
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
