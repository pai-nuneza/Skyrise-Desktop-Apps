using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using CommonLibrary;
using System.Threading;

namespace TimeKeepingManager.Service
{
    public partial class TimeKeepingManagerService : ServiceBase
    {
        public TimeKeepingManagerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TimeKeepingManager.Con.Logger _logger = new TimeKeepingManager.Con.Logger();
            try
            {
                // call scheduler for the service
                _logger.WriteLog(Application.StartupPath, "Service", "Service Started", "", true);

                ServiceScheduler _servicescheduler_c = new ServiceScheduler();
                _servicescheduler_c.InitializeServiceSchedule();
            }
            catch (Exception ex)
            {
                _logger.WriteLog(Application.StartupPath, "Service", "Error", ex.ToString(), true);
            }

        }

        protected override void OnStop()
        {
            TimeKeepingManager.Con.Logger _logger = new TimeKeepingManager.Con.Logger();
            try
            {
                // call scheduler for the service
                _logger.WriteLog(Application.StartupPath, "Service", "Service Stop", "", true);
            }
            catch
            { }
        }
        // methods
        public void StartServiceConsoleProcess()
        {
            TimeKeepingManager.Con.Logger _logger = new TimeKeepingManager.Con.Logger();
            try
            {
                //Author : Nilo Luansing Jr. <nlluansing@n-pax.com>

                // reading xml configuration
                string _consolePath = (ConfigurationManager.AppSettings["BIOMETRICConsolePath"].ToString().Trim());
                string _consoleName = (ConfigurationManager.AppSettings["BIOMETRICConsoleName"].ToString().Trim());
                bool _BIOMETRICSychTime = (ConfigurationManager.AppSettings["BIOMETRICSychTime"].ToString().Trim().ToUpper()).Equals("TRUE") ? true : false;
                string GETDEVICELOGS = "GETDEVICELOGS";
                string GETMINMAXLOGS = "GETMINMAXLOGS";
                string SETDEVICETIME = "SETDEVICETIME";
                string Slash = @"\";
                string[] BIOMETRICPeriods = new string[30]; // max of 30 times to load all option
                BIOMETRICPeriods = (ConfigurationManager.AppSettings["BIOMETRICPeriods"].ToString().Trim().Split(','));

                if (_consoleName != "")
                {
                    try
                    {// get hours check if time to upload : HARDCODED TO LOG A TEXT FILE FROM DEVICE EVERY 11:45 PM
                        if (_consolePath.Substring(_consolePath.Length - 1, 1) == Slash)
                            Slash = "";
                        for (int _bioperiod = 0; _bioperiod < BIOMETRICPeriods.Length; _bioperiod++)
                        {
                            if (BIOMETRICPeriods[_bioperiod].Trim().Replace(":", "") == DateTime.Now.ToString("HHmm").Trim()
                                || DateTime.Now.ToString("HHmm").Trim() == "2345")
                            {
                                _logger.WriteLog(Application.StartupPath, "Service", "On Process",String.Format("Executing  : {0}{1}{2} {3}", _consolePath ,Slash ,_consoleName , GETDEVICELOGS), true);
                                ProcessCreator(_consolePath + Slash + _consoleName, GETDEVICELOGS + (_BIOMETRICSychTime ? ("_" + SETDEVICETIME) : ""));
                                _logger.WriteLog(Application.StartupPath, "Service", "Console Name : " + _consoleName, "Started", true);
                                break;    
                            }
                        }
                        if (DateTime.Now.ToString("HHmm").Trim() == "2355" || DateTime.Now.ToString("HHmm").Trim() == "2359") //HARDCODED TO GET MIN MAX AT THE END OF THE DAY 11:55 PM AND 11:59 PM
                        {
                            _logger.WriteLog(Application.StartupPath, "Service", "On Process", String.Format("Executing  : {0}{1}{2} {3}", _consolePath, Slash, _consoleName, GETMINMAXLOGS), true);
                            ProcessCreator(_consolePath + Slash + _consoleName, GETMINMAXLOGS);
                            _logger.WriteLog(Application.StartupPath, "Service", "Console Name : " + _consoleName, "Started", true);
                            Thread.Sleep(5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteLog(Application.StartupPath, "Service", "Console Name : " + _consoleName, ex.ToString(), true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(Application.StartupPath, "Service", "Error", ex.ToString(), true);
            }
        }

        private void ProcessCreator(String ConsoleFile, String Argument)
        {
            ProcessStartInfo info = new ProcessStartInfo(ConsoleFile, Argument);
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            info.ErrorDialog = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = Process.Start(info);   
        }
    }
}
