using System;
using System.Data;
using Posting.DAL;
using System.Drawing;

namespace UploadDownloadSchedulerConsole
{
    public class HRCReportsBL : Posting.BLogic.BaseBL
    {

        #region main overrides
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

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        //#reports
        #region reports
        private string[] reports = new string[13] { "Other Income and Benefits Report",
        "Deduction and Loan Report",
        "Deduction and Loan Ledger Report",
        "Adjustment Report",
        "Log Report",
        "Assume Present Report",
        "Overtime Report",
        "Leave Availment Report",
        "Leave Ledger Report",
        "DTR Report",
        "Work Location Report",
        "Restday Report",
        "Post-Payroll Report" };
        #endregion

        //#menucodes
        #region menucodes
        private string[] menucodes = new string[13] { "OTHERINCOMEREP",
        "DEDUCTIONREP",
        "DEDUCTNLEDGRREP",
        "ADJUSTMENTREP",
        "LOGREP",
        "ASSUMEPRESREP",
        "OVERTIMEREP",
        "LEAVEAVAILREP",
        "LEAVELEDGERREP",
        "RAWDTRREP",
        "WORKLOCREP",
        "RESTDAYREP",
        "POSTPAYCALCREP" };
        #endregion

        //#systemID 
        #region systemID
        public string[] SystemIDs = new string[13] { "PAYROLL",
        "PAYROLL",
        "PAYROLL",
        "PAYROLL",
        "TIMEKEEP",
        "TIMEKEEP",
        "OVERTIME",
        "LEAVE",
        "LEAVE",
        "TIMEKEEP",
        "TIMEKEEP",
        "TIMEKEEP",
        "PAYROLL" };
        #endregion

        public string[] ReturnMenuCodes()
        {
            return menucodes;
        }

        public string[] ReturnSystemIDs()
        {
            return SystemIDs;
        }

        public string[] ReturnReports()
        {
            return reports;
        }

        public bool CheckIfMenucodeExists(string menu)
        {
            bool flag = false;
            for (int i = 0; i < menucodes.Length && !flag; i++)
            {
                if (menucodes[i] == menu)
                    flag = true;
            }
            return flag;
        }

        public DataSet getHeaderData()
        {
            DataSet ds = new DataSet();

            #region query

            string query = @"
                            declare @processflag1 as bit
                            set @processflag1 = (select pcm_processflag from t_processcontrolmaster
                            where pcm_systemid = 'PERSONNEL' and pcm_processid = 'VWNICKNAME')

                            declare @processflag2 as bit
                            set @processflag2 = (select pcm_processflag from t_processcontrolmaster
                            where pcm_systemid = 'PERSONNEL' and pcm_processid = 'DSPIDCODE')

                            Select Ccd_CompanyName
                                  ,Ccd_CompanyAddress1 + ' ' + Ccd_CompanyAddress2 + ', ' + Adt_AccountDesc as Address
                                  ,'TEL NO. ' + Ccd_TelephoneNo + ' FAX NO. ' + Ccd_FaxNo as Contacts
                                  ,Ccd_CompanyLogo  
                                  
                            From T_CompanyMaster
                            Inner Join T_AccountDetail on Ccd_CompanyAddress3 = Adt_AccountCode and Adt_AccountType='ZIPCODE'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int[] getMargins()
        {
            int[] margs = new int[4] { 50, 50, 50, 50 };
            return margs;
        }

        public string EncodeFilterItems(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            string strFilterItem = "";
            foreach (string col in strArrFilterItems)
            {
                strFilterItem += "'" + col.Trim() + "',";
            }
            if (strFilterItem != "")
                strFilterItem = strFilterItem.Substring(0, strFilterItem.Length - 1);
            return strFilterItem;
        }

        private string SetUpItemsForEncodeFilterItems(DataTable dt)
        {
            string s = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0)
                    s += ",";
                s += dt.Rows[i][0].ToString().Trim();
            }
            return s;
        }

        public Font getReportFont()
        {
            Font font = new Font("Times New Roman", 8f);
            return font;
        }

        public string[] GetColumnsNotIncludedInSummary()
        {
            string[] cols = new string[57] { "Profiles",
            "ID Number",
            "IDNumber",
            "Employee Name",
            "Costcenter",
            "Tax Code",
            "Description",
            "Payperiod",
            "PayPeriod",
            "ThisPayperiod",
            "This Payperiod",
            "ThisPeriod",
            "LeaveYear",
            "Location",
            "ActualTimeIn1",
            "ActualTimeIn2",
            "ActualTimeOut1",
            "ActualTimeOut2",
            "MinutesLogIN1beforeshift",
            "MinutesLogIN2beforeshift",
            "MinutesLogOUT1aftershift",
            "MinutesLogOUT2aftershift",
            "StartTime",
            "EndTime",
            "Checker1",
            "Checker2",
            "Approver",
            "OvertimeFlag",
            "LeaveFlag",
            "LeaveCategory",
            "LeaveCode",
            "WorkLocation",
            "RestDay",
            "Remarks",
            "Costcenter",
            "Description",
            "Fullname",
            "FullName",
            "PayrollType",
            "EmploymentStatus",
            "Daycode",
            "WorkType",
            "LogDate",
            "WorkGroup",
            "DayCode",
            "ShiftCode",

            "LogType",
            "JobStatus",

            "Division",
            "Department",
            "Section",
            "Profilename",
            "DeductionType",
            "VoucherNumber",
            "DeductionCode",

            "PaymentType",
            "StartofDeduction" };
            return cols;
        }
    }
}
