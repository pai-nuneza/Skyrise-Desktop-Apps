using System;
using System.Collections.Generic;
using System.Text;
using CommonPostingLibrary;
using Posting.DAL;
using System.Data;

namespace Posting.BLogic
{
    public class WorkLocationBL : BaseBL
    {
        #region <Override Functions>

        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string Ewl_EmployeeID, string Ewl_EffectivityDate)
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

        #region <Function Defined>

        public DataSet FetchRecord(string Ewl_EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select CONVERT(char(10), Ewl_EffectivityDate, 101) as Ewl_EffectivityDate
	                                  , Adt_AccountDesc as WorkLocation
                                      , Ewl_EmployeeID as Emt_EmployeeID
                                From T_EmployeeWorkLocation Inner Join T_AccountDetail
                                    on Ewl_LocationCode = Adt_AccountCode
                                Where Ewl_EmployeeID = @Ewl_EmployeeID
                                and Adt_AccountType = 'ZIPCODE'
                                and Adt_Status = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetLatestEffectDate(string processdate, string Ewl_EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Convert(char(10), MAX(Ewl_EffectivityDate), 101) as Ewl_EffectivityDate
                                 From T_EmployeeWorkLocation 
                                 Where Ewl_EffectivityDate <= @processdate
		                            and Ewl_EmployeeID = @Ewl_EmployeeID
		                            --and (Select Ppm_EndCycle From T_PayPeriodMaster
                                    --    Where Ppm_CycleIndicator = 'C'
                                    --    And Ppm_Status = 'A' ) <= @processdate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@processdate", processdate);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetLatestEffectDate(string processdate, string Ewl_EmployeeID, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Convert(char(10), MAX(Ewl_EffectivityDate), 101) as Ewl_EffectivityDate
                                 From T_EmployeeWorkLocation 
                                 Where Ewl_EffectivityDate <= @processdate
		                            and Ewl_EmployeeID = @Ewl_EmployeeID
		                            --and (Select Ppm_EndCycle From T_PayPeriodMaster
                                    --    Where Ppm_CycleIndicator = 'C'
                                    --    And Ppm_Status = 'A' ) <= @processdate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@processdate", processdate);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }


        public string GetEffectiveLocationCode(string processdate, string Ewl_EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Ewl_LocationCode From T_EmployeeWorkLocation
                                Where Ewl_EmployeeID = @Ewl_EmployeeID
                                And Ewl_EffectivityDate = @Ewl_EffectivityDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@Ewl_EffectivityDate", processdate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["Ewl_LocationCode"].ToString();
            else
                return string.Empty;
        }

        public string GetEffectiveLocationCode(string processdate, string Ewl_EmployeeID, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Ewl_LocationCode From T_EmployeeWorkLocation
                                Where Ewl_EmployeeID = @Ewl_EmployeeID
                                And Ewl_EffectivityDate = @Ewl_EffectivityDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@Ewl_EffectivityDate", processdate);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["Ewl_LocationCode"].ToString();
            else
                return string.Empty;
        }

        public void DeleteEmpWorkLocRec(string Ewl_EmployeeID, string Ewl_EffectivityDate)
        {
            #region query
            string sqlGetLocation = @"Select convert(char(10),Ewl_EffectivityDate,101),Ewl_LocationCode 
                                    from T_EmployeeWorkLocation 
			                            where Ewl_EmployeeID = @Ewl_EmployeeID  and Ewl_EffectivityDate <> @Ewl_EffectivityDate
		                            order by  convert(char(10),Ewl_EffectivityDate,101) asc";
            string qString = @" 
                                Delete From T_EmployeeWorkLocation
                                    Where Ewl_EmployeeID = @Ewl_EmployeeID
                                        And Ewl_EffectivityDate = @Ewl_EffectivityDate
                                update t_employeelogledger set ell_locationcode = ''
                                        where ell_processdate >= @Ewl_EffectivityDate and ell_employeeid=@Ewl_EmployeeID
                                ";
            string sqlUpdateLedgerLoc = @"update t_employeelogledger set ell_locationcode = '{2}'
                                            where convert(char(10),ell_processdate,101) >= '{0}' and ell_employeeid='{1}'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@Ewl_EffectivityDate", Ewl_EffectivityDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {                   
                    dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    DataSet ds = dal.ExecuteDataSet(sqlGetLocation, CommandType.Text, paramInfo);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            dal.ExecuteNonQuery(string.Format(sqlUpdateLedgerLoc, ds.Tables[0].Rows[i][0].ToString().Trim(), Ewl_EmployeeID.Trim(), ds.Tables[0].Rows[i][1].ToString().Trim()), CommandType.Text);
                        }
                    }
                    dal.CommitTransactionSnapshot();
                }
                catch
                {
                    dal.RollBackTransactionSnapshot();

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            //try
            //{
            //    dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            //}
            //catch
            //{

            //}
        }

