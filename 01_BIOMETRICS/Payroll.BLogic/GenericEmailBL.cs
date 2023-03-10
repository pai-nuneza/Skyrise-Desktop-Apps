using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using System.Data;
using CommonLibrary;


namespace Payroll.BLogic
{
    public class GenericEmailBL : BaseMasterBase
    {
        #region Variables

        #endregion



        public DataSet InitializeEmailGrid()
        {
            string query = @"
                select 
	                Esh_ServiceCode [Service Code]
	                ,Esh_ServiceDesc [Description]
	                ,case when Esh_Status = 'A'
	                then 'ACTIVE'
	                else 'CANCELLED' end [Status]
                from T_EmailserviceHeader
            ";
            DataSet ds = ExecuteDataSetQuery2(query);

            return ds;
        }

        public void InsertNewEmailSettings(string RecurrencePattern
            , ParameterInfo[] param
            , string sqlEmailServiceDetail
            , string sqlSchedulerserviceTimeSetting
            , string sqlProfile
            , string user)
        {

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    string InsertQuery = string.Empty;
                    
                    #region Insert Email Service Header

                    if (RecurrencePattern == "D")
                    {
                        #region
                        InsertQuery = string.Format(@"
                            insert into T_EmailServiceHeader
                            (
	                            Esh_ServiceCode
	                            ,Esh_ServiceDesc
	                            ,Esh_ScheduleType
	                            ,Esh_Interval
	                            ,Esh_Runtime
	                            ,Esh_TimeSelection
	                            ,Esh_Subject
                                ,Esh_SubjectFormula
	                            ,Esh_MessageBody
	                            ,Esh_MessageFormula
	                            ,Esh_ResultDisplayAs
	                            ,Esh_Status
	                            ,Usr_Login
	                            ,Ludatetime
                                ,Esh_From
                                ,Esh_SpecialInterval
                                ,Esh_SpecialCondition
                                ,Esh_InitialRun
                                ,Esh_NextRun
                                ,Esh_EmailNoDetails
                                ,Esh_TimeSetting
                            )
                            values
                            (

	                            @Esh_ServiceCode
	                            ,@Esh_ServiceDesc
	                            ,@Esh_ScheduleType
	                            ,@Esh_Interval
	                            ,@Esh_Runtime
	                            ,@Esh_TimeSelection
	                            ,@Esh_Subject
                                ,@Esh_SubjectFormula
	                            ,@Esh_MessageBody
	                            ,@Esh_MessageFormula
	                            ,@Esh_ResultDisplayAs
	                            ,@Esh_Status
	                            ,'{0}'
	                            ,GETDATE()
                                ,@Esh_From
                                ,@Esh_SpecialInterval
                                ,@Esh_SpecialCondition
                                ,@Esh_InitialRun
                                ,@Esh_InitialRun
                                ,@Esh_EmailNoDetails
                                ,@Esh_TimeSetting
                            )
                        ", user);
                        #endregion
                    }
                    else if (RecurrencePattern == "W")
                    {
                        #region

                        InsertQuery = string.Format(
                            @"
                            insert into T_EmailServiceHeader
                            (
	                            Esh_ServiceCode
	                            ,Esh_ServiceDesc
	                            ,Esh_ScheduleType
	                            ,Esh_Interval
	                            ,Esh_Runtime
	                            ,Esh_TimeSelection
	                            ,Esh_Mon
	                            ,Esh_Tue
	                            ,Esh_Wed
	                            ,Esh_Thur
	                            ,Esh_Fri
	                            ,Esh_Sat
	                            ,Esh_Sun	
	                            ,Esh_Subject
                                ,Esh_SubjectFormula
	                            ,Esh_MessageBody
	                            ,Esh_MessageFormula
	                            ,Esh_ResultDisplayAs
	                            ,Esh_Status
	                            ,Usr_Login
	                            ,Ludatetime
                                ,Esh_From
                                ,Esh_SpecialInterval
                                ,Esh_SpecialCondition
                                ,Esh_InitialRun
                                ,Esh_NextRun
                                ,Esh_EmailNoDetails
                                ,Esh_TimeSetting
                            )
                            values
                            (

	                            @Esh_ServiceCode
	                            ,@Esh_ServiceDesc
	                            ,@Esh_ScheduleType
	                            ,@Esh_Interval
	                            ,@Esh_Runtime
	                            ,@Esh_TimeSelection
	                            ,@Esh_Mon
	                            ,@Esh_Tue
	                            ,@Esh_Wed
	                            ,@Esh_Thur
	                            ,@Esh_Fri
	                            ,@Esh_Sat
	                            ,@Esh_Sun	
	                            ,@Esh_Subject
                                ,@Esh_SubjectFormula
	                            ,@Esh_MessageBody
	                            ,@Esh_MessageFormula
	                            ,@Esh_ResultDisplayAs
	                            ,@Esh_Status
	                            ,'{0}'
	                            ,GETDATE()
                                ,@Esh_From
                                ,@Esh_SpecialInterval
                                ,@Esh_SpecialCondition
                                ,@Esh_InitialRun
                                ,@Esh_InitialRun
                                ,@Esh_EmailNoDetails
                                ,@Esh_TimeSetting
                            )
                            ", user
                            );
                            
                        #endregion
                    }
                    else if (RecurrencePattern == "M")
                    {
                        #region

                        InsertQuery = string.Format(
                            @"
                        insert into T_EmailServiceHeader
                        (
                            Esh_ServiceCode
                            ,Esh_ServiceDesc
                            ,Esh_ScheduleType
                            ,Esh_Interval
                            ,Esh_Runtime
                            ,Esh_TimeSelection
                            ,Esh_MonthDay
                            ,Esh_NthDay
                            ,Esh_DOW
                            ,Esh_Subject
                            ,Esh_SubjectFormula
                            ,Esh_MessageBody
                            ,Esh_MessageFormula
                            ,Esh_ResultDisplayAs
                            ,Esh_Status
                            ,Usr_Login
                            ,Ludatetime
                                ,Esh_From
                                ,Esh_SpecialInterval
                                ,Esh_SpecialCondition
                                ,Esh_InitialRun
                                ,Esh_NextRun
                                ,Esh_EmailNoDetails
                                ,Esh_TimeSetting
                        )
                        values
                        (

                            @Esh_ServiceCode
                            ,@Esh_ServiceDesc
                            ,@Esh_ScheduleType
                            ,@Esh_Interval
                            ,@Esh_Runtime
                            ,@Esh_TimeSelection
                            ,@Esh_MonthDay
                            ,@Esh_NthDay
                            ,@Esh_DOW
                            ,@Esh_Subject
                            ,@Esh_SubjectFormula
                            ,@Esh_MessageBody
                            ,@Esh_MessageFormula
                            ,@Esh_ResultDisplayAs
                            ,@Esh_Status
                            ,'{0}'
                            ,GETDATE()
                                ,@Esh_From
                                ,@Esh_SpecialInterval
                                ,@Esh_SpecialCondition
                                ,@Esh_InitialRun
                                ,@Esh_InitialRun
                                ,@Esh_EmailNoDetails
                                ,@Esh_TimeSetting
                        )
                            ", user
                            );

                        #endregion
                    }

                    dal.ExecuteNonQuery(InsertQuery, CommandType.Text, param);

                    #endregion

                    #region Insert Email Service Setting

                    dal.ExecuteNonQuery(@"
                        insert into T_EmailServiceDetail
                    " + sqlEmailServiceDetail);

                    #endregion

                    #region Insert Time schedule

                    if (param[5].Value.ToString() == "1")
                    {
                        dal.ExecuteNonQuery(sqlSchedulerserviceTimeSetting, CommandType.Text);
                    }

                    #endregion

                    #region Insert Profile
                    dal.ExecuteNonQuery(sqlProfile, CommandType.Text);

                    #endregion

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully inserted new record!");
                }
                catch(Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError(er.Message);   
                }
                finally
                {
                    dal.CloseDB();
                }
            }

        }

