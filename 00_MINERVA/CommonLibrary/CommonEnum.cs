using System;
using System.Collections.Generic;
using System.Text;
 
namespace CommonLibrary
{
    public class CommonEnum
    {
        public enum Version
        {
            [StringValue("4.0.0.0")]
            SystemVer = 0
        }

        public enum MenuCode
        {
            //Cut-off
            [StringValue("CYCLEOPENING")]
            CYCLEOPENING = 1,
            [StringValue("CYCLECLOSING")]
            CYCLECLOSING = 2,
            [StringValue("TIMECUTOFF")]
            TIMECUTOFF = 3,
            [StringValue("OVERTIMECUTOFF")]
            OVERTIMECUTOFF = 4,
            [StringValue("LEAVECUTOFF")]
            LEAVECUTOFF = 5,
            [StringValue("PAYROLLCUTOFF")]
            PAYROLLCUTOFF = 6,
            [StringValue("CYCLECUTOFF")]
            CYCLECUTOFF = 7,
            [StringValue("ALLCUTOFF")]
            ALLCUTOFF = 8,
            
            //Code Dtl Master
            [StringValue("CODEMASTER")]
            CODEMASTER = 9,
            //Employee
            [StringValue("EMPPAYMASTER")]
            EMPPAYMASTER = 10,
            [StringValue("MOVEMENTMASTER")]
            MOVEMENTMASTER = 11,

            [StringValue("OFFCYCLEOPENING")]
            OFFCYCLEOPENING = 12,
            [StringValue("OFFCYCLECLOSING")]
            OFFCYCLECLOSING = 13,

            //Processes
            [StringValue("PAYROLLCALC")]
            PAYROLLCALC = 14,
            [StringValue("LABORCOST")]
            LABORCOST = 15,
            [StringValue("OFFCYCLEPAYROLL")]
            OFFCYCLEPAYROLL = 16,
            [StringValue("FINALPAY")]
            FINALPAY = 17,
            [StringValue("ALPHALIST")]
            ALPHALIST = 18,
            [StringValue("YTD")]
            YTD = 19,

            [StringValue("BONUS")]
            BONUS = 20,
            [StringValue("LEAVEREFUND")]
            LEAVEREFUND = 21,
            [StringValue("PERFECTATTENDANCE")]
            PERFECTATTENDANCE = 22,
            [StringValue("RETROADJ")]
            RETROADJ = 23,

            [StringValue("LEAVECREDIT")]
            LEAVECREDIT = 24,
            [StringValue("LEAVECLOSE")]
            LEAVECLOSE = 25,
            
            //Entry
            [StringValue("TIMEREGISTER")]
            TIMEREGISTER = 26,
            [StringValue("INCOME")]
            INCOME = 27,
            [StringValue("DEDUCTION")]
            DEDUCTION = 28,
            [StringValue("ADJUSTMENT")]
            ADJUSTMENT = 29,
            [StringValue("PAYROLLTRN")]
            PAYROLLTRN = 30,

            //Reports
            [StringValue("PAYROLLSLIP")]
            PAYROLLSLIP = 31,
            [StringValue("PAYREMITTANCE")]
            PAYREMITTANCE = 32,
            [StringValue("GOVREMITTANCE")]
            GOVREMITTANCE = 33,
            [StringValue("BIR1601C")]
            BIR1601C = 34,

            //Utilities
            [StringValue("REPORTBUILDER")]
            REPORTBUILDER = 35,
            [StringValue("DATABASEUTIL")]
            DATABASEUTIL = 36,
            [StringValue("IDMAINTENANCE")]
            IDMAINTENANCE = 37,
            [StringValue("DOCPRINTING")]
            DOCPRINTING = 38,
            [StringValue("DATAUTIL")]
            DATAUTIL = 39
        }

        public enum FormState
        {
            [StringValue("NORMAL_STATE")]
            NORMAL_STATE = 0,
            [StringValue("NEW_STATE")]
            NEW_STATE = 1,
            [StringValue("MODIFY_STATE")]
            MODIFY_STATE = 2
        }

        public enum ConditionalOperators
        {
            [StringValue("=")]
            EQUAL = 0,
            [StringValue("<>")]
            NOT_EQUAL = 1,
            [StringValue(">")]
            GREATER_THAN = 2,
            [StringValue(">=")]
            GREATER_OR_EQUAL = 3,
            [StringValue("<")]
            LESS_THAN = 4,
            [StringValue("<=")]
            LESS_OR_EQUAL = 5,
            [StringValue("LIKE")]
            LIKE = 6
        }

        public enum Processes
        { 
            TIME
            ,
            PAYROLL
            ,
            LEAVE
            ,
            OVERTIME
            ,
            CYCLE
        }

