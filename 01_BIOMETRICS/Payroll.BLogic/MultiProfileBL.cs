//===========================================================
//
// Copyright (C) 2010 N-Pax Corporation
//
// Author: Clark Paul Tabasa
// Date  : August 10, 2010
//
//===========================================================

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
        private static string _bakDBNameNonConfi;
        private static string _bakDBNameConfi;
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

        private static System.Data.DataSet FetchProfile(string databaseNo)
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT *
                           FROM   M_Profile
                           WHERE  Mpf_RecordStatus = 'A' AND
                                  Mpf_DatabaseNo = '" + databaseNo + @"'";

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

        public System.Data.DataSet FetchAuthorizedProfiles(string userCode)
        {
            Backup();
            ActivateProfileDB();

            string sql = @"SELECT     Upt_DatabaseNo,
                                      Mpf_ProfileName
                           FROM       M_UserProfile
                           INNER JOIN M_Profile
                           ON         Mpf_DatabaseNo = Upt_DatabaseNo
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

        #endregion

        public static void ActivateProfileDB()
        {
            ConfigurationManager.AppSettings["DataSource"]     = ConfigurationManager.AppSettings["ProfileDataSource"];
            ConfigurationManager.AppSettings["DBNameNonConfi"] = ConfigurationManager.AppSettings["ProfileDBName"];
            ConfigurationManager.AppSettings["DBNameConfi"]    = ConfigurationManager.AppSettings["ProfileDBName"];
            ConfigurationManager.AppSettings["UserID"]         = ConfigurationManager.AppSettings["ProfileUserID"];
            ConfigurationManager.AppSettings["Password"]       = ConfigurationManager.AppSettings["ProfilePassword"];
        }

        public static void ActivateDB(string databaseNo)
        {
            System.Data.DataSet dset = FetchProfile(databaseNo);

            if (dset == null)
            {
                throw new PayrollException("Could not find profile database.");
            }
            if (dset.Tables[0].Rows.Count < 1)
            {
                throw new PayrollException("Could not find profile record.");
            }

            System.Data.DataRow drow = dset.Tables[0].Rows[0];
            ConfigurationManager.AppSettings["DataSource"]     = drow["Mpf_ServerName"].ToString();
            ConfigurationManager.AppSettings["DBNameNonConfi"] = drow["Mpf_DatabaseName"].ToString();
            ConfigurationManager.AppSettings["DBNameConfi"]    = drow["Mpf_DatabaseName"].ToString();
            ConfigurationManager.AppSettings["UserID"]         = drow["Mpf_UserID"].ToString();
            ConfigurationManager.AppSettings["Password"]       = drow["Mpf_Password"].ToString();

            // For Debug only (Do this if data is not yet encrypted):
#if true
            ConfigurationManager.AppSettings["DataSource"]     = Encrypt.encryptText(ConfigurationManager.AppSettings["DataSource"]);
            ConfigurationManager.AppSettings["DBNameNonConfi"] = Encrypt.encryptText(ConfigurationManager.AppSettings["DBNameNonConfi"]);
            ConfigurationManager.AppSettings["DBNameConfi"]    = Encrypt.encryptText(ConfigurationManager.AppSettings["DBNameConfi"]);
            ConfigurationManager.AppSettings["UserID"]         = Encrypt.encryptText(ConfigurationManager.AppSettings["UserID"]);
            ConfigurationManager.AppSettings["Password"]       = Encrypt.encryptText(ConfigurationManager.AppSettings["Password"]);
#endif
        }

        public static void Backup()
        {
            _bakDataSource     = ConfigurationManager.AppSettings["DataSource"];
            _bakDBNameNonConfi = ConfigurationManager.AppSettings["DBNameNonConfi"];
            _bakDBNameConfi    = ConfigurationManager.AppSettings["DBNameConfi"];
            _bakUserID         = ConfigurationManager.AppSettings["UserID"];
            _bakPassword       = ConfigurationManager.AppSettings["Password"];

        }

        public static void Restore()
        {
            ConfigurationManager.AppSettings["DataSource"]     = _bakDataSource;
            ConfigurationManager.AppSettings["DBNameNonConfi"] = _bakDBNameNonConfi;
            ConfigurationManager.AppSettings["DBNameConfi"]    = _bakDBNameConfi;
            ConfigurationManager.AppSettings["UserID"]         = _bakUserID;
            ConfigurationManager.AppSettings["Password"]       = _bakPassword;
        }
    }
}
