using System;
using System.Collections.Generic;
using System.Text;
using Payroll.BLogic;
using System.Data;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class EmployeeCedulaBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"INSERT INTO T_EmpCommunityTax
                                          (Tct_TaxYear
                                          ,Tct_IDNo
                                          ,Tct_CommunityTaxNo
                                          ,Tct_IssuedAt
                                          ,Tct_IssuedDate
                                          ,Tct_IssuedBy
                                          ,Tct_PaidAmt
                                          ,Usr_Login
                                          ,Ludatetime)
                                     VALUES
                                          (@Tct_TaxYear
                                          ,@Tct_IDNo
                                          ,@Tct_CommunityTaxNo
                                          ,@Tct_IssuedAt
                                          ,@Tct_IssuedDate
                                          ,@Tct_IssuedBy
                                          ,@Tct_PaidAmt
                                          ,@Usr_Login
                                          ,GETDATE())";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_TaxYear", row["Tct_TaxYear"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IDNo", row["Tct_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_CommunityTaxNo", row["Tct_CommunityTaxNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedAt", row["Tct_IssuedAt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedDate", row["Tct_IssuedDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedBy", row["Tct_IssuedBy"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_PaidAmt", row["Tct_PaidAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return retVal;
        }

        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"UPDATE T_EmpCommunityTax
                                       SET Tct_IDNo = @Tct_IDNo
                                          ,Tct_IssuedAt = @Tct_IssuedAt
                                          ,Tct_IssuedDate = @Tct_IssuedDate
                                          ,Tct_IssuedBy = @Tct_IssuedBy
                                          ,Tct_PaidAmt = @Tct_PaidAmt
                                          ,Usr_Login = @Usr_Login
                                          ,Ludatetime = GETDATE()
                                     WHERE Tct_TaxYear = @Tct_TaxYear
                                        AND Tct_IDNo = @Tct_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_TaxYear", row["Tct_TaxYear"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IDNo", row["Tct_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_CommunityTaxNo", row["Tct_CommunityTaxNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedAt", row["Tct_IssuedAt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedDate", row["Tct_IssuedDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedBy", row["Tct_IssuedBy"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_PaidAmt", row["Tct_PaidAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return retVal;
        }

        public override int Delete(string TaxYear, string EmployeeId)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"DELETE FROM T_EmpCommunityTax
                               WHERE Tct_TaxYear = @Tct_TaxYear
                                AND Tct_IDNo = @Tct_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_TaxYear", TaxYear);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IDNo", EmployeeId);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return retVal;
        }

        public override System.Data.DataSet FetchAll()
        {
            DataSet ds;
            string query = @"
                            SELECT *
                            FROM T_EmpCommunityTax";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            return ds;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int AddEmployeeCedula(DataRow row, DALHelper dal)
        {
            int retVal = 1;
            int paramIndex = 0;

            #region query

            string qString = @"IF NOT EXISTS (SELECT Tct_IDNo FROM T_EmpCommunityTax WHERE Tct_TaxYear = @Tct_TaxYear AND Tct_IDNo = @Tct_IDNo)
                               INSERT INTO T_EmpCommunityTax
                                          (Tct_TaxYear
                                          ,Tct_IDNo
                                          ,Tct_CommunityTaxNo
                                          ,Tct_IssuedAt
                                          ,Tct_IssuedDate
                                          ,Tct_IssuedBy
                                          ,Tct_PaidAmt
                                          ,Usr_Login
                                          ,Ludatetime)
                                     VALUES
                                          (@Tct_TaxYear
                                          ,@Tct_IDNo
                                          ,@Tct_CommunityTaxNo
                                          ,@Tct_IssuedAt
                                          ,@Tct_IssuedDate
                                          ,@Tct_IssuedBy
                                          ,@Tct_PaidAmt
                                          ,@Usr_Login
                                          ,GETDATE())
                                ELSE
                                    UPDATE T_EmpCommunityTax 
                                        SET Tct_CommunityTaxNo = @Tct_CommunityTaxNo
                                        , Tct_IssuedAt = @Tct_IssuedAt
                                        , Tct_IssuedDate = @Tct_IssuedDate
                                        , Tct_IssuedBy = @Tct_IssuedBy
                                        , Tct_PaidAmt = @Tct_PaidAmt
                                        , Usr_Login = @Usr_Login
                                        , Ludatetime = GETDATE()
                                    WHERE Tct_TaxYear = @Tct_TaxYear
                                        AND Tct_IDNo = @Tct_IDNo
                                    ";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_TaxYear", row["Tct_TaxYear"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IDNo", row["Tct_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_CommunityTaxNo", row["Tct_CommunityTaxNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedAt", row["Tct_IssuedAt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedDate", row["Tct_IssuedDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedBy", row["Tct_IssuedBy"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_PaidAmt", row["Tct_PaidAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            try
            {
                retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public int UpdateEmployeeCedula(System.Data.DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"UPDATE T_EmpCommunityTax
                                       SET Tct_CommunityTaxNo = @Tct_CommunityTaxNo
                                          ,Tct_IssuedAt = @Tct_IssuedAt
                                          ,Tct_IssuedDate = @Tct_IssuedDate
                                          ,Tct_IssuedBy = @Tct_IssuedBy
                                          ,Tct_PaidAmt = @Tct_PaidAmt
                                          ,Usr_Login = @Usr_Login
                                          ,Ludatetime = GETDATE()
                                     WHERE Tct_TaxYear = @Tct_TaxYear
                                        AND Tct_IDNo = @Tct_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_TaxYear", row["Tct_TaxYear"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IDNo", row["Tct_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_CommunityTaxNo", row["Tct_CommunityTaxNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedAt", row["Tct_IssuedAt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedDate", row["Tct_IssuedDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_IssuedBy", row["Tct_IssuedBy"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tct_PaidAmt", row["Tct_PaidAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            
            retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            
            return retVal;
        }

        public DataTable GetEmployeeMasterList()
        {
            #region query
            string query = string.Format(@"SELECT Mem_IDNo
                                           FROM M_Employee
                                           WHERE LEFT(Mem_WorkStatus, 1) = 'A'");
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
    }
}
