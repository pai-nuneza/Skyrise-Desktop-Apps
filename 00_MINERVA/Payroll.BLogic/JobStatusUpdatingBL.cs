using System;
using System.Collections.Generic;
using System.Text;

using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class JobStatusUpdatingBL : BaseBL
    {
        #region Override Functions

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
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Mem_IDNo
                                    , Mem_NickName
                                    , Mem_LastName
                                    , Mem_FirstName
                                    , LEFT(Mem_MiddleName,1) as Mem_MiddleName
	                                , Mem_WorkStatus
	                                , Mem_WorkStatus as OrigJobStatus
                                From M_Employee
                                --Where Mem_WorkStatus like 'A%'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public int Update(DataRow row, string OldJobStatus)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE M_Employee
                                   SET Mem_WorkStatus = @Mem_WorkStatus
                                      ,Usr_Login = @Usr_Login
                                      ,Ludatetime = GetDate()
                                 WHERE Mem_IDNo = @Mem_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Mem_WorkStatus", row["Mem_WorkStatus"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    this.CreateAuditTrailRec(this.AssignAuditRowRec("JobStatus", this.GenerateSeqNo("JobStatus", row["Mem_IDNo"].ToString().Trim(), dal), OldJobStatus, row["Mem_WorkStatus"].ToString().Trim(), row["Mem_IDNo"].ToString().Trim(), dal), dal);
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

        //For Audit Trail
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

        private string GenerateSeqNo(string colid, string empid, DALHelper DalUp)
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

        private DataRow AssignAuditRowRec(string colid, string seqno, string prevVal, string curVal, string txtIDNumber, DALHelper DalUp)
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

        //For Separation Information

        public string GetHireDate(string idnumber)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", idnumber);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("Select Convert(char(10),Mem_IntakeDate,101) as Mem_IntakeDate From M_Employee Where Mem_IDNo = @Mem_IDNo", CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
            else
                return string.Empty;
        }

        public DataSet GetSepInfo(string idnumber)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Mem_SeparationNoticeDate
	                                   ,Mem_SeparationCode
	                                   ,Mem_SeparationDate
	                                   ,Mem_ClearedDate
                                From M_Employee
                                Where Mem_IDNo = @Mem_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", idnumber);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public int UpdateSeparationInfo(DataRow row)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE M_Employee
                                   SET Mem_SeparationNoticeDate = @Mem_SeparationNoticeDate
                                      ,Mem_SeparationCode = @Mem_SeparationCode
                                      ,Mem_SeparationDate = @Mem_SeparationDate
                                      ,Mem_ClearedDate = @Mem_ClearedDate
                                      ,Usr_Login = @Usr_Login
                                      ,Ludatetime = GetDate()
                                 WHERE Mem_IDNo = @Mem_IDNo";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", row["Mem_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Mem_SeparationNoticeDate", row["Mem_SeparationNoticeDate"]);
            paramInfo[2] = new ParameterInfo("@Mem_SeparationCode", row["Mem_SeparationCode"]);
            paramInfo[3] = new ParameterInfo("@Mem_SeparationDate", row["Mem_SeparationDate"]);
            paramInfo[4] = new ParameterInfo("@Mem_ClearedDate", row["Mem_ClearedDate"]);
            paramInfo[5] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

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
    }
}
