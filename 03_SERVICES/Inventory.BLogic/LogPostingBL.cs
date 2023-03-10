using System;
using CommonPostingLibrary;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Posting.DAL;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Globalization;

namespace Posting.BLogic
{
    public class LogPostingBL
    {
        
        public static DALHelper dtrDB;
        private static string _dtrDBName = string.Empty;
        private static NLLogger.Logger _log = new NLLogger.Logger();

        public LogPostingBL(string dtrDBName)
        {
            dtrDB = new DALHelper(dtrDBName, false);
            _dtrDBName = dtrDBName;
        }
        #region Profile

        public List<string> GetListOfDBProfiles()
        {
            List<string> dbProfiles = new List<string>();
            string dbName = string.Empty;

            string sql = string.Format(@"SELECT Mpf_DatabaseName + '|' + Mpf_CompanyCode FROM {0}..M_Profile 
                                                WHERE Mpf_ProfileType IN ('P','S') 
                                                AND Mpf_RecordStatus <> 'C'
                                                AND Mpf_ProfileCategory = '{1}'"
                                , Globals.CentralProfile
                                , ConfigurationManager.AppSettings["ProfileCategory"].ToString());

            try
            {
                dtrDB.OpenDB();

                SqlDataReader dataReader = dtrDB.ExecuteReader(sql);

                while (dataReader.Read())
                {
                    AddToDBProfiles(dataReader[0].ToString(), ref dbProfiles);
                }

                dataReader.Close();
            }
            finally
            {
                dtrDB.CloseDB();
            }

            return dbProfiles;
        }

        private void AddToDBProfiles(string dbName, ref List<string> dbProfiles)
        {
            if (!string.IsNullOrEmpty(dbName) && !dbProfiles.Contains(dbName))
            {
                dbProfiles.Add(dbName);
            }
        }

        #endregion

        #region Uploading V2 Objects

        public struct TimeEntryPrev
        {
            public Int16 TimeIn;
            public Int16 TimeOut;
        }

        public struct FIRSTLASTINOUT
        {
            public String IN;
            public String Out;
        }

        public class EmployeeLedgerPrev
        {
            public string EmployeeID;
            public DateTime ProcessDate;
            public string PayPeriod;
            public string DayCode;
            public string ShiftCode;
            public bool IsHoliday;
            public bool RestDay;
            public TimeEntryPrev ShiftTime = new TimeEntryPrev();
            public TimeEntryPrev ShiftBreak = new TimeEntryPrev();
            public TimeEntryPrev LogTime1 = new TimeEntryPrev();
            public TimeEntryPrev LogTime2 = new TimeEntryPrev();
        }

        public class EmployeeInAfterOutDTR
        {
            public string EmployeeID;
            public DateTime Logdate;
            public Int16 Logtime;
            public Char Logtype;
        }

        public class EmployeeCapturedOut
        {
            public string EmployeeID;
            public DateTime Logdate;
            public Int16 Logtime;
            public Char Logtype;
        }

        #endregion
       
        #region Log uploading revision queries and dataset

        private DataSet DsRecoverGraveyardOut = new DataSet();
        private DataSet DsRecoverGraveUnpostLIFO = new DataSet();
        private DataSet DsCleanLogsFromNextDayPostedtoCurrent = new DataSet();
        private DataSet DsRecoveryRegularShift = new DataSet();
        private DataSet DsRecoveryGraveYardCleaningIn = new DataSet();
        private DataSet DsRecoveryGraveYardCleaningOut = new DataSet();
        private DataSet DsRecoveryGraveYardShift = new DataSet();
        private DataSet DsLogRecheckOverloadCleanup = new DataSet();
        private DataSet DsOvertimeAfterMidnightPlus24 = new DataSet();
        private DataSet DsRepostLogTrail = new DataSet();
        private DataSet DsIntellegentPostingRegShiftwithGraveLogs = new DataSet();
        private DataSet DsIntellegentPostingGraveShiftwithRegLogs = new DataSet();

        #region RecoverGraveyardOut

        /// <summary>
        /// Arguments [DTRDB][LEDGERDB][DTRTABLE]
        /// </summary>

        private static string QueryRecoverGraveyardOut = @"
                                                        SELECT
                                                         LEDGER.Ttr_IDNo		    [EmployeeID]
						                                ,@ProcessDate               [ProcessDate]
						                                ,LEDGER.Ttr_ActIn_1			[IN1]
						                                ,LEDGER.Ttr_ActOut_1		[OUT1]
						                                ,LEDGER.Ttr_ActIn_2		    [IN2]	

                                                        (ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
				                                                    WHERE 
				                                                    Tel_IDNo=LEDGER.Ttr_IDNo 
				                                                    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
					                                                    AND CONVERT(INT,Tel_LogTime) 
					                                                    >=(CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
				                                                    AND Tel_LogType='O'
				                                                    ORDER BY Tel_LogTime ASC),'0000'
			                                                    )
	                                                    ) [OUT2]  -- Needed for checking

