using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Data;

using Payroll.BLogic;
using Payroll.DAL;
using CommonLibrary;


namespace Payroll.CFC
{
    public class genericDBCombobox : System.Windows.Forms.ComboBox
    {
        // Still has to improve on the caching.
        private CommonEnum.GenericCBType genericDBType = CommonEnum.GenericCBType.CURRENCY;
        private bool isRequired = false;
        private static DataSet currencyDataSet = null;
        private static DataSet unitOfMeasureDataSet = null;
        private static DataSet userMasterDataSet = null;
        private static DataSet stockTypeMasterDataSet = null;
        private static DataSet productMasterDataSet = null;//Arthur Inserted 200608289AM
        private static DataSet psGroupDataSet = null;
        private static DataSet payTermsDataSet = null;
        private static DataSet deliveryTermsDataSet = null;
        private static DataSet dsCalendarScope = null;
        private static DataSet PackageType = null;
        private static DataSet PackageTypeDesc = null;
        private static DataSet ProductType = null;
        private static DataSet PRODUCT_TYPE = null;
        private static DataSet PackageTypeyIELD = null;
        private static DataSet PRODUCTIONPACKAGE = null;
        private static DataSet PRODUCTIONPRODUCT = null;
        private static DataSet PRODUCTIONPROCESS = null;
        private static DataSet BLOODTYPE = null;
        private static DataSet Payrate = null;////reynard inserted 20081230
        private static string formName = "";
        private static DataSet DownloadDate = null;
        private static DataSet dsPositionCombobox = null;
        private static DataSet GENDER = null;
        private static DataSet TAXSCHED = null;
        private static DataSet CIVILSTATUS = null;
        private static DataSet EMPLOYMENTSTATUS = null;
        private static DataSet WORKTYPE = null;
        private static DataSet JOBSTATUS = null;
        private static DataSet PAYMENTMODE = null;
        private static DataSet PAYMENTTYPE = null;
        private static DataSet TAXCODE = null;
        private static DataSet TaxClass = null;
        private static DataSet TRUEORFALSE = null;
        private static DataSet POSITION_COMBOBOX = null;
        private static DataSet LEAVETYPE_COMBOX = null;
        private static DataSet TAXCODENEW = null;
        private static DataSet EDUC_LEVEL = null;
        private static DataSet EDUC_LEVELWBLNK = null;
        private static DataSet PAYROLLTYPE = null;
        private static DataSet JOBSTATUSNEW = null;
        private static DataSet WORKTYPENEW = null;
        private static DataSet HDMFCODE = null;
        private static DataSet SearchEmployStat = null;
        private static DataSet SearchWrkTyp = null;
        private static DataSet SearchJobStat = null;
        private static DataSet SearchGender = null;
        private static DataSet MENUTYPE = null;
        private static DataSet MENUTYPEALL = null;
        private static DataSet GENDERWDBLNK = null;
        private static DataSet CIVILSTATUSWDBLNK = null;
        private static DataSet DEDUCTIONTYPE = null;
        private static DataSet PREMIUMCODES = null;
        private static DataSet DEDUCTTYPENOALL = null;
        private static DataSet SpecialPayPeriod = null;
        private static DataSet ForBonusComputationPayPeriod = null;
        private static DataSet YearList = null;
        private static DataSet PastAndCurrentPayPeriod = null;
        private static DataSet BILLABLE = null;
        private static DataSet CCTRTYPE = null;
        private static DataSet relational = null;

        public CommonEnum.GenericCBType DBType
        {
            get
            {
                return genericDBType; 
            }
            set
            {
                genericDBType = value;
            }
        }