        public enum GenieMessageBoxIcon
        { 
            WARNING
            ,
            ERROR
            ,
            CONFIRMATION
            ,
            INFORMATION
        }

        public enum GenieDialogResult
        { 
            Yes
            ,
            No
            ,
            Cancel
        }

        public enum LogicalOperators
        {
            [StringValue("AND")]
            AND = 1,
            [StringValue("OR")]
            OR = 2,
            [StringValue("NOT")]
            NOT = 3
        }

        public enum PaymentType
        {
            [StringValue("A")]
            ADVANCED = 0,
            [StringValue("D")]
            DEFERRED = 1,
            [StringValue("X")]
            SETUP = 2
        }

        public enum LeaveFlag
        {
            [StringValue("U")]
            UNPOSTED = 0,
            [StringValue("C")]
            CURRENT = 1,
            [StringValue("P")]
            PAST = 2,
            [StringValue("F")]
            FUTURE = 3,
            [StringValue("A")]
            AMEND = 4
        }

        public enum Status
        {
            [StringValue("A")]
            ACTIVE = 0,
            [StringValue("C")]
            CANCELLED = 1,
            [StringValue("U")]
            ONHOLD = 2,
            [StringValue("F")]
            FULFILLED = 3,
            [StringValue("N")]
            NEW = 4,
            [StringValue("A")]
            APPROVED = 5,
            [StringValue("G")]
            GENERATED = 6,
            [StringValue("A")]
            AMENDED = 7,
            [StringValue("F")]
            FOR_POSTING = 8,
            [StringValue("P")]
            POSTED = 9,
            [StringValue("L")]
            LOADED = 10,
            [StringValue("I")]
            INTRANSIT = 11,
            [StringValue("R")]
            REVIEWED = 12,
            [StringValue("X")]
            EXPAND = 13,
            [StringValue("A")]
            ALL = 14,
            [StringValue("B")]
            BILLABLE = 15,
            [StringValue("N")]
            NONBILLABLE = 16
        }        

        public enum Origin
        {
            [StringValue("L")]
            LOCAL = 0,
            [StringValue("E")]
            FOREIGN = 1  
        }
        
        public enum UsageType
        {
            [StringValue("C")]
            COMMON = 0,
            [StringValue("D")]
            DEDICATED = 1
        }

        public enum CriticalBalanceCategory
        {
            [StringValue("L")]
            LOW = 0,
            [StringValue("M")]
            MID = 1,
            [StringValue("H")]
            HIGH = 2
        }

        public enum BankCode
        {
            [StringValue("METROBANK")]
            METROBANK = 1,
            [StringValue("SBC")]
            SBC = 2,
            [StringValue("BPI")]
            BPI = 3,
            [StringValue("BDO")]
            BDO = 4,
        }

        public enum Shipping
        {
            [StringValue("AIR", "AIR")]
            AIR = 0,
            [StringValue("LAND", "LAND")]
            LAND = 1,
            [StringValue("SEA", "SEA")]
            SEA = 2,
            [StringValue("HAND-CARRY", "HAND-CARRY")]
            HAND_CARRY = 3
        }

        public enum PartsGen
        {
            [StringValue("T", "GENERATED")]
            GENERATED = 0,
            [StringValue("N", "NOT YET GENERATED")]
            NOT_YET_GENERATED = 1
        }

        public enum CorrectiveActConfirm
        {
            [StringValue("1")]
            Accepted = 1,
            [StringValue("0")]
            Rejected = 0
        }

