using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using System.Data;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class FormulaGeneratorBL : BaseBL
    {
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

        public DataSet LoadFormulaDetails(string menuCode)
        {
            string query = string.Format(@"SELECT 
	                                            Mdm_SubCode as 'Sub Code'
	                                            , Mdm_Name as 'Description'
	                                            , Mdm_Formula as 'Formula' 
                                                , Mdm_UpdatedBy as 'Updated By'
                                                , Mdm_UpdatedDate as 'Updated Date'
                                            FROM M_Formula
                                            WHERE Mdm_MainCode = '{0}'
                                                AND Mdm_RecordStatus = 'A'", menuCode);

            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            return ds;
        }

        public int IfTableExistsinFormula(string SourceDatabase, string table)
        {
            int Result;
            string query = string.Format(@"IF EXISTS (SELECT * FROM {0}..sysobjects WHERE xtype = 'U' AND name = '{1}')
                                        BEGIN
                                        SELECT 1 as 'Result'
                                        END
                                        ELSE
                                        BEGIN
                                        SELECT 0 as 'Result'
                                        END", SourceDatabase, table);

            DataSet ds;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            return Result = int.Parse(ds.Tables[0].Rows[0][0].ToString());
        }

        public DataSet GetSubFormulas(string FormulaCode)
        {
            string query = string.Format(@"DECLARE @TABLE table
                                                            (
	                                                            SubCode varchar (10),
	                                                            ColumnCount int IDENTITY(1,1)
                                                            )

                                                            INSERT INTO @TABLE (SubCode)
                                                            SELECT Mdm_SubCode FROM M_Formula
                                                            WHERE Mdm_MainCode = '{0}'

                                                            SELECT 
                                                            ColumnCount,
                                                            Mdm_Formula as Formula
                                                            FROM M_Formula
                                                            INNER JOIN @TABLE
                                                            ON Mdm_SubCode = SubCode 
                                                            where Mdm_MainCode = '{0}'", FormulaCode);

            DataSet ds;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            return ds;
        }

        public int SubFormulaCount(string FormulaCode)
        {
            string query = string.Format(@"DECLARE @TABLE table
                                                            (
	                                                            SubCode varchar (10),
	                                                            ColumnCount int IDENTITY(1,1)
                                                            )

                                                            INSERT INTO @TABLE (SubCode)
                                                            SELECT Mdm_SubCode FROM M_Formula
                                                            WHERE Mdm_MainCode = '{0}'

                                                            SELECT 
                                                            ColumnCount,
                                                            Mdm_Formula as Formula
                                                            FROM M_Formula
                                                            INNER JOIN @TABLE
                                                            ON Mdm_SubCode = SubCode 
                                                            where Mdm_MainCode = '{0}'", FormulaCode);

            DataSet ds;
            int FormulaCount;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            FormulaCount = ds.Tables[0].Rows.Count;

            return FormulaCount;
        }

        public int DeleteRecord(string MainCode, string SubCode)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mdm_MainCode", MainCode);
            paramInfo[1] = new ParameterInfo("@Mdm_SubCode", SubCode);

            string qstring = @"DELETE FROM M_Formula
	                                    WHERE Mdm_MainCode = @Mdm_MainCode
		                                    AND Mdm_SubCode = @Mdm_SubCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qstring, CommandType.Text, paramInfo);
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
    }
}
