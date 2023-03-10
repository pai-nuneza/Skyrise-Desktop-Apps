using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Configuration;

namespace Payroll.BLogic
{
    public class IndividualDeductionEntryBL : BaseBL
    {
        #region <Override Functions>

        public override int Add(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfo = new ParameterInfo[18];
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_EndDate", row["Tdh_EndDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionAmount", row["Tdh_DeductionAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", row["Tdh_PaidAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_CycleAmount", row["Tdh_CycleAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeferredAmount", row["Tdh_DeferredAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_CheckDate", row["Tdh_CheckDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_VoucherNumber", row["Tdh_VoucherNumber"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", row["Tdh_PaymentDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_ExemptInPayroll", row["Tdh_ExemptInPayroll"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PrincipalAmount", row["Tdh_PrincipalAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_ApplicablePayCycle", row["Tdh_ApplicablePayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_AgreementNo", (row["Tdh_AgreementNo"] == null || row["Tdh_AgreementNo"].ToString() == "") ? DBNull.Value : row["Tdh_AgreementNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_EntryDate", row["Tdh_EntryDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentTerms", row["Tdh_PaymentTerms"]);

            string LastSeqNo = GetLastSeqNoInTrail(row["Tdh_IDNo"].ToString(), row["Tdh_DeductionCode"].ToString(), row["Tdh_StartDate"].ToString(), CentralProfile);

            ParameterInfo[] InsparamInfo = new ParameterInfo[5];
            paramIndex = 0;
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_LineNo", LastSeqNo);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_RecordStatus", "A");

            string sqlQuery = string.Format(
                                        @"INSERT INTO {0}..T_EmpDeductionHdr
                                           (Tdh_IDNo
                                           ,Tdh_DeductionCode
                                           ,Tdh_StartDate
                                           ,Tdh_EndDate
                                           ,Tdh_DeductionAmount
                                           ,Tdh_PaidAmount
                                           ,Tdh_CycleAmount
                                           ,Tdh_DeferredAmount
                                           ,Tdh_CheckDate
                                           ,Tdh_VoucherNumber
                                           ,Tdh_PaymentDate
                                           ,Usr_Login
                                           ,Ludatetime
                                           ,Tdh_ExemptInPayroll
                                           ,Tdh_PrincipalAmount
                                           ,Tdh_ApplicablePayCycle
                                           ,Tdh_AgreementNo
                                           ,Tdh_EntryDate
                                           ,Tdh_PaymentTerms)
                                     VALUES
                                           (@Tdh_IDNo
                                           ,@Tdh_DeductionCode
                                           ,@Tdh_StartDate
                                           ,@Tdh_EndDate
                                           ,@Tdh_DeductionAmount
                                           ,@Tdh_PaidAmount
                                           ,@Tdh_CycleAmount
                                           ,@Tdh_DeferredAmount
                                           ,@Tdh_CheckDate
                                           ,@Tdh_VoucherNumber
                                           ,@Tdh_PaymentDate
                                           ,@Usr_Login
                                           ,Getdate()
                                           ,@Tdh_ExemptInPayroll
                                           ,@Tdh_PrincipalAmount
                                           ,@Tdh_ApplicablePayCycle
                                           ,@Tdh_AgreementNo
                                           ,@Tdh_EntryDate
                                           ,@Tdh_PaymentTerms)", CentralProfile);

            string sqlInsertTrail = string.Format(
                                        @"INSERT INTO {0}..T_EmpDeductionHdrTrl
                                               (Tdh_IDNo
                                               ,Tdh_DeductionCode
                                               ,Tdh_StartDate
                                               ,Tdh_EndDate
                                               ,Tdh_LineNo
                                               ,Tdh_DeductionAmount
                                               ,Tdh_PaidAmount
                                               ,Tdh_CycleAmount
                                               ,Tdh_DeferredAmount
                                               ,Tdh_CheckDate
                                               ,Tdh_VoucherNumber
                                               ,Tdh_PaymentDate
                                               ,Tdh_RecordStatus
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdh_ExemptInPayroll
                                               ,Tdh_PrincipalAmount
                                               ,Tdh_ApplicablePayCycle
                                               ,Tdh_AgreementNo
                                               ,Tdh_EntryDate
                                               ,Tdh_DocumentNo
                                               ,Tdh_PaymentTerms)
                                         SELECT
                                               Tdh_IDNo
		                                        , Tdh_DeductionCode
		                                        , Tdh_StartDate
		                                        , Tdh_EndDate
		                                        , @Tdh_LineNo
		                                        , Tdh_DeductionAmount
		                                        , Tdh_PaidAmount
		                                        , Tdh_CycleAmount
		                                        , Tdh_DeferredAmount
		                                        , Tdh_CheckDate
		                                        , Tdh_VoucherNumber
		                                        , Tdh_PaymentDate
		                                        , @Tdh_RecordStatus
		                                        , Usr_Login
		                                        , Ludatetime
                                                , Tdh_ExemptInPayroll
                                                , Tdh_PrincipalAmount
                                                , Tdh_ApplicablePayCycle
                                                , Tdh_AgreementNo
                                                , Tdh_EntryDate
                                                , Tdh_DocumentNo
                                                , Tdh_PaymentTerms
                                                FROM {0}..T_EmpDeductionHdr
                                                WHERE Tdh_IDNo = @Tdh_IDNo
                                                  And Tdh_DeductionCode = @Tdh_DeductionCode
                                                  And Tdh_StartDate = @Tdh_StartDate", CentralProfile);

            string updateQuery = string.Format(@"
                                            UPDATE {0}..T_DocumentNumber
                                            SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
                                            WHERE Tdn_DocumentCode = 'DEDDOCNUM'
                                                AND Tdn_CompanyCode = '{1}'

                                            UPDATE {0}..T_EmpDeductionHdr
											SET Tdh_DocumentNo = (SELECT Tdn_DocumentPrefix 
                                                                  + RIGHT('00000000000000' + CAST(Tdn_LastSeriesNumber AS VARCHAR), 14)
                                                                 FROM {0}..T_DocumentNumber
                                                                 WITH (UPDLOCK)
                                                                 WHERE Tdn_DocumentCode = 'DEDDOCNUM'
                                                                    AND Tdn_CompanyCode = '{1}' )
											WHERE Tdh_IDNo = @Tdh_IDNo
                                               AND Tdh_DeductionCode = @Tdh_DeductionCode
                                               AND Tdh_StartDate = @Tdh_StartDate"
                                            , CentralProfile
                                            , LoginInfo.getUser().CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
                    retVal = dal.ExecuteNonQuery(updateQuery, CommandType.Text, paramInfo);
                    retVal = dal.ExecuteNonQuery(sqlInsertTrail, CommandType.Text, InsparamInfo);
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

        #endregion

        #region <Functions Added>

        public int Update(DataRow row, DataRow Insrow)
        {
            int retVal = 0;
            int paramIndex = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfo = new ParameterInfo[20];
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_EndDate", row["Tdh_EndDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionAmount", row["Tdh_DeductionAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", row["Tdh_PaidAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_CycleAmount", row["Tdh_CycleAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeferredAmount", row["Tdh_DeferredAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_CheckDate", row["Tdh_CheckDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_VoucherNumber", row["Tdh_VoucherNumber"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", row["Tdh_PaymentDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", DateTime.Now);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_ExemptInPayroll", row["Tdh_ExemptInPayroll"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PrincipalAmount", row["Tdh_PrincipalAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_ApplicablePayCycle", row["Tdh_ApplicablePayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_AgreementNo", (row["Tdh_AgreementNo"] == null || row["Tdh_AgreementNo"].ToString() == "") ? DBNull.Value : row["Tdh_AgreementNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_EntryDate", row["Tdh_EntryDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DocumentNo", row["Tdh_DocumentNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentTerms", row["Tdh_PaymentTerms"]);
            

            string LastSeqNo = GetLastSeqNoInTrail(row["Tdh_IDNo"].ToString(), row["Tdh_DeductionCode"].ToString(), row["Tdh_StartDate"].ToString(), CentralProfile);

            ParameterInfo[] InsparamInfo = new ParameterInfo[22];
            paramIndex = 0;
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", Insrow["Tdh_IDNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", Insrow["Tdh_DeductionCode"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", Insrow["Tdh_StartDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_EndDate", Insrow["Tdh_EndDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_LineNo", LastSeqNo);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionAmount", Insrow["Tdh_DeductionAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", Insrow["Tdh_PaidAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_CycleAmount", Insrow["Tdh_CycleAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeferredAmount", Insrow["Tdh_DeferredAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_CheckDate", Insrow["Tdh_CheckDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_VoucherNumber", Insrow["Tdh_VoucherNumber"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", Insrow["Tdh_PaymentDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_RecordStatus", "E");
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_ExemptInPayroll", Insrow["Tdh_ExemptInPayroll"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PrincipalAmount", Insrow["Tdh_PrincipalAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_ApplicablePayCycle", Insrow["Tdh_ApplicablePayCycle"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Usr_Login", Insrow["Usr_Login"].ToString()); 
            InsparamInfo[paramIndex++] = new ParameterInfo("@Ludatetime", DateTime.Now);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_AgreementNo", (Insrow["Tdh_AgreementNo"] == null || Insrow["Tdh_AgreementNo"].ToString() == "") ? DBNull.Value : Insrow["Tdh_AgreementNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_EntryDate", row["Tdh_EntryDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DocumentNo", row["Tdh_DocumentNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentTerms", row["Tdh_PaymentTerms"]);

            string sqlInsert = @"INSERT INTO T_EmpDeductionHdrTrl
                                               (Tdh_IDNo
                                               ,Tdh_DeductionCode
                                               ,Tdh_StartDate
                                               ,Tdh_EndDate
                                               ,Tdh_LineNo
                                               ,Tdh_DeductionAmount
                                               ,Tdh_PaidAmount
                                               ,Tdh_CycleAmount
                                               ,Tdh_DeferredAmount
                                               ,Tdh_CheckDate
                                               ,Tdh_VoucherNumber
                                               ,Tdh_PaymentDate
                                               ,Tdh_RecordStatus
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdh_ExemptInPayroll
                                               ,Tdh_PrincipalAmount
                                               ,Tdh_ApplicablePayCycle
                                               ,Tdh_AgreementNo
                                               ,Tdh_EntryDate
                                               ,Tdh_DocumentNo
                                               ,Tdh_PaymentTerms)
                                         VALUES
                                               (@Tdh_IDNo
		                                        ,@Tdh_DeductionCode
		                                        ,@Tdh_StartDate
		                                        ,@Tdh_EndDate
		                                        ,@Tdh_LineNo
		                                        ,@Tdh_DeductionAmount
		                                        ,@Tdh_PaidAmount
		                                        ,@Tdh_CycleAmount
		                                        ,@Tdh_DeferredAmount
		                                        ,@Tdh_CheckDate
		                                        ,@Tdh_VoucherNumber
		                                        ,@Tdh_PaymentDate
		                                        ,@Tdh_RecordStatus
		                                        ,@Usr_Login
		                                        ,@Ludatetime
                                                ,@Tdh_ExemptInPayroll
                                                ,@Tdh_PrincipalAmount
                                                ,@Tdh_ApplicablePayCycle
                                                ,@Tdh_AgreementNo
                                                ,@Tdh_EntryDate
                                                ,@Tdh_DocumentNo
                                                ,@Tdh_PaymentTerms)";

           
            string sqlUpdate = @"UPDATE T_EmpDeductionHdr
                                               SET Tdh_PaidAmount = @Tdh_PaidAmount
                                                  ,Tdh_EndDate = @Tdh_EndDate
                                                  ,Tdh_DeferredAmount = @Tdh_DeferredAmount
                                                  ,Tdh_CycleAmount = @Tdh_CycleAmount
                                                  ,Tdh_DeductionAmount = @Tdh_DeductionAmount
                                                  ,Tdh_PaymentDate = @Tdh_PaymentDate
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = @Ludatetime
                                                  ,Tdh_CheckDate = @Tdh_CheckDate
                                                  ,Tdh_VoucherNumber = @Tdh_VoucherNumber
                                                  ,Tdh_ExemptInPayroll = @Tdh_ExemptInPayroll
                                                  ,Tdh_PrincipalAmount = @Tdh_PrincipalAmount
                                                  ,Tdh_ApplicablePayCycle = @Tdh_ApplicablePayCycle
                                                  ,Tdh_AgreementNo = @Tdh_AgreementNo
                                                  ,Tdh_EntryDate = @Tdh_EntryDate
                                                  ,Tdh_DocumentNo = @Tdh_DocumentNo
                                                  ,Tdh_PaymentTerms = @Tdh_PaymentTerms
                                             WHERE Tdh_IDNo = @Tdh_IDNo
                                                  And Tdh_DeductionCode = @Tdh_DeductionCode
                                                  And Tdh_StartDate = @Tdh_StartDate";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    //insert records
                    retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, InsparamInfo);
                    //update records
                    retVal = dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
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

        public int UpdateStopResumeDeduction(DataRow row, DataRow Insrow)
        {
            int retVal = 0;
            int paramIndex = 0;
            string s = Insrow["Tdh_IDNo"].ToString();
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfo = new ParameterInfo[20];
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_EndDate", row["Tdh_EndDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionAmount", row["Tdh_DeductionAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", row["Tdh_PaidAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_CycleAmount", row["Tdh_CycleAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeferredAmount", row["Tdh_DeferredAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_CheckDate", row["Tdh_CheckDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_VoucherNumber", row["Tdh_VoucherNumber"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", row["Tdh_PaymentDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", DateTime.Now);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_ExemptInPayroll", row["Tdh_ExemptInPayroll"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PrincipalAmount", row["Tdh_PrincipalAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_ApplicablePayCycle", row["Tdh_ApplicablePayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_AgreementNo", (row["Tdh_AgreementNo"] == null || row["Tdh_AgreementNo"].ToString() == "") ? DBNull.Value : row["Tdh_AgreementNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_EntryDate", row["Tdh_EntryDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DocumentNo", row["Tdh_DocumentNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentTerms", row["Tdh_PaymentTerms"]);


            string LastSeqNo = GetLastSeqNoInTrail(row["Tdh_IDNo"].ToString(), row["Tdh_DeductionCode"].ToString(), row["Tdh_StartDate"].ToString(), CentralProfile);

            ParameterInfo[] InsparamInfo = new ParameterInfo[22];
            paramIndex = 0;
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", Insrow["Tdh_IDNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", Insrow["Tdh_DeductionCode"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", Insrow["Tdh_StartDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_EndDate", Insrow["Tdh_EndDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_LineNo", LastSeqNo);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionAmount", Insrow["Tdh_DeductionAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", Insrow["Tdh_PaidAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_CycleAmount", Insrow["Tdh_CycleAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DeferredAmount", Insrow["Tdh_DeferredAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_CheckDate", Insrow["Tdh_CheckDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_VoucherNumber", Insrow["Tdh_VoucherNumber"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", Insrow["Tdh_PaymentDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_RecordStatus", "E");
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_ExemptInPayroll", Insrow["Tdh_ExemptInPayroll"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PrincipalAmount", Insrow["Tdh_PrincipalAmount"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_ApplicablePayCycle", Insrow["Tdh_ApplicablePayCycle"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Usr_Login", Insrow["Usr_Login"].ToString());
            InsparamInfo[paramIndex++] = new ParameterInfo("@Ludatetime", DateTime.Now);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_AgreementNo", (Insrow["Tdh_AgreementNo"] == null || Insrow["Tdh_AgreementNo"].ToString() == "") ? DBNull.Value : Insrow["Tdh_AgreementNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_EntryDate", row["Tdh_EntryDate"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_DocumentNo", row["Tdh_DocumentNo"]);
            InsparamInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentTerms", row["Tdh_PaymentTerms"]);

            string sqlInsert = @"INSERT INTO T_EmpDeductionHdrTrl
                                               (Tdh_IDNo
                                               ,Tdh_DeductionCode
                                               ,Tdh_StartDate
                                               ,Tdh_EndDate
                                               ,Tdh_LineNo
                                               ,Tdh_DeductionAmount
                                               ,Tdh_PaidAmount
                                               ,Tdh_CycleAmount
                                               ,Tdh_DeferredAmount
                                               ,Tdh_CheckDate
                                               ,Tdh_VoucherNumber
                                               ,Tdh_PaymentDate
                                               ,Tdh_RecordStatus
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdh_ExemptInPayroll
                                               ,Tdh_PrincipalAmount
                                               ,Tdh_ApplicablePayCycle
                                               ,Tdh_AgreementNo
                                               ,Tdh_EntryDate
                                               ,Tdh_DocumentNo
                                               ,Tdh_PaymentTerms)
                                         VALUES
                                               (@Tdh_IDNo
		                                        ,@Tdh_DeductionCode
		                                        ,@Tdh_StartDate
		                                        ,@Tdh_EndDate
		                                        ,@Tdh_LineNo
		                                        ,@Tdh_DeductionAmount
		                                        ,@Tdh_PaidAmount
		                                        ,@Tdh_CycleAmount
		                                        ,@Tdh_DeferredAmount
		                                        ,@Tdh_CheckDate
		                                        ,@Tdh_VoucherNumber
		                                        ,@Tdh_PaymentDate
		                                        ,@Tdh_RecordStatus
		                                        ,@Usr_Login
		                                        ,@Ludatetime
                                                ,@Tdh_ExemptInPayroll
                                                ,@Tdh_PrincipalAmount
                                                ,@Tdh_ApplicablePayCycle
                                                ,@Tdh_AgreementNo
                                                ,@Tdh_EntryDate
                                                ,@Tdh_DocumentNo
                                                ,@Tdh_PaymentTerms)";


            string sqlUpdate = @"UPDATE T_EmpDeductionHdr
                                               SET Tdh_PaidAmount = @Tdh_PaidAmount
                                                  ,Tdh_EndDate = @Tdh_EndDate
                                                  ,Tdh_DeferredAmount = @Tdh_DeferredAmount
                                                  ,Tdh_CycleAmount = @Tdh_CycleAmount
                                                  ,Tdh_DeductionAmount = @Tdh_DeductionAmount
                                                  ,Tdh_PaymentDate = @Tdh_PaymentDate
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = @Ludatetime
                                                   --added by kevin - added fields
                                                  ,Tdh_CheckDate = @Tdh_CheckDate
                                                  ,Tdh_VoucherNumber = @Tdh_VoucherNumber
                                                  ,Tdh_ExemptInPayroll = @Tdh_ExemptInPayroll
                                                  ,Tdh_PrincipalAmount = @Tdh_PrincipalAmount
                                                  ,Tdh_ApplicablePayCycle = @Tdh_ApplicablePayCycle
                                                  ,Tdh_AgreementNo = @Tdh_AgreementNo
                                                  ,Tdh_EntryDate = @Tdh_EntryDate
                                                  ,Tdh_DocumentNo = @Tdh_DocumentNo
                                                  ,Tdh_PaymentTerms = @Tdh_PaymentTerms
                                             WHERE Tdh_IDNo = @Tdh_IDNo
                                                  And Tdh_DeductionCode = @Tdh_DeductionCode
                                                  And Tdh_StartDate = @Tdh_StartDate";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    //insert records
                    retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, InsparamInfo);
                    //update records
                    retVal = dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
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

        public DataSet FetchRecord2(string Tdh_IDNo, string UserLogin, string CompanyCode, string CentralProfile, string NAMEDSPLY)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);


            string sqlQuery = @"SELECT Tdh_IDNo
                                      ,Tdh_DeductionCode
                                      ,Tdh_StartDate
                                      ,Tdh_EndDate
                                      ,{0} AS Tdh_DeductionAmount
                                      ,{1} AS Tdh_PaidAmount
                                      ,{2} AS Tdh_CycleAmount
                                      ,{3} AS Tdh_DeferredAmount
                                      ,Tdh_CheckDate
                                      ,Tdh_VoucherNumber
                                      ,Tdh_PaymentDate
                                      ,Mdn_DeductionName
                                      ,{4} AS Balance
                                      ,ISNULL([Tdh_PrincipalAmount], 0) AS Tdh_PrincipalAmount
                                      ,ISNULL([Tdh_ApplicablePayCycle],'') AS Tdh_ApplicablePayCycle
                                      ,PAYFREQ.Mcd_Name [ApplicablePayCycleName]
                                      ,Tdh_ExemptInPayroll
                                      ,Tdh_AgreementNo
                                      ,Tdh_DocumentNo
                                      ,Tdh_EntryDate
                                      ,Tdh_PaymentTerms
                                      ,0 AS IsHist
                                      ,{5} AS RemainingCycles
                                      ,Mdn_IsSystemReserved
                                      ,Mdn_DeductionGroup
                                      ,ISNULL(Mdn_IsRecurring,0) AS Mdn_IsRecurring
                                      ,ISNULL(Mdn_WithCheckDate,0) AS Mdn_WithCheckDate
                                      ,ISNULL(Mdn_WithAccountingVoucher,0) AS Mdn_WithAccountingVoucher
                                      ,Mdn_PaymentTerms
                                      ,ISNULL(Mdn_WithPrincipalAmount,0) AS Mdn_WithPrincipalAmount
                                      ,Mdn_PaidUpAmount
                                      ,ISNULL({6}.dbo.Udf_DisplayName(T_EmpDeductionHdr.Usr_Login,'{7}'),T_EmpDeductionHdr.Usr_Login) AS Usr_Login
                                      ,T_EmpDeductionHdr.Ludatetime
                                FROM T_EmpDeductionHdr
                                INNER JOIN M_Deduction ON Tdh_DeductionCode = Mdn_DeductionCode
                                    AND Mdn_CompanyCode = @CompanyCode
                                LEFT JOIN M_CodeDtl PAYFREQ 
								    ON PAYFREQ.Mcd_Code = ISNULL([Tdh_ApplicablePayCycle],'')
								    AND PAYFREQ.Mcd_CodeType = 'PAYFREQ'
                                    AND PAYFREQ.Mcd_CompanyCode = @CompanyCode
                                WHERE Tdh_IDNo = @Tdh_IDNo 

                                UNION ALL

                                SELECT Tdh_IDNo
                                      ,Tdh_DeductionCode
                                      ,Tdh_StartDate
                                      ,Tdh_EndDate
                                      ,{0} AS Tdh_DeductionAmount
                                      ,{1} AS Tdh_PaidAmount
                                      ,{2} AS Tdh_CycleAmount
                                      ,{3} AS Tdh_DeferredAmount
                                      ,Tdh_CheckDate
                                      ,Tdh_VoucherNumber
                                      ,Tdh_PaymentDate
                                      ,Mdn_DeductionName
                                      ,{4} AS Balance
                                      ,ISNULL([Tdh_PrincipalAmount], 0) AS Tdh_PrincipalAmount
                                      ,ISNULL([Tdh_ApplicablePayCycle],'') AS Tdh_ApplicablePayCycle
                                      ,PAYFREQ.Mcd_Name [ApplicablePayCycleName]
                                      ,Tdh_ExemptInPayroll
                                      ,Tdh_AgreementNo
                                      ,Tdh_DocumentNo
                                      ,Tdh_EntryDate
                                      ,Tdh_PaymentTerms
                                      ,1 AS IsHist
                                      ,{5} AS RemainingCycles
                                      ,Mdn_IsSystemReserved
                                      ,Mdn_DeductionGroup
                                      ,ISNULL(Mdn_IsRecurring,0) AS Mdn_IsRecurring
                                      ,ISNULL(Mdn_WithCheckDate,0) AS Mdn_WithCheckDate
                                      ,ISNULL(Mdn_WithAccountingVoucher,0) AS Mdn_WithAccountingVoucher
                                      ,Mdn_PaymentTerms
                                      ,ISNULL(Mdn_WithPrincipalAmount,0) AS Mdn_WithPrincipalAmount
                                      ,Mdn_PaidUpAmount
                                      ,ISNULL({6}.dbo.Udf_DisplayName(T_EmpDeductionHdrHst.Usr_Login,'{7}'),T_EmpDeductionHdrHst.Usr_Login) AS Usr_Login
                                      ,T_EmpDeductionHdrHst.Ludatetime
                                FROM T_EmpDeductionHdrHst
                                INNER JOIN M_Deduction ON Tdh_DeductionCode = Mdn_DeductionCode
                                    AND Mdn_CompanyCode = @CompanyCode
                                LEFT JOIN M_CodeDtl PAYFREQ 
								    ON PAYFREQ.Mcd_Code = ISNULL([Tdh_ApplicablePayCycle],'')
								    AND PAYFREQ.Mcd_CodeType = 'PAYFREQ'
                                    AND PAYFREQ.Mcd_CompanyCode = @CompanyCode
                                WHERE Tdh_IDNo = @Tdh_IDNo 
                                ORDER BY Tdh_DeductionCode";

            sqlQuery = string.Format(sqlQuery, "Tdh_DeductionAmount",
                                                "Tdh_PaidAmount",
                                                "Tdh_CycleAmount",
                                                "Tdh_DeferredAmount ",
                                                "(Tdh_DeductionAmount-Tdh_PaidAmount)",
                                                "CASE WHEN Tdh_CycleAmount = 0 THEN 0 ELSE CEILING((Tdh_DeductionAmount-Tdh_PaidAmount)/Tdh_CycleAmount) END",
                                                CentralProfile,
                                                NAMEDSPLY);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet FetchRecord2(string Tdh_IDNo, string UserLogin, string Tdh_DeductionCode, object Tdh_StartDate, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdh_DeductionCode", Tdh_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdh_StartDate", Tdh_StartDate, SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@CompanyCode", CompanyCode);

            string sqlQuery = @"SELECT Tdh_IDNo
                                      ,Tdh_DeductionCode
                                      ,Tdh_StartDate
                                      ,Tdh_EndDate
                                      ,Tdh_DeductionAmount
                                      ,Tdh_PaidAmount
                                      ,Tdh_CycleAmount
                                      ,Tdh_DeferredAmount
                                      ,Tdh_CheckDate
                                      ,Tdh_VoucherNumber
                                      ,Tdh_PaymentDate
                                      ,Mdn_DeductionName
                                      ,(Tdh_DeductionAmount-Tdh_PaidAmount) as Balance
                                      ,ISNULL([Tdh_PrincipalAmount], 0) AS Tdh_PrincipalAmount
                                      ,ISNULL([Tdh_ApplicablePayCycle],'') AS Tdh_ApplicablePayCycle
                                      ,PAYFREQ.Mcd_Name [ApplicablePayCycleName]
                                      ,Tdh_ExemptInPayroll
                                      ,Tdh_AgreementNo
                                      ,Tdh_DocumentNo
                                      ,Tdh_EntryDate
                                      ,Tdh_PaymentTerms
                                      ,CASE WHEN Tdh_CycleAmount = 0 THEN 0 ELSE CEILING((Tdh_DeductionAmount-Tdh_PaidAmount)/Tdh_CycleAmount) END as RemainingCycles
                                      ,Mdn_IsSystemReserved
                                      ,Mdn_DeductionGroup
                                      ,Mdn_IsRecurring
                                      ,Mdn_WithCheckDate
                                      ,Mdn_WithAccountingVoucher
                                      ,Mdn_PaymentTerms
                                      ,Mdn_WithPrincipalAmount
                                      ,Mdn_PaidUpAmount
                                      ,T_EmpDeductionHdr.Usr_Login
                                      ,T_EmpDeductionHdr.Ludatetime
                                FROM T_EmpDeductionHdr
                                INNER JOIN M_Deduction
                                ON Tdh_DeductionCode = Mdn_DeductionCode
                                    AND Mdn_CompanyCode = @CompanyCode
                                LEFT JOIN M_CodeDtl PAYFREQ 
								ON PAYFREQ.Mcd_Code = ISNULL(Tdh_ApplicablePayCycle,'')
								AND PAYFREQ.Mcd_CodeType = 'PAYFREQ'
                                AND PAYFREQ.Mcd_CompanyCode = @CompanyCode
                                WHERE Tdh_IDNo = @Tdh_IDNo
                                    AND Tdh_DeductionCode= @Tdh_DeductionCode
                                    AND Tdh_StartDate = @Tdh_StartDate";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetLastSeqNoInTrail(string Tdh_IDNo, string Tdh_DeductionCode, string Tdh_StartDate, string CentralProfile)
        {
            int x = 0;
            string LastSeqNo = string.Empty;

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdh_DeductionCode", Tdh_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdh_StartDate", Tdh_StartDate);

            string sqlQuery = @"SELECT COUNT(Tdh_LineNo)
                                FROM T_EmpDeductionHdrTrl
                                WHERE Tdh_IDNo = @Tdh_IDNo
                                AND Tdh_DeductionCode = @Tdh_DeductionCode
                                AND Tdh_StartDate = @Tdh_StartDate";
            
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                x = Convert.ToInt32(dal.ExecuteScalar(sqlQuery, CommandType.Text, paramInfo));

                dal.CloseDB();
            }
            x++;
            if (x < 10)
                LastSeqNo = "0" + x.ToString();
            else
                LastSeqNo = x.ToString();

            return LastSeqNo;
        }

        public int DeleteRecord(DataRow row)
        {
            int retVal = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"], SqlDbType.Date);
            paramInfo[1] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[2] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            //paramInfo[3] = new ParameterInfo("@Tdh_EndDate", row["Tdh_EndDate"]);

            string LastSeqNo = GetLastSeqNoInTrail(row["Tdh_IDNo"].ToString(),row["Tdh_DeductionCode"].ToString(),row["Tdh_StartDate"].ToString(), CentralProfile);

            ParameterInfo[] InsparamInfo = new ParameterInfo[22];
            InsparamInfo[0] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            InsparamInfo[1] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            InsparamInfo[2] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"], SqlDbType.Date);
            InsparamInfo[3] = new ParameterInfo("@Tdh_EndDate", (GetValue(row["Tdh_EndDate"]) != string.Empty ? row["Tdh_EndDate"] : DBNull.Value), SqlDbType.Date);
            InsparamInfo[4] = new ParameterInfo("@Tdh_LineNo", LastSeqNo);
            InsparamInfo[5] = new ParameterInfo("@Tdh_DeductionAmount", row["Tdh_DeductionAmount"]);
            InsparamInfo[6] = new ParameterInfo("@Tdh_PaidAmount", row["Tdh_PaidAmount"]);
            InsparamInfo[7] = new ParameterInfo("@Tdh_CycleAmount", row["Tdh_CycleAmount"]);
            InsparamInfo[8] = new ParameterInfo("@Tdh_DeferredAmount", row["Tdh_DeferredAmount"]);
            InsparamInfo[9] = new ParameterInfo("@Tdh_CheckDate", row["Tdh_CheckDate"]);
            InsparamInfo[10] = new ParameterInfo("@Tdh_VoucherNumber", row["Tdh_VoucherNumber"]);
            InsparamInfo[11] = new ParameterInfo("@Tdh_PaymentDate", row["Tdh_PaymentDate"]);
            InsparamInfo[12] = new ParameterInfo("@Tdh_RecordStatus", "D");
            InsparamInfo[13] = new ParameterInfo("@Tdh_PrincipalAmount", row["Tdh_PrincipalAmount"]);
            InsparamInfo[14] = new ParameterInfo("@Tdh_ApplicablePayCycle", row["Tdh_ApplicablePayCycle"]);
            InsparamInfo[15] = new ParameterInfo("@Tdh_ExemptInPayroll", row["Tdh_ExemptInPayroll"]);

            DataSet tempds;
            tempds = this.GetUserandLudatetime(row["Tdh_IDNo"].ToString(), row["Tdh_DeductionCode"].ToString(), row["Tdh_StartDate"].ToString(), CentralProfile);
            InsparamInfo[16] = new ParameterInfo("@Usr_Login", tempds.Tables[0].Rows[0][0].ToString());
            InsparamInfo[17] = new ParameterInfo("@Ludatetime", tempds.Tables[0].Rows[0][1].ToString());
            InsparamInfo[18] = new ParameterInfo("@Tdh_AgreementNo", row["Tdh_AgreementNo"]);
            InsparamInfo[19] = new ParameterInfo("@Tdh_DocumentNo", row["Tdh_DocumentNo"]);
            InsparamInfo[20] = new ParameterInfo("@Tdh_EntryDate", row["Tdh_EntryDate"]);
            InsparamInfo[21] = new ParameterInfo("@Tdh_PaymentTerms", row["Tdh_PaymentTerms"]);


            string sqlInsert = @"INSERT INTO T_EmpDeductionHdrTrl
                                               (Tdh_IDNo
                                               ,Tdh_DeductionCode
                                               ,Tdh_StartDate
                                               ,Tdh_EndDate
                                               ,Tdh_LineNo
                                               ,Tdh_DeductionAmount
                                               ,Tdh_PaidAmount
                                               ,Tdh_CycleAmount
                                               ,Tdh_DeferredAmount
                                               ,Tdh_CheckDate
                                               ,Tdh_VoucherNumber
                                               ,Tdh_PaymentDate
                                               ,Tdh_RecordStatus
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdh_PrincipalAmount
                                               ,Tdh_ApplicablePayCycle
                                               ,Tdh_ExemptInPayroll
                                               ,Tdh_AgreementNo
                                               ,Tdh_DocumentNo
                                               ,Tdh_EntryDate
                                               ,Tdh_PaymentTerms)
                                         VALUES
                                               (@Tdh_IDNo
		                                        ,@Tdh_DeductionCode
		                                        ,@Tdh_StartDate
		                                        ,@Tdh_EndDate
		                                        ,@Tdh_LineNo
		                                        ,@Tdh_DeductionAmount
		                                        ,@Tdh_PaidAmount
		                                        ,@Tdh_CycleAmount
		                                        ,@Tdh_DeferredAmount
		                                        ,@Tdh_CheckDate
		                                        ,@Tdh_VoucherNumber
		                                        ,@Tdh_PaymentDate
		                                        ,@Tdh_RecordStatus
		                                        ,@Usr_Login
		                                        ,@Ludatetime
                                                ,@Tdh_PrincipalAmount
                                                ,@Tdh_ApplicablePayCycle
                                                ,@Tdh_ExemptInPayroll
                                                ,@Tdh_AgreementNo
                                                ,@Tdh_DocumentNo
                                                ,@Tdh_EntryDate
                                                ,@Tdh_PaymentTerms)";

            string sqlDelete = @"DELETE FROM T_EmpDeductionHdr
                                                WHERE Tdh_StartDate = @Tdh_StartDate
                                                --AND Tdh_EndDate = @Tdh_EndDate
                                                AND Tdh_DeductionCode = @Tdh_DeductionCode
                                                AND Tdh_IDNo = @Tdh_IDNo";
   
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    //insert record in trail
                    retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, InsparamInfo);
                    //delete records
                    retVal = dal.ExecuteNonQuery(sqlDelete, CommandType.Text, paramInfo);
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

        public bool CheckifFullyPaid(string DeductionAmount, string PaidAmount)
        {
            if ((Convert.ToDecimal(DeductionAmount) - Convert.ToDecimal(PaidAmount)) == 0)
                return true;
            else
                return false;
        }

        public bool CheckifCutOff()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Tsc_SetFlag
                                    From T_SettingControl
                                    Where Tsc_SystemCode = 'PAYROLL'
                                    And Tsc_SettingCode = 'CUT-OFF'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows[0][0].ToString() == "True")
                return true;
            else
                return false;
        }

        public string GetNewServerDate()
        {
            DataSet ds = new DataSet();

            string qString = @"Select Getdate()";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }

            return ds.Tables[0].Rows[0][0].ToString();
        }

        public DataSet GetDeductionCodeMasterValues(string Mdn_DeductionCode, string Mdn_CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mdn_DeductionCode", Mdn_DeductionCode);
            paramInfo[1] = new ParameterInfo("@Mdn_CompanyCode", Mdn_CompanyCode);

            string sqlQuery = @"Select Mdn_DeductionGroup
                                        ,Mdn_WithCheckDate
                                        ,Mdn_WithAccountingVoucher
                                        ,Mdn_PaymentTerms
                                        ,Mdn_WithPrincipalAmount
                                FROM M_Deduction
                                WHERE Mdn_DeductionCode = @Mdn_DeductionCode
                                    AND Mdn_CompanyCode = @Mdn_CompanyCode";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public bool IsGovernmentLoan(string DeductionCode, string EmployeeID, string StartDeductionDate, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@DeductionCode", DeductionCode);
            paramInfo[1] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[2] = new ParameterInfo("@StartDeductionDate", StartDeductionDate);
            paramInfo[3] = new ParameterInfo("@CompanyCode", CompanyCode);

            string sqlQuery = @"SELECT Mgr_RemittanceCode
                                , Mgr_RemittanceName
                                FROM T_EmpDeductionHdr
                                INNER JOIN M_GovRemittance
                                    ON Mgr_RemittanceCode = Tdh_DeductionCode
                                    AND Mgr_CompanyCode = @CompanyCode
                                WHERE Tdh_IDNo = @IDNo
                                    AND Tdh_DeductionCode = @DeductionCode
                                    AND Tdh_StartDate != @StartDeductionDate
                                    AND Tdh_PaidAmount + Tdh_DeferredAmount != Tdh_DeductionAmount
                                    AND Mgr_RemittanceType IN ('L', 'P')";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetAmortizationAmount(string Mla_DeductionCode, string Mla_DeductionAmount)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Mla_Amortization
                             From M_LoanAmortization
                             Where Mla_DeductionCode = @Mla_DeductionCode
                                And Mla_DeductionAmount = @Mla_DeductionAmount
                                And Mla_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mla_DeductionCode", Mla_DeductionCode);
            paramInfo[1] = new ParameterInfo("@Mla_DeductionAmount", double.Parse(Mla_DeductionAmount));

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public bool CheckIfRecordExists(string Tdh_IDNo, string Tdh_DeductionCode, string Tdh_StartDate, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdh_DeductionCode", Tdh_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdh_StartDate", Tdh_StartDate);

            string sqlQuery = @"SELECT * FROM T_EmpDeductionHdr
                                        WHERE Tdh_IDNo = @Tdh_IDNo
                                        AND Tdh_DeductionCode = @Tdh_DeductionCode
                                        AND Tdh_StartDate = @Tdh_StartDate";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataSet GetUserandLudatetime(string Tdh_IDNo, string Tdh_DeductionCode, string Tdh_StartDate, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdh_DeductionCode", Tdh_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdh_StartDate", Tdh_StartDate);

            string sqlQuery = @"Select Usr_Login
		                                            ,Ludatetime
                                            From T_EmpDeductionHdr
                                            Where Tdh_IDNo = @Tdh_IDNo
                                            And Tdh_DeductionCode = @Tdh_DeductionCode
                                            And Tdh_StartDate = @Tdh_StartDate";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        #endregion

        #region <For Deduction Entry Trail>

        public DataSet FetchAllDeductionTrailValues(string Tdh_IDNo, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);

            string sqlQuery = @"SELECT Tdh_DeductionCode  AS [Deduction Code]
                                                    ,Tdh_StartDate AS [Start Date]
                                                    ,Tdh_EndDate AS [End Date] 
                                                    ,Tdh_LineNo AS [Seq No]
                                                    ,CASE WHEN ISNULL([Tdh_ApplicablePayCycle],'') = '0'
                                                        THEN 'ALL QUINCENAS'
                                                        WHEN ISNULL([Tdh_ApplicablePayCycle],'') = '1'
                                                        THEN '1ST QUINCENA'
                                                        WHEN ISNULL([Tdh_ApplicablePayCycle],'') = '2'
                                                        THEN '2ND QUINCENA'
                                                        ELSE ''
                                                        END AS [Timing of Deduction]
                                                    ,Tdh_DeductionAmount AS [Total Payable Amount] 
                                                    ,Tdh_PaymentTerms AS [Payment Term(s)]      
                                                    ,Tdh_CycleAmount AS [Deduction Amount]                        
                                                    ,Tdh_PaidAmount  AS [Payment to Date]                                   
                                                    ,Tdh_DeferredAmount  AS [Deferred Amount]   
                                                    ,Tdh_PaymentDate AS [Payment Date]    
                                                    ,Tdh_ExemptInPayroll AS [Hold]
                                                    ,Tdh_CheckDate AS [Check Date]    
                                                    ,Tdh_VoucherNumber AS [Voucher Number]
                                                    ,Tdh_AgreementNo AS [Agreement Number]
                                                    ,ISNULL([Tdh_PrincipalAmount], 0) AS [Principal Amount]
                                                    ,CONVERT(CHAR(10), Tdh_EntryDate, 101) AS [Entry Date]
                                                    ,Tdh_DocumentNo AS [Document Number]
                                                    ,CASE Tdh_RecordStatus 
                                                                        WHEN 'D' then 'DELETED'

                                                                        WHEN 'A' then 'ADDED'
                                                                        WHEN 'E' then 'UPDATED'
                                                                        WHEN 'U' then 'BATCH UPLOADED'
                                                                        WHEN 'P' then 'BATCH UPDATED'
                                                                        END AS [Type] 
                                                    ,LEFT(Mur_UserFirstName,1) + LEFT(Mur_UserMiddleName,1) + RTRIM(Mur_UserLastName) AS [User]   
                                                    ,T_EmpDeductionHdrTrl.Ludatetime AS [Last datetime]                                                           
                                                FROM T_EmpDeductionHdrTrl LEFT JOIN M_User
		                                            ON T_EmpDeductionHdrTrl.Usr_Login = Mur_UserCode
                                                WHERE Tdh_IDNo = @Tdh_IDNo
                                                ORDER BY Ludatetime DESC";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        #endregion

        #region <For Make Payments>

        public int UpdatePaidAmount(DataRow row, DALHelper dalCentral)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", row["Tdh_PaidAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", row["Tdh_PaymentDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);

            string sqlQuery = @"UPDATE T_EmpDeductionHdr
                                   SET Tdh_PaidAmount = Tdh_PaidAmount + @Tdh_PaidAmount
                                      ,Tdh_PaymentDate = @Tdh_PaymentDate
                                      ,Usr_Login = @Usr_Login
                                      ,Ludatetime = GetDate()
                                 WHERE Tdh_IDNo = @Tdh_IDNo
                                     And Tdh_DeductionCode = @Tdh_DeductionCode
                                     And Tdh_StartDate = @Tdh_StartDate";

            try
            {
                retVal = dalCentral.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);                        
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }                            
                       
            return retVal;
        }

        public int StopResumeAmount(DataRow row, DALHelper dalCentral)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionAmount", row["Tdh_DeductionAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);

            string sqlQuery = @"UPDATE T_EmpDeductionHdr
                                   SET Tdh_DeductionAmount = @Tdh_DeductionAmount
                                      ,Usr_Login = @Usr_Login
                                      ,Ludatetime = GETDATE()
                                 WHERE Tdh_IDNo = @Tdh_IDNo
                                     And Tdh_DeductionCode = @Tdh_DeductionCode
                                     And Tdh_StartDate = @Tdh_StartDate";

            try
            {
                retVal = dalCentral.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retVal;
        }

        public int InsertDetailHist(DataRow row,DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            AdjustmentPaymentsBL AdjPay = new AdjustmentPaymentsBL();

            ParameterInfo[] paramInfo = new ParameterInfo[11];
            
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_ThisPayCycle", row["Tdd_ThisPayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PaymentType", row["Tdd_PaymentType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", AdjPay.GetSeqNo(row["Tdd_IDNo"].ToString(),row["Tdd_DeductionCode"].ToString(),row["Tdd_StartDate"],row["Tdd_ThisPayCycle"].ToString(),row["Tdd_PayCycle"].ToString()));
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Amount", row["Tdd_Amount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredFlag", row["Tdd_DeferredFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Remarks", row["Tdd_Remarks"]);

            string sqlQuery = @"INSERT INTO T_EmpDeductionDtlHst
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_ThisPayCycle
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_PaymentType
                                               ,Tdd_Amount
                                               ,Tdd_DeferredFlag
                                               ,Tdd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdd_Remarks)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_ThisPayCycle
                                               ,@Tdd_PayCycle
                                               ,'01' -- jan modified 20081013
                                               ,@Tdd_PaymentType
                                               ,@Tdd_Amount
                                               ,@Tdd_DeferredFlag
                                               ,'1'
                                               ,@Usr_Login
                                               ,GetDate()
                                               ,@Tdd_Remarks)";

            try
            {
                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);                
            }
            catch (Exception e)
            {                   
                throw new PayrollException(e);
            }                            

            return retVal;
        }

        public DataSet GetDedDeferredData(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate, DALHelper dal)
        {
            DataSet ds = new DataSet();

            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date);

            string sqlQuery = @"SELECT  Tdd_IDNo
		                                            ,Tdd_DeductionCode
		                                            ,Tdd_StartDate
		                                            ,Tdd_PayCycle
		                                            ,Tdd_LineNo
		                                            ,Tdd_DeferredAmt
                                 FROM T_EmpDeductionDefer
                                 WHERE Tdd_IDNo = @Tdd_IDNo
                                      AND Tdd_DeductionCode = @Tdd_DeductionCode
                                      AND Tdd_StartDate = @Tdd_StartDate
                                 ORDER BY Tdd_PayCycle,Tdd_LineNo";

            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetDedDeferredData(string Tdd_IDNo, string Tdd_DeductionCode, string Tdd_StartDate, string LoginDBName, DALHelper dal)
        {
            DataSet ds = new DataSet();

            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate);

            string sqlQuery = string.Format(@"SELECT Tdd_IDNo
		                                       ,Tdd_DeductionCode
		                                       ,Tdd_StartDate
		                                       ,Tdd_PayCycle
		                                       ,Tdd_LineNo
		                                       ,Tdd_DeferredAmt
                                FROM {0}..T_EmpDeductionDefer
                                WHERE Tdd_IDNo = @Tdd_IDNo
                                    AND Tdd_DeductionCode = @Tdd_DeductionCode
                                    AND Tdd_StartDate = @Tdd_StartDate
                                ORDER BY Tdd_PayCycle, Tdd_LineNo", LoginDBName);

            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public int InsDelRecords(DataRow row, DataRow origdr, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] delparamInfo = new ParameterInfo[5];

            delparamInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            delparamInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            delparamInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            delparamInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            delparamInfo[4] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"]);

            ParameterInfo[] paramInfo = new ParameterInfo[10];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
      
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_ThisPayCycle", origdr["Tdd_ThisPayCycle"].ToString().Trim());

            DataSet dsSeq = this.GetSeqNo(row["Tdd_IDNo"].ToString(), row["Tdd_DeductionCode"].ToString(), row["Tdd_StartDate"].ToString(), origdr["Tdd_ThisPayCycle"].ToString(), row["Tdd_PayCycle"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", this.getNextSeqNo(dsSeq.Tables[0].Rows[0][0].ToString()));

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PaymentType", origdr["Tdd_PaymentType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Amount", row["Tdd_DeferredAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredFlag", origdr["Tdd_DeferredFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", origdr["Usr_Login"]);

            string sqlDelete = @"DELETE FROM T_EmpDeductionDefer
                                               WHERE Tdd_IDNo = @Tdd_IDNo
                                                AND Tdd_DeductionCode = @Tdd_DeductionCode
                                                AND Tdd_StartDate = @Tdd_StartDate
                                                AND Tdd_PayCycle = @Tdd_PayCycle
                                                AND Tdd_LineNo = @Tdd_LineNo";

            string sqlInsert = @"INSERT INTO T_EmpDeductionDtlHst
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_ThisPayCycle 
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_PaymentType
                                               ,Tdd_Amount
                                               ,Tdd_DeferredFlag
                                               ,Tdd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_ThisPayCycle
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_PaymentType
                                               ,@Tdd_Amount
                                               ,@Tdd_DeferredFlag
                                               ,'1'
                                               ,@Usr_Login
                                               ,GetDate())";

                try
                {
                    //delete
                    retVal = dal.ExecuteNonQuery(sqlDelete, CommandType.Text, delparamInfo);
                    //insert
                    retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);           
                }
                catch (Exception e)
                {
                   
                    throw new PayrollException(e);
                }            
            return retVal;
        }

        private string getNextSeqNo(string seqno)
        {
            string y = string.Empty;
            int x = Convert.ToInt32(seqno);
            x++;
            if (x < 10)
            {
                y = "0" + x.ToString();
            }
            else
            {
                y = x.ToString();
            }
            return y;
        }

        public int UpdateSplitedRecord(DataRow row, string UserLogin, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[7];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredAmt", row["Tdd_DeferredAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", UserLogin);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);

            string sqlQuery = @"UPDATE T_EmpDeductionDefer
                                               SET Tdd_LineNo = @Tdd_LineNo
                                                  ,Tdd_DeferredAmt = @Tdd_DeferredAmt
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GetDate()
                                             WHERE Tdd_IDNo = @Tdd_IDNo
                                                  And Tdd_DeductionCode = @Tdd_DeductionCode
                                                  And Tdd_StartDate = @Tdd_StartDate
                                                  And Tdd_PayCycle = @Tdd_PayCycle";
            
                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);                  
                }
                catch (Exception e)
                {
                    
                    throw new PayrollException(e);
                }
                

            return retVal;
        }

        public int UpdatePaidandDeferred(DataRow row, string CentralProfile)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaidAmount", row["Tdh_PaidAmount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_PaymentDate", row["Tdh_PaymentDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdh_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdh_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdh_StartDate"]);

            string sqlQuery = @"UPDATE T_EmpDeductionHdr
                                               SET Tdh_PaidAmount = Tdh_PaidAmount + @Tdh_PaidAmount
                                                  ,Tdh_DeferredAmount = Tdh_DeferredAmount - @Tdh_PaidAmount
                                                  ,Tdh_PaymentDate = @Tdh_PaymentDate
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GetDate()
                                             WHERE Tdh_IDNo = @Tdh_IDNo
                                                  And Tdh_DeductionCode = @Tdh_DeductionCode
                                                  And Tdh_StartDate = @Tdh_StartDate";

            try
            {
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);

                    dal.CloseDB();
                }

            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            finally
            {
            }
          
            return retVal;
        }