        public enum GenericCBType
        {
            [StringValue("CURRENCY")]
            CURRENCY = 1,
            [StringValue("BLOODTYPE")]
            BLOODTYPE = 2,
            [StringValue("STATUS")]
            STATUS = 3,
            [StringValue("STATUS_NC")]
            STATUS_NC = 4,
            [StringValue("STATUS_AC")]
            STATUS_AC = 5,
            [StringValue("STATUS_AOC")]
            STATUS_AOC = 6,
            [StringValue("SHIPPING")]
            SHIPPING = 7,
            [StringValue("COR_ACT_STAT")]
            COR_ACT_STAT = 8,
            [StringValue("ORIGIN")]
            ORIGIN = 9,
            [StringValue("USAGE_TYPE")]
            USAGE_TYPE = 10,
            [StringValue("BALANCE_CATEGORY")]
            BALANCE_CATEGORY = 11,
            [StringValue("GENERIC_TYPE")]
            GENERIC_TYPE = 12,
            [StringValue("PARTS_GEN")]
            PARTS_GEN = 13,
            [StringValue("STATUS_NFOC")]
            STATUS_NFOC = 14,
            [StringValue("STATUS_ANC")]
            STATUS_ANC = 15,
            [StringValue("STATUS_AOCF")]
            STATUS_AOCF = 16,
            [StringValue("STATUS")]
            STATUS1 = 17,
            [StringValue("STATUS_NCAF")]
            STATUS_NCAF = 18,
            [StringValue("STATUS_NCAU")]
            STATUS_NCAU = 19,
            [StringValue("AFCO_STATUS")]
            AFCO_STATUS = 20,
            [StringValue("STATUS_AFC")]
            STATUS_AFC = 21,
            [StringValue("CRITICAL_BALANCE")]
            CRITICAL_BALANCE = 22,
            [StringValue("Calendar_Scope")]
            Calendar_Scope = 23,  
            [StringValue("GENDER")]
            GENDER = 24,
            [StringValue("CIVILSTATUS")]
            CIVILSTATUS = 25,
            [StringValue("EMPLOYMENTSTATUS")]
            EMPLOYMENTSTATUS = 26,
            [StringValue("WORKTYPE")]
            WORKTYPE = 27,
            [StringValue("JOBSTATUS")]
            JOBSTATUS = 28,
            [StringValue("PAYMENTMODE")]
            PAYMENTMODE = 29,
            [StringValue("PAYMENTTYPE")]
            PAYMENTTYPE = 30,
            [StringValue("TAXCODE")]
            TAXCODE = 31,
            [StringValue("TRUEORFALSE")]
            TRUEORFALSE = 32,
            [StringValue("POSITION_COMBOBOX")]
            POSITION_COMBOBOX = 33,
            [StringValue("LEAVETYPE_COMBOBOX")]
            LEAVETYPE_COMBOBOX = 34,
            [StringValue("PAYTYPE_COMBOBOX")]
            PAYTYPE_COMBOBOX = 35,
            [StringValue("EDUC_LEVEL")]
            EDUC_LEVEL = 36,
            [StringValue("TAXCODENEW")]
            TAXCODENEW = 37,
            [StringValue("PAYROLLTYPE")]
            PAYROLLTYPE = 38,
            [StringValue("JOBSTATUSNEW")]
            JOBSTATUSNEW = 39,
            [StringValue("WORKTYPENEW")]
            WORKTYPENEW = 40,
            [StringValue("STATUS_MASTER")] 
            STATUS_MASTER = 41,             
            [StringValue("DATEINDICATOR")] 
            DATEINDICATOR = 42,
            [StringValue("TrainingType")]
            TrainingType = 43,
            [StringValue("TaxSchedule")]
            TaxSchedule = 44,
            [StringValue("TaxClass")]
            TaxClass = 45,
            [StringValue("ScheduleType")]
            ScheduleType = 46,
            [StringValue("HDMFCODE")]
            HDMFCODE = 47,
            [StringValue("SearchEmployStat")]
            SearchEmployStat = 48,
            [StringValue("SearchWrkType")]
            SearchWrkType = 49,
            [StringValue("SearchJobStat")]
            SearchJobStat = 50,
            [StringValue("SearchGender")]
            SearchGender = 51,
            [StringValue("DeductionType")]
            DeductionType = 52,
            [StringValue("EDUC_LEVELWBLNK")]
            EDUC_LEVELWBLNK = 53,
            [StringValue("MENU_TYPE")]
            MENU_TYPE = 54,
            [StringValue("MENU_TYPEALL")]
            MENU_TYPEALL = 55,
            [StringValue("NightPremium")]
            NightPremium = 56,
            [StringValue("GENDERWBLNK")]
            GENDERWBLNK = 57,
            [StringValue("CIVILSTATUSWBLNK")]
            CIVILSTATUSWBLNK = 58,
            [StringValue("APPLICABLEPAYPERIOD")]
            APPLICABLEPAYPERIOD = 59,
            [StringValue("PREMIUMCODES")]
            PREMIUMCODES = 60,
            [StringValue("DEDUCTTYPENOALL")]
            DEDUCTTYPENOALL = 61,
            [StringValue("Payrate")]
            Payrate = 62,
            [StringValue("EMPLOYEETYPE")]
            EMPLOYEETYPE = 63,
            [StringValue("SpecialPayPeriod")] //Jule Added 20090116
            SpecialPayPeriod = 64,
            [StringValue("ForBonusComputationPayPeriod")]
            ForBonusComputationPayPeriod = 65,
            [StringValue("YearList")]
            YearList = 66,
            [StringValue("YEAR")]//Gerry Added 20091125
            YEAR = 67,
            [StringValue("OCCURRENCE")]
            OCCURRENCE = 68,
            [StringValue("PastAndCurrentPayPeriod")]
            PastAndCurrentPayPeriod = 69,
            [StringValue("CCTRTYPE")]
            CCTRTYPE = 70,
            [StringValue("BILLABLE")]
            BILLABLE = 71,
            [StringValue("relational")]
            relational = 72,
            [StringValue("PAYROLLTYPE1")]
            PAYROLLTYPE1 = 73
        }