        public bool CheckIfRecExists(string Ewl_EmployeeID, string Ewl_EffectivityDate)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select  *
                                From T_EmployeeWorkLocation
                                Where Ewl_EmployeeID = @Ewl_EmployeeID
                                And Ewl_EffectivityDate = @Ewl_EffectivityDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@Ewl_EffectivityDate", Ewl_EffectivityDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void InsertEmpWorkLocRec(DataRow row)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", row["Ewl_EmployeeID"]);
            paramInfo[1] = new ParameterInfo("@Ewl_EffectivityDate", row["Ewl_EffectivityDate"]);
            paramInfo[2] = new ParameterInfo("@Ewl_LocationCode", row["Ewl_LocationCode"]);
            paramInfo[3] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region qstring
            string sqlGetLocation = @"Select convert(char(10),Ewl_EffectivityDate,101),Ewl_LocationCode 
                                    from T_EmployeeWorkLocation 
			                            where Ewl_EmployeeID = @Ewl_EmployeeID  
		                            order by  convert(char(10),Ewl_EffectivityDate,101) asc";
            string sqlstring = @"INSERT INTO T_EmployeeWorkLocation
                                           (Ewl_EmployeeID
                                           ,Ewl_EffectivityDate
                                           ,Ewl_LocationCode
                                           ,Usr_Login
                                           ,Ludatetime)
                                        VALUES
                                           (@Ewl_EmployeeID
                                           ,@Ewl_EffectivityDate
                                           ,@Ewl_LocationCode
                                           ,@Usr_Login
                                           ,Getdate())";
            string sqlUpdateLedgerLoc = @"update t_employeelogledger set ell_locationcode = '{2}'
                                            where convert(char(10),ell_processdate,101) >= '{0}' and ell_employeeid='{1}'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    dal.ExecuteNonQuery(sqlstring, CommandType.Text, paramInfo);
                    DataSet ds = dal.ExecuteDataSet(sqlGetLocation, CommandType.Text, paramInfo);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            dal.ExecuteNonQuery(string.Format(sqlUpdateLedgerLoc, ds.Tables[0].Rows[i][0].ToString().Trim(), row["Ewl_EmployeeID"].ToString().Trim(), ds.Tables[0].Rows[i][1].ToString().Trim()), CommandType.Text);
                        }
                    }
                    dal.CommitTransactionSnapshot();
                }
                catch 
                {
                    dal.RollBackTransactionSnapshot();
                    
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            //try
            //{
            //    dal.ExecuteNonQuery(sqlstring, CommandType.Text, paramInfo);
            //}
            //catch
            //{
            //}
        }

        public DataTable PopulateEmployeeDataList(string IdNum)
        {
            string LocationCode = string.Empty;
            DataTable EmployeeDataList = new DataTable();

            if (IdNum != string.Empty)
            {
                object[] tempobj = new object[2];

                EmployeeDataList.Columns.Add("Emt_EmployeeID");
                EmployeeDataList.Columns.Add("Emt_LocationCode");

                tempobj[0] = IdNum;
                tempobj[1] = LocationCode.Trim();

                EmployeeDataList.Rows.Add(tempobj);
            }

            return EmployeeDataList;
        }

        public string GetLocationCode(string processdate, string IdNum)
        {
            string LatestEffectDate = string.Empty;
            string LocationCode = string.Empty;
            
            if (IdNum != string.Empty)
            {
                LatestEffectDate = this.GetLatestEffectDate(processdate, IdNum);
                if (LatestEffectDate != string.Empty)
                {
                    LocationCode = this.GetEffectiveLocationCode(LatestEffectDate, IdNum);
                }
            }

            return LocationCode;
        }

        public bool EmpWorkLocRecExists(string Ewl_EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select  *
                                From T_EmployeeWorkLocation
                                Where Ewl_EmployeeID = @Ewl_EmployeeID";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }


        public DataTable GetPrevEmployee(string EmployeeId)
        {
            string EmployeeCondition = "";
            if (EmployeeId.Length > 0)
                EmployeeCondition = " and Emt_EmployeeId < '" + EmployeeId + "'";

            string query = string.Format(@" select top(1) Emt_EmployeeId, Emt_LastName, Emt_FirstName, left(Emt_MiddleName,1) as Emt_MiddleName
                                            ,Emt_CostCenterCode as [CostCenterCode]
                                            from T_EmployeeMaster
                            LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Emt_CostCenterCode
                            LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                            LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                            LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                            LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                            LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                            WHERE  Emt_CostCenterCode IN  ( SELECT Cct_CostCenterCode 
                                                            FROM t_Costcenter
                                    WHERE Cct_status = 'A' and LEFT(emt_jobstatus,1) = 'A' {0})
                            order by Emt_EmployeeId desc", EmployeeCondition);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetNextEmployee(string EmployeeId)
        {
            string EmployeeCondition = "";
            if (EmployeeId.Length > 0)
                EmployeeCondition = " and Emt_EmployeeId > '" + EmployeeId + "'";

            string query = string.Format(@" select top(1) Emt_EmployeeId, 
                            Emt_LastName, 
                            Emt_FirstName, 
                            left(Emt_MiddleName,1) as Emt_MiddleName
                            ,Emt_CostCenterCode as [CostCenterCode]
                            from T_EmployeeMaster
                            LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Emt_CostCenterCode
                            LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                            LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                            LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                            LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                            LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                            WHERE  Emt_CostCenterCode IN  ( SELECT Cct_CostCenterCode 
                                                        FROM t_Costcenter
                                                        WHERE Cct_status = 'A' and LEFT(emt_jobstatus,1) = 'A' {0})", EmployeeCondition);
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

    }
}
