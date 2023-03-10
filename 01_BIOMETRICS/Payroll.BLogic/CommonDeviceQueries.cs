using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.BLogic
{
    public static class CommonDeviceQueries
    {
        public static string SelectEmployeeFingerPrintData(string Database, 
                                                           string DeviceID, 
                                                           string UserLogin)
        {
            return string.Format(@"   SELECT [Trf_EmployeeID]
                                            ,[Trf_RFID]
                                            ,[Trf_FingerIndex]
                                            ,[Trf_Template]
                                            ,[Trf_Privilege]
                                            ,[Trf_Password]
                                            ,[Trf_Enabled]
                                            ,[Trf_Flag]
                                            ,GetDate()
                                            ,'{2}'
                                            ,[Trf_EmployeeIDMapping]
                                            ,[Trf_FaceData]
                                        FROM [dbo].[{0}]
                                        where Trf_EmployeeID='{1}'"
                                            ,Database
                                            ,DeviceID
                                            ,UserLogin);
        }

        public static string SelectAllEmployeeData(string Database)
        {
            //Note that the ordering of this data is very important.
            //It must be ordered by ID, FP index.
            //Since the looping procedure of loading to device is dependent on the ID sequence for assigning of Finger Print, RFID, Face Data.
            return string.Format(@"   SELECT [Trf_EmployeeID] as EmployeeID
                                            ,[Trf_RFID] as RFID
                                            ,[Trf_FingerIndex] as FingerIndex
                                            ,[Trf_Template] as Template
                                            ,[Trf_Privilege] as Privilege
                                            ,[Trf_Password] as Password
                                            ,[Trf_Enabled] as Enabled
                                            ,[Trf_Flag] as Flag
                                            ,[ludatetime] as Ludatetime
                                            ,[Usr_Login] as UserLogin
                                            ,[Trf_FaceData] as FaceData
                                        FROM [{0}]
                                       WHERE (ISNULL(Trf_Template, '') <> '')
                                          OR ((ISNULL(Trf_RFID, '') <> ''and Trf_RFID <> '0')
										  OR ISNULL(Trf_FaceData, '') <> '')
                                        ORDER BY Trf_EmployeeID, Trf_FingerIndex", Database);
        }

        public static string SelectEmployeeDataDisplay(string Database)
        { 
            return string.Format(@"    SELECT CONVERT(VARCHAR(100),[Trf_EmployeeID]) AS [PK]
	                                         ,[Trf_EmployeeID] AS [XXX_DeviceID]
		                                     ,IsNull([Trf_EmployeeIDMapping], CASE WHEN IsNull([Tel_LastName], '') = '' THEN ' UNMAPPED' ELSE [Tel_IDNo] END) AS [XXX_EmployeeID]
		                                     ,IsNull([Tel_LastName], ' UNMAPPED') [XXX_LastName]
		                                     ,IsNull([Tel_FirstName], ' UNMAPPED') [XXX_FirstName]
		                                     ,IsNull([Tel_MiddleName], '') [XXX_MiddleName]
		                                     ,(SELECT COUNT([Trf_FingerIndex]) FROM [{0}] WHERE MAIN.Trf_EmployeeID = Trf_EmployeeID AND LEN([Trf_Template]) > 0) AS [XXX_TotalFPIndex]
		                                     ,CASE WHEN IsNull([Trf_FaceData], '') = '' THEN 'None' ELSE 'Ok' END AS [XXX_FaceData]
		                                     ,[Trf_RFID] AS [XXX_RFID]
                                             ,CASE WHEN LEN([Trf_Template]) < 1 THEN ''
                                                   WHEN Trf_Template like 'oco%' THEN '9.0' --Base on finger print data only.
                                                   ELSE '10.0' END [XXX_Algorithm]
	                                     FROM [{0}] MAIN
                                    LEFT JOIN T_EmpLog
	                                       ON (ISNUMERIC(Tel_IDNo) = 1 AND CONVERT(BIGINT,Trf_employeeid) = CONVERT(BIGINT,Tel_IDNo))
	                                       OR (Trf_EmployeeIDMapping = Tel_IDNo)
	                                    WHERE [Trf_FingerIndex] = 0
                                     ORDER BY [PK], [XXX_LastName], [XXX_FirstName]", Database);
        }

        public static string SelectEmployeeFromTableGroup(string TableName)
        {
            return string.Format(@"    SELECT CONVERT(VARCHAR(100),[Trf_EmployeeID]) AS [PK]
	                                         ,[Trf_EmployeeID] AS [DeviceID]
                                             ,ISNULL([Trf_EmployeeIDMapping],'') AS [ID Number]
		                                     ---,ISNULL([Trf_EmployeeIDMapping], CASE WHEN ISNULL([Tel_LastName], '') = '' THEN ' UNMAPPED' ELSE [Tel_IDNo] END) AS [ID Number]
		                                     ,CASE WHEN Tel_LastName IS NOT NULL THEN  Tel_LastName + ', ' + Tel_FirstName + ' ' + Tel_MiddleName ELSE 'UNMAPPED' END AS [Name] 
                                             ---,ISNULL([Tel_LastName], ' UNMAPPED') AS [Last Name]
		                                     ---,ISNULL([Tel_FirstName], ' UNMAPPED') AS [First Name]
		                                     ---,ISNULL([Tel_MiddleName], ' UNMAPPED') AS [Middle Name]
		                                     ,(SELECT COUNT([Trf_FingerIndex]) FROM [{0}] WHERE MAIN.Trf_EmployeeID = Trf_EmployeeID AND LEN([Trf_Template]) > 0) AS [Finger Print Index Count]
		                                     ---,CASE WHEN IsNull([Trf_FaceData], '') = '' THEN 'None' ELSE 'Ok' END AS [FaceData]
		                                     ,[Trf_RFID] AS [RFID]
                                             ---,CASE WHEN LEN([Trf_Template]) < 1 THEN ''
                                             ---      WHEN Trf_Template like 'oco%' THEN '9.0' --Base on finger print data only.
                                             --- ELSE '10.0' END [Algorithm]
                                            , CASE WHEN Tel_RecordStatus = 'A' THEN 'Active' WHEN Tel_RecordStatus = 'C' THEN 'Inactive' ELSE '' END AS [Work Status]
                                            , '' AS [Remarks]
                                            ---, Trf_EmployeeIDMapping AS [EmployeeIDMapping]
	                                     FROM [{0}] MAIN
                                    LEFT JOIN T_EmpLog
	                                       ON ((ISNUMERIC(Tel_IDNo) = 1 AND CONVERT(BIGINT,Trf_EmployeeID) = CONVERT(BIGINT,Tel_IDNo))
	                                       OR (Trf_EmployeeIDMapping = Tel_IDNo))
	                                    WHERE [Trf_FingerIndex] = 0
                                     ORDER BY [Name]", TableName);
        }

        public static string SelectMappingTableDisplay(string TableName)
        {
            return String.Format(@"   SELECT [Trf_EmployeeIDMapping] AS [MappingID]
                                            ,[Trf_EmployeeID] AS [DeviceID]
                                            ,CASE WHEN [Tel_LastName] IS NULL THEN '' ELSE [Tel_LastName] END [LastName]
                                            ,CASE WHEN [Tel_FirstName] IS NULL THEN '' ELSE [Tel_FirstName] END [FirstName]
                                            ,CASE WHEN [Tel_MiddleName] IS NULL THEN '' ELSE [Tel_MiddleName] END [MiddleName]
                                            ,(SELECT COUNT([Trf_FingerIndex]) FROM [{0}] WHERE MAIN.Trf_EmployeeID = Trf_EmployeeID  AND LEN([Trf_Template]) > 0)AS [TotalFPIndex]
                                            ,CASE WHEN IsNull([Trf_FaceData], '') = '' THEN 'None' ELSE 'Ok' END AS [FaceData]
                                            ,[Trf_RFID] AS [RFID]
                                       FROM [{0}] MAIN
                                  LEFT JOIN T_EmpLog
                                         ON (ISNUMERIC(Tel_IDNo)=1
                                        AND CONVERT(BIGINT,Trf_EmployeeID) = CONVERT(BIGINT,Tel_IDNo))
                                         OR (Trf_EmployeeIDMapping = Tel_IDNo)
                                      WHERE [Trf_FingerIndex]=0
                                      ORDER BY 1,2", TableName);
        }

        public static string CreateEmployeeFingerPrintTable(string TableName)
        {
            return string.Format(@"CREATE TABLE [dbo].[{0}](
	                                        [Trf_EmployeeID] [varchar](15) NOT NULL,
	                                        [Trf_RFID] [varchar](15) NULL,
	                                        [Trf_FingerIndex] [tinyint] NOT NULL,
	                                        [Trf_Template] [varchar](max) NULL,
	                                        [Trf_Privilege] [tinyint] NULL,
	                                        [Trf_Password] [varchar](15) NOT NULL,
	                                        [Trf_Enabled] [bit] NOT NULL,
	                                        [Trf_Flag] [bit] NOT NULL,
                                            [Trf_EmployeeIDMapping] [varchar](15) NULL,
	                                        [Trf_FaceData] [varchar](max) NULL,
                                            [Usr_Login] [varchar](15) NULL,
	                                        [Ludatetime] [datetime] NOT NULL,
                                         CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                                        (
	                                        [Trf_EmployeeID] ASC,
	                                        [Trf_FingerIndex] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]", TableName);
        }

        public static string CreateEmployeeFingerPrintTableTemp()
        {
            return string.Format(@"IF OBJECT_ID('tempdb..#TEMPRF') IS NOT NULL
                                        DROP TABLE #TEMPRF

                                    CREATE TABLE #TEMPRF (
										[Trf_EmployeeID] [varchar](15) NOT NULL,
										[Trf_RFID] [varchar](15) NULL,
										[Trf_FingerIndex] [tinyint] NOT NULL,
										[Trf_Template] [varchar](max) NULL,
										[Trf_Privilege] [tinyint] NULL,
										[Trf_Password] [varchar](15) NOT NULL,
										[Trf_Enabled] [bit] NOT NULL,
										[Trf_Flag] [bit] NOT NULL,
										[Trf_EmployeeIDMapping] [varchar](15) NULL,
										[Trf_FaceData] [varchar](max) NULL,
										[Usr_Login] [varchar](15) NULL,
										[Ludatetime] [datetime] NOT NULL)");
        }

        public static string CreateEmployeeDeviceData(string TableName)
        {
            return string.Format(@"IF EXISTS(SELECT Trf_EmployeeID FROM {0}
	                                          WHERE Trf_EmployeeID = @EmployeeID
                                                AND Trf_FingerIndex = @FingerIndex)
	                                    BEGIN
		                                     UPDATE {0}
                                                SET [Trf_RFID] = Case When Len(RTRIM(@RFID)) > 0 Then @RFID Else [Trf_RFID] End 
                                                   ,[Trf_Template] = Case When Len(RTRIM(@template)) > 0 Then @template Else [Trf_Template] End 
                                                   ,[Trf_Privilege] = @Privilege
                                                   ,[Trf_Password] = @Password
                                                   ,[Trf_Enabled] = @Enabled
                                                   ,[Trf_Flag] = @Flag
                                                   ,[ludatetime] = @Ludatetime
                                                   ,[Usr_Login] = @UsrLogin
                                                   ,[Trf_FaceData] = Case When Len(RTRIM(@FaceData)) > 0 Then  @FaceData Else [Trf_FaceData] End
                                              WHERE [Trf_EmployeeID] = @EmployeeID
                                                AND [Trf_FingerIndex] = @FingerIndex
	                                    END
                                    ELSE
                                        BEGIN

                                        INSERT INTO {0}
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
                                                  ,[Trf_FaceData])
                                              VALUES
                                                  (@EmployeeID
                                                  ,@RFID
                                                  ,@FingerIndex
                                                  ,@template
                                                  ,@Privilege
                                                  ,@Password
                                                  ,@Enabled
                                                  ,@Flag
                                                  ,@Ludatetime
                                                  ,@UsrLogin
                                                  ,@FaceData)
                                         END", TableName);
        }

        public static string InsertDataFromAnotherDatabase(string SourceTableName, string TargetTableName, string SourceDeviceID, string Usr_Login)
        {
            return string.Format(@"IF EXISTS(SELECT Trf_EmployeeID FROM [{1}] WHERE Trf_EmployeeID = '{2}' )
	                                    UPDATE [{1}] SET Trf_RFID       = A.Trf_RFID
                                                , Trf_Template          = A.Trf_Template
                                                , Trf_Privilege         = A.Trf_Privilege
                                                , Trf_Password          = A.Trf_Password
                                                , Trf_Enabled           = A.Trf_Enabled
                                                , Trf_Flag              = A.Trf_Flag
                                                , Trf_EmployeeIDMapping = A.Trf_EmployeeIDMapping
                                                , Trf_FaceData          = A.Trf_FaceData
                                                , Trf_FingerIndex       = A.Trf_FingerIndex
                                                , Usr_Login             = '{3}'
                                                , Ludatetime            = GETDATE()
                                        FROM [{0}] A
                                        INNER JOIN [{1}] B ON A.Trf_EmployeeID = B.Trf_EmployeeID
                                            AND A.Trf_FingerIndex = B.Trf_FingerIndex
                                        WHERE A.Trf_EmployeeID = '{2}'
                                    ELSE
	                                    INSERT INTO [{1}]
                                                  ([Trf_EmployeeID]
                                                  ,[Trf_RFID]
                                                  ,[Trf_FingerIndex]
                                                  ,[Trf_Template]
                                                  ,[Trf_Privilege]
                                                  ,[Trf_Password]
                                                  ,[Trf_Enabled]
                                                  ,[Trf_Flag]
                                                  ,[Trf_EmployeeIDMapping]
                                                  ,[Trf_FaceData]
                                                  ,[Usr_Login]
                                                  ,[Ludatetime])
                                        SELECT [Trf_EmployeeID]
                                                  ,[Trf_RFID]
                                                  ,[Trf_FingerIndex]
                                                  ,[Trf_Template]
                                                  ,[Trf_Privilege]
                                                  ,[Trf_Password]
                                                  ,[Trf_Enabled]
                                                  ,[Trf_Flag]
                                                  ,[Trf_EmployeeIDMapping]
                                                  ,[Trf_FaceData]
                                                  ,'{3}'
                                                  ,GETDATE()
                                        FROM [{0}]
                                        WHERE Trf_EmployeeID = '{2}'
                                        ORDER BY Trf_EmployeeID, Trf_FingerIndex"
                                          , SourceTableName
                                          , TargetTableName
                                          , SourceDeviceID
                                          , Usr_Login);
        }

        public static string UpdateIDMapping(string TableName, string DeviceID, string IDMapping, string Usr_Login)
        {
            return string.Format(@"UPDATE @DTRDB..[{0}] SET Trf_EmployeeIDMapping = '{2}'
                                       , Usr_Login = '{3}'
                                       , Ludatetime = GETDATE()
                                   WHERE Trf_EmployeeID = '{1}'
                                    ", TableName
                                    , DeviceID
                                    , IDMapping
                                    , Usr_Login);
        }
        public static string SelectStandardVerificationTypeShedule()
        {
            return @"    SELECT [Msc_DeviceIP] as [IP]
		                        ,DatePart(Year, [Msc_FrYear]) as [From Year]
		                        ,DatePart(Year, [Msc_ToYear]) as [To Year]
		                        ,DatePart(Month,[Msc_FrMonth]) as [From Month]
		                        ,DatePart(Month,[Msc_ToMonth]) as [To Month]
		                        ,Substring(Convert(char,[Msc_FrTime],108), 0, 6) as [From Time]
		                        ,Substring(Convert(char,[Msc_ToTime],108), 0, 6) as [To Time]
		                        ,[Msc_Mon] as [MON] 
		                        ,[Msc_Tue] as [TUE]
		                        ,[Msc_Wed] as [WED]
		                        ,[Msc_Thu] as [THU]
		                        ,[Msc_Fri] as [FRI]
		                        ,[Msc_Sat] as [SAT]
		                        ,[Msc_Sun] as [SUN]
		                        ,[Msc_AllowPin]  as [Verify-1]
		                        ,[Msc_AllowRFID] as [Verify-2]
		                        ,[Msc_AllowFP]   as [Verify-3]
		                        ,[Msc_AllowFace] as [Verify-4]
		                        ,[Msc_ComboMode] as [Combo]
	                        FROM [dbo].[M_Schedule] 
	                        JOIN [dbo].[M_TerminalDevice2]
		                        ON Msc_DeviceIP =  Mtd_DeviceIP
		                        AND Msc_RecordStatus = 'A'
		                        AND Mtd_RecordStatus = 'A'
	                        ORDER BY Msc_ScheduleCode Desc";
        }

        public static string SelectEmployeePicture(string TableName)
        {
            return string.Format(@" SELECT [Trf_EmployeeID] AS [EmployeeID]
				                          ,[Tel_Picture] [Picture]
		                              FROM [{0}] MAIN
	                             LEFT JOIN T_EmpLog
		                                ON ((ISNUMERIC(Tel_IDNo) = 1 AND CONVERT(BIGINT,Trf_EmployeeID) = CONVERT(BIGINT,Tel_IDNo))
		                                OR (Trf_EmployeeIDMapping = Tel_IDNo))
		                             WHERE [Trf_FingerIndex] = 0
                                       AND ((ISNULL(Trf_Template, '') <> '')
	                                    OR ((ISNULL(Trf_RFID, '') <> '' AND Trf_RFID <> '0')
	                                    OR ISNULL(Trf_FaceData, '') <> ''))
		                               AND NOT (Tel_Picture IS NULL)", TableName);
        }

        public static string GenerateDeviceIDMapping()
        {
            return string.Format(@"
                                BEGIN --MAPPING
                                SET NOCOUNT ON;                                            


                                DECLARE @FPTABLE CHAR(50)
                                DECLARE @UNIONEDTABLE VARCHAR(MAX)
                                DECLARE @INDEX INT

	                                BEGIN

                                    SET @INDEX = 0
    
	                                DECLARE FPCURSOR CURSOR FOR
                                    SELECT Mfp_TableName FROM M_FingerPrint
                                    WHERE Mfp_RecordStatus = 'A'

                                    OPEN FPCURSOR --Opening Cursor

                                        FETCH NEXT FROM FPCURSOR  --Get the content of the table set to your cursor
                                        INTO @FPTABLE
                                        WHILE @@FETCH_STATUS = 0 

                                            --Starting Method
                                            BEGIN
                                                IF(@INDEX=0)
                                                    BEGIN
                                                        SET @UNIONEDTABLE = 'SELECT Trf_EmployeeID as [EmployeeID] ,
						                                                            Trf_EmployeeIDMapping as [MappingID],
																					NULL as [EmployeeName]
											                                   FROM dbo.' + @FPTABLE + 
																			      'WHERE Trf_EmployeeIDMapping IS NOT NULL
																				     AND Trf_EmployeeID <> Trf_EmployeeIDMapping'
                                                    END
                                                ELSE
                                                    BEGIN
                                                        SET @UNIONEDTABLE = @UNIONEDTABLE + ' UNION 
														                                     SELECT Trf_EmployeeID as [EmployeeID] ,
															                                        Trf_EmployeeIDMapping as [MappingID], 
																									NULL as [EmployeeName]
															                                   FROM dbo.' + @FPTABLE + 
																							   'WHERE Trf_EmployeeIDMapping IS NOT NULL
																								  AND Trf_EmployeeID <> Trf_EmployeeIDMapping'
                                                    END

                                                SET @INDEX = @INDEX+1

                                            FETCH NEXT FROM FPCURSOR  --this Line serves as incrementation to your loop
                                                INTO   @FPTABLE

                                            END
                                        --Closing Cursor
                                        CLOSE FPCURSOR;
                                        DEALLOCATE FPCURSOR; 

	                                END

                                    DECLARE @MappingTable AS TABLE
                                    (
                                        EmployeeID CHAR(15) NULL ,
		                                MappingID CHAR(15) NULL,
										EmployeeName CHAR(30) NULL
                                    )

                                    BEGIN
                                        INSERT INTO @MappingTable
                                        EXECUTE(@UNIONEDTABLE)
                                    END

	                                SELECT EmployeeID ,
	                                       MappingID ,
										   Tel_FirstName EmployeeName,
										   'TRUE' as IsMapped
                                      FROM @MappingTable
							     LEFT JOIN T_EmpLog 
									    ON MappingID = Tel_IDNo
							           
									 UNION

									SELECT Tel_IDNo as EmployeeID,
										   Tel_IDNo as MappingID,
										   Tel_FirstName as EmployeeName,
										   'FALSE' as IsMapped
									  FROM T_EmpLog
								 LEFT JOIN @MappingTable
									    ON Tel_IDNo <> MappingID

                                  ORDER BY EmployeeName 

                                END");
        }
    }
}
