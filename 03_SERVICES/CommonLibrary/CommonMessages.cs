using System;

namespace CommonPostingLibrary
{
    public class CommonMessages
    {
        public static string ERPRecordsAffected(int val)
        {
            return String.Format("{0} ERP Database Records Affected", val);
        }
        public static string JapanRecordsAffected(int val)
        {
            return String.Format("{0} Japan Database Records Affected", val);
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

        public static string Deleted(string recordName)
        {
            return String.Format("Deleting {0} Record Completed.", recordName);
        }

        public static string InconsistentCurrency(string fieldName)
        {
            return String.Format("{0}: Inconsistent Currency.");
        }

        public static string NoCurrencyConversion()
        {
            return String.Format("Please set currency conversion first.");
        }
        public static string NoCodeAssigned()
        {
            return String.Format("Please assign code for this transaction first.");
        }
        public static string AddOneDetail()
        {
            return string.Format("Please ADD one detail before saving!");
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
        public static string NotAllRecordsIsNew()
        {
            return string.Format("Cannot Delete this Record because not all Detail status is NEW!");
        }
        public static string OnlyHeaderCanDelete()
        {
            //return string.Format("Only the Header can Delete this record!");//deleted Roy April 5,2007 3PM 
            return string.Format("Invalid Operation. Delete the header in order to delete this record!");//added Roy April 5,2007 3PM
        }

        public static string GenerateRR(string RRno)
        {
            return String.Format("Generated Temporary RRno: \n TEMP{0}", RRno);
        }

        public static string GenerateRRTEMP(string RRno)
        {
            return String.Format("Generated Temporary RRno: {0}", RRno);
        }

        public static string GenerateCPOTEMP(string CPOno)
        {
            return String.Format("Generated Temporary CPO No.: {0}", CPOno);
        }


        public static string GenerateRR(string RRno, string RRno1)
        {
            return String.Format("Generated Temporary RRno: \n TEMP{0} \n TEMP{1} ", RRno, RRno1);
        }
        public static string NoDataforRR(string supplierCode, string date)
        {
            return String.Format("No Delivery Scheduled with:\n \nSupplier Name: {0} \nDate: {1}", supplierCode, date);
        }
        public static string CodeNotExist(string code)
        {
            return string.Format("[{0}]: Code does not exist", code);
        }
        public static string CannotCancelThisRecord(string recordNum, string remarks)
        {
            return string.Format("Cannot CANCEL this Record {0} {1}", recordNum, remarks);
        }
        public static string RecordMustHave(string code, string message)
        {
            return string.Format("Cannot Use this STOCKCODE {0},because it has {1} ", code, message);
        }

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
            return String.Format("{0} has been {1} by another user.", prNumber, status);
        }

        public static string StockCodeCostCenterError()
        {
            return String.Format("Cannot Add Same Stock And Cost Center.");
        }

        public static string PrintingStatus()
        {
            return String.Format("Printing successful?");
        }

        public static string PrintingError()
        {
            return String.Format("Sorry you don't have rights to reprint this document.");
        }

        public static string SaveRRError()
        {
            return String.Format("Not enough stocks for this reservation.");
        }

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
            return String.Format("This Invoice Number {0} does not exits!", invoiceNum);
        }

        public static string GeneratedRRNumber(string val)
        {
            return String.Format("Generate RRNo: {0}", val);
        }

        public static string NoWILDCharacters(string val)
        {
            return String.Format("{0}:Please don't input Wild Characters such as : [ %  ", val);
        }

        public static string NoWorkDay(string field, string val)
        {
            return String.Format("{0}[{1}]: Non-workin day.", field, val);
        }
        public static string GenerateIITEMP(string IIno)
        {
            return String.Format("Generated Temporary Incoming Inspection No.: {0}", IIno);
        }

        public static string LoginFailed(string DocType)
        {
            return String.Format("Good day! \n\nLogin-In to FTP failed during execution of {0}.  \nPlease check Connection with the FTP. \n\n\n\nRespectully, \n\ni-merge@fujielectric.com.my", DocType);
        }

        public static string LoginFailed(string DocDesc, Exception Error, string server)
        {
            return String.Format("Good day! \n\nThis is a system generated message to inform you that Login-In to FTP failed during execution of {0}.\n\nError Message: {1} \nError Desc: {2}   \n\nPlease check Connection with the ftp://{3}. \n\n\nRespectfully, \n\ni-merge@fujielectric.com.my", DocDesc, Error.Message, Error.InnerException, server);
        }

        public static string NoDataInFTP(string DocType)
        {
            return String.Format("Good day! \n\nUnable to Process {0} for Parsing. \nEither there is no OK file or there are no files in the FTP. \n\n\nRespectfully, \n\ni-merge@fujielectric.com.my", DocType);
        }

        public static string DownloadFailed(string filename, Exception Error)
        {
            return String.Format("Good day! \n\nDownloading {0} from FTP failed. \n\nError Message: {1} \nError Desc: {2}   \n\n \n\n\nRespectully, \n\ni-merge@fujielectric.com.my", filename, Error.Message, Error.InnerException.Message);
        }

        public static string DownloadFailed(string filename)
        {
            return String.Format("Good day!\n\n\nThis is a system generated message to inform you that downloading {0} from FTP failed. \n\nThese are the download statistics: \n\nDateTime Started: {1}\n\nError:", filename, DateTime.Now.ToString());
        }
        public static string DownloadCancelled(string filename)
        {
            return String.Format("Good day!\n\n\nThis is a system generated message to inform you that downloading {0} from FTP Cancelled. \n\nThese are the download statistics: \n\nDateTime Started: {1}\n\nError:", filename, DateTime.Now.ToString());
        }



        public static string GenericEmailFooter()
        {
            return String.Format("\n\n\nRespectfully Yours, \n\ni-merge@fujielectric.com.my");
        }

        public static string GenericDownloadBody(string docType, string Result, string Detail)
        {
            return string.Format("Good day!\n\n\nThis is a system generated message to inform you that downloading {0} from FTP {1}.\n\nDateTime Download: {2}\n{3}", docType, Result, DateTime.Now.ToString(), Detail) + GenericEmailFooter();
        }

        public static string UploadSuccess(string docType, string FTP, string detail)
        {
            return String.Format("Good day!\n\n\nThis is a system generated message to inform you that uploading {0} to {1} successful. \n\nThese are the upload statistics: \n\nDate and time Upload: {2}\n\n{3}", docType, FTP, DateTime.Now.ToString(), detail) + GenericEmailFooter();
        }

        public static string UploadCancelled(string filename)
        {
            return String.Format("Good day!\n\n\nThis is a system generated message to inform you that uploading {0} to FTP failed. \n\nThese are the download statistics: \n\n- DateTime Started: {1}", filename, DateTime.Now.ToString());
        }

        public static string UploadFailed(string docType, string FTP, string detail)
        {
            return String.Format("Good day!\n\n\nThis is a system generated message to inform you that uploading {0} to {1} failed. \n\nThese are the upload statistics: \n\nDate and time Upload: {2}\n\n{3}", docType, FTP, DateTime.Now.ToString(), detail) + GenericEmailFooter();
        }
    }
}

