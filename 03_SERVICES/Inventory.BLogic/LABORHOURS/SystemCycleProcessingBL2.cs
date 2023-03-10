using System;
using System.Collections.Generic;
using System.Text;
using Posting.DAL;
using CommonPostingLibrary;
using System.Data;
using System.Data.SqlClient;

namespace Posting.BLogic
{
    public class SystemCycleProcessingBL2
    {
        #region Variables
        DALHelper dalHelper;
        String PayPeriod;
        String UserCode;
        #endregion

        #region Constructors
        public SystemCycleProcessingBL2()
        {
            this.dalHelper = new DALHelper();
            this.PayPeriod = string.Empty;
            this.UserCode = LoginInfo.getUser().UserCode;
        }

        public SystemCycleProcessingBL2(DALHelper dal, string payPeriod)
        {
            this.dalHelper = dal;
            this.PayPeriod = payPeriod;
            this.UserCode = LoginInfo.getUser().UserCode;
        }

        public SystemCycleProcessingBL2(DALHelper dal, string payPeriod, string userCode)
        {
            this.dalHelper = dal;
            this.PayPeriod = payPeriod;
            this.UserCode = userCode;
        }
        #endregion

        #region Select/Insert/Update/Count
        public string[] CountRecords(string sqlCountQuery)
        {
            string[] strValue;

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);

            try
            {
                DataTable dtValue = dalHelper.ExecuteDataSet(sqlCountQuery, CommandType.Text, paramInfo).Tables[0];
                strValue = new string[dtValue.Columns.Count];

                foreach (DataColumn Col in dtValue.Columns)
                {
                    strValue[Col.Ordinal] = dtValue.Rows[0][Col].ToString();
                }

                return strValue;
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }

