using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using CommonLibrary;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Payroll.DAL;

namespace TimeKeepingManager.Con
{
    public class Logger
    {
        public bool WriteLog(string location, string filename,string header,string msg, bool AppendTxt)
        {
            try
            {
                string strOrigHeader = header;
                String IsCleanup = DateTime.Now.ToString("M-d");
                if (IsCleanup == "1-1") 
                {
                    // delete directory
                    if (System.IO.Directory.Exists(location + @"\\Logged") == true)
                    {
                        Directory.Delete(location + @"\Logged\",true);
                    }
                }
                if (header == "")
                    header = "";
                else
                header = string.Format(@"=============================================================================
                           {0}Function Name : {1}
                           {0}Date/Time : {2}
                           {0}Msg : {0}", Environment.NewLine, header, DateTime.Now);
             
                // create directory
                if (System.IO.Directory.Exists(location + @"\\Logged") != true)
                {
                    Directory.CreateDirectory(location + @"\Logged\");
                }
                location = location + @"\Logged\";

                // create filename
                filename = filename + DateTime.Now.ToString("-MM-dd-yyyy"); //Create new log file daily.
                string myLog = location + "\\" + "~"+filename + ".log";
                FileStream FS = null;

                //write or append txt
                if (!AppendTxt)
                {
                    if (File.Exists(myLog))
                    {
                        File.Delete(myLog);
                    }
                    using (FS = File.Create(myLog)) { }
                    FS.Close();
                    StreamWriter TXT_WRITE = new StreamWriter(myLog);
                    TXT_WRITE.WriteLine(header);
                    TXT_WRITE.WriteLine(msg);
                    TXT_WRITE.Close();
                }
                else
                {
                    FileStream FSAppend = new FileStream(myLog, FileMode.Append, FileAccess.Write);
                    StreamWriter TXT_WRITE = new StreamWriter(FSAppend);
                    TXT_WRITE.WriteLine(header);
                    TXT_WRITE.WriteLine(msg);
                    TXT_WRITE.Close();
                    FSAppend.Close();
                }

                try
                {
                    #region Get IP Address
                    IPHostEntry host;
                    string localIP = "";
                    host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (IPAddress ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            localIP = ip.ToString();
                            break;
                        }
                    }

                    if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                        localIP = "";
                    #endregion

                    #region Get Mac Address
                    ArrayList MacAddresses = new ArrayList();
                    string MacAddress = string.Empty;

                    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        // Only consider Ethernet network interfaces, thereby ignoring any
                        if (nic.OperationalStatus == OperationalStatus.Up)
                        {
                            switch (nic.NetworkInterfaceType)
                            {
                                case NetworkInterfaceType.Ethernet:
                                case NetworkInterfaceType.Ethernet3Megabit:
                                case NetworkInterfaceType.FastEthernetT:
                                case NetworkInterfaceType.FastEthernetFx:
                                case NetworkInterfaceType.Wireless80211:
                                case NetworkInterfaceType.GigabitEthernet:
                                    MacAddresses.Add(nic.GetPhysicalAddress().ToString());
                                    break;
                                default:
                                    break;
                            }

                            if (MacAddresses.Count != 0)
                                MacAddress = MacAddresses[0].ToString().Trim();
                            else
                                MacAddress = "";
                        }
                    }
                    #endregion

                    using (DALHelper dal = new DALHelper(true))
                    {
                        dal.OpenDB();
                        dal.ExecuteNonQuery(string.Format(@"INSERT INTO T_TKSysLog VALUES ('{0}', '{1}', '{2}', '{3}', GETDATE())", LoginInfo.getUser().CompanyCode, localIP, MacAddress, strOrigHeader));
                        dal.CloseDB();
                    }
                }
                catch { }

                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool WriteText(string Folder, string FileName, string Content, bool AppendTxt)
        {
            try
            {
                // create directory
                if (!System.IO.Directory.Exists(Folder))
                {
                    Directory.CreateDirectory(Folder);
                }

                FileName = Folder + FileName;
                
                FileStream FS = null;
                //write or append txt
                if (!AppendTxt)
                {
                    if (File.Exists(FileName))
                    {
                        File.Delete(FileName);
                    }
                    using (FS = File.Create(FileName)) { }
                    FS.Close();
                    StreamWriter TXT_WRITE = new StreamWriter(FileName);
                    TXT_WRITE.WriteLine(Content);
                    TXT_WRITE.Close();
                }
                else
                {
                    FileStream FSAppend = new FileStream(FileName, FileMode.Append, FileAccess.Write);
                    StreamWriter TXT_WRITE = new StreamWriter(FSAppend);
                    TXT_WRITE.WriteLine(Content);
                    TXT_WRITE.Close();
                    FSAppend.Close();
                }

                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool AttLogExist(string location, string filename)
        {
            bool exist = false;
            // create filename
            string myLog = string.Format("{0}\\AttLog\\{1}.tsv.txt", location, filename);
            if (File.Exists(myLog))
            {
                exist = true;
            }
            return exist;
        }
        public void WriteAttLog(string location, string filename, string msg, bool AppendTxt)
        { 
            try
            {
            // create directory
                if (System.IO.Directory.Exists(location + @"\\AttLog") != true)
                {
                    Directory.CreateDirectory(location + @"\AttLog\");
                }
                location = location + @"\AttLog\";

                // create filename
                string myLog = location + "\\" +filename + ".tsv.txt";
                FileStream FS = null;

                //write or append txt
                if (!AppendTxt)
                {
                    if (File.Exists(myLog))
                    {
                        File.Delete(myLog);
                    }
                    using (FS = File.Create(myLog)) { }
                    FS.Close();
                    StreamWriter TXT_WRITE = new StreamWriter(myLog);
                    TXT_WRITE.WriteLine(msg);
                    TXT_WRITE.Close();
                }
                else
                {
                    FileStream FSAppend = new FileStream(myLog, FileMode.Append, FileAccess.Write);
                    StreamWriter TXT_WRITE = new StreamWriter(FSAppend);
                    TXT_WRITE.WriteLine(msg);
                    TXT_WRITE.Close();
                    FSAppend.Close();
                }
                
            }
            catch 
            { 
                return;
            }
        }
        public bool ConfigCopier(String ReadPath, String ReadConfig, String WritePath, String WriteConfig)
        {
            bool copied = false;
            try
            {
                string ReadConfiguration = string.Format("{0}\\{1}.exe.config", ReadPath, ReadConfig);
                string WriteConfiguration = string.Format("{0}\\{1}.exe.config", WritePath, WriteConfig);
                if (!File.Exists(ReadConfiguration))
                {
                    return copied;
                }
                if (File.Exists(WriteConfiguration))
                {
                    File.Copy(ReadConfiguration, WriteConfiguration, true);
                    copied = true;
                }
            }
            catch
            {
                return copied;    
            }
            return copied;
        }

        //Other Config Processor
        public void GetServiceConfiguration(String _SERVICECONFIGPATH)
        {
            try
            {
                LogGlobal.Path = _SERVICECONFIGPATH.Trim();

                if (File.Exists(LogGlobal.Path))
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(LogGlobal.Path.Replace(".config", ""));
                    AppSettingsSection AppSection = (AppSettingsSection)config.GetSection("appSettings");

                    try { LogGlobal.DataSource = AppSection.Settings["DataSource"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.DTRDBName = AppSection.Settings["DTRDBName"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.UserID = AppSection.Settings["UserID"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.Password = AppSection.Settings["Password"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.BIOMETRICConsolePath = AppSection.Settings["BIOMETRICConsolePath"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.BIOMETRICConsoleName = AppSection.Settings["BIOMETRICConsoleName"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.BIOMETRICPeriods = AppSection.Settings["BIOMETRICPeriods"].Value.ToString(); }
                    catch { }
                    try { LogGlobal.BIOMETRICSychTime = AppSection.Settings["BIOMETRICSychTime"].Value.ToString(); }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Service configuration path does not exist. Loading service default value!", "Save");
                }
            }
            catch(Exception Err)
            {

            }
        }
        public String SetServiceConfiguration(String ResourceConfig)
        {
            try
            {
                string ServiceConfigContent = ResourceConfig;
                //local consoles
                //end
                ServiceConfigContent = ServiceConfigContent.Replace("[DataSource]", LogGlobal.DataSource.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[DTRDBName]", LogGlobal.DTRDBName.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[UserID]", LogGlobal.UserID.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[Password]", LogGlobal.Password.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[BIOMETRICConsolePath]", LogGlobal.BIOMETRICConsolePath.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[BIOMETRICConsoleName]", LogGlobal.BIOMETRICConsoleName.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[BIOMETRICPeriods]", LogGlobal.BIOMETRICPeriods.Trim());
                ServiceConfigContent = ServiceConfigContent.Replace("[BIOMETRICSychTime]", LogGlobal.BIOMETRICSychTime.Trim());
                File.WriteAllText(LogGlobal.Path.Trim(), ServiceConfigContent, Encoding.UTF8);
            }
            catch { }
            return LogGlobal.BIOMETRICPeriods;
        }
        /// </summary>
        public void ServiceTrigger(String ServiceName,String command) //cmd = Start   :  Stop  
        {
            try
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + string.Format("SC {0} \"{1}\"", command,ServiceName));
                    //string xx = procStartInfo.ToString();
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    
                    procStartInfo.CreateNoWindow = true;
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                    // Get the output into a string
                    string result = proc.StandardOutput.ReadToEnd();
                    // Display the command output.
                    Console.WriteLine(result);
                      }
                      catch (Exception objException)
                      {
                      // Log the exception
                      }
                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(ServiceName.Trim());
                Thread.Sleep(1000);
                MessageBox.Show(string.Format("{0} : {1}",ServiceName,sc.Status));
                 
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
    }
    #region Logger Global Variables
    public class LogGlobal
    {
        public static string Path = string.Empty;
        public static string DataSource = string.Empty;
        public static string DTRDBName = string.Empty;
        public static string UserID = string.Empty;
        public static string Password = string.Empty;
        public static string BIOMETRICConsolePath = string.Empty;
        public static string BIOMETRICConsoleName = string.Empty;
        public static string BIOMETRICPeriods = string.Empty;
        public static string BIOMETRICSychTime = string.Empty;
        public static string AppPath = Application.StartupPath;
        public static string RFFP = "RFFPLog";

    }
    #endregion
}
