using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class NewW2EntryBL : Payroll.BLogic.BaseBL
    {
        #region Overrides
        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        public DataTable GetAlphalistTaxInfo(string TaxYear, string CompanyCode, string CentralProfile)
        {
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();

                    string query = @"
                            SELECT Tal_OtherTaxableSpecify1
                                ,Tal_OtherTaxableSpecify2
                                ,Tal_SupplementaryTaxableSpecify1
                                ,Tal_SupplementaryTaxableSpecify2
                            FROM T_Alphalist
                            WHERE Tal_CompanyCode = '{0}'
                                AND Tal_TaxYear = '{1}'
                    ";

                    query = string.Format(query, CompanyCode, TaxYear);
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

        public DataTable GetAlphalistTaxInfo(string TaxYear, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataTable dtResult = null;
            try
            {
                string query = @"
                            SELECT Tal_OtherTaxableSpecify1
                                ,Tal_OtherTaxableSpecify2
                                ,Tal_SupplementaryTaxableSpecify1
                                ,Tal_SupplementaryTaxableSpecify2
                            FROM T_Alphalist
                            WHERE Tal_CompanyCode = '{0}'
                                AND Tal_TaxYear = '{1}'
                    ";

                query = string.Format(query, CompanyCode, TaxYear);
                dtResult = dal.ExecuteDataSet(query).Tables[0];
            }
            catch (Exception er)
            {
                CommonProcedures.showMessageError(er.Message);
            }
            return dtResult;
        }

        public DataTable GetTaxMasterInfo(string CompanyCode, string CentralProfile)
        {
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();

                    string query = @"
                            SELECT Mtx_OtherTaxableSpecify1
                            , Mtx_OtherTaxableSpecify2
                            , Mtx_SupplementaryTaxableSpecify1
                            , Mtx_SupplementaryTaxableSpecify2
                            FROM M_Tax
                            WHERE Mtx_CompanyCode = '{0}'
                    ";

                    query = string.Format(query, CompanyCode);
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

        public DataSet GetData(string IDNumber, string TaxYear, string CentralProfile)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                        string.Format(@"
                            SELECT Tah_TaxYear [Year] 
	                        ,Tah_StartDate [PeriodFrom]
	                        ,Tah_EndDate [PeriodTo]
                        FROM T_EmpAlphalistHdr
                        WHERE Tah_IDNo = '{0}'
                            AND Tah_TaxYear = '{1}'
                        ORDER BY Tah_TaxYear DESC
                        ", IDNumber, TaxYear));

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

        public DataSet GetDetails(string EmployeeID, string year, string SchedCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    string W2table = string.Empty;

                    DataSet dsYear = dal.ExecuteDataSet(string.Format(@"
                        SELECT TOP 1 * FROM T_EmpAlphalistHdrHst
                            WHERE Tah_TaxYear = '{0}'
                    ", year));

                    if (dsYear != null
                        && dsYear.Tables[0].Rows.Count > 0)
                        W2table = CommonConstants.TableName.T_EmpAlphalistHdrHst;
                    else
                        W2table = CommonConstants.TableName.T_EmpAlphalistHdr;
                    
                    #region Query

                    string query = @"
                            SELECT Tah_IDNo
                                  ,Tah_TaxYear
                                  ,Tah_StartDate 
                                  ,Tah_EndDate 
                                  ,Tah_LastName
                                  ,Tah_FirstName
                                  ,Tah_MiddleName
                                  ,Tah_Schedule
                                  ,{4}.Mcd_Name AS Tah_ScheduleDesc
                                  ,Tah_TIN
                                  ,Tah_PresCompleteAddress
                                  ,Tah_PresAddressMunicipalityCity
                                  ,Tah_TelephoneNo
                                  ,Tah_ProvCompleteAddress
                                  ,Tah_ProvAddressMunicipalityCity
                                  ,Tah_WifeClaim
                                  ,Tah_MWEBasicCurER
                                  ,Tah_MWEHolidayCurER
                                  ,Tah_MWEOvertimeCurER
                                  ,Tah_MWENightShiftCurER
                                  ,Tah_MWEHazardCurER
                                  ,Tah_Nontaxable13thMonthCurER
                                  ,Tah_DeMinimisCurER
                                  ,Tah_PremiumsUnionDuesCurER
                                  ,Tah_NontaxableSalariesCompensationCurER
                                  ,Tah_TaxableBasicNetPremiumsCurER
                                  ,Tah_TaxableBasicCurER
                                  ,Tah_TaxableOvertimeCurER
                                  ,Tah_Taxable13thMonthCurER
                                  ,Tah_TaxableSalariesCompensationCurER
                                  ,Tah_MWEBasicPrvER
                                  ,Tah_MWEHolidayPrvER
                                  ,Tah_MWEOvertimePrvER
                                  ,Tah_MWENightShiftPrvER
                                  ,Tah_MWEHazardPrvER
                                  ,Tah_Nontaxable13thMonthPrvER
                                  ,Tah_TaxableBasicNetPremiumsPrvER
                                  ,Tah_DeMinimisPrvER
                                  ,Tah_PremiumsUnionDuesPrvER
                                  ,Tah_NontaxableSalariesCompensationPrvER
                                  
                                  ,Tah_RepresentationCurER
                                  ,Tah_TransportationCurER
                                  ,Tah_CostLivingAllowanceCurER
                                  ,Tah_FixedHousingAllowanceCurER
                                  ,Tah_OtherTaxable1CurER
                                  ,Tah_OtherTaxable2CurER
                                  ,Tah_CommisionCurER
                                  ,Tah_ProfitSharingCurER
                                  ,Tah_FeesCurER
                                  ,Tah_HazardCurER
                                  ,Tah_SupplementaryTaxable1CurER
                                  ,Tah_SupplementaryTaxable2CurER

                                  ,Tah_TaxableBasicPrvER
                                  ,Tah_TaxableOvertimePrvER
                                  ,Tah_Taxable13thMonthPrvER
                                  ,Tah_TaxableSalariesCompensationPrvER

                                  ,Tah_RepresentationPrvER
                                  ,Tah_TransportationPrvER
                                  ,Tah_CostLivingAllowancePrvER
                                  ,Tah_FixedHousingAllowancePrvER
                                  ,Tah_OtherTaxable1PrvER
                                  ,Tah_OtherTaxable2PrvER
                                  ,Tah_CommisionPrvER
                                  ,Tah_ProfitSharingPrvER
                                  ,Tah_FeesPrvER
                                  ,Tah_HazardPrvER
                                  ,Tah_SupplementaryTaxable1PrvER
                                  ,Tah_SupplementaryTaxable2PrvER

                                  ,Tah_ExemptionCode
                                  ,Tah_ExemptionAmount
                                  ,Tah_PremiumPaidOnHealth
                                  ,Tah_TaxDue
                                  ,Tah_TaxWithheldPrvER
                                  ,Tah_TaxWithheldCurER
                                  ,Tah_TotalTaxWithheldJanDec
                                  ,Tah_TaxAmount
                                  ,Tah_TaxBracket
                                  ,Tah_IsSubstitutedFiling
                                  ,Tah_IsTaxExempted
                                  ,Tah_FinalPayIndicator
                                  ,Tah_IsZeroOutLastQuinYear
                                  ,Tah_CostcenterCode
                                  ,Tah_PayrollType
                                  ,Tah_PayrollGroup
                                  ,Tah_WorkStatus
                                  ,Tah_EmploymentStatus
                                  ,CASE WHEN Mur_UserCode IS NOT NULL
                                    THEN Mur_UserLastName + ', ' + Mur_UserFirstName + ' ' + Mur_UserMiddleName
                                    ELSE ''
                                    END AS Usr_Login
                                  ,{2}.Ludatetime
                                  ,Tah_GrossCompensationIncome
                                  ,Tah_NontaxableIncomeCurER
                                  ,Tah_NontaxableIncomePrvER
                                  ,Tah_TaxableIncomeCurER
                                  ,Tah_TaxableIncomePrvER
                                  ,Tah_NetTaxableIncome
                                  ,Tah_TotalTaxWithheld
                                  ,Tah_AssumeBasic
                                  ,Tah_Assume13th
                                  ,Tah_AssumeSalariesCompensation
                                  ,Tah_AssumePremiumsUnionDues
                                  ,Tah_AssumePayCycle
                                  ,Tah_CurrentEmploymentStatus
                                  ,BIR_EMPSTAT.Mcd_Name AS [CurrentEmploymentStatus]
                                  ,Tah_Nationality
                                  ,CITIZEN.Mcd_Name AS [Nationality]
                                  ,Tah_SeparationReason
                                  ,BIR_SEPCODE.Mcd_Name AS [SeparationReason]
                            FROM {2}
                            LEFT JOIN M_CodeDtl {4}
                                ON {4}.Mcd_Code = Tah_Schedule
                                AND {4}.Mcd_CodeType = '{4}'
                                AND {4}.Mcd_CompanyCode = '{3}'
                            LEFT JOIN M_CodeDtl CITIZEN
                                ON CITIZEN.Mcd_Code = Tah_Nationality
                                AND CITIZEN.Mcd_CodeType = 'CITIZEN'
                                AND CITIZEN.Mcd_CompanyCode = '{3}'
                            LEFT JOIN M_CodeDtl BIR_EMPSTAT
                                ON BIR_EMPSTAT.Mcd_Code = Tah_CurrentEmploymentStatus
                                AND BIR_EMPSTAT.Mcd_CodeType = 'BIR_EMPSTAT'
                                AND BIR_EMPSTAT.Mcd_CompanyCode = '{3}'
                            LEFT JOIN M_CodeDtl BIR_SEPCODE
                                ON BIR_SEPCODE.Mcd_Code = Tah_SeparationReason
                                AND BIR_SEPCODE.Mcd_CodeType = 'BIR_SEPCODE'
                                AND BIR_SEPCODE.Mcd_CompanyCode = '{3}'
                            LEFT JOIN M_User
                                ON Mur_UserCode = {2}.Usr_Login
                            WHERE Tah_IDNo = '{0}'
                                AND Tah_TaxYear = '{1}'
                    ";

                    #endregion

                    query = string.Format(query, EmployeeID, year, W2table, CompanyCode, SchedCode);
                    ds = dal.ExecuteDataSet(query);


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
        
        public DataTable GetCTCDetails(string EmployeeID, string year, string CentralProfile)
        {
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();

                    string query = @"
                            SELECT Tct_CommunityTaxNo
                                   , Tct_IssuedAt
                                   , Tct_IssuedDate
                                   , Tct_IssuedBy
                                   , Tct_PaidAmt
                            FROM T_EmpCommunityTax
                            WHERE Tct_IDNo = '{0}'
                            AND Tct_TaxYear = '{1}'
                            UNION ALL
                            SELECT Tct_CommunityTaxNo
                                   , Tct_IssuedAt
                                   , Tct_IssuedDate
                                   , Tct_IssuedBy
                                   , Tct_PaidAmt
                            FROM T_EmpCommunityTaxHst
                            WHERE Tct_IDNo = '{0}'
                            AND Tct_TaxYear = '{1}'
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

        public DataSet GetEmployeeDetails(string employeeID)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                        string.Format(@"
                            SELECT 
                                  CASE WHEN RTRIM(Mem_PresLocationCode) = ''
								    THEN 'NONE'
								    ELSE Mem_PresLocationCode
								    END AS [Mem_PresLocationCode]
                                , CASE WHEN RTRIM(Mem_PresMailingAddress) = ''
								    THEN 'NONE'
								    ELSE Mem_PresMailingAddress
								    END AS [Mem_PresMailingAddress]
                                , CASE WHEN RTRIM(Mem_ProvLocationCode) = ''
								    THEN 'NONE'
								    ELSE Mem_ProvLocationCode
								    END AS Mem_ProvLocationCode
                                , CASE WHEN RTRIM(Mem_ProvMailingAddress) = ''
								    THEN 'NONE'
								    ELSE Mem_ProvMailingAddress
								    END AS Mem_ProvMailingAddress
                                , Mem_WifeClaim
                                , Mem_TaxCode
                                , Mem_TIN
                                , ISNULL(Mem_IsTaxExempted ,0) AS Mem_IsTaxExempted 
                                , Mem_CostcenterCode
							    , Mem_PayrollType
							    , Mem_PayrollGroup
							    , Mem_WorkStatus
							    , Mem_EmploymentStatusCode
                                , Mem_PayrollGroup
                                , CASE WHEN RTRIM(Mem_PresContactNo) = ''
		                            THEN 'NONE'  
	                             ELSE 
	                                RTRIM(Mem_PresContactNo)
								 END AS Mem_PresContactNo
                                , Mem_MiddleName
                                , Mem_Gender
                            FROM M_Employee
                            WHERE Mem_IDNo = '{0}'
                        ", employeeID));

                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public bool CheckIfPrimaryKeyAlreadyExists(string EmployeeID, string Year, string CentralProfile)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(
                        string.Format(@"
                            SELECT * FROM T_EmpAlphalistHdr
                            WHERE Tah_IDNo = '{0}'
                            AND Tah_TaxYear = '{1}'
                            UNION ALL
                            SELECT * FROM T_EmpAlphalistHdrHst
                            WHERE Tah_IDNo = '{0}'
                            AND Tah_TaxYear = '{1}'
                        ", EmployeeID, Year));
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
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public void InsertNewW2Entry(ParameterInfo[] param, string CentralProfile)
        {
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    #region Query
                    string query = @"
                        INSERT INTO T_EmpAlphalistHdr
                                   (Tah_IDNo
                                    ,Tah_TaxYear
                                    ,Tah_StartDate
                                    ,Tah_EndDate
                                    ,Tah_LastName
                                    ,Tah_FirstName
                                    ,Tah_MiddleName
                                    ,Tah_ExtensionName
                                    ,Tah_Schedule
                                    ,Tah_TIN
                                    ,Tah_PresCompleteAddress
                                    ,Tah_PresAddressMunicipalityCity
                                    ,Tah_TelephoneNo
                                    ,Tah_ProvCompleteAddress
                                    ,Tah_ProvAddressMunicipalityCity
                                    ,Tah_WifeClaim
                                    ,Tah_MWEBasicCurER
                                    ,Tah_MWEHolidayCurER
                                    ,Tah_MWEOvertimeCurER
                                    ,Tah_MWENightShiftCurER
                                    ,Tah_MWEHazardCurER
                                    ,Tah_Nontaxable13thMonthCurER
                                    ,Tah_DeMinimisCurER
                                    ,Tah_PremiumsUnionDuesCurER
                                    ,Tah_NontaxableSalariesCompensationCurER
                                    ,Tah_TaxableBasicCurER
                                    ,Tah_TaxableBasicNetPremiumsCurER
                                    ,Tah_TaxableOvertimeCurER
                                    ,Tah_Taxable13thMonthCurER
                                    ,Tah_TaxableSalariesCompensationCurER
                                    ,Tah_MWEBasicPrvER
                                    ,Tah_MWEHolidayPrvER
                                    ,Tah_MWEOvertimePrvER
                                    ,Tah_MWENightShiftPrvER
                                    ,Tah_MWEHazardPrvER
                                    ,Tah_Nontaxable13thMonthPrvER
                                    ,Tah_DeMinimisPrvER
                                    ,Tah_PremiumsUnionDuesPrvER
                                    ,Tah_NontaxableSalariesCompensationPrvER
                                    ,Tah_TaxableBasicPrvER
                                    ,Tah_TaxableBasicNetPremiumsPrvER
                                    ,Tah_TaxableOvertimePrvER
                                    ,Tah_Taxable13thMonthPrvER
                                    ,Tah_TaxableSalariesCompensationPrvER
                                    ,Tah_RepresentationCurER
                                    ,Tah_TransportationCurER
                                    ,Tah_CostLivingAllowanceCurER
                                    ,Tah_FixedHousingAllowanceCurER
                                    ,Tah_OtherTaxable1CurER
                                    ,Tah_OtherTaxable2CurER
                                    ,Tah_CommisionCurER
                                    ,Tah_ProfitSharingCurER
                                    ,Tah_FeesCurER
                                    ,Tah_HazardCurER
                                    ,Tah_SupplementaryTaxable1CurER
                                    ,Tah_SupplementaryTaxable2CurER
                                    ,Tah_RepresentationPrvER
                                    ,Tah_TransportationPrvER
                                    ,Tah_CostLivingAllowancePrvER
                                    ,Tah_FixedHousingAllowancePrvER
                                    ,Tah_OtherTaxable1PrvER
                                    ,Tah_OtherTaxable2PrvER
                                    ,Tah_CommisionPrvER
                                    ,Tah_ProfitSharingPrvER
                                    ,Tah_FeesPrvER
                                    ,Tah_HazardPrvER
                                    ,Tah_SupplementaryTaxable1PrvER
                                    ,Tah_SupplementaryTaxable2PrvER
                                    ,Tah_ExemptionCode
                                    ,Tah_ExemptionAmount
                                    ,Tah_PremiumPaidOnHealth
                                    ,Tah_GrossCompensationIncome
                                    ,Tah_NontaxableIncomeCurER
                                    ,Tah_NontaxableIncomePrvER
                                    ,Tah_TaxableIncomeCurER
                                    ,Tah_TaxableIncomePrvER
                                    ,Tah_NetTaxableIncome
                                    ,Tah_TaxDue
                                    ,Tah_TaxWithheldPrvER
                                    ,Tah_TaxWithheldCurER
                                    ,Tah_TotalTaxWithheld
                                    ,Tah_TotalTaxWithheldJanDec
                                    ,Tah_TaxAmount
                                    ,Tah_TaxBracket
                                    ,Tah_IsSubstitutedFiling
                                    ,Tah_IsTaxExempted
                                    ,Tah_FinalPayIndicator
                                    ,Tah_IsZeroOutLastQuinYear
                                    ,Tah_CostcenterCode
                                    ,Tah_PayrollGroup
                                    ,Tah_PayrollType
                                    ,Tah_EmploymentStatus
                                    ,Tah_WorkStatus
                                    ,Tah_AssumeBasic
                                    ,Tah_Assume13th
                                    ,Tah_AssumeSalariesCompensation
                                    ,Tah_AssumePremiumsUnionDues
                                    ,Tah_AssumePayCycle
                                    ,Tah_CurrentEmploymentStatus
                                    ,Tah_Nationality
                                    ,Tah_SeparationReason
                                    ,Usr_Login
                                    ,Ludatetime)
                             VALUES
                                   (@Tah_IDNo
                                    ,@Tah_TaxYear
                                    ,@Tah_StartDate
                                    ,@Tah_EndDate
                                    ,@Tah_LastName
                                    ,@Tah_FirstName
                                    ,@Tah_MiddleName
                                    ,@Tah_ExtensionName
                                    ,@Tah_Schedule
                                    ,@Tah_TIN
                                    ,@Tah_PresCompleteAddress
                                    ,@Tah_PresAddressMunicipalityCity
                                    ,@Tah_TelephoneNo
                                    ,@Tah_ProvCompleteAddress
                                    ,@Tah_ProvAddressMunicipalityCity
                                    ,@Tah_WifeClaim
                                    ,@Tah_MWEBasicCurER
                                    ,@Tah_MWEHolidayCurER
                                    ,@Tah_MWEOvertimeCurER
                                    ,@Tah_MWENightShiftCurER
                                    ,@Tah_MWEHazardCurER
                                    ,@Tah_Nontaxable13thMonthCurER
                                    ,@Tah_DeMinimisCurER
                                    ,@Tah_PremiumsUnionDuesCurER
                                    ,@Tah_NontaxableSalariesCompensationCurER
                                    ,@Tah_TaxableBasicCurER
                                    ,@Tah_TaxableBasicNetPremiumsCurER
                                    ,@Tah_TaxableOvertimeCurER
                                    ,@Tah_Taxable13thMonthCurER
                                    ,@Tah_TaxableSalariesCompensationCurER
                                    ,@Tah_MWEBasicPrvER
                                    ,@Tah_MWEHolidayPrvER
                                    ,@Tah_MWEOvertimePrvER
                                    ,@Tah_MWENightShiftPrvER
                                    ,@Tah_MWEHazardPrvER
                                    ,@Tah_Nontaxable13thMonthPrvER
                                    ,@Tah_DeMinimisPrvER
                                    ,@Tah_PremiumsUnionDuesPrvER
                                    ,@Tah_NontaxableSalariesCompensationPrvER
                                    ,@Tah_TaxableBasicPrvER
                                    ,@Tah_TaxableBasicNetPremiumsPrvER
                                    ,@Tah_TaxableOvertimePrvER
                                    ,@Tah_Taxable13thMonthPrvER
                                    ,@Tah_TaxableSalariesCompensationPrvER
                                    ,@Tah_RepresentationCurER
                                    ,@Tah_TransportationCurER
                                    ,@Tah_CostLivingAllowanceCurER
                                    ,@Tah_FixedHousingAllowanceCurER
                                    ,@Tah_OtherTaxable1CurER
                                    ,@Tah_OtherTaxable2CurER
                                    ,@Tah_CommisionCurER
                                    ,@Tah_ProfitSharingCurER
                                    ,@Tah_FeesCurER
                                    ,@Tah_HazardCurER
                                    ,@Tah_SupplementaryTaxable1CurER
                                    ,@Tah_SupplementaryTaxable2CurER
                                    ,@Tah_RepresentationPrvER
                                    ,@Tah_TransportationPrvER
                                    ,@Tah_CostLivingAllowancePrvER
                                    ,@Tah_FixedHousingAllowancePrvER
                                    ,@Tah_OtherTaxable1PrvER
                                    ,@Tah_OtherTaxable2PrvER
                                    ,@Tah_CommisionPrvER
                                    ,@Tah_ProfitSharingPrvER
                                    ,@Tah_FeesPrvER
                                    ,@Tah_HazardPrvER
                                    ,@Tah_SupplementaryTaxable1PrvER
                                    ,@Tah_SupplementaryTaxable2PrvER
                                    ,@Tah_ExemptionCode
                                    ,@Tah_ExemptionAmount
                                    ,@Tah_PremiumPaidOnHealth
                                    ,@Tah_GrossCompensationIncome
                                    ,@Tah_NontaxableIncomeCurER
                                    ,@Tah_NontaxableIncomePrvER
                                    ,@Tah_TaxableIncomeCurER
                                    ,@Tah_TaxableIncomePrvER
                                    ,@Tah_NetTaxableIncome
                                    ,@Tah_TaxDue
                                    ,@Tah_TaxWithheldPrvER
                                    ,@Tah_TaxWithheldCurER
                                    ,@Tah_TotalTaxWithheld
                                    ,0
                                    ,@Tah_TaxAmount
                                    ,@Tah_TaxBracket
                                    ,@Tah_IsSubstitutedFiling
                                    ,@Tah_IsTaxExempted
                                    ,@Tah_FinalPayIndicator
                                    ,@Tah_IsZeroOutLastQuinYear
                                    ,@Tah_CostcenterCode
                                    ,@Tah_PayrollGroup
                                    ,@Tah_PayrollType
                                    ,@Tah_EmploymentStatus
                                    ,@Tah_WorkStatus
                                    ,@Tah_AssumeBasic
                                    ,@Tah_Assume13th
                                    ,@Tah_AssumeSalariesCompensation
                                    ,@Tah_AssumePremiumsUnionDues
                                    ,@Tah_AssumePayCycle
                                    ,@Tah_CurrentEmploymentStatus
                                    ,@Tah_Nationality
                                    ,@Tah_SeparationReason
                                    ,@Usr_Login
                                    ,GETDATE()) ";
                    #endregion

                    dal.ExecuteNonQuery(query, CommandType.Text, param);


                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully inserted record.");
                }
                catch(Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError("Error in saving: " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }


        public int InsertUpdateCTCEntry(ParameterInfo[] param, string CentralProfile)
        {
            int ret = 0;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    #region Query
                    string query = @"
                        IF NOT EXISTS (SELECT Tct_IDNo FROM T_EmpCommunityTax WHERE Tct_TaxYear = @Tct_TaxYear AND Tct_IDNo = @Tct_IDNo)
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
                                    ,Tct_IssuedAt = @Tct_IssuedAt
                                    ,Tct_IssuedDate = @Tct_IssuedDate
                                    ,Tct_IssuedBy = @Tct_IssuedBy
                                    ,Tct_PaidAmt = @Tct_PaidAmt
                                    ,Usr_Login = @Usr_Login
                                    ,Ludatetime = GETDATE()
                                 WHERE Tct_TaxYear = @Tct_TaxYear
                                     AND Tct_IDNo = @Tct_IDNo
                        ";
                    #endregion

                    ret = dal.ExecuteNonQuery(query, CommandType.Text, param);
                    dal.CommitTransaction();
                    //CommonProcedures.showMessageInformation("Successfully inserted record.");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    ret = 0;
                    //CommonProcedures.showMessageError("Error in saving: " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public void ModifyW2Entry(ParameterInfo[] param, string CentralProfile)
        {
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(@"
                                UPDATE T_EmpAlphalistHdr
                                SET Tah_EndDate = @Tah_EndDate
                                  ,Tah_LastName = @Tah_LastName
                                  ,Tah_FirstName = @Tah_FirstName
                                  ,Tah_MiddleName = @Tah_MiddleName
                                  ,Tah_ExtensionName = @Tah_ExtensionName
                                  ,Tah_Schedule = @Tah_Schedule
                                  ,Tah_TIN = @Tah_TIN
                                  ,Tah_PresCompleteAddress = @Tah_PresCompleteAddress
                                  ,Tah_PresAddressMunicipalityCity = @Tah_PresAddressMunicipalityCity
                                  ,Tah_TelephoneNo = @Tah_TelephoneNo
                                  ,Tah_ProvCompleteAddress = @Tah_ProvCompleteAddress
                                  ,Tah_ProvAddressMunicipalityCity = @Tah_ProvAddressMunicipalityCity
                                  ,Tah_WifeClaim = @Tah_WifeClaim
                                  ,Tah_MWEBasicCurER = @Tah_MWEBasicCurER
                                  ,Tah_MWEHolidayCurER = @Tah_MWEHolidayCurER
                                  ,Tah_MWEOvertimeCurER = @Tah_MWEOvertimeCurER
                                  ,Tah_MWENightShiftCurER = @Tah_MWENightShiftCurER
                                  ,Tah_MWEHazardCurER = @Tah_MWEHazardCurER
                                  ,Tah_Nontaxable13thMonthCurER = @Tah_Nontaxable13thMonthCurER
                                  ,Tah_DeMinimisCurER = @Tah_DeMinimisCurER
                                  ,Tah_PremiumsUnionDuesCurER = @Tah_PremiumsUnionDuesCurER
                                  ,Tah_NontaxableSalariesCompensationCurER = @Tah_NontaxableSalariesCompensationCurER
                                  ,Tah_TaxableBasicCurER = @Tah_TaxableBasicCurER
                                  ,Tah_TaxableBasicNetPremiumsCurER = @Tah_TaxableBasicNetPremiumsCurER
                                  ,Tah_TaxableOvertimeCurER = @Tah_TaxableOvertimeCurER
                                  ,Tah_Taxable13thMonthCurER = @Tah_Taxable13thMonthCurER
                                  ,Tah_TaxableSalariesCompensationCurER = @Tah_TaxableSalariesCompensationCurER
                                  ,Tah_MWEBasicPrvER = @Tah_MWEBasicPrvER
                                  ,Tah_MWEHolidayPrvER = @Tah_MWEHolidayPrvER
                                  ,Tah_MWEOvertimePrvER = @Tah_MWEOvertimePrvER
                                  ,Tah_MWENightShiftPrvER = @Tah_MWENightShiftPrvER
                                  ,Tah_MWEHazardPrvER = @Tah_MWEHazardPrvER
                                  ,Tah_Nontaxable13thMonthPrvER = @Tah_Nontaxable13thMonthPrvER
                                  ,Tah_DeMinimisPrvER = @Tah_DeMinimisPrvER
                                  ,Tah_PremiumsUnionDuesPrvER = @Tah_PremiumsUnionDuesPrvER
                                  ,Tah_NontaxableSalariesCompensationPrvER = @Tah_NontaxableSalariesCompensationPrvER
                                  ,Tah_TaxableBasicPrvER = @Tah_TaxableBasicPrvER
                                  ,Tah_TaxableBasicNetPremiumsPrvER = @Tah_TaxableBasicNetPremiumsPrvER
                                  ,Tah_TaxableOvertimePrvER = @Tah_TaxableOvertimePrvER
                                  ,Tah_Taxable13thMonthPrvER = @Tah_Taxable13thMonthPrvER
                                  ,Tah_TaxableSalariesCompensationPrvER = @Tah_TaxableSalariesCompensationPrvER
                                  ,Tah_RepresentationCurER = @Tah_RepresentationCurER
                                  ,Tah_TransportationCurER = @Tah_TransportationCurER
                                  ,Tah_CostLivingAllowanceCurER = @Tah_CostLivingAllowanceCurER
                                  ,Tah_FixedHousingAllowanceCurER = @Tah_FixedHousingAllowanceCurER
                                  ,Tah_OtherTaxable1CurER = @Tah_OtherTaxable1CurER
                                  ,Tah_OtherTaxable2CurER = @Tah_OtherTaxable2CurER
                                  ,Tah_CommisionCurER = @Tah_CommisionCurER
                                  ,Tah_ProfitSharingCurER = @Tah_ProfitSharingCurER
                                  ,Tah_FeesCurER = @Tah_FeesCurER
                                  ,Tah_HazardCurER = @Tah_HazardCurER
                                  ,Tah_SupplementaryTaxable1CurER = @Tah_SupplementaryTaxable1CurER
                                  ,Tah_SupplementaryTaxable2CurER = @Tah_SupplementaryTaxable2CurER
                                  ,Tah_RepresentationPrvER = @Tah_RepresentationPrvER
                                  ,Tah_TransportationPrvER = @Tah_TransportationPrvER
                                  ,Tah_CostLivingAllowancePrvER = @Tah_CostLivingAllowancePrvER
                                  ,Tah_FixedHousingAllowancePrvER = @Tah_FixedHousingAllowancePrvER
                                  ,Tah_OtherTaxable1PrvER = @Tah_OtherTaxable1PrvER
                                  ,Tah_OtherTaxable2PrvER = @Tah_OtherTaxable2PrvER
                                  ,Tah_CommisionPrvER = @Tah_CommisionPrvER
                                  ,Tah_ProfitSharingPrvER = @Tah_ProfitSharingPrvER
                                  ,Tah_FeesPrvER = @Tah_FeesPrvER
                                  ,Tah_HazardPrvER = @Tah_HazardPrvER
                                  ,Tah_SupplementaryTaxable1PrvER = @Tah_SupplementaryTaxable1PrvER
                                  ,Tah_SupplementaryTaxable2PrvER = @Tah_SupplementaryTaxable2PrvER
                                  ,Tah_ExemptionCode = @Tah_ExemptionCode
                                  ,Tah_ExemptionAmount = @Tah_ExemptionAmount
                                  ,Tah_PremiumPaidOnHealth = @Tah_PremiumPaidOnHealth
                                  ,Tah_GrossCompensationIncome = @Tah_GrossCompensationIncome
                                  ,Tah_NontaxableIncomeCurER = @Tah_NontaxableIncomeCurER
                                  ,Tah_NontaxableIncomePrvER = @Tah_NontaxableIncomePrvER
                                  ,Tah_TaxableIncomeCurER = @Tah_TaxableIncomeCurER
                                  ,Tah_TaxableIncomePrvER = @Tah_TaxableIncomePrvER
                                  ,Tah_NetTaxableIncome = @Tah_NetTaxableIncome
                                  ,Tah_TaxDue = @Tah_TaxDue
                                  ,Tah_TaxWithheldPrvER = @Tah_TaxWithheldPrvER
                                  ,Tah_TaxWithheldCurER = @Tah_TaxWithheldCurER
                                  ,Tah_TotalTaxWithheld = @Tah_TotalTaxWithheld
                                  ,Tah_TaxAmount = @Tah_TaxAmount
                                  ,Tah_TaxBracket = @Tah_TaxBracket
                                  ,Tah_IsSubstitutedFiling = @Tah_IsSubstitutedFiling
                                  ,Tah_IsTaxExempted = @Tah_IsTaxExempted
                                  ,Tah_IsZeroOutLastQuinYear = @Tah_IsZeroOutLastQuinYear
                                  ,Tah_AssumeBasic = @Tah_AssumeBasic
                                  ,Tah_Assume13th = @Tah_Assume13th
                                  ,Tah_AssumeSalariesCompensation = @Tah_AssumeSalariesCompensation
                                  ,Tah_AssumePremiumsUnionDues = @Tah_AssumePremiumsUnionDues
                                  ,Tah_AssumePayCycle = @Tah_AssumePayCycle
                                  ,Tah_CurrentEmploymentStatus =@Tah_CurrentEmploymentStatus
                                  ,Tah_Nationality = @Tah_Nationality
                                  ,Tah_SeparationReason = @Tah_SeparationReason
                                  ,Usr_Login = @Usr_Login
                                  ,Ludatetime = GETDATE()
                             WHERE Tah_IDNo = @Tah_IDNo
                                 AND Tah_TaxYear = @Tah_TaxYear
                                 AND Tah_StartDate = @Tah_StartDate
                    ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully updated record.");
                }
                catch(Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError("Error in saving: " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public void DeleteW2Entry(string EmployeeID, string Year, object StartPeriod, string CentralProfile)
        {
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        string.Format(@"
                            DECLARE @StartDate DATE = '{2}'

                            DELETE FROM T_EmpAlphalistDtl
                            WHERE Tad_IDNo = '{0}'
                            AND Tad_TaxYear = '{1}'

                            DELETE FROM T_EmpAlphalistHdr
                            WHERE Tah_IDNo = '{0}'
                                AND Tah_TaxYear = '{1}'
                                AND Tah_StartDate = @StartDate
    
                            DELETE FROM T_EmpAlphalistStaging
	                        WHERE Tas_IDNo = '{0}'
	                        AND Tas_PayCycle LIKE '{1}%'
                        ", EmployeeID, Year, StartPeriod));

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully deleted record.");
                }
                catch(Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError("Error in saving: " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public string GetTopCurrentPayCycle()
        {
            #region query
            string query = @"SELECT TOP 1 Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C'";
            #endregion
            DALHelper dal = new DALHelper();
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else return "";
        }

        public decimal GetTaxCodeExemptionTable(string TaxCode, string CompanyCode, string CentralProfile)
        {
            #region Query
            string strQuery = string.Format(@"  DECLARE @MaxTaxAnnualPayPeriod AS VARCHAR(7)
                                                DECLARE @DependentCount1 AS DECIMAL(8,2)
                                                DECLARE @DependentCount2 AS DECIMAL(8,2)
                                                DECLARE @DependentCount3 AS DECIMAL(8,2)
                                                DECLARE @DependentCount4 AS DECIMAL(8,2)

                                                SELECT @MaxTaxAnnualPayPeriod = Max(Myt_PayCycle)
                                                FROM M_YearlyTaxSchedule
                                                WHERE Myt_PayCycle <= '{1}'
                                                    AND Myt_RecordStatus = 'A'
                                                    AND Myt_CompanyCode = '{2}'

                                                SELECT @DependentCount1 = Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D1' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_RecordStatus = 'A' 
                                                    AND Mte_ExemptType = 'A'
                                                    AND Mte_CompanyCode = '{2}'

                                                SELECT @DependentCount2 = @DependentCount1 + Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D2' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_RecordStatus = 'A' 
                                                    AND Mte_ExemptType = 'A'
                                                    AND Mte_CompanyCode = '{2}'

                                                SELECT @DependentCount3 = @DependentCount2 + Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D3' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_RecordStatus = 'A' 
                                                    AND Mte_ExemptType = 'A'
                                                    AND Mte_CompanyCode = '{2}'

                                                SELECT @DependentCount4 = @DependentCount3 + Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D4' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_RecordStatus = 'A' 
                                                    AND Mte_ExemptType = 'A'
                                                    AND Mte_CompanyCode = '{2}'

                                                SELECT Mcd_Code AS [TaxCode]
	                                                , (SELECT M_TaxExemption.Mte_Amount + 
		                                                (SELECT CASE WHEN LEN(RTRIM(RIGHT(Mcd_Code,1 ))) = 0 THEN 
				                                                0 
		                                                ELSE  
			                                                CASE 
				                                                WHEN RIGHT(Mcd_Code, 1) = '1' THEN CONVERT(DECIMAL, @DependentCount1)
				                                                WHEN RIGHT(Mcd_Code, 1) = '2' THEN CONVERT(DECIMAL, @DependentCount2)
				                                                WHEN RIGHT(Mcd_Code, 1) = '3' THEN CONVERT(DECIMAL, @DependentCount3)
				                                                WHEN RIGHT(Mcd_Code, 1) = '4' THEN CONVERT(DECIMAL, @DependentCount4)
				                                                ELSE 0 END  
			                                                END )
		                                                FROM M_TaxExemption
		                                                WHERE M_TaxExemption.Mte_ExemptType = 'P'
                                                            AND M_TaxExemption.Mte_CompanyCode = '{2}'
			                                                AND M_TaxExemption.Mte_PayCycle = @MaxTaxAnnualPayPeriod
			                                                AND M_TaxExemption.Mte_TaxCode = RTRIM(ISNULL(LEFT(Mcd_Code,2),'S'))) AS [ExemptionAmount]
                                                FROM M_CodeDtl
                                                WHERE Mcd_CodeType = 'TAXCODE'
                                                    AND Mcd_Code = '{0}'
                                                    AND Mcd_CompanyCode = '{2}'", TaxCode, GetTopCurrentPayCycle(), CompanyCode);
            #endregion

            DALHelper dal = new DALHelper(CentralProfile, false);
            DataTable dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
            decimal dExemptionAmount = 0;

            if (dtResult.Rows.Count > 0)
                dExemptionAmount = Convert.ToDecimal(dtResult.Rows[0][1]);

            return dExemptionAmount;
        }

        public DataTable GetAnnualizedTaxAmount(decimal SalaryAmount, string TaxYear, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @MaxTaxAnnual as varchar(20)
                                            SET @MaxTaxAnnual = (
                                                SELECT MAX(Myt_PayCycle) FROM M_YearlyTaxSchedule
                                                                    WHERE Myt_PayCycle <= '{1}122'
                                                                    AND Myt_CompanyCode = '{2}'
                                            )
                                            
                                            DECLARE @TAXANNUALPERIOD AS VARCHAR(10)
                                            SET @TAXANNUALPERIOD = (SELECT Max(Myt_PayCycle) as [PayPeriod]
                                                                   FROM M_YearlyTaxSchedule
                                                                   WHERE Myt_PayCycle <= @MaxTaxAnnual
                                                                        AND Myt_RecordStatus = 'A'
                                                                        AND Myt_CompanyCode = '{2}')

                                            SELECT Myt_TaxOnCompensationLevel + (( {0:0.00} - Myt_ExcessOverCompensationLevel) * (Myt_TaxOnExcess/100))
                                            , Myt_BracketNo
                                            FROM M_YearlyTaxSchedule
                                            WHERE M_YearlyTaxSchedule.Myt_RecordStatus = 'A'
                                                    AND Myt_CompanyCode = '{2}'
                                                    AND M_YearlyTaxSchedule.Myt_PayCycle = @TAXANNUALPERIOD
                                                    AND {0:0.00} >= Myt_MinCompensationLevel and {0:0.00} <= Myt_MaxCompensationLevel"
                                                , SalaryAmount, TaxYear, CompanyCode);
            #endregion
            DALHelper dal = new DALHelper(CentralProfile, false);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        public DataTable GetAnnualizedTax(decimal SalaryAmount, string TaxYear, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @MaxTaxAnnual as varchar(20)
                                            SET @MaxTaxAnnual = (
                                                SELECT MAX(Myt_PayCycle) FROM M_YearlyTaxSchedule
                                                                    WHERE Myt_PayCycle <= '{1}122'
                                                                    AND Myt_CompanyCode = '{2}'
                                            )
                                            
                                            DECLARE @TAXANNUALPERIOD AS VARCHAR(10)
                                            SET @TAXANNUALPERIOD = (SELECT Max(Myt_PayCycle) as [PayPeriod]
                                                                   FROM M_YearlyTaxSchedule
                                                                   WHERE Myt_PayCycle <= @MaxTaxAnnual
                                                                        AND Myt_RecordStatus = 'A'
                                                                        AND Myt_CompanyCode = '{2}')

                                            SELECT Myt_TaxOnCompensationLevel + (( {0:0.00} - Myt_ExcessOverCompensationLevel) * (Myt_TaxOnExcess/100)) as [TaxAmount]
                                                , Myt_TaxOnCompensationLevel
                                                , Myt_ExcessOverCompensationLevel
                                                , Myt_TaxOnExcess
                                                , Myt_MinCompensationLevel
                                                , Myt_MaxCompensationLevel
                                                , Myt_BracketNo
                                            FROM M_YearlyTaxSchedule
                                            WHERE M_YearlyTaxSchedule.Myt_RecordStatus = 'A'
                                                    AND Myt_CompanyCode = '{2}'
                                                    AND M_YearlyTaxSchedule.Myt_PayCycle = @TAXANNUALPERIOD
                                                    AND {0:0.00} >= Myt_MinCompensationLevel and {0:0.00} <= Myt_MaxCompensationLevel"
                                                , SalaryAmount, TaxYear, CompanyCode);
            #endregion
            DALHelper dal = new DALHelper(CentralProfile, false);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void InsertBeneficiary(string TaxYear, DataRow row)
        {
            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, true))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    #region Query
                    string sqlQuery = string.Format(@"
                    DECLARE @Mef_IDNo VARCHAR(15) = '{0}'
                    DECLARE @DateCompare as datetime set @DateCompare = (SELECT Tps_EndCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')
                    DECLARE @SIZE_Mef_LineNo INT
                    DECLARE @SEQ_NO VARCHAR(10)
                    SET @SIZE_Mef_LineNo=(SELECT Character_Maximum_Length
					                    FROM   information_schema.columns
					                    WHERE  table_name = 'M_EmpFamily'
						                    AND Column_Name = 'Mef_LineNo')

                    SET @SEQ_NO=(SELECT CONVERT(VARCHAR,Replicate('0'
					                    ,@SIZE_Mef_LineNo-Len((MAX(Mef_LineNo)+1)))) 
				                    + CONVERT(VARCHAR,MAX(Mef_LineNo)+1)
				                    FROM M_EmpFamily
			                    WHERE Mef_IDNo=@Mef_IDNo)
                    if(@SEQ_NO IS NULL)
	                    SET @SEQ_NO = (SELECT CONVERT(VARCHAR,Replicate('0'
					                    ,@SIZE_Mef_LineNo-1)) 
				                    + CONVERT(VARCHAR,1))

                    IF EXISTS (SELECT * FROM M_EmpFamily 
			                    WHERE Mef_IDNo = @Mef_IDNo
				                    AND Mef_LastName = '{1}' 
				                    AND Mef_FirstName = '{2}' 
				                    AND Mef_MiddleName = '{3}') 
                    BEGIN
                        UPDATE M_EmpFamily 
                        SET Mef_BirthDate = '{4}'
	                            ,Mef_RelationToEmployee = '{5}'
	                            ,Mef_BIRDependent = 1
	                            ,Mef_Hierarchy = '{6}'
	                            ,Mef_Gender = '{7}'
	                            ,Mem_CivilStatusCode = '{8}'
	                            ,Mef_Age = CASE WHEN dateadd(year, datediff (year, '{4}', @DateCompare), '{4}') > @DateCompare
					                        THEN datediff (year, '{4}', @DateCompare) - 1
					                        ELSE datediff (year, '{4}', @DateCompare) END
	                            ,Usr_Login = '{9}'
	                            ,Ludatetime = GETDATE()
                        WHERE Mef_IDNo = @Mef_IDNo
	                        AND Mef_LastName = '{1}' 
	                        AND Mef_FirstName = '{2}' 
	                        AND Mef_MiddleName = '{3}'
                    END
                    ELSE
                    BEGIN
                        INSERT INTO M_EmpFamily
	                        (Mef_IDNo
	                        ,Mef_LineNo
	                        ,Mef_LastName
	                        ,Mef_FirstName
	                        ,Mef_MiddleName
	                        ,Mef_BirthDate
	                        ,Mef_RelationToEmployee
	                        ,Mef_BIRDependent
	                        ,Mef_DeceasedDate
	                        ,Mef_CancelledDate
	                        ,Mef_Hierarchy
	                        ,Mef_HMODependent
	                        ,Mef_InsuranceDependent
	                        ,Mef_AccidentInsuranceDependent
	                        ,Mef_Gender
	                        ,Mem_CivilStatusCode
	                        ,Mef_Age
	                        ,Mef_Occupation
	                        ,Mef_Company
	                        ,Mef_Remarks
	                        ,Mef_RecordStatus
	                        ,Usr_Login
	                        ,Ludatetime)
                        SELECT @Mef_IDNo
	                        ,@SEQ_NO
	                        ,'{1}'
	                        ,'{2}'
	                        ,'{3}'
	                        ,'{4}'
	                        ,'{5}'
	                        ,1
	                        ,NULL
	                        ,NULL
	                        ,'{6}'
	                        ,0
	                        ,0
	                        ,0
	                        ,'{7}'
	                        ,'{8}'
	                        ,CASE WHEN dateadd(year, datediff (year, '{4}', @DateCompare), '{4}') > @DateCompare
		                        THEN datediff (year, '{4}', @DateCompare) - 1
		                        ELSE datediff (year, '{4}', @DateCompare) END
	                        ,''
	                        ,''
	                        ,'FROM ALPHALIST REGISTER'
	                        ,'A'
	                        ,'{9}'
	                        ,GETDATE()
                    END

                    SET @SIZE_Mef_LineNo = 2 --Fixed

                    SET @SEQ_NO=(SELECT CONVERT(VARCHAR,Replicate('0'
					                    ,@SIZE_Mef_LineNo-Len((MAX(Tad_LineNo)+1)))) 
				                    + CONVERT(VARCHAR,MAX(Tad_LineNo)+1)
				                    FROM T_EmpAlphalistDtl
			                    WHERE Tad_IDNo=@Mef_IDNo
			                    AND Tad_TaxYear = '{10}')
                    if(@SEQ_NO IS NULL)
	                    SET @SEQ_NO = (SELECT CONVERT(VARCHAR,Replicate('0'
					                    ,@SIZE_Mef_LineNo-1)) 
				                    + CONVERT(VARCHAR,1))

                    INSERT INTO T_EmpAlphalistDtl
	                    (Tad_IDNo
	                    ,Tad_TaxYear
	                    ,Tad_LineNo
	                    ,Tad_LastName
	                    ,Tad_FirstName
	                    ,Tad_MiddleName
	                    ,Tad_BirthDate
	                    ,Usr_Login
	                    ,Ludatetime)
                    SELECT @Mef_IDNo
	                    ,'{10}'
	                    ,@SEQ_NO
	                    ,'{1}'
	                    ,'{2}'
	                    ,'{3}'
	                    ,'{4}'
	                    ,'{9}'
	                    ,GETDATE()", row["Mef_IDNo"]
                                   , row["Mef_LastName"]
                                   , row["Mef_FirstName"]
                                   , row["Mef_MiddleName"]
                                   , row["Mef_BirthDate"]
                                   , row["Mef_RelationToEmployee"]
                                   , row["Mef_Hierarchy"]
                                   , row["Mef_Gender"]
                                   , row["Mem_CivilStatusCode"]
                                   , row["Usr_Login"]
                                   , TaxYear);
                    #endregion

                    dal.ExecuteNonQuery(sqlQuery);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public int DeleteW2Detail(string EmployeeID, string Year, string SeqNo)
        {

            int value = 1;
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@Tad_IDNo", EmployeeID);
            param[1] = new ParameterInfo("@Tad_TaxYear", Year);
            param[2] = new ParameterInfo("@Tad_LineNo", SeqNo);

            string query = @"DELETE FROM T_EmpAlphalistDtl
                           WHERE Tad_IDNo = @Tad_IDNo AND Tad_TaxYear = @Tad_TaxYear AND Tad_LineNo = @Tad_LineNo";

            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, true))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    value = dal.ExecuteNonQuery(query, CommandType.Text, param);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return value;
        }
        
    }
}
