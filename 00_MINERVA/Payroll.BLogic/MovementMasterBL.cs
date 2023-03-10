using System;
using System.Collections.Generic;
using System.Configuration;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class MovementMasterBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            CommonProcedures.ShowMessage(10113, ""); return -1;
        }

        public override int Update(System.Data.DataRow row)
        {
            CommonProcedures.ShowMessage(10113, ""); return -1;
        }

        public override int Delete(string code, string userLogin)
        {
            CommonProcedures.ShowMessage(10113, ""); return -1;
        }

        public override System.Data.DataSet FetchAll()
        {
            CommonProcedures.ShowMessage(10113, ""); return null;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            CommonProcedures.ShowMessage(10113, ""); return null;
        }

        #region Profile / Job Status Movement
        public DataTable GetEmployeeJobStatusMovement(string IDNumber, string CentralProfile, string CompanyCode)
        {
            #region query
            string query = string.Format(@"SELECT Tep_StartDate as 'Start Date'
                                                    ,Tep_EndDate as 'End Date'
                                                    ,ProfileDBSource.Mpf_ProfileName as 'Profile Source'
                                                    ,ProfileDBDestination.Mpf_ProfileName as 'Profile Destination'
                                                    ,WORKSTAT.Mcd_Name as 'Job Status'
		                                            ,Tep_SourcePayCycle as 'Pay Period Source'
                                                    ,Tep_TargetPayCycle as 'Pay Period Destination'
                                                    ,Reason.Mcd_Name as 'Reason'
		                                            ,Tep_IsLatestProfile 'Latest'
                                            FROM T_EmpProfile
                                            LEFT JOIN M_Profile ProfileDBSource
                                                ON ProfileDBSource.Mpf_DatabaseNo = Tep_SourceProfile
                                            LEFT JOIN M_Profile ProfileDBDestination
                                                ON ProfileDBDestination.Mpf_DatabaseNo = Tep_TargetProfile
                                            LEFT JOIN {1}..M_CodeDtl WORKSTAT
                                                ON WORKSTAT.Mcd_Code = Tep_WorkStatus
                                                AND WORKSTAT.Mcd_CodeType = 'WORKSTAT'
                                                AND WORKSTAT.Mcd_CompanyCode = '{2}'
                                            LEFT JOIN {1}..M_CodeDtl Reason
                                                ON Reason.Mcd_Code = Tep_ReasonCode
                                                AND Reason.Mcd_CodeType IN ('PROFILEMVE', 'SEPARATION')
                                                AND Reason.Mcd_CompanyCode = '{2}'
                                            WHERE Tep_IDNo = '{0}'
                                                ORDER BY Tep_StartDate DESC"
                                        , IDNumber
                                        , CentralProfile
                                        , CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeJobStatusMovement(string SystemID, string user_Logged, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM {0}..T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tep_IDNo as 'Employee ID'
	                                              ,Mem_LastName as 'Last Name'
	                                              ,Mem_FirstName as 'First Name'
                                                  ,Tep_StartDate as 'Effective Date'
                                                  ,ProfileDBSource.Mpf_DatabaseNo as 'Profile No. Source'
                                                  ,ProfileDBSource.Mpf_ProfileName as 'Profile Source'
                                                  ,ProfileDBDestination.Mpf_DatabaseNo as 'Profile No. Destination'
                                                  ,ProfileDBDestination.Mpf_ProfileName as 'Profile Destination'
                                                  ,Tep_WorkStatus as 'Job Status'
                                                  ,JobStat.Mcd_Name as 'Description'
                                                  , Mem_SeparationNoticeDate as 'Notice Date'
		                                          ,Tep_SourcePayCycle as 'Pay Period Source'
                                                  ,Tep_TargetPayCycle as 'Pay Period Destination'
                                                  ,Tep_ReasonCode as 'Reason Code'
                                                  ,Reason.Mcd_Name as 'Reason'
		                                          ,Tep_IsLatestProfile 'Latest'
                                            FROM T_EmpProfile
                                            INNER JOIN M_Employee
                                            ON Tep_IDNo = Mem_IDNo
                                                AND  Mem_WorkStatus NOT IN ('IN','IM')
@ACCESSRIGHTS
                                            LEFT JOIN M_Profile ProfileDBSource
                                            ON ProfileDBSource.Mpf_DatabaseNo = Tep_SourceProfile
                                            LEFT JOIN M_Profile ProfileDBDestination
                                            ON ProfileDBDestination.Mpf_DatabaseNo = Tep_TargetProfile
                                            LEFT JOIN {1}..M_CodeDtl WORKSTAT
                                                ON WORKSTAT.Mcd_Code = Tep_WorkStatus
                                                AND WORKSTAT.Mcd_CodeType = 'WORKSTAT'
                                                AND WORKSTAT.Mcd_CompanyCode = '{2}'
                                            LEFT JOIN {1}..M_CodeDtl Reason
                                                ON Reason.Mcd_Code = Tep_ReasonCode
                                                AND Reason.Mcd_CodeType IN ('PROFILEMVE', 'SEPARATION')
                                                AND Reason.Mcd_CompanyCode = '{2}'
                                            WHERE Tep_IDNo + Convert(Char(10),Tep_StartDate,112) in ( SELECT Tep_IDNo + Convert(Char(10), Tep_StartDate, 112)
                                                                                                             FROM (SELECT Tep_IDNo , Max(Tep_StartDate) as Tep_StartDate
                                                                                                                    FROM {1}..T_EmpProfile
                                                                                                                    WHERE @EndNewCycle >= Tep_StartDate
                                                                                                                        AND ISNULL(Tep_EndDate, @StartNewCycle) >= @StartNewCycle
                                                                                                                    GROUP BY Tep_IDNo ) Temp) 
                                            ORDER BY Mem_LastName, Mem_FirstName"
                                        , ProfileCode
                                        , CentralProfile
                                        , CompanyCode);
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(ProfileCode, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, "", false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeeJobStatusRow(string IDNumber, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT
	                                        Mem_WorkStatus
                                        FROM M_Employee
                                        WHERE Mem_IDNo = '{0}'", IDNumber);
            #endregion
            DataTable dtResult;
            DataRow drJobStatus = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drJobStatus = dtResult.Rows[0];

            return drJobStatus;
        }

        public bool IsCurrentDB(string IDNumber, string DatabaseNo, string CentralProfile)
        {
            string query = string.Format(@"Select Tep_TargetProfile from T_EmpProfile 
                                            where Tep_IDNo = '{0}'
											AND Tep_IsLatestProfile = 1", IDNumber);
            DataSet ds;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString() == DatabaseNo ? true : false;
            else return true;
        }

        public DataTable GetEmployeeJobStatusMovement(string IDNumber, DateTime StartDate, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Tep_StartDate as 'Start Date'
                                                    ,Tep_EndDate as 'End Date'
                                                    ,ProfileDBSource.Mpf_DatabaseNo as 'Profile Source No'
                                                    ,ProfileDBSource.Mpf_ProfileName as 'Profile Source'
                                                    ,ProfileDBDestination.Mpf_DatabaseNo as 'Profile Destination No'
                                                    ,ProfileDBDestination.Mpf_ProfileName as 'Profile Destination'
                                                    ,Tep_WorkStatus
                                                    ,WorkStat.Mcd_Name as 'Job Status'
		                                            ,Tep_SourcePayCycle as 'Pay Period Source'
                                                    ,Tep_TargetPayCycle as 'Pay Period Destination'
                                                    ,Tep_ReasonCode
                                                    ,Reason.Mcd_Name as 'Reason'
		                                            ,Tep_IsLatestProfile 'Latest'
                                            FROM T_EmpProfile
                                            LEFT JOIN M_Profile ProfileDBSource
                                            ON ProfileDBSource.Mpf_DatabaseNo = Tep_SourceProfile
                                            LEFT JOIN M_Profile ProfileDBDestination
                                            ON ProfileDBDestination.Mpf_DatabaseNo = Tep_TargetProfile
                                            LEFT JOIN {1}..M_CodeDtl WorkStat
                                            ON WorkStat.Mcd_Code = Tep_WorkStatus
                                            AND WorkStat.Mcd_CodeType = 'WORKSTAT'
                                            LEFT JOIN {1}..M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tep_ReasonCode
                                            AND Reason.Mcd_CodeType IN ('PROFILEMVE', 'SEPARATION')
                                            WHERE Tep_IDNo = '{0}'
                                                AND Tep_StartDate = '{2}'
                                            ORDER BY Tep_StartDate DESC", IDNumber, CentralProfile, StartDate);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertJobStatus(string EmployeeId
                                    , object EffectivityDate
                                    , object EndDate
                                    , string Profile
                                    , string JobStatus
                                    , string MovementReason
                                    , string Usr_Login
                                    , string LatestProfileDB
                                    , string NewProfileDB
                                    , string user_DBNumber
                                    , bool isInactive
                                    , DALHelper dal)
        {
            string query = string.Empty;
            bool isCurrentDB = false;

            #region Get Database Name of the Target DB
            string strResult = "";
            query = string.Format(@"SELECT Mpf_DatabaseName
                                           FROM M_Profile
                                           WHERE Mpf_DatabaseNo = '{0}'", Profile);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                strResult = dtResult.Rows[0][0].ToString();
            #endregion

            #region Get Current Pay Period from Target DB
            object destinationpayperiod = null;
            object sourcePayPeriod = null;
            if (LatestProfileDB != "" && NewProfileDB != "" && LatestProfileDB != NewProfileDB)
            {
                query = string.Format(@"SELECT Tps_PayCycle 
                                            FROM {0}..T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'", strResult);
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                if (dtResult.Rows.Count > 0)
                    destinationpayperiod = dtResult.Rows[0][0];

                query = string.Format(@"SELECT Tps_PayCycle 
                                            FROM {0}..T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'", LatestProfileDB);
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                if (dtResult.Rows.Count > 0)
                    sourcePayPeriod = dtResult.Rows[0][0];
            }
            #endregion

            #region Check if Current DB
            query = string.Format(@"Select Tep_TargetProfile from T_EmpProfile 
                                            where Tep_IDNo = '{0}'
											AND Tep_IsLatestProfile = 1", EmployeeId);
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                isCurrentDB = (dtResult.Rows[0][0].ToString() == user_DBNumber ? true : false);
            #endregion

            if (isInactive) EndDate = EffectivityDate;

            ParameterInfo[] paramInfo = new ParameterInfo[11];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileSource", user_DBNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileDestination", Profile);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@JobStatus", JobStatus);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@SourcePayPeriod", (sourcePayPeriod == null) ? DBNull.Value : sourcePayPeriod); ;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@DestinationPayPeriod", (destinationpayperiod == null) ? DBNull.Value : destinationpayperiod); ;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@LatestProfile", isCurrentDB);

            #region query
            query = string.Format(@"INSERT INTO T_EmpProfile
                                           (Tep_IDNo
                                           ,Tep_StartDate
                                           ,Tep_EndDate
                                           ,Tep_SourceProfile
                                           ,Tep_TargetProfile
                                           ,Tep_WorkStatus
                                           ,Tep_ReasonCode
                                           ,Usr_Login
                                           ,Ludatetime
                                           ,Tep_SourcePayCycle
                                           ,Tep_TargetPayCycle
                                           ,Tep_IsLatestProfile)
                                     VALUES
                                           (@EmployeeId
                                           ,@EffectivityDate
                                           ,@EndDate
                                           ,@ProfileSource
                                           ,@ProfileDestination
                                           ,@JobStatus
                                           ,@MovementReason
                                           ,@Usr_Login
                                           ,GETDATE()
                                           ,@SourcePayPeriod
                                           ,@DestinationPayPeriod
                                           ,@LatestProfile)");
            #endregion

            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            //if (user_DBNumber != Profile)
            //    UpdateSourceUserProfile(EmployeeId, Usr_Login, user_DBNumber, dal);
        }

        public void UpdateSourceUserProfile(string EmployeeId
                                        , string Usr_Login
                                        , string user_DBNumber
                                        , DALHelper dal)
        {

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileSource", user_DBNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            #region query
            string query = @"UPDATE M_UserProfile 
                                    SET Mup_RecordStatus = 'C'
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = GETDATE()
                                    WHERE  Mup_ProfileCode = @ProfileSource
                                    AND Mup_UserCode = @EmployeeId";
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateJobStatus(string EmployeeId
                                    , object EffectivityDate
                                    , object EndDate
                                    , string Profile
                                    , string JobStatus
                                    , string MovementReason
                                    , string Usr_Login
                                    , string user_DBNumber
                                    , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Profile", Profile);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@JobStatus", JobStatus);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpProfile
                                               SET Tep_EndDate = @EndDate
                                                  ,Tep_TargetProfile = @Profile
                                                  ,Tep_WorkStatus = @JobStatus
                                                  ,Tep_ReasonCode = @MovementReason
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Tep_IDNo = @EmployeeId
                                             AND Tep_StartDate = @EffectivityDate
                                            ");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            if (user_DBNumber != Profile)
                UpdateSourceUserProfile(EmployeeId, Usr_Login, user_DBNumber, dal);
        }

        public void UpdateLatestJobStatus(string EmployeeId
                                            , object EffectivityDate
                                            , string Usr_Login
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@LatestProfile", Convert.ToInt32(!IsCurrentDB(EmployeeId, LoginInfo.getUser().DBNumber, LoginInfo.getUser().CentralProfileName)));

            #region query
            string query = string.Format(@"UPDATE T_EmpProfile
                                               SET Tep_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                  --,Usr_Login = @Usr_Login
                                                  --,Ludatetime = GETDATE()
                                             WHERE Tep_IDNo = @EmployeeId
                                             AND Tep_StartDate = (SELECT MAX(JobStatus.Tep_StartDate)
							                                            FROM T_EmpProfile JobStatus
							                                            WHERE Tep_IDNo = @EmployeeId)");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestProfile(string EmployeeId
                                            , string LatestProfile
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@LatestProfile", LatestProfile);

            #region query
            string query = string.Format(@"UPDATE T_EmpProfile
                                               SET Tep_IsLatestProfile = 
                                                (
                                                CASE WHEN Tep_TargetProfile = @LatestProfile
                                                    THEN 0
                                                    ELSE Tep_IsLatestProfile
                                                END
                                                )
                                             WHERE Tep_IDNo = @EmployeeId
                                             AND Tep_IsLatestProfile = 1");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        //public void UpdateUserMasterJobStatus(string EmployeeId
        //                                        , string JobStatus
        //                                        , string Usr_Login
        //                                        , string Database
        //                                        , DALHelper dal)
        //{
        //    ParameterInfo[] paramInfo = new ParameterInfo[3];
        //    int paramInfoCnt = 0;
        //    paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
        //    paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
        //    paramInfo[paramInfoCnt++] = new ParameterInfo("@JobStatus", JobStatus);
        //    #region query
        //    string query = string.Format(@"UPDATE {0}..M_UserHdr
        //                                    SET Muh_RecordStatus = @JobStatus
        //                                        , Usr_Login = @Usr_Login
        //                                        , Ludatetime = getdate()
        //                                    WHERE Muh_UserCode = @EmployeeId

        //                                    UPDATE M_UserHdr
        //                                    SET Muh_RecordStatus = @JobStatus
        //                                        , Usr_Login = @Usr_Login
        //                                        , Ludatetime = getdate()
        //                                    WHERE Muh_UserCode = @EmployeeId", Database);
        //    #endregion
        //    dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        //}

        public void UpdateUserMasterJobStatus(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string DatabaseProfile
                                                , string DatabaseProfileCode
                                                , string Usr_Login
                                                , bool isInactive
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileCode", DatabaseProfileCode);

            #region query
            string query = string.Empty;
            if (isInactive)
            {
                query = string.Format(@"UPDATE {0}..M_UserHdr
                                            SET Muh_RecordStatus = CASE WHEN (ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                    Order By Tep_StartDate DESC), '')) = 'IN'
                                                                THEN 'C'
                                                                ELSE 'A'
                                                                END
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Muh_UserCode = @EmployeeId

                                            UPDATE M_User
                                            SET Mur_RecordStatus = CASE WHEN (ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                    Order By Tep_StartDate DESC), '')) = 'IN'
                                                                THEN 'C'
                                                                ELSE 'A'
                                                                END
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mur_UserCode = @EmployeeId
                                            ", DatabaseProfile);
            }
            else
            {
                query = string.Format(@"UPDATE {0}..M_UserHdr
                                            SET Muh_RecordStatus = CASE WHEN (ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                        AND ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                        AND @PayPeriodEnd >= Tep_StartDate
                                                                    Order By Tep_StartDate DESC), '')) = 'IN'
                                                                THEN 'C'
                                                                ELSE 'A'
                                                                END
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Muh_UserCode = @EmployeeId

                                            UPDATE M_User
                                            SET Mur_RecordStatus = CASE WHEN (ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                        AND ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                        AND @PayPeriodEnd >= Tep_StartDate
                                                                    Order By Tep_StartDate DESC), '')) = 'IN'
                                                                THEN 'C'
                                                                ELSE 'A'
                                                                END
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mur_UserCode = @EmployeeId
                                            ", DatabaseProfile);
            }
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            #region M_UserProfile Insert/Update
            query = string.Format(@"SELECT Mup_RecordStatus FROM M_UserProfile 
                    WHERE Mup_ProfileCode = '{0}'
                    AND Mup_UserCode = '{1}'", DatabaseProfileCode, EmployeeId);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                if (dtResult.Rows[0][0].ToString().ToUpper() == "C")
                {
                    query = @"Update M_UserProfile 
                                            Set Mup_RecordStatus = 'A'
                                            , Usr_Login = @Usr_Login
                                            , Ludatetime = GETDATE()
                                            where Mup_UserCode = @EmployeeId
                                            AND Mup_ProfileCode = @ProfileCode";
                }
            }
            else
            {
                query = string.Format(@"IF EXISTS(SELECT COUNT(*) [Cnt] 
		                                from {0}..M_UserHdr where Muh_UserCode= @EmployeeId
		                                HAVING COUNT(*) > 0)
                                            BEGIN 
		
		                                            INSERT INTO M_UserProfile
                                                       (Mup_UserCode
                                                       ,Mup_ProfileCode
                                                       ,Mup_RecordStatus
                                                       ,Usr_Login
                                                       ,Ludatetime)
		                                            VALUES(@EmployeeId
                                                          , @ProfileCode
                                                          , 'A'
                                                          , @Usr_Login
                                                          , GETDATE())
                                            END
                        ", DatabaseProfile);
            }
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
            #endregion 

        }

        public void DeletePermJobStatus(string EmployeeId, DateTime EffectivityDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"DELETE FROM T_EmpProfile
                                           WHERE Tep_IDNo = '{0}'
                                            AND Tep_StartDate = '{1}'

                                        Declare @Enddate as datetime
                                            Set @Enddate = (select top(1)Tep_EndDate from T_EmpProfile
                                            Where Tep_IDNo = '{0}'
                                            order by Tep_StartDate desc)

                                        Update TOP(1) 
                                            T_EmpProfile
                                            set Tep_EndDate = null
                                            Where Tep_IDNo = '{0}'
                                             and Tep_EndDate = @Enddate   

                                            ", EmployeeId, EffectivityDate);
            #endregion
            dal.ExecuteNonQuery(query);
        }

        public void UpdateEmployeeMasterJobStatus(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            string CentralProfile = LoginInfo.getUser().CentralProfileName;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Empty;
            query = string.Format(@"UPDATE M_Employee
                                            SET Mem_WorkStatus = ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From {0}..T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tep_StartDate 
	                                                                    And ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tep_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId", CentralProfile);

            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateEmployeeMasterJobStatus(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string DatabaseProfile
                                                , string Usr_Login
                                                , bool isInactive
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            UpdateEmployeeMasterJobStatusAuditTrail(EmployeeId, PayPeriodStart, PayPeriodEnd, DatabaseProfile, Usr_Login, isInactive, dal);
            #region query
            string query = string.Empty;
            if (isInactive)
            {
                query = string.Format(@"UPDATE {0}..M_Employee
                                            SET Mem_WorkStatus = ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                    Order By Tep_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId

                                            UPDATE M_Employee
                                            SET Mem_WorkStatus = ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                    Order By Tep_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId
                                            ", DatabaseProfile);
            }
            else
            {
                query = string.Format(@"UPDATE {0}..M_Employee
                                            SET Mem_WorkStatus = ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tep_StartDate 
	                                                                    And ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tep_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId

                                            UPDATE M_Employee
                                            SET Mem_WorkStatus = ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tep_StartDate 
	                                                                    And ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tep_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId", DatabaseProfile);
            }
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateEmployeeMasterJobStatusAuditTrail(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string DatabaseProfile
                                                , string Usr_Login
                                                , bool isInactive
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Empty;
            if (isInactive)
            {
                query = string.Format(@"SELECT ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                    Order By Tep_StartDate DESC), '')", DatabaseProfile);
            }
            else
            {
                query = string.Format(@"SELECT ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tep_StartDate 
	                                                                    And ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tep_StartDate DESC), '')", DatabaseProfile);
            }
                
            #endregion

            DataSet ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string jobStatMov = ds.Tables[0].Rows[0][0].ToString();
                query = string.Format(@"SELECT * FROM [{0}]..M_Employee WHERE Mem_IDNo = '{1}'", DatabaseProfile, EmployeeId);
                DataSet dsEmp = dal.ExecuteDataSet(query);
                if (dsEmp != null && dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    string payStatEmp = dsEmp.Tables[0].Rows[0]["Mem_IsComputedPayroll"].ToString();
                    string jobStatEmp = dsEmp.Tables[0].Rows[0]["Mem_WorkStatus"].ToString();
                    EmployeeMasterBL EmpBL = new EmployeeMasterBL();
                    if (jobStatMov.Trim().ToUpper() == "IN"
                        && payStatEmp.Trim().ToUpper() == "TRUE")
                    {
                        EmpBL.CreateAuditTrailRec(EmpBL.AssignAuditRowRec("PayrollStatus", EmpBL.GenerateSeqNo("PayrollStatus", EmployeeId, DatabaseProfile, dal), "TRUE", "FALSE", EmployeeId, DatabaseProfile, dal), DatabaseProfile, dal);
                    }
                    if (jobStatMov.Trim().ToUpper() == "IN")
                    {
                        (new NewPayrollCalculationBL()).DeletePayrollRecords((new CommonBL()).GetCurrentPayPeriod(), EmployeeId, DatabaseProfile, dal);
                    }
                    if (jobStatEmp.Trim().ToUpper() != jobStatMov.Trim().ToUpper())
                    {
                        EmpBL.CreateAuditTrailRec(EmpBL.AssignAuditRowRec("JobStatus", EmpBL.GenerateSeqNo("JobStatus", EmployeeId, DatabaseProfile, dal), jobStatEmp, jobStatMov, EmployeeId, DatabaseProfile, dal), DatabaseProfile, dal);
                    }
                }
            }
        }

        public void UpdateEmployeeMasterJobStatus(string EmployeeId
                                                , string JobStatus
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@JobStatus", JobStatus);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_WorkStatus = @JobStatus
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }
        public void AutoUpdateDataInSourceProfile(string EmployeeId
                                                    , string Usr_Login
                                                    , string separationCode
                                                    , DateTime separationDateEffectivity
                                                    , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[1] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[2] = new ParameterInfo(@"@SeparationCode", separationCode);
            paramInfo[3] = new ParameterInfo(@"@SeparationDateEffectivity", separationDateEffectivity);

            string query = @"UPDATE M_Employee
                            SET Mem_WorkStatus = 'IM'
                            , Mem_IsComputedPayroll = 0
                            , Mem_SeparationCode = @SeparationCode
	                        , Mem_SeparationDate = @SeparationDateEffectivity
                            , Usr_Login = @Usr_Login
                            , Ludatetime = getdate()
                        WHERE Mem_IDNo = @EmployeeId

                        UPDATE M_UserHdr
                            SET Muh_RecordStatus = 'C'
                                , Usr_Login = @Usr_Login
                                ,ludatetime = GETDATE()
                                WHERE Muh_UserCode = @EmployeeId

                        UPDATE M_UserDtl
                            SET Mud_RecordStatus = 'C'
                            , Usr_Login = @Usr_Login
                            , ludatetime = GETDATE()
                        where Mud_UserCode = @EmployeeId";

            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }        
        public void UpdateEmployeeMasterJobStatus(string EmployeeId
                                                , string JobStatus
                                                , string DatabaseProfile
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            string CentralProfile = LoginInfo.getUser().CentralProfileName;

            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@JobStatus", JobStatus);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE {0}..M_Employee
                                            SET Mem_WorkStatus = @JobStatus
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId", DatabaseProfile);

            string queryProfile = string.Format(@"UPDATE {0}..M_Employee
                                            SET Mem_WorkStatus = @JobStatus
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId", CentralProfile);
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            //Update Profile Employee Master
            dal.ExecuteNonQuery(queryProfile, CommandType.Text, paramInfo);
        }

        public void ClearEmployeeMasterSeparationDetails(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string DatabaseProfile
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE {0}..M_Employee
                                            SET Mem_SeparationNoticeDate = null
                                                , Mem_SeparationCode = null
                                                , Mem_SeparationDate = null
                                                , Mem_SeparationSysDate = null
                                                , Mem_ClearedDate = null
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId
                                            AND ISNULL((
                                                                    Select TOP(1) Tep_WorkStatus
                                                                    From T_EmpProfile
                                                                    Where Tep_IDNo = @EmployeeId 
                                                                        AND (Tep_StartDate BETWEEN @PayPeriodStart AND @PayPeriodEnd)
                                                                        And (ISNULL(Tep_EndDate, @PayPeriodStart) BETWEEN @PayPeriodStart AND @PayPeriodEnd)
                                                                    Order By Tep_StartDate DESC), '') <> 'IN'", DatabaseProfile);
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public DataSet GetEmployeeRecord(string EmployeeId
                                    , string Database
                                    , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            int paramInfoCnt = 0;
            DataSet ds;

            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);

            #region query
            string query = string.Format(@"SELECT
	                                        *
                                        FROM {0}..M_Employee
                                        WHERE Mem_IDNo = @EmployeeId", Database);
            #endregion

            ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

            return ds;
        }

        public DataTable GetLatestJobStatusMovement(string EmployeeId, string CentralProfile)
        {
            #region query
            string query = string.Format(@"Select TOP(1) * 
                                           From T_EmpProfile
                                           Where Tep_IDNo = '{0}'
                                           Order By Tep_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetLatestJobStatusMovement(string EmployeeId, DateTime StartDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"Select TOP(2) * 
                                           From T_EmpProfile
                                           INNER JOIN M_Profile
                                              ON Tep_TargetProfile = Mpf_DatabaseNo
                                           Where Tep_IDNo = '{0}'
                                              And Tep_StartDate <= '{1}'
                                           Order By Tep_StartDate DESC", EmployeeId, StartDate);
            #endregion
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        
        public bool isWorkTypeExist(string idnumber, string newprofile)// string wrkgroup, string wrktype)
        {
            DataSet ds = new DataSet();
            DataSet dsWrkType = new DataSet();
            string wrkgroup = "";
            string wrktype = "";
            #region get workgrp/type
            string idquery = string.Format(@"select Mem_CalendarGroup, Mem_CalendarType 
                                            from M_Employee
                                            where Mem_IDNo = '{0}'",idnumber);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(idquery, CommandType.Text);
                dal.CloseDB();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                wrkgroup = ds.Tables[0].Rows[0][0].ToString();
                wrktype = ds.Tables[0].Rows[0][1].ToString();
            }

            #endregion

            
            #region check calendar group
            string query = string.Format(@"select Tcl_CalendarType from {0}..T_Calendar
                                            where Tcl_CalendarGroup = '{1}' and Tcl_CalendarType = '{2}'
                                            ",newprofile, wrkgroup,wrktype);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsWrkType = dal.ExecuteDataSet(query,CommandType.Text);
                dal.CloseDB();
            }

            if (dsWrkType != null && dsWrkType.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void ExecuteProfileTransfer(string DBSource, string DBTarget, string EmployeeId, string UserLogin, DateTime StartDate, DALHelper dal)
        {
            DataTable dtTablesSaved = new DataTable();
            string strPreChecking = "", strWorkflowOnRoute = "", strErrorTables = "", strErrorCostCenter = "";

            dtTablesSaved.Columns.Add("Table Name");
            dtTablesSaved.Columns.Add("Records From Target");
            dtTablesSaved.Columns.Add("Records From Source");

            string errMsg = "";
            #region Pre-checking of Cutoff Flags
            strPreChecking = CheckCutoffFlagsBeforeTransfer(DBSource, DBTarget, dal);
            if (strPreChecking != "")
                errMsg += strPreChecking + "\n";

            #endregion
            #region Pre-checking of Unapproved Workflow Transactions
            strWorkflowOnRoute = CheckWorkflowTransactionsBeforeTransfer(DBSource, StartDate, EmployeeId, dal);
            if (strWorkflowOnRoute != "")
                errMsg += "There are still Workflow transactions on route for this employee.\nPlease approve or disapprove the transactions.\n\n" + strWorkflowOnRoute + "\n";
            #endregion

            strErrorCostCenter = CheckCostCenterBeforeTransfer(DBSource, DBTarget, EmployeeId, dal);
            if (strErrorCostCenter != "")
                errMsg += strErrorCostCenter + "\n";

            List<string>  strErrorViolationCode = CheckReferencesBeforeTransfer(DBSource, DBTarget, "T_EmpOffense", "Tof_OffenseCode", "Tof_IDNo", "M_OffenseHdr", "Moh_OffenseCode", EmployeeId, dal);
            if (strErrorViolationCode.Count > 0)
                errMsg += "The following Violation Code(s) of employee does not exist in the Destination Profile:" + CommonMessages.ErrorFields(strErrorViolationCode) + "\n";
            List<string> strPenalty = CheckReferencesBeforeTransfer(DBSource, DBTarget, "T_EmpOffense", "Tof_PenaltyCode", "Tof_IDNo", "M_OffenseDtl", "Mod_PenaltyCode", EmployeeId, dal);
            if (strPenalty.Count > 0)
                errMsg += "The following Penalty Code(s) of employee does not exist in the Destination Profile:" + CommonMessages.ErrorFields(strErrorViolationCode) + "\n";
            
            List<string> strDeduction = CheckReferencesBeforeTransfer(DBSource, DBTarget, "T_EmpDeductionHdr", "Tdh_DeductionCode", "Tdh_IDNo", "M_Deduction", "Mdn_DeductionCode", EmployeeId, dal);
            strDeduction = CheckReferencesBeforeTransfer(DBSource, DBTarget, "T_EmpDeductionDtlHst", "Tdd_DeductionCode", "Tdd_IDNo", "M_Deduction", "Mdn_DeductionCode", EmployeeId, strDeduction, dal);
            if (strDeduction.Count > 0)
                errMsg += "The following Deduction Code(s) of employee does not exist in the Destination Profile:" + CommonMessages.ErrorFields(strDeduction) + "\n";
            List<string> strLeaveType = CheckReferencesBeforeTransfer(DBSource, DBTarget, "T_EmpLeaveLdg", "Tll_LeaveCode", "Tll_IDNo", "M_Leave", "Mlv_LeaveCode", EmployeeId, dal);
            if (strLeaveType.Count > 0)
                errMsg += "The following Leave Type(s) of employee does not exist in the Destination Profile:" + CommonMessages.ErrorFields(strDeduction) + "\n";
            List<string> strPerformanceType = CheckReferencesBeforeTransfer(DBSource, DBTarget, "T_EmpEvaluation", "Tev_EvaluationType", "Tev_IDNo", "M_Evaluation", "Mev_EvaluationType", EmployeeId, dal);
            if (strLeaveType.Count > 0)
                errMsg += "The following Performance Type(s) of employee does not exist in the Destination Profile:" + CommonMessages.ErrorFields(strDeduction) + "\n";

            if (strPreChecking != "" || strWorkflowOnRoute != "" || strErrorCostCenter != "" || strErrorViolationCode.Count > 0 || strDeduction.Count > 0)
            {
                CommonProcedures.showMessageError(errMsg);
                throw new Exception("Profile Movement");
            }

            try
            {
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_Gender", "GENDER", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_CivilStatusCode", "CIVILSTAT", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_NationalityCode", "CITIZEN", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ReligionCode", "RELIGION", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PresAddressBarangay", "BARANGAY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PresAddressMunicipalityCity", "ZIPCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_EducationCode", "EDUCLEVEL", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_SchoolCode", "SCHOOL", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_CourseCode", "COURSE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_BloodType", "BLOODGROUP", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ICERelation", "RELATION", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ICEAddressBarangay", "BARANGAY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ICEAddressMunicipalityCity", "ZIPCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_SeparationCode", "SEPARATION", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ExpenseClass", "EMPTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ProvAddressBarangay", "BARANGAY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ProvAddressMunicipalityCity", "ZIPCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ShoesSize", "SHOESSIZE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ShirtSize", "SHIRTSIZE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_HairColor", "HAIRCOLOR", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_EyeColor", "EYECOLOR", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_RankCode", "POSITIONGRADE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PositionCategory", "POSCATGORY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PositionClass", "POSCLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PremiumGrpCode", "PREMGRP", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_JobGrade", "JOBGRADE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PayClass", "PAYCLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_EmploymentStatusCode", "EMPSTAT", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_WorkLocationCode", "ZIPCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_JobTitleCode", "POSITION", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PaymentMode", "PAYMODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PayrollType", "PAYTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_WorkStatus", "JOBSTATUS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_CalendarType", "WORKTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_CalendarGroup", "WORKGROUP", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PagIbigRule", "PREMCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_SSSRule", "PREMCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PHRule", "PREMCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_ExpenseClass", "EMPTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_TaxCode", "TAXCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mem_IDNo", "M_Employee", "Mem_PayrollBankCode", "BANK", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tas_IDNo", "T_EmpAsset", "Tas_AssetCode", "ASSETS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tas_IDNo", "T_EmpAsset", "Tas_AssetClass", "ASSETCLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tas_IDNo", "T_EmpAsset", "Tas_UnitOfMeasure", "ASSETUNIT", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tcc_IDNo", "T_EmpCostcenter", "Tcc_ReasonCode", "CCTRMVE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mfm_IDNo", "M_EmpFamily", "Mfm_RelationToEmployee", "RELATION", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Mfm_IDNo", "M_EmpFamily", "Mfm_Hierarchy", "HIERARCHDP", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tes_IDNo", "T_EmpEmploymentStatus", "Tes_EmploymentStatusCode", "EMPSTAT", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tes_IDNo", "T_EmpEmploymentStatus", "Tes_ReasonCode", "EMPSTATMVE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tcg_IDNo", "T_EmpCalendarGroup", "Tcg_CalendarType", "WORKTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tcg_IDNo", "T_EmpCalendarGroup", "Tcg_ReasonCode", "GROUPMVE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tcg_IDNo", "T_EmpCalendarGroup", "Tcg_CalendarGroup", "WORKGROUP", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_JobTitleCode", "POSITION", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_RankCode", "POSITIONGRADE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_PositionCategory", "POSITIONLEVEL", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_PositionClass", "POSCLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_JobGradeCode", "JOBGRADE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_PayClass", "PAYCLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpo_IDNo", "T_EmpPosition", "Tpo_ReasonCode", "POSMVE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpg_IDNo", "T_EmpPremiumGroup", "Tpg_PremiumGroup", "PREMCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tpg_IDNo", "T_EmpPremiumGroup", "Tpg_ReasonCode", "PREMMVE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tsl_IDNo", "T_EmpSalary", "Tsl_PayrollType", "PAYTYPE", dal);
                //TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tsl_IDNo", "T_EmpSalary", "Tsl_SalaryType", "PAYRATE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tsl_IDNo", "T_EmpSalary", "Tsl_ReasonCode", "SALARYMVE", dal);
                //TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tsl_IDNo", "T_EmpSalary", "Tsl_CurrencyCode", "CURRENCY", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tds_IDNo", "T_EmpDesignation", "Tds_JobTitleCode", "POSITION", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingCode", "TRAINING", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingType", "TRAINTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_EmployerCode", "EMPLOYER", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingInstitution", "TRAININGIN", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingVenue", "TRAININGLC", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_ResourcePerson", "TRAINOR", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_DocumentType", "DOCTYPE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_Location", "COUNTRY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_Entry", "DOCENTRY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_DocumentStatus", "DOCSTATUS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_DocumentKeptBy", "DOCKEEPING", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_DocumentCategory", "DOCATEGORY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_ProcessedBy", "DOCPROCBY", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tde_IDNo", "T_EmpDocumentExpiry", "Tde_RenewedBy", "DOCPROCBY", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Twl_IDNo", "T_EmpWorkLocation", "Twl_WorkLocationCode", "ZIPCODE", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Twl_IDNo", "T_EmpWorkLocation", "Twl_ReasonCode", "LOCMVE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tof_IDNo", "T_EmpOffense", "Tof_OffenseNature", "VIOCAT", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tof_IDNo", "T_EmpOffense", "Tof_OffenseStatus", "VIOSTAT", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingCode", "TRAINING", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_EmployerCode", "EMPLOYER", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingInstitution", "TRAININGIN", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_TrainingVenue", "TRAININGLC", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tet_IDNo", "T_EmpTraining", "Tet_ResourcePerson", "TRAINOR", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Med_IDNo", "M_EmpEducation", "Med_EducationCode", "EDUCLEVEL", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Med_IDNo", "M_EmpEducation", "Med_SchoolCode", "SCHOOL", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Med_IDNo", "M_EmpEducation", "Med_CourseCode", "COURSE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tev_IDNo", "T_EmpEvaluation", "Tev_EvaluationType", "PATYPE", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tlv_IDNo", "T_EmpLeave", "Tlv_LeaveUnit", "DAYUNIT", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tlv_IDNo", "T_EmpLeave", "Tlv_InitiatedBy", "LVEINTIATR", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tlv_IDNo", "T_EmpLeave", "Tlv_LeaveReasonCode", "LVEREASON", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tlv_IDNo", "T_EmpLeave", "Tlv_LeaveClass", "LVECLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tlv_IDNo", "T_EmpLeave", "Tlv_LeaveStatus", "DOCSTATUS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tlv_IDNo", "T_EmpLeave", "Tlv_LeaveFlag", "DOCUFLAG", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tot_IDNo", "T_EmpOvertime", "Tot_OvertimeClass", "OTCLASS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tot_IDNo", "T_EmpOvertime", "Tot_OvertimeStatus", "DOCSTATUS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Tot_IDNo", "T_EmpOvertime", "Tot_OvertimeFlag", "DOCUFLAG", dal);

                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Ttm_IDNo", "T_EmpTimeCorrection", "Ttm_TimeCorStatus", "DOCSTATUS", dal);
                TransferAccountDetail(DBSource, DBTarget, EmployeeId, "Ttm_IDNo", "T_EmpTimeCorrection", "Ttm_TimeCorFlag", "DOCUFLAG", dal);

                //end
            }
            catch { }

            #region Transfer HR-Related Tables
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_AuditTrl", "Tat_IDNo", dal); }
            catch { strErrorTables += "T_AuditTrl, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_Employee", "Mem_IDNo", dal); }
            catch { strErrorTables += "M_Employee, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpAsset", "Tas_IDNo", dal); }
            catch { strErrorTables += "T_EmpAsset, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpAchievement", "Tac_IDNo", dal); }
            catch { strErrorTables += "T_EmpAchievement, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_EmpFamily", "Mfm_IDNo", dal); }
            catch { strErrorTables += "M_EmpFamily, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpWorkExperience", "Twe_IDNo", dal); }
            catch { strErrorTables += "T_EmpWorkExperience, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpCostcenter", "Tcc_IDNo", dal); }
            catch { strErrorTables += "T_EmpCostcenter, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_EmpEducation", "Med_IDNo", dal); }
            catch { strErrorTables += "M_EmpEducation, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpEmploymentStatus", "Tes_IDNo", dal); }
            catch { strErrorTables += "T_EmpEmploymentStatus, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpCalendarGroup", "Tcg_IDNo", dal); }
            catch { strErrorTables += "T_EmpCalendarGroup, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_EmpImage", "Mei_IDNo", dal); }
            catch { strErrorTables += "M_EmpImage, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpLeaveLdg", "Tll_IDNo", dal); }
            catch { strErrorTables += "T_EmpLeaveLdg, "; } 
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpNotes", "Tnt_IDNo", dal); }
            catch { strErrorTables += "T_EmpNotes, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpOffense", "Tof_IDNo", dal); }
            catch { strErrorTables += "T_EmpOffense, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpEvaluation", "Tev_IDNo", dal); }
            catch { strErrorTables += "T_EmpEvaluation, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpPosition", "Tpo_IDNo", dal); }
            catch { strErrorTables += "T_EmpPosition, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpPremiumGroup", "Tpg_IDNo", dal); }
            catch { strErrorTables += "T_EmpPremiumGroup, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpRest", "Ter_IDNo", dal); }
            catch { strErrorTables += "T_EmpRest, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpTranspo", "Tet_IDNo", dal); }
            catch { strErrorTables += "T_EmpTranspo, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpSalary", "Tsl_IDNo", dal); }
            catch { strErrorTables += "T_EmpSalary, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpDesignation", "Tds_IDNo", dal); }
            catch { strErrorTables += "T_EmpDesignation, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpTraining", "Tet_IDNo", dal); }
            catch { strErrorTables += "T_EmpTraining, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpDocumentExpiry", "Tde_IDNo", dal); }
            catch { strErrorTables += "T_EmpDocumentExpiry, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_EmpUnion", "Mup_IDNo", dal); }
            catch { strErrorTables += "M_EmpUnion, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpWorkLocation", "Twl_IDNo", dal); }
            catch { strErrorTables += "T_EmpWorkLocation, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpGovRemittance", "Tgr_IDNo", dal); }
            catch { strErrorTables += "T_EmpGovRemittance, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_UserExt", "Mue_UserCode", dal); }
            catch { strErrorTables += "M_UserExt, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_UserDtl", " Mud_UserCode", dal); }
            catch { strErrorTables += "M_UserDtl, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "M_UserHdr", "Muh_UserCode", dal); }
            catch { strErrorTables += "M_UserHdr, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpYTD", "Tyt_IDNo", dal); }
            catch { strErrorTables += "T_EmpYTD, "; }
            try { TransferRecords(DBSource, DBTarget, EmployeeId, "T_EmpApprovalRoute", "Tar_IDNo", dal); }
            catch { strErrorTables += "T_EmpApprovalRoute, "; }
            #endregion

            //Create Selected Tables in Target DB
            TransferWorkflowTransactions(DBSource, DBTarget, EmployeeId, StartDate.ToString(), dal);
            TransferDeductions(DBSource, DBTarget, EmployeeId, dal);
            TransferEmployeeLogLedger(DBSource, DBTarget, EmployeeId, StartDate.ToString(), LoginInfo.getUser().CentralProfileName, dal);

            //Delete Records from Source DB
            try { DeleteEmployeeRecords(DBSource, EmployeeId, "T_EmpLeaveLdg", "Tll_IDNo", dal); }
            catch { strErrorTables += "T_EmpLeaveLdg, "; }
            try { DeleteEmployeeRecords(DBSource, EmployeeId, "T_EmpGovRemittance", "Tgr_IDNo", dal); }
            catch { strErrorTables += "T_EmpGovRemittance, "; }
            try { DeleteEmployeeRecords(DBSource, EmployeeId, "T_EmpYTD", "Tyt_IDNo", dal); }
            catch { strErrorTables += "T_EmpYTD, "; }

            if (strErrorTables != "")
                throw new Exception("Cannot transfer records in the ff. tables: \n\n " + strErrorTables);
        }

        private string CheckCutoffFlagsBeforeTransfer(string DBSource, string DBTarget, DALHelper dal)
        {
            #region Query
            string query = string.Format(@"
DECLARE @FlagSource AS BIT
DECLARE @FlagDestination AS BIT

SELECT @FlagSource = Tsc_SetFlag 
FROM {0}..T_SettingControl
WHERE Tsc_SettingCode = 'CUT-OFF' AND Tsc_SystemCode = 'TIMEKEEP'

SELECT @FlagDestination = Tsc_SetFlag 
FROM {1}..T_SettingControl
WHERE Tsc_SettingCode = 'CUT-OFF' AND Tsc_SystemCode = 'TIMEKEEP'

SELECT CASE WHEN @FlagSource != @FlagDestination
		THEN CASE WHEN @FlagSource = 1
			THEN 'Source profile is on Timekeep Cutoff. Please ensure that the cutoff flag is released before moving the records.'
			WHEN @FlagDestination = 1
			THEN 'Destination profile is on Timekeep Cutoff. Please ensure that the cutoff flag is released before moving the records.'
			END
		ELSE CASE WHEN @FlagSource = 1 AND @FlagDestination = 1
			THEN 'Both Source and Destination profiles are on Timekeep Cutoff. Please ensure that the cutoff flags are released before moving the records.'
			END
		END

SELECT @FlagSource = Tsc_SetFlag 
FROM {0}..T_SettingControl
WHERE Tsc_SettingCode = 'CUT-OFF' AND Tsc_SystemCode = 'PAYROLL'

SELECT @FlagDestination = Tsc_SetFlag 
FROM {1}..T_SettingControl
WHERE Tsc_SettingCode = 'CUT-OFF' AND Tsc_SystemCode = 'PAYROLL'

SELECT CASE WHEN @FlagSource != @FlagDestination
		THEN CASE WHEN @FlagSource = 1
			THEN 'Source profile is on Payroll Cutoff. Please ensure that the cutoff flag is released before moving the records.'
			WHEN @FlagDestination = 1
			THEN 'Destination profile is on Payroll Cutoff. Please ensure that the cutoff flag is released before moving the records.'
			END
		ELSE CASE WHEN @FlagSource = 1 AND @FlagDestination = 1
			THEN 'Both Source and Destination profiles are on Payroll Cutoff. Please ensure that the cutoff flags are released before moving the records.'
			END
		END

SELECT @FlagSource = Tsc_SetFlag 
FROM {0}..T_SettingControl
WHERE Tsc_SettingCode = 'PAYCALC' AND Tsc_SystemCode = 'PAYROLL'

SELECT @FlagDestination = Tsc_SetFlag 
FROM {1}..T_SettingControl
WHERE Tsc_SettingCode = 'PAYCALC' AND Tsc_SystemCode = 'PAYROLL'

SELECT CASE WHEN @FlagSource != @FlagDestination
		THEN CASE WHEN @FlagSource = 1
			THEN 'Source profile has already posted the payroll transactions. Please restore the allowances before moving the records.'
			WHEN @FlagDestination = 1
			THEN 'Destination profile has already posted the payroll transactions. Please restore the allowances before moving the records.'
			END
		ELSE CASE WHEN @FlagSource = 1 AND @FlagDestination = 1
			THEN 'Both Source and Destination profiles has already posted the payroll transactions. Please restore the allowances before moving the records.'
			END
		END", DBSource, DBTarget);
            #endregion

            string strErrorMsg = "";
            DataSet dsResult = dal.ExecuteDataSet(query);
            if (dsResult.Tables.Count > 0)
            {
                if (dsResult.Tables[0].Rows[0][0].ToString() != "")
                    strErrorMsg += dsResult.Tables[0].Rows[0][0].ToString() + "\n\n"; //TIMEKEEP cutoff
                if (dsResult.Tables[1].Rows[0][0].ToString() != "")
                    strErrorMsg += dsResult.Tables[1].Rows[0][0].ToString() + "\n\n"; //PAYROLL cutoff
                if (dsResult.Tables[2].Rows[0][0].ToString() != "")
                    strErrorMsg += dsResult.Tables[2].Rows[0][0].ToString() + "\n\n"; //PAYTRNPOST flag
            }

            return strErrorMsg;
        }

        private List<string> CheckReferencesBeforeTransfer(string DBSource
            , string DBTarget
            , string ReferencingTable
            , string ReferencingColumn
            , string ReferencingEmployeeIDColumn
            , string ForeignTable
            , string ForeignColumn
            , string EmployeeId
            , DALHelper dal)
        {
            return CheckReferencesBeforeTransfer(DBSource
                , DBTarget
                , ReferencingTable
                , ReferencingColumn
                , ReferencingEmployeeIDColumn
                , ForeignTable
                , ForeignColumn
                , EmployeeId
                , new List<string>()
                , dal);
        }

        private List<string> CheckReferencesBeforeTransfer(string DBSource
            , string DBTarget
            , string ReferencingTable
            , string ReferencingColumn
            , string ReferencingEmployeeIDColumn
            , string ForeignTable
            , string ForeignColumn
            , string EmployeeId
            , List<string> CurrentErrorList
            , DALHelper dal)
        {

            string query = string.Format(@"
                            DECLARE @Reference TABLE
							(
								ReferenceCol VARCHAR(20)
							)
							INSERT INTO @Reference
                            SELECT {4} FROM {0}..{3}
                            WHERE {5} = '{2}'
                            
                            SELECT 
                            CASE WHEN b.{7} IS NULL
                            THEN A.{7}
                            ELSE ''
                            END
	                        FROM {0}..{6} A
                            LEFT JOIN {1}..{6} B
                                ON A.{7} = B.{7}
                            WHERE A.{7} IN (SELECT ReferenceCol FROM @Reference)"
                , DBSource
                , DBTarget
                , EmployeeId
                , ReferencingTable
                , ReferencingColumn
                , ReferencingEmployeeIDColumn
                , ForeignTable
                , ForeignColumn);
            DataSet ds = dal.ExecuteDataSet(query);
            //string error = "";
            for (int i = 0; ds != null && i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i][0] != null && ds.Tables[0].Rows[i][0].ToString() != "" && !CurrentErrorList.Contains(ds.Tables[0].Rows[i][0].ToString()))
                    CurrentErrorList.Add(ds.Tables[0].Rows[i][0].ToString());
            }
            return CurrentErrorList;
        }

        private string CheckCostCenterBeforeTransfer(string DBSource, string DBTarget, string EmployeeId, DALHelper dal)
        {
            string query = string.Format(@"
                            DECLARE @CostCenter VARCHAR(20)
                            SET @CostCenter = (SELECT Mem_CostcenterCode FROM {0}..M_Employee
                            WHERE Mem_IDNo = '{2}')
                            
                            SELECT 
                            CASE WHEN b.Mcc_CostCenterCode IS NULL
                            THEN 'CostCenter code of employee does not exist in the Destination Profile.\nCostCenterCode: ' + A.Mcc_CostCenterCode
                            END, 
                            CASE WHEN {0}.dbo.Udf_DisplayCostCenterName(A.Mcc_CostCenterCode) != {1}.dbo.Udf_DisplayCostCenterName(B.Mcc_CostCenterCode)
                            THEN 'CostCenter description of employee does not match in the Destination Profile.\nSource: '
	                            + {0}.dbo.Udf_DisplayCostCenterName(A.Mcc_CostCenterCode) + '\nDestination: ' + {1}.dbo.Udf_DisplayCostCenterName(B.Mcc_CostCenterCode)
                            END,
                            CASE WHEN {0}.dbo.Udf_DisplayCostCenterName(A.Mcc_CostCenterCode) != {1}.dbo.Udf_DisplayCostCenterName(B.Mcc_CostCenterCode)
                            THEN 'CostCenter short name of employee does not match in the Destination Profile.\nSource: '
	                            + {0}.dbo.Udf_DisplayCostCenterName(A.Mcc_CostCenterCode) + '\nDestination: ' + {1}.dbo.Udf_DisplayCostCenterName(B.Mcc_CostCenterCode)
                            END
	                        FROM {0}..M_CostCenter A
                            LEFT JOIN {1}..M_CostCenter B
                                ON A.Mcc_CostCenterCode = B.Mcc_CostCenterCode
                            WHERE A.Mcc_CostCenterCode = @CostCenter", DBSource, DBTarget, EmployeeId);
            DataSet ds =dal.ExecuteDataSet(query);
            string error = "";
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != null && ds.Tables[0].Rows[0][0].ToString() != "")
                    error += ds.Tables[0].Rows[0][0].ToString().Replace("\\n", "\n") + "\n\n";
                if (ds.Tables[0].Rows[0][1] != null && ds.Tables[0].Rows[0][1].ToString() != "")
                    error += ds.Tables[0].Rows[0][1].ToString().Replace("\\n", "\n") + "\n\n";
                if (ds.Tables[0].Rows[0][2] != null && ds.Tables[0].Rows[0][2].ToString() != "")
                    error += ds.Tables[0].Rows[0][2].ToString().Replace("\\n", "\n") + "\n\n";
            }
            return error != "" ? error.Substring(0, error.Length - 1) : "";
        }

        private string CheckWorkflowTransactionsBeforeTransfer(string DBSource, DateTime StartDate, string EmployeeId,DALHelper dal)
        {
            #region Query
            string query = string.Format(@"
SELECT 'OVERTIME' AS [Trans], COUNT(*) AS [Cnt]
FROM {0}..T_EmpOvertime
WHERE Tot_OvertimeStatus IN ('01','02','04','06','08','10','12')
	AND Tot_IDNo = '{1}'
    AND Tot_OverTimeDate < '{2}'

SELECT 'LEAVE' AS [Trans], COUNT(*) AS [Cnt]
FROM {0}..T_EmpLeave
WHERE Tlv_LeaveStatus IN ('01','02','04','06','08','10','12')
	AND Tlv_IDNo = '{1}'
    AND Tlv_LeaveDate < '{2}'

SELECT 'TIME CORRECTION' AS [Trans], COUNT(*) AS [Cnt]
FROM {0}..T_EmpTimeCorrection
WHERE Ttm_TimeCorStatus IN ('01','02','04','06','08','10','12')
	AND Ttm_IDNo = '{1}'
    AND Ttm_TimeCorDate < '{2}'

---SELECT 'WORKGROUP/RESTDAY/SHIFT MOVEMENT' AS [Trans], COUNT(*) AS [Cnt]
---FROM {0}..T_Movement
---WHERE Mve_Status IN ('01','02','04','06','08','10','12')
---AND Mve_EmployeeId = '{1}'", DBSource, EmployeeId, StartDate.ToString());
            #endregion

            string strErrorMsg = "";
            DataSet dsResult = dal.ExecuteDataSet(query);
            if (dsResult.Tables.Count > 0)
            {
                if (Convert.ToInt32(dsResult.Tables[0].Rows[0][1]) > 0)
                    strErrorMsg += dsResult.Tables[0].Rows[0][0].ToString() + " (" + dsResult.Tables[0].Rows[0][1].ToString() + ")\n"; //OT
                if (Convert.ToInt32(dsResult.Tables[1].Rows[0][1]) > 0)
                    strErrorMsg += dsResult.Tables[1].Rows[0][0].ToString() + " (" + dsResult.Tables[1].Rows[0][1].ToString() + ")\n"; //LEAVE
                if (Convert.ToInt32(dsResult.Tables[2].Rows[0][1]) > 0)
                    strErrorMsg += dsResult.Tables[2].Rows[0][0].ToString() + " (" + dsResult.Tables[2].Rows[0][1].ToString() + ")\n"; //TIMECOR
                //if (Convert.ToInt32(dsResult.Tables[3].Rows[0][1]) > 0)
                //    strErrorMsg += dsResult.Tables[3].Rows[0][0].ToString() + " (" + dsResult.Tables[3].Rows[0][1].ToString() + ")\n"; //MOVEMENT
            }

            return strErrorMsg;
        }

        private void TransferEmployeeLogLedger(string DBSource, string DBTarget, string EmployeeId, string StartDate, string CentralProfile, DALHelper dalHelper)
        {
            string strCurPeriod = "";
            string strNextPeriod = "";
            DataTable dtResult;
            string query = "";
            string user = LoginInfo.getUser().UserCode;

            //if (CheckWeeklyCode(DBSource, DBTarget) != "NONE")
            //{
                //Get Current Pay Period from Target DB
                query = string.Format(@"SELECT Tps_PayCycle 
                                            FROM {0}..T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'", DBTarget);
                dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
                if (dtResult.Rows.Count > 0)
                    strCurPeriod = dtResult.Rows[0][0].ToString();

                //Get Next Pay Period from Target DB
                query = string.Format(@"DECLARE @EndDate as Datetime
                                            Set @EndDate = (Select Tps_EndCycle 
				                                            From {0}..T_PaySchedule
				                                            Where Tps_CycleIndicator = 'C' 
				                                            and Tps_RecordStatus = 'A')

                                            SELECT Tps_PayCycle
                                            FROM {0}..T_PaySchedule
                                            Where Tps_StartCycle = dateadd(dd,1,@EndDate)", DBTarget);
                dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
                if (dtResult.Rows.Count > 0)
                    strNextPeriod = dtResult.Rows[0][0].ToString();

                SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL(dalHelper, "");
                DataTable dtEmployee = SystemCycleProcessingBL.GetActiveEmployeeList(EmployeeId, DBTarget);

                //Create Log Ledger
                //SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                //                                                , strCurPeriod, "", ""
                //                                                , true, false, false, false, false, false, false, user, DBTarget, CentralProfile);
                //SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                //                                                , strNextPeriod, "", ""
                //                                                , true, false, false, false, false, false, false, user, DBTarget, CentralProfile);


                //Copy Logs from Source to Target DB
                query = string.Format(@"UPDATE {1}..T_EmpTimeRegister
                                            SET Ttr_ActIn_1 = A.Ttr_ActIn_1
	                                            , Ttr_ActOut_1 = A.Ttr_ActOut_1
	                                            , Ttr_ActIn_2 = A.Ttr_ActIn_2
	                                            , Ttr_ActOut_2 = A.Ttr_ActOut_2
                                            FROM {0}..T_EmpTimeRegister A
                                            INNER JOIN {1}..T_EmpTimeRegister B
                                            ON A.Ttr_IDNo = B.Ttr_IDNo
	                                            AND A.Ttr_Date = B.Ttr_Date
                                            WHERE A.Ttr_IDNo = '{2}'
                                            AND A.Ttr_Date >= '{3}'", DBSource, DBTarget, EmployeeId, StartDate);
                dalHelper.ExecuteNonQuery(query);


                //Remove Logs from SourceDB
                query = string.Format(@"UPDATE {0}..T_EmpTimeRegister
                                            SET Ttr_ActIn_1 = '0000'
	                                            , Ttr_ActOut_1 = '0000'
	                                            , Ttr_ActIn_2 = '0000'
	                                            , Ttr_ActOut_2 = '0000'
                                            WHERE Ttr_IDNo = '{1}'
                                            AND Ttr_Date >= '{2}'", DBSource, EmployeeId, StartDate);
                dalHelper.ExecuteNonQuery(query);

                //Correct Log Ledger
                //SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                //                                                , strCurPeriod, "", ""
                //                                                , false, true, true, true, true, true, false, user, DBTarget, CentralProfile);
                //SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                //                                                , strNextPeriod, "", ""
                //                                                , false, true, true, true, true, true, false, user, DBTarget, CentralProfile);
            //}
        }

        private void TransferWorkflowTransactions(string DBSource, string DBTarget, string EmployeeId, string StartDate, DALHelper dalHelper)
        {
            string strCurPeriod = "";
            DataTable dtResult;
            //Get Current Pay Period from Target DB
            string query = string.Format(@"SELECT Tps_PayCycle 
                                    FROM {0}..T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                    And Tps_RecordStatus = 'A'", DBTarget);
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                strCurPeriod = dtResult.Rows[0][0].ToString();

            #region Query
            query = string.Format(@"
INSERT INTO {3}..T_EmpOvertime
    (Tot_DocumentNo
    ,Tot_PayCycle
    ,Tot_RequestType
    ,Tot_RequestDate
    ,Tot_IDNo
    ,Tot_OvertimeDate
    ,Tot_OvertimeType
    ,Tot_StartTime
    ,Tot_EndTime
    ,Tot_OvertimeHours
    ,Tot_ReasonForRequest
    ,Tot_OvertimeClass
    ,Tot_OvertimeStatus
    ,Tot_OvertimeFlag
    ,Tot_SubmittedBy
    ,Tot_SubmittedDate
    ,Tot_Authority1
    ,Tot_Authority1Date
    ,Tot_Authority2
    ,Tot_Authority2Date
    ,Tot_Authority3
    ,Tot_Authority3Date
    ,Tot_Authority4
    ,Tot_Authority4Date
    ,Tot_Authority5
    ,Tot_Authority5Date
    ,Tot_ShiftCode
    ,Tot_CostcenterCode
    ,Tot_Authority1Comments
    ,Tot_Authority2Comments
    ,Tot_Authority3Comments
    ,Tot_Authority4Comments
    ,Tot_Authority5Comments
    ,Tot_Delegation
    ,Tot_DocumentBatchNo
    ,Tot_OriginalDocumentNo
    ,Tot_CreatedBy
    ,Tot_CreatedDate
    ,Tot_UpdatedBy
    ,Tot_UpdatedDate)
SELECT Tot_DocumentNo
    ,'{4}'
    ,Tot_RequestType
    ,Tot_RequestDate
    ,Tot_IDNo
    ,Tot_OvertimeDate
    ,Tot_OvertimeType
    ,Tot_StartTime
    ,Tot_EndTime
    ,Tot_OvertimeHours
    ,Tot_ReasonForRequest
    ,Tot_OvertimeClass
    ,Tot_OvertimeStatus
    ,Tot_OvertimeFlag
    ,Tot_SubmittedBy
    ,Tot_SubmittedDate
    ,Tot_Authority1
    ,Tot_Authority1Date
    ,Tot_Authority2
    ,Tot_Authority2Date
    ,Tot_Authority3
    ,Tot_Authority3Date
    ,Tot_Authority4
    ,Tot_Authority4Date
    ,Tot_Authority5
    ,Tot_Authority5Date
    ,Tot_ShiftCode
    ,Tot_CostcenterCode
    ,Tot_Authority1Comments
    ,Tot_Authority2Comments
    ,Tot_Authority3Comments
    ,Tot_Authority4Comments
    ,Tot_Authority5Comments
    ,Tot_Delegation
    ,Tot_DocumentBatchNo
    ,Tot_OriginalDocumentNo
    ,Tot_CreatedBy
    ,Tot_CreatedDate
    ,Tot_UpdatedBy
    ,Tot_UpdatedDate
FROM {0}..T_EmpOvertime
WHERE Tot_OvertimeDate >= '{1}'
	AND Tot_IDNo = '{2}'

INSERT INTO {3}..T_EmpLeave
    (Tlv_DocumentNo
    ,Tlv_PayCycle
    ,Tlv_RequestType
    ,Tlv_RequestDate
    ,Tlv_IDNo
    ,Tlv_LeaveDate
    ,Tlv_LeaveCode
    ,Tlv_StartTime
    ,Tlv_EndTime
    ,Tlv_LeaveHours
    ,Tlv_LeaveUnit
    ,Tlv_ReasonForRequest
    ,Tlv_InitiatedBy
    ,Tlv_LeaveReasonCode
    ,Tlv_LeaveClass
    ,Tlv_LeaveStatus
    ,Tlv_LeaveFlag
    ,Tlv_SubmittedBy
    ,Tlv_SubmittedDate
    ,Tlv_Authority1
    ,Tlv_Authority1Date
    ,Tlv_Authority2
    ,Tlv_Authority2Date
    ,Tlv_Authority3
    ,Tlv_Authority3Date
    ,Tlv_Authority4
    ,Tlv_Authority4Date
    ,Tlv_Authority5
    ,Tlv_Authority5Date
    ,Tlv_ShiftCode
    ,Tlv_CostcenterCode
    ,Tlv_Authority1Comments
    ,Tlv_Authority2Comments
    ,Tlv_Authority3Comments
    ,Tlv_Authority4Comments
    ,Tlv_Authority5Comments
    ,Tlv_Delegation
    ,Tlv_DocumentBatchNo
    ,Tlv_OriginalDocumentNo
    ,Tlv_CreatedBy
    ,Tlv_CreatedDate
    ,Tlv_UpdatedBy
    ,Tlv_UpdatedDate)
SELECT Tlv_DocumentNo
    ,'{4}'
    ,Tlv_RequestType
    ,Tlv_RequestDate
    ,Tlv_IDNo
    ,Tlv_LeaveDate
    ,Tlv_LeaveCode
    ,Tlv_StartTime
    ,Tlv_EndTime
    ,Tlv_LeaveHours
    ,Tlv_LeaveUnit
    ,Tlv_ReasonForRequest
    ,Tlv_InitiatedBy
    ,Tlv_LeaveReasonCode
    ,Tlv_LeaveClass
    ,Tlv_LeaveStatus
    ,Tlv_LeaveFlag
    ,Tlv_SubmittedBy
    ,Tlv_SubmittedDate
    ,Tlv_Authority1
    ,Tlv_Authority1Date
    ,Tlv_Authority2
    ,Tlv_Authority2Date
    ,Tlv_Authority3
    ,Tlv_Authority3Date
    ,Tlv_Authority4
    ,Tlv_Authority4Date
    ,Tlv_Authority5
    ,Tlv_Authority5Date
    ,Tlv_ShiftCode
    ,Tlv_CostcenterCode
    ,Tlv_Authority1Comments
    ,Tlv_Authority2Comments
    ,Tlv_Authority3Comments
    ,Tlv_Authority4Comments
    ,Tlv_Authority5Comments
    ,Tlv_Delegation
    ,Tlv_DocumentBatchNo
    ,Tlv_OriginalDocumentNo
    ,Tlv_CreatedBy
    ,Tlv_CreatedDate
    ,Tlv_UpdatedBy
    ,Tlv_UpdatedDate
FROM {0}..T_EmpLeave
WHERE Tlv_LeaveDate >= '{1}'
	AND Tlv_IDNo = '{2}'

INSERT INTO {3}..T_EmpTimeCorrection
    (Ttm_DocumentNo
    ,Ttm_PayCycle
    ,Ttm_RequestDate
    ,Ttm_IDNo
    ,Ttm_TimeCorDate
    ,Ttm_TimeCorType
    ,Ttm_TimeIn1
    ,Ttm_TimeOut1
    ,Ttm_TimeIn2
    ,Ttm_TimeOut2
    ,Ttm_ReasonForRequest
    ,Ttm_TimeCorStatus
    ,Ttm_TimeCorFlag
    ,Ttm_SubmittedBy
    ,Ttm_SubmittedDate
    ,Ttm_Authority1
    ,Ttm_Authority1Date
    ,Ttm_Authority2
    ,Ttm_Authority2Date
    ,Ttm_Authority3
    ,Ttm_Authority3Date
    ,Ttm_Authority4
    ,Ttm_Authority4Date
    ,Ttm_Authority5
    ,Ttm_Authority5Date
    ,Ttm_ShiftCode
    ,Ttm_CostcenterCode
    ,Ttm_Authority1Comments
    ,Ttm_Authority2Comments
    ,Ttm_Authority3Comments
    ,Ttm_Authority4Comments
    ,Ttm_Authority5Comments
    ,Ttm_Delegation
    ,Ttm_DocumentBatchNo
    ,Ttm_LogControl
    ,Ttm_CreatedBy
    ,Ttm_CreatedDate
    ,Ttm_UpdatedBy
    ,Ttm_UpdatedDate)
SELECT Ttm_DocumentNo
    ,'{4}'
    ,Ttm_RequestDate
    ,Ttm_IDNo
    ,Ttm_TimeCorDate
    ,Ttm_TimeCorType
    ,Ttm_TimeIn1
    ,Ttm_TimeOut1
    ,Ttm_TimeIn2
    ,Ttm_TimeOut2
    ,Ttm_ReasonForRequest
    ,Ttm_TimeCorStatus
    ,Ttm_TimeCorFlag
    ,Ttm_SubmittedBy
    ,Ttm_SubmittedDate
    ,Ttm_Authority1
    ,Ttm_Authority1Date
    ,Ttm_Authority2
    ,Ttm_Authority2Date
    ,Ttm_Authority3
    ,Ttm_Authority3Date
    ,Ttm_Authority4
    ,Ttm_Authority4Date
    ,Ttm_Authority5
    ,Ttm_Authority5Date
    ,Ttm_ShiftCode
    ,Ttm_CostcenterCode
    ,Ttm_Authority1Comments
    ,Ttm_Authority2Comments
    ,Ttm_Authority3Comments
    ,Ttm_Authority4Comments
    ,Ttm_Authority5Comments
    ,Ttm_Delegation
    ,Ttm_DocumentBatchNo
    ,Ttm_LogControl
    ,Ttm_CreatedBy
    ,Ttm_CreatedDate
    ,Ttm_UpdatedBy
    ,Ttm_UpdatedDate
FROM {0}..T_EmpTimeCorrection
WHERE Ttm_TimeCorDate >= '{1}'
	AND Ttm_IDNo = '{2}'

--INSERT INTO {2}..T_Movement
--    (Mve_ControlNo
--    ,Mve_CurrentPayPeriod
--    ,Mve_EmployeeId
--    ,Mve_EffectivityDate
--    ,Mve_AppliedDate
--    ,Mve_Costcenter
--    ,Mve_Type
--    ,Mve_From
--    ,Mve_To
--    ,Mve_Reason
--    ,Mve_EndorsedDateToChecker
--    ,Mve_CheckedBy
--    ,Mve_CheckedDate
--    ,Mve_Checked2By
--    ,Mve_Checked2Date
--    ,Mve_ApprovedBy
--    ,Mve_ApprovedDate
--    ,Mve_Status
--    ,Mve_BatchNo
--    ,Mve_Flag
--    ,Mve_CostCenterLine
--    ,Usr_Login
--    ,Ludatetime)
--SELECT Mve_ControlNo
--    ,'{3}'
--    ,Mve_EmployeeId
--    ,Mve_EffectivityDate
--    ,Mve_AppliedDate
--    ,Mve_Costcenter
--    ,Mve_Type
--    ,Mve_From
--    ,Mve_To
--    ,Mve_Reason
--    ,Mve_EndorsedDateToChecker
--    ,Mve_CheckedBy
--    ,Mve_CheckedDate
--    ,Mve_Checked2By
--    ,Mve_Checked2Date
--    ,Mve_ApprovedBy
--    ,Mve_ApprovedDate
--    ,Mve_Status
--    ,Mve_BatchNo
--    ,Mve_Flag
--    ,Mve_CostCenterLine
--    ,Usr_Login
--    ,Ludatetime
--FROM {0}..T_Movement
--WHERE Mve_Status in ('14','15')
--	AND Mve_EffectivityDate >= (SELECT Tps_StartCycle 
--							 FROM {0}..T_PaySchedule
--							 WHERE Tps_CycleIndicator = 'C')
--	AND Mve_EmployeeId = '{1}'

", DBSource, StartDate, EmployeeId, DBTarget, strCurPeriod);
            #endregion
            dalHelper.ExecuteNonQuery(query);

            #region Delete EWSS
            query = string.Format(@"
            Delete from {0}..T_EmpOvertime 
            where Tot_OvertimeDate >= '{1}'
	        AND Tot_IDNo = '{2}' 

            Delete from {0}..T_EmpLeave 
            where Tlv_LeaveDate >= '{1}'
	        AND Tlv_IDNo = '{2}'

            Delete from {0}..T_EmpTimeCorrection 
            where Ttm_TimeCorDate >= '{1}'
	        AND Ttm_IDNo = '{2}'
            ", DBSource, StartDate, EmployeeId);

            #endregion  

            dalHelper.ExecuteNonQuery(query);

        }

        private void TransferDeductions(string DBSource, string DBTarget, string EmployeeId, DALHelper dalHelper)
        {
            string strPrevPeriod = "";
            DataTable dtResult;
            //Get Previous Pay Period from Target DB
            string query = string.Format(@"Select Tps_PayCycle 
                                            From {0}..T_PaySchedule 
                                            Where Tps_EndCycle = (Select dateadd(dd, -1, Tps_StartCycle)
                                                                From {0}..T_PaySchedule
                                                                Where Tps_CycleIndicator = 'C'
						                                            and Tps_RecordStatus = 'A')
                                            And Tps_RecordStatus = 'A'", DBTarget);
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                strPrevPeriod = dtResult.Rows[0][0].ToString();

            query = string.Format(@"INSERT INTO {1}..T_EmpDeductionHdr
                                    SELECT * 
                                    FROM {0}..T_EmpDeductionHdr
                                    WHERE Tdh_PaidAmount < Tdh_DeductionAmount
                                        AND Tdh_IDNo = '{2}'

                                    INSERT INTO {1}..T_EmpDeductionDtlHst
                                    SELECT Tdd_IDNo
	                                    , Tdd_DeductionCode
	                                    , Tdd_StartDate
	                                    , '{3}'
	                                    , '{3}'
	                                    , '00'
	                                    , 'T' --TRANSFERRED TRANSACTION
	                                    , SUM(Tdd_Amount)
	                                    , 0
	                                    , 1
	                                    , 'TRANSFERRED TRANSACTION'
	                                    , 'SA'
	                                    , GETDATE()
                                    FROM {0}..T_EmpDeductionDtlHst
                                    INNER JOIN {0}..T_EmpDeductionHdr
                                    ON Tdd_IDNo = Tdh_IDNo
	                                    AND Tdd_DeductionCode = Tdh_DeductionCode
	                                    AND Tdd_StartDate = Tdh_StartDate
                                    WHERE Tdh_PaidAmount < Tdh_DeductionAmount
	                                    AND Tdd_PaymentFlag = 1
	                                    AND Tdh_IDNo = '{2}'
                                    GROUP BY Tdd_IDNo, Tdd_DeductionCode, Tdd_StartDate", DBSource, DBTarget, EmployeeId, strPrevPeriod);
            dalHelper.ExecuteNonQuery(query);
        }

        private bool GetProcessControlFlag(string Tsc_SystemCode, string Tsc_SettingCode, string DBTarget)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@" --USE {0}
                                Select Tsc_SetFlag, Tsc_RecordStatus, Tsc_SettingName
                                From {0}..T_SettingControl
                                Where Tsc_SystemCode = @Tsc_SystemCode
                                And Tsc_SettingCode = @Tsc_SettingCode
                                And Tsc_RecordStatus = 'A'", DBTarget);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            paramInfo[1] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString().Trim());
            }
            else
                throw (new Exception(Tsc_SystemCode + " - " + Tsc_SettingCode + " is not yet setup."));
        }

        private TimeLog GetTimeLogOfEmployee(DateTime dProcessdate, DataTable dtResult)
        {
            TimeLog timeLog = new TimeLog();
            foreach (DataRow dtRow in dtResult.Rows)
            {
                if (Convert.ToDateTime(dtRow["Ttr_Date"]) == dProcessdate)
                {
                    timeLog = new TimeLog(dtRow["Ttr_ActIn_1"].ToString(),
                                        dtRow["Ttr_ActIn_2"].ToString(),
                                        dtRow["Ttr_ActOut_1"].ToString(),
                                        dtRow["Ttr_ActOut_2"].ToString());
                    break;
                }
            }
            return timeLog;
        }

        private string CheckWeeklyCode(string DBSource, string DBTarget)
        {
            string query = string.Format(@"IF((SELECT Tsc_SetFlag FROM {0}..T_SettingControl 
                                            WHERE Tsc_SettingCode = 'Weekpay')=1 AND
                                            (SELECT Tsc_SetFlag FROM {1}..T_SettingControl 
                                            WHERE Tsc_SettingCode = 'Weekpay')=0)
	                                            SELECT 'TOMONTHLY'
                                            ELSE IF((SELECT Tsc_SetFlag FROM {0}..T_SettingControl 
                                            WHERE Tsc_SettingCode = 'Weekpay')=0 AND
                                            (SELECT Tsc_SetFlag FROM {1}..T_SettingControl 
                                            where Tsc_SettingCode = 'Weekpay')=1)
	                                            SELECT 'TOWEEKLY'
                                            ELSE SELECT 'NONE'", DBSource, DBTarget);
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult.Rows[0][0].ToString();
        }

        private DataTable GetLogLedgerPayPeriods(string DBTarget)
        {
            string query = string.Format(@"SELECT DISTINCT Ttr_PayCycle AS 'Payroll Period'
			                             , Convert(char(10),Tps_StartCycle,101) as 'Start Date'
			                             , Convert(char(10),Tps_EndCycle,101) as 'End Date'
                            FROM {0}..T_EmpTimeRegister
                            INNER JOIN {0}..T_PaySchedule
                            ON Ttr_PayCycle = Tps_PayCycle", DBTarget);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        private int TransferAccountDetail(string DBSource
            , string DBTarget
            , string EmployeeID
            , string EmployeeIDTableName
            , string TableName
            , string TableColumnName
            , string AccountType
            , DALHelper dal)
        {
            int ret = 0;
            string query = string.Format(@"
                                            DECLARE @Account TABLE
                                            (
                                                AccountCode VARCHAR(20)
                                            )
                                            INSERT INTO @Account
                                            SELECT {4} 
                                            FROM {0}..{3}
                                            WHERE {6} = '{2}'
                                            GROUP BY {4}

                                            INSERT INTO {1}..M_CodeDtl
	                                            (Mcd_CodeType
	                                            ,Mcd_Code
	                                            ,Mcd_Name
	                                            ,Mcd_RecordStatus
	                                            ,Usr_Login
	                                            ,Ludatetime)
                                            SELECT
	                                            Mcd_CodeType
	                                            ,Mcd_Code
	                                            ,Mcd_Name
	                                            ,Mcd_RecordStatus
	                                            ,Usr_Login
	                                            ,Ludatetime
                                            FROM {0}..M_CodeDtl
                                            WHERE Mcd_CodeType = '{5}'
                                            AND Mcd_Code IN (
                                                                    SELECT * FROM @Account
                                                                    WHERE AccountCode NOT IN 
                                                                    (SELECT Mcd_Code
                                                                    FROM {1}..M_CodeDtl
                                                                    WHERE Mcd_CodeType = '{5}')
                                                                    )
                                            ", DBSource, DBTarget, EmployeeID, TableName, TableColumnName, AccountType, EmployeeIDTableName);
            ret = dal.ExecuteNonQuery(query);
            return ret;
        }
        private DataRow TransferRecords(string DBSource
                                        , string DBTarget
                                        , string EmployeeId
                                        , string TableName
                                        , string EmployeeIdColumnName
                                        , DALHelper dal)
        {
            #region query
            string query = string.Format(@"DECLARE @EmployeeId AS varchar(15)
                                            SET @EmployeeId = '{0}'

                                            DECLARE @DelRowCount AS int
                                            SET @DelRowCount = 0

                                            DECLARE @InsRowCount AS int
                                            SET @InsRowCount = 0

                                            DELETE FROM {2}..{3}
                                            WHERE {4} = @EmployeeId

                                            SET @DelRowCount = @@ROWCOUNT

                                            INSERT INTO {2}..{3}
                                            SELECT * FROM {1}..{3}
                                            WHERE {4} = @EmployeeId

                                            SET @InsRowCount = @@ROWCOUNT

                                            SELECT '{3}' AS 'Table Name'
                                                ,@DelRowCount AS 'Records From Target'
                                                ,@InsRowCount AS 'Records From Source'"
                                                , EmployeeId, DBSource, DBTarget, TableName, EmployeeIdColumnName);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0];
            return null;
        }

        private void DeleteEmployeeRecords(string DBSource
                                            , string EmployeeId
                                            , string TableName
                                            , string EmployeeIdColumnName
                                            , DALHelper dal)
        {
            #region query
            string query = string.Format(@"DELETE FROM {1}..{2}
                                            WHERE {3} = '{0}'"
                                                , EmployeeId, DBSource, TableName, EmployeeIdColumnName);
            #endregion
            dal.ExecuteNonQuery(query);
        }
        #endregion

        #region Work Location Movement
        public DataTable GetEmployeeWorkLocationMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT Twl_StartDate as 'Start Date'
                                                  ,Twl_EndDate as 'End Date'
                                                  ,Twl_WorkLocationCode as 'Location Code'
                                                  ,Location.Mcd_Name as 'Description'
                                                  ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpWorkLocation
                                          LEFT JOIN M_CodeDtl Location
                                            ON Location.Mcd_Code = Twl_WorkLocationCode
                                            AND Location.Mcd_CodeType = 'ZIPCODE'
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Twl_ReasonCode
                                            AND Reason.Mcd_CodeType = 'LOCMVE'
                                          WHERE Twl_IDNo = '{0}'
                                          ORDER BY Twl_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeWorkLocationMovement(string SystemID, string user_Logged, string ProfileCode)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Twl_IDNo as 'Employee ID'
	                                              ,Mem_LastName as 'Last Name'
	                                              ,Mem_FirstName as 'First Name'
                                                  ,Twl_StartDate as 'Effective Date'
                                                  ,Twl_WorkLocationCode as 'Location'
                                                  ,Location.Mcd_Name as 'Description'
                                                  ,Twl_ReasonCode as 'Reason Code'
                                                  ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpWorkLocation
                                          INNER JOIN M_Employee
                                            ON Twl_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
@ACCESSRIGHTS
                                          LEFT JOIN M_CodeDtl Location
                                            ON Location.Mcd_Code = Twl_WorkLocationCode
                                            AND Location.Mcd_CodeType = 'ZIPCODE'
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Twl_ReasonCode
                                            AND Reason.Mcd_CodeType = 'LOCMVE'
                                          WHERE Twl_IDNo + Convert(Char(10),Twl_StartDate,112) in ( SELECT Twl_IDNo + Convert(Char(10), Twl_StartDate, 112)
																                                 FROM (SELECT Twl_IDNo , Max(Twl_StartDate) as Twl_StartDate
																			                            FROM T_EmpWorkLocation
																			                            WHERE @EndNewCycle >= Twl_StartDate
																					                        AND ISNULL(Twl_EndDate, @StartNewCycle) >= @StartNewCycle
																			                            GROUP BY Twl_IDNo ) Temp)
                                          ORDER BY Mem_LastName, Mem_FirstName");
            #endregion

            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(ProfileCode, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", "","", false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeeWorkLocationMovementRow(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT
	                                            Mem_WorkLocationCode
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'", EmployeeId);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeeWorkLocationMovement(string EmployeeId, DateTime EffectivityDate)
        {
            #region query
            string query = string.Format(@"SELECT Twl_StartDate as 'Start Date'
                                                  ,Twl_EndDate as 'End Date'
                                                  ,Twl_WorkLocationCode as 'Location Code'
                                                  ,Location.Mcd_Name as 'Description'
                                                  ,Twl_ReasonCode
                                                  ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpWorkLocation
                                          LEFT JOIN M_CodeDtl Location
                                            ON Location.Mcd_Code = Twl_WorkLocationCode
                                            AND Location.Mcd_CodeType = 'ZIPCODE'
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Twl_ReasonCode
                                            AND Reason.Mcd_CodeType = 'LOCMVE'
                                          WHERE Twl_IDNo = '{0}'
                                            AND Twl_StartDate = '{1}'
                                          ORDER BY Twl_StartDate DESC", EmployeeId, EffectivityDate);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertWorkLocation(string EmployeeId
                                        , object EffectivityDate
                                        , object EndDate
                                        , string LocationCode
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@LocationCode", LocationCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpWorkLocation
                                                   (Twl_IDNo
                                                   ,Twl_StartDate
                                                   ,Twl_EndDate
                                                   ,Twl_WorkLocationCode
                                                   ,Twl_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@EffectivityDate
                                                   ,@EndDate
                                                   ,@LocationCode
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE())");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateWorkLocation(string EmployeeId
                                        , object EffectivityDate
                                        , object EndDate
                                        , string LocationCode
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@LocationCode", LocationCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpWorkLocation
                                               SET Twl_EndDate = @EndDate
                                                  ,Twl_WorkLocationCode = @LocationCode
                                                  ,Twl_ReasonCode = @MovementReason
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Twl_IDNo = @EmployeeId
                                             AND Twl_StartDate = @EffectivityDate");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectWorkLocationEffectivity(string EmployeeId
                                                    , string Usr_Login
                                                    , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpWorkLocation
                                        SET Twl_StartDate = CORRECTED.NewStartDate
	                                        , Twl_EndDate = CORRECTED.NewEndDate
                                            , Usr_Login = @Usr_Login
                                            , Ludatetime = GETDATE()
                                        FROM T_EmpWorkLocation ORIG
                                        INNER JOIN (
	                                        SELECT A.Twl_IDNo AS EmployeeID
		                                        , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Twl_StartDate)) AS NewStartDate
		                                        , (
				                                        SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Twl_StartDate)) - 1 
				                                        FROM T_EmpWorkLocation B
				                                        WHERE A.Twl_IDNo = B.Twl_IDNo
				                                        AND B.Twl_StartDate > A.Twl_StartDate
			                                        ) AS NewEndDate
	                                        FROM T_EmpWorkLocation A
                                        ) CORRECTED
                                        ON ORIG.Twl_IDNo = CORRECTED.EmployeeID
	                                        AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Twl_StartDate)) = CORRECTED.NewStartDate
                                        WHERE ORIG.Twl_StartDate != CORRECTED.NewStartDate
	                                        OR ORIG.Twl_EndDate != CORRECTED.NewEndDate
	                                        OR (ORIG.Twl_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                        OR (ORIG.Twl_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                            AND ORIG.Twl_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestWorkLocation(string EmployeeId
                                            , object EffectivityDate
                                            , string Usr_Login
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpWorkLocation
                                               SET Twl_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                  --,Usr_Login = @Usr_Login
                                                  --,Ludatetime = GETDATE()
                                             WHERE Twl_IDNo = @EmployeeId
                                             AND Twl_StartDate = (SELECT MAX(WorkLoc.Twl_StartDate)
							                                            FROM T_EmpWorkLocation WorkLoc
							                                            WHERE Twl_IDNo = @EmployeeId)");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermWorkLocation(string EmployeeId, DateTime EffectivityDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"DELETE FROM T_EmpWorkLocation
                                           WHERE Twl_IDNo = '{0}'
                                            AND Twl_StartDate = '{1}'

                                            Declare @Enddate as datetime
                                                Set @Enddate = (select top(1)Twl_EndDate from T_EmpWorkLocation
                                                Where Twl_IDNo = '{0}'
                                                order by Twl_StartDate desc)

                                            Update TOP(1) 
                                                T_EmpWorkLocation
                                                set Twl_EndDate = null
                                                Where Twl_IDNo = '{0}'
                                                 and Twl_EndDate = @Enddate  
                                        ", EmployeeId, EffectivityDate);
            #endregion
            dal.ExecuteNonQuery(query);
        }

        public void CorrectLogLedgerLocation(string EmployeeId
                                            , string Usr_Login
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpTimeRegister
                                            SET Ttr_WorkLocationCode = ISNULL((Select TOP(1) Twl_WorkLocationCode 
                                                                    From T_EmpWorkLocation
                                                                        Where Twl_IDNo = Ttr_IDNo 
                                                                         And Twl_StartDate <= Ttr_Date
                                                                         And Ttr_Date <= ISNULL(Twl_EndDate, Ttr_Date)
                                                                         Order By Twl_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            FROM T_EmpTimeRegister
                                            WHERE Ttr_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateEmployeeMasterLocation(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_WorkLocationCode = ISNULL((
                                                                    Select TOP(1) Twl_WorkLocationCode 
                                                                    From T_EmpWorkLocation
                                                                    Where Twl_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Twl_StartDate 
	                                                                    And ISNULL(Twl_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Twl_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public DataTable GetLatestWorkLocationMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"Select TOP(1) * 
                                           From T_EmpWorkLocation
                                           Where Twl_IDNo = '{0}'
                                           Order By Twl_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        #endregion

        #region Cost Center Movement
        public DataTable GetEmployeeCostCenterMovement(string IDNumber, string CompanyCode, string CentralProfile, string CCTRDSPLY)
        {
            #region query
            string query = string.Format(@"SELECT Tcc_StartDate as 'Start Date'
                                                  ,Tcc_EndDate as 'End Date'
                                                  ,Tcc_CostCenterCode as 'Cost Center'
                                                  ,dbo.Udf_DisplayCostCenterName('{1}',Tcc_CostCenterCode,'{2}') as 'Description'
                                                  ,Reason.Mcd_Name as 'Reason'
												  ,Tcc_Remarks as 'Remarks'
												  ,Tcc_IsLateEntry as 'Late Entry'
                                                  ,Tcc_PayCycle as 'Pay Cycle'
                                          FROM T_EmpCostcenter
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tcc_ReasonCode
                                            AND Reason.Mcd_CodeType = 'CCTRMVE'
                                            AND Reason.Mcd_CompanyCode = '{1}'
                                          WHERE Tcc_IDNo = '{0}'
                                          ORDER BY Tcc_StartDate DESC", IDNumber, CompanyCode, CCTRDSPLY);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeCostCenterMovement(string SystemID, string user_Logged, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tcc_IDNo as 'ID Number'
	                                              ,Mem_LastName as 'Last Name'
	                                              ,Mem_FirstName as 'First Name'
                                                  ,Tcc_StartDate as 'Effective Date'
                                                  ,Tcc_CostCenterCode as 'Cost Center Code'
                                                  ,{0}.dbo.Udf_DisplayCostCenterName('{1}',Tcc_CostCenterCode,'{2}') as 'Cost Center'
                                                  ,Tcc_ReasonCode as 'Reason Code'
                                                  ,Reason.Mcd_Name as 'Reason'
                                                  ,Tcc_Remarks as 'Remarks'
                                                  ,Tcc_IsLateEntry as 'Late Entry'
                                          FROM {0}..T_EmpCostcenter
                                          INNER JOIN M_Employee
                                            ON Tcc_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
                                            @ACCESSRIGHTS
                                          LEFT JOIN {0}..M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tcc_ReasonCode
                                            AND Reason.Mcd_CodeType = 'CCTRMVE'
                                            AND Reason.Mcd_CompanyCode = '{0}'
                                          WHERE Tcc_IDNo + Convert(Char(10),Tcc_StartDate,112) in ( SELECT Tcc_IDNo + Convert(Char(10), Tcc_StartDate, 112)
																                                 FROM (SELECT Tcc_IDNo , Max(Tcc_StartDate) as Tcc_StartDate
																			                            FROM {0}..T_EmpCostcenter
																			                            WHERE @EndNewCycle >= Tcc_StartDate
																					                        AND ISNULL(Tcc_EndDate, @StartNewCycle) >= @StartNewCycle
																			                            GROUP BY Tcc_IDNo ) Temp) 
                                          ORDER BY Mem_LastName, Mem_FirstName", CentralProfile, CompanyCode, new CommonBL().GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(CentralProfile, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, ProfileCode, false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }


        public bool CanViewRate(string UsrLogin, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mup_CanViewSalary FROM M_UserProfile WHERE Mup_UserCode = '{0}' AND Mup_CompanyCode = '{1}' AND Mup_ProfileCode = '{2}'"
                                            , UsrLogin, CompanyCode, ProfileCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dtResult.Rows[0][0]))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public DataRow FetchEmployeeCostCenterMovementRow(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT
	                                            Mem_CostcenterCode
	                                            , Mem_CostcenterDate
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'", EmployeeId);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeeCostCenterMovement(string EmployeeId, DateTime StartDate, string CompanyCode, string CentralProfile, string CCTRDSPLY)
        {
            #region query
            string query = string.Format(@"SELECT Tcc_StartDate as 'Start Date'
                                                  ,Tcc_EndDate as 'End Date'
                                                  ,Tcc_CostCenterCode as 'Cost Center'
                                                  ,dbo.Udf_DisplayCostCenterName('{2}', Tcc_CostCenterCode,'{3}') as 'Description'
                                                  ,Tcc_ReasonCode
                                                  ,Reason.Mcd_Name as 'Reason'
                                                  ,Tcc_Remarks as 'Remarks'
                                                  ,Tcc_IsLateEntry as 'Late Entry'
                                          FROM T_EmpCostcenter
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tcc_ReasonCode
                                            AND Reason.Mcd_CodeType = 'CCTRMVE'
                                            AND Reason.Mcd_CompanyCode = '{2}'
                                          WHERE Tcc_IDNo = '{0}'
                                            AND Tcc_StartDate = '{1}'
                                          ORDER BY Tcc_StartDate DESC", EmployeeId, StartDate, CompanyCode, CCTRDSPLY);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertCostCenter(string IDNumber
                                        , object StartDate
                                        , object EndDate
                                        , string CostCenter
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , string CompanyCode
                                        , string ProfileCode
                                        , string PayCycleCode
                                        , DateTime PayPeriodStart
                                        , DateTime PayPeriodEnd
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[11];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CostCenter", CostCenter);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileCode", ProfileCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IsLateEntry", (Convert.ToDateTime(StartDate) < PayPeriodStart ? 1 : 0));
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayCycle", PayCycleCode);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpCostcenter
                                                   (Tcc_IDNo
                                                   ,Tcc_StartDate
                                                   ,Tcc_EndDate
                                                   ,Tcc_CostCenterCode
                                                   ,Tcc_ReasonCode
                                                   ,Tcc_Remarks
                                                   ,Tcc_IsLateEntry
                                                   ,Tcc_PayCycle
                                                   ,Tcc_CompanyCode
                                                   ,Tcc_ProfileCode
                                                   ,Tcc_RecordStatus
                                                   ,Tcc_CreatedBy
                                                   ,Tcc_CreatedDate)
                                             VALUES
                                                   (@IDNumber
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@CostCenter
                                                   ,@MovementReason
                                                   ,@Remarks
                                                   ,@IsLateEntry
                                                   ,@PayCycle
                                                   ,@CompanyCode
                                                   ,@ProfileCode
                                                   ,'A'
                                                   ,@Usr_Login
                                                   ,GETDATE())");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectCostCenterInOtherTable(string IDNumber, object StartDate, string Usr_Login, string PayrollDBName, string CompanyCode, DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartCycle", StartDate);

            string query = string.Format(@"UPDATE {0}..T_EmpOvertime
                                                SET Tot_CostcenterCode = ISNULL((SELECT TOP(1) Tcc_CostCenterCode
								                                            FROM T_EmpCostcenter
								                                            WHERE Tcc_IDNo = Tot_IDNo
								                                            AND Tcc_StartDate <= Tot_OvertimeDate
								                                            AND Tot_OvertimeDate <= ISNULL(Tcc_EndDate, Tot_OvertimeDate)
								                                            ORDER BY Tcc_StartDate DESC), Tot_CostcenterCode)
                                                , Tot_UpdatedBy = @Usr_Login
                                                , Tot_UpdatedDate = GETDATE()
                                            WHERE Tot_IDNo = @IDNumber

                                           
                                            UPDATE {0}..T_EmpLeave
                                                SET Tlv_CostcenterCode = ISNULL((SELECT TOP(1) Tcc_CostCenterCode
								                                            FROM T_EmpCostcenter
								                                            WHERE Tcc_IDNo = Tlv_IDNo
								                                            AND Tcc_StartDate <= Tlv_LeaveDate
								                                            AND Tlv_LeaveDate <= ISNULL(Tcc_EndDate, Tlv_LeaveDate)
								                                            ORDER BY Tcc_StartDate DESC), Tlv_CostcenterCode) 
                                                , Tlv_UpdatedBy = @Usr_Login
                                                , Tlv_UpdatedDate = GETDATE()
                                            WHERE Tlv_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeCorrection
                                                SET Ttm_CostcenterCode = ISNULL((SELECT TOP(1) Tcc_CostCenterCode
								                                            FROM T_EmpCostcenter
								                                            WHERE Tcc_IDNo = Ttm_IDNo
								                                            AND Tcc_StartDate <= Ttm_TimeCorDate
								                                            AND Ttm_TimeCorDate <= ISNULL(Tcc_EndDate, Ttm_TimeCorDate)
								                                            ORDER BY Tcc_StartDate DESC), Ttm_CostcenterCode) 
                                                , Ttm_UpdatedBy = @Usr_Login
                                                , Ttm_UpdatedDate = GETDATE()
                                            WHERE Ttm_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpShift
                                                SET Tes_CostcenterCode = ISNULL((SELECT TOP(1) Tcc_CostCenterCode
								                                            FROM T_EmpCostcenter
								                                            WHERE Tcc_IDNo = Tes_IDNo
								                                            AND Tcc_StartDate <= Tes_ShiftDate
								                                            AND Tes_ShiftDate <= ISNULL(Tcc_EndDate, Tes_ShiftDate)
								                                            ORDER BY Tcc_StartDate DESC), Tes_CostcenterCode) 
                                                , Tes_UpdatedBy = @Usr_Login
                                                , Tes_UpdatedDate = GETDATE()
                                            WHERE Tes_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpInfo
                                                SET Tei_CostcenterCode = ISNULL((SELECT TOP(1) Tcc_CostCenterCode
								                                            FROM T_EmpCostcenter
								                                            WHERE Tcc_IDNo = Tei_IDNo
								                                            AND Tcc_StartDate <= Tei_RequestDate
								                                            AND Tei_RequestDate <= ISNULL(Tcc_EndDate, Tei_RequestDate)
								                                            ORDER BY Tcc_StartDate DESC), Tei_CostcenterCode) 
                                                , Tei_UpdatedBy = @Usr_Login
                                                , Tei_UpdatedDate = GETDATE()
                                            WHERE Tei_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeRegister
                                                SET Ttr_CostcenterCode = ISNULL((SELECT TOP(1) Tcc_CostCenterCode
								                                            FROM T_EmpCostcenter
								                                            WHERE Tcc_IDNo = Ttr_IDNo
								                                            AND Tcc_StartDate <= Ttr_Date
								                                            AND Ttr_Date <= ISNULL(Tcc_EndDate, Ttr_Date)
								                                            ORDER BY Tcc_StartDate DESC), Ttr_CostcenterCode) 
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                            WHERE Ttr_IDNo = @IDNumber 

                                           /*
                                           DECLARE @EndCycle datetime = (SELECT Tps_EndCycle FROM {0}..T_PaySchedule WHERE Tps_Paycycle = '')

                                           UPDATE {0}..T_EmpOvertime
                                                SET Tot_CostcenterCode = Tcc_Costcentercode
                                                , Tot_UpdatedBy = @Usr_Login
                                                , Tot_UpdatedDate = GETDATE()
                                            FROM {0}..T_EmpOvertime
                                            INNER JOIN T_EmpCostcenter ON Tcc_IDNo = Tot_IDNo	
	                                        WHERE Tot_OvertimeDate BETWEEN Tcc_StartDate AND ISNULL(Tcc_EndDate, CASE WHEN Tcc_StartDate >= @EndCycle THEN Tcc_StartDate ELSE 
		                                            CASE WHEN Tcc_EndDate IS NULL THEN @EndCycle ELSE Tcc_EndDate END 
                                                    END) --- >= @StartCycle and @EndCycle >= Tcc_StartDate
                                                AND Tot_CostcenterCode <> Tcc_Costcentercode
                                                AND Tot_IDNo = @IDNumber

                                            UPDATE {0}..T_EmpLeave
                                                SET Tlv_CostcenterCode = Tcc_Costcentercode
                                                , Tlv_UpdatedBy = @Usr_Login
                                                , Tlv_UpdatedDate = GETDATE()
                                            FROM {0}..T_EmpLeave
                                            INNER JOIN T_EmpCostcenter ON Tcc_IDNo = Tlv_IDNo	
	                                        WHERE Tlv_LeaveDate BETWEEN Tcc_StartDate AND ISNULL(Tcc_EndDate, CASE WHEN Tcc_StartDate >= @EndCycle THEN Tcc_StartDate ELSE 
		                                            CASE WHEN Tcc_EndDate IS NULL THEN @EndCycle ELSE Tcc_EndDate END 
                                                    END) --- >= @StartCycle and @EndCycle >= Tcc_StartDate
                                                AND Tlv_CostcenterCode <> Tcc_Costcentercode
                                                AND Tlv_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeCorrection
                                                SET Ttm_CostcenterCode = Tcc_Costcentercode
                                                , Ttm_UpdatedBy = @Usr_Login
                                                , Ttm_UpdatedDate = GETDATE()
                                            FROM {0}..T_EmpTimeCorrection
                                            INNER JOIN T_EmpCostcenter ON Tcc_IDNo = Ttm_IDNo	
	                                        WHERE Ttm_TimeCorDate BETWEEN Tcc_StartDate  AND ISNULL(Tcc_EndDate, CASE WHEN Tcc_StartDate >= @EndCycle THEN Tcc_StartDate ELSE 
		                                            CASE WHEN Tcc_EndDate IS NULL THEN @EndCycle ELSE Tcc_EndDate END 
                                                    END) --- >= @StartCycle and @EndCycle >= Tcc_StartDate
                                                AND Ttm_CostcenterCode <> Tcc_Costcentercode
                                                AND Ttm_IDNo = @IDNumber 

                                             UPDATE {0}..T_EmpTimeRegister
                                                SET Ttr_CostcenterCode = Tcc_Costcentercode 
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                             FROM {0}..T_EmpTimeRegister
                                             INNER JOIN T_EmpCostcenter ON Tcc_IDNo = Ttr_IDNo	
	                                         WHERE Ttr_Date BETWEEN Tcc_StartDate AND ISNULL(Tcc_EndDate, CASE WHEN Tcc_StartDate >= @EndCycle THEN Tcc_StartDate ELSE 
		                                            CASE WHEN Tcc_EndDate IS NULL THEN @EndCycle ELSE Tcc_EndDate END 
                                                    END) --- >= @StartCycle and @EndCycle >= Tcc_StartDate
                                                AND Ttr_CostcenterCode <> Tcc_Costcentercode
                                                AND Ttr_IDNo = @IDNumber 
                                            */
                                            ", PayrollDBName);

            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateCostCenterAccess(string EmployeeID
            , string SystemID
            , DateTime PayPeriodStart
            , DateTime PayPeriodEnd
            , string SeqNo
            , string Usr_Login
            , DALHelper dal)
        {
            
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@SystemID", SystemID);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@SeqNo", SeqNo);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            string query = @"
DECLARE @COUNTHAVINGALL int
DECLARE @COUNTNOALL int

SET @COUNTNOALL = (SELECT COUNT(*) 
FROM M_UserExt
WHERE Mue_UserCode = @EmployeeID
AND Mue_SystemCode = @SystemID
)

SET @COUNTHAVINGALL = (SELECT COUNT(*) 
FROM M_UserExt
WHERE Mue_UserCode = @EmployeeID
AND Mue_SystemCode = @SystemID
AND Mue_CostCenterCode = 'ALL')

IF(@COUNTNOALL = 1 AND @COUNTHAVINGALL = 0)
BEGIN
    UPDATE M_UserExt
    SET Mue_CostCenterCode = ISNULL((
                            SELECT TOP(1) Tcc_CostCenterCode 
                            FROM T_EmpCostcenter
                            WHERE Tcc_IDNo = @EmployeeID 
	                            AND @PayPeriodEnd >= Tcc_StartDate 
	                            AND ISNULL(Tcc_EndDate, @PayPeriodStart) >= @PayPeriodStart
                            ORDER BY Tcc_StartDate DESC), Mue_CostCenterCode)
    , Mue_UpdatedBy = @Usr_Login
    , Mue_UpdatedDate = getdate()
    WHERE Mue_UserCode = @EmployeeID

    INSERT M_UserExtTrl
    SELECT 
        Mue_SystemCode
        , Mue_UserCode
        , @SeqNo
        , Mue_CostCenterCode
        , Mue_RankCode
        , Mue_EmploymentStatus
        , Mue_RecordStatus
        , Usr_Login
        , Ludatetime 
    FROM M_UserExt
    WHERE Mue_UserCode = @EmployeeID
    AND Mue_SystemCode = @SystemID
END
";

            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateCostCenter(string IDNumber
                                        , object StartDate
                                        , object EndDate
                                        , string CostCenter
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CostCenter", CostCenter);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpCostcenter
                                               SET Tcc_EndDate = @EndDate
                                                  ,Tcc_CostCenterCode = @CostCenter
                                                  ,Tcc_ReasonCode = @MovementReason
                                                  ,Tcc_Remarks = @Remarks
                                                  ,Tcc_UpdatedBy = @Usr_Login
                                                  ,Tcc_UpdatedDate = GETDATE()
                                             WHERE Tcc_IDNo = @IDNumber
                                             AND Tcc_StartDate = @StartDate");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectCostCenterEffectivity(string EmployeeId
                                        , string Usr_Login
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpCostcenter
                                            SET Tcc_StartDate = CORRECTED.NewStartDate
	                                            , Tcc_EndDate = CORRECTED.NewEndDate
                                                , Tcc_UpdatedBy = @Usr_Login
                                                , Tcc_UpdatedDate = GETDATE()
                                            FROM T_EmpCostcenter ORIG
                                            INNER JOIN (
	                                            SELECT A.Tcc_IDNo AS EmployeeID
		                                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Tcc_StartDate)) AS NewStartDate
		                                            , (
				                                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Tcc_StartDate)) - 1 
				                                            FROM T_EmpCostcenter B
				                                            WHERE A.Tcc_IDNo = B.Tcc_IDNo
				                                            AND B.Tcc_StartDate > A.Tcc_StartDate
			                                            ) AS NewEndDate
	                                            FROM T_EmpCostcenter A
                                            ) CORRECTED
                                            ON ORIG.Tcc_IDNo = CORRECTED.EmployeeID
	                                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Tcc_StartDate)) = CORRECTED.NewStartDate
                                            WHERE ORIG.Tcc_StartDate != CORRECTED.NewStartDate
	                                            OR ORIG.Tcc_EndDate != CORRECTED.NewEndDate
	                                            OR (ORIG.Tcc_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                            OR (ORIG.Tcc_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                                AND ORIG.Tcc_IDNo = @EmployeeId");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestCostCenter(string EmployeeId
                                            , object StartDate
                                            , string Usr_Login
                                            , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpCostcenter
                                           SET Tcc_EndDate = dateadd(dd, -1, @StartDate)
                                                  ,Tcc_UpdatedBy = @Usr_Login
                                                  ,Tcc_UpdatedDate = GETDATE()
                                           WHERE Tcc_IDNo = @EmployeeId
                                                AND Tcc_StartDate = (SELECT MAX(CostCentr.Tcc_StartDate)
					                                                FROM T_EmpCostcenter CostCentr
					                                                WHERE Tcc_IDNo = @EmployeeId)");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermCostCenter(string EmployeeId, DateTime StartDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"  

                                           DELETE FROM T_EmpCostcenter
                                           WHERE Tcc_IDNo = '{0}'
                                            AND Tcc_StartDate = '{1}'

                                            Declare @Enddate as datetime
                                                Set @Enddate = (select top(1)Tcc_EndDate from T_EmpCostcenter
                                                Where Tcc_IDNo = '{0}'
                                                order by Tcc_StartDate desc)

                                           Update TOP(1) 
                                                T_EmpCostcenter
                                                set Tcc_EndDate = null
                                                Where Tcc_IDNo = '{0}'
                                                and Tcc_EndDate = @Enddate ", EmployeeId, StartDate);
            #endregion
            dal.ExecuteNonQuery(query);
        }

        public void UpdateEmployeeMasterCostCenter(string IDNumber
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , string PayrollDBName
                                                , string CompanyCode
                                                , string CCTRDSPLY
                                                , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            string dtrDB = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());

            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_CostcenterDate = ISNULL((
                                                                    SELECT TOP(1) Tcc_StartDate 
                                                                    FROM T_EmpCostcenter
                                                                    WHERE Tcc_IDNo = @IDNumber 
	                                                                    AND @PayPeriodEnd >= Tcc_StartDate 
	                                                                    AND ISNULL(Tcc_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tcc_StartDate DESC), Mem_CostcenterDate)
                                                , Mem_CostcenterCode = ISNULL((
                                                                    SELECT TOP(1) Tcc_CostCenterCode 
                                                                    FROM T_EmpCostcenter
                                                                    WHERE Tcc_IDNo = @IDNumber 
	                                                                    AND @PayPeriodEnd >= Tcc_StartDate 
	                                                                    AND ISNULL(Tcc_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tcc_StartDate DESC), Mem_CostcenterCode)
                                                , Mem_UpdatedBy = @Usr_Login
                                                , Mem_UpdatedDate = GETDATE()
                                            WHERE Mem_IDNo = @IDNumber");

            string queryPayroll = string.Format(@"UPDATE {0}..M_Employee
                                               SET Mem_CostcenterDate = ISNULL((
                                                                    SELECT TOP(1) Tcc_StartDate 
                                                                    FROM T_EmpCostcenter
                                                                    WHERE Tcc_IDNo = @IDNumber 
	                                                                    AND @PayPeriodEnd >= Tcc_StartDate 
	                                                                    AND ISNULL(Tcc_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tcc_StartDate DESC), Mem_CostcenterDate)
                                            , Mem_CostcenterCode = ISNULL((
                                                                    SELECT TOP(1) Tcc_CostCenterCode 
                                                                    FROM T_EmpCostcenter
                                                                    WHERE Tcc_IDNo = @IDNumber 
	                                                                    AND @PayPeriodEnd >= Tcc_StartDate 
	                                                                    AND ISNULL(Tcc_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tcc_StartDate DESC), Mem_CostcenterCode)
                                                , Mem_UpdatedBy = @Usr_Login
                                                , Mem_UpdatedDate = GETDATE()
                                            WHERE Mem_IDNo = @IDNumber", PayrollDBName);

            string queryDTR = string.Format(@"UPDATE {0}..T_EmpLog
                                            SET Tel_CostcenterName = ISNULL(dbo.Udf_DisplayCostCenterName('{1}',Mem_CostcenterCode, '{2}'), Tel_CostcenterName)
                                                , Tel_UpdatedBy = @Usr_Login
                                                , Tel_UpdatedDate = GETDATE()
                                            FROM {0}..T_EmpLog
                                            INNER JOIN M_Employee ON Mem_IDNo = Tel_IDNo
                                            WHERE Tel_IDNo = @IDNumber", dtrDB, CompanyCode, CCTRDSPLY);
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            //Update Payroll Employee Master
            dalCentral.ExecuteNonQuery(queryPayroll, CommandType.Text, paramInfo);

            //Update Log Master
            dalCentral.ExecuteNonQuery(queryDTR, CommandType.Text, paramInfo);

            //string SeqNo = GetNextCostCenterAccessTrailSeqNo(IDNumber);
            //UpdateCostCenterAccess(IDNumber, "GENERAL", PayPeriodStart, PayPeriodEnd, SeqNo, Usr_Login, dal);
            //UpdateCostCenterAccess(IDNumber, "LEAVE", PayPeriodStart, PayPeriodEnd, SeqNo, Usr_Login, dal);
            //UpdateCostCenterAccess(IDNumber, "OVERTIME", PayPeriodStart, PayPeriodEnd, SeqNo, Usr_Login, dal);
            //UpdateCostCenterAccess(IDNumber, "PAYROLL", PayPeriodStart, PayPeriodEnd, SeqNo, Usr_Login, dal);
            //UpdateCostCenterAccess(IDNumber, "PERSONNEL", PayPeriodStart, PayPeriodEnd, SeqNo, Usr_Login, dal);
            //UpdateCostCenterAccess(IDNumber, "TIMEKEEP", PayPeriodStart, PayPeriodEnd, SeqNo, Usr_Login, dal);
        }

        public DataTable GetLatestCostCenterMovement(string IDNumber, string CentralProfile)
        {
            #region query
            string query = string.Format(@"Select TOP(1) * 
                                           From T_EmpCostcenter
                                           Where Tcc_IDNo = '{0}'
                                           Order By Tcc_StartDate DESC", IDNumber);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        #endregion

        #region Position Movement
        public DataTable GetEmployeePositionMovement(string IDNumber, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Tpo_StartDate as 'Start Date'
                                              ,Tpo_EndDate as 'End Date'
                                              ,Position.Mcd_Name as 'Position'
                                              ,Grade.Mcd_Name as 'Grade'
                                              ,PositionLevel.Mcd_Name as 'Level'
                                              ,Reason.Mcd_Name as 'Reason'
											  ,Tpo_Remarks as 'Remarks'
											  ,Tpo_IsLateEntry as 'Late Entry'
                                              ,Tpo_PayCycle as 'Pay Cycle'
                                          FROM T_EmpPosition
                                          LEFT JOIN M_CodeDtl Position
                                            ON Position.Mcd_Code = Tpo_PositionCode
                                            AND Position.Mcd_CodeType = 'POSITION'
											AND Position.Mcd_CompanyCode = '{1}'
                                          LEFT JOIN M_CodeDtl Grade
                                            ON Grade.Mcd_Code = Tpo_PositionGrade
                                            AND Grade.Mcd_CodeType = 'POSITIONGRADE'
											AND Grade.Mcd_CompanyCode = '{1}'
                                          LEFT JOIN M_CodeDtl PositionLevel
                                            ON PositionLevel.Mcd_Code = Tpo_PositionLevel
                                            AND PositionLevel.Mcd_CodeType = 'POSITIONLEVEL'
											AND PositionLevel.Mcd_CompanyCode = '{1}'
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tpo_ReasonCode
                                            AND Reason.Mcd_CodeType = 'POSMVE'
                                            AND Reason.Mcd_CompanyCode = '{1}'
                                          WHERE Tpo_IDNo = '{0}'
                                          ORDER BY Tpo_StartDate DESC", IDNumber, CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeePositionMovement(string SystemID, string user_Logged, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tpo_IDNo as 'ID Number'
                                              ,Mem_LastName as 'Last Name'
                                              ,Mem_FirstName as 'First Name'
                                              ,Tpo_StartDate as 'Effective Date'
                                              ,Tpo_PositionCode as 'Position Code'
                                              ,Position.Mcd_Name as 'Position'
                                              ,Tpo_PositionGrade as 'Grade Code'
                                              ,Grade.Mcd_Name as 'Grade'
                                              ,Tpo_PositionLevel as 'Level Code'
                                              ,PositionLevel.Mcd_Name as 'Level'
                                              ,Tpo_ReasonCode as 'Reason Code'
                                              ,Reason.Mcd_Name as 'Reason'
                                              ,Tpo_Remarks as 'Remarks'
											  ,Tpo_IsLateEntry as 'Late Entry'
                                          FROM {0}..T_EmpPosition
                                          INNER JOIN M_Employee
                                            ON Tpo_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
                                            @ACCESSRIGHTS
                                          LEFT JOIN {0}..M_CodeDtl Position
                                            ON Position.Mcd_Code = Tpo_PositionCode
                                            AND Position.Mcd_CodeType = 'POSITION'
                                            AND Position.Mcd_CompanyCode = '{1}'
                                          LEFT JOIN {0}..M_CodeDtl Grade
                                            ON Grade.Mcd_Code = Tpo_PositionGrade
                                            AND Grade.Mcd_CodeType = 'POSITIONGRADE'
                                            AND Grade.Mcd_CompanyCode = '{1}'
                                          LEFT JOIN {0}..M_CodeDtl PositionLevel
                                            ON PositionLevel.Mcd_Code = Tpo_PositionLevel
                                            AND PositionLevel.Mcd_CodeType = 'POSITIONLEVEL'
                                            AND PositionLevel.Mcd_CompanyCode = '{1}'
                                          LEFT JOIN {0}..M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tpo_ReasonCode
                                            AND Reason.Mcd_CodeType = 'POSMVE'
                                            AND Reason.Mcd_CompanyCode = '{1}'
                                          WHERE Tpo_IDNo + Convert(Char(10),Tpo_StartDate,112) in ( SELECT Tpo_IDNo + Convert(Char(10), Tpo_StartDate, 112)
																                                 FROM (SELECT Tpo_IDNo , Max(Tpo_StartDate) as Tpo_StartDate
																			                            FROM {0}..T_EmpPosition
																			                            WHERE @EndNewCycle >= Tpo_StartDate
																					                        AND ISNULL(Tpo_EndDate, @StartNewCycle) >= @StartNewCycle
																			                            GROUP BY Tpo_IDNo ) Temp)
                                          ORDER BY Mem_LastName, Mem_FirstName", CentralProfile, CompanyCode);
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(CentralProfile, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, ProfileCode, false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeePositionMovementRow(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT
	                                        Mem_PositionLevel
	                                        , Mem_PositionCode
	                                        , Mem_PositionDate
                                            , Mem_PositionGrade
                                        FROM M_Employee
                                        WHERE Mem_IDNo = '{0}'", EmployeeId);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeePositionMovement(string IDNumber, DateTime StartDate, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Tpo_StartDate as 'Start Date'
                                              ,Tpo_EndDate as 'End Date'
                                              ,Tpo_PositionCode
                                              ,Position.Mcd_Name as 'Position'
                                              ,Tpo_PositionGrade
                                              ,Grade.Mcd_Name as 'Grade'
                                              ,Tpo_PositionLevel
                                              ,PositionLevel.Mcd_Name as 'Level'
                                              ,Tpo_ReasonCode
                                              ,Reason.Mcd_Name as 'Reason'
											  ,Tpo_Remarks as 'Remarks'
                                          FROM T_EmpPosition
                                          LEFT JOIN M_CodeDtl Position
                                            ON Position.Mcd_Code = Tpo_PositionCode
                                            AND Position.Mcd_CodeType = 'POSITION'
											AND Position.Mcd_CompanyCode = '{2}'
                                          LEFT JOIN M_CodeDtl Grade
                                            ON Grade.Mcd_Code = Tpo_PositionGrade
                                            AND Grade.Mcd_CodeType = 'POSITIONGRADE'
											AND Grade.Mcd_CompanyCode = '{2}'
                                          LEFT JOIN M_CodeDtl PositionLevel
                                            ON PositionLevel.Mcd_Code = Tpo_PositionLevel
                                            AND PositionLevel.Mcd_CodeType = 'POSITIONLEVEL'
											AND PositionLevel.Mcd_CompanyCode = '{2}'
                                          LEFT JOIN M_CodeDtl Reason
                                            ON Reason.Mcd_Code = Tpo_ReasonCode
                                            AND Reason.Mcd_CodeType = 'POSMVE'
											AND Reason.Mcd_CompanyCode = '{2}'
                                          WHERE Tpo_IDNo = '{0}'
                                            AND Tpo_StartDate = '{1}'
                                          ORDER BY Tpo_StartDate DESC", IDNumber, StartDate, CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertPosition(string EmployeeId
                                        , object StartDate
                                        , object EndDate
                                        , string Position
                                        , string Grade
                                        , string Level
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , string CompanyCode
                                        , string ProfileCode
                                        , string PayCycleCode
                                        , DateTime PayPeriodStart
                                        , DateTime PayPeriodEnd
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[13];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Position", Position);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Level", Level);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Grade", Grade);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IsLateEntry", (Convert.ToDateTime(StartDate) < PayPeriodStart ? 1 : 0));
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileCode", ProfileCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayCycleCode", PayCycleCode);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpPosition
                                                   (Tpo_IDNo
                                                   ,Tpo_StartDate
                                                   ,Tpo_EndDate
                                                   ,Tpo_PositionCode
                                                   ,Tpo_PositionGrade
                                                   ,Tpo_PositionLevel
                                                   ,Tpo_ReasonCode
                                                   ,Tpo_Remarks
                                                   ,Tpo_IsLateEntry
                                                   ,Tpo_PayCycle
                                                   ,Tpo_CompanyCode
                                                   ,Tpo_ProfileCode
                                                   ,Tpo_RecordStatus
                                                   ,Tpo_CreatedBy
                                                   ,Tpo_CreatedDate)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@Position
                                                   ,@Grade
                                                   ,@Level
                                                   ,@MovementReason
                                                   ,@Remarks
                                                   ,@IsLateEntry
                                                   ,@PayCycleCode
                                                   ,@CompanyCode
                                                   ,@ProfileCode
                                                   ,'A'
                                                   ,@Usr_Login
                                                   ,GETDATE())");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdatePosition(string IDNumber
                                        , object StartDate
                                        , object EndDate
                                        , string Position
                                        , string Grade
                                        , string Level
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[9];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Position", Position);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Grade", Grade);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Level", Level);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpPosition
                                               SET Tpo_EndDate = @EndDate
                                                  ,Tpo_PositionCode = @Position
                                                  ,Tpo_PositionGrade = @Grade
                                                  ,Tpo_PositionLevel = @Level
                                                  ,Tpo_ReasonCode = @MovementReason
                                                  ,Tpo_Remarks = @Remarks
                                                  ,Tpo_UpdatedBy = @Usr_Login
                                                  ,Tpo_UpdatedDate = GETDATE()
                                             WHERE Tpo_IDNo = @IDNumber
                                             AND Tpo_StartDate = @StartDate");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectPositionEffectivity(string EmployeeId
                                                    , string Usr_Login
                                                    , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpPosition
                                            SET Tpo_StartDate = CORRECTED.NewStartDate
	                                            , Tpo_EndDate = CORRECTED.NewEndDate
                                                , Tpo_UpdatedBy = @Usr_Login
                                                , Tpo_UpdatedDate = GETDATE()
                                            FROM T_EmpPosition ORIG
                                            INNER JOIN (
	                                            SELECT A.Tpo_IDNo AS EmployeeID
		                                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Tpo_StartDate)) AS NewStartDate
		                                            , (
				                                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Tpo_StartDate)) - 1 
				                                            FROM T_EmpPosition B
				                                            WHERE A.Tpo_IDNo = B.Tpo_IDNo
				                                            AND B.Tpo_StartDate > A.Tpo_StartDate
			                                            ) AS NewEndDate
	                                            FROM T_EmpPosition A
                                            ) CORRECTED
                                            ON ORIG.Tpo_IDNo = CORRECTED.EmployeeID
	                                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Tpo_StartDate)) = CORRECTED.NewStartDate
                                            WHERE ORIG.Tpo_StartDate != CORRECTED.NewStartDate
	                                            OR ORIG.Tpo_EndDate != CORRECTED.NewEndDate
	                                            OR (ORIG.Tpo_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                            OR (ORIG.Tpo_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                                AND ORIG.Tpo_IDNo = @EmployeeId");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestPosition(string EmployeeId
                                            , object StartDate
                                            , string Usr_Login
                                            , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpPosition
                                               SET Tpo_EndDate = dateadd(dd, -1, @StartDate)
                                                  ,Tpo_UpdatedBy = @Usr_Login
                                                  ,Tpo_UpdatedDate = GETDATE()
                                             WHERE Tpo_IDNo = @EmployeeId
                                             AND Tpo_StartDate = (SELECT MAX(Position.Tpo_StartDate)
					                                                FROM T_EmpPosition Position
					                                                WHERE Tpo_IDNo = @EmployeeId)");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermPosition(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"DELETE FROM T_EmpPosition
                                           WHERE Tpo_IDNo = '{0}'
                                            AND Tpo_StartDate = '{1}'
                                            
                                            Declare @Enddate as datetime
                                            Set @Enddate = (select top(1)Tpo_EndDate from T_EmpPosition
                                            Where Tpo_IDNo = '{0}'
                                            order by Tpo_StartDate desc)

                                        Update TOP(1) 
                                            T_EmpPosition
                                            set Tpo_EndDate = null
                                            Where Tpo_IDNo = '{0}'
                                             and Tpo_EndDate = @Enddate 
                                        ", EmployeeId, StartDate);
            #endregion
            dalCentral.ExecuteNonQuery(query);
        }

        public void CorrectPositionGradeInOtherTable(string IDNumber, object StartDate, string Usr_Login, string PayrollDBName, string CompanyCode, DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartCycle", StartDate);

            string query = string.Format(@"UPDATE {0}..T_EmpOvertime
                                                SET Tot_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = Tot_IDNo
								                                            AND Tpo_StartDate <= Tot_OvertimeDate
								                                            AND Tot_OvertimeDate <= ISNULL(Tpo_EndDate, Tot_OvertimeDate)
								                                            ORDER BY Tpo_StartDate DESC), Tot_Grade)
                                                , Tot_UpdatedBy = @Usr_Login
                                                , Tot_UpdatedDate = GETDATE()
                                            WHERE Tot_IDNo = @IDNumber

                                           
                                            UPDATE {0}..T_EmpLeave
                                                SET Tlv_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = Tlv_IDNo
								                                            AND Tpo_StartDate <= Tlv_LeaveDate
								                                            AND Tlv_LeaveDate <= ISNULL(Tpo_EndDate, Tlv_LeaveDate)
								                                            ORDER BY Tpo_StartDate DESC), Tlv_Grade) 
                                                , Tlv_UpdatedBy = @Usr_Login
                                                , Tlv_UpdatedDate = GETDATE()
                                            WHERE Tlv_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeCorrection
                                                SET Ttm_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = Ttm_IDNo
								                                            AND Tpo_StartDate <= Ttm_TimeCorDate
								                                            AND Ttm_TimeCorDate <= ISNULL(Tpo_EndDate, Ttm_TimeCorDate)
								                                            ORDER BY Tpo_StartDate DESC), Ttm_Grade) 
                                                , Ttm_UpdatedBy = @Usr_Login
                                                , Ttm_UpdatedDate = GETDATE()
                                            WHERE Ttm_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpShift
                                                SET Tes_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = Tes_IDNo
								                                            AND Tpo_StartDate <= Tes_ShiftDate
								                                            AND Tes_ShiftDate <= ISNULL(Tpo_EndDate, Tes_ShiftDate)
								                                            ORDER BY Tpo_StartDate DESC), Tes_Grade) 
                                                , Tes_UpdatedBy = @Usr_Login
                                                , Tes_UpdatedDate = GETDATE()
                                            WHERE Tes_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpInfo
                                                SET Tei_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = Tei_IDNo
								                                            AND Tpo_StartDate <= Tei_RequestDate
								                                            AND Tei_RequestDate <= ISNULL(Tpo_EndDate, Tei_RequestDate)
								                                            ORDER BY Tpo_StartDate DESC), Tei_Grade) 
                                                , Tei_UpdatedBy = @Usr_Login
                                                , Tei_UpdatedDate = GETDATE()
                                            WHERE Tei_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeRegister
                                                SET Ttr_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = Ttr_IDNo
								                                            AND Tpo_StartDate <= Ttr_Date
								                                            AND Ttr_Date <= ISNULL(Tpo_EndDate, Ttr_Date)
								                                            ORDER BY Tpo_StartDate DESC), Ttr_Grade) 
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                            WHERE Ttr_IDNo = @IDNumber 

                                            ", PayrollDBName);

            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }
        public void UpdateEmployeeMasterPosition(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , string CompanyCode
                                                , string PayrollDBName
                                                , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            string dtrDB = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());

            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_PositionDate = ISNULL((
                                                                    SELECT TOP(1) Tpo_StartDate 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionDate)
                                                , Mem_PositionCode = ISNULL((
                                                                    SELECT TOP(1) Tpo_PositionCode 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionCode)
                                                , Mem_PositionGrade = ISNULL((
                                                                    SELECT TOP(1) Tpo_PositionGrade 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionGrade)
                                                , Mem_PositionLevel = ISNULL((
                                                                    SELECT TOP(1) Tpo_PositionLevel 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionLevel)
                                                , Mem_UpdatedBy = @Usr_Login
                                                , Mem_UpdatedDate = GETDATE()
                                            WHERE Mem_IDNo = @EmployeeId");

            string queryPayroll = string.Format(@"UPDATE {0}..M_Employee
                                                SET Mem_PositionDate = ISNULL((
                                                                    SELECT TOP(1) Tpo_StartDate 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionDate)
                                                , Mem_PositionCode = ISNULL((
                                                                    SELECT TOP(1) Tpo_PositionCode 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionCode)
                                                , Mem_PositionGrade = ISNULL((
                                                                    SELECT TOP(1) Tpo_PositionGrade 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionGrade)
                                                , Mem_PositionLevel = ISNULL((
                                                                    SELECT TOP(1) Tpo_PositionLevel 
                                                                    FROM T_EmpPosition
                                                                    WHERE Tpo_IDNo = @EmployeeId 
	                                                                    AND @PayPeriodEnd >= Tpo_StartDate 
	                                                                    AND ISNULL(Tpo_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    ORDER BY Tpo_StartDate DESC), Mem_PositionLevel)
                                                , Mem_UpdatedBy = @Usr_Login
                                                , Mem_UpdatedDate = GETDATE()
                                                WHERE Mem_IDNo = @EmployeeId", PayrollDBName);

            string queryTimeRegister = string.Format(@" UPDATE {0}..T_EmpTimeRegister
                                                        SET Ttr_Grade = ISNULL((SELECT TOP(1) Tpo_PositionGrade
								                                            FROM T_EmpPosition
								                                            WHERE Tpo_IDNo = @EmployeeId
								                                            AND Tpo_StartDate <= Ttr_Date
								                                            AND Ttr_Date <= ISNULL(Tpo_EndDate, Ttr_Date)
								                                            ORDER BY Tpo_StartDate DESC), Ttr_Grade) 
                                                            , Usr_Login = @Usr_Login
                                                            , Ludatetime = GETDATE()
                                                        WHERE Ttr_IDNo = @EmployeeId", PayrollDBName);


            string queryDTR = string.Format(@"UPDATE {0}..T_EmpLog
                                            SET Tel_PositionName = ISNULL(POSITION.Mcd_Name, Tel_PositionName)
                                                , Tel_UpdatedBy = @Usr_Login
                                                , Tel_UpdatedDate = GETDATE()
                                            FROM {0}..T_EmpLog
											INNER JOIN M_Employee ON Mem_IDNo = Tel_IDNo
											LEFT JOIN M_CodeDtl POSITION ON POSITION.Mcd_Code = Mem_PositionCode
												AND POSITION.Mcd_CodeType = 'POSITION'
												AND POSITION.Mcd_CompanyCode = '{1}'
                                            WHERE Tel_IDNo = @EmployeeId", dtrDB, CompanyCode);



            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            //Update Payroll Employee Master
            dalCentral.ExecuteNonQuery(queryPayroll, CommandType.Text, paramInfo);
            dalCentral.ExecuteNonQuery(queryTimeRegister, CommandType.Text, paramInfo);

            //Update Log Master
            dalCentral.ExecuteNonQuery(queryDTR, CommandType.Text, paramInfo);
        }

        public DataTable GetLatestPositionMovement(string IDNumber, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT TOP(1) * 
                                           FROM T_EmpPosition
                                           WHERE Tpo_IDNo = '{0}'
                                           ORDER BY Tpo_StartDate DESC", IDNumber);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        #endregion

        #region Salary Movement
        public DataTable GetEmployeeSalaryMovement(string IDNumber, string UsrLogin, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"
                                        DECLARE @CanViewRate bit

                                        SET @CanViewRate = (SELECT Mup_CanViewSalary FROM M_UserProfile WHERE Mup_UserCode = '{1}' AND Mup_CompanyCode = '{2}' AND Mup_ProfileCode = '{3}')

                                        SELECT CONVERT(CHAR(10),Tsl_StartDate,101) as 'Start Date'
                                              ,Tsl_EndDate as 'End Date'
                                              , CASE @CanViewRate
                                                    WHEN 1 THEN CAST(Tsl_SalaryRate as VARCHAR(20))
                                                    ELSE '************'
                                                END as 'Salary Rate'
                                              ,PayrollType.Mcd_Name as 'Payroll Type'
                                              ,Reason.Mcd_Name as 'Reason'
                                              ,Tsl_Remarks as 'Remarks'
                                              ,Tsl_IsLateEntry as 'Late Entry'
                                              ,Tsl_PayCycle as 'Pay Cycle'
                                          FROM T_EmpSalary
									      LEFT JOIN M_CodeDtl PayrollType
											ON PayrollType.Mcd_Code = Tsl_PayrollType
											AND PayrollType.Mcd_CodeType = 'PAYTYPE'
                                            AND PayrollType.Mcd_CompanyCode = '{2}'
										  LEFT JOIN M_CodeDtl Reason
											ON Reason.Mcd_Code = Tsl_ReasonCode
											AND Reason.Mcd_CodeType = 'SALARYMVE'
                                            AND Reason.Mcd_CompanyCode = '{2}'
                                          WHERE Tsl_IDNo = '{0}'
                                          ORDER BY Tsl_StartDate DESC", IDNumber, UsrLogin,  CompanyCode, ProfileCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetSalaryMovement(string SystemID, string UsrLogin, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
	                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tsl_IDNo as 'ID Number'
                                              ,Mem_LastName as 'Last Name'
                                              ,Mem_FirstName as 'First Name'
                                              ,Tsl_StartDate as 'Effective Date'
                                              ,Tsl_SalaryRate as 'Salary Rate'
                                              ,Tsl_PayrollType as 'Payroll Type Code'
                                              ,PayrollType.Mcd_Name as 'Payroll Type'
                                              ,Tsl_ReasonCode as 'Reason Code'
                                              ,Reason.Mcd_Name as 'Reason'
                                              ,Tsl_Remarks as 'Remarks'
                                              ,Tsl_IsLateEntry as 'Late Entry'
                                          FROM {0}..T_EmpSalary
                                          INNER JOIN M_Employee
                                            ON Tsl_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
                                          @ACCESSRIGHTS
									      LEFT JOIN {0}..M_CodeDtl PayrollType
											ON PayrollType.Mcd_Code = Tsl_PayrollType
											AND PayrollType.Mcd_CodeType = 'PAYTYPE'
                                            AND PayrollType.Mcd_CompanyCode = '{1}'
										  LEFT JOIN {0}..M_CodeDtl Reason
											ON Reason.Mcd_Code = Tsl_ReasonCode
											AND Reason.Mcd_CodeType = 'SALARYMVE'
                                            AND Reason.Mcd_CompanyCode = '{1}'
                                          WHERE Tsl_IDNo + Convert(Char(10),Tsl_StartDate,112) in (
	                                            SELECT Tsl_IDNo + Convert(Char(10), Tsl_StartDate, 112)
	                                             FROM (SELECT Tsl_IDNo , Max(Tsl_StartDate) as Tsl_StartDate
			                                            FROM {0}..T_EmpSalary
			                                            WHERE @EndNewCycle >= Tsl_StartDate
				                                            AND ISNULL(Tsl_EndDate, @StartNewCycle) >= @StartNewCycle
			                                            GROUP BY Tsl_IDNo ) Temp
                                            )
                                          ORDER BY Mem_LastName, Mem_FirstName", CentralProfile, CompanyCode);
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(CentralProfile, SystemID, UsrLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, ProfileCode, false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeeSalaryMovementRow(string IDNumber)
        {
            #region query
            string query = string.Format(@"SELECT
	                                            Mem_SalaryDate
	                                            , Mem_Salary
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'", IDNumber);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeeSalaryMovement(string IDNumber, DateTime StartDate, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Tsl_StartDate as 'Start Date'
                                              ,Tsl_EndDate as 'End Date'
                                              ,Tsl_SalaryRate as 'Salary Rate'
                                              ,Tsl_PayrollType
                                              ,PayrollType.Mcd_Name as 'Payroll Type'
                                              ,Reason.Mcd_Name as 'Reason'
                                              ,Tsl_ReasonCode
                                              ,Tsl_Remarks as 'Remarks'
                                          FROM T_EmpSalary
									      LEFT JOIN M_CodeDtl PayrollType
											ON PayrollType.Mcd_Code = Tsl_PayrollType
											AND PayrollType.Mcd_CodeType = 'PAYTYPE'
                                            AND PayrollType.Mcd_CompanyCode = '{2}'
										  LEFT JOIN M_CodeDtl Reason
											ON Reason.Mcd_Code = Tsl_ReasonCode
											AND Reason.Mcd_CodeType = 'SALARYMVE'
                                            AND Reason.Mcd_CompanyCode = '{2}'
                                          WHERE Tsl_IDNo = '{0}'
                                            AND Tsl_StartDate = '{1}'
                                          ORDER BY Tsl_StartDate DESC", IDNumber, StartDate, CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertSalary(string IDNumber
                                        , object StartDate
                                        , object EndDate
                                        , string Salary
                                        , string PayrollType
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , string CompanyCode
                                        , string ProfileCode
                                        , string PayCycleCode
                                        , DateTime PayPeriodStart
                                        , DateTime PayPeriodEnd
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[12];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Salary", Salary);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayrollType", PayrollType);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileCode", ProfileCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IsLateEntry", (Convert.ToDateTime(StartDate) < PayPeriodStart ? 1 : 0));
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayCycleCode", PayCycleCode);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpSalary
                                                   (Tsl_IDNo
                                                   ,Tsl_StartDate
                                                   ,Tsl_EndDate
                                                   ,Tsl_SalaryRate
                                                   ,Tsl_PayrollType
                                                   ,Tsl_ReasonCode
                                                   ,Tsl_Remarks
                                                   ,Tsl_IsLateEntry
                                                   ,Tsl_PayCycle
                                                   ,Tsl_CompanyCode
                                                   ,Tsl_ProfileCode
                                                   ,Tsl_RecordStatus
                                                   ,Tsl_CreatedBy
                                                   ,Tsl_CreatedDate)
                                             VALUES
                                                   (@IDNumber
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@Salary
                                                   ,@PayrollType
                                                   ,@MovementReason
                                                   ,@Remarks
                                                   ,@IsLateEntry
                                                   ,@PayCycleCode
                                                   ,@CompanyCode
                                                   ,@ProfileCode
                                                   ,'A'
                                                   ,@Usr_Login
                                                   ,GETDATE())
                                        ");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateSalary(string EmployeeId
                                        , object StartDate
                                        , object EndDate
                                        , string Salary
                                        , string PayrollType
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[8];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Salary", Salary);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayrollType", PayrollType);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpSalary
                                                   SET Tsl_EndDate = @EndDate
                                                      ,Tsl_SalaryRate = @Salary
                                                      ,Tsl_PayrollType = @PayrollType
                                                      ,Tsl_ReasonCode = @MovementReason
                                                      ,Tsl_UpdatedBy = @Usr_Login
                                                      ,Tsl_UpdatedDate = GETDATE()
                                                      ,Tsl_Remarks = @Remarks
                                                 WHERE Tsl_IDNo = @EmployeeId
                                                 AND Tsl_StartDate = @StartDate");

            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectSalaryEffectivity(string EmployeeId
                                                    , string Usr_Login
                                                    , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpSalary
                                            SET Tsl_StartDate = CORRECTED.NewStartDate
	                                            , Tsl_EndDate = CORRECTED.NewEndDate
                                                , Tsl_UpdatedBy = @Usr_Login
                                                , Tsl_UpdatedDate = GETDATE()
                                            FROM T_EmpSalary ORIG
                                            INNER JOIN (
	                                            SELECT A.Tsl_IDNo AS EmployeeID
		                                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Tsl_StartDate)) AS NewStartDate
		                                            , (
				                                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Tsl_StartDate)) - 1 
				                                            FROM T_EmpSalary B
				                                            WHERE A.Tsl_IDNo = B.Tsl_IDNo
				                                            AND B.Tsl_StartDate > A.Tsl_StartDate
			                                            ) AS NewEndDate
	                                            FROM T_EmpSalary A
                                            ) CORRECTED
                                            ON ORIG.Tsl_IDNo = CORRECTED.EmployeeID
	                                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Tsl_StartDate)) = CORRECTED.NewStartDate
                                            WHERE ORIG.Tsl_StartDate != CORRECTED.NewStartDate
	                                            OR ORIG.Tsl_EndDate != CORRECTED.NewEndDate
	                                            OR (ORIG.Tsl_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                            OR (ORIG.Tsl_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                                AND ORIG.Tsl_IDNo = @EmployeeId");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestSalary(string EmployeeId
                                        , object StartDate
                                        , string Usr_Login
                                        , DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpSalary
                                               SET Tsl_EndDate = dateadd(dd, -1, @StartDate)
                                               ,Tsl_UpdatedBy = @Usr_Login
                                               ,Tsl_UpdatedDate = GETDATE()
                                             WHERE Tsl_IDNo = @EmployeeId
                                             AND Tsl_StartDate = (SELECT MAX(Salary.Tsl_StartDate)
					                                                FROM T_EmpSalary Salary
					                                                WHERE Tsl_IDNo = @EmployeeId)
                                            ");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermSalary(string EmployeeId, DateTime StartDate, string user, DALHelper dalCentral)
        {
            #region query
            string query = "";

                query = string.Format(@"DELETE FROM T_EmpSalary
                                               WHERE Tsl_IDNo = '{0}'
                                                AND Tsl_StartDate = '{1}'
                                
                                        DECLARE @Enddate as datetime
                                            SET @Enddate = (SELECT TOP(1)Tsl_EndDate 
                                                            FROM T_EmpSalary
                                                            WHERE Tsl_IDNo = '{0}'
                                                            ORDER BY Tsl_StartDate desc)

                                        UPDATE TOP(1) 
                                            T_EmpSalary
                                            SET Tsl_EndDate = NULL
                                            ,Tsl_UpdatedBy = '{2}'
                                            ,Tsl_UpdatedDate = GETDATE()
                                            WHERE Tsl_IDNo = '{0}'
                                             AND Tsl_EndDate = @Enddate
                                        ", EmployeeId, StartDate, user);


            #endregion
            dalCentral.ExecuteNonQuery(query);
        }

        public void CorrectPayrollTypeInOtherTable(string IDNumber, object StartDate, string Usr_Login, string PayrollDBName, string CompanyCode, DALHelper dalCentral)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartCycle", StartDate);

            string query = string.Format(@"UPDATE {0}..T_EmpOvertime
                                                SET Tot_PayrollType = ISNULL((SELECT TOP(1) Tsl_PayrollType
								                                            FROM T_EmpSalary
								                                            WHERE Tsl_IDNo = Tot_IDNo
								                                            AND Tsl_StartDate <= Tot_OvertimeDate
								                                            AND Tot_OvertimeDate <= ISNULL(Tsl_EndDate, Tot_OvertimeDate)
								                                            ORDER BY Tsl_StartDate DESC), Tot_PayrollType)
                                                , Tot_UpdatedBy = @Usr_Login
                                                , Tot_UpdatedDate = GETDATE()
                                            WHERE Tot_IDNo = @IDNumber

                                           
                                            UPDATE {0}..T_EmpLeave
                                                SET Tlv_PayrollType = ISNULL((SELECT TOP(1) Tsl_PayrollType
								                                            FROM T_EmpSalary
								                                            WHERE Tsl_IDNo = Tlv_IDNo
								                                            AND Tsl_StartDate <= Tlv_LeaveDate
								                                            AND Tlv_LeaveDate <= ISNULL(Tsl_EndDate, Tlv_LeaveDate)
								                                            ORDER BY Tsl_StartDate DESC), Tlv_PayrollType) 
                                                , Tlv_UpdatedBy = @Usr_Login
                                                , Tlv_UpdatedDate = GETDATE()
                                            WHERE Tlv_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeCorrection
                                                SET Ttm_PayrollType = ISNULL((SELECT TOP(1) Tsl_PayrollType
								                                            FROM T_EmpSalary
								                                            WHERE Tsl_IDNo = Ttm_IDNo
								                                            AND Tsl_StartDate <= Ttm_TimeCorDate
								                                            AND Ttm_TimeCorDate <= ISNULL(Tsl_EndDate, Ttm_TimeCorDate)
								                                            ORDER BY Tsl_StartDate DESC), Ttm_PayrollType) 
                                                , Ttm_UpdatedBy = @Usr_Login
                                                , Ttm_UpdatedDate = GETDATE()
                                            WHERE Ttm_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpShift
                                                SET Tes_PayrollType = ISNULL((SELECT TOP(1) Tsl_PayrollType
								                                            FROM T_EmpSalary
								                                            WHERE Tsl_IDNo = Tes_IDNo
								                                            AND Tsl_StartDate <= Tes_ShiftDate
								                                            AND Tes_ShiftDate <= ISNULL(Tsl_EndDate, Tes_ShiftDate)
								                                            ORDER BY Tsl_StartDate DESC), Tes_PayrollType) 
                                                , Tes_UpdatedBy = @Usr_Login
                                                , Tes_UpdatedDate = GETDATE()
                                            WHERE Tes_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpInfo
                                                SET Tei_PayrollType = ISNULL((SELECT TOP(1) Tsl_PayrollType
								                                            FROM T_EmpSalary
								                                            WHERE Tsl_IDNo = Tei_IDNo
								                                            AND Tsl_StartDate <= Tei_RequestDate
								                                            AND Tei_RequestDate <= ISNULL(Tsl_EndDate, Tei_RequestDate)
								                                            ORDER BY Tsl_StartDate DESC), Tei_PayrollType) 
                                                , Tei_UpdatedBy = @Usr_Login
                                                , Tei_UpdatedDate = GETDATE()
                                            WHERE Tei_IDNo = @IDNumber 

                                            UPDATE {0}..T_EmpTimeRegister
                                                SET Ttr_PayrollType = ISNULL((SELECT TOP(1) Tsl_PayrollType
								                                            FROM T_EmpSalary
								                                            WHERE Tsl_IDNo = Ttr_IDNo
								                                            AND Tsl_StartDate <= Ttr_Date
								                                            AND Ttr_Date <= ISNULL(Tsl_EndDate, Ttr_Date)
								                                            ORDER BY Tsl_StartDate DESC), Ttr_PayrollType) 
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                            WHERE Ttr_IDNo = @IDNumber 

                                            ", PayrollDBName);

            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateEmployeeMasterSalary(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , string PayrollDBName
                                                , DALHelper dalCentral)
        {
            //ParameterInfo[] paramInfo = new ParameterInfo[4];
            //int paramInfoCnt = 0;
            //paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            //paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            //paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            //paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@" DECLARE @NewStartDate datetime
                                            DECLARE @NewSalaryRate decimal(9, 2)
                                            DECLARE @NewPayrollType char(1)

                                            IF EXISTS (SELECT *
			                                            FROM T_EmpSalary
			                                            WHERE Tsl_IDNo = '{0}'
				                                            And Tsl_StartDate <= '{2}')
                                            BEGIN
	                                            SELECT TOP 1 
                                                @NewStartDate = Tsl_StartDate
                                                , @NewSalaryRate = Tsl_SalaryRate
                                                , @NewPayrollType = Tsl_PayrollType
	                                            FROM T_EmpSalary
	                                            WHERE Tsl_IDNo = '{0}'
	                                            And Tsl_StartDate <= '{2}'
	                                            ORDER BY Tsl_StartDate DESC
                                            END
                                            ELSE BEGIN
	                                            SELECT TOP 1 
                                                @NewStartDate = Tsl_StartDate
                                                , @NewSalaryRate = Tsl_SalaryRate
                                                , @NewPayrollType = Tsl_PayrollType
	                                            FROM T_EmpSalary
	                                            WHERE Tsl_IDNo = '{0}'
	                                            AND Tsl_StartDate > '{2}'
	                                            ORDER BY Tsl_StartDate
                                            END

                                            UPDATE M_Employee
                                            SET Mem_SalaryDate = ISNULL(@NewStartDate, Mem_SalaryDate)
                                                , Mem_Salary = ISNULL(@NewSalaryRate, Mem_Salary)
                                                , Mem_PayrollType = ISNULL(@NewPayrollType, Mem_PayrollType)
                                                , Mem_OldSalaryDate = ISNULL((
                                                                            SELECT TOP 1 Tsl_StartDate
                                                                            FROM T_EmpSalary
                                                                            WHERE Tsl_IDNo = Mem_IDNo
                                                                                AND Tsl_StartDate < @NewStartDate
                                                                                ORDER BY Tsl_StartDate DESC), Mem_OldSalaryDate)
                                                , Mem_OldSalary = ISNULL((
                                                                            SELECT TOP 1 Tsl_SalaryRate
                                                                            FROM T_EmpSalary
                                                                            WHERE Tsl_IDNo = Mem_IDNo
	                                                                            AND Tsl_StartDate < @NewStartDate
	                                                                            ORDER BY Tsl_StartDate DESC), Mem_OldSalary)
                                                , Mem_OldPayrollType = ISNULL((
                                                                            SELECT TOP 1 Tsl_PayrollType
                                                                            FROM T_EmpSalary
                                                                            WHERE Tsl_IDNo = Mem_IDNo
	                                                                            AND Tsl_StartDate < @NewStartDate
	                                                                            ORDER BY Tsl_StartDate DESC), Mem_OldPayrollType)
                                                , Mem_UpdatedBy = '{3}'
                                                , Mem_UpdatedDate = GETDATE()
                                            WHERE Mem_IDNo = '{0}'
                                            ",  EmployeeId, PayPeriodStart, PayPeriodEnd, Usr_Login);


            string PayrollQuery = string.Format(@" DECLARE @NewStartDate datetime
                                            DECLARE @NewSalaryRate decimal(9, 2)
                                            DECLARE @NewPayrollType char(1)

                                            IF EXISTS (SELECT *
			                                            FROM T_EmpSalary
			                                            WHERE Tsl_IDNo = '{0}'
				                                            And Tsl_StartDate <= '{2}')
                                            BEGIN
	                                            SELECT TOP 1 
                                                @NewStartDate = Tsl_StartDate
                                                , @NewSalaryRate = Tsl_SalaryRate
                                                , @NewPayrollType = Tsl_PayrollType
	                                            FROM T_EmpSalary
	                                            WHERE Tsl_IDNo = '{0}'
	                                            AND Tsl_StartDate <= '{2}'
	                                            ORDER BY Tsl_StartDate DESC
                                            END
                                            ELSE BEGIN
	                                            SELECT TOP 1 
                                                @NewStartDate = Tsl_StartDate
                                                , @NewSalaryRate = Tsl_SalaryRate
                                                , @NewPayrollType = Tsl_PayrollType
	                                            FROM T_EmpSalary
	                                            WHERE Tsl_IDNo = '{0}'
	                                            AND Tsl_StartDate > '{2}'
	                                            ORDER BY Tsl_StartDate
                                            END

                                            UPDATE {4}..M_Employee
                                            SET Mem_SalaryDate = ISNULL(@NewStartDate, Mem_SalaryDate)
                                                , Mem_Salary = ISNULL(@NewSalaryRate, Mem_Salary)
                                                , Mem_PayrollType = ISNULL(@NewPayrollType, Mem_PayrollType)
                                                , Mem_OldSalaryDate = ISNULL((
                                                                            SELECT TOP 1 Tsl_StartDate
                                                                            FROM T_EmpSalary
                                                                            WHERE Tsl_IDNo = Mem_IDNo
                                                                                AND Tsl_StartDate < @NewStartDate
                                                                                ORDER BY Tsl_StartDate DESC), Mem_OldSalaryDate)
                                                , Mem_OldSalary = ISNULL((
                                                                            SELECT TOP 1 Tsl_SalaryRate
                                                                            FROM T_EmpSalary
                                                                            WHERE Tsl_IDNo = Mem_IDNo
	                                                                            AND Tsl_StartDate < @NewStartDate
	                                                                            ORDER BY Tsl_StartDate DESC), Mem_OldSalary)
                                                , Mem_OldPayrollType = ISNULL((
                                                                            SELECT TOP 1 Tsl_PayrollType
                                                                            FROM T_EmpSalary
                                                                            WHERE Tsl_IDNo = Mem_IDNo
	                                                                            AND Tsl_StartDate < @NewStartDate
	                                                                            ORDER BY Tsl_StartDate DESC), Mem_OldPayrollType)
                                                , Mem_UpdatedBy = '{3}'
                                                , Mem_UpdatedDate = GETDATE()
                                            WHERE Mem_IDNo = '{0}'
                                            ", EmployeeId, PayPeriodStart, PayPeriodEnd, Usr_Login, PayrollDBName);
            #endregion
            dalCentral.ExecuteNonQuery(query);
            dalCentral.ExecuteNonQuery(PayrollQuery);
        }

        public DataTable GetLatestSalaryMovement(string IDNumber, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT TOP(1) * 
                                           FROM T_EmpSalary
                                           WHERE Tsl_IDNo = '{0}'
                                           ORDER BY Tsl_StartDate DESC", IDNumber);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        #endregion

        #region Workgroup Movement
        public DataTable GetEmployeeGroupMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT Tcg_StartDate as 'Start Date'
                                              ,Tcg_EndDate as 'End Date'
                                              ,Tcg_CalendarType as 'Work Type'
                                              ,Tcg_CalendarGroup as 'Work Group'
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpCalendarGroup
										  LEFT JOIN M_CodeDtl Reason
											ON Reason.Mcd_Code = Tcg_ReasonCode
											AND Reason.Mcd_CodeType = 'GROUPMVE'
                                          WHERE Tcg_IDNo = '{0}'
                                          ORDER BY Tcg_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeGroupMovement(string SystemID, string user_Logged, string ProfileCode)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tcg_IDNo as 'Employee ID'
                                              ,Mem_LastName as 'Last Name'
                                              ,Mem_FirstName as 'First Name'
                                              ,Tcg_StartDate as 'Effective Date'
                                              ,Tcg_CalendarType as 'Work Type'
                                              ,Tcg_CalendarGroup as 'Work Group'
                                              ,Tcg_ReasonCode as 'Reason Code'
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpCalendarGroup
                                          INNER JOIN M_Employee
                                            ON Tcg_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
@ACCESSRIGHTS
										  LEFT JOIN M_CodeDtl Reason
											ON Reason.Mcd_Code = Tcg_ReasonCode
											AND Reason.Mcd_CodeType = 'GROUPMVE'
                                          WHERE Tcg_IDNo + Convert(Char(10),Tcg_StartDate,112) in ( SELECT Tcg_IDNo + Convert(Char(10), Tcg_StartDate, 112)
																                                 FROM (SELECT Tcg_IDNo , Max(Tcg_StartDate) as Tcg_StartDate
																			                            FROM T_EmpCalendarGroup
																			                            WHERE @EndNewCycle >= Tcg_StartDate
																					                        AND ISNULL(Tcg_EndDate, @StartNewCycle) >= @StartNewCycle
																			                            GROUP BY Tcg_IDNo ) Temp) 
                                          ORDER BY Mem_LastName, Mem_FirstName");
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(ProfileCode, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", "", "", false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeeGroupMovementRow(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT
	                                        Mem_CalendarGroup
	                                        , Mem_CalendarType
                                        FROM M_Employee
                                        WHERE Mem_IDNo = '{0}'", EmployeeId);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeeGroupMovement(string EmployeeId, DateTime EffectivityDate)
        {
            #region query
            string query = string.Format(@"SELECT Tcg_StartDate as 'Start Date'
                                              ,Tcg_EndDate as 'End Date'
                                              ,Tcg_CalendarType as 'Work Type'
                                              ,Tcg_CalendarGroup as 'Work Group'
                                              ,Tcg_ReasonCode
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpCalendarGroup
										  LEFT JOIN M_CodeDtl Reason
											ON Reason.Mcd_Code = Tcg_ReasonCode
											AND Reason.Mcd_CodeType = 'GROUPMVE'
                                          WHERE Tcg_IDNo = '{0}'
                                            AND Tcg_StartDate = '{1}'
                                          ORDER BY Tcg_StartDate DESC", EmployeeId, EffectivityDate);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertWorkgroup(string EmployeeId
                                        , object EffectivityDate
                                        , object EndDate
                                        , string Worktype
                                        , string Workgroup
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Worktype", Worktype);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Workgroup", Workgroup);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpCalendarGroup
                                                   (Tcg_IDNo
                                                   ,Tcg_StartDate
                                                   ,Tcg_EndDate
                                                   ,Tcg_CalendarType
                                                   ,Tcg_CalendarGroup
                                                   ,Tcg_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@EffectivityDate
                                                   ,@EndDate
                                                   ,@Worktype
                                                   ,@Workgroup
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE())");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateWorkgroup(string EmployeeId
                                        , object EffectivityDate
                                        , object EndDate
                                        , string Worktype
                                        , string Workgroup
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Worktype", Worktype);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Workgroup", Workgroup);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpCalendarGroup
                                               SET Tcg_EndDate = @EndDate
                                                  ,Tcg_CalendarType = @Worktype
                                                  ,Tcg_CalendarGroup = @Workgroup
                                                  ,Tcg_ReasonCode = @MovementReason
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Tcg_IDNo = @EmployeeId
                                             AND Tcg_StartDate = @EffectivityDate");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestWorkgroup(string EmployeeId
                                            , object EffectivityDate
                                            , string Usr_Login
                                            , DALHelper dalCentral)
         {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EffectivityDate", EffectivityDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpCalendarGroup
                                               SET Tcg_EndDate = dateadd(dd, -1, @EffectivityDate)
                                                  --,Usr_Login = @Usr_Login
                                                  --,Ludatetime = GETDATE()
                                             WHERE Tcg_IDNo = @EmployeeId
                                             AND Tcg_StartDate = (SELECT MAX(Workgroup.Tcg_StartDate)
							                                            FROM T_EmpCalendarGroup Workgroup
							                                            WHERE Tcg_IDNo = @EmployeeId)");
            #endregion
            dalCentral.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectWorkgroupEffectivity(string EmployeeId
                                            , string Usr_Login
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpCalendarGroup
                                            SET Tcg_StartDate = CORRECTED.NewStartDate
	                                            , Tcg_EndDate = CORRECTED.NewEndDate
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                            FROM T_EmpCalendarGroup ORIG
                                            INNER JOIN (
	                                            SELECT A.Tcg_IDNo AS EmployeeID
		                                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Tcg_StartDate)) AS NewStartDate
		                                            , (
				                                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Tcg_StartDate)) - 1 
				                                            FROM T_EmpCalendarGroup B
				                                            WHERE A.Tcg_IDNo = B.Tcg_IDNo
				                                            AND B.Tcg_StartDate > A.Tcg_StartDate
			                                            ) AS NewEndDate
	                                            FROM T_EmpCalendarGroup A
                                            ) CORRECTED
                                            ON ORIG.Tcg_IDNo = CORRECTED.EmployeeID
	                                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Tcg_StartDate)) = CORRECTED.NewStartDate
                                            WHERE ORIG.Tcg_StartDate != CORRECTED.NewStartDate
	                                            OR ORIG.Tcg_EndDate != CORRECTED.NewEndDate
	                                            OR (ORIG.Tcg_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                            OR (ORIG.Tcg_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                                AND ORIG.Tcg_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermWorkgroup(string EmployeeId, DateTime EffectivityDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"DELETE FROM T_EmpCalendarGroup
                                           WHERE Tcg_IDNo = '{0}'
                                            AND Tcg_StartDate = '{1}'
                                          
                                          	Declare @Enddate as datetime
                                                Set @Enddate = (select top(1)Tcg_EndDate from T_EmpCalendarGroup
                                                Where Tcg_IDNo = '{0}'
                                                order by Tcg_StartDate desc)

                                            Update TOP(1) 
                                                T_EmpCalendarGroup
                                                set Tcg_EndDate = null
                                                Where Tcg_IDNo = '{0}'
                                                 and Tcg_EndDate = @Enddate   
                                        ", EmployeeId, EffectivityDate);
            #endregion
            dal.ExecuteNonQuery(query);
        }


        public void UpdateEmployeeMasterWorkgroup(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_CalendarType = ISNULL((
                                                                    Select TOP(1) Tcg_CalendarType 
                                                                    From T_EmpCalendarGroup
                                                                    Where Tcg_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tcg_StartDate 
	                                                                    And ISNULL(Tcg_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tcg_StartDate DESC), '')
                                                , Mem_CalendarGroup = ISNULL((
                                                                    Select TOP(1) Tcg_CalendarGroup
                                                                    From T_EmpCalendarGroup
                                                                    Where Tcg_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tcg_StartDate 
	                                                                    And ISNULL(Tcg_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tcg_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public DataTable GetLatestWorkgroupMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"Select TOP(1) * 
                                           From T_EmpCalendarGroup
                                           Where Tcg_IDNo = '{0}'
                                           Order By Tcg_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        #endregion

        #region Premium Group Movement
        public DataTable GetEmployeePremiumGroupMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT Tpg_StartDate as 'Start Date'
                                              ,Tpg_EndDate as 'End Date'
                                              ,PremGrp.Mcd_Name as 'Premium Group'
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpPremiumGroup
                                          LEFT JOIN M_CodeDtl PremGrp
	                                        ON PremGrp.Mcd_Code = Tpg_PremiumGroup
	                                        AND PremGrp.Mcd_CodeType = 'PREMGRP'
                                          LEFT JOIN M_CodeDtl Reason
	                                        ON Reason.Mcd_Code = Tpg_ReasonCode
	                                        AND Reason.Mcd_CodeType = 'PREMMVE'
	                                      WHERE Tpg_IDNo = '{0}'
                                          ORDER BY Tpg_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeePremiumGroupMovement(string SystemID, string user_Logged, string ProfileCode)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tpg_IDNo as 'Employee ID'
                                              ,Mem_LastName as 'Last Name'
                                              ,Mem_FirstName as 'First Name'
                                              ,Tpg_StartDate as 'Effective Date'
                                              ,Tpg_PremiumGroup as 'Day Premium Group'
                                              ,PremGrp.Mcd_Name as 'Description'
                                              ,Tpg_ReasonCode as 'Reason Code'
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpPremiumGroup
                                          INNER JOIN M_Employee
                                            ON Tpg_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
@ACCESSRIGHTS
                                          LEFT JOIN M_CodeDtl PremGrp
	                                        ON PremGrp.Mcd_Code = Tpg_PremiumGroup
	                                        AND PremGrp.Mcd_CodeType = 'PREMGRP'
                                          LEFT JOIN M_CodeDtl Reason
	                                        ON Reason.Mcd_Code = Tpg_ReasonCode
	                                        AND Reason.Mcd_CodeType = 'PREMMVE'
	                                      WHERE Tpg_IDNo + Convert(Char(10),Tpg_StartDate,112) in ( SELECT Tpg_IDNo + Convert(Char(10), Tpg_StartDate, 112)
																                                 FROM (SELECT Tpg_IDNo , Max(Tpg_StartDate) as Tpg_StartDate
																			                            FROM T_EmpPremiumGroup
																			                            WHERE @EndNewCycle >= Tpg_StartDate
																				                               AND ISNULL(Tpg_EndDate, @StartNewCycle) >= @StartNewCycle 
																			                            GROUP BY Tpg_IDNo ) Temp) 
                                          ORDER BY Mem_LastName, Mem_FirstName");
            #endregion
            
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(ProfileCode, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", "", "", false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeePremiumGroupMovementRow(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT
                                            Mem_PremiumGrpCode
                                            , Mem_PremiumGrpDate
                                        FROM M_Employee
                                        WHERE Mem_IDNo = '{0}'", EmployeeId);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeePremiumGroupMovement(string EmployeeId, DateTime EffectivityDate)
        {
            #region query
            string query = string.Format(@"SELECT Tpg_StartDate as 'Start Date'
                                              ,Tpg_EndDate as 'End Date'
                                              ,Tpg_PremiumGroup
                                              ,PremGrp.Mcd_Name as 'Premium Group'
                                              ,Tpg_ReasonCode
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpPremiumGroup
                                          LEFT JOIN M_CodeDtl PremGrp
	                                        ON PremGrp.Mcd_Code = Tpg_PremiumGroup
	                                        AND PremGrp.Mcd_CodeType = 'PREMGRP'
                                          LEFT JOIN M_CodeDtl Reason
	                                        ON Reason.Mcd_Code = Tpg_ReasonCode
	                                        AND Reason.Mcd_CodeType = 'PREMMVE'
	                                      WHERE Tpg_IDNo = '{0}'
                                            AND Tpg_StartDate = '{1}'
                                          ORDER BY Tpg_StartDate DESC", EmployeeId, EffectivityDate);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertPremiumGroup(string EmployeeId
                                        , object StartDate
                                        , object EndDate
                                        , string PremiumGroup
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PremiumGroup", PremiumGroup);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpPremiumGroup
                                                   (Tpg_IDNo
                                                   ,Tpg_StartDate
                                                   ,Tpg_EndDate
                                                   ,Tpg_PremiumGroup
                                                   ,Tpg_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@PremiumGroup
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE())");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdatePremiumGroup(string EmployeeId
                                        , object StartDate
                                        , object EndDate
                                        , string PremiumGroup
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PremiumGroup", PremiumGroup);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpPremiumGroup
                                               SET Tpg_EndDate = @EndDate
                                                  ,Tpg_PremiumGroup = @PremiumGroup
                                                  ,Tpg_ReasonCode = @MovementReason
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Tpg_IDNo = @EmployeeId
                                             AND Tpg_StartDate = @StartDate");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectPremiumGroupEffectivity(string EmployeeId
                                                    , string Usr_Login
                                                    , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpPremiumGroup
                                            SET Tpg_StartDate = CORRECTED.NewStartDate
	                                            , Tpg_EndDate = CORRECTED.NewEndDate
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                            FROM T_EmpPremiumGroup ORIG
                                            INNER JOIN (
	                                            SELECT A.Tpg_IDNo AS EmployeeID
		                                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Tpg_StartDate)) AS NewStartDate
		                                            , (
				                                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Tpg_StartDate)) - 1 
				                                            FROM T_EmpPremiumGroup B
				                                            WHERE A.Tpg_IDNo = B.Tpg_IDNo
				                                            AND B.Tpg_StartDate > A.Tpg_StartDate
			                                            ) AS NewEndDate
	                                            FROM T_EmpPremiumGroup A
                                            ) CORRECTED
                                            ON ORIG.Tpg_IDNo = CORRECTED.EmployeeID
	                                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Tpg_StartDate)) = CORRECTED.NewStartDate
                                            WHERE ORIG.Tpg_StartDate != CORRECTED.NewStartDate
	                                            OR ORIG.Tpg_EndDate != CORRECTED.NewEndDate
	                                            OR (ORIG.Tpg_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                            OR (ORIG.Tpg_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                                AND ORIG.Tpg_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestPremiumGroup(string EmployeeId
                                            , object StartDate
                                            , string Usr_Login
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpPremiumGroup
                                               SET Tpg_EndDate = dateadd(dd, -1, @StartDate)
                                                  --,Usr_Login = @Usr_Login
                                                  --,Ludatetime = GETDATE()
                                             WHERE Tpg_IDNo = @EmployeeId
                                             AND Tpg_StartDate = (SELECT MAX(PremGrp.Tpg_StartDate)
					                                                FROM T_EmpPremiumGroup PremGrp
					                                                WHERE Tpg_IDNo = @EmployeeId)");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermPremiumGroup(string EmployeeId, DateTime StartDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"DELETE FROM T_EmpPremiumGroup
                                           WHERE Tpg_IDNo = '{0}'
                                            AND Tpg_StartDate = '{1}'
                                            
                                           Declare @Enddate as datetime
                                            Set @Enddate = (select top(1)Tpg_EndDate from T_EmpPremiumGroup
                                            Where Tpg_IDNo = '{0}'
                                            order by Tpg_StartDate desc)

                                        Update TOP(1) 
                                            T_EmpPremiumGroup
                                            set Tpg_EndDate = null
                                            Where Tpg_IDNo = '{0}'
                                             and Tpg_EndDate = @Enddate 
                                        ", EmployeeId, StartDate);
            #endregion
            dal.ExecuteNonQuery(query);
        }

        public void UpdateEmployeeMasterPremiumGroup(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_PremiumGrpDate = ISNULL((
                                                                                Select TOP(1) Tpg_StartDate 
                                                                                From T_EmpPremiumGroup
                                                                                Where Tpg_IDNo = @EmployeeId 
	                                                                                And @PayPeriodEnd >= Tpg_StartDate 
	                                                                                And ISNULL(Tpg_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                                Order By Tpg_StartDate DESC), Mem_PremiumGrpDate)
                                                , Mem_PremiumGrpCode = ISNULL((
                                                                    Select TOP(1) Tpg_PremiumGroup 
                                                                    From T_EmpPremiumGroup
                                                                    Where Tpg_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tpg_StartDate 
	                                                                    And ISNULL(Tpg_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tpg_StartDate DESC), Mem_PremiumGrpCode)
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public DataTable GetLatestPremiumGroupMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"Select TOP(1) * 
                                           From T_EmpPremiumGroup
                                           Where Tpg_IDNo = '{0}'
                                           Order By Tpg_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        #endregion

        #region Employment Status Movement
        public DataTable GetEmployeeEmploymentStatusMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT Tes_StartDate as 'Start Date'
                                              ,Tes_EndDate as 'End Date'
                                              ,EMPSTAT.Mcd_Name as 'Employment Status'
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpEmploymentStatus
                                          LEFT JOIN M_CodeDtl EMPSTAT
	                                        ON EMPSTAT.Mcd_Code = Tes_EmploymentStatusCode
	                                        AND EMPSTAT.Mcd_CodeType = 'EMPSTAT'
                                          LEFT JOIN M_CodeDtl Reason
	                                        ON Reason.Mcd_Code = Tes_ReasonCode
	                                        AND Reason.Mcd_CodeType = 'EMPSTATMVE'
	                                      WHERE Tes_IDNo = '{0}'
                                          ORDER BY Tes_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeEmploymentStatusMovement(string SystemID, string user_Logged, string ProfileCode)
        {
            #region query
            string query = string.Format(@"DECLARE @StartNewCycle DATETIME 
                                            DECLARE @EndNewCycle DATETIME

                                            SELECT @StartNewCycle = Tps_StartCycle
		                                            , @EndNewCycle = Tps_EndCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'C'

                                            SELECT Tes_IDNo as 'Employee ID'
                                              ,Mem_LastName as 'Last Name'
                                              ,Mem_FirstName as 'First Name'
                                              ,Tes_StartDate as 'Effective Date'
                                              ,Tes_EmploymentStatusCode as 'Employment Status'
                                              ,EMPSTAT.Mcd_Name as 'Description'
                                              ,Tes_ReasonCode as 'Reason Code'
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpEmploymentStatus
                                          INNER JOIN M_Employee
                                            ON Tes_IDNo = Mem_IDNo
                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
@ACCESSRIGHTS
                                          LEFT JOIN M_CodeDtl EMPSTAT
	                                        ON EMPSTAT.Mcd_Code = Tes_EmploymentStatusCode
	                                        AND EMPSTAT.Mcd_CodeType = 'EMPSTAT'
                                          LEFT JOIN M_CodeDtl Reason
	                                        ON Reason.Mcd_Code = Tes_ReasonCode
	                                        AND Reason.Mcd_CodeType = 'EMPSTATMVE'
	                                      WHERE Tes_IDNo + Convert(Char(10),Tes_StartDate,112) in ( SELECT Tes_IDNo + Convert(Char(10), Tes_StartDate, 112)
                                                                                                                 FROM (SELECT Tes_IDNo , Max(Tes_StartDate) as Tes_StartDate
                                                                                                                        FROM T_EmpEmploymentStatus
                                                                                                                        WHERE @EndNewCycle >= Tes_StartDate
                                                                                                                            AND ISNULL(Tes_EndDate, @StartNewCycle) >= @StartNewCycle
                                                                                                                        GROUP BY Tes_IDNo ) Temp) 
                                          ORDER BY Mem_LastName, Mem_FirstName");
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(ProfileCode, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", "", "", false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataRow FetchEmployeeEmploymentStatusMovementRow(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT
                                            Mem_EmploymentStatusCode
                                        FROM M_Employee
                                        WHERE Mem_IDNo = '{0}'", EmployeeId);
            #endregion
            DataTable dtResult;
            DataRow drResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                drResult = dtResult.Rows[0];

            return drResult;
        }

        public DataTable GetEmployeeEmploymentStatusMovement(string EmployeeId, DateTime EffectivityDate)
        {
            #region query
            string query = string.Format(@"SELECT Tes_StartDate as 'Start Date'
                                              ,Tes_EndDate as 'End Date'
                                              ,Tes_EmploymentStatusCode
                                              ,EMPSTAT.Mcd_Name as 'Employment Status'
                                              ,Tes_ReasonCode
                                              ,Reason.Mcd_Name as 'Reason'
                                          FROM T_EmpEmploymentStatus
                                          LEFT JOIN M_CodeDtl EMPSTAT
	                                        ON EMPSTAT.Mcd_Code = Tes_EmploymentStatusCode
	                                        AND EMPSTAT.Mcd_CodeType = 'EMPSTAT'
                                          LEFT JOIN M_CodeDtl Reason
	                                        ON Reason.Mcd_Code = Tes_ReasonCode
	                                        AND Reason.Mcd_CodeType = 'EMPSTATMVE'
	                                      WHERE Tes_IDNo = '{0}'
                                            AND Tes_StartDate = '{1}'
                                          ORDER BY Tes_StartDate DESC", EmployeeId, EffectivityDate);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void InsertEmploymentStatus(string EmployeeId
                                        , object StartDate
                                        , object EndDate
                                        , string EmploymentStatus
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmploymentStatus", EmploymentStatus);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"INSERT INTO T_EmpEmploymentStatus
                                                   (Tes_IDNo
                                                   ,Tes_StartDate
                                                   ,Tes_EndDate
                                                   ,Tes_EmploymentStatusCode
                                                   ,Tes_ReasonCode
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   (@EmployeeId
                                                   ,@StartDate
                                                   ,@EndDate
                                                   ,@EmploymentStatus
                                                   ,@MovementReason
                                                   ,@Usr_Login
                                                   ,GETDATE())");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateEmploymentStatus(string EmployeeId
                                        , object StartDate
                                        , object EndDate
                                        , string EmploymentStatus
                                        , string MovementReason
                                        , string Usr_Login
                                        , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndDate", (EndDate == null || EndDate.ToString() == "") ? DBNull.Value : EndDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmploymentStatus", EmploymentStatus);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpEmploymentStatus
                                               SET Tes_EndDate = @EndDate
                                                  ,Tes_EmploymentStatusCode = @EmploymentStatus
                                                  ,Tes_ReasonCode = @MovementReason
                                                  ,Usr_Login = @Usr_Login
                                                  ,Ludatetime = GETDATE()
                                             WHERE Tes_IDNo = @EmployeeId
                                             AND Tes_StartDate = @StartDate");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void CorrectEmploymentStatusEffectivity(string EmployeeId
                                                    , string Usr_Login
                                                    , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpEmploymentStatus
                                            SET Tes_StartDate = CORRECTED.NewStartDate
	                                            , Tes_EndDate = CORRECTED.NewEndDate
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = GETDATE()
                                            FROM T_EmpEmploymentStatus ORIG
                                            INNER JOIN (
	                                            SELECT A.Tes_IDNo AS EmployeeID
		                                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Tes_StartDate)) AS NewStartDate
		                                            , (
				                                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Tes_StartDate)) - 1 
				                                            FROM T_EmpEmploymentStatus B
				                                            WHERE A.Tes_IDNo = B.Tes_IDNo
				                                            AND B.Tes_StartDate > A.Tes_StartDate
			                                            ) AS NewEndDate
	                                            FROM T_EmpEmploymentStatus A
                                            ) CORRECTED
                                            ON ORIG.Tes_IDNo = CORRECTED.EmployeeID
	                                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Tes_StartDate)) = CORRECTED.NewStartDate
                                            WHERE ORIG.Tes_StartDate != CORRECTED.NewStartDate
	                                            OR ORIG.Tes_EndDate != CORRECTED.NewEndDate
	                                            OR (ORIG.Tes_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                                            OR (ORIG.Tes_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL)
                                                AND ORIG.Tes_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateLatestEmploymentStatus(string EmployeeId
                                            , object StartDate
                                            , string Usr_Login
                                            , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE T_EmpEmploymentStatus
                                               SET Tes_EndDate = dateadd(dd, -1, @StartDate)
                                                  --,Usr_Login = @Usr_Login
                                                  --,Ludatetime = GETDATE()
                                             WHERE Tes_IDNo = @EmployeeId
                                             AND Tes_StartDate = (SELECT MAX(EMPSTAT.Tes_StartDate)
					                                                FROM T_EmpEmploymentStatus EMPSTAT
					                                                WHERE Tes_IDNo = @EmployeeId)");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeletePermEmploymentStatus(string EmployeeId, DateTime StartDate, DALHelper dal)
        {
            #region query
            string query = string.Format(@"DELETE FROM T_EmpEmploymentStatus
                                           WHERE Tes_IDNo = '{0}'
                                            AND Tes_StartDate = '{1}'
                                            
                                           Declare @Enddate as datetime
                                                Set @Enddate = (select top(1)Tes_EndDate from T_EmpEmploymentStatus
                                                Where Tes_IDNo = '{0}'
                                                order by Tes_StartDate desc)

                                            Update TOP(1) 
                                                T_EmpEmploymentStatus
                                                set Tes_EndDate = null
                                                Where Tes_IDNo = '{0}'
                                                 and Tes_EndDate = @Enddate 
                                            ", EmployeeId, StartDate);
            #endregion
            dal.ExecuteNonQuery(query);
        }

        public void UpdateEmployeeMasterEmploymentStatus(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string Usr_Login
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"UPDATE M_Employee
                                            SET Mem_EmploymentStatusCode = ISNULL((
                                                                    Select TOP(1) Tes_EmploymentStatusCode 
                                                                    From T_EmpEmploymentStatus
                                                                    Where Tes_IDNo = @EmployeeId 
	                                                                    And @PayPeriodEnd >= Tes_StartDate 
	                                                                    And ISNULL(Tes_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                                    Order By Tes_StartDate DESC), '')
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            WHERE Mem_IDNo = @EmployeeId");
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public DataTable GetLatestEmploymentStatusMovement(string EmployeeId)
        {
            #region query
            string query = string.Format(@"Select TOP(1) * 
                                           From T_EmpEmploymentStatus
                                           Where Tes_IDNo = '{0}'
                                           Order By Tes_StartDate DESC", EmployeeId);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        #endregion

        #region Fix Allowance Movement
        public DataTable GetEmployeeFixAllowanceMovement(string IDNumber, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Tfa_AllowanceCode as [Code]
                                                , Min_IncomeName as [Name]
                                                , Tfa_StartPayCycle as [Start Paycycle]
                                                , ISNULL(Tfa_EndPayCycle,'') AS [End Paycycle] 
                                                , CAST(Tfa_Amount AS decimal(10,2)) AS [Amount]
                                                , FALWMVE.Mcd_Name AS [Reason]
                                                , Tfa_Remarks AS [Remarks]
                                                --, Tfa_StartPayCycle AS [Pay Cycle]
                                           FROM T_EmpFixAllowance FIXALLOW
                                           LEFT JOIN M_Income 
                                                ON Min_IncomeCode = Tfa_AllowanceCode
											    AND Min_CompanyCode = '{1}'
                                           LEFT JOIN M_CodeDtl FALWMVE ON FALWMVE.Mcd_Code = Tfa_ReasonCode
                                                AND FALWMVE.Mcd_CompanyCode = '{1}'
                                                AND FALWMVE.Mcd_CodeType = 'FALWMVE' 
                                           WHERE Tfa_IDNo = '{0}'
                                                --AND Tfa_StartPayCycle = (SELECT MAX(Tfa_StartPayCycle) FROM T_EmpFixAllowance as f
					                            --                         WHERE f.Tfa_IDNo = FIXALLOW.Tfa_IDNo
					                            --                            AND f.Tfa_AllowanceCode = FIXALLOW.Tfa_AllowanceCode )
                                           ORDER BY Tfa_AllowanceCode, Tfa_StartPayCycle ASC", IDNumber, CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeFixAllowanceMovement(string IDNumber, string IncomeCode, string StartPayCycle, string CompanyCode, string CentralProfile, string PayrollDBName)
        {
            #region query
            string query = string.Format(@"SELECT Tfa_AllowanceCode AS [Code]
                                                , Min_IncomeName AS [Name]
                                                , Tfa_StartPayCycle AS [Start Paycycle]
                                                , CONVERT(CHAR(10), STARTCYCLE.Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), STARTCYCLE.Tps_EndCycle, 101) as [Start Pay Cycle Range]
                                                , ISNULL(Tfa_EndPayCycle,'') AS [End Paycycle]
                                                , CASE WHEN ENDCYCLE.Tps_PayCycle IS NOT NULL THEN CONVERT(CHAR(10), ENDCYCLE.Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), ENDCYCLE.Tps_EndCycle, 101)
                                                    ELSE '' END AS [End Pay Cycle Range]
                                                , CAST(Tfa_Amount AS decimal(10,2)) AS [Amount]
                                                , Tfa_ReasonCode AS [ReasonCode]
                                                , FALWMVE.Mcd_Name AS [Reason]
                                                , ISNULL(Tfa_Remarks,'') AS [Remarks]
                                           FROM T_EmpFixAllowance FIXALLOW
                                           LEFT JOIN M_Income 
                                                ON Min_IncomeCode = Tfa_AllowanceCode
											    AND Min_CompanyCode = '{1}'
                                           LEFT JOIN {4}.dbo.T_PaySchedule STARTCYCLE ON STARTCYCLE.Tps_PayCycle = Tfa_StartPayCycle
                                           LEFT JOIN {4}.dbo.T_PaySchedule ENDCYCLE ON ENDCYCLE.Tps_PayCycle = ISNULL(Tfa_EndPayCycle,'')
                                           LEFT JOIN M_CodeDtl FALWMVE ON FALWMVE.Mcd_Code = Tfa_ReasonCode
                                                AND FALWMVE.Mcd_CompanyCode = '{1}'
                                                AND FALWMVE.Mcd_CodeType = 'FALWMVE' 
                                           WHERE Tfa_IDNo = '{0}'
                                                AND Tfa_AllowanceCode = '{2}'
                                                AND Tfa_StartPayCycle = '{3}' ", IDNumber
                                                                               , CompanyCode
                                                                               , IncomeCode
                                                                               , StartPayCycle
                                                                               , PayrollDBName);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeFixAllowanceMovement(string SystemID, string user_Logged, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @CurPayCycle CHAR(7) = (SELECT Tps_PayCycle FROM T_PaySchedule
                                                                           WHERE Tps_CycleType = 'N'
                                                                               AND Tps_CycleIndicator = 'C'
                                                                               AND Tps_RecordStatus = 'A')

                                            SELECT Tfa_IDNo as 'ID Number'
                                              ,Mem_LastName as 'Last Name'
                                              ,Mem_FirstName as 'First Name'
                                              ,Tfa_AllowanceCode as 'Code'
                                              ,Min_IncomeName as 'Name'
                                              ,Tfa_StartPayCycle as [Start Paycycle]
                                              ,ISNULL(Tfa_EndPayCycle,'') AS [End Paycycle]
                                              ,CAST(Tfa_Amount AS decimal(10,2)) AS [Amount]
                                              ,Tfa_ReasonCode as 'Reason Code'
                                              ,Reason.Mcd_Name as 'Reason'
                                              ,Tfa_Remarks as 'Remarks'
                                          FROM {0}..T_EmpFixAllowance FIXALLOW
                                          INNER JOIN M_Employee
                                                ON FIXALLOW.Tfa_IDNo = Mem_IDNo
                                                AND  Mem_WorkStatus NOT IN ('IN','IM')
                                                @ACCESSRIGHTS
                                          LEFT JOIN {0}..M_Income 
                                                ON Min_IncomeCode = FIXALLOW.Tfa_AllowanceCode
											    AND Min_CompanyCode = '{1}'
                                          LEFT JOIN {0}..M_CodeDtl Reason
                                                ON Reason.Mcd_Code = FIXALLOW.Tfa_ReasonCode
                                                AND Reason.Mcd_CodeType = 'FALWMVE'
                                                AND Reason.Mcd_CompanyCode = '{1}'
                                          WHERE FIXALLOW.Tfa_IDNo + FIXALLOW.Tfa_StartPayCycle IN ( SELECT Tfa_IDNo + Tfa_StartPayCycle
																                                    FROM (SELECT Tfa_IDNo , MAX(Tfa_StartPayCycle) as Tfa_StartPayCycle
																			                              FROM {0}..T_EmpFixAllowance f
																			                              WHERE f.Tfa_IDNo = FIXALLOW.Tfa_IDNo
																											 AND f.Tfa_AllowanceCode = FIXALLOW.Tfa_AllowanceCode
																			                              GROUP BY Tfa_IDNo ) Temp)
                                                AND ISNULL(FIXALLOW.Tfa_EndPayCycle, @CurPayCycle) >= @CurPayCycle
                                        ORDER BY Mem_LastName, Mem_FirstName, Tfa_AllowanceCode,Tfa_StartPayCycle ", CentralProfile, CompanyCode);
            #endregion
            query = query.Replace("@ACCESSRIGHTS", UserCostCenterAccessTmpQuery(CentralProfile, SystemID, user_Logged, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, ProfileCode, false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public void UpdateLatestFixAllowance(string IDNumber
                                            , string PrevEndPayCycle
                                            , string IncomeCode
                                            , string CompanyCode
                                            , string CentralProfile
                                            , string Usr_Login
                                            , string PayrollDBName
                                            , DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PrevEndPayCycle", PrevEndPayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"
                                           UPDATE {0}.dbo.T_EmpFixAllowance
                                           SET Tfa_EndPayCycle = @PrevEndPayCycle
                                                ,Tfa_UpdatedBy = @Usr_Login
                                                ,Tfa_UpdatedDate = GETDATE()
                                           WHERE Tfa_IDNo = @EmployeeId
                                                AND Tfa_AllowanceCode = @IncomeCode
                                                AND Tfa_StartPayCycle = (SELECT MAX(Tfa_StartPayCycle) 
                                                                         FROM {0}.dbo.T_EmpFixAllowance as f
					                                                     WHERE f.Tfa_IDNo = @EmployeeId
					                                                        AND f.Tfa_AllowanceCode = @IncomeCode)
                                                AND ISNULL(Tfa_EndPayCycle,'') = '' ", CentralProfile, PayrollDBName);
            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void InsertFixAllowance(string IDNumber
                                        , string StartPayCycle
                                        , string EndPayCycle
                                        , string IncomeCode
                                        , decimal Amount
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , string CompanyCode
                                        , string ProfileCode
                                        , string CentralProfile
                                        , DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[10];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartPayCycle", StartPayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndPayCycle", (string.IsNullOrEmpty(EndPayCycle) ? (object)DBNull.Value : EndPayCycle));
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Amount", Amount, SqlDbType.Decimal);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks, SqlDbType.NVarChar, 500);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@ProfileCode", ProfileCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            #region query
            string query = string.Format(@"INSERT INTO {0}.dbo.T_EmpFixAllowance   
                                            (Tfa_IDNo
                                            , Tfa_AllowanceCode
                                            , Tfa_StartPayCycle
                                            , Tfa_EndPayCycle
                                            , Tfa_Amount
                                            , Tfa_ReasonCode
                                            , Tfa_Remarks
                                            , Tfa_CompanyCode
                                            , Tfa_ProfileCode
                                            , Tfa_RecordStatus
                                            , Tfa_CreatedBy
                                            , Tfa_CreatedDate)
                                             VALUES (@IDNumber
                                                   ,@IncomeCode
                                                   ,@StartPayCycle
                                                   ,@EndPayCycle
                                                   ,@Amount
                                                   ,@MovementReason
                                                   ,@Remarks
                                                   ,@CompanyCode
                                                   ,@ProfileCode
                                                   ,'A'
                                                   ,@Usr_Login
                                                   ,GETDATE())", CentralProfile);
            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void UpdateFixAllowance(string IDNumber
                                        , string StartPayCycle
                                        , string EndPayCycle
                                        , string IncomeCode
                                        , decimal Amount
                                        , string MovementReason
                                        , string Remarks
                                        , string Usr_Login
                                        , string CentralProfile
                                        , DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[8];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartPayCycle", StartPayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EndPayCycle", (string.IsNullOrEmpty(EndPayCycle) ? (object)DBNull.Value : EndPayCycle));
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Amount", Amount, SqlDbType.Decimal);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@MovementReason", MovementReason);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Remarks", Remarks, SqlDbType.NVarChar, 500);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            #region query
            string query = string.Format(@"UPDATE {0}.dbo.T_EmpFixAllowance   
                                           SET Tfa_EndPayCycle = @EndPayCycle
                                            --, Tfa_Amount = @Amount
                                            , Tfa_ReasonCode = @MovementReason
                                            , Tfa_Remarks = @Remarks
                                            , Tfa_UpdatedBy = @Usr_Login
                                            , Tfa_UpdatedDate = GETDATE()
                                           WHERE Tfa_IDNo = @IDNumber
                                                AND Tfa_StartPayCycle = @StartPayCycle
                                                AND Tfa_AllowanceCode = @IncomeCode ", CentralProfile);
            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeleteFixAllowance(string IDNumber, string StartPayCycle, string IncomeCode, string PayrollDBName, string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartPayCycle", StartPayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@CompanyCode", CompanyCode);
            #region query
            string query = string.Format(@"DELETE FROM T_EmpFixAllowance
                                           WHERE Tfa_IDNo = @IDNumber
                                                AND Tfa_AllowanceCode = @IncomeCode
                                                AND Tfa_StartPayCycle = @StartPayCycle

                                            DECLARE @ApplicCycle CHAR(1) = (SELECT Min_ApplicablePayCycle FROM {1}.dbo.M_Income
							                                               WHERE Min_CompanyCode = @CompanyCode
							                                               AND Min_IncomeCode = @IncomeCode)
                                            

                                            DECLARE @EndPayCycle CHAR(7) = (SELECT  MAX(Tps_PayCycle) as [EndCycle]
                                                                            FROM {0}.dbo.T_PaySchedule
                                                                            WHERE Tps_CycleType = 'N'
                                                                                AND Tps_RecordStatus = 'A'
                                                                                AND Tps_PayCycle < @StartPayCycle
                                                                                AND (RIGHT(Tps_Paycycle,1) = @ApplicCycle OR @ApplicCycle = '0'))
                                            

                                            UPDATE TOP(1) T_EmpFixAllowance
                                            SET Tfa_EndPayCycle = NULL
                                            WHERE Tfa_IDNo = @IDNumber
                                                 AND Tfa_AllowanceCode = @IncomeCode
                                                 AND Tfa_EndPayCycle = @EndPayCycle 
                                        ", PayrollDBName, CentralProfile);
            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void InsertEmployeeAllowance(string IDNumber
                                        , string PayCycle
                                        , string IncomeCode
                                        , decimal Amount
                                        , string Usr_Login
                                        , string PayrollDBName
                                        , DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayCycle", PayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Amount", Amount, SqlDbType.Decimal);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);

            #region query
            string query = string.Format(@"INSERT INTO {0}.dbo.T_EmpIncome (Tin_IDNo
                                                                            , Tin_PayCycle
                                                                            , Tin_OrigPayCycle
                                                                            , Tin_IncomeCode
                                                                            , Tin_PostFlag
                                                                            , Tin_IncomeAmt
                                                                            , Usr_Login
                                                                            , Ludatetime)
				                            VALUES (@IDNumber
                                                , @PayCycle
                                                , @PayCycle
                                                , @IncomeCode
                                                , 0
                                                , @Amount
                                                , @Usr_Login
                                                , GETDATE()) ", PayrollDBName);

            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);

        }

        public void UpdateEmployeeAllowance(string IDNumber
                                        , string PayCycle
                                        , string OrigPayCycle
                                        , string IncomeCode
                                        , decimal Amount
                                        , string Usr_Login
                                        , string PayrollDBName
                                        , DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayCycle", PayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@OrigPayCycle", OrigPayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Amount", Amount, SqlDbType.Decimal);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            #region query
            string query = string.Format(@"UPDATE {0}.dbo.T_EmpIncome 
                                           SET Tin_IncomeAmt = @Amount
                                             , Tin_PostFlag = 0
                                             , Usr_Login = @Usr_Login
                                             , Ludatetime = GETDATE()
                                           WHERE Tin_IDNo = @IDNumber
                                                AND Tin_PayCycle = @PayCycle
                                                AND Tin_OrigPayCycle = @OrigPayCycle
                                                AND Tin_IncomeCode = @IncomeCode ", PayrollDBName);

            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public void DeleteEmployeeAllowance(string IDNumber
                                        , string PayCycle
                                        , string OrigPayCycle
                                        , string IncomeCode
                                        , string Usr_Login
                                        , string PayrollDBName
                                        , DALHelper dalhelper)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IDNumber", IDNumber);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayCycle", PayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@OrigPayCycle", OrigPayCycle);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@IncomeCode", IncomeCode);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            #region query
            string query = string.Format(@"DELETE FROM {0}.dbo.T_EmpIncome 
                                           WHERE Tin_IDNo = @IDNumber
                                                AND Tin_PayCycle = @PayCycle
                                                AND Tin_OrigPayCycle = @OrigPayCycle
                                                AND Tin_IncomeCode = @IncomeCode 

                                           INSERT INTO {0}.dbo.T_EmpIncome (Tin_IDNo
                                                                            , Tin_PayCycle
                                                                            , Tin_OrigPayCycle
                                                                            , Tin_IncomeCode
                                                                            , Tin_PostFlag
                                                                            , Tin_IncomeAmt
                                                                            , Usr_Login
                                                                            , Ludatetime)
				                            SELECT Tfa_IDNo
                                                , @PayCycle
                                                , @PayCycle
                                                , Tfa_AllowanceCode
                                                , 0
                                                , Tfa_Amount
                                                , @Usr_Login
                                                , GETDATE()
                                            FROM T_EmpFixAllowance FIXALLOW
                                            WHERE Tfa_IDNo = @IDNumber
                                                AND Tfa_AllowanceCode = @IncomeCode
                                                AND Tfa_StartPayCycle = (SELECT MAX(Tfa_StartPayCycle) FROM T_EmpFixAllowance as f
					                                                     WHERE f.Tfa_IDNo = FIXALLOW.Tfa_IDNo
					                                                        AND f.Tfa_AllowanceCode = FIXALLOW.Tfa_AllowanceCode )
                                               ", PayrollDBName);

            #endregion
            dalhelper.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public string GetFixAllowanceAppicablePayCycleQuery()
        {
            return @"DECLARE @FIXALLOWYRS INT = {0}
                                DECLARE @StartYear CHAR(4) = (SELECT MAX(LEFT(Tps_PayCycle,4))+1 
								                              FROM {5}.dbo.T_PaySchedule
								                              WHERE Tps_CycleType = 'N')

                                DECLARE @EndYear CHAR(4) = @StartYear + @FIXALLOWYRS-1

                                DECLARE  @StartDate DATETIME = @StartYear +'0101'
                                        ,@EndDate DATETIME = @EndYear + '1231'

                                DECLARE @ApplicCycle CHAR(1) = (SELECT Min_ApplicablePayCycle FROM {4}.dbo.M_Income
							                                    WHERE Min_CompanyCode = '{3}'
							                                        AND Min_IncomeCode = '{2}')

                                DECLARE @PAYFREQNCY CHAR(1) = '{1}'

                                {6}
                                
                                SELECT Tps_PayCycle  AS [Pay Cycle]
                                , CONVERT(VARCHAR(10),Tps_StartCycle,101) AS [Start Cycle]
                                , CONVERT(VARCHAR(10),Tps_EndCycle,101) AS [End Cycle]
                                FROM {5}.dbo.T_PaySchedule 
                                WHERE Tps_CycleType = 'N'
	                                AND Tps_CycleIndicator IN ('C','F')
                                    AND (RIGHT(Tps_Paycycle,1) = @ApplicCycle OR @ApplicCycle = '0')

                                UNION 

                                SELECT  CONVERT(CHAR(6), DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, v.number, @StartDate)), 0), 112) + '0' AS [Pay Cycle]
                                , '' AS [Start Cycle]
                                , '' AS [End Cycle]
                                FROM    master.dbo.spt_values v
                                WHERE   v.type = 'P'        
	                                AND DATEDIFF(MONTH, @StartDate, @EndDate) >= v.number
	                                AND @ApplicCycle = '0'
	                                AND @PAYFREQNCY  = 'M'

                                UNION 

                                SELECT  CONVERT(CHAR(6), DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, v.number, @StartDate)), 0), 112) + '1' AS [Pay Cycle]
                                , '' AS [Start Cycle]
                                , '' AS [End Cycle]
                                FROM    master.dbo.spt_values v
                                WHERE   v.type = 'P'        
	                                AND DATEDIFF(MONTH, @StartDate, @EndDate) >= v.number
	                                AND (@ApplicCycle = 1 OR @ApplicCycle = '0')
	                                AND @PAYFREQNCY  IN ('S','W')

                                UNION

                                SELECT  CONVERT(CHAR(6), DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, v.number, @StartDate)), 0), 112) + '2' AS [Pay Cycle]
                                , '' AS [Start Cycle]
                                , '' AS [End Cycle]
                                FROM    master.dbo.spt_values v
                                WHERE   v.type = 'P'        
	                                AND  DATEDIFF(MONTH, @StartDate, @EndDate) >= v.number
	                                AND (@ApplicCycle = 2 OR @ApplicCycle = '0')
	                                AND @PAYFREQNCY  IN ('S','W')

                                UNION

                                SELECT  CONVERT(CHAR(6), DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, v.number, @StartDate)), 0), 112) + '3' AS [Pay Cycle]
                                , '' AS [Start Cycle]
                                , '' AS [End Cycle]
                                FROM    master.dbo.spt_values v
                                WHERE   v.type = 'P'        
	                                AND  DATEDIFF(MONTH, @StartDate, @EndDate) >= v.number
	                                AND (@ApplicCycle = 3 OR @ApplicCycle = '0')
	                                AND @PAYFREQNCY = 'W'

                                UNION

                                SELECT  CONVERT(CHAR(6), DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, v.number, @StartDate)), 0), 112) + '4' AS [Pay Cycle]
                                , '' AS [Start Cycle]
                                , '' AS [End Cycle]
                                FROM    master.dbo.spt_values v
                                WHERE   v.type = 'P'        
	                                AND  DATEDIFF(MONTH, @StartDate, @EndDate) >= v.number
	                                AND (@ApplicCycle = 4 OR @ApplicCycle = '0')
	                                AND @PAYFREQNCY = 'W'

                                UNION

                                SELECT  CONVERT(CHAR(6), DATEADD(MONTH, DATEDIFF(MONTH, 0, DATEADD(MONTH, v.number, @StartDate)), 0), 112) + '5' AS [Pay Cycle]
                                , '' AS [Start Cycle]
                                , '' AS [End Cycle]
                                FROM    master.dbo.spt_values v
                                WHERE   v.type = 'P'        
	                                AND  DATEDIFF(MONTH, @StartDate, @EndDate) >= v.number
	                                AND (@ApplicCycle = 5 OR @ApplicCycle = '0')
	                                AND @PAYFREQNCY = 'W'

                                ORDER BY [Pay Cycle] ";
        }

        #endregion

        public string UserCostCenterAccessTmpQuery(string profile, string systemID, string userCode, string costCenterColumn, string payrollGroupColumn, string empStatusColumn, string payrollTypeColumn, string CompanyCode, string ProfileCode, bool reportType)
        {
            string query = string.Format(@"
INNER JOIN 
{0}..M_UserExtTmp
ON Mue_SystemCode = '{1}'
AND Mue_UserCode = '{2}'
AND Mue_CompanyCode = '{3}'
AND Mue_ProfileCode = '{4}'
AND Mue_CostCenterCode = {5}
AND Mue_PayrollGroup = {6}
AND Mue_PayrollType = {7}
AND Mue_EmploymentStatus = {8}"
                , profile, systemID, userCode, CompanyCode, ProfileCode, costCenterColumn, payrollGroupColumn, payrollTypeColumn, empStatusColumn);
            if (reportType)
            {
                query = query.Replace("'", "''");
            }

            return query;
        }

        public DataTable GetEmployeeMasterList(string PayrollDBName, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT Mem_IDNo, Mem_WorkStatus
                                           FROM {0}..M_Employee
                                           ---WHERE LEFT(Mem_WorkStatus, 1) = 'A'", PayrollDBName);
            #endregion
            DataTable dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAccountDetail(string CompanyCode, string CentralProfile, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT Mcd_CodeType, Mcd_Code
                                           FROM M_CodeDtl
                                           WHERE Mcd_RecordStatus = 'A'
                                                AND Mcd_CompanyCode = '{0}'", CompanyCode);
            #endregion
            DataTable dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetDatabaseProfiles(string CentralProfile, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT *
                                           FROM M_Profile
                                           WHERE Mpf_RecordStatus = 'A'
                                                AND Mpf_ProfileType IN ('P','S')");
            #endregion
            DataTable dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetCostCenters(string CompanyCode, string CentralProfile, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT Mcc_CostCenterCode 
                                            FROM M_CostCenter
                                            WHERE Mcc_RecordStatus = 'A'
                                                AND Mcc_CompanyCode = '{0}'", CompanyCode);
            #endregion
            DataTable dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetRecurringAllowance(string CompanyCode, string CentralProfile, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT Min_IncomeCode
                                            , Min_IncomeName
                                            , Min_ApplicablePayCycle
                                            , Min_RecordStatus
                                           FROM M_Income
                                           WHERE Min_CompanyCode = '{0}'
                                                AND Min_IsRecurring = 1
                                                AND Min_IncomeGroup = 'FAL'", CompanyCode);
            #endregion
            DataTable dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public bool CheckExistingStartDateCycle(string EmployeeId, string StartCycyle, string EndCycyle, string IncomeCode, string PayrollDBName, string CentralProfile)
        {
            #region query
            string query = string.Format(@"DECLARE @CurrentPayCycle CHAR(7) = (SELECT Tps_Paycycle 
                                                                              FROM {4}.dbo.T_PaySchedule 
									                                          WHERE Tps_CycleType ='N' 
                                                                                    AND Tps_CycleIndicator ='C' 
									                                                AND Tps_RecordStatus='A')

                                           SELECT Tfa_IDNo
                                           FROM T_EmpFixAllowance
                                           WHERE Tfa_IDNo = '{0}'
                                                AND Tfa_AllowanceCode = '{1}'
                                                AND '{2}' BETWEEN Tfa_StartPayCycle AND ISNULL(Tfa_EndPayCycle, @CurrentPayCycle) 
	                                            --AND ISNULL('{3}', @CurrentPayCycle) >= Tfa_StartPayCycle ", EmployeeId, IncomeCode, StartCycyle, EndCycyle, PayrollDBName);
            #endregion
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                DataSet dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0
                    && !string.IsNullOrEmpty(dsResult.Tables[0].Rows[0][0].ToString()))
                    return true;

            }
            return false;
        }

        public bool CheckExistingStartDateCycle(string EmployeeId, string StartCycyle, string EndCycyle, string IncomeCode, string PayrollDBName, DALHelper dalhelper)
        {
            #region query
            string query = string.Format(@"DECLARE @CurrentPayCycle CHAR(7) = (SELECT Tps_Paycycle 
                                                                              FROM {4}.dbo.T_PaySchedule 
									                                          WHERE Tps_CycleType ='N' 
                                                                                    AND Tps_CycleIndicator ='C' 
									                                                AND Tps_RecordStatus='A')

                                           SELECT Tfa_IDNo
                                           FROM T_EmpFixAllowance
                                           WHERE Tfa_IDNo = '{0}'
                                                AND Tfa_AllowanceCode = '{1}'
                                                AND '{2}' BETWEEN Tfa_StartPayCycle AND ISNULL(Tfa_EndPayCycle, @CurrentPayCycle) ", EmployeeId, IncomeCode, StartCycyle, EndCycyle, PayrollDBName);
            #endregion
            DataSet dsResult = dalhelper.ExecuteDataSet(query);
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0
                && !string.IsNullOrEmpty(dsResult.Tables[0].Rows[0][0].ToString()))
                return true;
            return false;
        }

        public bool CheckFixAllowanceExistingRecord(string EmployeeId, string StartCycyle, string IncomeCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Tfa_IDNo
                                           FROM T_EmpFixAllowance
                                           WHERE Tfa_IDNo = '{0}'
                                                AND Tfa_AllowanceCode = '{1}'
                                                AND Tfa_StartPayCycle = '{2}'", EmployeeId, IncomeCode, StartCycyle);
            #endregion
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                DataSet dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0
                    && !string.IsNullOrEmpty(dsResult.Tables[0].Rows[0][0].ToString()))
                    return true;

            }
            return false;
        }

        public bool CheckFixAllowanceExistingRecord(string EmployeeId, string StartCycyle, string IncomeCode, DALHelper dalhelper)
        {
            #region query
            string query = string.Format(@"SELECT Tfa_IDNo
                                           FROM T_EmpFixAllowance
                                           WHERE Tfa_IDNo = '{0}'
                                                AND Tfa_AllowanceCode = '{1}'
                                                AND Tfa_StartPayCycle = '{2}'", EmployeeId, IncomeCode, StartCycyle);
            #endregion
            DataSet dsResult = dalhelper.ExecuteDataSet(query);
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0
                && !string.IsNullOrEmpty(dsResult.Tables[0].Rows[0][0].ToString()))
                return true;
            return false;
        }

        public void UpdateSeperationDetails(string EmployeeId
                                                , DateTime PayPeriodStart
                                                , DateTime PayPeriodEnd
                                                , string DatabaseProfile
                                                , string Usr_Login
                                                , bool isInactive
                                                , string SeparationNoticeDate
                                                , DALHelper dal)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodStart", PayPeriodStart);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@PayPeriodEnd", PayPeriodEnd);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@isInactive", isInactive);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@SeparationNotice", SeparationNoticeDate);

            #region query
            string query = string.Empty;
            if (isInactive)
            {
                query = string.Format(@"UPDATE {0}..M_Employee
                                                SET Mem_SeparationDate = Case When @isInactive = 'TRUE'
                                                THEN TBL.Tep_StartDate
                                                ELSE NULL
                                                END
                                                , Mem_SeparationCode = TBL.Tep_ReasonCode
                                                , Mem_SeparationSysDate = GETDATE()
                                                , Mem_IsComputedPayroll = 0
                                                , Usr_Login = @Usr_Login
                                                , Mem_SeparationNoticeDate=Case When @isInactive = 'TRUE' and @SeparationNotice <> ''
                                                THEN @SeparationNotice
                                                ELSE NULL
                                                END
                                                , Ludatetime = getdate()
                                            FROM {0}..M_Employee
                                            INNER JOIN
                                            (
                                                Select TOP(1) *
                                                From T_EmpProfile
                                                Where Tep_IDNo = @EmployeeId 
                                                    AND Tep_WorkStatus = 'IN'
                                                Order By Tep_StartDate DESC
                                            )TBL
                                            ON  TBL.Tep_IDNo = Mem_IDNo
		                                    ---WHERE Mem_SeparationDate IS NULL OR Mem_SeparationDate = ''

                                            UPDATE M_Employee
                                                SET Mem_SeparationDate = Case When @isInactive = 'TRUE'
                                                THEN TBL.Tep_StartDate
                                                ELSE NULL
                                                END
                                                , Mem_SeparationCode = TBL.Tep_ReasonCode
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            FROM M_Employee
                                            INNER JOIN
                                            (
                                                Select TOP(1) *
                                                From T_EmpProfile
                                                Where Tep_IDNo = @EmployeeId 
                                                    AND Tep_WorkStatus = 'IN'
                                                Order By Tep_StartDate DESC
                                            )TBL
                                            ON  TBL.Tep_IDNo = Mem_IDNo
		                                    --WHERE Mem_SeparationDate IS NULL OR Mem_SeparationDate = ''

                                            ", DatabaseProfile);
            }
            else
            {
                query = string.Format(@"UPDATE {0}..M_Employee
                                                SET Mem_SeparationDate = Case When @isInactive = 'TRUE'
                                                THEN TBL.Tep_StartDate
                                                ELSE NULL
                                                END
                                                , Mem_SeparationCode = TBL.Tep_ReasonCode
                                                , Mem_SeparationSysDate = GETDATE()
                                                , Mem_IsComputedPayroll = 0
                                                , Usr_Login = @Usr_Login
                                                , Mem_SeparationNoticeDate=Case When @isInactive = 'TRUE' and @SeparationNotice <> ''
                                                THEN @SeparationNotice
                                                ELSE NULL
                                                END
                                                , Ludatetime = getdate()
                                            FROM {0}..M_Employee
                                            INNER JOIN
                                            (
                                                Select TOP(1) *
                                                From T_EmpProfile
                                                Where Tep_IDNo = @EmployeeId 
                                                   AND ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                   AND @PayPeriodEnd >= Tep_StartDate
                                                   AND Tep_WorkStatus = 'IN'
                                                Order By Tep_StartDate DESC
                                            )TBL
                                            ON  TBL.Tep_IDNo = Mem_IDNo
		                                    --WHERE Mem_SeparationDate IS NULL OR Mem_SeparationDate = ''
                                            
                                            UPDATE M_Employee
                                                SET Mem_SeparationDate = Case When @isInactive = 'TRUE'
                                                THEN TBL.Tep_StartDate
                                                ELSE NULL
                                                END
                                                , Mem_SeparationCode = TBL.Tep_ReasonCode
                                                , Usr_Login = @Usr_Login
                                                , Ludatetime = getdate()
                                            FROM M_Employee
                                            INNER JOIN
                                            (
                                                Select TOP(1) *
                                                From T_EmpProfile
                                                Where Tep_IDNo = @EmployeeId 
                                                   AND ISNULL(Tep_EndDate, @PayPeriodStart) >= @PayPeriodStart
                                                   AND @PayPeriodEnd >= Tep_StartDate
                                                   AND Tep_WorkStatus = 'IN'
                                                Order By Tep_StartDate DESC
                                            )TBL
                                            ON  TBL.Tep_IDNo = Mem_IDNo

                                            ", DatabaseProfile);
            } 
            #endregion

            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
        }

        public bool CheckLatestCostCenterEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpCostcenter
                                            WHERE Tcc_IDNo = @EmployeeId
                                            AND Tcc_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckLatestPositionEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpPosition
                                            WHERE Tpo_IDNo = @EmployeeId
                                            AND Tpo_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckLatestPremiumGroupEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpPremiumGroup
                                            WHERE Tpg_IDNo = @EmployeeId
                                            AND Tpg_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckLatestSalaryEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpSalary
                                            WHERE Tsl_IDNo = @EmployeeId
                                            AND Tsl_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckLatestWorkgroupEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpCalendarGroup
                                            WHERE Tcg_IDNo = @EmployeeId
                                            AND Tcg_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckLatestWorkLocationEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpWorkLocation
                                            WHERE Twl_IDNo = @EmployeeId
                                            AND Twl_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckLatestEmploymentStatusEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpEmploymentStatus
                                            WHERE Tes_IDNo = @EmployeeId
                                            AND Tes_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }

        public bool CheckWorkgroupCombination(string Worktype, string Workgroup, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_Calendar
                                            WHERE Tcl_CalendarType = @Worktype
                                                AND Tcl_CalendarGroup = @Workgroup
                                                AND Tcl_RecordStatus = 'A'");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Worktype", Worktype);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@Workgroup", Workgroup);
            #endregion
            DataSet ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return true;
            else return false;
        }

        public bool CheckLatestProfileEffectivity(string EmployeeId, DateTime StartDate, DALHelper dalCentral)
        {
            #region query
            string query = string.Format(@"SELECT * FROM T_EmpProfile
                                            WHERE Tep_IDNo = @EmployeeId
                                            AND Tep_StartDate >= @StartDate");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            int paramInfoCnt = 0;
            paramInfo[paramInfoCnt++] = new ParameterInfo("@EmployeeId", EmployeeId);
            paramInfo[paramInfoCnt++] = new ParameterInfo("@StartDate", StartDate);
            #endregion
            DataSet ds;
            ds = dalCentral.ExecuteDataSet(query, CommandType.Text, paramInfo);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return false;
            else return true;
        }
        public DataTable GetPayPeriodStartDate(string startDate)
        {
            string query = string.Format(@"Select Convert(char(10), Tps_StartCycle, 101) as Tps_StartCycle
                                            From T_PaySchedule
                                            Where Tps_CycleIndicator = 'C'
                                            And Tps_RecordStatus = 'A'
											and '{0}' between Tps_StartCycle and Tps_EndCycle", startDate);
            DataTable dt = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dt;
        }

        public DataTable GetEmployeeSeparationNotice(string employeeID)
        {
            string query = string.Format(@"Select Mem_SeparationNoticeDate
                            From M_Employee
                            Where Mem_IDNo= '{0}'", employeeID);
            DataTable dt = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dt;
        }
    }

    public class TimeLog
    {
        private string TimeIn1 = string.Empty;
        private string TimeIn2 = string.Empty;
        private string TimeOut1 = string.Empty;
        private string TimeOut2 = string.Empty;

        public string TimeLogIn1
        {
            get
            {
                return TimeIn1;
            }
        }

        public string TimeLogIn2
        {
            get
            {
                return TimeIn2;
            }
        }

        public string TimeLogOut1
        {
            get
            {
                return TimeOut1;
            }
        }

        public string TimeLogOut2
        {
            get
            {
                return TimeOut2;
            }
        }

        public TimeLog()
        {
            this.TimeIn1 =
            this.TimeIn2 =
            this.TimeOut1 =
            this.TimeOut2 = "0000";
        }

        public TimeLog(string timein1, string timein2, string timeout1, string timeout2)
        {
            this.TimeIn1 = timein1;
            this.TimeIn2 = timein2;
            this.TimeOut1 = timeout1;
            this.TimeOut2 = timeout2;
        }
    }
}
