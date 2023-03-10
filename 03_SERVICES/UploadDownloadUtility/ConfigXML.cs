using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using CommonPostingLibrary;

namespace UploadDownloadUtility
{

    public class ConfigXML 
    {

    }
    public class AppConfigFile
    {
        public string XmlKey;
        public string XmlValue;
        public void SetXmlKey(string strXmlKey)
        {
            this.XmlKey = strXmlKey;
        }

        public void SetXmlValue(string strXmlValue) 
        {
            this.XmlValue = strXmlValue;
        }
    }

    public class ProgramConfigFile
    {
        public string ProgramKey;
        public string ProgramValue;

        public void SetKey(string strKey)
        {
            ProgramKey = strKey;
        }

        public void SetValue(string strValue)
        {
            ProgramValue = strValue;
        }
    }

    public class AppConfigXML 
    {
        private string AppConfigPath;
        public List<AppConfigFile> AppConfigList;
        private string[] encryptData = new string[7] { "ReportServerUsername"
                                                    , "ReportServerPassword"
                                                    , "DataSource"
                                                    , "CentralDBName"
                                                    , "DBNameDTR"
                                                    , "UserID"
                                                    , "Password"};

        public AppConfigXML()
        {
            this.AppConfigList = new List<AppConfigFile>();
            this.AppConfigPath = "";
        }

        public bool LoadAppConfigXMLFile(string AppConfigPath)
        {
            this.AppConfigPath = AppConfigPath;
            bool ret = false;

            if (File.Exists(AppConfigPath))
            {
                AppConfigList.Clear(); //reset list

                XmlReader reader = XmlReader.Create(AppConfigPath);

                AppConfigFile appConfig;
                while (reader.Read())
                {
                    if (reader.Name == "add")
                    {
                        if (reader.GetAttribute("key") != null && reader.GetAttribute("value") != null)
                        {
                            appConfig = CreateAppConfig(reader.GetAttribute("key"), reader.GetAttribute("value"));
                            InsertAppConfig(appConfig);
                        }
                    }
                }
                reader.Close();
                ret = true;
            }
            else
            {
                MessageBox.Show("Could not find file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ret = false;
            }

            return ret;
        }

        

        private void InsertAppConfig(AppConfigFile programConfig)
        {
            AppConfigList.Add(programConfig);
        }

        private AppConfigFile CreateAppConfig(string Key, string Value)
        {
            foreach (string data in encryptData)
            {
                if (Key == data)
                {
                    Value = Encrypt.decryptText(Value);
                    break;
                }
            }

            AppConfigFile programConfigFile = new AppConfigFile();
            programConfigFile.SetXmlKey(Key);
            programConfigFile.SetXmlValue(Value);
            return programConfigFile;
        }

        public void SaveAllXMLFile(List<ProgramConfigFile> rowsOrig, DataGridViewRowCollection rowsCopy, List<string> keysOrig, List<string> keysCopy, string configPath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            FileAttributes attributes = File.GetAttributes(@configPath);
            File.SetAttributes(@configPath, FileAttributes.Normal);
            using (XmlWriter writer = XmlWriter.Create(@configPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("configuration");

                writer.WriteStartElement("configSections");
                writer.WriteEndElement();

                writer.WriteStartElement("connectionStrings");

                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "PayrollConnectionString");
                writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                writer.WriteEndElement();

                //writer.WriteStartElement("add");
                //writer.WriteAttributeString("name", "PayrollConfiConnectionString");
                //writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                //writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                //writer.WriteEndElement();

                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "dtrConnectionString");
                writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteStartElement("appSettings");

                //write from original/source
                for (int i = 0; i < rowsOrig.Count; i++)
                {
                    string Key = rowsOrig[i].ProgramKey;
                    string Value = rowsOrig[i].ProgramValue;

                    foreach (string key in keysOrig)
                    {
                        if (Key == key)
                        {

                            foreach (string data in encryptData)
                            {
                                if (Key == data)
                                {
                                    Value = Encrypt.encryptText(Value);
                                }
                            }
                            //Add Profile
                            writer.WriteStartElement("add");

                            writer.WriteAttributeString("key", Key);
                            writer.WriteAttributeString("value", Value);

                            writer.WriteEndElement();

                            break;
                        }
                    }
                }

                //write from copy
                for (int i = 0; i < rowsCopy.Count; i++)
                {
                    string Key = rowsCopy[i].Cells["PayrollGenieKey"].Value.ToString();
                    string Value = rowsCopy[i].Cells["PayrollGenieValue"].Value.ToString();

                    foreach (string key in keysCopy)
                    {
                        if (Key == key)
                        {
                            //Add Profile
                            writer.WriteStartElement("add");

                            writer.WriteAttributeString("key", Key);
                            writer.WriteAttributeString("value", Value);

                            writer.WriteEndElement();

                            break;
                        }
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            File.SetAttributes(@configPath, attributes);
        }

        public void SaveXMLFile(DataGridViewRowCollection rows)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            FileAttributes attributes = File.GetAttributes(@AppConfigPath);
            File.SetAttributes(@AppConfigPath, FileAttributes.Normal);
            using (XmlWriter writer = XmlWriter.Create(@AppConfigPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("configuration");

                writer.WriteStartElement("configSections");
                writer.WriteEndElement();

                writer.WriteStartElement("connectionStrings");

                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "PayrollConnectionString");
                writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                writer.WriteEndElement();

                //writer.WriteStartElement("add");
                //writer.WriteAttributeString("name", "PayrollConfiConnectionString");
                //writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                //writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                //writer.WriteEndElement();

                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "dtrConnectionString");
                writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                writer.WriteEndElement();

                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "ProximityConfiConnectionString");
                writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                writer.WriteEndElement();

                writer.WriteStartElement("add");
                writer.WriteAttributeString("name", "rsConnectionString");
                writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteStartElement("appSettings");

                for (int i = 0; i < rows.Count; i++)
                {
                    string Key = rows[i].Cells["dataGridKey"].Value.ToString();
                    string Value = rows[i].Cells["dataGridValue"].Value.ToString();

                    foreach (string data in encryptData)
                    {
                        if (Key == data)
                        {
                            Value = Encrypt.encryptText(Value);
                            break;
                        }
                    }
                    //Add Profile
                    writer.WriteStartElement("add");

                    writer.WriteAttributeString("key", Key);
                    writer.WriteAttributeString("value", Value);

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            File.SetAttributes(@AppConfigPath, attributes);
        }

        public void SaveXMLFile(DataGridViewRowCollection rows,string dataSource, string profile, string nonConfi, string confi, string dtr, bool otMidnight, bool defaultShift, bool autoChangeShift,string user, string password) 
        {
            try 
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                FileAttributes attributes = File.GetAttributes(@AppConfigPath);
                File.SetAttributes(@AppConfigPath, FileAttributes.Normal);
                using (XmlWriter writer = XmlWriter.Create(@AppConfigPath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("configuration");

                    writer.WriteStartElement("configSections");
                    writer.WriteEndElement();

                    writer.WriteStartElement("connectionStrings");

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("name", "PayrollConnectionString");
                    writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                    writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                    writer.WriteEndElement();

                    //writer.WriteStartElement("add");
                    //writer.WriteAttributeString("name", "PayrollConfiConnectionString");
                    //writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                    //writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                    //writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("name", "dtrConnectionString");
                    writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                    writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("name", "ProximityConfiConnectionString");
                    writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                    writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("name", "rsConnectionString");
                    writer.WriteAttributeString("connectionString", "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}");
                    writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteStartElement("appSettings");

                    for (int i = 0; i < rows.Count; i++)
                    {
                        string Key = rows[i].Cells["dataGridKey"].Value.ToString();
                        string Value = rows[i].Cells["dataGridValue"].Value.ToString();
                        switch(Key)
                        {
                            case "DataSource":
                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(dataSource);
                                        break;
                                    }
                                    else
                                    {
                                        Value = dataSource;
                                    }

                                }
                                break;
                            case "CentralDBName":
                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(profile);
                                        break;
                                    }
                                    else
                                    {
                                        Value = profile;
                                    }

                                }
                                break;
                            case "DBNameConfi":
                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(confi);
                                        break;
                                    }
                                    else
                                    {
                                        Value = confi;
                                    }

                                }
                                break;
                            case "DBNameNonConfi":

                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(nonConfi);
                                        break;
                                    }
                                    else
                                    {
                                        Value = nonConfi;
                                    }

                                }