        public string GetAmortizationAmnt(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate);

            string sqlQuery = @"SELECT TOP(1) Tdd_DeferredAmt
                                FROM T_EmpDeductionDefer
                                WHERE Tdd_IDNo = @Tdd_IDNo
                                     AND Tdd_DeductionCode = @Tdd_DeductionCode
                                     AND Tdd_StartDate = @Tdd_StartDate";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds.Tables[0].Rows[0][0].ToString();
        }

        public bool CheckIfDeferredAmntSynchs(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate);

            string sqlQuery = string.Format(@"SELECT ISNULL(TotalDeferred, 0) as TotalDeferred
                                                    ,Tdh_DeferredAmount
                                                FROM {0}..T_EmpDeductionHdr  
                                                    LEFT JOIN ( SELECT   Tdd_IDNo 
						                                        , Tdd_DeductionCode 
						                                        , Tdd_StartDate
					                                            , Sum(Tdd_DeferredAmt) as TotalDeferred
					                                        FROM T_EmpDeductionDefer
					                                        GROUP BY Tdd_IDNo 
						                                        , Tdd_DeductionCode 
						                                        , Tdd_StartDate ) Deff on Tdd_IDNo = Tdh_IDNo 
													                                         And Tdd_DeductionCode = Tdh_DeductionCode
													                                        And Tdd_StartDate = Tdh_StartDate
                                         WHERE Tdh_IDNo =@Tdd_IDNo
                                            AND Tdd_DeductionCode = @Tdd_DeductionCode
                                            AND Tdh_StartDate = @Tdd_StartDate"
                                        , LoginInfo.getUser().CentralProfileName);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["TotalDeferred"].ToString().Trim() == ds.Tables[0].Rows[0]["Tdh_DeferredAmount"].ToString().Trim())
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public DataSet GetSeqNo(DataRow row)
        {
            DataSet ds = new DataSet();
                             
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[3] = new ParameterInfo("@Tdd_ThisPayCycle", row["Tdd_ThisPayCycle"]);
            paramInfo[4] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);

            string sqlQuery = @"SELECT COUNT(Tdd_IDNo) FROM T_EmpDeductionDtlHst
                                WHERE Tdd_IDNo = @Tdd_IDNo
                                      AND Tdd_DeductionCode = @Tdd_DeductionCode
                                      AND Tdd_StartDate = @Tdd_StartDate
                                      AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                      AND Tdd_PayCycle = @Tdd_PayCycle
                                GROUP BY Tdd_IDNo";
           
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetSeqNo(string Tdd_IDNo, string Tdd_DeductionCode, string Tdd_StartDate, string Tdd_ThisPayCycle, string Tdd_PayCycle)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate);
            paramInfo[3] = new ParameterInfo("@Tdd_ThisPayCycle", Tdd_ThisPayCycle);
            paramInfo[4] = new ParameterInfo("@Tdd_PayCycle", Tdd_PayCycle);

            string sqlQuery = @"Select Count(Tdd_IDNo) From T_EmpDeductionDtlHst
                                    WHERE Tdd_IDNo = @Tdd_IDNo
                                          And Tdd_DeductionCode = @Tdd_DeductionCode
                                          And Tdd_StartDate = @Tdd_StartDate
                                          And Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                          And Tdd_PayCycle = @Tdd_PayCycle
                                          Group by Tdd_IDNo";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public int UpdateInsRecords(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            AdjustmentPaymentsBL AdjPay = new AdjustmentPaymentsBL();
            ParameterInfo[] paramInfo = new ParameterInfo[11];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_ThisPayCycle", row["Tdd_ThisPayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", AdjPay.GetSeqNo(row["Tdd_IDNo"].ToString(), row["Tdd_DeductionCode"].ToString(), row["Tdd_StartDate"].ToString(), row["Tdd_ThisPayCycle"].ToString(), row["Tdd_PayCycle"].ToString()));
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PaymentType", row["Tdd_PaymentType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Amount", row["Tdd_Amount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredFlag", row["Tdd_DeferredFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Remarks", row["Tdd_Remarks"]);

            string sqlQuery = @"INSERT INTO T_EmpDeductionDtlHst
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_ThisPayCycle 
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_PaymentType
                                               ,Tdd_Amount
                                               ,Tdd_DeferredFlag
                                               ,Tdd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdd_Remarks)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_ThisPayCycle
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_PaymentType
                                               ,@Tdd_Amount
                                               ,@Tdd_DeferredFlag
                                               ,'1'
                                               ,@Usr_Login
                                               ,GETDATE()
                                               ,@Tdd_Remarks)";

            try
            {
                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retVal;
        }

        public int UpdateInsRecords(DataRow row, string LoginDBName, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            AdjustmentPaymentsBL AdjPay = new AdjustmentPaymentsBL();
            ParameterInfo[] paramInfo = new ParameterInfo[11];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_ThisPayCycle", row["Tdd_ThisPayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", AdjPay.GetSeqNo(row["Tdd_IDNo"].ToString(), row["Tdd_DeductionCode"].ToString(), row["Tdd_StartDate"].ToString(), row["Tdd_ThisPayCycle"].ToString(), row["Tdd_PayCycle"].ToString()));
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PaymentType", row["Tdd_PaymentType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Amount", row["Tdd_Amount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredFlag", row["Tdd_DeferredFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Remarks", row["Tdd_Remarks"]);

            string sqlQuery = string.Format(@"INSERT INTO {0}..T_EmpDeductionDtlHst
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_ThisPayCycle 
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_PaymentType
                                               ,Tdd_Amount
                                               ,Tdd_DeferredFlag
                                               ,Tdd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tdd_Remarks)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_ThisPayCycle
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_PaymentType
                                               ,@Tdd_Amount
                                               ,@Tdd_DeferredFlag
                                               ,'1'
                                               ,@Usr_Login
                                               ,GETDATE()
                                               ,@Tdd_Remarks)", LoginDBName);

            try
            {
                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);                    
            }
            catch (Exception e)
            {                    
                throw new PayrollException(e);
            }               
            
            return retVal;
        }

        public int UpdateDeductionLedgerFullyPaidAmount(DataRow row, string CentralProfile)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdd_StartDate"]);

            string sqlQuery = @"UPDATE T_EmpDeductionHdr
                                   SET Tdh_PaidAmount = Tdh_DeductionAmount
                                      ,Tdh_DeferredAmount = 0
                                      ,Tdh_PaymentDate = GetDate()
                                      ,Usr_Login = @Usr_Login
                                      ,Ludatetime = GetDate()
                                 WHERE Tdh_IDNo = @Tdh_IDNo
                                     And Tdh_DeductionCode = @Tdh_DeductionCode
                                     And Tdh_StartDate = @Tdh_StartDate";

            try
            {
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();

                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);

                    dal.CloseDB();
                }
                
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retVal;
        }

        public int UpdateDeductionLedgerFullyPaidAmount(DataRow row, DALHelper dalCentral)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdh_StartDate", row["Tdd_StartDate"]);

            string sqlQuery = @"UPDATE T_EmpDeductionHdr
                                   SET Tdh_PaidAmount = Tdh_DeductionAmount
                                      ,Tdh_DeferredAmount = 0
                                      ,Tdh_PaymentDate = GETDATE()
                                      ,Usr_Login = @Usr_Login
                                      ,Ludatetime = GetDate()
                                 WHERE Tdh_IDNo = @Tdh_IDNo
                                     AND Tdh_DeductionCode = @Tdh_DeductionCode
                                     AND Tdh_StartDate = @Tdh_StartDate";

            try
            {
                retVal = dalCentral.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retVal;
        }

        public int TransferDeferredToHist(DataRow row, DataRow origdr, DALHelper dal)
        {
            AdjustmentPaymentsBL AdjPay = new AdjustmentPaymentsBL();
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] delparamInfo = new ParameterInfo[5];

            delparamInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            delparamInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            delparamInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            delparamInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            delparamInfo[4] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"]);

            ParameterInfo[] paramInfo = new ParameterInfo[10];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_ThisPayCycle", origdr["Tdd_ThisPayCycle"].ToString().Trim());
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", AdjPay.GetSeqNo(origdr["Tdd_IDNo"].ToString(), origdr["Tdd_DeductionCode"].ToString(), origdr["Tdd_StartDate"].ToString(), origdr["Tdd_ThisPayCycle"].ToString(), origdr["Tdd_PayCycle"].ToString()));
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PaymentType", "Z");
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Amount", row["Tdd_DeferredAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredFlag", "TRUE");
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", origdr["Usr_Login"]);

            string sqlDelete = @"DELETE FROM T_EmpDeductionDefer
                                 WHERE Tdd_IDNo = @Tdd_IDNo
                                      AND Tdd_DeductionCode = @Tdd_DeductionCode
                                      AND Tdd_StartDate = @Tdd_StartDate
                                      AND Tdd_PayCycle = @Tdd_PayCycle
                                      AND Tdd_LineNo = @Tdd_LineNo";

            string sqlInsert = @"INSERT INTO T_EmpDeductionDtlHst
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_ThisPayCycle 
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_PaymentType
                                               ,Tdd_Amount
                                               ,Tdd_DeferredFlag
                                               ,Tdd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_ThisPayCycle
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_PaymentType
                                               ,@Tdd_Amount
                                               ,@Tdd_DeferredFlag
                                               ,'1'
                                               ,@Usr_Login
                                               ,GetDate())";

            try
            {
                //delete
                retVal = dal.ExecuteNonQuery(sqlDelete, CommandType.Text, delparamInfo);
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int TransferDeferredToHist(DataRow row, DataRow origdr, string LoginDBName, DALHelper dal)
        {
            AdjustmentPaymentsBL AdjPay = new AdjustmentPaymentsBL();
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] delparamInfo = new ParameterInfo[5];

            delparamInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            delparamInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            delparamInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            delparamInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            delparamInfo[4] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"]);

            ParameterInfo[] paramInfo = new ParameterInfo[10];

            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_ThisPayCycle", origdr["Tdd_ThisPayCycle"].ToString().Trim());
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_LineNo", AdjPay.GetSeqNo(origdr["Tdd_IDNo"].ToString(), origdr["Tdd_DeductionCode"].ToString(), origdr["Tdd_StartDate"].ToString(), origdr["Tdd_ThisPayCycle"].ToString(), origdr["Tdd_PayCycle"].ToString()));
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PaymentType", "Z");
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_Amount", row["Tdd_DeferredAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_DeferredFlag", "TRUE");
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", origdr["Usr_Login"]);

            string sqlDelete = string.Format(@"DELETE FROM {0}..T_EmpDeductionDefer
                                             WHERE Tdd_IDNo = @Tdd_IDNo
                                                 AND Tdd_DeductionCode = @Tdd_DeductionCode
                                                 AND Tdd_StartDate = @Tdd_StartDate
                                                 AND Tdd_PayCycle = @Tdd_PayCycle
                                                 AND Tdd_LineNo = @Tdd_LineNo", LoginDBName);

            string sqlInsert = string.Format(@"INSERT INTO {0}..T_EmpDeductionDtlHst
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_ThisPayCycle 
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_PaymentType
                                               ,Tdd_Amount
                                               ,Tdd_DeferredFlag
                                               ,Tdd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_ThisPayCycle
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_PaymentType
                                               ,@Tdd_Amount
                                               ,@Tdd_DeferredFlag
                                               ,'1'
                                               ,@Usr_Login
                                               ,GETDATE())", LoginDBName);

            try
            {
                //delete
                retVal = dal.ExecuteNonQuery(sqlDelete, CommandType.Text, delparamInfo);
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public DataSet FetchforViewPayments(string Tdd_IDNo, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[paramIndex++] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@CompanyCode", CompanyCode);

            string sqlQuery = string.Format(@"SELECT Tdd_DeductionCode as 'Deduction Code'
		                                            , Tdd_StartDate as 'Start Date'
                                                    , Tdd_ThisPayCycle as 'Pay Cycle'
                                                    , Tdd_LineNo as 'Seq'
                                                    , ISNULL(PAYTYPE.Mcd_Name, ' ') as 'Payment Type'
                                                    , Tdd_Amount as Amount
                                                    , CASE WHEN Tdd_DeferredFlag = 1 THEN Tdd_PayCycle ELSE '' END as 'Deferred Cycle'
                                                    , Tdd_Remarks as 'Remarks'
                                                    FROM Udv_DeductionPayments 
                                                    LEFT JOIN {0}..M_User
                                                            ON Udv_DeductionPayments.Usr_Login = Mur_UserCode
                                                    INNER JOIN {0}..M_Deduction ON Mdn_DeductionCode = Tdd_DeductionCode
                                                            AND Mdn_CompanyCode = @CompanyCode
                                                    LEFT JOIN {0}..M_CodeDtl PAYTYPE ON PAYTYPE.Mcd_CodeType = 'DEDPAYTYPE' 
                                                            AND PAYTYPE.Mcd_Code = Tdd_PaymentType 
                                                            AND Mcd_CompanyCode = @CompanyCode
                                                    WHERE Tdd_IDNo = @Tdd_IDNo
                                                        AND Tdd_PaymentFlag = 1

                                                    ORDER BY Tdd_ThisPayCycle
                                                    , CASE WHEN Tdd_DeferredFlag = 1 THEN Tdd_PayCycle ELSE '' END
                                                    , Tdd_DeductionCode
													, Tdd_StartDate
                                                    , Tdd_LineNo", CentralProfile);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        #endregion

        #region <View Deleted Trail>

        public DataSet FetchAllDeletedDeductionTrailValues(string Tdh_IDNo, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Tdh_DeductionCode AS [Deduction Code]
                                    ,Tdh_StartDate AS [Start Date]
                                    ,Tdh_EndDate AS [Due Date] 
                                    ,Tdh_LineNo AS [Seq No]
                                    ,CASE WHEN ISNULL([Tdh_ApplicablePayCycle],'') = '0'
                                                        THEN 'ALL QUINCENAS'
                                                        WHEN ISNULL([Tdh_ApplicablePayCycle],'') = '1'
                                                        THEN '1ST QUINCENA'
                                                        WHEN ISNULL([Tdh_ApplicablePayCycle],'') = '2'
                                                        THEN '2ND QUINCENA'
                                                        ELSE ''
                                                        END
													    AS [Timing of Deduction]
                                    ,Tdh_DeductionAmount AS [Deduction Amount]      
                                    ,Tdh_PaymentTerms AS [Payment Term(s)]    
                                    ,Tdh_CycleAmount AS [Deduction per Cycle]               
                                    ,Tdh_PaidAmount  AS [Payment to Date]                                       
                                    ,Tdh_DeferredAmount AS [Deferred Amount]   
                                    ,CONVERT(CHAR(10), Tdh_PaymentDate, 101) AS [Payment Date]                     
                                    ,Tdh_ExemptInPayroll AS [Hold]
                                    ,CONVERT(CHAR(10), Tdh_CheckDate, 101) AS [Check Date]
                                    ,Tdh_VoucherNumber AS [Voucher Number]
                                    ,Tdh_AgreementNo AS [Agreement Number]
                                    ,ISNULL([Tdh_PrincipalAmount], 0) AS [Principal Amount]
                                    ,CONVERT(CHAR(10), Tdh_EntryDate, 101) AS [Entry Date]
                                    ,Tdh_DocumentNo AS [Document Number]
                                    ,CASE Tdh_RecordStatus
                                                        WHEN 'D' then 'DELETED' 
                                                        WHEN 'A' then 'ADDED' 
                                                        WHEN 'A' then 'ADDED' 
                                                        WHEN 'U' then 'BATCH UPLOADED'
                                                        WHEN 'D' then 'DELETED'
                                                      END AS [Record Status] 
                                    ---,LEFT(Mur_UserFirstName,1) + LEFT(Mur_UserMiddleName,1) + RTRIM(Mur_UserLastName) as Usr_Login        
                                    ---,T_EmpDeductionHdrTrl.Ludatetime 
                                    FROM T_EmpDeductionHdrTrl LEFT JOIN M_User
                                    ON T_EmpDeductionHdrTrl.Usr_Login = Mur_UserCode
                                    WHERE Tdh_IDNo = @Tdh_IDNo
                                    AND Tdh_RecordStatus = 'D'
                                    ORDER BY Tdh_LineNo ASC";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        #endregion

        public DataTable GetDeductionPayType(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcd_Name
                                            FROM M_CodeDtl 
                                            WHERE Mcd_CodeType = 'DEDPAYTYPE'
                                            AND Mcd_Code NOT IN ('P', 'S', 'Z')
                                            AND Mcd_RecordStatus = 'A'
                                            AND Mcd_CompanyCode = '{0}'", CompanyCode);
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetDeductionType() 
        {  
            string query = @"SELECT 0 AS 'Deduction Type', 'NON RECURRING' As 'Deduction Name'
                            UNION 
                            SELECT 1 AS 'Deduction Type', 'RECURRING' As 'Deduction Name'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDeductionCodes(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                                SELECT DISTINCT Tdh_DeductionCode AS 'Deduction Code'
                                    , Mdn_DeductionName As 'Deduction Name'
                            FROM {1}..T_EmpDeductionHdr
                            INNER JOIN M_Employee
                                ON Mem_IDNo = Tdh_IDNo
                            INNER JOIN {1}..M_Deduction 
                                ON Mdn_DeductionCode = Tdh_DeductionCode
                                AND Mdn_CompanyCode = '{0}'
                                AND Mdn_RecordStatus = 'A'", CompanyCode, CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDeductionGroup(string condition, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                            SELECT Mcd_Code AS 'Deduction Group'
                                    , Mcd_Name As 'Deduction Name'
                            FROM M_CodeDtl
                            WHERE Mcd_CodeType = 'DEDNGRP'
                                {0}
                                AND Mcd_CompanyCode = '{1}'
                                AND Mcd_RecordStatus = 'A'", condition, CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeDetails(string EmployeeID, string CompanyCode, string CentralProfile)
        {
            #region query

            string query = string.Format(@"SELECT Mem_IDNo as [ID Number]
                                , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
								     THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_Firstname + ' ' + Mem_MiddleName as [Name]
                                ----, Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
								----								THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                ----, Mem_FirstName as Mem_FirstName
								----, Mem_MiddleName as Mem_MiddleName
                                , PAYGRP.Mcd_Name as [Payroll Group]
                                , EmploymentStatus.Mcd_Name as [Employment Status]
								, PayrollType.Mcd_Name as [Payroll Type]
                                , Position.Mcd_Name as Position
								, Mem_CostcenterCode as [Cost Center Code]
                                , {0}.dbo.Udf_DisplayCostCenterName('{2}',Mem_CostcenterCode,'{3}') as [Cost Center Name]
                                , CONVERT(CHAR(10), Mem_IntakeDate, 101) as [Hire Date]
	                            , CONVERT(CHAR(10), Mem_RegularDate, 101) as [Regular Date]
	                            , CONVERT(CHAR(10), Mem_SeparationDate, 101) as [Separation Date]
				FROM M_Employee
				LEFT OUTER JOIN {0}..M_CodeDtl as EmploymentStatus
                                        ON EmploymentStatus.Mcd_Code = Mem_EmploymentStatusCode AND
                                        EmploymentStatus.Mcd_CodeType = 'EMPSTAT' AND 
                                        EmploymentStatus.Mcd_CompanyCode = '{2}' AND
                                        EmploymentStatus.Mcd_RecordStatus = 'A'
                                    LEFT OUTER JOIN {0}..M_CodeDtl AS Position ON Position.Mcd_Code = Mem_PayrollType AND
                                        Position.Mcd_CodeType = 'POSITION' AND
                                        Position.Mcd_CompanyCode = '{2}' AND
                                        Position.Mcd_RecordStatus = 'A'
                                    LEFT OUTER JOIN {0}..M_CodeDtl AS PayrollType ON PayrollType.Mcd_Code = Mem_PayrollType AND
                                        PayrollType.Mcd_CodeType = 'PAYTYPE' AND
                                        PayrollType.Mcd_CompanyCode = '{2}' AND
                                        PayrollType.Mcd_RecordStatus = 'A'
                                    LEFT OUTER JOIN {0}..M_CodeDtl AS PAYGRP ON PAYGRP.Mcd_Code = Mem_PayrollGroup AND
                                        PAYGRP.Mcd_CodeType = 'PAYGRP' AND 
                                        PAYGRP.Mcd_CompanyCode = '{2}' AND
                                        PAYGRP.Mcd_RecordStatus = 'A'
			    WHERE Mem_IDNo = '{1}'"
                        , CentralProfile
                        , EmployeeID
                        , CompanyCode
                        , (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));

            #endregion
            DataTable dt = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dt;
        }
    }
}
