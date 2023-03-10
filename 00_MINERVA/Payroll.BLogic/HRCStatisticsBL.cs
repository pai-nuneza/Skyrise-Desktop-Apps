using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Collections;

namespace Payroll.BLogic
{
    /// <summary>
    /// Description : HRC Statistics BL
    /// </summary>
    /// 

    public enum StatisticsType
    {
        COST_CENTER = 0,
        POSITION = 1,
        RESIGNEES = 2,
        NEW_HIREES = 3,
        EDUCATION = 4,
        CIVIL_STATUS = 5,
        DEMOGRAPHICS = 6,
        SALARY = 7, //must be at seventh index always
        LATE = 8,
        UNDERTIME = 9,
        ABSENCE = 10,
        LEAVE = 11,
        OVERTIME = 12,
        MANPOWER = 13,
        VIOLATION = 14,
        TRAINING = 15,
        TRAINING_BUDGET = 16,
        CPH_MANPOWER = 17,
        MANPOWER_STATUS = 18,
        TARDINESS = 19
    }

    public enum StatisticsView
    {
        SUMMARY = 0,
        DETAIL = 1,
        GRAPH = 2
    }

    public class HRCStatisticsBL : BaseBL
    {
        DALHelper dal = new DALHelper();

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

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataTable GetClassification()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'POSCLASS'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetJobStatus()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'WORKSTAT'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmploymentStatus()
        {
            string query = @"SELECT [Mcd_Code] AS 'Employment Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'EMPSTAT'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetGender()
        {
            string query = @"SELECT 'M' AS 'Status Code'
                                   ,'MALE' AS 'Description'
                             UNION
                             SELECT 'F' AS 'Status Code'
                                   ,'FEMALE' AS 'Description'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetCostCenter()
        {
            string query = @"SELECT Mcc_CostCenterCode AS 'Status Code'
                                    , dbo.Udf_DisplayCostCenterName(Mcc_CostCenterCode) AS 'Description'
                            FROM M_CostCenter";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetPosition()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'POSITION'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetJobLevel()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'JOBLEVEL'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetClusterPosition()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'POSCATGORY'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEducationLevel()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'EDUCLEVEL'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetSchool()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'SCHOOL'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetCourse()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'COURSE'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetCivilStatus()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'CIVILSTAT'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetReligion()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'RELIGION'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetCitizenship()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'CITIZEN'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetBloodType()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'BLOODGROUP'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetShoesSize()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'SHOESSIZE'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetShirtSize()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'SHIRTSIZE'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetHairColor()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'HAIRCOLOR'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEyeColor()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'EYECOLOR'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetBarangay()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'BARANGAY'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetZipCode()
        {
            string query = @"SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'ZIPCODE'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

       
        public DataTable GetPayrollType()
        {
            string query = @"SELECT [Mcd_Code] AS 'Payroll Type Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'PAYTYPE'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetRankLevel()
        {
            string query = @"SELECT [Mcd_Code] AS 'Rank/Level Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'RANKLEVEL'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public string GetQueryCountGivenColumn(string ColumnName, StatisticsView statView)
        {
            string query = "";
            string strTemplate1 = @"SELECT AcctDet.Mcd_Name as Description
                                    , COUNT(Mem_IDNo) AS ''Count''
                                    FROM {0}..M_Employee
                                    LEFT JOIN {0}..M_CodeDtl AcctDet on AcctDet.Mcd_Code = {2}			
                                    and AcctDet.Mcd_CodeType = ''{3}''
                                    {1}
                                    GROUP BY AcctDet.Mcd_Name";

            if (statView == StatisticsView.SUMMARY)
            {
                switch (ColumnName)
                {
                    case "Classification":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_PositionClass", "JOBTITLE");
                        break;
                    case "JobStatus":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_WorkStatus", "JOBSTATUS");
                        break;
                    case "EmploymentStatus":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_EmploymentStatusCode", "EMPSTAT");
                        break;
                    case "Gender":
                        query = @"SELECT CASE Mem_Gender WHEN 'M' THEN 'MALE' WHEN 'F' THEN 'FEMALE' ELSE '' END as Description
                                    , COUNT(Mem_IDNo) AS 'Count'
                                    FROM {0}..M_Employee
                                    {1}
                                    GROUP BY Mem_Gender";
                        break;
                    case "CostCenter":
                        query = @"SELECT dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description
                                    , COUNT(Mem_IDNo) AS 'Count'
                                    FROM {0}..M_Employee
                                    {1}
                                    GROUP BY Mem_CostcenterCode";
                        break;
                    case "Position":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_JobTitleCode", "POSITION");
                        break;
                    case "JobLevel":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_RankCode", "JOBLEVEL");
                        break;
                    case "ClusterPosition":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_PositionCategory", "POSCATGORY");
                        break;
                    case "EducationLevel":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_EducationCode", "EDUCLEVEL");
                        break;
                    case "School":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_SchoolCode", "SCHOOL");
                        break;
                    case "Course":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_CourseCode", "COURSE");
                        break;
                    case "CivilStatus":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_CivilStatusCode", "CIVILSTAT");
                        break;
                    case "Religion":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_ReligionCode", "RELIGION");
                        break;
                    case "Citizenship":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_NationalityCode", "CITIZEN");
                        break;
                    case "BloodType":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_BloodType", "BLOODGROUP");
                        break;
                    case "ShoesSize":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_ShoesSize", "SHOESSIZE");
                        break;
                    case "ShirtSize":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_ShirtSize", "SHIRTSIZE");
                        break;
                    case "HairColor":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_HairColor", "HAIRCOLOR");
                        break;
                    case "EyeColor":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_EyeColor", "EYECOLOR");
                        break;
                    case "Barangay":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_PresAddressBarangay", "BARANGAY");
                        break;
                    case "ZipCode":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_PresAddressMunicipalityCity", "ZIPCODE");
                        break;
                    case "PayrollType":
                        query = string.Format(strTemplate1, "{0}", "{1}", "Mem_PayrollType", "PAYTYPE");
                        break;
                }
            }
            return query;
        }

        public string ReturnQuery(StatisticsType statType, StatisticsView statView)
        {
            string query = "";
            if (statView == StatisticsView.SUMMARY)
            {
                switch (statType)
                {
                    case StatisticsType.COST_CENTER:
                        query = @"EXEC CrossTab			
                                'select Mem_CostcenterCode as Costcenter 			
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description			
                                , Mem_IDNo			
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender			
                                , EmpStat.Mcd_Name as EmploymentStatus			
                                , Isnull(Classification.Mcd_Name, space(1)) as Classification			
                                , JobStatus.Mcd_Name as JobStatus			
                                from M_Employee			
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode			
	                                and EmpStat.Mcd_CodeType=''EMPSTAT''		
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus			
	                                and JobStatus.Mcd_CodeType = ''WORKSTAT''		
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass			
	                                and Classification.Mcd_CodeType = ''JOBTITLE''	
                                {0}',
                                '{1}',			
                                'Count(Mem_IDNo)[]',			
                                '{2}'"; 
                        break;
                    case StatisticsType.POSITION:
                        query = @"EXEC CrossTab 'select  Mem_CostcenterCode as Costcenter				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Classification.Mcd_Name, space(1)) as Classification				
                                , Mem_IDNo 				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , Isnull(Position.Mcd_Name, space(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, space(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, space(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, space(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Mcd_Name, space(1))  as JobStatus				
                                from M_Employee				
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''			
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
                                    and Position.Mcd_CodeType = ''POSITION''			
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
                                    and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
                                    and JobStatus.Mcd_CodeType = ''WORKSTAT''
                                {0}',			
                                '{1}',				
                                'Count(Mem_IDNo)[]',				
                                '{2}'	";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"EXEC CrossTab 'select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification				
                                , Mem_IDNo 				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , Mem_CostcenterCode as Costcenter 				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus
                                , Isnull(JobStatus.Mcd_Name, SPACE(1))  as JobStatus				
                                , Isnull(Separation.Mcd_Name, SPACE(1))  as ReasonofSeparation				
                                , Round(DATEDIFF(dd,Mem_IntakeDate, Mem_SeparationDate)/365.00,3)	as ServiceYears			
                                , CONVERT(Char(6), Mem_SeparationDate,112) as SeparationYearMonth				
                                , CONVERT(Char(4), Mem_SeparationDate,112) as SeparationYear				
                                from M_Employee				
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
	                                and EmpStat.Mcd_CodeType=''EMPSTAT''			
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
	                                and Position.Mcd_CodeType = ''POSITION''			
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
	                                and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
	                                and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
	                                and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
                                    and JobStatus.Mcd_CodeType = ''WORKSTAT''
                                left join M_CodeDtl Separation on Separation.Mcd_Code = Mem_SeparationCode				
	                                and Separation.Mcd_CodeType = ''SEPARATION'' 
                                {0}',			
                                '{1}',				
                                'Count(Mem_IDNo)[]',				
                                '{2}'";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"EXEC CrossTab 'select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification				
                                , Mem_IDNo 				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , Mem_CostcenterCode as Costcenter 				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Mcd_Name, SPACE(1))  as JobStatus	
                                , Round(DATEDIFF(dd,Mem_IntakeDate,''{3}'')/365.00,3) as ServiceYears		
                                , CONVERT(Char(6), Mem_IntakeDate,112) as HireYearMonth				
                                , CONVERT(Char(4), Mem_IntakeDate,112) as HireYear				
                                from M_Employee				
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''			
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
                                    and Position.Mcd_CodeType = ''POSITION''			
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
                                    and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
                                    and JobStatus.Mcd_CodeType = ''WORKSTAT''
                                {0}',			
                                '{1}',				
                                'Count(Mem_IDNo)[]',				
                                '{2}'";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"EXEC CrossTab				
                                'select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification				
                                , Mem_IDNo				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , Mem_CostcenterCode as Costcenter 				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Mcd_Name, SPACE(1))  as JobStatus				
                                , Isnull(EducLevel.Mcd_Name, SPACE(1)) as EducationalLevel   				
                                , Isnull(School.Mcd_Name, SPACE(1)) as School   				
                                , Isnull(Course.Mcd_Name, SPACE(1)) as Course 				
                                from M_Employee				
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
	                                and EmpStat.Mcd_CodeType= ''EMPSTAT''			
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
	                                and Position.Mcd_CodeType = ''POSITION''			
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
	                                and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
	                                and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
	                                and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
	                                and JobStatus.Mcd_CodeType = ''WORKSTAT''			
                                Left Join M_CodeDtl EducLevel on EducLevel.Mcd_Code = Mem_EducationCode   				
	                                and  EducLevel.Mcd_CodeType = ''EDUCLEVEL''   			
                                Left Join M_CodeDtl School on School.Mcd_Code = Mem_SchoolCode   				
	                                and  School.Mcd_CodeType = ''SCHOOL''   			
                                Left Join M_CodeDtl Course on Course.Mcd_Code = Mem_CourseCode   				
	                                 and  Course.Mcd_CodeType = ''COURSE''
                                {0}',			
                                '{1}',				
                                'Count(Mem_IDNo)[]',				
                                '{2}' ";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"EXEC CrossTab											
                                'select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification											
                                , Mem_IDNo 											
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender											
                                , Mem_CostcenterCode as Costcenter 											
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description											
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position											
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel											
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition											
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus											
                                , Isnull(CivilStatus.Mcd_Name, Space(1)) AS  CivilStatus											
                                , Isnull(Religion.Mcd_Name, SPACE(1)) AS Religion 											
                                , Isnull(Citizenship.Mcd_Name, SPACE(1)) AS Citizenship											
                                , Isnull(BloodType.Mcd_Name, SPACE(1))  AS BloodType											
                                , Isnull(ShoesSize.Mcd_Code, SPACE(1)) AS ShoesSize											
                                , Isnull(ShirtSize.Mcd_Name, SPACE(1)) AS ShirtSize											
                                , Isnull(HairColor.Mcd_Name, SPACE(1)) AS HairColor   											
                                , Isnull(EyeColor.Mcd_Name, SPACE(1)) AS EyeColor	
                                , Mem_Age AS Age										
                                from M_Employee											
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode											
	                                and EmpStat.Mcd_CodeType=''EMPSTAT''										
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode											
	                                and Position.Mcd_CodeType = ''POSITION''										
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory											
	                                and PositionCategory.Mcd_CodeType = ''POSCATGORY''										
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass											
	                                and Classification.Mcd_CodeType = ''JOBTITLE''										
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode											
	                                and JobLevel.Mcd_CodeType = ''JOBLEVEL''										
                                LEFT JOIN M_CodeDtl AS Religion ON Religion.Mcd_Code = Mem_ReligionCode 											
                                AND   Religion.Mcd_CodeType = ''RELIGION'' 											
                                LEFT JOIN  M_CodeDtl AS Citizenship ON Citizenship.Mcd_Code = Mem_NationalityCode 											
	                                AND  Citizenship.Mcd_CodeType = ''CITIZEN''										
                                LEFT  JOIN  M_CodeDtl AS BloodType ON BloodType.Mcd_Code = Mem_BloodType    											
	                                AND BloodType.Mcd_CodeType = ''BLOODGROUP'' 										
                                LEFT JOIN  M_CodeDtl AS ShoesSize ON ShoesSize.Mcd_Code = M_Employee.Mem_ShoesSize    											
	                                AND ShoesSize.Mcd_CodeType = ''SHOESSIZE'' 										
                                LEFT JOIN M_CodeDtl AS ShirtSize ON ShirtSize.Mcd_Code = M_Employee.Mem_ShirtSize    											
	                                AND ShirtSize.Mcd_CodeType = ''SHIRTSIZE'' 										
                                LEFT JOIN M_CodeDtl AS HairColor ON HairColor.Mcd_Code = M_Employee.Mem_HairColor    											
	                                AND HairColor.Mcd_CodeType = ''HAIRCOLOR'' 										
                                LEFT JOIN M_CodeDtl AS EyeColor ON EyeColor.Mcd_Code = M_Employee.Mem_EyeColor    											
	                                AND EyeColor.Mcd_CodeType = ''EYECOLOR''										
                                 LEFT  JOIN  M_CodeDtl AS CivilStatus ON CivilStatus.Mcd_Code = dbo.M_Employee.Mem_CivilStatusCode    											
                                                     AND CivilStatus.Mcd_CodeType = ''CIVILSTAT''
                                {0}',											
                                '{1}',											
                                'Count(Mem_IDNo)[]',											
                                '{2}'";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"EXEC CrossTab										
                                'select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification										
                                , Mem_IDNo										
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender									
                                , Mem_CostcenterCode as Costcenter 										
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description										
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position										
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus										
                                , Isnull(EmpBarangay.Mcd_Name, SPACE(1))  AS BarangayMunicipality										
                                , Isnull(EmpCity.Mcd_Name, SPACE(1))  AS CityProvinceDistrict										
                                from M_Employee										
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode										
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''									
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode										
                                    and Position.Mcd_CodeType = ''POSITION''									
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory										
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''									
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass										
                                    and Classification.Mcd_CodeType = ''JOBTITLE''									
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode										
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''									
                                LEFT JOIN M_CodeDtl AS EmpBarangay ON EmpBarangay.Mcd_Code = Mem_PresAddressBarangay 										
                                    AND   EmpBarangay.Mcd_CodeType = ''BARANGAY'' 									
                                LEFT JOIN M_CodeDtl AS EmpCity ON EmpCity.Mcd_Code = Mem_PresAddressMunicipalityCity    										
                                    AND  EmpCity.Mcd_CodeType = ''ZIPCODE''
                                {0}',										
                                '{1}',										
                                'Count(Mem_IDNo)[]',										
                                '{2}'";
                        break;
                    case StatisticsType.SALARY:
                        query = @"EXEC CrossTab										
                                'select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification										
                                , Mem_IDNo										
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender										
                                , Mem_CostcenterCode as Costcenter 										
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description										
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position										
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus										
                                , Isnull(Mem_Salary,0) AS SalaryRate										
                                , Isnull(PayrollType.Mcd_Name, SPACE(1)) AS PayrollType										
                                from M_Employee										
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode										
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''									
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode										
                                    and Position.Mcd_CodeType = ''POSITION''									
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory										
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''									
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass										
                                    and Classification.Mcd_CodeType = ''JOBTITLE''									
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode										
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''									
                                LEFT JOIN  M_CodeDtl AS PayrollType ON PayrollType.Mcd_Code = Mem_PayrollType 										
                                    AND  PayrollType.Mcd_CodeType = ''PAYTYPE''
                                {0}',									
                                '{1}',										
                                'Count(Mem_IDNo)[]',										
                                '{2}'";
                        break;
                }
            }
            else if (statView == StatisticsView.DETAIL)
            {
                switch (statType)
                {
                    case StatisticsType.COST_CENTER:
                        query = @"select Mem_CostcenterCode as [Costcenter code]				
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]				
                                        , Mem_IDNo as [Employee ID]				
                                        , Mem_LastName	as [Last Name]			
                                        , Mem_FirstName	as [First Name]			
                                        , Mem_MiddleName as [Middle Name]	 			
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]			
                                        , Classification.Mcd_Name as [Classification]				
                                        , EmpStat.Mcd_Name as [Employment Status]				
                                        , JobStatus.Mcd_Name as [Job Status]				
                                from M_Employee				
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
	                                and JobStatus.Mcd_CodeType = 'WORKSTAT'			
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
	                                and Classification.Mcd_CodeType = 'JOBTITLE'	
                                {0}		
                                Order by Mem_CostcenterCode				
                                , Mem_LastName				
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.POSITION:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Mcd_Name, SPACE(1))  as [Job Status]					
                                from M_Employee					
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus					
	                                and JobStatus.Mcd_CodeType = 'WORKSTAT'	
                                {0}			
                                Order by [Classification]					
                                , Mem_LastName					
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , Isnull(Separation.Mcd_Name, SPACE(1))  as [Reason of Separation]					
                                        , CONVERT(Char(10), Mem_IntakeDate,101) as [Hire Date]					
                                        , CONVERT(Char(10), Mem_SeparationDate,101) as [Separation Date]					
                                        , Round(DATEDIFF(dd,Mem_IntakeDate, Mem_SeparationDate)/365.00,3)	as [Service Years]				
                                from M_Employee					
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                left join M_CodeDtl Separation on Separation.Mcd_Code = Mem_SeparationCode					
	                                and Separation.Mcd_CodeType = 'SEPARATION'	
                                {0}			
                                Order by  [Classification]					
                                , Mem_LastName					
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Mcd_Name, SPACE(1))  as [Job Status]	
                                        , Round(DATEDIFF(dd,Mem_IntakeDate,'{1}')/365.00,3) as [Service Years]					
                                        , CONVERT(Char(10), Mem_IntakeDate,101) as [Hire Date]					
                                from M_Employee					
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                left join M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus					
	                                and JobStatus.Mcd_CodeType = 'WORKSTAT'	
                                {0}			
                                Order by  [Classification]					
                                , Mem_LastName					
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , EducLevel.Mcd_Name as [Educational Level]   					
                                        , School.Mcd_Name as [School]   					
                                        , Course.Mcd_Name as [Course] 					
                                from M_Employee					
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                Left Join M_CodeDtl EducLevel on EducLevel.Mcd_Code = Mem_EducationCode   					
	                                and  EducLevel.Mcd_CodeType = 'EDUCLEVEL'   				
                                Left Join M_CodeDtl School on School.Mcd_Code = Mem_SchoolCode   					
	                                and  School.Mcd_CodeType = 'SCHOOL'   				
                                Left Join M_CodeDtl Course on Course.Mcd_Code = Mem_CourseCode   					
	                                 and  Course.Mcd_CodeType = 'COURSE' 
                                {0}				
                                Order by  [Classification]					
                                , Mem_LastName					
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]											
                                        , Mem_IDNo as [Employee ID]											
                                        , Mem_LastName	as [Last Name]										
                                        , Mem_FirstName	as [First Name]										
                                        , Mem_MiddleName as [Middle Name]	 										
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , Mem_CostcenterCode as [Costcenter code]											
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]											
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]											
                                        , Isnull(CivilStatus.Mcd_Name, Space(1)) AS [Civil Status]											
                                        , Isnull(Religion.Mcd_Name, SPACE(1)) AS [Religion] 											
                                        , Isnull(Citizenship.Mcd_Name, SPACE(1)) AS [Citizenship]											
                                        , Isnull(BloodType.Mcd_Name, SPACE(1))  AS [Blood Type]											
                                        , Isnull(ShoesSize.Mcd_Code, SPACE(1)) AS [Shoes Size]											
                                        , Isnull(ShirtSize.Mcd_Name, SPACE(1)) AS [Shirt Size]											
                                        , Isnull(HairColor.Mcd_Name, SPACE(1)) AS [Hair Color]   											
                                        , Isnull(EyeColor.Mcd_Name, SPACE(1)) AS [Eye Color]	
                                        , Mem_Age AS [Age]										
                                from M_Employee											
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode											
	                                and EmpStat.Mcd_CodeType='EMPSTAT'										
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode											
	                                and Position.Mcd_CodeType = 'POSITION'										
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory											
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'										
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass											
	                                and Classification.Mcd_CodeType = 'JOBTITLE'										
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode											
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'										
                                LEFT JOIN M_CodeDtl AS Religion ON Religion.Mcd_Code = Mem_ReligionCode 											
                                AND   Religion.Mcd_CodeType = 'RELIGION' 											
                                LEFT JOIN  M_CodeDtl AS Citizenship ON Citizenship.Mcd_Code = Mem_NationalityCode 											
	                                AND  Citizenship.Mcd_CodeType = 'CITIZEN'										
                                LEFT  JOIN  M_CodeDtl AS BloodType ON BloodType.Mcd_Code = Mem_BloodType    											
	                                AND BloodType.Mcd_CodeType = 'BLOODGROUP' 										
                                LEFT JOIN  M_CodeDtl AS ShoesSize ON ShoesSize.Mcd_Code = M_Employee.Mem_ShoesSize    											
	                                AND ShoesSize.Mcd_CodeType = 'SHOESSIZE' 										
                                LEFT JOIN M_CodeDtl AS ShirtSize ON ShirtSize.Mcd_Code = M_Employee.Mem_ShirtSize    											
	                                AND ShirtSize.Mcd_CodeType = 'SHIRTSIZE' 										
                                LEFT JOIN M_CodeDtl AS HairColor ON HairColor.Mcd_Code = M_Employee.Mem_HairColor    											
	                                AND HairColor.Mcd_CodeType = 'HAIRCOLOR' 										
                                LEFT JOIN M_CodeDtl AS EyeColor ON EyeColor.Mcd_Code = M_Employee.Mem_EyeColor    											
	                                AND EyeColor.Mcd_CodeType = 'EYECOLOR'     										
                                 LEFT  JOIN  M_CodeDtl AS CivilStatus ON CivilStatus.Mcd_Code = dbo.M_Employee.Mem_CivilStatusCode    											
                                    AND CivilStatus.Mcd_CodeType = 'CIVILSTAT'
                                {0}											
                                Order by  [Classification]											
                                , Mem_LastName											
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]										
                                        , Mem_IDNo as [Employee ID]										
                                        , Mem_LastName	as [Last Name]									
                                        , Mem_FirstName	as [First Name]									
                                        , Mem_MiddleName as [Middle Name]	 									
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]									
                                        , Mem_CostcenterCode as [Costcenter code]										
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]										
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]										
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]										
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]										
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]										
                                        , Isnull(EmpBarangay.Mcd_Name, SPACE(1))  AS [Barangay/Municipality]										
                                        , Isnull(EmpCity.Mcd_Name, SPACE(1))  AS [City/Province/District]										
                                from M_Employee										
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode										
	                                and EmpStat.Mcd_CodeType='EMPSTAT'									
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode										
	                                and Position.Mcd_CodeType = 'POSITION'									
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory										
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'									
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass										
	                                and Classification.Mcd_CodeType = 'JOBTITLE'									
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode										
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'									
                                LEFT JOIN M_CodeDtl AS EmpBarangay ON EmpBarangay.Mcd_Code = Mem_PresAddressBarangay 										
	                                AND   EmpBarangay.Mcd_CodeType = 'BARANGAY' 									
                                LEFT JOIN M_CodeDtl AS EmpCity ON EmpCity.Mcd_Code = Mem_PresAddressMunicipalityCity    										
                                    AND  EmpCity.Mcd_CodeType = 'ZIPCODE'
                                {0}										
                                Order by  [Classification]										
                                , Mem_LastName										
                                , Mem_FirstName	";
                        break;
                    case StatisticsType.SALARY:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]											
                                        , Mem_IDNo as [Employee ID]											
                                        , Mem_LastName	as [Last Name]										
                                        , Mem_FirstName	as [First Name]										
                                        , Mem_MiddleName as [Middle Name]	 										
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , Mem_CostcenterCode as [Costcenter code]											
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]											
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]											
                                        , Isnull(Mem_Salary,0) AS [Salary Rate]											
                                        , Isnull(PayrollType.Mcd_Name, SPACE(1)) AS [Payroll Type]											
                                from M_Employee											
                                left join M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode											
	                                and EmpStat.Mcd_CodeType='EMPSTAT'										
                                left join M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode											
	                                and Position.Mcd_CodeType = 'POSITION'										
                                left join M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory											
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'										
                                left join M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass											
	                                and Classification.Mcd_CodeType = 'JOBTITLE'										
                                left join M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode											
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'										
                                LEFT JOIN  M_CodeDtl AS PayrollType ON PayrollType.Mcd_Code = Mem_PayrollType 											
	                                AND  PayrollType.Mcd_CodeType = 'PAYTYPE'
                                {0}										
                                Order by  [Classification]											
                                , Mem_LastName											
                                , Mem_FirstName	";
                        break;
                }
            }
            return query;
        }

        public string ReturnQueryWithProfile(StatisticsType statType, StatisticsView statView)
        {
            string query = "";
            if (statView == StatisticsView.SUMMARY)
            {
                switch (statType)
                {
                    case StatisticsType.COST_CENTER:
                        query = @"select Mem_CostcenterCode as Costcenter 			
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description			
                                , Mem_IDNo			
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender			
                                , EmpStat.Mcd_Name as EmploymentStatus			
                                , Isnull(Classification.Mcd_Name, space(1)) as Classification			
                                , JobStatus.Mcd_Name as JobStatus			
                                from {0}..M_Employee			
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode			
	                                and EmpStat.Mcd_CodeType=''EMPSTAT''		
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus			
	                                and JobStatus.Mcd_CodeType = ''WORKSTAT''		
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass			
	                                and Classification.Mcd_CodeType = ''JOBTITLE''	
                                {1}";
                        break;
                    case StatisticsType.POSITION:
                        query = @"select  Mem_CostcenterCode as Costcenter				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Classification.Mcd_Name, space(1)) as Classification				
                                , Mem_IDNo 				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , Isnull(Position.Mcd_Name, space(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, space(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, space(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, space(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Mcd_Name, space(1))  as JobStatus				
                                from {0}..M_Employee				
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''			
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
                                    and Position.Mcd_CodeType = ''POSITION''			
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
                                    and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
                                    and JobStatus.Mcd_CodeType = ''WORKSTAT''
                                {1}";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification				
                                , Mem_IDNo 				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , Mem_CostcenterCode as Costcenter 				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus
                                , Isnull(JobStatus.Mcd_Name, SPACE(1))  as JobStatus				
                                , Isnull(Separation.Mcd_Name, SPACE(1))  as ReasonofSeparation				
                                , Round(DATEDIFF(dd,Mem_IntakeDate, Mem_SeparationDate)/365.00,3)	as ServiceYears			
                                , CONVERT(Char(6), Mem_SeparationDate,112) as SeparationYearMonth				
                                , CONVERT(Char(4), Mem_SeparationDate,112) as SeparationYear				
                                from {0}..M_Employee				
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
	                                and EmpStat.Mcd_CodeType=''EMPSTAT''			
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
	                                and Position.Mcd_CodeType = ''POSITION''			
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
	                                and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
	                                and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
	                                and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
                                    and JobStatus.Mcd_CodeType = ''WORKSTAT''
                                left join {0}..M_CodeDtl Separation on Separation.Mcd_Code = Mem_SeparationCode				
	                                and Separation.Mcd_CodeType = ''SEPARATION'' 
                                {1}";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification				
                                , Mem_IDNo 				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , Mem_CostcenterCode as Costcenter 				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Mcd_Name, SPACE(1))  as JobStatus	
                                , Round(DATEDIFF(dd,Mem_IntakeDate,''{2}'')/365.00,3) as ServiceYears		
                                , CONVERT(Char(6), Mem_IntakeDate,112) as HireYearMonth				
                                , CONVERT(Char(4), Mem_IntakeDate,112) as HireYear				
                                from {0}..M_Employee				
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''			
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
                                    and Position.Mcd_CodeType = ''POSITION''			
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
                                    and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
                                    and JobStatus.Mcd_CodeType = ''WORKSTAT''
                                {1}";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification				
                                , Mem_IDNo				
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , Mem_CostcenterCode as Costcenter 				
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description				
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position				
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Mcd_Name, SPACE(1))  as JobStatus				
                                , Isnull(EducLevel.Mcd_Name, SPACE(1)) as EducationalLevel   				
                                , Isnull(School.Mcd_Name, SPACE(1)) as School   				
                                , Isnull(Course.Mcd_Name, SPACE(1)) as Course 				
                                from {0}..M_Employee				
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
	                                and EmpStat.Mcd_CodeType= ''EMPSTAT''			
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode				
	                                and Position.Mcd_CodeType = ''POSITION''			
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory				
	                                and PositionCategory.Mcd_CodeType = ''POSCATGORY''			
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
	                                and Classification.Mcd_CodeType = ''JOBTITLE''			
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode				
	                                and JobLevel.Mcd_CodeType = ''JOBLEVEL''			
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
	                                and JobStatus.Mcd_CodeType = ''WORKSTAT''			
                                Left Join {0}..M_CodeDtl EducLevel on EducLevel.Mcd_Code = Mem_EducationCode   				
	                                and  EducLevel.Mcd_CodeType = ''EDUCLEVEL''   			
                                Left Join {0}..M_CodeDtl School on School.Mcd_Code = Mem_SchoolCode   				
	                                and  School.Mcd_CodeType = ''SCHOOL''   			
                                Left Join {0}..M_CodeDtl Course on Course.Mcd_Code = Mem_CourseCode   				
	                                 and  Course.Mcd_CodeType = ''COURSE''
                                {1}";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification											
                                , Mem_IDNo 											
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender											
                                , Mem_CostcenterCode as Costcenter 											
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description											
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position											
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel											
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition											
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus											
                                , Isnull(CivilStatus.Mcd_Name, Space(1)) AS  CivilStatus											
                                , Isnull(Religion.Mcd_Name, SPACE(1)) AS Religion 											
                                , Isnull(Citizenship.Mcd_Name, SPACE(1)) AS Citizenship											
                                , Isnull(BloodType.Mcd_Name, SPACE(1))  AS BloodType											
                                , Isnull(ShoesSize.Mcd_Code, SPACE(1)) AS ShoesSize											
                                , Isnull(ShirtSize.Mcd_Name, SPACE(1)) AS ShirtSize											
                                , Isnull(HairColor.Mcd_Name, SPACE(1)) AS HairColor   											
                                , Isnull(EyeColor.Mcd_Name, SPACE(1)) AS EyeColor	
                                , Mem_Age AS Age										
                                from {0}..M_Employee											
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode											
	                                and EmpStat.Mcd_CodeType=''EMPSTAT''										
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode											
	                                and Position.Mcd_CodeType = ''POSITION''										
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory											
	                                and PositionCategory.Mcd_CodeType = ''POSCATGORY''										
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass											
	                                and Classification.Mcd_CodeType = ''JOBTITLE''										
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode											
	                                and JobLevel.Mcd_CodeType = ''JOBLEVEL''										
                                LEFT JOIN {0}..M_CodeDtl Religion ON Religion.Mcd_Code = Mem_ReligionCode 											
                                AND   Religion.Mcd_CodeType = ''RELIGION'' 											
                                LEFT JOIN  {0}..M_CodeDtl Citizenship ON Citizenship.Mcd_Code = Mem_NationalityCode 											
	                                AND  Citizenship.Mcd_CodeType = ''CITIZEN''										
                                LEFT  JOIN  {0}..M_CodeDtl BloodType ON BloodType.Mcd_Code = Mem_BloodType    											
	                                AND BloodType.Mcd_CodeType = ''BLOODGROUP'' 										
                                LEFT JOIN  {0}..M_CodeDtl ShoesSize ON ShoesSize.Mcd_Code = Mem_ShoesSize    											
	                                AND ShoesSize.Mcd_CodeType = ''SHOESSIZE'' 										
                                LEFT JOIN {0}..M_CodeDtl ShirtSize ON ShirtSize.Mcd_Code = Mem_ShirtSize    											
	                                AND ShirtSize.Mcd_CodeType = ''SHIRTSIZE'' 										
                                LEFT JOIN {0}..M_CodeDtl HairColor ON HairColor.Mcd_Code = Mem_HairColor    											
	                                AND HairColor.Mcd_CodeType = ''HAIRCOLOR'' 										
                                LEFT JOIN {0}..M_CodeDtl EyeColor ON EyeColor.Mcd_Code = Mem_EyeColor    											
	                                AND EyeColor.Mcd_CodeType = ''EYECOLOR''										
                                 LEFT  JOIN  {0}..M_CodeDtl CivilStatus ON CivilStatus.Mcd_Code = Mem_CivilStatusCode    											
                                    AND CivilStatus.Mcd_CodeType = ''CIVILSTAT''
                                {1}";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification										
                                , Mem_IDNo										
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender									
                                , Mem_CostcenterCode as Costcenter 										
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description										
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position										
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus										
                                , Isnull(EmpBarangay.Mcd_Name, SPACE(1))  AS BarangayMunicipality										
                                , Isnull(EmpCity.Mcd_Name, SPACE(1))  AS CityProvinceDistrict										
                                from {0}..M_Employee										
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode										
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''									
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode										
                                    and Position.Mcd_CodeType = ''POSITION''									
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory										
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''									
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass										
                                    and Classification.Mcd_CodeType = ''JOBTITLE''									
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode										
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''									
                                LEFT JOIN {0}..M_CodeDtl AS EmpBarangay ON EmpBarangay.Mcd_Code = Mem_PresAddressBarangay 										
                                    AND   EmpBarangay.Mcd_CodeType = ''BARANGAY'' 									
                                LEFT JOIN {0}..M_CodeDtl AS EmpCity ON EmpCity.Mcd_Code = Mem_PresAddressMunicipalityCity    										
                                    AND  EmpCity.Mcd_CodeType = ''ZIPCODE''
                                {1}";
                        break;
                    case StatisticsType.SALARY:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as Classification										
                                , Mem_IDNo										
                                , Case when Mem_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender										
                                , Mem_CostcenterCode as Costcenter 										
                                , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as Description										
                                , Isnull(Position.Mcd_Name, SPACE(1)) as Position										
                                , Isnull(JobLevel.Mcd_Name, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Mcd_Name, SPACE(1))  as EmploymentStatus										
                                , Isnull(Mem_Salary,0) AS SalaryRate										
                                , Isnull(PayrollType.Mcd_Name, SPACE(1)) AS PayrollType										
                                from {0}..M_Employee										
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode										
                                    and EmpStat.Mcd_CodeType=''EMPSTAT''									
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode										
                                    and Position.Mcd_CodeType = ''POSITION''									
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory										
                                    and PositionCategory.Mcd_CodeType = ''POSCATGORY''									
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass										
                                    and Classification.Mcd_CodeType = ''JOBTITLE''									
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode										
                                    and JobLevel.Mcd_CodeType = ''JOBLEVEL''									
                                LEFT JOIN  {0}..M_CodeDtl AS PayrollType ON PayrollType.Mcd_Code = Mem_PayrollType 										
                                    AND  PayrollType.Mcd_CodeType = ''PAYTYPE''
                                {1}";
                        break;
                }
            }
            else if (statView == StatisticsView.DETAIL)
            {
                switch (statType)
                {
                    case StatisticsType.COST_CENTER:
                        query = @"select Mem_CostcenterCode as [Costcenter code]				
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]				
                                        , Mem_IDNo as [Employee ID]				
                                        , Mem_LastName	as [Last Name]			
                                        , Mem_FirstName	as [First Name]			
                                        , Mem_MiddleName as [Middle Name]	 			
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]			
                                        , Classification.Mcd_Name as [Classification]				
                                        , EmpStat.Mcd_Name as [Employment Status]				
                                        , JobStatus.Mcd_Name as [Job Status]				
                                from {0}..M_Employee				
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode				
                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus				
	                                and JobStatus.Mcd_CodeType = 'WORKSTAT'			
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass				
	                                and Classification.Mcd_CodeType = 'JOBTITLE'	
                                {1}		";
                        break;
                    case StatisticsType.POSITION:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Mcd_Name, SPACE(1))  as [Job Status]					
                                from {0}..M_Employee					
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus					
	                                and JobStatus.Mcd_CodeType = 'WORKSTAT'	
                                {1}		";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , Isnull(Separation.Mcd_Name, SPACE(1))  as [Reason of Separation]					
                                        , CONVERT(Char(10), Mem_IntakeDate,101) as [Hire Date]					
                                        , CONVERT(Char(10), Mem_SeparationDate,101) as [Separation Date]					
                                        , Round(DATEDIFF(dd,Mem_IntakeDate, Mem_SeparationDate)/365.00,3)	as [Service Years]				
                                from {0}..M_Employee					
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                left join {0}..M_CodeDtl Separation on Separation.Mcd_Code = Mem_SeparationCode					
	                                and Separation.Mcd_CodeType = 'SEPARATION'	
                                {1}		";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Mcd_Name, SPACE(1))  as [Job Status]	
                                        , Round(DATEDIFF(dd,Mem_IntakeDate,'{2}')/365.00,3) as [Service Years]					
                                        , CONVERT(Char(10), Mem_IntakeDate,101) as [Hire Date]					
                                from {0}..M_Employee					
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                left join {0}..M_CodeDtl JobStatus on JobStatus.Mcd_Code = Mem_WorkStatus					
	                                and JobStatus.Mcd_CodeType = 'WORKSTAT'	
                                {1}	";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]					
                                        , Mem_IDNo as [Employee ID]					
                                        , Mem_LastName	as [Last Name]				
                                        , Mem_FirstName	as [First Name]				
                                        , Mem_MiddleName as [Middle Name]	 				
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , Mem_CostcenterCode as [Costcenter code]					
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]					
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]					
                                        , EducLevel.Mcd_Name as [Educational Level]   					
                                        , School.Mcd_Name as [School]   					
                                        , Course.Mcd_Name as [Course] 					
                                from {0}..M_Employee					
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode					
	                                and EmpStat.Mcd_CodeType='EMPSTAT'				
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode					
	                                and Position.Mcd_CodeType = 'POSITION'				
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory					
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'				
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass					
	                                and Classification.Mcd_CodeType = 'JOBTITLE'				
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode					
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'				
                                Left Join {0}..M_CodeDtl EducLevel on EducLevel.Mcd_Code = Mem_EducationCode   					
	                                and  EducLevel.Mcd_CodeType = 'EDUCLEVEL'   				
                                Left Join {0}..M_CodeDtl School on School.Mcd_Code = Mem_SchoolCode   					
	                                and  School.Mcd_CodeType = 'SCHOOL'   				
                                Left Join {0}..M_CodeDtl Course on Course.Mcd_Code = Mem_CourseCode   					
	                                 and  Course.Mcd_CodeType = 'COURSE' 
                                {1}		";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]											
                                        , Mem_IDNo as [Employee ID]											
                                        , Mem_LastName	as [Last Name]										
                                        , Mem_FirstName	as [First Name]										
                                        , Mem_MiddleName as [Middle Name]	 										
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , Mem_CostcenterCode as [Costcenter code]											
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]											
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]											
                                        , Isnull(CivilStatus.Mcd_Name, Space(1)) AS [Civil Status]											
                                        , Isnull(Religion.Mcd_Name, SPACE(1)) AS [Religion] 											
                                        , Isnull(Citizenship.Mcd_Name, SPACE(1)) AS [Citizenship]											
                                        , Isnull(BloodType.Mcd_Name, SPACE(1))  AS [Blood Type]											
                                        , Isnull(ShoesSize.Mcd_Code, SPACE(1)) AS [Shoes Size]											
                                        , Isnull(ShirtSize.Mcd_Name, SPACE(1)) AS [Shirt Size]											
                                        , Isnull(HairColor.Mcd_Name, SPACE(1)) AS [Hair Color]   											
                                        , Isnull(EyeColor.Mcd_Name, SPACE(1)) AS [Eye Color]	
                                        , Mem_Age AS [Age]										
                                from {0}..M_Employee											
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode											
	                                and EmpStat.Mcd_CodeType='EMPSTAT'										
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode											
	                                and Position.Mcd_CodeType = 'POSITION'										
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory											
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'										
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass											
	                                and Classification.Mcd_CodeType = 'JOBTITLE'										
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode											
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'										
                                LEFT JOIN {0}..M_CodeDtl AS Religion ON Religion.Mcd_Code = Mem_ReligionCode 											
                                AND   Religion.Mcd_CodeType = 'RELIGION' 											
                                LEFT JOIN  {0}..M_CodeDtl AS Citizenship ON Citizenship.Mcd_Code = Mem_NationalityCode 											
	                                AND  Citizenship.Mcd_CodeType = 'CITIZEN'										
                                LEFT  JOIN  {0}..M_CodeDtl AS BloodType ON BloodType.Mcd_Code = Mem_BloodType    											
	                                AND BloodType.Mcd_CodeType = 'BLOODGROUP' 										
                                LEFT JOIN  {0}..M_CodeDtl AS ShoesSize ON ShoesSize.Mcd_Code = Mem_ShoesSize    											
	                                AND ShoesSize.Mcd_CodeType = 'SHOESSIZE' 										
                                LEFT JOIN {0}..M_CodeDtl AS ShirtSize ON ShirtSize.Mcd_Code = Mem_ShirtSize    											
	                                AND ShirtSize.Mcd_CodeType = 'SHIRTSIZE' 										
                                LEFT JOIN {0}..M_CodeDtl AS HairColor ON HairColor.Mcd_Code = Mem_HairColor    											
	                                AND HairColor.Mcd_CodeType = 'HAIRCOLOR' 										
                                LEFT JOIN {0}..M_CodeDtl AS EyeColor ON EyeColor.Mcd_Code = Mem_EyeColor    											
	                                AND EyeColor.Mcd_CodeType = 'EYECOLOR'     										
                                 LEFT  JOIN  {0}..M_CodeDtl AS CivilStatus ON CivilStatus.Mcd_Code = Mem_CivilStatusCode    											
                                    AND CivilStatus.Mcd_CodeType = 'CIVILSTAT'
                                {1}		";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]										
                                        , Mem_IDNo as [Employee ID]										
                                        , Mem_LastName	as [Last Name]									
                                        , Mem_FirstName	as [First Name]									
                                        , Mem_MiddleName as [Middle Name]	 									
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]									
                                        , Mem_CostcenterCode as [Costcenter code]										
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]										
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]										
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]										
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]										
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]										
                                        , Isnull(EmpBarangay.Mcd_Name, SPACE(1))  AS [Barangay/Municipality]										
                                        , Isnull(EmpCity.Mcd_Name, SPACE(1))  AS [City/Province/District]										
                                from {0}..M_Employee										
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode										
	                                and EmpStat.Mcd_CodeType='EMPSTAT'									
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode										
	                                and Position.Mcd_CodeType = 'POSITION'									
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory										
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'									
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass										
	                                and Classification.Mcd_CodeType = 'JOBTITLE'									
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode										
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'									
                                LEFT JOIN {0}..M_CodeDtl AS EmpBarangay ON EmpBarangay.Mcd_Code = Mem_PresAddressBarangay 										
	                                AND   EmpBarangay.Mcd_CodeType = 'BARANGAY' 									
                                LEFT JOIN {0}..M_CodeDtl AS EmpCity ON EmpCity.Mcd_Code = Mem_PresAddressMunicipalityCity    										
                                    AND  EmpCity.Mcd_CodeType = 'ZIPCODE'
                                {1}		";
                        break;
                    case StatisticsType.SALARY:
                        query = @"select  Isnull(Classification.Mcd_Name, SPACE(1)) as [Classification]											
                                        , Mem_IDNo as [Employee ID]											
                                        , Mem_LastName	as [Last Name]										
                                        , Mem_FirstName	as [First Name]										
                                        , Mem_MiddleName as [Middle Name]	 										
                                        , Case when Mem_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , Mem_CostcenterCode as [Costcenter code]											
                                        , dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode) as [Description]											
                                        , Isnull(Position.Mcd_Name, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Mcd_Name, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Mcd_Name, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Mcd_Name, SPACE(1))  as [Employment Status]											
                                        , Isnull(Mem_Salary,0) AS [Salary Rate]											
                                        , Isnull(PayrollType.Mcd_Name, SPACE(1)) AS [Payroll Type]											
                                from {0}..M_Employee											
                                left join {0}..M_CodeDtl EmpStat on EmpStat.Mcd_Code = Mem_EmploymentStatusCode											
	                                and EmpStat.Mcd_CodeType='EMPSTAT'										
                                left join {0}..M_CodeDtl Position on Position.Mcd_Code = Mem_JobTitleCode											
	                                and Position.Mcd_CodeType = 'POSITION'										
                                left join {0}..M_CodeDtl PositionCategory on PositionCategory.Mcd_Code = Mem_PositionCategory											
	                                and PositionCategory.Mcd_CodeType = 'POSCATGORY'										
                                left join {0}..M_CodeDtl Classification on Classification.Mcd_Code = Mem_PositionClass											
	                                and Classification.Mcd_CodeType = 'JOBTITLE'										
                                left join {0}..M_CodeDtl JobLevel on JobLevel.Mcd_Code = Mem_RankCode											
	                                and JobLevel.Mcd_CodeType = 'JOBLEVEL'										
                                LEFT JOIN  {0}..M_CodeDtl AS PayrollType ON PayrollType.Mcd_Code = Mem_PayrollType 											
	                                AND  PayrollType.Mcd_CodeType = 'PAYTYPE'
                                {1}	";
                        break;
                }
            }
            return query;
        }

        public bool CanUserViewRate(string UserCode, DALHelper dal)
        {
            string query = string.Format(@"SELECT Muh_CanViewRate
                                            FROM M_UserHdr
                                            WHERE Muh_UserCode = '{0}'", UserCode);

            DataSet ds = new DataSet();
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();

            if (ds.Tables[0].Rows.Count > 0)
                if (Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString()))
                    return true;

            return false;
        }

        public DataTable GetGroup(string costCenter)
        {
            if (costCenter != "''")
            {
                costCenter = string.Format(@" WHERE Clm_CostCenterCode IN ({0})", costCenter);
            }
            else
            {
                costCenter = "";
            }
            string query = string.Format(@"SELECT DISTINCT Clm_LineCode [Code]
                                            FROM E_CostCenterLineMaster
                                            {0}
                                            ORDER BY 1", costCenter);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetBank(string CompanyCode, string CentralProfile)
        {
            string sqlQuery = string.Format(
                                @"SELECT Mbn_BankCode as [Bank Code]
                                , Mbn_BankName as [Description]
                                FROM M_Bank
                                WHERE Mbn_BankType IN ('C','B')
                                AND Mbn_RecordStatus = 'A'
                                AND Mbn_CompanyCode = '{0}'", CompanyCode);

            DataTable dtResult;

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(sqlQuery).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetPayCycle()
        {
            string sqlQuery = string.Format(@"SELECT Tps_PayCycle AS 'Pay Cycle'
                                                                 , Convert(char(10),Tps_StartCycle,101) as 'Start Date'
                                                                 , Convert(char(10),Tps_EndCycle,101) as 'End Date'
                                                                 , Tps_CycleIndicator as 'Cycle'
                                                            FROM T_PaySchedule
                                                            WHERE Tps_CycleIndicator IN ('C','P','S')
                                                            ORDER BY 1 DESC"); 

            DataTable dtResult;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(sqlQuery).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

    }
}
