

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class MultiProfileBL : BaseBL
    {
        private static string _bakDataSource;
        private static string _bakCentralDBName;
        private static string _bakUserID;
        private static string _bakPassword;

        #region Data Access

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

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT *
                           FROM   M_Profile";

            System.Data.DataSet dset = new System.Data.DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dset = dal.ExecuteDataSet(sql);
                }
                catch (Exception ex)
                {
                    throw new PayrollException(ex);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            Restore();

            return dset;
        }

        public System.Data.DataSet FetchAllActive()
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT *
                           FROM   M_Profile
                           WHERE  Mpf_RecordStatus = 'A'";

            System.Data.DataSet dset = new System.Data.DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dset = dal.ExecuteDataSet(sql);
                }
                catch (Exception ex)
                {
                    throw new PayrollException(ex);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            Restore();

            return dset;
        }

        private static System.Data.DataSet FetchProfile(string databaseNo, string CentralProfile)
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT *
                           FROM   M_Profile
                           WHERE  Mpf_RecordStatus = 'A' AND
                                  Mpf_DatabaseNo = '" + databaseNo + @"'";

            System.Data.DataSet dset = new System.Data.DataSet();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    dset = dal.ExecuteDataSet(sql);
                }
                catch
                {
                    throw new PayrollException("System cannot access the database.");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            Restore();

            return dset;
        }

        public System.Data.DataSet FetchProfile2(string databaseNo, string CentralProfile)
        {
            return FetchProfile(databaseNo, CentralProfile);
        }

        public System.Data.DataSet FetchAuthorizedProfiles(string userCode)
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT     Mup_ProfileCode,
                                      Mpf_ProfileName
                           FROM       M_UserProfile
                           INNER JOIN M_Profile
                           ON         Mpf_DatabaseNo = Mup_ProfileCode
                           AND        Mpf_RecordStatus = 'A'
                           WHERE      Mup_RecordStatus = 'A' AND
                                      Mup_UserCode = '" + userCode + @"'";

            System.Data.DataSet dset = new System.Data.DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dset = dal.ExecuteDataSet(sql);
                }
                catch (Exception ex)
                {
                    throw new PayrollException(ex);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            Restore();

            return dset;
        }

        public System.Data.DataSet FetchDatabases()
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT
	                            Mpf_DatabaseNo
	                            , Mpf_DatabaseName
	                            , Mpf_RecordStatus
                            FROM M_Profile
                            WHERE Mpf_RecordStatus = 'A'";

            System.Data.DataSet dset = new System.Data.DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dset = dal.ExecuteDataSet(sql);
                }
                catch (Exception ex)
                {
                    throw new PayrollException(ex);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            Restore();

            return dset;
        }

        #endregion

        public static void ActivateProfileDB()
        {
            ConfigurationManager.AppSettings["DataSource"]     = ConfigurationManager.AppSettings["DataSource"];
            ConfigurationManager.AppSettings["CentralDBName"]  = ConfigurationManager.AppSettings["CentralDBName"];
            ConfigurationManager.AppSettings["UserID"]         = ConfigurationManager.AppSettings["UserID"];
            ConfigurationManager.AppSettings["Password"]       = ConfigurationManager.AppSettings["Password"];
        }

        public static void ActivateDB(string databaseNo, string CentralProfile)
        {
            System.Data.DataSet dset = FetchProfile(databaseNo, CentralProfile);

            if (dset == null)
            {
                throw new PayrollException("Could not find profile database.");
            }
            if (dset.Tables[0].Rows.Count < 1)
            {
                throw new PayrollException("Could not find profile record.");
            }

            System.Data.DataRow drow = dset.Tables[0].Rows[0];
            ConfigurationManager.AppSettings["DataSource"]      = drow["Mpf_ServerName"].ToString();
            ConfigurationManager.AppSettings["CentralDBName"]   = drow["Mpf_DatabaseName"].ToString();
            ConfigurationManager.AppSettings["UserID"]          = drow["Mpf_UserID"].ToString();
            ConfigurationManager.AppSettings["Password"]        = drow["Mpf_Password"].ToString();

            // For Debug only (Do this if data is not yet encrypted):
#if true
            ConfigurationManager.AppSettings["DataSource"]      = Encrypt.encryptText(ConfigurationManager.AppSettings["DataSource"]);
            ConfigurationManager.AppSettings["CentralDBName"]   = Encrypt.encryptText(ConfigurationManager.AppSettings["CentralDBName"]);
            ConfigurationManager.AppSettings["UserID"]          = Encrypt.encryptText(ConfigurationManager.AppSettings["UserID"]);
            ConfigurationManager.AppSettings["Password"]        = Encrypt.encryptText(ConfigurationManager.AppSettings["Password"]);
#endif
        }

        public static void Backup()
        {
            _bakDataSource      = ConfigurationManager.AppSettings["DataSource"];
            _bakCentralDBName   = ConfigurationManager.AppSettings["CentralDBName"];
            _bakUserID          = ConfigurationManager.AppSettings["UserID"];
            _bakPassword        = ConfigurationManager.AppSettings["Password"];

        }

        public static void Restore()
        {
            ConfigurationManager.AppSettings["DataSource"]      = _bakDataSource;
            ConfigurationManager.AppSettings["CentralDBName"]   = _bakCentralDBName;
            ConfigurationManager.AppSettings["UserID"]          = _bakUserID;
            ConfigurationManager.AppSettings["Password"]        = _bakPassword;
        }
    }
}
