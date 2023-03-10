using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class PayrollTransactionPostingBL : BaseBL
    {
        DataSet ds = new DataSet();

        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
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

        #region <Query Statements>

        public string SQLFetchAdjustmentRecords()
        {
            return @"Select  Tma_IDNo
	                                   ,Tma_TotalAdjAmt
	                                   ,Tma_TaxableAdjAmt
	                                   ,Tma_NontaxableAdjAmt
                                From T_EmpManualAdj Inner Join T_EmpPayTranHdr
	                                On Tma_IDNo = Tph_IDNo
	                                And Tma_PayCycle = Tph_PayCycle
                                Where Tma_PayCycle = @Tma_PayCycle
	                                And Tma_PostFlag = 0";
        }

        public string SQLUpdatePayrollTransAdj()
        {  
            return @"Update T_EmpPayTranHdr
                                Set  Tph_TotalAdjAmt = @Tma_TotalAdjAmt
	                                ,Tph_TaxableAdjAmt = @Tma_TaxableAdjAmt
	                                ,Tph_NontaxableAdjAmt = @Tma_NontaxableAdjAmt
	                                ,Usr_Login = @Usr_Login
	                                ,Ludatetime = Getdate()
                                Where Tph_PayCycle = @Tma_PayCycle
                                    and Tph_IDNo =@Tma_IDNo
                                --and Ept_PayrollPost = 0";
        }

        public string SQLUpdateAdjPostFlag()
        {
            return @"Update T_EmpManualAdj
                                Set Tma_PostFlag = 1
                                From T_EmpManualAdj Inner Join T_EmpPayTranHdr
	                                On Tma_IDNo = Tph_IDNo
	                                And Tma_PayCycle = Tph_PayCycle
                                Where Tma_PayCycle = @Tma_PayCycle";
        }

        public string SQLUpdatePayrollTransAllowance()
        {
            return @"Update T_EmpPayTranHdr
                                Set  Tph_TaxableIncomeAmt = TaxAllowanceAmt
	                                ,Tph_NontaxableIncomeAmt = NonTaxAllowanceAmt
	                                ,Usr_Login = @Usr_Login
	                                ,Ludatetime = Getdate()
                                From T_EmpPayTranHdr Inner Join
                                (Select Tin_IDNo
		                                ,TaxAllowanceAmt = Sum(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
		                                ,NonTaxAllowanceAmt = Sum(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                 From T_EmpIncome Inner Join M_Income
	                                On Min_IncomeCode = Tin_IncomeCode
                                 Where Tin_PayCycle = @Tph_PayCycle
	                                And Tin_PostFlag = 0
                                 Group by Tin_IDNo)Allowance on Tph_IDNo = Tin_IDNo
                                Where Tph_PayCycle = @Tph_PayCycle";
        }

        public string SQLUpdateAllowancePostFlag()
        {
            return @"Update T_EmpIncome
                                Set Tin_PostFlag = 1
                                From T_EmpIncome Inner Join T_EmpPayTranHdr
	                                On Tin_IDNo = Tph_IDNo
	                                And Tin_PayCycle = Tph_PayCycle
                                Where Tin_PayCycle = @Tin_PayCycle";
        }

        public string SQLUpdateProcessControlFlag()
        {
            return @"Update T_SettingControl
                                    Set Tsc_SetFlag = @Tsc_SetFlag
                                    ,Usr_Login = @Usr_Login
                                    ,Ludatetime = Getdate()
                                    Where Tsc_SystemCode = @Tsc_SystemCode
                                    And Tsc_SettingCode = @Tsc_SettingCode";
        }

        public string SQLCleanUpBeforePosting(string CurrentPayPeriod)
        {
            return string.Format(@"Update T_EmpPayTranHdr
                                    Set  Tph_TotalAdjAmt = '0.00'
                                        ,Tph_TaxableAdjAmt = '0.00'
                                        ,Tph_NontaxableAdjAmt = '0.00'
                                        ,Tph_TaxableIncomeAmt = '0.00'
	                                    ,Tph_NontaxableIncomeAmt = '0.00'
                                    Where Tph_PayCycle = '{0}'
                                    And (Tph_TotalAdjAmt <> 0 
                                       OR Tph_TaxableAdjAmt <> 0
                                       OR Tph_NontaxableAdjAmt <> 0
                                       OR Tph_TaxableIncomeAmt <> 0
                                       OR Tph_NontaxableIncomeAmt <> 0)", CurrentPayPeriod);
        }

        public string SQLCleanUpAllowanceBeforePosting(string CurrentPayPeriod)
        {
            return string.Format(@"Update T_EmpIncome
                                    Set Tin_PostFlag = 0
                                    Where Tin_PayCycle = '{0}'", CurrentPayPeriod);
        }

        public string SQLCleanUpAdjustmentBeforePosting(string CurrentPayPeriod)
        {
            return string.Format(@"Update T_EmpManualAdj
                                    Set Tma_PostFlag = 0
                                    Where Tma_PayCycle = '{0}'", CurrentPayPeriod);
        }

        #endregion

        #region <Functions for Posting>

        public DataSet FetchAdjustmentRecords(string CurrentPayPeriod, DALHelper DalUp)
        {
            DataSet ds = new DataSet();

            string qString = this.SQLFetchAdjustmentRecords();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tma_PayCycle", CurrentPayPeriod);

            ds = DalUp.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            return ds;
        }

        public void CleanUp(string CurrentPayPeriod, DALHelper DalUp)
        {
            int retVal = 0;

            string qString = this.SQLCleanUpBeforePosting(CurrentPayPeriod);
            string qString1 = this.SQLCleanUpAllowanceBeforePosting(CurrentPayPeriod);
            string qString2 = this.SQLCleanUpAdjustmentBeforePosting(CurrentPayPeriod);

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text);
            retVal = DalUp.ExecuteNonQuery(qString1, CommandType.Text);
            retVal = DalUp.ExecuteNonQuery(qString2, CommandType.Text);
        }

        public void UpdatePayrollTransAdj(string CurrentPayPeriod, string LaborHrsAdjustmentAmnt, string TaxAdjustmentAmt, string NonTaxAdjustmentAmt, string EmployeeID, DALHelper DalUp)
        {
            int retVal = 0;

            string qString = this.SQLUpdatePayrollTransAdj();
            //gcd 111608
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Tma_TotalAdjAmt", LaborHrsAdjustmentAmnt);
            paramInfo[1] = new ParameterInfo("@Tma_TaxableAdjAmt", TaxAdjustmentAmt);
            paramInfo[2] = new ParameterInfo("@Tma_NontaxableAdjAmt", NonTaxAdjustmentAmt);
            paramInfo[3] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            paramInfo[4] = new ParameterInfo("@Tma_PayCycle", CurrentPayPeriod);
            paramInfo[5] = new ParameterInfo("@Tma_IDNo", EmployeeID );

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

        }

        public void UpdateAdjPostFlag(string CurrentPayPeriod, DALHelper DalUp)
        {
            int retVal = 0;

            string qString = this.SQLUpdateAdjPostFlag();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tma_PayCycle", CurrentPayPeriod);

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        public void ExecuteAdjProcess(string CurrentPayPeriod, DALHelper DalUp)
        {
            string LaborHrsAdjustmentAmnt = string.Empty;
            string TaxAdjustmentAmt = string.Empty;
            string NonTaxAdjustmentAmt = string.Empty;
            // gcd 111608
            string EmployeeID = string.Empty;

            ds = this.FetchAdjustmentRecords(CurrentPayPeriod, DalUp);
            
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                LaborHrsAdjustmentAmnt = ds.Tables[0].Rows[i]["Tma_TotalAdjAmt"].ToString().Trim();
                TaxAdjustmentAmt = ds.Tables[0].Rows[i]["Tma_TaxableAdjAmt"].ToString().Trim();
                NonTaxAdjustmentAmt = ds.Tables[0].Rows[i]["Tma_NontaxableAdjAmt"].ToString().Trim();
                // gcd111608
                EmployeeID = ds.Tables[0].Rows[i]["Tma_IDNo"].ToString().Trim();

                this.UpdatePayrollTransAdj(CurrentPayPeriod, LaborHrsAdjustmentAmnt, TaxAdjustmentAmt, NonTaxAdjustmentAmt, EmployeeID,DalUp);
            }
            if (ds.Tables[0].Rows.Count > 0)
                this.UpdateAdjPostFlag(CurrentPayPeriod, DalUp);
        }

        public void UpdatePayrollTransAllowance(string CurrentPayPeriod, DALHelper DalUp)
        {
            int retVal = 0;

            string qString = this.SQLUpdatePayrollTransAllowance();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tph_PayCycle", CurrentPayPeriod);
            paramInfo[1] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        public void UpdateAllowancePostFlag(string CurrentPayPeriod, DALHelper DalUp)
        {
            int retVal = 0;

            string qString = this.SQLUpdateAllowancePostFlag();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tin_PayCycle", CurrentPayPeriod);

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        public void UpdateProcessControlFlag(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode, DALHelper DalUp)
        {
            int retVal = 0;

            string qString = this.SQLUpdateProcessControlFlag();

            ParameterInfo[] UpparamInfo = new ParameterInfo[4];
            UpparamInfo[0] = new ParameterInfo("@Tsc_SetFlag", Tsc_SetFlag);
            UpparamInfo[1] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            UpparamInfo[2] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);
            UpparamInfo[3] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            
            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, UpparamInfo);
        }

        public void ExecuteAllowanceProcess(string CurrentPayPeriod, DALHelper DalUp)
        {
            this.UpdatePayrollTransAllowance(CurrentPayPeriod, DalUp);
            this.UpdateAllowancePostFlag(CurrentPayPeriod, DalUp);

            this.UpdateProcessControlFlag("True", "PAYROLL", "PAYTRNPOST", DalUp);
        }

        #endregion

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

        public DataTable GetValidPayPeriod()
        {
            #region query
            string query = string.Format(@"SELECT Tps_PayCycle FROM T_PaySchedule
                                           WHERE Tps_CycleIndicator IN ('C', 'S')
                                                and Tps_RecordStatus = 'A'");
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

        public DataTable GetPayrollTransactionRecords()
        {
            #region query
            string query = @"select Tph_IDNo, Tph_PayCycle
                              from T_EmpPayTranHdr ";
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

        public DataTable GetDayCodeNames(string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(
                                @"SELECT Mmd_MiscDayID
	                            , CASE WHEN Mmd_RestDayFlag = 1 
		                            THEN 'REST ' 
		                            ELSE ''
		                            END + Mmd_DayCode as Mmd_DayCode
	                            , CASE WHEN Mmd_RestDayFlag = 1 
		                            THEN 'REST ' 
		                            ELSE ''
		                            END + Mdy_DayName as Mdy_DayName
                            FROM M_MiscellaneousDay
                            INNER JOIN M_Day
                                ON Mmd_DayCode = Mdy_DayCode
                                AND  Mmd_CompanyCode = Mdy_CompanyCode
                            WHERE Mmd_RecordStatus = 'A'
                                AND Mdy_CompanyCode = '{0}'", CompanyCode);
            #endregion
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public int checkInFiller(string CompanyCode, string CentralProfile)
        {
            string query;
            DataTable dt;

            #region query
            query = string.Format(
                    @"SELECT Isnull(Count(Mmd_DayCode),0) as CheckFiller
                    FROM M_MiscellaneousDay
                    INNER JOIN M_Day on Mmd_DayCode = Mdy_DayCode
                          AND Mmd_CompanyCode = Mdy_CompanyCode
                    WHERE Mmd_RecordStatus = 'A' 
                          AND Mdy_RecordStatus = 'A'
                            AND Mmd_CompanyCode = '{0}'", CompanyCode);
            #endregion

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (!dt.Rows[0]["CheckFiller"].ToString().Equals("0"))
            {
                return Convert.ToInt32(dt.Rows[0]["CheckFiller"].ToString());
            }
            else
                return 0;
        }

        public int InsertEmployeePayrollTransaction(DataRow rowEmployeePayrollTransaction, DALHelper dal)
        {
            int retVal = 0;
            #region Parameters

            int i = 0;
            ParameterInfo[] param = new ParameterInfo[63];
            param[i++] = new ParameterInfo("@Tph_IDNo", rowEmployeePayrollTransaction["Tph_IDNo"]);
            param[i++] = new ParameterInfo("@Tph_PayCycle", rowEmployeePayrollTransaction["Tph_PayCycle"]);
            param[i++] = new ParameterInfo("@Tph_LTHr", rowEmployeePayrollTransaction["Tph_LTHr"]);
            param[i++] = new ParameterInfo("@Tph_UTHr", rowEmployeePayrollTransaction["Tph_UTHr"]);
            param[i++] = new ParameterInfo("@Tph_UPLVHr", rowEmployeePayrollTransaction["Tph_UPLVHr"]);
            param[i++] = new ParameterInfo("@Tph_ABSLEGHOLHr", rowEmployeePayrollTransaction["Tph_ABSLEGHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_ABSSPLHOLHr", rowEmployeePayrollTransaction["Tph_ABSSPLHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_ABSCOMPHOLHr", rowEmployeePayrollTransaction["Tph_ABSCOMPHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_ABSPSDHr", rowEmployeePayrollTransaction["Tph_ABSPSDHr"]);
            param[i++] = new ParameterInfo("@Tph_ABSOTHHOLHr", rowEmployeePayrollTransaction["Tph_ABSOTHHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_WDABSHr", rowEmployeePayrollTransaction["Tph_WDABSHr"]);
            param[i++] = new ParameterInfo("@Tph_LTUTMaxHr", rowEmployeePayrollTransaction["Tph_LTUTMaxHr"]);
            param[i++] = new ParameterInfo("@Tph_ABSHr", rowEmployeePayrollTransaction["Tph_ABSHr"]);
            param[i++] = new ParameterInfo("@Tph_REGHr", rowEmployeePayrollTransaction["Tph_REGHr"]);
            param[i++] = new ParameterInfo("@Tph_PDLVHr", rowEmployeePayrollTransaction["Tph_PDLVHr"]);
            param[i++] = new ParameterInfo("@Tph_PDLEGHOLHr", rowEmployeePayrollTransaction["Tph_PDLEGHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_PDSPLHOLHr", rowEmployeePayrollTransaction["Tph_PDSPLHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_PDCOMPHOLHr", rowEmployeePayrollTransaction["Tph_PDCOMPHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_PDPSDHr", rowEmployeePayrollTransaction["Tph_PDPSDHr"]);
            param[i++] = new ParameterInfo("@Tph_PDOTHHOLHr", rowEmployeePayrollTransaction["Tph_PDOTHHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_PDRESTLEGHOLHr", rowEmployeePayrollTransaction["Tph_PDRESTLEGHOLHr"]);

            param[i++] = new ParameterInfo("@Tph_REGOTHr", rowEmployeePayrollTransaction["Tph_REGOTHr"]);
            param[i++] = new ParameterInfo("@Tph_REGNDHr", rowEmployeePayrollTransaction["Tph_REGNDHr"]);
            param[i++] = new ParameterInfo("@Tph_REGNDOTHr", rowEmployeePayrollTransaction["Tph_REGNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_RESTHr", rowEmployeePayrollTransaction["Tph_RESTHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTOTHr", rowEmployeePayrollTransaction["Tph_RESTOTHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTNDHr", rowEmployeePayrollTransaction["Tph_RESTNDHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTNDOTHr", rowEmployeePayrollTransaction["Tph_RESTNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_LEGHOLHr", rowEmployeePayrollTransaction["Tph_LEGHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_LEGHOLOTHr", rowEmployeePayrollTransaction["Tph_LEGHOLOTHr"]);
            param[i++] = new ParameterInfo("@Tph_LEGHOLNDHr", rowEmployeePayrollTransaction["Tph_LEGHOLNDHr"]);
            param[i++] = new ParameterInfo("@Tph_LEGHOLNDOTHr", rowEmployeePayrollTransaction["Tph_LEGHOLNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_SPLHOLHr", rowEmployeePayrollTransaction["Tph_SPLHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_SPLHOLOTHr", rowEmployeePayrollTransaction["Tph_SPLHOLOTHr"]);
            param[i++] = new ParameterInfo("@Tph_SPLHOLNDHr", rowEmployeePayrollTransaction["Tph_SPLHOLNDHr"]);
            param[i++] = new ParameterInfo("@Tph_SPLHOLNDOTHr", rowEmployeePayrollTransaction["Tph_SPLHOLNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_COMPHOLHr", rowEmployeePayrollTransaction["Tph_COMPHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_COMPHOLOTHr", rowEmployeePayrollTransaction["Tph_COMPHOLOTHr"]);
            param[i++] = new ParameterInfo("@Tph_COMPHOLNDHr", rowEmployeePayrollTransaction["Tph_COMPHOLNDHr"]);
            param[i++] = new ParameterInfo("@Tph_COMPHOLNDOTHr", rowEmployeePayrollTransaction["Tph_COMPHOLNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_PSDHr", rowEmployeePayrollTransaction["Tph_PSDHr"]);
            param[i++] = new ParameterInfo("@Tph_PSDOTHr", rowEmployeePayrollTransaction["Tph_PSDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_PSDNDHr", rowEmployeePayrollTransaction["Tph_PSDNDHr"]);
            param[i++] = new ParameterInfo("@Tph_PSDNDOTHr", rowEmployeePayrollTransaction["Tph_PSDNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_RESTLEGHOLHr", rowEmployeePayrollTransaction["Tph_RESTLEGHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTLEGHOLOTHr", rowEmployeePayrollTransaction["Tph_RESTLEGHOLOTHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTLEGHOLNDHr", rowEmployeePayrollTransaction["Tph_RESTLEGHOLNDHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTLEGHOLNDOTHr", rowEmployeePayrollTransaction["Tph_RESTLEGHOLNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_RESTSPLHOLHr", rowEmployeePayrollTransaction["Tph_RESTSPLHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTSPLHOLOTHr", rowEmployeePayrollTransaction["Tph_RESTSPLHOLOTHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTSPLHOLNDHr", rowEmployeePayrollTransaction["Tph_RESTSPLHOLNDHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTSPLHOLNDOTHr", rowEmployeePayrollTransaction["Tph_RESTSPLHOLNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_RESTCOMPHOLHr", rowEmployeePayrollTransaction["Tph_RESTCOMPHOLHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTCOMPHOLOTHr", rowEmployeePayrollTransaction["Tph_RESTCOMPHOLOTHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTCOMPHOLNDHr", rowEmployeePayrollTransaction["Tph_RESTCOMPHOLNDHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTCOMPHOLNDOTHr", rowEmployeePayrollTransaction["Tph_RESTCOMPHOLNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_RESTPSDHr", rowEmployeePayrollTransaction["Tph_RESTPSDHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTPSDOTHr", rowEmployeePayrollTransaction["Tph_RESTPSDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTPSDNDHr", rowEmployeePayrollTransaction["Tph_RESTPSDNDHr"]);
            param[i++] = new ParameterInfo("@Tph_RESTPSDNDOTHr", rowEmployeePayrollTransaction["Tph_RESTPSDNDOTHr"]);

            param[i++] = new ParameterInfo("@Tph_WorkDay", rowEmployeePayrollTransaction["Tph_WorkDay"]);
            param[i++] = new ParameterInfo("@Tph_RetainUserEntry", rowEmployeePayrollTransaction["Tph_RetainUserEntry"]);
            param[i++] = new ParameterInfo("@Usr_Login", rowEmployeePayrollTransaction["Usr_Login"]);

            #endregion         

            #region Query
            string query = @"INSERT INTO T_EmpPayTranHdr
                            ( Tph_IDNo
                            , Tph_PayCycle
                            , Tph_LTHr
                            , Tph_UTHr
                            , Tph_UPLVHr
                            , Tph_ABSLEGHOLHr
                            , Tph_ABSSPLHOLHr
                            , Tph_ABSCOMPHOLHr
                            , Tph_ABSPSDHr
                            , Tph_ABSOTHHOLHr
                            , Tph_WDABSHr
                            , Tph_LTUTMaxHr
                            , Tph_ABSHr
                            , Tph_REGHr
                            , Tph_PDLVHr
                            , Tph_PDLEGHOLHr
                            , Tph_PDSPLHOLHr
                            , Tph_PDCOMPHOLHr
                            , Tph_PDPSDHr
                            , Tph_PDOTHHOLHr
                            , Tph_PDRESTLEGHOLHr

                            , Tph_REGOTHr
                            , Tph_REGNDHr
                            , Tph_REGNDOTHr
                            , Tph_RESTHr
                            , Tph_RESTOTHr
                            , Tph_RESTNDHr
                            , Tph_RESTNDOTHr
                            , Tph_LEGHOLHr
                            , Tph_LEGHOLOTHr
                            , Tph_LEGHOLNDHr
                            , Tph_LEGHOLNDOTHr
                            , Tph_SPLHOLHr
                            , Tph_SPLHOLOTHr
                            , Tph_SPLHOLNDHr
                            , Tph_SPLHOLNDOTHr
                            , Tph_PSDHr
                            , Tph_PSDOTHr
                            , Tph_PSDNDHr
                            , Tph_PSDNDOTHr
                            , Tph_COMPHOLHr
                            , Tph_COMPHOLOTHr
                            , Tph_COMPHOLNDHr
                            , Tph_COMPHOLNDOTHr
                            , Tph_RESTLEGHOLHr
                            , Tph_RESTLEGHOLOTHr
                            , Tph_RESTLEGHOLNDHr
                            , Tph_RESTLEGHOLNDOTHr
                            , Tph_RESTSPLHOLHr
                            , Tph_RESTSPLHOLOTHr
                            , Tph_RESTSPLHOLNDHr
                            , Tph_RESTSPLHOLNDOTHr
                            , Tph_RESTCOMPHOLHr
                            , Tph_RESTCOMPHOLOTHr
                            , Tph_RESTCOMPHOLNDHr
                            , Tph_RESTCOMPHOLNDOTHr
                            , Tph_RESTPSDHr
                            , Tph_RESTPSDOTHr
                            , Tph_RESTPSDNDHr
                            , Tph_RESTPSDNDOTHr

                            , Tph_SRGAdjHr
                            , Tph_SRGAdjAmt
                            , Tph_SOTAdjHr
                            , Tph_SOTAdjAmt
                            , Tph_SHOLAdjHr
                            , Tph_SHOLAdjAmt
                            , Tph_SNDAdjHr
                            , Tph_SNDAdjAmt
                            , Tph_SLVAdjHr
                            , Tph_SLVAdjAmt
                            , Tph_MRGAdjHr
                            , Tph_MRGAdjAmt
                            , Tph_MOTAdjHr
                            , Tph_MOTAdjAmt
                            , Tph_MHOLAdjHr
                            , Tph_MHOLAdjAmt
                            , Tph_MNDAdjHr
                            , Tph_MNDAdjAmt
            
                            , Tph_TotalAdjAmt
                            , Tph_TaxableIncomeAmt
                            , Tph_NontaxableIncomeAmt
                            
                            , Tph_WorkDay
                            , Tph_PayrollType
                            , Tph_RetainUserEntry
                            , Usr_Login
                            , Ludatetime)
                           VALUES
                            ( @Tph_IDNo
                            , @Tph_PayCycle
                            , @Tph_LTHr
                            , @Tph_UTHr
                            , @Tph_UPLVHr
                            , @Tph_ABSLEGHOLHr
                            , @Tph_ABSSPLHOLHr
                            , @Tph_ABSCOMPHOLHr
                            , @Tph_ABSPSDHr
                            , @Tph_ABSOTHHOLHr
                            , @Tph_WDABSHr
                            , @Tph_LTUTMaxHr
                            , @Tph_ABSHr
                            , @Tph_REGHr
                            , @Tph_PDLVHr
                            , @Tph_PDLEGHOLHr
                            , @Tph_PDSPLHOLHr
                            , @Tph_PDCOMPHOLHr
                            , @Tph_PDPSDHr
                            , @Tph_PDOTHHOLHr
                            , @Tph_PDRESTLEGHOLHr

                            , @Tph_REGOTHr
                            , @Tph_REGNDHr
                            , @Tph_REGNDOTHr
                            , @Tph_RESTHr
                            , @Tph_RESTOTHr
                            , @Tph_RESTNDHr
                            , @Tph_RESTNDOTHr
                            , @Tph_LEGHOLHr
                            , @Tph_LEGHOLOTHr
                            , @Tph_LEGHOLNDHr
                            , @Tph_LEGHOLNDOTHr
                            , @Tph_SPLHOLHr
                            , @Tph_SPLHOLOTHr
                            , @Tph_SPLHOLNDHr
                            , @Tph_SPLHOLNDOTHr
                            , @Tph_PSDHr
                            , @Tph_PSDOTHr
                            , @Tph_PSDNDHr
                            , @Tph_PSDNDOTHr
                            , @Tph_COMPHOLHr
                            , @Tph_COMPHOLOTHr
                            , @Tph_COMPHOLNDHr
                            , @Tph_COMPHOLNDOTHr
                            , @Tph_RESTLEGHOLHr
                            , @Tph_RESTLEGHOLOTHr
                            , @Tph_RESTLEGHOLNDHr
                            , @Tph_RESTLEGHOLNDOTHr
                            , @Tph_RESTSPLHOLHr
                            , @Tph_RESTSPLHOLOTHr
                            , @Tph_RESTSPLHOLNDHr
                            , @Tph_RESTSPLHOLNDOTHr
                            , @Tph_RESTCOMPHOLHr
                            , @Tph_RESTCOMPHOLOTHr
                            , @Tph_RESTCOMPHOLNDHr
                            , @Tph_RESTCOMPHOLNDOTHr
                            , @Tph_RESTPSDHr
                            , @Tph_RESTPSDOTHr
                            , @Tph_RESTPSDNDHr
                            , @Tph_RESTPSDNDOTHr

                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0
                            , 0

                            , @Tph_WorkDay
                            , (SELECT Mem_PayrollType FROM M_Employee WHERE Mem_IDNo = @Tph_IDNo)
                            , @Tph_RetainUserEntry
                            , @Usr_Login
                            , GETDATE())"; 
            #endregion
            
            retVal = dal.ExecuteNonQuery(query, CommandType.Text, param);
                
            return retVal;
        }

        public int InsertEmployeePayrollTransactionExt(DataRow rowEmployeePayrollTransactionExt, DALHelper dal)
        {
            int retVal = 0;
            #region Parameters

            int i = 0;
            ParameterInfo[] param = new ParameterInfo[27];
            param[i++] = new ParameterInfo("@Tph_IDNo", rowEmployeePayrollTransactionExt["Tph_IDNo"]);
            param[i++] = new ParameterInfo("@Tph_PayCycle", rowEmployeePayrollTransactionExt["Tph_PayCycle"]);
            param[i++] = new ParameterInfo("@Tph_Misc1Hr", rowEmployeePayrollTransactionExt["Tph_Misc1Hr"]);
            param[i++] = new ParameterInfo("@Tph_Misc1OTHr", rowEmployeePayrollTransactionExt["Tph_Misc1OTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc1NDHr", rowEmployeePayrollTransactionExt["Tph_Misc1NDHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc1NDOTHr", rowEmployeePayrollTransactionExt["Tph_Misc1NDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc2Hr", rowEmployeePayrollTransactionExt["Tph_Misc2Hr"]);
            param[i++] = new ParameterInfo("@Tph_Misc2OTHr", rowEmployeePayrollTransactionExt["Tph_Misc2OTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc2NDHr", rowEmployeePayrollTransactionExt["Tph_Misc2NDHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc2NDOTHr", rowEmployeePayrollTransactionExt["Tph_Misc2NDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc3Hr", rowEmployeePayrollTransactionExt["Tph_Misc3Hr"]);
            param[i++] = new ParameterInfo("@Tph_Misc3OTHr", rowEmployeePayrollTransactionExt["Tph_Misc3OTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc3NDHr", rowEmployeePayrollTransactionExt["Tph_Misc3NDHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc3NDOTHr", rowEmployeePayrollTransactionExt["Tph_Misc3NDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc4Hr", rowEmployeePayrollTransactionExt["Tph_Misc4Hr"]);
            param[i++] = new ParameterInfo("@Tph_Misc4OTHr", rowEmployeePayrollTransactionExt["Tph_Misc4OTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc4NDHr", rowEmployeePayrollTransactionExt["Tph_Misc4NDHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc4NDOTHr", rowEmployeePayrollTransactionExt["Tph_Misc4NDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc5Hr", rowEmployeePayrollTransactionExt["Tph_Misc5Hr"]);
            param[i++] = new ParameterInfo("@Tph_Misc5OTHr", rowEmployeePayrollTransactionExt["Tph_Misc5OTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc5NDHr", rowEmployeePayrollTransactionExt["Tph_Misc5NDHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc5NDOTHr", rowEmployeePayrollTransactionExt["Tph_Misc5NDOTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc6Hr", rowEmployeePayrollTransactionExt["Tph_Misc6Hr"]);
            param[i++] = new ParameterInfo("@Tph_Misc6OTHr", rowEmployeePayrollTransactionExt["Tph_Misc6OTHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc6NDHr", rowEmployeePayrollTransactionExt["Tph_Misc6NDHr"]);
            param[i++] = new ParameterInfo("@Tph_Misc6NDOTHr", rowEmployeePayrollTransactionExt["Tph_Misc6NDOTHr"]);
            param[i++] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            #endregion

            #region Query
            string query = @"INSERT INTO T_EmpPayTranHdrMisc
                           (Tph_IDNo
                            , Tph_PayCycle
                            , Tph_Misc1Hr
                            , Tph_Misc1OTHr
                            , Tph_Misc1NDHr
                            , Tph_Misc1NDOTHr
                            , Tph_Misc2Hr
                            , Tph_Misc2OTHr
                            , Tph_Misc2NDHr
                            , Tph_Misc2NDOTHr
                            , Tph_Misc3Hr
                            , Tph_Misc3OTHr
                            , Tph_Misc3NDHr
                            , Tph_Misc3NDOTHr
                            , Tph_Misc4Hr
                            , Tph_Misc4OTHr
                            , Tph_Misc4NDHr
                            , Tph_Misc4NDOTHr
                            , Tph_Misc5Hr
                            , Tph_Misc5OTHr
                            , Tph_Misc5NDHr
                            , Tph_Misc5NDOTHr
                            , Tph_Misc6Hr
                            , Tph_Misc6OTHr
                            , Tph_Misc6NDHr
                            , Tph_Misc6NDOTHr
                            , Usr_Login
                            , Ludatetime)
                           VALUES
                            (@Tph_IDNo
                            , @Tph_PayCycle
                            , @Tph_Misc1Hr
                            , @Tph_Misc1OTHr
                            , @Tph_Misc1NDHr
                            , @Tph_Misc1NDOTHr
                            , @Tph_Misc2Hr
                            , @Tph_Misc2OTHr
                            , @Tph_Misc2NDHr
                            , @Tph_Misc2NDOTHr
                            , @Tph_Misc3Hr
                            , @Tph_Misc3OTHr
                            , @Tph_Misc3NDHr
                            , @Tph_Misc3NDOTHr
                            , @Tph_Misc4Hr
                            , @Tph_Misc4OTHr
                            , @Tph_Misc4NDHr
                            , @Tph_Misc4NDOTHr
                            , @Tph_Misc5Hr
                            , @Tph_Misc5OTHr
                            , @Tph_Misc5NDHr
                            , @Tph_Misc5NDOTHr
                            , @Tph_Misc6Hr
                            , @Tph_Misc6OTHr
                            , @Tph_Misc6NDHr
                            , @Tph_Misc6NDOTHr
                            , @Usr_Login
                            , GETDATE())";
            #endregion

            retVal = dal.ExecuteNonQuery(query, CommandType.Text, param);

            return retVal;
        }

    }
}
