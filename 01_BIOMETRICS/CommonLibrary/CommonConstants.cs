using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace CommonLibrary
{
    public class CommonConstants
    {
        #region tableName

        public class TableName
        {
            public const string T_W2 = "T_W2";
            public const string EmployeeRecurringAllowance = "T_EmployeeRecurringAllowance";
            public const string SpecialWorkHour = "T_SpecialWorkHour";
            public const string T_ServiceEmailRecipient = "T_ServiceEmailRecipient";
            public const string T_LogControl = "T_LogControl";
            public const string GenericMaster = "T_GenericMaster";
            public const string EmployeeLedger = "T_EmployeeLedger";
            public const string UserMaster = "T_UserMaster";
            public const string UserGroupHeader = "T_UserGroupHeader";
            public const string UserGroupDetail = "T_UserGroupDetail";
            public const string T_TaxExemption = "T_TaxExemption";
            public const string T_DayCodeMaster = "T_DayCodeMaster";
            public const string T_DeductionCodeMaster = "T_DeductionCodeMaster";
            public const string T_ParameterMaster = "T_ParameterMaster";
            public const string HolidayMaster = "T_HolidayMaster";
            public const string T_DayPremiumMaster = "T_DayPremiumMaster";
            public const string LeaveTypeMaster = "T_LeaveTypeMaster";
            public const string T_PayPeriodMaster = "T_PayPeriodMaster";
            public const string T_AllowanceCodeMaster = "T_AllowanceCodeMaster";
            public const string T_ShiftCodeMaster = "T_ShiftCodeMaster";
            public const string T_EmployeeMaster = "T_EmployeeMaster";
            public const string T_AnnualTaxSchedule = "T_AnnualTaxSchedule";
            public const string T_ContributionScheduleMaster = "T_PremiumContributionMaster";
            public const string T_TaxTableHeader = "T_TaxScheduleHeader";
            public const string T_TaxTableDetail = "T_TaxScheduleDetail";
            public const string T_EmployeeBeneficiary = "T_EmployeeBeneficiary";
            public const string EmployeeLeaveMaster = "T_EmployeeLeaveMaster";
            public const string T_MyMasterFile = "T_MyMasterFile";
            public const string T_AccountHeader = "T_AccountHeader";
            public const string T_AccountDetail = "T_AccountDetail";
            public const string UserGrant = "T_UserGrant";
            public const string DepartmentCodeMaster = "T_DepartmentCodeMaster";
            public const string CostCenterMaster = "T_CostCenter";
            public const string DivisionCodeMaster = "T_DivisionCodeMaster";
            public const string ProcessCodeMaster = "T_ProcessCodeMaster";
            public const string SectionCodeMaster = "T_SectionCodeMaster";
            public const string SubSectionCodeMaster = "T_SubSectionCodeMaster";
            // public const string T_DayCodeMaster = "T_DayCodeMaster";
            public const string T_CompanyMaster = "T_CompanyMaster";
            public const string T_PayrollPeriodMaster = "T_PayrollPeriodMaster";
            public const string T_EmployeeCostcenterMovement = "T_EmployeeCostcenterMovement";
            public const string T_EmployeeSalaryMovement = "T_EmployeeSalaryMovement";
            public const string T_EmployeePositionMovement = " T_EmployeePositionMovement";
            public const string T_AnnualTaxTable = "T_AnnualTaxTable";
            public const string T_DayPremiumRateMaster = "T_DayPremiumRateMaster";
            public const string T_AuditTrail = "T_AuditTrail";
            public const string T_PremiumRemittance = "T_PremiumRemittance";
            public const string T_EmployeeDeduction = "T_EmployeeDeduction";
            public const string T_LoanCollection = "T_LoanCollection";
            public const string T_AdvancePayments = "T_AdvancePayments";
            public const string T_EmployeeOffense = "T_EmployeeOffense";
            public const string T_EmployeeTraining = "T_EmployeeTraining";
            public const string T_EmployeeLeaveAvailment = "T_EmployeeLeaveAvailment";
            public const string T_EmployeeLeaveAvailmentHist = "T_EmployeeLeaveAvailmentHist";
            public const string T_EmployeeLeaveMaster = "T_EmployeeLeaveMaster";
            public const string T_EmployeeOvertime = "T_EmployeeOvertime";
            public const string T_EmployeeOvertimehist = "T_EmployeeOvertimehist";
            public const string T_EmployeeDeductionLedger = "T_EmployeeDeductionLedger";
            public const string T_EmployeeLeaveGrant = "T_EmployeeLeaveGrant";
            public const string T_EmployeeDeductionExemption = "T_EmployeeDeductionExemption";
            public const string T_EmployeeLogLedger = "T_EmployeeLogLedger";
            public const string T_EmployeeLogTrail = "T_EmployeeLogTrail";
            public const string T_EmployeeDeductionDetailHist = "T_EmployeeDeductionDetailHist";
            public const string T_EmployeePayrollTransaction = "T_EmployeePayrollTransaction";
            public const string T_EmployeePayrollCalc = "T_EmployeePayrollCalc";
            public const string T_EmployeePayrollCalcExt = "T_EmployeePayrollCalcExt";
            public const string T_EmployeePayrollCalcSep = "T_EmployeePayrollCalcSep";
            public const string T_EmployeePayrollCalcSepExt = "T_EmployeePayrollCalcSepExt";
            public const string T_PayrollError = "T_PayrollError";
            public const string T_LogMaster = "T_LogMaster";
            public const string T_DTR = "T_DTR";
            public const string T_DocumentSignatoryMaster = "T_DocumentSignatoryMaster";
            public const string T_UserGroupDetail = "T_UserGroupDetail";
            public const string T_UserCostCenterAccess = "T_UserCostCenterAccess";
            public const string T_SaturdayWorkStatusdetail = "T_SaturdayWorkStatusdetail";
            public const string T_SaturdayWorkStatusHeader = "T_SaturdayWorkStatusHeader";
            public const string T_YTDAdjustment = "T_YTDAdjustment";
            public const string T_EmployeeRestDay = "T_EmployeeRestDay";
            public const string T_EmployeeLogLedgerTrail = "T_EmployeeLogLedgerTrail";
            public const string T_EmployeeLogLedgerHist = "T_EmployeeLogLedgerHist";
            public const string T_ProcessControlMaster = "T_ProcessControlMaster";
            public const string T_Offset = "T_Offset";
            public const string T_SystemMenu = "T_SystemMenu";
            public const string T_Amortization = "T_Amortization";
            public const string T_EmployeeAllowance = "T_EmployeeAllowance";
            public const string T_EmployeeAdjustment = "T_EmployeeAdjustment";
            public const string T_DTRLedger = "T_DTRLedger";
            public const string T_StationMaster = "T_StationMaster";
            public const string T_SBRPayments = "T_SBRPayments";
            public const string T_EmployeeLeaveRefund = "T_EmployeeLeaveRefund";
            public const string T_EmployeeBonus = "T_EmployeeBonus";
            public const string T_EmployeeDeductionDeffered = "T_EmployeeDeductionDeffered";
            public const string T_EmployeeWorkLocation = "T_EmployeeWorkLocation";
            public const string EmployeeLeave = "T_EmployeeLeave";
            public const string T_SubWorkMaster = "T_SubWorkMaster";
            public const string T_SpecialDayMaster = "T_SpecialDayMaster";
            /* Add Start Employee Contract Management Toby/10-11-2010 */
            public const string T_EmployeeContractMaster = "T_EmployeeContractMaster";
            /* Add End Employee Contract Management Toby/10-11-2010 */
            /* Add Start Company Bank Account Master Toby/10-20-2010 */
            public const string T_CompanyBankMaster = "T_CompanyBankMaster";
            /* Add End Company Bank Account Master Toby/10-20-2010 */
            /* Add Start Employee Bank Account Master Toby/10-21-2010 */
            public const string T_EmployeeBankAccMaster = "T_EmployeeBankAccMaster";
            /* Add Start Employee Bank Account Master Toby/10-21-2010 */
            /* Add Start Document Type Tag Master Toby/10-26-2010 */
            public const string T_DocumentTypeTagMaster = "T_DocumentTypeTagMaster";
            /* Add Start Document Type Tag Master Toby/10-26-2010 */
            /* Add Start Document Tag Master Toby/10-27-2010 */
            public const string T_DocumentTagMaster = "T_DocumentTagMaster";
            /* Add Start Document Tag Master Toby/10-27-2010 */
            /* Add Start Document Type Master Toby/10-28-2010 */
            public const string T_DocumentTypeMaster = "T_DocumentTypeMaster";
            /* Add Start Document Type Master Toby/10-28-2010 */
            /* Add Start Overseas Control Master Toby/11-01-2010 */
            public const string T_OverSeasControlMaster = "T_OverSeasControlMaster";
            /* Add Start Overseas Control Master Toby/11-01-2010 */
            /* Add Start Overseas Control Position Master Toby/11-01-2010 */
            public const string T_OverseasControlPositionDetail = "T_OverseasControlPositionDetail";
            /* Add Start Overseas Control Position Master Toby/11-01-2010 */
            /* Add Start Overseas Control Allowance Master Toby/11-01-2010 */
            public const string T_OverseasControlAllowanceDetail = "T_OverseasControlAllowanceDetail";
            /* Add Start Overseas Control Allowance Master Toby/11-01-2010 */
            public const string T_MapPositionLevel = "T_MapPositionLevel";
            public const string T_MapJobPosition = "T_MapJobPosition";
            public const string T_SignatoryMaster = "T_SignatoryMaster";
            public const string T_DocumentSignatoryHeader = "T_DocumentSignatoryHeader";
            public const string T_DocumentSignatoryDetail = "T_DocumentSignatoryDetail";
            public const string T_DocumentSignatorySubDetail = "T_DocumentSignatorySubDetail";
            public const string T_FormulaMaster = "T_FormulaMaster";            
            public const string T_EmployeePayrollTransactionExt = "T_EmployeePayrollTransactionExt";
            public const string T_EmployeePayrollTransactionSep = "T_EmployeePayrollTransactionSep";
            public const string T_EmployeePayrollTransactionSepExt = "T_EmployeePayrollTransactionSepExt";
            public const string T_EmployeePayrollTransactionDetail = "T_EmployeePayrollTransactionDetail";
            public const string T_EmployeePayrollTransactionExtDetail = "T_EmployeePayrollTransactionExtDetail";
            public const string T_EmployeePayrollTransactionSepDetail = "T_EmployeePayrollTransactionSepDetail";
            public const string T_EmployeePayrollTransactionSepExtDetail = "T_EmployeePayrollTransactionSepExtDetail";
            public const string T_ForeignExchangeMaster = "T_ForeignExchangeMaster";
            public const string T_LaborHrsAdjustment = "T_LaborHrsAdjustment";
            public const string T_LaborHrsAdjustmentExt = "T_LaborHrsAdjustmentExt";
            public const string T_EmployeePayrollTransactionHist = "T_EmployeePayrollTransactionHist";
            public const string T_EmployeePayrollTransactionHistExt = "T_EmployeePayrollTransactionHistExt";
            public const string T_EmployeePayrollTransactionTrail = "T_EmployeePayrollTransactionTrail";
            public const string T_EmployeePayrollTransactionTrailExt = "T_EmployeePayrollTransactionTrailExt";
            public const string T_EmployeePayrollTransactionHistDetail = "T_EmployeePayrollTransactionHistDetail";
            public const string T_EmployeePayrollTransactionHistExtDetail = "T_EmployeePayrollTransactionHistExtDetail";
            public const string T_EmployeePayrollTransactionTrailDetail = "T_EmployeePayrollTransactionTrailDetail";
            public const string T_EmployeePayrollTransactionTrailExtDetail = "T_EmployeePayrollTransactionTrailExtDetail";

            
        }

        #endregion

        #region reports

        public class Reports
        {
        }

        #endregion

        #region queries

        public class Queries
        {

            #region SelectCostCenter

            public const string SelectCostCenter = @"SELECT  Cct_CostCenterCode as [CostCenter Code]
                                                          ,   CASE WHEN Len(Rtrim(IsNull(Dcm_DivisionDesc,' '))) > 0 THEN Rtrim(IsNull(Dcm_DivisionDesc,' '))
		                                                        ELSE ' ' END + 
		                                                        CASE WHEN Len(Rtrim(IsNull(Dcm_Departmentdesc,' '))) > 0 THEN ' / ' + Rtrim(IsNull(Dcm_Departmentdesc,' '))
		                                                        ELSE ' ' END + 
		                                                        CASE WHEN Len(Rtrim(IsNull(Scm_Sectiondesc,' '))) > 0 THEN ' / ' + Rtrim(IsNull(Scm_Sectiondesc,' '))
		                                                        ELSE ' ' END + 
		                                                        CASE WHEN Len(Rtrim(IsNull(Sscm_Sectiondesc,' '))) > 0 THEN ' / ' + Rtrim(IsNull(Sscm_Sectiondesc,' '))
		                                                        ELSE ' ' END  + 
		                                                        CASE WHEN Len(Rtrim(IsNull(Pcm_ProcessDesc,' '))) > 0 THEN ' / ' + Rtrim(IsNull(Pcm_ProcessDesc,' '))
		                                                        ELSE ' ' END  as [Description]
                                                        FROM  T_CostCenter 
                                                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                                                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                                                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                                                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                                                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                                                        WHERE Cct_CostCenterCode IN  (  SELECT Uca_CostCenterCode
								                                                        FROM T_UserCostCenterAccess
								                                                        WHERE Uca_SytemID = '{0}'
								                                                        and Uca_Usercode = '{1}'
								                                                        and Uca_Status = 'A'

								                                                        UNION

								                                                        SELECT Cct_CostCenterCode 
								                                                        FROM t_Costcenter
								                                                        WHERE Cct_status = 'A'
								                                                        and 1 = {2} )
                                                                 and  Cct_status = 'A'";

            #endregion

            #region SelectIDNumber
            public const string Selectidi = @"INSERT INTO T_EmployeeDeductionDetailHist
                                               (Edd_EmployeeID
                                               ,Edd_DeductionCode
                                               ,Edd_StartDeductionDate
                                               ,Edd_CurrentPayPeriod 
                                               ,Edd_PayPeriod
                                               ,Edd_SeqNo
                                               ,Edd_PaymentType
                                               ,Edd_Amount
                                               ,Edd_FromDeferred
                                               ,Edd_PaymentFlag
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Edd_EmployeeID
                                               ,@Edd_DeductionCode
                                               ,@Edd_StartDeductionDate
                                               ,@CurrentPayPeriod
                                               ,@Edd_PayPeriod
                                               ,@Edd_SeqNo
                                               ,@Edd_PaymentType
                                               ,@Edd_Amount
                                               ,@Edd_FromDeferred
                                               ,'1'
                                               ,@Usr_Login
                                               ,GetDate())";

            public const string SelectIDNumber = @"SELECT  Emt_EmployeeID as 'ID Number'
                , Emt_OldEmployeeID as [Old ID Number]
                , Emt_NickName as [Nick Name]
                , Emt_LastName as 'Last Name'
                , Emt_FirstName as 'First Name'
                , Left(Emt_MiddleName, 1) as MI
                , b.Adt_AccountDesc as [Job Status]
                , Emt_CostCenterCode as [CostCenter Code]
            FROM T_EmployeeMaster
            LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Emt_CostCenterCode
            LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
            LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
            LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
            LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
            LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
            LEFT JOIN t_accountdetail b on b.adt_accountcode = Emt_JobStatus
             and b.adt_accounttype='JOBSTATUS'
            WHERE --( Emt_ConfidentialPayroll  = '{0}' ) and
                 Emt_CostCenterCode IN  (  SELECT Uca_CostCenterCode
                                        FROM T_UserCostCenterAccess
                                        WHERE Uca_SytemID = '{1}'
                                        and Uca_Usercode = '{2}'
                                        and Uca_Status = 'A'

                                        UNION

                                        SELECT Cct_CostCenterCode 
                                        FROM t_Costcenter
                                        WHERE Cct_status = 'A'
                                        and  1 = {3} )";

            #endregion

            #region SelectPayrollPeriod
            public const string SelectPayrollPeriod = @"Select   Ppm_PayPeriod as [Payroll Period]
		                                                        ,Convert(char(10),Ppm_StartCycle,101) as [Start Date]
		                                                        ,Convert(char(10),Ppm_EndCycle,101) as [End Date] 
                                                        From T_PayPeriodMaster
                                                        Where Ppm_Status = 'A'";


            #endregion

            #region SelectEmployeeAll
            public const string SelectEmployeeAll = @"SELECT  Emt_EmployeeID AS [Employee ID],
                                                        Emt_LastName AS [Last Name],
                                                        Emt_FirstName AS [First Name],
                                                        Emt_MiddleName AS [Middle Name]
                                                    FROM T_EmployeeMaster ";


            #endregion

            #region selectEmplyeeID
            public const string selectEmplyeeID = @"SELECT DISTINCT emt_employeeID as 'ID no'
                                                    ,emt_Lastname as 'Last Name'
                                                    ,emt_Firstname as 'First Name'
                                                    ,emt_Middlename as 'Middle Name'
                                                    FROM T_EmployeeMaster 
                                                    INNER JOIN T_EMployeeBEneficiary on emt_employeeID=ebm_IDnumber";
            #endregion
            #region DeferredPaymentEntry
            public const string SelectDeferredPaymentEntry = @"
                                                                SELECT  
	                                                                T_EmployeeMaster.Emt_EmployeeID AS EmployeeID,
	                                                                T_EmployeeMaster.Emt_FirstName AS Firstname,
	                                                                T_EmployeeMaster.Emt_LastName AS Lastname,
	                                                                T_EmployeeMaster.Emt_MiddleName AS MiddleName,
	                                                                T_DeferredPayments.Dpt_DeductionCode AS DeductionCode,
	                                                                T_DeductionCodeMaster.Dcm_DeductionDesc AS DeductionDesc,
	                                                                T_DeferredPayments.Dpt_StartDeductionCode AS StartDeductionCode,
	                                                                T_EmployeeDeduction.Edt_DeductionAmount AS DeductionAmount,
	                                                                T_EmployeeDeduction.Edt_PaidAmount AS PaidAmount,
	                                                                T_DeferredPayments.Dpt_DeferredAmount AS DeferredAmount,
	                                                                T_EmployeeDeduction.Edt_AmortizationAmount AS AmortizationAmount
                                                                FROM 
	                                                                T_DeferredPayments, 
	                                                                T_EmployeeMaster,
	                                                                T_DeductionCodeMaster,
	                                                                T_EmployeeDeduction
                                                                WHERE 
	                                                                T_EmployeeMaster.Emt_EmployeeID = T_DeferredPayments.Dpt_EmployeeID 
                                                                AND
	                                                                T_DeferredPayments.Dpt_DeductionCode = T_DeductionCodeMaster.Dcm_DeductionCode
                                                                AND 
	                                                                T_EmployeeMaster.Emt_EmployeeID = T_EmployeeDeduction.Edt_EmployeeID
                                                              ";

            #endregion
            #region AccountHeaderFetch
            public const string AccountHeaderfetch = @"SELECT aht_accounttype as 'Account Type', aht_accountname as 'Description', CASE aht_status 
                                                        WHEN 'A' then 'ACTIVE'
                                                        WHEN 'C' then 'CANCELLED'
                                                        END as 'Status'
                                                        FROM T_accountheader
                                                        WHERE aht_status<>'C'";
            #endregion
            #region PositionComboBoxQuery
            //dave added 01/07/08
            public const string PositionComboBoxQuery = @"
                                                        SELECT '' AS POSITION_VAL, '' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'ADMIN' AS POSITION_VAL, 'ADMIN' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'ASST. MANAGER' AS POSITION_VAL, 'ASST. MANAGER' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'ENGINEER' AS POSITION_VAL, 'ENGINEER' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'FORMAN' AS POSITION_VAL, 'FORMAN' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'I.T. SYSTEMS ANALYST' AS POSITION_VAL, 'I.T. SYSTEMS ANALYST' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'LINE LEADER' AS POSITION_VAL, 'LINE LEADER' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'PRODUCTION OPERATOR' AS POSITION_VAL, 'PRODUCTION OPERATOR' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'SENIOR CLERK' AS POSITION_VAL, 'SENIOR CLERK' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'SENIOR WAREHOUSEMAN' AS POSITION_VAL, 'SENIOR WAREHOUSEMAN' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'SUPERVISOR' AS POSITION_VAL, 'SUPERVISOR' AS POSITION_DISP
                                                        UNION
                                                        SELECT 'WAREHOUSEMAN' AS POSITION_VAL, 'WAREHOUSEMAN' AS POSITION_DISP
                                                        ";
            #endregion
            #region selectUserGrantAll
            public const string selectUserGrantAll = @"SELECT     T_UserGrant.Ugt_Usergroup, 
	                                                               T_UserGrant.Ugt_SystemID,
                                                                   T_UserGrant.Ugt_sysmenucode, 
                                                                   T_UserGroupHeader.Ugh_UserGroupDesc,
                                                                   T_SystemMenu.Smc_MenuDesc, 
                                                                   T_UserGrant.Ugt_CanRetrieve,
                                                                   T_UserGrant.Ugt_CanAdd,
                                                                   T_UserGrant.Ugt_CanEdit,
                                                                   T_UserGrant.Ugt_CanDelete,
                                                                   T_UserGrant.Ugt_CanGenerate,
                                                                   T_UserGrant.Ugt_CanCheck,
                                                                   T_UserGrant.Ugt_CanApprove,
                                                                   T_UserGrant.Ugt_CanPrintPreview,
                                                                   T_UserGrant.Ugt_CanPrint,
                                                                   T_UserGrant.Ugt_CanReprint,
		                                                           T_SystemMenu.Smc_MenuType,
                                                                   T_UserGrant.Ugt_Status 
                                                            FROM         T_UserGrant 
                                                                      INNER JOIN T_UserGroupHeader 
                                                            ON (T_UserGrant.Ugt_Usergroup = T_UserGroupHeader.Ugh_UserGroupCode)
                                                                      INNER JOIN
                                                                      T_SystemMenu 
                                                            ON (T_UserGrant.Ugt_sysmenucode = T_SystemMenu.Smc_MenuCode)";
            #endregion
            #region selectUserGrantDuplicate
            public const string selectUserGrantDuplicate = @"SELECT     Ugt_Usergroup, Ugt_sysmenucode
                                                            FROM        T_UserGrant
                                                            WHERE       (Ugt_Usergroup = @userGroup) AND 
                                                                        (Ugt_sysmenucode = @menuCode)";
            #endregion
            #region fetchUserGrant
            public const string fetchUserGrant = @"select * FROM T_UserGrant";
            #endregion
            #region selectSubSectionMaster
            public const string selectSubSectionMaster = @"SELECT Sscm_Sectioncode, Sscm_Sectiondesc, Sscm_status 
                                                                FROM T_SubSectionCodeMaster ORDER BY ludatetime DESC";
            #endregion
            #region SelectSubSectionDesc
            public const string SelectSubSectionDesc = @"SELECT Sscm_SectionDesc FROM T_SubSectionCodeMaster WHERE Sscm_SectionCode = @Sscm_SectionCode";
            #endregion
            #region checkNickExist
            public const string checkNickExist = @"SELECT * FROM T_ProcessCodeMaster WHERE Pcm_ProcessDesc = @pcm_ProcessNickName";
            #endregion
            #region checkCostCenterCodeExist
            public const string checkCostCenterCodeExist = @"SELECT * FROM T_CostCenter WHERE Cct_CostCenterCode = @costcentercode";
            #endregion
            #region selectSubSectionMasterTypeList
            public const string selectSubSectionMasterTypeList = @"SELECT Sscm_Sectioncode, Sscm_Sectiondesc, Sscm_status 
                                                                FROM T_SubSectionCodeMaster WHERE Sscm_status = @status ORDER BY ludatetime DESC";
            #endregion
            #region Cost Center Look Up
            public const string CostCenterLookUp = @"Select dbo.getCostCenterName(Cct_CostCenterCode)as 'CCC' from T_CostCenter
                                                           WHERE Cct_DivisionCode='02' 
                                                                AND Cct_Departmentcode='09'
                                                                AND LEN(Cct_CostCenterCode)=10
								                                AND Cct_CostCenterCode=@CCC ";
            #endregion
            #region Cost Center Info
            //litlit 09192007
            public const string selectCodeExist = @"SELECT * FROM T_CostCenter 
                                                WHERE left(Cct_CostCenterCode, 8) = left(@CostCenterCode, 8) 
                                                AND Cct_ProcessSeqNo = @ProcessSeqNo";

            //end

            //reynard july 30, 2007
            public const string UpdateCCPInfo = @"UPDATE T_CostCenter SET Cct_ForQI=@ForQI,Cct_QIArea=@QIArea,Cct_ProcessSeqNo=@ProcessSeqNo,Cct_ProdLineCode=@ProdLineCode,Cct_ProdRouteNo=@ProdRouteNo WHERE Cct_CostCenterCode=@costcentercode";
            public const string SelectCCPInfo = @"Select Cct_CostCenterCode ,Scm_Sectiondesc ,Sscm_Sectiondesc ,Pcm_ProcessDesc ,Cct_ForQI,case when Cct_QIArea='P' Then 'PRODUCTION' when Cct_QIArea='W' then 'WAREHOUSE' else '' end Cct_QIArea ,Cct_ProcessSeqNo ,Cct_ProdLineCode,Cct_ProdRouteNo,case when Cct_status='A' then 'ACTIVE'else 'CANCELLED'end Cct_status 
	                                                        From T_CostCenter left outer join T_SectionCodeMaster on T_CostCenter.Cct_Sectioncode=T_SectionCodeMaster.Scm_Sectioncode
		                                                                      left outer join T_SubSectionCodeMaster on T_CostCenter.Cct_Subsectioncode=T_SubSectionCodeMaster.Sscm_Sectioncode
		                                                                      left outer join T_ProcessCodeMaster on T_CostCenter.Cct_Processcode=T_ProcessCodeMaster.Pcm_Processcode
		                                                    Where substring(Cct_CostCenterCode,1,2)='02' AND len(Cct_CostCenterCode) IN ('10', '6')
                                                            Group by Cct_CostCenterCode,Scm_Sectiondesc,Sscm_Sectiondesc,Pcm_ProcessDesc,Cct_ForQI,Cct_QIArea,Cct_ProcessSeqNo ,Cct_ProdLineCode,Cct_ProdRouteNo,Cct_status
                                                            Having len(Cct_CostCenterCode)>=6 and len(Cct_CostCenterCode)!>10 ";
            //end 
            #endregion
            #region SelectCCCodeLine
            public const string SelectCCCodeLine = @"Select Cct_ProdLineCode from T_CostCenter where Cct_ProdLineCode=@CCPLineCode";
            #endregion
            #region fetchAllowedCostCenters
            public const string fetchAllowedCostCenters = @"select 
	                                                            Cct_CostCenterCode,
	                                                            Cct_DivisionCode,
	                                                            Cct_Departmentcode,
	                                                            Cct_Sectioncode,
	                                                            Cct_Subsectioncode,
	                                                            Cct_Processcode
                                                            from t_costCenter {0}";
            #endregion
            #region SelectSupplierName
            public const string SelectSupplierName = @"SELECT Smt_Suppliername FROM T_SupplierMaster WHERE Smt_SupplierCode = @Scm_SupplierCode";
            #endregion
            #region SelectDivisionDesc
            public const string SelectDivisionDesc = @"SELECT Dcm_DivisionDesc FROM T_DivisionCodeMaster WHERE Dcm_DivisionCode = @Dcm_DivisionCode";
            #endregion
            #region SelectDepartmentDesc
            public const string SelectDepartmentDesc = @"SELECT Dcm_DepartmentDesc FROM T_DepartmentCodeMaster WHERE Dcm_DepartmentCode  = @Dcm_DepartmentCode";
            #endregion
            #region SelectSectionDesc
            public const string SelectSectionDesc = @"SELECT Scm_SectionDesc FROM T_SectionCodeMaster WHERE Scm_SectionCode = @Scm_SectionCode";
            #endregion
            #region SelectProcessDesc
            public const string SelectProcessDesc = @"SELECT Pcm_ProcessDesc FROM T_ProcessCodeMaster WHERE Pcm_ProcessCode = @Pcm_ProcessCode";
            #endregion
            #region checkProcessExist
            public const string checkProcessExist = @"SELECT * FROM T_ProcessCodeMaster WHERE Pcm_Processcode=@pcm_Processcode";
            #endregion
            #region checkSectionExist
            public const string checkSectionExist = @"SELECT * FROM T_SectionCodeMaster WHERE Scm_Sectioncode = @sectionCode";
            #endregion
            #region checkSubSectionExist
            public const string checkSubSectionExist = @"SELECT * FROM T_SubSectionCodeMaster WHERE Sscm_Sectioncode = @sectionCode";
            #endregion
            #region checkDivisionExist
            public const string checkDivisionExist = @"SELECT * FROM T_DivisionCodeMaster WHERE Dcm_DivisionCode = @divCode";
            #endregion
            #region checkDepartmentCodeExist
            public const string checkDepartmentCodeExist = @"SELECT * FROM T_DepartmentCodeMaster WHERE Dcm_Departmentcode = @code";
            #endregion
            #region selectCostCenterMasterTypeList
            public const string selectCostCenterMasterTypeList = @"SELECT
                                                                                      CC.Cct_CostCenterCode,
                                                                                      CC.Cct_DivisionCode,
                                                                                      CC.Cct_Departmentcode, 
                                                                                      CC.Cct_Sectioncode, 
                                                                                      CC.Cct_Subsectioncode, 
                                                                                      CC.Cct_Processcode, 
                                                                                                          
                                                                                      CC.Cct_status, 
                                                                                      CC.User_login, 
                                                                                      CC.ludatetime, 
                                                                                      ISNULL(T_AccountDetail.Adt_AccountDesc, ' ') AS ClassificationDesc, 
                                                                                                          
                                                                                      T_DivisionCodeMaster.Dcm_DivisionDesc, 
                                                                                      T_DepartmentCodeMaster.Dcm_Departmentdesc, 
                                                                                      T_SectionCodeMaster.Scm_Sectiondesc, 
                                                                                                          
                                                                                      T_SubSectionCodeMaster.Sscm_Sectiondesc, 
                                                                                      T_ProcessCodeMaster.Pcm_ProcessDesc
                                                                FROM         T_CostCenter AS CC LEFT OUTER JOIN
                                                                                      T_DivisionCodeMaster ON CC.Cct_DivisionCode = T_DivisionCodeMaster.Dcm_DivisionCode LEFT OUTER JOIN
                                                                                      T_DepartmentCodeMaster ON CC.Cct_Departmentcode = T_DepartmentCodeMaster.Dcm_Departmentcode LEFT OUTER JOIN
                                                                                      T_SectionCodeMaster ON CC.Cct_Sectioncode = T_SectionCodeMaster.Scm_Sectioncode LEFT OUTER JOIN
                                                                                      T_SubSectionCodeMaster ON CC.Cct_Subsectioncode = T_SubSectionCodeMaster.Sscm_Sectioncode LEFT OUTER JOIN
                                                                                      T_ProcessCodeMaster ON CC.Cct_Processcode = T_ProcessCodeMaster.Pcm_Processcode LEFT OUTER JOIN
                                                                                      T_AccountDetail ON CC.Cct_classification = T_AccountDetail.Adt_AccountCode AND Adt_AccountType = 'CCTRCLASS' ";
            #endregion
            #region
            public const string CalendarScope = @"  select 
                                                        '  ' as 'CalendarScopeValue'
                                                        ,'[COMPANY]' as 'CalendarScopeDisplay'

                                                    union

                                                    select distinct
	                                                    Cct_CostCenterCode as 'CalendarScopeValue'
	                                                    ,'['+RTRIM(Scm_Sectiondesc)+']'+' '+'['+RTRIM(Sscm_Sectiondesc)+']' as 'CalendarScopeDisplay'
                                                    from dbo.T_CostCenter
	                                                    join dbo.T_SectionCodeMaster
		                                                    on Scm_Sectioncode = Cct_Sectioncode
														join dbo.T_SubSectionCodeMaster
															on Sscm_Sectioncode = Cct_Subsectioncode
                                                    where Cct_DivisionCode = '02'
	                                                    and Cct_Departmentcode = '09'
	                                                    and LEN(Cct_CostCenterCode) = 8";
            #endregion
            #region checkGenericeExist
            public const string checkGenericeExist = @"SELECT * FROM T_GenericMaster WHERE Gmt_Genericcode = @gmt_Genericcode";
            #endregion
            #region SelectIfCostCenterSectionExist
            public const string SelectIfCostCenterSectionExist = @"SELECT DISTINCT Cct_CostCenterCode FROM T_CostCenter WHERE Cct_CostCenterCode = @Umt_UserCostCenter";
            #endregion
            #region SelectIfCostCenterExist
            public const string SelectIfCostCenterExist = @"SELECT DISTINCT Cct_CostCenterCode 
                                                            FROM T_CostCenter 
                                                            WHERE Cct_CostCenterCode = @Umt_UserCostCenter";
            #endregion
            #region SelectCostCenterDesc
            public const string SelectCostCenterDesc = @"SELECT DISTINCT dbo.getCostCenterName(Cct_CostCenterCode) as [Cost Center Desc] FROM T_CostCenter WHERE Cct_CostCenterCode = @Umt_UserCostCenter AND LEN(Cct_CostCenterCode) = 6";
            #endregion
            #region checkHumidity
            public const string checkHumidity = @"Select Count(*) 
                                                From T_RawMaterialsAndSupplies 
                                                Where 
                                                    Rst_Humidity In (Select Gmt_Genericcode From T_GenericMaster where Gmt_Genericcode = @genCode)";
            #endregion
            #region checkPilingHeight
            public const string checkPilingHeight = @"Select Count(*) 
                                                    From T_RawMaterialsAndSupplies 
                                                    Where 
	                                                    Rst_Pilingheight In (Select Gmt_Genericcode From T_GenericMaster where Gmt_Genericcode = @genCode) ";
            #endregion
            #region checkDayCodeMasterExist
            public const string checkDayCodeMasterExist = @"SELECT * FROM T_DayCodeMaster WHERE Dcm_DayCode = @DayCode";
            #endregion
            #region checkAllowanceCodeMasterExist
            public const string checkAllowanceCodeMasterExist = @"SELECT * FROM T_AllowanceCodeMaster WHERE Acm_AllowanceCode = @AllowanceCode";
            #endregion
            #region checkDeductionCodeMasterExist
            public const string checkDeductionCodeMasterExist = @"SELECT * FROM T_DeductionCodeMaster WHERE DeductionCode = @DeductionCode";
            #endregion
            #region checkMyMasterFileExist
            public const string checkMyMasterFileExist = @"SELECT * FROM T_MyMasterFile WHERE IDNum = @umt_IDNo";
            #endregion
            #region SelectUserMaster
            public const string SelectUserMaster = @"SELECT [Umt_Usercode] AS 'UserCode',
                                                            [Umt_userfname] AS 'Userfname',
                                                            [Umt_usermi] AS 'Usermi',
                                                            [Umt_userlname] AS 'Userlname',
                                                            [Umt_status] AS 'Status',
                                                            [Umt_Userpswd] AS 'Userpswd',
                                                            [Umt_UserCostCenter] AS 'UserCostCenter',
                                                            dbo.getCostCenterName(Umt_UserCostCenter) AS 'UserCostCenterDesc',
		                                                    [Umt_UserHandyPin] AS 'UserHandyPin',
                                                            [Umt_Usernumber] AS 'UserNumber',
                                                            [Umt_usersupv] AS 'Usersupv',
                                                            [Umt_usermnt] AS 'Usermnt',
                                                            [Umt_Position] AS 'Position',
                                                            [Umt_imagepath] AS 'ImagePath',
                                                            [Umt_Email] AS 'EmailAdd'
                                                    FROM T_UserMaster
                                                    ORDER BY [Umt_userlname], [Umt_userfname], [Umt_usermi], [Umt_Usercode]
                                                    ";
            #endregion
            #region SelectUser
            public const string SelectUser = @"SELECT [Umt_userlname] 
                                                                      ,[Umt_userfname] 
                                                                      ,[Umt_userview] 
                                                                      ,[Umt_userappend] 
                                                                      ,[Umt_useredit] 
                                                                      ,[Umt_userdelete] 
                                                                      ,[Umt_userprint] 
                                                                      ,[Umt_usersupv] 
                                                                      ,[Umt_usermnt] 
                                                                      ,[Umt_Usercode]
                                                                      ,[Umt_Userpswd]
                                                                      ,[Umt_Usernumber]      
                                                                      ,[Umt_usermi]     
                                                                      ,[Umt_status]                                                                      
                                                                FROM T_UserMaster
                                                                WHERE (Umt_Usercode = @userCode)";
            #endregion
            #region SelectIfUserCodeExist
            public const string SelectIfUserCodeExist = @"SELECT DISTINCT Umt_Usercode FROM T_UserMaster WHERE Umt_Usercode = @Umt_Usercode";
            #endregion
            #region SelectIfUserGroupCodeExist
            public const string SelectIfUserGroupCodeExist = @"SELECT DISTINCT Ugh_usergroupcode FROM T_UserGroupHeader WHERE Ugh_usergroupcode = @Ugh_usergroupcode";
            #endregion
            #region SelectIfGroupCodeExist
            public const string SelectIfGroupCodeExist = @"SELECT DISTINCT Ugt_groupcode FROM T_UserGroup WHERE Ugt_groupcode = @Ugt_groupcode AND Ugt_status <> 'C'";
            #endregion
            #region SelectIfControlNumberExist
            public const string SelectIfControlNumberExist = @"SELECT DISTINCT Umt_Usernumber FROM T_UserMaster WHERE Umt_Usernumber = @Umt_Usernumber";
            #endregion
            #region SelectGroupDesc
            public const string SelectGroupDesc = @"SELECT Ugt_groupdesc FROM T_UserGroup WHERE Ugt_groupcode = @Ugt_groupcode AND Ugt_status <> 'C'";
            #endregion
            #region SelectUserMenu
            public const string SelectUserMenu = @"SELECT 
	                                                    Ugt_sysmenucode
                                                      ,Ugt_CanRetrieve
                                                      ,Ugt_CanAdd
                                                      ,Ugt_CanEdit
                                                      ,Ugt_CanDelete
                                                      ,Ugt_CanGenerate
                                                      ,Ugt_CanCheck
                                                      ,Ugt_CanApprove
                                                      ,Ugt_CanPrintPreview
                                                      ,Ugt_CanPrint
                                                      ,Ugt_CanReprint
                                                    FROM T_UserGrant
                                                    INNER JOIN T_UserGroupDetail
                                                        ON Ugt_Usergroup = Ugd_usergroupcode
                                                        	And Ugt_SystemID = Ugd_SystemID
                                                            And Ugd_Status = 'A'
                                                    WHERE Ugd_usercode = @userCode
                                                    AND Ugt_Status = 'A'";
            #endregion
            #region selectCostCenterOfCurrentUser
            public const string selectCostCenterOfCurrentUser = @"SELECT Umt_UserCostCenter FROM T_UserMaster WHERE Umt_Usercode='{0}'";
            #endregion
            #region checkIdnumberExist
            public const string checkIdnumberExist = @"SELECT * FROM T_EmployeeLedger WHERE Idnumber = @Idnumber";
            #endregion
            #region SelectIfParameterIDExist
            public const string SelectIfParameterIDExist = @"SELECT DISTINCT Pmt_ParameterID FROM T_ParameterMaster WHERE Pmt_ParameterID = @ParameterID";
            #endregion
            #region checkMinBracketAmtExist
            public const string checkMinBracketAmtExist = @"SELECT * FROM T_AnnualTaxSchedule WHERE Ats_MinBracketAmt = @MinBracketAmt";
            #endregion
            #region selectDepartmentCodeMasterListStatus
            public const string selectDepartmentCodeMasterListStatus = @"SELECT Dcm_Departmentcode, Dcm_Departmentdesc, Dcm_DepartmentHead, Dcm_status  FROM T_DepartmentCodeMaster WHERE Dcm_status = @status";
            #endregion
            #region FetchCostCenterName
            public const string FetchCostCenterName = @"Select Dcm_DivisionDesc,
                                                            Dcm_Departmentdesc,
                                                            Scm_Sectiondesc,
                                                            Sscm_Sectiondesc, 
                                                            Pcm_ProcessDesc
                                                        FROM T_CostCenter
                                                        LEFT JOIN T_DivisionCodeMaster ON Cct_DivisionCode = Dcm_DivisionCode
                                                        LEFT JOIN T_DepartmentCodeMaster ON Cct_Departmentcode = Dcm_Departmentcode
                                                        LEFT JOIN T_SectionCodeMaster ON Cct_Sectioncode = Scm_Sectioncode
                                                        LEFT JOIN T_SubSectionCodeMaster ON Cct_Subsectioncode = Sscm_Sectioncode
                                                        LEFT JOIN T_ProcessCodeMaster ON Cct_Processcode = Pcm_Processcode
                                                        WHERE Cct_CostCenterCode = @Emt_CostCenterCode";
            #endregion
            #region selectFieldsAdvancePayments
            public const string selectFieldsAdvancePayments = @"SELECT	  Edt_EmployeeID                                                                        
                                                                        , Emt_FirstName
                                                                        , Emt_LastName
                                                                        , Emt_MiddleName                                                                        
                                                                        , Edt_DeductionCode	
                                                                        , Dcm_DeductionDesc                                                                    
                                                                        , Edt_StartDeductionDate
                                                                        , Edt_DeductionAmount
                                                                        , Edt_PaidAmount
                                                                        , Edt_DeferredAmount
                                                                        , Edt_AmortizationAmount                                                                   
                                                                        , Edt_Status					

                                                                FROM	T_EmployeeDeduction 

                                                                INNER JOIN	T_EmployeeMaster ON	Edt_EmployeeID = Emt_EmployeeID
                                                                INNER JOIN   T_DeductionCodeMaster ON Edt_DeductionCode = Dcm_DeductionCode   
                                                                WHERE 	Edt_DeferredAmount = 0
                                                                      AND   (Edt_DeductionAmount - Edt_PaidAmount) > 0";
            #endregion
            #region GetDeductionDescription
            public const string getDeductionDesc = @"SELECT Dcm_DeductionDesc
                                              FROM T_DeductionCodeMaster
                                              WHERE Dcm_DeductionCode=@Dcm_DeductionCode";
            #endregion
            #region GetEmployeeIDperCostCenterCode
            public const string getEmployeeIDperCostCenterCode = @"SELECT Emt_EmployeeID as [ID Number],
		                                                                    Emt_LastName as [Last Name],
		                                                                    Emt_FirstName as [First Name],
		                                                                    Emt_MiddleName as [MI]
		                                                            FROM T_EmployeeMaster
                                                                    WHERE Emt_CostCenterCode IN 
	                                                                    (	
		                                                                    SELECT Uca_CostCenterCode 
		                                                                    FROM T_UserCostCenterAccess 
		                                                                    WHERE Uca_Usercode = '{0}')";
            #endregion
            #region GetEmpIDIndividualLeave
            public const string GetEmpIDIndividualLeave = @"SELECT Elm_VLBalance,		
                                                                    Elm_SLBalance,
                                                                    Elm_ELBalance,
                                                                    Elm_PLBalance,
                                                                    Elm_BLBalance
                                                                    FROM T_EmployeeLeaveMaster
                                                            WHERE Elm_EmployeeId = '{0}'";
            #endregion
            #region GetEmployeeID
            public const string getEmployeeID = @"SELECT Emt_EmployeeID as [ID Number],
		                                                                    Emt_LastName as [Last Name],
		                                                                    Emt_FirstName as [First Name],
		                                                                    Emt_MiddleName as [MI]
		                                                            FROM T_EmployeeMaster
                                                                        WHERE LEFT(Emt_JobStatus, 1) = 'A'";


            #endregion
            #region GetCostCenterLookUp
            public const string GetCostCenterLookUp = @"SELECT DISTINCT Cct_CostCenterCode AS [Cost Center Code]
	                                                          ,Dcm_DivisionDesc AS [Division Description]
                                                              ,Dcm_Departmentdesc AS [Department Description]
                                                              ,Scm_Sectiondesc AS [Section Description]
                                                              ,Sscm_Sectiondesc AS [Sub-Section Description]
                                                              ,Pcm_ProcessDesc AS [Process Description]
	                                                        FROM T_CostCenter 
		                                                        LEFT JOIN T_DivisionCodeMaster ON Dcm_DivisionCode= Cct_DivisionCode 
		                                                        LEFT JOIN T_DepartmentCodeMaster ON Dcm_Departmentcode = Cct_Departmentcode
		                                                        LEFT JOIN T_SectionCodeMaster ON  Scm_Sectioncode = Cct_Sectioncode
		                                                        LEFT JOIN T_SubSectionCodeMaster  ON Sscm_Sectioncode = Cct_Subsectioncode 
		                                                        LEFT JOIN T_ProcessCodeMaster ON Pcm_Processcode = Cct_Processcode
                                                        ORDER BY Cct_CostCenterCode";


            #endregion
            #region DeductionCodeAmortLookUp
            public const string DeductionCodeAmortLookUp = @"SELECT Dcm_DeductionCode AS [Deduction Code]
                                                                    , Dcm_DeductionDesc AS [Deduction Description]
                                                                FROM T_DEDUCTIONCODEMASTER
                                                                    WHERE Dcm_AmortLookup = 1
                                                                        AND Dcm_Status = 'A'";
            #endregion


            #region GetRelation
            public const string GetRelation = @"Select 
                                    Adt_AccountCode, Adt_AccountDesc
                                    FROM T_AccountDetail
                                    WHERE Adt_AccountType = 'RELATION' AND Adt_AccountCode = @Relation AND Adt_Status = 'A'";
            #endregion

            #region checkSplDayCombinationExist
            public const string checkSplDayCombinationExist = @"SELECT *  
                                                                  FROM T_SpecialDayMaster
                                                                 WHERE Ard_Date = @Date
                                                                   and Ard_WorkType = @WorkType
                                                                   and Ard_WorkGroup = @WorkGroup";
            #endregion

            #region IfCutOff
            public const string IfCutOff = @"SELECT Pcm_ProcessFlag
                            FROM T_ProcessControlMaster
                            WHERE Pcm_SystemID = 'LEAVE' AND Pcm_ProcessID = 'CUT-OFF' AND Pcm_ProcessFlag = 'true'";
            #endregion

            #region [Jule Added 20081226 : Cycle Processing CommonQueries]
            #region [Count for Cycle Closing]
            public const string countProcess1 = @"SELECT IsNull(Count(Eal_EmployeeId), 0) NumEmp FROM T_EmployeeAllowanceHist";
            public const string countAdded1 = @"SELECT IsNull(Count(A.Eal_EmployeeId), 0) NumEmp 
                                                FROM T_EmployeeAllowance A
                                                Left Join T_EmployeeAllowanceHist B
                                                On A.Eal_EmployeeId = B.Eal_EmployeeId
                                                And A.Eal_CurrentPayPeriod = B.Eal_CurrentPayPeriod
                                                And A.Eal_AllowanceCode = B.Eal_AllowanceCode
                                                WHERE A.Eal_CurrentPayPeriod = @CurPayPeriod
                                                And B.Eal_CurrentPayPeriod Is Null";

            public const string countProcess2 = @"SELECT IsNull(COUNT(Ead_EmployeeId), 0) NumEmp FROM T_EmployeeAdjustmentHist";
            public const string countAdded2 = @"SELECT IsNull(COUNT(A.Ead_EmployeeId), 0) NumEmp 
                                                FROM T_EmployeeAdjustment A
                                                Left Join T_EmployeeAdjustmentHist B
                                                On A.Ead_EmployeeId = B.Ead_EmployeeId
                                                And A.Ead_CurrentPayPeriod = B.Ead_CurrentPayPeriod
                                                WHERE A.Ead_CurrentPayPeriod = @CurPayPeriod
                                                And B.Ead_CurrentPayPeriod Is Null";

            public const string countProcess3 = @"SELECT IsNull(Count(Edd_EmployeeID), 0) NumEmp, IsNull(Sum(Edd_Amount), 0) SumAmt FROM T_EmployeeDeductionDetailHist";
            public const string countAdded3 = @"SELECT IsNull(COUNT(A.Edd_EmployeeID), 0) NumEmp, 
		                                                IsNull(Sum(A.Edd_Amount), 0) SumAmt 
                                                FROM T_EmployeeDeductionDetail A
                                                Left Join T_EmployeeDeductionDetailHist B
                                                On A.Edd_EmployeeID = B.Edd_EmployeeID
                                                And A.Edd_DeductionCode = B.Edd_DeductionCode
                                                And A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                And A.Edd_CurrentPayPeriod = B.Edd_CurrentPayPeriod
                                                And A.Edd_PayPeriod = B.Edd_PayPeriod
                                                And A.Edd_SeqNo = B.Edd_SeqNo
                                                WHERE A.Edd_CurrentPayPeriod = @CurPayPeriod
                                                And B.Edd_CurrentPayPeriod Is Null";

            public const string countProcess4 = @"SELECT IsNull(COUNT(Epc_EmployeeID), 0) NumEmp, IsNull(SUM(Epc_NetpayAmt), 0) SumAmt FROM T_EmployeePayrollCalcAnnual";
            public const string countAdded4 = @"SELECT	IsNull(COUNT(A.Epc_EmployeeID), 0) NumEmp, 
		                                                IsNull(SUM(A.Epc_NetpayAmt), 0) SumAmt 
                                                FROM T_EmployeePayrollCalc A
                                                Left Join T_EmployeePayrollCalcAnnual B
                                                On A.Epc_EmployeeId = B.Epc_EmployeeId
                                                And A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                WHERE A.Epc_CurrentPayPeriod = @CurPayPeriod 
                                                And B.Epc_CurrentPayPeriod Is Null";

            public const string countProcess5 = @"SELECT IsNull(count(Per_EmployeeId), 0) NumEmp FROM  T_PayrollErrorHist";
            public const string countAdded5 = @"SELECT IsNull(count(A.Per_EmployeeId), 0) NumEmp 
                                                FROM T_PayrollError A
                                                Left Join T_PayrollErrorHist B
                                                On A.Per_EmployeeId = B.Per_EmployeeId
                                                And A.Per_Remarks = B.Per_Remarks
                                                And A.Per_CurrentPayPeriod = B.Per_CurrentPayPeriod
                                                WHERE A.Per_CurrentPayPeriod = @CURPAYPERIOD
                                                And B.Per_CurrentPayPeriod Is Null";

            public const string countProcess6 = @"SELECT IsNull(SUM(Edl_PaidAmount), 0) PaidAmt, IsNull(SUM(Edl_DeferredAmount), 0) DefAmt FROM T_EmployeeDeductionLedger";
            public const string countAdded6 = @"SELECT  PaidAmt = IsNull(Sum(Edd_Amount), 0) , DeferredAmt = IsNull(Sum(Case When Edd_FromDeferred = 1  Then Edd_Amount Else 0 End), 0) FROM T_EmployeeDeductionDetail WHERE Edd_CurrentPayPeriod = @CurPayPeriod and  Edd_PaymentFlag = 1";

            public const string countProcess7 = @"SELECT IsNull(count(Edl_EmployeeID), 0) NumEmp, IsNull(SUM(Edl_PaidAmount), 0) SumAmt FROM T_EmployeeDeductionLedgerHist";
            public const string countAdded7 = @"SELECT IsNull(count(A.Edl_EmployeeID), 0) NumEmp
	                                                ,IsNull(SUM(A.Edl_PaidAmount), 0) SumAmt 
                                                FROM T_EmployeeDeductionLedger A
                                                Left Join T_EmployeeDeductionLedgerHist B
                                                On A.Edl_EmployeeID = B.Edl_EmployeeID
                                                And A.Edl_DeductionCode = B.Edl_DeductionCode
                                                And A.Edl_StartDeductionDate = B.Edl_StartDeductionDate
                                                WHERE B.Ludatetime Is Null
                                                and A.Edl_DeductionAmount = A.Edl_PaidAmount
                                                And A.Edl_FullyPaidDate is not null";

            public const string countProcess8 = @"SELECT IsNull(Count(Edd_EmployeeID), 0) NumEmp, IsNull(Sum(Edd_Amount), 0) SumAmt FROM T_EmployeeDeductionDetailHistFP";
            public const string countAdded8 = @"SELECT IsNull(COUNT(A.Edd_EmployeeID), 0) NumEmp, IsNull(Sum(A.Edd_Amount), 0) SumAmt
                                                From T_EmployeeDeductionDetailHist A
                                                Inner join T_EmployeeDeductionLedger on Edl_EmployeeID = A.Edd_EmployeeID
                                                and Edl_DeductionCode = A.Edd_DeductionCode
                                                and Edl_StartDeductionDate = A.Edd_StartDeductionDate
                                                and Edl_FullyPaidDate is not null
                                                and Edl_DeductionAmount = Edl_PaidAmount
                                                Left Join T_EmployeeDeductionDetailHistFP B
                                                On A.Edd_EmployeeID = B.Edd_EmployeeID
                                                And A.Edd_DeductionCode = B.Edd_DeductionCode
                                                And A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                And A.Edd_CurrentPayPeriod = B.Edd_CurrentPayPeriod
                                                And A.Edd_PayPeriod = B.Edd_PayPeriod
                                                And A.Edd_SeqNo = B.Edd_SeqNo
                                                Where B.Ludatetime Is Null";

            public const string countProcess9 = @"SELECT IsNull(COUNT(Edd_EmployeeID), 0) NumEmp, IsNull(SUM(Edd_DeferredAmount), 0) SumAmt FROM T_EmployeeDeductionDeffered";
            public const string countAdded9 = @"SELECT IsNull(COUNT(A.Edd_EmployeeID), 0) NumEmp, IsNull(Sum(A.Edd_Amount), 0) SumAmt
                                                FROM T_EmployeeDeductionDetail A
                                                Left Join T_EmployeeDeductionDeffered B
                                                On A.Edd_EmployeeID = B.Edd_EmployeeID
                                                And A.Edd_DeductionCode = B.Edd_DeductionCode
                                                And A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                And A.Edd_PayPeriod = B.Edd_PayPeriod
                                                And A.Edd_SeqNo = B.Edd_SeqNo
                                                WHERE A.Edd_CurrentPayPeriod = @CurPayPeriod
                                                    and A.Edd_PaymentFlag = 0
                                                    and A.Edd_FromDeferred = 0
                                                    And B.Ludatetime Is Null";

            public const string countProcess10 = @"SELECT IsNull(SUM(Edl_DeferredAmount ), 0) SumAmt FROM T_EmployeeDeductionLedger";
            public const string countAdded10 = @"SELECT ISNULL(Sum(Edd_Amount),0) DeferredAmt
                                                        FROM T_EmployeeDeductionDetail                            
                                                        WHERE Edd_CurrentPayPeriod = @CurPayPeriod
	                                                        and Edd_PaymentFlag = 0
	                                                        and Edd_FromDeferred = 0";

            public const string countProcess11 = @"SELECT IsNull(Count(Ell_EmployeeID), 0) NumEmp FROM T_EmployeeLogLedgerHist";
            public const string countAdded11 = @"SELECT IsNull(Count(A.Ell_EmployeeID), 0) NumEmp 
                                                FROM T_EmployeeLogLedger A
                                                Left Join T_EmployeeLogLedgerHist B
                                                On A.Ell_EmployeeId = B.Ell_EmployeeId
                                                And A.Ell_ProcessDate = B.Ell_ProcessDate
                                                WHERE A.Ell_PayPeriod = @CurPayPeriod
                                                And B.Ell_PayPeriod Is Null";

            public const string countProcess12 = @"SELECT IsNull(Count(Off_EmployeeId), 0) NumEmp FROM T_OffSet WHERE Off_Served = 1 and Off_Type = 'U'";
            public const string countAdded12 = @"SELECT IsNull(Count(Off_EmployeeId), 0) NumEmp
	                                            FROM T_OffSet
	                                            Inner Join T_EmployeeLogLedger On Ell_EmployeeId = Off_EmployeeId
	                                            and Ell_ProcessDate = Off_DateWork
	                                            Where Off_Served = 0
	                                            and Off_Type = 'U'
	                                            and Ell_ComputedDayWorkMin = Ell_ForOffsetMin";

            public const string countProcess13 = @"SELECT IsNull(COUNT(Off_EmployeeId), 0) NumEmp FROM T_Offset";
            public const string countAdded13 = @"SELECT IsNull(COUNT(Ell_EmployeeId), 0) NumEmp
		                                        FROM T_EmployeeLogLedger
		                                        WHERE Ell_PayPeriod = @CURPAYPERIOD
		                                        and Ell_DayCode = 'SOFF'
		                                        and Ell_EarnedSatOff > 0";

            public const string countProcess14 = @"SELECT IsNull(COUNT(Off_EmployeeId), 0) NumEmp FROM T_Offset";
            public const string countAdded14 = @"SELECT IsNull(COUNT(Ell_EmployeeId), 0) NumEmp
		                                        FROM T_EmployeeLogLedger
		                                        WHERE Ell_PayPeriod = @CURPAYPERIOD
		                                        and Ell_excessoffset > 0";
            /// Not Sure
            public const string countProcess15 = @"SELECT IsNull(Count(Emt_EmployeeID), 0) NumEmp
                                                From T_EmployeeMaster
                                                Inner Join T_EmployeeLogLedger on T_EmployeeLogLedger.Ell_EmployeeID = T_EmployeeMaster.Emt_EmployeeID  
                                                Inner Join ( Select Ell_EmployeeID
		                                                , RegDate =Max(Ell_ProcessDate)
			                                                From T_EmployeeLogLedger
			                                                where Ell_PayPeriod = @CURPAYPERIOD
				                                                and Ell_DayCode = 'REG'
		                                                Group by Ell_EmployeeID) EmpShift on EmpShift.Ell_EmployeeID = T_EmployeeLogLedger.Ell_EmployeeID
					                                                and EmpShift.RegDate = T_EmployeeLogLedger.Ell_ProcessDate";


            public const string countProcess16 = @"SELECT IsNull(count(Lhe_EmployeeId), 0) NumEmp FROM T_LaborHourErrorHist";
            public const string countAdded16 = @"SELECT IsNull(count(A.Lhe_EmployeeId), 0) NumEmp 
                                                FROM T_LaborHourError A
                                                Left Join dbo.T_LaborHourErrorHist B
                                                On A.Lhe_EmployeeId = B.Lhe_EmployeeId
                                                And A.Lhe_CurrentPayperiod = B.Lhe_CurrentPayperiod
                                                WHERE A.Lhe_CurrentPayperiod = @CURPAYPERIOD
                                                And B.Lhe_CurrentPayperiod Is Null";

            public const string countProcess17 = @"SELECT IsNull(COUNT(Elt_EmployeeId), 0) NumEmp FROM T_EmployeeLeaveAvailmentHist";
            public const string countAdded17 = @"SELECT IsNull(COUNT(B.Elt_EmployeeId), 0) NumEmp 
                                                FROM  T_EmployeeLeaveAvailment B
                                                left Join T_EmployeeLeaveAvailmentHist C
                                                ON	B.Elt_EmployeeId = C.Elt_EmployeeId
                                                AND	B.Elt_LeaveDate = C.Elt_LeaveDate
                                                AND	B.Elt_LeaveType = C.Elt_LeaveType
                                                WHERE C.Elt_LeaveFlag IS NULL
                                                AND B.Elt_LeaveFlag in ('C','P')";

            public const string countProcess18 = @"SELECT IsNull(COUNT(Elr_EmployeeId), 0) NumEmp FROM T_EmployeeLeaveRefundHist";
            public const string countAdded18 = @"SELECT IsNull(COUNT(A.Elr_EmployeeId), 0) NumEmp
                                                FROM  T_EmployeeLeaveRefund A
                                                Left Join T_EmployeeLeaveRefundHist B
                                                On A.Elr_EmployeeId = B.Elr_EmployeeId 
                                                And A.Elr_CurrentPayPeriod = B.Elr_CurrentPayPeriod
                                                And A.Elr_LeaveType = B.Elr_LeaveType
                                                And A.Elr_TaxClass = B.Elr_TaxClass
                                                WHERE A.Elr_CurrentPayPeriod = @CURPAYPERIOD
                                                And B.Elr_CurrentPayPeriod Is Null";
            public const string countProcess19 = @"SELECT IsNull(COUNT(Eot_EmployeeId), 0) NumEmp FROM T_EmployeeOvertimeHist";
            public const string countAdded19 = @"SELECT IsNull(COUNT(A.Eot_EmployeeId), 0) NumEmp 
                                                FROM  T_EmployeeOvertime A
                                                Left Join T_EmployeeOvertimeHist B
                                                On A.Eot_CurrentPayPeriod = B.Eot_CurrentPayPeriod
                                                And A.Eot_EmployeeId = B.Eot_EmployeeId
                                                And A.Eot_OvertimeDate = B.Eot_OvertimeDate
                                                And A.Eot_OvertimeType = B.Eot_OvertimeType
                                                WHERE A.Eot_CurrentPayPeriod = @CURPAYPERIOD
                                                AND B.Eot_CurrentPayPeriod Is Null";

            public const string countProcess20 = @"SELECT IsNull(COUNT(Emt_EmployeeID), 0) NumEmp FROM T_EmployeeMasterHist";
            public const string countAdded20 = @"SELECT IsNull(COUNT(A.Emt_EmployeeID), 0) NumEmp FROM T_EmployeeMaster A
                                                Left Join T_EmployeeMasterHist B
                                                On A.Emt_EmployeeID = B.Emt_EmployeeID
                                                And B.Emt_PayPeriod = @CURPAYPERIOD
                                                Where B.Ludatetime Is Null";

            public const string countProcess21 = @"SELECT IsNull(COUNT(Eof_EmployeeId), 0) NumEmp FROM T_EmployeeOffsetHist";
            public const string countAdded21 = @"SELECT IsNull(COUNT(A.Eof_EmployeeId), 0) NumEmp 
                                                FROM  T_EmployeeOffset A
                                                Left Join T_EmployeeOffsetHist B
                                                On A.Eof_CurrentPayPeriod = B.Eof_CurrentPayPeriod
                                                And A.Eof_EmployeeId = B.Eof_EmployeeId
                                                And A.Eof_OffsetDate = B.Eof_OffsetDate
                                                And A.Eof_Seqno = B.Eof_Seqno
                                                WHERE A.Eof_CurrentPayPeriod = @CURPAYPERIOD
                                                AND B.Eof_CurrentPayPeriod Is Null";
            #endregion
            #region [InsertUpdate  for Cycle Closing]
            public const string insertPosting1 = @"INSERT INTO T_EmployeeAllowanceHist 
                                                    SELECT A.* FROM T_EmployeeAllowance A
                                                    Left Join T_EmployeeAllowanceHist B
                                                    On A.Eal_EmployeeId = B.Eal_EmployeeId
                                                    And A.Eal_CurrentPayPeriod = B.Eal_CurrentPayPeriod
                                                    And A.Eal_AllowanceCode = B.Eal_AllowanceCode
                                                    WHERE A.Eal_CurrentPayPeriod = @CurPayPeriod
                                                    And B.Eal_CurrentPayPeriod Is Null";
            public const string insertPosting2 = @"INSERT INTO T_EmployeeAdjustmentHist 
                                                    SELECT A.* FROM T_EmployeeAdjustment A
                                                    Left Join T_EmployeeAdjustmentHist B
                                                    On A.Ead_EmployeeId = B.Ead_EmployeeId
                                                    And A.Ead_CurrentPayPeriod = B.Ead_CurrentPayPeriod
                                                    WHERE A.Ead_CurrentPayPeriod = @CurPayPeriod
                                                    And B.Ead_CurrentPayPeriod Is Null";
            public const string insertPosting3 = @"INSERT INTO T_EmployeeDeductionDetailHist 
                                                    SELECT A.* FROM T_EmployeeDeductionDetail A
                                                    Left Join T_EmployeeDeductionDetailHist B
                                                    On A.Edd_EmployeeID = B.Edd_EmployeeID
                                                    And A.Edd_DeductionCode = B.Edd_DeductionCode
                                                    And A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                    And A.Edd_CurrentPayPeriod = B.Edd_CurrentPayPeriod
                                                    And A.Edd_PayPeriod = B.Edd_PayPeriod
                                                    And A.Edd_SeqNo = B.Edd_SeqNo
                                                    WHERE A.Edd_CurrentPayPeriod = @CurPayPeriod
                                                    And B.Edd_CurrentPayPeriod Is Null";
            public const string insertPosting4 = @"INSERT INTO T_EmployeePayrollCalcAnnual 
                                                    SELECT A.* FROM T_EmployeePayrollCalc A
                                                    Left Join T_EmployeePayrollCalcAnnual B
                                                    On A.Epc_EmployeeId = B.Epc_EmployeeId
                                                    And A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                    WHERE A.Epc_CurrentPayPeriod = @CurPayPeriod 
                                                    And B.Epc_CurrentPayPeriod Is Null

                                                    INSERT INTO T_EmployeePayrollCalcAnnualExt 
                                                    SELECT A.* FROM T_EmployeePayrollCalcExt A
                                                    Left Join T_EmployeePayrollCalcAnnualExt B
                                                    On A.Epc_EmployeeId = B.Epc_EmployeeId
                                                    And A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                    WHERE A.Epc_CurrentPayPeriod = @CurPayPeriod 
                                                    And B.Epc_CurrentPayPeriod Is Null";
            public const string insertPosting5 = @"INSERT INTO T_PayrollErrorHist 
                                                    SELECT A.* FROM T_PayrollError A
                                                    Left Join T_PayrollErrorHist B
                                                    On A.Per_EmployeeId = B.Per_EmployeeId
                                                    And A.Per_Remarks = B.Per_Remarks
                                                    And A.Per_CurrentPayPeriod = B.Per_CurrentPayPeriod
                                                    WHERE A.Per_CurrentPayPeriod = @CURPAYPERIOD
                                                    And B.Per_CurrentPayPeriod Is Null";
            public const string updatePosting6 = @"declare @CurPayPeriodEndDate as datetime set @CurPayPeriodEndDate = (SELECT Ppm_EndCycle FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = @CURPAYPERIOD)
                                                    UPDATE T_EmployeeDeductionLedger
                                                    SET   Edl_PaidAmount = Edl_PaidAmount  + PaidAmt
                                                    , Edl_DeferredAmount = Edl_DeferredAmount - DeferredAmt
                                                    , Edl_FullyPaidDate = CASE WHEN Edl_PaidAmount  + PaidAmt = Edl_DeductionAmount then @CurPayPeriodEndDate ELSE Edl_FullyPaidDate END
                                                    , Usr_Login = @UserLogin
                                                    , Ludatetime = GetDate()
                                                    FROM T_EmployeeDeductionLedger
                                                    INNER JOIN ( SELECT Edd_EmployeeID  
                                                    , Edd_DeductionCode 
                                                    , Edd_StartDeductionDate  
                                                    , PaidAmt = Sum(Edd_Amount) 
                                                    , DeferredAmt = Sum(Case When Edd_FromDeferred = 1  Then Edd_Amount Else 0 End)
                                                    FROM T_EmployeeDeductionDetail
                                                    WHERE Edd_CurrentPayPeriod = @CurPayPeriod
                                                    and  Edd_PaymentFlag = 1
                                                    GROUP BY Edd_EmployeeID  
                                                    , Edd_DeductionCode 
                                                    , Edd_StartDeductionDate  ) PaidDed on PaidDed.Edd_EmployeeID = T_EmployeeDeductionLedger.Edl_EmployeeID
                                                    and PaidDed.Edd_DeductionCode = T_EmployeeDeductionLedger.Edl_DeductionCode
                                                    and PaidDed.Edd_StartDeductionDate = T_EmployeeDeductionLedger.Edl_StartDeductionDate";
            public const string insertPosting7 = @"INSERT INTO T_EmployeeDeductionLedgerHist 
                                                    SELECT A.* FROM T_EmployeeDeductionLedger A
                                                    Left Join T_EmployeeDeductionLedgerHist B
                                                    On A.Edl_EmployeeID = B.Edl_EmployeeID
                                                    And A.Edl_DeductionCode = B.Edl_DeductionCode
                                                    And A.Edl_StartDeductionDate = B.Edl_StartDeductionDate
                                                    WHERE B.Ludatetime Is Null
                                                    and A.Edl_DeductionAmount = A.Edl_PaidAmount
                                                    And A.Edl_FullyPaidDate is not null";
            public const string insertPosting8 = @"Insert Into T_EmployeeDeductionDetailHistFP
                                                    Select A.* From T_EmployeeDeductionDetailHist A
                                                    Inner join T_EmployeeDeductionLedger on Edl_EmployeeID = A.Edd_EmployeeID
                                                    and Edl_DeductionCode = A.Edd_DeductionCode
                                                    and Edl_StartDeductionDate = A.Edd_StartDeductionDate
                                                    and Edl_FullyPaidDate is not null
                                                    and Edl_DeductionAmount = Edl_PaidAmount
                                                    Left Join T_EmployeeDeductionDetailHistFP B
                                                    On A.Edd_EmployeeID = B.Edd_EmployeeID
                                                    And A.Edd_DeductionCode = B.Edd_DeductionCode
                                                    And A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                    And A.Edd_CurrentPayPeriod = B.Edd_CurrentPayPeriod
                                                    And A.Edd_PayPeriod = B.Edd_PayPeriod
                                                    And A.Edd_SeqNo = B.Edd_SeqNo
                                                    Where B.Ludatetime Is Null";
            public const string insertPosting9 = @"INSERT INTO T_EmployeeDeductionDeffered
                                                                    SELECT A.Edd_EmployeeID  
                                                                        , A.Edd_DeductionCode 
                                                                        , A.Edd_StartDeductionDate  
                                                                        , A.Edd_CurrentPayPeriod 
                                                                        , A.Edd_SeqNo 
                                                                        , A.Edd_Amount
                                                                        , A.Usr_Login       
                                                                        , A.Ludatetime
                                                                    FROM T_EmployeeDeductionDetail A
                                                                    Left Join T_EmployeeDeductionDeffered B
                                                                    On A.Edd_EmployeeID = B.Edd_EmployeeID
                                                                    And A.Edd_DeductionCode = B.Edd_DeductionCode
                                                                    And A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                                    And A.Edd_PayPeriod = B.Edd_PayPeriod
                                                                    And A.Edd_SeqNo = B.Edd_SeqNo
                                                                    WHERE A.Edd_CurrentPayPeriod = @CurPayPeriod
                                                                        and A.Edd_PaymentFlag = 0
                                                                        and A.Edd_FromDeferred = 0
	                                                                    And B.Ludatetime Is Null";
            public const string updatePosting10 = @"UPDATE T_EmployeeDeductionLedger
                                                                    SET Edl_DeferredAmount = Edl_DeferredAmount + DeferredAmt
	                                                                    , Usr_Login = @UserLogin
	                                                                    , Ludatetime = GetDate()
                                                                    FROM T_EmployeeDeductionLedger
                                                                    INNER JOIN ( SELECT Edd_EmployeeID  
				                                                                    , Edd_DeductionCode 
				                                                                    , Edd_StartDeductionDate  
				                                                                    , DeferredAmt = Sum(Edd_Amount) 
				                                                                    FROM T_EmployeeDeductionDetail                            
				                                                                    WHERE Edd_CurrentPayPeriod = @CurPayPeriod
					                                                                    and Edd_PaymentFlag = 0
					                                                                    and Edd_FromDeferred = 0
				                                                                    GROUP BY Edd_EmployeeID  
				                                                                    , Edd_DeductionCode 
				                                                                    , Edd_StartDeductionDate  ) DeferredDed on DeferredDed.Edd_EmployeeID = T_EmployeeDeductionLedger.Edl_EmployeeID
							                                                                    and DeferredDed.Edd_DeductionCode = T_EmployeeDeductionLedger.Edl_DeductionCode
							                                                                    and DeferredDed.Edd_StartDeductionDate = T_EmployeeDeductionLedger.Edl_StartDeductionDate";
            public const string insertPosting11 = @"INSERT INTO  T_EmployeeLogLedgerHist
                                                    SELECT A.* FROM T_EmployeeLogLedger A
                                                    Left Join T_EmployeeLogLedgerHist B
                                                    On A.Ell_EmployeeId = B.Ell_EmployeeId
                                                    And A.Ell_ProcessDate = B.Ell_ProcessDate
                                                    WHERE A.Ell_PayPeriod = @CurPayPeriod
                                                    And B.Ell_PayPeriod Is Null";
            public const string updatePosting12 = @"UPDATE T_OffSet
		                                            SET Off_Served = 1
		                                            , Usr_login = @UserLogin      
		                                            , Ludatetime = GetDate()
		                                            From T_OffSet
		                                            Inner Join T_EmployeeLogLedger On Ell_EmployeeId = Off_EmployeeId
		                                              And Ell_ProcessDate = Off_DateWork
		                                            Where Off_Served = 0
		                                              And Off_Type = 'U'
		                                              And Ell_ComputedDayWorkMin = Ell_ForOffsetMin";
            public const string insertPosting13 = @"INSERT INTO T_Offset
		                                            SELECT 
		                                            Ell_EmployeeId
		                                            , Ell_ProcessDate
		                                            , 'E'
		                                            , '00'
		                                            , Ell_EarnedSatOff/60.000
		                                            , 0
		                                            , 1
		                                            , Null
		                                            , @UserLogin
		                                            , Getdate()
		                                             FROM T_EmployeeLogLedger
		                                            WHERE Ell_PayPeriod = @CurPayPeriod
			                                            and Ell_DayCode = 'SOFF'
		                                               and Ell_EarnedSatOff >  0";
            public const string insertPosting14 = @"INSERT INTO T_Offset
		                                            SELECT Ell_EmployeeId
		                                            , Ell_ProcessDate
		                                            , 'E'
		                                            , IsNull((REPLICATE ( '0', 2 - len(MaxSeq))+ Convert(char,MaxSeq)), 0) as MaxSeq
		                                            , Ell_excessoffset/60.000
		                                            , 0
		                                            , 1
		                                            , Null
		                                            , @UserLogin
		                                            , Getdate()
		                                            FROM T_EmployeeLogLedger
		                                            LEFT JOIN ( SELECT Off_EmployeeId
				                                            , Off_DateWork
				                                            , Max(IsNull(Off_Seqno,0))+ 1 as MaxSeq 
				                                            FROM T_Offset 
			                                            GROUP BY Off_EmployeeId
				                                            , Off_DateWork ) Offset on Off_EmployeeId =Ell_EmployeeId
					                                            and Off_DateWork = Ell_ProcessDate
		                                            WHERE Ell_PayPeriod = @CurPayPeriod
		                                            and Ell_excessoffset > 0";
            public const string updatePosting15 = @"UPDATE T_EmployeeMaster
                                                    SET Emt_Shiftcode = T_EmployeeLogLedger.Ell_ShiftCode
                                                    From T_EmployeeMaster
                                                    Inner Join T_EmployeeLogLedger on T_EmployeeLogLedger.Ell_EmployeeID = T_EmployeeMaster.Emt_EmployeeID  
                                                    Inner Join ( Select Ell_EmployeeID
                                                                , RegDate =Max(Ell_ProcessDate)
                                                                    From T_EmployeeLogLedger
                                                                    where Ell_PayPeriod =  @CURPAYPERIOD
                                                                        and Ell_DayCode = 'REG'
                                                                Group by Ell_EmployeeID) EmpShift on EmpShift.Ell_EmployeeID = T_EmployeeLogLedger.Ell_EmployeeID
                                                                            and EmpShift.RegDate = T_EmployeeLogLedger.Ell_ProcessDate  ";
            public const string insertPosting16 = @"INSERT INTO T_LaborHourErrorHist
                                                    SELECT A.* FROM T_LaborHourError A
                                                    Left Join dbo.T_LaborHourErrorHist B
                                                    On A.Lhe_EmployeeId = B.Lhe_EmployeeId
                                                    And A.Lhe_CurrentPayperiod = B.Lhe_CurrentPayperiod
                                                    WHERE A.Lhe_CurrentPayperiod = @CURPAYPERIOD
                                                    And B.Lhe_CurrentPayperiod Is Null";
            public const string insertPosting17 = @"INSERT INTO T_EmployeeLeaveAvailmentHist
                                                    SELECT B.* FROM  T_EmployeeLeaveAvailment B
                                                    left Join T_EmployeeLeaveAvailmentHist C
                                                    ON	B.Elt_EmployeeId = C.Elt_EmployeeId
                                                    AND	B.Elt_LeaveDate = C.Elt_LeaveDate
                                                    AND	B.Elt_LeaveType = C.Elt_LeaveType
                                                    WHERE C.Elt_LeaveFlag IS NULL
                                                    AND B.Elt_LeaveFlag in ('C','P')
                                                    AND B.Elt_Status in ('0','2','4','6','8','9')
                                                    AND B.Elt_CurrentPayPeriod = @CURPAYPERIOD";
            public const string insertPosting18 = @"INSERT INTO T_EmployeeLeaveRefundHist
                                                    SELECT A.* FROM  T_EmployeeLeaveRefund A
                                                    Left Join T_EmployeeLeaveRefundHist B
                                                    On A.Elr_EmployeeId = B.Elr_EmployeeId 
                                                    And A.Elr_CurrentPayPeriod = B.Elr_CurrentPayPeriod
                                                    And A.Elr_LeaveType = B.Elr_LeaveType
                                                    And A.Elr_TaxClass = B.Elr_TaxClass
                                                    WHERE A.Elr_CurrentPayPeriod = @CURPAYPERIOD
                                                    And B.Elr_CurrentPayPeriod Is Null";
            public const string insertPosting19 = @"INSERT INTO T_EmployeeOvertimeHist
                                                    SELECT A.*,'' FROM  T_EmployeeOvertime A
                                                    Left Join T_EmployeeOvertimeHist B
                                                    On A.Eot_CurrentPayPeriod = B.Eot_CurrentPayPeriod
                                                    And A.Eot_EmployeeId = B.Eot_EmployeeId
                                                    And A.Eot_OvertimeDate = B.Eot_OvertimeDate
                                                    And A.Eot_Seqno = B.Eot_Seqno
                                                    WHERE A.Eot_CurrentPayPeriod = @CURPAYPERIOD
													AND A.Eot_OvertimeFlag  IN ('P','C')
                                                    AND A.Eot_Status in ('0','2','4','6','8','9')
                                                    AND B.Eot_CurrentPayPeriod Is Null";
            public const string insertPosting20 = @"INSERT INTO T_EmployeeMasterHist
                                                    SELECT  @CURPAYPERIOD
                                                        , A.Emt_EmployeeID 
                                                        , A.Emt_CostCenterDate 
                                                        , A.Emt_CostCenterCode 
                                                        , A.Emt_EmploymentStatus 
                                                        , A.Emt_LocationCode
                                                        , A.Emt_PositionCode 
                                                        , A.Emt_PositionDate 
                                                        , A.Emt_ImmediateSuperior 
                                                        , A.Emt_NextLevelSuperior 
                                                        , A.Emt_PayrollStatus 
                                                        , A.Emt_PaymentMode
                                                        , A.Emt_PayrollType
                                                        , A.Emt_SalaryEffectivityDate 
                                                        , A.Emt_SalaryRate 
                                                        , A.Emt_JobStatus 
                                                        , A.Emt_ConfidentialPayroll
                                                        , A.Emt_WorkType
                                                        , A.Emt_WorkGroup
                                                        , A.Emt_HDMFCode
                                                        , A.Emt_HDMFFixedContrib
                                                        , A.Emt_SSSCode
                                                        , A.Emt_SSSFixedContrib
                                                        , A.Emt_PhilhealthCode
                                                        , A.Emt_PhilhealthFixedContrib
                                                        , A.Usr_Login
                                                        , A.Ludatetime
                                                       , A.Emt_TaxRatePayrollType
                                                       , A.Emt_TaxRateEffectivityDate
                                                       , A.Emt_TaxRate
                                                       , A.Emt_BillingPayrollType
                                                       , A.Emt_BillingEffectivityDate
                                                       , A.Emt_BillingRate
                                                    FROM T_EmployeeMaster A
                                                    Left Join T_EmployeeMasterHist B
                                                    On A.Emt_EmployeeID = B.Emt_EmployeeID
                                                    And B.Emt_PayPeriod = @CURPAYPERIOD
                                                    Where B.Ludatetime Is Null";
            public const string updatePosting21 = @"declare @DateCompare as datetime set @DateCompare = (SELECT Ppm_EndCycle FROM T_PayPeriodMaster WHERE Ppm_PayPeriod = @CURPAYPERIOD)
                                                    UPDATE t_employeemaster
                                                    SET Emt_Age  = CASE
                                                           WHEN dateadd(year, datediff (year, Emt_Birthdate, @DateCompare), Emt_Birthdate) > @DateCompare
                                                        THEN datediff (year, Emt_Birthdate, @DateCompare) - 1
                                                                                 ELSE datediff (year, Emt_Birthdate, @DateCompare) END
                                                                                 WHERE Emt_Birthdate is not null AND (CASE
                                                           WHEN dateadd(year, datediff (year, Emt_Birthdate, @DateCompare), Emt_Birthdate) > @DateCompare
                                                        THEN datediff (year, Emt_Birthdate, @DateCompare) - 1
                                                                                 ELSE datediff (year, Emt_Birthdate, @DateCompare) END) > 0";

            public const string transferOffsetToHistory = @"INSERT INTO T_EmployeeOffsetHist
                                                            SELECT A.* FROM  T_EmployeeOffset A
                                                            Left Join T_EmployeeOffsetHist B
                                                            On A.Eof_CurrentPayPeriod = B.Eof_CurrentPayPeriod
                                                            And A.Eof_EmployeeId = B.Eof_EmployeeId
                                                            And A.Eof_OffsetDate = B.Eof_OffsetDate
                                                            And A.Eof_Seqno = B.Eof_Seqno
                                                            WHERE A.Eof_CurrentPayPeriod = @CURPAYPERIOD
                                                            AND A.Eof_Status in ('0','A')
                                                            AND B.Eof_CurrentPayPeriod Is Null";

            public const string updatePosting22 = @"Update T_ProcessControlMaster
                                                    Set Pcm_ProcessFlag = 0
                                                    ,Usr_Login = @UserLogin
                                                    Where Pcm_SystemID = 'PAYROLL'
                                                    And Pcm_ProcessID in ('CYCLEOPEN', 'DBOPNBCKUP')

                                                    Insert Into [T_ProcessControlTrail]
                                                    SELECT Stm_SystemID    = 'PAYROLL'
                                                         , Stm_ProcessID   = 'CYCLEOPEN'
                                                         , Stm_ProcessFlag = 0
                                                         , Usr_Login       = @UserLogin
                                                         , Ludatetime      = getdate()

                                                    Insert Into [T_ProcessControlTrail]
                                                    SELECT Stm_SystemID    = 'PAYROLL'
                                                         , Stm_ProcessID   = 'DBOPNBCKUP'
                                                         , Stm_ProcessFlag = 0
                                                         , Usr_Login       = @UserLogin
                                                         , Ludatetime      = getdate()

                                                    Update T_ProcessControlMaster
                                                    Set Pcm_ProcessFlag = 1
                                                    ,Usr_Login = @UserLogin
                                                    Where Pcm_SystemID = 'PAYROLL'
                                                    And Pcm_ProcessID = 'CYCLECLOSE'

                                                    Insert Into [T_ProcessControlTrail]
                                                    SELECT Stm_SystemID    = 'PAYROLL'
                                                         , Stm_ProcessID   = 'CYCLECLOSE'
                                                         , Stm_ProcessFlag = 1
                                                         , Usr_Login       = @UserLogin
                                                         , Ludatetime      = getdate()

                                                    If Right(@CURPAYPERIOD, 3) = '122' 
                                                    Begin
	                                                    Update T_ProcessControlMaster
	                                                    Set Pcm_ProcessFlag = 0
	                                                    ,Usr_Login = @UserLogin
	                                                    Where Pcm_SystemID = 'PAYROLL'
	                                                    And Pcm_ProcessID in ( 'YEAREND', 'DBYRBCKUP')
                                                    End";
            public const string transferCurrentDeductionDetailBilling = @"Insert into T_EmployeeDeductionDetailHistBilling
                                                    Select A.* from T_EmployeeDeductionDetailBilling A
                                                    LEFT JOIN T_EmployeeDeductionDetailHistBilling B
                                                    ON A.Edd_EmployeeID = B.Edd_EmployeeID
                                                    AND A.Edd_DeductionCode = B.Edd_DeductionCode
                                                    AND A.Edd_StartDeductionDate = B.Edd_StartDeductionDate
                                                    AND A.Edd_CurrentPayPeriod = B.Edd_CurrentPayPeriod
                                                    AND A.Edd_PayPeriod = B.Edd_PayPeriod
                                                    AND A.Edd_SeqNo = B.Edd_SeqNo
                                                    WHERE B.Ludatetime IS NULL";
            public const string transferCurrentDeductionDetailTax = @"Insert into T_EmployeeDeductionDetailHistTax
                                                    Select A.* from T_EmployeeDeductionDetailTax A
                                                    LEFT JOIN T_EmployeeDeductionDetailHistTax B
                                                    ON A.Edd_EmployeeID = B.Edd_EmployeeID
                                                    AND A.Edd_DeductionCode = B.Edd_DeductionCode 
                                                    AND A.Edd_StartDeductionDate = B.Edd_StartDeductionDate 
                                                    AND A.Edd_CurrentPayPeriod = B.Edd_CurrentPayPeriod 
                                                    AND A.Edd_PayPeriod = B.Edd_PayPeriod 
                                                    AND A.Edd_SeqNo = B.Edd_SeqNo 
                                                    WHERE B.Ludatetime IS NULL";

            public const string transferCurrentCyclePayrollBilling = @"Insert into T_EmployeePayrollCalcAnnualBilling
                                                    Select A.* from T_EmployeePayrollCalcBilling A
                                                    LEFT JOIN T_EmployeePayrollCalcAnnualBilling B
                                                    ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                    AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                    WHERE B.Ludatetime IS NULL

                                                    Insert into T_EmployeePayrollCalcAnnualBillingExt
                                                    Select A.* from T_EmployeePayrollCalcBillingExt A
                                                    LEFT JOIN T_EmployeePayrollCalcAnnualBillingExt B
                                                    ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                    AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                    WHERE B.Ludatetime IS NULL";
            public const string transferCurrentCyclePayrollTax = @"Insert Into T_EmployeePayrollCalcAnnualTax
                                                    Select A.* from T_EmployeePayrollCalcTax A
                                                    LEFT JOIN T_EmployeePayrollCalcAnnualTax B
                                                    ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                    AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                    WHERE B.Ludatetime IS NULL

                                                    Insert Into T_EmployeePayrollCalcAnnualTaxExt
                                                    Select A.* from T_EmployeePayrollCalcTaxExt A
                                                    LEFT JOIN T_EmployeePayrollCalcAnnualTaxExt B
                                                    ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                    AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                    WHERE B.Ludatetime IS NULL";

            public const string transferPayrollErrorBilling = @"Insert into T_PayrollErrorHistBilling
                                                    Select A.* from T_PayrollErrorBilling A
                                                    LEFT JOIN T_PayrollErrorHistBilling B
                                                    ON A.Per_EmployeeId = B.Per_EmployeeId
                                                    AND A.Per_CurrentPayPeriod = B.Per_CurrentPayPeriod
                                                    WHERE B.Ludatetime IS NULL";
            public const string transferPayrollErrorTax = @"Insert into T_PayrollErrorHistTax
                                                    Select A.* from T_PayrollErrorTax A
                                                    LEFT JOIN T_PayrollErrorHistTax B
                                                    ON A.Per_EmployeeId = B.Per_EmployeeId
                                                    AND A.Per_CurrentPayPeriod = B.Per_CurrentPayPeriod
                                                    WHERE B.Ludatetime IS NULL ";
            public const string updateMovementTransaction = @"UPDATE T_Movement
                                                    SET Mve_Status = 2
                                                        , Mve_Reason = 'CANCELLED BY CYCLE CLOSING'
                                                        , Usr_Login = 'SA'
                                                        , Ludatetime = getdate()
                                                    WHERE Mve_Status NOT IN ('9', 'A')
                                                    AND Mve_Flag = 'C'";
#endregion
            #region [For Year-End House Keeping]
            public const string countInsertAnnualPayrollHistory = @"SELECT IsNull(Count(A.Epc_EmployeeId), 0) NumEmp
                                                                    From T_EmployeePayrollCalcAnnual A
                                                                    Left Join T_EmployeePayrollCalcHist B
                                                                    On A.Epc_EmployeeId = B.Epc_EmployeeId 
                                                                    And A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                    Where B.Epc_CurrentPayPeriod Is Null";
            public const string insertAnnualPayrollHistory = @"Insert Into T_EmployeePayrollCalcHist
                                                                    Select A.* From T_EmployeePayrollCalcAnnual A
                                                                    Left Join T_EmployeePayrollCalcHist B
                                                                    On A.Epc_EmployeeId = B.Epc_EmployeeId 
                                                                    And A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                    Where B.Epc_CurrentPayPeriod Is Null

                                                                Insert Into T_EmployeePayrollCalcHistExt
                                                                    Select A.* From T_EmployeePayrollCalcAnnualExt A
                                                                    Left Join T_EmployeePayrollCalcHistExt B
                                                                    On A.Epc_EmployeeId = B.Epc_EmployeeId 
                                                                    And A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                    Where B.Epc_CurrentPayPeriod Is Null";
            public const string countLeaveBalancesRemain = @"Select IsNull(Sum(Elm_VLBalance), 0) SumVL
	                                                                    , IsNull(Sum(Elm_SLBalance), 0) SumSL
	                                                                    , IsNull(Sum(Elm_PLBalance), 0) SumPL
	                                                                    , IsNull(Sum(Elm_BLBalance), 0) SumBL
	                                                                    , IsNull(Sum(Elm_ELBalance), 0) SumEL
		                                                            From T_EmployeeLeaveMaster";
            public const string countLeaveBalancesDeduct = @"Select Sum(Case when Elr_LeaveType = 'VL' then Elr_LeaveHr else 0 end) SumVL
		                                                                , Sum(Case when Elr_LeaveType = 'SL' then Elr_LeaveHr else 0 end) SumSL
	                                                                From T_EmployeeLeaveRefund";
            public const string deductLeaveBalances = @"UPDATE T_EmployeeLeaveMaster
	                                                                SET Elm_VLBalance = Elm_VLBalance - Elr_LeaveHr
	                                                                FROM T_EmployeeLeaveMaster 
	                                                                Inner join  T_EmployeeLeaveRefund on  Elr_EmployeeID = Elm_EmployeeID
	                                                                and Elr_LeaveType = 'VL'

	                                                                UPDATE T_EmployeeLeaveMaster
	                                                                SET Elm_SLBalance = Elm_SLBalance - Elr_LeaveHr
	                                                                FROM T_EmployeeLeaveMaster 
	                                                                Inner join  T_EmployeeLeaveRefund on  Elr_EmployeeID = Elm_EmployeeID
	                                                                and Elr_LeaveType = 'SL'

	                                                                UPDATE T_EmployeeLeaveMaster
	                                                                SET Elm_PLBalance = 0,
	                                                                Elm_BLBalance = 0,
	                                                                Elm_ELBalance = 0
	                                                                FROM T_EmployeeLeaveMaster 
	                                                                INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
	                                                                and Left(Emt_Jobstatus, 1) = 'A'";
            public const string countLeaveBalancesPost = @"DECLARE @EndOfYear as Datetime Set @EndOfYear  = '12/31/'+ (select Ccd_CurrentYear from t_companymaster)
                                                                    DECLARE @VLCREDIT  decimal(7,4), @SLCREDIT decimal(7,4)

                                                                    SET @VLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                                    where Pmt_ParameterID = 'VLCREDIT' and pmt_status = 'A')

                                                                    SET @SLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                                    where Pmt_ParameterID = 'SLCREDIT' and pmt_status = 'A')

                                                                    SELECT SUM(CASE WHEN
			                                                                        CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  THEN 
					                                                                        datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear  )) - 1
				                                                                        ELSE 
					                                                                        datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear ))
				                                                                        END  >= 1 THEN  @VLCREDIT
	                                                                        ELSE 0 END) as VLBal
                                                                    ,SUM(CASE WHEN
			                                                                        CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  THEN 
						                                                                        datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear  )) - 1
					                                                                        ELSE 
						                                                                        datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear ))
					                                                                        END  >= 1 THEN  @SLCREDIT 
		                                                                        ELSE 0 END) as SLBal
                                                                    From T_EmployeeLeaveMaster
                                                                    INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
                                                                        and CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfYear )), 		Emt_HireDate) > @EndOfYear  THEN 
				                                                                        datediff (year, Emt_HireDate, Convert(Datetime,@EndOfYear  )) 		- 1
			                                                                        ELSE 
				                                                                        datediff (year, Emt_HireDate, Convert(Datetime,@EndOfYear ))
			                                                                        END  >= 1
                                                                        and LEFT(Emt_JobStatus, 1) = 'A'";
            public const string insertLeaveBalancesOLD = @"DECLARE @EndOfYear as Datetime Set @EndOfYear  = '12/31/'+ (select Ccd_CurrentYear from t_companymaster)
                                                                    DECLARE @VLCREDIT  decimal(7,4), @SLCREDIT decimal(7,4)

                                                                    SET @VLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                                    where Pmt_ParameterID = 'VLCREDIT' and pmt_status = 'A')

                                                                    SET @SLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                                    where Pmt_ParameterID = 'SLCREDIT' and pmt_status = 'A')

                                                                    UPDATE T_EmployeeLeaveMaster
                                                                    SET	Elm_VLBalance = Elm_VLBalance + 
	                                                                    CASE WHEN
			                                                                    CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  THEN 
						                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear  )) - 1
					                                                                    ELSE 
						                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear ))
					                                                                    END  >= 1 THEN  @VLCREDIT
		                                                                    ELSE 0 END
                                                                    , Elm_SLBalance = Elm_SLBalance + 
		                                                                    CASE WHEN
				                                                                    CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  THEN 
							                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear  )) - 1
						                                                                    ELSE 
							                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfYear ))
						                                                                    END  >= 1 THEN  @SLCREDIT
			                                                                    ELSE 0 END
                                                                    From T_EmployeeLeaveMaster
                                                                    INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
	                                                                    and CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfYear )), 		Emt_HireDate) > @EndOfYear  THEN 
					                                                                    datediff (year, Emt_HireDate, Convert(Datetime,@EndOfYear  )) 		- 1
				                                                                    ELSE 
					                                                                    datediff (year, Emt_HireDate, Convert(Datetime,@EndOfYear ))
				                                                                    END  >= 1
	                                                                    and LEFT(Emt_JobStatus, 1) = 'A'
                                                                        and Emt_EmploymentStatus = 'RG'";
            public const string insertLeaveGrant = @"DECLARE @EndOfYear DATETIME SET @EndOfYear  = '12/31/'+ (SELECT Ccd_CurrentYear FROM t_companymaster)
                                                    INSERT	INTO T_EmployeeLeaveGrant
                                                    SELECT	Emt_EmployeeID
		                                                    ,'00'
		                                                    ,'VL'
		                                                    ,(SELECT CASE 
                                                                        WHEN DATEADD(YEAR, DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  
                                                                        THEN CASE 
                                                                                 WHEN DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) -1 > (SELECT MAX(Lve_ServiceYears) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
                                                                                 THEN (SELECT MAX(Lve_Credit) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
                                                                                 ELSE (SELECT Lve_Credit FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL' AND Lve_ServiceYears = DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) - 1)
                                                                             END
                                                                        ELSE CASE 
                                                                                 WHEN DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) > (SELECT MAX(Lve_ServiceYears) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
                                                                                 THEN (SELECT MAX(Lve_Credit) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
                                                                                 ELSE (SELECT Lve_Credit FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL' AND Lve_ServiceYears = DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)))
                                                                             END
                                                                    END)
		                                                    ,GETDATE()
		                                                    ,@UserLogin
		                                                    ,GETDATE()
                                                    FROM	T_EmployeeMaster
                                                    WHERE	LEFT(Emt_JobStatus, 1) = 'A'
                                                    AND     Emt_EmploymentStatus = 'RG'
                                                    AND		CASE 
			                                                     WHEN DATEADD(YEAR,DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  
			                                                     THEN DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear  )) - 1
                                                                 ELSE DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear ))
		                                                    END >= 1";
            public const string insertLeaveBalances = @"DECLARE @EndOfYear DATETIME SET @EndOfYear  = '12/31/'+ (SELECT Ccd_CurrentYear FROM t_companymaster)

                                                        UPDATE T_EmployeeLeaveMaster
                                                        SET	Elm_VLBalance = Elm_VLBalance + (SELECT CASE 
												                                                        WHEN DATEADD(YEAR, DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  
												                                                        THEN CASE 
														                                                         WHEN DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) -1 > (SELECT MAX(Lve_ServiceYears) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
														                                                         THEN (SELECT MAX(Lve_Credit) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
														                                                         ELSE (SELECT Lve_Credit FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL' AND Lve_ServiceYears = DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) - 1)
													                                                         END
												                                                        ELSE CASE 
														                                                         WHEN DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) > (SELECT MAX(Lve_ServiceYears) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
														                                                         THEN (SELECT MAX(Lve_Credit) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL')
														                                                         ELSE (SELECT Lve_Credit FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'VL' AND Lve_ServiceYears = DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)))
													                                                         END
											                                                        END),
	                                                        Elm_SLBalance = Elm_SLBalance + (SELECT CASE 
												                                                        WHEN DATEADD(YEAR, DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  
												                                                        THEN CASE 
														                                                         WHEN DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) -1 > (SELECT MAX(Lve_ServiceYears) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'SL')
														                                                         THEN (SELECT MAX(Lve_Credit) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'SL')
														                                                         ELSE (SELECT Lve_Credit FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'SL' AND Lve_ServiceYears = DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) - 1)
													                                                         END
												                                                        ELSE CASE 
														                                                         WHEN DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)) > (SELECT MAX(Lve_ServiceYears) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'SL')
														                                                         THEN (SELECT MAX(Lve_Credit) FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'SL')
														                                                         ELSE (SELECT Lve_Credit FROM T_LeaveEntitlement WHERE Lve_LeaveType = 'SL' AND Lve_ServiceYears = DATEDIFF(YEAR, Emt_HireDate,CONVERT(Datetime,@EndOfYear)))
													                                                         END
											                                                        END)
                                                            ,Usr_Login = @UserLogin,Ludatetime = GetDate()
                                                        FROM T_EmployeeLeaveMaster
                                                        INNER JOIN T_EmployeeMaster ON Emt_EmployeeID = Elm_EmployeeID
                                                        AND CASE WHEN DATEADD(YEAR,DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear )), Emt_HireDate) > @EndOfYear  THEN 
                                                                        DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear  )) - 1
                                                                    ELSE 
                                                                        DATEDIFF(YEAR, Emt_HireDate, CONVERT(Datetime,@EndOfYear ))
                                                                    END  >= 1
                                                        AND     LEFT(Emt_JobStatus, 1) = 'A'
                                                        AND     Emt_EmploymentStatus = 'RG'";

            public const string countYearsService = @"SELECT CASE	WHEN dateadd(year, datediff (year, @NextPayPeriodStartDate, Convert(Datetime,@NextPayPeriodEndDate )), @NextPayPeriodStartDate) > @NextPayPeriodEndDate  
			                                                        THEN  datediff (year, @NextPayPeriodStartDate,Convert(Datetime,@NextPayPeriodEndDate  )) - 1
			                                                        ELSE datediff (year, @NextPayPeriodStartDate,Convert(Datetime,@NextPayPeriodEndDate )) 
	                                                         END";
            public const string countLeaveBalancesDeductDL = @"Select  IsNull(Sum(Elm_DLBalance), 0) SumDL
                                                                From T_EmployeeLeaveMaster
                                                                Inner Join T_EmployeeMaster On Emt_EmployeeID = Elm_EmployeeID
                                                                        And Not(SubString(Convert(char(10), Emt_Birthdate, 112), 5, 4) Between '{0}' and '{1}')
                                                                Where Elm_DLBalance > 0";
            public const string deductLeaveBalancesDL = @"Update T_EmployeeLeaveMaster
                                                                Set Elm_DLBalance = 0
                                                                From T_EmployeeLeaveMaster
                                                                Inner Join T_EmployeeMaster On Emt_EmployeeID = Elm_EmployeeID
                                                                        And Not(SubString(Convert(char(10), Emt_Birthdate, 112), 5, 4) Between '{0}' and '{1}')
                                                                Where Elm_DLBalance > 0";
            public const string updateProcessFlagLeaveCredit = @"Update T_ProcessControlMaster
	                                                            Set Pcm_ProcessFlag = 0
	                                                            ,Usr_Login = @UserLogin
	                                                            ,Ludatetime = GetDate()
	                                                            Where Pcm_SystemID = 'LEAVE'
	                                                            And Pcm_ProcessID = 'LVEREFUND'";
            public const string updateProcessFlagYearEnd = @"Update T_ProcessControlMaster
                                                                Set Pcm_ProcessFlag = 1
	                                                            ,Usr_Login = @UserLogin
	                                                            ,Ludatetime = GetDate()
                                                                Where Pcm_SystemID = 'PAYROLL'
                                                                And Pcm_ProcessID = 'YEAREND'

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'YEAREND'
                                                                     , Stm_ProcessFlag = 1
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = GetDate()";
            public const string transferAnnualPayrolltoHistoryBilling = @"Insert into T_EmployeePayrollCalcHistBilling
                                                                Select A.* from T_EmployeePayrollCalcAnnualBilling A
                                                                LEFT JOIN T_EmployeePayrollCalcHistBilling B
                                                                ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                                AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                WHERE B.Ludatetime IS NULL

                                                                Insert into T_EmployeePayrollCalcHistBillingExt
                                                                Select A.* from T_EmployeePayrollCalcAnnualBillingExt A
                                                                LEFT JOIN T_EmployeePayrollCalcHistBillingExt B
                                                                ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                                AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                WHERE B.Ludatetime IS NULL";
            public const string transferAnnualPayrolltoHistoryTax = @"Insert into T_EmployeePayrollCalcHistTax
                                                                Select A.* from T_EmployeePayrollCalcAnnualTax A
                                                                LEFT JOIN T_EmployeePayrollCalcHistTax B
                                                                ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                                AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                WHERE B.Ludatetime IS NULL

                                                                Insert into T_EmployeePayrollCalcHistTaxExt
                                                                Select A.* from T_EmployeePayrollCalcAnnualTaxExt A
                                                                LEFT JOIN T_EmployeePayrollCalcHistTaxExt B
                                                                ON A.Epc_EmployeeId = B.Epc_EmployeeId
                                                                AND A.Epc_CurrentPayPeriod = B.Epc_CurrentPayPeriod
                                                                WHERE B.Ludatetime IS NULL";

            #endregion
            #region [For Cycle Opening]
            public const string countEmployeeAllowanceRecords = @"SELECT IsNull(COUNT(Eal_EmployeeId), 0) NumEmp
                                                                FROM T_EmployeeAllowance
                                                                INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
                                                                    and Acm_Recursive = 0
                                                                    and Eal_CurrentPayPeriod = @CurPayPeriod";
            public const string countEmployeeAllowanceRecordsDelete = @"SELECT IsNull(COUNT(Eal_EmployeeId), 0) NumEmp
                                                                FROM T_EmployeeAllowance
                                                                INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
	                                                                and Acm_Recursive = 0
                                                                WHERE Eal_PayrollPost = 1
	                                                                and Eal_CurrentPayPeriod = @CurPayPeriod";
            public const string deleteEmployeeAllowanceRecords = @"DELETE FROM T_EmployeeAllowance
                                                                FROM T_EmployeeAllowance
                                                                INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
	                                                                and Acm_Recursive = 0
                                                                WHERE Eal_PayrollPost = 1
	                                                                and Eal_CurrentPayPeriod = @CurPayPeriod";

            public const string countEmployeeAllowanceRecords1 = @"SELECT IsNull(COUNT(Eal_EmployeeId), 0) NumEmp
                                                                FROM T_EmployeeAllowance
                                                                INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
	                                                                and Acm_Recursive = 1
                                                                WHERE Eal_CurrentPayPeriod = @CurPayPeriod";
            public const string countEmployeeAllowanceRecordsInitialize = @"SELECT IsNull(COUNT(Eal_EmployeeId), 0) NumEmp
                                                                FROM T_EmployeeAllowance
                                                                INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
	                                                                and Acm_Recursive = 1
                                                                WHERE Eal_PayrollPost = 1
                                                                and Eal_CurrentPayPeriod = @CurPayPeriod";
//            public const string initializeEmployeeAllowanceRecords = @"UPDATE T_EmployeeAllowance
//                                                                SET Eal_PayrollPost = 0
//                                                                FROM T_EmployeeAllowance
//                                                                INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
//	                                                                and Acm_Recursive = 1
//                                                                WHERE Eal_PayrollPost = 1
//	                                                                and Eal_CurrentPayPeriod = @CurPayPeriod";
            /// JULE CHANGED 20090620 : As per maam gladz
            public const string initializeEmployeeAllowanceRecords = @"DELETE FROM T_EmployeeAllowance
                                                                    FROM T_EmployeeAllowance
                                                                    INNER JOIN T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
	                                                                    and Acm_Recursive = 1
                                                                    WHERE Eal_CurrentPayPeriod = @CurPayPeriod

                                                                    	SELECT RecurAllow.Era_EmployeeId
		                                                                      ,MAX(RecurAllow.Era_PayPeriod) AS PayPeriod
		                                                                      ,RecurAllow.Era_AllowanceCode
                                                                          INTO #TempList
                                                                          FROM T_EmployeeRecurringAllowance RecurAllow
                                                                    INNER JOIN T_AllowanceCodeMaster 
                                                                            ON Acm_AllowanceCode = RecurAllow.Era_AllowanceCode
	                                                                       AND Acm_Recursive = 1
	                                                                       AND Acm_Status = 'A'
	                                                                       AND (Acm_ApplicablePayrollPeriod = '0' OR Acm_ApplicablePayrollPeriod = @Quincena)
 	                                                                     WHERE Era_Status = 'A'
	                                                                       AND Era_PayPeriod <= @NextPayPeriod
                                                                      GROUP BY Era_EmployeeId,
 		                                                                       Era_AllowanceCode

                                                                    INSERT INTO T_EmployeeAllowance
                                                                                ([Eal_EmployeeId]
                                                                               ,[Eal_CurrentPayPeriod]
                                                                               ,[Eal_AllowanceCode]
                                                                               ,[Eal_PayrollPost]
                                                                               ,[Eal_AllowanceAmt]
                                                                               ,[Usr_Login]
                                                                               ,[Ludatetime]
                                                                               ,[Eal_Currency]
                                                                               ,[Eal_ExchangeRate])
                                                                         SELECT #TempList.Era_EmployeeId
		                                                                       ,@NextPayPeriod as Era_PayPeriod
		                                                                       ,#TempList.Era_AllowanceCode
		                                                                       ,0 as Eal_PayrollPost 
		                                                                       ,RecurAllow.Era_AllowanceAmt
		                                                                       ,Usr_Login = 'sa'
		                                                                       ,Ludatetime = Getdate()
                                                                                ,'PHP'
                                                                                ,1.0000
	                                                                       FROM #TempList 
                                                                     INNER JOIN T_EmployeeRecurringAllowance RecurAllow  
		                                                                     ON #TempList.Era_EmployeeId = RecurAllow.Era_EmployeeId
		                                                                    AND #TempList.PayPeriod	= RecurAllow.Era_PayPeriod
		                                                                    AND #TempList.Era_AllowanceCode = RecurAllow.Era_AllowanceCode
                                                                     DROP TABLE #TempList";

            public const string initializeEmployeeAllowanceRecords1 = @"UPDATE T_EmployeeAllowance
                                                                SET Eal_CurrentPayPeriod = @NextPayPeriod,
	                                                                Usr_Login = @UserLogin,
	                                                                Ludatetime = getdate()
                                                                WHERE Eal_CurrentPayPeriod = @CurPayPeriod";
            public const string countEmployeeAdjustmentRecordsDelete = @"SELECT IsNull(Count(Ead_EmployeeId), 0) NumEmp 
                                                                FROM T_EmployeeAdjustment
                                                                WHERE Ead_PayrollPost = 1
	                                                                and Ead_CurrentPayPeriod = @CurPayPeriod";
            public const string deleteEmployeeAdjustmentExtRecords = @"DELETE FROM T_EmployeeAdjustmentExt
                                                                WHERE Ead_CurrentPayPeriod = @CurPayPeriod";
            public const string deleteEmployeeAdjustmentRecords = @"DELETE FROM T_EmployeeAdjustment
                                                                WHERE Ead_PayrollPost = 1
	                                                                and Ead_CurrentPayPeriod = @CurPayPeriod";
            public const string initializeEmployeeAdjustmentRecords = @"UPDATE T_EmployeeAdjustment
                                                                        SET Ead_CurrentPayPeriod = @NextPayPeriod,
	                                                                        Usr_Login = @UserLogin,
	                                                                        Ludatetime = getdate()
                                                                        FROM t_EmployeeAllowance
                                                                        WHERE Ead_CurrentPayPeriod = @CurPayPeriod";
            public const string initializeEmployeeAdjustmentExtRecords = @"UPDATE T_EmployeeAdjustmentExt
                                                                        SET Ead_CurrentPayPeriod = @NextPayPeriod,
	                                                                        Usr_Login = @UserLogin,
	                                                                        Ludatetime = getdate()
                                                                        FROM t_EmployeeAllowance
                                                                        WHERE Ead_CurrentPayPeriod = @CurPayPeriod";
            public const string countEmployeeDeductionDefferedDelete = @"SELECT IsNull(Count(T_EmployeeDeductionDeffered.Edd_EmployeeID), 0) NumEmp ,
	                                                                IsNull(SUM(Edd_DeferredAmount),0) SumAmt
                                                                FROM T_EmployeeDeductionDeffered
                                                                INNER JOIN T_EmployeeDeductionDetail on T_EmployeeDeductionDetail.Edd_EmployeeID =   T_EmployeeDeductionDeffered.Edd_EmployeeID 
	                                                                and T_EmployeeDeductionDetail.Edd_DeductionCode = T_EmployeeDeductionDeffered.Edd_DeductionCode
	                                                                and T_EmployeeDeductionDetail.Edd_StartDeductionDate  = T_EmployeeDeductionDeffered.Edd_StartDeductionDate
	                                                                and T_EmployeeDeductionDetail.Edd_PayPeriod = T_EmployeeDeductionDeffered.Edd_PayPeriod
	                                                                and T_EmployeeDeductionDetail.Edd_SeqNo = T_EmployeeDeductionDeffered.Edd_SeqNo
	                                                                and Edd_PaymentFlag = 1
	                                                                and Edd_CurrentPayPeriod = @CurPayPeriod";
            public const string deleteEmployeeDeductionDefferedDelete = @"DELETE FROM T_EmployeeDeductionDeffered
                                                                FROM T_EmployeeDeductionDeffered
                                                                INNER JOIN T_EmployeeDeductionDetail on T_EmployeeDeductionDetail.Edd_EmployeeID =   T_EmployeeDeductionDeffered.Edd_EmployeeID 
	                                                                and T_EmployeeDeductionDetail.Edd_DeductionCode = T_EmployeeDeductionDeffered.Edd_DeductionCode
	                                                                and T_EmployeeDeductionDetail.Edd_StartDeductionDate  = T_EmployeeDeductionDeffered.Edd_StartDeductionDate
	                                                                and T_EmployeeDeductionDetail.Edd_PayPeriod = T_EmployeeDeductionDeffered.Edd_PayPeriod
	                                                                and T_EmployeeDeductionDetail.Edd_SeqNo = T_EmployeeDeductionDeffered.Edd_SeqNo
	                                                                and Edd_PaymentFlag = 1
	                                                                and Edd_CurrentPayPeriod = @CurPayPeriod";
            public const string countBirthdayLeaveCredits = @"
                                                                    Select IsNull(Count(Elm_EmployeeID), 0) * (SELECT Pmt_NumericValue FROM T_ParameterMaster WHERE Pmt_ParameterID = 'DLCREDIT' and Pmt_status ='A')
	                                                                From T_EmployeeLeaveMaster
	                                                                Inner Join T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeId
	                                                                Where Left(Emt_JobStatus, 1) = 'A'
		                                                                 And Substring(Convert(Char(6),Emt_BirthDate, 101), 1, 2) + 
	                                                                Substring(Convert(Char(6),Emt_BirthDate, 101), 4, 2) between 	@NextPayPeriodStartDate and @NextPayPeriodEndDate";
            public const string updateBirthdayLeaveCredits = @"UPDATE T_EmployeeLeaveMaster
                                                                SET Elm_DLBalance = Elm_DLBalance + (SELECT Pmt_NumericValue FROM T_ParameterMaster WHERE Pmt_ParameterID = 'DLCREDIT' and Pmt_status ='A')
                                                                  , Usr_Login = @UserLogin
                                                                  , Ludatetime = GetDate()
                                                                FROM T_EmployeeLeaveMaster
	                                                            INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeId
	                                                            Where Left(Emt_JobStatus, 1) = 'A'
	                                                            And Substring(Convert(Char(6),Emt_BirthDate, 101), 1, 2) + 
		                                                            Substring(Convert(Char(6),Emt_BirthDate, 101), 4, 2) between @NextPayPeriodStartDate and @NextPayPeriodEndDate";
            public const string countDeductionDetailHistoryDelete = @"SELECT IsNull(Count(Edd_EmployeeID), 0) NumEmp, 
	                                                                IsNull(Sum(Edd_Amount), 0) SumAmt 
                                                                From T_EmployeeDeductionDetailHist
                                                                Inner join T_EmployeeDeductionLedger on Edl_EmployeeID = Edd_EmployeeID
	                                                                and Edl_DeductionCode = Edd_DeductionCode
	                                                                and Edl_StartDeductionDate = Edd_StartDeductionDate
	                                                                and Edl_FullyPaidDate is not null   ";
            public const string deleteDeductionDetailHistoryDelete = @"Delete From T_EmployeeDeductionDetailHist
                                                                From T_EmployeeDeductionDetailHist
                                                                Inner join T_EmployeeDeductionLedger on Edl_EmployeeID = Edd_EmployeeID
                                                                    and Edl_DeductionCode = Edd_DeductionCode
                                                                    and Edl_StartDeductionDate = Edd_StartDeductionDate
                                                                    and Edl_FullyPaidDate is not null

                                                                Delete from T_EmployeeDeductionLedger
                                                                Where Edl_FullyPaidDate is not null
                                                                and Edl_DeductionAmount = Edl_PaidAmount";
            public const string countOffsetRecords = @"SELECT IsNull(COUNT(Off_EmployeeId), 0)  NumEmp 
	                                                    , IsNull(Sum(Off_HoursUsed * 60), 0)  SumMinutes 
	                                                    FROM T_Offset
	                                                    Where Off_ApplicableDate BETWEEN @NextPayPeriodStartDate and @NextPayPeriodEndDate";
            public const string initializeOffsetRecords = @"UPDATE T_EmployeeLogLedger
	                                                        SET Ell_ForOffsetMin = IsNull(OffsetMin, 0) 
	                                                        FROM T_EmployeeLogLedger
	                                                        LEFT JOIN (SELECT Off_EmployeeId
	                                                        , Off_ApplicableDate
	                                                        , OffsetMin = Sum(Off_HoursUsed * 60)
	                                                        FROM T_Offset
	                                                        Where Off_ApplicableDate BETWEEN @NextPayPeriodStartDate and @NextPayPeriodEndDate
                                                            GROUP BY Off_EmployeeId
                                                            , Off_ApplicableDate ) OffTrn ON  Off_EmployeeId = Ell_EmployeeId
                                                            AND Off_ApplicableDate = Ell_ProcessDate
                                                            WHERE Ell_PayPeriod = @NextPayPeriod
                                                            AND Ell_ForOffsetMin <> IsNull(OffsetMin, 0)";
            public const string updateLogControlRecord = @"UPDATE T_LogControl
                                                            SET Lct_PayPeriod = @NextPayPeriod
                                                            , Lct_Day01 = 0
                                                            , Lct_Day02 = 0
                                                            , Lct_Day03 = 0
                                                            , Lct_Day04 = 0
                                                            , Lct_Day05 = 0
                                                            , Lct_Day06 = 0
                                                            , Lct_Day07 = 0
                                                            , Lct_Day08 = 0
                                                            , Lct_Day09 = 0
                                                            , Lct_Day10 = 0
                                                            , Lct_Day11 = 0
                                                            , Lct_Day12 = 0
                                                            , Lct_Day13 = 0
                                                            , Lct_Day14 = 0
                                                            , Lct_Day15 = 0
                                                            , Lct_Day16 = 0
                                                            , Lct_Day17 = 0
                                                            , Lct_Day18 = 0
                                                            , Lct_Day19 = 0
                                                            , Lct_Day20 = 0
                                                            , Usr_Login = @UserLogin
                                                            , Ludatetime = getdate()";
            public const string countEmployeeLeaveMasterML = @"DECLARE @EndOfMonth as Datetime

                                                            Set @EndOfMonth = (Select Ppm_EndCycle From t_PayPeriodMaster
			                                                              Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')


                                                            DECLARE @VLCREDIT  decimal(7,4), @SLCREDIT decimal(7,4)

                                                            SET @VLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'VLCREDIT'
                                                            and pmt_status = 'A')

                                                            SET @SLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'SLCREDIT'
                                                            and pmt_status = 'A')

                                                            SELECT SUM(CASE WHEN
                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
		                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
	                                                            ELSE 
		                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
	                                                            END  >= 1 THEN  @VLCREDIT/8.000 /12.0000
                                                            ELSE 0 END) as VLBal
                                                            ,SUM(CASE WHEN
                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
			                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
		                                                            ELSE 
			                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
		                                                            END  >= 1 THEN  @SLCREDIT /8.000 /12.0000
                                                            ELSE 0 END) as SLBal
                                                            From T_EmployeeLeaveMaster
                                                            INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
                                                            and CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth)), 		Emt_HireDate) > @EndOfMonth THEN 
	                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth )) 		- 1
                                                            ELSE 
	                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth))
                                                            END  >= 1
                                                            and LEFT(Emt_JobStatus, 1) = 'A'";
            public const string countEmployeeLeaveMasterAL = @"DECLARE @EndOfMonth as Datetime

                                                            Set @EndOfMonth = (Select Ppm_EndCycle From t_PayPeriodMaster
	                                                                      Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')


                                                            DECLARE @VLCREDIT  decimal(7,4), @SLCREDIT decimal(7,4)

                                                            SET @VLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'VLCREDIT'
                                                            and pmt_status = 'A')

                                                            SET @SLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'SLCREDIT'
                                                            and pmt_status = 'A')

                                                            SELECT SUM(CASE WHEN
                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
                                                                ELSE 
                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
                                                                END  >= 1 THEN  @VLCREDIT
                                                            ELSE 0 END) as VLBal
                                                            ,SUM(CASE WHEN
                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
	                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
                                                                    ELSE 
	                                                                    datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
                                                                    END  >= 1 THEN  @SLCREDIT
                                                            ELSE 0 END) as SLBal
                                                            From T_EmployeeLeaveMaster
                                                            INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
                                                            and CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth)), 		Emt_HireDate) > @EndOfMonth THEN 
                                                                datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth )) 		- 1
                                                            ELSE 
                                                                datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth))
                                                            END  >= 1
                                                            and LEFT(Emt_JobStatus, 1) = 'A' and right(convert(char(8),Emt_HireDate,112),4) between @NextPayPeriodStartDay and @NextPayPeriodEndDay ";
            public const string updateEmployeeLeaveMasterML = @"DECLARE @EndOfMonth as Datetime

                                                            Set @EndOfMonth = (Select Ppm_EndCycle From t_PayPeriodMaster
			                                                              Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')


                                                            DECLARE @VLCREDIT  decimal(7,4), @SLCREDIT decimal(7,4)

                                                            SET @VLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'VLCREDIT'
                                                            and pmt_status = 'A')

                                                            SET @SLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'SLCREDIT'
                                                            and pmt_status = 'A')

                                                            UPDATE T_EmployeeLeaveMaster
                                                            SET	Elm_VLBalance = Elm_VLBalance + 
                                                            CASE WHEN
                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
			                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
		                                                            ELSE 
			                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
		                                                            END  >= 1 THEN  @VLCREDIT /8.000 /12.0000
                                                            ELSE 0 END
                                                            , Elm_SLBalance = Elm_SLBalance + 
                                                            CASE WHEN
	                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
				                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
			                                                            ELSE 
				                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
			                                                            END  >= 1 THEN  @SLCREDIT /8.000 /12.0000
                                                            ELSE 0 END
                                                            From T_EmployeeLeaveMaster
                                                            INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
                                                            and CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth)), 		Emt_HireDate) > @EndOfMonth THEN 
		                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth )) 		- 1
	                                                            ELSE 
		                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth))
	                                                            END  >= 1
                                                            and LEFT(Emt_JobStatus, 1) = 'A'";
            public const string updateEmployeeLeaveMasterAL = @"DECLARE @EndOfMonth as Datetime

                                                            Set @EndOfMonth = (Select Ppm_EndCycle From t_PayPeriodMaster
			                                                              Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')


                                                            DECLARE @VLCREDIT  decimal(7,4), @SLCREDIT decimal(7,4)

                                                            SET @VLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'VLCREDIT'
                                                            and pmt_status = 'A')

                                                            SET @SLCREDIT = (select Pmt_NumericValue from t_parametermaster
                                                            where Pmt_ParameterID = 'SLCREDIT'
                                                            and pmt_status = 'A')

                                                            UPDATE T_EmployeeLeaveMaster
                                                            SET	Elm_VLBalance = Elm_VLBalance + 
                                                            CASE WHEN
	                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
				                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
			                                                            ELSE 
				                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
			                                                            END  >= 1 THEN  @VLCREDIT
                                                            ELSE 0 END
                                                            , Elm_SLBalance = Elm_SLBalance + 
                                                            CASE WHEN
		                                                            CASE WHEN dateadd(year, datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth)), Emt_HireDate) > @EndOfMonth THEN 
					                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth )) - 1
				                                                            ELSE 
					                                                            datediff (year, Emt_HireDate, 		Convert(Datetime,@EndOfMonth))
				                                                            END  >= 1 THEN  @SLCREDIT
	                                                            ELSE 0 END
                                                            From T_EmployeeLeaveMaster
                                                            INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
                                                            and CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth)), 		Emt_HireDate) > @EndOfMonth THEN 
			                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth )) 		- 1
		                                                            ELSE 
			                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth))
		                                                            END  >= 1
                                                            and LEFT(Emt_JobStatus, 1) = 'A' and right(convert(char(8),Emt_HireDate,112),4) between @NextPayPeriodStartDay and @NextPayPeriodEndDay 

                                                            -- Reset to No anniversary indicator
                                                            UPDATE T_EmployeeMaster
                                                            SET Emt_Anniversary = 0
                                                            WHERE Emt_Anniversary = 1


                                                            -- Post Yes anniversary indicator for employees whose anniversary falls on the new current cycle
                                                            UPDATE T_EmployeeMaster
                                                            SET Emt_Anniversary = 1
                                                            WHERE CASE WHEN dateadd(year, datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth)), 		Emt_HireDate) > @EndOfMonth THEN 
			                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth )) 		- 1
		                                                            ELSE 
			                                                            datediff (year, Emt_HireDate, Convert(Datetime,@EndOfMonth))
		                                                            END  >= 1
                                                            and LEFT(Emt_JobStatus, 1) = 'A' and right(convert(char(8),Emt_HireDate,112),4) between @NextPayPeriodStartDay and @NextPayPeriodEndDay ";

            public const string updateNewPayrollPeriod = @"DECLARE @EndDate as Datetime

                                                            Set @EndDate = (Select Ppm_EndCycle From t_PayPeriodMaster
                                                                          Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')

                                                            Update t_PayPeriodMaster
                                                            Set Ppm_CycleIndicator = 'C'
                                                            , Usr_Login = @UserLogin
                                                            , Ludatetime = GETDATE()
                                                            Where Ppm_StartCycle = dateadd(dd,1,@EndDate)
                                                            Update t_PayPeriodMaster
                                                            Set Ppm_CycleIndicator = 'P'
                                                            , Usr_Login = @UserLogin
                                                            , Ludatetime = GETDATE()
                                                            Where Ppm_EndCycle = @EndDate and Ppm_Status = 'A' and Right(Ppm_PayPeriod,1) in ('1','2','0')";

            public const string updateProcessFlagCycleOpening = @"Update T_ProcessControlMaster
                                                                Set Pcm_ProcessFlag = 0
                                                                ,Usr_Login = @UserLogin
                                                                ,Ludatetime = GetDate()
                                                                Where Pcm_SystemID = 'TIMEKEEP'
                                                                And Pcm_ProcessID In ('CUT-OFF', 'LABHRADJ', 'LABHRGEN', 'ASSUMEDOUT', 'ASSUMEDAY')

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'TIMEKEEP'
                                                                     , Stm_ProcessID   = 'CUT-OFF'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'TIMEKEEP'
                                                                     , Stm_ProcessID   = 'LABHRADJ'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'TIMEKEEP'
                                                                     , Stm_ProcessID   = 'LABHRGEN'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'TIMEKEEP'
                                                                     , Stm_ProcessID   = 'ASSUMEDOUT'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'TIMEKEEP'
                                                                     , Stm_ProcessID   = 'ASSUMEDAY'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Update T_ProcessControlMaster
                                                                Set Pcm_ProcessFlag = 0
                                                                ,Usr_Login = @UserLogin
                                                                ,Ludatetime = GetDate()
                                                                Where Pcm_SystemID = 'OVERTIME'
                                                                And Pcm_ProcessID = 'CUT-OFF'

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'OVERTIME'
                                                                     , Stm_ProcessID   = 'CUT-OFF'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Update T_ProcessControlMaster
                                                                Set Pcm_ProcessFlag = 0
                                                                ,Usr_Login = @UserLogin
                                                                ,Ludatetime = GetDate()
                                                                Where Pcm_SystemID = 'LEAVE'
                                                                And Pcm_ProcessID = 'CUT-OFF'

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'LEAVE'
                                                                     , Stm_ProcessID   = 'CUT-OFF'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Update T_ProcessControlMaster
                                                                Set Pcm_ProcessFlag = 0
                                                                ,Usr_Login = @UserLogin
                                                                ,Ludatetime = GetDate()
                                                                Where Pcm_SystemID = 'PAYROLL'
                                                                And Pcm_ProcessID In  ('CYCCUT-OFF', 'DBCLSBCKUP', 'CYCLECLOSE', 'ADJBACKUP' ,'ADJPOST','ALWBACKUP' ,'ALWPOST','CUT-OFF','PAYTRNPOST','PAYCALC','ATMDISKGEN')

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'CYCCUT-OFF'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'DBCLSBCKUP'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'CYCLECLOSE'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'ADJBACKUP'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'ADJPOST'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'ALWBACKUP'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'ALWPOST'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'CUT-OFF'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'PAYTRNPOST'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'PAYCALC'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'ATMDISKGEN'
                                                                     , Stm_ProcessFlag = 0
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()

                                                                Update T_ProcessControlMaster
                                                                Set Pcm_ProcessFlag = 1
                                                                ,Usr_Login = @UserLogin
                                                                ,Ludatetime = GetDate()
                                                                Where Pcm_SystemID = 'PAYROLL'
                                                                And Pcm_ProcessID = 'CYCLEOPEN'

                                                                Insert Into [T_ProcessControlTrail]
                                                                SELECT Stm_SystemID    = 'PAYROLL'
                                                                     , Stm_ProcessID   = 'CYCLEOPEN'
                                                                     , Stm_ProcessFlag = 1
                                                                     , Usr_Login       = @UserLogin
                                                                     , Ludatetime      = getdate()";
            public const string checkInvalidShiftCode = @"SELECT Emt_EmployeeID + ' - ' + Emt_Firstname + ' ' + Emt_Lastname + '  (' + Case when Scm_Status = 'I' then 'Invalid Shift' else 'Unregistered Shift' end + ')'
                                                            FROM T_EmployeeMaster A
                                                            LEFT JOIN T_ShiftCodeMaster  ON  Scm_ShiftCode = Emt_Shiftcode  
                                                            Where Left(Emt_JobStatus, 1) = 'A'  and
                                                            ( Scm_Status = 'I' or Scm_ShiftCode is null) 
                                                            Order by Emt_Lastname,
                                                                Emt_Firstname";
            public const string updateEncodedOTCycOpen = @"UPDATE T_EmployeeLogLedger
                                                        SET Ell_EncodedOvertimeAdvHr = Isnull(AdvOvtHr,0)
                                                        , Ell_EncodedOvertimePostHr = Isnull(PostOvtHr,0) 
                                                        FROM T_EmployeeLogLedger
                                                        LEFT JOIN (SELECT Eot_EmployeeId
                                                        , Eot_OvertimeDate
                                                        , AdvOvtHr = Sum(Case when Eot_OvertimeType = 'A' Then Eot_OvertimeHour Else 0 End)
                                                        , PostOvtHr = Sum(Case when Eot_OvertimeType = 'P' Then Eot_OvertimeHour Else 0 End)
                                                        FROM T_EmployeeOvertime
                                                        WHERE Eot_Status in ('A', '9')
                                                        GROUP BY Eot_EmployeeId
                                                        , Eot_OvertimeDate ) OvtTrn ON  Eot_EmployeeId = Ell_EmployeeId
                                                        AND Eot_OvertimeDate = Ell_ProcessDate
                                                        WHERE ( Ell_EncodedOvertimeAdvHr <> IsNull(AdvOvtHr, 0) OR
                                                        Ell_EncodedOvertimePostHr <> IsNull(PostOvtHr, 0) )";

            #endregion

            #region [For Labor Hour Generation]
            public const string fetchPreLaborHourProcessingError = @"Select Ell_EmployeeID [Employee ID]
                                                                         , @CurPayPeriod as [Current Payperiod]
                                                                         , null as [Process Date]
                                                                         , 'No restday set-up' as Remarks
                                                                    From T_EmployeeLogLedger
                                                                    WHERE Ell_PayPeriod = @CurPayPeriod
                                                                    Group by Ell_EmployeeID
                                                                    Having Sum(Case when Ell_RestDay = 1 then 1 else 0 end) = 0

                                                                    UNION

                                                                    SELECT Ell_EmployeeId
                                                                           , @CurPayPeriod as CurrentPayperiod
                                                                           ,Ell_ProcessDate as ProcessDate
                                                                           , RTRIM(Ell_DayCode) + '-Invalid Day Code' as Remarks
                                                                    FROM T_EmployeeLogLedger
                                                                    LEFT JOIN T_DayCodeMaster ON Dcm_DayCode = Ell_DayCode
                                                                    WHERE Ell_PayPeriod = @CurPayPeriod
                                                                          AND ( Dcm_DayCode is null or Dcm_Status = 'C')


                                                                    UNION

                                                                    SELECT Ell_EmployeeId
                                                                           ,@CurPayPeriod as CurrentPayperiod
                                                                           ,Ell_ProcessDate as ProcessDate
                                                                           , RTRIM(Ell_ShiftCode) + '-Invalid Shift Code' as Remarks
                                                                    FROM T_EmployeeLogLedger
                                                                    LEFT JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode
                                                                    WHERE Ell_PayPeriod = @CurPayPeriod
                                                                        AND ( Scm_ShiftCode is null or Scm_Status = 'C') 
                                    
                                                                     UNION

                                                                     SELECT  Ell_EmployeeId
                                                                           ,@CurPayPeriod as CurrentPayperiod
                                                                           , Ell_ProcessDate as ProcessDate
                                                                           , RTRIM(Ell_ShiftCode) + '-Non-Regular days not 8 hours' as Remarks
                                                                    FROM T_EmployeeLogLedger
                                                                    LEFT JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode
                                                                    WHERE Ell_PayPeriod = @CurPayPeriod
                                                                          and LEN(RTRIM(Scm_EquivalentShiftCode)) > 0
                                                                          AND Scm_ShiftHours > 8
                                                                          AND Ell_DayCode <>  'REG'
                                                                  UNION

                                                                 SELECT Ell_EmployeeId
                                                                           , @CurPayPeriod as CurrentPayperiod
                                                                           ,Ell_ProcessDate as ProcessDate
                                                                           , RTRIM(Ell_daycode) + '- No Special Prem Set-up' as Remarks
                                                                    FROM T_EmployeeLogLedger
                                                                    LEFT JOIN T_DayCodeMaster ON Dcm_DayCode = Ell_DayCode
                                                                 LEFT JOIN T_SpecialDayPremiumMaster on Dpm_Daycode = Ell_daycode
                                                                   And Dpm_RestDay = Ell_Restday
                                                                    WHERE Ell_PayPeriod = @CurPayPeriod
                                                                       And left(ell_locationcode,1)='D'
                                                                       And Dpm_Daycode is null

                                                                UNION

                                                                 SELECT Ell_EmployeeId
                                                                       , @CurPayPeriod as CurrentPayperiod
                                                                       , Ell_ProcessDate as ProcessDate
                                                                       , 'No Work Location Set-up' as Remarks
                                                                    FROM T_EmployeeLogLedger
                                                                    WHERE Ell_PayPeriod = @CurPayPeriod  
                                                                  And (Ell_LocationCode is null or len(rtrim(Ell_LocationCode))=0)

                                                                union 
    
                                                                select Ell_EmployeeId
                                                                   , @CurPayPeriod as CurrentPayperiod
                                                                   , Ell_ProcessDate as ProcessDate
                                                                   , 'Incorrect day code' as Remarks
                                                                 from  t_employeelogledger
                                                                inner join t_holidaymaster on Hmt_HolidayDate = Ell_Processdate
                                                                 and Hmt_ApplicCity = Ell_Locationcode
                                                                where ell_daycode <> Hmt_HolidayCode
                                                                       and Ell_PayPeriod = @CurPayPeriod";

            public const string updateEncodedOffSet = @"UPDATE T_EmployeeLogLedger
                                                    SET Ell_ForOffsetMin = IsNull(OffsetMin, 0) 
                                                    FROM T_EmployeeLogLedger
                                                    LEFT JOIN (SELECT Off_EmployeeId
                                                                , Off_ApplicableDate
                                                                , OffsetMin = Sum(Off_HoursUsed * 60)
                                                                FROM T_Offset
                                                                Where Off_ApplicableDate 
                                                                BETWEEN (Select Ppm_StartCycle From T_PayPeriodMaster Where Ppm_CycleIndicator = 'C' and Ppm_status = 'A') 
                                                                AND (Select Ppm_EndCycle From T_PayPeriodMaster Where Ppm_CycleIndicator = 'C' and Ppm_status = 'A')
                                                                GROUP BY Off_EmployeeId
                                                                , Off_ApplicableDate 
                                                               ) OffTrn ON  Off_EmployeeId = Ell_EmployeeId
                                                               AND Off_ApplicableDate = Ell_ProcessDate
                                                    WHERE Ell_PayPeriod = @CurPayPeriod
                                                     AND Ell_ForOffsetMin <> IsNull(OffsetMin, 0)";


            public const string updateEncodedOT = @"UPDATE T_EmployeeLogLedger
                                                        SET Ell_EncodedOvertimeAdvHr = Isnull(AdvOvtHr,0) 
                                                        , Ell_EncodedOvertimePostHr = Isnull(PostOvtHr,0) 
                                                        FROM T_EmployeeLogLedger
                                                        LEFT JOIN (SELECT Eot_EmployeeId
                                                        , Eot_OvertimeDate
                                                        , AdvOvtHr = Sum(Case when Eot_OvertimeType = 'A' Then Eot_OvertimeHour Else 0 End)
                                                        , PostOvtHr = Sum(Case when Eot_OvertimeType = 'P' Then Eot_OvertimeHour Else 0 End)
                                                        FROM T_EmployeeOvertime
                                                        WHERE Eot_Status in ('A', '9')
                                                        GROUP BY Eot_EmployeeId
                                                        , Eot_OvertimeDate ) OvtTrn ON  Eot_EmployeeId = Ell_EmployeeId
                                                        AND Eot_OvertimeDate = Ell_ProcessDate
                                                        WHERE Ell_PayPeriod = @CurPayPeriod
                                                        AND ( Ell_EncodedOvertimeAdvHr <> IsNull(AdvOvtHr, 0) OR
                                                        Ell_EncodedOvertimePostHr <> IsNull(PostOvtHr, 0) )";
            ////changed : 11/14/2009
            ////jccapetillo
//            public const string updateEncodedPaidLeave = @"UPDATE T_EmployeeLogLedger
//                                                        SET  Ell_EncodedPayLeaveType = CASE WHEN  IsNull(LveHour, 0) = 0 THEN '' ELSE Ell_EncodedPayLeaveType END
//				                                            , Ell_EncodedPayLeaveHr =  IsNull(LveHour, 0) 
//                                                        FROM T_EmployeeLogLedger
//                                                        LEFT JOIN ( SELECT Elt_EmployeeId
//                                                                            , Elt_LeaveDate
//                                                                            , LveHour = Sum(Elt_LeaveHour)
//                                                                    FROM T_EmployeeLeaveAvailment INNER JOIN T_LeaveTypeMaster 
//							                                            ON Ltm_LeaveType = Elt_LeaveType
//                                                                            AND Ltm_PaidLeave = 1
//                                                                    WHERE Elt_LeaveFlag = 'C' 
//                                                                        AND Elt_Status in ('A', '9')
//                                                                    GROUP BY Elt_EmployeeId
//                                                                            , Elt_LeaveDate ) LveTrn 
//						                                            ON Elt_EmployeeId = Ell_EmployeeId
//                                                                     AND Elt_LeaveDate = Ell_ProcessDate
//                                                        WHERE Ell_PayPeriod = @CurPayPeriod
//                                                            AND Ell_EncodedPayLeaveHr <> IsNull(LveHour,0)
//
//                                                        UPDATE T_EmployeeLogLedger
//                                                        SET  Ell_EncodedPayLeaveType = ''
//                                                        WHERE Ell_EncodedPayLeaveHr = 0.00
//                                                          and LEN(RTRIM(Ell_EncodedPayLeaveType)) > 0";

            public const string updateEncodedPaidLeave = @"
                                                        UPDATE T_EmployeeLogLedger
                                                        SET  Ell_EncodedPayLeaveType = ''
                                                        WHERE Ell_EncodedPayLeaveHr = 0.00
                                                          and LEN(RTRIM(Ell_EncodedPayLeaveType)) > 0";
            ////end

            ////changed : 11/14/2009
            ////jccapetillo
//            public const string updateEncodedNoPayLeave = @"UPDATE T_EmployeeLogLedger
//                                                        SET Ell_EncodedNoPayLeaveType = CASE WHEN  IsNull(LveHour, 0) = 0 THEN '' ELSE Ell_EncodedNoPayLeaveType END
//				                                            ,Ell_EncodedNoPayLeaveHr = Isnull( LveHour, 0)
//                                                        FROM T_EmployeeLogLedger
//                                                        INNER JOIN ( SELECT Elt_EmployeeId
//					                                                        , Elt_LeaveDate
//					                                                        , LveHour = Sum(Elt_LeaveHour)
//			                                                        FROM T_EmployeeLeaveAvailment 
//			                                                        INNER JOIN  T_LeaveTypeMaster ON Ltm_LeaveType = Elt_LeaveType
//					                                                        AND Ltm_PaidLeave = 0
//			                                                        WHERE Elt_LeaveFlag = 'C'
//				                                                        AND Elt_Status in ('A', '9')
//			                                                        GROUP BY Elt_EmployeeId
//					                                                        , Elt_LeaveDate ) LveTrn ON Elt_EmployeeId = Ell_EmployeeId
//								                                                        AND Elt_LeaveDate = Ell_ProcessDate
//                                                        WHERE Ell_PayPeriod = @CurPayPeriod
//	                                                        AND Ell_EncodedNoPayLeaveHr <> IsNull(LveHour, 0)
//
//                                                        UPDATE T_EmployeeLogLedger
//                                                        SET  Ell_EncodedNoPayLeaveType = ''
//                                                        WHERE Ell_EncodedNoPayLeaveHr = 0.00
//                                                        and LEN(RTRIM(Ell_EncodedNoPayLeaveType)) > 0";

            public const string updateEncodedNoPayLeave = @"
                                                        UPDATE T_EmployeeLogLedger
                                                        SET  Ell_EncodedNoPayLeaveType = ''
                                                        WHERE Ell_EncodedNoPayLeaveHr = 0.00
                                                        and LEN(RTRIM(Ell_EncodedNoPayLeaveType)) > 0";
            ////end

            public const string fetchEmployeePayrollType = @"Select Distinct Emt_EmployeeID
				                                                            ,Emt_PayrollType 
				                                                            ,Emt_LastName + ', ' + Emt_FirstName + ' ' + LEFT(Emt_MiddleName,1) + '.' [Emt_EmployeeName]
                                                            From T_EmployeeLogledger Inner join T_EmployeeMaster 
                                                            on Emt_EmployeeID = Ell_EmployeeID
                                                            Where Ell_Payperiod = @CurPayperiod";

            #endregion
            #region [For Labor Hour Adjustment]
            public const string fetchLaborHoursAdjErrorData = @"Select DIstinct Lha_PayPeriod 
                                                                From T_LaborHrsAdjustment
                                                                Left Join ( Select Emt_PayPeriod
                                                                        , Count(Emt_employeeID) as Cnt
                                                                            From T_EmployeeMasterHist 
                                                                           Group by Emt_PayPeriod ) Period on Emt_PayPeriod = Lha_PayPeriod
                                                                Where Lha_AdjustpayPeriod = @CurPayperiod
                                                                    and Emt_PayPeriod is null

                                                                Select Distinct Lha_EmployeeID
                                                                        , Emt_Lastname + ', ' + Emt_Firstname + ' ' +  Left(Emt_MiddleName, 1) + '.' [Lha_EmployeeName]
                                                                        , Lha_PayPeriod as 'Adjusted Period'
                                                                        , case when T_EmployeeMasterHist.Emt_EmployeeID is null  then 'No employee record.'
                                                                                else 'Salary rate is zero.' end as Remark
                                                                From T_LaborHrsAdjustment
                                                                        Left Join T_EmployeeMasterHist  on Emt_PayPeriod = Lha_PayPeriod
                                                                         and Emt_EmployeeID = Lha_EmployeeID 
                                                                        Left Join T_EmployeeMaster on T_EmployeeMaster.Emt_Employeeid = T_LaborHrsAdjustment.Lha_Employeeid
                                                                 Where Lha_AdjustpayPeriod = @CurPayperiod
                                                                    and (T_EmployeeMasterHist.Emt_EmployeeID is null or Isnull(T_EmployeeMasterHist.Emt_SalaryRate, 0) = 0)
                                                                Order by Emt_Lastname + ', ' + Emt_Firstname + ' ' +  Left(Emt_MiddleName, 1) + '.' ,Lha_PayPeriod";
            public const string fetchEmployeeForLaborHoursAdj = @"Select DISTINCT Lha_EmployeeId,@CurPayPeriod [Lha_PayPeriod],Emt_Lastname + ', ' + Emt_Firstname + ' ' +  Left(Emt_MiddleName, 1) + '.' [Lha_EmployeeName]
                                                                From T_LaborHrsAdjustment
                                                                JOIN	T_EmployeeMaster
                                                                ON		Emt_EmployeeID = Lha_EmployeeId
                                                                Where Lha_AdjustpayPeriod = @CurPayPeriod
                                                                AND		Lha_PayrollPost = 0";

            public const string updateEmployeeAdj = @"UPDATE T_EmployeeAdjustment
                                                        SET Ead_HrlyRate = Adj.HourlyRate,
														  Ead_RegularHr = Ead_RegularHr + Adj.RegularHr,
                                                          Ead_RegularOTHr = Ead_RegularOTHr + Adj.RegularOTHr,
                                                          Ead_RegularNDHr = Ead_RegularNDHr + Adj.RegularNDHr,
                                                          Ead_RegularOTNDHr = Ead_RegularOTNDHr + Adj.RegularOTNDHr,
                                                          Ead_RestdayHr = Ead_RestdayHr + Adj.RestdayHr,
                                                          Ead_RestdayOTHr = Ead_RestdayOTHr + Adj.RestdayOTHr,
                                                          Ead_RestdayNDHr = Ead_RestdayNDHr + Adj.RestdayNDHr,
                                                          Ead_RestdayOTNDHr = Ead_RestdayOTNDHr + Adj.RestdayOTNDHr,
                                                          Ead_LegalHolidayHr = Ead_LegalHolidayHr + Adj.LegalHolidayHr,
                                                          Ead_LegalHolidayOTHr = Ead_LegalHolidayOTHr + Adj.LegalHolidayOTHr,
                                                          Ead_LegalHolidayNDHr = Ead_LegalHolidayNDHr + Adj.LegalHolidayNDHr,
                                                          Ead_LegalHolidayOTNDHr = Ead_LegalHolidayOTNDHr + Adj.LegalHolidayOTNDHr,
                                                          Ead_SpecialHolidayHr = Ead_SpecialHolidayHr + Adj.SpecialHolidayHr,
                                                          Ead_SpecialHolidayOTHr = Ead_SpecialHolidayOTHr + Adj.SpecialHolidayOTHr,
                                                          Ead_SpecialHolidayNDHr = Ead_SpecialHolidayNDHr + Adj.SpecialHolidayNDHr,
                                                          Ead_SpecialHolidayOTNDHr = Ead_SpecialHolidayOTNDHr + Adj.SpecialHolidayOTNDHr,
                                                          Ead_PlantShutdownHr = Ead_PlantShutdownHr + Adj.PlantShutdownHr,
                                                          Ead_PlantShutdownOTHr = Ead_PlantShutdownOTHr + Adj.PlantShutdownOTHr,
                                                          Ead_PlantShutdownNDHr = Ead_PlantShutdownNDHr + Adj.PlantShutdownNDHr,
                                                          Ead_PlantShutdownOTNDHr = Ead_PlantShutdownOTNDHr + Adj.PlantShutdownOTNDHr,
                                                          Ead_CompanyHolidayHr = Ead_CompanyHolidayHr + Adj.CompanyHolidayHr,
                                                          Ead_CompanyHolidayOTHr = Ead_CompanyHolidayOTHr + Adj.CompanyHolidayOTHr,
                                                          Ead_CompanyHolidayNDHr = Ead_CompanyHolidayNDHr + Adj.CompanyHolidayNDHr,
                                                          Ead_CompanyHolidayOTNDHr = Ead_CompanyHolidayOTNDHr + Adj.CompanyHolidayOTNDHr,
                                                          Ead_RestdayLegalHolidayHr = Ead_RestdayLegalHolidayHr + Adj.RestdayLegalHolidayHr,
                                                          Ead_RestdayLegalHolidayOTHr = Ead_RestdayLegalHolidayOTHr + Adj.RestdayLegalHolidayOTHr,
                                                          Ead_RestdayLegalHolidayNDHr = Ead_RestdayLegalHolidayNDHr + Adj.RestdayLegalHolidayNDHr,
                                                          Ead_RestdayLegalHolidayOTNDHr =Ead_RestdayLegalHolidayOTNDHr  + Adj.RestdayLegalHolidayOTNDHr,
                                                          Ead_RestdaySpecialHolidayHr = Ead_RestdaySpecialHolidayHr + Adj.RestdaySpecialHolidayHr,
                                                          Ead_RestdaySpecialHolidayOTHr = Ead_RestdaySpecialHolidayOTHr + Adj.RestdaySpecialHolidayOTHr,
                                                          Ead_RestdaySpecialHolidayNDHr = Ead_RestdaySpecialHolidayNDHr + Adj.RestdaySpecialHolidayNDHr,
                                                          Ead_RestdaySpecialHolidayOTNDHr = Ead_RestdaySpecialHolidayOTNDHr + Adj.RestdaySpecialHolidayOTNDHr,
                                                          Ead_RestdayCompanyHolidayHr = Ead_RestdayCompanyHolidayHr + Adj.RestdayCompanyHolidayHr,
                                                          Ead_RestdayCompanyHolidayOTHr = Ead_RestdayCompanyHolidayOTHr + Adj.RestdayCompanyHolidayOTHr,
                                                          Ead_RestdayCompanyHolidayNDHr = Ead_RestdayCompanyHolidayNDHr + Adj.RestdayCompanyHolidayNDHr,
                                                          Ead_RestdayCompanyHolidayOTNDHr = Ead_RestdayCompanyHolidayOTNDHr + Adj.RestdayCompanyHolidayOTNDHr,
                                                          Ead_RestdayPlantShutdownHr = Ead_RestdayPlantShutdownHr + Adj.RestdayPlantShutdownHr,
                                                          Ead_RestdayPlantShutdownOTHr = Ead_RestdayPlantShutdownOTHr + Adj.RestdayPlantShutdownOTHr,
                                                          Ead_RestdayPlantShutdownNDHr = Ead_RestdayPlantShutdownNDHr + Adj.RestdayPlantShutdownNDHr,
                                                          Ead_RestdayPlantShutdownOTNDHr = Ead_RestdayPlantShutdownOTNDHr + Adj.RestdayPlantShutdownOTNDHr,
                                                          Ead_LaborHrsAdjustmentAmt = Ead_LaborHrsAdjustmentAmt + Adj.LaborHrsAdjustmentAmt,
                                                          Usr_Login = @UserLogin,
                                                          Ludatetime = GetDate()
                                                        FROM T_EmployeeAdjustment
                                                        INNER JOIN  (Select Lha_EmployeeId,Lha_HourlyRate as HourlyRate
                                                        , Sum(Lha_RegularHr) as RegularHr
                                                        , Sum(Lha_RegularOTHr) as RegularOTHr
                                                        , Sum(Lha_RegularNDHr) as RegularNDHr
                                                        , Sum(Lha_RegularOTNDHr) as RegularOTNDHr
                                                        , Sum(Lha_RestdayHr) as RestdayHr
                                                        , Sum(Lha_RestdayOTHr) as RestdayOTHr
                                                        , Sum(Lha_RestdayNDHr) as RestdayNDHr
                                                        , Sum(Lha_RestdayOTNDHr) as RestdayOTNDHr
                                                        , Sum(Lha_LegalHolidayHr) as LegalHolidayHr
                                                        , Sum(Lha_LegalHolidayOTHr) as LegalHolidayOTHr
                                                        , Sum(Lha_LegalHolidayNDHr) as LegalHolidayNDHr
                                                        , Sum(Lha_LegalHolidayOTNDHr) as LegalHolidayOTNDHr
                                                        , Sum(Lha_SpecialHolidayHr) as SpecialHolidayHr
                                                        , Sum(Lha_SpecialHolidayOTHr) as SpecialHolidayOTHr
                                                        , Sum(Lha_SpecialHolidayNDHr) as SpecialHolidayNDHr
                                                        , Sum(Lha_SpecialHolidayOTNDHr) as SpecialHolidayOTNDHr
                                                        , Sum(Lha_PlantShutdownHr) as PlantShutdownHr
                                                        , Sum(Lha_PlantShutdownOTHr) as PlantShutdownOTHr
                                                        , Sum(Lha_PlantShutdownNDHr) as PlantShutdownNDHr
                                                        , Sum(Lha_PlantShutdownOTNDHr) as PlantShutdownOTNDHr
                                                        , Sum(Lha_CompanyHolidayHr) as CompanyHolidayHr
                                                        , Sum(Lha_CompanyHolidayOTHr) as CompanyHolidayOTHr
                                                        , Sum(Lha_CompanyHolidayNDHr) as CompanyHolidayNDHr
                                                        , Sum(Lha_CompanyHolidayOTNDHr) as CompanyHolidayOTNDHr
                                                        , Sum(Lha_RestdayLegalHolidayHr) as RestdayLegalHolidayHr
                                                        , Sum(Lha_RestdayLegalHolidayOTHr) as RestdayLegalHolidayOTHr
                                                        , Sum(Lha_RestdayLegalHolidayNDHr) as RestdayLegalHolidayNDHr
                                                        , Sum(Lha_RestdayLegalHolidayOTNDHr) as RestdayLegalHolidayOTNDHr
                                                        , Sum(Lha_RestdaySpecialHolidayHr) as RestdaySpecialHolidayHr
                                                        , Sum(Lha_RestdaySpecialHolidayOTHr) as RestdaySpecialHolidayOTHr
                                                        , Sum(Lha_RestdaySpecialHolidayNDHr) as RestdaySpecialHolidayNDHr
                                                        , Sum(Lha_RestdaySpecialHolidayOTNDHr) as RestdaySpecialHolidayOTNDHr
                                                        , Sum(Lha_RestdayCompanyHolidayHr) as RestdayCompanyHolidayHr
                                                        , Sum(Lha_RestdayCompanyHolidayOTHr) as RestdayCompanyHolidayOTHr
                                                        , Sum(Lha_RestdayCompanyHolidayNDHr) as RestdayCompanyHolidayNDHr
                                                        , Sum(Lha_RestdayCompanyHolidayOTNDHr) as RestdayCompanyHolidayOTNDHr
                                                        , Sum(Lha_RestdayPlantShutdownHr) as RestdayPlantShutdownHr
                                                        , Sum(Lha_RestdayPlantShutdownOTHr) as RestdayPlantShutdownOTHr
                                                        , Sum(Lha_RestdayPlantShutdownNDHr) as RestdayPlantShutdownNDHr
                                                        , Sum(Lha_RestdayPlantShutdownOTNDHr) as RestdayPlantShutdownOTNDHr
                                                        , Sum(Lha_LaborHrsAdjustmentAmt) as LaborHrsAdjustmentAmt
                                                        From T_LaborHrsAdjustment
                                                        Where Lha_AdjustpayPeriod = @CurPayPeriod
                                                        And	  Lha_EmployeeId = @EmployeeID
                                                        Group by Lha_EmployeeId,Lha_HourlyRate)  Adj on Adj.Lha_EmployeeId = Ead_EmployeeID  
                                                        Where Ead_CurrentPayPeriod = @CurPayPeriod
                                                        And	  Ead_EmployeeID = @EmployeeID
                                                        ----------Update Lha_PayrollPost after update-------
                                                        UPDATE	T_LaborHrsAdjustment
                                                        SET		Lha_PayrollPost = 1
                                                        WHERE	Lha_EmployeeId = @EmployeeID
                                                        AND		Lha_AdjustpayPeriod = @CurPayPeriod";

            //Added by Rendell Uy - 06/18/2010
            public const string updateEmployeeAdjExt = @"UPDATE T_EmployeeAdjustmentExt
                                                          SET Ead_Filler01_Hr = Ead_Filler01_Hr + Adj.Filler01Hr
                                                            , Ead_Filler01_OTHr = Ead_Filler01_OTHr + Adj.Filler01OTHr
                                                            , Ead_Filler01_NDHr = Ead_Filler01_NDHr + Adj.Filler01NDHr
                                                            , Ead_Filler01_OTNDHr = Ead_Filler01_OTNDHr + Adj.Filler01OTNDHr
                                                            , Ead_Filler02_Hr = Ead_Filler02_Hr + Adj.Filler02Hr
                                                            , Ead_Filler02_OTHr = Ead_Filler02_OTHr + Adj.Filler02OTHr
                                                            , Ead_Filler02_NDHr = Ead_Filler02_NDHr + Adj.Filler02NDHr
                                                            , Ead_Filler02_OTNDHr = Ead_Filler02_OTNDHr + Adj.Filler02OTNDHr
                                                            , Ead_Filler03_Hr = Ead_Filler03_Hr + Adj.Filler03Hr
                                                            , Ead_Filler03_OTHr = Ead_Filler03_OTHr + Adj.Filler03OTHr
                                                            , Ead_Filler03_NDHr = Ead_Filler03_NDHr + Adj.Filler03NDHr
                                                            , Ead_Filler03_OTNDHr = Ead_Filler03_OTNDHr + Adj.Filler03OTNDHr
                                                            , Ead_Filler04_Hr = Ead_Filler04_Hr + Adj.Filler04Hr
                                                            , Ead_Filler04_OTHr = Ead_Filler04_OTHr + Adj.Filler04OTHr
                                                            , Ead_Filler04_NDHr = Ead_Filler04_NDHr + Adj.Filler04NDHr
                                                            , Ead_Filler04_OTNDHr = Ead_Filler04_OTNDHr + Adj.Filler04OTNDHr
                                                            , Ead_Filler05_Hr = Ead_Filler05_Hr + Adj.Filler05Hr
                                                            , Ead_Filler05_OTHr = Ead_Filler05_OTHr + Adj.Filler05OTHr
                                                            , Ead_Filler05_NDHr = Ead_Filler05_NDHr + Adj.Filler05NDHr
                                                            , Ead_Filler05_OTNDHr = Ead_Filler05_OTNDHr + Adj.Filler05OTNDHr
                                                            , Ead_Filler06_Hr = Ead_Filler06_Hr + Adj.Filler06Hr
                                                            , Ead_Filler06_OTHr = Ead_Filler06_OTHr + Adj.Filler06OTHr
                                                            , Ead_Filler06_NDHr = Ead_Filler06_NDHr + Adj.Filler06NDHr
                                                            , Ead_Filler06_OTNDHr = Ead_Filler06_OTNDHr + Adj.Filler06OTNDHr
                                                            , Usr_Login = @UserLogin
                                                            , Ludatetime = GetDate()
                                                        FROM T_EmployeeAdjustmentExt
                                                        INNER JOIN (SELECT A.Lha_EmployeeId
                                                            , B.Lha_HourlyRate
                                                            , SUM(A.Lha_Filler01_Hr) AS Filler01Hr
                                                            , SUM(A.Lha_Filler01_OTHr) AS Filler01OTHr
                                                            , SUM(A.Lha_Filler01_NDHr) AS Filler01NDHr
                                                            , SUM(A.Lha_Filler01_OTNDHr) AS Filler01OTNDHr
                                                            , SUM(A.Lha_Filler02_Hr) AS Filler02Hr
                                                            , SUM(A.Lha_Filler02_OTHr) AS Filler02OTHr
                                                            , SUM(A.Lha_Filler02_NDHr) AS Filler02NDHr
                                                            , SUM(A.Lha_Filler02_OTNDHr) AS Filler02OTNDHr
                                                            , SUM(A.Lha_Filler03_Hr) AS Filler03Hr
                                                            , SUM(A.Lha_Filler03_OTHr) AS Filler03OTHr
                                                            , SUM(A.Lha_Filler03_NDHr) AS Filler03NDHr
                                                            , SUM(A.Lha_Filler03_OTNDHr) AS Filler03OTNDHr
                                                            , SUM(A.Lha_Filler04_Hr) AS Filler04Hr
                                                            , SUM(A.Lha_Filler04_OTHr) AS Filler04OTHr
                                                            , SUM(A.Lha_Filler04_NDHr) AS Filler04NDHr
                                                            , SUM(A.Lha_Filler04_OTNDHr) AS Filler04OTNDHr
                                                            , SUM(A.Lha_Filler05_Hr) AS Filler05Hr
                                                            , SUM(A.Lha_Filler05_OTHr) AS Filler05OTHr
                                                            , SUM(A.Lha_Filler05_NDHr) AS Filler05NDHr
                                                            , SUM(A.Lha_Filler05_OTNDHr) AS Filler05OTNDHr
                                                            , SUM(A.Lha_Filler06_Hr) AS Filler06Hr
                                                            , SUM(A.Lha_Filler06_OTHr) AS Filler06OTHr
                                                            , SUM(A.Lha_Filler06_NDHr) AS Filler06NDHr
                                                            , SUM(A.Lha_Filler06_OTNDHr) AS Filler06OTNDHr
                                                            FROM T_LaborHrsAdjustmentExt A
                                                            INNER JOIN T_LaborHrsAdjustment B 
                                                            ON A.Lha_EmployeeId = B.Lha_EmployeeId
                                                                AND A.Lha_AdjustpayPeriod = B.Lha_AdjustpayPeriod
                                                                AND A.Lha_PayPeriod = B.Lha_PayPeriod
                                                                AND A.Lha_ProcessDate = B.Lha_ProcessDate
                                                            WHERE A.Lha_AdjustpayPeriod = @CurPayPeriod
                                                                AND	A.Lha_EmployeeId = @EmployeeID
                                                            GROUP BY A.Lha_EmployeeId, B.Lha_HourlyRate) Adj 
                                                        ON Ead_EmployeeId = Adj.Lha_EmployeeId
                                                        WHERE Ead_CurrentPayPeriod = @CurPayPeriod
                                                            AND	Ead_EmployeeID = @EmployeeID";

            public const string insertEmployeeAdj = @"INSERT INTO T_EmployeeAdjustment SELECT
                                                        Lha_EmployeeId
                                                        , Lha_AdjustpayPeriod
                                                        , 0
                                                        , Lha_HourlyRate
                                                        , Sum(Lha_RegularHr) as RegularHr
                                                        , Sum(Lha_RegularOTHr) as RegularOTHr
                                                        , Sum(Lha_RegularNDHr) as RegularNDHr
                                                        , Sum(Lha_RegularOTNDHr) as RegularOTNDHr
                                                        , Sum(Lha_RestdayHr) as RestdayHr
                                                        , Sum(Lha_RestdayOTHr) as RestdayOTHr
                                                        , Sum(Lha_RestdayNDHr) as RestdayNDHr
                                                        , Sum(Lha_RestdayOTNDHr) as RestdayOTNDHr
                                                        , Sum(Lha_LegalHolidayHr) as LegalHolidayHr
                                                        , Sum(Lha_LegalHolidayOTHr) as LegalHolidayOTHr
                                                        , Sum(Lha_LegalHolidayNDHr) as LegalHolidayNDHr
                                                        , Sum(Lha_LegalHolidayOTNDHr) as LegalHolidayOTNDHr
                                                        , Sum(Lha_SpecialHolidayHr) as SpecialHolidayHr
                                                        , Sum(Lha_SpecialHolidayOTHr) as SpecialHolidayOTHr
                                                        , Sum(Lha_SpecialHolidayNDHr) as SpecialHolidayNDHr
                                                        , Sum(Lha_SpecialHolidayOTNDHr) as SpecialHolidayOTNDHr
                                                        , Sum(Lha_PlantShutdownHr) as PlantShutdownHr
                                                        , Sum(Lha_PlantShutdownOTHr) as PlantShutdownOTHr
                                                        , Sum(Lha_PlantShutdownNDHr) as PlantShutdownNDHr
                                                        , Sum(Lha_PlantShutdownOTNDHr) as PlantShutdownOTNDHr
                                                        , Sum(Lha_CompanyHolidayHr) as CompanyHolidayHr
                                                        , Sum(Lha_CompanyHolidayOTHr) as CompanyHolidayOTHr
                                                        , Sum(Lha_CompanyHolidayNDHr) as CompanyHolidayNDHr
                                                        , Sum(Lha_CompanyHolidayOTNDHr) as CompanyHolidayOTNDHr
                                                        , Sum(Lha_RestdayLegalHolidayHr) as RestdayLegalHolidayHr
                                                        , Sum(Lha_RestdayLegalHolidayOTHr) as RestdayLegalHolidayOTHr
                                                        , Sum(Lha_RestdayLegalHolidayNDHr) as RestdayLegalHolidayNDHr
                                                        , Sum(Lha_RestdayLegalHolidayOTNDHr) as RestdayLegalHolidayOTNDHr
                                                        , Sum(Lha_RestdaySpecialHolidayHr) as RestdaySpecialHolidayHr
                                                        , Sum(Lha_RestdaySpecialHolidayOTHr) as RestdaySpecialHolidayOTHr
                                                        , Sum(Lha_RestdaySpecialHolidayNDHr) as RestdaySpecialHolidayNDHr
                                                        , Sum(Lha_RestdaySpecialHolidayOTNDHr) as RestdaySpecialHolidayOTNDHr
                                                        , Sum(Lha_RestdayCompanyHolidayHr) as RestdayCompanyHolidayHr
                                                        , Sum(Lha_RestdayCompanyHolidayOTHr) as RestdayCompanyHolidayOTHr
                                                        , Sum(Lha_RestdayCompanyHolidayNDHr) as RestdayCompanyHolidayNDHr
                                                        , Sum(Lha_RestdayCompanyHolidayOTNDHr) as RestdayCompanyHolidayOTNDHr
                                                        , Sum(Lha_RestdayPlantShutdownHr) as RestdayPlantShutdownHr
                                                        , Sum(Lha_RestdayPlantShutdownOTHr) as RestdayPlantShutdownOTHr
                                                        , Sum(Lha_RestdayPlantShutdownNDHr) as RestdayPlantShutdownNDHr
                                                        , Sum(Lha_RestdayPlantShutdownOTNDHr) as RestdayPlantShutdownOTNDHr
                                                        , Sum(Lha_LaborHrsAdjustmentAmt) as LaborHrsAdjustmentAmt
                                                        , 0,0,'',GETDATE()
                                                        From T_LaborHrsAdjustment
                                                        Where Lha_AdjustpayPeriod = @CurPayPeriod
                                                        And	  Lha_EmployeeId = @EmployeeID
                                                        Group by Lha_EmployeeId, Lha_AdjustpayPeriod, Lha_HourlyRate
                                                        ----------Update Lha_PayrollPost after insert-------
                                                        UPDATE	T_LaborHrsAdjustment
                                                        SET		Lha_PayrollPost = 1
                                                        WHERE	Lha_EmployeeId = @EmployeeID
                                                        AND		Lha_AdjustpayPeriod = @CurPayPeriod";

            //Added by Rendell Uy - 06/18/2010
            public const string insertEmployeeAdjExt = @"INSERT INTO T_EmployeeAdjustmentExt 
                                                            SELECT A.Lha_EmployeeId
                                                            , A.Lha_PayPeriod
                                                            , SUM(A.Lha_Filler01_Hr) AS Filler01_Hr
                                                            , SUM(A.Lha_Filler01_OTHr) AS Filler01_OTHr
                                                            , SUM(A.Lha_Filler01_NDHr) AS Filler01_NDHr
                                                            , SUM(A.Lha_Filler01_OTNDHr) AS Filler01_OTNDHr
                                                            , SUM(A.Lha_Filler02_Hr) AS Filler02_Hr
                                                            , SUM(A.Lha_Filler02_OTHr) AS Filler02_OTHr
                                                            , SUM(A.Lha_Filler02_NDHr) AS Filler02_NDHr
                                                            , SUM(A.Lha_Filler02_OTNDHr) AS Filler02_OTNDHr
                                                            , SUM(A.Lha_Filler03_Hr) AS Filler03_Hr
                                                            , SUM(A.Lha_Filler03_OTHr) AS Filler03_OTHr
                                                            , SUM(A.Lha_Filler03_NDHr) AS Filler03_NDHr
                                                            , SUM(A.Lha_Filler03_OTNDHr) AS Filler03_OTNDHr
                                                            , SUM(A.Lha_Filler04_Hr) AS Filler04_Hr
                                                            , SUM(A.Lha_Filler04_OTHr) AS Filler04_OTHr
                                                            , SUM(A.Lha_Filler04_NDHr) AS Filler04_NDHr
                                                            , SUM(A.Lha_Filler04_OTNDHr) AS Filler04_OTNDHr
                                                            , SUM(A.Lha_Filler05_Hr) AS Filler05_Hr
                                                            , SUM(A.Lha_Filler05_OTHr) AS Filler05_OTHr
                                                            , SUM(A.Lha_Filler05_NDHr) AS Filler05_NDHr
                                                            , SUM(A.Lha_Filler05_OTNDHr) AS Filler05_OTNDHr
                                                            , SUM(A.Lha_Filler06_Hr) AS Filler06_Hr
                                                            , SUM(A.Lha_Filler06_OTHr) AS Filler06_OTHr
                                                            , SUM(A.Lha_Filler06_NDHr) AS Filler06_NDHr
                                                            , SUM(A.Lha_Filler06_OTNDHr) AS Filler06_OTNDHr
                                                            , @UserLogin
                                                            , GetDate()
                                                            FROM T_LaborHrsAdjustmentExt A
                                                            WHERE A.Lha_AdjustpayPeriod = @CurPayPeriod
                                                            AND	A.Lha_EmployeeId = @EmployeeID
                                                            GROUP BY A.Lha_EmployeeId, A.Lha_AdjustpayPeriod, A.Lha_PayPeriod";

            #endregion
            #region [For Allowance Posting]
            public const string fetchEmployeeForLaborHoursAlw = @"SELECT DISTINCT Elr_EmployeeId,Elr_LeaveAmt,Acm_AllowanceCode Elr_AllowanceCode,Emt_Lastname + ', ' + Emt_Firstname + ' ' +  Left(Emt_MiddleName, 1) + '.' [Elr_EmployeeName]	, Elr_LeaveType, Elr_TaxClass
                                                                    FROM			T_EmployeeLeaveRefund
                                                                    INNER JOIN		T_AllowanceCodeMaster
                                                                    ON				LEFT(Acm_AllowanceCode,2) = Elr_LeaveType
                                                                    AND				Acm_LinkToLeave = 1
                                                                    AND				Acm_TaxClass = Elr_TaxClass
                                                                    JOIN			T_EmployeeMaster
                                                                    ON				Emt_EmployeeID = Elr_EmployeeId
                                                                    WHERE			Elr_CurrentPayPeriod = @CurPayPeriod
                                                                    AND				Elr_Post =0
                                                                    -----------------------------------------------------
                                                                    SELECT DISTINCT Elr_LeaveType 
                                                                    FROM			T_EmployeeLeaveRefund
                                                                    LEFT JOIN		T_AllowanceCodeMaster
                                                                    ON				LEFT(Acm_AllowanceCode,2) = Elr_LeaveType
                                                                    AND				Acm_LinkToLeave = 1
                                                                    AND				Acm_TaxClass = Elr_TaxClass
                                                                    JOIN			T_EmployeeMaster
                                                                    ON				Emt_EmployeeID = Elr_EmployeeId
                                                                    AND				Acm_AllowanceCode IS NULL";
            public const string updateEmployeeAlw = @"Update T_EmployeeAllowance
			                                             Set Eal_AllowanceAmt = Eal_AllowanceAmt + @AllowanceAmt
	                                                        ,Usr_Login = @UserLogin
	                                                        ,Ludatetime = GetDate()
                                                       Where Eal_EmployeeId = @EmployeeId
			                                             And Eal_CurrentPayPeriod = @CurPayPeriod
			                                             And Eal_AllowanceCode = @AllowanceCode
                                                       -----------------------------------------------------
                                                       Update T_EmployeeLeaveRefund
                                                         Set Elr_Post = 1
                                                       Where Elr_EmployeeID = @EmployeeId
                                                         And Elr_CurrentPayPeriod = @CurPayPeriod
                                                         And Elr_LeaveType = @LeaveType
                                                         And Elr_TaxClass = @TaxClass";
            public const string insertEmployeeAlw = @"Insert Into T_EmployeeAllowance Values (@EmployeeID
			                                                ,@CurPayPeriod
			                                                ,@AllowanceCode
			                                                ,'False'
			                                                ,@AllowanceAmt
			                                                ,@UserLogin
			                                                ,Getdate())
                                                       -----------------------------------------------------
                                                       Update T_EmployeeLeaveRefund
                                                         Set Elr_Post = 1
                                                       Where Elr_EmployeeID = @EmployeeId
                                                         And Elr_CurrentPayPeriod = @CurPayPeriod
                                                         And Elr_LeaveType = @LeaveType
                                                         And Elr_TaxClass = @TaxClass";
            public const string fetchEmployeeFor13thMonthAlw = @"SELECT DISTINCT Elr_EmployeeId,Elr_LeaveAmt,Acm_AllowanceCode Elr_AllowanceCode,Emt_Lastname + ', ' + Emt_Firstname + ' ' +  Left(Emt_MiddleName, 1) + '.' [Elr_EmployeeName]
                                                                    FROM			T_EmployeeLeaveRefund
                                                                    INNER JOIN		T_AllowanceCodeMaster
                                                                    ON				LEFT(Acm_AllowanceCode,2) = Elr_LeaveType
                                                                    AND				Acm_LinkToLeave = 1
                                                                    AND				Acm_TaxClass = Elr_TaxClass
                                                                    JOIN			T_EmployeeMaster
                                                                    ON				Emt_EmployeeID = Elr_EmployeeId
                                                                    WHERE			Elr_CurrentPayPeriod = @CurPayPeriod
                                                                    AND				Elr_Post =0
                                                                    -----------------------------------------------------
                                                                    SELECT DISTINCT Elr_LeaveType 
                                                                    FROM			T_EmployeeLeaveRefund
                                                                    LEFT JOIN		T_AllowanceCodeMaster
                                                                    ON				LEFT(Acm_AllowanceCode,2) = Elr_LeaveType
                                                                    AND				Acm_LinkToLeave = 1
                                                                    AND				Acm_TaxClass = Elr_TaxClass
                                                                    JOIN			T_EmployeeMaster
                                                                    ON				Emt_EmployeeID = Elr_EmployeeId
                                                                    AND				Acm_AllowanceCode IS NULL";
            #endregion
            #region [For Labor Hours Adjustment Calculation]
            public const string fetchEmployeeForLaborHoursCalc = @"SELECT DISTINCT Ell_EmployeeId ,Emt_Lastname + ', ' + Emt_Firstname + ' ' +  Left(Emt_MiddleName, 1) + '.' [Ell_EmployeeName] 
                                                                    FROM	T_EmployeeLogLedgerTrail 
                                                                    JOIN	T_EmployeeMaster
                                                                    ON		Emt_EmployeeID = Ell_EmployeeId
                                                                    WHERE	Ell_AdjustpayPeriod = @CurPayPeriod";
            public const string insertLaborHoursAdjRecords = @"INSERT INTO T_LaborHrsAdjustment
                                                               SELECT Ell_EmployeeId 
                                                                    , Ell_AdjustpayPeriod
                                                                    , Ell_PayPeriod
                                                                    , Ell_ProcessDate  
                                                                    , Ell_RegularHr = 0.00  
                                                                    , Ell_RegularOTHr = 0.00 
                                                                    , Ell_RegularNDHr = 0.00
                                                                    , Ell_RegularOTNDHr = 0.00
                                                                    , Ell_RestdayHr = 0.00  
                                                                    , Ell_RestdayOTHr = 0.00  
                                                                    , Ell_RestdayNDHr = 0.00 
                                                                    , Ell_RestdayOTNDHr = 0.00 
                                                                    , Ell_LegalHolidayHr = 0.00  
                                                                    , Ell_LegalHolidayOTHr = 0.00  
                                                                    , Ell_LegalHolidayNDHr = 0.00 
                                                                    , Ell_LegalHolidayOTNDHr = 0.00 
                                                                    , Ell_SpecialHolidayHr = 0.00  
                                                                    , Ell_SpecialHolidayOTHr = 0.00  
                                                                    , Ell_SpecialHolidayNDHr = 0.00 
                                                                    , Ell_SpecialHolidayOTNDHr = 0.00 
                                                                    , Ell_PlantShutdownHr = 0.00  
                                                                    , Ell_PlantShutdownOTHr = 0.00  
                                                                    , Ell_PlantShutdownNDHr = 0.00  
                                                                    , Ell_PlantShutdownOTNDHr = 0.00  
                                                                    , Ell_CompanyHolidayHr = 0.00  
                                                                    , Ell_CompanyHolidayOTHr = 0.00  
                                                                    , Ell_CompanyHolidayNDHr = 0.00  
                                                                    , Ell_CompanyHolidayOTNDHr = 0.00         
                                                                    , Ell_RestdayLegalHolidayHr = 0.00  
                                                                    , Ell_RestdayLegalHolidayOTHr = 0.00  
                                                                    , Ell_RestdayLegalHolidayNDHr = 0.00
                                                                    , Ell_RestdayLegalHolidayOTNDHr = 0.00
                                                                    , Ell_RestdaySpecialHolidayHr = 0.00  
                                                                    , Ell_RestdaySpecialHolidayOTHr = 0.00
                                                                    , Ell_RestdaySpecialHolidayNDHr = 0.00 
                                                                    , Ell_RestdaySpecialHolidayOTNDHr = 0.00
                                                                    , Ell_RestdayCompanyHolidayHr = 0.00
                                                                    , Ell_RestdayCompanyHolidayOTHr = 0.00
                                                                    , Ell_RestdayCompanyHolidayNDHr = 0.00
                                                                    , Ell_RestdayCompanyHolidayOTNDHr = 0.00
                                                                    , Lha_RestdayPlantShutdownHr = 0.00
                                                                    , Lha_RestdayPlantShutdownOTHr = 0.00
                                                                    , Lha_RestdayPlantShutdownNDHr = 0.00
                                                                    , Lha_RestdayPlantShutdownOTNDHr = 0.00
                                                                    , Ell_LaborHrsAdjustmentAmt =0.00
                                                                    , 0 as SalaryRate
                                                                    , 0 as HourlyRate
                                                                    , Ell_PayrollPost = 0
                                                                    , Usr_Login  = @UserLogin
                                                                    , Ludatetime = getdate() 
                                                                FROM	T_EmployeeLogLedgerTrail A
	                                                            LEFT JOIN T_LaborHrsAdjustment B
	                                                            ON		Ell_EmployeeId = Lha_EmployeeId
	                                                            AND		Ell_AdjustpayPeriod = Lha_AdjustpayPeriod
	                                                            AND		Ell_PayPeriod = Lha_PayPeriod
	                                                            AND		Ell_ProcessDate = Lha_ProcessDate
                                                                WHERE	Ell_AdjustpayPeriod = @CurPayPeriod
	                                                            AND		Lha_EmployeeId IS NULL";
            #region [Prvious calculateAdjHours query]
//            public const string calculateAdjHours = @"  
//                                                        DECLARE @MAXABSHR AS decimal
//                                                        SET @MAXABSHR = (select Pmt_NumericValue from t_parametermaster
//                                                         where pmt_parameterid = 'MAXABSHR')
//                                                        
//                                                        
//                                                        UPDATE T_LaborHrsAdjustment  
//                                                        SET  Lha_RegularHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//						                                                      ELSE  
//								                                                     0.00  
//						                                                      END),  
//                                                            Lha_RegularOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//							                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//						                                                       ELSE  
//								                                                    0.00  
//						                                                       END),  
//		                                                    Lha_RegularNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//						                                                      ELSE  
//								                                                     0.00  
//						                                                      END),  
//                                                            Lha_RegularOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//							                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//						                                                       ELSE  
//								                                                    0.00  
//						                                                       END),  
//                                                            Lha_RestdayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//						                                                     ELSE  
//								                                                     0.00  
//						                                                     END),  
//                                                            Lha_RestdayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                    (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//							                                                    ELSE  
//								                                                    0.00  
//							                                                    END),  
//		                                                    Lha_RestdayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//						                                                     ELSE  
//								                                                     0.00  
//						                                                     END),  
//                                                            Lha_RestdayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                    (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//							                                                    ELSE  
//								                                                    0.00  
//							                                                    END),  
//                                                            Lha_LegalHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                      (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//							                                                      ELSE  
//								                                                      0.00  
//							                                                      END),  
//                                                            Lha_LegalHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                     (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//								                                                     ELSE  
//									                                                     0.00  
//								                                                    END),  
//		                                                    Lha_LegalHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                      (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//							                                                      ELSE  
//								                                                      0.00  
//							                                                      END),  
//                                                            Lha_LegalHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                     (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//								                                                     ELSE  
//									                                                     0.00  
//								                                                    END),  
//                                                        
//                                                            Lha_SpecialHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//								                                                    ELSE  
//									                                                    0.00  
//								                                                    END),  
//                                                            Lha_SpecialHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//								                                                      ELSE  
//									                                                       0.00  
//								                                                      END),  
//                                                           Lha_SpecialHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//								                                                    ELSE  
//									                                                    0.00  
//								                                                    END),  
//                                                            Lha_SpecialHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//								                                                      ELSE  
//									                                                       0.00  
//								                                                      END),  
//                                                            Lha_PlantShutdownHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    ((T_EmployeeLogLedgerHist.Ell_RegularHour + T_EmployeeLogLedgerHist.Ell_LeaveHour) - (T_EmployeeLogLedgerTrail.Ell_RegularHour + T_EmployeeLogLedgerTrail.Ell_LeaveHour))  
//							                                                       ELSE  
//									                                                    0.00  
//							                                                       END),  
//                                                            Lha_PlantShutdownOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//								                                                     ELSE  
//									                                                       0.00  
//							                                                         END),  
//                                                           Lha_PlantShutdownNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour )  
//							                                                       ELSE  
//									                                                    0.00  
//							                                                       END),  
//                                                            Lha_PlantShutdownOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//								                                                     ELSE  
//									                                                       0.00  
//							                                                         END),  
//		                                                    Lha_RestdayLegalHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayLegalHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),  
//		                                                    Lha_RestdayLegalHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayLegalHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),  
//		                                                    Lha_RestdaySpecialHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdaySpecialHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),  
//		                                                    Lha_RestdaySpecialHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdaySpecialHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),  
//		                                                    Lha_RestdayCompanyHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdayCompanyHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),  
//		                                                    Lha_RestdayCompanyHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdayCompanyHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),
//                                                            Lha_RestdayPlantShutdownHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayPlantShutdownOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),  
//		                                                    Lha_RestdayPlantShutdownNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayPlantShutdownOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),
//                                                            Lha_CompanyHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                 (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//                                                                                            ELSE 0.00
//                                                                                           END),
//                                                             Lha_CompanyHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                  (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//                                                                                            ELSE 0.00
//                                                                                           END),
//                                                             Lha_CompanyHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//                                                                                            ELSE 0.00
//                                                                                           END),
//                                                             Lha_CompanyHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                     (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//                                                                                            ELSE 0.00
//                                                                                           END)
//                                                        FROM T_LaborHrsAdjustment  
//                                                        INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
//                                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
//                                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
//                                                        INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
//                                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
//                                                        WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = @CurPayPeriod
//
//                                                        --gcd 11.28.2009
//                                                        UPDATE T_LaborHrsAdjustment  
//                                                        SET  Lha_RegularHr = Lha_RegularHr + (T_EmployeeLogLedgerTrail.Ell_AbsentHour - T_EmployeeLogLedgerHist.Ell_AbsentHour)
//                                                        FROM T_LaborHrsAdjustment  
//                                                        INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
//                                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
//                                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
//                                                        INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
//                                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
//                                                        WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = @CurPayPeriod
//                                                        and T_EmployeeLogLedgerTrail.Ell_LeaveHour <> T_EmployeeLogLedgerHist.Ell_LeaveHour
//                                                        and T_EmployeeLogLedgerTrail.Ell_AbsentHour <> T_EmployeeLogLedgerHist.Ell_AbsentHour
//                                                        and T_EmployeeLogLedgerTrail.Ell_RegularHour = T_EmployeeLogLedgerHist.Ell_RegularHour
//
//                                                        --ADDED BY KEVIN 08082009 - TO ADD MAXIMUM ABSENT HOURS ADJUSTMENT
//
//                                                        UPDATE T_LaborHrsAdjustment  
//                                                        SET Lha_RegularHr = @MAXABSHR
//                                                        WHERE Lha_AdjustpayPeriod = @CurPayPeriod
//                                                        AND Lha_RegularHr > @MAXABSHR
//
//                                                        --END OF ADD 08082009 - KEVIN
//
//                                                        --ADDED BY JAN 11242009 - 
//
//                                                        Update T_LaborHrsAdjustment
//                                                        Set lha_regularhr = ell_absenthour
//                                                        From T_LaborHrsAdjustment
//                                                        Inner Join t_employeelogledgertrail on ell_employeeid = lha_employeeid
//                                                          and ell_processdate = lha_processdate
//                                                        where lha_regularhr > ell_absenthour
//                                                          and ell_absenthour = (@MAXABSHR/2)
//                                                          and lha_adjustpayperiod=@CurPayPeriod
//
//                                                        --END OF ADD 11242009 - JAN";
            #endregion

            #region [Previous calculateAdjHours query]
//            public const string calculateAdjHours = @"  
//                                                        DECLARE @MAXABSHR AS decimal
//                                                        SET @MAXABSHR = (select Pmt_NumericValue from t_parametermaster
//                                                         where pmt_parameterid = 'MAXABSHR')
//                                                        
//                                                        
//                                                        UPDATE T_LaborHrsAdjustment  
//                                                        SET  Lha_RegularHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//						                                                      ELSE  
//								                                                     0.00  
//						                                                      END),  
//                                                            Lha_RegularOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//							                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//						                                                       ELSE  
//								                                                    0.00  
//						                                                       END),  
//		                                                    Lha_RegularNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//						                                                      ELSE  
//								                                                     0.00  
//						                                                      END),  
//                                                            Lha_RegularOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//							                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//						                                                       ELSE  
//								                                                    0.00  
//						                                                       END),  
//                                                            Lha_RestdayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//						                                                     ELSE  
//								                                                     0.00  
//						                                                     END),  
//                                                            Lha_RestdayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                    (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//							                                                    ELSE  
//								                                                    0.00  
//							                                                    END),  
//		                                                    Lha_RestdayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                     (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//						                                                     ELSE  
//								                                                     0.00  
//						                                                     END),  
//                                                            Lha_RestdayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//								                                                    (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//							                                                    ELSE  
//								                                                    0.00  
//							                                                    END),  
//                                                            Lha_LegalHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                      (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//							                                                      ELSE  
//								                                                      0.00  
//							                                                      END),  
//                                                            Lha_LegalHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                     (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//								                                                     ELSE  
//									                                                     0.00  
//								                                                    END),  
//		                                                    Lha_LegalHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//								                                                      (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//							                                                      ELSE  
//								                                                      0.00  
//							                                                      END),  
//                                                            Lha_LegalHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                     (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//								                                                     ELSE  
//									                                                     0.00  
//								                                                    END),  
//                                                        
//                                                            Lha_SpecialHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//								                                                    ELSE  
//									                                                    0.00  
//								                                                    END),  
//                                                            Lha_SpecialHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//								                                                      ELSE  
//									                                                       0.00  
//								                                                      END),  
//                                                           Lha_SpecialHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//								                                                    ELSE  
//									                                                    0.00  
//								                                                    END),  
//                                                            Lha_SpecialHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//								                                                      ELSE  
//									                                                       0.00  
//								                                                      END),  
//                                                            Lha_PlantShutdownHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    ((T_EmployeeLogLedgerHist.Ell_RegularHour + T_EmployeeLogLedgerHist.Ell_LeaveHour) - (T_EmployeeLogLedgerTrail.Ell_RegularHour + T_EmployeeLogLedgerTrail.Ell_LeaveHour))  
//							                                                       ELSE  
//									                                                    0.00  
//							                                                       END),  
//                                                            Lha_PlantShutdownOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//								                                                     ELSE  
//									                                                       0.00  
//							                                                         END),  
//                                                           Lha_PlantShutdownNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour )  
//							                                                       ELSE  
//									                                                    0.00  
//							                                                       END),  
//                                                            Lha_PlantShutdownOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//									                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//								                                                     ELSE  
//									                                                       0.00  
//							                                                         END),  
//		                                                    Lha_RestdayLegalHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayLegalHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),  
//		                                                    Lha_RestdayLegalHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayLegalHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),  
//		                                                    Lha_RestdaySpecialHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdaySpecialHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),  
//		                                                    Lha_RestdaySpecialHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdaySpecialHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),  
//		                                                    Lha_RestdayCompanyHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdayCompanyHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),  
//		                                                    Lha_RestdayCompanyHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//										                                                    ELSE  
//											                                                    0.00  
//										                                                    END),  
//		                                                    Lha_RestdayCompanyHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//											                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//										                                                     ELSE  
//											                                                       0.00  
//										                                                     END),
//                                                            Lha_RestdayPlantShutdownHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayPlantShutdownOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),  
//		                                                    Lha_RestdayPlantShutdownNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//									                                                    ELSE  
//										                                                    0.00  
//									                                                     END),  
//                                                            Lha_RestdayPlantShutdownOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
//										                                                       (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//									                                                       ELSE  
//										                                                       0.00  
//									                                                       END),
//                                                            Lha_CompanyHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                 (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
//                                                                                            ELSE 0.00
//                                                                                           END),
//                                                             Lha_CompanyHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                  (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
//                                                                                            ELSE 0.00
//                                                                                           END),
//                                                             Lha_CompanyHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                    (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
//                                                                                            ELSE 0.00
//                                                                                           END),
//                                                             Lha_CompanyHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
//                                                                                                     (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
//                                                                                            ELSE 0.00
//                                                                                           END)
//                                                        FROM T_LaborHrsAdjustment  
//                                                        INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
//                                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
//                                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
//                                                        INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
//                                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
//                                                        WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = @CurPayPeriod
//
//                                                        --gcd 11.28.2009
//                                                        UPDATE T_LaborHrsAdjustment  
//                                                        SET  Lha_RegularHr = Lha_RegularHr + (T_EmployeeLogLedgerTrail.Ell_AbsentHour - T_EmployeeLogLedgerHist.Ell_AbsentHour)
//                                                        FROM T_LaborHrsAdjustment  
//                                                        INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
//                                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
//                                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
//                                                        INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
//                                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
//                                                        WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = @CurPayPeriod
//                                                        and T_EmployeeLogLedgerTrail.Ell_LeaveHour <> T_EmployeeLogLedgerHist.Ell_LeaveHour
//                                                        and T_EmployeeLogLedgerTrail.Ell_AbsentHour <> T_EmployeeLogLedgerHist.Ell_AbsentHour
//                                                        and T_EmployeeLogLedgerTrail.Ell_RegularHour = T_EmployeeLogLedgerHist.Ell_RegularHour
//
//                                                        --ADDED BY KEVIN 08082009 - TO ADD MAXIMUM ABSENT HOURS ADJUSTMENT
//
//                                                        UPDATE T_LaborHrsAdjustment  
//                                                        SET Lha_RegularHr = case when Lha_RegularHr < 0 then @MAXABSHR*-1 else @MAXABSHR end 
//                                                        WHERE Lha_AdjustpayPeriod = @CurPayPeriod
//                                                        AND Abs(Lha_RegularHr) > @MAXABSHR
//
//                                                        --END OF ADD 08082009 - KEVIN
//
//                                                        --ADDED BY JAN 11242009 - 
//                                                        Update T_LaborHrsAdjustment
//                                                        Set lha_regularhr = ell_absenthour
//                                                        From T_LaborHrsAdjustment
//                                                        Inner Join t_employeelogledgertrail on ell_employeeid = lha_employeeid
//                                                          and ell_processdate = lha_processdate
//                                                        where lha_regularhr > ell_absenthour
//                                                          and ell_absenthour = (@MAXABSHR/2)
//                                                          and lha_adjustpayperiod=@CurPayPeriod
//
//                                                        --ADDED 01132010
//
//                                                        -- for half day leaves
//                                                        Update T_LaborHrsAdjustment
//                                                        Set lha_regularhr = (@MAXABSHR/2) * -1
//                                                        From T_LaborHrsAdjustment
//                                                        Inner Join t_employeelogledgertrail on t_employeelogledgertrail.ell_employeeid = lha_employeeid
//                                                          and t_employeelogledgertrail.ell_processdate = lha_processdate
//                                                        Inner Join t_employeelogledgerhist on t_employeelogledgerhist.ell_employeeid = lha_employeeid
//                                                          and t_employeelogledgerhist.ell_processdate = lha_processdate
//                                                        where t_employeelogledgertrail.Ell_RegularHour > t_employeelogledgerhist.Ell_RegularHour
//                                                                and t_employeelogledgerhist.Ell_RegularHour <  t_employeelogledgerhist.Ell_LeaveHour
//                                                                and t_employeelogledgerhist.Ell_RegularHour + t_employeelogledgerhist.Ell_LeaveHour <> t_employeelogledgerhist.Ell_ShiftMin/60.00
//                                                                and t_employeelogledgerhist.Ell_LeaveHour >= (@MAXABSHR/2)
//                                                                and t_employeelogledgerhist.Ell_LeaveHour <= @MAXABSHR  
//                                                                and t_employeelogledgerhist.ell_absenthour =  (@MAXABSHR/2)
//                                                                and t_employeelogledgertrail.Ell_LeaveHour = 0
//                                                                and lha_adjustpayperiod=@CurPayPeriod
//  
//                                                        --With Adjusted leave and punches
//                                                        Update T_LaborHrsAdjustment
//                                                        Set lha_regularhr = 0
//                                                        From T_LaborHrsAdjustment
//                                                        Inner Join t_employeelogledgertrail on t_employeelogledgertrail.ell_employeeid = lha_employeeid
//                                                          and t_employeelogledgertrail.ell_processdate = lha_processdate
//														  and t_employeelogledgertrail.ell_adjustpayperiod = @CurPayPeriod
//                                                        Inner Join t_employeelogledgerhist on t_employeelogledgerhist.ell_employeeid = lha_employeeid
//                                                          and t_employeelogledgerhist.ell_processdate = lha_processdate
//                                                        where t_employeelogledgerhist.ell_regularhour + t_employeelogledgerhist.ell_leavehour = t_employeelogledgertrail.ell_regularhour 
//                                                                and t_employeelogledgertrail.ell_leavehour = 0
//                                                                and t_employeelogledgerhist.ell_absenthour = 0
//                                                                and t_employeelogledgertrail.ell_absenthour = 0
//                                                                and lha_adjustpayperiod=@CurPayPeriod";
            #endregion

            public const string calculateAdjHours = @"  
		DECLARE @MAXABSHR AS decimal
		SET @MAXABSHR = (select Pmt_NumericValue from t_parametermaster
		where pmt_parameterid = 'MAXABSHR')


		UPDATE T_LaborHrsAdjustment  
		SET  Lha_RegularHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
					 (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
			  ELSE  
					 0.00  
			  END),  
		Lha_RegularOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
				   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
			   ELSE  
					0.00  
			   END),  
		Lha_RegularNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
					 (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
			  ELSE  
					 0.00  
			  END),  
		Lha_RegularOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REG' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
				   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
			   ELSE  
					0.00  
			   END),  
		Lha_RestdayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
					 (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
			 ELSE  
					 0.00  
			 END),  
		Lha_RestdayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
					(T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
				ELSE  
					0.00  
				END),  
		Lha_RestdayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
					 (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
			 ELSE  
					 0.00  
			 END),  
		Lha_RestdayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'REST' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
					(T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
				ELSE  
					0.00  
				END),  
		Lha_LegalHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
					  (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
				  ELSE  
					  0.00  
				  END),  
		Lha_LegalHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						 (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
					 ELSE  
						 0.00  
					END),  
		Lha_LegalHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
					  (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
				  ELSE  
					  0.00  
				  END),  
		Lha_LegalHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						 (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
					 ELSE  
						 0.00  
					END),  

		Lha_SpecialHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						(T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
					ELSE  
						0.00  
					END),  
		Lha_SpecialHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
					   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
					  ELSE  
						   0.00  
					  END),  
		Lha_SpecialHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
					ELSE  
						0.00  
					END),  
		Lha_SpecialHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
					   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
					  ELSE  
						   0.00  
					  END),  
		Lha_PlantShutdownHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						((T_EmployeeLogLedgerHist.Ell_RegularHour + T_EmployeeLogLedgerHist.Ell_LeaveHour) - (T_EmployeeLogLedgerTrail.Ell_RegularHour + T_EmployeeLogLedgerTrail.Ell_LeaveHour))  
				   ELSE  
						0.00  
				   END),  
		Lha_PlantShutdownOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
					 ELSE  
						   0.00  
					 END),  
		Lha_PlantShutdownNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour )  
				   ELSE  
						0.00  
				   END),  
		Lha_PlantShutdownOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
					 ELSE  
						   0.00  
					 END),  
		Lha_RestdayLegalHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							(T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
						ELSE  
							0.00  
						 END),  
		Lha_RestdayLegalHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
						   ELSE  
							   0.00  
						   END),  
		Lha_RestdayLegalHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
						ELSE  
							0.00  
						 END),  
		Lha_RestdayLegalHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'HOL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
						   ELSE  
							   0.00  
						   END),  
		Lha_RestdaySpecialHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								(T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
							ELSE  
								0.00  
							END),  
		Lha_RestdaySpecialHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
							 ELSE  
								   0.00  
							 END),  
		Lha_RestdaySpecialHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
							ELSE  
								0.00  
							END),  
		Lha_RestdaySpecialHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SPL' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
							 ELSE  
								   0.00  
							 END),  
		Lha_RestdayCompanyHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								(T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
							ELSE  
								0.00  
							END),  
		Lha_RestdayCompanyHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
							 ELSE  
								   0.00  
							 END),  
		Lha_RestdayCompanyHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
							ELSE  
								0.00  
							END),  
		Lha_RestdayCompanyHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
								   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
							 ELSE  
								   0.00  
							 END),
		Lha_RestdayPlantShutdownHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							(T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
						ELSE  
							0.00  
						 END),  
		Lha_RestdayPlantShutdownOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							   (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
						   ELSE  
							   0.00  
						   END),  
		Lha_RestdayPlantShutdownNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
						ELSE  
							0.00  
						 END),  
		Lha_RestdayPlantShutdownOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'PSD' and T_EmployeeLogLedgerTrail.Ell_RestDay = 1 then  
							   (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
						   ELSE  
							   0.00  
						   END),
		Lha_CompanyHolidayHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
								 (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour)  
							ELSE 0.00
						   END),
		Lha_CompanyHolidayOTHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
								  (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  
							ELSE 0.00
						   END),
		Lha_CompanyHolidayNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
									(T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
							ELSE 0.00
						   END),
		Lha_CompanyHolidayOTNDHr = (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'COMP' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
									 (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
							ELSE 0.00
						   END)
		FROM T_LaborHrsAdjustment  
		INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
		INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
		WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = @CurPayPeriod

		--gcd 11.28.2009
		UPDATE T_LaborHrsAdjustment  
		SET  Lha_RegularHr = Lha_RegularHr + (T_EmployeeLogLedgerTrail.Ell_AbsentHour - T_EmployeeLogLedgerHist.Ell_AbsentHour)
		FROM T_LaborHrsAdjustment  
		INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
		INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
		WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = @CurPayPeriod
		and T_EmployeeLogLedgerTrail.Ell_LeaveHour <> T_EmployeeLogLedgerHist.Ell_LeaveHour
		and T_EmployeeLogLedgerTrail.Ell_AbsentHour <> T_EmployeeLogLedgerHist.Ell_AbsentHour
		and T_EmployeeLogLedgerTrail.Ell_RegularHour = T_EmployeeLogLedgerHist.Ell_RegularHour

		--ADDED BY KEVIN 08082009 - TO ADD MAXIMUM ABSENT HOURS ADJUSTMENT

		UPDATE T_LaborHrsAdjustment  
		SET Lha_RegularHr = case when Lha_RegularHr < 0 then @MAXABSHR*-1 else @MAXABSHR end 
		WHERE Lha_AdjustpayPeriod = @CurPayPeriod
		AND Abs(Lha_RegularHr) > @MAXABSHR

		--END OF ADD 08082009 - KEVIN

		--ADDED BY JAN 11242009 - 
		Update T_LaborHrsAdjustment
		Set lha_regularhr = ell_absenthour
		From T_LaborHrsAdjustment
		Inner Join t_employeelogledgertrail on ell_employeeid = lha_employeeid
		and ell_processdate = lha_processdate
		and Ell_AdjustpayPeriod  = Lha_AdjustpayPeriod    
		where lha_regularhr > ell_absenthour
		and ell_absenthour = (@MAXABSHR/2)
		and lha_adjustpayperiod=@CurPayPeriod

		--ADDED 01132010

		-- for half day leaves
		Update T_LaborHrsAdjustment
		Set lha_regularhr = (@MAXABSHR/2) * -1
		From T_LaborHrsAdjustment
		Inner Join t_employeelogledgertrail on t_employeelogledgertrail.ell_employeeid = lha_employeeid
		and t_employeelogledgertrail.ell_processdate = lha_processdate
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = Lha_AdjustpayPeriod 
		Inner Join t_employeelogledgerhist on t_employeelogledgerhist.ell_employeeid = lha_employeeid
		and t_employeelogledgerhist.ell_processdate = lha_processdate
		where t_employeelogledgertrail.Ell_RegularHour > t_employeelogledgerhist.Ell_RegularHour
		and t_employeelogledgerhist.Ell_RegularHour <  t_employeelogledgerhist.Ell_LeaveHour
		and t_employeelogledgerhist.Ell_RegularHour + t_employeelogledgerhist.Ell_LeaveHour <> t_employeelogledgerhist.Ell_ShiftMin/60.00
		and t_employeelogledgerhist.Ell_LeaveHour >= (@MAXABSHR/2)
		and t_employeelogledgerhist.Ell_LeaveHour <= @MAXABSHR  
		and t_employeelogledgerhist.ell_absenthour =  (@MAXABSHR/2)
		and t_employeelogledgertrail.Ell_LeaveHour = 0
		and lha_adjustpayperiod=@CurPayPeriod

		--With Adjusted leave and punches
		Update T_LaborHrsAdjustment
		Set lha_regularhr = 0
		From T_LaborHrsAdjustment
		Inner Join t_employeelogledgertrail on t_employeelogledgertrail.ell_employeeid = lha_employeeid
		and t_employeelogledgertrail.ell_processdate = lha_processdate
		and t_employeelogledgertrail.ell_adjustpayperiod = Lha_AdjustpayPeriod 
		Inner Join t_employeelogledgerhist on t_employeelogledgerhist.ell_employeeid = lha_employeeid
		and t_employeelogledgerhist.ell_processdate = lha_processdate
		where t_employeelogledgerhist.ell_regularhour + t_employeelogledgerhist.ell_leavehour = t_employeelogledgertrail.ell_regularhour 
		and t_employeelogledgertrail.ell_leavehour = 0
		and t_employeelogledgerhist.ell_absenthour = 0
		and t_employeelogledgertrail.ell_absenthour = 0
		and lha_adjustpayperiod=@CurPayPeriod

		Update T_LaborHrsAdjustment
		Set lha_regularhr = 0
		From T_LaborHrsAdjustment
		Inner Join t_employeelogledgertrail on t_employeelogledgertrail.ell_employeeid = lha_employeeid
		and t_employeelogledgertrail.ell_processdate = lha_processdate
		and t_employeelogledgertrail.ell_adjustpayperiod = Lha_AdjustpayPeriod  
		Inner Join t_employeelogledgerhist on t_employeelogledgerhist.ell_employeeid = lha_employeeid
		and t_employeelogledgerhist.ell_processdate = lha_processdate
		where lha_adjustpayperiod = @CurPayPeriod
		and t_employeelogledgerhist.ell_absenthour = 0
		and t_employeelogledgertrail.ell_absenthour = 0
		and (( t_employeelogledgertrail.ell_regularhour = @MAXABSHR
		and t_employeelogledgerhist.ell_regularhour > @MAXABSHR) or
		( t_employeelogledgerhist.ell_regularhour = @MAXABSHR
		and t_employeelogledgertrail.ell_regularhour > @MAXABSHR))";

            public const string insertToDailyAllowance = @"
-----------------INSERTED BY KEVIN 04072009 - T_EMPLOYEEDAILYALLOWANCE --------------------
Declare @SqlScript1 varchar(max)
Declare @SqlScript2 varchar(max)
Declare @Cnt smallint
Declare @Code varchar(5)
Declare @Payperiod varchar(10)
Declare @user varchar(40)

set @Cnt = 0
set @SqlScript1 = ''
set @SqlScript2 = ''
set @Code =''
set @Payperiod = @CurPayPeriod
set @user = @UserLogin
while (@Cnt < 6)
begin
	set @Cnt = @Cnt + 1
	set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	set @SqlScript1 = @SqlScript1 + 
	'
	insert into T_EmployeeDailyAllowance
	select
	T_LaborHrsAdjustment.Lha_EmployeeId
	, '''+ @Payperiod + '''
	, Alh_AllowanceAdjCode
	, ''false''
	, (isnull(sum(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code +'),0) - isnull(sum(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code +'),0))
	, ''P''
	, '''+ @user + '''
	, GetDate()
	FROM T_LaborHrsAdjustment 
	INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
	INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
	INNER JOIN T_Allowanceheader ON Alh_LedgerAlwCol = ''' + @Code + '''
	WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = ''' + @Payperiod + '''
	AND (isnull(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code + ',0) - isnull(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code + ',0)) <> 0
    AND ISNULL(T_EmployeeLogLedgerTrail.Ell_AssumedPostBack, '''') != ''A'' --Added By Rendell Uy - 1/28/2011
group by T_LaborHrsAdjustment.Lha_EmployeeId,Alh_AllowanceAdjCode'
end

while (@Cnt < 12)
begin
	set @Cnt = @Cnt + 1
	set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	set @SqlScript2 = @SqlScript2 + 
	'
	insert into T_EmployeeDailyAllowance
	select
	T_LaborHrsAdjustment.Lha_EmployeeId
	, '''+ @Payperiod + '''
	, Alh_AllowanceAdjCode
	, ''false''
	, (isnull(sum(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code +'),0) - isnull(sum(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code +'),0))
	, ''P''
	, '''+ @user + '''
	, GetDate()
	FROM T_LaborHrsAdjustment 
	INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
	INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
	INNER JOIN T_Allowanceheader ON Alh_LedgerAlwCol = ''' + @Code + '''
	WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = ''' + @Payperiod + '''
	AND (isnull(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code + ',0) - isnull(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code + ',0)) <> 0
    AND ISNULL(T_EmployeeLogLedgerTrail.Ell_AssumedPostBack, '''') != ''A'' --Added By Rendell Uy - 1/28/2011
group by T_LaborHrsAdjustment.Lha_EmployeeId,Alh_AllowanceAdjCode'
end

exec (@SqlScript1 + @SqlScript2)
--------------------  END -----------------------------------
";

            public const string insertToDailyAllowanceNotAdj = @"
-----------------INSERTED BY KEVIN 04072009 - T_EMPLOYEEDAILYALLOWANCE --------------------
Declare @SqlScript1 varchar(max)
Declare @SqlScript2 varchar(max)
Declare @Cnt smallint
Declare @Code varchar(5)
Declare @Payperiod varchar(10)
Declare @user varchar(40)

set @Cnt = 0
set @SqlScript1 = ''
set @SqlScript2 = ''
set @Code =''
set @Payperiod = @CurPayPeriod
set @user = @UserLogin
while (@Cnt < 6)
begin
	set @Cnt = @Cnt + 1
	set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	set @SqlScript1 = @SqlScript1 + 
	'
	insert into T_EmployeeDailyAllowance
	select
	T_LaborHrsAdjustment.Lha_EmployeeId
	, '''+ @Payperiod + '''
	, Alh_AllowanceCode
	, ''false''
	, (isnull(sum(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code +'),0) - isnull(sum(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code +'),0))
	, ''C''
	, '''+ @user + '''
	, GetDate()
	FROM T_LaborHrsAdjustment 
	INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
	INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
	INNER JOIN T_Allowanceheader ON Alh_LedgerAlwCol = ''' + @Code + '''
	WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = ''' + @Payperiod + '''
	AND (isnull(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code + ',0) - isnull(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code + ',0)) <> 0
    AND ISNULL(T_EmployeeLogLedgerTrail.Ell_AssumedPostBack, '''') = ''A'' --Added By Rendell Uy - 1/28/2011
group by T_LaborHrsAdjustment.Lha_EmployeeId,Alh_AllowanceCode'
end

while (@Cnt < 12)
begin
	set @Cnt = @Cnt + 1
	set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	set @SqlScript2 = @SqlScript2 + 
	'
	insert into T_EmployeeDailyAllowance
	select
	T_LaborHrsAdjustment.Lha_EmployeeId
	, '''+ @Payperiod + '''
	, Alh_AllowanceCode
	, ''false''
	, (isnull(sum(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code +'),0) - isnull(sum(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code +'),0))
	, ''C''
	, '''+ @user + '''
	, GetDate()
	FROM T_LaborHrsAdjustment 
	INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
	INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
	INNER JOIN T_Allowanceheader ON Alh_LedgerAlwCol = ''' + @Code + '''
	WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = ''' + @Payperiod + '''
	AND (isnull(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code + ',0) - isnull(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code + ',0)) <> 0
    AND ISNULL(T_EmployeeLogLedgerTrail.Ell_AssumedPostBack, '''') = ''A'' --Added By Rendell Uy - 1/28/2011
group by T_LaborHrsAdjustment.Lha_EmployeeId,Alh_AllowanceCode'
end

exec (@SqlScript1 + @SqlScript2)
--------------------  END -----------------------------------
";

            public const string addAdjHoursForSOFF = @"UPDATE T_LaborHrsAdjustment  
                                                        SET Lha_RegularOTHr = Lha_RegularOTHr + 
                                                            (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SOFF' and 
                                                                        T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then                                                        
                                                            (T_EmployeeLogLedgerHist.Ell_OvertimeHour - T_EmployeeLogLedgerTrail.Ell_OvertimeHour)  + 
                                                            (T_EmployeeLogLedgerHist.Ell_RegularHour - T_EmployeeLogLedgerTrail.Ell_RegularHour) 
                                                                ELSE 0.00  
                                                             END),  
	                                                        Lha_RegularNDHr =  Lha_RegularNDHr + (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SOFF' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
							                                                         (T_EmployeeLogLedgerHist.Ell_RegularNightPremHour - T_EmployeeLogLedgerTrail.Ell_RegularNightPremHour)  
					                                                          ELSE  
							                                                         0.00  
					                                                          END),  
                                                            Lha_RegularOTNDHr = Lha_RegularOTNDHr + (CASE WHEN T_EmployeeLogLedgerTrail.Ell_DayCode = 'SOFF' and T_EmployeeLogLedgerTrail.Ell_RestDay = 0 then  
						                                                           (T_EmployeeLogLedgerHist.Ell_OvertimeNightPremHour - T_EmployeeLogLedgerTrail.Ell_OvertimeNightPremHour)  
					                                                           ELSE  
							                                                        0.00  
					                                                           END)
                                                     FROM T_LaborHrsAdjustment  
                                                        INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
                                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
                                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
                                                        INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
                                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
                                                        WHERE Lha_AdjustpayPeriod = @CurPayPeriod";
            //MODIFIED BY KEVIN 04082009 - FOR PAY type
            public const string updateRates = @"Update T_LaborHrsAdjustment
                                                        Set Lha_SalaryRate = {0},
                                                         Lha_HourlyRate = ISNULL(CASE 
                                                                         WHEN T_EmployeeMasterHist.{1} = 'D' THEN T_EmployeeMasterHist.{0} / 8
                                                                         WHEN T_EmployeeMasterHist.{1} = 'M' THEN ((T_EmployeeMasterHist.{0} * 12) / (Select Pmt_NumericValue From T_ParameterMaster Where Pmt_ParameterID = 'MDIVISOR' And Pmt_Status = 'A')) / 8
                                                                         WHEN T_EmployeeMasterHist.{1} = 'H' THEN T_EmployeeMasterHist.{0}
                                                                         END, 0),
                                                         Usr_Login = @UserLogin,
                                                         Ludatetime = Getdate()
                                                        FROM T_LaborHrsAdjustment 
                                                        Inner Join T_EmployeeMasterHist on Emt_EmployeeID = Lha_EmployeeId
	                                                        and  Emt_PayPeriod = Lha_PayPeriod
                                                            WHERE Lha_AdjustpayPeriod = @CurPayPeriod";

            public const string deleteZeroSum = @"DELETE FROM T_LaborHrsAdjustment 
                                                        WHERE (Lha_RegularHr
                                                              + Lha_RegularOTHr
                                                              + Lha_RegularNDHr
                                                              + Lha_RegularOTNDHr
                                                              + Lha_RestdayHr
                                                              + Lha_RestdayOTHr
                                                              + Lha_RestdayNDHr
                                                              + Lha_RestdayOTNDHr
                                                              + Lha_LegalHolidayHr
                                                              + Lha_LegalHolidayOTHr
                                                              + Lha_LegalHolidayNDHr
                                                              + Lha_LegalHolidayOTNDHr
                                                              + Lha_SpecialHolidayHr
                                                              + Lha_SpecialHolidayOTHr
                                                              + Lha_SpecialHolidayNDHr
                                                              + Lha_SpecialHolidayOTNDHr
                                                              + Lha_PlantShutdownHr
                                                              + Lha_PlantShutdownOTHr
                                                              + Lha_PlantShutdownNDHr
                                                              + Lha_PlantShutdownOTNDHr
                                                              + Lha_CompanyHolidayHr
                                                              + Lha_CompanyHolidayOTHr
                                                              + Lha_CompanyHolidayNDHr
                                                              + Lha_CompanyHolidayOTNDHr
                                                              + Lha_RestdayLegalHolidayHr
                                                              + Lha_RestdayLegalHolidayOTHr
                                                              + Lha_RestdayLegalHolidayNDHr
                                                              + Lha_RestdayLegalHolidayOTNDHr
                                                              + Lha_RestdaySpecialHolidayHr
                                                              + Lha_RestdaySpecialHolidayOTHr
                                                              + Lha_RestdaySpecialHolidayNDHr
                                                              + Lha_RestdaySpecialHolidayOTNDHr
                                                              + Lha_RestdayCompanyHolidayHr
                                                              + Lha_RestdayCompanyHolidayOTHr
                                                              + Lha_RestdayCompanyHolidayNDHr
                                                              + Lha_RestdayCompanyHolidayOTNDHr
                                                              + Lha_RestdayPlantShutdownHr
                                                              + Lha_RestdayPlantShutdownOTHr
                                                              + Lha_RestdayPlantShutdownNDHr
                                                              + Lha_RestdayPlantShutdownOTNDHr) = 0
                                                    AND Lha_AdjustpayPeriod = @CurPayPeriod";

            public const string updateLaborHoursAmt = @"UPDATE T_LaborHrsAdjustment
                                                        SET Lha_LaborHrsAdjustmentAmt = 
                                                            (Lha_RegularHr * Lha_HourlyRate)
                                                          + (Lha_RegularOTHr * Lha_HourlyRate * @RegOT) 
                                                          + (Lha_RegularNDHr * Lha_HourlyRate * @Reg * @NGTPrem)
                                                          + (Lha_RegularOTNDHr * Lha_HourlyRate * @RegOT * @NGTPrem)
                                                          + (Lha_RestdayHr * Lha_HourlyRate * @Rest)
                                                          + (Lha_RestdayOTHr * Lha_HourlyRate * @RestOT)
                                                          + (Lha_RestdayNDHr * Lha_HourlyRate * @Rest * @NGTPrem)
                                                          + (Lha_RestdayOTNDHr * Lha_HourlyRate * @RestOT * @NGTPrem)
                                                          + (Lha_LegalHolidayHr * Lha_HourlyRate * @Hol)
                                                          + (Lha_LegalHolidayOTHr * Lha_HourlyRate * @HolOT)
                                                          + (Lha_LegalHolidayNDHr * Lha_HourlyRate * @Hol * @NGTPrem)
                                                          + (Lha_LegalHolidayOTNDHr * Lha_HourlyRate * @HolOT * @NGTPrem)
                                                          + (Lha_SpecialHolidayHr * Lha_HourlyRate * @SPL)
                                                          + (Lha_SpecialHolidayOTHr * Lha_HourlyRate * @SPLOT)
                                                          + (Lha_SpecialHolidayNDHr * Lha_HourlyRate * @SPL * @NGTPrem)
                                                          + (Lha_SpecialHolidayOTNDHr * Lha_HourlyRate * @SPLOT * @NGTPrem)
                                                          + (Lha_PlantShutdownHr * Lha_HourlyRate * @PSD)
                                                          + (Lha_PlantShutdownOTHr * Lha_HourlyRate * @PSDOT)
                                                          + (Lha_PlantShutdownNDHr * Lha_HourlyRate * @PSD * @NGTPrem)
                                                          + (Lha_PlantShutdownOTNDHr * Lha_HourlyRate * @PSDOT * @NGTPrem)
                                                          + (Lha_CompanyHolidayHr * Lha_HourlyRate * @Comp)
                                                          + (Lha_CompanyHolidayOTHr * Lha_HourlyRate * @CompOT)
                                                          + (Lha_CompanyHolidayNDHr * Lha_HourlyRate * @Comp * @NGTPrem)
                                                          + (Lha_CompanyHolidayOTNDHr * Lha_HourlyRate * @CompOT * @NGTPrem)
                                                          + (Lha_RestdayLegalHolidayHr * Lha_HourlyRate * @RestHol)
                                                          + (Lha_RestdayLegalHolidayOTHr * Lha_HourlyRate * @RestHolOT)
                                                          + (Lha_RestdayLegalHolidayNDHr * Lha_HourlyRate * @RestHol * @NGTPrem)
                                                          + (Lha_RestdayLegalHolidayOTNDHr * Lha_HourlyRate * @RestHolOT * @NGTPrem) 
                                                          + (Lha_RestdaySpecialHolidayHr * Lha_HourlyRate * @RestSPL)
                                                          + (Lha_RestdaySpecialHolidayOTHr * Lha_HourlyRate * @RestSPLOT)
                                                          + (Lha_RestdaySpecialHolidayNDHr * Lha_HourlyRate * @RestSPL * @NGTPrem) 
                                                          + (Lha_RestdaySpecialHolidayOTNDHr * Lha_HourlyRate * @RestSPLOT * @NGTPrem)
                                                          + (Lha_RestdayCompanyHolidayHr * Lha_HourlyRate * @RestComp)
                                                          + (Lha_RestdayCompanyHolidayOTHr * Lha_HourlyRate * @RestCompOT) 
                                                          + (Lha_RestdayCompanyHolidayNDHr * Lha_HourlyRate * @RestComp * @NGTPrem) 
                                                          + (Lha_RestdayCompanyHolidayOTNDHr * Lha_HourlyRate * @RestCompOT * @NGTPrem)
                                                          + (Lha_RestdayPlantShutdownHr * Lha_HourlyRate * @RestPSD)
                                                          + (Lha_RestdayPlantShutdownOTHr * Lha_HourlyRate * @RestPSDOT)
                                                          + (Lha_RestdayPlantShutdownNDHr * Lha_HourlyRate * @RestPSD * @NGTPrem)
                                                          + (Lha_RestdayPlantShutdownOTNDHr * Lha_HourlyRate * @RestPSDOT * @NGTPrem) 
                                                        FROM T_LaborHrsAdjustment
                                                        INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
	                                                        and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
	                                                        and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod    
                                                        WHERE Lha_AdjustpayPeriod = @CurPayPeriod
                                                        AND ISNULL(T_EmployeeLogLedgerTrail.Ell_AssumedPostBack, '') != 'A' --Added By Rendell Uy - 1/28/2011";
            public const string deleteZeroLaborHrsAdjAmt = @"DELETE FROM T_LaborHrsAdjustment 
                                                             WHERE Lha_LaborHrsAdjustmentAmt = 0
                                                             AND Lha_AdjustpayPeriod = @CurPayPeriod";
            public const string checkRequiredConstant = @"SELECT * FROM DBO.NumericValue('NIGHTPREM')
                                                          SELECT * FROM GETPREMIUMS('REG', 'false')
                                                          SELECT * FROM GETPREMIUMS('REST', 'true')
                                                          SELECT * FROM GETPREMIUMS('HOL', 'false')
                                                          SELECT * FROM GETPREMIUMS('SPL', 'false')
                                                          SELECT * FROM GETPREMIUMS('PSD', 'false')
                                                          SELECT * FROM GETPREMIUMS('COMP', 'false')
                                                          SELECT * FROM GETPREMIUMS('HOL', 'true')
                                                          SELECT * FROM GETPREMIUMS('SPL', 'true')
                                                          SELECT * FROM GETPREMIUMS('COMP', 'true')
                                                          SELECT * FROM GETPREMIUMS('PSD', 'true')
                                                        ------------------------------------------------------------------------
                                                        Select Distinct Lha_EmployeeId + ' - ' + Emt_Firstname + ' ' + Emt_Lastname + ' (' + convert(char(10),Ppm_StartCycle,101) + ' - ' + convert(char(10),Ppm_EndCycle,101) + ')'
                                                        FROM  T_LaborHrsAdjustment 
                                                        LEFT Join T_EmployeeMasterHist on T_EmployeeMasterHist.Emt_EmployeeID = Lha_EmployeeId
                                                        and  Emt_PayPeriod = Lha_PayPeriod
                                                        LEFT JOIN T_EmployeeMaster on T_EmployeeMaster.Emt_EmployeeID = T_EmployeeMasterHist.Emt_EmployeeID
                                                        LEFT JOIN T_PayperiodMaster on ppm_payperiod =  Lha_PayPeriod
                                                        WHERE Lha_AdjustpayPeriod = @CurPayPeriod
                                                        -- MODIFIED BY KEVIN 04072009 FOR PAY TYPE
                                                        and (T_EmployeeMasterHist.{0} is null or T_EmployeeMasterHist.{0} =0)";
            #endregion
            #region [For Payroll Transaction Posting]
            public const string clearPayrollTransactionTable = @"Update T_EmployeePayrollTransaction
                                                                Set  Ept_LaborHrsAdjustmentAmt = '0.00'
	                                                                ,Ept_TaxAdjustmentAmt = '0.00'
	                                                                ,Ept_NonTaxAdjustmentAmt = '0.00'
	                                                                ,Ept_TaxAllowanceAmt = '0.00'
	                                                                ,Ept_NonTaxAllowanceAmt = '0.00'
                                                                Where Ept_CurrentPayPeriod = @CurPayPeriod
                                                                And (Ept_LaborHrsAdjustmentAmt <> 0 
                                                                OR Ept_TaxAdjustmentAmt <> 0
                                                                OR Ept_NonTaxAdjustmentAmt <> 0
                                                                OR Ept_TaxAllowanceAmt <> 0
                                                                OR Ept_NonTaxAllowanceAmt <> 0)";
            public const string updateEmployeeAdjustmentTransaction = @"
                                                                    --Update T_EmployeePayrollTransaction
                                                                    --Set  Ept_LaborHrsAdjustmentAmt = Ead_LaborHrsAdjustmentAmt
                                                                    --    ,Ept_TaxAdjustmentAmt = Ead_TaxAdjustmentAmt
                                                                    --    ,Ept_NonTaxAdjustmentAmt = Ead_NonTaxAdjustmentAmt
                                                                    --    ,Usr_Login = @UserLogin
                                                                    --    ,Ludatetime = Getdate()
                                                                    --From T_EmployeeAdjustment Inner Join T_EmployeePayrollTransaction
                                                                    --    On Ead_EmployeeId = Ept_EmployeeId
                                                                    --    And Ead_CurrentPayPeriod = Ept_CurrentPayPeriod
                                                                    --Where Ead_CurrentPayPeriod = @CurPayPeriod
                                                                    --    And Ead_PayrollPost = 0
                                                                    
                                                                    Update T_EmployeePayrollTransaction
                                                                    Set  Ept_LaborHrsAdjustmentAmt = Ead_LaborHrsAdjustmentAmt
                                                                        ,Ept_TaxAdjustmentAmt = Ead_TaxAdjustmentAmt
                                                                        ,Ept_NonTaxAdjustmentAmt = Ead_NonTaxAdjustmentAmt
                                                                        ,Usr_Login = @UserLogin
                                                                        ,Ludatetime = Getdate()
                                                                    From T_EmployeePayrollTransaction
                                                                    Inner Join (Select Ead_Employeeid, Ead_CurrentPayPeriod
			                                                                    , Sum(Ead_LaborHrsAdjustmentAmt) as Ead_LaborHrsAdjustmentAmt
			                                                                    , Sum(Ead_TaxAdjustmentAmt) as Ead_TaxAdjustmentAmt
			                                                                    , Sum(Ead_NonTaxAdjustmentAmt) as Ead_NonTaxAdjustmentAmt
			                                                                    From T_EmployeeAdjustment 
			                                                                    Where Ead_CurrentPayPeriod = @CurPayPeriod
				                                                                    And Ead_PayrollPost = 0
			                                                                    Group by Ead_EmployeeID,Ead_CurrentPayPeriod ) xx On xx.Ead_EmployeeId = Ept_EmployeeId
						                                                                    And xx.Ead_CurrentPayPeriod = Ept_CurrentPayPeriod
                                                                    Where Ept_CurrentPayPeriod = @CurPayPeriod";
            public const string updateEmployeeAdjustmentFlags = @"Update T_EmployeeAdjustment
                                                                    Set Ead_PayrollPost = 1
                                                                    From T_EmployeeAdjustment Inner Join T_EmployeePayrollTransaction
                                                                        On Ead_EmployeeId = Ept_EmployeeId
                                                                        And Ead_CurrentPayPeriod = Ept_CurrentPayPeriod
                                                                    Where Ead_CurrentPayPeriod = @CurPayPeriod";
            public const string updateEmployeeAllowanceTransaction = @"Update T_EmployeePayrollTransaction
                                                                        Set  Ept_TaxAllowanceAmt = TaxAllowanceAmt
                                                                            ,Ept_NonTaxAllowanceAmt = NonTaxAllowanceAmt
                                                                            ,Usr_Login = @UserLogin
                                                                            ,Ludatetime = Getdate()
                                                                        From T_EmployeePayrollTransaction Inner Join
                                                                        (Select Eal_EmployeeID
                                                                                ,TaxAllowanceAmt = Sum(CASE WHEN Acm_TaxClass = 'T' THEN Eal_AllowanceAmt ELSE 0 END)
                                                                                ,NonTaxAllowanceAmt = Sum(CASE WHEN Acm_TaxClass = 'N' THEN Eal_AllowanceAmt ELSE 0 END)
                                                                         From T_EmployeeAllowance Inner Join T_AllowanceCodeMaster
                                                                            On Acm_AllowanceCode = Eal_AllowanceCode
                                                                         Where Eal_CurrentPayPeriod = @CurPayPeriod
                                                                            And Eal_PayrollPost = 0
                                                                         Group by Eal_EmployeeID)Allowance on Ept_EmployeeId = Eal_EmployeeID
                                                                        Where Ept_CurrentPayPeriod = @CurPayPeriod";
            public const string updateEmployeeAllowanceFlags = @"Update T_EmployeeAllowance
                                                                Set Eal_PayrollPost = 1
                                                                From T_EmployeeAllowance Inner Join T_EmployeePayrollTransaction
                                                                    On Eal_EmployeeId = Ept_EmployeeId
                                                                    And Eal_CurrentPayPeriod = Ept_CurrentPayPeriod
                                                                Where Eal_CurrentPayPeriod = @CurPayPeriod";

            #endregion
            #region [For Leave Refund Calculation]
            
            //edited by jccapetillo 11152009
//            public const string FetchRecordsForRefund = @"declare @VLDMINIMIS as decimal(9,2)
//
//                                                            set @VLDMINIMIS = (select Pmt_NumericValue from T_ParameterMaster
//                                                            where Pmt_ParameterID ='VLDMINIMIS')
//
//                                                            SELECT	DISTINCT
//                                                                            Emt_EmployeeID EmployeeID, 
//                                                                            Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
//                                                                            Ltm_LeaveType LeaveType, 
//                                                                            Ltm_LeaveDesc LeaveDesc,
//				                                                            CASE WHEN Ltm_LeaveType = 'VL' THEN 'NON-TAXABLE'
//				                                                            ELSE 'TAXABLE'
//				                                                            END	 TaxClass,
//                                                                            CASE Ltm_LeaveType
//                                                                            WHEN 'VL' THEN 
//						                                                            CASE WHEN Elm_VLBalance <= @VLDMINIMIS THEN
//								                                                            Elm_VLBalance
//						                                                            ELSE	
//								                                                            @VLDMINIMIS
//						                                                            END
//	                                                                        WHEN 'SL' THEN Elm_SLBalance
//                                                                            WHEN 'PL' THEN Elm_SLBalance 
//                                                                            WHEN 'BL' THEN Elm_SLBalance
//                                                                            WHEN 'DL' THEN Elm_SLBalance
//                                                                            ELSE  0   END LeaveBalance,
//                                                                    0 RefundAmount,
//                                                                    Emt_SalaryRate SalaryRate,
//                                                                    Emt_PayrollType PayrollType,
//                                                                    Emt_BillingRate BillingRate,
//                                                                    Emt_BillingPayrollType BillingPayrollType,
//                                                                    Emt_TaxRate TaxRate,
//                                                                    Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
//                                                                    FROM	T_LeaveTypeMaster
//                                                                    JOIN	T_EmployeeMaster ON	LEFT(Emt_JobStatus, 1) = 'A' 
//                                                            {0}AND  Emt_Anniversary = 1
//                                                                    JOIN	T_EmployeeLeaveMaster ON Elm_EmployeeId = Emt_EmployeeID
//                                                                    WHERE	Ltm_ConvertibleToCash = 1
//                                                                    AND		Ltm_Status = 'A'
//                                                                    AND     (CASE Ltm_LeaveType
//				                                                            WHEN 'VL' THEN Elm_VLBalance
//				                                                            WHEN 'SL' THEN Elm_SLBalance
//				                                                            WHEN 'PL' THEN Elm_SLBalance
//				                                                            WHEN 'BL' THEN Elm_SLBalance
//				                                                            WHEN 'DL' THEN Elm_SLBalance
//				                                                            ELSE  0 END) > 0
//
//                                                            UNION ALL
//
//                                                            SELECT	DISTINCT
//                                                                            Emt_EmployeeID EmployeeID, 
//                                                                            Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
//                                                                            Ltm_LeaveType LeaveType, 
//                                                                            Ltm_LeaveDesc LeaveDesc,
//				                                                            'TAXABLE' TaxClass,
//                                                                            Elm_VLBalance - @VLDMINIMIS LeaveBalance,
//                                                                    0 RefundAmount,
//                                                                    Emt_SalaryRate SalaryRate,
//                                                                    Emt_PayrollType PayrollType,
//                                                                    Emt_BillingRate BillingRate,
//                                                                    Emt_BillingPayrollType BillingPayrollType,
//                                                                    Emt_TaxRate TaxRate,
//                                                                    Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
//                                                                    FROM	T_LeaveTypeMaster
//                                                                    JOIN	T_EmployeeMaster ON	LEFT(Emt_JobStatus, 1) = 'A' 
//                                                            {0}AND  Emt_Anniversary = 1
//                                                                     JOIN	T_EmployeeLeaveMaster ON Elm_EmployeeId = Emt_EmployeeID
//                                                                  WHERE	Ltm_Status = 'A'
//                                                                    AND    Ltm_LeaveType='VL'  
//		                                                            AND Elm_VLBalance > @VLDMINIMIS
//                                                                    ORDER BY 1";

            public const string FetchRecordsForRefund = @"  declare @VLDMINIMIS as decimal(9,2)
                                                            set @VLDMINIMIS = (select Pmt_NumericValue from T_ParameterMaster
                                                            where Pmt_ParameterID ='VLDMINIMIS')
                                                             
                                                            SELECT      Emt_EmployeeID EmployeeID, 
                                                                        Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
                                                                        Ltm_LeaveType LeaveType, 
                                                                        Ltm_LeaveDesc LeaveDesc,
                                                                              CASE WHEN  Elm_Entitled  - Elm_Used  <= @VLDMINIMIS THEN
                                                                                          'NON-TAXABLE'
                                                                              ELSE 'TAXABLE'
                                                                        END   TaxClass, 
                                                                              Elm_Entitled  - Elm_Used  as LeaveBalance,
                                                                        0.00 RefundAmount,
                                                                        Emt_SalaryRate SalaryRate,
                                                                        Emt_PayrollType PayrollType,
                                                                        Emt_BillingRate BillingRate,
                                                                        Emt_BillingPayrollType BillingPayrollType,
                                                                        Emt_TaxRate TaxRate,
                                                                        Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
                                                             FROM T_EmployeeLeave
                                                            INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elm_Leavetype
                                                                        and Ltm_ConvertibleToCash = 1
                                                            INNER JOIN  T_EmployeeMaster ON     Emt_Employeeid = Elm_EmployeeId
                                                                  AND LEFT(Emt_JobStatus, 1) = 'A' 
                                                            {0}AND  Emt_Anniversary = 1
                                                            WHERE Elm_Entitled  - Elm_Used > 0
                                                            --AND Elm_Entitled  - Elm_Used  <= @VLDMINIMIS 
															  and ltrim(isnull(Ltm_PartOfLeave, '')) = ''

                                                            UNION ALL

                                                            SELECT      Emt_EmployeeID EmployeeID, 
                                                                        Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
                                                                        Ltm_LeaveType LeaveType, 
                                                                        Ltm_LeaveDesc LeaveDesc,
                                                                              CASE WHEN  Elm_Entitled  - Elm_Used  <= @VLDMINIMIS THEN
                                                                                          'NON-TAXABLE'
                                                                              ELSE 'TAXABLE'
                                                                        END   TaxClass,
                                                                              (Elm_Entitled  - Elm_Used) - @VLDMINIMIS as LeaveBalance,
                                                                        0.00 RefundAmount,
                                                                        Emt_SalaryRate SalaryRate,
                                                                        Emt_PayrollType PayrollType,
                                                                        Emt_BillingRate BillingRate,
                                                                        Emt_BillingPayrollType BillingPayrollType,
                                                                        Emt_TaxRate TaxRate,
                                                                        Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
                                                             FROM T_EmployeeLeave
                                                            INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elm_Leavetype
                                                                        and Ltm_ConvertibleToCash = 1
                                                            INNER JOIN  T_EmployeeMaster ON     Emt_Employeeid = Elm_EmployeeId
                                                                  AND LEFT(Emt_JobStatus, 1) = 'A' 
                                                            {0}AND  Emt_Anniversary = 1
                                                            WHERE Elm_Entitled  - Elm_Used > 0  
                                                                  and Elm_Entitled  - Elm_Used  > @VLDMINIMIS 
															      and ltrim(isnull(Ltm_PartOfLeave, '')) = ''
                                                            ORDER BY 1";

            //20091214 : jccapetillo added
            public const string FetchRecordsForRefund2 = @" 
                                                            DECLARE @LeaveConversionYear as varchar(20)
                                                            SET @LeaveConversionYear = '{2}'

                                                            DECLARE @CurrentPayperiod as varchar(7)
                                                            SET @CurrentPayperiod = (
                                                                    Select Ppm_PayPeriod From T_PayPeriodMaster
                                                                            Where Ppm_CycleIndicator = 'C'
                                                                            And Ppm_Status = 'A')

                                                            declare @VLDMINIMIS as decimal(9,2)
                                                            set @VLDMINIMIS = (select Pmt_NumericValue from T_ParameterMaster
                                                            where Pmt_ParameterID ='VLDMINIMIS')

                                                            DECLARE @LVHRENTRY as bit
                                                            SET @LVHRENTRY = (SELECT Pcm_ProcessFlag
				                                                            FROM T_processcontrolmaster
				                                                            WHERE Pcm_SystemID='LEAVE'
				                                                            and  Pcm_ProcessID='LVHRENTRY')

                                                            DECLARE @MDIVISOR as decimal(9,2)

                                                            SET @MDIVISOR = (SELECT	Pmt_NumericValue 
                                                                            FROM	T_ParameterMaster 
                                                                            WHERE	Pmt_ParameterID = 'MDIVISOR')
                                                            
						                                    If @CurrentPayperiod = '{1}'
                                                                begin
                                                                    SELECT      Emt_EmployeeID EmployeeID, 
                                                                                Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
                                                                                Ltm_LeaveType LeaveType, 
                                                                                Ltm_LeaveDesc LeaveDesc

                                                                                , Convert(decimal(18,2),case when @LVHRENTRY = 1 then Elm_Entitled
	                                                                                else
	                                                                                (Elm_Entitled)/8
                                                                                  end) as EntitledLeave
                                                                                , Convert(decimal(18,2),case when @LVHRENTRY = 1 then Elm_Used   
	                                                                                else
	                                                                                (Elm_Used)/8.0
                                                                                  end) as UsedLeave
                                                                                , Convert(decimal(18,2),case when @LVHRENTRY = 1 then Elm_Entitled - Elm_Used - Elm_Reserved   
	                                                                                else
	                                                                                (Elm_Entitled - Elm_Used - Elm_Reserved)/8.0
                                                                                  end) as UnusedLeave
                                                                                , 0.00 as Nontax
                                                                                , 0.00 as Tax
                                                                                , 0.00 as Total

                                                                                ,Emt_SalaryRate SalaryRate,
                                                                                Emt_PayrollType PayrollType,
                                                                                Emt_BillingRate BillingRate,
                                                                                Emt_BillingPayrollType BillingPayrollType,
                                                                                Emt_TaxRate TaxRate,
                                                                                Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
                                                                    FROM T_EmployeeLeave
                                                                    INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elm_Leavetype
                                                                                and Ltm_ConvertibleToCash = 1
                                                                    INNER JOIN  T_EmployeeMaster ON Emt_Employeeid = Elm_EmployeeId
                                                                          AND LEFT(Emt_JobStatus, 1) = 'A' and Emt_JobStatus not in ('AJ', 'AM')
                                                                    {0}AND  Emt_Anniversary = 1
								                                    LEFT JOIN ( SELECT  Elm_LeaveYear
												                                      , Elm_EmployeeId
												                                      , Elm_Used as Used
											                                    FROM T_EmployeeLeave
											                                    INNER JOIN T_Employeemaster on Emt_Employeeid = Elm_Employeeid
												                                    and Left(Emt_Jobstatus, 1) = 'A' and Emt_JobStatus not in ('AJ', 'AM')
											                                    WHERE Elm_Used  > 0
												                                    And Elm_Leaveyear = @LeaveConversionYear
												                                    And Elm_Leavetype = 'GL') GLUsed on GLUsed.Elm_Employeeid =  T_EmployeeLeave.Elm_Employeeid
																		                                    and GLUsed.Elm_Leaveyear =  T_EmployeeLeave.Elm_Leaveyear
                                                                    WHERE Elm_Entitled  - Elm_Used > 0
                                                                          --AND Elm_Entitled  - Elm_Used  >= @VLDMINIMIS
															              and ltrim(isnull(Ltm_PartOfLeave, '')) = ''
								                                          And T_EmployeeLeave.Elm_Leaveyear = @LeaveConversionYear 
                                                                end
						                                    else
                                                                begin
                                                                    SELECT      T_EmployeeMasterHist.Emt_EmployeeID EmployeeID, 
                                                                                T_EmployeeMaster.Emt_Firstname + ' ' + T_EmployeeMaster.Emt_Lastname EmployeeName, 
                                                                                Ltm_LeaveType LeaveType, 
                                                                                Ltm_LeaveDesc LeaveDesc

                                                                                , Convert(decimal(18,2),case when @LVHRENTRY = 1 then Elm_Entitled
	                                                                                else
	                                                                                (Elm_Entitled)/8
                                                                                  end) as EntitledLeave
                                                                                , Convert(decimal(18,2),case when @LVHRENTRY = 1 then Elm_Used   
	                                                                                else
	                                                                                (Elm_Used)/8.0
                                                                                  end) as UsedLeave
                                                                                , Convert(decimal(18,2),case when @LVHRENTRY = 1 then Elm_Entitled - Elm_Used - Elm_Reserved   
	                                                                                else
	                                                                                (Elm_Entitled - Elm_Used - Elm_Reserved)/8.0
                                                                                  end) as UnusedLeave
                                                                                , 0.00 as Nontax
                                                                                , 0.00 as Tax
                                                                                , 0.00 as Total

                                                                                ,T_EmployeeMasterHist.Emt_SalaryRate SalaryRate,
                                                                                T_EmployeeMasterHist.Emt_PayrollType PayrollType,
                                                                                T_EmployeeMasterHist.Emt_BillingRate BillingRate,
                                                                                T_EmployeeMasterHist.Emt_BillingPayrollType BillingPayrollType,
                                                                                T_EmployeeMasterHist.Emt_TaxRate TaxRate,
                                                                                T_EmployeeMasterHist.Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
                                                                    FROM T_EmployeeLeave
                                                                    INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elm_Leavetype
                                                                                and Ltm_ConvertibleToCash = 1
                                                                    INNER JOIN  T_EmployeeMasterHist ON T_EmployeeMasterHist.Emt_Employeeid = Elm_EmployeeId
                                                                          AND LEFT(T_EmployeeMasterHist.Emt_JobStatus, 1) = 'A' and T_EmployeeMasterHist.Emt_JobStatus not in ('AJ', 'AM')
                                                                          AND T_EmployeeMasterHist.Emt_PayPeriod = '{1}'
								                                    LEFT JOIN ( SELECT  Elm_LeaveYear
												                                      , Elm_EmployeeId
												                                      , Elm_Used as Used
											                                    FROM T_EmployeeLeave
											                                    INNER JOIN T_EmployeeMasterHist on Emt_Employeeid = Elm_Employeeid
												                                    and Left(Emt_Jobstatus, 1) = 'A' and Emt_JobStatus not in ('AJ', 'AM')
												                                    and Emt_Payperiod = '{1}'
											                                    WHERE Elm_Used  > 0
												                                    And Elm_Leaveyear = @LeaveConversionYear
												                                    And Elm_Leavetype = 'GL') GLUsed on GLUsed.Elm_Employeeid =  T_EmployeeLeave.Elm_Employeeid
																			                                    and GLUsed.Elm_Leaveyear =  T_EmployeeLeave.Elm_Leaveyear
                                                                    INNER JOIN  T_EmployeeMaster ON T_EmployeeMaster.Emt_Employeeid = T_EmployeeLeave.Elm_EmployeeId
                                                                    WHERE Elm_Entitled  - Elm_Used > 0
                                                                          --AND Elm_Entitled  - Elm_Used  >= @VLDMINIMIS
															              and ltrim(isnull(Ltm_PartOfLeave, '')) = ''
								                                          And T_EmployeeLeave.Elm_Leaveyear = @LeaveConversionYear 
                                                                end";
            //end
            public const string checkPayRates = @"declare @DMaxRate decimal set @DMaxRate = (select Pmt_NumericValue 
											                                                from t_parametermaster
											                                                where pmt_parameterid='DMAXRATE'
											                                                and pmt_status='A')
                                                    --Employee's Rate <= 0
                                                     SELECT Count(Isnull(Emt_EmployeeID,0)) as Cnt
                                                     FROM T_EmployeeMaster 
                                                     INNER JOIN T_EmployeePayrollTransaction on Ept_EmployeeID = Emt_EmployeeID
                                                    WHERE CASE '{0}' 
                                                           WHEN 'B' then Emt_SalaryRate
                                                           WHEN 'T' THEN Emt_TaxRate
                                                           WHEN 'C' THEN Emt_BillingRate
                                                           ELSE 0 END <= 0
                                                     UNION
                                                    --Employee's Rate  > Daily-Paid maximum rate AND Payroll Type = Daily
                                                     SELECT Count(Isnull(Emt_EmployeeID,0)) as Cnt
                                                     FROM T_EmployeeMaster 
                                                     INNER JOIN T_EmployeePayrollTransaction on Ept_EmployeeID = Emt_EmployeeID
                                                     WHERE CASE '{0}' 
                                                           WHEN 'B' THEN Emt_SalaryRate
                                                           WHEN 'T' THEN Emt_TaxRate
                                                           WHEN 'C' THEN Emt_BillingRate
                                                           ELSE 0 END > @DMaxRate 
                                                     AND CASE '{0}' 
                                                           WHEN 'B' THEN Emt_PayrollType
                                                           WHEN 'T' THEN Emt_TaxRatePayrollType
                                                           WHEN 'C' THEN Emt_BillingPayrollType
                                                           ELSE '' END  = 'D'
                                                     UNION
                                                     --  Employee's Rate <  Daily-Paid maximum rate AND  Payroll Type = Monthly
                                                     SELECT Count(Isnull(Emt_EmployeeID,0)) as Cnt
                                                     FROM T_EmployeeMaster 
                                                     INNER JOIN T_EmployeePayrollTransaction on Ept_EmployeeID = Emt_EmployeeID
                                                     WHERE CASE '{0}' 
                                                           WHEN 'B' THEN Emt_SalaryRate
                                                           WHEN 'T' THEN Emt_TaxRate
                                                           WHEN 'C' THEN Emt_BillingRate
                                                           ELSE 0 END < @DMaxRate 
                                                     AND CASE '{0}' 
                                                           WHEN 'B' THEN Emt_PayrollType
                                                           WHEN 'T' THEN Emt_TaxRatePayrollType
                                                           WHEN 'C' THEN Emt_BillingPayrollType
                                                           ELSE '' END  = 'M'
                                                     SELECT COUNT(Ebn_EmployeeId) FROM dbo.T_EmployeeBonus WHERE Ebn_PayPeriod = @CurPayPeriod";


            #endregion
            #region [For Bonus Calculation]
            public const string fetchEmpBonusRegAndAdjOrOT = @"SELECT	SUM(Epc_RegularAmt) FROM {0}
                                                                WHERE	Epc_EmployeeId = @EmployeeID
                                                                AND		Epc_CurrentPayPeriod = @PayPeriod

                                                                SELECT	SUM(Lha_RegularHr * Lha_HourlyRate) 
                                                                FROM	T_LaborHrsAdjustment
                                                                WHERE	Lha_EmployeeId = @EmployeeID
                                                                AND		Lha_ProcessDate
                                                                BETWEEN	@StartDate
                                                                AND		@EndDate

                                                                SELECT	Sum(Epc_RegularAmt + Epc_RegularOTAmt + 
	                                                                    Epc_RegularNDAmt + Epc_RegularOTNDAmt + 
	                                                                    Epc_RestdayAmt + Epc_RestdayOTAmt + 
	                                                                    Epc_RestdayNDAmt + Epc_RestdayOTNDAmt + 
	                                                                    Epc_LegalHolidayAmt + Epc_LegalHolidayOTAmt + 
	                                                                    Epc_LegalHolidayNDAmt + Epc_LegalHolidayOTNDAmt + 
	                                                                    Epc_SpecialHolidayAmt + Epc_SpecialHolidayOTAmt + 
	                                                                    Epc_SpecialHolidayNDAmt + Epc_SpecialHolidayOTNDAmt + 
	                                                                    Epc_PlantShutdownAmt + Epc_PlantShutdownOTAmt + 
	                                                                    Epc_PlantShutdownNDAmt + Epc_PlantShutdownOTNDAmt +
	                                                                    Epc_CompanyHolidayAmt + Epc_CompanyHolidayOTAmt + 
	                                                                    Epc_CompanyHolidayNDAmt + Epc_CompanyHolidayOTNDAmt + 
	                                                                    Epc_RestdayLegalHolidayAmt + Epc_RestdayLegalHolidayOTAmt + 
	                                                                    Epc_RestdayLegalHolidayNDAmt + Epc_RestdayLegalHolidayOTNDAmt + 
	                                                                    Epc_RestdaySpecialHolidayAmt + Epc_RestdaySpecialHolidayOTAmt + 
	                                                                    Epc_RestdaySpecialHolidayNDAmt + Epc_RestdaySpecialHolidayOTNDAmt + 
	                                                                    Epc_RestdayCompanyHolidayAmt + Epc_RestdayCompanyHolidayOTAmt + 
	                                                                    Epc_RestdayCompanyHolidayNDAmt + Epc_RestdayCompanyHolidayOTNDAmt + 
	                                                                    Epc_RestdayPlantShutdownAmt + Epc_RestdayPlantShutdownOTAmt + 
	                                                                    Epc_RestdayPlantShutdownNDAmt + Epc_RestdayPlantShutdownOTNDAmt +
	                                                                    Epc_LaborHrsAdjustmentAmt) FROM {0}
                                                                WHERE	Epc_EmployeeId = @EmployeeID
                                                                AND		Epc_CurrentPayPeriod = @PayPeriod";

            // Clark/13thMonth START
            public const string insertEmployee13thMonthBonus = @"
                                                        --Retrieve CostcenterCode
                                                        DECLARE @EmployeeCostcenter as varchar(12)
                                                        SELECT  @EmployeeCostcenter = Emt_CostcenterCode
                                                        FROM     T_Employeemaster
                                                        WHERE    Emt_EmployeeId = @EmployeeId

                                                        --Insert bonus
                                                        INSERT INTO [T_EmployeeBonus]
                                                                   ([Ebn_Payperiod]
                                                                   ,[Ebn_EmployeeId]
                                                                   ,[Ebn_SalaryRate]
                                                                   ,[Ebn_PayrollType]
                                                                   ,[Ebn_RegularAmt]
                                                                   ,[Ebn_AdjustmentAmt]
                                                                   ,[Ebn_BonusAmt]
                                                                   ,[usr_login]
                                                                   ,[ludatetime]
                                                                   ,[Ebn_Costcenter]
                                                                   ,[Ebn_UsedNontaxAmt]
                                                                   ,[Ebn_NontaxAmt]
                                                                   ,[Ebn_TaxAmt]
                                                                   ,[Ebn_Post])
                                                             VALUES(@PayPeriod
                                                                   ,@EmployeeId
                                                                   ,@SalaryRate
                                                                   ,@PayrollType
                                                                   ,@Ebn_RegularAmt
                                                                   ,@Ebn_AdjustmentAmt
                                                                   ,@Amount
                                                                   ,@UserLogin
                                                                   ,GetDate()
                                                                   ,@EmployeeCostcenter
                                                                   ,@Ebn_UsedNontaxAmt
                                                                   ,@Ebn_NontaxAmt
                                                                   ,@Ebn_TaxAmt
                                                                   ,'False')

                                                        --Delete allowances
                                                        DELETE FROM [T_EmployeeAllowance]
                                                        WHERE Eal_CurrentPayPeriod = @PayPeriod
                                                        AND   Eal_EmployeeId = @EmployeeId

                                                        --Insert Non-Taxable Allowance
                                                        IF @Ebn_NontaxAmt > 0.0
                                                        INSERT INTO [T_EmployeeAllowance]
                                                                   ([Eal_EmployeeId]
                                                                   ,[Eal_CurrentPayPeriod]
                                                                   ,[Eal_AllowanceCode]
                                                                   ,[Eal_PayrollPost]
                                                                   ,[Eal_AllowanceAmt]
                                                                   ,[Usr_Login]
                                                                   ,[Ludatetime])
                                                             VALUES (@EmployeeId
                                                                   ,@PayPeriod
                                                                   ,'13MONONTAX'
                                                                   ,'False'
                                                                   ,@Ebn_NontaxAmt
                                                                   ,@UserLogin
                                                                   ,GetDate())
                                                        
                                                        --Insert Taxable Allowance
                                                        IF @Ebn_TaxAmt > 0.0
                                                        INSERT INTO [T_EmployeeAllowance]
                                                                   ([Eal_EmployeeId]
                                                                   ,[Eal_CurrentPayPeriod]
                                                                   ,[Eal_AllowanceCode]
                                                                   ,[Eal_PayrollPost]
                                                                   ,[Eal_AllowanceAmt]
                                                                   ,[Usr_Login]
                                                                   ,[Ludatetime])
                                                             VALUES (@EmployeeId
                                                                   ,@PayPeriod
                                                                   ,'13MOTAX'
                                                                   ,'False'
                                                                   ,@Ebn_TaxAmt
                                                                   ,@UserLogin
                                                                   ,GetDate())

                                                        --Update Post Flag for Bonus
                                                            UPDATE [T_EmployeeBonus]
                                                               SET [Ebn_Post] = 'True'
                                                             WHERE [Ebn_Payperiod] = @PayPeriod
                                                               AND [Ebn_EmployeeId] = @EmployeeId";
            // Clark/13thMonth END

            public const string insertEmployeeBonus = @"DELETE FROM dbo.T_EmployeeBonus WHERE Ebn_Payperiod = @PayPeriod AND Ebn_EmployeeId = @EmployeeId
                                                                        DECLARE @COSTCENTER AS CHAR(12) SET @COSTCENTER = (SELECT Emt_CostCenterCode FROM T_EmployeeMaster WHERE Emt_EmployeeID = @EmployeeID)
                                                                        INSERT INTO [T_EmployeeBonus]
                                                                        VALUES(@PayPeriod
                                                                        ,@EmployeeId
                                                                        ,@SalaryRate
                                                                        ,@PayrollType
                                                                        ,0
                                                                        ,0
                                                                        ,@Amount
                                                                        ,0
                                                                        ,0
                                                                        ,0
                                                                        ,0
                                                                        ,@UserLogin
                                                                        ,GetDate(),@COSTCENTER)";

            public const string insertEmployeeBonusSep = @"DELETE FROM dbo.T_EmployeeBonusSep WHERE Ebn_EmployeeId = @EmployeeId
                                                                        DECLARE @COSTCENTER AS CHAR(12) SET @COSTCENTER = (SELECT Emt_CostCenterCode FROM T_EmployeeMaster WHERE Emt_EmployeeID = @EmployeeID)
                                                                        INSERT INTO [T_EmployeeBonusSep]
                                                                        VALUES(@PayPeriod
                                                                        ,@EmployeeId
                                                                        ,@SalaryRate
                                                                        ,@PayrollType
                                                                        ,0
                                                                        ,0
                                                                        ,@Amount
                                                                        ,0
                                                                        ,0
                                                                        ,0
                                                                        ,0
                                                                        ,@UserLogin
                                                                        ,GetDate(),@COSTCENTER)";

            public const string insertEmployeePayrollCalcSpecial = @"DELETE FROM T_EmployeePayrollCalcSpecial 
                                                                    WHERE Epc_EmployeeId = @EmployeeId 
                                                                    AND Epc_CurrentPayPeriod = @PayPeriod

                                                                    INSERT INTO T_EmployeePayrollCalcSpecial
                                                                    SELECT  DISTINCT @EmployeeId,
                                                                    @PayPeriod,
                                                                    @SalaryRate,
                                                                    @HourlyRate,    
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    0,0,0,0,0,0,0,0,0,0,
                                                                    @Tax,@NTax,@Tax,
                                                                    0,0,0,0,0,0,0,0,0,0,0,
                                                                    T_EmployeeMaster.Emt_CostCenterCode,
                                                                    T_EmployeeMaster.Emt_PositionCode,
                                                                    T_EmployeeMaster.Emt_EmploymentStatus,
                                                                    T_EmployeeMaster.Emt_PayrollType,
                                                                    T_EmployeeMaster.Emt_PaymentMode,
                                                                    T_EmployeeMaster.Emt_BankCode,
                                                                    T_EmployeeMaster.Emt_BankAccountNo,
                                                                    T_EmployeeMaster.Emt_TaxCode,
                                                                    T_EmployeeMaster.Emt_WorkType,
                                                                    T_EmployeeMaster.Emt_WorkGroup,
                                                                    T_EmployeeMaster.Emt_HDMFCode,
                                                                    T_EmployeeMaster.Emt_HDMFFixedContrib,
                                                                    T_EmployeeMaster.Emt_SSSCode,
                                                                    T_EmployeeMaster.Emt_SSSFixedContrib,
                                                                    T_EmployeeMaster.Emt_PhilhealthCode,
                                                                    T_EmployeeMaster.Emt_PhilhealthFixedContrib,
                                                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                                                    T_EmployeeMaster.Emt_ConfidentialPayroll,
                                                                    0,0,@UserLogin,GETDATE()
                                                                    FROM T_EmployeeMaster 
                                                                    WHERE Emt_employeeID = @EmployeeId";
            public const string insertEmpDeductionDetailSpecial = @"SELECT T_EmployeeDeductionLedger.Edl_EmployeeID
                                                                         , T_EmployeeDeductionLedger.Edl_DeductionCode
                                                                         , T_EmployeeDeductionLedger.Edl_StartDeductionDate
                                                                         ,  @PayPeriod
                                                                         ,  @PayPeriod
                                                                         , '00' as Edd_SeqNo
                                                                         , 'P' as Edd_PaymentType
                                                                         , Edl_DeductionAmount - (Edl_DeferredAmount + Edl_PaidAmount) as Edd_Amount
                                                                         , 0 as Edd_FromDeferred
                                                                         , 0 as Edd_PaymentFlag
                                                                         , @UserLogin
                                                                         , GETDATE()
                                                                    FROM T_EmployeeDeductionLedger
                                                                    WHERE T_EmployeeDeductionLedger.Edl_EmployeeID  = @EmployeeID
                                                                    AND Edl_DeferredAmount + Edl_PaidAmount < Edl_DeductionAmount
                                                                    AND Edl_AmortizationAmount > 0
                                                                    AND Edl_DeductionCode = 'BONUSDEDN'";
            public const string updateBonusAmount = @"UPDATE T_EmployeeBonus
                                                                    SET  Ebn_UsedNontaxAmt = isnull((Select Sum(Eal_AllowanceAmt)
						                                                                    From T_EmployeeAllowanceHist
						                                                                    Where Left(Eal_CurrentPayPeriod,4) = '{0}' -- payperiod year
							                                                                    and  Eal_AllowanceCode ='13MONONTAX'
							                                                                    and Eal_PayrollPost = 1
							                                                                    and Eal_Employeeid = Ebn_EmployeeID), 0)
                                                                    FROM T_EmployeeBonus 
                                                                    WHERE Ebn_Payperiod = '{1}'--@PayPeriod
                                                                    and Ebn_EmployeeID = '{2}'

                                                                    UPDATE T_EmployeeBonus
                                                                    SET Ebn_NontaxAmt = Case When {3} - Ebn_UsedNontaxAmt > 0  then
						                                                                    Case When Ebn_BonusAmt > {3} - Ebn_UsedNontaxAmt  then
							                                                                        {3} - Ebn_UsedNontaxAmt 
						                                                                    Else    Ebn_BonusAmt
					                                                                        End
				                                                                        Else
						                                                                    0
					                                                                    End 
                                                                    WHERE Ebn_Payperiod = '{1}'--@PayPeriod
                                                                    and Ebn_EmployeeID = '{2}'

                                                                    UPDATE T_EmployeeBonus
                                                                    SET Ebn_taxAmt = Ebn_BonusAmt - Ebn_NontaxAmt
                                                                    WHERE Ebn_Payperiod = '{1}'--@PayPeriod
                                                                    and Ebn_EmployeeID = '{2}'";
            public const string updateBonusSepAmount = @"UPDATE T_EmployeeBonusSep
                                                                    SET  Ebn_UsedNontaxAmt = isnull((Select Sum(Eal_AllowanceAmt)
						                                                                    From T_EmployeeAllowanceHist
						                                                                    Where Left(Eal_CurrentPayPeriod,4) = '{0}' -- payperiod year
							                                                                    and  Eal_AllowanceCode ='13MONONTAX'
							                                                                    and Eal_PayrollPost = 1
							                                                                    and Eal_Employeeid = Ebn_EmployeeID), 0)
                                                                    FROM T_EmployeeBonusSep 
                                                                    WHERE Ebn_Payperiod = '{1}'--@PayPeriod
                                                                    and Ebn_EmployeeID = '{2}'

                                                                    UPDATE T_EmployeeBonusSep
                                                                    SET Ebn_NontaxAmt = Case When {3} - Ebn_UsedNontaxAmt > 0  then
						                                                                    Case When Ebn_BonusAmt > {3} - Ebn_UsedNontaxAmt  then
							                                                                        {3} - Ebn_UsedNontaxAmt 
						                                                                    Else    Ebn_BonusAmt
					                                                                        End
				                                                                        Else
						                                                                    0
					                                                                    End 
                                                                    WHERE Ebn_Payperiod = '{1}'--@PayPeriod
                                                                    and Ebn_EmployeeID = '{2}'

                                                                    UPDATE T_EmployeeBonusSep
                                                                    SET Ebn_taxAmt = Ebn_BonusAmt - Ebn_NontaxAmt
                                                                    WHERE Ebn_Payperiod = '{1}'--@PayPeriod
                                                                    and Ebn_EmployeeID = '{2}'";

            #endregion
            #region [For Clearance Computation]
            public const string insertPayrollSeparation = @"INSERT INTO T_EmployeePayrollTransactionSep(Ept_EmployeeId
                                                       ,Ept_CurrentPayPeriod
                                                       ,Ept_AbsentHr
                                                       ,Ept_RegularHr
                                                       ,Ept_RegularOTHr
                                                       ,Ept_RegularNDHr
                                                       ,Ept_RegularOTNDHr
                                                       ,Ept_RestdayHr
                                                       ,Ept_RestdayOTHr
                                                       ,Ept_RestdayNDHr
                                                       ,Ept_RestdayOTNDHr
                                                       ,Ept_LegalHolidayHr
                                                       ,Ept_LegalHolidayOTHr
                                                       ,Ept_LegalHolidayNDHr
                                                       ,Ept_LegalHolidayOTNDHr
                                                       ,Ept_SpecialHolidayHr
                                                       ,Ept_SpecialHolidayOTHr
                                                       ,Ept_SpecialHolidayNDHr
                                                       ,Ept_SpecialHolidayOTNDHr
                                                       ,Ept_PlantShutdownHr
                                                       ,Ept_PlantShutdownOTHr
                                                       ,Ept_PlantShutdownNDHr
                                                       ,Ept_PlantShutdownOTNDHr
                                                       ,Ept_CompanyHolidayHr
                                                       ,Ept_CompanyHolidayOTHr
                                                       ,Ept_CompanyHolidayNDHr
                                                       ,Ept_CompanyHolidayOTNDHr
                                                       ,Ept_RestdayLegalHolidayHr
                                                       ,Ept_RestdayLegalHolidayOTHr
                                                       ,Ept_RestdayLegalHolidayNDHr
                                                       ,Ept_RestdayLegalHolidayOTNDHr
                                                       ,Ept_RestdaySpecialHolidayHr
                                                       ,Ept_RestdaySpecialHolidayOTHr
                                                       ,Ept_RestdaySpecialHolidayNDHr
                                                       ,Ept_RestdaySpecialHolidayOTNDHr
                                                       ,Ept_RestdayCompanyHolidayHr
                                                       ,Ept_RestdayCompanyHolidayOTHr
                                                       ,Ept_RestdayCompanyHolidayNDHr
                                                       ,Ept_RestdayCompanyHolidayOTNDHr
                                                       ,Ept_LaborHrsAdjustmentAmt
                                                       ,Ept_TaxAdjustmentAmt
                                                       ,Ept_NonTaxAdjustmentAmt
                                                       ,Ept_TaxAllowanceAmt
                                                       ,Ept_NonTaxAllowanceAmt
                                                       ,Ept_RestdayLegalHolidayCount
                                                       ,Ept_WorkingDay
                                                       ,Ept_PayrollType
                                                       ,Ept_RestdayPlantShutdownHr
                                                       ,Ept_RestdayPlantShutdownOTHr
                                                       ,Ept_RestdayPlantShutdownNDHr
                                                       ,Ept_RestdayPlantShutdownOTNDHr
                                                       ,Usr_Login
                                                       ,Ludatetime)
                                                 VALUES
                                                       (@EmployeeId
                                                       ,@PayPeriod
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,0
                                                       ,0
                                                       ,@PayType
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,'0.00'
                                                       ,@UserLogin
                                                       ,GetDate())";
            public const string insertDeductionDetailSep = @"INSERT INTO T_EmployeeDeductionDetailSep
                                                            -- For Deffered Cycles
                                                            SELECT    T_EmployeeDeductionDeffered.Edd_EmployeeID
                                                              , T_EmployeeDeductionDeffered.Edd_DeductionCode
                                                              , T_EmployeeDeductionDeffered.Edd_StartDeductionDate
                                                                 , @PayPeriod as Edd_CurrentPayPeriod
                                                              , T_EmployeeDeductionDeffered.Edd_PayPeriod   as Edd_PayPeriod
                                                              , T_EmployeeDeductionDeffered.Edd_SeqNo   as Edd_SeqNo
                                                              , 'P' as Edd_PaymentType
                                                              , T_EmployeeDeductionDeffered.Edd_DeferredAmount as Edd_Amount
                                                              , 1 as  Edd_FromDeferred
                                                              , 0 as Edd_PaymentFlag
                                                                 , @UserLogin
                                                                 , GetDate()
                                                            FROM T_EmployeeDeductionLedger
                                                            INNER JOIN T_EmployeeDeductionDeffered on T_EmployeeDeductionDeffered.Edd_EmployeeID = T_EmployeeDeductionLedger.Edl_EmployeeID
                                                                AND T_EmployeeDeductionDeffered.Edd_DeductionCode = T_EmployeeDeductionLedger.Edl_DeductionCode 
                                                                AND T_EmployeeDeductionDeffered.Edd_StartDeductionDate = T_EmployeeDeductionLedger.Edl_StartDeductionDate 
                                                            Inner Join T_DeductionCodeMaster on Dcm_DeductionCode = Edl_DeductionCode
                                                              and Dcm_DeductionType = 'C'
                                                            WHERE T_EmployeeDeductionLedger.Edl_EmployeeID = @EmployeeID 

                                                            UNION
                                                            -- For Current Cycles
                                                            SELECT T_EmployeeDeductionLedger.Edl_EmployeeID
                                                             , T_EmployeeDeductionLedger.Edl_DeductionCode
                                                             , T_EmployeeDeductionLedger.Edl_StartDeductionDate
                                                             , @PayPeriod as Edd_CurrentPayPeriod
                                                             , @PayPeriod as Edd_PayPeriod
                                                             , '00' as Edd_SeqNo
                                                             , 'P' as Edd_PaymentType
                                                             , Edl_DeductionAmount - (Edl_DeferredAmount + Edl_PaidAmount) as Edd_Amount
                                                                , 0 as Edd_FromDeferred
                                                                , 0 as Edd_PaymentFlag
                                                                , @UserLogin
                                                                , GetDate()
                                                            FROM T_EmployeeDeductionLedger
                                                            Inner Join T_DeductionCodeMaster on Dcm_DeductionCode = Edl_DeductionCode
                                                              and Dcm_DeductionType = 'C'
                                                            WHERE T_EmployeeDeductionLedger.Edl_EmployeeID = @EmployeeID
                                                                AND Edl_DeferredAmount + Edl_PaidAmount < Edl_DeductionAmount
                                                                AND Edl_AmortizationAmount > 0";

            public const string checkClearanceData0 = @"SELECT Ppm_PayPeriod FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C'";
            public const string checkClearanceData1 = @"SELECT * FROM T_EmployeeMaster WHERE Emt_EmployeeID = @EmployeeID";
            public const string checkClearanceData2 = @"SELECT DISTINCT LEFT(Elr_CurrentPayPeriod,4) FROM T_EmployeeLeaveRefund WHERE Elr_EmployeeId = @EmployeeID";

            public const string checkClearanceData3 = @"declare @VLDMINIMIS as decimal(9,2)
                                                        set @VLDMINIMIS = (select Pmt_NumericValue from T_ParameterMaster
                                                        where Pmt_ParameterID ='VLDMINIMIS')

                                                        SELECT	DISTINCT
                                                                Emt_EmployeeID EmployeeID, 
                                                                Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
                                                                Ltm_LeaveType LeaveType, 
                                                                Ltm_LeaveDesc LeaveDesc,
                                                                CASE WHEN Ltm_LeaveType = 'VL' THEN 'NON-TAXABLE'
                                                                ELSE 'TAXABLE'
                                                                END	 TaxClass,
                                                                CASE Ltm_LeaveType
                                                                WHEN 'VL' THEN 
                                                                        CASE WHEN Elm_VLBalance <= @VLDMINIMIS THEN
                                                                                Elm_VLBalance
                                                                        ELSE	
                                                                                @VLDMINIMIS
                                                                        END
                                                                WHEN 'SL' THEN Elm_SLBalance
                                                                WHEN 'PL' THEN Elm_SLBalance 
                                                                WHEN 'BL' THEN Elm_SLBalance
                                                                WHEN 'DL' THEN Elm_SLBalance
                                                                ELSE  0   END LeaveBalance,
                                                        0 RefundAmount,
                                                        Emt_SalaryRate,
                                                        Emt_PayrollType,
                                                        Emt_BillingRate,
                                                        Emt_BillingPayrollType,
                                                        Emt_TaxRate,
                                                        Emt_TaxRatePayrollType, 1 IncludeFlag
                                                        FROM	T_LeaveTypeMaster
                                                        JOIN	T_EmployeeMaster ON	Emt_EmployeeID = @EmployeeID
                                                        JOIN	T_EmployeeLeaveMaster ON Elm_EmployeeId = Emt_EmployeeID
                                                        WHERE	Ltm_ConvertibleToCash = 1
                                                        AND		Ltm_Status = 'A'
                                                        AND     (CASE Ltm_LeaveType
                                                                WHEN 'VL' THEN Elm_VLBalance
                                                                WHEN 'SL' THEN Elm_SLBalance
                                                                WHEN 'PL' THEN Elm_SLBalance
                                                                WHEN 'BL' THEN Elm_SLBalance
                                                                WHEN 'DL' THEN Elm_SLBalance
                                                                ELSE  0 END) > 0

                                                UNION ALL

                                                SELECT	DISTINCT
                                                                Emt_EmployeeID EmployeeID, 
                                                                Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
                                                                Ltm_LeaveType LeaveType, 
                                                                Ltm_LeaveDesc LeaveDesc,
                                                                'TAXABLE' TaxClass,
                                                                Elm_VLBalance - @VLDMINIMIS LeaveBalance,
                                                        0 RefundAmount,
                                                        Emt_SalaryRate SalaryRate,
                                                        Emt_PayrollType PayrollType,
                                                        Emt_BillingRate BillingRate,
                                                        Emt_BillingPayrollType BillingPayrollType,
                                                        Emt_TaxRate TaxRate,
                                                        Emt_TaxRatePayrollType TaxRatePayrollType, 1 IncludeFlag
                                                        FROM	T_LeaveTypeMaster
                                                        JOIN	T_EmployeeMaster ON	Emt_EmployeeID = @EmployeeID
                                                         JOIN	T_EmployeeLeaveMaster ON Elm_EmployeeId = Emt_EmployeeID
                                                      WHERE	Ltm_Status = 'A'
                                                        AND    Ltm_LeaveType='VL'  
                                                        AND Elm_VLBalance > @VLDMINIMIS
                                                        ORDER BY 1";

            public const string checkClearanceData4 = @"SELECT	Ccd_Bonus FROM	T_CompanyMaster";

            public const string checkClearanceData5 = @"SELECT DISTINCT LEFT(Ebn_PayPeriod, 4) FROM	T_EmployeeBonus WHERE Ebn_PayPeriod = @PayPeriod AND Ebn_EmployeeId = @EmployeeId";

            public const string checkClearanceData6 = @"SELECT	Emt_EmployeeID EmployeeID, 
                                                                Emt_Firstname + ' ' + Emt_Lastname EmployeeName, 
                                                                0 RefundAmount,
                                                                Emt_SalaryRate,
                                                                Emt_PayrollType,
                                                                Emt_BillingRate,
                                                                Emt_BillingPayrollType,
                                                                Emt_TaxRate,
                                                                Emt_TaxRatePayrollType, Emt_CostCenterCode
                                                        FROM	T_EmployeeMaster
                                                        WHERE	Emt_EmployeeID = @EmployeeID
                                                        ORDER BY 1  ";
            #endregion
            #endregion

            #region Common Queries by Rendell Uy
            public const string getGroupPremiums = @"SELECT * FROM DBO.NumericValue('NIGHTPREM')
                                                    SELECT * FROM GETGROUPPREMIUMS('REG', 'false', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('REST', 'true', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('HOL', 'false', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('SPL', 'false', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('PSD', 'false', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('COMP', 'false', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('HOL', 'true', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('SPL', 'true', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('COMP', 'true', @PremiumGroup)
                                                    SELECT * FROM GETGROUPPREMIUMS('PSD', 'true', @PremiumGroup)";
            #endregion
        }
        #endregion

        #region storedProcedures

        public class StoredProcedures
        {
            // user master
            public const string spUserMasterFetchAll = "spUserMasterFetchAll";
            public const string spUserMasterFetch = "spUserMasterFetch";
            public const string spUserMasterAdd = "spUserMasterAdd";
            public const string spUserMasterUpdate = "spUserMasterUpdate";
            public const string spUserMasterDelete = "spUserMasterDelete";
            public const string spUserMasterFetchUserNumber = "spUserMasterFetchUserNumber";

            //user grant
            public const string spUserGrantAdd = "spUserGrantAdd";
            public const string spUserGrantDelete = "spUserGrantDelete";
            public const string spUserGrantUpdate = "spUserGrantUpdate";
            public const string spUserGrantFetch = "spUserGrantFetch";
            public const string spUserGrantFetchAll = "spUserGrantFetchAll";

            // User Group
            public const string spUserGroupFetchAll = "spUserGroupFetchAll";
            public const string spUserGroupHeaderAdd = "spUserGroupHeaderAdd";
            public const string spUserGroupMemberAdd = "spUserGroupMemberAdd";
            public const string spUserGroupHeaderUpdate = "spUserGroupHeaderUpdate";
            public const string spUserGroupHeaderDelete = "spUserGroupHeaderDelete";
            public const string spUserGroupMemberDelete = "spUserGroupMemberDelete";
            public const string spUserGroupMemberDeleteCancel = "spUserGroupMemberDeleteCancel";

            //Company Master
            public const string spCompanyMasterAdd = "spCompanyMasterAdd";
            public const string spCompanyMasterDelete = "spCompanyMasterDelete";
            public const string spCompanyMasterUpdate = "spCompanyMasterUpdate";
            public const string spCompanyMasterFetch = "spCompanyMasterFetch";
            public const string spCompanyMasterFetchAll = "spCompanyMasterFetchAll";

            //Parameter Master
            public const string spParameterMasterAdd = "spParameterMasterAdd";
            public const string spParameterMasterUpdate = "spParameterMasterUpdate";
            public const string spParameterMasterDelete = "spParameterMasterDelete";
            public const string spParameterMasterFetchALL = "spParameterMasterFetchALL";

            //Tax Exemption
            public const string spTaxExemptionAdd = "spTaxExemptionAdd";
            public const string spTaxExemptionUpdate = "spTaxExemptionUpdate";
            public const string spTaxExemptionDelete = "spTaxExemptionDelete";
            public const string spTaxExemptionFetch = "spTaxExemptionFetch";
            public const string spTaxExemptionFetchALL = "spTaxExemptionFetchALL";
            public const string spTaxExemptionCount = "spTaxExemptionCount";

            //Day Code Master
            public const string spDayCodeMasterAdd = "spDayCodeMasterAdd";
            public const string spDayCodeMasterUpdate = "spDayCodeMasterUpdate";
            public const string spDayCodeMasterDelete = "spDayCodeMasterDelete";
            public const string spDayCodeMasterFetchAll = "spDayCodeMasterFetchAll";
            public const string spDayCodeMasterFetch = "spDayCodeMasterFetch";

            //DeductionCode Master
            public const string spDeductionCodeMasterAdd = "spDeductionCodeMasterAdd";
            public const string spDeductionCodeMasterUpdate = "spDeductionCodeMasterUpdate";
            public const string spDeductionCodeMasterDelete = "spDeductionCodeMasterDelete";
            public const string spDeductionCodeMasterFetchAll = "spDeductionCodeMasterFetchAll";

            //Holiday Master
            public const string spHolidayMasterAdd = "spHolidayMasterAdd";
            public const string spHolidayMasterUpdate = "spHolidayMasterUpdate";
            public const string spHolidayMasterDelete = "spHolidayMasterDelete";
            public const string spHolidayMasterFetchAll = "spHolidayMasterFetchAll";

            //Premium Rate Master
            public const string spDayPremiumMasterAdd = "spDayPremiumMasterAdd";
            public const string spDayPremiumMasterUpdate = "spDayPremiumMasterUpdate";
            public const string spDayPremiumMasterDelete = "spDayPremiumMasterDelete";
            public const string spDayPremiumMasterFetchAll = "spDayPremiumMasterFetchAll";

            //Leave Type Master
            public const string spLeaveTypeMasterAdd = "spLeaveTypeMasterAdd";
            public const string spLeaveTypeMasterUpdate = "spLeaveTypeMasterUpdate";
            public const string spLeaveTypeMasterDelete = "spLeaveTypeMasterDelete";
            public const string spLeaveTypeMasterFetchAll = "spLeaveTypeMasterFetchAll";

            //Payroll Period Control master
            public const string spPayrollPeriodControlMasterAdd = "spPayrollPeriodControlMasterAdd";
            public const string spPayrollPeriodControlMasterUpdate = "spPayrollPeriodControlMasterUpdate";
            public const string spPayrollPeriodControlMasterDelete = "spPayrollPeriodControlMasterDelete";
            public const string spPayrollPeriodControlMasterFetch = "spPayrollPeriodControlMasterFetch";
            public const string spPayrollPeriodControlMasterFetchALL = "spPayrollPeriodControlMasterFetchALL";

            //Allowance Master
            public const string spAllowanceCodeMasterAdd = "spAllowanceCodeMasterAdd";
            public const string spAllowanceCodeMasterUpdate = "spAllowanceCodeMasterUpdate";
            public const string spAllowanceCodeMasterDelete = "spAllowanceCodeMasterDelete";
            public const string spAllowanceCodeMasterFetchAll = "spAllowanceCodeMasterFetchAll";
            public const string spAllowanceCodeMasterFetch = "spAllowanceCodeMasterFetch";

            //My Master File
            public const string spMyMasterFileAdd = "spMyMasterFileAdd";
            public const string spMyMasterFileUpdate = "spMyMasterFileUpdate";
            public const string spMyMasterFileDelete = "spMyMasterFileDelete";
            public const string spMyMasterFileFetchAll = "spMyMasterFileFetchAll";
            public const string spMyMasterFileFetch = "spMyMasterFileFetch";

            //Shift Code Master
            public const string spShiftCodeMasterAdd = "spShiftCodeMasterAdd";
            public const string spShiftCodeMasterUpdate = "spShiftCodeMasterUpdate";
            public const string spShiftCodeMasterDelete = "spShiftCodeMasterDelete";
            public const string spShiftCodeMasterFetchALL = "spShiftCodeMasterFetchALL";

            //Employee Master
            public const string spEmployeeMasterAdd = "spEmployeeMasterAdd";
            public const string spEmployeeMasterUpdate = "spEmployeeMasterUpdate";
            public const string spEmployeeMasterDelete = "spEmployeeMasterDelete";
            public const string spEmployeeMasterFetchALL = "spEmployeeMasterFetchALL";
            public const string spEmployeeMasterFetchBeneficiary = "spEmployeeMasterFetchBeneficiary";

            //Annual Tax Table
            public const string spAnnualTaxScheduleFetchAll = "spAnnualTaxScheduleFetchAll";
            public const string spAnnualTaxScheduleAdd = "spAnnualTaxScheduleAdd";
            public const string spAnnualTaxScheduleUpdate = "spAnnualTaxScheduleUpdate";
            public const string spAnnualTaxScheduleDelete = "spAnnualTaxScheduleDelete";
            public const string spAnnualTaxScheduleFetch = "spAnnualTaxScheduleFetch";

            //Contribution Schedule Master
            public const string spContributionScheduleMasterAdd = "spContributionScheduleMasterAdd";
            public const string spContributionScheduleMasterUpdate = "spContributionScheduleMasterUpdate";
            public const string spContributionScheduleMasterDelete = "spContributionScheduleMasterDelete";
            public const string spContributionScheduleMasterFetchAll = "spContributionScheduleMasterFetchAll";
            public const string spContributionScheduleMasterFetch = "spContributionScheduleMasterFetch";

            //Tax Table
            public const string spTaxTableAdd = "spTaxTableAdd";
            public const string spTaxTableUpdate = "spTaxTableUpdate";
            public const string spTaxTableDelete = "spTaxTableDelete";
            public const string spTaxTableFetch = "spTaxTableFetch";
            public const string spTaxTableFetchALL = "spTaxTableFetchALL";

            //Employee Leave Master
            public const string spEmployeeLeaveMasterAdd = "spEmployeeLeaveMasterAdd";
            public const string spEmployeeLeaveMasterUpdate = "spEmployeeLeaveMasterUpdate";
            public const string spEmployeeLeaveMasterDelete = "spEmployeeLeaveMasterDelete";
            public const string spEmployeeLeaveMasterFetch = "spEmployeeLeaveMasterFetch";
            public const string spEmployeeLeaveMasterFetchAll = "spEmployeeLeaveMasterFetchAll";

            //Employee Beneficiary Master
            public const string spEmployeeBeneficiaryMasterAdd = "spEmployeeBeneficiaryMasterAdd";
            public const string spEmployeeBeneficiaryMasterUpdate = "spEmployeeBeneficiaryMasterUpdate";
            public const string spEmployeeBeneficiaryMasterDelete = "spEmployeeBeneficiaryMasterDelete";
            public const string spEmployeeBeneficiaryMasterFetch = "spEmployeeBeneficiaryMasterFetch";
            public const string spEmployeeBeneficiaryMasterFetchAll = "spEmployeeBeneficiaryMasterFetchAll";

            //Employee Ledger
            public const string spEmployeeLedgerAdd = "spEmployeeLedgerAdd";
            public const string spEmployeeLedgerUpdate = "spEmployeeLedgerUpdate";
            public const string spEmployeeLedgerDelete = "spEmployeeLedgerDelete";
            public const string spEmployeeLedgerFetchALL = "spEmployeeLedgerFetchALL";

            //Generic Master
            public const string spGenericMasterAdd = "spGenericMasterAdd";
            public const string spGenericMasterUpdate = "spGenericMasterUpdate";
            public const string spGenericMasterDelete = "spGenericMasterDelete";
            public const string spGenericMasterFetch = "spGenericMasterFetch";
            public const string spGenericMasterFetchHumidity = "spGenericMasterFetchHumidity";
            public const string spGenericMasterFetchPiling = "spGenericMasterFetchPiling";
            public const string spGenericMasterFetchAll = "spGenericMasterFetchAll";

            //Report Print Counter
            public const string spReportPrintCounterAdd = "spReportPrintCounterAdd";
            public const string spReportPrintCounterFetch = "spReportPrintCounterFetch";
            public const string spReportPrintCounterInc = "spReportPrintCounterInc";

            //Account Master
            public const string spAccountMasterHeaderAdd = "spAccountMasterHeaderAdd";
            public const string spAccountMasterDetailAdd = "spAccountMasterDetailAdd";
            public const string spAccountMasterHeaderDelete = "spAccountMasterHeaderDelete";
            public const string spAccountMasterDetailDelete = "spAccountMasterDetailDelete";
            public const string spAccountMasterFetch = "spAccountMasterFetch";
            public const string spAccountMasterFetchall = "spAccountMasterFetchall";
            public const string spAccountMasterHeaderFetch = "spAccountMasterHeaderFetch";
            public const string spAccountMasterHeaderUpdate = "spAccountMasterHeaderUpdate";
            public const string spAccountMasterDetailUpdate = "spAccountMasterDetailUpdate";

            //Department Code Master
            public const string spDepartmentCodeAdd = "spDepartmentCodeAdd";
            public const string spDepartmentCodeUpdate = "spDepartmentCodeUpdate";
            public const string spDepartmentCodeDelete = "spDepartmentCodeDelete";
            public const string spDepartmentCodeFetchALL = "spDepartmentCodeFetchALL";
            public const string spDepartmentCodeFetch = "spDepartmentCodeFetch";
            public const string spDepartmentCodeShowAllActiveStats = "spDepartmentCodeShowAllActiveStats";
            public const string spDepartmentCodeShowAllCancelledStats = "spDepartmentCodeShowAllCancelledStats";
            public const string spDepartmentCodeShowAllOnHoldStats = "spDepartmentCodeShowAllOnHoldStats";

            //Cost Center Master
            public const string spCostCenterMasterAdd = "spCostCenterMasterAdd";
            public const string spCostCenterMasterUpdate = "spCostCenterMasterUpdate";
            public const string spCostCenterMasterDelete = "spCostCenterMasterDelete";
            public const string spCostCenterMasterFetchALL = "spCostCenterMasterFetchALL";
            public const string spCostCenterMasterFetch = "spCostCenterMasterFetch";

            //Product Master
            public const string spCostLookUp = "spCostLookUp";

            //FG Endorse
            public const string spCostCenterMasterFetchForFG = "spCostCenterMasterFetchForFG";

            //Division Code
            public const string spDivisionCodeAdd = "spDivisionCodeAdd";
            public const string spDivisionCodeUpdate = "spDivisionCodeUpdate";
            public const string spDivisionCodeDelete = "spDivisionCodeDelete";
            public const string spDivisionCodeFetchALL = "spDivisionCodeFetchALL";
            public const string spDivisionCodeFetch = "spDivisionCodeFetch";

            //Process Code Master
            public const string spProcessCodeMasterAdd = "spProcessCodeMasterAdd";
            public const string spProcessCodeMasterDelete = "spProcessCodeMasterDelete";
            public const string spProcessCodeMasterFetch = "spProcessCodeMasterFetch";
            public const string spProcessCodeMasterFetchAll = "spProcessCodeMasterFetchAll";
            public const string spProcessCodeMasterUpdate = "spProcessCodeMasterUpdate";
            public const string spProcessCodeMasterFetch1 = "spProcessCodeMasterFetch1";

            //Section Code Master
            public const string spSectionCodeAdd = "spSectionCodeAdd";
            public const string spSectionCodeUpdate = "spSectionCodeUpdate";
            public const string spSectionCodeDelete = "spSectionCodeDelete";
            public const string spSectionCodeFetchALL = "spSectionCodeFetchALL";
            public const string spSectionCodeFetch = "spSectionCodeFetch";

            //Subsection Code Master
            public const string spSubSectionCodeAdd = "spSubSectionCodeAdd";
            public const string spSubSectionCodeUpdate = "spSubSectionCodeUpdate";
            public const string spSubSectionCodeDelete = "spSubSectionCodeDelete";
            public const string spSubSectionCodeFetchALL = "spSubSectionCodeFetchALL";
            public const string spSubSectionCodeFetch = "spSubSectionCodeFetch";
            public const string spSubSectionCodeShowAllActive = "spSubSectionCodeShowAllActive";
            public const string spSubSectionCodeShowAllCancelled = "spSubSectionCodeShowAllCancelled";
            public const string spSubSectionCodeShowAllOnhold = "spSubSectionCodeShowAllOnhold";

            //Employee Position Movement
            public const string spEmployeePositionMovementAdd = "spEmployeePositionMovementAdd";
            public const string spEmployeePositionMovementDelete1 = "spEmployeePositionMovementDelete1";
            public const string spEmployeePositionMovementUpdate = "spEmployeePositionMovementUpdate";
            public const string spEmployeePositionMovementFetchALL = "spEmployeePositionMovementFetchALL";

            //Employee Costcenter Movement
            public const string spEmpCostcenterMovementAdd = "spEmpCostcenterMovementAdd";
            public const string spEmpCostcenterMovementDelete1 = "spEmpCostcenterMovementDelete1";
            public const string spEmpCostcenterMovementUpdate = "spEmpCostcenterMovementUpdate";
            public const string spEmpCostcenterMovementFetchALL = "spEmpCostcenterMovementFetchALL";

            //Employee Salary Movement
            public const string spEmployeeSalaryMovementAdd = "spEmployeeSalaryMovementAdd";
            public const string spEmployeeSalaryMovementDelete1 = "spEmployeeSalaryMovementDelete1";
            public const string spEmployeeSalaryMovementUpdate = "spEmployeeSalaryMovementUpdate";
            public const string spEmployeeSalaryMovementFetchALL = "spEmployeeSalaryMovementFetchALL";

            //Annual Tax
            public const string spAnnualTaxTableFetchAll = "spAnnualTaxTableFetchAll";
            public const string spAnnualTaxTableAdd = "spAnnualTaxTableAdd";
            public const string spAnnualTaxTableUpdate = "spAnnualTaxTableUpdate";
            public const string spAnnualTaxTableDelete = "spAnnualTaxTableDelete";

            //Allowance Master
            public const string spAllowanceMasterAdd = "spAllowanceMasterAdd";
            public const string spAllowanceMasterUpdate = "spAllowanceMasterUpdate";
            public const string spAllowanceMasterDelete = "spAllowanceMasterDelete";
            public const string spAllowanceMasterFetchAll = "spAllowanceMasterFetchAll";
            public const string spAllowanceMasterFetch = "spAllowanceMasterFetch";

            //Premium Rate Master
            public const string spPremiumRateMasterAdd = "spPremiumRateMasterAdd";
            public const string spPremiumRateMasterUpdate = "spPremiumRateMasterUpdate";
            public const string spPremiumRateMasterDelete = "spPremiumRateMasterDelete";
            public const string spPremiumRateMasterFetchAll = "spPremiumRateMasterFetchAll";

            //Security Movement
            public const string spEmployeeSecurityMovementAdd = "spEmployeeSecurityMovementAdd";
            public const string spEmployeeSecurityMovementFetchAll = "spEmployeeSecurityMovementFetchAll";
            public const string spEmployeeSecurityMovementFetchAll1 = "spEmployeeSecurityMovementFetchAll1";

            //Employee Master Updates
            public const string spEmployeeMasterAddressUpdate = "spEmployeeMasterAddressUpdate";
            public const string spEmployeeMasterAddressFetchall = "spEmployeeMasterAddressFetchall";
            public const string spEmployeeMasterSecurityNoUpdate = "spEmployeeMasterSecurityNoUpdate";
            public const string spEmployeeMasterSecurityNoFetchall = "spEmployeeMasterSecurityNoFetchall";
            public const string spEmployeeMasterEducationFetchALL = "spEmployeeMasterEducationFetchALL";
            public const string spEmployeeMasterEducationUpdate = "spEmployeeMasterEducationUpdate";
            public const string spEmployeeMasterStatusFetchALL = "spEmployeeMasterStatusFetchALL";
            public const string spEmployeeMasterStatusUpdate = "spEmployeeMasterStatusUpdate";
            public const string spEmployeeMasterStatusGetEmployeeStatus = "spEmployeeMasterStatusGetEmployeeStatus";

            //Loan Collection Entry
            public const string spLoanCollectionFetchAll = "spLoanCollectionFetchAll";
            public const string spLoanCollectionAdd = "spLoanCollectionAdd";
            public const string spLoanCollectionUpdate = "spLoanCollectionUpdate";
            public const string spLoanCollectionDelete = "spLoanCollectionDelete";
            public const string spLoanCollectionCheckIfExist = "spLoanCollectionCheckIfExist";
            public const string spLoanCollectionGetEmployeeName = "spLoanCollectionGetEmployeeName";
            public const string spLoanCollectionGetDeductDesc = "spLoanCollectionGetDeductDesc";
            public const string spLoanCollectionCheckifYearMonthExist = "spLoanCollectionCheckifYearMonthExist";

            //Premium Remittance Entry
            public const string spPremiumRemittanceEntryAdd = "spPremiumRemittanceEntryAdd";
            public const string spPremiumRemittanceEntryUpdate = "spPremiumRemittanceEntryUpdate";
            public const string spPremiumRemittanceEntryDelete = "spPremiumRemittanceEntryDelete";
            public const string spPremiumRemittanceEntryFetchALL = "spPremiumRemittanceEntryFetchALL";
            public const string spPremiumRemittanceEntryFetchNew = "spPremiumRemittanceEntryFetchNew";
            public const string spPremiumRemittanceEntryCheckifRecordExist = "spPremiumRemittanceEntryCheckifRecordExist";
            public const string spPremiumRemittanceEntryGetEmployeeName = "spPremiumRemittanceEntryGetEmployeeName";
            public const string spPremiumRemittanceEntryGetDeductDesc = "spPremiumRemittanceEntryGetDeductDesc";
            public const string spPremiumRemittanceEntryCheckifYearMonthExist = "spPremiumRemittanceEntryCheckifYearMonthExist";

            //Batch Deduction Entry 
            public const string spBatchDeductionEntryAdd = "spBatchDeductionEntryAdd";
            public const string spBatchDeductionEntryUpdate = "spBatchDeductionEntryUpdate";
            public const string spBatchDeductionEntryDelete = "spBatchDeductionEntryDelete";
            public const string spBatchDeductionEntryFetchALL = "spBatchDeductionEntryFetchALL";
            public const string spIsWithCheck = "spIsWithCheck";
            public const string spIsAmortLookup = "spIsAmortLookup";
            public const string spPaymentStartCycle = "spPaymentStartCycle";
            public const string spFindDeductionDescription = "spFindDeductionDescription";
            public const string spFindEmployeeName = "spFindEmployeeName";
            public const string spFetchRecordToBeExportInExcel = "spFetchRecordToBeExportInExcel";

            //Employee Statistics
            public const string spEmployeeStatisticsFetchOne = "spEmployeeStatisticsFetchOne";
            public const string spEmployeeStatisticsFetchWithFilter = "spEmployeeStatisticsFetchWithFilter";
            public const string spFetchCostCenter = "spFetchCostCenter";
            public const string spGetFilteredData = "spGetFilteredData";

            //Individual Overtime Entry
            public const string spEmployeeOvertimeRecord = "spEmployeeOvertimeRecord";
            public const string spPayrollPeriod = "spPayrollPeriod";
            public const string spEmployeeOvertimeRecordAdd = "spEmployeeOvertimeRecordAdd";
            public const string spEmployeeOvertimeRecordUpdate = "spEmployeeOvertimeRecordUpdate";
            public const string spEmployeeDeleteRecord = "spEmployeeDeleteRecord";
            public const string spEmployeeLogLedgerUpdate = "spEmployeeLogLedgerUpdate";
            public const string spEmployeeShiftSchedule = "spEmployeeShiftSchedule";
            public const string spEmployeeShiftCode = "spEmployeeShiftCode";
            public const string spGetMinimumOTHour = "spGetMinimumOTHour";
            public const string spGetOTFraction = "spGetOTFraction";
            public const string spOTCutOff = "spOTCutOff";
            public const string spEmployeeOvertimeExist = "spEmployeeOvertimeExist";
            public const string spDayCode = "spDayCode";
            public const string spFetchShiftHoursAndDayCode = "spFetchShiftHoursAndDayCode";
            public const string spGetCurrentPayrollCycle = "spGetCurrentPayrollCycle";
            public const string FetchAllowableNoOfOT = "FetchAllowableNoOfOT";
            public const string FetchMaxOTtype = "FetchMaxOTtype";
            public const string FetchEmployeeNoOfOTFiled = "FetchEmployeeNoOfOTFiled";
            public const string spFetchEmployeeOTConsumed = "spFetchEmployeeOTConsumed";
            public const string spLastUpdatedBy = "spLastUpdatedBy";

            //Address Update
            public const string spFetchEmployeeInfo = "spFetchEmployeeInfo";
            public const string spUpdateEmployeeInfo = "spUpdateEmployeeInfo";


            #region SelectDateToday
            public const string SelectDateToday = "SelectDateToday";
            #endregion

            //Employee Beneficiary Entry
            public const string spEmployeeBeneficiaryEntryFetchALL = "spEmployeeBeneficiaryEntryFetchALL";
            public const string spEmployeeBeneficiaryEntryFetch = "spEmployeeBeneficiaryEntryFetch";
            public const string spEmployeeBeneficiaryEntryAdd = "spEmployeeBeneficiaryEntryAdd";
            public const string spEmployeeBeneficiaryEntryUpdate = "spEmployeeBeneficiaryEntryUpdate";
            public const string spEmployeeBeneficiaryEntryDelete = "spEmployeeBeneficiaryEntryDelete";
            public const string spEmployeeBeneficiaryEntryGetEmployeeName = "spEmployeeBeneficiaryEntryGetEmployeeName";
            public const string spEmployeeBeneficiaryEntryRevertUpdate = "spEmployeeBeneficiaryEntryRevertUpdate";
            public const string spEmployeeBeneficiaryEntryCheck = "spEmployeeBeneficiaryEntryCheck";
            public const string spEmployeeBeneficiaryEntryCheckIfExistInElm = "spEmployeeBeneficiaryEntryCheckIfExistInElm";
            public const string spEmployeeBeneficiaryEntryCheckNames = "spEmployeeBeneficiaryEntryCheckNames";

            //Employee Leave Grant
            public const string spEmployeeLeaveGrantAdd = "spEmployeeLeaveGrantAdd";
            public const string spEmployeeLeaveGrantUpdate = "spEmployeeLeaveGrantUpdate";
            public const string spEmployeeLeaveGrantUpdateBL = "spEmployeeLeaveGrantUpdateBL";
            public const string spEmployeeLeaveGrantCheck = "spEmployeeLeaveGrantCheck";
            public const string spEmployeeLeaveGrantCheckDuplicate = "spEmployeeLeaveGrantCheckDuplicate";

            //Individual Leave Entry 
            public const string spIndividualLeaveEntryGetAllRecords = "spIndividualLeaveEntryGetAllRecords";
            public const string spIndividualLeaveEntryGetLeaveBalances = "spIndividualLeaveEntryGetLeaveBalances";
            public const string spIndividualLeaveEntryCheckDuplicateEntry = "spIndividualLeaveEntryCheckDuplicateEntry";
            public const string spIndividualLeaveEntryCheckLeaveBalances = "spIndividualLeaveEntryCheckLeaveBalances";
            public const string spIndividualLeaveEntryGetPayrollTypeAndBDate = "spIndividualLeaveEntryGetPayrollTypeAndBDate";
            public const string spIndividualLeaveEntryCheckIfPaidWithCreditCombined = "spIndividualLeaveEntryCheckIfPaidWithCreditCombined";
            public const string spIndividualLeaveEntryGetShiftCodeBreakOutInDayCode = "spIndividualLeaveEntryGetShiftCodeBreakOutInDayCode";
            public const string spIndividualLeaveEntryCheckIfHoliday = "spIndividualLeaveEntryCheckIfHoliday";
            public const string spIndividualLeaveEntryGetMaxHour = "spIndividualLeaveEntryGetMaxHour";
            public const string spIndividualLeaveEntryCheckCut0ff = "spIndividualLeaveEntryCheckCut0ff";
            public const string spIndividualLeaveEntryGetMinLvHrAndLvFraction = "spIndividualLeaveEntryGetMinLvHrAndLvFraction";
            public const string spIndividualLeaveEntryInsertLeaveEntry = "spIndividualLeaveEntryInsertLeaveEntry";
            public const string spIndividualLeaveEntryUpdateLeaveEntry = "spIndividualLeaveEntryUpdateLeaveEntry";
            public const string spIndividualLeaveEntryDeleteLeaveEntry = "spIndividualLeaveEntryDeleteLeaveEntry";
            public const string spIndividualLeaveEntryGetLeaveTypeDesc = "spIndividualLeaveEntryGetLeaveTypeDesc";
            public const string spIndividualLeaveEntryUpdateLeaveBalance = "spIndividualLeaveEntryUpdateLeaveBalance";
            public const string spIndividualLeaveGetCurrentPayPeriodCycle = "spIndividualLeaveGetCurrentPayPeriodCycle";
            public const string spIndividualLeaveEntryGetMaxHourFuture = "spIndividualLeaveEntryGetMaxHourFuture";
            public const string spIndividualLeaveEntryGetShiftCodeBreakOutInFuture = "spIndividualLeaveEntryGetShiftCodeBreakOutInFuture";
            public const string spIndividualLeaveEntryPostToLogLedger = "spIndividualLeaveEntryPostToLogLedger";
            public const string spIndividualLeaveEntryPostToLogLedgerWithNoPay = "spIndividualLeaveEntryPostToLogLedgerWithNoPay";
            public const string spIndividualLeaveEntryInsertLeaveEntryHist = "spIndividualLeaveEntryInsertLeaveEntryHist";

            //Log Ledger Updating
            public const string spLogLedgerFetchCurrentCycle = "spLogLedgerFetchCurrentCycle";
            public const string spLogLedgerFetchCurrentEmpLogRecord = "spLogLedgerFetchCurrentEmpLogRecord";
            public const string spLogLedgerGetShiftData = "spLogLedgerGetShiftData";
            public const string spLogLedgerGetLogTrail = "spLogLedgerGetLogTrail";
            public const string spLogLedgerGetLastUpdateByAndDate = "spLogLedgerGetLastUpdateByAndDate";
            public const string spLogLedgerCheckIfCutOffTrue = "spLogLedgerCheckIfCutOffTrue";
            public const string spLogLedgerUpdate = "spLogLedgerUpdate";
            public const string spLogLedgerCheckIfEmployeeIDExist = "spLogLedgerCheckIfEmployeeIDExist";
            public const string spLogLedgerCheckIfPayPeriodExists = "spLogLedgerCheckIfPayPeriodExists";
            public const string spLogLedgerInsertIntoLogTrail = "spLogLedgerInsertIntoLogTrail";
            public const string spLogLedgerGetLastSequenceNo = "spLogLedgerGetLastSequenceNo";
            public const string spLogLedgerCheckIfShiftCodeExists = "spLogLedgerCheckIfShiftCodeExists";
            public const string spLogLedgerFetchForView = "spLogLedgerFetchForView";

            //Individual Deduction Entry
            public const string spIndividualDeductionEntryAdd = "spIndividualDeductionEntryAdd";
            public const string spIndividualDeductionEntryUpdate = "spIndividualDeductionEntryUpdate";
            public const string spIndividualDeductionEntryTrailAdd = "spIndividualDeductionEntryTrailAdd";
            public const string spIndividualDeductionEntryDelete = "spIndividualDeductionEntryDelete";
            public const string spIndividualDeductionEntryFetchRecord = "spIndividualDeductionEntryFetchRecord";
            public const string spIndividualDeductionEntryFetchCompleteName = "spIndividualDeductionEntryFetchCompleteName";
            public const string spIndividualDeductionEntryGetLastSeqNoInTrail = "spIndividualDeductionEntryGetLastSeqNoInTrail";
            public const string spIndividualDeductionEntryCheckifCutOff = "spIndividualDeductionEntryCheckifCutOff";
            public const string spIndividualDeductionEntryGetStartandEndCycle = "spIndividualDeductionEntryGetStartandEndCycle";
            public const string spIndividualDeductionEntryGetDeductionCodeMasterValues = "spIndividualDeductionEntryGetDeductionCodeMasterValues";
            public const string spIndividualDeductionEntryGetAmortizationAmount = "spIndividualDeductionEntryGetAmortizationAmount";
            public const string spIndividualDeductionEntryGetLastUpdatedInfo = "spIndividualDeductionEntryGetLastUpdatedInfo";
            public const string spIndividualDeductionEntryGetDeductDesc = "spIndividualDeductionEntryGetDeductDesc";

            public const string spIndividualDeductionEntryCheckIfRecordExists = "spIndividualDeductionEntryCheckIfRecordExists";
            public const string spEmployeeDeductionExemptionFetchAll = "spEmployeeDeductionExemptionFetchAll";
            public const string spEmployeeDeductionExemptionInsertRecord = "spEmployeeDeductionExemptionInsertRecord";

            public const string spIndividualDeductionEntryFetchforViewPayments = "spIndividualDeductionEntryFetchforViewPayments";
            public const string spIndividualDeductionEntryGetSeqNo = "spIndividualDeductionEntryGetSeqNo";
            public const string spIndividualDeductionEntryCheckIfDeferredAmntSynchs = "spIndividualDeductionEntryCheckIfDeferredAmntSynchs";
            public const string spIndividualDeductionEntryGetUserandLudatetime = "spIndividualDeductionEntryGetUserandLudatetime";
            public const string spIndividualDeductionEntryFetchAllDeductionTrailValues = "spIndividualDeductionEntryFetchAllDeductionTrailValues";
            public const string spIndividualDeductionEntryGetAmortizationAmnt = "spIndividualDeductionEntryGetAmortizationAmnt";
            public const string spIndividualDeductionEntryGetDedDeferredData = "spIndividualDeductionEntryGetDedDeferredData";
            public const string spIndividualDeductionEntryGetCurrentPayPeriod = "spIndividualDeductionEntryGetCurrentPayPeriod";
            public const string spIndividualDeductionEntryUpdatePaidandDeferred = "spIndividualDeductionEntryUpdatePaidandDeferred";
            public const string spIndividualDeductionEntryUpdatePaidAmount = "spIndividualDeductionEntryUpdatePaidAmount";
            public const string spIndividualDeductionEntryUpdateSplitedRecord = "spIndividualDeductionEntryUpdateSplitedRecord";
            public const string spIndividualDeductionEntryUpdateInsRecordsUpdate = "spIndividualDeductionEntryUpdateInsRecordsUpdate";
            public const string spIndividualDeductionEntryInsDelRecordsDelete = "spIndividualDeductionEntryInsDelRecordsDelete";
            public const string spIndividualDeductionEntryInsertDetailHist = "spIndividualDeductionEntryInsertDetailHist";
            public const string spIndividualDeductionEntryUpdateInsRecordsInsert = "spIndividualDeductionEntryUpdateInsRecordsInsert";

            //Individual Deduction Entry end

            //Security Numbers Updating
            public const string spSecurityNumbersUpdatingAdd = "spSecurityNumbersUpdatingAdd";
            public const string spSecurityNumbersUpdatingCheckDuplicate = "spSecurityNumbersUpdatingCheckDuplicate";
            public const string spSecurityNumbersUpdatingUpdate = "spSecurityNumbersUpdatingUpdate";
            public const string spSecurityNumbersUpdatingFetchData = "spSecurityNumbersUpdatingFetchData";
            public const string spSecurityNumbersUpdatingCheckIfExist = "spSecurityNumbersUpdatingCheckIfExist";
            public const string spSecurityNumbersUpdatingFetchALL = "spSecurityNumbersUpdatingFetchALL";
            public const string spSecurityNumbersUpdatingGetEmployeeName = "spSecurityNumbersUpdatingGetEmployeeName";
            public const string spSecurityNumbersUpdatingGetLatestSeq = "spSecurityNumbersUpdatingGetLatestSeq";

            //Log Reading
            public const string spLogReadingGetEmployeeInformation = "spLogReadingGetEmployeeInformation";
            public const string spLogReadingCheckIfLogExists = "spLogReadingCheckIfLogExists";
            public const string spLogReadingInsertToDTR = "spLogReadingInsertToDTR";
            public const string spLogReadingUpdateLogMasterLastLog = "spLogReadingUpdateLogMasterLastLog";
            public const string spLogReadingGetStationNo = "spLogReadingGetStationNo";
            public const string spLogReadingForEmpImageGetLocationAndExtension = "spLogReadingForEmpImageGetLocationAndExtension";

            //EmployeeMaster added SP
            public const string spEmployeeMasterGetCostCenterDesc = "spEmployeeMasterGetCostCenterDesc";
            public const string spEmployeeMasterGetDataForID = "spEmployeeMasterGetDataForID";
            public const string spEmployeeMasterCreateEmpLeaveRec = "spEmployeeMasterCreateEmpLeaveRec";
            public const string spEmployeeMasterCreateEmpLogMasterRecord = "spEmployeeMasterCreateEmpLogMasterRecord";
            public const string spEmployeeMasterGetPositionDesc = "spEmployeeMasterGetPositionDesc";
            public const string spEmployeeMasterCountAuditTrail = "spEmployeeMasterCountAuditTrail";
            public const string spEmployeeMasterCreateAuditTrailRecord = "spEmployeeMasterCreateAuditTrailRecord";
            public const string spEmployeeMasterCreateEmployeeLogLedgerRecord = "spEmployeeMasterCreateEmployeeLogLedgerRecord";
            public const string spEmployeeMasterGetShiftCodeValues = "spEmployeeMasterGetShiftCodeValues";
            public const string spEmployeeMasterCheckifHoliday = "spEmployeeMasterCheckifHoliday";
            public const string spEmployeeMasterUpdateEmployeeLogLedgerRecord = "spEmployeeMasterUpdateEmployeeLogLedgerRecord";

            //ID Printing
            public const string spIDPrintingGetEmployeeInformation = "spIDPrintingGetEmployeeInformation";
            public const string spIDPrintingCheckIfIDExists = "spIDPrintingCheckIfIDExists";
            public const string spIDPrintingUpdateLogMaster = "spIDPrintingUpdateLogMaster";

            //Training Master Entry
            public const string spTrainingMasterEntryFetch = "spTrainingMasterEntryFetch";
            public const string spTrainingMasterEntryFetchName = "spTrainingMasterEntryFetchName";
            public const string spTrainingMasterEntryTrainingModuleLP = "spTrainingMasterEntryTrainingModuleLP";
            public const string spTrainingMasterEntryResourcePersonInternalLP = "spTrainingMasterEntryResourcePersonInternalLP";
            public const string spTrainingMasterEntryResourcePersonExternalLP = "spTrainingMasterEntryResourcePersonExternalLP";
            public const string spTrainingMasterEntryUpdate = "spTrainingMasterEntryUpdate";
            public const string spTrainingMasterEntryFetch_TC_TPID = "spTrainingMasterEntryFetch_TC_TPID";
            public const string spTrainingMasterEntryAdd = "spTrainingMasterEntryAdd";
            public const string spTrainingMasterEntryGenerateSeqNo = "spTrainingMasterEntryGenerateSeqNo";
            public const string spTrainingMasterEntryDelete = "spTrainingMasterEntryDelete";
            public const string spTrainingMasterEntryGetTrainingModuleCode = "spTrainingMasterEntryGetTrainingModuleCode";
            public const string spTrainingMasterEntryGetResourcePersonCodeExternal = "spTrainingMasterEntryGetResourcePersonCodeExternal";
            public const string spTrainingMasterEntryGetResourcePersonCodeInternal = "spTrainingMasterEntryGetResourcePersonCodeInternal";

            public const string spTrainingMasterEntryCheckUserLoginOnDelete = "spTrainingMasterEntryCheckUserLoginOnDelete";
            public const string spTrainingMasterEntryCheckUserLoginOnAdd = "spTrainingMasterEntryCheckUserLoginOnAdd";
            public const string spTrainingMasterEntryCheckUserLoginOnEdit = "spTrainingMasterEntryCheckUserLoginOnEdit";
            public const string spTrainingMasterEntryCheckUserLoginOnPrint = "spTrainingMasterEntryCheckUserLoginOnPrint";
            public const string spTrainingMasterEntryCheckUserLoginOnView = "spTrainingMasterEntryCheckUserLoginOnView";
            public const string spTrainingMasterEntryCheckUserLogin = "spTrainingMasterEntryCheckUserLogin";

            //Log Uploading
            public const string spLogUploadingCheckIfCycleOpen = "spLogUploadingCheckIfCycleOpen";
            public const string spLogUploadingGetAllRecInLogControl = "spLogUploadingGetAllRecInLogControl";
            public const string spLogUploadingRetrievePunches = "spLogUploadingRetrievePunches";
            public const string spLogUploadingRetrieveEmployeeLogLedgerRec = "spLogUploadingRetrieveEmployeeLogLedgerRec";
            public const string spLogUploadingUpdateShiftCode = "spLogUploadingUpdateShiftCode";
            public const string spLogUploadingGetNewShiftCode = "spLogUploadingGetNewShiftCode";

            //Payroll Calculation
            //Legend:
            //          GT = Greater Than
            //          LT = Lesser Than
            public const string spPayrollCalculationGetPayPeriodAndEndCycle = "spPayrollCalculationGetPayPeriodAndEndCycle";
            public const string spPayrollCalculationGetNumericValue = "spPayrollCalculationGetNumericValue";
            public const string spPayrollCalculationGetCompanyTaxSchedule = "spPayrollCalculationGetCompanyTaxSchedule";
            public const string spPayrollCalculationGetProcessFlag = "spPayrollCalculationGetProcessFlag";
            public const string spPayrollCalculationGetPrem = "spPayrollCalculationGetPrem";
            ////Deduction Pre-Checking
            public const string spPayrollCalculationGetEmpWithPaidAmountGTDeductionAmount = "spPayrollCalculationGetEmpWithPaidAmountGTDeductionAmount";
            public const string spPayrollCalculationGetEmpWithDeductionAmountLTPaidPlusDeferredAmount = "spPayrollCalculationGetEmpWithDeductionAmountLTPaidPlusDeferredAmount";
            public const string spPayrollCalculationGetEmpWithAmortizationAmountGTDeductionAmount = "spPayrollCalculationGetEmpWithAmortizationAmountGTDeductionAmount";
            public const string spPayrollCalculationGetEmpWithFullyPaidButWithDeferredAmount = "spPayrollCalculationGetEmpWithFullyPaidButWithDeferredAmount";
            public const string spPayrollCalculationGetEmpWithDeferredAmountNegative = "spPayrollCalculationGetEmpWithDeferredAmountNegative";
            public const string spPayrollCalculationGetEmpWithPaidAmountNegative = "spPayrollCalculationGetEmpWithPaidAmountNegative";
            public const string spPayrollCalculationDeferredAmountChecking = "spPayrollCalculationDeferredAmountChecking";
            ////Employee Pre-Checking
            public const string spPayrollCalculationGetEmpWithNoSalaryRateRegistered = "spPayrollCalculationGetEmpWithNoSalaryRateRegistered";
            public const string spPayrollCalculationGetEmpWithDailySalRateGTMaxRateForDailyPaid = "spPayrollCalculationGetEmpWithDailySalRateGTMaxRateForDailyPaid";
            public const string spPayrollCalculationGetEmpWithMonthlySalRateLTMaxRateForDailyPaid = "spPayrollCalculationGetEmpWithMonthlySalRateLTMaxRateForDailyPaid";
            public const string spPayrollCalculationGetEmpWithNoTaxCodeRegistered = "spPayrollCalculationGetEmpWithNoTaxCodeRegistered";
            public const string spPayrollCalculationGetEmpWithSpecialHDMFContributionIsZero = "spPayrollCalculationGetEmpWithSpecialHDMFContributionIsZero";
            ////Process Flag Checking Regular Employee
            public const string spPayrollCalculationCheckIfPayrollErrorHasData = "spPayrollCalculationCheckIfPayrollErrorHasData";
            public const string spPayrollCalculationDeletePayrollErrorData = "spPayrollCalculationDeletePayrollErrorData";
            public const string spPayrollCalculationCheckIfEmployeePayrollCalcHasData = "spPayrollCalculationCheckIfEmployeePayrollCalcHasData";
            public const string spPayrollCalculationDeleteEmployeePayrollCalcData = "spPayrollCalculationDeleteEmployeePayrollCalcData";
            public const string spPayrollCalculationCheckIfEmployeeDeductionDetailHasData = "spPayrollCalculationCheckIfEmployeeDeductionDetailHasData";
            public const string spPayrollCalculationDeleteEmployeeDeductionDetailData = "spPayrollCalculationDeleteEmployeeDeductionDetailData";
            ////Process Flag Checking Confidential Employee
            public const string spPayrollCalculationCheckIfPayrollErrorConfidentialHasData = "spPayrollCalculationCheckIfPayrollErrorConfidentialHasData";
            public const string spPayrollCalculationDeletePayrollErrorConfidentialData = "spPayrollCalculationDeletePayrollErrorConfidentialData";
            public const string spPayrollCalculationCheckIfEmployeePayrollCalcConfidentialHasData = "spPayrollCalculationCheckIfEmployeePayrollCalcConfidentialHasData";
            public const string spPayrollCalculationDeleteEmployeePayrollCalcConfidentialData = "spPayrollCalculationDeleteEmployeePayrollCalcConfidentialData";
            public const string spPayrollCalculationCheckIfEmployeeDeductionDetailConfidentialHasData = "spPayrollCalculationCheckIfEmployeeDeductionDetailConfidentialHasData";
            public const string spPayrollCalculationDeleteEmployeeDeductionDetailConfidentialData = "spPayrollCalculationDeleteEmployeeDeductionDetailConfidentialData";
            public const string spPayrollCalculationCreateCurrentDeduction = "spPayrollCalculationCreateCurrentDeduction";
            public const string spPayrollCalculationCreateCurrentDeductionConfi = "spPayrollCalculationCreateCurrentDeductionConfi";
            public const string spPayrollCalculationCheckIfEmpPayrollTransactionHasData = "spPayrollCalculationCheckIfEmpPayrollTransactionHasData";
            public const string spPayrollCalculationCheckIfEmpPayrollTransactionHasDataConfi = "spPayrollCalculationCheckIfEmpPayrollTransactionHasDataConfi";
            public const string spPayrollCalculationCreateEmpPayrollRecords = "spPayrollCalculationCreateEmpPayrollRecords";
            public const string spPayrollCalculationCreateEmpPayrollRecordsConfi = "spPayrollCalculationCreateEmpPayrollRecordsConfi";
            public const string spPayrollCalculationComputeRegAbsOTAndGrossPay = "spPayrollCalculationComputeRegAbsOTAndGrossPay";
            public const string spPayrollCalculationComputeRegAbsOTAndGrossPayConfi = "spPayrollCalculationComputeRegAbsOTAndGrossPayConfi";
            public const string spPayrollCalculationCreateErroneousPayrollRecords = "spPayrollCalculationCreateErroneousPayrollRecords";
            public const string spPayrollCalculationCreateErroneousPayrollRecordsConfi = "spPayrollCalculationCreateErroneousPayrollRecordsConfi";
            public const string spPayrollCalculationUpdatePayrollPostAndJobStatus = "spPayrollCalculationUpdatePayrollPostAndJobStatus";
            public const string spPayrollCalculationUpdatePayrollPostAndJobStatusConfi = "spPayrollCalculationUpdatePayrollPostAndJobStatusConfi";
            public const string spPayrollCalculationDeleteEmpCurrPayWithNegAmount = "spPayrollCalculationDeleteEmpCurrPayWithNegAmount";
            public const string spPayrollCalculationDeleteEmpCurrPayWithNegAmountConfi = "spPayrollCalculationDeleteEmpCurrPayWithNegAmountConfi";
            public const string spPayrollCalculationComputePremiumShare = "spPayrollCalculationComputePremiumShare";
            public const string spPayrollCalculationComputePremiumShareConfi = "spPayrollCalculationComputePremiumShareConfi";
            public const string spPayrollCalculationGetEmpNames = "spPayrollCalculationGetEmpNames";
            public const string spPayrollCalculationGetEmpNamesConfi = "spPayrollCalculationGetEmpNamesConfi";
            public const string spPayrollCalculationComputeYearToDateTotals = "spPayrollCalculationComputeYearToDateTotals";
            public const string spPayrollCalculationComputeYearToDateTotalsConfi = "spPayrollCalculationComputeYearToDateTotalsConfi";
            public const string spPayrollCalculationComputeTax = "spPayrollCalculationComputeTax";
            public const string spPayrollCalculationComputeTaxConfi = "spPayrollCalculationComputeTaxConfi";
            public const string spPayrollCalculationCountEmpInDeductionDetail = "spPayrollCalculationCountEmpInDeductionDetail";
            public const string spPayrollCalculationCountEmpInDeductionDetailConfi = "spPayrollCalculationCountEmpInDeductionDetailConfi";
            public const string spPayrollCalculationGetEmpDeductionDetail = "spPayrollCalculationGetEmpDeductionDetail";
            public const string spPayrollCalculationGetEmpDeductionDetailConfi = "spPayrollCalculationGetEmpDeductionDetailConfi";
            public const string spPayrollCalculationUpdateDeductionDetailPaymentFlag = "spPayrollCalculationUpdateDeductionDetailPaymentFlag";
            public const string spPayrollCalculationUpdateDeductionDetailPaymentFlagConfi = "spPayrollCalculationUpdateDeductionDetailPaymentFlagConfi";
            public const string spPayrollCalculationUpdatePayCalcOtherAmt = "spPayrollCalculationUpdatePayCalcOtherAmt";
            public const string spPayrollCalculationUpdatePayCalcOtherAmtConfi = "spPayrollCalculationUpdatePayCalcOtherAmtConfi";
            public const string spPayrollCalculationUpdatePayCalcTotalDedctAmt = "spPayrollCalculationUpdatePayCalcTotalDedctAmt";
            public const string spPayrollCalculationUpdatePayCalcTotalDedctAmtConfi = "spPayrollCalculationUpdatePayCalcTotalDedctAmtConfi";
            public const string spPayrollCalculationUpdatePayCalcNetPayAmt = "spPayrollCalculationUpdatePayCalcNetPayAmt";
            public const string spPayrollCalculationUpdatePayCalcNetPayAmtConfi = "spPayrollCalculationUpdatePayCalcNetPayAmtConfi";
            public const string spPayrollCalculationCreateErrPayRecAfterComp = "spPayrollCalculationCreateErrPayRecAfterComp";
            public const string spPayrollCalculationCreateErrPayRecAfterCompConfi = "spPayrollCalculationCreateErrPayRecAfterCompConfi";
            public const string spPayrollCalculationUpdateProcessFlag = "spPayrollCalculationUpdateProcessFlag";
            public const string spPayrollCalculationGetEmpPayrollRec = "spPayrollCalculationGetEmpPayrollRec";
            public const string spPayrollCalculationComputeHDMFPremiumShare = "spPayrollCalculationComputeHDMFPremiumShare";
            public const string spPayrollCalculationComputeSSSPremiumShareWhenSSSCodeIsG = "spPayrollCalculationComputeSSSPremiumShareWhenSSSCodeIsG";
            public const string spPayrollCalculationComputeSSSPremiumShareWhenSSSCodeIsR = "spPayrollCalculationComputeSSSPremiumShareWhenSSSCodeIsR";
            public const string spPayrollCalculationComputeSSSPremiumShareWhenSSSCodeIsF = "spPayrollCalculationComputeSSSPremiumShareWhenSSSCodeIsF";
            public const string spPayrollCalculationComputeSSSPremiumShareWhenPhilHealthCodeIsG = "spPayrollCalculationComputeSSSPremiumShareWhenPhilHealthCodeIsG";
            public const string spPayrollCalculationComputeSSSPremiumShareWhenPhilHealthCodeIsR = "spPayrollCalculationComputeSSSPremiumShareWhenPhilHealthCodeIsR";
            public const string spPayrollCalculationComputeSSSPremiumShareWhenPhilHealthCodeIsF = "spPayrollCalculationComputeSSSPremiumShareWhenPhilHealthCodeIsF";


            //Labor Hours Generation
            public const string spLaborHoursGenerationCheckIfThereAreRecordstoProcess = "spLaborHoursGenerationCheckIfThereAreRecordstoProcess";
            public const string spLaborHoursGenerationCheckRestDay = "spLaborHoursGenerationCheckRestDay";
            public const string spLaborHoursGenerationUpdateRestDay = "spLaborHoursGenerationUpdateRestDay";
            public const string spLaborHoursGenerationCheckHolidayDay = "spLaborHoursGenerationCheckHolidayDay";
            public const string spLaborHoursGenerationUpdateHolidayDay = "spLaborHoursGenerationUpdateHolidayDay";
            public const string spLaborHoursGenerationCheckEncodedOT = "spLaborHoursGenerationCheckEncodedOT";
            public const string spLaborHoursGenerationUpdateEncodedOT = "spLaborHoursGenerationUpdateEncodedOT";
            public const string spLaborHoursGenerationCheckEncodedPayLeave = "spLaborHoursGenerationCheckEncodedPayLeave";
            public const string spLaborHoursGenerationUpdateEncodedPayLeave = "spLaborHoursGenerationUpdateEncodedPayLeave";
            public const string spLaborHoursGenerationCheckEncodedNoPayLeave = "spLaborHoursGenerationCheckEncodedNoPayLeave";
            public const string spLaborHoursGenerationUpdateEncodedNoPayLeave = "spLaborHoursGenerationUpdateEncodedNoPayLeave";
            public const string spLaborHoursGenerationCheckInvalidDayCode = "spLaborHoursGenerationCheckInvalidDayCode";
            public const string spLaborHoursGenerationCheckInvalidShiftCode = "spLaborHoursGenerationCheckInvalidShiftCode";
            public const string spLaborHoursGenerationDeleteEmpPayTrans = "spLaborHoursGenerationDeleteEmpPayTrans";
            public const string spLaborHoursGenerationGetSOffData = "spLaborHoursGenerationGetSOffData";
            public const string spLaborHoursGenerationUpdateSOffInEmpLogLedger = "spLaborHoursGenerationUpdateSOffInEmpLogLedger";
            public const string spLaborHoursGenerationCheckForOffsetMin = "spLaborHoursGenerationCheckForOffsetMin";
            public const string spLaborHoursGenerationUpdateForOffsetMin = "spLaborHoursGenerationUpdateForOffsetMin";
            public const string spLaborHoursGenerationCreatePayrollTransRec = "spLaborHoursGenerationCreatePayrollTransRec";
            public const string spLaborHoursGenerationUpdateRegHoursMonthDaily = "spLaborHoursGenerationUpdateRegHoursMonthDaily";
            public const string spLaborHoursGenerationDeleteLaborHrErr = "spLaborHoursGenerationDeleteLaborHrErr";
            public const string spLaborHoursGenerationCreateLaborHrErr = "spLaborHoursGenerationCreateLaborHrErr";
            public const string spLaborHoursGenerationCreatePayrollTransRecConfidential = "spLaborHoursGenerationCreatePayrollTransRecConfidential";
            public const string spLaborHoursGenerationDeleteEmpPayTransConfidential = "spLaborHoursGenerationDeleteEmpPayTransConfidential";

            //Bank Debit Advice 
            public const string spBankDebitAdviceGetBankDetails = "spBankDebitAdviceGetBankDetails";
            public const string spBankDebitAdviceGetSum = "spBankDebitAdviceGetSum";
            public const string spBankDebitAdviceGetPayPeriod = "spBankDebitAdviceGetPayPeriod";

            //Allowance Entry Viewing
            public const string spAllowanceEntryViewingGetRecords = "spAllowanceEntryViewingGetRecords";
            public const string spAllowanceEntryViewingGetNonTaxAllowance = "spAllowanceEntryViewingGetNonTaxAllowance";
            public const string spAllowanceEntryViewingGetTaxAllowance = "spAllowanceEntryViewingGetTaxAllowance";

            //Payroll Deduction Viewing
            public const string spPayrollDeductionViewingGetRecords = "spPayrollDeductionViewingGetRecords";

            //Leave Viewing
            public const string spLeaveViewingGetRecords = "spLeaveViewingGetRecords";

            //Previous Cycle Log Uploading
            public const string spPrevCycleLogUploadingFetch = "spPrevCycleLogUploadingFetch";
            public const string spPrevCycleLogUploadingUpdateLogLedgerHist = "spPrevCycleLogUploadingUpdateLogLedgerHist";
            public const string spPrevCycleLogUploadingUpdateLogLedgerTrail = "spPrevCycleLogUploadingUpdateLogLedgerTrail";
            public const string spPrevCycleLogUploadingInsertLogLedgerTrail = "spPrevCycleLogUploadingInsertLogLedgerTrail";
            public const string spPrevCycleLogUploadingFetchForView = "spPrevCycleLogUploadingFetchForView";

            //Labor Hours Adjustment
            public const string spLaborHoursAdjustmentDeleteRecord = "spLaborHoursAdjustmentDeleteRecord";
            public const string spLaborHoursAdjustmentInsertRecord = "spLaborHoursAdjustmentInsertRecord";
            public const string spLaborHoursAdjustmentUpdateRecord = "spLaborHoursAdjustmentUpdateRecord";
            public const string spLaborHoursAdjustmentUpdateSatOff = "spLaborHoursAdjustmentUpdateSatOff";
            public const string spLaborHoursAdjustmentCleanUpBeforeSum = "spLaborHoursAdjustmentCleanUpBeforeSum";
            public const string spLaborHoursAdjustmentCleanUpAfterSum = "spLaborHoursAdjustmentCleanUpAfterSum";
            public const string spLaborHoursAdjustmentGetIdToProcess = "spLaborHoursAdjustmentGetIdToProcess";

            //Backup and Restore Data
            public const string spBackupAndRestoreDeleteAllowance = "spBackupAndRestoreDeleteAllowance";
            public const string spBackupAndRestoreDeleteAllowanceBackup = "spBackupAndRestoreDeleteAllowanceBackup";
            public const string spBackupAndRestoreInsertAllowance = "spBackupAndRestoreInsertAllowance";
            public const string spBackupAndRestoreInsertAllowanceBackup = "spBackupAndRestoreInsertAllowanceBackup";
            public const string spBackupAndRestoreFetchAllowance = "spBackupAndRestoreFetchAllowance";
            public const string spBackupAndRestoreFetchAllowanceBackup = "spBackupAndRestoreFetchAllowanceBackup";
            public const string spBackupAndRestoreDeleteAdjustment = "spBackupAndRestoreDeleteAdjustment";
            public const string spBackupAndRestoreDeleteAdjustmentBack = "spBackupAndRestoreDeleteAdjustmentBack";
            public const string spBackupAndRestoreInsertAdjustment = "spBackupAndRestoreInsertAdjustment";
            public const string spBackupAndRestoreInsertAsjustmentBackup = "spBackupAndRestoreInsertAsjustmentBackup";
            public const string spBackupAndRestoreFetchAdjustment = "spBackupAndRestoreFetchAdjustment";
            public const string spBackupAndRestoreFetchAdjustmentBackup = "spBackupAndRestoreFetchAdjustmentBackup";

            //Cycle Opening
            public const string spCycleOpeningCountEmpInEmpAllowance = "spCycleOpeningCountEmpInEmpAllowance";
            public const string spCycleOpeningCountEmpInEmpAllowanceToBeDel = "spCycleOpeningCountEmpInEmpAllowanceToBeDel";
            public const string spCycleOpeningDeleteEmpInEmpAllowance = "spCycleOpeningDeleteEmpInEmpAllowance";
            public const string spCycleOpeningCountEmpInEmpAllowanceForRecAllowances = "spCycleOpeningCountEmpInEmpAllowanceForRecAllowances";
            public const string spCycleOpeningUpdateEmpInEmpAllowanceSetPayPostToZero = "spCycleOpeningUpdateEmpInEmpAllowanceSetPayPostToZero";
            public const string spCycleOpeningUpdateEmpInEmpAllowanceSetCurPayToNextPay = "spCycleOpeningUpdateEmpInEmpAllowanceSetCurPayToNextPay";
            public const string spCycleOpeningCountEmpInEmpAdjustments = "spCycleOpeningCountEmpInEmpAdjustments";
            public const string spCycleOpeningCountEmpInEmpAdjustmentsToBeDel = "spCycleOpeningCountEmpInEmpAdjustmentsToBeDel";
            public const string spCycleOpeningDeleteEmpInEmpAdjustments = "spCycleOpeningDeleteEmpInEmpAdjustments";
            public const string spCycleOpeningUpdateEmpInEmpAdjustmentSetCurPayToNextPay = "spCycleOpeningUpdateEmpInEmpAdjustmentSetCurPayToNextPay";
            public const string spCycleOpeningCountEmpSumAmtInEmpDeductionDeffered = "spCycleOpeningCountEmpSumAmtInEmpDeductionDeffered";
            public const string spCycleOpeningCountEmpSumAmtInEmpDeductionDefferedToBeDel = "spCycleOpeningCountEmpSumAmtInEmpDeductionDefferedToBeDel";
            public const string spCycleOpeningDeleteEmpInEmpDeductionDeffered = "spCycleOpeningDeleteEmpInEmpDeductionDeffered";
            public const string spCycleOpeningCountEmpSumAmtInEmpDeductionDetail = "spCycleOpeningCountEmpSumAmtInEmpDeductionDetail";
            public const string spCycleOpeningDeleteEmpInEmpDeductionDetail = "spCycleOpeningDeleteEmpInEmpDeductionDetail";
            public const string spCycleOpeningCountEmpSumAmtInEmpDeductionDetailConfi = "spCycleOpeningCountEmpSumAmtInEmpDeductionDetailConfi";
            public const string spCycleOpeningDeleteEmpInEmpDeductionDetailConfi = "spCycleOpeningDeleteEmpInEmpDeductionDetailConfi";
            public const string spCycleOpeningCountEmpSumAmtInEmpPayrollCalc = "spCycleOpeningCountEmpSumAmtInEmpPayrollCalc";
            public const string spCycleOpeningDeleteEmpInEmpPayrollCalc = "spCycleOpeningDeleteEmpInEmpPayrollCalc";
            public const string spCycleOpeningCountEmpInEmpPayrollCalcConfi = "spCycleOpeningCountEmpInEmpPayrollCalcConfi";
            public const string spCycleOpeningDeleteEmpInEmpPayrollCalcConfi = "spCycleOpeningDeleteEmpInEmpPayrollCalcConfi";
            public const string spCycleOpeningCountEmpInEmpPayrollErr = "spCycleOpeningCountEmpInEmpPayrollErr";
            public const string spCycleOpeningDeleteEmpInEmpPayrollErr = "spCycleOpeningDeleteEmpInEmpPayrollErr";
            public const string spCycleOpeningCountEmpInEmpPayrollErrConfi = "spCycleOpeningCountEmpInEmpPayrollErrConfi";
            public const string spCycleOpeningDeleteEmpInEmpPayrollErrConfi = "spCycleOpeningDeleteEmpInEmpPayrollErrConfi";
            public const string spCycleOpeningCountEmpInEmpOvertime = "spCycleOpeningCountEmpInEmpOvertime";
            public const string spCycleOpeningDeleteEmpInEmpOvertime = "spCycleOpeningDeleteEmpInEmpOvertime";
            public const string spCycleOpeningCountEmpInEmpLogLedger = "spCycleOpeningCountEmpInEmpLogLedger";
            public const string spCycleOpeningDeleteEmpInEmpLogLedger = "spCycleOpeningDeleteEmpInEmpLogLedger";
            public const string spCycleOpeningCreateEmployeeLogLedgerRecord = "spCycleOpeningCreateEmployeeLogLedgerRecord";
            public const string spCycleOpeningCountEmpInEmpPayrollTrans = "spCycleOpeningCountEmpInEmpPayrollTrans";
            public const string spCycleOpeningDeleteEmpInEmpPayrollTrans = "spCycleOpeningDeleteEmpInEmpPayrollTrans";
            public const string spCycleOpeningCountEmpInEmpPayrollTransConfi = "spCycleOpeningCountEmpInEmpPayrollTransConfi";
            public const string spCycleOpeningDeleteEmpInEmpPayrollTransConfi = "spCycleOpeningDeleteEmpInEmpPayrollTransConfi";
            public const string spCycleOpeningUpdateLogControl = "spCycleOpeningUpdateLogControl";
            public const string spCycleOpeningCountEmpInEmpLeaveAvailment = "spCycleOpeningCountEmpInEmpLeaveAvailment";
            public const string spCycleOpeningCountEmpInEmpLeaveAvailmentWhereLFInCP = "spCycleOpeningCountEmpInEmpLeaveAvailmentWhereLFInCP";
            public const string spCycleOpeningDeleteEmpInEmpLeaveAvailment = "spCycleOpeningDeleteEmpInEmpLeaveAvailment";
            public const string spCycleOpeningGetEmpInLeaveAvailmentWhereLFIsU = "spCycleOpeningGetEmpInLeaveAvailmentWhereLFIsU";
            public const string spCycleOpeningPostToLogLedger = "spCycleOpeningPostToLogLedger";
            public const string spCycleOpeningPostToLogLedgerWithNoPay = "spCycleOpeningPostToLogLedgerWithNoPay";
            public const string spCycleOpeningUpdateLeaveBalance = "spCycleOpeningUpdateLeaveBalance";
            public const string spCycleOpeningUpdateLeaveFlag = "spCycleOpeningUpdateLeaveFlag";
            public const string spCycleOpeningGetEmpInLeaveAvailmentWhereLFIsF = "spCycleOpeningGetEmpInLeaveAvailmentWhereLFIsF";
            public const string spCycleOpeningCreateEmpRecNotInLeaveMaster = "spCycleOpeningCreateEmpRecNotInLeaveMaster";
            public const string spCycleOpeningCountEmpInLeaveMasterWhereStatIsA = "spCycleOpeningCountEmpInLeaveMasterWhereStatIsA";
            public const string spCycleOpeningUpdateBDayLeaveCredits = "spCycleOpeningUpdateBDayLeaveCredits";
            public const string spCycleOpeningUpdatePayPeriodCycleIndicator = "spCycleOpeningUpdatePayPeriodCycleIndicator";
            public const string spCycleOpeningUpdateProcessFlags = "spCycleOpeningUpdateProcessFlags";
            public const string spCycleOpeningGetNextPayPeriod = "spCycleOpeningGetNextPayPeriod";
            public const string spCycleOpeningGetCurrentPayPeriod = "spCycleOpeningGetCurrentPayPeriod";
            public const string spCycleOpeningGetShiftCodeAndEmpID = "spCycleOpeningGetShiftCodeAndEmpID";
            public const string InsertIntoLogTrail = "InsertIntoLogTrail";
            public const string spCycleOpeningCountEmpInEmpAllowanceWherePayPeriodIsCurrent = "spCycleOpeningCountEmpInEmpAllowanceWherePayPeriodIsCurrent";
            public const string spCycleOpeningPostSatOff = "spCycleOpeningPostSatOff";

            //Cycle Closing
            public const string spCycleClosingCountEmpInEmpAllowanceHist = "spCycleClosingCountEmpInEmpAllowanceHist";
            public const string spCycleClosingCountEmpInEmpAllowanceToInsert = "spCycleClosingCountEmpInEmpAllowanceToInsert";
            public const string spCycleClosingInsertInEmpInEmpAllowanceHist = "spCycleClosingInsertInEmpInEmpAllowanceHist";
            public const string spCycleClosingCountEmpInEmpAdjustmentHist = "spCycleClosingCountEmpInEmpAdjustmentHist";
            public const string spCycleClosingCountEmpInEmpAdjustmentToInsert = "spCycleClosingCountEmpInEmpAdjustmentToInsert";
            public const string spCycleClosingInsertInEmpInEmpAdjustmentHist = "spCycleClosingInsertInEmpInEmpAdjustmentHist";
            public const string spCycleClosingSumAndCountEmpInEmpDeductionDetailHist = "spCycleClosingSumAndCountEmpInEmpDeductionDetailHist";
            public const string spCycleClosingSumAndCountEmpInEmpDeductionDetailToInsert = "spCycleClosingSumAndCountEmpInEmpDeductionDetailToInsert";
            public const string spCycleClosingInsertInEmpInEmpDeductionDetailHist = "spCycleClosingInsertInEmpInEmpDeductionDetailHist";
            public const string spCycleClosingSumAndCountEmpInEmpPayrollCalcAnnual = "spCycleClosingSumAndCountEmpInEmpPayrollCalcAnnual";
            public const string spCycleClosingCountEmpInEmpPayrollCalcAnnualToInsert = "spCycleClosingCountEmpInEmpPayrollCalcAnnualToInsert";
            public const string spCycleClosingInsertInEmpInEmpPayrollCalcAnnual = "spCycleClosingInsertInEmpInEmpPayrollCalcAnnual";
            public const string spCycleClosingSumAndCountEmpInEmpDeductionLedgerHist = "spCycleClosingSumAndCountEmpInEmpDeductionLedgerHist";
            public const string spCycleClosingSumAndCountEmpInEmpDeductionLedgerHistToInsert = "spCycleClosingSumAndCountEmpInEmpDeductionLedgerHistToInsert";
            public const string spCycleClosingInsertInEmpInEmpDeductionLedgerHist = "spCycleClosingInsertInEmpInEmpDeductionLedgerHist";
            public const string spCycleClosingSumEmpInEmpDeductionLedger = "spCycleClosingSumEmpInEmpDeductionLedger";
            public const string spCycleClosingCountEmpInEmpDeductionDetailToUpdate = "spCycleClosingCountEmpInEmpDeductionDetailToUpdate";
            public const string spCycleClosingUpdateInEmpInEmpDeductionLedger = "spCycleClosingUpdateInEmpInEmpDeductionLedger";
            public const string spCycleClosingSumAndCountEmpInEmpDeductionDeffered = "spCycleClosingSumAndCountEmpInEmpDeductionDeffered";
            public const string spCycleClosingCountEmpInEmpDeductionDetailToInsert = "spCycleClosingCountEmpInEmpDeductionDetailToInsert";
            public const string spCycleClosingInsertInEmpInEmpDeductionDeffered = "spCycleClosingInsertInEmpInEmpDeductionDeffered";
            public const string spCycleClosingCountEmpInEmpLogLedgerHist = "spCycleClosingCountEmpInEmpLogLedgerHist";
            public const string spCycleClosingCountEmpInEmpLogLedgerToInsert = "spCycleClosingCountEmpInEmpLogLedgerToInsert";
            public const string spCycleClosingInsertInEmpInEmpLogLedgerHist = "spCycleClosingInsertInEmpInEmpLogLedgerHist";
            public const string spCycleClosingCountEmpInOffSet = "spCycleClosingCountEmpInOffSet";
            public const string spCycleClosingCountEmpInOffSetToUpdate = "spCycleClosingCountEmpInOffSetToUpdate";
            public const string spCycleClosingUpdateInEmpOffSet = "spCycleClosingUpdateInEmpOffSet";
            public const string spCycleClosingCountEarnedSatOff = "spCycleClosingCountEarnedSatOff";
            public const string spCycleClosingCountEarnedSatOffToInsert = "spCycleClosingCountEarnedSatOffToInsert";
            public const string spCycleClosingInsertEarnedSatOff = "spCycleClosingInsertEarnedSatOff";
            public const string spCycleClosingCountEmpInEmployeeMaster = "spCycleClosingCountEmpInEmployeeMaster";
            public const string spCycleClosingCountEmpInEmployeeMasterToUpdate = "spCycleClosingCountEmpInEmployeeMasterToUpdate";
            public const string spCycleClosingUpdateEmpInEmployeeMaster = "spCycleClosingUpdateEmpInEmployeeMaster";
            public const string spCycleClosingCountEmpInEmployeeMasterHist = "spCycleClosingCountEmpInEmployeeMasterHist";
            public const string spCycleClosingCountEmpInEmployeeMasterHistToInsert = "spCycleClosingCountEmpInEmployeeMasterHistToInsert";
            public const string spCycleClosingInsertEmpInEmployeeMasterHist = "spCycleClosingInsertEmpInEmployeeMasterHist";
            public const string spCycleClosingCountEmpInLaborHrErrorHist = "spCycleClosingCountEmpInLaborHrErrorHist";
            public const string spCycleClosingCountEmpInLaborHrErrorHistToInsert = "spCycleClosingCountEmpInLaborHrErrorHistToInsert";
            public const string spCycleClosingInsertEmpInLaborHrErrorHist = "spCycleClosingInsertEmpInLaborHrErrorHist";
            public const string spCycleClosingCountEmpInLeaveAvailHist = "spCycleClosingCountEmpInLeaveAvailHist";
            public const string spCycleClosingCountEmpInLeaveAvailHistToInsert = "spCycleClosingCountEmpInLeaveAvailHistToInsert";
            public const string spCycleClosingInsertEmpInLeaveAvailHist = "spCycleClosingInsertEmpInLeaveAvailHist";
            public const string spCycleClosingCountEmpInOvertimeHist = "spCycleClosingCountEmpInOvertimeHist";
            public const string spCycleClosingCountEmpInOvertimeHistToInsert = "spCycleClosingCountEmpInOvertimeHistToInsert";
            public const string spCycleClosingInsertEmpInOvertimeHist = "spCycleClosingInsertEmpInOvertimeHist";
            public const string spCycleClosingCountEmpInPayrollErrorHist = "spCycleClosingCountEmpInPayrollErrorHist";
            public const string spCycleClosingCountEmpInPayrollErrorToInsert = "spCycleClosingCountEmpInPayrollErrorToInsert";
            public const string spCycleClosingInsertEmpInPayrollErrorHist = "spCycleClosingInsertEmpInPayrollErrorHist";

            //Backup Database
            public const string sp_get_ip_address = "sp_get_ip_address";

        }

        #endregion//storedProcedures

        #region messages

        public class Messages
        {
            public const string msgErrInvalidVersion = "There is a new version that you skipped. Please contact your IT Team for assistance.";
            public const string msgInfoInputUserCode = "Please input your UserCode.";
            public const string msgInfoInputPassword = "Please input your Password.";
            public const string msgDeleteMessage = "Are you sure to delete this record? \r\nChoose OK to continue.\r\n";
            public const string msgDeleteMessageHeader = "Delete Confirmation.";
            public const string msgHasChangesDuringFormClosing = "There are some changes that are not committed to the database.\r\nClosing this form is not yet allowed.\r\n";
            public const string msgHasChangesDuringFormClosingHeader = "Form Closing Information.";
            public const string msgCodeExist = "Code already exists";
            public const string cancelledGeneric = "Generic record cannot be cancelled since data is used by stock master";
            public const string msgFailedLogOut = "Some forms are still in an Edit/Add mode.\nCannot continue to log-out.";
            public const string msgGenericMasterCode = "Invalid Generic Code";
            public const string msgDescRequired = "Description required";
            public const string msgStatusInf = "Please select status.";
            public const string msgErrInvalidUserCredentials = "The credentials you supplied are invalid.";

            //Payroll Calculation
            public const string msgProceed = "Proceed with processing?";
        }

        #endregion//messages

        #region ComboBox Values

        public class ComboBoxValues
        {
            public const string status_A = "Active";
            public const string status_F = "Fulfilled";
            public const string status_U = "On Hold";
            public const string status_C = "Cancelled";
            public const string status_N = "New";
            public const string partGen_T = "Generated";
            public const string partGen_N = "Not yet generated";
            public const string shipDet_S = "Sea";
            public const string shipDet_A = "Air";
            public const string shipDet_L = "Land";

            public const string CurrencyPESO = "php";
            public const string InspectCriteria_100 = "100%";
            public const string InspectCriteria_Non = "Non-Inspection";
            public const string InspectCriteria_Re = "Reinspection";

            public const string dateCode_RH = "Regular Holiday";
            public const string dateCode_CH = "Company Holiday";
            public const string dateCode_SP = "Special Holiday";
            public const string dateCode_CS = "Company Shutdown";
            public const string dateCode_RD = "Regular Day";
            public const string dateCode_NW = "No Work";
            public const string origin_L = "Local";
            public const string origin_F = "Foreign";
            public const string usageType_C = "Common Material";
            public const string usageType_D = "Dedicated";
            public const string critBalCategory_L = "Low";
            public const string critBalCategory_M = "Medium";
            public const string critBalCategory_H = "High";
        }


        public class GenericDBFields
        {
            public const string PositionComboBoxVal = "POSITION_VAL";
            public const string PositionComboBoxDisp = "POSITION_DISP";

            public const string CalendarScopeValueField = "CalendarScopeValue";
            public const string CalendarScopeDisplayField = "CalendarScopeDisplay";
        }

        #endregion

        #region Misc

        public class Misc
        {
            public const string LoginData = "LoginInfo";
            public const int PasswordLength = 15;
            public const int BOMDepthLevel = 11;
            public const string MDIStatusUser = "User: ";
            public const string SPODummySupplier = "V999999999";
            public const string ErrorLogPathSPOUploader = @"C:\";
            public const string newline = @"";
        }

        #endregion

        #region Data Format

        public class DataFormat
        {
            public const string NumericNoDecimal = "#,##0";
            public const string NumericQty = "#,##0.000";
            public const string NumericAmount = "#,##0.000000";
            public const string NumericTotal = "#,##0.00";
            public const string Date = "MM/dd/yyyy";
            public const string DateTime = "MM/dd/yyyy hh:mm tt";
        }
        #endregion


    }
}
