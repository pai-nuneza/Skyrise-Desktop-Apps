using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;

namespace CommonLibrary
{
    public class GenericPromptBL
    {
        private DataTable GetTable(string Query, int messageCode, string CentralProfile)
        {
            try
            {
                DataTable dtResult = new DataTable();
                using (DALHelper dal = new DALHelper(CentralProfile, false))
                {
                    dal.OpenDB();

                    dtResult = dal.ExecuteDataSet(Query).Tables[0];

                    dal.CloseDB();
                }

                if (dtResult != null && dtResult.Rows.Count > 0)
                    return dtResult;
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                try
                {
                    CommonProcedures.showMessageError(System.Runtime.InteropServices.Marshal.GetExceptionForHR(messageCode).Message);
                }
                catch (Exception)
                {
                    CommonProcedures.showMessageError(string.Format("Cannot proceed with transaction since Message Code {0} does not exist. Please make sure it exists in M_SystemMessage table.", messageCode));
                }
                return null;
            }
        }

        private void Execute(string Query)
        {
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dal.ExecuteNonQuery(Query);

                dal.CloseDB();
            }
        }

        public DataTable GetMessageCodeDetails(int MessageCode)
        {
            #region query
            string query = string.Format(@"
                    SELECT
                        Msm_MessageCode
                        , Msm_MessageType
                        , Msm_MessageName
                        , Msm_Suggestion
                        , Msm_UpdatedBy
                        , Msm_UpdatedDate
                    FROM M_SystemMessage
                    WHERE Msm_MessageCode = '{0}'
                        AND Msm_CompanyCode = '{1}'"
                , MessageCode, LoginInfo.getUser().CompanyCode);
            #endregion

            return GetTable(query, MessageCode, LoginInfo.getUser().CentralProfileName);
        }

        public void AddSystemLog(int MessageCode, string MenuCode)
        {
            #region Query
            string query = string.Format(@"
                DECLARE @PAYPERIOD VARCHAR(7)

                SET @PAYPERIOD = (SELECT Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')

                INSERT INTO T_SystemMessageLog
                (
                    Tml_MessageCode
                    , Tml_ModuleCode
                    , Tml_PayCycle
                    , Usr_Login
                    , Ludatetime
                )
                VALUES
                (
                    {0}
                    , '{1}'
                    , @PAYPERIOD
                    , '{2}'
                    , GETDATE()
                )"
                , MessageCode
                , MenuCode
                , LoginInfo.getUser().UserCode);
            #endregion

            Execute(query);
        }

        public string GetSystemMessage(int MessageCode)
        {
            #region query
            string query = string.Format(@"
                    SELECT
                        Msm_MessageName
                    FROM M_SystemMessage
                    WHERE Msm_MessageCode = '{0}'"
                , MessageCode);
            #endregion

            DataTable dt = GetTable(query, MessageCode, LoginInfo.getUser().CentralProfileName);
            return ((dt != null) ?  dt.Rows[0][0].ToString() :  "");
        }
    }
}
