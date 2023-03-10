using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;

namespace CommonPostingLibrary
{
    public enum LogType : int
    {
        INFO = 0,
        WARN = 1,
        ERR = 2,
        DEBUG = 9
    }

    public class Logger
    {
        private string logPath = string.Empty;

        public Logger()
        {
        }

        //Arthur Inserted 2006092211AM Start
        public Logger(string logPath)
        {
            this.logPath = logPath;
        }
        //end

        public void loggerMethod(LogType logType, string message)
        {
            string path;
            string logFileName;
            FileStream fs;

            //path = ConfigurationSettings.AppSettings["logPath"];
            //path = System.Windows.Forms.Application.UserAppDataPath;//Arthur Deleted 2006092211AM 

            if (this.logPath.Equals(string.Empty))
                path = System.Windows.Forms.Application.UserAppDataPath;
            else
                path = this.logPath;

            if (!path.EndsWith(@"\"))
                path += @"\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            logFileName = path + "Shimadzu_" + System.DateTime.Now.ToString("MMdd") + ".log";

            try
            {
                if (File.Exists(logFileName))
                    fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
                else
                    fs = new FileStream(logFileName, FileMode.OpenOrCreate, FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine("{0} {1} {2}", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), logType, message);

                sw.Close();
                fs.Close();
            }
            catch
            {
            }
        }
    }
}
