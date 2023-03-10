using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Configuration;
using System.IO;

namespace CommonPostingLibrary
{
    public class ParseTextFile
    {
        string Path = string.Empty;
        string tableName = string.Empty;
        string delimeter = string.Empty;


        public ParseTextFile()
        {
        }

        public DataSet Parse()
        {
            if (!(this.Path.Length < 1 || this.tableName.Length < 1 || this.delimeter.Length < 1))
            {
                try
                {
                    DataSet ds = new DataSet();
                    StreamReader sr = new StreamReader(Path);
                    int MaxRows = 0;
                    string wholeRecord = sr.ReadToEnd();
                    string[] totalRecords = wholeRecord.Split("\r\n".ToCharArray());
                    foreach (string total in totalRecords)
                    {
                        string[] splitted = total.Split(delimeter.ToCharArray());
                        int noRows = splitted.Length;
                        if (noRows > MaxRows)
                            MaxRows = noRows;
                    }
                    sr.Close();
                    sr = new StreamReader(Path);
                    string[] columns = sr.ReadLine().Split(delimeter.ToCharArray());
                    columns = sr.ReadLine().Split(delimeter.ToCharArray());
                    ds.Tables.Add(TableName);

                    for (int i = 0; i < MaxRows; i++)
                    {
                        string columnName = "Column_" + i;
                        ds.Tables[TableName].Columns.Add(columnName);
                    }

                    sr.Close();
                    sr = new StreamReader(Path);

                    string AllData = sr.ReadToEnd();
                    string[] rows = AllData.Split("\r\n".ToCharArray());
                    foreach (string r in rows)
                    {
                        string[] items = r.Split(delimeter.ToCharArray());
                        if (r != "")
                            ds.Tables[TableName].Rows.Add(items);
                    }
                    sr.Close();
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                return null;
        }

        public string FilePath
        {
            get
            {
                return this.Path;
            }
            set
            {
                this.Path = value;
            }
        }

        public string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                this.tableName = value;
            }
        }

        public string Delimeter
        {
            get
            {
                return delimeter;
            }
            set
            {
                this.delimeter = value;
            }
        }
    }
}