        public void UpdateEmailSettings(string RecurrencePattern
            , ParameterInfo[] param
            , string sqlEmailServiceDetail
            , string sqlSchedulerserviceTimeSetting
            , string sqlSchedulerserviceTimeSettingInsert
            , string sqlProfile
            , string user
            , string ServiceCode
            , bool isTimeSettingChanged)
        {

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    string InsertQuery = string.Empty;

                    #region Update Email Service Header

                    if (RecurrencePattern == "D")
                    {
                        #region
                        InsertQuery = string.Format(@"
                            update T_EmailServiceHeader
                            set
	                             Esh_ServiceDesc = @Esh_ServiceDesc
	                            ,Esh_ScheduleType = @Esh_ScheduleType 	                            
                                ,Esh_Interval = @Esh_Interval
	                            ,Esh_Runtime = @Esh_Runtime
	                            ,Esh_TimeSelection = @Esh_TimeSelection
	                            ,Esh_Subject = @Esh_Subject
                                ,Esh_SubjectFormula = @Esh_SubjectFormula
	                            ,Esh_MessageBody = @Esh_MessageBody
	                            ,Esh_MessageFormula = @Esh_MessageFormula
	                            ,Esh_ResultDisplayAs = @Esh_ResultDisplayAs
	                            ,Esh_Status = @Esh_Status
	                            ,Usr_Login = '{0}'
	                            ,Ludatetime = GETDATE()
                                ,Esh_From = @Esh_From
                                ,Esh_SpecialInterval = @Esh_SpecialInterval
                                ,Esh_SpecialCondition = @Esh_SpecialCondition
                                ,Esh_InitialRun = @Esh_InitialRun
                                @ISTIMESETTINGCHANGED
                                ,Esh_EmailNoDetails = @Esh_EmailNoDetails
                                ,Esh_TimeSetting = @Esh_TimeSetting
                            where 
                                Esh_ServiceCode = @Esh_ServiceCode
                        ", user);
                        #endregion
                    }
                    else if (RecurrencePattern == "W")
                    {
                        #region

                        InsertQuery = string.Format(
                            @"
                            update T_EmailServiceHeader
                            set
	                             Esh_ServiceDesc = @Esh_ServiceDesc
	                            ,Esh_ScheduleType = @Esh_ScheduleType
	                            ,Esh_Interval = @Esh_Interval
	                            ,Esh_Runtime = @Esh_Runtime
	                            ,Esh_TimeSelection = @Esh_TimeSelection
	                            ,Esh_Mon = @Esh_Mon
	                            ,Esh_Tue = @Esh_Tue
	                            ,Esh_Wed = @Esh_Wed
	                            ,Esh_Thur = @Esh_Thur
	                            ,Esh_Fri = @Esh_Fri
	                            ,Esh_Sat = @Esh_Sat
	                            ,Esh_Sun = @Esh_Sun
	                            ,Esh_Subject = @Esh_Subject
                                ,Esh_SubjectFormula = @Esh_SubjectFormula
	                            ,Esh_MessageBody = @Esh_MessageBody
	                            ,Esh_MessageFormula = @Esh_MessageFormula
	                            ,Esh_ResultDisplayAs = @Esh_ResultDisplayAs
	                            ,Esh_Status = @Esh_Status
	                            ,Usr_Login = '{0}'
	                            ,Ludatetime = GETDATE()
                                ,Esh_From = @Esh_From
                                ,Esh_SpecialInterval = @Esh_SpecialInterval
                                ,Esh_SpecialCondition = @Esh_SpecialCondition
                                ,Esh_InitialRun = @Esh_InitialRun
                                @ISTIMESETTINGCHANGED
                                ,Esh_EmailNoDetails = @Esh_EmailNoDetails
                                ,Esh_TimeSetting = @Esh_TimeSetting
                            where Esh_ServiceCode = @Esh_ServiceCode
                            ", user
                            );

                        #endregion
                    }
                    else if (RecurrencePattern == "M")
                    {
                        #region

                        InsertQuery = string.Format(
                            @"
                        update T_EmailServiceHeader
                        set 
                             Esh_ServiceDesc = @Esh_ServiceDesc
                            ,Esh_ScheduleType = @Esh_ScheduleType
                            ,Esh_Interval = @Esh_Interval
                            ,Esh_Runtime = @Esh_Runtime
                            ,Esh_TimeSelection = @Esh_TimeSelection
                            ,Esh_MonthDay = @Esh_MonthDay
                            ,Esh_NthDay = @Esh_NthDay
                            ,Esh_DOW = @Esh_DOW
                            ,Esh_Subject = @Esh_Subject
                            ,Esh_SubjectFormula = @Esh_SubjectFormula
                            ,Esh_MessageBody = @Esh_MessageBody
                            ,Esh_MessageFormula = @Esh_MessageFormula
                            ,Esh_ResultDisplayAs = @Esh_ResultDisplayAs
                            ,Esh_Status = @Esh_Status
                            ,Usr_Login = '{0}'
                            ,Ludatetime = GETDATE()
                            ,Esh_From = @Esh_From
                            ,Esh_SpecialInterval = @Esh_SpecialInterval
                            ,Esh_SpecialCondition = @Esh_SpecialCondition
                            ,Esh_InitialRun = @Esh_InitialRun
                            @ISTIMESETTINGCHANGED
                            ,Esh_EmailNoDetails = @Esh_EmailNoDetails
                            ,Esh_TimeSetting = @Esh_TimeSetting

                        where Esh_ServiceCode = @Esh_ServiceCode
                            ", user
                            );

                        #endregion
                    }
                    if (isTimeSettingChanged)
                    {
                        InsertQuery = InsertQuery.Replace("@ISTIMESETTINGCHANGED", ",Esh_NextRun = @Esh_InitialRun");
                    }
                    else
                    {
                        InsertQuery = InsertQuery.Replace("@ISTIMESETTINGCHANGED", "");
                    }

                    dal.ExecuteNonQuery(InsertQuery, CommandType.Text, param);

                    #endregion

                    #region Update Email Service Setting

                    dal.ExecuteNonQuery(
                        @"
                            delete from T_EmailServiceDetail
                            where Esd_ServiceCode = @Esh_ServiceCode
                        ", CommandType.Text
                         , param
                        );

                    dal.ExecuteNonQuery(@"
                        insert into T_EmailServiceDetail
                    " + sqlEmailServiceDetail);

                    #endregion

                    #region Update Time schedule

                    if (param[5].Value.ToString() == "1")
                    {
                        if (dal.ExecuteDataSet(
                            string.Format(
                            @"
                            select 
	                            Sst_ServiceCode
                            from T_SchedulerServiceTimeSetting
                            where Sst_ServiceCode = '{0}'
                            ", ServiceCode)
                            ).Tables[0].Rows.Count > 0)
                        {
                            dal.ExecuteNonQuery(sqlSchedulerserviceTimeSetting, CommandType.Text);
                        }
                        else
                        {
                            dal.ExecuteNonQuery(sqlSchedulerserviceTimeSettingInsert, CommandType.Text);
                        }
                    }

                    #endregion

                    #region Update Profile

                    dal.ExecuteNonQuery(sqlProfile, CommandType.Text);

                    #endregion

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully updated record!");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

        }

        public bool IsDuplicatePrimarykey(string serviceCode)
        {
            bool ret = false;
            DataSet ds = ExecuteDataSetQuery2(
                string.Format(
                @"
                    select * from T_EmailserviceHeader
                    where Esh_serviceCode = '{0}'
                ",serviceCode
                )
                );
            if (ds.Tables[0].Rows.Count > 0)
                ret = true;
            else
                ret = false;
            return ret;
        }

        public DataSet ExecuteDataSetQuery2(string query)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                }
                catch (Exception er)
                {
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }

            }
            return ds;
        }

        public bool ExecuteNonQuery2(string query)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(query, CommandType.Text);

                    dal.CommitTransaction();
                    ret = true;
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    ret = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }
    }
}
