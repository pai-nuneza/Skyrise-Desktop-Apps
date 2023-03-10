using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class AllowanceMasterBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Min_IncomeCode", row["Min_IncomeCode"]);
            paramInfo[1] = new ParameterInfo("@Min_IncomeName", row["Min_IncomeName"]);
            paramInfo[2] = new ParameterInfo("@Min_TaxClass", row["Min_TaxClass"]);
            paramInfo[3] = new ParameterInfo("@Min_IsRecurring", row["Min_IsRecurring"]);
            paramInfo[4] = new ParameterInfo("@Min_RecordStatus", row["Min_RecordStatus"]);
            paramInfo[5] = new ParameterInfo("@Min_CreatedBy", row["Min_CreatedBy"]);
            paramInfo[6] = new ParameterInfo("@Min_ApplicablePayCycle", row["Min_ApplicablePayCycle"]);
            paramInfo[7] = new ParameterInfo("@Min_AlphalistCategory", row["Min_AlphalistCategory"]);
            paramInfo[8] = new ParameterInfo("@Min_IncomeGroup", row["Min_IncomeGroup"]);
            paramInfo[9] = new ParameterInfo("@Min_IsSystemReserved", row["Min_IsSystemReserved"]);
            paramInfo[10] = new ParameterInfo("@Min_AccountGrp", row["Min_AccountGrp"]);
            paramInfo[11] = new ParameterInfo("@Min_RemittanceLoanType", row["Min_RemittanceLoanType"]);

            string sqlQuery = @"INSERT INTO M_Income
                                                 (Min_IncomeCode, 
                                                 Min_IncomeName,
                                                 Min_TaxClass,
                                                 Min_IsRecurring,
                                                 Min_RecordStatus, 
                                                 Min_CreatedBy, 
                                                 Min_CreatedDate, 
                                                 Min_ApplicablePayCycle,
                                                 Min_AlphalistCategory,
                                                 Min_IncomeGroup,
                                                 Min_IsSystemReserved,
                                                 Min_AccountGrp,
                                                 Min_RemittanceLoanType)
                                               VALUES
                                                (@Min_IncomeCode, 
                                                 @Min_IncomeName, 
                                                 @Min_TaxClass, 
                                                 @Min_IsRecurring, 
                                                 @Min_RecordStatus, 
                                                 @Min_CreatedBy, 
                                                 GETDATE(), 
                                                 @Min_ApplicablePayCycle,
                                                 @Min_AlphalistCategory,
                                                 @Min_IncomeGroup,
                                                 @Min_IsSystemReserved,
                                                 @Min_AccountGrp,
                                                 @Min_RemittanceLoanType)";

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

        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[11];
            paramInfo[0] = new ParameterInfo("@Min_IncomeCode", row["Min_IncomeCode"]);
            paramInfo[1] = new ParameterInfo("@Min_IncomeName", row["Min_IncomeName"]);
            paramInfo[2] = new ParameterInfo("@Min_TaxClass", row["Min_TaxClass"]);
            paramInfo[3] = new ParameterInfo("@Min_IsRecurring", row["Min_IsRecurring"]);
            paramInfo[4] = new ParameterInfo("@Min_RecordStatus", row["Min_RecordStatus"]);
            paramInfo[5] = new ParameterInfo("@Min_UpdatedBy", row["Min_UpdatedBy"]);
            paramInfo[6] = new ParameterInfo("@Min_ApplicablePayCycle", row["Min_ApplicablePayCycle"]);
            paramInfo[7] = new ParameterInfo("@Min_AlphalistCategory", row["Min_AlphalistCategory"]);
            paramInfo[8] = new ParameterInfo("@Min_IncomeGroup", row["Min_IncomeGroup"]);
            paramInfo[9] = new ParameterInfo("@Min_IsSystemReserved", row["Min_IsSystemReserved"]);
            paramInfo[10] = new ParameterInfo("@Min_AccountGrp", row["Min_AccountGrp"]);

            string sqlQuery = @"UPDATE M_Income
                                              SET Min_IncomeName = @Min_IncomeName, 
                                                  Min_TaxClass = @Min_TaxClass,
                                                  Min_IsRecurring = @Min_IsRecurring,
                                                  Min_ApplicablePayCycle = @Min_ApplicablePayCycle,
                                                  Min_IncomeGroup = @Min_IncomeGroup,
                                                  Min_AlphalistCategory = @Min_AlphalistCategory,
                                                  Min_IsSystemReserved = @Min_IsSystemReserved,
                                                  Min_AccountGrp= @Min_AccountGrp,
                                                  Min_RecordStatus = @Min_RecordStatus,     
                                                  Min_UpdatedBy = @Min_UpdatedBy, 
                                                  Min_UpdatedDate = GETDATE()
                                              WHERE Min_IncomeCode = @Min_IncomeCode";

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

        public override int Delete(string code, string userLogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@AllowanceCode", code, SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@user_login", userLogin, SqlDbType.Char, 15);

            string sqlQuery = @"UPDATE M_Income 
                                              SET Min_RecordStatus = 'C', 
                                                  Usr_Login = @user_Login, 
                                                  Ludatetime = GetDate() 
                                              WHERE Min_IncomeCode = @AllowanceCode";

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

            string sqlQuery = @"Select
                                Min_IncomeCode
                                ,Min_IncomeName
                                ,Min_TaxClass
                                ,Min_IsRecurring
                                ,CASE 
	                                WHEN Min_RecordStatus = 'A' THEN 'ACTIVE'
	                                WHEN Min_RecordStatus = 'C' THEN 'CANCELLED' 
                                END as 'Min_RecordStatus' 

                                ,Min_ApplicablePayCycle
                                ,A.Mcd_Name As [Acm_ApplicablePayPeriodDesc]

                                ,Min_IncomeGroup
                                ,B.Mcd_Name as [Acm_TypeDesc]

                                ,Min_AlphalistCategory
                                ,C.Mcd_Name as [AlpalistCategoryDesc]
                                
                                ,Min_IsSystemReserved [SystemReserved]
                                ,Min_AccountGrp
                                ,Min_RemittanceLoanType
                                ,ALLWNC.Min_UpdatedBy
                                ,ALLWNC.Min_UpdatedDate
                                from M_Income ALLWNC
                                left join M_CodeDtl A on Min_ApplicablePayCycle = A.Mcd_Code
                                and A.Mcd_CodeType ='PAYFREQ'    
                                left join M_CodeDtl B on Min_IncomeGroup = B.Mcd_Code
                                and B.Mcd_CodeType = 'INCOMEGRP'
                                left join M_CodeDtl C on Min_AlphalistCategory = C.Mcd_Code
                                and C.Mcd_CodeType = 'IALPHACATGY'
                                ";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            DataSet ds = new DataSet();
            
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@AllowanceCode", code, SqlDbType.Char, 10);

            string sqlQuery = @"SELECT *
                                              FROM M_Income
                                              WHERE Min_IncomeCode = @AllowanceCode
                                              AND [Min_RecordStatus] <> 'C'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public System.Data.DataRow FetchCode(string code, string companyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region Query
            string sqlQuery = string.Format(@"
                                SELECT
                                Min_IncomeCode
                                ,Min_IncomeName
                                ,CASE 
                                      WHEN Min_TaxClass = 'T' THEN '{0}'
                                      WHEN Min_TaxClass = 'N' THEN '{1}'
                                 END as Min_TaxClass 
                                ,Min_IsRecurring
                                ,CASE 
	                                WHEN Min_RecordStatus = 'A' THEN 'ACTIVE'
	                                WHEN Min_RecordStatus = 'C' THEN 'CANCELLED' 
                                END as 'Min_RecordStatus' 
                                ,Min_ApplicablePayCycle
                                ,A.Mcd_Name as [Acm_ApplicablePayPeriodDesc]
                                ,Min_IncomeGroup
                                ,B.Mcd_Name as [Acm_TypeDesc]
                                ,Min_AlphalistCategory
                                ,C.Mcd_Name as [AlpalistCategoryDesc]
                                ,Min_IsSystemReserved [SystemReserved]
                                ,Min_AccountGrp
                                ,Min_RemittanceLoanType
                                ,Min_UpdatedBy
                                ,Min_UpdatedDate
                                ,Min_CreatedBy
                                ,Min_CreatedDate
                                FROM M_Income 
                                LEFT JOIN M_CodeDtl A 
                                    ON Min_ApplicablePayCycle = A.Mcd_Code
                                    AND Min_CompanyCode = A.Mcd_CompanyCode
                                    AND A.Mcd_CodeType ='PAYFREQ'  
                                LEFT JOIN M_CodeDtl B 
                                    ON Min_IncomeGroup = B.Mcd_Code
                                    AND Min_CompanyCode = B.Mcd_CompanyCode
                                    AND B.Mcd_CodeType = 'INCOMEGRP'
                                LEFT JOIN M_CodeDtl C 
                                    ON Min_AlphalistCategory = C.Mcd_Code
                                    AND C.Mcd_CodeType = 'IALPHACATGY'
                                    AND Min_CompanyCode = C.Mcd_CompanyCode
                                WHERE Min_IncomeCode = @IncomeCode
                                    AND Min_CompanyCode = @CompanyCode
                                ", CommonEnum.TaxClass.TAXABLE
                                , CommonEnum.TaxClass.NONTAXABLE);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@IncomeCode", code, SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@CompanyCode", companyCode, SqlDbType.Char, 10);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB(); 
                //ds = dal.ExecuteDataSet(CommonConstants.Queries.checkAllowanceCodeMasterExist, CommandType.Text, paramInfo);
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public System.Data.DataRow Exist(string AllowanceCode)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@AllowanceCode", AllowanceCode, SqlDbType.Char, 10);
                
                dal.OpenDB();
                ds = dal.ExecuteDataSet(@"SELECT Min_IncomeCode 
                                          FROM M_Income 
                                          WHERE Min_IncomeCode = @AllowanceCode", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public System.Data.DataRow AllowanceDescriptionExist(string AllowanceCode, string AllowanceDescription)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[2];
                paramCollection[0] = new ParameterInfo("@AllowanceCode", AllowanceCode, SqlDbType.Char, 10);
                paramCollection[1] = new ParameterInfo("@AllowanceDescription", AllowanceDescription);

                dal.OpenDB();
                ds = dal.ExecuteDataSet(@"SELECT Min_IncomeCode 
                                          FROM M_Income 
                                          WHERE Min_IncomeCode != @AllowanceCode
                                            AND Min_IncomeName = @AllowanceDescription", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public int DeleteAllowanceEntry(string Tin_IDNo, string Tin_PayCycle, string Tin_IncomeCode, string Tin_OrigPayCycle, string AllowanceTable)
        {
            int retVal = 0;

            #region Parameters

            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@Tin_IDNo", Tin_IDNo);
            param[1] = new ParameterInfo("@Tin_PayCycle", Tin_PayCycle);
            param[2] = new ParameterInfo("@Tin_OrigPayCycle", Tin_OrigPayCycle);
            param[3] = new ParameterInfo("@Tin_IncomeCode", Tin_IncomeCode);

            #endregion

            #region query

            string statement = string.Format(@"Delete From {0}
                                 WHERE Tin_IDNo = @Tin_IDNo
                                 AND Tin_PayCycle = @Tin_PayCycle
                                 AND Tin_OrigPayCycle = @Tin_OrigPayCycle
                                 AND Tin_IncomeCode = @Tin_IncomeCode", AllowanceTable);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(statement, CommandType.Text, param);
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

        public bool CheckIfAllowDelete(string allowancecode)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@allowancecode", allowancecode, SqlDbType.Char, 25);

            string qstring = @"SELECT DISTINCT Tin_IncomeCode
                               FROM T_EmpIncome
                               WHERE Tin_IncomeCode  = @allowancecode

                               UNION ALL

                               SELECT DISTINCT Tta_IncomeCode
                               FROM T_EmpTimeBaseAllowanceCycle
                               WHERE Tta_IncomeCode = @allowancecode
                               
                               UNION ALL

                               SELECT DISTINCT Tfa_AllowanceCode
                               FROM T_EmpFixAllowance
                               WHERE Tfa_AllowanceCode = @allowancecode
                               and Tfa_RecordStatus = 'A'

                ";

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

        public bool CheckIfAllowEdit(string allowancecode)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@allowancecode", allowancecode, SqlDbType.Char, 25);

            string qstring = @"
                               SELECT DISTINCT Tin_IncomeCode
                               FROM T_EmpIncomeHst
                               WHERE Tin_IncomeCode = @allowancecode
                               AND Tin_PostFlag = 1
                ";

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

        public DataSet GetHeaderData(string Status)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"Select Mcm_CompanyName
,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ', ' + Mcd_Name as Address
,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
,Mcm_CompanyLogo, @Status as Status
From M_Company
Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code
and Mcd_CodeType='ZIPCODE'";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Status", Status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetDetailData(string Status)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"SELECT Min_IncomeCode
	                                ,Min_IncomeName
	                                ,CASE 
		                                WHEN Min_TaxClass = 'T' THEN 'TAX'
		                                WHEN Min_TaxClass = 'N' THEN 'NON TAX'
	                                 END AS Min_TaxClass
	                                ,CASE 
		                                WHEN Min_IsRecurring = 0 THEN 'No'
		                                WHEN Min_IsRecurring = 1 THEN 'Yes'
	                                 END Min_IsRecurring
	                                ,CASE 
		                                WHEN Min_LinkToLeave = 0 THEN 'No'
		                                WHEN Min_LinkToLeave = 1 THEN 'Yes'
	                                 END Min_LinkToLeave
                                    ,Min_AlphalistCategory as 'AlphalistCategory'
                                    ,CASE
                                        WHEN Min_RecordStatus = 'A'  THEN 'ACTIVE'
                                        WHEN Min_RecordStatus = 'C'  THEN 'CANCELLED'
                                     END AS Min_RecordStatus
									,Mcd_Name as 'ApplicPayPeriod'
	                                  FROM M_Income
	                                  LEFT JOIN M_CodeDtl ON Mcd_CodeType = 'PAYFREQ' 
											AND Mcd_Code = Min_ApplicablePayCycle
		                                WHERE Min_RecordStatus = @status
			                                 OR @status = 'ALL'";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Status", Status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        public char GetBracketBasis(string LedgerAlwCol)
        {
            string query = string.Format("SELECT Mah_BaseType FROM M_TimeBaseAllowanceHdr WHERE Mah_TimeBaseID = '{0}'", LedgerAlwCol);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                return Convert.ToChar(dtResult.Rows[0][0]);
            else
                throw new PayrollException("Invalid Allowance Column.");
        }

        public DataTable GetIncomeCodes(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                                SELECT DISTINCT Tin_IncomeCode AS 'Income Code'
                                    , Min_IncomeName As 'Name Name'
                            FROM T_EmpIncome
                            INNER JOIN M_Employee
                                ON Mem_IDNo = Tin_IDNo
                            INNER JOIN {1}..M_Income 
                                ON Min_IncomeCode = Tin_IncomeCode
                                AND Min_CompanyCode = '{0}'
                                AND Min_RecordStatus = 'A'", CompanyCode, CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
    }
}
