using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class CycleOpeningBL : BaseBL
    {
        #region Override Functions

        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
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

        #endregion

        #region <Pre-Checking and retrieving of needed data.>

        public DataSet GetNextPayPeriod()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"DECLARE @EndDate as Datetime
                              Set @EndDate = (Select Tps_EndCycle From T_PaySchedule
                              Where Tps_CycleIndicator = 'C' and Tps_RecordStatus = 'A')

                              SELECT Tps_PayCycle as NEXTPAYPERIOD, Convert(char(10), Tps_StartCycle, 101) Tps_StartCycle, Convert(char(10), Tps_EndCycle, 101) Tps_EndCycle
                              FROM T_PaySchedule
                              Where Tps_StartCycle = dateadd(dd,1,@EndDate)
                              And Tps_CycleIndicator <> 'S'
                              And Tps_RecordStatus = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetNext2PayPeriod(DALHelper dal)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"DECLARE @EndDate as Datetime
                                SET @EndDate = (SELECT TOP 1 Tps_EndCycle FROM T_PaySchedule
                                WHERE Tps_CycleIndicator = 'F' AND Tps_RecordStatus = 'A' 
                                ORDER BY Tps_EndCycle ASC)

                                SELECT Tps_PayCycle AS NEXTPAYPERIOD
                                , CONVERT(char(10), Tps_StartCycle, 101) Tps_StartCycle
                                , CONVERT(char(10), Tps_EndCycle, 101) Tps_EndCycle
                                FROM T_PaySchedule
                                WHERE Tps_StartCycle = dateadd(dd,1,@EndDate)
                                AND Tps_CycleIndicator <> 'S'
                                AND Tps_RecordStatus = 'A'";


            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
            return ds;
        }

        public DataSet GetCurrentPayPeriod()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT Tps_PayCycle, Convert(char(10), Tps_StartCycle, 101) Tps_StartCycle, Convert(char(10), Tps_EndCycle, 101) Tps_EndCycle
                                              FROM T_PaySchedule
                                              Where Tps_CycleIndicator = 'C'
                                              And Tps_RecordStatus = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            
            return ds;
        }

        public DataTable GetTaxMaster(string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            #region query
            string query = string.Format(@"
                               SELECT UPPER('DEDUCT ON ' + (SELECT Mcd_Name  + ', ' as 'data()' FROM {1}.dbo.Udf_Split(Mtx_DeductionPayCycle,',')
									LEFT JOIN {1}..M_CodeDtl on Mcd_Code = data
									 AND Mcd_Codetype = 'PAYFREQ' 
                                     AND Mcd_CompanyCode  = '{0}'
									FOR XML PATH('')) + Mcd_name
									+ CASE WHEN Mtx_ReferPreviousIncomeTax = 1 THEN ', Refer to Previous Base and W/Tax Amount' ELSE '' END)
								    AS [Pay Cycle to deduct] 
                                    , Mtx_ReferPreviousIncomeTax
                                    , Mtx_TaxComputation
                                    , Mtx_DeductionPayCycle
								FROM  {1}..M_Tax
								LEFT JOIN {1}..M_CodeDtl 
                                    ON Mtx_TaxComputation = Mcd_Code
                                    AND Mtx_CompanyCode = Mcd_CompanyCode
								    AND Mcd_CodeType = 'TAXBASE'
                                WHERE Mtx_CompanyCode = '{0}'", CompanyCode, CentralProfile);
            #endregion
            DataTable dt = dalhelper.ExecuteDataSet(query, CommandType.Text).Tables[0];
            return dt;
        }

        public DataTable GetGovernmentMaster(string RemittanceCode, string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            #region query
            string query = string.Format(@"
                               SELECT Mgr_DeductionPayCycle FROM {2}..M_GovRemittance
                                WHERE Mgr_CompanyCode = '{0}'
                                    AND Mgr_RemittanceCode = '{1}'", CompanyCode, RemittanceCode, CentralProfile);
            #endregion
            DataTable dt = dalhelper.ExecuteDataSet(query, CommandType.Text).Tables[0];
            return dt;
        }

        #endregion

        #region <Start of Process>

        DALHelper dalTest = new DALHelper();

        #endregion


        public void InitializeDALHelper()
        {
            dalTest.OpenDB();
            dalTest.BeginTransactionSnapshot();
        }

        public void CommitDALHelper()
        {
            dalTest.CommitTransactionSnapshot();
            dalTest.CloseDB();
        }

        public void RollBackDALHelper()
        {
            try
            {
                dalTest.RollBackTransactionSnapshot();
                dalTest.CloseDB();
            }
            catch
            { }
        }

        public bool IsPayrollCalcUnclosed()
        {
            DataSet ds;

            string sqlQuery = @"SELECT COUNT(Tpy_IDNo) FROM T_EmpPayroll
                                WHERE Tpy_PayCycle != 
                                    (SELECT Tps_PayCycle FROM T_PaySchedule WHERE Tps_CycleIndicator = 'C')";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()) > 0)
                return true;
            else return false;
        }
    }
}
