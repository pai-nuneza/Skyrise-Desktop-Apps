using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class CardRegistrationBL : Payroll.BLogic.BaseMasterBase
    {

        #region Select Query

        private static string SelectQuery = @"    SELECT [Trf_EmployeeID] AS [Device ID]
                                                         ,CASE WHEN IsNull([Trf_EmployeeIDMapping], '') = '' 
			                                                           THEN 
					                                                        CASE WHEN IsNull([Tel_LastName], '') = '' THEN ' UNMAPPED' ELSE [Tel_IDNo] END
			                                                           ELSE
					                                                        [Trf_EmployeeIDMapping]
			                                                           END AS [Employee ID]
                                                         ,CASE WHEN IsNull([Tel_LastName], '') = '' 
						                                       THEN ' UNMAPPED'
						                                       ELSE [Tel_LastName] + ', ' + [Tel_FirstName]
					                                      END [Employee Name]
                                                          ,[Trf_RFID] AS [Card Number]
				                                    FROM [{0}] MAIN
                                                LEFT JOIN T_EmpLog
                                                       ON (ISNUMERIC(Tel_IDNo) = 1 AND CONVERT(BIGINT,Trf_employeeid) = CONVERT(BIGINT,Tel_IDNo))
                                                       OR (Trf_EmployeeIDMapping = Tel_IDNo)
                                                    WHERE [Trf_FingerIndex] = 0
                                                 ORDER BY [Employee Name], [Device ID]";

        #endregion

        #region Insert Query

        private static string InsertQuery = @"  INSERT INTO [dbo].[{0}]
                                                           ([Trf_EmployeeID]
                                                           ,[Trf_RFID]
                                                           ,[Trf_FingerIndex]
                                                           ,[Trf_Template]
                                                           ,[Trf_Privilege]
                                                           ,[Trf_Password]
                                                           ,[Trf_Enabled]
                                                           ,[Trf_Flag]
                                                           ,[ludatetime]
                                                           ,[Usr_Login]
                                                           ,[Trf_EmployeeIDMapping]
                                                           ,[Trf_FaceData])
                                                     VALUES
                                                           (@DeviceID
                                                           ,@RFID
                                                           ,'0'
                                                           ,NULL
                                                           ,'0'
                                                           ,''
                                                           ,'TRUE'
                                                           ,'TRUE'
                                                           ,GETDATE()
                                                           ,@Usr_Login
                                                           ,@EmployeeIDMapping
                                                           ,NULL)";


        #endregion

        #region UpdateQuery

        private static string UpdateQuery = @"  UPDATE [dbo].[{0}]
                                                         SET [Trf_RFID] = @RFID
                                                            ,[Usr_Login] = @Usr_Login
                                                            ,[LudateTime] = Getdate()
                                                        WHERE Trf_employeeid = @DeviceID
                                                          AND [Trf_FingerIndex] = 0";

        #endregion

        #region IsExist Query

        private static string IsExistQuery = @" SELECT 1
                                                  FROM [{0}] 
                                                 WHERE Trf_EmployeeID = @DeviceID";

        private static string IsRFIDExistQuery = @"   SELECT 1
                                                        FROM [{0}] 
                                                        WHERE Trf_EmployeeID <> @DeviceID
                                                          AND Trf_RFID = @RFID
                                                          AND Trf_RFID = (CASE WHEN Trf_RFID = '0' THEN 'XXXXXXXXXX' ELSE Trf_RFID END)";
        #endregion


        public DataSet SetupRegistrationGrid(string TableName)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(SelectQuery, TableName), CommandType.Text);
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

        public DataSet SetupMasterGrid()
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(@"  SELECT Tel_IDNo [Employee ID]
	                                                  ,Tel_LastName + ', ' + Tel_FirstName [Employee Name]
                                                  FROM T_EmpLog
                                                ORDER BY [Employee Name]", CommandType.Text);
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

        private bool IsExist(DALHelper dal, ParameterInfo[] param, string TableName, bool IDOnly)
        {
            bool ret = false;
            try
            {
                DataTable dt = dal.ExecuteDataSet(string.Format(((IDOnly) ? IsExistQuery : IsRFIDExistQuery), TableName), CommandType.Text, param).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    ret = true;
                    CommonProcedures.showMessageInformation("Registered ID already exist.");
                    break;
                }
            }
            catch
            {

            }
            return ret;
        }

        public bool AddNew(ParameterInfo[] param, string TableName)
        {
            bool Result = false;

            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    if (IsExist(dal, param, TableName, true))
                    {
                        //Do nothing
                    }
                    else
                    {
                        dal.ExecuteNonQuery(string.Format(InsertQuery, TableName), CommandType.Text, param);
                        Result = true;
                        //CommonProcedures.showMessageInformation("Successfully inserted new schedule.");
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

        public bool Modify(ParameterInfo[] param, string TableName)
        {
            bool _return = false;

            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    if (IsExist(dal, param, TableName, false))
                    {
                        //Do nothing
                    }
                    else
                    {
                        dal.BeginTransaction();
                        dal.ExecuteNonQuery(String.Format(UpdateQuery, TableName), CommandType.Text, param);
                        dal.CommitTransaction();
                        _return = true;
                        //CommonProcedures.showMessageInformation("Successfully updated schedule.");
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

            return _return;
        }

        public void Delete(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.ExecuteNonQuery("", CommandType.Text, param);
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
