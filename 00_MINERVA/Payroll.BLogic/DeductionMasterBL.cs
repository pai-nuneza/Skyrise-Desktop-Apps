using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class DeductionMasterBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[16];
            paramInfo[0] = new ParameterInfo("@Mdn_DeductionCode", row["Mdn_DeductionCode"]);
            paramInfo[1] = new ParameterInfo("@Mdn_DeductionName", row["Mdn_DeductionName"]);
            paramInfo[2] = new ParameterInfo("@Mdn_DeductionGroup", row["Mdn_DeductionGroup"]);
            paramInfo[3] = new ParameterInfo("@Mdn_PriorityNo", row["Mdn_PriorityNo"]);
            paramInfo[4] = new ParameterInfo("@Mdn_IsAutoComputeAmort", row["Mdn_IsAutoComputeAmort"]);
            paramInfo[5] = new ParameterInfo("@Mdn_WithCheckDate", row["Mdn_WithCheckDate"]);
            paramInfo[6] = new ParameterInfo("@Mdn_WithAccountingVoucher", row["Mdn_WithAccountingVoucher"]);
            paramInfo[7] = new ParameterInfo("@Mdn_PaymentTerms", row["Mdn_PaymentTerms"]);
            paramInfo[8] = new ParameterInfo("@Mdn_ApplicablePayCycle", row["Mdn_ApplicablePayCycle"]);
            paramInfo[9] = new ParameterInfo("@Mdn_RecordStatus", row["Mdn_RecordStatus"]);
            paramInfo[10] = new ParameterInfo("@Mdn_CreatedBy", row["Mdn_CreatedBy"]);
            paramInfo[11] = new ParameterInfo("@Mdn_IsAllowDeferred", row["Mdn_IsAllowDeferred"]);
            paramInfo[12] = new ParameterInfo("@Mdn_WithPrincipalAmount", row["Mdn_WithPrincipalAmount"]);
            paramInfo[13] = new ParameterInfo("@Mdn_IsSystemReserved", row["Mdn_IsSystemReserved"]);
            paramInfo[14] = new ParameterInfo("@Mdn_IsRecurring", row["Mdn_IsRecurring"]);
            paramInfo[15] = new ParameterInfo("@Mdn_PaidUpAmount", row["Mdn_PaidUpAmount"]);

            string statement = @"INSERT INTO M_Deduction
                                            (Mdn_DeductionCode,
                                             Mdn_DeductionName,
                                             Mdn_DeductionGroup,
                                             Mdn_PriorityNo,
                                             Mdn_IsAutoComputeAmort,
                                             Mdn_WithCheckDate,
                                             Mdn_WithAccountingVoucher,
                                             Mdn_PaymentTerms,
                                             Mdn_ApplicablePayCycle,
                                             Mdn_RecordStatus,
                                             Mdn_CreatedBy,
                                             Mdn_CreatedDate,
                                             Mdn_IsAllowDeferred,
                                             Mdn_WithPrincipalAmount,
                                             Mdn_IsSystemReserved,
                                             Mdn_IsRecurring,
                                             Mdn_PaidUpAmount)

                                     VALUES (@Mdn_DeductionCode,
                                             @Mdn_DeductionName,
                                             @Mdn_DeductionGroup,
                                             @Mdn_PriorityNo,
                                             @Mdn_IsAutoComputeAmort,
                                             @Mdn_WithCheckDate,
                                             @Mdn_WithAccountingVoucher,
                                             @Mdn_PaymentTerms,
                                             @Mdn_ApplicablePayCycle,
                                             @Mdn_RecordStatus,
                                             @Mdn_CreatedBy,
                                             GETDATE(),
                                             @Mdn_IsAllowDeferred,
                                             @Mdn_WithPrincipalAmount,
                                             @Mdn_IsSystemReserved,
                                             @Mdn_IsRecurring,
                                             @Mdn_PaidUpAmount)";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(statement, CommandType.Text, paramInfo);
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
        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[15];
            paramInfo[0] = new ParameterInfo("@Mdn_DeductionCode", row["Mdn_DeductionCode"]);
            paramInfo[1] = new ParameterInfo("@Mdn_DeductionName", row["Mdn_DeductionName"]);
            paramInfo[2] = new ParameterInfo("@Mdn_DeductionGroup", row["Mdn_DeductionGroup"]);
            paramInfo[3] = new ParameterInfo("@Mdn_PriorityNo", row["Mdn_PriorityNo"]);
            paramInfo[4] = new ParameterInfo("@Mdn_IsAutoComputeAmort", row["Mdn_IsAutoComputeAmort"]);
            paramInfo[5] = new ParameterInfo("@Mdn_WithCheckDate", row["Mdn_WithCheckDate"]);
            paramInfo[6] = new ParameterInfo("@Mdn_WithAccountingVoucher", row["Mdn_WithAccountingVoucher"]);
            paramInfo[7] = new ParameterInfo("@Mdn_PaymentTerms", row["Mdn_PaymentTerms"]);
            paramInfo[8] = new ParameterInfo("@Mdn_ApplicablePayCycle", row["Mdn_ApplicablePayCycle"]);
            paramInfo[9] = new ParameterInfo("@Mdn_RecordStatus", row["Mdn_RecordStatus"]);
            paramInfo[10] = new ParameterInfo("@Mdn_UpdatedBy", row["Mdn_UpdatedBy"]);
            paramInfo[11] = new ParameterInfo("@Mdn_IsAllowDeferred", row["Mdn_IsAllowDeferred"]);
            paramInfo[12] = new ParameterInfo("@Mdn_WithPrincipalAmount", row["Mdn_WithPrincipalAmount"]);
            paramInfo[13] = new ParameterInfo("@Mdn_IsRecurring", row["Mdn_IsRecurring"]);
            paramInfo[14] = new ParameterInfo("@Mdn_PaidUpAmount", row["Mdn_PaidUpAmount"]);

            string statement = @"UPDATE M_Deduction
                                 SET Mdn_DeductionName = @Mdn_DeductionName,
                                     Mdn_DeductionGroup = @Mdn_DeductionGroup,
                                     Mdn_PriorityNo = @Mdn_PriorityNo,
                                     Mdn_IsAutoComputeAmort = @Mdn_IsAutoComputeAmort,
                                     Mdn_WithCheckDate = @Mdn_WithCheckDate,
                                     Mdn_WithAccountingVoucher = @Mdn_WithAccountingVoucher,
                                     Mdn_PaymentTerms = @Mdn_PaymentTerms,
                                     Mdn_ApplicablePayCycle = @Mdn_ApplicablePayCycle,
                                     Mdn_RecordStatus = @Mdn_RecordStatus,
                                     Mdn_UpdatedBy = @Mdn_UpdatedBy,
                                     Mdn_UpdatedDate = GETDATE(),
                                     Mdn_IsAllowDeferred = @Mdn_IsAllowDeferred,
                                     Mdn_WithPrincipalAmount = @Mdn_WithPrincipalAmount,
                                     Mdn_IsRecurring = @Mdn_IsRecurring,
                                     Mdn_PaidUpAmount = @Mdn_PaidUpAmount
                                 WHERE Mdn_DeductionCode = @Mdn_DeductionCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(statement, CommandType.Text, paramInfo);
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
        public int Update(System.Data.DataRow row, string LServCode, string LUserCode)
        {
            int retVal = 0;

            return retVal;
        }
        public override int Delete(string deductcode, string Userlogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@deductcode", deductcode);
            paramInfo[1] = new ParameterInfo("@Userlogin", Userlogin);

            string sqlQuery = @"UPDATE M_Deduction 
                                               SET 
                                                  Mdn_RecordStatus='C',
                                                  Usr_Login = @Userlogin, 
                                                  ludatetime = GetDate()
                                               WHERE Mdn_DeductionCode=@deductcode        
                                                         ";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
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
        public override System.Data.DataSet FetchAll()
        {
            DataSet ds = new DataSet();
            string statement = @"select 
                                    Mdn_DeductionCode
                                    ,Mdn_DeductionName
                                    ,Mdn_DeductionGroup
                                    ,Mdn_PriorityNo
                                    ,Mdn_IsAutoComputeAmort
                                    ,Mdn_WithCheckDate
                                    ,Mdn_WithAccountingVoucher
                                    ,Mdn_PaymentTerms
                                    ,ApplicPayPeriod.Mcd_Name as [Mdn_ApplicablePayCycle]
                                    ,Mdn_RecordStatus
                                    ,DdcnCM.Mdn_UpdatedBy
                                    ,DdcnCM.Mdn_UpdatedDate
                                    ,Mdn_ApplicablePayCycle as [Dcm_ApplicablePayrollPeriodCode]
                                    ,DeductionType.Mcd_Name as Mdn_DeductionGroupDesc
                                    ,ISNULL(Mdn_IsAllowDeferred, 'false') as Mdn_IsAllowDeferred
                                    ,ISNULL(Mdn_WithPrincipalAmount, 'false') as Mdn_WithPrincipalAmount
                                    ,Mdn_AlphalistCategory
                                    ,Alphalist.Mcd_Name as Mdn_AlphalistCategoryDesc
                                    ,Mdn_IsSystemReserved
                                    ,Mdn_IsRecurring
                                    ,Mdn_PaidUpAmount
                                    from M_Deduction as DdcnCM
                                    left join M_CodeDtl ApplicPayPeriod on ApplicPayPeriod.Mcd_CodeType ='PAYFREQ' and ApplicPayPeriod.Mcd_Code = Mdn_ApplicablePayCycle
                                    left join M_CodeDtl DeductionType on DeductionType.Mcd_CodeType ='DEDNGRP' and DeductionType.Mcd_Code = Mdn_DeductionGroup
                                    left join M_CodeDtl Alphalist on Alphalist.Mcd_CodeType ='DALPHACATGY' and Alphalist.Mcd_Code = Mdn_AlphalistCategory";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(statement, CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        

        public System.Data.DataRow ifDeductionExist(string deductcode)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@DeductionCode", deductcode, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mdn_DeductionCode FROM M_Deduction WHERE Mdn_DeductionCode=@DeductionCode", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public System.Data.DataRow ifDeductionDescExist(string deductdesc,string deductcode)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[2];
                paramCollection[0] = new ParameterInfo("@DeductionCode", deductcode, SqlDbType.Char, 10);
                paramCollection[1] = new ParameterInfo("@Deductiondesc", deductdesc, SqlDbType.Char, 50);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mdn_DeductionName FROM M_Deduction WHERE Mdn_DeductionName=@Deductiondesc AND Mdn_DeductionCode<>@DeductionCode", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public bool CheckIfAllowDelete(string deductioncode)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@deductioncode", deductioncode);

            string qstring = @"Select Distinct Tdh_DeductionCode From T_EmpDeductionHdr
                               Where Tdh_DeductionCode = @deductioncode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qstring, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return false;
            else
                return true;
        }
        
        public DataSet GetHeaderData(string Mdn_DeductionGroup)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"Select Mcm_CompanyName
                                  ,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ', ' + Mcd_Name as Address
                                  ,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
                                  ,Mcm_CompanyLogo
                                  ,CASE 
		                            WHEN @Mdn_DeductionGroup = 'C' THEN 'COMPANY'
		                            WHEN @Mdn_DeductionGroup = 'G' THEN 'GOVERNMENT MANDATED'
		                            WHEN @Mdn_DeductionGroup = 'L' THEN 'GOVERNMENT LOANS'
		                            WHEN @Mdn_DeductionGroup = 'E' THEN 'EXTERNAL PARTY'
                                    WHEN @Mdn_DeductionGroup = 'ALL' THEN 'ALL'
                                  END as DeductionDesc
                            From M_Company
                            Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code and Mcd_CodeType='ZIPCODE'";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Mdn_DeductionGroup", Mdn_DeductionGroup);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetDetail(string Status, string Mdn_DeductionGroup)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"SELECT Mdn_DeductionCode
                    ,Mdn_DeductionName
                    ,a.Mcd_Name AS Mdn_DeductionGroup
                    ,Mdn_PriorityNo
                    ,Mdn_IsAutoComputeAmort
                    ,Mdn_WithCheckDate
                    ,Mdn_WithAccountingVoucher
                    ,Mdn_PaymentTerms
                    , b.Mcd_Name AS Mdn_ApplicablePayCycle 
                    , CASE
                       WHEN Mdn_RecordStatus = 'A'  THEN 'ACTIVE'
                       WHEN Mdn_RecordStatus = 'C'  THEN 'CANCELLED'
                    END AS Mdn_RecordStatus            
                FROM M_Deduction
                LEFT JOIN M_CodeDtl a on a.Mcd_Code = Mdn_DeductionGroup
	                and a.Mcd_CodeType ='DEDNGRP'
                LEFT JOIN M_CodeDtl b on b.Mcd_Code = Mdn_ApplicablePayCycle
	                and b.Mcd_CodeType ='PAYFREQ'
                WHERE (Mdn_RecordStatus = @Status
                    OR @Status = 'ALL')
                    AND (Mdn_DeductionGroup = @Mdn_DeductionGroup or @Mdn_DeductionGroup = 'ALL')";
            #endregion

            ParameterInfo[] param = new ParameterInfo[2];
            param[paramIndex++] = new ParameterInfo("@Status", Status);
            param[paramIndex++] = new ParameterInfo("@Mdn_DeductionGroup", Mdn_DeductionGroup);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }


        public int UpdateApplicablePeriod(string deductioncode, string ApplicablePayrollPeriod, string Userlogin)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@deductioncode", deductioncode);
            paramInfo[1] = new ParameterInfo("@ApplicablePayrollPeriod", ApplicablePayrollPeriod);
            paramInfo[2] = new ParameterInfo("@Userlogin", Userlogin);

            string statement = @"UPDATE M_Deduction
                                 SET Mdn_ApplicablePayCycle = @ApplicablePayrollPeriod,
                                     Usr_Login = @Userlogin,
                                     Ludatetime = GETDATE()
                                 WHERE Mdn_DeductionCode = @deductioncode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(statement, CommandType.Text, paramInfo);
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

        public System.Data.DataRow ifDeductionDescriptionExist(string deductcode, string DeductionDesc)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[2];
                paramCollection[0] = new ParameterInfo("@DeductionCode", deductcode, SqlDbType.Char, 10);
                paramCollection[1] = new ParameterInfo("@DeductionDesc", DeductionDesc);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mdn_DeductionCode FROM M_Deduction WHERE Mdn_DeductionCode!=@DeductionCode AND Mdn_DeductionName=@DeductionDesc", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public string GetDeductionType(string DeductionCode)
        {
            string dedType = string.Empty;

            string sqlQuery = string.Format(@"  SELECT Mdn_DeductionGroup FROM M_Deduction WHERE Mdn_DeductionCode = '{0}'", DeductionCode);
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                dedType = ds.Tables[0].Rows[0][0].ToString();

            return dedType;
        }

    }
}
