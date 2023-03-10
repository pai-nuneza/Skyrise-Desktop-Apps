using System;
using System.Collections.Generic;

//using System.Linq;
using System.Text;
using System.IO;

namespace NLLogger
{
    class Logger
    {
        public void WriteLog(string location, string filename, string header, string msg, bool AppendTxt)
        {
            try
            {
                String IsCleanup = DateTime.Now.ToString("M-d");
                if (IsCleanup == "1-1")
                {
                    // delete directory
                    if (System.IO.Directory.Exists(location + @"\\Logged") == true)
                    {
                        Directory.Delete(location + @"\Logged\", true);
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
                string myLog = location + "\\" + "~" + filename + ".log";
                FileStream FS = null;

                //write or append txt
                if (!AppendTxt)
                {
                    if (File.Exists(myLog))
                    {
                        File.Delete(myLog);
                    }
                    using (FS = File.Create(myLog))
                    {
                    }
                    FS.Close();
                    StreamWriter TXT_WRITE = new StreamWriter(myLog);
                    if(!string.IsNullOrEmpty(header.Trim()))
                        TXT_WRITE.WriteLine(header);
                    TXT_WRITE.WriteLine(msg);
                    TXT_WRITE.Close();
                }
                else
                {
                    FileStream FSAppend = new FileStream(myLog, FileMode.Append, FileAccess.Write);
                    StreamWriter TXT_WRITE = new StreamWriter(FSAppend);
                    if (!string.IsNullOrEmpty(header.Trim()))
                        TXT_WRITE.WriteLine(header);
                    TXT_WRITE.WriteLine(msg);
                    TXT_WRITE.Close();
                    FSAppend.Close();
                }
            }
            catch
            {
            }
        }

        public bool DeleteLog(string location, string filename)
        {
            try
            { 
                location = location + @"\Logged\";

                // create filename
                string myLog = location + "\\" + "~" + filename + ".log";
                FileStream FS = null;

                //delete log
                if (File.Exists(myLog))
                {
                    File.Delete(myLog);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