        public bool ValueRequired
        {
            get
            {
                return isRequired;
            }
            set
            {
                isRequired = value;
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            try
            {
                string strFrmName = GetFormName(this.Parent);
                if (formName != strFrmName)
                {
                    currencyDataSet = null;
                    unitOfMeasureDataSet = null;
                    userMasterDataSet = null;
                }
                formName = strFrmName;
                DataSet ds = GetComboBoxDataSet();
                DataView dtv = ds.Tables[0].DefaultView;
                dtv.Sort = this.DisplayMember.ToString() + " ASC ";
                this.DataSource = dtv;
                this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                //if (!ValueRequired) this.Items.Insert(0, "");

            }
            catch { }
        }

        private DataSet GetComboBoxDataSet()
        {
            this.DisplayMember = "DisplayMember";
            this.ValueMember = "ValueMember";

            switch (genericDBType)
            {
                case CommonEnum.GenericCBType.POSITION_COMBOBOX:
                    {
                        string sql;
                        sql = CommonConstants.Queries.PositionComboBoxQuery;
                        return generateComboData(CommonConstants.GenericDBFields.PositionComboBoxDisp, CommonConstants.GenericDBFields.PositionComboBoxVal, ref dsPositionCombobox, sql);
                    }
               case CommonEnum.GenericCBType.Calendar_Scope:
                   {
                       string query;
                       query = CommonConstants.Queries.CalendarScope;
                       return generateComboData(CommonConstants.GenericDBFields.CalendarScopeDisplayField, CommonConstants.GenericDBFields.CalendarScopeValueField, ref dsCalendarScope, query);
                   }
                case CommonEnum.GenericCBType.STATUS:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.ACTIVE, CommonEnum.Status.FULFILLED, CommonEnum.Status.ONHOLD, CommonEnum.Status.CANCELLED, CommonEnum.Status.NEW });
                    }
                case CommonEnum.GenericCBType.STATUS1://Arthur Inserted 2006082909AM Start
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.APPROVED, CommonEnum.Status.FULFILLED, CommonEnum.Status.ONHOLD, CommonEnum.Status.CANCELLED, CommonEnum.Status.NEW,CommonEnum.Status.REVIEWED });// 03/01/2007:Eugene :added Review
                    }//end
                case CommonEnum.GenericCBType.AFCO_STATUS:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.ACTIVE, CommonEnum.Status.FULFILLED, CommonEnum.Status.CANCELLED, CommonEnum.Status.ONHOLD });
                    }
                case CommonEnum.GenericCBType.PAYTYPE_COMBOBOX://jan added 20080529
                    {
                        return generateComboData(new Enum[] { CommonEnum.PaymentType.ADVANCED, CommonEnum.PaymentType.DEFERRED});
                    }//end add
                case CommonEnum.GenericCBType.STATUS_NC:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.NEW, CommonEnum.Status.CANCELLED });
                    }
                case CommonEnum.GenericCBType.STATUS_ANC:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.APPROVED, CommonEnum.Status.NEW, CommonEnum.Status.CANCELLED });
                    }
                case CommonEnum.GenericCBType.STATUS_NFOC:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.NEW, CommonEnum.Status.FULFILLED, CommonEnum.Status.ONHOLD, CommonEnum.Status.CANCELLED });
                    }
                case CommonEnum.GenericCBType.STATUS_AC:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.ACTIVE, CommonEnum.Status.CANCELLED });
                    }
                case CommonEnum.GenericCBType.STATUS_AOC:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.ACTIVE, CommonEnum.Status.ONHOLD, CommonEnum.Status.CANCELLED });
                    }
                case CommonEnum.GenericCBType.STATUS_AOCF:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.ACTIVE, CommonEnum.Status.ONHOLD, CommonEnum.Status.CANCELLED, CommonEnum.Status.GENERATED });
                    }
                case CommonEnum.GenericCBType.SHIPPING:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Shipping.AIR, CommonEnum.Shipping.LAND, CommonEnum.Shipping.SEA });
                    }
                case CommonEnum.GenericCBType.COR_ACT_STAT:
                    {
                        return generateComboData(new Enum[] { CommonEnum.CorrectiveActConfirm.Accepted, CommonEnum.CorrectiveActConfirm.Rejected });
                    }
                case CommonEnum.GenericCBType.ORIGIN:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Origin.FOREIGN, CommonEnum.Origin.LOCAL });
                    }
                case CommonEnum.GenericCBType.USAGE_TYPE:
                    {
                        return generateComboData(new Enum[] { CommonEnum.UsageType.COMMON, CommonEnum.UsageType.DEDICATED });
                    }
                case CommonEnum.GenericCBType.BALANCE_CATEGORY:
                    {
                        return generateComboData(new Enum[] { CommonEnum.CriticalBalanceCategory.LOW, CommonEnum.CriticalBalanceCategory.MID, CommonEnum.CriticalBalanceCategory.HIGH });
                    }
                case CommonEnum.GenericCBType.PARTS_GEN:
                    {
                        return generateComboData(new Enum[] { CommonEnum.PartsGen.GENERATED, CommonEnum.PartsGen.NOT_YET_GENERATED });
                    }
                case CommonEnum.GenericCBType.STATUS_MASTER:
                    {
                        return generateComboData(new Enum[] { CommonEnum.Status.ACTIVE, CommonEnum.Status.CANCELLED, CommonEnum.Status.ALL });
                    }
                case CommonEnum.GenericCBType.DATEINDICATOR:
                    {
                        return generateComboData(new Enum[] { CommonEnum.DateIndicator.PAST, CommonEnum.DateIndicator.CURRENT, CommonEnum.DateIndicator.FUTURE, CommonEnum.DateIndicator.SPECIAL});
                    }
                case CommonEnum.GenericCBType.TrainingType: //Shane added 06/09/2008
                    {
                        return generateComboData(new Enum[] { CommonEnum.TrainingType.Internal, CommonEnum.TrainingType.External });
                    }//end
                case CommonEnum.GenericCBType.ScheduleType: //Shane added 06/17/2008
                    {
                        return generateComboData(new Enum[] { CommonEnum.ScheduleType.MORNING, CommonEnum.ScheduleType.AFTERNOON, CommonEnum.ScheduleType.GRAVEYARD });
                    }
                case CommonEnum.GenericCBType.NightPremium:
                    {
                        return generateComboData(new Enum[] { CommonEnum.NightPremium.OVERTIME, CommonEnum.NightPremium.REGULAR });
                    }
                case CommonEnum.GenericCBType.Payrate://reynard
                    {
                        string sql;
                        #region sql
                        sql = @"select Adt_AccountDesc as Rate, Adt_Accountcode as BValue from t_accountdetail where adt_accounttype='PAYRATE' and adt_status='A'";
                        #endregion
                        return generateComboData("Rate","BValue",ref Payrate, sql);
                    }

                case CommonEnum.GenericCBType.BLOODTYPE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT '' as [bloodtype], 'BLNK' as [BValue]
                                UNION 
                                SELECT 'A+' as [bloodtype], 'A+' as [BValue]
                                UNION 
                                SELECT 'A-' as [bloodtype], 'A-' as [BValue]
                                UNION
                                SELECT 'B+' as [bloodtype], 'B+' as [BValue]
                                UNION
                                SELECT 'B-' as [bloodtype], 'B-' as [BValue]
                                UNION
                                SELECT 'AB+' as [bloodtype], 'AB+' as [BValue]
                                UNION
                                SELECT 'AB-' as [bloodtype], 'AB-' as [BValue]
                                UNION
                                SELECT 'O+' as [bloodtype], 'O+' as [BValue]
                                UNION
                                SELECT 'O-' as [bloodtype], 'O-' as [BValue]";
                        #endregion
                        return generateComboData("bloodtype", "BValue", ref BLOODTYPE, sql);
                    }
                case CommonEnum.GenericCBType.GENDER:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'MALE' as [gender], 'M' as [char]
                                UNION
                                SELECT 'FEMALE' as [gender], 'F' as [char]";
                        #endregion
                        return generateComboData("gender", "char", ref GENDER, sql);
                    }
                case CommonEnum.GenericCBType.GENDERWBLNK:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT '' as [gender], 'BLNK' as [char]
                                UNION
                                SELECT 'MALE' as [gender], 'M' as [char]
                                UNION
                                SELECT 'FEMALE' as [gender], 'F' as [char]";
                        #endregion
                        return generateComboData("gender", "char", ref GENDERWDBLNK, sql);
                    }
                case CommonEnum.GenericCBType.SearchGender:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'ALL' as [gender], 'ALL' as [char]
                                UNION
                                SELECT 'MALE' as [gender], 'M' as [char]
                                UNION
                                SELECT 'FEMALE' as [gender], 'F' as [char]";
                        #endregion
                        return generateComboData("gender", "char", ref SearchGender, sql);
                    }
                case CommonEnum.GenericCBType.TaxSchedule:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'DAILY' as [TaxSchedule], 'D' as [char]
                                UNION
                                SELECT 'WEEKLY' as [TaxSchedule], 'W' as [char]
                                UNION
                                SELECT 'SEMI-MONTHLY' as [TaxSchedule], 'S' as [char]
                                UNION
                                SELECT 'MONTHLY' as [TaxSchedule], 'M' as [char]";
                        #endregion
                        return generateComboData("TaxSchedule", "char", ref TAXSCHED, sql);
                    }
                case CommonEnum.GenericCBType.CIVILSTATUS:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'SINGLE' as [cstatus], 'S' as [cstatusvalue]
                                UNION
                                SELECT 'MARRIED' as [cstatus], 'M' as [cstatusvalue]
                                UNION
                                SELECT 'SEPARATED' as [cstatus], 'E' as [cstatusvalue]
                                UNION
                                SELECT 'WIDOWER/WIDOWED' as [cstatus], 'W' as [cstatusvalue]";
                        #endregion
                        return generateComboData("cstatus", "cstatusvalue", ref CIVILSTATUS, sql);
                    }
                case CommonEnum.GenericCBType.CIVILSTATUSWBLNK:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT '' as [cstatus], 'BLNK' as [cstatusvalue]
                                UNION
                                SELECT 'SINGLE' as [cstatus], 'S' as [cstatusvalue]
                                UNION
                                SELECT 'MARRIED' as [cstatus], 'M' as [cstatusvalue]
                                UNION
                                SELECT 'SEPARATED' as [cstatus], 'E' as [cstatusvalue]
                                UNION
                                SELECT 'WIDOWER/WIDOWED' as [cstatus], 'W' as [cstatusvalue]";
                        #endregion
                        return generateComboData("cstatus", "cstatusvalue", ref CIVILSTATUSWDBLNK, sql);
                    }
                case CommonEnum.GenericCBType.EMPLOYMENTSTATUS:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'PROBATIONARY' as [cstatus], 'PB' as [cvalue]
                                UNION
                                SELECT 'REGULAR' as [cstatus], 'RG' as [cvalue]
                                UNION
                                SELECT 'CONTRACTUAL' as [cstatus], 'CT' as [cvalue]
                                UNION
                                SELECT 'SEASONAL' as [cstatus], 'SN' as [cvalue]";
                        #endregion
                        return generateComboData("cstatus", "cvalue", ref EMPLOYMENTSTATUS, sql);
                    }
                case CommonEnum.GenericCBType.SearchEmployStat:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'ALL' as [cstatus], 'ALL' as [cvalue]
                                UNION
                                SELECT 'PROBATIONARY' as [cstatus], 'PB' as [cvalue]
                                UNION
                                SELECT 'REGULAR' as [cstatus], 'RG' as [cvalue]
                                UNION
                                SELECT 'CONTRACTUAL' as [cstatus], 'CT' as [cvalue]
                                UNION
                                SELECT 'SEASONAL' as [cstatus], 'SN' as [cvalue]";
                        #endregion
                        return generateComboData("cstatus", "cvalue", ref SearchEmployStat, sql);
                    }
                case CommonEnum.GenericCBType.PREMIUMCODES:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT '' as [premDesc], 'BLNK' as [premValue]
                                UNION
                                SELECT 'FIXED' as [premDesc], 'F' as [premValue]
                                UNION
                                SELECT 'COMPUTED REG' as [premDesc], 'R' as [premValue]
                                UNION
                                SELECT 'COMPUTED GROSS' as [premDesc], 'G' as [premValue]
                                UNION
                                SELECT 'NONE' as [premDesc], 'N' as [premValue]";
                        #endregion
                        return generateComboData("premDesc", "premValue", ref PREMIUMCODES, sql);
                    }
                case CommonEnum.GenericCBType.EDUC_LEVEL:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'ELEMENTARY' as [ELevel], 'EL' as [EValue]
                                UNION
                                SELECT 'COLLEGE LEVEL' as [ELevel], 'CL' as [EValue]
                                UNION
                                SELECT 'COLLEGE GRADUATE' as [ELevel], 'CG' as [EValue]
                                UNION
                                SELECT 'POST GRADUATE' as [ELevel], 'PG' as [EValue]
                                UNION
                                SELECT 'HIGH SCHOOL GRADUATE' as [ELevel], 'HG' as [EValue]
                                UNION
                                SELECT 'HIGH SCHOOL LEVEL' as [ELevel], 'HL' as [EValue]";
                        #endregion
                        return generateComboData("ELevel", "EValue", ref EDUC_LEVEL, sql);
                    }
                case CommonEnum.GenericCBType.EDUC_LEVELWBLNK:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT '' as [ELevel], 'BLNK' as [EValue]
                                UNION
                                SELECT 'ELEMENTARY' as [ELevel], 'EL' as [EValue]
                                UNION
                                SELECT 'COLLEGE LEVEL' as [ELevel], 'CL' as [EValue]
                                UNION
                                SELECT 'COLLEGE GRADUATE' as [ELevel], 'CG' as [EValue]
                                UNION
                                SELECT 'POST GRADUATE' as [ELevel], 'PG' as [EValue]
                                UNION
                                SELECT 'HIGH SCHOOL GRADUATE' as [ELevel], 'HG' as [EValue]
                                UNION
                                SELECT 'HIGH SCHOOL LEVEL' as [ELevel], 'HL' as [EValue]";
                        #endregion
                        return generateComboData("ELevel", "EValue", ref EDUC_LEVELWBLNK, sql);
                    }
                case CommonEnum.GenericCBType.WORKTYPE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'NORMAL SCHED' as [worktype], 'NM' as [worktypevalue]
                                UNION
                                SELECT '4-2 SCHED' as [worktype], '42' as [worktypevalue]
                                UNION
                                SELECT 'FLEX SCHED' as [worktype], 'FL' as [worktypevalue]";
                        #endregion
                        return generateComboData("worktype", "worktypevalue", ref WORKTYPE, sql);
                    }
                case CommonEnum.GenericCBType.JOBSTATUS:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'ACTIVE, CURRENT' as [jobstatus], 'AC' as [jobstatusvalue]
                                UNION
                                SELECT 'ACTIVE, ON LEAVE' as [jobstatus], 'AL' as [jobstatusvalue]
                                UNION
                                SELECT 'INACTIVE, SEPARATED' as [jobstatus], 'I' as [jobstatusvalue]";
                        #endregion
                        return generateComboData("jobstatus", "jobstatusvalue", ref JOBSTATUS, sql);
                    }
                case CommonEnum.GenericCBType.PAYMENTMODE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'CASH' as [paymentmode], 'C' as [paymentmodevalue]
                                UNION
                                SELECT 'BANK' as [paymentmode], 'B' as [paymentmodevalue]
                                UNION
                                SELECT 'CHEQUE' as [paymentmode], 'Q' as [paymentmodevalue]";
                        #endregion
                        return generateComboData("paymentmode", "paymentmodevalue", ref PAYMENTMODE, sql);
                    }
                case CommonEnum.GenericCBType.PAYROLLTYPE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'HOURLY' as [paymentmode], 'H' as [paymentmodevalue]
                                UNION
                                SELECT 'MONTHLY' as [paymentmode], 'M' as [paymentmodevalue]
                                UNION
                                SELECT 'DAILY' as [paymentmode], 'D' as [paymentmodevalue]";
                        #endregion
                        return generateComboData("paymentmode", "paymentmodevalue", ref PAYROLLTYPE, sql);
                    }
                case CommonEnum.GenericCBType.JOBSTATUSNEW:
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT 'ACTIVE (JAPANESE MANAGER)' as [paymentmode], 'AJ' as [paymentmodevalue]
                                UNION
                                SELECT 'ACTIVE (LOCAL MANAGER)' as [paymentmode], 'AM' as [paymentmodevalue]
                                UNION
                                SELECT 'ACTIVE (JAPAN TRAINEE)' as [paymentmode], 'AT' as [paymentmodevalue]
                                UNION
                                SELECT 'ACTIVE (AWOL)' as [paymentmode], 'AA' as [paymentmodevalue]
                                UNION
                                SELECT 'ACTIVE (ON-LEAVE)' as [paymentmode], 'AL' as [paymentmodevalue]
                                UNION
                                SELECT 'ACTIVE (CURRENT)' as [paymentmode], 'AC' as [paymentmodevalue]
                                UNION
                                SELECT 'INACTIVE' as [paymentmode], 'IN' as [paymentmodevalue]";
                        #endregion
                        return generateComboData("paymentmode", "paymentmodevalue", ref JOBSTATUSNEW, sql);
                    }
                case CommonEnum.GenericCBType.SearchJobStat:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'ALL' as [JobStat], 'ALL' as [JobStatValue]
                                UNION
                                SELECT 'ACTIVE (CURRENT)' as [JobStat], 'AC' as [JobStatValue]
                                UNION
                                SELECT 'ACTIVE (ON-LEAVE)' as [JobStat], 'AL' as [JobStatValue]
                                UNION
                                SELECT 'ACTIVE (MANAGER)' as [JobStat], 'AM' as [JobStatValue]
                                UNION
                                SELECT 'INACTIVE' as [JobStat], 'IN' as [JobStatValue]";
                        #endregion
                        return generateComboData("JobStat", "JobStatValue", ref SearchJobStat, sql);
                    }
                case CommonEnum.GenericCBType.WORKTYPENEW:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT '4D WORK, 2D REST' as [paymentmode], '4D' as [paymentmodevalue]
                                UNION
                                SELECT '5D WORK, 2D REST' as [paymentmode], '5D' as [paymentmodevalue]
                                UNION
                                SELECT '6D WORK, 1D REST' as [paymentmode], '6D' as [paymentmodevalue]";
                        #endregion
                        return generateComboData("paymentmode", "paymentmodevalue", ref WORKTYPENEW, sql);
                    }
                case CommonEnum.GenericCBType.MENU_TYPE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'TRANSACTION' as [mTypeVAlue], 'T' as [mTypeCode]
                                UNION
                                SELECT 'MAINTENANCE' as [mTypeVAlue], 'M' as [mTypeCode]
                                UNION
                                SELECT 'INQUIRY' as [mTypeVAlue], 'Q' as [mTypeCode]
                                UNION
                                SELECT 'REPORTS' as [mTypeVAlue], 'R' as [mTypeCode]
                                UNION
                                SELECT 'UTILITIES' as [mTypeVAlue], 'U' as [mTypeCode]";
                        #endregion
                        return generateComboData("mTypeVAlue", "mTypeCode", ref MENUTYPE, sql);
                    }
                case CommonEnum.GenericCBType.MENU_TYPEALL:
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT 'ALL' as [mTypeVAlueAll], 'A' as [mTypeCodeAll]
                                UNION
                                SELECT 'TRANSACTION' as [mTypeVAlueAll], 'T' as [mTypeCodeAll]
                                UNION
                                SELECT 'MAINTENANCE' as [mTypeVAlueAll], 'M' as [mTypeCodeAll]
                                UNION
                                SELECT 'INQUIRY' as [mTypeVAlueAll], 'Q' as [mTypeCodeAll]
                                UNION
                                SELECT 'REPORTS' as [mTypeVAlueAll], 'R' as [mTypeCodeAll]
                                UNION
                                SELECT 'UTILITIES' as [mTypeVAlueAll], 'U' as [mTypeCodeAll]";
                        #endregion
                        return generateComboData("mTypeVAlueAll", "mTypeCodeAll", ref MENUTYPEALL, sql);
                    }
                case CommonEnum.GenericCBType.SearchWrkType:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'ALL' as [worktype], 'ALL' as [worktypevalue]
                                UNION
                                SELECT '4-2 SHIFT' as [worktype], 'SPL' as [worktypevalue]
                                UNION
                                SELECT 'NORMAL SHIFT' as [worktype], 'NML' as [worktypevalue]";
                        #endregion
                        return generateComboData("worktype", "worktypevalue", ref SearchWrkTyp, sql);
                    }
                case CommonEnum.GenericCBType.PAYMENTTYPE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'MONTHLY' as [paymenttype], 'M' as [paymenttypevalue]
                                UNION
                                SELECT 'DAILY' as [paymenttype], 'D' as [paymenttypevalue]";
                        #endregion
                        return generateComboData("paymenttype", "paymenttypevalue", ref PAYMENTTYPE, sql);
                    }
                case CommonEnum.GenericCBType.TAXCODE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'SINGLE' as [taxcode], 'S' as [taxcodevalue]
                                UNION
                                SELECT 'HEAD OF THE FAMILY' as [taxcode], 'HF' as [taxcodevalue]
                                UNION
                                SELECT 'MARRIED' as [taxcode], 'ME' as [taxcodevalue]
                                UNION
                                SELECT 'EMPLOYED HUSBAND' as [taxcode], 'EH' as [taxcodevalue]
                                UNION
                                SELECT 'EMPLOYED WIFE' as [taxcode], 'EW' as [taxcodevalue]";
                        #endregion
                        return generateComboData("taxcode", "taxcodevalue", ref TAXCODE, sql);
                    }
                case CommonEnum.GenericCBType.TaxClass:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'TAX' as [taxclass], 'T' as [taxclassvalue]
                                UNION
                                SELECT 'NON-TAX' as [taxclass], 'N' as [taxclassvalue]";
                        #endregion
                        return generateComboData("taxclass", "taxclassvalue", ref TaxClass, sql);
                    }
                case CommonEnum.GenericCBType.TAXCODENEW:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'S' as [taxcode], 'S' as [taxcodevalue]
                                UNION
                                SELECT 'HF' as [taxcode], 'HF' as [taxcodevalue]
                                UNION
                                SELECT 'HF1' as [taxcode], 'HF1' as [taxcodevalue]
                                UNION
                                SELECT 'HF2' as [taxcode], 'HF2' as [taxcodevalue]
                                UNION
                                SELECT 'HF3' as [taxcode], 'HF3' as [taxcodevalue]
                                UNION
                                SELECT 'HF4' as [taxcode], 'HF4' as [taxcodevalue]
                                UNION
                                SELECT 'ME' as [taxcode], 'ME' as [taxcodevalue]
                                UNION
                                SELECT 'ME1' as [taxcode], 'ME1' as [taxcodevalue]
                                UNION
                                SELECT 'ME2' as [taxcode], 'ME2' as [taxcodevalue]
                                UNION
				                SELECT 'ME3' as [taxcode], 'ME3' as [taxcodevalue]
                                UNION
				                SELECT 'ME4' as [taxcode], 'ME4' as [taxcodevalue]
                                UNION
				                SELECT 'Z' as [taxcode], 'Z' as [taxcodevalue]
                                UNION
				                SELECT 'X' as [taxcode], 'NOT APPLICABLE' as [taxcodevalue]";
                        #endregion
                        return generateComboData("taxcode", "taxcodevalue", ref TAXCODENEW, sql);
                    }
                case CommonEnum.GenericCBType.HDMFCODE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'REGULAR' as [HDMFcode], 'R' as [HDMFcodevalue]
                                UNION
                                SELECT 'SPECIAL' as [HDMFcode], 'S' as [HDMFcodevalue]
                                UNION
                                SELECT 'NOT A MEMBER' as [HDMFcode], 'N' as [HDMFcodevalue]";
                        #endregion
                        return generateComboData("HDMFcode", "HDMFcodevalue", ref HDMFCODE, sql);
                    }
                case CommonEnum.GenericCBType.TRUEORFALSE:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'TRUE' as [trueorfalse]
                                UNION
                                SELECT 'FALSE' as [trueorfalse]";
                        #endregion
                        return generateComboData("trueorfalse", "trueorfalse", ref TRUEORFALSE, sql);
                    }
                case CommonEnum.GenericCBType.LEAVETYPE_COMBOBOX:
                    {
                        string sql;
                        #region sgl
                        sql = @"SELECT Ltm_LeaveType AS LeaveType_Val
                                                            ,Ltm_LeaveDesc  AS LeaveType_Type
                                                        FROM dbo.T_LeaveTypeMaster
                                                        WHERE Ltm_CombinedLeave = '0'   
                                                    ";
                        #endregion
                        return generateComboData("LeaveType_Type", "LeaveType_Val", ref LEAVETYPE_COMBOX, sql);

                    }
                case CommonEnum.GenericCBType.DeductionType:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'COMPANY' as [DeductionType], 'C' as [DeductionTypeValue]
                                UNION
                                SELECT 'GOVERNMENT MANDATED' as [DeductionType], 'G' as [DeductionTypeValue]
                                UNION
                                SELECT 'GOVERNMENT LOANS' as [DeductionType], 'L' as [DeductionTypeValue]
                                UNION
                                SELECT 'EXTERNAL PARTY' as [DeductionType], 'E' as [DeductionTypeValue]
                                UNION
                                SELECT 'ALL' as [DeductionType], 'A' as [DeductionTypeValue]";
                        #endregion
                        return generateComboData("DeductionType", "DeductionTypeValue", ref DEDUCTIONTYPE, sql);
                    }
                case CommonEnum.GenericCBType.DEDUCTTYPENOALL:
                    {
                        string sql;
                        #region sql
                        sql = @"
                                SELECT 'COMPANY' as [DeductionType], 'C' as [DeductionTypeValue]
                                UNION
                                SELECT 'GOVERNMENT MANDATED' as [DeductionType], 'G' as [DeductionTypeValue]
                                UNION
                                SELECT 'GOVERNMENT LOANS' as [DeductionType], 'L' as [DeductionTypeValue]
                                UNION
                                SELECT 'EXTERNAL PARTY' as [DeductionType], 'E' as [DeductionTypeValue]";
                        #endregion
                        return generateComboData("DeductionType", "DeductionTypeValue", ref DEDUCTTYPENOALL, sql);
                    }
                case CommonEnum.GenericCBType.SpecialPayPeriod:  //Jule Added 20090116
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT Ppm_PayPeriod [Period] , '[' + CONVERT(CHAR(12),Ppm_StartCycle,107) + '] - [' + CONVERT(CHAR(12),Ppm_EndCycle,107) + ']' [Range]
                                FROM T_PayPeriodMaster
                                Left Join
                                (SELECT distinct Elr_CurrentPayPeriod AS PayPeriod
                                 FROM T_EmployeeLeaveRefundHist) xx ON xx.PayPeriod = Ppm_PayPeriod
                                WHERE xx.PayPeriod  is null
                                and Ppm_CycleIndicator ='S'
                                and PPm_Status = 'A'";
                        #endregion
                        return generateComboData("Period", "Range", ref SpecialPayPeriod, sql);
                    }
                case CommonEnum.GenericCBType.ForBonusComputationPayPeriod:
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT Ppm_PayPeriod [Period] , '[' + CONVERT(CHAR(12),Ppm_StartCycle,107) + '] - [' + CONVERT(CHAR(12),Ppm_EndCycle,107) + ']' [Range]
                                FROM T_PayPeriodMaster
                                Left Join
                                (SELECT distinct Elr_CurrentPayPeriod AS PayPeriod
                                 FROM T_EmployeeLeaveRefundHist) xx ON xx.PayPeriod = Ppm_PayPeriod
                                WHERE xx.PayPeriod  is null
                                and Ppm_CycleIndicator in ('C', 'F')
                                and PPm_Status = 'A'";
                        #endregion
                        return generateComboData("Period", "Range", ref ForBonusComputationPayPeriod, sql);
                    }
                case CommonEnum.GenericCBType.YearList:
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT distinct(left(Ppm_PayPeriod, 4)) [Year], left(Ppm_PayPeriod, 4) [YearValue]
                                FROM T_PayPeriodMaster
                                Left Join
                                (SELECT distinct Elr_CurrentPayPeriod AS PayPeriod
                                 FROM T_EmployeeLeaveRefundHist) xx ON xx.PayPeriod = Ppm_PayPeriod
                                WHERE xx.PayPeriod  is null
                                and Ppm_CycleIndicator in ('C', 'F')
                                and PPm_Status = 'A'
                                order by 1 desc";
                        #endregion
                        return generateComboData("Year", "YearValue", ref YearList, sql);
                    }
                case CommonEnum.GenericCBType.PastAndCurrentPayPeriod:  
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT Ppm_PayPeriod [Period] , '[' + CONVERT(CHAR(12),Ppm_StartCycle,107) + '] - [' + CONVERT(CHAR(12),Ppm_EndCycle,107) + ']' [Range]
                                FROM T_PayPeriodMaster
                                Left Join
                                (SELECT distinct Elr_CurrentPayPeriod AS PayPeriod
                                 FROM T_EmployeeLeaveRefundHist) xx ON xx.PayPeriod = Ppm_PayPeriod
                                WHERE xx.PayPeriod  is null
                                and Ppm_CycleIndicator in ('P', 'C')
                                and PPm_Status = 'A'
								ORDER BY Ppm_PayPeriod DESC";
                        #endregion
                        return generateComboData("Period", "Range", ref PastAndCurrentPayPeriod, sql);
                    }
                case CommonEnum.GenericCBType.BILLABLE:  
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT 'BILLABLE' as [cstatus], 'B' as [cvalue]
                                UNION
                                SELECT 'NON-BILLABLE' as [cstatus], 'N' as [cvalue]";
                        #endregion
                        return generateComboData("cstatus", "cvalue", ref BILLABLE, sql);
                    }
                case CommonEnum.GenericCBType.CCTRTYPE:
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT 'Cost Center Code' as [cost], '00' as [cvalue]
                                UNION
                                SELECT 'Division Code' as [cost], '01' as [cvalue]
                                UNION
                                SELECT 'Department Code' as [cost], '02' as [cvalue]
                                UNION
                                SELECT 'Section Code' as [cost], '03' as [cvalue]
                                UNION
                                SELECT 'Subsection Code' as [cost], '04' as [cvalue]
                                UNION
                                SELECT 'Process Code' as [cost], '05' as [cvalue]";
                        #endregion
                        return generateComboData("cost", "cvalue", ref CCTRTYPE, sql);
                    }
                case CommonEnum.GenericCBType.relational:
                    {
                        string sql;
                        #region sql
                        sql = @"SELECT '>' as [rel], '>' as [rvalue]
                                UNION
                                SELECT '<' as [rel], '<' as [rvalue]
                                UNION
                                SELECT '=' as [rel], '=' as [rvalue]
                                UNION
                                SELECT '>=' as [rel], '>=' as [rvalue]
                                UNION
                                SELECT '<=' as [rel], '<=' as [rvalue]";
                        #endregion
                        return generateComboData("rel", "rvalue", ref relational, sql);
                    }
                case CommonEnum.GenericCBType.PAYROLLTYPE1://ADDED BY JHAEL
                    {
                        return generateComboData(new Enum[] { CommonEnum.PayrollType1.MONTLY, CommonEnum.PayrollType1.DAILY, CommonEnum.PayrollType1.HOURLY, CommonEnum.PayrollType1.ALL });
                    }
            }

            return null;
        }

        private DataSet generateComboData(string displayMember, string valueMember, ref DataSet ds, BLogic.BaseBL bLogic)
        {
            this.DisplayMember = displayMember;
            this.ValueMember = valueMember;
            if (ds != null) return ds.Copy();

            ds = bLogic.FetchAll();
            return ds;

        }

        //Seldon Added 04/13/2007
        private DataSet generateComboData(string displayMember, string valueMember, ref DataSet ds, string queryString)
        {
            this.DisplayMember = displayMember;
            this.ValueMember = valueMember;
            if (ds != null) return ds.Copy();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(queryString, CommandType.Text);

                dal.CloseDB();
            }

            return ds;

        }


        private DataSet generateComboData(Enum[] e)
        {
            DataTable dt = createGenericDataTable();

            foreach (Enum enm in e)
            {
                dt.Rows.Add(addGenericComboBoxRow(dt, enm));
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        private DataTable createGenericDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DisplayMember");
            dt.Columns.Add("ValueMember");
            return dt;
        }

        // Date: 08/01/2006 8:00
        // Author: Dax
        // BEGIN
        private DataTable createGenericDataTableNumeric()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DisplayMember", typeof(System.Int16));
            dt.Columns.Add("ValueMember", typeof(System.Int16));
            return dt;
        }
        // END

        private DataRow addGenericComboBoxRow(DataTable dt, Enum e)
        {
            DataRow row = dt.NewRow();
            row["DisplayMember"] = StringEnum.GetEnumDisplay(e);
            row["ValueMember"] = StringEnum.GetStringValue(e);
            return row;
        }

        private string GetFormName(System.Windows.Forms.Control ctrl)
        {
            if ((ctrl as System.Windows.Forms.Form) == null)
            {
                return GetFormName(ctrl.Parent);
            }
            else
            {
                return ctrl.Name;
            }
        }

        //arthur added start
        public void BindCBOData()
        {
            DataSet ds = GetComboBoxDataSet();
            DataView dtv = ds.Tables[0].DefaultView;
            dtv.Sort = this.DisplayMember.ToString() + " ASC ";
            this.DataSource = dtv;
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        }

        //end
    }
}