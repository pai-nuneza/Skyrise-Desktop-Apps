using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace TimeKeepingManager.Con
{
    public class Logger
    {
        public void WriteLog(string location, string filename,string header,string msg, bool AppendTxt)
        {
            try
            {
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
                
            }
            catch
            {
                return;
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
    }
}
