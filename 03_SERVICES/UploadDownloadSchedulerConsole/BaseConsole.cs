using System;
using System.IO;
using System.Configuration;
using System.Data;

namespace UploadDownloadSchedulerConsole
{
    public abstract class BaseConsole
    {
        public BaseConsole()
        {
        }

        public string GenerateTextFile(string strFileName, string strPath, string strText)
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

        //public void SendEmail(string Subject, string MailBody, string Transaction, bool isError)
        //{
        //    try
        //    {
        //        EmailSending em = new EmailSending();
        //        em.To = "melchor-zarcilla@dash.com.ph";
        //        em.Cc = "";//BaseBL.ReturnEmailAdd(Transaction, isError);
        //        em.From = ConfigurationManager.AppSettings["FROM"];
        //        em.Subject = Subject;
        //        em.MailBody = MailBody;

        //        if (isError)
        //            em.Bcc = string.Empty;

        //        em.Send();
        //    }
        //    catch (Exception Ex)
        //    {
        //        GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], Ex.Message);
        //    }
        //}
        ////chip download email
        //public void SendEmail(string Subject, string MailBody, string docType, bool isError, string To)
        //{
        //    try
        //    {
        //        EmailSending em = new EmailSending();
        //        em.To = To;
        //        em.Cc = "";//BaseBL.ReturnEmailAdd(docType,isError);
        //        em.From = "i-merge@fujielectric.com.my";
        //        em.Subject = Subject;
        //        em.MailBody = MailBody;
        //        em.Send();
        //    }
        //    catch (Exception Ex)
        //    {
        //        GenerateTextFile("Log" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ConfigurationManager.AppSettings["LogPath"], Ex.Message);
        //    }
        //}

        public static DataSet ConvertData(string File, string TableName, string delimiter)
        {
            DataSet ds = new DataSet();
            StreamReader sr = new StreamReader(File);
            int MaxRows = 0;
            string wholeRecord = sr.ReadToEnd();
            string[] totalRecords = wholeRecord.Split("\r\n".ToCharArray());
            foreach (string total in totalRecords)
            {
                string[] splitted = total.Split(delimiter.ToCharArray());
                int noRows = splitted.Length;
                if (noRows > MaxRows)
                    MaxRows = noRows;
            }
            sr.Close();
            sr = new StreamReader(File);
            string[] columns = sr.ReadLine().Split(delimiter.ToCharArray());
            columns = sr.ReadLine().Split(delimiter.ToCharArray());
            ds.Tables.Add(TableName);

            for (int i = 0; i < MaxRows; i++)
            {
                string columnName = "Column_" + i;
                ds.Tables[TableName].Columns.Add(columnName);
            }

            sr.Close();
            sr = new StreamReader(File);//read from start again ky na read naman ang first line sa pagbuhat ug header name

            string AllData = sr.ReadToEnd();
            string[] rows = AllData.Split("\r\n".ToCharArray());
            foreach (string r in rows)
            {
                string[] items = r.Split(delimiter.ToCharArray());
                if (r != "")
                    //to avoid rows with blank values
                    ds.Tables[TableName].Rows.Add(items);
            }
            sr.Close();
            return ds;
        }

        public void TruncateLocalDirectory(string docType)
        {
            string[] FileList = Directory.GetFiles(ConfigurationManager.AppSettings["LocalDestination"], docType + "*");
            foreach (string filename in FileList)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }

            FileList = Directory.GetFiles(ConfigurationManager.AppSettings["LocalArchive"], docType + "*");
            foreach (string filename in FileList)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        protected bool isInteger(string strToCheck)
        {
            string Pattern = @"(?n)^(\d*)$";
            return System.Text.RegularExpressions.Regex.IsMatch(strToCheck, Pattern);
        }

        protected bool isDecimal(string strToCheck)
        {
            if (strToCheck == "0")
                return true;
            string Pattern = @"(?n)^((\d*)\.(\d{3})-?)$";
            return System.Text.RegularExpressions.Regex.IsMatch(strToCheck, Pattern);
        }

        protected string GetValue(object objVal)
        {
            if (objVal != null)
                return objVal.ToString().Trim();
            else
                return String.Empty;
        }
    }
}
