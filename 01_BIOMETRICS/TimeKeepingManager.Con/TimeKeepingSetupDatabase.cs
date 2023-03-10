using System;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
using System.Threading;
using System.Windows.Forms;

namespace TimeKeepingManager.Con
{
    class TimeKeepingSetupDatabase
    {
        /// <summary>
        /// Authour : Nilo L. Luansing Jr. Dev 1105
        /// Date : 11/04/2013
        /// Desc : Functions use to alter DTR database compatible for Time Keeping Manager Application.
        /// </summary>

        private DALHelper _Dal = new DALHelper(true);
        private bool isExist = false;
        TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();

        #region Constructor
        public TimeKeepingSetupDatabase()
        {
            SetupTables();
            SetupSP();
            TableCreator(DatabaseManager.TableName, DatabaseManager.CreateQuery, DatabaseManager.ProcedureName, DatabaseManager.ProcedureQuery);
            //Check for culomns.
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@TableNameValue", "T_EmpRFFP_%", SqlDbType.VarChar);
            param[1] = new ParameterInfo("@CulomnNameValue", "Trf_FaceData", SqlDbType.VarChar);
            param[2] = new ParameterInfo("@CulomnTypeValue", "Varchar(MAX) NULL", SqlDbType.VarChar);
            AddCulomn(param);
            if(!isExist)
                TransferRecord();
            Console.WriteLine("Setup Completed.");
        }
        #endregion

        #region Methods

        private void AddCulomn(ParameterInfo[] Param)
        {

            #region Query

            string Query = @"   DECLARE @TableName VarChar(150)
                                DECLARE @CulomnName VarChar(150)
                                DECLARE @CulomnType VarChar(150)
                                
                                SET @TableName = @TableNameValue
                                SET @CulomnName = @CulomnNameValue
                                SET @CulomnType = @CulomnTypeValue

                                DECLARE @Result AS Table
                                (
	                                Status Varchar(150) NULL
                                )

                                DECLARE @FPTABLE VARCHAR(150)

                                DECLARE FPCURSOR CURSOR FOR
                                SELECT name
                                  FROM sys.tables
                                 WHERE name like @TableName
 
                                OPEN FPCURSOR 

                                FETCH NEXT FROM FPCURSOR  
                                INTO @FPTABLE
                                WHILE @@FETCH_STATUS = 0 

                                BEGIN

	                                IF NOT EXISTS(SELECT 1 
					                                FROM sys.columns 
				                                   WHERE Name = @CulomnName and Object_ID = Object_ID(@FPTABLE))
		                                BEGIN
			                                EXEC('ALTER TABLE ' + @FPTABLE + ' ADD ' + @CulomnName + ' ' + @CulomnType)
			                                INSERT INTO @Result(Status) Values ('Altered. ' + @FPTABLE)
		                                END
	                                ELSE
		                                BEGIN
			                                INSERT INTO @Result(Status) Values ('Skipped. ' + @FPTABLE)
		                                END


	                                FETCH NEXT FROM FPCURSOR  
	                                    INTO   @FPTABLE

                                END 

                                CLOSE FPCURSOR;
                                DEALLOCATE FPCURSOR; 

                                BEGIN
	                                SELECT * FROM @Result
                                END";

            #endregion

            using (_Dal)
            {
                try
                {
                    _Dal.OpenDB();
                    DataTable dt = _Dal.ExecuteDataSet(Query, CommandType.Text, Param).Tables[0];
                    Console.WriteLine(String.Format("Alter Tables. Add column : {0}", Param[1].Value) + "\n");
                    foreach (DataRow dr in dt.Rows)
                    {
                        Console.WriteLine(dr["Status"].ToString() + "\n");
                        Thread.Sleep(2000);
                    }
                    dt.Dispose();

                    //Alter device table add default logtype per device.
                    string alterdeviceQuery = @"
    
                                                IF NOT EXISTS
		                                              (SELECT 1
			                                             FROM sys.columns 
			                                            WHERE [name] = N'Mtd_DefaultLogType' 
			                                              AND [object_id] = OBJECT_ID(N'M_TerminalDevice'))
	                                            BEGIN
			                                            ALTER TABLE M_TerminalDevice
					                                            ADD Mtd_DefaultLogType CHAR(1)
		                                                SELECT 'DONE' [RESULT]
	                                            END
	                                            ELSE        
	                                            BEGIN
		                                                SELECT 'EXIST' [RESULT]
	                                            END";

                    DataTable dtDevice = _Dal.ExecuteDataSet(alterdeviceQuery, CommandType.Text, Param).Tables[0];
                    Console.WriteLine(String.Format("Alter Table M_TerminalDevice. Add column : {0}", "Mtd_DefaultLogType") + "\n");
                    foreach (DataRow dr in dtDevice.Rows)
                    {
                        Console.WriteLine(dr["RESULT"].ToString() + "\n");
                        Thread.Sleep(2000);
                    }
                    dtDevice.Dispose();
                }
                catch
                { }
                finally
                {
                    _Dal.CloseDB();
                }
            }
        }

