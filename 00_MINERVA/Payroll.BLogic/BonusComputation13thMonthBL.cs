using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Threading;
using System.Collections;

namespace Payroll.BLogic
{
    public class BonusComputation13thMonthBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL SystemCycleProcessingBL;
        CommonBL commonBL;

        string LoginUser        = "";
        string CompanyCode      = "";
        string CentralProfile   = "";
        public string MenuCode  = "";

        decimal M13EXCLTAX;

        DataTable dtBonusMaster;
        DataTable dtEmployeeForProcess = null;

        #endregion

        #region Overrides
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

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Event handlers for bonus processing
        public delegate void EmpDispEventHandler(object sender, EmpDispEventArgs e);
        public class EmpDispEventArgs : System.EventArgs
        {
            public string EmployeeId;
            public string LastName;
            public string FirstName;
            public string StatusMsg;

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                StatusMsg = "Successful";
            }

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst, string strStatusMsg)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                StatusMsg = strStatusMsg;
            }
        }
        public event EmpDispEventHandler EmpDispHandler;

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
        #endregion

        public int InsertHdr(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[29];
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_ControlNo", row["Tbh_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_PayCycle", row["Tbh_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BonusCode", row["Tbh_BonusCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_StartCoverage", row["Tbh_StartCoverage"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_EndCoverage", row["Tbh_EndCoverage"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_Mode", row["Tbh_Mode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseAmt1Formula", row["Tbh_BaseAmt1Formula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseAmt2Formula", row["Tbh_BaseAmt2Formula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseNumFormula", row["Tbh_BaseNumFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseCharFormula", row["Tbh_BaseCharFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseSalaryFormula", row["Tbh_BaseSalaryFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BasePayrollTypeFormula", row["Tbh_BasePayrollTypeFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BonusFormula", row["Tbh_BonusFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_NontaxableCode", row["Tbh_NontaxableCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_TaxableCode", row["Tbh_TaxableCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_IDNoLIst", row["Tbh_IDNoLIst"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_CostCenterList", row["Tbh_CostCenterList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_PayrollGroupList", row["Tbh_PayrollGroupList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_EmploymentStatusList", row["Tbh_EmploymentStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_PayrollTypeList", row["Tbh_PayrollTypeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_WorkStatusList", row["Tbh_WorkStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_GradeList", row["Tbh_GradeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_HireRegOption", row["Tbh_HireRegOption"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_HireRegDate", (GetValue(row["Tbh_HireRegDate"]) != string.Empty ? row["Tbh_HireRegDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BonusCheckFormula", row["Tbh_BonusCheckFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_OverrideBonusCeiling", row["Tbh_OverrideBonusCeiling"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_TaxCompensationLevel", row["Tbh_TaxCompensationLevel"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_RecordStatus", row["Tbh_RecordStatus"]);


            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpBonusHdr
                                            (Tbh_ControlNo
                                            ,Tbh_PayCycle
                                            ,Tbh_BonusCode
                                            ,Tbh_StartCoverage
                                            ,Tbh_EndCoverage
                                            ,Tbh_Mode
                                            ,Tbh_BaseAmt1Formula
                                            ,Tbh_BaseAmt2Formula
                                            ,Tbh_BaseNumFormula
                                            ,Tbh_BaseCharFormula
                                            ,Tbh_BaseSalaryFormula
                                            ,Tbh_BasePayrollTypeFormula
                                            ,Tbh_BonusFormula
                                            ,Tbh_BonusCheckFormula
                                            ,Tbh_NontaxableCode
                                            ,Tbh_TaxableCode
                                            ,Tbh_IDNoLIst
                                            ,Tbh_CostCenterList
                                            ,Tbh_PayrollGroupList
                                            ,Tbh_EmploymentStatusList
                                            ,Tbh_PayrollTypeList
                                            ,Tbh_WorkStatusList
                                            ,Tbh_GradeList
                                            ,Tbh_HireRegOption
                                            ,Tbh_HireRegDate
                                            ,Tbh_OverrideBonusCeiling
                                            ,Tbh_TaxCompensationLevel
                                            ,Tbh_RecordStatus
                                            ,Usr_Login
                                            ,Ludatetime)

                                            VALUES(@Tbh_ControlNo
                                            , @Tbh_PayCycle
                                            , @Tbh_BonusCode
                                            , @Tbh_StartCoverage
                                            , @Tbh_EndCoverage
                                            , @Tbh_Mode
                                            , @Tbh_BaseAmt1Formula
                                            , @Tbh_BaseAmt2Formula
                                            , @Tbh_BaseNumFormula
                                            , @Tbh_BaseCharFormula
                                            , @Tbh_BaseSalaryFormula
                                            , @Tbh_BasePayrollTypeFormula
                                            , @Tbh_BonusFormula
                                            , @Tbh_BonusCheckFormula
                                            , @Tbh_NontaxableCode
                                            , @Tbh_TaxableCode
                                            , @Tbh_IDNoLIst
                                            , @Tbh_CostCenterList
                                            , @Tbh_PayrollGroupList
                                            , @Tbh_EmploymentStatusList
                                            , @Tbh_PayrollTypeList
                                            , @Tbh_WorkStatusList
                                            , @Tbh_GradeList
                                            , @Tbh_HireRegOption
                                            , @Tbh_HireRegDate
                                            , @Tbh_OverrideBonusCeiling
                                            , @Tbh_TaxCompensationLevel
                                            , @Tbh_RecordStatus
                                            , @Usr_Login
                                            , GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertDtl(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[21];
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_ControlNo", row["Tbd_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_LineNo", row["Tbd_LineNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_IDNo", row["Tbd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_SalaryRate", row["Tbd_SalaryRate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_PayrollType", row["Tbd_PayrollType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BaseAmt1", row["Tbd_BaseAmt1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BaseAmt2", row["Tbd_BaseAmt2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_TotalBonusAmt", row["Tbd_TotalBonusAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_AccumBonusBeforeThisCycle", row["Tbd_AccumBonusBeforeThisCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BonusNontaxableAmt", row["Tbd_BonusNontaxableAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BonusTaxableAmt", row["Tbd_BonusTaxableAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_PostFlag", row["Tbd_PostFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_CostCenter", row["Tbd_CostCenter"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_PayrollGroup", row["Tbd_PayrollGroup"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_EmploymentStatus", row["Tbd_EmploymentStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_WorkStatus", row["Tbd_WorkStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_Grade", row["Tbd_Grade"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BaseChar", row["Tbd_BaseChar"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BaseNum", row["Tbd_BaseNum"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_RetainUserEntry", row["Tbd_RetainUserEntry"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpBonusDtl
                                            (Tbd_ControlNo
                                            ,Tbd_LineNo
                                            ,Tbd_IDNo
                                            ,Tbd_SalaryRate
                                            ,Tbd_PayrollType
                                            ,Tbd_BaseAmt1
                                            ,Tbd_BaseAmt2
                                            ,Tbd_TotalBonusAmt
                                            ,Tbd_AccumBonusBeforeThisCycle
                                            ,Tbd_BonusNontaxableAmt
                                            ,Tbd_BonusTaxableAmt
                                            ,Tbd_PostFlag
                                            ,Tbd_BaseChar
                                            ,Tbd_BaseNum
                                            ,Tbd_CostCenter
                                            ,Tbd_PayrollGroup
                                            ,Tbd_EmploymentStatus
                                            ,Tbd_WorkStatus
                                            ,Tbd_Grade
                                            ,Tbd_RetainUserEntry
                                            ,Usr_Login
                                            ,Ludatetime)

                                            VALUES(@Tbd_ControlNo
                                            ,@Tbd_LineNo
                                            ,@Tbd_IDNo
                                            ,@Tbd_SalaryRate
                                            ,@Tbd_PayrollType
                                            ,@Tbd_BaseAmt1
                                            ,@Tbd_BaseAmt2
                                            ,@Tbd_TotalBonusAmt
                                            ,@Tbd_AccumBonusBeforeThisCycle
                                            ,@Tbd_BonusNontaxableAmt
                                            ,@Tbd_BonusTaxableAmt
                                            ,@Tbd_PostFlag
                                            ,@Tbd_BaseChar
                                            ,@Tbd_BaseNum
                                            ,@Tbd_CostCenter
                                            ,@Tbd_PayrollGroup
                                            ,@Tbd_EmploymentStatus
                                            ,@Tbd_WorkStatus
                                            ,@Tbd_Grade
                                            ,@Tbd_RetainUserEntry
                                            ,@Usr_Login
                                            ,GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int UpdateHdr(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[27];
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_ControlNo", row["Tbh_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_PayCycle", row["Tbh_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BonusCode", row["Tbh_BonusCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_StartCoverage", row["Tbh_StartCoverage"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_EndCoverage", row["Tbh_EndCoverage"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseAmt1Formula", row["Tbh_BaseAmt1Formula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseAmt2Formula", row["Tbh_BaseAmt2Formula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseNumFormula", row["Tbh_BaseNumFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseCharFormula", row["Tbh_BaseCharFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BaseSalaryFormula", row["Tbh_BaseSalaryFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BasePayrollTypeFormula", row["Tbh_BasePayrollTypeFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_BonusCheckFormula", row["Tbh_BonusCheckFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_NontaxableCode", row["Tbh_NontaxableCode"], SqlDbType.VarChar, 10);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_TaxableCode", row["Tbh_TaxableCode"], SqlDbType.VarChar, 10);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_IDNoLIst", row["Tbh_IDNoLIst"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_CostCenterList", row["Tbh_CostCenterList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_PayrollGroupList", row["Tbh_PayrollGroupList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_EmploymentStatusList", row["Tbh_EmploymentStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_PayrollTypeList", row["Tbh_PayrollTypeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_WorkStatusList", row["Tbh_WorkStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_GradeList", row["Tbh_GradeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_HireRegOption", row["Tbh_HireRegOption"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_HireRegDate", (GetValue(row["Tbh_HireRegDate"]) != string.Empty ? row["Tbh_HireRegDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_OverrideBonusCeiling", row["Tbh_OverrideBonusCeiling"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_TaxCompensationLevel", row["Tbh_TaxCompensationLevel"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbh_RecordStatus", row["Tbh_RecordStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Update Query
            string sqlUpdate = @"UPDATE T_EmpBonusHdr
                                SET Tbh_StartCoverage = @Tbh_StartCoverage
                                , Tbh_EndCoverage = @Tbh_EndCoverage
                                , Tbh_BaseAmt1Formula = @Tbh_BaseAmt1Formula
                                , Tbh_BaseAmt2Formula = @Tbh_BaseAmt2Formula
                                , Tbh_BaseNumFormula = @Tbh_BaseNumFormula
                                , Tbh_BaseCharFormula = @Tbh_BaseCharFormula
                                , Tbh_BaseSalaryFormula = @Tbh_BaseSalaryFormula
                                , Tbh_BasePayrollTypeFormula = @Tbh_BasePayrollTypeFormula
                                , Tbh_BonusCheckFormula = @Tbh_BonusCheckFormula
                                , Tbh_NontaxableCode = @Tbh_NontaxableCode
                                , Tbh_TaxableCode = @Tbh_TaxableCode
                                , Tbh_IDNoLIst = @Tbh_IDNoLIst
                                , Tbh_CostCenterList = @Tbh_CostCenterList
                                , Tbh_PayrollGroupList = @Tbh_PayrollGroupList
                                , Tbh_EmploymentStatusList = @Tbh_EmploymentStatusList
                                , Tbh_PayrollTypeList = @Tbh_PayrollTypeList
                                , Tbh_WorkStatusList = @Tbh_WorkStatusList
                                , Tbh_GradeList = @Tbh_GradeList
                                , Tbh_HireRegOption = @Tbh_HireRegOption
                                , Tbh_HireRegDate = @Tbh_HireRegDate
                                , Tbh_OverrideBonusCeiling = @Tbh_OverrideBonusCeiling
                                , Tbh_TaxCompensationLevel = @Tbh_TaxCompensationLevel
                                , Tbh_RecordStatus = @Tbh_RecordStatus
                                , Usr_Login = @Usr_Login
                                , Ludatetime = GETDATE()
                                WHERE Tbh_ControlNo = @Tbh_ControlNo
                                AND Tbh_PayCycle = @Tbh_PayCycle
                                AND Tbh_BonusCode = @Tbh_BonusCode";
            #endregion

            try
            {
                retVal = dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int UpdateDtl(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[10];
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_ControlNo", row["Tbd_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_LineNo", row["Tbd_LineNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_IDNo", row["Tbd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_TotalBonusAmt", row["Tbd_TotalBonusAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BonusNontaxableAmt", row["Tbd_BonusNontaxableAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_BonusTaxableAmt", row["Tbd_BonusTaxableAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_AccumBonusBeforeThisCycle", row["Tbd_AccumBonusBeforeThisCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_PostFlag", false, SqlDbType.Bit);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbd_RetainUserEntry", row["Tbd_RetainUserEntry"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Update Query
            string sqlUpdate = @"
                                DECLARE @LineNo INT = 999999 
                                IF NOT EXISTS (SELECT Tbd_LineNo FROM T_EmpBonusDtl
			                                   WHERE Tbd_ControlNo = @Tbd_ControlNo
				                                    AND Tbd_RetainUserEntry = 1
				                                    AND Tbd_IDNo = @Tbd_IDNo)
                                BEGIN 
                                IF EXISTS (SELECT COUNT(Tbd_LineNo) 
                                           FROM T_EmpBonusDtl
			                               WHERE Tbd_ControlNo = @Tbd_ControlNo
				                                AND Tbd_RetainUserEntry = 1
			                               HAVING COUNT(Tbd_LineNo) > 0)
                                    SET @LineNo = (SELECT MIN(Tbd_LineNo) - 1
                                                   FROM T_EmpBonusDtl
												   WHERE Tbd_ControlNo = @Tbd_ControlNo
												       AND Tbd_RetainUserEntry = 1)
                                END
                                ELSE
	                                 SET @LineNo = @Tbd_LineNo                       

                                UPDATE T_EmpBonusDtl
                                SET Tbd_LineNo = @LineNo
                                    , Tbd_TotalBonusAmt = @Tbd_TotalBonusAmt
                                    , Tbd_BonusNontaxableAmt = @Tbd_BonusNontaxableAmt
                                    , Tbd_BonusTaxableAmt = @Tbd_BonusTaxableAmt
                                    , Tbd_AccumBonusBeforeThisCycle = @Tbd_AccumBonusBeforeThisCycle
                                    , Tbd_RetainUserEntry = @Tbd_RetainUserEntry
                                    , Tbd_PostFlag = @Tbd_PostFlag
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = GETDATE()
                                WHERE Tbd_ControlNo = @Tbd_ControlNo
                                    AND Tbd_LineNo = @Tbd_LineNo
                                    AND Tbd_IDNo = @Tbd_IDNo";
            #endregion

            try
            {
                retVal = dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertBonusFactor(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[paramIndex++] = new ParameterInfo("@Tbf_IDNo", row["Tbf_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbf_Year", row["Tbf_Year"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tbf_BonusRating", row["Tbf_BonusRating"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Insert Query
            string query = @"IF EXISTS (SELECT Tbf_IDNo FROM T_BonusFactor WHERE Tbf_IDNo = @Tbf_IDNo AND Tbf_Year = @Tbf_Year)
                                    DELETE FROM T_BonusFactor WHERE Tbf_IDNo = @Tbf_IDNo AND Tbf_Year = @Tbf_Year

                                    INSERT INTO T_BonusFactor
                                            (Tbf_IDNo
                                            ,Tbf_Year
                                            ,Tbf_BonusRating
                                            ,Usr_Login
                                            ,Ludatetime)
                                    VALUES(@Tbf_IDNo
                                            ,@Tbf_Year
                                            ,@Tbf_BonusRating
                                            ,@Usr_Login
                                            ,GETDATE())
                                 ";
            #endregion

            try
            {
                retVal = dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            return retVal;
        }

        public int PostToIncome(bool ProcessSeparated, string IDNumberList, string PayCycleCode, string ControlNo, string bNontaxCode, string bTaxCode, string userLogin, string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            int retVal = 0;
            string Condition = string.Empty;
            string tableIncome = CommonConstants.TableName.T_EmpIncome;
            if (ProcessSeparated)
            {
                tableIncome = CommonConstants.TableName.T_EmpIncomeFinalPay;
                Condition = string.Format("AND [@FIELDNAME] = '{0}'", IDNumberList);
            }

            #region Query
            string query = string.Format(@"IF NOT EXISTS(SELECT * FROM {6}..M_Income WHERE Min_IncomeCode = '{2}' AND Min_CompanyCode = '{5}')
                                                    BEGIN
	                                                    INSERT INTO {6}..M_Income
                                                               (Min_CompanyCode
                                                               ,Min_IncomeCode
                                                               ,Min_IncomeName
                                                               ,Min_TaxClass
                                                               ,Min_IsRecurring
															   ,Min_ApplicablePayCycle
															   ,Min_IncomeGroup
															   ,Min_AlphalistCategory
															   ,Min_IsSystemReserved
                                                               ,Min_AccountGrp
															   ,Min_RemittanceLoanType
                                                               ,Min_RecordStatus
                                                               ,Min_CreatedBy
                                                               ,Min_CreatedDate)
                                                         VALUES
                                                               ('{5}'
                                                               ,'{2}'
                                                               ,'{2} NONTAXABLE'
                                                               ,'N'
                                                               ,0
                                                               ,0
                                                               ,'BON'
                                                               ,'N-13THBEN'
                                                               , 1
                                                               ,''
                                                               ,''
															   ,'A'
                                                               ,'{4}'
                                                               ,GETDATE())
                                                    END

                                                    IF NOT EXISTS(SELECT * FROM {6}..M_Income WHERE Min_IncomeCode = '{3}' AND Min_CompanyCode = '{5}')
                                                    BEGIN
	                                                    INSERT INTO {6}..M_Income
                                                               (Min_CompanyCode
                                                               ,Min_IncomeCode
                                                               ,Min_IncomeName
                                                               ,Min_TaxClass
                                                               ,Min_IsRecurring
															   ,Min_ApplicablePayCycle
															   ,Min_IncomeGroup
															   ,Min_AlphalistCategory
															   ,Min_IsSystemReserved
                                                               ,Min_AccountGrp
															   ,Min_RemittanceLoanType
                                                               ,Min_RecordStatus
                                                               ,Min_CreatedBy
                                                               ,Min_CreatedDate)
                                                         VALUES
                                                               ('{5}'
                                                               ,'{3}'
                                                               ,'{3} TAXABLE'
                                                               ,'T'
                                                               ,0
                                                               ,0
                                                               ,'BON'
                                                               ,'T-13THBEN'
                                                               , 1
                                                               ,''
                                                               ,''
															   ,'A'
                                                               ,'{4}'
                                                               ,GETDATE())
                                                    END

                                                    DELETE FROM {7}
                                                    WHERE Tin_IncomeCode IN ('{2}', '{3}')
                                                        AND Tin_PayCycle = '{0}'
                                                        AND Tin_OrigPayCycle = '{0}'
                                                        {9}

                                                    INSERT INTO {7}
                                                               (Tin_IDNo
                                                               ,Tin_PayCycle
                                                               ,Tin_OrigPayCycle
                                                               ,Tin_IncomeCode
                                                               ,Tin_PostFlag
                                                               ,Tin_IncomeAmt
                                                               ,Usr_Login
                                                               ,Ludatetime)
                                                    SELECT Tbd_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'{2}'
                                                          ,0
                                                          ,Tbd_BonusNontaxableAmt
                                                          ,'{4}'
                                                          ,GETDATE()
                                                      FROM T_EmpBonusDtl
                                                      WHERE Tbd_BonusNontaxableAmt > 0
                                                        AND Tbd_ControlNo = '{1}'
                                                        {8}

                                                    INSERT INTO {7}
                                                               (Tin_IDNo
                                                               ,Tin_PayCycle
                                                               ,Tin_OrigPayCycle
                                                               ,Tin_IncomeCode
                                                               ,Tin_PostFlag
                                                               ,Tin_IncomeAmt
                                                               ,Usr_Login
                                                               ,Ludatetime)
                                                    SELECT Tbd_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'{3}'
                                                          ,0
                                                          ,Tbd_BonusTaxableAmt
                                                          ,'{4}'
                                                          ,GETDATE()
                                                      FROM T_EmpBonusDtl
                                                      WHERE Tbd_BonusTaxableAmt > 0
                                                        AND Tbd_ControlNo = '{1}'
                                                        {8}
                                                      
                                                      UPDATE T_EmpBonusDtl
                                                      SET Tbd_PostFlag = 1
                                                        , Usr_login = '{4}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE (Tbd_BonusNontaxableAmt > 0 OR Tbd_BonusTaxableAmt > 0)
                                                        AND Tbd_ControlNo = '{1}'
                                                        {8}

                                                      UPDATE T_EmpBonusDtl
                                                      SET Tbd_PostFlag = 0
                                                        , Usr_login = '{4}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE (Tbd_BonusNontaxableAmt <= 0 AND Tbd_BonusTaxableAmt <= 0)
                                                        AND Tbd_ControlNo = '{1}'
                                                        {8}

                                                   ", PayCycleCode
                                                        , ControlNo
                                                        , bNontaxCode
                                                        , bTaxCode
                                                        , userLogin
                                                        , CompanyCode
                                                        , CentralProfile
                                                        , tableIncome
                                                        , Condition.Replace("@FIELDNAME", "Tbd_IDNo")
                                                        , Condition.Replace("@FIELDNAME", "Tin_IDNo"));
            #endregion

            try
            {
                retVal = dalhelper.ExecuteNonQuery(query);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int UpdateNonTaxAndTaxAmt(string IDNumber, string ControlNo, string AccumBonus, decimal M13EXCLTAX, DALHelper dal)
        {
            int retVal = 0;

            #region Update Query
            string queryNonTaxAmt = string.Format(@"
                                        DECLARE @M13EXCLTAX AS decimal(9,2)
                                        SET @M13EXCLTAX = {3}

                                        UPDATE T_EmpBonusDtl
                                          SET Tbd_AccumBonusBeforeThisCycle = {2}
                                              , Tbd_BonusNontaxableAmt = CASE WHEN Tbd_TotalBonusAmt - CASE WHEN Tbd_TotalBonusAmt - (@M13EXCLTAX - {2}) < 0
											                                                THEN 0
											                                                ELSE Tbd_TotalBonusAmt - (@M13EXCLTAX - {2})
											                                                END < 0
					                                                THEN 0
					                                                ELSE Tbd_TotalBonusAmt - CASE WHEN Tbd_TotalBonusAmt - (@M13EXCLTAX - {2}) < 0
											                                                THEN 0
											                                                ELSE Tbd_TotalBonusAmt - (@M13EXCLTAX - {2})
											                                                END
					                                                END
                                              , Tbd_BonusTaxableAmt = CASE WHEN ( CASE WHEN (Tbd_TotalBonusAmt + {2}) - @M13EXCLTAX < 0
								                                                THEN 0
								                                                ELSE (Tbd_TotalBonusAmt + {2}) - @M13EXCLTAX
								                                                END
							                                                - CASE WHEN {2} - @M13EXCLTAX < 0
								                                                THEN 0
								                                                ELSE {2} - @M13EXCLTAX
								                                                END ) < 0
				                                                THEN 0
				                                                ELSE ( CASE WHEN (Tbd_TotalBonusAmt + {2}) - @M13EXCLTAX < 0
							                                                THEN 0
							                                                ELSE (Tbd_TotalBonusAmt + {2}) - @M13EXCLTAX
							                                                END
						                                                - CASE WHEN {2} - @M13EXCLTAX < 0
							                                                THEN 0
							                                                ELSE {2} - @M13EXCLTAX
							                                                END )
				                                                END
                                          WHERE Tbd_IDNo = '{0}'
                                            AND Tbd_ControlNo = '{1}'", IDNumber
                                                                      , ControlNo
                                                                      , AccumBonus
                                                                      , M13EXCLTAX);
            #endregion

            try
            {
                retVal = dal.ExecuteNonQuery(queryNonTaxAmt);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public DataSet GetLastSeqNo(string IDNumber, string ControlNumber, DALHelper dal)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@ControlNumber", ControlNumber);
            paramInfo[1] = new ParameterInfo("@IDNumber", IDNumber);
            
            string sqlQuery = @"SELECT MAX(CONVERT(INT, Tbd_LineNo)) AS NewSeqNo
                                              FROM T_EmpBonusDtl
                                              WHERE Tbd_ControlNo = @ControlNumber
                                              AND Tbd_IDNo = @IDNumber";

            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public int GetLastSeqNo(string ControlNumber, DALHelper dal)
        {
            try
            {
                DataTable dt = new DataTable();
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ControlNumber", ControlNumber);

                string sqlQuery = @"SELECT MAX(CONVERT(INT, Tbd_LineNo)) AS [Count] 
                                    FROM T_EmpBonusDtl
                                    WHERE Tbd_ControlNo = @ControlNumber";

                dt = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch
            { return 0; }

        }

        public DataTable GetCostCenters(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcc_CostCenterCode as 'Cost Center Code'
                                                , dbo.Udf_DisplayCostCenterName('{0}', Mcc_CostCenterCode,'{1}') as 'Description'
                                            FROM M_CostCenter
                                            WHERE Mcc_RecordStatus = 'A'
                                                AND Mcc_CompanyCode = '{0}'"
                                            , CompanyCode, (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeMasterList()
        {
            #region query
            string query = string.Format(@"SELECT Mem_IDNo, Mem_WorkStatus
                                           FROM M_Employee");
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetAccountCode(string AccountType, string strMenuCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcd_Code AS 'Code'
                                               , Mcd_Name AS 'Description'
                                          FROM {2}..M_CodeDtl
                                          WHERE Mcd_RecordStatus = 'A'
                                                AND Mcd_CodeType = '{0}' 
                                                AND Mcd_CompanyCode = '{1}'
                                                AND Mcd_Code IN (SELECT Mpd_SubCode 
                                                                        FROM M_PolicyDtl 
				                                                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				                                                        AND Mpd_ParamValue = 1)", AccountType, CompanyCode, CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public double GetAccumulatedBonus(string IDNumber, string PayCycle, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            try
            {
                double dValue = 0;
                #region query
                string query = string.Format(@"SELECT ISNULL(SUM(Tin_IncomeAmt),0) AS Amt
                                            FROM T_EmpIncomeHst
                                            INNER JOIN {3}..M_Income ON Min_IncomeCode = Tin_IncomeCode
                                                AND Min_AlphalistCategory IN ('N-13THBEN','T-13THBEN')
                                                AND  Min_CompanyCode = '{2}'
                                            WHERE LEFT(Tin_PayCycle, 4) = LEFT('{1}',4)
                                                AND Tin_IDNo = '{0}'
                                                AND  Tin_PostFlag = 1", IDNumber, PayCycle, CompanyCode, CentralProfile);
                #endregion
                DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
                if (dtResult.Rows.Count > 0)
                {
                    dValue = Convert.ToDouble(dtResult.Rows[0][0]);
                }
                return dValue;
            }
            catch
            {
                return 0;
            }
        }

        public double GetFormulaQueryDoubleValue(string query, ParameterInfo[] paramInfo, DALHelper dal)
        {
            try
            {
                double dValue = 0;
                DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                if (dtResult.Rows.Count > 0)
                {
                    dValue = Convert.ToDouble(dtResult.Rows[0][0]);
                }
                return dValue;
            }
            catch
            {
                return 0;
            }
        }

        public decimal GetFormulaQueryDecimalValue(string query, ParameterInfo[] paramInfo, DALHelper dal)
        {
            decimal dValue = 0;
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                dValue = Convert.ToDecimal(dtResult.Rows[0][0]);
            }
            return dValue;
        }

        public string GetFormulaQueryStringValue(string query, ParameterInfo[] paramInfo, DALHelper dal)
        {
            if (query == string.Empty)
                return string.Empty;

            string sValue = string.Empty;
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                sValue = GetValue(dtResult.Rows[0][0]);
            }
            return sValue;
        }

        public DataTable GetFormulaQueryDataTable(string query, ParameterInfo[] paramInfo, DALHelper dal)
        {
            if (query == string.Empty)
                return null;

            string sValue = string.Empty;
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeeForProcess(string conditions, object HireRegDate, bool Recompute, string ControlNumber)
        {
            string RecomputeCondition = string.Format(@"
                                                AND Mem_IDNo NOT IN (SELECT Tbd_IDNo FROM T_EmpBonusDtl 
                                                WHERE Tbd_RetainUserEntry = 1
											    AND Tbd_ControlNo = '{0}')", ControlNumber);

            string query = string.Format(@"DECLARE @HireRegDate DATE = '{2}'

                                            SELECT Mem_IDNo
                                                                    , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
													                    THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' 
		                                                                + Mem_FirstName + ' '
		                                                                + Mem_MiddleName [Employee Name]
                                                                    , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
													                    THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                                                    , Mem_FirstName
                                                                    , Mem_MiddleName
                                                                    , Mem_CostcenterCode
                                                                    , Mem_PayrollType
																    , Mem_PayrollGroup
                                                                    , Mem_EmploymentStatusCode
																    , Mem_PositionGrade
                                                                    , Mem_WorkStatus
                                                        FROM M_Employee 
                                                        {0}
                                                        {1}", conditions, (!Recompute ? "" : RecomputeCondition), HireRegDate);

            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public string ComputeBonus(bool Recompute, string PayrollPeriod, string EmployeeID, string UserLogin, string companyCode, string centralProfile, string profileCode, string strControlNumber, string conditions, string BonusCode, object bStartCoverage, object bEndCoverage, DALHelper dalHelper)
        {
            return ComputeBonus(true, Recompute, PayrollPeriod, EmployeeID, UserLogin, companyCode, centralProfile, profileCode, "", "", "", "", "", "", "", "", "", strControlNumber, conditions, BonusCode, bStartCoverage, bEndCoverage, false, "", dalHelper);
        }

        public string ComputeBonus(
            bool ProcessSeparated
            , bool Recompute
            , string PayrollPeriod
            , string EmployeeID
            , string UserLogin
            , string companyCode
            , string centralProfile
            , string profileCode
            , string IDNoLIst
            , string CostCenterList
            , string PayrollGroupList
            , string EmploymentStatusList
            , string PayrollTypeList
            , string WorkStatusList
            , string GradeList
            , string HireRegOption
            , object HireRegDate
            , string strControlNumber
            , string conditions
            , string BonusCode
            , object bStartCoverage
            , object bEndCoverage
            , bool bOverrideBonusCeiling
            , string TaxScheduleCode
            , DALHelper dalHelper)
        {

            #region Variables
            string ControlNumber = string.Empty;
            int ctr = 1;
            #endregion

            try
            {
                #region Initial Setup
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, PayrollPeriod, UserLogin, companyCode, centralProfile);
                commonBL        = new CommonBL();
                LoginUser       = UserLogin;
                CompanyCode     = companyCode;
                CentralProfile  = centralProfile;
                ControlNumber   = strControlNumber;
                
                SetProcessFlags();
                #endregion

                if (PayrollPeriod == string.Empty)
                    throw new PayrollException("No Pay Cycle Code");

                #region 13th Month

                dtBonusMaster = GetBonusMasterDetails(BonusCode);
                if (dtBonusMaster.Rows.Count > 0)
                {
                    string BaseAmt1Formula          = dtBonusMaster.Rows[0]["Mbn_BaseAmt1Formula"].ToString().Trim();
                    string BaseAmt2Formula          = dtBonusMaster.Rows[0]["Mbn_BaseAmt2Formula"].ToString().Trim();
                    string BaseNumFormula           = dtBonusMaster.Rows[0]["Mbn_BaseNumFormula"].ToString().Trim();
                    string BaseCharFormula          = dtBonusMaster.Rows[0]["Mbn_BaseCharFormula"].ToString().Trim();
                    string BaseSalaryFormula        = dtBonusMaster.Rows[0]["Mbn_BaseSalaryFormula"].ToString().Trim();
                    string BasePayrollTypeFormula   = dtBonusMaster.Rows[0]["Mbn_BasePayrollTypeFormula"].ToString().Trim();
                    string BonusFormula             = dtBonusMaster.Rows[0]["Mbn_BonusFormula"].ToString().Trim();
                    string BonusCheckFormula        = GetValue(dtBonusMaster.Rows[0]["Mbn_BonusCheckFormula"].ToString().Trim());
                    string BonusNontaxableCode      = dtBonusMaster.Rows[0]["Mbn_NontaxableCode"].ToString().Trim();
                    string BonusTaxableCode         = dtBonusMaster.Rows[0]["Mbn_TaxableCode"].ToString().Trim();
                    decimal dTaxCompensationLevel   = 0;

                    if  (bOverrideBonusCeiling)
                        dTaxCompensationLevel = GetTaxCompensationLevel(PayrollPeriod, TaxScheduleCode);

                    if (ControlNumber == "")
                        ControlNumber = commonBL.GetDocumentNumber(dal, new DALHelper(CentralProfile, false), "BONUS", profileCode);

                    dtEmployeeForProcess = GetAllEmployeeForProcess(conditions, HireRegDate, Recompute, ControlNumber);
                    if (dtEmployeeForProcess.Rows.Count > 0)
                    {
                        if (!Recompute)
                        {
                            #region Create Bonus Hdr Records
                            DataRow drEmpBonusHdr = DbRecord.Generate(CommonConstants.TableName.T_EmpBonusHdr);
                            drEmpBonusHdr["Tbh_ControlNo"]              = ControlNumber;
                            drEmpBonusHdr["Tbh_PayCycle"]               = PayrollPeriod;
                            drEmpBonusHdr["Tbh_BonusCode"]              = BonusCode;
                            drEmpBonusHdr["Tbh_StartCoverage"]          = "";
                            drEmpBonusHdr["Tbh_EndCoverage"]            = "";
                            if (!ProcessSeparated)
                            {
                                drEmpBonusHdr["Tbh_StartCoverage"]      = bStartCoverage;
                                drEmpBonusHdr["Tbh_EndCoverage"]        = bEndCoverage;
                            }
                            drEmpBonusHdr["Tbh_Mode"]                   = "C";
                            drEmpBonusHdr["Tbh_BaseAmt1Formula"]        = BaseAmt1Formula;
                            drEmpBonusHdr["Tbh_BaseAmt2Formula"]        = BaseAmt2Formula;
                            drEmpBonusHdr["Tbh_BaseNumFormula"]         = BaseNumFormula;
                            drEmpBonusHdr["Tbh_BaseCharFormula"]        = BaseCharFormula;
                            drEmpBonusHdr["Tbh_BaseSalaryFormula"]      = BaseSalaryFormula;
                            drEmpBonusHdr["Tbh_BasePayrollTypeFormula"] = BasePayrollTypeFormula;
                            drEmpBonusHdr["Tbh_BonusFormula"]           = BonusFormula;
                            drEmpBonusHdr["Tbh_BonusCheckFormula"]      = BonusCheckFormula;
                            drEmpBonusHdr["Tbh_NontaxableCode"]         = BonusNontaxableCode;
                            drEmpBonusHdr["Tbh_TaxableCode"]            = BonusTaxableCode;
                            drEmpBonusHdr["Tbh_IDNoLIst"]               = IDNoLIst;
                            drEmpBonusHdr["Tbh_CostCenterList"]         = CostCenterList;
                            drEmpBonusHdr["Tbh_PayrollGroupList"]       = PayrollGroupList;
                            drEmpBonusHdr["Tbh_EmploymentStatusList"]   = EmploymentStatusList;
                            drEmpBonusHdr["Tbh_PayrollTypeList"]        = PayrollTypeList;
                            drEmpBonusHdr["Tbh_WorkStatusList"]         = WorkStatusList;
                            drEmpBonusHdr["Tbh_GradeList"]              = GradeList;
                            drEmpBonusHdr["Tbh_HireRegOption"]          = HireRegOption;
                            if (GetValue(HireRegDate) != string.Empty)
                                drEmpBonusHdr["Tbh_HireRegDate"] = Convert.ToDateTime(HireRegDate);
                            else
                                drEmpBonusHdr["Tbh_HireRegDate"]        = DBNull.Value;
                            drEmpBonusHdr["Tbh_OverrideBonusCeiling"]   = bOverrideBonusCeiling;
                            drEmpBonusHdr["Tbh_TaxCompensationLevel"]   = dTaxCompensationLevel;
                            drEmpBonusHdr["Tbh_RecordStatus"]           = "A";
                            drEmpBonusHdr["Usr_Login"]                  = LoginUser;

                            InsertHdr(drEmpBonusHdr, dal);

                            #endregion
                        }

                        else if (Recompute)
                        {
                            #region Delete in Income
                            string tableIncome = CommonConstants.TableName.T_EmpIncome;
                            string Condition = string.Empty;
                            if (ProcessSeparated)
                            {
                                tableIncome = CommonConstants.TableName.T_EmpIncomeFinalPay;
                                Condition = string.Format("AND Tin_IDNo IN ({0})", EmployeeID);
                            }
                            string queryDelete = string.Format(@"DELETE FROM {3}
                                                                 WHERE Tin_IncomeCode IN ('{1}', '{2}')
                                                                    AND Tin_PayCycle = '{0}'
                                                                    AND Tin_OrigPayCycle = '{0}'
                                                                    {4}", PayrollPeriod
                                                                        , BonusTaxableCode
                                                                        , BonusNontaxableCode
                                                                        , tableIncome
                                                                        , Condition);
                            dal.ExecuteNonQuery(queryDelete);
                            #endregion

                            #region Update Bonus Hdr Records
                            DataRow drHdr = DbRecord.Generate(CommonConstants.TableName.T_EmpBonusHdr);
                            #region Assign to row
                            drHdr["Tbh_ControlNo"]                  = ControlNumber;
                            drHdr["Tbh_PayCycle"]                   = PayrollPeriod;
                            drHdr["Tbh_BonusCode"]                  = BonusCode;
                            drHdr["Tbh_StartCoverage"]              = "";
                            drHdr["Tbh_EndCoverage"]                = "";
                            if (!ProcessSeparated)
                            {
                                drHdr["Tbh_StartCoverage"]          = bStartCoverage;
                                drHdr["Tbh_EndCoverage"]            = bEndCoverage;
                            }    
                            drHdr["Tbh_BaseAmt1Formula"]            = BaseAmt1Formula;
                            drHdr["Tbh_BaseAmt2Formula"]            = BaseAmt2Formula;
                            drHdr["Tbh_BaseNumFormula"]             = BaseNumFormula;
                            drHdr["Tbh_BaseCharFormula"]            = BaseCharFormula;
                            drHdr["Tbh_BaseSalaryFormula"]          = BaseSalaryFormula;
                            drHdr["Tbh_BasePayrollTypeFormula"]     = BasePayrollTypeFormula;
                            drHdr["Tbh_BonusFormula"]               = BonusFormula;
                            drHdr["Tbh_BonusCheckFormula"]          = BonusCheckFormula;
                            drHdr["Tbh_NontaxableCode"]             = BonusNontaxableCode;
                            drHdr["Tbh_TaxableCode"]                = BonusTaxableCode;
                            drHdr["Tbh_IDNoLIst"]                   = IDNoLIst;
                            drHdr["Tbh_CostCenterList"]             = CostCenterList;
                            drHdr["Tbh_PayrollGroupList"]           = PayrollGroupList;
                            drHdr["Tbh_EmploymentStatusList"]       = EmploymentStatusList;
                            drHdr["Tbh_PayrollTypeList"]            = PayrollTypeList;
                            drHdr["Tbh_WorkStatusList"]             = WorkStatusList;
                            drHdr["Tbh_GradeList"]                  = GradeList;
                            drHdr["Tbh_HireRegOption"]              = HireRegOption;
                            if (GetValue(HireRegDate) != string.Empty)
                                drHdr["Tbh_HireRegDate"]            = Convert.ToDateTime(HireRegDate);
                            else
                                drHdr["Tbh_HireRegDate"]            = DBNull.Value;
                            drHdr["Tbh_OverrideBonusCeiling"]       = bOverrideBonusCeiling;
                            drHdr["Tbh_TaxCompensationLevel"]       = dTaxCompensationLevel;
                            drHdr["Tbh_RecordStatus"]               = "A";
                            drHdr["Usr_Login"]                      = LoginUser;
                            #endregion
                            UpdateHdr(drHdr, dal);
                            #endregion

                            #region Cleanup Dtl
                            Condition = "";
                            if (ProcessSeparated)
                            {
                                Condition = string.Format("AND Tbd_IDNo IN ({0})", EmployeeID);
                            }
                            dal.ExecuteNonQuery(string.Format(@"DELETE FROM T_EmpBonusDtl 
                                                        WHERE Tbd_ControlNo = '{0}'
                                                        AND Tbd_RetainUserEntry = 0
                                                        {1}", ControlNumber, Condition));
                            
                            #endregion
                        }

                        #region Create Bonus Dtl Records
                        for (int i = 0; i < dtEmployeeForProcess.Rows.Count; i++)
                        {
                            string curEmployeeId = dtEmployeeForProcess.Rows[i]["Mem_IDNo"].ToString();

                            int idxx = 0;
                            ParameterInfo[] paramDtl = new ParameterInfo[5];
                            paramDtl[idxx++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                            paramDtl[idxx++] = new ParameterInfo("@START", bStartCoverage);
                            paramDtl[idxx++] = new ParameterInfo("@END", bEndCoverage);
                            paramDtl[idxx++] = new ParameterInfo("@YEAR", PayrollPeriod.Substring(0, 4));
                            paramDtl[idxx++] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);

                            DataRow drEmpBonusDtl = DbRecord.Generate(CommonConstants.TableName.T_EmpBonusDtl);
                            drEmpBonusDtl["Tbd_ControlNo"]                  = ControlNumber;
                            drEmpBonusDtl["Tbd_LineNo"]                     = (!ProcessSeparated ? ctr.ToString().PadLeft(6, '0') : (GetLastSeqNo(ControlNumber, dal) + 1).ToString().PadLeft(6, '0'));
                            drEmpBonusDtl["Tbd_IDNo"]                       = curEmployeeId;
                            drEmpBonusDtl["Tbd_SalaryRate"]                 = GetFormulaQueryStringValue(BaseSalaryFormula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            drEmpBonusDtl["Tbd_PayrollType"]                = GetFormulaQueryStringValue(BasePayrollTypeFormula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            if (BaseAmt1Formula != string.Empty)
                                drEmpBonusDtl["Tbd_BaseAmt1"]               = GetFormulaQueryDoubleValue(BaseAmt1Formula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            else
                                drEmpBonusDtl["Tbd_BaseAmt1"]               = "0.00";
                            if (BaseAmt2Formula != string.Empty)
                                drEmpBonusDtl["Tbd_BaseAmt2"]               = GetFormulaQueryDoubleValue(BaseAmt2Formula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            else
                                drEmpBonusDtl["Tbd_BaseAmt2"]               = "0.00";
                            if (BonusFormula != string.Empty)
                                drEmpBonusDtl["Tbd_TotalBonusAmt"]          = GetFormulaQueryDoubleValue(BonusFormula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            else
                                drEmpBonusDtl["Tbd_TotalBonusAmt"]          = "0.00";
                            drEmpBonusDtl["Tbd_AccumBonusBeforeThisCycle"]  = 0;
                            drEmpBonusDtl["Tbd_BonusNontaxableAmt"]         = 0;
                            drEmpBonusDtl["Tbd_BonusTaxableAmt"]            = 0;

                            if (!bOverrideBonusCeiling)
                                drEmpBonusDtl["Tbd_AccumBonusBeforeThisCycle"] = GetAccumulatedBonus(curEmployeeId, PayrollPeriod, companyCode, CentralProfile, dal);
                            else
                            {
                                if (Convert.ToDecimal(drEmpBonusDtl["Tbd_TotalBonusAmt"]) < dTaxCompensationLevel)
                                    drEmpBonusDtl["Tbd_BonusNontaxableAmt"] = Convert.ToDecimal(drEmpBonusDtl["Tbd_TotalBonusAmt"]);
                                else
                                    drEmpBonusDtl["Tbd_BonusTaxableAmt"]    = Convert.ToDecimal(drEmpBonusDtl["Tbd_TotalBonusAmt"]);
                            }

                            drEmpBonusDtl["Tbd_PostFlag"]                   = 0;
                            drEmpBonusDtl["Tbd_CostCenter"]                 = dtEmployeeForProcess.Rows[i]["Mem_CostcenterCode"].ToString();
                            drEmpBonusDtl["Tbd_PayrollGroup"]               = dtEmployeeForProcess.Rows[i]["Mem_PayrollGroup"].ToString();
                            drEmpBonusDtl["Tbd_EmploymentStatus"]           = dtEmployeeForProcess.Rows[i]["Mem_EmploymentStatusCode"].ToString();
                            drEmpBonusDtl["Tbd_WorkStatus"]                 = dtEmployeeForProcess.Rows[i]["Mem_WorkStatus"].ToString();
                            drEmpBonusDtl["Tbd_Grade"]                      = dtEmployeeForProcess.Rows[i]["Mem_PositionGrade"].ToString();
                            drEmpBonusDtl["Tbd_BaseChar"]                   = GetFormulaQueryStringValue(BaseCharFormula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            if (BaseNumFormula != string.Empty)
                                drEmpBonusDtl["Tbd_BaseNum"]                = GetFormulaQueryDoubleValue(BaseNumFormula.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            else
                                drEmpBonusDtl["Tbd_BaseNum"]                = "0.00";
                            drEmpBonusDtl["Tbd_RetainUserEntry"]            = 0;
                            drEmpBonusDtl["Usr_Login"]                      = LoginUser;

                            InsertDtl(drEmpBonusDtl, dal);

                            #region Used NonTax and NonTax Amount Query
                            if (!bOverrideBonusCeiling)
                                UpdateNonTaxAndTaxAmt(curEmployeeId
                                                              , ControlNumber
                                                              , drEmpBonusDtl["Tbd_AccumBonusBeforeThisCycle"].ToString()
                                                              , M13EXCLTAX
                                                              , dal);
                            #endregion

                            ctr++;
                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmployeeForProcess.Rows[i]["Mem_LastName"].ToString(), dtEmployeeForProcess.Rows[i]["Mem_FirstName"].ToString()));

                        }

                        #endregion
                    }
                }
                else
                    throw new PayrollException(string.Format("{0} - No Bonus Master set-up.", BonusCode));

                #endregion
            }
            catch (Exception ex)
            {
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Bonus Calculation has encountered some errors: \n" + ex.Message);
            }

            return ControlNumber;
        }



        public void SetProcessFlags()
        {
            string strResult = string.Empty;
            //string LastPayProcessingErrorMessage = "";

            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetParameterValueFromPayroll("M13EXCLTAX", CompanyCode, dal);
            M13EXCLTAX = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  M13EXCLTAX = {0}", M13EXCLTAX), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  M13EXCLTAX = {0}", M13EXCLTAX), true));

            #endregion
        }

        private string EncodeFilterItems(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            string strFilterItem = "";
            foreach (string col in strArrFilterItems)
            {
                strFilterItem += "'" + col.Trim() + "',";
            }
            if (strFilterItem != "")
                strFilterItem = strFilterItem.Substring(0, strFilterItem.Length - 1);
            return strFilterItem;
        }

        public void DeleteBonus(string IDNumber, string ControlNo)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Format(@"DELETE FROM T_EmpBonusDtl
                                              WHERE Tbd_ControlNo = '{0}'
                                                AND Tbd_IDNo = '{1}'

                                            IF NOT EXISTS (SELECT Tbd_LineNo FROM T_EmpBonusDtl WHERE Tbd_ControlNo = '{0}' )
                                            BEGIN
	                                            UPDATE T_EmpBonusHdr SET Tbh_RecordStatus = 'C'
	                                            WHERE Tbh_ControlNo = '{0}'
                                            END", ControlNo, IDNumber);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(sqlQuery);
                dal.CloseDB();
            }
        }

        public DataTable GetBonusMasterDetails(string BonusCode)
        {
            string query = string.Format(@"SELECT * FROM {1}..M_Bonus
                                           WHERE Mbn_RecordStatus = 'A'
                                                AND Mbn_CompanyCode = '{0}'
                                                AND Mbn_BonusCode = '{2}'", CompanyCode, CentralProfile, BonusCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetBonusMasterDetails(string companyCode, string centralProfile)
        {
            string query = string.Format(@"SELECT Mbn_BonusCode
                                            ,Mbn_BonusName
                                            ,Mbn_Mode
                                            ,Mbn_BaseAmt1Formula
                                            ,Mbn_BaseAmt2Formula
                                            ,Mbn_BaseNumFormula
                                            ,Mbn_BaseCharFormula
                                            ,Mbn_BaseSalaryFormula
                                            ,Mbn_BasePayrollTypeFormula
                                            ,Mbn_BonusFormula
                                            ,Mbn_BonusCheckFormula
                                            ,Mbn_BonusDetailCheckFormula
                                            ,Mbn_CoverageSelection
                                            ,Mbn_StartCoverage
                                            ,Mbn_EndCoverage
                                            ,Mbn_BaseAmt1Heading
                                            ,Mbn_BaseAmt2Heading
                                            ,Mbn_BaseNumHeading
                                            ,Mbn_BaseCharHeading
                                            ,Mbn_BonusHeading
                                            ,Mbn_NontaxableCode
                                            ,Mbn_TaxableCode
                                            ,Mbn_PostToIncome 
                                        FROM {1}..M_Bonus
                                        WHERE Mbn_RecordStatus = 'A'
                                            AND Mbn_CompanyCode = '{0}'", companyCode, centralProfile);
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetBonusMasterDetails(string companyCode, string centralProfile, DALHelper dal)
        {
            string query = string.Format(@"SELECT * FROM {1}..M_Bonus
                            WHERE Mbn_RecordStatus = 'A'
                                AND Mbn_CompanyCode = '{0}'", companyCode, centralProfile);
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public decimal GetTaxCompensationLevel(string PayCycleCode, string TaxScheduleCode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string query = string.Format(@"DECLARE @MAXTAXHEADER CHAR(7) = (SELECT Max(Mth_PayCycle)
                                                                            FROM {0}..M_TaxScheduleHdr
                                                                            WHERE Mth_TaxSchedule = '{3}'
                                                                                AND Mth_PayCycle <= '{2}'
                                                                                AND Mth_CompanyCode = '{1}'
                                                                                AND Mth_RecordStatus = 'A')

                                           SELECT TOP 1 Mtd_CompensationLevel AS [Tax Compensation Level]
                                           FROM {0}..M_TaxScheduleDtl
                                           INNER JOIN {0}..M_TaxScheduleHdr 
                                           ON Mth_CompanyCode = Mtd_CompanyCode
	                                             AND Mth_PayCycle = Mtd_PayCycle
	                                             AND Mth_TaxSchedule = Mtd_TaxSchedule
	                                             AND Mth_BracketNo = Mtd_BracketNo
	                                             AND Mth_TaxOnExcess > 0
                                          WHERE Mtd_CompanyCode =  '{1}'
	                                             AND Mtd_paycycle =  @MAXTAXHEADER
	                                             AND Mtd_TaxSchedule = '{3}'
	                                             AND Mtd_RecordStatus = 'A'"
                                          , centralProfile
                                          , companyCode
                                          , PayCycleCode
                                          , TaxScheduleCode);

            DataTable dt = dalhelper.ExecuteDataSet(query).Tables[0];
            if (dt.Rows.Count > 0)
                return Convert.ToDecimal(dt.Rows[0][0].ToString());
            else
                return 0;
        }


        public decimal GetTaxCompensationLevel(string PayCycleCode, string TaxScheduleCode)
        {
            string query = string.Format(@"DECLARE @MAXTAXHEADER CHAR(7) = (SELECT Max(Mth_PayCycle)
                                                                            FROM {0}..M_TaxScheduleHdr
                                                                            WHERE Mth_TaxSchedule = '{3}'
                                                                                AND Mth_PayCycle <= '{2}'
                                                                                AND Mth_CompanyCode = '{1}'
                                                                                AND Mth_RecordStatus = 'A')

                                           SELECT TOP 1 Mtd_CompensationLevel AS [Tax Compensation Level]
                                           FROM {0}..M_TaxScheduleDtl
                                           INNER JOIN {0}..M_TaxScheduleHdr 
                                           ON Mth_CompanyCode = Mtd_CompanyCode
	                                             AND Mth_PayCycle = Mtd_PayCycle
	                                             AND Mth_TaxSchedule = Mtd_TaxSchedule
	                                             AND Mth_BracketNo = Mtd_BracketNo
	                                             AND Mth_TaxOnExcess > 0
                                          WHERE Mtd_CompanyCode =  '{1}'
	                                             AND Mtd_paycycle =  @MAXTAXHEADER
	                                             AND Mtd_TaxSchedule = '{3}'
	                                             AND Mtd_RecordStatus = 'A'"
                                          , CentralProfile
                                          , CompanyCode
                                          , PayCycleCode
                                          , TaxScheduleCode);

            DataTable dt = dal.ExecuteDataSet(query).Tables[0];
            if (dt.Rows.Count > 0)
                return Convert.ToDecimal(dt.Rows[0][0].ToString());
            else
                return 0;
        }


        public void CheckPreProcessingExists(string ProfileCode, string centralProfile, string PayCycle, string EmployeeID, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"
            DECLARE @MMaxRate DECIMAL(18, 2) = 0
            DECLARE @DMaxRate DECIMAL(18, 2) = 0
            DECLARE @MMinRate DECIMAL(18, 2) = 0
            DECLARE @DMinRate DECIMAL(18, 2) = 0
            DECLARE @MenuCode VARCHAR(20) = '{5}'
           
            SELECT @MMaxRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MAXRATE'
	            AND Mpd_SubCode = 'M'
                AND Mpd_CompanyCode = '{1}'

            SELECT @DMaxRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MAXRATE'
	            AND Mpd_SubCode = 'D'
                AND Mpd_CompanyCode = '{1}'

            SELECT @MMINRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MINRATE'
	            AND Mpd_SubCode = 'M'
                AND Mpd_CompanyCode = '{1}'            	

            SELECT @DMINRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MINRATE'
	            AND Mpd_SubCode = 'D'
                AND Mpd_CompanyCode = '{1}' 

         DELETE FROM T_EmpProcessCheck WHERE Tpc_ModuleCode = @MenuCode AND Tpc_SystemDefined = 1
            AND Tpc_PayCycle = '{3}'
            AND ('{2}' = '' OR Tpc_IDNo = '{2}')

         INSERT INTO T_EmpProcessCheck

         SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , Mem_Salary 
	            , @MMaxRate 
	            ,'Employee Salary is more than Monthlies Maximum Salary' 
                , 1
	            ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @MMaxRate > 0
	            AND Mem_Salary > @MMaxRate
	            AND Mem_PayrollType = 'M'
                AND ('{2}' = '' OR Mem_IDNo = '{2}')

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BW' 
	            ,'{3}'
	            , Mem_Salary 
	            , @MMinRate 
	            ,'Employee Salary is less than Monthlies Minimum Salary' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @MMinRate > 0
	            AND Mem_Salary < @MMinRate
	            AND Mem_PayrollType = 'M'
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
            		
            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , Mem_Salary 
	            , @DMaxRate 
	            ,'Employee Salary is more than Dailies Maximum Salary' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @DMaxRate > 0
	            AND Mem_Salary > @DMaxRate
	            AND Mem_PayrollType = 'D'
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , Mem_Salary 
	            , @DMinRate 
	            ,'Employee Salary is less than Dailies Minimum Salary' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @DMinRate > 0
	            AND Mem_Salary < @DMinRate
	            AND Mem_PayrollType = 'D'
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            ,Mem_Salary 
	            , NULL 
	            ,'Employee Salary is zero or negative' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND Mem_Salary <= 0
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
            "
            , centralProfile
            , companyCode
            , EmployeeID
            , PayCycle
            , UserLogin
            , menucode);
            #endregion
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public void CheckPostProcessingExists(string ProfileCode, string centralProfile, string PayCycle, string ControlNo, string EmployeeID, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"
                INSERT INTO T_EmpProcessCheck

                SELECT '{5}'
                        , Tbd_IDNo
                        , 'AE'
                        , '{3}'
                        , Tbd_SalaryRate
                        , 0
                        , 'Salary Rate < Total Bonus Amount' as Remarks
                        , 1
                        , '{4}'
                        , GETDATE()
                    FROM T_EmpBonusDtl
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE Tbd_ControlNo = '{6}'
                        AND Tbd_SalaryRate < Tbd_TotalBonusAmt 
                        AND ('{2}' = '' OR Tbd_IDNo = '{2}')
                       --@CONDITIONS

                UNION ALL

                SELECT '{5}'
                        , Tbd_IDNo
                        , 'AE'
                        , '{3}'
                        , Tbd_TotalBonusAmt
                        , 0
                        , 'Zero Total Bonus Amount' as Remarks
                        , 1
                        , '{4}'
                        , GETDATE()
                    FROM T_EmpBonusDtl
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE Tbd_ControlNo = '{6}'
                        AND Tbd_TotalBonusAmt = 0
                        AND ('{2}' = '' OR Tbd_IDNo = '{2}')
                       --@CONDITIONS

                UNION ALL

                SELECT '{5}'
                        , Tbd_IDNo
                        , 'AE'
                        , '{3}'
                        , Tbd_TotalBonusAmt
                        , 0
                        , 'Negative Total Bonus Amount' as Remarks
                        , 1
                        , '{4}'
                        , GETDATE()
                    FROM T_EmpBonusDtl
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE Tbd_ControlNo = '{6}'
                        AND Tbd_TotalBonusAmt < 0
                        AND ('{2}' = '' OR Tbd_IDNo = '{2}')
                       --@CONDITIONS

                        "
           , centralProfile, companyCode, EmployeeID, PayCycle, UserLogin, menucode, ControlNo);
            #endregion
            //query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "Tbd_CostCenter", "Tbd_PayrollGroup", "Tbd_EmploymentStatus", "Tbd_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public DataTable BonusExceptionList(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string userlogin, string companyCode, string ErrorType, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (ErrorType != string.Empty)
            {
                condition = string.Format(@"
                                            And Tpc_Type LIKE '%{0}'", ErrorType);
            }
            string query = string.Format(@"
                        SELECT CASE WHEN Tpc_Type IN ('AW','BW') THEN 'WARNING' 
                                    WHEN Tpc_Type IN ('AE','BE') THEN 'ERROR' END as [Type]
                                    , Tpc_IDNo AS [ID Number] 
									, Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
										THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
									, Mem_FirstName as [First Name]
									, Mem_MiddleName as [Middle Name]
                                    , Tpc_NumValue1 AS [Amount]
                                    , Tpc_NumValue2 AS [Base Amount]
                                    , Tpc_Remarks AS [Remarks]
                                    FROM (SELECT * FROM T_EmpProcessCheck 
                                          UNION ALL
                                          SELECT * FROM T_EmpProcessCheckHst ) Tmp
									INNER JOIN M_Employee ON Mem_IDNo = Tpc_IDNo
                                    @USERCOSTCENTERACCESSCONDITION  
                                    WHERE Tpc_ModuleCode = '{4}'
                                    AND Tpc_PayCycle = '{0}'
                                    AND ('{1}' = '' OR Tpc_IDNo = '{1}')
                                    @CONDITIONS	
                                    "
                                    , PayCycle, EmployeeID, companyCode, centralProfile, menucode);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", userlogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
    }
}
