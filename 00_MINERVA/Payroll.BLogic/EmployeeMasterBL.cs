using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class EmployeeMasterBL : BaseBL
    {
        #region <Override Functions>

        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;
            
            #region query string
            string qString = @"INSERT INTO M_Employee
                                                (Mem_IDNo,
                                                Mem_LastName,
                                                Mem_FirstName,
                                                Mem_MiddleName,
                                                Mem_MaidenName,
                                                Mem_BirthDate,
                                                Mem_BirthPlace,
                                                Mem_Age,
                                                Mem_Gender,
                                                Mem_CivilStatusCode,
                                                Mem_MarriedDate,
                                                Mem_NationalityCode,
                                                Mem_ReligionCode,
                                                Mem_PresCompleteAddress,
                                                Mem_PresAddressBarangay,
                                                Mem_PresAddressMunicipalityCity,
                                                Mem_LandlineNo,
                                                Mem_CellNo,
                                                Mem_OfficeEmailAddress,
                                                Mem_PersonalEmail,
                                                Mem_EducationCode,
                                                Mem_SchoolCode,
                                                Mem_CourseCode,
                                                Mem_BloodType,
                                                Mem_SSSNo,
                                                Mem_PhilhealthNo,
                                                Mem_PayrollBankCode,
                                                Mem_BankAcctNo,
                                                Mem_TIN,
                                                Mem_TaxCode,
                                                Mem_PagIbigNo,
                                                Mem_PagIbigRule,
                                                Mem_PagIbigShare,
                                                Mem_ICEContactPerson,
                                                Mem_ICERelation,
                                                Mem_ICECompleteAddress,
                                                Mem_ICEAddressBarangay,
                                                Mem_ICEAddressMunicipalityCity,
                                                Mem_ICEContactNo,
                                                Mem_CostcenterDate,
                                                Mem_CostcenterCode,
                                                Mem_EmploymentStatusCode,
                                                Mem_IntakeDate,
                                                Mem_RegularDate,
                                                Mem_WorkLocationCode,
                                                Mem_JobTitleCode,
                                                Mem_PositionDate,
                                                Mem_Superior1,
                                                Mem_Superior2,
                                                Mem_IsComputedPayroll,
                                                Mem_PaymentMode,
                                                Mem_PayrollType,
                                                Mem_SalaryDate,
                                                Mem_Salary,
                                                Mem_WorkStatus,
                                                Mem_IsConfidential,
                                                Mem_ShiftCode,
                                                Mem_CalendarType,
                                                Mem_CalendarGroup,
                                                Mem_SeparationNoticeDate,
                                                Mem_SeparationCode,
                                                Mem_SeparationDate,
                                                Mem_ClearedDate,
                                                Mem_WifeClaim,
                                                Mem_ShoesSize,
                                                Mem_ShirtSize,
                                                Mem_HairColor,
                                                Mem_EyeColor,
                                                Mem_DistinguishMark,
                                                Mem_GraduatedDate,
                                                Mem_Contact1,
                                                Mem_Contact2,
                                                Mem_Contact3,
                                                Usr_Login,
                                                Ludatetime) 
                                               VALUES
                                                (@Mem_IDNo,
                                                @Mem_LastName,
                                                @Mem_FirstName,
                                                @Mem_MiddleName,
                                                @Mem_MaidenName,
                                                @Mem_BirthDate,
                                                @Mem_BirthPlace,
                                                @Mem_Age,
                                                @Mem_Gender,
                                                @Mem_CivilStatusCode,
                                                @Mem_MarriedDate,
                                                @Mem_NationalityCode,
                                                @Mem_ReligionCode,
                                                @Mem_PresCompleteAddress,
                                                @Mem_PresAddressBarangay,
                                                @Mem_PresAddressMunicipalityCity,
                                                @Mem_LandlineNo,
                                                @Mem_CellNo,
                                                @Mem_OfficeEmailAddress,
                                                @Mem_PersonalEmail,
                                                @Mem_EducationCode,
                                                @Mem_SchoolCode,
                                                @Mem_CourseCode,
                                                @Mem_BloodType,
                                                @Mem_SSSNo,
                                                @Mem_PhilhealthNo,
                                                @Mem_PayrollBankCode,
                                                @Mem_BankAcctNo,
                                                @Mem_TIN,
                                                @Mem_TaxCode,
                                                @Mem_PagIbigNo,
                                                @Mem_PagIbigRule,
                                                @Mem_PagIbigShare,
                                                @Mem_ICEContactPerson,
                                                @Mem_ICERelation,
                                                @Mem_ICECompleteAddress,
                                                @Mem_ICEAddressBarangay,
                                                @Mem_ICEAddressMunicipalityCity,
                                                @Mem_ICEContactNo,
                                                @Mem_CostcenterDate,
                                                @Mem_CostcenterCode,
                                                @Mem_EmploymentStatusCode,
                                                @Mem_IntakeDate,
                                                @Mem_RegularDate,
                                                @Mem_WorkLocationCode,
                                                @Mem_JobTitleCode,
                                                @Mem_PositionDate,
                                                @Mem_Superior1,
                                                @Mem_Superior2,
                                                @Mem_IsComputedPayroll,
                                                @Mem_PaymentMode,
                                                @Mem_PayrollType,
                                                @Mem_SalaryDate,
                                                @Mem_Salary,
                                                @Mem_WorkStatus,
                                                @Mem_IsConfidential,
                                                @Mem_ShiftCode,
                                                @Mem_CalendarType,
                                                @Mem_CalendarGroup,
                                                @Mem_SeparationNoticeDate,
                                                @Mem_SeparationCode,
                                                @Mem_SeparationDate,
                                                @Mem_ClearedDate,
                                                @Mem_WifeClaim,
                                                @Mem_ShoesSize,
                                                @Mem_ShirtSize,
                                                @Mem_HairColor,
                                                @Mem_EyeColor,
                                                @Mem_DistinguishMark,
                                                @Mem_GraduatedDate,
                                                @Mem_Contact1,
                                                @Mem_Contact2,
                                                @Mem_Contact3,
                                                @Usr_Login,
                                                GetDate())";

            #endregion

            ParameterInfo[] UpdateparamInfo = new ParameterInfo[1];
            UpdateparamInfo[0] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            ParameterInfo[] paramInfo = new ParameterInfo[78];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Mem_LastName", row["Mem_LastName"]);
            paramInfo[2] = new ParameterInfo("@Mem_FirstName", row["Mem_FirstName"]);
            paramInfo[3] = new ParameterInfo("@Mem_MiddleName", row["Mem_MiddleName"]);
            paramInfo[4] = new ParameterInfo("@Mem_MaidenName", row["Mem_MaidenName"]);
            paramInfo[5] = new ParameterInfo("@Mem_BirthDate", row["Mem_BirthDate"]);
            paramInfo[6] = new ParameterInfo("@Mem_BirthPlace", row["Mem_BirthPlace"]);
            paramInfo[7] = new ParameterInfo("@Mem_Age", row["Mem_Age"]);
            paramInfo[8] = new ParameterInfo("@Mem_Gender", row["Mem_Gender"]);
            paramInfo[9] = new ParameterInfo("@Mem_CivilStatusCode", row["Mem_CivilStatusCode"]);
            paramInfo[10] = new ParameterInfo("@Mem_MarriedDate", row["Mem_MarriedDate"]);
            paramInfo[11] = new ParameterInfo("@Mem_NationalityCode", row["Mem_NationalityCode"]);
            paramInfo[12] = new ParameterInfo("@Mem_ReligionCode", row["Mem_ReligionCode"]);
            paramInfo[13] = new ParameterInfo("@Mem_PresCompleteAddress", row["Mem_PresCompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[14] = new ParameterInfo("@Mem_PresAddressBarangay", row["Mem_PresAddressBarangay"]);
            paramInfo[15] = new ParameterInfo("@Mem_PresAddressMunicipalityCity", row["Mem_PresAddressMunicipalityCity"]);
            paramInfo[16] = new ParameterInfo("@Mem_LandlineNo", row["Mem_LandlineNo"]);
            paramInfo[17] = new ParameterInfo("@Mem_CellNo", row["Mem_CellNo"]);
            paramInfo[18] = new ParameterInfo("@Mem_OfficeEmailAddress", row["Mem_OfficeEmailAddress"]);
            paramInfo[19] = new ParameterInfo("@Mem_EducationCode", row["Mem_EducationCode"]);
            paramInfo[20] = new ParameterInfo("@Mem_SchoolCode", row["Mem_SchoolCode"]);
            paramInfo[21] = new ParameterInfo("@Mem_CourseCode", row["Mem_CourseCode"]);
            paramInfo[22] = new ParameterInfo("@Mem_BloodType", row["Mem_BloodType"]);
            paramInfo[23] = new ParameterInfo("@Mem_SSSNo", row["Mem_SSSNo"]);
            paramInfo[24] = new ParameterInfo("@Mem_PhilhealthNo", row["Mem_PhilhealthNo"]);
            paramInfo[25] = new ParameterInfo("@Mem_PayrollBankCode", row["Mem_PayrollBankCode"]);
            paramInfo[26] = new ParameterInfo("@Mem_BankAcctNo", row["Mem_BankAcctNo"]);
            paramInfo[27] = new ParameterInfo("@Mem_TIN", row["Mem_TIN"]);
            paramInfo[28] = new ParameterInfo("@Mem_TaxCode", row["Mem_TaxCode"]);
            paramInfo[29] = new ParameterInfo("@Mem_PagIbigNo", row["Mem_PagIbigNo"]);
            paramInfo[30] = new ParameterInfo("@Mem_PagIbigRule", row["Mem_PagIbigRule"]);
            paramInfo[31] = new ParameterInfo("@Mem_PagIbigShare", row["Mem_PagIbigShare"]);
            paramInfo[32] = new ParameterInfo("@Mem_ICEContactPerson", row["Mem_ICEContactPerson"]);
            paramInfo[33] = new ParameterInfo("@Mem_ICERelation", row["Mem_ICERelation"]);
            paramInfo[34] = new ParameterInfo("@Mem_ICECompleteAddress", row["Mem_ICECompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[35] = new ParameterInfo("@Mem_ICEAddressBarangay", row["Mem_ICEAddressBarangay"]);
            paramInfo[36] = new ParameterInfo("@Mem_ICEAddressMunicipalityCity", row["Mem_ICEAddressMunicipalityCity"]);
            paramInfo[37] = new ParameterInfo("@Mem_ICEContactNo", row["Mem_ICEContactNo"]);
            paramInfo[38] = new ParameterInfo("@Mem_CostcenterDate", row["Mem_CostcenterDate"]);
            paramInfo[39] = new ParameterInfo("@Mem_CostcenterCode", row["Mem_CostcenterCode"]);
            paramInfo[40] = new ParameterInfo("@Mem_EmploymentStatusCode", row["Mem_EmploymentStatusCode"]);
            paramInfo[41] = new ParameterInfo("@Mem_IntakeDate", row["Mem_IntakeDate"]);
            paramInfo[42] = new ParameterInfo("@Mem_RegularDate", row["Mem_RegularDate"]);
            paramInfo[43] = new ParameterInfo("@Mem_WorkLocationCode", row["Mem_WorkLocationCode"]);
            paramInfo[44] = new ParameterInfo("@Mem_JobTitleCode", row["Mem_JobTitleCode"]);
            paramInfo[45] = new ParameterInfo("@Mem_PositionDate", row["Mem_PositionDate"]);
            paramInfo[46] = new ParameterInfo("@Mem_Superior1", row["Mem_Superior1"]);
            paramInfo[47] = new ParameterInfo("@Mem_Superior2", row["Mem_Superior2"]);
            paramInfo[48] = new ParameterInfo("@Mem_IsComputedPayroll", row["Mem_IsComputedPayroll"]);
            paramInfo[49] = new ParameterInfo("@Mem_PaymentMode", row["Mem_PaymentMode"]);
            paramInfo[50] = new ParameterInfo("@Mem_PayrollType", row["Mem_PayrollType"]);
            paramInfo[51] = new ParameterInfo("@Mem_SalaryDate", row["Mem_SalaryDate"]);
            paramInfo[52] = new ParameterInfo("@Mem_Salary", row["Mem_Salary"]);
            paramInfo[53] = new ParameterInfo("@Mem_WorkStatus", row["Mem_WorkStatus"]);
            paramInfo[54] = new ParameterInfo("@Mem_IsConfidential", row["Mem_IsConfidential"]);
            paramInfo[55] = new ParameterInfo("@Mem_ShiftCode", row["Mem_ShiftCode"]);
            paramInfo[56] = new ParameterInfo("@Mem_CalendarType", row["Mem_CalendarType"]);
            paramInfo[57] = new ParameterInfo("@Mem_CalendarGroup", row["Mem_CalendarGroup"]);
            paramInfo[58] = new ParameterInfo("@Mem_SeparationNoticeDate", row["Mem_SeparationNoticeDate"]);
            paramInfo[59] = new ParameterInfo("@Mem_SeparationCode", row["Mem_SeparationCode"]);
            paramInfo[60] = new ParameterInfo("@Mem_SeparationDate", row["Mem_SeparationDate"]);
            paramInfo[61] = new ParameterInfo("@Mem_ClearedDate", row["Mem_ClearedDate"]);
            paramInfo[62] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[63] = new ParameterInfo("@Mem_NickName", row["Mem_NickName"]);
            paramInfo[64] = new ParameterInfo("@Mem_WifeClaim", row["Mem_WifeClaim"]);

            paramInfo[65] = new ParameterInfo("@Mem_ShoesSize", row["Mem_ShoesSize"]);
            paramInfo[66] = new ParameterInfo("@Mem_ShirtSize", row["Mem_ShirtSize"]);
            paramInfo[67] = new ParameterInfo("@Mem_HairColor", row["Mem_HairColor"]);
            paramInfo[68] = new ParameterInfo("@Mem_EyeColor", row["Mem_EyeColor"]);
            paramInfo[69] = new ParameterInfo("@Mem_DistinguishMark", row["Mem_DistinguishMark"]);

            paramInfo[70] = new ParameterInfo("@Mem_PersonalEmail", row["Mem_PersonalEmail"]);
            paramInfo[71] = new ParameterInfo("@Mem_GraduatedDate", row["Mem_GraduatedDate"]);
            paramInfo[72] = new ParameterInfo("@Mem_Contact1", row["Mem_Contact1"]);
            paramInfo[73] = new ParameterInfo("@Mem_Contact2", row["Mem_Contact2"]);
            paramInfo[74] = new ParameterInfo("@Mem_Contact3", row["Mem_Contact3"]);
            paramInfo[75] = new ParameterInfo("@Mem_OldPayrollType", row["Mem_PayrollType"]);
            paramInfo[76] = new ParameterInfo("@Mem_OldSalaryDate", row["Mem_SalaryDate"]);
            paramInfo[77] = new ParameterInfo("@Mem_OldSalaryRate", row["Mem_Salary"]);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    //update company master last employee id number 
                    //retVal = dal.ExecuteNonQuery(UpdateString, CommandType.Text, UpdateparamInfo);
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

            return retVal;
        }

        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[78];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Mem_LastName", row["Mem_LastName"]);
            paramInfo[2] = new ParameterInfo("@Mem_FirstName", row["Mem_FirstName"]);
            paramInfo[3] = new ParameterInfo("@Mem_MiddleName", row["Mem_MiddleName"]);
            paramInfo[4] = new ParameterInfo("@Mem_MaidenName", row["Mem_MaidenName"]);
            paramInfo[5] = new ParameterInfo("@Mem_BirthDate", row["Mem_BirthDate"]);
            paramInfo[6] = new ParameterInfo("@Mem_BirthPlace", row["Mem_BirthPlace"]);
            paramInfo[7] = new ParameterInfo("@Mem_Age", row["Mem_Age"]);
            paramInfo[8] = new ParameterInfo("@Mem_Gender", row["Mem_Gender"]);
            paramInfo[9] = new ParameterInfo("@Mem_CivilStatusCode", row["Mem_CivilStatusCode"]);
            paramInfo[10] = new ParameterInfo("@Mem_MarriedDate", row["Mem_MarriedDate"]);
            paramInfo[11] = new ParameterInfo("@Mem_NationalityCode", row["Mem_NationalityCode"]);
            paramInfo[12] = new ParameterInfo("@Mem_ReligionCode", row["Mem_ReligionCode"]);
            paramInfo[13] = new ParameterInfo("@Mem_PresCompleteAddress", row["Mem_PresCompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[14] = new ParameterInfo("@Mem_PresAddressBarangay", row["Mem_PresAddressBarangay"]);
            paramInfo[15] = new ParameterInfo("@Mem_PresAddressMunicipalityCity", row["Mem_PresAddressMunicipalityCity"]);
            paramInfo[16] = new ParameterInfo("@Mem_LandlineNo", row["Mem_LandlineNo"]);
            paramInfo[17] = new ParameterInfo("@Mem_CellNo", row["Mem_CellNo"]);
            paramInfo[18] = new ParameterInfo("@Mem_OfficeEmailAddress", row["Mem_OfficeEmailAddress"]);
            paramInfo[19] = new ParameterInfo("@Mem_EducationCode", row["Mem_EducationCode"]);
            paramInfo[20] = new ParameterInfo("@Mem_SchoolCode", row["Mem_SchoolCode"]);
            paramInfo[21] = new ParameterInfo("@Mem_CourseCode", row["Mem_CourseCode"]);
            paramInfo[22] = new ParameterInfo("@Mem_BloodType", row["Mem_BloodType"]);
            paramInfo[23] = new ParameterInfo("@Mem_SSSNo", row["Mem_SSSNo"]);
            paramInfo[24] = new ParameterInfo("@Mem_PhilhealthNo", row["Mem_PhilhealthNo"]);
            paramInfo[25] = new ParameterInfo("@Mem_PayrollBankCode", row["Mem_PayrollBankCode"]);
            paramInfo[26] = new ParameterInfo("@Mem_BankAcctNo", row["Mem_BankAcctNo"]);
            paramInfo[27] = new ParameterInfo("@Mem_TIN", row["Mem_TIN"]);
            paramInfo[28] = new ParameterInfo("@Mem_TaxCode", row["Mem_TaxCode"]);
            paramInfo[29] = new ParameterInfo("@Mem_PagIbigNo", row["Mem_PagIbigNo"]);
            paramInfo[30] = new ParameterInfo("@Mem_PagIbigRule", row["Mem_PagIbigRule"]);
            paramInfo[31] = new ParameterInfo("@Emt_HDMFContribution", row["Emt_HDMFContribution"]);
            paramInfo[32] = new ParameterInfo("@Mem_ICEContactPerson", row["Mem_ICEContactPerson"]);
            paramInfo[33] = new ParameterInfo("@Mem_ICERelation", row["Mem_ICERelation"]);
            paramInfo[34] = new ParameterInfo("@Mem_ICECompleteAddress", row["Mem_ICECompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[35] = new ParameterInfo("@Mem_ICEAddressBarangay", row["Mem_ICEAddressBarangay"]);
            paramInfo[36] = new ParameterInfo("@Mem_ICEAddressMunicipalityCity", row["Mem_ICEAddressMunicipalityCity"]);
            paramInfo[37] = new ParameterInfo("@Mem_ICEContactNo", row["Mem_ICEContactNo"]);
            paramInfo[38] = new ParameterInfo("@Mem_CostcenterDate", row["Mem_CostcenterDate"]);
            paramInfo[39] = new ParameterInfo("@Mem_CostcenterCode", row["Mem_CostcenterCode"]);
            paramInfo[40] = new ParameterInfo("@Mem_EmploymentStatusCode", row["Mem_EmploymentStatusCode"]);
            paramInfo[41] = new ParameterInfo("@Mem_IntakeDate", row["Mem_IntakeDate"]);
            paramInfo[42] = new ParameterInfo("@Mem_RegularDate", row["Mem_RegularDate"]);
            paramInfo[43] = new ParameterInfo("@Mem_WorkLocationCode", row["Mem_WorkLocationCode"]);
            paramInfo[44] = new ParameterInfo("@Mem_JobTitleCode", row["Mem_JobTitleCode"]);
            paramInfo[45] = new ParameterInfo("@Mem_PositionDate", row["Mem_PositionDate"]);
            paramInfo[46] = new ParameterInfo("@Mem_Superior1", row["Mem_Superior1"]);
            paramInfo[47] = new ParameterInfo("@Mem_Superior2", row["Mem_Superior2"]);
            paramInfo[48] = new ParameterInfo("@Mem_IsComputedPayroll", row["Mem_IsComputedPayroll"]);
            paramInfo[49] = new ParameterInfo("@Mem_PaymentMode", row["Mem_PaymentMode"]);
            paramInfo[50] = new ParameterInfo("@Mem_PayrollType", row["Mem_PayrollType"]);
            paramInfo[51] = new ParameterInfo("@Mem_SalaryDate", row["Mem_SalaryDate"]);
            paramInfo[52] = new ParameterInfo("@Mem_Salary", row["Mem_Salary"]);
            paramInfo[53] = new ParameterInfo("@Mem_WorkStatus", row["Mem_WorkStatus"]);
            paramInfo[54] = new ParameterInfo("@Mem_IsConfidential", row["Mem_IsConfidential"]);
            paramInfo[55] = new ParameterInfo("@Mem_ShiftCode", row["Mem_ShiftCode"]);
            paramInfo[56] = new ParameterInfo("@Mem_CalendarType", row["Mem_CalendarType"]);
            paramInfo[57] = new ParameterInfo("@Mem_CalendarGroup", row["Mem_CalendarGroup"]);
            paramInfo[58] = new ParameterInfo("@Mem_SeparationNoticeDate", row["Mem_SeparationNoticeDate"]);
            paramInfo[59] = new ParameterInfo("@Mem_SeparationCode", row["Mem_SeparationCode"]);
            paramInfo[60] = new ParameterInfo("@Mem_SeparationDate", row["Mem_SeparationDate"]);
            paramInfo[61] = new ParameterInfo("@Mem_ClearedDate", row["Mem_ClearedDate"]);
            paramInfo[62] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[63] = new ParameterInfo("@Mem_WifeClaim", row["Mem_WifeClaim"]);

            paramInfo[64] = new ParameterInfo("@Mem_ShoesSize", row["Mem_ShoesSize"]);
            paramInfo[65] = new ParameterInfo("@Mem_ShirtSize", row["Mem_ShirtSize"]);
            paramInfo[66] = new ParameterInfo("@Mem_HairColor", row["Mem_HairColor"]);
            paramInfo[67] = new ParameterInfo("@Mem_EyeColor", row["Mem_EyeColor"]);
            paramInfo[68] = new ParameterInfo("@Mem_DistinguishMark", row["Mem_DistinguishMark"]);

            paramInfo[69] = new ParameterInfo("@Mem_PersonalEmail", row["Mem_PersonalEmail"]);
            paramInfo[70] = new ParameterInfo("@Mem_GraduatedDate", row["Mem_GraduatedDate"]);
            paramInfo[71] = new ParameterInfo("@Mem_Contact1", row["Mem_Contact1"]);
            paramInfo[72] = new ParameterInfo("@Mem_Contact2", row["Mem_Contact2"]);
            paramInfo[73] = new ParameterInfo("@Mem_Contact3", row["Mem_Contact3"]);
            paramInfo[74] = new ParameterInfo("@Mem_NickName", row["Mem_NickName"]);

            paramInfo[75] = new ParameterInfo("@Mem_OldPayrollType", row["Mem_PayrollType"]);
            paramInfo[76] = new ParameterInfo("@Mem_OldSalaryDate", row["Mem_SalaryDate"]);
            paramInfo[77] = new ParameterInfo("@Mem_OldSalaryRate", row["Mem_Salary"]);

            string sqlQuery = @"UPDATE M_Employee 
                                               SET Mem_LastName = @Mem_LastName,
                                                Mem_FirstName = @Mem_FirstName,
                                                Mem_MiddleName = @Mem_MiddleName,
                                                Mem_MaidenName = @Mem_MaidenName,
                                                Mem_BirthDate = @Mem_BirthDate,
                                                Mem_BirthPlace = @Mem_BirthPlace,
                                                Mem_Age = @Mem_Age,
                                                Mem_Gender = @Mem_Gender,
                                                Mem_CivilStatusCode = @Mem_CivilStatusCode,
                                                Mem_MarriedDate = @Mem_MarriedDate,
                                                Mem_NationalityCode = @Mem_NationalityCode,
                                                Mem_ReligionCode = @Mem_ReligionCode,
                                                Mem_PresCompleteAddress = @Mem_PresCompleteAddress,
                                                Mem_PresAddressBarangay = @Mem_PresAddressBarangay,
                                                Mem_PresAddressMunicipalityCity = @Mem_PresAddressMunicipalityCity,
                                                Mem_LandlineNo = @Mem_LandlineNo,
                                                Mem_CellNo = @Mem_CellNo,
                                                Mem_OfficeEmailAddress = @Mem_OfficeEmailAddress,
                                                Mem_PersonalEmail = @Mem_PersonalEmail,
                                                Mem_EducationCode = @Mem_EducationCode,
                                                Mem_SchoolCode = @Mem_SchoolCode,
                                                Mem_CourseCode = @Mem_CourseCode,
                                                Mem_BloodType = @Mem_BloodType,
                                                Mem_SSSNo = @Mem_SSSNo,
                                                Mem_PhilhealthNo = @Mem_PhilhealthNo,
                                                Mem_PayrollBankCode = @Mem_PayrollBankCode,
                                                Mem_BankAcctNo = @Mem_BankAcctNo,
                                                Mem_TIN = @Mem_TIN,
                                                Mem_TaxCode = @Mem_TaxCode,
                                                Mem_PagIbigNo = @Mem_PagIbigNo,
                                                Mem_PagIbigRule = @Mem_PagIbigRule,
                                                Mem_PagIbigShare = @Mem_PagIbigShare,
                                                Mem_ICEContactPerson = @Mem_ICEContactPerson,
                                                Mem_ICERelation = @Mem_ICERelation,
                                                Mem_ICECompleteAddress = @Mem_ICECompleteAddress,
                                                Mem_ICEAddressBarangay = @Mem_ICEAddressBarangay,
                                                Mem_ICEAddressMunicipalityCity = @Mem_ICEAddressMunicipalityCity,
                                                Mem_ICEContactNo = @Mem_ICEContactNo,
                                                Mem_CostcenterDate =  @Mem_CostcenterDate,
                                                Mem_CostcenterCode = @Mem_CostcenterCode,
                                                Mem_EmploymentStatusCode = @Mem_EmploymentStatusCode,
                                                Mem_IntakeDate = @Mem_IntakeDate,
                                                Mem_RegularDate = @Mem_RegularDate,
                                                Mem_WorkLocationCode = @Mem_WorkLocationCode,
                                                Mem_JobTitleCode = @Mem_JobTitleCode,
                                                Mem_PositionDate = @Mem_PositionDate,
                                                Mem_Superior1 = @Mem_Superior1,
                                                Mem_Superior2 = @Mem_Superior2,
                                                Mem_IsComputedPayroll = @Mem_IsComputedPayroll,
                                                Mem_PaymentMode = @Mem_PaymentMode,
                                                Mem_PayrollType = @Mem_PayrollType,
                                                Mem_SalaryDate = @Mem_SalaryDate,
                                                Mem_Salary = @Mem_Salary,
                                                Mem_WorkStatus = @Mem_WorkStatus,
                                                Mem_IsConfidential = @Mem_IsConfidential,
                                                Mem_ShiftCode = @Mem_ShiftCode,
                                                Mem_CalendarType = @Mem_CalendarType,
                                                Mem_CalendarGroup = @Mem_CalendarGroup,
                                                Mem_SeparationNoticeDate = @Mem_SeparationNoticeDate,
                                                Mem_SeparationCode = @Mem_SeparationCode,
                                                Mem_SeparationDate = @Mem_SeparationDate
                                                ,Mem_SSSRule = @Mem_SSSRule
                                                ,Mem_SSSShare = @Mem_SSSShare
                                                ,Mem_PHRule = @Mem_PHRule
                                                ,Mem_PHShare = @Mem_PHShare
                                                ,Mem_ClearedDate = @Mem_ClearedDate
                                                ,Mem_ProbationDate = @Mem_ProbationDate
                                                ,Usr_Login = @Usr_Login
                                                ,Ludatetime = GetDate()
                                                ,Mem_Height = @Mem_Height
                                                ,Mem_Weight = @Mem_Weight
                                                ,Mem_ProvCompleteAddress = @Mem_ProvCompleteAddress
                                                ,Mem_ProvAddressBarangay = @Mem_ProvAddressBarangay
                                                ,Mem_ProvAddressMunicipalityCity = @Mem_ProvAddressMunicipalityCity
                                                ,Mem_ProvLandlineNo = @Mem_ProvLandlineNo
                                                ,Mem_AwardsRecognition = @Mem_AwardsRecognition
                                                ,Mem_PRCLicense = @Mem_PRCLicense
                                                ,Mem_ExpenseClass = @Mem_ExpenseClass
                                                ,Mem_WifeClaim = @Mem_WifeClaim
                                                ,Mem_ShoesSize = @Mem_ShoesSize
                                                ,Mem_ShirtSize = @Mem_ShirtSize
                                                ,Mem_HairColor = @Mem_HairColor
                                                ,Mem_EyeColor = @Mem_EyeColor
                                                ,Mem_DistinguishMark = @Mem_DistinguishMark
                                                ,Mem_GraduatedDate = @Mem_GraduatedDate
                                                ,Mem_Contact1 = @Mem_Contact1
                                                ,Mem_Contact2 = @Mem_Contact2
                                                ,Mem_Contact3 = @Mem_Contact3
                                                ,Mem_NickName = @Mem_NickName
                                                ,Mem_OldPayrollType = @Mem_OldPayrollType
                                                ,Mem_OldSalaryDate = @Mem_OldSalaryDate
                                                ,Mem_OldSalaryRate = @Mem_OldSalaryRate
                                                WHERE Mem_IDNo=@Mem_IDNo";

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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public override int Delete(string Mem_IDNo, string Usr_Login)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", Mem_IDNo);
            paramInfo[1] = new ParameterInfo("@Usr_Login", Usr_Login);

            string sqlQuery = @"UPDATE M_Employee
                                               SET Mem_WorkStatus = 'IN'
	                                              ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = Getdate()
                                             WHERE Mem_IDNo = @Mem_IDNo";

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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
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

            string sqlQuery = @"SELECT Mem_IDNo,
                                                        Mem_LastName,
                                                        Mem_FirstName,
                                                        Mem_MiddleName,
                                                        Mem_MaidenName,
                                                        Mem_BirthDate,
                                                        Mem_BirthPlace,
                                                        Mem_Age,
                                                        Mem_Gender,
                                                        Mem_CivilStatusCode,
                                                        Mem_MarriedDate,
                                                        Mem_NationalityCode,
                                                        Mem_ReligionCode,
                                                        Mem_PresCompleteAddress,
                                                        Mem_PresAddressBarangay,
                                                        Mem_PresAddressMunicipalityCity,
                                                        Mem_LandlineNo,
                                                        Mem_CellNo,
                                                        Mem_OfficeEmailAddress,
                                                        Mem_PersonalEmail,
                                                        Mem_EducationCode,
                                                        Mem_SchoolCode,
                                                        Mem_CourseCode,
                                                        Mem_BloodType,
                                                        Mem_SSSNo,
                                                        Mem_PhilhealthNo,
                                                        Mem_PayrollBankCode,
                                                        Mem_BankAcctNo,
                                                        Mem_TIN,
                                                        Mem_TaxCode,
                                                        Mem_PagIbigNo,
                                                        --Emt_HDMFContribution,
                                                        Mem_ICEContactPerson,
                                                        Mem_ICERelation,
                                                        Mem_ICECompleteAddress,
                                                        Mem_ICEAddressBarangay,
                                                        Mem_ICEAddressMunicipalityCity,
                                                        Mem_ICEContactNo,
                                                        Mem_CostcenterDate,
                                                        Replace(Mdv_DivName, ' ', '') + '/' + Replace(Mdp_DptName,' ','') + '/' + Replace(Msc_SecName,' ','') + '/' + Replace(Msb_SubSecName,' ','') + '/' + Replace(Mpr_PrcName,' ','') AS 'Mem_CostcenterCode',
                                                        Mem_EmploymentStatusCode,
                                                        Mem_IntakeDate,
                                                        Mem_RegularDate,
                                                        Mem_WorkLocationCode,
                                                        Mem_JobTitleCode,
                                                        Mem_PositionDate,
                                                        Mem_Superior1,
                                                        Mem_Superior2,
                                                        Mem_IsComputedPayroll,
                                                        Mem_PaymentMode,
                                                        Mem_PayrollType,
                                                        Mem_SalaryDate,
                                                        Mem_Salary,
                                                        Mem_WorkStatus,
                                                        Mem_IsConfidential,
                                                        Mem_ShiftCode,
                                                        Mem_CalendarType,
                                                        Mem_CalendarGroup,
                                                        Mem_SeparationNoticeDate,
                                                        Mem_SeparationCode,
                                                        Mem_SeparationDate,
                                                        Mem_ClearedDate,
                                                        Mem_CostcenterCode as 'CostCenterCode'
                                                        ,Mem_PagIbigRule
                                                        ,Mem_PagIbigShare
                                                        ,Mem_SSSRule
                                                        ,Mem_SSSShare
                                                        ,Mem_PHRule
                                                        ,Mem_PHShare
                                                        ,Mem_Image
                                                        ,Mem_ProbationDate
                                                        ,Mem_ExpenseClass
                                                        ,Mem_TaxPayrollType
                                                        ,Mem_TaxSalaryDate
                                                        ,Mem_TaxSalary
                                                        ,Mem_BillingPayrollType
                                                        ,Mem_BillingDate
                                                        ,Mem_BillingSalary
                                                        ---,Emt_BasicPayrollType
                                                        ---,Emt_BasicEffectivityDate
                                                        ---,Emt_BasicRate
                                                        ,Mem_Height
                                                        ,Mem_Weight
                                                        ,Mem_ProvCompleteAddress
                                                        ,Mem_ProvAddressBarangay
                                                        ,Mem_ProvAddressMunicipalityCity
                                                        ,Mem_ProvLandlineNo
                                                        ,Mem_ProvLandlineNo
                                                        ,Mem_AwardsRecognition
                                                        ,Mem_PRCLicense
                                                        ,Mem_WifeClaim
                                                        ,Mem_ShoesSize
                                                        ,Mem_ShirtSize
                                                        ,Mem_HairColor
                                                        ,Mem_EyeColor
                                                        ,Mem_DistinguishMark
                                                        ,Mem_GraduatedDate
                                                        ,Mem_Contact1
                                                        ,Mem_Contact2
                                                        ,Mem_Contact3
                                                        ,Mem_OldPayrollType
                                                        ,Mem_OldSalaryDate
                                                        ,Mem_OldSalaryRate
	                                                  FROM M_Employee
                                                      LEFT JOIN M_CodeDtl AcctDtl1 on Mem_JobTitleCode = AcctDtl1.Mcd_Code
                                                        AND AcctDtl1.Mcd_CodeType = 'POSITION'
                                                      LEFT JOIN M_CodeDtl AcctDtl2 on Mem_Gender = AcctDtl2.Mcd_Code 
                                                        AND AcctDtl2.Mcd_CodeType = 'GENDER'
                                                      LEFT JOIN M_CodeDtl AcctDtl3 on Mem_WorkStatus = AcctDtl3.Mcd_Code
                                                        AND AcctDtl3.Mcd_CodeType = 'JOBSTAT'
                                                      LEFT JOIN M_CodeDtl AcctDtl4 on Mem_EmploymentStatusCode = AcctDtl4.Mcd_Code
                                                        AND AcctDtl4.Mcd_CodeType = 'EMPLOYSTAT'
                                                      LEFT JOIN M_CodeDtl AcctDtl5 on Mem_CalendarType = AcctDtl5.Mcd_Code
                                                        AND AcctDtl5.Mcd_CodeType = 'WORKTYPE'
                                                      LEFT JOIN M_CostCenter ON Mem_CostcenterCode = Mcc_CostCenterCode
                                                      LEFT JOIN M_Division ON Mcc_DivCode = Mdv_DivCode
                                                      LEFT JOIN M_Department ON Mcc_DptCode = Mdp_DptCode
                                                      LEFT JOIN M_Section ON Mcc_SecCode = Msc_SecCode
                                                      LEFT JOIN M_SubSection ON Mcc_SubsecCode = Msb_SubSecCode
                                                      LEFT JOIN M_Process ON Mcc_PrcCode = Mpr_PrcCode
                                                      Where Mem_IDNo = @Mem_IDNo";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override DataRow Fetch(string code)
        {
            CommonProcedures.ShowMessage(10113, ""); return null;
        }

        #endregion

        #region <Functions Defined>

        public bool CheckCanRetrieve(string User_Code, string SysMenuCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@UserCode", User_Code);
            paramInfo[1] = new ParameterInfo("@Mra_ModuleCode", SysMenuCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mra_CanView " +
                                        "FROM M_UserRoleAccess " +
                                        "INNER JOIN M_UserDtl " +
	                                        "ON Mra_UserRoleCode = Mud_UserRoleCode " +
		                                        "AND Mra_SystemCode = Mud_SystemCode " +
		                                        "AND Mra_RecordStatus = 'A' " +
                                        "WHERE Mud_UserCode = @UserCode " +
                                        "AND	Mra_ModuleCode=@Mra_ModuleCode " +
                                        "AND	Mra_RecordStatus = 'A'", CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Mra_CanView"].ToString().Trim() == "True")
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public DataSet FetchAll(string Mem_IDNo)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", Mem_IDNo);

            string sqlQuery = @"SELECT Mem_IDNo,
                                                        Mem_LastName,
                                                        Mem_FirstName,
                                                        Mem_MiddleName,
                                                        Mem_MaidenName,
                                                        Mem_BirthDate,
                                                        Mem_BirthPlace,
                                                        Mem_Age,
                                                        Mem_Gender,
                                                        Mem_CivilStatusCode,
                                                        Mem_MarriedDate,
                                                        Mem_NationalityCode,
                                                        Mem_ReligionCode,
                                                        Mem_PresCompleteAddress,
                                                        Mem_PresAddressBarangay,
                                                        Mem_PresAddressMunicipalityCity,
                                                        Mem_LandlineNo,
                                                        Mem_CellNo,
                                                        Mem_OfficeEmailAddress,
                                                        Mem_PersonalEmail,
                                                        Mem_EducationCode,
                                                        Mem_SchoolCode,
                                                        Mem_CourseCode,
                                                        Mem_BloodType,
                                                        Mem_SSSNo,
                                                        Mem_PhilhealthNo,
                                                        Mem_PayrollBankCode,
                                                        Mem_BankAcctNo,
                                                        Mem_TIN,
                                                        Mem_TaxCode,
                                                        Mem_PagIbigNo,
                                                        --Emt_HDMFContribution,
                                                        Mem_ICEContactPerson,
                                                        Mem_ICERelation,
                                                        Mem_ICECompleteAddress,
                                                        Mem_ICEAddressBarangay,
                                                        Mem_ICEAddressMunicipalityCity,
                                                        Mem_ICEContactNo,
                                                        Mem_CostcenterDate,
                                                        Replace(Mdv_DivName, ' ', '') + '/' + Replace(Mdp_DptName,' ','') + '/' + Replace(Msc_SecName,' ','') + '/' + Replace(Msb_SubSecName,' ','') + '/' + Replace(Mpr_PrcName,' ','') AS 'Mem_CostcenterCode',
                                                        Mem_EmploymentStatusCode,
                                                        Mem_IntakeDate,
                                                        Mem_RegularDate,
                                                        Mem_WorkLocationCode,
                                                        Mem_JobTitleCode,
                                                        Mem_PositionDate,
                                                        Mem_Superior1,
                                                        Mem_Superior2,
                                                        Mem_IsComputedPayroll,
                                                        Mem_PaymentMode,
                                                        Mem_PayrollType,
                                                        Mem_SalaryDate,
                                                        Mem_Salary,
                                                        Mem_WorkStatus,
                                                        Mem_IsConfidential,
                                                        Mem_ShiftCode,
                                                        Mem_CalendarType,
                                                        Mem_CalendarGroup,
                                                        Mem_SeparationNoticeDate,
                                                        Mem_SeparationCode,
                                                        Mem_SeparationDate,
                                                        Mem_ClearedDate,
                                                        Mem_CostcenterCode as 'CostCenterCode'
                                                        ,Mem_PagIbigRule
                                                        ,Mem_PagIbigShare
                                                        ,Mem_SSSRule
                                                        ,Mem_SSSShare
                                                        ,Mem_PHRule
                                                        ,Mem_PHShare
                                                        ,Mem_Image
                                                        ,Mem_ProbationDate
                                                        ,Mem_ExpenseClass
                                                        ,Mem_SeparationSysDate
                                                        ,Mem_TaxPayrollType
                                                        ,Mem_TaxSalaryDate
                                                        ,Mem_TaxSalary
                                                        ,Mem_BillingPayrollType
                                                        ,Mem_BillingDate
                                                        ,Mem_BillingSalary
                                                        --,Emt_BasicPayrollType
                                                        --,Emt_BasicEffectivityDate
                                                        --,Emt_BasicRate
                                                        ,Mem_Height
                                                        ,Mem_Weight
                                                        ,Mem_ProvCompleteAddress
                                                        ,Mem_ProvAddressBarangay
                                                        ,Mem_ProvAddressMunicipalityCity
                                                        ,Mem_ProvLandlineNo
                                                        ,Mem_AwardsRecognition
                                                        ,Mem_PRCLicense
                                                        ,Mem_Contact1
                                                        ,Mem_IsCompanyAnniversary
                                                        ,Mem_NickName
                                                        ,Mem_AltAcctCode
                                                        ,Mem_WifeClaim
                                                        ,Mem_ShoesSize
                                                        ,Mem_ShirtSize
                                                        ,Mem_HairColor
                                                        ,Mem_EyeColor
                                                        ,Mem_DistinguishMark
                                                        ,Mem_GraduatedDate
                                                        ,Mem_Contact1
                                                        ,Mem_Contact2
                                                        ,Mem_Contact3
                                                        ,(SELECT Mcd_Name FROM M_CodeDtl WHERE Mcd_Code = Mem_JobGrade AND Mcd_CodeType = 'JOBGRADE') as Emt_JobGradeDesc
                                                        ,Mem_JobGrade 
                                                        ,(SELECT Mcd_Name FROM M_CodeDtl WHERE Mcd_Code = Mem_RankCode AND Mcd_CodeType = 'RANKLEVEL') as Mem_RankCodeDesc
                                                        ,Mem_RankCode
                                                        ,(SELECT Mcd_Name FROM M_CodeDtl WHERE Mcd_Code = Mem_PositionCategory AND Mcd_CodeType = 'POSCATGORY') as Emt_PositionCategoryDesc
                                                        ,Mem_PositionCategory
                                                        ,(SELECT Mcd_Name FROM M_CodeDtl WHERE Mcd_Code = Mem_PositionClass AND Mcd_CodeType = 'POSCLASS') as Mem_PositionClassDesc
                                                        ,Mem_PositionClass
                                                        ,Mem_OldPayrollType
                                                        ,Mem_OldSalaryDate
                                                        ,Mem_OldSalaryRate                                                        
                                                        ,Mem_PremiumGrpCode
                                                        ,AcctDtl6.Mcd_Name as Emt_PremiumGroupDesc
                                                        ,Tpg_StartDate as Mem_PremiumGrpDate
                                                        ,Mem_IsTaxExempted
                                                        ,Mem_PayClass
                                                        ,(SELECT Mcd_Name FROM M_CodeDtl WHERE Mcd_Code = Mem_PayClass AND Mcd_CodeType = 'PAYCLASS') as Emt_PayClassDesc
	                                                  FROM M_Employee
                                                      LEFT JOIN M_CodeDtl AcctDtl1 on Mem_JobTitleCode = AcctDtl1.Mcd_Code
                                                        AND AcctDtl1.Mcd_CodeType = 'POSITION'
                                                      LEFT JOIN M_CodeDtl AcctDtl2 on Mem_Gender = AcctDtl2.Mcd_Code 
                                                        AND AcctDtl2.Mcd_CodeType = 'GENDER'
                                                      LEFT JOIN M_CodeDtl AcctDtl3 on Mem_WorkStatus = AcctDtl3.Mcd_Code
                                                        AND AcctDtl3.Mcd_CodeType = 'JOBSTAT'
                                                      LEFT JOIN M_CodeDtl AcctDtl4 on Mem_EmploymentStatusCode = AcctDtl4.Mcd_Code
                                                        AND AcctDtl4.Mcd_CodeType = 'EMPLOYSTAT'
                                                      LEFT JOIN M_CodeDtl AcctDtl5 on Mem_CalendarType = AcctDtl5.Mcd_Code
                                                        AND AcctDtl5.Mcd_CodeType = 'WORKTYPE'
                                                      LEFT JOIN M_CodeDtl AcctDtl6 on Mem_PremiumGrpCode = AcctDtl6.Mcd_Code
                                                        AND AcctDtl6.Mcd_CodeType = 'PREMGRP'
                                                      LEFT JOIN T_EmpPremiumGroup ON Tpg_IDNo = Mem_IDNo
                                                           AND Tpg_PremiumGroup = Mem_PremiumGrpCode   
                                                      LEFT JOIN M_CostCenter ON Mem_CostcenterCode = Mcc_CostCenterCode
                                                      LEFT JOIN M_Division ON Mcc_DivCode = Mdv_DivCode
                                                      LEFT JOIN M_Department ON Mcc_DptCode = Mdp_DptCode
                                                      LEFT JOIN M_Section ON Mcc_SecCode = Msc_SecCode
                                                      LEFT JOIN M_SubSection ON Mcc_SubsecCode = Msb_SubSecCode
                                                      LEFT JOIN M_Process ON Mcc_PrcCode = Mpr_PrcCode
                                                      Where Mem_IDNo = @Mem_IDNo";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

       
        public DataRow FetchAcctDescInAccountDetail(string Mcd_Code, string Mcd_CodeType)
        {
            if (Mcd_Code != "")
            {
                DataSet ds = new DataSet();

                using (DALHelper dal = new DALHelper())
                {
                    ParameterInfo[] paramCollection = new ParameterInfo[2];
                    paramCollection[0] = new ParameterInfo("@Mcd_Code", Mcd_Code);
                    paramCollection[1] = new ParameterInfo("@Mcd_CodeType", Mcd_CodeType);

                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(@"SELECT CASE WHEN @Mcd_Code = 'REG' AND @Mcd_CodeType = 'WORKTYPE'
	                                                THEN  'REG SHIFT'
	                                                ELSE 
	                                                (
		                                                SELECT Mcd_Name 
		                                                FROM M_CodeDtl 
		                                                WHERE Mcd_CodeType=@Mcd_CodeType AND Mcd_Code=@Mcd_Code
	                                                )
                                                END", CommandType.Text, paramCollection); 
                    
                    dal.CloseDB();
                }

                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows[0];
                else
                    return null;
            }
            else
                return null;
        }

        public DataRow FetchCostCenterName(string Mem_CostcenterCode)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@Mem_CostcenterCode", Mem_CostcenterCode);

                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.FetchCostCenterName, CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public double GetSecurityNumberLength(string SecNumType, string CompanyCode, string CentralProfile)
        {
            string qString = string.Format(@"SELECT Mpd_ParamValue 
                                             FROM M_PolicyDtl
                                             WHERE Mpd_PolicyCode = 'SECNUMLEN'
                                                AND Mpd_SubCode = '{0}'
                                                AND Mpd_CompanyCode = '{1}'", SecNumType, CompanyCode);

            DataTable dtResult;
            double dLength = -1;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(qString).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0)
            {
                dLength = Convert.ToDouble(dtResult.Rows[0][0]);
            }
            return dLength;
        }

       
        private DataTable GetOrigValuesInDB(string condition, string tablename)
        {
            DataSet ds = new DataSet();
            string[] param = new string[2];
            param[0] = tablename;
            param[1] = condition;

            string qString = string.Format("SELECT * FROM {0} {1}", param);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds.Tables[0];
        }

        #endregion

        #region <Functions Added>

        public DataSet GetDataForID()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Mcm_ProcessYear
		                                            ,Mcm_IDNoIssued
                                            From M_Company";

            using (DALHelper dal = new DALHelper("NON-CONFI"))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }
        
        public int CreateEmpPremiumGroupRec(string Tpg_IDNo, DateTime Tpg_StartDate, string Tpg_PremiumGroup, string Usr_Login, DALHelper dalUp)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tpg_IDNo", Tpg_IDNo);
            paramInfo[1] = new ParameterInfo("@Tpg_StartDate", Tpg_StartDate);
            paramInfo[2] = new ParameterInfo("@Tpg_PremiumGroup", Tpg_PremiumGroup);
            paramInfo[3] = new ParameterInfo("@Usr_Login", Usr_Login);

            string sqlQuery = @"INSERT INTO T_EmpPremiumGroup
                                               (Tpg_IDNo
                                               ,Tpg_StartDate
                                               ,Tpg_PremiumGroup
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tpg_IDNo
                                               ,@Tpg_StartDate
                                               ,@Tpg_PremiumGroup
                                               ,@Usr_Login
                                               ,GetDate())";

            retVal = dalUp.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);

            return retVal;
        }
        public int UpdateEmpPremiumGroupRec(string Tpg_IDNo, DateTime Tpg_StartDate, string Tpg_PremiumGroup, string Usr_Login)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tpg_IDNo", Tpg_IDNo);
            paramInfo[1] = new ParameterInfo("@Tpg_StartDate", Tpg_StartDate);
            paramInfo[2] = new ParameterInfo("@Tpg_PremiumGroup", Tpg_PremiumGroup);
            paramInfo[3] = new ParameterInfo("@Usr_Login", Usr_Login);

            string sqlQuery = @"UPDATE T_EmpPremiumGroup
                                               SET
                                               Tpg_StartDate = @Tpg_StartDate
                                               ,Tpg_PremiumGroup = @Tpg_PremiumGroup
                                               ,Usr_Login = @Usr_Login
                                               ,Ludatetime = GETDATE()
                                                WHERE Tpg_IDNo = @Tpg_IDNo";


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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public void UpdateIDNumberAutoGenSeries(string userlogin)
        {
            ParameterInfo[] UpdateparamInfo = new ParameterInfo[1];
            UpdateparamInfo[0] = new ParameterInfo("@Usr_Login", userlogin);

            string UpdateString = @"UPDATE T_DocumentNumber
                                   SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
                                      ,Usr_Login = @Usr_Login
	                                  ,ludatetime = GetDate()
                                      where Tdn_DocumentCode = 'IDSERIES'";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    dal.ExecuteNonQuery(UpdateString, CommandType.Text,UpdateparamInfo);
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
        public void UpdateIDNumberAutoGenSeries(string userlogin, string transactionCode, DALHelper dal)
        {
            string UpdateString = string.Format(@"UPDATE T_DocumentNumber
                                   SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
                                      ,Usr_Login = '{0}'
	                                  ,ludatetime = GetDate()
                                      where Tdn_DocumentCode = '{1}'", userlogin, transactionCode);

            dal.ExecuteNonQuery(UpdateString, CommandType.Text);
        }

        public DataSet GetDataIDEmployeeType()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT Mcd_Code,Mcd_Name FROM M_CodeDtl
                                WHERE Mcd_CodeType = 'IDEMPTYPE'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }
        public string GetDataIDEmployeeTypeDesc(string Mcd_Code)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] IDEmpTypeParamInfo = new ParameterInfo[1];
            IDEmpTypeParamInfo[0] = new ParameterInfo("@Mcd_Code", Mcd_Code);

            string sqlQuery = @"SELECT Mcd_Name FROM M_CodeDtl
                                WHERE Mcd_CodeType = 'IDEMPTYPE'
                                AND Mcd_Code = @Mcd_Code";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, IDEmpTypeParamInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public DataSet GetDataForID_INDIRECTEMP()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT Tdn_DocumentPrefix, Tdn_LastSeriesNumber 
                                    FROM T_DocumentNumber
                                    WHERE Tdn_DocumentCode = 'IDSERIES'";

            using (DALHelper dal = new DALHelper("NON-CONFI"))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int CreateEmpLeaveRec(string Tll_IDNo, string Usr_Login, DALHelper dalUp)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tll_IDNo", Tll_IDNo);
            paramInfo[1] = new ParameterInfo("@Usr_Login", Usr_Login);

            string sqlQuery = @"INSERT INTO T_EmployeeLeaveMaster
                                               (Tll_IDNo
                                               ,Elm_VLBalance
                                               ,Elm_SLBalance
                                               ,Elm_ELBalance
                                               ,Elm_PLBalance
                                               ,Elm_BLBalance
                                               ,Elm_DLBalance
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tll_IDNo
                                               ,'0.00'
                                               ,'0'
                                               ,'0'
                                               ,'0'
                                               ,'0'
                                               ,'0'
                                               ,@Usr_Login
                                               ,GetDate())";

            retVal = dalUp.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            
            return retVal;
        }

        public int CreateEmpLogMasterRecord(DataRow row, int ImageSize)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[11];
            paramInfo[0] = new ParameterInfo("@Lmt_EmployeeID", row["Lmt_EmployeeID"]);
            paramInfo[1] = new ParameterInfo("@Lmt_Lastname", row["Lmt_Lastname"]);
            paramInfo[2] = new ParameterInfo("@Lmt_Firstname", row["Lmt_Firstname"]);
            paramInfo[3] = new ParameterInfo("@Lmt_Middlename", row["Lmt_Middlename"]);
            paramInfo[4] = new ParameterInfo("@Lmt_CostCenterDesc", row["Lmt_CostCenterDesc"]);
            paramInfo[5] = new ParameterInfo("@Lmt_PositionDesc", row["Lmt_PositionDesc"]);
            paramInfo[6] = new ParameterInfo("@Lmt_Gender", row["Lmt_Gender"]);
            paramInfo[7] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[8] = new ParameterInfo("@Lmt_Nickname", row["Lmt_Nickname"]);
            paramInfo[9] = new ParameterInfo("@Lmt_Picture", row["Lmt_Picture"], SqlDbType.Image, ImageSize);
            paramInfo[10] = new ParameterInfo("@Lmt_BirthDate", row["Lmt_BirthDate"]);

            string sqlQuery = @"INSERT INTO T_LogMaster
                                                           (Lmt_EmployeeID
                                                           ,Lmt_BarcodeID
                                                           ,Lmt_Lastname
                                                           ,Lmt_Firstname
                                                           ,Lmt_Middlename
                                                           ,Lmt_Nickname --added by kevin 08072009
                                                           ,Lmt_CostCenterDesc
                                                           ,Lmt_PositionDesc
                                                           ,Lmt_Gender
                                                           ,Lmt_LastLogType 
		                                                   ,Lmt_IDPrintCtr 
		                                                   ,Lmt_IDPrintedBy 
		                                                   ,Lmt_ID_PrintedDate 
                                                           ,Lmt_Status
                                                           ,Usr_Login
                                                           ,LudateTime
                                                            ,Lmt_SwipeMailAlert
                                                            ,Lmt_SwipeMailAlertRem
                                                            ,Lmt_SwipeMailAlertBy
                                                            ,Lmt_SwipeMailAlertDate
                                                            ,Lmt_Hold
                                                            ,Lmt_HolRem
                                                            ,Lmt_HoldBy
                                                            ,Lmt_HoldDate
                                                            ,Lmt_Picture
                                                            ,Lmt_BirthDate)
                                                     VALUES
                                                           (@Lmt_EmployeeID
                                                           ,@Lmt_EmployeeID
                                                           ,@Lmt_Lastname
                                                           ,@Lmt_Firstname
                                                           ,@Lmt_Middlename
                                                           ,@Lmt_Nickname --added by kevin 08072009
                                                           ,@Lmt_CostCenterDesc
                                                           ,@Lmt_PositionDesc
                                                           ,@Lmt_Gender
                                                           ,'O'
		                                                   ,null
                                                           ,null
	                                                       ,null
                                                           ,'A'
                                                           ,@Usr_Login
                                                           ,GetDate()
                                                            ,'False'
                                                            ,null
                                                            ,null
                                                            ,null
                                                            ,null
                                                            ,null
                                                            ,null
                                                            ,null
                                                            ,@Lmt_Picture
                                                            ,@Lmt_BirthDate)";

            using (DALHelper dal = new DALHelper(true))
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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
    }

           
            return retVal;
        }

        public DataSet GetCostCenterDesc(string Mcc_CostCenterCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mcc_CostCenterCode", Mcc_CostCenterCode);

            string sqlQuery = @"Select 
                                                Case when Len(Rtrim(IsNull(t2.Mdv_DivName,' '))) > 0 THEN Rtrim(IsNull(t2.Mdv_DivName,' '))
                                                    Else ' ' End + 
                                                    Case when Len(Rtrim(IsNull(t3.Mdp_DptName,' '))) > 0 THEN ' / ' + Rtrim(IsNull(t3.Mdp_DptName,' '))
                                                    Else ' ' End + 
                                                    Case when Len(Rtrim(IsNull(t4.Msc_SecName,' '))) > 0 THEN ' / ' + Rtrim(IsNull(t4.Msc_SecName,' '))
                                                    Else ' ' End + 
                                                    Case when Len(Rtrim(IsNull(t5.Msb_SubSecName,' '))) > 0 THEN ' / ' + Rtrim(IsNull(t5.Msb_SubSecName,' '))
                                                    Else ' ' End  + 
                                                    Case when Len(Rtrim(IsNull(t6.Mpr_PrcName,' '))) > 0 THEN ' / ' + Rtrim(IsNull(t6.Mpr_PrcName,' '))
                                                    Else ' ' 
                                                    End 
                                                From M_CostCenter as t1
	                                                Inner Join M_Division as t2
		                                                ON t1.Mcc_DivCode = t2.Mdv_DivCode
	                                                Inner Join M_Department as t3
		                                                ON t1.Mcc_DptCode = t3.Mdp_DptCode
	                                                Inner Join M_Section as t4
		                                                ON t1.Mcc_SecCode = t4.Msc_SecCode
	                                                Inner Join M_SubSection as t5
		                                                ON t1.Mcc_SubsecCode = t5.Msb_SubSecCode
	                                                Inner Join M_Process as t6
		                                                ON t1.Mcc_PrcCode = t6.Mpr_PrcCode
                                                Where Mcc_CostCenterCode = @Mcc_CostCenterCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetPositionDesc(string Mcd_Code)
        {
            DataSet ds = new DataSet();
            string retval = string.Empty;

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mcd_Code", Mcd_Code);

            string sqlQuery = @"Select Mcd_Name
                                                From M_CodeDtl
                                                Where Mcd_CodeType = 'POSITION'
                                                And Mcd_Code = @Mcd_Code";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                retval = ds.Tables[0].Rows[0][0].ToString();
            }
            return retval;
        }

        public int CreateAuditTrailRec(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Tat_ColId", row["Tat_ColId"]);
            paramInfo[1] = new ParameterInfo("@Tat_IDNo", row["Tat_IDNo"]);
            paramInfo[2] = new ParameterInfo("@Tat_LineNo", row["Tat_LineNo"]);
            paramInfo[3] = new ParameterInfo("@Tat_OldValue", row["Tat_OldValue"]);
            paramInfo[4] = new ParameterInfo("@Tat_NewValue", row["Tat_NewValue"]);
            paramInfo[5] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            string sqlQuery = @"INSERT INTO T_AuditTrl
                                                   (Tat_ColId
                                                   ,Tat_IDNo
                                                   ,Tat_LineNo
                                                   ,Tat_OldValue
                                                   ,Tat_NewValue
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@Tat_ColId
                                                   ,@Tat_IDNo
                                                   ,@Tat_LineNo
                                                   ,@Tat_OldValue
                                                   ,@Tat_NewValue
                                                   ,@Usr_Login
                                                   ,Getdate())";
            
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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public string GetLastSeqNo(string Tat_ColId, string Tat_IDNo)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tat_ColId", Tat_ColId);
            paramInfo[1] = new ParameterInfo("@Tat_IDNo", Tat_IDNo);

            string sqlQuery = @"Select Count(Tat_ColId)
                                                From T_AuditTrl
                                                Where Tat_ColId = @Tat_ColId
                                                And Tat_IDNo = @Tat_IDNo";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds.Tables[0].Rows[0][0].ToString();
        }

        public DataSet GetCurrentStartEndCycle()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Convert(char(10), Tps_StartCycle, 101) as Tps_StartCycle
		                                            ,Convert(char(10), Tps_EndCycle, 101) as Tps_EndCycle
                                            From T_PaySchedule
                                            Where Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        //added function get
        public string GetIDPaddingForZero()
        {
            DataSet ds = new DataSet();

            string qString = @"Select Mph_NumValue From M_PolicyHdr
                                    Where Mph_PolicyCode = 'IDSERIES'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds.Tables[0].Rows[0][0].ToString();
        }

        public bool isCCTManufacturin(string Mcc_CostCenterCode)
        {
            DataSet ds = new DataSet();

            string qString = @"SELECT Mcc_ExpenseClass From M_CostCenter
                                    WHERE Mcc_RecordStatus = 'A'
                                    AND Mcc_CostCenterCode = '" +Mcc_CostCenterCode+ @"'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds != null)
            {
                if (ds.Tables[0].Rows[0][0].ToString() != string.Empty && ds.Tables[0].Rows[0][0].ToString().Substring(0, 1) == "M")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public string MapJobPosition(string Mem_RankCode)
        {
            DataSet ds = new DataSet();

            string qString = @"SELECT Mcd_Name as Mem_RankCode From M_JobTitleRank
                                    innner join M_CodeDtl on Mcd_Code = Mem_RankCode
                                    and Mcd_CodeType = 'RANKLEVEL'
                                    WHERE Mem_RankCode = '" + Mem_RankCode + @"'"; //  

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
                return string.Empty ;
        }

        public string MapClassification(string Mem_PositionClass)
        {
            DataSet ds = new DataSet();

            string qString = @"SELECT Mcd_Name as Mem_PositionClass From M_Employee
                                    innner join M_CodeDtl on Mcd_Code = Mem_PositionClass
                                    and Mcd_CodeType = 'POSCLASS'
                                    WHERE  Mem_PositionClass = '" + Mem_PositionClass + @"'";//Emt_Status = 'A' AND

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
                return string.Empty;
        }

        public string MapCategory(string Mem_PositionCategory)
        {
            DataSet ds = new DataSet();

            string qString = @"SELECT Mcd_Name as Mem_PositionCategory From M_Employee
                                    innner join M_CodeDtl on Mcd_Code = Mem_PositionCategory
                                    and Mcd_CodeType = 'POSCATGORY'
                                    WHERE  Mem_PositionCategory = '" + Mem_PositionCategory + @"'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
                return string.Empty;
        }

        public int CreateEmployeeLogLedgerRecord(DataRow row)
        {
            int retVal = 0;

            #region query
            string qString = @"INSERT INTO T_EmpTimeRegister
                                                   (Ttr_IDNo
                                                   ,Ttr_Date
                                                   ,Ttr_PayCycle
                                                   ,Ttr_DayCode
                                                   ,Ttr_ShiftCode
                                                   ,Ttr_HolidayFlag
                                                   ,Ttr_RestDayFlag
                                                   ,Ttr_ActIn_1
                                                   ,Ttr_ActOut_1
                                                   ,Ttr_ActIn_2
                                                   ,Ttr_ActOut_2
                                                   ,Ttr_ConvIn_1Min
                                                   ,Ttr_ConvOut_1Min
                                                   ,Ttr_ConvIn_2Min
                                                   ,Ttr_ConvOut_2Min
                                                   ,Ttr_CompIn_1Min
                                                   ,Ttr_CompOut_1Min
                                                   ,Ttr_CompIn_2Min
                                                   ,Ttr_CompOut_2Min
                                                   ,Ttr_ShiftIn_1Min
                                                   ,Ttr_ShiftOut_1Min
                                                   ,Ttr_ShiftIn_2Min
                                                   ,Ttr_ShiftOut_2Min
                                                   ,Ttr_ShiftMin 
                                                   ,Ttr_ScheduleType
                                                   ,Ttr_WFPayLVCode
                                                   ,Ttr_WFPayLVHr
                                                   ,Ttr_PayLVMin
                                                   ,Ttr_ExcLVMin --renamed
                                                   ,Ttr_WFNoPayLVCode
                                                   ,Ttr_WFNoPayLVHr
                                                   ,Ttr_NoPayLVMin
                                                   ,Ttr_WFOTAdvHr--renamed
				                                   ,Ttr_WFOTPostHr--added
				                                   ,Ttr_OTMin--added
                                                   ,Ttr_CompOTMin
                                                   ,Ttr_OffsetOTMin
                                                   ,Ttr_CompLT1Min
                                                   ,Ttr_LTPostFlag -- added
                                                   ,Ttr_InitialABSMin
                                                   ,Ttr_CompABSMin
                                                   ,Ttr_CompREGMin
                                                   ,Ttr_CompWorkMin
                                                   ,Ttr_CompNDMin
                                                   ,Ttr_CompNDOTMin
                                                   ,Ttr_PrvDayWorkMin
                                                   ,Ttr_PrvDayHolRef
                                                   ,Ttr_GraveyardPostFlag
                                                   ,Ttr_GrvPostBy -- added
                                                   ,Ttr_GrvPostDate -- added
                                                   ,Ttr_AssumedFlag
                                                   ,Ttr_AssumedBy -- added
                                                   ,Ttr_AssumedDate -- added
                                                   ,Ttr_PaidLEGHour -- added
                                                   ,Ttr_ForceLeaveBy -- added
                                                   ,Ttr_ForceLeaveDate -- added
                                                   ,Ttr_ForOffsetMin -- renamed
                                                   ,Ttr_ExcOffset
                                                   ,Ttr_RESTLEGHOLDay
                                                   ,Ttr_WorkDay
                                                   ,Ttr_MealDay
                                                   ,Ttr_EXPHour -- added
                                                   ,Ttr_ABSHour
                                                   ,Ttr_REGHour
                                                   ,Ttr_OTHour
                                                   ,Ttr_NDHour
                                                   ,Ttr_NDOTHour
                                                   ,Ttr_LVHour
                                                   ,Ttr_CompAdvOTMin
                                                   ,Ttr_EarnedSatOff
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@Ttr_IDNo
                                                   ,@Ttr_Date
                                                   ,@Ttr_PayCycle
                                                   ,'REG'
                                                   ,@Ttr_ShiftCode
                                                   ,@Ttr_HolidayFlag
                                                   ,'False'
                                                   ,'0000'
                                                   ,'0000'
                                                   ,'0000'
                                                   ,'0000'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,@Ttr_ShiftIn_1Min
                                                   ,@Ttr_ShiftOut_1Min
                                                   ,@Ttr_ShiftIn_2Min
                                                   ,@Ttr_ShiftOut_2Min
                                                   ,@Ttr_ShiftMin
                                                   ,@Ttr_ScheduleType
                                                   ,' '
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,' '
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
				                                   ,'0'
				                                   ,0
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'False'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,null
                                                   ,'False'
                                                   ,null
                                                   ,null
                                                   ,'False'
                                                   ,null
                                                   ,null
                                                   ,'0.00'
                                                   ,null
                                                   ,null
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,0
                                                   ,0
                                                   ,@Usr_Login
                                                   ,GetDate())";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);
            paramInfo[2] = new ParameterInfo("@Ttr_PayCycle", row["Ttr_PayCycle"]);
            paramInfo[3] = new ParameterInfo("@Ttr_ShiftCode", row["Ttr_ShiftCode"]);
            paramInfo[4] = new ParameterInfo("@Ttr_HolidayFlag", row["Ttr_HolidayFlag"]);
            paramInfo[5] = new ParameterInfo("@Ttr_ShiftIn_1Min", row["Ttr_ShiftIn_1Min"]);
            paramInfo[6] = new ParameterInfo("@Ttr_ShiftOut_1Min", row["Ttr_ShiftOut_1Min"]);
            paramInfo[7] = new ParameterInfo("@Ttr_ShiftIn_2Min", row["Ttr_ShiftIn_2Min"]);
            paramInfo[8] = new ParameterInfo("@Ttr_ShiftOut_2Min", row["Ttr_ShiftOut_2Min"]);
            paramInfo[9] = new ParameterInfo("@Ttr_ShiftMin", row["Ttr_ShiftMin"]);
            paramInfo[10] = new ParameterInfo("@Ttr_ScheduleType", row["Ttr_ScheduleType"]);
            paramInfo[11] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public bool CheckifHoliday(string HolidayDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@HolidayDate", HolidayDate);

            string sqlQuery = @"Select Convert(char(10), Thl_HolidayDate, 101) as Thl_HolidayDate
                                                        From T_Holiday
                                                        Where Thl_HolidayDate = @HolidayDate";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataSet GetShiftCodeValues(string Msh_ShiftCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", Msh_ShiftCode);

            string sqlQuery = @"Select Msh_ShiftIn1
		                                            ,Msh_ShiftOut1
		                                            ,Msh_ShiftIn2
		                                            ,Msh_ShiftOut2
		                                            ,Msh_ShiftHours * 60 as Msh_ShiftHours
		                                            ,Msh_Schedule
                                            From M_Shift
                                            Where Msh_ShiftCode = @Msh_ShiftCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public int UpdateEmployeeLogLedgerRecord(DataRow row, string query)
        {
            int retVal = 0;
            #region query
            string qString = @"UPDATE T_EmpTimeRegister
                               SET Ttr_DayCode =
                                   CASE 
                                    WHEN Ttr_HolidayFlag = 'True'
                                    Then Ttr_DayCode
                                    Else @Ttr_DayCode
                                   END 
                                  ,Ttr_RestDayFlag = @Ttr_RestDayFlag
                                  ,Usr_Login = @Usr_Login
                                  ,Ludatetime = Getdate()
                             WHERE Ttr_IDNo = @Ttr_IDNo
                              And Ttr_Date = @Ttr_Date";

            if (query != string.Empty)
                qString = qString + query;
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Ttr_DayCode", row["Ttr_DayCode"]);
            paramInfo[1] = new ParameterInfo("@Ttr_RestDayFlag", row["Ttr_RestDayFlag"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[3] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[4] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    //CommonConstants.StoredProcedures.spEmployeeMasterUpdateEmployeeLogLedgerRecord, CommandType.StoredProcedure, paramInfo);
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
            return retVal;
        }

        public string GetCurrentPayPeriod()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Tps_PayCycle From T_PaySchedule
                                                Where Tps_CycleIndicator = 'C'
                                                And Tps_RecordStatus = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            return ds.Tables[0].Rows[0][0].ToString();
        }

        public int UpdateLogMasterRecord(DataRow row, int ImageSize)
        {
            int retVal = 0;
            #region query
            string qString = @"UPDATE T_LogMaster
                               SET Lmt_Lastname = @Lmt_Lastname
                                  ,Lmt_Firstname = @Lmt_Firstname
                                  ,Lmt_Middlename = @Lmt_Middlename
                                  ,Lmt_Nickname = @Lmt_Nickname --added by kevin 08072009
                                  ,Lmt_CostCenterDesc = @Lmt_CostCenterDesc
                                  ,Lmt_PositionDesc = @Lmt_PositionDesc
                                  ,Lmt_Gender = @Lmt_Gender
                                  ,Lmt_Status = @Lmt_Status
                                  ,Usr_Login = @Usr_Login
                                  ,LudateTime = Getdate()
                                  ,Lmt_Picture = @Lmt_Picture
                                  ,Lmt_BirthDate = @Lmt_BirthDate
                             WHERE Lmt_EmployeeID = @Lmt_EmployeeID";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Lmt_Lastname", row["Lmt_Lastname"]);
            paramInfo[1] = new ParameterInfo("@Lmt_Firstname", row["Lmt_Firstname"]);
            paramInfo[2] = new ParameterInfo("@Lmt_Middlename", row["Lmt_Middlename"]);
            paramInfo[3] = new ParameterInfo("@Lmt_CostCenterDesc", row["Lmt_CostCenterDesc"]);
            paramInfo[4] = new ParameterInfo("@Lmt_PositionDesc", row["Lmt_PositionDesc"]);
            paramInfo[5] = new ParameterInfo("@Lmt_Gender", row["Lmt_Gender"]);
            paramInfo[6] = new ParameterInfo("@Lmt_Status", row["Lmt_Status"]);
            paramInfo[7] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[8] = new ParameterInfo("@Lmt_EmployeeID", row["Lmt_EmployeeID"]);
            paramInfo[9] = new ParameterInfo("@Lmt_Nickname", row["Lmt_Nickname"]);
            paramInfo[10] = new ParameterInfo("@Lmt_Picture", row["Lmt_Picture"], SqlDbType.Image, ImageSize);
            paramInfo[11] = new ParameterInfo("@Lmt_BirthDate", row["Lmt_BirthDate"]);

            using (DALHelper dal = new DALHelper(true))
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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public bool CheckifRecordExistInEmployeeLogLedger(string Ttr_IDNo)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select * From T_EmpTimeRegister
                                Where Ttr_IDNo = @Ttr_IDNo";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", Ttr_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataSet GetDMaxRate()
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Mph_NumValue From M_PolicyHdr
                                    Where Mph_PolicyCode = 'DMAXRATE'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetUserGroup(string Mud_UserCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Mud_UserRoleCode
                                From M_UserDtl
                                Where Mud_UserCode = @Mud_UserCode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mud_UserCode", Mud_UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckifHasSupRights(string Mra_UserRoleCode, string Mra_ModuleCode)
        {
            bool retval = false;
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Ugt_CanViewRate
                                From M_UserRoleAccess
                                Where Mra_UserRoleCode = @Mra_UserRoleCode
                                And Mra_ModuleCode = @Mra_ModuleCode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mra_UserRoleCode", Mra_UserRoleCode);
            paramInfo[1] = new ParameterInfo("@Mra_ModuleCode", Mra_ModuleCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString()) == true)
                    retval = true;
                else
                    retval = false;
            }
            return retval;
        }

        public string GetLatestEmploymentDate()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("Select MAX(Mem_IntakeDate) From M_Employee", CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public DataSet GetDefaultWrkAssign()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(@"Select Mcm_CompanyAddress3
		                                        , Mcd_Name
                                        From M_Company Inner Join M_CodeDtl
	                                        on Mcm_CompanyAddress3 = Mcd_Code
	                                        And  SUBSTRING(Mcd_CodeType,1,7) = 'ZIPCODE'", CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }

        #endregion

        #region <For Checking>

        public bool ExistsInLeaveMaster(string Tll_IDNo, DALHelper dalUp)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Tll_IDNo From T_EmployeeLeaveMaster
                                Where Tll_IDNo=@Tll_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tll_IDNo", Tll_IDNo);

            ds = dalUp.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool ExistsInLogMaster(string Lmt_EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Lmt_EmployeeID From T_LogMaster
                                Where Lmt_EmployeeID=@Lmt_EmployeeID";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Lmt_EmployeeID", Lmt_EmployeeID);

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool ExistsInLogLedger(string Ttr_IDNo, string Ttr_Date, DALHelper dalUp)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Ttr_IDNo From T_EmpTimeRegister
                                Where Ttr_IDNo = @Ttr_IDNo
	                                And Ttr_Date = @Ttr_Date";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", Ttr_IDNo);
            paramInfo[1] = new ParameterInfo("@Ttr_Date", Ttr_Date);

            ds = dalUp.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        #endregion

        #region <For Insertion>

        public int CreateEmployeeLogLedgerRecordInsertion(DataRow row, DALHelper dalUp)
        {
            int retVal = 0;

            #region query
            string qString = @"INSERT INTO T_EmpTimeRegister
                                                   (Ttr_IDNo
                                                   ,Ttr_Date
                                                   ,Ttr_PayCycle
                                                   ,Ttr_DayCode
                                                   ,Ttr_ShiftCode
                                                   ,Ttr_HolidayFlag
                                                   ,Ttr_RestDayFlag
                                                   ,Ttr_ActIn_1
                                                   ,Ttr_ActOut_1
                                                   ,Ttr_ActIn_2
                                                   ,Ttr_ActOut_2
                                                   ,Ttr_ConvIn_1Min
                                                   ,Ttr_ConvOut_1Min
                                                   ,Ttr_ConvIn_2Min
                                                   ,Ttr_ConvOut_2Min
                                                   ,Ttr_CompIn_1Min
                                                   ,Ttr_CompOut_1Min
                                                   ,Ttr_CompIn_2Min
                                                   ,Ttr_CompOut_2Min
                                                   ,Ttr_ShiftIn_1Min
                                                   ,Ttr_ShiftOut_1Min
                                                   ,Ttr_ShiftIn_2Min
                                                   ,Ttr_ShiftOut_2Min
                                                   ,Ttr_ShiftMin 
                                                   ,Ttr_ScheduleType
                                                   ,Ttr_WFPayLVCode
                                                   ,Ttr_WFPayLVHr
                                                   ,Ttr_PayLVMin
                                                   ,Ttr_ExcLVMin --renamed
                                                   ,Ttr_WFNoPayLVCode
                                                   ,Ttr_WFNoPayLVHr
                                                   ,Ttr_NoPayLVMin
                                                   ,Ttr_WFOTAdvHr--renamed
				                                   ,Ttr_WFOTPostHr--added
				                                   ,Ttr_OTMin--added
                                                   ,Ttr_CompOTMin
                                                   ,Ttr_OffsetOTMin
                                                   ,Ttr_CompLT1Min
                                                   ,Ttr_LTPostFlag -- added
                                                   ,Ttr_InitialABSMin
                                                   ,Ttr_CompABSMin
                                                   ,Ttr_CompREGMin
                                                   ,Ttr_CompWorkMin
                                                   ,Ttr_CompNDMin
                                                   ,Ttr_CompNDOTMin
                                                   ,Ttr_PrvDayWorkMin
                                                   ,Ttr_PrvDayHolRef
                                                   ,Ttr_GraveyardPostFlag
                                                   ,Ttr_GrvPostBy -- added
                                                   ,Ttr_GrvPostDate -- added
                                                   ,Ttr_AssumedFlag
                                                   ,Ttr_AssumedBy -- added
                                                   ,Ttr_AssumedDate -- added
                                                   ,Ttr_PaidLEGHour -- added
                                                   ,Ttr_ForceLeaveBy -- added
                                                   ,Ttr_ForceLeaveDate -- added
                                                   ,Ttr_ForOffsetMin -- renamed
                                                   ,Ttr_ExcOffset
                                                   ,Ttr_RESTLEGHOLDay
                                                   ,Ttr_WorkDay
                                                   ,Ttr_MealDay
                                                   ,Ttr_EXPHour -- added
                                                   ,Ttr_ABSHour
                                                   ,Ttr_REGHour
                                                   ,Ttr_OTHour
                                                   ,Ttr_NDHour
                                                   ,Ttr_NDOTHour
                                                   ,Ttr_LVHour
                                                   ,Ttr_CompAdvOTMin
                                                   ,Ttr_EarnedSatOff
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@Ttr_IDNo
                                                   ,@Ttr_Date
                                                   ,@Ttr_PayCycle
                                                   ,'REG'
                                                   ,@Ttr_ShiftCode
                                                   ,@Ttr_HolidayFlag
                                                   ,'False'
                                                   ,'0000'
                                                   ,'0000'
                                                   ,'0000'
                                                   ,'0000'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,@Ttr_ShiftIn_1Min
                                                   ,@Ttr_ShiftOut_1Min
                                                   ,@Ttr_ShiftIn_2Min
                                                   ,@Ttr_ShiftOut_2Min
                                                   ,@Ttr_ShiftMin
                                                   ,@Ttr_ScheduleType
                                                   ,' '
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,' '
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
				                                   ,'0'
				                                   ,0
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'False'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,null
                                                   ,'False'
                                                   ,null
                                                   ,null
                                                   ,'False'
                                                   ,null
                                                   ,null
                                                   ,'0.00'
                                                   ,null
                                                   ,null
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,'0'
                                                   ,0
                                                   ,0
                                                   ,@Usr_Login
                                                   ,GetDate())";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[12];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);
            paramInfo[2] = new ParameterInfo("@Ttr_PayCycle", row["Ttr_PayCycle"]);
            paramInfo[3] = new ParameterInfo("@Ttr_ShiftCode", row["Ttr_ShiftCode"]);
            paramInfo[4] = new ParameterInfo("@Ttr_HolidayFlag", row["Ttr_HolidayFlag"]);
            paramInfo[5] = new ParameterInfo("@Ttr_ShiftIn_1Min", row["Ttr_ShiftIn_1Min"]);
            paramInfo[6] = new ParameterInfo("@Ttr_ShiftOut_1Min", row["Ttr_ShiftOut_1Min"]);
            paramInfo[7] = new ParameterInfo("@Ttr_ShiftIn_2Min", row["Ttr_ShiftIn_2Min"]);
            paramInfo[8] = new ParameterInfo("@Ttr_ShiftOut_2Min", row["Ttr_ShiftOut_2Min"]);
            paramInfo[9] = new ParameterInfo("@Ttr_ShiftMin", row["Ttr_ShiftMin"]);
            paramInfo[10] = new ParameterInfo("@Ttr_ScheduleType", row["Ttr_ScheduleType"]);
            paramInfo[11] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            retVal = dalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

            return retVal;
        }

        private void CreateEmployeeLogLedgerRecord(string idnumber, string shiftcode, DALHelper dalUp)
        {
            DataSet tempds;
            DataSet shiftcodeds;
            DataRow dr = DbRecord.Generate(CommonConstants.TableName.T_EmpTimeRegister);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", shiftcode);

            string sqlQuery = @"Select Convert(char(10), Tps_StartCycle, 101) as Tps_StartCycle
		                                            ,Convert(char(10), Tps_EndCycle, 101) as Tps_EndCycle
                                            From T_PaySchedule
                                            Where Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'";

            tempds = dalUp.ExecuteDataSet(sqlQuery, CommandType.Text);

            sqlQuery = @"Select Msh_ShiftIn1
		                                            ,Msh_ShiftOut1
		                                            ,Msh_ShiftIn2
		                                            ,Msh_ShiftOut2
		                                            ,Msh_ShiftHours * 60 as Msh_ShiftHours
		                                            ,Msh_Schedule
                                            From M_Shift
                                            Where Msh_ShiftCode = @Msh_ShiftCode";

            shiftcodeds = dalUp.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

            if (tempds.Tables[0].Rows.Count > 0)
            {
                string strtdate = tempds.Tables[0].Rows[0][0].ToString();
                string enddate = tempds.Tables[0].Rows[0][1].ToString();
                string shiftmin = string.Empty;

                while (Convert.ToDateTime(strtdate) <= Convert.ToDateTime(enddate))
                {
                    dr["Ttr_IDNo"] = idnumber;
                    dr["Ttr_Date"] = strtdate;
                    dr["Ttr_PayCycle"] = this.GetCurrentPayPeriodForAddition(dalUp);
                    dr["Ttr_ShiftCode"] = shiftcode;
                    dr["Ttr_HolidayFlag"] = this.CheckifHolidayforAddition(strtdate, dalUp).ToString();
                    if (shiftcodeds.Tables[0].Rows.Count > 0)
                    {
                        dr["Ttr_ShiftIn_1Min"] = shiftcodeds.Tables[0].Rows[0][0].ToString();
                        dr["Ttr_ShiftOut_1Min"] = shiftcodeds.Tables[0].Rows[0][1].ToString();
                        dr["Ttr_ShiftIn_2Min"] = shiftcodeds.Tables[0].Rows[0][2].ToString();
                        dr["Ttr_ShiftOut_2Min"] = shiftcodeds.Tables[0].Rows[0][3].ToString();
                        shiftmin = shiftcodeds.Tables[0].Rows[0][4].ToString();
                        dr["Ttr_ShiftMin"] = shiftmin.Remove(shiftmin.Length - 3);
                        dr["Ttr_ScheduleType"] = shiftcodeds.Tables[0].Rows[0][5].ToString();
                    }
                    else
                    {
                        dr["Ttr_ShiftIn_1Min"] = "0";
                        dr["Ttr_ShiftOut_1Min"] = "0";
                        dr["Ttr_ShiftIn_2Min"] = "0";
                        dr["Ttr_ShiftOut_2Min"] = "0";
                        dr["Ttr_ShiftMin"] = "0";
                        dr["Ttr_ScheduleType"] = "0";
                    }
                    dr["Usr_Login"] = LoginInfo.getUser().UserCode;
                    if (!this.ExistsInLogLedger(idnumber, strtdate, dalUp))
                        this.CreateEmployeeLogLedgerRecordInsertion(dr, dalUp);
                    strtdate = Convert.ToString(Convert.ToDateTime(strtdate).AddDays(1).ToString("MM/dd/yyyy"));
                }
            }
        }

        public int Add(DataRow row, DALHelper dalUp, int ImageSize)
        {
            int retVal = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            #region query string

            string qString = @"INSERT INTO M_Employee
                                                (Mem_IDNo,
                                                Mem_LastName,
                                                Mem_FirstName,
                                                Mem_MiddleName,
                                                Mem_MaidenName,
                                                Mem_BirthDate,
                                                Mem_BirthPlace,
                                                Mem_Age,
                                                Mem_Gender,
                                                Mem_CivilStatusCode,
                                                Mem_MarriedDate,
                                                Mem_NationalityCode,
                                                Mem_ReligionCode,
                                                Mem_PresCompleteAddress,
                                                Mem_PresAddressBarangay,
                                                Mem_PresAddressMunicipalityCity,
                                                Mem_LandlineNo,
                                                Mem_CellNo,
                                                Mem_OfficeEmailAddress,
                                                Mem_PersonalEmail,
                                                Mem_EducationCode,
                                                Mem_SchoolCode,
                                                Mem_CourseCode,
                                                Mem_BloodType,
                                                Mem_SSSNo,
                                                Mem_PhilhealthNo,
                                                Mem_PayrollBankCode,
                                                Mem_BankAcctNo,
                                                Mem_TIN,
                                                Mem_TaxCode,
                                                Mem_PagIbigNo,
                                                Mem_PagIbigRule,
                                                Mem_PagIbigShare,
                                                Mem_ICEContactPerson,
                                                Mem_ICERelation,
                                                Mem_ICECompleteAddress,
                                                Mem_ICEAddressBarangay,
                                                Mem_ICEAddressMunicipalityCity,
                                                Mem_ICEContactNo,
                                                Mem_CostcenterDate,
                                                Mem_CostcenterCode,
                                                Mem_EmploymentStatusCode,
                                                Mem_IntakeDate,
                                                Mem_RegularDate,
                                                Mem_WorkLocationCode,
                                                Mem_JobTitleCode,
                                                Mem_PositionDate,
                                                Mem_Superior1,
                                                Mem_Superior2,
                                                Mem_IsComputedPayroll,
                                                Mem_PaymentMode,
                                                Mem_PayrollType,
                                                Mem_SalaryDate,
                                                Mem_Salary,
                                                Mem_WorkStatus,
                                                Mem_IsConfidential,
                                                Mem_ShiftCode,
                                                Mem_CalendarType,
                                                Mem_CalendarGroup,
                                                Mem_SeparationNoticeDate,
                                                Mem_SeparationCode,
                                                Mem_SeparationDate,
                                                Mem_ClearedDate
                                                ,Mem_SSSRule 
                                                ,Mem_SSSShare 
                                                ,Mem_PHRule 
                                                ,Mem_PHShare 
                                                ,Mem_Image
                                                ,Mem_ProbationDate
                                                ,Mem_Height
                                                ,Mem_Weight
                                                ,Mem_ProvCompleteAddress
                                                ,Mem_ProvAddressBarangay
                                                ,Mem_ProvAddressMunicipalityCity
                                                ,Mem_ProvLandlineNo
                                                ,Mem_AwardsRecognition
                                                ,Mem_PRCLicense
                                                ,Mem_ExpenseClass
                                                ,Usr_Login
                                                ,Ludatetime
                                                ,Mem_NickName
                                                ,Emt_OldEmployeeID
                                                ,Mem_WifeClaim
                                                ,Mem_ShoesSize
                                                ,Mem_ShirtSize
                                                ,Mem_HairColor
                                                ,Mem_EyeColor
                                                ,Mem_DistinguishMark
                                                ,Mem_JobGrade
                                                ,Mem_RankCode
                                                ,Mem_PositionCategory
                                                ,Mem_PositionClass
                                                ,Mem_PremiumGrpCode
                                                ,Mem_GraduatedDate
                                                ,Mem_Contact1
                                                ,Mem_Contact2
                                                ,Mem_Contact3
                                                ,Mem_OldPayrollType
                                                ,Mem_OldSalaryDate
                                                ,Mem_OldSalaryRate
                                                ,Mem_IsTaxExempted
                                                ,Mem_PayClass
                                                ,Mem_IsCompanyAnniversary
                                                ,Mem_PremiumGrpCodeEffectivity)
                                               VALUES
                                                (@Mem_IDNo,
                                                @Mem_LastName,
                                                @Mem_FirstName,
                                                @Mem_MiddleName,
                                                @Mem_MaidenName,
                                                @Mem_BirthDate,
                                                @Mem_BirthPlace,
                                                @Mem_Age,
                                                @Mem_Gender,
                                                @Mem_CivilStatusCode,
                                                @Mem_MarriedDate,
                                                @Mem_NationalityCode,
                                                @Mem_ReligionCode,
                                                @Mem_PresCompleteAddress,
                                                @Mem_PresAddressBarangay,
                                                @Mem_PresAddressMunicipalityCity,
                                                @Mem_LandlineNo,
                                                @Mem_CellNo,
                                                @Mem_OfficeEmailAddress,
                                                @Mem_PersonalEmail,
                                                @Mem_EducationCode,
                                                @Mem_SchoolCode,
                                                @Mem_CourseCode,
                                                @Mem_BloodType,
                                                @Mem_SSSNo,
                                                @Mem_PhilhealthNo,
                                                @Mem_PayrollBankCode,
                                                @Mem_BankAcctNo,
                                                @Mem_TIN,
                                                @Mem_TaxCode,
                                                @Mem_PagIbigNo,
                                                @Mem_PagIbigRule,
                                                @Mem_PagIbigShare,
                                                @Mem_ICEContactPerson,
                                                @Mem_ICERelation,
                                                @Mem_ICECompleteAddress,
                                                @Mem_ICEAddressBarangay,
                                                @Mem_ICEAddressMunicipalityCity,
                                                @Mem_ICEContactNo,
                                                @Mem_CostcenterDate,
                                                @Mem_CostcenterCode,
                                                @Mem_EmploymentStatusCode,
                                                @Mem_IntakeDate,
                                                @Mem_RegularDate,
                                                @Mem_WorkLocationCode,
                                                @Mem_JobTitleCode,
                                                @Mem_PositionDate,
                                                @Mem_Superior1,
                                                @Mem_Superior2,
                                                @Mem_IsComputedPayroll,
                                                @Mem_PaymentMode,
                                                @Mem_PayrollType,
                                                @Mem_SalaryDate,
                                                @Mem_Salary,
                                                @Mem_WorkStatus,
                                                @Mem_IsConfidential,
                                                @Mem_ShiftCode,
                                                @Mem_CalendarType,
                                                @Mem_CalendarGroup,
                                                @Mem_SeparationNoticeDate,
                                                @Mem_SeparationCode,
                                                @Mem_SeparationDate,
                                                @Mem_ClearedDate
                                                ,@Mem_SSSRule 
                                                ,@Mem_SSSShare 
                                                ,@Mem_PHRule 
                                                ,@Mem_PHShare 
                                                ,@Mem_Image
                                                ,@Mem_ProbationDate
                                                ,@Mem_Height
                                                ,@Mem_Weight
                                                ,@Mem_ProvCompleteAddress
                                                ,@Mem_ProvAddressBarangay
                                                ,@Mem_ProvAddressMunicipalityCity
                                                ,@Mem_ProvLandlineNo
                                                ,@Mem_AwardsRecognition
                                                ,@Mem_PRCLicense
                                                ,@Mem_ExpenseClass
                                                ,@Usr_Login
                                                ,GetDate()
                                                ,@Mem_NickName
                                                ,@Emt_OldEmployeeID
                                                ,@Mem_WifeClaim
                                                ,@Mem_ShoesSize
                                                ,@Mem_ShirtSize
                                                ,@Mem_HairColor
                                                ,@Mem_EyeColor
                                                ,@Mem_DistinguishMark
                                                ,@Mem_JobGrade
                                                ,@Mem_RankCode
                                                ,@Mem_PositionCategory
                                                ,@Mem_PositionClass
                                                ,@Mem_PremiumGrpCode
                                                ,@Mem_GraduatedDate
                                                ,@Mem_Contact1
                                                ,@Mem_Contact2
                                                ,@Mem_Contact3
                                                ,@Mem_OldPayrollType
                                                ,@Mem_OldSalaryDate
                                                ,@Mem_OldSalaryRate
                                                ,@Mem_IsTaxExempted
                                                ,@Mem_PayClass
                                                ,@Mem_IsCompanyAnniversary
                                                ,@Mem_PremiumGrpCodeEffectivity)";

            string queryProfile = string.Format(@"   
                                       Insert Into {0}..M_Employee
                                       Select
                                           [Mem_IDNo]
                                          ,[Mem_LastName]
                                          ,[Mem_FirstName]
                                          ,[Mem_MiddleName]
                                          ,[Mem_MaidenName]
                                          ,[Mem_NickName]
                                          ,[Mem_Gender]
                                          ,[Mem_CivilStatusCode]
                                          ,[Mem_OfficeEmailAddress]
                                          ,[Mem_CostcenterCode]
                                          ,[Mem_EmploymentStatusCode]
                                          ,[Mem_RankCode]
                                          ,[Mem_IntakeDate]
                                          ,[Mem_RegularDate]
                                          ,[Mem_JobTitleCode]
                                          ,[Mem_PayrollType]
                                          ,[Mem_WorkStatus]
                                          ,[Mem_SeparationCode]
                                          ,[Mem_SeparationDate]
                                          ,[Usr_Login]
                                          ,GETDATE()
                                      From M_Employee
                                      Where Mem_IDNo = @Mem_IDNo", CentralProfile);

            #endregion

            #region [Parameter]
            ParameterInfo[] paramInfo = new ParameterInfo[103];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Mem_LastName", row["Mem_LastName"]);
            paramInfo[2] = new ParameterInfo("@Mem_FirstName", row["Mem_FirstName"]);
            paramInfo[3] = new ParameterInfo("@Mem_MiddleName", row["Mem_MiddleName"]);
            paramInfo[4] = new ParameterInfo("@Mem_MaidenName", row["Mem_MaidenName"]);
            paramInfo[5] = new ParameterInfo("@Mem_BirthDate", row["Mem_BirthDate"]);
            paramInfo[6] = new ParameterInfo("@Mem_BirthPlace", row["Mem_BirthPlace"]);
            paramInfo[7] = new ParameterInfo("@Mem_Age", row["Mem_Age"]);
            paramInfo[8] = new ParameterInfo("@Mem_Gender", row["Mem_Gender"]);
            paramInfo[9] = new ParameterInfo("@Mem_CivilStatusCode", row["Mem_CivilStatusCode"]);
            paramInfo[10] = new ParameterInfo("@Mem_MarriedDate", row["Mem_MarriedDate"]);
            paramInfo[11] = new ParameterInfo("@Mem_NationalityCode", row["Mem_NationalityCode"]);
            paramInfo[12] = new ParameterInfo("@Mem_ReligionCode", row["Mem_ReligionCode"]);
            paramInfo[13] = new ParameterInfo("@Mem_PresCompleteAddress", row["Mem_PresCompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[14] = new ParameterInfo("@Mem_PresAddressBarangay", row["Mem_PresAddressBarangay"]);
            paramInfo[15] = new ParameterInfo("@Mem_PresAddressMunicipalityCity", row["Mem_PresAddressMunicipalityCity"]);
            paramInfo[16] = new ParameterInfo("@Mem_LandlineNo", row["Mem_LandlineNo"]);
            paramInfo[17] = new ParameterInfo("@Mem_CellNo", row["Mem_CellNo"]);
            paramInfo[18] = new ParameterInfo("@Mem_OfficeEmailAddress", row["Mem_OfficeEmailAddress"]);
            paramInfo[19] = new ParameterInfo("@Mem_EducationCode", row["Mem_EducationCode"]);
            paramInfo[20] = new ParameterInfo("@Mem_SchoolCode", row["Mem_SchoolCode"]);
            paramInfo[21] = new ParameterInfo("@Mem_CourseCode", row["Mem_CourseCode"]);
            paramInfo[22] = new ParameterInfo("@Mem_BloodType", row["Mem_BloodType"]);
            paramInfo[23] = new ParameterInfo("@Mem_SSSNo", row["Mem_SSSNo"]);
            paramInfo[24] = new ParameterInfo("@Mem_PhilhealthNo", row["Mem_PhilhealthNo"]);
            paramInfo[25] = new ParameterInfo("@Mem_PayrollBankCode", row["Mem_PayrollBankCode"]);
            paramInfo[26] = new ParameterInfo("@Mem_BankAcctNo", row["Mem_BankAcctNo"]);
            paramInfo[27] = new ParameterInfo("@Mem_TIN", row["Mem_TIN"]);
            paramInfo[28] = new ParameterInfo("@Mem_TaxCode", row["Mem_TaxCode"]);
            paramInfo[29] = new ParameterInfo("@Mem_PagIbigNo", row["Mem_PagIbigNo"]);
            paramInfo[30] = new ParameterInfo("@Mem_PagIbigRule", row["Mem_PagIbigRule"]);
            paramInfo[31] = new ParameterInfo("@Mem_PagIbigShare", Convert.ToDecimal(row["Mem_PagIbigShare"]));
            paramInfo[32] = new ParameterInfo("@Mem_ICEContactPerson", row["Mem_ICEContactPerson"]);
            paramInfo[33] = new ParameterInfo("@Mem_ICERelation", row["Mem_ICERelation"]);
            paramInfo[34] = new ParameterInfo("@Mem_ICECompleteAddress", row["Mem_ICECompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[35] = new ParameterInfo("@Mem_ICEAddressBarangay", row["Mem_ICEAddressBarangay"]);
            paramInfo[36] = new ParameterInfo("@Mem_ICEAddressMunicipalityCity", row["Mem_ICEAddressMunicipalityCity"]);
            paramInfo[37] = new ParameterInfo("@Mem_ICEContactNo", row["Mem_ICEContactNo"]);
            paramInfo[38] = new ParameterInfo("@Mem_CostcenterDate", row["Mem_CostcenterDate"]);
            paramInfo[39] = new ParameterInfo("@Mem_CostcenterCode", row["Mem_CostcenterCode"]);
            paramInfo[40] = new ParameterInfo("@Mem_EmploymentStatusCode", row["Mem_EmploymentStatusCode"]);
            paramInfo[41] = new ParameterInfo("@Mem_IntakeDate", row["Mem_IntakeDate"]);
            paramInfo[42] = new ParameterInfo("@Mem_RegularDate", row["Mem_RegularDate"]);
            paramInfo[43] = new ParameterInfo("@Mem_WorkLocationCode", row["Mem_WorkLocationCode"]);
            paramInfo[44] = new ParameterInfo("@Mem_JobTitleCode", row["Mem_JobTitleCode"]);
            paramInfo[45] = new ParameterInfo("@Mem_PositionDate", row["Mem_PositionDate"]);
            paramInfo[46] = new ParameterInfo("@Mem_Superior1", row["Mem_Superior1"]);
            paramInfo[47] = new ParameterInfo("@Mem_Superior2", row["Mem_Superior2"]);
            paramInfo[48] = new ParameterInfo("@Mem_IsComputedPayroll", row["Mem_IsComputedPayroll"]);
            paramInfo[49] = new ParameterInfo("@Mem_PaymentMode", row["Mem_PaymentMode"]);
            paramInfo[50] = new ParameterInfo("@Mem_PayrollType", row["Mem_PayrollType"]);
            paramInfo[51] = new ParameterInfo("@Mem_SalaryDate", row["Mem_SalaryDate"]);
            paramInfo[52] = new ParameterInfo("@Mem_Salary", Convert.ToDecimal(row["Mem_Salary"]));
            paramInfo[53] = new ParameterInfo("@Mem_WorkStatus", row["Mem_WorkStatus"]);
            paramInfo[54] = new ParameterInfo("@Mem_IsConfidential", row["Mem_IsConfidential"]);
            paramInfo[55] = new ParameterInfo("@Mem_ShiftCode", row["Mem_ShiftCode"]);
            paramInfo[56] = new ParameterInfo("@Mem_CalendarType", row["Mem_CalendarType"]);
            paramInfo[57] = new ParameterInfo("@Mem_CalendarGroup", row["Mem_CalendarGroup"]);
            paramInfo[58] = new ParameterInfo("@Mem_SeparationNoticeDate", row["Mem_SeparationNoticeDate"]);
            paramInfo[59] = new ParameterInfo("@Mem_SeparationCode", row["Mem_SeparationCode"]);
            paramInfo[60] = new ParameterInfo("@Mem_SeparationDate", row["Mem_SeparationDate"]);

            paramInfo[61] = new ParameterInfo("@Mem_SSSRule", row["Mem_SSSRule"]);
            paramInfo[62] = new ParameterInfo("@Mem_SSSShare", row["Mem_SSSShare"]);
            paramInfo[63] = new ParameterInfo("@Mem_PHRule", row["Mem_PHRule"]);
            paramInfo[64] = new ParameterInfo("@Mem_PHShare", row["Mem_PHShare"]);

            paramInfo[65] = new ParameterInfo("@Mem_ClearedDate", row["Mem_ClearedDate"]);
            paramInfo[66] = new ParameterInfo("@Mem_Image", row["Mem_Image"], SqlDbType.Image, ImageSize);
            paramInfo[67] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            paramInfo[68] = new ParameterInfo("@Mem_ProbationDate", row["Mem_ProbationDate"]);

            paramInfo[69] = new ParameterInfo("@Mem_Height", (row["Mem_Height"].ToString().ToString() == "") ? " " : row["Mem_Height"]);
            paramInfo[70] = new ParameterInfo("@Mem_Weight", (row["Mem_Weight"].ToString() == "") ? " " : row["Mem_Weight"]);

            paramInfo[71] = new ParameterInfo("@Mem_ProvCompleteAddress", (row["Mem_ProvCompleteAddress"].ToString() == "") ? " " : row["Mem_ProvCompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[72] = new ParameterInfo("@Mem_ProvAddressBarangay", (row["Mem_ProvAddressBarangay"].ToString() == "") ? " " : row["Mem_ProvAddressBarangay"]);
            paramInfo[73] = new ParameterInfo("@Mem_ProvAddressMunicipalityCity", (row["Mem_ProvAddressMunicipalityCity"].ToString() == "") ? " " : row["Mem_ProvAddressMunicipalityCity"]);
            paramInfo[74] = new ParameterInfo("@Mem_ProvLandlineNo", (row["Mem_ProvLandlineNo"].ToString() == "") ? " " : row["Mem_ProvLandlineNo"]);

            paramInfo[75] = new ParameterInfo("@Mem_AwardsRecognition", (row["Mem_AwardsRecognition"].ToString() == "") ? " " : row["Mem_AwardsRecognition"]);
            paramInfo[76] = new ParameterInfo("@Mem_PRCLicense", (row["Mem_PRCLicense"].ToString() == "") ? " " : row["Mem_PRCLicense"]);
            paramInfo[77] = new ParameterInfo("@Mem_ExpenseClass", row["Mem_ExpenseClass"]);
            paramInfo[78] = new ParameterInfo("@Mem_NickName", (row["Mem_NickName"].ToString() == "") ? " " : row["Mem_NickName"]);
            paramInfo[79] = new ParameterInfo("@Emt_OldEmployeeID", (row["Emt_OldEmployeeID"].ToString() == "") ? " " : row["Emt_OldEmployeeID"]); 
            paramInfo[80] = new ParameterInfo("@Mem_WifeClaim", row["Mem_WifeClaim"]);

            paramInfo[81] = new ParameterInfo("@Mem_ShoesSize", (row["Mem_ShoesSize"].ToString() == "") ? " " : row["Mem_ShoesSize"]);
            paramInfo[82] = new ParameterInfo("@Mem_ShirtSize", (row["Mem_ShirtSize"].ToString() == "") ? " " : row["Mem_ShirtSize"]);
            paramInfo[83] = new ParameterInfo("@Mem_HairColor", (row["Mem_HairColor"].ToString() == "") ? " " : row["Mem_HairColor"]);
            paramInfo[84] = new ParameterInfo("@Mem_EyeColor", (row["Mem_EyeColor"].ToString() == "") ? " " : row["Mem_EyeColor"]);
            paramInfo[85] = new ParameterInfo("@Mem_DistinguishMark", (row["Mem_DistinguishMark"].ToString() == "") ? " " : row["Mem_DistinguishMark"]);
            paramInfo[86] = new ParameterInfo("@Mem_JobGrade", (row["Mem_JobGrade"].ToString() == "") ? " " : row["Mem_JobGrade"]);
            paramInfo[87] = new ParameterInfo("@Mem_PositionCategory", (row["Mem_PositionCategory"].ToString() == "") ? " " : row["Mem_PositionCategory"]);
            paramInfo[88] = new ParameterInfo("@Mem_PositionClass", (row["Mem_PositionClass"].ToString() == "") ? " " : row["Mem_PositionClass"]);
            paramInfo[89] = new ParameterInfo("@Mem_PremiumGrpCode", (row["Mem_PremiumGrpCode"].ToString() == "") ? " " : row["Mem_PremiumGrpCode"]);
           
            paramInfo[89] = new ParameterInfo("@Mem_PersonalEmail", (row["Mem_PersonalEmail"].ToString() == "") ? " " : row["Mem_PersonalEmail"]);
            paramInfo[90] = new ParameterInfo("@Mem_GraduatedDate", row["Mem_GraduatedDate"]);
            paramInfo[91] = new ParameterInfo("@Mem_Contact1", (row["Mem_Contact1"].ToString() == "") ? " " : row["Mem_Contact1"]);
            paramInfo[92] = new ParameterInfo("@Mem_Contact2", (row["Mem_Contact2"].ToString() == "") ? " " : row["Mem_Contact2"]);
            paramInfo[93] = new ParameterInfo("@Mem_Contact3", (row["Mem_Contact3"].ToString() == "") ? " " : row["Mem_Contact3"]);
            paramInfo[94] = new ParameterInfo("@Mem_RankCode", (row["Mem_RankCode"].ToString() == "") ? " " : row["Mem_RankCode"]);

            paramInfo[95] = new ParameterInfo("@Mem_OldPayrollType", row["Mem_OldPayrollType"]);
            paramInfo[96] = new ParameterInfo("@Mem_OldSalaryDate", row["Mem_OldSalaryDate"]);
            paramInfo[97] = new ParameterInfo("@Mem_OldSalaryRate", row["Mem_OldSalaryRate"]);
            paramInfo[98] = new ParameterInfo("@Mem_IsTaxExempted", row["Mem_IsTaxExempted"]);
            paramInfo[99] = new ParameterInfo("@Mem_PremiumGrpCode", row["Mem_PremiumGrpCode"]);
            paramInfo[100] = new ParameterInfo("@Mem_PayClass", "");
            paramInfo[101] = new ParameterInfo("@Mem_IsCompanyAnniversary", 0);
            paramInfo[102] = new ParameterInfo("@Mem_PremiumGrpDate", row["Mem_PremiumGrpDate"]);
            #endregion

            retVal = dalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

            dalUp.ExecuteNonQuery(queryProfile, CommandType.Text, paramInfo);

            return retVal;
        }

        public int AddEmployee(DataRow row, DataRow drWorkLocation, DataRow drCostCnterMvmnt, DataRow drPremGroup, DataRow drPos, DataRow drEmpStat, DataRow drProfMovement, DataRow drSalaryMov, DataRow drEmployeeGrp, DataRow drUserMaster, DataRow drPUserMaster, DataRow drLogMaster, DataRow drEmployeeRestDay, int imageSize, string strCopiedFromID, bool isIDAutoGenerated, string transactionCode)
        {
            string CentralProfile = LoginInfo.getUser().CentralProfileName;
            string DTRDB = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());
            int bSuccess = 0;

            #region M_Employee

            #region M_Employee Parameters

            ParameterInfo[] pEmployeeMaster = new ParameterInfo[103];
            pEmployeeMaster[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            pEmployeeMaster[1] = new ParameterInfo("@Mem_LastName", row["Mem_LastName"]);
            pEmployeeMaster[2] = new ParameterInfo("@Mem_FirstName", row["Mem_FirstName"]);
            pEmployeeMaster[3] = new ParameterInfo("@Mem_MiddleName", row["Mem_MiddleName"]);
            pEmployeeMaster[4] = new ParameterInfo("@Mem_MaidenName", row["Mem_MaidenName"]);
            pEmployeeMaster[5] = new ParameterInfo("@Mem_BirthDate", row["Mem_BirthDate"]);
            pEmployeeMaster[6] = new ParameterInfo("@Mem_BirthPlace", row["Mem_BirthPlace"]);
            pEmployeeMaster[7] = new ParameterInfo("@Mem_Age", row["Mem_Age"]);
            pEmployeeMaster[8] = new ParameterInfo("@Mem_Gender", row["Mem_Gender"]);
            pEmployeeMaster[9] = new ParameterInfo("@Mem_CivilStatusCode", row["Mem_CivilStatusCode"]);
            pEmployeeMaster[10] = new ParameterInfo("@Mem_MarriedDate", row["Mem_MarriedDate"]);
            pEmployeeMaster[11] = new ParameterInfo("@Mem_NationalityCode", row["Mem_NationalityCode"]);
            pEmployeeMaster[12] = new ParameterInfo("@Mem_ReligionCode", row["Mem_ReligionCode"]);
            pEmployeeMaster[13] = new ParameterInfo("@Mem_PresCompleteAddress", row["Mem_PresCompleteAddress"], SqlDbType.VarChar, 200);
            pEmployeeMaster[14] = new ParameterInfo("@Mem_PresAddressBarangay", row["Mem_PresAddressBarangay"]);
            pEmployeeMaster[15] = new ParameterInfo("@Mem_PresAddressMunicipalityCity", row["Mem_PresAddressMunicipalityCity"]);
            pEmployeeMaster[16] = new ParameterInfo("@Mem_LandlineNo", row["Mem_LandlineNo"]);
            pEmployeeMaster[17] = new ParameterInfo("@Mem_CellNo", row["Mem_CellNo"]);
            pEmployeeMaster[18] = new ParameterInfo("@Mem_OfficeEmailAddress", row["Mem_OfficeEmailAddress"]);
            pEmployeeMaster[19] = new ParameterInfo("@Mem_EducationCode", row["Mem_EducationCode"]);
            pEmployeeMaster[20] = new ParameterInfo("@Mem_SchoolCode", row["Mem_SchoolCode"]);
            pEmployeeMaster[21] = new ParameterInfo("@Mem_CourseCode", row["Mem_CourseCode"]);
            pEmployeeMaster[22] = new ParameterInfo("@Mem_BloodType", row["Mem_BloodType"]);
            pEmployeeMaster[23] = new ParameterInfo("@Mem_SSSNo", row["Mem_SSSNo"]);
            pEmployeeMaster[24] = new ParameterInfo("@Mem_PhilhealthNo", row["Mem_PhilhealthNo"]);
            pEmployeeMaster[25] = new ParameterInfo("@Mem_PayrollBankCode", row["Mem_PayrollBankCode"]);
            pEmployeeMaster[26] = new ParameterInfo("@Mem_BankAcctNo", row["Mem_BankAcctNo"]);
            pEmployeeMaster[27] = new ParameterInfo("@Mem_TIN", row["Mem_TIN"]);
            pEmployeeMaster[28] = new ParameterInfo("@Mem_TaxCode", row["Mem_TaxCode"]);
            pEmployeeMaster[29] = new ParameterInfo("@Mem_PagIbigNo", row["Mem_PagIbigNo"]);
            pEmployeeMaster[30] = new ParameterInfo("@Mem_PagIbigRule", row["Mem_PagIbigRule"]);
            pEmployeeMaster[31] = new ParameterInfo("@Mem_PagIbigShare", Convert.ToDecimal(row["Mem_PagIbigShare"]));
            pEmployeeMaster[32] = new ParameterInfo("@Mem_ICEContactPerson", row["Mem_ICEContactPerson"]);
            pEmployeeMaster[33] = new ParameterInfo("@Mem_ICERelation", row["Mem_ICERelation"]);
            pEmployeeMaster[34] = new ParameterInfo("@Mem_ICECompleteAddress", row["Mem_ICECompleteAddress"], SqlDbType.VarChar, 200);
            pEmployeeMaster[35] = new ParameterInfo("@Mem_ICEAddressBarangay", row["Mem_ICEAddressBarangay"]);
            pEmployeeMaster[36] = new ParameterInfo("@Mem_ICEAddressMunicipalityCity", row["Mem_ICEAddressMunicipalityCity"]);
            pEmployeeMaster[37] = new ParameterInfo("@Mem_ICEContactNo", row["Mem_ICEContactNo"]);
            pEmployeeMaster[38] = new ParameterInfo("@Mem_CostcenterDate", row["Mem_CostcenterDate"]);
            pEmployeeMaster[39] = new ParameterInfo("@Mem_CostcenterCode", row["Mem_CostcenterCode"]);
            pEmployeeMaster[40] = new ParameterInfo("@Mem_EmploymentStatusCode", row["Mem_EmploymentStatusCode"]);
            pEmployeeMaster[41] = new ParameterInfo("@Mem_IntakeDate", row["Mem_IntakeDate"]);
            pEmployeeMaster[42] = new ParameterInfo("@Mem_RegularDate", row["Mem_RegularDate"]);
            pEmployeeMaster[43] = new ParameterInfo("@Mem_WorkLocationCode", row["Mem_WorkLocationCode"]);
            pEmployeeMaster[44] = new ParameterInfo("@Mem_JobTitleCode", row["Mem_JobTitleCode"]);
            pEmployeeMaster[45] = new ParameterInfo("@Mem_PositionDate", row["Mem_PositionDate"]);
            pEmployeeMaster[46] = new ParameterInfo("@Mem_Superior1", row["Mem_Superior1"]);
            pEmployeeMaster[47] = new ParameterInfo("@Mem_Superior2", row["Mem_Superior2"]);
            pEmployeeMaster[48] = new ParameterInfo("@Mem_IsComputedPayroll", row["Mem_IsComputedPayroll"]);
            pEmployeeMaster[49] = new ParameterInfo("@Mem_PaymentMode", row["Mem_PaymentMode"]);
            pEmployeeMaster[50] = new ParameterInfo("@Mem_PayrollType", row["Mem_PayrollType"]);
            pEmployeeMaster[51] = new ParameterInfo("@Mem_SalaryDate", row["Mem_SalaryDate"]);
            pEmployeeMaster[52] = new ParameterInfo("@Mem_Salary", Convert.ToDecimal(row["Mem_Salary"]));
            pEmployeeMaster[53] = new ParameterInfo("@Mem_WorkStatus", row["Mem_WorkStatus"]);
            pEmployeeMaster[54] = new ParameterInfo("@Mem_IsConfidential", row["Mem_IsConfidential"]);
            pEmployeeMaster[55] = new ParameterInfo("@Mem_ShiftCode", row["Mem_ShiftCode"]);
            pEmployeeMaster[56] = new ParameterInfo("@Mem_CalendarType", row["Mem_CalendarType"]);
            pEmployeeMaster[57] = new ParameterInfo("@Mem_CalendarGroup", row["Mem_CalendarGroup"]);
            pEmployeeMaster[58] = new ParameterInfo("@Mem_SeparationNoticeDate", row["Mem_SeparationNoticeDate"]);
            pEmployeeMaster[59] = new ParameterInfo("@Mem_SeparationCode", row["Mem_SeparationCode"]);
            pEmployeeMaster[60] = new ParameterInfo("@Mem_SeparationDate", row["Mem_SeparationDate"]);
            pEmployeeMaster[61] = new ParameterInfo("@Mem_SSSRule", row["Mem_SSSRule"]);
            pEmployeeMaster[62] = new ParameterInfo("@Mem_SSSShare", row["Mem_SSSShare"]);
            pEmployeeMaster[63] = new ParameterInfo("@Mem_PHRule", row["Mem_PHRule"]);
            pEmployeeMaster[64] = new ParameterInfo("@Mem_PHShare", row["Mem_PHShare"]);
            pEmployeeMaster[65] = new ParameterInfo("@Mem_ClearedDate", row["Mem_ClearedDate"]);
            pEmployeeMaster[66] = new ParameterInfo("@Mem_Image", row["Mem_Image"], SqlDbType.Image, imageSize);
            pEmployeeMaster[67] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            pEmployeeMaster[68] = new ParameterInfo("@Mem_ProbationDate", row["Mem_ProbationDate"]);
            pEmployeeMaster[69] = new ParameterInfo("@Mem_Height", (row["Mem_Height"].ToString().ToString() == "") ? " " : row["Mem_Height"]);
            pEmployeeMaster[70] = new ParameterInfo("@Mem_Weight", (row["Mem_Weight"].ToString() == "") ? " " : row["Mem_Weight"]);
            pEmployeeMaster[71] = new ParameterInfo("@Mem_ProvCompleteAddress", (row["Mem_ProvCompleteAddress"].ToString() == "") ? " " : row["Mem_ProvCompleteAddress"], SqlDbType.VarChar, 200);
            pEmployeeMaster[72] = new ParameterInfo("@Mem_ProvAddressBarangay", (row["Mem_ProvAddressBarangay"].ToString() == "") ? " " : row["Mem_ProvAddressBarangay"]);
            pEmployeeMaster[73] = new ParameterInfo("@Mem_ProvAddressMunicipalityCity", (row["Mem_ProvAddressMunicipalityCity"].ToString() == "") ? " " : row["Mem_ProvAddressMunicipalityCity"]);
            pEmployeeMaster[74] = new ParameterInfo("@Mem_ProvLandlineNo", (row["Mem_ProvLandlineNo"].ToString() == "") ? " " : row["Mem_ProvLandlineNo"]);
            pEmployeeMaster[75] = new ParameterInfo("@Mem_AwardsRecognition", (row["Mem_AwardsRecognition"].ToString() == "") ? " " : row["Mem_AwardsRecognition"], SqlDbType.VarChar, 200);
            pEmployeeMaster[76] = new ParameterInfo("@Mem_PRCLicense", (row["Mem_PRCLicense"].ToString() == "") ? " " : row["Mem_PRCLicense"]);
            pEmployeeMaster[77] = new ParameterInfo("@Mem_ExpenseClass", row["Mem_ExpenseClass"]);
            pEmployeeMaster[78] = new ParameterInfo("@Mem_NickName", (row["Mem_NickName"].ToString() == "") ? " " : row["Mem_NickName"]);
            pEmployeeMaster[79] = new ParameterInfo("@Mem_AltAcctCode", (row["Mem_AltAcctCode"].ToString() == "") ? " " : row["Mem_AltAcctCode"]);
            pEmployeeMaster[80] = new ParameterInfo("@Mem_WifeClaim", row["Mem_WifeClaim"]);
            pEmployeeMaster[81] = new ParameterInfo("@Mem_ShoesSize", (row["Mem_ShoesSize"].ToString() == "") ? " " : row["Mem_ShoesSize"]);
            pEmployeeMaster[82] = new ParameterInfo("@Mem_ShirtSize", (row["Mem_ShirtSize"].ToString() == "") ? " " : row["Mem_ShirtSize"]);
            pEmployeeMaster[83] = new ParameterInfo("@Mem_HairColor", (row["Mem_HairColor"].ToString() == "") ? " " : row["Mem_HairColor"]);
            pEmployeeMaster[84] = new ParameterInfo("@Mem_EyeColor", (row["Mem_EyeColor"].ToString() == "") ? " " : row["Mem_EyeColor"]);
            pEmployeeMaster[85] = new ParameterInfo("@Mem_DistinguishMark", (row["Mem_DistinguishMark"].ToString() == "") ? " " : row["Mem_DistinguishMark"], SqlDbType.VarChar, 200);
            pEmployeeMaster[86] = new ParameterInfo("@Mem_JobGrade", (row["Mem_JobGrade"].ToString() == "") ? " " : row["Mem_JobGrade"]);
            pEmployeeMaster[87] = new ParameterInfo("@Mem_PositionCategory", (row["Mem_PositionCategory"].ToString() == "") ? " " : row["Mem_PositionCategory"]);
            pEmployeeMaster[88] = new ParameterInfo("@Mem_PositionClass", (row["Mem_PositionClass"].ToString() == "") ? " " : row["Mem_PositionClass"]);
            pEmployeeMaster[89] = new ParameterInfo("@Mem_PremiumGrpCode", (row["Mem_PremiumGrpCode"].ToString() == "") ? " " : row["Mem_PremiumGrpCode"]);
            pEmployeeMaster[89] = new ParameterInfo("@Mem_PersonalEmail", (row["Mem_PersonalEmail"].ToString() == "") ? " " : row["Mem_PersonalEmail"]);
            pEmployeeMaster[90] = new ParameterInfo("@Mem_GraduatedDate", row["Mem_GraduatedDate"]);
            pEmployeeMaster[91] = new ParameterInfo("@Mem_Contact1", (row["Mem_Contact1"].ToString() == "") ? " " : row["Mem_Contact1"]);
            pEmployeeMaster[92] = new ParameterInfo("@Mem_Contact2", (row["Mem_Contact2"].ToString() == "") ? " " : row["Mem_Contact2"]);
            pEmployeeMaster[93] = new ParameterInfo("@Mem_Contact3", (row["Mem_Contact3"].ToString() == "") ? " " : row["Mem_Contact3"]);
            pEmployeeMaster[94] = new ParameterInfo("@Mem_RankCode", (row["Mem_RankCode"].ToString() == "") ? " " : row["Mem_RankCode"]);
            pEmployeeMaster[95] = new ParameterInfo("@Mem_OldPayrollType", row["Mem_OldPayrollType"]);
            pEmployeeMaster[96] = new ParameterInfo("@Mem_OldSalaryDate", row["Mem_OldSalaryDate"]);
            pEmployeeMaster[97] = new ParameterInfo("@Mem_OldSalaryRate", row["Mem_OldSalaryRate"]);
            pEmployeeMaster[98] = new ParameterInfo("@Mem_IsTaxExempted", row["Mem_IsTaxExempted"]);
            pEmployeeMaster[99] = new ParameterInfo("@Mem_PremiumGrpCode", row["Mem_PremiumGrpCode"]);
            pEmployeeMaster[100] = new ParameterInfo("@Mem_PayClass", (row["Mem_PayClass"].ToString() == "") ? " " : row["Mem_PayClass"]);
            pEmployeeMaster[101] = new ParameterInfo("@Mem_IsCompanyAnniversary", 0);
            pEmployeeMaster[102] = new ParameterInfo("@Mem_PremiumGrpDate", row["Mem_PremiumGrpDate"]);
            #endregion

            #region M_Employee Insert Query

            string qEmployeeMaster = @"INSERT INTO M_Employee
                                                (Mem_IDNo,
                                                Mem_LastName,
                                                Mem_FirstName,
                                                Mem_MiddleName,
                                                Mem_MaidenName,
                                                Mem_BirthDate,
                                                Mem_BirthPlace,
                                                Mem_Age,
                                                Mem_Gender,
                                                Mem_CivilStatusCode,
                                                Mem_MarriedDate,
                                                Mem_NationalityCode,
                                                Mem_ReligionCode,
                                                Mem_PresCompleteAddress,
                                                Mem_PresAddressBarangay,
                                                Mem_PresAddressMunicipalityCity,
                                                Mem_LandlineNo,
                                                Mem_CellNo,
                                                Mem_OfficeEmailAddress,
                                                Mem_PersonalEmail,
                                                Mem_EducationCode,
                                                Mem_SchoolCode,
                                                Mem_CourseCode,
                                                Mem_BloodType,
                                                Mem_SSSNo,
                                                Mem_PhilhealthNo,
                                                Mem_PayrollBankCode,
                                                Mem_BankAcctNo,
                                                Mem_TIN,
                                                Mem_TaxCode,
                                                Mem_PagIbigNo,
                                                Mem_PagIbigRule,
                                                Mem_PagIbigShare,
                                                Mem_ICEContactPerson,
                                                Mem_ICERelation,
                                                Mem_ICECompleteAddress,
                                                Mem_ICEAddressBarangay,
                                                Mem_ICEAddressMunicipalityCity,
                                                Mem_ICEContactNo,
                                                Mem_CostcenterDate,
                                                Mem_CostcenterCode,
                                                Mem_EmploymentStatusCode,
                                                Mem_IntakeDate,
                                                Mem_RegularDate,
                                                Mem_WorkLocationCode,
                                                Mem_JobTitleCode,
                                                Mem_PositionDate,
                                                Mem_Superior1,
                                                Mem_Superior2,
                                                Mem_IsComputedPayroll,
                                                Mem_PaymentMode,
                                                Mem_PayrollType,
                                                Mem_SalaryDate,
                                                Mem_Salary,
                                                Mem_WorkStatus,
                                                Mem_IsConfidential,
                                                Mem_ShiftCode,
                                                Mem_CalendarType,
                                                Mem_CalendarGroup,
                                                Mem_SeparationNoticeDate,
                                                Mem_SeparationCode,
                                                Mem_SeparationDate,
                                                Mem_ClearedDate
                                                ,Mem_SSSRule 
                                                ,Mem_SSSShare 
                                                ,Mem_PHRule 
                                                ,Mem_PHShare 
                                                ,Mem_Image
                                                ,Mem_ProbationDate
                                                ,Mem_Height
                                                ,Mem_Weight
                                                ,Mem_ProvCompleteAddress
                                                ,Mem_ProvAddressBarangay
                                                ,Mem_ProvAddressMunicipalityCity
                                                ,Mem_ProvLandlineNo
                                                ,Mem_AwardsRecognition
                                                ,Mem_PRCLicense
                                                ,Mem_ExpenseClass
                                                ,Usr_Login
                                                ,Ludatetime
                                                ,Mem_NickName
                                                ,Mem_AltAcctCode
                                                ,Mem_WifeClaim
                                                ,Mem_ShoesSize
                                                ,Mem_ShirtSize
                                                ,Mem_HairColor
                                                ,Mem_EyeColor
                                                ,Mem_DistinguishMark
                                                ,Mem_JobGrade
                                                ,Mem_RankCode
                                                ,Mem_PositionCategory
                                                ,Mem_PositionClass
                                                ,Mem_PremiumGrpCode
                                                ,Mem_GraduatedDate
                                                ,Mem_Contact1
                                                ,Mem_Contact2
                                                ,Mem_Contact3
                                                ,Mem_OldPayrollType
                                                ,Mem_OldSalaryDate
                                                ,Mem_OldSalaryRate
                                                ,Mem_IsTaxExempted
                                                ,Mem_PayClass
                                                ,Mem_IsCompanyAnniversary
                                                ,Mem_PremiumGrpDate)
                                               VALUES
                                                (@Mem_IDNo,
                                                @Mem_LastName,
                                                @Mem_FirstName,
                                                @Mem_MiddleName,
                                                @Mem_MaidenName,
                                                @Mem_BirthDate,
                                                @Mem_BirthPlace,
                                                @Mem_Age,
                                                @Mem_Gender,
                                                @Mem_CivilStatusCode,
                                                @Mem_MarriedDate,
                                                @Mem_NationalityCode,
                                                @Mem_ReligionCode,
                                                @Mem_PresCompleteAddress,
                                                @Mem_PresAddressBarangay,
                                                @Mem_PresAddressMunicipalityCity,
                                                @Mem_LandlineNo,
                                                @Mem_CellNo,
                                                @Mem_OfficeEmailAddress,
                                                @Mem_PersonalEmail,
                                                @Mem_EducationCode,
                                                @Mem_SchoolCode,
                                                @Mem_CourseCode,
                                                @Mem_BloodType,
                                                @Mem_SSSNo,
                                                @Mem_PhilhealthNo,
                                                @Mem_PayrollBankCode,
                                                @Mem_BankAcctNo,
                                                @Mem_TIN,
                                                @Mem_TaxCode,
                                                @Mem_PagIbigNo,
                                                @Mem_PagIbigRule,
                                                @Mem_PagIbigShare,
                                                @Mem_ICEContactPerson,
                                                @Mem_ICERelation,
                                                @Mem_ICECompleteAddress,
                                                @Mem_ICEAddressBarangay,
                                                @Mem_ICEAddressMunicipalityCity,
                                                @Mem_ICEContactNo,
                                                @Mem_CostcenterDate,
                                                @Mem_CostcenterCode,
                                                @Mem_EmploymentStatusCode,
                                                @Mem_IntakeDate,
                                                @Mem_RegularDate,
                                                @Mem_WorkLocationCode,
                                                @Mem_JobTitleCode,
                                                @Mem_PositionDate,
                                                @Mem_Superior1,
                                                @Mem_Superior2,
                                                @Mem_IsComputedPayroll,
                                                @Mem_PaymentMode,
                                                @Mem_PayrollType,
                                                @Mem_SalaryDate,
                                                @Mem_Salary,
                                                @Mem_WorkStatus,
                                                @Mem_IsConfidential,
                                                @Mem_ShiftCode,
                                                @Mem_CalendarType,
                                                @Mem_CalendarGroup,
                                                @Mem_SeparationNoticeDate,
                                                @Mem_SeparationCode,
                                                @Mem_SeparationDate,
                                                @Mem_ClearedDate
                                                ,@Mem_SSSRule 
                                                ,@Mem_SSSShare 
                                                ,@Mem_PHRule 
                                                ,@Mem_PHShare 
                                                ,@Mem_Image
                                                ,@Mem_ProbationDate
                                                ,@Mem_Height
                                                ,@Mem_Weight
                                                ,@Mem_ProvCompleteAddress
                                                ,@Mem_ProvAddressBarangay
                                                ,@Mem_ProvAddressMunicipalityCity
                                                ,@Mem_ProvLandlineNo
                                                ,@Mem_AwardsRecognition
                                                ,@Mem_PRCLicense
                                                ,@Mem_ExpenseClass
                                                ,@Usr_Login
                                                ,GetDate()
                                                ,@Mem_NickName
                                                ,@Mem_AltAcctCode
                                                ,@Mem_WifeClaim
                                                ,@Mem_ShoesSize
                                                ,@Mem_ShirtSize
                                                ,@Mem_HairColor
                                                ,@Mem_EyeColor
                                                ,@Mem_DistinguishMark
                                                ,@Mem_JobGrade
                                                ,@Mem_RankCode
                                                ,@Mem_PositionCategory
                                                ,@Mem_PositionClass
                                                ,@Mem_PremiumGrpCode
                                                ,@Mem_GraduatedDate
                                                ,@Mem_Contact1
                                                ,@Mem_Contact2
                                                ,@Mem_Contact3
                                                ,@Mem_OldPayrollType
                                                ,@Mem_OldSalaryDate
                                                ,@Mem_OldSalaryRate
                                                ,@Mem_IsTaxExempted
                                                ,@Mem_PayClass
                                                ,@Mem_IsCompanyAnniversary
                                                ,@Mem_PremiumGrpDate)";

            #endregion

            #region Profile M_Employee Insert Query

            string qPEmployeeMaster = string.Format(@"   
                                       Insert Into {0}..M_Employee
                                        (      Mem_IDNo
                                              ,Mem_LastName
                                              ,Mem_FirstName
                                              ,Mem_MiddleName
                                              ,Mem_MaidenName
                                              ,Mem_NickName
                                              ,Mem_Gender
                                              ,Mem_CivilStatusCode
                                              ,Mem_OfficeEmailAddress
                                              ,Mem_CostcenterCode
                                              ,Mem_EmploymentStatusCode
                                              ,Mem_RankCode
                                              ,Mem_IntakeDate
                                              ,Mem_RegularDate
                                              ,Mem_JobTitleCode
                                              ,Mem_PayrollType
                                              ,Mem_WorkStatus
                                              ,Mem_SeparationCode
                                              ,Mem_SeparationDate
                                              ,Usr_Login
                                              ,Ludatetime
                                        )
                                       Select
                                           [Mem_IDNo]
                                          ,[Mem_LastName]
                                          ,[Mem_FirstName]
                                          ,[Mem_MiddleName]
                                          ,[Mem_MaidenName]
                                          ,[Mem_NickName]
                                          ,[Mem_Gender]
                                          ,[Mem_CivilStatusCode]
                                          ,[Mem_OfficeEmailAddress]
                                          ,[Mem_CostcenterCode]
                                          ,[Mem_EmploymentStatusCode]
                                          ,[Mem_RankCode]
                                          ,[Mem_IntakeDate]
                                          ,[Mem_RegularDate]
                                          ,[Mem_JobTitleCode]
                                          ,[Mem_PayrollType]
                                          ,[Mem_WorkStatus]
                                          ,[Mem_SeparationCode]
                                          ,[Mem_SeparationDate]
                                          ,[Usr_Login]
                                          ,GETDATE()
                                      From M_Employee
                                      Where Mem_IDNo = @Mem_IDNo", CentralProfile);

            #endregion

            #endregion

            #region T_EmpWorkLocation

            #region T_EmpWorkLocation Parameters

            ParameterInfo[] pEmployeeWorkLoc = new ParameterInfo[4];
            pEmployeeWorkLoc[0] = new ParameterInfo("@Twl_IDNo", drWorkLocation["Twl_IDNo"]);
            pEmployeeWorkLoc[1] = new ParameterInfo("@Twl_WorkLocationCode", drWorkLocation["Twl_WorkLocationCode"]);
            pEmployeeWorkLoc[2] = new ParameterInfo("@Twl_ReasonCode", drWorkLocation["Twl_ReasonCode"]);
            pEmployeeWorkLoc[3] = new ParameterInfo("@Usr_Login", drWorkLocation["Usr_Login"]);

            #endregion

            #region T_EmpWorkLocation Insert Query

            string qEmployeeWorkLoc = @"
                                    DECLARE @EffectivityDate datetime
                                    SET @EffectivityDate = (SELECT Tps_StartCycle
                                                        FROM T_PaySchedule
                                                        WHERE Tps_CycleIndicator = 'C')

                                       INSERT INTO T_EmpWorkLocation
                                           (Twl_IDNo
                                           ,Twl_StartDate
                                           ,Twl_WorkLocationCode
                                           ,Twl_ReasonCode
                                           ,Usr_Login
                                           ,Ludatetime)
                                        VALUES
                                           (@Twl_IDNo
                                           ,@EffectivityDate
                                           ,@Twl_WorkLocationCode
                                           ,@Twl_ReasonCode
                                           ,@Usr_Login
                                           ,Getdate())";

            #endregion

            #endregion

            #region T_EmpCostcenter

            #region T_EmpCostcenter Parameters
            ParameterInfo[] pEmployeeCstCntrMvment = new ParameterInfo[6];
            pEmployeeCstCntrMvment[0] = new ParameterInfo("@Tcc_IDNo", drCostCnterMvmnt["Tcc_IDNo"]);
            pEmployeeCstCntrMvment[1] = new ParameterInfo("@Tcc_StartDate", drCostCnterMvmnt["Tcc_StartDate"]);
            pEmployeeCstCntrMvment[2] = new ParameterInfo("@Tcc_EndDate", DBNull.Value);
            pEmployeeCstCntrMvment[3] = new ParameterInfo("@Tcc_CostCenterCode", drCostCnterMvmnt["Tcc_CostCenterCode"]);
            pEmployeeCstCntrMvment[4] = new ParameterInfo("@Usr_Login", drCostCnterMvmnt["Usr_Login"]);
            pEmployeeCstCntrMvment[5] = new ParameterInfo("@Tcc_ReasonCode", drCostCnterMvmnt["Tcc_ReasonCode"]);
            #endregion

            #region T_EmpCostcenter Insert Query
            string qEmpCstCnterMvment = @"INSERT INTO T_EmpCostcenter 
                                            (Tcc_IDNo, 
                                             Tcc_StartDate,
                                             Tcc_EndDate,
                                             Tcc_CostCenterCode,
                                             Usr_Login, 
                                             Ludatetime
                                             ,Tcc_ReasonCode) 
                                           VALUES
                                            (@Tcc_IDNo, 
                                             @Tcc_StartDate, 
                                             @Tcc_EndDate,
                                             @Tcc_CostCenterCode,
                                             @Usr_Login, 
                                             GetDate()
                                            ,@Tcc_ReasonCode)";
            #endregion

            #endregion

            #region T_EmpPremiumGroup

            #region T_EmpPremiumGroup Parameters

            ParameterInfo[] pPremGroup = new ParameterInfo[5];
            pPremGroup[0] = new ParameterInfo("@Tpg_IDNo", drPremGroup["Tpg_IDNo"]);
            pPremGroup[1] = new ParameterInfo("@Tpg_StartDate", drPremGroup["Tpg_StartDate"]);
            pPremGroup[2] = new ParameterInfo("@Tpg_PremiumGroup", drPremGroup["Tpg_PremiumGroup"]);
            pPremGroup[3] = new ParameterInfo("@Usr_Login", drPremGroup["Usr_Login"]);
            pPremGroup[4] = new ParameterInfo("@Tpg_ReasonCode", drPremGroup["Tpg_ReasonCode"]);

            #endregion

            #region T_EmpPremiumGroup Insert Query

            string qPremGroup = @"INSERT INTO T_EmpPremiumGroup
                                               (Tpg_IDNo
                                               ,Tpg_StartDate
                                               ,Tpg_PremiumGroup
                                               ,Usr_Login
                                               ,Ludatetime
                                               ,Tpg_ReasonCode)
                                         VALUES
                                               (@Tpg_IDNo
                                               ,@Tpg_StartDate
                                               ,@Tpg_PremiumGroup
                                               ,@Usr_Login
                                               ,GetDate()
                                               ,@Tpg_ReasonCode)";

            #endregion

            #endregion

            #region T_EmpPosition

            #region T_EmpPosition Parameters

            ParameterInfo[] pPosMovement = new ParameterInfo[12];
            pPosMovement[0] = new ParameterInfo("@Tpo_IDNo", drPos["Tpo_IDNo"]);
            pPosMovement[1] = new ParameterInfo("@Tep_StartDate", drPos["Tep_StartDate"]);
            pPosMovement[2] = new ParameterInfo("@Tep_EndDate", drPos["Tep_EndDate"]);
            pPosMovement[3] = new ParameterInfo("@Epm_PositionCode", drPos["Epm_PositionCode"]);
            pPosMovement[4] = new ParameterInfo("@Epm_JobLevel", drPos["Epm_JobLevel"]);
            pPosMovement[5] = new ParameterInfo("@Epm_PositionCategory", drPos["Epm_PositionCategory"]);
            pPosMovement[6] = new ParameterInfo("@Epm_Classification", drPos["Epm_Classification"]);
            pPosMovement[7] = new ParameterInfo("@Epm_JobGrade", drPos["Epm_JobGrade"]);
            pPosMovement[8] = new ParameterInfo("@Epm_PayClass", drPos["Epm_PayClass"]);
            pPosMovement[9] = new ParameterInfo("@Epm_ReasonForMovement", drPos["Epm_ReasonForMovement"]);
            pPosMovement[10] = new ParameterInfo("@Usr_Login", drPos["Usr_Login"]);
            pPosMovement[11] = new ParameterInfo("@Ludatetime", drPos["Ludatetime"]);

            #endregion

            #region T_EmpPosition Insert Query

            string qPosMovement = @"INSERT INTO T_EmpPosition 
                                                (Tpo_IDNo, 
                                                 Tep_StartDate,
                                                 Tep_EndDate,
                                                 Epm_PositionCode,
                                                 Epm_JobLevel,
                                                 Epm_PositionCategory,
                                                 Epm_Classification,
                                                 Epm_JobGrade,
                                                 Epm_PayClass,
                                                 Epm_ReasonForMovement,
                                                 Usr_Login, 
                                                 Ludatetime) 
                                               VALUES
                                                (@Tpo_IDNo, 
                                                 @Tep_StartDate, 
                                                 @Tep_EndDate,
                                                 @Epm_PositionCode,
                                                 @Epm_JobLevel,
                                                 @Epm_PositionCategory,
                                                 @Epm_Classification,
                                                 @Epm_JobGrade,
                                                 @Epm_PayClass,
                                                 @Epm_ReasonForMovement,
                                                 @Usr_Login, 
                                                 GetDate())";
            #endregion

            #endregion

            #region T_EmpEmploymentStatus

            #region T_EmpEmploymentStatus Parameters

            ParameterInfo[] pEmpStat = new ParameterInfo[4];
            pEmpStat[0] = new ParameterInfo("@Tes_IDNo", drEmpStat["Tes_IDNo"]);
            pEmpStat[1] = new ParameterInfo("@Tes_EmploymentStatusCode", drEmpStat["Tes_EmploymentStatusCode"]);
            pEmpStat[2] = new ParameterInfo("@Usr_Login", drEmpStat["Usr_Login"]);
            pEmpStat[3] = new ParameterInfo("@Tes_ReasonCode", drEmpStat["Tes_ReasonCode"]);

            #endregion

            #region T_EmpEmploymentStatus Insert Query

            string qEmpStat = @" DECLARE @EffectivityDate datetime
                                              SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                    FROM T_PaySchedule
                                                                    WHERE Tps_CycleIndicator = 'C')

                                       INSERT INTO T_EmpEmploymentStatus
                                           (Tes_IDNo
                                           ,Tes_StartDate
                                           ,Tes_EmploymentStatusCode
                                           ,Tes_ReasonCode
                                           ,Usr_Login
                                           ,Ludatetime)
                                        VALUES
                                           (@Tes_IDNo
                                           ,@EffectivityDate
                                           ,@Tes_EmploymentStatusCode
                                           ,@Tes_ReasonCode
                                           ,@Usr_Login
                                           ,Getdate())";

            #endregion
            #endregion

            #region PROFILE T_EmpProfile

            #region PROFILE T_EmpProfile Parameters

            ParameterInfo[] pProfMovement = new ParameterInfo[6];
            pProfMovement[0] = new ParameterInfo("@Tep_IDNo", drProfMovement["Tep_IDNo"]);
            pProfMovement[1] = new ParameterInfo("@Tep_SourceProfile", drProfMovement["Tep_SourceProfile"]);
            pProfMovement[2] = new ParameterInfo("@Tep_TargetProfile", drProfMovement["Tep_TargetProfile"]);
            pProfMovement[3] = new ParameterInfo("@Tep_WorkStatus", drProfMovement["Tep_WorkStatus"]);
            pProfMovement[4] = new ParameterInfo("@Tep_ReasonCode", drProfMovement["Tep_ReasonCode"]);
            pProfMovement[5] = new ParameterInfo("@Usr_Login", drProfMovement["Usr_Login"]);

            #endregion

            #region PROFILE T_EmpProfile Insert Query

            string qProfMovement = string.Format(@" DECLARE @EffectivityDate datetime
                                                  SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                        FROM T_PaySchedule
                                                                        WHERE Tps_CycleIndicator = 'C')

                                                       INSERT INTO {0}..T_EmpProfile
                                                           (Tep_IDNo
                                                           ,Tep_StartDate
                                                           ,Tep_SourceProfile
                                                           ,Tep_TargetProfile
                                                           ,Tep_WorkStatus
                                                           ,Tep_ReasonCode
                                                           ,Tep_IsLatestProfile
                                                           ,Usr_Login
                                                           ,Ludatetime)
                                                        VALUES
                                                           (@Tep_IDNo
                                                           ,@EffectivityDate
                                                           ,@Tep_SourceProfile
                                                           ,@Tep_TargetProfile
                                                           ,@Tep_WorkStatus
                                                           ,@Tep_ReasonCode
                                                           ,1
                                                           ,@Usr_Login
                                                           ,Getdate())", CentralProfile);

            #endregion

            #endregion

            #region T_EmpSalary

            #region T_EmpSalary Parameters

            ParameterInfo[] pSalaryMov = new ParameterInfo[9];
            pSalaryMov[0] = new ParameterInfo("@Tsl_IDNo", drSalaryMov["Tsl_IDNo"]);
            pSalaryMov[1] = new ParameterInfo("@Tsl_StartDate", drSalaryMov["Tsl_StartDate"]);
            pSalaryMov[2] = new ParameterInfo("@Tsl_EndDate", drSalaryMov["Tsl_EndDate"]);
            pSalaryMov[3] = new ParameterInfo("@Tsl_SalaryRate", drSalaryMov["Tsl_SalaryRate"]);
            pSalaryMov[4] = new ParameterInfo("@Tsl_PayrollType", drSalaryMov["Tsl_PayrollType"]);
            pSalaryMov[5] = new ParameterInfo("@Usr_Login", drSalaryMov["Usr_Login"]);
            pSalaryMov[6] = new ParameterInfo("@Tsl_SalaryType", drSalaryMov["Tsl_SalaryType"]);
            pSalaryMov[7] = new ParameterInfo("@Tsl_ReasonCode", drSalaryMov["Tsl_ReasonCode"]);
            pSalaryMov[8] = new ParameterInfo("@Tsl_CurrencyCode", drSalaryMov["Tsl_CurrencyCode"]);

            #endregion

            #region T_EmpSalary Insert Query

            string qSalaryMov = @"INSERT INTO T_EmpSalary
                                    (
                                        Tsl_IDNo
                                        , Tsl_StartDate
                                        , Tsl_EndDate
                                        , Tsl_SalaryRate
                                        , Tsl_PayrollType
                                        , Usr_Login
                                        , Ludatetime
                                        , Tsl_SalaryType
                                        , Tsl_ReasonCode
                                        , Tsl_CurrencyCode
                                    )
                                    VALUES
                                    (
                                        @Tsl_IDNo
                                        , @Tsl_StartDate
                                        , @Tsl_EndDate
                                        , @Tsl_SalaryRate
                                        , @Tsl_PayrollType
                                        , @Usr_Login
                                        , GETDATE()
                                        , @Tsl_SalaryType
                                        , @Tsl_ReasonCode
                                        , @Tsl_CurrencyCode
                                    )";

            #endregion

            #endregion

            #region  T_EmpCalendarGroup

            #region T_EmpCalendarGroup Parameters

            ParameterInfo[] pEmployeeGrp = new ParameterInfo[5];
            pEmployeeGrp[0] = new ParameterInfo("@Tcg_IDNo", drEmployeeGrp["Tcg_IDNo"]);
            pEmployeeGrp[1] = new ParameterInfo("@Tcg_CalendarType", drEmployeeGrp["Tcg_CalendarType"]);
            pEmployeeGrp[2] = new ParameterInfo("@Tcg_CalendarGroup", drEmployeeGrp["Tcg_CalendarGroup"]);
            pEmployeeGrp[3] = new ParameterInfo("@Tcg_ReasonCode", drEmployeeGrp["Tcg_ReasonCode"]);
            pEmployeeGrp[4] = new ParameterInfo("@Usr_Login", drEmployeeGrp["Usr_Login"]);

            #endregion

            #region T_EmpCalendarGroup Insert Query
            string qEmployeeGrp = @" DECLARE @EffectivityDate datetime
                                  SET @EffectivityDate = (SELECT Tps_StartCycle
                                                        FROM T_PaySchedule
                                                        WHERE Tps_CycleIndicator = 'C')

                                       INSERT INTO T_EmpCalendarGroup
                                           (Tcg_IDNo
                                           ,Tcg_StartDate
                                           ,Tcg_CalendarType
                                           ,Tcg_CalendarGroup
                                           ,Tcg_ReasonCode
                                           ,Usr_Login
                                           ,Ludatetime)
                                        VALUES
                                           (@Tcg_IDNo
                                           ,@EffectivityDate
                                           ,@Tcg_CalendarType
                                           ,@Tcg_CalendarGroup
                                           ,@Tcg_ReasonCode
                                           ,@Usr_Login
                                           ,Getdate())";
            #endregion

            #endregion

            #region M_UserHdr

            #region M_UserHdr Parameters
            ParameterInfo[] pUserMaster = new ParameterInfo[12];
            pUserMaster[0] = new ParameterInfo("@Muh_UserCode", drUserMaster["Muh_UserCode"]);
            pUserMaster[1] = new ParameterInfo("@Muh_Password", drUserMaster["Muh_Password"]);
            pUserMaster[2] = new ParameterInfo("@Muh_LastName", drUserMaster["Muh_LastName"]);
            pUserMaster[3] = new ParameterInfo("@Muh_FirstName", drUserMaster["Muh_FirstName"]);
            pUserMaster[4] = new ParameterInfo("@Muh_MiddleName", drUserMaster["Muh_MiddleName"]);
            pUserMaster[5] = new ParameterInfo("@Muh_NickName", drUserMaster["Muh_NickName"]);
            pUserMaster[6] = new ParameterInfo("@Muh_JobTitle", drUserMaster["Muh_JobTitle"]);
            pUserMaster[7] = new ParameterInfo("@Muh_EmailAddress", drUserMaster["Muh_EmailAddress"]);
            pUserMaster[8] = new ParameterInfo("@Muh_CanViewRate", drUserMaster["Muh_CanViewRate"]);
            pUserMaster[9] = new ParameterInfo("@Muh_RecordStatus", drUserMaster["Muh_RecordStatus"]);
            pUserMaster[10] = new ParameterInfo("@Usr_Login", drUserMaster["Usr_Login"]);
            pUserMaster[11] = new ParameterInfo("@Muh_CanConsolidateReport", drUserMaster["Muh_CanConsolidateReport"]);
            #endregion

            #region M_UserHdr Insert Query
            string qUserMaster = @"INSERT INTO M_UserHdr
                                    (
                                        Muh_UserCode
                                        , Muh_Password
                                        , Muh_LastName
                                        , Muh_FirstName
                                        , Muh_MiddleName
                                        , Muh_NickName
                                        , Muh_JobTitle
                                        , Muh_EmailAddress
                                        , Muh_CanViewRate
                                        , Muh_RecordStatus
                                        , Usr_Login
                                        , ludatetime
                                        , Muh_CanConsolidateReport
                                        , Muh_EffectivityDate
                                    )
                                    VALUES
                                    (
                                        @Muh_UserCode
                                        , @Muh_Password
                                        , @Muh_LastName
                                        , @Muh_FirstName
                                        , @Muh_MiddleName
                                        , @Muh_NickName
                                        , @Muh_JobTitle
                                        , @Muh_EmailAddress
                                        , @Muh_CanViewRate
                                        , @Muh_RecordStatus
                                        , @Usr_Login
                                        , GETDATE()
                                        , @Muh_CanConsolidateReport
                                        , GETDATE()
                                    )";
            #endregion

            #region PROFILE M_UserHdr Parameters

            ParameterInfo[] pPUserMaster = new ParameterInfo[10];
            pPUserMaster[0] = new ParameterInfo("@Muh_UserCode", drPUserMaster["Muh_UserCode"]);
            pPUserMaster[1] = new ParameterInfo("@Muh_EffectivityDate", drPUserMaster["Muh_EffectivityDate"]);
            pPUserMaster[2] = new ParameterInfo("@Muh_Password", drPUserMaster["Muh_Password"]);
            pPUserMaster[3] = new ParameterInfo("@Muh_LastName", drPUserMaster["Muh_LastName"]);
            pPUserMaster[4] = new ParameterInfo("@Muh_FirstName", drPUserMaster["Muh_FirstName"]);
            pPUserMaster[5] = new ParameterInfo("@Muh_MiddleName", drPUserMaster["Muh_MiddleName"]);
            pPUserMaster[6] = new ParameterInfo("@Muh_NickName", drPUserMaster["Muh_NickName"]);
            pPUserMaster[7] = new ParameterInfo("@Muh_EmailAddress", drPUserMaster["Muh_EmailAddress"]);
            pPUserMaster[8] = new ParameterInfo("@Muh_RecordStatus", drPUserMaster["Muh_RecordStatus"]);
            pPUserMaster[9] = new ParameterInfo("@Usr_Login", drPUserMaster["Usr_Login"]);

            #endregion

            #region PROFILE M_UserHdr Insert Query
            string qPUserMaster = string.Format(@"INSERT INTO {0}..M_UserHdr
                                        (
                                            Muh_UserCode
                                            , Muh_EffectivityDate
                                            , Muh_Password
                                            , Muh_LastName
                                            , Muh_FirstName
                                            , Muh_MiddleName
                                            , Muh_NickName
                                            , Muh_EmailAddress
                                            , Muh_RecordStatus
                                            , Usr_Login
                                            , Ludatetime
                                        )
                                        VALUES
                                        (
                                            @Muh_UserCode
                                            , GETDATE()
                                            , @Muh_Password
                                            , @Muh_LastName
                                            , @Muh_FirstName
                                            , @Muh_MiddleName
                                            , @Muh_NickName
                                            , @Muh_EmailAddress
                                            , @Muh_RecordStatus
                                            , @Usr_Login
                                            , GETDATE()
                                        )", CentralProfile);
            #endregion

            #endregion

            #region DTR T_LogMaster

            #region DTR T_LogMaster Parameters
            ParameterInfo[] pLogMaster = new ParameterInfo[11];
            pLogMaster[0] = new ParameterInfo("@Lmt_EmployeeID", drLogMaster["Lmt_EmployeeID"]);
            pLogMaster[1] = new ParameterInfo("@Lmt_Lastname", drLogMaster["Lmt_Lastname"]);
            pLogMaster[2] = new ParameterInfo("@Lmt_Firstname", drLogMaster["Lmt_Firstname"]);
            pLogMaster[3] = new ParameterInfo("@Lmt_Middlename", drLogMaster["Lmt_Middlename"]);
            pLogMaster[4] = new ParameterInfo("@Lmt_CostCenterDesc", drLogMaster["Lmt_CostCenterDesc"]);
            pLogMaster[5] = new ParameterInfo("@Lmt_PositionDesc", drLogMaster["Lmt_PositionDesc"]);
            pLogMaster[6] = new ParameterInfo("@Lmt_Gender", drLogMaster["Lmt_Gender"]);
            pLogMaster[7] = new ParameterInfo("@Usr_Login", drLogMaster["Usr_Login"]);
            pLogMaster[8] = new ParameterInfo("@Lmt_Nickname", drLogMaster["Lmt_Nickname"]);
            pLogMaster[9] = new ParameterInfo("@Lmt_Picture", drLogMaster["Lmt_Picture"], SqlDbType.Image, imageSize);
            pLogMaster[10] = new ParameterInfo("@Lmt_BirthDate", drLogMaster["Lmt_BirthDate"]);
            #endregion

            #region PROFILE T_LogMaster Insert Query
            string qLogMaster = string.Format(@"INSERT INTO {0}..T_LogMaster
                                                       (Lmt_EmployeeID
                                                       ,Lmt_BarcodeID
                                                       ,Lmt_Lastname
                                                       ,Lmt_Firstname
                                                       ,Lmt_Middlename
                                                       ,Lmt_Nickname --added by kevin 08072009
                                                       ,Lmt_CostCenterDesc
                                                       ,Lmt_PositionDesc
                                                       ,Lmt_Gender
                                                       ,Lmt_LastLogType 
	                                                   ,Lmt_IDPrintCtr 
	                                                   ,Lmt_IDPrintedBy 
	                                                   ,Lmt_ID_PrintedDate 
                                                       ,Lmt_Status
                                                       ,Usr_Login
                                                       ,LudateTime
                                                        ,Lmt_SwipeMailAlert
                                                        ,Lmt_SwipeMailAlertRem
                                                        ,Lmt_SwipeMailAlertBy
                                                        ,Lmt_SwipeMailAlertDate
                                                        ,Lmt_Hold
                                                        ,Lmt_HolRem
                                                        ,Lmt_HoldBy
                                                        ,Lmt_HoldDate
                                                        ,Lmt_Picture
                                                        ,Lmt_BirthDate)
                                                 VALUES
                                                       (@Lmt_EmployeeID
                                                       ,@Lmt_EmployeeID
                                                       ,@Lmt_Lastname
                                                       ,@Lmt_Firstname
                                                       ,@Lmt_Middlename
                                                       ,@Lmt_Nickname --added by kevin 08072009
                                                       ,@Lmt_CostCenterDesc
                                                       ,@Lmt_PositionDesc
                                                       ,@Lmt_Gender
                                                       ,'O'
	                                                   ,null
                                                       ,null
                                                       ,null
                                                       ,'A'
                                                       ,@Usr_Login
                                                       ,GetDate()
                                                        ,'False'
                                                        ,null
                                                        ,null
                                                        ,null
                                                        ,null
                                                        ,null
                                                        ,null
                                                        ,null
                                                        ,@Lmt_Picture
                                                        ,@Lmt_BirthDate)", DTRDB);
            #endregion

            #endregion

            #region T_EmpRest

            #region T_EmpRest Parameters
            ParameterInfo[] pEmployeeRestDay = new ParameterInfo[3];
            pEmployeeRestDay[0] = new ParameterInfo("@Ter_IDNo", drEmployeeRestDay["Ter_IDNo"]);
            pEmployeeRestDay[1] = new ParameterInfo("@Ter_RestDayIndic", drEmployeeRestDay["Ter_RestDayIndic"]);
            pEmployeeRestDay[2] = new ParameterInfo("@Usr_Login", drEmployeeRestDay["Usr_Login"]);
            #endregion

            #region T_EmpRest Insert Query
            string qEmployeeRestDay = @" DECLARE @EffectivityDate datetime
                                SET @EffectivityDate = (SELECT Tps_StartCycle
                                                        FROM T_PaySchedule
                                                        WHERE Tps_CycleIndicator = 'C')

                                    INSERT INTO T_EmpRest
                                           (Ter_IDNo
                                           ,Ter_StartDate
                                           ,Ter_RestDayIndic
                                           ,Usr_Login
                                           ,Ludatetime)
                                     VALUES
                                           (@Ter_IDNo
                                           ,@EffectivityDate
                                           ,@Ter_RestDayIndic
                                           ,@Usr_Login
                                           ,GetDate())";

            #endregion

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();

                try
                {
                    dal.ExecuteNonQuery(qEmployeeMaster, CommandType.Text, pEmployeeMaster); 
                    dal.ExecuteNonQuery(qPEmployeeMaster, CommandType.Text, pEmployeeMaster); 
                    dal.ExecuteNonQuery(qEmployeeRestDay, CommandType.Text, pEmployeeRestDay); 
                    dal.ExecuteNonQuery(qEmployeeWorkLoc, CommandType.Text, pEmployeeWorkLoc); 
                    dal.ExecuteNonQuery(qEmpCstCnterMvment, CommandType.Text, pEmployeeCstCntrMvment); 
                    dal.ExecuteNonQuery(qPremGroup, CommandType.Text, pPremGroup); 
                    dal.ExecuteNonQuery(qPosMovement, CommandType.Text, pPosMovement); 
                    dal.ExecuteNonQuery(qEmpStat, CommandType.Text, pEmpStat); 
                    dal.ExecuteNonQuery(qProfMovement, CommandType.Text, pProfMovement); 
                    dal.ExecuteNonQuery(qSalaryMov, CommandType.Text, pSalaryMov); 
                    dal.ExecuteNonQuery(qEmployeeGrp, CommandType.Text, pEmployeeGrp); 
                    dal.ExecuteNonQuery(qUserMaster, CommandType.Text, pUserMaster); 
                    dal.ExecuteNonQuery(qPUserMaster, CommandType.Text, pPUserMaster); 
                    dal.ExecuteNonQuery(qLogMaster, CommandType.Text, pLogMaster);
                    if (isIDAutoGenerated)
                        UpdateIDNumberAutoGenSeries(LoginInfo.getUser().UserCode, transactionCode, dal);

                    CorrectLogLedgerRec(row["Mem_IDNo"].ToString().Trim(), dal);

                    //"Copy" function
                    if (strCopiedFromID != "")
                        InsertIDTrail(strCopiedFromID, row["Mem_IDNo"].ToString(), LoginInfo.getUser().UserCode, dal);
                    bSuccess = 1;
                    dal.CommitTransaction();
                }
                catch (Exception e)
                {
                    bSuccess = 0;
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                    dal.RollBackTransaction();

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return bSuccess;
        }

        public bool IsCurrentDB(string EmployeeID, string DatabaseNo, string CentralProfile)
        {
            string query = string.Format(@"SELECT CASE WHEN Tep_TargetProfile = '{0}'
                                              THEN 1
                                              ELSE 0
                                              END FROM T_EmpProfile
                                            WHERE Tep_IDNo = '{1}'
                                            AND Tep_IsLatestProfile = 1", DatabaseNo, EmployeeID);
            DataSet ds;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
            else return true;
        }

        public void ModifyEmployee(DataRow drEmployeeMaster, DataRow drOrigEmployeeMaster, DataRow drCostCnterMvmnt, DataRow drPremGroup, DataRow drPos, DataRow drEmpStat, DataRow drSalaryMov, DataRow drUserMaster, DataRow drPUserMaster, DataRow drLogMaster, DataRow drGroup, DataRow drJobStat, DataRow drWorkLoc, int imageSize)
        {
            string CentralProfile = LoginInfo.getUser().CentralProfileName;
            string DTRDB = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());
            bool isHireDateReadOnly = CheckAllowEditHireDate(Convert.ToDateTime(drEmployeeMaster["Mem_IntakeDate"]));

            #region string initialization
            string qUpdateRecentCstCnterMvment = "";
            string qDeleteCostCenter = "";
            string qInsertCostCnter = "";

            string qUpdateRecentPremGroup = "";
            string qDeletePremGroup = "";
            string qInsertPremGroup = "";

            string qUpdateEmpStat = "";
            string qDeleteEmpStat = "";
            string qInsertEmpStat = "";

            string qUpdatePosMov = "";
            string qDeletePosMov = "";
            string qInsertPosMov = "";

            string qUpdateSalMov = "";
            string qDeleteSalMov = "";
            string qInsertSalMov = "";

            string qUpdateGroup = "";
            string qDeleteGroup = "";
            string qInsertGroup = "";

            string qUpdateJobStat = "";
            string qDeleteJobStat = "";
            string qInsertJobStat = "";

            string qUpdateWorkLoc = "";
            string qDeleteWorkLoc = "";
            string qInsertWorkLoc = "";
            #endregion

            #region parameterinfo[] initialization
            ParameterInfo[] pUpdteRcentCstCnter = null;
            ParameterInfo[] pDeleteCstCnter = null;
            ParameterInfo[] pInsertCstCenter = null;

            ParameterInfo[] pUpdateRecentPremGroup = null;
            ParameterInfo[] pDeletePremGroup = null;
            ParameterInfo[] pInsertPremGroup = null;

            ParameterInfo[] pUpdateEmpStat = null;
            ParameterInfo[] pDeleteEmpStat = null;
            ParameterInfo[] pInsertEmpStat = null;

            ParameterInfo[] pUpdatePosMov = null;
            ParameterInfo[] pDeletePosMov = null;
            ParameterInfo[] pInsertPosMov = null;

            ParameterInfo[] pUpdateSalMov = null;
            ParameterInfo[] pDeleteSalMov = null;
            ParameterInfo[] pInsertSalMov = null;

            ParameterInfo[] pUpdateGroup = null;
            ParameterInfo[] pDeleteGroup = null;
            ParameterInfo[] pInsertGroup = null;

            ParameterInfo[] pUpdateJobStat = null;
            ParameterInfo[] pDeleteJobStat = null;
            ParameterInfo[] pInsertJobStat = null;

            ParameterInfo[] pUpdateWorkLoc = null;
            ParameterInfo[] pDeleteWorkLoc = null;
            ParameterInfo[] pInsertWorkLoc = null;
            #endregion

            #region M_Employee

            #region M_Employee Parameters

            ParameterInfo[] pEmployeeMaster = new ParameterInfo[102];
            pEmployeeMaster[0] = new ParameterInfo("@Mem_IDNo", drEmployeeMaster["Mem_IDNo"]);
            pEmployeeMaster[1] = new ParameterInfo("@Mem_LastName", drEmployeeMaster["Mem_LastName"]);
            pEmployeeMaster[2] = new ParameterInfo("@Mem_FirstName", drEmployeeMaster["Mem_FirstName"]);
            pEmployeeMaster[3] = new ParameterInfo("@Mem_MiddleName", drEmployeeMaster["Mem_MiddleName"]);
            pEmployeeMaster[4] = new ParameterInfo("@Mem_MaidenName", drEmployeeMaster["Mem_MaidenName"]);
            pEmployeeMaster[5] = new ParameterInfo("@Mem_BirthDate", drEmployeeMaster["Mem_BirthDate"]);
            pEmployeeMaster[6] = new ParameterInfo("@Mem_BirthPlace", drEmployeeMaster["Mem_BirthPlace"]);
            pEmployeeMaster[7] = new ParameterInfo("@Mem_Age", drEmployeeMaster["Mem_Age"]);
            pEmployeeMaster[8] = new ParameterInfo("@Mem_Gender", drEmployeeMaster["Mem_Gender"]);
            pEmployeeMaster[9] = new ParameterInfo("@Mem_CivilStatusCode", drEmployeeMaster["Mem_CivilStatusCode"]);
            pEmployeeMaster[10] = new ParameterInfo("@Mem_MarriedDate", drEmployeeMaster["Mem_MarriedDate"]);
            pEmployeeMaster[11] = new ParameterInfo("@Mem_NationalityCode", drEmployeeMaster["Mem_NationalityCode"]);
            pEmployeeMaster[12] = new ParameterInfo("@Mem_ReligionCode", drEmployeeMaster["Mem_ReligionCode"]);
            pEmployeeMaster[13] = new ParameterInfo("@Mem_PresCompleteAddress", drEmployeeMaster["Mem_PresCompleteAddress"], SqlDbType.VarChar, 200);
            pEmployeeMaster[14] = new ParameterInfo("@Mem_PresAddressBarangay", drEmployeeMaster["Mem_PresAddressBarangay"]);
            pEmployeeMaster[15] = new ParameterInfo("@Mem_PresAddressMunicipalityCity", drEmployeeMaster["Mem_PresAddressMunicipalityCity"]);
            pEmployeeMaster[16] = new ParameterInfo("@Mem_LandlineNo", drEmployeeMaster["Mem_LandlineNo"]);
            pEmployeeMaster[17] = new ParameterInfo("@Mem_CellNo", drEmployeeMaster["Mem_CellNo"]);
            pEmployeeMaster[18] = new ParameterInfo("@Mem_OfficeEmailAddress", drEmployeeMaster["Mem_OfficeEmailAddress"]);
            pEmployeeMaster[19] = new ParameterInfo("@Mem_EducationCode", drEmployeeMaster["Mem_EducationCode"]);
            pEmployeeMaster[20] = new ParameterInfo("@Mem_SchoolCode", drEmployeeMaster["Mem_SchoolCode"]);
            pEmployeeMaster[21] = new ParameterInfo("@Mem_CourseCode", drEmployeeMaster["Mem_CourseCode"]);
            pEmployeeMaster[22] = new ParameterInfo("@Mem_BloodType", drEmployeeMaster["Mem_BloodType"]);
            pEmployeeMaster[23] = new ParameterInfo("@Mem_SSSNo", drEmployeeMaster["Mem_SSSNo"]);
            pEmployeeMaster[24] = new ParameterInfo("@Mem_PhilhealthNo", drEmployeeMaster["Mem_PhilhealthNo"]);
            pEmployeeMaster[25] = new ParameterInfo("@Mem_PayrollBankCode", drEmployeeMaster["Mem_PayrollBankCode"]);
            pEmployeeMaster[26] = new ParameterInfo("@Mem_BankAcctNo", drEmployeeMaster["Mem_BankAcctNo"]);
            pEmployeeMaster[27] = new ParameterInfo("@Mem_TIN", drEmployeeMaster["Mem_TIN"]);
            pEmployeeMaster[28] = new ParameterInfo("@Mem_TaxCode", drEmployeeMaster["Mem_TaxCode"]);
            pEmployeeMaster[29] = new ParameterInfo("@Mem_PagIbigNo", drEmployeeMaster["Mem_PagIbigNo"]);
            pEmployeeMaster[30] = new ParameterInfo("@Mem_PagIbigRule", drEmployeeMaster["Mem_PagIbigRule"]);
            pEmployeeMaster[31] = new ParameterInfo("@Mem_PagIbigShare", drEmployeeMaster["Mem_PagIbigShare"]);
            pEmployeeMaster[32] = new ParameterInfo("@Mem_ICEContactPerson", drEmployeeMaster["Mem_ICEContactPerson"]);
            pEmployeeMaster[33] = new ParameterInfo("@Mem_ICERelation", drEmployeeMaster["Mem_ICERelation"]);
            pEmployeeMaster[34] = new ParameterInfo("@Mem_ICECompleteAddress", drEmployeeMaster["Mem_ICECompleteAddress"], SqlDbType.VarChar, 200);
            pEmployeeMaster[35] = new ParameterInfo("@Mem_ICEAddressBarangay", drEmployeeMaster["Mem_ICEAddressBarangay"]);
            pEmployeeMaster[36] = new ParameterInfo("@Mem_ICEAddressMunicipalityCity", drEmployeeMaster["Mem_ICEAddressMunicipalityCity"]);
            pEmployeeMaster[37] = new ParameterInfo("@Mem_ICEContactNo", drEmployeeMaster["Mem_ICEContactNo"]);
            pEmployeeMaster[38] = new ParameterInfo("@Mem_CostcenterDate", drEmployeeMaster["Mem_CostcenterDate"]);
            pEmployeeMaster[39] = new ParameterInfo("@Mem_CostcenterCode", drEmployeeMaster["Mem_CostcenterCode"]);
            pEmployeeMaster[40] = new ParameterInfo("@Mem_EmploymentStatusCode", drEmployeeMaster["Mem_EmploymentStatusCode"]);
            pEmployeeMaster[41] = new ParameterInfo("@Mem_IntakeDate", drEmployeeMaster["Mem_IntakeDate"]);
            pEmployeeMaster[42] = new ParameterInfo("@Mem_RegularDate", drEmployeeMaster["Mem_RegularDate"]);
            pEmployeeMaster[43] = new ParameterInfo("@Mem_WorkLocationCode", drEmployeeMaster["Mem_WorkLocationCode"]);
            pEmployeeMaster[44] = new ParameterInfo("@Mem_JobTitleCode", drEmployeeMaster["Mem_JobTitleCode"]);
            pEmployeeMaster[45] = new ParameterInfo("@Mem_PositionDate", drEmployeeMaster["Mem_PositionDate"]);
            pEmployeeMaster[46] = new ParameterInfo("@Mem_Superior1", drEmployeeMaster["Mem_Superior1"]);
            pEmployeeMaster[47] = new ParameterInfo("@Mem_Superior2", drEmployeeMaster["Mem_Superior2"]);
            pEmployeeMaster[48] = new ParameterInfo("@Mem_IsComputedPayroll", drEmployeeMaster["Mem_IsComputedPayroll"]);
            pEmployeeMaster[49] = new ParameterInfo("@Mem_PaymentMode", drEmployeeMaster["Mem_PaymentMode"]);
            pEmployeeMaster[50] = new ParameterInfo("@Mem_PayrollType", drEmployeeMaster["Mem_PayrollType"]);
            pEmployeeMaster[51] = new ParameterInfo("@Mem_SalaryDate", drEmployeeMaster["Mem_SalaryDate"]);
            pEmployeeMaster[52] = new ParameterInfo("@Mem_Salary", drEmployeeMaster["Mem_Salary"]);
            pEmployeeMaster[53] = new ParameterInfo("@Mem_WorkStatus", drEmployeeMaster["Mem_WorkStatus"]);
            pEmployeeMaster[54] = new ParameterInfo("@Mem_IsConfidential", drEmployeeMaster["Mem_IsConfidential"]);
            pEmployeeMaster[55] = new ParameterInfo("@Mem_ShiftCode", drEmployeeMaster["Mem_ShiftCode"]);
            pEmployeeMaster[56] = new ParameterInfo("@Mem_CalendarType", drEmployeeMaster["Mem_CalendarType"]);
            pEmployeeMaster[57] = new ParameterInfo("@Mem_CalendarGroup", drEmployeeMaster["Mem_CalendarGroup"]);
            pEmployeeMaster[58] = new ParameterInfo("@Mem_SeparationNoticeDate", drEmployeeMaster["Mem_SeparationNoticeDate"]);
            pEmployeeMaster[59] = new ParameterInfo("@Mem_SeparationCode", drEmployeeMaster["Mem_SeparationCode"]);
            pEmployeeMaster[60] = new ParameterInfo("@Mem_SeparationDate", drEmployeeMaster["Mem_SeparationDate"]);

            pEmployeeMaster[61] = new ParameterInfo("@Mem_SSSRule", drEmployeeMaster["Mem_SSSRule"]);
            pEmployeeMaster[62] = new ParameterInfo("@Mem_SSSShare", drEmployeeMaster["Mem_SSSShare"]);
            pEmployeeMaster[63] = new ParameterInfo("@Mem_PHRule", drEmployeeMaster["Mem_PHRule"]);
            pEmployeeMaster[64] = new ParameterInfo("@Mem_PHShare", drEmployeeMaster["Mem_PHShare"]);

            pEmployeeMaster[65] = new ParameterInfo("@Mem_ClearedDate", drEmployeeMaster["Mem_ClearedDate"]);
            pEmployeeMaster[66] = new ParameterInfo("@Usr_Login", drEmployeeMaster["Usr_Login"]);
            pEmployeeMaster[67] = new ParameterInfo("@Mem_ProbationDate", drEmployeeMaster["Mem_ProbationDate"]);

            pEmployeeMaster[68] = new ParameterInfo("@Mem_Height", (drEmployeeMaster["Mem_Height"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_Height"]));
            pEmployeeMaster[69] = new ParameterInfo("@Mem_Weight", (drEmployeeMaster["Mem_Weight"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_Weight"]));

            pEmployeeMaster[70] = new ParameterInfo("@Mem_ProvCompleteAddress", (drEmployeeMaster["Mem_ProvCompleteAddress"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_ProvCompleteAddress"]), SqlDbType.VarChar, 200);
            pEmployeeMaster[71] = new ParameterInfo("@Mem_ProvAddressBarangay", (drEmployeeMaster["Mem_ProvAddressBarangay"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_ProvAddressBarangay"]));
            pEmployeeMaster[72] = new ParameterInfo("@Mem_ProvAddressMunicipalityCity", (drEmployeeMaster["Mem_ProvAddressMunicipalityCity"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_ProvAddressMunicipalityCity"]));
            pEmployeeMaster[73] = new ParameterInfo("@Mem_ProvLandlineNo", (drEmployeeMaster["Mem_ProvLandlineNo"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_ProvLandlineNo"]));

            pEmployeeMaster[74] = new ParameterInfo("@Mem_AwardsRecognition", (drEmployeeMaster["Mem_AwardsRecognition"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_AwardsRecognition"]));
            pEmployeeMaster[75] = new ParameterInfo("@Mem_PRCLicense", (drEmployeeMaster["Mem_PRCLicense"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_PRCLicense"]));
            pEmployeeMaster[76] = new ParameterInfo("@Mem_ExpenseClass", drEmployeeMaster["Mem_ExpenseClass"]);
            pEmployeeMaster[77] = new ParameterInfo("@Mem_NickName", (drEmployeeMaster["Mem_NickName"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_NickName"]));
            pEmployeeMaster[78] = new ParameterInfo("@Mem_AltAcctCode", (drEmployeeMaster["Mem_AltAcctCode"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_AltAcctCode"]));
            pEmployeeMaster[79] = new ParameterInfo("@Mem_WifeClaim", drEmployeeMaster["Mem_WifeClaim"]);

            pEmployeeMaster[80] = new ParameterInfo("@Mem_ShoesSize", (drEmployeeMaster["Mem_ShoesSize"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_ShoesSize"]));
            pEmployeeMaster[81] = new ParameterInfo("@Mem_ShirtSize", (drEmployeeMaster["Mem_ShirtSize"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_ShirtSize"]));
            pEmployeeMaster[82] = new ParameterInfo("@Mem_HairColor", (drEmployeeMaster["Mem_HairColor"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_HairColor"]));
            pEmployeeMaster[83] = new ParameterInfo("@Mem_EyeColor", (drEmployeeMaster["Mem_EyeColor"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_EyeColor"]));
            pEmployeeMaster[84] = new ParameterInfo("@Mem_DistinguishMark", (drEmployeeMaster["Mem_DistinguishMark"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_DistinguishMark"]));
            pEmployeeMaster[85] = new ParameterInfo("@Mem_JobGrade", (drEmployeeMaster["Mem_JobGrade"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_JobGrade"]));
            pEmployeeMaster[86] = new ParameterInfo("@Mem_PositionCategory", (drEmployeeMaster["Mem_PositionCategory"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_PositionCategory"]));
            pEmployeeMaster[87] = new ParameterInfo("@Mem_PositionClass", (drEmployeeMaster["Mem_PositionClass"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_PositionClass"]));
            pEmployeeMaster[88] = new ParameterInfo("@Mem_PremiumGrpCode", (drEmployeeMaster["Mem_PremiumGrpCode"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_PremiumGrpCode"]));
            pEmployeeMaster[89] = new ParameterInfo("@Mem_PersonalEmail", drEmployeeMaster["Mem_PersonalEmail"]);
            pEmployeeMaster[90] = new ParameterInfo("@Mem_GraduatedDate", drEmployeeMaster["Mem_GraduatedDate"]);
            pEmployeeMaster[91] = new ParameterInfo("@Mem_Contact1", (drEmployeeMaster["Mem_Contact1"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_Contact1"]));
            pEmployeeMaster[92] = new ParameterInfo("@Mem_Contact2", (drEmployeeMaster["Mem_Contact2"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_Contact2"]));
            pEmployeeMaster[93] = new ParameterInfo("@Mem_Contact3", (drEmployeeMaster["Mem_Contact3"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_Contact3"]));
            pEmployeeMaster[94] = new ParameterInfo("@Mem_RankCode", (drEmployeeMaster["Mem_RankCode"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_RankCode"]));
            pEmployeeMaster[95] = new ParameterInfo("@Mem_OldPayrollType", drEmployeeMaster["Mem_OldPayrollType"], SqlDbType.Char, 1);
            pEmployeeMaster[96] = new ParameterInfo("@Mem_OldSalaryDate", drEmployeeMaster["Mem_OldSalaryDate"]);
            pEmployeeMaster[97] = new ParameterInfo("@Mem_OldSalaryRate", drEmployeeMaster["Mem_OldSalaryRate"]);
            pEmployeeMaster[98] = new ParameterInfo("@Mem_IsTaxExempted", drEmployeeMaster["Mem_IsTaxExempted"]);
            pEmployeeMaster[99] = new ParameterInfo("@Mem_PremiumGrpDate", drEmployeeMaster["Mem_PremiumGrpDate"]);
            pEmployeeMaster[100] = new ParameterInfo("@Mem_Image", drEmployeeMaster["Mem_Image"], SqlDbType.Image, imageSize);
            pEmployeeMaster[101] = new ParameterInfo("@Mem_PayClass", (drEmployeeMaster["Mem_PayClass"].ToString().Trim() == "" ? " " : drEmployeeMaster["Mem_PayClass"]));
            #endregion

            #region M_Employee Insert Query
            string qEmployeeMaster = string.Format(@"
                                            UPDATE M_Employee 
                                               SET Mem_LastName = @Mem_LastName,
                                                Mem_FirstName = @Mem_FirstName,
                                                Mem_MiddleName = @Mem_MiddleName,
                                                Mem_MaidenName = @Mem_MaidenName,
                                                Mem_BirthDate = @Mem_BirthDate,
                                                Mem_BirthPlace = @Mem_BirthPlace,
                                                Mem_Age = @Mem_Age,
                                                Mem_Gender = @Mem_Gender,
                                                Mem_CivilStatusCode = @Mem_CivilStatusCode,
                                                Mem_MarriedDate = @Mem_MarriedDate,
                                                Mem_NationalityCode = @Mem_NationalityCode,
                                                Mem_ReligionCode = @Mem_ReligionCode,
                                                Mem_PresCompleteAddress = @Mem_PresCompleteAddress,
                                                Mem_PresAddressBarangay = @Mem_PresAddressBarangay,
                                                Mem_PresAddressMunicipalityCity = @Mem_PresAddressMunicipalityCity,
                                                Mem_LandlineNo = @Mem_LandlineNo,
                                                Mem_CellNo = @Mem_CellNo,
                                                Mem_OfficeEmailAddress = @Mem_OfficeEmailAddress,
                                                Mem_PersonalEmail = @Mem_PersonalEmail,
                                                Mem_EducationCode = @Mem_EducationCode,
                                                Mem_SchoolCode = @Mem_SchoolCode,
                                                Mem_CourseCode = @Mem_CourseCode,
                                                Mem_BloodType = @Mem_BloodType,
                                                Mem_SSSNo = @Mem_SSSNo,
                                                Mem_PhilhealthNo = @Mem_PhilhealthNo,
                                                Mem_PayrollBankCode = @Mem_PayrollBankCode,
                                                Mem_BankAcctNo = @Mem_BankAcctNo,
                                                Mem_TIN = @Mem_TIN,
                                                Mem_TaxCode = @Mem_TaxCode,
                                                Mem_PagIbigNo = @Mem_PagIbigNo,
                                                Mem_PagIbigRule = @Mem_PagIbigRule,
                                                Mem_PagIbigShare = @Mem_PagIbigShare,
                                                Mem_ICEContactPerson = @Mem_ICEContactPerson,
                                                Mem_ICERelation = @Mem_ICERelation,
                                                Mem_ICECompleteAddress = @Mem_ICECompleteAddress,
                                                Mem_ICEAddressBarangay = @Mem_ICEAddressBarangay,
                                                Mem_ICEAddressMunicipalityCity = @Mem_ICEAddressMunicipalityCity,
                                                Mem_ICEContactNo = @Mem_ICEContactNo,
                                                Mem_CostcenterDate =  @Mem_CostcenterDate,
                                                Mem_CostcenterCode = @Mem_CostcenterCode,
                                                Mem_EmploymentStatusCode = @Mem_EmploymentStatusCode,
                                                Mem_IntakeDate = @Mem_IntakeDate,
                                                Mem_RegularDate = @Mem_RegularDate,
                                                Mem_WorkLocationCode = @Mem_WorkLocationCode,
                                                Mem_JobTitleCode = @Mem_JobTitleCode,
                                                Mem_PositionDate = @Mem_PositionDate,
                                                Mem_Superior1 = @Mem_Superior1,
                                                Mem_Superior2 = @Mem_Superior2,
                                                Mem_IsComputedPayroll = @Mem_IsComputedPayroll,
                                                Mem_PaymentMode = @Mem_PaymentMode,
                                                Mem_PayrollType = @Mem_PayrollType,
                                                Mem_SalaryDate = @Mem_SalaryDate,
                                                Mem_Salary = @Mem_Salary,
                                                Mem_WorkStatus = @Mem_WorkStatus,
                                                Mem_IsConfidential = @Mem_IsConfidential,
                                                Mem_ShiftCode = @Mem_ShiftCode,
                                                Mem_CalendarType = @Mem_CalendarType,
                                                Mem_CalendarGroup = @Mem_CalendarGroup,
                                                Mem_SeparationNoticeDate = @Mem_SeparationNoticeDate,
                                                Mem_SeparationCode = @Mem_SeparationCode,
                                                Mem_SeparationDate = @Mem_SeparationDate
                                                ,Mem_SSSRule = @Mem_SSSRule
                                                ,Mem_SSSShare = @Mem_SSSShare
                                                ,Mem_PHRule = @Mem_PHRule
                                                ,Mem_PHShare = @Mem_PHShare
                                                ,Mem_ClearedDate = @Mem_ClearedDate
                                                ,Mem_ProbationDate = @Mem_ProbationDate
                                                ,Usr_Login = @Usr_Login
                                                ,Ludatetime = GetDate()
                                                ,Mem_Height = @Mem_Height
                                                ,Mem_Weight = @Mem_Weight
                                                ,Mem_ProvCompleteAddress = @Mem_ProvCompleteAddress
                                                ,Mem_ProvAddressBarangay = @Mem_ProvAddressBarangay
                                                ,Mem_ProvAddressMunicipalityCity = @Mem_ProvAddressMunicipalityCity
                                                ,Mem_ProvLandlineNo = @Mem_ProvLandlineNo
                                                ,Mem_AwardsRecognition = @Mem_AwardsRecognition
                                                ,Mem_PRCLicense = @Mem_PRCLicense
                                                ,Mem_ExpenseClass = @Mem_ExpenseClass
                                                ,Mem_NickName = @Mem_NickName
                                                ,Mem_AltAcctCode = @Mem_AltAcctCode
                                                ,Mem_WifeClaim = @Mem_WifeClaim
                                                ,Mem_ShoesSize = @Mem_ShoesSize
                                                ,Mem_ShirtSize = @Mem_ShirtSize
                                                ,Mem_HairColor = @Mem_HairColor
                                                ,Mem_EyeColor = @Mem_EyeColor
                                                ,Mem_DistinguishMark = @Mem_DistinguishMark
                                                ,Mem_JobGrade = @Mem_JobGrade
                                                ,Mem_RankCode = @Mem_RankCode
                                                ,Mem_PositionCategory = @Mem_PositionCategory
                                                ,Mem_PositionClass = @Mem_PositionClass
                                                ,Mem_PremiumGrpCode = @Mem_PremiumGrpCode
                                                ,Mem_GraduatedDate = @Mem_GraduatedDate
                                                ,Mem_Contact1 = @Mem_Contact1
                                                ,Mem_Contact2 = @Mem_Contact2
                                                ,Mem_Contact3 = @Mem_Contact3
                                                ,Mem_OldPayrollType = @Mem_OldPayrollType
                                                ,Mem_OldSalaryDate = @Mem_OldSalaryDate
                                                ,Mem_OldSalaryRate = @Mem_OldSalaryRate
                                                ,Mem_IsTaxExempted = @Mem_IsTaxExempted
                                                ,Mem_PremiumGrpDate = @Mem_PremiumGrpDate
												,Mem_Image = @Mem_Image
                                                ,Mem_PayClass = @Mem_PayClass
                                            WHERE Mem_IDNo=@Mem_IDNo");

            #endregion

            #region Profile M_Employee Parameters


            ParameterInfo[] pPEmployeeMaster = new ParameterInfo[15];
            pPEmployeeMaster[0] = new ParameterInfo("@Mem_IDNo", drEmployeeMaster["Mem_IDNo"]);
            pPEmployeeMaster[1] = new ParameterInfo("@Mem_LastName", drEmployeeMaster["Mem_LastName"]);
            pPEmployeeMaster[2] = new ParameterInfo("@Mem_FirstName", drEmployeeMaster["Mem_FirstName"]);
            pPEmployeeMaster[3] = new ParameterInfo("@Mem_MiddleName", drEmployeeMaster["Mem_MiddleName"]);
            pPEmployeeMaster[4] = new ParameterInfo("@Mem_Gender", drEmployeeMaster["Mem_Gender"]);
            pPEmployeeMaster[5] = new ParameterInfo("@Mem_CivilStatusCode", drEmployeeMaster["Mem_CivilStatusCode"]);
            pPEmployeeMaster[6] = new ParameterInfo("@Mem_OfficeEmailAddress", drEmployeeMaster["Mem_OfficeEmailAddress"]);
            pPEmployeeMaster[7] = new ParameterInfo("@Mem_CostcenterCode", drEmployeeMaster["Mem_CostcenterCode"]);
            pPEmployeeMaster[8] = new ParameterInfo("@Mem_EmploymentStatusCode", drEmployeeMaster["Mem_EmploymentStatusCode"]);
            pPEmployeeMaster[9] = new ParameterInfo("@Mem_IntakeDate", drEmployeeMaster["Mem_IntakeDate"]);
            pPEmployeeMaster[10] = new ParameterInfo("@Mem_RegularDate", drEmployeeMaster["Mem_RegularDate"]);
            pPEmployeeMaster[11] = new ParameterInfo("@Mem_JobTitleCode", drEmployeeMaster["Mem_JobTitleCode"]);
            pPEmployeeMaster[12] = new ParameterInfo("@Mem_PayrollType", drEmployeeMaster["Mem_PayrollType"]);
            pPEmployeeMaster[13] = new ParameterInfo("@Mem_WorkStatus", drEmployeeMaster["Mem_WorkStatus"]);
            pPEmployeeMaster[14] = new ParameterInfo("@Usr_Login", drEmployeeMaster["Usr_Login"]);

            #endregion

            #region Profile M_Employee Insert Query

            string qPEmployeeMaster = string.Format(@"
                UPDATE {0}..M_Employee
                    SET Mem_LastName = @Mem_LastName
                    , Mem_FirstName = @Mem_FirstName
                    , Mem_MiddleName = @Mem_MiddleName
                    , Mem_Gender = @Mem_Gender
                    , Mem_CivilStatusCode = @Mem_CivilStatusCode
                    , Mem_CostcenterCode = @Mem_CostcenterCode
                    , Mem_JobTitleCode = @Mem_JobTitleCode
                    , Mem_PayrollType = @Mem_PayrollType
                    , Mem_WorkStatus = @Mem_WorkStatus
                    , Usr_Login = @Usr_Login
                    , Mem_OfficeEmailAddress = @Mem_OfficeEmailAddress
                WHERE Mem_IDNo = @Mem_IDNo
                                                            ", CentralProfile);

            #endregion

            #endregion

            #region T_EmpCostcenter
            if (drCostCnterMvmnt != null)
            {
                #region T_EmpCostcenter Parameters


                pUpdteRcentCstCnter = new ParameterInfo[3];

                pUpdteRcentCstCnter[0] = new ParameterInfo("@EmployeeId", drCostCnterMvmnt["Tcc_IDNo"]);
                pUpdteRcentCstCnter[1] = new ParameterInfo("@StartDate", drCostCnterMvmnt["Tcc_StartDate"]);
                pUpdteRcentCstCnter[2] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);


                pInsertCstCenter = new ParameterInfo[6];

                pInsertCstCenter[0] = new ParameterInfo("@EmployeeId", drCostCnterMvmnt["Tcc_IDNo"]);
                pInsertCstCenter[1] = new ParameterInfo("@StartDate", drCostCnterMvmnt["Tcc_StartDate"]);
                pInsertCstCenter[2] = new ParameterInfo("@EndDate", DBNull.Value);
                pInsertCstCenter[3] = new ParameterInfo("@CostCenter", drCostCnterMvmnt["Tcc_CostCenterCode"]);
                pInsertCstCenter[4] = new ParameterInfo("@MovementReason", drCostCnterMvmnt["Tcc_ReasonCode"]);
                pInsertCstCenter[5] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

                pDeleteCstCnter = new ParameterInfo[1];

                pDeleteCstCnter[0] = new ParameterInfo("@EmployeeId", drCostCnterMvmnt["Tcc_IDNo"]);

                #endregion

                #region T_EmpCostcenter Insert Query

                qUpdateRecentCstCnterMvment = @"UPDATE T_EmpCostcenter
                                                   SET Tcc_EndDate = dateadd(dd, -1, @StartDate)
                                                      ,Usr_Login = @Usr_Login
                                                      ,Ludatetime = GETDATE()
                                                 WHERE Tcc_IDNo = @EmployeeId
                                                 AND Tcc_StartDate = (SELECT MAX(CostCentr.Tcc_StartDate)
					                                                    FROM T_EmpCostcenter CostCentr
					                                                    WHERE Tcc_IDNo = @EmployeeId)";

                qInsertCostCnter = string.Format(@"INSERT INTO T_EmpCostcenter
                                                       (Tcc_IDNo
                                                       ,Tcc_StartDate
                                                       ,Tcc_EndDate
                                                       ,Tcc_CostCenterCode
                                                       ,Tcc_ReasonCode
                                                       ,Usr_Login
                                                       ,Ludatetime)
                                                 VALUES
                                                       (@EmployeeId
                                                       ,@StartDate
                                                       ,@EndDate
                                                       ,@CostCenter
                                                       ,@MovementReason
                                                       ,@Usr_Login
                                                       ,GETDATE())");

                qDeleteCostCenter = @"DELETE FROM T_EmpCostcenter
                                    WHERE Tcc_IDNo = @EmployeeId";
                #endregion
            }

            #endregion

            #region T_EmpPremiumGroup

            if (drPremGroup != null)
            {
                #region T_EmpPremiumGroup Parameters

                pUpdateRecentPremGroup = new ParameterInfo[3];

                pUpdateRecentPremGroup[0] = new ParameterInfo("@EmployeeId", drPremGroup["Tpg_IDNo"]);
                pUpdateRecentPremGroup[1] = new ParameterInfo("@StartDate", drPremGroup["Tpg_StartDate"]);
                pUpdateRecentPremGroup[2] = new ParameterInfo("@Usr_Login", drPremGroup["Usr_Login"]);

                pInsertPremGroup = new ParameterInfo[6];

                pInsertPremGroup[0] = new ParameterInfo("@EmployeeId", drPremGroup["Tpg_IDNo"]);
                pInsertPremGroup[1] = new ParameterInfo("@StartDate", drPremGroup["Tpg_StartDate"]);
                pInsertPremGroup[2] = new ParameterInfo("@EndDate", DBNull.Value);
                pInsertPremGroup[3] = new ParameterInfo("@PremiumGroup", drPremGroup["Tpg_PremiumGroup"]);
                pInsertPremGroup[4] = new ParameterInfo("@MovementReason", drPremGroup["Tpg_ReasonCode"]);
                pInsertPremGroup[5] = new ParameterInfo("@Usr_Login", drPremGroup["Usr_Login"]);

                pDeletePremGroup = new ParameterInfo[1];

                pDeletePremGroup[0] = new ParameterInfo("@EmployeeId", drPremGroup["Tpg_IDNo"]);

                #endregion

                #region T_EmpPremiumGroup Insert Query

                qUpdateRecentPremGroup = string.Format(@"UPDATE T_EmpPremiumGroup
                                                   SET Tpg_EndDate = dateadd(dd, -1, @StartDate)
                                                      ,Usr_Login = @Usr_Login
                                                      ,Ludatetime = GETDATE()
                                                 WHERE Tpg_IDNo = @EmployeeId
                                                 AND Tpg_StartDate = (SELECT MAX(PremGrp.Tpg_StartDate)
					                                                    FROM T_EmpPremiumGroup PremGrp
					                                                    WHERE Tpg_IDNo = @EmployeeId)");

                qInsertPremGroup = string.Format(@"INSERT INTO T_EmpPremiumGroup
                                                   (Tpg_IDNo
                                                   ,Tpg_StartDate
                                                   ,Tpg_EndDate
                                                   ,Tpg_PremiumGroup
                                                   ,Tpg_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@PremiumGroup
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE())");

                qDeletePremGroup = string.Format(@"DELETE FROM T_EmpPremiumGroup
                                                 WHERE Tpg_IDNo = @EmployeeId");

                #endregion
            }
            #endregion

            #region T_EmpPosition

            if (drPos != null)
            {
                #region T_EmpPosition Parameters

                pUpdatePosMov = new ParameterInfo[3];
                pUpdatePosMov[0] = new ParameterInfo("@EmployeeId", drPos["Tpo_IDNo"]);
                pUpdatePosMov[1] = new ParameterInfo("@StartDate", drPos["Tep_StartDate"]);
                pUpdatePosMov[2] = new ParameterInfo("@Usr_Login", drPos["Usr_Login"]);

                pInsertPosMov = new ParameterInfo[11];
                pInsertPosMov[0] = new ParameterInfo("@EmployeeId", drPos["Tpo_IDNo"]);
                pInsertPosMov[1] = new ParameterInfo("@StartDate", drPos["Tep_StartDate"]);
                pInsertPosMov[2] = new ParameterInfo("@EndDate", DBNull.Value);
                pInsertPosMov[3] = new ParameterInfo("@Position", drPos["Epm_PositionCode"]);
                pInsertPosMov[4] = new ParameterInfo("@JobLevel", drPos["Epm_JobLevel"]);
                pInsertPosMov[5] = new ParameterInfo("@Category", drPos["Epm_PositionCategory"]);
                pInsertPosMov[6] = new ParameterInfo("@Classification", drPos["Epm_Classification"]);
                pInsertPosMov[7] = new ParameterInfo("@JobGrade", drPos["Epm_JobGrade"]);
                pInsertPosMov[8] = new ParameterInfo("@PayClass", drPos["Epm_PayClass"]);
                pInsertPosMov[9] = new ParameterInfo("@MovementReason", drPos["Epm_ReasonForMovement"]);
                pInsertPosMov[10] = new ParameterInfo("@Usr_Login", drPos["Usr_Login"]);

                pDeletePosMov = new ParameterInfo[1];
                pDeletePosMov[0] = new ParameterInfo("@EmployeeId", drPos["Tpo_IDNo"]);

                #endregion

                #region T_EmpPosition Insert Query

                qUpdatePosMov = string.Format(@"UPDATE T_EmpPosition
                                               SET Tep_EndDate = dateadd(dd, -1, @StartDate)
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Tpo_IDNo = @EmployeeId
                                             AND Tep_StartDate = (SELECT MAX(Position.Tep_StartDate)
					                                                FROM T_EmpPosition Position
					                                                WHERE Tpo_IDNo = @EmployeeId)");

                qInsertPosMov = string.Format(@"INSERT INTO T_EmpPosition
                                                   (Tpo_IDNo
                                                   ,Tep_StartDate
                                                   ,Tep_EndDate
                                                   ,Epm_PositionCode
                                                   ,Epm_JobLevel
                                                   ,Epm_PositionCategory
                                                   ,Epm_Classification
                                                   ,Epm_JobGrade
                                                   ,Epm_PayClass
                                                   ,Epm_ReasonForMovement
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@Position
                                                   ,@JobLevel
                                                   ,@Category
                                                   ,@Classification
                                                   ,@JobGrade
                                                   ,@PayClass
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE())");

                qDeletePosMov = string.Format(@"DELETE FROM T_EmpPosition
                                             WHERE Tpo_IDNo = @EmployeeId");

                #endregion
            }

            #endregion

            #region T_EmpEmploymentStatus

            if (drEmpStat != null)
            {
                #region T_EmpEmploymentStatus Parameters

                pUpdateEmpStat = new ParameterInfo[4];

                pUpdateEmpStat[0] = new ParameterInfo("@Tes_IDNo", drEmpStat["Tes_IDNo"]);
                pUpdateEmpStat[1] = new ParameterInfo("@Tes_EmploymentStatusCode", drEmpStat["Tes_EmploymentStatusCode"]);
                pUpdateEmpStat[2] = new ParameterInfo("@Usr_Login", drEmpStat["Usr_Login"]);
                pUpdateEmpStat[3] = new ParameterInfo("@Tes_ReasonCode", drEmpStat["Tes_ReasonCode"]);


                pInsertEmpStat = new ParameterInfo[4];

                pInsertEmpStat[0] = new ParameterInfo("@Tes_IDNo", drEmpStat["Tes_IDNo"]);
                pInsertEmpStat[1] = new ParameterInfo("@Tes_EmploymentStatusCode", drEmpStat["Tes_EmploymentStatusCode"]);
                pInsertEmpStat[2] = new ParameterInfo("@Usr_Login", drEmpStat["Usr_Login"]);
                pInsertEmpStat[3] = new ParameterInfo("@Tes_ReasonCode", drEmpStat["Tes_ReasonCode"]);

                pDeleteEmpStat = new ParameterInfo[1];

                pDeleteEmpStat[0] = new ParameterInfo("@Tes_IDNo", drEmpStat["Tes_IDNo"]);

                #endregion

                #region T_EmpEmploymentStatus Insert Query

                qUpdateEmpStat = @" DECLARE @EffectivityDate datetime
                                                  SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                        FROM T_PaySchedule
                                                                        WHERE Tps_CycleIndicator = 'C')
                                            
                                            IF(SELECT COUNT(*) FROM T_EmpEmploymentStatus
                                            WHERE Tes_IDNo = @Tes_IDNo
                                            AND Tes_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               UPDATE T_EmpEmploymentStatus
                                                   SET Tes_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                   ,Usr_Login = @Usr_Login
                                                   ,Ludatetime = Getdate()
                                                WHERE Tes_IDNo = @Tes_IDNo
                                                 AND Tes_StartDate = (SELECT MAX(EmpStatus.Tes_StartDate)
					                                                        FROM T_EmpEmploymentStatus EmpStatus
					                                                        WHERE Tes_IDNo = @Tes_IDNo)
                                            END";


                qInsertEmpStat = @" DECLARE @EffectivityDate datetime
                                              SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                    FROM T_PaySchedule
                                                                    WHERE Tps_CycleIndicator = 'C')

                                            IF(SELECT COUNT(*) FROM T_EmpEmploymentStatus
                                            WHERE Tes_IDNo = @Tes_IDNo
                                            AND Tes_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               INSERT INTO T_EmpEmploymentStatus
                                                   (Tes_IDNo
                                                   ,Tes_StartDate
                                                   ,Tes_EmploymentStatusCode
                                                   ,Tes_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                                VALUES
                                                   (@Tes_IDNo
                                                   ,@EffectivityDate
                                                   ,@Tes_EmploymentStatusCode
                                                   ,@Tes_ReasonCode
                                                   ,@Usr_Login
                                                   ,Getdate())
                                            END
                                            ELSE
                                                UPDATE T_EmpEmploymentStatus
                                                    SET Tes_EmploymentStatusCode = @Tes_EmploymentStatusCode
                                                    ,Tes_ReasonCode = @Tes_ReasonCode
                                                    ,Usr_Login = @Usr_Login
                                                    ,Ludatetime = Getdate()
                                                WHERE Tes_IDNo = @Tes_IDNo
                                                    AND Tes_StartDate = @EffectivityDate";

                qDeleteEmpStat = @"DELETE FROM T_EmpEmploymentStatus
                                   WHERE Tes_IDNo = @Tes_IDNo";
                #endregion
            }

            #endregion

            #region T_EmpCalendarGroup

            if (drGroup != null)
            {
                #region T_EmpCalendarGroup Parameters

                pUpdateGroup = new ParameterInfo[5];

                pUpdateGroup[0] = new ParameterInfo("@Tcg_IDNo", drGroup["Tcg_IDNo"]);
                pUpdateGroup[1] = new ParameterInfo("@Tcg_CalendarType", drGroup["Tcg_CalendarType"]);
                pUpdateGroup[2] = new ParameterInfo("@Tcg_CalendarGroup", drGroup["Tcg_CalendarGroup"]);
                pUpdateGroup[3] = new ParameterInfo("@Usr_Login", drGroup["Usr_Login"]);
                pUpdateGroup[4] = new ParameterInfo("@Tcg_ReasonCode", drGroup["Tcg_ReasonCode"]);


                pInsertGroup = new ParameterInfo[5];

                pInsertGroup[0] = new ParameterInfo("@Tcg_IDNo", drGroup["Tcg_IDNo"]);
                pInsertGroup[1] = new ParameterInfo("@Tcg_CalendarType", drGroup["Tcg_CalendarType"]);
                pInsertGroup[2] = new ParameterInfo("@Tcg_CalendarGroup", drGroup["Tcg_CalendarGroup"]);
                pInsertGroup[3] = new ParameterInfo("@Usr_Login", drGroup["Usr_Login"]);
                pInsertGroup[4] = new ParameterInfo("@Tcg_ReasonCode", drGroup["Tcg_ReasonCode"]);

                pDeleteGroup = new ParameterInfo[1];

                pDeleteGroup[0] = new ParameterInfo("@Tcg_IDNo", drGroup["Tcg_IDNo"]);

                #endregion

                #region T_EmpCalendarGroup Insert Query

                qUpdateGroup = @" DECLARE @EffectivityDate datetime
                                                  SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                        FROM T_PaySchedule
                                                                        WHERE Tps_CycleIndicator = 'C')
                                            
                                            IF(SELECT COUNT(*) FROM T_EmpCalendarGroup
                                            WHERE Tcg_IDNo = @Tcg_IDNo
                                            AND Tcg_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               UPDATE T_EmpCalendarGroup
                                                   SET Tcg_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                   ,Usr_Login = @Usr_Login
                                                   ,Ludatetime = Getdate()
                                                WHERE Tcg_IDNo = @Tcg_IDNo
                                                 AND Tcg_StartDate = (SELECT MAX(EmpGroup.Tcg_StartDate)
					                                                        FROM T_EmpCalendarGroup EmpGroup
					                                                        WHERE Tcg_IDNo = @Tcg_IDNo)
                                            END";


                qInsertGroup = @" DECLARE @EffectivityDate datetime
                                              SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                    FROM T_PaySchedule
                                                                    WHERE Tps_CycleIndicator = 'C')

                                            IF(SELECT COUNT(*) FROM T_EmpCalendarGroup
                                            WHERE Tcg_IDNo = @Tcg_IDNo
                                            AND Tcg_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               INSERT INTO T_EmpCalendarGroup
                                                   (Tcg_IDNo
                                                   ,Tcg_StartDate
                                                   ,Tcg_CalendarType
                                                   ,Tcg_CalendarGroup
                                                   ,Tcg_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                                VALUES
                                                   (@Tcg_IDNo
                                                   ,@EffectivityDate
                                                   ,@Tcg_CalendarType
                                                   ,@Tcg_CalendarGroup
                                                   ,@Tcg_ReasonCode
                                                   ,@Usr_Login
                                                   ,Getdate())
                                            END
                                            ELSE
                                                UPDATE T_EmpCalendarGroup
                                                    SET Tcg_CalendarType = @Tcg_CalendarType
                                                    ,Tcg_CalendarGroup = @Tcg_CalendarGroup
                                                    ,Tcg_ReasonCode = @Tcg_ReasonCode
                                                    ,Usr_Login = @Usr_Login
                                                    ,Ludatetime = Getdate()
                                                WHERE Tcg_IDNo = @Tcg_IDNo
                                                    AND Tcg_StartDate = @EffectivityDate";

                qDeleteGroup = @"DELETE FROM T_EmpCalendarGroup
                                   WHERE Tcg_IDNo = @Tcg_IDNo";
                #endregion
            } 

            #endregion

            #region T_EmpProfile

            if (drJobStat != null)
            {
                #region T_EmpProfile Parameters

                pUpdateJobStat = new ParameterInfo[7];

                pUpdateJobStat[0] = new ParameterInfo("@Tep_IDNo", drJobStat["Tep_IDNo"]);
                pUpdateJobStat[1] = new ParameterInfo("@Tep_WorkStatus", drJobStat["Tep_WorkStatus"]);
                pUpdateJobStat[2] = new ParameterInfo("@Tep_SourceProfile", LoginInfo.getUser().DBNumber);
                pUpdateJobStat[3] = new ParameterInfo("@Tep_TargetProfile", LoginInfo.getUser().DBNumber);
                pUpdateJobStat[4] = new ParameterInfo("@Tep_IsLatestProfile", IsCurrentDB(GetValue(drJobStat["Tep_IDNo"]), LoginInfo.getUser().DBNumber, CentralProfile));
                pUpdateJobStat[5] = new ParameterInfo("@Usr_Login", drJobStat["Usr_Login"]);
                pUpdateJobStat[6] = new ParameterInfo("@Tep_ReasonCode", drJobStat["Tcg_ReasonCode"]);


                pInsertJobStat = new ParameterInfo[7];

                pInsertJobStat[0] = new ParameterInfo("@Tep_IDNo", drJobStat["Tep_IDNo"]);
                pInsertJobStat[1] = new ParameterInfo("@Tep_WorkStatus", drJobStat["Tep_WorkStatus"]);
                pInsertJobStat[2] = new ParameterInfo("@Tep_SourceProfile", LoginInfo.getUser().DBNumber);
                pInsertJobStat[3] = new ParameterInfo("@Tep_TargetProfile", LoginInfo.getUser().DBNumber);
                pInsertJobStat[4] = new ParameterInfo("@Tep_IsLatestProfile", IsCurrentDB(GetValue(drJobStat["Tep_IDNo"]), LoginInfo.getUser().DBNumber, CentralProfile));
                pInsertJobStat[5] = new ParameterInfo("@Usr_Login", drJobStat["Usr_Login"]);
                pInsertJobStat[6] = new ParameterInfo("@Tep_ReasonCode", drJobStat["Tcg_ReasonCode"]);

                pDeleteJobStat = new ParameterInfo[1];

                pDeleteJobStat[0] = new ParameterInfo("@Tep_IDNo", drJobStat["Tep_IDNo"]);

                #endregion

                #region T_EmpProfile Insert Query

                qUpdateJobStat = string.Format(@" DECLARE @EffectivityDate datetime
                                                  SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                        FROM T_PaySchedule
                                                                        WHERE Tps_CycleIndicator = 'C')
                                            
                                            IF(SELECT COUNT(*) FROM {0}..T_EmpProfile
                                            WHERE Tep_IDNo = @Tep_IDNo
                                            AND Tep_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               UPDATE {0}..T_EmpProfile
                                                   SET Tep_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                   ,Usr_Login = @Usr_Login
                                                   ,Ludatetime = Getdate()
                                                WHERE Tep_IDNo = @Tep_IDNo
                                                 AND Epm_EffectivityDate = (SELECT MAX(JobStat.Tep_StartDate)
					                                                        FROM {0}..T_EmpProfile JobStat
					                                                        WHERE Tep_IDNo = @Tep_IDNo)
                                            END", CentralProfile);


                qInsertJobStat = string.Format(@" DECLARE @EffectivityDate datetime
                                              SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                    FROM T_PaySchedule
                                                                    WHERE Tps_CycleIndicator = 'C')

                                            IF(SELECT COUNT(*) FROM {0}..T_EmpProfile
                                            WHERE Tep_IDNo = @Tep_IDNo
                                            AND Tep_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               INSERT INTO {0}..T_EmpProfile
                                                           (Tep_IDNo
                                                           ,Tep_StartDate
                                                           ,Tep_SourceProfile
                                                           ,Tep_TargetProfile
                                                           ,Tep_WorkStatus
                                                           ,Tep_ReasonCode
                                                           ,Tep_IsLatestProfile
                                                           ,Usr_Login
                                                           ,Ludatetime)
                                                        VALUES
                                                           (@Tep_IDNo
                                                           ,@EffectivityDate
                                                           ,@Tep_SourceProfile
                                                           ,@Tep_TargetProfile
                                                           ,@Tep_WorkStatus
                                                           ,@Tep_ReasonCode
                                                           ,@Tep_IsLatestProfile
                                                           ,@Usr_Login
                                                           ,Getdate())
                                            END
                                            ELSE
                                                UPDATE {0}..T_EmpProfile
                                                    SET Tep_WorkStatus = @Tep_WorkStatus
                                                    ,Tep_ReasonCode = @Tep_ReasonCode
                                                    ,Usr_Login = @Usr_Login
                                                    ,Ludatetime = Getdate()
                                                WHERE Tep_IDNo = @Tep_IDNo
                                                    AND Tep_StartDate = @EffectivityDate", CentralProfile);

                qDeleteJobStat = string.Format(@"DELETE FROM {0}..T_EmpProfile
                                   WHERE Tep_IDNo = @Tep_IDNo", CentralProfile);
                #endregion
            }

            #endregion

            #region T_EmpWorkLocation

            if (drWorkLoc != null)
            {
                #region T_EmpWorkLocation Parameters

                pUpdateWorkLoc = new ParameterInfo[4];

                pUpdateWorkLoc[0] = new ParameterInfo("@Twl_IDNo", drWorkLoc["Twl_IDNo"]);
                pUpdateWorkLoc[1] = new ParameterInfo("@Twl_WorkLocationCode", drWorkLoc["Twl_WorkLocationCode"]);
                pUpdateWorkLoc[2] = new ParameterInfo("@Usr_Login", drWorkLoc["Usr_Login"]);
                pUpdateWorkLoc[3] = new ParameterInfo("@Twl_ReasonCode", drWorkLoc["Twl_ReasonCode"]);

                pInsertWorkLoc = new ParameterInfo[4];

                pInsertWorkLoc[0] = new ParameterInfo("@Twl_IDNo", drWorkLoc["Twl_IDNo"]);
                pInsertWorkLoc[1] = new ParameterInfo("@Twl_WorkLocationCode", drWorkLoc["Twl_WorkLocationCode"]);
                pInsertWorkLoc[2] = new ParameterInfo("@Usr_Login", drWorkLoc["Usr_Login"]);
                pInsertWorkLoc[3] = new ParameterInfo("@Twl_ReasonCode", drWorkLoc["Twl_ReasonCode"]);

                pDeleteWorkLoc = new ParameterInfo[1];

                pDeleteWorkLoc[0] = new ParameterInfo("@Twl_IDNo", drWorkLoc["Twl_IDNo"]);

                #endregion

                #region T_EmpWorkLocation Insert Query

                qUpdateWorkLoc = @" DECLARE @EffectivityDate datetime
                                                  SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                        FROM T_PaySchedule
                                                                        WHERE Tps_CycleIndicator = 'C')
                                            
                                            IF(SELECT COUNT(*) FROM T_EmpWorkLocation
                                            WHERE Twl_IDNo = @Twl_IDNo
                                            AND Twl_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               UPDATE T_EmpWorkLocation
                                                   SET Twl_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                   ,Usr_Login = @Usr_Login
                                                   ,Ludatetime = Getdate()
                                                WHERE Twl_IDNo = @Twl_IDNo
                                                 AND Twl_StartDate = (SELECT MAX(WorkLoc.Twl_StartDate)
					                                                        FROM T_EmpWorkLocation WorkLoc
					                                                        WHERE Twl_IDNo = @Twl_IDNo)
                                            END";


                qInsertWorkLoc = @" DECLARE @EffectivityDate datetime
                                              SET @EffectivityDate = (SELECT Tps_StartCycle
                                                                    FROM T_PaySchedule
                                                                    WHERE Tps_CycleType = 'C')

                                            IF(SELECT COUNT(*) FROM T_EmpWorkLocation
                                            WHERE Twl_IDNo = @Twl_IDNo
                                            AND Twl_StartDate = @EffectivityDate) <= 0
                                            BEGIN
                                               INSERT INTO T_EmpWorkLocation
                                                   (Twl_IDNo
                                                   ,Twl_StartDate
                                                   ,Twl_WorkLocationCode
                                                   ,Twl_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                                VALUES
                                                   (@Twl_IDNo
                                                   ,@EffectivityDate
                                                   ,@Twl_WorkLocationCode
                                                   ,@Twl_ReasonCode
                                                   ,@Usr_Login
                                                   ,Getdate())
                                            END
                                            ELSE
                                                UPDATE T_EmpWorkLocation
                                                    SET Twl_WorkLocationCode = @Twl_WorkLocationCode
                                                    ,Twl_ReasonCode = @Twl_ReasonCode
                                                    ,Usr_Login = @Usr_Login
                                                    ,Ludatetime = Getdate()
                                                WHERE Twl_IDNo = @Twl_IDNo
                                                    AND Twl_StartDate = @EffectivityDate";

                qDeleteWorkLoc = @"DELETE FROM T_EmpWorkLocation
                                   WHERE Twl_IDNo = @Twl_IDNo";
                #endregion
            }

            #endregion

            #region T_EmpSalary

            if (drSalaryMov != null)
            {
                #region T_EmpSalary Parameters

                pUpdateSalMov = new ParameterInfo[4];

                pUpdateSalMov[0] = new ParameterInfo("@EmployeeId", drSalaryMov["Tsl_IDNo"]);
                pUpdateSalMov[1] = new ParameterInfo("@StartDate", drSalaryMov["Tsl_StartDate"]);
                pUpdateSalMov[2] = new ParameterInfo("@PayRate", drSalaryMov["Tsl_SalaryType"]);
                pUpdateSalMov[3] = new ParameterInfo("@Usr_Login", drSalaryMov["Usr_Login"]);


                pInsertSalMov = new ParameterInfo[9];

                pInsertSalMov[0] = new ParameterInfo("@EmployeeId", drSalaryMov["Tsl_IDNo"]);
                pInsertSalMov[1] = new ParameterInfo("@StartDate", drSalaryMov["Tsl_StartDate"]);
                pInsertSalMov[2] = new ParameterInfo("@EndDate", DBNull.Value);
                pInsertSalMov[3] = new ParameterInfo("@Salary", drSalaryMov["Tsl_SalaryRate"]);
                pInsertSalMov[4] = new ParameterInfo("@PayrollType", drSalaryMov["Tsl_PayrollType"]);
                pInsertSalMov[5] = new ParameterInfo("@PayRate", drSalaryMov["Tsl_SalaryType"]);
                pInsertSalMov[6] = new ParameterInfo("@MovementReason", drSalaryMov["Tsl_ReasonCode"]);
                pInsertSalMov[7] = new ParameterInfo("@Currency", drSalaryMov["Tsl_CurrencyCode"]);
                pInsertSalMov[8] = new ParameterInfo("@Usr_Login", drSalaryMov["Usr_Login"]);

                pDeleteSalMov = new ParameterInfo[1];

                pDeleteSalMov[0] = new ParameterInfo("@EmployeeId", drSalaryMov["Tsl_IDNo"]);

                #endregion

                #region T_EmpSalary Insert Query


                qUpdateSalMov = string.Format(@"UPDATE T_EmpSalary
                                               SET Tsl_EndDate = dateadd(dd, -1, @StartDate)
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Tsl_IDNo = @EmployeeId
                                             AND Tsl_StartDate = (SELECT MAX(Salary.Tsl_StartDate)
					                                                FROM T_EmpSalary Salary
					                                                WHERE Tsl_IDNo = @EmployeeId
                                                                    AND Tsl_SalaryType = @PayRate)
                                             AND Tsl_SalaryType = @PayRate
                                            ");

                qInsertSalMov = string.Format(@"INSERT INTO T_EmpSalary
                                                   (Tsl_IDNo
                                                   ,Tsl_StartDate
                                                   ,Tsl_EndDate
                                                   ,Tsl_SalaryRate
                                                   ,Tsl_PayrollType
                                                   ,Tsl_SalaryType
                                                   ,Tsl_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime
                                                   ,Tsl_CurrencyCode)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@Salary
                                                   ,@PayrollType
                                                   ,@PayRate
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE()
                                                   ,@Currency)
                                        ");

                qDeleteSalMov = string.Format(@"DELETE FROM T_EmpSalary
                                             WHERE Tsl_IDNo = @EmployeeId");
            }

                #endregion

            #endregion

            #region M_UserHdr

            #region M_UserHdr Parameters
            ParameterInfo[] pUserMaster = new ParameterInfo[12];
            pUserMaster[0] = new ParameterInfo("@Muh_UserCode", drUserMaster["Muh_UserCode"]);
            pUserMaster[1] = new ParameterInfo("@Muh_Password", drUserMaster["Muh_Password"]);
            pUserMaster[2] = new ParameterInfo("@Muh_LastName", drUserMaster["Muh_LastName"]);
            pUserMaster[3] = new ParameterInfo("@Muh_FirstName", drUserMaster["Muh_FirstName"]);
            pUserMaster[4] = new ParameterInfo("@Muh_MiddleName", drUserMaster["Muh_MiddleName"]);
            pUserMaster[5] = new ParameterInfo("@Muh_NickName", drUserMaster["Muh_NickName"]);
            pUserMaster[6] = new ParameterInfo("@Muh_JobTitle", drUserMaster["Muh_JobTitle"]);
            pUserMaster[7] = new ParameterInfo("@Muh_EmailAddress", drUserMaster["Muh_EmailAddress"]);
            pUserMaster[8] = new ParameterInfo("@Muh_CanViewRate", drUserMaster["Muh_CanViewRate"]);
            pUserMaster[9] = new ParameterInfo("@Muh_RecordStatus", drUserMaster["Muh_RecordStatus"]);
            pUserMaster[10] = new ParameterInfo("@Usr_Login", drUserMaster["Usr_Login"]);
            pUserMaster[11] = new ParameterInfo("@Muh_CanConsolidateReport", drUserMaster["Muh_CanConsolidateReport"]);
            #endregion

            #region M_UserHdr Insert Query
            string qUserMaster = @"
    UPDATE M_UserHdr
        SET Muh_LastName = @Muh_LastName
        , Muh_FirstName = @Muh_FirstName
        , Muh_MiddleName = @Muh_MiddleName
        , Muh_JobTitle = @Muh_JobTitle
        , Usr_Login = @Usr_Login
        , ludatetime = GETDATE()
        , Muh_EmailAddress = @Muh_EmailAddress
    WHERE Muh_UserCode = @Muh_UserCode";
            #endregion

            #region PROFILE M_UserHdr Parameters

            ParameterInfo[] pPUserMaster = new ParameterInfo[10];
            pPUserMaster[0] = new ParameterInfo("@Muh_UserCode", drPUserMaster["Muh_UserCode"]);
            pPUserMaster[1] = new ParameterInfo("@Muh_EffectivityDate", drPUserMaster["Muh_EffectivityDate"]);
            pPUserMaster[2] = new ParameterInfo("@Muh_Password", drPUserMaster["Muh_Password"]);
            pPUserMaster[3] = new ParameterInfo("@Muh_LastName", drPUserMaster["Muh_LastName"]);
            pPUserMaster[4] = new ParameterInfo("@Muh_FirstName", drPUserMaster["Muh_FirstName"]);
            pPUserMaster[5] = new ParameterInfo("@Muh_MiddleName", drPUserMaster["Muh_MiddleName"]);
            pPUserMaster[6] = new ParameterInfo("@Muh_NickName", drPUserMaster["Muh_NickName"]);
            pPUserMaster[7] = new ParameterInfo("@Muh_EmailAddress", drPUserMaster["Muh_EmailAddress"]);
            pPUserMaster[8] = new ParameterInfo("@Muh_RecordStatus", drPUserMaster["Muh_RecordStatus"]);
            pPUserMaster[9] = new ParameterInfo("@Usr_Login", drPUserMaster["Usr_Login"]);

            #endregion

            #region PROFILE M_UserHdr Insert Query
            string qPUserMaster = string.Format(@"
    UPDATE {0}..M_UserHdr
        SET Muh_LastName = @Muh_LastName
        , Muh_FirstName = @Muh_FirstName
        , Muh_MiddleName = @Muh_MiddleName
        , Usr_Login = @Usr_Login
        , Ludatetime = GETDATE()
        , Muh_EmailAddress = @Muh_EmailAddress
    WHERE Muh_UserCode = @Muh_UserCode", CentralProfile);
            #endregion

            #endregion

            #region DTR T_LogMaster

            #region DTR T_LogMaster Parameters
            ParameterInfo[] pLogMaster = new ParameterInfo[11];
            pLogMaster[0] = new ParameterInfo("@Lmt_EmployeeID", drLogMaster["Lmt_EmployeeID"]);
            pLogMaster[1] = new ParameterInfo("@Lmt_Lastname", drLogMaster["Lmt_Lastname"]);
            pLogMaster[2] = new ParameterInfo("@Lmt_Firstname", drLogMaster["Lmt_Firstname"]);
            pLogMaster[3] = new ParameterInfo("@Lmt_Middlename", drLogMaster["Lmt_Middlename"]);
            pLogMaster[4] = new ParameterInfo("@Lmt_CostCenterDesc", drLogMaster["Lmt_CostCenterDesc"]);
            pLogMaster[5] = new ParameterInfo("@Lmt_PositionDesc", drLogMaster["Lmt_PositionDesc"]);
            pLogMaster[6] = new ParameterInfo("@Lmt_Gender", drLogMaster["Lmt_Gender"]);
            pLogMaster[7] = new ParameterInfo("@Usr_Login", drLogMaster["Usr_Login"]);
            pLogMaster[8] = new ParameterInfo("@Lmt_Nickname", drLogMaster["Lmt_Nickname"]);
            pLogMaster[9] = new ParameterInfo("@Lmt_Picture", drLogMaster["Lmt_Picture"], SqlDbType.Image, imageSize);
            pLogMaster[10] = new ParameterInfo("@Lmt_BirthDate", drLogMaster["Lmt_BirthDate"]);
            #endregion

            #region DTR T_LogMaster Insert Query
            string qLogMaster = string.Format(@"
UPDATE {0}..T_LogMaster
       SET Lmt_Lastname = @Lmt_Lastname
       ,Lmt_Firstname = @Lmt_Firstname
       ,Lmt_Middlename = @Lmt_Middlename
       ,Lmt_CostCenterDesc = @Lmt_CostCenterDesc
       ,Lmt_PositionDesc = @Lmt_PositionDesc
       ,Lmt_Gender = @Lmt_Gender
       ,Usr_Login = @Usr_Login
       ,LudateTime = GETDATE()
       ,Lmt_Picture = @Lmt_Picture
       ,Lmt_BirthDate = @Lmt_BirthDate
WHERE Lmt_EmployeeID = @Lmt_EmployeeID", DTRDB);
            #endregion

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();

                try
                {
                    dal.ExecuteNonQuery(qEmployeeMaster, CommandType.Text, pEmployeeMaster);
                    dal.ExecuteNonQuery(qPEmployeeMaster, CommandType.Text, pPEmployeeMaster);

                    if (drCostCnterMvmnt != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateRecentCstCnterMvment, CommandType.Text, pUpdteRcentCstCnter);
                        else
                            dal.ExecuteNonQuery(qDeleteCostCenter, CommandType.Text, pDeleteCstCnter);
                        dal.ExecuteNonQuery(qInsertCostCnter, CommandType.Text, pInsertCstCenter);
                    }

                    if (drPremGroup != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateRecentPremGroup, CommandType.Text, pUpdateRecentPremGroup);
                        else
                            dal.ExecuteNonQuery(qDeletePremGroup, CommandType.Text, pDeletePremGroup);
                        dal.ExecuteNonQuery(qInsertPremGroup, CommandType.Text, pInsertPremGroup);
                    }

                    if (drPos != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdatePosMov, CommandType.Text, pUpdatePosMov);
                        else
                            dal.ExecuteNonQuery(qDeletePosMov, CommandType.Text, pDeletePosMov);
                        dal.ExecuteNonQuery(qInsertPosMov, CommandType.Text, pInsertPosMov);
                    }

                    if (drEmpStat != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateEmpStat, CommandType.Text, pUpdateEmpStat);
                        else
                            dal.ExecuteNonQuery(qDeleteEmpStat, CommandType.Text, pDeleteEmpStat);
                        dal.ExecuteNonQuery(qInsertEmpStat, CommandType.Text, pInsertEmpStat);
                    }

                    if (drGroup != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateGroup, CommandType.Text, pUpdateGroup);
                        else
                            dal.ExecuteNonQuery(qDeleteGroup, CommandType.Text, pDeleteGroup);
                        dal.ExecuteNonQuery(qInsertGroup, CommandType.Text, pInsertGroup);
                    }

                    if (drJobStat != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateJobStat, CommandType.Text, pUpdateJobStat);
                        else
                            dal.ExecuteNonQuery(qDeleteJobStat, CommandType.Text, pDeleteJobStat);
                        dal.ExecuteNonQuery(qInsertJobStat, CommandType.Text, pInsertJobStat);
                    }

                    if (drWorkLoc != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateWorkLoc, CommandType.Text, pUpdateWorkLoc);
                        else
                            dal.ExecuteNonQuery(qDeleteWorkLoc, CommandType.Text, pDeleteWorkLoc);
                        dal.ExecuteNonQuery(qInsertWorkLoc, CommandType.Text, pInsertWorkLoc);
                    }

                    if (drSalaryMov != null)
                    {
                        if (isHireDateReadOnly)
                            dal.ExecuteNonQuery(qUpdateSalMov, CommandType.Text, pUpdateSalMov);
                        else
                            dal.ExecuteNonQuery(qDeleteSalMov, CommandType.Text, pDeleteSalMov);
                        dal.ExecuteNonQuery(qInsertSalMov, CommandType.Text, pInsertSalMov);

                        DateTime CurPayPeriodStart = new DateTime();
                        DateTime CurPayPeriodEnd = new DateTime();

                        DataSet dsResult = (new CommonBL()).GetCurrentStartEndCycleDate();
                        if (dsResult.Tables[0].Rows.Count > 0)
                        {
                            CurPayPeriodStart = Convert.ToDateTime(dsResult.Tables[0].Rows[0]["Tps_StartCycle"]);
                            CurPayPeriodEnd = Convert.ToDateTime(dsResult.Tables[0].Rows[0]["Tps_EndCycle"]);
                        }
                        (new MovementMasterBL()).UpdateEmployeeMasterSalary(drSalaryMov["Tsl_IDNo"].ToString(), CurPayPeriodStart, CurPayPeriodEnd, drSalaryMov["Usr_Login"].ToString(),"", dal);
                    }

                    dal.ExecuteNonQuery(qUserMaster, CommandType.Text, pUserMaster);
                    dal.ExecuteNonQuery(qPUserMaster, CommandType.Text, pPUserMaster);
                    dal.ExecuteNonQuery(qLogMaster, CommandType.Text, pLogMaster);

                    CheckModificationandAuditTrailAdd(drOrigEmployeeMaster
                                                        , drEmployeeMaster["Mem_CivilStatusCode"].ToString()
                                                        , drEmployeeMaster["Mem_SSSNo"].ToString()
                                                        , drEmployeeMaster["Mem_PhilhealthNo"].ToString()
                                                        , drEmployeeMaster["Mem_PayrollBankCode"].ToString()
                                                        , drEmployeeMaster["Mem_BankAcctNo"].ToString()
                                                        , drEmployeeMaster["Mem_TIN"].ToString()
                                                        , drEmployeeMaster["Mem_TaxCode"].ToString()
                                                        , drEmployeeMaster["Mem_PagIbigNo"].ToString()
                                                        , drEmployeeMaster["Mem_EmploymentStatusCode"].ToString()
                                                        , (Convert.ToBoolean(drEmployeeMaster["Mem_IsComputedPayroll"])) ? "YES" : "NO"
                                                        , drEmployeeMaster["Mem_PaymentMode"].ToString()
                                                        , drEmployeeMaster["Mem_WorkStatus"].ToString()
                                                        , drEmployeeMaster["Mem_CalendarType"].ToString()
                                                        , drEmployeeMaster["Mem_CalendarGroup"].ToString()
                                                        , drEmployeeMaster["Mem_IDNo"].ToString()
                                                        , drEmployeeMaster["Mem_SSSRule"].ToString()
                                                        , drEmployeeMaster["Mem_PagIbigRule"].ToString()
                                                        , drEmployeeMaster["Mem_PHRule"].ToString()
                                                        , (Convert.ToBoolean(drEmployeeMaster["Mem_IsTaxExempted"])) ? "YES" : "NO"
                                                        , dal);

                    if (Convert.ToBoolean(drEmployeeMaster["Mem_IsComputedPayroll"]) == false)
                    {
                        DeletePayrollRecords(new CommonBL().GetCurrentPayPeriod(), drEmployeeMaster["Mem_IDNo"].ToString(), dal);
                    }

                    dal.CommitTransaction();
                }
                catch (Exception e)
                {
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                    dal.RollBackTransaction();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        private bool CheckAllowEditHireDate(DateTime dtHireDate)
        {
            //DataSet ds;
            bool retVal = false;
            string query = string.Format(@"SELECT CASE WHEN ('{0}' >= Tps_StartCycle)
                                                THEN 0
                                                ELSE 1
                                            END
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'", dtHireDate.ToShortDateString());
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                retVal = Convert.ToBoolean(dal.ExecuteScalar(query));
                dal.CloseDB();
            }
            return retVal;
        }

        public int UpdateCompanyMasterID()
        {
            int retVal = 0;

            #region query
            string qString = @"UPDATE M_Company
                                   SET Mcm_IDNoIssued = Mcm_IDNoIssued + 1
                                      ,Usr_Login = @Usr_Login
	                                  ,ludatetime = GetDate()";
            #endregion

            ParameterInfo[] UpdateparamInfo = new ParameterInfo[1];
            UpdateparamInfo[0] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper("NON-CONFI"))
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, UpdateparamInfo);
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
            return retVal;
        }

        public bool CheckifHolidayforAddition(string HolidayDate, DALHelper dalUp)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@HolidayDate", HolidayDate);

            string sqlQuery = @"Select Convert(char(10), Thl_HolidayDate, 101) as Thl_HolidayDate
                                                        From T_Holiday
                                                        Where Thl_HolidayDate = @HolidayDate";

            ds = dalUp.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public string GetCurrentPayPeriodForAddition(DALHelper dalUp)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Tps_PayCycle From T_PaySchedule
                                                Where Tps_CycleIndicator = 'C'
                                                And Tps_RecordStatus = 'A'";

            ds = dalUp.ExecuteDataSet(sqlQuery, CommandType.Text);
            return ds.Tables[0].Rows[0][0].ToString();

        }

        private void UpdateEmployeeLogLedgerHolidayDAte(string idnumber, DALHelper dalUp)
        {
            DataSet tempds;
            string condition = string.Empty;

            string sqlQuery = @"Select Convert(char(10), Tps_StartCycle, 101) as Tps_StartCycle
		                                            ,Convert(char(10), Tps_EndCycle, 101) as Tps_EndCycle
                                            From T_PaySchedule
                                            Where Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'";

            //Gets current start and end cycle date
            tempds = dalUp.ExecuteDataSet(sqlQuery, CommandType.Text);

            if (tempds.Tables[0].Rows.Count > 0)
            {
                string strtdate = tempds.Tables[0].Rows[0][0].ToString();
                string enddate = tempds.Tables[0].Rows[0][1].ToString();

                this.UpdateHolidayDate(strtdate, enddate, idnumber, dalUp);
            }
        }

        private void UpdateHolidayDate(string strtdate, string enddate, string idnumber, DALHelper dalUp)
        {
            DataSet tempds;
            DataRow dr = DbRecord.Generate(CommonConstants.TableName.T_EmpTimeRegister);
            tempds = this.FetchHolidayRecordbyLocCode(strtdate, enddate, idnumber, dalUp);
            //1st Pass
            for (int i = 0; i < tempds.Tables[0].Rows.Count; i++)
            {
                dr["Ttr_IDNo"] = idnumber;
                dr["Ttr_Date"] = tempds.Tables[0].Rows[i][0].ToString().Trim();
                dr["Ttr_DayCode"] = tempds.Tables[0].Rows[i][1].ToString().Trim();
                dr["Ttr_HolidayFlag"] = "true";
                dr["Usr_Login"] = LoginInfo.getUser().UserCode;

                this.UpdateHOLEmployeeLogLedgerRecord(dr, dalUp);
            }
            //2nd Pass
            tempds.Clear();
            tempds = this.FetchHolidayRecordAll(strtdate, enddate, dalUp);
            for (int i = 0; i < tempds.Tables[0].Rows.Count; i++)
            {
                dr["Ttr_IDNo"] = idnumber;
                dr["Ttr_Date"] = tempds.Tables[0].Rows[i][0].ToString().Trim();
                dr["Ttr_DayCode"] = tempds.Tables[0].Rows[i][1].ToString().Trim();
                dr["Ttr_HolidayFlag"] = "true";
                dr["Usr_Login"] = LoginInfo.getUser().UserCode;

                this.UpdateHOLEmployeeLogLedgerRecord(dr, dalUp);
            }
        }

        private int UpdateHOLEmployeeLogLedgerRecord(DataRow row, DALHelper dalUp)
        {
            int retVal = 0;
            #region query
            string qString = @"UPDATE T_EmpTimeRegister
                               SET Ttr_DayCode = @Ttr_DayCode
                                  ,Ttr_HolidayFlag = @Ttr_HolidayFlag
                                  ,Usr_Login = @Usr_Login
                                  ,Ludatetime = Getdate()
                             WHERE Ttr_IDNo = @Ttr_IDNo
                              And Ttr_Date = @Ttr_Date";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Ttr_DayCode", row["Ttr_DayCode"]);
            paramInfo[1] = new ParameterInfo("@Ttr_HolidayFlag", row["Ttr_HolidayFlag"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[3] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[4] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);

            retVal = dalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            
            return retVal;
        }

        private DataSet FetchHolidayRecordbyLocCode(string startDate, string endDate, string idnumber, DALHelper dalUp)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"select CONVERT(char(10), Thl_HolidayDate, 101) as HolidayDate
		                                ,Thl_HolidayCode 
                                FROM T_Holiday
                                INNER JOIN M_Employee on Mem_IDNo = Mem_IDNo
                                   and  Thl_LocationCode = Mem_WorkLocationCode
                                Where Thl_HolidayDate between @startDate and @endDate
                                and Mem_IDNo = @Mem_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@startDate", startDate);
            paramInfo[1] = new ParameterInfo("@endDate", endDate);
            paramInfo[2] = new ParameterInfo("@Mem_IDNo", idnumber);

            ds = dalUp.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            return ds;
        }

        private DataSet FetchHolidayRecordAll(string startDate, string endDate, DALHelper dalUp)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"select CONVERT(char(10), Thl_HolidayDate, 101) as HolidayDate
		                                    ,Thl_HolidayCode 
                                    from T_Holiday
                                    where  Thl_LocationCode = 'ALL'
                                    and Thl_HolidayDate between @startDate and @endDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@startDate", startDate);
            paramInfo[1] = new ParameterInfo("@endDate", endDate);
            
            ds = dalUp.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            return ds;
        }

        public int PerformInsertandRecordSetup(DataRow row,string idnumber, string shiftcode, DataRow LogMasterdr, int ImageSize)
        {
            int retval = 0;
            bool errflag = false;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    //Insert Employee Master Record
                    retval = this.Add(row, dal, ImageSize);

                    //Create Employee Log Ledger Record
                    CorrectLogLedgerRec(idnumber, dal);
                    
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    errflag = true;
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (!errflag)
            {//Create LogMaster record
                if (!this.ExistsInLogMaster(idnumber))
                    retval = this.CreateEmpLogMasterRecord(LogMasterdr, ImageSize);
                //this.UpdateCompanyMasterID();
            }
            return retval;
        }

        private void InsertIDTrail(string strOldID, string strNewID, string UserLogin, DALHelper dal)
        {
            string query = string.Format(@"DECLARE @OldID varchar(15) = '{0}'
                                            DECLARE @NewID varchar(15) = '{1}'
                                            DECLARE @ParentID varchar(15) = @OldID

                                            INSERT INTO T_EmpIDTrail
                                                       (Tid_FromIDNo
                                                       ,Tid_ToIDNo
                                                       ,Tid_Action
		                                               ,ldm_RefID
                                                       ,Usr_Login
                                                       ,Ludatetime)
                                                 VALUES
                                                       (@OldID
                                                       ,@NewID
                                                       ,'C'
		                                               ,@NewID 
                                                       ,'{2}'
                                                       ,GETDATE())

                                            WHILE(
	                                            SELECT TOP 1 Tid_FromIDNo
	                                            FROM T_EmpIDTrail
	                                            WHERE Tid_ToIDNo = @ParentID
                                            ) IS NOT NULL
                                            BEGIN
	                                            SELECT TOP 1 @OldID = Tid_FromIDNo
	                                            FROM T_EmpIDTrail
	                                            WHERE Tid_ToIDNo = @ParentID

	                                            UPDATE T_EmpIDTrail
	                                            SET ldm_RefID = @NewID
	                                            WHERE Tid_FromIDNo = @OldID AND Tid_ToIDNo = @ParentID

	                                            SET @ParentID = @OldID
                                            END
                                            ", strOldID, strNewID, UserLogin);
            dal.ExecuteNonQuery(query);
        }

        #endregion

        #region <For Update>

        public int Update(System.Data.DataRow row, DALHelper DalUp, int ImageSize)
        {
            int retVal = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;
            //DataSet dsProfile = this.SetUpProfiles();
            string strUserMasterDB = string.Empty;

            ParameterInfo[] paramInfo = new ParameterInfo[100];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Mem_LastName", row["Mem_LastName"]);
            paramInfo[2] = new ParameterInfo("@Mem_FirstName", row["Mem_FirstName"]);
            paramInfo[3] = new ParameterInfo("@Mem_MiddleName", row["Mem_MiddleName"]);
            paramInfo[4] = new ParameterInfo("@Mem_MaidenName", row["Mem_MaidenName"]);
            paramInfo[5] = new ParameterInfo("@Mem_BirthDate", row["Mem_BirthDate"]);
            paramInfo[6] = new ParameterInfo("@Mem_BirthPlace", row["Mem_BirthPlace"]);
            paramInfo[7] = new ParameterInfo("@Mem_Age", row["Mem_Age"]);
            paramInfo[8] = new ParameterInfo("@Mem_Gender", row["Mem_Gender"]);
            paramInfo[9] = new ParameterInfo("@Mem_CivilStatusCode", row["Mem_CivilStatusCode"]);
            paramInfo[10] = new ParameterInfo("@Mem_MarriedDate", row["Mem_MarriedDate"]);
            paramInfo[11] = new ParameterInfo("@Mem_NationalityCode", row["Mem_NationalityCode"]);
            paramInfo[12] = new ParameterInfo("@Mem_ReligionCode", row["Mem_ReligionCode"]);
            paramInfo[13] = new ParameterInfo("@Mem_PresCompleteAddress", row["Mem_PresCompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[14] = new ParameterInfo("@Mem_PresAddressBarangay", row["Mem_PresAddressBarangay"]);
            paramInfo[15] = new ParameterInfo("@Mem_PresAddressMunicipalityCity", row["Mem_PresAddressMunicipalityCity"]);
            paramInfo[16] = new ParameterInfo("@Mem_LandlineNo", row["Mem_LandlineNo"]);
            paramInfo[17] = new ParameterInfo("@Mem_CellNo", row["Mem_CellNo"]);
            paramInfo[18] = new ParameterInfo("@Mem_OfficeEmailAddress", row["Mem_OfficeEmailAddress"]);
            paramInfo[19] = new ParameterInfo("@Mem_EducationCode", row["Mem_EducationCode"]);
            paramInfo[20] = new ParameterInfo("@Mem_SchoolCode", row["Mem_SchoolCode"]);
            paramInfo[21] = new ParameterInfo("@Mem_CourseCode", row["Mem_CourseCode"]);
            paramInfo[22] = new ParameterInfo("@Mem_BloodType", row["Mem_BloodType"]);
            paramInfo[23] = new ParameterInfo("@Mem_SSSNo", row["Mem_SSSNo"]);
            paramInfo[24] = new ParameterInfo("@Mem_PhilhealthNo", row["Mem_PhilhealthNo"]);
            paramInfo[25] = new ParameterInfo("@Mem_PayrollBankCode", row["Mem_PayrollBankCode"]);
            paramInfo[26] = new ParameterInfo("@Mem_BankAcctNo", row["Mem_BankAcctNo"]);
            paramInfo[27] = new ParameterInfo("@Mem_TIN", row["Mem_TIN"]);
            paramInfo[28] = new ParameterInfo("@Mem_TaxCode", row["Mem_TaxCode"]);
            paramInfo[29] = new ParameterInfo("@Mem_PagIbigNo", row["Mem_PagIbigNo"]);
            paramInfo[30] = new ParameterInfo("@Mem_PagIbigRule", row["Mem_PagIbigRule"]);
            paramInfo[31] = new ParameterInfo("@Mem_PagIbigShare", row["Mem_PagIbigShare"]);
            paramInfo[32] = new ParameterInfo("@Mem_ICEContactPerson", row["Mem_ICEContactPerson"]);
            paramInfo[33] = new ParameterInfo("@Mem_ICERelation", row["Mem_ICERelation"]);
            paramInfo[34] = new ParameterInfo("@Mem_ICECompleteAddress", row["Mem_ICECompleteAddress"], SqlDbType.VarChar, 200);
            paramInfo[35] = new ParameterInfo("@Mem_ICEAddressBarangay", row["Mem_ICEAddressBarangay"]);
            paramInfo[36] = new ParameterInfo("@Mem_ICEAddressMunicipalityCity", row["Mem_ICEAddressMunicipalityCity"]);
            paramInfo[37] = new ParameterInfo("@Mem_ICEContactNo", row["Mem_ICEContactNo"]);
            paramInfo[38] = new ParameterInfo("@Mem_CostcenterDate", row["Mem_CostcenterDate"]);
            paramInfo[39] = new ParameterInfo("@Mem_CostcenterCode", row["Mem_CostcenterCode"]);
            paramInfo[40] = new ParameterInfo("@Mem_EmploymentStatusCode", row["Mem_EmploymentStatusCode"]);
            paramInfo[41] = new ParameterInfo("@Mem_IntakeDate", row["Mem_IntakeDate"]);
            paramInfo[42] = new ParameterInfo("@Mem_RegularDate", row["Mem_RegularDate"]);
            paramInfo[43] = new ParameterInfo("@Mem_WorkLocationCode", row["Mem_WorkLocationCode"]);
            paramInfo[44] = new ParameterInfo("@Mem_JobTitleCode", row["Mem_JobTitleCode"]);
            paramInfo[45] = new ParameterInfo("@Mem_PositionDate", row["Mem_PositionDate"]);
            paramInfo[46] = new ParameterInfo("@Mem_Superior1", row["Mem_Superior1"]);
            paramInfo[47] = new ParameterInfo("@Mem_Superior2", row["Mem_Superior2"]);
            paramInfo[48] = new ParameterInfo("@Mem_IsComputedPayroll", row["Mem_IsComputedPayroll"]);
            paramInfo[49] = new ParameterInfo("@Mem_PaymentMode", row["Mem_PaymentMode"]);
            paramInfo[50] = new ParameterInfo("@Mem_PayrollType", row["Mem_PayrollType"]);
            paramInfo[51] = new ParameterInfo("@Mem_SalaryDate", row["Mem_SalaryDate"]);
            paramInfo[52] = new ParameterInfo("@Mem_Salary", row["Mem_Salary"]);
            paramInfo[53] = new ParameterInfo("@Mem_WorkStatus", row["Mem_WorkStatus"]);
            paramInfo[54] = new ParameterInfo("@Mem_IsConfidential", row["Mem_IsConfidential"]);
            paramInfo[55] = new ParameterInfo("@Mem_ShiftCode", row["Mem_ShiftCode"]);
            paramInfo[56] = new ParameterInfo("@Mem_CalendarType", row["Mem_CalendarType"]);
            paramInfo[57] = new ParameterInfo("@Mem_CalendarGroup", row["Mem_CalendarGroup"]);
            paramInfo[58] = new ParameterInfo("@Mem_SeparationNoticeDate", row["Mem_SeparationNoticeDate"]);
            paramInfo[59] = new ParameterInfo("@Mem_SeparationCode", row["Mem_SeparationCode"]);
            paramInfo[60] = new ParameterInfo("@Mem_SeparationDate", row["Mem_SeparationDate"]);

            paramInfo[61] = new ParameterInfo("@Mem_SSSRule", row["Mem_SSSRule"]);
            paramInfo[62] = new ParameterInfo("@Mem_SSSShare", row["Mem_SSSShare"]);
            paramInfo[63] = new ParameterInfo("@Mem_PHRule", row["Mem_PHRule"]);
            paramInfo[64] = new ParameterInfo("@Mem_PHShare", row["Mem_PHShare"]);

            paramInfo[65] = new ParameterInfo("@Mem_ClearedDate", row["Mem_ClearedDate"]);
            paramInfo[66] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[67] = new ParameterInfo("@Mem_ProbationDate", row["Mem_ProbationDate"]);

            paramInfo[68] = new ParameterInfo("@Mem_Height", (row["Mem_Height"].ToString().Trim() == "" ? " " : row["Mem_Height"]));
            paramInfo[69] = new ParameterInfo("@Mem_Weight", (row["Mem_Weight"].ToString().Trim() == "" ? " " : row["Mem_Weight"]));

            paramInfo[70] = new ParameterInfo("@Mem_ProvCompleteAddress", (row["Mem_ProvCompleteAddress"].ToString().Trim() == "" ? " " : row["Mem_ProvCompleteAddress"]), SqlDbType.VarChar, 200);
            paramInfo[71] = new ParameterInfo("@Mem_ProvAddressBarangay", (row["Mem_ProvAddressBarangay"].ToString().Trim() == "" ? " " : row["Mem_ProvAddressBarangay"]));
            paramInfo[72] = new ParameterInfo("@Mem_ProvAddressMunicipalityCity", (row["Mem_ProvAddressMunicipalityCity"].ToString().Trim() == "" ? " " : row["Mem_ProvAddressMunicipalityCity"]));
            paramInfo[73] = new ParameterInfo("@Mem_ProvLandlineNo", (row["Mem_ProvLandlineNo"].ToString().Trim() == "" ? " " : row["Mem_ProvLandlineNo"]));

            paramInfo[74] = new ParameterInfo("@Mem_AwardsRecognition", (row["Mem_AwardsRecognition"].ToString().Trim() == "" ? " " : row["Mem_AwardsRecognition"]));
            paramInfo[75] = new ParameterInfo("@Mem_PRCLicense", (row["Mem_PRCLicense"].ToString().Trim() == "" ? " " : row["Mem_PRCLicense"]));
            paramInfo[76] = new ParameterInfo("@Mem_ExpenseClass", row["Mem_ExpenseClass"]);
            paramInfo[77] = new ParameterInfo("@Mem_NickName", (row["Mem_NickName"].ToString().Trim() == "" ? " " : row["Mem_NickName"]));
            paramInfo[78] = new ParameterInfo("@Emt_OldEmployeeID", (row["Emt_OldEmployeeID"].ToString().Trim() == "" ? " " : row["Emt_OldEmployeeID"]));
            paramInfo[79] = new ParameterInfo("@Mem_WifeClaim", row["Mem_WifeClaim"]);

            paramInfo[80] = new ParameterInfo("@Mem_ShoesSize", (row["Mem_ShoesSize"].ToString().Trim() == "" ? " " : row["Mem_ShoesSize"]));
            paramInfo[81] = new ParameterInfo("@Mem_ShirtSize", (row["Mem_ShirtSize"].ToString().Trim() == "" ? " " : row["Mem_ShirtSize"]));
            paramInfo[82] = new ParameterInfo("@Mem_HairColor", (row["Mem_HairColor"].ToString().Trim() == "" ? " " : row["Mem_HairColor"]));
            paramInfo[83] = new ParameterInfo("@Mem_EyeColor", (row["Mem_EyeColor"].ToString().Trim() == "" ? " " : row["Mem_EyeColor"]));
            paramInfo[84] = new ParameterInfo("@Mem_DistinguishMark", (row["Mem_DistinguishMark"].ToString().Trim() == "" ? " " : row["Mem_DistinguishMark"]));
            paramInfo[85] = new ParameterInfo("@Mem_JobGrade", (row["Mem_JobGrade"].ToString().Trim() == "" ? " " : row["Mem_JobGrade"]));
            paramInfo[86] = new ParameterInfo("@Mem_PositionCategory", (row["Mem_PositionCategory"].ToString().Trim() == "" ? " " : row["Mem_PositionCategory"]));
            paramInfo[87] = new ParameterInfo("@Mem_PositionClass", (row["Mem_PositionClass"].ToString().Trim() == "" ? " " : row["Mem_PositionClass"]));
            paramInfo[88] = new ParameterInfo("@Mem_PremiumGrpCode", (row["Mem_PremiumGrpCode"].ToString().Trim() == "" ? " " : row["Mem_PremiumGrpCode"]));
            paramInfo[89] = new ParameterInfo("@Mem_PersonalEmail", row["Mem_PersonalEmail"]);
            paramInfo[90] = new ParameterInfo("@Mem_GraduatedDate", row["Mem_GraduatedDate"]);
            paramInfo[91] = new ParameterInfo("@Mem_Contact1", (row["Mem_Contact1"].ToString().Trim() == "" ? " " : row["Mem_Contact1"]));
            paramInfo[92] = new ParameterInfo("@Mem_Contact2", (row["Mem_Contact2"].ToString().Trim() == "" ? " " : row["Mem_Contact2"]));
            paramInfo[93] = new ParameterInfo("@Mem_Contact3", (row["Mem_Contact3"].ToString().Trim() == "" ? " " : row["Mem_Contact3"]));
            paramInfo[94] = new ParameterInfo("@Mem_RankCode", (row["Mem_RankCode"].ToString().Trim() == "" ? " " : row["Mem_RankCode"]));
            paramInfo[95] = new ParameterInfo("@Mem_OldPayrollType", row["Mem_OldPayrollType"], SqlDbType.Char, 1);
            paramInfo[96] = new ParameterInfo("@Mem_OldSalaryDate", row["Mem_OldSalaryDate"]);
            paramInfo[97] = new ParameterInfo("@Mem_OldSalaryRate", row["Mem_OldSalaryRate"]);
            paramInfo[98] = new ParameterInfo("@Mem_IsTaxExempted", row["Mem_IsTaxExempted"]);
            paramInfo[99] = new ParameterInfo("@Mem_PremiumGrpDate", row["Mem_PremiumGrpDate"]);

            ParameterInfo[] paramInfo1 = new ParameterInfo[2];

            paramInfo1[0] = new ParameterInfo("@Mem_Image", row["Mem_Image"], SqlDbType.Image, ImageSize);
            paramInfo1[1] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);

            string strSQL = @"UPDATE M_Employee SET Mem_Image = @Mem_Image Where Mem_IDNo = @Mem_IDNo";

            strUserMasterDB = @"UPDATE M_UserHdr
                                    SET Muh_LastName = @Mem_LastName,
                                        Muh_FirstName = @Mem_FirstName,
                                        Muh_MiddleName = @Mem_MiddleName,
                                        Muh_NickName = @Mem_NickName,
                                        Muh_JobTitle = Mcd_Name,
                                        Muh_EmailAddress = @Mem_OfficeEmailAddress,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    FROM M_UserHdr
                                    LEFT JOIN M_CodeDtl ON Mcd_Code = @Mem_JobTitleCode
                                                                AND Mcd_CodeType = 'POSITION'
                                    WHERE Muh_UserCode = @Mem_IDNo
                                    ";

            string strUserMasterPROFILEDB = string.Format(@"UPDATE {0}..M_UserHdr
                                                SET Muh_LastName = @Mem_LastName,
                                                    Muh_FirstName = @Mem_FirstName,
                                                    Muh_MiddleName = @Mem_MiddleName,
                                                    Muh_NickName = @Mem_NickName,
                                                    Muh_EmailAddress = @Mem_OfficeEmailAddress,
                                                    Usr_Login = @Usr_Login,
                                                    Ludatetime = getdate()
                                                WHERE Muh_UserCode = @Mem_IDNo
                                                ", CentralProfile);

            #region sql query for updating
            string sqlQuery = @"UPDATE M_Employee 
                                               SET Mem_LastName = @Mem_LastName,
                                                Mem_FirstName = @Mem_FirstName,
                                                Mem_MiddleName = @Mem_MiddleName,
                                                Mem_MaidenName = @Mem_MaidenName,
                                                Mem_BirthDate = @Mem_BirthDate,
                                                Mem_BirthPlace = @Mem_BirthPlace,
                                                Mem_Age = @Mem_Age,
                                                Mem_Gender = @Mem_Gender,
                                                Mem_CivilStatusCode = @Mem_CivilStatusCode,
                                                Mem_MarriedDate = @Mem_MarriedDate,
                                                Mem_NationalityCode = @Mem_NationalityCode,
                                                Mem_ReligionCode = @Mem_ReligionCode,
                                                Mem_PresCompleteAddress = @Mem_PresCompleteAddress,
                                                Mem_PresAddressBarangay = @Mem_PresAddressBarangay,
                                                Mem_PresAddressMunicipalityCity = @Mem_PresAddressMunicipalityCity,
                                                Mem_LandlineNo = @Mem_LandlineNo,
                                                Mem_CellNo = @Mem_CellNo,
                                                Mem_OfficeEmailAddress = @Mem_OfficeEmailAddress,
                                                Mem_PersonalEmail = @Mem_PersonalEmail,
                                                Mem_EducationCode = @Mem_EducationCode,
                                                Mem_SchoolCode = @Mem_SchoolCode,
                                                Mem_CourseCode = @Mem_CourseCode,
                                                Mem_BloodType = @Mem_BloodType,
                                                Mem_SSSNo = @Mem_SSSNo,
                                                Mem_PhilhealthNo = @Mem_PhilhealthNo,
                                                Mem_PayrollBankCode = @Mem_PayrollBankCode,
                                                Mem_BankAcctNo = @Mem_BankAcctNo,
                                                Mem_TIN = @Mem_TIN,
                                                Mem_TaxCode = @Mem_TaxCode,
                                                Mem_PagIbigNo = @Mem_PagIbigNo,
                                                Mem_PagIbigRule = @Mem_PagIbigRule,
                                                Mem_PagIbigShare = @Mem_PagIbigShare,
                                                Mem_ICEContactPerson = @Mem_ICEContactPerson,
                                                Mem_ICERelation = @Mem_ICERelation,
                                                Mem_ICECompleteAddress = @Mem_ICECompleteAddress,
                                                Mem_ICEAddressBarangay = @Mem_ICEAddressBarangay,
                                                Mem_ICEAddressMunicipalityCity = @Mem_ICEAddressMunicipalityCity,
                                                Mem_ICEContactNo = @Mem_ICEContactNo,
                                                Mem_CostcenterDate =  @Mem_CostcenterDate,
                                                Mem_CostcenterCode = @Mem_CostcenterCode,
                                                Mem_EmploymentStatusCode = @Mem_EmploymentStatusCode,
                                                Mem_IntakeDate = @Mem_IntakeDate,
                                                Mem_RegularDate = @Mem_RegularDate,
                                                Mem_WorkLocationCode = @Mem_WorkLocationCode,
                                                Mem_JobTitleCode = @Mem_JobTitleCode,
                                                Mem_PositionDate = @Mem_PositionDate,
                                                Mem_Superior1 = @Mem_Superior1,
                                                Mem_Superior2 = @Mem_Superior2,
                                                Mem_IsComputedPayroll = @Mem_IsComputedPayroll,
                                                Mem_PaymentMode = @Mem_PaymentMode,
                                                Mem_PayrollType = @Mem_PayrollType,
                                                Mem_SalaryDate = @Mem_SalaryDate,
                                                Mem_Salary = @Mem_Salary,
                                                Mem_WorkStatus = @Mem_WorkStatus,
                                                Mem_IsConfidential = @Mem_IsConfidential,
                                                Mem_ShiftCode = @Mem_ShiftCode,
                                                Mem_CalendarType = @Mem_CalendarType,
                                                Mem_CalendarGroup = @Mem_CalendarGroup,
                                                Mem_SeparationNoticeDate = @Mem_SeparationNoticeDate,
                                                Mem_SeparationCode = @Mem_SeparationCode,
                                                Mem_SeparationDate = @Mem_SeparationDate
                                                ,Mem_SSSRule = @Mem_SSSRule
                                                ,Mem_SSSShare = @Mem_SSSShare
                                                ,Mem_PHRule = @Mem_PHRule
                                                ,Mem_PHShare = @Mem_PHShare
                                                ,Mem_ClearedDate = @Mem_ClearedDate
                                                ,Mem_ProbationDate = @Mem_ProbationDate
                                                ,Usr_Login = @Usr_Login
                                                ,Ludatetime = GetDate()
                                                ,Mem_Height = @Mem_Height
                                                ,Mem_Weight = @Mem_Weight
                                                ,Mem_ProvCompleteAddress = @Mem_ProvCompleteAddress
                                                ,Mem_ProvAddressBarangay = @Mem_ProvAddressBarangay
                                                ,Mem_ProvAddressMunicipalityCity = @Mem_ProvAddressMunicipalityCity
                                                ,Mem_ProvLandlineNo = @Mem_ProvLandlineNo
                                                ,Mem_AwardsRecognition = @Mem_AwardsRecognition
                                                ,Mem_PRCLicense = @Mem_PRCLicense
                                                ,Mem_ExpenseClass = @Mem_ExpenseClass
                                                ,Mem_NickName = @Mem_NickName
                                                ,Emt_OldEmployeeID = @Emt_OldEmployeeID
                                                ,Mem_WifeClaim = @Mem_WifeClaim
                                                ,Mem_ShoesSize = @Mem_ShoesSize
                                                ,Mem_ShirtSize = @Mem_ShirtSize
                                                ,Mem_HairColor = @Mem_HairColor
                                                ,Mem_EyeColor = @Mem_EyeColor
                                                ,Mem_DistinguishMark = @Mem_DistinguishMark
                                                ,Mem_JobGrade = @Mem_JobGrade
                                                ,Mem_RankCode = @Mem_RankCode
                                                ,Mem_PositionCategory = @Mem_PositionCategory
                                                ,Mem_PositionClass = @Mem_PositionClass
                                                ,Mem_PremiumGrpCode = @Mem_PremiumGrpCode
                                                ,Mem_GraduatedDate = @Mem_GraduatedDate
                                                ,Mem_Contact1 = @Mem_Contact1
                                                ,Mem_Contact2 = @Mem_Contact2
                                                ,Mem_Contact3 = @Mem_Contact3
                                                ,Mem_OldPayrollType = @Mem_OldPayrollType
                                                ,Mem_OldSalaryDate = @Mem_OldSalaryDate
                                                ,Mem_OldSalaryRate = @Mem_OldSalaryRate
                                                ,Mem_IsTaxExempted = @Mem_IsTaxExempted
                                                ,Mem_PremiumGrpDate = @Mem_PremiumGrpDate
                                                WHERE Mem_IDNo=@Mem_IDNo";

            string queryUpdateProfile = string.Format(@"
                           UPDATE {0}..M_Employee
                           SET [Mem_IDNo] = @Mem_IDNo
                              ,[Mem_LastName] = @Mem_LastName
                              ,[Mem_FirstName] = @Mem_FirstName
                              ,[Mem_MiddleName] = @Mem_MiddleName
                              ,[Mem_MaidenName] = @Mem_MaidenName
                              ,[Mem_NickName] = @Mem_NickName
                              ,[Mem_Gender] = @Mem_Gender
                              ,[Mem_CivilStatusCode] = @Mem_CivilStatusCode
                              ,[Mem_OfficeEmailAddress] = @Mem_OfficeEmailAddress
                              ,[Mem_CostcenterCode] = @Mem_CostcenterCode
                              ,[Mem_EmploymentStatusCode] = @Mem_EmploymentStatusCode
                              ,[Mem_RankCode] = @Mem_RankCode
                              ,[Mem_IntakeDate] = @Mem_IntakeDate
                              ,[Mem_RegularDate] = @Mem_RegularDate
                              ,[Mem_JobTitleCode] = @Mem_JobTitleCode
                              ,[Mem_PayrollType] = @Mem_PayrollType
                              ,[Mem_WorkStatus] = @Mem_WorkStatus
                              ,[Mem_SeparationCode] = @Mem_SeparationCode
                              ,[Mem_SeparationDate] = @Mem_SeparationDate
                              ,[Usr_Login] = @Usr_Login
                              ,[Ludatetime] = GETDATE()
                         WHERE Mem_IDNo=@Mem_IDNo", CentralProfile);

            #endregion

            retVal = DalUp.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            DalUp.ExecuteNonQuery(string.Format(strSQL, row["Mem_Image"]), CommandType.Text, paramInfo1);

            //Update M_Employee in ProfileDB
            DalUp.ExecuteNonQuery(queryUpdateProfile, CommandType.Text, paramInfo);

            //Update M_UserHdr in same DB
            DalUp.ExecuteNonQuery(strUserMasterDB, CommandType.Text, paramInfo);

            //Update M_UserHdr in Profile DB
            DalUp.ExecuteNonQuery(strUserMasterPROFILEDB, CommandType.Text, paramInfo);

            return retVal;
        }

        public int CreateAuditTrailRec(DataRow row, DALHelper DalUp)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Tat_ColId", row["Tat_ColId"]);
            paramInfo[1] = new ParameterInfo("@Tat_IDNo", row["Tat_IDNo"]);
            paramInfo[2] = new ParameterInfo("@Tat_LineNo", row["Tat_LineNo"]);
            paramInfo[3] = new ParameterInfo("@Tat_OldValue", row["Tat_OldValue"]);
            paramInfo[4] = new ParameterInfo("@Tat_NewValue", row["Tat_NewValue"]);
            paramInfo[5] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            string sqlQuery = @"INSERT INTO T_AuditTrl
                                                   (Tat_ColId
                                                   ,Tat_IDNo
                                                   ,Tat_LineNo
                                                   ,Tat_OldValue
                                                   ,Tat_NewValue
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@Tat_ColId
                                                   ,@Tat_IDNo
                                                   ,@Tat_LineNo
                                                   ,@Tat_OldValue
                                                   ,@Tat_NewValue
                                                   ,@Usr_Login
                                                   ,Getdate())";
            
            retVal = DalUp.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        public int CreateAuditTrailRec(DataRow row, string DatabaseProfile, DALHelper DalUp)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Tat_ColId", row["Tat_ColId"]);
            paramInfo[1] = new ParameterInfo("@Tat_IDNo", row["Tat_IDNo"]);
            paramInfo[2] = new ParameterInfo("@Tat_LineNo", row["Tat_LineNo"]);
            paramInfo[3] = new ParameterInfo("@Tat_OldValue", row["Tat_OldValue"]);
            paramInfo[4] = new ParameterInfo("@Tat_NewValue", row["Tat_NewValue"]);
            paramInfo[5] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            string sqlQuery = string.Format(@"INSERT INTO [{0}]..T_AuditTrl
                                                   (Tat_ColId
                                                   ,Tat_IDNo
                                                   ,Tat_LineNo
                                                   ,Tat_OldValue
                                                   ,Tat_NewValue
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@Tat_ColId
                                                   ,@Tat_IDNo
                                                   ,@Tat_LineNo
                                                   ,@Tat_OldValue
                                                   ,@Tat_NewValue
                                                   ,@Usr_Login
                                                   ,Getdate())", DatabaseProfile);

            retVal = DalUp.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        public string GetLastSeqNo(string Tat_ColId, string Tat_IDNo, DALHelper DalUp)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tat_ColId", Tat_ColId);
            paramInfo[1] = new ParameterInfo("@Tat_IDNo", Tat_IDNo);

            string sqlQuery = @"Select Count(Tat_ColId)
                                                From T_AuditTrl
                                                Where Tat_ColId = @Tat_ColId
                                                And Tat_IDNo = @Tat_IDNo";

            ds = DalUp.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            
            return ds.Tables[0].Rows[0][0].ToString();
        }

        public string GetLastSeqNo(string Tat_ColId, string Tat_IDNo, string DatabaseProfile, DALHelper DalUp)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tat_ColId", Tat_ColId);
            paramInfo[1] = new ParameterInfo("@Tat_IDNo", Tat_IDNo);

            string sqlQuery = string.Format(@"Select Count(Tat_ColId)
                                                From [{0}]..T_AuditTrl
                                                Where Tat_ColId = @Tat_ColId
                                                And Tat_IDNo = @Tat_IDNo", DatabaseProfile);

            ds = DalUp.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

            return ds.Tables[0].Rows[0][0].ToString();
        }

        public DataRow AssignAuditRowRec(string colid, string seqno, string prevVal, string curVal, string txtIDNumber, DALHelper DalUp)
        {
            DataRow dr = DbRecord.Generate(CommonConstants.TableName.T_AuditTrl);
            dr["Tat_ColId"] = colid;
            dr["Tat_IDNo"] = txtIDNumber;
            dr["Tat_LineNo"] = GenerateSeqNo(colid, txtIDNumber, DalUp);
            dr["Tat_OldValue"] = prevVal;
            dr["Tat_NewValue"] = curVal;
            dr["Usr_Login"] = LoginInfo.getUser().UserCode;
            return dr;
        }

        public DataRow AssignAuditRowRec(string colid, string seqno, string prevVal, string curVal, string txtIDNumber, DALHelper DalUp, string user)
        {
            DataRow dr = DbRecord.Generate(CommonConstants.TableName.T_AuditTrl);
            dr["Tat_ColId"] = colid;
            dr["Tat_IDNo"] = txtIDNumber;
            dr["Tat_LineNo"] = GenerateSeqNo(colid, txtIDNumber, DalUp);
            dr["Tat_OldValue"] = prevVal;
            dr["Tat_NewValue"] = curVal;
            dr["Usr_Login"] = user;
            return dr;
        }

        public DataRow AssignAuditRowRec(string colid, string seqno, string prevVal, string curVal, string txtIDNumber, string DatabaseProfile, DALHelper DalUp)
        {
            DataRow dr = DbRecord.Generate(CommonConstants.TableName.T_AuditTrl);
            dr["Tat_ColId"] = colid;
            dr["Tat_IDNo"] = txtIDNumber;
            dr["Tat_LineNo"] = GenerateSeqNo(colid, txtIDNumber, DatabaseProfile, DalUp);
            dr["Tat_OldValue"] = prevVal;
            dr["Tat_NewValue"] = curVal;
            dr["Usr_Login"] = LoginInfo.getUser().UserCode;
            return dr;
        }

        public string GenerateSeqNo(string colid, string empid, DALHelper DalUp)
        {
            string x = this.GetLastSeqNo(colid, empid, DalUp);
            int y = Convert.ToInt32(x);
            y = y + 1;
            if (y < 10)
            {
                x = "0" + y.ToString();
            }
            else
                x = y.ToString();
            return x;
        }

        public string GenerateSeqNo(string colid, string empid, string DatabaseProfile, DALHelper DalUp)
        {
            string x = this.GetLastSeqNo(colid, empid, DatabaseProfile, DalUp);
            int y = Convert.ToInt32(x);
            y = y + 1;
            if (y < 10)
            {
                x = "0" + y.ToString();
            }
            else
                x = y.ToString();
            return x;
        }

        private int UpdateUserMaster(DataRow dr, string txtIDNumber)
        {
            int retVal = 0;

            #region query
            string qString = @"Update M_UserHdr
                                Set Muh_LastName = @Muh_LastName
	                                ,Muh_FirstName = @Muh_FirstName
	                                ,Muh_MiddleName = @Muh_MiddleName
	                                ,Muh_JobTitle = @Muh_JobTitle
                                Where Muh_UserCode = @Muh_UserCode";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Muh_LastName", dr["Lmt_Lastname"]);
             paramInfo[1] = new ParameterInfo("@Muh_FirstName", dr["Lmt_Firstname"]);

            if (dr["Lmt_Middlename"].ToString() == null)
                paramInfo[2] = new ParameterInfo("@Muh_MiddleName", dr["Lmt_Middlename"].ToString().Trim().Substring(0, 1));
            else
                paramInfo[2] = new ParameterInfo("@Muh_MiddleName", "");

            paramInfo[3] = new ParameterInfo("@Muh_JobTitle", dr["Lmt_PositionDesc"]);
            paramInfo[4] = new ParameterInfo("@Muh_UserCode", txtIDNumber);

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
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        private bool CheckforUpdateInLogMaster(string txtLastname
                                                , string txtFirstname
                                                , string txtMiddlename
                                                , string txtCostCenter
                                                , string txtPosTitle
                                                , string txtPicture
                                                , string txtBirthDate
                                                , DataRow Origdr)
        {
            bool retval = false;
            string LastName = Origdr["Mem_LastName"].ToString();
            string FirstName = Origdr["Mem_FirstName"].ToString();
            string Middlename = Origdr["Mem_MiddleName"].ToString();
            string CostCenterCode = Origdr["Mem_CostcenterCode"].ToString();
            string PositionCode = Origdr["Mem_JobTitleCode"].ToString();
            string Picture = Origdr["Mem_Image"].ToString();
            string BirthDate = Origdr["Mem_BirthDate"].ToString();
            if (!LastName.Trim().Equals(txtLastname))
            {
                retval = true;
            }
            if (!FirstName.Trim().Equals(txtFirstname))
            {
                retval = true;
            }
            if (!Middlename.Trim().Equals(txtMiddlename))
            {
                retval = true;
            }
            if (!CostCenterCode.Trim().Equals(txtCostCenter))
            {
                retval = true;
            }
            if (!PositionCode.Trim().Equals(txtPosTitle))
            {
                retval = true;
            }
            if (!Picture.Trim().Equals(txtPicture))
            {
                retval = true;
            }
            if (!BirthDate.Trim().Equals(txtBirthDate))
            {
                retval = true;
            }
            return retval;
        }

        private void CheckModificationandAuditTrailAdd(DataRow Origdr
                                                        , string CmbobxCivilStatus
                                                        , string txtSSS
                                                        , string txtPhilhealthNo
                                                        , string txtBankCode
                                                        , string txtBankAccountNo
                                                        , string txtTIN
                                                        , string txtTaxCode
                                                        , string txtHDMFNo
                                                        , string txtEmploymentStatus
                                                        , string txtPayrollStatus
                                                        , string txtPaymentMode
                                                        , string txtJobStatus
                                                        , string txtWorkType
                                                        , string txtWorkGroup
                                                        , string txtIDNumber
                                                        , string SSSCode
                                                        , string HDMFCode
                                                        , string PhilhealthCode
                                                        , string txtTaxExempt
                                                        , DALHelper DalUp)
        {
            string CivilStatus = Origdr["Mem_CivilStatusCode"].ToString();
            string SSSNO = Origdr["Mem_SSSNo"].ToString();
            string PhilhealthNo = Origdr["Mem_PhilhealthNo"].ToString();
            string BankCode = Origdr["Mem_PayrollBankCode"].ToString();
            string BankAccountNo = Origdr["Mem_BankAcctNo"].ToString();
            string TIN = Origdr["Mem_TIN"].ToString();
            string TaxCode = Origdr["Mem_TaxCode"].ToString();
            string HDMFNo = Origdr["Mem_PagIbigNo"].ToString();
            string EmploymentStatus = Origdr["Mem_EmploymentStatusCode"].ToString();
            string PayrollStatus = Origdr["Mem_IsComputedPayroll"].ToString();

            string PaymentMode = Origdr["Mem_PaymentMode"].ToString();
            string JobStatus = Origdr["Mem_WorkStatus"].ToString();
            string WorkType = Origdr["Mem_CalendarType"].ToString();
            string WorkGroup = Origdr["Mem_CalendarGroup"].ToString();

            string OrigHDMFCode = Origdr["Mem_PagIbigRule"].ToString();
            string OrigPhilhealthCode = Origdr["Mem_PHRule"].ToString();
            string OrigSSSCode = Origdr["Mem_SSSRule"].ToString();
            string TaxExempt = Origdr["Mem_IsTaxExempted"].ToString();
            if (!OrigHDMFCode.Equals(HDMFCode))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("HDMFCode", this.GenerateSeqNo("HDMFCode", txtIDNumber, DalUp), OrigHDMFCode, HDMFCode, txtIDNumber, DalUp), DalUp);
            }
            if (!OrigSSSCode.Equals(SSSCode))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("SSSCode", this.GenerateSeqNo("SSSCode", txtIDNumber, DalUp), OrigSSSCode, SSSCode, txtIDNumber, DalUp), DalUp);
            }
            if (!OrigPhilhealthCode.Equals(PhilhealthCode))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("PhilhealthCode", this.GenerateSeqNo("PhilhealthCode", txtIDNumber, DalUp), OrigPhilhealthCode, PhilhealthCode, txtIDNumber, DalUp), DalUp);
            }

            string paystat = string.Empty;

            if (!CivilStatus.Equals(CmbobxCivilStatus))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("CivilStatus", this.GenerateSeqNo("CivilStatus", txtIDNumber, DalUp), CivilStatus, CmbobxCivilStatus, txtIDNumber, DalUp), DalUp);
            }
            if (!SSSNO.Equals(txtSSS))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("SSSNo", this.GenerateSeqNo("SSSNo", txtIDNumber, DalUp), SSSNO, txtSSS, txtIDNumber, DalUp), DalUp);
            }
            if (!PhilhealthNo.Equals(txtPhilhealthNo))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("PhilhealthNo", this.GenerateSeqNo("PhilhealthNo", txtIDNumber, DalUp), PhilhealthNo, txtPhilhealthNo, txtIDNumber, DalUp), DalUp);
            }
            if (!BankCode.Equals(txtBankCode))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("BankCode", this.GenerateSeqNo("BankCode", txtIDNumber, DalUp), BankCode, txtBankCode, txtIDNumber, DalUp), DalUp);
            }
            if (!BankAccountNo.Equals(txtBankAccountNo))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("BankAccountNo", this.GenerateSeqNo("BankAccountNo", txtIDNumber, DalUp), BankAccountNo, txtBankAccountNo, txtIDNumber, DalUp), DalUp);
            }
            if (!TIN.Equals(txtTIN))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("TIN", this.GenerateSeqNo("TIN", txtIDNumber, DalUp), TIN, txtTIN, txtIDNumber, DalUp), DalUp);
            }
            if (!TaxCode.Equals(txtTaxCode))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("TaxCode", this.GenerateSeqNo("TaxCode", txtIDNumber, DalUp), TaxCode, txtTaxCode, txtIDNumber, DalUp), DalUp);
            }
            if (!HDMFNo.Equals(txtHDMFNo))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("HDMFNo", this.GenerateSeqNo("HDMFNo", txtIDNumber, DalUp), HDMFNo, txtHDMFNo, txtIDNumber, DalUp), DalUp);
            }
            if (!EmploymentStatus.Equals(txtEmploymentStatus))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("EmploymentStatu", this.GenerateSeqNo("EmploymentStatu", txtIDNumber, DalUp), EmploymentStatus, txtEmploymentStatus, txtIDNumber, DalUp), DalUp);
            }

            if (txtPayrollStatus == "YES")
                paystat = "True";
            else
                paystat = "False";
            if (!PayrollStatus.Equals(paystat))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("PayrollStatus", this.GenerateSeqNo("PayrollStatus", txtIDNumber, DalUp), PayrollStatus, paystat, txtIDNumber, DalUp), DalUp);
            }

            if (txtTaxExempt == "YES")
                paystat = "True";
            else
                paystat = "False";
            if (!TaxExempt.Equals(paystat))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("TaxExempt", this.GenerateSeqNo("TaxExempt", txtIDNumber, DalUp), TaxExempt, paystat, txtIDNumber, DalUp), DalUp);
            }
            
            if (!PaymentMode.Equals(txtPaymentMode))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("PaymentMode", this.GenerateSeqNo("PaymentMode", txtIDNumber, DalUp), PaymentMode, txtPaymentMode, txtIDNumber, DalUp), DalUp);
            }

            if (!JobStatus.Equals(txtJobStatus))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("JobStatus", this.GenerateSeqNo("JobStatus", txtIDNumber, DalUp), JobStatus, txtJobStatus, txtIDNumber, DalUp), DalUp);
            }
            if (!WorkType.Equals(txtWorkType))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("WorkType", this.GenerateSeqNo("WorkType", txtIDNumber, DalUp), WorkType, txtWorkType, txtIDNumber, DalUp), DalUp);
            }
            if (!WorkGroup.Equals(txtWorkGroup))
            {
                this.CreateAuditTrailRec(this.AssignAuditRowRec("WorkGroup", this.GenerateSeqNo("WorkGroup", txtIDNumber, DalUp), WorkGroup, txtWorkGroup, txtIDNumber, DalUp), DalUp);
            }
        }

        public int PerformUpdate(DataRow dr
                                , DataRow Origdr
                                , DataRow LogMasterdr
                                , string CmbobxCivilStatus
                                , string txtSSS
                                , string txtPhilhealthNo
                                , string txtBankCode
                                , string txtBankAccountNo
                                , string txtTIN
                                , string txtTaxCode
                                , string txtHDMFNo
                                , string txtEmploymentStatus
                                , string txtPayrollStatus
                                , string txtPaymentMode
                                , string txtJobStatus
                                , string txtWorkType
                                , string txtWorkGroup
                                , string txtIDNumber
                                , string txtLastname
                                , string txtFirstname
                                , string txtMiddlename
                                , string txtCostCenter
                                , string txtPosTitle
                                , string txtjoblevel
                                , int ImageSize
                                , string SSSCode
                                , string HDMFCode
                                , string TaxExempt
                                , string PhilhealthCode)
        {
            int retval = 0;
            bool errFlag = false;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    //Update in Confi/NonConfi Employee Master and Profle Employee Master
                    retval = this.Update(dr, dal, ImageSize);

                    #region CheckModificationandAuditTrailAdd()
                    //Check if modification Exists
                    this.CheckModificationandAuditTrailAdd(Origdr
                                                        , CmbobxCivilStatus
                                                        , txtSSS
                                                        , txtPhilhealthNo
                                                        , txtBankCode
                                                        , txtBankAccountNo
                                                        , txtTIN
                                                        , txtTaxCode
                                                        , txtHDMFNo
                                                        , txtEmploymentStatus
                                                        , txtPayrollStatus
                                                        , txtPaymentMode
                                                        , txtJobStatus
                                                        , txtWorkType
                                                        , txtWorkGroup
                                                        , txtIDNumber
                                                        , SSSCode
                                                        , HDMFCode
                                                        , PhilhealthCode
                                                        , TaxExempt
                                                        , dal);
                    #endregion

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    errFlag = true;
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (!errFlag)
            {
                if (this.CheckforUpdateInLogMaster(txtLastname
                                                , txtFirstname
                                                , txtMiddlename
                                                , txtCostCenter
                                                , txtPosTitle
                                                , dr["Mem_Image"].ToString()
                                                , dr["Mem_BirthDate"].ToString()
                                                , Origdr))
                {
                    this.UpdateLogMasterRecord(LogMasterdr, ImageSize);
                    
                    #region Check If Exist In User Master And Update
                    this.CheckIfExistInUserMasterAndUpdate(dr["Mem_IDNo"].ToString()
                                                , txtLastname
                                                , txtFirstname
                                                , txtMiddlename
                                                , txtPosTitle
                                                , dr["Mem_OfficeEmailAddress"].ToString());
                    #endregion
                }
                
            }
            return retval;
        }

        #endregion

        #region <For Counting>
        public string CountEmployeeBeneficiary(string Mfm_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Mfm_IDNo)
                                FROM M_EmpFamily
                                WHERE Mfm_IDNo = @Mfm_IDNo
								AND Mfm_CancelledDate is null";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mfm_IDNo", Mfm_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountEmpRestDay(string Ter_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Ter_IDNo)
                                FROM T_EmpRest
                                WHERE Ter_IDNo = @Ter_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountEmployeeTraining(string Tet_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tet_IDNo)
                                FROM T_EmpTraining
                                WHERE Tet_IDNo = @Tet_IDNo
                                AND Tet_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tet_IDNo", Tet_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountEmployeeOffense(string Tof_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tof_IDNo)
                                FROM T_EmpOffense
                                WHERE Tof_IDNo = @Tof_IDNo
                                AND Tof_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tof_IDNo", Tof_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountEmployeeAssets(string Tas_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tas_IDNo)
                                FROM T_EmpAsset
                                WHERE Tas_IDNo = @Tas_IDNo
                                AND Tas_RecordStatus = 'A'
                                AND Tas_AssetCode <> '' ";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tas_IDNo", Tas_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountLeaveCredits(string Tas_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tll_IDNo)
                                FROM T_EmpLeaveLdg
                                WHERE Tll_IDNo = @Tas_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tas_IDNo", Tas_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountCareerInformation(string Tas_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Twe_IDNo)
                                FROM T_EmpWorkExperience
                                WHERE Twe_IDNo = @Tas_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tas_IDNo", Tas_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountVisaInformation(string Tas_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tde_IDNo)
                                FROM T_EmpDocumentExpiry
                                WHERE Tde_IDNo = @Tas_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tas_IDNo", Tas_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountNotes(string Tnt_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tnt_IDNo)
                                FROM T_EmpNotes
                                WHERE Tnt_IDNo = @Tnt_IDNo
                                And Tnt_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tnt_IDNo", Tnt_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountAwardsAndRecognition(string Tac_EmpID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tac_EmpID)
                                FROM T_EmpAchievement
                                WHERE Tac_EmpID = @Tac_EmpID
                                And Tac_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tac_EmpID", Tac_EmpID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        public string CountSpecialAppointments(string Tds_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT COUNT(Tds_IDNo)
                                FROM T_EmpDesignation
                                WHERE Tds_IDNo = @Tds_IDNo
                                And Tds_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tds_IDNo", Tds_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "0";
        }

        #endregion
        public void CheckIfExistInUserMasterAndUpdate(string Muh_UserCode,
                                                      string Muh_LastName,
                                                      string Muh_FirstName,
                                                      string Muh_MiddleName,
                                                      string Muh_JobTitle,
                                                      string Muh_EmailAddress)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"UPDATE M_UserHdr
                                SET Muh_LastName = '" + Muh_LastName + @"' ,
                                    Muh_FirstName = '" + Muh_FirstName + @"' ,
                                    Muh_MiddleName = '" + Muh_MiddleName + @"' ,
                                    Muh_JobTitle = '" + Muh_JobTitle + @"' ,
                                    Muh_EmailAddress = '" + Muh_EmailAddress + @"' 
                               WHERE Muh_UserCode = '" + Muh_UserCode + @"'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
        }

        public string GetTaxCodeDef(string Mcd_Code)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"select Mcd_Name
                               from M_CodeDtl
                               where Mcd_CodeType = 'TAXCODE'
                                and Mcd_Code = '" + Mcd_Code + @"' ";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        #region added by Kevin 01052009 - c1 report conversion

        public DataSet GetCompanyInfo()
        {
            DataSet ds = new DataSet();
            string sql = @"Select Mcm_CompanyName
                              ,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ' ' + Mcd_Name  as Address
                              ,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
                              ,Mcm_CompanyLogo
                        From M_Company
                        Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code and Mcd_CodeType='ZIPCODE'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetEmployeeInfo(string employeeID)
        {
            DataSet ds;
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@EmployeeID", employeeID);
            #region query
            string sql = @"select  Mem_IDNo
                            , Mem_LastName
                            , Mem_FirstName
                            , Mem_MiddleName
                            , Mem_BirthDate
                            , Mem_Gender
                            ,Mem_Age
                            ,x.Mcd_Name as Mem_BirthPlace
                            ,y.Mcd_Name as Mem_NationalityCode
                            , o.Mcd_Name as Religion
                            , a.Mcd_Name as Mem_CivilStatusCode
                            , Mem_PresCompleteAddress
                            , b.Mcd_Name as Mem_PresAddressBarangay
                            , c.Mcd_Name as Mem_PresAddressMunicipalityCity
                            , Mem_LandlineNo
                            , Mem_CellNo
                            , Mem_OfficeEmailAddress
                            , d.Mcd_Name as Mem_EducationCode
                            , e.Mcd_Name as Mem_SchoolCode
                            , f.Mcd_Name as Mem_CourseCode
                            , Mem_ICEContactPerson
                            , g.Mcd_Name as Mem_ICERelation
                            , Mem_ICECompleteAddress
                            , h.Mcd_Name as Mem_ICEAddressBarangay
                            , i.Mcd_Name as Mem_ICEAddressMunicipalityCity
                            , Mem_ICEContactNo
                            , j.Mcd_Name as Mem_EmploymentStatusCode
                            , Mem_IntakeDate
                            ,   Case When Len(Rtrim(IsNull(Mdv_DivName,''))) > 0 Then
                                 Rtrim(IsNull(Mdv_DivName,''))
                                Else 
                                 '' 
                                End  +
                                  Case When Len(Rtrim(IsNull(Mdp_DptName,''))) > 0 Then
                                 ' / '+ Rtrim(IsNull(Mdp_DptName,''))
                                Else 
                                 '' 
                                End  +
                                 Case When Len(Rtrim(IsNull(Msc_SecName,''))) > 0 Then
                                 ' / '+ Rtrim(IsNull(Msc_SecName,''))
                                Else 
                                 '' 
                                End  +  
                                 Case When Len(Rtrim(IsNull(Msb_SubSecName,''))) > 0 Then
                                 ' / '+ Rtrim(IsNull(Msb_SubSecName,''))
                                Else 
                                 '' 
                                End  +
                                 Case When Len(Rtrim(IsNull(Mpr_PrcName,''))) > 0 Then
                                 ' / '+ Rtrim(IsNull(Mpr_PrcName,''))
                                Else 
                                 '' 
                                End  as CostCenter
                            , n.Mcd_Name as Mem_JobTitleCode
                            , k.Mcd_Name as Mem_WorkStatus
                            , Mem_ProvCompleteAddress
                            , l.Mcd_Name as Mem_ProvAddressBarangay
                            , m.Mcd_Name as Mem_ProvAddressMunicipalityCity
                            , Mem_ProvLandlineNo
                            , Mem_BloodType
                            , Mem_SSSNo
                            , Mem_PhilhealthNo
                            , Mem_TIN
                            , Mem_TaxCode
                            , Mem_PagIbigNo
                            , Mem_JobGrade
                            , Mem_RankCode
                            , Mem_PositionCategory
                            , Mem_PositionClass
                            , Mem_PremiumGrpCode
                            , p.Mcd_Name as Emt_PremiumGroupDesc
                            --, Tpg_StartDate as Mem_PremiumGrpDate
                             from M_Employee
                            left join M_CodeDtl a on a.Mcd_Code = Mem_CivilStatusCode
                             and a.Mcd_CodeType='CIVILSTAT'
                            left join M_CodeDtl b on b.Mcd_Code = Mem_PresAddressBarangay
                             and b.Mcd_CodeType='BARANGAY'
                            left join M_CodeDtl c on c.Mcd_Code = Mem_PresAddressMunicipalityCity
                             and c.Mcd_CodeType='ZIPCODE'
                            left join M_CodeDtl d on d.Mcd_Code = Mem_EducationCode
                             and d.Mcd_CodeType='EDUCLEVEL'
                            left join M_CodeDtl e on e.Mcd_Code = Mem_SchoolCode
                             and e.Mcd_CodeType='SCHOOL'
                            left join M_CodeDtl f on f.Mcd_Code = Mem_CourseCode
                             and f.Mcd_CodeType='COURSE'
                            left join M_CodeDtl g on g.Mcd_Code = Mem_ICERelation
                             and g.Mcd_CodeType='RELATION'
                            left join M_CodeDtl h on h.Mcd_Code = Mem_ICEAddressBarangay
                             and h.Mcd_CodeType='BARANGAY'
                            left join M_CodeDtl i on i.Mcd_Code = Mem_ICEAddressMunicipalityCity
                             and i.Mcd_CodeType='ZIPCODE'
                            left join M_CodeDtl j on j.Mcd_Code = Mem_EmploymentStatusCode
                             and j.Mcd_CodeType='EMPSTAT'
                            left join M_CodeDtl k on k.Mcd_Code = Mem_WorkStatus
                             and k.Mcd_CodeType='WORKSTAT'
                            left join M_CodeDtl l on l.Mcd_Code = Mem_ProvAddressBarangay
                             and l.Mcd_CodeType='BARANGAY'
                            left join M_CodeDtl m on m.Mcd_Code = Mem_ProvAddressMunicipalityCity
                             and m.Mcd_CodeType='ZIPCODE'
                            left join M_CodeDtl n on n.Mcd_Code = Mem_JobTitleCode
                             and n.Mcd_CodeType='POSITION'
                            left join M_CodeDtl o on o.Mcd_Code = Mem_ReligionCode
                             and o.Mcd_CodeType='RELIGION'
                            left join M_CodeDtl x on x.Mcd_Code = Mem_BirthPlace
                            and x.Mcd_CodeType = 'ZIPCODE'
                            left join M_CodeDtl y on y.Mcd_Code = Mem_NationalityCode
                            and y.Mcd_CodeType = 'CITIZEN'
                            LEFT JOIN M_CodeDtl p on p.Mcd_Code = Mem_PremiumGrpCode
                            AND p.Mcd_CodeType = 'PREMGRP'
                            LEFT JOIN T_EmpPremiumGroup ON Tpg_IDNo = Mem_IDNo  
                            AND Tpg_PremiumGroup = Mem_PremiumGrpCode
                            LEFT JOIN M_CostCenter on   Mcc_CostCenterCode = Mem_CostcenterCode
                            LEFT JOIN M_Division on Mdv_DivCode= Mcc_DivCode 
                            LEFT JOIN M_Department on Mdp_DptCode = Mcc_DptCode
                            LEFT JOIN M_Section on  Msc_SecCode = Mcc_SecCode
                            LEFT JOIN M_SubSection  on Msb_SubSecCode = Mcc_SubsecCode 
                            LEFT JOIN M_Process on Mpr_PrcCode = Mcc_PrcCode
                            where  Mem_IDNo=@EmployeeID
                            order by Mem_LastName
                            , Mem_FirstName
                            , Mem_MiddleName";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            return ds;
        }

        #endregion
        public bool Existing(string IDno)
        {
            bool flag = false;

            string sql = @"Select * from T_LogMaster where Lmt_EmployeeID='{0}'";
            using (DALHelper dal = new DALHelper(true))
            {
                DataSet ds = dal.ExecuteDataSet(string.Format(sql, IDno), CommandType.Text);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        #region overload
        public int PerformInsertandRecordSetup(DataRow row, string idnumber, string shiftcode, DataRow LogMasterdr, int ImageSize, DALHelper dal) 
        {
            int retval = 0;
            bool errflag = false;
           

            try
            {
                //Insert Employee Master Record in Confi/NonConfi Employee Master Table and Profile Employee Master Table
                retval = this.Add(row, dal, ImageSize);
            }
            catch (Exception e)
            {
                errflag = true;
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                CommonProcedures.ShowMessage(messageCode, "");
            }
              

            if (!errflag)
            {
                //Create LogMaster record
                try
                {
                    if (!this.ExistsInLogMaster(idnumber))
                        retval = this.CreateEmpLogMasterRecord(LogMasterdr, ImageSize);
                }
                catch 
                { }
            }
            return retval;
        }
        #endregion

        public void CreateFlexLedgerRecord(string HireYear, string EmployeeID, DALHelper dal)
        {
            #region qstring
            string sqlstring = @"   Insert Into t_flexledger
                                    select '{0}','{1}','L',99,0,0,'sa',getdate()
                                    union
                                    select '{0}','{1}','T',3,0,0,'sa',getdate()";
            #endregion

            dal.ExecuteNonQuery(string.Format(sqlstring, HireYear, EmployeeID), CommandType.Text);
        }

        public DataSet GetDefaultFromCompanyMaster()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"select isnull(Mcm_InitialShift, '') as DefaultShift
	                                , isnull(Mcm_BankCode, '') as DefaultBankCode
	                                , isnull(Mcm_InitialPagIbigRule, '') as DefaultHDMFCode
	                                , isnull(Mcm_InitialSSSRule, '') as DefaultSSSCode
	                                , isnull(Mcm_InitialPhilhealthRule, '') as DefaultPhilhealthCode
	                                , isnull(Mcm_InitialRestday, '') as DefaultRestday
	                                , isnull(Mcm_CompanyAddress3, '') as DefaultWorkLocation
                               from M_Company";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }

        public bool IsWorkTypeExists(string strWorktype)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = string.Format(@"select Tcl_CalendarType
                                            from T_Calendar
                                            where Tcl_CalendarType = '{0}'", strWorktype);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool IsWorkTypeGroupExists(string strWorktype, string strWorkgroup)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = string.Format(@"select Tcl_CalendarType
                                            from T_Calendar
                                            where Tcl_CalendarType = '{0}' and Tcl_CalendarGroup = '{1}'", strWorktype, strWorkgroup);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void CorrectLogLedgerRec(string EmployeeID, DALHelper dal)
        {
            SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, "");
            DataTable dtEmployee = SystemCycleProcessingBL.GetActiveEmployeeList(EmployeeID);
            SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                            , new CommonBL().GetCurrentPayPeriod(), "", ""
                                                            , true, true, true, true, true, true, true
                                                            , true, true, true, true, true, true, true, LoginInfo.getUser().UserCode);
            SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                            , new CommonBL().GetNextPayPeriod(), "", ""
                                                            , true, true, true, true, true, true, true
                                                            , true, true, true, true, true, true, true, LoginInfo.getUser().UserCode); 
        }


        public void DeletePayrollRecords(string PayPeriod, string EmployeeId, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"DELETE FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = '{0}'
                                            AND Tpy_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                            AND Tpm_IDNo = '{1}'

                                            DELETE FROM T_EmpDeductionDtl
                                            WHERE Tdd_PayCycle = '{0}'
                                            AND Tdd_IDNo = '{1}'

                                            DELETE FROM T_EmpDeductionHdrCycle
                                            WHERE Tdh_PayCycle = '{0}'
                                            AND Tdh_IDNo = '{1}'

                                            DELETE FROM T_EmpPayTranHdr
                                            WHERE Tph_PayCycle = '{0}'
                                            AND Tph_IDNo = '{1}'

                                            DELETE FROM T_EmpPayTranDtl
                                            WHERE Tpd_PayCycle = '{0}'
                                            AND Tpd_IDNo = '{1}'

                                            DELETE FROM T_EmpPayTranHdrMisc
                                            WHERE Tph_PayCycle = '{0}'
                                            AND Tph_IDNo = '{1}'

                                            DELETE FROM T_EmpPayTranDtlMisc
                                            WHERE Tpd_PayCycle = '{0}'
                                            AND Tpd_IDNo = '{1}'

                                            DELETE FROM T_EmpSystemAdj
                                            WHERE Tsa_AdjPayCycle = '{0}'
                                            AND Tsa_IDNo = '{1}'

                                            UPDATE T_EmpIncome
                                            SET Tin_PostFlag = 0
                                            WHERE Tin_PayCycle = '{0}'
                                            AND Tin_IDNo = '{1}'

                                            UPDATE T_EmpManualAdj
                                            SET Tma_PostFlag = 0
                                            WHERE Tma_PayCycle = '{0}'
                                            AND Tma_IDNo = '{1}'
                                            ", PayPeriod, EmployeeId);
            #endregion
            dalHelper.ExecuteNonQuery(query);
        }
    }
}