                                                    FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
                                                        INNER JOIN 
                                                        {1}..M_Shift SHIFT
                                                        ON 
		                                                    Msh_ShiftCode=Ttr_ShiftCode
                                                            AND Ttr_Date = @ProcessDate
		                                                    AND CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
		                                                    AND Ttr_ActIn_1 !='0000'
		                                                    AND Ttr_ActOut_2 ='0000'
		                                                    AND LEDGER.Usr_Login!='LOGUPLDSRVCS'
		                                                    AND 
		                                                    (
			                                                    CONVERT(
					                                                    INT,(ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
									                                                    WHERE  Tel_IDNo = Ttr_IDNo 
									                                                    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
										                                                    AND CONVERT(INT,Tel_LogTime) 
										                                                    >= (CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
									                                                    AND Tel_LogType='O'
									                                                    ORDER BY Tel_LogTime ASC),'0000'
								                                                    )
						                                                    ) 
					                                                    )
			
		                                                    ) <=1200
                                                        ";

        #endregion

        #region RecoverGraveUnpostLIFO

        /// <summary>
        /// Arguments [DTRDB][LEDGERDB][DTRTABLE]
        /// </summary>
        private static string QueryRecoverGraveUnpostLIFO = @"SELECT
														     LEDGER.Ttr_IDNo [EmployeeID]
														    ,@ProcessDate [ProcessDate]
														 
		                                                    ,(ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
																	    WHERE 
																	    Tel_IDNo=LEDGER.Ttr_IDNo 
																	    AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
																		    AND CONVERT(INT,Tel_LogTime)  
																		    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
																	    AND Tel_LogType='I' ORDER BY Tel_LogTime DESC),'0000')
														      ) [IN1] -- for updating
														
														     ,LEDGER.Ttr_ActOut_1 [OUT1]
														
														    ,LEDGER.Ttr_ActIn_2 [IN2]
														
		                                                    ,(ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
																	    WHERE 
																	    Tel_IDNo=LEDGER.Ttr_IDNo  
																	    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
																		    AND CONVERT(INT,Tel_LogTime) 
																		    >=(CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
																	    AND Tel_LogType='O'
																	    ORDER BY Tel_LogTime ASC), '0000')
														      ) [OUT2] --for updating
			                                                
                                                      FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
	                                                    JOIN 
	                                                    {1}.dbo.M_Shift SHIFT
	                                                    ON Msh_ShiftCode=Ttr_ShiftCode
													    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
													    AND
													    CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
													    AND CONVERT(INT,Ttr_ActOut_2) = '0000'
													    AND CONVERT(INT,Ttr_ActIn_1) = '0000'

                                                        AND
													    --CHECKING OF IN 1
													    
													    (
															    ((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
																	    WHERE 
																	    Tel_IDNo=LEDGER.Ttr_IDNo  
																	    AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date
																		    AND CONVERT(INT,Tel_LogTime) 
																		    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
																	    AND Tel_LogType='I' ORDER BY Tel_LogTime DESC))!='0000'
															    OR
			
															    ((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
																    WHERE 
																    Tel_IDNo=LEDGER.Ttr_IDNo 
																    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
																	    AND CONVERT(INT,Tel_LogTime) 
																	    >= (CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
																    AND Tel_LogType='O'
																    ORDER BY Tel_LogTime ASC)) <> '0000'
													    )
													    --CHECKING OF OUT2

													    --IN1 is PM
													    AND
															    (CONVERT(INT,((SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{2} 
																    WHERE 
																    Tel_IDNo=LEDGER.Ttr_IDNo 
																    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
																	    AND CONVERT(INT,Tel_LogTime) 
																	    >= (CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
																    AND Tel_LogType='O'
																    ORDER BY Tel_LogTime ASC)))) < '1000'";

        #endregion

        #region QueryCleanLogsFromNextDayPostedtoCurrent
        /// <summary>
        ///Arguments [LEDGERDB][DTRDB][RULED_IN][RULE_OUT][DTRTABLE]
        /// </summary>

        private static string QueryCleanLogsFromNextDayPostedtoCurrent = @"----***************************************
                                                                    ----LOGS FROM NEXT DAY WAS POSTED REG SHIFT
                                                                    ----***************************************
                                                                    SELECT
														                 Ttr_IDNo [EmployeeID]
														                ,@ProcessDate [ProcessDate]
	                                                                    ,(CASE WHEN Ttr_ActIn_1 < 1300 THEN ISNULL((SELECT TOP 1 Tel_LogTime FROM {1}.dbo.{4}
                                                                            WHERE CONVERT(DATETIME,Tel_LogDate) = Ttr_Date
                                                                            AND Tel_IDNo=Ttr_IDNo
                                                                            AND Tel_LogTime < 1200
                                                                            AND Tel_LogType = 'I' ORDER BY Tel_LogTime {2}
                                                                            ),'0000')
                                                                        ELSE '0000' END) [IN1]
														                ,Ttr_ActOut_1 [OUT1]
														                ,Ttr_ActIn_2 [IN2]
														                ,Ttr_ActOut_2 [OUT2]
                                                                    FROM {0}.dbo.T_EmpTimeRegister
                                                                        JOIN 
                                                                        {0}.dbo.M_Shift SHIFT
                                                                        ON Msh_ShiftCode=Ttr_ShiftCode
														                AND (Ttr_Date between @ProcessDate and @ProcessDate AND Msh_ShiftIn1 < Msh_ShiftOut2) 
														                AND (
														                (((SELECT TOP 1 Tel_IDNo FROM {1}.dbo.{4}
																                WHERE CONVERT(DATETIME,Tel_LogDate) = Ttr_Date 
																                AND Tel_IDNo = Ttr_IDNo
																                AND Tel_LogTime = Ttr_ActIn_1
																                AND Tel_LogType = 'I') IS NULL AND Ttr_ActIn_1 <> '0000')
															                )
														                AND 
															                ((SELECT TOP 1 Tel_IDNo FROM {1}.dbo.{4}
																                WHERE CONVERT(DATETIME,Tel_LogDate) = DATEADD(DD,1,Ttr_Date)
																                AND Tel_IDNo = Ttr_IDNo
																                AND Tel_LogTime = Ttr_ActIn_1
																                AND Tel_LogType = 'I') IS NOT NULL AND Ttr_ActIn_1 <> '0000')
														                )";

        #endregion

        #region RecoveryRegularShift
        /// <summary>
        ///Arguments [LEDGERDB][DTRDB][RULED_IN][RULE_OUT][DTRTABLE]
        /// </summary>
        private static string QueryRecoveryRegularShift = @"-----********************************
					                                      -----RECOVERY FOR REGULAR SHIFT LOGS
					                                      -----********************************
					                                      SELECT
					                                       LEDGER.Ttr_IDNo [EmployeeID]
													      ,@ProcessDate [ProcessDate]
														 
					                                      ----//---- SETTING IN1
					                                      ,(CASE when Ttr_ActIn_1 <> '0000' then Ttr_ActIn_1
													       ELSE 
							                                    ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
							                                    WHERE 
							                                    Tel_IDNo = LEDGER.Ttr_IDNo 
							                                    AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
							                                    AND CONVERT(INT,Tel_LogTime) < CONVERT(INT,SHIFT.Msh_ShiftOut1)
							                                    AND Tel_LogType = 'I' ORDER BY Tel_LogTime {2}
							                                    ),'0000')
					                                       END) [IN1]
					                                   
					                                       ,LEDGER.Ttr_ActOut_1 [OUT1]
														
													       ,LEDGER.Ttr_ActIn_2 [IN2]
														
					                                      ----//---- SETTING OUT2
					                                       ,(CASE when Ttr_ActOut_2 <> '0000' then Ttr_ActOut_2
													         ELSE 
							                                    ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
							                                    WHERE 
							                                    Tel_IDNo = LEDGER.Ttr_IDNo  
							                                    AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
							                                    AND CONVERT(INT,Tel_LogTime) >= CONVERT(INT,SHIFT.Msh_ShiftIn2)
							                                    AND Tel_LogTime > Ttr_ActIn_2
							                                    AND Tel_LogTime > Ttr_ActOut_1
							                                    AND Tel_LogType = 'O'
							                                    ORDER BY Tel_LogTime {3}
							                                    ),'0000')
						                                    END) [OUT2]
					                                    FROM {0}.[dbo].[T_EmpTimeRegister] LEDGER
					                                    JOIN 
					                                    {0}.[dbo].M_Shift SHIFT
					                                    ON Msh_ShiftCode=Ttr_ShiftCode
													    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
													    AND CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
													    AND(Ttr_ActIn_1 = '0000' OR Ttr_ActOut_2 = '0000')

													    --FILTERING
													    AND
													    (CONVERT(INT,(SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
																    WHERE 
																    Tel_IDNo = LEDGER.Ttr_IDNo
																    AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
																	    AND CONVERT(INT,Tel_LogTime) 
																	    >= CONVERT(INT,SHIFT.Msh_ShiftIn2)
																    AND Tel_LogType = 'O'
																    ORDER BY Tel_LogTime DESC))>0
													     OR
													     CONVERT(INT,(SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
																    WHERE 
																    Tel_IDNo = LEDGER.Ttr_IDNo 
																    AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
																	    AND CONVERT(INT,Tel_LogTime) 
																	    < CONVERT(INT,SHIFT.Msh_ShiftOut1)
																    AND Tel_LogType = 'I' ORDER BY Tel_LogTime ASC)) > 0
													     )";

        #endregion

        #region RecoveryGraveYardCleaningIn
        /// <summary>
        ///Arguments [LEDGERDB][DTRDB][RULED_IN][RULE_OUT][DTRTABLE]
        /// </summary>
        private static string QueryRecoveryGraveYardCleaningIn = @"----//----REMOVE IN1 POSTED ON CURRENT DAY LOGS FROM NEXT DAY
                                                    ---//-----COMMON SCENARIO GRAVEYARD RESTDAY
                                                    SELECT 
														Ttr_IDNo [EmployeeID]
													   ,@ProcessDate [ProcessDate]
                                                       ,CASE WHEN Ttr_ActIn_1 > 1300 THEN ISNULL((SELECT TOP 1 Tel_LogTime FROM {1}.dbo.{4} 
					                                        WHERE CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
					                                        AND Tel_IDNo=Ttr_IDNo
					                                        AND Tel_LogTime>1200
					                                        AND Tel_LogTime<=2200
					                                        AND Tel_LogType='I' ORDER BY Tel_LogTime DESC),'0000')
				                                        ELSE '0000' END [IN1]
				                                        ,Ttr_ActOut_1 [OUT1]
													    ,Ttr_ActIn_2 [IN2]
													    ,Ttr_ActOut_2 [OUT2]
                                                    FROM {0}.dbo.T_EmpTimeRegister
                                                         JOIN 
                                                        {0}.dbo.M_Shift SHIFT
                                                        ON Msh_ShiftCode=Ttr_ShiftCode
                                                    AND (Ttr_Date between @ProcessDate and @ProcessDate and Msh_ShiftIn1>Msh_ShiftOut2) 
                                                    AND (
                                                    (((SELECT TOP 1 Tel_IDNo FROM {1}.dbo.{4} 
                                                            WHERE CONVERT(DATETIME,Tel_LogDate) = Ttr_Date 
                                                            AND Tel_IDNo = Ttr_IDNo
                                                            AND Tel_LogTime = Ttr_ActIn_1
                                                            AND Tel_LogType = 'I') IS NULL AND Ttr_ActIn_1 <> '0000')
                                                        )
                                                    AND 
                                                        ((SELECT TOP 1 Tel_IDNo FROM {1}.dbo.{4} 
                                                            WHERE CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
                                                            AND Tel_IDNo = Ttr_IDNo
                                                            AND Tel_LogTime = Ttr_ActIn_1
                                                            AND Tel_LogType = 'I') IS NOT NULL AND Ttr_ActIn_1 <> '0000')
                                                    )";

        #endregion

        #region RecoveryGraveYardCleaningOut 
        /// <summary>
        ///Arguments [LEDGERDB][DTRDB][RULED_IN][RULE_OUT][DTRTABLE]
        /// </summary>
        private static string QueryRecoveryGraveYardCleaningOut = @"----//----REMOVE OUT2 POSTED ON CURRENT LOGS FROM NEXT DAY > 1200
                                                                    ---//-----COMMON SCENARIO GRAVEYARD RESTDAY
                                                                    SELECT 
                                                                        Ttr_IDNo [EmployeeID]
													                   ,@ProcessDate [ProcessDate]
                                                                       ,Ttr_ActIn_1 [IN1]
                                                                       ,Ttr_ActOut_1 [OUT1]
													                   ,Ttr_ActIn_2 [IN2]
													                   ,(CASE WHEN Ttr_ActOut_2 > 1300 THEN '0000' ELSE Ttr_ActOut_2 END) [OUT2]
                                                                    FROM {0}.dbo.T_EmpTimeRegister
                                                                        JOIN 
                                                                        {0}.dbo.M_Shift SHIFT
                                                                        ON Msh_ShiftCode=Ttr_ShiftCode
														                AND (Ttr_Date  between @ProcessDate and @ProcessDate and Msh_ShiftIn1 > Msh_ShiftOut2) 
														                AND (
														                (((SELECT TOP 1 Tel_IDNo FROM {1}.dbo.{4} 
																                WHERE CONVERT(DATETIME,Tel_LogDate) = Ttr_Date 
																                AND Tel_IDNo = Ttr_IDNo
																                AND TEL_LOGTIME = Ttr_ActIn_1
																                AND TEL_LogType = 'O') IS NULL AND Ttr_ActIn_1 <> '0000')
															                )
														                AND 
															                ((SELECT TOP 1 Tel_IDNo FROM {1}.dbo.{4} 
																                WHERE CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
																                AND Tel_IDNo = Ttr_IDNo
																                AND TEL_LOGTIME = Ttr_ActIn_1
																                AND TEL_LogType = 'O') IS NOT NULL AND Ttr_ActIn_1 <> '0000')
														                )
														                AND Ttr_ActOut_2 <> '0000'
														                AND Ttr_ActOut_2 > 1300";

        #endregion

        #region RecoveryGraveYardShift
        /// <summary>
        ///Arguments [LEDGERDB][DTRDB][RULED_IN][RULE_OUT][DTRTABLE]
        /// </summary>
        private static string QueryRecoveryGraveYardShift = @"SELECT 
                                                      
                                                      Ttr_IDNo [EmployeeID]
													  ,@ProcessDate [ProcessDate]
                                                      ----//---SETTING IN1
                                                     
                                                      ,CASE when Ttr_ActIn_1 <> '0000' then Ttr_ActIn_1
                                                      ELSE 
			                                                ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
                                                            WHERE 
                                                            Tel_IDNo=LEDGER.Ttr_IDNo 
                                                            AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
                                                            AND CONVERT(INT,TEL_LOGTIME) < 2300
                                                            AND TEL_LOGTIME>1200 ----//--IN1 IS PM
                                                            AND TEL_LogType='I' ORDER BY TEL_LOGTIME 
                                                            {2}
                                                            ),'0000')
                                                       END [IN1]
                                                       
                                                       
                                                      ,Ttr_ActOut_1 [OUT1]
													  ,Ttr_ActIn_2 [IN2]
													   
                                                      ----//---SETTING OUT2
                                                      
                                                      ,CASE when Ttr_ActOut_2 <> '0000' then Ttr_ActOut_2
                                                      ELSE 
			                                                ISNULL((SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
                                                            WHERE 
                                                            Tel_IDNo=LEDGER.Ttr_IDNo 
                                                            AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
                                                            AND CONVERT(INT,TEL_LOGTIME) >= 0200
                                                            AND TEL_LOGTIME<1200 ----//OUT2 IS AM
                                                            AND TEL_LogType='O'
                                                            ORDER BY TEL_LOGTIME 
                                                            {3}
                                                            ),'0000')
                                                       END [OUT2]
                                                  FROM {0}.[dbo].[T_EmpTimeRegister] LEDGER
                                                    JOIN 
                                                    {0}.dbo.M_Shift SHIFT
                                                    ON Msh_ShiftCode=Ttr_ShiftCode

													AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
													AND
													CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
													AND (Ttr_ActIn_1='0000' OR Ttr_ActOut_2='0000')
													AND
													--FILTERING--
													(CONVERT(INT,(SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
																WHERE 
																Tel_IDNo=LEDGER.Ttr_IDNo 
																AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
																	AND CONVERT(INT,TEL_LOGTIME) 
																	< CONVERT(INT,SHIFT.Msh_ShiftIn1)
																AND TEL_LogType='I' ORDER BY TEL_LOGTIME ASC))>0
													OR
													CONVERT(INT,(SELECT TOP 1 TEL_LOGTIME FROM {1}.dbo.{4} 
																WHERE 
																Tel_IDNo=LEDGER.Ttr_IDNo 
																AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
																AND CONVERT(INT,TEL_LOGTIME) >= 0200
																AND TEL_LOGTIME<1200 ----//OUT2 IS AM
																AND TEL_LogType='O'
																ORDER BY TEL_LOGTIME DESC))>0)";

        #endregion

        #region IntellegentPostingRegShiftwithGraveLogs
        /// <summary>
        /// Arguments [DTRDB][LEDGERDB][DTRTABLE]
        /// </summary>
        private static string QueryIntellegentPostingRegShiftwithGraveLogs = @" ------***********************************************
                                                                                ------INTELLEGENT POSTING REG SHIFT HAVING GRAVE LOGS
                                                                                ------***********************************************
                                                                                SELECT Ttr_IDNo [EmployeeID]
                                                                                      ,@ProcessDate [ProcessDate]
                                                                                      ----//----SETTING IN1
                                                                                      ----//---- IN DURING CURRENT DATE
                                                                                      ,CASE WHEN Ttr_ActIn_1 <> '0000' THEN Ttr_ActIn_1
                                                                                       ELSE ISNULL((SELECT MAX(TEL_LOGTIME) FROM {0}.dbo.{2} 
                                                                                                     WHERE Tel_IDNo = LEDGER.Ttr_IDNo 
                                                                                                       AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
                                                                                                       AND TEL_LogType = 'I'
                                                                                                       AND TEL_LOGTIME > 1200),'0000') END [IN1]
                                                                                      ---//----CLEAN Between Logs
                                                                                      ,'0000' [OUT1]
                                                                                      ,'0000' [IN2]
                                                                                      ----//----OUT ON NEXT DAY
                                                                                      ,CASE WHEN Ttr_ActOut_2 <> '0000' then Ttr_ActOut_2
                                                                                       ELSE ISNULL((SELECT MIN(TEL_LOGTIME) FROM {0}.dbo.{2} 
				                                                                                     WHERE Tel_IDNo = LEDGER.Ttr_IDNo 
				                                                                                       AND CONVERT(DATETIME,Tel_LogDate) = DATEADD(DD,1,LEDGER.Ttr_Date)
                                                                                                       AND TEL_LogType = 'O'
                                                                                                       AND TEL_LOGTIME < 1200),'0000') END [OUT2]
                                                                                      FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
                                                                                      JOIN {1}.dbo.M_Shift SHIFT
                                                                                        ON Msh_ShiftCode = Ttr_ShiftCode
                                                                                       AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
                                                                                       AND CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
                                                                                       AND Ttr_ActIn_1 = '0000'
                                                                                       AND Ttr_ActOut_2 = '0000'
    
                                                                                     AND	
    
	                                                                                 ---------//In of current date is greater than the last out of the current date
    
                                                                                     (
                                                                                     ISNULL((SELECT MAX(TEL_LOGTIME) FROM {0}.dbo.{2} WHERE Tel_IDNo = LEDGER.Ttr_IDNo AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date AND TEL_LogType = 'I' AND TEL_LOGTIME > 1200),'1200')
                                                                                     >
                                                                                     ISNULL((SELECT MAX(TEL_LOGTIME) FROM {0}.dbo.{2} WHERE Tel_IDNo = LEDGER.Ttr_IDNo AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date AND TEL_LogType = 'O' AND TEL_LOGTIME > 1200),'1200')
	                                                                                 )	   
    
                                                                                     AND 
                                                                                     -------//---Out of Next Day is less than the Min In of the next day
                                                                                     (
                                                                                     ISNULL((SELECT MAX(TEL_LOGTIME) FROM {0}.dbo.{2} WHERE Tel_IDNo = LEDGER.Ttr_IDNo AND CONVERT(DATETIME,Tel_LogDate) = DATEADD(DD, 1, LEDGER.Ttr_Date) AND TEL_LogType = 'O' AND TEL_LOGTIME < 1200),'1200')
                                                                                     < 
                                                                                     ISNULL((SELECT MIN(TEL_LOGTIME) FROM {0}.dbo.{2} WHERE Tel_IDNo = LEDGER.Ttr_IDNo AND CONVERT(DATETIME,Tel_LogDate) = DATEADD(DD, 1, LEDGER.Ttr_Date) AND TEL_LogType = 'I' AND TEL_LOGTIME < 1200),'1200')
                                                                                     )
	                                                                                 ORDER BY IN1 DESC, OUT2 DESC";

        #endregion

        #region IntellegentPostingGraveShiftwithRegLogs
        /// <summary>
        ///  Arguments [DTRDB][LEDGERDB][DTRTABLE]
        /// </summary>
        private static string QueryIntellegentPostingGraveShiftwithRegLogs = @" ------***********************************************
                                                                                ------INTELLEGENT POSTING GRAVE SHIFT HAVING REG LOGS
                                                                                ------***********************************************
                                                                                SELECT Ttr_IDNo [EmployeeID]
                                                                                      ,@ProcessDate [ProcessDate]
                                                                                      --SETTING IN1
                                                                                      ,CASE when Ttr_ActIn_1 <> '0000' then Ttr_ActIn_1
                                                                                       ELSE ISNULL((SELECT MIN(TEL_LOGTIME) FROM {0}.dbo.{2} 
                                                                                                     WHERE Tel_IDNo = LEDGER.Ttr_IDNo 
 					                                                                                   AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
                                                                                                       AND CONVERT(INT,TEL_LOGTIME) < 1200
                                                                                                       AND TEL_LogType = 'I'),'0000') END [IN1]


                                                                                      ---//----CLEAN Between Logs
                                                                                      ,'0000' [OUT1]
                                                                                      ,'0000' [IN2]

                                                                                      -- SETTING OUT2
	                                                                                  ,CASE WHEN Ttr_ActOut_2 <> '0000' then Ttr_ActOut_2
                                                                                       ELSE 
                                                                                       ISNULL((SELECT MAX(TEL_LOGTIME) FROM {0}.dbo.{2} 
                                                                                                WHERE Tel_IDNo = LEDGER.Ttr_IDNo 
                                                                                                  AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date
                                                                                                  AND CONVERT(INT,TEL_LOGTIME) > 1200
                                                                                                  AND TEL_LogType = 'O'),'0000') END [OUT2]
                  
                                                                                 FROM {1}.[dbo].[T_EmpTimeRegister] LEDGER
                                                                                 JOIN {1}.dbo.M_Shift SHIFT
                                                                                   ON Msh_ShiftCode = Ttr_ShiftCode
                                                                                  AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
                                                                                  AND CONVERT(INT,Msh_ShiftIn1) > CONVERT(INT,Msh_ShiftOut2)
                                                                                  AND Ttr_ActIn_1 = '0000' 
                                                                                  AND Ttr_ActOut_2 = '0000'
                                                                                  AND
                                                                                  ----//There must have a pair logs with in the day
                                                                                 (CONVERT(INT,(SELECT MIN(TEL_LOGTIME) FROM {0}.dbo.{2} WHERE Tel_IDNo = LEDGER.Ttr_IDNo AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date AND CONVERT(INT,TEL_LOGTIME) < 1200 AND TEL_LogType = 'I')) > 0)
                                                                                  AND 
                                                                                 (CONVERT(INT,(SELECT MAX(TEL_LOGTIME) FROM {0}.dbo.{2} WHERE Tel_IDNo = LEDGER.Ttr_IDNo AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date AND CONVERT(INT,TEL_LOGTIME) > 1200 AND TEL_LogType = 'O')) > 0)";

        #endregion

        #region LogRecheckOverloadCleanup
        /// <summary>
        /// Arguments [DTRDB][LEDGERDB][RegularInverseQuery][GraveInverseQuery][DTRTABLE]
        /// </summary>
        private static string QueryLogRecheckOverloadCleanup = @"---REG Invers Posting Here
                                                                {2}
  
                                                                ------- CLEANING IN1 OUT2 GRAVE YARD SHIFT HAVING REG LOG POST BUT NOT EXISTING

                                                                SELECT
												                         Ttr_IDNo [EmployeeID]
													                    ,@ProcessDate [ProcessDate]
	                                                                    ,'0000' [IN1]
	                                                                    ,Ttr_ActOut_1 [OUT1]
		                                                                ,Ttr_ActIn_2 [IN2]
		                                                                ,'0000' [OUT2]
                                                                FROM {1}.[dbo].[T_EmpTimeRegister] LEDGER
                                                                    JOIN 
                                                                    {1}..M_Shift SHIFT
                                                                    ON Msh_ShiftCode = Ttr_ShiftCode
                                                                    AND CONVERT(INT,Msh_ShiftIn1) > CONVERT(INT,Msh_ShiftOut2)
                                                                    AND Ttr_ActIn_1 < Ttr_ActOut_2
		                                                            AND (Ttr_ActIn_1 < 1100 AND Ttr_ActIn_1 <> '0000')
	                                                                AND Ttr_ActOut_2 > 1400
	                                                                AND Ttr_Date = @ProcessDate
	                                                                AND (SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{4} 
							                                                                WHERE Tel_LogDate = Ttr_Date
							                                                                AND Ttr_ActIn_1 = TEL_LOGTIME
							                                                                AND Ttr_IDNo = Tel_IDNo
							                                                                AND TEL_LogType = 'I') IS NULL
	                                                                AND (SELECT TOP 1 TEL_LOGTIME FROM {0}.dbo.{4} 
							                                                                WHERE Tel_LogDate = Ttr_Date
							                                                                AND Ttr_ActIn_1 = TEL_LOGTIME
							                                                                AND Ttr_IDNo = Tel_IDNo
							                                                                AND TEL_LogType = 'O') IS NULL

                                                                ------------------------------------------------------------------------------
                                                                --------REVERSE CLEANING Transfer OUT1 to OUT2 LOGS OF GRAVE SHIFT WITH REG LOGS --FOR HOYA CLOSING
                                                                ------------------------------------------------------------------------------
                                                                SELECT
													                    Ttr_IDNo [EmployeeID]
													                    ,@ProcessDate [ProcessDate]
	                                                                    ,Ttr_ActIn_1 [IN1]
	                                                                    ,'0000' [OUT1]
		                                                                ,Ttr_ActIn_2 [IN2]
		                                                                ,Ttr_ActOut_1 [OUT2] --Transferring OUT1 -> OUT2
                                                                  FROM {1}.[dbo].[T_EmpTimeRegister]
	                                                                JOIN 
	                                                                {1}.[dbo].M_Shift
	                                                                ON Msh_ShiftCode = Ttr_ShiftCode
                                                                    AND  Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
												                    AND
												                    CONVERT(INT,Msh_ShiftIn1) > CONVERT(INT,Msh_ShiftOut2)
												                    and convert(int,Ttr_ActIn_1) < 1000
												                    and Ttr_ActIn_1 <> '0000'
												                    and convert(int,Ttr_ActOut_1) > 1300
												                    and Ttr_ActOut_1 <> '0000'
                                            

                                                                ----Grave Shift Inverse Posting Here
                                                                {3}                                            
                                                                

                                                                ---------********************************
                                                                ---------START CLEANING UP OF DIRTY LOGS
                                                                ---------********************************
                                                                -----CLEAN UP DIRTY GRAVE LOGS WHICH IN1 BECAME  =  TO OUT2 [REMOVE IN1]
                                                                SELECT
                                            
												                     Ttr_IDNo [EmployeeID]
												                    ,@ProcessDate [ProcessDate]
		                                                            ,(isnull((SELECT TOP 1 TEL_LOGTIME FROM {0}..{4} 
			                                                            WHERE 
			                                                            Tel_IDNo = LEDGER.Ttr_IDNo 
			                                                            AND CONVERT(DATETIME,Tel_LogDate) = LEDGER.Ttr_Date 
				                                                            AND CONVERT(INT,TEL_LOGTIME) 
				                                                             <  CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                                            AND TEL_LogType = 'I' ORDER BY TEL_LOGTIME DESC),'0000')) [IN1]
			                                                         ,Ttr_ActOut_1 [OUT1]
	                                                                 ,Ttr_ActIn_2 [IN2]
	                                                                 ,Ttr_ActOut_2 [OUT2]
	                                             
                                                              FROM {1}.DBO.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.DBO.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode
											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) > CONVERT(INT,Msh_ShiftOut2)
											                    AND CONVERT(INT,Ttr_ActIn_1) < 1000
											                    and Ttr_ActIn_1 <> '0000'
											                    and  Ttr_ActIn_1 = Ttr_ActOut_2

                                                            ---CLEANING DIRTY LOGS FOR REG SHIFT HAVING SAME IN1 && OUT2 [REMOVE IN1]
                                                            SELECT 
												                    Ttr_IDNo [EmployeeID]
												                    ,@ProcessDate [ProcessDate]
		                                                            ,'0000' [IN1]
			                                                        ,Ttr_ActOut_1 [OUT1]
	                                                                ,Ttr_ActIn_2 [IN2]
	                                                                ,Ttr_ActOut_2 [OUT2]
	                                             
                                                              FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.dbo.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode
											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
											                    and Ttr_ActIn_1 <> '0000'
											                    and Ttr_ActIn_1 = Ttr_ActOut_2

                                                            ---CLEANING DIRTY LOGS FOR REG SHIFT HAVING SAME IN1 && OUT1 [REMOVE OUT1]
                                                            SELECT 
											
												                    Ttr_IDNo [EmployeeID]
												                    ,@ProcessDate [ProcessDate]
		                                                            ,Ttr_ActIn_1 [IN1]
			                                                        ,'0000' [OUT1]
	                                                                ,Ttr_ActIn_2 [IN2]
	                                                                ,Ttr_ActOut_2 [OUT2]
	                                            
                                                              FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.dbo.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode
											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND
											                    CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
											                    AND CONVERT(INT,Ttr_ActOut_1) <= CONVERT(INT,Msh_ShiftIn2)
											                    and Ttr_ActIn_1 <> '0000'
											                    and Ttr_ActIn_1 = Ttr_ActOut_1
    
                                                            ----CLEANING DIRTY LOGS FOR REG SHIFT HAVING IN2  =  OUT2 [REMOVE OUT2]
                                                            SELECT 
												                    Ttr_IDNo [EmployeeID]
												                    ,@ProcessDate [ProcessDate]
		                                                            ,Ttr_ActIn_1 [IN1]
			                                                        ,Ttr_ActOut_1 [OUT1]
	                                                                ,Ttr_ActIn_2 [IN2]
	                                                                ,'0000' [OUT2]
	                                            
                                                              FROM {1}.DBO.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.DBO.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode
											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
											                    AND CONVERT(INT,Ttr_ActIn_2) >  = CONVERT(INT,Msh_ShiftOut1)
											                    and Ttr_ActIn_2 <> '0000'
											                    and Ttr_ActIn_2 = Ttr_ActOut_2

                                                            ---CLEANING THE LOGS REG SHIFT HAVING OUT1 = OUT2 [Checking OUT1 and OUT2]
                                                            SELECT
												                    Ttr_IDNo [EmployeeID]
												                    ,@ProcessDate [ProcessDate]
		                                                            ,Ttr_ActIn_1 [IN1]
			                                                        ,(CASE WHEN Ttr_ActOut_1 > Msh_ShiftIn2 THEN '0000' ELSE Ttr_ActOut_1 END) [OUT1]
	                                                                ,Ttr_ActIn_2 [IN2]
	                                                                ,(CASE WHEN Ttr_ActOut_2 <  = Msh_ShiftIn2 THEN '0000' ELSE Ttr_ActOut_2 END) [OUT2]
	                                            
	                                                        FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.dbo.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode
											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
											                    and Ttr_ActOut_1 <> '0000'
											                    and Ttr_ActOut_1 = Ttr_ActOut_2

                                                            ----CLEANING LOGS HAVING GRAVE SHIFT GET NEXT DAY OUT OF REG SHIFT [Clean OUT2]
                                                            SELECT
                                        
											                    Ttr_IDNo [EmployeeID]
											                    ,@ProcessDate [ProcessDate]
	                                                            ,Ttr_ActIn_1 [IN1]
		                                                        ,Ttr_ActOut_1 [OUT1]
                                                                ,Ttr_ActIn_2 [IN2]
                                                                ,'0000' [OUT2]
                                            
                                                            FROM {1}.DBO.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.DBO.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode

											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) > CONVERT(INT,Msh_ShiftOut2)
											                    and Ttr_ActIn_1 = '0000'
											                    and Ttr_ActOut_2 <> '0000'
											                    and Ttr_ActOut_2 >  = 1400                             

                                                            -----CLEANING LOGS HAVING REG SHIFT WHERE EQUAL IN1 && IN2 [Check IN1 and IN2]

                                                            SELECT
                                        
											                    Ttr_IDNo [EmployeeID]
											                    ,@ProcessDate [ProcessDate]
	                                                            ,(CASE WHEN Ttr_ActIn_1 >  = Msh_ShiftOut1 THEN '0000' ELSE Ttr_ActIn_1 END) [IN1]
		                                                        ,Ttr_ActOut_1 [OUT1]
                                                                ,(CASE WHEN Ttr_ActIn_2 < Msh_ShiftOut1 THEN '0000' ELSE Ttr_ActIn_2 END) [IN2]
                                                                ,Ttr_ActOut_2 [OUT2]
	                                            
	                                                        FROM {1}.DBO.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.DBO.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode

											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) < CONVERT(INT,Msh_ShiftOut2)
											                    and Ttr_ActIn_1 <> '0000'
											                    and Ttr_ActIn_1 = Ttr_ActIn_2

                                                            ------CLEANING GRAVE SHIFT HAVING SAME OUT1 && OUT2 [Checking OUT1 OUT2]
                                                            SELECT 
                                        
											                    Ttr_IDNo [EmployeeID]
											                    ,@ProcessDate [ProcessDate]
	                                                            ,Ttr_ActIn_1 [IN1]
		                                                        ,(CASE WHEN Ttr_ActOut_1 <  = Msh_ShiftIn2 THEN Ttr_ActOut_1 ELSE '0000' END) [OUT1]
                                                                ,Ttr_ActIn_2 [IN2]
                                                                ,(CASE WHEN Ttr_ActOut_2 <  = Msh_ShiftOut1 THEN '0000' ELSE Ttr_ActOut_2 END) [OUT2]
                                        	
	                                                         FROM {1}.dbo.[T_EmpTimeRegister] LEDGER
	                                                            JOIN 
	                                                            {1}.dbo.M_Shift SHIFT
	                                                            ON Msh_ShiftCode = Ttr_ShiftCode

											                    AND Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
											                    AND CONVERT(INT,Msh_ShiftIn1) > CONVERT(INT,Msh_ShiftOut2)
											                    and Ttr_ActOut_1 <> '0000'
											                    and Ttr_ActOut_1 = Ttr_ActOut_2

                                                            ----CLEANING OUT1 OF REG SHIFT WITH GRAVE ON PREVIOUS 
                                        
                                                            SELECT
                                        
											                    Ttr_IDNo [EmployeeID]
											                    ,@ProcessDate [ProcessDate]
	                                                            ,Ttr_ActIn_1 [IN1]
		                                                        ,'0000' [OUT1]
                                                                ,Ttr_ActIn_2 [IN2]
                                                                ,Ttr_ActOut_2 [OUT2]
                                            
                                                            from {1}.dbo.T_EmpTimeRegister Ledger
                                                            join {1}.dbo.M_Shift
	                                                            on Ttr_ShiftCode = Msh_ShiftCode
	                                                            and Msh_ShiftIn1 < Msh_ShiftOut2
											                    AND Ttr_ActOut_1 <> '0000'
											                    and Ttr_ActOut_1 < 900
											                    AND Ttr_Date = @ProcessDate
											                    AND Ttr_ActIn_1 = '0000'
											                    and Ttr_ActOut_2 = '0000'
											                    and Ttr_ActOut_1 = (select Ttr_ActOut_2 from {1}.dbo.T_EmpTimeRegister 
																	                    where Ttr_ActOut_2 = Ledger.Ttr_ActOut_1
																	                    and DATEADD(DD,-1,@ProcessDate) = Ttr_Date
																	                    and Ttr_IDNo = Ledger.Ttr_IDNo
																	                    and Ttr_ActIn_1 > 1200)

                                                            ----//----REMOVE OUT2 WHERE IN1 AND OUT ARE  >  1300
                                                            SELECT
											
											                    Ttr_IDNo [EmployeeID]
											                    ,@ProcessDate [ProcessDate]
	                                                            ,Ttr_ActIn_1 [IN1]
		                                                        ,Ttr_ActOut_1 [OUT1]
                                                                ,Ttr_ActIn_2 [IN2]
                                                                ,'0000' [OUT2]
                                        
                                                            FROM {1}.dbo.T_EmpTimeRegister
	                                                            JOIN 
                                                                {1}.dbo.M_Shift SHIFT
                                                                ON Msh_ShiftCode = Ttr_ShiftCode
											                    AND (Ttr_Date  between @ProcessDate and @ProcessDate and
											                    Msh_ShiftIn1 > Msh_ShiftOut2) 
											                    AND Ttr_ActOut_2 <> '0000'
											                    AND Ttr_ActOut_2 > 1300
											                    AND Ttr_ActIn_1 > 1300";

        #endregion

        #region OvertimeAfterMidnightPlus24
        /// <summary>
        ///  Argurments [LEDGERDB][LEDGERTABLE] LedgerDBName, Globals.T_Ledger
        /// </summary>
        private static string QueryOvertimeAfterMidnightPlus24 = @"SELECT 
										                                Ttr_IDNo [EmployeeID]
										                            ,@ProcessDate [ProcessDate]
										                            ,Ttr_ActIn_1 [IN1]
	                                                                ,Ttr_ActOut_1 [OUT1]
                                                                    ,Ttr_ActIn_2 [IN2]
                                                                    ,CONVERT(INT,Ttr_ActOut_2)+2400 [OUT2] --+24 for OUT2
                                                                FROM 
                                                                {0}..{1} LEDGER
                                                                JOIN
                                                                {0}..M_Shift
                                                                ON Ttr_ShiftCode=Msh_ShiftCode 
                                                                AND CONVERT(INT,Ttr_ActOut_2)<CONVERT(INT,Msh_ShiftIn1)
                                                                AND CONVERT(INT,Ttr_ActOut_2)!=0
                                                                AND CONVERT(INT,Ttr_ActOut_2)<0600
                                                                AND CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)
                                                                AND (CONVERT(BIGINT,Ttr_ActIn_2+Ttr_ActIn_1)!=0 --Must have IN1 or IN2
                                                                        AND 
                                                                        CONVERT(INT,Ttr_ActIn_1)<1400)
                                                                AND Ttr_Date=@ProcessDate";

        #endregion

        #region RepostLogTrail

        private string QueryRepostLogTrail = @"SELECT
													    Ttr_IDNo [EmployeeID]
													  , Ttr_Date [ProcessDate]	
		                                              , (CASE WHEN Ttl_ActIn_1 <> '0000' THEN Ttl_ActIn_1 ELSE Ttr_ActIn_1 END) [IN1]
	                                                  , Ttl_ActOut_1 [OUT1]
	                                                  , Ttl_ActIn_2 [IN2]
	                                                  , (CASE WHEN Ttl_ActOut_2 <> '0000' THEN Ttl_ActOut_2 ELSE Ttr_ActOut_2 END) [OUT2]
                                                FROM {0}.dbo.T_EmpTimeRegister
                                                JOIN {0}.dbo.T_EmpTimeRegisterLog A
	                                                ON Ttr_IDNo  =  Ttl_IDNo
                                                    {1} -- For Multiple Pocket
                                                    AND Ttr_Date  BETWEEN dateadd(DD, -1, @ProcessDate) AND  @ProcessDate
                                                    AND Ttr_Date  =  Ttl_Date
                                                    AND (Ttr_ActIn_1 <> Ttl_ActIn_1
		                                                OR Ttr_ActOut_1 <> Ttl_ActOut_1
		                                                OR Ttr_ActIn_2 <> Ttl_ActIn_2
		                                                OR Ttr_ActOut_2 <> Ttl_ActOut_2)
                                                    AND (Ttl_ActIn_1 <> '0000'
		                                                OR Ttl_ActOut_1 <> '0000'
		                                                OR Ttl_ActIn_2 <> '0000'
		                                                OR Ttl_ActOut_2 <> '0000')
													AND Ttl_LineNo  =  (SELECT MAX(Ttl_LineNo) FROM {0}.dbo.T_EmpTimeRegisterLog 
					                                                WHERE A.Ttl_IDNo  =  Ttl_IDNo
						                                                  AND A.Ttl_Date  =  Ttl_Date)";

        #endregion

        #endregion

        #region Log uploading manipulate shift queries and dataset

        private DataSet DsManipulateEquivalentShiftHolidayRestday = new DataSet();
        private DataSet DsManipulateDefaultShiftCPHandDenso = new DataSet();
        private DataSet DsFlexShifting = new DataSet();
        private DataSet DsManipulateShiftwithShiftMovementDenso = new DataSet();

        #region ManipulateEquivalentShiftHolidayRestday
        /// <summary>
        /// Arguments [LEDGERDB][LEDGERTABLE]
        /// </summary>
        private static string QueryManipulateEquivalentShiftHolidayRestday = @"SELECT
																	             Ttr_IDNo [EmployeeID]
																	            ,@ProcessDate [ProcessDate]
																	            ,(CASE 
																		            WHEN Ttr_RestDayFlag = 1 OR Ttr_HolidayFlag = 1 THEN 
																			            isnull((SELECT [Msh_8HourShiftCode] FROM [{0}].[dbo].M_Shift 
																				            WHERE 
																				            Msh_ShiftCode = Ttr_ShiftCode
																				            and Msh_Schedule = Ttr_ScheduleType), Ttr_ShiftCode)
																		            ELSE Ttr_ShiftCode END) [ShiftCode]
			                                                                FROM [{0}].[dbo].[{1}] 
                                                                            WHERE 
                                                                            Ttr_Date = @ProcessDate
                                                                            AND 
                                                                            (Ttr_RestDayFlag = 1 OR Ttr_HolidayFlag = 1)
                                                                            AND (CASE 
																		            WHEN Ttr_RestDayFlag = 1 OR Ttr_HolidayFlag = 1 THEN 
																			            isnull((SELECT [Msh_8HourShiftCode] FROM [{0}].[dbo].M_Shift 
																				            WHERE 
																				            Msh_ShiftCode = Ttr_ShiftCode
																				            and Msh_Schedule = Ttr_ScheduleType), Ttr_ShiftCode)
																		            ELSE Ttr_ShiftCode END) <> Ttr_ShiftCode";

        #endregion

        #region ManipulateDefualtShifWednesdayAndLeave
        /// <summary>
        /// Arguments [LEDGERDB][LEDGERTABLE]
        /// </summary>
        private static string QueryManipulateDefaultShiftWednesdayAndLeaveCPH = @"SELECT
														                         Ttr_IDNo [EmployeeID]
														                        ,@ProcessDate [ProcessDate]
														                        ,(CASE
															                        WHEN 
															                        (CONVERT(FLOAT,Ttr_WFNoPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr) > 0 AND Ttr_DayCode = 'REG') 
															                        OR 
															                        (datepart(WEEKDAY,Ttr_Date) = 4 and Ttr_HolidayFlag <> 1 and Ttr_RestDayFlag <> 1)
															                        THEN
																                        isnull((SELECT [Msh_ShiftCode] FROM [{0}].[dbo].M_Shift 
																	                        WHERE 
																	                        Msh_IsDefaultShift  =  'TRUE'
																	                        and Msh_Schedule  =  Ttr_ScheduleType),Ttr_ShiftCode)
															                        ELSE Ttr_ShiftCode
															                        END) [ShiftCode] -- wednesday or leave
			                                                                FROM  [{0}].[dbo].[{1}]
                                                                            WHERE 
                                                                            Ttr_Date  =  @ProcessDate
                                                                            AND
                                                                            (
														                        (CONVERT(FLOAT,Ttr_WFNoPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr) > 0 AND Ttr_DayCode = 'REG') 
														                        OR 
														                        (datepart(WEEKDAY,Ttr_Date) = 4 and Ttr_HolidayFlag <> 1 and Ttr_RestDayFlag <> 1)
			                                                                )
			                                                                AND (CASE
															                        WHEN 
															                        (CONVERT(FLOAT,Ttr_WFNoPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr) > 0 AND Ttr_DayCode = 'REG') 
															                        OR 
															                        (datepart(WEEKDAY,Ttr_Date) = 4 and Ttr_HolidayFlag <> 1 and Ttr_RestDayFlag <> 1)
															                        THEN
																                        isnull((SELECT [Msh_ShiftCode] FROM [{0}].[dbo].M_Shift 
																	                        WHERE 
																	                        Msh_IsDefaultShift  =  'TRUE'
																	                        and Msh_Schedule  =  Ttr_ScheduleType),Ttr_ShiftCode)
															                        ELSE Ttr_ShiftCode
															                        END) <> Ttr_ShiftCode";

        #endregion

        #region ManipulateDefaultShiftOvertimeAndLeaveDenso
        /// <summary>
        /// Arguments [LEDGERDB][LEDGERTABLE]
        /// </summary>
        private string QueryManipulateDefaultShiftOvertimeAndLeaveDenso = @"SELECT
														                     Ttr_IDNo [EmployeeID]
														                    ,@ProcessDate [ProcessDate]
														                    ,(CASE
														                    WHEN 
														                    (CONVERT(FLOAT,Ttr_WFNoPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr) > 0 AND Ttr_DayCode = 'REG') 
														                    OR
														                    Ttr_IDNo IN
																                    (SELECT DISTINCT Tot_IDNo FROM {0}.dbo.T_EmpOvertime
																                    WHERE Tot_OvertimeDate  =  @Processdate
																                    AND Tot_OvertimeType = 'A')
														                    AND
														                    (Ttr_HolidayFlag <> 1 and Ttr_RestDayFlag <> 1)
														                    THEN
															                    (SELECT [Msh_ShiftCode] FROM [{0}].[dbo].M_Shift 
																                    WHERE 
																                    Msh_IsDefaultShift  =  'TRUE'
																                    and Msh_Schedule  =  Ttr_ScheduleType)
														                    ELSE Ttr_ShiftCode
														                    END) [ShiftCode] -- Advance OT Type or leave
			                                                            FROM  [{0}].[dbo].[{1}] 
                                                                        WHERE 
                                                                        Ttr_Date  =  @ProcessDate
                                                                        AND
                                                                        (CASE
														                    WHEN 
														                    (CONVERT(FLOAT,Ttr_WFNoPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr) > 0 AND Ttr_DayCode = 'REG') 
														                    OR
														                    Ttr_IDNo IN
																                    (SELECT DISTINCT Tot_IDNo FROM {0}.dbo.T_EmpOvertime
																                    WHERE Tot_OvertimeDate  =  @Processdate
																                    AND Tot_OvertimeType = 'A')
														                    AND
														                    (Ttr_HolidayFlag <> 1 and Ttr_RestDayFlag <> 1)
														                    THEN
															                    (SELECT [Msh_ShiftCode] FROM [{0}].[dbo].M_Shift 
																                    WHERE 
																                    Msh_IsDefaultShift  =  'TRUE'
																                    and Msh_Schedule  =  Ttr_ScheduleType)
														                    ELSE Ttr_ShiftCode
														                    END) <> Ttr_ShiftCode";
        
        #endregion

        #region FlexShiftCPH
        /// <summary>
        ///Arguments [LEDGERDB][LEDGERTABLE][POSITIONEXCLUEDED]
        /// </summary>
        private static string QueryFlexShiftCPH = @"SELECT
															 Ttr_IDNo [EmployeeID]
															,@ProcessDate [ProcessDate]
															,			isnull((case when 
	                                                                    (SELECT TOP 1 [Msh_ShiftCode]
	                                                                    FROM {0}.dbo.[M_Shift] 
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)> = CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Msh_ShiftCode NOT LIKE 'R00%'
	                                                                    AND Ttr_ScheduleType = Msh_Schedule
	                                                                    AND Msh_RecordStatus  =  'A') 
	                                                                    IS null  then 
					                                                                    (select top 1 Msh_ShiftCode from {0}.dbo.M_Shift where Msh_Schedule = 'N' 
					                                                                    and Msh_ShiftCode not like 'R00%' 
					                                                                    and Msh_RecordStatus  =  'A'
					                                                                    order  by Msh_ShiftIn1 desc)
	                                                                    else
	                                                                    (SELECT TOP 1 [Msh_ShiftCode]
	                                                                    FROM {0}.dbo.[M_Shift] 
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)> = CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Ttr_ScheduleType = Msh_Schedule
	                                                                    AND Msh_RecordStatus  =  'A') END),Ttr_ShiftCode) [ShiftCode]
		                                            FROM {0}.dbo.{1}
                                                        JOIN {0}.dbo.M_Employee
                                                        ON Ttr_IDNo = Mem_IDNo
														AND Ttr_Date  =  @Processdate
														AND CONVERT(INT,Ttr_ActIn_1)>0
														AND Ttr_HolidayFlag! = 1 AND Ttr_RestDayFlag! = 1 AND DATEPART(WEEKDAY,Ttr_Date)! = 4
														AND (CONVERT(FLOAT,Ttr_WFNoPayLVHr)+CONVERT(FLOAT,Ttr_WFNoPayLVHr))< = 0
														AND isnull((case when 
	                                                                    (SELECT TOP 1 [Msh_ShiftCode]
	                                                                    FROM {0}.dbo.[M_Shift] 
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)> = CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Msh_ShiftCode NOT LIKE 'R00%'
	                                                                    AND Ttr_ScheduleType = Msh_Schedule
	                                                                    AND Msh_RecordStatus  =  'A') 
	                                                                    IS null  then 
					                                                                    (select top 1 Msh_ShiftCode from {0}.dbo.M_Shift where Msh_Schedule = 'N' 
					                                                                    and Msh_ShiftCode not like 'R00%' 
					                                                                    and Msh_RecordStatus  =  'A'
					                                                                    order  by Msh_ShiftIn1 desc)
	                                                                    else
	                                                                    (SELECT TOP 1 [Msh_ShiftCode]
	                                                                    FROM {0}.dbo.[M_Shift] 
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)> = CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Ttr_ScheduleType = Msh_Schedule
	                                                                    AND Msh_RecordStatus  =  'A') END),Ttr_ShiftCode) <> Ttr_ShiftCode                                                   

                                                                        {2} -- Position Exclusion(DRVR)";

        #endregion

        #region FlexShiftLear
        /// <summary>
        /// Arguments [LEDGERDB][LEDGERTABLE]
        /// Nilo Modified 03/23/2014 : Do not inclue with advance OT
        /// </summary>
        private static string QueryFlexShiftLear = @"   SELECT Ttr_IDNo [EmployeeID]
															        , @Processdate [ProcessDate]
															        , ISNULL((SELECT TOP 1 [Msh_ShiftCode]
	                                                                            FROM [{0}].dbo.[M_Shift] 
																			   WHERE Msh_ShiftIn1 >= Ttr_ActIn_1
																			     AND ISNULL(PARSENAME(REPLACE(Ttr_ShiftCode, '-', '.'),2),Ttr_ShiftCode) = ISNULL(PARSENAME(REPLACE(Msh_ShiftCode, '-', '.'),2),Msh_ShiftCode) 
																			     AND Ttr_ScheduleType = Msh_Schedule
																			     AND Msh_RecordStatus  =  'A'
																		    ORDER BY Msh_ShiftIn1 ASC), Ttr_ShiftCode) [ShiftCode]
														        FROM [{0}].dbo.[{1}]
                                                               WHERE Ttr_Date = @Processdate
														         AND Ttr_ActIn_1 <> '0000'
                                                                 AND Ttr_ShiftCode <> ISNULL((SELECT TOP 1 [Msh_ShiftCode]
	                                                                                            FROM [{0}].dbo.[M_Shift] 
																			                   WHERE Msh_ShiftIn1 >= Ttr_ActIn_1
																			                     AND ISNULL(PARSENAME(REPLACE(Ttr_ShiftCode, '-', '.'),2),Ttr_ShiftCode) = ISNULL(PARSENAME(REPLACE(Msh_ShiftCode, '-', '.'),2),Msh_ShiftCode) 
																			                     AND Ttr_ScheduleType = Msh_Schedule
																			                     AND Msh_RecordStatus  =  'A'
																		                    ORDER BY Msh_ShiftIn1 ASC), Ttr_ShiftCode)";

        #endregion

        #region FlexShiftDenso
        /// <summary>
        /// Arguments [LEDGERDB][LEDGERTABLE][EMPLOYMENTEXCLUDED]
        /// </summary>
        private static string QueryFlexShiftDenso = @"

                                    DECLARE @DEFAULTSHIFT as char(4) = 
								   (SELECT TOP 1 ISNULL(Msh_ShiftCode , 'D005')
										  FROM {0}.dbo.M_Shift 
										 WHERE Msh_Schedule = 'D' 
										   AND Msh_IsDefaultShift = 1
					                       AND Msh_RecordStatus = 'A')
													
													   SELECT
															 Ttr_IDNo [EmployeeID]
															,@ProcessDate [ProcessDate]
															,Ttr_ActIn_1
															,COALESCE((SELECT TOP 1 [Msh_ShiftCode]
																				FROM {0}.dbo.[M_Shift] 
																			   WHERE CONVERT(INT,Msh_ShiftIn1) >  = CONVERT(INT,Ttr_ActIn_1)
																				 AND Msh_ShiftCode NOT LIKE 'R00%'
																				 AND Ttr_ScheduleType = Msh_Schedule
																				 AND Msh_RecordStatus = 'A')
                                                                                ,(SELECT TOP 1 [Msh_ShiftCode]
																				FROM {0}.dbo.[M_Shift] 
																			   WHERE Msh_ShiftCode NOT LIKE 'R00%'
																				 AND Ttr_ScheduleType = Msh_Schedule
																				 AND Msh_RecordStatus = 'A'
                                                                                ORDER BY [Msh_ShiftCode] desc)
                                                                                ,@DEFAULTSHIFT)
															  [ShiftCode]
		                                            FROM {0}.dbo.{1} 
                                                        JOIN {0}.dbo.M_Employee
                                                        ON Ttr_IDNo = Mem_IDNo
														AND CONVERT(INT,Ttr_ActIn_1) > 0
														AND Ttr_HolidayFlag <>  1 
														AND Ttr_RestDayFlag <>  1
														AND (CONVERT(FLOAT,Ttr_WFPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr)) < = 0
														AND Ttr_Date = @Processdate
														AND Ttr_IDNo NOT IN
															(SELECT DISTINCT Tot_IDNo FROM {0}.dbo.T_EmpOvertime
															WHERE Tot_OvertimeDate = @Processdate
															AND Tot_OvertimeType = 'A')
                                                                  
                                                         {2} -- Emp Status Exclusion(ProB)";
        
        #endregion

        #region ManipulateShiftwithShiftMovementDenso

        private static string QueryManipulateShiftwithShiftMovementDenso = @"SELECT
														                         Ttr_IDNo [EmployeeID]
														                        ,@ProcessDate [ProcessDate]
														                        ,isnull(mve_to,Ttr_ShiftCode) [ShiftCode]
                                                                            FROM {0}.dbo.{1}
                                                                            JOIN {0}.dbo.T_MOVEMENT
	                                                                                ON [Mve_EmployeeId] = Ttr_IDNo
	                                                                                AND convert(varchar,[Mve_ApprovedDate],101) = @Processdate
	                                                                        JOIN {0}.dbo.M_Employee --set hrc
		                                                                                ON Ttr_IDNo = Mem_IDNo
			                                                                AND
			                                                                Ttr_HolidayFlag = 0 AND Ttr_RestDayFlag = 0
			                                                                AND (CONVERT(FLOAT,Ttr_WFNoPayLVHr) + CONVERT(FLOAT,Ttr_WFNoPayLVHr)) <= 0
			                                                                AND Ttr_Date = convert(varchar,[Mve_EffectivityDate],101)
			                                                                AND [Mve_Status] = 9
			                                                                AND Ttr_ShiftCode <> MVE_TO
                                                                            {2} -- Emp Status Exclusion(ProB)";

        #endregion

        #endregion

        #region Uploading V2 Methods Log Validations

        public string FormatCapturedOutOccurence(DateTime CapturedOUTOccurence)
        {
            string CapturedOUT;
            CapturedOUT = (Convert.ToInt16(CapturedOUTOccurence.ToString("HH"))).ToString("00") + (Convert.ToInt16(CapturedOUTOccurence.ToString("mm"))).ToString("00");
            return CapturedOUT;
        }

        

        public double GetTimeGap(string LedgerDbName, string LedgerCompanyName)
        {
            double INOUTTIMEGAP = 0;
            try
            {
                string sql = string.Format(@"SELECT [Mph_PolicyCode] as [TIMEGAP]
                                                   ,[Mph_NumValue] as [TIMEGAPVALUE]
                                               FROM {0}..[M_PolicyHdr]
                                              WHERE Mph_PolicyCode='TIMEGAP'
                                                ANd Mph_CompanyCode = '{1}'", LedgerDbName, LedgerCompanyName);
                try
                {
                    dtrDB.OpenDB();
                    DataSet ds = dtrDB.ExecuteDataSet(sql);

                    if (ds.Tables != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string Val = ds.Tables[0].Rows[0]["TIMEGAPVALUE"].ToString();
                            INOUTTIMEGAP = Convert.ToDouble(Val);
                        }
                        else
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", String.Format("{0}",LedgerDbName), "Parameter Master has no TIMEGAP parameter.", true);
                        }
                    }
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "GetTimeGap : RollBack Query", e.ToString(), true);
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "Posting", "GetTimeGap : General", e.ToString(), true);
            }

            return INOUTTIMEGAP;
        }

        public double GetExtensionBeforeTimeIn(string LedgerDbName, string LedgerCompanyName)
        {
            double EXTENSION = 0;
            try
            {
                string sql = string.Format(@"SELECT [Mph_PolicyCode] as [EXTENSION]
                                                   ,[Mph_NumValue] as [EXTENSIONVALUE]
                                               FROM {0}..[M_PolicyHdr]
                                              WHERE Mph_PolicyCode='EXTENDIN1'
                                                AND Mph_CompanyCode = '{1}'", LedgerDbName, LedgerCompanyName);
                try
                {
                    dtrDB.OpenDB();
                    DataSet ds = dtrDB.ExecuteDataSet(sql);

                    if (ds.Tables != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string Val = ds.Tables[0].Rows[0]["EXTENSIONVALUE"].ToString();
                            EXTENSION = Convert.ToDouble(Val);
                        }
                        else
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", String.Format("{0}", LedgerDbName), "Parameter Master has no EXTENDIN1 parameter.", true);
                        }
                    }
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "GetExtensionFromTimeIn : RollBack Query", e.ToString(), true);
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "Posting", "GetTimeGap : General", e.ToString(), true);
            }

            return EXTENSION;
        }

        public bool _isTimeGapValid(DateTime Log2, DateTime Log1)
        {
            bool valid = true;
            try
            {
                TimeSpan Span = Log2.Subtract(Log1);
                Double CapturedGap = Convert.ToDouble(Span.TotalMinutes);
                if (Globals.TIMEGAP > CapturedGap
                && Log1 != DateTime.MinValue)
                    valid = false;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "_isTimeGapValid : General", e.ToString(), true);
            }
            return valid;
        }

        public bool _isTimeGapValidOutOccurence(DateTime Log2, DateTime Log1)
        {
            bool valid = true;
            try
            {
                TimeSpan Span = Log2.Subtract(Log1);
                Double CapturedGap = Convert.ToDouble(Span.TotalMinutes);
                if (Globals.TIMEGAP > CapturedGap
                && Log2 != DateTime.MinValue)
                    valid = false;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "CapturedGap : General", e.ToString(), true);
            }
            return valid;
        }

        //Check for consecutive graveyard shift
        public bool _isConsecutiveGY(DateTime dtrShiftTimeIN, DateTime LedgerShiftTimeOUT)
        {
            bool Consecutive = false;
            try
            {
                if (dtrShiftTimeIN.AddDays(1).ToShortDateString() != LedgerShiftTimeOUT.ToShortDateString())
                    Consecutive = true;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "CapturedGap : General", e.ToString(), true);
            }
            return Consecutive;
        }

        public EmployeeLedger ErrLogRegShiftCleaner(EmployeeLedger LedgerCleaner, DateTime ReferenceDate, DateTime RegShiftOUT)
        {
            Int16 IN1 = Convert.ToInt16(LedgerCleaner.LogTime1.TimeIn.ToString("HHmm"));
            Int16 OUT1 = Convert.ToInt16(LedgerCleaner.LogTime1.TimeOut.ToString("HHmm"));
            Int16 IN2 = Convert.ToInt16(LedgerCleaner.LogTime2.TimeIn.ToString("HHmm"));
            Int16 OUT2 = Convert.ToInt16(LedgerCleaner.LogTime2.TimeOut.ToString("HHmm"));
            try
            {
                //Same In Out Cleaner / Time Gap In Out Cleaner

                if ((IN1 >= OUT1
                   || LedgerCleaner.LogTime1.TimeIn > LedgerCleaner.LogTime1.TimeOut.AddMinutes(-Globals.TIMEGAP)
                   || ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime1.TimeOut.ToShortDateString())
                   && LedgerCleaner.LogTime1.TimeIn != DateTime.MinValue
                   && LedgerCleaner.LogTime1.TimeOut != DateTime.MinValue)
                {
                    LedgerCleaner.LogTime1.TimeOut = DateTime.MinValue;
                }

                if ((IN2 >= OUT2
                   || LedgerCleaner.LogTime2.TimeIn > LedgerCleaner.LogTime2.TimeOut.AddMinutes(-Globals.TIMEGAP)
                   || ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime2.TimeOut.ToShortDateString())
                   && LedgerCleaner.LogTime2.TimeIn != DateTime.MinValue
                   && LedgerCleaner.LogTime2.TimeOut != DateTime.MinValue)
                {
                    if (!Globals.isFirstINLastOut 
                        && LedgerCleaner.LogTime1.TimeIn.Hour > 12 
                        && LedgerCleaner.LogTime2.TimeOut.Hour < 12
                        && LedgerCleaner.LogTime1.TimeOut == DateTime.MinValue
                        && LedgerCleaner.LogTime2.TimeIn == DateTime.MinValue)
                    {
                        //Nilo Added 20130827 : 
                        //Do not remove Out 2 since it is reposted with intellegent inverse post
                    }
                    else
                    {
                        LedgerCleaner.LogTime2.TimeOut = DateTime.MinValue;
                    }
                }

                if ((IN1 >= OUT2
                   || LedgerCleaner.LogTime1.TimeIn > LedgerCleaner.LogTime2.TimeOut.AddMinutes(-Globals.TIMEGAP)
                   || ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime2.TimeOut.ToShortDateString())
                   && LedgerCleaner.LogTime1.TimeIn != DateTime.MinValue
                   && LedgerCleaner.LogTime2.TimeOut != DateTime.MinValue)
                {
                    if (!Globals.isFirstINLastOut
                        && LedgerCleaner.LogTime1.TimeIn.Hour > 12
                        && LedgerCleaner.LogTime2.TimeOut.Hour < 12
                        && LedgerCleaner.LogTime1.TimeOut == DateTime.MinValue
                        && LedgerCleaner.LogTime2.TimeIn == DateTime.MinValue)
                    {
                        //Do not remove Out 2 since it is reposted with intellegent inverse post
                    }
                    else
                    {
                        LedgerCleaner.LogTime2.TimeOut = DateTime.MinValue;
                    }
                }

                if (ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime2.TimeOut.ToShortDateString()
                   && LedgerCleaner.LogTime2.TimeOut != DateTime.MinValue)
                {
                    if (!Globals.isFirstINLastOut
                        && LedgerCleaner.LogTime1.TimeIn.Hour > 12
                        && LedgerCleaner.LogTime2.TimeOut.Hour < 12
                        && LedgerCleaner.LogTime1.TimeOut == DateTime.MinValue
                        && LedgerCleaner.LogTime2.TimeIn == DateTime.MinValue)
                    {
                        //Do not remove Out 2 since it is reposted with intellegent inverse post
                    }
                    else
                    {
                        LedgerCleaner.LogTime2.TimeOut = DateTime.MinValue;
                    }
                }

            }
            catch (Exception e)
            {
                //_log.WriteLog(Application.StartupPath, "Posting", "ErrLogRegShiftCleaner : General", e.ToString(), true);
            }

            return LedgerCleaner;
        }

        public String PriorityIN()
        {
            FIRSTLASTINOUT Priority;
            if (Globals.isFirstINLastOut)
                Priority.IN = "ASC";
            else
                Priority.IN = "DESC";
            return Priority.IN;
        }

        public String PriorityOUT()
        {
            FIRSTLASTINOUT Priority;
            if (Globals.isFirstINLastOut)
                Priority.Out = "DESC";
            else
                Priority.Out = "ASC";
            return Priority.Out;
        }

        public DataTable GetEmployeeIdProcesssingList(string LedgerDbName)
        {
            DataTable employeelist = new DataTable();
            try
            {
                dtrDB.OpenDB();
                employeelist = dtrDB.ExecuteDataSet(string.Format("SELECT DISTINCT(Ttr_IDNo) EmployeeId FROM {0}.dbo.{1}", LedgerDbName, Globals.T_TimeRegister)).Tables[0];
                dtrDB.CloseDB();
            }
            catch(Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "GetEmployeeIdProcesssingList : RollBack", e.ToString(), true);
            }
            return employeelist;
        }

        #endregion

        #region Methods used externally controlling logs
        
        public void LogpostingCounterMeasureFunction(String LedgerDBName, DALHelper dal)
        {
            #region Accumulate Dataset

            //Order of the dataset is important base on what process must be executed first
            DataSet[] dsAccumulatedPostingScenario = new DataSet[12];
            //Recovery
            dsAccumulatedPostingScenario[0] = DsRecoverGraveyardOut;
            dsAccumulatedPostingScenario[1] = DsRecoverGraveUnpostLIFO;
            dsAccumulatedPostingScenario[2] = DsRecoveryRegularShift;
            dsAccumulatedPostingScenario[3] = DsRecoveryGraveYardShift;
            dsAccumulatedPostingScenario[4] = DsIntellegentPostingRegShiftwithGraveLogs;
            dsAccumulatedPostingScenario[5] = DsIntellegentPostingGraveShiftwithRegLogs;

            //Clean Up
            dsAccumulatedPostingScenario[6] = DsCleanLogsFromNextDayPostedtoCurrent;
            dsAccumulatedPostingScenario[7] = DsRecoveryGraveYardCleaningIn;
            dsAccumulatedPostingScenario[8] = DsRecoveryGraveYardCleaningOut;
            dsAccumulatedPostingScenario[9] = DsLogRecheckOverloadCleanup; //Multiple Data table
            //OT Aftermidnight
            dsAccumulatedPostingScenario[10] = DsOvertimeAfterMidnightPlus24;
            //Repost log trail
            dsAccumulatedPostingScenario[11] = DsRepostLogTrail;

            #endregion

            int i = 0;

            foreach (DataSet ds in dsAccumulatedPostingScenario)
            {
                if (ds == null)
                {
                    //Do nothing
                }
                else
                {
                    GenericCleanedLogsPosting(ds, LedgerDBName, dal);
                    ds.Dispose();
                }
                ++i;
            }

            #region Dataset Disposal

            DsRecoverGraveyardOut = new DataSet();
            DsRecoverGraveUnpostLIFO = new DataSet();
            DsCleanLogsFromNextDayPostedtoCurrent = new DataSet();
            DsRecoveryRegularShift = new DataSet();
            DsRecoveryGraveYardCleaningIn = new DataSet();
            DsRecoveryGraveYardCleaningOut = new DataSet();
            DsRecoveryGraveYardShift = new DataSet();
            DsLogRecheckOverloadCleanup = new DataSet();
            DsOvertimeAfterMidnightPlus24 = new DataSet();
            DsRepostLogTrail = new DataSet();
            DsIntellegentPostingRegShiftwithGraveLogs = new DataSet();
            DsIntellegentPostingGraveShiftwithRegLogs = new DataSet();
            dsAccumulatedPostingScenario = null;

            #endregion

        }

        public void ShiftCodeManipulationFunction(String LedgerDBName, DALHelper dal)
        {
            #region Accumulate Dataset

            //Order of the dataset is important base on what process must be executed first

            DataSet[] dsAccumulatedPostingScenario = new DataSet[4];
            dsAccumulatedPostingScenario[0] = DsFlexShifting;
            dsAccumulatedPostingScenario[1] = DsManipulateEquivalentShiftHolidayRestday;
            dsAccumulatedPostingScenario[2] = DsManipulateDefaultShiftCPHandDenso;
            dsAccumulatedPostingScenario[3] = DsManipulateShiftwithShiftMovementDenso;

            #endregion

            int i = 0;
            foreach (DataSet ds in dsAccumulatedPostingScenario)
            {
                if (ds == null)
                {
                    //Do nothing
                }
                else
                {
                    GenericShiftCodeManipulator(ds, LedgerDBName, dal);
                    ds.Dispose();
                }
                ++i;
            }

            #region Dataset Disposal

            dsAccumulatedPostingScenario = null;
            DsManipulateEquivalentShiftHolidayRestday = new DataSet();
            DsManipulateDefaultShiftCPHandDenso = new DataSet();
            DsFlexShifting = new DataSet();
            DsManipulateShiftwithShiftMovementDenso = new DataSet();

            #endregion

        }

        //Generic Clean 4 Pocket Posting
        private void GenericCleanedLogsPosting(DataSet dsLedger, String LedgerDBName, DALHelper dal)
        {
            try
            {
                if (dsLedger == null)
                    return;
                //dtrDB.OpenDB();
                foreach (DataTable dt in dsLedger.Tables)
                {
                    int affected = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        ParameterInfo[] paramInfo = new ParameterInfo[6];

                        paramInfo[0] = new ParameterInfo("@EMPLOYEEID", dr["EmployeeID"], SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@PROCESSDATE", dr["ProcessDate"], SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@IN1", dr["IN1"], SqlDbType.Char, 4);
                        paramInfo[3] = new ParameterInfo("@OUT1", dr["OUT1"], SqlDbType.Char, 4);
                        paramInfo[4] = new ParameterInfo("@IN2", dr["IN2"], SqlDbType.Char, 4);
                        paramInfo[5] = new ParameterInfo("@OUT2", dr["OUT2"], SqlDbType.Char, 4);


                        String sql = String.Format(@"UPDATE {0}.dbo.{1}
                                                        SET Ttr_ActIn_1 = @IN1
                                                           ,Ttr_ActOut_1 = @OUT1
                                                           ,Ttr_ActIn_2 = @IN2
                                                           ,Ttr_ActOut_2 = @OUT2
                                                           ,USR_LOGIN = 'LOGUPLDSRVC'
                                                           ,LUDATETIME = GETDATE()
                                                      WHERE Ttr_Date = @PROCESSDATE
                                                        AND Ttr_IDNo = @EMPLOYEEID", LedgerDBName, Globals.T_TimeRegister);

                        try
                        {
                            affected = dal.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                            affected = 0;
                        }
                        catch (Exception e)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "GenericCleanLogsPosting : RollBack", e.Message, true);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "GenericCleanLogsPosting", e.Message, true);
            }
            finally
            {
                //dtrDB.CloseDB();
            }
        }

        //Generic ShiftCode Manipulator
        private void GenericShiftCodeManipulator(DataSet dsLedger, String LedgerDBName, DALHelper dal)
        { 
            try
            {
                if (dsLedger == null)
                    return;

                foreach (DataTable dt in dsLedger.Tables)
                {
                    int affected = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        ParameterInfo[] paramInfo = new ParameterInfo[3];

                        paramInfo[0] = new ParameterInfo("@EMPLOYEEID", dr["EmployeeID"], SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@PROCESSDATE", dr["ProcessDate"], SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@ShiftCode", dr["ShiftCode"].ToString().Trim(), SqlDbType.VarChar);


                        String sql = String.Format(@"UPDATE [{0}].[dbo].[{1}] 
		                                                SET Ttr_ShiftCode = @ShiftCode
                                                      WHERE Ttr_Date = @ProcessDate
                                                        AND Ttr_IDNo = @EmployeeId", LedgerDBName, Globals.T_TimeRegister);

                        try
                        {
                            affected = dal.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                            affected = 0;
                        }
                        catch (Exception e)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "GenericShiftCodeManipulator : RollBack", e.Message, true);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "GenericShiftCodeManipulator", e.Message, true);
            }
            finally
            {
                //dtrDB.CloseDB();
            }
        }

        public void LIFOGraveYardCapturingInBeforOut(DateTime ProcessDate, String LedgerDBName, Double TimeGap, DALHelper dal)
        { 
            //Used only for LIFO Posting

            if (Globals.isFirstINLastOut)
                return;

            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@START", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
            paramInfo[1] = new ParameterInfo("@END", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
            paramInfo[2] = new ParameterInfo("@GAP", TimeGap, SqlDbType.Float);
            #region query
            String sql = string.Format(@"SELECT 
	                                      CASE WHEN(SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) IS NULL THEN '--'
			                                    ELSE 
			                                    (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) END AS DTRIN1
	                                      ,CASE WHEN (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    <= CONVERT(INT,SHIFT.Msh_ShiftOut1)
			                                    AND TEL_LogType='O'
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    > CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    ORDER BY TEL_LOGTIME ASC)IS NULL THEN '--' 
			                                    ELSE 
			                                    (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    <= CONVERT(INT,SHIFT.Msh_ShiftOut1)
			                                    AND TEL_LogType='O'
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    > CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    ORDER BY TEL_LOGTIME ASC) END AS DTROUT1
	                                      ,CASE WHEN (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    >= CONVERT(INT,SHIFT.Msh_ShiftIn2)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) IS NULL THEN '--'
			                                    ELSE
			                                    (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    >= CONVERT(INT,SHIFT.Msh_ShiftIn2)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) END AS DTRIN2
	                                      ,CASE WHEN(SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    >=(CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
			                                    AND TEL_LogType='O'
			                                    ORDER BY TEL_LOGTIME ASC) IS NULL THEN '--'
			                                    ELSE
			                                    (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    >= (CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
			                                    AND TEL_LogType='O'
			                                    ORDER BY TEL_LOGTIME ASC) END AS DTROUT2
	                                      ,[Ttr_IDNo] AS ID
                                          ,[Ttr_Date] AS PRCDATE
                                          ,[Ttr_ActIn_1] AS IN1
                                          ,[Ttr_ActOut_1] AS OUT1
                                          ,[Ttr_ActIn_2] AS IN2
                                          ,[Ttr_ActOut_2] AS OUT2
                                          ,[Ttr_ShiftCode] AS SHIFTCOD
                                          ,Msh_ShiftIn1 AS SHFTIN
                                          ,Msh_ShiftOut1 AS BRKIN
                                          ,Msh_ShiftIn2 AS BRKOUT
                                          ,Msh_ShiftOut2 AS SHFTOUT
	                                      FROM
	                                      (SELECT 
		                                       [Ttr_IDNo]
		                                      ,[Ttr_Date]
		                                      ,[Ttr_ActIn_1]
		                                      ,[Ttr_ActOut_1]
		                                      ,[Ttr_ActIn_2] 
		                                      ,[Ttr_ActOut_2] 
		                                      ,[Ttr_ShiftCode]
                                           FROM {0}..T_EmpTimeRegister
                                           WHERE Ttr_Date BETWEEN @START AND @END
                                           UNION
	                                       SELECT 
		                                       [Ttr_IDNo] 
		                                      ,[Ttr_Date]
		                                      ,[Ttr_ActIn_1] 
		                                      ,[Ttr_ActOut_1]
		                                      ,[Ttr_ActIn_2] 
		                                      ,[Ttr_ActOut_2]
		                                      ,[Ttr_ShiftCode]
                                           FROM {0}..T_EmpTimeRegisterHst
                                           WHERE Ttr_Date BETWEEN @START AND @END) LEDGERLIST
	                                    LEFT JOIN 
	                                    {0}..M_Shift SHIFT
	                                    ON Msh_ShiftCode=Ttr_ShiftCode
                                    WHERE
                                    CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
                                    AND Ttr_ActOut_1='0000'
                                    AND Ttr_ActOut_2='0000'
                                    AND Ttr_ActIn_1!='0000'
                                    AND 
	                                    CONVERT(INT,REPLACE(LEFT(CONVERT(varchar(25),DATEADD(MINUTE,(120),LEFT(Ttr_ActIn_1,2)+':'+RIGHT(Ttr_ActIn_1,2)),24),5),':',''))
	                                    <
	                                    CONVERT(INT,Msh_ShiftIn1)
                                    AND
		                                    -- DTR OUT >= IN1 - GAP
		                                    (
		                                    CONVERT(INT,CASE WHEN(SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
		                                    WHERE 
		                                    Tel_IDNo=Ttr_IDNo 
		                                    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
			                                    AND CONVERT(INT,TEL_LOGTIME) 
			                                    >=(CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
		                                    AND TEL_LogType='O'
		                                    ORDER BY TEL_LOGTIME ASC) IS NULL THEN '0000'
		                                    ELSE
		                                    (SELECT TOP 1 
			                                    TEL_LOGTIME FROM {1}..{2} 
		                                    WHERE 
		                                    Tel_IDNo=Ttr_IDNo 
		                                    AND CONVERT(DATETIME,Tel_LogDate)=DATEADD(DD,1,Ttr_Date)
			                                    AND CONVERT(INT,TEL_LOGTIME) 
			                                    >= (CONVERT(INT,SHIFT.Msh_ShiftIn2)-2400)
		                                    AND TEL_LogType='O'
		                                    ORDER BY TEL_LOGTIME ASC) END)
		                                    )>=CONVERT(INT,REPLACE(LEFT(CONVERT(varchar(25),DATEADD(MINUTE,(@GAP*-1),LEFT(Ttr_ActIn_1,2)+':'+RIGHT(Ttr_ActIn_1,2)),24),5),':',''))
                                    AND
		                                    -- OUT1 != 0
		                                    CASE WHEN(SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) IS NULL THEN '0000'
			                                    ELSE 
			                                    (SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC)END!='0000'
                                    AND
		                                    --IN1 is not in the morning
	                                        (CONVERT(INT,CASE WHEN(SELECT TOP 1 TEL_LOGTIME FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) IS NULL THEN '0000'
			                                    ELSE 
			                                    (SELECT TOP 1 
                                                    -- hardcoded 3 hours befor shift time in
				                                    REPLACE(LEFT(CONVERT(varchar(25),DATEADD(MINUTE,(180),LEFT(TEL_LOGTIME,2)+':'+RIGHT(TEL_LOGTIME,2)),24),5),':','')
		                                        FROM {1}..{2} 
			                                    WHERE 
			                                    Tel_IDNo=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Tel_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,TEL_LOGTIME) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND TEL_LogType='I' ORDER BY TEL_LOGTIME DESC) END)
			                                    )>=CONVERT(INT,Msh_ShiftIn1)
                                    ORDER BY Ttr_Date,Ttr_IDNo", LedgerDBName, _dtrDBName, Globals.T_EmpDTR);
            #endregion
            try
            {
                //dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
                setNightShiftHandlerLIFOINb4OUT(ds, LedgerDBName, dal);
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "Posting", "NightShiftHandlerLIFOINb4OUT : RollBack", e.ToString(), true);
            }
            finally
            {
                //dtrDB.CloseDB();
            }
        }

        public void setNightShiftHandlerLIFOINb4OUT(DataSet ds, String LedgerDBName, DALHelper dal )
        {
            DataTable dt = ds.Tables[0];
            DataSet dsLedgerExist = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int affectedrow = 0;
            int LedgerSelect = 0;
            String _isLedger;
            String sql;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        Console.WriteLine("\tUnpost Recovery Night Shift With Out before In {0} of {1}  [{2}]", i, dt.Rows.Count, dt.Rows[i]["PRCDATE"]);

                        _isLedger = "T_EmpTimeRegister";
                        paramInfo[0] = new ParameterInfo("@ID", dt.Rows[i]["ID"].ToString(), SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@PRCDATE", dt.Rows[i]["PRCDATE"].ToString(), SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@DTRIN1", dt.Rows[i]["DTRIN1"].ToString(), SqlDbType.VarChar, 4);
                        paramInfo[3] = new ParameterInfo("@DTROUT2", dt.Rows[i]["DTROUT2"].ToString(), SqlDbType.VarChar, 4);

                        sql = string.Format("Select Count(Ttr_IDNo) AS COUNT FROM {0}..T_EmpTimeRegister WHERE Ttr_IDNo=@ID AND Ttr_Date=@PRCDATE", LedgerDBName);

                        //dtrDB.BeginTransaction();
                        dsLedgerExist = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                        LedgerSelect = Convert.ToInt16(dsLedgerExist.Tables[0].Rows[0]["COUNT"].ToString());
                        if (LedgerSelect <= 0)
                            _isLedger = "T_EmpTimeRegisterHst";
                        sql = string.Format(@"Update {0}..{1}
                                                 SET Ttr_ActIn_1=@DTRIN1
                                                 ,Ttr_ActOut_2=@DTROUT2
                                                 ,USR_LOGIN='LOGUPLDSRVC'
                                                 ,LUDATETIME=GETDATE()
                                                 WHERE Ttr_Date=@PRCDATE AND Ttr_IDNo=@ID",
                        LedgerDBName, _isLedger);
                        affectedrow = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch
                    {
                        //dtrDB.RollBackTransaction();
                    }
                }
            }

            dsLedgerExist = null;
        }

        //Forchanging
        public void RecoverGraveyardOut(DateTime Processdate, String LedgerDBName)
        {
            //Resume condition here
            try
            {
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        DsRecoverGraveyardOut = dtrDB.ExecuteDataSet(String.Format(QueryRecoverGraveyardOut, _dtrDBName, LedgerDBName, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveyardOut : RollBack", e.Message, true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveyardOut : General", e.ToString(), true);
            }
            
        }

        //Forchanging
        public void RecoverGraveUnpostLIFO(DateTime Processdate, String LedgerDBName)
        {
            if (Globals.isFirstINLastOut)
                return;

            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    DsRecoverGraveUnpostLIFO = dtrDB.ExecuteDataSet(String.Format(QueryRecoverGraveUnpostLIFO, _dtrDBName, LedgerDBName, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveUnpostLIFO : RollBack", e.Message, true);
                    //dtrDB.RollBackTransaction();
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveUnpostLIFO : General", e.ToString(), true);
            }
        }

        /// <summary>
        /// Process Date -1
        /// </summary>
        /// <param name="ProcessDate"></param>
        /// <param name="LedgerDBName"></param>
        /// <param name="dal"></param>
        /// <param name="CleanUpOnly"></param>
        public void UnpostRecoveryPreviousDateSetDataSet(DateTime ProcessDate, String LedgerDBName, DALHelper dal, bool CleanUpOnly)
        {
            //Recovery and Review of logs from previous date
            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.AddDays(-1).ToShortDateString(), SqlDbType.DateTime);

                string RULED_IN = "ASC";
                string RULE_OUT = "DESC";
                if (!Globals.isFirstINLastOut)
                {
                    RULED_IN = "DESC";
                    RULE_OUT = "ASC";
                }
                    
                try
                {
                    //dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    
                    if (!CleanUpOnly)
                    {
                        //Recovery
                        DsRecoveryRegularShift = dal.ExecuteDataSet(String.Format(QueryRecoveryRegularShift, LedgerDBName, _dtrDBName, RULED_IN, RULE_OUT, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                        DsRecoveryGraveYardShift = dal.ExecuteDataSet(String.Format(QueryRecoveryGraveYardShift, LedgerDBName, _dtrDBName, RULED_IN, RULE_OUT, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                    }
                    else
                    {
                        //Clean Up
                        DsCleanLogsFromNextDayPostedtoCurrent = dal.ExecuteDataSet(String.Format(QueryCleanLogsFromNextDayPostedtoCurrent, LedgerDBName, _dtrDBName, RULED_IN, RULE_OUT, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                        DsRecoveryGraveYardCleaningIn = dal.ExecuteDataSet(String.Format(QueryRecoveryGraveYardCleaningIn, LedgerDBName, _dtrDBName, RULED_IN, RULE_OUT, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                        DsRecoveryGraveYardCleaningOut = dal.ExecuteDataSet(String.Format(QueryRecoveryGraveYardCleaningOut, LedgerDBName, _dtrDBName, RULED_IN, RULE_OUT, Globals.T_EmpDTR), CommandType.Text, paramInfo);
                    }

                    //dtrDB.CommitTransaction();
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "UnpostRecovery : RollBack", e.ToString(), true);
                    //dtrDB.RollBackTransaction();
                }
                finally
                {
                    //dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UnpostRecovery : General", e.ToString(), true);
            }
        }

        /// <summary>
        /// Process Date -1
        /// </summary>
        /// <param name="ProcessDate"></param>
        /// <param name="LedgerDBName"></param>
        /// <param name="dal"></param>
        public void IntelligentInversePostingPreviousDateSetDataSet(DateTime ProcessDate, String LedgerDBName, DALHelper dal)
        {
            if (Globals.isFirstINLastOut) //reverse is intended for HOYA only / Applying Lear
                return;
            
            try
            {
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.AddDays(-1).ToShortDateString(), SqlDbType.DateTime);

                    #region query

                    String RegInversePosting = String.Format(QueryIntellegentPostingRegShiftwithGraveLogs, _dtrDBName, LedgerDBName, Globals.T_EmpDTR);
                    String GraveInversePosting = String.Format(QueryIntellegentPostingGraveShiftwithRegLogs, _dtrDBName, LedgerDBName, Globals.T_EmpDTR);

                    if (Globals.isFirstINLastOut) //reverse is intended for HOYA only / Applying Lear
                    {
                        RegInversePosting = String.Empty;
                        GraveInversePosting = String.Empty;
                    }

                    ////These are raw query which is very heavy and hard to control.
                    ////This block is commented after fixing the base function for log posting.
                    //String sql = String.Format(QueryLogRecheckOverloadCleanup, _dtrDBName, LedgerDBName, RegInversePosting, GraveInversePosting, Globals.Used_T_DTR);

                    #endregion

                        try
                        {
                            //dtrDB.OpenDB();
                            //dtrDB.BeginTransaction();
                            if (!string.IsNullOrEmpty(RegInversePosting)) 
                                DsIntellegentPostingRegShiftwithGraveLogs = dal.ExecuteDataSet(RegInversePosting, CommandType.Text, paramInfo);
                            if (!string.IsNullOrEmpty(GraveInversePosting))
                                DsIntellegentPostingGraveShiftwithRegLogs = dal.ExecuteDataSet(GraveInversePosting, CommandType.Text, paramInfo);

                            LogpostingCounterMeasureFunction(LedgerDBName, dal); //Force execution of recovery before clean up

                            #region Dispose dataset

                            if (DsIntellegentPostingRegShiftwithGraveLogs != null) { DsIntellegentPostingRegShiftwithGraveLogs.Dispose(); }
                            if (DsIntellegentPostingGraveShiftwithRegLogs != null) { DsIntellegentPostingGraveShiftwithRegLogs.Dispose(); }

                            #endregion

                            ////Nilo Commented 20130823 : These are raw query which is very heavy and hard to control.
                            ////This block is commented after fixing the base function for log posting.
                            //DsLogRecheckOverloadCleanup = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                        }
                        catch (Exception e)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "IntelligentInversePostingPreviousDateSetDataSet : RollBack", e.ToString(), true);
                            //dtrDB.RollBackTransaction();
                        }
                        finally
                        {
                            //dtrDB.CloseDB();
                        }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "IntelligentInversePostingPreviousDateSetDataSet : General", e.ToString(), true);
            }
        }
        
        //LOGIC FOR CPH
        public void OvertimeAfterMidnightCapturing(DateTime processdate, String LedgerDBName, String LedgerCompanyCode, DALHelper dal)
        {
            OvertimeAfterMidnightCapturingPerDay(processdate.AddDays(-1), LedgerDBName, LedgerCompanyCode, dal); //Do previous day
            OvertimeAfterMidnightCapturingPerDay(processdate, LedgerDBName, LedgerCompanyCode, dal); //Do current day
        }

        public void OvertimeAfterMidnightCapturingPerDay(DateTime processdate, String LedgerDBName, String LedgerCompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();
            try
            {
                String sqlOTAfterMid;

                if (!CommonProcedures.GetAppSettingConfigBool("OTAfterMidnight", false))
                {
                    return;
                }
                ParameterInfo[] paramInfo = new ParameterInfo[3];
                paramInfo[0] = new ParameterInfo("@PREVDATE", processdate.AddDays(-1).ToShortDateString(), SqlDbType.DateTime);
                paramInfo[1] = new ParameterInfo("@CURRENTDATE", processdate.ToShortDateString(), SqlDbType.DateTime);
                paramInfo[2] = new ParameterInfo("@GAP", Globals.TIMEGAP, SqlDbType.Float);

                #region query
                sqlOTAfterMid = string.Format(@"SELECT 
	                                             OUTNXTD.Tel_LogDate
	                                            ,OUTNXTD.Tel_LogTime
	                                            ,CONVERT(DATETIME,Tel_LogDate
			                                            +' '+
			                                            LEFT((SELECT TOP(1)INAFTER.Tel_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Tel_LogType='I' 
			                                            AND Tel_IDNo=Ttr_IDNo AND Tel_LogDate=@CURRENTDATE ORDER BY Tel_LogTime),2)
			                                            +':'+ 
			                                            RIGHT((SELECT TOP(1)INAFTER.Tel_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Tel_LogType='I' 
			                                            AND Tel_IDNo=Ttr_IDNo AND Tel_LogDate=@CURRENTDATE ORDER BY Tel_LogTime),2)
			                                            ) Dtr_DateTime
	                                            ,Msh_ShiftIn1 AS SHIFT
	                                            ,Ttr_IDNo AS EMP_ID
	                                            ,CONVERT(VARCHAR(20),Ttr_Date,101) AS PROCESS_DATE
	                                            ,Ttr_ActIn_1 AS IN1
	                                            ,Ttr_ActIn_2 AS OUT1
	                                            ,Ttr_ActOut_1 AS IN2
	                                            ,Ttr_ActOut_2 AS OUT2
                                                FROM 
		                                            (SELECT Ttr_IDNo
			                                            ,Ttr_Date
			                                            ,Ttr_ActIn_1
			                                            ,Ttr_ActIn_2
			                                            ,Ttr_ActOut_1
			                                            ,Ttr_ActOut_2
			                                            ,Ttr_ShiftCode
		                                            FROM {0}..T_EmpTimeRegister
		                                            WHERE Ttr_Date BETWEEN @PREVDATE AND @CURRENTDATE
		                                            UNION 
		                                            select Ttr_IDNo
			                                            ,Ttr_Date
			                                            ,Ttr_ActIn_1
			                                            ,Ttr_ActIn_2
			                                            ,Ttr_ActOut_1
			                                            ,Ttr_ActOut_2
			                                            ,Ttr_ShiftCode
		                                            FROM {0}..T_EmpTimeRegisterHst
		                                            WHERE Ttr_Date BETWEEN @PREVDATE AND @CURRENTDATE
		                                            ) LEDGERLIST
                                                    JOIN
                                                        {4}..M_Shift SHIFT
                                                        ON Ttr_ShiftCode=Msh_ShiftCode
                                                        AND Msh_CompanyCode = '{3}'
                                                    LEFT JOIN
                                                    {1}..{2} AS OUTNXTD
                                                    ON Tel_IDNo=Ttr_IDNo
                                                    AND Tel_LogDate=@CURRENTDATE
                                                    AND Tel_LogType='O' 
                                                    --ORDER BY TEL_LOGTIME
                                                    AND CONVERT(INT,DATEDIFF(MI,CONVERT(DATETIME,Tel_LogDate +' '+ LEFT(Tel_LogTime,2)+':'+RIGHT(Tel_LogTime,2)),
									                                            CASE WHEN 
										                                            CONVERT(INT,(SELECT TOP(1)INAFTER.Tel_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Tel_LogType='I' 
										                                            AND Tel_IDNo=Ttr_IDNo AND Tel_LogDate=@CURRENTDATE ORDER BY Tel_LogTime)) 
										                                            IS NULL THEN 
										                                            CONVERT(DATETIME,CONVERT(VARCHAR,@CURRENTDATE,101) +' '+ LEFT(SHIFT.Msh_ShiftIn1,2)+':'+RIGHT(SHIFT.Msh_ShiftIn1,2)) 
									                                            ELSE 
										                                            CONVERT(DATETIME,Tel_LogDate
												                                            +' '+
												                                            LEFT((SELECT TOP(1)INAFTER.Tel_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Tel_LogType='I' 
												                                            AND Tel_IDNo=Ttr_IDNo AND Tel_LogDate=@CURRENTDATE ORDER BY Tel_LogTime),2)
												                                            +':'+ 
												                                            RIGHT((SELECT TOP(1)INAFTER.Tel_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Tel_LogType='I' 
												                                            AND Tel_IDNo=Ttr_IDNo AND Tel_LogDate=@CURRENTDATE ORDER BY Tel_LogTime),2)
												                                            )
									
									                                            END)
					                                            )> @GAP
                                                WHERE 
                                                (CONVERT(BIGINT,Ttr_ActIn_2+Ttr_ActIn_1)!=0) --Must have IN1 or IN2
                                                    AND 
                                                    (CONVERT(INT,Ttr_ActOut_2)=0 OR (CONVERT(INT,Ttr_ActOut_2)<CONVERT(INT,Msh_ShiftIn1) AND CONVERT(INT,Ttr_ActIn_1)<1400))
                                                AND Ttr_Date=@PREVDATE
                                                AND 
                                                CONVERT(INT,Tel_LogTime)<CONVERT(INT,Msh_ShiftIn1)
                                                AND CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)"
                                    , LedgerDBName, _dtrDBName, Globals.T_EmpDTR, LedgerCompanyCode, Globals.CentralProfile);
                #endregion

                try
                {
                    //dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    ds = dal.ExecuteDataSet(sqlOTAfterMid, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                    //start updating of ledger
                    OTafternextDayUpdater(ds, LedgerDBName, dal);
                    if (ds != null)
                        ds.Dispose();
                    //plus 24 all OT
                    OTAfternextDayPlus24(processdate, LedgerDBName, dal);
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "OTAfterMindightCapturing : RollBack", e.ToString(), true);
                }
                finally
                {
                    //dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "OTAfterMindightCapturing : General", e.ToString(), true);
            }
        }

        public void OTafternextDayUpdater(DataSet ds, String LedgerDBName, DALHelper dal)
        {
            DataTable dt = ds.Tables[0];
            DataSet dsLedgerExist = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int affectedrow = 0;
            int LedgerSelect = 0;
            String _isLedger;
            String sql;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        if (Convert.ToInt16(dt.Rows[i]["Tel_LogTime"]) < 1200)
                        {
                            _isLedger = "T_EmpTimeRegister";
                            paramInfo[0] = new ParameterInfo("@EMP_ID", dt.Rows[i]["EMP_ID"].ToString(), SqlDbType.VarChar, 15);
                            paramInfo[1] = new ParameterInfo("@PROCESS_DATE", dt.Rows[i]["PROCESS_DATE"].ToString(), SqlDbType.DateTime);
                            paramInfo[2] = new ParameterInfo("@TEL_LOGTIME", dt.Rows[i]["TEL_LOGTIME"].ToString(), SqlDbType.VarChar, 4);

                            sql = string.Format("Select Count(Ttr_IDNo) AS COUNT FROM {0}..T_EmpTimeRegister WHERE Ttr_IDNo=@EMP_ID AND Ttr_Date=@PROCESS_DATE", LedgerDBName);
                            //dtrDB.BeginTransaction();
                            dsLedgerExist = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                            LedgerSelect = Convert.ToInt16(dsLedgerExist.Tables[0].Rows[0]["COUNT"].ToString());
                            if (LedgerSelect <= 0)
                                _isLedger = "T_EmpTimeRegisterHst";
                            sql = string.Format(@"Update {0}..{1}
                                                 SET Ttr_ActOut_2=CONVERT(INT,@TEL_LOGTIME)+2400
                                                 ,USR_LOGIN='LOGUPLDONXTD'
                                                 ,LUDATETIME=GETDATE()
                                                 WHERE Ttr_Date=@PROCESS_DATE AND Ttr_IDNo=@EMP_ID",
                            LedgerDBName, _isLedger);
                            affectedrow = dal.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                        }
                    }
                    catch
                    {
                        //dtrDB.RollBackTransaction();
                    }
                }
            }
        }

        //For Changing
        public void OTAfternextDayPlus24(DateTime processdate, String LedgerDBName, DALHelper dal)
        {
            //if (!LogUploading_V2_Globals.isReposting)
            //    return;
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ProcessDate", processdate.ToShortDateString(), SqlDbType.DateTime);

            #region SQL
            string sql = string.Format(QueryOvertimeAfterMidnightPlus24, LedgerDBName, Globals.T_TimeRegister);
            #endregion

            try
            {
                //dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                DsOvertimeAfterMidnightPlus24 = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                //dtrDB.CloseDB();
                _log.WriteLog(Application.StartupPath, "Posting", "OTAfternextDayPlus24 : General", e.ToString(), true);
            }
            finally
            {
                //dtrDB.CloseDB();
            }
        }
        
        //GENERAL USE
        public DataSet getUnpostedAfterPosting(DateTime Processdate, String LedgerDBName)
        {
            DataSet dsUnposted = new DataSet();
            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                String sql = string.Format(@"SELECT 
	                                          -- IN 1
	                                          (SELECT TOP 1 TEL_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Tel_IDNo=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,TEL_LOGTIME) 
				                                        < CONVERT(INT,SHIFT.Msh_ShiftOut1)
			                                        AND TEL_LogType='I' ORDER BY TEL_LOGTIME {4}) AS DTR_IN1
	                                          --OUT 1
	                                          ,(SELECT TOP 1 TEL_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Tel_IDNo=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,TEL_LOGTIME) 
				                                        <= CONVERT(INT,SHIFT.Msh_ShiftIn2)
			                                        AND TEL_LogType='O'
			                                        ORDER BY TEL_LOGTIME {5})  AS DTR_OUT1
	                                          --IN 2
		                                        ,(SELECT TOP 1 TEL_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Tel_IDNo=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,TEL_LOGTIME) 
				                                        >= CONVERT(INT,SHIFT.Msh_ShiftOut1)
			                                        AND TEL_LogType='I' ORDER BY TEL_LOGTIME {4})  AS DTR_IN2
	                                          --OUT 2
	                                          ,(SELECT TOP 1 TEL_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Tel_IDNo=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Tel_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,TEL_LOGTIME) 
				                                        >= CONVERT(INT,SHIFT.Msh_ShiftIn2)
			                                        AND TEL_LogType='O'
			                                        ORDER BY TEL_LOGTIME {5}) AS  DTR_OUT2
	                                          ,[Ttr_IDNo] AS EMPLOYEE_ID
                                              ,[Ttr_Date] AS PRCDATE
                                              ,[Ttr_ActIn_1] AS LEDGER_IN1
                                              ,[Ttr_ActOut_1]AS LEDGER_OUT1
                                              ,[Ttr_ActIn_2] AS LEDGER_IN2
                                              ,[Ttr_ActOut_2] AS lEDGER_OUT2
                                          FROM {1}..[{2}] LEDGER
	                                        LEFT JOIN 
	                                        {1}..M_Shift SHIFT
	                                        ON Msh_ShiftCode=Ttr_ShiftCode
                                        WHERE Ttr_Date = @ProcessDate
                                        AND
                                        CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)
                                        AND 
                                        (CONVERT(BIGINT,Ttr_ActIn_1+Ttr_ActIn_2+Ttr_ActOut_1+Ttr_ActOut_2)=0
                                        OR
	                                        (CONVERT(BIGINT,Ttr_ActIn_1+Ttr_ActIn_2+Ttr_ActOut_1)=0
	                                        AND
	                                        CONVERT(INT,Ttr_ActOut_2)>2400)
                                        )
                                        ORDER BY Ttr_Date,Ttr_IDNo",
                _dtrDBName, LedgerDBName, Globals.T_TimeRegister, Globals.T_EmpDTR, PriorityIN(), PriorityOUT());
                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    dsUnposted = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "getUnpostedAfterPosting : Rollback", e.ToString(), true);
                    dtrDB.CloseDB();
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "getUnpostedAfterPosting : General", e.ToString(), true);
            }
            return dsUnposted;
        }
        
        public void UploadReadDiffTextFileLogs(String FileFolder, DateTime processDate)
        {
            try
            {
                try
                {
                    //Create Differential Table and Stored Procedure
                    T_DTRDifferentialCreator();

                    //DELETING DIFFERENTIAL DTR DATE -1
                    int Deleted = 0;
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@PREVDATE", processDate.AddDays(-2).ToShortDateString(), SqlDbType.DateTime);
                    String delsql = string.Format("DELETE {0}..T_DTRDifferential WHERE CONVERT(DATETIME,Tel_LogDate,101)<=@PREVDATE", _dtrDBName);
                    Deleted = dtrDB.ExecuteNonQuery(delsql, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                    dtrDB.CloseDB();
                }
                catch (Exception ex)
                {
                    //dtrDB.RollBackTransaction();
                    dtrDB.CloseDB();
                    _log.WriteLog(Application.StartupPath, "Posting", "UplaodReadDiffTextFileLogs : Delete Diff", "", true);
                }

                _log.WriteLog(Application.StartupPath, "Posting", "Reposting Previous Day TextFile Started", "", true);

                String LogsLine = "";
                String[] LogsData;
                System.IO.StreamReader objReader;
                string[] FTPFiles = Directory.GetFiles(FileFolder);

                dtrDB.OpenDB();

                foreach (string _FTPFiles in FTPFiles)
                {
                    try
                    {
                        objReader = new System.IO.StreamReader(_FTPFiles);

                        String FTPTxtDate = _FTPFiles.Replace(FileFolder, "");
                        FTPTxtDate = FTPTxtDate.Replace(@"\", "");
                        //parse the .txt filename by yearmonthdate (text file must be yyyymmdd--any-other-character.text
                        FTPTxtDate = (String.Format("{0}-{1}-{2}", FTPTxtDate.Substring(4, 2)
                        , FTPTxtDate.Substring(6, 2)
                        , FTPTxtDate.Substring(0, 4)));
                        if (Convert.ToDateTime(FTPTxtDate).ToShortDateString() == processDate.ToShortDateString()
                        || Convert.ToDateTime(FTPTxtDate).ToShortDateString() == processDate.AddDays(-1).ToShortDateString())
                        {
                            while (objReader.Peek() != -1)
                            {
                                LogsLine = objReader.ReadLine();
                                LogsData = LogsLine.Split(',');
                                if (LogsLine.Trim() != String.Empty)
                                {
                                    try
                                    {
                                        TextFileDiffDTRUploading(LogsData, processDate, dtrDB);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            objReader.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        dtrDB.CloseDB();
                        _log.WriteLog(Application.StartupPath, "Posting", "UplaodReadDiffTextFileLogs : GetFileLoop", e.ToString(), true);
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UplaodReadDiffTextFileLogs : General", e.ToString(), true);
            }
        }

        public bool TextFileDiffDTRUploading(String[] LogsData, DateTime processDate, DALHelper dalDtr)
        { 
            return TextFileDiffDTRUploading("", LogsData, processDate, "spLogReadingInsertToServerDTR_Diff", dalDtr);
        }

        public bool TextFileDiffDTRUploading(string MappedIDNo, String[] LogsData, DateTime processDate, String StoredPocedure, DALHelper dalDtr)
        {
            bool bRetVal = false;
            int nAffectedRows = 0;
            #region parameters


            ParameterInfo[] paramInsertDTR = new ParameterInfo[9];
            paramInsertDTR[0] = new ParameterInfo("@Tel_IDNo", (MappedIDNo != "" ? MappedIDNo : LogsData[0].Trim()), SqlDbType.VarChar, 15);
            paramInsertDTR[1] = new ParameterInfo("@Tel_LogDate", Convert.ToDateTime(LogsData[1].Trim()).ToString("MM/dd/yyyy"), SqlDbType.VarChar, 10);
            paramInsertDTR[2] = new ParameterInfo("@Tel_LogTime", Convert.ToDateTime(LogsData[1].Trim()).ToString("HHmm"), SqlDbType.Char, 4);//LogsData[2].Trim(), SqlDbType.Char, 4);
            paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", LogsData[3].Trim() == "0" ? "I" : "O", SqlDbType.Char, 1);
            paramInsertDTR[4] = new ParameterInfo("@Tel_StationNo", "TX", SqlDbType.Char, 2);
            paramInsertDTR[5] = new ParameterInfo("@Tel_IsPosted", 0, SqlDbType.Bit);
            paramInsertDTR[6] = new ParameterInfo("@Tel_IsUploaded", 1, SqlDbType.Bit);
            paramInsertDTR[7] = new ParameterInfo("@Usr_Login", "LOGUPLDTXT", SqlDbType.VarChar, 15);
            paramInsertDTR[8] = new ParameterInfo("@LudateTime", Convert.ToDateTime(string.Format("{0} {1}:{2}", Convert.ToDateTime(LogsData[1].Trim()).ToString("MM/dd/yyyy"), LogsData[1].Substring(0, 2), LogsData[1].Substring(2, 2))), SqlDbType.DateTime);

            #endregion

            try
            {
                //dtrDB.BeginTransaction();
                //System.Threading.Thread.Sleep(10);  // Allowing CPU to breath
                nAffectedRows = dalDtr.ExecuteNonQuery(StoredPocedure, CommandType.StoredProcedure, paramInsertDTR);
                //dtrDB.CommitTransaction();
                bRetVal = true;

                try
                {
                    Console.WriteLine(String.Format("\n\tPosting ID {0} \n\t\tDate [{1}] Time[{2}] Type[{3}]  [{4}]"
                                                                                                      , LogsData[0]
                                                                                                      , Convert.ToDateTime(LogsData[1].Trim()).ToString("MM/dd/yyyy")
                                                                                                      , LogsData[2]
                                                                                                      , LogsData[3].Trim() == "I" ? "In" : "Out"
                                                                                                      , LogsData[1]));
                }
                catch { }
                
            }
            catch (Exception ex)
            {
                bRetVal = false;
                //dtrDB.RollBackTransaction();
            }

            return bRetVal;
        }

        public bool AppDiffDTRUploading(string MappedIDNo, String[] LogsData, DateTime processDate, String StoredPocedure, DALHelper dalDtr)
        {
            bool bRetVal = false;
            int nAffectedRows = 0;
            #region parameters

            DateTime dt = DateTime.ParseExact(LogsData[1].Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            ParameterInfo[] paramInsertDTR = new ParameterInfo[11];
            paramInsertDTR[0] = new ParameterInfo("@Tel_IDNo", (MappedIDNo != "" ? MappedIDNo : LogsData[0].Trim()), SqlDbType.VarChar, 15);
            paramInsertDTR[1] = new ParameterInfo("@Tel_LogDate", dt.ToString("MM/dd/yyyy"), SqlDbType.Date);
            paramInsertDTR[2] = new ParameterInfo("@Tel_LogTime", dt.ToString("HHmm"), SqlDbType.Char, 4); //LogsData[2].Trim(), SqlDbType.Char, 4);
            paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", LogsData[3].Trim() == "0" ? "I" : "O", SqlDbType.Char, 1);
            paramInsertDTR[4] = new ParameterInfo("@Tel_StationNo", "MT", SqlDbType.Char, 2);
            paramInsertDTR[5] = new ParameterInfo("@Tel_IsPosted", 0, SqlDbType.Bit);
            paramInsertDTR[6] = new ParameterInfo("@Tel_IsUploaded", 1, SqlDbType.Bit);
            paramInsertDTR[7] = new ParameterInfo("@Tel_Latitude", LogsData[6].Trim(), SqlDbType.Decimal);
            paramInsertDTR[8] = new ParameterInfo("@Tel_Longitude", LogsData[7].Trim(), SqlDbType.Decimal);
            paramInsertDTR[9] = new ParameterInfo("@Usr_Login", "LOGUPLDTXT", SqlDbType.VarChar, 15);
            paramInsertDTR[10] = new ParameterInfo("@LudateTime", dt, SqlDbType.DateTime);

            #endregion

            try
            {
                //dtrDB.BeginTransaction();
                //System.Threading.Thread.Sleep(10);  // Allowing CPU to breath
                nAffectedRows = dalDtr.ExecuteNonQuery(StoredPocedure, CommandType.StoredProcedure, paramInsertDTR);
                //dtrDB.CommitTransaction();
                bRetVal = true;

                try
                {
                    Console.WriteLine(String.Format("\n\tPosting ID {0} \n\t\tDate [{1}] Time[{2}] Type[{3}]  [{4}]"
                                                                                                      , LogsData[0]
                                                                                                      , dt.ToString("MM/dd/yyyy")
                                                                                                      , LogsData[2]
                                                                                                      , LogsData[3].Trim() == "I" ? "In" : "Out"
                                                                                                      , LogsData[1]));
                }
                catch { }

            }
            catch (Exception ex)
            {
                bRetVal = false;
                //dtrDB.RollBackTransaction();
            }

            return bRetVal;
        }

        public bool LEARTextFiletoDTRUploading(String LogsData, DateTime processDate, String StoredPocedure, DALHelper dalDtr, ref bool hasExeption)
        {
            bool bRetVal = false;
            int nAffectedRows = 0;
            #region parameters

            LogsData = LogsData.TrimStart();
            LogsData = LogsData.TrimEnd();
            string EmployeeID = "";
            string LogDate = "";
            string LogTime = "";
            string LogType = "";
            string StationNo = "";
            DateTime LudateTime;
            bool StationNoTx = true;

                //015441 10 01 2013 0400 I
                //015048101520132032I  -> normal
                //015048 10 15 2013 2032 I  -> normal
                //64430088784 10 04 2013 1825 O -> rfid
                EmployeeID = LogsData.Substring(0, LogsData.Length - 13);
                LogDate = LogsData.Substring(LogsData.Length - 13, 2) + "/" + LogsData.Substring(LogsData.Length - 11, 2) + "/" + LogsData.Substring(LogsData.Length - 9, 4);
                LogTime = LogsData.Substring(LogsData.Length - 5, 4);
                LogType = LogsData.EndsWith("I") ? "I" : "O";
                LudateTime = Convert.ToDateTime(string.Format("{0} {1}:{2}", LogDate, LogTime.Substring(0, 2), LogTime.Substring(2, 2)));
           
            

            ParameterInfo[] paramInsertDTR = new ParameterInfo[9];
            paramInsertDTR[0] = new ParameterInfo("@Tel_IDNo", EmployeeID, SqlDbType.VarChar, 15);
            paramInsertDTR[1] = new ParameterInfo("@Tel_LogDate", LogDate, SqlDbType.VarChar, 10);
            paramInsertDTR[2] = new ParameterInfo("@TEL_LOGTIME", LogTime, SqlDbType.Char, 4);//LogsData[2].Trim(), SqlDbType.Char, 4);
            paramInsertDTR[3] = new ParameterInfo("@TEL_LogType", LogType, SqlDbType.Char, 1);
            paramInsertDTR[4] = new ParameterInfo("@Dtr_StationNo", StationNoTx?"TX":StationNo, SqlDbType.Char, 2);
            paramInsertDTR[5] = new ParameterInfo("@Dtr_PostFlag", 0, SqlDbType.Bit);
            paramInsertDTR[6] = new ParameterInfo("@Dtr_UploadedFlag", 1, SqlDbType.Bit);
            paramInsertDTR[7] = new ParameterInfo("@Usr_Login", "LOGUPLDTXT", SqlDbType.VarChar, 15);
            paramInsertDTR[8] = new ParameterInfo("@LudateTime", LudateTime, SqlDbType.DateTime);

            #endregion

            try
            {
                //dtrDB.BeginTransaction();
                nAffectedRows = dalDtr.ExecuteNonQuery(StoredPocedure, CommandType.StoredProcedure, paramInsertDTR);
                if (nAffectedRows == -1)
                {
                    string TestBreak = "";
                }
                //dtrDB.CommitTransaction();
                bRetVal = true;

                try
                {
                    Console.Clear();
                    Console.WriteLine(String.Format("\n\tPosting ID {0} \n\t\tDate [{1}] Time[{2}] Type[{3}]  [{4}]"
                                                                                                      , EmployeeID
                                                                                                      , LogDate
                                                                                                      , LogTime
                                                                                                      , LogType == "I" ? "In" : "Out"
                                                                                                      , LudateTime));
                }
                catch { }

            }
            catch (Exception ex)
            {
                bRetVal = false;
                hasExeption = true;
                //dtrDB.RollBackTransaction();
            }

            return bRetVal;
        }
        
        public void T_DTRDifferentialCreator()
        {
            ParameterInfo[] paramInsertDTR = new ParameterInfo[1];
            paramInsertDTR[0] = new ParameterInfo("@NOTHING", "", SqlDbType.VarChar, 15);
            String sqlGetDiffSP = string.Format(@"USE [{0}]
                                                SELECT COUNT(NAME) AS DIFFSTORPROC FROM SYS.procedures
                                                WHERE name='spLogReadingInsertToServerDTR_Diff'", _dtrDBName);
            String sqlGetT_DTRDiff = string.Format(@"USE [{0}] SELECT COUNT(NAME) AS DTRDIFF FROM SYS.tables WHERE NAME='T_DTRDifferential' ", _dtrDBName);
            #region Create T_DTRDIfferential ...
            String sql = string.Format(@"USE [{0}]
                                        SET ANSI_NULLS ON
                                        SET QUOTED_IDENTIFIER ON
                                        SET ANSI_PADDING ON
                                        CREATE TABLE [dbo].[T_DTRDifferential](
	                                        [Tel_IDNo] [varchar](15) NOT NULL,
	                                        [Tel_LogDate] [varchar](10) NOT NULL,
	                                        [Tel_LogTime] [char](4) NOT NULL,
	                                        [Tel_LogType] [char](1) NOT NULL,
	                                        [Tel_StationNo] [char](2) NOT NULL,
	                                        [Tel_IsPosted] [bit] NOT NULL,
	                                        [Usr_Login] [varchar](15) NOT NULL,
	                                        [LudateTime] [datetime] NOT NULL,
	                                        [Tel_IsUploaded] [bit] NULL,
                                         CONSTRAINT [PK_T_DTRDifferential] PRIMARY KEY CLUSTERED 
                                        (
	                                        [Tel_IDNo] ASC,
	                                        [Tel_LogDate] ASC,
	                                        [Tel_LogTime] ASC,
	                                        [Tel_LogType] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]
                                        SET ANSI_PADDING OFF", _dtrDBName);
            #endregion
            #region Create Differential SP ...
            String sqlCreateSP = string.Format(@"CREATE PROCEDURE [dbo].[spLogReadingInsertToServerDTR_Diff]            
                                                (            
                                                  @Tel_IDNo VARCHAR(15)            
                                                 ,@Tel_LogDate VARCHAR(10)            
                                                 ,@Tel_LogTime CHAR(4)            
                                                 ,@Tel_LogType CHAR(1)            
                                                 ,@Tel_StationNo CHAR(2)            
                                                 ,@Tel_IsPosted BIT            
                                                 ,@Tel_IsUploaded BIT            
                                                 ,@Usr_Login VARCHAR(15)            
                                                 ,@Ludatetime DATETIME            
                                                )            
                                                AS            
                                                IF NOT EXISTS             
                                                  (SELECT 1            
                                                     FROM T_DTRDifferential             
                                                    WHERE Tel_IDNo = @Tel_IDNo            
                                                      AND Tel_LogDate = @Tel_LogDate            
                                                      AND Tel_LogTime = @Tel_LogTime            
                                                      AND Tel_LogType = @Tel_LogType)            
                                                  BEGIN            
                                                   INSERT INTO T_DTRDifferential            
                                                                       (Tel_IDNo            
                                                                       ,Tel_LogDate            
                                                                       ,Tel_LogTime            
                                                                       ,Tel_LogType            
                                                                       ,Tel_StationNo            
                                                                       ,Tel_IsPosted            
                                                                       ,Tel_IsUploaded            
                                                                       ,Usr_Login            
                                                                       ,LudateTime            
                                                                       )            
                                                                VALUES (@Tel_IDNo            
                                                                       ,@Tel_LogDate            
                                                                       ,@TEL_LOGTIME            
                                                                       ,@TEL_LogType            
                                                                       ,@Tel_StationNo            
                                                                       ,@Tel_IsPosted            
                                                                       ,@Tel_IsUploaded            
                                                                       ,@Usr_Login            
                                                                       ,@LudateTime            
                                                                       )                                                                                         
                                                    IF (ISNULL((SELECT Tel_LastSwipe             
                                                         FROM T_EmpLog            
                                                        WHERE Tel_IDNo = @Tel_IDNo), '1900-01-01')            
                                                     <= CAST(@Tel_LogDate + ' ' + LEFT(@TEL_LOGTIME, 2) + ':' + RIGHT(@TEL_LOGTIME, 2) AS DATETIME))            
                                                    BEGIN            
                                                     UPDATE T_EmpLog            
                                                        SET Tel_LastSwipe = CAST(@Tel_LogDate + ' ' + LEFT(@TEL_LOGTIME, 2) + ':' + RIGHT(@TEL_LOGTIME, 2) AS DATETIME)            
                                                           ,Tel_LastLogType = @TEL_LogType            
                                                      WHERE Tel_IDNo = @Tel_IDNo            
                                                    END            
                                                  END", _dtrDBName);
            #endregion
            try
            {
                dtrDB.OpenDB();
                //Create Table
                //dtrDB.BeginTransaction();
                DataSet ds_existing = dtrDB.ExecuteDataSet(sqlGetT_DTRDiff);
                //dtrDB.CommitTransaction();
                DataTable T_EXISTING = ds_existing.Tables[0];
                int affected = Convert.ToInt16(T_EXISTING.Rows[0]["DTRDIFF"].ToString());
                if (affected == 0)
                    try
                    {
                        //dtrDB.BeginTransaction();
                        dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInsertDTR);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        //dtrDB.RollBackTransaction();
                        _log.WriteLog(Application.StartupPath, "Posting", "T_DTRDifferentialCreator : Create Diff Table", e.ToString(), true);
                    }

                //Create SP
                //dtrDB.BeginTransaction();
                ds_existing = dtrDB.ExecuteDataSet(sqlGetDiffSP);
                //dtrDB.CommitTransaction();
                T_EXISTING = ds_existing.Tables[0];
                affected = Convert.ToInt16(T_EXISTING.Rows[0]["DIFFSTORPROC"].ToString());
                dtrDB.CloseDB();
                DALHelper dal = new DALHelper();
                using (SqlConnection connection = dal.getConnectionDTR())
                {
                    SqlCommand command = new SqlCommand(sqlCreateSP, connection);
                    command.CommandType = CommandType.Text;
                    connection.Open();

                    if (affected == 0)
                        try
                        {
                            command.ExecuteScalar();
                        }
                        catch (Exception e)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "T_DTRDifferentialCreator : Create Diff STored Proc", e.ToString(), true);
                        }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                dtrDB.CloseDB();
                _log.WriteLog(Application.StartupPath, "Posting", "T_DTRDifferentialCreator : General", e.ToString(), true);
            }
        }
        
        public void PostFlagtoZero(DateTime Processdate, String LedgerDBName, String DtrDBName)
        {
            try
            {
                int Affected = 0;
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                String sql = string.Format(@"UPDATE {0}.dbo.{2}
	                                                SET Tel_IsPosted=0
                                                WHERE  
                                                CONVERT(DATETIME,Tel_LogDate) = @ProcessDate
                                                AND 
                                                Tel_IsPosted=1", DtrDBName, LedgerDBName, Globals.T_EmpDTR);

                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    Affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "PostFlagto_ZERO : Committed", "", true);
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "PostFlagto_ZERO : RollBack", e.ToString(), true);
                    //dtrDB.RollBackTransaction();
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "PostFlagto_ZERO : General", e.ToString(), true);
            }
        }
        
        
        //public void RepostLogTrail(String LedgerDBName, DateTime ProcessDate, DALHelper dal)
        //{
        //    RepostLogTrail(LedgerDBName, ProcessDate, dal, "");
        //}

        //public void RepostLogTrail(String LedgerDBName, DateTime ProcessDate, DALHelper dal, string FilterLedgerExt)
        //{
        //    try
        //    {
        //        if (!Globals.isRepostLedgerTrail) //If this key is false then repost trail is not executed
        //            return;

        //        ParameterInfo[] paramInfo = new ParameterInfo[1];
        //        paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
        //        String sql = string.Format(QueryRepostLogTrail, LedgerDBName, FilterLedgerExt);
        //        try
        //        {
        //            //dtrDB.OpenDB();
        //            //dtrDB.BeginTransaction();
        //            DsRepostLogTrail = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
        //            //dtrDB.CommitTransaction();
        //            //_log.WriteLog(Application.StartupPath, "Posting", "Repost Log Trail : Committed", "", true);
        //        }
        //        catch (Exception e)
        //        {
        //            _log.WriteLog(Application.StartupPath, "Posting", "Repost Log Trail : RollBack", e.ToString(), true);
        //            //dtrDB.RollBackTransaction();
        //        }
        //        finally
        //        {
        //            //dal.CloseDB();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.WriteLog(Application.StartupPath, "Posting", "Repost Log Trail : General", e.ToString(), true);
        //    }
        //}
        //FUNCTION COPIED FROM BASECONSOLE

        private DataTable GetDTRMapping(DALHelper dalDtr)
        {
            try
            {
                #region Query
                string strQuery = string.Format(@"
                    SELECT * FROM {0}
                    WHERE Tel_RecordStatus = 'A'", CommonConstants.TableName.T_EmpLog);
                #endregion

                DataTable dt = dalDtr.ExecuteDataSet(strQuery).Tables[0];
                return dt;
            }
            catch
            {
                return null;
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
        public void DtrTextFileHandler(String FileFolder, DateTime processDate, DALHelper dalDtr)
        {
            try
            {
                if (!Directory.Exists(FileFolder))
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "Dtr text file directory does not exist.", "", true);
                    return;
                }
                if (!Directory.Exists(FileFolder + @"\Processed"))
                {
                    Directory.CreateDirectory(FileFolder + @"\Processed");
                }
                _log.WriteLog(Application.StartupPath, "Posting", "Convert Logfiles Started", "", true);

                String LogsLine = "";
                String[] LogsData;
                string[] DtrLogFiles = Directory.GetFiles(FileFolder);
                String ProgressFileName = "";

                foreach (string _dtrLogFile in DtrLogFiles)
                {
                    if (_dtrLogFile.Contains(".txt") || _dtrLogFile.Contains(".log") || _dtrLogFile.Contains(".Log") || _dtrLogFile.Contains(".dat"))
                    {
                        try
                        {
                            ProgressFileName = _dtrLogFile.Substring(0, _dtrLogFile.Length - 4) + ".prg";
                             
                            //Create progress file, indicate that text file is under processing.
                            if (!File.Exists(ProgressFileName))
                            {
                                using (StreamWriter write = new StreamWriter(ProgressFileName))
                                {
                                    write.Dispose();   
                                }
                            }
                            
                        }
                        catch
                        { }

                        try
                        {
                            #region ID Mapping
                            DataTable dt = GetDTRMapping(dalDtr);
                            #endregion

                            using (System.IO.StreamReader objReader = new System.IO.StreamReader(_dtrLogFile))
                            {
                                int x = 0;
                                while (objReader.Peek() != -1)
                                {
                                    LogsLine = objReader.ReadLine();
                                    if (LogsLine.Contains("\t")) LogsData = LogsLine.Split('\t');
                                    else LogsData = LogsLine.Split(',');
                                    x = x+1;

                                    if (LogsLine.Trim() != String.Empty)
                                    {
                                        try
                                        {
                                            if (LogsData.Length == 6)
                                            {
                                                string IDNo = LogsData[0].Trim();
                                                try
                                                {
                                                    DataRow[] dr = dt.Select(string.Format(@"Tel_RFID = '{0}'", IDNo));
                                                    if (dr.Length > 0) IDNo = dr[0]["Tel_IDNo"].ToString();
                                                }
                                                catch
                                                {
                                                    IDNo = LogsData[0].Trim();
                                                }
                                                TextFileDiffDTRUploading(IDNo, LogsData, processDate, "spLogReadingInsertToServerDTR_2", dalDtr);
                                            }
                                            if (LogsData.Length == 8)
                                            {
                                                string IDNo = LogsData[0].Trim();
                                                try
                                                {
                                                    DataRow[] dr = dt.Select(string.Format(@"Tel_RFID = '{0}'", IDNo));
                                                    if (dr.Length > 0) IDNo = dr[0]["Tel_IDNo"].ToString();
                                                }
                                                catch
                                                {
                                                    IDNo = LogsData[0].Trim();
                                                }
                                                AppDiffDTRUploading(IDNo, LogsData, processDate, "spLogReadingInsertToServerDTR_3", dalDtr);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //TO PROVIDE TRAIL OF ALL LOGS THAT WERE NOT COPIED TO DTR
                                            GenerateTextFile("ErrorTXTfileToDTR " + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["PayrollErrorLogPath"], String.Format("*Error on line no.{0} of {1} \r\n\n*ERROR:{2} \r\n\n*Error type: {3}", x, _dtrLogFile, LogsData[0], ex.Message));
                                            //END
                                        }
                                    }
                                }

                                objReader.Close();
                            }

                            try
                            {
                                //if (!hasLearTextFileException)
                                //{
                                    string _FileProcessDateTime = "[" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + "] ";
                                    string _dtrLogProcessedFile = _dtrLogFile.Replace(FileFolder, (FileFolder.EndsWith(@"\") ? (FileFolder + @"Processed\" + _FileProcessDateTime) : (FileFolder + @"\Processed\" + _FileProcessDateTime)));
                                    _dtrLogProcessedFile = _dtrLogProcessedFile.Substring(0, _dtrLogProcessedFile.Length - 4) + ".prc";
                                    if(File.Exists(_dtrLogProcessedFile))
                                    {
                                        _dtrLogProcessedFile = _dtrLogProcessedFile.Substring(0, _dtrLogProcessedFile.Length - 4) + " - Copy" + DateTime.Now.ToLongDateString() +".prc";
                                    }

                                    File.Move(_dtrLogFile, _dtrLogProcessedFile);

                                    try
                                    {
                                        if (File.Exists(ProgressFileName))
                                        {
                                            File.Delete(ProgressFileName);
                                        }
                                    }
                                    catch { }
                                //}
                            }
                            catch { }
                        }
                        catch (Exception e)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "DtrTextFileHandler : GetFileLoop", e.ToString(), true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "DtrTextFileHandler : General", e.ToString(), true);
            }
        }

        #endregion

        #region Logic For MinMax Posting

        //public DataSet GetMinMaxLogs(DateTime Processdate, String LedgerDBName, String LedgerTable)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        {
        //            ParameterInfo[] paramInfo = new ParameterInfo[1];
        //            paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
        //            string DENSO = "SCM.Msh_ShiftOut1";
        //            if (Globals.isCPHAutoShift)
        //                //set to chiyoda condition
        //                DENSO = "SCM.Msh_ShiftIn2";
        //            #region MinMax Query
        //            string sql = string.Format(@"SELECT 
	       //                                         DISTINCT Tel_IDNo AS EMPLOYEEID 
	       //                                         --IN 1
	       //                                         ,(SELECT MIN(Tel_LOGTIME) FROM {0}..{4}
	       //                                             WHERE CONVERT(DATETIME,Tel_LOGDATE,101)=@Processdate
	       //                                             AND Tel_LOGTYPE='I'
	       //                                             AND Tel_IDNo=DTR.Tel_IDNo
	       //                                             AND CONVERT(INT,Tel_LOGTIME)<CONVERT(INT,SCM.Msh_ShiftOut1)
	       //                                             AND Tel_IsPosted=0) AS IN1
	       //                                         --OUT1
	       //                                         ,(SELECT MAX(Tel_LOGTIME) FROM {0}..{4}
	       //                                             WHERE CONVERT(DATETIME,Tel_LOGDATE,101)=@Processdate
	       //                                             AND Tel_LOGTYPE='O'
	       //                                             AND Tel_IDNo=DTR.Tel_IDNo
	       //                                             AND CONVERT(INT,Tel_LOGTIME)<=CONVERT(INT,{3})
	       //                                             AND Tel_IsPosted=0) AS OUT1
	       //                                         --IN2
	       //                                         ,(SELECT MIN(Tel_LOGTIME) FROM {0}..{4}
	       //                                             WHERE CONVERT(DATETIME,Tel_LOGDATE,101)=@Processdate
	       //                                             AND Tel_LOGTYPE='I'
	       //                                             AND Tel_IDNo=DTR.Tel_IDNo
	       //                                             AND CONVERT(INT,Tel_LOGTIME)>=CONVERT(INT,SCM.Msh_ShiftOut1)
	       //                                             AND Tel_IsPosted=0) AS IN2
	       //                                         --OUT2
	       //                                         ,(SELECT MAX(Tel_LOGTIME) FROM {0}..{4}
	       //                                             WHERE CONVERT(DATETIME,Tel_LOGDATE,101)=@Processdate
	       //                                             AND Tel_LOGTYPE='O'
	       //                                             AND Tel_IDNo=DTR.Tel_IDNo
	       //                                             AND CONVERT(INT,Tel_LOGTIME)>CONVERT(INT,SCM.Msh_ShiftOut1)
	       //                                             AND Tel_IsPosted=0) AS OUT2
	       //                                             --,Ttr_IDNo AS EMPLOYEEID
	       //                                             ,CONVERT(VARCHAR(20),Ttr_Date,101) AS PROCESSDATE
	       //                                             ,Ttr_ShiftCode AS SHIFT
	       //                                             ,Msh_ShiftIn1 AS SHFTIN
	       //                                             ,Msh_ShiftOut1 AS BRKIN
	       //                                             ,Msh_ShiftIn2 AS BRKOUT
	       //                                             ,Msh_ShiftOut2 AS SHFTOUT
	       //                                             ,[Ttr_ActIn_1]AS LEDGERIN1
        //                                                ,[Ttr_ActOut_1] AS LEDGEROUT1
        //                                                ,[Ttr_ActIn_2] AS LEDGERIN2
        //                                                ,[Ttr_ActOut_2] AS LEDGEROUT2
        //                                        FROM {0}..{4} DTR
	       //                                         JOIN
	       //                                         {1}..{2}
        //                                                ON Ttr_IDNo=Tel_IDNo
        //                                                AND CONVERT(DATETIME,Tel_LOGDATE,101)=@Processdate
        //                                                AND Ttr_Date=@Processdate
		      //                                      JOIN {1}..M_Shift SCM
		      //                                          ON Ttr_ShiftCode=Msh_ShiftCode", _dtrDBName, LedgerDBName, LedgerTable, DENSO, Globals.Used_T_DTR);
        //            #endregion

        //            try
        //            {
        //                dtrDB.OpenDB();
        //                //dtrDB.BeginTransaction();
        //                ds = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
        //                //dtrDB.CommitTransaction();
        //            }
        //            catch (Exception e)
        //            {
        //                _log.WriteLog(Application.StartupPath, "Posting", "MINMAXPosting : RollBack", e.ToString(), true);
        //                //dtrDB.RollBackTransaction();
        //            }
        //            finally
        //            {
        //                dtrDB.CloseDB();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.WriteLog(Application.StartupPath, "Posting", "MINMAXPosting : General", e.ToString(), true);
        //    }
        //    return ds;
        //}

        //public void StartMinMaxPosting(DateTime processdate, String LedgerDBName, String LedgerTabe)
        //{
        //    if (!Globals.isDensoAutoShift  || !Globals.isFirstINLastOut)
        //            return;
        //    try
        //    {
        //        Console.WriteLine("\tStarting Min Max Posting");

        //        DataSet ds = GetMinMaxLogs(processdate, LedgerDBName, LedgerTabe);
        //        DataTable dt = ds.Tables[0];
        //        ds.Dispose();
        //        DataRow dr;
        //        int affected = 0;

        //        ParameterInfo[] paramInfo = new ParameterInfo[10];

        //        if (dt.Rows.Count > 0)
        //        {
        //            int rowcnt = dt.Rows.Count;
        //            dtrDB.OpenDB();
        //            _log.WriteLog(Application.StartupPath, "Posting", "Min/Max Posting : Started", "", true);
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                try 
        //                {
        //                    Globals.Progress = ((Convert.ToDouble(i)) / Convert.ToDouble(rowcnt)) * 100;
        //                }
        //                catch { }
        //                dt.Rows[i]["IN1"] = checkifnull(dt.Rows[i]["IN1"].ToString());
        //                dt.Rows[i]["OUT1"] = checkifnull(dt.Rows[i]["OUT1"].ToString());
        //                dt.Rows[i]["IN2"] = checkifnull(dt.Rows[i]["IN2"].ToString());
        //                dt.Rows[i]["OUT2"] = checkifnull(dt.Rows[i]["OUT2"].ToString());
        //                dt.Rows[i]["LEDGERIN1"] = FILO(dt.Rows[i]["IN1"].ToString(), dt.Rows[i]["LEDGERIN1"].ToString(), true);
        //                dt.Rows[i]["LEDGEROUT1"] = FILO(dt.Rows[i]["OUT1"].ToString(), dt.Rows[i]["LEDGEROUT1"].ToString(), false);
        //                dt.Rows[i]["LEDGERIN2"] = FILO(dt.Rows[i]["IN2"].ToString(), dt.Rows[i]["LEDGERIN2"].ToString(), true);
        //                dt.Rows[i]["LEDGEROUT2"] = FILO(dt.Rows[i]["OUT2"].ToString(), dt.Rows[i]["LEDGEROUT2"].ToString(), false);

        //                //Cleaning Logs by time gap
        //                String[] CleanGap = new String[4];
        //                CleanGap[0] = dt.Rows[i]["LEDGERIN1"].ToString();
        //                CleanGap[1] = dt.Rows[i]["LEDGEROUT1"].ToString();
        //                CleanGap[2] = dt.Rows[i]["LEDGERIN2"].ToString();
        //                CleanGap[3] = dt.Rows[i]["LEDGEROUT2"].ToString();
        //                CleanGap = CleanLogs(CleanGap);
        //                dt.Rows[i]["LEDGERIN1"] = CleanGap[0];
        //                dt.Rows[i]["LEDGEROUT1"] = CleanGap[1];
        //                dt.Rows[i]["LEDGERIN2"] = CleanGap[2];
        //                dt.Rows[i]["LEDGEROUT2"] = CleanGap[3];

        //                paramInfo[0] = new ParameterInfo("@EMPLOYEEID", dt.Rows[i]["EmployeeID"], SqlDbType.VarChar, 15);
        //                paramInfo[1] = new ParameterInfo("@ProcessDate", processdate.ToShortDateString(), SqlDbType.DateTime);
        //                paramInfo[2] = new ParameterInfo("@IN1", dt.Rows[i]["IN1"], SqlDbType.Char, 4);
        //                paramInfo[3] = new ParameterInfo("@OUT1", dt.Rows[i]["OUT1"], SqlDbType.Char, 4);
        //                paramInfo[4] = new ParameterInfo("@IN2", dt.Rows[i]["IN2"], SqlDbType.Char, 4);
        //                paramInfo[5] = new ParameterInfo("@OUT2", dt.Rows[i]["OUT2"], SqlDbType.Char, 4);
        //                paramInfo[6] = new ParameterInfo("@LEDGERIN1", dt.Rows[i]["LEDGERIN1"], SqlDbType.Char, 4);
        //                paramInfo[7] = new ParameterInfo("@LEDGEROUT1", dt.Rows[i]["LEDGEROUT1"], SqlDbType.Char, 4);
        //                paramInfo[8] = new ParameterInfo("@LEDGERIN2", dt.Rows[i]["LEDGERIN2"], SqlDbType.Char, 4);
        //                paramInfo[9] = new ParameterInfo("@LEDGEROUT2", dt.Rows[i]["LEDGEROUT2"], SqlDbType.Char, 4);

        //                //Remove Mid logs if  there is IN1 and OUT2
        //                if (Globals.isDensoAutoShift)
        //                {
        //                    if (dt.Rows[i]["LEDGEROUT2"].ToString().Trim() != "0000" && dt.Rows[i]["LEDGERIN1"].ToString().Trim() != "0000")
        //                    {
        //                        paramInfo[7] = new ParameterInfo("@LEDGEROUT1", "0000", SqlDbType.Char, 4);
        //                        paramInfo[8] = new ParameterInfo("@LEDGERIN2", "0000", SqlDbType.Char, 4);
        //                    }
        //                }


        //                String sql = String.Format(@"UPDATE {0}..{1}
        //                                SET Ttr_ActIn_1=@LEDGERIN1
        //                                    ,Ttr_ActOut_1=@LEDGEROUT1
        //                                    ,Ttr_ActIn_2=@LEDGERIN2
        //                                    ,Ttr_ActOut_2=@LEDGEROUT2
        //                                    ,USR_LOGIN='LOGUPLDSRVC'
        //                                    ,LUDATETIME=GETDATE()
        //                                WHERE Ttr_Date=@ProcessDate
        //                                AND Ttr_IDNo=@EMPLOYEEID", LedgerDBName, LedgerTabe);
        //                //updating post flag
        //                //Nilo Modified 20131112 : Update post flag to 1 to all log with in Min and Max.
        //                string MaxOut = "";
        //                if (dt.Rows[i]["OUT2"].ToString().Trim() != "0000")
        //                {
        //                    MaxOut = String.Format("AND TEL_LOGTIME <= '{0}'", dt.Rows[i]["OUT2"].ToString());
        //                }
        //                else if (dt.Rows[i]["IN2"].ToString().Trim() != "0000")
        //                {
        //                    MaxOut = String.Format("AND TEL_LOGTIME <= '{0}'", dt.Rows[i]["IN2"].ToString());
        //                }
        //                else if (dt.Rows[i]["OUT1"].ToString().Trim() != "0000")
        //                {
        //                    MaxOut = String.Format("AND TEL_LOGTIME <= '{0}'", dt.Rows[i]["OUT1"].ToString());
        //                }
        //                else
        //                {
        //                    MaxOut = String.Format("AND TEL_LOGTIME <= '{0}'", dt.Rows[i]["IN1"].ToString());
        //                }

        //                string sqlDTR = string.Format(@"UPDATE {0}..{1}
        //                                                   SET Tel_IsPosted=1
        //                                                      ,USR_LOGIN='LOGUPLDSRVC'
        //                                                    --,LUDATETIME=GETDATE()
        //                                                 WHERE CONVERT(DATETIME,Tel_LogDate,101)=@ProcessDate
        //                                                   AND TEL_LOGTIME >= '{2}' 
        //                                                   {3}
        //                                                   AND Tel_IDNo = @EMPLOYEEID"
        //                                                , _dtrDBName
        //                                                , Globals.Used_T_DTR
        //                                                , dt.Rows[i]["IN1"].ToString()
        //                                                , MaxOut);
        //                try
        //                {
        //                    dtrDB.BeginTransaction();
        //                    affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);

        //                    affected = dtrDB.ExecuteNonQuery(sqlDTR, CommandType.Text, paramInfo);
        //                    dtrDB.CommitTransaction();
        //                }
        //                catch (Exception e)
        //                {
        //                    dtrDB.RollBackTransaction();
        //                    _log.WriteLog(Application.StartupPath, "Posting", "startMaxPosting : RollBack", e.ToString(), true);
        //                }
        //            }
        //            _log.WriteLog(Application.StartupPath, "Posting", "Min/Max Posting : Committed", "", true);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.WriteLog(Application.StartupPath, "Posting", "startMaxPosting : General", e.ToString(), true);
        //    }
        //    finally
        //    {
        //        dtrDB.CloseDB();
        //    }
        //}

        //MIN MAX POSTING PRIVATE LOG CONTROLLING FUNCTIONS
        private string checkifnull(string LOG)
        {
            if (LOG == "")
                LOG = "0000";
            return LOG;
        }

        private string WithGap(String Log, bool Add)
        {
            int hh = Convert.ToInt16(Log.Substring(0, 2));
            int mm = Convert.ToInt16(Log.Substring(2, 2));
            if (!Add)
            {
                if ((mm - Convert.ToInt16(Globals.TIMEGAP)) < 0)
                {
                    hh = hh - 1;
                    mm = mm + 60;
                }
                mm = mm - Convert.ToInt16(Globals.TIMEGAP);
            }
            else
            {
                if ((mm + Convert.ToInt16(Globals.TIMEGAP)) > 60)
                {
                    hh = hh + 1;
                    mm = mm + Convert.ToInt16(Globals.TIMEGAP) - 60;
                }
                mm = mm - Convert.ToInt16(Globals.TIMEGAP);
            }
            return string.Format("{0:00}{1:00}", hh, mm);
        }
        //Military Time Subtraction for Time Gap
        private string[] CleanLogs(string[] Log)
        {
            //OUT1 > IN1 and IN1 is not 0000 then remove OUT1
            if (Convert.ToInt16(Log[0]) >= Convert.ToInt16(WithGap(Log[1], false)) && Log[0].Trim() != "0000")
                Log[1] = "0000";
            //IN2 > OUT2 and OUT2 is not 0000 then remove IN2
            if (Convert.ToInt16(Log[2]) >= Convert.ToInt16(WithGap(Log[3], false)) && Log[3].Trim() != "0000")
                Log[2] = "0000";
            //Check if no out to because value if OUT2 is same to OUT1 
            //Then remove OUT2
            //For CPH
            if (Log[1].Trim().Equals(Log[3].Trim()))
                Log[3] = "0000";
            return Log;
        }

        private string FILO(String DTR, String LEDGER, bool IO)
        {
            if (IO)
            {
                //IN condition
                if ((Convert.ToInt16(DTR) < Convert.ToInt16(LEDGER) && DTR != "0000") || LEDGER == "0000")
                    return DTR;
                else
                    return LEDGER;
            }
            else
            {
                //OUT condition
                if ((Convert.ToInt16(DTR) > Convert.ToInt16(LEDGER) && DTR != "0000") || LEDGER == "0000")
                    return DTR;
                else
                    return LEDGER;
            }
        }

        #endregion

        #region Manipulate the shift code 
        //For Changing
        public void ManipulateShiftCodeSetDataSet(DateTime ProcessDate, String LedgerDBName, DALHelper dal)
        {
            try
            {
                
                if (!CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", false))
                {
                    return;
                }

                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.ToShortDateString(), SqlDbType.DateTime);

                if (CommonProcedures.GetAppSettingConfigBool("DefaultShift", true))
                {
                    // Shift code manipulation for Holiday and Restday
                    string sqlHolRest = string.Format(QueryManipulateEquivalentShiftHolidayRestday, LedgerDBName, Globals.T_TimeRegister);

                    try
                    {
                        //dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        DsManipulateEquivalentShiftHolidayRestday = dal.ExecuteDataSet(sqlHolRest, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        //dtrDB.RollBackTransaction();
                        _log.WriteLog(Application.StartupPath, "Posting", "ManipulateShiftCode : RollBack", e.ToString(), true);
                    }
                    finally
                    {
                        //dtrDB.CloseDB();
                    }

                    //wednesday and leave : default shift handling WEDNESDAY CPH HARDCODED
                    #region CPH Query
                    string sqlWedLeave = "";
                    //if (Globals.isCPHAutoShift)
                    //    sqlWedLeave = string.Format(QueryManipulateDefaultShiftWednesdayAndLeaveCPH, LedgerDBName, Globals.T_Ledger);
                    #endregion
                    #region DENZO Query
                    //Denzo default shift for with advance type OT
                    //if (Globals.isDensoAutoShift)
                    //    sqlWedLeave = string.Format(QueryManipulateDefaultShiftOvertimeAndLeaveDenso, LedgerDBName, Globals.T_Ledger);
                    #endregion

                    try
                    {
                        //dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        DsManipulateDefaultShiftCPHandDenso = dal.ExecuteDataSet(sqlWedLeave, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "ManipulateShiftCode : RollBack", e.ToString(), true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        //dtrDB.CloseDB();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "ManipulateShiftCode : General", e.ToString(), true);
            }
        }

        public void FlexShiftSetDataSet(DateTime ProcessDate, String LedgerDBName, DALHelper dal)
        {
            try
            {
                if (!CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", false))
                {
                    return;
                }
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@Processdate", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
                
                    //flex shift handling
                    String sqlFlexShift = "";
                    String sqlShiftMV = "";
                    #region CPH Query
                    //if (Globals.isCPHAutoShift)
                    //{
                    //    sqlFlexShift = String.Format(QueryFlexShiftCPH, LedgerDBName, Globals.T_Ledger, GetPositionExcluded());
                    //}
                    #endregion
                    #region LEAR Query 
                    //if (Globals.isLEARAutoShift)
                    //{
                    //    sqlFlexShift = string.Format(QueryFlexShiftLear, LedgerDBName, Globals.T_Ledger);
                    //}
                    #endregion
                    #region Denzo Query
                    //if (Globals.isDensoAutoShift)
                    //{
                    //    #region Query Flex Shift
                    //    sqlFlexShift = String.Format(QueryFlexShiftDenso, LedgerDBName, Globals.T_Ledger, GetEmploymentExcluded());
                    //    #endregion
                    //    #region Query Approved Shift Movement
                    //    sqlShiftMV = String.Format(QueryManipulateShiftwithShiftMovementDenso, LedgerDBName, Globals.T_Ledger, GetEmploymentExcluded());
                    //    #endregion
                    //}
                    #endregion
                    try
                    {
                        if (!string.IsNullOrEmpty(sqlFlexShift.Trim()))
                        {
                            //dtrDB.OpenDB();
                            //dtrDB.BeginTransaction();
                            DsFlexShifting = dal.ExecuteDataSet(sqlFlexShift, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                        }
                        if (!string.IsNullOrEmpty(sqlShiftMV.Trim()))
                        {
                            //dtrDB.BeginTransaction();
                            DsManipulateShiftwithShiftMovementDenso = dal.ExecuteDataSet(sqlShiftMV, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                        }
                    }
                    catch (Exception e)
                    {
                        //dtrDB.RollBackTransaction();
                        _log.WriteLog(Application.StartupPath, "Posting", "FlexShift : RollBack", e.ToString(), true);
                    }
                    finally
                    {
                        //dtrDB.CloseDB();
                    }
                
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "FlexShift : General", e.ToString(), true);
            }
        }

        
        #endregion

        #region Handling Advance Over Time Application

        public DataSet OTAdvanceApplication(DateTime processdate, String LedgerDBName, String LedgerCompanyCode, DALHelper dal)
        {
            DataSet dsOT = new DataSet();

            //if (Globals.isDensoAutoShift)
            //    return dsOT;

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ProcessDate", processdate.ToShortDateString());

            String sqlOTApp = String.Format(@"select Ttr_IDNo AS EmployeeID
                                             , Ttr_Date AS OTDate
                                             , Ttr_ShiftCode AS ShiftCode
                                             , Tot_StartTime AS StartTime
                                             , Tot_EndTime AS EndTime
                                             ,{0}.dbo.addMinutes(Tot_StartTime, (Convert(int, RIGHT(Tot_StartTime,2)) - Convert(int, RIGHT(Msh_ShiftOut2,2))) * -1) AS [NewStartTime]
                                             ,{0}.dbo.addMinutes(Tot_EndTime
                                                 , (Convert(int, RIGHT(Tot_StartTime,2)) - Convert(int, RIGHT(Msh_ShiftOut2,2))) * -1
                                                 ) AS [NewEndTime]
                                                 , Tot_OvertimeHours
                                            FROM {0}..{1}
                                            INNER JOIN {0}..{2} on Ttr_IDNo  = Tot_IDNo and Ttr_Date = Tot_OvertimeDate
                                            INNER JOIN {4}..M_Shift on Msh_ShiftCode = Ttr_ShiftCode
                                                AND Msh_CompanyCode = '{3}'
                                            WHERE Msh_ShiftOut2 <> Tot_StartTime
                                            AND LEFT(Msh_ShiftOut2,2) = LEFT(Tot_StartTime,2)
                                            AND (Ttr_ActIn_1 <> '0000' or Ttr_ActIn_2 <> '0000')
                                            AND Ttr_Date = @ProcessDate", LedgerDBName, Globals.T_Overtime, Globals.T_TimeRegister, LedgerCompanyCode, Globals.CentralProfile);
            sqlOTApp = sqlOTApp + (false ? "" : " AND Ttr_DayCode = 'REG'");
            try
            {
                //dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                dsOT = dal.ExecuteDataSet(sqlOTApp, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "Posting", "OTAdvanceApplication : RollBack", e.ToString(), true);
            }
            finally
            {
                //dtrDB.CloseDB();
            }
            return dsOT;
        }

        public void UpdateEmployeeAdvanceOTApp(DateTime processdate, String LedgerDBName, String LedgerCompanyCode, DALHelper dal)
        {
            //Updating advance OT application

            try
            {
                if (!CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", false))
                {
                    return;
                }

                DataSet dsOT = OTAdvanceApplication(processdate, LedgerDBName, LedgerCompanyCode, dal);
                DataTable dtOT = dsOT.Tables[0];
                DataRow drOT;
                int affected = 0;
                int rowcnt = 0;

                ParameterInfo[] paramInfo = new ParameterInfo[7];

                if (dtOT.Rows.Count > 0)
                {
                    //dtrDB.OpenDB();

                    for (int i = 0; i < dtOT.Rows.Count; i++)
                    {
                        ++rowcnt;
                        Console.WriteLine("\tApply Flex OT {0} of {1}  [{2}]", rowcnt, dtOT.Rows.Count, processdate.ToShortDateString());

                        paramInfo[0] = new ParameterInfo("@EmployeeID", dtOT.Rows[i]["EmployeeID"], SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@OTDate", dtOT.Rows[i]["OTDate"], SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@StartTime", dtOT.Rows[i]["StartTime"], SqlDbType.Char, 4);
                        paramInfo[3] = new ParameterInfo("@EndTime", dtOT.Rows[i]["EndTime"], SqlDbType.Char, 4);
                        paramInfo[4] = new ParameterInfo("@NewStartTime", dtOT.Rows[i]["NewStartTime"], SqlDbType.Char, 4);
                        paramInfo[5] = new ParameterInfo("@NewEndTime", dtOT.Rows[i]["NewEndTime"], SqlDbType.Char, 4);
                        paramInfo[6] = new ParameterInfo("@Eot_OvertimeHour", dtOT.Rows[i]["Eot_OvertimeHour"], SqlDbType.Decimal);

                        String sql = String.Format(@"UPDATE {0}..{1}
                                        SET Tot_StartTime=@NewStartTime
                                           ,Tot_EndTime=@NewEndTime
                                           ,Usr_Login='LOGUPLDSRVC'
                                        WHERE Tot_IDNo=@EmployeeID
                                        AND CONVERT(VARCHAR,Tot_OvertimeDate,101)=@OTDate
                                        AND CONVERT(INT,Tot_StartTime)=@StartTime
                                        AND CONVERT(INT,Tot_EndTime)=@EndTime", LedgerDBName, Globals.T_Overtime);
                        try
                        {
                            //dtrDB.BeginTransaction();
                            affected = dal.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                            _log.WriteLog(Application.StartupPath, "AdvanceOTApplicationORIG.Log", "",
                            String.Format("{0},{1},{2},{3},{4},{5},{6}"
                            , dtOT.Rows[i]["EmployeeID"].ToString().Trim()
                            , dtOT.Rows[i]["OTDate"].ToString().Trim()
                            , dtOT.Rows[i]["StartTime"].ToString().Trim()
                            , dtOT.Rows[i]["EndTime"].ToString().Trim()
                            , dtOT.Rows[i]["NewStartTime"].ToString().Trim()
                            , dtOT.Rows[i]["NewEndTime"].ToString().Trim()
                            , dtOT.Rows[i]["Eot_OvertimeHour"].ToString().Trim())
                            , true);
                        }
                        catch (Exception e)
                        {
                            //dtrDB.RollBackTransaction();
                            _log.WriteLog(Application.StartupPath, "Posting", "UpdateEmployeeAdvanceOTApp : RollBack", e.ToString(), true);
                        }
                    }

                    //dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UpdateEmployeeAdvanceOTApp : General", e.ToString(), true);
            }
        }

        #endregion
    }

    #region Uploading Global Variables

    public static class Globals
    {
        public static bool RegularShiftSched            = false;
        public static bool IsOUTBeforeIN                = true;
        public static string EmloyeeIDPrevDate          = String.Empty;
        public static string EmployeeIDCurrentTOProcess = String.Empty;
        public static DateTime LogDateTimePrevDate;
        public static string ShiftCodePrevDate          = String.Empty;
        public static string CapturedOutOccurence       = String.Empty;
        public static bool IsFourPackets                = true;
        public static string LedgerDBName               = String.Empty;
        public static string LedgerCompanyCode          = String.Empty;
        public static bool FromTextFilePosting          = false;
        public static bool isLedgerHist                 = false;
        public static bool isFirstINLastOut             = true;
        public static string T_TimeRegister             = "T_EmpTimeRegister";
        public static string T_Overtime                 = "T_EmpOvertime";
        public static string T_EmpDTR                   = "T_EmpDTR"; 
        public static string NXTDUSER                   = "LOGUPLDONXTD";
        public static bool isServicePost                = false;
        public static bool isOverwrite                  = false;
        public static bool isTextFilePosting            = CommonProcedures.GetAppSettingConfigBool("TextFilePosting", false);
        public static string DTRDBName                  = CommonProcedures.GetAppSettingConfigString("DBNameDTR", string.Empty);
        public static bool isSoftPostingEnable          = CommonProcedures.GetAppSettingConfigBool("SoftPostingEnable", false);
        public static bool isAutoRepostLogsViaLogControl = CommonProcedures.GetAppSettingConfigBool("AutoRepostLogsViaLogControl", true);
        public static bool isAutoChangeShift            = CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", false);
        public static string ShiftPocket                = CommonProcedures.GetAppSettingConfigString("ChangeShiftPocket", string.Empty);
        public static bool isOverLoadingRetrieval       = false;
        public static DateTime LedgerMinDate            = DateTime.UtcNow;
        public static double Progress                   = 0;
        public static string ProgressProcess            = string.Empty;
        public static string CurrentProcess             = string.Empty;
        public static int GenericCount                  = 0;
        public static int Count                         = 0;
        public static bool isLedgerMindate              = false;
        public static string CentralProfile             = CommonProcedures.GetAppSettingConfigString("CentralDBName", "", true);
        public static bool bLogTypePostingEnable        = CommonProcedures.GetAppSettingConfigBool("LogTypePostingEnable", false);

        public static double TIMEGAP                    = 0;
        public static int POCKETTIME                    = 0;
        public static int POCKETGAP                     = 0;
        public static int EXTENDIN1                     = 0;
        public static int TKADJCYCLE                    = 0;
        public static int POCKETSIZE                    = 0;
        public static int LATEMAX2                      = 0;
        public static string LOGPOSTINGTYPE             = string.Empty;
        //public static int BRKSTARTPRIORMIN            = 0;
        //public static int BRKENDAFTERMIN              = 0;
        //public static string TIMEPOLICY               = string.Empty;
    }

    #endregion
}


