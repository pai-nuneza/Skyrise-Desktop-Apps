using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class GovernmentRemittancesBL
    {
        public string Filename = string.Empty;
        public string CompanyCode = string.Empty;
        public string CompanyName = string.Empty;
        public string Address = string.Empty;
        public string YearMonth = string.Empty;
        public string RemittanceNumber = string.Empty;
        public string EmployerLocatorCode = string.Empty;
        public string RemittanceBranchCode = string.Empty;
        public DateTime TransactDate;

        public GovernmentRemittancesBL(string companycode
                , string companyname
                , string filename
                , string remittancenumber
                , string yearmonth
                , string employerlocatorcode
                , string remittancebranchcode
                , DateTime dtTransact)
        {
            this.CompanyCode = companycode;
            this.CompanyName = companyname;
            this.Filename = filename;
            this.RemittanceNumber = remittancenumber;
            this.TransactDate = dtTransact;
            this.YearMonth = yearmonth;
            this.EmployerLocatorCode = employerlocatorcode;
            this.RemittanceBranchCode = remittancebranchcode;
        }

        public string GenerateTextfileSSSPremGovernment(DataTable dt, string SBRReceiptNo, string SBRDate, string SBRAmount)
        {
            string strMsg = string.Empty;
            StringBuilder text = new StringBuilder();
            Int32 detailCounter = 0;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPH = 0;

            #region Header
                try
                {
                    text.Append("00");
                    //----------------------------------------------------------------------------
                    if (this.CompanyName.Trim().Length < 30)
                    {
                        text.Append(this.CompanyName.Trim().PadRight(30));
                    }
                    else
                    {
                        text.Append(this.CompanyName.Trim().Substring(0, 30));
                    }
                    //----------------------------------------------------------------------------
                    text.Append(YearMonth.Substring(4, 2) + YearMonth.Substring(0, 4));
                    //----------------------------------------------------------------------------
                    text.Append(this.RemittanceNumber.Trim());
                    //----------------------------------------------------------------------------
                    if (SBRReceiptNo.Trim() != string.Empty)
                    {
                        //----------------------------------------------------------------------------
                        if (SBRReceiptNo.Trim().Length < 10)
                        {
                            text.Append(SBRReceiptNo.Trim().PadLeft(10));
                        }
                        else
                        {
                            text.Append(SBRReceiptNo.Trim().Substring(0, 10));
                        }
                        //----------------------------------------------------------------------------
                        DateTime dtSBRDate = Convert.ToDateTime(SBRDate.Trim());
                        text.Append(dtSBRDate.ToString("MMddyyyy"));
                        //----------------------------------------------------------------------------
                        if (SBRAmount.Trim().Length < 12)
                        {
                            text.Append(SBRAmount.Trim().PadLeft(12));
                        }
                        else
                        {
                            text.Append(SBRAmount.Trim().Substring(0, 12));
                        }
                        //----------------------------------------------------------------------------
                    }
                    else
                    {
                        /////////         1         2         3
                        /////////1234567890123456789012345678901234567890
                        text.Append("                              ");
                    }

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                if (strMsg == string.Empty)
                {
                    try
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (text.Length > 0)
                            {
                                text.Append("\r\n");
                            }
                            //----------------------------------------------------------------------------
                            text.Append("20");
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Last Name"].ToString().Trim().Length < 15)
                            {
                                text.Append(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").PadRight(15));
                            }
                            else
                            {
                                string str = dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N");
                                text.Append(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 15));
                            }
                            //----------------------------------------------------------------------------
                            if (string.Format(" {0}", dt.Rows[i]["First Name"].ToString().Trim()).Length < 15)
                            {
                                text.Append(string.Format(" {0}", dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N")).PadRight(15));
                            }
                            else
                            {
                                text.Append(string.Format(" {0}", dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N")).Substring(0, 15));
                            }
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                text.Append(dt.Rows[i]["Middle Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 1));
                            }
                            else
                            {
                                text.Append(" ");
                            }
                            //----------------------------------------------------------------------------
                            text.Append(dt.Rows[i]["SSS Number"].ToString().Trim());
                            //----------------------------------------------------------------------------
                            int monthIndicator = Convert.ToInt32(YearMonth.Substring(4, 2));
                            string temp2 = string.Empty;
                            string temp3 = string.Empty;
                            string temp4 = string.Empty;
                            temp2 = string.Format("{0:0.00}", Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim())
                                                            + Convert.ToDouble(dt.Rows[i]["ER"].ToString().Trim()));
                            temp3 = "0.00";
                            temp4 = dt.Rows[i]["EC Fund"].ToString().Trim(); // Employer Share

                            TotalSSS += Convert.ToDouble(temp2);
                            TotalPH += Convert.ToDouble(temp3);
                            TotalEC += Convert.ToDouble(temp4);

                            if (monthIndicator == 1 || monthIndicator == 4 || monthIndicator == 7 || monthIndicator == 10)
                            {
                                #region 1st month
                                //----------------------------------------------------------------------------
                                //////////////////////////////////_1234567_1234567
                                text.Append(" " + temp2.PadLeft(7) + "    0.00    0.00");
                                //----------------------------------------------------------------------------
                                //////////////////////////////////_12345_12345
                                text.Append(" " + temp3.PadLeft(5) + "  0.00  0.00");
                                //----------------------------------------------------------------------------
                                //////////////////////////////////_12345_12345
                                text.Append(" " + temp4.PadLeft(5) + "  0.00  0.00");
                                //----------------------------------------------------------------------------
                                #endregion
                            }
                            else if (monthIndicator == 2 || monthIndicator == 5 || monthIndicator == 8 || monthIndicator == 11)
                            {
                                #region 2nd month
                                //----------------------------------------------------------------------------
                                /////////_1234567_////////////////////////_1234567
                                text.Append("    0.00 " + temp2.PadLeft(7) + "    0.00");
                                //----------------------------------------------------------------------------
                                /////////_12345_////////////////////////_12345
                                text.Append("  0.00 " + temp3.PadLeft(5) + "  0.00");
                                //----------------------------------------------------------------------------
                                /////////_12345_////////////////////////_12345
                                text.Append("  0.00 " + temp4.PadLeft(5) + "  0.00");
                                //----------------------------------------------------------------------------
                                #endregion
                            }
                            else if (monthIndicator == 3 || monthIndicator == 6 || monthIndicator == 9 || monthIndicator == 12)
                            {
                                #region 3rd month
                                //----------------------------------------------------------------------------
                                /////////_1234567_1234567_////////////////////////
                                text.Append("    0.00    0.00 " + temp2.PadLeft(7));
                                //----------------------------------------------------------------------------
                                /////////_12345_12345_////////////////////////
                                text.Append("  0.00  0.00 " + temp3.PadLeft(5));
                                //----------------------------------------------------------------------------
                                /////////_12345_12345_////////////////////////
                                text.Append("  0.00  0.00 " + temp4.PadLeft(5));
                                //----------------------------------------------------------------------------
                                #endregion
                            }
                            //----------------------------------------------------------------------------
                            /////////         1         2         3
                            /////////1234567890123456789012345678901234567890
                            text.Append("      ");
                            //----------------------------------------------------------------------------
                            string code = string.Empty;
                            if (dt.Rows[i]["Rem Code"].ToString().Trim() == string.Empty)
                            {
                                if (dt.Rows[i]["Separation Date"].ToString().Trim() != string.Empty)
                                {
                                    text.Append("2");
                                    code = "2";
                                }
                                else
                                {
                                    DateTime dtHire = Convert.ToDateTime(dt.Rows[i]["Hired Date"].ToString().Trim());
                                    if (dtHire.ToString("MM") == YearMonth.Substring(4, 2)
                                        && dtHire.ToString("yyyy") == YearMonth.Substring(0, 4))
                                    {
                                        text.Append("1");
                                        code = "1";
                                    }
                                    else
                                    {
                                        if ((Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim())
                                                            + Convert.ToDouble(dt.Rows[i]["ER"].ToString().Trim())) == 0)
                                        {
                                            text.Append("3");
                                            code = "3";
                                        }
                                        else
                                        {
                                            text.Append("N");
                                            code = "N";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                text.Append(dt.Rows[i]["Rem Code"].ToString().Trim().Substring(0, 1));
                                code = dt.Rows[i]["Rem Code"].ToString().Trim().Substring(0, 1);
                            }
                            //----------------------------------------------------------------------------
                            DateTime dte;
                            if (code != "N")
                            {
                                if (dt.Rows[i]["Separation Date"].ToString().Trim() != string.Empty)
                                {
                                    dte = Convert.ToDateTime(dt.Rows[i]["Separation Date"].ToString().Trim());
                                }
                                else
                                {
                                    dte = Convert.ToDateTime(dt.Rows[i]["Hired Date"].ToString().Trim());
                                }
                                text.Append(dte.ToString("MMddyyyy"));
                            }
                            else
                            {
                                text.Append("        ");
                            }
                            //----------------------------------------------------------------------------
                            detailCounter++;
                        }
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Trail
                if (strMsg == string.Empty)
                {
                    try
                    {
                        if (text.Length > 0)
                        {
                            text.Append("\r\n");
                        }
                        //----------------------------------------------------------------------------
                        text.Append("99");
                        //----------------------------------------------------------------------------
                        int monthIndicator = Convert.ToInt32(YearMonth.Substring(4, 2));
                        string temp2 = string.Empty;
                        string temp3 = string.Empty;
                        string temp4 = string.Empty;
                        temp2 = string.Format("{0:0.00}", TotalSSS);
                        temp3 = "0.00";
                        temp4 = string.Format("{0:0.00}", TotalEC);
                        if (monthIndicator == 1 || monthIndicator == 4 || monthIndicator == 7 || monthIndicator == 10)
                        {
                            #region 1st month
                            //----------------------------------------------------------------------------
                            ///////////////////////////////////_12345678901_12345678901
                            text.Append(" " + temp2.PadLeft(11) + "        0.00        0.00");
                            //----------------------------------------------------------------------------
                            //////////////////////////////////_123456789_123456789
                            text.Append(" " + temp3.PadLeft(9) + "      0.00      0.00");
                            //----------------------------------------------------------------------------
                            //////////////////////////////////_123456789_123456789
                            text.Append(" " + temp4.PadLeft(9) + "      0.00      0.00");
                            //----------------------------------------------------------------------------
                            #endregion
                        }
                        else if (monthIndicator == 2 || monthIndicator == 5 || monthIndicator == 8 || monthIndicator == 11)
                        {
                            #region 2nd month
                            //----------------------------------------------------------------------------
                            /////////_12345678901_/////////////////////////_12345678901
                            text.Append("        0.00 " + temp2.PadLeft(11) + "        0.00");
                            //----------------------------------------------------------------------------
                            /////////_123456789_////////////////////////_123456789
                            text.Append("      0.00 " + temp3.PadLeft(9) + "      0.00");
                            //----------------------------------------------------------------------------
                            /////////_123456789_////////////////////////_123456789
                            text.Append("      0.00 " + temp4.PadLeft(9) + "      0.00");
                            //----------------------------------------------------------------------------
                            #endregion
                        }
                        else if (monthIndicator == 3 || monthIndicator == 6 || monthIndicator == 9 || monthIndicator == 12)
                        {
                            #region 3rd month
                            //----------------------------------------------------------------------------
                            /////////_12345678901_12345678901_////////////////////////
                            text.Append("        0.00        0.00 " + temp2.PadLeft(11));
                            //----------------------------------------------------------------------------
                            /////////_123456789_123456789_////////////////////////
                            text.Append("      0.00      0.00 " + temp3.PadLeft(9));
                            //----------------------------------------------------------------------------
                            /////////_123456789_123456789_////////////////////////
                            text.Append("      0.00      0.00 " + temp4.PadLeft(9));
                            //----------------------------------------------------------------------------
                            #endregion
                        }
                        //----------------------------------------------------------------------------
                        /////////         1         2         3
                        /////////123456789012345678901234567890
                        text.Append("                    ");
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Length == 0)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text.ToString());
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSPremMetrobankOnline(DataTable dt)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            string temp2 = string.Empty;

           #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {           //123456789012345678901234567890
                    header.Append(string.Format("00   1{0}         " + this.TransactDate.Year.ToString()
                                    + this.TransactDate.ToString("MMddyyyy").Substring(0, 2)
                                    + this.TransactDate.ToString("MMddyyyy").Substring(2, 2)
                                    + this.YearMonth.ToString(), this.TransactDate.Year.ToString() + this.TransactDate.Month.ToString().PadLeft(2, '0')));

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim().Replace(".", "").Replace(",", "").Replace("Ñ", "N").Replace("-", "");
                    if (temp2.Length < 40)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (40 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 40));
                    }

                    errtemp1 = "Error in Remittance Number";
                    temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception(errtemp1);
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    errtemp1 = " Error in Employer Branch Code";
                    header.Append("   ");

                    errtemp1 = " Error in Employer Locator Code";
                    if (this.EmployerLocatorCode.Trim() != string.Empty)
                        temp2 = this.EmployerLocatorCode;
                    else
                        throw new Exception(errtemp1);

                    header.Append(temp2);

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }
                #endregion

           #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dt.Rows[i]["Total"]) > 0)
                    {
                        StringBuilder temp = new StringBuilder();
                        string errTemp = string.Empty;
                        try
                        {
                            temp.Append("20   ");

                            temp2 = string.Empty;
                            errTemp = "Error in Last Name";
                            temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 20)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (20 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 20));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in First Name";
                            temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 20)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (20 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 20));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in Middle Name";
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                temp.Append(dt.Rows[i]["Middle Name"].ToString().Replace("Ñ", "N").Replace("-", "").Replace("*", " ").Trim().ToUpper().Substring(0, 1));
                            }
                            else
                            {
                                temp.Append(" ");
                            }

                            errTemp = "Error in SSS Number";
                            if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                            {
                                throw new Exception();
                            }
                            else
                            {
                                temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                            }

                            errTemp = "Error in SSS PREMIUM & ECFUND Values";
                            int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                            string temp3 = string.Empty;
                            string temp4 = string.Empty;

                            temp2 = dt.Rows[i]["Total"].ToString().Trim();
                            temp3 = dt.Rows[i]["EC Fund"].ToString().Trim();
                            temp4 = dt.Rows[i]["EC Fund"].ToString().Trim();

                            TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                            TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());
                            TotalPhil += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());

                            int len2 = temp2.Length;
                            for (int idx = 0; idx < (8 - len2); idx++)
                                temp.Append(" ");
                            temp.Append(temp2);

                            len2 = temp3.Length;
                            for (int idx = 0; idx < (8 - len2); idx++)
                                temp.Append(" ");

                            temp.Append(temp3);

                            errTemp = "Error in Hire/Sep Date";
                            if (dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "") != string.Empty)
                            {
                                temp2 = dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "");
                                temp.Append(temp2);
                            }
                            else
                            {
                                temp.Append("        ");
                            }
                            errTemp = "Error in Rem Code";
                            temp2 = dt.Rows[i]["Rem Code"].ToString().Trim();
                            if (temp2.Trim() != string.Empty)
                            {
                                 temp.Append(temp2.Trim().Substring(0, 1));
                            }
                            else
                            {
                                temp.Append("3");
                            }

                            text += temp + "\r\n";
                            ++DetailCounter;
                        }
                        catch (Exception er)
                        {
                            strMsg += er.Message;
                        }
                    }
                }
                #endregion

           #region Trail

                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99   ");

                    errTrail = "Error in Totals";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    temp2 = string.Format("{0:0.00}", TotalSSS);
                    temp3 = string.Format("{0:0.00}", TotalPhil);
                    temp4 = string.Format("{0:0.00}", TotalEC);

                    int len = temp2.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp2);

                    len = temp4.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp4);

                    Trail.Append(this.TransactDate.Year.ToString() + this.TransactDate.Month.ToString().PadLeft(2, '0') + this.TransactDate.Day.ToString().PadLeft(2, '0'));

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }


                #endregion

           #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSPremMetrobank(DataTable dt, string RemittanceBranchCode)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("00   1000001         ");

                    errtemp1 = "Error in Date";
                    header.Append(this.TransactDate.ToString("MMddyyyy").Substring(4, 4)
                        + this.TransactDate.ToString("MMddyyyy").Substring(0, 2)
                        + this.TransactDate.ToString("MMddyyyy").Substring(2, 2));

                    errtemp1 = "Error in Applicable Month";
                    header.Append(this.YearMonth.ToString());

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Replace("Ñ", "N").Trim();
                    if (temp2.Length < 40)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (40 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 40));
                    }

                    errtemp1 = "Error in Remittance Number";
                    temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception(errtemp1);
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    temp2 = RemittanceBranchCode.Trim();
                    if (temp2.Length < 3)
                    {
                        for (int idx = 0; idx < temp2.Length; idx++)
                            header.Append(" ");
                        header.Append(temp2);
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 3));
                    }

                    errtemp1 = " Error in Employer Locator Code";
                    if (this.EmployerLocatorCode.Trim() != string.Empty)
                        temp2 = this.EmployerLocatorCode;
                    else
                        throw new Exception(errtemp1);

                    if (temp2.Length < 3)
                    {
                        header.Append(temp2);
                        for (int idx = 0; idx < (3 - temp2.Length); idx++)
                        {
                            header.Append(" ");
                        }
                    }

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        temp.Append("20   ");

                        temp2 = string.Empty;
                        errTemp = "Error in Last Name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 20));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First Name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 20));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middle Name";
                        if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                        {
                            temp.Append(dt.Rows[i]["Middle Name"].ToString().Trim().Replace("Ñ", "N").ToUpper().Substring(0, 1));
                        }
                        else
                        {
                            temp.Append(" ");
                        }

                        errTemp = "Error in SSS Number";
                        if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                        }

                        errTemp = "Error in SSS PREMIUM, ECFUND, & PHILHEALTH Values";
                        int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = dt.Rows[i]["Total"].ToString().Trim();
                        temp3 = "0.00";  //0.00[PHILHEALTH]
                        temp4 = dt.Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                        TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());
                        TotalPhil += 0.00;

                        int len2 = temp2.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp.Append(" ");

                        temp.Append(temp2);

                        len2 = temp3.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp.Append(" ");

                        temp.Append(temp3);

                        len2 = temp4.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp.Append(" ");

                        temp.Append(temp4);


                        errTemp = "Error in Hire/Sep Date";
                        if (dt.Rows[i]["Birth Date]"].ToString().Trim().Replace("/", "") != string.Empty)
                        {
                            DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Birth Date]"].ToString().Trim());
                            temp2 = dtTemp.ToString("MMddyyyy");
                            temp.Append(temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2));
                        }
                        else
                        {
                            temp.Append("        ");
                        }
                        errTemp = "Error in Rem Code";
                        temp2 = dt.Rows[i]["Rem Code"].ToString().Trim();
                        if (temp2.Trim() != string.Empty)
                        {
                            temp.Append(temp2.Trim().Substring(0, 1));
                        }
                        else
                        {
                            temp.Append("3");
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += er.Message;
                    }
                }


                #endregion

            #region Trail
                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99   ");

                    errTrail = "Error in Totals";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    temp2 = string.Format("{0:0.00}", TotalSSS);
                    temp3 = string.Format("{0:0.00}", TotalPhil);
                    temp4 = string.Format("{0:0.00}", TotalEC);

                    int len = temp2.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp2);

                    len = temp3.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp3);

                    len = temp4.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp4);

                    Trail.Append("00000001");

                    //         1         2         3
                    //123456789012345678901234567890
                    Trail.Append("       ");

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }


                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSPremUnionBank(DataTable dt, string DocumentNo, string PaymentOrderNo)
        {
                string strMsg = string.Empty;
                string text = string.Empty;
                int DetailCounter = 0;
                string Error = string.Empty;
                //string batchNumber = string.Empty;
                double TotalSSS = 0;
                double TotalEC = 0;
                string temp2 = string.Empty;

                #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {           //123456789012345678901234567890

                    header.Append("00   1");
                    errtemp1 = "Error in Document Number";
                    temp2 = DocumentNo.Trim();
                    if (temp2.Length < 6)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (6 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 6));
                    }
                    header.Append("         ");
                    header.Append(this.TransactDate.ToString("yyyyMMdd"));

                    errtemp1 = "Error in Applicable Month";
                    header.Append(CommonBL.PadSubString(this.YearMonth.ToString(), 6));

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim().Replace(".", "").Replace(",", "").Replace("Ñ", "N").Replace("-", "");
                    if (temp2.Length < 40)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (40 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 40));
                    }

                    errtemp1 = "Error in Remittance Number";
                    temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception(errtemp1);
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    errtemp1 = " Error in Employer Branch Code";
                    header.Append("   ");

                    errtemp1 = " Error in Employer Locator Code";
                    if (this.EmployerLocatorCode.Trim() != string.Empty)
                        temp2 = this.EmployerLocatorCode;
                    else
                        throw new Exception(errtemp1);

                    header.Append(temp2);
                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }
                #endregion

                #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dt.Rows[i]["Total"]) > 0)
                    {
                        StringBuilder temp = new StringBuilder();
                        string errTemp = string.Empty;
                        try
                        {
                            temp.Append("20   ");

                            temp2 = string.Empty;
                            errTemp = "Error in Last Name";
                            temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 20)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (20 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 20));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in First Name";
                            temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 20)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (20 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 20));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in Middle Name";
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                temp.Append(dt.Rows[i]["Middle Name"].ToString().Replace("Ñ", "N").Replace("-", "").Replace("*", " ").Trim().ToUpper().Substring(0, 1));
                            }
                            else
                            {
                                temp.Append(" ");
                            }

                            errTemp = "Error in SSS Number";
                            if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                            {
                                throw new Exception();
                            }
                            else
                            {
                                temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                            }

                            errTemp = "Error in SSS PREMIUM & ECFUND Values";
                            int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                            string temp3 = string.Empty;
                            string temp4 = string.Empty;

                            temp2 = dt.Rows[i]["Total"].ToString();
                            temp3 = dt.Rows[i]["EC Fund"].ToString().Trim();
                            temp4 = dt.Rows[i]["EC Fund"].ToString().Trim();

                            TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                            TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());
                            //TotalPhil += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());

                            int len2 = temp2.Length;
                            for (int idx = 0; idx < (8 - len2); idx++)
                                temp.Append(" ");
                            temp.Append(temp2);

                            len2 = temp3.Length;
                            for (int idx = 0; idx < (8 - len2); idx++)
                                temp.Append(" ");

                            temp.Append(temp3);

                            errTemp = "Error in Hire/Sep Date";
                            if (dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "") != string.Empty)
                            {
                                DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Check Date"].ToString().Trim());
                                temp2 = dtTemp.ToString("yyyyMMdd").Trim();
                                temp.Append(temp2);
                            }
                            else
                            {
                                temp.Append("        ");
                            }
                            errTemp = "Error in Rem Code";
                            temp2 = dt.Rows[i]["Rem Code"].ToString();
                            if (temp2.Trim() != string.Empty)
                            {
                                if (temp2.ToUpper().Trim() == "H")
                                    temp.Append("1");
                                else if (temp2.ToUpper().Trim() == "S")
                                    temp.Append("2");
                                else if (temp2.ToUpper().Trim() == "N")
                                    temp.Append("3");
                            }
                            else
                            {
                                temp.Append("3");
                            }

                            text += temp + "\r\n";
                            ++DetailCounter;
                        }
                        catch (Exception er)
                        {
                            strMsg += er.Message;
                        }
                    }
                }
                #endregion

                #region Trail

                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99   ");

                    errTrail = "Error in Totals";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    temp2 = string.Format("{0:0.00}", TotalSSS);
                    string temp4 = string.Format("{0:0.00}", TotalEC);

                    int len = temp2.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp2);

                    len = temp4.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp4);

                    temp2 = PaymentOrderNo.Trim();
                    if (temp2.Length < 8)
                    {
                        len = temp2.Length;
                        Trail.Append(temp2);
                        for (int idx = 0; idx < (8 - len); idx++)
                            Trail.Append(" ");
                    }
                    else
                    {
                        Trail.Append(temp2.Substring(0, 8));
                    }

                    Trail.Append("       ");
                    //         1         2         3
                    //123456789012345678901234567890
                    //Trail += "       ";

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }


                #endregion

                #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
            #endregion

                return strMsg;
        }

        public string GenerateTextFileSSSPremBancNet(DataTable dt)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            string temp2 = string.Empty;

            #region Header

            StringBuilder header = new StringBuilder();
            string errtemp1 = string.Empty;
            try
            {           //123456789012345678901234567890
                header.Append(string.Format("00   1{0}         " + this.TransactDate.Year.ToString()
                                + this.TransactDate.ToString("MMddyyyy").Substring(0, 2)
                                + this.TransactDate.ToString("MMddyyyy").Substring(2, 2)
                                + this.YearMonth.ToString(), this.TransactDate.Year.ToString() + this.TransactDate.Month.ToString().PadLeft(2, '0')));

                errtemp1 = "Error in Employer Name";
                temp2 = this.CompanyName.Trim().Replace(".", "").Replace(",", "").Replace("Ñ", "N").Replace("-", "");
                if (temp2.Length < 40)
                {
                    int len = temp2.Length;
                    header.Append(temp2);
                    for (int idx = 0; idx < (40 - len); idx++)
                        header.Append(" ");
                }
                else
                {
                    header.Append(temp2.Substring(0, 40));
                }

                errtemp1 = "Error in Remittance Number";
                temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception(errtemp1);
                }
                else
                {
                    header.Append(temp2);
                }
                temp2 = string.Empty;

                errtemp1 = " Error in Employer Branch Code";
                header.Append("   ");

                errtemp1 = " Error in Employer Locator Code";
                if (this.EmployerLocatorCode.Trim() != string.Empty)
                    temp2 = this.EmployerLocatorCode;
                else
                    throw new Exception(errtemp1);

                header.Append(temp2);
                text += header;
                text += "\r\n";
            }
            catch (Exception er)
            {
                strMsg += er.Message;
            }
            #endregion

            #region Detail
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToDouble(dt.Rows[i]["Total"]) > 0)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        temp.Append("20   ");

                        temp2 = string.Empty;
                        errTemp = "Error in Last Name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 20));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First Name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 20));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middle Name";
                        if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                        {
                            temp.Append(dt.Rows[i]["Middle Name"].ToString().Trim().ToUpper().Substring(0, 1));
                        }
                        else
                        {
                            temp.Append(" ");
                        }

                        errTemp = "Error in SSS Number";
                        if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                        }

                        errTemp = "Error in SSS PREMIUM & ECFUND Values";
                        int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = dt.Rows[i]["Total"].ToString().Trim();
                        temp3 = dt.Rows[i]["EC Fund"].ToString().Trim();
                        temp4 = "0.00"; // Medicare amount
                                        //dt.Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                        TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());
                        TotalPhil += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());

                        int len2 = temp2.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp.Append(" ");
                        temp.Append(temp2);

                        len2 = temp4.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp.Append(" ");
                        temp.Append(temp4);

                        len2 = temp3.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp.Append(" ");

                        temp.Append(temp3);

                        errTemp = "Error in Hire/Sep Date";
                        if (dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "") != string.Empty)
                        {
                            temp2 = dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "");
                            temp.Append(temp2); //temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp.Append("        ");
                        }

                        errTemp = "Error in Rem Code";
                        temp2 = dt.Rows[i]["Rem Code"].ToString().Trim();
                        if (temp2.Trim() != string.Empty)
                        {
                            temp.Append(temp2.Trim().Substring(0, 1));
                        }
                        else
                        {
                            temp.Append("3");
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
            }
            #endregion

            #region Trail

            StringBuilder Trail = new StringBuilder();
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in Record ID Trail";
                Trail.Append("99   ");

                errTrail = "Error in Totals";
                int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                string temp3 = string.Empty;
                string temp4 = string.Empty;

                temp2 = string.Format("{0:0.00}", TotalSSS);
                temp3 = "0.00"; // Medicare amount to 0, since already taken from SSS
                temp4 = string.Format("{0:0.00}", TotalEC);

                int len = temp2.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail.Append(" ");

                Trail.Append(temp2);

                len = temp3.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail.Append(" ");

                Trail.Append(temp3);

                len = temp4.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail.Append(" ");

                Trail.Append(temp4);

                Trail.Append(this.TransactDate.Year.ToString() + this.TransactDate.Month.ToString().PadLeft(2, '0') + this.TransactDate.Day.ToString().PadLeft(2, '0'));

                Trail.Append("       ");

                //         1         2         3
                //123456789012345678901234567890
                //Trail += "       ";

                text += Trail;

            }
            catch (Exception er)
            {
                strMsg += er.Message;
            }


            #endregion

            #region Write To Textfile
            if (strMsg == string.Empty)
            {
                if (text.Trim() == string.Empty)
                {
                    strMsg += "\nNo government remittance generated.";
                }
                else
                {
                    TextWriter txtWriter = new StreamWriter(this.Filename);
                    txtWriter.WriteLine(text);
                    txtWriter.Close();
                }
            }
            #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSPremBDO(DataTable dt)
        {
            string strMsg = string.Empty;
            
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            string temp2 = string.Empty;

            #region Header

            StringBuilder header = new StringBuilder();
            string errtemp1 = string.Empty;
            try
            {
                header.Append("00   1");

                errtemp1 = "Error in Applicable Month";
                header.Append(this.YearMonth.Substring(0, 4) + this.YearMonth.Substring(4, 2) + "01");

                //1234567890
                header.Append("       ");

                errtemp1 = "Error in Date Transaction";
                header.Append(this.TransactDate.ToString("yyyyMMdd"));

                errtemp1 = "Error in Month Year";
                header.Append(this.YearMonth);

                errtemp1 = "Error in Employer Name";
                temp2 = this.CompanyName.Trim();
                if (temp2.Length < 40)
                {
                    int len = temp2.Length;
                    header.Append(temp2);
                    for (int idx = 0; idx < (40 - len); idx++)
                        header.Append(" ");
                }
                else
                {
                    header.Append(temp2.Substring(0, 40));
                }

                errtemp1 = "Error in Remittance Number";
                temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception(errtemp1);
                }
                else
                {
                    header.Append(temp2);
                }
                temp2 = string.Empty;

                //12345678
                header.Append("   ");

                errtemp1 = " Error in Employer Locator Code";
                if (this.EmployerLocatorCode.Trim() != string.Empty)
                    temp2 = this.EmployerLocatorCode;
                else
                    throw new Exception(errtemp1);

                text += header;
                text += "\r\n";
            }
            catch (Exception er)
            {
                strMsg += er.Message;
            }
            #endregion

            #region Detail
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StringBuilder temp = new StringBuilder();
                string errTemp = string.Empty;
                try
                {
                    temp.Append("20   ");

                    temp2 = string.Empty;
                    errTemp = "Error in Last Name";
                    temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                    if (temp2.Length < 20)
                    {
                        temp.Append(temp2);
                        int len = temp2.Length;
                        for (int idx = 0; idx < (20 - len); idx++)
                            temp.Append(" ");
                    }
                    else
                    {
                        temp.Append(temp2.Substring(0, 20));
                    }
                    temp2 = string.Empty;

                    errTemp = "Error in First Name";
                    temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                    if (temp2.Length < 20)
                    {
                        temp.Append(temp2);
                        int len = temp2.Length;
                        for (int idx = 0; idx < (20 - len); idx++)
                            temp.Append(" ");
                    }
                    else
                    {
                        temp.Append(temp2.Substring(0, 20));
                    }
                    temp2 = string.Empty;

                    errTemp = "Error in Middle Name";
                    if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                    {
                        temp.Append(dt.Rows[i]["Middle Name"].ToString().Replace("Ñ", "N").Trim().ToUpper().Substring(0, 1));
                    }
                    else
                    {
                        temp.Append(" ");
                    }

                    errTemp = "Error in SSS Number";
                    if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                    }

                    errTemp = "Error in SSS PREMIUM & ECFUND Values";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    temp2 = dt.Rows[i]["Total"].ToString().Trim();
                    temp3 = dt.Rows[i]["EC Fund"].ToString().Trim();
                    temp4 = dt.Rows[i]["EC Fund"].ToString().Trim();

                    TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                    TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());
                    TotalPhil += 0;// Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());

                    int len2 = temp2.Length;
                    for (int idx = 0; idx < (8 - len2); idx++)
                        temp.Append(" ");
                    temp.Append(temp2);

                    temp.Append("    0.00");

                    len2 = temp3.Length;
                    for (int idx = 0; idx < (8 - len2); idx++)
                        temp.Append(" ");

                    temp.Append(temp3);

                    string strRemarks = string.Empty;
                    errTemp = "Error in Rem Code";
                    temp2 = dt.Rows[i]["Rem Code"].ToString().Trim();
                    if (temp2.Trim() != string.Empty)
                    {
                        strRemarks = temp2.Trim().Substring(0, 1);
                    }
                    else
                    {
                        strRemarks = "N";
                    }

                    errTemp = "Error in Hire/Sep Date";
                    if (dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "") != string.Empty
                        && strRemarks != "N")
                    {
                        temp2 = dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "");
                        temp.Append(temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2));
                    }
                    else
                    {
                        temp.Append("        ");
                    }
                    errTemp = "Error in Rem Code";
                    if (strRemarks.Trim() != string.Empty
                        && strRemarks.Trim() != "N")
                    {
                        temp.Append(strRemarks);
                    }
                    //1234567890
                    temp.Append("        ");

                    text += temp + "\r\n";
                    ++DetailCounter;
                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }
            }
            #endregion

            #region Trail

                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99   ");

                    errTrail = "Error in Totals";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    temp2 = string.Format("{0:0.00}", TotalSSS);
                    temp3 = string.Format("{0:0.00}", TotalPhil);
                    temp4 = string.Format("{0:0.00}", TotalEC);

                    int len = temp2.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp2);

                    Trail.Append("        0.00");

                    len = temp4.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                        Trail.Append(" ");

                    Trail.Append(temp4);

                    //12345678901234567890
                    Trail.Append("               ");

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }


            #endregion

            #region Write To Textfile
            if (strMsg == string.Empty)
            {
                if (text.Trim() == string.Empty)
                {
                    strMsg += "\nNo government remittance generated.";
                }
                else
                {
                    TextWriter txtWriter = new StreamWriter(this.Filename);
                    txtWriter.WriteLine(text);
                    txtWriter.Close();
                }
            }
            #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSPremSBC(DataTable dt, string totalSSS)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("00");

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim();
                    if (temp2.Length < 30)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (30 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 30));
                    }

                    errtemp1 = "Error in Applicable Month";
                    header.Append(this.YearMonth.Substring(4, 2) + this.YearMonth.Substring(0, 4));


                    errtemp1 = "Error in Remittance Number";
                    temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception(errtemp1);
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    errtemp1 = "Error in SBR";
                    
                    temp2 = DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString() + "07";

                    header.Append(temp2);

                    temp2 = TransactDate.Month.ToString().PadLeft(2, '0') + TransactDate.Day.ToString().PadLeft(2, '0') + TransactDate.Year.ToString();

                    header.Append(temp2);

                    temp2 = totalSSS.Replace(",", "").Replace(".", "").PadLeft(12, '0');

                    header.Append(temp2);

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }

                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToDecimal(dt.Rows[i]["Total"].ToString().Trim()) != 0)
                    {
                        StringBuilder temp = new StringBuilder();
                        int j = 0;
                        string errTemp = string.Empty;
                        try
                        {
                            temp.Append("20");

                            temp2 = string.Empty;
                            errTemp = "Error in Last Name";
                            temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                            if (temp2.Length < 15)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (15 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 15));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in First Name";
                            temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                            if (temp2.Length < 15)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (15 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 15));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in Middle Name";
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                temp.Append(dt.Rows[i]["Middle Name"].ToString().Trim().ToUpper().Substring(0, 1));
                            }
                            else
                            {
                                temp.Append(" ");
                            }

                            errTemp = "Error in SSS Number";
                            if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                            {
                                throw new Exception();
                            }
                            else
                            {
                                temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                            }

                            temp.Append(" ");

                            errTemp = "Error in SSS PREMIUM, ECFUND, & PHILHEALTH Values";
                            int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                            string temp3 = string.Empty;
                            string temp4 = string.Empty;

                            temp2 = dt.Rows[i]["Total"].ToString().Trim();
                            temp3 = dt.Rows[i]["Philhealth"].ToString().Trim();
                            temp4 = dt.Rows[i]["EC Fund"].ToString().Trim();

                            TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                            TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());
                            TotalPhil += Convert.ToDouble(dt.Rows[i]["Philhealth"].ToString());

                            if (monthIndicator == 1
                               || monthIndicator == 4
                                || monthIndicator == 7
                                || monthIndicator == 10)
                            {
                                int len = temp2.Length;
                                for (int idx = 0; idx < (7 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp2);
                                temp.Append("    0.00    0.00 ");

                                len = temp3.Length;
                                for (int idx = 0; idx < (5 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp3);
                                temp.Append("  0.00  0.00 ");

                                len = temp4.Length;
                                for (int idx = 0; idx < (5 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp4);
                                temp.Append("  0.00  0.00      ");
                            }
                            else if (monthIndicator == 2
                               || monthIndicator == 5
                                || monthIndicator == 8
                                || monthIndicator == 11)
                            {
                                int len = temp2.Length;
                                temp.Append("   0.00 ");
                                for (int idx = 0; idx < (7 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp2);
                                temp.Append("    0.00 ");

                                len = temp3.Length;
                                temp.Append(" 0.00 ");
                                for (int idx = 0; idx < (5 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp3);
                                temp.Append("  0.00 ");

                                len = temp4.Length;
                                temp.Append(" 0.00 ");
                                for (int idx = 0; idx < (5 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp4);
                                temp.Append("  0.00      ");
                            }
                            else if (monthIndicator == 3
                          || monthIndicator == 6
                           || monthIndicator == 9
                           || monthIndicator == 12)
                            {
                                int len = temp2.Length;
                                temp.Append("   0.00    0.00 ");
                                for (int idx = 0; idx < (7 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp2);
                                temp.Append(" ");

                                len = temp3.Length;
                                temp.Append(" 0.00  0.00 ");
                                for (int idx = 0; idx < (5 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp3);
                                temp.Append(" ");

                                len = temp4.Length;
                                temp.Append(" 0.00  0.00 ");
                                for (int idx = 0; idx < (5 - len); idx++)
                                    temp.Append(" ");

                                temp.Append(temp4);
                                temp.Append("      ");
                            }

                            errTemp = "Error in Rem Code";

                            if (dt.Rows[i]["Rem Code"].ToString().Trim() == string.Empty)
                            {
                                temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(dt.Rows[i]["Rem Code"].ToString().Substring(0, 1));
                            }
                            errTemp = "Error in Hire Date";
                            
                            if (dt.Rows[i]["Hired Date"].ToString().Trim() != string.Empty)
                            {
                                DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Hired Date"].ToString().Trim());
                                temp2 = dtTemp.ToString("MMddyyyy").Trim();
                                temp.Append(temp2);
                                //.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                            }
                            else
                            {
                                temp.Append("        ");
                            }

                            text += temp + "\r\n";
                            ++DetailCounter;
                        }
                        catch (Exception er)
                        {
                            strMsg += er.Message;
                        }
                    }
                }

                #endregion

            #region Trail

                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99");

                    Trail.Append(" ");

                    errTrail = "Error in Totals";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    temp2 = string.Format("{0:0.00}", TotalSSS);
                    temp3 = string.Format("{0:0.00}", TotalPhil);
                    temp4 = string.Format("{0:0.00}", TotalEC);

                    if (monthIndicator == 1
                               || monthIndicator == 4
                                || monthIndicator == 7
                                || monthIndicator == 10)
                    {
                        int len = temp2.Length;
                        for (int idx = 0; idx < (11 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp2);
                        Trail.Append("        0.00        0.00 ");

                        len = temp3.Length;
                        for (int idx = 0; idx < (9 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp3);
                        Trail.Append("      0.00      0.00 ");

                        len = temp4.Length;
                        for (int idx = 0; idx < (9 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp4);
                        Trail.Append("      0.00      0.00");
                    }
                    else if (monthIndicator == 2
                               || monthIndicator == 5
                                || monthIndicator == 8
                                || monthIndicator == 11)
                    {
                        int len = temp2.Length;
                        Trail.Append("       0.00 ");
                        for (int idx = 0; idx < (11 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp2);
                        Trail.Append("        0.00 ");

                        len = temp3.Length;
                        Trail.Append("     0.00 ");
                        for (int idx = 0; idx < (9 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp3);
                        Trail.Append("      0.00 ");

                        len = temp4.Length;
                        Trail.Append("     0.00 ");
                        for (int idx = 0; idx < (9 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp4);
                        Trail.Append("      0.00");
                    }
                    else if (monthIndicator == 3
                  || monthIndicator == 6
                   || monthIndicator == 9
                   || monthIndicator == 12)
                    {
                        int len = temp2.Length;
                        Trail.Append("       0.00        0.00 ");
                        for (int idx = 0; idx < (11 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp2);

                        len = temp3.Length;
                        Trail.Append("      0.00      0.00 ");
                        for (int idx = 0; idx < (9 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp3);

                        len = temp4.Length;
                        Trail.Append("      0.00      0.00 ");
                        for (int idx = 0; idx < (9 - len); idx++)
                            Trail.Append(" ");

                        Trail.Append(temp4);
                    }
                    //         1         2         3
                    //123456789012345678901234567890
                    Trail.Append("                    ");

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }


                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSPremBPI(DataTable dt, string DocumentNo)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("00   1");

                    errtemp1 = "Error in Document Number";
                    temp2 = DocumentNo.Trim();
                    if (temp2.Length < 6)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (6 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 6));
                    }
                    header.Append("         ");
                    header.Append(this.TransactDate.ToString("yyyyMMdd"));


                    errtemp1 = "Error in Applicable Month";
                    header.Append(CommonBL.PadSubString(this.YearMonth.ToString(), 6));

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Replace("Ñ", "N").Trim();
                    header.Append(CommonBL.PadSubString(temp2, 40));

                    errtemp1 = "Error in Remittance Number";
                    temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception(errtemp1);
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    temp2 = "";
                    header.Append(CommonBL.PadSubString(temp2, 3));

                    errtemp1 = " Error in Employer Locator Code";
                    if (this.EmployerLocatorCode.Trim() != string.Empty)
                        temp2 = this.EmployerLocatorCode.Trim();
                    else
                        throw new Exception(errtemp1);

                    header.Append(CommonBL.PadSubString(temp2, 3));

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        temp.Append("20   ");

                        temp2 = string.Empty;
                        errTemp = "Error in Last Name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 20));

                        errTemp = "Error in First Name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 20));

                        errTemp = "Error in Middle Name";
                        temp2 = dt.Rows[i]["Middle Name"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 1));

                        errTemp = "Error in SSS Number";
                        if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                        }

                        errTemp = "Error in SSS PREMIUM & ECFUND Values";
                        int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = dt.Rows[i]["Total"].ToString().Trim();
                        //temp3 = dt.Rows[i]["Philhealth"].ToString().Trim();
                        temp4 = dt.Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(dt.Rows[i]["Total"].ToString());
                        //TotalPhil += Convert.ToDouble(dt.Rows[i]["Philhealth"].ToString());
                        TotalEC += Convert.ToDouble(dt.Rows[i]["EC Fund"].ToString());


                        temp.Append(CommonBL.PadSubString(temp2, 8, CommonBL.PADDIRECTION.LEFT, '0'));
                        //temp += CommonBL.PadSubString(temp3, 8, CommonBL.PADDIRECTION.RIGHT, '0');
                        temp.Append(CommonBL.PadSubString(temp4, 8, CommonBL.PADDIRECTION.LEFT, '0'));

                        errTemp = "Error in Hire/Sep Date";
                        if (dt.Rows[i]["Rem Code"].ToString().Trim().ToUpper() == "H"
                            && dt.Rows[i]["Hired Date"].ToString().Trim() != string.Empty)
                        {
                            DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Hired Date"].ToString().Trim());
                            temp.Append(dtTemp.ToString("yyyyMMdd").Trim());
                        }
                        else if (dt.Rows[i]["Rem Code"].ToString().Trim().ToUpper() == "S"
                            && dt.Rows[i]["Separation Date"].ToString().Trim() != string.Empty)
                        {
                            DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Separation Date"].ToString().Trim());
                            temp.Append(dtTemp.ToString("yyyyMMdd").Trim());
                        }
                        else
                        {
                            temp.Append(string.Empty.PadRight(8));
                        }

                        errTemp = "Error in Rem Code";
                        temp2 = dt.Rows[i]["Rem Code"].ToString().ToUpper().Trim();
                        if (temp2.Trim() != string.Empty)
                        {
                            if (temp2 == "H")
                                temp.Append("1");
                            else if (temp2 == "S")
                                temp.Append("2");
                        }
                        else
                        {
                            temp.Append("N");
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += er.Message;
                    }
                }


                #endregion

            #region Trail
                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99   ");

                    errTrail = "Error in Totals";
                    int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));

                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    temp2 = string.Format("{0:0.00}", TotalSSS);
                    //temp3 = string.Format("{0:0.00}", TotalPhil);
                    temp4 = string.Format("{0:0.00}", TotalEC);

                    Trail.Append(CommonBL.PadSubString(temp2, 12, CommonBL.PADDIRECTION.LEFT, '0'));
                    //Trail += CommonBL.PadSubString(temp3, 12, CommonBL.PADDIRECTION.LEFT, '0');
                    Trail.Append(CommonBL.PadSubString(temp4, 12, CommonBL.PADDIRECTION.LEFT, '0'));

                    Trail.Append("00000000");

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += er.Message;
                }


                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSLoanGovernment(DataTable dt, string SubmissionType, string ReceiptNo, DateTime ReceiptDate)
        {
            string strMsg = string.Empty;

            StringBuilder text = new StringBuilder();
            Int32 detailCounter = 0;
            double TotalPenalty = 0;
            double TotalAmort = 0;

            #region Header
            if (SubmissionType.ToUpper() == "B")
            {
                try
                {
                    text.Append("00");
                    //----------------------------------------------------------------------------
                    text.Append(this.RemittanceNumber.Trim());
                    //----------------------------------------------------------------------------
                    if (this.CompanyName.Trim().Length < 30)
                    {
                        text.Append(this.CompanyName.Trim().PadRight(30));
                    }
                    else
                    {
                        text.Append(this.CompanyName.Trim().Substring(0, 30));
                    }
                    //----------------------------------------------------------------------------
                    text.Append(YearMonth.Substring(2, 4));
                    //----------------------------------------------------------------------------
                    /////////         1         2         3
                    /////////1234567890123456789012345678901234567890
                    text.Append("                           ");
                    //----------------------------------------------------------------------------

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
            }
            else if (SubmissionType.ToUpper() == "O")
            {
                try
                {
                    text.Append("00");
                    //----------------------------------------------------------------------------
                    text.Append(this.RemittanceNumber.Trim());
                    //----------------------------------------------------------------------------
                    if (this.CompanyName.Trim().Length < 30)
                    {
                        text.Append(this.CompanyName.Trim().PadRight(30));
                    }
                    else
                    {
                        text.Append(this.CompanyName.Trim().Substring(0, 30));
                    }
                    //----------------------------------------------------------------------------
                    text.Append(YearMonth.Substring(2, 4));
                    text.Append(this.RemittanceBranchCode);
                    if (ReceiptNo.Trim().Length < 30)
                    {
                        text.Append(ReceiptNo.Trim().PadRight(30));
                    }
                    else
                    {
                        text.Append(ReceiptNo.Trim().Substring(0, 30));
                    }
                    text.Append(ReceiptDate.ToString("yyyyMMdd"));

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
            }
            #endregion

            #region Detail
            if (strMsg == string.Empty)
            {
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (text.Length > 0)
                        {
                            text.Append("\r\n");
                        }
                        //----------------------------------------------------------------------------
                        text.Append("10");
                        //----------------------------------------------------------------------------
                        text.Append(dt.Rows[i]["SSS Number"].ToString().Trim());
                        //----------------------------------------------------------------------------
                        if (dt.Rows[i]["Last Name"].ToString().Trim().Length < 15)
                        {
                            text.Append(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").PadRight(15));
                        }
                        else
                        {
                            string str = dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N");
                            text.Append(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 15));
                        }
                        //----------------------------------------------------------------------------
                        if (dt.Rows[i]["First Name"].ToString().Trim().Length < 15)
                        {
                            text.Append(dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N").PadRight(15));
                        }
                        else
                        {
                            text.Append(dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 15));
                        }
                        //----------------------------------------------------------------------------
                        if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                        {
                            text.Append(dt.Rows[i]["Middle Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 1) + " ");
                        }
                        else
                        {
                            text.Append("  ");
                        }
                        //----------------------------------------------------------------------------
                        text.Append(dt.Rows[i]["Loan Type"].ToString().Trim().PadLeft(1).Substring(0, 1));
                        //----------------------------------------------------------------------------
                        if (dt.Rows[i]["Loan Date"].ToString().Trim() != string.Empty)
                        {
                            DateTime dtLoanDate = Convert.ToDateTime(dt.Rows[i]["Loan Date"].ToString().Trim());
                            text.Append(dtLoanDate.ToString("yyyy").Substring(2, 2) + dtLoanDate.ToString("MMdd"));
                        }
                        else
                        {
                            /////////123456
                            text.Append("      ");
                        }
                        //----------------------------------------------------------------------------
                        string strLoanAmount = string.Format("{0:0}", Convert.ToDouble(dt.Rows[i]["Loan Amt"].ToString().Trim()));
                        text.Append(strLoanAmount.PadLeft(6, '0'));
                        //----------------------------------------------------------------------------
                        string strPenaltyAmount = dt.Rows[i]["Penalty Amt"].ToString().Trim().Replace(".", "");
                        text.Append(strPenaltyAmount.PadLeft(7, '0'));
                        TotalPenalty += Convert.ToDouble(dt.Rows[i]["Penalty Amt"].ToString().Trim());
                        //----------------------------------------------------------------------------
                        string strAmortAmount = dt.Rows[i]["Amort Paid"].ToString().Trim().Replace(".", "");
                        text.Append(strAmortAmount.PadLeft(7, '0'));
                        TotalAmort += Convert.ToDouble(dt.Rows[i]["Amort Paid"].ToString().Trim());
                        //----------------------------------------------------------------------------
                        text.Append(" ");
                        //----------------------------------------------------------------------------
                        if (dt.Rows[i]["Rem Code"].ToString().Trim() != string.Empty)
                        {
                            text.Append(dt.Rows[i]["Rem Code"].ToString().Trim().Substring(0, 1));
                        }
                        else
                        {
                            text.Append(" ");
                        }
                        //----------------------------------------------------------------------------
                        detailCounter++;
                    }
                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
            }
            #endregion

            #region Trail
            if (SubmissionType.ToUpper() == "B")
            {
                if (strMsg == string.Empty)
                {
                    try
                    {
                        if (text.Length > 0)
                        {
                            text.Append("\r\n");
                        }
                        //----------------------------------------------------------------------------
                        text.Append("99");
                        //----------------------------------------------------------------------------
                        DataView dv = new DataView(dt);
                        DataTable dtEmp = dv.ToTable(true, "ID Number");
                        string strTotalEmp = dtEmp.Rows.Count.ToString();
                        text.Append(strTotalEmp.PadLeft(4, '0'));
                        //----------------------------------------------------------------------------
                        string strTotalPenalty = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                        text.Append(strTotalPenalty.PadLeft(9, '0'));
                        //----------------------------------------------------------------------------
                        string strTotalAmort = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                        text.Append(strTotalAmort.PadLeft(9, '0'));
                        //----------------------------------------------------------------------------
                        /////////         1         2         3         4         5         6         7
                        /////////1234567890123456789012345678901234567890123456789012345678901234567890
                        //text += "                                                ";
                        //----------------------------------------------------------------------------
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
            }
            else if (SubmissionType.ToUpper() == "O")
            {
                if (strMsg == string.Empty)
                {
                    try
                    {
                        if (text.Length > 0)
                        {
                            text.Append("\r\n");
                        }
                        //----------------------------------------------------------------------------
                        text.Append("99");
                        //----------------------------------------------------------------------------
                        DataView dv = new DataView(dt);
                        DataTable dtEmp = dv.ToTable(true, "ID Number");
                        string strTotalEmp = dtEmp.Rows.Count.ToString();
                        text.Append(strTotalEmp.PadLeft(6, '0'));
                        //----------------------------------------------------------------------------
                        //string strTotalPenalty = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                        text.Append("0000000000");
                        //----------------------------------------------------------------------------
                        string strTotalAmort = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                        text.Append(strTotalAmort.PadLeft(10, '0'));
                        //----------------------------------------------------------------------------
                        /////////         1         2         3         4         5         6         7
                        /////////1234567890123456789012345678901234567890123456789012345678901234567890
                        //text += "                                                ";
                        //----------------------------------------------------------------------------
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
            }
            #endregion

            #region Write To Textfile
            if (strMsg == string.Empty)
            {
                if (text.Length == 0)
                {
                    strMsg += "\nNo government remittance generated.";
                }
                else
                {
                    TextWriter txtWriter = new StreamWriter(this.Filename);
                    txtWriter.WriteLine(text.ToString());
                    txtWriter.Close();
                }
            }
            #endregion

           return strMsg;
        }

        public string GenerateTextfileSSSLoanMetrobank(DataTable dt, string ReceiptNo, DateTime ReceiptDate)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            Int32 detailCounter = 0;
            double TotalPenalty = 0;
            double TotalAmort = 0;
            int DetailCounter = 0;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("00");

                    errtemp1 = "Error in Remittance Number";
                    string temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim();
                    if (temp2.Length < 30)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (30 - len); idx++)
                            header.Append(" ");

                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 30));
                    }

                    errtemp1 = "Error in Applicable Month";
                    header.Append(YearMonth.Substring(2, 4));

                    errtemp1 = "Error in Remittance Branch Code";
                    header.Append(this.RemittanceBranchCode.Trim().Substring(0, 2));

                    errtemp1 = "Error in SBR";
                    temp2 = ReceiptNo.Trim().Replace(" ", "").Replace("-", "").Replace(".", "");
                    if (temp2.Length < 8)
                    {
                        int len = temp2.Length;
                        for (int idx = 0; idx < (8 - len); idx++)
                            header.Append("0");
                        header.Append(temp2);
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 8));
                    }

                    temp2 = ReceiptDate.ToString().Trim();
                    header.Append(temp2.Replace("/", "").Substring(4, 4) + temp2.Replace("/", "").Substring(0, 2) + temp2.Replace("/", "").Substring(2, 2));

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        temp.Append("10");

                        errTemp = "Error in SSS Number";
                        if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                        }

                        string temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 15));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 15));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                        {
                            temp.Append(dt.Rows[i]["Middle Name"].ToString().Trim().ToUpper().Substring(0, 1) + " ");
                        }
                        else
                        {
                            temp.Append("  ");
                        }

                        errTemp = "Error in Loan Type";
                        if (dt.Rows[i]["LoanType"].ToString().Trim() != string.Empty)
                            temp.Append(dt.Rows[i]["Loan Type"].ToString().Trim());
                        else
                            temp.Append(" ");

                        errTemp = "Error in Loan Date";
                        if (dt.Rows[i]["Check Date"].ToString().Trim() != string.Empty)
                        {
                            temp2 = dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "");
                            temp.Append(temp2.Substring(6, 2) + temp2.Substring(0, 2) + temp2.Substring(2, 2));
                        }
                        else
                        {
                            temp.Append("      ");
                        }

                        errTemp = "Error in Loan Amount";
                        if (dt.Rows[i]["Loan Amt"].ToString().Trim() != string.Empty)
                        {
                            double d = Convert.ToDouble(dt.Rows[i]["Loan Amt"].ToString());
                            temp2 = string.Format("{0:0}", d);
                            if (temp2.Length < 6)
                            {
                                for (int idx = 0; idx < (6 - temp2.Length); idx++)
                                    temp.Append("0");
                                temp.Append(temp2);
                            }
                            else
                            {
                                temp.Append(temp2);
                            }
                        }
                        else
                        {
                            temp.Append(string.Empty);
                        }

                        errTemp = "Error in Penalty Amount";
                        temp2 = dt.Rows[i]["Penalty Amt"].ToString().Trim();
                        if (temp2.Replace(".", "").Length < 7)
                        {
                            for (int idx = 0; idx < (7 - temp2.Replace(".", "").Length); idx++)
                                temp.Append("0");
                            temp.Append(temp2.Replace(".", ""));
                        }
                        else
                        {
                            temp.Append(temp2.Replace(".", ""));
                        }

                        TotalPenalty += Convert.ToDouble(dt.Rows[i]["Penalty Amt"].ToString().Trim());

                        errTemp = "Error in Amort Amount";
                        temp2 = dt.Rows[i]["Amort Paid"].ToString().Trim();
                        if (temp2.Replace(".", "").Length < 7)
                        {
                            int len = temp2.Replace(".", "").Length;
                            for (int idx = 0; idx < (7 - len); idx++)
                                temp.Append("0");
                            temp.Append(temp2.Replace(".", ""));
                        }
                        else
                            temp.Append(temp2.Replace(".", ""));

                        TotalAmort += Convert.ToDouble(temp2);

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Trail
                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in record id trail";
                    Trail.Append("99");

                    errTrail = "Error in Total number of employees";
                    string temp = DetailCounter.ToString();
                    for (int idx = 0; idx < (6 - temp.Length); idx++)
                        Trail.Append("0");
                    Trail.Append(temp);

                    errTrail = "Error in Total Penalty paid";
                    string temp2 = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                    if (temp2.Length < 10)
                    {
                        int len = temp2.Length;
                        for (int idx = 0; idx < (10 - len); idx++)
                            Trail.Append("0");
                        Trail.Append(temp2);
                    }
                    else
                    {
                        Trail.Append(temp2.Substring(0, 10));
                    }

                    errTrail = "Error in Amort paid";
                    temp2 = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                    if (temp2.Length < 10)
                    {
                        int len = temp2.Length;
                        for (int idx = 0; idx < (10 - len); idx++)
                            Trail.Append("0");
                        Trail.Append(temp2);
                    }
                    else
                    {
                        Trail.Append(temp2.Substring(0, 10));
                    }

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }


                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSLoanMetrobankOnline(DataTable dt, string SubmissionType)
        {
            string strMsg = string.Empty;
                StringBuilder text = new StringBuilder();
                Int32 detailCounter = 0;
                double TotalPenalty = 0;
                double TotalAmort = 0;

            #region Header

                try
                {
                    text.Append("00");
                    //----------------------------------------------------------------------------
                    text.Append(this.RemittanceNumber.Trim());
                    //----------------------------------------------------------------------------
                    if (this.CompanyName.Trim().Length < 30)
                    {
                        text.Append(this.CompanyName.Trim().PadRight(30));
                    }
                    else
                    {
                        text.Append(this.CompanyName.Trim().Substring(0, 30));
                    }
                    //----------------------------------------------------------------------------
                    text.Append(YearMonth.Substring(2, 4));
                    text.Append(this.RemittanceBranchCode.Trim());
                    //12345678
                    text.Append("00000000");

                    text.Append(DateTime.Now.ToString("yyyy/MM/dd").Replace("/", ""));
                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                if (strMsg == string.Empty)
                {
                    try
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (text.Length > 0)
                            {
                                text.Append("\r\n");
                            }
                            //----------------------------------------------------------------------------
                            text.Append("10");
                            //----------------------------------------------------------------------------
                            text.Append(dt.Rows[i]["SSS Number"].ToString().Trim());
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Last Name"].ToString().Trim().Length < 15)
                            {
                                text.Append(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").PadRight(15));
                            }
                            else
                            {
                                string str = dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N");
                                text.Append(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 15));
                            }
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["First Name"].ToString().Trim().Length < 15)
                            {
                                text.Append(dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N").PadRight(15));
                            }
                            else
                            {
                                text.Append(dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 15));
                            }
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                text.Append(dt.Rows[i]["Middle Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 1) + " ");
                            }
                            else
                            {
                                text.Append("  ");
                            }
                            //----------------------------------------------------------------------------
                            text.Append(dt.Rows[i]["Rem Code"].ToString().Trim().PadLeft(1).Substring(0, 1));

                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Rem Date"].ToString().Trim() != string.Empty)
                            {
                                DateTime dtLoanDate = Convert.ToDateTime(dt.Rows[i]["Rem Date"].ToString().Trim());
                                text.Append(dtLoanDate.ToString("yyyy").Substring(2, 2) + dtLoanDate.ToString("MMdd"));
                            }
                            else
                            {
                                /////////123456
                                text.Append("      ");
                            }
                            //----------------------------------------------------------------------------
                            string strLoanAmount = string.Format("{0:0}", Convert.ToDouble(dt.Rows[i]["Loan Amt"].ToString().Trim()));
                            text.Append(strLoanAmount.PadLeft(6, '0'));
                            //----------------------------------------------------------------------------
                            text.Append("0000000");
                            //----------------------------------------------------------------------------
                            string strAmortAmount = dt.Rows[i]["EE"].ToString().Trim().Replace(".", "");
                            text.Append(strAmortAmount.PadLeft(7, '0'));
                            TotalAmort += Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim());
                            //----------------------------------------------------------------------------
                            text.Append(" ");
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Rem Code"].ToString().Trim() != string.Empty)
                            {
                                text.Append(dt.Rows[i]["Rem Code"].ToString().Trim().Substring(0, 1));
                            }
                            else
                            {
                                text.Append(" ");
                            }
                            //----------------------------------------------------------------------------
                            detailCounter++;
                        }
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Trail
                if (SubmissionType.ToUpper() == "B")
                {
                    if (strMsg == string.Empty)
                    {
                        try
                        {
                            if (text.Length > 0)
                            {
                                text.Append("\r\n");
                            }
                            //----------------------------------------------------------------------------
                            text.Append("99");
                            //----------------------------------------------------------------------------
                            DataView dv = new DataView(dt);
                            DataTable dtEmp = dv.ToTable(true, "ID Number");
                            string strTotalEmp = dtEmp.Rows.Count.ToString();
                            text.Append(strTotalEmp.PadLeft(4, '0'));
                            //----------------------------------------------------------------------------
                            string strTotalPenalty = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                            text.Append(strTotalPenalty.PadLeft(9, '0'));
                            //----------------------------------------------------------------------------
                            string strTotalAmort = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                            text.Append(strTotalAmort.PadLeft(9, '0'));

                        }
                        catch (Exception er)
                        {
                            strMsg += "Error : " + er.Message;
                        }
                    }
                }
                else if (SubmissionType.ToUpper() == "O")
                {
                    if (strMsg == string.Empty)
                    {
                        try
                        {
                            if (text.Length > 0)
                            {
                                text.Append("\r\n");
                            }
                            //----------------------------------------------------------------------------
                            text.Append("99");
                            //----------------------------------------------------------------------------
                            DataView dv = new DataView(dt);
                            DataTable dtEmp = dv.ToTable(true, "ID Number");
                            string strTotalEmp = dtEmp.Rows.Count.ToString();
                            text.Append(strTotalEmp.PadLeft(6, '0'));
                            //----------------------------------------------------------------------------
                            //string strTotalPenalty = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                            text.Append("0000000000");
                            //----------------------------------------------------------------------------
                            string strTotalAmort = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                            text.Append(strTotalAmort.PadLeft(10, '0'));
                            //----------------------------------------------------------------------------
                            /////////         1         2         3         4         5         6         7
                            /////////1234567890123456789012345678901234567890123456789012345678901234567890
                            //text += "                                                ";
                            //----------------------------------------------------------------------------
                        }
                        catch (Exception er)
                        {
                            strMsg += "Error : " + er.Message;
                        }
                    }
                }
                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Length == 0)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text.ToString());
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSLoanUnionBank(DataTable dt)
        {
            string strMsg = string.Empty;

            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalPenalty = 0;
            double TotalAmort = 0;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("00");

                    errtemp1 = "Error in Remittance Number";
                    string temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim().Replace(",", "").Replace(".", "");
                    if (temp2.Length < 30)
                    {
                        //int len = temp2.Length;
                        //header += temp2;
                        //for (int idx = 0; idx < (30 - len); idx++)
                        //    header += " ";
                        header.Append(temp2.PadRight(30, ' '));
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 30));
                    }

                    errtemp1 = "Error in Applicable Month";
                    header.Append(this.YearMonth.Substring(2, 4));

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dt.Rows[i]["Amort Paid"]) > 0)
                    {
                        StringBuilder temp = new StringBuilder();
                        string errTemp = string.Empty;
                        try
                        {
                            temp.Append("10");

                            errTemp = "Error in SSS Number";
                            if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                            {
                                throw new Exception();
                            }
                            else
                            {
                                temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                            }

                            string temp2 = string.Empty;
                            errTemp = "Error in Last name";
                            temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 15)
                            {
                                //temp += temp2;
                                //int len = temp2.Length;
                                // for (int idx = 0; idx < (15 - len); idx++)
                                //    temp += " ";
                                temp.Append(temp2.PadRight(15, ' '));
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 15));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in First name";
                            temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 15)
                            {
                                //temp += temp2;
                                //int len = temp2.Length;
                                //for (int idx = 0; idx < (15 - len); idx++)
                                //    temp += " ";
                                temp.Append(temp2.PadRight(15, ' '));
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 15));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in Middlename";
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                temp.Append(dt.Rows[i]["Middle Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper().Substring(0, 1).Replace("*", " ") + " ");
                            }
                            else
                            {
                                temp.Append("  ");
                            }

                            errTemp = "Error in Loan Type";
                            if (dt.Rows[i]["Loan Type"].ToString().Trim() != string.Empty)
                            {
                                temp.Append(dt.Rows[i]["Loan Type"].ToString().Trim().Substring(0, 1));
                            }
                            else
                            {
                                throw new Exception();
                            }


                        errTemp = "Error in Loan Date";
                            if (dt.Rows[i]["Loan Date"].ToString().Trim() != string.Empty)
                            {
                                DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Loan Date"].ToString().Trim());
                                temp2 = dtTemp.ToString("yyMMdd");
                                temp.Append(temp2);
                            }
                            else
                            {
                                throw new Exception();
                            }

                            errTemp = "Error in Loan Amount";
                            if (dt.Rows[i]["Loan Amt"].ToString().Trim() != string.Empty)
                            {
                                double d = Convert.ToDouble(dt.Rows[i]["Loan Amt"].ToString());
                                temp2 = string.Format("{0:0}", d);
                                if (temp2.Length < 6)
                                {
                                    //for (int idx = 0; idx < (6 - temp2.Length); idx++)
                                    //    temp += "0";
                                    temp.Append(temp2.PadLeft(6, '0'));
                                }
                                else
                                {
                                    temp.Append(temp2);
                                }
                            }
                            else
                            {
                                temp.Append(string.Empty);
                            }

                            errTemp = "Error in Penalty Amount";
                            temp2 = dt.Rows[i]["Penalty Amt"].ToString().Trim();
                            temp2 = temp2.Replace(".", "");
                            if (temp2.Length < 7)
                            {
                                //for (int idx = 0; idx < (7 - temp2.Length); idx++)
                                //    temp += "0";
                                temp.Append(temp2.PadLeft(7, '0'));
                            }
                            else
                            {
                                temp.Append(temp2);
                            }

                            TotalPenalty += (dt.Rows[i]["Penalty Amt"].ToString().Trim() == "") ? 0 : Convert.ToDouble(dt.Rows[i]["Penalty Amt"].ToString().Trim());

                            errTemp = "Error in Loan Amount";
                            temp2 = dt.Rows[i]["Amort Paid"].ToString().Trim();
                            if (temp2.Replace(".", "").Length < 7)
                                temp.Append(temp2.Replace(".", "").PadLeft(7, '0'));
                            else
                                temp.Append(temp2.Replace(".", "").Substring(0, 7));

                            TotalAmort += (temp2 == "") ? 0 : Convert.ToDouble(temp2);

                            //errTemp = "Error in Amort paid Sign";
                            //temp += dt.Rows[i]["Amort Paid Sign"].ToString().Trim();

                            errTemp = "Error in AMPSDG";
                            temp.Append(" ");

                            errTemp = "Error in Rem Code";
                            temp2 = dt.Rows[i]["Rem Code"].ToString();
                            if (temp2.Trim() != string.Empty)
                            {
                                if (temp2.ToUpper().Trim() == "H")
                                    temp.Append("H");
                                else if (temp2.ToUpper().Trim() == "S")
                                    temp.Append("T");
                                else
                                    temp.Append(temp2.Substring(0,1));
                            }
                            else
                            {
                                temp.Append(" ");
                            }

                            //errTemp = "Error in Remarks";
                            //temp += (dt.Rows[i]["Remarks"].ToString().Trim() == "") ? " " : dt.Rows[i]["Remarks"].ToString().Trim().Substring(0, 1);

                            text += temp + "\r\n";
                            ++DetailCounter;
                        }
                        catch (Exception er)
                        {
                            strMsg += "Error : " + er.Message;
                        }
                    }
                }
                #endregion

            #region Trail
                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in Record ID Trail";
                    Trail.Append("99");

                    errTrail = "Error in Total number of employees";
                    string temp = DetailCounter.ToString();
                    for (int idx = 0; idx < (4 - temp.Length); idx++)
                        Trail.Append("0");
                    Trail.Append(temp);

                    errTrail = "Error in Total Penalty paid";
                    string temp2 = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                    if (temp2.Length < 9)
                    {
                        //int len = temp2.Length;
                        //for (int idx = 0; idx < (9 - len); idx++)
                        //    Trail += "0";
                        Trail.Append(temp2.PadLeft(9, '0'));
                    }
                    else
                    {
                        Trail.Append(temp2.Substring(0, 9));
                    }

                    errTrail = "Error in Amort paid";
                    temp2 = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                    if (temp2.Length < 9)
                    {
                        //int len = temp2.Length;
                        //for (int idx = 0; idx < (9 - len); idx++)
                        //    Trail += "0";
                        Trail.Append(temp2.PadLeft(9, '0'));
                    }
                    else
                    {
                        Trail.Append(temp2.Substring(0, 9));
                    }

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }


                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSLoanSBC(DataTable dt)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalPenalty = 0;
            double TotalAmort = 0;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("00");

                    errtemp1 = "Error in Remittance Number";
                    string temp2 = this.RemittanceNumber.Replace(" ", "").Replace("-", "");
                    if (temp2.Length < 10)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        header.Append(temp2);
                    }
                    temp2 = string.Empty;

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim();
                    if (temp2.Length < 30)
                    {
                        header.Append(temp2.PadRight(30, ' '));
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 30));
                    }

                    errtemp1 = "Error in Applicable Month";
                    header.Append(this.YearMonth.Substring(2, 4));

                    errtemp1 = "Error in Branch Code";
                    temp2 = this.RemittanceBranchCode;
                    if (temp2.Length >= 2)
                        header.Append(temp2.Substring(0, 2));
                    else
                        header.Append(temp2.PadLeft(2, '0'));

                    errtemp1 = "Error in Repayment Number";
                    header.Append("SBC" + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "9");

                    errtemp1 = "Error in Repayment Date";
                    header.Append(this.TransactDate.Year.ToString() + this.TransactDate.Month.ToString().PadLeft(2, '0') + this.TransactDate.Day.ToString().PadLeft(2, '0'));

                    errtemp1 = "Error in bank branch code";
                    temp2 = this.RemittanceBranchCode;
                    if (temp2.Length >= 8)
                        header.Append(temp2.Substring(0, 8));
                    else
                        throw new Exception();

                    text += header;
                    text += "\r\n";
                }
                catch (Exception er)
                {
                    strMsg += "Error : " + errtemp1;
                }

                #endregion

            #region Details
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    int j = 0;
                    string errTemp = string.Empty;
                    try
                    {
                        temp.Append("10");

                        errTemp = "Error in SSS Number";
                        if (dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp.Append(dt.Rows[i]["SSS Number"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10));
                        }

                        string temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp.Append(temp2.PadRight(15, ' '));
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 15));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp.Append(temp2.PadRight(15, ' '));
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 15));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                        {
                            temp.Append(dt.Rows[i]["Middle Name"].ToString().Trim().ToUpper().Substring(0, 1) + " ");
                        }
                        else
                        {
                            temp.Append("  ");
                        }

                        errTemp = "Error in Loan Type";
                        temp.Append(dt.Rows[i]["Loan Type"].ToString().Trim());


                        errTemp = "Error in Loan Date";
                        if (dt.Rows[i]["Check Date"].ToString().Trim() != string.Empty)
                        {
                            temp2 = dt.Rows[i]["Check Date"].ToString().Trim().Replace("/", "");
                            temp.Append(temp2.Substring(6, 2) + temp2.Substring(0, 2) + temp2.Substring(2, 2));
                        }
                        else
                        {
                            temp.Append(string.Empty);
                        }

                        errTemp = "Error in Loan Amount";
                        if (dt.Rows[i]["Loan Amt"].ToString().Trim() != string.Empty)
                        {
                            double d = Convert.ToDouble(dt.Rows[i]["Loan Amt"].ToString());
                            temp2 = string.Format("{0:0}", d);
                            if (temp2.Length < 6)
                            {
                                temp.Append(temp2.PadLeft(6, '0'));
                            }
                            else
                            {
                                temp.Append(temp2);
                            }
                        }
                        else
                        {
                            temp.Append(string.Empty);
                        }

                        errTemp = "Error in Penalty Amount";
                        temp2 = dt.Rows[i]["Penalty Amt"].ToString().Trim();
                        temp2 = temp2.Replace(".", "");
                        if (temp2.Length < 7)
                        {
                            temp.Append(temp2.PadLeft(7, '0'));
                        }
                        else
                        {
                            temp.Append(temp2);
                        }

                        TotalPenalty += Convert.ToDouble(dt.Rows[i]["Penalty Amt"].ToString().Trim());

                        errTemp = "Error in Amort Amount";
                        temp2 = dt.Rows[i]["Amort Paid"].ToString().Trim();
                        if (temp2.Replace(".", "").Length < 7)
                        {
                            temp.Append(temp2.Replace(".", "").PadLeft(7, '0'));
                        }
                        else
                            temp.Append(temp2.Replace(".", "").Substring(0, 7));

                        TotalAmort += Convert.ToDouble(temp2);


                        errTemp = "Error in Amort paid Sign";
                        if (dt.Rows[i]["Amort Paid Sign"].ToString().Trim() == "")
                            temp.Append(" ");
                        else
                            temp.Append(dt.Rows[i]["Amort Paid Sign"].ToString());

                        errTemp = "Error in Rem Code";

                        if (dt.Rows[i]["Rem Code"].ToString().Trim() == string.Empty)
                        {
                            temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(dt.Rows[i]["Rem Code"].ToString().Trim());
                        }
                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + errTemp;
                    }
                }

                #endregion

            #region Trailer

                StringBuilder Trail = new StringBuilder();
                string errTrail = string.Empty;

                try
                {
                    errTrail = "Error in record id trail";
                    Trail.Append("99");

                    errTrail = "Error in Total number of employees";
                    string temp = DetailCounter.ToString().PadLeft(6, '0');
                    Trail.Append(temp);

                    errTrail = "Error in Total Penalty paid";
                    string temp2 = string.Format("{0:0.00}", TotalPenalty).Replace(".", "").PadLeft(10, '0');
                    Trail.Append(temp2);

                    errTrail = "Error in Amort paid";
                    temp2 = string.Format("{0:0.00}", TotalAmort).Replace(".", "").PadLeft(10, '0');
                    Trail.Append(temp2);

                    text += Trail;

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + errTrail;
                }


                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileSSSLoanBPI(DataTable dt)
        {
            string strMsg = string.Empty;

            StringBuilder text = new StringBuilder();
            Int32 detailCounter = 0;
            double TotalPenalty = 0;
            double TotalAmort = 0;
            string temp2 = string.Empty;

            #region Header
                try
                {
                    text.Append("00");
                    //----------------------------------------------------------------------------
                    text.Append(this.RemittanceNumber.Trim());
                    //----------------------------------------------------------------------------
                    text.Append(CommonBL.PadSubString(this.CompanyName.Trim(), 30));
                    //----------------------------------------------------------------------------
                    text.Append(YearMonth.Substring(2, 4));
                    //----------------------------------------------------------------------------
                    temp2 = this.RemittanceBranchCode;
                    text.Append(CommonBL.PadSubString(temp2, 2, CommonBL.PADDIRECTION.RIGHT, '0'));
                    //----------------------------------------------------------------------------

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                if (strMsg == string.Empty)
                {
                    try
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (text.Length > 0)
                            {
                                text.Append("\r\n");
                            }
                            //----------------------------------------------------------------------------
                            text.Append("10");
                            //----------------------------------------------------------------------------
                            text.Append(CommonBL.PadSubString(dt.Rows[i]["SSS Number"].ToString().Trim(), 10));
                            //----------------------------------------------------------------------------
                            text.Append(CommonBL.PadSubString(dt.Rows[i]["Last Name"].ToString().Trim().Replace("Ñ", "N"), 15));
                            //----------------------------------------------------------------------------
                            text.Append(CommonBL.PadSubString(dt.Rows[i]["First Name"].ToString().Trim().Replace("Ñ", "N"), 15));
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Middle Name"].ToString().Trim() != string.Empty)
                            {
                                text.Append(dt.Rows[i]["Middle Name"].ToString().Trim().Replace("Ñ", "N").Substring(0, 1) + " ");
                            }
                            else
                            {
                                text.Append("  ");
                            }
                            //----------------------------------------------------------------------------
                            //if (dt.Rows[i]["Deduction Code"].ToString().Trim() == "SSSLOAN")
                            //{
                            //    text += "S";
                            //}
                            //else if (dt.Rows[i]["Deduction Code"].ToString().Trim().IndexOf("SLER") != -1)
                            //{
                            //    text += "1";
                            //}
                            //else
                            //{
                            //    text += " ";
                            //}
                            text.Append(dt.Rows[i]["Rem Code"].ToString().Trim());
                            //----------------------------------------------------------------------------
                            if (dt.Rows[i]["Rem Date"].ToString().Trim() != string.Empty)
                            {
                                DateTime dtLoanDate = Convert.ToDateTime(dt.Rows[i]["Rem Date"].ToString().Trim());
                                text.Append(dtLoanDate.ToString("yyyy").Substring(2, 2) + dtLoanDate.ToString("MMdd"));
                            }
                            else
                            {
                                /////////123456
                                text.Append("      ");
                            }
                            //----------------------------------------------------------------------------
                            string strLoanAmount = string.Format("{0:0}", Convert.ToDouble(dt.Rows[i]["Loan Amt"].ToString().Trim()));
                            text.Append(strLoanAmount.PadLeft(6, '0'));
                            //----------------------------------------------------------------------------
                            text.Append("0000000");
                            //----------------------------------------------------------------------------
                            string strAmortAmount = dt.Rows[i]["EE"].ToString().Trim().Replace(".", "");
                            text.Append(strAmortAmount.PadLeft(7, '0'));
                            TotalAmort += Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim());
                            //----------------------------------------------------------------------------
                            text.Append(" ");
                            //----------------------------------------------------------------------------
                            text.Append(CommonBL.PadSubString(dt.Rows[i]["Rem Code"].ToString().Trim(), 1));
                            //----------------------------------------------------------------------------
                            detailCounter++;
                        }
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Trail
                if (strMsg == string.Empty)
                {
                    try
                    {
                        if (text.Length > 0)
                        {
                            text.Append("\r\n");
                        }
                        //----------------------------------------------------------------------------
                        text.Append("99");
                        //----------------------------------------------------------------------------
                        DataView dv = new DataView(dt);
                        DataTable dtEmp = dv.ToTable(true, "ID Number");
                        string strTotalEmp = dtEmp.Rows.Count.ToString();
                        text.Append(strTotalEmp.PadLeft(6, '0'));
                        //----------------------------------------------------------------------------
                        string strTotalPenalty = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                        text.Append(strTotalPenalty.PadLeft(10, '0'));
                        //----------------------------------------------------------------------------
                        string strTotalAmort = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                        text.Append(strTotalAmort.PadLeft(10, '0'));
                        //----------------------------------------------------------------------------
                        /////////         1         2         3         4         5         6         7
                        /////////1234567890123456789012345678901234567890123456789012345678901234567890
                        //text += "                                                ";
                        //----------------------------------------------------------------------------
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Length == 0)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text.ToString());
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileHDMFGovernment(DataTable dt, string EmployerType, string HDMFType)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("EH");

                    errtemp1 = "Error in header Branch Code";
                    header.Append(this.RemittanceBranchCode.Substring(0, 2));

                    header.Append(this.YearMonth);

                    errtemp1 = "Error in header SSS";
                    temp2 = this.RemittanceNumber.Trim().Replace("-", "");
                    if (temp2.Length < 15)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (15 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 15));
                    }

                    errtemp1 = "Error in Employer Type, Paytype";
                    header.Append(EmployerType.Trim() + HDMFType.Trim());

                    errtemp1 = "Error in Employer name";
                    temp2 = this.CompanyName.Trim();
                    if (temp2.Length < 100)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (100 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 100));
                    }

                    errtemp1 = "Error in Employer Address";
                    if (this.Address != string.Empty)
                        temp2 = dsComp.Tables[0].Rows[0]["Address1"].ToString().Trim();
                    if (dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() != string.Empty)
                        temp2 += dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim();
                    if (dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim() != string.Empty)
                        temp2 += dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim();


                    if (temp2.Length < 107)
                    {
                        header.Append(temp2);
                        for (int idx = 0; idx < (100 - temp2.Length); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 100));
                    }

                    temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim();
                    if (temp2.Length < 7)
                    {
                        header.Append(temp2);
                        for (int idx = 0; idx < (7 - temp2.Length); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 7));
                    }

                    if (dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length < 15)
                    {
                        int len = dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length;
                        header.Append(dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", ""));

                    }
                    else
                    {
                        header.Append(dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Substring(0, 15));
                    }

                    text += header;
                    text += "\r\n";

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        temp2 = string.Empty;

                        temp.Append("DT");

                        errTemp = "Error in HDMF Number";
                        temp2 = dt.Rows[i]["HDMF Number"].ToString();
                        if (temp2 == string.Empty)
                        {
                            //123456789012345678901234567890
                            temp.Append("            ");
                        }
                        else if (temp2.Trim().Replace("-", "").Replace(" ", "").Length < 12)
                        {
                            int len = temp2.Trim().Replace("-", "").Replace(" ", "").Length;
                            temp.Append(temp2);
                            for (int idx = 0; idx < (12 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Trim().Replace("-", "").Replace(" ", "").Substring(0, 12));
                        }

                        errTemp = "Error in Employee Number";
                        temp2 = dt.Rows[i]["ID Number"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        if (temp2.Length < 15)
                        {
                            int len = temp2.Length;
                            temp.Append(temp2);
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 15));
                        }


                        errTemp = "Error in Last name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 30));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 30));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middle name";
                        temp2 = dt.Rows[i]["Middle Name"].ToString().Trim().ToUpper();
                        if (temp2 == string.Empty)
                        {
                            //123456789012345678901234567890
                            temp.Append("                              ");
                        }
                        else if (temp2.Length < 30)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 30));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in PagIbig share Values";
                        int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = dt.Rows[i]["EE"].ToString().Trim();
                        temp3 = dt.Rows[i]["ER"].ToString().Trim();

                        TotalContribEE += Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(dt.Rows[i]["ER"].ToString().Trim());

                        if (temp2.Length < 13)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (13 - len); idx++)
                                temp2 += " ";
                            temp.Append(temp2);
                        }
                        if (temp3.Length < 13)
                        {
                            int len = temp3.Length;
                            for (int idx = 0; idx < (13 - len); idx++)
                                temp3 += " ";
                            temp.Append(temp3);
                        }

                        errTemp = "Error in TIN number";
                        temp2 = dt.Rows[i]["TIN Number"].ToString().Trim().Replace("-", "");
                        if (temp2 == string.Empty)
                        {
                            //123456789012345678901234567890
                            temp.Append("               ");
                        }
                        else if (temp2.Length < 15)
                        {
                            temp.Append(temp2);
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp.Append(" ");
                        }
                        else
                        {
                            temp.Append(temp2.Substring(0, 15));
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Birth Date";
                        DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Birth Date"].ToString().Trim());
                        temp.Append(dtTemp.ToString("MMddyyyy").Substring(0, 8)); //NOT SURE SA FORMAT


                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Write To Textfile   
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion
            return strMsg;
        }

        public string GenerateTextfileHDMFUnionBank(DataTable dt, string EmployerType, string HDMFType, string HDMFMode)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("EH");

                    errtemp1 = "Error in header branch code";
                    header.Append(CommonBL.PadSubString(this.RemittanceBranchCode.Trim(), 2));

                    header.Append(this.YearMonth);

                    errtemp1 = "Error in Comapny SSS ID Number";
                    temp2 = this.RemittanceNumber.Trim().Replace("-", "");
                    if (temp2.Length < 15)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (15 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 15));
                    }

                    errtemp1 = "Error in Employer Type";
                    header.Append(EmployerType);

                    errtemp1 = "Error in Pay Type";
                    header.Append(CommonBL.PadSubString(HDMFType, 2));

                    errtemp1 = "Error in Employer Name";
                    temp2 = this.CompanyName.Trim().Replace(",", "").Replace(".", "");
                    if (temp2.Length < 100)
                    {
                        int len = temp2.Length;
                        header.Append(temp2);
                        for (int idx = 0; idx < (100 - len); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 100));
                    }

                    errtemp1 = "Error in Employer Address";
                    if (dsComp.Tables[0].Rows[0]["Address1"].ToString() != string.Empty)
                        temp2 = dsComp.Tables[0].Rows[0]["Address1"].ToString().Trim() + " ";
                    if (dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() != string.Empty)
                        temp2 += dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() + " ";

                    if (temp2.Length < 100)
                    {
                        header.Append(temp2);
                        for (int idx = 0; idx < (100 - temp2.Length); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(temp2.Substring(0, 100));
                    }

                    if (dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim() != string.Empty)
                    {
                        temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim();
                        header.Append(temp2);
                        for (int idx = 0; idx < (7 - temp2.Length); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        throw new Exception("Postal Code Number required.");
                    }

                    if (dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length < 15)
                    {
                        header.Append(dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", ""));

                        for (int idx = 0; idx < (15 - dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length); idx++)
                            header.Append(" ");
                    }
                    else
                    {
                        header.Append(dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Substring(0, 15));
                    }

                    text += header;
                    text += "\r\n";

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                int blankCounter = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double EmpShare = (HDMFType == "MC" ? Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim()) : Convert.ToDouble(dt.Rows[i]["Amort Paid"].ToString().Trim()));
                    if (EmpShare > 0)
                    {
                        StringBuilder temp = new StringBuilder();
                        string errTemp = string.Empty;
                        try
                        {
                            temp2 = string.Empty;

                            temp.Append("DT");

                            errTemp = "Error in HDMF Number";
                            temp2 = dt.Rows[i]["HDMF Number"].ToString().Trim().Replace("-", "").Replace(" ", "");

                            if (temp2 == string.Empty || temp2 == "APPLIED")
                            {
                                //123456789012345678901234567890
                                temp.Append(string.Format("{0}", blankCounter++).PadRight(12));
                            }
                            else if (temp2.Length < 12)
                            {
                                int len = temp2.Length;
                                temp.Append(temp2);
                                for (int idx = 0; idx < (12 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 12));
                            }

                            errTemp = "Error in Employee Number";
                            temp2 = dt.Rows[i]["ID Number"].ToString().Trim().Replace("-", "").Replace(" ", "");
                            if (temp2.Length < 15)
                            {
                                int len = temp2.Length;
                                temp.Append(temp2);
                                for (int idx = 0; idx < (15 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 15));
                            }


                            errTemp = "Error in Last name";
                            temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 30)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (30 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 30));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in First name";
                            temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Replace("-", "").Trim().ToUpper();
                            if (temp2.Length < 30)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (30 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 30));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in Middle name";
                            temp2 = dt.Rows[i]["Middle Name"].ToString().Replace("Ñ", "N").Replace("-", "").Replace("*", " ").Trim().ToUpper();
                            if (temp2 == string.Empty)
                            {
                                //123456789012345678901234567890
                                temp.Append("                              ");
                            }
                            else if (temp2.Length < 30)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (30 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 30));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in HDMF share Values";
                            int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));
                            string temp3 = string.Empty;
                            if (HDMFType ==  "MC")
                                temp2 = dt.Rows[i]["EE"].ToString().Trim();
                            else
                                temp2 = dt.Rows[i]["Amort Paid"].ToString().Trim();
                            TotalContribEE += (HDMFType == "MC" ? Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim()) : Convert.ToDouble(dt.Rows[i]["Amort Paid"].ToString().Trim()));

                            if (HDMFType == "MC")
                            {
                                temp3 = dt.Rows[i]["ER"].ToString().Trim();
                                TotalContribER += Convert.ToDouble(dt.Rows[i]["ER"].ToString().Trim());
                            }

                            if (temp2.Length < 13)
                            {
                                int len = temp2.Length;
                                string spaces = "";
                                for (int idx = 0; idx < (13 - len); idx++)
                                    spaces += " ";
                                temp.Append(spaces + temp2);
                            }
                            if (temp3.Length < 13)
                            {
                                int len = temp3.Length;
                                string spaces = "";
                                for (int idx = 0; idx < (13 - len); idx++)
                                    spaces += " ";
                                temp.Append(spaces + temp3);
                            }

                            errTemp = "Error in TIN number";
                            temp2 = dt.Rows[i]["TIN Number"].ToString().Trim().Replace("-", "");
                            if (temp2 == string.Empty)
                            {
                                //123456789012345678901234567890
                                temp.Append("               ");
                            }
                            else if (temp2.Length < 15)
                            {
                                temp.Append(temp2);
                                int len = temp2.Length;
                                for (int idx = 0; idx < (15 - len); idx++)
                                    temp.Append(" ");
                            }
                            else
                            {
                                temp.Append(temp2.Substring(0, 15));
                            }
                            temp2 = string.Empty;

                            errTemp = "Error in Birth Date";

                            if (dt.Rows[i]["Birth Date"].ToString().Trim() != string.Empty)
                            {
                                DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Birth Date"].ToString());
                                temp.Append(dtTemp.ToString("yyyyMMdd").Substring(0, 8));
                            }
                            else
                            {
                                temp.Append("        ");
                            }


                            text += temp + "\r\n";
                            ++DetailCounter;
                        }
                        catch (Exception er)
                        {
                            strMsg += "Error : " + er.Message;
                        }
                    }
                }
            #endregion

            #region Write To Textfile
            if (strMsg == string.Empty)
            {
                if (text.Trim() == string.Empty)
                {
                    strMsg += "\nNo government remittance generated.";
                }
                else
                {
                    TextWriter txtWriter = new StreamWriter(this.Filename);
                    txtWriter.WriteLine(text);
                    txtWriter.Close();
                }
            }
            #endregion

            return strMsg;
        }


        public string GenerateTextfileHDMFMetroBank(DataTable dt, string EmployerType, string HDMFType)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("EH");

                    errtemp1 = "Error in header Branch Code";
                    header.Append(CommonBL.PadSubString(this.RemittanceBranchCode.Trim(), 2));

                    errtemp1 = "Error in period";
                    header.Append(CommonBL.PadSubString(YearMonth, 6));


                    temp2 = this.RemittanceNumber.Trim().Replace("-", "");
                    header.Append(CommonBL.PadSubString(temp2, 15));

                    errtemp1 = "Error in Employer Type, Paytype";
                    header.Append(CommonBL.PadSubString(EmployerType, 1));
                    header.Append(CommonBL.PadSubString(HDMFType, 2));

                    errtemp1 = "Error in Employer name";
                    temp2 = this.CompanyName.Trim();
                    header.Append(CommonBL.PadSubString(temp2, 100));

                    errtemp1 = "Error in Employer Address";
                    temp2 = string.Format("{0} {1} {2}", dsComp.Tables[0].Rows[0]["Address1"].ToString().Replace("Ñ", "N").Trim()
                    , dsComp.Tables[0].Rows[0]["Address2"].ToString().Replace("Ñ", "N").Trim()
                    , dsComp.Tables[0].Rows[0]["Address3"].ToString().Replace("Ñ", "N").Trim());
                    header.Append(CommonBL.PadSubString(temp2, 100));

                    errtemp1 = "Error in Zip Code";
                    temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim();
                    header.Append(CommonBL.PadSubString(temp2, 7));

                    errtemp1 = "Error in Telephone";
                    temp2 = dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "");
                    header.Append(CommonBL.PadSubString(temp2, 15));

                    text += header;

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        if (text.Trim() != string.Empty)
                        {
                            text += "\r\n";
                        }
                        temp2 = string.Empty;

                        temp.Append("DT");

                        errTemp = "Error in HDMF Number";
                        temp2 = dt.Rows[i]["HDMF Number"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        temp.Append(CommonBL.PadSubString(temp2, 12));

                        errTemp = "Error in Employee Number";
                        temp2 = dt.Rows[i]["ID Number"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        temp.Append(CommonBL.PadSubString(temp2, 15));

                        errTemp = "Error in Last name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 30));

                        errTemp = "Error in First name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 30));

                        errTemp = "Error in Middle name";
                        temp2 = dt.Rows[i]["Middle Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 30));

                        errTemp = "Error in PagIbig share Values";
                        int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = dt.Rows[i]["EE"].ToString().Trim().PadLeft(13, '0');
                        temp3 = dt.Rows[i]["ER"].ToString().Trim().PadLeft(13, '0');

                        TotalContribEE += Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(dt.Rows[i]["ER"].ToString().Trim());

                        temp.Append(CommonBL.PadSubString(temp2, 13));
                        temp.Append(CommonBL.PadSubString(temp3, 13));

                        errTemp = "Error in TIN number";
                        temp2 = dt.Rows[i]["TIN Number"].ToString().Trim().Replace("-", "");
                        temp.Append(CommonBL.PadSubString(temp2, 15));

                        errTemp = "Error in Birth Date";
                        DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Birth Date]"].ToString().Trim());
                        temp2 = dtTemp.ToString("MMddyyyy"); //NOT SURE SA FORMAT
                        temp.Append(CommonBL.PadSubString(temp2, 8));

                        text += temp;
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
                #endregion

            #region Write To Textfile
                if (strMsg == string.Empty)
                {
                    if (text.Trim() == string.Empty)
                    {
                        strMsg += "\nNo government remittance generated.";
                    }
                    else
                    {
                        TextWriter txtWriter = new StreamWriter(this.Filename);
                        txtWriter.WriteLine(text);
                        txtWriter.Close();
                    }
                }
                #endregion

            return strMsg;
        }

        public string GenerateTextfileHDMFBPI(DataTable dt, string EmployerType, string HDMFType)
        {
            string strMsg = string.Empty;
            string text = string.Empty;
            int DetailCounter = 0;
            string Error = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;

            #region Header

                StringBuilder header = new StringBuilder();
                string errtemp1 = string.Empty;
                try
                {
                    header.Append("EH");

                    errtemp1 = "Error in header branch code";
                    header.Append(CommonBL.PadSubString(this.RemittanceBranchCode.Trim(), 2));

                    errtemp1 = "Error in period";
                    header.Append(CommonBL.PadSubString(YearMonth, 6));

                    errtemp1 = "Error in header SSS";
                    temp2 = this.RemittanceNumber.Trim().Replace("-", "");
                    header.Append(CommonBL.PadSubString(temp2, 15));

                    errtemp1 = "Error in Employer Type, Pay Type";
                    header.Append(CommonBL.PadSubString(EmployerType, 1));
                    header.Append(CommonBL.PadSubString(HDMFType, 2));

                    errtemp1 = "Error in Employer name";
                    temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                    header.Append(CommonBL.PadSubString(temp2, 100));

                    errtemp1 = "Error in Employer Address";
                    temp2 = string.Format("{0} {1} {2}", dsComp.Tables[0].Rows[0]["Address1"].ToString().Replace("Ñ", "N").Trim()
                    , dsComp.Tables[0].Rows[0]["Address2"].ToString().Replace("Ñ", "N").Trim()
                    , dsComp.Tables[0].Rows[0]["Address3"].ToString().Replace("Ñ", "N").Trim());
                    header.Append(CommonBL.PadSubString(temp2, 100));

                    errtemp1 = "Error in Zip Code";
                    temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim();
                    header.Append(CommonBL.PadSubString(temp2, 7));

                    errtemp1 = "Error in Telephone";
                    temp2 = dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "");
                    header.Append(CommonBL.PadSubString(temp2, 15));

                    text += header;

                }
                catch (Exception er)
                {
                    strMsg += "Error : " + er.Message;
                }
                #endregion

            #region Detail
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder temp = new StringBuilder();
                    string errTemp = string.Empty;
                    try
                    {
                        if (text.Trim() != string.Empty)
                        {
                            text += "\r\n";
                        }
                        temp2 = string.Empty;

                        temp.Append("DT");

                        errTemp = "Error in HDMF Number";
                        temp2 = dt.Rows[i]["HDMF Number"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        temp.Append(CommonBL.PadSubString(temp2, 12));

                        errTemp = "Error in Employee Number";
                        temp2 = dt.Rows[i]["ID Number"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        temp.Append(CommonBL.PadSubString(temp2, 15));

                        errTemp = "Error in Last name";
                        temp2 = dt.Rows[i]["Last Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 30));

                        errTemp = "Error in First name";
                        temp2 = dt.Rows[i]["First Name"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 30));

                        errTemp = "Error in Middle name";
                        temp2 = dt.Rows[i]["Mem_MiddleName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        temp.Append(CommonBL.PadSubString(temp2, 30));

                        errTemp = "Error in PagIbig share Values";
                        int monthIndicator = Convert.ToInt32(this.YearMonth.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = dt.Rows[i]["EE"].ToString().Trim();
                        temp3 = dt.Rows[i]["ER"].ToString().Trim();

                        TotalContribEE += Convert.ToDouble(dt.Rows[i]["EE"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(dt.Rows[i]["ER"].ToString().Trim());

                        temp.Append(CommonBL.PadSubString(temp2, 13));
                        temp.Append(CommonBL.PadSubString(temp3, 13));

                        errTemp = "Error in TIN number";
                        temp2 = dt.Rows[i]["TIN Number"].ToString().Trim().Replace("-", "");
                        temp.Append(CommonBL.PadSubString(temp2, 15));

                        errTemp = "Error in Birth Date";
                        DateTime dtTemp = Convert.ToDateTime(dt.Rows[i]["Birth Date"].ToString().Trim());
                        temp2 = dtTemp.ToString("yyyyMMdd").ToString();  //YYYYMMDD
                        temp.Append(CommonBL.PadSubString(temp2, 8));

                        text += temp;
                        ++DetailCounter;
                    }
                    catch (Exception er)
                    {
                        strMsg += "Error : " + er.Message;
                    }
                }
            #endregion

            #region Write To Textfile
            if (strMsg == string.Empty)
            {
                if (text.Trim() == string.Empty)
                {
                    strMsg += "\nNo government remittance generated.";
                }
                else
                {
                    TextWriter txtWriter = new StreamWriter(this.Filename);
                    txtWriter.WriteLine(text);
                    txtWriter.Close();
                }
            }
            #endregion

            return strMsg;
        }

        private DataSet GetCompanyInfo()
        {
            DataSet ds = new DataSet();

            #region query

            string query = @"
                           select 
	                            Case when Mcm_CompanyAddress1 <> ''
	                            then Mcm_CompanyAddress1
	                            else 
	                            ''
	                            end [Address1],
	                            Case when Mcm_CompanyAddress2 <> ''
	                            then Mcm_CompanyAddress2
	                            else 
	                            ''
	                            end [Address2],
	                            Case when Mcd_Name <> '' or Mcd_Name <> null
	                            then Mcd_Name
	                            else 
	                            ''
	                            end [Address3],
	                            M_Company.* 
	                            ,Mcd_Name [Address3]
                            from M_Company
                            left join M_CodeDtl
                            on Mcd_Code = Mcm_CompanyAddress3
                            and Mcd_CodeType = 'ZIPCODE'
                            ";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }
    }
}
