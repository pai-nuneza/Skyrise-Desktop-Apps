using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class AmortizationBL: BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                //dal.BeginTransactionSnapshot();
                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[6];

                    paramInfo[0] = new ParameterInfo("@Mla_DeductionCode", row["Mla_DeductionCode"], SqlDbType.Char);
                    paramInfo[1] = new ParameterInfo("@Mla_DeductionAmount", row["Mla_DeductionAmount"]);
                    paramInfo[2] = new ParameterInfo("@Mla_PrincipalAmount", row["Mla_PrincipalAmount"]);
                    paramInfo[3] = new ParameterInfo("@Mla_Amortization", row["Mla_Amortization"]);
                    paramInfo[4] = new ParameterInfo("@Mla_RecordStatus", row["Mla_RecordStatus"]);
                    paramInfo[5] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

                    string qAdd = @"INSERT INTO M_LoanAmortization	(Mla_DeductionCode
								                                    ,Mla_DeductionAmount
                                                                    ,Mla_PrincipalAmount
								                                    ,Mla_Amortization
								                                    ,Mla_RecordStatus
								                                    ,Usr_Login
								                                    ,Ludatetime)
							                                    VALUES(@Mla_DeductionCode
								                                    ,@Mla_DeductionAmount
                                                                    ,@Mla_PrincipalAmount
								                                    ,@Mla_Amortization
								                                    ,@Mla_RecordStatus
								                                    ,@Usr_Login
								                                    ,Getdate())";

                    retVal = dal.ExecuteNonQuery(qAdd, CommandType.Text, paramInfo);
                    //retVal = dal.ExecuteNonQuery(CommonConstants.StoredProcedures.spUserMasterAdd, CommandType.StoredProcedure, paramInfo);


                    //dal.CommitTransactionSnapshot();
                    dal.BeginTransactionSnapshot();
                }
                catch (Exception e)
                {
                    //dal.RollBackTransactionSnapshot();                    
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
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                //dal.BeginTransactionSnapshot();
                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[6];

                    paramInfo[0] = new ParameterInfo("@Mla_DeductionCode", row["Mla_DeductionCode"], SqlDbType.Char);
                    paramInfo[1] = new ParameterInfo("@Mla_DeductionAmount", row["Mla_DeductionAmount"]);
                    paramInfo[2] = new ParameterInfo("@Mla_PrincipalAmount", row["Mla_PrincipalAmount"]);
                    paramInfo[3] = new ParameterInfo("@Mla_Amortization", row["Mla_Amortization"]);
                    paramInfo[4] = new ParameterInfo("@Mla_RecordStatus", row["Mla_RecordStatus"]);
                    paramInfo[5] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);


                    string qEdit = @"UPDATE M_LoanAmortization		
		                                        SET Mla_DeductionCode = @Mla_DeductionCode
			                                        ,Mla_DeductionAmount = @Mla_DeductionAmount
                                                    ,Mla_PrincipalAmount = @Mla_PrincipalAmount
			                                        ,Mla_Amortization = @Mla_Amortization
			                                        ,Mla_RecordStatus = @Mla_RecordStatus
			                                        ,Usr_Login = @Usr_Login
			                                        ,Ludatetime = GetDate()
	                                        WHERE Mla_DeductionCode = @Mla_DeductionCode
		                                        AND Mla_DeductionAmount = @Mla_DeductionAmount
						                                        ";

                    retVal = dal.ExecuteNonQuery(qEdit, CommandType.Text, paramInfo);

                    //dal.CommitTransactionSnapshot();
                    dal.BeginTransactionSnapshot();
                }
                catch (Exception e)
                {
                    //dal.RollBackTransactionSnapshot();
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

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");           
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");           
        }

        public System.Data.DataSet FetchMe(string code)
        {
            DataSet ds = new DataSet();
            string qString = @"SELECT Mla_DeductionCode
		                            ,Mdn_DeductionName
		                            ,Mla_DeductionAmount
                                    ,Mla_PrincipalAmount
		                            ,Mla_Amortization
		                            ,Mla_RecordStatus
		                            ,M_LoanAmortization.Usr_Login AS [Usr_Login]
		                            ,M_LoanAmortization.Ludatetime AS [Ludatetime]
                                    ,Convert(varchar(50),Mla_DeductionAmount) as 'DeductionAmountSearch'
                                    ,CONVERT(varchar(50) , Mla_Amortization) AS 'AmortizationSearch'
                                    ,CONVERT(varchar(50) , Mla_PrincipalAmount) AS 'PrincipalAmountSearch'
	                            FROM M_LoanAmortization
		                            LEFT JOIN M_Deduction
			                            ON Mdn_DeductionCode = Mla_DeductionCode 
	                            WHERE Mla_DeductionCode = @Mla_DeductionCode";

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@Mla_DeductionCode", code, SqlDbType.Char);

                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramCollection);

                dal.CloseDB();
            }
            return ds;            
        }

        public bool CheckIfRecordExists(string EmployeeIDV, string SofInfracV)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeIDV", EmployeeIDV);
            paramInfo[1] = new ParameterInfo("@SofInfracV", SofInfracV);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    string sqlstring = @"SELECT * FROM T_EmpOffense 
                                Where Tof_IDNo = @EmployeeIDV
                                And Eot_StartInfractionDate = @SofInfracV";

                    ds = dal.ExecuteDataSet(sqlstring, CommandType.Text, paramInfo);

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

                if (ds.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public int DeleteRecord(string deductionCode, string deductionAmount, string UsrLogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Mla_DeductionCode", deductionCode, SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@Mla_DeductionAmount", deductionAmount, SqlDbType.Decimal, 9);
            paramInfo[2] = new ParameterInfo("@Usr_Login", UsrLogin, SqlDbType.Char, 15);

            string qstring = @"UPDATE M_LoanAmortization
	                                    SET Mla_RecordStatus = 'C'
		                                    ,Usr_Login = @Usr_Login
		                                    ,Ludatetime = Getdate()
	                                    WHERE Mla_DeductionCode = @Mla_DeductionCode
		                                    AND Mla_DeductionAmount = @Mla_DeductionAmount";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qstring, CommandType.Text, paramInfo);
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

        public bool CheckIfRecordExistsInAmort(string Mla_DeductionCode, string Mla_DeductionAmount)
        {
            DataSet ds = new DataSet();

            #region query

            string sqlstring = @"Select * From M_LoanAmortization
                                            Where Mla_DeductionCode = @Mla_DeductionCode
                                            And Mla_DeductionAmount = @Mla_DeductionAmount";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mla_DeductionCode", Mla_DeductionCode);
            paramInfo[1] = new ParameterInfo("@Mla_DeductionAmount", Mla_DeductionAmount);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    ds = dal.ExecuteDataSet(sqlstring, CommandType.Text, paramInfo);

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

                if (ds.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public DataSet FetchAllRecord(string Mla_DeductionCode)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Mla_DeductionCode
	                                   ,Mdn_DeductionName
	                                   ,Mla_DeductionAmount
	                                   ,Mla_Amortization
	                                   ,Mla_RecordStatus
	                                   ,M_LoanAmortization.Usr_Login
	                                   ,M_LoanAmortization.Ludatetime
                                From M_LoanAmortization Inner Join M_Deduction
		                                on Mla_DeductionCode = Mdn_DeductionCode
                                Where Mla_DeductionCode = @Mla_DeductionCode";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@Mla_DeductionCode", Mla_DeductionCode);

                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramCollection);

                dal.CloseDB();
            }
            return ds;
        }

        //Reports
        public DataSet GetHeaderData(string DeductionCode)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"Select Mcm_CompanyName
                                   ,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ', ' + Mcd_Name as Address
                                   ,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
                                   ,Mcm_CompanyLogo
                                   ,(SELECT DISTINCT Mdn_DeductionName 
                                            FROM M_Deduction
                                     WHERE Mdn_DeductionCode = @Mla_DeductionCode) As DeductionDesc
                                     From M_Company
                                     Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code and Mcd_CodeType='ZIPCODE'";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Mla_DeductionCode", DeductionCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetDetailData(string DeductionCode)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"DECLARE @ALL VARCHAR(3)
                                 SET @ALL = 'ALL'
                           SELECT Mla_DeductionCode
		                            ,Mla_DeductionAmount
		                            ,Mla_Amortization
		                            , CASE WHEN  Mla_RecordStatus = 'A' THEN 'ACTIVE' 
		                                   WHEN  Mla_RecordStatus = 'C' THEN 'CANCELLED' END as 'Status' 
		                            ,M_LoanAmortization.Usr_Login AS [Usr_Login]
		                            ,M_LoanAmortization.Ludatetime AS [Ludatetime]
		                            ,Mdn_DeductionName
	                            FROM M_LoanAmortization
		                            LEFT JOIN M_Deduction
			                            ON Mdn_DeductionCode = Mla_DeductionCode 
	                            WHERE Mla_DeductionCode = @Mla_DeductionCode";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Mla_DeductionCode", DeductionCode);


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        } 

    }
}
