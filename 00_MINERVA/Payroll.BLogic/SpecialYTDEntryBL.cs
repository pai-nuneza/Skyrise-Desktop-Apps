using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using Payroll.BLogic;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class SpecialYTDEntryBL : BaseBL
    {
        public override int Add(DataRow row)
        {
            int retVal = 0;
            string qString = @"INSERT INTO T_YTDAdjustment 
                                        (Ytd_Period
                                        ,Tyt_IDNo
                                        ,Ytd_TaxIncomeAmt
                                        ,Ytd_EEPremiumAmt
                                        ,Ytd_PremiumPaid
                                        ,Ytd_WtaxAmt
                                        ,Ytd_NonTaxIncomeAmt
                                        ,Ytd_MWEBasicSalary
                                        ,Tyt_MWEHoliday
                                        ,Tyt_MWEOvertime
                                        ,Tyt_MWENightShiftPay
                                        ,Tyt_MWEHazard
                                        ,Ytd_NonTax13thMonth
                                        ,Ytd_DeMinimis
                                        ,Ytd_BasicSalary
                                        ,Ytd_TaxOvertimePay
                                        ,Ytd_TaxHazardPay
                                        ,Ytd_Tax13thMonth
                                        ,Ytd_OtherTaxIncome
                                        ,usr_login
                                        ,ludatetime
                                        ,Ytd_WithPrvER
                                        ,Ytd_PrvER
                                        ,Ytd_CompanyAddressER
                                        ,Tyt_TIN
                                        ,Ytd_CompanyZipCodeER)
									VALUES
                                        (@Ytd_Period
                                        ,@Ytd_EmployeeId
                                        ,@Ytd_TaxIncomeAmt
                                        ,@Ytd_EEPremiumAmt
                                        ,@Ytd_PremiumPaid
                                        ,@Ytd_WtaxAmt
                                        ,@Ytd_NonTaxIncomeAmt
                                        ,@Ytd_MWEBasicSalary
                                        ,@Tyt_MWEHoliday
                                        ,@Tyt_MWEOvertime
                                        ,@Tyt_MWENightShiftPay
                                        ,@Tyt_MWEHazard
                                        ,@Ytd_NonTax13thMonth
                                        ,@Ytd_DeMinimis
                                        ,@Ytd_BasicSalary
                                        ,@Ytd_TaxOvertimePay
                                        ,@Ytd_TaxHazardPay
                                        ,@Ytd_Tax13thMonth
                                        ,@Ytd_OtherTaxIncome
                                        ,@usr_login
                                        ,GetDate()
                                        ,@Ytd_WithPrvER
                                        ,@Ytd_PrvER
                                        ,@Ytd_CompanyAddressER
                                        ,@Tyt_TIN
                                        ,@Ytd_CompanyZipCodeER)";


            ParameterInfo[] paramInfo = new ParameterInfo[25];
            paramInfo[0] = new ParameterInfo("@Ytd_Period", row["Ytd_Period"]);
            paramInfo[1] = new ParameterInfo("@Ytd_EmployeeId", row["Ytd_EmployeeId"]);
            paramInfo[2] = new ParameterInfo("@Ytd_TaxIncomeAmt", row["Ytd_TaxIncomeAmt"]);
            paramInfo[3] = new ParameterInfo("@Ytd_EEPremiumAmt", row["Ytd_EEPremiumAmt"]);
            paramInfo[4] = new ParameterInfo("@Ytd_PremiumPaid", row["Ytd_PremiumPaid"]);
            paramInfo[5] = new ParameterInfo("@Ytd_WtaxAmt", row["Ytd_WtaxAmt"]);
            paramInfo[6] = new ParameterInfo("@Ytd_NonTaxIncomeAmt", row["Ytd_NonTaxIncomeAmt"]);
            paramInfo[7] = new ParameterInfo("@Ytd_MWEBasicSalary", row["Ytd_MWEBasicSalary"]);
            paramInfo[8] = new ParameterInfo("@Tyt_MWEHoliday", row["Tyt_MWEHoliday"]);
            paramInfo[9] = new ParameterInfo("@Tyt_MWEOvertime", row["Tyt_MWEOvertime"]);
            paramInfo[10] = new ParameterInfo("@Tyt_MWENightShiftPay", row["Tyt_MWENightShiftPay"]);
            paramInfo[11] = new ParameterInfo("@Tyt_MWEHazard", row["Tyt_MWEHazard"]);
            paramInfo[12] = new ParameterInfo("@Ytd_NonTax13thMonth", row["Ytd_NonTax13thMonth"]);
            paramInfo[13] = new ParameterInfo("@Ytd_DeMinimis", row["Ytd_DeMinimis"]);
            paramInfo[14] = new ParameterInfo("@Ytd_BasicSalary", row["Ytd_BasicSalary"]);
            paramInfo[15] = new ParameterInfo("@Ytd_TaxOvertimePay", row["Ytd_TaxOvertimePay"]);
            paramInfo[16] = new ParameterInfo("@Ytd_TaxHazardPay", row["Ytd_TaxHazardPay"]);
            paramInfo[17] = new ParameterInfo("@Ytd_Tax13thMonth", row["Ytd_Tax13thMonth"]);
            paramInfo[18] = new ParameterInfo("@Ytd_OtherTaxIncome", row["Ytd_OtherTaxIncome"]);
            paramInfo[19] = new ParameterInfo("@usr_login", row["usr_login"]);
            paramInfo[20] = new ParameterInfo("@Ytd_WithPrvER", row["Ytd_WithPrvER"]);
            paramInfo[21] = new ParameterInfo("@Ytd_PrvER", row["Ytd_PrvER"]);
            paramInfo[22] = new ParameterInfo("@Ytd_CompanyAddressER", row["Ytd_CompanyAddressER"]);
            paramInfo[23] = new ParameterInfo("@Tyt_TIN", row["Tyt_TIN"]);
            paramInfo[24] = new ParameterInfo("@Ytd_CompanyZipCodeER", row["Ytd_CompanyZipCodeER"]);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
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
            int retVal = 0;
            string qString = @"UPDATE T_YTDAdjustment 
                                    SET Ytd_TaxIncomeAmt = @Ytd_TaxIncomeAmt
                                        ,Ytd_EEPremiumAmt = @Ytd_EEPremiumAmt
                                        ,Ytd_PremiumPaid = @Ytd_PremiumPaid
                                        ,Ytd_WtaxAmt = @Ytd_WtaxAmt
                                        ,Ytd_NonTaxIncomeAmt = @Ytd_NonTaxIncomeAmt
                                        ,Ytd_MWEBasicSalary =  @Ytd_MWEBasicSalary
                                        ,Tyt_MWEHoliday = @Tyt_MWEHoliday
                                        ,Tyt_MWEOvertime =  @Tyt_MWEOvertime
                                        ,Tyt_MWENightShiftPay =  @Tyt_MWENightShiftPay
                                        ,Tyt_MWEHazard = @Tyt_MWEHazard
                                        ,Ytd_NonTax13thMonth = @Ytd_NonTax13thMonth
                                        ,Ytd_DeMinimis = @Ytd_DeMinimis 
                                        ,Ytd_BasicSalary = @Ytd_BasicSalary
                                        ,Ytd_TaxOvertimePay = @Ytd_TaxOvertimePay
                                        ,Ytd_TaxHazardPay = @Ytd_TaxHazardPay
                                        ,Ytd_Tax13thMonth = @Ytd_Tax13thMonth
                                        ,Ytd_OtherTaxIncome = @Ytd_OtherTaxIncome
                                        ,usr_login = @usr_login
                                        ,ludatetime = GetDate()
                                        ,Ytd_WithPrvER = @Ytd_WithPrvER
                                        ,Ytd_PrvER = @Ytd_PrvER
                                        ,Ytd_CompanyAddressER = @Ytd_CompanyAddressER
                                        ,Tyt_TIN = @Tyt_TIN
                                        ,Ytd_CompanyZipCodeER = @Ytd_CompanyZipCodeER
                                    WHERE YTD_Period = @YTD_Period
                                        AND YTD_EmployeeId = @YTD_EmployeeId";


            ParameterInfo[] paramInfo = new ParameterInfo[25];
            paramInfo[0] = new ParameterInfo("@YTD_Period", row["YTD_Period"]);
            paramInfo[1] = new ParameterInfo("@YTD_EmployeeId", row["YTD_EmployeeId"]);
            paramInfo[2] = new ParameterInfo("@Ytd_TaxIncomeAmt", row["Ytd_TaxIncomeAmt"]);
            paramInfo[3] = new ParameterInfo("@Ytd_EEPremiumAmt", row["Ytd_EEPremiumAmt"]);
            paramInfo[4] = new ParameterInfo("@Ytd_PremiumPaid", row["Ytd_PremiumPaid"]);
            paramInfo[5] = new ParameterInfo("@Ytd_WtaxAmt", row["Ytd_WtaxAmt"]);
            paramInfo[6] = new ParameterInfo("@Ytd_NonTaxIncomeAmt", row["Ytd_NonTaxIncomeAmt"]);
            paramInfo[7] = new ParameterInfo("@Ytd_MWEBasicSalary", row["Ytd_MWEBasicSalary"]);
            paramInfo[8] = new ParameterInfo("@Tyt_MWEHoliday", row["Tyt_MWEHoliday"]);
            paramInfo[9] = new ParameterInfo("@Tyt_MWEOvertime", row["Tyt_MWEOvertime"]);
            paramInfo[10] = new ParameterInfo("@Tyt_MWENightShiftPay", row["Tyt_MWENightShiftPay"]);
            paramInfo[11] = new ParameterInfo("@Tyt_MWEHazard", row["Tyt_MWEHazard"]);
            paramInfo[12] = new ParameterInfo("@Ytd_NonTax13thMonth", row["Ytd_NonTax13thMonth"]);
            paramInfo[13] = new ParameterInfo("@Ytd_DeMinimis", row["Ytd_DeMinimis"]);
            paramInfo[14] = new ParameterInfo("@Ytd_BasicSalary", row["Ytd_BasicSalary"]);
            paramInfo[15] = new ParameterInfo("@Ytd_TaxOvertimePay", row["Ytd_TaxOvertimePay"]);
            paramInfo[16] = new ParameterInfo("@Ytd_TaxHazardPay", row["Ytd_TaxHazardPay"]);
            paramInfo[17] = new ParameterInfo("@Ytd_Tax13thMonth", row["Ytd_Tax13thMonth"]);
            paramInfo[18] = new ParameterInfo("@Ytd_OtherTaxIncome", row["Ytd_OtherTaxIncome"]);
            paramInfo[19] = new ParameterInfo("@usr_login", row["usr_login"]);
            paramInfo[20] = new ParameterInfo("@Ytd_WithPrvER", row["Ytd_WithPrvER"]);
            paramInfo[21] = new ParameterInfo("@Ytd_PrvER", row["Ytd_PrvER"]);
            paramInfo[22] = new ParameterInfo("@Ytd_CompanyAddressER", row["Ytd_CompanyAddressER"]);
            paramInfo[23] = new ParameterInfo("@Tyt_TIN", row["Tyt_TIN"]);
            paramInfo[24] = new ParameterInfo("@Ytd_CompanyZipCodeER", row["Ytd_CompanyZipCodeER"]);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
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

        public override int Delete(string EmployeeID, string Year)
        {
            DataSet ds = new DataSet();

            string qstring = string.Empty;
            qstring = @"DELETE
	                        FROM T_YTDAdjustment
		                        WHERE Tyt_IDNo = @EmployeeID
			                        AND YTD_Period = @Year";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@Year", Year);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qstring, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return 0;
        }

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataRow FetchRecord(string EmployeeID, string Year)
        {
            DataSet ds = new DataSet();

            string qstring = string.Empty;
            qstring = @"SELECT Ytd_TaxIncomeAmt
                                ,Ytd_EEPremiumAmt
                                ,Ytd_PremiumPaid
                                ,Ytd_WtaxAmt
                                ,Ytd_NonTaxIncomeAmt
                                ,Ytd_MWEBasicSalary
                                ,Tyt_MWEHoliday
                                ,Tyt_MWEOvertime
                                ,Ytd_MWENightShiftPay
                                ,Tyt_MWEHazard
                                ,Ytd_NonTax13thMonth
                                ,Ytd_DeMinimis
                                ,Ytd_BasicSalary
                                ,Ytd_TaxOvertimePay
                                ,Ytd_TaxHazardPay
                                ,Ytd_Tax13thMonth
                                ,Ytd_OtherTaxIncome
                              --,usr_login
                              --,ludatetime
                                ,Ytd_WithPrvER
                                ,Ytd_PrvER
                                ,Ytd_CompanyAddressER
                                ,Tyt_TIN
                                ,Ytd_CompanyZipCodeER
                                ,Mcd_Name
	                        FROM T_YTDAdjustment
                       LEFT JOIN M_CodeDtl on Ytd_CompanyZipCodeER = Mcd_Code 
                             AND Mcd_CodeType = 'ZIPCODE'
		                   WHERE Tyt_IDNo = @EmployeeID
			                 AND YTD_Period = @Year";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@Year", Year);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                //ds = dal.ExecuteDataSet(CommonConstants.StoredProcedures.spTrainingMasterEntryFetch, CommandType.StoredProcedure, paramInfo);
                ds = dal.ExecuteDataSet(qstring, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }


    }
    

}
