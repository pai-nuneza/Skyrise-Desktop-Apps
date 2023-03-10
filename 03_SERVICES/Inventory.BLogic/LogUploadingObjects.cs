using System;
using System.Collections.Generic;
using System.Text;

using CommonPostingLibrary;

namespace Posting.BLogic
{
    public enum LogTypes
    {
        IN,
        OUT
    }

    public enum LedgerLogTypes
    {
        UNDEFINED,
        IN1,
        IN2,
        OUT1,
        OUT2,
        EXTENSIONIN,
        EXTENSIONOUT
    }

    public enum ScheduleTypes
    {
        DAY,
        GRAVEYARD,
        SWING
    }

    public enum RecordType
    {
        RETRIEVED,
        NEW,
        UPDATED
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
        public bool IsUnprocessed;
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
        public bool IsRestday;
        public TimeEntry ShiftTime = new TimeEntry();
        public TimeEntry ShiftBreak = new TimeEntry();
        public TimeEntry LogTime1 = new TimeEntry();
        public TimeEntry LogTime2 = new TimeEntry();
        public LedgerLogTypes LastLogType = LedgerLogTypes.UNDEFINED;
        public int isNormalShift;
        public List<LedgerExtension> LedgerExtension = new List<LedgerExtension>();
        public bool IsForPosting = false;
        public bool IsSoftPosting = false;
    }

    public class LedgerExtension
    {
        public string EmployeeID;
        public DateTime ProcessDate;
        public string PayPeriod;
        public int SequenceNumber;
        public TimeEntry LogTime = new TimeEntry();
        public string UpdatedBy = "SERVICE";
        public RecordType RecordType;
    }
}
