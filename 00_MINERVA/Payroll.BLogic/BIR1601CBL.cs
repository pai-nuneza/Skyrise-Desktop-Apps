using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class BIR1601CBL
    {

        public DataTable GetTaxMaster(string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Mtx_ReferPreviousIncomeTax
                                                , Mtx_TaxAnnualization
                                                , Mtx_TaxRule
                                                , Mtx_BIREmploymentStatusToProcess
	                                       FROM M_Tax
                                           WHERE  Mtx_RecordStatus = 'A'
                                                AND Mtx_CompanyCode = '{0}'", CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public string GetDetailQuery(string YearMonthCode, bool bCanViewRate, string BIREMPSTATTOPROCESS)
        {
            if (bCanViewRate)
                return string.Format(@"
                            SELECT @PROFILENAME as [Profile]
	                        , Tpy_IDNo as [ID Number]
                            , Mem_Lastname +  CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
                                    THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_FirstName as [Name]
	                        , Tpy_PayCycle AS [PayCycle]

	                        , Tpy_GrossIncomeAmt AS [Gross Income]
	                        , Tpy_BIRTotalAmountofCompensation - Tpy_GrossIncomeAmt AS [NOTINCLUDE Income]
	                        , Tpy_BIRTotalAmountofCompensation AS [TOTAL AMOUNT OF COMPENSATION]

	                        , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_REGAmt ELSE 0 END AS [Attended Days] 
	                        , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_PDLVAmt ELSE 0 END AS [Paid Leave] 
                            , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_SRGAdjAmt + Tpy_MRGAdjAmt + Tpy_SLVAdjAmt ELSE 0 END AS [Regular Adjustment]

	                        , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_BIRStatutoryMinimumWage - (Tpy_REGAmt + Tpy_PDLVAmt + Tpy_SRGAdjAmt + Tpy_MRGAdjAmt + Tpy_SLVAdjAmt) ELSE 0 END AS [BASICSAL Income] 
	                        , Tpy_BIRStatutoryMinimumWage AS [TOTAL STATUTORY MINIMUM WAGE FOR MWEs]

	                        , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_PDLEGHOLAmt   + Tpy_PDSPLHOLAmt   + Tpy_PDCOMPHOLAmt  + Tpy_PDOTHHOLAmt   + Tpy_PDRESTLEGHOLAmt ELSE 0 END AS [Holiday]
	                        , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_TotalOTNDAmt ELSE 0 END AS [Overtime and Night Premium] 
	                        , CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_SHOLAdjAmt   + Tpy_MHOLAdjAmt   + Tpy_SOTAdjAmt  + Tpy_MOTAdjAmt   + Tpy_SNDAdjAmt + Tpy_MNDAdjAmt ELSE 0 END AS [Holiday, Overtime and Night Premium Adjustment]
                            , Tpy_BIRHolidayOvertimeNightShiftHazard - (CASE WHEN Tpy_IsTaxExempted = 1 
                                                                        THEN 
					                                                        (Tpy_TotalOTNDAmt + Tpy_PDLEGHOLAmt   + Tpy_PDSPLHOLAmt   + Tpy_PDCOMPHOLAmt  + Tpy_PDOTHHOLAmt   + Tpy_PDRESTLEGHOLAmt
                                                                                + Tpy_SHOLAdjAmt   + Tpy_MHOLAdjAmt   + Tpy_SOTAdjAmt  + Tpy_MOTAdjAmt   + Tpy_SNDAdjAmt + Tpy_MNDAdjAmt )
			                                                            ELSE 0 END) 
                                                                        AS [HOLIDAYPY, NDPAY, OVERTIMEPY & HAZARDPY Income]
	                        , Tpy_BIRHolidayOvertimeNightShiftHazard AS [TOTAL HOLIDAY, OVERTIME, NIGHT SHIFT, HAZARD PAY]

	                        , Tpy_BIR13thMonthPayOtherBenefits AS [TOTAL 13TH MONTH PAY AND OTHER BENEFITS]

	                        , Tpy_BIRDeMinimisBenefits AS [TOTAL DE MINIMIS BENEFITS]

	                        , Tpy_SSSEE AS [SSS]
                            , Tpy_MPFEE AS [MPF]
	                        , Tpy_PhilhealthEE AS [PHIC]
	                        , Tpy_PagIbigEE AS [HDMF]
	                        , Tpy_UnionAmt AS [Union Dues]
	                        , Tpy_BIRSSSGSISPHICHDMFUnionDues - (Tpy_SSSEE+Tpy_MPFEE+Tpy_PagIbigEE+Tpy_PhilhealthEE+Tpy_UnionAmt) AS [PREMIUMS Income/Deduction] 
	                        , Tpy_BIRSSSGSISPHICHDMFUnionDues AS [TOTAL SSS, GSIS, PHIC, HDMF & UNION DUES]

	                        , CASE WHEN Tpy_WtaxAmt < 0 THEN (Tpy_WtaxAmt *-1) ELSE 0 END AS [Tax Refund]
	                        , Tpy_BIROtherNonTaxableCompensation - (CASE WHEN Tpy_WtaxAmt < 0 THEN Tpy_WtaxAmt*-1 ELSE 0 END) AS [N-SALOTH Income]
	                        , Tpy_BIROtherNonTaxableCompensation AS [TOTAL OTHER NONTAXABLE COMPENSATION]
		
	                        , Tpy_BIRTotalTaxableCompensation as [TOTAL TAXABLE COMPENSATION]
						
	                        , Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax AS [TAXABLE COMPENSATION NOT SUBJECT TO WITHHOLDING TAX]

	                        , CASE WHEN Tpy_WtaxAmt > 0  THEN Tpy_WtaxAmt ELSE 0 END AS [W/Tax]
	                        , Tpy_BIRTotalTaxesWithheld - (CASE WHEN Tpy_WtaxAmt > 0  THEN Tpy_WtaxAmt ELSE 0 END) AS [WTAX Income/Deduction]
	                        , Tpy_BIRTotalTaxesWithheld AS [TOTAL TAXES WITHHELD]

                            FROM @PROFILES..Udv_Payroll 
                            INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                            WHERE LEFT(Tpy_PayCycle,6) = '{0}'
                                AND Tpy_EmploymentStatus IN ({1}) ", YearMonthCode
                                                               , new CommonBL().EncodeFilterItems(BIREMPSTATTOPROCESS,true));
            else
                return string.Format(@"
                            SELECT @PROFILENAME AS [Profile] 
                            , Tpy_IDNo AS [ID Number]
                            , Mem_Lastname +  CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
                                    THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_FirstName AS [Name]
	                        , Tpy_PayCycle AS [PayCycle]

	                        , 0.00 AS [Gross Income]
	                        , 0.00 AS [NOTINCLUDE Income]
	                        , 0.00 AS [TOTAL AMOUNT OF COMPENSATION]

	                        , 0.00 AS [Attended Days] 
	                        , 0.00 AS [Paid Leave] 
                            , 0.00 AS [Regular Adjustment] 
	                        , 0.00 AS [BASICSAL Income] 
	                        , 0.00 AS [TOTAL STATUTORY MINIMUM WAGE FOR MWEs]

	                        , 0.00 AS [Holiday]
	                        , 0.00 AS [Overtime and Night Premium] 
                            , 0.00 AS [Holiday, Overtime and Night Premium Adjustment] 
	                        , 0.00 AS [HOLIDAYPY, NDPAY, OVERTIMEPY & HAZARDPY Income]
	                        , 0.00 AS [TOTAL HOLIDAY, OVERTIME, NIGHT SHIFT, HAZARD PAY]

	                        , 0.00 AS [TOTAL 13TH MONTH PAY AND OTHER BENEFITS]

	                        , 0.00 AS [TOTAL DE MINIMIS BENEFITS]

	                        , 0.00 AS [SSS]
                            , 0.00 AS [MPF]
	                        , 0.00 AS [PHIC]
	                        , 0.00 AS [HDMF]
	                        , 0.00 AS [Union Dues]
	                        , 0.00 AS [PREMIUMS Income/Deduction] 
	                        , 0.00 AS [TOTAL SSS, GSIS, PHIC, HDMF & UNION DUES]

	                        , 0.00 AS [Tax Refund]
	                        , 0.00 AS [N-SALOTH Income]
	                        , 0.00 AS [TOTAL OTHER NONTAXABLE COMPENSATION]
		
	                        , 0.00 AS [TOTAL TAXABLE COMPENSATION]
						
	                        , 0.00 AS [TAXABLE COMPENSATION NOT SUBJECT TO WITHHOLDING TAX]

	                        , 0.00 AS [W/Tax]
	                        , 0.00 AS [WTAX Income/Deduction]
	                        , 0.00 AS [TOTAL TAXES WITHHELD]
                        FROM @PROFILES..Udv_Payroll 
                        INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                        WHERE LEFT(Tpy_PayCycle,6) = '{0}'
                            AND Tpy_EmploymentStatus IN ({1}) ", YearMonthCode
                                                            , new CommonBL().EncodeFilterItems(BIREMPSTATTOPROCESS, true));

        }

        public string GetSummaryQuery(string YearMonthCode, bool bCanViewRate, string BIREMPSTATTOPROCESS)
        {
            if (bCanViewRate)
                return string.Format(@"SELECT @PROFILENAME AS [Profile]
	                                , Tpy_PayCycle AS [Pay Cycle]
	                                , SUM(Tpy_GrossIncomeAmt) AS [Gross Income]
	                                , SUM(Tpy_BIRTotalAmountofCompensation - Tpy_GrossIncomeAmt) AS [NOTINCLUDE Income]
	                                , SUM(Tpy_BIRTotalAmountofCompensation) AS [TOTAL AMOUNT OF COMPENSATION]

	                                , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_REGAmt ELSE 0 END) AS [Attended Days] 
	                                , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_PDLVAmt ELSE 0 END) AS [Paid Leave] 
                                    , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_SRGAdjAmt + Tpy_MRGAdjAmt + Tpy_SLVAdjAmt ELSE 0 END) AS [Regular Adjustment]
	                                , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_BIRStatutoryMinimumWage - (Tpy_REGAmt + Tpy_PDLVAmt + Tpy_SRGAdjAmt + Tpy_MRGAdjAmt + Tpy_SLVAdjAmt) ELSE 0 END) AS [BASICSAL Income] 
	                                , SUM(Tpy_BIRStatutoryMinimumWage) AS [TOTAL STATUTORY MINIMUM WAGE FOR MWEs]

	                                , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_PDLEGHOLAmt   + Tpy_PDSPLHOLAmt   + Tpy_PDCOMPHOLAmt  + Tpy_PDOTHHOLAmt   + Tpy_PDRESTLEGHOLAmt ELSE 0 END) AS [Holiday]
                                    , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_TotalOTNDAmt ELSE 0 END) AS [Overtime and Night Premium] 
                                    , SUM(CASE WHEN Tpy_IsTaxExempted = 1 THEN Tpy_SHOLAdjAmt   + Tpy_MHOLAdjAmt   + Tpy_SOTAdjAmt  + Tpy_MOTAdjAmt   + Tpy_SNDAdjAmt + Tpy_MNDAdjAmt ELSE 0 END) AS [Holiday, Overtime and Night Premium Adjustment]
	                                , SUM(Tpy_BIRHolidayOvertimeNightShiftHazard - (CASE WHEN Tpy_IsTaxExempted = 1 THEN 
					                                                                    (Tpy_TotalOTNDAmt + Tpy_PDLEGHOLAmt   + Tpy_PDSPLHOLAmt   + Tpy_PDCOMPHOLAmt  + Tpy_PDOTHHOLAmt   + Tpy_PDRESTLEGHOLAmt
			                                                                            + Tpy_SHOLAdjAmt   + Tpy_MHOLAdjAmt   + Tpy_SOTAdjAmt  + Tpy_MOTAdjAmt   + Tpy_SNDAdjAmt + Tpy_MNDAdjAmt )
                                                                                    ELSE 0 END)) AS [HOLIDAYPY, NDPAY, OVERTIMEPY & HAZARDPY Income]
	                                , SUM(Tpy_BIRHolidayOvertimeNightShiftHazard) AS [TOTAL HOLIDAY, OVERTIME, NIGHT SHIFT, HAZARD PAY]

	                                , SUM(Tpy_BIR13thMonthPayOtherBenefits) AS [TOTAL 13TH MONTH PAY AND OTHER BENEFITS]

	                                , SUM(Tpy_BIRDeMinimisBenefits) AS [TOTAL DE MINIMIS BENEFITS]

	                                , SUM(Tpy_SSSEE) AS [SSS]
                                    , SUM(Tpy_MPFEE) AS [MPF]
	                                , SUM(Tpy_PhilhealthEE) AS [PHIC]
	                                , SUM(Tpy_PagIbigEE) AS [HDMF]
	                                , SUM(Tpy_UnionAmt) AS [Union Dues]
	                                , SUM(Tpy_BIRSSSGSISPHICHDMFUnionDues - (Tpy_SSSEE+Tpy_MPFEE+Tpy_PagIbigEE+Tpy_PhilhealthEE+Tpy_UnionAmt)) AS [PREMIUMS Income/Deduction] 
	                                , SUM(Tpy_BIRSSSGSISPHICHDMFUnionDues) AS [TOTAL SSS, GSIS, PHIC, HDMF & UNION DUES]

	                                , SUM(CASE WHEN Tpy_WtaxAmt < 0 THEN (Tpy_WtaxAmt *-1) ELSE 0 END) AS [Tax Refund]
	                                , SUM(Tpy_BIROtherNonTaxableCompensation - (CASE WHEN Tpy_WtaxAmt < 0 THEN Tpy_WtaxAmt*-1 ELSE 0 END)) AS [N-SALOTH Income]
	                                , SUM(Tpy_BIROtherNonTaxableCompensation) AS [TOTAL OTHER NONTAXABLE COMPENSATION]
		
	                                , SUM(Tpy_BIRTotalTaxableCompensation) AS [TOTAL TAXABLE COMPENSATION]
						
	                                , SUM(Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax) AS [TAXABLE COMPENSATION NOT SUBJECT TO WITHHOLDING TAX]

	                                , SUM(CASE WHEN Tpy_WtaxAmt > 0  THEN Tpy_WtaxAmt ELSE 0 END) AS [W/Tax]
	                                , SUM(Tpy_BIRTotalTaxesWithheld - (CASE WHEN Tpy_WtaxAmt > 0  THEN Tpy_WtaxAmt ELSE 0 END)) AS [WTAX Income/Deduction]
	                                , SUM(Tpy_BIRTotalTaxesWithheld) AS [TOTAL TAXES WITHHELD]

                                FROM @PROFILES..Udv_Payroll 
                                WHERE LEFT(Tpy_PayCycle,6) = '{0}'
                                    AND Tpy_EmploymentStatus IN ({1})
                                GROUP BY Tpy_PayCycle
                                ", YearMonthCode
                                , new CommonBL().EncodeFilterItems(BIREMPSTATTOPROCESS, true));
            else
                return string.Format(@"SELECT @PROFILENAME as [Profile]
	                                , Tpy_PayCycle as [Pay Cycle]
	                                , 0.00 AS [Gross Income]
	                                , 0.00 AS [NOTINCLUDE Income]
	                                , 0.00 AS [TOTAL AMOUNT OF COMPENSATION]

	                                , 0.00 AS [Attended Days] 
	                                , 0.00 AS [Paid Leave] 
                                    , 0.00 AS [Regular Adjustment] 
	                                , 0.00 AS [BASICSAL Income] 
	                                , 0.00 AS [TOTAL STATUTORY MINIMUM WAGE FOR MWEs]

	                                , 0.00 AS [Holiday] 
	                                , 0.00 AS [Overtime and Night Premium] 
                                    , 0.00 AS [Holiday, Overtime and Night Premium Adjustment]
	                                , 0.00 AS [HOLIDAYPY, NDPAY, OVERTIMEPY & HAZARDPY Income]
	                                , 0.00 AS [TOTAL HOLIDAY, OVERTIME, NIGHT SHIFT, HAZARD PAY]

	                                , 0.00 AS [TOTAL 13TH MONTH PAY AND OTHER BENEFITS]

	                                , 0.00 AS [TOTAL DE MINIMIS BENEFITS]

	                                , 0.00 AS [SSS]
                                    , 0.00 AS [MPF]
	                                , 0.00 AS [PHIC]
	                                , 0.00 AS [HDMF]
	                                , 0.00 AS [Union Dues]
	                                , 0.00 AS [PREMIUMS Income/Deduction] 
	                                , 0.00 AS [TOTAL SSS, GSIS, PHIC, HDMF & UNION DUES]

	                                , 0.00 AS [Tax Refund]
	                                , 0.00 AS [N-SALOTH Income]
	                                , 0.00 AS [TOTAL OTHER NONTAXABLE COMPENSATION]
		
	                                , 0.00 AS [TOTAL TAXABLE COMPENSATION]
						
	                                , 0.00 AS [TAXABLE COMPENSATION NOT SUBJECT TO WITHHOLDING TAX]

	                                , 0.00 AS [W/Tax]
	                                , 0.00 AS [WTAX Income/Deduction]
	                                , 0.00 AS [TOTAL TAXES WITHHELD]
                                FROM @PROFILES..Udv_Payroll
                                WHERE LEFT(Tpy_PayCycle,6) = '{0}'
                                    AND Tpy_EmploymentStatus IN ({1})
                                GROUP BY Tpy_PayCycle
                                ", YearMonthCode
                                 , new CommonBL().EncodeFilterItems(BIREMPSTATTOPROCESS, true));
        }

        public string GetMonthlyQuery(string YearMonthCode, bool bCanViewRate, string BIREMPSTATTOPROCESS)
        {
            if (bCanViewRate)
                return string.Format(@"SELECT SUM(Tpy_BIRTotalAmountofCompensation) AS Tpy_BIRTotalAmountofCompensation
                                , SUM(Tpy_BIRStatutoryMinimumWage) AS Tpy_BIRStatutoryMinimumWage
                                , SUM(Tpy_BIRHolidayOvertimeNightShiftHazard) AS Tpy_BIRHolidayOvertimeNightShiftHazard
                                , SUM(Tpy_BIR13thMonthPayOtherBenefits) AS Tpy_BIR13thMonthPayOtherBenefits
                                , SUM(Tpy_BIRDeMinimisBenefits) AS Tpy_BIRDeMinimisBenefits
                                , SUM(Tpy_BIRSSSGSISPHICHDMFUnionDues) AS Tpy_BIRSSSGSISPHICHDMFUnionDues
                                , SUM(Tpy_BIROtherNonTaxableCompensation) AS Tpy_BIROtherNonTaxableCompensation
                                , SUM(Tpy_BIRTotalTaxableCompensation) AS Tpy_BIRTotalTaxableCompensation
                                , SUM(Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax) AS Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax
                                , SUM(Tpy_BIRTotalTaxesWithheld) AS Tpy_BIRTotalTaxesWithheld
                                FROM @PROFILES..Udv_Payroll
                                WHERE LEFT(Tpy_PayCycle,6) = '{0}'
                                    AND Tpy_EmploymentStatus IN ({1}) ", YearMonthCode
                                                                   , new CommonBL().EncodeFilterItems(BIREMPSTATTOPROCESS, true));
            else
                return string.Format(@"SELECT 0 AS Tpy_BIRTotalAmountofCompensation
                                , 0 AS Tpy_BIRStatutoryMinimumWage
                                , 0 AS Tpy_BIRHolidayOvertimeNightShiftHazard
                                , 0 AS Tpy_BIR13thMonthPayOtherBenefits
                                , 0 AS Tpy_BIRDeMinimisBenefits
                                , 0 AS Tpy_BIRSSSGSISPHICHDMFUnionDues
                                , 0 AS Tpy_BIROtherNonTaxableCompensation
                                , 0 AS Tpy_BIRTotalTaxableCompensation
                                , 0 AS Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax
                                , 0 AS Tpy_BIRTotalTaxesWithheld
                                FROM @PROFILES..Udv_Payroll
                                WHERE LEFT(Tpy_PayCycle,6) = '{0}'
                                    AND Tpy_EmploymentStatus IN ({1}) ", YearMonthCode
                                                                   , new CommonBL().EncodeFilterItems(BIREMPSTATTOPROCESS, true));
        }
        public void InsertUpdate(ParameterInfo[] param,string yearMonth)
        {
            string query = string.Format(@"IF NOT EXISTS (SELECT * FROM T_W2Monthly WHERE W2m_YearMonth = '{0}')
                            BEGIN
                            INSERT INTO T_W2Monthly
                            (W2m_YearMonth
                            ,W2m_TotalAmountOfCompensation
                            ,W2m_TotalAmountOfCompensationPayroll
                            ,W2m_TotalAmountOfCompensationAdj
                            ,W2m_StatutoryMinimumWage
                            ,W2m_StatutoryMinimumWagePayroll
                            ,W2m_StatutoryMinimumWageAdj
                            ,W2m_OtherIncomeMWE
                            ,W2m_OtherIncomeMWEPayroll
                            ,W2m_OtherIncomeMWEAdj
                            ,W2m_OtherNonTaxableCompensation
                            ,W2m_OtherNonTaxableCompensationPayroll
                            ,W2m_OtherNonTaxableCompensationAdj
                            ,W2m_TaxableCompensation
                            ,W2m_TaxableCompensationPayroll
                            ,W2m_TaxableCompensationAdj
                            ,W2m_TaxWithheld
                            ,W2m_TaxWithheldPayroll
                            ,W2m_TaxWithheldAdj
                            ,W2m_RemittanceDate
                            ,W2m_ReceivingOffice
                            ,W2m_RORNumber
                            ,W2m_PaymentMode
                            ,Usr_Login
                            ,Ludatetime
                            )
                        
                        VALUES
                            (@W2m_YearMonth
                            ,@W2m_TotalAmountOfCompensation
                            ,@W2m_TotalAmountOfCompensationPayroll
                            ,@W2m_TotalAmountOfCompensationAdj
                            ,@W2m_StatutoryMinimumWage
                            ,@W2m_StatutoryMinimumWagePayroll
                            ,@W2m_StatutoryMinimumWageAdj
                            ,@W2m_OtherIncomeMWE
                            ,@W2m_OtherIncomeMWEPayroll
                            ,@W2m_OtherIncomeMWEAdj
                            ,@W2m_OtherNonTaxableCompensation
                            ,@W2m_OtherNonTaxableCompensationPayroll
                            ,@W2m_OtherNonTaxableCompensationAdj
                            ,@W2m_TaxableCompensation
                            ,@W2m_TaxableCompensationPayroll
                            ,@W2m_TaxableCompensationAdj
                            ,@W2m_TaxWithheld
                            ,@W2m_TaxWithheldPayroll
                            ,@W2m_TaxWithheldAdj
                            ,@W2m_RemittanceDate
                            ,@W2m_ReceivingOffice
                            ,@W2m_RORNumber
                            ,@W2m_PaymentMode
                            ,@Usr_Login
                            ,GETDATE()
                            )
                        END
                        ELSE
                        BEGIN

                        UPDATE T_W2Monthly
                        SET W2m_TotalAmountOfCompensation = @W2m_TotalAmountOfCompensation
                            ,W2m_TotalAmountOfCompensationPayroll = @W2m_TotalAmountOfCompensationPayroll
                            ,W2m_TotalAmountOfCompensationAdj = @W2m_TotalAmountOfCompensationAdj
                            ,W2m_StatutoryMinimumWage = @W2m_StatutoryMinimumWage
                            ,W2m_StatutoryMinimumWagePayroll = @W2m_StatutoryMinimumWagePayroll
                            ,W2m_StatutoryMinimumWageAdj = @W2m_StatutoryMinimumWageAdj
                            ,W2m_OtherIncomeMWE = @W2m_OtherIncomeMWE
                            ,W2m_OtherIncomeMWEPayroll = @W2m_OtherIncomeMWEPayroll
                            ,W2m_OtherIncomeMWEAdj = @W2m_OtherIncomeMWEAdj
                            ,W2m_OtherNonTaxableCompensation = @W2m_OtherNonTaxableCompensation
                            ,W2m_OtherNonTaxableCompensationPayroll = @W2m_OtherNonTaxableCompensationPayroll
                            ,W2m_OtherNonTaxableCompensationAdj = @W2m_OtherNonTaxableCompensationAdj
                            ,W2m_TaxableCompensation = @W2m_TaxableCompensation
                            ,W2m_TaxableCompensationPayroll = @W2m_TaxableCompensationPayroll
                            ,W2m_TaxableCompensationAdj = @W2m_TaxableCompensationAdj
                            ,W2m_TaxWithheld = @W2m_TaxWithheld
                            ,W2m_TaxWithheldPayroll = @W2m_TaxWithheldPayroll
                            ,W2m_TaxWithheldAdj = @W2m_TaxWithheldAdj
                            ,W2m_RemittanceDate = @W2m_RemittanceDate
                            ,W2m_ReceivingOffice = @W2m_ReceivingOffice
                            ,W2m_RORNumber = @W2m_RORNumber
                            ,W2m_PaymentMode = @W2m_PaymentMode
                            ,Usr_Login = @Usr_Login
                            ,Ludatetime = GETDATE()
                            where W2m_YearMonth = '{0}'
                        END
                        ", yearMonth);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(query,CommandType.Text,param);
                dal.CloseDB();
            }
        }
        public DataTable FetchDataByYearMonth(string yearmonth)
        {
            DataTable dtResult = new DataTable();
            string query = string.Format(@"Select * from T_W2Monthly
            where W2m_YearMonth = '{0}'", yearmonth);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public static string Get1601CQuery(string YearMonthCode, string CompanyCode)
        {
            string query = string.Format(@"SELECT LEFT(Tmt_YearMonth,4) as [Year]
                            ,RIGHT(Tmt_YearMonth,2) as [Month]
                            , CASE WHEN Tmt_IsAmendedReturn = 1 THEN 'X' ELSE '' END as Tmt_IsAmendedReturnYes
							, CASE WHEN Tmt_IsAmendedReturn = 0 THEN 'X' ELSE '' END as Tmt_IsAmendedReturnNo
							, CASE WHEN Tmt_AnyTaxesWithheld = 1 THEN 'X' ELSE '' END as Tmt_AnyTaxesWithheldYes
							, CASE WHEN Tmt_AnyTaxesWithheld = 0 THEN 'X' ELSE '' END as Tmt_AnyTaxesWithheldNo
							,Tmt_NumberofSheetsAttached
							,LEFT(Mcm_TIN,3) AS TIN1
                            ,LEFT(RIGHT(Mcm_TIN,6),3) AS TIN2
							,RIGHT(Mcm_TIN,3) AS TIN3
							,CASE WHEN LEN(Mcm_TIN) = '12' THEN RIGHT(Mcm_TIN,3) + '0'
							    WHEN LEN(Mcm_TIN) ='13' THEN RIGHT(Mcm_TIN,4) 
							    ELSE ''
							    END AS TIN4
                            ,Mcm_TIN
							,Mcm_RDOCode
                            ,Mcm_CompanyName
							,Mcm_BusinessAddress
                            ,Mlh_ZipCode AS Mcm_MunicipalityCity
                            ,Mcm_TelNo

                            ,CASE WHEN Mcm_EmployerType = 'P' THEN 'X' ELSE '' END AS [Private]
							,CASE WHEN Mcm_EmployerType = 'G' THEN 'X' ELSE '' END AS [Government]
							,Mcm_EmailAddress

							,CASE WHEN Tmt_IsPayeesAvailingTaxRelief = 1 THEN 'X' ELSE '' END as Tmt_IsPayeesAvailingTaxReliefYes
							,CASE WHEN Tmt_IsPayeesAvailingTaxRelief = 0 THEN 'X' ELSE '' END as Tmt_IsPayeesAvailingTaxReliefNo
							,Tmt_TaxReliefSpecify
							,Tmt_TotalAmountofCompensation
							,Tmt_StatutoryMinimumWage
							,Tmt_HolidayOvertimeNightShiftHazard
							,Tmt_13thMonthPayOtherBenefits
							,Tmt_DeMinimisBenefits
							,Tmt_SSSGSISPHICHDMFUnionDues
							,Tmt_OtherNonTaxableCompensation
							,Tmt_OtherNonTaxableCompensationSpecify
							,Tmt_TotalNontaxableCompensation
							,Tmt_TotalTaxableCompensation
							,Tmt_TaxableCompensationNotSubjectToWithholdingTax
							,Tmt_NetTaxableCompensation
							,Tmt_TotalTaxesWithheld
							,Tmt_AdjustmentofTaxesWithheldFromPreviousMonths
							,Tmt_TaxesWithheldForRemittance
							,Tmt_TaxRemittedInReturnPreviouslyFiled
							,Tmt_OtherRemittancesMade
							,Tmt_OtherRemittancesMadeSpecify
							,Tmt_TotalTaxRemittancesMade
							,Tmt_TaxStillDueOver
							,Tmt_Surcharge
							,Tmt_Interest
							,Tmt_Compromise
							,Tmt_TotalPenalties
							,Tmt_TotalAmountStillDueOver
							,CASE WHEN Tmt_PaymentMode = 'C' THEN Tmt_DraweeBankAgency ELSE '' END As PayModeCashBank
							,CASE WHEN Tmt_PaymentMode = 'Q' THEN Tmt_DraweeBankAgency ELSE '' END As PayModeChequeBank
                            ,CASE WHEN Tmt_PaymentMode = 'O' THEN Tmt_DraweeBankAgency ELSE '' END As PayModeOthersBank	
							,Tmt_OthersSpecify

							,CASE WHEN Tmt_PaymentMode = 'C' THEN Tmt_RORNumber ELSE '' END As PayModeCashRORNumber 
							,CASE WHEN Tmt_PaymentMode = 'Q' THEN Tmt_RORNumber ELSE '' END As PayModeChequeRORNumber 
							,CASE WHEN Tmt_PaymentMode = 'T' THEN Tmt_RORNumber ELSE '' END As PayModeTaxDebitRORNumber		
                            ,CASE WHEN Tmt_PaymentMode = 'O' THEN Tmt_RORNumber ELSE '' END As PayModeOthersRORNumber	

							,CASE WHEN Tmt_PaymentMode = 'C' THEN CONVERT(varchar(10),Tmt_PaymentDate,101) ELSE '' END As PayModeCashDate	
							,CASE WHEN Tmt_PaymentMode = 'Q' THEN CONVERT(varchar(10),Tmt_PaymentDate,101) ELSE '' END As PayModeChequeDate	
							,CASE WHEN Tmt_PaymentMode = 'T' THEN CONVERT(varchar(10),Tmt_PaymentDate,101) ELSE '' END As PayModeTaxDebitDate	
                            ,CASE WHEN Tmt_PaymentMode = 'O' THEN CONVERT(varchar(10),Tmt_PaymentDate,101) ELSE '' END As PayModeOthersDate	
								
							,CASE WHEN Tmt_PaymentMode = 'C' THEN CAST(Tmt_TotalAmountStillDueOver AS INT) ELSE 0 END As PayModeCashAmt 
							,CASE WHEN Tmt_PaymentMode = 'Q' THEN CAST(Tmt_TotalAmountStillDueOver AS INT) ELSE 0 END As PayModeChequeAmt  
							,CASE WHEN Tmt_PaymentMode = 'T' THEN CAST(Tmt_TotalAmountStillDueOver AS INT) ELSE 0 END As PayModeTaxDebitAmt  
                            ,CASE WHEN Tmt_PaymentMode = 'O' THEN CAST(Tmt_TotalAmountStillDueOver AS INT) ELSE 0 END As PayModeOthersAmt 

							,CASE WHEN Tmt_PaymentMode = 'C' THEN 
															CASE WHEN CAST(Tmt_TotalAmountStillDueOver AS INT) = 0 THEN '' ELSE RIGHT(Tmt_TotalAmountStillDueOver, LEN(Tmt_TotalAmountStillDueOver)-CHARINDEX('.',Tmt_TotalAmountStillDueOver)) END
															ELSE '' END as PayModeCashAmtDecimal 
							,CASE WHEN Tmt_PaymentMode = 'Q' THEN 
															CASE WHEN CAST(Tmt_TotalAmountStillDueOver AS INT) = 0 THEN '' ELSE RIGHT(Tmt_TotalAmountStillDueOver, LEN(Tmt_TotalAmountStillDueOver)-CHARINDEX('.',Tmt_TotalAmountStillDueOver)) END
															ELSE '' END as PayModeChequeAmtDecimal	
							,CASE WHEN Tmt_PaymentMode = 'T' THEN 
															CASE WHEN CAST(Tmt_TotalAmountStillDueOver AS INT) = 0 THEN '' ELSE RIGHT(Tmt_TotalAmountStillDueOver, LEN(Tmt_TotalAmountStillDueOver)-CHARINDEX('.',Tmt_TotalAmountStillDueOver)) END
															ELSE '' END As PayModeTaxDebitAmtDecimal	
                            ,CASE WHEN Tmt_PaymentMode = 'O' THEN 
															CASE WHEN CAST(Tmt_TotalAmountStillDueOver AS INT) = 0 THEN '' ELSE RIGHT(Tmt_TotalAmountStillDueOver, LEN(Tmt_TotalAmountStillDueOver)-CHARINDEX('.',Tmt_TotalAmountStillDueOver)) END 
															ELSE '' END As PayModeOthersAmtDecimal	

							FROM Udv_MonthlyTax
                            LEFT JOIN M_Company
                            ON Tmt_CompanyCode = Mcm_CompanyCode
                            LEFT JOIN M_LocationHdr ON Mlh_CompanyCode = Mcm_CompanyCode
                                AND Mlh_LocationCode = Mcm_LocationCode
                            WHERE Tmt_YearMonth = '{0}'
							AND Tmt_CompanyCode = '{1}'", YearMonthCode, CompanyCode);
            return query;
        }

        public static string Get1601CQuery2(string YearMonthCode, string CompanyCode)
        {
            string query = string.Format(@"SELECT MAX([PrevMonth1]) as [PrevMonth1]
	                            , MAX([PrevMonth2]) as [PrevMonth2]
	                            , MAX([PrevMonth3]) as [PrevMonth3]
	                            , MAX([DatePaid1]) as [DatePaid1]
	                            , MAX([DatePaid2]) as [DatePaid2]
	                            , MAX([DatePaid3]) as [DatePaid3]
	                            , MAX([DraweeBankAgency1]) as [DraweeBankAgency1]
	                            , MAX([DraweeBankAgency2]) as [DraweeBankAgency2]
	                            , MAX([DraweeBankAgency3]) as [DraweeBankAgency3]
	                            , MAX([RORNumber1]) as [RORNumber1]
	                            , MAX([RORNumber2]) as [RORNumber2]
	                            , MAX([RORNumber3]) as [RORNumber3]
	                            , -MAX([TaxPaid1]) as [TaxPaid1]		
	                            ---, RIGHT(MAX([TaxPaid1]), LEN(MAX([TaxPaid1]))-CHARINDEX('.',MAX([TaxPaid1]))) as TaxPaid1Decimal
	                            , MAX([TaxPaid2]) as [TaxPaid2]		
	                            ---, RIGHT(MAX([TaxPaid2]), LEN(MAX([TaxPaid2]))-CHARINDEX('.',MAX([TaxPaid2]))) as TaxPaid2Decimal
	                            , MAX([TaxPaid3]) as [TaxPaid3]		
	                            ---, RIGHT(MAX([TaxPaid3]), LEN(MAX([TaxPaid3]))-CHARINDEX('.',MAX([TaxPaid3]))) as TaxPaid3Decimal
	                            , MAX([ShouldBeTaxDue1]) as [ShouldBeTaxDue1]
	                            ---, RIGHT(MAX([ShouldBeTaxDue1]), LEN(MAX([ShouldBeTaxDue1]))-CHARINDEX('.',MAX([ShouldBeTaxDue1]))) as ShouldBeTaxDue1Decimal
	                            , MAX([ShouldBeTaxDue2]) as [ShouldBeTaxDue2]
	                            ---, RIGHT(MAX([ShouldBeTaxDue2]), LEN(MAX([ShouldBeTaxDue2]))-CHARINDEX('.',MAX([ShouldBeTaxDue2]))) as ShouldBeTaxDue2Decimal
	                            , MAX([ShouldBeTaxDue3]) as [ShouldBeTaxDue3]
	                            ---, RIGHT(MAX([ShouldBeTaxDue3]), LEN(MAX([ShouldBeTaxDue3]))-CHARINDEX('.',MAX([ShouldBeTaxDue3]))) as ShouldBeTaxDue3Decimal
	                            , MAX([Adjustment1]) as [Adjustment1]
	                            ---, RIGHT(MAX([Adjustment1]), LEN(MAX([Adjustment1]))-CHARINDEX('.',MAX([Adjustment1]))) as Adjustment1Decimal
	                            , MAX([Adjustment2]) as [Adjustment2]
	                            ---, RIGHT(MAX([Adjustment2]), LEN(MAX([Adjustment2]))-CHARINDEX('.',MAX([Adjustment2]))) as Adjustment2Decimal
	                            , MAX([Adjustment3]) as [Adjustment3]
	                            ---, RIGHT(MAX([Adjustment3]), LEN(MAX([Adjustment3]))-CHARINDEX('.',MAX([Adjustment3]))) as Adjustment3Decimal
	                            , MAX(Tmt_AdjustmentofTaxesWithheldFromPreviousMonths) as Tmt_AdjustmentofTaxesWithheldFromPreviousMonths
	                            ---, RIGHT(Tmt_AdjustmentofTaxesWithheldFromPreviousMonths, LEN(Tmt_AdjustmentofTaxesWithheldFromPreviousMonths)-CHARINDEX('.',Tmt_AdjustmentofTaxesWithheldFromPreviousMonths)) as Tmt_AdjustmentofTaxesWithheldFromPreviousMonthsDecimal
	                            , Mcm_CompanyName
	                            , Mcm_TIN
                            FROM ( SELECT CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN Tta_PreviousMonth ELSE '' END as [PrevMonth1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN Tta_PreviousMonth ELSE '' END as [PrevMonth2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN Tta_PreviousMonth ELSE '' END as [PrevMonth3]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN CONVERT(VARCHAR(10), Tta_DatePaid,101) ELSE '' END as [DatePaid1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN CONVERT(VARCHAR(10), Tta_DatePaid,101) ELSE '' END as [DatePaid2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN CONVERT(VARCHAR(10), Tta_DatePaid,101) ELSE '' END as [DatePaid3]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN Tta_DraweeBankAgency ELSE '' END as [DraweeBankAgency1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN Tta_DraweeBankAgency ELSE '' END as [DraweeBankAgency2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN Tta_DraweeBankAgency ELSE '' END as [DraweeBankAgency3]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN Tta_RORNumber ELSE '' END as [RORNumber1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN Tta_RORNumber ELSE '' END as [RORNumber2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN Tta_RORNumber ELSE '' END as [RORNumber3]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN Tta_TaxPaid ELSE 0 END as [TaxPaid1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN Tta_TaxPaid ELSE 0 END as [TaxPaid2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN Tta_TaxPaid ELSE 0 END as [TaxPaid3]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN Tta_ShouldBeTaxDue ELSE 0 END as [ShouldBeTaxDue1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN Tta_ShouldBeTaxDue ELSE 0 END as [ShouldBeTaxDue2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN Tta_ShouldBeTaxDue ELSE 0 END as [ShouldBeTaxDue3]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 1 THEN Tta_Adjustment ELSE 0 END as [Adjustment1]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 2 THEN Tta_Adjustment ELSE 0 END as [Adjustment2]
		                            , CASE WHEN ROW_NUMBER() OVER (ORDER BY Tta_PreviousMonth) = 3 THEN Tta_Adjustment ELSE 0 END as [Adjustment3]
		                            , Tta_Companycode
		                            , Tta_Yearmonth
                            FROM Udv_MonthlyTaxAdj
                            WHERE Tta_Companycode = '{1}'
                            AND Tta_Yearmonth = '{0}') temp
                            INNER JOIN Udv_MonthlyTax 
                            ON Tta_Companycode = Tmt_CompanyCode
                            AND Tta_Yearmonth = Tmt_YearMonth
                            LEFT JOIN M_Company ON Mcm_CompanyCode = Tta_Companycode
                            GROUP BY Mcm_CompanyName, Mcm_TIN , Tmt_AdjustmentofTaxesWithheldFromPreviousMonths", YearMonthCode, CompanyCode);
            return query;
        }

        public DataTable GetTaxYear(bool bNew, string centralProfile, string companyCode)
        {
            DataTable dt = new DataTable();
            string condition = string.Empty;
            if (bNew)
                condition = "AND Tmt_YearMonth IS NULL";
            else
                condition = "AND Tmt_YearMonth IS NOT NULL";

            string sqlStatement = string.Empty;
            sqlStatement = string.Format(@"
                          SELECT DISTINCT LEFT(Tps_PayCycle, 4) + ' - ' 
                            + CASE WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '01' THEN 'JANUARY'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '02' THEN 'FEBRUARY'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '03' THEN 'MARCH'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '04' THEN 'APRIL'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '05' THEN 'MAY'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '06' THEN 'JUNE'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '07' THEN 'JULY'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '08' THEN 'AUGUST'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '09' THEN 'SEPTEMBER'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '10' THEN 'OCTOBER'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '11' THEN 'NOVEMBER'
					        WHEN RIGHT(LEFT(Tps_PayCycle, 6),2) = '12' THEN 'DECEMBER'
					        END AS [Tax Year - Month] 
                            , LEFT(Tps_PayCycle, 4) + RIGHT(LEFT(Tps_PayCycle, 6),2) AS [Year Month Code]
                            FROM T_PaySchedule
                            LEFT JOIN {0}..Udv_MonthlyTax
							ON LEFT(Tps_PayCycle, 6) = Tmt_YearMonth
								AND Tmt_CompanyCode = '{1}'
                            WHERE Tps_RecordStatus = 'A'
	                             AND Tps_MonthEnd = 1
								 AND Tps_CycleType = 'N'
                                 AND Tps_PayCycle = (SELECT MAX(Tps_PayCycle) FROM T_PaySchedule 
										            WHERE Tps_RecordStatus = 'A'
	                                                 AND Tps_MonthEnd = 1
								                     AND Tps_CycleType = 'N'
										             AND Tps_CycleIndicator = 'P')
                                {2}
                        ", centralProfile, companyCode, condition);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlStatement).Tables[0];
                dal.CloseDB();
            }
            return dt;
        }
    }
}
