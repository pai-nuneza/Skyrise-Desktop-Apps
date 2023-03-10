using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class StandardTimeTypeScheduleBL : Payroll.BLogic.BaseMasterBase
    {

        #region Select Query

        private static string SelectQuery = @"    SELECT [Msc_ScheduleName] as [Schedule Description]
	                                                    ,[Mtd_DeviceName] as [Device Name]
                                                        ,[Msc_DeviceIP] as [Device IP]
                                                        ,DatePart(Year, [Msc_FrYear]) as [From Year]
                                                        ,DatePart(Year, [Msc_ToYear]) as [To Year]
                                                        ,DatePart(Month,[Msc_FrMonth]) as [From Month]
                                                        ,DatePart(Month,[Msc_ToMonth]) as [To Month]
                                                        ,[Msc_Mon] as [Mon]
                                                        ,[Msc_Tue] as [Tue]
                                                        ,[Msc_Wed] as [Wed]
                                                        ,[Msc_Thu] as [Thu]
                                                        ,[Msc_Fri] as [Fri]
                                                        ,[Msc_Sat] as [Sat]
                                                        ,[Msc_Sun] as [Sun]
                                                        ,Substring(Convert(char,[Msc_FrTime],108), 0, 6) as [From Time]
		                                                ,Substring(Convert(char,[Msc_ToTime],108), 0, 6) as [To Time]
                                                        ,[Msc_AllowRFID] as [Allow RFID]
                                                        ,[Msc_AllowFP] as [Allow FP]
                                                        ,[Msc_AllowFace] as [Allow FaceData]
                                                        ,[Msc_AllowPin] as [Allow Pin]
                                                        ,[Msc_ComboMode] as [Combo Mode]
	                                                    ,[Msc_ScheduleCode] as [ScheduleCode]
                                                        ,[Msc_AddedDate] as [Date Added]
                                                        ,[Msc_LastActiveDate] as [Last Active]
                                                    FROM [dbo].[M_Schedule] 
                                                    JOIN [dbo].[M_TerminalDevice2]
	                                                    ON Msc_DeviceIP =  Mtd_DeviceIP
	                                                    AND Msc_RecordStatus = 'A'
                                                        AND Mtd_RecordStatus = 'A'
                                                    ORDER BY Msc_ScheduleCode Desc "; 

        #endregion

        #region Insert Query

        private static string InsertQuery = @"INSERT INTO [dbo].[M_Schedule]
                                                               ([Msc_ScheduleCode]
                                                               ,[Msc_ScheduleName]
                                                               ,[Msc_DeviceIP]
                                                               ,[Msc_FrYear]
                                                               ,[Msc_ToYear]
                                                               ,[Msc_FrMonth]
                                                               ,[Msc_ToMonth]
                                                               ,[Msc_Mon]
                                                               ,[Msc_Tue]
                                                               ,[Msc_Wed]
                                                               ,[Msc_Thu]
                                                               ,[Msc_Fri]
                                                               ,[Msc_Sat]
                                                               ,[Msc_Sun]
                                                               ,[Msc_FrTime]
                                                               ,[Msc_ToTime]
                                                               ,[Msc_AllowRFID]
                                                               ,[Msc_AllowFP]
                                                               ,[Msc_AllowFace]
                                                               ,[Msc_AllowPin]
                                                               ,[Msc_ComboMode]
                                                               ,[Msc_AddedDate]
                                                               ,[Msc_LastActiveDate]
                                                               ,[Msc_RecordStatus]
                                                               ,[Usr_Login]
                                                               ,[LudateTime])
                                                         VALUES
                                                               (   (SELECT ISNULL((SELECT TOP 1 'SC-' + Replicate('0', 12 - LEN(Cast(Isnull((Replace(Msc_ScheduleCode, 'SC-', '') + 1),0) as Char))) 
				                                                                       + Cast(Isnull((Replace(Msc_ScheduleCode, 'SC-', '') + 1),0) as Char) 
                                                                          FROM M_Schedule
                                                                      ORDER BY Msc_ScheduleCode DESC), 'SC-000000000001'))
                                                                    

                                                               ,@ScheduleDesc
                                                               ,@DeviceIP
                                                               ,@YearFrom
                                                               ,@YearTo
                                                               ,@MonthFrom
                                                               ,@MonthTo
                                                               ,@Mon
                                                               ,@Tue
                                                               ,@Wed
                                                               ,@Thu
                                                               ,@Fri
                                                               ,@Sat
                                                               ,@Sun
                                                               ,@TimeFrom
                                                               ,@TimeTo
                                                               ,@AllowRFID
                                                               ,@AllowFP
                                                               ,@AllowFace
                                                               ,@AllowPin
                                                               ,@ComboMode
                                                               ,Getdate()
                                                               ,@DateLastActive
                                                               ,'A'
                                                               ,@Usr_Login
                                                               ,Getdate())";


        #endregion

        #region UpdateQuery

        private static string UpdateQuery = @"  UPDATE [dbo].[M_Schedule]
                                                         SET [Msc_LastActiveDate] = Getdate()
                                                            ,[Msc_RecordStatus] = '{0}'
                                                            ,[Usr_Login] = @Usr_Login
                                                            ,[LudateTime] = Getdate()
                                                        WHERE Msc_ScheduleCode = @ScheduleCode";

        #endregion

        #region IsExist Query

        private static string IsExistQuery = @" SELECT Msc_ScheduleName as [Desc],
	                                                   Msc_DeviceIP as [DeviceIP]
                                                  FROM M_Schedule
                                                 WHERE Msc_DeviceIP = @DeviceIP
                                                   AND (@YearFrom >= DATEPART(Year, Msc_FrYear) OR @YearTo <= DATEPART(Year, Msc_ToYear))
                                                   AND (@MonthFrom >= DATEPART(Month, Msc_FrMonth) OR @MonthTo <= DATEPART(Month, Msc_ToMonth))
                                                   AND (   Msc_Mon = @Mon
		                                                OR Msc_Tue = @Tue
		                                                OR Msc_Wed = @Wed
		                                                OR Msc_Thu = @Thu
		                                                OR Msc_Fri = @Fri
		                                                OR Msc_Sat = @Sat
		                                                OR Msc_Sun = @Sun
		                                                )
                                                   AND (@HourFrom >= DATEPART(Hour, Msc_FrTime) OR @HourTo <= DATEPART(Hour, Msc_ToTime))
                                                   AND (@MinFrom >= DATEPART(Minute, Msc_FrTime) OR @MinTo <= DATEPART(Minute, Msc_ToTime))
                                                   AND Msc_RecordStatus = 'A'
                                                   AND (Msc_ScheduleCode <> CASE WHEN 1 = @IsUpdate THEN @ScheduleCode ELSE '' END)";

        #endregion


        public DataSet SetupStandardTimeTypeScheduleGrid()
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SelectQuery, CommandType.Text);
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds;
        }

        private bool IsExist(DALHelper dal, ParameterInfo[] param)
        { 
            bool ret = false;
            try
            {
                DataTable dt = dal.ExecuteDataSet(IsExistQuery, CommandType.Text, param).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    ret = true;
                    CommonProcedures.showMessageInformation("Please reset schedule.\nShedule range already exist.\nDescription : " + dr["Desc"].ToString() + "\nDevice : " + dr["DeviceIP"].ToString());
                    break;
                }
            }
            catch
            {
                
            }
            return ret;
        }

        public bool AddNew(ParameterInfo[] param)
        {
            bool Result = false;

            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    if (IsExist(dal, param))
                    {
                        //Do nothing
                    }
                    else
                    {
                        dal.ExecuteNonQuery(InsertQuery, CommandType.Text, param);
                        Result = true;
                        CommonProcedures.showMessageInformation("Successfully inserted new schedule.");
                    }
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return Result;
        }

        public void Modify(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    if (IsExist(dal, param))
                    {
                        //Do nothing
                    }
                    else
                    {
                        dal.BeginTransaction();
                        dal.ExecuteNonQuery(InsertQuery, CommandType.Text, param);
                        dal.ExecuteNonQuery(String.Format(UpdateQuery, "I"), CommandType.Text, param);
                        dal.CommitTransaction();
                        CommonProcedures.showMessageInformation("Successfully updated schedule.");
                    }
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

        public void Delete(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.ExecuteNonQuery(String.Format(UpdateQuery, "C"), CommandType.Text, param);
                    CommonProcedures.showMessageInformation("Successfully deleted.");
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }
    }
}
