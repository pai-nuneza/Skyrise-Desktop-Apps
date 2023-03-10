using System;
using Payroll.DAL;
using CommonLibrary;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

namespace Payroll.BLogic
{
    public class SystemCycleProcessingBL
    {
        #region <Variables>
        DALHelper dalHelper;
        String PayPeriod;
        String UserCode;
        String CompanyCode;
        String CentralProfile;

        #endregion

        #region <Constructors>
        public SystemCycleProcessingBL()
        {
            this.PayPeriod = string.Empty;
            this.UserCode = LoginInfo.getUser().UserCode;
            this.CompanyCode = LoginInfo.getUser().CompanyCode;
            this.CentralProfile = LoginInfo.getUser().CentralProfileName;
            this.dalHelper = new DALHelper();            
        }

        public SystemCycleProcessingBL(DALHelper dal, string payPeriod)
        {
            this.dalHelper = dal;
            this.PayPeriod = payPeriod;
            this.UserCode = LoginInfo.getUser().UserCode;
            this.CompanyCode = LoginInfo.getUser().CompanyCode;
            this.CentralProfile = LoginInfo.getUser().CentralProfileName;
        }

        public SystemCycleProcessingBL(DALHelper dal, string payPeriod, string userCode, string companyCode, string centralProfile)
        {
            this.dalHelper = dal;
            this.PayPeriod = payPeriod;
            this.UserCode = userCode;
            this.CompanyCode = companyCode;
            this.CentralProfile = centralProfile;
        }
        #endregion

