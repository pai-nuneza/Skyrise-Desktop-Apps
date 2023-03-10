using System;
using System.Collections.Generic;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Configuration;

namespace Payroll.BLogic
{
    public class AssumedDayPostingBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        //SystemCycleProcessingBL SystemCycleProcessingBL;
        CommonBL commonBL;
        

        public string CompanyCode = "";
        public string CentralProfile = "";
        public string MenuCode = "";

        //Flags and parameters
        public string ASSUMEPOSTCRIT = "";
        public string EQUIVASSUMECRITERIA = "";
        public DataTable ASSUMEPOSTGROUP = null;

        #endregion

        #region <Override Functions>

        public override int Add(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public delegate void StatusEventHandler(object sender, StatusEventArgs e);
        public class StatusEventArgs : System.EventArgs
        {
            public string Status;
            public bool Done;

            public StatusEventArgs(string strStat, bool bDone)
            {
                Status = strStat;
                Done = bDone;
            }
        }

        public event StatusEventHandler StatusHandler;

        public AssumedDayPostingBL()
        {
        }

        public void AssumedDayPosting(bool ProcessAll, bool bAssumeDays, string PayrollPeriod, string IDNumber, object startAssume, object endAssume, string UserLogin, string companyCode, string centralProfile, string menucode, DALHelper dalHelper)
        {
            #region Initial Setup
            this.dal = dalHelper;
            commonBL = new CommonBL();

            CompanyCode = companyCode;
            CentralProfile = centralProfile;
            MenuCode = menucode;

            #endregion
            string PrevPayrollPeriod = commonBL.GetPrevPayPeriod(PayrollPeriod, dal);
            DataSet dsResult = commonBL.GetPayCycleDtl2(PrevPayrollPeriod, companyCode, centralProfile, dal);

            string AssumedDaysErrorMsg = GetAssumedDaysParameters();
            if (!AssumedDaysErrorMsg.Equals(""))
                throw new Exception(AssumedDaysErrorMsg);

            string IDNumberFilter = "";
            if (!ProcessAll && IDNumber != "")
                IDNumberFilter = string.Format("AND Ttr_IDNo = '{0}'", IDNumber);

            #region Initialize Current Pay Period
            string strInitializeCurrent = string.Format(@" UPDATE T_EmpTimeRegister
                                                            SET Ttr_AssumedPost = 'N'
                                                            WHERE Ttr_AssumedFlag = 0
                                                            AND Ttr_PayCycle = '{0}'
                                                            {1}
                                                            ", PayrollPeriod
                                                            , IDNumberFilter);
            #endregion

            #region Create Trail for Previous Pay Period
            string strTrailPrevious = @" 
DECLARE @AffectedPayPeriod VARCHAR(7) = (SELECT TOP(1) Tps_PayCycle 
                                        FROM T_PaySchedule 
                                        WHERE Tps_CycleIndicator = 'P'
                                        ORDER BY Tps_StartCycle DESC)
DECLARE @AdjustPayPeriod VARCHAR(7) = (SELECT Tps_PayCycle 
									   FROM T_PaySchedule 
									   WHERE Tps_CycleIndicator = 'C' 
									    AND Tps_RecordStatus = 'A')

--INSERT INTO LOG LEDGER TRAIL
INSERT INTO T_EmpTimeRegisterTrl
(
	    Ttr_IDNo
        ,Ttr_Date
        ,Ttr_AdjPayCycle
        ,Ttr_PayCycle
        ,Ttr_DayCode
        ,Ttr_ShiftCode
        ,Ttr_HolidayFlag
        ,Ttr_RestDayFlag
        ,Ttr_ActIn_1
        ,Ttr_ActOut_1
        ,Ttr_ActIn_2
        ,Ttr_ActOut_2
        ,Ttr_WFPayLVCode
        ,Ttr_WFPayLVHr
        ,Ttr_PayLVMin
        ,Ttr_ExcLVMin
        ,Ttr_WFNoPayLVCode
        ,Ttr_WFNoPayLVHr
        ,Ttr_NoPayLVMin
        ,Ttr_WFOTAdvHr
        ,Ttr_WFOTPostHr
        ,Ttr_OTMin
        ,Ttr_CompOTMin
        ,Ttr_OffsetOTMin
        ,Ttr_WFTimeMod
        ,Ttr_WFFlexTime
        ,Ttr_Amnesty
        ,Ttr_SkipService
        ,Ttr_SkipServiceBy
        ,Ttr_SkipServiceDate
        ,Ttr_AssumedFlag
        ,Ttr_AssumedBy
        ,Ttr_AssumedDate
        ,Ttr_AssumedPost
        ,Ttr_ConvIn_1Min
        ,Ttr_ConvOut_1Min
        ,Ttr_ConvIn_2Min
        ,Ttr_ConvOut_2Min
        ,Ttr_CompIn_1Min
        ,Ttr_CompOut_1Min
        ,Ttr_CompIn_2Min
        ,Ttr_CompOut_2Min
        ,Ttr_CompAdvOTMin
        ,Ttr_ShiftIn_1Min
        ,Ttr_ShiftOut_1Min
        ,Ttr_ShiftIn_2Min
        ,Ttr_ShiftOut_2Min
        ,Ttr_ShiftMin
        ,Ttr_ScheduleType
        ,Ttr_ActLT1Min
        ,Ttr_ActLT2Min
        ,Ttr_CompLT1Min
        ,Ttr_CompLT2Min
        ,Ttr_ActUT1Min
        ,Ttr_ActUT2Min
        ,Ttr_CompUT1Min
        ,Ttr_CompUT2Min
        ,Ttr_InitialABSMin
        ,Ttr_CompABSMin
        ,Ttr_CompREGMin
        ,Ttr_CompWorkMin
        ,Ttr_CompNDMin
        ,Ttr_CompNDOTMin
        ,Ttr_PrvDayWorkMin
        ,Ttr_PrvDayHolRef
        ,Ttr_PDHOLHour
        ,Ttr_PDRESTLEGHOLDay
        ,Ttr_WorkDay
        ,Ttr_EXPHour
        ,Ttr_ABSHour
        ,Ttr_REGHour
        ,Ttr_OTHour
        ,Ttr_NDHour
        ,Ttr_NDOTHour
        ,Ttr_LVHour
        ,Ttr_PaidBreakHour
        ,Ttr_OBHour
        ,Ttr_RegPlusHour
        ,Ttr_TBAmt01
        ,Ttr_TBAmt02
        ,Ttr_TBAmt03
        ,Ttr_TBAmt04
        ,Ttr_TBAmt05
        ,Ttr_TBAmt06
        ,Ttr_TBAmt07
        ,Ttr_TBAmt08
        ,Ttr_TBAmt09
        ,Ttr_TBAmt10
        ,Ttr_TBAmt11
        ,Ttr_TBAmt12
        ,Ttr_WorkLocationCode
        ,Ttr_CalendarGroup
        ,Ttr_PremiumGrpCode
        ,Ttr_PayrollGroup
        ,Ttr_CostcenterCode
        ,Ttr_EmploymentStatusCode
        ,Ttr_PayrollType
        ,Ttr_Grade
        ,Usr_Login
        ,Ludatetime
)
        SELECT A.Ttr_IDNo
	    ,A.Ttr_Date
	    ,@AdjustPayPeriod
        ,A.Ttr_PayCycle
        ,A.Ttr_DayCode
        ,A.Ttr_ShiftCode
        ,A.Ttr_HolidayFlag
        ,A.Ttr_RestDayFlag
        ,A.Ttr_ActIn_1
        ,A.Ttr_ActOut_1
        ,A.Ttr_ActIn_2
        ,A.Ttr_ActOut_2
        ,A.Ttr_WFPayLVCode
        ,A.Ttr_WFPayLVHr
        ,A.Ttr_PayLVMin
        ,A.Ttr_ExcLVMin
        ,A.Ttr_WFNoPayLVCode
        ,A.Ttr_WFNoPayLVHr
        ,A.Ttr_NoPayLVMin
        ,A.Ttr_WFOTAdvHr
        ,A.Ttr_WFOTPostHr
        ,A.Ttr_OTMin
        ,A.Ttr_CompOTMin
        ,A.Ttr_OffsetOTMin
        ,A.Ttr_WFTimeMod
        ,A.Ttr_WFFlexTime
        ,A.Ttr_Amnesty
        ,A.Ttr_SkipService
        ,A.Ttr_SkipServiceBy
        ,A.Ttr_SkipServiceDate
        ,A.Ttr_AssumedFlag
        ,A.Ttr_AssumedBy
        ,A.Ttr_AssumedDate
        ,A.Ttr_AssumedPost
        ,A.Ttr_ConvIn_1Min
        ,A.Ttr_ConvOut_1Min
        ,A.Ttr_ConvIn_2Min
        ,A.Ttr_ConvOut_2Min
        ,A.Ttr_CompIn_1Min
        ,A.Ttr_CompOut_1Min
        ,A.Ttr_CompIn_2Min
        ,A.Ttr_CompOut_2Min
        ,A.Ttr_CompAdvOTMin
        ,A.Ttr_ShiftIn_1Min
        ,A.Ttr_ShiftOut_1Min
        ,A.Ttr_ShiftIn_2Min
        ,A.Ttr_ShiftOut_2Min
        ,A.Ttr_ShiftMin
        ,A.Ttr_ScheduleType
        ,A.Ttr_ActLT1Min
        ,A.Ttr_ActLT2Min
        ,A.Ttr_CompLT1Min
        ,A.Ttr_CompLT2Min
        ,A.Ttr_ActUT1Min
        ,A.Ttr_ActUT2Min
        ,A.Ttr_CompUT1Min
        ,A.Ttr_CompUT2Min
        ,A.Ttr_InitialABSMin
        ,A.Ttr_CompABSMin
        ,A.Ttr_CompREGMin
        ,A.Ttr_CompWorkMin
        ,A.Ttr_CompNDMin
        ,A.Ttr_CompNDOTMin
        ,A.Ttr_PrvDayWorkMin
        ,A.Ttr_PrvDayHolRef
        ,A.Ttr_PDHOLHour
        ,A.Ttr_PDRESTLEGHOLDay
        ,A.Ttr_WorkDay
        ,A.Ttr_EXPHour
        ,A.Ttr_ABSHour
        ,A.Ttr_REGHour
        ,A.Ttr_OTHour
        ,A.Ttr_NDHour
        ,A.Ttr_NDOTHour
        ,A.Ttr_LVHour
        ,A.Ttr_PaidBreakHour
        ,A.Ttr_OBHour
        ,A.Ttr_RegPlusHour
        ,A.Ttr_TBAmt01
        ,A.Ttr_TBAmt02
        ,A.Ttr_TBAmt03
        ,A.Ttr_TBAmt04
        ,A.Ttr_TBAmt05
        ,A.Ttr_TBAmt06
        ,A.Ttr_TBAmt07
        ,A.Ttr_TBAmt08
        ,A.Ttr_TBAmt09
        ,A.Ttr_TBAmt10
        ,A.Ttr_TBAmt11
        ,A.Ttr_TBAmt12
        ,A.Ttr_WorkLocationCode
        ,A.Ttr_CalendarGroup
        ,A.Ttr_PremiumGrpCode
        ,A.Ttr_PayrollGroup
        ,A.Ttr_CostcenterCode
        ,A.Ttr_EmploymentStatusCode
        ,A.Ttr_PayrollType
        ,A.Ttr_Grade
        ,A.Usr_Login
        ,A.Ludatetime
FROM T_EmpTimeRegisterHst A
LEFT JOIN T_EmpTimeRegisterTrl B
ON A.Ttr_IDNo = B.Ttr_IDNo
	AND A.Ttr_Date = B.Ttr_Date
    AND A.Ttr_PayCycle = B.Ttr_PayCycle
	AND B.Ttr_AdjPayCycle = @AdjustPayPeriod
WHERE B.Ttr_IDNo IS NULL
    AND A.Ttr_PayCycle = @AffectedPayPeriod
    {0}

---INSERT INTO LOG LEDGER MISC TRAIL
INSERT INTO T_EmpTimeRegisterMiscTrl  
(   Ttm_IDNo
      ,Ttm_Date
      ,Ttm_AdjPayCycle
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
      ,Ttm_ActIn2
      ,Ttm_ActOut1
      ,Ttm_ActOut2
      ,Usr_Login
      ,Ludatetime
 )
 
 SELECT A.Ttm_IDNo
      ,A.Ttm_Date
      ,@AdjustPayPeriod
      ,A.Ttm_PayCycle
      ,A.Ttm_ActIn_01
      ,A.Ttm_ActOut_01
      ,A.Ttm_ActIn_02
      ,A.Ttm_ActOut_02
      ,A.Ttm_ActIn_03
      ,A.Ttm_ActOut_03
      ,A.Ttm_ActIn_04
      ,A.Ttm_ActOut_04
      ,A.Ttm_ActIn_05
      ,A.Ttm_ActOut_05
      ,A.Ttm_ActIn_06
      ,A.Ttm_ActOut_06
      ,A.Ttm_ActIn_07
      ,A.Ttm_ActOut_07
      ,A.Ttm_ActIn_08
      ,A.Ttm_ActOut_08
      ,A.Ttm_ActIn_09
      ,A.Ttm_ActOut_09
      ,A.Ttm_ActIn_10
      ,A.Ttm_ActOut_10
      ,A.Ttm_ActIn_11
      ,A.Ttm_ActOut_11
      ,A.Ttm_ActIn_12
      ,A.Ttm_ActOut_12
      ,A.Ttm_Result
      ,A.Ttm_ActIn1
      ,A.Ttm_ActIn2
      ,A.Ttm_ActOut1
      ,A.Ttm_ActOut2
      ,A.Usr_Login
      ,A.Ludatetime
FROM T_EmpTimeRegisterMiscHst A
LEFT JOIN T_EmpTimeRegisterMiscTrl B
ON A.Ttm_IDNo = B.Ttm_IDNo
	AND A.Ttm_Date = B.Ttm_Date
    AND A.Ttm_PayCycle = B.Ttm_PayCycle
	AND B.Ttm_AdjPayCycle = @AdjustPayPeriod
WHERE B.Ttm_IDNo IS NULL
    AND A.Ttm_PayCycle = @AffectedPayPeriod
    {3}

--INSERT INTO PAYROLL TRANSACTION TRAIL
INSERT INTO T_EmpPayTranHdrTrl
(
        Tph_IDNo
        ,Tph_AdjPayCycle
        ,Tph_PayCycle
        ,Tph_LTHr
        ,Tph_UTHr
        ,Tph_UPLVHr
        ,Tph_ABSLEGHOLHr
        ,Tph_ABSSPLHOLHr
        ,Tph_ABSCOMPHOLHr
        ,Tph_ABSPSDHr
        ,Tph_ABSOTHHOLHr
        ,Tph_WDABSHr
        ,Tph_LTUTMaxHr
        ,Tph_ABSHr
        ,Tph_REGHr
        ,Tph_PDLVHr
        ,Tph_PDLEGHOLHr
        ,Tph_PDSPLHOLHr
        ,Tph_PDCOMPHOLHr
        ,Tph_PDPSDHr
        ,Tph_PDOTHHOLHr
        ,Tph_PDRESTLEGHOLHr
        ,Tph_REGOTHr
        ,Tph_REGNDHr
        ,Tph_REGNDOTHr
        ,Tph_RESTHr
        ,Tph_RESTOTHr
        ,Tph_RESTNDHr
        ,Tph_RESTNDOTHr
        ,Tph_LEGHOLHr
        ,Tph_LEGHOLOTHr
        ,Tph_LEGHOLNDHr
        ,Tph_LEGHOLNDOTHr
        ,Tph_SPLHOLHr
        ,Tph_SPLHOLOTHr
        ,Tph_SPLHOLNDHr
        ,Tph_SPLHOLNDOTHr
        ,Tph_PSDHr
        ,Tph_PSDOTHr
        ,Tph_PSDNDHr
        ,Tph_PSDNDOTHr
        ,Tph_COMPHOLHr
        ,Tph_COMPHOLOTHr
        ,Tph_COMPHOLNDHr
        ,Tph_COMPHOLNDOTHr
        ,Tph_RESTLEGHOLHr
        ,Tph_RESTLEGHOLOTHr
        ,Tph_RESTLEGHOLNDHr
        ,Tph_RESTLEGHOLNDOTHr
        ,Tph_RESTSPLHOLHr
        ,Tph_RESTSPLHOLOTHr
        ,Tph_RESTSPLHOLNDHr
        ,Tph_RESTSPLHOLNDOTHr
        ,Tph_RESTCOMPHOLHr
        ,Tph_RESTCOMPHOLOTHr
        ,Tph_RESTCOMPHOLNDHr
        ,Tph_RESTCOMPHOLNDOTHr
        ,Tph_RESTPSDHr
        ,Tph_RESTPSDOTHr
        ,Tph_RESTPSDNDHr
        ,Tph_RESTPSDNDOTHr
        ,Tph_SRGAdjHr
        ,Tph_SRGAdjAmt
        ,Tph_SOTAdjHr
        ,Tph_SOTAdjAmt
        ,Tph_SHOLAdjHr
        ,Tph_SHOLAdjAmt
        ,Tph_SNDAdjHr
        ,Tph_SNDAdjAmt
        ,Tph_SLVAdjHr
        ,Tph_SLVAdjAmt
        ,Tph_MRGAdjHr
        ,Tph_MRGAdjAmt
        ,Tph_MOTAdjHr
        ,Tph_MOTAdjAmt
        ,Tph_MHOLAdjHr
        ,Tph_MHOLAdjAmt
        ,Tph_MNDAdjHr
        ,Tph_MNDAdjAmt
        ,Tph_TotalAdjAmt
        ,Tph_TaxableIncomeAmt
        ,Tph_NontaxableIncomeAmt
        ,Tph_WorkDay
        ,Tph_PayrollType
        ,Tph_RetainUserEntry
        ,Usr_Login
        ,Ludatetime
)
        SELECT A.Tph_IDNo
        ,@AdjustPayPeriod
        ,A.Tph_PayCycle
        ,A.Tph_LTHr
        ,A.Tph_UTHr
        ,A.Tph_UPLVHr
        ,A.Tph_ABSLEGHOLHr
        ,A.Tph_ABSSPLHOLHr
        ,A.Tph_ABSCOMPHOLHr
        ,A.Tph_ABSPSDHr
        ,A.Tph_ABSOTHHOLHr
        ,A.Tph_WDABSHr
        ,A.Tph_LTUTMaxHr
        ,A.Tph_ABSHr
        ,A.Tph_REGHr
        ,A.Tph_PDLVHr
        ,A.Tph_PDLEGHOLHr
        ,A.Tph_PDSPLHOLHr
        ,A.Tph_PDCOMPHOLHr
        ,A.Tph_PDPSDHr
        ,A.Tph_PDOTHHOLHr
        ,A.Tph_PDRESTLEGHOLHr
        ,A.Tph_REGOTHr
        ,A.Tph_REGNDHr
        ,A.Tph_REGNDOTHr
        ,A.Tph_RESTHr
        ,A.Tph_RESTOTHr
        ,A.Tph_RESTNDHr
        ,A.Tph_RESTNDOTHr
        ,A.Tph_LEGHOLHr
        ,A.Tph_LEGHOLOTHr
        ,A.Tph_LEGHOLNDHr
        ,A.Tph_LEGHOLNDOTHr
        ,A.Tph_SPLHOLHr
        ,A.Tph_SPLHOLOTHr
        ,A.Tph_SPLHOLNDHr
        ,A.Tph_SPLHOLNDOTHr
        ,A.Tph_PSDHr
        ,A.Tph_PSDOTHr
        ,A.Tph_PSDNDHr
        ,A.Tph_PSDNDOTHr
        ,A.Tph_COMPHOLHr
        ,A.Tph_COMPHOLOTHr
        ,A.Tph_COMPHOLNDHr
        ,A.Tph_COMPHOLNDOTHr
        ,A.Tph_RESTLEGHOLHr
        ,A.Tph_RESTLEGHOLOTHr
        ,A.Tph_RESTLEGHOLNDHr
        ,A.Tph_RESTLEGHOLNDOTHr
        ,A.Tph_RESTSPLHOLHr
        ,A.Tph_RESTSPLHOLOTHr
        ,A.Tph_RESTSPLHOLNDHr
        ,A.Tph_RESTSPLHOLNDOTHr
        ,A.Tph_RESTCOMPHOLHr
        ,A.Tph_RESTCOMPHOLOTHr
        ,A.Tph_RESTCOMPHOLNDHr
        ,A.Tph_RESTCOMPHOLNDOTHr
        ,A.Tph_RESTPSDHr
        ,A.Tph_RESTPSDOTHr
        ,A.Tph_RESTPSDNDHr
        ,A.Tph_RESTPSDNDOTHr
        ,A.Tph_SRGAdjHr
        ,A.Tph_SRGAdjAmt
        ,A.Tph_SOTAdjHr
        ,A.Tph_SOTAdjAmt
        ,A.Tph_SHOLAdjHr
        ,A.Tph_SHOLAdjAmt
        ,A.Tph_SNDAdjHr
        ,A.Tph_SNDAdjAmt
        ,A.Tph_SLVAdjHr
        ,A.Tph_SLVAdjAmt
        ,A.Tph_MRGAdjHr
        ,A.Tph_MRGAdjAmt
        ,A.Tph_MOTAdjHr
        ,A.Tph_MOTAdjAmt
        ,A.Tph_MHOLAdjHr
        ,A.Tph_MHOLAdjAmt
        ,A.Tph_MNDAdjHr
        ,A.Tph_MNDAdjAmt
        ,A.Tph_TotalAdjAmt
        ,A.Tph_TaxableIncomeAmt
        ,A.Tph_NontaxableIncomeAmt
        ,A.Tph_WorkDay
        ,A.Tph_PayrollType
        ,A.Tph_RetainUserEntry
        ,A.Usr_Login
        ,A.Ludatetime
FROM T_EmpPayTranHdrHst A
LEFT JOIN T_EmpPayTranHdrTrl B
ON A.Tph_IDNo = B.Tph_IDNo
	AND A.Tph_PayCycle = B.Tph_PayCycle
    AND B.Tph_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tph_IDNo IS NULL
    AND A.Tph_PayCycle = @AffectedPayPeriod
    {1}

--INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
INSERT INTO T_EmpPayTranDtlTrl
(
        Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_LTHr
        ,Tpd_UTHr
        ,Tpd_UPLVHr
        ,Tpd_ABSLEGHOLHr
        ,Tpd_ABSSPLHOLHr
        ,Tpd_ABSCOMPHOLHr
        ,Tpd_ABSPSDHr
        ,Tpd_ABSOTHHOLHr
        ,Tpd_WDABSHr
        ,Tpd_LTUTMaxHr
        ,Tpd_ABSHr
        ,Tpd_REGHr
        ,Tpd_PDLVHr
        ,Tpd_PDLEGHOLHr
        ,Tpd_PDSPLHOLHr
        ,Tpd_PDCOMPHOLHr
        ,Tpd_PDPSDHr
        ,Tpd_PDOTHHOLHr
        ,Tpd_PDRESTLEGHOLHr
        ,Tpd_REGOTHr
        ,Tpd_REGNDHr
        ,Tpd_REGNDOTHr
        ,Tpd_RESTHr
        ,Tpd_RESTOTHr
        ,Tpd_RESTNDHr
        ,Tpd_RESTNDOTHr
        ,Tpd_LEGHOLHr
        ,Tpd_LEGHOLOTHr
        ,Tpd_LEGHOLNDHr
        ,Tpd_LEGHOLNDOTHr
        ,Tpd_SPLHOLHr
        ,Tpd_SPLHOLOTHr
        ,Tpd_SPLHOLNDHr
        ,Tpd_SPLHOLNDOTHr
        ,Tpd_PSDHr
        ,Tpd_PSDOTHr
        ,Tpd_PSDNDHr
        ,Tpd_PSDNDOTHr
        ,Tpd_COMPHOLHr
        ,Tpd_COMPHOLOTHr
        ,Tpd_COMPHOLNDHr
        ,Tpd_COMPHOLNDOTHr
        ,Tpd_RESTLEGHOLHr
        ,Tpd_RESTLEGHOLOTHr
        ,Tpd_RESTLEGHOLNDHr
        ,Tpd_RESTLEGHOLNDOTHr
        ,Tpd_RESTSPLHOLHr
        ,Tpd_RESTSPLHOLOTHr
        ,Tpd_RESTSPLHOLNDHr
        ,Tpd_RESTSPLHOLNDOTHr
        ,Tpd_RESTCOMPHOLHr
        ,Tpd_RESTCOMPHOLOTHr
        ,Tpd_RESTCOMPHOLNDHr
        ,Tpd_RESTCOMPHOLNDOTHr
        ,Tpd_RESTPSDHr
        ,Tpd_RESTPSDOTHr
        ,Tpd_RESTPSDNDHr
        ,Tpd_RESTPSDNDOTHr
        ,Tpd_WorkDay
        ,Tpd_PayrollType
        ,Tpd_PremiumGrpCode
        ,Usr_Login
        ,Ludatetime
)
SELECT A.Tpd_IDNo
        ,@AdjustPayPeriod
        ,A.Tpd_PayCycle
        ,A.Tpd_Date
        ,A.Tpd_LTHr
        ,A.Tpd_UTHr
        ,A.Tpd_UPLVHr
        ,A.Tpd_ABSLEGHOLHr
        ,A.Tpd_ABSSPLHOLHr
        ,A.Tpd_ABSCOMPHOLHr
        ,A.Tpd_ABSPSDHr
        ,A.Tpd_ABSOTHHOLHr
        ,A.Tpd_WDABSHr
        ,A.Tpd_LTUTMaxHr
        ,A.Tpd_ABSHr
        ,A.Tpd_REGHr
        ,A.Tpd_PDLVHr
        ,A.Tpd_PDLEGHOLHr
        ,A.Tpd_PDSPLHOLHr
        ,A.Tpd_PDCOMPHOLHr
        ,A.Tpd_PDPSDHr
        ,A.Tpd_PDOTHHOLHr
        ,A.Tpd_PDRESTLEGHOLHr
        ,A.Tpd_REGOTHr
        ,A.Tpd_REGNDHr
        ,A.Tpd_REGNDOTHr
        ,A.Tpd_RESTHr
        ,A.Tpd_RESTOTHr
        ,A.Tpd_RESTNDHr
        ,A.Tpd_RESTNDOTHr
        ,A.Tpd_LEGHOLHr
        ,A.Tpd_LEGHOLOTHr
        ,A.Tpd_LEGHOLNDHr
        ,A.Tpd_LEGHOLNDOTHr
        ,A.Tpd_SPLHOLHr
        ,A.Tpd_SPLHOLOTHr
        ,A.Tpd_SPLHOLNDHr
        ,A.Tpd_SPLHOLNDOTHr
        ,A.Tpd_PSDHr
        ,A.Tpd_PSDOTHr
        ,A.Tpd_PSDNDHr
        ,A.Tpd_PSDNDOTHr
        ,A.Tpd_COMPHOLHr
        ,A.Tpd_COMPHOLOTHr
        ,A.Tpd_COMPHOLNDHr
        ,A.Tpd_COMPHOLNDOTHr
        ,A.Tpd_RESTLEGHOLHr
        ,A.Tpd_RESTLEGHOLOTHr
        ,A.Tpd_RESTLEGHOLNDHr
        ,A.Tpd_RESTLEGHOLNDOTHr
        ,A.Tpd_RESTSPLHOLHr
        ,A.Tpd_RESTSPLHOLOTHr
        ,A.Tpd_RESTSPLHOLNDHr
        ,A.Tpd_RESTSPLHOLNDOTHr
        ,A.Tpd_RESTCOMPHOLHr
        ,A.Tpd_RESTCOMPHOLOTHr
        ,A.Tpd_RESTCOMPHOLNDHr
        ,A.Tpd_RESTCOMPHOLNDOTHr
        ,A.Tpd_RESTPSDHr
        ,A.Tpd_RESTPSDOTHr
        ,A.Tpd_RESTPSDNDHr
        ,A.Tpd_RESTPSDNDOTHr
        ,A.Tpd_WorkDay
        ,A.Tpd_PayrollType
        ,A.Tpd_PremiumGrpCode
        ,A.Usr_Login
        ,A.Ludatetime
FROM T_EmpPayTranDtlHst A
LEFT JOIN T_EmpPayTranDtlTrl B
ON A.Tpd_IDNo = B.Tpd_IDNo
	AND A.Tpd_PayCycle = B.Tpd_PayCycle
	AND A.Tpd_Date = B.Tpd_Date
    AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tpd_IDNo IS NULL
    AND A.Tpd_PayCycle = @AffectedPayPeriod
    {2}	

--INSERT INTO PAYROLL TRANSACTION TRAIL EXT
INSERT INTO T_EmpPayTranHdrMiscTrl
(
        Tph_IDNo
        ,Tph_AdjPayCycle
        ,Tph_PayCycle
        ,Tph_Misc1Hr
        ,Tph_Misc1OTHr
        ,Tph_Misc1NDHr
        ,Tph_Misc1NDOTHr
        ,Tph_Misc2Hr
        ,Tph_Misc2OTHr
        ,Tph_Misc2NDHr
        ,Tph_Misc2NDOTHr
        ,Tph_Misc3Hr
        ,Tph_Misc3OTHr
        ,Tph_Misc3NDHr
        ,Tph_Misc3NDOTHr
        ,Tph_Misc4Hr
        ,Tph_Misc4OTHr
        ,Tph_Misc4NDHr
        ,Tph_Misc4NDOTHr
        ,Tph_Misc5Hr
        ,Tph_Misc5OTHr
        ,Tph_Misc5NDHr
        ,Tph_Misc5NDOTHr
        ,Tph_Misc6Hr
        ,Tph_Misc6OTHr
        ,Tph_Misc6NDHr
        ,Tph_Misc6NDOTHr
        ,Usr_Login
        ,Ludatetime
)
        SELECT A.Tph_IDNo
        ,@AdjustPayPeriod
        ,A.Tph_PayCycle
        ,A.Tph_Misc1Hr
        ,A.Tph_Misc1OTHr
        ,A.Tph_Misc1NDHr
        ,A.Tph_Misc1NDOTHr
        ,A.Tph_Misc2Hr
        ,A.Tph_Misc2OTHr
        ,A.Tph_Misc2NDHr
        ,A.Tph_Misc2NDOTHr
        ,A.Tph_Misc3Hr
        ,A.Tph_Misc3OTHr
        ,A.Tph_Misc3NDHr
        ,A.Tph_Misc3NDOTHr
        ,A.Tph_Misc4Hr
        ,A.Tph_Misc4OTHr
        ,A.Tph_Misc4NDHr
        ,A.Tph_Misc4NDOTHr
        ,A.Tph_Misc5Hr
        ,A.Tph_Misc5OTHr
        ,A.Tph_Misc5NDHr
        ,A.Tph_Misc5NDOTHr
        ,A.Tph_Misc6Hr
        ,A.Tph_Misc6OTHr
        ,A.Tph_Misc6NDHr
        ,A.Tph_Misc6NDOTHr
        ,A.Usr_Login
        ,A.Ludatetime
FROM T_EmpPayTranHdrMiscHst A
LEFT JOIN T_EmpPayTranHdrMiscTrl B
ON A.Tph_IDNo = B.Tph_IDNo
	AND A.Tph_PayCycle = B.Tph_PayCycle
    AND B.Tph_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tph_IDNo IS NULL
    AND A.Tph_PayCycle = @AffectedPayPeriod
    {1}
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
INSERT INTO T_EmpPayTranDtlMiscTrl
(
        Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_Misc1Hr
        ,Tpd_Misc1OTHr
        ,Tpd_Misc1NDHr
        ,Tpd_Misc1NDOTHr
        ,Tpd_Misc2Hr
        ,Tpd_Misc2OTHr
        ,Tpd_Misc2NDHr
        ,Tpd_Misc2NDOTHr
        ,Tpd_Misc3Hr
        ,Tpd_Misc3OTHr
        ,Tpd_Misc3NDHr
        ,Tpd_Misc3NDOTHr
        ,Tpd_Misc4Hr
        ,Tpd_Misc4OTHr
        ,Tpd_Misc4NDHr
        ,Tpd_Misc4NDOTHr
        ,Tpd_Misc5Hr
        ,Tpd_Misc5OTHr
        ,Tpd_Misc5NDHr
        ,Tpd_Misc5NDOTHr
        ,Tpd_Misc6Hr
        ,Tpd_Misc6OTHr
        ,Tpd_Misc6NDHr
        ,Tpd_Misc6NDOTHr
        ,Usr_Login
        ,Ludatetime
)
        SELECT A.Tpd_IDNo
        ,@AdjustPayPeriod
        ,A.Tpd_PayCycle
        ,A.Tpd_Date
        ,A.Tpd_Misc1Hr
        ,A.Tpd_Misc1OTHr
        ,A.Tpd_Misc1NDHr
        ,A.Tpd_Misc1NDOTHr
        ,A.Tpd_Misc2Hr
        ,A.Tpd_Misc2OTHr
        ,A.Tpd_Misc2NDHr
        ,A.Tpd_Misc2NDOTHr
        ,A.Tpd_Misc3Hr
        ,A.Tpd_Misc3OTHr
        ,A.Tpd_Misc3NDHr
        ,A.Tpd_Misc3NDOTHr
        ,A.Tpd_Misc4Hr
        ,A.Tpd_Misc4OTHr
        ,A.Tpd_Misc4NDHr
        ,A.Tpd_Misc4NDOTHr
        ,A.Tpd_Misc5Hr
        ,A.Tpd_Misc5OTHr
        ,A.Tpd_Misc5NDHr
        ,A.Tpd_Misc5NDOTHr
        ,A.Tpd_Misc6Hr
        ,A.Tpd_Misc6OTHr
        ,A.Tpd_Misc6NDHr
        ,A.Tpd_Misc6NDOTHr
        ,A.Usr_Login
        ,A.Ludatetime
        FROM T_EmpPayTranDtlMiscHst A
        LEFT JOIN T_EmpPayTranDtlMiscTrl B
        ON A.Tpd_IDNo = B.Tpd_IDNo
	        AND A.Tpd_PayCycle = B.Tpd_PayCycle
	        AND A.Tpd_Date = B.Tpd_Date
            AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
        WHERE B.Tpd_IDNo IS NULL
            AND A.Tpd_PayCycle = @AffectedPayPeriod 
            {2}";
            #endregion

            #region Clear Assume Flag for Previous Pay Period
            string strClearAssumePrevious = string.Format(@"UPDATE T_EmpTimeRegisterHst
                                                         SET Ttr_AssumedPost='A' 
                                                            , Usr_Login = '{1}'
                                                            , Ludatetime = GETDATE()
                                                         WHERE Ttr_AssumedPost='T'
                                                         AND Ttr_PayCycle = '{0}'
                                                         {2}
                                                            ", PrevPayrollPeriod
                                                            , UserLogin
                                                            , IDNumberFilter);
            #endregion

            #region Assume Current Pay Period
            string strAssumeCurrent = string.Format(@" UPDATE T_EmpTimeRegister
                                                    SET Ttr_AssumedPost = 'T' --Tagged
                                                        , Usr_Login = '{4}'
                                                        , Ludatetime = GETDATE()
                                                    FROM T_EmpTimeRegister
                                                    INNER JOIN M_PolicyDtl
                                                         ON Mpd_CompanyCode = '{0}'
                                                         AND Mpd_PolicyCode = 'ASSUMEPOSTGROUP'
                                                    INNER JOIN {6}..M_Day
													ON Ttr_DayCode = Mdy_DayCode
													    AND Mdy_CompanyCode = '{0}'
													    AND Mdy_HolidayFlag = 0
													    AND Mdy_RestdayFlag = 0
                                                    WHERE {1} = Mpd_SubCode
                                                        AND Ttr_Date BETWEEN '{2}' AND '{3}'
                                                        AND Ttr_AssumedFlag = 0
                                                    {5}

                                                    --SET ASSUMED POSTBACK FLAG TO N WHEN SEPARATED
                                                    UPDATE T_EmpTimeRegister
                                                    SET Ttr_AssumedPost = 'N'                                                                     
	                                                    , Usr_Login = '{4}'
	                                                    , Ludatetime = GETDATE()
                                                    FROM T_EmpTimeRegister
                                                    INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
                                                          AND Mem_SeparationDate <= '{3}'
	                                                      AND Ttr_Date >= Mem_SeparationDate
                                                    WHERE Ttr_Date BETWEEN '{2}' AND '{3}'
                                                    {5}
                                                    --SET ASSUMED POSTBACK FLAG TO N WHEN HIRED
                                                    UPDATE T_EmpTimeRegister
                                                    SET Ttr_AssumedPost = 'N'
                                                       , Usr_Login = '{4}'
                                                       , Ludatetime = GETDATE()
                                                    FROM T_EmpTimeRegister 
                                                    INNER JOIN M_Employee ON Mem_IDNo = Ttr_IDNo
                                                          AND Mem_IntakeDate > '{2}' 
                                                          AND Ttr_Date < Mem_IntakeDate
                                                    WHERE Ttr_Date BETWEEN '{2}' AND '{3}'
                                                    {5}"
                                                    , companyCode
                                                    , EQUIVASSUMECRITERIA
                                                    , startAssume
                                                    , endAssume
                                                    , UserLogin
                                                    , IDNumberFilter
                                                    , centralProfile);
            #endregion

            #region //Differential

            //string differentialqueryCur = string.Format(@"INSERT INTO T_EmpPayrollTrack
            //                                            (Tpt_Type
            //                                            ,Tpt_IDNo
            //                                            ,Tpt_ModuleCode
            //                                            ,Usr_Login
            //                                            ,Ludatetime
            //                                             )
            //                                            SELECT DISTINCT 'N'
            //                                            , Ttr_IDNo
            //                                            , '{0}'
            //                                            , '{1}'
            //                                            , GETDATE()
            //                                            FROM T_EmpTimeRegister
            //                                            LEFT JOIN T_EmpPayrollTrack 
            //                                            ON Tpt_IDNo = Ttr_IDNo
            //			AND Tpt_ModuleCode = '{0}'
            //                                            WHERE Ttr_AssumedPost = 'T'
            //                                            AND Ttr_PayCycle = '{2}'
            //                                            AND Tpt_IDNo IS NULL
            //                                            {3}
            //                                            ", MenuCode
            //                                            , UserLogin
            //                                            , PayrollPeriod
            //                                            , IDNumberFilter);

            //string differentialqueryPrev = string.Format(@"INSERT INTO T_EmpPayrollTrack
            //                                            (Tpt_Type
            //                                            ,Tpt_IDNo
            //                                            ,Tpt_ModuleCode
            //                                            ,Usr_Login
            //                                            ,Ludatetime
            //                                             )
            //                                            SELECT DISTINCT 'N'
            //                                            , Ttr_IDNo
            //                                            , '{0}'
            //                                            , '{1}'
            //                                            , GETDATE()
            //                                            FROM T_EmpTimeRegisterHst
            //                                            LEFT JOIN T_EmpPayrollTrack 
            //                                            ON Tpt_IDNo = Ttr_IDNo
            //			AND Tpt_ModuleCode = '{0}'
            //                                            WHERE Ttr_AssumedPost = 'A'
            //                                            AND Ttr_PayCycle = '{2}'
            //                                            AND Tpt_IDNo IS NULL
            //                                            {3}
            //                                            ", MenuCode
            //                                            , UserLogin
            //                                            , PrevPayrollPeriod
            //                                            , IDNumberFilter);


            #endregion

            #region Process Current
            if (bAssumeDays)
            {
                dal.ExecuteNonQuery(strInitializeCurrent);
                dal.ExecuteNonQuery(strAssumeCurrent);
                //dal.ExecuteNonQuery(differentialqueryCur);
            }
            #endregion

            #region Process History
            if (dsResult.Tables[0].Rows.Count > 0 && Convert.ToBoolean(dsResult.Tables[0].Rows[0]["Assume Days"].ToString()))
            {
                if (!ProcessAll && IDNumber != "")
                    dal.ExecuteNonQuery(string.Format(strTrailPrevious
                                                        , "AND A.Ttr_IDNo = '" + IDNumber + "'"
                                                        , "AND A.Tph_IDNo = '" + IDNumber + "'"
                                                        , "AND A.Tpd_IDNo = '" + IDNumber + "'"
                                                        , "AND A.Ttm_IDNo = '" + IDNumber + "'"));
                else
                {
                    string queryIDNumber = string.Format(@"SELECT DISTINCT Ttr_IDNo
                                                               FROM T_EmpTimeRegisterHst
                                                               WHERE Ttr_AssumedPost='T'
                                                                 AND Ttr_PayCycle = '{0}'
                                                            ", PrevPayrollPeriod);
                    DataSet dsIDNumber = dal.ExecuteDataSet(queryIDNumber);
                    string strIDNumber = "";
                    if (dsIDNumber != null && dsResult.Tables[0].Rows.Count > 0)
                        strIDNumber = JoinEmployeesFromDataTableArray(dsIDNumber.Tables[0], true);

                    if (strIDNumber.Equals(""))
                        dal.ExecuteNonQuery(string.Format(strTrailPrevious
                                                        , string.Format("AND A.Ttr_IDNo IN ({0})", strIDNumber)
                                                        , string.Format("AND A.Tph_IDNo IN ({0})", strIDNumber)
                                                        , string.Format("AND A.Tpd_IDNo IN ({0})", strIDNumber)
                                                        , string.Format("AND A.Ttm_IDNo IN ({0})", strIDNumber)));
                    else
                        dal.ExecuteNonQuery(string.Format(strTrailPrevious, "", "", "", ""));
                }
                    

                dal.ExecuteNonQuery(strClearAssumePrevious);
                //dal.ExecuteNonQuery(differentialqueryPrev);
            }
            #endregion
        }

        public string GetAssumedDaysParameters()
        {
            string AssumedDaysErrorMessage = "";
            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            ASSUMEPOSTCRIT = commonBL.GetParameterValueFromPayroll("ASSUMEPOSTCRIT", CompanyCode, dal);
            if (ASSUMEPOSTCRIT == "")
                AssumedDaysErrorMessage += "Assume Post Criteria is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format(" ASSUMEPOSTCRIT = {0} ", ASSUMEPOSTCRIT), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" ASSUMEPOSTCRIT = {0} ", ASSUMEPOSTCRIT), true));

            EQUIVASSUMECRITERIA = EquivAssumedDaysCritieria();

            ASSUMEPOSTGROUP = commonBL.GetParameterDtlListAllowedfromPayroll("ASSUMEPOSTGROUP", CompanyCode, dal);
            if (ASSUMEPOSTGROUP == null || ASSUMEPOSTGROUP.Rows.Count == 0)
                AssumedDaysErrorMessage += "AssumeGroup Mapping is not set-up in Policy Detail.\n";
            StatusHandler(this, new StatusEventArgs(string.Format(" ASSUMEPOSTGROUP = {0} ", ASSUMEPOSTGROUP), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" ASSUMEPOSTGROUP = {0} ", ASSUMEPOSTGROUP), true));
            #endregion
            return AssumedDaysErrorMessage;
        }

        public string JoinEmployeesFromDataTableArray(DataTable dtData, bool bWithQuotes)
        {
            string strListCommaDelimited = "";
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (bWithQuotes == true)
                    strListCommaDelimited += string.Format(@"'{0}',", GetValue(dtData.Rows[i][0]));
                else
                    strListCommaDelimited += string.Format(@"{0},", GetValue(dtData.Rows[i][0]));
            }
            if (strListCommaDelimited != "")
                strListCommaDelimited = strListCommaDelimited.Substring(0, strListCommaDelimited.Length - 1);

            return strListCommaDelimited;
        }