        public void InsertUpdate(string sqlInsertUpdateQuery)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                throw new Exception("Error: " + error.Message);
            }
        }

        public void InsertUpdate(string sqlInsertUpdateQuery, string PayPeriod2)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod2);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                throw new Exception("Error: " + error.Message);
            }
        }

        public string[] CountRecords(string sqlCountQuery, string NextPayPeriodStartDate, string NextPayPeriodEndDate, string NextPayPeriod)
        {
            string[] strValue;

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@NextPayPeriodStartDate", NextPayPeriodStartDate);
            paramInfo[2] = new ParameterInfo("@NextPayPeriodEndDate", NextPayPeriodEndDate);
            paramInfo[3] = new ParameterInfo("@NextPayPeriod", NextPayPeriod);

            try
            {
                DataTable dtValue = dalHelper.ExecuteDataSet(sqlCountQuery, CommandType.Text, paramInfo).Tables[0];
                strValue = new string[dtValue.Columns.Count];

                foreach (DataColumn Col in dtValue.Columns)
                {
                    strValue[Col.Ordinal] = dtValue.Rows[0][Col].ToString();
                }
                return strValue;
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }

        }

        public void InsertUpdate(string sqlInsertUpdateQuery, string NextPayPeriodStartDate, string NextPayPeriodEndDate, string NextPayPeriod)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[2] = new ParameterInfo("@NextPayPeriod", NextPayPeriod);
            paramInfo[3] = new ParameterInfo("@NextPayPeriodStartDate", NextPayPeriodStartDate);
            paramInfo[4] = new ParameterInfo("@NextPayPeriodEndDate", NextPayPeriodEndDate);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                throw new Exception("Error: " + error.Message);
            }
        }

        public DataTable dtFetch(string sqlQuery)
        {
            return dsFetch(sqlQuery).Tables[0];
        }

        public DataSet dsFetch(string sqlQuery)
        {
            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);

                return dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }
        #endregion

        #region Active Employee List
        public DataTable GetActiveEmployeeList()
        {
            try
            {
                return dalHelper.ExecuteDataSet(@"SELECT	Emt_EmployeeID, 
                                                            Emt_Shiftcode, 
                                                            Emt_LocationCode, 
                                                            Scm_ScheduleType,
                                                            Scm_ShiftTimeIn [In1],
                                                            Scm_ShiftBreakStart [Out1],
                                                            Scm_ShiftBreakEnd [In2],
                                                            Scm_ShiftTimeOut [Out2],
                                                            Emt_HireDate,
                                                            Emt_WorkType,
                                                            Emt_WorkGroup,
                                                            Emt_PayrollType,
                                                            Emt_LastName + ', ' + Emt_FirstName + ' ' + LEFT(Emt_MiddleName,1) + '.' [Emt_EmployeeName]
                                                    FROM	T_EmployeeMaster A
                                                       LEFT JOIN	T_ShiftCodeMaster
                                                       ON		    Scm_ShiftCode = Emt_Shiftcode		
                                                    WHERE	Emt_JobStatus != 'IN'
                                                    ORDER BY Emt_LastName, Emt_FirstName").Tables[0];
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }

        public DataTable GetActiveEmployeeList(String EmployeeID)
        {
            try
            {
                return dalHelper.ExecuteDataSet(string.Format(@"SELECT	Emt_EmployeeID, 
                                                                        Emt_Shiftcode, 
                                                                        Emt_LocationCode, 
                                                                        Scm_ScheduleType,
                                                                        Scm_ShiftTimeIn [In1],
                                                                        Scm_ShiftBreakStart [Out1],
                                                                        Scm_ShiftBreakEnd [In2],
                                                                        Scm_ShiftTimeOut [Out2],
                                                                        Emt_HireDate,
                                                                        Emt_WorkType,
                                                                        Emt_WorkGroup,
                                                                        Emt_PayrollType
                                                                FROM	T_EmployeeMaster A
                                                                left JOIN	T_ShiftCodeMaster
                                                                ON		Scm_ShiftCode = Emt_Shiftcode		
                                                                Where	Emt_JobStatus != 'IN'
                                                                AND		Emt_EmployeeID = '{0}'", EmployeeID)).Tables[0];
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }

        public DataTable GetActiveEmployeeList(String EmployeeID, string DBName)
        {
            try
            {
                return dalHelper.ExecuteDataSet(string.Format(@"SELECT	Emt_EmployeeID, 
                                                                        Emt_Shiftcode, 
                                                                        Emt_LocationCode, 
                                                                        Scm_ScheduleType,
                                                                        Scm_ShiftTimeIn [In1],
                                                                        Scm_ShiftBreakStart [Out1],
                                                                        Scm_ShiftBreakEnd [In2],
                                                                        Scm_ShiftTimeOut [Out2],
                                                                        Emt_HireDate,
                                                                        Emt_WorkType,
                                                                        Emt_WorkGroup,
                                                                        Emt_PayrollType
                                                                FROM	{1}..T_EmployeeMaster A
                                                                left JOIN {1}..T_ShiftCodeMaster
                                                                ON		Scm_ShiftCode = Emt_Shiftcode		
                                                                Where	Left(Emt_JobStatus, 1) = 'A'
                                                                AND		Emt_EmployeeID = '{0}'", EmployeeID, DBName)).Tables[0];
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }

        public DataSet GetEmployeeListForLaborHours()
        {
            try
            {
                DataSet ds;
                ds = dalHelper.ExecuteDataSet(@"SELECT	'False' AS 'Select'
                                                        , Emt_EmployeeID AS IDNumber
                                                        , Emt_LastName AS LastName
                                                        , Emt_FirstName AS FirstName
                                                        , Emt_JobStatus
                                                        , JobStatus.Adt_AccountDesc AS JobStatus
                                                        , Emt_EmploymentStatus
                                                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
                                                        , Emt_PayrollType
                                                        , PayrollType.Adt_AccountDesc AS PayrollType
                                                        , Emt_PositionCode
                                                        , Position.Adt_AccountDesc AS Position
                                                        , Emt_PayrollStatus AS PayrollStatus
                                                        , Emt_CostCenterCode
                                                        , dbo.getCostCenterFullNameV2(dbo.T_EmployeeMaster.Emt_CostCenterCode) AS CostCenter
                                                        , Emt_WorkType As 'Worktype'
                                                        , Emt_WorkGroup As 'Workgroup'
                                                FROM	T_EmployeeMaster 
                                                LEFT JOIN T_ShiftCodeMaster
                                                    ON Scm_ShiftCode = Emt_Shiftcode	
                                                LEFT OUTER JOIN T_AccountDetail AS EmploymentStatus 
	                                                ON EmploymentStatus.Adt_AccountCode = T_EmployeeMaster.Emt_EmploymentStatus AND 
	                                                EmploymentStatus.Adt_AccountType = 'EMPSTAT' 
                                                LEFT OUTER JOIN T_AccountDetail AS Position 
	                                                ON Position.Adt_AccountCode = T_EmployeeMaster.Emt_PositionCode AND 
	                                                Position.Adt_AccountType = 'POSITION' 	
                                                LEFT OUTER JOIN T_AccountDetail AS PayrollType 
	                                                ON PayrollType.Adt_AccountCode = T_EmployeeMaster.Emt_PayrollType AND 
                                                    PayrollType.Adt_AccountType = 'PAYTYPE' 
                                                LEFT OUTER JOIN T_AccountDetail AS JobStatus 
	                                                ON JobStatus.Adt_AccountCode = T_EmployeeMaster.Emt_JobStatus AND 
                                                    JobStatus.Adt_AccountType = 'JOBSTATUS'
                                                WHERE Emt_JobStatus != 'IN'
                                                ORDER BY Emt_LastName, Emt_FirstName");
                return ds;
            }
            catch (Exception err)
            {
                throw new PayrollException("Error: " + err.Message);
            }
        }

        public DataSet GetEmployeeListForPayroll()
        {
            try
            {
                DataSet ds;
                ds = dalHelper.ExecuteDataSet(@"SELECT	'False' AS 'Select'
                                                        , Emt_EmployeeID AS IDNumber
                                                        , Emt_LastName AS LastName
                                                        , Emt_FirstName AS FirstName
                                                        , Emt_JobStatus
                                                        , JobStatus.Adt_AccountDesc AS JobStatus
                                                        , Emt_EmploymentStatus
                                                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
                                                        , Emt_PayrollType
                                                        , PayrollType.Adt_AccountDesc AS PayrollType
                                                        , Emt_PaymentMode
                                                        , PaymentMode.Adt_AccountDesc AS PaymentMode
                                                        , Emt_TaxCode 
                                                        , Emt_TaxCode AS TaxCode
                                                        , Emt_PositionCode
                                                        , Position.Adt_AccountDesc AS Position
                                                        , Emt_EmployeeType
                                                        , EmployeeType.Adt_AccountDesc AS EmployeeType
                                                        , Emt_CostCenterCode
                                                        , dbo.getCostCenterFullNameV2(dbo.T_EmployeeMaster.Emt_CostCenterCode) AS CostCenter
                                                FROM	T_EmployeeMaster 
                                                LEFT JOIN T_ShiftCodeMaster
                                                    ON Scm_ShiftCode = Emt_Shiftcode	
                                                LEFT OUTER JOIN T_AccountDetail AS EmploymentStatus 
	                                                ON EmploymentStatus.Adt_AccountCode = T_EmployeeMaster.Emt_EmploymentStatus AND 
	                                                EmploymentStatus.Adt_AccountType = 'EMPSTAT' 
                                                LEFT OUTER JOIN T_AccountDetail AS Position 
	                                                ON Position.Adt_AccountCode = T_EmployeeMaster.Emt_PositionCode AND 
	                                                Position.Adt_AccountType = 'POSITION' 	
                                                LEFT OUTER JOIN T_AccountDetail AS PayrollType 
	                                                ON PayrollType.Adt_AccountCode = T_EmployeeMaster.Emt_PayrollType AND 
                                                    PayrollType.Adt_AccountType = 'PAYTYPE' 
                                                LEFT OUTER JOIN T_AccountDetail AS JobStatus 
	                                                ON JobStatus.Adt_AccountCode = T_EmployeeMaster.Emt_JobStatus AND 
                                                    JobStatus.Adt_AccountType = 'JOBSTATUS'
                                                LEFT OUTER JOIN T_AccountDetail AS PaymentMode 
													ON PaymentMode.Adt_AccountCode = T_EmployeeMaster.Emt_PaymentMode AND 
													PaymentMode.Adt_AccountType = 'PAYMODE' 
												LEFT OUTER JOIN T_AccountDetail AS EmployeeType 
													ON EmployeeType.Adt_AccountCode = T_EmployeeMaster.Emt_EmployeeType AND 
													EmployeeType.Adt_AccountType = 'EMPTYPE' 
                                                WHERE Emt_PayrollStatus = 1
                                                    AND Emt_JobStatus != 'IN'
                                                ORDER BY Emt_LastName, Emt_FirstName");
                return ds;
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }
        #endregion

        #region Log Ledger Correction
        public void GenerateLogLedgerRecord(DataRow Employee, DateTime ProcessDate, string NextPayPeriod)
        {
            #region [sqlQuery]
            string sqlInsertQuery = @"DECLARE @WorkGroup AS VARCHAR(10)
                                      DECLARE @WorkType AS VARCHAR(10)

                                      SET @WorkGroup = (SELECT TOP 1 Emv_WorkGroup FROM T_EmployeeGroup
                                                        WHERE Emv_EffectivityDate <= @Ell_ProcessDate
                                                        AND Emv_EmployeeID = @Ell_EmployeeId
                                                        ORDER BY Emv_EffectivityDate DESC)


                                      SET @WorkType = (SELECT TOP 1 Emv_WorkType FROM T_EmployeeGroup
                                                        WHERE Emv_EffectivityDate <= @Ell_ProcessDate
                                                        AND Emv_EmployeeID = @Ell_EmployeeId
                                                        ORDER BY Emv_EffectivityDate DESC)

                                      DELETE FROM [T_EmployeeLogLedger]
                                            WHERE [Ell_EmployeeId] =  @Ell_EmployeeId
                                              AND [Ell_ProcessDate] = @Ell_ProcessDate

                                      INSERT INTO [T_EmployeeLogLedger]
                                            (
                                            Ell_EmployeeId
                                            , Ell_ProcessDate
                                            , Ell_PayPeriod
                                            , Ell_DayCode
                                            , Ell_ShiftCode
                                            , Ell_Holiday
                                            , Ell_RestDay
                                            , Ell_ActualTimeIn_1
                                            , Ell_ActualTimeOut_1
                                            , Ell_ActualTimeIn_2
                                            , Ell_ActualTimeOut_2
                                            , Ell_ConvertedTimeIn_1Min
                                            , Ell_ConvertedTimeOut_1Min
                                            , Ell_ConvertedTimeIn_2Min
                                            , Ell_ConvertedTimeOut_2Min
                                            , Ell_ComputedTimeIn_1Min
                                            , Ell_ComputedTimeOut_1Min
                                            , Ell_ComputedTimeIn_2Min
                                            , Ell_ComputedTimeOut_2Min
                                            , Ell_AdjustShiftMin
                                            , Ell_ShiftTimeIn_1Min
                                            , Ell_ShiftTimeOut_1Min
                                            , Ell_ShiftTimeIn_2Min
                                            , Ell_ShiftTimeOut_2Min
                                            , Ell_ShiftMin
                                            , Ell_ScheduleType
                                            , Ell_EncodedPayLeaveType
                                            , Ell_EncodedPayLeaveHr
                                            , Ell_PayLeaveMin
                                            , Ell_ExcessLeaveMin
                                            , Ell_EncodedNoPayLeaveType
                                            , Ell_EncodedNoPayLeaveHr
                                            , Ell_NoPayLeaveMin
                                            , Ell_EncodedOvertimeAdvHr
                                            , Ell_EncodedOvertimePostHr
                                            , Ell_EncodedOvertimeMin
                                            , Ell_ComputedOvertimeMin
                                            , Ell_OffsetOvertimeMin
                                            , Ell_ComputedLateMin
                                            , Ell_LatePost
                                            , Ell_InitialAbsentMin
                                            , Ell_ComputedAbsentMin
                                            , Ell_ComputedRegularMin
                                            , Ell_ComputedDayWorkMin
                                            , Ell_ComputedRegularNightPremMin
                                            , Ell_ComputedOvertimeNightPremMin
                                            , Ell_PreviousDayWorkMin
                                            , Ell_PreviousDayHolidayReference
                                            , Ell_GraveyardPost
                                            , Ell_GraveyardPostBy
                                            , Ell_GraveyardPostDate
                                            , Ell_AssumedPresent
                                            , Ell_AssumedPresentBy
                                            , Ell_AssumedPresentDate
                                            , Ell_ForceLeave
                                            , Ell_ForceLeaveBy
                                            , Ell_ForceLeaveDate
                                            , Ell_ForOffsetMin
                                            , Ell_ExcessOffset
                                            , Ell_EarnedSatOff
                                            , Ell_SundayHolidayCount
                                            , Ell_WorkingDay
                                            , Ell_MealDay
                                            , Ell_ExpectedHour
                                            , Ell_AbsentHour
                                            , Ell_RegularHour
                                            , Ell_OvertimeHour
                                            , Ell_RegularNightPremHour
                                            , Ell_OvertimeNightPremHour
                                            , Ell_LeaveHour
                                            , Usr_Login
                                            , Ludatetime
                                            , Ell_ForwardedNextDayHour
                                            , Ell_AllowanceAmt01
                                            , Ell_AllowanceAmt02
                                            , Ell_AllowanceAmt03
                                            , Ell_AllowanceAmt04
                                            , Ell_AllowanceAmt05
                                            , Ell_AllowanceAmt06
                                            , Ell_AllowanceAmt07
                                            , Ell_AllowanceAmt08
                                            , Ell_AllowanceAmt09
                                            , Ell_AllowanceAmt10
                                            , Ell_AllowanceAmt11
                                            , Ell_AllowanceAmt12
                                            , Ell_LocationCode
                                            , Ell_Flex
                                            , Ell_TagFlex
                                            , Ell_TagTimeMod
                                            , Ell_WorkType
                                            , Ell_WorkGroup
                                            , Ell_AssumedPostBack
                                            )
                                           VALUES (@Ell_EmployeeId
                                                 ,@Ell_ProcessDate
                                                 ,@Ell_PayPeriod
                                                 ,'REG'
                                                 ,@Ell_ShiftCode
                                                 ,'False'
                                                 ,'False'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,@TimeIn1--
                                                 ,@TimeOut1--
                                                 ,@TimeIn2--
                                                 ,@TimeOut2--
                                                 ,0
                                                 ,@SchedType--
                                                 ,''
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,''
                                                 ,0.00
                                                 ,0
                                                 ,0.00
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,'False'
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,null
                                                 ,'False'
                                                 ,null
                                                 ,null
                                                 ,'False'
                                                 ,null
                                                 ,null
                                                 ,0.00
                                                 ,null
                                                 ,null
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,@UserLogin
                                                 ,GETDATE()
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,''
                                                 ,''
                                                 ,'N'
                                                 ,'N'
                                                 ,ISNULL(@WorkType, @WorkTypeEmpMas)
                                                 ,ISNULL(@WorkGroup, @WorkGroupEmpMas)
                                                 , null)";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", Employee["Emt_EmployeeID"]);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", ProcessDate);
            paramInfo[2] = new ParameterInfo("@Ell_PayPeriod", NextPayPeriod);
            paramInfo[3] = new ParameterInfo("@Ell_ShiftCode", Employee["Emt_Shiftcode"]);
            paramInfo[4] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[5] = new ParameterInfo("@TimeIn1", Employee["In1"]);
            paramInfo[6] = new ParameterInfo("@TimeOut1", Employee["Out1"]);
            paramInfo[7] = new ParameterInfo("@TimeIn2", Employee["In2"]);
            paramInfo[8] = new ParameterInfo("@TimeOut2", Employee["Out2"]);
            paramInfo[9] = new ParameterInfo("@SchedType", Employee["Scm_ScheduleType"]);
            paramInfo[10] = new ParameterInfo("@WorkTypeEmpMas", Employee["Emt_WorkType"]);
            paramInfo[11] = new ParameterInfo("@WorkGroupEmpMas", Employee["Emt_WorkGroup"]);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                throw new Exception("Error: " + error.Message);
            }
        }

        public void GenerateLogLedgerRecord(string EmployeeList, string PayPeriod, string UserCode)
        {
            #region [new query]
            string sqlInsertQuery = string.Format(@"declare @currentdate datetime
                                        declare @enddate datetime
                                        declare @PAYPERIODDATE as table (currentdate datetime null)

                                        select @currentdate = Ppm_StartCycle, @enddate = Ppm_EndCycle from t_payperiodmaster
                                        where Ppm_PayPeriod = @Ell_PayPeriod
                                        WHILE(@currentdate <= @enddate)
                                        BEGIN
                                            INSERT INTO @PAYPERIODDATE
                                            SELECT @currentdate

                                            SELECT  @currentdate = DATEADD(d, 1, @currentdate)
                                        END

                                        INSERT INTO [T_EmployeeLogLedger]
                                        (
                                            Ell_EmployeeId
                                            , Ell_ProcessDate
                                            , Ell_PayPeriod
                                            , Ell_DayCode
                                            , Ell_ShiftCode
                                            , Ell_Holiday
                                            , Ell_RestDay
                                            , Ell_ActualTimeIn_1
                                            , Ell_ActualTimeOut_1
                                            , Ell_ActualTimeIn_2
                                            , Ell_ActualTimeOut_2
                                            , Ell_ConvertedTimeIn_1Min
                                            , Ell_ConvertedTimeOut_1Min
                                            , Ell_ConvertedTimeIn_2Min
                                            , Ell_ConvertedTimeOut_2Min
                                            , Ell_ComputedTimeIn_1Min
                                            , Ell_ComputedTimeOut_1Min
                                            , Ell_ComputedTimeIn_2Min
                                            , Ell_ComputedTimeOut_2Min
                                            , Ell_AdjustShiftMin
                                            , Ell_ShiftTimeIn_1Min
                                            , Ell_ShiftTimeOut_1Min
                                            , Ell_ShiftTimeIn_2Min
                                            , Ell_ShiftTimeOut_2Min
                                            , Ell_ShiftMin
                                            , Ell_ScheduleType
                                            , Ell_EncodedPayLeaveType
                                            , Ell_EncodedPayLeaveHr
                                            , Ell_PayLeaveMin
                                            , Ell_ExcessLeaveMin
                                            , Ell_EncodedNoPayLeaveType
                                            , Ell_EncodedNoPayLeaveHr
                                            , Ell_NoPayLeaveMin
                                            , Ell_EncodedOvertimeAdvHr
                                            , Ell_EncodedOvertimePostHr
                                            , Ell_EncodedOvertimeMin
                                            , Ell_ComputedOvertimeMin
                                            , Ell_OffsetOvertimeMin
                                            , Ell_ComputedLateMin
                                            , Ell_LatePost
                                            , Ell_InitialAbsentMin
                                            , Ell_ComputedAbsentMin
                                            , Ell_ComputedRegularMin
                                            , Ell_ComputedDayWorkMin
                                            , Ell_ComputedRegularNightPremMin
                                            , Ell_ComputedOvertimeNightPremMin
                                            , Ell_PreviousDayWorkMin
                                            , Ell_PreviousDayHolidayReference
                                            , Ell_GraveyardPost
                                            , Ell_GraveyardPostBy
                                            , Ell_GraveyardPostDate
                                            , Ell_AssumedPresent
                                            , Ell_AssumedPresentBy
                                            , Ell_AssumedPresentDate
                                            , Ell_ForceLeave
                                            , Ell_ForceLeaveBy
                                            , Ell_ForceLeaveDate
                                            , Ell_ForOffsetMin
                                            , Ell_ExcessOffset
                                            , Ell_EarnedSatOff
                                            , Ell_SundayHolidayCount
                                            , Ell_WorkingDay
                                            , Ell_MealDay
                                            , Ell_ExpectedHour
                                            , Ell_AbsentHour
                                            , Ell_RegularHour
                                            , Ell_OvertimeHour
                                            , Ell_RegularNightPremHour
                                            , Ell_OvertimeNightPremHour
                                            , Ell_LeaveHour
                                            , Ell_ForwardedNextDayHour
                                            , Ell_AllowanceAmt01
                                            , Ell_AllowanceAmt02
                                            , Ell_AllowanceAmt03
                                            , Ell_AllowanceAmt04
                                            , Ell_AllowanceAmt05
                                            , Ell_AllowanceAmt06
                                            , Ell_AllowanceAmt07
                                            , Ell_AllowanceAmt08
                                            , Ell_AllowanceAmt09
                                            , Ell_AllowanceAmt10
                                            , Ell_AllowanceAmt11
                                            , Ell_AllowanceAmt12
                                            , Ell_LocationCode
                                            , Ell_Flex
                                            , Ell_TagFlex
                                            , Ell_TagTimeMod
                                            , Ell_WorkType
                                            , Ell_WorkGroup
                                            , Ell_AssumedPostBack
                                            , Ell_InitialOffsetMin
                                            , Ell_AppliedOffsetMin
                                            , Ell_ComputedOffsetMin
                                            , Ell_ComputedLate2Min
                                            , Ell_ComputedUndertimeMin
                                            , Ell_ComputedUndertime2Min
                                            , Usr_Login
                                            , Ludatetime
                                        )
                                        SELECT Emt_EmployeeID
                                                 ,currentdate
                                                 ,@Ell_PayPeriod
                                                 ,'REG'
                                                 ,Emt_Shiftcode
                                                 ,'False'
                                                 ,'False'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,Scm_ShiftTimeIn
                                                 ,Scm_ShiftBreakStart
                                                 ,Scm_ShiftBreakEnd
                                                 ,Scm_ShiftTimeOut
                                                 ,0
                                                 ,Scm_ScheduleType
                                                 ,''
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,''
                                                 ,0.00
                                                 ,0
                                                 ,0.00
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,'False'
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,null
                                                 ,'False'
                                                 ,null
                                                 ,null
                                                 ,'False'
                                                 ,null
                                                 ,null
                                                 ,0.00
                                                 ,null
                                                 ,null
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,''
                                                 ,''
                                                 ,'N'
                                                 ,'N'
                                                 ,Emt_WorkType
                                                 ,Emt_WorkGroup
                                                 ,null
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,@UserLogin
                                                 ,GETDATE()
                                        FROM @PAYPERIODDATE, T_EmployeeMaster
                                        LEFT JOIN T_ShiftCodeMaster
                                        ON Scm_ShiftCode = Emt_Shiftcode
                                        LEFT JOIN T_EmployeeLogLedger 
                                        ON Emt_EmployeeID = Ell_EmployeeId
                                           AND Ell_PayPeriod = @Ell_PayPeriod
                                        WHERE Ell_EmployeeId is null  
                                          {0} 
                                          AND Left(Emt_JobStatus, 1) = 'A'", EmployeeList);
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ell_PayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                throw new Exception("Error: " + error.Message);
            }
        }

        public void GenerateLogLedgerRecord(string EmployeeList, string PayPeriod, string UserCode, string DBName)
        {
            #region [new query]
            string sqlInsertQuery = string.Format(@"declare @currentdate datetime
                                        declare @enddate datetime
                                        declare @PAYPERIODDATE as table (currentdate datetime null)

                                        select @currentdate = Ppm_StartCycle, @enddate = Ppm_EndCycle from {1}..t_payperiodmaster
                                        where Ppm_PayPeriod = @Ell_PayPeriod
                                        WHILE(@currentdate <= @enddate)
                                        BEGIN
                                            INSERT INTO @PAYPERIODDATE
                                            SELECT @currentdate

                                            SELECT  @currentdate = DATEADD(d, 1, @currentdate)
                                        END

                                        INSERT INTO {1}..[T_EmployeeLogLedger]
                                        (
                                            Ell_EmployeeId
                                            , Ell_ProcessDate
                                            , Ell_PayPeriod
                                            , Ell_DayCode
                                            , Ell_ShiftCode
                                            , Ell_Holiday
                                            , Ell_RestDay
                                            , Ell_ActualTimeIn_1
                                            , Ell_ActualTimeOut_1
                                            , Ell_ActualTimeIn_2
                                            , Ell_ActualTimeOut_2
                                            , Ell_ConvertedTimeIn_1Min
                                            , Ell_ConvertedTimeOut_1Min
                                            , Ell_ConvertedTimeIn_2Min
                                            , Ell_ConvertedTimeOut_2Min
                                            , Ell_ComputedTimeIn_1Min
                                            , Ell_ComputedTimeOut_1Min
                                            , Ell_ComputedTimeIn_2Min
                                            , Ell_ComputedTimeOut_2Min
                                            , Ell_AdjustShiftMin
                                            , Ell_ShiftTimeIn_1Min
                                            , Ell_ShiftTimeOut_1Min
                                            , Ell_ShiftTimeIn_2Min
                                            , Ell_ShiftTimeOut_2Min
                                            , Ell_ShiftMin
                                            , Ell_ScheduleType
                                            , Ell_EncodedPayLeaveType
                                            , Ell_EncodedPayLeaveHr
                                            , Ell_PayLeaveMin
                                            , Ell_ExcessLeaveMin
                                            , Ell_EncodedNoPayLeaveType
                                            , Ell_EncodedNoPayLeaveHr
                                            , Ell_NoPayLeaveMin
                                            , Ell_EncodedOvertimeAdvHr
                                            , Ell_EncodedOvertimePostHr
                                            , Ell_EncodedOvertimeMin
                                            , Ell_ComputedOvertimeMin
                                            , Ell_OffsetOvertimeMin
                                            , Ell_ComputedLateMin
                                            , Ell_LatePost
                                            , Ell_InitialAbsentMin
                                            , Ell_ComputedAbsentMin
                                            , Ell_ComputedRegularMin
                                            , Ell_ComputedDayWorkMin
                                            , Ell_ComputedRegularNightPremMin
                                            , Ell_ComputedOvertimeNightPremMin
                                            , Ell_PreviousDayWorkMin
                                            , Ell_PreviousDayHolidayReference
                                            , Ell_GraveyardPost
                                            , Ell_GraveyardPostBy
                                            , Ell_GraveyardPostDate
                                            , Ell_AssumedPresent
                                            , Ell_AssumedPresentBy
                                            , Ell_AssumedPresentDate
                                            , Ell_ForceLeave
                                            , Ell_ForceLeaveBy
                                            , Ell_ForceLeaveDate
                                            , Ell_ForOffsetMin
                                            , Ell_ExcessOffset
                                            , Ell_EarnedSatOff
                                            , Ell_SundayHolidayCount
                                            , Ell_WorkingDay
                                            , Ell_MealDay
                                            , Ell_ExpectedHour
                                            , Ell_AbsentHour
                                            , Ell_RegularHour
                                            , Ell_OvertimeHour
                                            , Ell_RegularNightPremHour
                                            , Ell_OvertimeNightPremHour
                                            , Ell_LeaveHour
                                            , Ell_ForwardedNextDayHour
                                            , Ell_AllowanceAmt01
                                            , Ell_AllowanceAmt02
                                            , Ell_AllowanceAmt03
                                            , Ell_AllowanceAmt04
                                            , Ell_AllowanceAmt05
                                            , Ell_AllowanceAmt06
                                            , Ell_AllowanceAmt07
                                            , Ell_AllowanceAmt08
                                            , Ell_AllowanceAmt09
                                            , Ell_AllowanceAmt10
                                            , Ell_AllowanceAmt11
                                            , Ell_AllowanceAmt12
                                            , Ell_LocationCode
                                            , Ell_Flex
                                            , Ell_TagFlex
                                            , Ell_TagTimeMod
                                            , Ell_WorkType
                                            , Ell_WorkGroup
                                            , Ell_AssumedPostBack
                                            , Ell_InitialOffsetMin
                                            , Ell_AppliedOffsetMin
                                            , Ell_ComputedOffsetMin
                                            , Ell_ComputedLate2Min
                                            , Ell_ComputedUndertimeMin
                                            , Ell_ComputedUndertime2Min
                                            , Usr_Login
                                            , Ludatetime
                                        )
                                        SELECT Emt_EmployeeID
                                                 ,currentdate
                                                 ,@Ell_PayPeriod
                                                 ,'REG'
                                                 ,Emt_Shiftcode
                                                 ,'False'
                                                 ,'False'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,Scm_ShiftTimeIn
                                                 ,Scm_ShiftBreakStart
                                                 ,Scm_ShiftBreakEnd
                                                 ,Scm_ShiftTimeOut
                                                 ,0
                                                 ,Scm_ScheduleType
                                                 ,''
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,''
                                                 ,0.00
                                                 ,0
                                                 ,0.00
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,'False'
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,null
                                                 ,'False'
                                                 ,null
                                                 ,null
                                                 ,'False'
                                                 ,null
                                                 ,null
                                                 ,0.00
                                                 ,null
                                                 ,null
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0.00
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,''
                                                 ,''
                                                 ,'N'
                                                 ,'N'
                                                 ,Emt_WorkType
                                                 ,Emt_WorkGroup
                                                 ,null
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,@UserLogin
                                                 ,GETDATE()
                                        FROM @PAYPERIODDATE, {1}..T_EmployeeMaster
                                        LEFT JOIN {1}..T_ShiftCodeMaster
                                        ON Scm_ShiftCode = Emt_Shiftcode
                                        LEFT JOIN {1}..T_EmployeeLogLedger 
                                        ON Emt_EmployeeID = Ell_EmployeeId
                                           AND Ell_PayPeriod = @Ell_PayPeriod
                                        WHERE Ell_EmployeeId is null  
                                          {0} 
                                          AND Left(Emt_JobStatus, 1) = 'A'", EmployeeList, DBName);
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ell_PayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                throw new Exception("Error: " + error.Message);
            }
        }

        //This is the main function to correct the log ledger records (without Profile name)
        public void CorrectLogLedgerRecord(bool ProcessAll, bool ProcessPerPayPeriod, bool UseDataTable
                                            , DataTable EmployeeDataList, string EmployeeFilter, string PayPeriod, string DateStart, string DateEnd
                                            , bool CreateLogLedger, bool ResetWorkType, bool CorrectRestday, bool CorrectHoliday, bool CorrectShift, bool CorrectLogs, bool CorrectWorkflowMovement
                                            , string UserLogin)
        {
            //****************************
            //Flag Description:
            // ProcessAll
            //  True: No Employee Condition
            //  False: Use either EmployeeDataList(DataTable) or EmployeeFilter(Comma-separated String); Depends on UseDataTable flag
            //
            // UseDataTable
            //  True: Use EmployeeDataList(DataTable)
            //  False: Use EmployeeFilter(Comma-separated String)
            //
            // ProcessPerPayPeriod
            //  True: Use PayPeriod (query the start and end cycle)
            //  False: Use DateStart and DateEnd
            //
            // CreateLogLedger: Creates log ledger records (if ProcessPerPayPeriod is TRUE)
            // ResetWorkType: Updates the worktype/workgroup in log ledger and employee master (from T_EmployeeGroup)
            // CorrectRestday: Updates the day code and restday 
            // CorrectHoliday: Updates the holiday
            // CorrectShift: Updates the shift code
            // CorrectLogs: Updates the logs from assumed present and time modification tables
            // CorrectWorkflowMovement: Updates the worktype/workgroup, shift, restday and logs based on the ResetWorkType, CorrectShift, CorrectRestday, and CorrectLogs flags, respectively
            //****************************
            try
            {
                #region <Initialize string conditions>
                string query = "";
                string queryExtension = "";
                if (ProcessAll == false)
                {
                    if (UseDataTable == true)
                    {
                        if (EmployeeDataList.Rows.Count > 0)
                        {
                            queryExtension = @" AND Ell_EmployeeId IN (";
                            foreach (DataRow drEmployee in EmployeeDataList.Rows)
                            {
                                queryExtension += string.Format(@"'{0}',", drEmployee["Emt_EmployeeId"].ToString());
                            }
                            queryExtension = queryExtension.Substring(0, queryExtension.Length - 1);
                            queryExtension += @") ";
                        }
                    }
                    else
                    {
                        queryExtension = EmployeeFilter;
                    }
                }

                if (ProcessPerPayPeriod == true)
                {
                    DataTable dtPayperiod = GetCycleRange(PayPeriod);
                    if (dtPayperiod.Rows.Count > 0)
                    {
                        DateStart = dtPayperiod.Rows[0]["Ppm_StartCycle"].ToString();
                        DateEnd = dtPayperiod.Rows[0]["Ppm_EndCycle"].ToString();
                    }
                }
                #endregion

                #region <Create Log Ledger Records if not exist> //Works only if ProcessPerPayPeriod is true
                if (CreateLogLedger == true && ProcessPerPayPeriod == true)
                {
                    queryExtension = queryExtension.Replace("Ell_EmployeeId", "Emt_EmployeeId");
                    GenerateLogLedgerRecord(queryExtension, PayPeriod, UserLogin);

                    string NextPeriod = GetNextCycle(1);
                    GenerateLogLedgerRecord(queryExtension, NextPeriod, UserLogin);
                    queryExtension = queryExtension.Replace("Emt_EmployeeId", "Ell_EmployeeId");
                }
                #endregion

                #region <Initialize Worktype and Workgroup>
                if (ResetWorkType == true)
                {
                    queryExtension = queryExtension.Replace("Ell_EmployeeId", "Emt_EmployeeId");
                    query = string.Format(@"--Added By Rendell Uy - 4/11/2013 : Initialize Employee Master Worktype and Workgroup Based on T_EmployeeGroup
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                              
                            UPDATE T_EmployeeMaster
                            SET Emt_WorkType = Emv_WorkType
                              , Emt_WorkGroup = Emv_WorkGroup
                              , T_EmployeeMaster.Usr_Login = '{2}'
                              , T_EmployeeMaster.Ludatetime = getdate()
                            FROM T_EmployeeMaster
                            INNER JOIN T_EmployeeGroup on Emt_EmployeeID = Emv_EmployeeID
                            WHERE @EndNewCycle >= Emv_EffectivityDate
                                   AND ISNULL(Emv_EndDate, @StartNewCycle) >= @StartNewCycle
                                   AND (Emt_WorkType != Emv_WorkType OR Emt_WorkGroup != Emv_WorkGroup)
                                   and Emv_EmployeeID + Convert(Char(10),Emv_EffectivityDate,112) in ( SELECT Emv_EmployeeID + Convert(Char(10), Emv_EffectivityDate, 112)
																                                       FROM (SELECT Emv_EmployeeID , Max(Emv_EffectivityDate) as Emv_EffectivityDate
																		                                    FROM T_EmployeeGroup
																		                                    WHERE @EndNewCycle >= Emv_EffectivityDate
																			                                    AND ISNULL(Emv_EndDate, @StartNewCycle) >= @StartNewCycle
																		                                    GROUP BY Emv_EmployeeID ) Temp) ", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Emt_EmployeeId", "Ell_EmployeeId");

                    query = string.Format(@"--Added By Rendell Uy - 4/27/2013 : Initialize Worktype and Workgroup
                                        Update T_EmployeeLogledger
                                        Set Ell_WorkType = Emv_WorkType
                                            , Ell_WorkGroup = Emv_WorkGroup
                                            , T_EmployeeLogledger.Usr_Login = '{2}'
                                            , T_EmployeeLogledger.Ludatetime = getdate()
                                        FROM T_EmployeeLogledger
                                        INNER JOIN T_EmployeeGroup
                                        ON Ell_EmployeeId = Emv_EmployeeID
	                                        AND Emv_EffectivityDate = (SELECT TOP 1 Emv_EffectivityDate
								                                        FROM T_EmployeeGroup
								                                        WHERE Emv_EmployeeID = ELL_EMPLOYEEID
								                                         AND Emv_EffectivityDate <= ELL_PROCESSDATE
								                                        ORDER BY Emv_EffectivityDate DESC)
                                        WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        queryExtension = queryExtension.Replace("Ell_EmployeeId", "Mve_EmployeeId");
                        query = string.Format(@"SELECT * 
                                                FROM T_Movement M
                                                WHERE Mve_Type = 'G'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                FROM T_Movement
																				WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'G'
                                                                                    AND Mve_Status IN ('A', '9') ) --Get latest effectivity
                                                            OR Mve_EffectivityDate >= '{0}') --Get effectivity from start of pay period
                                                    AND Mve_ApprovedDate = ( SELECT MAX(Mve_ApprovedDate)
                                                                                FROM T_Movement
								                                                WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'G'
                                                                                    AND Mve_Status IN ('A', '9')
                                                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )
                                                    {1}
                                                ORDER BY Mve_EmployeeID, Mve_EffectivityDate", DateStart, queryExtension);
                        DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];

                        foreach (DataRow drRow in dtResult.Rows)
                        {
                            CascadeUpdateWorkgroup(drRow["Mve_EmployeeId"].ToString()
                                                    , Convert.ToDateTime(drRow["Mve_EffectivityDate"].ToString())
                                                    , drRow["Mve_To"].ToString().Substring(0, 3).Trim()
                                                    , drRow["Mve_To"].ToString().Substring(3, 3).Trim()
                                                    , dalHelper);
                        }
                        queryExtension = queryExtension.Replace("Mve_EmployeeId", "Ell_EmployeeId");
                    }
                }
                #endregion

                #region <Update Rest Day for REG worktype>
                if (CorrectRestday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 06/24/2013 : Initialize all day codes to REG
                                  UPDATE [T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = 'REG'
                                        , [Ell_RestDay] = 0
                                        , [Ell_Holiday] = 0
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM [T_EmployeeLogLedger]
                                    WHERE [Ell_WorkType] = 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    query = string.Format(@"
                                    UPDATE T_EmployeeLogLedger
                                    SET Ell_RestDay = CASE WHEN REST = '0'
	                                                    THEN 0
	                                                    ELSE 1
                                                      END
                                        ,[Ell_DayCode] = CASE WHEN REST = '0'
	                                                        THEN 'REG'
	                                                        ELSE 'REST'
                                                          END
                                        ,[Ell_Holiday] = 0
                                        , Usr_Login = '{2}'
                                        , Ludatetime = getdate()
                                      FROM T_EmployeeLogLedger
                                    INNER JOIN 
                                    (
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 2 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 1, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 3 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 2, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 4 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 3, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 5 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 4, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 6 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 5, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 7 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 6, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 1 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 7, 1) AS REST
                                        FROM	T_EmployeeRestDay
                                    ) EmpRestDay
                                    ON Ell_EmployeeId = Erd_EmployeeID
                                        AND DATEPART(DW, Ell_ProcessDate) = DATPART
                                        AND ERD_EFFECTIVITYDATE = (SELECT TOP 1 ERD_EFFECTIVITYDATE
                                                                    FROM T_EMPLOYEERESTDAY
                                                                    WHERE ERD_EMPLOYEEID = ELL_EMPLOYEEID
                                                                     AND ERD_EFFECTIVITYDATE <= ELL_PROCESSDATE
                                                                    ORDER BY ERD_EFFECTIVITYDATE DESC)
                                    WHERE [Ell_WorkType] = 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        query = string.Format(@"UPDATE T_EmployeeLogLedger
                                                SET Ell_DayCode = 'REST'
                                                    , Ell_RestDay = 1
                                                    , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM T_Movement M
                                                INNER JOIN T_EmployeeLogLedger
                                                    ON Mve_EmployeeId = Ell_EmployeeId
                                                    AND Mve_To = Ell_ProcessDate
                                                    {1}
                                                WHERE Mve_Type = 'R'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                 FROM T_Movement
							                                                     WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'R'
                                                                                    AND Mve_Status IN ('A', '9'))
                                                             OR Mve_EffectivityDate >= '{0}')
	                                                    AND Mve_ApprovedDate = (SELECT MAX(Mve_ApprovedDate)
							                                                    FROM T_Movement 
							                                                    WHERE Mve_EmployeeId = M.Mve_EmployeeId
								                                                    AND Mve_Type = 'R'
								                                                    AND Mve_Status IN ('A', '9')
								                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )

                                                UPDATE T_EmployeeLogLedger
                                                SET Ell_DayCode = 'REG'
                                                    , Ell_RestDay = 0
                                                    , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM T_Movement M
                                                INNER JOIN T_EmployeeLogLedger
                                                    ON Mve_EmployeeId = Ell_EmployeeId
                                                    AND Mve_From = Ell_ProcessDate
                                                    {1}
                                                WHERE Mve_Type = 'R'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                 FROM T_Movement
							                                                     WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'R'
                                                                                    AND Mve_Status IN ('A', '9'))
                                                             OR Mve_EffectivityDate >= '{0}')
	                                                    AND Mve_ApprovedDate = (SELECT MAX(Mve_ApprovedDate)
							                                                    FROM T_Movement 
							                                                    WHERE Mve_EmployeeId = M.Mve_EmployeeId
								                                                    AND Mve_Type = 'R'
								                                                    AND Mve_Status IN ('A', '9')
								                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )", DateStart, queryExtension, UserLogin);
                        dalHelper.ExecuteNonQuery(query);
                    }
                }
                #endregion

                #region <Initial Day Code based on Calendar Group>
                if (CorrectRestday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 06/24/2013 : Initialize all day codes to REG
                                  UPDATE [T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = 'REG'
                                        , [Ell_RestDay] = 0
                                        , [Ell_Holiday] = 0
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM [T_EmployeeLogLedger]
                                    WHERE [Ell_WorkType] != 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Initial Day Code based on Calendar Group
                                  UPDATE [T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = CASE WHEN RTRIM(Cal_WorkCode) = 'R' THEN 'REST'
                                                               WHEN LEN(RTRIM(Cal_WorkCode)) > 1 THEN 'REG' + SUBSTRING(RTRIM(Cal_WorkCode), 2, 1)
                                                               ELSE 'REG' END
                                        , [Ell_RestDay] = CASE WHEN RTRIM(Cal_WorkCode) = 'R' THEN 1 ELSE 0 END
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM [T_EmployeeLogLedger] 
                                    INNER JOIN [T_CalendarGroupTmp]
                                        ON Cal_WorkType = Ell_WorkType
                                        AND Cal_WorkGroup = Ell_WorkGroup
                                        AND Cal_ProcessDate = Ell_ProcessDate
                                    WHERE [Ell_WorkType] != 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Update Shift Code for REG worktype>
                if (CorrectShift == true)
                {
                    query = string.Format(@"--New query for update shift in logledger (REG worktype)
                                              UPDATE [T_EmployeeLogLedger]
                                                SET [Ell_ShiftCode] = Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                                                           then Emt_Shiftcode
                                                                           else
                                                                              case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                                                   then Scm_EquivalentShiftCode
                                                                                   else Emt_Shiftcode
                                                                              end
                                                                      end
                                                    , T_EmployeeLogledger.Usr_Login = '{2}'
                                                    , T_EmployeeLogledger.Ludatetime = getdate()
                                              From T_EmployeeLogledger
                                              Inner Join T_EmployeeMaster on Emt_EmployeeID = Ell_EmployeeID
                                              Inner Join T_Shiftcodemaster on Scm_Shiftcode = Emt_Shiftcode
                                              WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'
                                              AND Ell_WorkType = 'REG'
                                              @EXTENSION

                                            IF (SELECT Pmx_ParameterValue FROM T_ParameterMasterExt WHERE Pmx_ParameterID = 'SHFTCYCOPN' AND Pmx_Classification = 'LSTREGSHFT' AND Pmx_Status = 'A') = 1
                                            BEGIN
                                                DECLARE @ShiftTable as TABLE
                                                (Ell_EmployeeID varchar(MAX), Ell_ProcessDate datetime)

                                                DECLARE @DateStart datetime
                                                SET @DateStart = (SELECT Ppm_StartCycle FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = '{3}')

                                                INSERT INTO @ShiftTable
                                                SELECT Ell_EmployeeID, MAX(Ell_ProcessDate)
                                                FROM
                                                (SELECT Ell_EmployeeId, MAX(Ell_ProcessDate) Ell_ProcessDate FROM T_EmployeeLogLedger
                                                INNER JOIN T_EmployeeMaster ON Emt_EmployeeID = Ell_EmployeeId
                                                WHERE Ell_RestDay = 0 and Ell_Holiday = 0
                                                AND Ell_ProcessDate < @DateStart
                                                GROUP BY Ell_EmployeeId

                                                UNION 

                                                SELECT Ell_EmployeeId, MAX(Ell_ProcessDate) Ell_ProcessDate FROM T_EmployeeLogLedgerHist
                                                INNER JOIN T_EmployeeMaster ON Emt_EmployeeID = Ell_EmployeeId
                                                WHERE Ell_RestDay = 0 and Ell_Holiday = 0
                                                AND Ell_ProcessDate < @DateStart
                                                GROUP BY Ell_EmployeeId)TEMP
                                                GROUP BY Ell_EmployeeId
                                                ORDER BY Ell_EmployeeId

                                                UPDATE T_EmployeeLogLedger 
                                                SET [Ell_ShiftCode] = Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                                then ELLPREV.Ell_ShiftCode
                                                else
                                                    case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                        then Scm_EquivalentShiftCode
                                                        else ELLPREV.Ell_ShiftCode
                                                    end
                                                end
                                                , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                , T_EmployeeLogLedger.Ludatetime = getdate()
                                                 FROM T_EmployeeLogLedger ELL
                                                LEFT JOIN 
                                                (
	                                                SELECT SHFT.Ell_EmployeeID, SHFT.Ell_ProcessDate, Ell_ShiftCode
	                                                FROM T_EmployeeLogLedger LDR
	                                                INNER JOIN @ShiftTable SHFT
	                                                ON SHFT.Ell_EmployeeID = LDR.Ell_EmployeeId
	                                                AND SHFT.Ell_ProcessDate = LDR.Ell_ProcessDate

	                                                UNION 

	                                                SELECT SHFT.Ell_EmployeeID, SHFT.Ell_ProcessDate, Ell_ShiftCode
	                                                FROM T_EmployeeLogLedgerHist LDR
	                                                INNER JOIN @ShiftTable SHFT
	                                                ON SHFT.Ell_EmployeeID = LDR.Ell_EmployeeId
	                                                AND SHFT.Ell_ProcessDate = LDR.Ell_ProcessDate
                                                )ELLPREV
                                                ON ELLPREV.Ell_EmployeeId = ELL.Ell_EmployeeId
                                                INNER JOIN T_ShiftCodeMaster ON Scm_ShiftCode = ELLPREV.Ell_ShiftCode
                                                INNER JOIN T_EmployeeGroup ON Emv_EmployeeID = ELL.Ell_EmployeeId
                                                AND Emv_EffectivityDate = (SELECT TOP 1 Emv_EffectivityDate
							                                                FROM T_EmployeeGroup
							                                                WHERE Emv_EmployeeID = ELL.ELL_EMPLOYEEID
								                                                AND Emv_EffectivityDate <= ELL.ELL_PROCESSDATE
							                                                ORDER BY Emv_EffectivityDate DESC)
                                                AND Emv_WorkType = 'REG'
                                                WHERE ELL.Ell_PayPeriod = '{3}'
                                                @INNEREXTENSION
                                            END", DateStart, DateEnd, UserLogin, PayPeriod);
                    query = query.Replace("@EXTENSION", queryExtension);
                    query = query.Replace("@INNEREXTENSION", queryExtension.Replace("Ell_", "ELL.Ell_"));
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        query = string.Format(@"UPDATE T_EmployeeLogLedger
                                                SET Ell_ShiftCode = Mve_To
                                                    , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM T_Movement M
                                                INNER JOIN T_EmployeeLogLedger
                                                    ON Mve_EmployeeId = Ell_EmployeeId
                                                    AND Mve_EffectivityDate = Ell_ProcessDate
                                                    {1}
                                                WHERE Mve_Type = 'S'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND Mve_To != Ell_ShiftCode
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                    FROM T_Movement
								                                                    WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                        AND Mve_Type = 'S'
                                                                                        AND Mve_Status IN ('A', '9'))
                                                             OR Mve_EffectivityDate >= '{0}')
	                                                    AND Mve_ApprovedDate = (SELECT MAX(Mve_ApprovedDate)
							                                                    FROM T_Movement 
							                                                    WHERE Mve_EmployeeId = M.Mve_EmployeeId
								                                                    AND Mve_Type = 'S'
								                                                    AND Mve_Status IN ('A', '9')
								                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )", DateStart, queryExtension, UserLogin);
                        dalHelper.ExecuteNonQuery(query);
                    }
                }
                #endregion

                #region <Initial Shift Code based on Calendar Group>
                if (CorrectShift == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Shift Code based on Calendar Group
                                              UPDATE [T_EmployeeLogLedger]
                                                 SET [Ell_ShiftCode] = Cal_ShiftCode
                                                    , T_EmployeeLogledger.Usr_Login = '{2}'
                                                    , T_EmployeeLogledger.Ludatetime = getdate()
                                                FROM [T_EmployeeLogLedger] 
                                                INNER JOIN [T_CalendarGroupTmp]
                                                    ON Cal_WorkType = Ell_WorkType
                                                    AND Cal_WorkGroup = Ell_WorkGroup
                                                    AND Cal_ProcessDate = Ell_ProcessDate
                                                WHERE [Ell_WorkType] != 'REG'
                                                    AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Special Day>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy -2/22/2011 : Special Day Update
                          UPDATE T_EmployeeLogLedger
                            SET Ell_DayCode = Ard_DayCode,
                                Ell_RestDay = CASE WHEN (Ard_DayCode = 'REST') THEN 1 ELSE Ell_RestDay END,
                                Ell_Holiday = Dcm_Holiday,
                                Ell_Shiftcode = CASE WHEN Ell_WorkType = 'REG' -- Non-Calendar
                                                THEN CASE WHEN (Ard_DayCode = 'REG') 
                                                    THEN Emt_Shiftcode 
                                                    ELSE
                                                        (SELECT case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                                   then Scm_EquivalentShiftCode
                                                                   else Scm_ShiftCode
                                                                end
                                                        FROM T_ShiftCodeMaster
                                                        WHERE Scm_ShiftCode = Emt_Shiftcode) 
                                                    END
                                                ELSE
                                                    Ell_Shiftcode
                                                END,
                                T_EmployeeLogLedger.Usr_Login = '{2}',
                                T_EmployeeLogLedger.Ludatetime = getdate()
                            FROM T_EmployeeLogLedger
                            INNER JOIN T_EmployeeMaster
                            ON Ell_EmployeeId = Emt_EmployeeId
                            INNER JOIN T_SpecialDayMaster
                            ON Ell_ProcessDate = Ard_Date
                                AND Ell_WorkType = Ard_WorkType
                                AND Ell_WorkGroup = Ard_WorkGroup
                                AND Emt_PayrollType = Ard_PayrollType
                            INNER JOIN T_DayCodeMaster
                            ON Dcm_DayCode = Ard_DayCode
                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Location Code>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Update Location Code
                          UPDATE T_EmployeeLogLedger
                            SET Ell_LocationCode = (Select TOP(1) Ewl_LocationCode 
                                                    From T_EmployeeWorkLocation
                                                        Where Ewl_EmployeeID = Ell_EmployeeId 
	                                                     And Ewl_EffectivityDate <= Ell_ProcessDate
                                                         And Ell_ProcessDate <= ISNULL(Ewl_EndDate, Ell_ProcessDate)
	                                                     Order By Ewl_EffectivityDate DESC)
                                , Usr_Login = '{2}'
                                , Ludatetime = getdate()
                            FROM T_EmployeeLogLedger
                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Holiday>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Update Holiday Code
                          UPDATE T_EmployeeLogLedger
                            SET Ell_DayCode = Hmt_HolidayCode
                                , Ell_Holiday = Dcm_Holiday
                                , Ell_ShiftCode = Case when Ell_RestDay = 0 and Dcm_Holiday = 0
                                                       then Ell_ShiftCode
                                                       else
                                                          case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                               then Scm_EquivalentShiftCode
                                                               else Ell_ShiftCode
                                                          end
                                                  end
                                , T_EmployeeLogLedger.Usr_Login = '{2}'
                                , T_EmployeeLogLedger.Ludatetime = getdate()
                            FROM T_EmployeeLogLedger
                            INNER JOIN T_EmployeeMaster
                            ON Ell_EmployeeId = Emt_EmployeeID
                            INNER JOIN T_HolidayMaster
	                            ON Ell_Processdate = Hmt_HolidayDate
	                            AND (Hmt_ApplicCity = Ell_LocationCode
                                    OR Hmt_ApplicCity = 'ALL')
	                            AND (Hmt_PayTypeExclusion IS NULL OR Hmt_PayTypeExclusion = '' OR Emt_PayrollType NOT IN (SELECT Data FROM dbo.Split(Hmt_PayTypeExclusion,',')))
	                            AND (Hmt_JobStatusExclusion IS NULL OR Hmt_JobStatusExclusion = '' OR Emt_JobStatus NOT IN (SELECT Data FROM dbo.Split(Hmt_JobStatusExclusion,',')))
	                            AND (Hmt_EmpStatusExclusion IS NULL OR Hmt_EmpStatusExclusion = '' OR Emt_EmploymentStatus NOT IN (SELECT Data FROM dbo.Split(Hmt_EmpStatusExclusion,',')))
	                            AND (Hmt_WorktypegroupExclusion IS NULL OR Hmt_WorktypegroupExclusion = '' OR RTRIM(Ell_WorkType)+'/'+RTRIM(Ell_WorkGroup) NOT IN (SELECT Data FROM dbo.Split(Hmt_WorktypegroupExclusion,',')))
                            INNER JOIN T_DayCodeMaster
                            ON Hmt_HolidayCode = Dcm_DayCode
                            INNER JOIN T_Shiftcodemaster 
                            ON Scm_Shiftcode = Ell_ShiftCode
                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Day Code Filler Restday>
                if (CorrectRestday == true || CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 4/11/2013 : Added Restday Indicator in Day Code Master
                                    UPDATE T_EmployeeLogLedger
                                    SET Ell_RestDay = Dcm_Restday
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM T_EmployeeLogLedger
                                    INNER JOIN T_DayCodeFiller
                                        ON Ell_DayCode = Dcf_DayCode
                                    INNER JOIN T_DayCodeMaster
										ON Dcf_DayCode = Dcm_DayCode
                                    WHERE Dcf_Status = 'A'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Employee Assumed Present>
                if (CorrectLogs == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 6/18/2013 : Assumed Present Updating
                                              UPDATE T_EmployeeLogLedger
                                                SET Ell_AssumedPresent = 1
                                                    ,Ell_AssumedPresentBy = '{2}'
                                                    ,Ell_AssumedPresentDate = getdate()
                                                FROM T_EmployeeLogLedger
                                                INNER JOIN T_EmployeeAssumedPresent 
	                                                on Eap_EmployeeID = ell_employeeid
	                                                and Ell_ProcessDate between Eap_StartDate and ISNULL(Eap_EndDate, Ell_ProcessDate)
                                                WHERE Ell_DayCode = 'REG'
                                                    and [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        query = string.Format(@"UPDATE T_EmployeeLogLedger
                                                SET Ell_ActualTimeIn_1 = CASE WHEN Trm_ActualTimeIn1 != ''
							                                                THEN Trm_ActualTimeIn1
							                                                ELSE '0000'
							                                                END
                                                , Ell_ActualTimeOut_1 = CASE WHEN Trm_ActualTimeOut1 != ''
							                                                THEN Trm_ActualTimeOut1
							                                                ELSE '0000'
							                                                END
                                                , Ell_ActualTimeIn_2 = CASE WHEN Trm_ActualTimeIn2 != ''
							                                                THEN Trm_ActualTimeIn2
							                                                ELSE '0000'
							                                                END
                                                , Ell_ActualTimeOut_2 = CASE WHEN Trm_ActualTimeOut2 != ''
							                                                THEN Trm_ActualTimeOut2
							                                                ELSE '0000'
							                                                END
                                                , T_EmployeeLogLedger.Usr_Login = '{3}'
                                                , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM T_TimeRecMod T
                                                INNER JOIN T_EmployeeLogLedger
                                                    ON Trm_EmployeeId = Ell_EmployeeId
                                                    AND Trm_ModDate = Ell_ProcessDate
                                                    {2}
                                                WHERE Trm_Status IN ('A', '9')
                                                    AND Trm_ModDate >= '{0}' AND Trm_ModDate <= '{1}'
                                                    AND Trm_ApprovedDate = (SELECT MAX(Trm_ApprovedDate)
                                                                            FROM T_TimeRecMod 
                                                                            WHERE Trm_EmployeeId = T.Trm_EmployeeId
                                                                                AND Trm_Status IN ('A', '9')
                                                                                AND Trm_ModDate = T.Trm_ModDate )", DateStart, DateEnd, queryExtension, UserLogin);
                        dalHelper.ExecuteNonQuery(query);
                    }
                }
                #endregion

                #region <Update to REG day code on New and Separated Employees>
                if (CorrectRestday)
                {
                    query = string.Format(@"--Added By Karl Galagar - 7/1/2014 : Update to REG day code on New and Separated Employees
                                              UPDATE T_EmployeeLogLedger
                                                    SET Ell_DayCode = 'REG'
                                                    , Ell_RestDay = 0
                                                    , Ell_Holiday = 0
                                                    , T_EmployeeLogLedger.Usr_Login = '{0}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM T_EmployeeLogLedger
                                                INNER JOIN T_EmployeeMaster
	                                                on Emt_EmployeeID = Ell_EmployeeId
								                WHERE (Ell_ProcessDate < Emt_HireDate)
                                                OR (Emt_JobStatus = 'IN'
                                                AND Emt_SeparationEffectivityDate IS NOT NULL
						                        AND Ell_ProcessDate >= Emt_SeparationEffectivityDate)", UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        //This is the main function to correct the log ledger records (with Profile name)
        public void CorrectLogLedgerRecord(bool ProcessAll, bool ProcessPerPayPeriod, bool UseDataTable
                                            , DataTable EmployeeDataList, string EmployeeFilter, string PayPeriod, string DateStart, string DateEnd
                                            , bool CreateLogLedger, bool ResetWorkType, bool CorrectRestday, bool CorrectHoliday, bool CorrectShift, bool CorrectLogs, bool CorrectWorkflowMovement
                                            , string UserLogin, string DBName)
        {
            //****************************
            //Flag Description:
            // ProcessAll
            //  True: No Employee Condition
            //  False: Use either EmployeeDataList(DataTable) or EmployeeFilter(Comma-separated String); Depends on UseDataTable flag
            //
            // UseDataTable
            //  True: Use EmployeeDataList(DataTable)
            //  False: Use EmployeeFilter(Comma-separated String)
            //
            // ProcessPerPayPeriod
            //  True: Use PayPeriod (query the start and end cycle)
            //  False: Use DateStart and DateEnd
            //
            // CreateLogLedger: Creates log ledger records (if ProcessPerPayPeriod is TRUE)
            // ResetWorkType: Updates the worktype/workgroup in log ledger and employee master (from T_EmployeeGroup)
            // CorrectRestday: Updates the day code and restday 
            // CorrectHoliday: Updates the holiday
            // CorrectShift: Updates the shift code
            // CorrectLogs: Updates the logs from assumed present and time modification tables
            // CorrectWorkflowMovement: Updates the worktype/workgroup, shift, restday and logs based on the ResetWorkType, CorrectShift, CorrectRestday, and CorrectLogs flags, respectively
            //****************************
            try
            {
                #region <Initialize string conditions>
                string query = "";
                string queryExtension = "";
                if (ProcessAll == false)
                {
                    if (UseDataTable == true)
                    {
                        if (EmployeeDataList.Rows.Count > 0)
                        {
                            queryExtension = @" AND Ell_EmployeeId IN (";
                            foreach (DataRow drEmployee in EmployeeDataList.Rows)
                            {
                                queryExtension += string.Format(@"'{0}',", drEmployee["Emt_EmployeeId"].ToString());
                            }
                            queryExtension = queryExtension.Substring(0, queryExtension.Length - 1);
                            queryExtension += @") ";
                        }
                    }
                    else
                    {
                        queryExtension = EmployeeFilter;
                    }
                }

                if (ProcessPerPayPeriod == true)
                {
                    DataTable dtPayperiod = GetCycleRangeFromTargetDB(PayPeriod, DBName);
                    if (dtPayperiod.Rows.Count > 0)
                    {
                        DateStart = dtPayperiod.Rows[0]["Ppm_StartCycle"].ToString();
                        DateEnd = dtPayperiod.Rows[0]["Ppm_EndCycle"].ToString();
                    }
                }
                #endregion

                #region <Create Log Ledger Records if not exist> //Works only if ProcessPerPayPeriod is true
                if (CreateLogLedger == true && ProcessPerPayPeriod == true)
                {
                    queryExtension = queryExtension.Replace("Ell_EmployeeId", "Emt_EmployeeId");
                    GenerateLogLedgerRecord(queryExtension, PayPeriod, UserLogin, DBName);

                    string NextPeriod = GetNextCycle(1);
                    GenerateLogLedgerRecord(queryExtension, NextPeriod, UserLogin, DBName);
                    queryExtension = queryExtension.Replace("Emt_EmployeeId", "Ell_EmployeeId");
                }
                #endregion

                #region <Initialize Worktype and Workgroup>
                if (ResetWorkType == true)
                {
                    queryExtension = queryExtension.Replace("Ell_EmployeeId", "Emt_EmployeeId");
                    query = string.Format(@"--Added By Rendell Uy - 4/11/2013 : Initialize Employee Master Worktype and Workgroup Based on T_EmployeeGroup
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                              
                            UPDATE {3}..T_EmployeeMaster
                            SET Emt_WorkType = Emv_WorkType
                              , Emt_WorkGroup = Emv_WorkGroup
                              , T_EmployeeMaster.Usr_Login = '{2}'
                              , T_EmployeeMaster.Ludatetime = getdate()
                            FROM {3}..T_EmployeeMaster
                            INNER JOIN {3}..T_EmployeeGroup on Emt_EmployeeID = Emv_EmployeeID
                            WHERE @EndNewCycle >= Emv_EffectivityDate
                                   AND ISNULL(Emv_EndDate, @StartNewCycle) >= @StartNewCycle
                                   AND (Emt_WorkType != Emv_WorkType OR Emt_WorkGroup != Emv_WorkGroup)
                                   and Emv_EmployeeID + Convert(Char(10),Emv_EffectivityDate,112) in ( SELECT Emv_EmployeeID + Convert(Char(10), Emv_EffectivityDate, 112)
																                                       FROM (SELECT Emv_EmployeeID , Max(Emv_EffectivityDate) as Emv_EffectivityDate
																		                                    FROM {3}..T_EmployeeGroup
																		                                    WHERE @EndNewCycle >= Emv_EffectivityDate
																			                                    AND ISNULL(Emv_EndDate, @StartNewCycle) >= @StartNewCycle
																		                                    GROUP BY Emv_EmployeeID ) Temp) ", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Emt_EmployeeId", "Ell_EmployeeId");

                    query = string.Format(@"--Added By Rendell Uy - 4/27/2013 : Initialize Worktype and Workgroup
                                        Update {3}..T_EmployeeLogledger
                                        Set Ell_WorkType = Emv_WorkType
                                            , Ell_WorkGroup = Emv_WorkGroup
                                            , T_EmployeeLogledger.Usr_Login = '{2}'
                                            , T_EmployeeLogledger.Ludatetime = getdate()
                                        FROM {3}..T_EmployeeLogledger
                                        INNER JOIN {3}..T_EmployeeGroup
                                        ON Ell_EmployeeId = Emv_EmployeeID
	                                        AND Emv_EffectivityDate = (SELECT TOP 1 Emv_EffectivityDate
								                                        FROM {3}..T_EmployeeGroup
								                                        WHERE Emv_EmployeeID = ELL_EMPLOYEEID
								                                         AND Emv_EffectivityDate <= ELL_PROCESSDATE
								                                        ORDER BY Emv_EffectivityDate DESC)
                                        WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        queryExtension = queryExtension.Replace("Ell_EmployeeId", "Mve_EmployeeId");
                        query = string.Format(@"SELECT * 
                                                FROM {2}..T_Movement M
                                                WHERE Mve_Type = 'G'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                FROM {2}..T_Movement
																				WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'G'
                                                                                    AND Mve_Status IN ('A', '9') ) --Get latest effectivity
                                                            OR Mve_EffectivityDate >= '{0}') --Get effectivity from start of pay period
                                                    AND Mve_ApprovedDate = ( SELECT MAX(Mve_ApprovedDate)
                                                                                FROM {2}..T_Movement
								                                                WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'G'
                                                                                    AND Mve_Status IN ('A', '9')
                                                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )
                                                    {1}
                                                ORDER BY Mve_EmployeeID, Mve_EffectivityDate", DateStart, queryExtension, DBName);
                        DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];

                        foreach (DataRow drRow in dtResult.Rows)
                        {
                            CascadeUpdateWorkgroup(drRow["Mve_EmployeeId"].ToString()
                                                    , Convert.ToDateTime(drRow["Mve_EffectivityDate"].ToString())
                                                    , drRow["Mve_To"].ToString().Substring(0, 3).Trim()
                                                    , drRow["Mve_To"].ToString().Substring(3, 3).Trim()
                                                    , dalHelper); // to be added with db name param
                        }
                        queryExtension = queryExtension.Replace("Mve_EmployeeId", "Ell_EmployeeId");
                    }
                }
                #endregion

                #region <Update Rest Day for REG worktype>
                if (CorrectRestday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 06/24/2013 : Initialize all day codes to REG
                                  UPDATE {3}..[T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = 'REG'
                                        , [Ell_RestDay] = 0
                                        , [Ell_Holiday] = 0
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM {3}..[T_EmployeeLogLedger]
                                    WHERE [Ell_WorkType] = 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    query = string.Format(@"
                                    UPDATE {3}..T_EmployeeLogLedger
                                    SET Ell_RestDay = CASE WHEN REST = '0'
	                                                    THEN 0
	                                                    ELSE 1
                                                      END
                                        ,[Ell_DayCode] = CASE WHEN REST = '0'
	                                                        THEN 'REG'
	                                                        ELSE 'REST'
                                                          END
                                        ,[Ell_Holiday] = 0
                                        , Usr_Login = '{2}'
                                        , Ludatetime = getdate()
                                      FROM {3}..T_EmployeeLogLedger
                                    INNER JOIN 
                                    (
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 2 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 1, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 3 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 2, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 4 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 3, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 5 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 4, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 6 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 5, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 7 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 6, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                        UNION
                                        SELECT	Erd_EmployeeID
                                                , Erd_EffectivityDate
                                                , 1 AS DATPART
                                                , SUBSTRING(Erd_RestDay, 7, 1) AS REST
                                        FROM	{3}..T_EmployeeRestDay
                                    ) EmpRestDay
                                    ON Ell_EmployeeId = Erd_EmployeeID
                                        AND DATEPART(DW, Ell_ProcessDate) = DATPART
                                        AND ERD_EFFECTIVITYDATE = (SELECT TOP 1 ERD_EFFECTIVITYDATE
                                                                    FROM {3}..T_EMPLOYEERESTDAY
                                                                    WHERE ERD_EMPLOYEEID = ELL_EMPLOYEEID
                                                                     AND ERD_EFFECTIVITYDATE <= ELL_PROCESSDATE
                                                                    ORDER BY ERD_EFFECTIVITYDATE DESC)
                                    WHERE [Ell_WorkType] = 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        query = string.Format(@"UPDATE {3}..T_EmployeeLogLedger
                                                SET Ell_DayCode = 'REST'
                                                    , Ell_RestDay = 1
                                                    , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM {3}..T_Movement M
                                                INNER JOIN {3}..T_EmployeeLogLedger
                                                    ON Mve_EmployeeId = Ell_EmployeeId
                                                    AND Mve_To = Ell_ProcessDate
                                                    {1}
                                                WHERE Mve_Type = 'R'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                 FROM {3}..T_Movement
							                                                     WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'R'
                                                                                    AND Mve_Status IN ('A', '9'))
                                                             OR Mve_EffectivityDate >= '{0}')
	                                                    AND Mve_ApprovedDate = (SELECT MAX(Mve_ApprovedDate)
							                                                    FROM {3}..T_Movement 
							                                                    WHERE Mve_EmployeeId = M.Mve_EmployeeId
								                                                    AND Mve_Type = 'R'
								                                                    AND Mve_Status IN ('A', '9')
								                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )

                                                UPDATE {3}..T_EmployeeLogLedger
                                                SET Ell_DayCode = 'REG'
                                                    , Ell_RestDay = 0
                                                    , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM {3}..T_Movement M
                                                INNER JOIN {3}..T_EmployeeLogLedger
                                                    ON Mve_EmployeeId = Ell_EmployeeId
                                                    AND Mve_From = Ell_ProcessDate
                                                    {1}
                                                WHERE Mve_Type = 'R'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                 FROM {3}..T_Movement
							                                                     WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                    AND Mve_Type = 'R'
                                                                                    AND Mve_Status IN ('A', '9'))
                                                             OR Mve_EffectivityDate >= '{0}')
	                                                    AND Mve_ApprovedDate = (SELECT MAX(Mve_ApprovedDate)
							                                                    FROM {3}..T_Movement 
							                                                    WHERE Mve_EmployeeId = M.Mve_EmployeeId
								                                                    AND Mve_Type = 'R'
								                                                    AND Mve_Status IN ('A', '9')
								                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )", DateStart, queryExtension, UserLogin, DBName);
                        dalHelper.ExecuteNonQuery(query);
                    }
                }
                #endregion

                #region <Initial Day Code based on Calendar Group>
                if (CorrectRestday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 06/24/2013 : Initialize all day codes to REG
                                  UPDATE {3}..[T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = 'REG'
                                        , [Ell_RestDay] = 0
                                        , [Ell_Holiday] = 0
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM {3}..[T_EmployeeLogLedger]
                                    WHERE [Ell_WorkType] != 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Initial Day Code based on Calendar Group
                                  UPDATE {3}..[T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = CASE WHEN RTRIM(Cal_WorkCode) = 'R' THEN 'REST'
                                                               WHEN LEN(RTRIM(Cal_WorkCode)) > 1 THEN 'REG' + SUBSTRING(RTRIM(Cal_WorkCode), 2, 1)
                                                               ELSE 'REG' END
                                        , [Ell_RestDay] = CASE WHEN RTRIM(Cal_WorkCode) = 'R' THEN 1 ELSE 0 END
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM {3}..[T_EmployeeLogLedger] 
                                    INNER JOIN {3}..[T_CalendarGroupTmp]
                                        ON Cal_WorkType = Ell_WorkType
                                        AND Cal_WorkGroup = Ell_WorkGroup
                                        AND Cal_ProcessDate = Ell_ProcessDate
                                    WHERE [Ell_WorkType] != 'REG'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Update Shift Code for REG worktype>
                if (CorrectShift == true)
                {
                    query = string.Format(@"--New query for update shift in logledger (REG worktype)
                                              UPDATE {4}..[T_EmployeeLogLedger]
                                                SET [Ell_ShiftCode] = Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                                                           then Emt_Shiftcode
                                                                           else
                                                                              case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                                                   then Scm_EquivalentShiftCode
                                                                                   else Emt_Shiftcode
                                                                              end
                                                                      end
                                                    , T_EmployeeLogledger.Usr_Login = '{2}'
                                                    , T_EmployeeLogledger.Ludatetime = getdate()
                                              From {4}..T_EmployeeLogledger
                                              Inner Join {4}..T_EmployeeMaster on Emt_EmployeeID = Ell_EmployeeID
                                              Inner Join {4}..T_Shiftcodemaster on Scm_Shiftcode = Emt_Shiftcode
                                              WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'
                                              AND Ell_WorkType = 'REG'
                                              @EXTENSION

                                            IF (SELECT Pmx_ParameterValue FROM {4}..T_ParameterMasterExt WHERE Pmx_ParameterID = 'SHFTCYCOPN' AND Pmx_Classification = 'LSTREGSHFT' AND Pmx_Status = 'A') = 1
                                            BEGIN
                                                DECLARE @ShiftTable as TABLE
                                                (Ell_EmployeeID varchar(MAX), Ell_ProcessDate datetime)

                                                DECLARE @DateStart datetime
                                                SET @DateStart = (SELECT Ppm_StartCycle FROM {4}..T_PayPeriodMaster WHERE Ppm_PayPeriod = '{3}')

                                                INSERT INTO @ShiftTable
                                                SELECT Ell_EmployeeID, MAX(Ell_ProcessDate)
                                                FROM
                                                (SELECT Ell_EmployeeId, MAX(Ell_ProcessDate) Ell_ProcessDate FROM {4}..T_EmployeeLogLedger
                                                INNER JOIN {4}..T_EmployeeMaster ON Emt_EmployeeID = Ell_EmployeeId
                                                WHERE Ell_RestDay = 0 and Ell_Holiday = 0
                                                AND Ell_ProcessDate < @DateStart
                                                GROUP BY Ell_EmployeeId

                                                UNION 

                                                SELECT Ell_EmployeeId, MAX(Ell_ProcessDate) Ell_ProcessDate FROM {4}..T_EmployeeLogLedgerHist
                                                INNER JOIN {4}..T_EmployeeMaster ON Emt_EmployeeID = Ell_EmployeeId
                                                WHERE Ell_RestDay = 0 and Ell_Holiday = 0
                                                AND Ell_ProcessDate < @DateStart
                                                GROUP BY Ell_EmployeeId)TEMP
                                                GROUP BY Ell_EmployeeId
                                                ORDER BY Ell_EmployeeId

                                                UPDATE {4}..T_EmployeeLogLedger 
                                                SET [Ell_ShiftCode] = Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                                then ELLPREV.Ell_ShiftCode
                                                else
                                                    case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                        then Scm_EquivalentShiftCode
                                                        else ELLPREV.Ell_ShiftCode
                                                    end
                                                end
                                                , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                , T_EmployeeLogLedger.Ludatetime = getdate()
                                                 FROM {4}..T_EmployeeLogLedger ELL
                                                LEFT JOIN 
                                                (
	                                                SELECT SHFT.Ell_EmployeeID, SHFT.Ell_ProcessDate, Ell_ShiftCode
	                                                FROM {4}..T_EmployeeLogLedger LDR
	                                                INNER JOIN @ShiftTable SHFT
	                                                ON SHFT.Ell_EmployeeID = LDR.Ell_EmployeeId
	                                                AND SHFT.Ell_ProcessDate = LDR.Ell_ProcessDate

	                                                UNION 

	                                                SELECT SHFT.Ell_EmployeeID, SHFT.Ell_ProcessDate, Ell_ShiftCode
	                                                FROM {4}..T_EmployeeLogLedgerHist LDR
	                                                INNER JOIN @ShiftTable SHFT
	                                                ON SHFT.Ell_EmployeeID = LDR.Ell_EmployeeId
	                                                AND SHFT.Ell_ProcessDate = LDR.Ell_ProcessDate
                                                )ELLPREV
                                                ON ELLPREV.Ell_EmployeeId = ELL.Ell_EmployeeId
                                                INNER JOIN {4}..T_ShiftCodeMaster ON Scm_ShiftCode = ELLPREV.Ell_ShiftCode
                                                INNER JOIN {4}..T_EmployeeGroup ON Emv_EmployeeID = ELL.Ell_EmployeeId
                                                AND Emv_EffectivityDate = (SELECT TOP 1 Emv_EffectivityDate
							                                                FROM {4}..T_EmployeeGroup
							                                                WHERE Emv_EmployeeID = ELL.ELL_EMPLOYEEID
								                                                AND Emv_EffectivityDate <= ELL.ELL_PROCESSDATE
							                                                ORDER BY Emv_EffectivityDate DESC)
                                                AND Emv_WorkType = 'REG'
                                                WHERE ELL.Ell_PayPeriod = '{3}'
                                                @INNEREXTENSION
                                            END", DateStart, DateEnd, UserLogin, PayPeriod, DBName);
                    query = query.Replace("@EXTENSION", queryExtension);
                    query = query.Replace("@INNEREXTENSION", queryExtension.Replace("Ell_", "ELL.Ell_"));
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        query = string.Format(@"UPDATE {3}..T_EmployeeLogLedger
                                                SET Ell_ShiftCode = Mve_To
                                                    , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM {3}..T_Movement M
                                                INNER JOIN {3}..T_EmployeeLogLedger
                                                    ON Mve_EmployeeId = Ell_EmployeeId
                                                    AND Mve_EffectivityDate = Ell_ProcessDate
                                                    {1}
                                                WHERE Mve_Type = 'S'
                                                    AND Mve_Status IN ('A', '9')
                                                    AND Mve_To != Ell_ShiftCode
                                                    AND (Mve_EffectivityDate = ( SELECT MAX(Mve_EffectivityDate)
                                                                                    FROM {3}..T_Movement
								                                                    WHERE Mve_EmployeeID = M.Mve_EmployeeId
                                                                                        AND Mve_Type = 'S'
                                                                                        AND Mve_Status IN ('A', '9'))
                                                             OR Mve_EffectivityDate >= '{0}')
	                                                    AND Mve_ApprovedDate = (SELECT MAX(Mve_ApprovedDate)
							                                                    FROM {3}..T_Movement 
							                                                    WHERE Mve_EmployeeId = M.Mve_EmployeeId
								                                                    AND Mve_Type = 'S'
								                                                    AND Mve_Status IN ('A', '9')
								                                                    AND Mve_EffectivityDate = M.Mve_EffectivityDate )", DateStart, queryExtension, UserLogin, DBName);
                        dalHelper.ExecuteNonQuery(query);
                    }
                }
                #endregion

                #region <Initial Shift Code based on Calendar Group>
                if (CorrectShift == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Shift Code based on Calendar Group
                                              UPDATE {3}..[T_EmployeeLogLedger]
                                                 SET [Ell_ShiftCode] = Cal_ShiftCode
                                                    , T_EmployeeLogledger.Usr_Login = '{2}'
                                                    , T_EmployeeLogledger.Ludatetime = getdate()
                                                FROM {3}..[T_EmployeeLogLedger] 
                                                INNER JOIN {3}..[T_CalendarGroupTmp]
                                                    ON Cal_WorkType = Ell_WorkType
                                                    AND Cal_WorkGroup = Ell_WorkGroup
                                                    AND Cal_ProcessDate = Ell_ProcessDate
                                                WHERE [Ell_WorkType] != 'REG'
                                                    AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Special Day>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy -2/22/2011 : Special Day Update
                          UPDATE {3}..T_EmployeeLogLedger
                            SET Ell_DayCode = Ard_DayCode,
                                Ell_RestDay = CASE WHEN (Ard_DayCode = 'REST') THEN 1 ELSE Ell_RestDay END,
                                Ell_Holiday = Dcm_Holiday,
                                Ell_Shiftcode = CASE WHEN Ell_WorkType = 'REG' -- Non-Calendar
                                                THEN CASE WHEN (Ard_DayCode = 'REG') 
                                                    THEN Emt_Shiftcode 
                                                    ELSE
                                                        (SELECT case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                                   then Scm_EquivalentShiftCode
                                                                   else Scm_ShiftCode
                                                                end
                                                        FROM {3}..T_ShiftCodeMaster
                                                        WHERE Scm_ShiftCode = Emt_Shiftcode) 
                                                    END
                                                ELSE
                                                    Ell_Shiftcode
                                                END,
                                T_EmployeeLogLedger.Usr_Login = '{2}',
                                T_EmployeeLogLedger.Ludatetime = getdate()
                            FROM {3}..T_EmployeeLogLedger
                            INNER JOIN {3}..T_EmployeeMaster
                            ON Ell_EmployeeId = Emt_EmployeeId
                            INNER JOIN {3}..T_SpecialDayMaster
                            ON Ell_ProcessDate = Ard_Date
                                AND Ell_WorkType = Ard_WorkType
                                AND Ell_WorkGroup = Ard_WorkGroup
                                AND Emt_PayrollType = Ard_PayrollType
                            INNER JOIN {3}..T_DayCodeMaster
                            ON Dcm_DayCode = Ard_DayCode
                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Location Code>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Update Location Code
                          UPDATE {3}..T_EmployeeLogLedger
                            SET Ell_LocationCode = (Select TOP(1) Ewl_LocationCode 
                                                    From {3}..T_EmployeeWorkLocation
                                                        Where Ewl_EmployeeID = Ell_EmployeeId 
	                                                     And Ewl_EffectivityDate <= Ell_ProcessDate
                                                         And Ell_ProcessDate <= ISNULL(Ewl_EndDate, Ell_ProcessDate)
	                                                     Order By Ewl_EffectivityDate DESC)
                                , Usr_Login = '{2}'
                                , Ludatetime = getdate()
                            FROM {3}..T_EmployeeLogLedger
                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Holiday>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 08/04/2010 : Update Holiday Code
                          UPDATE {3}..T_EmployeeLogLedger
                            SET Ell_DayCode = Hmt_HolidayCode
                                , Ell_Holiday = Dcm_Holiday
                                , Ell_ShiftCode = Case when Ell_RestDay = 0 and Dcm_Holiday = 0
                                                       then Ell_ShiftCode
                                                       else
                                                          case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                               then Scm_EquivalentShiftCode
                                                               else Ell_ShiftCode
                                                          end
                                                  end
                                , T_EmployeeLogLedger.Usr_Login = '{2}'
                                , T_EmployeeLogLedger.Ludatetime = getdate()
                            FROM {3}..T_EmployeeLogLedger
                            INNER JOIN {3}..T_EmployeeMaster
                            ON Ell_EmployeeId = Emt_EmployeeID
                            INNER JOIN {3}..T_HolidayMaster
	                            ON Ell_Processdate = Hmt_HolidayDate
	                            AND (Hmt_ApplicCity = Ell_LocationCode
                                    OR Hmt_ApplicCity = 'ALL')
	                            AND (Hmt_PayTypeExclusion IS NULL OR Hmt_PayTypeExclusion = '' OR Emt_PayrollType NOT IN (SELECT Data FROM {3}.dbo.Split(Hmt_PayTypeExclusion,',')))
	                            AND (Hmt_JobStatusExclusion IS NULL OR Hmt_JobStatusExclusion = '' OR Emt_JobStatus NOT IN (SELECT Data FROM {3}.dbo.Split(Hmt_JobStatusExclusion,',')))
	                            AND (Hmt_EmpStatusExclusion IS NULL OR Hmt_EmpStatusExclusion = '' OR Emt_EmploymentStatus NOT IN (SELECT Data FROM {3}.dbo.Split(Hmt_EmpStatusExclusion,',')))
	                            AND (Hmt_WorktypegroupExclusion IS NULL OR Hmt_WorktypegroupExclusion = '' OR RTRIM(Ell_WorkType)+'/'+RTRIM(Ell_WorkGroup) NOT IN (SELECT Data FROM {3}.dbo.Split(Hmt_WorktypegroupExclusion,',')))
                            INNER JOIN {3}..T_DayCodeMaster
                            ON Hmt_HolidayCode = Dcm_DayCode
                            INNER JOIN {3}..T_Shiftcodemaster 
                            ON Scm_Shiftcode = Ell_ShiftCode
                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Day Code Filler Restday>
                if (CorrectRestday == true || CorrectHoliday == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 4/11/2013 : Added Restday Indicator in Day Code Master
                                    UPDATE {3}..T_EmployeeLogLedger
                                    SET Ell_RestDay = Dcm_Restday
                                        , T_EmployeeLogLedger.Usr_Login = '{2}'
                                        , T_EmployeeLogLedger.Ludatetime = getdate()
                                    FROM {3}..T_EmployeeLogLedger
                                    INNER JOIN {3}..T_DayCodeFiller
                                        ON Ell_DayCode = Dcf_DayCode
                                    INNER JOIN {3}..T_DayCodeMaster
										ON Dcf_DayCode = Dcm_DayCode
                                    WHERE Dcf_Status = 'A'
                                        AND [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Employee Assumed Present>
                if (CorrectLogs == true)
                {
                    query = string.Format(@"--Added By Rendell Uy - 6/18/2013 : Assumed Present Updating
                                              UPDATE {3}..T_EmployeeLogLedger
                                                SET Ell_AssumedPresent = 1
                                                    ,Ell_AssumedPresentBy = '{2}'
                                                    ,Ell_AssumedPresentDate = getdate()
                                                FROM {3}..T_EmployeeLogLedger
                                                INNER JOIN {3}..T_EmployeeAssumedPresent 
	                                                on Eap_EmployeeID = ell_employeeid
	                                                and Ell_ProcessDate between Eap_StartDate and ISNULL(Eap_EndDate, Ell_ProcessDate)
                                                WHERE Ell_DayCode = 'REG'
                                                    and [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'", DateStart, DateEnd, UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    if (CorrectWorkflowMovement == true)
                    {
                        query = string.Format(@"UPDATE {4}..T_EmployeeLogLedger
                                                SET Ell_ActualTimeIn_1 = CASE WHEN Trm_ActualTimeIn1 != ''
							                                                THEN Trm_ActualTimeIn1
							                                                ELSE '0000'
							                                                END
                                                , Ell_ActualTimeOut_1 = CASE WHEN Trm_ActualTimeOut1 != ''
							                                                THEN Trm_ActualTimeOut1
							                                                ELSE '0000'
							                                                END
                                                , Ell_ActualTimeIn_2 = CASE WHEN Trm_ActualTimeIn2 != ''
							                                                THEN Trm_ActualTimeIn2
							                                                ELSE '0000'
							                                                END
                                                , Ell_ActualTimeOut_2 = CASE WHEN Trm_ActualTimeOut2 != ''
							                                                THEN Trm_ActualTimeOut2
							                                                ELSE '0000'
							                                                END
                                                , T_EmployeeLogLedger.Usr_Login = '{3}'
                                                , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM {4}..T_TimeRecMod T
                                                INNER JOIN {4}..T_EmployeeLogLedger
                                                    ON Trm_EmployeeId = Ell_EmployeeId
                                                    AND Trm_ModDate = Ell_ProcessDate
                                                    {2}
                                                WHERE Trm_Status IN ('A', '9')
                                                    AND Trm_ModDate >= '{0}' AND Trm_ModDate <= '{1}'
                                                    AND Trm_ApprovedDate = (SELECT MAX(Trm_ApprovedDate)
                                                                            FROM {4}..T_TimeRecMod 
                                                                            WHERE Trm_EmployeeId = T.Trm_EmployeeId
                                                                                AND Trm_Status IN ('A', '9')
                                                                                AND Trm_ModDate = T.Trm_ModDate )", DateStart, DateEnd, queryExtension, UserLogin, DBName);
                        dalHelper.ExecuteNonQuery(query);
                    }
                }
                #endregion

                #region <Update to REG day code on New and Separated Employees>
                if (CorrectRestday)
                {
                    query = string.Format(@"--Added By Karl Galagar - 7/1/2014 : Update to REG day code on New and Separated Employees
                                              UPDATE {1}..T_EmployeeLogLedger
                                                    SET Ell_DayCode = 'REG'
                                                    , Ell_RestDay = 0
                                                    , Ell_Holiday = 0
                                                    , T_EmployeeLogLedger.Usr_Login = '{0}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM {1}..T_EmployeeLogLedger
                                                INNER JOIN {1}..T_EmployeeMaster
	                                                on Emt_EmployeeID = Ell_EmployeeId
								                WHERE (Ell_ProcessDate < Emt_HireDate)
                                                OR (Emt_JobStatus = 'IN'
                                                AND Emt_SeparationEffectivityDate IS NOT NULL
						                        AND Ell_ProcessDate >= Emt_SeparationEffectivityDate)", UserLogin, DBName);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public void CorrectDayCodeNewAndSeparatedEmployee(string EmployeeID, string UserLogin)
        {
            #region <Update to REG day code on New and Separated Employees>
            string query = string.Format(@"--Added By Karl Galagar - 7/1/2014 : Update to REG day code on New and Separated Employees
                                              UPDATE T_EmployeeLogLedger
                                                    SET Ell_DayCode = 'REG'
                                                    , Ell_RestDay = 0
                                                    , Ell_Holiday = 0
                                                    , T_EmployeeLogLedger.Usr_Login = '{1}'
                                                    , T_EmployeeLogLedger.Ludatetime = getdate()
                                                FROM T_EmployeeLogLedger
                                                INNER JOIN T_EmployeeMaster
	                                                on Emt_EmployeeID = Ell_EmployeeId
								                WHERE (Ell_ProcessDate < Emt_HireDate)
                                                OR (Emt_JobStatus = 'IN'
                                                AND Emt_SeparationEffectivityDate IS NOT NULL
						                        AND Ell_ProcessDate >= Emt_SeparationEffectivityDate)
                                                AND Ell_EmployeeID = '{0}'", EmployeeID, UserLogin);

            dalHelper.ExecuteNonQuery(query);
            #endregion
        }

        public void CorrectShiftRecord(string PayPeriod, string UserLogin)
        {
            DataTable dtPayperiod = GetCycleRange(PayPeriod);
            string DateStart = string.Empty, DateEnd = string.Empty;

            if (dtPayperiod.Rows.Count > 0)
            {
                DateStart = dtPayperiod.Rows[0]["Ppm_StartCycle"].ToString();
                DateEnd = dtPayperiod.Rows[0]["Ppm_EndCycle"].ToString();
            }

            #region query
            string query = string.Format(@"
                                        IF (SELECT Pmx_ParameterValue FROM T_ParameterMasterExt WHERE Pmx_ParameterID = 'SHFTCYCOPN' AND Pmx_Classification = 'NOUPDATE' AND Pmx_Status = 'A') = 0
                                        BEGIN
                                            --New query for update shift in logledger (REG worktype)
                                              UPDATE [T_EmployeeLogLedger]
                                                SET [Ell_ShiftCode] = Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                                                           then Emt_Shiftcode
                                                                           else
                                                                              case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                                                   then Scm_EquivalentShiftCode
                                                                                   else Emt_Shiftcode
                                                                              end
                                                                      end
                                                    , T_EmployeeLogledger.Usr_Login = '{2}'
                                                    , T_EmployeeLogledger.Ludatetime = getdate()
                                              From T_EmployeeLogledger
                                              Inner Join T_EmployeeMaster on Emt_EmployeeID = Ell_EmployeeID
                                              Inner Join T_Shiftcodemaster on Scm_Shiftcode = Emt_Shiftcode
                                              WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'
                                                AND Ell_WorkType = 'REG'

                                            IF (SELECT Pmx_ParameterValue FROM T_ParameterMasterExt WHERE Pmx_ParameterID = 'SHFTCYCOPN' AND Pmx_Classification = 'LSTREGSHFT' AND Pmx_Status = 'A') = 1
                                            BEGIN
                                                DECLARE @ShiftTable as TABLE
                                                (Ell_EmployeeID varchar(MAX), Ell_ProcessDate datetime)

                                                DECLARE @DateStart datetime
                                                SET @DateStart = (SELECT Ppm_StartCycle FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = '{3}')

                                                INSERT INTO @ShiftTable
                                                SELECT Ell_EmployeeID, MAX(Ell_ProcessDate)
                                                FROM
                                                (SELECT Ell_EmployeeId, MAX(Ell_ProcessDate) Ell_ProcessDate FROM T_EmployeeLogLedger
                                                INNER JOIN T_EmployeeMaster ON Emt_EmployeeID = Ell_EmployeeId
                                                WHERE Ell_RestDay = 0 and Ell_Holiday = 0
                                                AND Ell_ProcessDate < @DateStart
                                                GROUP BY Ell_EmployeeId

                                                UNION 

                                                SELECT Ell_EmployeeId, MAX(Ell_ProcessDate) Ell_ProcessDate FROM T_EmployeeLogLedgerHist
                                                INNER JOIN T_EmployeeMaster ON Emt_EmployeeID = Ell_EmployeeId
                                                WHERE Ell_RestDay = 0 and Ell_Holiday = 0
                                                AND Ell_ProcessDate < @DateStart
                                                GROUP BY Ell_EmployeeId)TEMP
                                                GROUP BY Ell_EmployeeId
                                                ORDER BY Ell_EmployeeId

                                                UPDATE T_EmployeeLogLedger 
                                                SET [Ell_ShiftCode] = Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                                then ELLPREV.Ell_ShiftCode
                                                else
                                                    case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                        then Scm_EquivalentShiftCode
                                                        else ELLPREV.Ell_ShiftCode
                                                    end
                                                end
                                                , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                , T_EmployeeLogLedger.Ludatetime = getdate()
                                                 FROM T_EmployeeLogLedger ELL
                                                LEFT JOIN 
                                                (
	                                                SELECT SHFT.Ell_EmployeeID, SHFT.Ell_ProcessDate, Ell_ShiftCode
	                                                FROM T_EmployeeLogLedger LDR
	                                                INNER JOIN @ShiftTable SHFT
	                                                ON SHFT.Ell_EmployeeID = LDR.Ell_EmployeeId
	                                                AND SHFT.Ell_ProcessDate = LDR.Ell_ProcessDate

	                                                UNION 

	                                                SELECT SHFT.Ell_EmployeeID, SHFT.Ell_ProcessDate, Ell_ShiftCode
	                                                FROM T_EmployeeLogLedgerHist LDR
	                                                INNER JOIN @ShiftTable SHFT
	                                                ON SHFT.Ell_EmployeeID = LDR.Ell_EmployeeId
	                                                AND SHFT.Ell_ProcessDate = LDR.Ell_ProcessDate
                                                )ELLPREV
                                                ON ELLPREV.Ell_EmployeeId = ELL.Ell_EmployeeId
                                                INNER JOIN T_ShiftCodeMaster ON Scm_ShiftCode = ELLPREV.Ell_ShiftCode
                                                INNER JOIN T_EmployeeGroup ON Emv_EmployeeID = ELL.Ell_EmployeeId
                                                AND Emv_EffectivityDate = (SELECT TOP 1 Emv_EffectivityDate
							                                                FROM T_EmployeeGroup
							                                                WHERE Emv_EmployeeID = ELL.ELL_EMPLOYEEID
								                                                AND Emv_EffectivityDate <= ELL.ELL_PROCESSDATE
							                                                ORDER BY Emv_EffectivityDate DESC)
                                                AND Emv_WorkType = 'REG'
                                                WHERE ELL.Ell_PayPeriod = '{3}'
                                            END

                                            --update holiday and the equivalent shift
                                            UPDATE T_EmployeeLogLedger
                                            SET Ell_DayCode = Hmt_HolidayCode
                                                , Ell_Holiday = Dcm_Holiday
                                                , Ell_ShiftCode = Case when Ell_RestDay = 0 and Dcm_Holiday = 0
                                                                       then Ell_ShiftCode
                                                                       else
                                                                          case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                                               then Scm_EquivalentShiftCode
                                                                               else Ell_ShiftCode
                                                                          end
                                                                  end
                                                , T_EmployeeLogLedger.Usr_Login = '{2}'
                                                , T_EmployeeLogLedger.Ludatetime = getdate()
                                            FROM T_EmployeeLogLedger
                                            INNER JOIN T_HolidayMaster
	                                            ON Ell_Processdate = Hmt_HolidayDate
	                                            AND (Hmt_ApplicCity = Ell_LocationCode
                                                OR Hmt_ApplicCity = 'ALL')
                                            INNER JOIN T_DayCodeMaster
                                            ON Hmt_HolidayCode = Dcm_DayCode
                                            INNER JOIN T_Shiftcodemaster 
                                            ON Scm_Shiftcode = Ell_ShiftCode
                                            WHERE [Ell_ProcessDate] >= '{0}' AND [Ell_ProcessDate] <= '{1}'
                                        END", DateStart, DateEnd, UserLogin, PayPeriod);
            #endregion

            dalHelper.ExecuteNonQuery(query);
        }

        public string GetLocationCode(string processdate, string IdNum, DALHelper dal)
        {
            WorkLocationBL wrkLocBL = new WorkLocationBL();

            string LatestEffectDate = string.Empty;
            string LocationCode = string.Empty;

            if (IdNum != string.Empty)
            {
                LatestEffectDate = wrkLocBL.GetLatestEffectDate(processdate, IdNum, dal);
                if (LatestEffectDate != string.Empty)
                {
                    LocationCode = wrkLocBL.GetEffectiveLocationCode(LatestEffectDate, IdNum, dal);
                }
            }

            if (LocationCode.Trim() != string.Empty)
            {
                ////reynard::20090527 To update work location
                string sqlLocUpdate = @"update t_EmployeeLogLedger set Ell_LocationCode='{2}' where ell_employeeid='{0}' and ell_processdate='{1}'";
                dalHelper.ExecuteNonQuery(string.Format(sqlLocUpdate, IdNum, processdate, LocationCode), CommandType.Text);
                ////end
                return LocationCode;
            }
            else
            {
                return "REG";
            }
        }

        public void CascadeUpdateWorkgroup(string EmployeeId, DateTime dtEffectivityDate, string Worktype, string Workgroup, DALHelper dalHelper)
        {
            DataRow drCycleRange = dalHelper.ExecuteDataSet(@" SELECT MIN(Ell_ProcessDate) [Min]
                                                                    , MAX(Ell_ProcessDate) [Max]
                                                               FROM T_EmployeeLogLedger").Tables[0].Rows[0];
            DateTime dtEllStart = Convert.ToDateTime(drCycleRange["Min"]);
            DateTime dtEllEnd = Convert.ToDateTime(drCycleRange["Max"]);

            ParameterInfo[] paramInfo = new ParameterInfo[7];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", EmployeeId);
            paramInfo[4] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[5] = new ParameterInfo("@Ell_WorkType", Worktype);
            paramInfo[6] = new ParameterInfo("@Ell_WorkGroup", Workgroup);

            string query;
            while (dtEffectivityDate <= dtEllEnd && dtEffectivityDate >= dtEllStart)
            {
                //Set default values
                paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dtEffectivityDate);
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", "REG");
                paramInfo[3] = new ParameterInfo("@Ell_RestDay", "False");

                //Check for special shifting
                string DayCode = GetInitialDayCode(Worktype, Workgroup, dtEffectivityDate);
                if (!DayCode.Equals("REG"))
                {
                    paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                    if (DayCode.Equals("REST"))
                    {
                        paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                    }
                }

                //Update Log Ledger table
                query = @"UPDATE [T_EmployeeLogLedger]
                                         SET [Ell_DayCode] = @Ell_DayCode
                                            ,[Ell_RestDay] = @Ell_RestDay
                                            ,[Ell_WorkType] = @Ell_WorkType
                                            ,[Ell_WorkGroup] = @Ell_WorkGroup
                                            ,[Usr_Login] = @UserLogin
                                            ,[Ludatetime] = GETDATE()
                                       WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                         AND [Ell_ProcessDate] = @Ell_ProcessDate";
                dalHelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);

                dtEffectivityDate = dtEffectivityDate.AddDays(1);
            }
        }

        public DataSet CompleteCascadeUpdateWorkgroupPerDate(string EmployeeId, DateTime dtProcessDate, string Worktype, string Workgroup, string PayrollType, DALHelper dalHelper)
        {
            //Set default values
            string query;
            dtProcessDate = Convert.ToDateTime(dtProcessDate.ToShortDateString());

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", EmployeeId);
            paramInfo[4] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[5] = new ParameterInfo("@Ell_WorkType", Worktype);
            paramInfo[6] = new ParameterInfo("@Ell_WorkGroup", Workgroup);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dtProcessDate);
            paramInfo[2] = new ParameterInfo("@Ell_DayCode", "REG");
            paramInfo[3] = new ParameterInfo("@Ell_RestDay", "False");
            paramInfo[7] = new ParameterInfo("@Ell_Holiday", "False");

            //Check for special shifting
            string DayCode = GetInitialDayCode(Worktype, Workgroup, dtProcessDate);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                if (DayCode.Equals("REST"))
                {
                    paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                }
            }

            //Check if rest day
            DayCode = isRestDay(EmployeeId, dtProcessDate);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
            }

            //Check for special day
            DayCode = isSpecialDay(Worktype, Workgroup, dtProcessDate, PayrollType);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                if (DayCode.Equals("REST"))
                {
                    paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                }
            }

            //Check if holiday
            DayCode = isHoliday(this.GetLocationCode(dtProcessDate.ToShortDateString(), EmployeeId, dalHelper), dtProcessDate);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                paramInfo[7] = new ParameterInfo("@Ell_Holiday", "True");
            }

            //Update Log Ledger table
            query = string.Format(@"UPDATE [T_EmployeeLogLedger]
                                     SET [Ell_DayCode] = '{2}'
                                        ,[Ell_RestDay] = '{3}'
                                        ,[Ell_Holiday] = '{4}'
                                        ,[Ell_WorkType] = '{5}'
                                        ,[Ell_WorkGroup] = '{6}'
                                        ,[Usr_Login] = '{7}'
                                        ,[Ludatetime] = GETDATE()
                                   WHERE [Ell_EmployeeId] = '{0}'
                                     AND [Ell_ProcessDate] = '{1}'", EmployeeId
                                                                  , dtProcessDate
                                                                  , paramInfo[2].Value
                                                                  , paramInfo[3].Value
                                                                  , paramInfo[7].Value
                                                                  , Worktype
                                                                  , Workgroup
                                                                  , UserCode);
            dalHelper.ExecuteNonQuery(query);

            if (!Worktype.Equals("") && !Worktype.ToString().Equals("REG"))
            {
                //Update shift code for employee with special shift (e.g. 4-2, 5-1, 512)
                query = string.Format(@"UPDATE T_EmployeeLogLedger
                                        SET Ell_ShiftCode = (SELECT Cal_ShiftCode_{0:dd} 
                                                              FROM T_CalendarGroup
                                                              WHERE Cal_WorkType = '{2}'
                                                              AND Cal_WorkGroup = '{3}'
                                                              AND Cal_YearMonth = {0:yyyyMM})
                                        WHERE Ell_EmployeeId = '{1}'
                                        AND Ell_ProcessDate = '{0}'", dtProcessDate, EmployeeId, Worktype, Workgroup);
            }
            dalHelper.ExecuteNonQuery(query);

            //Get Log Ledger detail
            query = string.Format(@"SELECT [Ell_DayCode]
                                        ,[Ell_RestDay]
                                        ,[Ell_Holiday] 
                                        ,[Ell_WorkType]
                                        ,[Ell_WorkGroup] 
                                        ,[Ell_ShiftCode]
                                   FROM [T_EmployeeLogLedger]
                                   WHERE [Ell_EmployeeId] = '{0}'
                                     AND [Ell_ProcessDate] = '{1}'", EmployeeId, dtProcessDate);
            DataSet ds = dalHelper.ExecuteDataSet(query);

            return ds;
        }

        public string[] GetWorkgroupDetails(string EmployeeId, DateTime dtProcessDate, string Worktype, string Workgroup, string PayrollType, DALHelper dalHelper)
        {
            //Set default values
            string[] strWorkgroupDetails = new string[2];
            string query;
            dtProcessDate = Convert.ToDateTime(dtProcessDate.ToShortDateString());

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", EmployeeId);
            paramInfo[4] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[5] = new ParameterInfo("@Ell_WorkType", Worktype);
            paramInfo[6] = new ParameterInfo("@Ell_WorkGroup", Workgroup);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dtProcessDate);
            paramInfo[2] = new ParameterInfo("@Ell_DayCode", "REG");
            paramInfo[3] = new ParameterInfo("@Ell_RestDay", "False");
            paramInfo[7] = new ParameterInfo("@Ell_Holiday", "False");

            //Check for special shifting
            string DayCode = GetInitialDayCode(Worktype, Workgroup, dtProcessDate);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                if (DayCode.Equals("REST"))
                {
                    paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                }
            }

            //Check if rest day
            DayCode = isRestDay(EmployeeId, dtProcessDate);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
            }

            //Check for special day
            DayCode = isSpecialDay(Worktype, Workgroup, dtProcessDate, PayrollType);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                if (DayCode.Equals("REST"))
                {
                    paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                }
            }

            //Check if holiday
            DayCode = isHoliday(this.GetLocationCode(dtProcessDate.ToShortDateString(), EmployeeId, dalHelper), dtProcessDate);
            if (!DayCode.Equals("REG"))
            {
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                paramInfo[7] = new ParameterInfo("@Ell_Holiday", "True");
            }

            if (!Worktype.Equals("") && !Worktype.ToString().Equals("REG"))
            {
                //Get shift code for employee with special shift (e.g. 4-2, 5-1, 512)
                query = string.Format(@"SELECT Cal_ShiftCode_{0:dd} 
                                          FROM T_CalendarGroup
                                          WHERE Cal_WorkType = '{1}'
                                          AND Cal_WorkGroup = '{2}'
                                          AND Cal_YearMonth = {0:yyyyMM}", dtProcessDate, Worktype, Workgroup);
            }
            else
            {
                //Get shift code for employee with REG shift
                query = string.Format(@"SELECT Case when Ell_RestDay = 0 and Ell_Holiday = 0
                                               then Emt_Shiftcode
                                               else
                                                  case when Len(Rtrim(Scm_EquivalentShiftCode)) > 0 
                                                       then Scm_EquivalentShiftCode
                                                       else Emt_Shiftcode
                                                  end
                                              end as ShiftCode
                                      From T_EmployeeLogledger
                                      Inner Join T_EmployeeMaster on Emt_EmployeeID = Ell_EmployeeID
                                      Inner Join T_Shiftcodemaster on Scm_Shiftcode = Emt_Shiftcode
                                      WHERE [Ell_EmployeeId] = '{0}'
                                        AND [Ell_ProcessDate] = '{1}'", EmployeeId, dtProcessDate);
            }
            DataSet ds = dalHelper.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0)
                strWorkgroupDetails[1] = ds.Tables[0].Rows[0][0].ToString();

            strWorkgroupDetails[0] = paramInfo[2].Value.ToString();

            return strWorkgroupDetails;
        }

        public string GetInitialDayCode(string strWorkType, string strWorkGroup, DateTime ProcessDate)
        {
            string Value = "REG";
            try
            {
                if (!(strWorkType.Equals("REG")))
                {
                    string sqlCalendarGrpQuery = string.Format(@"SELECT Cal_WorkCode_{0:dd} 
                                                                  FROM T_CalendarGroup
                                                                  WHERE Cal_WorkType = '{1}'
                                                                  AND Cal_WorkGroup = '{2}'
                                                                  AND Cal_YearMonth = '{0:yyyyMM}'", ProcessDate, strWorkType, strWorkGroup);

                    DataTable dtResult;
                    dtResult = dalHelper.ExecuteDataSet(sqlCalendarGrpQuery).Tables[0];
                    string strWorkCode = dtResult.Rows[0][0].ToString();

                    //Check if WorkCode is R or rest day
                    if (strWorkCode.Equals("R"))
                    {
                        Value = "REST";
                    }

                    //Check for special WorkCode (e.g. D5, N5)
                    if (strWorkCode.Length > 1)
                    {
                        Value = "REG" + strWorkCode.Substring(1, 1);
                    }
                }
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isRestDay(string EmployeeID, DateTime ProcessDate)
        {
            string Value = "REG";
            try
            {
                string sqlQuery = string.Format(@"SELECT	TOP(1) Erd_RestDay
                                                    FROM	T_EmployeeRestDay	
                                                    WHERE	Erd_EmployeeID = '{0}'
                                                    AND		Erd_EffectivityDate <= '{1}'
                                                    ORDER BY Erd_EffectivityDate DESC", EmployeeID, ProcessDate);

                DataTable dtRestDay = dalHelper.ExecuteDataSet(sqlQuery).Tables[0];

                string Day = ProcessDate.ToString("ddd").ToUpper();
                string Rest = dtRestDay.Rows[0][0].ToString();

                if (Day == "MON" && Rest.Substring(0, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "TUE" && Rest.Substring(1, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "WED" && Rest.Substring(2, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "THU" && Rest.Substring(3, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "FRI" && Rest.Substring(4, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "SAT" && Rest.Substring(5, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "SUN" && Rest.Substring(6, 1).Equals("1"))
                    Value = "REST";
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isSpecialDay(string strWorkType, string strWorkGroup, DateTime ProcessDate, string strPayrollType)
        {
            string Value = "REG";
            try
            {
                string sqlSplDayQuery = string.Format(@"SELECT Ard_DayCode 
                                                          FROM T_SpecialDayMaster
                                                          WHERE Ard_WorkType = '{1}'
                                                          AND Ard_WorkGroup = '{2}'
                                                          AND Ard_Date = '{0}'
                                                          AND Ard_PayrollType = '{3}'", 
                                                          ProcessDate, strWorkType, strWorkGroup, strPayrollType);

                DataTable dtResult;
                dtResult = dalHelper.ExecuteDataSet(sqlSplDayQuery).Tables[0];
                Value = dtResult.Rows[0][0].ToString();
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isHoliday(string Location, DateTime ProcessDate)
        {
            string Value = "REG";
            try
            {
                string sqlQuery = string.Format(@"SELECT	Hmt_HolidayCode 
                                                    FROM	T_HolidayMaster 
                                                    WHERE	Hmt_HolidayDate = '{1}'
                                                    AND		(Hmt_ApplicCity = '{0}'
                                                    OR		 Hmt_ApplicCity = 'ALL')", Location, ProcessDate);

                DataTable dtHoliday = dalHelper.ExecuteDataSet(sqlQuery).Tables[0];

                Value = dtHoliday.Rows[0][0].ToString();
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }
        #endregion

        #region Refresh Default OT
        public void RefreshGroupDefaultOTCurPayPeriod(string PayPeriod, string EmployeeID, string ProcessDate, DALHelper dal) //also for future pay period
        {
            string sqlMain = "";
            string sqlOTCondition = "";
            string sqlLogLedgerCondition = "";
            string strEmpIdCondition = "";
            string strDateCondition = "";

            if (EmployeeID != "")
            {
                strEmpIdCondition = string.Format(" = '{0}' ", EmployeeID);
                sqlOTCondition += " AND Eot_EmployeeId = @EmployeeID ";
                sqlLogLedgerCondition += " AND Ell_EmployeeId = @EmployeeID ";
            }

            if (ProcessDate != "")
            {
                strDateCondition = string.Format(" = '{0}' ", ProcessDate);
                sqlOTCondition += " AND Eot_OvertimeDate = @OvertimeDate ";
                sqlLogLedgerCondition += " AND Ell_ProcessDate = @OvertimeDate ";
            }

            #region Main Query
            sqlMain = @"
---REFRESH DEFAULT OT CURRENT/FUTURE PAY PERIOD 

IF EXISTS (SELECT TOP 1 Gdo_WorkType
            FROM T_GroupDefaultOT)
BEGIN

DECLARE @PayPeriod varchar(7) = '{0}'
DECLARE @OvertimeFlag char(1) = (SELECT Ppm_CycleIndicator FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = @PayPeriod)
DECLARE @CurrentYear varchar(4) = (SELECT SUBSTRING(Ccd_CurrentYear, 3, 2) FROM T_CompanyMaster)
DECLARE @EmployeeID AS varchar(15) {1}
DECLARE @OvertimeDate AS datetime {2}
DECLARE @OvertimeHour AS decimal(5, 2)
DECLARE @ControlNo AS varchar(12)
DECLARE @OvertimeStart AS char(4)
DECLARE @OvertimeEnd AS char(4)
DECLARE @ShiftHours AS decimal(5, 2)
DECLARE @CostCenterCode AS varchar(12)
DECLARE @WorkCode AS char(2)
DECLARE @LastSeries AS int
DECLARE @AddMins AS smallint

------DELETE
UPDATE T_EmployeeLogLedger
SET Ell_EncodedOvertimePostHr = CASE WHEN (Ell_EncodedOvertimePostHr - Eot_OvertimeHour) < 0
							THEN 0
							ELSE (Ell_EncodedOvertimePostHr - Eot_OvertimeHour)
							END
	, ludatetime = getdate()
FROM T_EmployeeLogLedger
INNER JOIN (
SELECT Eot_EmployeeId, Eot_OvertimeDate, Eot_OvertimeHour, Eot_ControlNo
FROM T_EmployeeOvertime 
WHERE LEFT(Eot_ControlNo, 1) = 'D' 
  AND Eot_CurrentPayPeriod = @PayPeriod
  AND Eot_Status IN ('9', 'A')
  {3}
) TEMP
ON Ell_EmployeeId = Eot_EmployeeId
	AND Ell_ProcessDate = Eot_OvertimeDate

DELETE FROM T_EmployeeOvertime 
WHERE LEFT(Eot_ControlNo, 1) = 'D' 
  AND Eot_CurrentPayPeriod = @PayPeriod
  AND Eot_Status IN ('9', 'A')
  {3}

------INSERT
DECLARE cursorInsert CURSOR FOR 
---CALENDAR-BASED, REGULAR DAY
SELECT Ell_EmployeeId
    , Ell_ProcessDate
    , ISNULL(Gdo_OTHours, 0) AS Gdo_OTHours
    , Scm_ShiftTimeIn
    , Scm_ShiftTimeOut
    , Scm_ShiftHours
    , Emt_CostCenterCode
    , Cal_WorkCode
FROM T_EmployeeLogLedger
INNER JOIN T_GroupDefaultOT 
    ON Gdo_WorkType = Ell_Worktype
        AND Gdo_WorkGroup = Ell_WorkGroup
        AND Gdo_Shift = Ell_ShiftCode
		AND Gdo_DayCode = Ell_DayCode
		AND Gdo_Restday = Ell_RestDay
		AND Gdo_Holiday = Ell_Holiday
INNER JOIN T_ShiftCodeMaster 
    ON Scm_ShiftCode = Ell_ShiftCode
INNER JOIN T_EmployeeMaster 
    ON Emt_EmployeeId = Ell_EmployeeId
INNER JOIN T_CalendarGroupTmp 
    ON Cal_WorkType = Ell_WorkType 
        AND Cal_WorkGroup = Ell_WorkGroup 
        AND Cal_ProcessDate = Ell_ProcessDate
WHERE Ell_PayPeriod = @PayPeriod
    AND Gdo_Status = 'A'
    AND Gdo_OTHours > 0
    AND Ell_RestDay = 0
    AND Ell_Holiday = 0
    AND Ell_Worktype != 'REG'
    {4}
UNION
---NON-CALENDAR-BASED, REGULAR DAY
SELECT Ell_EmployeeId
    , Ell_ProcessDate
    , ISNULL(Gdo_OTHours, 0) AS Gdo_OTHours
    , Scm_ShiftTimeIn
    , Scm_ShiftTimeOut
    , Scm_ShiftHours
    , Emt_CostCenterCode
    , 'D' as Cal_WorkCode --temp
FROM T_EmployeeLogLedger
INNER JOIN T_GroupDefaultOT 
    ON Gdo_WorkType = Ell_Worktype
        AND Gdo_WorkGroup = Ell_WorkGroup
        AND Gdo_Shift = Ell_ShiftCode
		AND Gdo_DayCode = Ell_DayCode
		AND Gdo_Restday = Ell_RestDay
		AND Gdo_Holiday = Ell_Holiday
INNER JOIN T_ShiftCodeMaster 
    ON Scm_ShiftCode = Ell_ShiftCode
INNER JOIN T_EmployeeMaster 
    ON Emt_EmployeeId = Ell_EmployeeId
WHERE Ell_PayPeriod = @PayPeriod
    AND Gdo_Status = 'A'
    AND Gdo_OTHours > 0
    AND Ell_RestDay = 0
    AND Ell_Holiday = 0
    AND Ell_Worktype = 'REG'
    {4}
UNION
---CALENDAR-BASED AND NON-CALENDAR-BASED, RESTDAY OR HOLIDAY
SELECT Ell_EmployeeId
    , Ell_ProcessDate
    , ISNULL(Gdo_OTHours, 0) AS Gdo_OTHours
    , Scm_ShiftTimeIn
    , Scm_ShiftTimeOut
    , Scm_ShiftHours
    , Emt_CostCenterCode
    , 'D8' as Cal_WorkCode --temp
FROM T_EmployeeLogLedger
INNER JOIN T_GroupDefaultOT 
    ON Gdo_WorkType = Ell_Worktype
        AND Gdo_WorkGroup = Ell_WorkGroup
        AND Gdo_Shift = Ell_ShiftCode
		AND Gdo_DayCode = Ell_DayCode
		AND Gdo_Restday = Ell_RestDay
		AND Gdo_Holiday = Ell_Holiday
INNER JOIN T_ShiftCodeMaster 
    ON Scm_ShiftCode = Ell_ShiftCode
INNER JOIN T_EmployeeMaster 
    ON Emt_EmployeeId = Ell_EmployeeId
WHERE Ell_PayPeriod = @PayPeriod
	AND Gdo_Status = 'A'
    AND Gdo_OTHours > 0
    AND (Ell_Holiday = 1 
        OR Ell_RestDay = 1)
    {4}
    
--GET LAST TRANSACTION SERIES
SET @LastSeries = (SELECT Tcm_LastSeries 
					FROM T_TransactionControlMaster
					WITH (UPDLOCK)
					WHERE Tcm_TransactionCode = 'DFOVERTIME')

OPEN cursorInsert
FETCH NEXT FROM cursorInsert INTO @EmployeeID
								, @OvertimeDate
								, @OvertimeHour
								, @OvertimeStart
								, @OvertimeEnd
								, @ShiftHours
								, @CostCenterCode
								, @WorkCode

WHILE @@FETCH_STATUS = 0 
BEGIN 
	SET @AddMins = @OvertimeHour * 60
	IF LEN(@WorkCode) >= 2
	BEGIN 
		SET @OvertimeEnd = @OvertimeStart 
	END
	ELSE
	BEGIN 
		SET @OvertimeStart = @OvertimeEnd
	END
    SET @OvertimeEnd = (SELECT DBO.addMinutes(@OvertimeEnd, @AddMins))

	IF NOT EXISTS (SELECT 1 
					FROM T_EmployeeOvertime 
					WHERE Eot_EmployeeId = @EmployeeID
						AND Eot_OvertimeDate = @OvertimeDate
						AND ((@OvertimeStart > Eot_StartTime AND @OvertimeStart < Eot_EndTime)
							OR (@OvertimeEnd > Eot_StartTime AND @OvertimeEnd < Eot_EndTime)))
	BEGIN 
		SET @LastSeries = @LastSeries + 1
		
		INSERT INTO T_EmployeeOvertime
			( Eot_CurrentPayPeriod
			, Eot_EmployeeId
			, Eot_OvertimeDate
			, Eot_Seqno
			, Eot_AppliedDate
			, Eot_OvertimeType
			, Eot_StartTime
			, Eot_EndTime
			, Eot_OvertimeHour
			, Eot_Reason
			, Eot_JobCode
			, Eot_ClientJobNo
			, Eot_EndorsedDateToChecker
			, Eot_CheckedBy
			, Eot_CheckedDate
			, Eot_Checked2By
			, Eot_Checked2Date
			, Eot_ApprovedBy
			, Eot_ApprovedDate
			, Eot_Status
			, Eot_ControlNo
			, Eot_OvertimeFlag
			, Eot_Costcenter
			, Eot_Filler1
			, Eot_Filler2
			, Eot_Filler3
			, Eot_BatchNo
			, Usr_Login
			, Ludatetime )
		  VALUES( @PayPeriod
			, @EmployeeID
			, @OvertimeDate
			, '01'
			, Getdate()
			, 'P'
			, @OvertimeStart
			, @OvertimeEnd
			, @OvertimeHour
			, 'DEFAULT OT'
			, ''
			, ''
			, Getdate()
			, 'sa'
			, Getdate()
			, 'sa'
			, Getdate()
			, 'sa'
			, Getdate()
			, '9'
			, 'D' + @CurrentYear + RIGHT(REPLICATE('0', 9) + CAST(@LastSeries AS VARCHAR(9)), 9)
			, @OvertimeFlag
			, @CostCenterCode
			, ''
			, ''
			, ''
			, ''
			, 'sa'
			, Getdate() )
	                                                                            
		UPDATE T_EmployeeLogLedger
		SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr + @OvertimeHour
			, ludatetime = getdate()
		WHERE Ell_EmployeeId = @EmployeeID
			AND Ell_ProcessDate = @OvertimeDate
	END 
	
	FETCH NEXT FROM cursorInsert INTO @EmployeeID
									, @OvertimeDate
									, @OvertimeHour
									, @OvertimeStart
									, @OvertimeEnd
									, @ShiftHours
									, @CostCenterCode
									, @WorkCode
                      
END

CLOSE cursorInsert 
DEALLOCATE cursorInsert

--UPDATE LAST TRANSACTION SERIES
UPDATE T_TransactionControlMaster
SET Tcm_LastSeries = @LastSeries
	, ludatetime = getdate()
WHERE Tcm_TransactionCode = 'DFOVERTIME'

END
";
            #endregion

            sqlMain = string.Format(sqlMain, PayPeriod, strEmpIdCondition, strDateCondition, sqlOTCondition, sqlLogLedgerCondition);
            dal.ExecuteNonQuery(sqlMain);
        }

        public void RefreshGroupDefaultOTPastPayPeriod(string PayPeriod, string EmployeeID, string ProcessDate, DALHelper dal)
        {
            string sqlMain = "";
            string sqlOTCondition = "";
            string sqlLogLedgerCondition = "";
            string strEmpIdCondition = "";
            string strDateCondition = "";

            if (EmployeeID != "")
            {
                strEmpIdCondition = string.Format(" = '{0}' ", EmployeeID);
                sqlOTCondition += " AND Eot_EmployeeId = @EmployeeID ";
                sqlLogLedgerCondition += " AND Ell_EmployeeId = @EmployeeID ";
            }

            if (ProcessDate != "")
            {
                strDateCondition = string.Format(" = '{0}' ", ProcessDate);
                sqlOTCondition += " AND Eot_OvertimeDate = @OvertimeDate ";
                sqlLogLedgerCondition += " AND Ell_ProcessDate = @OvertimeDate ";
            }

            #region Main Query
            sqlMain = @"
---REFRESH DEFAULT OT PREVIOUS PAY PERIOD 

IF EXISTS (SELECT TOP 1 Gdo_WorkType
            FROM T_GroupDefaultOT)
BEGIN

DECLARE @PayPeriod varchar(7) = '{0}'
DECLARE @OvertimeFlag char(1) = (SELECT Ppm_CycleIndicator FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = @PayPeriod)
DECLARE @CurrentYear varchar(4) = (SELECT SUBSTRING(Ccd_CurrentYear, 3, 2) FROM T_CompanyMaster)
DECLARE @EmployeeID AS varchar(15) {1}
DECLARE @OvertimeDate AS datetime {2}
DECLARE @OvertimeHour AS decimal(5, 2)
DECLARE @ControlNo AS varchar(12)
DECLARE @OvertimeStart AS char(4)
DECLARE @OvertimeEnd AS char(4)
DECLARE @ShiftHours AS decimal(5, 2)
DECLARE @CostCenterCode AS varchar(12)
DECLARE @WorkCode AS char(2)
DECLARE @LastSeries AS int
DECLARE @AddMins AS smallint

------DELETE
UPDATE T_EmployeeLogLedgerHist
SET Ell_EncodedOvertimePostHr = CASE WHEN (Ell_EncodedOvertimePostHr - Eot_OvertimeHour) < 0
							THEN 0
							ELSE (Ell_EncodedOvertimePostHr - Eot_OvertimeHour)
							END
	, ludatetime = getdate()
FROM T_EmployeeLogLedgerHist
INNER JOIN (
SELECT Eot_EmployeeId, Eot_OvertimeDate, Eot_OvertimeHour, Eot_ControlNo
FROM T_EmployeeOvertimeHist 
WHERE LEFT(Eot_ControlNo, 1) = 'D' 
  AND Eot_CurrentPayPeriod = @PayPeriod
  AND Eot_Status IN ('9', 'A')
  {3}
) TEMP
ON Ell_EmployeeId = Eot_EmployeeId
	AND Ell_ProcessDate = Eot_OvertimeDate

UPDATE T_EmployeeOvertimeHist
SET Eot_Reason = Eot_Reason + ' (CANCELLED)'
	, Eot_Status = '2' 
	, ludatetime = getdate()   
WHERE LEFT(Eot_ControlNo, 1) = 'D' 
  AND Eot_CurrentPayPeriod = @PayPeriod
  AND Eot_Status IN ('9', 'A')
  {3}

------INSERT
DECLARE cursorInsert CURSOR FOR 
---CALENDAR-BASED, REGULAR DAY
SELECT Ell_EmployeeId
    , Ell_ProcessDate
    , ISNULL(Gdo_OTHours, 0) AS Gdo_OTHours
    , Scm_ShiftTimeIn
    , Scm_ShiftTimeOut
    , Scm_ShiftHours
    , Emt_CostCenterCode
    , Cal_WorkCode
FROM T_EmployeeLogLedgerHist
INNER JOIN T_GroupDefaultOT 
    ON Gdo_WorkType = Ell_Worktype
        AND Gdo_WorkGroup = Ell_WorkGroup
        AND Gdo_Shift = Ell_ShiftCode
		AND Gdo_DayCode = Ell_DayCode
		AND Gdo_Restday = Ell_RestDay
		AND Gdo_Holiday = Ell_Holiday
INNER JOIN T_ShiftCodeMaster 
    ON Scm_ShiftCode = Ell_ShiftCode
INNER JOIN T_EmployeeMaster 
    ON Emt_EmployeeId = Ell_EmployeeId
INNER JOIN T_CalendarGroupTmp 
    ON Cal_WorkType = Ell_WorkType 
        AND Cal_WorkGroup = Ell_WorkGroup 
        AND Cal_ProcessDate = Ell_ProcessDate
WHERE Ell_PayPeriod = @PayPeriod
    AND Gdo_Status = 'A'
    AND Gdo_OTHours > 0
    AND Ell_RestDay = 0
    AND Ell_Holiday = 0
    AND Ell_Worktype != 'REG'
    {4}
UNION
---NON-CALENDAR-BASED, REGULAR DAY
SELECT Ell_EmployeeId
    , Ell_ProcessDate
    , ISNULL(Gdo_OTHours, 0) AS Gdo_OTHours
    , Scm_ShiftTimeIn
    , Scm_ShiftTimeOut
    , Scm_ShiftHours
    , Emt_CostCenterCode
    , 'D' as Cal_WorkCode --temp
FROM T_EmployeeLogLedgerHist
INNER JOIN T_GroupDefaultOT 
    ON Gdo_WorkType = Ell_Worktype
        AND Gdo_WorkGroup = Ell_WorkGroup
        AND Gdo_Shift = Ell_ShiftCode
		AND Gdo_DayCode = Ell_DayCode
		AND Gdo_Restday = Ell_RestDay
		AND Gdo_Holiday = Ell_Holiday
INNER JOIN T_ShiftCodeMaster 
    ON Scm_ShiftCode = Ell_ShiftCode
INNER JOIN T_EmployeeMaster 
    ON Emt_EmployeeId = Ell_EmployeeId
WHERE Ell_PayPeriod = @PayPeriod
    AND Gdo_Status = 'A'
    AND Gdo_OTHours > 0
    AND Ell_RestDay = 0
    AND Ell_Holiday = 0
    AND Ell_Worktype = 'REG'
    {4}
UNION
---CALENDAR-BASED AND NON-CALENDAR-BASED, RESTDAY OR HOLIDAY
SELECT Ell_EmployeeId
    , Ell_ProcessDate
    , ISNULL(Gdo_OTHours, 0) AS Gdo_OTHours
    , Scm_ShiftTimeIn
    , Scm_ShiftTimeOut
    , Scm_ShiftHours
    , Emt_CostCenterCode
    , 'D8' as Cal_WorkCode --temp
FROM T_EmployeeLogLedgerHist
INNER JOIN T_GroupDefaultOT 
    ON Gdo_WorkType = Ell_Worktype
        AND Gdo_WorkGroup = Ell_WorkGroup
        AND Gdo_Shift = Ell_ShiftCode
		AND Gdo_DayCode = Ell_DayCode
		AND Gdo_Restday = Ell_RestDay
		AND Gdo_Holiday = Ell_Holiday
INNER JOIN T_ShiftCodeMaster 
    ON Scm_ShiftCode = Ell_ShiftCode
INNER JOIN T_EmployeeMaster 
    ON Emt_EmployeeId = Ell_EmployeeId
WHERE Ell_PayPeriod = @PayPeriod
	AND Gdo_Status = 'A'
    AND Gdo_OTHours > 0
    AND (Ell_Holiday = 1 
        OR Ell_RestDay = 1)
    {4}
    
--GET LAST TRANSACTION SERIES
SET @LastSeries = (SELECT Tcm_LastSeries 
					FROM T_TransactionControlMaster
					WITH (UPDLOCK)
					WHERE Tcm_TransactionCode = 'DFOVERTIME')

OPEN cursorInsert
FETCH NEXT FROM cursorInsert INTO @EmployeeID
								, @OvertimeDate
								, @OvertimeHour
								, @OvertimeStart
								, @OvertimeEnd
								, @ShiftHours
								, @CostCenterCode
								, @WorkCode

WHILE @@FETCH_STATUS = 0 
BEGIN 
	SET @AddMins = @OvertimeHour * 60
	IF LEN(@WorkCode) >= 2
	BEGIN 
		SET @OvertimeHour = @OvertimeHour + @ShiftHours 
	END
	ELSE
	BEGIN 
		SET @OvertimeStart = @OvertimeEnd
	END
	SET @OvertimeEnd = (SELECT DBO.addMinutes(@OvertimeEnd, @AddMins))

	IF NOT EXISTS (SELECT 1 
					FROM T_EmployeeOvertimeHist 
					WHERE Eot_EmployeeId = @EmployeeID
						AND Eot_OvertimeDate = @OvertimeDate
						AND ((@OvertimeStart > Eot_StartTime AND @OvertimeStart < Eot_EndTime)
							OR (@OvertimeEnd > Eot_StartTime AND @OvertimeEnd < Eot_EndTime)))
	BEGIN 
		SET @LastSeries = @LastSeries + 1
		
		INSERT INTO T_EmployeeOvertimeHist
			( Eot_CurrentPayPeriod
			, Eot_EmployeeId
			, Eot_OvertimeDate
			, Eot_Seqno
			, Eot_AppliedDate
			, Eot_OvertimeType
			, Eot_StartTime
			, Eot_EndTime
			, Eot_OvertimeHour
			, Eot_Reason
			, Eot_JobCode
			, Eot_ClientJobNo
			, Eot_EndorsedDateToChecker
			, Eot_CheckedBy
			, Eot_CheckedDate
			, Eot_Checked2By
			, Eot_Checked2Date
			, Eot_ApprovedBy
			, Eot_ApprovedDate
			, Eot_Status
			, Eot_ControlNo
			, Eot_OvertimeFlag
			, Eot_Costcenter
			, Eot_Filler1
			, Eot_Filler2
			, Eot_Filler3
			, Eot_BatchNo
			, Usr_Login
			, Ludatetime )
		  VALUES( @PayPeriod
			, @EmployeeID
			, @OvertimeDate
			, '01'
			, Getdate()
			, 'P'
			, @OvertimeStart
			, @OvertimeEnd
			, @OvertimeHour
			, 'DEFAULT OT'
			, ''
			, ''
			, Getdate()
			, 'sa'
			, Getdate()
			, 'sa'
			, Getdate()
			, 'sa'
			, Getdate()
			, '9'
			, 'D' + @CurrentYear + RIGHT(REPLICATE('0', 9) + CAST(@LastSeries AS VARCHAR(9)), 9)
			, @OvertimeFlag
			, @CostCenterCode
			, ''
			, ''
			, ''
			, ''
			, 'sa'
			, Getdate() )
	                                                                            
		UPDATE T_EmployeeLogLedgerHist
		SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr + @OvertimeHour
			, ludatetime = getdate()
		WHERE Ell_EmployeeId = @EmployeeID
			AND Ell_ProcessDate = @OvertimeDate
	END 
	
	FETCH NEXT FROM cursorInsert INTO @EmployeeID
									, @OvertimeDate
									, @OvertimeHour
									, @OvertimeStart
									, @OvertimeEnd
									, @ShiftHours
									, @CostCenterCode
									, @WorkCode
                      
END

CLOSE cursorInsert 
DEALLOCATE cursorInsert

--UPDATE LAST TRANSACTION SERIES
UPDATE T_TransactionControlMaster
SET Tcm_LastSeries = @LastSeries
	, ludatetime = getdate()
WHERE Tcm_TransactionCode = 'DFOVERTIME'

END
";
            #endregion

            sqlMain = string.Format(sqlMain, PayPeriod, strEmpIdCondition, strDateCondition, sqlOTCondition, sqlLogLedgerCondition);
            dal.ExecuteNonQuery(sqlMain);
        }
        #endregion

        #region Log Ledger Updating Footer
        public DataSet GetLastTrailWithRemarksPerDate(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Elt_ProcessDate
                                                , Elt_Seqno
                                                , CASE Elt_Restday
                                                        WHEN '1' THEN Elt_DayCode + '*'
                                                        ELSE Elt_DayCode
                                                END [Elt_DayCode]
                                                , Elt_ShiftCode
                                                , Elt_ActualTimeIn_1
                                                , Elt_ActualTimeOut_1
                                                , Elt_ActualTimeIn_2
                                                , Elt_ActualTimeOut_2
                                                , Elt_Remarks
                                                , T_EmployeeLogTrail.Usr_Login AS Usr_Login
                                                , T_EmployeeLogTrail.Ludatetime AS Ludatetime
                                                , Emt_LastName
                                                , Emt_FirstName
                                            FROM T_EmployeeLogTrail
                                            LEFT JOIN T_EmployeeMaster
                                            ON T_EmployeeLogTrail.Usr_Login = Emt_EmployeeID
                                            AND T_EmployeeLogTrail.Usr_Login != 'SA'
                                            WHERE Elt_EmployeeId = '{0}'
                                            AND Elt_ProcessDate >= '{1}' AND Elt_ProcessDate <= '{2}'
                                            ORDER BY Elt_ProcessDate, Elt_Seqno", EmployeeId, DateStart, DateEnd);
            DataSet dsLogTrail;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsLogTrail = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsLogTrail;
        }

        public DataSet GetOvertimeRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Eot_OvertimeDate, Eot_StartTime, Eot_EndTime, Eot_OvertimeHour 
                                            , CASE Eot_OvertimeType
                                                WHEN 'A' THEN 'ADVANCE'
                                                WHEN 'M' THEN 'MID'
                                                WHEN 'P' THEN 'POST'
                                                ELSE ''
                                                END AS Eot_OvertimeType
                                            , Eot_Reason
                                            , (SELECT Umt_Usercode + ' - ' + Umt_userfname + ' ' + Umt_userlname FROM T_UserMaster WHERE Umt_Usercode =  Eot_ApprovedBy) 'Approved By'
	                                        , CONVERT(varchar(10), Eot_ApprovedDate, 101) + ' ' + CONVERT(varchar(8), Eot_ApprovedDate, 114) AS 'Approved On'
                                            FROM T_EmployeeOvertime
                                            WHERE Eot_EmployeeId = '{0}'
                                            AND Eot_OvertimeDate >= '{1}' 
                                            AND Eot_OvertimeDate <= '{2}'
                                            AND Eot_Status = '9'

                                            UNION ALL

                                            SELECT Eot_OvertimeDate, Eot_StartTime, Eot_EndTime, Eot_OvertimeHour 
                                            , CASE Eot_OvertimeType
                                                WHEN 'A' THEN 'ADVANCE'
                                                WHEN 'M' THEN 'MID'
                                                WHEN 'P' THEN 'POST'
                                                ELSE ''
                                                END AS Eot_OvertimeType
                                            , Eot_Reason
                                            , (SELECT Umt_Usercode + ' - ' + Umt_userfname + ' ' + Umt_userlname FROM T_UserMaster WHERE Umt_Usercode =  Eot_ApprovedBy) 'Approved By'
	                                        , CONVERT(varchar(10), Eot_ApprovedDate, 101) + ' ' + CONVERT(varchar(8), Eot_ApprovedDate, 114) AS 'Approved On'
                                            FROM T_EmployeeOvertimeHist
                                            WHERE Eot_EmployeeId = '{0}'
                                            AND Eot_OvertimeDate >= '{1}' 
                                            AND Eot_OvertimeDate <= '{2}'
                                            AND Eot_Status = '9'

                                            ORDER BY Eot_OvertimeDate, Eot_StartTime", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetPendingOvertimeRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Eot_OvertimeDate, Eot_StartTime, Eot_EndTime, Eot_OvertimeHour 
                                            , CASE Eot_OvertimeType
                                                WHEN 'A' THEN 'ADVANCE'
                                                WHEN 'M' THEN 'MID'
                                                WHEN 'P' THEN 'POST'
                                                ELSE ''
                                                END AS Eot_OvertimeType
                                            , StatusDesc.Adt_AccountDesc AS Eot_Status
                                            , Eot_Reason
                                            FROM T_EmployeeOvertime
                                            LEFT JOIN T_AccountDetail StatusDesc
                                            ON StatusDesc.Adt_AccountCode = Eot_Status
                                            AND StatusDesc.Adt_AccountType = 'WFSTATUS'
                                            WHERE Eot_EmployeeId = '{0}'
                                            AND Eot_OvertimeDate >= '{1}' 
                                            AND Eot_OvertimeDate <= '{2}'
                                            AND Eot_Status != '9'

                                            UNION ALL

                                            SELECT Eot_OvertimeDate, Eot_StartTime, Eot_EndTime, Eot_OvertimeHour 
                                            , CASE Eot_OvertimeType
                                                WHEN 'A' THEN 'ADVANCE'
                                                WHEN 'M' THEN 'MID'
                                                WHEN 'P' THEN 'POST'
                                                ELSE ''
                                                END AS Eot_OvertimeType
                                            , StatusDesc.Adt_AccountDesc AS Eot_Status
                                            , Eot_Reason
                                            FROM T_EmployeeOvertimeHist
                                            LEFT JOIN T_AccountDetail StatusDesc
                                            ON StatusDesc.Adt_AccountCode = Eot_Status
                                            AND StatusDesc.Adt_AccountType = 'WFSTATUS'
                                            WHERE Eot_EmployeeId = '{0}'
                                            AND Eot_OvertimeDate >= '{1}' 
                                            AND Eot_OvertimeDate <= '{2}'
                                            AND Eot_Status != '9'

                                            ORDER BY Eot_OvertimeDate, Eot_StartTime", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetLeaveRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Elt_LeaveDate, Elt_LeaveType, Elt_StartTime, Elt_EndTime
                                            , Elt_LeaveHour
                                            , CASE Elt_DayUnit
                                                WHEN 'WH' THEN 'WHOLE DAY'
                                                WHEN 'HA' THEN 'HALF DAY AM'
                                                WHEN 'HP' THEN 'HALF DAY PM'
                                                ELSE ''
                                                END AS Elt_DayUnit
                                            , Elt_Reason
                                            , (SELECT Umt_Usercode + ' - ' + Umt_userfname + ' ' + Umt_userlname FROM T_UserMaster WHERE Umt_Usercode =  Elt_ApprovedBy) 'Approved By'
	                                        , CONVERT(varchar(10), Elt_ApprovedDate, 101) + ' ' + CONVERT(varchar(8), Elt_ApprovedDate, 114) AS 'Approved On'
                                            FROM T_EmployeeLeaveAvailment
                                            WHERE Elt_EmployeeId = '{0}'
                                            AND Elt_LeaveDate >= '{1}' 
                                            AND Elt_LeaveDate <= '{2}'
                                            AND Elt_Status IN ('9', '0')

                                            UNION ALL

                                            SELECT Elt_LeaveDate, Elt_LeaveType, Elt_StartTime, Elt_EndTime
                                            , Elt_LeaveHour
                                            , CASE Elt_DayUnit
                                                WHEN 'WH' THEN 'WHOLE DAY'
                                                WHEN 'HA' THEN 'HALF DAY AM'
                                                WHEN 'HP' THEN 'HALF DAY PM'
                                                ELSE ''
                                                END AS Elt_DayUnit
                                            , Elt_Reason
                                            , (SELECT Umt_Usercode + ' - ' + Umt_userfname + ' ' + Umt_userlname FROM T_UserMaster WHERE Umt_Usercode =  Elt_ApprovedBy) 'Approved By'
	                                        , CONVERT(varchar(10), Elt_ApprovedDate, 101) + ' ' + CONVERT(varchar(8), Elt_ApprovedDate, 114) AS 'Approved On'
                                            FROM T_EmployeeLeaveAvailmentHist
                                            WHERE Elt_EmployeeId = '{0}'
                                            AND Elt_LeaveDate >= '{1}' 
                                            AND Elt_LeaveDate <= '{2}'
                                            AND Elt_Status IN ('9', '0')

                                            ORDER BY Elt_LeaveDate, Elt_StartTime", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetPendingLeaveRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Elt_LeaveDate, Elt_LeaveType, Elt_StartTime, Elt_EndTime
                                            , Elt_LeaveHour
                                            , CASE Elt_DayUnit
                                                WHEN 'WH' THEN 'WHOLE DAY'
                                                WHEN 'HA' THEN 'HALF DAY AM'
                                                WHEN 'HP' THEN 'HALF DAY PM'
                                                ELSE ''
                                                END AS Elt_DayUnit
                                            , StatusDesc.Adt_AccountDesc AS Elt_Status
                                            , Elt_Reason
                                            FROM T_EmployeeLeaveAvailment
                                            LEFT JOIN T_AccountDetail StatusDesc
                                            ON StatusDesc.Adt_AccountCode = Elt_Status
                                            AND StatusDesc.Adt_AccountType = 'WFSTATUS'
                                            WHERE Elt_EmployeeId = '{0}'
                                            AND Elt_LeaveDate >= '{1}' 
                                            AND Elt_LeaveDate <= '{2}'
                                            AND Elt_Status NOT IN ('9', '0')

                                            UNION ALL

                                            SELECT Elt_LeaveDate, Elt_LeaveType, Elt_StartTime, Elt_EndTime
                                            , Elt_LeaveHour
                                            , CASE Elt_DayUnit
                                                WHEN 'WH' THEN 'WHOLE DAY'
                                                WHEN 'HA' THEN 'HALF DAY AM'
                                                WHEN 'HP' THEN 'HALF DAY PM'
                                                ELSE ''
                                                END AS Elt_DayUnit
                                            , StatusDesc.Adt_AccountDesc AS Elt_Status
                                            , Elt_Reason
                                            FROM T_EmployeeLeaveAvailmentHist
                                            LEFT JOIN T_AccountDetail StatusDesc
                                            ON StatusDesc.Adt_AccountCode = Elt_Status
                                            AND StatusDesc.Adt_AccountType = 'WFSTATUS'
                                            WHERE Elt_EmployeeId = '{0}'
                                            AND Elt_LeaveDate >= '{1}' 
                                            AND Elt_LeaveDate <= '{2}'
                                            AND Elt_Status NOT IN ('9', '0')

                                            ORDER BY Elt_LeaveDate, Elt_StartTime", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetTimeRecModificationRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Trm_ModDate
                                              ,Trm_ActualTimeIn1
                                              ,Trm_ActualTimeOut1
                                              ,Trm_ActualTimeIn2
                                              ,Trm_ActualTimeOut2
                                              ,Trm_Reason
                                              ,Trm_Status
                                              , (SELECT Umt_Usercode + ' - ' + Umt_userfname + ' ' + Umt_userlname FROM T_UserMaster WHERE Umt_Usercode =  Trm_ApprovedBy) 'Approved By'
	                                          , CONVERT(varchar(10), Trm_ApprovedDate, 101) + ' ' + CONVERT(varchar(8), Trm_ApprovedDate, 114) AS 'Approved On'
                                          FROM T_TimeRecMod
                                          WHERE Trm_EmployeeId = '{0}'
	                                        AND Trm_ModDate >= '{1}' 
	                                        AND Trm_ModDate <= '{2}'
	                                        AND Trm_Status IN ('9')

                                            ORDER BY Trm_ModDate, Trm_ActualTimeIn1", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetPendingTimeRecModificationRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Trm_ModDate
                                              ,Trm_ActualTimeIn1
                                              ,Trm_ActualTimeOut1
                                              ,Trm_ActualTimeIn2
                                              ,Trm_ActualTimeOut2
                                              ,Trm_Reason
                                              ,StatusDesc.Adt_AccountDesc AS Trm_Status
                                          FROM T_TimeRecMod
                                          LEFT JOIN T_AccountDetail StatusDesc
                                            ON StatusDesc.Adt_AccountCode = Trm_Status
                                            AND StatusDesc.Adt_AccountType = 'WFSTATUS'
                                          WHERE Trm_EmployeeId = '{0}'
	                                        AND Trm_ModDate >= '{1}' 
	                                        AND Trm_ModDate <= '{2}'
	                                        AND Trm_Status NOT IN ('9')

                                            ORDER BY Trm_ModDate, Trm_ActualTimeIn1", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetMovementRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Mve_EffectivityDate
	                                             , CASE (Mve_Type)
	                                                 WHEN 'R' THEN 'RESTDAY'
	                                                 WHEN 'C' THEN 'COST CENTER'
	                                                 WHEN 'G' THEN 'WORKGROUP'
	                                                 WHEN 'S' THEN 'SHIFT'
	                                                 END AS Mve_Type
	                                             , Mve_From
	                                             , Mve_To
	                                             , Mve_Reason
                                                 , (SELECT Umt_Usercode + ' - ' + Umt_userfname + ' ' + Umt_userlname FROM T_UserMaster WHERE Umt_Usercode =  Mve_ApprovedBy) 'Approved By'
	                                             , CONVERT(varchar(10), Mve_ApprovedDate, 101) + ' ' + CONVERT(varchar(8), Mve_ApprovedDate, 114) AS 'Approved On'
                                            FROM T_Movement
                                            WHERE Mve_EmployeeId = '{0}'
                                            AND Mve_EffectivityDate >= '{1}' 
                                            AND Mve_EffectivityDate <= '{2}'
                                            AND Mve_Status IN ('9', '0')

                                            ORDER BY Mve_EffectivityDate", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetPendingMovementRecordsPerEmployeePerRange(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Mve_EffectivityDate
	                                             , CASE (Mve_Type)
	                                                 WHEN 'R' THEN 'RESTDAY'
	                                                 WHEN 'C' THEN 'COST CENTER'
	                                                 WHEN 'G' THEN 'WORKGROUP'
	                                                 WHEN 'S' THEN 'SHIFT'
	                                                 END AS Mve_Type
	                                             , Mve_From
	                                             , Mve_To
                                                 , StatusDesc.Adt_AccountDesc AS Mve_Status
	                                             , Mve_Reason
                                            FROM T_Movement
                                            LEFT JOIN T_AccountDetail StatusDesc
                                            ON StatusDesc.Adt_AccountCode = Mve_Status
                                            AND StatusDesc.Adt_AccountType = 'WFSTATUS'
                                            WHERE Mve_EmployeeId = '{0}'
                                            AND Mve_EffectivityDate >= '{1}' 
                                            AND Mve_EffectivityDate <= '{2}'
                                            AND Mve_Status NOT IN ('9', '0')

                                            ORDER BY Mve_EffectivityDate", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }
        #endregion

        #region Parameters and Process Flags
        public bool AllowCycleProcessing(string MenuCode)
        {
            //if (MenuCode.Equals(StringEnum.GetStringValue(CommonEnum.MenuCode.CYCLECLOSING)) && GetProcessFlag("PAYROLL", "CYCLEOPEN") && GetProcessFlag("PAYROLL", "DBOPNBCKUP") && !GetProcessFlag("PAYROLL", "CYCLECLOSE") && GetProcessFlag("PAYROLL", "DBCLSBCKUP") && GetProcessFlag("OVERTIME", "CUT-OFF") && GetProcessFlag("TIMEKEEP", "CUT-OFF") && GetProcessFlag("LEAVE", "CUT-OFF") && GetProcessFlag("PAYROLL", "CUT-OFF") && GetProcessFlag("PAYROLL", "PAYCALC") && GetProcessFlag("PAYROLL", "ATMDISKGEN"))
            //    return true;
            //else if (MenuCode.Equals(StringEnum.GetStringValue(CommonEnum.MenuCode.CYCLEOPENING)) && !GetProcessFlag("PAYROLL", "CYCLEOPEN") && GetProcessFlag("PAYROLL", "DBOPNBCKUP") && GetProcessFlag("PAYROLL", "CYCLECLOSE") && GetProcessFlag("PAYROLL", "DBCLSBCKUP") && GetProcessFlag("OVERTIME", "CUT-OFF") && GetProcessFlag("TIMEKEEP", "CUT-OFF") && GetProcessFlag("LEAVE", "CUT-OFF") && GetProcessFlag("PAYROLL", "CUT-OFF") && GetProcessFlag("PAYROLL", "PAYCALC") && GetProcessFlag("PAYROLL", "ATMDISKGEN"))
            //    return true;
            //else if (MenuCode.Equals(StringEnum.GetStringValue(CommonEnum.MenuCode.YEARLYHOUSEKEEP)) && GetProcessFlag("OVERTIME", "CUT-OFF") && GetProcessFlag("TIMEKEEP", "CUT-OFF") && GetProcessFlag("LEAVE", "CUT-OFF") && GetProcessFlag("PAYROLL", "CUT-OFF") && GetProcessFlag("PAYROLL", "DBYRBCKUP") && !GetProcessFlag("PAYROLL", "YEAREND") && !GetProcessFlag("PAYROLL", "CYCLEOPEN") && GetProcessFlag("PAYROLL", "CYCLECLOSE"))
            //    return true;
            //else if (MenuCode.Equals(StringEnum.GetStringValue(CommonEnum.MenuCode.LABHRGENERATION)) && GetProcessFlag("OVERTIME", "CUT-OFF") && GetProcessFlag("TIMEKEEP", "CUT-OFF") && GetProcessFlag("LEAVE", "CUT-OFF") && GetProcessFlag("PAYROLL", "CYCLEOPEN") && !GetProcessFlag("PAYROLL", "CYCLECLOSE"))
            //    return true;
            //else
                return false;
        }

        public decimal GetParameterValue(string Parameter)
        {
            string sqlQuery = @"SELECT Pmt_NumericValue FROM T_ParameterMaster WHERE Pmt_ParameterID = @Pmt_ParameterID";

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Pmt_ParameterID", Parameter);

            try
            {
                return Convert.ToDecimal(dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0].Rows[0][0]);
            }
            catch
            {
                throw new PayrollException(string.Format("Parameter ID [{0}] not found in Parameter Master!", Parameter));
            }
        }

        public bool CheckIfParameterExists(string strParam)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Param", strParam);

            string strQuery = @"SELECT Pmt_NumericValue 
                                FROM T_ParameterMaster
                                WHERE Pmt_ParameterID = @Param AND Pmt_status ='A'";

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery, CommandType.Text, paramInfo).Tables[0];

            if (dtResult.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetProcessFlag(string SystemID, string ProcessID)
        {
            string sqlQuery = @"SELECT	Pcm_ProcessFlag 
                                FROM	T_ProcessControlMaster 
                                WHERE	Pcm_SystemID = @Pcm_SystemID
                                AND		Pcm_ProcessID = @Pcm_ProcessID";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Pcm_SystemID", SystemID);
            paramInfo[1] = new ParameterInfo("@Pcm_ProcessID", ProcessID);

            try
            {
                return Convert.ToBoolean(dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0].Rows[0][0]);
            }
            catch
            {
                throw new PayrollException(string.Format("System ID [{0}] and Process ID [{1}] combination not found in Process Control Master!", SystemID, ProcessID));
            }
        }

        public int UpdateProcessControlFlag(string Pcm_ProcessFlag, string Pcm_SystemID, string Pcm_ProcessID, string user)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region update query
            string Upstring = @"Update T_ProcessControlMaster
                                    Set Pcm_ProcessFlag = @Pcm_ProcessFlag
                                        ,Usr_Login = @Usr_Login
                                        ,Ludatetime = GetDate()
                                    Where Pcm_SystemID = @Pcm_SystemID
                                    And Pcm_ProcessID = @Pcm_ProcessID

                                Insert Into [T_ProcessControlTrail]
                                SELECT Stm_SystemID    = @Pcm_SystemID
                                     , Stm_ProcessID   = @Pcm_ProcessID
                                     , Stm_ProcessFlag = @Pcm_ProcessFlag
                                     , Usr_Login       = @Usr_Login
                                     , Ludatetime      = getdate()";
            #endregion

            ParameterInfo[] UpparamInfo = new ParameterInfo[4];
            UpparamInfo[0] = new ParameterInfo("@Pcm_ProcessFlag", Pcm_ProcessFlag);
            UpparamInfo[1] = new ParameterInfo("@Pcm_SystemID", Pcm_SystemID);
            UpparamInfo[2] = new ParameterInfo("@Pcm_ProcessID", Pcm_ProcessID);
            UpparamInfo[3] = new ParameterInfo("@Usr_Login", user);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(Upstring, CommandType.Text, UpparamInfo);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException("Error: " + e.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public bool GetTransactionFlag(string strTransCode)
        {
            string sqlQuery = @"SELECT	Tcm_TransactionCode 
                                FROM	T_TransactionControlMaster 
                                WHERE	Tcm_TransactionCode = @TransCode
                                AND		Tcm_Status = 'A'";

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@TransCode", strTransCode);

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];

            if (dtResult.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Time-based Allowance Computation
        public void GenerateOtherAllowances(string EmployeeID, bool currentflag)
        {
            GenerateOtherAllowances(EmployeeID, currentflag, string.Empty);
        }

        public void GenerateOtherAllowances(string EmployeeID, bool currentflag, string forClearanceComputation)
        {
            GenerateOtherAllowances(false, EmployeeID, "", currentflag, forClearanceComputation);
        }

        public void GenerateOtherAllowances(bool ProcessAll, string EmployeeId, string EmployeeList, bool currentflag, string forClearanceComputation)
        {
            #region Variables
            string sqlUpdateHeader = string.Empty;
            string strStatusExclusion = "";
            string strPayTypeExclusion = "";
            string strJobLevelExclusion = "";
            string strAllowanceColNo = "00";
            string strDispatchedExclusion = "FALSE";
            string unit = string.Empty;
            string delimeter = ",";
            string[] rows;
            decimal RGOTOFFSET = 0;

            DataTable dtAllowanceDetails, dtLogLedger, dtOvertime, dtAllowanceAmt;
            DataRow[] drArrOvertime;
            bool bCheckOT;
            bool bFound;
            int iStartOT, iEndOT, iLogOut;
            #endregion

            #region Initialize Conditions
            string strForHist = string.Empty;
            string strEmpMasterHistCondition = string.Empty;
            string strClearAllowanceCondition = string.Empty;
            string strHourBasedAlwCondition = string.Empty;
            string strJobStatusCondition = string.Empty;
            string strSpecialAlwCondition = string.Empty;
            string strMonthlyAlwCondition = string.Empty;

            if (!currentflag)
            {
                strForHist = "Hist";
                strEmpMasterHistCondition = "AND Emt_PayPeriod = Ell_PayPeriod";
            }

            if (!ProcessAll && EmployeeId != "")
            {
                strClearAllowanceCondition = " AND Ell_EmployeeID = '" + EmployeeId + "'";
                strHourBasedAlwCondition = " AND Ell_EmployeeID = '" + EmployeeId + "'";
                strSpecialAlwCondition = " AND Esa_EmployeeID = '" + EmployeeId + "'";
                strMonthlyAlwCondition = " AND a.Ell_EmployeeID = '" + EmployeeId + "'";
            }
            else if (ProcessAll == true && EmployeeList != "")
            {
                strClearAllowanceCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ")";
                strHourBasedAlwCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ")";
                strSpecialAlwCondition = " AND Esa_EmployeeID IN (" + EmployeeList + ")";
                strMonthlyAlwCondition = " AND a.Ell_EmployeeID IN (" + EmployeeList + ")";
            }
            else
            {
                strClearAllowanceCondition = "";
                strHourBasedAlwCondition = "";
                strSpecialAlwCondition = "";
                strMonthlyAlwCondition = "";
            }

            if (forClearanceComputation == "")
                strJobStatusCondition = " --AND LEFT([Emt_JobStatus],1) = 'A'";
            #endregion

            #region Query for Allowance Initialization
            string strClearAllowance = @"UPDATE [T_EmployeeLogLedger{0}]
                                            SET [Ell_AllowanceAmt{1}] = 0
                                        WHERE [Ell_PayPeriod] = '{2}'
                                            {3}";
            #endregion

            #region Query for Hour-based
            string sqlUpdateRegOT = @" UPDATE [T_EmployeeLogLedger{6}]
                                       SET [Ell_AllowanceAmt{0}] = ISNULL(dbo.GetAllowanceAmountHourBased(
								                                       Ell_ProcessDate
								                                       , Ell_EmployeeId
								                                       , '{0}'
								                                       , (CASE WHEN Ell_DayCode = 'REG' 
											                                    THEN(CASE Alh_PostingType
													                                    WHEN 'R'
													                                    THEN Ell_RegularHour
													                                    WHEN 'O' 
													                                    THEN CASE WHEN {5} > 0 and (Ell_ShiftMin/60.00) = 8.00 THEN
																	                                    Ell_OvertimeHour - {5}
															                                       ELSE
																	                                    Ell_OvertimeHour
															                                       END
													                                    WHEN 'L'
													                                    THEN Ell_LeaveHour
													                                    WHEN 'A'
													                                    THEN Ell_RegularHour + Ell_OvertimeHour
													                                    WHEN 'B'
													                                    THEN Ell_RegularHour + Ell_LeaveHour
													                                    WHEN 'D'
													                                    THEN CASE WHEN Ell_AdjustShiftMin > Ell_ComputedOvertimeMin - Ell_AdjustShiftMin
                                                                                             THEN (Ell_AdjustShiftMin/60.00)
                                                                                             ELSE ((Ell_ComputedOvertimeMin - Ell_AdjustShiftMin)/60.0)
                                                                                             END
													                                    WHEN 'H'
													                                    THEN Ell_RegularHour + Ell_LeaveHour
													                                    ELSE 0
												                                     END)
											                                    ELSE (CASE [Alh_PostingType]
													                                    WHEN 'O' 
													                                    THEN Ell_RegularHour + Ell_OvertimeHour
													                                    WHEN 'L'
													                                    THEN Ell_LeaveHour
													                                    WHEN 'A'
													                                    THEN Ell_RegularHour + Ell_OvertimeHour
													                                    WHEN 'B'
													                                    THEN Ell_RegularHour + Ell_OvertimeHour + Ell_LeaveHour
													                                    WHEN 'D'
													                                    THEN Ell_RegularHour + CASE WHEN Ell_AdjustShiftMin > Ell_ComputedOvertimeMin - Ell_AdjustShiftMin
                                                                                                                 THEN (Ell_AdjustShiftMin/60.00)
                                                                                                                 ELSE ((Ell_ComputedOvertimeMin - Ell_AdjustShiftMin)/60.0)
                                                                                                                 END
													                                    WHEN 'H'
													                                    THEN CASE WHEN Ell_DayCode = 'HOL' THEN Ell_ForceLeave ELSE 0 END
													                                    ELSE 0
												                                     END) 
										                                      END)
								                                       , Ell_DayCode
                                                                       , Alh_ExtensionTable
                                                                       , Alh_MinAmount
                                                                       , Alh_MaxAmount),0)
                                          FROM [T_EmployeeLogLedger{6}]
                                          JOIN [T_EmployeeMaster{6}]
	                                        ON [Emt_EmployeeID] = [Ell_EmployeeId]     
                                            {8}                                      
                                            {9}
                                          JOIN [T_AllowanceHeader]
	                                        ON [Alh_LedgerAlwCol] = '{0}'
                                         WHERE [Ell_PayPeriod] = '{1}'
                                           {4}
                                           AND (CASE WHEN Left(Ell_Locationcode,1) = 'D' THEN 'TRUE' ELSE 'NONE' END) <> '{3}'
                                           {2} 
                                           {7} ";
            #endregion

            #region Query for Time-based
            string strGetAllowanceTimeBased = @"SELECT ISNULL(dbo.GetAllowanceAmountTimeBased(
											     '{0}'
											   , '{1}'
											   , '{2}'
											   , '{3}'
											   , '{4}'
											   , {5}
											   , {6}),0) as Amount";

            string sqlUpdateRegOTTimeBased = @"UPDATE [T_EmployeeLogLedger{6}]
                                                SET [Ell_AllowanceAmt{0}] = {8}
                                                  FROM [T_EmployeeLogLedger{6}]
                                                  JOIN [T_EmployeeMaster{6}]
	                                                ON [Emt_EmployeeID] = [Ell_EmployeeId]     
                                                    {10}                                      
                                                    {11}
                                                  JOIN [T_AllowanceHeader]
	                                                ON [Alh_LedgerAlwCol] = '{0}'
                                                 WHERE [Ell_PayPeriod] = '{1}'
                                                   AND [Ell_EmployeeId] = '{4}'
                                                   AND Ell_ProcessDate = '{9}'
                                                   AND (CASE WHEN Left(Ell_Locationcode,1) = 'D' THEN 'TRUE' ELSE 'NONE' END) <> '{3}'
                                                   {2} 
                                                   {7}";
            #endregion

            DataTable dtHeader = dalHelper.ExecuteDataSet("SELECT * FROM dbo.T_AllowanceHeader WHERE Alh_Status = 'A'").Tables[0];
            foreach (DataRow drHeader in dtHeader.Rows)
            {
                #region Initialize values
                sqlUpdateHeader = string.Empty;
                strAllowanceColNo = drHeader["Alh_LedgerAlwCol"].ToString().Trim().PadLeft(2, '0');
                strDispatchedExclusion = drHeader["Alh_DispatchedExclusion"].ToString().Trim().ToUpper();
                #endregion

                #region Employment Status Exclusion
                unit = string.Empty;
                delimeter = ",";
                strStatusExclusion = drHeader["Alh_EmpTypeExclusion"].ToString().Trim();
                rows = strStatusExclusion.Split(",".ToCharArray());
                strStatusExclusion = "";
                foreach (string r in rows)
                {
                    string[] items = r.Split(delimeter.ToCharArray());
                    if (r != "")
                    {
                        if (unit.Trim() != "")
                            unit = unit + ",'" + r.ToString().Trim() + "'";
                        else
                            unit = "'" + r.ToString().Trim() + "'";
                    }
                }
                if (unit.Trim() != "")
                    strStatusExclusion = string.Format(@"  and Emt_EmploymentStatus not in ({0})", unit);
                else
                    strStatusExclusion = string.Empty;
                #endregion

                #region Rank/Level Exclusion
                unit = string.Empty;
                strJobLevelExclusion = drHeader["Alh_RankLevelExclusion"].ToString().Trim();
                rows = strJobLevelExclusion.Split(",".ToCharArray());
                foreach (string r in rows)
                {
                    string[] items = r.Split(delimeter.ToCharArray());
                    if (r != "")
                    {
                        if (unit.Trim() != "")
                            unit = unit + ",'" + r.ToString().Trim() + "'";
                        else
                            unit = "'" + r.ToString().Trim() + "'";
                    }
                }
                if (unit.Trim() != "")
                    strStatusExclusion += string.Format(@"  and Emt_JobLevel not in ({0})", unit);
                else
                    strStatusExclusion += string.Empty;
                #endregion

                #region Payroll Type Exclusion
                unit = string.Empty;
                strPayTypeExclusion = drHeader["Alh_PayTypeExclusion"].ToString().Trim();
                rows = strPayTypeExclusion.Split(",".ToCharArray());
                strPayTypeExclusion = "";
                foreach (string r in rows)
                {
                    string[] items = r.Split(delimeter.ToCharArray());
                    if (r != "")
                    {
                        if (unit.Trim() != "")
                            unit = unit + ",'" + r.ToString().Trim() + "'";
                        else
                            unit = "'" + r.ToString().Trim() + "'";

                        //Wage Order No. 15 of the RTWPB IV-A
                        if (r == "MW")
                            strPayTypeExclusion += @"  and Emt_TaxExempt = 1 ";
                        else if (r == "!MW")
                            strPayTypeExclusion += @"  and Emt_TaxExempt = 0 ";
                    }
                }
                if (unit.Trim() != "")
                    strPayTypeExclusion += string.Format(@"  and Emt_PayrollType not in ({0}) ", unit);
                else
                    strPayTypeExclusion += string.Empty;
                #endregion

                #region Cleanup allowance values
                sqlUpdateHeader = string.Format(strClearAllowance, strForHist, strAllowanceColNo, PayPeriod, strClearAllowanceCondition);
                dalHelper.ExecuteNonQuery(sqlUpdateHeader);
                #endregion

                #region Hour Based
                if (drHeader["Alh_BracketBasis"].ToString().Equals("H")) //Hour Based
                {
                    sqlUpdateHeader = string.Format(sqlUpdateRegOT
                                                        , strAllowanceColNo             //0
                                                        , PayPeriod                     //1
                                                        , strStatusExclusion            //2
                                                        , strDispatchedExclusion        //3
                                                        , strHourBasedAlwCondition      //4
                                                        , RGOTOFFSET                    //5
                                                        , strForHist                    //6
                                                        , strPayTypeExclusion           //7
                                                        , strEmpMasterHistCondition     //8
                                                        , strJobStatusCondition         //9
                                                        , strSpecialAlwCondition);      //10
                    dalHelper.ExecuteNonQuery(sqlUpdateHeader);
                }
                #endregion

                #region Time Based
                if (drHeader["Alh_BracketBasis"].ToString().Equals("T"))
                {
                    dtAllowanceDetails = GetAllowanceDetails(strAllowanceColNo);
                    bCheckOT = false;
                    bFound = false;
                    foreach (DataRow drAllowanceDetail in dtAllowanceDetails.Rows)
                    {
                        if (drAllowanceDetail["Alh_PostingType"].ToString().Equals("O") || drAllowanceDetail["Alh_PostingType"].ToString().Equals("A"))
                            bCheckOT = true;

                        dtLogLedger = GetEmployeesLoggedOutPastTimeQuota(ProcessAll, EmployeeId, EmployeeList
                                                                        , drAllowanceDetail["Ald_DayCode"].ToString()
                                                                        , drAllowanceDetail["Ald_TimeStart"].ToString()
                                                                        , drAllowanceDetail["Ald_TimeEnd"].ToString()
                                                                        , bCheckOT
                                                                        , currentflag);
                        dtOvertime = GetAllOvertimeRecords(ProcessAll, EmployeeId, EmployeeList
                                                            , drAllowanceDetail["Ald_TimeStart"].ToString()
                                                            , drAllowanceDetail["Ald_TimeEnd"].ToString());

                        foreach (DataRow drLogLedger in dtLogLedger.Rows)
                        {
                            bFound = false;
                            if (bCheckOT)
                            {
                                //Compare with OT applications
                                drArrOvertime = dtOvertime.Select(string.Format("Eot_EmployeeId = '{0}' and Eot_OvertimeDate = '{1}'", drLogLedger["Ell_EmployeeID"], drLogLedger["Ell_ProcessDate"]));
                                if (drArrOvertime.Length > 0)
                                    bFound = true;
                            }
                            else
                                bFound = true; //Actual OUT is past the allowance time start

                            if (bFound)
                            {
                                //Save Allowance (Not all records can be saved because of other factors like effectivity date, min-max allowance amount, or data setup errors)
                                string sqlquery = string.Format(strGetAllowanceTimeBased
                                                                , drLogLedger["Ell_ProcessDate"].ToString()             //0
                                                                , drLogLedger["Ell_EmployeeID"].ToString()              //1
                                                                , strAllowanceColNo                                     //2
                                                                , drLogLedger["Ell_DayCode"].ToString()                 //3
                                                                , drAllowanceDetail["Alh_ExtensionTable"].ToString()    //4
                                                                , drAllowanceDetail["Alh_MinAmount"].ToString()         //5
                                                                , drAllowanceDetail["Alh_MaxAmount"].ToString());       //6
                                dtAllowanceAmt = dalHelper.ExecuteDataSet(sqlquery).Tables[0];

                                if (dtAllowanceAmt.Rows.Count > 0)
                                {
                                    sqlUpdateHeader = string.Format(sqlUpdateRegOTTimeBased
                                                                        , strAllowanceColNo                         //0
                                                                        , PayPeriod                                 //1
                                                                        , strStatusExclusion                        //2
                                                                        , strDispatchedExclusion                    //3
                                                                        , drLogLedger["Ell_EmployeeID"].ToString()  //4
                                                                        , RGOTOFFSET                                //5
                                                                        , strForHist                                //6
                                                                        , strPayTypeExclusion                       //7
                                                                        , dtAllowanceAmt.Rows[0]["Amount"]          //8
                                                                        , drLogLedger["Ell_ProcessDate"].ToString() //9
                                                                        , strEmpMasterHistCondition                 //10
                                                                        , strJobStatusCondition);                   //11
                                    dalHelper.ExecuteNonQuery(sqlUpdateHeader);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Monthly Allowance computation
                if (!Convert.ToBoolean(drHeader["Alh_DailyRate"].ToString()))
                {
                    #region Get divisor amount
                    string strDivisorQuery = "";
                    if (drHeader["Alh_FactorDivisor"].ToString().Equals("R")) //Regular days only
                    {
                        strDivisorQuery = string.Format(@"SELECT Ell_EmployeeId as EmployeeId, COUNT(*) as Divisor
                                                            FROM T_EmployeeLogLedger
                                                            WHERE Ell_RestDay != 1 AND Ell_Holiday != 1 AND Ell_PayPeriod = '{0}'
                                                            GROUP BY Ell_EmployeeId", PayPeriod);
                    }
                    else if (drHeader["Alh_FactorDivisor"].ToString().Equals("A")) //All days
                    {
                        strDivisorQuery = string.Format(@"SELECT Ell_EmployeeId as EmployeeId, COUNT(*) as Divisor
                                                            FROM T_EmployeeLogLedger
                                                            WHERE Ell_PayPeriod = '{0}'
                                                            GROUP BY Ell_EmployeeId", PayPeriod);
                    }
                    else //All working days
                    {
                        strDivisorQuery = string.Format(@"SELECT Ell_EmployeeId as EmployeeId, sum(ell_workingday) as Divisor
                                                            FROM T_EmployeeLogLedger
                                                            WHERE Ell_PayPeriod = '{0}'
                                                            GROUP BY Ell_EmployeeId", PayPeriod);
                    }
                    #endregion

                    #region Update monthly allowance
                    string strMonthlyRateQuery = @"UPDATE T_EmployeeLogLedger
                                                    SET Ell_AllowanceAmt{0} = AllowanceAmt
                                                    FROM T_EmployeeLogLedger a
                                                    INNER JOIN
                                                    (
                                                    SELECT Ell_EmployeeId, SUM(Ell_AllowanceAmt{0})/Divisor as AllowanceAmt
                                                    FROM T_EmployeeLogLedger b
                                                    INNER JOIN (
                                                        {1}
                                                    ) c
                                                    ON c.EmployeeId = b.Ell_EmployeeId
                                                    WHERE Ell_AllowanceAmt{0} > 0
                                                    GROUP BY Ell_EmployeeId, Divisor
                                                    ) d
                                                    ON d.Ell_EmployeeId = a.Ell_EmployeeId
                                                    WHERE Ell_ProcessDate = (SELECT Ppm_StartCycle 
						                                                        FROM T_PayPeriodMaster 
						                                                        WHERE Ppm_PayPeriod = '{2}')
                                                    {3}

                                                    UPDATE T_EmployeeLogLedger
                                                    SET Ell_AllowanceAmt{0} = 0
                                                    WHERE Ell_ProcessDate != (SELECT Ppm_StartCycle 
						                                                        FROM T_PayPeriodMaster 
						                                                        WHERE Ppm_PayPeriod = '{2}')
                                                    {4}";

                    strMonthlyRateQuery = string.Format(strMonthlyRateQuery
                                                            , strAllowanceColNo
                                                            , strDivisorQuery
                                                            , PayPeriod
                                                            , strMonthlyAlwCondition
                                                            , strClearAllowanceCondition);
                    dalHelper.ExecuteNonQuery(strMonthlyRateQuery, CommandType.Text);
                    #endregion
                }
                #endregion
            }
        }

        public void PostOtherAllowances(bool ProcessAll, string EmployeeId, string EmployeeList, string PayPeriod)
        {
            string strEmployeeDlyAlwCondition = "";
            string strEmployeeLedgerCondition = "";
            if (!ProcessAll && EmployeeId != "")
            {
                strEmployeeDlyAlwCondition = " AND Eda_EmployeeId = '" + EmployeeId + "'";
                strEmployeeLedgerCondition = " AND Ell_EmployeeId = '" + EmployeeId + "'";
            }
            else if (ProcessAll == true && EmployeeList != "")
            {
                strEmployeeDlyAlwCondition = " AND Eda_EmployeeId IN (" + EmployeeList + ")";
                strEmployeeLedgerCondition = " AND Ell_EmployeeId IN (" + EmployeeList + ")";
            }
            else
            {
                strEmployeeDlyAlwCondition = "";
                strEmployeeLedgerCondition = "";
            }

            dalHelper.ExecuteNonQuery(string.Format(@"DELETE FROM T_EmployeeDailyAllowance 
                                                      WHERE Eda_Cycle = 'C' 
                                                        AND Eda_CurrentPayPeriod = '{1}'
                                                        {0}", strEmployeeDlyAlwCondition, PayPeriod));
            string sqlInsert = @" INSERT INTO [T_EmployeeDailyAllowance] ";
            for (int i = 1; i <= 12; i++)
            {
                sqlInsert += @"
                                  SELECT [Ell_EmployeeId] [Eda_EmployeeId]
		                                ,[Ell_PayPeriod] [Eda_CurrentPayPeriod]
		                                ,[Alh_AllowanceCode] [Eda_AllowanceCode]
		                                ,0 [Eda_PayrollPost]
	                                    ,SUM([Ell_AllowanceAmt{0}]) [Eda_AllowanceAmt]
		                                ,'C' [Eda_Cycle]
		                                ,'{2}'
		                                ,GETDATE()		
	                                FROM [T_EmployeeLogLedger]
	                                JOIN [T_AllowanceHeader]
	                                  ON [Alh_LedgerAlwCol] = '{0}'
                                   WHERE [Ell_AllowanceAmt{0}] > 0
                                     AND [Ell_PayPeriod] = '{3}'
                                     {1}
                                GROUP BY [Ell_EmployeeId]
		                                ,[Ell_PayPeriod]
		                                ,[Alh_AllowanceCode]
                                
                                UNION ALL";

                sqlInsert = string.Format(sqlInsert, i.ToString().PadLeft(2, '0'), strEmployeeLedgerCondition, UserCode, PayPeriod);

            }
            sqlInsert = sqlInsert.Remove(sqlInsert.Length - 9);
            dalHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
        }

        public DataTable GetAllowanceDetails(string strAllowanceColNo)
        {
            string query = string.Format(@"SELECT * FROM T_AllowanceHeader
                                          INNER JOIN T_AllowanceDetail
                                          ON Alh_LedgerAlwCol = Ald_LedgerAlwCol
                                          WHERE Alh_BracketBasis = 'T'
                                            AND Alh_LedgerAlwCol = '{0}'
                                          ORDER BY Alh_LedgerAlwCol", strAllowanceColNo);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeesLoggedOutPastTimeQuota(bool ProcessAll, string EmployeeId, string EmployeeList, string DayCode, string AllowanceTimeStart, string AllowanceTimeEnd, bool CheckOT, bool IsCurrent)
        {
            string strOTCondition = "";
            if (CheckOT)
                strOTCondition = " AND Ell_ComputedOvertimeMin > 0 ";

            string EmployeeLogLedgerTable = "T_EmployeeLogLedger";
            if (!IsCurrent)
                EmployeeLogLedgerTable = "T_EmployeeLogLedgerHist";

            string strEmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                strEmployeeCondition = " AND Ell_EmployeeID = '" + EmployeeId + "'";
            else if (ProcessAll == true && EmployeeList != "")
                strEmployeeCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ")";
            else
                strEmployeeCondition = "";

            string strTimeCondition = "";
            if (AllowanceTimeStart.Length == 4 && AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Ell_ActualTimeIn_1 <= '{0}' AND Ell_ActualTimeOut_2 >= '{1}' ", AllowanceTimeStart, AllowanceTimeEnd);
            else if (AllowanceTimeStart.Length == 4)
                strTimeCondition += string.Format(" AND Ell_ActualTimeIn_1 <= '{0}' ", AllowanceTimeStart);
            else if (AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Ell_ActualTimeOut_2 >= '{0}' ", AllowanceTimeEnd);

            string query = string.Format(@"SELECT Ell_EmployeeId, Ell_ProcessDate, Ell_DayCode, Ell_ActualTimeIn_1, Ell_ActualTimeOut_2 FROM {5}
                                            WHERE Ell_PayPeriod = '{1}'
                                            AND Ell_DayCode = '{2}' {3} {4} {0}", strTimeCondition, PayPeriod, DayCode, strOTCondition, strEmployeeCondition, EmployeeLogLedgerTable);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllOvertimeRecords(bool ProcessAll, string EmployeeId, string EmployeeList, string AllowanceTimeStart, string AllowanceTimeEnd)
        {
            string strEmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                strEmployeeCondition = " AND Eot_EmployeeID = '" + EmployeeId + "'";
            else if (ProcessAll == true && EmployeeList != "")
                strEmployeeCondition = " AND Eot_EmployeeID IN (" + EmployeeList + ")";
            else
                strEmployeeCondition = "";

            string strTimeCondition = "";
            if (AllowanceTimeStart.Length == 4 && AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Eot_StartTime <= '{0}' AND Eot_EndTime >= '{1}' ", AllowanceTimeStart, AllowanceTimeEnd);
            else if (AllowanceTimeStart.Length == 4)
                strTimeCondition += string.Format(" AND Eot_StartTime <= '{0}' ", AllowanceTimeStart);
            else if (AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Eot_EndTime >= '{0}' ", AllowanceTimeEnd);

            #region query
            string query = string.Format(@"DECLARE @CurPeriod AS CHAR(7)
                                           SET @CurPeriod = (SELECT Ppm_PayPeriod FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C')

                                           SELECT DISTINCT Eot_EmployeeId, Eot_StartTime, Eot_EndTime, Eot_CurrentPayPeriod, Eot_OvertimeDate, Eot_OvertimeType
                                           FROM T_EmployeeOvertime 
                                           WHERE Eot_CurrentPayPeriod <= @CurPeriod
                                                 AND Eot_Status IN ('A','9') 
                                                 {0}
                                                 {1}
                                           UNION
                                           SELECT DISTINCT Eot_EmployeeId, Eot_StartTime, Eot_EndTime, Eot_CurrentPayPeriod, Eot_OvertimeDate, Eot_OvertimeType
                                           FROM T_EmployeeOvertimeHist
                                           WHERE Eot_CurrentPayPeriod <= @CurPeriod
                                                 AND Eot_Status IN ('A','9')
                                                 {0}
                                                 {1}
                                           ORDER BY Eot_EmployeeId, Eot_OvertimeDate, Eot_StartTime, Eot_EndTime"
                                                , strEmployeeCondition, strTimeCondition);
            #endregion
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        #endregion

        #region Pay Period
        public string GetNextCycle(int NextCount)
        {
            DataTable dtPayPeriods;
            string sqlQuery = @"SELECT Ppm_PayPeriod 
                                  FROM T_PayPeriodMaster
                                 WHERE Ppm_Status = 'A'
                                   AND Ppm_CycleIndicator = 'F'
                              ORDER BY Ppm_PayPeriod ASC";

            using (DALHelper dal = new DALHelper())
            {
                dtPayPeriods = dal.ExecuteDataSet(sqlQuery, CommandType.Text).Tables[0];
            }

            if (dtPayPeriods.Rows.Count < NextCount)
            {
                CommonProcedures.showMessageInformation(string.Format("{0} does not exist.", "Next " + NextCount + " Payroll Period"));
                return "";
            }
            else
                return dtPayPeriods.Rows[NextCount - 1][0].ToString().Trim();
        }

        public string GetNextCycleFromTargetDB(int NextCount, string TargetDB)
        {
            DataTable dtPayPeriods;
            string sqlQuery = string.Format(@"SELECT Ppm_PayPeriod 
                                  FROM {0}..T_PayPeriodMaster
                                 WHERE Ppm_Status = 'A'
                                   AND Ppm_CycleIndicator = 'F'
                              ORDER BY Ppm_PayPeriod ASC", TargetDB);

            using (DALHelper dal = new DALHelper())
            {
                dtPayPeriods = dal.ExecuteDataSet(sqlQuery, CommandType.Text).Tables[0];
            }

            if (dtPayPeriods.Rows.Count < NextCount)
            {
                CommonProcedures.showMessageInformation(string.Format("{0} does not exist.", "Next " + NextCount + " Payroll Period"));
                return "";
            }
            else
                return dtPayPeriods.Rows[NextCount - 1][0].ToString().Trim();
        }

        public DateTime GetPayPeriodDateRange(string PayPeriod, bool isEndDate)
        {
            DataTable dtPayPeriod;
            string sqlQuery = @"SELECT [Ppm_StartCycle]
                                      ,[Ppm_StartCycle]
                                  FROM [T_PayPeriodMaster]
                                 WHERE [Ppm_PayPeriod] = '{0}'";

            using (DALHelper dal = new DALHelper())
            {
                dtPayPeriod = dal.ExecuteDataSet(string.Format(sqlQuery, PayPeriod), CommandType.Text).Tables[0];
            }

            if (dtPayPeriod.Rows.Count == 0)
            {
                CommonProcedures.showMessageInformation(string.Format("{0} does not exist.", "Payroll Period " + PayPeriod));
                return new DateTime();
            }
            else if (isEndDate)
                return Convert.ToDateTime(dtPayPeriod.Rows[0][1]);
            else
                return Convert.ToDateTime(dtPayPeriod.Rows[0][0]);
        }

        public DataTable GetCycleRange(string PayPeriod)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@PayPeriod", PayPeriod);

            string strQuery = @"SELECT Ppm_StartCycle, Ppm_EndCycle FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = @PayPeriod";

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery, CommandType.Text, paramInfo).Tables[0];

            return dtResult;
        }

        public DataTable GetCycleRangeFromTargetDB(string PayPeriod, string TargetDB)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@PayPeriod", PayPeriod);

            string strQuery = string.Format(@"SELECT Ppm_StartCycle, Ppm_EndCycle FROM {0}..T_PayPeriodMaster WHERE Ppm_PayPeriod = @PayPeriod", TargetDB);

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery, CommandType.Text, paramInfo).Tables[0];

            return dtResult;
        }
        #endregion

        #region Payroll
        public int GetFillerDayCodesCount()
        {
            string strQuery = @"SELECT Isnull(Count(Dcf_DayCode),0)
                                FROM T_DayCodeFiller
                                INNER JOIN T_DayCodeMaster on Dcf_DayCode = Dcm_DayCode
                                WHERE Dcf_Status = 'A' 
                                AND Dcm_Status = 'A'";

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery).Tables[0];

            return Convert.ToInt32(dtResult.Rows[0][0].ToString());
        }

        public bool CheckIfPayRateExists(string strPayRate)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@PayRate", strPayRate);

            string strQuery = @"SELECT Adt_AccountCode 
                                FROM T_AccountDetail
                                WHERE Adt_AccountType = 'PAYRATE' AND Adt_AccountCode = @PayRate AND Adt_Status = 'A'";

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery, CommandType.Text, paramInfo).Tables[0];

            if (dtResult.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Unused
        public void RefreshDefaultOTPerEmployee(string PayPeriod, string OvertimeFlag, string EmployeeId, string ProcessDate)
        {
            string strDeletedControlNo = RemoveDefaultOTApplicationsByEmployee(PayPeriod, OvertimeFlag, EmployeeId, ProcessDate);
            CreateDefaultOTApplicationsByEmployee(PayPeriod, OvertimeFlag, EmployeeId, ProcessDate, strDeletedControlNo);
        }

        public void CreateDefaultOTApplicationsByEmployee(string PayPeriod, string OvertimeFlag, string EmployeeId, string ProcessDate, string DeletedControlNo)
        {
            try
            {
                string strOvertimeTable = "T_EmployeeOvertime";
                string strLogLedgerTable = "T_EmployeeLogLedger";
                string strEmployeeMasterTable = "T_EmployeeMaster";
                string strEmpMasterHistCondition = "";

                if (OvertimeFlag.Equals("P"))
                {
                    strOvertimeTable = "T_EmployeeOvertimeHist";
                    strLogLedgerTable = "T_EmployeeLogLedgerHist";
                    strEmployeeMasterTable = "T_EmployeeMasterHist";
                    strEmpMasterHistCondition = "and Emt_PayPeriod = Ell_PayPeriod";
                }

                //Overtime Reason (if applicable)
                if (DeletedControlNo.Length > 0)
                {
                    DeletedControlNo = " FROM " + DeletedControlNo;
                }

                //Process Date Condition
                string ProcessDateCondition = "";
                if (!ProcessDate.Equals(""))
                {
                    ProcessDateCondition = string.Format(" AND Ell_ProcessDate = '{0}'", ProcessDate);
                }

                //Get all data needed for OT application
                string sqlGetLogLedgerDataQuery = string.Format(@"SELECT Ell_EmployeeId
                                                                    , Ell_ProcessDate
                                                                    , Ell_DayCode
                                                                    , Ell_EncodedOvertimePostHr
                                                                    , Gdo_OTHours
                                                                    , Scm_ShiftTimeIn
                                                                    , Scm_ShiftTimeOut
                                                                    , Scm_ShiftHours
                                                                    , Emt_CostCenterCode
                                                                    , Cal_WorkCode
                                                                    , Scm_ScheduleType
                                                                FROM {2}
                                                                INNER JOIN T_GroupDefaultOT ON Gdo_WorkType = Ell_Worktype
                                                                INNER JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode
                                                                INNER JOIN {3} ON Emt_EmployeeId = Ell_EmployeeId
                                                                {5}
                                                                INNER JOIN T_CalendarGroupTmp ON Cal_WorkType = Ell_WorkType AND Cal_WorkGroup = Ell_WorkGroup AND Cal_ProcessDate = Ell_ProcessDate
                                                                WHERE Ell_PayPeriod = '{0}'
                                                                    AND Gdo_Status = 'A'
                                                                    AND Gdo_OTHours > 0
                                                                    AND Ell_RestDay = 0
                                                                    AND Ell_Holiday = 0
                                                                    AND Scm_ShiftCode != 'N001'
                                                                    AND Ell_EmployeeId = '{1}' {4}
                                                                UNION
                                                                SELECT Ell_EmployeeId
                                                                    , Ell_ProcessDate
                                                                    , Ell_DayCode
                                                                    , Ell_EncodedOvertimePostHr
                                                                    , ISNULL(Gdo_OTHours, 0)
                                                                    , Scm_ShiftTimeIn
                                                                    , Scm_ShiftTimeOut
                                                                    , Scm_ShiftHours
                                                                    , Emt_CostCenterCode
                                                                    , 'D8' --temp
                                                                    , 'D'
                                                                FROM {2}
                                                                INNER JOIN T_GroupDefaultOT ON Gdo_WorkType = Ell_Worktype
                                                                INNER JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode
                                                                INNER JOIN {3} ON Emt_EmployeeId = Ell_EmployeeId
                                                                {5}
                                                                WHERE Ell_PayPeriod = '{0}' 
																	AND Gdo_Status = 'A'
                                                                    AND Gdo_OTHours > 0
                                                                    AND Ell_Holiday = 1 
                                                                    AND Ell_RestDay = 0
                                                                    AND Scm_ShiftCode != 'N001'
                                                                    AND Ell_EmployeeId = '{1}' {4}", PayPeriod, EmployeeId, strLogLedgerTable, strEmployeeMasterTable, ProcessDateCondition, strEmpMasterHistCondition);

                DataTable dtEmployeeResult;
                dtEmployeeResult = dalHelper.ExecuteDataSet(sqlGetLogLedgerDataQuery).Tables[0];

                if (dtEmployeeResult.Rows.Count > 0)
                {
                    //Get last increment of DFOVERTIME
                    string sqlDFOVERTIMEIncrementQuery = @"SELECT Tcm_LastSeries 
                                                           FROM T_TransactionControlMaster
                                                           WITH (UPDLOCK)
                                                           WHERE Tcm_TransactionCode = 'DFOVERTIME'";
                    DataTable dtResult;
                    dtResult = dalHelper.ExecuteDataSet(sqlDFOVERTIMEIncrementQuery).Tables[0];
                    int iSeries = Convert.ToInt32(dtResult.Rows[0][0].ToString());

                    //Update last increment of DFOVERTIME
                    sqlDFOVERTIMEIncrementQuery = string.Format(@"UPDATE T_TransactionControlMaster
                                                                  SET Tcm_LastSeries = {0}
                                                                    , ludatetime = getdate()
                                                                  WHERE Tcm_TransactionCode = 'DFOVERTIME'", iSeries + dtEmployeeResult.Rows.Count);
                    InsertUpdate(sqlDFOVERTIMEIncrementQuery);

                    //Get company current year
                    string sqlCompanyCurYearQuery = @"SELECT Ccd_CurrentYear FROM T_CompanyMaster";
                    dtResult = dalHelper.ExecuteDataSet(sqlCompanyCurYearQuery).Tables[0];
                    string strCompanyYear = dtResult.Rows[0][0].ToString().Substring(2, 2);

                    foreach (DataRow drEmployee in dtEmployeeResult.Rows)
                    {
                        string strStartTime;
                        string strEndTime;
                        double dOvertimeHour;
                        double dDefaultOvertimeHour = Convert.ToDouble(drEmployee["Gdo_OTHours"].ToString());

                        if (drEmployee["Cal_WorkCode"].ToString().Trim().Length >= 2)
                        {
                            strStartTime = drEmployee["Scm_ShiftTimeIn"].ToString();
                            strEndTime = AddMinutesToHourStr(drEmployee["Scm_ShiftTimeOut"].ToString(), Convert.ToInt32(dDefaultOvertimeHour * 60));
                            dOvertimeHour = Convert.ToDouble(drEmployee["Scm_ShiftHours"].ToString()) + dDefaultOvertimeHour;
                        }
                        else
                        {
                            strStartTime = drEmployee["Scm_ShiftTimeOut"].ToString();
                            strEndTime = AddMinutesToHourStr(drEmployee["Scm_ShiftTimeOut"].ToString(), Convert.ToInt32(dDefaultOvertimeHour * 60));
                            dOvertimeHour = dDefaultOvertimeHour;
                        }

                        //Increment control value
                        iSeries++;
                        string strControlNo = "D" + strCompanyYear + string.Format("{0:000000000}", iSeries);

                        if (!CheckIfExistsOvertimeApplication(drEmployee["Ell_EmployeeId"].ToString()
                                                                , drEmployee["Ell_ProcessDate"].ToString()
                                                                , strStartTime
                                                                , strEndTime
                                                                , strOvertimeTable))
                        {
                            //Create OT application per employee per day
                            #region Insert into Employee Overtime table
                            string sqlCreateOTPermitsQuery = string.Format(@"INSERT INTO {10}
                                                                            ( Eot_CurrentPayPeriod
                                                                            , Eot_EmployeeId
                                                                            , Eot_OvertimeDate
                                                                            , Eot_Seqno
                                                                            , Eot_AppliedDate
                                                                            , Eot_OvertimeType
                                                                            , Eot_StartTime
                                                                            , Eot_EndTime
                                                                            , Eot_OvertimeHour
                                                                            , Eot_Reason
                                                                            , Eot_JobCode
                                                                            , Eot_ClientJobNo
                                                                            , Eot_EndorsedDateToChecker
                                                                            , Eot_CheckedBy
                                                                            , Eot_CheckedDate
                                                                            , Eot_Checked2By
                                                                            , Eot_Checked2Date
                                                                            , Eot_ApprovedBy
                                                                            , Eot_ApprovedDate
                                                                            , Eot_Status
                                                                            , Eot_ControlNo
                                                                            , Eot_OvertimeFlag
                                                                            , Eot_Costcenter
                                                                            , Eot_Filler1
                                                                            , Eot_Filler2
                                                                            , Eot_Filler3
                                                                            , Eot_BatchNo
                                                                            , Usr_Login
                                                                            , Ludatetime )
                                                                          VALUES( '{0}'
                                                                            , '{1}'
                                                                            , '{2}'
                                                                            , '01'
                                                                            , Getdate()
                                                                            , 'P'
                                                                            , '{3}'
                                                                            , '{4}'
                                                                            , {5}
                                                                            , '{6}'
                                                                            , ''
                                                                            , ''
                                                                            , Getdate()
                                                                            , 'sa'
                                                                            , Getdate()
                                                                            , 'sa'
                                                                            , Getdate()
                                                                            , 'sa'
                                                                            , Getdate()
                                                                            , '9'
                                                                            , '{7}'
                                                                            , '{8}'
                                                                            , '{9}'
                                                                            , ''
                                                                            , ''
                                                                            , ''
                                                                            , ''
                                                                            , 'sa'
                                                                            , Getdate() )"
                                                                                    , PayPeriod
                                                                                    , drEmployee["Ell_EmployeeId"].ToString()
                                                                                    , drEmployee["Ell_ProcessDate"].ToString()
                                                                                    , strStartTime
                                                                                    , strEndTime
                                                                                    , dOvertimeHour
                                                                                    , "DEFAULT OT" + DeletedControlNo
                                                                                    , strControlNo
                                                                                    , OvertimeFlag
                                                                                    , drEmployee["Emt_CostCenterCode"].ToString()
                                                                                    , strOvertimeTable);
                            InsertUpdate(sqlCreateOTPermitsQuery);
                            #endregion

                            //Update log ledger table
                            string sqlUpdateLogLedgerQuery = string.Format(@"UPDATE {3} 
                                                                         SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr + {0}
                                                                            , ludatetime = getdate()
                                                                         WHERE Ell_EmployeeId = '{1}'
                                                                            AND Ell_ProcessDate = '{2}'"
                                                                                    , dOvertimeHour
                                                                                    , drEmployee["Ell_EmployeeId"].ToString()
                                                                                    , drEmployee["Ell_ProcessDate"].ToString()
                                                                                    , strLogLedgerTable);
                            InsertUpdate(sqlUpdateLogLedgerQuery);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public string RemoveDefaultOTApplicationsByEmployee(string PayPeriod, string OvertimeFlag, string EmployeeId, string ProcessDate)
        {
            string strDeletedControlNo = "";
            try
            {
                string strOvertimeTable = "T_EmployeeOvertime";
                string strLogLedgerTable = "T_EmployeeLogLedger";

                if (OvertimeFlag.Equals("P"))
                {
                    strOvertimeTable = "T_EmployeeOvertimeHist";
                    strLogLedgerTable = "T_EmployeeLogLedgerHist";
                }

                //Get all OT applications
                string sqlGetLogLedgerDataQuery = string.Format(@"SELECT * 
                                                                  FROM {2} 
                                                                  WHERE LEFT(Eot_ControlNo, 1) = 'D' 
                                                                      AND Eot_CurrentPayPeriod = '{0}'
                                                                      AND Eot_Status IN ('9', 'A')
                                                                      AND Eot_EmployeeId = '{1}'", PayPeriod, EmployeeId, strOvertimeTable);

                if (!ProcessDate.Equals(""))
                {
                    sqlGetLogLedgerDataQuery += string.Format(" AND Eot_OvertimeDate = '{0}'", ProcessDate);
                }

                DataTable dtOTApps;
                dtOTApps = dalHelper.ExecuteDataSet(sqlGetLogLedgerDataQuery).Tables[0];

                if (dtOTApps.Rows.Count > 0)
                {
                    foreach (DataRow dtOTApp in dtOTApps.Rows)
                    {
                        strDeletedControlNo = dtOTApp["Eot_ControlNo"].ToString();

                        //Update log ledger table
                        string sqlUpdateLogLedgerQuery = string.Format(@"UPDATE {3} 
                                                                         SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr - {0}
                                                                            , ludatetime = getdate()
                                                                         WHERE Ell_EmployeeId = '{1}'
                                                                            AND Ell_ProcessDate = '{2}'"
                                                                                , dtOTApp["Eot_OvertimeHour"].ToString()
                                                                                , dtOTApp["Eot_EmployeeId"].ToString()
                                                                                , dtOTApp["Eot_OvertimeDate"].ToString()
                                                                                , strLogLedgerTable);
                        InsertUpdate(sqlUpdateLogLedgerQuery);

                        if (OvertimeFlag.Equals("P")) //previous
                        {
                            string sqlDeleteOTPermitsQuery = string.Format(@"UPDATE T_EmployeeOvertimeHist
                                                                             SET Eot_Reason = Eot_Reason + '{0}'
                                                                                , Eot_Status = '{1}' 
                                                                                , ludatetime = getdate()   
                                                                             WHERE Eot_ControlNo = '{2}'"
                                                                                , " (CANCELLED)", "2", strDeletedControlNo);
                            InsertUpdate(sqlDeleteOTPermitsQuery);
                        }
                    }

                    //Delete OT application per employee per day
                    if (OvertimeFlag.Equals("C")) //current
                    {
                        string sqlDeleteOTPermitsQuery = string.Format(@"DELETE FROM T_EmployeeOvertime 
                                                                         WHERE LEFT(Eot_ControlNo, 1) = 'D' 
                                                                            AND Eot_CurrentPayPeriod = '{0}'
                                                                            AND Eot_EmployeeId = '{1}'", PayPeriod, EmployeeId);
                        if (!ProcessDate.Equals(""))
                        {
                            sqlDeleteOTPermitsQuery += string.Format(" AND Eot_OvertimeDate = '{0}'", ProcessDate);
                        }

                        InsertUpdate(sqlDeleteOTPermitsQuery);
                    }
                }
            }
            catch (Exception err)
            {
                throw new PayrollException("Error: " + err.Message);
            }
            return strDeletedControlNo;
        }

        public string AddMinutesToHourStr(string baseHour, int minutes)
        {
            int iSumInMins = GetMinsFromHourStr(baseHour) + minutes;
            return GetHourStrFromMins(iSumInMins);
        }

        public bool CheckIfExistsOvertimeApplication(string EmployeeId, string OvertimeDate, string StartTime, string EndTime, string OvertimeTable)
        {
            string query = string.Format(@"SELECT COUNT(*) FROM {0}
                                            WHERE Eot_EmployeeId = '{1}'
                                            AND Eot_OvertimeDate = '{2}'
                                            AND (('{3}' > Eot_StartTime AND '{3}' < Eot_EndTime)
                                            OR ('{4}' > Eot_StartTime AND '{4}' < Eot_EndTime))", OvertimeTable, EmployeeId, OvertimeDate, StartTime, EndTime);
            DataTable dtEmployeeCnt;
            int Cnt = 0;
            dtEmployeeCnt = dalHelper.ExecuteDataSet(query).Tables[0];

            if (dtEmployeeCnt.Rows.Count > 0)
                Cnt = Convert.ToInt32(dtEmployeeCnt.Rows[0][0]);

            return (Cnt > 0);
        }

        public int GetMinsFromHourStr(string hour)
        {
            return (Convert.ToInt32(hour.Substring(0, 2)) * 60) + Convert.ToInt32(hour.Substring(2, 2));
        }

        public string GetHourStrFromMins(int minutes)
        {
            int iHours, iMinutes;
            string strHours, strMinutes;

            iHours = minutes / 60;
            strHours = iHours.ToString();
            iMinutes = minutes % 60;
            strMinutes = iMinutes.ToString();

            // Pad left zeros
            if (strHours.Length < 2)
            {
                strHours = "0" + strHours;
            }
            if (strMinutes.Length < 2)
            {
                strMinutes = "0" + strMinutes;
            }

            // Concat hour and minutes
            return strHours + strMinutes;
        }

        public string GetEmployeeSalary(string EmployeeID, string PayPeriod)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"DECLARE @CurPayPeriod as varchar(7)
                                SET @CurPayPeriod = (
                                        Select Ppm_PayPeriod From T_PayPeriodMaster
                                                Where Ppm_CycleIndicator = 'C'
                                                And Ppm_Status = 'A')
								DECLARE @PayPeriod as varchar(7)
                                SET @PayPeriod = '{0}'
								DECLARE @EmployeeID as varchar(15)
                                SET @EmployeeID = '{1}'

								If @CurPayPeriod = @PayPeriod
									begin 
										SELECT isnull(Emt_SalaryRate, 0)
										FROM T_EmployeeMaster 
										WHERE Emt_EmployeeID = @EmployeeID
									end
								else
									begin
										SELECT isnull(Emt_SalaryRate, 0)
										FROM T_EmployeeMasterHist Cur
										WHERE Emt_EmployeeID = @EmployeeID
										  and Emt_PayPeriod = @PayPeriod
									end";
            sqlQuery = string.Format(sqlQuery, PayPeriod, EmployeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string[] GetLeaveYear()
        {
            string[] years;
            DataSet ds;
            string sql = @"select distinct(elm_leaveyear) from t_employeeleave";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                years = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    years[i] = ds.Tables[0].Rows[i][0].ToString().Trim();
                }
            }
            else
            {
                years = new string[1];
                years[0] = "";
            }

            return years;
        }

        public DataSet GetHeaderLeaveRefundForExcel()
        {
            DataSet ds = new DataSet();

            #region Other Fields for Header
            string sql = @"
                                SELECT 'COSTCENTER'
                                , 'EMPLOYEE ID'
                                , 'LAST NAME'
                                , 'FIRST NAME'
                                , 'ID CODE'
                                , 'BASIC SALARY'
                                , 'DAILY RATE'
                                , 'HOURLY RATE'
                                , 'LEAVE TYPE'
                                , 'ENTITLED LEAVE'
                                , 'USED LEAVE'
                                , 'UNUSED LEAVE'
                                , 'NON TAX AMOUNT'
                                , 'TAX AMOUNT'
                                , 'TOTAL AMOUNT'
                        ";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sql, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public DataTable getEmployeeListingLeaveRefund(string PaypPeriod)
        {
            try
            {
                return dalHelper.ExecuteDataSet(@"
                            DECLARE @LVHRENTRY as bit
                            SET @LVHRENTRY = (SELECT Pcm_ProcessFlag
				                            FROM T_processcontrolmaster
				                            WHERE Pcm_SystemID='LEAVE'
				                            and  Pcm_ProcessID='LVHRENTRY')


                            DECLARE @MDIVISOR as decimal(9,2)

                            SET @MDIVISOR = (SELECT	Pmt_NumericValue 
                                            FROM	T_ParameterMaster 
                                            WHERE	Pmt_ParameterID = 'MDIVISOR')

                            select Distinct dbo.GetCostcenterFullName(EmployeeName.Emt_CostcenterCode)
                            , leaverefund.Elr_Employeeid
                            , EmployeeName.Emt_lastname
                            , EmployeeName.emt_firstname
                            , EmployeeName.Emt_NickName
                            --, case when EmployeeName.Emt_middlename = '' then '' else left(EmployeeName.emt_middlename,1) + '.' end
	                        , leaverefund.elr_salaryrate as [Salary Rate]
	                        , leaverefund.elr_hourlyrate * 8 as [Daily Rate]
	                        , leaverefund.elr_hourlyrate as [Hourly rate]
                            , leaverefund.Elr_LeaveType
                            , isnull(convert(decimal(18,2), case when @LVHRENTRY = 1 then Elm_Entitled
	                            else
	                            (Elm_Entitled)/8.0
                              end),0.00) as Entitled
                            , isnull(convert(decimal(18,2), case when @LVHRENTRY = 1 then Elm_Entitled - (Nontax.Elr_LeaveHr + Tax.Elr_LeaveHr)
	                            else
	                            (Elm_Entitled-(isnull(Nontax.Elr_LeaveHr,0.00) + isnull(Tax.Elr_LeaveHr,0.00)))/8.0
                              end),0.00) as Used
                            , isnull(convert(decimal(18,2), case when @LVHRENTRY = 1 then (Nontax.Elr_LeaveHr + Tax.Elr_LeaveHr)    
	                            else
	                            ((isnull(Nontax.Elr_LeaveHr,0.00) + isnull(Tax.Elr_LeaveHr,0.00)) )/8.0
                              end),0.00) as Unused
                            , convert(decimal(18,2), Isnull(Nontax.Elr_LeaveAmt, 0)) as Nontax
                            , convert(decimal(18,2), Isnull(Tax.Elr_LeaveAmt, 0)) as Tax
                            , convert(decimal(18,2), Isnull(Nontax.Elr_LeaveAmt, 0) + Isnull(Tax.Elr_LeaveAmt, 0)) as Total
                            from t_employeeleaverefund leaverefund
                            left join t_employeeleaverefund Nontax on Nontax.Elr_EmployeeId  = leaverefund. Elr_EmployeeId
	                            and Nontax.Elr_CurrentPayPeriod  = leaverefund.Elr_CurrentPayPeriod
	                            and Nontax.Elr_LeaveType = leaverefund.Elr_LeaveType
	                            and Nontax.Elr_TaxClass = 'N'
                            left join t_employeeleaverefund Tax on Tax.Elr_EmployeeId  = leaverefund. Elr_EmployeeId
	                            and Tax.Elr_CurrentPayPeriod  = leaverefund.Elr_CurrentPayPeriod
	                            and Tax.Elr_LeaveType = leaverefund.Elr_LeaveType
	                            and Tax.Elr_TaxClass = 'T'
                            INNER JOIN T_EmployeeMaster on Emt_Employeeid = leaverefund.Elr_EmployeeId
                            LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Emt_CostCenterCode
                            LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                            LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                            LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                            LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                            LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                            INNER JOIN T_EmployeeLeave on Elm_Employeeid = leaverefund.Elr_Employeeid
	                            and Elm_Leavetype = leaverefund.Elr_LeaveType
                            inner join T_LeaveTypeMaster on Ltm_LeaveType = Elm_LeaveType
                            left join t_employeemaster EmployeeName on EmployeeName.emt_employeeid = leaverefund.Elr_Employeeid
                            WHERE leaverefund.Elr_CurrentPayPeriod = '" + PayPeriod + @"'
							and ltrim(isnull(Ltm_PartOfLeave, '')) = '' 
                            --and leaverefund.Elr_leavetype='AL'
                            and T_LeaveTypeMaster.Ltm_ConvertibleToCash = 1
							order by 1, 3, 4").Tables[0];
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public DataTable getEmployeeListingLeaveRefundC1(string PaypPeriod)
        {
            try
            {
                return dalHelper.ExecuteDataSet(@"
                            DECLARE @LVHRENTRY as bit
                            SET @LVHRENTRY = (SELECT Pcm_ProcessFlag
				                            FROM T_processcontrolmaster
				                            WHERE Pcm_SystemID='LEAVE'
				                            and  Pcm_ProcessID='LVHRENTRY')


                            DECLARE @MDIVISOR as decimal(9,2)

                            SET @MDIVISOR = (SELECT	Pmt_NumericValue 
                                            FROM	T_ParameterMaster 
                                            WHERE	Pmt_ParameterID = 'MDIVISOR')

                            select Distinct leaverefund.Elr_Employeeid
                            , dbo.getcostcenterfullname(EmployeeName.Emt_CostCenterCode) CostCenter
                            , EmployeeName.Emt_lastname + ', ' + EmployeeName.emt_firstname + ' ' + case when EmployeeName.Emt_middlename = '' then '' else left(EmployeeName.emt_middlename,1) + '.' end as EmployeeName
                            , EmployeeName.emt_nickname
                            , leaverefund.Elr_LeaveType
                            , isnull(convert(decimal(18,2), case when @LVHRENTRY = 1 then Elm_Entitled
	                            else
	                            (Elm_Entitled)/8.0
                              end),0.00) as Entitled
                            , isnull(convert(decimal(18,2), case when @LVHRENTRY = 1 then Elm_Entitled - (Nontax.Elr_LeaveHr + Tax.Elr_LeaveHr)
	                            else
	                            (Elm_Entitled-(isnull(Nontax.Elr_LeaveHr,0.00) + isnull(Tax.Elr_LeaveHr,0.00)))/8.0
                              end),0.00) as Used
                            , isnull(convert(decimal(18,2), case when @LVHRENTRY = 1 then (Nontax.Elr_LeaveHr + Tax.Elr_LeaveHr)    
	                            else
	                            ((isnull(Nontax.Elr_LeaveHr,0.00) + isnull(Tax.Elr_LeaveHr,0.00)) )/8.0
                              end),0.00) as Unused
                            , convert(decimal(18,2), Isnull(Nontax.Elr_LeaveAmt, 0)) as Nontax
                            , convert(decimal(18,2), Isnull(Tax.Elr_LeaveAmt, 0)) as Tax
                            , convert(decimal(18,2), Isnull(Nontax.Elr_LeaveAmt, 0) + Isnull(Tax.Elr_LeaveAmt, 0)) as Total
                            from t_employeeleaverefund leaverefund
                            left join t_employeeleaverefund Nontax on Nontax.Elr_EmployeeId  = leaverefund. Elr_EmployeeId
	                            and Nontax.Elr_CurrentPayPeriod  = leaverefund.Elr_CurrentPayPeriod
	                            and Nontax.Elr_LeaveType = leaverefund.Elr_LeaveType
	                            and Nontax.Elr_TaxClass = 'N'
                            left join t_employeeleaverefund Tax on Tax.Elr_EmployeeId  = leaverefund. Elr_EmployeeId
	                            and Tax.Elr_CurrentPayPeriod  = leaverefund.Elr_CurrentPayPeriod
	                            and Tax.Elr_LeaveType = leaverefund.Elr_LeaveType
	                            and Tax.Elr_TaxClass = 'T'
                            INNER JOIN T_EmployeeMaster on Emt_Employeeid = leaverefund.Elr_EmployeeId
                            LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Emt_CostCenterCode
                            LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                            LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                            LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                            LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                            LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                            INNER JOIN T_EmployeeLeave on Elm_Employeeid = leaverefund.Elr_Employeeid
	                            and Elm_Leavetype = leaverefund.Elr_LeaveType
                            inner join T_LeaveTypeMaster on Ltm_LeaveType = Elm_LeaveType
                            left join t_employeemaster EmployeeName on EmployeeName.emt_employeeid = leaverefund.Elr_Employeeid
                            WHERE leaverefund.Elr_CurrentPayPeriod = '" + PayPeriod + @"'
							and ltrim(isnull(Ltm_PartOfLeave, '')) = '' 
                            --and leaverefund.Elr_leavetype='AL'
                            and T_LeaveTypeMaster.Ltm_ConvertibleToCash = 1").Tables[0];
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public DataSet GetLastTrailPerDate(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT Elt_ProcessDate
                                                , Elt_Seqno
                                                , Elt_ShiftCode
                                                , Elt_ActualTimeIn_1
                                                , Elt_ActualTimeOut_1
                                                , Elt_ActualTimeIn_2
                                                , Elt_ActualTimeOut_2
                                                , T_EmployeeLogTrail.Usr_Login AS Usr_Login
                                                , T_EmployeeLogTrail.Ludatetime AS Ludatetime
                                                , Emt_LastName
                                                , Emt_FirstName
                                            FROM T_EmployeeLogTrail
                                            LEFT JOIN T_EmployeeMaster
                                            ON T_EmployeeLogTrail.Usr_Login = Emt_EmployeeID
                                            AND T_EmployeeLogTrail.Usr_Login != 'SA'
                                            WHERE Elt_EmployeeId = '{0}'
                                            AND Elt_ProcessDate >= '{1}' AND Elt_ProcessDate <= '{2}'
                                            ORDER BY Elt_ProcessDate, Elt_Seqno", EmployeeId, DateStart, DateEnd);
            DataSet dsLogTrail;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsLogTrail = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsLogTrail;
        }

        public DataSet GetOffsetApplications(string EmployeeId, string DateStart, string DateEnd)
        {
            string query = string.Format(@"SELECT * FROM T_EmployeeOffset
                                            WHERE Eof_EmployeeId = '{0}'
                                            AND Eof_OffsetDate >= '{1}' AND Eof_OffsetDate <= '{2}'
                                            UNION
                                            SELECT * FROM T_EmployeeOffsetHist
                                            WHERE Eof_EmployeeId = '{0}'
                                            AND Eof_OffsetDate >= '{1}' AND Eof_OffsetDate <= '{2}'", EmployeeId, DateStart, DateEnd);
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public string isSatOFF(string EmployeeID, DateTime ProcessDate)
        {
            string Value = "REG";
            try
            {
                if (ProcessDate.ToString("ddd").ToUpper() == "SAT" || !GetProcessFlag("TIMEKEEP", "LATEOFFSET"))
                {
                    string sqlQuery = @"SELECT	swh_Sat1Date,
		                                        swh_Sat2Date,
		                                        swh_Sat3Date, 
		                                        swh_Sat4Date,
		                                        swh_Sat5Date
                                        FROM	T_SaturdayWorkStatusHeader
                                        WHERE	swh_YearMonth = @YearMonth
                                        AND		swh_status = 'A'
                                        AND		(swh_Sat1Date = @ProcessDate
                                        OR		swh_Sat2Date = @ProcessDate
                                        OR		swh_Sat3Date = @ProcessDate
                                        OR		swh_Sat4Date = @ProcessDate
                                        OR		swh_Sat5Date = @ProcessDate)
                                        -----------------------------------
                                        SELECT	swd_Sat1Status,
                                                swd_Sat2Status,
                                                swd_Sat3Status,	
                                                swd_Sat4Status,
                                                swd_Sat5Status
                                        FROM	T_SaturdayWorkStatusDetail
                                        WHERE	swd_YearMonth = @YearMonth
                                        AND		swd_EmployeeId = @EmployeeId";

                    ParameterInfo[] paramInfo = new ParameterInfo[3];
                    paramInfo[0] = new ParameterInfo("@YearMonth", ProcessDate.ToString("yyyyMM"));
                    paramInfo[1] = new ParameterInfo("@EmployeeId", EmployeeID);
                    paramInfo[2] = new ParameterInfo("@ProcessDate", ProcessDate);
                    DataSet dsSatOFF = dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                    if (dsSatOFF.Tables[0].Rows.Count == 0 || dsSatOFF.Tables[1].Rows.Count == 0)
                        Value = "REG";
                    else
                    {
                        if (dsSatOFF.Tables[0].Rows[0]["swh_Sat1Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat1Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat2Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat2Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat3Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat3Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat4Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat4Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat5Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat5Status"].Equals(true))
                            Value = "SOFF";
                    }
                }
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public void RemoveDefaultOTApplicationsHist(string PayPeriod) //temporary for History records
        {
            try
            {
                //Get all default OT records
                DataTable dtOTApps = GetDefaultOTCandidateDeleteRecordsHist(PayPeriod);

                if (dtOTApps.Rows.Count > 0)
                {
                    foreach (DataRow dtOTApp in dtOTApps.Rows)
                    {
                        //Update log ledger table
                        string sqlUpdateLogLedgerQuery = string.Format(@"UPDATE T_EmployeeLogLedgerHist 
                                                                         SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr - {0}
                                                                            , ludatetime = getdate()
                                                                         WHERE Ell_EmployeeId = '{1}'
                                                                            AND Ell_ProcessDate = '{2}'"
                                                                                , dtOTApp["Eot_OvertimeHour"].ToString()
                                                                                , dtOTApp["Eot_EmployeeId"].ToString()
                                                                                , dtOTApp["Eot_OvertimeDate"].ToString());
                        InsertUpdate(sqlUpdateLogLedgerQuery);
                    }
                }

                //Delete OT application per employee per day
                string sqlDeleteOTPermitsQuery = string.Format(@"DELETE FROM T_EmployeeOvertimeHist WHERE LEFT(Eot_ControlNo, 1) = 'D' AND Eot_CurrentPayPeriod = '{0}'", PayPeriod);
                InsertUpdate(sqlDeleteOTPermitsQuery);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public DataTable GetDefaultOTCandidateDeleteRecordsHist(string PayPeriod) //temporary
        {
            string sqlGetLogLedgerDataQuery = string.Format(@"SELECT * 
                                                                FROM T_EmployeeOvertimeHist 
                                                                WHERE LEFT(Eot_ControlNo, 1) = 'D' 
                                                                    AND Eot_CurrentPayPeriod = '{0}'
                                                                    AND Eot_Status IN ('9', 'A')", PayPeriod);
            DataTable dtEmployeeResult;
            dtEmployeeResult = dalHelper.ExecuteDataSet(sqlGetLogLedgerDataQuery).Tables[0];





            return dtEmployeeResult;
        }

        public void UpdateTaxAmount(string TableName, ParameterInfo[] Param, DALHelper dalHelper)
        {
            #region [Yucky]
            string sqlQuery = string.Format(@"declare  @TaxSchedule char(1)
                                                set @TaxSchedule = (SELECT Ccd_TaxSchedule FROM T_CompanyMaster)
                                                declare @TaxHeaderPayPeriod char(7)
                                                set @TaxHeaderPayPeriod = (SELECT Max(Tsh_PayPeriod) as [PayPeriod]
                                                FROM dbo.T_TaxScheduleHeader
                                                WHERE Tsh_TaxSchedule = @TaxSchedule
                                                and Tsh_PayPeriod <= @PayPeriod
                                                and Tsh_Status = 'A')


                                                UPDATE {0}
                                                SET Epc_WTaxAmt = IsNull((SELECT TOP 1 T_TaxScheduleHeader.Tsh_FixedAmt + ((({0}.Epc_GrossPayAmt - ({0}.Epc_SSSEmployeeShare + 
                                                {0}.Epc_PhilhealthEmployeeShare + {0}.Epc_HDMFEmployeeShare)) - 
                                                T_TaxScheduleDetail.Tsd_BracketAmt) * (T_TaxScheduleHeader.Tsh_TaxRate/100.00))
                                                FROM T_TaxScheduleDetail
                                                INNER JOIN T_TaxScheduleHeader on T_TaxScheduleHeader.Tsh_TaxSchedule = T_TaxScheduleDetail.Tsd_TaxSchedule
                                                and T_TaxScheduleHeader.Tsh_PayPeriod = T_TaxScheduleDetail.Tsd_PayPeriod  
                                                and T_TaxScheduleHeader.Tsh_BracketNo = T_TaxScheduleDetail.Tsd_BracketNo 
                                                and T_TaxScheduleHeader.Tsh_Status = 'A'
                                                WHERE  T_TaxScheduleHeader.Tsh_TaxSchedule = @TaxSchedule
                                                and T_TaxScheduleDetail.Tsd_Status = 'A'
                                                and T_TaxScheduleDetail.Tsd_PayPeriod = @TaxHeaderPayPeriod
                                                and T_TaxScheduleDetail.Tsd_TaxCode = IsNull({0}.Epc_TaxCode,'S') 
                                                and T_TaxScheduleDetail.Tsd_BracketAmt < ({0}.Epc_GrossPayAmt - ({0}.Epc_SSSEmployeeShare + 
                                                {0}.Epc_PhilhealthEmployeeShare + {0}.Epc_HDMFEmployeeShare))
                                                ORDER BY T_TaxScheduleDetail.Tsd_BracketAmt DESC), 0)
                                                WHERE {0}.Epc_EmployeeId = @EmployeeID", TableName);
            #endregion

            dalHelper.ExecuteNonQuery(sqlQuery, CommandType.Text, Param);
        }

        public DataTable getEmployeeListingWithBonus(string PaypPeriod)
        {
            try
            {
                return dalHelper.ExecuteDataSet(@"
                                SELECT Emt_Lastname
                                , Emt_Firstname
                                , Ebn_EmployeeID
                                , Emt_Nickname
                                , Convert(char(10),Emt_Hiredate, 101) as HireDate
                                , Ebn_SalaryRate
                                , (Case when Year(Emt_HireDate)= '" + PaypPeriod.Substring(0, 4) + @"' then 
							                                (12-Month(Emt_HireDate))+ 1
					                                  when Year(Emt_HireDate)< '" + PaypPeriod.Substring(0, 4) + @"' then 
							                                12
					                                   else
							                                0
					                                   end) as Months
                                , Ebn_BonusAmt
                                , Ebn_NonTaxAmt
                                , Ebn_TaxAmt
                                 FROM T_EmployeeBonus
                                INNER JOIN T_EmployeeMaster on Emt_Employeeid = Ebn_EmployeeID
                                WHERE Ebn_Payperiod = '" + PaypPeriod + @"'
                                ORDER BY  Emt_Lastname
                                , Emt_Firstname").Tables[0];
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public DataSet GetHeaderForExcel()
        {
            DataSet ds = new DataSet();

            #region Other Fields for Header
            string sql = @"
                                SELECT 'LAST NAME'
                                , 'FIRST NAME'
                                , 'EMPLOYEE ID'
                                , 'NICKNAME'
                                , 'HIRE DATE'
                                , 'SALARY RATE'
                                , 'MONTHS'
                                , 'BONUS AMOUNT'
                                , 'NON TAX AMOUNT'
                                , 'TAX AMOUNT'
                        ";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sql, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        //MODIFIED BY KEVIN 04072009 - ADDING 2 PARAMETERS FOR PAY RATE
        public void CalculateLaborHoursAdjustment(string colRate, string colPayRate, int iDayCodeCnt)
        {
            DataSet dsPremuims = dsFetch(string.Format(CommonConstants.Queries.checkRequiredConstant, colRate, colPayRate));

            ParameterInfo[] paramInfo = new ParameterInfo[22];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@NGTPrem", Convert.ToDecimal(dsPremuims.Tables[0].Rows[0][0]) / 100);
            paramInfo[2] = new ParameterInfo("@Reg", Convert.ToDecimal(dsPremuims.Tables[1].Rows[0][0]) / 100);
            paramInfo[3] = new ParameterInfo("@RegOT", Convert.ToDecimal(dsPremuims.Tables[1].Rows[0][1]) / 100);
            paramInfo[4] = new ParameterInfo("@Rest", Convert.ToDecimal(dsPremuims.Tables[2].Rows[0][0]) / 100);
            paramInfo[5] = new ParameterInfo("@RestOT", Convert.ToDecimal(dsPremuims.Tables[2].Rows[0][1]) / 100);
            paramInfo[6] = new ParameterInfo("@Hol", Convert.ToDecimal(dsPremuims.Tables[3].Rows[0][0]) / 100);
            paramInfo[7] = new ParameterInfo("@HolOT", Convert.ToDecimal(dsPremuims.Tables[3].Rows[0][1]) / 100);
            paramInfo[8] = new ParameterInfo("@SPL", Convert.ToDecimal(dsPremuims.Tables[4].Rows[0][0]) / 100);
            paramInfo[9] = new ParameterInfo("@SPLOT", Convert.ToDecimal(dsPremuims.Tables[4].Rows[0][1]) / 100);
            paramInfo[10] = new ParameterInfo("@PSD", Convert.ToDecimal(dsPremuims.Tables[5].Rows[0][0]) / 100);
            paramInfo[11] = new ParameterInfo("@PSDOT", Convert.ToDecimal(dsPremuims.Tables[5].Rows[0][1]) / 100);
            paramInfo[12] = new ParameterInfo("@Comp", Convert.ToDecimal(dsPremuims.Tables[6].Rows[0][0]) / 100);
            paramInfo[13] = new ParameterInfo("@CompOT", Convert.ToDecimal(dsPremuims.Tables[6].Rows[0][1]) / 100);
            paramInfo[14] = new ParameterInfo("@RestHol", Convert.ToDecimal(dsPremuims.Tables[7].Rows[0][0]) / 100);
            paramInfo[15] = new ParameterInfo("@RestHolOT", Convert.ToDecimal(dsPremuims.Tables[7].Rows[0][1]) / 100);
            paramInfo[16] = new ParameterInfo("@RestSPL", Convert.ToDecimal(dsPremuims.Tables[8].Rows[0][0]) / 100);
            paramInfo[17] = new ParameterInfo("@RestSPLOT", Convert.ToDecimal(dsPremuims.Tables[8].Rows[0][1]) / 100);
            paramInfo[18] = new ParameterInfo("@RestComp", Convert.ToDecimal(dsPremuims.Tables[9].Rows[0][0]) / 100);
            paramInfo[19] = new ParameterInfo("@RestCompOT", Convert.ToDecimal(dsPremuims.Tables[9].Rows[0][1]) / 100);
            paramInfo[20] = new ParameterInfo("@RestPSD", Convert.ToDecimal(dsPremuims.Tables[10].Rows[0][0]) / 100);
            paramInfo[21] = new ParameterInfo("@RestPSDOT", Convert.ToDecimal(dsPremuims.Tables[10].Rows[0][1]) / 100);

            dalHelper.ExecuteNonQuery(CommonConstants.Queries.updateLaborHoursAmt, CommandType.Text, paramInfo); //Modified By Rendell Uy - 1/28/2011 (Get adjustment amount of records with Ell_AssumedPostback != 'A' only)

            //REG5 Incorporation; Added by Rendell Uy - 06/17/2010
            if (iDayCodeCnt > 0)
            {
                string strQueryDayCodeFillers = @"SELECT Dcf_FillerSeq, Dcf_DayCode FROM t_DayCodeFiller WHERE Dcf_Status = 'A'";

                DataTable dtResult;
                dtResult = dalHelper.ExecuteDataSet(strQueryDayCodeFillers).Tables[0];

                foreach (DataRow drFiller in dtResult.Rows)
                {
                    DataTable dtPremiums = GetPremiumForFillerDayCode(Convert.ToString(drFiller["Dcf_DayCode"]));

                    string strQuery = string.Format(@"UPDATE T_LaborHrsAdjustment
                                                    SET Lha_LaborHrsAdjustmentAmt = Lha_LaborHrsAdjustmentAmt +
                                                        (B.Lha_Filler{0}_Hr * A.Lha_HourlyRate * {1}) 
                                                        + (B.Lha_Filler{0}_OTHr * A.Lha_HourlyRate * {2})
                                                        + (B.Lha_Filler{0}_NDHr * A.Lha_HourlyRate * {1} * @NGTPrem)
                                                        + (B.Lha_Filler{0}_OTNDHr * A.Lha_HourlyRate * {2} * @NGTPrem) 
                                                    FROM T_LaborHrsAdjustment A
                                                    INNER JOIN T_LaborHrsAdjustmentExt B
                                                    ON A.Lha_EmployeeId = B.Lha_EmployeeId
                                                        AND A.Lha_AdjustpayPeriod = B.Lha_AdjustpayPeriod
                                                        AND A.Lha_PayPeriod = B.Lha_PayPeriod
                                                        AND A.Lha_ProcessDate = B.Lha_ProcessDate
                                                    INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
	                                                        and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
	                                                        and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod 
                                                    WHERE A.Lha_AdjustpayPeriod = @CurPayPeriod
                                                    AND ISNULL(T_EmployeeLogLedgerTrail.Ell_AssumedPostBack, '') != 'A' --Added By Rendell Uy - 1/28/2011"
                                                    , drFiller["Dcf_FillerSeq"]
                                                    , Convert.ToDecimal(dtPremiums.Rows[0]["Dpm_RegularHourPremium"].ToString()) / 100
                                                    , Convert.ToDecimal(dtPremiums.Rows[0]["Dpm_OvertimeHourPremium"].ToString()) / 100);

                    dalHelper.ExecuteNonQuery(strQuery, CommandType.Text, paramInfo);
                }
            }
        }

        public DataTable GetPremiumForFillerDayCode(string strDayCode)
        {
            ParameterInfo[] paramDayCode = new ParameterInfo[1];
            paramDayCode[0] = new ParameterInfo("@DayCode", strDayCode);

            string strQuery = @"SELECT Dpm_RegularHourPremium, Dpm_OvertimeHourPremium 
                                FROM T_DayPremiumMaster
                                WHERE Dpm_Daycode = @DayCode";

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery, CommandType.Text, paramDayCode).Tables[0];

            return dtResult;
        }

        public void CreateDefaultOTApplications(string PayPeriod, string OvertimeFlag)
        {
            try
            {
                //Get all data needed for OT application
                DataTable dtEmployeeResult = GetDefaultOTCandidateInsertRecords(PayPeriod);

                if (dtEmployeeResult.Rows.Count > 0)
                {
                    //Get last increment of DFOVERTIME
                    string sqlDFOVERTIMEIncrementQuery = @"SELECT Tcm_LastSeries 
                                                           FROM T_TransactionControlMaster
                                                           WITH (UPDLOCK)
                                                           WHERE Tcm_TransactionCode = 'DFOVERTIME'";
                    DataTable dtResult;
                    dtResult = dalHelper.ExecuteDataSet(sqlDFOVERTIMEIncrementQuery).Tables[0];
                    int iSeries = Convert.ToInt32(dtResult.Rows[0][0].ToString());

                    //Update last increment of DFOVERTIME
                    sqlDFOVERTIMEIncrementQuery = string.Format(@"UPDATE T_TransactionControlMaster
                                                                  SET Tcm_LastSeries = {0}
                                                                    , ludatetime = getdate()
                                                                  WHERE Tcm_TransactionCode = 'DFOVERTIME'", iSeries + dtEmployeeResult.Rows.Count);
                    InsertUpdate(sqlDFOVERTIMEIncrementQuery);

                    //Get company current year
                    string sqlCompanyCurYearQuery = @"SELECT Ccd_CurrentYear FROM T_CompanyMaster";
                    dtResult = dalHelper.ExecuteDataSet(sqlCompanyCurYearQuery).Tables[0];
                    string strCompanyYear = dtResult.Rows[0][0].ToString().Substring(2, 2);

                    foreach (DataRow drEmployee in dtEmployeeResult.Rows)
                    {
                        string strStartTime;
                        string strEndTime;
                        double dOvertimeHour;
                        double dDefaultOvertimeHour = Convert.ToDouble(drEmployee["Gdo_OTHours"].ToString());

                        if (drEmployee["Cal_WorkCode"].ToString().Trim().Length >= 2)
                        {
                            strStartTime = drEmployee["Scm_ShiftTimeIn"].ToString();
                            strEndTime = AddMinutesToHourStr(drEmployee["Scm_ShiftTimeOut"].ToString(), Convert.ToInt32(dDefaultOvertimeHour * 60));
                            dOvertimeHour = Convert.ToDouble(drEmployee["Scm_ShiftHours"].ToString()) + dDefaultOvertimeHour;
                        }
                        else
                        {
                            strStartTime = drEmployee["Scm_ShiftTimeOut"].ToString();
                            strEndTime = AddMinutesToHourStr(drEmployee["Scm_ShiftTimeOut"].ToString(), Convert.ToInt32(dDefaultOvertimeHour * 60));
                            dOvertimeHour = dDefaultOvertimeHour;
                        }

                        //Increment control value
                        iSeries++;
                        string strControlNo = "D" + strCompanyYear + string.Format("{0:000000000}", iSeries);

                        if (!CheckIfExistsOvertimeApplication(drEmployee["Ell_EmployeeId"].ToString()
                                                                , drEmployee["Ell_ProcessDate"].ToString()
                                                                , strStartTime
                                                                , strEndTime
                                                                , "T_EmployeeOvertime"))
                        {
                            //Create OT application per employee per day
                            #region Insert into Employee Overtime table
                            string sqlCreateOTPermitsQuery = string.Format(@"INSERT INTO T_EmployeeOvertime
                                                                        ( Eot_CurrentPayPeriod
                                                                        , Eot_EmployeeId
                                                                        , Eot_OvertimeDate
                                                                        , Eot_Seqno
                                                                        , Eot_AppliedDate
                                                                        , Eot_OvertimeType
                                                                        , Eot_StartTime
                                                                        , Eot_EndTime
                                                                        , Eot_OvertimeHour
                                                                        , Eot_Reason
                                                                        , Eot_JobCode
                                                                        , Eot_ClientJobNo
                                                                        , Eot_EndorsedDateToChecker
                                                                        , Eot_CheckedBy
                                                                        , Eot_CheckedDate
                                                                        , Eot_Checked2By
                                                                        , Eot_Checked2Date
                                                                        , Eot_ApprovedBy
                                                                        , Eot_ApprovedDate
                                                                        , Eot_Status
                                                                        , Eot_ControlNo
                                                                        , Eot_OvertimeFlag
                                                                        , Eot_Costcenter
                                                                        , Eot_Filler1
                                                                        , Eot_Filler2
                                                                        , Eot_Filler3
                                                                        , Eot_BatchNo
                                                                        , Usr_Login
                                                                        , Ludatetime )
                                                                          VALUES( '{0}'
                                                                            , '{1}'
                                                                            , '{2}'
                                                                            , '01'
                                                                            , Getdate()
                                                                            , 'P'
                                                                            , '{3}'
                                                                            , '{4}'
                                                                            , {5}
                                                                            , 'DEFAULT OT'
                                                                            , ''
                                                                            , ''
                                                                            , Getdate()
                                                                            , 'sa'
                                                                            , Getdate()
                                                                            , 'sa'
                                                                            , Getdate()
                                                                            , 'sa'
                                                                            , Getdate()
                                                                            , '9'
                                                                            , '{6}'
                                                                            , '{7}'
                                                                            , '{8}'
                                                                            , ''
                                                                            , ''
                                                                            , ''
                                                                            , ''
                                                                            , 'sa'
                                                                            , Getdate() )"
                                                                                , PayPeriod
                                                                                , drEmployee["Ell_EmployeeId"].ToString()
                                                                                , drEmployee["Ell_ProcessDate"].ToString()
                                                                                , strStartTime
                                                                                , strEndTime
                                                                                , dOvertimeHour
                                                                                , strControlNo
                                                                                , OvertimeFlag
                                                                                , drEmployee["Emt_CostCenterCode"].ToString());
                            InsertUpdate(sqlCreateOTPermitsQuery);
                            #endregion

                            //Update log ledger table
                            string sqlUpdateLogLedgerQuery = string.Format(@"UPDATE T_EmployeeLogLedger 
                                                                         SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr + {0}
                                                                            , ludatetime = getdate()
                                                                         WHERE Ell_EmployeeId = '{1}'
                                                                            AND Ell_ProcessDate = '{2}'"
                                                                                    , dOvertimeHour
                                                                                    , drEmployee["Ell_EmployeeId"].ToString()
                                                                                    , drEmployee["Ell_ProcessDate"].ToString());
                            InsertUpdate(sqlUpdateLogLedgerQuery);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new PayrollException("Error: " + err.Message);
            }
        }

        public void RemoveDefaultOTApplications(string PayPeriod)
        {
            try
            {
                //Get all default OT records
                DataTable dtOTApps = GetDefaultOTCandidateDeleteRecords(PayPeriod);

                if (dtOTApps.Rows.Count > 0)
                {
                    foreach (DataRow dtOTApp in dtOTApps.Rows)
                    {
                        //Update log ledger table
                        string sqlUpdateLogLedgerQuery = string.Format(@"UPDATE T_EmployeeLogLedger 
                                                                         SET Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr - {0}
                                                                            , ludatetime = getdate()
                                                                         WHERE Ell_EmployeeId = '{1}'
                                                                            AND Ell_ProcessDate = '{2}'"
                                                                                , dtOTApp["Eot_OvertimeHour"].ToString()
                                                                                , dtOTApp["Eot_EmployeeId"].ToString()
                                                                                , dtOTApp["Eot_OvertimeDate"].ToString());
                        InsertUpdate(sqlUpdateLogLedgerQuery);
                    }
                }

                //Delete OT application per employee per day
                string sqlDeleteOTPermitsQuery = string.Format(@"DELETE FROM T_EmployeeOvertime WHERE LEFT(Eot_ControlNo, 1) = 'D' AND Eot_CurrentPayPeriod = '{0}'", PayPeriod);
                InsertUpdate(sqlDeleteOTPermitsQuery);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public DataTable GetDefaultOTCandidateInsertRecords(string PayPeriod)
        {
            string sqlGetLogLedgerDataQuery = string.Format(@"SELECT Ell_EmployeeId
                                                                    , Ell_ProcessDate
                                                                    , Ell_DayCode
                                                                    , Ell_EncodedOvertimePostHr
                                                                    , Gdo_OTHours
                                                                    , Scm_ShiftTimeIn
                                                                    , Scm_ShiftTimeOut
                                                                    , Scm_ShiftHours
                                                                    , Emt_CostCenterCode
                                                                    , Cal_WorkCode
                                                                    , Scm_ScheduleType
                                                                FROM T_EmployeeLogLedger
                                                                INNER JOIN T_GroupDefaultOT ON Gdo_WorkType = Ell_Worktype
                                                                INNER JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode
                                                                INNER JOIN T_EmployeeMaster ON Emt_EmployeeId = Ell_EmployeeId
                                                                INNER JOIN T_CalendarGroupTmp ON Cal_WorkType = Ell_WorkType AND Cal_WorkGroup = Ell_WorkGroup AND Cal_ProcessDate = Ell_ProcessDate
                                                                WHERE Ell_PayPeriod = '{0}'
                                                                    AND Gdo_Status = 'A'
                                                                    AND Gdo_OTHours > 0
                                                                    AND Ell_RestDay = 0
                                                                    AND Ell_Holiday = 0
                                                                    AND Scm_ShiftCode != 'N001'
                                                                UNION
                                                                SELECT Ell_EmployeeId
                                                                    , Ell_ProcessDate
                                                                    , Ell_DayCode
                                                                    , Ell_EncodedOvertimePostHr
                                                                    , ISNULL(Gdo_OTHours, 0)
                                                                    , Scm_ShiftTimeIn
                                                                    , Scm_ShiftTimeOut
                                                                    , Scm_ShiftHours
                                                                    , Emt_CostCenterCode
                                                                    , 'D8' --temp
                                                                    , 'D'
                                                                FROM T_EmployeeLogLedger
                                                                INNER JOIN T_GroupDefaultOT ON Gdo_WorkType = Ell_Worktype
                                                                INNER JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode
                                                                INNER JOIN T_EmployeeMaster ON Emt_EmployeeId = Ell_EmployeeId
                                                                WHERE Ell_PayPeriod = '{0}' 
																	AND Gdo_Status = 'A'
                                                                    AND Gdo_OTHours > 0
                                                                    AND Ell_Holiday = 1 
                                                                    AND Ell_RestDay = 0
                                                                    AND Scm_ShiftCode != 'N001'", PayPeriod);
            DataTable dtEmployeeResult;
            dtEmployeeResult = dalHelper.ExecuteDataSet(sqlGetLogLedgerDataQuery).Tables[0];

            return dtEmployeeResult;
        }

        public DataTable GetDefaultOTCandidateDeleteRecords(string PayPeriod)
        {
            string sqlGetLogLedgerDataQuery = string.Format(@"SELECT * 
                                                                FROM T_EmployeeOvertime 
                                                                WHERE LEFT(Eot_ControlNo, 1) = 'D' 
                                                                    AND Eot_CurrentPayPeriod = '{0}'
                                                                    AND Eot_Status IN ('9', 'A')", PayPeriod);
            DataTable dtEmployeeResult;
            dtEmployeeResult = dalHelper.ExecuteDataSet(sqlGetLogLedgerDataQuery).Tables[0];

            return dtEmployeeResult;
        }
        #endregion

    }
}
