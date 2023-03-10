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
    public class NotifyEmailBL : BaseBL
    {
        #region Override Functions

        public override int Add(DataRow row)
        {
            int retVal = 0;

            #region query

            string qString = @"
                                if(select count(Sse_MenuCode) from T_ServiceEmailRecipient where Sse_MenuCode=@Sse_MenuCode and Sse_UserCode=@Sse_UserCode)>0
                                begin
                                    update T_ServiceEmailRecipient 
                                        set Sse_RegisteredDate=@Sse_RegisteredDate,
                                            Sse_RecipientType=@Sse_RecipientType,
                                            Sse_Status=@Sse_Status,
                                            Usr_Login=@Usr_Login,
                                            ludatetime=Getdate()
                                        where Sse_MenuCode=@Sse_MenuCode and Sse_UserCode=@Sse_UserCode
                                end
                                else
                                begin
                                    INSERT INTO T_ServiceEmailRecipient
                                           (Sse_MenuCode
                                           ,Sse_UserCode
                                           ,Sse_RegisteredDate
                                           ,Sse_RecipientType
                                           ,Sse_Status
                                           ,Usr_Login
                                           ,ludatetime)
                                     VALUES
                                           (@Sse_MenuCode
                                           ,@Sse_UserCode
                                           ,@Sse_RegisteredDate
                                           ,@Sse_RecipientType
                                           ,@Sse_Status
                                           ,@Usr_Login
                                           ,GetDate())
                                end
                                ";

            #endregion

            #region parameters

            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Sse_MenuCode", row["Sse_MenuCode"]);
            paramInfo[1] = new ParameterInfo("@Sse_UserCode", row["Sse_UserCode"]);
            paramInfo[2] = new ParameterInfo("@Sse_RegisteredDate", row["Sse_RegisteredDate"]);
            paramInfo[3] = new ParameterInfo("@Sse_RecipientType", row["Sse_RecipientType"]);
            paramInfo[4] = new ParameterInfo("@Sse_Status", row["Sse_Status"]);
            paramInfo[5] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #endregion

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
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public override int Update(DataRow row)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE T_ServiceEmailRecipient
                                   SET Sse_Status = @Sse_Status
                                      ,Usr_Login = @Usr_Login
                                      ,ludatetime = GetDate()
                                      ,Sse_RegisteredDate = @Sse_RegisteredDate
                                      ,Sse_RecipientType = @Sse_RecipientType
                               WHERE Sse_MenuCode = @Sse_MenuCode
                                 And Sse_UserCode = @Sse_UserCode";

            #endregion

            #region parameters

            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Sse_Status", row["Sse_Status"]);
            paramInfo[1] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[2] = new ParameterInfo("@Sse_RegisteredDate", row["Sse_RegisteredDate"].ToString());
            paramInfo[3] = new ParameterInfo("@Sse_RecipientType", row["Sse_RecipientType"]);
            paramInfo[4] = new ParameterInfo("@Sse_MenuCode", row["Sse_MenuCode"]);
            paramInfo[5] = new ParameterInfo("@Sse_UserCode", row["Sse_UserCode"]);

            #endregion

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
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
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

        #region Defined Functions

        public DataSet FetchRecords(string code)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Sse_UserCode as EmployeeID
                                     ,Muh_LastName + ', ' + Muh_FirstName + ' ' + LEFT(Muh_MiddleName,1) + 
                                                case when Muh_MiddleName = '' then ''
                                                     when Muh_MiddleName is null then ''
                                                else 
                                                     '.' 
                                                     end As EmploName
                                     ,Mem_NickName as IDCode
                                     ,Convert(char(10), Sse_RegisteredDate, 101) As RegDate
                                     ,Sse_RecipientType as Type
                                     ,Sse_Status as Status									 
                               FROM T_ServiceEmailRecipient
                               Inner Join M_UserHdr on Sse_UserCode = Muh_UserCode
                               LEFT JOIN M_Employee on Mem_IDNo = Sse_UserCode
                               Where Sse_MenuCode = @MenuCode";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@MenuCode", code);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet CheckIfExists(string menuCode, string userCode)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Sse_MenuCode 
                               From T_ServiceEmailRecipient
                               Where Sse_MenuCode = @MenuCode
                                 And Sse_UserCode = @UserCode and Sse_Status = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@MenuCode", menuCode);
            paramInfo[1] = new ParameterInfo("@UserCode", userCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        //added by Kevin 20082710 for delete query
        public int DeleteRecord(DataRow row)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE T_ServiceEmailRecipient
                                   SET Sse_Status = 'C'
                                      ,Usr_Login = @Usr_Login
                                      ,ludatetime = GetDate()
                               WHERE Sse_MenuCode = @Sse_MenuCode
                                 And Sse_UserCode = @Sse_UserCode";

            #endregion

            #region parameters

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Sse_MenuCode", row["Sse_MenuCode"]);
            paramInfo[1] = new ParameterInfo("@Sse_UserCode", row["Sse_UserCode"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #endregion

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
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }
        #endregion

    }
}
