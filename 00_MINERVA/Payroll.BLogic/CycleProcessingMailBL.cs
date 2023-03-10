using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class CycleProcessingMailBL : BaseServiceBL
    {
        private String MenuCode;
        private String UserCode;
        private String CompanyCode;
        private String CentralProfile;

        public CycleProcessingMailBL(string _MenuCode, string _UserCode, string _CompanyCode, string _CentralProfile)
        {
            this.MenuCode = _MenuCode;
            this.UserCode = _UserCode;
            this.CompanyCode = _CompanyCode;
            this.CentralProfile = _CentralProfile;
        }

        public void SendMailTrail(DateTime dtStart, DateTime dtEnd)
        {
            string msgSubject = string.Empty;
            string msgDetail = string.Empty;

            SendMailTrail(dtStart, dtEnd, string.Empty);
        }

        public void SendMailTrail(DateTime dtStart, DateTime dtEnd, string msgSpecific)
        {
            string msgSubject = string.Empty;
            string msgDetail = string.Empty;
            string msgPeriod = string.Empty;

            using (Payroll.DAL.DALHelper dalHelper = new Payroll.DAL.DALHelper())
            {
                string sqlQuery = @"SELECT	CONVERT(CHAR(10), Tps_StartCycle, 101) 
		                                    + ' - ' 
		                                    + CONVERT(CHAR(10), Tps_EndCycle, 101)
                                    FROM	T_PaySchedule
                                    WHERE	Tps_CycleIndicator = 'C'";
                try
                {
                    dalHelper.OpenDB();
                    msgPeriod = Convert.ToString(dalHelper.ExecuteDataSet(sqlQuery).Tables[0].Rows[0][0]).Trim();
                    dalHelper.CloseDB();
                }
                catch
                {
                    msgPeriod = "Current Period";
                }
            }

            if (this.MenuCode.Equals(Convert.ToString(CommonEnum.MenuCode.DATABASEUTIL)))
            {
                msgSubject = "Cycle Closing DB Backup";
                msgDetail = "database backup before cycle closing has been successful.";
                if (!msgSpecific.Equals(string.Empty))
                    msgSpecific = "Data File: " + msgSpecific;
            }
            else if (this.MenuCode.Equals(Convert.ToString(CommonEnum.MenuCode.CYCLEOPENING)))
            {
                msgSubject = "New Payroll Successful";
                msgDetail = "new payroll has been successful.";
            }
            //else if (this.MenuCode.Equals(Convert.ToString(CommonEnum.MenuCode.YEARLYHOUSEKEEP)))
            //{
            //    msgSubject = "Yearly Housekeeping Successful";
            //    msgDetail = "yearly housekeeping has been successful.";
            //}
            else if (this.MenuCode.Equals(Convert.ToString(CommonEnum.MenuCode.BONUS)))
            {
                msgSubject = "Bonus Calculation Successful";
                msgDetail = "bonus calculation has been successful.";
            }
            else if (this.MenuCode.Equals(Convert.ToString(CommonEnum.MenuCode.PAYROLLCALC)))
            {
                msgSubject = "Payroll Calculation Successful";
                msgDetail = "payroll calculation for the period [ " + msgPeriod + " ] has been successful.";
            }
            else if (this.MenuCode.Equals(Convert.ToString(CommonEnum.MenuCode.IDMAINTENANCE)))
            {
                msgSubject = "Changed of ID Number Successful";
                msgDetail = "change of employee ID number from " + msgSpecific + " successful.";
            }

            SendMailTrail(msgSubject, msgDetail, string.Empty, dtStart, dtEnd);
        }

        public void SendMailTrail(string msgSubject, string msgDetail, DateTime dtStart, DateTime dtEnd)
        {
            SendMailTrail(msgSubject, msgDetail, string.Empty, dtStart, dtEnd);
        }

        public void SendMailTrail(string msgSubject, string msgDetail, string msgSpecific, DateTime dtStart, DateTime dtEnd)
        {
            string msgBody = GenericMailBody(msgDetail, dtStart, dtEnd, msgSpecific, this.UserCode);
            string msgFrom = GetUserEmail(this.UserCode);

            SendEmail(msgSubject, msgBody, this.MenuCode, msgFrom, string.Empty, this.CentralProfile, this.CompanyCode);
        }

        public bool SendMailPassword(string msgSubject, string msgBody, string msgFrom, string msgTo)
        {
            return SendEmail(msgSubject, msgBody, this.MenuCode, msgFrom, msgTo, this.CentralProfile, this.CompanyCode);
        }
        public bool SendMailTrailWithAttatchment(string emailTo, string msgSubject, string msgBody, string attachmentPath)
        {
            string msgFrom = GetUserEmail(this.UserCode);
            string msgTo = emailTo;

            return SendEmailWithAttachment(msgSubject, msgBody, this.MenuCode, msgTo, msgFrom, attachmentPath, this.CentralProfile, this.CompanyCode);
        }

        public bool SendMailTrailWithBinaryData(string emailTo, string msgSubject, string msgBody, byte[] data, string IDNumber, bool IsAlternate, string EmailPassword, string PayCycle)
        {
            string msgFrom = GetUserEmail(this.UserCode);
            string msgTo = emailTo;

            return SendEmailWithBinaryData(msgSubject, msgBody, this.MenuCode, msgTo, msgFrom, data, IDNumber, IsAlternate, EmailPassword, this.CentralProfile, this.CompanyCode, PayCycle);
        }

        public string SendMailTrailWithBinaryDataString(string emailTo, string msgSubject, string msgBody, byte[] data, string IDNumber, bool IsAlternate, string EmailPassword, string PayCycle)
        {
            string msgFrom = GetUserEmail(this.UserCode);
            string msgTo = emailTo;

            return SendMailTrailWithBinaryDataString(msgSubject, msgBody, this.MenuCode, msgTo, msgFrom, data, IDNumber, IsAlternate, EmailPassword, this.CentralProfile, this.CompanyCode, PayCycle);
        }
    }
}
