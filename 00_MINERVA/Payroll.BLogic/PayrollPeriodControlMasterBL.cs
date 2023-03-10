using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class PayrollPeriodControlMasterBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[0] = new ParameterInfo("@payrollPeriod", row["Tps_PayCycle"], SqlDbType.Char, 7);
            paramInfo[1] = new ParameterInfo("@indicator", row["Tps_CycleIndicator"], SqlDbType.Char, 1);
            paramInfo[2] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);
            paramInfo[3] = new ParameterInfo("@Tps_Remarks", row["Tps_Remarks"], SqlDbType.VarChar, 200);
            paramInfo[4] = new ParameterInfo("@Tps_ComputeTax", row["Tps_ComputeTax"]);
            paramInfo[5] = new ParameterInfo("@Tps_TaxSchedule", row["Tps_TaxSchedule"]);
            paramInfo[6] = new ParameterInfo("@Tps_CycleIndicatorSpecial", row["Tps_CycleIndicatorSpecial"]);
            paramInfo[7] = new ParameterInfo("@Tps_CycleType", row["Tps_CycleType"]);
            
            string sqlquery = @"INSERT INTO T_PaySchedule 
                                                (Tps_PayCycle, 
                                                 Tps_StartCycle,
                                                 Tps_EndCycle,
                                                 Tps_CycleIndicator,
                                                 Tps_RecordStatus,
                                                 Usr_Login, 
                                                 Ludatetime,
                                                 Tps_Remarks,
                                                 Tps_ComputeTax,
                                                 Tps_ComputeSSS,
                                                 Tps_ComputePH,
                                                 Tps_ComputePagIbig,
                                                 Tps_MonthEnd,
                                                 Tps_TaxSchedule,
                                                 Tps_CycleIndicatorSpecial,
                                                 Tps_CycleType) 
                                               VALUES
                                                (@payrollPeriod, 
                                                GETDATE(),
                                                 GETDATE(),
                                                 @indicator,
                                                 'A',
                                                 @Usr_Login, 
                                                 GetDate(),
                                                 @Tps_Remarks,
                                                 @Tps_ComputeTax,
                                                 0,
                                                 0,
                                                 0,
                                                 0,
                                                 @Tps_TaxSchedule,
                                                 @Tps_CycleIndicatorSpecial,
                                                 @Tps_CycleType)";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }


        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[11];
            paramInfo[0] = new ParameterInfo("@payrollPeriod", row["Tps_PayCycle"], SqlDbType.Char, 7);
            paramInfo[1] = new ParameterInfo("@indicator", row["Tps_CycleIndicator"], SqlDbType.Char, 1);
            paramInfo[2] = new ParameterInfo("@status", row["Tps_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[3] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);
            paramInfo[4] = new ParameterInfo("@Tps_Remarks", row["Tps_Remarks"], SqlDbType.VarChar, 200);
            paramInfo[5] = new ParameterInfo("@Tps_ComputeTax", row["Tps_ComputeTax"]);
            paramInfo[6] = new ParameterInfo("@Tps_TaxSchedule", row["Tps_TaxSchedule"]);
            paramInfo[7] = new ParameterInfo("@Tps_CycleIndicatorSpecial", row["Tps_CycleIndicatorSpecial"]);
            paramInfo[8] = new ParameterInfo("@Tps_CycleType", row["Tps_CycleType"]);
            paramInfo[9] = new ParameterInfo("@Tps_TaxComputation", row["Tps_TaxComputation"]);
            paramInfo[10] = new ParameterInfo("@Tps_PayDate", row["Tps_PayDate"], SqlDbType.Date);

            string sqlquery = @"UPDATE T_PaySchedule 
                                               SET  
                                                 Tps_StartCycle = GETDATE(),
                                                 Tps_EndCycle = GETDATE(),
                                                 Tps_CycleIndicator = @indicator,
                                                 Tps_RecordStatus = @status,
                                                 Usr_Login = @Usr_Login, 
                                                 Ludatetime = GetDate(),
                                                 Tps_Remarks = @Tps_Remarks,
                                                 Tps_ComputeTax =@Tps_ComputeTax,
                                                 Tps_ComputeSSS =0,
                                                 Tps_ComputePH =0,
                                                 Tps_ComputePagIbig =0,
                                                 Tps_MonthEnd =0,
                                                 Tps_TaxSchedule =@Tps_TaxSchedule,
                                                 Tps_CycleIndicatorSpecial =@Tps_CycleIndicatorSpecial,
                                                 Tps_CycleType = @Tps_CycleType,
                                                 Tps_TaxComputation = @Tps_TaxComputation,
                                                 Tps_PayDate = @Tps_PayDate
                                                WHERE Tps_PayCycle = @payrollPeriod";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public override int Delete(string code, string userLogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@payrollPeriod", code, SqlDbType.Char, 7);
            paramInfo[1] = new ParameterInfo("@Usr_Login", userLogin, SqlDbType.Char, 15);


            string sqlquery = "UPDATE T_PaySchedule SET Tps_RecordStatus = 'C', Usr_Login = @Usr_Login, Ludatetime = GetDate() WHERE Tps_PayCycle = @payrollPeriod";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public override System.Data.DataSet FetchAll()
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT convert(nvarchar(50),Tps_PayCycle) as PayrollPeriod
                , convert(char(10),Tps_StartCycle,101) as StartCycle
                , convert(char(10),Tps_EndCycle,101) as EndCycle
                , Tps_CycleIndicator as 'Indicator'
                , A.Mcd_Name as 'IndicatorDesc'
                , Tps_CycleIndicatorSpecial as 'CycleIndicatorSpecial'
                , C.Mcd_Name as 'CycleIndicatorSpecialDesc'
                , Tps_CycleType as 'CycleType'
                , D.Mcd_Name as 'CycleTypeDesc'
                , Tps_RecordStatus as Status
                , Tps_Remarks
                , T_PaySchedule.Usr_Login 
                , T_PaySchedule.Ludatetime
                , Tps_TaxSchedule as 'TaxSchedule'
                , B.Mcd_Name as 'Tps_TaxSchedule'
                , CASE WHEN Tps_ComputeTax = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Tps_ComputeTax'
                , CASE WHEN Tps_ComputeSSS = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Tps_ComputeSSS'
                , CASE WHEN Tps_ComputePH = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Tps_ComputePH'
                , CASE WHEN Tps_ComputePagIbig = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Tps_ComputePagIbig'
                , CASE WHEN Tps_MonthEnd = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Tps_MonthEnd'
                FROM T_PaySchedule
                LEFT JOIN M_CodeDtl A on  A.Mcd_CodeType = 'CYCLEINDIC' 
					and A.Mcd_Code = Tps_CycleIndicator
                LEFT JOIN M_CodeDtl B on  B.Mcd_CodeType = 'TAXSCHED'
					and B.Mcd_Code = Tps_TaxSchedule
                LEFT JOIN M_CodeDtl C on  C.Mcd_CodeType = 'CYCLEINDIC' 
					and C.Mcd_Code = Tps_CycleIndicatorSpecial
                LEFT JOIN M_CodeDtl D on  D.Mcd_CodeType = 'CYCLETYPE' 
					and D.Mcd_Code = Tps_CycleType
                ORDER BY Tps_PayCycle DESC";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@payrollPeriod", code, SqlDbType.Char, 7);

            string sqlquery = @"SELECT Tps_PayCycle as PayrollPeriod
                                             , CONVERT(CHAR(10), Tps_StartCycle, 101) as StartCycle
                                             , CONVERT(CHAR(10),Tps_EndCycle, 101) as EndCycle
                                             , Tps_CycleIndicator as Indicator
                                             , Usr_Login 
                                             , Ludatetime 
                                            FROM T_PaySchedule
                                            WHERE Tps_PayCycle = @payrollPeriod";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        
        
       
        

       

       
        
        
        

        

        

        
    }
}