                                break;
                            case "DBNameDTR":

                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(dtr);
                                        break;
                                    }
                                    else
                                    {
                                        Value = dtr;

                                    }

                                }

                                break;
                            case "OTAfterMidnight":
                                if (otMidnight == true)
                                {
                                    Value = "TRUE";
                                }
                                else 
                                {
                                    Value = "FALSE";
                                }
                                break;
                            case "DefaultShift":
                                if (defaultShift == true)
                                {
                                    Value = "TRUE";
                                }
                                else 
                                {
                                    Value = "FALSE";
                                }
                                break;
                            case "AutoChangeShift":
                                if (autoChangeShift == true)
                                {
                                    Value = "TRUE";
                                }
                                else 
                                {
                                    Value = "FALSE";
                                }
                                break;
                            case "UserID":

                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(user);
                                        break;
                                    }
                                    else
                                    {
                                        Value = user;

                                    }

                                }

                                break;
                            case "Password":

                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(password);
                                        break;
                                    }
                                    else
                                    {
                                        Value = password;

                                    }

                                }

                                break;

                            default:
                                foreach (string data in encryptData)
                                {
                                    if (Key == data)
                                    {
                                        Value = Encrypt.encryptText(Value);
                                        break;
                                    }
                                }
                                break;

                        }

                   
                        //Add Profile
                        writer.WriteStartElement("add");

                        writer.WriteAttributeString("key", Key);
                        writer.WriteAttributeString("value", Value);

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                File.SetAttributes(@AppConfigPath, attributes);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}