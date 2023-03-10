using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;

namespace Payroll.BLogic
{
    public class CommonBL : BaseBL
    {
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

        //If error found it returns true

        public string GetNumericValue(string Mph_PolicyCode)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"Select Mph_NumValue From M_PolicyHdr
                                Where Mph_PolicyCode = @Mph_PolicyCode
                                    And Mph_RecordStatus = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", Mph_PolicyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                throw (new Exception("Parameter " + Mph_PolicyCode + " is not yet setup."));
        }


        public string GetSystemID(string Smc_MenuCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Smc_MenuCode", Smc_MenuCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("Select Smc_SystemID From T_SystemMenu Where Smc_MenuCode = @Smc_MenuCode", CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public int GetCostCenterAccess(string SystemID)
        {
            object retVal = 0;
            
            string qString = @"SELECT ISNULL(COUNT(Uca_CostCenterCode),0)
                                        FROM T_UserCostCenterAccess
                                        WHERE Uca_SytemID = @SystemID
                                        AND Uca_Usercode = @Uca_Usercode
                                        AND Uca_Status = 'A'
                                        AND Uca_CostCenterCode = 'ALL'";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@SystemID", SystemID);
            paramInfo[1] = new ParameterInfo("@Uca_Usercode", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                retVal = dal.ExecuteScalar(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (Convert.ToInt32(retVal) > 0)
                return 1;
            else
                return 0;
        }

        public string CostCenterLookupStatement(string Smc_MenuCode)
        {
            string qString = string.Empty;
            string[] varparams = new string[3];
            string SystemID = this.GetSystemID(Smc_MenuCode);

            varparams[0] = SystemID;
            varparams[1] = LoginInfo.getUser().UserCode;
            varparams[2] = this.GetCostCenterAccess(SystemID).ToString();

            qString = string.Format(CommonConstants.Queries.SelectCostCenter, varparams);
            
            return qString; 
        }

        public string IDNumberLookupStatement(string Smc_MenuCode)
        {
            string confi;

            confi = ConfigurationManager.AppSettings["Confidential"].ToString();

            string qString = string.Empty;
            string[] varparams = new string[4];
            string SystemID = this.GetSystemID(Smc_MenuCode);

            varparams[0] = confi;
            varparams[1] = SystemID;
            varparams[2] = LoginInfo.getUser().UserCode;
            varparams[3] = this.GetCostCenterAccess(SystemID).ToString();
            
            qString = string.Format(CommonConstants.Queries.SelectIDNumber, varparams);

            return qString; 
        }

        public string[] GetIDNumberArgs()
        {
            string[] args = new string[7];
            args[0] = "ID Number";
            args[1] = "Last Name";
            args[2] = "First Name";
            args[3] = "MI";
            args[4] = "Job Status";
            args[5] = "CostCenter Code";
            args[6] = "ID Code";

            return args;
        }

        public string[] GetIDArgs()
        {
            string[] args = new string[4];
            args[0] = "ID Number";
            args[1] = "Last Name";
            args[2] = "First Name";
            args[3] = "MI";

            return args;
        }

        public string[] GetCostCenterArgs()
        {
            string[] args = new string[2];
            args[0] = "CostCenter Code";
            args[1] = "Description";

            return args;
        }


        private string SQLSelectEmpID()
        {
            return @"SELECT  Emt_EmployeeID as [ID Number]
                    , Emt_OldEmployeeID as [Old ID Number]
                    , Emt_NickName as [Nick Name]
             , Emt_LastName as [Last Name]
             , Emt_FirstName  as [First Name]
             , Left(Emt_MiddleName, 1) as [MI]
             , a.Adt_AccountDesc as [Position]
             , b.Adt_AccountDesc as [Job Status]
             FROM T_EmployeeMaster
             LEFT JOIN T_AccountDetail a on a.Adt_AccountCode =  Emt_PositionCode
              and a.Adt_AccountType = 'POSITION'
            LEFT JOIN t_accountdetail b on b.adt_accountcode = Emt_JobStatus
             and b.adt_accounttype='JOBSTATUS'";
        }

        private void CheckIfRecordExistsInNonConfi(string IDNumber, DataSet NonConfids)
        {
            for (int i = 0; i < NonConfids.Tables[0].Rows.Count; i++)
            {
                if (IDNumber.Trim().Equals(NonConfids.Tables[0].Rows[i]["ID Number"].ToString().Trim()))
                {
                    NonConfids.Tables[0].Rows.RemoveAt(i);
                    break;
                    //return true;
                }
            }

            //return false;
        }


        private void CheckIfRecordExistsInNonConfiExID(string IDNumber, DataSet NonConfids)
        {
            for (int i = 0; i < NonConfids.Tables[0].Rows.Count; i++)
            {
                if (IDNumber.Trim().Equals(NonConfids.Tables[0].Rows[i]["ID Number"].ToString().Trim()))
                {
                    NonConfids.Tables[0].Rows.RemoveAt(i);
                    break;
                    //return true;
                }
            }

            //return false;
        }

        private string SQLSelectEmpIDUserMaster()
        {
            return @"SELECT  Emt_EmployeeID as [ID Number]
                 , Emt_OldEmployeeID as [Old ID Number]
                 , Emt_NickName as [Nick Name]
                 , Emt_LastName as [Last Name]
                 , Emt_FirstName  as [First Name]
                 , Left(Emt_MiddleName, 1) as [MI]
              , a.Adt_AccountDesc as [Position]
              , b.Adt_AccountDesc as [Job Status]
            FROM T_EmployeeMaster
             LEFT JOIN T_AccountDetail a on a.Adt_AccountCode =  Emt_PositionCode
              and a.Adt_AccountType = 'POSITION'
            LEFT JOIN t_accountdetail b on b.adt_accountcode = Emt_JobStatus
             and b.adt_accounttype='JOBSTATUS'
            Where Emt_EmployeeID not in (Select Mur_UserCode From M_User)";
        }

        private DataSet GetConfiRecordsUserMaster()//For Confi
        {
            DataSet ds = new DataSet();

            string sqlstatement = this.SQLSelectEmpIDUserMaster();

            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlstatement, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }


        private string PadIDNumber(string txtBoxName, string PADIDZERO)
        {
            if (Convert.ToBoolean(PADIDZERO))
            {
                string retval = this.GetNumericValue("PADZEROCNT");
                double PADZEROCNT;
                if (retval.Trim() == string.Empty)
                    retval = "0";

                PADZEROCNT = Convert.ToDouble(retval);

                if (txtBoxName.Trim() != string.Empty)
                {
                    if (Convert.ToDouble(txtBoxName.Trim().Length) < PADZEROCNT)
                    {
                        double ZeroTobeAdded = PADZEROCNT - Convert.ToDouble(txtBoxName.Trim().Length);

                        for (double i = 0; i < ZeroTobeAdded; i++)
                            txtBoxName = "0" + txtBoxName.Trim();
                    }
                }
            }
            return txtBoxName;
        }

        private DataTable GetOrigValuesInDB(string condition, string tablename)
        {
            DataSet ds = new DataSet();
            string[] param = new string[2];
            param[0] = tablename;
            param[1] = condition;

            string qString = string.Format("SELECT * FROM {0} {1}", param);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds.Tables[0];
        }

        public string GetServerDateTime()
        {
            string dateTime = string.Empty;

            #region query

            string qString = @"Select Getdate()";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dateTime = dal.ExecuteScalar(qString, CommandType.Text).ToString();
                }
                catch (Exception e)
                {
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return dateTime;
        }

        #region ForExcel Generics
        public string[] GetProfileCompanyForExcel()
        {
            DataTable dt;
            string query = @"
                SELECT Ccd_CompanyCode
	                ,Ccd_CompanyName  
	                ,Ccd_CompanyAddress1  
	                ,Ccd_CompanyAddress2+' '+Adt_Accountdesc 'Ccd_CompanyAddress2'
	                ,Ccd_EmailAddress 
	                ,Ccd_TelephoneNo 
	                ,Ccd_CellNo 
	                ,Ccd_FaxNo
                FROM T_CompanyMaster
                INNER JOIN T_AccountDetail 
				ON Ccd_CompanyAddress3 = Adt_AccountCode and Adt_AccountType = 'zipcode'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            string contact = string.Format(@"Phone No: {0} | Fax No: {1} | Email Add: {2}",
                                  dt.Rows[0]["Ccd_TelephoneNo"].ToString(), dt.Rows[0]["Ccd_FaxNo"].ToString(),
                                  dt.Rows[0]["Ccd_EmailAddress"].ToString());

            string[] comp = new string[4];
            comp[0] = dt.Rows[0]["Ccd_CompanyName"].ToString();
            comp[1] = dt.Rows[0]["Ccd_CompanyAddress1"].ToString();
            comp[2] = dt.Rows[0]["Ccd_CompanyAddress2"].ToString();
            comp[3] = contact;

            return comp;
        }
        #endregion

        public string GetControlNumber(string transactionCode)
        {
            string controlNum = "";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    controlNum = GetControlNumber(transactionCode, dal);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return controlNum;
        }

        public string GetControlNumber(string transactionCode, DALHelper dal)
        {
            string controlNum = "";

            string sqlControlNoFetch = @"   DECLARE @yr2Digit varchar(2)
                                            SET @yr2Digit = (SELECT right(Ccd_CurrentYear, 2) from T_CompanyMaster)


                                            SELECT Tcm_TransactionPrefix 
	                                            + @yr2Digit 
	                                            + replicate('0', 9 - len(RTrim(Tcm_LastSeries)))
	                                            + RTrim(Tcm_LastSeries)
                                            FROM T_TransactionControlMaster
                                            WITH (UPDLOCK)
                                            WHERE Tcm_TransactionCode = '{0}'";

            string sqlControlNoUpdate = @"  UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = '{0}'";

            dal.ExecuteNonQuery(string.Format(sqlControlNoUpdate, transactionCode.ToUpper()), CommandType.Text);
            controlNum = Convert.ToString(dal.ExecuteScalar(string.Format(sqlControlNoFetch, transactionCode.ToUpper()), CommandType.Text));

            return controlNum;
        }
        
       
        public string[] EncodeFilterItems(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            return strArrFilterItems;
        }

        public DataSet getAccessRightsforMenu(string userLogged, string menuCode, string companyCode)
        {
            DataSet dsrights = null;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dsrights = dal.ExecuteDataSet(
                            string.Format(
                            @"
                                SELECT 
                                    Mra_CanAppend
	                                ,Mra_CanUpdate
	                                ,Mra_CanDelete
	                                ,Mra_CanProcess
	                                ,Mra_CanPrint
	                                ,Mra_CanApprove
	                                ,Mra_CanReprint
                                    ,Mra_CanView
                                FROM M_UserRoleAccess
                                LEFT JOIN M_UserDtl
                                ON Mra_SystemCode = Mud_SystemCode
                                AND Mra_UserRoleCode = Mud_UserRoleCode
                                WHERE Mud_UserCode = '{1}'
                                AND Mra_ModuleCode = '{0}'
                                AND Mra_CompanyCode = '{2}'
                            ", menuCode, userLogged, companyCode)
                            , CommandType.Text
                            );

                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                    dsrights = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsrights;

        }

        public static Font GetFont(bool bRegular)
        {
            Font font;
            if (bRegular)
                font = new Font("Tahoma", 8.25F, FontStyle.Regular);
            else
                font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            return font;
        }

        public DataSet GetCompanyInfoHeader(string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"
                           SELECT Mcm_CompanyName 		
                             , Mcm_BusinessAddress AS [Address]			
                             , LTRIM(CASE WHEN LEN(Mcm_EmailAddress) = 0 THEN '' 
							 ELSE 'Email Address: ' + Mcm_EmailAddress END + 
							 CASE WHEN LEN(Mcm_telno) = 0 THEN '' ELSE ' Telephone Number: ' + Mcm_telno END 			
                             + CASE WHEN LEN(Mcm_cellno) = 0 THEN '' 
							 ELSE ' Cellular Number: ' + Mcm_cellno END + 
							 CASE WHEN LEN(Mcm_faxno) = 0 THEN '' ELSE ' Fax Number: ' + Mcm_faxno END) as Contacts			
                             , Mcm_CompanyLogo	
                            FROM M_Company			
                            WHERE Mcm_companycode = '{0}'			
                            AND Mcm_RecordStatus = 'A'", CompanyCode);
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }
            return ds;
        }

        public string GetParameterFormulaFromCentral(string PolicyCode, string CompanyCode)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_Formula 
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper("", true))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Mph_Formula"].ToString();
            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetParameterFormulaFromCentral(string PolicyCode, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = string.Format(@"SELECT Mph_Formula 
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'");
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Mph_Formula"].ToString();
            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetFormulaQueryStringValue(string query, ParameterInfo[] paramInfo, DALHelper dal)
        {
            if (query == string.Empty)
                return string.Empty;

            string sValue = string.Empty;
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                sValue = GetValue(dtResult.Rows[0][0]);
            }
            return sValue;
        }
    }
}
