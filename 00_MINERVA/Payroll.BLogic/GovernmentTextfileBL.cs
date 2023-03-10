using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using CommonLibrary;
using Payroll.DAL;

using System.Configuration;

namespace Payroll.BLogic
{
    public class GovernmentTextfileBL : BaseBL
    {
        #region Overrides
        public override int Add(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfo = new ParameterInfo[14];
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_PayCycleMonth", row["Tgp_PayCycleMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_DeductionCode", row["Tgp_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_IDNo", row["Tgp_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_ERShare", row["Tgp_ERShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_EEShare", row["Tgp_EEShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_ECShare", row["Tgp_ECShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_MPFERShare", row["Tgp_MPFERShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_MPFEEShare", row["Tgp_MPFEEShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_SalaryBase", row["Tgp_SalaryBase"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_BaseAmount", row["Tgp_BaseAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_RemCode", row["Tgp_RemCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_RemDate", (GetValue(row["Tgp_RemDate"]) != string.Empty ? row["Tgp_RemDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_EntryType", row["Tgp_EntryType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);


            string query = @"UPDATE T_EmpGovPremRemittance 
	                         SET Tgp_ERShare = @Tgp_ERShare
	                            , Tgp_EEShare = @Tgp_EEShare
	                            , Tgp_ECShare = @Tgp_ECShare
                                , Tgp_MPFERShare = @Tgp_MPFERShare
                                , Tgp_MPFEEShare = @Tgp_MPFEEShare
                                , Tgp_SalaryBase = @Tgp_SalaryBase
	                            , Tgp_BaseAmount = @Tgp_BaseAmount
                                , Tgp_RemCode = @Tgp_RemCode 
                                , Tgp_RemDate = @Tgp_RemDate
                                , Tgp_EntryType = @Tgp_EntryType
                                , Usr_Login = @Usr_Login
                                , Ludatetime = GETDATE()
	                        WHERE Tgp_PayCycleMonth = @Tgp_PayCycleMonth
                                AND Tgp_DeductionCode = @Tgp_DeductionCode
                                AND Tgp_IDNo = @Tgp_IDNo";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
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
        public int Insert(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfo = new ParameterInfo[14];
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_PayCycleMonth", row["Tgp_PayCycleMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_DeductionCode", row["Tgp_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_IDNo", row["Tgp_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_ERShare", row["Tgp_ERShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_EEShare", row["Tgp_EEShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_ECShare", row["Tgp_ECShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_MPFERShare", row["Tgp_MPFERShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_MPFEEShare", row["Tgp_MPFEEShare"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_SalaryBase", row["Tgp_SalaryBase"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_BaseAmount", row["Tgp_BaseAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_RemCode", row["Tgp_RemCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_RemDate", (GetValue(row["Tgp_RemDate"]) != string.Empty ? row["Tgp_RemDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tgp_EntryType", row["Tgp_EntryType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);



            string query = @"INSERT INTO T_EmpGovPremRemittance (
                                                                Tgp_PayCycleMonth
                                                                , Tgp_DeductionCode
                                                                , Tgp_IDNo
                                                                , Tgp_ERShare
                                                                , Tgp_EEShare
                                                                , Tgp_ECShare
                                                                , Tgp_MPFERShare
                                                                , Tgp_MPFEEShare
                                                                , Tgp_SalaryBase
                                                                , Tgp_BaseAmount
                                                                , Tgp_RemCode
                                                                , Tgp_RemDate
                                                                , Tgp_EntryType
                                                                , Usr_Login
                                                                , Ludatetime)
                            VALUES(@Tgp_PayCycleMonth
                                    , @Tgp_DeductionCode
                                    , @Tgp_IDNo
                                    , @Tgp_ERShare
                                    , @Tgp_EEShare
                                    , @Tgp_ECShare
                                    , @Tgp_MPFERShare
                                    , @Tgp_MPFEEShare
                                    , @Tgp_SalaryBase
                                    , @Tgp_BaseAmount
                                    , @Tgp_RemCode
                                    , @Tgp_RemDate
                                    , @Tgp_EntryType
                                    , @Usr_Login
                                    , GETDATE())";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }
        public void InsertMaster(DataRow row, DALHelper dal)
        {
            #region Master
            int paramIndex = 0;
            ParameterInfo[] paramInfoMaster = new ParameterInfo[12];
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_CompanyCode", row["Msr_CompanyCode"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_IDNo", row["Msr_IDNo"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_LastName", row["Msr_LastName"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_FirstName", row["Msr_FirstName"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_MiddleName", row["Msr_MiddleName"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_MaidenName", row["Msr_MaidenName"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_SSSNo", row["Msr_SSSNo"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_PhilhealthNo", row["Msr_PhilhealthNo"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_TIN", row["Msr_TIN"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_PagIbigNo", row["Msr_PagIbigNo"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_Status", row["Msr_Status"]);
            paramInfoMaster[paramIndex++] = new ParameterInfo("@Msr_CreatedBy", row["Msr_CreatedBy"]);

            string queryMaster = @"INSERT INTO M_EmpSpecialRemittance (
                            Msr_CompanyCode
                            , Msr_IDNo
                            , Msr_LastName
                            , Msr_FirstName
                            , Msr_MiddleName
                            , Msr_MaidenName
                            , Msr_SSSNo
                            , Msr_PhilhealthNo
                            , Msr_TIN
                            , Msr_PagIbigNo
                            , Msr_Status
                            , Msr_CreatedBy
                            , Msr_CreatedDate)
                            VALUES(@Msr_CompanyCode
                            , @Msr_IDNo
                            , @Msr_LastName
                            , @Msr_FirstName
                            , @Msr_MiddleName
                            , @Msr_MaidenName
                            , @Msr_SSSNo
                            , @Msr_PhilhealthNo
                            , @Msr_TIN
                            , @Msr_PagIbigNo
                            , @Msr_Status
                            , @Msr_CreatedBy
                            , GETDATE())";
            #endregion
            dal.ExecuteNonQuery(queryMaster, CommandType.Text, paramInfoMaster);
        }
        public void InsertPremium(DataRow row, DALHelper dal)
        {
            #region Premium 
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[16];
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_IDNo", row["Tpr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSStartYearMonth", row["Tpr_SSSStartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSEndYearMonth", row["Tpr_SSSEndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSEE", row["Tpr_SSSEE"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSER", row["Tpr_SSSER"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_EC", row["Tpr_EC"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICStartYearMonth", row["Tpr_PHICStartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICEndYearMonth", row["Tpr_PHICEndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICEE", row["Tpr_PHICEE"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICER", row["Tpr_PHICER"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigStartYearMonth", row["Tpr_PagIbigStartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigEndYearMonth", row["Tpr_PagIbigEndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigEE", row["Tpr_PagIbigEE"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigER", row["Tpr_PagIbigER"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_Status", row["Tpr_Status"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_CreatedBy", row["Tpr_CreatedBy"]);

            string queryPremium = @"INSERT INTO T_EmpSpecialGovPremRemittance (
                             Tpr_IDNo
                            , Tpr_SSSStartYearMonth
                            , Tpr_SSSEndYearMonth
                            , Tpr_SSSEE
                            , Tpr_SSSER
                            , Tpr_EC
                            , Tpr_PHICStartYearMonth
                            , Tpr_PHICEndYearMonth
                            , Tpr_PHICEE
                            , Tpr_PHICER
                            , Tpr_PagIbigStartYearMonth
                            , Tpr_PagIbigEndYearMonth
                            , Tpr_PagIbigEE
                            , Tpr_PagIbigER
                            , Tpr_Status
                            , Tpr_CreatedBy
                            , Tpr_CreatedDate)
                            VALUES(@Tpr_IDNo
                            , @Tpr_SSSStartYearMonth
                            , @Tpr_SSSEndYearMonth
                            , @Tpr_SSSEE
                            , @Tpr_SSSER
                            , @Tpr_EC
                            , @Tpr_PHICStartYearMonth
                            , @Tpr_PHICEndYearMonth
                            , @Tpr_PHICEE
                            , @Tpr_PHICER
                            , @Tpr_PagIbigStartYearMonth
                            , @Tpr_PagIbigEndYearMonth
                            , @Tpr_PagIbigEE
                            , @Tpr_PagIbigER
                            , @Tpr_Status
                            , @Tpr_CreatedBy
                            , GETDATE())";
            #endregion
            dal.ExecuteNonQuery(queryPremium, CommandType.Text, paramInfo);
        }
        public void InsertLoan(DataRow row, DALHelper dal)
        {
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[10];
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_IDNo", row["Tlr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_DeductionCode", row["Tlr_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LoanType", row["Tlr_LoanType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_StartYearMonth", row["Tlr_StartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EndYearMonth", row["Tlr_EndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_AmortPaid", row["Tlr_AmortPaid"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LoanAmt", row["Tlr_LoanAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LoanDate", (GetValue(row["Tlr_LoanDate"]) != string.Empty ? row["Tlr_LoanDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_Status", row["Tlr_Status"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_CreatedBy", row["Tlr_CreatedBy"]);

            string query = @"INSERT INTO T_EmpSpecialGovLoanRemittance (
                           Tlr_IDNo
                            , Tlr_DeductionCode
                            , Tlr_LoanType
                            , Tlr_StartYearMonth
                            , Tlr_EndYearMonth
                            , Tlr_AmortPaid
                            , Tlr_LoanAmt
                            , Tlr_LoanDate
                            , Tlr_Status
                            , Tlr_CreatedBy
                            , Tlr_CreatedDate)
                            VALUES(@Tlr_IDNo
                            , @Tlr_DeductionCode
                            , @Tlr_LoanType
                            , @Tlr_StartYearMonth
                            , @Tlr_EndYearMonth
                            , @Tlr_AmortPaid
                            , @Tlr_LoanAmt
                            , @Tlr_LoanDate
                            , @Tlr_Status
                            , @Tlr_CreatedBy
                            , GETDATE())";

            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }
        public void UpdateMaster(DataRow row, DALHelper dal)
        {
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[11];
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_CompanyCode", row["Msr_CompanyCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_IDNo", row["Msr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_LastName", row["Msr_LastName"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_FirstName", row["Msr_FirstName"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_MiddleName", row["Msr_MiddleName"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_MaidenName", row["Msr_MaidenName"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_SSSNo", row["Msr_SSSNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_PhilhealthNo", row["Msr_PhilhealthNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_TIN", row["Msr_TIN"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_PagIbigNo", row["Msr_PagIbigNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Msr_UpdatedBy", row["Msr_UpdatedBy"]);

            #region Master Query
            string queryMaster = @"UPDATE M_EmpSpecialRemittance 
                                   SET Msr_LastName = @Msr_LastName
                                   , Msr_FirstName = @Msr_FirstName
                                   , Msr_MiddleName = @Msr_MiddleName 
                                   , Msr_MaidenName = @Msr_MaidenName
                                   , Msr_SSSNo = @Msr_SSSNo
                                   , Msr_PhilhealthNo = @Msr_PhilhealthNo
                                   , Msr_TIN = @Msr_TIN
                                   , Msr_PagIbigNo = @Msr_PagIbigNo
                                   , Msr_UpdatedBy = @Msr_UpdatedBy
                                   , Msr_UpdatedDate = GETDATE()
                                   WHERE Msr_IDNo = @Msr_IDNo
                                   AND Msr_CompanyCode = @Msr_CompanyCode";
            #endregion

            dal.ExecuteNonQuery(queryMaster, CommandType.Text, paramInfo);
        }
        public void UpdatePremium(DataRow row, DALHelper dal)
        {
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[10];
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_IDNo", row["Tpr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSStartYearMonth", row["Tpr_SSSStartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSEndYearMonth", row["Tpr_SSSEndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSEE", row["Tpr_SSSEE"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_SSSER", row["Tpr_SSSER"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_EC", row["Tpr_EC"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICStartYearMonth", row["Tpr_PHICStartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICEndYearMonth", row["Tpr_PHICEndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICEE", row["Tpr_PHICEE"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PHICER", row["Tpr_PHICER"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigStartYearMonth", row["Tpr_PagIbigStartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigEndYearMonth", row["Tpr_PagIbigEndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigEE", row["Tpr_PagIbigEE"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_PagIbigER", row["Tpr_PagIbigER"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tpr_UpdatedBy", row["Tpr_UpdatedBy"]);

            #region Master Query
            string queryMaster = @"UPDATE T_EmpSpecialGovPremRemittance 
                                   SET Tpr_SSSStartYearMonth = @Tpr_SSSStartYearMonth 
                                   , Tpr_SSSEndYearMonth = @Tpr_SSSEndYearMonth
                                   , Tpr_SSSEE = @Tpr_SSSEE
                                   , Tpr_SSSER = @Tpr_SSSER
                                   , Tpr_EC = @Tpr_EC
                                   , Tpr_PHICStartYearMonth = @Tpr_PHICStartYearMonth
                                   , Tpr_PHICEndYearMonth = @Tpr_PHICEndYearMonth
                                   , Tpr_PHICEE = @Tpr_PHICEE
                                   , Tpr_PHICER = @Tpr_PHICER
                                   , Tpr_PagIbigStartYearMonth = @Tpr_PagIbigStartYearMonth
                                   , Tpr_PagIbigEndYearMonth = @Tpr_PagIbigEndYearMonth
                                   , Tpr_PagIbigEE = @Tpr_PagIbigEE
                                   , Tpr_PagIbigER = @Tpr_PagIbigER
                                   , Tpr_UpdatedBy = @Tpr_UpdatedBy
                                   , Tpr_UpdatedDate = GETDATE()
                                   WHERE Tpr_IDNo = @Tpr_IDNo";
            #endregion

            dal.ExecuteNonQuery(queryMaster, CommandType.Text, paramInfo);
        }
        public void UpdateLoan(DataRow row, DALHelper dal)
        {
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[9];
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_IDNo", row["Tlr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_DeductionCode", row["Tlr_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LoanType", row["Tlr_LoanType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_StartYearMonth", row["Tlr_StartYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EndYearMonth", row["Tlr_EndYearMonth"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_AmortPaid", row["Tlr_AmortPaid"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LoanAmt", row["Tlr_LoanAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LoanDate", (GetValue(row["Tlr_LoanDate"]) != string.Empty ? row["Tlr_LoanDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_UpdatedBy", row["Tlr_UpdatedBy"]);


            string query = @"UPDATE T_EmpSpecialGovLoanRemittance 
	                        SET Tlr_StartYearMonth = @Tlr_StartYearMonth
	                        , Tlr_EndYearMonth = @Tlr_EndYearMonth
	                        , Tlr_AmortPaid = @Tlr_AmortPaid
                            , Tlr_LoanAmt = @Tlr_LoanAmt
	                        , Tlr_LoanDate = @Tlr_LoanDate
                            , Tlr_UpdatedBy = @Tlr_UpdatedBy 
                            , Tlr_UpdatedDate = GETDATE()
	                        WHERE Tlr_IDNo = @Tlr_IDNo
                            AND Tlr_DeductionCode = @Tlr_DeductionCode
                            AND Tlr_LoanType = @Tlr_LoanType";

            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }
        public int UpdateSignatory(DataRow row, string CentralProfile)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfoSignatory = new ParameterInfo[5];
            paramInfoSignatory[paramIndex++] = new ParameterInfo("@Mgr_CompanyCode", row["Mgr_CompanyCode"]);
            paramInfoSignatory[paramIndex++] = new ParameterInfo("@Mgr_RemittanceCode", row["Mgr_RemittanceCode"]);
            paramInfoSignatory[paramIndex++] = new ParameterInfo("@Mgr_SignatoryID", row["Mgr_SignatoryID"]);
            paramInfoSignatory[paramIndex++] = new ParameterInfo("@Mgr_SignatoryPosition", row["Mgr_SignatoryPosition"]);
            paramInfoSignatory[paramIndex++] = new ParameterInfo("@Mgr_UpdatedBy", row["Mgr_UpdatedBy"]);

            string query = @"UPDATE M_GovRemittance 
	                        SET Mgr_SignatoryID = @Mgr_SignatoryID
	                            , Mgr_SignatoryPosition = @Mgr_SignatoryPosition
	                            , Mgr_UpdatedBy = @Mgr_UpdatedBy
	                            , Mgr_UpdatedDate = GETDATE()
	                        WHERE Mgr_CompanyCode = @Mgr_CompanyCode
                                AND Mgr_RemittanceCode = @Mgr_RemittanceCode";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(query, CommandType.Text, paramInfoSignatory);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }
        public int UpdateSBR(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfoSBR = new ParameterInfo[8];
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_CompanyCode", row["Tms_CompanyCode"]);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_YearMonth", row["Tms_YearMonth"]);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_DeductionCode", row["Tms_DeductionCode"]);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_ReceiptNo", row["Tms_ReceiptNo"]);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_SPANo", row["Tms_SPANo"]);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_PaymentDate", (GetValue(row["Tms_PaymentDate"]) != string.Empty ? row["Tms_PaymentDate"] : DBNull.Value), SqlDbType.Date);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Tms_PaidAmount", row["Tms_PaidAmount"]);
            paramInfoSBR[paramIndex++] = new ParameterInfo("@Usr_login", row["Usr_login"]);

            string query = @"IF EXISTS(SELECT * FROM T_MonthlySBR
		                                WHERE Tms_YearMonth = @Tms_YearMonth
		                                    AND Tms_DeductionCode = @Tms_DeductionCode
                                            AND Tms_CompanyCode = @Tms_CompanyCode)
                            BEGIN
	                            UPDATE T_MonthlySBR 
	                            SET Tms_ReceiptNo = @Tms_ReceiptNo
                                , Tms_SPANo = @Tms_SPANo
	                            , Tms_PaymentDate = @Tms_PaymentDate
	                            , Tms_PaidAmount = @Tms_PaidAmount
	                            , Usr_login = @Usr_login
                                , Ludatetime = GETDATE()
	                            WHERE Tms_YearMonth = @Tms_YearMonth
	                                AND Tms_DeductionCode = @Tms_DeductionCode
                                    AND Tms_CompanyCode = @Tms_CompanyCode
                            END
                            ELSE
                            BEGIN
	                            INSERT INTO T_MonthlySBR
	                            (Tms_CompanyCode
                                ,Tms_YearMonth
	                            ,Tms_DeductionCode
	                            ,Tms_ReceiptNo
	                            ,Tms_SPANo
	                            ,Tms_PaymentDate
	                            ,Tms_PaidAmount
	                            ,Usr_login
	                            ,Ludatetime)
	                            VALUES (@Tms_CompanyCode
                                , @Tms_YearMonth
	                            , @Tms_DeductionCode
	                            , @Tms_ReceiptNo
	                            , @Tms_SPANo
	                            , @Tms_PaymentDate
	                            , @Tms_PaidAmount
	                            , @Usr_login
	                            , GETDATE())
                            END";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(query, CommandType.Text, paramInfoSBR);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }
        public int Delete(string Remittance, string ApplicableMonth, string IDNumber, string LoanType, string CentralProfile)
        {
            int retVal = 0;

            ParameterInfo[] param = null;
            string query = string.Empty;
            if (Remittance.Contains("PREM"))
            {
                #region Parameters
                param = new ParameterInfo[3];
                param[0] = new ParameterInfo("@Tgp_PayCycleMonth", ApplicableMonth);
                param[1] = new ParameterInfo("@Tgp_DeductionCode", Remittance);
                param[2] = new ParameterInfo("@Tgp_IDNo", IDNumber);
                #endregion

                #region query
                query = string.Format(@"DELETE FROM T_EmpGovPremRemittance
                                 WHERE Tgp_PayCycleMonth = @Tgp_PayCycleMonth
                                 AND Tgp_DeductionCode = @Tgp_DeductionCode
                                 AND Tgp_IDNo = @Tgp_IDNo");
                #endregion
            }
            else if (Remittance.Contains("LOAN"))
            {
                #region Parameters
                param = new ParameterInfo[4];
                param[0] = new ParameterInfo("@Tgl_PayCycleMonth", ApplicableMonth);
                param[1] = new ParameterInfo("@Tgl_DeductionCode", Remittance);
                param[2] = new ParameterInfo("@Tgl_IDNo", IDNumber);
                param[3] = new ParameterInfo("@Tgl_LoanType", LoanType);
                #endregion

                #region query
                query = string.Format(@"DELETE FROM T_EmpGovLoanRemittance
                                 WHERE Tgl_PayCycleMonth = @Tgl_PayCycleMonth
                                 AND Tgl_DeductionCode = @Tgl_DeductionCode
                                 AND Tgl_LoanType = @Tgl_LoanType
                                 AND Tgl_IDNo = @Tgl_IDNo");
                #endregion
            }

            if (query != string.Empty)
            {
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();

                    try
                    {
                        retVal = dal.ExecuteNonQuery(query, CommandType.Text, param);
                        dal.CommitTransactionSnapshot();
                    }
                    catch (Exception e)
                    {
                        dal.RollBackTransactionSnapshot();
                        throw new PayrollException(e);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }

            return retVal;
        }
        public int DeleteSpecialRemittance(string IDNumber, string CentralProfile)
        {
            int retVal = 0;

            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@IDNo", IDNumber);
            
            #region query
                string query = string.Format(@"
                                 DELETE FROM T_EmpSpecialGovPremRemittance
                                 WHERE Tpr_IDNo = @IDNo

                                 DELETE FROM T_EmpSpecialGovLoanRemittance
                                 WHERE Tlr_IDNo = @IDNo

                                 DELETE FROM M_EmpSpecialRemittance
                                 WHERE Msr_IDNo = @IDNo
                                ");
                #endregion
            
            
            if (query != string.Empty)
            {
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();

                    try
                    {
                        retVal = dal.ExecuteNonQuery(query, CommandType.Text, param);
                        dal.CommitTransactionSnapshot();
                    }
                    catch (Exception e)
                    {
                        dal.RollBackTransactionSnapshot();
                        throw new PayrollException(e);
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }

            return retVal;
        }
        public bool CheckIfExistsinEmployeeMaster(string ColName, string EmployeeID, string Value)
        {
            DataSet dsExist;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dsExist = dal.ExecuteDataSet(string.Format(@"SELECT
                                                                Mem_IDNo
                                                            FROM M_Employee
                                                            WHERE {0} = '{1}'
                                                                AND Mem_WorkStatus <> 'IN'"
                                                            , ColName
                                                            , Value));
                dal.CloseDB();
            }

            if (dsExist.Tables[0].Rows.Count > 0)
            {
                if (dsExist.Tables[0].Rows[0][0].ToString() != EmployeeID)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public DataTable FetchAll(string CompanyCode, string CentralProfile)
        {
            DataTable dt = new DataTable();
            #region query
            string query = string.Format(@"SELECT  Msr_IDNo [ID Number]
                            , Msr_LastName [Last Name]
                            , Msr_FirstName [First Name] 
                            , Msr_MiddleName [Middle Name]
                            , Msr_MaidenName [Maiden Name]
                            , Msr_LastName + ', ' 
		                                + Msr_FirstName + ' '
		                                + Msr_MiddleName [Employee Name]
                            , Msr_SSSNo [SSS Number]
                            , Msr_PhilhealthNo [PHIC Number]
                            , Msr_TIN [TIN Number]
                            , Msr_PagIbigNo [HDMF Number]
                            FROM M_EmpSpecialRemittance 
                            WHERE Msr_Status = 'A'
                                AND Msr_CompanyCode = '{0}'", CompanyCode);
            #endregion
            if (query != string.Empty)
            {
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                    dal.CloseDB();
                }
            }
            return dt;
        }
        public DataTable FetchSelected(string IDNumber)
        {
            DataTable dt = new DataTable();
            #region query
            string query = string.Format(@"SELECT  Msr_IDNo [ID Number]
                            , Msr_LastName [Last Name]
                            , Msr_FirstName [First Name] 
                            , Msr_MiddleName [Middle Name]
                            , Msr_MaidenName [Maiden Name]
                            , Msr_LastName + ', ' 
		                                + Msr_FirstName + ' '
		                                + Msr_MiddleName [Employee Name]
                            , Msr_SSSNo [SSS Number]
                            , Msr_PhilhealthNo [PHIC Number]
                            , Msr_TIN [TIN Number]
                            , Msr_PagIbigNo [HDMF Number]
                            FROM M_EmpSpecialRemittance 
                            WHERE Msr_Status = 'A'
                            AND Msr_IDNo = '{0}'", IDNumber);
            #endregion
            if (query != string.Empty)
            {
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                    dal.CloseDB();
                }
            }
            return dt;
        }
        public DataTable FetchPremiumSelected(string IDNumber)
        {
            DataTable dt = new DataTable();
            #region query
            string query = string.Format(@"SELECT  Msr_IDNo [ID Number]
                            , Msr_LastName [Last Name]
                            , Msr_FirstName [First Name] 
                            , Msr_MiddleName [Middle Name]
                            , Msr_MaidenName [Maiden Name]
                            , Msr_LastName + ', ' 
		                                + Msr_FirstName + ' '
		                                + Msr_MiddleName [Employee Name]
                            , Msr_SSSNo [SSS Number]
                            , Msr_PhilhealthNo [PHIC Number]
                            , Msr_TIN [TIN Number]
                            , Msr_PagIbigNo [HDMF Number]
                            FROM M_EmpSpecialRemittance 
                            WHERE Msr_Status = 'A'
                            AND Msr_IDNo = '{0}'", IDNumber);
            #endregion
            if (query != string.Empty)
            {
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                    dal.CloseDB();
                }
            }
            return dt;
        }

        public DataTable FetchLoan(string LoanCode, string CentralProfile)
        {
            DataTable dt = new DataTable();
            #region query
            string query = string.Format(@"SELECT Tlr_IDNo [ID Number]
                            , Tlr_LoanType [Loan Type Code] 
                            , Tlr_StartYearMonth [Start Year Month]
                            , Tlr_EndYearMonth [End Year Month]
                            , ISNULL(Tlr_AmortPaid, 0.00) [Amort Paid]
                            , ISNULL(Tlr_LoanAmt, 0.00) [Loan Amt]
                            , CONVERT(VARCHAR(20), Tlr_LoanDate, 101) [Loan Date]
                            FROM T_EmpSpecialGovLoanRemittance 
                            WHERE Tlr_Status = 'A'
                            AND Tlr_DeductionCode = '{0}'", LoanCode);
            #endregion
            if (query != string.Empty)
            {
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                    dal.CloseDB();
                }
            }
            return dt;
        }
        public bool SpecialPremiumExists(string Code, string Value, string IDNumber, string CompanyCode, string CentralProfile)
        {
            DataSet dsExist;
            string ColName = string.Empty;
            switch(Code)
            {
                case "SSS":
                    ColName = "Msr_SSSNo";
                    break;
                case "PH":
                    ColName = "Msr_PhilhealthNo";
                    break;
                case "HDMF":
                    ColName = "Msr_PagIbigNo";
                    break;
                case "TIN":
                    ColName = "Msr_TIN";
                    break;
            }

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                dsExist = dal.ExecuteDataSet(string.Format(@"SELECT
                                                                Msr_IDNo
                                                            FROM M_EmpSpecialRemittance
                                                            WHERE {0} = '{1}'
                                                                AND Msr_CompanyCode = '{2}'"
                                                            , ColName
                                                            , Value
                                                            , CompanyCode));
                dal.CloseDB();
            }

            if (dsExist.Tables[0].Rows.Count > 0)
            {
                if (dsExist.Tables[0].Rows[0][0].ToString() != IDNumber)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool SpecialEmployeeExists(string EmployeeID, string CompanyCode, string CentralProfile)
        {
            DataSet dsExist;


            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                dsExist = dal.ExecuteDataSet(string.Format(@"SELECT
                                                                Msr_IDNo
                                                            FROM M_EmpSpecialRemittance
                                                            WHERE Msr_IDNo = '{0}'
                                                                AND Msr_CompanyCode = '{1}'"
                                                            , EmployeeID, CompanyCode));
                dal.CloseDB();
            }

            if (dsExist.Tables[0].Rows.Count > 0)
            {
                return true; 
            }
            else
                return false;
        }

        //public DataSet GetPremiumSchedule(string YearMonth, string CompanyCode, string CentralProfile)
        //{
        //    #region query
        //    string query = string.Format(@"SELECT * FROM M_PremiumSchedule
        //                                   WHERE Mps_PayCycle = (SELECT MAX(Mps_PayCycle) as [PayCycle]
								//							     FROM M_PremiumSchedule
								//							     WHERE Mps_PayCycle <= '{0}'            
								//									AND Mps_CompanyCode = '{1}'
								//									AND Mps_RecordStatus = 'A')
        //                                        AND Mps_CompanyCode = '{1}' ", YearMonth, CompanyCode);
        //    #endregion

        //    DataSet ds = null;
        //    using (DALHelper dal = new DALHelper(CentralProfile, false))
        //    {
        //        dal.OpenDB();

        //        ds = dal.ExecuteDataSet(query);
        //        dal.CloseDB();
        //    }

        //    return ds;
        //}

        public DataSet GetPremiumSchedule(string PremCode, string YearMonth, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT * FROM M_PremiumSchedule
										   WHERE Mps_DeductionCode = '{0}'
										        AND Mps_PayCycle = (SELECT MAX(PayCycle.Mps_PayCycle)
															        FROM M_PremiumSchedule PayCycle
															        WHERE Mps_PayCycle <= '{1}2'
                                                                        AND Mps_DeductionCode = '{0}'
															            AND Mps_CompanyCode = '{2}')
										        AND Mps_CompanyCode = '{2}' ", PremCode, YearMonth, CompanyCode);
            #endregion

            DataSet ds = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetPremiumSchedule(decimal BaseAmount, string PremCode, string YearMonth, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @MAXPAYCYCLE CHAR(7) = (SELECT MAX(Mps_Paycycle) 
                                                                        FROM M_PremiumSchedule 
                                                                        WHERE Mps_PayCycle <= '{1}2'
									                                        AND Mps_CompanyCode = '{2}'													
						                                                    AND Mps_DeductionCode ='{0}'
                                                                            AND Mps_RecordStatus = 'A')
																		
                                        DECLARE @SALARY DECIMAL(9,2) = '{3}'																																								
                                        SELECT * FROM M_PremiumSchedule													
                                        WHERE Mps_CompanyCode = '{2}'												
	                                        AND Mps_DeductionCode ='{0}'													
	                                        AND Mps_PayCycle = @MAXPAYCYCLE
	                                        AND @SALARY BETWEEN Mps_CompensationFrom AND Mps_CompensationTo ", PremCode, YearMonth, CompanyCode, BaseAmount);
            #endregion

            
            try
            {
                DataSet ds = null;
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(query);
                    dal.CloseDB();
                }
                return ds;
            }
            catch
            {
                return null;
            }
        }

    }
}