        private void TableCreator(String[] TableNameList,String[] CreateTableList, String[] SPNameList, String[] CreateSPList)
        { 
            String Exist = string.Empty;
            using (_Dal)
            {
                try
                {
                    _Dal.OpenDB();

                    #region Creating Tables
                    for(int i = 0; i < DatabaseManager.T_Count; ++i)
                    {
                        _Dal.BeginTransaction();
                        Exist = _Dal.ExecuteDataSet(
                                String.Format(@"SELECT COUNT(NAME) AS TableName FROM SYS.tables WHERE NAME='{0}'", TableNameList[i]), CommandType.Text).Tables[0].Rows[0]["TableName"].ToString().Trim();
                        if (Exist == "0")
                        {
                            _Dal.ExecuteNonQuery(CreateTableList[i], CommandType.Text);
                            _Dal.CommitTransaction();
                            Console.WriteLine(String.Format("Creating Table {0} ...\n", TableNameList[i]));
                            _log.WriteLog(Application.StartupPath, "SetupDB", "Table Creation Success", String.Format("Creating table {0} ...\n", TableNameList[i]), true);
                        }
                        else
                        {
                            _Dal.RollBackTransaction();
                            Console.WriteLine(String.Format("Table {0} already exist...\n", TableNameList[i]));
                            _log.WriteLog(Application.StartupPath, "SetupDB", "Table Creation Success", String.Format("Table {0} already exists...\n", TableNameList[i]), true);
                            if (TableNameList[i].ToString() == "M_TerminalDevice2")
                            {
                                isExist = true;
                            }
                        }
                        Thread.Sleep(2000);
                    }
                    #endregion

                    #region Creating Stored Procedures
                    for (int i = 0; i < DatabaseManager.SP_Count; ++i)
                    {
                        _Dal.BeginTransaction();
                        Exist = _Dal.ExecuteDataSet(
                                String.Format(@"SELECT COUNT(NAME) AS SPName FROM SYS.procedures WHERE NAME='{0}'", SPNameList[i]), CommandType.Text).Tables[0].Rows[0]["SPName"].ToString().Trim();
                        if (Exist == "0")
                        {
                            _Dal.ExecuteNonQuery(CreateSPList[i], CommandType.Text);
                            _Dal.CommitTransaction();
                            Console.WriteLine(String.Format("Creating StoredProc {0} ...\n", SPNameList[i]));
                            _log.WriteLog(Application.StartupPath, "SetupDB", "StoredProc Creation Success", String.Format("Creating StoredProc {0} ...\n", SPNameList[i]), true);
                        }
                        else
                        {
                            _Dal.RollBackTransaction();
                            Console.WriteLine(String.Format("StoredProc {0} already exist...\n", SPNameList[i]));
                            _log.WriteLog(Application.StartupPath, "SetupDB", "StoredProc Creation Success", String.Format("StoredProc {0} already exists...\n", SPNameList[i]), true);
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    Console.WriteLine("End\n");
                }
                catch (Exception e)
                {
                    _Dal.RollBackTransaction();
                }
                finally
                {
                    _Dal.CloseDB();
                }
            }
        }

        //private void AlterTable
        private void SetupTables()
        {
            //TableNames
            DatabaseManager.TableName[0] = "T_TKSysLog";
            DatabaseManager.TableName[1] = "T_EmpDTR";
            DatabaseManager.TableName[2] = "T_EmpDTRDevice";
            DatabaseManager.TableName[3] = "M_FingerPrint";
            DatabaseManager.TableName[4] = "M_TerminalDevice";
            DatabaseManager.TableName[5] = "M_Schedule";
            DatabaseManager.TableName[6] = "T_EmpSubstandardDTR";
            DatabaseManager.TableName[7] = "M_TerminalDevice2";
            
            //Create Query
            #region T_TKSysLog
            DatabaseManager.CreateQuery[0] = @"CREATE TABLE [dbo].[T_TKSysLog](
                                                    [Tkl_CompanyCode] [varchar](10) NOT NULL,
	                                                [Tkl_UserIPAddress] [varchar](30) NOT NULL,
	                                                [Tkl_UserMacAddress] [varchar](50) NOT NULL,
	                                                [Tkl_Function] [varchar](100) NOT NULL,
	                                                [Ludatetime] [datetime] NOT NULL
                                                ) ON [PRIMARY]";
            #endregion
            #region T_EmpDTR

            DatabaseManager.CreateQuery[1] = @"   CREATE TABLE [dbo].[T_EmpDTR](
	                                                            [Tel_IDNo] [varchar](15) NOT NULL,
	                                                            [Tel_LogDate] [varchar](10) NOT NULL,
	                                                            [Tel_LogTime] [char](4) NOT NULL,
	                                                            [Tel_LogType] [char](1) NOT NULL,
	                                                            [Tel_StationNo] [char](2) NOT NULL,
	                                                            [Tel_IsPosted] [bit] NOT NULL,
	                                                            [Usr_Login] [varchar](15) NOT NULL,
	                                                            [LudateTime] [datetime] NOT NULL,
	                                                            [Tel_IsUploaded] [bit] NULL,
                                                     CONSTRAINT [PK_T_EmpDTR] PRIMARY KEY CLUSTERED 
                                                     (
	                                                            [Tel_IDNo] ASC,
	                                                            [Tel_LogDate] ASC,
	                                                            [Tel_LogTime] ASC,
	                                                            [Tel_LogType] ASC
                                                     )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                            ) ON [PRIMARY]";

            #endregion
            #region T_EmpDTRDevice

            DatabaseManager.CreateQuery[2] = @"   CREATE TABLE [dbo].[T_EmpDTRDevice](
	                                                            [Tel_IDNo] [varchar](15) NOT NULL,
	                                                            [Tel_LogDate] [varchar](10) NOT NULL,
	                                                            [Tel_LogTime] [char](4) NOT NULL,
	                                                            [Tel_LogType] [char](1) NOT NULL,
	                                                            [Tel_StationNo] [char](2) NOT NULL,
	                                                            [Tel_IsPosted] [bit] NOT NULL,
	                                                            [Usr_Login] [varchar](15) NOT NULL,
	                                                            [LudateTime] [datetime] NOT NULL,
	                                                            [Tel_IsUploaded] [bit] NULL,
                                                     CONSTRAINT [PK_T_EmpDTRDevice] PRIMARY KEY CLUSTERED 
                                                     (
	                                                            [Tel_IDNo] ASC,
	                                                            [Tel_LogDate] ASC,
	                                                            [Tel_LogTime] ASC,
	                                                            [Tel_LogType] ASC
                                                     )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                           ) ON [PRIMARY]";

