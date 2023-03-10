using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class CommonMessages
    {

        public static string CuttOff(string cutoffID)
        {
            return string.Format("Transaction made was cancelled.\n Cut-off for {0} was implemented for payroll processing purposes.", cutoffID);
        }

        public static string InputDate(string fieldName)
        {
            return String.Format("{0}: Must not exceed 12:59 AM/PM.", fieldName);
        }
        public static string NoExist(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Does not Exist.", fieldName, value);
        }
        public static string LesserThan(string fieldName1, string value1, string fieldName2, string value2)
        {
            return String.Format("{0}[{1}]: Should be lesser than {2}[{3}].", fieldName1, value1, fieldName2, value2);
        }

        public static string GreaterThanEqual(string fieldName1, string value1, string fieldName2, string value2)
        {
            return String.Format("{0}[{1}]: Should be greater than or equal to {2}[{3}].", fieldName1, value1, fieldName2, value2);
        }

        public static string LesserThanEqual(string fieldName1, string value1, string fieldName2, string value2)
        {
            return String.Format("{0}[{1}]: Should be lesser than or equal to {2}[{3}].", fieldName1, value1, fieldName2, value2);
        }

        public static string Equals(string fieldName1, string value1, string fieldName2, string value2)
        {
            return String.Format("{0}[{1}]: Should be Equal to {2}[{3}].", fieldName1, value1, fieldName2, value2);
        }

        public static string Required(string fieldName)
        {
            return String.Format("{0}: Required.", fieldName);
        }

        public static string Required(List<string> fieldNames)
        {
            string _sysMessage = "Please fill-up the fields listed below:\n\n";

            for (int i = 0; i < fieldNames.Count; i++)
                _sysMessage += string.Format("   {0}. {1}\n", i + 1, fieldNames[i]);

            return _sysMessage;
        }

        public static string ErrorFields(List<string> fieldNames)
        {
            string _sysMessage = "\n";

            for (int i = 0; i < fieldNames.Count; i++)
                _sysMessage += string.Format("   {0}. {1}\n", i + 1, fieldNames[i]);

            return _sysMessage;
        }

        public static string ErrorList(List<string> errors)
        {
            string _sysMessage = "Please correct the errors listed below:\n\n";

            for (int i = 0; i < errors.Count; i++)
                _sysMessage += string.Format("   {0}. {1}\n", i + 1, errors[i]);

            return _sysMessage;

        }

        public static string Inform(List<string> information)
        {
            string _sysMessage = "Do you want to continue with these conditions?\n\n";

            for (int i = 0; i < information.Count; i++)
                _sysMessage += string.Format("   {0}. {1}\n", i + 1, information[i]);

            return _sysMessage;

        }

        public static string Inform(List<string> information, string strHeader)
        {
            string _sysMessage = string.Format("{0}?\n\n", strHeader);

            for (int i = 0; i < information.Count; i++)
                _sysMessage += string.Format("   {0}. {1}\n", i + 1, information[i]);

            return _sysMessage;

        }

        public static string MaxAllowable(string value)
        {
            return String.Format("Amount to Pay should be lesser than or equal to Maximum Allowable [{0}]", value);
        }

        public static string InvalidInput(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Input.", fieldName, value);
        }

        public static string InvalideInput2(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Input. Get Status Code must be 2 digits.", fieldName, value);
        }

        public static string InvalidFormat(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Format.", fieldName, value);
        }

        public static string InvalidFormat(string fieldName, string value, string format)
        {
            return String.Format("{0}[{1}]: Invalid Format ({2}). ", fieldName, value, format);
        }

        public static string InvalidDate(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Date.", fieldName, value);
        }

        public static string InvalidNumber(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Number.", fieldName, value);
        }

        public static string InvalidCurrency(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Currency.", fieldName, value);
        }

        public static string InvalidLength(string fieldName, string value, string maxLength)
        {
            return String.Format("{0}[{1}]: Must not exceed {2} characters.", fieldName, value, maxLength);
        }

        public static string InvalidMinLength(string fieldName, string maxLength)
        {
            return String.Format("{0}: Must be atleast {1} characters.", fieldName, maxLength);
        }

        public static string InputNumbers(string fieldName)
        {
            return String.Format("{0}: Must be a valid Number.", fieldName);
        }

        public static string InvalidPercent(string fieldName, string value)
        { 
            return String.Format("{0}[{1}]: Invalid Percent.", fieldName, value);
        }

        public static string DuplicateEntry(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Duplicate Entry.", fieldName, value);
        }

        public static string DuplicateEntry2(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Duplicate Entry. Record has been CANCELLED. Please contact your System Administrator.", fieldName, value);
        }

        public static string Exist(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Already Exist.", fieldName, value);
        }

        public static string DateIsLessThanStartCycle(string StrtDate, string DateToday)
        {
            return String.Format("Date today: {0} is less than start cycle date: {1} .", DateToday, StrtDate);
        }

        public static string CutOff()
        {
            return String.Format("Cannot Save Record since it is already cutoff.");
        }

        public static string NotMatch(string fieldName)
        {
            return String.Format("{0}: Does not match.", fieldName);
        }

        public static string NotEditable(string fieldName, string value)
        {
            return String.Format("{0} [{1}]: Non Editable Record.", fieldName, value);
        }

        public static string NotDelete()
        {
            return string.Format("Record used by End Customer.");
        }

        public static string Saved(string recordName)
        {
            return String.Format("Saving {0} Record Completed.", recordName);
        }

        public static string Updated(string recordName)
        {
            return String.Format("Updating {0} Record Completed.", recordName);
        }
        public static string GreaterthanTheOther(string field1, string field2)
        {
            return String.Format("{0} should be greater than {1}.", field1, field2);
        }
        public static string Deleted(string recordName)
        {
            return String.Format("Deleting {0} Record Completed.", recordName);
        }

        public static string NoCodeAssigned()
        {
            return String.Format("Please assign code for this transaction first.");
        }
        public static string NoDeliveryOnDate()
        {
            return String.Format("No Delivery Scheduled from Supplier for Specific Date!");
        }
        public static string CannotEditThisRecord(string userCode)
        {
            return string.Format("This record is Lock by user {0}", userCode.Trim());
        }        
        public static string NotUpdatedata()
        {
            return string.Format("Somebody is updating the records,It will automatically reload the Data!");
        }
        //public static string NotAllRecordsIsNew()
        //{
        //    return string.Format("Cannot Delete this Record because not all Detail status is NEW!");
        //}
        
        public static string OnlyHeaderCanDelete()
        {
            //return string.Format("Only the Header can Delete this record!");//deleted Roy April 5,2007 3PM 
            return string.Format("Invalid Operation. Delete the header in order to delete this record!");//added Roy April 5,2007 3PM
        }

        //public static string GenerateRR(string RRno)
        //{
        //    return String.Format("Generated Temporary RRno: \n TEMP{0}", RRno);
        //}

        //public static string GenerateRRTEMP(string RRno)
        //{
        //    return String.Format("Generated Temporary RRno: {0}", RRno);
        //}

        //public static string GenerateCPOTEMP(string CPOno)
        //{
        //    return String.Format("Generated Temporary CPO No.: {0}", CPOno);
        //}


        //public static string GenerateRR(string RRno, string RRno1)
        //{
        //    return String.Format("Generated Temporary RRno: \n TEMP{0} \n TEMP{1} ", RRno, RRno1);
        //}
        //public static string NoDataforRR(string supplierCode, string date)
        //{
        //    return String.Format("No Delivery Scheduled with:\n \nSupplier Name: {0} \nDate: {1}", supplierCode, date);
        //}
        public static string CodeNotExist(string code)
        {
            return string.Format("[{0}]: Code does not exist", code);
        }
        public static string CannotCancelThisRecord(string recordNum, string remarks)
        {
            return string.Format("Cannot CANCEL this Record {0} {1}", recordNum, remarks);

        }
        //public static string RecordMustHave(string code, string message)
        //{
        //    return string.Format("Cannot Use this STOCKCODE {0},because it has {1} ", code, message);
        //}

        public static string RecordMustHave(string code, string message, string message1)
        {
            return string.Format("Cannot Use this {0} {1},because is has {2}", message1, code, message);
        }
        public static string CannotSelectDateBeforeToday(string date)
        {
            return string.Format("Cannot select this Date {0} before the current DATE", date);
        }

        public static string InfoEmptyDelDate(string DRNo, string DRSeqNo)
        {
            return string.Format("[DRNo {0}:SeqNo {1}] You need to input a Delivery Answer Date for all SPLITS.", DRNo, DRSeqNo);
        }

        public static string InfoTotalQtyLessThanOrderQty(string DRNo, string DRSeqNo, string AnsQTY, string OrderQTY)
        {
            return string.Format("[PONo {0}:SeqNo {1}] Total Answer Quantity ({2}) should not be lesser than Order Quantity ({3}).", DRNo, DRSeqNo, AnsQTY, OrderQTY);
        }

        public static string InfoTotalQtyGreatThanOrderQty(string DRNo, string DRSeqNo, string AnsQTY, string OrderQTY)
        {
            return string.Format("[PONo {0}:SeqNo {1}] Total Answer Quantity ({2}) should be not be greater than Order Quantity ({3}).", DRNo, DRSeqNo, AnsQTY, OrderQTY);
        }

        public static string InvalidCode(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Code.", fieldName, value);
        }
        public static string InvalidQuantity(string fieldName, string value)
        {
            return String.Format("{0}[{1}]: Invalid Quantity.", fieldName, value);
        }
        public static string InvalidValues()
        {
            return String.Format("Invalid size values.");
        }

        public static string NoValues()
        {
            return String.Format("Please provide all necessary information except RR No.");
        }

        public static string NoValues2()
        {
            return String.Format("Please provide all information.");
        }

        public static string LocatorCostCenterConflict()
        {
            return String.Format("Cost Center Source and Target can't be the same.");
        }

        public static string PackSizeError()
        {
            return String.Format("Full Pack Size should be greater than Partial Pack Size.");
        }

        public static string LotNoShift()
        {
            return String.Format("Please provide shift schedule.");
        }

        public static string TravelogExist()
        {
            return String.Format("Travelog No. and Product Code already exists.");
        }
        //end

        public static string NoCostCenter()
        {
            return string.Format("LogIn User Has No Cost Center Assigned OR Has No Section assigned!");
        }

        public static string NoGroup()
        {
            return string.Format("Logged in User does not belong to any Group or Has No Rights. Please Contact Your Administrator.");
        }

        public static string GreaterThanZero(string fieldName)
        {
            return String.Format("{0}: Should be greater than 0.", fieldName);
        }

        public static string GreaterThanZero()
        {
            return String.Format("Incorrect input.\nInput number greater than 0.");
        }

        public static string GreaterThan(string fieldName1, string value1, string fieldName2, string value2)
        {
            return String.Format("{0}[{1}]: Should be greater than {2}[{3}].", fieldName1, value1, fieldName2, value2);
        }

        public static string NotGreaterThan(string fieldName1, string value1, string fieldName2, string value2)
        {
            return String.Format("{0}[{1}]: Should not be greater than {2}[{3}].", fieldName1, value1, fieldName2, value2);
        }

        public static string NegativeNumber(string fieldName)
        {
            return String.Format("{0}: Should not be a negative Number.", fieldName);
        }

        public static string PRDoesNotExist(string prNumber, string status)
        {
            return String.Format("{0} has been {1} by another user.", prNumber,status);
        }

        //public static string StockCodeCostCenterError()
        //{
        //    return String.Format("Cannot Add Same Stock And Cost Center.");
        //}

        public static string PrintingStatus()
        {
            return String.Format("Printing successful?");
        }

        public static string PrintingError()
        {
            return String.Format("Sorry you don't have rights to reprint this document.");
        }

        //public static string SaveRRError()
        //{
        //    return String.Format("Not enough stocks for this reservation.");
        //}

        public static string NotSetField(string field)
        {
            return String.Format("{0}: is not set.", field);
        }

        public static string MustBeBetween(string souceField, string fromString, string toString)
        {
            return String.Format("{0}: must between {1} and {2}", souceField, fromString, toString);
        }

        public static string InvoiceNotExist(string invoiceNum)
        {
            return String.Format("This Invoice Number {0} does not exits!",invoiceNum);
        }

        ////added by louie 20070228
        //public static string GeneratedRRNumber(string val)
        //{
        //    return String.Format("Generate RRNo: {0}",val);
        //}
        ////end

        public static string NoWILDCharacters(string val)
        {
            return String.Format("{0}:Please don't input Wild Characters such as : [ %  ",val);
        }

        public static string NoWorkDay(string field, string val)
        {
            return String.Format("{0}[{1}]: Non-workin day.", field, val);
        }
        public static string GenerateIITEMP(string IIno)
        {
            return String.Format("Generated Temporary Incoming Inspection No.: {0}", IIno);
        }

        //Added by Charlie 06/11/08
        //<start>
        public static string CycleNotOpen()
        {
            return string.Format("Current Cycle not open.");
        }

        public static string CycleClose()
        {
            return string.Format("Current Cycle already close.");
        }

        public static string AdjustNotPosted()
        {
            return string.Format("Adjustments not yet posted.");
        }

        public static string AllowanceNotPosted()
        {
            return string.Format("Allowances not yet posted.");
        }

        public static string PayrollCalculated()
        {
            return string.Format("Payroll already calculated.");
        }

        public static string PayrollOnCutoff()
        {
            return string.Format("Payroll is already on cut-off.");
        }
        //<end>
        public static string InvalidInput(string Fieldname)
        {
            return string.Format("Invalid {0}.", Fieldname);
        }

        public static string FieldNotExist(string Fieldname)
        {
            return string.Format("The {0} does not exist.", Fieldname);
        }

        public static string Empty(string Fieldname)
        {
            return string.Format("The {0} field is empty.", Fieldname);
        }

        public static string Existed(string Fieldname)
        {
            return string.Format("The {0} already exists.", Fieldname);
        }        
                
        public static string Deletion(string Field1, string value1)
        {
            return string.Format("Are you sure you want to Delete the record \n" +  Field1 + " : "  + value1 + " ?");
        }

        public static string Deletion(string Field1, string value1, string Field2, string value2)
        {
            return string.Format("Are you sure you want to Delete the record \n" + Field1 + " : " + value2 + ", " + Field2 + " : " + value2 + " ?");
        }

        public static string Deletion(string Field1, string value1, string Field2, string value2, string Field3, string value3)
        {
            return string.Format("Are you sure you want to Delete the record \n" + Field1 + " : " + value1 + ", " + Field2 + " : " + value2 + ", " + Field3 + " : " + value3 + " ?");
        }

        public static string NoRecordsFound(string PayPeriod, string IDNumber)
        {
            return string.Format("No records found for ID Number: " + IDNumber + " ,Payroll Period: " + PayPeriod + "\r\n  Create?" );
        }

        public static string RecordIsUsed(string Fieldname, string value)
        {
            return string.Format("Cannot delete [" + Fieldname + " :" + value + "]. \n It is being used by other records.");
        }
        public static string RecordIsUsedEdit(string Fieldname, string value)
        {
            return string.Format("Cannot edit [" + Fieldname + " :" + value + "]. \n It is being used by other records.");
        }

        public static string FormEditedAndMustReload(string ludatetime, string userlogin)
        {
            return string.Format("This record has already been modified by another person. \nPlease close this form and try again. \nLast Modified By: {0} \nLast Modified On: {1} ", userlogin, ludatetime);
        }

        public static string NoModification()
        {
            return string.Format("No modification(s) detected.");
        }

    }
}

