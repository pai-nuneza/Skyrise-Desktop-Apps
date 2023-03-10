using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Data;
using Payroll.DAL;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;
using CommonLibrary;
using System.Globalization;

namespace Payroll.BLogic
{
    public class CommonBL : BaseBL
    {
        private static decimal _HOURSINDAY = 0;

        public static decimal HOURSINDAY
        {
            get
            {
                if (_HOURSINDAY == 0)
                {
                    _HOURSINDAY = GetHoursInDay();
                }
                return _HOURSINDAY;
            }
            set
            {
                _HOURSINDAY = value;
            }
        }

        public static decimal GetHoursInDay()
        {
            string Mph_PolicyCode = "HDIVISOR";
            DataSet ds = new DataSet();

            #region qurey
            string qString = string.Format(@"
                                SELECT Mph_NumValue 
                                FROM M_PolicyHdr
                                WHERE Mph_PolicyCode = @Mph_PolicyCode
                                    AND Mph_RecordStatus = 'A'");
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
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            else
                throw (new Exception("Policy " + Mph_PolicyCode + " is not yet setup."));
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

        public static string RegionShortDateFormat()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        public static string GetNameDisplay()
        {
            return string.Format(@", CASE WHEN @NAMEDSPLY = 'C' THEN Mem_Lastname + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_Firstname + ' ' + Mem_Middlename
	                                            WHEN @NAMEDSPLY = 'F' THEN Mem_Firstname
	                                            WHEN @NAMEDSPLY = 'M' THEN Mem_Lastname + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_Firstname + ' ' + CASE WHEN LEN(RTRIM(Mem_Middlename)) = 0 THEN '' ELSE LEFT(Mem_Middlename, 1) + '.' END
	                                            WHEN @NAMEDSPLY = 'S' THEN Mem_NickName
	                                            WHEN @NAMEDSPLY = 'X' THEN Mem_Firstname + ' ' + CASE WHEN LEN(RTRIM(Mem_Middlename)) = 0 THEN ' ' ELSE LEFT(Mem_Middlename, 1) + '. ' END +  Mem_Lastname + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 THEN  ' ' + Mem_ExtensionName ELSE '' END
	                                            ELSE ' ' END as [Name] 
                                  ");
        }

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

        public bool CanViewSalary(string menuCode, string OldSalaryRate)
        {
            bool CanAccess = false;
            if (LoginInfo.getViewRate().CanViewRate)
            {
                LoginInfoRecord user = LoginInfo.getUser();
                MenuRecord menu = user.getMenu(menuCode);
                try
                {
                    if (menu.CanApprove == false && Convert.ToDecimal(OldSalaryRate) > 0)
                        CanAccess = false;
                    else if (menu.CanApprove)
                        CanAccess = true;
                }
                catch
                { }

            }
            else
                CanAccess = false;
            return CanAccess;
        }

        public bool CheckProcessControlFlag(string Tsc_SystemCode, string Tsc_SettingCode)
        {
            DataSet ds = new DataSet();
            bool retval = false;
            #region query
            string qString = @"SELECT Tsc_SetFlag, Tsc_RecordStatus, Tsc_SettingName
                               FROM T_SettingControl
                               WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode
                                    AND Tsc_RecordStatus = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            paramInfo[1] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Tsc_RecordStatus"].ToString() == "C")
                {
                    CommonProcedures.showMessageError(ds.Tables[0].Rows[0]["Tsc_RecordStatus"].ToString().Trim() + " status has been cancelled.");
                    retval = true;
                }
                else
                {
                    if ((Tsc_SystemCode == "TIMEKEEP") && (Tsc_SettingCode == "CUT-OFF") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == false))
                    {
                        CommonProcedures.showMessageError("Current Cycle not yet on cut-off");
                        retval = true;
                    }
                    else if ((Tsc_SystemCode == "TIMEKEEP") && (Tsc_SettingCode == "CYCLEOPEN") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == false))
                    {
                        CommonProcedures.showMessageError("Current Cycle not open");
                        retval = true;
                    }
                    else if ((Tsc_SystemCode == "TIMEKEEP") && (Tsc_SettingCode == "CYCLECLOSE") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == true))
                    {
                        CommonProcedures.showMessageError("Current Cycle already closed");
                        retval = true;
                    }
                    else if ((Tsc_SystemCode == "PAYROLL") && (Tsc_SettingCode == "CUT-OFF") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == true))
                    {
                        CommonProcedures.showMessageError("Current Cycle not yet on cut-off");
                        retval = true;
                    }
                    else if ((Tsc_SystemCode == "PAYROLL") && (Tsc_SettingCode == "CYCLEOPEN") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == true))
                    {
                        CommonProcedures.showMessageError("Current Cycle not open");
                        retval = true;
                    }
                    else if ((Tsc_SystemCode == "PAYROLL") && (Tsc_SettingCode == "CYCLECLOSE") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == false))
                    {
                        CommonProcedures.showMessageError("Current Cycle already closed");
                        retval = true;
                    }
                    else if ((Tsc_SystemCode == "PAYROLL") && (Tsc_SettingCode == "PAYCALC") && (Convert.ToBoolean(ds.Tables[0].Rows[0]["Tsc_SetFlag"].ToString()) == true))
                    {
                        CommonProcedures.showMessageError("Payroll already calculated.");
                        retval = true;
                    }
                    else
                        retval = false;
                }
            }
            else
                retval = true;
            return retval;
        }


        public bool isPayPeriodCurrent(string payPeriod)
        {
            #region query
            string query = string.Format(@"SELECT Tps_CycleIndicator
                                           FROM T_PaySchedule
                                           WHERE Tps_PayCycle = '{0}'"
                                           , payPeriod);
            #endregion

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
            {
                DataRow drResult = dtResult.Rows[0];

                if (drResult[0].ToString() != "P")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public string GetNumericValue(string Mph_PolicyCode)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_NumValue 
                               FROM M_PolicyHdr
                               WHERE Mph_PolicyCode = @Mph_PolicyCode
                                    AND Mph_RecordStatus = 'A'";
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
                throw (new Exception("Policy " + Mph_PolicyCode + " is not yet setup."));
        }

        public string GetParameterValueFromCentral(string PolicyCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_DataType
                                    , Mph_NumValue
                                    , Mph_CharValue
                                    , Mph_Formula 
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup in Central.", CompanyCode, PolicyCode)));
        }

        public string GetParameterValueFromCentral(string PolicyCode, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = string.Format(@"SELECT Mph_DataType
                                            , Mph_NumValue
                                            , Mph_CharValue
                                            , Mph_Formula 
                                       FROM {0}..M_PolicyHdr
                                       WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                        AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                        AND RTRIM(Mph_RecordStatus) = 'A'",CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup in Central.", CompanyCode, PolicyCode)));
        }

        public string GetParameterValueFromPayroll(string PolicyCode, string CompanyCode)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_DataType
                                    , Mph_NumValue
                                    , Mph_CharValue
                                    , Mph_Formula 
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetParameterValueFromPayroll(string PolicyCode, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_DataType
                                    , Mph_NumValue
                                    , Mph_CharValue
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetParameterValueFromPayroll(string PolicyCode, string CompanyCode, string LoginDBName, DALHelper dalCentral)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = string.Format(@"SELECT Mph_DataType
                                            , Mph_NumValue
                                            , Mph_CharValue
                                            FROM {0}..M_PolicyHdr
                                            WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                                AND RTRIM(Mph_RecordStatus) = 'A'", LoginDBName);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            ds = dalCentral.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                switch (ds.Tables[0].Rows[0]["Mph_DataType"].ToString())
                {
                    case "I":
                        int i = (int)Convert.ToDecimal(GetValue(ds.Tables[0].Rows[0]["Mph_NumValue"]));
                        return i.ToString();
                    case "D":
                        return ds.Tables[0].Rows[0]["Mph_NumValue"].ToString();
                    case "B":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    case "C":
                        return ds.Tables[0].Rows[0]["Mph_CharValue"].ToString();
                    default:
                        throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
                }

            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetParameterDataTypeValueFromPayroll(string PolicyCode, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = @"SELECT Mph_DataType
                               FROM M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mph_PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@Mph_CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Mph_DataType"].ToString();
            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }

        public string GetParameterDtlValueFromCentral(string ParameterType, string ParameterDesc, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT Mpd_ParamValue 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @ParameterType 
	                                AND RTRIM(Mpd_SubCode) = @ParameterDesc
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@ParameterType", ParameterType);
            paramInfo[1] = new ParameterInfo("@ParameterDesc", ParameterDesc);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetParameterDtlValueFromCentral(string ParameterType, string ParameterDesc, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"SELECT Mpd_ParamValue 
                                            FROM {0}.dbo.M_PolicyDtl
                                            WHERE RTRIM(Mpd_PolicyCode) = @ParameterType 
	                                            AND RTRIM(Mpd_SubCode) = @ParameterDesc
                                                AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                            AND RTRIM(Mpd_RecordStatus) = 'A'", CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@ParameterType", ParameterType);
            paramInfo[1] = new ParameterInfo("@ParameterDesc", ParameterDesc);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);
            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }
        public string GetParameterDtlValueFromPayroll(string PolicyCode, string SubCode, string CompanyCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT Mpd_ParamValue 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode 
	                                AND RTRIM(Mpd_SubCode) = @SubCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@SubCode", SubCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetParameterDtlValueFromPayroll(string ParameterType, string ParameterDesc, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT Mpd_ParamValue 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @ParameterType 
	                                AND RTRIM(Mpd_SubCode) = @ParameterDesc
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@ParameterType", ParameterType);
            paramInfo[1] = new ParameterInfo("@ParameterDesc", ParameterDesc);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetParameterDtlValueFromPayroll(string ParameterType, string ParameterDesc, string CompanyCode, string LoginDBName, DALHelper dalCentral)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"SELECT Mpd_ParamValue 
                                            FROM {0}..M_PolicyDtl
                                            WHERE RTRIM(Mpd_PolicyCode) = @ParameterType 
	                                            AND RTRIM(Mpd_SubCode) = @ParameterDesc
                                                AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                            AND RTRIM(Mpd_RecordStatus) = 'A'", LoginDBName);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@ParameterType", ParameterType);
            paramInfo[1] = new ParameterInfo("@ParameterDesc", ParameterDesc);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dalCentral.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public double GetDoubleParameterDtlValueFromCentral(string PolicyCode, string CompanyCode, string SubCode, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT Mpd_ParamValue 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode 
	                                AND RTRIM(Mpd_SubCode) = @SubCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@SubCode", SubCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);
            double dLength = -1;
            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            if (ds.Tables[0].Rows.Count > 0)
                return Convert.ToDouble(ds.Tables[0].Rows[0][0].ToString());
            else
                return dLength;
        }

        public string GetParameterFormulaFromPayroll(string PolicyCode, string CompanyCode)
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

            using (DALHelper dal = new DALHelper())
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

        public string GetParameterFormulaFromCentral(string PolicyCode, string CompanyCode, string CentralProfile)
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

            using (DALHelper dal = new DALHelper(CentralProfile, false))
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

        public string GetParameterFormulaFromCentral(string PolicyCode, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region qurey
            string qString = string.Format(@"SELECT Mph_Formula 
                               FROM {0}..M_PolicyHdr
                               WHERE RTRIM(Mph_PolicyCode) = @Mph_PolicyCode
                                AND RTRIM(Mph_CompanyCode) = @Mph_CompanyCode
                                AND RTRIM(Mph_RecordStatus) = 'A'", CentralProfile);
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

        public string GetCurrentPayPeriod()
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = @"SELECT Tps_PayCycle 
                                    FROM T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                        AND Tps_RecordStatus = 'A'";

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetCurrentSpecialPayPeriod()
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = @"SELECT Tps_PayCycle 
                                    FROM T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'S' 
                                        AND Tps_CycleIndicatorSpecial = 'C'
                                        AND Tps_RecordStatus = 'A'";

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetCurrentPayCycleAndRange()
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = @"SELECT Tps_PayCycle + ' (' + CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) + ')'
                                    FROM T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                        AND Tps_RecordStatus = 'A'";

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetCurrentPayPeriod(DALHelper dal)
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = @"SELECT Tps_PayCycle 
                                    FROM T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                        AND Tps_RecordStatus = 'A'";

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetCurrentSpecialPayPeriod(DALHelper dal)
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = @"SELECT Tps_PayCycle 
                                    FROM T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'S' 
                                        AND Tps_CycleIndicatorSpecial = 'C'
                                        AND Tps_RecordStatus = 'A'";

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetCurrentPayPeriod(string ProfileName)
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = string.Format(@"SELECT Tps_PayCycle 
                                    FROM {0}..T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                        AND Tps_RecordStatus = 'A'", ProfileName);

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetCurrentPayPeriod(string ProfileName, DALHelper dal)
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = string.Format(@"SELECT Tps_PayCycle 
                                    FROM {0}..T_PaySchedule
                                    WHERE Tps_CycleIndicator = 'C'
                                        AND Tps_RecordStatus = 'A'", ProfileName);

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public DataSet GetCurrentPayPeriodAndDate(bool bSpecial, string CompanyCode, string CentralProfile)
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = string.Format(@"SELECT Tps_PayCycle [Pay Cycle]
                                                    ,CONVERT(varchar(20), Tps_StartCycle, 101) [Start Cycle]
	                                                ,CONVERT(varchar(20), Tps_EndCycle, 101) [End Cycle] 
                                                    ,Tps_CycleIndicator AS [Cycle Indicator]
                                                    ,Tps_CycleType AS [Cycle Type]
                                                    ,Mcd_Name AS [Cycle Type Name]
                                                    FROM T_PaySchedule
                                                    LEFT JOIN {2}..M_CodeDtl
														ON Mcd_Code = Tps_CycleType
														AND Mcd_CodeType = 'CYCLETYPE'
														AND Mcd_CompanyCode = '{1}'
                                                    WHERE {0}
                                                        AND Tps_RecordStatus = 'A'"
                                                    , (bSpecial ? "Tps_CycleIndicator = 'S' AND Tps_CycleIndicatorSpecial = 'C'" : "Tps_CycleIndicator = 'C'")
                                                    , CompanyCode
                                                    , CentralProfile);

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetNextPayPeriod()
        {
            try
            {
                DataSet ds;
                using (DALHelper dal = new DALHelper())
                {
                    string sqlGetPayPeriod = @" DECLARE @EndDate as Datetime
                                                SET @EndDate = (SELECT Tps_EndCycle 
                                                                FROM T_PaySchedule
                                                                WHERE Tps_CycleIndicator = 'C' 
                                                                        AND Tps_RecordStatus = 'A')

                                                  SELECT Tps_PayCycle
                                                  FROM T_PaySchedule
                                                  WHERE Tps_StartCycle = DATEADD(dd,1,@EndDate)
                                                      AND Tps_CycleIndicator <> 'S'
                                                      AND Tps_RecordStatus = 'A'";

                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlGetPayPeriod, CommandType.Text);
                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public string GetNextPayPeriodNoFuture(string Tps_PayCycle, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string _nextpayperiod = string.Empty;

            #region query
            string qString = @"SELECT Tps_PayCycle 
                                FROM T_PaySchedule 
                                WHERE Tps_StartCycle = (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @Tps_PayCycle
						                                    AND Tps_RecordStatus = 'A'
                                                            AND Tps_CycleIndicator <> 'F'
						                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5'))
                                    AND  Tps_RecordStatus = 'A'
                                    AND Tps_CycleIndicator <> 'F'
                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5')";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _nextpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _nextpayperiod;
        }

        public string GetNextPayPeriod(string Tps_PayCycle, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string _nextpayperiod = string.Empty;

            #region query
            string qString = @"SELECT Tps_PayCycle 
                                FROM T_PaySchedule 
                                WHERE Tps_StartCycle = (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @Tps_PayCycle
						                                    AND Tps_RecordStatus = 'A'
						                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5'))
                                    AND  Tps_RecordStatus = 'A'
                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5')";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _nextpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _nextpayperiod;
        }

        public string GetPrevPayPeriodNoFuture(string Tps_PayCycle, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string _prevpayperiod = string.Empty;

            #region query
            string qString = @"SELECT Tps_PayCycle 
                                FROM T_PaySchedule 
                                WHERE Tps_EndCycle = (SELECT DATEADD(dd, -1, Tps_StartCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @Tps_PayCycle
						                                    AND Tps_RecordStatus = 'A'
                                                            AND Tps_CycleIndicator <> 'F'
						                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5'))
                                    AND  Tps_RecordStatus = 'A'
                                    AND Tps_CycleIndicator <> 'F'
                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5')";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _prevpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _prevpayperiod;
        }

        public string GetPrevPayPeriod(string Tps_PayCycle, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string _prevpayperiod = string.Empty;

            #region query
            string qString = @"SELECT Tps_PayCycle 
                                FROM T_PaySchedule 
                                WHERE Tps_EndCycle = (SELECT DATEADD(dd, -1, Tps_StartCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @Tps_PayCycle
						                                    AND Tps_RecordStatus = 'A'
						                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5'))
                                    AND  Tps_RecordStatus = 'A'
                                    AND Right(Tps_PayCycle,1) in ('1','2','0','3','4','5')";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _prevpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _prevpayperiod;
        }

        public string GetPrevSpecialPayPeriod(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();
            string _prevpayperiod = string.Empty;

            #region query
            string qString = string.Format(@"SELECT TOP 1 Tps_PayCycle 
                                            FROM T_PaySchedule 
                                            WHERE Tps_EndCycle <= (SELECT DATEADD(dd, -1, Tps_StartCycle)
                                                                FROM T_PaySchedule
                                                                WHERE  Tps_PayCycle = '{0}'
						                                            AND Tps_RecordStatus = 'A')
                                                AND Tps_CycleIndicator = 'S'
                                                AND Tps_CycleType <> 'L'
								            ORDER BY Tps_StartCycle DESC", Tps_PayCycle);
            #endregion
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _prevpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _prevpayperiod;
        }

        public string GetNextSpecialPayPeriod(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();
            string _nextpayperiod = string.Empty;

            #region query
            string qString = string.Format(@"SELECT TOP 1 Tps_PayCycle 
                                            FROM T_PaySchedule 
                                            WHERE Tps_StartCycle >= (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                                FROM T_PaySchedule
                                                                WHERE  Tps_PayCycle = '{0}'
						                                            AND Tps_RecordStatus = 'A')
                                                AND Tps_CycleIndicator = 'S'
                                                AND Tps_CycleType <> 'L'
								            ORDER BY Tps_StartCycle", Tps_PayCycle);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _nextpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _nextpayperiod;
        }

        public string GetPrevFinalPayPayPeriod(string Tps_PayCycle, string Tps_CycleType)
        {
            DataSet ds = new DataSet();
            string _prevpayperiod = string.Empty;

            #region query
            string qString = string.Format(@"SELECT TOP 1 Tps_PayCycle 
                                            FROM T_PaySchedule 
                                            WHERE Tps_EndCycle <= (SELECT DATEADD(dd, -1, Tps_StartCycle)
                                                                FROM T_PaySchedule
                                                                WHERE  Tps_PayCycle = '{0}'
						                                            AND Tps_RecordStatus = 'A')
                                                AND Tps_CycleIndicator = 'S'
                                                AND Tps_CycleType = '{1}'
								            ORDER BY Tps_StartCycle DESC", Tps_PayCycle, Tps_CycleType);
            #endregion
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _prevpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _prevpayperiod;
        }

        public string GetNextFinalPayPayPeriod(string Tps_PayCycle, string Tps_CycleType)
        {
            DataSet ds = new DataSet();
            string _nextpayperiod = string.Empty;

            #region query
            string qString = string.Format(@"SELECT TOP 1 Tps_PayCycle 
                                            FROM T_PaySchedule 
                                            WHERE Tps_StartCycle >= (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                                FROM T_PaySchedule
                                                                WHERE  Tps_PayCycle = '{0}'
						                                            AND Tps_RecordStatus = 'A')
                                                AND Tps_CycleIndicator = 'S'
                                                AND Tps_CycleType = '{1}'
								            ORDER BY Tps_StartCycle", Tps_PayCycle, Tps_CycleType);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                _nextpayperiod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return _nextpayperiod;
        }

        public DataSet GetCurrentStartEndCycleDate()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT CONVERT(DATE,Tps_StartCycle) AS Tps_StartCycle
                                ,CONVERT(DATE,Tps_EndCycle) AS Tps_EndCycle
                                ,Tps_PayCycle
                                ,Tps_Remarks
                                FROM T_PaySchedule
                                WHERE Tps_CycleIndicator = 'C'
                                    AND Tps_RecordStatus = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycles(string PayCycle, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                        + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                FROM T_PaySchedule
                                LEFT JOIN {2}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = '{1}'					
                                LEFT JOIN {2}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = '{1}'
	                            WHERE Tps_CycleType = 'N'
                                AND Tps_CycleIndicator IN ('C','P')
	                            AND LEFT(Tps_PayCycle, 6) = LEFT('{0}',6)
                                AND Tps_PayCycle <= '{0}'
                                ORDER BY RIGHT(Tps_PayCycle,1)", PayCycle
                                                              , CompanyCode
                                                              , CentralProfile);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetPayCycleFromDate(string Date)
        {
            try
            {
                DataSet ds = new DataSet();

                string sqlQuery = string.Format(@"SELECT Tps_PayCycle 
                                                FROM T_PaySchedule
                                                WHERE '{0}' BETWEEN Tps_StartCycle AND Tps_EndCycle
                                                    AND Tps_CycleType = 'N'
                                                    AND Tps_RecordStatus = 'A'", Date);

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex.Message);
            }
        }

        public DataSet GetPayPeriodStartEndCycleDate(string PayPeriod)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"SELECT Tps_StartCycle --CONVERT(CHAR(10), Tps_StartCycle, 101) as Tps_StartCycle
                                        , Tps_EndCycle --CONVERT(CHAR(10), Tps_EndCycle, 101) as Tps_EndCycle
                                        , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [CycleDescription]
                                        , Tps_PayCycle
                                        , Tps_PayDate 
                                        , Tps_CycleIndicator
                                        , Tps_CycleType
                                        , CYCLETYPE.Mcd_Name AS [Cycle Type Name]
										, Tps_Remarks
										, CASE WHEN Tps_ComputeTax = 1 THEN 'YES' ELSE 'NO' END AS Tps_ComputeTax
										, Tps_TaxSchedule  
                                        , TAXSCHED.Mcd_Name AS [Tax Schedule Name]        
										, Tps_TaxComputation                          
                                FROM T_PaySchedule
                                LEFT OUTER JOIN {0}..M_CodeDtl TAXSCHED
								    ON TAXSCHED.Mcd_Code = Tps_TaxSchedule 
									AND TAXSCHED.Mcd_CodeType = 'TAXSCHED' 
                                    AND TAXSCHED.Mcd_CompanyCode = @Mcd_CompanyCode
								LEFT OUTER JOIN {0}..M_CodeDtl CYCLETYPE
									ON CYCLETYPE.Mcd_Code = Tps_CycleType 
									AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE' 
                                    AND CYCLETYPE.Mcd_CompanyCode = @Mcd_CompanyCode
                                WHERE Tps_PayCycle = @Tps_PayCycle
                                    AND Tps_RecordStatus = 'A'"
                                , LoginInfo.getUser().CentralProfileName);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@Mcd_CompanyCode", LoginInfo.getUser().CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetPayPeriodGivenDate(string strGivenDate)
        {
            DataTable dtResult = new DataTable();
            string strPayPeriod = string.Empty;

            string sqlQuery = string.Format(@"SELECT TOP 1 Tps_PayCycle
                                            FROM T_PaySchedule
                                            WHERE '{0}' BETWEEN Tps_StartCycle AND Tps_EndCycle
                                                AND Tps_RecordStatus = 'A'
										        AND Tps_CycleIndicator IN ('P','C','F')
                                            ORDER BY Tps_PayCycle DESC", strGivenDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(sqlQuery, CommandType.Text).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                strPayPeriod = dtResult.Rows[0]["Tps_PayCycle"].ToString();

            return strPayPeriod;
        }

        public string GetCycleIndicator(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Tps_CycleIndicator
                                    , Tps_RecordStatus 
                                 FROM T_PaySchedule
                                WHERE Tps_PayCycle = @Tps_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString() == "C")
                {
                    CommonProcedures.showMessageError(ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString().Trim() + " status has been cancelled.");
                    return string.Empty;
                }
                else
                    return ds.Tables[0].Rows[0]["Tps_CycleIndicator"].ToString().Trim();
            }
            else
                return string.Empty;
        }

        public string GetCycleIndicator(string Tps_PayCycle, string ProfileName, DALHelper dal)
        {
            #region query
            string qString = string.Format(@"SELECT Tps_CycleIndicator
                                                , Tps_RecordStatus 
                                             FROM {0}.dbo.T_PaySchedule
                                             WHERE Tps_PayCycle = @Tps_PayCycle", ProfileName);

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            DataSet ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Tps_CycleIndicator"].ToString().Trim();
            }
            else
                return string.Empty;
        }

        public string GetSpecialCycleIndicator(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Tps_CycleIndicatorSpecial
                                    , Tps_RecordStatus 
                                 FROM T_PaySchedule
                                WHERE Tps_PayCycle = @Tps_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString() == "C")
                {
                    CommonProcedures.showMessageError(ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString().Trim() + " status has been cancelled.");
                    return string.Empty;
                }
                else
                    return ds.Tables[0].Rows[0]["Tps_CycleIndicatorSpecial"].ToString().Trim();
            }
            else
                return string.Empty;
        }

        public DataTable GetUserPasswordFromProfile(string CompanyCode, string CentralProfile)
        {
            #region query
            string qString = string.Format(@"SELECT Mur_UserCode
									,CASE WHEN Mur_UserPassword IS NOT NULL
										THEN Mur_UserPassword ELSE '{0}' END AS UserPassword
									, CASE WHEN Mur_UserPassword IS NOT NULL 
                                        THEN 'False' ELSE 'True' END as DefaultPassword
									FROM M_User", GetParameterValueFromCentral("PASSWRDDEF", CompanyCode, CentralProfile));

            #endregion
            DataTable dt = new DataTable();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(qString, CommandType.Text).Tables[0];
                dal.CloseDB();
            }
            if (dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        public string GetCycleIndicatorSpecial(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Tps_CycleIndicatorSpecial
                                    , Tps_RecordStatus 
                                FROM T_PaySchedule
                                WHERE Tps_PayCycle = @Tps_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString() == "C")
                {
                    CommonProcedures.showMessageError(ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString().Trim() + " status has been cancelled.");
                    return string.Empty;
                }
                else
                    return ds.Tables[0].Rows[0]["Tps_CycleIndicatorSpecial"].ToString().Trim();
            }
            else
                return string.Empty;
        }


        public string GetCurrentCycleTypeSpecial()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Tps_CycleType
                                , Tps_RecordStatus 
                                FROM T_PaySchedule
                                WHERE Tps_CycleIndicator = 'S'
                                    AND Tps_CycleIndicatorSpecial = 'C'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString() == "C")
                {
                    CommonProcedures.showMessageError(ds.Tables[0].Rows[0]["Tps_RecordStatus"].ToString().Trim() + " status has been cancelled.");
                    return string.Empty;
                }
                else
                    return ds.Tables[0].Rows[0]["Tps_CycleType"].ToString().Trim();
            }
            else
                return string.Empty;
        }

        public DataSet GetCurrentSpecialPayPeriod(string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle as PayCycleCode
                                                , CYCLETYPE.Mcd_Name as CycleType
                                                FROM T_PaySchedule 
								                LEFT JOIN {0}..M_CodeDtl CYCLETYPE
								                ON Tps_CycleType = Mcd_Code
								                AND Mcd_CodeType = 'CYCLETYPE'
								                AND Mcd_CompanyCode = '{1}'
                                                WHERE Tps_CycleIndicator = 'S'
                                                    AND Tps_CycleIndicatorSpecial = 'C'
                                                    AND Tps_RecordStatus = 'A'", CentralProfile, CompanyCode);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            return ds;
        }

        public string GetCurrentSpecialPayPeriodWithType(string Type)
        {
            DataSet ds = new DataSet();
            string strSpecialPayPeriod = string.Empty;

            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'S'
                                                AND Tps_CycleIndicatorSpecial = 'C'
                                                AND Tps_CycleType IN ({0})
                                                AND Tps_RecordStatus = 'A'", EncodeFilterItems(Type, true));
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count != 0)
                strSpecialPayPeriod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return strSpecialPayPeriod;
        }

        public string GetCurrentSpecialPayPeriodWithType(string Type, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string strSpecialPayPeriod = string.Empty;

            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_CycleIndicator = 'S'
                                                AND Tps_CycleIndicatorSpecial = 'C'
                                                AND Tps_CycleType IN ('{0}')
                                                AND Tps_RecordStatus = 'A'", Type);
            #endregion

            ds = dal.ExecuteDataSet(qString, CommandType.Text);

            if (ds.Tables[0].Rows.Count != 0)
                strSpecialPayPeriod = ds.Tables[0].Rows[0]["Tps_PayCycle"].ToString();

            return strSpecialPayPeriod;
        }


        public int UpdateProcessControlFlag(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode)
        {
            int retVal = 0;

            #region update query
            string Upstring = @"UPDATE T_SettingControl
                                SET Tsc_SetFlag = @Tsc_SetFlag
                                    ,Usr_Login = @Usr_Login
                                    ,Ludatetime = GetDate()
                                WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode";
            #endregion

            ParameterInfo[] UpparamInfo = new ParameterInfo[4];
            UpparamInfo[0] = new ParameterInfo("@Tsc_SetFlag", Tsc_SetFlag);
            UpparamInfo[1] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            UpparamInfo[2] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);
            UpparamInfo[3] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(Upstring, CommandType.Text, UpparamInfo);

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

        public bool GetProcessControlFlag(string Tsc_SystemCode, string Tsc_SettingCode)
        {
            DataSet ds = new DataSet();
            
            #region query
            string qString = @"SELECT Tsc_SetFlag, Tsc_RecordStatus, Tsc_SettingName
                                FROM T_SettingControl
                                WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode
                                    AND Tsc_RecordStatus = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            paramInfo[1] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString().Trim());
            }
            else
               
                throw (new Exception(Tsc_SystemCode + " - " + Tsc_SettingCode + " is not yet setup."));
            
        }

        public bool IsFoundInParameterTable(DataTable dtTable, string key)
        {
            bool isFound = false;
            foreach (DataRow drRow in dtTable.Rows)
            {
                if (drRow["Mpd_SubCode"].ToString().ToUpper().Trim().Equals(key.ToUpper().Trim()))
                {
                    isFound = true;
                    break;
                }
            }
            return isFound;
        }

        public string GetPolicyDtlParameterValue(string PolicyCode, string PolicySubCode, string CompanyCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT RTRIM(Mpd_ParamValue) 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode 
	                                AND RTRIM(Mpd_SubCode) = @PolicySubCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@PolicySubCode", PolicySubCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetPolicyDtlParameterValue(string PolicyCode, string PolicySubCode, string CompanyCode, DALHelper dal)
        {
            #region query
            string qString = @"SELECT RTRIM(Mpd_ParamValue)  
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode 
	                                AND RTRIM(Mpd_SubCode) = @PolicySubCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@PolicySubCode", PolicySubCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            DataSet ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetPolicyDtlDescription(string PolicyCode, string PolicySubCode, string CompanyCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT Mpd_SubName 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode 
	                                AND RTRIM(Mpd_SubCode) = @PolicySubCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@PolicySubCode", PolicySubCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetPolicyDtlFormula(string PolicyCode, string PolicySubCode, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT Mpd_Formula 
                               FROM M_PolicyDtl
                                WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode 
	                                AND RTRIM(Mpd_SubCode) = @PolicySubCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@PolicySubCode", PolicySubCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public DataTable GetPolicyDtlListfromCentral(string PolicyCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode
                                    , RTRIM(Mpd_ParamValue) AS Mpd_ParamValue
                               FROM M_PolicyDtl
                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetPolicyDtlListFormulafromCentral(string PolicyCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode, Mpd_Formula
                               FROM M_PolicyDtl
                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetPolicyDtlListFormulafromCentral(string PolicyCode, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode, Mpd_Formula
                                               FROM {0}..M_PolicyDtl
                                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                                AND RTRIM(Mpd_RecordStatus) = 'A'", CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetParameterDtlListfromPayroll(string PolicyCode, string CompanyCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode
                                    , RTRIM(Mpd_ParamValue) AS Mpd_ParamValue
                               FROM M_PolicyDtl
                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetParameterDtlListfromPayroll(string PolicyCode, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode
                                    , RTRIM(Mpd_ParamValue) AS Mpd_ParamValue
                               FROM M_PolicyDtl
                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
	                                AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetParameterDtlListAllowedfromPayroll(string PolicyCode, string CompanyCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode, Mpd_SubName
                               FROM M_PolicyDtl
                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
                                    AND RTRIM(Mpd_ParamValue) = '1'
                                    AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetParameterDtlListAllowedfromPayroll(string PolicyCode, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"SELECT RTRIM(Mpd_SubCode) AS Mpd_SubCode, Mpd_SubName
                               FROM M_PolicyDtl
                               WHERE RTRIM(Mpd_PolicyCode) = @PolicyCode
                                    AND RTRIM(Mpd_CompanyCode) = @CompanyCode
                                    AND RTRIM(Mpd_ParamValue) = '1'
                                    AND RTRIM(Mpd_RecordStatus) = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PolicyCode", PolicyCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public int CheckProcessControlFlagSetup(string Tsc_SystemCode, string Tsc_SettingCode)
        {
            Object temp = new Object();

            #region query

            string qString = @"SELECT Count(Tsc_SystemCode)
                               FROM T_SettingControl
                               WHERE Tsc_SystemCode = @Tsc_SystemCode
                                    AND Tsc_SettingCode = @Tsc_SettingCode
                                    AND Tsc_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tsc_SystemCode", Tsc_SystemCode);
            paramInfo[1] = new ParameterInfo("@Tsc_SettingCode", Tsc_SettingCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                temp = dal.ExecuteScalar(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return Convert.ToInt32(temp);
        }

        public bool CheckIfProcessFlagExists(string[,] Param, int endindex)
        {
            CommonBL cmnBL = new CommonBL();
            bool errFlg = false;
            string errMsg = string.Empty;

            for (int i = 0; i < endindex; i++)
            {
                if (cmnBL.CheckProcessControlFlagSetup(Param[i, 0].Trim(), Param[i, 1].Trim()) == 0)
                {
                    errFlg = true;
                    errMsg = errMsg + "\nSystem: " + Param[i, 0].Trim() + ", Process: " + Param[i, 1].Trim();
                }
            }
            if (errFlg)
            {
                CommonProcedures.showMessageInformation("Setting control flag not setup: " + errMsg);
                return false;
            }
            else
                return true;
        }


        #region <For Lookup>

        public string GetSystemID(string ModuleCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(string.Format("SELECT Msm_SystemCode FROM M_SystemModule WHERE Msm_ModuleCode = '{0}' AND Msm_CompanyCode = '{1}'", ModuleCode, CompanyCode));

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetSystemID(string ModuleCode, string CompanyCode, DALHelper dalCentral)
        {
            DataSet ds = new DataSet();
            ds = dalCentral.ExecuteDataSet(string.Format("SELECT Msm_SystemCode FROM M_SystemModule WHERE Msm_ModuleCode = '{0}' AND Msm_CompanyCode = '{1}'", ModuleCode, CompanyCode));
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }
        public string GetSystemID(string ModuleCode, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();
            ds = dal.ExecuteDataSet(string.Format("SELECT Msm_SystemCode FROM {2}..M_SystemModule WHERE Msm_ModuleCode = '{0}' AND Msm_CompanyCode = '{1}'", ModuleCode, CompanyCode, CentralProfile));
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }
        public bool CheckUserHasAccessOnEmployee(string employeeID, string systemID, string userCode, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            #region Query
            string query = string.Format(@"
                SELECT Mem_IDNo
                FROM M_Employee
                INNER JOIN {5}..M_UserExtTmp ON Mue_SystemCode = '{1}'
                    AND Mue_UserCode = '{2}'
                    AND Mue_CompanyCode = '{3}'
					AND Mue_ProfileCode = '{4}'
                    AND Mue_CostCenterCode = Mem_CostcenterCode
                    AND Mue_PayrollGroup = Mem_PayrollGroup
                    AND Mue_EmploymentStatus = Mem_EmploymentStatusCode
                WHERE Mem_IDNo = '{0}'"
                , employeeID
                , systemID
                , userCode
                , CompanyCode
                , ProfileCode
                , CentralProfile);
            #endregion

            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
        public bool CheckUserHasAccessOnEmployee(string employeeID, string systemID, string userCode, string CompanyCode, string ProfileCode, string CentralProfile, DALHelper dalCentral)
        {
            #region Query
            string query = string.Format(@"
                SELECT Mem_IDNo
                FROM M_Employee
                INNER JOIN {5}..M_UserExtTmp ON Mue_SystemCode = '{1}'
                    AND Mue_UserCode = '{2}'
                    AND Mue_CompanyCode = '{3}'
					AND Mue_ProfileCode = '{4}'
                    AND Mue_CostCenterCode = Mem_CostcenterCode
                    AND Mue_PayrollGroup = Mem_PayrollGroup
                    AND Mue_EmploymentStatus = Mem_EmploymentStatusCode
                WHERE Mem_IDNo = '{0}'"
                , employeeID
                , systemID
                , userCode
                , CompanyCode
                , ProfileCode
                , CentralProfile);
            #endregion

            DataSet ds = dalCentral.ExecuteDataSet(query);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public int GetCostCenterAccess(string SystemCode, string CompanyCode, string UserCode, string CentralProfile)
        {
            object retVal = 0;

            string qString = string.Format(@"SELECT ISNULL(COUNT(Mue_CostCenterCode),0)
                                            FROM M_UserExt
                                            WHERE Mue_SystemCode = '{0}'
                                                AND Mue_UserCode = '{1}'
                                                AND Mue_CompanyCode = '{2}'
												AND Mue_ProfileCode = '{3}'
                                                AND Mue_CostCenterCode = 'ALL'
												AND Mue_RecordStatus = 'A'"
                                            , SystemCode
                                            , UserCode
                                            , CompanyCode
                                            , LoginInfo.getUser().DBNumber);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                retVal = dal.ExecuteScalar(qString);

                dal.CloseDB();
            }
            if (Convert.ToInt32(retVal) > 0)
                return 1;
            else
                return 0;
        }

        public int GetCostCenterAccess(string SystemCode, string CompanyCode, string UserCode, string CentralProfile, string ProfileCode)
        {
            object retVal = 0;

            string qString = string.Format(@"SELECT ISNULL(COUNT(Mue_CostCenterCode),0)
                                            FROM M_UserExt
                                            WHERE Mue_SystemCode = '{0}'
                                                AND Mue_UserCode = '{1}'
                                                AND Mue_CompanyCode = '{2}'
												AND Mue_ProfileCode = '{3}'
                                                AND Mue_CostCenterCode = 'ALL'
												AND Mue_RecordStatus = 'A'"
                                            , SystemCode
                                            , UserCode
                                            , CompanyCode
                                            , ProfileCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                retVal = dal.ExecuteScalar(qString);

                dal.CloseDB();
            }
            if (Convert.ToInt32(retVal) > 0)
                return 1;
            else
                return 0;
        }

        public string IDNumberLookupStatement(string ModuleCode)
        {
            string companyCode = LoginInfo.getUser().CompanyCode;
            string centralProfile = LoginInfo.getUser().CentralProfileName;
            string UserCode = LoginInfo.getUser().UserCode;
            string profileCode = LoginInfo.getUser().DBNumber;
            string qString = string.Empty;
            string[] varparams = new string[7];
            string SystemID = this.GetSystemID(ModuleCode, companyCode, centralProfile);

            varparams[0] = centralProfile;
            varparams[1] = SystemID;
            varparams[2] = UserCode;
            varparams[3] = this.GetCostCenterAccess(SystemID, companyCode, UserCode, centralProfile, profileCode).ToString();
            varparams[4] = companyCode;
            varparams[5] = profileCode;
            varparams[6] = (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", companyCode, centralProfile);

            qString = string.Format(CommonConstants.Queries.SelectIDNumber, varparams);

            return qString;
        }

        public string IDNumberLookupStatementWNotIn(string Msm_ModuleCode, string Condition, string Condition2)
        {
            string companyCode = LoginInfo.getUser().CompanyCode;
            string centralProfile = LoginInfo.getUser().CentralProfileName;
            string UserCode = LoginInfo.getUser().UserCode;
            string profileCode = LoginInfo.getUser().DBNumber;
            string qString = string.Empty;
            string[] varparams = new string[9];
            string SystemID = this.GetSystemID(Msm_ModuleCode, companyCode, centralProfile);

            varparams[0] = centralProfile;
            varparams[1] = SystemID;
            varparams[2] = UserCode;
            varparams[3] = this.GetCostCenterAccess(SystemID, companyCode, UserCode, centralProfile, profileCode).ToString();
            varparams[4] = companyCode;
            varparams[5] = profileCode;
            varparams[6] = (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", companyCode, centralProfile);
            varparams[7] = Condition;
            varparams[8] = Condition2;

            qString = string.Format(CommonConstants.Queries.SelectIDNumberCondition, varparams);

            return qString;
        }

        public string IDNumberLookupStatement(string Msm_ModuleCode, string Condition, string ConditionExclude)
        {
            string companyCode = LoginInfo.getUser().CompanyCode;
            string centralProfile = LoginInfo.getUser().CentralProfileName;
            string UserCode = LoginInfo.getUser().UserCode;
            string profileCode = LoginInfo.getUser().DBNumber;
            string qString = string.Empty;
            string[] varparams = new string[8];
            string SystemID = this.GetSystemID(Msm_ModuleCode, companyCode, centralProfile);

            varparams[0] = centralProfile;//confi;
            varparams[1] = SystemID;
            varparams[2] = UserCode;
            varparams[3] = this.GetCostCenterAccess(SystemID, companyCode, UserCode, centralProfile, profileCode).ToString();
            varparams[4] = companyCode;
            varparams[5] = profileCode;
            varparams[6] = (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", companyCode, centralProfile);
            varparams[7] = ConditionExclude;

            qString = string.Format(CommonConstants.Queries.SelectIDNumberConditionExclude, varparams);

            return qString;
        }

        public DataTable GetProfile(string DBNumber, DALHelper dalHelper)
        {
            string strQuery = string.Format(@"SELECT * FROM M_Profile
                                                WHERE Mpf_RecordStatus = 'A'
                                                AND Mpf_ProfileType IN ('P','S')
                                                AND Mpf_DatabaseNo = '{0}'", DBNumber);

            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        public string FetchBackupFileName(string DBName, bool bCycleBackup, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query
            #region old query
            //string qString = @"SELECT TOP 1 Tps_PayCycle + '_' + '{0}' + '_{1}' + RIGHT(Convert(char(10),Tps_EndCycle,101),4)
            //                       + Substring(Convert(char(10),Tps_EndCycle,101),1,2)
            //                          + Substring(Convert(char(10),Tps_EndCycle,101),4,2) 
            //                             + '_' + REPLACE(CONVERT(varchar, GETDATE(), 108), ':', '') + '.bak' as Filename
            //                       , Convert(char(10),Tps_EndCycle,101)
            //                    FROM T_PaySchedule
            //                    WHERE Tps_CycleIndicator = 'C'
            //                    AND Tps_RecordStatus = 'A'
            //                    ORDER BY Tps_EndCycle";
            #endregion

            string qString = @"SELECT TOP 1 Tps_PayCycle + '_' + '{0}' + '_{1}' + CONVERT(VARCHAR,GETDATE(),112)
                                         + '_' + REPLACE(CONVERT(varchar, GETDATE(), 108), ':', '') + '.bak' as Filename
                                FROM T_PaySchedule
                                WHERE Tps_CycleIndicator = 'C'
                                AND Tps_RecordStatus = 'A'
                                ORDER BY Tps_PayCycle";

            qString = string.Format(qString, DBName, (bCycleBackup ? "CYCLEBACKUP_" : ""));
            #endregion

            ds = dal.ExecuteDataSet(qString, CommandType.Text);
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string[] GetIDNumberArgs()
        {
            string[] args = new string[10];
            args[0] = "ID Number";
            args[1] = "Last Name";
            args[2] = "First Name";
            args[3] = "Middle Name";
            args[4] = "Nick Name";
            args[5] = "Payroll Group";
            args[6] = "Payroll Type";
            args[7] = "Employment Status";
            args[8] = "Work Status";
            args[9] = "Cost Center";
            

            return args;
        }

        public string[] GetIDNumberInactiveArgs()
        {
            string[] args = new string[11];
            args[0] = "ID Number";
            args[1] = "Last Name";
            args[2] = "First Name";
            args[3] = "Middle Name";
            args[4] = "Nick Name";
            args[5] = "Age";
            args[6] = "Hired Date";
            args[7] = "Regular Date";
            args[8] = "Separation Date";
            args[9] = "Employment Status";
            args[10] = "CostCenter Code";

            return args;
        }

        public string[] GetIDNumberRetroPayArgs()
        {
            string[] args = new string[9];
            args[0] = "ID Number";
            args[1] = "Last Name";
            args[2] = "First Name";
            args[3] = "Middle Name";
            args[4] = "Nick Name";
            args[5] = "Start Date";
            args[6] = "Salary";
            args[7] = "Reason";
            args[8] = "Remarks";

            return args;
        }

        public string[] GetIDNumberFinalPayArgs()
        {
            string[] args = new string[9];
            args[0] = "ID Number";
            args[1] = "Last Name";
            args[2] = "First Name";
            args[3] = "Middle Name";
            args[4] = "Nick Name";
            args[5] = "Employment Status";
            args[6] = "Hire Date";
            args[7] = "Regular Date";
            args[8] = "Separation Date";
            return args;
        }


        #endregion

        #region <For combined Confi and Non-Confi Lookup>

        public string GetDatabaseName(string dbNumber, string centralProfile)
        {
            string DBNumber = string.Empty;
            string CentralProfile = string.Empty;

            if (dbNumber != string.Empty && centralProfile != string.Empty)
            {
                DBNumber = dbNumber;
                CentralProfile = centralProfile;
            }
            else
            {
                LoginInfoRecord user = LoginInfo.getUser();
                DBNumber = user.DBNumber;
                CentralProfile = user.CentralProfileName;
            }
            string CurrentDatabase = string.Empty;

            DataSet ds = new DataSet();

            string sqlstatement = @"SELECT Mpf_DatabaseName
                                    FROM M_Profile
                                    WHERE Mpf_DatabaseNo = '" + DBNumber + "'";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlstatement);

                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                CurrentDatabase = ds.Tables[0].Rows[0][0].ToString();
            }

            return CurrentDatabase;

        }

        public string GetProfileName(string dbNumber, string centralProfile)
        {
            string DBNumber = string.Empty;
            string CentralProfile = string.Empty;

            if (dbNumber != string.Empty && centralProfile != string.Empty)
            {
                DBNumber = dbNumber;
                CentralProfile = centralProfile;
            }
            else
            {
                LoginInfoRecord user = LoginInfo.getUser();
                DBNumber = user.DBNumber;
                CentralProfile = user.CentralProfileName;
            }
            string CurrentDatabase = string.Empty;

            DataSet ds = new DataSet();

            string sqlstatement = @"SELECT Mpf_ProfileName
                                    FROM M_Profile
                                    WHERE Mpf_DatabaseNo = '" + DBNumber + "'";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlstatement);

                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                CurrentDatabase = ds.Tables[0].Rows[0][0].ToString();
            }

            return CurrentDatabase;

        }
        #endregion


        #region <For combined Confi and Non-Confi Lookup For User Master>
        private string SQLSelectEmpIDUserMaster()
        {
            return @"SELECT  Mem_IDNo as [ID Number]
                         , Emt_OldEmployeeID as [Old ID Number]
                         , Mem_NickName as [Nick Name]
                         , Mem_LastName as [Last Name]
                         , Mem_FirstName  as [First Name]
                         , LEFT(Mem_MiddleName, 1) as [MI]
                         , a.Mcd_Name as [Position]
                         , b.Mcd_Name as [Job Status]
                    FROM M_Employee
                    LEFT JOIN M_CodeDtl a on a.Mcd_Code =  Mem_JobTitleCode
                        AND a.Mcd_CodeType = 'POSITION'
                    LEFT JOIN M_CodeDtl b on b.Mcd_Code = Mem_WorkStatus
                        AND b.Mcd_CodeType='WORKSTAT'
                    WHERE Mem_IDNo not in (SELECT Muh_UserCode FROM M_UserHdr)";
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

        private DataSet GetNonConfiRecordsUserMaster()//For NonConfi
        {
            DataSet ds = new DataSet();

            string sqlstatement = this.SQLSelectEmpIDUserMaster();

            using (DALHelper dal = new DALHelper("NON-CONFI"))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlstatement, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        private void CheckIfRecordExistsInNonConfiUserMaster(string IDNumber, DataSet NonConfids)
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

        #endregion

        public string GetDayColor(string DayCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Mcd_Name
                               FROM M_Day
                               INNER JOIN M_CodeDtl on Mdy_ColorCode = Mcd_Code 
                                    AND Mcd_CompanyCode = Mdy_CompanyCode
                                    AND Mcd_CodeType = 'DAYCOLOR'
                               WHERE Mdy_RecordStatus = 'A'
                               AND Mdy_DayCode = @DayCode
                               AND Mdy_CompanyCode = @CompanyCode";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@DayCode", DayCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["Mcd_Name"].ToString() != string.Empty)
            {
                return ds.Tables[0].Rows[0]["Mcd_Name"].ToString();
            }
            else
                return "White";
        }

        public string GetDTRDatabaseName()
        {
            return Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());
        }

        public DataTable GetNewSignatory(string MenuCode, string CentralProfile, string CompanyCode)
        {
            string strQuery = string.Format(@"SELECT  Mds_SignatorySeqNo as [Seq]	
                                            , Mds_SignatoryCode as [Signatory ID Number]	
                                            , SIG.Mcd_Name as [Signatory Role]		
                                            , Mem_FirstName + ' ' + CASE WHEN LEN(RTRIM(Mem_MiddleName)) > 0 
	                                            THEN LEFT(Mem_MiddleName, 1) + '. '  ELSE ' ' END + Mem_LastName + ' ' + Mem_ExtensionName as [Signatory Name]		
                                            , POS.Mcd_Name as [Position]		
                                            FROM M_DocumentSignatory		
                                            INNER JOIN M_Employee ON Mem_IDNo = Mds_SignatoryCode		
                                            LEFT JOIN M_CodeDtl POS ON POS.Mcd_Code = Mem_PositionCode		
                                            AND POS.Mcd_CompanyCode = '{0}'		
                                            AND POS.Mcd_CodeType = 'POSITION'		
                                            LEFT JOIN M_CodeDtl SIG ON SIG.Mcd_Code = Mds_SignatoryRoleCode		
                                            AND SIG.Mcd_CompanyCode ='{0}'
                                            AND SIG.Mcd_CodeType = 'DOCSIGROLE'		
                                            WHERE Mds_CompanyCode = '{0}'		
                                            AND Mds_ModuleCode = '{1}'		
                                            AND Mds_IsDefaultSignatory=1		
                                            AND Mds_RecordStatus = 'A'		
                                            ORDER BY Mds_SignatorySeqNo	"
                                            , CompanyCode
                                            , MenuCode);

            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dtResult = dal.ExecuteDataSet(strQuery, CommandType.Text).Tables[0];
                }
                catch (Exception er)
                {
                    dtResult = null;
                    CommonProcedures.showMessageError("Error in signatories : " + er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dtResult;
        }

        public DataSet FetchPicture(string IDNumber, string CentralProfile, string CompanyCode)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@IDNo", IDNumber);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            DataSet ds = new DataSet();
            string sqlQuery = @"  SELECT Msg_Signature
                                   FROM M_Signatory 
                                   WHERE Msg_SignatoryCode = @IDNo
                                        AND Msg_CompanyCode = @CompanyCode";
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
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

        public static string SetupQueryWithProfiles(string query, string CentralProfile)
        {
            string ret = string.Empty;

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    DataSet ds = dal.ExecuteDataSet(
                        string.Format(@"
                        SELECT Mpf_DatabaseName 
                        FROM M_Profile
                        WHERE Mpf_RecordStatus = 'A'
                            AND Mpf_ProfileCategory = '{0}'
                        ", Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileCategory"].ToString())));

                    for (int idx = 0; ds != null
                                && idx < ds.Tables[0].Rows.Count; idx++)
                    {
                        if (ret != string.Empty)
                        {
                            ret += @" UNION
                                ";
                        }

                        ret += query.Replace("@Prof", ds.Tables[0].Rows[idx]["Mpf_DatabaseName"].ToString().Trim());
                    }
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError("Error in profile loop : " + er.Message);
                    ret = string.Empty;
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ret;
        }

        public DataTable GetProfileList(string CentralProfile, string CompanyCode)
        {
            string strQuery = string.Format(@"
                                        SELECT Mpf_DatabaseName 
                                        FROM [{1}].dbo.M_Profile
                                        WHERE Mpf_CompanyCode = '{2}'
                                        AND Mpf_ProfileType = 'P'
                                        AND Mpf_ProfileCategory = '{0}'
                                        AND Mpf_RecordStatus = 'A'
                                        ", Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileCategory"].ToString())
                                        , CentralProfile
                                        , CompanyCode);


            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dtResult = dal.ExecuteDataSet(strQuery, CommandType.Text).Tables[0];
                }
                catch (Exception ex)
                {
                    dtResult = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dtResult;
        }

        public string GetServerDateTime()
        {
            string dateTime = string.Empty;

            #region query

            string qString = @"SELECT Getdate()";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dateTime = dal.ExecuteScalar(qString, CommandType.Text).ToString();
                }
                catch
                {
                    dateTime = DateTime.Now.ToString();
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
                SELECT Mcm_CompanyCode
	                ,Mcm_CompanyCode  
	                ,Mcm_CompanyAddress1  
	                ,Mcm_CompanyAddress2+' '+ Mcd_Name 'Mcm_CompanyAddress2'
	                ,Mcm_EmailAddress 
	                ,Mcm_TelNo 
	                ,Mcm_CellNo 
	                ,Mcm_FaxNo
                FROM M_Company
                INNER JOIN M_CodeDtl ON Mcm_CompanyAddress3 = Mcd_Code AND Mcd_CodeType = 'ZIPCODE'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            string contact = string.Format(@"Phone No: {0} | Fax No: {1} | Email Add: {2}",
                                  dt.Rows[0]["Mcm_TelNo"].ToString(), dt.Rows[0]["Mcm_FaxNo"].ToString(),
                                  dt.Rows[0]["Mcm_EmailAddress"].ToString());

            string[] comp = new string[4];
            comp[0] = dt.Rows[0]["Mcm_CompanyCode"].ToString();
            comp[1] = dt.Rows[0]["Mcm_CompanyAddress1"].ToString();
            comp[2] = dt.Rows[0]["Mcm_CompanyAddress2"].ToString();
            comp[3] = contact;

            return comp;
        }
        #endregion

        public int CreateProcessControlTrail(string Tsc_SetFlag, string Tsc_SystemCode, string Tsc_SettingCode)
        {
            int retVal = 0;

            #region update query
            string Upstring = @"INSERT INTO T_SettingControlTrl
                                SELECT @SystemID
                                     , @ProcessID
                                     , Tsc_SettingName
                                     , @ProcessFlag
                                     , Tsc_AccessType
                                     , @PayCycle
                                     , NULL
                                     , 'A'
                                     , @Usr_Login
                                     , GETDATE()
                                FROM T_SettingControl 
                                WHERE Tsc_SystemCode = @SystemID
                                    AND Tsc_SettingCode = @ProcessID";
            #endregion

            ParameterInfo[] UpparamInfo = new ParameterInfo[5];
            UpparamInfo[0] = new ParameterInfo("@ProcessFlag", Tsc_SetFlag);
            UpparamInfo[1] = new ParameterInfo("@SystemID", Tsc_SystemCode);
            UpparamInfo[2] = new ParameterInfo("@ProcessID", Tsc_SettingCode);
            UpparamInfo[3] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            UpparamInfo[4] = new ParameterInfo("@PayCycle", GetCurrentPayPeriod());

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(Upstring, CommandType.Text, UpparamInfo);

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
                                            SET @yr2Digit = (SELECT right(Mcm_ProcessYear, 2) FROM M_Company)


                                            SELECT Tdn_DocumentPrefix 
	                                            + @yr2Digit 
	                                            + replicate('0', 9 - LEN(RTrim(Tdn_LastSeriesNumber)))
	                                            + RTrim(Tdn_LastSeriesNumber)
                                            FROM T_DocumentNumber
                                            WITH (UPDLOCK)
                                            WHERE Tdn_DocumentCode = '{0}'";

            string sqlControlNoUpdate = @"  UPDATE T_DocumentNumber
                                            SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
                                            WHERE Tdn_DocumentCode = '{0}'";

            dal.ExecuteNonQuery(string.Format(sqlControlNoUpdate, transactionCode.ToUpper()), CommandType.Text);
            controlNum = Convert.ToString(dal.ExecuteScalar(string.Format(sqlControlNoFetch, transactionCode.ToUpper()), CommandType.Text));

            return controlNum;
        }

        public bool ControlNumberCodeExists(string ControlNumberCode)
        {
            string ControlNumber = string.Empty;
            string sql = string.Format(@"SELECT Tdn_DocumentCode 
                                        FROM T_DocumentNumber
                                        WHERE Tdn_DocumentCode = '{0}'
                                        AND Tdn_RecordStatus = 'A'", ControlNumberCode.ToUpper());

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ControlNumber = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
                dal.CloseDB();
            }
            if (ControlNumber != string.Empty)
                return true;
            else
                return false;
        }


        public string[] EncodeFilterItems(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            return strArrFilterItems;
        }

        public string EncodeFilterItems(string strDelimited, bool bQuoted)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            string strFilterItem = "";
            foreach (string col in strArrFilterItems)
            {
                if (!string.IsNullOrEmpty(col))
                {
                    if (bQuoted)
                        strFilterItem += "'" + col.Trim() + "',";
                    else
                        strFilterItem += col.Trim() + ",";
                }
            }
            if (!string.IsNullOrEmpty(strFilterItem))
                strFilterItem = strFilterItem.Substring(0, strFilterItem.Length - 1);
            return strFilterItem;
        }

        public string SetUpItemsForEncodeFilterItems(DataTable dt, int colNum, bool bQuoted)
        {
            string strFilterItem = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (bQuoted)
                    strFilterItem += "'" + dt.Rows[i][colNum].ToString().Trim() + "',";
                else
                    strFilterItem += dt.Rows[i][colNum].ToString().Trim() + ",";
            }
            if (strFilterItem != "")
                strFilterItem = strFilterItem.Substring(0, strFilterItem.Length - 1);
            return strFilterItem;
        }

        public string[] GetEmployeeDetails(string ID)
        {
            string[] det = new string[2];
            det[0] = string.Empty;
            det[1] = string.Empty;

            string sql = string.Format(@"
                    SELECT * FROM M_Formula
                    WHERE Mdm_MainCode = 'EMPDETAIL'
					AND Mdm_RecordStatus = 'A'
                ");

            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, true))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(sql, CommandType.Text);
                    string query;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        query = ds.Tables[0].Rows[0]["Mdm_Formula"].ToString().Trim();
                        query = query.Replace("@", "'");
                        query +=
                        string.Format(
                        @"
                            WHERE Mem_IDNo = '{0}'
                        ", ID);

                        DataSet dsDetail = dal.ExecuteDataSet(query, CommandType.Text);
                        if (dsDetail.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsDetail.Tables[0].Columns.Count; i++)
                            {
                                if (i > 0)
                                    det[1] += ", ";
                                string s = dsDetail.Tables[0].Rows[0][dsDetail.Tables[0].Columns[i].Caption.ToString()].ToString().Trim();
                                det[1] += dsDetail.Tables[0].Columns[i].Caption + " - [" + s + "]";
                            }
                        }


                    }
                    else
                    {
                        det[1] = " ";
                    }
                    query = @"
                            SELECT dbo.Udf_DisplayCostCenterName(Mem_CostcenterCode)
                            FROM M_Employee
                            WHERE Mem_IDNo = '" + ID + @"'
                        ";
                    det[0] = (string)dal.ExecuteScalar(query, CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return det;
        }

        public string[] GetEmployeeDetails(string ID, string CompanyCode, string CentralProfile)
        {
            string[] det = new string[2];
            det[0] = string.Empty;
            det[1] = string.Empty;

            DALHelper dal = new DALHelper();
            try
            {
                dal.OpenDB();

                DataSet ds = GetFormulaFromCentral("EMPDETAIL", CompanyCode, CentralProfile);
                string query;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    query = ds.Tables[0].Rows[0]["Mdm_Formula"].ToString().Trim();
                    query = query.Replace("@CENTRALDB", CentralProfile);
                    query +=
                    string.Format(
                    @"
                            WHERE Mem_IDNo = '{0}'
                        ", ID);

                    DataSet dsDetail = dal.ExecuteDataSet(query, CommandType.Text);
                    if (dsDetail.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsDetail.Tables[0].Columns.Count; i++)
                        {
                            if (i > 0)
                                det[1] += ", ";
                            string s = dsDetail.Tables[0].Rows[0][dsDetail.Tables[0].Columns[i].Caption.ToString()].ToString().Trim();
                            det[1] += dsDetail.Tables[0].Columns[i].Caption + " - [" + s + "]";
                        }
                    }


                }
                else
                {
                    det[1] = " ";
                }
                CommonBL CommonBL = new CommonBL();
                query = string.Format(@"
                            SELECT {3}.dbo.Udf_DisplayCostCenterName('{1}',Mem_CostcenterCode, '{2}')
                            FROM M_Employee
                            WHERE Mem_IDNo = '{0}'"
                        , ID
                        , CompanyCode
                        , CommonBL.GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile, new DALHelper(CentralProfile, false))
                        , CentralProfile);

                det[0] = (string)dal.ExecuteScalar(query, CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }

            return det;
        }

        public DataTable GetPrevEmployee(string EmployeeId, string MenuCode, string CompanyCode, string CentralProfile)
        {
            return GetPrevEmployee(EmployeeId, MenuCode, "", CompanyCode, CentralProfile);
        }

        public DataTable GetPrevEmployee(string EmployeeId, string MenuCode, string Condition, string CompanyCode, string CentralProfile)
        {
            string SystemID = this.GetSystemID(MenuCode, CompanyCode, CentralProfile);

            string query = string.Format(@"DECLARE @EmpName AS varchar(90)
                                            SET @EmpName = (SELECT Mem_LastName+Mem_FirstName+Mem_MiddleName
		                                                    FROM M_Employee
		                                                    WHERE Mem_IDNo = '{0}')
                                            				
                                            IF @EmpName IS NULL OR @EmpName = ''
                                            BEGIN
                                                SET @EmpName = 'zzz'
                                            END

                                            SELECT TOP(1) Mem_IDNo 
                                                , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
																THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                                , Mem_FirstName as Mem_FirstName
												, Mem_MiddleName as Mem_MiddleName
                                                , Mem_NickName as Mem_NickName
                                                , PAYGRP.Mcd_Name as [Payroll Group]
                                                , PAYTYPE.Mcd_Name as [Payroll Type]
                                                , EMPSTATN.Mcd_Name as [Employment Status]
                                                , WORKSTAT.Mcd_Name as [Work Status]
                                            FROM M_Employee
                                            {6}
                                            INNER JOIN {7}..M_UserExtTmp 
                                                        ON Mue_SystemCode = '{1}'
                                                        AND Mue_UserCode = '{2}'
	                                                    AND Mue_CompanyCode = '{4}'
	                                                    AND Mue_ProfileCode = '{5}'
                                                        AND Mue_CostCenterCode = Mem_CostcenterCode
	                                                    AND Mue_PayrollGroup = Mem_PayrollGroup
	                                                    AND Mue_PayrollType = Mem_PayrollType 
	                                                    AND Mue_EmploymentStatus = Mem_EmploymentStatusCode
                                            LEFT JOIN {7}..M_CodeDtl PAYGRP 
                                                            ON PAYGRP.Mcd_Code = Mem_PayrollGroup
	                                                        AND PAYGRP.Mcd_Codetype = 'PAYGRP' 
	                                                        AND PAYGRP.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl PAYTYPE 
                                                            ON PAYTYPE.Mcd_Code = Mem_PayrollType
	                                                        AND PAYTYPE.Mcd_Codetype = 'PAYTYPE' 
	                                                        AND PAYTYPE.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl EMPSTATN 
                                                            ON EMPSTATN.Mcd_Code = Mem_EmploymentStatusCode
	                                                        AND EMPSTATN.Mcd_Codetype = 'EMPSTAT' 
	                                                        AND EMPSTATN.Mcd_CompanyCode = '{4}'
                                           LEFT JOIN {7}..M_CodeDtl WORKSTAT 
                                                            ON WORKSTAT.Mcd_Code = Mem_WorkStatus
	                                                        AND WORKSTAT.Mcd_Codetype = 'WORKSTAT' 
	                                                        AND WORKSTAT.Mcd_CompanyCode = '{4}'
                                            WHERE  Mem_LastName+Mem_FirstName+Mem_MiddleName < @EmpName
	                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
                                            ORDER BY Mem_LastName+Mem_FirstName+Mem_MiddleName DESC"
                                                    , EmployeeId
                                                    , SystemID
                                                    , LoginInfo.getUser().UserCode
                                                    , GetCostCenterAccess(SystemID,CompanyCode, LoginInfo.getUser().UserCode, CentralProfile).ToString()
                                                    , CompanyCode
                                                    , LoginInfo.getUser().DBNumber
                                                    , Condition
                                                    , CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetPrevInactiveEmployee(string EmployeeId, string MenuCode, string Condition, string ConditionEx, string CompanyCode, string CentralProfile)
        {
            string SystemID = this.GetSystemID(MenuCode, CompanyCode, CentralProfile);

            string query = string.Format(@"DECLARE @EmpName AS varchar(90)
                                            SET @EmpName = (SELECT Mem_LastName+Mem_FirstName+Mem_MiddleName
		                                                    FROM M_Employee
		                                                    WHERE Mem_IDNo = '{0}')
                                            				
                                            IF @EmpName IS NULL OR @EmpName = ''
                                            BEGIN
                                                SET @EmpName = 'zzz'
                                            END

                                            SELECT TOP(1) Mem_IDNo 
                                                , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
																THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                                , Mem_FirstName as Mem_FirstName
												, Mem_MiddleName as Mem_MiddleName
                                                , Mem_NickName as Mem_NickName
                                                , PAYGRP.Mcd_Name as [Payroll Group]
                                                , PAYTYPE.Mcd_Name as [Payroll Type]
                                                , EMPSTATN.Mcd_Name as [Employment Status]
                                                , WORKSTAT.Mcd_Name as [Work Status]
                                            FROM M_Employee
                                            {6}
                                            INNER JOIN {7}..M_UserExtTmp 
                                                        ON Mue_SystemCode = '{1}'
                                                        AND Mue_UserCode = '{2}'
	                                                    AND Mue_CompanyCode = '{4}'
	                                                    AND Mue_ProfileCode = '{5}'
                                                        AND Mue_CostCenterCode = Mem_CostcenterCode
	                                                    AND Mue_PayrollGroup = Mem_PayrollGroup
	                                                    AND Mue_PayrollType = Mem_PayrollType 
	                                                    AND Mue_EmploymentStatus = Mem_EmploymentStatusCode
                                            LEFT JOIN {7}..M_CodeDtl PAYGRP 
                                                            ON PAYGRP.Mcd_Code = Mem_PayrollGroup
	                                                        AND PAYGRP.Mcd_Codetype = 'PAYGRP' 
	                                                        AND PAYGRP.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl PAYTYPE 
                                                            ON PAYTYPE.Mcd_Code = Mem_PayrollType
	                                                        AND PAYTYPE.Mcd_Codetype = 'PAYTYPE' 
	                                                        AND PAYTYPE.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl EMPSTATN 
                                                            ON EMPSTATN.Mcd_Code = Mem_EmploymentStatusCode
	                                                        AND EMPSTATN.Mcd_Codetype = 'EMPSTAT' 
	                                                        AND EMPSTATN.Mcd_CompanyCode = '{4}'
                                           LEFT JOIN {7}..M_CodeDtl WORKSTAT 
                                                            ON WORKSTAT.Mcd_Code = Mem_WorkStatus
	                                                        AND WORKSTAT.Mcd_Codetype = 'WORKSTAT' 
	                                                        AND WORKSTAT.Mcd_CompanyCode = '{4}'
                                            WHERE  Mem_LastName+Mem_FirstName+Mem_MiddleName < @EmpName
	                                            AND  Mem_WorkStatus = 'IN'
                                                {8}
                                            ORDER BY Mem_LastName+Mem_FirstName+Mem_MiddleName DESC"
                                                    , EmployeeId
                                                    , SystemID
                                                    , LoginInfo.getUser().UserCode
                                                    , GetCostCenterAccess(SystemID, CompanyCode, LoginInfo.getUser().UserCode, CentralProfile).ToString()
                                                    , CompanyCode
                                                    , LoginInfo.getUser().DBNumber
                                                    , Condition
                                                    , CentralProfile
                                                    , ConditionEx);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetNextEmployee(string EmployeeId, string MenuCode, string CompanyCode, string CentralProfile)
        {
            return GetNextEmployee(EmployeeId, MenuCode, "", CompanyCode, CentralProfile);
        }

        public DataTable GetNextEmployee(string EmployeeId, string MenuCode, string Condition, string CompanyCode, string CentralProfile)
        {
            string SystemID = this.GetSystemID(MenuCode, CompanyCode, CentralProfile);

            string query = string.Format(@"DECLARE @EmpName AS varchar(90)
                                            SET @EmpName = (SELECT Mem_LastName+Mem_FirstName+Mem_MiddleName
		                                                    FROM M_Employee
		                                                    WHERE Mem_IDNo = '{0}')
                                            				
                                            IF @EmpName IS NULL OR @EmpName = ''
                                            BEGIN
                                                SET @EmpName = 'aaa'
                                            END

                                            SELECT TOP(1) Mem_IDNo 
                                                , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
																THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                                , Mem_FirstName as Mem_FirstName
												, Mem_MiddleName as Mem_MiddleName
                                                , Mem_NickName as Mem_NickName
                                                , PAYGRP.Mcd_Name as [Payroll Group]
                                                , PAYTYPE.Mcd_Name as [Payroll Type]
                                                , EMPSTATN.Mcd_Name as [Employment Status]
                                                , WORKSTAT.Mcd_Name as [Work Status]
                                            FROM M_Employee
                                            {6}
                                            INNER JOIN {7}..M_UserExtTmp 
                                                        ON Mue_SystemCode = '{1}'
                                                        AND Mue_UserCode = '{2}'
	                                                    AND Mue_CompanyCode = '{4}'
	                                                    AND Mue_ProfileCode = '{5}'
                                                        AND Mue_CostCenterCode = Mem_CostcenterCode
	                                                    AND Mue_PayrollGroup = Mem_PayrollGroup
	                                                    AND Mue_PayrollType = Mem_PayrollType 
	                                                    AND Mue_EmploymentStatus = Mem_EmploymentStatusCode
                                            LEFT JOIN {7}..M_CodeDtl PAYGRP 
                                                            ON PAYGRP.Mcd_Code = Mem_PayrollGroup
	                                                        AND PAYGRP.Mcd_Codetype = 'PAYGRP' 
	                                                        AND PAYGRP.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl PAYTYPE 
                                                            ON PAYTYPE.Mcd_Code = Mem_PayrollType
	                                                        AND PAYTYPE.Mcd_Codetype = 'PAYTYPE' 
	                                                        AND PAYTYPE.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl EMPSTATN 
                                                            ON EMPSTATN.Mcd_Code = Mem_EmploymentStatusCode
	                                                        AND EMPSTATN.Mcd_Codetype = 'EMPSTAT' 
	                                                        AND EMPSTATN.Mcd_CompanyCode = '{4}'
                                           LEFT JOIN {7}..M_CodeDtl WORKSTAT 
                                                            ON WORKSTAT.Mcd_Code = Mem_WorkStatus
	                                                        AND WORKSTAT.Mcd_Codetype = 'WORKSTAT' 
	                                                        AND WORKSTAT.Mcd_CompanyCode = '{4}'
                                            WHERE  Mem_LastName+Mem_FirstName+Mem_MiddleName > @EmpName
	                                            AND  Mem_WorkStatus NOT IN ('IN','IM')
                                            ORDER BY Mem_LastName+Mem_FirstName+Mem_MiddleName"
                                                    , EmployeeId
                                                    , SystemID
                                                    , LoginInfo.getUser().UserCode
                                                    , GetCostCenterAccess(SystemID, CompanyCode, LoginInfo.getUser().UserCode, CentralProfile).ToString()
                                                    , CompanyCode
                                                    , LoginInfo.getUser().DBNumber
                                                    , Condition
                                                    , CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetNextInactiveEmployee(string EmployeeId, string MenuCode, string Condition, string ConditionEx, string CompanyCode, string CentralProfile)
        {
            string SystemID = this.GetSystemID(MenuCode, CompanyCode, CentralProfile);

            string query = string.Format(@"DECLARE @EmpName AS varchar(90)
                                            SET @EmpName = (SELECT Mem_LastName+Mem_FirstName+Mem_MiddleName
		                                                    FROM M_Employee
		                                                    WHERE Mem_IDNo = '{0}')
                                            				
                                            IF @EmpName IS NULL OR @EmpName = ''
                                            BEGIN
                                                SET @EmpName = 'aaa'
                                            END

                                            SELECT TOP(1) Mem_IDNo 
                                                , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
																THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                                , Mem_FirstName as Mem_FirstName
												, Mem_MiddleName as Mem_MiddleName
                                                , Mem_NickName as Mem_NickName
                                                , PAYGRP.Mcd_Name as [Payroll Group]
                                                , PAYTYPE.Mcd_Name as [Payroll Type]
                                                , EMPSTATN.Mcd_Name as [Employment Status]
                                                , WORKSTAT.Mcd_Name as [Work Status]
                                            FROM M_Employee
                                            {6}
                                            INNER JOIN {7}..M_UserExtTmp 
                                                        ON Mue_SystemCode = '{1}'
                                                        AND Mue_UserCode = '{2}'
	                                                    AND Mue_CompanyCode = '{4}'
	                                                    AND Mue_ProfileCode = '{5}'
                                                        AND Mue_CostCenterCode = Mem_CostcenterCode
	                                                    AND Mue_PayrollGroup = Mem_PayrollGroup
	                                                    AND Mue_PayrollType = Mem_PayrollType 
	                                                    AND Mue_EmploymentStatus = Mem_EmploymentStatusCode
                                            LEFT JOIN {7}..M_CodeDtl PAYGRP 
                                                            ON PAYGRP.Mcd_Code = Mem_PayrollGroup
	                                                        AND PAYGRP.Mcd_Codetype = 'PAYGRP' 
	                                                        AND PAYGRP.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl PAYTYPE 
                                                            ON PAYTYPE.Mcd_Code = Mem_PayrollType
	                                                        AND PAYTYPE.Mcd_Codetype = 'PAYTYPE' 
	                                                        AND PAYTYPE.Mcd_CompanyCode = '{4}' 
                                            LEFT JOIN {7}..M_CodeDtl EMPSTATN 
                                                            ON EMPSTATN.Mcd_Code = Mem_EmploymentStatusCode
	                                                        AND EMPSTATN.Mcd_Codetype = 'EMPSTAT' 
	                                                        AND EMPSTATN.Mcd_CompanyCode = '{4}'
                                           LEFT JOIN {7}..M_CodeDtl WORKSTAT 
                                                            ON WORKSTAT.Mcd_Code = Mem_WorkStatus
	                                                        AND WORKSTAT.Mcd_Codetype = 'WORKSTAT' 
	                                                        AND WORKSTAT.Mcd_CompanyCode = '{4}'
                                           WHERE  Mem_LastName+Mem_FirstName+Mem_MiddleName > @EmpName
	                                            AND Mem_WorkStatus = 'IN'
                                                {8}
                                           ORDER BY Mem_LastName+Mem_FirstName+Mem_MiddleName"
                                                    , EmployeeId
                                                    , SystemID
                                                    , LoginInfo.getUser().UserCode
                                                    , GetCostCenterAccess(SystemID, CompanyCode, LoginInfo.getUser().UserCode, CentralProfile).ToString()
                                                    , CompanyCode
                                                    , LoginInfo.getUser().DBNumber
                                                    , Condition
                                                    , CentralProfile
                                                    , ConditionEx);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public double GetFormulaQueryDecimalValue(string MenuCode, string SubCode, ParameterInfo[] paramInfo, DALHelper dal)
        {
            #region query
            string query = string.Format(@"SELECT Mdm_Formula 
                                            FROM M_Formula
                                            WHERE Mdm_MainCode = '{0}'
                                            AND Mdm_SubCode = '{1}'", MenuCode, SubCode);
            #endregion
            DataTable dtResult;
            double dValue = 0;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                query = dtResult.Rows[0][0].ToString();

                if (query != "")
                {
                    dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                    if (dtResult.Rows.Count > 0)
                    {
                        dValue = Convert.ToDouble(dtResult.Rows[0][0]);
                    }
                }
                else
                    throw new Exception("No equivalent query in Formula Master!");
            }
            return dValue;
        }

        public string GetFormulaQueryStringValue(string MenuCode, string SubCode, ParameterInfo[] paramInfo, DALHelper dal)
        {
            #region query
            string query = string.Format(@"SELECT Mdm_Formula 
                                            FROM M_Formula
                                            WHERE Mdm_MainCode = '{0}'
                                            AND Mdm_SubCode = '{1}'", MenuCode, SubCode);
            #endregion
            DataTable dtResult;
            string strValue = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                query = dtResult.Rows[0][0].ToString();

                if (query != "")
                {
                    dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                    if (dtResult.Rows.Count > 0)
                    {
                        strValue = dtResult.Rows[0][0].ToString();
                    }
                }
                else
                    throw new Exception("No equivalent query in Formula Master!");
            }
            return strValue;
        }

        public string IsolationValidation(DataRow drCurrent, DataRow drLatest, string tableName, string columnName, string columnValue)
        {
            string isolationErrMsg = "";
            bool errFlag = false;
            #region Table Isolation
            if (drCurrent != null)
            {
                //int _notUpdated = 0;
                string _colA = "";
                string _colB = "";

                for (int i = 0; i < drCurrent.Table.Columns.Count && !errFlag; i++)
                {
                    if (drCurrent.Table.Columns[i].DataType == typeof(byte[]))
                    {
                        _colA = Encoding.Unicode.GetString((drCurrent[i] != DBNull.Value) ? (byte[])drCurrent[i] : Encoding.Unicode.GetBytes(" "));
                        _colB = Encoding.Unicode.GetString((drLatest[i] != DBNull.Value) ? (byte[])drLatest[i] : Encoding.Unicode.GetBytes(" "));
                    }
                    else
                    {
                        _colA = drCurrent[i].ToString();
                        _colB = drLatest[i].ToString();
                    }

                    if (_colA != _colB)
                    {
                        errFlag = true;
                        #region Old Message

                        #endregion
                        DataSet ds;
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                string query = string.Format(@"SELECT ludatetime, usr_login FROM {0} WHERE [{1}] = '{2}'", tableName, columnName, columnValue);
                                dal.OpenDB();
                                ds = dal.ExecuteDataSet(query, CommandType.Text);
                            }
                            catch (Exception)
                            {
                                ds = null;
                            }
                            finally
                            {
                                dal.CloseDB();
                            }
                        }
                        string ludatetime = (ds == null) ? "" : ds.Tables[0].Rows[0][0].ToString();
                        string userlogin = (ds == null) ? "" : ds.Tables[0].Rows[0][1].ToString();
                        isolationErrMsg = CommonMessages.FormEditedAndMustReload(ludatetime, userlogin);
                    }
                }
            }
            #endregion
            return isolationErrMsg;
        }

        public string IsolationValidation(DataRow drCurrent, DataRow drLatest, string tableName, params CommonConstants.TableColumnNameValue[] cols)
        {
            string isolationErrMsg = "";
            bool errFlag = false;
            #region Table Isolation
            if (drCurrent != null)
            {
                //int _notUpdated = 0;
                string _colA = "";
                string _colB = "";

                for (int i = 0; i < drCurrent.Table.Columns.Count && !errFlag; i++)
                {
                    if (drCurrent.Table.Columns[i].DataType == typeof(byte[]))
                    {
                        _colA = Encoding.Unicode.GetString((drCurrent[i] != DBNull.Value) ? (byte[])drCurrent[i] : Encoding.Unicode.GetBytes(" "));
                        _colB = Encoding.Unicode.GetString((drLatest[i] != DBNull.Value) ? (byte[])drLatest[i] : Encoding.Unicode.GetBytes(" "));
                    }
                    else
                    {
                        _colA = drCurrent[i].ToString();
                        _colB = drLatest[i].ToString();
                    }

                    if (_colA != _colB)
                    {
                        errFlag = true;
                        DataSet ds;
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                string query = string.Format(@"SELECT ludatetime, usr_login FROM {0} WHERE", tableName);
                                foreach (CommonConstants.TableColumnNameValue col in cols)
                                {
                                    query += string.Format("AND [{0}] = '{1}' ", col.ColumnNameGet, col.ColumnValueGet);
                                }
                                query = query.Replace("whereand", "where");
                                dal.OpenDB();
                                ds = dal.ExecuteDataSet(query, CommandType.Text);
                            }
                            catch (Exception)
                            {
                                ds = null;
                            }
                            finally
                            {
                                dal.CloseDB();
                            }
                        }
                        string ludatetime = (ds == null) ? "" : ds.Tables[0].Rows[0][0].ToString();
                        string userlogin = (ds == null) ? "" : ds.Tables[0].Rows[0][1].ToString();
                        isolationErrMsg = CommonMessages.FormEditedAndMustReload(ludatetime, userlogin);
                    }
                }
            }
            #endregion
            return isolationErrMsg;
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

        public DataSet GetSubCompanyInfoHeader(string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"
							SELECT Mcm_EmployerName AS Mcm_CompanyName 		
							 , Mcm_BusinessAddress AS [Address]			
							 , LTRIM(CASE WHEN LEN(Mcm_EmailAddress) = 0 THEN '' 
							 ELSE 'Email Address: ' + Mcm_EmailAddress END + 
							 CASE WHEN LEN(Mcm_TelNo) = 0 THEN '' ELSE ' Telephone Number: ' + Mcm_TelNo END 			
							 + CASE WHEN LEN(Mcm_CellNo) = 0 THEN '' 
							 ELSE ' Cellular Number: ' + Mcm_CellNo END + 
							 CASE WHEN LEN(Mcm_FaxNo) = 0 THEN '' ELSE ' Fax Number: ' + Mcm_FaxNo END) as Contacts			
							 , Mcm_CompanyLogo
							FROM M_Employer			
							WHERE Mcm_EmployerCode = '{0}'	
							AND Mcm_RecordStatus = 'A'", CompanyCode);
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }
            return ds;
        }

        public string GetCompanyInfoHeader()
        {
            string retString = "";
            DataSet ds = new DataSet();
            ds = GetCompanyInfoHeader(LoginInfo.getUser().CompanyCode, LoginInfo.getUser().CentralProfileName);
            if (ds != null)
            {
                retString = ds.Tables[0].Rows[0]["Mcm_CompanyName"].ToString() + "\n" + ds.Tables[0].Rows[0]["Address"].ToString() + "\n" + ds.Tables[0].Rows[0]["Contacts"].ToString();
            }
            return retString;
        }

        public DataSet GetCompanyDtl(string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@"
                           SELECT Mcm_CompanyCode
                            ,Mcm_CompanyName
                            ,Mcm_CompanyShortName
                            ,CASE WHEN Mcm_EmployerType = 'P' THEN 'PRIVATE'
										WHEN Mcm_EmployerType = 'G' THEN 'GOVERNMENT'
										ELSE Mcm_EmployerType END AS Mcm_EmployerType
                            ,Mcm_Barangay
                            ,Mlh_ZipCode AS Mcm_MunicipalityCity 
                            ,Mcm_BusinessAddress
                            ,Mcm_EmailAddress
                            ,Mcm_TelNo
                            ,Mcm_CellNo
                            ,Mcm_FaxNo
                            ,Mcm_Website
                            ,Mcm_Industry
                            ,Mcm_SSS
                            ,Mcm_Philhealth
                            ,Mcm_PagIbig
                            ,Mcm_TIN
                            ,Mcm_SEC
                            ,Mcm_PEZA
                            ,Mcm_SSSBranchCode
                            ,Mcm_SSSLocatorCode
                            ,Mcm_PhilhealthBranchCode
                            ,Mcm_PagIbigBranchCode
                            ,Mcm_BIRBranchCode
                            ,Mcm_RDOCode
                            ,Mcm_CompanyLogo
                            ,Mcm_ProcessFlag
                            ,Mcm_ProcessInfo			
                            FROM M_Company	
                            LEFT JOIN M_LocationHdr ON Mlh_CompanyCode = Mcm_CompanyCode			
								AND Mlh_LocationCode = Mcm_LocationCode					
                            WHERE Mcm_CompanyCode = '{0}'			
                            AND Mcm_RecordStatus = 'A'", CompanyCode);
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }
            return ds;
        }

        public static bool IsCycleCutOff()
        {
            SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL();
            return SystemCycleProcessingBL.GetProcessFlag("GENERAL", "CYCCUT-OFF");
        }

        public static bool IsPayrollProcessOnGoing()
        {
            SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL();
            return SystemCycleProcessingBL.GetProcessFlag("PAYROLL", "PAYCALC");
        }

        public static bool IsProcessOnGoing(string ProcessCode)
        {
            SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL();
            return SystemCycleProcessingBL.GetProcessFlag("PAYROLL", ProcessCode);
        }

        public DataSet GetProcessControl(string SystemID, string ProcessID, string CentralProfileName)
        {
            string sqlQuery = string.Format(@"
                                SELECT	T_SettingControl.*
                                       , Mur_UserLastName + ', ' + Mur_UserFirstName + ' ' + Mur_UserMiddleName AS [Name]
                                FROM	T_SettingControl 
                                LEFT JOIN {0}..M_User ON Mur_UserCode = T_SettingControl.Usr_Login
                                WHERE	Tsc_SystemCode = '{1}'
                                     AND Tsc_SettingCode = '{2}'"
                                     , CentralProfileName
                                     , SystemID
                                     , ProcessID);
            DataSet ds = null;

            try
            {
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery);
                    dal.CloseDB();
                }
            }
            catch
            {
                CommonProcedures.ShowMessage(10072, SystemID + " - " + ProcessID, "");
            }
            return ds;
        }

        public enum PADDIRECTION
        {
            LEFT = 0,
            RIGHT = 1
        }
        /// <summary>
        /// Pad or sub string with right direction and blank pad character as default
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="length">Length</param>
        /// <returns>Padded or Subbed String</returns>
        public static string PadSubString(string text, int length)
        {
            return PadSubString(text, length, PADDIRECTION.RIGHT);
        }
        /// <summary>
        /// Pad or sub string with a given padded direction and blank pad character as default
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="length">Length</param>
        /// <param name="direction">Padded Direction</param>
        /// <returns>Padded or Subbed String</returns>
        public static string PadSubString(string text, int length, PADDIRECTION direction)
        {
            return PadSubString(text, length, direction, ' ');
        }
        /// <summary>
        /// Pad or sub string with a given padded direction and given padd character
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="length">Length</param>
        /// <param name="direction">Padded Direction</param>
        /// <param name="padd">Pad Character</param>
        /// <returns>Padded or Subbed String</returns>
        public static string PadSubString(string text, int length, PADDIRECTION direction, char padd)
        {
            if (text.Length < length)
            {
                if (direction == PADDIRECTION.LEFT)
                    text = text.PadLeft(length, padd);
                else if (direction == PADDIRECTION.RIGHT)
                    text = text.PadRight(length, padd);
            }
            else
            {
                text = text.Substring(0, length);
            }
            return text;
        }

        public Image resizeImage(Image imgToResize, Size size)
        {
            int destWidth = (int)size.Width;
            int destHeight = (int)size.Height;

            Bitmap tmpBmp = new Bitmap(destWidth, destHeight);
            Graphics tmpGraphics = Graphics.FromImage((Image)tmpBmp);
            tmpGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            tmpGraphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            tmpGraphics.Dispose();

            return (Image)tmpBmp;
        }

        public string GetDatabaseNameByProfileType(string CentralProfile, string CompanyCode, string ProfileType)
        {
            try
            {
                string query = string.Format(@"SELECT TOP 1 Mpf_DatabaseName AS DatabaseName
                                               FROM M_Profile WHERE Mpf_ProfileType = '{0}'", ProfileType);
                DALHelper dalProf = new DALHelper(CentralProfile, false);
                DataTable dt = dalProf.ExecuteDataSet(query).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetDocumentNumber(DALHelper dal, DALHelper dalCentral,string DocumentCode, string ProfileCode)
        {
            string DocumentNo = string.Empty;
            #region Document Prefix
            string query = string.Format(@"
                            UPDATE T_DocumentNumber
                             SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
                             WHERE Tdn_DocumentCode = '{0}'

                            SELECT Tdn_DocumentPrefix
                            , Tdn_LastSeriesNumber FROM T_DocumentNumber
                            WITH (updlock, holdlock)
                            WHERE Tdn_DocumentCode = '{0}'", DocumentCode);

            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            #endregion
            #region Profile Prefix and Company
            string strProfile = string.Format(@"SELECT Mpf_DatabasePrefix, Mpf_CompanyCode FROM M_Profile
                                                WHERE Mpf_RecordStatus = 'A'
                                                    AND Mpf_ProfileType IN ('P','S')
                                                    AND Mpf_DatabaseNo = '{0}'
                                                    AND Mpf_ProfileCategory = '{1}'"
                                            , ProfileCode
                                            , Encrypt.decryptText(System.Configuration.ConfigurationManager.AppSettings["ProfileCategory"].ToString()));

            string ProfilePrefix = dalCentral.ExecuteDataSet(strProfile).Tables[0].Rows[0][0].ToString().Trim();
            string CompanyCode = dalCentral.ExecuteDataSet(strProfile).Tables[0].Rows[0][1].ToString().Trim();
            string COMPYEAR = GetParameterValueFromPayroll("COMPYEAR", CompanyCode, dal).Substring(2);
            #endregion
            DocumentNo = GetValue(dtResult.Rows[0][0].ToString()) + ProfilePrefix + COMPYEAR + dtResult.Rows[0][1].ToString().PadLeft(8, '0');
            return DocumentNo;
        }

        public int GetLastSeriesNumber(DALHelper dal, string DocumentCode)
        {
            string DocumentNo = string.Empty;
            #region Document Prefix
            string query = string.Format(@"
                            UPDATE T_DocumentNumber
                             SET Tdn_LastSeriesNumber = Tdn_LastSeriesNumber + 1
                             WHERE Tdn_DocumentCode = '{0}'

                            SELECT Tdn_LastSeriesNumber FROM T_DocumentNumber
                            WITH (updlock, holdlock)
                            WHERE Tdn_DocumentCode = '{0}'", DocumentCode);

            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            #endregion
            DocumentNo = GetValue(dtResult.Rows[0][0].ToString());
            if (DocumentNo != string.Empty)
                return Convert.ToInt32(DocumentNo);
            else return 0;
        }

        public string GetCodeDetailQuery(string Mcd_CodeType, string Mcd_CompanyCode)
        {
            string query = string.Format(@"
                    SELECT 
                         Mcd_Code [Code],
                         Mcd_Name [Description]
                    FROM M_CodeDtl
                    WHERE Mcd_CodeType = '{0}'
                    AND Mcd_CompanyCode = '{1}'
                    AND Mcd_RecordStatus = 'A'
                    ", Mcd_CodeType, Mcd_CompanyCode);
            return query;
        }

        public DataTable GetCodeDetail(string Mcd_CodeType, string Mcd_CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                    SELECT 
                         Mcd_Code [Code],
                         Mcd_Name [Description]
                    FROM M_CodeDtl
                    WHERE Mcd_CodeType = '{0}'
                    AND Mcd_CompanyCode = '{1}'
                    AND Mcd_RecordStatus = 'A'
                    ", Mcd_CodeType, Mcd_CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public bool GetCodeDetailCode(string Mcd_CodeType, string Mcd_CompanyCode, string Mcd_Code, string CentralProfile)
        {
            string query = string.Format(@"
                    SELECT 
                         Mcd_Code
                    FROM M_CodeDtl
                    WHERE Mcd_CodeType = '{0}'
                        AND Mcd_CompanyCode = '{1}'
                        AND Mcd_Code = '{2}'
                        AND Mcd_RecordStatus = 'A'
                    ", Mcd_CodeType, Mcd_CompanyCode, Mcd_Code);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dtResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool CodeDetailExists(string Mcd_Code, string Mcd_CodeType, string Mcd_CompanyCode, DALHelper dalhelper)
        {
            string query = string.Format(@"
                    SELECT 
                         Mcd_Code [Code],
                         Mcd_Name [Description]
                    FROM M_CodeDtl
                    WHERE Mcd_CodeType = '{1}'
                    AND Mcd_CompanyCode = '{2}'
                    AND Mcd_Code = '{0}'
                    AND Mcd_RecordStatus = 'A'
                    ", Mcd_Code, Mcd_CodeType, Mcd_CompanyCode);
            DataSet ds = new DataSet();
            ds = dalhelper.ExecuteDataSet(query, CommandType.Text);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
        public string GetCodeDetailCodeName(string Mcd_CodeType, string Mcd_CompanyCode, string Mcd_Code, string CentralProfile)
        {
            string query = string.Format(@"
                    SELECT 
                         Mcd_Name
                    FROM M_CodeDtl
                    WHERE Mcd_CodeType = '{0}'
                        AND Mcd_CompanyCode = '{1}'
                        AND Mcd_Code = '{2}'
                        AND Mcd_RecordStatus = 'A'
                    ", Mcd_CodeType, Mcd_CompanyCode, Mcd_Code);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
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

        public bool GetCanViewRate(string userCode, string Company, string Profile, string CentralProfile)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    ret = Convert.ToBoolean(
                        dal.ExecuteScalar(
                            string.Format(@"
                            SELECT Mup_CanViewSalary FROM M_UserProfile
                            INNER JOIN M_Company ON Mup_CompanyCode = Mcm_CompanyCode AND Mcm_RecordStatus = 'A'
                            INNER JOIN M_Profile ON Mup_ProfileCode = Mpf_DatabaseNo AND Mpf_RecordStatus = 'A'
                            WHERE Mup_UserCode = '{0}'
                            AND Mup_CompanyCode = '{1}'
                            AND Mup_ProfileCode = '{2}'
                            AND Mup_RecordStatus = 'A'
                            ", userCode
                            , Company
                            , Profile), CommandType.Text));
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public DataSet fetchUserMenu(string UserCode, string Company, string Profile, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@UserCode", UserCode, SqlDbType.Char, 15);
            paramInfo[1] = new ParameterInfo("@CompanyCode", Company, SqlDbType.Char, 200);
            paramInfo[2] = new ParameterInfo("@ProfileCode", Profile, SqlDbType.Char, 50);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.SelectUserMenu2, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet getUserData(string userCode, string CentralProfile)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                            string.Format(@"
                            select 
	                            Mur_UserLastName
	                            , Mur_UserFirstName
	                            , Mur_UserMiddleName
                            from M_User
                            where Mur_UserCode = '{0}'
                            ", userCode), CommandType.Text);
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public DataSet GetEmployeeNameWithExtension(string IDNo, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"
                                SELECT Mem_IDNo as [ID Number]
	                                , Mem_Lastname +  CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
	                                THEN  ' ' + Mem_ExtensionName 
	                                ELSE '' END + ', ' + Mem_FirstName + ' ' + CASE WHEN '{1}' = 'TRUE' 
	                                THEN Mem_MiddleName ELSE '' END as [Name]
                                FROM M_Employee
                                WHERE Mem_IDNo = '{0}'", IDNo, GetParameterValueFromCentral("ENTRYMNAME", CompanyCode, CentralProfile));
            #endregion
            DataSet dsResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetFormulaFromCentral(string Code, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"
                               SELECT * FROM M_Formula
                                WHERE Mdm_MainCode = '{0}'
                                AND Mdm_CompanyCode = '{1}'
                                AND Mdm_RecordStatus = 'A'"
                                , Code, CompanyCode);
            #endregion
            DataSet dsResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataSet GetFormulaFromPayroll(string Code, DALHelper dal)
        {
            #region query
            string query = string.Format(@"
                               SELECT * FROM M_Formula
                                WHERE Mdm_MainCode LIKE '{0}%'
                                AND Mdm_RecordStatus = 'A'
                               ORDER BY Mdm_SubCode"
                                , Code);
            #endregion
            DataSet dsResult;
            dsResult = dal.ExecuteDataSet(query);
            return dsResult;
        }

        public string GetFormulaFromPayroll(string Code, string SubCode, DALHelper dal)
        {
            #region query
            string query = string.Format(@"
                               SELECT Mdm_Formula FROM M_Formula
                                WHERE Mdm_MainCode LIKE '{0}%'
                                AND Mdm_SubCode = '{1}'
                                AND Mdm_RecordStatus = 'A'
                               ORDER BY Mdm_SubCode"
                                , Code, SubCode);
            #endregion
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else
                return "";
        }

        public string GetParameterFormulaFromPayroll(string PolicyCode, string CompanyCode, DALHelper dal)
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

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Mph_Formula"].ToString();
            }
            else
                throw (new Exception(string.Format("Policy {0} - {1} is not yet setup.", CompanyCode, PolicyCode)));
        }
        
        public DataSet GetPayCycleCurrent(string CentralProfile, string CompanyCode)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    ,Tps_CycleIndicator AS [Cycle Indicator]
                                    ,Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , Tps_PayDate AS [Pay Date]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , CASE WHEN Tps_ComputeTax = 1 THEN 'YES' ELSE 'NO' END AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS 'Tax Schedule Name'        
                                    , Tps_TaxComputation AS [Tax Computation] 
                                    , Tps_StartCycle
                                    , Tps_EndCycle              		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode					
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode				
                                    WHERE Tps_CycleIndicator ='C'
                                        AND Tps_CycleType = 'N'
                                        AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);
           

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleCurrent(string CentralProfile, string CompanyCode, DALHelper dal)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , Tps_PayDate AS [Pay Date]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , CASE WHEN Tps_ComputeTax = 1 THEN 'YES' ELSE 'NO' END AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS 'Tax Schedule Name'        
                                    , Tps_TaxComputation AS [Tax Computation]  
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]   
                                    , Tps_StartCycle
                                    , Tps_EndCycle         		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode					
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode				
                                    WHERE Tps_CycleIndicator ='C'
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetPayCycleCurrent(string CentralProfile, string CompanyCode, string LoginDBName , DALHelper dalCentral)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , Tps_PayDate AS [Pay Date]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , CASE WHEN Tps_ComputeTax = 1 THEN 'YES' ELSE 'NO' END AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS 'Tax Schedule Name'        
                                    , Tps_TaxComputation AS [Tax Computation]  
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]     
                                    , Tps_StartCycle
                                    , Tps_EndCycle       		
                                    FROM {0}..T_PaySchedule						
                                    LEFT JOIN M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode					
                                    LEFT JOIN M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                    LEFT JOIN M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode				
                                    WHERE Tps_CycleIndicator ='C'
                                        AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , LoginDBName);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dalCentral.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetPayCycleCurrentSpecial(string CentralProfile, string CompanyCode)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    ,Tps_CycleIndicator AS [Cycle Indicator]
                                    ,Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , Tps_PayDate AS [Pay Date]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , ISNULL(Tps_ComputeTax,0) AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS [Tax Schedule Name]        
                                    , Tps_TaxComputation AS [Tax Computation] 
                                    , TAXOPT.Mcd_Name AS [Tax Computation Name]
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]  
                                    , Tps_StartCycle
                                    , Tps_EndCycle             		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode					
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode		
                                    LEFT JOIN {0}..M_CodeDtl TAXOPT ON TAXOPT.Mcd_Code = Tps_TaxComputation						
	                                    AND TAXOPT.Mcd_CodeType = 'TAXOPT'					
	                                    AND TAXOPT.Mcd_CompanyCode = @CompanyCode				
                                    WHERE Tps_CycleIndicator = 'S'
                                        AND Tps_CycleIndicatorSpecial = 'C'
                                        AND Tps_RecordStatus = 'A'
                                        AND Tps_CycleType <> 'N'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleCurrentSpecial(string CentralProfile, string CompanyCode, string CycleTypeCode)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , Tps_PayDate AS [Pay Date]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , ISNULL(Tps_ComputeTax,0) AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS [Tax Schedule Name]        
                                    , Tps_TaxComputation AS [Tax Computation] 
                                    , TAXOPT.Mcd_Name AS [Tax Computation Name]
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]  
                                    , Tps_StartCycle
                                    , Tps_EndCycle           		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode					
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode		
                                    LEFT JOIN {0}..M_CodeDtl TAXOPT ON TAXOPT.Mcd_Code = Tps_TaxComputation						
	                                    AND TAXOPT.Mcd_CodeType = 'TAXOPT'					
	                                    AND TAXOPT.Mcd_CompanyCode = @CompanyCode				
                                    WHERE Tps_CycleIndicator = 'S'
                                        AND Tps_CycleIndicatorSpecial = 'C'
                                        AND Tps_RecordStatus = 'A'
                                        AND Tps_CycleType IN ({1})
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile, EncodeFilterItems(CycleTypeCode, true));

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleCurrentSpecialExcludingFinalPay(string CentralProfile, string CompanyCode)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , Tps_PayDate AS [Pay Date]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , ISNULL(Tps_ComputeTax,0) AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS [Tax Schedule Name]        
                                    , Tps_TaxComputation AS [Tax Computation] 
                                    , TAXOPT.Mcd_Name AS [Tax Computation Name]
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]  
                                    , Tps_StartCycle
                                    , Tps_EndCycle  
                                    , Tps_PayDate         		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode					
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode		
                                    LEFT JOIN {0}..M_CodeDtl TAXOPT ON TAXOPT.Mcd_Code = Tps_TaxComputation						
	                                    AND TAXOPT.Mcd_CodeType = 'TAXOPT'					
	                                    AND TAXOPT.Mcd_CompanyCode = @CompanyCode				
                                    WHERE Tps_CycleIndicator = 'S'
                                        AND Tps_CycleIndicatorSpecial = 'C'
                                        AND Tps_RecordStatus = 'A'
                                        AND Tps_CycleType NOT IN ('N','L')
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleDtl(string PayPeriod, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , CONVERT(CHAR(10), Tps_StartAssume, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndAssume, 101) as [Assume Range]
                                    , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                    , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , CYCLEINDIC.Mcd_Name AS [Cycle Indicator Name]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , SPLCYCLEINDIC.Mcd_Name AS [Special Cycle Indicator Name]
                                    , Tps_CycleType AS [Cycle Type]
                                    , DATEDIFF(day,Tps_StartCycle, Tps_EndCycle) [No of Days]
                                    , CONVERT(CHAR(10), Tps_PayDate, 101) [Pay Date]    
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , ISNULL(Tps_ComputeTax,0) AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS [Tax Schedule Name]  
                                    , Tps_TaxComputation [Tax Computation]   
                                    , TAXOPT.Mcd_Name AS [Tax Computation Name]	
                                    , ISNULL(Tps_ComputeSSS,0) AS [Compute SSS]   
                                    , ISNULL(Tps_ComputePHIC,0) AS [Compute PHIC]
                                    , ISNULL(Tps_ComputePagIbig,0) AS [Compute HDMF]
                                    , ISNULL(Tps_ComputeUnion,0) AS [Compute UNION]   
                                    , Tps_MonthEnd AS [Month End]   
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]		
                                    , ISNULL(Tps_AssumeDays,0) AS [Assume Days]
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]
                                    , Tps_StartCycle
                                    , Tps_EndCycle
                                    , Tps_PayDate
                                    , Tps_StartAssume
                                    , Tps_EndAssume
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'						
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode		
                                        AND TAXSCHED.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Tps_CycleIndicator						
	                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                        AND CYCLEINDIC.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl SPLCYCLEINDIC ON SPLCYCLEINDIC.Mcd_Code = Tps_CycleIndicatorSpecial						
	                                    AND SPLCYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND SPLCYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                        AND SPLCYCLEINDIC.Mcd_RecordStatus = 'A'
                                    LEFT JOIN {0}..M_CodeDtl TAXOPT ON TAXOPT.Mcd_Code = Tps_TaxComputation						
	                                    AND TAXOPT.Mcd_CodeType = 'TAXOPT'					
	                                    AND TAXOPT.Mcd_CompanyCode = @CompanyCode
                                        AND TAXOPT.Mcd_RecordStatus = 'A'			
                                    WHERE Tps_PayCycle = @PayCycle
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleDtl(string PayPeriod, string CompanyCode, string CentralProfile, DALHelper dalHelper)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , CONVERT(CHAR(10), Tps_StartAssume, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndAssume, 101) as [Assume Range]
                                    , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                    , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , CYCLEINDIC.Mcd_Name AS [Cycle Indicator Name]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , SPLCYCLEINDIC.Mcd_Name AS [Special Cycle Indicator Name]
                                    , Tps_CycleType AS [Cycle Type]
                                    , DATEDIFF(day,Tps_StartCycle, Tps_EndCycle) [No of Days]
                                    , CONVERT(CHAR(10), Tps_PayDate, 101) [Pay Date]    
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , Tps_Remarks AS [Remarks]
                                    , ISNULL(Tps_ComputeTax,0) AS [Compute Tax]
                                    , Tps_TaxSchedule  AS [Tax Schedule]
                                    , TAXSCHED.Mcd_Name AS [Tax Schedule Name]  
                                    , Tps_TaxComputation [Tax Computation]   
                                    , TAXOPT.Mcd_Name AS [Tax Computation Name]	
                                    , Tps_ComputeSSS AS [Compute SSS]   
                                    , Tps_ComputePHIC AS [Compute PHIC]
                                    , Tps_ComputePagIbig AS [Compute HDMF]
                                    , Tps_ComputeUnion AS [Compute UNION]   
                                    , Tps_MonthEnd AS [Month End]   	
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                    , ISNULL(Tps_AssumeDays,0) AS [Assume Days]
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month] 
                                    , Tps_StartCycle
                                    , Tps_EndCycle
                                    , Tps_PayDate
                                    , Tps_StartAssume
                                    , Tps_EndAssume
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'						
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode	
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                    AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                    AND TAXSCHED.Mcd_CompanyCode = @CompanyCode		
                                        AND TAXSCHED.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Tps_CycleIndicator						
	                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                        AND CYCLEINDIC.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl SPLCYCLEINDIC ON SPLCYCLEINDIC.Mcd_Code = Tps_CycleIndicatorSpecial						
	                                    AND SPLCYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND SPLCYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                        AND SPLCYCLEINDIC.Mcd_RecordStatus = 'A'
                                    LEFT JOIN {0}..M_CodeDtl TAXOPT ON TAXOPT.Mcd_Code = Tps_TaxComputation						
	                                    AND TAXOPT.Mcd_CodeType = 'TAXOPT'					
	                                    AND TAXOPT.Mcd_CompanyCode = @CompanyCode
                                        AND TAXOPT.Mcd_RecordStatus = 'A'			
                                    WHERE Tps_PayCycle = @PayCycle
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);
            ds = dalHelper.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetPayCycleNameDtl(string PayPeriod, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01'))
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , DATEDIFF(day,Tps_StartCycle, Tps_EndCycle) [No of Days]
                                    , CONVERT(CHAR(10), Tps_PayDate, 101) [Pay Date]   
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]  
                                    , Tps_StartCycle
                                    , Tps_EndCycle	
                                    , Tps_PayDate    		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode	
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode		
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    WHERE Tps_PayCycle = @PayCycle
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleDtl2(string PayPeriod, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , DATEDIFF(day,Tps_StartCycle, Tps_EndCycle) [No of Days]
                                    , CONVERT(CHAR(10), Tps_PayDate, 101) [Pay Date]   
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]   
                                    , ISNULL(Tps_AssumeDays,0) AS [Assume Days] 
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]  
                                    , Tps_StartCycle
                                    , Tps_EndCycle 
                                    , Tps_PayDate		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode	
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode		
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    WHERE Tps_PayCycle = @PayCycle
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);
           

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleDtl2(string PayPeriod, string CompanyCode, string CentralProfile, string CycleTypeCode)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , DATEDIFF(day,Tps_StartCycle, Tps_EndCycle) [No of Days]
                                    , CONVERT(CHAR(10), Tps_PayDate, 101) [Pay Date]   
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]   
                                    , ISNULL(Tps_AssumeDays,0) AS [Assume Days] 
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]  
                                    , Tps_StartCycle
                                    , Tps_EndCycle 
                                    , Tps_PayDate		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode	
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode		
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    WHERE Tps_PayCycle = @PayCycle
                                        AND Tps_RecordStatus = 'A'
                                        {1}
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile
                                , (CycleTypeCode == "N" ? "AND Tps_CycleType = 'N'" : "AND Tps_CycleType <> 'N'"));

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleDtl2(string PayPeriod, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , Tps_PayCycle AS [Pay Cycle]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                    , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]
                                    , Tps_CycleIndicator AS [Cycle Indicator]
                                    , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                    , Tps_CycleType AS [Cycle Type]
                                    , CYCLETYPE.Mcd_Name AS [Cycle Type Name]	
                                    , DATEDIFF(day,Tps_StartCycle, Tps_EndCycle) [No of Days]
                                    , CONVERT(varchar(20), Tps_PayDate, 101) [Pay Date]   
                                    , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]  
                                    , ISNULL(Tps_AssumeDays,0) AS [Assume Days]   
                                    , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month] 
                                    , Tps_StartCycle
                                    , Tps_EndCycle	
                                    , Tps_PayDate
                                    , Tps_StartAssume
                                    , Tps_EndAssume
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode	
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode		
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    WHERE Tps_PayCycle = @PayCycle
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", PayPeriod);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public DataTable GetPayCycleStartendDate(string PayCycle)
        {
            string strQuery = string.Format(@"SELECT	Tps_PayCycle
		                                                ,CONVERT(CHAR(10), Tps_StartCycle, 101) as Tps_StartCycle
                                                        ,CONVERT(CHAR(10), Tps_EndCycle, 101) as Tps_EndCycle
                                                FROM	T_PaySchedule 
                                                WHERE	Tps_PayCycle = '{0}' 
                                                AND		Tps_RecordStatus = 'A'", PayCycle);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataSet GetPayCycleStartendDate(string PayCycle, DALHelper dal)
        {
            string strQuery = string.Format(@"SELECT	Tps_PayCycle
		                                                ,CONVERT(CHAR(10), Tps_StartCycle, 101) as Tps_StartCycle
                                                        ,CONVERT(CHAR(10), Tps_EndCycle, 101) as Tps_EndCycle
                                                FROM	T_PaySchedule 
                                                WHERE	Tps_PayCycle = '{0}' 
                                                AND		Tps_RecordStatus = 'A'", PayCycle);

            DataSet ds;
            ds = dal.ExecuteDataSet(strQuery);
            return ds;
        }

        public DataSet GetPayCycleUptoCurrent(string CompanyCode, string CentralProfile, string CycleType)
        {
            string CycleIndicator = "Tps_CycleIndicator";
            string Condition = "";
            if (!CycleType.Equals(string.Empty) && !CycleType.Equals("N"))
            {
                CycleIndicator = "Tps_CycleIndicatorSpecial";
                Condition = "AND Tps_CycleIndicator = 'S'";
            }

            DataSet ds = new DataSet();
            string sqlQuery = string.Format(@"
                                    SELECT Tps_PayCycle AS [Pay Cycle]
                                    , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , CYCLEINDIC.Mcd_Name AS [Cycle Indicator]              		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode		
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = {1}						
	                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode		
                                        AND CYCLEINDIC.Mcd_RecordStatus = 'A'			
                                    WHERE {1} IN ('C','P') 
                                    {2}
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile
                                , CycleIndicator
                                , Condition);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayCycleUptoCurrentWithSpecial(string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Format(@"
                                    SELECT Tps_PayCycle AS [Pay Cycle]
                                    , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                        + ' '+ FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]		
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                    , CONVERT(CHAR(10), Tps_StartCycle, 101) AS [Start Cycle]
                                    , CONVERT(CHAR(10), Tps_EndCycle, 101) AS [End Cycle]
                                    , CYCLEINDIC.Mcd_Name AS [Cycle Indicator]              		
                                    FROM T_PaySchedule						
                                    LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                    AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                    AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                        AND CYCLETYPE.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                    AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                    AND FREQ.Mcd_CompanyCode = @CompanyCode		
                                        AND FREQ.Mcd_RecordStatus = 'A'	
                                    LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Tps_CycleIndicator						
	                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode		
                                        AND CYCLEINDIC.Mcd_RecordStatus = 'A'
                                    LEFT JOIN {0}..M_CodeDtl SPLCYCLEINDIC ON SPLCYCLEINDIC.Mcd_Code = Tps_CycleIndicatorSpecial						
	                                    AND SPLCYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'					
	                                    AND SPLCYCLEINDIC.Mcd_CompanyCode = @CompanyCode		
                                        AND SPLCYCLEINDIC.Mcd_RecordStatus = 'A'			
                                    WHERE Tps_CycleIndicator IN ('C','P','S') 
                                    AND Tps_RecordStatus = 'A'
                                    ORDER BY Tps_PayCycle DESC"
                                , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPrevPayCycleNoFuture(string Tps_PayCycle, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                                    + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]	
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                , ISNULL(Tps_AssumeDays,0) AS [Assume Days]
                                                , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                                , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]  
                                                , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]     
                                                , Tps_StartCycle
                                                , Tps_EndCycle 
                                                , Tps_StartAssume
                                                , Tps_EndAssume                                       
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
												 WHERE Tps_EndCycle = (SELECT DATEADD(dd, -1, Tps_StartCycle)
																		FROM T_PaySchedule
																		WHERE  Tps_PayCycle = @PayCycle
																			AND Tps_RecordStatus = 'A'
																			AND Tps_CycleIndicator NOT IN ('F','S')
																			AND Tps_CycleType = 'N'
                                                                        )						
                                                    AND  Tps_RecordStatus = 'A'
											        AND Tps_CycleIndicator NOT IN ('F','S')
                                                    AND Tps_CycleType = 'N'"
                                        , CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", LoginInfo.getUser().CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetPrevSpecialPayCycleNoFuture(string Tps_PayCycle, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                                    + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]	
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                , ISNULL(Tps_AssumeDays,0) AS [Assume Days]
                                                , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                                , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]  
                                                , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month] 
                                                , Tps_StartCycle
                                                , Tps_EndCycle	
                                                , Tps_StartAssume
                                                , Tps_EndAssume                                            
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
												 WHERE Tps_EndCycle = (SELECT DATEADD(dd, -1, Tps_StartCycle)
																		FROM T_PaySchedule
																		WHERE  Tps_PayCycle = @PayCycle
																			AND Tps_RecordStatus = 'A'
																			AND Tps_CycleIndicator = 'S'
                                                                            AND Tps_CycleIndicatorSpecial <> 'F'
																			AND Tps_CycleType <> 'N'
                                                                        )						
                                                    AND  Tps_RecordStatus = 'A'
											        AND Tps_CycleIndicator = 'S'
                                                    AND Tps_CycleIndicatorSpecial <> 'F'
                                                    AND Tps_CycleType <> 'N'"
                                        , CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetPrevPayCycleWithSpecialNoFuture(string Tps_PayCycle, string CentralProfile)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                                    + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]	
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
												 WHERE Tps_EndCycle = (SELECT DATEADD(dd, -1, Tps_StartCycle)
																		FROM T_PaySchedule
																		WHERE  Tps_PayCycle = @PayCycle
																			AND Tps_RecordStatus = 'A'
																			AND Tps_CycleIndicator <> 'F'
                                                                            AND Tps_CycleType <> 'L')						
                                                    AND Tps_RecordStatus = 'A'
											        AND Tps_CycleIndicator <> 'F'
                                                    AND Tps_CycleType <> 'L'"
                                                , CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", LoginInfo.getUser().CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPrevPayCycleWithSpecialNoFuture(string Tps_PayCycle, string CompanyCode, string CentralProfile, string CycleTypeCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                                    + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]	
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
												 WHERE Tps_EndCycle = (SELECT DATEADD(dd, -1, Tps_StartCycle)
																		FROM T_PaySchedule
																		WHERE  Tps_PayCycle = @PayCycle
																			AND Tps_RecordStatus = 'A'
																			AND Tps_CycleIndicator <> 'F'
                                                                            AND Tps_CycleType = '{1}')						
                                                    AND Tps_RecordStatus = 'A'
											        AND Tps_CycleIndicator <> 'F'
                                                    AND Tps_CycleType = '{1}'"
                                                , CentralProfile
                                                , CycleTypeCode);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetNextPayCycleNoFuture(string Tps_PayCycle, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                                    + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]	
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                , ISNULL(Tps_AssumeDays,0) AS [Assume Days]
                                                , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                                , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]
                                                , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]
                                                , Tps_StartCycle
                                                , Tps_EndCycle	
                                                , Tps_StartAssume
                                                , Tps_EndAssume        
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
                                                WHERE Tps_StartCycle = (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @PayCycle
						                                    AND Tps_RecordStatus = 'A'
                                                            AND Tps_CycleIndicator NOT IN ('F','S')
						                                    AND Tps_CycleType = 'N')
                                                AND  Tps_RecordStatus = 'A'
                                                AND Tps_CycleIndicator NOT IN ('F','S')
                                                AND  Tps_CycleType = 'N'"
                                , CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", LoginInfo.getUser().CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetNextSpecialPayCycleNoFuture(string Tps_PayCycle, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3)
                                                    + ' ' + FREQ.Mcd_name  + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]	
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                , ISNULL(Tps_AssumeDays,0) AS [Assume Days]
                                                , CASE WHEN Tps_StartAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_StartAssume, 101) ELSE '' END as [Start Assume] 
                                                , CASE WHEN Tps_EndAssume IS NOT NULL THEN CONVERT(CHAR(10), Tps_EndAssume, 101) ELSE '' END as [End Assume]
                                                , ISNULL(Tps_Assume13thMonth,'') AS [Assume 13th Month]
                                                , Tps_StartCycle
                                                , Tps_EndCycle	
                                                , Tps_StartAssume
                                                , Tps_EndAssume        
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
                                                WHERE Tps_StartCycle = (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @PayCycle
						                                    AND Tps_RecordStatus = 'A'
                                                            AND Tps_CycleIndicator = 'S'
                                                            AND Tps_CycleIndicatorSpecial <> 'F'
						                                    AND Tps_CycleType <> 'N')
                                                AND  Tps_RecordStatus = 'A'
                                                AND Tps_CycleIndicator = 'S'
                                                AND Tps_CycleIndicatorSpecial <> 'F'
						                        AND Tps_CycleType <> 'N'"
                                , CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            return ds;
        }

        public DataSet GetNextPayCycleWithSpecialNoFuture(string Tps_PayCycle, string CentralProfile)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                                    + ' ' + FREQ.Mcd_name + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' 
                                                    THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 	
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
                                                WHERE Tps_StartCycle = (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @PayCycle
						                                    AND Tps_RecordStatus = 'A'
                                                            AND Tps_CycleIndicator <> 'F'
						                                    AND Tps_CycleType <> 'L'
                                                            )
                                    AND  Tps_RecordStatus = 'A'
                                    AND Tps_CycleIndicator <> 'F'
                                    AND Tps_CycleType <> 'L'
                                    "
                                , CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", LoginInfo.getUser().CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetNextPayCycleWithSpecialNoFuture(string Tps_PayCycle, string CompanyCode, string CentralProfile, string CycleTypeCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT Tps_PayCycle AS [Pay Cycle]	
                                                , LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) 
                                                    + ' ' + FREQ.Mcd_name + ', ' + LEFT(Tps_PayCycle,4) 
                                                    + CASE WHEN Tps_CycleIndicator = 'S' 
                                                    THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]	
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Range]
                                                , CONVERT(CHAR(10), Tps_StartCycle, 101) as [Start Cycle]
                                                , CONVERT(CHAR(10), Tps_EndCycle, 101) as [End Cycle]
                                                , Tps_CycleIndicator AS [Cycle Indicator] 	
                                                , Tps_CycleIndicatorSpecial AS [Special Cycle Indicator]
                                                , ISNULL(Tps_YearEndAdjustment,0) AS [Year End Adjustment]	
                                                , Tps_CycleType AS [Cycle Type]
                                                , Tps_TaxSchedule  AS [Tax Schedule]
                                                FROM T_PaySchedule						
                                                LEFT JOIN {0}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType						
	                                                AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'					
	                                                AND CYCLETYPE.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLETYPE.Mcd_RecordStatus = 'A'				
                                                LEFT JOIN {0}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)						
	                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'					
	                                                AND FREQ.Mcd_CompanyCode = @CompanyCode
                                                    AND FREQ.Mcd_RecordStatus = 'A'
                                                WHERE Tps_StartCycle = (SELECT DATEADD(dd, 1, Tps_EndCycle)
                                                        FROM T_PaySchedule
                                                        WHERE  Tps_PayCycle = @PayCycle
						                                    AND Tps_RecordStatus = 'A'
                                                            AND Tps_CycleIndicator <> 'F'
                                                            AND Tps_CycleType = '{1}'
                                                            )
                                    AND  Tps_RecordStatus = 'A'
                                    AND Tps_CycleIndicator <> 'F'
                                    AND Tps_CycleType = '{1}'
                                    "
                                , CentralProfile
                                , CycleTypeCode);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@PayCycle", Tps_PayCycle);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public int GetFillerDayCodesCount(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string strQuery = string.Format(@"
                                SELECT ISNULL(Count(Mmd_DayCode),0)
                                FROM {1}..M_MiscellaneousDay
                                INNER JOIN {1}..M_Day 
                                    ON Mmd_DayCode = Mdy_DayCode
								    AND Mmd_CompanyCode = Mdy_CompanyCode
                                WHERE Mmd_RecordStatus = 'A' 
                                    AND Mdy_RecordStatus = 'A'
									AND Mmd_CompanyCode = '{0}'", CompanyCode, CentralProfile);

            DataTable dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
            return Convert.ToInt32(dtResult.Rows[0][0]);
        }

        public int GetFillerDayCodesCount(string CompanyCode, string CentralProfile)
        {
            string strQuery = string.Format(@"
                                SELECT ISNULL(Count(Mmd_DayCode),0)
                                FROM M_MiscellaneousDay
                                INNER JOIN M_Day ON Mmd_DayCode = Mdy_DayCode
								AND Mmd_CompanyCode = Mdy_CompanyCode
                                WHERE Mmd_RecordStatus = 'A' 
                                    AND Mdy_RecordStatus = 'A'
									AND Mmd_CompanyCode = '{0}'", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
                dal.CloseDB();
            }
            return Convert.ToInt32(dtResult.Rows[0][0]);
        }

        public DataTable GetDayCodePremiumFillers(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string query = string.Format(@"
                                SELECT Mdp_PremiumGrpCode
                                  , Mdp_PayrollType
                                  , Mdp_DayCode
                                  , Mdp_RestDayFlag
                                  , Mdp_RGPremiumRate
                                  , Mdp_OTPremiumRate
                                  , Mdp_RGNDPremiumRate 
                                  , Mdp_OTNDPremiumRate 
                                  , Mdp_RGNDPercentageRate
                                  , Mdp_OTNDPercentageRate
                                  , Mmd_MiscDayID
                              FROM {1}..M_DayPremium
                              INNER JOIN {1}..M_MiscellaneousDay 
								ON Mdp_DayCode = Mmd_DayCode
								AND M_MiscellaneousDay.Mmd_RestDayFlag = M_DayPremium.Mdp_RestDayFlag
								AND M_MiscellaneousDay.Mmd_CompanyCode = M_DayPremium.Mdp_CompanyCode
                              WHERE Mdp_RecordStatus = 'A'
								AND Mdp_CompanyCode = '{0}'", CompanyCode, CentralProfile);

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodeFillers(string CompanyCode, string CentralProfile)
        {
            string strQueryDayCodeFillers = string.Format(@"
                                                SELECT Mmd_MiscDayID
                                                , Mmd_DayCode
                                                , ISNULL(Mmd_RestDayFlag,0) AS Mmd_RestDayFlag 
                                                , CASE Mmd_RestDayFlag 
													WHEN 1 THEN 'REST ' ELSE '' END 
													+ Mdy_DayName AS [DayName]
                                                FROM M_MiscellaneousDay 
                                                LEFT JOIN M_Day ON Mmd_DayCode = Mdy_DayCode
													AND Mmd_CompanyCode = Mdy_CompanyCode
                                                WHERE Mmd_RecordStatus = 'A'
                                                AND Mmd_CompanyCode = '{0}'", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(strQueryDayCodeFillers).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDayCodeFillers(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string strQueryDayCodeFillers = string.Format(@"
                                                SELECT Mmd_MiscDayID
                                                , Mmd_DayCode
                                                , ISNULL(Mmd_RestDayFlag,0) AS Mmd_RestDayFlag 
                                                FROM {1}..M_MiscellaneousDay 
                                                WHERE Mmd_RecordStatus = 'A'
                                                AND Mmd_CompanyCode = '{0}'", CompanyCode, CentralProfile);

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(strQueryDayCodeFillers).Tables[0];
            return dtResult;
        }

        public DataSet GetEmployeeImage(string IDNo, string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Mem_Image
                                        FROM M_Employee 
                                        WHERE Mem_IDNo = '{0}'
                                        AND Mem_CompanyCode = '{1}'", IDNo, CompanyCode);
            #endregion
            DataSet dsResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return dsResult;
        }

        public DataTable GetAllowanceHeader(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string query = string.Format(@"SELECT Mvh_TimeBaseID
                                   , Mvh_AllowanceCode
                                   , Mvh_AdjustmentCode
                              FROM  {1}..M_VarianceAllowanceHdr
                              WHERE Mvh_RecordStatus = 'A'
                                    AND Mvh_CompanyCode = '{0}'", CompanyCode, CentralProfile);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetCostCenters(string userlogged, string SystemID, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            string accessQuery = @"
                    SELECT
                       DISTINCT Mue_CostCenterCode
                    FROM M_UserExt
                    WHERE Mue_UserCode = '{0}'
                    AND Mue_SystemCode = '{1}'
                    AND Mue_CompanyCode = '{2}'
                    AND Mue_ProfileCode = '{3}'
                    AND Mue_RecordStatus = 'A'";

            accessQuery = string.Format(accessQuery, userlogged, SystemID, CompanyCode, ProfileCode);

            string cond = string.Empty;

            string query = @"
               		 SELECT 
						Mcc_CostCenterCode [Code],
						dbo.Udf_DisplayCostCenterName('{1}',Mcc_CostCenterCode,'{2}') [Description]
					FROM M_CostCenter
					WHERE Mcc_RecordStatus = 'A'
                    {0}
                    AND Mcc_CompanyCode = '{1}'
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                DataTable dt = dal.ExecuteDataSet(accessQuery).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() != "ALL")
                        cond = "AND Mcc_CostCenterCode IN (" + EncodeFilterItems((new HRCReportsBL()).SetUpItemsForEncodeFilterItems(dt)) + ")";
                    query = string.Format(query, cond, CompanyCode, (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));
                    dtResult = dal.ExecuteDataSet(query).Tables[0];
                }
                else
                {
                    dtResult = new DataTable();
                }
                dal.CloseDB();
            }
            return dtResult;

        }

        public bool CheckIfCycleIndicatorIsFuture(string PayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string query = string.Format(@"SELECT Tps_PayCycle
                                FROM T_PaySchedule
                                WHERE Tps_PayCycle = '{0}'
                                AND Tps_CycleIndicator = 'F'", PayCycle);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool IsLastPayCycleType(string PayCycle, DALHelper dal)
        {
            bool IsLastPay = false;
            string query = string.Format(@"SELECT Tps_CycleType  FROM T_PaySchedule WHERE Tps_PayCycle = '{0}'", PayCycle);
            DataSet ds = dal.ExecuteDataSet(query);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Tps_CycleType"].ToString().Trim() == "L")
                    IsLastPay = true;
            }
            return IsLastPay;
        }

        public DataTable FormatDataTableToRemoveZero(DataTable dt, bool bRemoveZero)
        {
            try
            {
                int rowCount = dt.Rows.Count;
                for (int i = dt.Columns.Count - 1; i >= 0; i--)
                {
                    bool hasNonZeroValue = false;
                    bool allZero = false;
                    for (int j = 0; j < rowCount; j++)
                    {
                        try
                        {
                            decimal dValue = Convert.ToDecimal(dt.Rows[j][i].ToString());
                            if (dValue == 0)
                            {
                                if (hasNonZeroValue == false)
                                    allZero = true;
                                if (bRemoveZero)
                                    dt.Rows[j][i] = DBNull.Value;
                            }
                            else if (dValue != 0)
                            {
                                allZero = false;
                                hasNonZeroValue = true;
                                //break;
                            }
                            //else allZero = false;
                        }
                        catch (Exception x)
                        {
                            allZero = false;
                        }
                    }
                    if (allZero)
                    {
                        dt.Columns.RemoveAt(i);
                    }
                }
                dt.AcceptChanges();
                return dt;
            }
            catch
            { return null; }
        }

        public DataTable GetTaxMaster(string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT *
	                            FROM M_Tax
                               WHERE  Mtx_RecordStatus = 'A'
                                    AND Mtx_CompanyCode = '{0}'", CompanyCode);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            
            return dtResult;
        }

        public DataSet GetProfilesPerUserAccess(string UsrLogin, string ProfileCode, string CompanyCode, string CentralProfile, bool bLogin)
        {
            DataSet ds = new DataSet();

            string Condition = string.Format("AND Mpf_DatabaseNo = '{0}'", ProfileCode);

            string sqlQuery = string.Format(@"
                                    SELECT Mpf_DatabaseNo, Mpf_ProfileName, Mpf_CompanyCode, Mcm_CompanyName AS Mpf_CompanyName
										, Mpf_DatabaseName, Mpf_Password, Mpf_ServerName, Mpf_CentralProfile, Mup_DefaultLogin AS Mpf_DefaultLogin
									FROM M_UserProfile
									INNER JOIN M_User ON Mup_UserCode = Mur_UserCode
										AND UPPER(Mup_UserCode) = '{0}'
									INNER JOIN M_Company ON Mcm_CompanyCode = Mup_CompanyCode
										AND Mcm_CompanyCode = '{1}'
									INNER JOIN M_Profile ON Mpf_DatabaseNo = Mup_ProfileCode
                                        AND Mpf_CompanyCode = Mup_CompanyCode
										AND Mpf_RecordStatus = 'A'
										AND Mpf_ProfileCategory = '{2}'
									WHERE Mup_RecordStatus = 'A'
                                    {3}
									ORDER BY Mup_DefaultLogin DESC, Mpf_DatabaseNo
                                    "
                                    , UsrLogin
                                    , CompanyCode
                                    , Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileCategory"].ToString()).ToUpper()
                                    , (bLogin ? Condition : "")
                                    , CentralProfile
             );

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }


        public DataSet GetProfilesPerUserAccess(string UsrLogin, string ProfileCode, string CompanyCode, string CentralProfile, string ModuleCode)
        {
            DataSet ds = new DataSet();

            string sqlQuery = string.Format(@"
                                    SELECT Mpf_ProfileName
							        , Mpf_DatabaseName
                                    , Mpf_DatabaseNo
							        FROM {3}..M_UserDtl 
							        INNER JOIN {3}..M_UserRoleAccess On Mud_SystemCode = Mra_SystemCode
							            AND Mud_CompanyCode = Mra_CompanyCode
							            AND Mud_UserRoleCode = Mra_UserRoleCode
							            AND Mra_ModuleCode = '{2}'
							            AND Mra_RecordStatus = 'A'
							        INNER JOIN {3}..M_Profile ON Mud_ProfileCode = Mpf_DatabaseNo
                                        AND Mpf_CompanyCode = Mud_CompanyCode
								        AND Mpf_RecordStatus = 'A'
								        AND Mpf_ProfileType IN ('P','S')
							        WHERE Mud_UserCode = '{0}'
							            AND  Mud_CompanyCode = '{1}'
							            AND Mud_RecordStatus = 'A'
							            AND 1 = CASE WHEN Mra_CanView = 1 THEN 1 ELSE 0 END
                                    "
                                    , UsrLogin
                                    , CompanyCode
                                    , ModuleCode
                                    , CentralProfile);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }

        public DataTable GetHolidays(string PayCycleCode, string CompanyCode, string CentralProfile)
        {
            try
            {
                DataTable dtResult = new DataTable();
                #region Query
                    string query = @"
                        SELECT CONVERT(CHAR(10), Thl_HolidayDate, 101) as [Holiday Date]	
	                    , Thl_HolidayCode  as [Holiday Code]
						, Thl_HolidayName  as [Holiday Name]		
	                    , Thl_PrevDayHourRequired as [Required hours on Prior day]
	                    , ISNULL(STUFF((SELECT  ', ' + Mcd_Name as 'data()' FROM {1}.dbo.Udf_Split(Thl_LocationCode,',')
												LEFT JOIN {1}..M_CodeDtl ON Mcd_Code = Data
													AND Mcd_Codetype = 'ZIPCODE' 
													AND Mcd_CompanyCode = '{2}' 
												FOR XML PATH('')), 1, 1,''),'ALL')  as [Location]

                    FROM {1}..T_Holiday
                    INNER JOIN {1}..M_Day ON Thl_HolidayCode = Mdy_DayCode
                    AND  Thl_CompanyCode = Mdy_CompanyCode
                    INNER JOIN T_PaySchedule ON Thl_HolidayDate BETWEEN Tps_StartCycle AND Tps_EndCycle
	                    AND Tps_CycleType = 'N'
	                    AND Tps_PayCycle = '{0}'
                    WHERE Thl_CompanyCode = '{2}'
                        AND Mdy_HolidayFlag = 1";
                    #endregion
                query = string.Format(query, PayCycleCode, CentralProfile, CompanyCode);

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    dtResult = dal.ExecuteDataSet(query).Tables[0];
                    dal.CloseDB();
                }
                return dtResult;
            }
            catch
            {
                return null;
            }
        }


        public DataTable GetOvertimeAndNightPremium(string PremiumGroupCode, string PayrollType, string CompanyCode, string CentralProfile)
        {
            string condition = "";
            if (!PremiumGroupCode.Equals(""))
                condition = string.Format("AND Mdp_PremiumGrpCode = '{0}'", PremiumGroupCode);

            string query = string.Format(@"
               SELECT  Mdy_dayname + case when Mdp_RestDayFlag = 1 
                    AND Mdp_DayCode <> 'REST' then ' FALLING ON A RESTDAY' else '' end as [Day]	
	                , Mdp_RGPremiumRate as [Basic Rate]	
	                , Mdp_RGNDPremiumRate as [Basic ND Basis]		
	                , Mdp_RGNDPercentageRate as  [Basic ND Rate] 	
                    , Mdp_OTPremiumRate as [Overtime Rate]	
	                , Mdp_OTNDPremiumRate as [Overtime ND Basis] 
	                , Mdp_OTNDPercentageRate as [Overtime ND Rate]	
                FROM M_DayPremium	
                LEFT JOIN M_day on Mdy_CompanyCode = Mdp_CompanyCode	
	                AND Mdy_daycode = Mdp_DayCode 	
                WHERE Mdp_CompanyCode = '{0}'
	                AND Mdp_PayrollType = '{2}'
	                AND Mdp_RecordStatus = 'A'
                    {1}
                ORDER BY  Mdp_SequenceOfDisplay"
                       , CompanyCode
                       , condition
                       , PayrollType);

            DataSet ds;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetOvertimeAndNightPremium(string PayrollType, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                SELECT  Mdp_PremiumGrpCode as [Premium Group]
                    , Mdy_dayname + case when Mdp_RestDayFlag = 1 
                        AND Mdp_DayCode <> 'REST' then ' FALLING ON A RESTDAY' else '' end as [Day]	
	                , Mdp_RGPremiumRate as [Basic Rate]	
	                , Mdp_OTPremiumRate as [Basic ND Basis]	
	                , Mdp_RGNDPremiumRate as [Basic ND Rate]	
	                , Mdp_OTNDPremiumRate as [Overtime Rate]	
	                , Mdp_RGNDPercentageRate as [Overtime ND Basis]	
	                , Mdp_OTNDPercentageRate as [Overtime ND Rate]	
                FROM M_DayPremium	
                LEFT JOIN M_day on Mdy_CompanyCode = Mdp_CompanyCode	
	                AND Mdy_daycode = Mdp_DayCode 	
                WHERE Mdp_CompanyCode = '{0}'
	                AND Mdp_PayrollType = '{1}'
	                AND Mdp_RecordStatus = 'A'
                ORDER BY  Mdp_PremiumGrpCode, Mdp_SequenceOfDisplay"
                       , CompanyCode
                       , PayrollType);

            DataSet ds;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataTable GetProfileDetails(string DataBaseNo, DALHelper dalCentral)
        {
            string strQuery = string.Format(@"SELECT A.Mpf_DatabaseName AS [Target Payroll]
                                            , ISNULL(A.Mpf_CentralProfile,'') AS [Target Central]
                                            , ISNULL(A.Mpf_SourceProfile,'') AS [Source Payroll]
											, ISNULL(B.Mpf_CentralProfile,'') AS [Source Central]
                                            , B.Mpf_ProfileType AS [Source Profile Type]
                                            , B.Mpf_RestorePath AS [Source Restore Path]
                                            FROM M_Profile A
                                            LEFT JOIN M_Profile B
                                            ON A.Mpf_SourceProfile = B.Mpf_DatabaseName
                                            WHERE A.Mpf_DatabaseNo = '{0}'
                                            AND A.Mpf_RecordStatus = 'A'
                                            AND A.Mpf_ProfileCategory = '{1}'"
                                            , DataBaseNo
                                            , Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileCategory"].ToString()));

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        public string GetNameDisplay(string IDNumber, string NAMEDSPLY)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT CASE WHEN '{1}' = 'C' THEN Mem_Lastname + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_Firstname + ' ' + Mem_Middlename
	                                            WHEN '{1}' = 'F' THEN Mem_Firstname
	                                            WHEN '{1}' = 'M' THEN Mem_Lastname + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_Firstname + ' ' + CASE WHEN LEN(RTRIM(Mem_Middlename)) = 0 THEN '' ELSE LEFT(Mem_Middlename, 1) + '.' END
	                                            WHEN '{1}' = 'S' THEN Mem_NickName
	                                            WHEN '{1}' = 'X' THEN Mem_Firstname + ' ' + CASE WHEN LEN(RTRIM(Mem_Middlename)) = 0 THEN ' ' ELSE LEFT(Mem_Middlename, 1) + '. ' END +  Mem_Lastname + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 THEN  ' ' + Mem_ExtensionName ELSE '' END
	                                            ELSE ' ' END as [Name]
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'", IDNumber, NAMEDSPLY);

            #endregion
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["Name"].ToString().Trim();
            else
                return string.Empty;
        }

        
        public string FixPath(string Path)
        {
            string tPath;
            if (Path.Substring(Path.Length - 1, 1) != "\\")
            {
                tPath = Path + "\\";
            }
            else
            {
                tPath = Path;
            }
            return tPath;
        }

        public DataTable Pivot(DataTable tbl)
        {
            var tblPivot = new DataTable();
            tblPivot.Columns.Add(tbl.Columns[0].ColumnName, typeof(string));
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                tblPivot.Columns.Add(tbl.Rows[i][0].ToString(), typeof(decimal));
            }
            for (int col = 1; col < tbl.Columns.Count; col++)
            {
                DataRow dr = tblPivot.NewRow();
                dr[0] = tbl.Columns[col].ColumnName.ToString();
                for (int j = 0; j < tbl.Rows.Count; j++)
                {
                    dr[j + 1] = tbl.Rows[j][col];
                }
                tblPivot.Rows.Add(dr);
            }
            return tblPivot;
        }

        public string JoinEmployeesFromDataTableArray(DataTable dtData, bool bWithQuotes)
        {
            string strListCommaDelimited = "";
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (bWithQuotes == true)
                    strListCommaDelimited += string.Format(@"'{0}',", GetValue(dtData.Rows[i][0]));
                else
                    strListCommaDelimited += string.Format(@"{0},", GetValue(dtData.Rows[i][0]));
            }
            if (strListCommaDelimited != "")
                strListCommaDelimited = strListCommaDelimited.Substring(0, strListCommaDelimited.Length - 1);

            return strListCommaDelimited;
        }

        public string JoinEmployeesFromStringArray(string[] strArrData, bool bWithQuotes)
        {
            string strEmployeeListCommaDelimited = "";
            foreach (string strData in strArrData)
            {
                if (bWithQuotes == true)
                    strEmployeeListCommaDelimited += string.Format(@"'{0}',", strData);
                else
                    strEmployeeListCommaDelimited += string.Format(@"{0},", strData);
            }
            if (strEmployeeListCommaDelimited != "")
                strEmployeeListCommaDelimited = strEmployeeListCommaDelimited.Substring(0, strEmployeeListCommaDelimited.Length - 1);

            return strEmployeeListCommaDelimited;
        }
    }
}
