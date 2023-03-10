using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class NewSpecialYTDEntryBL : Payroll.BLogic.BaseBL
    {
        #region Overrides
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
        #endregion

        public DataSet GetGridDetails(string EmployeeID, string Year, object PeriodFrom, string CompanyCode, string CentralProfile)
        {
            DataSet ds = null;
            string query = string.Empty;
            query = string.Format(@"
                                DECLARE @StartDate DATE = '{2}'

                                SELECT Tyt_StartDate --CONVERT(varchar(20), [Tyt_StartDate], 101) AS Tyt_StartDate
                                      ,Tyt_EndDate --,CONVERT(varchar(20), [Tyt_EndDate], 101) AS Tyt_EndDate
                                      ,Tyt_EmployerType
                                      ,Tyt_EmployerName
                                      ,Tyt_TIN
                                      ,Tyt_EmployerAddress
                                      ,Tyt_EmployerZipCode
                                      ,Tyt_MWEBasic
                                      ,Tyt_MWEHoliday
                                      ,Tyt_MWEOvertime
                                      ,Tyt_MWENightShift
                                      ,Tyt_MWEHazard
                                      ,Tyt_Nontaxable13thMonth
                                      ,Tyt_DeMinimis
                                      ,Tyt_PremiumsUnionDues
                                      ,Tyt_NontaxableSalariesCompensation
                                      ,Tyt_TaxableBasic
                                      ,Tyt_TaxableBasicNetPremiums
                                      ,Tyt_Taxable13thMonth
                                      ,Tyt_TaxableSalariesCompensation
                                      ,Tyt_TaxableOvertime
                                      ,Tyt_Hazard
                                      ,Tyt_PremiumPaidOnHealth
                                      ,Tyt_TaxWithheld
                                      ,Tyt_Representation
                                      ,Tyt_Transportation
                                      ,Tyt_CostLivingAllowance
                                      ,Tyt_FixedHousingAllowance
                                      ,Tyt_OtherTaxable1
                                      ,Tyt_OtherTaxable2
                                      ,Tyt_Commision
                                      ,Tyt_ProfitSharing
                                      ,Tyt_Fees
                                      ,Tyt_SupplementaryTaxable1
                                      ,Tyt_SupplementaryTaxable2
                                      ,Tyt_EmploymentStatus
                                      ,Tyt_SeparationReason
                                      ,BIR_EMPSTAT.Mcd_Name AS BIR_EMPSTATDesc
                                      ,BIR_SEPCODE.Mcd_Name AS BIR_SEPCODEDesc
                                  FROM Udv_YearToDate
                                  LEFT JOIN M_CodeDtl BIR_EMPSTAT ON BIR_EMPSTAT.Mcd_Code = Tyt_EmploymentStatus
	                                    AND BIR_EMPSTAT.Mcd_CodeType = 'BIR_EMPSTAT'
                                        AND BIR_EMPSTAT.Mcd_CompanyCode = '{3}'
                                  LEFT JOIN M_CodeDtl BIR_SEPCODE ON BIR_SEPCODE.Mcd_Code = Tyt_SeparationReason
	                                    AND BIR_SEPCODE.Mcd_CodeType = 'BIR_SEPCODE'
                                        AND BIR_SEPCODE.Mcd_CompanyCode = '{3}' 
                                  WHERE Tyt_IDNo = '{0}'
                                        AND Tyt_TaxYear = '{1}'
                                        AND Tyt_StartDate = @StartDate
                                        AND Tyt_EmployerType IN ('C','P')
                                    ", EmployeeID
                                    , Year
                                    , PeriodFrom
                                    , CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);

                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds;
        }

        public bool isYearInW2History(string year, string CentralProfile)
        {
            bool ret = false;

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(
                        string.Format(@"
                            SELECT TOP 1 * FROM T_EmpYTDHst
                            WHERE Tyt_TaxYear = '{0}'
                                AND Tyt_EmployerType IN ('C','P')
                        ", year), CommandType.Text);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        ret = true;
                    else
                        ret = false;
                }
                catch(Exception er)
                {
                    ret = false;
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ret;
        }

        public void UpdateYTD(ParameterInfo[] param, string CentralProfile)
        {
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(@"
                        UPDATE T_EmpYTD
                           SET Tyt_EndDate = @Tyt_EndDate
                              ,Tyt_EmployerType = @Tyt_EmployerType
                              ,Tyt_EmployerName = @Tyt_EmployerName
                              ,Tyt_TIN = @Tyt_TIN
                              ,Tyt_EmployerAddress = @Tyt_EmployerAddress
                              ,Tyt_EmployerZipCode = @Tyt_EmployerZipCode
                              ,Tyt_MWEBasic = @Tyt_MWEBasic
                              ,Tyt_MWEHoliday = @Tyt_MWEHoliday
                              ,Tyt_MWEOvertime = @Tyt_MWEOvertime
                              ,Tyt_MWENightShift = @Tyt_MWENightShift
                              ,Tyt_MWEHazard = @Tyt_MWEHazard
                              ,Tyt_Nontaxable13thMonth = @Tyt_Nontaxable13thMonth
                              ,Tyt_DeMinimis = @Tyt_DeMinimis
                              ,Tyt_PremiumsUnionDues = @Tyt_PremiumsUnionDues
                              ,Tyt_NontaxableSalariesCompensation = @Tyt_NontaxableSalariesCompensation
                              ,Tyt_TaxableBasic = @Tyt_TaxableBasic
                              ,Tyt_TaxableBasicNetPremiums = @Tyt_TaxableBasicNetPremiums
                              ,Tyt_Taxable13thMonth = @Tyt_Taxable13thMonth
                              ,Tyt_TaxableSalariesCompensation = @Tyt_TaxableSalariesCompensation
                              ,Tyt_TaxableOvertime = @Tyt_TaxableOvertime
                              ,Tyt_Hazard = @Tyt_Hazard
                              ,Tyt_PremiumPaidOnHealth = @Tyt_PremiumPaidOnHealth
                              ,Tyt_TaxWithheld = @Tyt_TaxWithheld
                              ,Tyt_Representation = @Tyt_Representation
                              ,Tyt_Transportation = @Tyt_Transportation
                              ,Tyt_CostLivingAllowance = @Tyt_CostLivingAllowance
                              ,Tyt_FixedHousingAllowance = @Tyt_FixedHousingAllowance
                              ,Tyt_OtherTaxable1 = @Tyt_OtherTaxable1
                              ,Tyt_OtherTaxable2 = @Tyt_OtherTaxable2
                              ,Tyt_Commision = @Tyt_Commision
                              ,Tyt_ProfitSharing = @Tyt_ProfitSharing
                              ,Tyt_Fees = @Tyt_Fees
                              ,Tyt_SupplementaryTaxable1 = @Tyt_SupplementaryTaxable1
                              ,Tyt_SupplementaryTaxable2 = @Tyt_SupplementaryTaxable2
                              ,Tyt_EmploymentStatus=@Tyt_EmploymentStatus
                              ,Tyt_SeparationReason=@Tyt_SeparationReason
                              ,Usr_Login = @Usr_Login
                              ,Ludatetime = GETDATE()
                         WHERE Tyt_IDNo = @Tyt_IDNo
                             AND Tyt_TaxYear = @Tyt_TaxYear
                             AND Tyt_StartDate = @Tyt_StartDate
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully updated record.");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError("Error in update : " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public int UpdateYTD(ParameterInfo[] param, string CentralProfile, DALHelper dalHelper)
        {
            try
            {
                return dalHelper.ExecuteNonQuery(@"
                            UPDATE T_EmpYTD
                            SET Tyt_EndDate = @Tyt_EndDate
                              ,Tyt_EmployerType = @Tyt_EmployerType
                              ,Tyt_EmployerName = @Tyt_EmployerName
                              ,Tyt_TIN = @Tyt_TIN
                              ,Tyt_EmployerAddress = @Tyt_EmployerAddress
                              ,Tyt_EmployerZipCode = @Tyt_EmployerZipCode
                              ,Tyt_MWEBasic = @Tyt_MWEBasic
                              ,Tyt_MWEHoliday = @Tyt_MWEHoliday
                              ,Tyt_MWEOvertime = @Tyt_MWEOvertime
                              ,Tyt_MWENightShift = @Tyt_MWENightShift
                              ,Tyt_MWEHazard = @Tyt_MWEHazard
                              ,Tyt_Nontaxable13thMonth = @Tyt_Nontaxable13thMonth
                              ,Tyt_DeMinimis = @Tyt_DeMinimis
                              ,Tyt_PremiumsUnionDues = @Tyt_PremiumsUnionDues
                              ,Tyt_NontaxableSalariesCompensation = @Tyt_NontaxableSalariesCompensation
                              ,Tyt_TaxableBasic = @Tyt_TaxableBasic
                              ,Tyt_TaxableBasicNetPremiums = @Tyt_TaxableBasicNetPremiums
                              ,Tyt_Taxable13thMonth = @Tyt_Taxable13thMonth
                              ,Tyt_TaxableSalariesCompensation = @Tyt_TaxableSalariesCompensation
                              ,Tyt_TaxableOvertime = @Tyt_TaxableOvertime
                              ,Tyt_Hazard = @Tyt_Hazard
                              ,Tyt_PremiumPaidOnHealth = @Tyt_PremiumPaidOnHealth
                              ,Tyt_TaxWithheld = @Tyt_TaxWithheld
                              ,Tyt_Representation = @Tyt_Representation
                              ,Tyt_Transportation = @Tyt_Transportation
                              ,Tyt_CostLivingAllowance = @Tyt_CostLivingAllowance
                              ,Tyt_FixedHousingAllowance = @Tyt_FixedHousingAllowance
                              ,Tyt_OtherTaxable1 = @Tyt_OtherTaxable1
                              ,Tyt_OtherTaxable2 = @Tyt_OtherTaxable2
                              ,Tyt_Commision = @Tyt_Commision
                              ,Tyt_ProfitSharing = @Tyt_ProfitSharing
                              ,Tyt_Fees = @Tyt_Fees
                              ,Tyt_SupplementaryTaxable1 = @Tyt_SupplementaryTaxable1
                              ,Tyt_SupplementaryTaxable2 = @Tyt_SupplementaryTaxable2
                              ,Tyt_EmploymentStatus=@Tyt_EmploymentStatus
                              ,Tyt_SeparationReason=@Tyt_SeparationReason
                              ,Usr_Login = @Usr_Login
                              ,Ludatetime = GETDATE()
                         WHERE Tyt_IDNo = @Tyt_IDNo
                             AND Tyt_TaxYear = @Tyt_TaxYear
                             AND Tyt_StartDate = @Tyt_StartDate
                        ", CommandType.Text, param);
            }
            catch
            {
                return 0;
            }
        }

        public void InsertYTD(ParameterInfo[] param, string CentralProfile)
        {
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(@"
                        INSERT INTO T_EmpYTD
                               (Tyt_IDNo
                               ,Tyt_TaxYear
                               ,Tyt_StartDate
                               ,Tyt_EndDate
                               ,Tyt_EmployerType
                               ,Tyt_EmployerName
                               ,Tyt_TIN
                               ,Tyt_EmployerAddress
                               ,Tyt_EmployerZipCode
                               ,Tyt_MWEBasic
                               ,Tyt_MWEHoliday
                               ,Tyt_MWEOvertime
                               ,Tyt_MWENightShift
                               ,Tyt_MWEHazard
                               ,Tyt_Nontaxable13thMonth
                               ,Tyt_DeMinimis
                               ,Tyt_PremiumsUnionDues
                               ,Tyt_NontaxableSalariesCompensation
                               ,Tyt_TaxableBasic
                               ,Tyt_TaxableBasicNetPremiums
                               ,Tyt_Taxable13thMonth
                               ,Tyt_TaxableSalariesCompensation
                               ,Tyt_TaxableOvertime
                               ,Tyt_Hazard
                               ,Tyt_PremiumPaidOnHealth
                               ,Tyt_TaxWithheld
                               ,Tyt_Representation
                               ,Tyt_Transportation
                               ,Tyt_CostLivingAllowance
                               ,Tyt_FixedHousingAllowance
                               ,Tyt_OtherTaxable1
                               ,Tyt_OtherTaxable2
                               ,Tyt_Commision
                               ,Tyt_ProfitSharing
                               ,Tyt_Fees
                               ,Tyt_SupplementaryTaxable1
                               ,Tyt_SupplementaryTaxable2
                               ,Tyt_EmploymentStatus
                               ,Tyt_SeparationReason
                               ,Usr_Login
                               ,Ludatetime)
                         VALUES
                               (@Tyt_IDNo
                               ,@Tyt_TaxYear
                               ,@Tyt_StartDate
                               ,@Tyt_EndDate
                               ,@Tyt_EmployerType
                               ,@Tyt_EmployerName
                               ,@Tyt_TIN
                               ,@Tyt_EmployerAddress
                               ,@Tyt_EmployerZipCode
                               ,@Tyt_MWEBasic
                               ,@Tyt_MWEHoliday
                               ,@Tyt_MWEOvertime
                               ,@Tyt_MWENightShift
                               ,@Tyt_MWEHazard
                               ,@Tyt_Nontaxable13thMonth
                               ,@Tyt_DeMinimis
                               ,@Tyt_PremiumsUnionDues
                               ,@Tyt_NontaxableSalariesCompensation
                               ,@Tyt_TaxableBasic
                               ,@Tyt_TaxableBasicNetPremiums
                               ,@Tyt_Taxable13thMonth
                               ,@Tyt_TaxableSalariesCompensation
                               ,@Tyt_TaxableOvertime
                               ,@Tyt_Hazard
                               ,@Tyt_PremiumPaidOnHealth
                               ,@Tyt_TaxWithheld
                               ,@Tyt_Representation
                               ,@Tyt_Transportation
                               ,@Tyt_CostLivingAllowance
                               ,@Tyt_FixedHousingAllowance
                               ,@Tyt_OtherTaxable1
                               ,@Tyt_OtherTaxable2
                               ,@Tyt_Commision
                               ,@Tyt_ProfitSharing
                               ,@Tyt_Fees
                               ,@Tyt_SupplementaryTaxable1
                               ,@Tyt_SupplementaryTaxable2
                               ,@Tyt_EmploymentStatus
                               ,@Tyt_SeparationReason
                               ,@Usr_Login
                               ,GETDATE())
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully inserted record.");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError("Error in saving : " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public int InsertYTD(ParameterInfo[] param, string CentralProfile, DALHelper dalHelper)
        {
            try
            {
                return dalHelper.ExecuteNonQuery(@"
                        INSERT INTO T_EmpYTD
                               (Tyt_IDNo
                               ,Tyt_TaxYear
                               ,Tyt_StartDate
                               ,Tyt_EndDate
                               ,Tyt_EmployerType
                               ,Tyt_EmployerName
                               ,Tyt_TIN
                               ,Tyt_EmployerAddress
                               ,Tyt_EmployerZipCode
                               ,Tyt_MWEBasic
                               ,Tyt_MWEHoliday
                               ,Tyt_MWEOvertime
                               ,Tyt_MWENightShift
                               ,Tyt_MWEHazard
                               ,Tyt_Nontaxable13thMonth
                               ,Tyt_DeMinimis
                               ,Tyt_PremiumsUnionDues
                               ,Tyt_NontaxableSalariesCompensation
                               ,Tyt_TaxableBasic
                               ,Tyt_TaxableBasicNetPremiums
                               ,Tyt_Taxable13thMonth
                               ,Tyt_TaxableSalariesCompensation
                               ,Tyt_TaxableOvertime
                               ,Tyt_Hazard
                               ,Tyt_PremiumPaidOnHealth
                               ,Tyt_TaxWithheld
                               ,Tyt_Representation
                               ,Tyt_Transportation
                               ,Tyt_CostLivingAllowance
                               ,Tyt_FixedHousingAllowance
                               ,Tyt_OtherTaxable1
                               ,Tyt_OtherTaxable2
                               ,Tyt_Commision
                               ,Tyt_ProfitSharing
                               ,Tyt_Fees
                               ,Tyt_SupplementaryTaxable1
                               ,Tyt_SupplementaryTaxable2
                               ,Tyt_EmploymentStatus
                               ,Tyt_SeparationReason
                               ,Usr_Login
                               ,Ludatetime)
                         VALUES
                               (@Tyt_IDNo
                               ,@Tyt_TaxYear
                               ,@Tyt_StartDate
                               ,@Tyt_EndDate
                               ,@Tyt_EmployerType
                               ,@Tyt_EmployerName
                               ,@Tyt_TIN
                               ,@Tyt_EmployerAddress
                               ,@Tyt_EmployerZipCode
                               ,@Tyt_MWEBasic
                               ,@Tyt_MWEHoliday
                               ,@Tyt_MWEOvertime
                               ,@Tyt_MWENightShift
                               ,@Tyt_MWEHazard
                               ,@Tyt_Nontaxable13thMonth
                               ,@Tyt_DeMinimis
                               ,@Tyt_PremiumsUnionDues
                               ,@Tyt_NontaxableSalariesCompensation
                               ,@Tyt_TaxableBasic
                               ,@Tyt_TaxableBasicNetPremiums
                               ,@Tyt_Taxable13thMonth
                               ,@Tyt_TaxableSalariesCompensation
                               ,@Tyt_TaxableOvertime
                               ,@Tyt_Hazard
                               ,@Tyt_PremiumPaidOnHealth
                               ,@Tyt_TaxWithheld
                               ,@Tyt_Representation
                               ,@Tyt_Transportation
                               ,@Tyt_CostLivingAllowance
                               ,@Tyt_FixedHousingAllowance
                               ,@Tyt_OtherTaxable1
                               ,@Tyt_OtherTaxable2
                               ,@Tyt_Commision
                               ,@Tyt_ProfitSharing
                               ,@Tyt_Fees
                               ,@Tyt_SupplementaryTaxable1
                               ,@Tyt_SupplementaryTaxable2
                               ,@Tyt_EmploymentStatus
                               ,@Tyt_SeparationReason
                               ,@Usr_Login
                               ,GETDATE())
                        ", CommandType.Text, param);
            }
            catch
            {
                return 0;
            }
        }

        public void DeleteYTD(string IDNumber, string TaxYear, object StartDate, string EmployerType, string CentralProfile)
        {
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(string.Format(@"DECLARE @StartDate DATE = '{2}'

                                                        DELETE FROM T_EmpYTD
                                                        WHERE Tyt_TaxYear = '{0}'
	                                                        AND Tyt_IDNo = '{1}'
	                                                        AND Tyt_StartDate = @StartDate
	                                                        AND Tyt_EmployerType = '{3}'              
                                                            ", TaxYear
                                                            , IDNumber
                                                            , StartDate
                                                            , EmployerType));

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully deleted record.");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError("Error in deletion : " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public bool checkForPrimaryKey(string EmployeeID, object startPeriod, object endPeriod, bool isPreviousEmployer, string Year)
        { 
            bool ret = false;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string strEmployer = string.Empty;

                    if (isPreviousEmployer)
                    {
                        strEmployer = "P";
                    }
                    else
                    {
                        strEmployer = "C";
                    }

                    string strQuery = string.Format(@"
                        DECLARE @StartDate DATE = '{2}'
                        DECLARE @EndDate DATE = '{3}'

                        SELECT TOP 1 * 
                        FROM T_EmpYTD
                        WHERE Tyt_IDNo = '{0}'
                            AND Tyt_TaxYear = '{1}'
                            AND ((@StartDate BETWEEN Tyt_StartDate AND Tyt_EndDate) OR (@EndDate BETWEEN Tyt_StartDate AND Tyt_EndDate))
                            AND Tyt_EmployerType = '{4}'
                    ", EmployeeID, Year, startPeriod, endPeriod, strEmployer);

                    DataSet ds = dal.ExecuteDataSet(strQuery);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                    ret = true;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public bool IsYTDExist(string empID, string year, object startDate, string CentralProfile)
        {
            bool ret = false;

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(
                        string.Format(@"
                            DECLARE @StartDate DATE = '{2}'

                            SELECT * 
                            FROM T_EmpYTD 
                            WHERE Tyt_IDNo = '{0}'
                                AND Tyt_TaxYear = '{1}'
                                AND Tyt_StartDate = @StartDate
                                AND Tyt_EmployerType IN ('C','P') ", empID, year, startDate), CommandType.Text);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        ret = true;
                    else
                        ret = false;
                }
                catch (Exception er)
                {
                    ret = false;
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ret;
        }

        public bool CheckIfPrevEmployerExists(string EmployeeID, string Year, string ERTIN, string ERName, string CentralProfile)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    string strQuery = string.Format(@"
                        SELECT TOP 1 * 
                        FROM Udv_YearToDate
                        WHERE Tyt_IDNo = '{0}'
                            AND Tyt_TaxYear = '{1}'
                            AND Tyt_EmployerType = 'P'
                            AND Tyt_TIN = '{2}'
                            AND Tyt_EmployerName = '{3}'
                    ", EmployeeID, Year, ERTIN, ERName);

                    DataSet ds = dal.ExecuteDataSet(strQuery);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                    ret = true;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public DataTable GetYTDDetails(string EmployeeID, string year, string CentralProfile)
        {
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();

                    string query = @"
                            SELECT Tyt_EmployerType
                                , Tyt_EmployerName
                                , Tyt_EmployerAddress
                                , Tyt_EmployerZipCode
                                , Tyt_TIN
                            FROM Udv_YearToDate
                            WHERE Tyt_IDNo = '{0}'
                                AND Tyt_TaxYear = '{1}'
                                AND Tyt_EmployerType IN ('C','P')                            
                    ";

                    query = string.Format(query, EmployeeID, year);
                    dtResult = dal.ExecuteDataSet(query).Tables[0];


                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dtResult;
        }
    }
}