        public string EquivAssumedDaysCritieria()
        {
            string rString = "";
            string[] strArrItems = ASSUMEPOSTCRIT.Split(new char[] { '|' });
            foreach (string strArr in strArrItems)
            {
                switch (strArr)
                {
                    case "PAYTYPE":
                        rString += "Ttr_PayrollType +'|'+ ";
                        break;
                    case "EMPSTAT":
                        rString += "Ttr_EmploymentStatusCode +'|'+ ";
                        break;
                    case "PAYGRP":
                        rString += "Ttr_PayrollGroup +'|'+ ";
                        break;
                    case "GRADE":
                        rString += "Ttr_Grade +'|'+ ";
                        break;
                    case "ZIPCODE":
                        rString += "Ttr_WorkLocationCode +'|'+ ";
                        break;
                    case "PREMGRP":
                        rString += "Ttr_PremiumGrpCode +'|'+ ";
                        break;
                    case "CALGRP":
                        rString += "Ttr_CalendarGroup +'|'+ ";
                        break;
                    case "CCTR":
                        rString += "Ttr_CostcenterCode +'|'+ ";
                        break;
                    case "SHIFT":
                        rString += "Ttr_ShiftCode +'|'+ ";
                        break;
                    case "SCHED":
                        rString += "Ttr_ScheduleType +'|'+ ";
                        break;
                    case "NONE":
                        rString += "";
                        break;
                }
            }

            if (rString != "")
                rString = rString.Substring(0, rString.Length - 6);

            return rString;
        }

    }
}