            #endregion
            #region M_FingerPrint

            DatabaseManager.CreateQuery[3] = @"   CREATE TABLE [dbo].[M_FingerPrint](
	                                                            [Mfp_TableName] [varchar](150) NOT NULL,
	                                                            [Mfp_Description] [varchar](max) NULL,
	                                                            [Mfp_RecordStatus] [char](1) NOT NULL,
	                                                            [Usr_Login] [varchar](15) NULL,
	                                                            [Ludatetime] [datetime] NULL,
                                                     CONSTRAINT [PK_M_FingerPrint] PRIMARY KEY CLUSTERED 
                                                     (
	                                                            [Mfp_TableName] ASC
                                                     )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                           ) ON [PRIMARY]";

            #endregion
            #region M_TerminalDevice

            DatabaseManager.CreateQuery[4] = @"   CREATE TABLE [dbo].[M_TerminalDevice](
	                                                            [Mtd_DeviceIP] [varchar](50) NOT NULL,
                                                                [Mtd_DeviceName] [varchar](50) NOT NULL,
	                                                            [Mtd_DevicePort] [char](10) NOT NULL,
	                                                            [Mtd_RS232Com] [char](5) NULL,
	                                                            [Mtd_RS232BaudRate] [char](10) NULL,
	                                                            [Mtd_LastLoadedTable] [varchar](100) NULL,
	                                                            [Mtd_TableName] [varchar](100) NULL,
	                                                            [Mtd_RecordStatus] [char](1) NOT NULL,
	                                                            [Usr_Login] [varchar](15) NULL,
	                                                            [Ludatetime] [datetime] NULL,
	                                                            [Mtd_VersionNo] [char](15) NULL
                                                     CONSTRAINT [PK_M_TerminalDevice] PRIMARY KEY CLUSTERED 
                                                     (
	                                                            [Mtd_DeviceIP] ASC
                                                               ,[Mtd_DeviceName] ASC
                                                     )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                           ) ON [PRIMARY]";
            #endregion
            #region M_Schedule
            DatabaseManager.CreateQuery[5] = @"   CREATE TABLE [dbo].[M_Schedule](
	                                                            [Msc_ScheduleCode] [nchar](15) NOT NULL,
	                                                            [Msc_ScheduleName] [varchar](50) NULL,
	                                                            [Msc_DeviceIP] [varchar](20) NOT NULL,
	                                                            [Msc_FrYear] [datetime] NOT NULL,
	                                                            [Msc_ToYear] [datetime] NOT NULL,
	                                                            [Msc_FrMonth] [datetime] NOT NULL,
	                                                            [Msc_ToMonth] [datetime] NOT NULL,
	                                                            [Msc_Mon] [bit] NOT NULL,
	                                                            [Msc_Tue] [bit] NOT NULL,
	                                                            [Msc_Wed] [bit] NOT NULL,
	                                                            [Msc_Thu] [bit] NOT NULL,
	                                                            [Msc_Fri] [bit] NOT NULL,
	                                                            [Msc_Sat] [bit] NOT NULL,
	                                                            [Msc_Sun] [bit] NOT NULL,
	                                                            [Msc_FrTime] [datetime] NOT NULL,
	                                                            [Msc_ToTime] [datetime] NOT NULL,
	                                                            [Msc_AllowRFID] [bit] NOT NULL,
	                                                            [Msc_AllowFP] [bit] NOT NULL,
	                                                            [Msc_AllowFace] [bit] NOT NULL,
	                                                            [Msc_AllowPin] [bit] NOT NULL,
	                                                            [Msc_ComboMode] [bit] NOT NULL,
	                                                            [Msc_AddedDate] [datetime] NOT NULL,
	                                                            [Msc_LastActiveDate] [datetime] NOT NULL,
	                                                            [Msc_RecordStatus] [char](1) NOT NULL,
	                                                            [Usr_Login] [varchar](15) NOT NULL,
	                                                            [LudateTime] [datetime] NOT NULL,
                                                             CONSTRAINT [PK_M_Schedule] PRIMARY KEY CLUSTERED 
                                                            (
	                                                            [Msc_ScheduleCode] ASC
                                                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                            ) ON [PRIMARY]";
            #endregion
            #region T_EmpSubstandardDTR
            DatabaseManager.CreateQuery[6] = @"   CREATE TABLE [dbo].[T_EmpSubstandardDTR](
	                                                            [Tsd_IDNo] [varchar](15) NOT NULL,
	                                                            [Tsd_LogDate] [varchar](10) NOT NULL,
	                                                            [Tsd_LogTime] [char](4) NOT NULL,
	                                                            [Tsd_LogType] [char](1) NOT NULL,
	                                                            [Tsd_LogDateTime] [datetime] NOT NULL,
	                                                            [Tsd_SubStandardID] [varchar](25) NULL,
	                                                            [Tsd_VerificationType] [varchar](15),
	                                                            [Tsd_StationType] [varchar](25),
	                                                            [Tsd_StationNo] [char](25) NULL,
	                                                            [Tsd_UploadedDate] [bit] NOT NULL,
	                                                            [Usr_Login] [varchar](15) NOT NULL,
	                                                            [LudateTime] [datetime] NOT NULL,
                                                             CONSTRAINT [PK_T_EmpSubstandardDTR] PRIMARY KEY CLUSTERED 
                                                            (
	                                                            [Tsd_IDNo] ASC,
	                                                            [Tsd_LogDate] ASC,
	                                                            [Tsd_LogTime] ASC,
	                                                            [Tsd_LogType] ASC,
	                                                            [Tsd_VerificationType] ASC,
	                                                            [Tsd_StationType] ASC
                                                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                            ) ON [PRIMARY]";
            #endregion
            #region M_TerminalDevice2
            DatabaseManager.CreateQuery[7] = @"CREATE TABLE [dbo].[M_TerminalDevice2](
	                                                        [Mtd_ID] [INT] NOT NULL IDENTITY(1,1),
	                                                        [Mtd_DeviceIP] [varchar](50) NOT NULL,
	                                                        [Mtd_DeviceName] [varchar](50) NOT NULL,
	                                                        [Mtd_DevicePort] [char](10) NOT NULL,
	                                                        [Mtd_RS232Com] [char](5) NULL,
	                                                        [Mtd_RS232BaudRate] [char](10) NULL,
	                                                        [Mtd_LastLoadedTable] [varchar](100) NULL,
	                                                        [Mtd_TableName] [varchar](100) NULL,
	                                                        [Mtd_RecordStatus] [char](1) NOT NULL,
	                                                        [Usr_Login] [varchar](15) NULL,
	                                                        [Ludatetime] [datetime] NULL,
	                                                        [Mtd_VersionNo] [char](15) NULL,
	                                                        [Mtd_DefaultLogType] [char](1) NULL,
                                                         CONSTRAINT [PK_M_TerminalDevice2] PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [Mtd_ID] ASC, 
	                                                        [Mtd_DeviceIP] ASC,
	                                                        [Mtd_DeviceName] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                        ) ON [PRIMARY]"; 
            #endregion            
        }

        private void SetupSP()
        { 
            //StoredProc
            DatabaseManager.ProcedureName[0] = "spLogReadingInsertToServerDTRMinMax";
            //Create Query
            #region spLogReadingInsertToServerDTRMinMax
            DatabaseManager.ProcedureQuery[0] = @"CREATE PROCEDURE [dbo].[spLogReadingInsertToServerDTRMinMax]          
                                                        (          
                                                          @Tel_IDNo VARCHAR(15)          
                                                         ,@Tel_LogDate VARCHAR(10)          
                                                         ,@Tel_LogTime CHAR(4)          
                                                         ,@Tel_LogType CHAR(1)          
                                                         ,@Tel_StationNo CHAR(2)          
                                                         ,@Tel_IsPosted BIT          
                                                         ,@Tel_IsUploaded BIT          
                                                         ,@Usr_Login VARCHAR(15)          
                                                         ,@Ludatetime DATETIME          
                                                        )          
                                                        AS          
                                                        IF NOT EXISTS           
                                                          (SELECT 1          
                                                             FROM T_EmpDTR           
                                                            WHERE Tel_IDNo = @Tel_IDNo          
                                                              AND Tel_LogDate = @Tel_LogDate          
                                                             -- AND Tel_LogTime = @Tel_LogTime  -- Ignoring time to select only the Min IN and Max  OUT
                                                              AND Tel_LogType = @Tel_LogType)          
                                                          BEGIN          
                                                           INSERT INTO T_EmpDTR          
                                                                               (Tel_IDNo          
                                                                               ,Tel_LogDate          
                                                                               ,Tel_LogTime          
                                                                               ,Tel_LogType          
                                                                               ,Tel_StationNo          
                                                                               ,Tel_IsPosted          
                                                                               ,Tel_IsUploaded          
                                                                               ,Usr_Login          
                                                                               ,LudateTime          
                                                                               )          
                                                                        VALUES (@Tel_IDNo          
                                                                               ,@Tel_LogDate          
                                                                               ,@Tel_LogTime          
                                                                               ,@Tel_LogType          
                                                                               ,@Tel_StationNo          
                                                                               ,@Tel_IsPosted          
                                                                               ,@Tel_IsUploaded          
                                                                               ,@Usr_Login          
                                                                               ,@LudateTime          
                                                                               )                                                                                       
                                                            IF (ISNULL((SELECT Tel_LastSwipe           
                                                                 FROM T_EmpLog          
                                                                WHERE Tel_IDNo = @Tel_IDNo), '1900-01-01')          
                                                             <= CAST(@Tel_LogDate + ' ' + LEFT(@Tel_LogTime, 2) + ':' + RIGHT(@Tel_LogTime, 2) AS DATETIME))          
                                                            BEGIN          
                                                             UPDATE T_EmpLog          
                                                                SET Tel_LastSwipe = CAST(@Tel_LogDate + ' ' + LEFT(@Tel_LogTime, 2) + ':' + RIGHT(@Tel_LogTime, 2) AS DATETIME)          
                                                                   ,Tel_LastLogType = @Tel_LogType          
                                                              WHERE Tel_IDNo = @Tel_IDNo          
                                                            END          
                                                          END ";
            #endregion
        }

        private void TransferRecord()
        {
            string deviceIP = string.Empty;
            try
            {
                string qInsert = @"
                            INSERT INTO M_TerminalDevice2
                            SELECT *
                                FROM M_TerminalDevice

                            DELETE FROM M_TerminalDevice
                            ";
                string qSelect = @"SELECT Mtd_ID
                                    , Mtd_DeviceIP
                                    , Mtd_DeviceName
                                 FROM M_TerminalDevice2";

                DataTable dtResult = new DataTable();
                using (_Dal)
                {
                    _Dal.OpenDB();
                    _Dal.ExecuteNonQuery(qInsert);
                    dtResult = _Dal.ExecuteDataSet(qSelect).Tables[0];

                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        deviceIP = Encrypt.encryptText(dtResult.Rows[i]["Mtd_DeviceIP"].ToString() + "_" + dtResult.Rows[i]["Mtd_ID"].ToString());
                        _Dal.ExecuteNonQuery(string.Format(@"UPDATE M_TerminalDevice2
                                                        SET Mtd_DeviceIP = '{0}'
                                                        WHERE Mtd_DeviceIP = '{1}'
                                                        AND Mtd_ID = '{2}'
                                                        AND Mtd_DeviceName = '{3}'", deviceIP
                                                                                       , dtResult.Rows[i]["Mtd_DeviceIP"].ToString()
                                                                                       , dtResult.Rows[i]["Mtd_ID"].ToString()
                                                                                       , dtResult.Rows[i]["Mtd_DeviceName"].ToString()));

                    }
                }
            }
            catch
            {
            }
            finally
            {
                _Dal.CloseDB();
            }
        }
        #endregion

        private class DatabaseManager
        {
            public static int T_Count = 8;
            public static String[] TableName = new String[T_Count];
            public static String[] CreateQuery = new String[T_Count];

            public static int SP_Count = 2;
            public static String[] ProcedureName = new String[SP_Count];
            public static String[] ProcedureQuery = new String[SP_Count];
        }

        private class TableInfo
        {
            string TableName = "";
            string ColumName = "";
            string StrDataType = "";
            public TableInfo(string tableName, string culomnname, string strdatatype)
            {
                this.TableName = tableName;
                this.ColumName = culomnname;
                this.StrDataType = strdatatype;
            }
        }
    }
}
