using System;
using System.Data;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class PayrollRegisterBL
    {
        public DataTable GetDayCodeOT(bool bMiscellaneous, string PayrollType, string PremiumGroup, string CompanyCode, string CentralProfile)
        {
            string condition = string.Empty;
            if (bMiscellaneous)
                condition = "AND ISNULL(Mmd_MiscDayID,0) > 0";
            else condition = "AND ISNULL(Mmd_MiscDayID,0) = 0";

            #region query
            string query = string.Format(@"
                            SELECT Mdp_SequenceOfDisplay  as [Seq]
                            , ISNULL(Mmd_MiscDayID,0) as [DayID]
							, CASE WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode = 'REST' then ''
                            WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode <> 'REST' then 'REST'
                               ELSE '' END +   
                                    CASE 
                                    WHEN Mdp_DayCode = 'HOL' then 'LEGHOL'
                                    WHEN Mdp_DayCode =  'SPL' then 'SPLHOL'
                                    WHEN Mdp_DayCode =  'COMP' then 'COMPHOL'
                                    ELSE  Mdp_DayCode END as [DayCode]
                                       , CASE Mdp_RestDayFlag 
                                        WHEN 1 THEN 
                                          CASE WHEN Mdp_DayCode = 'REST' THEN Mdy_DayName
                                          ELSE Mdy_DayName + ' FALLING ON A RESTDAY' END
                                        ELSE Mdy_DayName END as [DayName]
                            FROM M_DayPremium
                            LEFT JOIN M_Day ON Mdp_DayCode = Mdy_DayCode
                                  AND Mdp_CompanyCode = Mdy_CompanyCode
                                  AND Mdy_RecordStatus = 'A'
                            LEFT JOIN M_MiscellaneousDay ON Mmd_DayCode = Mdp_DayCode
                                  AND Mmd_RestDayFlag = Mdp_RestDayFlag
                                  AND Mmd_CompanyCode = Mdp_CompanyCode
                                  AND Mmd_RecordStatus = 'A'
							WHERE Mdp_PayrollType = '{0}' 
							    AND Mdp_RecordStatus = 'A'
                                AND Mdp_CompanyCode = '{2}'
                                AND Mdp_PremiumGrpCode = '{3}'
							    {1}
							ORDER BY Mdp_SequenceOfDisplay"
                        , PayrollType
                        , condition
                        , CompanyCode
                        , PremiumGroup);
            #endregion

            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDayCodeOT(string PayrollType, string PremiumGroup, string CompanyCode, string CentralProfile)
        {
            string condition = string.Empty;
            #region query
            string query = string.Format(@"
                                 SELECT Mdp_SequenceOfDisplay AS [Seq]
                                 , ISNULL(Mmd_MiscDayID,0) AS [DayID]
							     , CASE WHEN Mdp_RestDayFlag = 1 AND Mdp_DayCode = 'REST' THEN ''
                                   WHEN Mdp_RestDayFlag = 1 AND Mdp_DayCode <> 'REST' THEN 'REST'
                                   ELSE '' END +   
                                    CASE 
                                        WHEN Mdp_DayCode = 'HOL' THEN 'LEGHOL'
                                        WHEN Mdp_DayCode =  'SPL' THEN 'SPLHOL'
                                        WHEN Mdp_DayCode =  'COMP' THEN 'COMPHOL'
                                    ELSE  Mdp_DayCode END AS [DayCode]
                                , CASE Mdp_RestDayFlag 
                                        WHEN 1 THEN 
                                          CASE WHEN Mdp_DayCode = 'REST' THEN Mdy_DayName
                                          ELSE Mdy_DayName + ' FALLING ON A RESTDAY' END
                                        ELSE Mdy_DayName END AS [DayName]
                                FROM M_DayPremium
                                LEFT JOIN M_Day ON Mdp_DayCode = Mdy_DayCode
                                    AND Mdp_CompanyCode = Mdy_CompanyCode
                                    AND Mdy_RecordStatus = 'A'
                                LEFT JOIN M_MiscellaneousDay ON Mmd_DayCode = Mdp_DayCode
                                    AND Mmd_RestDayFlag = Mdp_RestDayFlag
                                    AND Mmd_CompanyCode = Mdp_CompanyCode
                                    AND Mmd_RecordStatus = 'A'
							    WHERE Mdp_PayrollType = '{0}' 
							        AND Mdp_RecordStatus = 'A'
                                    AND Mdp_CompanyCode = '{1}'
                                    AND Mdp_PremiumGrpCode = '{2}'
							    ORDER BY Mdp_SequenceOfDisplay"
                        , PayrollType
                        , CompanyCode
                        , PremiumGroup);
            #endregion

            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        public DataTable GetDayCodeREGABS(string Type, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"
                            SELECT CASE WHEN Mra_Seq = 0
                            THEN Mra_Subseq ELSE Mra_Seq END AS Seq 
                            , Mra_Code AS DayCode
                            , CASE WHEN Mra_DayCode = 'OTHHOL' 
								AND (STUFF((SELECT {2}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
											FROM M_MiscellaneousDay
											INNER JOIN M_Day ON Mdy_DayCode =  Mmd_DayCode
											AND Mdy_CompanyCode = Mmd_CompanyCode
											AND Mdy_HolidayFlag = 1
											WHERE Mmd_CompanyCode = '{1}'
											FOR XML PATH('')))),1,1,'')) <> '' 
								THEN UPPER(REPLACE(Mra_Description
											, 'OTHER HOLIDAY' 
											, (STUFF((SELECT {2}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
												FROM M_MiscellaneousDay
												INNER JOIN M_Day ON Mdy_DayCode =  Mmd_DayCode
												AND Mdy_CompanyCode = Mmd_CompanyCode
												AND Mdy_HolidayFlag = 1
												WHERE Mmd_CompanyCode = '{1}'
												FOR XML PATH('')))),1,1,''))))
								ELSE Mra_Description END AS [DayName]
                            FROM M_RegAbs
                            WHERE Mra_Type = '{0}'
                            AND Mra_RecordStatus = 'A'
                            AND Mra_CompanyCode = '{1}'
                            ORDER BY Seq", Type, CompanyCode, CentralProfile);
            #endregion

            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDayCodeREGABS(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            #region query
            string query = string.Format(@"
                            SELECT CASE WHEN Mra_Seq = 0
                            THEN Mra_Subseq ELSE Mra_Seq END AS Seq 
                            , Mra_Code AS DayCode
                            , CASE WHEN Mra_DayCode = 'OTHHOL' 
								AND (STUFF((SELECT {0}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
											FROM {0}..M_MiscellaneousDay
											INNER JOIN {0}..M_Day ON Mdy_DayCode =  Mmd_DayCode
											    AND Mdy_CompanyCode = Mmd_CompanyCode
											    AND Mdy_HolidayFlag = 1
											WHERE Mmd_CompanyCode = '{1}'
											FOR XML PATH('')))),1,1,'')) <> '' 
								THEN UPPER(REPLACE(Mra_Description
											, 'OTHER HOLIDAY' 
											, (STUFF((SELECT {0}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
												FROM {0}..M_MiscellaneousDay
												INNER JOIN {0}..M_Day ON Mdy_DayCode =  Mmd_DayCode
												    AND Mdy_CompanyCode = Mmd_CompanyCode
												    AND Mdy_HolidayFlag = 1
												WHERE Mmd_CompanyCode = '{1}'
												FOR XML PATH('')))),1,1,''))))
								ELSE Mra_Description END AS [DayName]
                            , Mra_Type AS [DayType]
                            FROM {0}..M_RegAbs
                            WHERE Mra_RecordStatus = 'A'
                                AND Mra_CompanyCode = '{1}'
                            ORDER BY Seq", CentralProfile, CompanyCode);
            #endregion

            DataTable dtResult = null;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public string GetSpecialRateRegAbsCode(string Type, string CompanyCode, string CentralProfile, DALHelper dal)
        {

            #region Query
            string query = string.Format(@"SELECT STUFF( (SELECT ', ' + Mra_Code FROM {0}..M_RegAbs
                                                            INNER JOIN (SELECT Mpd_SubCode, Mpd_SubName 
			                                                            FROM M_PolicyDtl
			                                                            WHERE Mpd_PolicyCode = 'PAYCODERATE'
				                                                            AND Mpd_CompanyCode = '{1}') Setup
			                                                            ON Mra_DayCode = Setup.Mpd_SubCode
                                                            WHERE Mra_CompanyCode = '{1}'
	                                                            AND Mra_Type = '{2}'
	                                                            FOR XML PATH('')), 1, 2, '')
                                            ", CentralProfile, CompanyCode, Type);
            #endregion

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else
                return "";
        }


        public string GetSpecialRateOvertimeCode(string PayrollType, string PremiumGroup, string CompanyCode, string CentralProfile, string DBCollation, DALHelper dal)
        {

            #region Query
            string query = string.Format(@"IF OBJECT_ID('tempdb..#Overtime') IS NOT NULL     
									            DROP TABLE #Overtime
									
									        CREATE TABLE #Overtime ([Seq] TINYINT, [DayCode] VARCHAR(50) COLLATE {4})

									        INSERT INTO #Overtime
									        SELECT Mdp_SequenceOfDisplay  as [Seq]
							                , CASE WHEN Mdp_RestDayFlag = 1 AND Mdp_DayCode = 'REST' THEN ''
										        WHEN Mdp_RestDayFlag = 1 AND Mdp_DayCode <> 'REST' THEN 'REST'
										        ELSE '' END +   
										        CASE 
											        WHEN Mdp_DayCode = 'HOL' THEN 'LEGHOL'
											        WHEN Mdp_DayCode =  'SPL' THEN 'SPLHOL'
											        WHEN Mdp_DayCode =  'COMP' THEN 'COMPHOL'
										        ELSE  
											        Mdp_DayCode 
										        END AS [DayCode]                                       
									           FROM {0}..M_DayPremium
                                               LEFT JOIN {0}..M_Day ON Mdp_DayCode = Mdy_DayCode
                                                    AND Mdp_CompanyCode = Mdy_CompanyCode
                                                    AND Mdy_RecordStatus = 'A'
                                               LEFT JOIN {0}..M_MiscellaneousDay
                                                    ON Mmd_DayCode = Mdp_DayCode
                                                    AND Mmd_RestDayFlag = Mdp_RestDayFlag
                                                    AND Mmd_CompanyCode = Mdp_CompanyCode
                                                    AND Mmd_RecordStatus = 'A'
										        WHERE Mdp_PayrollType = '{2}' 
										            AND Mdp_RecordStatus = 'A'
										            AND Mdp_CompanyCode = '{1}'
										            AND Mdp_PremiumGrpCode = '{3}'
										            AND Mdp_DayCode <> 'REG'
							                    ORDER BY Mdp_PayrollType, Mdp_SequenceOfDisplay

							                    SELECT STUFF( (SELECT ', ' + [DayCode] 
							                    FROM #Overtime
							                    INNER JOIN (SELECT Mpd_SubCode, Mpd_SubName 
			                                                FROM M_PolicyDtl
			                                                WHERE Mpd_PolicyCode = 'PAYCODERATE'
				                                                  AND Mpd_CompanyCode = '{1}') Setup
			                                                ON [DayCode] = Setup.Mpd_SubCode
												FOR XML PATH('')), 1, 2, '')
                                            ", CentralProfile, CompanyCode, PayrollType, PremiumGroup, DBCollation);
            #endregion

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else
                return "";
        }

        public DataTable GetDayCodeOtherTaxable()
        {
            #region query
            string query = @"SELECT 
				            1 [Seq], 'SRGAdj' [DayCode],  'S_RGADJ' [DayName] UNION SELECT
				            2 [Seq], 'SOTAdj' [DayCode],  'S_OTADJ' [DayName] UNION SELECT
				            3 [Seq], 'SHOLAdj' [DayCode],  'S_HLADJ' [DayName] UNION SELECT
				            4 [Seq], 'SNDAdj' [DayCode],  'S_NDADJ' [DayName] UNION SELECT
                            5 [Seq], 'SLVAdj' [DayCode],  'S_LVADJ' [DayName] UNION SELECT
				            6 [Seq], 'MRGAdj' [DayCode],  'M_RGADJ' [DayName] UNION SELECT
				            7 [Seq], 'MOTAdj' [DayCode],  'M_OTADJ' [DayName] UNION SELECT
                            8 [Seq], 'MHOLAdj' [DayCode],  'M_HLADJ' [DayName] UNION SELECT
                            9 [Seq], 'MNDAdj' [DayCode],  'M_NDADJ' [DayName] 
				            ORDER BY [Seq]";
            #endregion

            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetAdjustedPayCycle(string IDNumber, string Date, DALHelper dal)
        {
            DataTable dt = new DataTable();
            string query = string.Format(@"
                            DECLARE @id VARCHAR(15)='{0}'
                            DECLARE @date DATETIME='{1}'

                            CREATE TABLE #adj
                            (PAYCYCLE CHAR(7), ORIGPAYCYCLE CHAR(7))

                            INSERT INTO #adj
                            SELECT DISTINCT Tsa_AdjPayCycle, Tsa_OrigAdjPayCycle
                            FROM T_EmpSystemAdj
                            WHERE Tsa_IDNo = @id
	                            AND Tsa_Date = @date
	                            AND Tsa_PostFlag=1
                            UNION ALL
                            SELECT DISTINCT Tsa_AdjPayCycle, Tsa_OrigAdjPayCycle
                            FROM T_EmpSystemAdjHst
                            WHERE Tsa_IDNo = @id
	                            AND Tsa_Date = @date
	                            AND Tsa_PostFlag=1
	
                            SELECT  ISNULL(
                            (SELECT TOP 1 a.PAYCYCLE FROM #adj a 
                            WHERE b.PAYCYCLE < a.PAYCYCLE 
                            ORDER BY a.PAYCYCLE DESC ),'Hst')  AS  [LeftAdjPayCyCle] 
                            ,b.PAYCYCLE as [RightAdjPayCyCle]
                            ,b.ORIGPAYCYCLE as [Orig Pay Cycle]
                            FROM #adj b

                            DROP TABLE #adj", IDNumber, Date);

            dt = dal.ExecuteDataSet(query).Tables[0];
            return dt;
        }

        public string GetOTHHOLDescription(string Type, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            #region Query
            string query = string.Format(@"							
                                SELECT CASE WHEN (STUFF((SELECT {2}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
											FROM {2}..M_MiscellaneousDay
											INNER JOIN {2}..M_Day ON Mdy_DayCode =  Mmd_DayCode
											AND Mdy_CompanyCode = Mmd_CompanyCode
											AND Mdy_HolidayFlag = 1
											WHERE Mmd_CompanyCode = '{0}'
											FOR XML PATH('')))),1,1,'')) <> '' 
								THEN UPPER(REPLACE(Mra_Description
											, 'OTHER HOLIDAY' 
											, (STUFF((SELECT {2}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
												FROM {2}..M_MiscellaneousDay
												INNER JOIN {2}..M_Day ON Mdy_DayCode =  Mmd_DayCode
												AND Mdy_CompanyCode = Mmd_CompanyCode
												AND Mdy_HolidayFlag = 1
												WHERE Mmd_CompanyCode = '{0}'
												FOR XML PATH('')))),1,1,''))))
								ELSE Mra_Description END AS [DayName]
                            FROM {2}..M_RegAbs
                            WHERE Mra_Type = '{1}'
                            AND Mra_RecordStatus = 'A'
                            AND Mra_CompanyCode = '{0}'
							AND Mra_DayCode = 'OTHHOL'", CompanyCode, Type, CentralProfile);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetOTHHOLDescription(string Type, string CompanyCode, string CentralProfile)
        {
            #region Query
            string query = string.Format(@"							
                                SELECT CASE WHEN (STUFF((SELECT {2}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
											FROM {2}..M_MiscellaneousDay
											INNER JOIN {2}..M_Day ON Mdy_DayCode =  Mmd_DayCode
											AND Mdy_CompanyCode = Mmd_CompanyCode
											AND Mdy_HolidayFlag = 1
											WHERE Mmd_CompanyCode = '{0}'
											FOR XML PATH('')))),1,1,'')) <> '' 
								THEN UPPER(REPLACE(Mra_Description
											, 'OTHER HOLIDAY' 
											, (STUFF((SELECT {2}.dbo.Udf_GetTitleCase((SELECT DISTINCT ',' +  Mdy_DayName as 'data()' 		
												FROM {2}..M_MiscellaneousDay
												INNER JOIN {2}..M_Day ON Mdy_DayCode =  Mmd_DayCode
												AND Mdy_CompanyCode = Mmd_CompanyCode
												AND Mdy_HolidayFlag = 1
												WHERE Mmd_CompanyCode = '{0}'
												FOR XML PATH('')))),1,1,''))))
								ELSE Mra_Description END AS [DayName]
                            FROM {2}..M_RegAbs
                            WHERE Mra_Type = '{1}'
                            AND Mra_RecordStatus = 'A'
                            AND Mra_CompanyCode = '{0}'
							AND Mra_DayCode = 'OTHHOL'", CompanyCode, Type, CentralProfile);
            #endregion
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetReasonForAdjustment(string IDNumber, string Date, string AdjPayCycle, bool bTrail)
        {
            string tableNameLeft = "T_EmpTimeRegisterHst";  
            string condition = string.Empty;
            if (bTrail)
            {
                tableNameLeft = "T_EmpTimeRegisterTrl";
                condition = string.Format("and hst.Ttr_AdjPayCycle = '{0}'", AdjPayCycle);
            }
                

            DataTable dt = new DataTable();
            string query = string.Format(@"DECLARE @id varchar(15)='{0}'
                                           DECLARE @date datetime='{1}'

                                            SELECT CASE WHEN hst.Ttr_DayCode <> trl.Ttr_DayCode then 'Day Code, ' else '' end + 
                                            CASE WHEN hst.Ttr_ShiftCode <> trl.Ttr_ShiftCode  then 'Shift Code, ' else '' end + 
                                            CASE WHEN hst.Ttr_HolidayFlag <> trl.Ttr_HolidayFlag  then 'Holiday, ' else '' end + 
                                            CASE WHEN hst.Ttr_RestDayFlag <> trl.Ttr_RestDayFlag  then 'Rest Day, ' else '' end + 
                                            CASE WHEN hst.Ttr_ActIn_1 <> trl.Ttr_ActIn_1  then 'In 1, ' else '' end + 
                                            CASE WHEN hst.Ttr_ActOut_1 <> trl.Ttr_ActOut_1  then 'Out 1, ' else '' end + 
                                            CASE WHEN hst.Ttr_ActIn_2 <> trl.Ttr_ActIn_2  then 'In 2, ' else '' end + 
                                            CASE WHEN hst.Ttr_ActOut_2 <> trl.Ttr_ActOut_2  then 'Out 2, ' else '' end + 
                                            CASE WHEN ISNULL( hst.Ttr_WFPayLVCode,'') <> ISNULL(trl.Ttr_WFPayLVCode,'')  then 'Applied Paid Leave Type, ' else '' end + 
                                            CASE WHEN hst.Ttr_WFPayLVHr <> trl.Ttr_WFPayLVHr  then 'Applied Paid Leave Hours, ' else '' end + 
                                            CASE WHEN ISNULL(hst.Ttr_WFNoPayLVCode,'') <> ISNULL(trl.Ttr_WFNoPayLVCode,'')  then 'Applied Unpaid Leave Type, ' else '' end + 
                                            CASE WHEN hst.Ttr_WFNoPayLVHr <> trl.Ttr_WFNoPayLVHr  then 'Applied Unpaid Leave Hours, '  else '' end + 
                                            CASE WHEN hst.Ttr_WFOTAdvHr <> trl.Ttr_WFOTAdvHr  then 'Applied Early Overtime Hours, ' else '' end + 
                                            CASE WHEN hst.Ttr_WFOTPostHr <> trl.Ttr_WFOTPostHr  then 'Applied Mid/Late Overtime Hours, ' else '' end + 
                                            CASE WHEN hst.Ttr_AssumedFlag <> trl.Ttr_AssumedFlag  then 'Tag as Present, ' else '' end + 
                                            CASE WHEN ISNULL(hst.Ttr_Amnesty,'') <> ISNULL(trl.Ttr_Amnesty,'')  then 'Waived Late/Undertime, ' else '' end + 
                                            CASE WHEN ISNULL(hst.Ttr_AssumedPost,'') <> ISNULL(trl.Ttr_AssumedPost,'')  then 'Assume Day, ' else '' end as Remarks
                                            FROM {2} hst -- left
                                            LEFT JOIN T_EmpTimeRegisterTrl trl ON trl.Ttr_AdjPayCycle = '{3}' --right
	                                            AND trl.ttr_idno = hst.Ttr_IDNo
	                                            AND trl.ttr_date = hst.Ttr_Date
                                            WHERE hst.Ttr_IDNo = @id 
	                                            AND hst.Ttr_Date = @date
	                                            {4}
                                                ", IDNumber, Date, tableNameLeft, AdjPayCycle, condition);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dt.Rows[0][0].ToString();
        }

        public string GetReasonForAdjustmentMisc(string IDNumber, string Date, string AdjPayCycle, bool bTrail)
        {
            string tableNameLeft = "T_EmpTimeRegisterHst";
            string tableNameLeftMisc = "T_EmpTimeRegisterMiscHst";
            string condition = string.Empty;
            if (bTrail)
            {
                tableNameLeft = "T_EmpTimeRegisterTrl";
                tableNameLeftMisc = "T_EmpTimeRegisterMiscTrl";
                condition = string.Format("and hst.Ttr_AdjPayCycle = '{0}'", AdjPayCycle);
            }


            DataTable dt = new DataTable();
            string query = string.Format(@"
                                           DECLARE @date datetime='{1}'

                                            SELECT CASE WHEN hst.Ttr_DayCode <> trl.Ttr_DayCode then 'Day Code, ' else '' end + 
                                            CASE WHEN hst.Ttr_ShiftCode <> trl.Ttr_ShiftCode  then 'Shift Code, ' else '' end + 
                                            CASE WHEN hst.Ttr_HolidayFlag <> trl.Ttr_HolidayFlag  then 'Holiday, ' else '' end + 
                                            CASE WHEN hst.Ttr_RestDayFlag <> trl.Ttr_RestDayFlag  then 'Rest Day, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActIn_01 <> MiscTrl.Ttm_ActIn_01  then 'In 1, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_01 <> MiscTrl.Ttm_ActOut_01  then 'Out 1, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActIn_02 <> MiscTrl.Ttm_ActIn_02  then 'In 2, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_02 <> MiscTrl.Ttm_ActOut_02  then 'Out 2, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_03 <> MiscTrl.Ttm_ActIn_03  then 'In 3, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_03 <> MiscTrl.Ttm_ActOut_03  then 'Out 3, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_04 <> MiscTrl.Ttm_ActIn_04  then 'In 4, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_04 <> MiscTrl.Ttm_ActOut_04  then 'Out 4, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_05 <> MiscTrl.Ttm_ActIn_05  then 'In 5, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_05 <> MiscTrl.Ttm_ActOut_05  then 'Out 5, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_06 <> MiscTrl.Ttm_ActIn_06  then 'In 6, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_06 <> MiscTrl.Ttm_ActOut_06  then 'Out 6, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_07 <> MiscTrl.Ttm_ActIn_07  then 'In 7, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_07 <> MiscTrl.Ttm_ActOut_07  then 'Out 7, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_08 <> MiscTrl.Ttm_ActIn_08  then 'In 8, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_08 <> MiscTrl.Ttm_ActOut_08  then 'Out 8, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_09 <> MiscTrl.Ttm_ActIn_09  then 'In 9, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_09 <> MiscTrl.Ttm_ActOut_09  then 'Out 9, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_10 <> MiscTrl.Ttm_ActIn_10  then 'In 10, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_10 <> MiscTrl.Ttm_ActOut_10  then 'Out 10, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_11 <> MiscTrl.Ttm_ActIn_11  then 'In 11, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_11 <> MiscTrl.Ttm_ActOut_11  then 'Out 11, ' else '' end + 
											CASE WHEN Misc.Ttm_ActIn_12 <> MiscTrl.Ttm_ActIn_12  then 'In 12, ' else '' end + 
                                            CASE WHEN Misc.Ttm_ActOut_12 <> MiscTrl.Ttm_ActOut_12  then 'Out 12, ' else '' end + 
                                            CASE WHEN ISNULL( hst.Ttr_WFPayLVCode,'') <> ISNULL(trl.Ttr_WFPayLVCode,'')  then 'Applied Paid Leave Type, ' else '' end + 
                                            CASE WHEN hst.Ttr_WFPayLVHr <> trl.Ttr_WFPayLVHr  then 'Applied Paid Leave Hours, ' else '' end + 
                                            CASE WHEN ISNULL(hst.Ttr_WFNoPayLVCode,'') <> ISNULL(trl.Ttr_WFNoPayLVCode,'')  then 'Applied Unpaid Leave Type, ' else '' end + 
                                            CASE WHEN hst.Ttr_WFNoPayLVHr <> trl.Ttr_WFNoPayLVHr  then 'Applied Unpaid Leave Hours, '  else '' end + 
                                            CASE WHEN hst.Ttr_WFOTAdvHr <> trl.Ttr_WFOTAdvHr  then 'Applied Early Overtime Hours, ' else '' end + 
                                            CASE WHEN hst.Ttr_WFOTPostHr <> trl.Ttr_WFOTPostHr  then 'Applied Mid/Late Overtime Hours, ' else '' end + 
                                            CASE WHEN hst.Ttr_AssumedFlag <> trl.Ttr_AssumedFlag  then 'Tag as Present, ' else '' end + 
                                            CASE WHEN ISNULL(hst.Ttr_Amnesty,'') <> ISNULL(trl.Ttr_Amnesty,'')  then 'Waived Late/Undertime, ' else '' end + 
                                            CASE WHEN ISNULL(hst.Ttr_AssumedPost,'') <> ISNULL(trl.Ttr_AssumedPost,'')  then 'Assume Day, ' else '' end as Remarks
                                            FROM {2} hst -- left
                                            LEFT JOIN {5} Misc ON hst.Ttr_IDNo = Misc.Ttm_IDNo
												AND hst.Ttr_Date = Misc.Ttm_Date
                                            LEFT JOIN T_EmpTimeRegisterTrl trl ON trl.Ttr_AdjPayCycle = '{3}' --right
	                                            AND trl.ttr_idno = hst.Ttr_IDNo
	                                            AND trl.ttr_date = hst.Ttr_Date
                                            LEFT JOIN T_EmpTimeRegisterMiscTrl MiscTrl ON MiscTrl.Ttm_AdjPayCycle = '{3}' --right
												AND MiscTrl.Ttm_IDNo = hst.Ttr_IDNo
												AND MiscTrl.Ttm_date = hst.Ttr_Date
                                            WHERE hst.Ttr_IDNo = '{0}'
	                                            AND hst.Ttr_Date = @date
	                                            {4}
                                                ", IDNumber, Date, tableNameLeft, AdjPayCycle, condition, tableNameLeftMisc);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dt.Rows[0][0].ToString();
        }

        public DataTable GetPayrollTransactionDetailRecord(string IDNumber, string Date, string AdjPayCycle, bool bTrail, DALHelper dal)
        {
            string tableNameLeft = "T_EmpPayTranDtlHst";
            string tableNameMiscLeft = "T_EmpPayTranDtlMiscHst";
            string condition = string.Empty;
            string joincondition = string.Empty;
            if (bTrail)
            {
                tableNameLeft = "T_EmpPayTranDtlTrl";
                tableNameMiscLeft = "T_EmpPayTranDtlMiscTrl";
                condition = string.Format("and CALC.Tpd_AdjPayCycle = '{0}'", AdjPayCycle);
                joincondition = "AND CALC.Tpd_AdjPayCycle = EXT.Tpd_AdjPayCycle";
            }

            DataTable dt = new DataTable();
            #region Query
            string query = string.Format(@"
                                   SELECT CALC.Tpd_IDNo
									  ,CALC.Tpd_PayCycle
									  ,CALC.Tpd_Date
                                      ,CALC.Tpd_PayrollType
                                      ,Tpd_ABSHr
                                      ,Tpd_REGHr
                                      ,Tpd_REGOTHr
                                      ,Tpd_REGNDHr
                                      ,Tpd_REGNDOTHr
                                      ,Tpd_RESTHr
                                      ,Tpd_RESTOTHr
                                      ,Tpd_RESTNDHr
                                      ,Tpd_RESTNDOTHr
                                      ,Tpd_LEGHOLHr
                                      ,Tpd_LEGHOLOTHr
                                      ,Tpd_LEGHOLNDHr
                                      ,Tpd_LEGHOLNDOTHr
                                      ,Tpd_SPLHOLHr
                                      ,Tpd_SPLHOLOTHr
                                      ,Tpd_SPLHOLNDHr
                                      ,Tpd_SPLHOLNDOTHr
                                      ,Tpd_PSDHr
                                      ,Tpd_PSDOTHr
                                      ,Tpd_PSDNDHr
                                      ,Tpd_PSDNDOTHr
                                      ,Tpd_COMPHOLHr
                                      ,Tpd_COMPHOLOTHr
                                      ,Tpd_COMPHOLNDHr
                                      ,Tpd_COMPHOLNDOTHr
                                      ,Tpd_RESTLEGHOLHr
                                      ,Tpd_RESTLEGHOLOTHr
                                      ,Tpd_RESTLEGHOLNDHr
                                      ,Tpd_RESTLEGHOLNDOTHr
                                      ,Tpd_RESTSPLHOLHr
                                      ,Tpd_RESTSPLHOLOTHr
                                      ,Tpd_RESTSPLHOLNDHr
                                      ,Tpd_RESTSPLHOLNDOTHr
                                      ,Tpd_RESTCOMPHOLHr
                                      ,Tpd_RESTCOMPHOLOTHr
                                      ,Tpd_RESTCOMPHOLNDHr
                                      ,Tpd_RESTCOMPHOLNDOTHr
                                      ,Tpd_RESTPSDHr
                                      ,Tpd_RESTPSDOTHr
                                      ,Tpd_RESTPSDNDHr
                                      ,Tpd_RESTPSDNDOTHr
                                      ,Tpd_PDRESTLEGHOLHr
									  ,Tpd_WorkDay
                                      ,Tpd_LTHr
                                      ,Tpd_UTHr
                                      ,Tpd_UPLVHr
                                      ,Tpd_ABSLEGHOLHr
                                      ,Tpd_ABSSPLHOLHr
                                      ,Tpd_ABSCOMPHOLHr
                                      ,Tpd_ABSPSDHr
                                      ,Tpd_ABSOTHHOLHr
                                      ,Tpd_WDABSHr
                                      ,Tpd_LTUTMaxHr
                                      ,Tpd_PDLVHr
                                      ,Tpd_PDLEGHOLHr
                                      ,Tpd_PDSPLHOLHr
                                      ,Tpd_PDCOMPHOLHr
                                      ,Tpd_PDOTHHOLHr
                                      ,ISNULL(Tpd_Misc1Hr, 0) AS Tpd_Misc1Hr
                                      ,ISNULL(Tpd_Misc1OTHr, 0) AS Tpd_Misc1OTHr
                                      ,ISNULL(Tpd_Misc1NDHr, 0) AS Tpd_Misc1NDHr
                                      ,ISNULL(Tpd_Misc1NDOTHr, 0) AS Tpd_Misc1NDOTHr
                                      ,ISNULL(Tpd_Misc2Hr, 0) AS Tpd_Misc2Hr
                                      ,ISNULL(Tpd_Misc2OTHr, 0) AS Tpd_Misc2OTHr
                                      ,ISNULL(Tpd_Misc2NDHr, 0) AS Tpd_Misc2NDHr
                                      ,ISNULL(Tpd_Misc2NDOTHr, 0) AS Tpd_Misc2NDOTHr
                                      ,ISNULL(Tpd_Misc3Hr, 0) AS Tpd_Misc3Hr
                                      ,ISNULL(Tpd_Misc3OTHr, 0) AS Tpd_Misc3OTHr
                                      ,ISNULL(Tpd_Misc3NDHr, 0) AS Tpd_Misc3NDHr
                                      ,ISNULL(Tpd_Misc3NDOTHr, 0) AS Tpd_Misc3NDOTHr
                                      ,ISNULL(Tpd_Misc4Hr, 0) AS Tpd_Misc4Hr
                                      ,ISNULL(Tpd_Misc4OTHr, 0) AS Tpd_Misc4OTHr
                                      ,ISNULL(Tpd_Misc4NDHr, 0) AS Tpd_Misc4NDHr
                                      ,ISNULL(Tpd_Misc4NDOTHr, 0) AS Tpd_Misc4NDOTHr
                                      ,ISNULL(Tpd_Misc5Hr, 0) AS Tpd_Misc5Hr
                                      ,ISNULL(Tpd_Misc5OTHr, 0) AS Tpd_Misc5OTHr
                                      ,ISNULL(Tpd_Misc5NDHr, 0) AS Tpd_Misc5NDHr
                                      ,ISNULL(Tpd_Misc5NDOTHr, 0) AS Tpd_Misc5NDOTHr
                                      ,ISNULL(Tpd_Misc6Hr, 0) AS Tpd_Misc6Hr
                                      ,ISNULL(Tpd_Misc6OTHr, 0) AS Tpd_Misc6OTHr
                                      ,ISNULL(Tpd_Misc6NDHr, 0) AS Tpd_Misc6NDHr
                                      ,ISNULL(Tpd_Misc6NDOTHr, 0) AS Tpd_Misc6NDOTHr
                                      ,Tpd_PremiumGrpCode
                                FROM {2} CALC
                                LEFT JOIN {3} EXT 
								ON CALC.Tpd_IDNo = EXT.Tpd_IDNo
                                AND CALC.Tpd_PayCycle = EXT.Tpd_PayCycle
								AND CALC.Tpd_Date = EXT.Tpd_Date
                                {4}
								WHERE CALC.Tpd_IDNo = '{0}' 
								AND CALC.Tpd_Date = '{1}'
                                {5}
                                ", IDNumber, Date, tableNameLeft, tableNameMiscLeft, joincondition, condition);

            #endregion
            dt = dal.ExecuteDataSet(query).Tables[0];
            return dt;
        }

        public DataTable GetOvertimeNightPremium(DataTable dtResult, string CompanyCode, string CentralProfile)
        {
            decimal fHR = 0, fAmt = 0, fOTHR = 0, fOTAmt = 0, fNDHR = 0, fNDAmt = 0, fNDOTHR = 0, fNDOTAmt = 0, fTotal = 0;
            DataTable dtTaxInc = new DataTable();
            dtTaxInc.Columns.Add("Item", typeof(string));
            dtTaxInc.Columns.Add("Hour", typeof(decimal));
            dtTaxInc.Columns.Add("Amount", typeof(decimal));
            dtTaxInc.Columns.Add("OT Hour", typeof(decimal));
            dtTaxInc.Columns.Add("OT Amount", typeof(decimal));
            dtTaxInc.Columns.Add("ND Hour", typeof(decimal));
            dtTaxInc.Columns.Add("ND Amount", typeof(decimal));
            dtTaxInc.Columns.Add("ND OT Hour", typeof(decimal));
            dtTaxInc.Columns.Add("ND OT Amount", typeof(decimal));
            dtTaxInc.Columns.Add("Total Amount", typeof(decimal));

            DataRow dr = dtTaxInc.NewRow();
            #region 3.OVERTIME & NIGHT PREMIUM
            #region 3.1.OT & NPREM Header
            dr = dtTaxInc.NewRow();
            dr["Item"]                  = "   OVERTIME AND NIGHT PREMIUM";
            dr["Hour"]                  = DBNull.Value;
            dr["Amount"]                = DBNull.Value;
            dr["OT Hour"]               = DBNull.Value;
            dr["OT Amount"]             = DBNull.Value;
            dr["ND Hour"]               = DBNull.Value;
            dr["ND Amount"]             = DBNull.Value;
            dr["ND OT Hour"]            = DBNull.Value;
            dr["ND OT Amount"]          = DBNull.Value;
            dr["Total Amount"]          = DBNull.Value;
            dtTaxInc.Rows.Add(dr);
            #endregion

            #region 3.2.OT & NPREM Details
            DataTable dtDayCodesCol = GetDayCodeOT(dtResult.Rows[0]["Tpy_PayrollType"].ToString(), dtResult.Rows[0]["Tpy_PremiumGrpCode"].ToString(), CompanyCode, CentralProfile);
            for (int idx = 0; idx < dtDayCodesCol.Rows.Count; idx++)
            {
                string colPrefix = "Tpy_" + dtDayCodesCol.Rows[idx]["DayCode"].ToString().Trim();
                if (Convert.ToInt32(dtDayCodesCol.Rows[idx]["DayID"].ToString().Trim()) > 0)
                    colPrefix = "Tpm_Misc" + dtDayCodesCol.Rows[idx]["DayID"].ToString().Trim();

                dr = dtTaxInc.NewRow();
                dr["Item"]              = "       " + dtDayCodesCol.Rows[idx]["DayName"].ToString().Trim();
                if (idx == 0)
                {
                    dr["Hour"]          = DBNull.Value;
                    dr["Amount"]        = DBNull.Value;
                }
                else
                {
                    dr["Hour"]          = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Hr"]);
                    dr["Amount"]        = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Amt"]);
                    dr["Total Amount"]  = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Amt"]);

                    fHR                 += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Hr"]);
                    fAmt                += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Amt"]);
                }
                dr["OT Hour"]           = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"]);
                dr["OT Amount"]         = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTAmt"]);
                dr["ND Hour"]           = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"]);
                dr["ND Amount"]         = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDAmt"]);
                dr["ND OT Hour"]        = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"]);
                dr["ND OT Amount"]      = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTAmt"]);
                if (idx == 0)
                {
                    dr["Total Amount"]  = Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTAmt"])
                                            + Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDAmt"])
                                            + Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTAmt"]);
                }
                else
                {
                    dr["Total Amount"]  = Convert.ToDecimal(dr["Total Amount"])
                                            + Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTAmt"])
                                            + Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDAmt"])
                                            + Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTAmt"]);
                }
                dtTaxInc.Rows.Add(dr);

                fOTHR                   += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"]);
                fOTAmt                  += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTAmt"]);
                fNDHR                   += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"]);
                fNDAmt                  += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDAmt"]);
                fNDOTHR                 += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"]);
                fNDOTAmt                += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTAmt"]);
                fTotal                  += Convert.ToDecimal(dr["Total Amount"]);


            }
            #endregion

            #region //3.3.FILLER 1-6
                //DataTable dtDayCodeMisc = GetDayCodeOT(true, dtResult.Rows[0]["Tpy_PayrollType"].ToString(), dtResult.Rows[0]["Tpy_PremiumGrpCode"].ToString(), CompanyCode, CentralProfile);
                //for (int idxx = 0; idxx < dtDayCodeMisc.Rows.Count; idxx++)
                //{
                //    dr = dtTaxInc.NewRow();
                //    dr["Seq"]           = Convert.ToInt32(dtDayCodeMisc.Rows[idxx]["Seq"].ToString().Trim());
                //    dr["Item"]          = "       " + dtDayCodeMisc.Rows[idxx]["DayName"];
                //    dr["Hour"]          = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "Hr"]);
                //    dr["Amount"]        = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "Amt"]);
                //    dr["OT Hour"]       = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "OTHr"]);
                //    dr["OT Amount"]     = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "OTAmt"]);
                //    dr["ND Hour"]       = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDHr"]);
                //    dr["ND Amount"]     = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDAmt"]);
                //    dr["ND OT Hour"]    = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDOTHr"]);
                //    dr["ND OT Amount"]  = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDOTAmt"]);
                //    dr["Total Amount"]  = Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "Amt"])
                //                            + Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "OTAmt"])
                //                            + Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDAmt"])
                //                            + Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDOTAmt"]);
                //    dtTaxInc.Rows.Add(dr);
                //    fHR                 += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "Hr"]);
                //    fAmt                += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "Amt"]);
                //    fOTHR               += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "OTHr"]);
                //    fOTAmt              += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "OTAmt"]);
                //    fNDHR               += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDHr"]);
                //    fNDAmt              += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDAmt"]);
                //    fNDOTHR             += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDOTHr"]);
                //    fNDOTAmt            += Convert.ToDecimal(dtResult.Rows[0]["Tpm_Misc" + dtDayCodeMisc.Rows[idxx]["DayID"].ToString().Trim() + "NDOTAmt"]);
                //    fTotal              += Convert.ToDecimal(dr["Total Amount"]);
                //}
            #endregion

            #region 3.4.TOTAL OVERTIME AND NIGHT PREMIUM
            dr = dtTaxInc.NewRow();
            dr["Item"]              = "       TOTAL OVERTIME AND NIGHT PREMIUM";
            dr["Hour"]              = fHR;
            dr["Amount"]            = fAmt;
            dr["OT Hour"]           = fOTHR;
            dr["OT Amount"]         = fOTAmt;
            dr["ND Hour"]           = fNDHR;
            dr["ND Amount"]         = fNDAmt;
            dr["ND OT Hour"]        = fNDOTHR;
            dr["ND OT Amount"]      = fNDOTAmt;
            dr["Total Amount"]      = fTotal;
            dtTaxInc.Rows.Add(dr);
            #endregion
            #endregion
            return dtTaxInc;
        }

        public DataTable GetRegAbs(DataTable dtResult, string Type, string CompanyCode, string CentralProfile)
        {
            DataTable dtTaxInc = new DataTable();
            dtTaxInc.Columns.Add("Item", typeof(string));
            dtTaxInc.Columns.Add("Hour", typeof(decimal));
            dtTaxInc.Columns.Add("Amount", typeof(decimal));

            DataRow dr = dtTaxInc.NewRow();
            #region HEADER
            dr["Item"] = "   " + Type;
            dr["Hour"] = DBNull.Value;
            dr["Amount"] = DBNull.Value;
            dtTaxInc.Rows.Add(dr);
            #endregion
            #region DETAILS
            DataTable dtDayCodeREGABS = GetDayCodeREGABS(Type.Substring(0,1), CompanyCode, CentralProfile);
            for (int idx = 0; idx < dtDayCodeREGABS.Rows.Count; idx++)
            {
                dr = dtTaxInc.NewRow();
                dr["Item"] = "       " + dtDayCodeREGABS.Rows[idx]["DayName"].ToString().Trim();
                dr["Hour"] = Convert.ToDecimal(dtResult.Rows[0]["Tpy_" + dtDayCodeREGABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]);
                dr["Amount"] = Convert.ToDecimal(dtResult.Rows[0]["Tpy_" + dtDayCodeREGABS.Rows[idx]["DayCode"].ToString().Trim() + "Amt"]);
                dtTaxInc.Rows.Add(dr);
            }
            #endregion
            #region TOTAL REGULAR
            if (Type == "REGULAR")
            {
                dr = dtTaxInc.NewRow();
                dr["Item"] = "       TOTAL " + Type;
                dr["Hour"] = Convert.ToDecimal(dtResult.Rows[0]["Tpy_TotalREGHr"]);//( != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpy_TotalREGHr"])) : "");
                dr["Amount"] = Convert.ToDecimal(dtResult.Rows[0]["Tpy_TotalREGAmt"]);//( != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpy_TotalREGAmt"])) : "");
                dtTaxInc.Rows.Add(dr);
            }
            #endregion

            return dtTaxInc;
        }

        public DataTable GetRegAbsHr(DataTable dtResult, string Type, string CompanyCode, string CentralProfile)
        {
            decimal fHR = 0;
            DataTable dtTaxInc = new DataTable();
            dtTaxInc.Columns.Add("Item");
            dtTaxInc.Columns.Add("Hour");
            dtTaxInc.Columns.Add("OT Hour");
            dtTaxInc.Columns.Add("ND Hour");
            dtTaxInc.Columns.Add("ND OT Hour");

            DataRow dr = dtTaxInc.NewRow();
            #region HEADER
            dr["Item"] = "   " + Type;
            dr["Hour"] = DBNull.Value;
            dr["OT Hour"] = DBNull.Value;
            dr["ND Hour"] = DBNull.Value;
            dr["ND OT Hour"] = DBNull.Value;
            dtTaxInc.Rows.Add(dr);
            #endregion
            #region DETAILS
            DataTable dtDayCodeREGABS = GetDayCodeREGABS(Type.Substring(0, 1), CompanyCode, CentralProfile);
            for (int idx = 0; idx < dtDayCodeREGABS.Rows.Count; idx++)
            {
                dr = dtTaxInc.NewRow();
                dr["Item"] = "       " + dtDayCodeREGABS.Rows[idx]["DayName"].ToString().Trim();
                dr["Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREGABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREGABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"])) : "");
                dr["OT Hour"] = DBNull.Value;
                dr["ND Hour"] = DBNull.Value;
                dr["ND OT Hour"] = DBNull.Value;
                if (Type == "REGULAR" && Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREGABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]) != 0)
                    fHR += Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREGABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]);
                dtTaxInc.Rows.Add(dr);
            }
            #endregion
            #region TOTAL REGULAR
            if (Type == "REGULAR")
            {
                dr = dtTaxInc.NewRow();
                dr["Item"] = "       TOTAL " + Type;
                dr["Hour"] = (fHR != 0 ? string.Format("{0:n}", fHR) : "");
                dr["OT Hour"] = DBNull.Value;
                dr["ND Hour"] = DBNull.Value;
                dr["ND OT Hour"] = DBNull.Value;
                dtTaxInc.Rows.Add(dr);
            }
            #endregion

            return dtTaxInc;
        }

        public DataTable GetTotalTaxableIncomeHours(DataTable dtResult, bool bABS, bool bREG, bool bOT, string CompanyCode, string CentralProfile)
        {
            try
            {
                DataTable dtTaxInc = new DataTable();
                dtTaxInc.Columns.Add("Item");
                dtTaxInc.Columns.Add("Hour");
                dtTaxInc.Columns.Add("OT Hour");
                dtTaxInc.Columns.Add("ND Hour");
                dtTaxInc.Columns.Add("ND OT Hour");

                DataRow dr = dtTaxInc.NewRow();
                if (bABS)
                {
                    #region 1.ABSENCES
                    #region 1.1.ABSENCES Header
                    dr["Item"] = "   ABSENCES";
                    dr["Hour"] = DBNull.Value;
                    dr["OT Hour"] = DBNull.Value;
                    dr["ND Hour"] = DBNull.Value;
                    dr["ND OT Hour"] = DBNull.Value;
                    dtTaxInc.Rows.Add(dr);
                    #endregion
                    #region 1.2.ABSENCES Details
                    DataTable dtDayCodeABS = GetDayCodeREGABS("A", CompanyCode, CentralProfile);
                    for (int idx = 0; idx < dtDayCodeABS.Rows.Count; idx++)
                    {
                        dr = dtTaxInc.NewRow();
                        dr["Item"] = "       " + dtDayCodeABS.Rows[idx]["DayName"].ToString().Trim();
                        dr["Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeABS.Rows[idx]["DayCode"].ToString().Trim() + "Hr"])) : "");
                        dr["OT Hour"] = DBNull.Value;
                        dr["ND Hour"] = DBNull.Value;
                        dr["ND OT Hour"] = DBNull.Value;
                        dtTaxInc.Rows.Add(dr);
                    }
                    #endregion
                    #endregion
                }

                if (bREG)
                {
                    #region 2.REGULAR
                    decimal fHR = 0;
                    #region 2.1.REGULAR Header
                    dr = dtTaxInc.NewRow();
                    dr["Item"] = "   REGULAR";
                    dr["Hour"] = DBNull.Value;
                    dr["OT Hour"] = DBNull.Value;
                    dr["ND Hour"] = DBNull.Value;
                    dr["ND OT Hour"] = DBNull.Value;
                    dtTaxInc.Rows.Add(dr);
                    #endregion
                    #region 2.2.REGULAR Details
                    DataTable dtDayCodeREG = GetDayCodeREGABS("R", CompanyCode, CentralProfile);
                    for (int idx = 0; idx < dtDayCodeREG.Rows.Count; idx++)
                    {
                        dr = dtTaxInc.NewRow();
                        dr["Item"] = "       " + dtDayCodeREG.Rows[idx]["DayName"].ToString().Trim();
                        dr["Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREG.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREG.Rows[idx]["DayCode"].ToString().Trim() + "Hr"])) : "");
                        if (Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREG.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]) != 0)
                            fHR += Convert.ToDecimal(dtResult.Rows[0]["Tpd_" + dtDayCodeREG.Rows[idx]["DayCode"].ToString().Trim() + "Hr"]);
                        dr["OT Hour"] = DBNull.Value;
                        dr["ND Hour"] = DBNull.Value;
                        dr["ND OT Hour"] = DBNull.Value;
                        dtTaxInc.Rows.Add(dr);
                    }
                    #endregion
                    #region 2.3.TOTAL REGULAR
                    dr = dtTaxInc.NewRow();
                    dr["Item"] = "       TOTAL REGULAR";
                    dr["Hour"] = (fHR != 0 ? string.Format("{0:n}", fHR) : "");
                    dr["OT Hour"] = DBNull.Value;
                    dr["ND Hour"] = DBNull.Value;
                    dr["ND OT Hour"] = DBNull.Value;
                    dtTaxInc.Rows.Add(dr);
                    #endregion
                    #endregion
                }

                if (bOT)
                {
                    #region 3.OVERTIME & NIGHT PREMIUM
                    decimal fRGHr = 0, fOTHr = 0, fNDHr = 0, fNDOTHr = 0;
                    #region 3.1.OT & NPREM Header
                    dr = dtTaxInc.NewRow();
                    dr["Item"]          = "   OVERTIME & NIGHT PREMIUM";
                    dr["Hour"]          = DBNull.Value;
                    dr["OT Hour"]       = DBNull.Value;
                    dr["ND Hour"]       = DBNull.Value;
                    dr["ND OT Hour"]    = DBNull.Value;
                    dtTaxInc.Rows.Add(dr);
                    #endregion
                    #region 3.2.OT & NPREM Details
                    DataTable dtDayCodesCol = GetDayCodeOT(dtResult.Rows[0]["Tpd_PayrollType"].ToString(), dtResult.Rows[0]["Tpd_PremiumGrpCode"].ToString(), CompanyCode, CentralProfile);
                    for (int idx = 0; idx < dtDayCodesCol.Rows.Count; idx++)
                    {
                        string colPrefix = "Tpd_" + dtDayCodesCol.Rows[idx]["DayCode"].ToString().Trim();
                        if (Convert.ToInt32(dtDayCodesCol.Rows[idx]["DayID"].ToString().Trim()) > 0)
                            colPrefix = "Tpd_Misc" + dtDayCodesCol.Rows[idx]["DayID"].ToString().Trim();

                        dr = dtTaxInc.NewRow();
                        dr["Item"] = "       " + dtDayCodesCol.Rows[idx]["DayName"].ToString().Trim();
                        if (idx == 0)
                        {
                            dr["Hour"]          = DBNull.Value;
                            dr["OT Hour"]       = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"])) : "");
                            dr["ND Hour"]       = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"])) : "");
                            dr["ND OT Hour"]    = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"])) : "");

                            fOTHr       += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"]);
                            fNDHr       += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"]);
                            fNDOTHr     += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"]);
                        }
                        else
                        {
                            dr["Hour"]          = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Hr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Hr"])) : "");
                            dr["OT Hour"]       = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"])) : "");
                            dr["ND Hour"]       = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"])) : "");
                            dr["ND OT Hour"]    = (Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"])) : "");

                            fRGHr       += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "Hr"]);
                            fOTHr       += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "OTHr"]);
                            fNDHr       += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDHr"]);
                            fNDOTHr     += Convert.ToDecimal(dtResult.Rows[0][colPrefix + "NDOTHr"]);
                        }
                        dtTaxInc.Rows.Add(dr);
                    }
                    #endregion

                    #region //3.3.FILLER 1-6
                    //DataTable dtDayCodeMisc = GetDayCodeOT(true, dtResult.Rows[0]["Tpd_PayrollType"].ToString(), dtResult.Rows[0]["Tpd_PremiumGrpCode"].ToString(), CompanyCode, CentralProfile);
                    //for (int idxx = 0; idxx < dtDayCodesCol.Rows.Count; idxx++)
                    //{
                    //    dr = dtTaxInc.NewRow();
                    //    dr["Item"] = "       " + dtDayCodesCol.Rows[idxx]["DayName"];
                    //    dr["Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "Hr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "Hr"])) : "");
                    //    dr["OT Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "OTHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "OTHr"])) : "");
                    //    dr["ND Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "NDHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "NDHr"])) : "");
                    //    dr["ND OT Hour"] = (Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "NDOTHr"]) != 0 ? string.Format("{0:n}", Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "NDOTHr"])) : "");

                    //    fRGHr += Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "Hr"]);
                    //    fOTHr += Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "OTHr"]);
                    //    fNDHr += Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "NDHr"]);
                    //    fNDOTHr += Convert.ToDecimal(dtResult.Rows[0]["Tpd_Misc" + dtDayCodesCol.Rows[idxx]["DayID"].ToString().Trim() + "NDOTHr"]);
                    //    dtTaxInc.Rows.Add(dr);
                    //}
                    #endregion

                    #region 3.4.TOTAL OVERTIME & NIGHT PREMIUM
                    dr = dtTaxInc.NewRow();
                    dr["Item"]          = "       TOTAL OVERTIME & NIGHT PREMIUM";
                    dr["Hour"]          = (fRGHr != 0 ? string.Format("{0:n}", fRGHr) : "");
                    dr["OT Hour"]       = (fOTHr != 0 ? string.Format("{0:n}", fOTHr) : "");
                    dr["ND Hour"]       = (fNDHr != 0 ? string.Format("{0:n}", fNDHr) : "");
                    dr["ND OT Hour"]    = (fNDOTHr != 0 ? string.Format("{0:n}", fNDOTHr) : "");
                    dtTaxInc.Rows.Add(dr);
                    #endregion
                    #endregion
                }

                return dtTaxInc;
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public bool checkIfManualHoursEntry(string IDNumber, string PayCycle, bool bCurrentPayCycle, DALHelper dal)
        {
            try
            {
                string tableName = string.Empty;
                if (bCurrentPayCycle)
                    tableName = "T_EmpPayTranHdr";
                else
                    tableName = "T_EmpPayTranHdrHst";

                DataTable dt = new DataTable();
                string query = string.Format(@"SELECT Tph_RetainUserEntry FROM {0}
                                                    WHERE Tph_IDNo = '{1}'
                                                    AND Tph_PayCycle = '{2}'"
                                                , tableName, IDNumber, PayCycle);

                //using (DALHelper dal = new DALHelper())
                //
                    //dal.OpenDB();
                    dt = dal.ExecuteDataSet(query).Tables[0];
                    //dal.CloseDB();
                //}
                return Convert.ToBoolean(dt.Rows[0][0].ToString());
            }
            catch
            {
                return false;
            }
        }

        public bool checkIfSystemOverride(string IDNumber, string PayCycle, bool bCurrentPayCycle, DALHelper dal)
        {
            try
            {
                string tableName = string.Empty;
                string tableName2 = string.Empty;
                if (bCurrentPayCycle)
                {
                    tableName = "T_EmpSystemAdj";
                    tableName2 = "T_EmpManualAdj";
                }
                else
                {
                    tableName = "T_EmpSystemAdjHst";
                    tableName2 = "T_EmpManualAdjHst";
                }
                    

                DataTable dt = new DataTable();
                string query = string.Format(@"SELECT COUNT(*) [Count] FROM {0}
                                                LEFT JOIN {1} 
                                                ON Tsa_AdjPayCycle = Tma_PayCycle
                                                AND Tsa_IDNo = Tma_IDNo
                                                WHERE Tsa_AdjPayCycle = '{2}'
                                                AND Tsa_IDNo = '{3}'
                                                AND Tma_RetainUserEntry = 1"
                                                , tableName, tableName2, PayCycle, IDNumber);

                //using (DALHelper dal = new DALHelper())
                //{
                //    dal.OpenDB();
                    dt = dal.ExecuteDataSet(query).Tables[0];
                //    dal.CloseDB();
                //}
                return (Convert.ToInt32(dt.Rows[0][0].ToString()) > 0 ? true : false);
            }
            catch
            {
                return false;
            }
        }

        public bool checkIfComputedByHoursPresent(string IDNumber, string PayCycle, bool bCurrentPayCycle)
        {
            try
            {
                string tableName = string.Empty;
                string condition = string.Empty;
                if (bCurrentPayCycle)
                {
                    tableName = "M_Employee";
                }
                else
                {
                    tableName = "M_EmployeeHst";
                    condition = string.Format("AND Mem_PayCycle = '{0}' ", PayCycle);
                }


                DataTable dt = new DataTable();
                string query = string.Format(@"SELECT Mem_IsComputedPerDay FROM {0}
												WHERE Mem_IDNo = '{1}'
                                                {2}"
                                                , tableName, IDNumber, condition);

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(query).Tables[0];
                    dal.CloseDB();
                }
                return (Convert.ToInt32(dt.Rows[0][0].ToString()) > 0 ? true : false);
            }
            catch
            {
                return false;
            }
        }

        public DataTable GetLegend(string Type, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataTable dtLegendList = new DataTable();
            dtLegendList.Columns.Add("Code");
            dtLegendList.Columns.Add("Description");

            if (Type == "REGULAR")
            {
                string PAIDOTHERHOLIDAYDESC = GetOTHHOLDescription("R", CompanyCode, CentralProfile, dal);
                dtLegendList.Rows.Add("REG Amt", "Regular Amount");
                dtLegendList.Rows.Add("PDLV Amt", "Paid Leave Amount");
                dtLegendList.Rows.Add("PDLEGHOL Amt", "Paid Legal Holiday Amount");
                dtLegendList.Rows.Add("PDSPLHOL Amt", "Paid Special Holiday Amount");
                dtLegendList.Rows.Add("PDCOMPHOL Amt", "Paid Company Holiday Amount");
                dtLegendList.Rows.Add("PDPSD Amt", "Paid Plant Shutdown Amount");
                dtLegendList.Rows.Add("PDOTHHOL Amt", PAIDOTHERHOLIDAYDESC + " Amount");
                dtLegendList.Rows.Add("PDRESTLEGHOL Amt", "Paid Legal Holiday Falling on Restday Amount");
                dtLegendList.Rows.Add("Total Regular Pay", "Regular Amount + Paid Leave Amount + Paid Legal Holiday Amount + Paid Special Holiday Amount + Paid Company Holiday Amount + Paid Plant Shutdown Amount + " + PAIDOTHERHOLIDAYDESC + " Amount");

            }
            else if (Type == "ABSENCES")
            {
                dtLegendList.Rows.Add("LT Amt", "Late Amount");
                dtLegendList.Rows.Add("UT Amt", "Undertime Amount");
                dtLegendList.Rows.Add("UPLV Amt", "Unpaid Leave Amount");
                dtLegendList.Rows.Add("WDABS Amt", "Whole Day Absent Amount");
                dtLegendList.Rows.Add("LTUTMAX Hr", "Late/Undertime < Whole Day Absent Amount");
                dtLegendList.Rows.Add("ABSLEGHOL Amt", "Absent Legal Holiday Amount");
                dtLegendList.Rows.Add("ABSSPLHOL Amt", "Absent Special Holiday Amount");
                dtLegendList.Rows.Add("ABSCOMPHOL Amt", "Absent Company Holiday Amount");
                dtLegendList.Rows.Add("ABSPSD Amt", "Absent Plant Shutdown Amount");
                dtLegendList.Rows.Add("ABSOTHHOL Amt", "Absent Other Holiday Amount");
                dtLegendList.Rows.Add("Total ABS Amt", @"Late Amount + Undertime Amount + Unpaid Leave Amount + Whole Day Absent Amount + Absent Legal Holiday Amount + Absent Special Holiday Amount + Absent Company Holiday Amount + Absent Plant Shutdown Amount + Absent Other Holiday Amount");

            }
            else if (Type == "OVERTIME AND NIGHT PREMIUM")
            {
                dtLegendList.Rows.Add("REG OT Amt", "Regular Overtime Amount + Regular Night Premium Amount + Regular Night Premium Overtime Amount");
                dtLegendList.Rows.Add("REST Amt", "Restday Amount + Restday Overtime Amount + Restday Night Premium Amount + Restday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("LEGHOL Amt", "Legal Holiday Amount + Legal Holiday Overtime Amount + Legal Holiday Night Premium Amount + Legal Holiday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("SPLHOL Amt", "Special Holiday Amount + Special Holiday Overtime Amount + Special Holiday Night Premium Amount + Special Holiday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("PSD Amt", "Plant Shutdown Amount + Plant Shutdown Overtime Amount + Plant Shutdown Night Premium Amount + Plant Shutdown Night Premium Overtime Amount");
                dtLegendList.Rows.Add("COMPHOL Amt", "Company Holiday Amount + Company Holiday Overtime Amount + Company Holiday Night Premium Amount + Company Holiday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("RESTLEGHOL Amt", "Legal Holiday Falling on Restday Amount + Legal Holiday Falling on Restday Overtime Amount + Legal Holiday Falling on Restday Night Premium Amount + Legal Holiday Falling on Restday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("RESTSPLHOL Amt", "Special Holiday Falling on Restday Amount + Special Holiday Falling on Restday Overtime Amount + Special Holiday Falling on Restday Night Premium Amount + Special Holiday Falling on Restday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("RESTCOMPHOL Amt", "Company Holiday Falling on Restday Amount + Company Holiday Falling on Restday Overtime Amount + Company Holiday Falling on Restday Night Premium Amount + Company Holiday Falling on Restday Night Premium Overtime Amount");
                dtLegendList.Rows.Add("RESTPSD Amt", "Plant Shutdown Holiday Falling on Restday Amount + Plant Shutdown Holiday Falling on Restday Overtime Amount + Plant Shutdown Holiday Falling on Restday Night Premium Amount + Plant Shutdown Holiday Falling on Restday Night Premium Overtime Amount");

                if ((new CommonBL()).GetFillerDayCodesCount(CompanyCode, CentralProfile) > 0)
                {
                    DataTable dtFiller = (new CommonBL()).GetDayCodeFillers(CompanyCode, CentralProfile);
                    for (int i = 0; i < dtFiller.Rows.Count; i++)
                    {
                        if (i > 0)
                        {
                            if (dtFiller.Rows[i-1]["Mmd_DayCode"].ToString() != dtFiller.Rows[i]["Mmd_DayCode"].ToString())
                                dtLegendList.Rows.Add(string.Format("{0} Amt", dtFiller.Rows[i]["Mmd_DayCode"].ToString()), string.Format("{0} Amount + {0} Overtime Amount + {0} Night Premium Amount + {0} Night Premium Overtime Amount", dtFiller.Rows[i]["Mmd_DayCode"].ToString()));
                        }
                        else
                        dtLegendList.Rows.Add(string.Format("{0} Amt", dtFiller.Rows[i]["Mmd_DayCode"].ToString()), string.Format("{0} Amount + {0} Overtime Amount + {0} Night Premium Amount + {0} Night Premium Overtime Amount", dtFiller.Rows[i]["Mmd_DayCode"].ToString()));
                    }
                }
            }
            return dtLegendList;
        }
        public string GetMaxPayPeriodPremiumSchedule(string PremiumCode, string PayCycle, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            #region query

            string query = @"SELECT Max(Mps_PayCycle) AS [PayCycle]
                           FROM {3}..M_PremiumSchedule
                           WHERE Mps_DeductionCode = '{0}'
                                AND Mps_PayCycle <= '{1}'
                                AND Mps_CompanyCode = '{2}'
                                AND Mps_RecordStatus = 'A'";

            #endregion
            query = string.Format(query, PremiumCode, PayCycle, CompanyCode, CentralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }


        public DataTable GetTotalAdjustment(string IDNumber, string AdjDate, string AdjPayCycle, string OrigAdjPayCycle, DALHelper dal)
        {
            string queryTable = string.Format(@"IF EXISTS (
                                                    SELECT 1 FROM T_EmpSystemAdj
                                                    WHERE Tsa_AdjPayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0

                                                    IF EXISTS (
                                                    SELECT 1 FROM T_EmpSystemAdjHst
                                                    WHERE Tsa_AdjPayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0

                                                    IF EXISTS (
                                                    SELECT 1 FROM T_EmpManualAdj
                                                    WHERE Tma_PayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0

                                                    IF EXISTS (
                                                    SELECT 1 FROM T_EmpManualAdjHst
                                                    WHERE Tma_PayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0", AdjPayCycle);
            DataSet dsResult = null;
            string table = "T_EmpSystemAdj";
            string table1 = "T_EmpSystemAdj2";
            string table2 = "T_EmpSystemAdjMisc";
            string table3 = "T_EmpSystemAdjMisc2";
            string table4 = "T_EmpManualAdj";

            dsResult = dal.ExecuteDataSet(queryTable);

            try
            {
                if (Convert.ToInt32(dsResult.Tables[1].Rows[0][0]) > Convert.ToInt32(dsResult.Tables[0].Rows[0][0]))
                {
                    table = "T_EmpSystemAdjHst";
                    table1 = "T_EmpSystemAdj2Hst";
                    table2 = "T_EmpSystemAdjMiscHst";
                    table3 = "T_EmpSystemAdjMisc2Hst";
                }
                else
                {
                    table = "T_EmpSystemAdj";
                    table1 = "T_EmpSystemAdj2";
                    table2 = "T_EmpSystemAdjMisc";
                    table3 = "T_EmpSystemAdjMisc2";
                }
                if (Convert.ToInt32(dsResult.Tables[3].Rows[0][0]) > Convert.ToInt32(dsResult.Tables[2].Rows[0][0]))
                {
                    table4 = "T_EmpManualAdjHst";
                }
                else
                    table4 = "T_EmpManualAdj";

            }
            catch { }

            #region query 
            string query = string.Format(@"SELECT Tsa_PDLEGHOLHr as [PD LEGHOL HR]
                                          , Tsa_PDSPLHOLHr as [PD SPLHOL HR]
                                          , Tsa_PDCOMPHOLHr as [PD COMPHOL HR]
                                          , Tsa_PDPSDHr as [PD PSD HR]
                                          , Tsa_PDOTHHOLHr as [PD OTHHOL HR]
                                          , Tsa_PDRESTLEGHOLHr as [PD REST LEGHOL HR]

                                          , CASE WHEN A.Tsa_REGHr = 0 AND Tsa_ABSHr > 0 THEN Tsa_ABSHr*-1 ELSE A.Tsa_REGHr END  as [REG HR]
                                          , Tsa_REGOTHr as [REG OT HR]
                                          , Tsa_REGNDHr as [REG ND HR]
                                          , Tsa_REGNDOTHr as [REG NDOT HR]

                                          , Tsa_RESTHr as [REST HR]
	                                      , Tsa_RESTOTHr as [REST OT HR]
                                          , Tsa_RESTNDHr as [REST ND HR]
                                          , Tsa_RESTNDOTHr as [REST NDOT HR]

	                                      , Tsa_LEGHOLHr as [LEGHOL HR]
	                                      , Tsa_LEGHOLOTHr as [LEGHOL OT HR]
                                          , Tsa_LEGHOLNDHr as [LEGHOL ND HR]
                                          , Tsa_LEGHOLNDOTHr as [LEGHOL NDOT HR]

	                                      , Tsa_SPLHOLHr as [SPLHOL HR]
	                                      , Tsa_SPLHOLOTHr as [SPLHOL OT HR]
                                          , Tsa_SPLHOLNDHr as [SPLHOL ND HR]
                                          , Tsa_SPLHOLNDOTHr as [SPLHOL NDOT HR]

	                                      , Tsa_COMPHOLHr as [COMPHOL HR]
	                                      , Tsa_COMPHOLOTHr as [COMPHOL OT HR]
                                          , Tsa_COMPHOLNDHr as [COMPHOL ND HR]
                                          , Tsa_COMPHOLNDOTHr as [COMPHOL NDOT HR]

	                                      , Tsa_PSDHr as [PSD HR]
	                                      , Tsa_PSDOTHr as [PSD OT HR]
                                          , Tsa_PSDNDHr as [PSD ND HR]
                                          , Tsa_PSDNDOTHr as [PSD NDOT HR]

	                                      , Tsa_RESTLEGHOLHr as [REST LEGHOL HR]
	                                      , Tsa_RESTLEGHOLOTHr as [REST LEGHOL OT HR]
                                          , Tsa_RESTLEGHOLNDHr as [REST LEGHOL ND HR]
                                          , Tsa_RESTLEGHOLNDOTHr as [REST LEGHOL NDOT HR]

	                                      , Tsa_RESTSPLHOLHr as [REST SPLHOL HR]
	                                      , Tsa_RESTSPLHOLOTHr as [REST SPLHOL OT HR]
                                          , Tsa_RESTSPLHOLNDHr as [REST SPLHOL ND HR]
                                          , Tsa_RESTSPLHOLNDOTHr as [REST SPLHOL NDOT HR]

	                                      , Tsa_RESTCOMPHOLHr as [REST COMPHOL HR]
	                                      , Tsa_RESTCOMPHOLOTHr as [REST COMPHOL OT HR]
                                          , Tsa_RESTCOMPHOLNDHr as [REST COMPHOL ND HR]
                                          , Tsa_RESTCOMPHOLNDOTHr as [REST COMPHOL NDOT HR]

	                                      , Tsa_RESTPSDHr as [REST PSD HR]
	                                      , Tsa_RESTPSDOTHr as [REST PSD OT HR]
                                          , Tsa_RESTPSDNDHr as [REST PSD ND HR]
                                          , Tsa_RESTPSDNDOTHr as [REST PSD NDOT HR]

                                          , ISNULL(Tsm_Misc1Hr, 0) AS [MISC1 HR]
                                          , ISNULL(Tsm_Misc1OTHr, 0) AS [MISC1 OT HR]
                                          , ISNULL(Tsm_Misc1NDHr, 0) AS [MISC1 ND HR]
                                          , ISNULL(Tsm_Misc1NDOTHr, 0) AS [MISC1 NDOT HR]

                                          , ISNULL(Tsm_Misc2Hr, 0) AS [MISC2 HR]
                                          , ISNULL(Tsm_Misc2OTHr, 0) AS [MISC2 OT HR]
                                          , ISNULL(Tsm_Misc2NDHr, 0) AS [MISC2 ND HR]
                                          , ISNULL(Tsm_Misc2NDOTHr, 0) AS [MISC2 NDOT HR]

                                          , ISNULL(Tsm_Misc3Hr, 0) AS [MISC3 HR]
                                          , ISNULL(Tsm_Misc3OTHr, 0) AS [MISC3 OT HR]
                                          , ISNULL(Tsm_Misc3NDHr, 0) AS [MISC3 ND HR]
                                          , ISNULL(Tsm_Misc3NDOTHr, 0) AS [MISC3 NDOT HR]

                                          , ISNULL(Tsm_Misc4Hr, 0) AS [MISC4 HR]
                                          , ISNULL(Tsm_Misc4OTHr, 0) AS [MISC4 OT HR]
                                          , ISNULL(Tsm_Misc4NDHr, 0) AS [MISC4 ND HR]
                                          , ISNULL(Tsm_Misc4NDOTHr, 0) AS [MISC4 NDOT HR]

                                          , ISNULL(Tsm_Misc5Hr, 0) AS [MISC5 HR]
                                          , ISNULL(Tsm_Misc5OTHr, 0) AS [MISC5 OT HR]
                                          , ISNULL(Tsm_Misc5NDHr, 0) AS [MISC5 ND HR]
                                          , ISNULL(Tsm_Misc5NDOTHr, 0) AS [MISC5 NDOT HR]

                                          , ISNULL(Tsm_Misc6Hr, 0) AS [MISC6 HR]
                                          , ISNULL(Tsm_Misc6OTHr, 0) AS [MISC6 OT HR]
                                          , ISNULL(Tsm_Misc6NDHr, 0) AS [MISC6 ND HR]
                                          , ISNULL(Tsm_Misc6NDOTHr, 0) AS [MISC6 NDOT HR]

                                      FROM {0} A
                                      LEFT JOIN {1} B
                                        ON A.Tsa_IDNo = B.Tsm_IDNo
	                                    AND A.Tsa_AdjPayCycle = B.Tsm_AdjPayCycle
	                                    AND A.Tsa_PayCycle = B.Tsm_PayCycle
	                                    AND A.Tsa_Date = B.Tsm_Date
                                      LEFT JOIN {2} C 
                                        ON C.Tsa_IDNo = A.Tsa_IDNo
	                                    AND C.Tsa_AdjPayCycle = A.Tsa_AdjPayCycle
	                                    AND C.Tsa_PayCycle = A.Tsa_PayCycle
	                                    AND C.Tsa_Date = A.Tsa_Date
                                      LEFT JOIN {3} E 
                                        ON C.Tsa_IDNo = E.Tsm_IDNo
	                                    AND C.Tsa_AdjPayCycle = E.Tsm_AdjPayCycle
	                                    AND C.Tsa_PayCycle = E.Tsm_PayCycle
	                                    AND C.Tsa_Date = E.Tsm_Date  
                                      LEFT JOIN {4} D
									  ON A.Tsa_IDNo = D.Tma_IDNo
									  AND A.Tsa_PayCycle = D.Tma_PayCycle
                                      AND D.Tma_PostFlag = 1 AND D.Tma_RetainUserEntry  = 0
                                      WHERE A.Tsa_AdjPayCycle = '{5}'
									  AND A.Tsa_OrigAdjPayCycle = '{6}'
                                      AND A.Tsa_IDNo = '{7}'
									  AND A.Tsa_Date = '{8}'
									  AND A.Tsa_PostFlag = 1
                                    ", table
                                    , table2
                                    , table1
                                    , table3
                                    , table4
                                    , AdjPayCycle
                                    , OrigAdjPayCycle
                                    , IDNumber
                                    , AdjDate);
            #endregion
            DataTable dtResult = null;
            dtResult = dal.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataTable GetPremiumDetail(string IDNumber, string PayCycle, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string queryTable = string.Format(@"IF EXISTS (
                                                    SELECT 1 FROM T_EmpPayroll
                                                    WHERE Tpy_PayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0

                                                    IF EXISTS (
                                                    SELECT 1 FROM T_EmpPayrollYearly
                                                    WHERE Tpy_PayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0

                                                    IF EXISTS (
                                                    SELECT 1 FROM T_EmpPayrollHst
                                                    WHERE Tpy_PayCycle = '{0}')
                                                    SELECT 1
                                                    ELSE
                                                    SELECT 0", PayCycle);
            DataSet dsResult = null;
            string table = "T_EmpPayroll";
            string table2 = "T_EmpPayrollYearly";
            string query = string.Empty;

            dsResult = dal.ExecuteDataSet(queryTable);

            try
            {
                if (Convert.ToInt32(dsResult.Tables[0].Rows[0][0]) > Convert.ToInt32(dsResult.Tables[1].Rows[0][0]))
                {
                    table = "T_EmpPayroll";
                    table2 = "T_EmpPayrollYearly";
                }
                else if (Convert.ToInt32(dsResult.Tables[1].Rows[0][0]) > Convert.ToInt32(dsResult.Tables[2].Rows[0][0]))
                {
                    table = "T_EmpPayrollYearly";
                    table2 = "T_EmpPayrollYearly";
                }
                else
                {
                    table = "T_EmpPayrollHst";
                    table2 = "T_EmpPayrollHst";
                }
            }
            catch { }

            #region query

            if (PayCycle.Substring(6, 1) == "1")
            {
               query = @"
                                      SELECT Tpy_IDNo
                                      ,Tpy_PayCycle
                                      ,SSSPREM.Mcd_Name [SSS Rule]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_SSSBaseAmt), 1) [SSS Base Amount]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_SSSEE), 1)       [SSS EE]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_SSSER), 1)       [SSS ER]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_ECFundAmt), 1)   [SSS EC]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_MPFEE), 1)       [MPF EE]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_MPFER), 1)       [MPF ER]                                      
                                      ,PHICPREM.Mcd_Name [PHIC Rule]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PhilhealthBaseAmt), 1) [PHIC Base Amount]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PhilhealthEE), 1)    [PHIC EE]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PhilhealthER), 1)    [PHIC ER]
                                      ,HDMFPREM.Mcd_Name [HDMF Rule]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PagIbigBaseAmt), 1) [HDMF Base Amount]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PagIbigEE), 1)       [HDMF EE Nontaxable]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PagIbigTaxEE), 1)    [HDMF EE Taxable]
                                      ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_PagIbigER), 1)       [HDMF ER]
									  FROM {0}
									  LEFT JOIN {4}..M_CodeDtl SSSPREM 
                                        ON SSSPREM.Mcd_Code = Tpy_SSSRule
									    AND SSSPREM.Mcd_CodeType = 'PREMRULE'
                                        AND SSSPREM.Mcd_CompanyCode = '{3}'
									  LEFT JOIN {4}..M_CodeDtl PHICPREM 
                                        ON PHICPREM.Mcd_Code = Tpy_PhilhealthRule
									    AND PHICPREM.Mcd_CodeType = 'PREMRULE'
                                        AND PHICPREM.Mcd_CompanyCode = '{3}'
									  LEFT JOIN {4}..M_CodeDtl HDMFPREM 
                                        ON HDMFPREM.Mcd_Code = Tpy_PagIbigRule
									    AND HDMFPREM.Mcd_CodeType = 'PREMRULE'
                                        AND HDMFPREM.Mcd_CompanyCode = '{3}'
                                      WHERE Tpy_PayCycle = '{1}' 
                                      AND Tpy_IDNo = '{2}'";
            }

            else
            {
                query = @"
                                 SELECT C.Tpy_IDNo
                                ,C.Tpy_PayCycle
                                ,SSSPREM.Mcd_Name [SSS Rule]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, CASE WHEN SSS.Mgr_ReferPreviousIncomePremium = 1 THEN C.Tpy_SSSBaseAmt - ISNULL(Y.Tpy_SSSBaseAmt, 0) ELSE C.Tpy_SSSBaseAmt END ), 1) [SSS Base Amount]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_SSSEE), 1)       [SSS EE]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_SSSER), 1)       [SSS ER]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_ECFundAmt), 1)   [SSS EC]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_MPFEE), 1)       [MPF EE]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_MPFER), 1)       [MPF ER]                                      
                                ,PHICPREM.Mcd_Name [PHIC Rule]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, CASE WHEN PHIC.Mgr_ReferPreviousIncomePremium = 1 THEN C.Tpy_PhilhealthBaseAmt - ISNULL(Y.Tpy_PhilhealthBaseAmt, 0) ELSE C.Tpy_PhilhealthBaseAmt END), 1) [PHIC Base Amount]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_PhilhealthEE), 1)    [PHIC EE]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_PhilhealthER), 1)    [PHIC ER]
                                ,HDMFPREM.Mcd_Name [HDMF Rule]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, CASE WHEN HDMF.Mgr_ReferPreviousIncomePremium = 1 THEN C.Tpy_PagIbigBaseAmt - ISNULL(Y.Tpy_PagIbigBaseAmt, 0) ELSE C.Tpy_PagIbigBaseAmt END), 1) [HDMF Base Amount]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_PagIbigEE), 1)       [HDMF EE Nontaxable]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_PagIbigTaxEE), 1)    [HDMF EE Taxable]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, C.Tpy_PagIbigER), 1)       [HDMF ER]
                                FROM {0} C
                                LEFT JOIN {5} Y ON Y.Tpy_IDNo = C.Tpy_IDNo
                                AND Y.Tpy_PayCycle = LEFT('{1}', 6) + '1'
                                LEFT JOIN {4}..M_CodeDtl SSSPREM ON SSSPREM.Mcd_Code = C.Tpy_SSSRule
                                AND SSSPREM.Mcd_CodeType = 'PREMRULE'
                                AND SSSPREM.Mcd_CompanyCode = '{3}'
                                LEFT JOIN {4}..M_GovRemittance SSS ON SSS.Mgr_CompanyCode = '{3}'
                                AND SSS.Mgr_RemittanceCode ='SSSPREM'
                                LEFT JOIN {4}..M_CodeDtl PHICPREM ON PHICPREM.Mcd_Code = C.Tpy_PhilhealthRule
                                AND PHICPREM.Mcd_CodeType = 'PREMRULE'
                                AND PHICPREM.Mcd_CompanyCode = '{3}'
                                LEFT JOIN {4}..M_GovRemittance PHIC ON PHIC.Mgr_CompanyCode = '{3}'
                                AND PHIC.Mgr_RemittanceCode ='PHICPREM'
                                LEFT JOIN {4}..M_CodeDtl HDMFPREM ON HDMFPREM.Mcd_Code = C.Tpy_PagIbigRule
                                AND HDMFPREM.Mcd_CodeType = 'PREMRULE'
                                AND HDMFPREM.Mcd_CompanyCode = '{3}'
                                LEFT JOIN {4}..M_GovRemittance HDMF ON HDMF.Mgr_CompanyCode = '{3}'
                                AND HDMF.Mgr_RemittanceCode ='HDMFPREM'
                                WHERE C.Tpy_PayCycle = '{1}' 
                                AND C.Tpy_IDNo = '{2}'";
            }

            #endregion
            query = string.Format(query, table, PayCycle, IDNumber, CompanyCode, CentralProfile, table2);

            DataTable dtResult = null;
            dtResult = dal.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        
    }
}
