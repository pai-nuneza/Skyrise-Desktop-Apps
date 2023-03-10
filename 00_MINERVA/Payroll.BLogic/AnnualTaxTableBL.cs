using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class AnnualTaxTableBL : BaseBL
    {
        public AnnualTaxTableBL()
        {

        }
        public override DataSet FetchAll()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT  Myt_MinCompensationLevel,
	                                                  Myt_MaxCompensationLevel,
	                                                  Myt_TaxOnExcess,nnn
	                                                  Myt_TaxOnCompensationLevel,
	                                                  Myt_ExcessOverCompensationLevel,
                                                      convert(varchar(50), Myt_MinCompensationLevel) as 'MinBracketAmtSearch',
	                                                  convert(varchar(50), Myt_MaxCompensationLevel) as 'MaxBracketAmtSearch', 
	                                                  convert(varchar(50), Myt_TaxOnExcess) as 'FixedAmtSearch',
	                                                  convert(varchar(50), Myt_TaxOnCompensationLevel) as 'TaxRateSearch', 
	                                                  convert(varchar(50), Myt_ExcessOverCompensationLevel) as 'ExcessOverAmtSearch',
	                                                  Myt_RecordStatus,
	                                                  Usr_Login,
	                                                  Ludatetime                                                        
                                               FROM M_YearlyTaxSchedule";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override int Add(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[9];
            paramInfo[0] = new ParameterInfo("@MinBracketAmt", row["Myt_MinCompensationLevel"], SqlDbType.Decimal, 9);
            paramInfo[1] = new ParameterInfo("@MaxBracketAmt", row["Myt_MaxCompensationLevel"], SqlDbType.Decimal, 9);
            paramInfo[2] = new ParameterInfo("@FixedAmt", row["Myt_TaxOnExcess"], SqlDbType.Decimal, 9);
            paramInfo[3] = new ParameterInfo("@TaxRate", row["Myt_TaxOnCompensationLevel"], SqlDbType.Decimal, 6);
            paramInfo[4] = new ParameterInfo("@ExcessOverAmt", row["Myt_ExcessOverCompensationLevel"], SqlDbType.Decimal, 9);
            paramInfo[5] = new ParameterInfo("@Status", row["Myt_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[6] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);
            paramInfo[7] = new ParameterInfo("@Ludatetime", row["ludatetime"], SqlDbType.DateTime);
            paramInfo[8] = new ParameterInfo("@PayPeriod", row["Myt_PayCycle"], SqlDbType.Char, 7);

            string sqlQuery = @"INSERT M_YearlyTaxSchedule (Myt_MinCompensationLevel,
                                                                            Myt_MaxCompensationLevel,
                                                                            Myt_TaxOnExcess,	
                                                                            Myt_TaxOnCompensationLevel,
                                                                            Myt_ExcessOverCompensationLevel,
                                                                            Myt_RecordStatus,
                                                                            Usr_Login,
                                                                            Ludatetime, Myt_PayCycle)
                                                                    VALUES (@MinBracketAmt,
                                                                            @MaxBracketAmt,
                                                                            @FixedAmt,
                                                                            @TaxRate,
                                                                            @ExcessOverAmt,
                                                                            @Status,
                                                                            @Usr_Login,
                                                                            GetDate(), @PayPeriod)";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
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

        public override int Update(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[9];
            paramInfo[0] = new ParameterInfo("@MinBracketAmt", row["Myt_MinCompensationLevel"], SqlDbType.Decimal, 9);
            paramInfo[1] = new ParameterInfo("@MaxBracketAmt", row["Myt_MaxCompensationLevel"], SqlDbType.Decimal, 9);
            paramInfo[2] = new ParameterInfo("@FixedAmt", row["Myt_TaxOnExcess"], SqlDbType.Decimal, 9);
            paramInfo[3] = new ParameterInfo("@TaxRate", row["Myt_TaxOnCompensationLevel"], SqlDbType.Decimal, 6);
            paramInfo[4] = new ParameterInfo("@ExcessOverAmt", row["Myt_ExcessOverCompensationLevel"], SqlDbType.Decimal, 9);
            paramInfo[5] = new ParameterInfo("@Status", row["Myt_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[6] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);
            paramInfo[7] = new ParameterInfo("@Ludatetime", row["ludatetime"], SqlDbType.DateTime);
            paramInfo[8] = new ParameterInfo("@PayPeriod", row["Myt_PayCycle"], SqlDbType.Char, 7);

            string sqlQuery = @"UPDATE M_YearlyTaxSchedule 
                                                             SET 
                                                                 Myt_MaxCompensationLevel = @MaxBracketAmt,
                                                                 Myt_TaxOnExcess = @FixedAmt,	
                                                                 Myt_TaxOnCompensationLevel = @TaxRate,
                                                                 Myt_ExcessOverCompensationLevel = @ExcessOverAmt,
                                                                 Myt_RecordStatus = @Status,
                                                                 Usr_Login = @Usr_Login,
                                                                 Ludatetime = GetDate()
                                                            WHERE Myt_MinCompensationLevel = @MinBracketAmt
                                                            and Myt_PayCycle = @PayPeriod";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
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

        public DataSet FetchAnnualTax(string payperiod)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"SELECT  Myt_MinCompensationLevel,
	                                                  Myt_MaxCompensationLevel,
	                                                  Myt_TaxOnExcess,
	                                                  Myt_TaxOnCompensationLevel,
	                                                  Myt_ExcessOverCompensationLevel,
                                                      convert(varchar(50), Myt_MinCompensationLevel) as 'MinBracketAmtSearch',
	                                                  convert(varchar(50), Myt_MaxCompensationLevel) as 'MaxBracketAmtSearch', 
	                                                  convert(varchar(50), Myt_TaxOnCompensationLevel) as 'FixedAmtSearch',
	                                                  convert(varchar(50), Myt_TaxOnExcess) as 'TaxRateSearch', 
	                                                  convert(varchar(50), Myt_ExcessOverCompensationLevel) as 'ExcessOverAmtSearch',
	                                                  Myt_RecordStatus,
	                                                  Usr_Login,
	                                                  Ludatetime                                                        
                                               FROM M_YearlyTaxSchedule
                                               where Myt_PayCycle = @PayPeriod");
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@PayPeriod", payperiod);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataRow FetchIfExist(string minbracket, string payperiod)
        {

            string qry = @"SELECT * FROM M_YearlyTaxSchedule WHERE Myt_MinCompensationLevel = @MinBracketAmt and Myt_PayCycle = @PayPeriod";

            DataSet ds = new DataSet();
            decimal DecMinBAmt = Convert.ToDecimal(minbracket);
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@MinBracketAmt", DecMinBAmt, SqlDbType.Decimal, 9);
            paramInfo[1] = new ParameterInfo("@PayPeriod", payperiod, SqlDbType.Char, 7);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qry, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public override DataRow Fetch(string MinBracketAmt)
        {
            DataSet ds = new DataSet();
            decimal DecMinBAmt = Convert.ToDecimal(MinBracketAmt);
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@MinBracketAmt", DecMinBAmt, SqlDbType.Decimal, 9);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.checkMinBracketAmtExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int DeleteTaxTable(string MinBracket, string UsrLogin, string payperiod)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@MinBracketAmt", MinBracket, SqlDbType.Decimal, 9);
            paramInfo[1] = new ParameterInfo("@Usr_Login", UsrLogin, SqlDbType.Char, 15);
            paramInfo[2] = new ParameterInfo("@PayPeriod", payperiod, SqlDbType.Char, 7);

            string sqlQuery = "UPDATE M_YearlyTaxSchedule SET Myt_RecordStatus = 'C', Usr_Login = @Usr_Login, Ludatetime = GetDate() WHERE Myt_MinCompensationLevel = @MinBracketAmt and Myt_PayCycle = @PayPeriod";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
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

        //Reports
        public DataSet GetHeaderData(string Status, string effectivity)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"Select Mcm_CompanyName
                                   ,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ', ' + Mcd_Name as Address
                                   ,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
                                   ,Mcm_CompanyLogo
                                   ,@Status As Status, @Effectivity as Effectivity
                             From M_Company
                             Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code and Mcd_CodeType='ZIPCODE'";

            #endregion

            ParameterInfo[] param = new ParameterInfo[2];
            param[paramIndex++] = new ParameterInfo("@Status", Status);
            param[paramIndex++] = new ParameterInfo("@Effectivity", effectivity);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        //added by Kevin 20081216 - to get max payperiod
        public int FetchMaxPayPeriod()
        {
            int payperiod = 0;
            DataSet ds = new DataSet();
            string sql = @"select max(Myt_PayCycle) as [TopPayPeriod]
                    from dbo.M_YearlyTaxSchedule
                    where Myt_RecordStatus = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                payperiod = Convert.ToInt32(ds.Tables[0].Rows[0]["TopPayPeriod"].ToString());
            return payperiod;
        }

        //added by Kevin 20081216 - for posting function
        public int DoPost(string userLogin, string payperiod)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@UserLogin", userLogin);
            paramInfo[1] = new ParameterInfo("@PayPeriod", payperiod);

            string sqlQuery = @"
                    --get data base on max period
                    select Myt_MinCompensationLevel, Myt_MaxCompensationLevel, Myt_TaxOnExcess, Myt_TaxOnCompensationLevel,
                    Myt_ExcessOverCompensationLevel, Myt_RecordStatus, Usr_Login, Ludatetime, Myt_PayCycle

                    into #temp1

                    from dbo.M_YearlyTaxSchedule
                    where Myt_PayCycle = 
                    (select max(Myt_PayCycle) from dbo.M_YearlyTaxSchedule
                    where Myt_RecordStatus = 'A')

                    --update
                    update #temp1 set Myt_PayCycle = @PayPeriod, Usr_Login = @UserLogin, Ludatetime = GetDate()

                    --insert
                    insert into dbo.M_YearlyTaxSchedule
                    select Myt_MinCompensationLevel, Myt_MaxCompensationLevel, Myt_TaxOnExcess, Myt_TaxOnCompensationLevel,
                    Myt_ExcessOverCompensationLevel, Myt_RecordStatus, Usr_Login, Ludatetime, Myt_PayCycle
                    from #temp1

                    drop table #temp1
                    ";

            //spAnnualTaxTablePosting
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
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

        public DataSet GetDetailData(string Status, string payperiod)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"SELECT  convert(nvarchar(50),Myt_MinCompensationLevel) as 'Ats_MinBracketAmt',
                                      convert(varchar(50),Myt_MaxCompensationLevel) as 'Myt_MaxCompensationLevel',
                                      convert(varchar(50),Myt_TaxOnExcess) as 'Myt_TaxOnExcess',
                                      convert(int, Myt_TaxOnCompensationLevel) AS 'Myt_TaxOnCompensationLevel',
                                      convert(varchar(50),Myt_ExcessOverCompensationLevel) as Myt_ExcessOverCompensationLevel,
                                      CASE
	                                    WHEN Myt_RecordStatus = 'A' THEN 'ACTIVE'
	                                    WHEN Myt_RecordStatus = 'C' THEN 'CANCELLED'
	                                  END AS Myt_RecordStatus                                                                                                        
                                    FROM M_YearlyTaxSchedule
                                              WHERE (Myt_RecordStatus = @status
                                                                OR @status = 'ALL')
                                                    AND Myt_PayCycle = @PayPeriod";

            #endregion

            ParameterInfo[] param = new ParameterInfo[2];
            param[paramIndex++] = new ParameterInfo("@Status", Status);
            param[paramIndex++] = new ParameterInfo("@PayPeriod", payperiod);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPreviousBracketAmount(string Myt_MinCompensationLevel, string Myt_PayCycle)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"SELECT Myt_TaxOnExcess   
                                               , Myt_TaxOnCompensationLevel     
                                               , Myt_ExcessOverCompensationLevel                                             
                                               FROM M_YearlyTaxSchedule
                                               where Myt_MinCompensationLevel < @Myt_MinCompensationLevel
                                               and Myt_PayCycle = @Myt_PayCycle
                                               ORDER BY Myt_MinCompensationLevel DESC");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Myt_MinCompensationLevel", Myt_MinCompensationLevel);
            paramInfo[1] = new ParameterInfo("@Myt_PayCycle", Myt_PayCycle);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetNextBracketAmount(string Myt_MinCompensationLevel, string Myt_PayCycle)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"SELECT Myt_TaxOnExcess   
                                               , Myt_TaxOnCompensationLevel     
                                               , Myt_ExcessOverCompensationLevel                                             
                                               FROM M_YearlyTaxSchedule
                                               where Myt_MinCompensationLevel > @Myt_MinCompensationLevel
                                               and Myt_PayCycle = @Myt_PayCycle
                                               ORDER BY Myt_MinCompensationLevel ASC");
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Myt_MinCompensationLevel", Myt_MinCompensationLevel);
            paramInfo[1] = new ParameterInfo("@Myt_PayCycle", Myt_PayCycle);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetMaxMinMaxBracketAmount(string Myt_PayCycle)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"SELECT max(Myt_MaxCompensationLevel)
                                               FROM M_YearlyTaxSchedule
                                        WHERE Myt_PayCycle = @Myt_PayCycle
                                            AND Myt_RecordStatus = 'A'");
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Myt_PayCycle", Myt_PayCycle);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }
    }
}
