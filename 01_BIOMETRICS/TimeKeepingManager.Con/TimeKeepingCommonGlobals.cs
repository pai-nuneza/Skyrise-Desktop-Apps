using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.IO;
using System.Drawing;

namespace TimeKeepingManager.Con
{
    #region Global classes and variables

    public class DeviceGlobals
    {
        
        public static string _UsrLogin = "RFFPService";
        public static bool isDeviceCon = false;
        public static int DeviceLogCtr = 0;
        public static int DeviceDataCtr = 0;
        public static string TimeSched = "23:45";
        public static double Progress = 0;
        public static double MaxProgress = 0;
        public static bool VisibleBar = false;
        public static double TimeGap = 30.0;
        public static DataTable dtEmployeeIDMapping;
        public static DataTable dtStandardVerificaitonSchedule;
        //*****************************************************
        //
        // DirectDTR currently is preferable to deploy
        // Client using the staging to DTR_Device is CitiMart
        //
        //*****************************************************
        // true if reassigning of logtype is not needed [T_EmpDTR = true] [T_EmpDTRDevice = false]                                           
        public static bool DirectDTR 
        {
            get 
            { 
                    try
                    {
                        return ConfigurationManager.AppSettings["DirectDTR"].ToString().Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase) ? true : false;
                    }
                    catch
                    {
                        return true;
                    }
            }
        }
    }

    public class ErrCode
    {
        public static int DBDisconnected = -999;
        public static int DeviceErrDel = -997;
        public static bool DeviceCon = false;
        public static bool DeviceErrCon = false;
        public static bool DeviceNoData = false;
        public static bool DeviceNologs = false;
        public static bool NoAttLogs = true;
        public static int DeviceDupTmpl = 0;
        public static string ErrLog = "ApplicationLogs";
        public static string SrvLog = "ServiceLogs";
        public static string PathLog = Application.StartupPath;

        public static void createimage(string inputString)
        {
            byte[] imageBytes = Encoding.Unicode.GetBytes(inputString);

            // Don't need to use the constructor that takes the starting offset and length
            // as we're using the whole byte array.
            MemoryStream ms = new MemoryStream(imageBytes);

            Image image = Image.FromStream(ms, true, true);

        }
    }

    #endregion
}
