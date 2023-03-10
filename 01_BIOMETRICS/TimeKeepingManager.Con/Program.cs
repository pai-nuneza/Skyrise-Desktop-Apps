using System;
using System.Data;
//using System.Linq;
using System.Windows.Forms;

namespace TimeKeepingManager.Con
{
    class Program
    {   
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                //Setting DB tables
                Console.WriteLine("\n**************  TIME KEEPING MANAGER CONSOLE TRIGGER ***************"

                                    +"\n\nNote: Set the configurations before executing the ff. commands."
                                    + "\n[Run this console as administrator]"
                                    + "\n\n\nAvailable input arguments : "
                                    + "\n[1] SETUPTABLES"
                                    + "\n[2] SETDEVICETIME"
                                    + "\n[3] GETDEVICELOGS <Date of Logs>"
                                    + "\n[4] GETDEVICELOGS <Start Date> <End Date>"
                                    + "\n\nDescription"
                                    + "\n[1] Check database and create required tables."
                                    + "\n[2] Set all device time sychronized with server time."
                                    + "\n[3] Get device logs and record to DTR, with specified date."
                                    + "\n[4] Get device logs and record to DTR, with ranged date."
                                    + "\n\nENTER argument  :  ");
                
                string[] inputargs = Console.ReadLine().Split(new Char[] { ' ' });
                args = new string[inputargs.Length];
                args = inputargs;
            }
            
            string param = args[0];

            #region Setup Database Tables
            if (param.ToUpper() == "SETUPTABLES")
            {
                //Setting DB tables
                Console.WriteLine("\nStart setting up tables.\n");
                TimeKeepingSetupDatabase _setupDB = new TimeKeepingSetupDatabase();
                return;
            }
            #endregion

