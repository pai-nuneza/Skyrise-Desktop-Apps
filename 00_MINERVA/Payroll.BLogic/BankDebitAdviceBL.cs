using System;
using System.Configuration;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class BankDebitAdviceBL: BaseBL
    {
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

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        //-- FOR REPORTS
        public DataTable FetchDataDetail(string BankCode, string PayPeriod, string PayPeriodCycle, string flag, DataTable dtProfile, DevExpress.XtraEditors.CheckedListBoxControl chkListProfiles, string Condition, bool bFinalPay, string companyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"
                        SELECT  REPLACE(Tpy_BankAcctNo, '-', '') AS [Bank Account No]
                            , Tpy_NetAmt AS [Net Pay Amount]
                            , Tpy_IDNo AS [Employee ID]
                            , CASE WHEN Mem_WorkStatus NOT IN ('IN','IM') 
			                    THEN Mem_LastName + ', ' + Mem_FirstName + ' ' + Left(Mem_MiddleName, 1) 
			                    ELSE ''
			                    END AS [Full Name]
                            , Mem_LastName [Last Name]
                            , Mem_FirstName AS [First Name]
                            , LEFT(Mem_MiddleName, 1) AS MI
                            , '' AS [Horizontal Hash]
		                    , @PeriodCycle AS Payperiod
		                    , @flag AS flag 
                        FROM @PROFILE..{3}
                        INNER JOIN @PROFILE..M_Employee ON Tpy_IDNo = Mem_IDNo
                        INNER JOIN {0}..M_Bank ON Mbn_BankCode = Tpy_BankCode
                              AND Mbn_CompanyCode = '{1}'
                        WHERE Len(Rtrim(Tpy_BankAcctNo)) > 0
	                        {2}
                        "
                                    , CentralProfile
                                    , companyCode
                                    , Condition
                                    , (bFinalPay ? "T_EmpPayrollFinalPay" : "T_EmpPayroll"));
            #endregion

            //for multi-profile
            string finalQuery = "";
            for (int i = 0; i < chkListProfiles.CheckedItems.Count; i++)
            {
                DataRow[] drProfile = null;
                string strProfiles = string.Empty;
                strProfiles = chkListProfiles.CheckedItems[i].ToString();
                drProfile = dtProfile.Select("Mpf_ProfileName = '" + strProfiles + "'");
                if (i > 0)
                    finalQuery += @"
                                        UNION
                                        ";
                finalQuery += query.Replace("@PROFILE", drProfile[0]["Mpf_DatabaseName"].ToString());
            }

            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@PeriodCycle", PayPeriod);
            param[1] = new ParameterInfo("@flag", flag);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                try
                {
                    dtResult = dal.ExecuteDataSet(finalQuery, CommandType.Text, param).Tables[0];
                }
                catch (Exception e)
                {
                    // throw new PayrollException("Error in Bank Account code" + TBL1 + "\n" + "\n" + e.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (dtResult.Rows.Count == 0)
                throw new PayrollException("No records retrieved in the report.");
            else
            {
                //add new column
                DataColumn col = new DataColumn();
                col.Caption = "Remarks";
                col.DataType = typeof(string);
                col.ColumnName = "Remarks";

                dtResult.Columns.Add(col);
                int ctr;
                for (ctr = 0; ctr < dtResult.Rows.Count; ctr++)
                {
                    string remarks = CheckAccountNo(dtResult.Rows[ctr]["Bank Account No"].ToString(), BankCode, companyCode, CentralProfile);
                    dtResult.Rows[ctr]["Remarks"] = remarks;

                    if(remarks == "OK")
                        dtResult.Rows[ctr]["Horizontal Hash"] = this.HorizontalHash(dtResult.Rows[ctr]["Bank Account No"].ToString(), dtResult.Rows[ctr]["Net Pay Amount"].ToString());
                }

                CheckDuplicates(dtResult);
            }

            return dtResult;
        }

        private void CheckDuplicates(DataTable dtResult)
        {
            foreach (DataRow row in dtResult.Rows)
            {
                if (row["Bank Account No"].ToString() != "APPLIED")
                {
                    DataRow[] result = dtResult.Select(string.Format("[Bank Account No] = '{0}'", row["Bank Account No"].ToString()));

                    if (result.Length > 1)
                    {
                        foreach (DataRow r in result)
                        {
                            r["Remarks"] = "NOT VALID: Duplicate";
                        }
                    }
                }
            }
        }

        private string CheckAccountNo(string BankAccountNo, string BankCode, string CompanyCode, string CentralProfile)
        {
            string remarks = "";
            string codeLen = (new CommonBL()).GetParameterDtlValueFromCentral("SECNUMLEN", BankCode, CompanyCode, CentralProfile); 
            bool isLegalChar = (BankAccountNo != "APPLIED") ? HasIllegalChars(BankAccountNo) : false ;

            if (BankAccountNo == "APPLIED")
                remarks = "Account number: APPLIED;";
            if (BankAccountNo != "APPLIED")
            {
                if (Convert.ToInt32(codeLen) != BankAccountNo.Length)
                    remarks += "Account number length;";
            }
            if (isLegalChar)
                remarks += "Illegal characters;";

            return (remarks != "") ? "NOT VALID: " + remarks : "OK";
        }

        private bool HasIllegalChars(string BankAccountNo)
        {
            bool HasIllegalChars = false;

            foreach (char c in BankAccountNo)
            {
                if (!Char.IsDigit(c))
                {
                    HasIllegalChars = true;
                    break;
                }
            }

            return HasIllegalChars;
        }

        public DataSet GetBankCompanyDetails(string salutation, string payperiodcycle, string flag, string batchnumber, string payrolldate)
        {
            DataSet ds;

            string sql = @"select 
                         Mcm_ATMBankCode as [Company Code]
                        ,Mcm_BankAccountNo as [Company Acct No]
                        ,@Payperiod as Payperiod
                        ,@flag as flag
                        ,@BatchNum as [Batch Number]
                        ,@PayrollDate as [Payroll Date]
                        from M_Company";

            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@salutation", salutation);
            param[1] = new ParameterInfo("@Payperiod", payperiodcycle);
            param[2] = new ParameterInfo("@flag", flag);
            param[3] = new ParameterInfo("@PayrollDate", payrolldate);
            param[4] = new ParameterInfo("@BatchNum", batchnumber);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            return ds;
        }

        private string HorizontalHash(string Acctno, string Netpay)
        {
            string Hash = string.Empty;
            Netpay = Netpay.Replace(".", "").ToString();
            string AcctnoZeroPadded = string.Format("{0:0000000000}", Acctno);
            string temp = Convert.ToString(Convert.ToInt64(AcctnoZeroPadded.Substring(4, 2)));
            temp = Convert.ToString(Convert.ToInt64(Netpay.ToString()));
            string temps = AcctnoZeroPadded.Substring(4, 2);
            temps = AcctnoZeroPadded.Substring(8, 2);
            Hash = Convert.ToString((Convert.ToInt64(AcctnoZeroPadded.Substring(4, 2)) * Convert.ToInt64(Netpay)) +
                   (Convert.ToInt64(AcctnoZeroPadded.Substring(6, 2)) * Convert.ToInt64(Netpay)) +
                   (Convert.ToInt64(AcctnoZeroPadded.Substring(8, 2)) * Convert.ToInt64(Netpay))).Trim();

            int hashlength = Hash.Length;

            Hash = Hash.Insert(hashlength - 2, ".");

            return Hash;
        }

        public DataSet GetBankDetails(string BankCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds;
            string sql = @"SELECT Mbn_CompanyAccountNo
                                        , Mbn_BranchCodeIssuedByBank
                                        , Mbn_CompanyCodeIssuedByBank
                                        , Mbn_BankGenerationRule 
                                        , REMTENTITY.Mcd_name + ' - ' + BANKLAYOUT.Mcd_name as [Generation Rule Description]
                                        , Mbn_PayrollBranchCode
                                        , Mcm_CompanyName
                                        , Mbn_PayrollSignatory
                                        , Mbn_PayrollSignatoryPos
                                        , Mbn_BankFileLayout
                                        FROM M_Bank 
                                        INNER JOIN M_Company
                                             ON Mcm_CompanyCode = Mbn_CompanyCode   
                                        LEFT JOIN M_CodeDtl REMTENTITY 
                                            ON REMTENTITY.Mcd_code = Mbn_BankGenerationRule
											AND REMTENTITY.Mcd_codetype = 'REMTENTITY'
                                            AND REMTENTITY.Mcd_CompanyCode = Mbn_CompanyCode
										LEFT JOIN M_CodeDtl BANKLAYOUT 
                                            ON BANKLAYOUT.Mcd_code = Mbn_BankFileLayout
											AND BANKLAYOUT.Mcd_codetype = 'BANKLAYOUT'
                                            AND REMTENTITY.Mcd_CompanyCode = Mbn_CompanyCode
                                        WHERE Mbn_BankCode = @BankCode
                                            AND Mbn_CompanyCode = @CompanyCode";

            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@BankCode", BankCode);
            param[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            return ds;
        }

        //end
    }
}
