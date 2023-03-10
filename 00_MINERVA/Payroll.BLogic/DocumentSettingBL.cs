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
    public class DocumentSettingBL : BaseBL
    {
        #region <Override Functions>

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

        #region <Defined Functions>
        public System.Data.DataSet FetchAllDocTags(string DocumentCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"
                            SELECT cs.Data AS [Mdt_TagCode]
                            , Mdt_TagName
                            , Mdt_TagDisplay
                            , Mdt_ResultSet
                            , Mdt_ResultScript
                            , Mdt_TagGroup
                            FROM M_DocumentTemplate A
                            CROSS APPLY dbo.Udf_Split(A.Mdt_TagCode, '|') cs
                            INNER JOIN M_DocumentTag B ON RTRIM(cs.Data) = RTRIM(B.Mdt_TagCode)
	                            AND A.Mdt_CompanyCode = B.Mdt_CompanyCode
                            WHERE  Mdt_DocumentCode = @DocumentCode
	                            AND A.Mdt_CompanyCode = @CompanyCode";
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    ParameterInfo[] Param = new ParameterInfo[2];
                    Param[0] = new ParameterInfo("@DocumentCode", DocumentCode);
                    Param[1] = new ParameterInfo("@CompanyCode", CompanyCode);
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text,Param);
                }
                catch (Exception Error)
                {
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(Error);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        
        public string GetColumnValue(string sqlScript)
        {

            DataSet ds = new DataSet();
            string value = string.Empty;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlScript, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                value =  ds.Tables[0].Rows[0]["RESULT"].ToString().Trim();

            return value;
        }

        public string GetFormulaQueryStringValue(string query, ParameterInfo[] paramInfo)
        {
            if (query == string.Empty)
                return string.Empty;

            string sValue = string.Empty;
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0)
            {
                sValue = GetValue(dtResult.Rows[0][0]);
            }
            return sValue;
        }

        public string GetColumnValue(string sqlScript, ParameterInfo[] paramInfo)
        {
            DataSet ds = new DataSet();
            string value = string.Empty;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlScript, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                value = ds.Tables[0].Rows[0]["RESULT"].ToString().Trim();

            return value;
        }

        public System.Byte[] FetchDocument(string DocumentCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT Mdt_TagCode
                                    , Mdt_DocumentTemplate
                                FROM M_DocumentType
                                WHERE Mdt_DocumentCode = @DocumentCode
                                    AND Mdt_CompanyCode = @CompanyCode";
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    ParameterInfo[] Param = new ParameterInfo[2];
                    Param[0] = new ParameterInfo("@DocumentCode", DocumentCode);
                    Param[1] = new ParameterInfo("@CompanyCode", CompanyCode);

                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, Param);
                }
                catch (Exception Error)
                {
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(Error);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (ds.Tables[0].Rows.Count > 0)
                return (byte[])ds.Tables[0].Rows[0]["Mdt_Template"];
            else
                return null;
        }

        public DataTable FetchDocumentData(string DocumentCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT Mdt_TagCode AS [TagCode]
                                    , Mdt_DocumentTemplate AS [DocumentTemplate]
                                FROM M_DocumentTemplate
                                WHERE Mdt_DocumentCode = @DocumentCode
                                    AND Mdt_CompanyCode = @CompanyCode";
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    ParameterInfo[] Param = new ParameterInfo[2];
                    Param[0] = new ParameterInfo("@DocumentCode", DocumentCode);
                    Param[1] = new ParameterInfo("@CompanyCode", CompanyCode);

                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, Param);
                }
                catch (Exception Error)
                {
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(Error);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }
        #endregion
    }
}
