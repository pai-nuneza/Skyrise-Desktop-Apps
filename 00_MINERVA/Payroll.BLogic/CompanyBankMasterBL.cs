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
    public class CompanyBankMasterBL : BaseBL
    {
        #region <Override Functions>
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Mbn_CompanyAccountNo", row["Mbn_CompanyAccountNo"]);
            //paramInfo[1] = new ParameterInfo("@Cbm_BankCode", row["Cbm_BankCode"]);
            paramInfo[1] = new ParameterInfo("@Mbn_BankName", row["Mbn_BankName"]);
            paramInfo[2] = new ParameterInfo("@Mbn_BankCode", row["Mbn_BankCode"]);
            paramInfo[3] = new ParameterInfo("@Mbn_BankAddress", row["Mbn_BankAddress"], SqlDbType.VarChar, row["Mbn_BankAddress"].ToString().Length);
            paramInfo[4] = new ParameterInfo("@Mbn_DepositoryBranchCode", row["Mbn_DepositoryBranchCode"], SqlDbType.VarChar, row["Mbn_DepositoryBranchCode"].ToString().Length);
            paramInfo[5] = new ParameterInfo("@Mbn_CompanyCodeIssuedByBank", row["Mbn_CompanyCodeIssuedByBank"], SqlDbType.VarChar, row["Mbn_CompanyCodeIssuedByBank"].ToString().Length);
            paramInfo[6] = new ParameterInfo("@Mbn_BankInChargeName", row["Mbn_BankInChargeName"], SqlDbType.VarChar, row["Mbn_BankInChargeName"].ToString().Length);
            paramInfo[7] = new ParameterInfo("@Mbn_BankInChargeJobTitle", row["Mbn_BankInChargeJobTitle"]);
            paramInfo[8] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[9] = new ParameterInfo("@Mbn_BankType", row["Mbn_BankType"], SqlDbType.VarChar, row["Mbn_BankType"].ToString().Length);
            paramInfo[10] = new ParameterInfo("@Mbn_BankGenerationRule", row["Mbn_BankGenerationRule"], SqlDbType.VarChar, row["Mbn_BankGenerationRule"].ToString().Length);
            paramInfo[11] = new ParameterInfo("@Mbn_BranchCodeIssuedByBank", row["Mbn_BranchCodeIssuedByBank"], SqlDbType.VarChar, row["Mbn_BranchCodeIssuedByBank"].ToString().Length);
            string statement = @"INSERT INTO M_Bank
                                 VALUES (@Mbn_BankCode
                                      ,@Mbn_BankName
                                      ,@Mbn_BankAddress
                                      ,@Mbn_BankType
                                      ,@Mbn_DepositoryBranchCode
                                      ,@Mbn_CompanyAccountNo
                                      ,@Mbn_BankGenerationRule
                                      ,@Mbn_CompanyCodeIssuedByBank
                                      ,@Mbn_BranchCodeIssuedByBank
                                      ,@Mbn_BankInChargeName
                                      ,@Mbn_BankInChargeJobTitle
                                     ,'A'
                                     ,@Usr_Login,
                                     GETDATE())";
   
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

            ParameterInfo[] paramInfo = new ParameterInfo[13];
            //paramInfo[0] = new ParameterInfo("@RecordNo", row["RecordNo"]);
            paramInfo[0] = new ParameterInfo("@Mbn_CompanyAccountNo", row["Mbn_CompanyAccountNo"]);
            //paramInfo[1] = new ParameterInfo("@Cbm_BankCode", row["@Cbm_BankCode"]);
            //paramInfo[1] = new ParameterInfo("@Cbm_BankCompanyCode", row["Cbm_BankCompanyCode"]);
            paramInfo[1] = new ParameterInfo("@Mbn_BankName", row["Mbn_BankName"]);
            paramInfo[2] = new ParameterInfo("@Mbn_BankAddress", row["Mbn_BankAddress"], SqlDbType.VarChar, row["Mbn_BankAddress"].ToString().Length);
            paramInfo[3] = new ParameterInfo("@Mbn_DepositoryBranchCode", row["Mbn_DepositoryBranchCode"], SqlDbType.VarChar, row["Mbn_DepositoryBranchCode"].ToString().Length);
            paramInfo[4] = new ParameterInfo("@Mbn_CompanyCodeIssuedByBank", row["Mbn_CompanyCodeIssuedByBank"], SqlDbType.VarChar, row["Mbn_CompanyCodeIssuedByBank"].ToString().Length);
            paramInfo[5] = new ParameterInfo("@Mbn_RecordStatus", row["Mbn_RecordStatus"]);
            paramInfo[6] = new ParameterInfo("@Mbn_BankInChargeName", row["Mbn_BankInChargeName"], SqlDbType.VarChar, row["Mbn_BankInChargeName"].ToString().Length);
            paramInfo[7] = new ParameterInfo("@Mbn_BankInChargeJobTitle", row["Mbn_BankInChargeJobTitle"]);
            paramInfo[8] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[9] = new ParameterInfo("@Mbn_BankType", row["Mbn_BankType"], SqlDbType.VarChar, row["Mbn_BankType"].ToString().Length);
            paramInfo[10] = new ParameterInfo("@Mbn_BankGenerationRule", row["Mbn_BankGenerationRule"], SqlDbType.VarChar, row["Mbn_BankGenerationRule"].ToString().Length);
            paramInfo[11] = new ParameterInfo("@Mbn_BranchCodeIssuedByBank", row["Mbn_BranchCodeIssuedByBank"], SqlDbType.VarChar, row["Mbn_BranchCodeIssuedByBank"].ToString().Length);
            paramInfo[12] = new ParameterInfo("@Mbn_BankCode", row["Mbn_BankCode"], SqlDbType.VarChar, row["Mbn_BankCode"].ToString().Length);
            string statement = @"UPDATE M_Bank
                                 SET 
                                     --Cbm_BankCode = @Cbm_BankCode,
                                     --Cbm_BankCompanyCode = @Cbm_BankCompanyCode,
                                     Mbn_BankAddress = @Mbn_BankAddress,
                                     Mbn_DepositoryBranchCode = @Mbn_DepositoryBranchCode,
                                     Mbn_CompanyAccountNo = @Mbn_CompanyAccountNo,
                                     Mbn_BankInChargeName = @Mbn_BankInChargeName,
                                     Mbn_BankInChargeJobTitle = @Mbn_BankInChargeJobTitle,
                                     Mbn_RecordStatus = @Mbn_RecordStatus,
                                     Mbn_BankGenerationRule = @Mbn_BankGenerationRule,
                                     Mbn_CompanyCodeIssuedByBank = @Mbn_CompanyCodeIssuedByBank,
                                     Mbn_BranchCodeIssuedByBank = @Mbn_BranchCodeIssuedByBank,
                                     Mbn_BankType = @Mbn_BankType,
                                     Mbn_BankName = @Mbn_BankName,
                                     Usr_Login = @Usr_Login,
                                     Ludatetime = GETDATE()
                                 WHERE --Mbn_CompanyAccountNo = @Mbn_CompanyAccountNo                                    
                                     Mbn_BankCode = @Mbn_BankCode";    
    
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

        public override int Delete(string @Mbn_CompanyAccountNo, string Usr_Login)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@@Mbn_CompanyAccountNo", @Mbn_CompanyAccountNo);
            paramInfo[1] = new ParameterInfo("@Usr_Login", Usr_Login);
            string sqlQuery = @"DELETE FROM M_Bank 
                              WHERE Mbn_CompanyAccountNo=@@Mbn_CompanyAccountNo        
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
            return ds;
        }

        public System.Data.DataSet FetchAllBanks()
        {
            DataSet ds = new DataSet();


            string sqlQuery = @"--select 
                                --A.*
                                --,B.Mcd_Name as Cmb_BankName
                                 --from M_Bank A 
                                --left join M_CodeDtl B 
                                --on B.Mcd_Code = A.@Cbm_BankCode 
                                --and B.Mcd_CodeType = 'BANK'
                                --WHERE Cmb_Status = 'A'
                                --order by A.@Cbm_BankCode ASC
SELECT [Mbn_BankCode] 
      ,[Mbn_BankName]
      ,[Mbn_BankAddress]
      ,CASE WHEN [Mbn_BankType] = 'C' 
        THEN 'COMPANY DEPOSITORY'
        WHEN [Mbn_BankType] = 'E'
	  THEN 'EMPLOYEE PAYROLL ACCOUNT'
	  WHEN [Mbn_BankType] = 'B'
	  THEN 'BOTH FOR COMPANY AND EMPLOYEE ACCOUNT'
	  ELSE ''
	  END Mbn_BankType
      ,[Mbn_DepositoryBranchCode]
      ,[Mbn_CompanyAccountNo]
      ,[Mbn_BankGenerationRule]
      ,[Mbn_CompanyCodeIssuedByBank]
      ,[Mbn_BranchCodeIssuedByBank]
      ,[Mbn_BankInChargeName]
      ,[Mbn_BankInChargeJobTitle]
      ,[Mbn_RecordStatus]
  FROM [M_Bank]";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                }
                catch (Exception Error)
                {
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(Error);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

      

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
        #region <Defined Functions>

        /*===========================================================================
         * Function    : bool CheckIfAllowDelete(string transNo)   
         * Purpose     : This function checks if the transaction actually existis in
         *               exists in the M_Bank table
         * Parameters  : Input : string @Mbn_CompanyAccountNo                                                            
         *                                                                  
         *               Output: None                                  
         * Return      : bool (true if exist, false in not)                                                                                    
        /*===========================================================================*/
        public bool CheckIfAllowDelete(string @Mbn_CompanyAccountNo)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@@Mbn_CompanyAccountNo", @Mbn_CompanyAccountNo);

            string qstring = @"Select  @Mbn_CompanyAccountNo From M_Bank
                               Where @Mbn_CompanyAccountNo = @@Mbn_CompanyAccountNo";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qstring, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
        /*===========================================================================
         * Function    : CheckIfExist(string tableName, string fieldname1, string 
         *               value1, string fieldName2, string value2) 
         * Purpose     : This function is a generic checking of record
         * Parameters  : Input : string tableName  = source table                                                            
         *                       string fieldname1 = 1st constant field 
         *                       string value1     = 1st value
         *                       string fieldName2 = 2nd constant field
         *                       string value2     = 2nd value
         *               Output: None                                  
         * Return      : bool (true if exist, false in not)                                                    
         * Author      : Toby A. Trazo                                      
         * Date Created: October 12, 2010                                   
        /*===========================================================================*/
        public bool CheckIfExist(string tableName, string fieldname1, string value1)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@value1", value1);
            //paramInfo[1] = new ParameterInfo("@value2", value2);
            string qstring = @"Select 1 From " + tableName +
                               " Where " + fieldname1 + " = @value1";

            

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qstring, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
            
        #endregion
    }
}
