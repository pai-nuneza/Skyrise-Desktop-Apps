using System;
using System.Data;
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

            ParameterInfo[] paramInfo = new ParameterInfo[10];
            paramInfo[0] = new ParameterInfo("@Cbm_CompanyAccNo", row["Cbm_CompanyAccNo"]);
            paramInfo[1] = new ParameterInfo("@Cbm_BankCode", row["Cbm_BankCode"]);
            paramInfo[2] = new ParameterInfo("@Cbm_BankCompanyCode", row["Cbm_BankCompanyCode"]);
            paramInfo[3] = new ParameterInfo("@Cbm_BankBranchCode", row["Cbm_BankBranchCode"]);
            paramInfo[4] = new ParameterInfo("@Cbm_BankAddress1", row["Cbm_BankAddress1"]);
            paramInfo[5] = new ParameterInfo("@Cbm_BankAddress2", row["Cbm_BankAddress2"]);
            paramInfo[6] = new ParameterInfo("@Cbm_BankAddress3", row["Cbm_BankAddress3"]);
            paramInfo[7] = new ParameterInfo("@Cbm_PersonInCharge", row["Cbm_PersonInCharge"]);
            paramInfo[8] = new ParameterInfo("@Cbm_PersonPosition", row["Cbm_PersonPosition"]);
            paramInfo[9] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            string statement = @"INSERT INTO T_CompanyBankMaster
                                 VALUES (@Cbm_CompanyAccNo,
                                     @Cbm_BankCode,
                                     @Cbm_BankCompanyCode,
                                     @Cbm_BankBranchCode,
                                     @Cbm_BankAddress1,
                                     @Cbm_BankAddress2,
                                     @Cbm_BankAddress3,
                                     @Cbm_PersonInCharge,
                                     @Cbm_PersonPosition,
                                     'A',
                                     @Usr_Login,
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

            ParameterInfo[] paramInfo = new ParameterInfo[11];
            //paramInfo[0] = new ParameterInfo("@RecordNo", row["RecordNo"]);
            paramInfo[0] = new ParameterInfo("@Cbm_CompanyAccNo", row["Cbm_CompanyAccNo"]);
            paramInfo[1] = new ParameterInfo("@Cbm_BankCode", row["Cbm_BankCode"]);
            paramInfo[2] = new ParameterInfo("@Cbm_BankCompanyCode", row["Cbm_BankCompanyCode"]);
            paramInfo[3] = new ParameterInfo("@Cbm_BankBranchCode", row["Cbm_BankBranchCode"]);
            paramInfo[4] = new ParameterInfo("@Cbm_BankAddress1", row["Cbm_BankAddress1"]);
            paramInfo[5] = new ParameterInfo("@Cbm_BankAddress2", row["Cbm_BankAddress2"]);
            paramInfo[6] = new ParameterInfo("@Cbm_BankAddress3", row["Cbm_BankAddress3"]);
            paramInfo[7] = new ParameterInfo("@Cmb_Status", row["Cmb_Status"]);
            paramInfo[8] = new ParameterInfo("@Cbm_PersonInCharge", row["Cbm_PersonInCharge"]);
            paramInfo[9] = new ParameterInfo("@Cbm_PersonPosition", row["Cbm_PersonPosition"]);
            paramInfo[10] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            string statement = @"UPDATE T_CompanyBankMaster
                                 SET 
                                     Cbm_BankCode = @Cbm_BankCode,
                                     Cbm_BankCompanyCode = @Cbm_BankCompanyCode,
                                     Cbm_BankBranchCode = @Cbm_BankBranchCode,
                                     Cbm_BankAddress1 = @Cbm_BankAddress1,
                                     Cbm_BankAddress2 = @Cbm_BankAddress2,
                                     Cbm_BankAddress3 = @Cbm_BankAddress3,
                                     Cbm_PersonInCharge = @Cbm_PersonInCharge,
                                     Cbm_PersonPosition = @Cbm_PersonPosition,
                                     Cmb_Status = @Cmb_Status,
                                     Usr_Login = @Usr_Login,
                                     Ludatetime = GETDATE()
                                 WHERE Cbm_CompanyAccNo = @Cbm_CompanyAccNo";    
    
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

        public override int Delete(string Cbm_CompanyAccNo, string User_Login)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Cbm_CompanyAccNo", Cbm_CompanyAccNo);
            paramInfo[1] = new ParameterInfo("@Usr_Login", User_Login);
            string sqlQuery = @"UPDATE T_CompanyBankMaster 
                              SET 
                              Cmb_Status='C',
                              Usr_Login = @Usr_Login, 
                              Ludatetime = GetDate()
                              WHERE Cbm_CompanyAccNo=@Cbm_CompanyAccNo        
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


            string sqlQuery = @"select 
                                A.*
                                ,B.Adt_AccountDesc as Cmb_BankName
                                 from T_CompanyBankMaster A 
                                left join T_AccountDetail B 
                                on B.Adt_AccountCode = A.Cbm_BankCode 
                                and B.Adt_AccountType = 'BANK'
                                order by A.Cbm_BankCode ASC";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                }
                catch (Exception Error)
                {
                    CommonProcedures.showMessageError(Error.Message);
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
         *               exists in the T_CompanyBankMaster table
         * Parameters  : Input : string Cbm_CompanyAccNo                                                            
         *                                                                  
         *               Output: None                                  
         * Return      : bool (true if exist, false in not)                                                    
         * Author      : Toby A. Trazo                                      
         * Date Created: October 12, 2010                                   
        /*===========================================================================*/
        public bool CheckIfAllowDelete(string Cbm_CompanyAccNo)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Cbm_CompanyAccNo", Cbm_CompanyAccNo);

            string qstring = @"Select  Cbm_CompanyAccNo From T_CompanyBankMaster
                               Where Cbm_CompanyAccNo = @Cbm_CompanyAccNo";

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
