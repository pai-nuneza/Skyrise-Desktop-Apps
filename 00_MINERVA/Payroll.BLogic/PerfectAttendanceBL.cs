using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class PerfectAttendanceBL : BaseBL
    {
        #region Overridden Methods
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

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Event handlers for off-cycle processing
        public delegate void EmpDispEventHandler(object sender, EmpDispEventArgs e);
        public class EmpDispEventArgs : System.EventArgs
        {
            public string EmployeeId;
            public string LastName;
            public string FirstName;
            public string Stage;
            public string StatusMsg;

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst, string strStage)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                Stage = strStage;
                StatusMsg = "Successful";
            }

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst, string strStage, string strStatusMsg)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                Stage = strStage;
                StatusMsg = strStatusMsg;
            }
        }
        public event EmpDispEventHandler EmpDispHandler;
        #endregion


        #region Generation and Posting

        public int PostToIncome(bool ProcessSeparated, string PayCycleCode, string ControlNo, string IncomeCode, string DeductionCode, string userLogin, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            int retVal = 0;
            string tableIncome = CommonConstants.TableName.T_EmpIncome;
            if (ProcessSeparated)
                tableIncome = CommonConstants.TableName.T_EmpIncomeFinalPay;
            #region Income Query
            string query = string.Format(@"
                                                    DECLARE @CurPayCycleStart AS datetime
                                                    SET @CurPayCycleStart = (SELECT Tps_StartCycle
							                                            FROM T_PaySchedule
						                                                WHERE Tps_PayCycle = (SELECT Tah_PayCycle FROM T_EmpAttendanceHdr
																							    WHERE Tah_ControlNo = '{1}'))

                                                    IF NOT EXISTS(SELECT * FROM {6}..M_Income WHERE Min_IncomeCode = '{2}' AND Min_CompanyCode = '{5}')
                                                    BEGIN
	                                                    INSERT INTO {6}..M_Income
                                                               (Min_CompanyCode
                                                               ,Min_IncomeCode
                                                               ,Min_IncomeName
                                                               ,Min_TaxClass
                                                               ,Min_IsRecurring
															   ,Min_ApplicablePayCycle
															   ,Min_IncomeGroup
															   ,Min_AlphalistCategory
															   ,Min_IsSystemReserved
                                                               ,Min_AccountGrp
															   ,Min_RemittanceLoanType
                                                               ,Min_RecordStatus
                                                               ,Min_CreatedBy
                                                               ,Min_CreatedDate)
                                                         VALUES
                                                               ('{5}'
                                                               ,'{2}'
                                                               ,'{2} NONTAXABLE'
                                                               ,'N'
                                                               ,0
                                                               ,0
                                                               ,''
                                                               ,'N-SALOTH'
                                                               , 1
                                                               ,''
                                                               ,''
															   ,'A'
                                                               ,'{4}'
                                                               ,GETDATE())
                                                    END

                                                    DELETE FROM {7}
                                                    WHERE Tin_IncomeCode IN ('{2}')
                                                        AND Tin_PayCycle = '{0}'

                                                    INSERT INTO {7}
                                                               (Tin_IDNo
                                                               ,Tin_PayCycle
                                                               ,Tin_OrigPayCycle
                                                               ,Tin_IncomeCode
                                                               ,Tin_PostFlag
                                                               ,Tin_IncomeAmt
                                                               ,Usr_Login
                                                               ,Ludatetime)
                                                    SELECT Tad_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'{2}'
                                                          ,0
                                                          ,Tad_NetAmt
                                                          ,'{4}'
                                                          ,GETDATE()
                                                      FROM T_EmpAttendanceDtl
                                                      WHERE Tad_NetAmt > 0
                                                        AND Tad_ControlNo = '{1}'


                                                      UPDATE T_EmpAttendanceDtl
                                                      SET Tad_PostFlag = 1
                                                        , Usr_login = '{4}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE Tad_NetAmt != 0
                                                        AND Tad_ControlNo = '{1}'

                                                      UPDATE T_EmpAttendanceDtl
                                                      SET Tad_PostFlag = 0
                                                        , Usr_login = '{4}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE Tad_NetAmt = 0
                                                        AND Tad_ControlNo = '{1}'", PayCycleCode
                                                                                    , ControlNo
                                                                                    , IncomeCode
                                                                                    , DeductionCode
                                                                                    , userLogin
                                                                                    , CompanyCode
                                                                                    , CentralProfile
                                                                                    , tableIncome);


            #endregion

            #region //Deduction Query

            //string queryDedn = @"IF NOT EXISTS(SELECT * FROM {6}..M_Deduction WHERE Mdn_DeductionCode = '{2}' AND Mdn_CompanyCode = '{5}')
            //                                        BEGIN
	           //                                         INSERT INTO {6}..M_Deduction
            //                                            (Mdn_CompanyCode
            //                                            ,Mdn_DeductionCode
            //                                            ,Mdn_DeductionName
            //                                            ,Mdn_DeductionGroup
            //                                            ,Mdn_PriorityNo
            //                                            ,Mdn_PaymentTerms
            //                                            ,Mdn_ApplicablePayCycle
            //                                            ,Mdn_IsAllowDeferred
            //                                            ,Mdn_IsAllowPartialPayment
            //                                            ,Mdn_WithPrincipalAmount
            //                                            ,Mdn_WithCheckDate
            //                                            ,Mdn_AlphalistCategory
            //                                            ,Mdn_WithAccountingVoucher
            //                                            ,Mdn_PaidUpAmount
            //                                            ,Mdn_IsRecurring
            //                                            ,Mdn_AccountGrp
            //                                            ,Mdn_RemittanceLoanType
            //                                            ,Mdn_IsSystemReserved
            //                                            ,Mdn_RecordStatus
            //                                            ,Mdn_CreatedBy
            //                                            ,Mdn_CreatedDate)
            //                                            VALUES
            //                                                   ('{5}'
            //                                                   ,'{2}'
            //                                                   ,'{2}'
            //                                                   ,'' ----Mdn_DeductionGroup
            //                                                   , 1
            //                                                   , 1
            //                                                   , 0
            //                                                   , 0
            //                                                   , 0
            //                                                   , 0
            //                                                   , 0
            //                                                   , 'N-NOTINCLUDE' ---Mdn_AlphalistCategory
            //                                                   , 0
            //                                                   , 0.00
            //                                                   , 0
            //                                                   , '' ---Mdn_AccountGrp
            //                                                   , '' ---Mdn_RemittanceLoanType
            //                                                   , 1 
												//			   ,'A'
            //                                                   ,'{4}'
            //                                                   ,GETDATE())
            //                                        END

            //                                         DELETE FROM {6}..T_EmpDeductionHdr
            //                                         WHERE Tdh_DeductionCode = '{3}'
            //                                         AND Tdh_StartDate = @CurPayCycleStart


            //                                        INSERT INTO {6}..T_EmpDeductionHdr
            //                                               (Tdh_IDNo
												//			,Tdh_DeductionCode
												//			,Tdh_StartDate
												//			,Tdh_EndDate
												//			,Tdh_DeductionAmount
												//			,Tdh_PaidAmount
												//			,Tdh_CycleAmount
												//			,Tdh_DeferredAmount
												//			,Tdh_CheckDate
												//			,Tdh_VoucherNumber
												//			,Tdh_PaymentDate
												//			,Tdh_ExemptInPayroll
												//			,Tdh_PrincipalAmount
												//			,Tdh_ApplicablePayCycle
												//			,Tdh_AgreementNo
												//			,Tdh_DocumentNo
												//			,Tdh_EntryDate
												//			,Tdh_PaymentTerms
												//			,Usr_Login
												//			,Ludatetime)
            //                                     SELECT Tad_IDNo
		          //                                      , '{3}'
		          //                                      , @CurPayCycleStart
												//		, NULL
		          //                                      , Tad_NetAmt * -1
			         //                                   , 0
			         //                                   , 0
			         //                                   , 0
			         //                                   , NULL
			         //                                   , ''
			         //                                   , NULL
			         //                                   , 0
												//		, 0
												//		, 0
												//		, ''
												//		, ''        ---Tdh_DocumentNo
												//		, GETDATE() ---Tdh_EntryDate
            //                                            , 1         ---Tdh_PaymentTerms
			         //                                   , '{4}'
			         //                                   , GETDATE()
            //                                    FROM T_EmpAttendanceDtl
            //                                    WHERE Tad_ControlNo = @ControlNo
            //                                    AND Tad_NetAmt < 0";

           // string updateQuery = string.Format(@"DECLARE @yr2Digit varchar(2)
           //                                 SET @yr2Digit = (SELECT right('{3}', 2))
    
           //                                 UPDATE T_DocumentNumber
           //                                 SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
           //                                 WHERE Tdn_DocumentCode = 'DEDDOCNUM'

           //                                 UPDATE {0}..T_EmpDeductionHdr
											//SET Tdh_DocumentNo = (SELECT Tdn_DocumentPrefix 
           //                                     + (SELECT  Mpf_DatabasePrefix FROM {0}..M_Profile WHERE Mpf_DatabaseNo = '{1}' AND Mpf_CompanyCode = '{2}')
           //                                  + @yr2Digit 
           //                                  + RIGHT('00000000000' + CAST(Tdn_LastSeriesNumber AS varchar), 11)
           //                                 FROM T_DocumentNumber
           //                                 WITH (UPDLOCK)
           //                                 WHERE Tdn_DocumentCode = 'DEDDOCNUM')
											//WHERE Tdh_IDNo = @Tdh_IDNo
           //                                    AND Tdh_DeductionCode = @Tdh_DeductionCode
           //                                    AND Tdh_StartDate = @Tdh_StartDate"
           //                                 , CentralProfile
           //                                 , LoginInfo.getUser().DBNumber
           //                                 , CompanyCode
           //                                 , (new CommonBL()).GetParameterValueFromPayroll("COMPYEAR", CompanyCode));

            #endregion
            try
            {
                retVal = dal.ExecuteNonQuery(query);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        #endregion

        #region Utilities

        public string GetNextAvailableSeqNo(string ControlNo)
        {
            #region query
            string query = string.Format(@"SELECT TOP 1 Tad_LineNo 
                                            FROM T_AttendanceProcessingDtl
                                            WHERE Tad_ControlNo = '{0}'
                                            ORDER BY Tad_LineNo DESC", ControlNo);
            #endregion
            DataTable dtResult;
            string SubSeqNo = "";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0)
                SubSeqNo = string.Format("{0:000}", Convert.ToInt32(dtResult.Rows[0][0]) + 1);
            else
                SubSeqNo = "001";
            return SubSeqNo;
        }

        public string GetNextAvailableSubSeqNo(string ControlNo, string SeqNo)
        {
            #region query
            string query = string.Format(@"SELECT TOP 1 Tap_LineSplitNo 
                                            FROM T_AttendanceProcessingException
                                            WHERE Tap_ControlNo = '{0}'
                                                AND Tap_LineNo = '{1}'
                                            ORDER BY Tap_LineSplitNo DESC", ControlNo, SeqNo);
            #endregion
            DataTable dtResult;
            string SubSeqNo = "";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0)
                SubSeqNo = string.Format("{0:000}", Convert.ToInt32(dtResult.Rows[0][0]) + 1);
            else
                SubSeqNo = "001";
            return SubSeqNo;
        }

        public string GetNextAvailableSubSeqNo(string ControlNo, string SeqNo, DALHelper dal)
        {
            #region query
            string query = string.Format(@"SELECT TOP 1 Tap_LineSplitNo 
                                            FROM T_EmpAttendanceException
                                            WHERE Tap_ControlNo = '{0}'
                                                AND Tap_LineNo = '{1}'
                                            ORDER BY Tap_LineSplitNo DESC", ControlNo, SeqNo);
            #endregion
            DataTable dtResult;
            string SubSeqNo = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                SubSeqNo = string.Format("{0:000}", Convert.ToInt32(dtResult.Rows[0][0]) + 1);
            else
                SubSeqNo = "001";
            return SubSeqNo;
        }





       

        public DataTable GetUserGeneratedExclusion(string ControlNo, DALHelper dal)
        {
            #region query
            string query = string.Format(@"SELECT Tad_IDNo
                                                , Tap_Date as 'Date'
                                                , Tap_ExceptionType as 'Exclusion Type'
                                                , Tap_Remarks as 'Remarks'
                                            FROM T_EmpAttendanceException
                                            LEFT JOIN T_EmpAttendanceDtl
	                                            ON Tad_ControlNo = Tap_ControlNo
	                                            AND Tad_LineNo = Tap_LineNo
                                            WHERE Tap_IsSystemGenerated = 0
                                            AND Tap_ControlNo = '{0}'
                                            AND Tap_RecordStatus = 'A'", ControlNo);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public bool CheckIfExclusionExists(string EmployeeId, DataTable dtExclusion, DataTable dtUserExclusion)
        {
            DataRow[] drArrResult = dtExclusion.Select("[" + dtExclusion.Columns[0].ColumnName + "] = '" + EmployeeId + "'");
            if (drArrResult.Length == 0) //not in system exclusion
            {
                drArrResult = dtUserExclusion.Select("[" + dtUserExclusion.Columns[0].ColumnName + "] = '" + EmployeeId + "'");
                if (drArrResult.Length == 0) //not in user exclusion
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        public int GetDeductionDays(string IDNumber, DateTime dtStart, DateTime dtEnd, DataTable dtExclusion, DataTable dtUserExclusion)
        {
            DateTime dtDate = dtStart;
            int deductionCnt = 0;
            DataRow[] drArrResult;
            while (dtDate <= dtEnd)
            {
                drArrResult = dtExclusion.Select("[" + dtExclusion.Columns[0].ColumnName + "] = '" + IDNumber + "' AND [" + dtExclusion.Columns[1].ColumnName + "] = '" + dtDate.ToShortDateString() + "'");
                if (drArrResult.Length > 0)
                    deductionCnt++;
                else
                {
                    drArrResult = dtUserExclusion.Select("[" + dtUserExclusion.Columns[0].ColumnName + "] = '" + IDNumber + "' AND [" + dtUserExclusion.Columns[1].ColumnName + "] = '" + dtDate.ToShortDateString() + "'");
                    if (drArrResult.Length > 0)
                        deductionCnt++;
                }
                dtDate = dtDate.AddDays(1);
            }

            return deductionCnt;
        }

        public int GetUntitleDays(string IDNumber, object RegularDate, string dtStart, string dtEnd)
        {
            string query = string.Format(@"DECLARE @VARIABLEDATE DATE = '{1}'
                                            DECLARE @START DATE = '{2}'
                                            DECLARE @END DATET = '{3}'

                                            IF CASE WHEN @VARIABLEDATE BETWEEN @START AND @END
		                                            THEN 1 
		                                            ELSE 0 END = 1
                                            BEGIN
	                                            SET @END = @VARIABLEDATE

	                                            SELECT Ttr_Date,Ttr_DayCode
	                                            FROM T_EmpTimeRegister 
	                                            WHERE Ttr_IDNo= '{0}'
	                                            AND Ttr_DayCode LIKE 'REG%' 
	                                            AND Ttr_Date >= @START AND Ttr_Date < @END 

	                                            UNION ALL 

	                                            SELECT Ttr_Date,Ttr_DayCode  
	                                            FROM T_EmpTimeRegisterHst 
	                                            WHERE Ttr_IDNo= '{0}'
	                                            AND Ttr_DayCode LIKE 'REG%' 
	                                            AND Ttr_Date >= @START AND Ttr_Date < @END 

	                                            ---ORDER BY Ttr_Date
                                            END", IDNumber
                                                , RegularDate
                                                , dtStart
                                                , dtEnd);
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows.Count;
                else return 0;
            } 
            else return 0;
        }

        public DataRow GetEmployeeDetailRecord(string IDNumber, DataTable dtMasterFile)
        {
                DataRow[] drArrResult = dtMasterFile.Select("[" + dtMasterFile.Columns[0].ColumnName + "] = '" + IDNumber + "'");
                if (drArrResult.Length > 0)
                    return drArrResult[0];
                else
                    return null;
           
        }


        #endregion


        public int InsertHdr(DataRow row, DALHelper dalHelper)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[28];
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ControlNo", row["Tah_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PayCycle", row["Tah_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_RuleCode", row["Tah_RuleCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Effectivity", row["Tah_Effectivity"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Frequency", row["Tah_Frequency"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ComputeEntitlement", row["Tah_ComputeEntitlement"]); 
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ComputeUntitledDays", row["Tah_ComputeUntitledDays"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ExclusionFormula", row["Tah_ExclusionFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ExclusionType", row["Tah_ExclusionType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_WithTimer", row["Tah_WithTimer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Computation", row["Tah_Computation"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Classification", row["Tah_Classification"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_IncomeCode", row["Tah_IncomeCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_DeductionCode", row["Tah_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ComputeDeductionDays", row["Tah_ComputeDeductionDays"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_IDNoLIst", row["Tah_IDNoLIst"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_CostCenterList", row["Tah_CostCenterList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PayrollGroupList", row["Tah_PayrollGroupList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_EmploymentStatusList", row["Tah_EmploymentStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PayrollTypeList", row["Tah_PayrollTypeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PositionList", row["Tah_PositionList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_GradeList", row["Tah_GradeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_DateOption", row["Tah_DateOption"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_CutOffDate", row["Tah_CutOffDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_StartDate", row["Tah_StartDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_EndDate", row["Tah_EndDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_RecordStatus", row["Tah_RecordStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);


            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpAttendanceHdr
                                            (Tah_ControlNo
                                            ,Tah_ProcessedDate
                                            ,Tah_PayCycle
                                            ,Tah_RuleCode
                                            ,Tah_Effectivity
                                            ,Tah_Frequency
                                            ,Tah_ComputeEntitlement
                                            ,Tah_ComputeUntitledDays
                                            ,Tah_ExclusionFormula
                                            ,Tah_ExclusionType
                                            ,Tah_WithTimer
                                            ,Tah_Computation
                                            ,Tah_Classification
                                            ,Tah_IncomeCode
                                            ,Tah_DeductionCode
                                            ,Tah_ComputeDeductionDays
                                            ,Tah_IDNoLIst
                                            ,Tah_CostCenterList
                                            ,Tah_PayrollGroupList
                                            ,Tah_EmploymentStatusList
                                            ,Tah_PayrollTypeList
                                            ,Tah_PositionList
                                            ,Tah_GradeList
                                            ,Tah_DateOption
                                            ,Tah_CutOffDate
                                            ,Tah_StartDate
                                            ,Tah_EndDate
                                            ,Tah_RecordStatus
                                            ,Usr_Login
                                            ,Ludatetime)

                                            VALUES(@Tah_ControlNo
                                            , GETDATE()
                                            , @Tah_PayCycle
                                            , @Tah_RuleCode
                                            , @Tah_Effectivity
                                            , @Tah_Frequency
                                            , @Tah_ComputeEntitlement
                                            , @Tah_ComputeUntitledDays
                                            , @Tah_ExclusionFormula
                                            , @Tah_ExclusionType
                                            , @Tah_WithTimer
                                            , @Tah_Computation
                                            , @Tah_Classification
                                            , @Tah_IncomeCode
                                            , @Tah_DeductionCode
                                            , @Tah_ComputeDeductionDays
                                            , @Tah_IDNoLIst
                                            , @Tah_CostCenterList
                                            , @Tah_PayrollGroupList
                                            , @Tah_EmploymentStatusList
                                            , @Tah_PayrollTypeList
                                            , @Tah_PositionList
                                            , @Tah_GradeList
                                            , @Tah_DateOption
                                            , @Tah_CutOffDate
                                            , @Tah_StartDate
                                            , @Tah_EndDate
                                            , @Tah_RecordStatus
                                            , @Usr_Login
                                            , GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dalHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int UpdateHdr(DataRow row, DALHelper dalHelper)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[28];
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ControlNo", row["Tah_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PayCycle", row["Tah_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_RuleCode", row["Tah_RuleCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Effectivity", row["Tah_Effectivity"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Frequency", row["Tah_Frequency"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ComputeEntitlement", row["Tah_ComputeEntitlement"]); 
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ComputeUntitledDays", row["Tah_ComputeUntitledDays"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ExclusionFormula", row["Tah_ExclusionFormula"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ExclusionType", row["Tah_ExclusionType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_WithTimer", row["Tah_WithTimer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Computation", row["Tah_Computation"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_Classification", row["Tah_Classification"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_IncomeCode", row["Tah_IncomeCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_DeductionCode", row["Tah_DeductionCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_ComputeDeductionDays", row["Tah_ComputeDeductionDays"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_IDNoLIst", row["Tah_IDNoLIst"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_CostCenterList", row["Tah_CostCenterList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PayrollGroupList", row["Tah_PayrollGroupList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_EmploymentStatusList", row["Tah_EmploymentStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PayrollTypeList", row["Tah_PayrollTypeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_PositionList", row["Tah_PositionList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_GradeList", row["Tah_GradeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_DateOption", row["Tah_DateOption"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_CutOffDate", row["Tah_CutOffDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_StartDate", row["Tah_StartDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_EndDate", row["Tah_EndDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tah_RecordStatus", row["Tah_RecordStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Update Query
            string sqlUpdate = @"UPDATE T_EmpAttendanceHdr
                                SET Tah_ProcessedDate = GETDATE()
                                , Tah_RuleCode = @Tah_RuleCode
                                , Tah_Effectivity = @Tah_Effectivity
                                , Tah_Frequency = @Tah_Frequency
                                , Tah_ComputeEntitlement = @Tah_ComputeEntitlement
                                , Tah_ComputeUntitledDays = @Tah_ComputeUntitledDays
                                , Tah_ExclusionFormula = @Tah_ExclusionFormula
                                , Tah_ExclusionType = @Tah_ExclusionType
                                , Tah_WithTimer = @Tah_WithTimer
                                , Tah_Computation = @Tah_Computation
                                , Tah_Classification = @Tah_Classification
                                , Tah_IncomeCode = @Tah_IncomeCode
                                , Tah_DeductionCode = @Tah_DeductionCode
                                , Tah_ComputeDeductionDays = @Tah_ComputeDeductionDays
                                , Tah_IDNoLIst = @Tah_IDNoLIst
                                , Tah_CostCenterList = @Tah_CostCenterList
                                , Tah_PayrollGroupList = @Tah_PayrollGroupList
                                , Tah_EmploymentStatusList = @Tah_EmploymentStatusList
                                , Tah_PayrollTypeList = @Tah_PayrollTypeList
                                , Tah_PositionList = @Tah_PositionList
                                , Tah_GradeList = @Tah_GradeList
                                , Tah_DateOption = @Tah_DateOption
                                , Tah_CutOffDate = @Tah_CutOffDate
                                , Tah_StartDate = @Tah_StartDate
                                , Tah_EndDate = @Tah_EndDate
                                , Tah_RecordStatus = @Tah_RecordStatus
                                , Usr_Login = @Usr_Login
                                , Ludatetime = GETDATE()
                                WHERE Tah_ControlNo = @Tah_ControlNo
                                    AND Tah_PayCycle = @Tah_PayCycle";
            #endregion

            try
            {
                retVal = dalHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertRule(DataRow row, DALHelper dalHelper)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_ControlNo", row["Tar_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_RuleType", row["Tar_RuleType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_Timer", row["Tar_Timer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_Classification", row["Tar_Classification"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_Amount", row["Tar_Amount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpAttendanceRule
                                            (Tar_ControlNo
                                            ,Tar_RuleType
                                            ,Tar_Timer
                                            ,Tar_Classification
                                            ,Tar_Amount
                                            ,Usr_Login
                                            ,Ludatetime)
                                            VALUES(@Tar_ControlNo
                                            , @Tar_RuleType
                                            , @Tar_Timer
                                            , @Tar_Classification
                                            , @Tar_Amount
                                            , @Usr_Login
                                            , GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dalHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int UpdateRule(DataRow row, DALHelper dalHelper)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_ControlNo", row["Tar_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_RuleType", row["Tar_RuleType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_Timer", row["Tar_Timer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_Classification", row["Tar_Classification"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tar_Amount", row["Tar_Amount"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Update Query
            string sqlUpdate = @"UPDATE T_EmpAttendanceRule
                                SET Tar_Amount = @Tar_Amount
                                , Usr_Login = @Usr_Login
                                , Ludatetime = GETDATE()
                                WHERE Tar_ControlNo = @Tar_ControlNo
                                    AND Tar_RuleType = @Tar_RuleType
                                    AND Tar_Timer = @Tar_Timer
                                    AND Tar_Classification = @Tar_Classification";
            #endregion

            try
            {
                retVal = dalHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertDtl(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[21];
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_ControlNo", row["Tad_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_LineNo", row["Tad_LineNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_IDNo", row["Tad_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_EntitledAmt", row["Tad_EntitledAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_UntitledDays", row["Tad_UntitledDays"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_DailyUntitledAmt", row["Tad_DailyUntitledAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_UntitledAmt", row["Tad_UntitledAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_DeductionDays", row["Tad_DeductionDays"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_DailyDeductionAmt", row["Tad_DailyDeductionAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_DeductionAmt", row["Tad_DeductionAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_NetAmt", row["Tad_NetAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_HasAvailed", row["Tad_HasAvailed"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_PostFlag", row["Tad_PostFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_Timer", row["Tad_Timer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_CostCenter", row["Tad_CostCenter"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_PayrollGroup", row["Tad_PayrollGroup"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_EmploymentStatus", row["Tad_EmploymentStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_PayrollType", row["Tad_PayrollType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_Position", row["Tad_Position"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tad_Grade", row["Tad_Grade"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpAttendanceDtl
                                            (Tad_ControlNo
                                            ,Tad_LineNo
                                            ,Tad_IDNo
                                            ,Tad_EntitledAmt
                                            ,Tad_UntitledDays
                                            ,Tad_DailyUntitledAmt
                                            ,Tad_UntitledAmt
                                            ,Tad_DeductionDays
                                            ,Tad_DailyDeductionAmt
                                            ,Tad_DeductionAmt
                                            ,Tad_NetAmt
                                            ,Tad_HasAvailed
                                            ,Tad_PostFlag
                                            ,Tad_Timer
                                            ,Tad_CostCenter
                                            ,Tad_PayrollGroup
                                            ,Tad_EmploymentStatus
                                            ,Tad_PayrollType
                                            ,Tad_Position
                                            ,Tad_Grade
                                            ,Usr_Login
                                            ,Ludatetime)

                                            VALUES(@Tad_ControlNo
                                            ,@Tad_LineNo
                                            ,@Tad_IDNo
                                            ,@Tad_EntitledAmt
                                            ,@Tad_UntitledDays
                                            ,@Tad_DailyUntitledAmt
                                            ,@Tad_UntitledAmt
                                            ,@Tad_DeductionDays
                                            ,@Tad_DailyDeductionAmt
                                            ,@Tad_DeductionAmt
                                            ,@Tad_NetAmt
                                            ,@Tad_HasAvailed
                                            ,@Tad_PostFlag
                                            ,@Tad_Timer
                                            ,@Tad_CostCenter
                                            ,@Tad_PayrollGroup
                                            ,@Tad_EmploymentStatus
                                            ,@Tad_PayrollType
                                            ,@Tad_Position
                                            ,@Tad_Grade
                                            ,@Usr_Login
                                            ,GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertLedger(DataRow row, string CentralProfile, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_IDNo", row["Tal_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_RuleCode", row["Tal_RuleCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_Timer", row["Tal_Timer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_PrevTimer", row["Tal_PrevTimer"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_FirstEffectivity", row["Tal_FirstEffectivity"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_LastEffectivity", row["Tal_LastEffectivity"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tal_CreatedBy", row["Tal_CreatedBy"]);

            #region Insert Query
            string sqlInsert = string.Format(@" IF NOT EXISTS(SELECT Tal_IDNo FROM {0}..T_EmpAttendanceLdg 
                                                              WHERE Tal_IDNo = @Tal_IDNo
                                                              AND Tal_RuleCode = @Tal_RuleCode)

                                                BEGIN

                                                    INSERT INTO {0}..T_EmpAttendanceLdg
                                                                      (Tal_IDNo
                                                                      ,Tal_RuleCode
                                                                      ,Tal_Timer
                                                                      ,Tal_PrevTimer
                                                                      ,Tal_FirstEffectivity
                                                                      ,Tal_LastEffectivity
                                                                      ,Tal_CreatedBy
                                                                      ,Tal_CreatedDate)
                                                                VALUES(@Tal_IDNo
                                                                      ,@Tal_RuleCode
                                                                      ,@Tal_Timer
                                                                      ,@Tal_PrevTimer
                                                                      ,@Tal_FirstEffectivity
                                                                      ,@Tal_LastEffectivity
                                                                      ,@Tal_CreatedBy
                                                                      ,GETDATE())

                                                END
                                                ELSE 
                                                BEGIN
                                                    UPDATE {0}..T_EmpAttendanceLdg SET Tal_Timer = @Tal_Timer
                                                        , Tal_LastEffectivity = @Tal_LastEffectivity
                                                        , Tal_UpdatedBy = @Tal_CreatedBy
                                                        , Tal_UpdatedDate = GETDATE()
                                                    WHERE Tal_IDNo = @Tal_IDNo
                                                        AND Tal_RuleCode = @Tal_RuleCode
                                                END", CentralProfile);

            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public DataTable GetAccountCode(string AccountType, string strMenuCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcd_Code AS 'Code'
                                               , Mcd_Name AS 'Description'
                                          FROM {2}..M_CodeDtl
                                          WHERE Mcd_RecordStatus = 'A'
                                                AND Mcd_CodeType = '{0}' 
                                                AND Mcd_CompanyCode = '{1}'
                                                AND Mcd_Code IN (SELECT Mpd_SubCode 
                                                                        FROM M_PolicyDtl 
				                                                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				                                                        AND Mpd_ParamValue = 1)", AccountType, CompanyCode, CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetCostCenters(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcc_CostCenterCode as 'Cost Center Code'
                                                , dbo.Udf_DisplayCostCenterName('{0}', Mcc_CostCenterCode,'{1}') as 'Description'
                                            FROM M_CostCenter
                                            WHERE Mcc_RecordStatus = 'A'
                                                AND Mcc_CompanyCode = '{0}'"
                                            , CompanyCode, (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public decimal ComputeDailyAmt(string IDNumber
                                      , int empTimer
                                      , string RuleCode
                                      , string RuleType
                                      , string Classification
                                      , string ComputationType
                                      , string CentralProfile
                                      , string CompanyCode
                                      , string StartDate
                                      , string EndDate
                                      , string PayCycleCode
                                      , DataTable dtAttendanceMasterDtl
                                      , string PayrollType
                                      , decimal SalaryRate
                                      , decimal MDIVISOR
                                      , decimal HRLYRTEDEC)
        {
            decimal dAmt = 0;

            if (ComputationType == "S")
            {
                if (PayrollType == "D")
                    dAmt = SalaryRate;
                else if (PayrollType == "M")
                    dAmt = SalaryRate * 12 / MDIVISOR;

                if (HRLYRTEDEC > 0)
                {
                    if (PayrollType == "D")
                        dAmt = Math.Round(SalaryRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                    else if (PayrollType == "M")
                        dAmt = Math.Round(SalaryRate * 12 / MDIVISOR, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                }
            } 
            else if (ComputationType == "V")
            {
                DataRow[] drResult = dtAttendanceMasterDtl.Select(string.Format(@"Mrd_RuleCode = '{0}' AND Mrd_RuleType = '{1}' AND Mrd_Timer = '{2}' AND Mrd_Classification = '{3}'", RuleCode, RuleType, empTimer, Classification));
                if (drResult.Length > 0)
                    dAmt = Convert.ToDecimal(drResult[0]["Mrd_Amount"].ToString());
            }
            else if (ComputationType == "F")
            {
                DataRow[] drResult = dtAttendanceMasterDtl.Select(string.Format(@"Mrd_RuleCode = '{0}' AND Mrd_RuleType = '{1}' AND Mrd_Timer = '{2}'", RuleCode, RuleType, empTimer));
                if (drResult.Length > 0)
                    dAmt = Convert.ToDecimal(drResult[0]["Mrd_Amount"].ToString());
            }

            return dAmt;
        }

        public void DeletePerfectAttendance(string ControlNo, string IDNumber, string LineNo)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Format(@"DELETE FROM T_EmpAttendanceDtl
                                              WHERE Tad_ControlNo = '{0}'
                                                AND Tad_IDNo = '{1}'

                                              DELETE FROM T_EmpAttendanceException
                                              WHERE Tap_ControlNo = '{0}'
                                                AND Tap_LineNo = '{2}'
                                                AND Tap_IsSystemGenerated = 1

                                            IF NOT EXISTS (SELECT Tad_LineNo FROM T_EmpAttendanceDtl WHERE Tad_ControlNo = '{0}' )
                                            BEGIN
	                                            UPDATE T_EmpAttendanceHdr SET Tah_RecordStatus = 'C'
	                                            WHERE Tah_ControlNo = '{0}'
                                            END ", ControlNo, IDNumber, LineNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(sqlQuery);
                dal.CloseDB();
            }
        }


        public void DeletePerfectAttendanceException(string strControlNo, string strLineNo , string strLineSplitNo)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Format(@"DELETE FROM T_EmpAttendanceException
                                              WHERE Tap_ControlNo = '{0}'
                                                AND Tap_LineNo = '{1}'
                                                AND Tap_LineSplitNo = '{2}'

                                           ", strControlNo, strLineNo, strLineSplitNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(sqlQuery);
                dal.CloseDB();
            }
        }
    }
}
