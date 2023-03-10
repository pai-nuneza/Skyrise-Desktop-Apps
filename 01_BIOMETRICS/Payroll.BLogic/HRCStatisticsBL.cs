using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Collections;

namespace Payroll.BLogic
{
    public enum StatisticsType
    {
        COST_CENTER = 0,
        POSITION = 1,
        RESIGNEES = 2,
        NEW_HIREES = 3,
        EDUCATION = 4,
        CIVIL_STATUS = 5,
        DEMOGRAPHICS = 6,
        SALARY = 7
    }

    public enum StatisticsView
    {
        SUMMARY = 0,
        DETAIL = 1
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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'JOBTITLE'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'JOBSTATUS'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'EMPSTAT'";

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
            string query = @"SELECT Cct_CostCenterCode AS 'Status Code'
                                    , dbo.getCostCenterFullNameV2(Cct_CostCenterCode) AS 'Description'
                            FROM T_CostCenter";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'POSITION'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'JOBLEVEL'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'POSCATGORY'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'EDUCLEVEL'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'SCHOOL'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'COURSE'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'CIVILSTAT'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'RELIGION'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'CITIZEN'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'BLOODGROUP'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'SHOESSIZE'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'SHIRTSIZE'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'HAIRCOLOR'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'EYECOLOR'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'BARANGAY'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'ZIPCODE'";

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
            string query = @"SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'PAYTYPE'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
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
                                'select emt_costcentercode as Costcenter 			
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description			
                                , Emt_Employeeid			
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender			
                                , EmpStat.Adt_AccountDesc as EmploymentStatus			
                                , Isnull(Classification.Adt_AccountDesc, space(1)) as Classification			
                                , JobStatus.Adt_AccountDesc as JobStatus			
                                from t_employeemaster			
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus			
	                                and EmpStat.Adt_AccountType=''EMPSTAT''		
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus			
	                                and JobStatus.Adt_AccountType = ''JOBSTATUS''		
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification			
	                                and Classification.Adt_AccountType = ''JOBTITLE''	
                                {0}',
                                '{1}',			
                                'Count(Emt_EmployeeID)[]',			
                                '{2}'"; 
                        break;
                    case StatisticsType.POSITION:
                        query = @"EXEC CrossTab 'select  emt_costcentercode as Costcenter				
                                , dbo.getCostCenterFullName(emt_costcentercode) as Description				
                                , Isnull(Classification.Adt_AccountDesc, space(1)) as Classification				
                                , Emt_EmployeeID 				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , Isnull(Position.Adt_AccountDesc, space(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, space(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, space(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, space(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Adt_AccountDesc, space(1))  as JobStatus				
                                from t_employeemaster				
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
                                    and EmpStat.Adt_AccountType=''EMPSTAT''			
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
                                    and Position.Adt_AccountType = ''POSITION''			
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
                                    and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
                                    and JobStatus.Adt_AccountType = ''JOBSTATUS''
                                {0}',			
                                '{1}',				
                                'Count(Emt_EmployeeID)[]',				
                                '{2}'	";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"EXEC CrossTab 'select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification				
                                , Emt_EmployeeID 				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , emt_costcentercode as Costcenter 				
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description				
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus
                                , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as JobStatus				
                                , Isnull(Separation.Adt_AccountDesc, SPACE(1))  as ReasonofSeparation				
                                , Round(DATEDIFF(dd,Emt_Hiredate, Emt_separationeffectivitydate)/365.00,3)	as ServiceYears			
                                , CONVERT(Char(6), Emt_separationeffectivitydate,112) as SeparationYearMonth				
                                , CONVERT(Char(4), Emt_separationeffectivitydate,112) as SeparationYear				
                                from t_employeemaster				
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
	                                and EmpStat.Adt_AccountType=''EMPSTAT''			
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
	                                and Position.Adt_AccountType = ''POSITION''			
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
	                                and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
	                                and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
	                                and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
                                    and JobStatus.Adt_AccountType = ''JOBSTATUS''
                                left join T_AccountDetail Separation on Separation.Adt_AccountCode = Emt_SeparationCode				
	                                and Separation.Adt_AccountType = ''SEPARATION'' 
                                {0}',			
                                '{1}',				
                                'Count(Emt_EmployeeID)[]',				
                                '{2}'";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"EXEC CrossTab 'select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification				
                                , Emt_EmployeeID 				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , emt_costcentercode as Costcenter 				
                                , dbo.getCostCenterFullName(emt_costcentercode) as Description				
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as JobStatus	
                                , Round(DATEDIFF(dd,Emt_hiredate,''{3}'')/365.00,3) as ServiceYears		
                                , CONVERT(Char(6), Emt_Hiredate,112) as HireYearMonth				
                                , CONVERT(Char(4), Emt_Hiredate,112) as HireYear				
                                from t_employeemaster				
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
                                    and EmpStat.Adt_AccountType=''EMPSTAT''			
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
                                    and Position.Adt_AccountType = ''POSITION''			
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
                                    and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
                                    and JobStatus.Adt_AccountType = ''JOBSTATUS''
                                {0}',			
                                '{1}',				
                                'Count(Emt_EmployeeID)[]',				
                                '{2}'";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"EXEC CrossTab				
                                'select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification				
                                , Emt_EmployeeID				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , emt_costcentercode as Costcenter 				
                                , dbo.getCostCenterFullName(emt_costcentercode) as Description				
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as JobStatus				
                                , Isnull(EducLevel.adt_accountdesc, SPACE(1)) as EducationalLevel   				
                                , Isnull(School.adt_accountdesc, SPACE(1)) as School   				
                                , Isnull(Course.adt_accountdesc, SPACE(1)) as Course 				
                                from t_employeemaster				
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
	                                and EmpStat.Adt_AccountType= ''EMPSTAT''			
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
	                                and Position.Adt_AccountType = ''POSITION''			
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
	                                and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
	                                and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
	                                and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
	                                and JobStatus.Adt_AccountType = ''JOBSTATUS''			
                                Left Join T_AccountDetail EducLevel on EducLevel.adt_accountcode = Emt_EducLevel   				
	                                and  EducLevel.adt_accounttype = ''EDUCLEVEL''   			
                                Left Join T_AccountDetail School on School.adt_accountcode = Emt_SchoolCode   				
	                                and  School.adt_accounttype = ''SCHOOL''   			
                                Left Join T_AccountDetail Course on Course.adt_accountcode = Emt_CourseCode   				
	                                 and  Course.adt_accounttype = ''COURSE''
                                {0}',			
                                '{1}',				
                                'Count(Emt_EmployeeID)[]',				
                                '{2}' ";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"EXEC CrossTab											
                                'select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification											
                                , Emt_EmployeeID 											
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender											
                                , emt_costcentercode as Costcenter 											
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description											
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position											
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel											
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition											
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus											
                                , Isnull(CivilStatus.Adt_AccountDesc, Space(1)) AS  CivilStatus											
                                , Isnull(Religion.Adt_AccountDesc, SPACE(1)) AS Religion 											
                                , Isnull(Citizenship.Adt_AccountDesc, SPACE(1)) AS Citizenship											
                                , Isnull(BloodType.Adt_AccountDesc, SPACE(1))  AS BloodType											
                                , Isnull(ShoesSize.Adt_AccountCode, SPACE(1)) AS ShoesSize											
                                , Isnull(ShirtSize.Adt_AccountDesc, SPACE(1)) AS ShirtSize											
                                , Isnull(HairColor.Adt_AccountDesc, SPACE(1)) AS HairColor   											
                                , Isnull(EyeColor.Adt_AccountDesc, SPACE(1)) AS EyeColor	
                                , Emt_Age AS Age										
                                from t_employeemaster											
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus											
	                                and EmpStat.Adt_AccountType=''EMPSTAT''										
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode											
	                                and Position.Adt_AccountType = ''POSITION''										
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory											
	                                and PositionCategory.Adt_AccountType = ''POSCATGORY''										
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification											
	                                and Classification.Adt_AccountType = ''JOBTITLE''										
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel											
	                                and JobLevel.Adt_AccountType = ''JOBLEVEL''										
                                LEFT JOIN T_AccountDetail AS Religion ON Religion.Adt_AccountCode = Emt_ReligionCode 											
                                AND   Religion.Adt_AccountType = ''RELIGION'' 											
                                LEFT JOIN  T_AccountDetail AS Citizenship ON Citizenship.Adt_AccountCode = Emt_CitizenshipCode 											
	                                AND  Citizenship.Adt_AccountType = ''CITIZEN''										
                                LEFT  JOIN  T_AccountDetail AS BloodType ON BloodType.Adt_AccountCode = Emt_BloodType    											
	                                AND BloodType.Adt_AccountType = ''BLOODGROUP'' 										
                                LEFT JOIN  T_AccountDetail AS ShoesSize ON ShoesSize.Adt_AccountCode = T_EmployeeMaster.Emt_ShoesSize    											
	                                AND ShoesSize.Adt_AccountType = ''SHOESSIZE'' 										
                                LEFT JOIN T_AccountDetail AS ShirtSize ON ShirtSize.Adt_AccountCode = T_EmployeeMaster.Emt_ShirtSize    											
	                                AND ShirtSize.Adt_AccountType = ''SHIRTSIZE'' 										
                                LEFT JOIN T_AccountDetail AS HairColor ON HairColor.Adt_AccountCode = T_EmployeeMaster.Emt_HairColor    											
	                                AND HairColor.Adt_AccountType = ''HAIRCOLOR'' 										
                                LEFT JOIN T_AccountDetail AS EyeColor ON EyeColor.Adt_AccountCode = T_EmployeeMaster.Emt_EyeColor    											
	                                AND EyeColor.Adt_AccountType = ''EYECOLOR''										
                                 LEFT  JOIN  T_AccountDetail AS CivilStatus ON CivilStatus.Adt_AccountCode = dbo.T_EmployeeMaster.Emt_CivilStatus    											
                                                     AND CivilStatus.Adt_AccountType = ''CIVILSTAT''
                                {0}',											
                                '{1}',											
                                'Count(Emt_EmployeeID)[]',											
                                '{2}'";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"EXEC CrossTab										
                                'select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification										
                                , Emt_EmployeeID										
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender									
                                , emt_costcentercode as Costcenter 										
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description										
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position										
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus										
                                , Isnull(EmpBarangay.Adt_AccountDesc, SPACE(1))  AS BarangayMunicipality										
                                , Isnull(EmpCity.Adt_AccountDesc, SPACE(1))  AS CityProvinceDistrict										
                                from t_employeemaster										
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus										
                                    and EmpStat.Adt_AccountType=''EMPSTAT''									
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode										
                                    and Position.Adt_AccountType = ''POSITION''									
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory										
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''									
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification										
                                    and Classification.Adt_AccountType = ''JOBTITLE''									
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel										
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''									
                                LEFT JOIN T_AccountDetail AS EmpBarangay ON EmpBarangay.Adt_AccountCode = Emt_EmployeeAddress2 										
                                    AND   EmpBarangay.Adt_AccountType = ''BARANGAY'' 									
                                LEFT JOIN T_AccountDetail AS EmpCity ON EmpCity.Adt_AccountCode = Emt_EmployeeAddress3    										
                                    AND  EmpCity.Adt_AccountType = ''ZIPCODE''
                                {0}',										
                                '{1}',										
                                'Count(Emt_EmployeeID)[]',										
                                '{2}'";
                        break;
                    case StatisticsType.SALARY:
                        query = @"EXEC CrossTab										
                                'select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification										
                                , Emt_EmployeeID										
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender										
                                , emt_costcentercode as Costcenter 										
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description										
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position										
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus										
                                , Isnull(Emt_SalaryRate,0) AS SalaryRate										
                                , Isnull(PayrollType.Adt_AccountDesc, SPACE(1)) AS PayrollType										
                                from t_employeemaster										
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus										
                                    and EmpStat.Adt_AccountType=''EMPSTAT''									
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode										
                                    and Position.Adt_AccountType = ''POSITION''									
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory										
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''									
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification										
                                    and Classification.Adt_AccountType = ''JOBTITLE''									
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel										
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''									
                                LEFT JOIN  T_AccountDetail AS PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 										
                                    AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                                {0}',									
                                '{1}',										
                                'Count(Emt_EmployeeID)[]',										
                                '{2}'";
                        break;
                }
            }
            else if (statView == StatisticsView.DETAIL)
            {
                switch (statType)
                {
                    case StatisticsType.COST_CENTER:
                        query = @"select emt_costcentercode as [Costcenter code]				
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]				
                                        , Emt_EmployeeID as [Employee ID]				
                                        , Emt_LastName	as [Last Name]			
                                        , Emt_FirstName	as [First Name]			
                                        , Emt_Middlename as [Middle Name]	 			
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]			
                                        , Classification.Adt_AccountDesc as [Classification]				
                                        , EmpStat.Adt_AccountDesc as [Employment Status]				
                                        , JobStatus.Adt_AccountDesc as [Job Status]				
                                from t_employeemaster				
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
	                                and JobStatus.Adt_AccountType = 'JOBSTATUS'			
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
	                                and Classification.Adt_AccountType = 'JOBTITLE'	
                                {0}		
                                Order by Emt_CostCenterCode				
                                , Emt_LastName				
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.POSITION:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as [Job Status]					
                                from t_employeemaster					
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus					
	                                and JobStatus.Adt_AccountType = 'JOBSTATUS'	
                                {0}			
                                Order by [Classification]					
                                , Emt_LastName					
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , Isnull(Separation.Adt_AccountDesc, SPACE(1))  as [Reason of Separation]					
                                        , CONVERT(Char(10), Emt_Hiredate,101) as [Hire Date]					
                                        , CONVERT(Char(10), Emt_separationeffectivitydate,101) as [Separation Date]					
                                        , Round(DATEDIFF(dd,Emt_Hiredate, Emt_separationeffectivitydate)/365.00,3)	as [Service Years]				
                                from t_employeemaster					
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                left join T_AccountDetail Separation on Separation.Adt_AccountCode = Emt_SeparationCode					
	                                and Separation.Adt_AccountType = 'SEPARATION'	
                                {0}			
                                Order by  [Classification]					
                                , Emt_LastName					
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as [Job Status]	
                                        , Round(DATEDIFF(dd,Emt_hiredate,'{1}')/365.00,3) as [Service Years]					
                                        , CONVERT(Char(10), Emt_Hiredate,101) as [Hire Date]					
                                from t_employeemaster					
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                left join T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus					
	                                and JobStatus.Adt_AccountType = 'JOBSTATUS'	
                                {0}			
                                Order by  [Classification]					
                                , Emt_LastName					
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , EducLevel.adt_accountdesc as [Educational Level]   					
                                        , School.adt_accountdesc as [School]   					
                                        , Course.adt_accountdesc as [Course] 					
                                from t_employeemaster					
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                Left Join T_AccountDetail EducLevel on EducLevel.adt_accountcode = Emt_EducLevel   					
	                                and  EducLevel.adt_accounttype = 'EDUCLEVEL'   				
                                Left Join T_AccountDetail School on School.adt_accountcode = Emt_SchoolCode   					
	                                and  School.adt_accounttype = 'SCHOOL'   				
                                Left Join T_AccountDetail Course on Course.adt_accountcode = Emt_CourseCode   					
	                                 and  Course.adt_accounttype = 'COURSE' 
                                {0}				
                                Order by  [Classification]					
                                , Emt_LastName					
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]											
                                        , Emt_EmployeeID as [Employee ID]											
                                        , Emt_LastName	as [Last Name]										
                                        , Emt_FirstName	as [First Name]										
                                        , Emt_Middlename as [Middle Name]	 										
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , emt_costcentercode as [Costcenter code]											
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]											
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]											
                                        , Isnull(CivilStatus.Adt_AccountDesc, Space(1)) AS [Civil Status]											
                                        , Isnull(Religion.Adt_AccountDesc, SPACE(1)) AS [Religion] 											
                                        , Isnull(Citizenship.Adt_AccountDesc, SPACE(1)) AS [Citizenship]											
                                        , Isnull(BloodType.Adt_AccountDesc, SPACE(1))  AS [Blood Type]											
                                        , Isnull(ShoesSize.Adt_AccountCode, SPACE(1)) AS [Shoes Size]											
                                        , Isnull(ShirtSize.Adt_AccountDesc, SPACE(1)) AS [Shirt Size]											
                                        , Isnull(HairColor.Adt_AccountDesc, SPACE(1)) AS [Hair Color]   											
                                        , Isnull(EyeColor.Adt_AccountDesc, SPACE(1)) AS [Eye Color]	
                                        , Emt_Age AS [Age]										
                                from t_employeemaster											
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus											
	                                and EmpStat.Adt_AccountType='EMPSTAT'										
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode											
	                                and Position.Adt_AccountType = 'POSITION'										
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory											
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'										
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification											
	                                and Classification.Adt_AccountType = 'JOBTITLE'										
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel											
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'										
                                LEFT JOIN T_AccountDetail AS Religion ON Religion.Adt_AccountCode = Emt_ReligionCode 											
                                AND   Religion.Adt_AccountType = 'RELIGION' 											
                                LEFT JOIN  T_AccountDetail AS Citizenship ON Citizenship.Adt_AccountCode = Emt_CitizenshipCode 											
	                                AND  Citizenship.Adt_AccountType = 'CITIZEN'										
                                LEFT  JOIN  T_AccountDetail AS BloodType ON BloodType.Adt_AccountCode = Emt_BloodType    											
	                                AND BloodType.Adt_AccountType = 'BLOODGROUP' 										
                                LEFT JOIN  T_AccountDetail AS ShoesSize ON ShoesSize.Adt_AccountCode = T_EmployeeMaster.Emt_ShoesSize    											
	                                AND ShoesSize.Adt_AccountType = 'SHOESSIZE' 										
                                LEFT JOIN T_AccountDetail AS ShirtSize ON ShirtSize.Adt_AccountCode = T_EmployeeMaster.Emt_ShirtSize    											
	                                AND ShirtSize.Adt_AccountType = 'SHIRTSIZE' 										
                                LEFT JOIN T_AccountDetail AS HairColor ON HairColor.Adt_AccountCode = T_EmployeeMaster.Emt_HairColor    											
	                                AND HairColor.Adt_AccountType = 'HAIRCOLOR' 										
                                LEFT JOIN T_AccountDetail AS EyeColor ON EyeColor.Adt_AccountCode = T_EmployeeMaster.Emt_EyeColor    											
	                                AND EyeColor.Adt_AccountType = 'EYECOLOR'     										
                                 LEFT  JOIN  T_AccountDetail AS CivilStatus ON CivilStatus.Adt_AccountCode = dbo.T_EmployeeMaster.Emt_CivilStatus    											
                                    AND CivilStatus.Adt_AccountType = 'CIVILSTAT'
                                {0}											
                                Order by  [Classification]											
                                , Emt_LastName											
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]										
                                        , Emt_EmployeeID as [Employee ID]										
                                        , Emt_LastName	as [Last Name]									
                                        , Emt_FirstName	as [First Name]									
                                        , Emt_Middlename as [Middle Name]	 									
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]									
                                        , emt_costcentercode as [Costcenter code]										
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]										
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]										
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]										
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]										
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]										
                                        , Isnull(EmpBarangay.Adt_AccountDesc, SPACE(1))  AS [Barangay/Municipality]										
                                        , Isnull(EmpCity.Adt_AccountDesc, SPACE(1))  AS [City/Province/District]										
                                from t_employeemaster										
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus										
	                                and EmpStat.Adt_AccountType='EMPSTAT'									
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode										
	                                and Position.Adt_AccountType = 'POSITION'									
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory										
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'									
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification										
	                                and Classification.Adt_AccountType = 'JOBTITLE'									
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel										
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'									
                                LEFT JOIN T_AccountDetail AS EmpBarangay ON EmpBarangay.Adt_AccountCode = Emt_EmployeeAddress2 										
	                                AND   EmpBarangay.Adt_AccountType = 'BARANGAY' 									
                                LEFT JOIN T_AccountDetail AS EmpCity ON EmpCity.Adt_AccountCode = Emt_EmployeeAddress3    										
                                    AND  EmpCity.Adt_AccountType = 'ZIPCODE'
                                {0}										
                                Order by  [Classification]										
                                , Emt_LastName										
                                , Emt_FirstName	";
                        break;
                    case StatisticsType.SALARY:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]											
                                        , Emt_EmployeeID as [Employee ID]											
                                        , Emt_LastName	as [Last Name]										
                                        , Emt_FirstName	as [First Name]										
                                        , Emt_Middlename as [Middle Name]	 										
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , emt_costcentercode as [Costcenter code]											
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]											
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]											
                                        , Isnull(Emt_SalaryRate,0) AS [Salary Rate]											
                                        , Isnull(PayrollType.Adt_AccountDesc, SPACE(1)) AS [Payroll Type]											
                                from t_employeemaster											
                                left join T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus											
	                                and EmpStat.Adt_AccountType='EMPSTAT'										
                                left join T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode											
	                                and Position.Adt_AccountType = 'POSITION'										
                                left join T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory											
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'										
                                left join T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification											
	                                and Classification.Adt_AccountType = 'JOBTITLE'										
                                left join T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel											
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'										
                                LEFT JOIN  T_AccountDetail AS PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 											
	                                AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                                {0}										
                                Order by  [Classification]											
                                , Emt_LastName											
                                , Emt_FirstName	";
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
                        query = @"select emt_costcentercode as Costcenter 			
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description			
                                , Emt_Employeeid			
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender			
                                , EmpStat.Adt_AccountDesc as EmploymentStatus			
                                , Isnull(Classification.Adt_AccountDesc, space(1)) as Classification			
                                , JobStatus.Adt_AccountDesc as JobStatus			
                                from {0}..t_employeemaster			
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus			
	                                and EmpStat.Adt_AccountType=''EMPSTAT''		
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus			
	                                and JobStatus.Adt_AccountType = ''JOBSTATUS''		
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification			
	                                and Classification.Adt_AccountType = ''JOBTITLE''	
                                {1}";
                        break;
                    case StatisticsType.POSITION:
                        query = @"select  emt_costcentercode as Costcenter				
                                , dbo.getCostCenterFullName(emt_costcentercode) as Description				
                                , Isnull(Classification.Adt_AccountDesc, space(1)) as Classification				
                                , Emt_EmployeeID 				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , Isnull(Position.Adt_AccountDesc, space(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, space(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, space(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, space(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Adt_AccountDesc, space(1))  as JobStatus				
                                from {0}..t_employeemaster				
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
                                    and EmpStat.Adt_AccountType=''EMPSTAT''			
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
                                    and Position.Adt_AccountType = ''POSITION''			
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
                                    and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
                                    and JobStatus.Adt_AccountType = ''JOBSTATUS''
                                {1}";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification				
                                , Emt_EmployeeID 				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender				
                                , emt_costcentercode as Costcenter 				
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description				
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus
                                , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as JobStatus				
                                , Isnull(Separation.Adt_AccountDesc, SPACE(1))  as ReasonofSeparation				
                                , Round(DATEDIFF(dd,Emt_Hiredate, Emt_separationeffectivitydate)/365.00,3)	as ServiceYears			
                                , CONVERT(Char(6), Emt_separationeffectivitydate,112) as SeparationYearMonth				
                                , CONVERT(Char(4), Emt_separationeffectivitydate,112) as SeparationYear				
                                from {0}..t_employeemaster				
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
	                                and EmpStat.Adt_AccountType=''EMPSTAT''			
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
	                                and Position.Adt_AccountType = ''POSITION''			
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
	                                and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
	                                and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
	                                and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
                                    and JobStatus.Adt_AccountType = ''JOBSTATUS''
                                left join {0}..T_AccountDetail Separation on Separation.Adt_AccountCode = Emt_SeparationCode				
	                                and Separation.Adt_AccountType = ''SEPARATION'' 
                                {1}";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification				
                                , Emt_EmployeeID 				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , emt_costcentercode as Costcenter 				
                                , dbo.getCostCenterFullName(emt_costcentercode) as Description				
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as JobStatus	
                                , Round(DATEDIFF(dd,Emt_hiredate,''{2}'')/365.00,3) as ServiceYears		
                                , CONVERT(Char(6), Emt_Hiredate,112) as HireYearMonth				
                                , CONVERT(Char(4), Emt_Hiredate,112) as HireYear				
                                from {0}..t_employeemaster				
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
                                    and EmpStat.Adt_AccountType=''EMPSTAT''			
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
                                    and Position.Adt_AccountType = ''POSITION''			
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
                                    and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
                                    and JobStatus.Adt_AccountType = ''JOBSTATUS''
                                {1}";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification				
                                , Emt_EmployeeID				
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender			
                                , emt_costcentercode as Costcenter 				
                                , dbo.getCostCenterFullName(emt_costcentercode) as Description				
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position				
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel				
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition				
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus				
                                , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as JobStatus				
                                , Isnull(EducLevel.adt_accountdesc, SPACE(1)) as EducationalLevel   				
                                , Isnull(School.adt_accountdesc, SPACE(1)) as School   				
                                , Isnull(Course.adt_accountdesc, SPACE(1)) as Course 				
                                from {0}..t_employeemaster				
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
	                                and EmpStat.Adt_AccountType= ''EMPSTAT''			
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode				
	                                and Position.Adt_AccountType = ''POSITION''			
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory				
	                                and PositionCategory.Adt_AccountType = ''POSCATGORY''			
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
	                                and Classification.Adt_AccountType = ''JOBTITLE''			
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel				
	                                and JobLevel.Adt_AccountType = ''JOBLEVEL''			
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
	                                and JobStatus.Adt_AccountType = ''JOBSTATUS''			
                                Left Join {0}..T_AccountDetail EducLevel on EducLevel.adt_accountcode = Emt_EducLevel   				
	                                and  EducLevel.adt_accounttype = ''EDUCLEVEL''   			
                                Left Join {0}..T_AccountDetail School on School.adt_accountcode = Emt_SchoolCode   				
	                                and  School.adt_accounttype = ''SCHOOL''   			
                                Left Join {0}..T_AccountDetail Course on Course.adt_accountcode = Emt_CourseCode   				
	                                 and  Course.adt_accounttype = ''COURSE''
                                {1}";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification											
                                , Emt_EmployeeID 											
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender											
                                , emt_costcentercode as Costcenter 											
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description											
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position											
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel											
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition											
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus											
                                , Isnull(CivilStatus.Adt_AccountDesc, Space(1)) AS  CivilStatus											
                                , Isnull(Religion.Adt_AccountDesc, SPACE(1)) AS Religion 											
                                , Isnull(Citizenship.Adt_AccountDesc, SPACE(1)) AS Citizenship											
                                , Isnull(BloodType.Adt_AccountDesc, SPACE(1))  AS BloodType											
                                , Isnull(ShoesSize.Adt_AccountCode, SPACE(1)) AS ShoesSize											
                                , Isnull(ShirtSize.Adt_AccountDesc, SPACE(1)) AS ShirtSize											
                                , Isnull(HairColor.Adt_AccountDesc, SPACE(1)) AS HairColor   											
                                , Isnull(EyeColor.Adt_AccountDesc, SPACE(1)) AS EyeColor	
                                , Emt_Age AS Age										
                                from {0}..t_employeemaster											
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus											
	                                and EmpStat.Adt_AccountType=''EMPSTAT''										
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode											
	                                and Position.Adt_AccountType = ''POSITION''										
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory											
	                                and PositionCategory.Adt_AccountType = ''POSCATGORY''										
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification											
	                                and Classification.Adt_AccountType = ''JOBTITLE''										
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel											
	                                and JobLevel.Adt_AccountType = ''JOBLEVEL''										
                                LEFT JOIN {0}..T_AccountDetail Religion ON Religion.Adt_AccountCode = Emt_ReligionCode 											
                                AND   Religion.Adt_AccountType = ''RELIGION'' 											
                                LEFT JOIN  {0}..T_AccountDetail Citizenship ON Citizenship.Adt_AccountCode = Emt_CitizenshipCode 											
	                                AND  Citizenship.Adt_AccountType = ''CITIZEN''										
                                LEFT  JOIN  {0}..T_AccountDetail BloodType ON BloodType.Adt_AccountCode = Emt_BloodType    											
	                                AND BloodType.Adt_AccountType = ''BLOODGROUP'' 										
                                LEFT JOIN  {0}..T_AccountDetail ShoesSize ON ShoesSize.Adt_AccountCode = Emt_ShoesSize    											
	                                AND ShoesSize.Adt_AccountType = ''SHOESSIZE'' 										
                                LEFT JOIN {0}..T_AccountDetail ShirtSize ON ShirtSize.Adt_AccountCode = Emt_ShirtSize    											
	                                AND ShirtSize.Adt_AccountType = ''SHIRTSIZE'' 										
                                LEFT JOIN {0}..T_AccountDetail HairColor ON HairColor.Adt_AccountCode = Emt_HairColor    											
	                                AND HairColor.Adt_AccountType = ''HAIRCOLOR'' 										
                                LEFT JOIN {0}..T_AccountDetail EyeColor ON EyeColor.Adt_AccountCode = Emt_EyeColor    											
	                                AND EyeColor.Adt_AccountType = ''EYECOLOR''										
                                 LEFT  JOIN  {0}..T_AccountDetail CivilStatus ON CivilStatus.Adt_AccountCode = Emt_CivilStatus    											
                                    AND CivilStatus.Adt_AccountType = ''CIVILSTAT''
                                {1}";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification										
                                , Emt_EmployeeID										
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end	as Gender									
                                , emt_costcentercode as Costcenter 										
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description										
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position										
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus										
                                , Isnull(EmpBarangay.Adt_AccountDesc, SPACE(1))  AS BarangayMunicipality										
                                , Isnull(EmpCity.Adt_AccountDesc, SPACE(1))  AS CityProvinceDistrict										
                                from {0}..t_employeemaster										
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus										
                                    and EmpStat.Adt_AccountType=''EMPSTAT''									
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode										
                                    and Position.Adt_AccountType = ''POSITION''									
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory										
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''									
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification										
                                    and Classification.Adt_AccountType = ''JOBTITLE''									
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel										
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''									
                                LEFT JOIN {0}..T_AccountDetail AS EmpBarangay ON EmpBarangay.Adt_AccountCode = Emt_EmployeeAddress2 										
                                    AND   EmpBarangay.Adt_AccountType = ''BARANGAY'' 									
                                LEFT JOIN {0}..T_AccountDetail AS EmpCity ON EmpCity.Adt_AccountCode = Emt_EmployeeAddress3    										
                                    AND  EmpCity.Adt_AccountType = ''ZIPCODE''
                                {1}";
                        break;
                    case StatisticsType.SALARY:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as Classification										
                                , Emt_EmployeeID										
                                , Case when Emt_Gender = ''M'' then ''MALE'' else ''FEMALE'' end as Gender										
                                , emt_costcentercode as Costcenter 										
                                , dbo.getCostCenterFullNameV2(emt_costcentercode) as Description										
                                , Isnull(Position.Adt_AccountDesc, SPACE(1)) as Position										
                                , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as JobLevel										
                                , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as ClusterPosition										
                                , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as EmploymentStatus										
                                , Isnull(Emt_SalaryRate,0) AS SalaryRate										
                                , Isnull(PayrollType.Adt_AccountDesc, SPACE(1)) AS PayrollType										
                                from {0}..t_employeemaster										
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus										
                                    and EmpStat.Adt_AccountType=''EMPSTAT''									
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode										
                                    and Position.Adt_AccountType = ''POSITION''									
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory										
                                    and PositionCategory.Adt_AccountType = ''POSCATGORY''									
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification										
                                    and Classification.Adt_AccountType = ''JOBTITLE''									
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel										
                                    and JobLevel.Adt_AccountType = ''JOBLEVEL''									
                                LEFT JOIN  {0}..T_AccountDetail AS PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 										
                                    AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                                {1}";
                        break;
                }
            }
            else if (statView == StatisticsView.DETAIL)
            {
                switch (statType)
                {
                    case StatisticsType.COST_CENTER:
                        query = @"select emt_costcentercode as [Costcenter code]				
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]				
                                        , Emt_EmployeeID as [Employee ID]				
                                        , Emt_LastName	as [Last Name]			
                                        , Emt_FirstName	as [First Name]			
                                        , Emt_Middlename as [Middle Name]	 			
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]			
                                        , Classification.Adt_AccountDesc as [Classification]				
                                        , EmpStat.Adt_AccountDesc as [Employment Status]				
                                        , JobStatus.Adt_AccountDesc as [Job Status]				
                                from {0}..t_employeemaster				
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus				
                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus				
	                                and JobStatus.Adt_AccountType = 'JOBSTATUS'			
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification				
	                                and Classification.Adt_AccountType = 'JOBTITLE'	
                                {1}		";
                        break;
                    case StatisticsType.POSITION:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as [Job Status]					
                                from {0}..t_employeemaster					
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus					
	                                and JobStatus.Adt_AccountType = 'JOBSTATUS'	
                                {1}		";
                        break;
                    case StatisticsType.RESIGNEES:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , Isnull(Separation.Adt_AccountDesc, SPACE(1))  as [Reason of Separation]					
                                        , CONVERT(Char(10), Emt_Hiredate,101) as [Hire Date]					
                                        , CONVERT(Char(10), Emt_separationeffectivitydate,101) as [Separation Date]					
                                        , Round(DATEDIFF(dd,Emt_Hiredate, Emt_separationeffectivitydate)/365.00,3)	as [Service Years]				
                                from {0}..t_employeemaster					
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                left join {0}..T_AccountDetail Separation on Separation.Adt_AccountCode = Emt_SeparationCode					
	                                and Separation.Adt_AccountType = 'SEPARATION'	
                                {1}		";
                        break;
                    case StatisticsType.NEW_HIREES:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , Isnull(JobStatus.Adt_AccountDesc, SPACE(1))  as [Job Status]	
                                        , Round(DATEDIFF(dd,Emt_hiredate,'{2}')/365.00,3) as [Service Years]					
                                        , CONVERT(Char(10), Emt_Hiredate,101) as [Hire Date]					
                                from {0}..t_employeemaster					
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                left join {0}..T_AccountDetail JobStatus on JobStatus.Adt_AccountCode = Emt_JobStatus					
	                                and JobStatus.Adt_AccountType = 'JOBSTATUS'	
                                {1}	";
                        break;
                    case StatisticsType.EDUCATION:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]					
                                        , Emt_EmployeeID as [Employee ID]					
                                        , Emt_LastName	as [Last Name]				
                                        , Emt_FirstName	as [First Name]				
                                        , Emt_Middlename as [Middle Name]	 				
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]				
                                        , emt_costcentercode as [Costcenter code]					
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]					
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]					
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]					
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]					
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]					
                                        , EducLevel.adt_accountdesc as [Educational Level]   					
                                        , School.adt_accountdesc as [School]   					
                                        , Course.adt_accountdesc as [Course] 					
                                from {0}..t_employeemaster					
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus					
	                                and EmpStat.Adt_AccountType='EMPSTAT'				
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode					
	                                and Position.Adt_AccountType = 'POSITION'				
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory					
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'				
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification					
	                                and Classification.Adt_AccountType = 'JOBTITLE'				
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel					
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'				
                                Left Join {0}..T_AccountDetail EducLevel on EducLevel.adt_accountcode = Emt_EducLevel   					
	                                and  EducLevel.adt_accounttype = 'EDUCLEVEL'   				
                                Left Join {0}..T_AccountDetail School on School.adt_accountcode = Emt_SchoolCode   					
	                                and  School.adt_accounttype = 'SCHOOL'   				
                                Left Join {0}..T_AccountDetail Course on Course.adt_accountcode = Emt_CourseCode   					
	                                 and  Course.adt_accounttype = 'COURSE' 
                                {1}		";
                        break;
                    case StatisticsType.CIVIL_STATUS:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]											
                                        , Emt_EmployeeID as [Employee ID]											
                                        , Emt_LastName	as [Last Name]										
                                        , Emt_FirstName	as [First Name]										
                                        , Emt_Middlename as [Middle Name]	 										
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , emt_costcentercode as [Costcenter code]											
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]											
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]											
                                        , Isnull(CivilStatus.Adt_AccountDesc, Space(1)) AS [Civil Status]											
                                        , Isnull(Religion.Adt_AccountDesc, SPACE(1)) AS [Religion] 											
                                        , Isnull(Citizenship.Adt_AccountDesc, SPACE(1)) AS [Citizenship]											
                                        , Isnull(BloodType.Adt_AccountDesc, SPACE(1))  AS [Blood Type]											
                                        , Isnull(ShoesSize.Adt_AccountCode, SPACE(1)) AS [Shoes Size]											
                                        , Isnull(ShirtSize.Adt_AccountDesc, SPACE(1)) AS [Shirt Size]											
                                        , Isnull(HairColor.Adt_AccountDesc, SPACE(1)) AS [Hair Color]   											
                                        , Isnull(EyeColor.Adt_AccountDesc, SPACE(1)) AS [Eye Color]	
                                        , Emt_Age AS [Age]										
                                from {0}..t_employeemaster											
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus											
	                                and EmpStat.Adt_AccountType='EMPSTAT'										
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode											
	                                and Position.Adt_AccountType = 'POSITION'										
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory											
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'										
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification											
	                                and Classification.Adt_AccountType = 'JOBTITLE'										
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel											
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'										
                                LEFT JOIN {0}..T_AccountDetail AS Religion ON Religion.Adt_AccountCode = Emt_ReligionCode 											
                                AND   Religion.Adt_AccountType = 'RELIGION' 											
                                LEFT JOIN  {0}..T_AccountDetail AS Citizenship ON Citizenship.Adt_AccountCode = Emt_CitizenshipCode 											
	                                AND  Citizenship.Adt_AccountType = 'CITIZEN'										
                                LEFT  JOIN  {0}..T_AccountDetail AS BloodType ON BloodType.Adt_AccountCode = Emt_BloodType    											
	                                AND BloodType.Adt_AccountType = 'BLOODGROUP' 										
                                LEFT JOIN  {0}..T_AccountDetail AS ShoesSize ON ShoesSize.Adt_AccountCode = Emt_ShoesSize    											
	                                AND ShoesSize.Adt_AccountType = 'SHOESSIZE' 										
                                LEFT JOIN {0}..T_AccountDetail AS ShirtSize ON ShirtSize.Adt_AccountCode = Emt_ShirtSize    											
	                                AND ShirtSize.Adt_AccountType = 'SHIRTSIZE' 										
                                LEFT JOIN {0}..T_AccountDetail AS HairColor ON HairColor.Adt_AccountCode = Emt_HairColor    											
	                                AND HairColor.Adt_AccountType = 'HAIRCOLOR' 										
                                LEFT JOIN {0}..T_AccountDetail AS EyeColor ON EyeColor.Adt_AccountCode = Emt_EyeColor    											
	                                AND EyeColor.Adt_AccountType = 'EYECOLOR'     										
                                 LEFT  JOIN  {0}..T_AccountDetail AS CivilStatus ON CivilStatus.Adt_AccountCode = Emt_CivilStatus    											
                                    AND CivilStatus.Adt_AccountType = 'CIVILSTAT'
                                {1}		";
                        break;
                    case StatisticsType.DEMOGRAPHICS:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]										
                                        , Emt_EmployeeID as [Employee ID]										
                                        , Emt_LastName	as [Last Name]									
                                        , Emt_FirstName	as [First Name]									
                                        , Emt_Middlename as [Middle Name]	 									
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]									
                                        , emt_costcentercode as [Costcenter code]										
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]										
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]										
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]										
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]										
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]										
                                        , Isnull(EmpBarangay.Adt_AccountDesc, SPACE(1))  AS [Barangay/Municipality]										
                                        , Isnull(EmpCity.Adt_AccountDesc, SPACE(1))  AS [City/Province/District]										
                                from {0}..t_employeemaster										
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus										
	                                and EmpStat.Adt_AccountType='EMPSTAT'									
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode										
	                                and Position.Adt_AccountType = 'POSITION'									
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory										
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'									
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification										
	                                and Classification.Adt_AccountType = 'JOBTITLE'									
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel										
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'									
                                LEFT JOIN {0}..T_AccountDetail AS EmpBarangay ON EmpBarangay.Adt_AccountCode = Emt_EmployeeAddress2 										
	                                AND   EmpBarangay.Adt_AccountType = 'BARANGAY' 									
                                LEFT JOIN {0}..T_AccountDetail AS EmpCity ON EmpCity.Adt_AccountCode = Emt_EmployeeAddress3    										
                                    AND  EmpCity.Adt_AccountType = 'ZIPCODE'
                                {1}		";
                        break;
                    case StatisticsType.SALARY:
                        query = @"select  Isnull(Classification.Adt_AccountDesc, SPACE(1)) as [Classification]											
                                        , Emt_EmployeeID as [Employee ID]											
                                        , Emt_LastName	as [Last Name]										
                                        , Emt_FirstName	as [First Name]										
                                        , Emt_Middlename as [Middle Name]	 										
                                        , Case when Emt_Gender = 'M' then 'MALE' else 'FEMALE' end	as [Gender]										
                                        , emt_costcentercode as [Costcenter code]											
                                        , dbo.getCostCenterFullNameV2(emt_costcentercode) as [Description]											
                                        , Isnull(Position.Adt_AccountDesc, SPACE(1)) as [Position]											
                                        , Isnull(JobLevel.Adt_AccountDesc, SPACE(1)) as [Job Level]											
                                        , Isnull(PositionCategory.Adt_AccountDesc, SPACE(1)) as [Cluster Position]											
                                        , Isnull(EmpStat.Adt_AccountDesc, SPACE(1))  as [Employment Status]											
                                        , Isnull(Emt_SalaryRate,0) AS [Salary Rate]											
                                        , Isnull(PayrollType.Adt_AccountDesc, SPACE(1)) AS [Payroll Type]											
                                from {0}..t_employeemaster											
                                left join {0}..T_AccountDetail EmpStat on EmpStat.Adt_AccountCode = Emt_EmploymentStatus											
	                                and EmpStat.Adt_AccountType='EMPSTAT'										
                                left join {0}..T_AccountDetail Position on Position.Adt_AccountCode = Emt_PositionCode											
	                                and Position.Adt_AccountType = 'POSITION'										
                                left join {0}..T_AccountDetail PositionCategory on PositionCategory.Adt_AccountCode = Emt_PositionCategory											
	                                and PositionCategory.Adt_AccountType = 'POSCATGORY'										
                                left join {0}..T_AccountDetail Classification on Classification.Adt_AccountCode = Emt_Classification											
	                                and Classification.Adt_AccountType = 'JOBTITLE'										
                                left join {0}..T_AccountDetail JobLevel on JobLevel.Adt_AccountCode = Emt_JobLevel											
	                                and JobLevel.Adt_AccountType = 'JOBLEVEL'										
                                LEFT JOIN  {0}..T_AccountDetail AS PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 											
	                                AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                                {1}	";
                        break;
                }
            }
            return query;
        }

        public bool CanUserViewRate(string UserCode, DALHelper dal)
        {
            string query = string.Format(@"SELECT Umt_CanViewRate
                                            FROM M_User
                                            WHERE Mur_UserCode = '{0}'", UserCode);

            DataSet ds = new DataSet();
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();

            if (ds.Tables[0].Rows.Count > 0)
                if (Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString()))
                    return true;

            return false;
        }
    }
}
