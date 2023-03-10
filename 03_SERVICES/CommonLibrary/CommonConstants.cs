using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace CommonPostingLibrary
{
    public class CommonConstants
    {
        #region tableName
        public class TableName
        {
            public const string T_EmpLog = "T_EmpLog";
            public const string T_EmpDTR = "T_EmpDTR";
            public const string T_EmpTimeRegister = "T_EmpTimeRegister";
            public const string T_AuditTrl = "T_AuditTrl";
        }

        #endregion


        #region queries

        public class Queries
        {

        }
        #endregion

        #region storedProcedures

        public class StoredProcedures
        {

        }

        #endregion

        #region messages

        public class Messages
        {
            public const string msgErrInvalidVersion = "There is a new version that you SKIPPED \n Please follow the following : \n 1) Please Click the <Cancel> Button to Close \n 2) Open the ERP Application Again \n 3) Please Click the <OK> Button when an Update is Available \n 4) If you stll can\'t Login, Please Contact the IT Team";
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

        #endregion

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
