using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class SystemMenuLogBL : BaseBL
    {
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

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region View
        public static int InsertViewLog(string menuCode, bool isSuccess)
        {
            return InsertViewLog(menuCode, isSuccess, LoginInfo.getUser().UserCode);
        }
        public static int InsertViewLog(string menuCode, bool isSuccess, string userCode)
        {
            return InsertLog(menuCode, "V", isSuccess, userCode, "", false);
        } 
        #endregion

        #region Add
        public static int InsertAddLog(string menuCode, bool isSuccess)
        {
            return InsertAddLog(menuCode, isSuccess, "", false);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential)
        {
            return InsertAddLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, "N");
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertAddLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, CycleType);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertLog(menuCode, "A", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType);
        }

        public static int InsertAddLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType, DALHelper dalhelper)
        {
            return InsertLog(menuCode, "A", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType, dalhelper);
        }
        #endregion

        #region Edit
        public static int InsertEditLog(string menuCode, bool isSuccess, bool isPayrollDifferential)
        {
            return InsertEditLog(menuCode, isSuccess, "", isPayrollDifferential);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID)
        {
            return InsertEditLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, false, "N");
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential)
        {
            return InsertEditLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, "N");
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertEditLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, CycleType);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertLog(menuCode, "E", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType);
        }

        public static int InsertEditLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType, DALHelper dalhelper)
        {
            return InsertLog(menuCode, "E", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType, dalhelper);
        }

        public static int InsertEditLog(string menuCode, bool isSuccess, UniqueList<string> employeeIDList, string userCode, bool isPayrollDifferential, string CycleType)
        {
            foreach (string employeeID in employeeIDList)
            {
                InsertLog(menuCode, "E", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType);
            }
            return 1;
        }
        #endregion

        #region Delete
        public static int InsertDeleteLog(string menuCode, bool isSuccess)
        {
            return InsertDeleteLog(menuCode, isSuccess, "", false);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential)
        {
            return InsertDeleteLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, "N");
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertDeleteLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, CycleType);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertLog(menuCode, "D", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType);
        }

        public static int InsertDeleteLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType, DALHelper dalhelper)
        {
            return InsertLog(menuCode, "D", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType, dalhelper);
        }
        #endregion

        #region Generate
        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode)
        {
            return InsertGenerateLog(menuCode, isSuccess, userCode, "", false);
        }

        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            return InsertGenerateLog(menuCode, isSuccess, userCode, employeeID, isPayrollDifferential, "N");
        }

        public static int InsertGenerateLog(string menuCode, string employeeID, bool isSuccess, bool isPayrollDifferential, string CycleType)
        {
            return InsertGenerateLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, CycleType);
        }

        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode, bool isPayrollDifferential, string CycleType)
        {
            return InsertGenerateLog(menuCode, isSuccess, userCode, "", isPayrollDifferential, CycleType);
        }

        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertLog(menuCode, "G", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType);
        }

        #endregion

        #region Print
        public static int InsertPrintLog(string menuCode, bool isSuccess)
        {
            return InsertPrintLog(menuCode, "", isSuccess);
        }
        public static int InsertPrintLog(string menuCode, string employeeID, bool isSuccess)
        {
            return InsertPrintLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, false, "N");
        }
        public static int InsertPrintLog(string menuCode, string employeeID, bool isSuccess, bool isPayrollDifferential)
        {
            return InsertPrintLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, "N");
        }

        public static int InsertPrintLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertPrintLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential, CycleType);
        }
        public static int InsertPrintLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            return InsertLog(menuCode, "P", isSuccess, userCode, employeeID, isPayrollDifferential, CycleType);
        } 
        #endregion

        private static int InsertLog(string menuCode, string action, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            int value = 1;

            string query = string.Format(@"INSERT INTO T_UserModuleLog
                        (Tml_ModuleCode
                        ,Tml_ModuleAction
                        ,Tml_IsSuccess
                        ,Usr_Login
                        ,Tml_IDNo
                        ,Tml_PayCycle
                        ,Ludatetime
                         )

                        VALUES(
                        '{0}'
                        ,'{1}'
                        ,'{2}'
                        ,'{3}'
                        ,'{4}'
                        ,(SELECT Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')
                        ,GETDATE())
                        ", menuCode, action, isSuccess, userCode, employeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();
                try
                {
                    dal.ExecuteDataSet(query, CommandType.Text);
                    if (isSuccess && (action == "A" || action == "E" || action == "D") && isPayrollDifferential && (employeeID != "" && employeeID != "ALL"))
                        InsertEmployeeLogDifferential(employeeID, userCode, menuCode, dal);
                    dal.CommitTransaction();
                }
                catch
                {
                    dal.RollBackTransaction();
                    value = 0;
                }
                dal.CloseDB();
            }
            return value;
        }

        private static int InsertLog(string menuCode, string action, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType)
        {
            int value = 1;
            string condition = @"(SELECT Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')";
            if (CycleType != "N")
                condition = string.Format(@"(SELECT Tps_PayCycle FROM T_PaySchedule 
                                                WHERE Tps_CycleIndicator = 'S' 
                                                AND Tps_CycleIndicatorSpecial = 'C' AND Tps_CycleType = '{0}')", CycleType);

            string query = string.Format(@"INSERT INTO T_UserModuleLog
                        (Tml_ModuleCode
                        ,Tml_ModuleAction
                        ,Tml_IsSuccess
                        ,Usr_Login
                        ,Tml_IDNo
                        ,Tml_PayCycle
                        ,Ludatetime
                         )

                        VALUES(
                        '{0}'
                        ,'{1}'
                        ,'{2}'
                        ,'{3}'
                        ,'{4}'
                        ,{5}
                        ,GETDATE())
                        ", menuCode, action, isSuccess, userCode, employeeID, condition);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();
                try
                {
                    dal.ExecuteDataSet(query, CommandType.Text);
                    if (isSuccess && (action != "V" || action != "P") && isPayrollDifferential && (employeeID != "" && employeeID != "ALL"))
                        InsertEmployeeLogDifferential(employeeID, userCode, CycleType, menuCode, dal);
                    dal.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransaction();
                    value = 0;
                }
                dal.CloseDB();
            }
            return value;
        }

        private static int InsertLog(string menuCode, string action, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential, string CycleType, DALHelper dalhelper)
        {
            int value = 1;
            string condition = @"(SELECT Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')";
            if (CycleType != "N")
                condition = string.Format(@"(SELECT Tps_PayCycle FROM T_PaySchedule 
                                                WHERE Tps_CycleIndicator = 'S' 
                                                AND Tps_CycleIndicatorSpecial = 'C' AND Tps_CycleType = '{0}')", CycleType);

            string query = string.Format(@"INSERT INTO T_UserModuleLog
                        (Tml_ModuleCode
                        ,Tml_ModuleAction
                        ,Tml_IsSuccess
                        ,Usr_Login
                        ,Tml_IDNo
                        ,Tml_PayCycle
                        ,Ludatetime
                         )

                        VALUES(
                        '{0}'
                        ,'{1}'
                        ,'{2}'
                        ,'{3}'
                        ,'{4}'
                        ,{5}
                        ,GETDATE())
                        ", menuCode, action, isSuccess, userCode, employeeID, condition);

            try
            {
                dalhelper.ExecuteDataSet(query, CommandType.Text);
                if (isSuccess && (action != "V" || action != "P") && isPayrollDifferential && (employeeID != "" && employeeID != "ALL"))
                    InsertEmployeeLogDifferential(employeeID, userCode, CycleType, menuCode, dalhelper);
            }
            catch (Exception ex)
            {
                value = 0;
            }
            return value;
        }

        private static int InsertEmployeeLogDifferential(string employeeID, string userCode, string menuCode, DALHelper dalhelper)
        {
            int value = 1;

            string query = string.Format(@"IF NOT EXISTS (SELECT Tpt_IDNo FROM T_EmpPayrollTrack WHERE Tpt_Type= 'N' AND Tpt_IDNo = '{0}' AND Tpt_ModuleCode = '{1}')
							            BEGIN
                                            INSERT INTO T_EmpPayrollTrack (Tpt_Type,Tpt_IDNo,Tpt_ModuleCode,Usr_Login,Ludatetime)
                                            SELECT 'N', Mem_IDNo,'{1}','{2}',GETDATE() FROM M_Employee WHERE Mem_IDNo = '{0}' AND Mem_WorkStatus = 'A'
                                        END", employeeID == "" ? "" : employeeID, menuCode, userCode);

            dalhelper.ExecuteDataSet(query, CommandType.Text);
            return value;
        }

        private static int InsertEmployeeLogDifferential(string employeeID, string userCode, string CycleType, string menuCode, DALHelper dalhelper)
        {
            int value = 1;

            string query = string.Format(@"IF NOT EXISTS (SELECT Tpt_IDNo FROM T_EmpPayrollTrack WHERE Tpt_Type= '{0}' AND Tpt_IDNo = '{1}' AND Tpt_ModuleCode = '{2}')
							            BEGIN
                                            INSERT INTO T_EmpPayrollTrack (Tpt_Type,Tpt_IDNo,Tpt_ModuleCode,Usr_Login,Ludatetime)
                                            SELECT '{0}', Mem_IDNo,'{2}','{3}', GETDATE() FROM M_Employee WHERE Mem_IDNo = '{1}' AND Mem_WorkStatus = 'A'
                                        END", CycleType, employeeID == "" ? "" : employeeID, menuCode, userCode);

            dalhelper.ExecuteDataSet(query, CommandType.Text);
            return value;
        }

        public static int InsertGroupDifferential(UniqueList<string> employeeIDList, string CycleType, string userLogin, string menuCode, string LoginDBName, DALHelper dalhelper)
        {
            int value = 0;
            string query = "";
            foreach (string employeeID in employeeIDList)
            {
                query += string.Format(@"IF NOT EXISTS (SELECT Tpt_IDNo FROM {4}..T_EmpPayrollTrack WHERE Tpt_Type= '{0}' AND Tpt_IDNo = '{1}' AND Tpt_ModuleCode = '{2}')
							            BEGIN
                                            INSERT INTO {4}..T_EmpPayrollTrack (Tpt_Type, Tpt_IDNo, Tpt_ModuleCode, Usr_Login, Ludatetime) 
                                            SELECT '{0}', Mem_IDNo, '{2}', '{3}', GETDATE() FROM M_Employee WHERE Mem_IDNo = '{1}' AND Mem_WorkStatus = 'A'
                                        END ", CycleType, employeeID, menuCode, userLogin, LoginDBName);
            }
            if (query != "")
            {
                dalhelper.ExecuteDataSet(query, CommandType.Text);
            }
            return value;
        }

        public static int InsertGroupDifferential(UniqueList<string> employeeIDList, string CycleType, string userLogin, string menuCode)
        {
            int value = 0;
            string query = "";
            foreach (string employeeID in employeeIDList)
            {
                query += string.Format(@"IF NOT EXISTS (SELECT Tpt_IDNo FROM T_EmpPayrollTrack WHERE Tpt_Type= '{0}' AND Tpt_IDNo = '{1}' AND Tpt_ModuleCode = '{2}')
							            BEGIN
                                            INSERT INTO T_EmpPayrollTrack (Tpt_Type, Tpt_IDNo, Tpt_ModuleCode, Usr_Login, Ludatetime) 
                                            SELECT '{0}', Mem_IDNo, '{2}', '{3}', GETDATE() FROM M_Employee WHERE Mem_IDNo = '{1}' AND Mem_WorkStatus = 'A'
                                        END ", CycleType, employeeID, menuCode,userLogin);
            }
            if (query != "")
            {
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    try
                    {
                        dal.ExecuteDataSet(query, CommandType.Text);
                        dal.CommitTransaction();
                    }
                    catch
                    {
                        dal.RollBackTransaction();
                        value = 0;
                    }
                    dal.CloseDB();
                }
            }
            return value;
        }

        public static int InsertGroupDifferential(UniqueList<string> employeeIDList, string CycleType, string userLogin, string menuCode, DALHelper dalhelper)
        {
            int value = 0;
            string query = "";
            foreach (string employeeID in employeeIDList)
            {
                query += string.Format(@"IF NOT EXISTS (SELECT Tpt_IDNo FROM T_EmpPayrollTrack WHERE Tpt_Type= '{0}' AND Tpt_IDNo = '{1}' AND Tpt_ModuleCode = '{2}')
							            BEGIN
                                            INSERT INTO T_EmpPayrollTrack (Tpt_Type, Tpt_IDNo, Tpt_ModuleCode, Usr_Login, Ludatetime) 
                                            SELECT '{0}', Mem_IDNo, '{2}', '{3}', GETDATE() FROM M_Employee WHERE Mem_IDNo = '{1}' AND Mem_WorkStatus = 'A'
                                        END ", CycleType, employeeID, menuCode, userLogin);
            }
            if (query != "")
            {
                dalhelper.ExecuteDataSet(query, CommandType.Text);
            }
            return value;
        }

        public class UniqueList<T> : List<T>
        {
            public new void Add(T obj)
            {
                if (!Contains(obj))
                {
                    base.Add(obj);
                }
            }
        }
    }
}
