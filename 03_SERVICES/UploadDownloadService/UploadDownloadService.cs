using System;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Posting.BLogic;
using System.Configuration;
using System.IO;

namespace UploadDownloadService
{
    public partial class UploadDownloadService : ServiceBase
    {
        //System.Timers.Timer oTimer = null;
        //double interval = 5000;
        public UploadDownloadService()
        {
            InitializeComponent();
            //InitializeService();
        }

        //void InitializeService()
        //{
        //    oTimer = new System.Timers.Timer(interval);
        //    oTimer.Enabled = true;
        //    oTimer.AutoReset = true;
        //    oTimer.Elapsed += new System.Timers.ElapsedEventHandler(oTimer_Elapsed);
        //}

        //void oTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    ClockTick(sender, e);
        //}

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            //EventLog.WriteEntry("My simple service started.");
            try
            {
                System.Timers.Timer t = new System.Timers.Timer();
                t.Interval = 60000;//1800000 every 30 minutes
                t.Start();
                GC.KeepAlive(t);
                t.Elapsed += new System.Timers.ElapsedEventHandler(ClockTick);
            }
            catch (Exception ex)
            {
                GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], ex.Message);
            }
        }

        protected void ClockTick(object obj, ElapsedEventArgs e)
        {
            DateTime dateEmailService = DateTime.Now;
            //End

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Mss_{0}{1}", DateTime.Now.Hour.ToString().PadLeft(2, '0').Substring(0, 2), DateTime.Now.Minute.ToString().PadLeft(2, '0').Substring(0, 2));
            //loop the Service timing database
            try
            {
                if (DateTime.Now.Minute == 30 || DateTime.Now.Minute == 0 || DateTime.Now.Minute == 15 || DateTime.Now.Minute == 45)
                {
                    LogUploadingBL ud = new LogUploadingBL();
                    DataTable dt = ud.GetServiceCode(sb.ToString());
                    //check if needed for logging
                    //if (ConfigurationManager.AppSettings["Logging"].ToUpper().Trim() == "ON")
                    //{
                    if (dt != null)
                    {
                        if (dt.Rows.Count != 0)
                        {
                            foreach (DataRow dr in dt.Rows)//loop return service code
                            {
                                ProcessToStart(dr[0].ToString());
                                //ProcessToStart("JOBSPLIT");
                                GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], "Service Code " + dr[0].ToString() + " was successfully called", true);
                                //call the console application
                            }
                        }
                        //else
                        // GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], "Service Code Cannot Be found");

                    }
                    //else
                    //GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], "Service Code Cannot Be found");
                    //}

                }

                //ProcessGenericEmail(dateEmailService);

            }
            catch (Exception ex)
            {
                //Generate textfile
                GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], ex.Message);
            }
        }

        public static void ProcessToStart(string serviceCode)
        {
            GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], "Current Processed Service Code " + serviceCode.ToUpper().Trim(), true);
            try
            {
                Process NewProcess = new Process();
                //ProcessStartInfo StartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["ConsolePath"] + ConfigurationManager.AppSettings["ConsoleName"], StringEnum.GetStringValue(CommonEnum.ConsoleArguments.MasterFiles));
                ProcessStartInfo StartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["ConsolePath"] + ConfigurationManager.AppSettings["ConsoleName"], serviceCode.ToUpper().Trim());
                StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                NewProcess.StartInfo = StartInfo;
                NewProcess.Start();

                NewProcess.Close();
            }
            catch (Exception ex)
            {
                GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], ex.Message);
            }
            //finally {
            //    GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], "Current Processed Service Code " + serviceCode.ToUpper().Trim());

            //}
        }

        public static void ProcessGenericEmail(DateTime datetemp)
        {
            GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], "Current Processed Service Code Generic Email " + datetemp.ToString("MM/dd/yyyy HH:mm"), true);
            try
            {
                Process NewProcess = new Process();
                ProcessStartInfo StartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["ConsolePath"] + ConfigurationManager.AppSettings["ConsoleName"], "GENERICEMAIL " + datetemp.ToString("MM/dd/yyyy-HH:mm"));
                StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                NewProcess.StartInfo = StartInfo;
                NewProcess.Start();

                NewProcess.Close();
            }
            catch (Exception ex)
            {
                GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], ex.Message);
            }
        }

        public static string GenerateTextFile(string strFileName, string strPath, string strText, bool flag)
        {
            if (flag)
            {
                string strFullFileName;
                System.IO.FileStream fs;
                System.Threading.Mutex file = new System.Threading.Mutex(false);

                if (!strPath.EndsWith(@"\"))
                    strPath += @"\";

                strFullFileName = String.Format("{0}{1}", strPath, strFileName);

                if (!System.IO.Directory.Exists(strPath))
                {
                    System.IO.Directory.CreateDirectory(strPath);
                }

                try
                {
                    file.WaitOne();

                    if (File.Exists(strFullFileName))
                        fs = new FileStream(strFullFileName, FileMode.Append, FileAccess.Write);
                    else
                        fs = new FileStream(strFullFileName, FileMode.OpenOrCreate, FileAccess.Write);

                    StreamWriter sw = new StreamWriter(fs);

                    sw.Write(DateTime.Now.ToString("s"));
                    sw.Write(sw.NewLine);
                    sw.Write(strText);
                    sw.Write(sw.NewLine);
                    sw.Write(sw.NewLine);


                    sw.Close();
                    fs.Close();
                }
                catch
                {
                }
                finally
                {
                    file.Close();
                }
            }
            return strFileName;
        }

        public static string GenerateTextFile(string strFileName, string strPath, string strText)
        {
            string strFullFileName;
            System.IO.FileStream fs;
            System.Threading.Mutex file = new System.Threading.Mutex(false);

            if (!strPath.EndsWith(@"\"))
                strPath += @"\";

            strFullFileName = String.Format("{0}{1}", strPath, strFileName);

            if (!System.IO.Directory.Exists(strPath))
            {
                System.IO.Directory.CreateDirectory(strPath);
            }

            try
            {
                file.WaitOne();

                if (File.Exists(strFullFileName))
                    fs = new FileStream(strFullFileName, FileMode.Append, FileAccess.Write);
                else
                    fs = new FileStream(strFullFileName, FileMode.OpenOrCreate, FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs);

                sw.Write(DateTime.Now.ToString("s"));
                sw.Write(sw.NewLine);
                sw.Write(strText);
                sw.Write(sw.NewLine);
                sw.Write(sw.NewLine);


                sw.Close();
                fs.Close();
            }
            catch
            {
            }
            finally
            {
                file.Close();
            }

            return strFileName;
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
