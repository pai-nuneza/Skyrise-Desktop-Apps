using System;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class frmCompanyMasterBL : BaseBL
    {
        public int Add(System.Data.DataRow row, int ImageSize)
        {
            int retVal = 0;

            #region query

            string qString = @"INSERT INTO T_CompanyMaster
                                                (Ccd_CompanyCode,
                                                Ccd_CompanyName,
                                                Ccd_CompanyAddress1,
                                                Ccd_CompanyAddress2,
                                                Ccd_CompanyAddress3,
                                                Ccd_EmailAddress,
                                                Ccd_TelephoneNo,
                                                Ccd_CellNo,
                                                Ccd_FaxNo,
                                                Ccd_CompanyLogo,
                                                Ccd_SSS,
                                                Ccd_TIN,
                                                Ccd_CurrentYear,
                                                Ccd_LastEmployeeID,
                                                Ccd_status,
                                                Ccd_SystemVer,
                                                Ccd_SystemVerSwipe,
                                                --Ccd_PictureLink,
                                                --Ccd_PictureFileExt,
                                                Ccd_TaxSchedule,
                                                User_login,
                                                ludatetime,
                                                Ccd_LeaveCredit,
                                                Ccd_LeaveRefund,
                                                Ccd_Bonus,
                                                Ccd_HDMFNo,
                                                Ccd_PhilhealthNo,
                                                Ccd_SECRegistration,
                                                Ccd_PEZARegistration) 
                                               VALUES
                                                (@CompanyCode,
                                                @CompanyName,
                                                @CompanyAddress1,
                                                @CompanyAddress2,
                                                @CompanyAddress3,
                                                @EmailAddress,
                                                @TelephoneNo,
                                                @CellNo,
                                                @FaxNo,
                                                @CompanyLogo,
                                                @SSS,
                                                @TIN,
                                                @status,
                                                @Ccd_SystemVer,
                                                @Ccd_SystemVerSwipe,
                                                --@Ccd_PictureLink,
                                                --@Ccd_PictureFileExt,
                                                @Ccd_TaxSchedule,
                                                @User_login,
                                                GetDate(),
                                                @Ccd_LeaveCredit,
                                                @Ccd_LeaveRefund,
                                                @Ccd_Bonus,
                                                @Ccd_HDMFNo,
                                                @Ccd_PhilhealthNo,
                                                @Ccd_SECRegistration,
                                                @Ccd_PEZARegistration)";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[26];
            paramInfo[0] = new ParameterInfo("@CompanyCode", row["Ccd_CompanyCode"]);
            paramInfo[1] = new ParameterInfo("@CompanyName", row["Ccd_CompanyName"]);
            paramInfo[2] = new ParameterInfo("@CompanyAddress1", row["Ccd_CompanyAddress1"]);
            paramInfo[3] = new ParameterInfo("@CompanyAddress2", row["Ccd_CompanyAddress2"]);
            paramInfo[4] = new ParameterInfo("@CompanyAddress3", row["Ccd_CompanyAddress3"]);
            paramInfo[5] = new ParameterInfo("@EmailAddress", row["Ccd_EmailAddress"]);
            paramInfo[6] = new ParameterInfo("@TelephoneNo", row["Ccd_TelephoneNo"]);
            paramInfo[7] = new ParameterInfo("@CellNo", row["Ccd_CellNo"]);
            paramInfo[8] = new ParameterInfo("@FaxNo", row["Ccd_FaxNo"]);
            paramInfo[9] = new ParameterInfo("@CompanyLogo", row["Ccd_CompanyLogo"]);
            paramInfo[10] = new ParameterInfo("@SSS", row["Ccd_SSS"]);
            paramInfo[11] = new ParameterInfo("@TIN", row["Ccd_TIN"]);
            paramInfo[12] = new ParameterInfo("@Ccd_CurrentYear", row["Ccd_CurrentYear"]);
            paramInfo[13] = new ParameterInfo("@Ccd_LastEmployeeID", row["Ccd_LastEmployeeID"]);
            paramInfo[14] = new ParameterInfo("@status", row["Ccd_status"]);
            paramInfo[15] = new ParameterInfo("@Ccd_SystemVer", row["Ccd_SystemVer"]);
            paramInfo[16] = new ParameterInfo("@Ccd_SystemVerSwipe", row["Ccd_SystemVerSwipe"]);
            //paramInfo[17] = new ParameterInfo("@Ccd_PictureLink", row["Ccd_PictureLink"]);
            //paramInfo[18] = new ParameterInfo("@Ccd_PictureFileExt", row["Ccd_PictureFileExt"]);
            paramInfo[17] = new ParameterInfo("@Ccd_TaxSchedule", row["Ccd_TaxSchedule"]);
            paramInfo[18] = new ParameterInfo("@user_login", row["User_login"]);
            paramInfo[19] = new ParameterInfo("@Ccd_LeaveCredit", row["Ccd_LeaveCredit"]);
            paramInfo[20] = new ParameterInfo("@Ccd_LeaveRefund", row["Ccd_LeaveRefund"]);
            paramInfo[21] = new ParameterInfo("@Ccd_Bonus", row["Ccd_Bonus"]);
            //added by kevin 01282009 - hdmf and philhealth
            paramInfo[22] = new ParameterInfo("@Ccd_HDMFNo", row["Ccd_HDMFNo"]);
            paramInfo[23] = new ParameterInfo("@Ccd_PhilhealthNo", row["Ccd_PhilhealthNo"]);
            //end
            paramInfo[24] = new ParameterInfo("@Ccd_SECRegistration", row["Ccd_SECRegistration"]);
            paramInfo[25] = new ParameterInfo("@Ccd_PEZARegistration", row["Ccd_PEZARegistration"]);

            ParameterInfo[] paramInfo1 = new ParameterInfo[1];
            paramInfo1[0] = new ParameterInfo("@CompanyLogo", row["Ccd_CompanyLogo"], SqlDbType.Image,ImageSize);
            string strSQL = @"UPDATE T_CompanyMaster SET Ccd_CompanyLogo=@CompanyLogo";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    dal.ExecuteNonQuery(string.Format(strSQL, row["Ccd_CompanyLogo"]), CommandType.Text, paramInfo1);
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

        public int Update(System.Data.DataRow row, string code, int ImageSize)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE T_CompanyMaster 
                                               SET 
                                                Ccd_CompanyCode = @CompanyCode,
                                                Ccd_CompanyName = @CompanyName,
                                                Ccd_CompanyAddress1 = @CompanyAddress1,
                                                Ccd_CompanyAddress2 = @CompanyAddress2,
                                                Ccd_CompanyAddress3 = @CompanyAddress3,
                                                Ccd_EmailAddress = @EmailAddress,
                                                Ccd_TelephoneNo = @TelephoneNo,
                                                Ccd_CellNo = @CellNo,
                                                Ccd_FaxNo = @FaxNo,
                                                Ccd_SSS = @SSS,
                                                Ccd_TIN = @TIN,
                                                Ccd_CurrentYear = @Ccd_CurrentYear,
                                                Ccd_LastEmployeeID = @Ccd_LastEmployeeID,
                                                Ccd_status=@status,
                                                Ccd_SystemVer = @Ccd_SystemVer,
                                                Ccd_SystemVerSwipe = @Ccd_SystemVerSwipe,
                                                --Ccd_PictureLink = @Ccd_PictureLink,
                                                --Ccd_PictureFileExt = @Ccd_PictureFileExt,
                                                Ccd_TaxSchedule = @Ccd_TaxSchedule,
                                                User_login = @User_login,
                                                ludatetime = GetDate(),
                                                Ccd_BankCode = @Ccd_BankCode,
                                                Ccd_BankAddress1 = @Ccd_BankAddress1,
                                                Ccd_BankAddress2 = @Ccd_BankAddress2,
                                                Ccd_BankAccountNo = @Ccd_BankAccountNo,
                                                Ccd_LastMCLNo = @Ccd_LastMCLNo,
                                                Ccd_BranchCode = @Ccd_BranchCode,
                                                Ccd_BankAddress3 = @Ccd_BankAddress3,
                                                Ccd_BankInCharge = @Ccd_BankInCharge,
                                                Ccd_BICPosition = @Ccd_BICPosition,
                                              Ccd_LeaveCredit = @Ccd_LeaveCredit,
                                              Ccd_LeaveRefund = @Ccd_LeaveRefund,
                                              Ccd_Bonus = @Ccd_Bonus,
                                                Ccd_HDMFNo = @Ccd_HDMFNo,
                                                Ccd_PhilhealthNo = @Ccd_PhilhealthNo,
                                                Ccd_DefaultShift = @Ccd_DefaultShift,
                                                Ccd_DefaultRestday = @Ccd_DefaultRestday,
                                                Ccd_DefaultSSSCode = @Ccd_DefaultSSSCode,
                                                Ccd_DefaultPhilhealthCode = @Ccd_DefaultPhilhealthCode,
                                                Ccd_DefaultHDMFCode = @Ccd_DefaultHDMFCode,
                                                Ccd_WeekCoverage = @Ccd_WeekCoverage,
                                                Ccd_ATMBankCode = @Ccd_ATMBankCode,
                                                Ccd_SECRegistration = @Ccd_SECRegistration,
                                                Ccd_PEZARegistration = @Ccd_PEZARegistration
                                                WHERE Ccd_CompanyCode = @LCompCode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[42];
            paramInfo[0] = new ParameterInfo("@CompanyCode", row["Ccd_CompanyCode"]);
            paramInfo[1] = new ParameterInfo("@CompanyName", row["Ccd_CompanyName"]);
            paramInfo[2] = new ParameterInfo("@CompanyAddress1", row["Ccd_CompanyAddress1"]);
            paramInfo[3] = new ParameterInfo("@CompanyAddress2", row["Ccd_CompanyAddress2"]);
            paramInfo[4] = new ParameterInfo("@CompanyAddress3", row["Ccd_CompanyAddress3"]);
            paramInfo[5] = new ParameterInfo("@EmailAddress", row["Ccd_EmailAddress"]);
            paramInfo[6] = new ParameterInfo("@TelephoneNo", row["Ccd_TelephoneNo"]);
            paramInfo[7] = new ParameterInfo("@CellNo", row["Ccd_CellNo"]);
            paramInfo[8] = new ParameterInfo("@FaxNo", row["Ccd_FaxNo"]);
            paramInfo[9] = new ParameterInfo("@SSS", row["Ccd_SSS"]);
            paramInfo[10] = new ParameterInfo("@TIN", row["Ccd_TIN"]);
            paramInfo[11] = new ParameterInfo("@Ccd_CurrentYear", row["Ccd_CurrentYear"]);
            paramInfo[12] = new ParameterInfo("@Ccd_LastEmployeeID", row["Ccd_LastEmployeeID"]);
            paramInfo[13] = new ParameterInfo("@status", row["Ccd_status"]);
            paramInfo[14] = new ParameterInfo("@Ccd_SystemVer", row["Ccd_SystemVer"]);
            paramInfo[15] = new ParameterInfo("@Ccd_SystemVerSwipe", row["Ccd_SystemVerSwipe"]);
            //paramInfo[16] = new ParameterInfo("@Ccd_PictureLink", row["Ccd_PictureLink"]);
            //paramInfo[17] = new ParameterInfo("@Ccd_PictureFileExt", row["Ccd_PictureFileExt"]);
            paramInfo[16] = new ParameterInfo("@Ccd_TaxSchedule", row["Ccd_TaxSchedule"]);
            paramInfo[17] = new ParameterInfo("@user_login", row["User_login"]);
            paramInfo[18] = new ParameterInfo("@LCompCode", code);

            paramInfo[19] = new ParameterInfo("@Ccd_BankCode", row["Ccd_BankCode"]);
            paramInfo[20] = new ParameterInfo("@Ccd_BankAddress1", row["Ccd_BankAddress1"]);
            paramInfo[21] = new ParameterInfo("@Ccd_BankAddress2", row["Ccd_BankAddress2"]);
            paramInfo[22] = new ParameterInfo("@Ccd_BankAccountNo", row["Ccd_BankAccountNo"]);
            paramInfo[23] = new ParameterInfo("@Ccd_LastMCLNo", row["Ccd_LastMCLNo"]);
            paramInfo[24] = new ParameterInfo("@Ccd_BranchCode", row["Ccd_BranchCode"]);
            paramInfo[25] = new ParameterInfo("@Ccd_BankAddress3", row["Ccd_BankAddress3"]);
            paramInfo[26] = new ParameterInfo("@Ccd_BankInCharge", row["Ccd_BankInCharge"]);
            paramInfo[27] = new ParameterInfo("@Ccd_BICPosition", row["Ccd_BICPosition"]);

            paramInfo[28] = new ParameterInfo("@Ccd_LeaveCredit", row["Ccd_LeaveCredit"]);
            paramInfo[29] = new ParameterInfo("@Ccd_LeaveRefund", row["Ccd_LeaveRefund"]);
            paramInfo[30] = new ParameterInfo("@Ccd_Bonus", row["Ccd_Bonus"]);
            paramInfo[31] = new ParameterInfo("@Ccd_HDMFNo", row["Ccd_HDMFNo"]);
            paramInfo[32] = new ParameterInfo("@Ccd_PhilhealthNo", row["Ccd_PhilhealthNo"]);
            //added by capetillo 11142009
            paramInfo[33] = new ParameterInfo("@Ccd_DefaultShift", row["Ccd_DefaultShift"]);
            paramInfo[34] = new ParameterInfo("@Ccd_DefaultRestday", row["Ccd_DefaultRestday"]);
            paramInfo[35] = new ParameterInfo("@Ccd_DefaultSSSCode", row["Ccd_DefaultSSSCode"]);
            paramInfo[36] = new ParameterInfo("@Ccd_DefaultPhilhealthCode", row["Ccd_DefaultPhilhealthCode"]);
            paramInfo[37] = new ParameterInfo("@Ccd_DefaultHDMFCode", row["Ccd_DefaultHDMFCode"]);
            paramInfo[38] = new ParameterInfo("@Ccd_WeekCoverage", row["Ccd_WeekCoverage"]);
            paramInfo[39] = new ParameterInfo("@Ccd_ATMBankCode", row["Ccd_ATMBankCode"]);
            //end
            //added by adam 10142010
            paramInfo[40] = new ParameterInfo("@Ccd_SECRegistration", row["Ccd_SECRegistration"]);
            paramInfo[41] = new ParameterInfo("@Ccd_PEZARegistration", row["Ccd_PEZARegistration"]);
            //end
            ParameterInfo[] paramInfo1 = new ParameterInfo[1];
            paramInfo1[0] = new ParameterInfo("@CompanyLogo", row["Ccd_CompanyLogo"], SqlDbType.Image, ImageSize);
            string strSQL = @"UPDATE T_CompanyMaster SET Ccd_CompanyLogo=@CompanyLogo";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    dal.ExecuteNonQuery(string.Format(strSQL, row["Ccd_CompanyLogo"]), CommandType.Text, paramInfo1);
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
       
        public override int Delete(string deductcode, string Userlogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@deductcode", deductcode, SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@Userlogin", Userlogin, SqlDbType.Char, 15);

            string sqlQuery = @"UPDATE T_DeductionCodeMaster 
                                               SET 
                                                  dcm_Status='C',
                                                  Usr_Login = @Userlogin, 
                                                  ludatetime = GetDate()
                                               WHERE dcm_DeductionCode=@deductcode        
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

            #region query

            string qString = @"SELECT Ccd_CompanyCode,
                                                Ccd_CompanyName,
                                                Ccd_CompanyAddress1,
                                                Ccd_CompanyAddress2,
                                                Ccd_CompanyAddress3,
                                                Ccd_EmailAddress,
                                                Ccd_TelephoneNo,
                                                Ccd_CellNo,
                                                Ccd_FaxNo,
                                                Ccd_CompanyLogo,
                                                Ccd_SSS,
                                                Ccd_TIN,
                                                Ccd_CurrentYear,
                                                Ccd_LastEmployeeID,
                                                Ccd_status,
                                                Ccd_SystemVer,
                                                Ccd_SystemVerSwipe,
                                                --Ccd_PictureLink,
                                                --Ccd_PictureFileExt,
                                                Ccd_TaxSchedule,
                                                Ccd_BankCode,
                                                Ccd_BankAddress1,
                                                Ccd_BankAddress2,
                                                Ccd_BankAccountNo,
                                                Ccd_LastMCLNo,
                                                Ccd_BranchCode
                                               FROM T_CompanyMaster";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public DataRow Fetch()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Ccd_CompanyCode,
                                      Ccd_CompanyName,
                                      Ccd_CompanyAddress1,
                                      Ccd_CompanyAddress2,
                                      Ccd_CompanyAddress3,
                                      Ccd_EmailAddress,
                                      Ccd_TelephoneNo,
                                      Ccd_CellNo,
                                      Ccd_FaxNo,
                                      Ccd_CompanyLogo,
                                      Ccd_SSS,
                                      Ccd_TIN,
                                      Ccd_CurrentYear,
                                      Ccd_LastEmployeeID,
                                      Ccd_status,
                                      Ccd_SystemVer,
                                      Ccd_SystemVerSwipe,
                                      Ccd_TaxSchedule,
                                      Ccd_BankCode,
                                      Ccd_BankAddress1,
                                      Ccd_BankAddress2,
                                      Ccd_BankAccountNo,
                                      Ccd_LastMCLNo,
                                      Ccd_BranchCode,
                                      Ccd_BankAddress3,
                                      Ccd_BankInCharge,
                                      Ccd_BICPosition,
                                      Ccd_LeaveCredit,
                                      Ccd_LeaveRefund,
                                      Ccd_Bonus,
                                      Ccd_HDMFNo,
                                      Ccd_PhilhealthNo,
                                      Ccd_DefaultShift,
                                      Ccd_ATMBankCode,
                                      Ccd_DefaultShift,
                                      Ccd_DefaultSSSCode,
                                      Ccd_DefaultHDMFCode,
                                      Ccd_DefaultPhilhealthCode,
                                      Ccd_DefaultRestday,
                                      Ccd_WeekCoverage,
                                      Ccd_SECRegistration,
                                      Ccd_PEZARegistration
                                 FROM T_CompanyMaster";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public string FetchCompanyName()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("Select RTRIM(Ccd_CompanyName) From T_CompanyMaster", CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
            else
                return string.Empty;
        }

        #region Unused Function

        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;
            return retVal;
        }

        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;
            return retVal;

        }

        public override System.Data.DataRow Fetch(string code)
        {
            DataSet ds = new DataSet();
            return ds.Tables[0].Rows[0];

        }

        #endregion

        #region Report

        public DataSet GetReportData()
        {
            DataSet ds = new DataSet();

            #region query

            string query = @"SELECT Ccd_CompanyCode
                                   ,Ccd_CompanyAddress1 + ' ' + Ccd_CompanyAddress2 + ', ' + Adt_AccountDesc as Address                                   
                                   ,Ccd_CompanyName
                                   ,Ccd_CompanyAddress1
                                   ,Ccd_CompanyAddress2
                                   ,Ccd_CompanyAddress3
                                   ,Ccd_EmailAddress
                                   ,Ccd_TelephoneNo
                                   ,Ccd_CellNo
                                   ,Ccd_FaxNo
                                   ,Ccd_CompanyLogo
                                   ,Ccd_SSS
                                   ,Ccd_TIN
                                   ,Ccd_CurrentYear
                                   ,Ccd_LastEmployeeID
                                   ,Ccd_status
                                   ,Ccd_SystemVer
                                   ,Ccd_SystemVerSwipe
                                   ,Ccd_TaxSchedule
                                   ,Ccd_BankCode
                                   ,Ccd_BankAddress1
                                   ,Ccd_BankAddress2
                                   ,Ccd_BankAccountNo
                                   ,Ccd_LastMCLNo
                                   ,Ccd_BranchCode
                                   ,Ccd_ATMBankCode
                                   ,Ccd_DefaultShift
                                   ,Ccd_DefaultRestday
                                   ,Ccd_DefaultSSSCode
                                   ,Ccd_DefaultPhilhealthCode
                                   ,Ccd_DefaultHDMFCode
                                   ,Ccd_WeekCoverage
                                   ,Ccd_BankInCharge
                                   ,Ccd_BICPosition
                                   ,Ccd_BankAddress3
                                   ,Ccd_LeaveCredit
                                   ,Ccd_LeaveRefund
                                   ,Ccd_Bonus
                                   ,Ccd_PhilhealthNo
                                   ,Ccd_HDMFNo
                                   ,Ccd_SECRegistration
                                   ,Ccd_PEZARegistration
                               FROM T_CompanyMaster
                         Inner Join T_AccountDetail on Ccd_CompanyAddress3 = Adt_AccountCode and Adt_AccountType='ZIPCODE'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }

            return ds;
        }

        #endregion

    }
}