            #region service
            if (param.ToUpper() == "GETDEVICELOGS" || param.ToUpper() == "GETDEVICELOGS_SETDEVICETIME")
            {
                #region Get device raw Logs
                Console.WriteLine(string.Format("\nStart getting device logs\nDirect DTR : {0}", DeviceGlobals.DirectDTR));
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;

                if (args.Length == 2)
                {
                    StartDate = Convert.ToDateTime(args[1]);
                    EndDate = Convert.ToDateTime(args[1]);
                }
                if (args.Length == 3)
                {
                    StartDate = Convert.ToDateTime(args[1]);
                    EndDate = Convert.ToDateTime(args[2]);
                }

                TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();
                DeviceDataDownloadManager _downloadEngine = new DeviceDataDownloadManager(true);
                TimeKeepingMainMethods _mainMethods = new TimeKeepingMainMethods(_downloadEngine.GetConStrDTR());

                if (param != null)
                {
                    if (!_log.ConfigCopier(Application.StartupPath, "BiometricDeviceOrganizer", Application.StartupPath, "BiometricDeviceOrganizer.Con"))
                        return;

                    _log.WriteLog(Application.StartupPath, "Service", "GETDEVICELOGS SERVICE START", "Started", true);

                    string TableName = "T_EmpDTRDevice";  
                    if (DeviceGlobals.DirectDTR)
                        TableName = "T_EmpDTR"; 

                    #region Download Device Logs
                    string[] tmpDeviceIP;
                    DataTable dt = _downloadEngine.GetDevicePropertyfromDatabaseList();
                    for (int i = 0; i < dt.Rows.Count; ++i)
                    {
                        DeviceVersions.DeviceType = DeviceVersions.GetDeviceType(dt.Rows[0]["VERSION"].ToString().Trim());
                        tmpDeviceIP = dt.Rows[i]["DEVICEIP"].ToString().Split('_');
                        _mainMethods.GETDEVICELOGS(tmpDeviceIP[0].ToString()
                                                   , dt.Rows[i]["DEVICEPORT"].ToString().Trim()
                                                   , TableName
                                                   , StartDate
                                                   , EndDate
                                                   , "23:45" 
                                                   , false); //HARDCODED TO CREATE A TEXT FROM DEVICE ATTLOG EVERY 11:45 PM
                        //System.Threading.Thread.Sleep(5000);

                        if (param.ToUpper() == "GETDEVICELOGS_SETDEVICETIME")
                        {
                            Console.WriteLine("\nStart setting device time.\n");
                            //_log.WriteLog(Application.StartupPath, "Service", "SET DEVICE TIME", "Start", true);
                            _mainMethods.SYNCDEVICETIME(tmpDeviceIP[0].ToString(), dt.Rows[i]["DEVICEPORT"].ToString().Trim());
                            //_log.WriteLog(Application.StartupPath, "Service", "SET DEVICE TIME", "End", true);
                        }
                    }
                    _log.WriteLog(Application.StartupPath, "Service", "GETDEVICELOGS SERVICE END", "End", true);
                    #endregion

                    #region Set Correct IN OUT (if not direct to dtr)
                    if (args.Length != 3 && !DeviceGlobals.DirectDTR)
                    {
                        //return; //Called by the application not by the service

                        if (!_log.ConfigCopier(Application.StartupPath, "BiometricDeviceOrganizer", Application.StartupPath, "BiometricDeviceOrganizer.Con"))
                            return;

                        _log.WriteLog(Application.StartupPath, "Service", "Process", "Started", true);
                        #region Download Device Logs
                            _mainMethods.GETLOGSEQUENTIALMETHOD("T_EmpDTRDevice"
                                                       , StartDate.AddDays(-5)
                                                       , EndDate.AddDays(-1)
                                                       , false); //HARD CODED IN THE SERVICE TO RUN AT THE END OF THE DAY 11:55 PM and 11:59 PM
                    
                        _log.WriteLog(Application.StartupPath, "Service", "Process", "End", true);
                        #endregion
                    #endregion

                    }
                #endregion
                }
            }
            else if (param.ToUpper() == "GETMINMAXLOGS")
            {
                Console.WriteLine("\nStart uploading DTR DEVICE to DTR\n");

                TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();

                if (DeviceGlobals.DirectDTR)
                {
                    // Reassigning type is not needed
                    _log.WriteLog(Application.StartupPath, "Service", "MINMAX LOGS", "Disabled", true);
                }
                return;

                #region GetMinMax
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;
                if (args.Length == 2)
                {
                    StartDate = Convert.ToDateTime(args[1]);
                    EndDate = Convert.ToDateTime(args[1]);
                }
                if (args.Length == 3)
                {
                    StartDate = Convert.ToDateTime(args[1]);
                    EndDate = Convert.ToDateTime(args[2]);
                }
                if (param != null)
                {
                    if (!_log.ConfigCopier(Application.StartupPath, "BiometricDeviceOrganizer", Application.StartupPath, "BiometricDeviceOrganizer.Con"))
                        return;

                    _log.WriteLog(Application.StartupPath, "Service", "Process", "Started", true);
                    DeviceDataDownloadManager _downloadEngine = new DeviceDataDownloadManager(true);
                    TimeKeepingMainMethods _mainMethods = new TimeKeepingMainMethods(_downloadEngine.GetConStrDTR());
                    #region Download Device Logs
                        _mainMethods.GETLOGSEQUENTIALMETHOD("T_EmpDTRDevice"
                                                   , StartDate
                                                   , EndDate
                                                   , false); //HARD CODED IN THE SERVICE TO RUN AT THE END OF THE DAY 11:55 PM and 11:59 PM
                    
                    _log.WriteLog(Application.StartupPath, "Service", "Process", "End", true);
                    #endregion
                #endregion
                }
            }
            else if (param.ToUpper() == "SETDEVICETIME")
            {
                Console.WriteLine("\nStart setting device time.\n");

                TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();
                DeviceDataDownloadManager _downloadEngine = new DeviceDataDownloadManager(true);
                TimeKeepingMainMethods _mainMethods = new TimeKeepingMainMethods(_downloadEngine.GetConStrDTR());

                #region Sychronize device time from database
                DataTable dt = _downloadEngine.GetDevicePropertyfromDatabaseList();
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    string[] tmpDeviceIP;
                    tmpDeviceIP = dt.Rows[i]["DEVICEIP"].ToString().Split('_');
                    string IP = tmpDeviceIP[0].ToString();
                    string Port = dt.Rows[i]["DEVICEPORT"].ToString().Trim();
                    _log.WriteLog(Application.StartupPath, "Service", "SETDEVICETIME SERVICE START", "Start", true);
                    _mainMethods.SYNCDEVICETIME(IP, Port);
                    //System.Threading.Thread.Sleep(5000);
                }

                #endregion

                _log.WriteLog(Application.StartupPath, "Service", "SETDEVICETIME SERVICE END", "End", true);

                return;
            }
            else
            {
                Console.WriteLine("Invalid argument. (Application will exit)");
                System.Threading.Thread.Sleep(5000);
                return;
            }
            #endregion
        }
    }
}