        #region SELECT/Insert/Update/Count
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
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(err);
                CommonProcedures.ShowMessage(messageCode, "");
                return null;
            }
        }

        public void InsertUpdate(string sqlInsertUpdateQuery)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(error);
                CommonProcedures.ShowMessage(messageCode, "");
            }
        }

        public void InsertUpdate(string sqlInsertUpdateQuery, string PayPeriod2)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod2);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(error);
                CommonProcedures.ShowMessage(messageCode, "");
            }
        }


        public void InsertUpdate(string sqlInsertUpdateQuery, string PayPeriod, string PAYFREQNCY, string STARTYEAR, string NEWYEAR)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[1] = new ParameterInfo("@PAYFREQNCY", PAYFREQNCY);
            paramInfo[2] = new ParameterInfo("@STARTYEAR", STARTYEAR);
            //paramInfo[3] = new ParameterInfo("@NEWYEAR", NEWYEAR);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(error);
                CommonProcedures.ShowMessage(messageCode, "");
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
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(err);
                CommonProcedures.ShowMessage(messageCode, "");
                return null;
            }

        }

        public void InsertUpdate(string sqlInsertUpdateQuery, string NextPayPeriodStartDate, string NextPayPeriodEndDate, string NextPayPeriod)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@UserLogin", UserCode);
            paramInfo[2] = new ParameterInfo("@NextPayPeriod", NextPayPeriod);
            paramInfo[3] = new ParameterInfo("@NextPayPeriodStartDate", NextPayPeriodStartDate);
            paramInfo[4] = new ParameterInfo("@NextPayPeriodEndDate", NextPayPeriodEndDate);
            paramInfo[5] = new ParameterInfo("@CompanyCode", CompanyCode);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(error);
                CommonProcedures.ShowMessage(messageCode, "");
            }
        }

        public void InsertUpdate(string sqlInsertUpdateQuery, DataRow row)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[10];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", row["Tps_PayCycle"], SqlDbType.Char, 7);
            paramInfo[1] = new ParameterInfo("@Tps_CycleIndicator", row["Tps_CycleIndicator"], SqlDbType.Char, 1);
            paramInfo[2] = new ParameterInfo("@Tps_CreatedBy", row["Tps_CreatedBy"], SqlDbType.Char, 15);
            paramInfo[3] = new ParameterInfo("@Tps_Remarks", row["Tps_Remarks"], SqlDbType.VarChar, 200);
            paramInfo[4] = new ParameterInfo("@Tps_ComputeTax", row["Tps_ComputeTax"]);
            paramInfo[5] = new ParameterInfo("@Tps_TaxSchedule", row["Tps_TaxSchedule"]);
            paramInfo[6] = new ParameterInfo("@Tps_CycleIndicatorSpecial", row["Tps_CycleIndicatorSpecial"]);
            paramInfo[7] = new ParameterInfo("@Tps_CycleType", row["Tps_CycleType"]);
            paramInfo[8] = new ParameterInfo("@Tps_TaxComputation", row["Tps_TaxComputation"]);
            paramInfo[9] = new ParameterInfo("@Tps_PayDate", row["Tps_PayDate"], SqlDbType.Date);

            try
            {
                dalHelper.ExecuteNonQuery(sqlInsertUpdateQuery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(error);
                CommonProcedures.ShowMessage(messageCode, "");
            }
        }

        public int Update(DataRow row)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", row["Tps_PayCycle"], SqlDbType.Char, 7);
            paramInfo[1] = new ParameterInfo("@Tps_CycleIndicator", row["Tps_CycleIndicator"], SqlDbType.Char, 1);
            paramInfo[2] = new ParameterInfo("@Tps_RecordStatus", row["Tps_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[3] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);
            paramInfo[4] = new ParameterInfo("@Tps_Remarks", row["Tps_Remarks"], SqlDbType.VarChar, 200);
            paramInfo[5] = new ParameterInfo("@Tps_ComputeTax", row["Tps_ComputeTax"]);
            paramInfo[6] = new ParameterInfo("@Tps_TaxSchedule", row["Tps_TaxSchedule"]);
            paramInfo[7] = new ParameterInfo("@Tps_CycleIndicatorSpecial", row["Tps_CycleIndicatorSpecial"]);
            paramInfo[8] = new ParameterInfo("@Tps_CycleType", row["Tps_CycleType"]);
            paramInfo[9] = new ParameterInfo("@Tps_TaxComputation", row["Tps_TaxComputation"]);
            paramInfo[10] = new ParameterInfo("@Tps_PayDate", row["Tps_PayDate"], SqlDbType.Date);
            paramInfo[11] = new ParameterInfo("@CycleDate", DateTime.Now, SqlDbType.Date);

            string sqlquery = @"UPDATE T_PaySchedule 
                                               SET Tps_StartCycle = @CycleDate,
                                                 Tps_EndCycle = @CycleDate,
                                                 Tps_CycleIndicator = @Tps_CycleIndicator,
                                                 Tps_RecordStatus = @Tps_RecordStatus,
                                                 Usr_Login = @Usr_Login, 
                                                 Ludatetime = GETDATE(),
                                                 Tps_Remarks = @Tps_Remarks,
                                                 Tps_ComputeTax =@Tps_ComputeTax,
                                                 Tps_ComputeSSS =0,
                                                 Tps_ComputePH =0,
                                                 Tps_ComputePagIbig =0,
                                                 Tps_MonthEnd =0,
                                                 Tps_TaxSchedule =@Tps_TaxSchedule,
                                                 Tps_CycleIndicatorSpecial =@Tps_CycleIndicatorSpecial,
                                                 Tps_CycleType = @Tps_CycleType,
                                                 Tps_TaxComputation = @Tps_TaxComputation,
                                                 Tps_PayDate = @Tps_PayDate
                                                WHERE Tps_PayCycle = @Tps_PayCycle";

            try
            {
                retVal = dalHelper.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
            }
            catch (Exception error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(error);
                CommonProcedures.ShowMessage(messageCode, "");
            }

            return retVal;
        }

        public DataSet dsFetch(string sqlQuery)
        {
            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[2];
                paramInfo[0] = new ParameterInfo("@CurPayPeriod", PayPeriod);
                paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

                return dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception err)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(err);
                CommonProcedures.ShowMessage(messageCode, "");
                throw;
            }
        }
        #endregion

        #region Active Employee List

        public DataTable GetActiveEmployeeList(String EmployeeID)
        {
            try
            {
                return dalHelper.ExecuteDataSet(string.Format(@"SELECT	Mem_IDNo, 
                                                                        Mem_ShiftCode, 
                                                                        Mem_WorkLocationCode, 
                                                                        Msh_Schedule,
                                                                        Msh_ShiftIn1 [In1],
                                                                        Msh_ShiftOut1 [Out1],
                                                                        Msh_ShiftIn2 [In2],
                                                                        Msh_ShiftOut2 [Out2],
                                                                        Mem_IntakeDate,
                                                                        Mem_CalendarType,
                                                                        Mem_CalendarGroup,
                                                                        Mem_PayrollType
                                                                FROM	M_Employee A
                                                                LEFT JOIN	M_Shift ON Msh_ShiftCode = Mem_ShiftCode		
                                                                Where LEFT(Mem_WorkStatus, 1) <> 'I'
                                                                AND		Mem_IDNo = '{0}'", EmployeeID)).Tables[0];
            }
            catch (Exception err)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(err);
                CommonProcedures.ShowMessage(messageCode, "");
                return null;
            }
        }

        public DataTable GetActiveEmployeeList(String EmployeeID, string DBName)
        {
            try
            {
                return dalHelper.ExecuteDataSet(string.Format(@"SELECT	Mem_IDNo, 
                                                                        Mem_ShiftCode, 
                                                                        Mem_WorkLocationCode, 
                                                                        Msh_Schedule,
                                                                        Msh_ShiftIn1 [In1],
                                                                        Msh_ShiftOut1 [Out1],
                                                                        Msh_ShiftIn2 [In2],
                                                                        Msh_ShiftOut2 [Out2],
                                                                        Mem_IntakeDate,
                                                                        Mem_CalendarType,
                                                                        Mem_CalendarGroup,
                                                                        Mem_PayrollType
                                                                FROM	{1}..M_Employee A
                                                                LEFT JOIN {1}..M_Shift ON Msh_ShiftCode = Mem_ShiftCode		
                                                                Where	Mem_IDNo = '{0}'", EmployeeID, DBName)).Tables[0];
            }
            catch (Exception err)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(err);
                CommonProcedures.ShowMessage(messageCode, "");
                return null;
            }
        }

        #endregion

        #region Default Values
        public DataTable GetDefaultEmploymentStatus()
        {
            string query = string.Format(@"SELECT STUFF( (SELECT ',' + Mpd_SubCode 
                                                    FROM M_PolicyDtl 
				                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                    AND Mpd_CompanyCode = '{0}'
				                                    AND Mpd_ParamValue = 1
                                                    FOR XML PATH('')), 
                                            1, 1, '')
                                            , STUFF( (SELECT ',' + Mpd_SubName 
                                                    FROM M_PolicyDtl 
				                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                    AND Mpd_CompanyCode = '{0}'
				                                    AND Mpd_ParamValue = 1
                                                    FOR XML PATH('')), 
                                            1, 1, '')", CompanyCode);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        #endregion

        #region Log Ledger Correction
        public void GenerateLogLedgerRecord(string EmployeeList, string PayPeriod, string UserCode)
        {
            #region [new query]
            string sqlInsertQuery = string.Format(@"DECLARE @currentdate datetime
                                        DECLARE @enddate datetime
                                        DECLARE @PAYPERIODDATE as table (currentdate datetime null)
                                        DECLARE @POCKETSIZE AS INT = (SELECT Mph_NumValue FROM M_PolicyHdr WHERE Mph_PolicyCode = 'POCKETSIZE' AND Mph_CompanyCode = '{2}')

                                        SELECT @currentdate = Tps_StartCycle, @enddate = Tps_EndCycle 
                                        FROM T_PaySchedule
                                        WHERE Tps_PayCycle = @Ttr_PayCycle
                                        WHILE(@currentdate <= @enddate)
                                        BEGIN
                                            INSERT INTO @PAYPERIODDATE
                                            SELECT @currentdate

                                            SELECT  @currentdate = DATEADD(d, 1, @currentdate)
                                        END

                                        INSERT INTO T_EmpTimeRegister
                                        (     Ttr_IDNo
                                            , Ttr_Date
                                            , Ttr_PayCycle
                                            , Ttr_DayCode
                                            , Ttr_ShiftCode
                                            , Ttr_HolidayFlag
                                            , Ttr_RestDayFlag
                                            , Ttr_ActIn_1
                                            , Ttr_ActOut_1
                                            , Ttr_ActIn_2
                                            , Ttr_ActOut_2
                                            , Ttr_WFPayLVCode
                                            , Ttr_WFPayLVHr
                                            , Ttr_PayLVMin
                                            , Ttr_ExcLVMin
                                            , Ttr_WFNoPayLVCode
                                            , Ttr_WFNoPayLVHr
                                            , Ttr_NoPayLVMin
                                            , Ttr_WFOTAdvHr
                                            , Ttr_WFOTPostHr
                                            , Ttr_OTMin
                                            , Ttr_CompOTMin
                                            , Ttr_OffsetOTMin
                                            , Ttr_WFTimeMod
                                            , Ttr_WFFlexTime
                                            , Ttr_Amnesty
                                            , Ttr_SkipService
                                            , Ttr_SkipServiceBy
                                            , Ttr_SkipServiceDate
                                            , Ttr_AssumedFlag
                                            , Ttr_AssumedBy
                                            , Ttr_AssumedDate
                                            , Ttr_AssumedPost
                                            , Ttr_ConvIn_1Min
                                            , Ttr_ConvOut_1Min
                                            , Ttr_ConvIn_2Min
                                            , Ttr_ConvOut_2Min
                                            , Ttr_CompIn_1Min
                                            , Ttr_CompOut_1Min
                                            , Ttr_CompIn_2Min
                                            , Ttr_CompOut_2Min
                                            , Ttr_CompAdvOTMin
                                            , Ttr_ShiftIn_1Min
                                            , Ttr_ShiftOut_1Min
                                            , Ttr_ShiftIn_2Min
                                            , Ttr_ShiftOut_2Min
                                            , Ttr_ShiftMin
                                            , Ttr_ScheduleType
                                            , Ttr_ActLT1Min
                                            , Ttr_ActLT2Min
                                            , Ttr_CompLT1Min
                                            , Ttr_CompLT2Min
                                            , Ttr_ActUT1Min
                                            , Ttr_ActUT2Min
                                            , Ttr_CompUT1Min
                                            , Ttr_CompUT2Min
                                            , Ttr_InitialABSMin
                                            , Ttr_CompABSMin
                                            , Ttr_CompREGMin
                                            , Ttr_CompWorkMin
                                            , Ttr_CompNDMin
                                            , Ttr_CompNDOTMin
                                            , Ttr_PrvDayWorkMin
                                            , Ttr_PrvDayHolRef
                                            , Ttr_PDHOLHour
                                            , Ttr_PDRESTLEGHOLDay
                                            , Ttr_WorkDay
                                            , Ttr_EXPHour
                                            , Ttr_ABSHour
                                            , Ttr_REGHour
                                            , Ttr_OTHour
                                            , Ttr_NDHour
                                            , Ttr_NDOTHour
                                            , Ttr_LVHour
                                            , Ttr_PaidBreakHour
                                            , Ttr_OBHour
                                            , Ttr_RegPlusHour
                                            , Ttr_TBAmt01
                                            , Ttr_TBAmt02
                                            , Ttr_TBAmt03
                                            , Ttr_TBAmt04
                                            , Ttr_TBAmt05
                                            , Ttr_TBAmt06
                                            , Ttr_TBAmt07
                                            , Ttr_TBAmt08
                                            , Ttr_TBAmt09
                                            , Ttr_TBAmt10
                                            , Ttr_TBAmt11
                                            , Ttr_TBAmt12
                                            , Ttr_WorkLocationCode
                                            , Ttr_CalendarGroup
                                            , Ttr_PremiumGrpCode
                                            , Ttr_PayrollGroup
                                            , Ttr_CostcenterCode
                                            , Ttr_EmploymentStatusCode
                                            , Ttr_PayrollType
                                            , Ttr_Grade
                                            , Usr_Login
                                            , Ludatetime )

                                        SELECT Mem_IDNo
                                                 ,currentdate
                                                 ,@Ttr_PayCycle
                                                 ,'REG'
                                                 ,Mem_ShiftCode
                                                 ,'False'
                                                 ,'False'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
                                                 ,'0000'
												 ,''  		---Ttr_WFPayLVCode
                                                 ,0.00		---Ttr_WFPayLVHr
                                                 ,0			---Ttr_PayLVMin
                                                 ,0			---Ttr_ExcLVMin
                                                 ,''		---Ttr_WFNoPayLVCode
                                                 ,0.00		---Ttr_WFNoPayLVHr
                                                 ,0			---Ttr_NoPayLVMin
                                                 ,0.00		---Ttr_WFOTAdvHr
                                                 ,0.00		---Ttr_WFOTPostHr
												 ,0			---Ttr_OTMin
                                                 ,0			---Ttr_CompOTMin
                                                 ,0			---Ttr_OffsetOTMin
												 ,'N'		---Ttr_WFTimeMod
                                                 ,'N'		---Ttr_WFFlexTime
                                                 ,'NA'		---Ttr_Amnesty
												 ,'N' 	    ---Ttr_SkipService
                                                 ,NULL		---Ttr_SkipServiceBy
                                                 ,NULL		---Ttr_SkipServiceDate
                                                 ,'False'	---Ttr_AssumedFlag
                                                 ,NULL		---Ttr_AssumedBy
                                                 ,NULL		---Ttr_AssumedDate
												 ,'N' 		---Ttr_AssumedPost
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,0
                                                 ,Msh_ShiftIn1
                                                 ,Msh_ShiftOut1
                                                 ,Msh_ShiftIn2
                                                 ,Msh_ShiftOut2
                                                 ,0			---Ttr_ShiftMin
                                                 ,Msh_Schedule
                                                 ,0 		---Ttr_ActLT1Min
                                                 ,0			---Ttr_ActLT2Min
                                                 ,0			---Ttr_CompLT1Min
												 ,0 		---Ttr_CompLT2Min
												 ,0 		---Ttr_ActUT1Min
                                                 ,0			---Ttr_ActUT2Min
                                                 ,0 		---Ttr_CompUT1Min
                                                 ,0 		---Ttr_CompUT2Min
                                                 ,0			---Ttr_InitialABSMin
                                                 ,0			---Ttr_CompABSMin
                                                 ,0			---Ttr_CompREGMin
                                                 ,0			---Ttr_CompWorkMin
                                                 ,0			---Ttr_CompNDMin
                                                 ,0			---Ttr_CompNDOTMin
                                                 ,0			---Ttr_PrvDayWorkMin
                                                 ,NULL		---Ttr_PrvDayHolRef
												 ,0.00		---Ttr_PDHOLHour
												 ,0 		---Ttr_PDRESTLEGHOLDay
                                                 ,0 		---Ttr_WorkDay
                                                 ,0.00 		---Ttr_EXPHour
                                                 ,0.00		---Ttr_ABSHour
                                                 ,0.00 		---Ttr_REGHour
                                                 ,0.00 		---Ttr_OTHour
                                                 ,0.00 		---Ttr_NDHour
                                                 ,0.00 		---Ttr_NDOTHour
                                                 ,0.00 		---Ttr_LVHour
                                                 ,0.00		---Ttr_PaidBreakHour
												 ,0.00 		---Ttr_OBHour
												 ,0.00 		---Ttr_RegPlusHour
												 
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
												 ,Mem_WorkLocationCode
												 ,Mem_CalendarGrpCode
												 ,Mem_PremiumGrpCode
												 ,Mem_PayrollGroup
												 ,Mem_CostcenterCode
                                                 ,Mem_EmploymentStatusCode
												 ,Mem_PayrollType
												 ,Mem_PositionGrade                                               
                                                 ,@UserLogin
                                                 ,GETDATE()
                                        FROM @PAYPERIODDATE, M_Employee
                                        LEFT JOIN {1}..M_Shift ON Msh_ShiftCode = Mem_ShiftCode
                                            AND Msh_CompanyCode = '{2}'
                                        LEFT JOIN T_EmpTimeRegister ON Mem_IDNo = Ttr_IDNo
                                           AND Ttr_PayCycle = @Ttr_PayCycle
                                        WHERE Ttr_IDNo IS NULL  
                                          {0} 
                                          AND LEFT(Mem_WorkStatus, 1) = 'A'

                                        IF (@POCKETSIZE > 2)
                                        BEGIN
                                            INSERT INTO T_EmpTimeRegisterMisc
                                                (Ttm_IDNo
                                                ,Ttm_Date
                                                ,Ttm_PayCycle
                                                ,Ttm_ActIn_01
                                                ,Ttm_ActOut_01
                                                ,Ttm_ActIn_02
                                                ,Ttm_ActOut_02
                                                ,Ttm_ActIn_03
                                                ,Ttm_ActOut_03
                                                ,Ttm_ActIn_04
                                                ,Ttm_ActOut_04
                                                ,Ttm_ActIn_05
                                                ,Ttm_ActOut_05
                                                ,Ttm_ActIn_06
                                                ,Ttm_ActOut_06
                                                ,Ttm_ActIn_07
                                                ,Ttm_ActOut_07
                                                ,Ttm_ActIn_08
                                                ,Ttm_ActOut_08
                                                ,Ttm_ActIn_09
                                                ,Ttm_ActOut_09
                                                ,Ttm_ActIn_10
                                                ,Ttm_ActOut_10
                                                ,Ttm_ActIn_11
                                                ,Ttm_ActOut_11
                                                ,Ttm_ActIn_12
                                                ,Ttm_ActOut_12
                                                ,Ttm_Result
                                                ,Ttm_ActIn1
                                                ,Ttm_ActOut1
                                                ,Ttm_ActIn2
                                                ,Ttm_ActOut2
                                                ,Usr_Login
                                                ,Ludatetime)

                                                SELECT Mem_IDNo
                                                     ,currentdate
                                                     ,@Ttr_PayCycle
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,''
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,'0000'
                                                     ,@UserLogin
                                                     ,GETDATE()
                                                FROM @PAYPERIODDATE, M_Employee
                                                LEFT JOIN T_EmpTimeRegisterMisc ON Mem_IDNo = Ttm_IDNo
                                                    AND Ttm_PayCycle = @Ttr_PayCycle
                                                WHERE Ttm_IDNo IS NULL  
                                                    {0} 
                                                AND LEFT(Mem_WorkStatus, 1) = 'A'

                                                UPDATE T_EmpTimeRegister
                                                SET Ttr_ActIn_1 = '9999'
                                                , Usr_Login = @UserLogin
                                                , Ludatetime = GETDATE()
                                                FROM T_EmpTimeRegister INNER JOIN M_Employee
                                                ON Ttr_IDNo = Mem_IDNo
                                                WHERE Ttr_PayCycle = @Ttr_PayCycle
                                                    {0}
                                                AND LEFT(Mem_WorkStatus, 1) = 'A'
                                        END", EmployeeList, CentralProfile, CompanyCode);
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ttr_PayCycle", PayPeriod);
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
                                            , DataTable EmployeeDataList
                                            , string EmployeeFilter
                                            , string PayPeriod
                                            , string DateStart
                                            , string DateEnd
                                            , bool CreateLogLedger
                                            , bool CorrectCalendar
                                            , bool CorrectRestday
                                            , bool CorrectHoliday
                                            , bool CorrectShift
                                            , bool CorrectAssumePresent
                                            , bool CorrectTimeCorrection
                                            , bool CorrectPayrollType
                                            , bool CorrectEmploymentStatus
                                            , bool CorrectPosition
                                            , bool CorrectPayrollGroup
                                            , bool CorrectPremiumGroup
                                            , bool CorrectWorkLocation
                                            , bool CorrectCostcenter
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
            //****************************
            try
            {
                #region <Initialize string conditions>
                string queryExtension = "";
                if (ProcessAll == false)
                {
                    if (UseDataTable == true)
                    {
                        if (EmployeeDataList.Rows.Count > 0)
                        {
                            queryExtension = @" AND Ttr_IDNo IN (";
                            foreach (DataRow drEmployee in EmployeeDataList.Rows)
                            {
                                queryExtension += string.Format(@"'{0}',", drEmployee["Mem_IDNo"].ToString());
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
                #endregion

                #region <Create Log Ledger Records if not exist> //Works only if ProcessPerPayPeriod is true
                if (CreateLogLedger == true && ProcessPerPayPeriod == true)
                {
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");
                    CorrectEmployeeTimeRegisterRecord(PayPeriod
                                                    , queryExtension
                                                    , CorrectCalendar
                                                    , false //CorrectRestday
                                                    , false //CorrectHoliday
                                                    , false //CorrectShift
                                                    , CorrectAssumePresent
                                                    , CorrectTimeCorrection
                                                    , CorrectPayrollType
                                                    , CorrectEmploymentStatus
                                                    , CorrectPosition
                                                    , CorrectPayrollGroup
                                                    , CorrectPremiumGroup
                                                    , CorrectWorkLocation
                                                    , CorrectCostcenter
                                                    , UserLogin);

                    int NoofFuturePayCycle = Convert.ToInt32((new CommonBL()).GetParameterValueFromPayroll("TIMEREGCYCLE", CompanyCode, dalHelper));
                    if (NoofFuturePayCycle > 0)
                    {
                        for (int i = 1; i <= NoofFuturePayCycle; i++)
                        {
                            string NextPeriod = GetNextCycle(i, PayPeriod, dalHelper);
                            GenerateLogLedgerRecord(queryExtension, NextPeriod, UserLogin);
                            CorrectShiftRecord(NextPeriod, UserLogin);
                            queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");
                            CorrectEmployeeTimeRegisterRecord(NextPeriod
                                                            , queryExtension
                                                            , CorrectCalendar
                                                            , CorrectRestday
                                                            , CorrectHoliday
                                                            , CorrectShift
                                                            , CorrectAssumePresent
                                                            , CorrectTimeCorrection
                                                            , CorrectPayrollType
                                                            , CorrectEmploymentStatus
                                                            , CorrectPosition
                                                            , CorrectPayrollGroup
                                                            , CorrectPremiumGroup
                                                            , CorrectWorkLocation
                                                            , CorrectCostcenter
                                                            , UserLogin);
                        }
                    }
                }
                else if (CreateLogLedger == true && !ProcessPerPayPeriod)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    GenerateLogLedgerRecord(queryExtension, PayPeriod, UserLogin);
                    CorrectShiftRecord(PayPeriod, UserLogin);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");
                    CorrectEmployeeTimeRegisterRecord(PayPeriod
                                                    , queryExtension
                                                    , CorrectCalendar
                                                    , CorrectRestday
                                                    , CorrectHoliday
                                                    , CorrectShift
                                                    , CorrectAssumePresent
                                                    , CorrectTimeCorrection
                                                    , CorrectPayrollType
                                                    , CorrectEmploymentStatus
                                                    , CorrectPosition
                                                    , CorrectPayrollGroup
                                                    , CorrectPremiumGroup
                                                    , CorrectWorkLocation
                                                    , CorrectCostcenter
                                                    , UserLogin);
                }
                #endregion

            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }


        public void CorrectEmployeeTimeRegisterRecord(string PayCycle
                                                    , string queryExtension
                                                    , bool CorrectCalendar
                                                    , bool CorrectRestday
                                                    , bool CorrectHoliday
                                                    , bool CorrectShift
                                                    , bool CorrectAssumePresent
                                                    , bool CorrectTimeCorrection
                                                    , bool CorrectPayrollType
                                                    , bool CorrectEmploymentStatus
                                                    , bool CorrectPosition
                                                    , bool CorrectCompanyProfileGroup
                                                    , bool CorrectPremiumGroup
                                                    , bool CorrectWorkLocation
                                                    , bool CorrectCostcenter
                                                    , string UserLogin)
        {
            //****************************
            // CorrectWorkGroup: Updates workgroup in log ledger and employee master FROM T_EmpAssignmentDate (CALENDAR)
            // CorrectRestday: Updates day code and restday 
            // CorrectHoliday: Updates holiday
            // CorrectShift: Updates shift code
            // CorrectAssumePresent: Updates the logs FROM assumed present and time modification tables
            // CorrectPayrollType: Updates payroll type in log ledger and employee master FROM T_EmpSalary
            // CorrectEmploymentStatus: Updates payroll type in log ledger and employee master FROM  T_EmpAssignmentDate (EMPSTAT)
            // CorrectPosition: Updates payroll type in log ledger and employee master FROM T_EmpPosition
            // CorrectCompanyProfileGroup: Updates payroll type in log ledger and employee master FROM T_EmpCompanyProfilePayGrp
            // CorrectPremiumGroup: Updates payroll type in log ledger and employee master FROM  T_EmpAssignmentDate (PREMIUM)
            // CorrectWorkLocation: Updates payroll type in log ledger and employee master FROM  T_EmpAssignmentDate(LOCATION)
            //****************************
            try
            {
                string query = string.Empty;
                string DateStart = string.Empty;
                string DateEnd = string.Empty;

                #region Get Start and End Cycle
                DataTable dtPayperiod = GetCycleRange(PayCycle);
                if (dtPayperiod.Rows.Count > 0)
                {
                    DateStart = dtPayperiod.Rows[0]["Tps_StartCycle"].ToString();
                    DateEnd = dtPayperiod.Rows[0]["Tps_EndCycle"].ToString();
                }
                #endregion

                #region <Correct Calendar>
                if (CorrectCalendar == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Calendar Group Based on CALENDAR
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_CalendarGrpDate = Tac_StartDate
                              , M_Employee.Mem_CalendarGrpCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate on Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'CALENDAR'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_CalendarGrpCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tcg_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'CALENDAR'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo 
                                                                                              ) Temp)
                                {4}
                              
                            UPDATE M_Employee
                            SET M_Employee.Mem_CalendarGrpDate = Tac_StartDate
                              , M_Employee.Mem_CalendarGrpCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate on Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'CALENDAR'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_CalendarGrpCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tcg_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'CALENDAR'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo 
                                                                                              ) Temp) 
                               {4} ", DateStart, DateEnd, UserLogin, CentralProfile, queryExtension);
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"--Initialize CalendarGroup
                                            UPDATE T_EmpTimeRegister
                                            SET Ttr_CalendarGroup = Tac_Value
                                                , T_EmpTimeRegister.Usr_Login = '{2}'
                                                , T_EmpTimeRegister.Ludatetime = GETDATE()
                                            FROM T_EmpTimeRegister
                                            INNER JOIN {3}..T_EmpAssignmentDate ON Ttr_IDNo = Tac_IDNo
	                                           AND Tac_AssignmentType = 'CALENDAR'
	                                           AND Ttr_Date BETWEEN Tac_StartDate AND ISNULL(Tac_EndDate, Ttr_Date)
                                            WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Restday>
                if (CorrectRestday == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Restday Based on RESTDAY
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_RestDate = Tac_StartDate
                              , M_Employee.Mem_RestDay = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
                                AND Tac_AssignmentType = 'RESTDAY'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_RestDay != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) AS Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'RESTDAY'
                                                                                                AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp) 
                                {4}
                    
                            UPDATE M_Employee
                            SET M_Employee.Mem_RestDate = Tac_StartDate
                              , M_Employee.Mem_RestDay = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
                                AND Tac_AssignmentType = 'RESTDAY'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_RestDay != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) AS Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'RESTDAY'
                                                                                                AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp) 
                                {4} "
                                , DateStart
                                , DateEnd
                                , UserLogin
                                , CentralProfile
                                , queryExtension);

                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    #region Update Rest Day for Calendar Group (REG)
                    query = string.Format(@"
                                          -- Initialize all day codes to REG
                                            UPDATE T_EmpTimeRegister
                                            SET Ttr_DayCode = 'REG'
                                                , Ttr_RestDayFlag = 0
                                                , Ttr_HolidayFlag = 0
                                                , T_EmpTimeRegister.Usr_Login = '{2}'
                                                , T_EmpTimeRegister.Ludatetime = GETDATE()
                                            FROM T_EmpTimeRegister
                                            WHERE Ttr_CalendarGroup = 'REG'
                                                AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    #endregion

                    query = string.Format(@"
                                            UPDATE T_EmpTimeRegister
                                            SET Ttr_RestDayFlag = CASE WHEN {3}.[dbo].[Udf_IsRestday] (Tac_Value, Ttr_date) = 1 THEN 1 ELSE 0 END
	                                            , Ttr_DayCode = CASE WHEN {3}.[dbo].[Udf_IsRestday] (Tac_Value, Ttr_date) = 1 THEN 'REST' ELSE 'REG' END
	                                            , Ttr_HolidayFlag = 0
                                                , Usr_Login = '{2}'
                                                , Ludatetime = GETDATE()
                                              FROM T_EmpTimeRegister
                                              INNER JOIN {3}..T_EmpAssignmentDate ON Tac_IDNo = TTr_IDNo
	                                                AND Tac_AssignmentType = 'RESTDAY'
                                                    AND Ttr_Date between Tac_StartDate AND ISNULL(Tac_EndDate, Ttr_Date)
                                              WHERE Ttr_CalendarGroup = 'REG'
                                                    AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'"
                                                        , DateStart
                                                        , DateEnd
                                                        , UserLogin
                                                        , CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);

                    #region Update Day Code based on Calendar Group (NONREG)
                    query = string.Format(@"--Initialize all day codes to REG
                                              UPDATE T_EmpTimeRegister
                                                 SET Ttr_DayCode = 'REG'
                                                    , Ttr_RestDayFlag = 0
                                                    , Ttr_HolidayFlag = 0
                                                    , T_EmpTimeRegister.Usr_Login = '{2}'
                                                    , T_EmpTimeRegister.Ludatetime = GETDATE()
                                                FROM T_EmpTimeRegister
                                                WHERE Ttr_CalendarGroup != 'REG'
                                                    AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'"
                                                , DateStart
                                                , DateEnd
                                                , UserLogin);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    #endregion

                    query = string.Format(@"--Initial Day Code based on Calendar Group
                                              UPDATE T_EmpTimeRegister
                                                 SET Ttr_DayCode = CASE WHEN LEN(RTRIM(Tct_WorkCode)) = 0 THEN '' WHEN RTRIM(Tct_WorkCode) = 'R' THEN 'REST' ELSE 'REG' END
                                                    , Ttr_RestDayFlag = CASE WHEN RTRIM(Tct_WorkCode) = 'R' THEN 1 ELSE 0 END
                                                    , T_EmpTimeRegister.Usr_Login = '{2}'
                                                    , T_EmpTimeRegister.Ludatetime = GETDATE()
                                                FROM T_EmpTimeRegister 
                                                INNER JOIN {3}..T_CalendarGroupTmp 
                                                    ON Tct_CalendarGroup = Ttr_CalendarGroup
                                                    AND Tct_Date = Ttr_Date
                                                    AND Tct_CompanyCode = '{4}'
                                                WHERE Ttr_CalendarGroup != 'REG'
                                                    AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'"
                                            , DateStart
                                            , DateEnd
                                            , UserLogin
                                            , CentralProfile
                                            , CompanyCode);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Holiday>
                if (CorrectHoliday == true)
                {
                    query = string.Format(@"--Update Holiday Code
                          UPDATE T_EmpTimeRegister
                            SET Ttr_DayCode = Thl_HolidayCode
                                , Ttr_HolidayFlag = Mdy_HolidayFlag
                                , Ttr_ShiftCode = CASE WHEN Ttr_RestDayFlag = 0 and Mdy_HolidayFlag = 0 THEN 
                                                            Ttr_ShiftCode
                                                       ELSE
                                                          CASE WHEN LEN(Rtrim(Msh_8HourShiftCode)) > 0 THEN 
                                                                Msh_8HourShiftCode
                                                          ELSE 
                                                                Ttr_ShiftCode
                                                          END
                                                  END
                                , T_EmpTimeRegister.Usr_Login = '{2}'
                                , T_EmpTimeRegister.Ludatetime = GETDATE()
                            FROM T_EmpTimeRegister
                            INNER JOIN M_Employee ON Ttr_IDNo = Mem_IDNo
                            INNER JOIN {4}..T_Holiday ON Ttr_Date = Thl_HolidayDate
                                AND Thl_CompanyCode = '{3}'
                            INNER JOIN {4}..M_Day ON Thl_HolidayCode = Mdy_DayCode
                                AND Mdy_CompanyCode = '{3}'
                            INNER JOIN {4}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
                                AND Msh_CompanyCode = '{3}'
                            WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'
	                            AND (Ttr_WorkLocationCode IN (SELECT Data FROM {4}.dbo.Udf_Split(Thl_LocationCode,','))  OR Thl_LocationCode = 'ALL')	 
	                            AND	(Ttr_PayrollGroup IN (SELECT Data FROM {4}.dbo.Udf_Split(Thl_PayrollGroup,',')) OR Thl_PayrollGroup = 'ALL')
	                            AND	(Ttr_PayrollType IN (SELECT Data FROM {4}.dbo.Udf_Split(Thl_PayrollType,',')) OR Thl_PayrollType = 'ALL')
	                            AND	(Ttr_EmploymentStatusCode IN (SELECT Data FROM {4}.dbo.Udf_Split(Thl_EmploymentStatus,',')) OR Thl_EmploymentStatus = 'ALL')
	                            AND	(Ttr_CalendarGroup IN (SELECT Data FROM {4}.dbo.Udf_Split(Thl_CalendarGroup,',')) OR Thl_CalendarGroup = 'ALL')
	                            AND	(Ttr_Grade IN (SELECT Data FROM {4}.dbo.Udf_Split(Thl_Grade,',')) OR Thl_Grade = 'ALL')"
                            , DateStart, DateEnd, UserLogin, CompanyCode, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                if (CorrectRestday == true || CorrectHoliday == true)
                {
                    #region <Correct Day Code Filler Restday>
                    query = string.Format(@"-- Added Restday Indicator in Day Code Master
                                    UPDATE T_EmpTimeRegister
                                    SET Ttr_RestDayFlag = Mdy_RestdayFlag
                                        , T_EmpTimeRegister.Usr_Login = '{2}'
                                        , T_EmpTimeRegister.Ludatetime = GetDate()
                                    FROM T_EmpTimeRegister
                                    INNER JOIN {4}..M_MiscellaneousDay ON Ttr_DayCode = Mmd_DayCode
                                            AND Mmd_CompanyCode = '{3}'
                                    INNER JOIN {4}..M_Day ON Mmd_DayCode = Mdy_DayCode
                                            AND Mdy_CompanyCode = '{3}'
                                    WHERE Mmd_RecordStatus = 'A'
                                        AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'"
                                , DateStart, DateEnd, UserLogin, CompanyCode, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    #endregion

                    #region <Update to REG day code on New and Separated Employees>
                    query = string.Format(@"
                                            DECLARE @REGDAYLEDG VARCHAR(10) = (SELECT Mph_CharValue FROM M_PolicyHdr WHERE RTRIM(Mph_PolicyCode) = 'REGDAYLEDG' AND RTRIM(Mph_CompanyCode) = '{1}' AND RTRIM(Mph_RecordStatus) = 'A')
                                                --Update to REG day code on New and Separated Employees                                             
                                                UPDATE T_EmpTimeRegister
                                                SET Ttr_DayCode = 'REG'
                                                    , Ttr_RestDayFlag = 0
                                                    , Ttr_HolidayFlag = 0
                                                    , Ttr_ShiftCode = Mem_ShiftCode
                                                    , T_EmpTimeRegister.Usr_Login = '{0}'
                                                    , T_EmpTimeRegister.Ludatetime = GETDATE()
                                                FROM T_EmpTimeRegister
                                                INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
							                    WHERE ((Ttr_Date < Mem_IntakeDate)
                                                    OR (LEFT(Mem_WorkStatus, 1) = 'I'
                                                        AND Mem_SeparationDate IS NOT NULL
					                                    AND Ttr_Date >= Mem_SeparationDate))
                                               AND ((Mem_PayrollType = 'M' AND @REGDAYLEDG = 'TRUE') OR Mem_PayrollType = 'D')"
                                            , UserLogin
                                            , CompanyCode);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    #endregion
                }

                #region <Correct Shift>
                if (CorrectShift == true)
                {
                    #region <Update Shift Code for Calendar Group (REG)>
                    query = string.Format(@"--New query for update shift in logledger (REG worktype)
                                              UPDATE T_EmpTimeRegister
                                                SET Ttr_ShiftCode = CASE WHEN Ttr_RestDayFlag = 0 AND Ttr_HolidayFlag = 0
                                                                           THEN Mem_ShiftCode
                                                                           ELSE
                                                                              CASE WHEN LEN(Rtrim(Msh_8HourShiftCode)) > 0 
                                                                                   THEN Msh_8HourShiftCode
                                                                                   ELSE Mem_ShiftCode
                                                                              END
                                                                      END
                                                    , T_EmpTimeRegister.Usr_Login = '{2}'
                                                    , T_EmpTimeRegister.Ludatetime = GETDATE()
                                              FROM T_EmpTimeRegister
                                              INNER JOIN M_Employee on Mem_IDNo = Ttr_IDNo
                                              INNER JOIN {5}..M_Shift on Msh_ShiftCode = Mem_ShiftCode
                                                    AND Msh_CompanyCode = '{4}'
                                              WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'
                                                AND Ttr_CalendarGroup = 'REG'
                                              @EXTENSION

                                            DECLARE @SHFTCYCOPN Char(1) = (SELECT Mph_CharValue FROM M_PolicyHdr WHERE Mph_PolicyCode = 'SHFTCYCOPN' AND Mph_CompanyCode = '{4}' AND Mph_RecordStatus = 'A')

                                            IF @SHFTCYCOPN = 'L' -- 'Last Regular Day Shift' 
                                            BEGIN
                                                DECLARE @ShiftTable as TABLE
                                                (Ttr_IDNo varchar(MAX), Ttr_Date datetime)

                                                DECLARE @DateStart datetime
                                                SET @DateStart = (SELECT Tps_StartCycle FROM T_PaySchedule WHERE Tps_PayCycle = '{3}')

                                                INSERT INTO @ShiftTable
                                                SELECT Ttr_IDNo, MAX(Ttr_Date)
                                                FROM  ( SELECT Ttr_IDNo, MAX(Ttr_Date) Ttr_Date 
                                                        FROM T_EmpTimeRegister
                                                        INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
                                                        WHERE Ttr_RestDayFlag = 0 and Ttr_HolidayFlag = 0
                                                            AND Ttr_Date < @DateStart
                                                        GROUP BY Ttr_IDNo

                                                        UNION 

                                                        SELECT Ttr_IDNo, MAX(Ttr_Date) Ttr_Date 
                                                        FROM T_EmpTimeRegisterHst
                                                        INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
                                                        WHERE Ttr_RestDayFlag = 0 and Ttr_HolidayFlag = 0
                                                            AND Ttr_Date < @DateStart
                                                        GROUP BY Ttr_IDNo)TEMP
                                                        GROUP BY Ttr_IDNo
                                                        ORDER BY Ttr_IDNo

                                                        UPDATE T_EmpTimeRegister 
                                                        SET Ttr_ShiftCode = CASE WHEN Ttr_RestDayFlag = 0 and Ttr_HolidayFlag = 0 THEN 
                                                                                        ELLPREV.Ttr_ShiftCode
                                                                                    ELSE
                                                                                        CASE WHEN LEN(Rtrim(Msh_8HourShiftCode)) > 0  THEN 
                                                                                                Msh_8HourShiftCode
                                                                                        ELSE 
                                                                                                ELLPREV.Ttr_ShiftCode
                                                                                        END
                                                                                    END
                                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                                        FROM T_EmpTimeRegister ELL
                                                        LEFT JOIN  ( SELECT SHFT.Ttr_IDNo, SHFT.Ttr_Date, Ttr_ShiftCode
	                                                                FROM T_EmpTimeRegister LDR
	                                                                INNER JOIN @ShiftTable SHFT ON SHFT.Ttr_IDNo = LDR.Ttr_IDNo
	                                                                    AND SHFT.Ttr_Date = LDR.Ttr_Date

	                                                                UNION 

	                                                                SELECT SHFT.Ttr_IDNo, SHFT.Ttr_Date, Ttr_ShiftCode
	                                                                FROM T_EmpTimeRegisterHst LDR
	                                                                INNER JOIN @ShiftTable SHFT ON SHFT.Ttr_IDNo = LDR.Ttr_IDNo
	                                                                    AND SHFT.Ttr_Date = LDR.Ttr_Date
                                                                )ELLPREV ON ELLPREV.Ttr_IDNo = ELL.Ttr_IDNo
                                                                INNER JOIN {5}..M_Shift ON Msh_ShiftCode = ELLPREV.Ttr_ShiftCode
                                                                        AND Msh_CompanyCode = '{4}'
                                                                INNER JOIN {5}..T_EmpAssignmentDate ON Tac_IDNo = ELL.Ttr_IDNo
				                                                AND Tac_AssignmentType  = 'CALENDAR'
                                                                AND Tac_StartDate = (SELECT TOP 1 Tac_StartDate
					                                                                FROM {5}..T_EmpAssignmentDate
					                                                                WHERE Tac_AssignmentType  = 'CALENDAR'
										                                                AND Tac_IDNo = ELL.Ttr_IDNo
						                                                                AND Tac_StartDate <= ELL.Ttr_Date
					                                                                ORDER BY Tac_StartDate DESC)
                                                                    AND Tac_Value = 'REG'
                                                                WHERE ELL.Ttr_PayCycle = '{3}'
                                                                @INNEREXTENSION
                                                            END", DateStart, DateEnd, UserLogin, PayCycle, CompanyCode, CentralProfile);
                    query = query.Replace("@EXTENSION", queryExtension);
                    query = query.Replace("@INNEREXTENSION", queryExtension.Replace("Ttr_", "ELL.Ttr_"));
                    dalHelper.ExecuteNonQuery(query);
                    #endregion

                    #region <Update Shift Code for Calendar Group (NONREG)>
                    query = string.Format(@"-- Shift Code based on Calendar Group
                                                UPDATE T_EmpTimeRegister
                                                SET Ttr_ShiftCode = Tct_ShiftCode
                                                    , T_EmpTimeRegister.Usr_Login = '{2}'
                                                    , T_EmpTimeRegister.Ludatetime = GETDATE()
                                                FROM T_EmpTimeRegister 
                                                INNER JOIN {4}..T_CalendarGroupTmp 
                                                    ON Tct_CalendarGroup = Ttr_CalendarGroup
                                                    AND Tct_CompanyCode = '{3}'
                                                    AND Tct_Date = Ttr_Date
                                                WHERE Ttr_CalendarGroup != 'REG'
                                                    AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'"
                                                , DateStart
                                                , DateEnd
                                                , UserLogin
                                                , CompanyCode
                                                , CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                    #endregion

                }
                #endregion

                #region <Correct Assumed Present>
                if (CorrectAssumePresent == true)
                {
                    queryExtension = queryExtension.Replace("AND Ttr_IDNo", "WHERE Tad_IDNo");
                    query = string.Format(@"SELECT * FROM {0}..T_EmpAssignmentDate
                                                WHERE Tac_AssignmentType = 'ASSUME'
                                                AND ISNULL(Tac_EndDate, '{2}') >= '{1}' AND '{2}' >= Tac_StartDate 
                                                ", CentralProfile, DateStart, DateEnd);

                    query += queryExtension;
                    DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];

                    if (dtResult.Rows.Count > 0)
                    {
                        queryExtension = queryExtension.Replace("WHERE Tad_IDNo", "AND Ttr_IDNo");
                        query = string.Format(@"
                                              UPDATE T_EmpTimeRegister
                                                SET Ttr_AssumedFlag = 0
                                                    ,Ttr_AssumedBy = '{1}'
                                                    ,Ttr_AssumedDate = GETDATE()
                                                WHERE Ttr_DayCode = 'REG'
                                                    AND Ttr_PayCycle = '{0}'", PayCycle, UserLogin);
                        query += queryExtension;
                        dalHelper.ExecuteNonQuery(query);

                        foreach (DataRow drRow in dtResult.Rows)
                        {
                            query = string.Format(@"
                                              ---Assumed Present Updating
                                              UPDATE T_EmpTimeRegister
                                                SET Ttr_AssumedFlag = 1
                                                    ,Ttr_AssumedBy = '{2}'
                                                    ,Ttr_AssumedDate = GETDATE()
                                                FROM T_EmpTimeRegister
                                                INNER JOIN {3}..T_EmpAssignmentDate ON Tac_IDNo = Ttr_IDNo
	                                                AND Ttr_Date BETWEEN Tac_StartDate AND ISNULL(Tac_EndDate, Ttr_Date)
	                                                AND Tac_AssignmentType = 'ASSUME'
                                                WHERE Ttr_DayCode = 'REG'
                                                    AND Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'",
                                                    DateStart, DateEnd, UserLogin, CentralProfile);
                            query += queryExtension;
                            dalHelper.ExecuteNonQuery(query);
                        }
                    }
                    else queryExtension = queryExtension.Replace("WHERE Tad_IDNo", "AND Ttr_IDNo");

                }
                #endregion

                #region <Correct Payroll Type>
                if (CorrectPayrollType == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Payroll Type Based ON T_EmpSalary
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_PayrollType = Tsl_PayrollType
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpSalary ON Mem_IDNo = Tsl_IDNo
                            WHERE @EndNewCycle >= Tsl_StartDate
	                            AND ISNULL(Tsl_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PayrollType != Tsl_PayrollType
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tsl_IDNo + CONVERT(CHAR(10),Tsl_StartDate,112) IN ( SELECT Tsl_IDNo + Convert(Char(10), Tsl_StartDate, 112)
															                            FROM (SELECT Tsl_IDNo , MAX(Tsl_StartDate) AS Tsl_StartDate
																                              FROM {3}..T_EmpSalary
																                              WHERE @EndNewCycle >= Tsl_StartDate
																	                            AND ISNULL(Tsl_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tsl_IDNo ) Temp) 
                                {4}
                    
                            UPDATE M_Employee
                            SET M_Employee.Mem_PayrollType = Tsl_PayrollType
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpSalary ON Mem_IDNo = Tsl_IDNo
                            WHERE @EndNewCycle >= Tsl_StartDate
	                            AND ISNULL(Tsl_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PayrollType != Tsl_PayrollType
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tsl_IDNo + CONVERT(CHAR(10),Tsl_StartDate,112) IN ( SELECT Tsl_IDNo + Convert(Char(10), Tsl_StartDate, 112)
															                            FROM (SELECT Tsl_IDNo , MAX(Tsl_StartDate) AS Tsl_StartDate
																                              FROM {3}..T_EmpSalary
																                              WHERE @EndNewCycle >= Tsl_StartDate
																	                            AND ISNULL(Tsl_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tsl_IDNo ) Temp) 
                               {4} "
                                    , DateStart
                                    , DateEnd
                                    , UserLogin
                                    , CentralProfile
                                    , queryExtension);
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        UPDATE T_EmpTimeRegister
                                        Set   Ttr_PayrollType = Tsl_PayrollType
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpSalary ON Ttr_IDNo = Tsl_IDNo
                                            AND Ttr_Date BETWEEN Tsl_StartDate AND ISNULL(Tsl_EndDate, Ttr_Date)
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Employment Status>
                if (CorrectEmploymentStatus == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Employment Status Based on EMPSTAT
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_EmploymentStatusCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'EMPSTAT'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_EmploymentStatusCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'EMPSTAT'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp) 
                                {4}
  
                            UPDATE M_Employee
                            SET Mem_EmploymentStatusCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'EMPSTAT'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_EmploymentStatusCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'EMPSTAT'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp) 
                                {4} ", DateStart
                                    , DateEnd
                                    , UserLogin
                                    , CentralProfile
                                    , queryExtension);

                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        ---Initialize EmploymentStatusCode
                                        UPDATE T_EmpTimeRegister
                                        SET Ttr_EmploymentStatusCode = Tac_Value
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpAssignmentDate ON Ttr_IDNo = Tac_IDNo
	                                       AND Tac_AssignmentType = 'EMPSTAT'
	                                       AND Ttr_Date BETWEEN Tac_StartDate AND ISNULL(Tac_EndDate, Ttr_Date) 
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Position>
                if (CorrectPosition == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Based on T_EmpPosition
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_PositionDate = Tpo_StartDate
                              , M_Employee.Mem_PositionCode = Tpo_PositionCode
                              , M_Employee.Mem_PositionGrade = Tpo_PositionGrade
                              , M_Employee.Mem_PositionLevel = Tpo_PositionLevel
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpPosition ON Mem_IDNo = Tpo_IDNo
                            WHERE @EndNewCycle >= Tpo_StartDate
	                            AND ISNULL(Tpo_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PositionGrade != Tpo_PositionGrade
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tpo_IDNo + CONVERT(CHAR(10),Tpo_StartDate,112) IN ( SELECT Tpo_IDNo + CONVERT(CHAR(10), Tpo_StartDate, 112)
															                            FROM (SELECT Tpo_IDNo , MAX(Tpo_StartDate) AS Tpo_StartDate
																                              FROM {3}..T_EmpPosition
																                              WHERE @EndNewCycle >= Tpo_StartDate
																	                            AND ISNULL(Tpo_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tpo_IDNo ) Temp)
                                {4}
  
                            UPDATE M_Employee
                            SET M_Employee.Mem_PositionDate = Tpo_StartDate
                              , M_Employee.Mem_PositionCode = Tpo_PositionCode
                              , M_Employee.Mem_PositionGrade = Tpo_PositionGrade
                              , M_Employee.Mem_PositionLevel = Tpo_PositionLevel
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpPosition ON Mem_IDNo = Tpo_IDNo
                            WHERE @EndNewCycle >= Tpo_StartDate
	                            AND ISNULL(Tpo_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PositionGrade != Tpo_PositionGrade
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tpo_IDNo + CONVERT(CHAR(10),Tpo_StartDate,112) IN ( SELECT Tpo_IDNo + CONVERT(CHAR(10), Tpo_StartDate, 112)
															                            FROM (SELECT Tpo_IDNo , MAX(Tpo_StartDate) AS Tpo_StartDate
																                              FROM {3}..T_EmpPosition
																                              WHERE @EndNewCycle >= Tpo_StartDate
																	                            AND ISNULL(Tpo_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tpo_IDNo ) Temp) 
                               {4} ", DateStart
                                   , DateEnd
                                   , UserLogin
                                   , CentralProfile
                                   , queryExtension);

                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        ---Initialize Grade
                                        UPDATE T_EmpTimeRegister
                                        SET Ttr_Grade = Tpo_PositionGrade
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpPosition ON Ttr_IDNo = Tpo_IDNo
	                                        AND Ttr_Date BETWEEN Tpo_StartDate AND ISNULL(Tpo_EndDate, Ttr_Date) 
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Payroll Group>
                if (CorrectCompanyProfileGroup == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Payroll Group Based on T_EmpCompanyProfilePayGrp
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'

                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_CompanyDate = Tec_StartDate
                              , M_Employee.Mem_CompanyCode = Tec_CompanyCode
                              , M_Employee.Mem_ProfileCode = Tec_ProfileCode
                              , M_Employee.Mem_PayrollGroup = Tec_PayrollGroup
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpCompanyProfilePayGrp ON Mem_IDNo = Tec_IDNo
                                AND Tec_CompanyCode = '{4}'
                            WHERE @EndNewCycle >= Tec_StartDate
	                            AND ISNULL(Tec_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PayrollGroup != Tec_PayrollGroup
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tec_IDNo + CONVERT(CHAR(10),Tec_StartDate,112) IN ( SELECT Tec_IDNo + CONVERT(CHAR(10), Tec_StartDate, 112)
															                            FROM (SELECT Tec_IDNo , MAX(Tec_StartDate) as Tec_StartDate
																                              FROM {3}..T_EmpCompanyProfilePayGrp
																                              WHERE @EndNewCycle >= Tec_StartDate
                                                                                                AND Tec_CompanyCode = '{4}'
																	                            AND ISNULL(Tec_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tec_IDNo ) Temp) 
                                {5}                           
  
                            UPDATE M_Employee
                            SET M_Employee.Mem_CompanyDate = Tec_StartDate
                              , M_Employee.Mem_CompanyCode = Tec_CompanyCode
                              , M_Employee.Mem_ProfileCode = Tec_ProfileCode
                              , M_Employee.Mem_PayrollGroup = Tec_PayrollGroup
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpCompanyProfilePayGrp ON Mem_IDNo = Tec_IDNo
                                AND Tec_CompanyCode = '{4}'
                            WHERE @EndNewCycle >= Tec_StartDate
	                            AND ISNULL(Tec_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PayrollGroup != Tec_PayrollGroup
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tec_IDNo + CONVERT(CHAR(10),Tec_StartDate,112) IN ( SELECT Tec_IDNo + CONVERT(CHAR(10), Tec_StartDate, 112)
															                            FROM (SELECT Tec_IDNo , MAX(Tec_StartDate) as Tec_StartDate
																                              FROM {3}..T_EmpCompanyProfilePayGrp
																                              WHERE @EndNewCycle >= Tec_StartDate
                                                                                                AND Tec_CompanyCode = '{4}'
																	                            AND ISNULL(Tec_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tec_IDNo ) Temp) 
                                {5} ", DateStart
                                    , DateEnd
                                    , UserLogin
                                    , CentralProfile
                                    , CompanyCode
                                    , queryExtension);

                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        ---Initialize Payroll Group
                                        UPDATE T_EmpTimeRegister
                                        Set   Ttr_PayrollGroup = Tec_PayrollGroup
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpCompanyProfilePayGrp ON Ttr_IDNo = Tec_IDNo
                                            AND Tec_CompanyCode = '{4}'
                                            AND Ttr_Date BETWEEN Tec_StartDate AND ISNULL(Tec_EndDate, Ttr_Date) 
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile, CompanyCode);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Premium Group>
                if (CorrectPremiumGroup == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Premium Group Based on PREMIUM
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_PremiumGrpDate = Tac_StartDate
                              , M_Employee.Mem_PremiumGrpCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'PREMIUM'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PremiumGrpCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'PREMIUM'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp)
                                {4}
  
                            UPDATE M_Employee
                            SET M_Employee.Mem_PremiumGrpDate = Tac_StartDate
                              , M_Employee.Mem_PremiumGrpCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'PREMIUM'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_PremiumGrpCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'PREMIUM'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp) 
                               {4} "
                                , DateStart
                                , DateEnd
                                , UserLogin
                                , CentralProfile
                                , queryExtension);
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        ---Initialize PremiumGrpCode
                                        UPDATE T_EmpTimeRegister
                                        SET Ttr_PremiumGrpCode = Tac_Value
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpAssignmentDate ON Ttr_IDNo = Tac_IDNo
	                                       AND Tac_AssignmentType = 'PREMIUM'
	                                       AND Ttr_Date BETWEEN Tac_StartDate AND ISNULL(Tac_EndDate, Ttr_Date) 
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Work Location>
                if (CorrectWorkLocation == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Work Location Based on LOCATION
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_LocationDate = Tac_StartDate
                              , M_Employee.Mem_WorkLocationCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'LOCATION'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_WorkLocationCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'LOCATION'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp)
                                {4}
  
                            UPDATE M_Employee
                            SET M_Employee.Mem_LocationDate = Tac_StartDate
                              , M_Employee.Mem_WorkLocationCode = Tac_Value
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpAssignmentDate ON Mem_IDNo = Tac_IDNo
                            WHERE @EndNewCycle >= Tac_StartDate
	                            AND Tac_AssignmentType = 'LOCATION'
	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_WorkLocationCode != Tac_Value
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tac_IDNo + CONVERT(CHAR(10),Tac_StartDate,112) IN ( SELECT Tac_IDNo + CONVERT(CHAR(10), Tac_StartDate, 112)
															                            FROM (SELECT Tac_IDNo , MAX(Tac_StartDate) as Tac_StartDate
																                              FROM {3}..T_EmpAssignmentDate
																                              WHERE Tac_AssignmentType = 'LOCATION'
																	                            AND @EndNewCycle >= Tac_StartDate
																	                            AND ISNULL(Tac_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tac_IDNo ) Temp) 
                                {4} "
                                , DateStart
                                , DateEnd
                                , UserLogin
                                , CentralProfile
                                , queryExtension);
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        ---Initialize WorkLocationCode
                                        UPDATE T_EmpTimeRegister
                                        SET Ttr_WorkLocationCode = Tac_Value
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpAssignmentDate ON Ttr_IDNo = Tac_IDNo
	                                       AND Tac_AssignmentType = 'LOCATION'
	                                       AND Ttr_Date BETWEEN Tac_StartDate AND ISNULL(Tac_EndDate, Ttr_Date) 
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'", DateStart, DateEnd, UserLogin, CentralProfile);
                    query += queryExtension;
                    dalHelper.ExecuteNonQuery(query);
                }
                #endregion

                #region <Correct Costcenter>
                if (CorrectCostcenter == true)
                {
                    queryExtension = queryExtension.Replace("Ttr_IDNo", "Mem_IDNo");
                    query = string.Format(@"
                            --Initialize Employee Master Costcenter Based on T_EmpCostcenter
                            DECLARE @StartNewCycle DATETIME = '{0}'
                            DECLARE @EndNewCycle DATETIME = '{1}'
                            
                            --CENTRAL
                            UPDATE {3}..M_Employee
                            SET M_Employee.Mem_CostcenterDate = Tcc_StartDate
                              , M_Employee.Mem_CostcenterCode = Tcc_CostCenterCode
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM {3}..M_Employee
                            INNER JOIN {3}..T_EmpCostcenter ON Mem_IDNo = Tcc_IDNo
                            WHERE @EndNewCycle >= Tcc_StartDate
	                            AND ISNULL(Tcc_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_CostcenterCode != Tcc_CostCenterCode
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tcc_IDNo + CONVERT(CHAR(10),Tcc_StartDate,112) IN ( SELECT Tcc_IDNo + CONVERT(CHAR(10), Tcc_StartDate, 112)
															                            FROM (SELECT Tcc_IDNo , MAX(Tcc_StartDate) as Tcc_StartDate
																                              FROM {3}..T_EmpCostcenter
																                              WHERE @EndNewCycle >= Tcc_StartDate
																	                            AND ISNULL(Tcc_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tcc_IDNo ) Temp)
                                {4}
  
                            UPDATE M_Employee
                            SET M_Employee.Mem_CostcenterDate = Tcc_StartDate
                              , M_Employee.Mem_CostcenterCode = Tcc_CostCenterCode
                              , M_Employee.Mem_UpdatedBy = '{2}'
                              , M_Employee.Mem_UpdatedDate = GETDATE()
                            FROM M_Employee
                            INNER JOIN {3}..T_EmpCostcenter ON Mem_IDNo = Tcc_IDNo
                            WHERE @EndNewCycle >= Tcc_StartDate
	                            AND ISNULL(Tcc_EndDate, @StartNewCycle) >= @StartNewCycle
	                            AND Mem_CostcenterCode != Tcc_CostCenterCode
                                AND Mem_WorkStatus LIKE 'A%'
	                            AND Tcc_IDNo + CONVERT(CHAR(10),Tcc_StartDate,112) IN ( SELECT Tcc_IDNo + CONVERT(CHAR(10), Tcc_StartDate, 112)
															                            FROM (SELECT Tcc_IDNo , MAX(Tcc_StartDate) as Tcc_StartDate
																                              FROM {3}..T_EmpCostcenter
																                              WHERE @EndNewCycle >= Tcc_StartDate
																	                            AND ISNULL(Tcc_EndDate, @StartNewCycle) >= @StartNewCycle
																                              GROUP BY Tcc_IDNo ) Temp)
                                {4} "
                                , DateStart
                                , DateEnd
                                , UserLogin
                                , CentralProfile
                                , queryExtension);
                    dalHelper.ExecuteNonQuery(query);
                    queryExtension = queryExtension.Replace("Mem_IDNo", "Ttr_IDNo");

                    query = string.Format(@"
                                        ---Initialize CostcenterCode
                                        UPDATE T_EmpTimeRegister
                                        SET Ttr_CostcenterCode = Tcc_CostCenterCode
                                            , T_EmpTimeRegister.Usr_Login = '{2}'
                                            , T_EmpTimeRegister.Ludatetime = GETDATE()
                                        FROM T_EmpTimeRegister
                                        INNER JOIN {3}..T_EmpCostcenter ON Ttr_IDNo = Tcc_IDNo
	                                       AND Ttr_Date BETWEEN Tcc_StartDate AND ISNULL(Tcc_EndDate, Ttr_Date) 
                                        WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'"
                                        , DateStart
                                        , DateEnd
                                        , UserLogin
                                        , CentralProfile);
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

        public void CorrectDayCodeNewAndSeparatedEmployee(string EmployeeID, string UserLogin, string REGDAYLEDG)
        {
            #region <Update to REG day code on New and Separated Employees>
            string query = string.Format(@"--Update to REG day code on New and Separated Employees
                                            --Added process flag condition
                                                  UPDATE T_EmpTimeRegister
                                                   SET Ttr_DayCode = 'REG'
                                                        , Ttr_RestDayFlag = 0
                                                        , Ttr_HolidayFlag = 0
                                                        , Ttr_ShiftCode = Mem_ShiftCode
                                                        , T_EmpTimeRegister.Usr_Login = '{1}'
                                                        , T_EmpTimeRegister.Ludatetime = GetDate()
                                                    FROM T_EmpTimeRegister
                                                    INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
								                    WHERE ((Ttr_Date < Mem_IntakeDate)
                                                        OR (Mem_WorkStatus = 'IN'
                                                            AND Mem_SeparationDate IS NOT NULL
						                                    AND Ttr_Date >= Mem_SeparationDate))
                                                    AND Ttr_IDNo = '{0}'
                                                    AND ((Mem_PayrollType = 'M' AND '{2}' = 'TRUE') OR Mem_PayrollType = 'D')
                                              END", EmployeeID, UserLogin, REGDAYLEDG);

            dalHelper.ExecuteNonQuery(query);
            #endregion
        }

        public void CorrectDayCodeNewAndSeparatedEmployee(string EmployeeID, string UserLogin, string ProfileDBName, string REGDAYLEDG, DALHelper dal)
        {
            #region <Update to REG day code on New and Separated Employees>
            string query = string.Format(@"--Update to REG day code on New and Separated Employees
                                            -- Added process flag condition
                                                  UPDATE {2}..T_EmpTimeRegister
                                                        SET Ttr_DayCode = 'REG'
                                                        , Ttr_RestDayFlag = 0
                                                        , Ttr_HolidayFlag = 0
                                                        , Ttr_ShiftCode = Mem_ShiftCode
                                                        , Usr_Login = '{1}'
                                                        , Ludatetime = GetDate()
                                                    FROM {2}..T_EmpTimeRegister
                                                    INNER JOIN {2}..M_Employee ON Mem_IDNo = Ttr_IDNo
								                    WHERE ((Ttr_Date < Mem_IntakeDate)
                                                            OR (Mem_WorkStatus = 'IN' AND Mem_SeparationDate IS NOT NULL AND Ttr_Date >= Mem_SeparationDate))
                                                        AND ((Mem_PayrollType = 'M' AND '{3}' = 'TRUE') OR Mem_PayrollType = 'D')
                                                        AND Ttr_IDNo = '{0}'
                                                    
                                               ", EmployeeID, UserLogin, ProfileDBName, REGDAYLEDG);

            dal.ExecuteNonQuery(query);
            #endregion
        }

        public void DeleteSeparatedEmployeesOnPastCycle(int POCKETSIZE)
        {
            #region <Delete Time Register of Employees who have been Separated from the Company on Previous Quincenas>
            string query = "";
            if (POCKETSIZE > 2)
                query = string.Format(@"DELETE FROM T_EmpTimeRegisterMisc
                                    FROM T_EmpTimeRegisterMisc
                                    INNER JOIN M_employee ON Mem_IDNo = Ttm_IDNo
                                    INNER JOIN T_PaySchedule ON Tps_CycleIndicator = 'P'
                                    WHERE Mem_Separationdate BETWEEN Tps_StartCycle 
                                    AND Tps_EndCycle
                                    
                                    DELETE FROM T_EmpTimeRegister
									FROM T_EmpTimeRegister
									INNER JOIN M_employee ON Mem_IDNo = Ttr_IDNo
									INNER JOIN T_PaySchedule ON Tps_CycleIndicator = 'P'
									WHERE Mem_Separationdate BETWEEN Tps_StartCycle 
									AND Tps_EndCycle");
            else
                query = string.Format(@"DELETE FROM T_EmpTimeRegister
                                    FROM T_EmpTimeRegister
                                    INNER JOIN M_employee ON Mem_IDNo = Ttr_IDNo
                                    INNER JOIN T_PaySchedule ON Tps_CycleIndicator = 'P'
                                    WHERE Mem_Separationdate BETWEEN Tps_StartCycle 
                                    AND Tps_EndCycle");
            dalHelper.ExecuteNonQuery(query);
            #endregion
        }

        public void CorrectShiftRecord(string PayPeriod,string UserLogin)
        {
            DataTable dtPayperiod = GetCycleRange(PayPeriod);
            string DateStart = string.Empty, DateEnd = string.Empty;

            if (dtPayperiod.Rows.Count > 0)
            {
                DateStart = dtPayperiod.Rows[0]["Tps_StartCycle"].ToString();
                DateEnd = dtPayperiod.Rows[0]["Tps_EndCycle"].ToString();
            }

            #region query
            string query = string.Format(@"
                                        DECLARE @SHFTCYCOPN CHAR(1) = (SELECT Mph_CharValue FROM M_PolicyHdr WHERE Mph_PolicyCode = 'SHFTCYCOPN' AND Mph_CompanyCode = '{4}' AND Mph_RecordStatus = 'A')

                                            IF  @SHFTCYCOPN = 'N' --  'No Update Shift'
                                            BEGIN
                                            --New query for update shift in logledger (REG worktype)
                                              UPDATE T_EmpTimeRegister
                                              SET Ttr_ShiftCode = CASE WHEN Ttr_RestDayFlag = 0 and Ttr_HolidayFlag = 0 THEN 
                                                                          Mem_ShiftCode
                                                                    ELSE
                                                                              CASE WHEN LEN(Rtrim(Msh_8HourShiftCode)) > 0 THEN 
                                                                                    Msh_8HourShiftCode
                                                                              ELSE 
                                                                                    Mem_ShiftCode
                                                                              END
                                                                      END
                                                    , T_EmpTimeRegister.Usr_Login = '{2}'
                                                    , T_EmpTimeRegister.Ludatetime = GetDate()
                                              FROM T_EmpTimeRegister
                                              INNER JOIN M_Employee on Mem_IDNo = Ttr_IDNo
                                              INNER JOIN {5}..M_Shift on Msh_ShiftCode = Mem_ShiftCode
                                              AND Msh_CompanyCode = '{4}'
                                              WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'
                                                AND Ttr_CalendarGroup = 'REG'
                                            END
                                            ELSE IF @SHFTCYCOPN = 'L' -- 'Last Regular Day Shift' 
                                            BEGIN
                                                DECLARE @ShiftTable as TABLE
                                                (Ttr_IDNo varchar(MAX), Ttr_Date datetime)

                                                DECLARE @DateStart datetime
                                                SET @DateStart = (SELECT Tps_StartCycle FROM T_PaySchedule WHERE Tps_PayCycle = '{3}')

                                                INSERT INTO @ShiftTable
                                                SELECT Ttr_IDNo, MAX(Ttr_Date)
                                                FROM (SELECT Ttr_IDNo, MAX(Ttr_Date) Ttr_Date 
                                                        FROM T_EmpTimeRegister
                                                        INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
                                                        WHERE Ttr_RestDayFlag = 0 and Ttr_HolidayFlag = 0
                                                            AND Ttr_Date < @DateStart
                                                        GROUP BY Ttr_IDNo

                                                        UNION 

                                                        SELECT Ttr_IDNo, MAX(Ttr_Date) Ttr_Date 
                                                        FROM T_EmpTimeRegisterHst
                                                        INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
                                                        WHERE Ttr_RestDayFlag = 0 and Ttr_HolidayFlag = 0
                                                            AND Ttr_Date < @DateStart
                                                        GROUP BY Ttr_IDNo)TEMP
                                                        GROUP BY Ttr_IDNo
                                                        ORDER BY Ttr_IDNo

                                                        UPDATE T_EmpTimeRegister 
                                                        SET Ttr_ShiftCode = CASE WHEN Ttr_RestDayFlag = 0 AND Ttr_HolidayFlag = 0 THEN 
                                                                                    ELLPREV.Ttr_ShiftCode
                                                                                ELSE
                                                                                    CASE WHEN LEN(Rtrim(Msh_8HourShiftCode)) > 0 THEN 
                                                                                        Msh_8HourShiftCode
                                                                                    ELSE 
                                                                                        ELLPREV.Ttr_ShiftCode
                                                                                    END
                                                                                END
                                                        , T_EmpTimeRegister.Usr_Login = '{2}'
                                                        , T_EmpTimeRegister.Ludatetime = GetDate()
                                                        FROM T_EmpTimeRegister ELL
                                                        LEFT JOIN (SELECT SHFT.Ttr_IDNo, SHFT.Ttr_Date, Ttr_ShiftCode
	                                                                FROM T_EmpTimeRegister LDR
	                                                                INNER JOIN @ShiftTable SHFT ON SHFT.Ttr_IDNo = LDR.Ttr_IDNo
	                                                                    AND SHFT.Ttr_Date = LDR.Ttr_Date

	                                                                UNION 

	                                                                SELECT SHFT.Ttr_IDNo, SHFT.Ttr_Date, Ttr_ShiftCode
	                                                                FROM T_EmpTimeRegisterHst LDR
	                                                                INNER JOIN @ShiftTable SHFT ON SHFT.Ttr_IDNo = LDR.Ttr_IDNo
	                                                                    AND SHFT.Ttr_Date = LDR.Ttr_Date )ELLPREV  ON ELLPREV.Ttr_IDNo = ELL.Ttr_IDNo
                                                                    INNER JOIN {5}..M_Shift ON Msh_ShiftCode = ELLPREV.Ttr_ShiftCode
                                                                        AND Msh_CompanyCode = '{4}'
                                                                    INNER JOIN {5}..T_EmpAssignmentDate ON Tac_IDNo = ELL.Ttr_IDNo
					                                                AND Tac_AssignmentType = 'CALENDAR'
					                                                AND Tac_StartDate = (SELECT TOP 1 Tac_StartDate
 										                                                 FROM {5}..T_EmpAssignmentDate
										                                                 WHERE Tac_IDNo = ELL.Ttr_IDNo
											                                                AND Tac_AssignmentType = 'CALENDAR'
											                                                AND Tac_StartDate <= ELL.Ttr_Date
										                                                 ORDER BY Tac_StartDate DESC)
					                                                AND Tac_Value = 'REG'
                                                                    WHERE ELL.Ttr_PayCycle = '{3}'
                                                    END

                                                    --update holiday and the equivalent shift
                                                    UPDATE T_EmpTimeRegister
                                                    SET Ttr_DayCode = Thl_HolidayCode
                                                        , Ttr_HolidayFlag = Mdy_HolidayFlag
                                                        , Ttr_ShiftCode = CASE WHEN Ttr_RestDayFlag = 0 and Mdy_HolidayFlag = 0
                                                                                       THEN Ttr_ShiftCode
                                                                                       ELSE
                                                                                          CASE WHEN LEN(Rtrim(Msh_8HourShiftCode)) > 0 
                                                                                               THEN Msh_8HourShiftCode
                                                                                               else Ttr_ShiftCode
                                                                                          END
                                                                                  END
                                                                , T_EmpTimeRegister.Usr_Login = '{2}'
                                                                , T_EmpTimeRegister.Ludatetime = GetDate()
                                                            FROM T_EmpTimeRegister
                                                            INNER JOIN {5}..T_Holiday ON Ttr_Date = Thl_HolidayDate
	                                                            AND (Thl_LocationCode = Ttr_WorkLocationCode
                                                                OR Thl_LocationCode = 'ALL')
                                                            INNER JOIN {5}..M_Day ON Thl_HolidayCode = Mdy_DayCode
                                                                AND Mdy_CompanyCode = '{4}'
                                                            INNER JOIN {5}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
                                                                AND Msh_CompanyCode = '{4}'
                                                            WHERE Ttr_Date >= '{0}' AND Ttr_Date <= '{1}'
                                                        ", DateStart, DateEnd, UserLogin, PayPeriod, CompanyCode, CentralProfile);
            #endregion

            dalHelper.ExecuteNonQuery(query);
        }

        #endregion

        #region Mandatory OverTime
        public void MandatoryOverTimePostingCurPayPeriod(string PayPeriod, string EmployeeID, string ProcessDate, string ProfileCode, DALHelper dal) //also for future pay period
        {
            string sqlMain = "";

            string strIDNumber = "";
            if (EmployeeID != "")
                strIDNumber = string.Format("AND Ttr_IDNo = @IDNo");
            string strDate = "";
            if (ProcessDate != "")
                strDate = string.Format("AND Ttr_Date = @ProcessDate");

            #region Main Query
            sqlMain = @"
        DECLARE @CurrentYear CHAR(2) = (SELECT RIGHT(Mph_CharValue, 2)
									        FROM M_PolicyHdr 
									        WHERE Mph_CompanyCode = @CompanyCode
									        AND Mph_PolicyCode ='COMPYEAR')

        DECLARE @PayCycleStartDate DATE
        DECLARE @PayCycleEndDate DATE
        DECLARE @OvertimeFlag CHAR(1)

        SELECT @OvertimeFlag = Tps_CycleIndicator 
            , @PayCycleStartDate = CONVERT(DATE,Tps_StartCycle)
            , @PayCycleEndDate = CONVERT(DATE,Tps_EndCycle)
        FROM T_PaySchedule WHERE Tps_PayCycle = @PayCycle


        DECLARE @DocBatchLastSeriesNo INT = (SELECT Tdn_LastSeriesNumber 
									        FROM T_DocumentNumber
									        WITH (UPDLOCK)
									        WHERE Tdn_DocumentCode = 'OTBATCH')

        DECLARE @DocBatchPrefix CHAR(1) = (SELECT Tdn_DocumentPrefix 
								        FROM T_DocumentNumber
								        WHERE Tdn_DocumentCode = 'OTBATCH')

        DECLARE @DocBatchNumber CHAR(12) =  @DocBatchPrefix + @CurrentYear + RIGHT(REPLICATE('0', 9) + CAST(@DocBatchLastSeriesNo AS VARCHAR(9)), 9)

        DECLARE @DocLastSeriesNo INT = (SELECT Tdn_LastSeriesNumber 
									        FROM T_DocumentNumber
									        WITH (UPDLOCK)
									        WHERE Tdn_DocumentCode = 'MOVERTIME')

        DECLARE @DocPrefix CHAR(1) = (SELECT Tdn_DocumentPrefix 
								        FROM T_DocumentNumber
								        WHERE Tdn_DocumentCode = 'MOVERTIME')

        DECLARE @OrigDocLastSeriesNo INT = @DocLastSeriesNo


        ---DELETE
        DELETE A
        FROM T_EmpOvertime A
        INNER JOIN T_EmpTimeRegister 
        ON A.Tot_IDNo = Ttr_IDNo
		AND A.Tot_OvertimeDate = Ttr_Date 
        INNER JOIN M_Employee ON Mem_IDNo =  Ttr_IDNo
	        AND Mem_Workstatus like 'A%'
        @USERCOSTCENTERACCESSCONDITION
        WHERE LEFT(A.Tot_DocumentNo, 1) = @DocPrefix 
          AND A.Tot_OvertimeStatus = '14'
          {1}
          {2}
      
        UPDATE T_EmpTimeRegister
        SET Ttr_WFOTAdvHr = AHrs
	        , Ttr_WFOTPostHr = OHrs
	        , Ludatetime = GETDATE()
        FROM T_EmpTimeRegister
        INNER JOIN ( SELECT Tot_IDNo
				        , Tot_OvertimeDate
				        , ISNULL(SUM(CASE WHEN Tot_OvertimeType = 'A' THEN Tot_OvertimeHours ELSE 0 END), 0) as AHrs
				        , ISNULL(SUM(CASE WHEN Tot_OvertimeType <> 'A' THEN Tot_OvertimeHours ELSE 0 END), 0) as OHrs
		            FROM T_EmpOvertime
                    INNER JOIN T_EmpTimeRegister 
                        ON Ttr_IDNo = Tot_IDNo
                        AND Ttr_Date = Tot_OvertimeDate
                    INNER JOIN M_Employee ON Mem_IDNo =  Ttr_IDNo
			            AND Mem_Workstatus like 'A%'
                    @USERCOSTCENTERACCESSCONDITION
		            WHERE LEFT(Tot_DocumentNo, 1) = @DocPrefix 
			            AND Tot_OvertimeStatus ='14'
                        {1}
                        {2}
		            GROUP BY Tot_IDNo, Tot_OvertimeDate
                    ) TEMP 
                    ON Ttr_IDNo = Tot_IDNo
                     AND Ttr_Date = Tot_OvertimeDate
                     

        
        DECLARE @Cnt SMALLINT = (SELECT COUNT(Ttr_IDNo) as [Cnt]
						        FROM  T_EmpTimeRegister
						        INNER JOIN M_Employee on Mem_IDNo =  Ttr_IDNo
							        AND Mem_Workstatus like 'A%'
						        @USERCOSTCENTERACCESSCONDITION
						        INNER JOIN {0}..M_Shift 
                                    ON Msh_CompanyCode = @CompanyCode
							        AND Msh_ShiftCode = Ttr_ShiftCode
                                INNER JOIN {0}..T_OvertimeGroupTmp 
                                    ON Tot_CompanyCode = @CompanyCode
							        AND Tot_CalendarGroup = Ttr_CalendarGroup
							        AND Tot_Date = Ttr_Date
						        WHERE CONVERT(DATE,Ttr_Date) BETWEEN  @PayCycleStartDate AND @PayCycleEndDate
						            AND Tot_AdvHrs > 0
                                    {1}
                                    {2})

        ---ADVANCE
        INSERT INTO T_EmpOvertime
        (Tot_DocumentNo
        , Tot_PayCycle
        , Tot_RequestType
        , Tot_RequestDate
        , Tot_IDNo
        , Tot_OvertimeDate
        , Tot_OvertimeType
        , Tot_StartTime
        , Tot_EndTime
        , Tot_OvertimeHours
        , Tot_ReasonForRequest
        , Tot_OvertimeClass
        , Tot_OvertimeStatus
        , Tot_OvertimeFlag
        , Tot_SubmittedBy
        , Tot_SubmittedDate
        , Tot_Authority1
        , Tot_Authority1Date
        , Tot_Authority2
        , Tot_Authority2Date
        , Tot_Authority3
        , Tot_Authority3Date
        , Tot_Authority4
        , Tot_Authority4Date
        , Tot_Authority5
        , Tot_Authority5Date
        , Tot_ShiftCode
        , Tot_CostcenterCode
        , Tot_EmploymentStatusCode
        , Tot_PayrollType
        , Tot_Grade
        , Tot_PayrollGroup
        , Tot_Authority1Comments
        , Tot_Authority2Comments
        , Tot_Authority3Comments
        , Tot_Authority4Comments
        , Tot_Authority5Comments
        , Tot_Delegation
        , Tot_DocumentBatchNo
        , Tot_OriginalDocumentNo
        , Tot_CreatedBy
        , Tot_CreatedDate
        , Tot_UpdatedBy
        , Tot_UpdatedDate)

        SELECT  @DocPrefix + @CurrentYear + RIGHT(REPLICATE('0', 9) + CAST(@DocLastSeriesNo +  ROW_NUMBER() OVER (ORDER BY Ttr_IDNo)  AS VARCHAR(9)), 9)
	        , @PayCycle
	        , 'A'
	        , GETDATE()
	        , Ttr_IDNo
	        , Ttr_Date
	        , 'A'
	        , CASE WHEN Ttr_DayCode = 'REG' THEN
		        {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1, -Tot_AdvHrs * 60) 
	          ELSE
		          Msh_ShiftIn1
	          END as [StartTime]
	        , CASE WHEN Ttr_DayCode = 'REG' THEN
		        {0}.dbo.Udf_AddMinutes({0}.dbo.Udf_AddMinutes(Msh_ShiftIn1, -Tot_AdvHrs * 60), Tot_AdvHrs * 60)
              ELSE -- Only Post OT Type on Non-Regular Day
			        CASE WHEN Tot_PostHrs <> {0}.dbo.[Udf_ComputeHoursBasedOnStartEndTime](@CompanyCode,Ttr_DayCode, Msh_ShiftIn1, {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ) THEN -- If with breaktime
				        {0}.dbo.Udf_AddMinutes({0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ,(Tot_AdvHrs-{0}.dbo.[Udf_ComputeHoursBasedOnStartEndTime](@CompanyCode,Ttr_DayCode, Msh_ShiftIn1, {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ))*60) 
			        ELSE
				        {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1, Tot_PostHrs * 60)
			        END
	          END as [EndTime]
	        , Tot_AdvHrs
	        , 'MANDATORY OVERTIME'
	        , 'PO'
	        , '14'
	        , @OvertimeFlag
	        , @LoginUser
	        , GETDATE()
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , Ttr_ShiftCode
	        , Ttr_CostcenterCode
	        , Ttr_EmploymentStatusCode
	        , Ttr_PayrollType
	        , Ttr_Grade
	        , Ttr_PayrollGroup
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , @DocBatchNumber
	        , NULL
	        , @LoginUser
	        , GETDATE()
	        , NULL
	        , NULL
        FROM  T_EmpTimeRegister
        INNER JOIN M_Employee on Mem_IDNo =  Ttr_IDNo
	        AND Mem_Workstatus like 'A%'
        @USERCOSTCENTERACCESSCONDITION
        INNER JOIN {0}..M_Shift ON Msh_CompanyCode = @CompanyCode
	        AND Msh_ShiftCode = Ttr_ShiftCode
        INNER JOIN {0}..T_OvertimeGroupTmp ON Tot_CompanyCode = @CompanyCode
	        AND Tot_CalendarGroup = Ttr_CalendarGroup
	        AND Tot_Date = Ttr_Date
        WHERE CONVERT(DATE,Ttr_Date) BETWEEN  @PayCycleStartDate AND @PayCycleEndDate
            AND Tot_AdvHrs > 0
            {1}
            {2}

    
        SET @DocLastSeriesNo = @DocLastSeriesNo +  @Cnt
        ---PRINT @DocLastSeriesNo

        SET @Cnt = 0
        SET @Cnt = (SELECT COUNT(Ttr_IDNo) as [Cnt]
						        FROM  T_EmpTimeRegister
						        INNER JOIN M_Employee on Mem_IDNo =  Ttr_IDNo
							        AND Mem_Workstatus like 'A%'
						        @USERCOSTCENTERACCESSCONDITION
						        INNER JOIN {0}..M_Shift 
                                    ON Msh_CompanyCode = @CompanyCode
							        AND Msh_ShiftCode = Ttr_ShiftCode
                                INNER JOIN {0}..T_OvertimeGroupTmp 
                                    ON Tot_CompanyCode = @CompanyCode
							        AND Tot_CalendarGroup = Ttr_CalendarGroup
							        AND Tot_Date = Ttr_Date
						        WHERE CONVERT(DATE,Ttr_Date) BETWEEN  @PayCycleStartDate AND @PayCycleEndDate
						            AND Tot_MidHrs > 0
                                    {1}
                                    {2})

        ---MID
        INSERT INTO T_EmpOvertime
        (Tot_DocumentNo
        , Tot_PayCycle
        , Tot_RequestType
        , Tot_RequestDate
        , Tot_IDNo
        , Tot_OvertimeDate
        , Tot_OvertimeType
        , Tot_StartTime
        , Tot_EndTime
        , Tot_OvertimeHours
        , Tot_ReasonForRequest
        , Tot_OvertimeClass
        , Tot_OvertimeStatus
        , Tot_OvertimeFlag
        , Tot_SubmittedBy
        , Tot_SubmittedDate
        , Tot_Authority1
        , Tot_Authority1Date
        , Tot_Authority2
        , Tot_Authority2Date
        , Tot_Authority3
        , Tot_Authority3Date
        , Tot_Authority4
        , Tot_Authority4Date
        , Tot_Authority5
        , Tot_Authority5Date
        , Tot_ShiftCode
        , Tot_CostcenterCode
        , Tot_EmploymentStatusCode
        , Tot_PayrollType
        , Tot_Grade
        , Tot_PayrollGroup
        , Tot_Authority1Comments
        , Tot_Authority2Comments
        , Tot_Authority3Comments
        , Tot_Authority4Comments
        , Tot_Authority5Comments
        , Tot_Delegation
        , Tot_DocumentBatchNo
        , Tot_OriginalDocumentNo
        , Tot_CreatedBy
        , Tot_CreatedDate
        , Tot_UpdatedBy
        , Tot_UpdatedDate)

        SELECT  @DocPrefix + @CurrentYear + RIGHT(REPLICATE('0', 9) + CAST(@DocLastSeriesNo +  ROW_NUMBER() OVER (ORDER BY Ttr_IDNo)  AS VARCHAR(9)), 9)
	        , @PayCycle
	        , 'A'
	        , GETDATE()
	        , Ttr_IDNo
	        , Ttr_Date
	        , 'M'
	        , CASE WHEN Ttr_DayCode = 'REG' THEN
		        Msh_ShiftOut1 
	          ELSE
		          Msh_ShiftIn1
	          END as [StartTime]
	        , CASE WHEN Ttr_DayCode = 'REG' THEN
		        {0}.dbo.Udf_AddMinutes(Msh_ShiftOut1, Tot_MidHrs * 60) 
              ELSE -- Only Post OT Type on Non-Regular Day
			        CASE WHEN Tot_PostHrs <> {0}.dbo.[Udf_ComputeHoursBasedOnStartEndTime](@CompanyCode,Ttr_DayCode, Msh_ShiftIn1, {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ) THEN -- If with breaktime
				        {0}.dbo.Udf_AddMinutes({0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ,(Tot_AdvHrs-{0}.dbo.[Udf_ComputeHoursBasedOnStartEndTime](@CompanyCode,Ttr_DayCode, Msh_ShiftIn1, {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ))*60) 
			        ELSE
				        {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1, Tot_PostHrs * 60)
			        END
	          END as [EndTime]
	        , Tot_MidHrs
	        , 'MANDATORY OVERTIME'
	        , 'PO'
	        , '14'
	        , @OvertimeFlag
	        , @LoginUser
	        , GETDATE()
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , Ttr_ShiftCode
	        , Ttr_CostcenterCode
	        , Ttr_EmploymentStatusCode
	        , Ttr_PayrollType
	        , Ttr_Grade
	        , Ttr_PayrollGroup
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , @DocBatchNumber
	        , NULL
	        , @LoginUser
	        , GETDATE()
	        , NULL
	        , NULL
        FROM  T_EmpTimeRegister
        INNER JOIN M_Employee on Mem_IDNo =  Ttr_IDNo
	        AND Mem_Workstatus like 'A%'
        @USERCOSTCENTERACCESSCONDITION
        INNER JOIN {0}..M_Shift ON Msh_CompanyCode = @CompanyCode
	        AND Msh_ShiftCode = Ttr_ShiftCode
        INNER JOIN {0}..T_OvertimeGroupTmp ON Tot_CompanyCode = @CompanyCode
	        AND Tot_CalendarGroup = Ttr_CalendarGroup
	        AND Tot_Date = Ttr_Date
        WHERE CONVERT(DATE,Ttr_Date) BETWEEN @PayCycleStartDate AND  @PayCycleEndDate
            AND Tot_MidHrs > 0
            {1}
            {2}

        SET @DocLastSeriesNo = @DocLastSeriesNo +  @Cnt
		---PRINT @DocLastSeriesNo

        SET @Cnt = 0
        SET @Cnt = (SELECT COUNT(Ttr_IDNo) as [Cnt]
						        FROM  T_EmpTimeRegister
						        INNER JOIN M_Employee on Mem_IDNo =  Ttr_IDNo
							        AND Mem_Workstatus like 'A%'
						        @USERCOSTCENTERACCESSCONDITION
						        INNER JOIN {0}..M_Shift 
                                    ON Msh_CompanyCode = @CompanyCode
							        AND Msh_ShiftCode = Ttr_ShiftCode
                                INNER JOIN {0}..T_OvertimeGroupTmp 
                                    ON Tot_CompanyCode = @CompanyCode
							        AND Tot_CalendarGroup = Ttr_CalendarGroup
							        AND Tot_Date = Ttr_Date
						        WHERE CONVERT(DATE,Ttr_Date) BETWEEN  @PayCycleStartDate AND @PayCycleEndDate
						            AND Tot_PostHrs > 0
                                    {1}
                                    {2})

        ---POST
        INSERT INTO T_EmpOvertime
        (Tot_DocumentNo
        , Tot_PayCycle
        , Tot_RequestType
        , Tot_RequestDate
        , Tot_IDNo
        , Tot_OvertimeDate
        , Tot_OvertimeType
        , Tot_StartTime
        , Tot_EndTime
        , Tot_OvertimeHours
        , Tot_ReasonForRequest
        , Tot_OvertimeClass
        , Tot_OvertimeStatus
        , Tot_OvertimeFlag
        , Tot_SubmittedBy
        , Tot_SubmittedDate
        , Tot_Authority1
        , Tot_Authority1Date
        , Tot_Authority2
        , Tot_Authority2Date
        , Tot_Authority3
        , Tot_Authority3Date
        , Tot_Authority4
        , Tot_Authority4Date
        , Tot_Authority5
        , Tot_Authority5Date
        , Tot_ShiftCode
        , Tot_CostcenterCode
        , Tot_EmploymentStatusCode
        , Tot_PayrollType
        , Tot_Grade
        , Tot_PayrollGroup
        , Tot_Authority1Comments
        , Tot_Authority2Comments
        , Tot_Authority3Comments
        , Tot_Authority4Comments
        , Tot_Authority5Comments
        , Tot_Delegation
        , Tot_DocumentBatchNo
        , Tot_OriginalDocumentNo
        , Tot_CreatedBy
        , Tot_CreatedDate
        , Tot_UpdatedBy
        , Tot_UpdatedDate)

        SELECT  @DocPrefix + @CurrentYear + RIGHT(REPLICATE('0', 9) + CAST(@DocLastSeriesNo +  ROW_NUMBER() OVER (ORDER BY Ttr_IDNo)  AS VARCHAR(9)), 9)
	        , @PayCycle
	        , 'A'
	        , GETDATE()
	        , Ttr_IDNo
	        , Ttr_Date
	        , 'P'
	        , CASE WHEN Ttr_DayCode = 'REG' THEN
		        Msh_ShiftOut2 
	          ELSE
		          Msh_ShiftIn1
	          END as [StartTime]
	        , CASE WHEN Ttr_DayCode = 'REG' THEN
		        {0}.dbo.Udf_AddMinutes(Msh_ShiftOut2, Tot_PostHrs * 60)
              ELSE -- Only Post OT Type on Non-Regular Day
			        CASE WHEN Tot_PostHrs <> {0}.dbo.[Udf_ComputeHoursBasedOnStartEndTime](@CompanyCode,Ttr_DayCode, Msh_ShiftIn1, {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ) THEN -- If with breaktime
				        {0}.dbo.Udf_AddMinutes({0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ,(Tot_AdvHrs-{0}.dbo.[Udf_ComputeHoursBasedOnStartEndTime](@CompanyCode,Ttr_DayCode, Msh_ShiftIn1, {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1,Tot_PostHrs*60) ))*60) 
			        ELSE
				        {0}.dbo.Udf_AddMinutes(Msh_ShiftIn1, Tot_PostHrs * 60)
			        END
	          END as [EndTime]
	        , Tot_PostHrs
	        , 'MANDATORY OVERTIME'
	        , 'PO'
	        , '14'
	        , @OvertimeFlag
	        , @LoginUser
	        , GETDATE()
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , Ttr_ShiftCode
	        , Ttr_CostcenterCode
	        , Ttr_EmploymentStatusCode
	        , Ttr_PayrollType
	        , Ttr_Grade
	        , Ttr_PayrollGroup
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , NULL
	        , @DocBatchNumber
	        , NULL
	        , @LoginUser
	        , GETDATE()
	        , NULL
	        , NULL
        FROM  T_EmpTimeRegister
        INNER JOIN M_Employee on Mem_IDNo =  Ttr_IDNo
	        AND Mem_Workstatus like 'A%'
        @USERCOSTCENTERACCESSCONDITION
        INNER JOIN {0}..M_Shift ON Msh_CompanyCode = @CompanyCode
	        AND Msh_ShiftCode = Ttr_ShiftCode
        INNER JOIN {0}..T_OvertimeGroupTmp ON Tot_CompanyCode = @CompanyCode
	        AND Tot_CalendarGroup = Ttr_CalendarGroup
	        AND Tot_Date = Ttr_Date
        WHERE CONVERT(DATE,Ttr_Date) BETWEEN @PayCycleStartDate AND @PayCycleEndDate
            AND Tot_PostHrs > 0
            {1}
            {2}

        SET @DocLastSeriesNo = @DocLastSeriesNo +  @Cnt
	    ---PRINT @DocLastSeriesNo

	    UPDATE T_DocumentNumber
	    SET Tdn_LastSeriesNumber = @DocLastSeriesNo 
		   , Ludatetime = GETDATE()
	    WHERE Tdn_DocumentCode = 'MOVERTIME'

        IF (@OrigDocLastSeriesNo <> @DocLastSeriesNo)
            UPDATE T_DocumentNumber
	            SET Tdn_LastSeriesNumber = @DocBatchLastSeriesNo + 1
		           , Ludatetime = GETDATE()
	            WHERE Tdn_DocumentCode = 'OTBATCH'
        ";
            #endregion

            sqlMain = sqlMain.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "TIME", UserCode, "Ttr_Costcentercode", "Ttr_PayrollGroup", "Ttr_EmploymentStatusCode", "Ttr_PayrollType", CompanyCode, CentralProfile, false));

            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            if (!ProcessDate.Equals(""))
                paramInfo[idxx++] = new ParameterInfo("@ProcessDate", ProcessDate, SqlDbType.Date);
            else
                paramInfo[idxx++] = new ParameterInfo("@ProcessDate", DBNull.Value, SqlDbType.Date);

            paramInfo[idxx++] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@LoginUser", UserCode);

            sqlMain = string.Format(sqlMain
                                    , CentralProfile
                                    , strIDNumber
                                    ,  strDate);

            dal.ExecuteNonQuery(sqlMain, CommandType.Text, paramInfo);
        }

        #endregion

        #region Parameters and Process Flags

        public bool GetProcessFlag(string SystemCode, string SettingCode)
        {
            string sqlQuery = @"SELECT	Tsc_SetFlag 
                                FROM T_SettingControl 
                                WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND	Tsc_SettingCode = @Tsc_SettingCode";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tsc_SystemCode", SystemCode);
            paramInfo[1] = new ParameterInfo("@Tsc_SettingCode", SettingCode);

            try
            {
                return Convert.ToBoolean(dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0].Rows[0][0]);
            }
            catch(Exception x)
            {
                MessageBox.Show(x.ToString());
                CommonProcedures.ShowMessage(10072, SystemCode + " - " + SettingCode, "");
                return false;
            }
        }

        public int UpdateProcessControlFlag(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode, string user, string companyCode, string PayCycleCode)
        {
            int retVal = 0;
            CommonBL cmnBL = new CommonBL();

            #region update query
            string Upstring = @"UPDATE T_SettingControl
                                SET Tsc_SetFlag    = @Tsc_SetFlag
                                    ,Usr_Login     = @Usr_Login
                                    ,Ludatetime    = GetDate()
                                WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode

                                INSERT INTO T_SettingControlTrl
                                SELECT  Tsc_SystemCode
                                     , Tsc_SettingCode
                                     , Tsc_SettingName
                                     , @Tsc_SetFlag
                                     , Tsc_AccessType
                                     , @Tsc_PayCycle
                                     , NULL
                                     , 'A'
                                     , @Usr_Login
                                     , GETDATE()
                                    FROM T_SettingControl
                                 WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode";
            #endregion

            ParameterInfo[] UpparamInfo = new ParameterInfo[5];
            UpparamInfo[0] = new ParameterInfo("@Tsc_SetFlag", Tsc_SetFlag);
            UpparamInfo[1] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            UpparamInfo[2] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);
            UpparamInfo[3] = new ParameterInfo("@Tsc_PayCycle", PayCycleCode);
            UpparamInfo[4] = new ParameterInfo("@Usr_Login", user);

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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public int UpdateProcessControlFlagWithStartDate(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode, string user, DateTime dateStart, DateTime dateEnd)
        {
            int retVal = 0;
            CommonBL cmnBL = new CommonBL();

            #region update query
            string Upstring = @"UPDATE T_SettingControl
                                SET Tsc_SetFlag    = @Tsc_SetFlag
                                    ,Usr_Login     = @Usr_Login
                                    ,Ludatetime    = GetDate()
                                WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode

                                INSERT INTO T_SettingControlTrl
                                SELECT  Tsc_SystemCode
                                     , Tsc_SettingCode
                                     , Tsc_SettingName
                                     , @Tsc_SetFlag
                                     , Tsc_AccessType
                                     , @Tsc_PayCycle
                                     , @Tsc_Start
                                     , 'A'
                                     , @Usr_Login
                                     , @Ludatetime
                                    FROM T_SettingControl
                                 WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode";
            #endregion

            ParameterInfo[] UpparamInfo = new ParameterInfo[7];
            UpparamInfo[0] = new ParameterInfo("@Tsc_SetFlag", Tsc_SetFlag);
            UpparamInfo[1] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            UpparamInfo[2] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);
            UpparamInfo[3] = new ParameterInfo("@Tsc_PayCycle", cmnBL.GetCurrentPayPeriod());
            UpparamInfo[4] = new ParameterInfo("@Usr_Login", user);
            UpparamInfo[5] = new ParameterInfo("@Tsc_Start", dateStart);
            UpparamInfo[6] = new ParameterInfo("@Ludatetime", dateEnd);

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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
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
            string strPayrollGroup = string.Empty;
            string strPayType = string.Empty;
            string strEmpStatus = string.Empty;
            string strCalendarGrp = string.Empty;
            string strWorkLocation = string.Empty;
            string strPositionGrade = string.Empty;
            string strShift = string.Empty;

            string strAllowanceColNo = "00";
            //string strDispatchedExclusion = "FALSE";
            string unit = string.Empty;
            string delimeter = ",";
            string[] rows;
            decimal RGOTOFFSET = 0;

            DataTable dtAllowanceDetails, dtLogLedger, dtOvertime, dtAllowanceAmt;
            DataRow[] drArrOvertime;
            bool bCheckOT;
            bool bFound;
            #endregion

            #region Initialize Conditions
            string strForHist = string.Empty;
            string strEmpMasterHistCondition = string.Empty;
            string strClearAllowanceCondition = string.Empty;
            string strHourBasedAlwCondition = string.Empty;
            //string strJobStatusCondition = string.Empty;
            string strSpecialAlwCondition = string.Empty;
            string strMonthlyAlwCondition = string.Empty;

            if (!currentflag)
            {
                strForHist = "Hst";
                strEmpMasterHistCondition = "AND Mem_PayCycle = Ttr_PayCycle";
            }

            if (!ProcessAll && EmployeeId != "")
            {
                strClearAllowanceCondition = " AND Ttr_IDNo = '" + EmployeeId + "'";
                strHourBasedAlwCondition = " AND Ttr_IDNo = '" + EmployeeId + "'";
                strSpecialAlwCondition = " AND Tds_IDNo = '" + EmployeeId + "'";
                strMonthlyAlwCondition = " AND a.Ttr_IDNo = '" + EmployeeId + "'";
            }
            else if (ProcessAll == true && EmployeeList != "")
            {
                strClearAllowanceCondition = " AND Ttr_IDNo IN (" + EmployeeList + ")";
                strHourBasedAlwCondition = " AND Ttr_IDNo IN (" + EmployeeList + ")";
                strSpecialAlwCondition = " AND Tds_IDNo IN (" + EmployeeList + ")";
                strMonthlyAlwCondition = " AND a.Ttr_IDNo IN (" + EmployeeList + ")";
            }
            else
            {
                strClearAllowanceCondition = "";
                strHourBasedAlwCondition = "";
                strSpecialAlwCondition = "";
                strMonthlyAlwCondition = "";
            }

            //if (forClearanceComputation == "")
            //    strJobStatusCondition = " --AND LEFT(Mem_WorkStatus,1) = 'A'";
            #endregion

            #region Query for Allowance Initialization
            string strClearAllowance = @"UPDATE T_EmpTimeRegister{0}
                                        SET Ttr_TBAmt{1} = 0
                                        WHERE Ttr_PayCycle = '{2}'
                                            {3}";
            #endregion

            #region Query for Get Posting Type Formula Value
            string sqlTimeBasedFormula = @" SELECT Mdm_Formula 
                                            FROM {2}..M_VarianceAllowanceHdr
								            INNER JOIN M_Formula ON Mdm_MainCode = 'TIMEBASEHB'
								                AND Mvh_ComputationType = Mdm_SubCode
								            WHERE Mvh_TimeBaseID = '{0}'
                                                AND Mvh_CompanyCode  = '{1}'";
            #endregion

            #region Query for Hour-based
            string sqlUpdateRegOT = @" UPDATE T_EmpTimeRegister{6}
                                       SET Ttr_TBAmt{0} = ISNULL({12}.dbo.GetAllowanceAmountHourBased(
								                                         Ttr_Date
								                                       , Ttr_IDNo
                                                                       , '{13}'
								                                       , '{0}'
								                                       , ({11})
								                                       , Ttr_DayCode
                                                                       , Mvh_BaseTable
                                                                       , Mvh_MinAllowance
                                                                       , Mvh_MaxAllowance),0)
                                          FROM T_EmpTimeRegister{6}
                                          JOIN M_Employee{6} ON Mem_IDNo = Ttr_IDNo     
                                            {8}                                      
                                          JOIN {12}..M_VarianceAllowanceHdr 
                                                ON Mvh_TimeBaseID = '{0}'
                                                AND Mvh_CompanyCode = '{13}'       
                                         WHERE Ttr_PayCycle = '{1}'
                                           {4}
				                           {9}
                                           {2} 
                                           {7}
				                           {3} 
                                           {14}
                                           {15}
                                           {16}";
            #endregion

            #region Query for Time-based
            string strGetAllowanceTimeBased = @"SELECT ISNULL(dbo.GetAllowanceAmountTimeBased(
											     '{0}'
											   , '{1}'
                                               , '{7}' ---Company Code
											   , '{2}'
											   , '{3}'
											   , '{4}'
											   , {5}
											   , {6}),0) as Amount";

            string sqlUpdateRegOTTimeBased = @"UPDATE T_EmpTimeRegister{6}
                                                SET Ttr_TBAmt{0} = {8}
                                                  FROM T_EmpTimeRegister{6}
                                                  JOIN M_Employee{6}
	                                                ON Mem_IDNo = Ttr_IDNo     
                                                    {10}                                      
                                                  JOIN {12}..M_VarianceAllowanceHdr
	                                                ON Mvh_TimeBaseID = '{0}'
                                                    AND Mvh_CompanyCode = '{13}'
                                                 WHERE Ttr_PayCycle = '{1}'
                                                   AND Ttr_IDNo = '{4}'
                                                   AND Ttr_Date = '{9}'
                                                    {11} 
				                                    {7}
					                                {2}
					                                {3}
						                            {14}
						                            {15}
                                                    {16}";
            #endregion

            DataTable dtHeader = dalHelper.ExecuteDataSet(string.Format("SELECT * FROM {0}.dbo.M_VarianceAllowanceHdr WHERE Mvh_RecordStatus = 'A' AND Mvh_CompanyCode = '{1}'"
                                            , CentralProfile
                                            , CompanyCode)).Tables[0];
            foreach (DataRow drHeader in dtHeader.Rows)
            {
                #region Initialize values
                sqlUpdateHeader = string.Empty;
                strAllowanceColNo = drHeader["Mvh_TimeBaseID"].ToString().Trim().PadLeft(2, '0');
                //strDispatchedExclusion = drHeader["Mvh_ExcludeDispatch"].ToString().Trim().ToUpper();
                #endregion

                #region Payroll Group
                unit = string.Empty;
                strPayrollGroup = drHeader["Mvh_PayrollGroup"].ToString().Trim();
                rows = strPayrollGroup.Split(",".ToCharArray());
                strPayrollGroup = "";
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
                    strPayrollGroup += string.Format(@"  AND (Ttr_PayrollGroup IN ({0}) OR Mvh_PayrollGroup = 'ALL')", unit);
                else
                    strPayrollGroup += string.Empty;
                #endregion

                #region Payroll Type
                unit = string.Empty;
                strPayType = drHeader["Mvh_PayrollType"].ToString().Trim();
                rows = strPayType.Split(",".ToCharArray());
                strPayType = "";
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
                            strPayType += @"  and Mem_IsTaxExempted = 1 ";
                        else if (r == "!MW")
                            strPayType += @"  and Mem_IsTaxExempted = 0 ";
                    }
                }
                if (unit.Trim() != "")
                    strPayType += string.Format(@"  AND (Ttr_PayrollType IN ({0}) OR Mvh_PayrollType = 'ALL')", unit);
                else
                    strPayType += string.Empty;
                #endregion

                #region Employment Status
                unit = string.Empty;
                delimeter = ",";
                strEmpStatus = drHeader["Mvh_EmploymentStatus"].ToString().Trim();
                rows = strEmpStatus.Split(",".ToCharArray());
                strEmpStatus = "";
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
                    strEmpStatus = string.Format(@"  AND (Ttr_EmploymentStatusCode IN ({0}) OR Mvh_EmploymentStatus = 'ALL')", unit);
                else
                    strEmpStatus = string.Empty;
                #endregion

                #region Calendar Group
                unit = string.Empty;
                delimeter = ",";
                strCalendarGrp = drHeader["Mvh_CalendarGrp"].ToString().Trim();
                rows = strCalendarGrp.Split(",".ToCharArray());
                strCalendarGrp = "";
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
                    strCalendarGrp = string.Format(@"  AND (Ttr_CalendarGroup IN ({0}) OR Mvh_CalendarGrp = 'ALL')", unit);
                else
                    strCalendarGrp = string.Empty;
                #endregion

                #region Work Location
                unit = string.Empty;
                delimeter = ",";
                strWorkLocation = drHeader["Mvh_WorkLocation"].ToString().Trim();
                rows = strWorkLocation.Split(",".ToCharArray());
                strWorkLocation = "";
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
                    strWorkLocation = string.Format(@"  AND (Ttr_WorkLocationCode IN ({0}) OR Mvh_WorkLocation = 'ALL')", unit);
                else
                    strWorkLocation = string.Empty;
                #endregion

                #region Position Grade
                unit = string.Empty;
                strPositionGrade = drHeader["Mvh_Grade"].ToString().Trim();
                rows = strPositionGrade.Split(",".ToCharArray());
                strPositionGrade = "";
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
                    strPositionGrade += string.Format(@"  AND (Ttr_Grade IN ({0}) OR Mvh_Grade = 'ALL')", unit);
                else
                    strPositionGrade += string.Empty;
                #endregion


                #region Shift
                unit = string.Empty;
                strShift = drHeader["Mvh_ShiftCode"].ToString().Trim();
                rows = strShift.Split(",".ToCharArray());
                strShift = "";
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
                    strShift += string.Format(@"  AND (Ttr_ShiftCode IN ({0}) OR Mvh_Grade = 'ALL')", unit);
                else
                    strShift += string.Empty;
                #endregion


                #region Cleanup allowance values
                sqlUpdateHeader = string.Format(strClearAllowance, strForHist, strAllowanceColNo, PayPeriod, strClearAllowanceCondition);
                dalHelper.ExecuteNonQuery(sqlUpdateHeader);
                #endregion

                #region Get Posting Type Formula Value
                string sqlTimeBasedValue = "0";
                DataSet dsTimeBasedFormula = dalHelper.ExecuteDataSet(string.Format(sqlTimeBasedFormula, strAllowanceColNo, CompanyCode, CentralProfile));
                if (dsTimeBasedFormula != null && dsTimeBasedFormula.Tables.Count > 0 && dsTimeBasedFormula.Tables[0].Rows.Count > 0)
                {
                    sqlTimeBasedValue = dsTimeBasedFormula.Tables[0].Rows[0][0].ToString();
                }
                #endregion

                #region Hour Based
                if (drHeader["Mvh_BaseType"].ToString().Equals("H")) //Hour Based
                {
                    sqlUpdateHeader = string.Format(sqlUpdateRegOT
                                                        , strAllowanceColNo             //0
                                                        , PayPeriod                     //1
                                                        , strEmpStatus                  //2
                                                        , strCalendarGrp                //3
                                                        , strHourBasedAlwCondition      //4
                                                        , RGOTOFFSET                    //5
                                                        , strForHist                    //6
                                                        , strPayType                    //7
                                                        , strEmpMasterHistCondition     //8
                                                        , strPayrollGroup               //9
                                                        , strSpecialAlwCondition        //10
                                                        , sqlTimeBasedValue             //11
                                                        , CentralProfile
                                                        , CompanyCode                   //13
                                                        , strWorkLocation               //14
                                                        , strPositionGrade              //15
                                                        , strShift                      //16
                                                        );                    
                    dalHelper.ExecuteNonQuery(sqlUpdateHeader);
                }
                #endregion

                #region Time Based
                if (drHeader["Mvh_BaseType"].ToString().Equals("T"))
                {
                    dtAllowanceDetails = GetAllowanceDetails(strAllowanceColNo);
                    bCheckOT = false;
                    bFound = false;
                    foreach (DataRow drAllowanceDetail in dtAllowanceDetails.Rows)
                    {
                        if (drAllowanceDetail["Mvh_ComputationType"].ToString().Equals("O") || drAllowanceDetail["Mvh_ComputationType"].ToString().Equals("A"))
                            bCheckOT = true;

                        dtLogLedger = GetEmployeesLoggedOutPastTimeQuota(ProcessAll, EmployeeId, EmployeeList
                                                                        , drAllowanceDetail["Mvd_DayCode"].ToString()
                                                                        , drAllowanceDetail["Mvd_StartTime"].ToString()
                                                                        , drAllowanceDetail["Mvd_EndTime"].ToString()
                                                                        , bCheckOT
                                                                        , currentflag);
                        dtOvertime = GetAllOvertimeRecords(ProcessAll, EmployeeId, EmployeeList
                                                            , drAllowanceDetail["Mvd_StartTime"].ToString()
                                                            , drAllowanceDetail["Mvd_EndTime"].ToString());

                        foreach (DataRow drLogLedger in dtLogLedger.Rows)
                        {
                            bFound = false;
                            if (bCheckOT)
                            {
                                //Compare with OT applications
                                drArrOvertime = dtOvertime.Select(string.Format("Tot_IDNo = '{0}' and Tot_OvertimeDate = '{1}'", drLogLedger["Ttr_IDNo"], drLogLedger["Ttr_Date"]));
                                if (drArrOvertime.Length > 0)
                                    bFound = true;
                            }
                            else
                                bFound = true; //Actual OUT is past the allowance time start

                            if (bFound)
                            {
                                //Save Allowance (Not all records can be saved because of other factors like effectivity date, min-max allowance amount, or data setup errors)
                                string sqlquery = string.Format(strGetAllowanceTimeBased
                                                                , drLogLedger["Ttr_Date"].ToString()                        //0
                                                                , drLogLedger["Ttr_IDNo"].ToString()                        //1
                                                                , strAllowanceColNo                                         //2
                                                                , drLogLedger["Ttr_DayCode"].ToString()                     //3
                                                                , drAllowanceDetail["Mvh_BaseTable"].ToString()             //4
                                                                , drAllowanceDetail["Mvh_MinAllowance"].ToString()          //5
                                                                , drAllowanceDetail["Mvh_MaxAllowance"].ToString()          //6
                                                                , CompanyCode);                                             //7
                                dtAllowanceAmt = dalHelper.ExecuteDataSet(sqlquery).Tables[0];

                                if (dtAllowanceAmt.Rows.Count > 0)
                                {
                                    sqlUpdateHeader = string.Format(sqlUpdateRegOTTimeBased
                                                                        , strAllowanceColNo                         //0
                                                                        , PayPeriod                                 //1
                                                                        , strEmpStatus                              //2
                                                                        , strCalendarGrp                            //3
                                                                        , drLogLedger["Ttr_IDNo"].ToString()        //4
                                                                        , RGOTOFFSET                                //5
                                                                        , strForHist                                //6
                                                                        , strPayType                                //7
                                                                        , dtAllowanceAmt.Rows[0]["Amount"]          //8
                                                                        , drLogLedger["Ttr_Date"].ToString()        //9
                                                                        , strEmpMasterHistCondition                 //10
                                                                        , strPayrollGroup                           //11
                                                                        , CentralProfile
                                                                        , CompanyCode                               //13
                                                                        , strWorkLocation                           //14
                                                                        , strPositionGrade                          //15  
                                                                        , strShift                                  //16
                                                                        );
                                    dalHelper.ExecuteNonQuery(sqlUpdateHeader);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Monthly Allowance computation
                if (!Convert.ToBoolean(drHeader["Mvh_ComputePerDay"].ToString()))
                {
                    #region Get divisor amount
                    string strDivisorQuery = "";
                    if (drHeader["Mvh_ProrationDivisor"].ToString().Equals("R")) //Regular days only
                    {
                        strDivisorQuery = string.Format(@"SELECT Ttr_IDNo as EmployeeId, COUNT(*) as Divisor
                                                            FROM T_EmpTimeRegister
                                                            WHERE Ttr_RestDayFlag != 1 AND Ttr_HolidayFlag != 1 AND Ttr_PayCycle = '{0}'
                                                            GROUP BY Ttr_IDNo", PayPeriod);
                    }
                    else if (drHeader["Mvh_ProrationDivisor"].ToString().Equals("A")) //All days
                    {
                        strDivisorQuery = string.Format(@"SELECT Ttr_IDNo as EmployeeId, COUNT(*) as Divisor
                                                            FROM T_EmpTimeRegister
                                                            WHERE Ttr_PayCycle = '{0}'
                                                            GROUP BY Ttr_IDNo", PayPeriod);
                    }
                    else //All working days
                    {
                        strDivisorQuery = string.Format(@"SELECT Ttr_IDNo as EmployeeId, sum(Ttr_WorkDay) as Divisor
                                                            FROM T_EmpTimeRegister
                                                            WHERE Ttr_PayCycle = '{0}'
                                                            GROUP BY Ttr_IDNo", PayPeriod);
                    }
                    #endregion

                    #region Update monthly allowance
                    string strMonthlyRateQuery = @"UPDATE T_EmpTimeRegister
                                                    SET Ttr_TBAmt{0} = AllowanceAmt
                                                    FROM T_EmpTimeRegister a
                                                    INNER JOIN
                                                    (
                                                    SELECT Ttr_IDNo, SUM(Ttr_TBAmt{0})/Divisor as AllowanceAmt
                                                    FROM T_EmpTimeRegister b
                                                    INNER JOIN (
                                                        {1}
                                                    ) c
                                                    ON c.EmployeeId = b.Ttr_IDNo
                                                    WHERE Ttr_TBAmt{0} > 0
                                                    GROUP BY Ttr_IDNo, Divisor
                                                    ) d
                                                    ON d.Ttr_IDNo = a.Ttr_IDNo
                                                    WHERE Ttr_Date = (SELECT Tps_StartCycle 
			                                                        FROM T_PaySchedule 
			                                                        WHERE Tps_PayCycle = '{2}')
                                                    {3}

                                                    UPDATE T_EmpTimeRegister
                                                    SET Ttr_TBAmt{0} = 0
                                                    WHERE Ttr_Date != (SELECT Tps_StartCycle 
				                                                        FROM T_PaySchedule 
				                                                        WHERE Tps_PayCycle = '{2}')
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
                strEmployeeDlyAlwCondition = " AND Tta_IDNo = '" + EmployeeId + "'";
                strEmployeeLedgerCondition = " AND Ttr_IDNo = '" + EmployeeId + "'";
            }
            else if (ProcessAll == true && EmployeeList != "")
            {
                strEmployeeDlyAlwCondition = " AND Tta_IDNo IN (" + EmployeeList + ")";
                strEmployeeLedgerCondition = " AND Ttr_IDNo IN (" + EmployeeList + ")";
            }
            else
            {
                strEmployeeDlyAlwCondition = "";
                strEmployeeLedgerCondition = "";
            }

            dalHelper.ExecuteNonQuery(string.Format(@"DELETE FROM T_EmpTimeBaseAllowanceCycle 
                                                      WHERE Tta_CycleIndicator = 'C' 
                                                        AND Tta_PayCycle = Tta_OrigPayCycle
                                                        AND Tta_PayCycle = '{1}'
                                                        {0}", strEmployeeDlyAlwCondition, PayPeriod));
            string sqlInsert = @" INSERT INTO T_EmpTimeBaseAllowanceCycle ";
            for (int i = 1; i <= 12; i++)
            {
                sqlInsert += @"
                                  SELECT Ttr_IDNo Tta_IDNo
		                                ,Ttr_PayCycle Tta_PayCycle
                                        ,Ttr_PayCycle Tta_OrigPayCycle
		                                ,Mvh_AllowanceCode Tta_IncomeCode
		                                ,0 Tta_PostFlag
	                                    ,SUM(Ttr_TBAmt{0}) Tta_IncomeAmt
		                                ,'C' Tta_CycleIndicator
		                                ,'{2}'
		                                ,GETDATE()		
	                                FROM T_EmpTimeRegister
	                                JOIN {5}..M_VarianceAllowanceHdr
	                                  ON Mvh_TimeBaseID = '{0}'
                                        AND Mvh_CompanyCode = '{4}'
                                        AND Mvh_RecordStatus = 'A'
                                   WHERE Ttr_TBAmt{0} > 0
                                     AND Ttr_PayCycle = '{3}'
                                     {1}
                                GROUP BY Ttr_IDNo
		                                ,Ttr_PayCycle
		                                ,Mvh_AllowanceCode
                                        ,Mvh_CompanyCode
                                
                                UNION ALL";

                sqlInsert = string.Format(sqlInsert, i.ToString().PadLeft(2, '0'), strEmployeeLedgerCondition, UserCode, PayPeriod, CompanyCode, CentralProfile);

            }
            sqlInsert = sqlInsert.Remove(sqlInsert.Length - 9);
            dalHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
        }

        public DataTable GetAllowanceDetails(string strAllowanceColNo)
        {
            string query = string.Format(@"SELECT * FROM {0}..M_VarianceAllowanceHdr
                                          INNER JOIN {0}..M_VarianceAllowanceDtl 
										  ON Mvh_TimeBaseID = Mvd_TimeBaseID
										  AND Mvh_CompanyCode = Mvd_CompanyCode
                                          WHERE Mvh_BaseType = 'T'
                                            AND Mvh_TimeBaseID = '{1}'
											AND Mvh_CompanyCode = '{2}'
                                            AND Mvh_RecordStatus = 'A'
                                            AND Mvd_RecordStatus = 'A'
                                          ORDER BY Mvh_TimeBaseID"
                                        , CentralProfile
                                        , strAllowanceColNo
                                        , CompanyCode);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeesLoggedOutPastTimeQuota(bool ProcessAll, string EmployeeId, string EmployeeList, string DayCode, string AllowanceTimeStart, string AllowanceTimeEnd, bool CheckOT, bool IsCurrent)
        {
            string strOTCondition = "";
            if (CheckOT)
                strOTCondition = " AND Ttr_CompOTMin > 0 ";

            string EmpTimeRegisterTable = "T_EmpTimeRegister";
            if (!IsCurrent)
                EmpTimeRegisterTable = "T_EmpTimeRegisterHst";

            string strEmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                strEmployeeCondition = " AND Ttr_IDNo = '" + EmployeeId + "'";
            else if (ProcessAll == true && EmployeeList != "")
                strEmployeeCondition = " AND Ttr_IDNo IN (" + EmployeeList + ")";
            else
                strEmployeeCondition = "";

            string strTimeCondition = "";
            if (AllowanceTimeStart.Length == 4 && AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Ttr_ActIn_1 <= '{0}' AND Ttr_ActOut_2 >= '{1}' ", AllowanceTimeStart, AllowanceTimeEnd);
            else if (AllowanceTimeStart.Length == 4)
                strTimeCondition += string.Format(" AND Ttr_ActIn_1 <= '{0}' ", AllowanceTimeStart);
            else if (AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Ttr_ActOut_2 >= '{0}' ", AllowanceTimeEnd);

            string query = string.Format(@"SELECT Ttr_IDNo, Ttr_Date, Ttr_DayCode, Ttr_ActIn_1, Ttr_ActOut_2 FROM {5}
                                            WHERE Ttr_PayCycle = '{1}'
                                            AND Ttr_DayCode = '{2}' {3} {4} {0}", strTimeCondition, PayPeriod, DayCode, strOTCondition, strEmployeeCondition, EmpTimeRegisterTable);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllOvertimeRecords(bool ProcessAll, string EmployeeId, string EmployeeList, string AllowanceTimeStart, string AllowanceTimeEnd)
        {
            string strEmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                strEmployeeCondition = " AND Tot_IDNo = '" + EmployeeId + "'";
            else if (ProcessAll == true && EmployeeList != "")
                strEmployeeCondition = " AND Tot_IDNo IN (" + EmployeeList + ")";
            else
                strEmployeeCondition = "";

            string strTimeCondition = "";
            if (AllowanceTimeStart.Length == 4 && AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Tot_StartTime <= '{0}' AND Tot_EndTime >= '{1}' ", AllowanceTimeStart, AllowanceTimeEnd);
            else if (AllowanceTimeStart.Length == 4)
                strTimeCondition += string.Format(" AND Tot_StartTime <= '{0}' ", AllowanceTimeStart);
            else if (AllowanceTimeEnd.Length == 4)
                strTimeCondition += string.Format(" AND Tot_EndTime >= '{0}' ", AllowanceTimeEnd);

            #region query
            string query = string.Format(@"DECLARE @CurPeriod AS CHAR(7)
                                           SET @CurPeriod = (SELECT Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')

                                           SELECT DISTINCT Tot_IDNo, Tot_StartTime, Tot_EndTime, Tot_PayCycle, Tot_OvertimeDate, Tot_OvertimeType
                                           FROM T_EmpOvertime 
                                           WHERE Tot_PayCycle <= @CurPeriod
                                                 AND Tot_OvertimeStatus IN ('A','9','14') 
                                                 {0}
                                                 {1}
                                           UNION
                                           SELECT DISTINCT Tot_IDNo, Tot_StartTime, Tot_EndTime, Tot_PayCycle, Tot_OvertimeDate, Tot_OvertimeType
                                           FROM T_EmpOvertimeHst
                                           WHERE Tot_PayCycle <= @CurPeriod
                                                 AND Tot_OvertimeStatus IN ('A','9','14')
                                                 {0}
                                                 {1}
                                           ORDER BY Tot_IDNo, Tot_OvertimeDate, Tot_StartTime, Tot_EndTime"
                                                , strEmployeeCondition, strTimeCondition);
            #endregion
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        #endregion

        #region Pay Period
        public string GetNextCycle(int NextCount, string PayCycleCode, DALHelper dal)
        {
            DataTable dtPayPeriods;
            string sqlQuery = string.Format(@"SELECT Tps_PayCycle 
                                  FROM T_PaySchedule
                                 WHERE Tps_RecordStatus = 'A'
                                   AND Tps_CycleIndicator = 'F'
                                   AND Tps_StartCycle >= (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = '{0}'
						                                    AND Tps_RecordStatus = 'A'
						                                    AND Tps_CycleType = 'N')
                              ORDER BY Tps_PayCycle ASC", PayCycleCode);

            //using (DALHelper dal = new DALHelper())
            //{
                dtPayPeriods = dal.ExecuteDataSet(sqlQuery, CommandType.Text).Tables[0];
            //}

            if (dtPayPeriods.Rows.Count < NextCount)
            {
                CommonProcedures.ShowMessage(40041, "Next " + NextCount + " Pay Cycle", "");
                return "";
            }
            else
                return dtPayPeriods.Rows[NextCount-1][0].ToString().Trim();
        }


        public DataTable GetCycleRange(string PayPeriod)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@PayPeriod", PayPeriod);

            string strQuery = @"SELECT Tps_StartCycle, Tps_EndCycle FROM T_PaySchedule WHERE Tps_PayCycle = @PayPeriod";

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery, CommandType.Text, paramInfo).Tables[0];

            return dtResult;
        }

        #endregion

        public int UpdateOffCycleClosing(string SpecialPayCycle, string Usr_Login, string CompanyCode, string CentralProfile)
        {
            int Ret = 0;
            string DTRDBName = new CommonBL().GetDatabaseNameByProfileType(CentralProfile, CompanyCode, "D");
            #region query
            string query = string.Format(@"
                        UPDATE T_PaySchedule
                        SET Tps_CycleIndicatorSpecial = 'P'
                            , Tps_UpdatedBy = @Usr_Login
                            , Tps_UpdatedDate = GETDATE()
                        WHERE Tps_PayCycle = @Tps_PayCycle 
                            AND Tps_RecordStatus = 'A'

                        IF ('{0}' = 'TRUE') ---SALDELBANK
                        BEGIN
                            UPDATE M_PolicyDtl
                            SET Mpd_ParamValue = 0
                            ,Mpd_UpdatedBy = @Usr_Login
                            ,Mpd_UpdatedDate = GETDATE()
                            WHERE Mpd_PolicyCode = 'OFFATMGEN'

                            UPDATE T_SettingControl
                            SET Tsc_SetFlag = 0
                            ,Usr_Login = @Usr_Login
                            ,Ludatetime = GETDATE()
                            WHERE Tsc_SystemCode = 'PAYROLL'
                            AND Tsc_SettingCode = 'OFFATMGEN'
                        END 

                        UPDATE T_SettingControl
                        SET Tsc_SetFlag = 0
                             ,Usr_Login = @Usr_Login
                             ,Ludatetime = GETDATE()
                        WHERE Tsc_SystemCode = 'PAYROLL'
                             And Tsc_SettingCode In  ('OFFPAYCALC','OFFATMGEN','SPECIALPAY','LCSOFFCALC','LCSOFFPAY')

                        INSERT INTO T_SettingControlTrl
                        SELECT  Tsc_SystemCode
                              , Tsc_SettingCode
                              , Tsc_SettingName
                              , 0
                              , Tsc_AccessType
                              , @Tps_PayCycle
                              , NULL
                              , 'A'
                              , @Usr_Login
                              , GETDATE()
                          FROM T_SettingControl
                          WHERE Tsc_SystemCode = 'PAYROLL'
	                           AND Tsc_SettingCode = 'OFFPAYCALC'
                        
                        INSERT INTO T_SettingControlTrl
                        SELECT  Tsc_SystemCode
                              , Tsc_SettingCode
                              , Tsc_SettingName
                              , 0
                              , Tsc_AccessType
                              , @Tps_PayCycle
                              , NULL
                              , 'A'
                              , @Usr_Login
                              , GETDATE()
                          FROM T_SettingControl
                          WHERE Tsc_SystemCode = 'PAYROLL'
	                           AND Tsc_SettingCode = 'OFFATMGEN'

                        INSERT INTO T_SettingControlTrl
                        SELECT  Tsc_SystemCode
                              , Tsc_SettingCode
                              , Tsc_SettingName
                              , 0
                              , Tsc_AccessType
                              , @Tps_PayCycle
                              , NULL
                              , 'A'
                              , @Usr_Login
                              , GETDATE()
                          FROM T_SettingControl
                          WHERE Tsc_SystemCode = 'PAYROLL'
	                           AND Tsc_SettingCode = 'SPECIALPAY'

                        INSERT INTO T_SettingControlTrl
                        SELECT  Tsc_SystemCode
                              , Tsc_SettingCode
                              , Tsc_SettingName
                              , 0
                              , Tsc_AccessType
                              , @Tps_PayCycle
                              , NULL
                              , 'A'
                              , @Usr_Login
                              , GETDATE()
                          FROM T_SettingControl
                          WHERE Tsc_SystemCode = 'PAYROLL'
	                           AND Tsc_SettingCode = 'LCSOFFCALC'

                        INSERT INTO T_SettingControlTrl
                        SELECT  Tsc_SystemCode
                              , Tsc_SettingCode
                              , Tsc_SettingName
                              , 0
                              , Tsc_AccessType
                              , @Tps_PayCycle
                              , NULL
                              , 'A'
                              , @Usr_Login
                              , GETDATE()
                          FROM T_SettingControl
                          WHERE Tsc_SystemCode = 'PAYROLL'
	                           AND Tsc_SettingCode = 'LCSOFFPAY'

                         UPDATE T_SettingControl
                         SET Tsc_SetFlag    = 0
                             ,Usr_Login     = @Usr_Login
                             ,Ludatetime    = GETDATE()
                         WHERE Tsc_SystemCode = 'PAYROLL'
                             AND Tsc_SettingCode = 'OFFCYCOPEN'

                         INSERT INTO T_SettingControlTrl
                         SELECT  Tsc_SystemCode
                               , Tsc_SettingCode
                               , Tsc_SettingName
                               , 0
                               , Tsc_AccessType
                               , @Tps_PayCycle
                               , NULL
                               , 'A'
                               , @Usr_Login
                               , GETDATE()
                         FROM T_SettingControl
                         WHERE Tsc_SystemCode = 'PAYROLL'
                              AND Tsc_SettingCode = 'OFFCYCOPEN'

                        UPDATE T_SettingControl
                         SET Tsc_SetFlag    = 1
                             ,Usr_Login     = @Usr_Login
                             ,Ludatetime    = GETDATE()
                         WHERE Tsc_SystemCode = 'PAYROLL'
                             AND Tsc_SettingCode = 'OFFCYCCLOS'

                         INSERT INTO T_SettingControlTrl
                         SELECT  Tsc_SystemCode
                               , Tsc_SettingCode
                               , Tsc_SettingName
                               , 1
                               , Tsc_AccessType
                               , @Tps_PayCycle
                               , NULL
                               , 'A'
                               , @Usr_Login
                               , GETDATE()
                         FROM T_SettingControl
                         WHERE Tsc_SystemCode = 'PAYROLL'
                              AND Tsc_SettingCode = 'OFFCYCCLOS' ", (new CommonBL()).GetParameterValueFromPayroll("SALDELBANK", CompanyCode));

            string query2 = string.Format(@"
                                DECLARE @CYCLETYPE CHAR(1) = (SELECT Tps_CycleType FROM T_PaySchedule
								                              WHERE Tps_PayCycle = @Tps_PayCycle)

                                DECLARE @CurPayPeriodEndDate DATETIME = (SELECT Tps_EndCycle FROM T_PaySchedule 
                                                                        WHERE Tps_PayCycle = @Tps_PayCycle)

                                IF (@CYCLETYPE = 'B' OR @CYCLETYPE = 'M')  --B - Bonus | M - 13th Month Pay
                                BEGIN

	                                INSERT INTO T_EmpBonusDtlHst 
	                                SELECT T_EmpBonusDtl.* FROM T_EmpBonusDtl 
	                                INNER JOIN T_EmpBonusHdr 
	                                ON Tbh_ControlNo = Tbd_ControlNo
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

	                                INSERT INTO T_EmpBonusHdrHst
	                                SELECT * FROM T_EmpBonusHdr
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpBonusDtl
	                                FROM T_EmpBonusDtl
	                                INNER JOIN T_EmpBonusHdr 
	                                ON Tbh_ControlNo = Tbd_ControlNo
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpBonusHdr
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpProcessCheckHst
                                    SELECT *
                                    FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'BONUS'

                                    DELETE FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'BONUS'

                                END
                                ELSE IF (@CYCLETYPE = 'R') ---Leave Refund
                                BEGIN
                                    UPDATE {0}..T_EmpLeaveLdg 
                                    SET Tll_ConvertedCredit = Tld_LeaveHr
                                        , Tll_UpdatedBy = @Usr_Login
                                        , Tll_UpdatedDate = GETDATE()
                                    FROM {0}..T_EmpLeaveLdg 
                                    INNER JOIN T_EmpLeaveRefundHdr ON Tlh_PayCycle = @Tps_PayCycle
                                    INNER JOIN T_EmpLeaveRefundDtl 
                                        ON Tld_ControlNo = Tlh_ControlNo
                                        AND Tll_LeaveCode = Tld_LeaveCode
                                        AND Tll_IDNo = Tld_IDNo
                                    INNER JOIN T_EmpLeaveRefundRule 
                                        ON Tld_ControlNo = Tlr_ControlNo
                                        AND Tll_LeaveYear = Tlr_LeaveYear 
                                        AND Tll_LeaveCode = Tlr_LeaveCode    

                                    INSERT INTO {0}..T_EmpLeaveLdgTrl (Tll_LeaveYear
                                                                      ,Tll_IDNo
                                                                      ,Tll_LeaveCode
                                                                      ,Tll_ProcessedDate
                                                                      ,Tll_Hours
                                                                      ,Tll_Remarks
                                                                      ,Tll_LeaveMonth
                                                                      ,Tll_Action
                                                                      ,Tll_SystemGenerated
                                                                      ,Usr_Login
                                                                      ,Ludatetime)
									SELECT Tll_LeaveYear,Tll_IDNo,Tll_LeaveCode,GETDATE(),Tld_LeaveHr,'CONVERTED CREDITS',0,'CA',1,@Usr_Login,GETDATE()
									FROM {0}..T_EmpLeaveLdg
									INNER JOIN T_EmpLeaveRefundHdr ON Tlh_PayCycle = @Tps_PayCycle
                                    INNER JOIN T_EmpLeaveRefundDtl 
                                        ON Tld_ControlNo = Tlh_ControlNo
                                        AND Tll_LeaveCode = Tld_LeaveCode
                                        AND Tll_IDNo = Tld_IDNo
                                    INNER JOIN T_EmpLeaveRefundRule 
                                        ON Tld_ControlNo = Tlr_ControlNo
                                        AND Tll_LeaveYear = Tlr_LeaveYear 
                                        AND Tll_LeaveCode = Tlr_LeaveCode     

                                    UPDATE T_LeaveSchedule 
                                    SET Tls_LeaveRefundFlag = 1
                                        , Tls_LeaveRefundBy = @Usr_Login
                                        , Tls_LeaveRefundDate = GETDATE()
                                    FROM T_LeaveSchedule 
                                    INNER JOIN T_EmpLeaveRefundRule 
                                         ON Tls_LeaveYear = Tlr_LeaveYear 
                                         AND Tls_LeaveCode = Tlr_LeaveCode
                                    INNER JOIN T_EmpLeaveRefundHdr ON Tlr_ControlNo = Tlh_ControlNo
                                        AND Tlh_PayCycle = @Tps_PayCycle                        

	                                INSERT INTO T_EmpLeaveRefundDtlHst
	                                SELECT T_EmpLeaveRefundDtl.* FROM T_EmpLeaveRefundDtl 
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tld_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

	                                INSERT INTO T_EmpLeaveRefundRuleHst
	                                SELECT T_EmpLeaveRefundRule.* FROM T_EmpLeaveRefundRule 
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tlr_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpLeaveRefundHdrHst
	                                SELECT * FROM T_EmpLeaveRefundHdr 
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpLeaveRefundDtl
	                                FROM T_EmpLeaveRefundDtl
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tld_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpLeaveRefundRule
	                                FROM T_EmpLeaveRefundRule
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tlr_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpLeaveRefundHdr
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpProcessCheckHst
                                    SELECT *
                                    FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'LEAVEREFUND'

                                    DELETE FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'LEAVEREFUND'
                                END

                                ELSE IF (@CYCLETYPE = 'A') ---Perfect Attendance
                                BEGIN

	                                INSERT INTO T_EmpAttendanceDtlHst
	                                SELECT T_EmpAttendanceDtl.* FROM T_EmpAttendanceDtl
	                                INNER JOIN T_EmpAttendanceHdr 
	                                ON Tah_ControlNo = Tad_ControlNo
	                                WHERE Tah_PayCycle = @Tps_PayCycle

	                                INSERT INTO T_EmpAttendanceRuleHst
	                                SELECT T_EmpAttendanceRule.* FROM T_EmpAttendanceRule 
	                                INNER JOIN T_EmpAttendanceHdr 
	                                ON Tah_ControlNo = Tar_ControlNo
	                                WHERE Tah_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpAttendanceExceptionHst
	                                SELECT T_EmpAttendanceException.* FROM T_EmpAttendanceException 
	                                INNER JOIN T_EmpAttendanceHdr 
	                                ON Tah_ControlNo = Tap_ControlNo
	                                WHERE Tah_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpAttendanceHdrHst
	                                SELECT * FROM T_EmpAttendanceHdr
	                                WHERE Tah_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpAttendanceDtl
	                                FROM T_EmpAttendanceDtl
	                                INNER JOIN T_EmpAttendanceHdr 
	                                ON Tah_ControlNo = Tad_ControlNo
	                                WHERE Tah_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpAttendanceRule
	                                FROM T_EmpAttendanceRule
	                                INNER JOIN T_EmpAttendanceHdr 
	                                ON Tah_ControlNo = Tar_ControlNo
	                                WHERE Tah_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpAttendanceException
	                                FROM T_EmpAttendanceException
	                                INNER JOIN T_EmpAttendanceHdr 
	                                ON Tah_ControlNo = Tap_ControlNo
	                                WHERE Tah_PayCycle = @Tps_PayCycle

                                    IF (SELECT Tah_WithTimer FROM T_EmpAttendanceHdr WHERE Tah_PayCycle = @Tps_PayCycle) = 1
                                    BEGIN

                                    INSERT INTO {0}..T_EmpAttendanceLdgtrl
                                    SELECT A.* FROM {0}..T_EmpAttendanceLdg  A
                                    INNER JOIN T_EmpAttendanceDtl B
		                                  ON A.Tal_IDNo = B.Tad_IDNo
                                    INNER JOIN T_EmpAttendanceHdr C 
                                          ON C.Tah_ControlNo = B.Tad_ControlNo
                                          AND A.Tal_RuleCode = C.Tah_RuleCode
			                              AND C.Tah_PayCycle = @Tps_PayCycle

                                    UPDATE {0}..T_EmpAttendanceLdg 
                                    SET Tal_PrevTimer = Tal_Timer
                                    , Tal_UpdatedBy = @Usr_Login
                                    , Tal_UpdatedDate = GETDATE()
                                    FROM {0}..T_EmpAttendanceLdg A
                                    INNER JOIN T_EmpAttendanceDtl B
		                                 ON A.Tal_IDNo = B.Tad_IDNo
                                    INNER JOIN T_EmpAttendanceHdr C 
                                         ON C.Tah_ControlNo = B.Tad_ControlNo
										 AND A.Tal_RuleCode = C.Tah_RuleCode
										 AND C.Tah_PayCycle = @Tps_PayCycle
                                    END

                                    UPDATE T_LastTransaction 
                                    SET Tlt_LastEffectivity = Tah_Effectivity
                                         , Tlt_UpdatedBy = @Usr_Login
                                         , Tlt_UpdatedDate = GETDATE()
                                    FROM T_LastTransaction 
									INNER JOIN T_EmpAttendanceHdr 
										ON Tah_RuleCode = Tlt_TransactionCode
									    AND Tah_PayCycle = @Tps_PayCycle
                                    
                                    DELETE FROM T_EmpAttendanceHdr
	                                WHERE Tah_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpProcessCheckHst
                                    SELECT *
                                    FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'PERFECTATTENDANCE'

                                    DELETE FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'PERFECTATTENDANCE'
                                END

                                ELSE IF (@CYCLETYPE = 'J') ---Retropay Adjustment
                                BEGIN
                                    INSERT INTO T_EmpPayrollDtlRetroHst
                                    SELECT * FROM T_EmpPayrollDtlRetro
	                                WHERE Tpd_RetroPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollDtlRetro
	                                WHERE Tpd_RetroPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollDtlMiscRetroHst
                                    SELECT * FROM T_EmpPayrollDtlMiscRetro
	                                WHERE Tpm_RetroPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollDtlMiscRetro
	                                WHERE Tpm_RetroPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollRetroHst
                                    SELECT * FROM T_EmpPayrollRetro
	                                WHERE Tpy_RetroPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollRetro
	                                WHERE Tpy_RetroPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollMiscRetroHst
                                    SELECT * FROM T_EmpPayrollMiscRetro
	                                WHERE Tpm_RetroPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollMiscRetro
	                                WHERE Tpm_RetroPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpSystemAdj2RetroHst
                                    SELECT * FROM T_EmpSystemAdj2Retro
	                                WHERE Tsa_RetroPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpSystemAdj2Retro
	                                WHERE Tsa_RetroPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpSystemAdjMisc2RetroHst
                                    SELECT * FROM T_EmpSystemAdjMisc2Retro
	                                WHERE Tsm_RetroPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpSystemAdjMisc2Retro
	                                WHERE Tsm_RetroPayCycle = @Tps_PayCycle
                                        
                                    INSERT INTO T_EmpRetroPayHst
	                                SELECT * FROM T_EmpRetroPay
	                                WHERE Ter_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpRetroPay
	                                WHERE Ter_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpProcessCheckHst
                                    SELECT *
                                    FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'RETROADJ'

                                    DELETE FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'RETROADJ'
                                END
                                ELSE IF (@CYCLETYPE = 'L') ---Final Pay
                                BEGIN   

                                    UPDATE [{0}].dbo.T_EmpApprovalRoute
                                    SET Tar_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
                                        ,Tar_UpdatedBy = @Usr_Login
                                        ,Tar_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpApprovalRoute 
                                    INNER JOIN T_EmpFinalPay ON Tar_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tar_EndDate IS NULL
                                        AND Tar_CompanyCode = @CompanyCode
                                    
                                    UPDATE [{0}].dbo.T_EmpCompanyProfilePayGrp
					                SET Tec_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
						                ,Tec_UpdatedBy = @Usr_Login
						                ,Tec_UpdatedDate = GETDATE()
					                FROM [{0}].dbo.T_EmpCompanyProfilePayGrp 
					                INNER JOIN T_EmpFinalPay ON Tec_IDNo = Tef_IDNo
						                AND Tef_PayCycle = @Tps_PayCycle
					                WHERE Tec_EndDate IS NULL
                                        AND Tec_CompanyCode = @CompanyCode

                                    UPDATE [{0}].dbo.T_EmpCostcenter
					                SET Tcc_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
						                ,Tcc_UpdatedBy = @Usr_Login
						                ,Tcc_UpdatedDate = GETDATE()
					                FROM [{0}].dbo.T_EmpCostcenter 
					                INNER JOIN T_EmpFinalPay ON Tcc_IDNo = Tef_IDNo
						                AND Tef_PayCycle = @Tps_PayCycle
					                WHERE Tcc_EndDate IS NULL

                                    UPDATE [{0}].dbo.T_EmpPosition
                                    SET Tpo_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
                                        ,Tpo_UpdatedBy = @Usr_Login
                                        ,Tpo_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpPosition 
					                INNER JOIN T_EmpFinalPay ON Tpo_IDNo = Tef_IDNo
						                AND Tef_PayCycle = @Tps_PayCycle
					                WHERE Tpo_EndDate IS NULL

                                    UPDATE [{0}].dbo.T_EmpSalary
                                    SET Tsl_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
                                        ,Tsl_UpdatedBy = @Usr_Login
                                        ,Tsl_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpSalary 
					                INNER JOIN T_EmpFinalPay ON Tsl_IDNo = Tef_IDNo
						                AND Tef_PayCycle = @Tps_PayCycle
					                WHERE Tsl_EndDate IS NULL

                                    UPDATE [{0}].dbo.T_UserApprovalDelegation
                                    SET Tad_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
                                        ,Tad_UpdatedBy = @Usr_Login
                                        ,Tad_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_UserApprovalDelegation 
                                    INNER JOIN T_EmpFinalPay ON Tad_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tad_EndDate IS NULL

                                    UPDATE [{0}].dbo.T_EmpAssignmentDate
                                    SET Tac_EndDate = DATEADD(dd, -1, Tef_SeparationDate)
                                        ,Tac_UpdatedBy = @Usr_Login
                                        ,Tac_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpAssignmentDate 
                                    INNER JOIN T_EmpFinalPay ON Tac_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tac_EndDate IS NULL
                                        
                                    UPDATE [{0}].dbo.T_EmpAssignmentCycle
                                    SET Tac_EndCycle = @Tps_PayCycle
                                        ,Tac_UpdatedBy = @Usr_Login
                                        ,Tac_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpAssignmentCycle 
                                    INNER JOIN T_EmpFinalPay ON Tac_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tac_EndCycle IS NULL

                                    UPDATE [{0}].dbo.T_EmpFixAllowance
                                    SET Tfa_EndPayCycle = Tef_PayCycleSeparation
                                        ,Tfa_UpdatedBy = @Usr_Login
                                        ,Tfa_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpFixAllowance 
                                    INNER JOIN T_EmpFinalPay ON Tfa_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tfa_EndPayCycle IS NULL

                                    UPDATE [{0}].dbo.T_EmpProration
                                    SET Tep_EndCycle = @Tps_PayCycle
                                        ,Tep_UpdatedBy = @Usr_Login
                                        ,Tep_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.T_EmpProration 
                                    INNER JOIN T_EmpFinalPay ON Tep_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tep_EndCycle IS NULL

                                    UPDATE [{0}].dbo.T_EmpSpecialProration
                                    SET Tes_EndCycle = @Tps_PayCycle
                                        ,Usr_Login = @Usr_Login
                                        ,Ludatetime = GETDATE()
                                    FROM [{0}].dbo.T_EmpSpecialProration 
                                    INNER JOIN T_EmpFinalPay ON Tes_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Tes_EndCycle IS NULL

                                    UPDATE [{0}].dbo.M_Employee
                                    SET Mem_ChangeStatus = GETDATE()
                                        ,Mem_UpdatedBy = @Usr_Login
                                        ,Mem_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_Employee   
                                    INNER JOIN T_EmpFinalPay ON Mem_IDNo = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle

                                    -- Set Record Status to C --
                                    UPDATE [{0}].dbo.M_User
                                    SET Mur_RecordStatus = 'C'
                                        ,Mur_UpdatedBy = @Usr_Login
                                        ,Mur_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_User   
                                    INNER JOIN T_EmpFinalPay ON Mur_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                   
                                    UPDATE [{0}].dbo.M_UserDtl
                                    SET Mud_RecordStatus = 'C'
                                        ,Mud_UpdatedBy = @Usr_Login
                                        ,Mud_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_UserDtl 
                                    INNER JOIN T_EmpFinalPay ON Mud_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Mud_CompanyCode = @CompanyCode

                                    UPDATE [{0}].dbo.M_UserExt
                                    SET Mue_RecordStatus = 'C'
                                        ,Mue_UpdatedBy = @Usr_Login
                                        ,Mue_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_UserExt 
                                    INNER JOIN T_EmpFinalPay ON Mue_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Mue_CompanyCode = @CompanyCode

                                    UPDATE [{0}].dbo.M_UserProfile
                                    SET Mup_RecordStatus = 'C'
                                        ,Mup_UpdatedBy = @Usr_Login
                                        ,Mup_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_UserProfile 
                                    INNER JOIN T_EmpFinalPay ON Mup_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Mup_CompanyCode = @CompanyCode

                                    UPDATE [{0}].dbo.M_UserGroupQuery
                                    SET Muq_RecordStatus = 'C'
                                        ,Muq_UpdatedBy = @Usr_Login
                                        ,Muq_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_UserGroupQuery 
                                    INNER JOIN T_EmpFinalPay ON Muq_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Muq_CompanyCode = @CompanyCode

                                    UPDATE [{0}].dbo.M_UserGroupDocument
                                    SET Mud_RecordStatus = 'C'
                                        ,Mud_UpdatedBy = @Usr_Login
                                        ,Mud_UpdatedDate = GETDATE()
                                    FROM [{0}].dbo.M_UserGroupDocument
                                    INNER JOIN T_EmpFinalPay ON Mud_UserCode = Tef_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    WHERE Mud_CompanyCode = @CompanyCode

                                    UPDATE [{1}].dbo.T_EmpLog
                                    SET Tel_RecordStatus = 'C'
                                       , Tel_UpdatedBy = @Usr_Login
                                       , Tel_UpdatedDate = GETDATE()
                                    FROM [{1}].dbo.T_EmpLog
                                    INNER JOIN T_EmpFinalPay ON Tef_IDNo = Tel_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle

                                    --Deduction
                                    UPDATE {0}..T_EmpDeductionHdr   --Deduction Ledger to History
                                    SET   Tdh_PaidAmount = Tdh_PaidAmount  + PaidAmt
	                                    , Tdh_DeferredAmount = Tdh_DeferredAmount - DeferredAmt
	                                    , Tdh_PaymentDate = CASE WHEN Tdh_PaidAmount  + PaidAmt = Tdh_DeductionAmount then @CurPayPeriodEndDate ELSE Tdh_PaymentDate END
	                                    , Usr_Login = @Usr_Login
	                                    , Ludatetime = GetDate()
                                    FROM {0}..T_EmpDeductionHdr
                                    INNER JOIN ( SELECT Tdd_IDNo  
				                                    , Tdd_DeductionCode 
				                                    , Tdd_StartDate  
				                                    , PaidAmt = Sum(Tdd_Amount) 
				                                    , DeferredAmt = Sum(Case When Tdd_DeferredFlag = 1  Then Tdd_Amount Else 0 End)
			                                    FROM T_EmpDeductionDtlFinalPay
			                                    WHERE Tdd_ThisPayCycle = @Tps_PayCycle
				                                    AND  Tdd_PaymentFlag = 1
			                                    GROUP BY Tdd_IDNo  
				                                    , Tdd_DeductionCode 
				                                    , Tdd_StartDate  ) PaidDed on PaidDed.Tdd_IDNo = T_EmpDeductionHdr.Tdh_IDNo
								                                    AND PaidDed.Tdd_DeductionCode = T_EmpDeductionHdr.Tdh_DeductionCode
								                                    AND PaidDed.Tdd_StartDate = T_EmpDeductionHdr.Tdh_StartDate
                                    ---Fully-Paid Deductions to Deduction Ledger History
                                    INSERT INTO {0}..T_EmpDeductionHdrHst 
                                    (Tdh_IDNo
                                    , Tdh_DeductionCode
                                    , Tdh_StartDate
                                    , Tdh_EndDate
                                    , Tdh_DeductionAmount
                                    , Tdh_PaidAmount
                                    , Tdh_CycleAmount
                                    , Tdh_DeferredAmount
                                    , Tdh_CheckDate
                                    , Tdh_VoucherNumber
                                    , Tdh_PaymentDate
                                    , Tdh_ExemptInPayroll
                                    , Tdh_PrincipalAmount
                                    , Tdh_ApplicablePayCycle
                                    , Tdh_AgreementNo
                                    , Tdh_DocumentNo
                                    , Tdh_EntryDate
                                    , Tdh_PaymentTerms
                                    , Usr_Login
                                    , Ludatetime)
                                    SELECT A.Tdh_IDNo
	                                    , A.Tdh_DeductionCode
	                                    , A.Tdh_StartDate
	                                    , A.Tdh_EndDate
	                                    , A.Tdh_DeductionAmount
	                                    , A.Tdh_PaidAmount
	                                    , A.Tdh_CycleAmount
	                                    , A.Tdh_DeferredAmount
	                                    , A.Tdh_CheckDate
	                                    , A.Tdh_VoucherNumber
	                                    , A.Tdh_PaymentDate
	                                    , A.Tdh_ExemptInPayroll
	                                    , A.Tdh_PrincipalAmount
	                                    , A.Tdh_ApplicablePayCycle
	                                    , A.Tdh_AgreementNo
	                                    , A.Tdh_DocumentNo
	                                    , A.Tdh_EntryDate
	                                    , A.Tdh_PaymentTerms
	                                    , A.Usr_Login
	                                    , A.Ludatetime
                                    FROM {0}..T_EmpDeductionHdr A
                                    INNER JOIN T_EmpFinalPay ON Tef_IDNo = A.Tdh_IDNo
	                                    AND Tef_PayCycle = @Tps_PayCycle
                                    LEFT JOIN {0}..T_EmpDeductionHdrHst B ON A.Tdh_IDNo = B.Tdh_IDNo
	                                    AND A.Tdh_DeductionCode = B.Tdh_DeductionCode
	                                    AND A.Tdh_StartDate = B.Tdh_StartDate
                                    WHERE B.Ludatetime IS NULL
	                                    AND A.Tdh_DeductionAmount = A.Tdh_PaidAmount
	                                    AND A.Tdh_PaymentDate IS NOT NULL 

                                    ---Move Summary Cycle Payment records to History
                                    INSERT INTO T_EmpDeductionHdrCycleFinalPayHst 
                                    (Tdh_PayCycle
                                    , Tdh_IDNo
                                    , Tdh_DeductionCode
                                    , Tdh_StartDate
                                    , Tdh_EndDate
                                    , Tdh_DeductionAmount
                                    , Tdh_AccumulatedPaidAmount
                                    , Tdh_CycleAmount
                                    , Tdh_PaidAmountThisCycle
                                    , Usr_Login
                                    , Ludatetime)
                                    SELECT A.Tdh_PayCycle
	                                    , A.Tdh_IDNo
	                                    , A.Tdh_DeductionCode
	                                    , A.Tdh_StartDate
	                                    , A.Tdh_EndDate
	                                    , A.Tdh_DeductionAmount
	                                    , A.Tdh_AccumulatedPaidAmount
	                                    , A.Tdh_CycleAmount
	                                    , A.Tdh_PaidAmountThisCycle
	                                    , A.Usr_Login
	                                    , A.Ludatetime
                                    FROM T_EmpDeductionHdrCycleFinalPay A
                                    LEFT JOIN T_EmpDeductionHdrCycleFinalPayHst B ON A.Tdh_IDNo = B.Tdh_IDNo
	                                    AND A.Tdh_PayCycle = B.Tdh_PayCycle
	                                    AND A.Tdh_DeductionCode = B.Tdh_DeductionCode
	                                    AND A.Tdh_StartDate = B.Tdh_StartDate
                                    WHERE A.Tdh_PayCycle = @Tps_PayCycle
	                                    AND B.Tdh_PayCycle IS NULL

                                    --Initialize Summary Cycle Payment records
                                    DELETE FROM T_EmpDeductionHdrCycleFinalPay WHERE Tdh_PayCycle = @Tps_PayCycle
                                
                                    --Fully-Paid Deductions to Employees' Deduction Detail Fully-Paid History
                                    INSERT INTO T_EmpDeductionDtlFullPayHst 
                                    (Tdd_IDNo
                                    , Tdd_DeductionCode
                                    , Tdd_StartDate
                                    , Tdd_ThisPayCycle
                                    , Tdd_PayCycle
                                    , Tdd_LineNo
                                    , Tdd_PaymentType
                                    , Tdd_Amount
                                    , Tdd_DeferredFlag
                                    , Tdd_PaymentFlag
                                    , Tdd_Remarks
                                    , Usr_Login
                                    , Ludatetime)
                                    SELECT A.Tdd_IDNo
	                                    , A.Tdd_DeductionCode
	                                    , A.Tdd_StartDate
	                                    , A.Tdd_ThisPayCycle
	                                    , A.Tdd_PayCycle
	                                    , A.Tdd_LineNo
	                                    , A.Tdd_PaymentType
	                                    , A.Tdd_Amount
	                                    , A.Tdd_DeferredFlag
	                                    , A.Tdd_PaymentFlag
	                                    , A.Tdd_Remarks
	                                    , A.Usr_Login
	                                    , A.Ludatetime
                                    FROM T_EmpDeductionDtlFinalPay A
                                    INNER JOIN {0}..T_EmpDeductionHdr on Tdh_IDNo = A.Tdd_IDNo
	                                    AND Tdh_DeductionCode = A.Tdd_DeductionCode
	                                    AND Tdh_StartDate = A.Tdd_StartDate
	                                    AND Tdh_PaymentDate IS NOT NULL
	                                    AND Tdh_DeductionAmount = Tdh_PaidAmount
                                    LEFT JOIN T_EmpDeductionDtlFullPayHst B ON A.Tdd_IDNo = B.Tdd_IDNo
	                                    AND A.Tdd_DeductionCode = B.Tdd_DeductionCode
	                                    AND A.Tdd_StartDate = B.Tdd_StartDate
	                                    AND A.Tdd_ThisPayCycle = B.Tdd_ThisPayCycle
	                                    AND A.Tdd_PayCycle = B.Tdd_PayCycle
	                                    AND A.Tdd_LineNo = B.Tdd_LineNo
                                    WHERE B.Ludatetime IS NULL

                                    --Delete Deferred Deduction Records Already Paid
                                    DELETE FROM T_EmpDeductionDefer
                                    FROM T_EmpDeductionDefer
                                    INNER JOIN T_EmpDeductionDtlFinalPay on T_EmpDeductionDtlFinalPay.Tdd_IDNo =   T_EmpDeductionDefer.Tdd_IDNo 
	                                    AND T_EmpDeductionDtlFinalPay.Tdd_DeductionCode = T_EmpDeductionDefer.Tdd_DeductionCode
	                                    AND T_EmpDeductionDtlFinalPay.Tdd_StartDate  = T_EmpDeductionDefer.Tdd_StartDate
	                                    AND T_EmpDeductionDtlFinalPay.Tdd_PayCycle = T_EmpDeductionDefer.Tdd_PayCycle
	                                    AND T_EmpDeductionDtlFinalPay.Tdd_LineNo = T_EmpDeductionDefer.Tdd_LineNo
	                                    AND Tdd_PaymentFlag = 1
	                                    AND Tdd_ThisPayCycle = @Tps_PayCycle 

                                    --Deduction Details Updating
                                    DELETE FROM T_EmpDeductionDtlFinalPay

                                    --Delete Fully-Paid Deduction Detail Records
                                    DELETE FROM T_EmpDeductionDtlFinalPay
                                    FROM T_EmpDeductionDtlFinalPay
                                    INNER JOIN {0}..T_EmpDeductionHdr on Tdh_IDNo = Tdd_IDNo
                                         AND Tdh_DeductionCode = Tdd_DeductionCode
                                         AND Tdh_StartDate = Tdd_StartDate
                                         AND Tdh_PaymentDate IS NOT NULL
                                         AND Tdh_DeductionAmount = Tdh_PaidAmount
                                    WHERE Tdd_ThisPayCycle = @Tps_PayCycle
                                                  
                                     DELETE FROM {0}..T_EmpDeductionHdr
                                     FROM {0}..T_EmpDeductionHdr
                                     INNER JOIN T_EmpFinalPay ON Tef_IDNo = Tdh_IDNo
	                                     AND Tef_PayCycle = @Tps_PayCycle
                                     WHERE Tdh_PaymentDate IS NOT NULL
	                                     AND Tdh_DeductionAmount = Tdh_PaidAmount

                                    --Income
                                    INSERT INTO T_EmpIncomeFinalPayHst
                                    SELECT *
                                    FROM T_EmpIncomeFinalPay
                                    WHERE Tin_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpIncomeFinalPay
                                    WHERE Tin_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpTimeBaseAllowanceCycleFinalPayHst
                                    SELECT *
                                    FROM T_EmpTimeBaseAllowanceCycleFinalPay
                                    WHERE Tta_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpTimeBaseAllowanceCycleFinalPay
                                    WHERE Tta_PayCycle = @Tps_PayCycle

                                    --Manual Adjustment
                                    INSERT INTO T_EmpManualAdj2FinalPayHst
                                    SELECT *
                                    FROM T_EmpManualAdj2FinalPay
                                    WHERE Tma_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpManualAdj2FinalPay
                                    WHERE Tma_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpManualAdjFinalPayHst
                                    SELECT *
                                    FROM T_EmpManualAdjFinalPay
                                    WHERE Tma_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpManualAdjFinalPay
                                    WHERE Tma_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpManualAdjMisc2FinalPayHst
                                    SELECT *
                                    FROM T_EmpManualAdjMisc2FinalPay
                                    WHERE Tmm_PayCycle = @Tps_PayCycle
       
                                    DELETE FROM T_EmpManualAdjMisc2FinalPay
                                    WHERE Tmm_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpManualAdjMiscFinalPayHst
                                    SELECT *
                                    FROM T_EmpManualAdjMiscFinalPay
                                    WHERE Tmm_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpManualAdjMiscFinalPay
                                    WHERE Tmm_PayCycle = @Tps_PayCycle

                                    --System Adjustment
                                    INSERT INTO T_EmpSystemAdj2FinalPayHst
                                    SELECT *
                                    FROM T_EmpSystemAdj2FinalPay
                                    WHERE Tsa_AdjPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpSystemAdj2FinalPay
                                    WHERE Tsa_AdjPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpSystemAdjFinalPayHst
                                    SELECT *
                                    FROM T_EmpSystemAdjFinalPay
                                    WHERE Tsa_AdjPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpSystemAdjFinalPay
                                    WHERE Tsa_AdjPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpSystemAdjMisc2FinalPayHst
                                    SELECT *
                                    FROM T_EmpSystemAdjMisc2FinalPay
                                    WHERE Tsm_OrigAdjPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpSystemAdjMisc2FinalPay
                                    WHERE Tsm_OrigAdjPayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpSystemAdjMiscFinalPayHst
                                    SELECT *
                                    FROM T_EmpSystemAdjMiscFinalPay
                                    WHERE Tsm_AdjPayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpSystemAdjMiscFinalPay
                                    WHERE Tsm_AdjPayCycle = @Tps_PayCycle

                                    --Payroll Transaction
                                    INSERT INTO T_EmpPayTranDtlFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayTranDtlFinalPay
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayTranDtlFinalPay
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayTranDtlMiscFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayTranDtlMiscFinalPay
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayTranDtlMiscFinalPay
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayTranHdrMiscFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayTranHdrMiscFinalPay
                                    WHERE Tph_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayTranHdrMiscFinalPay
                                    WHERE Tph_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayTranHdrFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayTranHdrFinalPay
                                    WHERE Tph_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayTranHdrFinalPay
                                    WHERE Tph_PayCycle = @Tps_PayCycle
                    
                                    --Payroll 
                                    INSERT INTO T_EmpPayroll2FinalPayHst
                                    SELECT *
                                    FROM T_EmpPayroll2FinalPay
                                    WHERE Tpy_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayroll2FinalPay
                                    WHERE Tpy_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollDtlFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayrollDtlFinalPay
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollDtlFinalPay
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollDtlMiscFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayrollDtlMiscFinalPay
                                    WHERE Tpm_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollDtlMiscFinalPay
                                    WHERE Tpm_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayrollFinalPay
                                    WHERE Tpy_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollFinalPay
                                    WHERE Tpy_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollMiscFinalPayHst
                                    SELECT *
                                    FROM T_EmpPayrollMiscFinalPay
                                    WHERE Tpm_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollMiscFinalPay
                                    WHERE Tpm_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpFinalPayHst
                                    SELECT *
                                    FROM T_EmpFinalPay
                                    WHERE Tef_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpFinalPay
                                    WHERE Tef_PayCycle = @Tps_PayCycle

                                    --Leave Refund
	                                INSERT INTO T_EmpLeaveRefundDtlHst
	                                SELECT T_EmpLeaveRefundDtl.* FROM T_EmpLeaveRefundDtl 
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tld_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

	                                INSERT INTO T_EmpLeaveRefundRuleHst
	                                SELECT T_EmpLeaveRefundRule.* FROM T_EmpLeaveRefundRule 
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tlr_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpLeaveRefundHdrHst
	                                SELECT * FROM T_EmpLeaveRefundHdr 
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    UPDATE {0}..T_EmpLeaveLdg 
                                    SET Tll_ConvertedCredit = Tld_LeaveHr
                                        , Tll_UpdatedBy = @Usr_Login
                                        , Tll_UpdatedDate = GETDATE()
                                    FROM {0}..T_EmpLeaveLdg 
                                    INNER JOIN T_EmpLeaveRefundHdr ON Tlh_PayCycle = @Tps_PayCycle
                                    INNER JOIN T_EmpLeaveRefundDtl 
                                        ON Tld_ControlNo = Tlh_ControlNo
                                        AND Tll_LeaveCode = Tld_LeaveCode
                                        AND Tll_IDNo = Tld_IDNo
                                    INNER JOIN T_EmpLeaveRefundRule 
                                        ON Tld_ControlNo = Tlr_ControlNo
                                        AND Tll_LeaveYear = Tlr_LeaveYear 
                                        AND Tll_LeaveCode = Tlr_LeaveCode

                                    UPDATE {0}..T_EmpLeaveLdgHst 
                                    SET Tll_ConvertedCredit = Tld_LeaveHr
                                        , Tll_UpdatedBy = @Usr_Login
                                        , Tll_UpdatedDate = GETDATE()
                                    FROM {0}..T_EmpLeaveLdgHst 
                                    INNER JOIN T_EmpLeaveRefundHdr ON Tlh_PayCycle = @Tps_PayCycle
                                    INNER JOIN T_EmpLeaveRefundDtl 
                                        ON Tld_ControlNo = Tlh_ControlNo
                                        AND Tll_LeaveCode = Tld_LeaveCode
                                        AND Tll_IDNo = Tld_IDNo
                                    INNER JOIN T_EmpLeaveRefundRule 
                                        ON Tld_ControlNo = Tlr_ControlNo
                                        AND Tll_LeaveYear = Tlr_LeaveYear 
                                        AND Tll_LeaveCode = Tlr_LeaveCode    

                                    INSERT INTO {0}..T_EmpLeaveLdgHst 
									SELECT T_EmpLeaveLdg.*
									FROM {0}..T_EmpLeaveLdg 
									INNER JOIN T_EmpLeaveRefundHdr ON Tlh_PayCycle = @Tps_PayCycle
									INNER JOIN T_EmpLeaveRefundDtl 
										ON Tld_ControlNo = Tlh_ControlNo
										AND Tll_LeaveCode = Tld_LeaveCode
										AND Tll_IDNo = Tld_IDNo
									INNER JOIN T_EmpLeaveRefundRule 
										ON Tld_ControlNo = Tlr_ControlNo
										AND Tll_LeaveYear = Tlr_LeaveYear 
										AND Tll_LeaveCode = Tlr_LeaveCode

									DELETE FROM T_EmpLeaveLdg
									FROM {0}..T_EmpLeaveLdg 
									INNER JOIN T_EmpLeaveRefundHdr ON Tlh_PayCycle = @Tps_PayCycle
									INNER JOIN T_EmpLeaveRefundDtl 
										ON Tld_ControlNo = Tlh_ControlNo
										AND Tll_LeaveCode = Tld_LeaveCode
										AND Tll_IDNo = Tld_IDNo
									INNER JOIN T_EmpLeaveRefundRule 
										ON Tld_ControlNo = Tlr_ControlNo
										AND Tll_LeaveYear = Tlr_LeaveYear 
										AND Tll_LeaveCode = Tlr_LeaveCode 

	                                DELETE FROM T_EmpLeaveRefundDtl
	                                FROM T_EmpLeaveRefundDtl
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tld_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpLeaveRefundRule
	                                FROM T_EmpLeaveRefundRule
	                                INNER JOIN T_EmpLeaveRefundHdr 
	                                ON Tlh_ControlNo = Tlr_ControlNo
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpLeaveRefundHdr
	                                WHERE Tlh_PayCycle = @Tps_PayCycle

                                    --Bonus
	                                INSERT INTO T_EmpBonusDtlHst 
	                                SELECT T_EmpBonusDtl.* FROM T_EmpBonusDtl 
	                                INNER JOIN T_EmpBonusHdr 
	                                ON Tbh_ControlNo = Tbd_ControlNo
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

	                                INSERT INTO T_EmpBonusHdrHst
	                                SELECT * FROM T_EmpBonusHdr
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpBonusDtl
	                                FROM T_EmpBonusDtl
	                                INNER JOIN T_EmpBonusHdr 
	                                ON Tbh_ControlNo = Tbd_ControlNo
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpBonusHdr
	                                WHERE Tbh_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpProcessCheckHst
                                    SELECT *
                                    FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'FINALPAY'

                                    DELETE FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'FINALPAY'

                                END
                                    INSERT INTO T_EmpLaborCheckHst
                                    SELECT *
                                    FROM T_EmpLaborCheck
                                    WHERE Tlc_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpLaborCheck
                                    WHERE Tlc_PayCycle = @Tps_PayCycle
                                    
                                    INSERT INTO T_EmpIncomeHst
                                    SELECT * FROM T_EmpIncome
	                                WHERE Tin_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpIncome
	                                WHERE Tin_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollDtlYearly
                                    SELECT * FROM T_EmpPayrollDtl
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollDtl
                                    WHERE Tpd_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollDtlMiscYearly
                                    SELECT * FROM T_EmpPayrollDtlMisc
                                    WHERE Tpm_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayrollDtlMisc
                                    WHERE Tpm_PayCycle = @Tps_PayCycle
                                   
                                    INSERT INTO T_EmpPayrollYearly
	                                SELECT * FROM T_EmpPayroll
	                                WHERE Tpy_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpPayroll
	                                WHERE Tpy_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayroll2Yearly
	                                SELECT * FROM T_EmpPayroll2
	                                WHERE Tpy_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpPayroll2
	                                WHERE Tpy_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpPayrollMiscYearly
	                                SELECT * FROM T_EmpPayrollMisc
	                                WHERE Tpm_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_EmpPayrollMisc
	                                WHERE Tpm_PayCycle = @Tps_PayCycle

                                    ---Labor Cost
                                    INSERT INTO T_EmpCostHst
                                    SELECT * FROM T_EmpCost
                                    WHERE Tec_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpCost
                                    WHERE Tec_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpCostHoursHst
                                    SELECT * FROM T_EmpCostHours
                                    WHERE Teh_PayCycle = @Tps_PayCycle

                                    DELETE FROM T_EmpCostHours
                                    WHERE Teh_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_CostDtlHst 
	                                SELECT T_CostDtl.* FROM T_CostDtl 
	                                INNER JOIN T_CostHdr 
	                                ON Tch_ControlNo = Tcd_ControlNo
	                                WHERE Tch_PayCycle = @Tps_PayCycle

	                                INSERT INTO T_CostHdrHst
	                                SELECT * FROM T_CostHdr
	                                WHERE Tch_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_CostDtl
	                                FROM T_CostDtl
	                                INNER JOIN T_CostHdr 
	                                ON Tch_ControlNo = Tcd_ControlNo
	                                WHERE Tch_PayCycle = @Tps_PayCycle

	                                DELETE FROM T_CostHdr
	                                WHERE Tch_PayCycle = @Tps_PayCycle

                                    INSERT INTO T_EmpProcessCheckHst
                                    SELECT *
                                    FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'LABORCOST'

                                    DELETE FROM T_EmpProcessCheck
                                    WHERE Tpc_PayCycle = @Tps_PayCycle
                                    AND Tpc_Modulecode = 'LABORCOST'

                                ", CentralProfile, DTRDBName);

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", SpecialPayCycle);
            paramInfo[1] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);
            

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    Ret = dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
                    Ret = dal.ExecuteNonQuery(query2, CommandType.Text, paramInfo);
                    dal.CommitTransaction();
                }
                catch (Exception ex)
                {
                    Ret = 0;
                    dal.RollBackTransaction();
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return Ret;
        }

        public int CloseYearMonth(string LeaveYearType, string Usr_Login, string CompanyCode, string CentralProfile)
        {
            int Ret = 0;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {

                    string[] strArrLeaveYearType = LeaveYearType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string strLeaveYearType in strArrLeaveYearType)
                    {
                        string strLeaveYear = strLeaveYearType.Substring(0, 4);
                        string strLeaveType = strLeaveYearType.Substring(4, 2);
                        int iNxtYear = Convert.ToInt32(strLeaveYear) + 1;

                        #region UPDATE T_LeaveSchedule | INSERT T_LeaveScheduleTrl
                        dal.ExecuteNonQuery(string.Format(@"
                                                            UPDATE T_LeaveSchedule
                                                            SET Tls_CycleIndicator = 'P'
                                                                , Tls_UpdatedBy = '{2}'
                                                                , Tls_UpdatedDate = GETDATE()
                                                            WHERE Tls_LeaveYear = '{0}'
                                                                AND Tls_LeaveCode = '{1}'
                                                                AND Tls_CycleIndicator = 'C'

                                                            INSERT INTO T_LeaveScheduleTrl (Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,Tls_CreatedBy,Tls_CreatedDate)
                                                            SELECT Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,'{2}',GETDATE()
                                                            FROM T_LeaveSchedule
                                                            WHERE Tls_LeaveYear = '{0}'
                                                                 AND Tls_LeaveCode = '{1}'
                                                                 AND Tls_CycleIndicator = 'P'

                                                            UPDATE T_LeaveSchedule
                                                            SET Tls_CycleIndicator = 'C'
                                                                , Tls_UpdatedBy = '{2}'
                                                                , Tls_UpdatedDate = GETDATE()
                                                            WHERE Tls_LeaveYear = '{3}'
                                                                AND Tls_LeaveCode = '{1}'
                                                                AND Tls_CycleIndicator = 'F'

                                                            INSERT INTO T_LeaveScheduleTrl (Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,Tls_CreatedBy,Tls_CreatedDate)
                                                            SELECT Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,'{2}',GETDATE()
                                                            FROM T_LeaveSchedule
                                                            WHERE Tls_LeaveYear = '{3}'
                                                                 AND Tls_LeaveCode = '{1}'
                                                                 AND Tls_CycleIndicator = 'C'
                                                            ", strLeaveYear, strLeaveType, Usr_Login, iNxtYear));
                        #endregion

                        #region INSERT LEAVE RECORDS TO HISTORY | DELETE LEAVE LEDGER
                        dal.ExecuteNonQuery(string.Format(@"INSERT INTO {0}..T_EmpLeaveLdgHst
                                                           (Tll_LeaveYear
                                                           ,Tll_IDNo
                                                           ,Tll_LeaveCode
                                                           ,Tll_BegCredit
                                                           ,Tll_CarryForwardCredit
                                                           ,Tll_UsedCredit
                                                           ,Tll_ConvertedCredit
                                                           ,Tll_PendingCreditA
                                                           ,Tll_PendingCreditC
                                                           ,Tll_RecordStatus
                                                           ,Tll_CreatedBy
                                                           ,Tll_CreatedDate)
                                                        SELECT A.Tll_LeaveYear
                                                               ,A.Tll_IDNo
                                                               ,A.Tll_LeaveCode
                                                               ,A.Tll_BegCredit
                                                               ,A.Tll_CarryForwardCredit
                                                               ,A.Tll_UsedCredit
                                                               ,A.Tll_ConvertedCredit
                                                               ,A.Tll_PendingCreditA
                                                               ,A.Tll_PendingCreditC
                                                               ,A.Tll_RecordStatus
                                                               ,A.Tll_CreatedBy
                                                               ,GETDATE() 
                                                        FROM {0}..T_EmpLeaveLdg A
                                                        INNER JOIN M_Employee ON Mem_IDNo = A.Tll_IDNo
                                                            AND Mem_WorkStatus LIKE 'A%'
                                                        LEFT JOIN {0}..T_EmpLeaveLdgHst B
                                                        ON A.Tll_IDNo = B.Tll_IDNo 
                                                            AND A.Tll_LeaveCode = B.Tll_LeaveCode
                                                            AND A.Tll_LeaveYear = B.Tll_LeaveYear
                                                        WHERE A.Tll_LeaveYear = '{1}'
                                                            AND A.Tll_LeaveCode = '{2}'
                                                            ---AND B.Tll_LeaveYear IS NULL
                                                            AND B.Tll_LeaveCode IS NULL


                                                        DELETE FROM {0}..T_EmpLeaveLdg
                                                        FROM {0}..T_EmpLeaveLdg 
                                                        INNER JOIN M_Employee ON Mem_IDNo = Tll_IDNo
                                                            AND Mem_WorkStatus LIKE 'A%'
                                                        WHERE Tll_LeaveYear = '{1}'
                                                            AND Tll_LeaveCode = '{2}'"
                                                            , CentralProfile
                                                            , strLeaveYear
                                                            , strLeaveType));
                        #endregion

                        #region //UPDATE OTHER LEAVE TYPES
                        //dal.ExecuteNonQuery(string.Format(@"DECLARE @StartLeaveYear CHAR(6) = (SELECT Mlv_StartLeaveYear FROM {0}..M_Leave WHERE Mlv_CompanyCode = '{1}' AND Mlv_LeaveCode = '{3}')
                        //                                DECLARE @EndLeaveYear CHAR(6)   = (SELECT Mlv_EndLeaveYear FROM {0}..M_Leave WHERE Mlv_CompanyCode = '{1}' AND Mlv_LeaveCode = '{3}')

                        //                                    UPDATE T_LeaveSchedule
                        //                                      SET Tls_CycleIndicator = 'P'
                        //                                      , Tls_UpdatedBy = '{4}'
                        //                                      , Tls_UpdatedDate = GETDATE()
                        //                                   FROM T_LeaveSchedule 
                        //                                   INNER JOIN {0}..M_Leave ON Tls_LeaveCode = Mlv_LeaveCode
                        //                                        AND Mlv_CompanyCode ='{1}'  
	                       //                                     AND Mlv_CreditSysGen = 0 
							                 //                   AND Mlv_IsCombineLeave = 0
                        //                                        AND Mlv_StartLeaveYear = @StartLeaveYear
                        //                                        AND Mlv_EndLeaveYear = @EndLeaveYear
                        //                                  WHERE Tls_LeaveYear = '{2}'
                        //                                       AND Tls_CycleIndicator = 'C'

                        //                                INSERT INTO T_LeaveScheduleTrl (Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,Tls_CreatedBy,Tls_CreatedDate)
                        //                                SELECT Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,'{4}',GETDATE()
                        //                                FROM T_LeaveSchedule
                        //                                INNER JOIN {0}..M_Leave ON Tls_LeaveCode = Mlv_LeaveCode
                        //                                        AND Mlv_CompanyCode ='{1}'  
	                       //                                     AND Mlv_CreditSysGen = 0 
							                 //                   AND Mlv_IsCombineLeave = 0
                        //                                        AND Mlv_StartLeaveYear = @StartLeaveYear
                        //                                        AND Mlv_EndLeaveYear = @EndLeaveYear
                        //                                WHERE Tls_LeaveYear = '{2}'
                        //                                        AND Tls_CycleIndicator = 'P'

                        //                                UPDATE T_LeaveSchedule
                        //                                      SET Tls_CycleIndicator = 'C'
                        //                                      , Tls_UpdatedBy = '{4}'
                        //                                      , Tls_UpdatedDate = GETDATE()
                        //                                   FROM T_LeaveSchedule 
                        //                                   INNER JOIN {0}..M_Leave ON Tls_LeaveCode = Mlv_LeaveCode
                        //                                        AND Mlv_CompanyCode ='{1}'  
	                       //                                     AND Mlv_CreditSysGen = 0 
							                 //                   AND Mlv_IsCombineLeave = 0
                        //                                        AND Mlv_StartLeaveYear = @StartLeaveYear
                        //                                        AND Mlv_EndLeaveYear = @EndLeaveYear
                        //                                  WHERE Tls_LeaveYear = '{5}'
                        //                                       AND Tls_CycleIndicator = 'F'

                        //                                INSERT INTO T_LeaveScheduleTrl (Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,Tls_CreatedBy,Tls_CreatedDate)
                        //                                SELECT Tls_LeaveYear,Tls_LeaveCode,Tls_StartCycle,Tls_EndCycle,Tls_CycleIndicator,Tls_CreditPickUp,Tls_LastCreditMonth,Tls_LastCreditBy,Tls_LastCreditDate,Tls_CarryForwardPickUp,Tls_CarryForward,Tls_CarryForwardBy,Tls_CarryForwardDate,Tls_LeaveRefundPickUp,Tls_LeaveRefundFlag,Tls_LeaveRefundBy,Tls_LeaveRefundDate,Tls_RecordStatus,'{4}',GETDATE()
                        //                                FROM T_LeaveSchedule
                        //                                INNER JOIN {0}..M_Leave ON Tls_LeaveCode = Mlv_LeaveCode
                        //                                        AND Mlv_CompanyCode ='{1}'  
	                       //                                     AND Mlv_CreditSysGen = 0 
							                 //                   AND Mlv_IsCombineLeave = 0
                        //                                        AND Mlv_StartLeaveYear = @StartLeaveYear
                        //                                        AND Mlv_EndLeaveYear = @EndLeaveYear
                        //                                WHERE Tls_LeaveYear = '{5}'
                        //                                        AND Tls_CycleIndicator = 'C'
                        //                            ", CentralProfile
                        //                                 , CompanyCode
                        //                                 , strLeaveYear
                        //                                 , strLeaveType
                        //                                 , Usr_Login
                        //                                 , iNxtYear));
                        #endregion
                    }

                    dal.CommitTransaction();
                    Ret = 1;
                    
                }
                catch
                {
                    dal.RollBackTransaction();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return Ret;
        }

        public void ClearPayrollTables(string PayrollPeriod)
        {
            string query = @"DELETE FROM T_EmpPayroll WHERE Tpy_PayCycle = '{0}'
                             DELETE FROM T_EmpPayroll2 WHERE Tpy_PayCycle = '{0}'
                             DELETE FROM T_EmpDeductionDtl WHERE Tdd_ThisPayCycle = '{0}'
                             DELETE FROM T_EmpPayrollMisc WHERE Tpm_PayCycle = '{0}'
                             DELETE FROM T_EmpDeductionHdrCycle WHERE Tdh_PayCycle = '{0}'
                             DELETE FROM T_EmpSpecialPayroll WHERE Tpy_PayCycle = '{0}'";
            query = string.Format(query, PayrollPeriod);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(query);
                dal.CloseDB();
            }
        }
    }
}
