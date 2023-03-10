using System;
using System.Configuration;
using System.Windows.Forms;
using System.Diagnostics;

namespace UploadDownloadSchedulerConsole
{
    public class SchedulerConsole
    {
        public static void Main(string[] args)
        {
            #region Console Notes
            if (args.Length==0)
            {
                Console.WriteLine(@"*************************************   HRMIS Services   *****************************************"
                                   + "\nNote : Set configurations" 
                                   + "\n[UploadDownloadSchedulerConsole.exe.config]"
                                   + "\n[UploadDownloadService.exe.config]"
                                   + "\n\nService Trigger Codes:"
                                   + "\n1. LOGUPLOADING"
                                   + "\n2. EWSSNOTIFICATION"
                                   + "\n3. UNPAIREDLOGS"
                                   + "\n\nHRMIS Services Manual Trigger?\n"
                                   + "\n1. Run Command Prompt as ADMINISTRATOR"
                                   + "\n2. Type on the command prompt the ff:"
                                   + "\n[INSTALLED DIRECTORY]\\[Executable.exe] [SERVICE CODE]"
                                   +"\n\nSample : "
                                   + "\n\nC:\\Program Files (x86)\\HRMIS Services\\UploadDownloadSchedulerConsole.exe LOGUPLOADING"
                                   + "\n\n\nAdditional Features :"
                                   +"\n\n============    Run Log Posting with specified date   ==============="  
                                   +"\n[INSTALLED DIRICTORY]\\UploadDownloadSchedulerConsole.exe LOGUPLOADING <DATE>"
                                   +"\n\nSample :"
                                   +"\n[INSTALLED DIRICTORY]\\UploadDownloadSchedulerConsole.exe LOGUPLOADING 01/30/2012"
                                   + "\n\n=========    Run Log Posting with specified date range   ==========="
                                   +"\n[INSTALLED DIRICTORY]\\UploadDownloadSchedulerConsole.exe LOGUPLOADING <DATESTART> <DATEEND>"
                                   +"\n\nSample :"
                                   + "\n[INSTALLED DIRICTORY]\\UploadDownloadSchedulerConsole.exe LOGUPLOADING 01/25/2019 01/30/2019"
                                   + "\n\n=============  Run Log Posting without overwritting   ==============="
                                   +"\n[INSTALLED DIRICTORY]\\UploadDownloadSchedulerConsole.exe LOGUPLOADING <DATESTART> <DATEEND> 0"
                                   +"\n\nSample :"
                                   + "\n[INSTALLED DIRICTORY]\\UploadDownloadSchedulerConsole.exe LOGUPLOADING 01/25/2019 01/30/2019 0"
                                   + "\n\n********************************************************************************************");

                //Console.SetWindowSize(100,55);
                //Console.SetWindowPosition(0, 0);
                Console.ReadLine();
                return;
            }
            #endregion

            string param = args[0];
            if (param != null)
            {
                NLLogger.Logger _log = new NLLogger.Logger();

                if (string.Compare(param, "LOGUPLOADING", true) == 0)
                {
                    #region Log Posting
                    if (IsProcessRunning())
                        return;
                    conLogUploading log = new conLogUploading();
                    log.UploadTheLogs(args);
                    #endregion
                }
                else if (param.ToUpper().Equals("EWSSNOTIFICATION"))
                {
                    #region EWSS Notification
                    try
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "EWSS nofication STARTED: Via Schedule", "", true);
                        conEWSSNotification conEWSSNotification = new conEWSSNotification();
                        conEWSSNotification.runEWSSNotification();
                        _log.WriteLog(Application.StartupPath, "Posting", "EWSS nofication COMMITED: Via Schedule", "", true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "EWSSError", "conEWSSNotification : RollBack", e.ToString(), true);
                    }
                    #endregion
                }
                else if (param.ToUpper().Equals("UNPAIREDLOGS"))
                {
                    #region Unpaired Logs Per Employee
                    try
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "Unpaired Logs Download STARTED: Via Schedule", "", true);
                        conUnpairedLogs conUnpairedLogs = new conUnpairedLogs();
                        conUnpairedLogs.runUnpairedLogsReport();
                        _log.WriteLog(Application.StartupPath, "Posting", "Unpaired Logs Download COMMITED: Via Schedule", "", true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "UnpairedLogsError", "conUnpairedLogs : RollBack", e.ToString(), true);
                    }
                    #endregion
                }
                else if (string.Compare(param, "MULTIPOCKETS", true) == 0)
                {
                    #region Multi Pocket Function
                    try
                    {
                        if (IsProcessRunning())
                            return;
                        _log.WriteLog(Application.StartupPath, "Posting", "MULTIPOCKET Posting STARTED: Via Schedule", "", true);
                        MultiPackets.MultiPocketVar._ServiceCode = "MULTIPOCKETS";
                        MultiPackets.LogUploadingMultiPockets multipacketuploading = new MultiPackets.LogUploadingMultiPockets();
                        multipacketuploading.UploadMutiPockets(args);
                        _log.WriteLog(Application.StartupPath, "Posting", "MULTIPOCKET Posting COMMITED: Via Schedule", "", true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "MULTIPOCKETS : RollBack", e.ToString(), true);
                    }
                    #endregion
                }
                else if (param.ToUpper().Equals("GENERICEMAIL"))
                {
                    #region Generic Email
                    if (args[1] != null && args[1].ToString().Trim() != string.Empty)
                    {
                        try
                        {
                            DateTime datetemp = Convert.ToDateTime(args[1].ToString().Trim().Replace("-", " "));
                            conGenericEmail EmailSending = new conGenericEmail();
                            EmailSending.ProcessEmail(datetemp);
                        }
                        catch
                        {
                        }
                    }
                    #endregion
                }
                else if (param.ToUpper().Equals("EWSSSUMMARYNOTIF"))
                {
                    #region EWSS Summary Notification
                    try
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "EWSS Summary nofication STARTED: Via Schedule", "", true);

                        conEWSSNotification conEWSSSummaryNotification = new conEWSSNotification();
                        conEWSSSummaryNotification.runEWSSSummaryNotification();
                        _log.WriteLog(Application.StartupPath, "Posting", "EWSS Summary nofication COMMITED: Via Schedule", "", true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "EWSSError", "conEWSSSummaryNotification : RollBack", e.ToString(), true);
                    }
                    #endregion
                }
            }
        }

        private static bool IsProcessRunning()
        {
            bool retVal = false;
            Process[] prc_existingtimekeepers = Process.GetProcessesByName("UploadDownloadSchedulerConsole");
            //foreach (Process prc in prc_existingtimekeepers)
            //{
            //    //_log.WriteLog(Application.StartupPath, "PID", "", prc.Id.ToString(), true);
            //}

            if (prc_existingtimekeepers.Length > 2)
            {
                Console.WriteLine("Another process is still on progress");
                retVal = true; //Wait until existing process is done
            }

            return retVal;
        }
    }
}
