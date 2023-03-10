using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Payroll.DAL;
using Payroll.BLogic;
using CommonLibrary;

namespace Payroll.CFC
{
    public partial class DateTimeTextBox : System.Windows.Forms.TextBox
    {
        private string displayFormat;

        public DateTime DateValue
        {
            get
            {
                DateTime retVal;
                try
                {
                    retVal = Convert.ToDateTime(this.Text);
                }
                catch (Exception)
                {
                    retVal = Convert.ToDateTime(GetServerDate());
                }
                return retVal;
            }
            set
            {
                this.Text = value.ToString(DateFormat);
            }
        }

        public DateTimeTextBox()
        {
            InitializeComponent();
        }

        private void ValidateDate(object sender, EventArgs e)
        {
            string[] divdate;
            string strDate = "", strTime = "";
            string[] splitargs = new string[1];
            TextBox txtSender = (sender as TextBox);
            splitargs[0] = " ";
            try
            {
                splitargs[0] = " ";
                divdate = txtSender.Text.Split(splitargs, StringSplitOptions.RemoveEmptyEntries);
                if (divdate.Length >= 2)
                {
                    strDate = divdate[0];
                    strTime = divdate[1];
                }
                else if (divdate.Length == 1)
                {
                    if (divdate[0].Length > 5)
                        strDate = divdate[0];
                    else
                        strTime = divdate[0];
                }
                prepareTime(ref strTime);
                prepareDate(ref strDate);
                txtSender.Text = Convert.ToDateTime(strDate + " " + strTime).ToString(DateFormat);
            }
            catch (FormatException)
            {
                txtSender.Text = Convert.ToDateTime(GetServerDate()).ToString(DateFormat);
            }
        }

        private void prepareTime(ref string strTime)
        {
            if (!strTime.Trim().Equals(""))
            {
                int hh = 0, mm = 0;
                string[] hhmm = strTime.Split(':');
                if (hhmm.Length >= 2)
                {
                    hh = getIntValue(hhmm[0]);
                    mm = getIntValue(hhmm[1]);
                }
                else if (hhmm.Length == 1)
                {
                    hh = getIntValue(hhmm[0]);
                    mm = hh % 100;
                    hh /= 100;
                }
                hh = (hh > 23) ? 23 : hh;
                mm = (mm > 59) ? 59 : mm;
                strTime = hh.ToString().PadLeft(2, '0') + ":" + mm.ToString().PadLeft(2, '0');
            }
            else
            {
                strTime = DefaultTime;
            }
        }

        private void prepareDate(ref string strDate)
        {
            //if (!strDate.Trim().Equals(""))
            //{
            //    int MM = 0, dd = 0, yyyy = 0;
            //    string[] splitargs = new string[3];
            //    splitargs[0] = "/";
            //    splitargs[1] = ".";
            //    splitargs[2] = "-";
            //    string[] MMddyyyy = strDate.Split(splitargs, StringSplitOptions.RemoveEmptyEntries);
            //    if (MMddyyyy.Length >= 3)
            //    {
            //        MM = getIntValue(MMddyyyy[0]);
            //        dd = getIntValue(MMddyyyy[1]);
            //        yyyy = getIntValue(MMddyyyy[2]);
            //    }
            //    else if (MMddyyyy.Length == 2)
            //    {
            //        MM = getIntValue(MMddyyyy[0]);
            //        dd = getIntValue(MMddyyyy[1]);
            //        yyyy = Convert.ToDateTime(GetServerDate()).Year;
            //    }
            //    else if (MMddyyyy.Length == 1 && getIntValue(MMddyyyy[0]) < 100)
            //    {
            //        MM = Convert.ToDateTime(GetServerDate()).Month;
            //        dd = getIntValue(MMddyyyy[0]);
            //        yyyy = Convert.ToDateTime(GetServerDate()).Year;
            //    }
            //    else if (getIntValue(MMddyyyy[0]) > 0 &&
            //            getIntValue(MMddyyyy[0].Substring(0, 1)) <= 1)
            //    {
            //        processMMddyyyy(MMddyyyy, ref MM, ref dd, ref yyyy);
            //    }
            //    else
            //    {
            //        processyyyyMMdd(MMddyyyy, ref MM, ref dd, ref yyyy);
            //    }
            //    MM = (MM > 12) ? 12 : MM;
            //    dd = (dd > DateTime.DaysInMonth(yyyy, MM)) ? DateTime.DaysInMonth(yyyy, MM) : dd;
            //    strDate = MM.ToString().PadLeft(2, '0') + "/" + dd.ToString().PadLeft(2, '0') + "/" + yyyy.ToString().PadLeft(4, '0');
            //}
            if (!strDate.Trim().Equals(""))
            {
                try
                {
                    strDate = Convert.ToDateTime(strDate).ToString(DateOnlyFormat);
                }
                catch (Exception)
                {
                    strDate = Convert.ToDateTime(GetServerDate()).ToString(DateOnlyFormat);
                }
            }
            else
            {
                strDate = Convert.ToDateTime(GetServerDate()).ToString(DateOnlyFormat);
            }
        }

        private void processMMddyyyy(string[] MMddyyyy, ref int MM, ref int dd, ref int yyyy)
        {
            int x = getIntValue(MMddyyyy[0]);
            if (x > 0)
            {
                if (x < 10000)
                {
                    MM = x / 100;
                    dd = x % 100;
                }
                else if (x < 1000000)
                {
                    yyyy = 2000 + x % 100;
                    MM = (x / 100) / 100;
                    dd = (x / 100) % 100;
                }
                else
                {
                    yyyy = x % 10000;
                    MM = (x / 10000) / 100;
                    dd = (x / 10000) % 100;
                }
            }
        }

        private void processyyyyMMdd(string[] MMddyyyy, ref int MM, ref int dd, ref int yyyy)
        {
            int x = getIntValue(MMddyyyy[0]);
            if (x > 0)
            {
                yyyy = x / 10000;
                MM = (x % 10000) / 100;
                dd = (x % 10000) % 100;
            }
        }

        protected Int32 getIntValue(object obj)
        {
            Int32 retval = 0;
            try
            {
                retval = Convert.ToInt32(obj);
            }
            catch (Exception)
            {
                retval = 0;
            }
            return retval;
        }

        public string DateFormat
        {
            set
            {
                displayFormat = value;
            }
            get
            {
                string trial, retVal = string.Empty;
                try
                {
                    trial = Convert.ToDateTime(GetServerDate()).ToString(displayFormat);
                    retVal = displayFormat;
                }
                catch (Exception)
                {
                    retVal = "MM/dd/yyyy HH:mm";
                }
                return retVal;
            }
        }

        public string DateOnlyFormat
        {
            get
            {
                return "MM/dd/yyyy";
            }
        }

        protected string GetServerDate()
        {
            string date;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                date = dal.ExecuteScalar("SelectDateToday", CommandType.StoredProcedure).ToString();
                dal.CloseDB();
            }

            return date;
        }

        public string DefaultTime
        {
            get
            {
                return "06:00";
            }
        }
    }
}