        public enum LoginFormType
        {
            NORMAL_LOGIN = 0,
            LOG_OUT = 1,
            LOCK = 2
        }

        public enum DateIndicator
        {
            [StringValue("P")]
            PAST = 0,
            [StringValue("C")]
            CURRENT = 1,
            [StringValue("F")]
            FUTURE = 2,
            [StringValue("S")]
            SPECIAL = 3
        }

        public enum TrainingType
        {
            [StringValue("I")]
            Internal = 0,
            [StringValue("E")]
            External = 1
        }

        public enum EmploymentStatus
        {
            [StringValue("PB")]
            PROBATIONARY = 0,
            [StringValue("RG")]
            REGULAR = 1,
            [StringValue("CT")]
            CONTRACTUAL = 2,
            [StringValue("SN")]
            SEASONAL = 3
        }

        public enum JobStatusNew
        {
            [StringValue("AC")]
            ACTIVE_CURRENT = 0,
            [StringValue("AL")]
            ACTIVE_ON_LEAVE = 1,
            [StringValue("AM")]
            ACTIVE_MANAGER = 2,
            [StringValue("IN")]
            INACTIVE = 3
        }

        public enum TAXCODE
        {
            [StringValue("S")]
            SINGLE = 0,
            [StringValue("HF")]
            HEAD_OF_THE_FAMILY = 1,
            [StringValue("ME")]
            MARRIED = 2,
            [StringValue("EH")]
            EMPLOYED_HUSBAND = 3,
            [StringValue("EW")]
            EMPLOYED_WIFE = 4
        }

        public enum TaxClass
        {
            [StringValue("T")]
            TAXABLE = 0,
            [StringValue("N")]
            NONTAXABLE = 1
        }

        public enum ScheduleType
        {
            [StringValue("M")]
            MORNING = 0,
            [StringValue("A")]
            AFTERNOON = 1,
            [StringValue("G")]
            GRAVEYARD = 2

        }

        public enum DeductionType
        {
            [StringValue("C")]
            COMPANY = 0,
            [StringValue("G")]
            GOVERNMENT_MANDATED = 1,
            [StringValue("L")]
            GOVERNMENT_LOANS = 2,
            [StringValue("E")]
            EXTERNAL_PARTY = 3
        }

        public enum TaxSchedule
        {
            [StringValue("D")]
            DAILY = 0,
            [StringValue("M")]
            MONTHLY = 1,
            [StringValue("S")]
            SEMI_MONTHLY = 2,
            [StringValue("W")]
            WEEKLY = 3
        }

        public enum NightPremium
        {            
            [StringValue("O")]
            OVERTIME = 0,
            [StringValue("R")]
            REGULAR = 1
        }

        public enum APPLICABLEPAYPERIOD
        {
            [StringValue("Both")]
            Both = 0,
            [StringValue("1")]
            First = 1,
            [StringValue("2")]
            Second = 2
        }

        public enum PayrollType1
        {
            [StringValue("M")]
            MONTHLY = 0,
            [StringValue("D")]
            DAILY = 1,
            [StringValue("H")]
            HOURLY = 2,
            [StringValue("A")]
            ALL = 3
        }

    }

    public enum LogTypes
    {
        IN,
        OUT
    }

    public enum ScheduleTypes
    {
        DAY,
        GRAVEYARD,
        SWING
    }

    public struct TimeEntry
    {
        public DateTime TimeIn;
        public DateTime TimeOut;
    }

    public class EmployeeDTR
    {
        public string EmployeeID;
        public DateTime LogDateTime;
        public LogTypes LogType;
        public string DayCode;
        public string ShiftCode;
        public bool IsHoliday;
        public bool IsRestDay;
        public bool IsPosted;
        public TimeEntry ShiftTime = new TimeEntry();
        public TimeEntry ShiftBreak = new TimeEntry();

        public DateTime DTRLedgerID = DateTime.MinValue; //Serves as the ID of the Ledger if DTR has been posted or not
    }

    public class EmployeeLedger
    {
        public string EmployeeID;
        public DateTime ProcessDate;
        public string PayPeriod;
        public string DayCode;
        public string ShiftCode;
        public bool IsHoliday;
        public bool RestDay;
        public TimeEntry ShiftTime = new TimeEntry();
        public TimeEntry ShiftBreak = new TimeEntry();
        public TimeEntry LogTime1 = new TimeEntry();
        public TimeEntry LogTime2 = new TimeEntry();

        public List<LedgerExtension> LedgerExtension = new List<LedgerExtension>();
    }

    public class LedgerExtension
    {
        public string EmployeeID;
        public DateTime ProcessDate;
        public string PayPeriod;
        public int SequenceNumber;
        public TimeEntry LogTime = new TimeEntry();
        public string UpdatedBy = "SERVICE";
    }

}
